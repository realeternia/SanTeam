using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using CommonConfig;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PlayerInfo : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private Image targetImage;
    public float blinkDuration = 1f;
    public Color startColor = Color.white;
    public Color endColor = new Color(0.1f, 0.1f, 0.1f, 0.8f);
    private float timer = 0f;

    public int pid;
    public int gold;
    public int winCount;
    public int loseCount;
    public Dictionary<int, int> cards = new Dictionary<int, int>(); // cardid - > exp

    public bool isOnTurn;
    public TMP_Text playerNameText;
    public Image playerImage;
    public TMP_Text goldText;
    public TMP_Text resultText;
    public Image playerBgImg;

    // 在 PlayerInfo 类中添加 AICardConfig 实例
    public PlayerBook.AICardConfig aiConfig;

    public string imgPath;
    public Color lineColor;
    public int banCount = 2; //最多两张

    // Start is called before the first frame update
    void Start()
    {
  		targetImage = GetComponent<Image>();
    }

    public void Init(int id, string name, string img, string colorStr, int g)
    {
        pid = id;
        playerNameText.text = name;
        imgPath = img;
        playerImage.sprite = Resources.Load<Sprite>(img);

        gold = g;
        goldText.text = g.ToString();
        resultText.text = "准备中";
        lineColor = ColorUtility.TryParseHtmlString(colorStr, out lineColor) ? lineColor : Color.white;
        playerBgImg.color = lineColor;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log($"UI 元素被抬起，位置：{eventData.position}");
        CardShopManager.Instance.UpdateCards(0);
    }    

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log($"UI 元素被按下，位置：{eventData.position}");
        CardShopManager.Instance.UpdateCards(pid);
    }

    public void AddGold(int g)
    {
        gold += g;
        goldText.text = gold.ToString();
    }

    public void SellCard(int cardId)
    {
        AddGold(HeroSelectionTool.GetPrice(HeroConfig.GetConfig(cardId)) * cards[cardId] / 2);
        cards.Remove(cardId);
        GameManager.Instance.PlaySound("Sounds/gold");
    }

    // Update is called once per frame
    void Update()
    {
        if (isOnTurn)
        {
            if (targetImage != null)
            {
                timer += Time.deltaTime;
                // 使用正弦函数计算插值因子，范围在 0 到 1 之间
                float t = (Mathf.Sin((timer / blinkDuration) * Mathf.PI * 2f) + 1f) / 2f;
                // 根据插值因子在 startColor 和 endColor 之间做差值
                targetImage.color = Color.Lerp(startColor, endColor, t);
                // 重置计时器，让其循环
                timer %= blinkDuration;
            }
        }
        else
        {
            if(targetImage != null)
            {
                if(targetImage.color != new Color(0.1f, 0.1f, 0.1f, 0.8f))
                {
                    targetImage.color = new Color(0.1f, 0.1f, 0.1f, 0.8f);
                }
            }
        }
    }

    public void CheckBan(List<PickPanelCellControl> cellControls)
    {
        // 根据aiConfig的配置过滤可ban的英雄
        List<PickPanelCellControl> availableBans = new List<PickPanelCellControl>();

        // 首先筛选出未被ban且不是主公的英雄
        foreach (var cell in cellControls)
        {
            if (cell.banState > 0 || cell.heroId < 100100)
                continue;

            var heroConfig = HeroConfig.GetConfig(cell.heroId);
            // 检查阵营限制
            if (aiConfig.pickSide > 0 && aiConfig.pickSide == heroConfig.Side)
                continue;

            var cardPrice = HeroSelectionTool.GetPrice(heroConfig);
            if(aiConfig.priceLower > 0 && aiConfig.priceUpper > 0)
            {
                if (aiConfig.priceLower <= cardPrice && aiConfig.priceUpper >= cardPrice)
                    continue;
            }
            else
            {
                if (aiConfig.priceLower > 0 && aiConfig.priceLower <= cardPrice)
                    continue;
                if (aiConfig.priceUpper > 0 && aiConfig.priceUpper >= cardPrice)
                    continue;
            }

            if(aiConfig.banStrongCard && heroConfig.Total < 240)
                continue;
            if(aiConfig.banWeakCard && heroConfig.Total > 215)
                continue;
            if(aiConfig.banRangeCard && heroConfig.Range < 20)
                continue;
            if(aiConfig.banCombatCard && heroConfig.Range > 20)
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

    public bool BuyCard(CardViewControl ctr, int cardId, int price)
    {
        if (gold < price)
        {
            return false;
        }
        gold -= price;
        goldText.text = gold.ToString();
        if (cards.TryGetValue(cardId, out int exp))
        {
            cards[cardId] = exp + 1;
        }
        else
        {
            cards[cardId] = 1;
        }
        GameManager.Instance.PlaySound("Sounds/gold");
        ctr.OnSold(this);
        return true;
    }

    public bool AiCheckBuyCard(int era)
    {
        // 获取所有未售出的卡片
        List<CardViewControl> availableCards = CardShopManager.Instance.cardViews
            .Where(card => !card.isSold)
            .ToList();

        // 如果没有可用卡片，直接返回
        if (availableCards.Count == 0)
            return false;

        // 过滤掉买不起的卡片
        var affordableCards = availableCards.Where(card => gold >= card.priceI).ToList();
        if (affordableCards.Count == 0)
            return false;

        bool hasSameCard = false;
        int weakCardId = 0;
        int weakCardPrice = 0;
        if (cards.Count >= aiConfig.cardLimit)
        {
            var weakCard = FindWeakCard();
            weakCardId = weakCard.Item1;
            weakCardPrice = weakCard.Item2;
        }

        //把战力前五的卡放到一个队列里
        var strongList = GetStrongCards(out int rangeCount, out int inteCount);

        // 初始化 side 卡牌数量
        int side1Count = 0;
        int side2Count = 0;
        int side3Count = 0;
        // 初始化特殊卡牌标志
        bool hasLiubei = false;
        bool hasCaocao = false;
        bool hasSunquan = false;

        foreach (int cardId in strongList)
        {
            // 检查是否为特殊卡牌
            if (cardId == 100001) hasLiubei = true;
            if (cardId == 100002) hasCaocao = true;
            if (cardId == 100003) hasSunquan = true;

            // 排除特殊卡牌后统计 side 数量
            if (cardId > 100010)
            {
                var heroConfig = HeroConfig.GetConfig(cardId);
                switch (heroConfig.Side)
                {
                    case 1:
                        side1Count++;
                        break;
                    case 2:
                        side2Count++;
                        break;
                    case 3:
                        side3Count++;
                        break;
                }
            }
        }

        // 计算每张卡片的加权分
        List<(CardViewControl card, float score)> scoredCards = new List<(CardViewControl card, float score)>();
        foreach (var pickCard in affordableCards)
        {
            float score = 1f;
            var pickCardCfg = HeroConfig.GetConfig(pickCard.cardId);

            // 根据价格区间调整分数
            if (pickCard.priceI < aiConfig.priceLower || pickCard.priceI > aiConfig.priceUpper)
            {
                score *= aiConfig.priceOutRate;
            }

            if(aiConfig.pickSide != 0 && pickCardCfg.Side != aiConfig.pickSide) //单阵营流
                continue;

            // 如果已经拥有该卡片，增加分数
            if (cards.ContainsKey(pickCard.cardId))
            {
                score *= aiConfig.sameCardRate;
                score *= (1 + Math.Max(0.2f, 0.3f * (4 - cards[pickCard.cardId]))); // 优先拿低等级卡
                if(strongList.Contains(pickCard.cardId)) //主力卡再增加权重
                    score *= 1.5f;
                hasSameCard = true;
            }
            else if (cards.Count >= aiConfig.cardLimit)
            {
                if (HeroSelectionTool.GetPrice(pickCardCfg) < weakCardPrice)
                    continue; //没必要换更弱的卡
            }

            if (aiConfig.pickSide > 0)
            {
                if(pickCardCfg.Id < 100010) //主公卡一定要拿
                    score *= aiConfig.findMasterRate;
            }

            if(strongList.Count >= 3)
            {
                if (rangeCount < 2)
                {
                    if (pickCardCfg.Range > 20)
                        score *= aiConfig.pickRangeCardRate; // 如果射程大于20且射程卡数量少于3，分数乘以1.3
                }
                if (inteCount < 1)
                {
                    if (pickCardCfg.Inte >= 90)
                        score *= aiConfig.pickInteCardRate; // 如果智力大于等于90且智力卡数量少于2，分数乘以1.5
                }

                if(hasLiubei && pickCardCfg.Side == 1)
                    score *= aiConfig.findMasterRate;
                else if(hasCaocao && pickCardCfg.Side == 2)
                    score *= aiConfig.findMasterRate;
                else if(hasSunquan && pickCardCfg.Side == 3)
                    score *= aiConfig.findMasterRate;

                if(side1Count >= 1 && pickCardCfg.Id == 100001)
                    score *= aiConfig.findMasterRate;
                else if(side2Count >= 1 && pickCardCfg.Id == 100002)
                    score *= aiConfig.findMasterRate;
                else if(side3Count >= 1 && pickCardCfg.Id == 100003)
                    score *= aiConfig.findMasterRate;
            }
            
            // 加入分数列表
            scoredCards.Add((pickCard, score));
        }

        // 如果没有有分数的卡片，直接返回
        if (scoredCards.Count == 0)
            return false;

        if (!hasSameCard)
        {
            if (availableCards.Count <= 7 && era < 3 && gold < 50) //最后几张牌考虑放弃
            {
                if (UnityEngine.Random.value < aiConfig.futureRate + (49 - gold) * 0.015f + (7 - availableCards.Count) * 0.08f + (2 - era) * 0.15f)
                    return false;
            }            
            else if (era < 2 && gold < 50 || era < 3 && gold < 25)
            {
                if (UnityEngine.Random.value < aiConfig.futureRate)
                    return false;
            }

        }

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

        if (cards.Count >= aiConfig.cardLimit && !cards.ContainsKey(selectedCard.cardId))
        {
            SellCard(weakCardId); //卖掉最弱的卡
        }

        // 购买选中的卡片
        BuyCard(selectedCard, selectedCard.cardId, selectedCard.priceI);
        return true;
    }

    private List<int> GetStrongCards(out int rangeCount, out int inteCount)
    {
        rangeCount = 0;
        inteCount = 0;        
        // 创建一个列表存储卡牌ID和对应的总战力
        List<(int cardId, int totalPrice)> sortDataList = new List<(int cardId, int totalPrice)>();
        foreach (int cardId in cards.Keys)
        {
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

            // 计算射程大于20的卡牌数量
            if (heroConfig.Range > 20)
                rangeCount++;

            // 计算智力大于等于90的卡牌数量
            if (heroConfig.Inte >= 90)
                inteCount++;
        }
        return strongCardIds;
    }

    public Tuple<int, int> FindWeakCard()
    {
        List<Tuple<int, int>> sortDataList = new List<Tuple<int, int>>();
        foreach (int cardId in cards.Keys)
        {
            var price = HeroSelectionTool.GetPrice(HeroConfig.GetConfig(cardId));
            sortDataList.Add(new Tuple<int, int>(cardId, price * cards[cardId] ));
        }
        sortDataList.Sort((a, b) => b.Item2.CompareTo(a.Item2));

        var weakCard = sortDataList[sortDataList.Count - 1];
        return weakCard;
    }    

    public List<Tuple<int, int>> GetBattleCardList()
    {
        List<Tuple<int, int>> sortDataList = new List<Tuple<int, int>>();
        foreach (int cardId in cards.Keys)
        {
            var heroConfig = HeroConfig.GetConfig(cardId);
            sortDataList.Add(new Tuple<int, int>(cardId, heroConfig.Total * (9 + cards[cardId]) / 10 ));
        }
        sortDataList.Sort((a, b) => b.Item2.CompareTo(a.Item2));

        List<Tuple<int, int>> result = new List<Tuple<int, int>>();
        for(int i = 0; i < sortDataList.Count; i++)
            result.Add(new Tuple<int, int>(sortDataList[i].Item1, HeroSelectionTool.GetCardLevel(cards[sortDataList[i].Item1])));
        if (result.Count >= 5)
            result = result.Take(5).ToList(); //按战力排出前5

        // 根据 Pos 属性重新调整卡牌位置
        List<Tuple<int, int>> newResult = new List<Tuple<int, int>>() { null, null, null, null, null };
        List<Tuple<int, int>> pos12 = new List<Tuple<int, int>>();
        List<Tuple<int, int>> pos3 = new List<Tuple<int, int>>();
        List<Tuple<int, int>> pos45 = new List<Tuple<int, int>>();

        // 根据 Pos 分类卡牌
        foreach (var item in result)
        {
            int pos = HeroConfig.GetConfig(item.Item1).Pos;
            if (pos == 3)
                pos45.Add(item);
            else if (pos == 2)
                pos3.Add(item);
            else
                pos12.Add(item);
        }

        // 填充 1-2 位置
        int index = 0;
        while (index < 2 && pos12.Count > 0)
        {
            newResult[index] = pos12[0];
            pos12.RemoveAt(0);
            index++;
        }

        // 填充 3 位置
        index = 2;
        if (pos3.Count > 0)
        {
            newResult[index] = pos3[0];
            pos3.RemoveAt(0);
        }

        // 填充 4-5 位置
        index = 3;
        while (index < 5 && pos45.Count > 0)
        {
            newResult[index] = pos45[0];
            pos45.RemoveAt(0);
            index++;
        }

        // 处理剩余卡牌，放到相邻位置
        List<Tuple<int, int>> remainingCards = new List<Tuple<int, int>>();
        remainingCards.AddRange(pos12);
        remainingCards.AddRange(pos3);
        remainingCards.AddRange(pos45);

        for(int i = 0; i < newResult.Count; i++)
        {
            if(newResult[i] == null && remainingCards.Count > 0)
            {
                newResult[i] = remainingCards[0];
                remainingCards.RemoveAt(0);
            }
        }
        
        return newResult;
    }

    public void onBattleResult(bool isWin)
    {
        if(isWin)
            winCount++;
        else
            loseCount++;
        resultText.text = winCount.ToString() + "胜" + loseCount.ToString() + "败";
    }
}

