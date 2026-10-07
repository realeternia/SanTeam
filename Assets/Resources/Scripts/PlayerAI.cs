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
    // like阶段：AI 按 playerConfig 的品质偏好 + 低价区间(5~10金) 选取卡牌点赞，共点赞 likeCount 张（默认2张，选不同卡）
    // （点赞只是把该卡选入收藏池，不移动/移除英雄池；收藏池会以 LikeCardRefreshRate 概率在商店刷新中重新出现）
    public static void CheckLike(PlayerInfo playerInfo, List<PickPanelCellControl> cellControls)
    {
        var playerConfig = playerInfo.playerConfig;
        var pid = playerInfo.pid;

        // 每个玩家可赞 likeCount 张（默认2张），尽量选不同卡
        int remain = playerInfo.likeCount;
        while (remain > 0)
        {
            // 根据playerConfig的配置过滤可点赞的英雄（只选未被该玩家赞过的卡）
            List<PickPanelCellControl> availableLikes = new List<PickPanelCellControl>();
            foreach (var cell in cellControls)
            {
                if (cell.likeState > 0 || ConfigManager.IsKingHero(cell.heroId))
                    continue;

                var heroConfig = HeroConfig.GetConfig(cell.heroId);

                // AI 一般只选 5~10 金的卡
                var price = HeroSelectionTool.GetPrice(heroConfig);
                if (price < 5 || price > 10)
                    continue;

                availableLikes.Add(cell);
            }

            // 从目标列表中随机选择一个进行点赞
            if (availableLikes.Count > 0)
            {
                int randomIndex = SysRandom.Range(0, availableLikes.Count);
                availableLikes[randomIndex].SetLike(pid);
            }
            else
            {
                // 如果没有满足所有条件的卡牌，选择一张满足基本条件的卡牌
                List<PickPanelCellControl> basicAvailableCells = new List<PickPanelCellControl>();
                foreach (var cell in cellControls)
                {
                    if (cell.likeState == 0 && !ConfigManager.IsKingHero(cell.heroId))
                        basicAvailableCells.Add(cell);
                }
                if (basicAvailableCells.Count > 0)
                {
                    int randomIndex = SysRandom.Range(0, basicAvailableCells.Count);
                    basicAvailableCells[randomIndex].SetLike(pid);
                }
            }

            // 本轮是否成功点掉一张：likeCount 已被 SetLike 自减
            if (playerInfo.likeCount >= remain)
                break; // 没点出去（无可用卡），避免死循环
            remain = playerInfo.likeCount;
        }
    }


    public static bool AiCheckBuyCard(PlayerInfo playerInfo, int era)
    {
        if(playerInfo.nextSkip)
            return false;

        var year = GameManager.Instance.year;
        
        var playerConfig = playerInfo.playerConfig;
        var cards = playerInfo.cards;

        // 获取所有未售出的卡片
        List<CardViewControl> availableCards = CardShopManager.Instance.cardViews
            .Where(card => !card.isSold)
            .ToList();

        // 如果没有可用卡片，直接返回
        if (availableCards.Count == 0)
            return false;

        // 过滤掉买不起的卡片
        var affordableCards = availableCards.Where(card => playerInfo.gold >= card.priceI).ToList(); //看一张卡的价格
        if (affordableCards.Count == 0)
            return false;

        // 商店不卖道具：只考虑英雄卡
        affordableCards = affordableCards.Where(card => card.isHeroCard).ToList();
        // 满级(Lv5)英雄卡不再提供经验，买入纯亏金币：直接从候选剔除（同卡加经验、换卡两种情形都不买）
        affordableCards = affordableCards
            .Where(card => !(cards.ContainsKey(card.cardId) && HeroSelectionTool.IsHeroCardMaxLevel(cards[card.cardId])))
            .ToList();
        if (affordableCards.Count == 0)
            return false;

        bool hasSameCard = false;
        Tuple<int, int> weakHeroCard = null;
        var heroCardCount = playerInfo.GetHeroCardList().Count;
        if (heroCardCount >= playerInfo.GetSlotCount() + playerConfig.Cardherolimit)
        {
            weakHeroCard = FindWeakCard(playerInfo);
            if (weakHeroCard != null)
            {
                var cardCount = playerInfo.cards[weakHeroCard.Item1];
                if (cardCount >= 2 && SysRandom.Range(0, 100) < 40 + cardCount * 20 - year * 10)
                    weakHeroCard = null;
            }
        }

        // 直接复用 PlayerInfo 自动上阵的最强卡选择：按总战力取当前上限张
        var strongList = playerInfo.GetStrongCardList(playerInfo.GetSlotCount()).Select(x => x.Item1).ToList();
        // 阵容羁绊快照：阵营/职业人数与近战远程构成，供国家、职业、远近评分子项复用
        var ctx = BuildContext(playerInfo, strongList);
        // 看未来：对比下回合品质概率（>0 表示下回合品质更好，AI 更倾向存钱）
        var futureBias = GetFutureBias(year);
        // 聪明度归一化(1~99 → 0.01~1)：越高越能正确评估羁绊价值、越少手滑选错
        float smart = Mathf.Clamp(playerConfig.Intelligence, 1, 99) / 99f;

        // 计算每张卡片的加权分
        List<(CardViewControl card, float score)> scoredCards = new List<(CardViewControl card, float score)>();
        foreach (var pickCard in affordableCards)
        {
            float score = 1f;
            hasSameCard = false;

            // 如果已经拥有该卡片，增加分数
            if (cards.ContainsKey(pickCard.cardId))
            {
                score *= playerConfig.SameCardRate;

                if (year < 8)
                {
                    score *= (1 + Math.Max(0, 0.15f * (4 - cards[pickCard.cardId]))); // 优先拿低等级卡
                    if (pickCard.isHeroCard && !strongList.Contains(pickCard.cardId)) //非主力卡-权重
                        score *= 0.7f;
                }
                else
                {
                    if (pickCard.isHeroCard && !strongList.Contains(pickCard.cardId)) //非主力卡-权重
                        score *= Math.Max(.5f - (year - 8) * 0.05f + cards[pickCard.cardId] * .1f, 0.1f); //card数多可以救一救
                }

                hasSameCard = true;
            }

            // 计算并记录买卡的价格偏好 bias
            float buyBias = GetBuyCostBias(pickCard.priceI, year, playerConfig);
            score *= 1f + buyBias;

            if (pickCard.isHeroCard)
            {
                var heroCfg = HeroConfig.GetConfig(pickCard.cardId);
                bool strongExempt = heroCfg.Quality == 4; // 品质4强卡豁免软上限：超限也照常买入
                // 软上限：非强卡新卡超限时按超出张数概率拒买（每超1张 AiOverLimitRejectPerCard%）
                if (!hasSameCard && !strongExempt
                    && heroCardCount >= playerInfo.GetSlotCount() + playerConfig.Cardherolimit
                    && SysRandom.Range(0, 100) < (heroCardCount - (playerInfo.GetSlotCount() + playerConfig.Cardherolimit)) * CombatConst.AiOverLimitRejectPerCard)
                    continue;
                // 非强卡超限未拒：只能通过卖旧买新换入
                if (!hasSameCard && !strongExempt
                    && heroCardCount >= playerInfo.GetSlotCount() + playerConfig.Cardherolimit)
                {
                    if (weakHeroCard == null) //没有可以换的卡
                        continue;

                    if (pickCard.priceI < weakHeroCard.Item2)
                        continue; //没必要换更弱的卡

                    if (year > 8 && pickCard.priceI < weakHeroCard.Item2 + year - 8)
                        continue; //新卡价格还不如旧卡，没必要换
                }
                // 近战/远程与羁绊统计均按"新增英雄"计算：重复卡（升星）只吃同卡分
                bool isNewHero = !strongList.Contains(pickCard.cardId);

                // 羁绊信任固定倍率：命中即加成。friend Bias 最大、job 次之、force(强卡/国家护盾)最小
                // force：候选卡所属势力命中 LikeForce 时，按推进同阵营护盾档位(2/3/4/5/6人→Lv1~5)计
                if (LikeHasForce(playerConfig.LikeForce, heroCfg.Side))
                    score *= 1f + CombatConst.ForceBias * GetForceGain(ctx, heroCfg, isNewHero);
                // job：候选卡所属职业命中 LikeJob 时，按推进或达成职业连锁档位(1/2/3/4/5人→Lv1~5)计
                if (LikeHasJob(playerConfig.LikeJob, heroCfg))
                    score *= 1f + CombatConst.JobChainBias * GetJobGain(ctx, heroCfg, isNewHero);
                // friend：候选卡所属好友组命中 LikeFriend 时，按连线档位(2~6人)/特殊连锁组进度/每回合金币组计
                if (LikeHasFriend(playerConfig.LikeFriend, pickCard.cardId))
                    score *= 1f + CombatConst.FriendBias * GetFriendGain(ctx, pickCard.cardId);
                // 远近搭配：缺哪类补哪类，同类堆多则扣分（保留聪明度折扣）
                score *= 1f + playerConfig.BalanceFactor * GetBalanceGain(ctx, heroCfg, isNewHero) * smart;
            }

            // 如果不是已拥有的同卡，根据拥有人数惩罚热门卡
            if (!hasSameCard)
            {
                //获取现在拥有这张卡牌的玩家人数
                int playersWithThisCard = 0;
                foreach (var ckPlayer in GameManager.Instance.players)
                {
                    if (ckPlayer.cards.ContainsKey(pickCard.cardId))
                        playersWithThisCard++;
                }

                //根据拥有人数调整分数，人数越多分数越低
                if (playersWithThisCard > 0)
                {
                    float rarityFactor = 1f / (playersWithThisCard + 1);
                    score *= (float)Math.Pow(playerConfig.OwnTooMuchCardRate, playersWithThisCard);
                }
            }            

            // 聪明度越低，评分抖动越大（看走眼/手滑选到差卡）
            score *= 1f + SysRandom.Range(-1f, 1f) * (1f - smart) * 0.5f;

            // 加入分数列表
            scoredCards.Add((pickCard, score));
        }

        // 如果没有有分数的卡片：金币低于开局一定比例时才允许跳过（看未来：下回合品质更好时更倾向存钱），否则随便选一张买掉
        if (scoredCards.Count == 0)
        {
            float skipGoldRate = 0.2f + Mathf.Clamp(playerConfig.Futurerate, 0f, 1f) * Mathf.Max(0f, futureBias) * 0.15f;
            if (playerInfo.gold < CardShopManager.Instance.playerStartGold[playerInfo.pid] * skipGoldRate)
                return false;

            foreach (var c in affordableCards)
                scoredCards.Add((c, 1f));
        }

        //scoredCards的key的priceI前三3的卡分别（1.5，1.3，1.1）
        if (scoredCards.Count >= 5 && scoredCards.Max(x => x.score) < 1.6f)
        {
            var top3Cards = scoredCards.OrderByDescending(x => x.card.priceI * x.card.count).Take(3).ToList();
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
        float accumulated = Mathf.Max(0f, playerConfig.AccumulatedCostBias) * Mathf.Sqrt(year);
        int targetCost = Math.Max(1, Mathf.RoundToInt(accumulated));
        sb.AppendLine($"{playerInfo.playerNameText.text} 选卡 scoredCards数量: {scoredCards.Count}, targetCost:{targetCost}, accumulated:{accumulated:F2}");
        for (int i = 0; i < scoredCards.Count; i++)
        {
            var card = scoredCards[i];
            float cardBuyBias = GetBuyCostBias(card.card.priceI, year, playerConfig);
            sb.AppendLine($"  [{i+1}] 卡片ID: {card.card.cardId}, 名称: {card.card.cardName.text}, 分数: {card.score}, 价格: {card.card.priceI}, buyBias: {cardBuyBias:F2}");
        }

        // 聪明度越高候选池越窄（只在最优的几张里挑）；越低越宽（更容易随机到次优卡）
        int pickPool = smart >= 0.7f ? 3 : (smart >= 0.4f ? 4 : 6);
        if (scoredCards.Count > pickPool)
            scoredCards = scoredCards.Take(pickPool).ToList();

        // 根据分数计算总权重
        float totalWeight = scoredCards.Sum(item => item.score);
        float randomValue = SysRandom.Range(0f, totalWeight);

        // 根据随机值和权重选择卡片
        float cumulativeWeight = 0f;
        CardViewControl selectedCard = null;
        foreach (var item in scoredCards)
        {
            cumulativeWeight += item.score;
            if (randomValue <= cumulativeWeight)
            {
                selectedCard = item.card;
                break;
            }
        }

        // 如果没有选到卡片，返回 false
        if (selectedCard == null)
            return false;

        if (selectedCard != null)
        {
            sb.AppendLine($"选中卡片: ID={selectedCard.cardId}, 名称={selectedCard.cardName.text}, roll={randomValue}, 英雄卡={selectedCard.isHeroCard}");
        }
        else
        {
            sb.AppendLine("未选中任何卡片");
        }      
        GameLog.Debug(sb.ToString());                

        hasSameCard = cards.ContainsKey(selectedCard.cardId);
        // 卖旧买新受次数限制：每个商店阶段最多自动卖 CombatConst.AiMaxSellPerShop 次，防止低价卡全额返还导致零成本换卡、金币永不消耗
        // 软上限：品质4强卡豁免（超限直接买，不强制卖弱）；非强卡超限且未被拒买时才卖旧买新
        // 硬上限（PlayerMaxHeroCards）：背包已满时新英雄必须卖弱卡腾位才能买入，品质4也不例外，否则购买必然失败
        bool atHardCap = heroCardCount >= CombatConst.PlayerMaxHeroCards;
        if (selectedCard.isHeroCard && heroCardCount >= playerInfo.GetSlotCount() + playerConfig.Cardherolimit && !hasSameCard && weakHeroCard != null
            && (HeroConfig.GetConfig(selectedCard.cardId).Quality != 4 || atHardCap)
            && playerInfo.aiShopSellCount < CombatConst.AiMaxSellPerShop)
        {
            playerInfo.SellCard(weakHeroCard.Item1); //卖掉最弱的卡
            playerInfo.aiShopSellCount++;
        }

        var finalBuyCount = 1;
        if (selectedCard.count > 0)
        {
            // 看未来：下回合品质更好时按 Futurerate 少囤卡、留钱；更差时照常买满
            float saveMood = 1f - Mathf.Clamp(playerConfig.Futurerate, 0f, 1f) * Mathf.Max(0f, futureBias);
            finalBuyCount = Mathf.Clamp((int)Math.Round(playerInfo.gold * 2f / 3f / selectedCard.priceI * saveMood), 1, selectedCard.count);
        }

        // 返回真实购买结果：英雄卡已满且无弱卡可卖等情况下购买会失败，
        // 此时必须返回 false 让调用方把该玩家标记为跳过本回合，否则 AI 会一直"选牌成功"却不跳过，导致选牌阶段死循环卡住
        return CardShopManager.Instance.OnPlayerBuyCard(selectedCard, playerInfo, selectedCard.cardId, selectedCard.isHeroCard, selectedCard.priceI * finalBuyCount, finalBuyCount);
    }

    private static float GetBuyCostBias(int cardPrice, int year, PlayerConfig cfg)
    {
        if (cardPrice <= 0 || cfg == null || year <= 0)
            return 0f;

        // Use sqrt(year) mapping so: sqrt(10)~3.16, sqrt(30)~5.48, sqrt(60)~7.75
        float accumulated = Mathf.Max(0f, cfg.AccumulatedCostBias) * Mathf.Sqrt(year);
        int targetCost = Mathf.Max(1, Mathf.RoundToInt(accumulated));

        if (cardPrice == targetCost)
            return 1.0f; // highest boost for exact target

        // mid tier (near target) still favored
        int lower = Math.Max(1, targetCost - 1);
        int upper = targetCost + 1;
        if (cardPrice >= lower && cardPrice <= upper)
            return 0.6f;

        // slightly lower costs acceptable
        if (cardPrice < lower)
            return 0.25f;

        // higher costs penalized progressively
        return -0.4f * (cardPrice - upper);
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
        foreach (int cardId in cards.Keys)
        {
            if (!ConfigManager.IsHeroCard(cardId))
                continue;

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

            var heroCfg = HeroConfig.GetConfig(cardId);
            var price = HeroSelectionTool.GetPrice(heroCfg);
            float value = GetLineupValue(ctx, heroCfg);
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
            ctx.sideCount[heroCfg.Side] = ctx.GetSideCount(heroCfg.Side) + 1;
            ctx.jobCount[heroCfg.Job] = ctx.GetJobCount(heroCfg.Job) + 1;
            if (HeroSelectionTool.IsMeleeHero(heroCfg))
                ctx.meleeCount++;
            else
                ctx.rangedCount++;
        }
        return ctx;
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

    // 看未来：下回合与当前回合的高品质(Q3+Q4)概率差，归一化到 -1~1（>0 表示下回合品质更好）
    private static float GetFutureBias(int year)
    {
        var curCfg = GameRoundConfig.GetConfig(Mathf.Clamp(year, 1, 100));
        var nextCfg = GameRoundConfig.GetConfig(Mathf.Clamp(year + 1, 1, 100));
        if (curCfg == null || nextCfg == null)
            return 0f;
        int curHigh = curCfg.Quality3Rate + curCfg.Quality4Rate;
        int nextHigh = nextCfg.Quality3Rate + nextCfg.Quality4Rate;
        return Mathf.Clamp((nextHigh - curHigh) / 20f, -1f, 1f);
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
        if (heroCfg.Quality == 4)
            return 1f;
        return GetSideGain(ctx, heroCfg, isNewHero);
    }

    private static float GetSideGain(AiContext ctx, HeroConfig heroCfg, bool isNewHero)
    {
        if (!isNewHero || !IsShieldSide(heroCfg.Side))
            return 0f;

        int count = ctx.GetSideCount(heroCfg.Side);
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
        if (friendCount >= CombatConst.FriendLineCounts[0])
        {
            int lv = GetTierLevel(friendCount, CombatConst.FriendLineCounts);
            gain += 0.8f + 0.25f * (lv - 1); // 已成团：档位越高收益越大
        }
        else if (friendCount == 1)
        {
            gain += 0.3f; // 差一人成团
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

                if (relCfg.SkillId == CombatConst.FriendGoldSkillSname)
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

        int sideCount = ctx.GetSideCount(heroCfg.Side);
        float sideContrib = IsShieldSide(heroCfg.Side) && sideCount >= 2 ? Mathf.Min(sideCount - 1, 5) / 5f : 0f;

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
        float forceContrib = heroCfg.Quality == 4 ? 1f : sideContrib;
        float syn = 0f;
        if (LikeHasForce(ctx.cfg.LikeForce, heroCfg.Side))
            syn += CombatConst.ForceBias * forceContrib;
        if (LikeHasJob(ctx.cfg.LikeJob, heroCfg))
            syn += CombatConst.JobChainBias * jobContrib;
        if (LikeHasFriend(ctx.cfg.LikeFriend, heroCfg.Id))
            syn += CombatConst.FriendBias * friendContrib;
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
