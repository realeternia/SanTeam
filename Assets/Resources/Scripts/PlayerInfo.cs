using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using CommonConfig;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfo : MonoBehaviour
{
    private Image targetImage;
    public float blinkDuration = 1f;
    public Color startColor = Color.white;
    public Color endColor = new Color(0.1f, 0.1f, 0.1f, 0.8f);
    private float timer = 0f;

    public int gold;
    public int winCount;
    public int loseCount;
    public Dictionary<int, int> cards = new Dictionary<int, int>(); // cardid - > exp

    public bool isOnTurn;
    public TMP_Text playerNameText;
    public Image playerImage;
    public TMP_Text goldText;
    public TMP_Text resultText;

    // Start is called before the first frame update
    void Start()
    {
  		targetImage = GetComponent<Image>();
    }

    public void Init(string name, string imgPath, int g)
    {
        playerNameText.text = name;
        playerImage.sprite = Resources.Load<Sprite>(imgPath);
        gold = g;
        goldText.text = g.ToString();
        resultText.text = "准备中";
    }

    public void AddGold(int g)
    {
        gold += g;
        goldText.text = gold.ToString();
    }

    public void SellCard(int cardId)
    {
        AddGold(HeroSelectionTool.GetPrice(HeroConfig.GetConfig((uint)cardId)) * cards[cardId] / 2);
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
        ctr.OnSold();
        return true;
    }

    public bool AiCheckBuyCard()
    {
        // 获取所有未售出的卡片
        List<CardViewControl> availableCards = CardShopManager.Instance.cardViews
            .Where(card => !card.isSold)
            .ToList();  
        
        // 如果没有可用卡片，直接返回
        if (availableCards.Count < 5)
        {
            // 检查是否有价值高于9的卡
            if (!availableCards.Any(card => card.priceI >= 9))
            {
                // 随机1-4，如果大于可用卡片数量则放弃
                int randomNum = UnityEngine.Random.Range(1, 5);
                if (randomNum >= availableCards.Count)
                    return false;
            }
        }
        if (availableCards.Count <= 2) // 最后两张卡不选
            return false;

        // 检查玩家当前拥有的卡片数量
        if (cards.Count <= 4)
        {
            // 少于4张，优先购买价格高的卡片
            if(TryBuyDearCard(availableCards, 3))
                return true;
        }
        else
        {
            // 等于或超过4张，优先购买已有的卡片（按价格从高到低）
            if(TryUpgradeCard(availableCards))
               return true;
            
            // 如果没有已有的卡片可买，购买价格最高的可用卡片
            if(TryBuyDearCard(availableCards, 3))
                return true;
        }
        return false;
    }

    private bool TryBuyDearCard(List<CardViewControl> availableCards, int n)
    {
        // 按价格从高到低排序
        availableCards.Sort((a, b) => 
            b.priceI.CompareTo(a.priceI));

        // 获取价值最高的n张牌，注意处理n大于可用卡片数量的情况
        int count = Math.Min(n, availableCards.Count);
        var topNCards = availableCards.Take(count).ToList();

        // 筛选出价格不超过玩家金币的卡片
        var affordableCards = topNCards.Where(card => 
            gold >= card.priceI).ToList();

        if (affordableCards.Count == 0)
        {
            return false;
        }

        // 随机选择一张可购买的卡片
        int randomIndex = UnityEngine.Random.Range(0, affordableCards.Count);
        var selectedCard = affordableCards[randomIndex];
        int price = selectedCard.priceI;
        BuyCard(selectedCard, selectedCard.cardId, price);
        return true;
    }

    private bool TryUpgradeCard(List<CardViewControl> availableCards)
    {
        foreach (var card in availableCards)
        {
            int price = card.priceI;
            if (gold >= price && cards.ContainsKey(card.cardId))
            {
                BuyCard(card, card.cardId, price);
                return true;
            }
        }
        return false;
    }

    public List<Tuple<int, int>> GetBattleCardList()
    {
        List<Tuple<int, int>> result = new List<Tuple<int, int>>();
        foreach (int cardId in cards.Keys)
        {
            result.Add(new Tuple<int, int>(cardId, cards[cardId]));
        }
        result.Sort((a, b) => b.Item2.CompareTo(a.Item2));
        if (result.Count >= 5)
        {
            result = result.Take(5).ToList();
        }
        return result;
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
