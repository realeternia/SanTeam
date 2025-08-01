using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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
        if (availableCards.Count <= 2) //最后两张卡不选
            return false;
        
        // 按价格从高到低排序
        availableCards.Sort((a, b) => 
            int.Parse(b.price.text).CompareTo(int.Parse(a.price.text)));
        
        // 检查玩家当前拥有的卡片数量
        if (cards.Count < 4)
        {
            // 少于4张，优先购买价格高的卡片
            foreach (var card in availableCards)
            {
                int price = int.Parse(card.price.text);
                if (gold >= price)
                {
                    BuyCard(card, card.cardId, price);
                    return true;
                }
            }
        }
        else
        {
            // 等于或超过4张，优先购买已有的卡片（按价格从高到低）
            foreach (var card in availableCards)
            {
                int price = int.Parse(card.price.text);
                if (gold >= price && cards.ContainsKey(card.cardId))
                {
                    BuyCard(card, card.cardId, price);
                    return true;
                }
            }
            
            // 如果没有已有的卡片可买，购买价格最高的可用卡片
            foreach (var card in availableCards)
            {
                int price = int.Parse(card.price.text);
                if (gold >= price)
                {
                    BuyCard(card, card.cardId, price);
                    return true;
                }
            }
        }
        return false;
    }
}
