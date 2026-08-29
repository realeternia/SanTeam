using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Linq;

public class CardShopManager : MonoBehaviour
{
    public static CardShopManager Instance;
    public List<CardViewControl> cardViews = new List<CardViewControl>();

    public GameObject cardViewPrefab; // 拖拽CardView预制体到此处
    public GameObject cardItemViewPrefab; // 拖拽CardView预制体到此处

    private int round = 10000;
    private int[] turnOrder = new int[8]; // 商店回合顺序：turnOrder[回合序号] = pid（按积分低到高排序）
    private bool[] playerPassed = new bool[8]; // 记录每个玩家是否pass过
    private int passedPlayers = 0; // 记录pass的玩家数量
    private const int SOLD_REMAIN_ROUNDS = 5; // 卡售出后维持的round数
    public int[] playerStartGold = new int[8]; // 记录每个玩家开局金币（用于AI跳过判定）

    public Button passBtn;
    public Button refreshBtn;
    public Button bagBtn;
    public Button rankBtn;
    public Button rankPlayerBtn;

    private int era = 0;
    public TMP_Text eraText;
    public MySelectControl mySelect;
    private bool isShopEnd = false;

    public int jadePlayer = -1; //购买和氏璧买家
    public int firstJumper = -1;
    private bool hasEnterBattle = false;

    private Coroutine shopCoroutine;


    // Start is called before the first frame update
    void Start()
    {
        Instance = this;

        passBtn.onClick.AddListener(() =>
        {
            OnP1Pass();
        });

        refreshBtn.onClick.AddListener(() =>
        {
            OnRefresh();
        });

        bagBtn.onClick.AddListener(() =>
        {
            PanelManager.Instance.ShowBag();
            for(int i = 0; i < cardViews.Count; i++)
                cardViews[i].ShowEffectLayer(false);
        });
        rankBtn.onClick.AddListener(() =>
        {
            PanelManager.Instance.ShowRank();
            for(int i = 0; i < cardViews.Count; i++)
                cardViews[i].ShowEffectLayer(false);
        });
        rankPlayerBtn.onClick.AddListener(() =>
        {
            PanelManager.Instance.ShowRankPlayer();
            for(int i = 0; i < cardViews.Count; i++)
                cardViews[i].ShowEffectLayer(false);
        });

        ShopBegin();
    }

    public void OnShow()
    {
        for(int i = 0; i < cardViews.Count; i++)
            cardViews[i].ShowEffectLayer(true);
    }

    private IEnumerator DelayedUpdate()
    { 
        yield return new WaitForSeconds(.7f);
        isShopEnd = false;
        while (!isShopEnd) // 模拟 Update 的循环
        {    
            yield return new WaitForSeconds(UnityEngine.Random.Range(0.3f, 0.5f));

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
                var result = PlayerAI.AiCheckBuyCard(playerInfo, era);
                
                if (!result)
                {
                    if(System.Linq.Enumerable.All(playerPassed, x => !x))
                        firstJumper = currentPlayerId;
                    // AI玩家放弃购买
                    playerPassed[currentPlayerId] = true;
                    passedPlayers++;
                    playerInfo.SetRoundOver(true);
                }
            }

            // 等待 1 秒（不阻塞主线程）
            yield return new WaitForSeconds(UnityEngine.Random.Range(0.5f, 0.8f));

            if (playerInfo.isAI)
            {
                AfterAct();
            }
        }
    }      

    // Update is called once per frame
    void Update()
    {
        
    }

    private void NewEra()
    {
        var movingCardImages = GameObject.FindGameObjectsWithTag("MovingCard");
        foreach (var img in movingCardImages)
            Destroy(img);

        //移除并销毁旧卡片
        foreach (Transform child in transform)
            Destroy(child.gameObject);
        // foreach (Transform child in cardItemView.transform)
        //     Destroy(child.gameObject);
        var unsoldItems = cardViews.FindAll(x => !x.isHeroCard && !x.isSold && !ItemConfig.GetConfig(x.cardId).AutoRemove).ConvertAll(a => a.cardId);
        cardViews.Clear();
        if(era == 0) //第一个回合不存装备
            unsoldItems.Clear();

        foreach(var player in GameManager.Instance.players)
            player.OnEra(era);

        var year = GameManager.Instance.year; //第几场比赛（每场战斗后+1，即一个回合）
        var shopCfg = ShopConfig.GetConfig(Math.Min(100, year));
        List<Tuple<int, int>> heroIds = new List<Tuple<int, int>>();
        int TOTAL_HERO_CARDS = 15;        
        // hero card
        for (int i = 0; i < TOTAL_HERO_CARDS; i++)
        {
            var heroId = HeroSelectionTool.GetRandomHeroIdByQuality(shopCfg);
            var existingIndex = heroIds.FindIndex(x => x.Item1 == heroId);
            if (existingIndex >= 0)
            { //重复卡的处理
                if (shopCfg.Id > 3)
                {
                    var existingTuple = heroIds[existingIndex];
                    heroIds[existingIndex] = new Tuple<int, int>(existingTuple.Item1, existingTuple.Item2 + 1);
                }

                i--;
                continue;
            }

            var count = 1;
            var heroPrice = HeroSelectionTool.GetPrice(HeroConfig.GetConfig(heroId));
            if (shopCfg.MultiPriceTotal > 2 * heroPrice)
            {
                var roll = UnityEngine.Random.Range(0, 100);
                if (roll < shopCfg.MultiCardRate)
                {
                    count = UnityEngine.Random.Range(1, shopCfg.MultiPriceTotal / heroPrice + 1);
                    if(roll >= 95 && shopCfg.ItemAmazingCount > count)
                        count = shopCfg.ItemAmazingCount;
                }

                if (count == 1)
                {
                    count = Math.Max(1, shopCfg.MultiPriceTotal / 3 / heroPrice);
                }
            }

            heroIds.Add(new Tuple<int, int>(heroId, count));
        }


        int CARDS_PER_ROW = 5; // 3列x5行，共15张
        float cardWidth = 228f;
        float cardHeight = 318f;
        float spacing = 5f;

        // 计算起始位置，使其居中显示
        float startX = -((CARDS_PER_ROW * cardWidth) + (CARDS_PER_ROW - 1) * spacing) / 2f + cardWidth / 2f - 50;
        float startY = (5 - 1) / 2f * (cardHeight + spacing) - 320; // 5行垂直居中

        for(int i = 0; i < heroIds.Count; i++)
        {
            var heroId = heroIds[i].Item1;
            var heroCount = heroIds[i].Item2;

            // 计算行和列
            int row = i / CARDS_PER_ROW;
            int col = i % CARDS_PER_ROW;

            // 计算位置
            float x = startX + col * (cardWidth + spacing);
            float y = startY - row * (cardHeight + spacing);

            // 创建CardView实例
            GameObject card = Instantiate(cardViewPrefab, transform);
            RectTransform rectTransform = card.GetComponent<RectTransform>();
            if (rectTransform != null)
                rectTransform.anchoredPosition = new Vector2(x, y);

            CardViewControl cardView = card.GetComponent<CardViewControl>();

            cardView.Init(heroId, true, heroCount, year);
            cardViews.Add(cardView);            
        }

        // 先把ItemConfig里所有RateAbs非0的item随出来，放到一个列表
        var itemIds = new List<int>();
        foreach (var itemCfg in ItemConfig.ConfigList)
        {
            if (itemCfg.RateAbs > 0 && itemCfg.ShopIdx <= shopCfg.Id && UnityEngine.Random.Range(0, 100) < itemCfg.RateAbs)
                itemIds.Add(itemCfg.Id);
        }

        for (int i = 0; i < shopCfg.ItemCount; i++)
        {
            itemIds.Add(HeroSelectionTool.GetRandomItemId(shopCfg.Id));
        }

        if (itemIds.Count < 9)
        {
            if (itemIds.Count + unsoldItems.Count > 9)
                unsoldItems.RemoveRange(0, itemIds.Count + unsoldItems.Count - 9);
            if (unsoldItems.Count > 0)
                itemIds.InsertRange(0, unsoldItems);
        }
        else if (itemIds.Count > 9)
        {
            itemIds.RemoveRange(9, itemIds.Count - 9);
        }

        int ids = 0;
        // item card
        // foreach (var itemId in itemIds)
        // {
        //     // 计算位置
        //     float x = -560 + ids * (140 + 5);
        //     ids++;
        //     float y = 0;

        //     // 创建CardView实例
        //     GameObject card = Instantiate(cardItemViewPrefab, cardItemView.transform);
        //     RectTransform rectTransform = card.GetComponent<RectTransform>();
        //     if (rectTransform != null)
        //         rectTransform.anchoredPosition = new Vector2(x, y);

        //     var count = 1;
        //     var itemCfg = ItemConfig.GetConfig(itemId);
        //     var cardPrice = itemCfg.Price;

        //     if (!itemCfg.SellOne && shopCfg.MultiPriceTotal > 2 * cardPrice)
        //     {
        //         var roll = UnityEngine.Random.Range(0, 100);
        //         if (roll < shopCfg.MultiCardRate)
        //         {
        //             count = UnityEngine.Random.Range(1, shopCfg.MultiPriceTotal / cardPrice + 1);
        //             if(roll >= 95 && shopCfg.ItemAmazingCount > count)
        //                 count = shopCfg.ItemAmazingCount;
        //         }

        //         if (count == 1)
        //         {
        //             count = Math.Max(1, shopCfg.MultiPriceTotal / 3 / cardPrice);
        //         }
        //     }            
        //     CardViewControl cardView = card.GetComponent<CardViewControl>();

        //     cardView.Init(itemId, false, count, year);
        //     cardViews.Add(cardView);
        // }

        era++;
        passBtn.gameObject.SetActive(true);

        var nowYear = GameManager.Instance.year + 179;
        eraText.text = nowYear + "年\n" + era + "月";

        // 重置所有玩家的pass状态
        for (int i = 0; i < playerPassed.Length; i++)
        {
            playerPassed[i] = false;
            GameManager.Instance.GetPlayer(i).SetRoundOver(false);
        }
        passedPlayers = 0;
        for (int i = 0; i < 8; i++)
            playerStartGold[i] = GameManager.Instance.GetPlayer(i).gold; // 记录开局金币

        int firstPid = -1;
        if (jadePlayer >= 0)
            firstPid = jadePlayer;
        else if (firstJumper >= 0)
            firstPid = firstJumper;

        if (firstPid >= 0)
            round = 8 * 100 + System.Array.IndexOf(turnOrder, firstPid); // 让该玩家排到回合最前
        else
            round = 1000;
        jadePlayer = -1;
        firstJumper = -1;

        var pid = GetTurnPid();
        GameManager.Instance.OnPlayerTurn(pid);
        mySelect.UpdateCards(GameManager.Instance.GetPlayer(pid));

        CheckEraBonusGold();
        GameManager.Instance.PlaySound("Sounds/page");
    }

    // 当前回合序号对应的玩家pid（回合顺序按积分低到高）
    private int GetTurnPid()
    {
        return turnOrder[round % turnOrder.Length];
    }

    // 商店开始时：按积分(mark)从低到高排序玩家，积分相同金币少的排前，再相同按pid排
    // 同时调整玩家位置（第1名排在最左边），回合顺序也按该排序
    private void SortPlayersByScore()
    {
        var players = GameManager.Instance.players;

        // 槽位位置：按当前X坐标从左到右排列，即第1名位置在最左
        var slotPos = players
            .Select(p => p.GetComponent<RectTransform>().anchoredPosition)
            .OrderBy(pos => pos.x)
            .ToArray();

        turnOrder = players
            .OrderBy(p => p.mark)
            .ThenBy(p => p.gold)
            .ThenBy(p => p.pid)
            .Select(p => p.pid)
            .ToArray();

        for (int i = 0; i < turnOrder.Length; i++)
        {
            players[turnOrder[i]].GetComponent<RectTransform>().anchoredPosition = slotPos[i];
        }
    }

    private void CheckEraBonusGold()
    {
        if(GameManager.Instance.year <= 2)
            return;

        // 获取所有玩家
        var players = new List<(int id, int gold)>();
        foreach (var player in GameManager.Instance.players)
            players.Add((player.pid, player.gold));

        // 按金币数量升序排序
        players.Sort((a, b) => a.gold.CompareTo(b.gold));

        if(players[0].gold == players[1].gold && players[1].gold == players[2].gold)
        {
            
        }
        else
        {
            if (players[0].gold < players[1].gold)
            {
                GameManager.Instance.GetPlayer(players[0].id).AddGold(2);
                if (players[1].gold < players[2].gold)
                {
                    GameManager.Instance.GetPlayer(players[1].id).AddGold(2);
                    GameManager.Instance.GetPlayer(players[2].id).AddGold(1);
                }
                else
                {
                    GameManager.Instance.GetPlayer(players[1].id).AddGold(1);
                    GameManager.Instance.GetPlayer(players[2].id).AddGold(1);
                }
            }
            else
            {
                GameManager.Instance.GetPlayer(players[0].id).AddGold(2);
                GameManager.Instance.GetPlayer(players[1].id).AddGold(2);
                GameManager.Instance.GetPlayer(players[2].id).AddGold(0);
            }
        }
    }

    public bool OnPlayerBuyCard(CardViewControl ctr, PlayerInfo player, int cardId, bool isHero, int price, int count)
    {
        if (player.BuyCard(ctr, cardId, isHero, price, count))
        {
            mySelect.UpdateCards(player);
            OnCardSelected(ctr);
            return true;
        }
        return false;
    }

    public void QuickView(int pid)
    {
        if (pid >= 0)
            mySelect.QuickView(GameManager.Instance.GetPlayer(pid));
        else
            mySelect.QuickViewFin();
    }

    public PlayerInfo GetCurrentPlayer()
    {
        return GameManager.Instance.GetPlayer(GetTurnPid());
    }

    public void OnP1Pass()
    {
        var nowPlayer = GameManager.Instance.GetPlayer(GetTurnPid());
        if(nowPlayer.isAI)
            return;
        if(playerPassed[nowPlayer.pid])
            return;

        passBtn.gameObject.SetActive(false);
        if(System.Linq.Enumerable.All(playerPassed, x => !x))
            firstJumper = nowPlayer.pid;
        playerPassed[nowPlayer.pid] = true;
        passedPlayers++;
        nowPlayer.SetRoundOver(true);

        AfterAct();
    }

    // 玩家支付2gold立刻刷新6张牌，可多次进行，不结束自己的回合
    private void OnRefresh()
    {
        var nowPlayer = GameManager.Instance.GetPlayer(GetTurnPid());
        if (nowPlayer.isAI)
            return;
        if (playerPassed[nowPlayer.pid])
            return;
        if (nowPlayer.gold < 2)
            return;

        nowPlayer.gold -= 2;
        nowPlayer.goldText.text = nowPlayer.gold.ToString();

        // 从未售出的卡牌中随机选取6张进行刷新（不足6张则全部刷新）
        var unsoldCards = cardViews.FindAll(x => !x.isSold);
        for (int i = 0; i < unsoldCards.Count; i++)
        {
            int j = UnityEngine.Random.Range(i, unsoldCards.Count);
            var tmp = unsoldCards[i];
            unsoldCards[i] = unsoldCards[j];
            unsoldCards[j] = tmp;
        }

        int refreshCount = Math.Min(6, unsoldCards.Count);
        for (int i = 0; i < refreshCount; i++)
            RefreshCard(unsoldCards[i]);

        GameManager.Instance.PlaySound("Sounds/page");
    }

    private void NextTurn()
    {
        UnityEngine.Debug.Log("NextTurn");
        for(int i = 0; i < 8; i++)
        {
            round++;
            var pid = GetTurnPid();
            if (!playerPassed[pid])
            {
                var nextPlayer = GameManager.Instance.GetPlayer(pid);
                passBtn.gameObject.SetActive(!nextPlayer.isAI);
                GameManager.Instance.OnPlayerTurn(pid);
                mySelect.UpdateCards(nextPlayer);
                return;
            }
        }
    }


    public void AfterAct()
    {
        NextTurn();

        // 只有一轮选牌：所有玩家都跳过时，选牌阶段结束进入战斗
        if (passedPlayers >= 8)
        {
            StartCoroutine(ShopEnd());
        }
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
        ctr.roundLeft = SOLD_REMAIN_ROUNDS;
        ctr.UpdateRoundLeft();

        // 每次有其他卡售出，所有已售出卡的倒计时-1，归0刷新
        foreach (var card in cardViews)
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

    // 刷新卡位：重新加载prefab生成一张随机新卡（防止复用旧对象导致样式/尺寸残留），roundLeft 重置为3
    private void RefreshCard(CardViewControl ctr)
    {
        int index = cardViews.IndexOf(ctr);
        if (index < 0)
            return;

        var year = GameManager.Instance.year;
        var shopCfg = ShopConfig.GetConfig(Math.Min(100, year));

        // 重新加载prefab，避免旧卡对象残留刷新前的样式/尺寸
        GameObject card = Instantiate(cardViewPrefab, transform);
        CardViewControl newCtr = card.GetComponent<CardViewControl>();

        if (ctr.isHeroCard)
        {
            // 按当前品质概率随机刷新，允许重复
            var heroId = HeroSelectionTool.GetRandomHeroIdByQuality(shopCfg);
            var heroPrice = HeroSelectionTool.GetPrice(HeroConfig.GetConfig(heroId));
            newCtr.Init(heroId, true, GetMultiCount(heroPrice, shopCfg), year);
        }
        else
        {
            var itemId = HeroSelectionTool.GetRandomItemId(shopCfg.Id);
            var itemCfg = ItemConfig.GetConfig(itemId);
            var count = itemCfg.SellOne ? 1 : GetMultiCount(itemCfg.Price, shopCfg);
            newCtr.Init(itemId, false, count, year);
        }

        // 保持原卡位的位置并替换列表引用，销毁旧卡
        newCtr.GetComponent<RectTransform>().anchoredPosition = ctr.GetComponent<RectTransform>().anchoredPosition;
        cardViews[index] = newCtr;

        Destroy(ctr.gameObject);
    }

    // 与初始刷牌一致的卡牌数量计算逻辑
    private int GetMultiCount(int cardPrice, ShopConfig shopCfg)
    {
        var count = 1;
        if (shopCfg.MultiPriceTotal > 2 * cardPrice)
        {
            var roll = UnityEngine.Random.Range(0, 100);
            if (roll < shopCfg.MultiCardRate)
            {
                count = UnityEngine.Random.Range(1, shopCfg.MultiPriceTotal / cardPrice + 1);
                if (roll >= 95 && shopCfg.ItemAmazingCount > count)
                    count = shopCfg.ItemAmazingCount;
            }

            if (count == 1)
                count = Math.Max(1, shopCfg.MultiPriceTotal / 3 / cardPrice);
        }
        return count;
    }

    // 获取一张卡的相邻卡：英雄卡在3列网格中算上下左右，道具卡在一行中算左右
    private List<CardViewControl> GetAdjacentCards(CardViewControl ctr)
    {
        var result = new List<CardViewControl>();
        int index = cardViews.IndexOf(ctr);
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
            if (index - 1 >= 0 && !cardViews[index - 1].isHeroCard)
                result.Add(cardViews[index - 1]);
            if (index + 1 < cardViews.Count && !cardViews[index + 1].isHeroCard)
                result.Add(cardViews[index + 1]);
        }
        return result;
    }

    private void TryAddAdjacent(List<CardViewControl> result, int row, int col)
    {
        if (row < 0 || col < 0 || row > 4 || col > 2)
            return;
        int i = row * 3 + col;
        if (i < cardViews.Count && cardViews[i].isHeroCard)
            result.Add(cardViews[i]);
    }

    public void ShopBegin()
    {
        UnityEngine.Debug.Log("ShopBegin");
        if(hasEnterBattle) //存档拉起进入游戏，不会重复存储
            GameManager.Instance.SaveToFile();

        var roll = UnityEngine.Random.Range(0, 3);
        BGMPlayer.Instance.PlaySound(roll == 0 ? "BGMs/chun" : (roll == 1 ? "BGMs/xia" : "BGMs/qiu"));

        if (GameManager.Instance.year == 0)
        {
            UnityEngine.Debug.Log("FirstRound ck");
            for(int i = 0; i < 8; i++)
                GameManager.Instance.GetPlayer(i).FirstRound();
        }

        var shopOpenIndex = GameManager.Instance.year; //第几场比赛
        var shopCfg = ShopConfig.GetConfig(Math.Min(100, shopOpenIndex + 1));
        var roundGold = shopCfg.RoundGold;
        for(int i = 0; i < 8; i++)
            GameManager.Instance.GetPlayer(i).RoundGold(roundGold);

        SortPlayersByScore();

        GameManager.Instance.year++;
        era = 0;
        NewEra();     
        shopCoroutine = StartCoroutine(DelayedUpdate()); 
    }

    private IEnumerator ShopEnd()
    {
        isShopEnd = true;

        yield return new WaitForSeconds(0.5f);
        if(shopCoroutine != null)
            StopCoroutine(shopCoroutine);
        shopCoroutine = null;

        GameManager.Instance.ClearTurn();

        var movingCardImages = GameObject.FindGameObjectsWithTag("MovingCard");
        foreach(var img in movingCardImages)
            Destroy(img);

        Tooltip.Instance.HideTooltip();
        PanelManager.Instance.HideShop();
        WorldManager.Instance.BattleBegin(); 
        hasEnterBattle = true;

        for(int i = 0; i < 8; i++)
            GameManager.Instance.GetPlayer(i).SetRoundOver(false);
    }
}
