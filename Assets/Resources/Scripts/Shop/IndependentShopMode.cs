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

    // AI 私有商店锁卡：pid → 锁定卡位；锁定时长1回合（锁定当回合结束生效，下回合结束自动解锁）
    private class AiShopLock
    {
        public int slotIndex;
        public int cardId;
        public int lockRound; // 锁定的回合：下次进入商店(year=lockRound+1)时生效
    }

    // 静态存储：商店模式对象每回合重建，锁卡记录需跨回合保留（新开一局时由 GameManager.ResetPlayersForNewGame 清空）
    private static readonly Dictionary<int, List<AiShopLock>> aiLocks = new Dictionary<int, List<AiShopLock>>();

    public static void ClearAiLocks()
    {
        aiLocks.Clear();
    }

    public override void Begin()
    {
        host.ClearCards();
        // 独立买卡商店支持卡牌锁定（仅人类私有卡位）
        host.SetLockMode(true);
        humanEnded = false;
        aiShops.Clear();

        var human = GameManager.Instance.GetPlayer(0);
        // 人类已淘汰：没有可操作的商店，直接跳过人类阶段
        if (human.isEliminated)
        {
            humanEnded = true;
            host.passBtn.gameObject.SetActive(false);
            GameLog.Debug("独立买卡商店：人类玩家已淘汰，跳过人类购买阶段");
        }
        else
        {
            // 人类(pid0)私有 5 格（按本人等级概率刷牌，like 走本人收藏池）
            // 上一商店阶段锁定的卡位在本阶段保持原卡牌与原位置
            int humanLevel = human.level;
            for (int i = 0; i < SLOT_COUNT; i++)
            {
                int lockedCardId = host.GetLockedCardId(i);
                int heroId;
                if (lockedCardId > 0 && HeroConfig.HasConfig(lockedCardId))
                {
                    heroId = lockedCardId;
                }
                else
                {
                    if (lockedCardId > 0)
                    {
                        GameLog.Warn($"独立买卡商店：卡位{i}的锁定卡牌 {lockedCardId} 配置不存在，改为随机刷牌");
                        host.RemoveShopLock(i);
                    }
                    heroId = RollOffer(humanLevel, 0).cardId;
                }

                var view = host.CreateCardView(heroId);
                view.shopSlotIndex = i;
                view.SetLocked(lockedCardId > 0 && heroId == lockedCardId);
                host.cardViews.Add(view);
            }
            host.LayoutCards(host.cardViews, SLOT_COUNT, 3);

            host.passBtn.gameObject.SetActive(true);
            host.SetPassButtonLabel("结束");

            GameManager.Instance.OnPlayerTurn(0);
            host.mySelect.UpdateCards(human);
        }

        // AI(pid1~7)各生成私有 5 格（无 UI，各自按本人等级概率刷牌，like 走各自收藏池；已淘汰AI不参与）
        for (int pid = 1; pid < 8; pid++)
        {
            var aiPlayer = GameManager.Instance.GetPlayer(pid);
            if (aiPlayer.isEliminated)
                continue;
            aiShops[pid] = BuildAiOffers(pid, aiPlayer);
        }
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
            if (player.isEliminated || player.nextSkip)
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

                // 买不动：满足"收益评估（当前无可用卡）+ 买得起（刷新后余额≥AiRefreshMinRemainGold）"才刷新，否则结束
                // 刷新上限：基础上限，金币充裕("钱多")时额外放开（PlayerAI.GetShopRefreshLimit）
                int refreshLimit = PlayerAI.GetShopRefreshLimit(player, CombatConst.AiMaxShopRefreshPerShop);
                if (refreshes < refreshLimit
                    && PlayerAI.ShouldAiRefresh(player, offers))
                {
                    RefreshShopFor(pid);
                    refreshes++;
                    continue;
                }
                break;
            }

            // 回合末锁卡：买不起但评分明显领先（或需要升星的品质3/4上阵卡）时锁到下回合，下回合结束自动解锁
            TryLockAiCard(pid, player);

            yield return new WaitForSeconds(SysRandom.Range(0.25f, 0.5f));
        }

        host.RequestEnd();
    }

    // 买卡阶段结束后判定是否锁卡：每个 AI 最多锁 AiLockMaxPerShop 个卡位
    private void TryLockAiCard(int pid, PlayerInfo player)
    {
        var activeLocks = GetActiveLocks(pid);
        if (activeLocks != null && activeLocks.Count >= CombatConst.AiLockMaxPerShop)
            return;

        if (!PlayerAI.TryAiLockCard(player, GetOffers(pid), out int lockHeroId))
            return;

        if (!aiShops.TryGetValue(pid, out var offers))
        {
            GameLog.Warn($"AI玩家{pid} 锁卡失败：私有商店不存在");
            return;
        }

        int slot = offers.FindIndex(o => o.cardId == lockHeroId && !o.sold);
        if (slot < 0)
        {
            GameLog.Warn($"AI玩家{pid} 锁卡失败：商店中找不到英雄 {lockHeroId} 的可售卡位");
            return;
        }

        aiLocks[pid] = new List<AiShopLock>
        {
            new AiShopLock { slotIndex = slot, cardId = lockHeroId, lockRound = GameManager.Instance.year }
        };
        GameLog.Debug($"AI玩家{pid} 锁定卡位{slot}(英雄{lockHeroId})，下回合保留，下回合结束后自动解锁");
    }

    // 当前回合有效的锁定记录（上回合锁定、本回合生效）；过期锁定直接清除
    private static List<AiShopLock> GetActiveLocks(int pid)
    {
        if (!aiLocks.TryGetValue(pid, out var locks))
            return null;

        int year = GameManager.Instance.year;
        locks.RemoveAll(l => l.lockRound + 1 != year); // 只锁1回合：过了下回合即失效
        if (locks.Count == 0)
        {
            aiLocks.Remove(pid);
            return null;
        }
        return locks;
    }

    // 生成 AI 私有商店，并保留本回合有效的锁定卡（位置不变）
    private List<ShopOffer> BuildAiOffers(int pid, PlayerInfo player)
    {
        var list = BuildOffers(player.level, SLOT_COUNT, pid);
        var locks = GetActiveLocks(pid);
        if (locks == null)
            return list;

        foreach (var l in locks)
        {
            if (l.slotIndex < 0 || l.slotIndex >= list.Count)
            {
                GameLog.Warn($"AI玩家{pid} 锁定卡位 {l.slotIndex} 超出商店范围，忽略该锁定");
                continue;
            }
            if (!HeroConfig.HasConfig(l.cardId))
            {
                GameLog.Warn($"AI玩家{pid} 锁定卡牌 {l.cardId} 配置不存在，忽略该锁定");
                continue;
            }
            var heroCfg = HeroConfig.GetConfig(l.cardId);
            list[l.slotIndex] = new ShopOffer
            {
                cardId = l.cardId,
                isHero = true,
                price = HeroSelectionTool.GetPrice(heroCfg),
            };
        }
        return list;
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
                // 锁定的卡位刷新时保持原卡牌，位置也不变
                if (host.cardViews[i].isLocked)
                    continue;

                var offer = RollOffer(player.level, 0);
                host.ReplaceCardAt(i, offer.cardId);
                // 新卡位重新登记槽位索引，锁定状态才可存档
                host.cardViews[i].shopSlotIndex = i;
            }
        }
        else
        {
            aiShops[pid] = BuildAiOffers(pid, player);
        }

        GameManager.Instance.PlaySound("Sounds/page");
    }

    // 随机一条英雄报价（按玩家等级品质概率 + 该玩家自己的收藏池）
    private ShopOffer RollOffer(int level, int pid)
    {
        var heroId = host.GetRandomShopHeroIdByLevel(level, pid);
        var price = HeroSelectionTool.GetPrice(HeroConfig.GetConfig(heroId));
        return new ShopOffer
        {
            cardId = heroId,
            isHero = true,
            price = price,
        };
    }

    private List<ShopOffer> BuildOffers(int level, int n, int pid)
    {
        var list = new List<ShopOffer>();
        for (int i = 0; i < n; i++)
            list.Add(RollOffer(level, pid));
        return list;
    }
}
