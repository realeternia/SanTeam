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
using System.Text;

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
    public int mark;
    public Dictionary<int, int> cards = new Dictionary<int, int>(); // cardid - > exp

    public Dictionary<int, int> itemEquips = new Dictionary<int, int>(); // heroId -> itemid
    public List<int> battleCards = new List<int>();


    public bool isOnTurn;
    public TMP_Text playerNameText;
    public Image playerImage;
    public TMP_Text goldText;
    public TMP_Text resultText;
    public TMP_Text fightMarkText;
    public Image playerBgImg;

    // 在 PlayerInfo 类中添加 AICardConfig 实例
    public PlayerConfig playerConfig;

    public string imgPath;
    public Color lineColor;
    public int banCount = 2; //最多两张

    public bool nextSkip = false; //下一轮skip
    public int sodatk = 0; //士兵atk强化
    public int sodhp = 0; //士兵def强化
    public int goldCostHero = 0;
    public int goldCostItem = 0;

    public CastleHUD castleHUD;

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
        resultText.text = "0分";
        fightMarkText.text = "";
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

    public void SubGold(int g, bool isHero)
    {
        gold -= g;
        goldText.text = gold.ToString();

        if(isHero)
            goldCostHero += g;
        else
            goldCostItem += g;
    }

    public void OnEra(int era)
    {
        nextSkip = false;
    }

    public void Equip(int heroId, int itemId)
    {
        foreach(var item in itemEquips)
        {
            if(item.Value == itemId)
            {
                itemEquips.Remove(item.Key);
                break;
            }
        }

        itemEquips[heroId] = itemId;
    }


    public void SellCard(int cardId)
    {
        var isHeroCard = ConfigManager.IsHeroCard(cardId);
        var price = 0;
        if(isHeroCard)
        {
            price = HeroSelectionTool.GetPrice(HeroConfig.GetConfig(cardId));
        }
        else
        {
            price = ItemConfig.GetConfig(cardId).Price;
        }

        AddGold(price * cards[cardId] / 2);
        cards.Remove(cardId);
        GameManager.Instance.PlaySound("Sounds/gold");

        battleCards.Remove(cardId);
        if(itemEquips.ContainsKey(cardId))
        {
            itemEquips.Remove(cardId);
        }
        foreach(var item in itemEquips)
        {
            if(item.Value == cardId)
            {
                itemEquips.Remove(item.Key);
                break;
            }
        }
    }

    public int GamePlayed()
    {
        return winCount + loseCount;

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

    public bool BuyCard(CardViewControl ctr, int cardId, bool isHero, int price, int count)
    {
        if (gold < price)
            return false;

        SubGold(price, isHero);
        if (!ctr.isHeroCard)
        {
            var itemCfg = ItemConfig.GetConfig(cardId);
            if (itemCfg.AutoUse)
            {
                GameManager.Instance.PlaySound("Sounds/gold");
                ctr.OnSold(this);
                if (itemCfg.Effect == "first")
                {
                    nextSkip = true;
                    CardShopManager.Instance.nextFirstPicker = pid;
                }
                else if (itemCfg.Effect == "sodatk")
                    sodatk += 2;
                else if (itemCfg.Effect == "sodhp")
                    sodhp += 2;
                return true;
            }

        }
        if (cards.TryGetValue(cardId, out int exp))
        {
            cards[cardId] = exp + count;
        }
        else
        {
            cards[cardId] = count;
        }
        GameManager.Instance.PlaySound("Sounds/gold");
        ctr.OnSold(this);
        return true;
    }

    public List<int> GetHeroCardList()
    {
        List<int> heroCardList = new List<int>();
        foreach (int cardId in cards.Keys)
        {
            if(ConfigManager.IsHeroCard(cardId))
                heroCardList.Add(cardId);
        }
        return heroCardList;
    }

    public List<int> GetItemCardList()
    {
        List<int> itemCardList = new List<int>();
        foreach (int cardId in cards.Keys)
        {
            if(ConfigManager.IsHeroCard(cardId))
                continue;

            itemCardList.Add(cardId);
        }
        return itemCardList;

    }

    public List<Tuple<int, int>> GetBattleCardList()
    {
        var strongCardIds = GetStrong5CardList();
        if(pid > 0)
            AutoCheckItem(strongCardIds);
        CheckFightMark(strongCardIds);
        return RearrangePos(strongCardIds);
    }


    private List<Tuple<int, int>> GetStrong5CardList()
    {
        List<Tuple<int, int>> sortDataList = new List<Tuple<int, int>>();

        foreach (int cardId in cards.Keys)
        {
            if (!ConfigManager.IsHeroCard(cardId))
                continue;         

            var heroConfig = HeroConfig.GetConfig(cardId);
            var heroPrice = HeroSelectionTool.GetPrice(heroConfig);

            sortDataList.Add(new Tuple<int, int>(cardId, heroPrice * HeroSelectionTool.GetCardLevel(cards[cardId])));
        }

        if(pid == 0)
        {
            sortDataList.Sort((a, b) => 
            {
                // 若 a 在 battleCards 中且 b 不在，则 a 排在前面
                if (battleCards.Contains(a.Item1) && !battleCards.Contains(b.Item1))
                    return -1;
                // 若 b 在 battleCards 中且 a 不在，则 b 排在前面
                if (!battleCards.Contains(a.Item1) && battleCards.Contains(b.Item1))
                    return 1;
                // 若两者都在或都不在 battleCards 中，则按 Item2 降序排序
                return b.Item2.CompareTo(a.Item2);
            });

            if (sortDataList.Count > 5)
                sortDataList = sortDataList.Take(5).ToList(); //按战力排出前5            
        }
        else
        {
            sortDataList.Sort((a, b) => b.Item2.CompareTo(a.Item2));

            // 获取前5卡
            var top5Cards = sortDataList.Take(5).ToList();
            
            // 计算所有卡对前5卡的friend数量，如果没有friend则item2*1.1
            for (int i = 0; i < sortDataList.Count; i++)
            {
                var currentCardId = sortDataList[i].Item1;
                var currentHeroConfig = HeroConfig.GetConfig(currentCardId);
                
                float friendCountMark = 0;

                // 检查当前卡是否与前5卡中的任何一个有friend关系
                for (int j = 0; j < top5Cards.Count; j++)
                {
                    if (currentCardId == top5Cards[j].Item1)
                        continue;

                    var friendLevel = ConfigManager.GetFriendLevel(currentCardId, top5Cards[j].Item1);
                    if (friendLevel > 0)
                        friendCountMark += 0.13f - 0.02f * j; //名次前的卡因子大
                }

                if (friendCountMark > 0)
                    sortDataList[i] = new Tuple<int, int>(currentCardId, (int)(sortDataList[i].Item2 * (1 + friendCountMark)));
            }
            
            // 重新排序
            sortDataList.Sort((a, b) => b.Item2.CompareTo(a.Item2));

            if (sortDataList.Count > 5)
                sortDataList = sortDataList.Take(5).ToList(); //按战力排出前5                 

            Dictionary<int, SideInfo> sideInfos = new Dictionary<int, SideInfo>();

            for (int i = 0; i < sortDataList.Count; i++)
            {
                var cardId = sortDataList[i].Item1;
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

            foreach (var sideItem in sideInfos)
            {
                if (!sideItem.Value.HasShuai && sideItem.Value.Count >= 2)
                {
                    var shuaiId = 100000 + sideItem.Key;
                    if (cards.ContainsKey(shuaiId))
                    {
                        sortDataList[sortDataList.Count - 1] = new Tuple<int, int>(shuaiId, 1);
                        break;
                    }
                }
            }
        }

        List<Tuple<int, int>> results = new List<Tuple<int, int>>();
        for (int i = 0; i < sortDataList.Count; i++)
            results.Add(new Tuple<int, int>(sortDataList[i].Item1, HeroSelectionTool.GetCardLevel(cards[sortDataList[i].Item1])));

        return results;

    }

    private void AutoCheckItem(List<Tuple<int, int>> results)
    {
        itemEquips.Clear();
        var itemCardList = GetItemCardList();

        if(itemCardList.Count == 0)
            return;

        for(int i = 0; i < results.Count; i++)
        {
            var heroCfg = HeroConfig.GetConfig(results[i].Item1);
            // 初始化最高得分和对应装备ID
            int maxScore = int.MinValue;
            int bestItemId = -1;
            
            // 获取英雄的各项属性
            int[] heroAttributes = { heroCfg.Str, heroCfg.Inte, heroCfg.LeadShip };

            int minAttr = heroAttributes.Min();
            int maxAttr = heroAttributes.Max();
            
            foreach(var itemId in itemCardList)
            {
                var itemCfg = ItemConfig.GetConfig(itemId);
                int score = itemCfg.Price;

                if (!string.IsNullOrEmpty(itemCfg.Attr1))
                {
                    if (itemCfg.Attr1 == "str" && heroCfg.Str == minAttr)
                        score += 25;
                    else if (itemCfg.Attr1 == "inte" && heroCfg.Inte == minAttr)
                        score += 25;
                    else if (itemCfg.Attr1 == "lead" && heroCfg.LeadShip == minAttr)
                        score += 25;
                    else if (itemCfg.Attr1 == "str" && heroCfg.Str == maxAttr)
                        score += 15;
                    else if (itemCfg.Attr1 == "inte" && heroCfg.Inte == maxAttr)
                        score += 15;
                    else if (itemCfg.Attr1 == "lead" && heroCfg.LeadShip == maxAttr)
                        score += 15;
                }
                
                // 更新最高得分和对应装备ID
                if (score > maxScore)
                {
                    maxScore = score;
                    bestItemId = itemId;
                }
            }
            
            itemEquips[results[i].Item1] = bestItemId;

            itemCardList.Remove(bestItemId);
            if(itemCardList.Count == 0)
                break;
        }
    }

    private void CheckFightMark(List<Tuple<int, int>> results)
    {
        int mark = 0;
        foreach(var item in results)
            mark += HeroSelectionTool.GetPrice(HeroConfig.GetConfig(item.Item1)) * cards[item.Item1];
        foreach (var item in itemEquips)
        {
            // 检查英雄ID是否存在于results中
            bool heroExists = false;
            foreach(var hero in results)
            {
                if(hero.Item1 == item.Key)
                {
                    heroExists = true;
                    break;
                }
            }
            if(!heroExists)
                continue;
            mark += ItemConfig.GetConfig(item.Value).Price * cards[item.Value];
        }
        fightMarkText.text = "$" + mark.ToString();
    }

    private List<Tuple<int, int>> RearrangePos(List<Tuple<int, int>> results)
    {
        // 根据 Pos 属性重新调整卡牌位置
        List<Tuple<int, int>> newResult = new List<Tuple<int, int>>() { null, null, null, null, null };
        List<Tuple<int, int>> pos12 = new List<Tuple<int, int>>();
        List<Tuple<int, int>> pos3 = new List<Tuple<int, int>>();
        List<Tuple<int, int>> pos45 = new List<Tuple<int, int>>();

        // 根据 Pos 分类卡牌
        foreach (var item in results)
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

    public void onBattleResult(bool isWin, int add)
    {
        if(isWin)
            winCount++;
        else
            loseCount++;
        mark += add;
        resultText.text = mark.ToString() + "分";
    }

    public bool HasCard(int cardId)
    {
        return cards.ContainsKey(cardId);
    }

    public bool HasFriend(int cardId)
    {
        foreach(var card in cards)
        {
            if(ConfigManager.GetFriendLevel(card.Key, cardId) > 0)
                return true;
        }
        return false;
    }
}

