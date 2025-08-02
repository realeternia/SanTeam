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

    private int ai_price_lower = 0;
    private int ai_price_upper = 0;
    private float ai_price_out_rate = 0; //价格区间外卡牌的兴趣度折扣
    private float ai_same_card_rate = 0; //已经拥有卡牌的兴趣倍率
    private int ai_card_limit = 8; //卡牌上限
    private float ai_sell_rate = 0.15f; //到达上限后卖牌概率
    private float ai_future_rate = 0.5f;

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

        var roll = UnityEngine.Random.Range(0, 4);
        if(roll == 0) // 低费流
        {
            ai_price_lower = 7;
            ai_price_upper = 9 + UnityEngine.Random.Range(0, 2);
            ai_card_limit = 7 + UnityEngine.Random.Range(0, 3);
            ai_future_rate = UnityEngine.Random.Range(0, 3) * 0.1f + 0.1f;            
            ai_same_card_rate = (UnityEngine.Random.Range(0, 20) + 20) * 0.1f;
        }
        else if(roll == 1) // 中费流
        {
            ai_price_lower = 8;
            ai_price_upper = 10 + UnityEngine.Random.Range(0, 2);
            ai_card_limit = 7 + UnityEngine.Random.Range(0, 2);
            ai_future_rate = UnityEngine.Random.Range(0, 4) * 0.1f + 0.15f; 
            ai_same_card_rate = (UnityEngine.Random.Range(0, 20) + 30) * 0.1f;
        }
        else
        {
            ai_price_lower = 10;
            ai_price_upper = 99;
            ai_card_limit = 6 + UnityEngine.Random.Range(0, 2);
            ai_future_rate = UnityEngine.Random.Range(0, 5) * 0.1f + 0.2f;
            ai_same_card_rate = (UnityEngine.Random.Range(0, 20) + 40) * 0.1f;
        }
        ai_price_out_rate = 0.1f + UnityEngine.Random.Range(0, 3) * 0.1f;
        ai_sell_rate = UnityEngine.Random.Range(0, 3) * 0.1f + 0.1f;
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

        if(era < 2 && gold < 24 || era < 3 && gold < 13)
        {
            if(UnityEngine.Random.value < ai_future_rate)
              return false;
        }

        bool cardLimit = cards.Count >= ai_card_limit;

        // 计算每张卡片的加权分
        List<(CardViewControl card, float score)> scoredCards = new List<(CardViewControl card, float score)>();
        foreach (var card in affordableCards)
        {
            float score = 1f;

            // 根据价格区间调整分数
            if (card.priceI < ai_price_lower || card.priceI > ai_price_upper)
            {
                score *= ai_price_out_rate;
            }

            // 如果已经拥有该卡片，增加分数
            if (cards.ContainsKey(card.cardId))
            {
                score *= ai_same_card_rate;
            }
            else if(cardLimit)
            {
                score = 0f;
                continue;
            }

            score *= HeroSelectionTool.GetTotalPriceRate(card.cardId); //性价比

            // 加入分数列表
            scoredCards.Add((card, score));
        }

        // 如果没有有分数的卡片，直接返回
        if (scoredCards.Count == 0)
            return false;

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

        // 检查是否达到卡牌上限
        if (cards.Count >= ai_card_limit && UnityEngine.Random.value > ai_sell_rate)
            return false;

        // 购买选中的卡片
        BuyCard(selectedCard, selectedCard.cardId, selectedCard.priceI);
        return true;
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
