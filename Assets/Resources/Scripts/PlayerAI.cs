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
        var affordableCards = availableCards.Where(card => playerInfo.gold >= card.priceI).ToList();
        if (affordableCards.Count == 0)
            return false;

        bool hasSameCard = false;
        int weakHeroCardId = 0;
        int weakHeroCardPrice = 0;
        var heroCardCount = playerInfo.GetHeroCardList().Count;
        if (heroCardCount >= playerConfig.Cardherolimit)
        {
            var weakCard = FindWeakCard(playerInfo);
            weakHeroCardId = weakCard.Item1;
            weakHeroCardPrice = weakCard.Item2;
        }

        var shopCfg = ShopConfig.GetConfig(playerInfo.GamePlayed() + 1);
        if(shopCfg.Id <= 2 && affordableCards.Count < 3 - shopCfg.Id + (3 - era) * 3)
            return false;        

        //把战力前五的卡放到一个队列里
        var strongList = GetStrongCards(playerInfo, out var groupList);
        // 初始化 side 卡牌数量
        Dictionary<int, SideInfo> sideInfos = new Dictionary<int, SideInfo>();
        foreach (int cardId in strongList)
        {
            var heroConfig = HeroConfig.GetConfig(cardId);
            if (!sideInfos.TryGetValue(heroConfig.Side, out var info))
            {
                sideInfos[heroConfig.Side] = new SideInfo();
            }
            if (heroConfig.Job == "shuai")
            {
                sideInfos[heroConfig.Side].HasShuai = true;
            }
            else
            {
                sideInfos[heroConfig.Side].Count++;
            }
        }

        CardViewControl checkFirst = null;            
        // 计算每张卡片的加权分
        List<(CardViewControl card, float score)> scoredCards = new List<(CardViewControl card, float score)>();
        foreach (var pickCard in affordableCards)
        {
            float score = 1f;

            // 如果已经拥有该卡片，增加分数
            if (cards.ContainsKey(pickCard.cardId))
            {
                score *= playerConfig.sameCardRate;
                score *= (1 + Math.Max(-.5f, 0.3f * (4 - cards[pickCard.cardId]))); // 优先拿低等级卡
                if(pickCard.isHeroCard && !strongList.Contains(pickCard.cardId)) //非主力卡-权重
                    score *= 0.7f;
                hasSameCard = true;
            }

            if (!hasSameCard)
            {
                if (era == 1 && playerInfo.gold < (int)(shopCfg.RoundGold * 0.6) || era == 2 && playerInfo.gold < (int)(shopCfg.RoundGold * 0.35))
                {
                    var cardRate = Math.Max(0, (9 - availableCards.Count) * 0.05f);
                    if (UnityEngine.Random.value < playerConfig.Futurerate + cardRate)
                        return false;
                }

                //获取现在拥有这张卡牌的玩家人数
                int playersWithThisCard = 0;
                foreach (var player in GameManager.Instance.players)
                {
                    if (player.cards.ContainsKey(pickCard.cardId))
                        playersWithThisCard++;
                }

                //根据拥有人数调整分数，人数越多分数越低
                if (playersWithThisCard > 0)
                {
                    float rarityFactor = 1f / (playersWithThisCard + 1);
                    score *= (float)Math.Pow(playerConfig.OwnTooMuchCardRate, playersWithThisCard);
                }
            }

            if (pickCard.isHeroCard)
            {
                if (!cards.ContainsKey(pickCard.cardId) && heroCardCount >= playerConfig.Cardherolimit)
                {
                    if (pickCard.priceI < weakHeroCardPrice)
                        continue; //没必要换更弱的卡
                }
                var heroCfg = HeroConfig.GetConfig(pickCard.cardId);
                if (playerConfig.Pickside != 0 && heroCfg.Side != playerConfig.Pickside) //单阵营流
                    continue;
                if (playerConfig.Pickside > 0)
                {
                    if (pickCard.cardId < 100010) //主公卡一定要拿
                        score *= playerConfig.Findmasterrate;
                }
                // 根据价格区间调整分数
                float priceS = pickCard.priceI / pickCard.count;
                if (priceS < playerConfig.Pricelower || priceS > playerConfig.Priceupper)
                {
                    score *= playerConfig.Priceoutrate;
                }
                else
                {
                    var rate = priceS / (playerConfig.Pricelower / 2 + playerConfig.Priceupper / 2); //高分卡加成
                    if (rate > 1)
                        score *= rate * rate;
                }

                if(heroCardCount < 3)
                { //前几张不拿辅助卡
                    if(string.IsNullOrEmpty(heroCfg.Group) || heroCfg.Group == "help")
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
                        score *= 1.8f;
                }                

                if (strongList.Count >= 3)
                {
                    if (sideInfos.TryGetValue(heroCfg.Side, out var info))
                    {
                        if (heroCfg.Job != "shuai" && info.HasShuai)
                            score *= playerConfig.Findmasterrate * .6f;
                        else if(heroCfg.Job == "shuai" && info.Count > 1)
                            score *= playerConfig.Findmasterrate;
                    }
                }
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
                if(heroCardCount < 3)
                    continue;

                var itemCfg = ItemConfig.GetConfig(pickCard.cardId);
                var itemCount = playerInfo.GetItemCardList().Count;
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
                    if (playerInfo.gold > 60 && playerInfo.GamePlayed() >= 8)
                        score *= 1.5f;
                    else if (heroCardCount >= 3)
                    {
                        if (itemCount == 0)
                            score *= 4;
                        else if (itemCount < 3)
                            score *= 1 + (3 - itemCount) * 0.6f;
                    }
                }
                else if(itemCfg.Effect == "first")
                {
                    checkFirst = pickCard;
                }
                else if(itemCfg.Effect == "sodatk" || itemCfg.Effect == "sodhp")
                {
                    score *= playerConfig.PickSoldierUp;
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
            var top3Cards = scoredCards.OrderByDescending(x => x.card.priceI).Take(3).ToList();
            for (int i = 0; i < top3Cards.Count; i++)
            {
                var card = top3Cards[i];
                var index = scoredCards.FindIndex(x => x.card == card.card);
                scoredCards[index] = (card.card, card.score * (1.9f - i * 0.3f));
            }
        }

        var mostScore = scoredCards.Max(x => x.score);
        if (checkFirst != null && mostScore < 1 && playerInfo.gold > 40)
        {
            var index = scoredCards.FindIndex(x => x.card == checkFirst);
            scoredCards[index] = (scoredCards[index].card, scoredCards[index].score * playerConfig.PickFirst);
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
        if (selectedCard.isHeroCard && heroCardCount >= playerConfig.Cardherolimit && !hasSameCard)
            playerInfo.SellCard(weakHeroCardId); //卖掉最弱的卡

        // 购买选中的卡片
        playerInfo.BuyCard(selectedCard, selectedCard.cardId, selectedCard.isHeroCard, selectedCard.priceI, selectedCard.count);

        return true;
    }

    private static List<int> GetStrongCards(PlayerInfo playerInfo, out List<Tuple<string, int>> groupList)
    {  
        var cards = playerInfo.cards;        
        // 创建一个列表存储卡牌ID和对应的总战力
        List<(int cardId, int totalPrice)> sortDataList = new List<(int cardId, int totalPrice)>();
        groupList = new List<Tuple<string, int>>();
        foreach (int cardId in cards.Keys)
        {
            if(!ConfigManager.IsHeroCard(cardId))
                continue;

            var price = HeroSelectionTool.GetPrice(HeroConfig.GetConfig(cardId));
            sortDataList.Add((cardId, price * cards[cardId]));
        }
        // 按总战力降序排序
        sortDataList.Sort((a, b) => b.totalPrice.CompareTo(a.totalPrice));

        // 将最强的前五张卡的ID加入队列
        List<int> strongCardIds = new List<int>();
        for (int i = 0; i < Math.Min(5, sortDataList.Count); i++)
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
        }
        return strongCardIds;
    }

    public static Tuple<int, int> FindWeakCard(PlayerInfo playerInfo)
    {
        var cards = playerInfo.cards;
        List<Tuple<int, int>> sortDataList = new List<Tuple<int, int>>();
        foreach (int cardId in cards.Keys)
        {
            if (!ConfigManager.IsHeroCard(cardId))
                continue;

            var price = HeroSelectionTool.GetPrice(HeroConfig.GetConfig(cardId));
            sortDataList.Add(new Tuple<int, int>(cardId, price * cards[cardId]));
        }
        sortDataList.Sort((a, b) => b.Item2.CompareTo(a.Item2));

        var weakCard = sortDataList[sortDataList.Count - 1];
        return weakCard;
    }
}
