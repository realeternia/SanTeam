using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Linq;
using System.Numerics;
using CommonConfig;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


// 自定义序列化属性类
[AttributeUsage(AttributeTargets.Field)]
public class CustomSerializeFieldAttribute : Attribute
{
}

public class PlayerInfo : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private Image targetImage;
    public float blinkDuration = 1f;
    public Color startColor = Color.white;
    public Color endColor = new Color(0.1f, 0.1f, 0.1f, 0.8f);
    private float timer = 0f;

    [CustomSerializeField]
    public int pid; 
    [CustomSerializeField]
    public int gold;
    public string playerName{ get { return playerConfig.Name; } }
    [CustomSerializeField]
    public int winCount;
    [CustomSerializeField]
    public int loseCount;
    [CustomSerializeField]
    public int mark;
    [CustomSerializeField]
    public Dictionary<int, int> cards = new Dictionary<int, int>(); // cardid - > exp

    [CustomSerializeField]
    public Dictionary<int, int> itemEquips = new Dictionary<int, int>(); // heroId -> itemid
    [CustomSerializeField]
    public int[] battleCards = new int[6];
    [CustomSerializeField]
    public bool isAI = false;
    public int food;
    [CustomSerializeField]
    public int maxFood;
    private float lastFoodDeductionTime = 0f;

    public bool isOnTurn;
    public TMP_Text playerNameText;
    public Image playerImage;
    public TMP_Text goldText;
    public TMP_Text resultText;
    public Image playerBgImg;
    public Image roundOverImg;

    [CustomSerializeField]
    public int playerId;  //配置表id
    // 在 PlayerInfo 类中添加 AICardConfig 实例
    public PlayerConfig playerConfig;

    public string imgPath{ get { return playerConfig.Imgpath; } }
    public Color lineColor;
    public int banCount = 2; //最多两张
    public int battleSide;

    public bool nextSkip = false; //下一轮skip
    [CustomSerializeField]
    public int sodatk = 0; //士兵atk强化
    [CustomSerializeField]
    public int sodhp = 0; //士兵def强化
    [CustomSerializeField]
    public int goldCostHero = 0;
    [CustomSerializeField]
    public int goldCostItem = 0;
    [CustomSerializeField]
    public Dictionary<int, AttrInfo> attrAddons = new Dictionary<int, AttrInfo>();

    [CustomSerializeField]
    public int lastFightMark;

    public CastleHUD castleHUD;

    // Start is called before the first frame update
    void Start()
    {
  		targetImage = GetComponent<Image>();
    }

    public void Init(int id, int pid1)
    {
        pid = id;
        playerId = pid1;
        isAI = id > 0;

        gold = 0;
        maxFood = 100;
        food = maxFood;

        SetPlayerData();
        UpdateView();
    }

    public void FirstRound()
    {
        if (playerConfig.InitGold > 0)
            AddGold(playerConfig.InitGold);
        else if (playerConfig.InitGold < 0)
            SubGold(-playerConfig.InitGold, false);
        if (playerConfig.InitCards != null)
        {
            foreach (var card in playerConfig.InitCards)
                cards[card] = 1;
        }
    }

    // init 和 load时候都会调用
    public void SetPlayerData()
    {
        playerConfig = PlayerConfig.GetConfig(playerId);
        lineColor = ColorUtility.TryParseHtmlString(playerConfig.Colorstr, out lineColor) ? lineColor : Color.white;
    }

    public void UpdateView()
    {
        playerNameText.text = playerName;        
        playerImage.sprite = Resources.Load<Sprite>(imgPath);
        goldText.text = gold.ToString();
        resultText.text = mark.ToString();
        playerBgImg.color = lineColor;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if(CardShopManager.Instance != null)
            CardShopManager.Instance.QuickView(-1);
    }    

    public void OnPointerDown(PointerEventData eventData)
    {
        if(CardShopManager.Instance != null)
            CardShopManager.Instance.QuickView(pid);

        PanelManager.Instance.SendSignal("SelectPlayer", "", pid);
    }

    public void AddGold(int g)
    {
        if(g <= 0)
         throw new ArgumentException("Gold must be greater than 0");

        gold += g;
        goldText.text = gold.ToString();
    }

    public void AddFood(int f)
    {
        if(f <= 0)
            throw new ArgumentException("Food must be greater than 0");

        food += f;
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
    
    public int SubFood(int f)
    {
        if(food <= 0)
            return 0;
        var sub = Mathf.Min(f, food);
        food -= sub;
        return sub;
    }

    public void RoundGold(int g)
    {
        g += GetItemPAttr("roundgold");
        AddGold(g);
    }

    public void OnEra(int era)
    {
        nextSkip = false;
    }

    public void OnBattleBegin()
    {
        food = maxFood;
        // 重置上次扣除粮食的时间为当前时间
        lastFoodDeductionTime = Time.time;
    }

    public void RoundFoodCost()
    {
        // 粮食扣除逻辑
        if (Time.time - lastFoodDeductionTime >= 5f) // 每5秒扣除一次粮食
        {
            // 计算时间差，每5s，扣10点粮食
            if(food < 10)
            {
                var units = WorldManager.Instance.GetUnitsMySide(battleSide);
                foreach(var unit in units)
                    unit.LackFood((float)(10 - food) / 10);
            }
            food -= 10;
            if (food < 0) food = 0;

            // 更新上次扣除粮食的时间
            lastFoodDeductionTime = Time.time;
        }
    }

    public void SetRoundOver(bool isOver)
    {
        roundOverImg.gameObject.SetActive(isOver);
    }

    public void UseItemToHero(int heroId, int itemId)
    {
        UnityEngine.Debug.Log($"UseItemToHero {heroId} {itemId}");
        AddAttrAddon(heroId, HeroSelectionTool.GetCardAttr(this, itemId, 1));
        RemoveCard(itemId, 1);
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

    public void SetBattlePos(int heroId, int pos)
    {
        if(pos < 0 || pos >= battleCards.Length)
            return;
        for(int i = 0; i < battleCards.Length; i++)
        {
            if(battleCards[i] == heroId)
            {
                battleCards[i] = 0;
                break;
            }
        }
        battleCards[pos] = heroId;
    }

    public float GetSellRate()
    {
        if(!HasItemByEffect("sellhigh"))
            return 0.5f;
        return .75f;
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

        var count = cards[cardId];
        AddGold((int)(price * count * GetSellRate()));
        RemoveCard(cardId, count);
    }

    private void RemoveCard(int cardId, int count)
    {
        if(cards.ContainsKey(cardId))
        {
            cards[cardId] -= count;
            if(cards[cardId] <= 0)
                cards.Remove(cardId);
            else
                return;
        }
        for (int i = 0; i < battleCards.Length; i++)
        {
            if (battleCards[i] == cardId)
            {
                battleCards[i] = 0;
                break;
            }
        }
        if (itemEquips.ContainsKey(cardId))
        {
            itemEquips.Remove(cardId);
        }
        foreach (var item in itemEquips)
        {
            if (item.Value == cardId)
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
        // 现有的闪烁逻辑
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
                ctr.OnSold(this, count);
                if (itemCfg.Effect == "first")
                {
                    nextSkip = true;
                    CardShopManager.Instance.jadePlayer = pid;
                }
                else if (itemCfg.Effect == "sodatk")
                    sodatk += itemCfg.Attr1Val;
                else if (itemCfg.Effect == "sodhp")
                    sodhp += itemCfg.Attr1Val;
                else if (itemCfg.Effect == "food")
                    maxFood += itemCfg.Attr1Val; 
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
        ctr.OnSold(this, count);

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

    public List<int> GetItemList(string effectName)
    {
        List<int> itemCardList = new List<int>();
        foreach (int cardId in cards.Keys)
        {
            if(ConfigManager.IsHeroCard(cardId))
                continue;
            var itemCfg = ItemConfig.GetConfig(cardId);
            if(itemCfg.Effect != effectName)
                continue;
            itemCardList.Add(cardId);
        }
        return itemCardList;
    }

    public int GetItemPAttr(string attrName)
    {
        int attrVal = 0;
        foreach (int cardId in cards.Keys)
        {
            if(ConfigManager.IsHeroCard(cardId))
                continue;
            var itemCfg = ItemConfig.GetConfig(cardId);
            if(itemCfg.Effect != "pattr")
                continue;
            if(itemCfg.Attr1 == attrName)
                attrVal += itemCfg.Attr1Val;
            else if(itemCfg.Attr2 == attrName)
                attrVal += itemCfg.Attr2Val;
        }
        return attrVal;
    }

    public bool HasItemByEffect(string effectName)
    {
        foreach (int cardId in cards.Keys)
        {
            if(ConfigManager.IsHeroCard(cardId))
                continue;
            var itemCfg = ItemConfig.GetConfig(cardId);
            if(itemCfg.Effect == effectName)
                return true;
        }
        return false;
    }

    public void AutoSetBattleCard()
    {
        var strongCardIds = GetBattleCardList(true);
        for(int i = 0; i < battleCards.Length; i++)
        {
            if (strongCardIds[i] != null)
                battleCards[i] = strongCardIds[i].Item1;
            else
                battleCards[i] = 0;
        }
    }

    public List<Tuple<int, int>> GetBattleCardList(bool isTest = false)
    {
        if(!isTest && !isAI && battleCards.Any(c => c > 0))
        {
            var saveCardIds = new List<Tuple<int, int>>();
            for(int i = 0; i < battleCards.Length; i++)
            {
                if (battleCards[i] > 0)
                {
                    var heroConfig = HeroConfig.GetConfig(battleCards[i]);
                    var heroPrice = HeroSelectionTool.GetPrice(heroConfig);
                    saveCardIds.Add(new Tuple<int, int>(battleCards[i], HeroSelectionTool.GetCardLevel(cards[heroConfig.Id], true)));
                }
            }

            UpdateFightMark(saveCardIds);
            return saveCardIds;
        }
        var strongCardIds = GetStrong6CardList();        
        if(isAI)
            AutoCheckItem(strongCardIds);
        if(!isTest)
            UpdateFightMark(strongCardIds);
        var results = RearrangePos(strongCardIds);

        if (isAI)
        {
            //把results保存到battleCards
            battleCards = new int[6];
            for (int i = 0; i < 6; i++)
                battleCards[i] = 0;
            for (int i = 0; i < results.Count; i++)
                battleCards[i] = results[i] == null ? 0 : results[i].Item1;
        }

        return results;
    }


    private List<Tuple<int, int>> GetStrong6CardList()
    {
        List<Tuple<int, int>> sortDataList = new List<Tuple<int, int>>();

        foreach (int cardId in cards.Keys)
        {
            if (!ConfigManager.IsHeroCard(cardId))
                continue;         

            var heroConfig = HeroConfig.GetConfig(cardId);
            var heroPrice = HeroSelectionTool.GetPrice(heroConfig);

            sortDataList.Add(new Tuple<int, int>(cardId, heroPrice * HeroSelectionTool.GetCardLevel(cards[cardId], true)));
        }

        sortDataList.Sort((a, b) => b.Item2.CompareTo(a.Item2));
        if(isAI)
        {
            // 获取前5卡
            var top5Cards = sortDataList.Take(6).ToList();
            
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
        }

        if(sortDataList.Count > 6)
        {
            int combatCount = 0;
            int rangeCount = 0;
            CountCard(sortDataList, ref combatCount, ref rangeCount);
            while (combatCount > 3)
            {
                if (!SwapCard(sortDataList, true))
                    break;

                CountCard(sortDataList, ref combatCount, ref rangeCount);
            }
            CountCard(sortDataList, ref combatCount, ref rangeCount);
            while (rangeCount > 3)
            {
                if (!SwapCard(sortDataList, false))
                    break;

                CountCard(sortDataList, ref combatCount, ref rangeCount);
            }
            sortDataList = sortDataList.Take(6).ToList(); //按战力排出前6   
        }

        Dictionary<int, SideInfo> sideInfos = new Dictionary<int, SideInfo>();
        for (int i = 0; i < sortDataList.Count; i++)
        {
            var cardId = sortDataList[i].Item1;
            var heroConfig = HeroConfig.GetConfig(cardId);

            if (!sideInfos.TryGetValue(heroConfig.Side, out var info))
                sideInfos[heroConfig.Side] = new SideInfo();
            if (heroConfig.Job == "shuai")
                sideInfos[heroConfig.Side].HasShuai = true;
            else
                sideInfos[heroConfig.Side].Count++;
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

        List<Tuple<int, int>> results = new List<Tuple<int, int>>();
        for (int i = 0; i < sortDataList.Count; i++)
            results.Add(new Tuple<int, int>(sortDataList[i].Item1, HeroSelectionTool.GetCardLevel(cards[sortDataList[i].Item1], true)));

        return results;

    }

    private static bool SwapCard(List<Tuple<int, int>> sortDataList, bool checkCombat)
    {
        // 找到6以内最后一张combat卡
        int lastCombatIndex = -1;
        for (int i = 5; i >= 3; i--)
        {
            var cardId = sortDataList[i].Item1;
            var heroConfig = HeroConfig.GetConfig(cardId);
            if (checkCombat && heroConfig.Pos == 1 || !checkCombat && heroConfig.Pos > 1) // combat类型
            {
                lastCombatIndex = i;
                break;
            }
        }

        if (lastCombatIndex >= 0)
        {
            UnityEngine.Debug.Log("SwapCard lastCombatIndex: " + lastCombatIndex);
            // 在6以外且index+3内（即前9名内）寻找range卡
            int rangeCardIndex = -1;
            for (int i = 6; i < Math.Min(sortDataList.Count, lastCombatIndex + 3); i++)
            {
                var cardId = sortDataList[i].Item1;
                var heroConfig = HeroConfig.GetConfig(cardId);
                if (checkCombat && heroConfig.Pos != 1 || !checkCombat && heroConfig.Pos == 1) // range类型
                {
                    rangeCardIndex = i;
                    break;
                }
            }

            // 如果找到合适的range卡，则进行交换
            UnityEngine.Debug.Log("SwapCard lastCombatIndex: " + lastCombatIndex + " rangeCardIndex: " + rangeCardIndex);
            if (rangeCardIndex >= 0)
            {
                var temp = sortDataList[lastCombatIndex];
                sortDataList[lastCombatIndex] = sortDataList[rangeCardIndex];
                sortDataList[rangeCardIndex] = temp;
            }
            else
                return false;
        }
        else
        {
            return false;
        }
        return true;
    }

    private static void CountCard(List<Tuple<int, int>> sortDataList, ref int combatCount, ref int rangeCount)
    {
        combatCount = 0;
        rangeCount = 0;
        for (int i = 0; i < 6; i++)
        {
            var cardId = sortDataList[i].Item1;
            var heroConfig = HeroConfig.GetConfig(cardId);
            if (heroConfig.Pos == 1)
                combatCount++;
            else
                rangeCount++;
        }
    }

    private void AutoCheckItem(List<Tuple<int, int>> results)
    {
        itemEquips.Clear();
        var attrItemList = GetItemList("attr");

        if(attrItemList.Count == 0)
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
            var attrDiff = maxAttr - minAttr;
            
            foreach(var itemId in attrItemList)
            {
                var itemCfg = ItemConfig.GetConfig(itemId);
                float score = itemCfg.Attr1Val * HeroSelectionTool.GetCardLevel(cards[itemId], false); //乘上等级

                if (!string.IsNullOrEmpty(itemCfg.Attr1))
                {
                    bool isMinAttr = false;
                    bool isMaxAttr = false;

                    if (itemCfg.Attr1 == "str" && heroCfg.Str == minAttr)
                        isMinAttr = true;
                    else if (itemCfg.Attr1 == "inte" && heroCfg.Inte == minAttr)
                        isMinAttr = true;
                    else if (itemCfg.Attr1 == "lead" && heroCfg.LeadShip == minAttr)
                        isMinAttr = true;
                    else if (itemCfg.Attr1 == "str" && heroCfg.Str == maxAttr)
                        isMaxAttr = true;
                    else if (itemCfg.Attr1 == "inte" && heroCfg.Inte == maxAttr)
                        isMaxAttr = true;  
                    else if (itemCfg.Attr1 == "lead" && heroCfg.LeadShip == maxAttr)
                        isMaxAttr = true;
                    
                    if(heroCfg.Pos == 1)
                    {
                        if(isMinAttr && attrDiff > 15)
                            score *= 1 + attrDiff * .015f;
                    }
                    if(isMaxAttr)
                        score *= 1.2f;
                }
                
                // 更新最高得分和对应装备ID
                if (score > maxScore)
                {
                    maxScore = (int)score;
                    bestItemId = itemId;
                }
            }

            Equip(results[i].Item1, bestItemId);

            attrItemList.Remove(bestItemId);
            if(attrItemList.Count == 0)
                break;
        }
    }

    private void UpdateFightMark(List<Tuple<int, int>> results)
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
        lastFightMark = mark / 10;
        
    }

    private List<Tuple<int, int>> RearrangePos(List<Tuple<int, int>> results)
    {
        // 根据 Pos 属性重新调整卡牌位置
        List<Tuple<int, int>> newResult = new List<Tuple<int, int>>() { null, null, null, null, null, null };
        List<Tuple<int, int>> pos123 = new List<Tuple<int, int>>();
        List<Tuple<int, int>> pos456 = new List<Tuple<int, int>>();

        // 根据 Pos 分类卡牌
        foreach (var item in results)
        {
            int pos = HeroConfig.GetConfig(item.Item1).Pos;
            if (pos == 3 || pos == 2)
                pos456.Add(item);
            else
                pos123.Add(item);
        }

        // 填充 1-2 位置
        int index = 0;
        while (index < 3 && pos123.Count > 0)
        {
            newResult[index] = pos123[0];
            pos123.RemoveAt(0);
            index++;
        }

        // 填充 4-5 位置
        index = 3;
        while (index < 6 && pos456.Count > 0)
        {
            newResult[index] = pos456[0];
            pos456.RemoveAt(0);
            index++;
        }

        // 处理剩余卡牌，放到相邻位置
        List<Tuple<int, int>> remainingCards = new List<Tuple<int, int>>();
        remainingCards.AddRange(pos123);
        remainingCards.AddRange(pos456);

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
        resultText.text = mark.ToString();
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

    public void AddAttrAddon(int cardId, AttrInfo attr)
    {
        if(!attrAddons.ContainsKey(cardId))
            attrAddons.Add(cardId, attr);
        else
            attrAddons[cardId].AddAttr(attr);
    }

    // 序列化方法：将PlayerInfo对象转换为JSON字符串
    public string Serialize()
    {
        try
        {
            // 创建一个临时类来存储需要序列化的数据
            SerializableData serializableData = new SerializableData();
            
            // 获取所有带有[CustomSerializeField]属性的字段
            var fields = GetType().GetFields();
            foreach (var field in fields)
            {
                var attribute = Attribute.GetCustomAttribute(field, typeof(CustomSerializeFieldAttribute));
                if (attribute != null)
                {
                    object fieldValue = field.GetValue(this);
                    if (fieldValue != null)
                    {
                        string stringValue = "";
                        string typeName = field.FieldType.Name;
                        
                        // 对于Unity的Color类型，特殊处理为可序列化的格式
                        if (field.FieldType == typeof(Color))
                        {
                            Color color = (Color)fieldValue;
                            stringValue = string.Format("{0},{1},{2},{3}", color.r, color.g, color.b, color.a);
                        }
                        // 对于字典类型，将其转换为JSON字符串
                        else if (fieldValue is Dictionary<int, int>)
                        {
                            Dictionary<int, int> dict = (Dictionary<int, int>)fieldValue;
                            List<string> dictEntries = new List<string>();
                            foreach (var kvp in dict)
                            {
                                dictEntries.Add(kvp.Key + ":" + kvp.Value);
                            }
                            stringValue = string.Join(",", dictEntries);
                        }
                        else if (fieldValue is Dictionary<int, AttrInfo>)
                        {
                            Dictionary<int, AttrInfo> dict = (Dictionary<int, AttrInfo>)fieldValue;
                            List<string> dictEntries = new List<string>();
                            foreach (var kvp in dict)
                            {
                                AttrInfo attr = kvp.Value;
                                dictEntries.Add(kvp.Key + ":" + JsonUtility.ToJson(attr));
                            }
                            stringValue = string.Join("; ", dictEntries);
                        }
                        // 对于数组类型，转换为逗号分隔的字符串
                        else if (fieldValue is int[])
                        {
                            int[] array = (int[])fieldValue;
                            stringValue = string.Join(",", array);
                        }
                        // 对于其他基本类型，直接存储
                        else if (field.FieldType.IsPrimitive || field.FieldType == typeof(string) || field.FieldType == typeof(decimal))
                        {
                            stringValue = fieldValue.ToString();
                        }
                        // 对于可序列化的类，使用JsonUtility
                        else if (field.FieldType.GetCustomAttributes(typeof(System.SerializableAttribute), true).Length > 0)
                        {
                            stringValue = JsonUtility.ToJson(fieldValue);
                        }
                        
                        if (!string.IsNullOrEmpty(stringValue))
                        {
                            serializableData.playerData.Add(new SaveDataPair(field.Name, stringValue));
                        }
                    }
                }
            }
            
            // 使用JsonUtility序列化
            string json = JsonUtility.ToJson(serializableData);
            return json;
        }
        catch (Exception e)
        {
            Debug.LogError("序列化PlayerInfo失败: " + e.Message);
            return null;
        }
    }
    
    // 反序列化方法：从JSON字符串恢复PlayerInfo对象
    public void Deserialize(string json)
    {
        try
        {
            if (string.IsNullOrEmpty(json))
                return;
            
            // 使用JsonUtility反序列化
            SerializableData serializableData = JsonUtility.FromJson<SerializableData>(json);
            
            // 获取所有带有[CustomSerializeField]属性的字段
            var fields = GetType().GetFields();
            
            // 遍历所有序列化的数据项
            foreach (var kvp in serializableData.playerData)
            {
                string fieldName = kvp.key;
                string stringValue = kvp.value;
                
                if (string.IsNullOrEmpty(stringValue))
                    continue;
                
                // 查找对应的字段
                var field = fields.FirstOrDefault(f => f.Name == fieldName);
                if (field == null)
                    continue;
                    
                    // 根据存储的类型信息进行反序列化
                    if (field.FieldType == typeof(Color))
                    {
                        string[] colorComponents = stringValue.Split(',');
                        if (colorComponents.Length == 4)
                        {
                            float r = float.Parse(colorComponents[0]);
                            float g = float.Parse(colorComponents[1]);
                            float b = float.Parse(colorComponents[2]);
                            float a = float.Parse(colorComponents[3]);
                            field.SetValue(this, new Color(r, g, b, a));
                        }
                    }
                    else if (field.FieldType == typeof(Dictionary<int,int>))
                    {
                        var dict = new Dictionary<int, int>();
                        string[] entries = stringValue.Split(',');
                        foreach (string entry in entries)
                        {
                            if (!string.IsNullOrEmpty(entry))
                            {
                                string[] parts = entry.Split(':');
                                if (parts.Length == 2 && int.TryParse(parts[0], out int key) && int.TryParse(parts[1], out int value))
                                {
                                    dict[key] = value;
                                }
                            }
                        }
                        field.SetValue(this, dict);
                    }
                    else if (field.FieldType == typeof(Dictionary<int,AttrInfo>))
                    {
                        var dict = new Dictionary<int, AttrInfo>();
                        string[] entries = stringValue.Split(';');
                        foreach (string entry in entries)
                        {
                            if (!string.IsNullOrEmpty(entry))
                            {
                                int colonIndex = entry.IndexOf(':');
                                if (colonIndex > 0 && int.TryParse(entry.Substring(0, colonIndex), out int key))
                                {
                                    string jsonValue = entry.Substring(colonIndex + 1);
                                    AttrInfo attrInfo = JsonUtility.FromJson<AttrInfo>(jsonValue);
                                    dict[key] = attrInfo;
                                }
                            }
                        }
                        field.SetValue(this, dict);
                    }
                    else if (field.FieldType == typeof(int[]))
                    {
                        string[] parts = stringValue.Split(',');
                        int[] array = new int[parts.Length];
                        for (int j = 0; j < parts.Length; j++)
                        {
                            if (!int.TryParse(parts[j], out array[j]))
                            {
                                array[j] = 0;
                            }
                        }
                        field.SetValue(this, array);
                    }
                    // 对于其他基本类型
                    else if (field.FieldType == typeof(int))
                    {
                        field.SetValue(this, int.Parse(stringValue));
                    }
                    else if (field.FieldType == typeof(float))
                    {
                        field.SetValue(this, float.Parse(stringValue));
                    }
                    else if (field.FieldType == typeof(bool))
                    {
                        field.SetValue(this, bool.Parse(stringValue));
                    }
                    else if (field.FieldType == typeof(string))
                    {
                        field.SetValue(this, stringValue);
                    }
                    // 对于可序列化的类，使用JsonUtility
                    else if (field.FieldType.GetCustomAttributes(typeof(System.SerializableAttribute), true).Length > 0)
                    {
                        object obj = System.Activator.CreateInstance(field.FieldType);
                        JsonUtility.FromJsonOverwrite(stringValue, obj);
                        field.SetValue(this, obj);
                    }
                
            }
        }
        catch (Exception e)
        {
            Debug.LogError("反序列化PlayerInfo失败: " + e.Message);
        }
    }
    
    // 用于JsonUtility序列化的辅助类
    [System.Serializable]
    private class SerializableData
    {
        public List<SaveDataPair> playerData = new List<SaveDataPair>();
    }

    [System.Serializable]
    private class SaveDataPair
    {
        public string key;
        public string value;

        public SaveDataPair(string key, string value)
        {
            this.key = key;
            this.value = value;
        }
    }
}

