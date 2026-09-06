using System;
using System.Collections.Generic;
using System.Linq;
using CommonConfig;
using UnityEngine;

// 定义一个单独的工具类
public static class HeroSelectionTool
{
    private static List<Tuple<int, int>> heroPoolCache = new List<Tuple<int, int>>();

    // 获取指定阵营的所有英雄ID
    public static List<int> GetAllHeroIdsBySide(int side)
    {
        List<int> heroIds = new List<int>();
        // 假设HeroConfig有一个方法GetAllConfigs()返回所有英雄配置
        foreach (var config in HeroConfig.ConfigList)
        {
            if (config.Side == side)
            {
                heroIds.Add((int)config.Id);
            }
        }
        return heroIds;
    }

    // 从源ID列表中随机选择指定数量的不重复ID
    public static List<int> GetRandomUniqueIds(List<int> sourceIds, int count)
    {
        List<int> result = new List<int>();
        if (sourceIds == null || sourceIds.Count == 0 || count <= 0)
        {
            return result;
        }

        // 创建源列表的副本以避免修改原列表
        List<int> tempIds = new List<int>(sourceIds);
        int actualCount = Mathf.Min(count, tempIds.Count);

        for (int i = 0; i < actualCount; i++)
        {
            int randomIndex = SysRandom.Range(0, tempIds.Count);
            result.Add(tempIds[randomIndex]);
            tempIds.RemoveAt(randomIndex);
        }

        return result;
    }


    public static void UpdateHeroPoolCache(List<int> heroIds)
    {
        heroPoolCache.Clear();
        foreach (var heroId in heroIds)
        {
            var config = HeroConfig.GetConfig(heroId);
            var rate = 1000 / Math.Max(5, GetPrice(config));
            if (config.Job == "shuai")
                rate += 15;
            heroPoolCache.Add(new Tuple<int, int>(heroId, rate));
        }

        heroPoolCache.Sort((a, b) =>
        {
            var configA = HeroConfig.GetConfig(a.Item1);
            var configB = HeroConfig.GetConfig(b.Item1);
            int sideCompare = configA.Side.CompareTo(configB.Side);
            if (sideCompare != 0)
            {
                return sideCompare;
            }

            // 检查ID是否在100100以下
            bool isBelow100100A = a.Item1 < 100100;
            bool isBelow100100B = b.Item1 < 100100;
            if (isBelow100100A != isBelow100100B)
            {
                return isBelow100100A ? -1 : 1;
            }

            // 按攻+法总面板排序（无双强度已在 PostModify 并入 Atk，Atk/Ap 为 1星带品质面板）
            return (configB.Atk + configB.Ap).CompareTo(configA.Atk + configA.Ap);
        });

    }

    public static List<int> GetHeroPoolCache()
    {
        // 返回只包含heroId的列表
        List<int> result = new List<int>();
        foreach (var hero in heroPoolCache)
        {
            result.Add(hero.Item1);
        }
        return result;
    }

    public static void SetBanList(List<int> banList)
    {
        heroPoolCache.RemoveAll(hero => banList.Contains(hero.Item1));
    }

    // 刷牌：先按GameRoundConfig品质概率roll出品质，再从该品质的英雄池随机选一张（ban 已在池中剔除）
    public static int GetRandomHeroIdByQuality(GameRoundConfig shopCfg)
    {
        int quality = RollQuality(shopCfg);
        List<int> candidates = new List<int>();
        foreach (var hero in heroPoolCache)
        {
            if (HeroConfig.GetConfig(hero.Item1).Quality == quality)
                candidates.Add(hero.Item1);
        }

        // 该品质池为空（如早期品质4未解锁或全部被ban）时，回退到整个池随机
        if (candidates.Count == 0)
            candidates = GetHeroPoolCache();
        if (candidates.Count == 0)
            return 0;

        return candidates[SysRandom.Range(0, candidates.Count)];
    }

    // 品质1=100-品质2-品质3-品质4
    private static int RollQuality(GameRoundConfig shopCfg)
    {
        int q2 = Math.Max(0, shopCfg.Quality2Rate);
        int q3 = Math.Max(0, shopCfg.Quality3Rate);
        int q4 = Math.Max(0, shopCfg.Quality4Rate);
        int q1 = Math.Max(0, 100 - q2 - q3 - q4);

        int roll = SysRandom.Range(0, 100);
        if (roll < q1)
            return 1;
        if (roll < q1 + q2)
            return 2;
        if (roll < q1 + q2 + q3)
            return 3;
        return 4;
    }

    public static bool HasHeroInPool(int heroId)
    {
        return heroPoolCache.Exists(hero => hero.Item1 == heroId);
    }

    public static int CountFriendInPool(int heroId)
    {
        int count = 0;
        foreach (var hero in heroPoolCache)
        {
            if (ConfigManager.GetFriendLevel(heroId, hero.Item1) > 0)
                count++;
        }
        return count;
    }

    public static int GetRandomItemId(int shopIdx)
    {
        var itemList = ItemConfig.ConfigList.ToList();
        // 剔除所有RateAbs非0的item
        itemList.RemoveAll(item => item.RateAbs > 0);
        // 剔除所有ShopId非0的item
        itemList.RemoveAll(item => item.ShopIdx > shopIdx);
        int randomIndex = SysRandom.Range(0, itemList.Count);
        return itemList[randomIndex].Id;
    }

    public static int GetPrice(HeroConfig heroCfg)
    {
        return heroCfg.Price;
    }

    // 英雄近战/远程判定：HeroConfig.Range 经 ConfigManager.PostModify 写回为 职业基准×(1+修正%/100)
    // （近战职业 17，远程职业 35~70），按写回射程 > 20 判为远程，与战斗侧 JobLinkManager 的 attackRange>20 规则一致
    public static bool IsRangedHero(HeroConfig heroCfg)
    {
        return heroCfg != null && heroCfg.Range > 20;
    }

    public static bool IsMeleeHero(HeroConfig heroCfg)
    {
        return heroCfg != null && heroCfg.Range <= 20;
    }

    // 主属性面板（Atk/Ap/Hp 统一计算入口，无双强度已并入 Atk）：
    // HeroConfig 数值列经 ConfigManager.PostModify 写回为“1星带品质面板” = 职业基准×(1+修正%/100) × 品质系数1.15^(Q-1)，
    // 此处只按星级成长放大：(100 + XP×(lv-1))/100。
    // XP(AtkP/ApP/HpP) 为每星成长百分比：80=每星+80%(2星≈1.8倍1星)，100=每星翻倍(2星2倍)；品质系数对主属性统一
    public static AttrInfo GetHeroAttr(HeroConfig heroCfg, int lv)
    {
        var attrInfo = new AttrInfo();
        if (heroCfg == null)
        {
            GameLog.Error("HeroSelectionTool.GetHeroAttr: heroCfg 为 null，无法计算面板");
            return attrInfo;
        }
        int lvGrow = Mathf.Max(1, lv) - 1;
        attrInfo.Hp = GrowPanel(heroCfg.Hp, heroCfg.HpP, lvGrow);    // 生命独立成长字段
        attrInfo.Atk = GrowPanel(heroCfg.Atk, heroCfg.AtkP, lvGrow);
        attrInfo.Ap = GrowPanel(heroCfg.Ap, heroCfg.ApP, lvGrow);
        return attrInfo;
    }

    private static int GrowPanel(int panelValue, int growP, int lvGrow)
    {
        // 1星带品质面板 × (100 + 成长P×已升星数)/100
        return (int)Math.Round(panelValue * (100f + growP * lvGrow) / 100f);
    }

    // 1星带品质主属性面板：图鉴/排行/开局发卡/AI判断/卡池排序统一口径（= GetHeroAttr 的 lv=1，即 PostModify 写回值）
    public static AttrInfo GetRankAttr(HeroConfig heroCfg)
    {
        return GetHeroAttr(heroCfg, 1);
    }

    // 升星成本：1→2星需3张，2→3星需5张，3→4星需7张……（每级新增2N-1张，累计n²张）
    private static int[] cardHeroExp = new int[] { 1, 4, 9, 16, 25, 36, 49, 64, 81, 100, 121, 144, 169, 196, 225, 256, 289, 324, 361, 400, 441, 484, 529, 576, 625, 676, 729, 784, 841, 900, 961, 1024, 1089, 999 };
    // 装备升级机制已移除：背包可存同id装备多件，每件独立生效，等级恒为1
    public static int GetCardLevel(int exp, bool isHero)
    {
        if(!isHero)
            return 1; // 装备不升级，多件同id装备各自生效
        for(int i = 0; i < cardHeroExp.Length; i++)
        {
            if(exp < cardHeroExp[i])
                return i;
        }
        return cardHeroExp.Length;
    }

    public static float GetExpRate(int exp, bool isHero)
    {
        if(!isHero)
            return 0; // 装备无升级进度条
        int level = GetCardLevel(exp, isHero);
        if(level >= cardHeroExp.Length)
            return 1f;
        if(level == 0)
            return 0;
        return (float)(exp - cardHeroExp[level - 1]) / (cardHeroExp[level] - cardHeroExp[level - 1]);
    }

    public static AttrInfo GetCardAttr(PlayerInfo player, int cardId, int lv)
    {
        var attrInfo = new AttrInfo();
        if (ConfigManager.IsHeroCard(cardId))
        {
            // 四主属性统一入口：职业基准×(1+修正%/100) × 品质系数 × 星级成长
            attrInfo = GetHeroAttr(HeroConfig.GetConfig(cardId), lv);
        }
        else
        {
            var itemConfig = ItemConfig.GetConfig(cardId);
            ApplyItemAttr(attrInfo, itemConfig.Attr1, itemConfig.Attr1Val);
            ApplyItemAttr(attrInfo, itemConfig.Attr2, itemConfig.Attr2Val);
            // 装备升级机制已移除：属性不再乘等级，每件装备固定属性
        }
        if(player.attrAddons.ContainsKey(cardId))
            attrInfo.AddAttr(player.attrAddons[cardId]);

        return attrInfo;

    }

    // 道具属性键解析：四主属性(四维)外，支持护甲/魔抗/回蓝及金铲铲式基础组件的攻速/暴击
    // 比例属性（attackRate/critRate/dodgeRate）配置存百分数（10=+10%），这里 ÷100 转为运行时比例；其余直接按数值
    private static void ApplyItemAttr(AttrInfo attrInfo, string key, int value)
    {
        if (string.IsNullOrEmpty(key) || value == 0)
            return;
        switch (key)
        {
            case "might": // 无双已并入攻击：老数据(未同步源表的 might 键)兼容为加攻击
            case "atk":
                attrInfo.Atk = value;
                break;
            case "ap":
                attrInfo.Ap = value;
                break;
            case "hp":
                attrInfo.Hp = value;
                break;
            case "armor":
                attrInfo.Armor = value;
                break;
            case "magicRes":
                attrInfo.MagicRes = value;
                break;
            case "mpRegen":
                attrInfo.MpRegen = value;
                break;
            case "attackRate":
                attrInfo.AttackRate = value / 100f;
                break;
            case "critRate":
                attrInfo.CritRate = value / 100f;
                break;
        }
    }

    // 品质色/阵营色定义已迁至 SysColor.GetQualityColor / SysColor.GetSideColor
}
