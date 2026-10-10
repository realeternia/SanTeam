using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 独立买卡商店（金铲铲式）：每个玩家拥有自己的私有商店，人类只看/买自己的 5 格，
/// 可花 2 金刷新自己 5 格；点“结束”后各 AI 依次计算并购买自己的商店，随后进入战斗。
/// AI 商店不实例化 UI，仅用 ShopOffer 数据对象承载。
/// </summary>
public class IndependentShopMode : ShopMode
{
    public override ShopModeType Type => ShopModeType.Independent;

    private const int SLOT_COUNT = 5; // 每个玩家的私有商店格数

    private bool humanEnded = false;
    private readonly Dictionary<int, List<ShopOffer>> aiShops = new Dictionary<int, List<ShopOffer>>();

    public override void Begin()
    {
        host.ClearCards();
        humanEnded = false;
        aiShops.Clear();

        // 人类(pid0)私有 5 格（按本人等级概率刷牌）
        int humanLevel = GameManager.Instance.GetPlayer(0).level;
        for (int i = 0; i < SLOT_COUNT; i++)
        {
            var offer = RollOffer(humanLevel);
            host.cardViews.Add(host.CreateCardView(offer.cardId));
        }
        host.LayoutCards(host.cardViews, SLOT_COUNT, 3);

        // AI(pid1~7)各生成私有 5 格（无 UI，各自按本人等级概率刷牌）
        for (int pid = 1; pid < 8; pid++)
            aiShops[pid] = BuildOffers(GameManager.Instance.GetPlayer(pid).level, SLOT_COUNT);

        host.passBtn.gameObject.SetActive(true);
        host.SetPassButtonLabel("结束");

        GameManager.Instance.OnPlayerTurn(0);
        host.mySelect.UpdateCards(GameManager.Instance.GetPlayer(0));
    }

    public override PlayerInfo GetActingPlayer()
    {
        return GameManager.Instance.GetPlayer(0);
    }

    public override List<ShopOffer> GetOffers(int pid)
    {
        if (pid == 0)
        {
            var list = new List<ShopOffer>();
            foreach (var v in host.cardViews)
            {
                if (!v.isSold)
                    list.Add(v.ToOffer());
            }
            return list;
        }

        // AI：隐藏商店，过滤掉已售出的报价
        if (aiShops.TryGetValue(pid, out var offers))
            return offers.Where(o => !o.sold).ToList();
        return new List<ShopOffer>();
    }

    public override void OnHumanPass()
    {
        if (humanEnded)
            return;
        humanEnded = true;
        host.passBtn.gameObject.SetActive(false);
    }

    public override void OnHumanRefresh()
    {
        if (humanEnded)
            return;
        var human = GameManager.Instance.GetPlayer(0);
        if (human.gold < CardShopManager.RefreshGoldCost)
        {
            SystemTip.Show($"金币不足，刷新需要{CardShopManager.RefreshGoldCost}金币");
            return;
        }
        RefreshShopFor(0);
    }

    public override IEnumerator Drive()
    {
        host.IsShopEnd = false;

        // 人类自由购买/刷新，直到点击“结束”（刷新/买卡由按钮事件驱动）
        while (!humanEnded)
            yield return new WaitForSeconds(0.2f);

        // 人类结束后：各 AI 依次计算并购买自己的私有商店（可刷新）
        for (int pid = 1; pid < 8; pid++)
        {
            while (IsPanelBlocked())
                yield return new WaitForSeconds(0.2f);

            var player = GameManager.Instance.GetPlayer(pid);
            if (player.nextSkip)
                continue;

            int refreshes = 0;
            int guard = 0;
            while (guard++ < 20)
            {
                // 每次购买后重新拉取报价：AI 隐藏商店买入会把报价标记为已售出，需过滤，避免重复买同一张
                var offers = GetOffers(pid);

                // 买得动就继续买（复用共享模式同一套 AI 评分/购买）
                if (PlayerAI.AiCheckBuyCard(player, host.Era, offers))
                    continue;

                // 买不动且还有余钱：刷新自己的商店后重试
                if (refreshes < CombatConst.AiMaxShopRefreshPerShop
                    && player.gold >= CardShopManager.RefreshGoldCost * 2)
                {
                    RefreshShopFor(pid);
                    refreshes++;
                    continue;
                }
                break;
            }

            yield return new WaitForSeconds(SysRandom.Range(0.25f, 0.5f));
        }

        host.RequestEnd();
    }

    // 花 2 金重掷某玩家自己的全部 5 格
    private void RefreshShopFor(int pid)
    {
        var player = GameManager.Instance.GetPlayer(pid);
        player.gold -= CardShopManager.RefreshGoldCost;
        if (player.goldText != null)
            player.goldText.text = player.gold.ToString();

        if (pid == 0)
        {
            for (int i = 0; i < host.cardViews.Count; i++)
            {
                var offer = RollOffer(player.level);
                host.ReplaceCardAt(i, offer.cardId);
            }
        }
        else
        {
            aiShops[pid] = BuildOffers(player.level, SLOT_COUNT);
        }

        GameManager.Instance.PlaySound("Sounds/page");
    }

    // 随机一条英雄报价（按玩家等级品质概率 + 收藏池）
    private ShopOffer RollOffer(int level)
    {
        var heroId = host.GetRandomShopHeroIdByLevel(level);
        var price = HeroSelectionTool.GetPrice(HeroConfig.GetConfig(heroId));
        return new ShopOffer
        {
            cardId = heroId,
            isHero = true,
            price = price,
        };
    }

    private List<ShopOffer> BuildOffers(int level, int n)
    {
        var list = new List<ShopOffer>();
        for (int i = 0; i < n; i++)
            list.Add(RollOffer(level));
        return list;
    }
}
