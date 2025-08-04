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

    private int ai_price_lower = 0;
    private int ai_price_upper = 0;
    private float ai_price_out_rate = 0; //价格区间外卡牌的兴趣度折扣
    private float ai_same_card_rate = 0; //已经拥有卡牌的兴趣倍率
    private int ai_card_limit = 8; //卡牌上限
    private float ai_future_rate = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
  		targetImage = GetComponent<Image>();
    }

    public void Init(int id, string name, string imgPath, int g)
    {
        pid = id;
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
            ai_future_rate = UnityEngine.Random.Range(0, 3) * 0.1f + 0.2f;            
            ai_same_card_rate = (UnityEngine.Random.Range(0, 20) + 20) * 0.1f;
        }
        else if(roll == 1) // 中费流
        {
            ai_price_lower = 8;
            ai_price_upper = 10 + UnityEngine.Random.Range(0, 2);
            ai_card_limit = 8 + UnityEngine.Random.Range(0, 2);
            ai_future_rate = UnityEngine.Random.Range(0, 4) * 0.1f + 0.3f; 
            ai_same_card_rate = (UnityEngine.Random.Range(0, 20) + 30) * 0.1f;
        }
        else
        {
            ai_price_lower = 10;
            ai_price_upper = 99;
            ai_card_limit = 7 + UnityEngine.Random.Range(0, 2);
            ai_future_rate = UnityEngine.Random.Range(0, 5) * 0.1f + 0.4f;
            ai_same_card_rate = (UnityEngine.Random.Range(0, 20) + 40) * 0.1f;
        }
        ai_price_out_rate = 0.1f + UnityEngine.Random.Range(0, 3) * 0.1f;
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
        int weakCardTotal = 0;        
        if(cards.Count >= ai_card_limit)
        {
            var weakCard = FindWeakCard();
            weakCardId = weakCard.Item1;
            weakCardTotal = weakCard.Item2;
        }

        // 计算每张卡片的加权分
        List<(CardViewControl card, float score)> scoredCards = new List<(CardViewControl card, float score)>();
        foreach (var pickCard in affordableCards)
        {
            float score = 1f;

            // 根据价格区间调整分数
            if (pickCard.priceI < ai_price_lower || pickCard.priceI > ai_price_upper)
            {
                score *= ai_price_out_rate;
            }

            // 如果已经拥有该卡片，增加分数
            if (cards.ContainsKey(pickCard.cardId))
            {
                score *= ai_same_card_rate;
                score *= (1 + Math.Max(0.2f, 0.3f * (4 - cards[pickCard.cardId]))); // 优先拿低等级卡
                hasSameCard = true;
            }
            else if(cards.Count >= ai_card_limit)
            {
                if(HeroConfig.GetConfig((uint)pickCard.cardId).Total < weakCardTotal)
                    continue; //没必要换更弱的卡
            }

            score *= HeroSelectionTool.GetTotalPriceRate(pickCard.cardId); //性价比

            // 加入分数列表
            scoredCards.Add((pickCard, score));
        }

        // 如果没有有分数的卡片，直接返回
        if (scoredCards.Count == 0)
            return false;

        if(!hasSameCard)
        {
            if(era < 2 && gold < 26 || era < 3 && gold < 13)
            {
                if(UnityEngine.Random.value < ai_future_rate)
                return false;
            }
            else if(availableCards.Count <= 6 && era < 3 && gold < 31) //最后几张牌考虑放弃
            {
                if(UnityEngine.Random.value < ai_future_rate  + (30 - gold) * 0.03f + (6 - availableCards.Count) * 0.08f + (2 - era) * 0.15f)
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

        if (cards.Count >= ai_card_limit && !cards.ContainsKey(selectedCard.cardId))
        {
            SellCard(weakCardId); //卖掉最弱的卡
        }            

        // 购买选中的卡片
        BuyCard(selectedCard, selectedCard.cardId, selectedCard.priceI);
        return true;
    }

    public Tuple<int, int> FindWeakCard()
    {
        List<Tuple<int, int>> sortDataList = new List<Tuple<int, int>>();
        foreach (int cardId in cards.Keys)
        {
            var heroConfig = HeroConfig.GetConfig((uint)cardId);
            sortDataList.Add(new Tuple<int, int>(cardId, heroConfig.Total * (9 + cards[cardId]) / 10 ));
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
            var heroConfig = HeroConfig.GetConfig((uint)cardId);
            sortDataList.Add(new Tuple<int, int>(cardId, heroConfig.Total * (9 + cards[cardId]) / 10 ));
        }
        sortDataList.Sort((a, b) => b.Item2.CompareTo(a.Item2));

        List<Tuple<int, int>> result = new List<Tuple<int, int>>();
        for(int i = 0; i < sortDataList.Count; i++)
            result.Add(new Tuple<int, int>(sortDataList[i].Item1, HeroSelectionTool.GetCardLevel(cards[sortDataList[i].Item1])));
        if (result.Count >= 5)
            result = result.Take(5).ToList(); //按战力排出前5

        //result按射程排序，射程远的在后面
        result.Sort((a, b) => HeroConfig.GetConfig((uint)a.Item1).Range.CompareTo(HeroConfig.GetConfig((uint)b.Item1).Range));

        //results[0]和resuls[3]对比，results[1]和resuls[4]对比，如果射程相等，等级更高的往后放
        if(result.Count >= 4 && HeroConfig.GetConfig((uint)result[0].Item1).Range == HeroConfig.GetConfig((uint)result[3].Item1).Range)
        {
            if(result[0].Item2 > result[3].Item2)
            {
                var temp = result[0];
                result[0] = result[3];
                result[3] = temp;
            }
        }
        if(result.Count >= 5 && HeroConfig.GetConfig((uint)result[1].Item1).Range == HeroConfig.GetConfig((uint)result[4].Item1).Range)
        {
            if(result[1].Item2 > result[4].Item2)
            {
                var temp = result[1];
                result[1] = result[4];
                result[4] = temp;
            }
        }
        // 如果results[0]或results[1]的job是shuai，而且results[2]不是shuai，互换results[0]和results[2]
        if(result.Count >= 3 && HeroConfig.GetConfig((uint)result[0].Item1).Job == "shuai" && HeroConfig.GetConfig((uint)result[2].Item1).Job != "shuai")
        {
            var temp = result[0];
            result[0] = result[2];
            result[2] = temp;
        }
        else if(result.Count >= 3 && HeroConfig.GetConfig((uint)result[1].Item1).Job == "shuai" && HeroConfig.GetConfig((uint)result[2].Item1).Job != "shuai")
        {
            var temp = result[0];
            result[0] = result[2];
            result[2] = temp;
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
