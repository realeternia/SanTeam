using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using CommonConfig;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Text;

public static class PlayerAI
{
    // 已告警过的缺失好友组Id（PlayerConfig.LikeFriend 可能引用配置表中不存在的组，避免重复刷日志）
    private static readonly HashSet<int> warnedMissingFriendGroups = new HashSet<int>();

    // 许愿阶段（第10/25/40回合）：AI 自动选择1个许愿英雄，参考自身喜好(LikeJob/LikeForce)与当前优势卡职业/阵营。
    // 一般许愿品质4，其余许愿品质3；选中的英雄在商店出卡权重×3，且同一英雄不会重复许愿。
    public static void CheckWish(PlayerInfo playerInfo, int wishSlot)
    {
        if (playerInfo == null || playerInfo.isEliminated)
            return;
        if (HeroSelectionTool.GetWishHeroId(playerInfo, wishSlot) != 0)
            return; // 该槽位已许愿，选定后不可修改
        if (!HeroSelectionTool.IsWishSlotUnlocked(wishSlot))
            return;

        // 一般许愿品质4，其余品质3
        int targetQuality = SysRandom.Range(0, 100) < AIConst.AiWishQuality4Rate ? 4 : 3;
        int heroId = PickWishHero(playerInfo, targetQuality);
        if (heroId == 0)
            heroId = PickWishHero(playerInfo, targetQuality == 4 ? 3 : 4);
        if (heroId == 0)
            return;

        HeroSelectionTool.SetWishHero(playerInfo, wishSlot, heroId);
        var heroCfg = HeroConfig.GetConfig(heroId);
        GameLog.Debug($"AI玩家{playerInfo.pid} 第{GameManager.Instance.year}回合许愿槽位{wishSlot}选择：{heroCfg.Name}(品质{HeroScenario.GetQuality(heroCfg)})");
    }

    // 在指定品质内根据自身喜好(LikeJob/LikeForce/LikeFriend)与当前手上的英雄加权随机一个许愿英雄
    private static int PickWishHero(PlayerInfo playerInfo, int quality)
    {
        var cfg = playerInfo.playerConfig;

        // 当前手上的英雄：上阵主力算两票，其余备用卡算一票，统计职业/阵营分布
        HashSet<int> lineupSet = new HashSet<int>(
            playerInfo.GetStrongCardList(playerInfo.GetSlotCount()).Select(x => x.Item1));
        Dictionary<string, int> ownedJobs = new Dictionary<string, int>();
        Dictionary<int, int> ownedSides = new Dictionary<int, int>();
        foreach (int cardId in playerInfo.cards.Keys)
        {
            if (!ConfigManager.IsHeroCard(cardId))
                continue;
            var heroCfg = HeroConfig.GetConfig(cardId);
            if (heroCfg == null || string.IsNullOrEmpty(heroCfg.Job))
                continue;
            int vote = lineupSet.Contains(cardId) ? 2 : 1;
            ownedJobs.TryGetValue(heroCfg.Job, out var jc);
            ownedJobs[heroCfg.Job] = jc + vote;
            ownedSides.TryGetValue(HeroScenario.GetSide(heroCfg), out var sc);
            ownedSides[HeroScenario.GetSide(heroCfg)] = sc + vote;
        }

        List<int> candidates = new List<int>();
        List<float> weights = new List<float>();
        foreach (int heroId in HeroSelectionTool.GetHeroPoolCache())
        {
            var heroCfg = HeroConfig.GetConfig(heroId);
            if (heroCfg == null || HeroScenario.GetQuality(heroCfg) != quality)
                continue;
            if (ConfigManager.IsKingHero(heroId))
                continue; // 主公不参与许愿
            if (HeroSelectionTool.IsWishedHero(playerInfo, heroId))
                continue; // 不重复许愿同一英雄

            float weight = 1f;
            // 自身喜好：职业 / 阵营 / 好友组命中即加权（与买卡评分同一口径）
            if (LikeHasJob(cfg.LikeJob, heroCfg))
                weight += 2f;
            if (LikeHasForce(cfg.LikeForce, HeroScenario.GetSide(heroCfg)))
                weight += 2f;
            if (LikeHasFriend(cfg.LikeFriend, heroId))
                weight += 2f;
            // 当前手上的英雄：同职业/同阵营越多越优先（主力占两票）
            if (ownedJobs.TryGetValue(heroCfg.Job, out var jobCount))
                weight += 1.5f * Mathf.Min(3, jobCount);
            if (ownedSides.TryGetValue(HeroScenario.GetSide(heroCfg), out var sideCount))
                weight += 0.5f * Mathf.Min(3, sideCount);

            candidates.Add(heroId);
            weights.Add(weight);
        }

        if (candidates.Count == 0)
            return 0;

        float total = weights.Sum();
        float roll = SysRandom.Range(0f, total);
        for (int i = 0; i < candidates.Count; i++)
        {
            roll -= weights[i];
            if (roll < 0f)
                return candidates[i];
        }
        return candidates[candidates.Count - 1];
    }


    // 商店开始时的 AI 检查（回合结束后、买卡前调用一次，只对 AI 调用）：
    //   1) 士兵强化：随机 0-1，小于 UpgradeSoldierRate 时花金币强化 1 级士兵
    //   2) 淘汰清仓：到达配置的淘汰回合后，把"非主力上阵"且等级在淘汰线及以下的卡立刻卖出
    //   3) 买经验：总经验低于期望（round×(2.5+ExpCheckOff)）时按缺口概率随机买经验，缺口越大触发概率越高（线性、封顶70%）
    public static void ShopBegin(PlayerInfo playerInfo)
    {
        if (playerInfo == null)
            return;

        // 士兵强化：每局商店开始随机 0-1，小于 UpgradeSoldierRate 时花金币强化 1 级士兵
        // （士兵向卡组（相的运筹、鼓手光环等）收益更高，故巴爸/巴妈配置为 0.3，其余 0.1）
        float upgradeSoldierRate = playerInfo.playerConfig.UpgradeSoldierRate;
        if (upgradeSoldierRate > 0f && SysRandom.Value < upgradeSoldierRate)
        {
            if (playerInfo.SodLvup())
                GameLog.Debug($"AI玩家{playerInfo.pid} 士兵强化+1，当前士兵等级{playerInfo.soldierLevel}(概率{upgradeSoldierRate:F2})");
            else
                GameLog.Debug($"AI玩家{playerInfo.pid} 士兵强化未执行(金币{playerInfo.gold}<{GameConst.SodLvupGoldCost}或已满级{GameConst.SoldierMaxLevel})");
        }

        // 淘汰清仓与 nextSkip 无关：到回合就必须清掉过期的低星卡
        EliminateOutdatedCards(playerInfo);
        // 回合开始时回收超出软上限的英雄卡，保证下回合回到软上限
        TrimHeroCardsToSoftCap(playerInfo);

        if (playerInfo.nextSkip)
            return;

        var round = GameManager.Instance.year;
        float expected = round * ((GameConst.BattleWinExp + GameConst.BattleLoseExp) / 2f +playerInfo.playerConfig.ExpCheckOff);
        int actual = playerInfo.GetTotalExp();
        if (actual >= expected)
            return;

        // 触发概率：缺口/期望，线性，封顶 70%
        float prob = expected > 0f ? Mathf.Min(0.7f, (expected - actual) / expected) : 0f;
        if (SysRandom.Value >= prob)
            return;

        // 用当前金币的一定比例买经验（4金=4经验）：默认 20%，金币充裕("钱多"、"多给一点钱买经验")时提高到 35%；
        // 金币足够时保底至少买 1 次，避免余钱不足导致命中却空手
        float expRate = IsRichGold(playerInfo) ? AIConst.RichExpBuyGoldRate : AIConst.NormalExpBuyGoldRate;
        int budget = Mathf.FloorToInt(playerInfo.gold * expRate);
        int times = budget / GameConst.ExpBuyGoldCost;
        if (playerInfo.gold >= GameConst.ExpBuyGoldCost)
            times = Mathf.Max(1, times);
        int bought = 0;
        for (int i = 0; i < times; i++)
        {
            if (!playerInfo.BuyExp())
                break;
            bought++;
        }
        GameLog.Debug($"AI玩家{playerInfo.pid} 经验检查 实际{actual}<期望{expected:F1}(系数{playerInfo.playerConfig.ExpCheckOff:F2}) 概率{prob:F2} 买经验{bought}次(花{bought * GameConst.ExpBuyGoldCost}金)");
    }

    // 淘汰清仓：到达该玩家的淘汰回合后，把"非主力上阵"且品质在淘汰线及以下的英雄卡逐回合卖出。
    // 逐回合渐进：按品质升序优先淘汰最低品质，每回合最多卖 EliminateMaxSellPerRound 张，避免单回合清空导致阵容断档。
    // 与喜好无关——即使它属于 LikeJob/LikeForce 也一视同仁（到回合就该换更强的卡）。
    // 不卖：主力上阵卡、主公、初始卡、当回合买入的新卡、聚焦目标成员、正在成组的好友组成员。
    public static void EliminateOutdatedCards(PlayerInfo playerInfo)
    {
        int maxQuality = GetEliminateMaxQuality(playerInfo);
        if (maxQuality <= 0)
            return;

        var strongList = playerInfo.GetStrongCardList(playerInfo.GetSlotCount()).Select(x => x.Item1).ToList();
        var strongSet = new HashSet<int>(strongList);
        var cfg = playerInfo.playerConfig;
        // 聚焦/成组保护快照（与买卡评分同一派生口径）
        var ctx = BuildContext(playerInfo, strongList);

        List<int> candidates = new List<int>();
        foreach (var cardId in playerInfo.cards.Keys)
        {
            if (!ConfigManager.IsHeroCard(cardId))
                continue;
            if (strongSet.Contains(cardId))
                continue; // 主力上阵不卖
            if (ConfigManager.IsKingHero(cardId))
                continue; // 主公核心不卖
            if (cfg.InitCards != null && cfg.InitCards.Contains(cardId))
                continue; // 初始卡不卖
            if (playerInfo.boughtThisShopCards.Contains(cardId))
                continue; // 当回合买入的新卡当回合不卖
            if (GetCardQuality(cardId) > maxQuality)
                continue; // 品质高于淘汰线（品质3/4）保留
            if (IsFocusMember(ctx, cardId))
                continue; // 聚焦目标成员不卖
            if (IsFormingFriendMember(playerInfo, cardId))
                continue; // 正在成组的好友组成员不卖
            candidates.Add(cardId);
        }

        // 品质升序：优先淘汰最低品质，让淘汰在多回合内摊开
        candidates.Sort((a, b) => GetCardQuality(a).CompareTo(GetCardQuality(b)));

        int sellCount = Mathf.Min(candidates.Count, Mathf.Max(1, AIConst.EliminateMaxSellPerRound));
        for (int i = 0; i < sellCount; i++)
        {
            int cardId = candidates[i];
            int quality = GetCardQuality(cardId);
            playerInfo.SellCard(cardId);
            GameLog.Debug($"AI玩家{playerInfo.pid} 淘汰清仓 卖出{HeroConfig.GetConfig(cardId).Name} 品质{quality} 淘汰线{maxQuality} 回合{GameManager.Instance.year}");
        }
    }

    // 当前淘汰线：0=未到回合不淘汰；1=淘汰品质1卡；2=淘汰品质2及以下卡（取两者中较大的已生效档位）
    public static int GetEliminateMaxQuality(PlayerInfo playerInfo)
    {
        var cfg = playerInfo == null ? null : playerInfo.playerConfig;
        if (cfg == null)
            return 0;

        int year = GameManager.Instance.year;
        int maxQuality = 0;
        if (cfg.EliminateQuality1 > 0 && year >= GetEliminateRound(playerInfo, cfg.EliminateQuality1))
            maxQuality = Mathf.Max(maxQuality, 1);
        if (cfg.EliminateQuality2 > 0 && year >= GetEliminateRound(playerInfo, cfg.EliminateQuality2))
            maxQuality = Mathf.Max(maxQuality, 2);
        return maxQuality;
    }

    // 实际淘汰回合 = 配置基准 ± round((1-smart)×EliminateRoundMaxOffset)：
    // smart(=聪明度/99) 越高偏移越小（±1 以内），越低越大（最多 ±5）；偏移方向按 pid 固定，一局内稳定不抖动
    private static int GetEliminateRound(PlayerInfo playerInfo, int baseRound)
    {
        float smart = Mathf.Clamp(playerInfo.playerConfig.Intelligence, 1, 99) / 99f;
        int offset = Mathf.RoundToInt((1f - smart) * AIConst.EliminateRoundMaxOffset);
        if (playerInfo.pid % 2 != 0)
            offset = -offset;
        return Mathf.Max(1, baseRound + offset);
    }

    // 该卡的品质（1~4）：品质是卡自身固定属性，与是否拥有/张数无关。
    // 因此未拥有的品质1/2卡在淘汰期同样不再买入；而品质3/4卡天然高于淘汰线，后期仍可买入补阵。
    private static int GetCardQuality(int cardId)
    {
        var cfg = HeroConfig.GetConfig(cardId);
        return cfg == null ? 0 : HeroScenario.GetQuality(cfg);
    }

    // 过滤出"当前真正可考虑购买"的卡：买得起 + 英雄卡 + 非满级同卡 + 通过淘汰过滤。
    // 供买卡评分与 AI 刷新收益判定共用，保证两处口径完全一致（避免分叉）。
    // ignoreGold=true 时保留"买不起"的卡（锁卡判定需要评估当前买不起的卡）
    public static List<ShopOffer> FilterOffers(PlayerInfo playerInfo, List<ShopOffer> offers, bool ignoreGold = false)
    {
        var result = new List<ShopOffer>();
        if (playerInfo == null || offers == null)
            return result;

        var cards = playerInfo.cards;
        // 买不起的直接剔除（锁卡判定时不过滤金币）
        result = ignoreGold ? offers.ToList() : offers.Where(card => playerInfo.gold >= card.price).ToList();
        // 商店不卖道具：只考虑英雄卡
        result = result.Where(card => card.isHero).ToList();
        // 满级(Lv5)英雄卡不再提供经验，买入纯亏金币：直接从候选剔除（同卡加经验、换卡两种情形都不买）
        result = result
            .Where(card => !(cards.ContainsKey(card.cardId) && HeroSelectionTool.IsHeroCardMaxLevel(cards[card.cardId])))
            .ToList();
        if (result.Count == 0)
            return result;

        // 淘汰期：品质已在淘汰线及以下的卡不再购买（仅非主力上阵的卡）
        // 品质是卡自身固定属性：未拥有的品质1/2卡同样被剔除；品质3/4卡天然高于淘汰线，不受影响
        // 例外：聚焦目标（好友组/职业）成员豁免本过滤——聚焦套卡的低品质卡仍可继续买入，避免"攒一半被淘汰线掐断"
        int eliminateMaxQuality = GetEliminateMaxQuality(playerInfo);
        if (eliminateMaxQuality > 0)
        {
            var strongList = playerInfo.GetStrongCardList(playerInfo.GetSlotCount()).Select(x => x.Item1).ToList();
            var strongSet = new HashSet<int>(strongList);
            var ctx = BuildContext(playerInfo, strongList);
            result = result
                .Where(card => strongSet.Contains(card.cardId)
                    || GetCardQuality(card.cardId) > eliminateMaxQuality
                    || IsFocusMember(ctx, card.cardId))
                .ToList();
        }
        return result;
    }

    // 金币是否充裕（"钱多"）：金币 >= RichGoldThreshold 即视为可多花——多刷新、多买经验
    public static bool IsRichGold(PlayerInfo playerInfo)
    {
        return playerInfo != null && playerInfo.gold >= AIConst.RichGoldThreshold;
    }

    // AI 是否继续花 2 金刷新当前商店（共享/独立模式共用）：
    //   1) 收益评估：当前"可用卡"为 0（一张能买/该买的卡都没有：全被满级/淘汰/买不起滤掉）才值得刷——
    //      刷新出的新卡与旧卡同源、品质期望不变，收益只来自把这批无效卡换掉；
    //      若已有可用卡，说明买不动是卡包容量/金币不足导致的，刷新解决不了，继续刷只是烧金币，
    //      余额留到下回合买更强的卡（金币跨回合累积，不是丢失）。
    //   2) "钱多"时仍允许刷新求更好的卡；买得起刷新费即可，不设保留金门槛（想留的卡交给锁卡）。
    //   nextSkip 玩家本轮主动跳过，刷新没有意义。
    public static bool ShouldAiRefresh(PlayerInfo playerInfo, List<ShopOffer> offers)
    {
        if (playerInfo == null || playerInfo.nextSkip)
            return false;
        if (FilterOffers(playerInfo, offers).Count > 0 && !IsRichGold(playerInfo))
            return false;
        return playerInfo.gold >= CardShopManager.RefreshGoldCost;
    }

    public static bool AiCheckBuyCard(PlayerInfo playerInfo, int era, List<ShopOffer> offers)
    {
        if(playerInfo.nextSkip)
            return false;

        // 超过软上限时先卖掉多余的弱卡，回到软上限再继续买卡
        TrimHeroCardsToSoftCap(playerInfo);

        var year = GameManager.Instance.year;
        
        var playerConfig = playerInfo.playerConfig;
        var cards = playerInfo.cards;

        // 过滤出真正可考虑的候选：买得起 + 英雄卡 + 非满级同卡 + 通过淘汰过滤（与 AI 刷新判定共用同一口径）
        var affordableCards = FilterOffers(playerInfo, offers);
        if (affordableCards.Count == 0)
            return false;

        Tuple<int, int> weakHeroCard = null;
        var heroCardCount = playerInfo.GetHeroCardList().Count;
        if (heroCardCount >= GetHeroCardSoftCap(playerInfo))
            weakHeroCard = FindWeakCard(playerInfo);

        // 直接复用 PlayerInfo 自动上阵的最强卡选择：按总战力取当前上限张
        var strongList = playerInfo.GetStrongCardList(playerInfo.GetSlotCount()).Select(x => x.Item1).ToList();
        // 阵容羁绊快照：阵营/职业人数与近战远程构成，供国家、职业、远近评分子项复用
        var ctx = BuildContext(playerInfo, strongList);
        // 聪明度归一化(1~99 → 0.01~1)：越高越能正确评估羁绊价值、越少手滑选错
        float smart = Mathf.Clamp(playerConfig.Intelligence, 1, 99) / 99f;

        // 计算每张卡片的加权分（评分逻辑统一抽到 ScoreOffer，锁卡判定复用同一口径）
        List<(ShopOffer card, float score)> scoredCards = new List<(ShopOffer card, float score)>();
        foreach (var pickCard in affordableCards)
        {
            float? score = ScoreOffer(playerInfo, playerConfig, cards, year, ctx, strongList, pickCard, heroCardCount, weakHeroCard, true);
            if (score == null) // 不参与本次选择（满级/超上限无法换位等）
                continue;
            // 加入分数列表
            scoredCards.Add((pickCard, score.Value));
        }

        // 没有可买的候选（如满包且无可卖弱卡、或只剩满级同卡）：不再为"看未来存钱"而跳过，
        // 因为现在有刷新与锁卡，想留的卡可以锁、想要的卡可以刷新找，攒钱没有收益
        if (scoredCards.Count == 0)
        {
            GameLog.Debug($"AI玩家{playerInfo.pid} 本回合没有可买候选（金币{playerInfo.gold}），结束购买");
            return false;
        }

        //scoredCards的key的priceI前三3的卡分别（1.5，1.3，1.1）
        if (scoredCards.Count >= 5 && scoredCards.Max(x => x.score) < 1.6f)
        {
            var top3Cards = scoredCards.OrderByDescending(x => x.card.price).Take(3).ToList();
            for (int i = 0; i < top3Cards.Count; i++)
            {
                var card = top3Cards[i];
                var index = scoredCards.FindIndex(x => x.card == card.card);
                scoredCards[index] = (card.card, card.score * (1.6f - i * 0.2f));
            }
        }

        scoredCards = scoredCards.OrderByDescending(x => x.score).ToList();
        //日志打印scoredCards和selectedCard

        var sb = new StringBuilder();
        sb.AppendLine($"{playerInfo.playerNameText.text} 选卡 scoredCards数量: {scoredCards.Count}");
        for (int i = 0; i < scoredCards.Count; i++)
        {
            var card = scoredCards[i];
            var logCfg = HeroConfig.GetConfig(card.card.cardId);
            sb.AppendLine($"  [{i+1}] 卡片ID: {card.card.cardId}, 名称: {logCfg.Name}, 分数: {card.score}, 价格: {card.card.price}, 品质: {HeroScenario.GetQuality(logCfg)}, 品质系数: {GetQualityBias(HeroScenario.GetQuality(logCfg), playerInfo):F2}");
        }

        // 聪明度越高候选池越窄（只在最优的几张里挑）；越低越宽（更容易随机到次优卡）
        int pickPool = smart >= 0.7f ? 3 : (smart >= 0.4f ? 4 : 6);
        if (scoredCards.Count > pickPool)
            scoredCards = scoredCards.Take(pickPool).ToList();

        // 尝试购买：按分数加权随机挑一张；若买不进（如满包且无可卖弱卡），跳过该卡继续挑下一张。
        // 好处：满包时仍能买到"同卡（升星）"等无需腾位的候选；避免一次失败就整回合收工导致金币花不出去。
        // 全部失败才返回 false，由调用方把该玩家标记跳过（防死循环）。
        var tryPool = new List<(ShopOffer card, float score)>(scoredCards);
        while (tryPool.Count > 0)
        {
            // 根据分数计算总权重
            float totalWeight = tryPool.Sum(item => item.score);
            float randomValue = totalWeight > 0f ? SysRandom.Range(0f, totalWeight) : 0f;

            // 根据随机值和权重选择卡片
            float cumulativeWeight = 0f;
            int pickIndex = tryPool.Count - 1;
            for (int i = 0; i < tryPool.Count; i++)
            {
                cumulativeWeight += tryPool[i].score;
                if (randomValue <= cumulativeWeight)
                {
                    pickIndex = i;
                    break;
                }
            }

            ShopOffer selectedCard = tryPool[pickIndex].card;
            tryPool.RemoveAt(pickIndex);

            sb.AppendLine($"选中卡片: ID={selectedCard.cardId}, 名称={HeroConfig.GetConfig(selectedCard.cardId).Name}, roll={randomValue}, 英雄卡={selectedCard.isHero}");

            bool sameCard = cards.ContainsKey(selectedCard.cardId);
            int heroCardCountNow = playerInfo.GetHeroCardList().Count;
            // 新英雄种类上限：达到硬上限直接跳过；超过软上限时先卖弱卡腾位，保证买入后回到软上限
            if (selectedCard.isHero && !sameCard)
            {
                if (heroCardCountNow >= GetHeroCardHardCap(playerInfo))
                {
                    GameLog.Debug($"AI玩家{playerInfo.pid} 英雄卡已达硬上限{GetHeroCardHardCap(playerInfo)}，跳过新英雄 {HeroConfig.GetConfig(selectedCard.cardId).Name}");
                    continue; // 换下一张候选
                }

                int softCap = GetHeroCardSoftCap(playerInfo);
                if (heroCardCountNow >= softCap)
                    SellWeakCards(playerInfo, heroCardCountNow + 1 - softCap); // 卖出多余的弱卡腾位
            }

            // 真实购买结果：英雄卡已满且无弱卡可卖等情况下购买会失败，此时换一张候选继续尝试
            if (CardShopManager.Instance.BuyOffer(playerInfo, selectedCard))
            {
                GameLog.Debug(sb.ToString());
                return true;
            }

            GameLog.Debug($"AI玩家{playerInfo.pid} 买卡失败跳过 {HeroConfig.GetConfig(selectedCard.cardId).Name}，继续尝试下一张");
        }

        GameLog.Debug(sb.ToString());
        return false;
    }

    // 品质偏好系数：按当前商店实际出卡概率线性插值——概率最高的品质取 QualityBiasMax(2.0)、最低取 QualityBiasMin(0.5)
    // 概率来源与真实刷牌一致：独立买卡(ShopType=1)按该玩家等级读 PlayerLevelConfig；共享轮换按存活最高玩家等级推算
    // 品质1概率 = 100 - 品质2 - 品质3 - 品质4
    private static float GetQualityBias(int quality, PlayerInfo playerInfo)
    {
        int q2, q3, q4;
        var shopMgr = CardShopManager.Instance;
        if (shopMgr != null && shopMgr.Mode != null && shopMgr.Mode.Type == ShopModeType.Independent)
        {
            var lvCfg = PlayerLevelConfig.GetConfig(Mathf.Clamp(playerInfo.level, 1, GameConst.PlayerMaxLevel));
            if (lvCfg == null)
                return 1f; // UI 数据未就绪：不加不减
            q2 = lvCfg.Quality2Rate;
            q3 = lvCfg.Quality3Rate;
            q4 = lvCfg.Quality4Rate;
        }
        else
        {
            // 共享商店：与真实出卡同一口径
            HeroSelectionTool.GetSharedShopQualityRates(out q2, out q3, out q4);
        }

        float[] rates =
        {
            Mathf.Max(0f, 100f - q2 - q3 - q4),
            Mathf.Max(0f, q2),
            Mathf.Max(0f, q3),
            Mathf.Max(0f, q4),
        };
        float min = rates[0];
        float max = rates[0];
        foreach (var r in rates)
        {
            if (r < min) min = r;
            if (r > max) max = r;
        }
        if (max <= min)
            return 1f; // 各品质概率相同，无偏好

        int idx = Mathf.Clamp(quality - 1, 0, rates.Length - 1);
        float bias = AIConst.QualityBiasMin
            + (AIConst.QualityBiasMax - AIConst.QualityBiasMin) * (rates[idx] - min) / (max - min);
        // 品质4强卡设下限：低等级阶段 Q4 概率最低会被压到 QualityBiasMin，抬高史诗卡获取（也是聚焦核心卡来源）
        if (quality == 4)
            bias = Mathf.Max(bias, AIConst.QualityBias4Floor);
        return bias;
    }

    // 升星导向收益：重复卡的"距下一星卡数"越近收益越高（1张=1.0、2张=0.6、3张=0.35、更远=0.15）；
    // 上阵主力额外×1，预备席×StarUpBenchFactor（上阵卡的星级才直接计入阵容价值）。已满级返回 0。
    private static float GetStarUpGain(PlayerInfo playerInfo, List<int> strongList, int cardId)
    {
        if (playerInfo == null || !playerInfo.cards.TryGetValue(cardId, out int exp) || exp <= 0)
            return 0f;
        if (HeroSelectionTool.IsHeroCardMaxLevel(exp))
            return 0f;

        int level = HeroSelectionTool.GetCardLevel(exp, true);
        int nextNeed = HeroSelectionTool.GetCardExpByLevel(level + 1);
        int remaining = nextNeed - exp;
        float prox = remaining <= 1 ? 1f : (remaining == 2 ? 0.6f : (remaining == 3 ? 0.35f : 0.15f));
        bool inLineup = strongList != null && strongList.Contains(cardId);
        return prox * (inLineup ? 1f : AIConst.StarUpBenchFactor);
    }

    // 候选卡职业是否为玩家 LikeJob 中"上阵人数最多"的那一路（决定 job 信任倍率是否用提升值）
    private static bool IsTopLikeJob(AiContext ctx, HeroConfig heroCfg)
    {
        if (ctx.cfg.LikeJob == null || ctx.cfg.LikeJob.Length == 0)
            return false;

        int best = 0;
        foreach (var jobId in ctx.cfg.LikeJob)
        {
            var jobCfg = JobConfig.GetConfig(jobId);
            if (jobCfg == null)
                continue;
            int count = ctx.GetJobCount(jobCfg.NameS);
            if (count > best)
                best = count;
        }
        return best > 0 && ctx.GetJobCount(heroCfg.Job) == best;
    }

    // 候选卡的（命中 LikeFriend 的）好友组中，是否存在"上阵成员最多"的那一组（决定 friend 信任倍率是否用提升值）
    private static bool IsTopLikeFriend(AiContext ctx, int cardId)
    {
        if (ctx.cfg.LikeFriend == null || ctx.cfg.LikeFriend.Length == 0)
            return false;

        int best = 0;
        foreach (var relId in ctx.cfg.LikeFriend)
        {
            int present = CountFriendMembersInLineup(ctx, relId);
            if (present > best)
                best = present;
        }
        if (best <= 0)
            return false;

        var cardRels = ConfigManager.GetHeroFriendInfo(cardId);
        if (cardRels == null)
            return false;
        foreach (var relId in cardRels)
        {
            if (System.Array.IndexOf(ctx.cfg.LikeFriend, relId) < 0)
                continue;
            if (CountFriendMembersInLineup(ctx, relId) == best)
                return true;
        }
        return false;
    }

    // 某好友组当前上阵成员数（组配置缺失时返回0，避免 LikeFriend 引用无效Id 抛异常）
    private static int CountFriendMembersInLineup(AiContext ctx, int friendRelId)
    {
        var relCfg = GetFriendConfigSafe(friendRelId);
        if (relCfg == null || relCfg.Heros == null)
            return 0;

        int present = 0;
        foreach (var memberId in relCfg.Heros)
        {
            if (ctx.InLineup(memberId))
                present++;
        }
        return present;
    }

    // 好友组配置安全获取：LikeFriend 等数据可能引用配置表中不存在的Id，缺失时告警一次并返回 null
    private static HeroFriendConfig GetFriendConfigSafe(int friendRelId)
    {
        if (HeroFriendConfig.HasConfig(friendRelId))
            return HeroFriendConfig.GetConfig(friendRelId);

        if (warnedMissingFriendGroups.Add(friendRelId))
            GameLog.Warn("好友组配置缺失id=" + friendRelId + "（疑似 PlayerConfig.LikeFriend 引用了不存在的组），AI 忽略该组");
        return null;
    }

    private static float GetSellCostBias(int cardPrice, int year, PlayerConfig cfg)
    {
        if (cardPrice <= 0 || cfg == null || year <= 0)
            return 0f;

        float accumulated = Mathf.Max(0f, cfg.AccumulatedCostBias) * Mathf.Sqrt(year);
        int targetCost = Mathf.Max(1, Mathf.RoundToInt(accumulated));

        // Prefer selling cards that are low relative to targetCost
        if (cardPrice <= targetCost)
            return -0.6f - Mathf.Max(0, targetCost - cardPrice) * 0.25f;
        if (cardPrice <= targetCost + 1)
            return -0.25f;
        return 0.2f; // high-cost cards are less likely to be sold
    }

    // 找最该卖的英雄卡：按"阵容价值"升序（价格×羁绊贡献），保护正在组的国家/职业/好友拼图
    public static Tuple<int, int> FindWeakCard(PlayerInfo playerInfo)
    {
        var cards = playerInfo.cards;
        var strongList = playerInfo.GetStrongCardList(playerInfo.GetSlotCount()).Select(x => x.Item1).ToList();
        var ctx = BuildContext(playerInfo, strongList);

        List<Tuple<int, float, int>> sortDataList = new List<Tuple<int, float, int>>(); // heroId, 阵容价值, 价格

        // 卖卡意向基准：以本家"同卡堆总价(单价×张数)"最高的卡堆为基数，其他卡堆按占比得倍率（总价越高越不舍得卖）
        float maxStackValue = 0f;
        foreach (var kv in cards)
        {
            if (!ConfigManager.IsHeroCard(kv.Key))
                continue;
            var stackCfg = HeroConfig.GetConfig(kv.Key);
            if (stackCfg == null)
                continue;
            maxStackValue = Mathf.Max(maxStackValue, HeroSelectionTool.GetPrice(stackCfg) * kv.Value);
        }

        foreach (int cardId in cards.Keys)
        {
            if (!ConfigManager.IsHeroCard(cardId))
                continue;

            // 当回合买入的新卡，当回合不卖
            if (playerInfo.boughtThisShopCards.Contains(cardId))
                continue;

            var heroCfg = HeroConfig.GetConfig(cardId);
            var price = HeroSelectionTool.GetPrice(heroCfg);

            // 尽量不卖高星卡（保护3星及以上）
            int cardLevel = HeroSelectionTool.GetCardLevel(cards[cardId], true);
            if (cardLevel >= 3) // 3级及以上视为未来潜力，跳过
                continue;

            // 如果已有多张并且已经接近升星（如2级且有2张及以上），也尽量保护
            if (cards[cardId] >= 2 && cardLevel >= 2)
                continue;

            if (playerInfo.playerConfig.InitCards != null && playerInfo.playerConfig.InitCards.Contains(cardId))
                continue; //初始卡不删

            if (ConfigManager.IsKingHero(cardId))
                continue; //主公核心不删

            if (IsFocusMember(ctx, cardId))
                continue; //聚焦目标成员不卖（与淘汰保护同一口径）

            float value = GetLineupValue(ctx, heroCfg);
            // 总价倍率：卡堆总价占本家最高卡堆的比例，线性映射到 [1, 3] 倍——低总价的卡堆卖得更靠前
            float stackRatio = maxStackValue > 0f ? price * cards[cardId] / maxStackValue : 1f;
            float stackMultiplier = AIConst.SellValueMultiplierMin
                + (AIConst.SellValueMultiplierMax - AIConst.SellValueMultiplierMin) * Mathf.Clamp01(stackRatio);
            value *= stackMultiplier;
            float sellBias = GetSellCostBias(price, GameManager.Instance.year, playerInfo.playerConfig);
            value *= 1f + sellBias;
            sortDataList.Add(new Tuple<int, float, int>(cardId, value, price));
        }

        if (sortDataList.Count == 0)
            return null;

        // 记录 debug：列出候选的卖卡及其计算值
        var sb = new StringBuilder();
        sb.AppendLine($"{playerInfo.playerNameText.text} 卖卡候选数: {sortDataList.Count}");
        foreach (var t in sortDataList)
        {
            int id = t.Item1;
            var cfg = HeroConfig.GetConfig(id);
            int lvl = HeroSelectionTool.GetCardLevel(cards[id], true);
            int cnt = cards[id];
            float price = t.Item3;
            float sellBias = GetSellCostBias((int)price, GameManager.Instance.year, playerInfo.playerConfig);
            sb.AppendLine($"  卡ID:{id}, 名称:{cfg.Name}, 等级:{lvl}, 数量:{cnt}, 价格:{price}, 价值:{t.Item2:F2}, sellBias:{sellBias:F2}");
        }
        GameLog.Debug(sb.ToString());

        sortDataList.Sort((a, b) => a.Item2.CompareTo(b.Item2)); // 价值升序，最弱在前
        var weakest = sortDataList[0];
        return new Tuple<int, int>(weakest.Item1, weakest.Item3);
    }

    // 单张候选卡的评分（买卡与锁卡共用同一口径）；返回 null 表示该卡本次不参与选择（满级/超上限无法换位等）
    // includeNoise=false 时不叠加"聪明度抖动"，用于锁卡时比较真实的分数差距
    private static float? ScoreOffer(PlayerInfo playerInfo, PlayerConfig playerConfig, Dictionary<int, int> cards,
        int year, AiContext ctx, List<int> strongList, ShopOffer pickCard, int heroCardCount,
        Tuple<int, int> weakHeroCard, bool includeNoise)
    {
        float smart = Mathf.Clamp(playerConfig.Intelligence, 1, 99) / 99f;
        float score = 1f;

        // 如果已经拥有该卡片，增加分数（只保留同卡倍率；低等级优先/非主力惩罚/价格偏好已移除）
        bool hasSameCard = cards.ContainsKey(pickCard.cardId);
        if (hasSameCard)
            score *= playerConfig.SameCardRate;

        if (pickCard.isHero)
        {
            var heroCfg = HeroConfig.GetConfig(pickCard.cardId);
            // 新英雄种类受上限约束：软上限=上阵格+Cardherolimit，硬上限=min(软上限+1, 背包绝对上限)。
            // 达到硬上限后不再买新种类；位于软上限与硬上限之间时，只能卖弱卡换入（买入后回到软上限）
            if (!hasSameCard)
            {
                if (heroCardCount >= GetHeroCardHardCap(playerInfo))
                    return null;
                if (heroCardCount >= GetHeroCardSoftCap(playerInfo))
                {
                    if (weakHeroCard == null) //没有可以换的卡
                        return null;
                    if (pickCard.price < weakHeroCard.Item2)
                        return null; //没必要换更弱的卡
                    if (year > 8 && pickCard.price < weakHeroCard.Item2 + year - 8)
                        return null; //新卡价格还不如旧卡，没必要换
                }
            }
            // 品质偏好：按本回合商店实际出卡概率提升——概率最高的品质系数 2.0、最低 0.5，中间线性插值
            score *= GetQualityBias(HeroScenario.GetQuality(heroCfg), playerInfo);
            // 价格偏好：阵容价值=价格×星级，星级受出卡随机性限制（多数卡只能到 2 星），
            // 故"价格"是更可控的价值杠杆——按 (价格/基准)^指数 放大高价卡权重（Q4史诗=最高价）
            score *= Mathf.Pow((float)pickCard.price / AIConst.CardPriceBiasBase, AIConst.CardPriceBiasExponent);

            // 许愿：候选卡是"当前玩家自己许愿的英雄"时，拿取意愿 ×2
            if (HeroSelectionTool.IsWishedHero(playerInfo, pickCard.cardId))
                score *= AIConst.WishCardBias;

            // 升星导向：重复卡按"距下一星的卡数"加权（越近越高），上阵主力额外加成。
            // 阵容价值=价格×星级，升星是乘法收益：把资源集中到核心卡升星，而非到处买新卡换血
            if (hasSameCard)
            {
                float starGain = GetStarUpGain(playerInfo, strongList, pickCard.cardId);
                if (starGain > 0f)
                    score *= 1f + AIConst.StarUpBias * starGain;
            }

            // 近战/远程与羁绊统计均按"新增英雄"计算：重复卡（升星）只吃同卡分
            bool isNewHero = !strongList.Contains(pickCard.cardId);

            // 羁绊信任固定倍率：命中即加成。friend Bias 最大、job 次之、force(强卡/国家护盾)最小；
            // 命中 Like 且该路为"当前上阵人数最多"的那一路时，job 0.6→1、friend 0.8→1.5
            // force：候选卡所属势力命中 LikeForce 时，按推进同阵营护盾档位(2/3/4/5/6人→Lv1~5)计
            if (LikeHasForce(playerConfig.LikeForce, HeroScenario.GetSide(heroCfg)))
                score *= 1f + AIConst.ForceBias * GetForceGain(ctx, heroCfg, isNewHero);
            // job：候选卡所属职业命中 LikeJob 时，按推进或达成职业连锁档位(1/2/3/4/5人→Lv1~5)计
            if (LikeHasJob(playerConfig.LikeJob, heroCfg))
            {
                float jobBias = IsTopLikeJob(ctx, heroCfg) ? AIConst.JobChainBiasTop : AIConst.JobChainBias;
                score *= 1f + jobBias * GetJobGain(ctx, heroCfg, isNewHero);
            }
            // friend：候选卡所属好友组命中 LikeFriend 时，按连线档位(2~6人)/特殊连锁组进度/每回合金币组计
            if (LikeHasFriend(playerConfig.LikeFriend, pickCard.cardId))
            {
                float friendBias = IsTopLikeFriend(ctx, pickCard.cardId) ? AIConst.FriendBiasTop : AIConst.FriendBias;
                score *= 1f + friendBias * GetFriendGain(ctx, pickCard.cardId);
            }
            // 聚焦：候选卡命中"派生的聚焦目标"(好友组/职业)时追加固定倍率。
            // 不要求命中 Like——聚焦是 AI 从已拥有的 Q4 核心卡"发现"出来的长期规划，只对有核心卡的玩家生效
            float focusGain = GetFocusGain(ctx, heroCfg, pickCard.cardId, isNewHero);
            if (focusGain > 0f)
                score *= 1f + AIConst.FocusBias * focusGain;
            // 远近搭配：缺哪类补哪类，同类堆多则扣分（保留聪明度折扣）
            score *= 1f + playerConfig.BalanceFactor * GetBalanceGain(ctx, heroCfg, isNewHero) * smart;
        }

        // 如果不是已拥有的同卡，根据拥有人数惩罚热门卡
        if (!hasSameCard)
        {
            //获取现在拥有这张卡牌的存活玩家人数（已淘汰玩家不再抢卡，不计入）
            int playersWithThisCard = 0;
            foreach (var ckPlayer in GameManager.Instance.players)
            {
                if (ckPlayer.isEliminated)
                    continue;
                if (ckPlayer.cards.ContainsKey(pickCard.cardId))
                    playersWithThisCard++;
            }

            // 线性惩罚：每多一名对手持有该卡按玩家"风险把控"强度折损，并设下限，避免热门核心卡被挤出候选池
            if (playersWithThisCard > 0)
                score *= Mathf.Max(AIConst.HotCardPenaltyFloor,
                    1f - AIConst.HotCardPenaltyPerPlayer * playerConfig.OwnTooMuchCardRate * playersWithThisCard);
        }

        // 聪明度越低，评分抖动越大（看走眼/手滑选到差卡）
        if (includeNoise)
            score *= 1f + SysRandom.Range(-1f, 1f) * (1f - smart) * 0.5f;

        return score;
    }

    // AI 回合末锁卡判定：商店里买不起的卡中，评分最高的达到第二名 AiLockScoreRatio 倍（明星卡），
    // 或该卡是品质3/4且已在上阵名单里（需要升星）时锁定，下回合保持原卡位，下回合结束自动解锁
    public static bool TryAiLockCard(PlayerInfo playerInfo, List<ShopOffer> offers, out int lockedHeroId)
    {
        lockedHeroId = 0;
        if (playerInfo == null || offers == null)
            return false;

        int year = GameManager.Instance.year;
        var playerConfig = playerInfo.playerConfig;
        var cards = playerInfo.cards;

        // 锁卡评估必须包含"当前买不起"的卡
        var candidates = FilterOffers(playerInfo, offers, true);
        if (candidates.Count == 0)
            return false;

        var heroCardCount = playerInfo.GetHeroCardList().Count;
        Tuple<int, int> weakHeroCard = heroCardCount >= GetHeroCardSoftCap(playerInfo) ? FindWeakCard(playerInfo) : null;
        var strongList = playerInfo.GetStrongCardList(playerInfo.GetSlotCount()).Select(x => x.Item1).ToList();
        var ctx = BuildContext(playerInfo, strongList);

        var scored = new List<(ShopOffer card, float score)>();
        foreach (var offer in candidates)
        {
            float? score = ScoreOffer(playerInfo, playerConfig, cards, year, ctx, strongList, offer, heroCardCount, weakHeroCard, false);
            if (score == null)
                continue;
            scored.Add((offer, score.Value));
        }
        if (scored.Count == 0)
            return false;

        scored = scored.OrderByDescending(x => x.score).ToList();
        var top = scored[0];
        // 买得起就不需要锁（买卡阶段已处理）
        if (playerInfo.gold >= top.card.price)
            return false;

        var topCfg = HeroConfig.GetConfig(top.card.cardId);
        // 需要升星的卡：品质3/4且已拥有且在上阵名单，直接锁（确定性收益，不看分差）
        bool needStarUp = topCfg != null && HeroScenario.GetQuality(topCfg) >= 3
            && cards.ContainsKey(top.card.cardId) && strongList.Contains(top.card.cardId);
        // 明星卡：评分达到第二名的 AiLockScoreRatio 倍
        bool dominant = scored.Count >= 2 && top.score >= scored[1].score * AIConst.AiLockScoreRatio;
        if (!needStarUp && !dominant)
            return false;

        lockedHeroId = top.card.cardId;
        GameLog.Debug($"AI玩家{playerInfo.pid} 锁卡 {topCfg?.Name}({lockedHeroId}) 分数{top.score:F2} 第二名{(scored.Count >= 2 ? scored[1].score : 0f):F2} 金币{playerInfo.gold} 价格{top.card.price} 升星卡={needStarUp}");
        return true;
    }

    // ---- 英雄卡数量上限 ----
    // 软上限=当前上阵格数+Cardherolimit（正常持有量），超过后要立刻卖卡回到软上限；
    // 硬上限=min(软上限+1, 背包绝对上限)（瞬时溢出上限），达到后不再买新英雄种类

    public static int GetHeroCardSoftCap(PlayerInfo playerInfo)
    {
        if (playerInfo == null || playerInfo.playerConfig == null)
        {
            GameLog.Warn("GetHeroCardSoftCap 参数为空，上限按0处理");
            return 0;
        }
        return playerInfo.GetSlotCount() + playerInfo.playerConfig.Cardherolimit;
    }

    public static int GetHeroCardHardCap(PlayerInfo playerInfo)
    {
        return Mathf.Min(GetHeroCardSoftCap(playerInfo) + 1, GameConst.PlayerMaxHeroCards);
    }

    // 超出软上限时逐张卖出最弱的卡，直到回到软上限
    private static void TrimHeroCardsToSoftCap(PlayerInfo playerInfo)
    {
        int softCap = GetHeroCardSoftCap(playerInfo);
        int guard = 0;
        while (playerInfo.GetHeroCardList().Count > softCap && guard++ < GameConst.PlayerMaxHeroCards)
        {
            var weak = FindWeakCard(playerInfo);
            if (weak == null)
            {
                GameLog.Debug($"AI玩家{playerInfo.pid} 英雄卡{playerInfo.GetHeroCardList().Count}张超出软上限{softCap}，但暂无可卖弱卡");
                return;
            }
            GameLog.Debug($"AI玩家{playerInfo.pid} 英雄卡超出软上限{softCap}，卖出最弱卡 {HeroConfig.GetConfig(weak.Item1).Name}");
            playerInfo.SellCard(weak.Item1);
        }
    }

    // 卖出最多 count 张最弱卡（买入新英雄前腾位）
    private static void SellWeakCards(PlayerInfo playerInfo, int count)
    {
        for (int i = 0; i < count; i++)
        {
            var weak = FindWeakCard(playerInfo);
            if (weak == null)
            {
                GameLog.Debug($"AI玩家{playerInfo.pid} 腾位时已无可卖弱卡，保留现有多余英雄卡待后续回收");
                return;
            }
            playerInfo.SellCard(weak.Item1);
        }
    }

    // ---- AI 选卡评分辅助 ----

    // AI 选卡评分上下文：一次选卡内复用的阵容羁绊快照
    private class AiContext
    {
        public PlayerConfig cfg;
        public List<int> lineup;                                                 // 当前主力上阵英雄(heroId)
        public Dictionary<int, int> sideCount = new Dictionary<int, int>();      // 阵营→同阵营英雄数
        public Dictionary<string, int> jobCount = new Dictionary<string, int>(); // 职业→同职业英雄数
        public int meleeCount;
        public int rangedCount;
        public List<FocusTarget> focusTargets = new List<FocusTarget>();         // 本回合派生的聚焦目标（好友组/职业，最多 FocusMaxTargets 个）

        public bool InLineup(int heroId)
        {
            return lineup.Contains(heroId);
        }

        public int GetSideCount(int side)
        {
            return sideCount.TryGetValue(side, out var c) ? c : 0;
        }

        public int GetJobCount(string job)
        {
            return jobCount.TryGetValue(job, out var c) ? c : 0;
        }
    }

    // 统计当前主力阵容的阵营/职业人数与近战远程构成
    private static AiContext BuildContext(PlayerInfo playerInfo, List<int> strongList)
    {
        var ctx = new AiContext();
        ctx.cfg = playerInfo.playerConfig;
        ctx.lineup = strongList;
        foreach (var heroId in strongList)
        {
            var heroCfg = HeroConfig.GetConfig(heroId);
            ctx.sideCount[HeroScenario.GetSide(heroCfg)] = ctx.GetSideCount(HeroScenario.GetSide(heroCfg)) + 1;
            ctx.jobCount[heroCfg.Job] = ctx.GetJobCount(heroCfg.Job) + 1;
            if (HeroSelectionTool.IsMeleeHero(heroCfg))
                ctx.meleeCount++;
            else
                ctx.rangedCount++;
        }
        // 聚焦目标：从"当前拥有的 Q4 核心卡"派生（依赖上阵快照算档位进度），供买卡评分与卖卡/淘汰保护复用
        ctx.focusTargets = DeriveFocusTargets(playerInfo, ctx);
        return ctx;
    }

    // 聚焦目标：好友组或职业。派生式无状态——不写存档、不做显式重置，卖掉核心卡即自动释放，跨回合天然稳定
    private class FocusTarget
    {
        public bool isJob;         // true=职业聚焦；false=好友组聚焦
        public int friendGroupId;  // 好友组Id（isJob=false 时有效）
        public string jobNameS;    // 职业短名（isJob=true 时有效）
    }

    // 聚焦候选的排序载体（core=已拥有Q4数、owned=已拥有成员数、progress=档位进度）
    private class FocusCandidate
    {
        public FocusTarget target;
        public int core;
        public int owned;
        public int progress;
    }

    // 从"当前拥有的核心卡(品质=FocusCoreQuality)"派生最多 FocusMaxTargets 个聚焦目标（好友组与职业共用名额）。
    // 候选=核心卡所属的好友组/职业（Q4数≥1）；排序 Q4数 desc → 已拥有成员数 desc → 档位进度 desc，取前 N。
    private static List<FocusTarget> DeriveFocusTargets(PlayerInfo playerInfo, AiContext ctx)
    {
        var result = new List<FocusTarget>();
        if (playerInfo == null || playerInfo.cards == null)
            return result;

        // 单次遍历已拥有卡：统计各好友组/职业的已拥有成员数，并挑出核心卡(Q4)
        var friendOwned = new Dictionary<int, int>();
        var jobOwned = new Dictionary<string, int>();
        var coreIds = new List<int>();
        foreach (var kv in playerInfo.cards)
        {
            if (kv.Value <= 0 || !ConfigManager.IsHeroCard(kv.Key))
                continue;
            var heroCfg = HeroConfig.GetConfig(kv.Key);
            if (heroCfg == null)
                continue;

            if (!string.IsNullOrEmpty(heroCfg.Job))
            {
                jobOwned.TryGetValue(heroCfg.Job, out var jc);
                jobOwned[heroCfg.Job] = jc + 1;
            }
            var relIds = ConfigManager.GetHeroFriendInfo(kv.Key);
            if (relIds != null)
            {
                foreach (var relId in relIds)
                {
                    friendOwned.TryGetValue(relId, out var fc);
                    friendOwned[relId] = fc + 1;
                }
            }
            if (HeroScenario.GetQuality(heroCfg) == AIConst.FocusCoreQuality)
                coreIds.Add(kv.Key);
        }
        if (coreIds.Count == 0)
            return result;

        // 核心卡归属的好友组 / 职业（Q4数 >= 1 才成为候选）
        var friendCore = new Dictionary<int, int>();
        var jobCore = new Dictionary<string, int>();
        foreach (var coreId in coreIds)
        {
            var relIds = ConfigManager.GetHeroFriendInfo(coreId);
            if (relIds != null)
            {
                foreach (var relId in relIds)
                {
                    friendCore.TryGetValue(relId, out var c);
                    friendCore[relId] = c + 1;
                }
            }
            var heroCfg = HeroConfig.GetConfig(coreId);
            if (heroCfg != null && !string.IsNullOrEmpty(heroCfg.Job))
            {
                jobCore.TryGetValue(heroCfg.Job, out var jc);
                jobCore[heroCfg.Job] = jc + 1;
            }
        }

        // 候选排序：Q4数 desc → 已拥有成员数 desc → 档位进度 desc（同分好友优先于职业）
        var ranked = new List<FocusCandidate>();
        foreach (var kv in friendCore)
        {
            friendOwned.TryGetValue(kv.Key, out var owned);
            ranked.Add(new FocusCandidate
            {
                target = new FocusTarget { isJob = false, friendGroupId = kv.Key },
                core = kv.Value,
                owned = owned,
                progress = GetTierLevel(CountFriendMembersInLineup(ctx, kv.Key), AIConst.FriendLineCounts),
            });
        }
        foreach (var kv in jobCore)
        {
            jobOwned.TryGetValue(kv.Key, out var owned);
            ranked.Add(new FocusCandidate
            {
                target = new FocusTarget { isJob = true, jobNameS = kv.Key },
                core = kv.Value,
                owned = owned,
                progress = Mathf.Min(ctx.GetJobCount(kv.Key), 5),
            });
        }
        ranked.Sort((a, b) =>
        {
            int cmp = b.core.CompareTo(a.core);
            if (cmp != 0) return cmp;
            cmp = b.owned.CompareTo(a.owned);
            if (cmp != 0) return cmp;
            cmp = b.progress.CompareTo(a.progress);
            if (cmp != 0) return cmp;
            return a.target.isJob.CompareTo(b.target.isJob); // 同分好友优先（false < true）
        });

        int take = Mathf.Min(AIConst.FocusMaxTargets, ranked.Count);
        for (int i = 0; i < take; i++)
            result.Add(ranked[i].target);
        return result;
    }

    // 某好友组当前已拥有的成员数（供聚焦候选评分 / 成组保护复用；组配置缺失返回0）
    private static int CountOwnedInFriendGroup(PlayerInfo playerInfo, int friendRelId)
    {
        var relCfg = GetFriendConfigSafe(friendRelId);
        if (relCfg == null || relCfg.Heros == null)
            return 0;

        int owned = 0;
        foreach (var memberId in relCfg.Heros)
        {
            if (playerInfo.cards.TryGetValue(memberId, out var cnt) && cnt > 0)
                owned++;
        }
        return owned;
    }

    // 聚焦收益：候选卡命中任一聚焦目标（属于聚焦好友组 或 职业==聚焦职业）时的羁绊收益，复用 GetFriendGain/GetJobGain。
    // 好友侧只算一次（同卡可能在多个聚焦组里），避免重复累加。
    private static float GetFocusGain(AiContext ctx, HeroConfig heroCfg, int cardId, bool isNewHero)
    {
        if (ctx.focusTargets == null || ctx.focusTargets.Count == 0)
            return 0f;

        bool friendHit = false;
        bool jobHit = false;
        var relIds = ConfigManager.GetHeroFriendInfo(cardId);
        foreach (var target in ctx.focusTargets)
        {
            if (target.isJob)
            {
                if (!string.IsNullOrEmpty(heroCfg.Job) && heroCfg.Job == target.jobNameS)
                    jobHit = true;
            }
            else if (relIds != null && relIds.Contains(target.friendGroupId))
            {
                friendHit = true;
            }
        }

        float gain = 0f;
        if (friendHit)
            gain += GetFriendGain(ctx, cardId);
        if (jobHit)
            gain += GetJobGain(ctx, heroCfg, isNewHero);
        return gain;
    }

    // 该卡是否为聚焦目标成员（属于聚焦好友组 或 职业为聚焦职业）：卖卡/淘汰保护用
    private static bool IsFocusMember(AiContext ctx, int cardId)
    {
        if (ctx == null || ctx.focusTargets == null || ctx.focusTargets.Count == 0)
            return false;

        var heroCfg = HeroConfig.GetConfig(cardId);
        if (heroCfg == null)
            return false;

        var relIds = ConfigManager.GetHeroFriendInfo(cardId);
        foreach (var target in ctx.focusTargets)
        {
            if (target.isJob)
            {
                if (!string.IsNullOrEmpty(heroCfg.Job) && heroCfg.Job == target.jobNameS)
                    return true;
            }
            else if (relIds != null && relIds.Contains(target.friendGroupId))
            {
                return true;
            }
        }
        return false;
    }

    // 该卡所属好友组是否"正在成组"（该组已拥有 >= 2 张）：淘汰时保护，避免拆散正在形成的好友羁绊
    private static bool IsFormingFriendMember(PlayerInfo playerInfo, int cardId)
    {
        var relIds = ConfigManager.GetHeroFriendInfo(cardId);
        if (relIds == null)
            return false;
        foreach (var relId in relIds)
        {
            if (CountOwnedInFriendGroup(playerInfo, relId) >= 2)
                return true;
        }
        return false;
    }

    // 通用档位：count 达到 thresholds 第 i 档 → 返回 i+1，未达标返回 0（thresholds 需递增）
    private static int GetTierLevel(int count, int[] thresholds)
    {
        var lv = 0;
        for (int i = 0; i < thresholds.Length; i++)
        {
            if (count >= thresholds[i])
                lv = i + 1;
        }
        return lv;
    }

    // 该阵营是否参与同阵营护盾（野=10 不参与，不参与则无国家收益）
    private static bool IsShieldSide(int side)
    {
        var forceCfg = ConfigManager.GetForceConfig(side);
        return forceCfg != null && forceCfg.JoinFactionShield;
    }

    // 国家档位收益：加这张新英雄后能否推进/达成同阵营护盾档位(2/3/4/5/6人→Lv1~5)；重复卡不改变上阵人数
    // force 命中度：品质4强卡直接命中(1)，否则按推进同阵营护盾档位计（SideFactor 已并入 force 信任）
    private static float GetForceGain(AiContext ctx, HeroConfig heroCfg, bool isNewHero)
    {
        if (HeroScenario.GetQuality(heroCfg) == 4)
            return 1f;
        return GetSideGain(ctx, heroCfg, isNewHero);
    }

    private static float GetSideGain(AiContext ctx, HeroConfig heroCfg, bool isNewHero)
    {
        if (!isNewHero || !IsShieldSide(HeroScenario.GetSide(heroCfg)))
            return 0f;

        int count = ctx.GetSideCount(HeroScenario.GetSide(heroCfg));
        int curLv = GetTierLevel(count, CombatConst.FactionShieldCounts);
        int newLv = GetTierLevel(count + 1, CombatConst.FactionShieldCounts);

        float gain;
        if (newLv > curLv)
        {
            gain = 1f + 0.25f * (newLv - 1); // 达成/提升档位：档位越高收益越大
        }
        else
        {
            int nextNeed = 0;
            foreach (var threshold in CombatConst.FactionShieldCounts)
            {
                if (threshold > count)
                {
                    nextNeed = threshold;
                    break;
                }
            }
            gain = nextNeed > 0 ? (float)count / nextNeed * 0.4f : 0f; // 未达档：越接近下一档越高
        }

        // 主公：同阵营已有英雄时额外加权（国家护盾叠主公加成）
        if (ConfigManager.IsKingHero(heroCfg.Id) && count >= 1)
            gain += 0.6f;
        return gain;
    }

    // 职业连锁收益：同职业 1/2/3/4/5 人对应 Lv1~5（等级即人数，封顶5）；重复卡不改变上阵人数
    private static float GetJobGain(AiContext ctx, HeroConfig heroCfg, bool isNewHero)
    {
        if (!isNewHero)
            return 0f;
        int count = ctx.GetJobCount(heroCfg.Job);
        if (count >= 5)
            return 0f; // 已满档不再鼓励堆叠
        return 0.3f * (count + 1); // Lv1=0.3 … Lv5=1.5
    }

    // 好友收益：与阵容内英雄的连线档位(2~6人→Lv1~5) + 特殊连锁组进度 + 每回合金币组
    private static float GetFriendGain(AiContext ctx, int cardId)
    {
        if (ctx.InLineup(cardId))
            return 0f; // 重复卡不新增连线

        float gain = 0f;

        int friendCount = 0;
        foreach (var heroId in ctx.lineup)
        {
            if (ConfigManager.GetFriendLevel(cardId, heroId) > 0)
                friendCount++;
        }
        if (friendCount >= AIConst.FriendLineCounts[0])
        {
            int lv = GetTierLevel(friendCount, AIConst.FriendLineCounts);
            gain += 0.8f + 0.25f * (lv - 1); // 已成团：档位越高收益越大
        }
        else if (friendCount == 1)
        {
            gain += AIConst.FriendGainOneShort; // 差一人成团：拉平到接近成团档(0.8)，鼓励补齐第 2 人
        }

        var relIds = ConfigManager.GetHeroFriendInfo(cardId);
        if (relIds != null)
        {
            foreach (var relId in relIds)
            {
                var relCfg = HeroFriendConfig.GetConfig(relId);
                if (relCfg == null)
                    continue;

                // 该关系组内已上阵的其他成员数（每多一个，特殊技能+1级）
                int present = 0;
                foreach (var memberId in relCfg.Heros)
                {
                    if (memberId == cardId)
                        continue;
                    if (ctx.InLineup(memberId))
                        present++;
                }
                if (present <= 0)
                    continue;

                if (relCfg.SkillId == GameConst.FriendGoldSkillSname)
                    gain += 0.25f; // 每回合金币组
                else if (!string.IsNullOrEmpty(relCfg.SkillId))
                    gain += 0.5f + 0.2f * Math.Min(present, 3); // 特殊连锁：成员越多技能等级越高
                else
                    gain += 0.3f; // 普通好友组，仍走默认连线
            }
        }
        return gain;
    }

    // 远近搭配：缺哪类补哪类(加分)，已堆多的一类再加扣分；上阵不足3人时不参与
    private static float GetBalanceGain(AiContext ctx, HeroConfig heroCfg, bool isNewHero)
    {
        if (!isNewHero || ctx.meleeCount + ctx.rangedCount < 3)
            return 0f;
        int diff = ctx.meleeCount - ctx.rangedCount;
        float advantage = HeroSelectionTool.IsMeleeHero(heroCfg) ? -diff : diff;
        return Mathf.Clamp(advantage, -3f, 3f) / 3f; // -1~1
    }

    // LikeForce 命中判定：候选卡所属势力(side)是否在喜欢的势力Id列表内
    private static bool LikeHasForce(int[] likeForce, int side)
    {
        if (likeForce == null || likeForce.Length == 0)
            return false;
        return System.Array.IndexOf(likeForce, side) >= 0;
    }

    // LikeJob 命中判定：候选卡职业缩写(JobConfig.NameS) 映射回 JobConfig.Id 后是否在喜欢的职业Id列表内
    private static bool LikeHasJob(int[] likeJob, HeroConfig heroCfg)
    {
        if (likeJob == null || likeJob.Length == 0)
            return false;
        int jobId = 0;
        foreach (var jobCfg in JobConfig.ConfigList)
        {
            if (jobCfg.NameS == heroCfg.Job)
            {
                jobId = jobCfg.Id;
                break;
            }
        }
        return jobId != 0 && System.Array.IndexOf(likeJob, jobId) >= 0;
    }

    // LikeFriend 命中判定：候选卡的任一好友组(HeroFriendConfig.Id)是否在喜欢的好友组Id列表内
    private static bool LikeHasFriend(int[] likeFriend, int heroId)
    {
        if (likeFriend == null || likeFriend.Length == 0)
            return false;
        var relIds = ConfigManager.GetHeroFriendInfo(heroId);
        if (relIds == null)
            return false;
        foreach (var relId in relIds)
        {
            if (System.Array.IndexOf(likeFriend, relId) >= 0)
                return true;
        }
        return false;
    }

    // 英雄的阵容价值：价格 × (1 + 阵营/职业/好友羁绊贡献)，贡献越高越不该卖
    private static float GetLineupValue(AiContext ctx, HeroConfig heroCfg)
    {
        float price = HeroSelectionTool.GetPrice(heroCfg);

        int sideCount = ctx.GetSideCount(HeroScenario.GetSide(heroCfg));
        float sideContrib = IsShieldSide(HeroScenario.GetSide(heroCfg)) && sideCount >= 2 ? Mathf.Min(sideCount - 1, 5) / 5f : 0f;

        int jobCount = ctx.GetJobCount(heroCfg.Job);
        float jobContrib = jobCount >= 2 ? Mathf.Min(jobCount - 1, 5) / 5f : 0f;

        int friendCount = 0;
        foreach (var heroId in ctx.lineup)
        {
            if (heroId != heroCfg.Id && ConfigManager.GetFriendLevel(heroCfg.Id, heroId) > 0)
                friendCount++;
        }
        float friendContrib = Mathf.Min(friendCount, 5) / 5f;

        // 羁绊信任固定倍率与选卡评分一致：命中即按 Bias×贡献计价。force(强卡/护盾)最小、job 次之、friend 最大
        float forceContrib = HeroScenario.GetQuality(heroCfg) == 4 ? 1f : sideContrib;
        float syn = 0f;
        if (LikeHasForce(ctx.cfg.LikeForce, HeroScenario.GetSide(heroCfg)))
            syn += AIConst.ForceBias * forceContrib;
        if (LikeHasJob(ctx.cfg.LikeJob, heroCfg))
            syn += AIConst.JobChainBias * jobContrib;
        if (LikeHasFriend(ctx.cfg.LikeFriend, heroCfg.Id))
            syn += AIConst.FriendBias * friendContrib;
        float value = price * (1f + syn);

        // 近战/远程搭配：已堆多的一类更容易被卖（与选卡评分里的平衡逻辑一致）
        if (ctx.meleeCount + ctx.rangedCount >= 3)
        {
            int diff = ctx.meleeCount - ctx.rangedCount;
            if (HeroSelectionTool.IsMeleeHero(heroCfg) && diff > 0)
                value *= 1f - Mathf.Min(diff, 3) * 0.1f;
            else if (HeroSelectionTool.IsRangedHero(heroCfg) && diff < 0)
                value *= 1f - Mathf.Min(-diff, 3) * 0.1f;
        }
        return value;
    }
}
