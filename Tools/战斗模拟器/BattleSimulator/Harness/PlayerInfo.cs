// ============================================================
// 战斗模拟器 · Harness —— PlayerInfo 无头替身
// 精简版玩家数据：卡片/道具/布阵/战斗侧，战斗代码所需的接口全部保留。
// ============================================================
using System;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

public class PlayerInfo
{
    public int pid;
    public int playerId;      // PlayerConfig.Id
    public PlayerConfig playerConfig;
    public bool isAI;

    public Dictionary<int, int> cards = new Dictionary<int, int>();   // 英雄卡 id → exp
    public List<SerializableItemSlot> items = new List<SerializableItemSlot>();
    public int[] battleCards = new int[CombatConst.PlayerMaxSlot];    // 25 格布阵
    public Dictionary<int, AttrInfo> attrAddons = new Dictionary<int, AttrInfo>();

    public int battleSide;
    public Color lineColor = Color.blue;
    public int banCount = 1;

    public int sodatk = 0;      // 士兵攻击强化
    public int sodhp = 0;       // 士兵生命强化
    public int soldierLevel = 1;
    public int level = 1;
    public int exp = 0;
    public int mark = 0;
    public int gold = 0;
    public bool lastBattleLose;
    public CastleHUD castleHUD;

    public string imgPath
    {
        get { return "Textures/" + (playerConfig != null ? playerConfig.Imgpath : "Hero"); }
    }

    public string playerName
    {
        get { return playerConfig != null ? playerConfig.Name : "P" + pid; }
    }

    public void Init(int id, int pid1)
    {
        pid = id;
        playerId = pid1;
        isAI = id > 0;
        if (playerConfig == null)
            playerConfig = PlayerConfig.GetConfig(pid1);
    }

    // 布阵英雄列表：按 battleCards 返回长度 25 的列表（索引=格子，null=空位）
    public List<Tuple<int, int>> GetBattleCardList(bool isTest = false)
    {
        var cardList = new List<Tuple<int, int>>(new Tuple<int, int>[CombatConst.FormationCellCount]);
        for (int i = 0; i < battleCards.Length && i < CombatConst.FormationCellCount; i++)
        {
            int heroId = battleCards[i];
            if (heroId <= 0 || !ConfigManager.IsHeroCard(heroId))
                continue;
            int exp = cards.ContainsKey(heroId) ? cards[heroId] : 1;
            cardList[i] = new Tuple<int, int>(heroId, HeroSelectionTool.GetCardLevel(exp, true));
        }
        return cardList;
    }

    public List<int> GetItemIdsOnHero(int heroId)
    {
        var ids = new List<int>();
        foreach (var slot in items)
        {
            if (slot.HeroId == heroId)
                ids.Add(slot.ItemId);
        }
        return ids;
    }

    // 装备/道具的属性总值（effect="pattr" 的道具）
    public int GetItemPAttr(string attrName)
    {
        int attrVal = 0;
        foreach (var slot in items)
        {
            var itemCfg = ItemConfig.GetConfig(slot.ItemId);
            if (itemCfg == null || itemCfg.Effect != "pattr")
                continue;
            foreach (var bonus in JobLinkManager.ParseBonuses(itemCfg.Attrs))
            {
                if (bonus.Attr == attrName)
                    attrVal += (int)bonus.Value;
            }
        }
        return attrVal;
    }

    public int GetSoldierAtkAdd() { return sodatk; }
    public int GetSoldierHpAdd() { return sodhp; }

    // 战斗结算：记录胜负即可
    public void onBattleResult(bool isWin, int add)
    {
        lastBattleLose = !isWin;
        if (isWin) winCount++;
        else loseCount++;
    }

    public int winCount;
    public int loseCount;

    public void OnBattleBegin() { }

    public void AddItemCard(int itemId)
    {
        items.Add(new SerializableItemSlot { ItemId = itemId, HeroId = 0 });
    }

    public int GetSlotCount() { return 5; }

    public bool HasCard(int cardId) { return cards.ContainsKey(cardId); }

    public void AddAttrAddon(int cardId, AttrInfo attr)
    {
        if (attrAddons.ContainsKey(cardId))
            attrAddons[cardId].AddAttr(attr);
        else
            attrAddons[cardId] = attr;
    }
}

[System.Serializable]
public class SerializableItemSlot
{
    public int ItemId;
    public int HeroId; // 0 = 在背包未装备，非 0 = 装备在该英雄上
}
