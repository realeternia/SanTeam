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
    public static void CheckBan(PlayerInfo playerInfo, List<PickPanelCellControl> cellControls)
    {
        var playerConfig = playerInfo.playerConfig;
        var pid = playerInfo.pid;

        // 根据playerConfig的配置过滤可ban的英雄
        List<PickPanelCellControl> availableBans = new List<PickPanelCellControl>();

        // 首先筛选出未被ban且不是主公的英雄
        foreach (var cell in cellControls)
        {
            if (cell.banState > 0 || ConfigManager.IsKingHero(cell.heroId))
                continue;

            var heroConfig = HeroConfig.GetConfig(cell.heroId);
            // 检查阵营限制
            if (playerConfig.Pickside > 0 && playerConfig.Pickside == heroConfig.Side)
                continue;

            // 强弱卡改按品质判定：强卡=品质4；弱卡=品质1/2
            // Banstrongcard：只允许 ban 品质4（强卡）
            if (playerConfig.Banstrongcard && heroConfig.Quality != 4)
                continue;
            // Banweakcard：只允许 ban 品质1/2（弱卡），品质3/4 视为强卡不参与
            if (playerConfig.Banweakcard && heroConfig.Quality >= 3)
                continue;
            availableBans.Add(cell);            
        }

        // 从目标列表中随机选择一个进行ban
        if (availableBans.Count > 0)
        {
            int randomIndex = SysRandom.Range(0, availableBans.Count);
            availableBans[randomIndex].SetBan(pid);
        }
        else
        {
            // 如果没有满足所有条件的卡牌，选择一张满足基本条件的卡牌
            List<PickPanelCellControl> basicAvailableCells = new List<PickPanelCellControl>();
            foreach (var cell in cellControls)
            {
                if (cell.banState == 0 && !ConfigManager.IsKingHero(cell.heroId))
                    basicAvailableCells.Add(cell);
            }
            
            if (basicAvailableCells.Count > 0)
            {
                int randomIndex = SysRandom.Range(0, basicAvailableCells.Count);
                basicAvailableCells[randomIndex].SetBan(pid);
            }
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
                score *= playerConfig.sameCardRate;

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

            if (pickCard.isHeroCard)
            {
                if (!hasSameCard && heroCardCount >= playerInfo.GetSlotCount() + playerConfig.Cardherolimit)
                {
                    if (weakHeroCard == null) //没有可以换的卡
                        continue;

                    if (pickCard.priceI < weakHeroCard.Item2)
                        continue; //没必要换更弱的卡

                    if (year > 8 && pickCard.priceI < weakHeroCard.Item2 + year - 8)
                        continue; //新卡价格还不如旧卡，没必要换
                }
                var heroCfg = HeroConfig.GetConfig(pickCard.cardId);
                if (playerConfig.Pickside != 0) //单阵营流：硬过滤非本阵营卡
                {
                    if (heroCfg.Side != playerConfig.Pickside)
                        continue;
                    if (ConfigManager.IsKingHero(pickCard.cardId)) //主公卡一定要拿
                        score *= playerConfig.Findmasterrate;
                }

                // 近战/远程与羁绊统计均按“新增英雄”计算：重复卡（升星）只吃同卡与强度分
                bool isNewHero = !strongList.Contains(pickCard.cardId);
                int ownCount = cards.TryGetValue(pickCard.cardId, out var own) ? own : 0;

                // 强度分：品质/面板/升星进度（强度因子）——硬实力直观，不吃聪明度折扣
                score *= 1f + playerConfig.PowerFactor * GetPowerMetric(heroCfg, ownCount);
                // 国家：推进或达成同阵营护盾档位(2/3/4/5/6人→Lv1~5)；聪明度越低越看不清羁绊价值
                score *= 1f + playerConfig.SideFactor * GetSideGain(ctx, heroCfg, isNewHero) * smart;
                // 职业：推进或达成职业连锁档位(1/2/3/4/5人→Lv1~5)
                score *= 1f + playerConfig.JobFactor * GetJobGain(ctx, heroCfg, isNewHero) * smart;
                // 好友：连线档位(2~6人)/特殊连锁组进度/每回合金币组
                score *= 1f + playerConfig.FriendFactor * GetFriendGain(ctx, pickCard.cardId) * smart;
                // 远近搭配：缺哪类补哪类，同类堆多则扣分
                score *= 1f + playerConfig.BalanceFactor * GetBalanceGain(ctx, heroCfg, isNewHero) * smart;

                // 主公：同阵营已有基础时优先拿下（国家护盾叠主公加成）
                if (ConfigManager.IsKingHero(heroCfg.Id) && ctx.GetSideCount(heroCfg.Side) >= 1)
                    score *= Mathf.Max(1f, playerConfig.Findmasterrate);
            }
            else
            {
                if (heroCardCount < 3)
                    continue;

                var itemCfg = ItemConfig.GetConfig(pickCard.cardId);
                var itemCount = playerInfo.GetItemList("attr").Count;

                if (itemCfg.Effect == "attr" && !hasSameCard)
                {
                    if (playerInfo.gold > 60 && year >= 8)
                        score *= 1.5f;
                    else if (heroCardCount >= 3)
                    {
                        if (itemCount == 0)
                            score *= 4;
                        else if (itemCount < 3)
                            score *= 1 + (3 - itemCount) * 0.6f;
                    }
                }
                else if (itemCfg.Effect == "tpattr" && year <= 8)
                {
                    score *= .5f;
                }                
            }

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
        sb.AppendLine($"{playerInfo.playerNameText.text} 选卡 scoredCards数量: {scoredCards.Count}");
        for (int i = 0; i < scoredCards.Count; i++)
        {
            var card = scoredCards[i];
            sb.AppendLine($"  [{i+1}] 卡片ID: {card.card.cardId}, 名称: {card.card.cardName.text}, 分数: {card.score}, 价格: {card.card.priceI}");
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
        if (selectedCard.isHeroCard && heroCardCount >= playerInfo.GetSlotCount() + playerConfig.Cardherolimit && !hasSameCard && weakHeroCard != null)
            playerInfo.SellCard(weakHeroCard.Item1); //卖掉最弱的卡

        var finalBuyCount = 1;
        if (selectedCard.count > 0)
        {
            // 看未来：下回合品质更好时按 Futurerate 少囤卡、留钱；更差时照常买满
            float saveMood = 1f - Mathf.Clamp(playerConfig.Futurerate, 0f, 1f) * Mathf.Max(0f, futureBias);
            finalBuyCount = Mathf.Clamp((int)Math.Round(playerInfo.gold * 2f / 3f / selectedCard.priceI * saveMood), 1, selectedCard.count);
        }

        CardShopManager.Instance.OnPlayerBuyCard(selectedCard, playerInfo, selectedCard.cardId, selectedCard.isHeroCard, selectedCard.priceI * finalBuyCount, finalBuyCount);

        return true;
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

            if (HeroSelectionTool.GetCardLevel(cards[cardId], true) >= 4) //4级以上卡不删了
                continue;

            if (playerInfo.playerConfig.InitCards != null && playerInfo.playerConfig.InitCards.Contains(cardId))
                continue; //初始卡不删

            if (ConfigManager.IsKingHero(cardId))
                continue; //主公核心不删

            var heroCfg = HeroConfig.GetConfig(cardId);
            sortDataList.Add(new Tuple<int, float, int>(cardId, GetLineupValue(ctx, heroCfg), HeroSelectionTool.GetPrice(heroCfg)));
        }

        if (sortDataList.Count == 0)
            return null;

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

    // 强度分：品质(1~4) + 主属性面板(1星带品质，240为强卡基准) + 升星进度
    private static float GetPowerMetric(HeroConfig heroCfg, int ownCount)
    {
        float quality = (heroCfg.Quality - 1) / 3f;
        var rankAttr = HeroSelectionTool.GetRankAttr(heroCfg);
        float panel = Mathf.Clamp((rankAttr.Atk + rankAttr.Ap) / 240f, 0f, 1.5f);
        float star = Mathf.Clamp(HeroSelectionTool.GetCardLevel(ownCount, true), 0, 5) / 5f;
        return 0.45f * quality + 0.4f * panel + 0.15f * star;
    }

    // 该阵营是否参与同阵营护盾（野=10 不参与，不参与则无国家收益）
    private static bool IsShieldSide(int side)
    {
        var forceCfg = ConfigManager.GetForceConfig(side);
        return forceCfg != null && forceCfg.JoinFactionShield;
    }

    // 国家档位收益：加这张新英雄后能否推进/达成同阵营护盾档位(2/3/4/5/6人→Lv1~5)；重复卡不改变上阵人数
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

        float value = price * (1f + ctx.cfg.SideFactor * sideContrib
                                  + ctx.cfg.JobFactor * jobContrib
                                  + ctx.cfg.FriendFactor * friendContrib);

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
