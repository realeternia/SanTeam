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
    public Color endColor = SysColor.Player.DeadBg;
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
    // 上一局是否失败（战斗开始类技能判定用，如「文」的获得道具概率加成；首局默认false=无加成）
    [CustomSerializeField]
    public bool lastBattleLose;
    [CustomSerializeField]
    public int mark;
    [CustomSerializeField]
    public Dictionary<int, int> cards = new Dictionary<int, int>(); // cardid - > exp（仅英雄卡）
    [CustomSerializeField]
    public List<SerializableItemSlot> items = new List<SerializableItemSlot>(); // 物品实例列表：每件物品一条记录(ItemId + HeroId)，HeroId=0表示在背包未装备，非0表示装备在该英雄上；多个同 id 物品各自一条
    [CustomSerializeField]
    public int[] battleCards = new int[CombatConst.PlayerMaxSlot];
    [CustomSerializeField]
    public bool isAI = false;
    // 玩家等级体系：等级(1~10)与经验（参考金铲铲，节奏放慢一倍；10级后9个格子全解锁）
    [CustomSerializeField]
    public int level = 1;
    [CustomSerializeField]
    public int exp = 0;

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

    public string imgPath{ get { return "Textures/" + playerConfig.Imgpath; } }
    public Color lineColor;
    public int banCount = 1; //最多一张
    public int battleSide;

    public bool nextSkip = false; //下一轮skip
    [CustomSerializeField]
    public int sodatk = 0; //士兵atk强化
    [CustomSerializeField]
    public int sodhp = 0; //士兵def强化
    [CustomSerializeField]
    public int soldierLevel = 1; //士兵等级(1~30)，决定士兵攻防加成（数量由玩家等级决定）
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

        SetPlayerData();
        EnsureSoldierCells();
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

        // 每局开局：随机发一张品质1且攻+法总面板240以下魏蜀吴(阵营1/2/3)的卡片
        var starterCandidates = HeroConfig.ConfigList
            .Where(x => x.Side >= 1 && x.Side <= 3 && x.Quality == 1)
            .ToList();
        // 校验：列出被排除的240及以上强卡(仅魏蜀吴阵营)（无双强度已在 PostModify 并入 Atk，Might=0）
        var excludedStrongCards = HeroConfig.ConfigList
            .Where(x => x.Side >= 1 && x.Side <= 3 && x.Atk + x.Ap >= 240)
            .OrderBy(x => x.Atk + x.Ap)
            .Select(x => string.Format("{0}({1})总={2}", x.Name, x.Id, x.Atk + x.Ap))
            .ToList();
        GameLog.Debug(string.Format(
            "[开局发卡] pid={0} 候选弱卡数量={1}，被排除的240及以上强卡({2}张): {3}",
            pid, starterCandidates.Count, excludedStrongCards.Count, string.Join("、", excludedStrongCards)));
        if (starterCandidates.Count > 0)
        {
            var starterHero = starterCandidates[SysRandom.Range(0, starterCandidates.Count)];
            if (cards.ContainsKey(starterHero.Id))
                cards[starterHero.Id]++;
            else
                cards[starterHero.Id] = 1;
            GameLog.Debug(string.Format(
                "[开局发卡] pid={0} 随机到：{1}(id={2}, 阵营={3}, 总属性={4})",
                pid, starterHero.Name, starterHero.Id, starterHero.Side, starterHero.Atk + starterHero.Ap));
            // 开局发的随机卡直接上阵（场上英雄少于上限时）
            AutoEquipBoughtHero(starterHero.Id);
        }
        else
        {
            GameLog.Warn("[开局发卡] pid=" + pid + " 没有符合条件的弱卡可发");
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

        if(PanelManager.Instance != null)
            PanelManager.Instance.SendSignal("SelectPlayer", "", pid);
    }

    public void AddGold(int g)
    {
        if(g <= 0)
         throw new ArgumentException("Gold must be greater than 0");

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
    
    public void RoundGold(int g)
    {
        g += GetItemPAttr("roundgold");
        g += GetFriendGold();
        AddGold(g);
    }

    // 好友羁绊·每回合金币（「济」组，技能为Dumb，金币在回合发钱时按上阵阵容结算）：
    // 遍历上阵阵容统计属于该好友组的英雄数，每名+1金；至少 FriendGoldMinCount(2) 人才生效，1人不成团
    private int GetFriendGold()
    {
        if (battleCards == null)
            return 0;

        int count = 0;
        foreach (int cardId in battleCards)
        {
            if (cardId <= 0 || !ConfigManager.IsHeroCard(cardId))
                continue;
            var relIds = ConfigManager.GetHeroFriendInfo(cardId);
            if (relIds == null)
                continue;
            foreach (int relId in relIds)
            {
                var relCfg = HeroFriendConfig.GetConfig(relId);
                if (relCfg == null || relCfg.SkillId != CombatConst.FriendGoldSkillSname)
                    continue;
                // 同一英雄可能属于多个关系组，只计一次
                count++;
                break;
            }
        }
        if (count < CombatConst.FriendGoldMinCount)
            return 0;

        int bonus = count * CombatConst.FriendGoldPerMember;
        GameLog.Debug($"每回合金币：玩家{pid}上阵{count}人，获得{bonus}金");
        return bonus;
    }

    public void OnEra(int era)
    {
        nextSkip = false;
    }

    public void OnBattleBegin()
    {
    }

    public void SetRoundOver(bool isOver)
    {
        roundOverImg.gameObject.SetActive(isOver);
    }

    // 道具是否可对指定英雄卡使用：ItemConfig.LimitSkillSname 非空时，目标英雄需属于该技能的好友羁绊组（如万民书限定「仁」）
    public bool CanUseItemToHero(int heroId, int itemId)
    {
        var itemCfg = ItemConfig.GetConfig(itemId);
        if (itemCfg == null)
            return false;

        return ConfigManager.HeroHasFriendSkill(heroId, itemCfg.LimitSkillSname);
    }

    public void UseItemToHero(int heroId, int itemId)
    {
        GameLog.Debug($"UseItemToHero {heroId} {itemId}");

        if (!CanUseItemToHero(heroId, itemId))
        {
            var itemCfg = ItemConfig.GetConfig(itemId);
            if (itemCfg == null)
                GameLog.Error($"道具使用失败：道具配置不存在 itemId={itemId}");
            else
                GameLog.Warn($"道具{itemId}限定了使用对象：仅可对拥有技能「{itemCfg.LimitSkillSname}」的英雄使用，heroId={heroId} 不满足，已取消");
            return;
        }

        AddAttrAddon(heroId, HeroSelectionTool.GetCardAttr(this, itemId, 1));
        RemoveCard(itemId, 1);
    }

    // 装备到英雄的空槽：没有空槽或没有多余副本时返回false（不替换已有装备）
    public bool Equip(int heroId, int itemId)
    {
        // 已装备数量不能超过持有数量（同id装备多件可分别装备）
        int owned = GetItemCount(itemId);
        if (GetEquippedCount(itemId) >= owned)
            return false;
        if (GetHeroEquippedCount(heroId) >= MaxEquipSlots)
            return false; // 没有空槽

        // 找一件背包中的该道具实例（HeroId==0）装到英雄上
        foreach (var slot in items)
        {
            if (slot.ItemId == itemId && slot.HeroId == 0)
            {
                slot.HeroId = heroId;
                return true;
            }
        }
        return false; // 没有空闲副本可装备
    }

    // 脱下英雄身上所有装备：返回脱下的装备数量，0表示没有装备
    public int UnwearAllEquips(int heroId)
    {
        int count = 0;
        foreach (var slot in items)
        {
            if (slot.HeroId == heroId)
            {
                slot.HeroId = 0;
                count++;
            }
        }
        return count;
    }

    // 统计某道具的持有总数（含已装备与背包中的全部实例）
    public int GetItemCount(int itemId)
    {
        int count = 0;
        foreach (var slot in items)
        {
            if (slot.ItemId == itemId)
                count++;
        }
        return count;
    }

    // 某道具有多少在背包中的空闲副本（未装备，可支配）
    public int GetItemFreeCount(int itemId)
    {
        int count = 0;
        foreach (var slot in items)
        {
            if (slot.ItemId == itemId && slot.HeroId == 0)
                count++;
        }
        return count;
    }

    // 该装备已被装备的数量（跨所有英雄统计）
    public int GetEquippedCount(int itemId)
    {
        int count = 0;
        foreach (var slot in items)
        {
            if (slot.ItemId == itemId && slot.HeroId != 0)
                count++;
        }
        return count;
    }

    // 某英雄当前已装备的物品数量
    public int GetHeroEquippedCount(int heroId)
    {
        int count = 0;
        foreach (var slot in items)
        {
            if (slot.HeroId == heroId)
                count++;
        }
        return count;
    }

    // 某英雄当前装备的所有物品 id（按装备顺序）
    public List<int> GetItemIdsOnHero(int heroId)
    {
        List<int> ids = new List<int>();
        foreach (var slot in items)
        {
            if (slot.HeroId == heroId)
                ids.Add(slot.ItemId);
        }
        return ids;
    }

    // 移除指定数量的某道具实例（优先移除背包副本，不足再移除已装备的）；用于合成/消耗
    private void RemoveItemInstances(int itemId, int count)
    {
        // pass 0 只删背包副本，pass 1 删已装备副本
        for (int pass = 0; pass < 2 && count > 0; pass++)
        {
            for (int i = items.Count - 1; i >= 0 && count > 0; i--)
            {
                var slot = items[i];
                if (slot.ItemId != itemId)
                    continue;
                if (pass == 0 && slot.HeroId != 0)
                    continue;
                items.RemoveAt(i);
                count--;
            }
        }
    }

    // 布阵：返回是否成功，失败原因写入 failReason（布阵格越界/该格被小兵占用/超出上阵上限）
    public bool SetBattlePos(int heroId, int pos, out string failReason)
    {
        failReason = null;

        // 可在5x5布阵图的任意格子自由摆放
        if(pos < 0 || pos >= CombatConst.FormationCellCount)
        {
            GameLog.Warn($"SetBattlePos: 布阵格越界 pos={pos}");
            failReason = "超出布阵范围";
            return false;
        }
        // 目标格已被小兵占用则不可布阵英雄
        if(battleCards[pos] == 500001 || battleCards[pos] == 500002)
        {
            failReason = "该格已被小兵占用";
            return false;
        }

        // 目标格已有英雄 = 替换操作（旧英雄回背包），不受上限限制
        bool isReplace = battleCards[pos] > 0 && ConfigManager.IsHeroCard(battleCards[pos]);
        bool isMoving = false;
        for(int i = 0; i < battleCards.Length; i++)
        {
            if(battleCards[i] == heroId)
            {
                isMoving = true;
                break;
            }
        }
        // 替换或移动已上阵英雄不受上限限制；新英雄上空格才检查上限
        if(!isReplace && !isMoving)
        {
            int heroCount = 0;
            for(int i = 0; i < battleCards.Length; i++)
            {
                if(battleCards[i] > 0 && ConfigManager.IsHeroCard(battleCards[i]))
                    heroCount++;
            }
            if(heroCount >= GetSlotCount())
            {
                failReason = $"上阵已满({GetSlotCount()}个)，升级玩家等级可解锁更多格子";
                return false;
            }
        }
        for(int i = 0; i < battleCards.Length; i++)
        {
            if(battleCards[i] == heroId)
            {
                battleCards[i] = 0;
                break;
            }
        }
        battleCards[pos] = heroId;
        return true;
    }

    // 布阵格之间交换单位（英雄/小兵自由交换，数量守恒）
    public void SwapBattleUnits(int fromPos, int toPos)
    {
        if(fromPos < 0 || fromPos >= CombatConst.FormationCellCount || toPos < 0 || toPos >= CombatConst.FormationCellCount)
            return;
        if(fromPos == toPos)
            return;
        var tmp = battleCards[fromPos];
        battleCards[fromPos] = battleCards[toPos];
        battleCards[toPos] = tmp;
    }

    public float GetSellRate()
    {
        if(!HasItemByEffect("sellhigh"))
            return 0.5f;
        return .75f;
    }

    // sellCount<=0 表示全部卖出（英雄整组）；物品每格一件，传入1只卖一件
    public void SellCard(int cardId, int sellCount = 0)
    {
        var isHeroCard = ConfigManager.IsHeroCard(cardId);
        if (!isHeroCard)
        {
            GameLog.Warn($"物品不可出售 cardId={cardId}，仅掉落获得");
            return;
        }
        var price = HeroSelectionTool.GetPrice(HeroConfig.GetConfig(cardId));

        var count = cards.TryGetValue(cardId, out var owned) ? owned : 0;
        if (sellCount > 0)
            count = Math.Min(sellCount, count);
        AddGold((int)(price * count * GetSellRate()));
        RemoveCard(cardId, count);
    }

    private void RemoveCard(int cardId, int count)
    {
        var isHeroCard = ConfigManager.IsHeroCard(cardId);
        if (isHeroCard)
        {
            if(cards.ContainsKey(cardId))
            {
                cards[cardId] -= count;
                if(cards[cardId] <= 0)
                    cards.Remove(cardId);
                else
                    return;
            }
        }
        else
        {
            // 物品：逐实例移除（优先背包副本，不足再移除已装备的），移除已装备即同时取消其装备关系
            RemoveItemInstances(cardId, count);
        }
        for (int i = 0; i < battleCards.Length; i++)
        {
            if (battleCards[i] == cardId)
            {
                battleCards[i] = 0;
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
                if(targetImage.color != SysColor.Player.DeadBg)
                {
                    targetImage.color = SysColor.Player.DeadBg;
                }
            }
        }
    }

    public bool BuyCard(CardViewControl ctr, int cardId, bool isHero, int price, int count)
    {
        if (gold < price)
            return false;

        // 背包英雄卡上限：新英雄（尚未拥有）会占用一个卡位，达到上限不能再买
        if (isHero && !cards.ContainsKey(cardId) && GetHeroCardList().Count >= CombatConst.PlayerMaxHeroCards)
        {
            GameLog.Warn($"英雄背包已满({CombatConst.PlayerMaxHeroCards}张)，无法购买新英雄 cardId={cardId}");
            return false;
        }

        SubGold(price, isHero);
        if (isHero && cards.TryGetValue(cardId, out int exp))
        {
            cards[cardId] = exp + count;
        }
        else if (isHero)
        {
            cards[cardId] = count;
        }
        else
        {
            // 物品不参与商店购买（仅掉落获得），此处不会执行
            GameLog.Warn($"物品不参与商店购买 cardId={cardId}");
        }
        GameManager.Instance.PlaySound("Sounds/gold");
        ctr.OnSold(this, count);

        // 购买英雄卡后自动上阵：场上英雄数少于当前上限时，把新英雄放到空闲英雄格
        if (isHero)
        {
            AutoEquipBoughtHero(cardId);
            // AI买完英雄后：背包里还有空余装备就立刻给最强英雄补满（方法内部只对 AI 生效）
            AutoEquipItems();
            // 阵容变化后重算商店特效层，亮起现在仍满足自动上阵的英雄卡
            if (CardShopManager.Instance != null)
                CardShopManager.Instance.OnShow();
        }

        return true;
    }

    // 购买英雄卡自动上阵：已在场上或场上英雄达到上限则不处理，否则放到第一个空闲英雄格
    private void AutoEquipBoughtHero(int heroId)
    {
        if (!CanAutoEquipHero(heroId))
            return;
        foreach (int pos in CombatConst.HeroCells)
        {
            if (battleCards[pos] == 0)
            {
                battleCards[pos] = heroId;
                GameLog.Debug($"购买英雄 {heroId} 自动上阵到格子 {pos}");
                return;
            }
        }
    }

    // 该英雄是否满足自动上阵条件：已在场上（购买重复卡升星）或场上英雄达到上限则不满足
    public bool CanAutoEquipHero(int heroId)
    {
        if (battleCards == null)
            return false;
        for (int i = 0; i < battleCards.Length; i++)
        {
            if (battleCards[i] == heroId)
                return false; // 已上阵
        }
        int heroCount = 0;
        for (int i = 0; i < battleCards.Length; i++)
        {
            if (battleCards[i] > 0 && ConfigManager.IsHeroCard(battleCards[i]))
                heroCount++;
        }
        return heroCount < GetSlotCount();
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
        foreach (var slot in items)
        {
            var itemCfg = ItemConfig.GetConfig(slot.ItemId);
            if (itemCfg == null || itemCfg.Effect != effectName)
                continue;
            if (!itemCardList.Contains(slot.ItemId))
                itemCardList.Add(slot.ItemId);
        }
        return itemCardList;
    }

    public int GetItemPAttr(string attrName)
    {
        int attrVal = 0;
        foreach (int itemId in GetItemList("pattr"))
        {
            var itemCfg = ItemConfig.GetConfig(itemId);
            if (itemCfg == null)
                continue;
            foreach (var bonus in JobLinkManager.ParseBonuses(itemCfg.Attrs))
            {
                if (bonus.Attr == attrName)
                    attrVal += (int)bonus.Value;
            }
        }
        return attrVal;
    }

    public bool HasItemByEffect(string effectName)
    {
        foreach (var slot in items)
        {
            var itemCfg = ItemConfig.GetConfig(slot.ItemId);
            if (itemCfg != null && itemCfg.Effect == effectName)
                return true;
        }
        return false;
    }

    public void AutoSetBattleCard()
    {
        // 复用 GetBattleCardList(true) 的 RearrangePos 逻辑：英雄填到中间9格(HeroCells)
        var strongCardIds = GetBattleCardList(true);
        battleCards = new int[CombatConst.FormationCellCount];
        for(int i = 0; i < strongCardIds.Count && i < CombatConst.FormationCellCount; i++)
        {
            battleCards[i] = strongCardIds[i] == null ? 0 : strongCardIds[i].Item1;
        }
        EnsureSoldierCells();
    }

    public List<Tuple<int, int>> GetBattleCardList(bool isTest = false)
    {
        if(!isTest && !isAI && battleCards.Any(c => c > 0 && ConfigManager.IsHeroCard(c)))
        {
            // 返回长度25的列表，索引=布阵格位置(null=空位)，供战斗按格子坐标生成
            var cardList = new List<Tuple<int, int>>(new Tuple<int, int>[CombatConst.FormationCellCount]);
            for(int i = 0; i < battleCards.Length; i++)
            {
                if (battleCards[i] > 0 && ConfigManager.IsHeroCard(battleCards[i]))
                {
                    var heroConfig = HeroConfig.GetConfig(battleCards[i]);
                    cardList[i] = new Tuple<int, int>(battleCards[i], HeroSelectionTool.GetCardLevel(cards[heroConfig.Id], true));
                }
            }

            UpdateFightMark(cardList.Where(x => x != null).ToList());
            return cardList;
        }
        var strongCardIds = GetStrongCardList(GetSlotCount());        
        if(isAI)
            AutoCheckItem(strongCardIds);
        if(!isTest)
            UpdateFightMark(strongCardIds);
        var results = RearrangePos(strongCardIds, CombatConst.FormationCellCount);

        if (isAI)
        {
            //把results保存到battleCards（随后补默认小兵）
            battleCards = new int[CombatConst.FormationCellCount];
            for (int i = 0; i < results.Count; i++)
                battleCards[i] = results[i] == null ? 0 : results[i].Item1;
            EnsureSoldierCells();
        }

        return results;
    }


    // 按总战力(价格×卡等级)降序取前count张英雄卡
    public List<Tuple<int, int>> GetStrongCardList(int count)
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

        List<Tuple<int, int>> results = new List<Tuple<int, int>>();
        for (int i = 0; i < Math.Min(count, sortDataList.Count); i++)
            results.Add(new Tuple<int, int>(sortDataList[i].Item1, HeroSelectionTool.GetCardLevel(cards[sortDataList[i].Item1], true)));

        return results;
    }

    private void AutoCheckItem(List<Tuple<int, int>> results)
    {
        var attrItemList = GetItemList("attr");

        if(attrItemList.Count == 0)
            return;

        for(int i = 0; i < results.Count; i++)
        {
            var heroCfg = HeroConfig.GetConfig(results[i].Item1);

            // 找短板属性：攻/法两主属性（无双强度已在 PostModify 并入 Atk，HeroConfig 数值为 1星带品质面板）
            int[] heroAttributes = { heroCfg.Ap, heroCfg.Atk };

            int minAttr = heroAttributes.Min();
            int maxAttr = heroAttributes.Max();
            var attrDiff = maxAttr - minAttr;

            // 每个英雄最多装备3件，逐槽选择最优装备
            for(int slot = 0; slot < 3 && attrItemList.Count > 0; slot++)
            {
                // 初始化最高得分和对应装备ID
                int maxScore = int.MinValue;
                int bestItemId = -1;

                foreach(var itemId in attrItemList)
                {
                    var itemCfg = ItemConfig.GetConfig(itemId);
                    // 属性加成走 Attrs：取第一条加成做主属性（attr 装备均为 atk/ap 单条配置）
                    var itemBonuses = JobLinkManager.ParseBonuses(itemCfg.Attrs);
                    if (itemBonuses.Count == 0)
                        continue;
                    var firstAttr = itemBonuses[0];
                    float score = firstAttr.Value * HeroSelectionTool.GetCardLevel(GetItemCount(itemId), false); //乘上数量

                    if (!string.IsNullOrEmpty(firstAttr.Attr))
                    {
                        bool isMinAttr = false;
                        bool isMaxAttr = false;

                        if (firstAttr.Attr == "ap" && heroCfg.Ap == minAttr)
                            isMinAttr = true;
                        else if (firstAttr.Attr == "atk" && heroCfg.Atk == minAttr)
                            isMinAttr = true;
                        else if (firstAttr.Attr == "ap" && heroCfg.Ap == maxAttr)
                            isMaxAttr = true;
                        else if (firstAttr.Attr == "atk" && heroCfg.Atk == maxAttr)
                            isMaxAttr = true;

                        if(HeroSelectionTool.IsMeleeHero(heroCfg))
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

                if (bestItemId < 0)
                    break;

                Equip(results[i].Item1, bestItemId);

                attrItemList.Remove(bestItemId);
            }

            if(attrItemList.Count == 0)
                break;
        }
    }

    // AI每回合进商店时：背包里未装备的合成材料装备，随机两两合成高级装备（配方来自 ItemCombineConfig），只对 AI 生效
    // 反复合成直到没有可成立的配方为止（剩余材料留给装备环节）
    public void AutoCombineItems()
    {
        if (!isAI)
            return;

        int combineCount = 0;
        while (true)
        {
            // 当前背包里未装备的道具实例（同 id 多件各算一份）
            var freeIds = items.Where(slot => slot.HeroId == 0).Select(slot => slot.ItemId).ToList();

            // 筛出材料齐备的配方（同 id 作两侧时需同时满足两份数量）
            var feasibleRecipes = new List<ItemCombineConfig>();
            foreach (var rcp in ItemCombineConfig.ConfigList)
            {
                if (rcp.ItemA == rcp.ItemB)
                {
                    if (freeIds.Count(id => id == rcp.ItemA) >= rcp.ItemAcount + rcp.ItemBcount)
                        feasibleRecipes.Add(rcp);
                }
                else if (freeIds.Count(id => id == rcp.ItemA) >= rcp.ItemAcount &&
                         freeIds.Count(id => id == rcp.ItemB) >= rcp.ItemBcount)
                {
                    feasibleRecipes.Add(rcp);
                }
            }

            if (feasibleRecipes.Count == 0)
                break;

            var pick = feasibleRecipes[SysRandom.Range(0, feasibleRecipes.Count)];
            if (!CombineTwoItems(pick.ItemA, pick.ItemB))
                break; // 防御性退出，避免合成失败时死循环
            combineCount++;
        }

        if (combineCount > 0)
            GameLog.Info($"AI自动合成：{playerConfig.Name} 合成{combineCount}件高级装备");
    }

    // AI进商店/买英雄时自动穿戴
    // 仅 Effect=="attr" 可穿戴（pattr/sellhigh 为玩家级道具，不占用英雄装备槽）；只对 AI 生效
    // 分配规则：待装备道具按品质从高到低逐件分配，先看培养度（等级+exp）最高的3个英雄、按职业偏好属性第1位→第3位找匹配的空槽，
    // 都不匹配再把候选扩到第4个英雄往后继续比；全部英雄都没有匹配属性时，兜底发给有空槽的最强英雄
    public void AutoEquipItems()
    {
        if (!isAI)
            return;

        var freeItemIds = items
            .Where(slot => slot.HeroId == 0)
            .Select(slot => slot.ItemId)
            .Where(itemId => ItemConfig.HasConfig(itemId) && ItemConfig.GetConfig(itemId).Effect == "attr")
            .OrderByDescending(itemId => ItemConfig.GetConfig(itemId).Quality)
            .ToList();

        if (freeItemIds.Count == 0)
            return;

        // 候选英雄：全部英雄卡按培养度降序（GetCardLevel 单调于 exp，"等级+exp"等价于按 exp 降序）
        var heroOrder = GetHeroCardList().OrderByDescending(cardId => cards[cardId]).ToList();
        if (heroOrder.Count == 0)
            return;

        int equippedCount = 0;
        foreach (int itemId in freeItemIds)
        {
            int heroId = FindEquipHero(heroOrder, itemId);
            if (heroId == 0)
                continue; // 所有英雄装备槽都满了
            if (!Equip(heroId, itemId))
                continue;
            equippedCount++;
        }

        if (equippedCount > 0)
            GameLog.Info($"AI自动装备：{playerConfig.Name} 装备{equippedCount}件");
    }

    // 为一件装备挑英雄：第一梯队=培养度最高的3个英雄，第二梯队=第4个往后，两梯队内都按职业偏好属性第1位→第3位比对；
    // 都没有匹配时兜底取第一个有空槽的英雄（培养度高的优先），返回0表示无空槽可用
    private int FindEquipHero(List<int> heroOrder, int itemId)
    {
        var itemCfg = ItemConfig.GetConfig(itemId);
        string mainAttr = itemCfg == null ? "" : itemCfg.MainAttr;

        for (int attrIdx = 0; attrIdx < EquipAttrSlotCount; attrIdx++)
        {
            for (int i = 0; i < Math.Min(EquipTopHeroCount, heroOrder.Count); i++)
            {
                if (IsEquipAttrMatch(heroOrder[i], mainAttr, attrIdx))
                    return heroOrder[i];
            }
        }

        for (int attrIdx = 0; attrIdx < EquipAttrSlotCount; attrIdx++)
        {
            for (int i = EquipTopHeroCount; i < heroOrder.Count; i++)
            {
                if (IsEquipAttrMatch(heroOrder[i], mainAttr, attrIdx))
                    return heroOrder[i];
            }
        }

        foreach (int heroId in heroOrder)
        {
            if (GetHeroEquippedCount(heroId) < MaxEquipSlots)
                return heroId;
        }
        return 0;
    }

    // 英雄有装备空槽，且其职业偏好属性第 attrIdx 位与装备主属性一致
    private bool IsEquipAttrMatch(int heroId, string mainAttr, int attrIdx)
    {
        if (string.IsNullOrEmpty(mainAttr))
            return false;
        if (GetHeroEquippedCount(heroId) >= MaxEquipSlots)
            return false;

        var heroCfg = HeroConfig.GetConfig(heroId);
        if (heroCfg == null)
            return false;
        var jobCfg = ConfigManager.GetJobConfig(heroCfg.Job);
        if (jobCfg == null || jobCfg.EquipAttr == null || attrIdx >= jobCfg.EquipAttr.Length)
            return false;
        return jobCfg.EquipAttr[attrIdx] == mainAttr;
    }

    private void UpdateFightMark(List<Tuple<int, int>> results)
    {
        int mark = 0;
        foreach(var item in results)
            mark += HeroSelectionTool.GetPrice(HeroConfig.GetConfig(item.Item1)) * cards[item.Item1];
        // 每件已装备在登场英雄身上的物品按价格计入（价格×装备件数）
        foreach (var slot in items)
        {
            if (slot.HeroId == 0)
                continue;
            // 检查该英雄是否存在于results中
            bool heroExists = false;
            foreach(var hero in results)
            {
                if(hero.Item1 == slot.HeroId)
                {
                    heroExists = true;
                    break;
                }
            }
            if(!heroExists)
                continue;
            var itemCfg = ItemConfig.GetConfig(slot.ItemId);
            if (itemCfg == null)
                continue;
            // 物品无价格字段（仅掉落获得），按品质折算战力分（1白/2绿/3蓝/4紫 → 1/2/3/4 基础，品质5紫金按5）
            mark += itemCfg.Quality;
        }
        lastFightMark = mark / 10;
        
    }

    private List<Tuple<int, int>> RearrangePos(List<Tuple<int, int>> results, int count)
    {
        // 根据 Pos 属性把英雄填到中间9格(HeroCells)，其他位置留 null
        // 前排近战(pos123)从 HeroCells 顶部往下填；后排远程(pos456)从 HeroCells 底部往上填
        List<Tuple<int, int>> newResult = new List<Tuple<int, int>>(count);
        for (int i = 0; i < count; i++)
            newResult.Add(null);
        var heroCells = CombatConst.HeroCells;
        List<Tuple<int, int>> pos123 = new List<Tuple<int, int>>();
        List<Tuple<int, int>> pos456 = new List<Tuple<int, int>>();

        // 根据近战/远程分类卡牌（射程<=20为近战，>20为远程）
        foreach (var item in results)
        {
            var heroCfg = HeroConfig.GetConfig(item.Item1);
            if (HeroSelectionTool.IsRangedHero(heroCfg))
                pos456.Add(item);
            else
                pos123.Add(item);
        }

        // 前排近战英雄从 HeroCells 顶部往下填
        int idx = 0;
        while (idx < heroCells.Length && pos123.Count > 0)
        {
            newResult[heroCells[idx]] = pos123[0];
            pos123.RemoveAt(0);
            idx++;
        }

        // 后排远程英雄从 HeroCells 底部往上填
        idx = heroCells.Length - 1;
        while (idx >= 0 && pos456.Count > 0)
        {
            if (newResult[heroCells[idx]] == null)
            {
                newResult[heroCells[idx]] = pos456[0];
                pos456.RemoveAt(0);
            }
            idx--;
        }

        // 处理剩余卡牌，填到 HeroCells 空位
        List<Tuple<int, int>> remainingCards = new List<Tuple<int, int>>();
        remainingCards.AddRange(pos123);
        remainingCards.AddRange(pos456);

        for(int i = 0; i < heroCells.Length; i++)
        {
            if(newResult[heroCells[i]] == null && remainingCards.Count > 0)
            {
                newResult[heroCells[i]] = remainingCards[0];
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
        lastBattleLose = !isWin;
        mark += add;
        resultText.text = mark.ToString();
        // 玩家等级体系：战斗获胜/失败获得经验（参考金铲铲，节奏放慢一倍：胜利+2，失败+1）
        AddExp(isWin ? CombatConst.BattleWinExp : CombatConst.BattleLoseExp);
    }

    // ---- 玩家等级体系 ----

    // 当前等级可用的上阵格子数（10级后9个格子全解锁）
    public int GetSlotCount()
    {
        var lv = Mathf.Clamp(level, 1, CombatConst.PlayerMaxLevel);
        if (PlayerLevelConfig.HasConfig(lv))
            return Mathf.Min(PlayerLevelConfig.GetConfig(lv).SlotCount, CombatConst.PlayerMaxSlot);
        return Mathf.Min(lv, CombatConst.PlayerMaxSlot);
    }

    // 升到下一级所需经验（满级返回0）
    public int GetExpToNext()
    {
        if (level >= CombatConst.PlayerMaxLevel)
            return 0;
        if (PlayerLevelConfig.HasConfig(level))
            return PlayerLevelConfig.GetConfig(level).ExpToNext;
        return 0;
    }

    // 增加经验并处理升级（经验溢出顺延到下一级）
    public void AddExp(int add)
    {
        if (add <= 0 || level >= CombatConst.PlayerMaxLevel)
            return;
        exp += add;
        var lv = level;
        while (lv < CombatConst.PlayerMaxLevel)
        {
            if (!PlayerLevelConfig.HasConfig(lv))
                break;
            var need = PlayerLevelConfig.GetConfig(lv).ExpToNext;
            if (exp < need)
                break;
            exp -= need;
            lv++;
        }
        if (lv != level)
        {
            level = lv;
            EnsureSoldierCells(); // 玩家等级提升可能解锁更多士兵，补齐布阵格
            GameLog.Debug($"玩家{pid} 升级到 {level} 级，上阵格子 {GetSlotCount()}，士兵 {GetSoldierMeleeCount()}步+{GetSoldierRangedCount()}弓，剩余经验 {exp}");
        }
    }

    // 用金币购买经验（参考金铲铲：4金币=4经验；UI后续接入）
    public bool BuyExp()
    {
        var cost = CombatConst.ExpBuyGoldCost;
        var amount = CombatConst.ExpBuyAmount;
        if (gold < cost)
            return false;
        SubGold(cost, false);
        AddExp(amount);
        return true;
    }

    // ---- 士兵等级体系 ----
    // 士兵等级攻防加成（近战全量，远程按 SoldierAtkRate/SoldierHpRate 折算）
    public int GetSoldierAtkAdd()
    {
        return SoldierLevelConfig.HasConfig(soldierLevel) ? SoldierLevelConfig.GetConfig(soldierLevel).AtkAdd : 0;
    }
    public int GetSoldierHpAdd()
    {
        return SoldierLevelConfig.HasConfig(soldierLevel) ? SoldierLevelConfig.GetConfig(soldierLevel).HpAdd : 0;
    }
    public int GetSoldierMeleeCount()
    {
        var lv = Mathf.Clamp(level, 1, CombatConst.PlayerMaxLevel);
        return PlayerLevelConfig.HasConfig(lv) ? PlayerLevelConfig.GetConfig(lv).MeleeCount : 0;
    }
    public int GetSoldierRangedCount()
    {
        var lv = Mathf.Clamp(level, 1, CombatConst.PlayerMaxLevel);
        return PlayerLevelConfig.HasConfig(lv) ? PlayerLevelConfig.GetConfig(lv).RangedCount : 0;
    }

    // 花金币提升士兵等级（最高30级），只提升士兵攻防加成（数量由玩家等级决定）
    public bool SodLvup()
    {
        if (soldierLevel >= CombatConst.SoldierMaxLevel)
            return false;
        if (gold < CombatConst.SodLvupGoldCost)
            return false;
        SubGold(CombatConst.SodLvupGoldCost, false);
        soldierLevel++;
        return true;
    }

    public bool HasCard(int cardId)
    {
        // 英雄查 cards（exp），物品查 items（实例列表）
        return ConfigManager.IsHeroCard(cardId)
            ? cards.ContainsKey(cardId)
            : GetItemCount(cardId) > 0;
    }

    // 直接获得一张道具卡（PVE怪物掉落等来源，不走商店购买流程）
    public void AddItemCard(int itemId)
    {
        items.Add(new SerializableItemSlot(itemId, 0));
        // 进背包自动合成：按 ItemConfig 配置检查该道具是否达到合成需求
        CheckAutoCombine(itemId);
    }

    // 背包自动合成：仅检查指定道具是否达到配置的合成需求，达到则消耗并合成目标道具
    private void CheckAutoCombine(int itemId)
    {
        var cfg = ItemConfig.GetConfig(itemId);
        if (cfg == null || cfg.CombineId <= 0 || cfg.CombineNeed <= 0)
            return;
        int srcCount = GetItemCount(itemId);
        if (srcCount < cfg.CombineNeed)
            return;
        int combineCount = srcCount / cfg.CombineNeed;
        int leftCount = srcCount - combineCount * cfg.CombineNeed;
        // 消耗合成所需数量（优先背包副本），产出目标道具进背包
        RemoveItemInstances(itemId, srcCount - leftCount);
        for (int i = 0; i < combineCount; i++)
            items.Add(new SerializableItemSlot(cfg.CombineId, 0));
        GameLog.Debug(string.Format("背包自动合成：玩家{0} 消耗{1}个道具{2}合成{3}个道具{4}",
            pid, combineCount * cfg.CombineNeed, itemId, combineCount, cfg.CombineId));
    }

    // 两件物品合成一件（ItemCombineConfig 合成表）：消耗 ItemA/ItemB 各配置数量，产出 ResultId 配置数量进背包
    // 返回是否成功（材料不足或未找到配方则失败）
    public bool CombineTwoItems(int itemA, int itemB)
    {
        // 找到匹配配方（A+B 或 B+A 均可）
        ItemCombineConfig rcp = null;
        foreach (var cfg in ItemCombineConfig.ConfigList)
        {
            if ((cfg.ItemA == itemA && cfg.ItemB == itemB) || (cfg.ItemA == itemB && cfg.ItemB == itemA))
            {
                rcp = cfg;
                break;
            }
        }
        if (rcp == null)
        {
            GameLog.Warn(string.Format("合成失败：未找到配方 {0}+{1}，玩家{2}", itemA, itemB, pid));
            return false;
        }
        // 材料数量校验：两件都需持有足够数量
        if (GetItemCount(rcp.ItemA) < rcp.ItemAcount ||
            GetItemCount(rcp.ItemB) < rcp.ItemBcount)
        {
            GameLog.Warn(string.Format("合成失败：材料不足 需要{0}x{1} + {2}x{3}，玩家{4}",
                rcp.ItemA, rcp.ItemAcount, rcp.ItemB, rcp.ItemBcount, pid));
            return false;
        }
        // 消耗材料
        ConsumeItemCount(rcp.ItemA, rcp.ItemAcount);
        ConsumeItemCount(rcp.ItemB, rcp.ItemBcount);
        // 产出结果
        AddItemCountDirect(rcp.ResultId, rcp.ResultCount);
        GameLog.Debug(string.Format("背包合成：玩家{0} 消耗{1}x{2}+{3}x{4} 合成{5}x{6}",
            pid, rcp.ItemA, rcp.ItemAcount, rcp.ItemB, rcp.ItemBcount, rcp.ResultId, rcp.ResultCount));
        return true;
    }

    // 消耗指定数量的物品实例（优先背包副本，不足再移除已装备的），不触发自动合成
    private void ConsumeItemCount(int itemId, int count)
    {
        if (count <= 0)
            return;
        RemoveItemInstances(itemId, count);
    }

    // 直接增加指定数量物品到背包（不触发自动合成检查）
    private void AddItemCountDirect(int itemId, int count)
    {
        for (int i = 0; i < count; i++)
            items.Add(new SerializableItemSlot(itemId, 0));
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
                        else if (fieldValue is Dictionary<int, int[]>)
                        {
                            Dictionary<int, int[]> dict = (Dictionary<int, int[]>)fieldValue;
                            List<string> dictEntries = new List<string>();
                            foreach (var kvp in dict)
                            {
                                // 槽位用|分隔：heroId:v0|v1|v2
                                dictEntries.Add(kvp.Key + ":" + string.Join("|", kvp.Value ?? new int[0]));
                            }
                            stringValue = string.Join(",", dictEntries);
                        }
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
                        // 物品实例列表：用 wrapper 包一层便于 JsonUtility 顶层序列化
                        else if (fieldValue is List<SerializableItemSlot>)
                        {
                            var wrapper = new ItemSlotListWrapper();
                            wrapper.list = (List<SerializableItemSlot>)fieldValue;
                            stringValue = JsonUtility.ToJson(wrapper);
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
            GameLog.Error("序列化PlayerInfo失败: " + e.Message);
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
                    else if (field.FieldType == typeof(Dictionary<int,int[]>))
                    {
                        var dict = new Dictionary<int, int[]>();
                        string[] entries = stringValue.Split(',');
                        foreach (string entry in entries)
                        {
                            if (string.IsNullOrEmpty(entry))
                                continue;
                            int colonIndex = entry.IndexOf(':');
                            if (colonIndex <= 0 || !int.TryParse(entry.Substring(0, colonIndex), out int key))
                                continue;

                            string valStr = entry.Substring(colonIndex + 1);
                            int[] slots = new int[3];
                            if (valStr.Contains("|"))
                            {
                                // 新格式：v0|v1|v2
                                string[] parts = valStr.Split('|');
                                for (int i = 0; i < parts.Length && i < slots.Length; i++)
                                    int.TryParse(parts[i], out slots[i]);
                            }
                            else
                            {
                                // 兼容旧存档：单装备放在第1个槽位
                                if (int.TryParse(valStr, out int oldItemId))
                                    slots[0] = oldItemId;
                            }
                            dict[key] = slots;
                        }
                        field.SetValue(this, dict);
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
                    // 物品实例列表：新格式 wrapper JSON；兼容旧存档 dict 格式 "id:count,id:count"
                    else if (field.FieldType == typeof(List<SerializableItemSlot>))
                    {
                        var list = new List<SerializableItemSlot>();
                        if (stringValue.StartsWith("{\"list\""))
                        {
                            var wrapper = JsonUtility.FromJson<ItemSlotListWrapper>(stringValue);
                            if (wrapper != null && wrapper.list != null)
                                list = wrapper.list;
                        }
                        else
                        {
                            string[] entries = stringValue.Split(',');
                            foreach (string entry in entries)
                            {
                                if (string.IsNullOrEmpty(entry))
                                    continue;
                                string[] parts = entry.Split(':');
                                if (parts.Length == 2 && int.TryParse(parts[0], out int id) && int.TryParse(parts[1], out int cnt))
                                    for (int i = 0; i < cnt; i++)
                                        list.Add(new SerializableItemSlot(id, 0));
                            }
                        }
                        field.SetValue(this, list);
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
            GameLog.Error("反序列化PlayerInfo失败: " + e.Message);
        }
        // 兼容旧存档：battleCards 统一为9格长度
        EnsureBattleCardsSize();
        // 兼容旧存档：旧存档中物品混在 cards，这里把非英雄条目迁移到 items
        MigrateLegacyItemsToItems();
        // 兼容旧存档：剔除 items 中配置表里已不存在的道具（401002士兵剑/401003士兵甲已移除），避免UI空引用
        RemoveObsoleteItemCards();
    }

    // 兼容旧存档：把 cards 中混入的物品条目迁移到独立的 items 容器（物品无 exp，纯数量）
    private void MigrateLegacyItemsToItems()
    {
        List<int> itemCardIds = null;
        foreach (var kv in cards)
        {
            if (ConfigManager.IsHeroCard(kv.Key))
                continue;
            if (itemCardIds == null)
                itemCardIds = new List<int>();
            itemCardIds.Add(kv.Key);
        }
        if (itemCardIds == null)
            return;
        foreach (var id in itemCardIds)
        {
            int count = cards[id];
            cards.Remove(id);
            for (int i = 0; i < count; i++)
                items.Add(new SerializableItemSlot(id, 0));
            GameLog.Debug($"旧存档迁移：道具 {id} 从 cards 迁移到 items，数量 {count}");
        }
    }

    // 剔除 items 中配置表里已不存在的道具（保留有效道具；英雄卡只位于 cards）
    private void RemoveObsoleteItemCards()
    {
        List<SerializableItemSlot> invalid = null;
        foreach (var slot in items)
        {
            if (ItemConfig.HasConfig(slot.ItemId))
                continue;
            if (invalid == null)
                invalid = new List<SerializableItemSlot>();
            invalid.Add(slot);
        }
        if (invalid != null)
        {
            foreach (var slot in invalid)
            {
                items.Remove(slot);
                GameLog.Debug($"旧存档清理：移除已下架道具 {slot.ItemId}");
            }
        }
    }

    // 兼容旧存档：battleCards 固定为9格长度
    private void EnsureBattleCardsSize()
    {
        if (battleCards == null)
        {
            battleCards = new int[CombatConst.PlayerMaxSlot];
            return;
        }
        if (battleCards.Length != CombatConst.PlayerMaxSlot)
        {
            var cards = new int[CombatConst.PlayerMaxSlot];
            for (int i = 0; i < Math.Min(battleCards.Length, CombatConst.PlayerMaxSlot); i++)
                cards[i] = battleCards[i];
            battleCards = cards;
        }
        // 补齐默认小兵（近战前3格、远程后2格）
        EnsureSoldierCells();
    }

    // 确保布阵格包含足够的小兵：battleCards 记录格子上的单位id（英雄 或 小兵500001/500002）
    // 小兵数量由玩家等级(PlayerLevelConfig)决定，不足时补到默认小兵格（旧存档/新玩家），英雄占据默认格时先移到空位
    public void EnsureSoldierCells()
    {
        if (battleCards == null)
        {
            battleCards = new int[CombatConst.PlayerMaxSlot];
        }
        else if (battleCards.Length != CombatConst.PlayerMaxSlot)
        {
            // 兼容旧长度(场景序列化的旧存档可能为9格)
            var cards = new int[CombatConst.PlayerMaxSlot];
            for (int i = 0; i < Math.Min(battleCards.Length, CombatConst.PlayerMaxSlot); i++)
                cards[i] = battleCards[i];
            battleCards = cards;
        }

        var cfg = PlayerLevelConfig.GetConfig(Mathf.Clamp(level, 1, CombatConst.PlayerMaxLevel));
        int meleeCount = 0;
        int rangedCount = 0;
        for (int i = 0; i < battleCards.Length; i++)
        {
            if (battleCards[i] == 500001)
                meleeCount++;
            else if (battleCards[i] == 500002)
                rangedCount++;
        }
        if (meleeCount >= cfg.MeleeCount && rangedCount >= cfg.RangedCount)
            return;

        for (int i = 0; i < CombatConst.SoldierMeleeCells.Length && meleeCount < cfg.MeleeCount; i++)
        {
            int pos = CombatConst.SoldierMeleeCells[i];
            if (battleCards[pos] == 500001 || battleCards[pos] == 500002)
                continue;
            if (battleCards[pos] > 0)
                MoveHeroToEmpty(pos);
            battleCards[pos] = 500001;
            meleeCount++;
        }
        for (int i = 0; i < CombatConst.SoldierRangedCells.Length && rangedCount < cfg.RangedCount; i++)
        {
            int pos = CombatConst.SoldierRangedCells[i];
            if (battleCards[pos] == 500001 || battleCards[pos] == 500002)
                continue;
            if (battleCards[pos] > 0)
                MoveHeroToEmpty(pos);
            battleCards[pos] = 500002;
            rangedCount++;
        }
    }

    // 将格子上英雄移到第一个空位（用于小兵占用默认格时）
    private void MoveHeroToEmpty(int fromPos)
    {
        int heroId = battleCards[fromPos];
        if (heroId == 0)
            return;
        battleCards[fromPos] = 0;
        for (int i = 0; i < battleCards.Length; i++)
        {
            if (battleCards[i] == 0)
            {
                battleCards[i] = heroId;
                break;
            }
        }
    }
    
    // 每个英雄最多可装备的物品数（原 3 槽位）
    private const int MaxEquipSlots = 3;

    // AI 装备分配：第一梯队取培养度最高的英雄数（超出后从第4个开始继续比对）
    private const int EquipTopHeroCount = 3;

    // AI 装备分配：参与比对的职业偏好属性位数（JobConfig.EquipAttr 前 N 位）
    private const int EquipAttrSlotCount = 3;

    // 物品实例数据结构：每件物品一条，ItemId + HeroId(HeroId=0 表示在背包未装备)
    [System.Serializable]
    public class SerializableItemSlot
    {
        public int ItemId;
        public int HeroId;

        public SerializableItemSlot() { }

        public SerializableItemSlot(int itemId, int heroId)
        {
            ItemId = itemId;
            HeroId = heroId;
        }
    }

    // 物品实例列表的序列化包装（JsonUtility 不支持 List<T> 顶层序列化）
    [System.Serializable]
    private class ItemSlotListWrapper
    {
        public List<SerializableItemSlot> list = new List<SerializableItemSlot>();
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

