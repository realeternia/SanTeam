using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 共享轮换商店（原商店逻辑搬迁）：8 名玩家共用一份商店，按积分低→高轮流买卡，
/// 可“跳过”，卡牌有售出倒计时与相邻刷新。
/// </summary>
public class SharedShopMode : ShopMode
{
    public override ShopModeType Type => ShopModeType.Shared;

    // 共享商店回合状态
    private bool[] playerPassed = new bool[8]; // 记录每个玩家是否pass过
    private int passedPlayers = 0;             // 记录pass的玩家数量
    private int round = 10000;                 // 回合序号（配合 host.TurnOrder 取当前玩家）

    public override void Begin()
    {
        host.ClearCards();

        var shopCfg = host.ShopCfg;
        List<Tuple<int, int>> heroIds = new List<Tuple<int, int>>();
        int TOTAL_HERO_CARDS = 15;
        // 防死循环：卡池中某品质（第一回合恒为品质1）人数可能不足15张，
        // 命中重复卡会原地打转，故用 while + 尝试上限，凑不满则生成已有的全部不重复卡后结束。
        int totalUnique = 0;
        int maxAttempts = TOTAL_HERO_CARDS * 20;
        int attempt = 0;
        while (totalUnique < TOTAL_HERO_CARDS && attempt < maxAttempts)
        {
            attempt++;
            var heroId = host.GetRandomShopHeroId(shopCfg);
            if (heroId == 0)
                break; // 卡池为空，无法再生成，跳出避免死循环
            var existingIndex = heroIds.FindIndex(x => x.Item1 == heroId);
            if (existingIndex >= 0)
            { // 重复卡的处理
                if (shopCfg.Id > 3)
                {
                    var existingTuple = heroIds[existingIndex];
                    heroIds[existingIndex] = new Tuple<int, int>(existingTuple.Item1, Mathf.Min(existingTuple.Item2 + 1, 2)); // 同一英雄卡最多2张
                }
                continue;
            }

            var count = 1;
            var heroPrice = HeroSelectionTool.GetPrice(HeroConfig.GetConfig(heroId));
            if (shopCfg.MultiPriceTotal > 2 * heroPrice)
            {
                var roll = SysRandom.Range(0, 100);
                if (roll < shopCfg.MultiCardRate)
                    count = SysRandom.Range(1, shopCfg.MultiPriceTotal / heroPrice + 1);

                if (count == 1)
                    count = Math.Max(1, shopCfg.MultiPriceTotal / 3 / heroPrice);
            }

            count = Mathf.Min(count, 2); // 商店同一英雄卡最多出售2张
            heroIds.Add(new Tuple<int, int>(heroId, count));
            totalUnique++;
        }

        for (int i = 0; i < heroIds.Count; i++)
            host.cardViews.Add(host.CreateCardView(heroIds[i].Item1, heroIds[i].Item2));

        host.LayoutCards(host.cardViews, 5, 5);
        host.passBtn.gameObject.SetActive(true);
        host.SetPassButtonLabel("跳过");

        // 重置所有玩家的pass状态
        for (int i = 0; i < playerPassed.Length; i++)
            playerPassed[i] = false;
        passedPlayers = 0;

        ResetRoundOrder();

        var pid = GetTurnPid();
        GameManager.Instance.OnPlayerTurn(pid);
        host.mySelect.UpdateCards(GameManager.Instance.GetPlayer(pid));
    }

    public override PlayerInfo GetActingPlayer()
    {
        return GameManager.Instance.GetPlayer(GetTurnPid());
    }

    public override List<ShopOffer> GetOffers(int pid)
    {
        var list = new List<ShopOffer>();
        foreach (var v in host.cardViews)
        {
            if (!v.isSold)
                list.Add(v.ToOffer());
        }
        return list;
    }

    public override void OnBought(ShopOffer offer, PlayerInfo player)
    {
        if (offer != null && offer.view != null)
            OnCardSelected(offer.view);
    }

    public override void OnHumanBought(PlayerInfo player, ShopOffer offer, int buyCount)
    {
        AfterAct();
    }

    public override void OnHumanPass()
    {
        var nowPlayer = GetActingPlayer();
        if (nowPlayer == null || nowPlayer.isAI)
            return;
        if (playerPassed[nowPlayer.pid])
            return;

        host.passBtn.gameObject.SetActive(false);
        if (playerPassed.All(x => !x))
            host.firstJumper = nowPlayer.pid;
        playerPassed[nowPlayer.pid] = true;
        passedPlayers++;
        nowPlayer.SetRoundOver(true);

        AfterAct();
    }

    // 玩家支付2gold立刻刷新6张牌，可多次进行，不结束自己的回合
    public override void OnHumanRefresh()
    {
        var nowPlayer = GetActingPlayer();
        if (nowPlayer == null || nowPlayer.isAI)
            return;
        if (playerPassed[nowPlayer.pid])
        {
            SystemTip.Show("你已跳过本回合，无法刷新");
            return;
        }
        if (nowPlayer.gold < CardShopManager.RefreshGoldCost)
        {
            SystemTip.Show($"金币不足，刷新需要{CardShopManager.RefreshGoldCost}金币");
            return;
        }

        nowPlayer.gold -= CardShopManager.RefreshGoldCost;
        nowPlayer.goldText.text = nowPlayer.gold.ToString();

        // 从未售出的卡牌中随机选取6张进行刷新（不足6张则全部刷新）
        var unsoldCards = host.cardViews.FindAll(x => !x.isSold);
        for (int i = 0; i < unsoldCards.Count; i++)
        {
            int j = SysRandom.Range(i, unsoldCards.Count);
            var tmp = unsoldCards[i];
            unsoldCards[i] = unsoldCards[j];
            unsoldCards[j] = tmp;
        }

        int refreshCount = Math.Min(6, unsoldCards.Count);
        for (int i = 0; i < refreshCount; i++)
            RefreshCard(unsoldCards[i]);

        GameManager.Instance.PlaySound("Sounds/page");
    }

    public override IEnumerator Drive()
    {
        yield return new WaitForSeconds(.7f);
        host.IsShopEnd = false;
        while (!host.IsShopEnd)
        {
            yield return new WaitForSeconds(SysRandom.Range(0.3f, 0.5f));

            // 有商店以外的面板打开（背包/排行/查看玩家等）时暂停AI选牌，等玩家关闭面板再继续
            if (IsPanelBlocked())
                continue;

            int currentPlayerId = GetTurnPid();

            // 如果当前玩家已经pass，则直接进入下一回合
            if (playerPassed[currentPlayerId])
            {
                NextTurn();
                continue;
            }

            var playerInfo = GameManager.Instance.GetPlayer(currentPlayerId);
            if (playerInfo.isAI)
            {
                var offers = GetOffers(currentPlayerId);
                var result = PlayerAI.AiCheckBuyCard(playerInfo, host.Era, offers);

                if (!result)
                {
                    if (playerPassed.All(x => !x))
                        host.firstJumper = currentPlayerId;
                    // AI玩家放弃购买
                    playerPassed[currentPlayerId] = true;
                    passedPlayers++;
                    playerInfo.SetRoundOver(true);
                }
            }

            // 等待（不阻塞主线程）
            yield return new WaitForSeconds(SysRandom.Range(0.5f, 0.8f));

            if (playerInfo.isAI)
                AfterAct();
        }
    }

    // 当前回合序号对应的玩家pid（回合顺序按积分低到高）
    private int GetTurnPid()
    {
        return host.TurnOrder[round % host.TurnOrder.Length];
    }

    // 依据上回合的“和氏璧买家/首个跳过者”设定本回合起始位
    private void ResetRoundOrder()
    {
        int firstPid = -1;
        if (host.jadePlayer >= 0)
            firstPid = host.jadePlayer;
        else if (host.firstJumper >= 0)
            firstPid = host.firstJumper;

        if (firstPid >= 0)
            round = 8 * 100 + System.Array.IndexOf(host.TurnOrder, firstPid); // 让该玩家排到回合最前
        else
            round = 1000;

        host.jadePlayer = -1;
        host.firstJumper = -1;
    }

    private void NextTurn()
    {
        GameLog.Debug("NextTurn");
        for (int i = 0; i < 8; i++)
        {
            round++;
            var pid = GetTurnPid();
            if (!playerPassed[pid])
            {
                var nextPlayer = GameManager.Instance.GetPlayer(pid);
                host.passBtn.gameObject.SetActive(!nextPlayer.isAI);
                GameManager.Instance.OnPlayerTurn(pid);
                host.mySelect.UpdateCards(nextPlayer);
                return;
            }
        }
    }

    private void AfterAct()
    {
        NextTurn();

        // 只有一轮选牌：所有玩家都跳过时，选牌阶段结束进入战斗
        if (passedPlayers >= 8)
            host.RequestEnd();
    }

    // 玩家选中（购买）一张卡：该卡保持售出状态并设置售出倒计时；相邻卡 round-1，归0立即刷新；每次有其他卡售出，所有已售出卡的倒计时-1，归0刷新
    private void OnCardSelected(CardViewControl ctr)
    {
        // 需要刷新（roundLeft归0）的卡先收集，遍历结束后再统一刷新，避免遍历中修改cardViews
        var toRefresh = new List<CardViewControl>();

        // 相邻未售出卡 round-1，归0立即刷新
        foreach (var adj in GetAdjacentCards(ctr))
        {
            if (adj.isSold)
                continue;
            adj.roundLeft--;
            if (adj.roundLeft <= 0)
                toRefresh.Add(adj);
            else
                adj.UpdateRoundLeft();
        }

        // 刚售出的卡设置售出倒计时
        ctr.roundLeft = CardShopManager.SOLD_REMAIN_ROUNDS;
        ctr.UpdateRoundLeft();

        // 每次有其他卡售出，所有已售出卡的倒计时-1，归0刷新
        foreach (var card in host.cardViews)
        {
            if (!card.isSold || card == ctr)
                continue;
            card.roundLeft--;
            if (card.roundLeft <= 0)
                toRefresh.Add(card);
            else
                card.UpdateRoundLeft();
        }

        foreach (var card in toRefresh)
            RefreshCard(card);
    }

    // 刷新卡位：重新加载prefab生成一张随机新卡（防止复用旧对象导致样式/尺寸残留）
    private void RefreshCard(CardViewControl ctr)
    {
        int index = host.cardViews.IndexOf(ctr);
        if (index < 0)
            return;

        if (!ctr.isHeroCard)
        {
            // 物品仅掉落获得，不参与商店，刷新后不补物品卡
            return;
        }

        var shopCfg = host.ShopCfg;
        // 按当前品质概率随机刷新，允许重复；每张卡有 LikeCardRefreshRate 概率替换为收藏卡
        var heroId = host.GetRandomShopHeroId(shopCfg);
        var heroPrice = HeroSelectionTool.GetPrice(HeroConfig.GetConfig(heroId));
        host.ReplaceCardAt(index, heroId, host.GetMultiCount(heroPrice, shopCfg));
    }

    // 获取一张卡的相邻卡：英雄卡在3列网格中算上下左右
    private List<CardViewControl> GetAdjacentCards(CardViewControl ctr)
    {
        var result = new List<CardViewControl>();
        int index = host.cardViews.IndexOf(ctr);
        if (index < 0)
            return result;

        if (ctr.isHeroCard)
        {
            const int CARDS_PER_ROW = 3;
            int row = index / CARDS_PER_ROW;
            int col = index % CARDS_PER_ROW;
            TryAddAdjacent(result, row - 1, col);
            TryAddAdjacent(result, row + 1, col);
            TryAddAdjacent(result, row, col - 1);
            TryAddAdjacent(result, row, col + 1);
        }
        else
        {
            if (index - 1 >= 0 && !host.cardViews[index - 1].isHeroCard)
                result.Add(host.cardViews[index - 1]);
            if (index + 1 < host.cardViews.Count && !host.cardViews[index + 1].isHeroCard)
                result.Add(host.cardViews[index + 1]);
        }
        return result;
    }

    private void TryAddAdjacent(List<CardViewControl> result, int row, int col)
    {
        if (row < 0 || col < 0 || row > 4 || col > 2)
            return;
        int i = row * 3 + col;
        if (i < host.cardViews.Count && host.cardViews[i].isHeroCard)
            result.Add(host.cardViews[i]);
    }
}
