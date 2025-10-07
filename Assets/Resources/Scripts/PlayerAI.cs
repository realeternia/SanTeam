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
            if (cell.banState > 0 || cell.heroId < 100100)
                continue;

            var heroConfig = HeroConfig.GetConfig(cell.heroId);
            // 检查阵营限制
            if (playerConfig.Pickside > 0 && playerConfig.Pickside == heroConfig.Side)
                continue;

            var cardPrice = HeroSelectionTool.GetPrice(heroConfig);
            if(playerConfig.Pricelower > 0 && playerConfig.Priceupper > 0)
            {
                if (playerConfig.Pricelower <= cardPrice && playerConfig.Priceupper >= cardPrice)
                    continue;
            }
            else
            {
                if (playerConfig.Pricelower > 0 && playerConfig.Pricelower <= cardPrice)
                    continue;
                if (playerConfig.Priceupper > 0 && playerConfig.Priceupper >= cardPrice)
                    continue;
            }

            if(playerConfig.Banstrongcard && heroConfig.Total < 240)
                continue;
            if(playerConfig.Banweakcard && heroConfig.Total > 215)
                continue;
            bool find = false;
            var cardsNeed = PlayerBook.GetCardNeeds(playerConfig.Id);
            foreach(var item in cardsNeed)
            {
                if(!string.IsNullOrEmpty(heroConfig.Group) && item.Item1 == heroConfig.Group)
                {
                    find = true;
                    break;
                }
            }
            if(find)
                continue;
            
            availableBans.Add(cell);            
        }

        // 从目标列表中随机选择一个进行ban
        if (availableBans.Count > 0)
        {
            int randomIndex = UnityEngine.Random.Range(0, availableBans.Count);
            availableBans[randomIndex].SetBan(pid);
        }
        else
        {
            // 如果没有满足所有条件的卡牌，选择一张满足基本条件的卡牌
            List<PickPanelCellControl> basicAvailableCells = new List<PickPanelCellControl>();
            foreach (var cell in cellControls)
            {
                if (cell.banState == 0 && cell.heroId > 100100)
                    basicAvailableCells.Add(cell);
            }
            
            if (basicAvailableCells.Count > 0)
            {
                int randomIndex = UnityEngine.Random.Range(0, basicAvailableCells.Count);
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
        if (heroCardCount >= playerConfig.Cardherolimit)
        {
            weakHeroCard = FindWeakCard(playerInfo);
            if (weakHeroCard != null)
            {
                var cardCount = playerInfo.cards[weakHeroCard.Item1];
                if (cardCount >= 2 && UnityEngine.Random.Range(0, 100) < 40 + cardCount * 20 - year * 10)
                    weakHeroCard = null;
            }
        }

        var shopCfg = ShopConfig.GetConfig(Math.Min(100, year + 1));
        if(shopCfg.Id <= 2 && affordableCards.Count < 6 - shopCfg.Id * 2 + (3 - era) * 4)
            return false;        

        //把战力前6的卡放到一个队列里
        var strongList = GetStrongCards(playerInfo, out var groupList, out var rangeCount, out var combatCount);
        // 初始化 side 卡牌数量
        Dictionary<int, SideInfo> sideInfos = new Dictionary<int, SideInfo>();
        foreach (int cardId in strongList)
        {
            var heroConfig = HeroConfig.GetConfig(cardId);
            if (!sideInfos.TryGetValue(heroConfig.Side, out var info))
                sideInfos[heroConfig.Side] = new SideInfo();
            if (heroConfig.Job == "shuai")
                sideInfos[heroConfig.Side].HasShuai = true;
            else
                sideInfos[heroConfig.Side].Count++;
        }

        CardViewControl checkFirst = null;            
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
                if (!hasSameCard && heroCardCount >= playerConfig.Cardherolimit)
                {
                    if (weakHeroCard == null) //没有可以换的卡
                        continue;

                    if (pickCard.priceI < weakHeroCard.Item2)
                        continue; //没必要换更弱的卡

                    if (year > 8 && pickCard.priceI < weakHeroCard.Item2 + year - 8)
                        continue; //新卡价格还不如旧卡，没必要换
                }
                var heroCfg = HeroConfig.GetConfig(pickCard.cardId);
                if (playerConfig.Pickside != 0) //单阵营流
                {
                    if (heroCfg.Side != playerConfig.Pickside)
                        continue;
                    if (pickCard.cardId < 100010) //主公卡一定要拿
                        score *= playerConfig.Findmasterrate;
                }

                // 根据价格区间调整分数
                if (pickCard.priceI < playerConfig.Pricelower || pickCard.priceI > playerConfig.Priceupper)
                {
                    score *= playerConfig.Priceoutrate;
                }
                else
                {
                    var rate = pickCard.priceI / (playerConfig.Pricelower / 2 + playerConfig.Priceupper / 2); //高分卡加成
                    if (rate > 1)
                        score *= rate * rate;
                }

                if (heroCardCount < 3)
                { //前几张不拿辅助卡
                    if (string.IsNullOrEmpty(heroCfg.Group) || heroCfg.Group == "help")
                        score *= 0.4f;
                }
                var needs = PlayerBook.GetCardNeeds(playerConfig.Id);
                if (!string.IsNullOrEmpty(heroCfg.Group) && needs.Exists(x => x.Item1 == heroCfg.Group))
                {
                    int count = 0;
                    var find = groupList.Find(x => x.Item1 == heroCfg.Group);
                    if (find != null)
                        count = find.Item2;
                    if (count < needs.Find(x => x.Item1 == heroCfg.Group).Item2)
                        score *= 1.6f;
                }

                if (combatCount + rangeCount >= 3)
                {
                    if (heroCfg.Pos == 1 && combatCount < rangeCount)
                        score *= 1 + (rangeCount - combatCount) * .5f;
                    else if (heroCfg.Pos > 1 && rangeCount < combatCount)
                        score *= 1 + (combatCount - rangeCount) * .5f;
                }

                if (strongList.Count >= 3)
                {
                    if (sideInfos.TryGetValue(heroCfg.Side, out var info))
                    {
                        if (heroCfg.Job != "shuai" && info.HasShuai)
                            score *= playerConfig.Findmasterrate * .6f;
                        else if (heroCfg.Job == "shuai" && info.Count > 1)
                            score *= playerConfig.Findmasterrate;
                    }
                }

                //朋友卡判定
                var friendCount = HeroSelectionTool.CountFriendInPool(pickCard.cardId);
                if (friendCount > 2)
                    score *= 1 + playerConfig.FriendFactor * (friendCount - 2) * .1f;

                var nowFriendCount = 0;
                foreach (var hero in playerInfo.cards)
                {
                    if (ConfigManager.GetFriendLevel(pickCard.cardId, hero.Key) > 0)
                        nowFriendCount++;
                }
                if(nowFriendCount > 0)
                    score *= 1 + playerConfig.FriendFactor * nowFriendCount * .25f;

                if (!hasSameCard)
                {
                    if (playerInfo.goldCostHero > 200)
                    {
                        var heroRate = (playerInfo.goldCostHero / playerInfo.goldCostHero + playerInfo.goldCostItem);
                        if (heroRate > playerConfig.HeroGoldRate)
                            score *= 0.5f;
                    }
                }
            }
            else
            {
                if (heroCardCount < 3)
                    continue;

                var itemCfg = ItemConfig.GetConfig(pickCard.cardId);
                var itemCount = playerInfo.GetItemList("attr").Count;
                if (!hasSameCard)
                {
                    if (itemCount >= playerConfig.Carditemlimit && itemCfg.Effect == "attr")
                        continue; //武器太多了
                    if (playerInfo.goldCostItem > 100)
                    {
                        var itemRate = (playerInfo.goldCostItem / playerInfo.goldCostHero + playerInfo.goldCostItem);
                        if (itemRate > playerConfig.ItemGoldRate)
                            score *= 0.5f;
                    }
                }

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
                else if (itemCfg.Effect == "first")
                {
                    checkFirst = pickCard;
                }
                else if (itemCfg.Effect == "sodatk" || itemCfg.Effect == "sodhp")
                {
                    score *= playerConfig.PickSoldierUp;
                }
                else if (itemCfg.Effect == "food")
                {
                    score *= playerConfig.PickFood;
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

            // 加入分数列表
            scoredCards.Add((pickCard, score));
        }

        // 如果没有有分数的卡片，直接返回
        if (scoredCards.Count == 0)
            return false;

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

        var mostScore = scoredCards.Max(x => x.score);
        if (checkFirst != null && mostScore < 1 && playerInfo.gold > 40)
        {
            var index = scoredCards.FindIndex(x => x.card == checkFirst);
            scoredCards[index] = (scoredCards[index].card, scoredCards[index].score * playerConfig.PickFirst);
        }

        if (mostScore <= 1.6 && (era < 3 && UnityEngine.Random.value < playerConfig.Futurerate * (1 + (2 - era) * 0.3f)))
        {
            var cardRate = Math.Min(0.5f, Math.Max(0, (18 - availableCards.Count) * 0.05f));
            var scoreRate = Math.Min(0.4f, (1.6f - mostScore) / 1.6f / 2);
            if (UnityEngine.Random.value < cardRate + scoreRate)
                return false;
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

        if(scoredCards.Count > 3)
            scoredCards = scoredCards.Take(3).ToList();     

        // 根据分数计算总权重
        float totalWeight = scoredCards.Sum(item => item.score);
        float randomValue = UnityEngine.Random.Range(0f, totalWeight);

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
        Debug.Log(sb.ToString());                

        hasSameCard = cards.ContainsKey(selectedCard.cardId);
        if (selectedCard.isHeroCard && heroCardCount >= playerConfig.Cardherolimit && !hasSameCard && weakHeroCard != null)
            playerInfo.SellCard(weakHeroCard.Item1); //卖掉最弱的卡

        var finalBuyCount = 1;
        if (selectedCard.count > 0)
            finalBuyCount = Math.Clamp(playerInfo.gold * 2 / 3 / selectedCard.priceI, 1, selectedCard.count);

        if (CardShopManager.Instance.OnPlayerBuyCard(selectedCard, playerInfo, selectedCard.cardId, selectedCard.isHeroCard, selectedCard.priceI * finalBuyCount, finalBuyCount))
        {
            AfterBuyCard(playerInfo, selectedCard.cardId, finalBuyCount, strongList);
        }

        return true;
    }

    private static List<int> GetStrongCards(PlayerInfo playerInfo, out List<Tuple<string, int>> groupList, out int rangeCount, out int combatCount)
    {  
        var cards = playerInfo.cards;        
        // 创建一个列表存储卡牌ID和对应的总战力
        List<(int cardId, int totalPrice)> sortDataList = new List<(int cardId, int totalPrice)>();
        groupList = new List<Tuple<string, int>>();
        rangeCount = 0;
        combatCount = 0;
        foreach (int cardId in cards.Keys)
        {
            if(!ConfigManager.IsHeroCard(cardId))
                continue;

            var price = HeroSelectionTool.GetPrice(HeroConfig.GetConfig(cardId));
            var cardLevel = HeroSelectionTool.GetCardLevel(cards[cardId], true);
            sortDataList.Add((cardId, price * cardLevel));
        }
        // 按总战力降序排序
        sortDataList.Sort((a, b) => b.totalPrice.CompareTo(a.totalPrice));

        // 将最强的前6张卡的ID加入队列
        List<int> strongCardIds = new List<int>();
        for (int i = 0; i < Math.Min(6, sortDataList.Count); i++)
        {
            strongCardIds.Add(sortDataList[i].cardId);

            // 获取当前卡牌的配置
            var heroConfig = HeroConfig.GetConfig(sortDataList[i].cardId);
            if(!string.IsNullOrEmpty(heroConfig.Group))
            {
                var group = heroConfig.Group;
                int existingIndex = groupList.FindIndex(x => x.Item1 == group);
                if(existingIndex >= 0)
                {
                    // 使用索引更新元组
                    var existingTuple = groupList[existingIndex];
                    groupList[existingIndex] = new Tuple<string, int>(existingTuple.Item1, existingTuple.Item2 + 1);
                }
                else
                {
                    groupList.Add(new Tuple<string, int>(group, 1));
                }
            }

            if(heroConfig.Pos == 1)
                combatCount++;
            else
                rangeCount++;
        }
        return strongCardIds;
    }

    public static Tuple<int, int> FindWeakCard(PlayerInfo playerInfo)
    {
        var cards = playerInfo.cards;
        List<Tuple<int, int>> sortDataList = new List<Tuple<int, int>>();
        int rangeCount = 0, combatCount = 0;
        foreach (int cardId in cards.Keys)
        {
            if (!ConfigManager.IsHeroCard(cardId))
                continue;

            var heroCfg = HeroConfig.GetConfig(cardId);
            if(heroCfg.Pos == 1)
                combatCount++;
            else
                rangeCount++;

            if(HeroSelectionTool.GetCardLevel(playerInfo.cards[cardId], true) >= 4) //4级以上卡不删了
                continue;

            if(playerInfo != null && playerInfo.playerConfig.InitCards != null && playerInfo.playerConfig.InitCards.Contains(cardId))
                continue; //初始卡不删

            var price = HeroSelectionTool.GetPrice(heroCfg);
            sortDataList.Add(new Tuple<int, int>(cardId, price));
        }

        if(sortDataList.Count == 0)
            return null;
        
        if(sortDataList.Count <= 1)
            return sortDataList[sortDataList.Count - 1];

        sortDataList.Sort((a, b) => b.Item2.CompareTo(a.Item2));

        var lastCard = sortDataList[sortDataList.Count - 1];
        var last2Card = sortDataList[sortDataList.Count - 2];
        var lastCardIsCombat = HeroConfig.GetConfig(lastCard.Item1).Pos == 1;
        var last2CardIsCombat = HeroConfig.GetConfig(last2Card.Item1).Pos == 1;

        var lastCardLevel = HeroSelectionTool.GetCardLevel(playerInfo.cards[lastCard.Item1], true);
        var last2CardLevel = HeroSelectionTool.GetCardLevel(playerInfo.cards[last2Card.Item1], true);

        if(last2CardLevel == lastCardLevel)
        {
            if(combatCount > rangeCount && !lastCardIsCombat && last2CardIsCombat)
                return last2Card;
            if(rangeCount > combatCount && lastCardIsCombat && !last2CardIsCombat)
                return last2Card;
        }

        return lastCard;
    }

    public static void AfterBuyCard(PlayerInfo playerInfo, int cardId, int count, List<int> strongList)
    {
        if(ConfigManager.IsHeroCard(cardId))
            return;
        
        var itemCfg = ItemConfig.GetConfig(cardId);
        if (itemCfg.Effect == "tpattr")
        {
            var itemAttr = itemCfg.Attr1;
            List<Tuple<int, float>> needList = new List<Tuple<int, float>>();
            float totalNeed = 0;
            
            foreach (var heroId in strongList)
            {
                var cardLevel = HeroSelectionTool.GetCardLevel(playerInfo.cards[heroId], true);
                var attr = HeroSelectionTool.GetCardAttr(playerInfo, heroId, cardLevel);
                
                var heroCfg = HeroConfig.GetConfig(heroId);
                int pos = heroCfg.Pos;
                
                // 获取三个属性值
                int str = attr.Str;
                int inte = attr.Inte;
                int lead = attr.Lead;
                
                // 计算总属性
                int totalAttr = str + inte + lead;
                
                // 找出最弱和次弱属性，最强和次强属性
                List<Tuple<string, int>> attrValues = new List<Tuple<string, int>>();
                attrValues.Add(new Tuple<string, int>("str", str));
                attrValues.Add(new Tuple<string, int>("inte", inte));
                attrValues.Add(new Tuple<string, int>("lead", lead));
                
                // 按属性值排序
                attrValues.Sort((a, b) => a.Item2.CompareTo(b.Item2));
                
                // 最弱和次弱属性
                int weakest = attrValues[0].Item2;
                int secondWeakest = attrValues[1].Item2;
                string weakestAttr = attrValues[0].Item1;
                
                // 最强和次强属性
                int strongest = attrValues[2].Item2;
                int secondStrongest = attrValues[1].Item2;
                string strongestAttr = attrValues[2].Item1;
                
                float needValue = 0;
                
                // 如果物品属性是最弱属性
                if (itemAttr == weakestAttr)
                {
                    needValue = Math.Abs(weakest - secondWeakest);
                }
                // 如果物品属性是最强属性
                else if (itemAttr == strongestAttr)
                {
                    if (pos == 3)
                    {
                        needValue = strongest - secondStrongest;
                    }
                    else if (pos == 1)
                    {
                        needValue = (strongest - secondStrongest) / 2f;
                    }
                }
                
                // 需求度乘以总属性/300
                needValue *= (float)totalAttr / 300f;
                
                // 确保需求度不为负数
                needValue = Math.Max(0.1f, needValue);
                
                needList.Add(new Tuple<int, float>(heroId, needValue));
                totalNeed += needValue;
            }
            
            // 如果所有英雄的需求度都是0，则随机选择一个
            if (totalNeed <= 0 && needList.Count > 0)
            {
                int randomIndex = UnityEngine.Random.Range(0, needList.Count);
                var targetHeroId = needList[randomIndex].Item1;
                for (int i = 0; i < count; i++)
                    playerInfo.UseItemToHero(targetHeroId, cardId);
            }
            // 否则进行加权随机选择
            else if (needList.Count > 0)
            {
                float randomValue = UnityEngine.Random.Range(0, totalNeed);
                float accumulatedNeed = 0;
                int targetHeroId = 0;
                
                foreach (var item in needList)
                {
                    accumulatedNeed += item.Item2;
                    if (accumulatedNeed >= randomValue)
                    {
                        targetHeroId = item.Item1;
                        break;
                    }
                }
                
                for (int i = 0; i < count; i++)
                    playerInfo.UseItemToHero(targetHeroId, cardId);
            }
        }
    }
}
