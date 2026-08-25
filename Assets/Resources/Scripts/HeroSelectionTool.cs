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
            int randomIndex = UnityEngine.Random.Range(0, tempIds.Count);
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

            // 按三攻总和排序
            return (configB.Atk + configB.Ap + configB.Might).CompareTo(configA.Atk + configA.Ap + configA.Might);
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

    public static int GetRandomHeroId()
    {
        // 实现价格加权随机：价格越高，被选中的概率越低
        // 使用价格的倒数作为权重
        float totalWeight = 0;
        foreach (var hero in heroPoolCache)
        {
            // 防止价格为0的情况
            float weight = hero.Item2 > 0 ? hero.Item2 : 1f;
            totalWeight += weight;
        }
        
        float randomValue = UnityEngine.Random.Range(0, totalWeight);
        float accumulatedWeight = 0;
        
        foreach (var hero in heroPoolCache)
        {
            float weight = hero.Item2 > 0 ? hero.Item2 : 1f;
            accumulatedWeight += weight;
            
            if (accumulatedWeight >= randomValue)
            {
                return hero.Item1;
            }
        }
        
        // 如果出现问题，返回第一个英雄（保底）
        return heroPoolCache[0].Item1;
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
        int randomIndex = UnityEngine.Random.Range(0, itemList.Count);
        return itemList[randomIndex].Id;
    }

    public static int GetPrice(HeroConfig heroCfg)
    {
        return heroCfg.Price;
    }

    // 属性字符串 → 图标贴图名（沿用旧贴图资源：atk→统帅图标、ap→智力图标、might→武力图标、hp→护盾图标）
    public static string GetAttrIcon(string attr)
    {
        switch (attr)
        {
            case "atk":
                return "attrlead";
            case "ap":
                return "attrinte";
            case "might":
                return "attrstr";
            case "hp":
                return "attrshield";
            default:
                return attr;
        }
    }

    // 升星成本：1→2星需3张，2→3星需5张，3→4星需7张……（每级新增2N-1张，累计n²张）
    private static int[] cardHeroExp = new int[] { 1, 4, 9, 16, 25, 36, 49, 64, 81, 100, 121, 144, 169, 196, 225, 256, 289, 324, 361, 400, 441, 484, 529, 576, 625, 676, 729, 784, 841, 900, 961, 1024, 1089, 999 };
    private static int[] cardItemExp = new int[] { 1, 2, 4, 6, 9, 12, 15, 19, 23, 27, 31, 36, 41, 46, 51, 56, 62, 68, 74, 80, 86, 92, 98, 104, 110, 116, 122, 128, 136, 142, 148, 154, 160, 166, 172, 178, 184, 190, 196, 202, 999 }; //生成后续数据
    public static int GetCardLevel(int exp, bool isHero)
    {
        if(isHero)
        {
            for(int i = 0; i < cardHeroExp.Length; i++)
            {
                if(exp < cardHeroExp[i])
                    return i;
            }
            return cardHeroExp.Length;
        }
        else
        {
            for(int i = 0; i < cardItemExp.Length; i++)
            {
                if(exp < cardItemExp[i])
                    return i;
            }
            return cardItemExp.Length;
        }
    }

    public static float GetExpRate(int exp, bool isHero)
    {
        int level = GetCardLevel(exp, isHero);
        if(level >= cardHeroExp.Length)
            return 1f;
        if(level <= 1)
            return 0;
        if(isHero)
            return (float)(exp - cardHeroExp[level - 1]) / (cardHeroExp[level] - cardHeroExp[level - 1]);
        else
            return (float)(exp - cardItemExp[level - 1]) / (cardItemExp[level] - cardItemExp[level - 1]);
    }

    public static AttrInfo GetCardAttr(PlayerInfo player, int cardId, int lv)
    {
        var attrInfo = new AttrInfo();
        if (ConfigManager.IsHeroCard(cardId))
        {
            var heroConfig = HeroConfig.GetConfig(cardId);

            // 品质系数：品质每高一档基础属性×1.15（普通1.0 优秀1.15 精良1.32 史诗1.52），参照金铲铲费用梯度
            float qualityFactor = Mathf.Pow(1.15f, heroConfig.Quality - 1);

            // 线性成长：每星按配置的成长百分比提升（AtkP/ApP/MightP，默认80=每星+80%，2星≈1.8倍1星）
            attrInfo.Hp = (int)(heroConfig.Hp * qualityFactor * (100 + heroConfig.HpP * (lv - 1)) / 100); // Hp 独立成长字段（每星+80%）
            attrInfo.Ap = (int)(heroConfig.Ap * qualityFactor * (100 + heroConfig.ApP * (lv - 1)) / 100);
            attrInfo.Might = (int)(heroConfig.Might * qualityFactor * (100 + heroConfig.MightP * (lv - 1)) / 100);
            attrInfo.Atk = (int)(heroConfig.Atk * qualityFactor * (100 + heroConfig.AtkP * (lv - 1)) / 100);
        }
        else
        {
            var itemConfig = ItemConfig.GetConfig(cardId);
            if (itemConfig.Attr1 == "might")
            {
                attrInfo.Might = itemConfig.Attr1Val;
            }
            else if (itemConfig.Attr1 == "ap")
            {
                attrInfo.Ap = itemConfig.Attr1Val;
            }
            else if (itemConfig.Attr1 == "atk")
            {
                attrInfo.Atk = itemConfig.Attr1Val;
            }
            else if (itemConfig.Attr1 == "hp")
            {
                attrInfo.Hp = itemConfig.Attr1Val;
            }

            if (itemConfig.Attr2 == "might")
            {
                attrInfo.Might = itemConfig.Attr2Val;
            }
            else if (itemConfig.Attr2 == "ap")
            {
                attrInfo.Ap = itemConfig.Attr2Val;
            }
            else if (itemConfig.Attr2 == "atk")
            {
                attrInfo.Atk = itemConfig.Attr2Val;
            }
            else if (itemConfig.Attr2 == "hp")
            {
                attrInfo.Hp = itemConfig.Attr2Val;
            }

            attrInfo.Hp = attrInfo.Hp * lv;
            attrInfo.Ap = attrInfo.Ap * lv;
            attrInfo.Might = attrInfo.Might * lv;
            attrInfo.Atk = attrInfo.Atk * lv;
        }
        if(player.attrAddons.ContainsKey(cardId))
            attrInfo.AddAttr(player.attrAddons[cardId]);

        return attrInfo;

    }

    public static Color GetQualityColor(int quality)
    {
        if (quality == 1)
            return new Color(255 / 255f, 255 / 255f, 255 / 255f); // 普通-白
        else if (quality == 2)
            return new Color(30 / 255f, 255 / 255f, 0 / 255f); // 优秀-绿
        else if (quality == 3)
            return new Color(0 / 255f, 112 / 255f, 221 / 255f); // 精良-蓝
        else
            return new Color(163 / 255f, 53 / 255f, 238 / 255f); // 史诗-紫
    }

    public static Color GetSideColor(int side)
    {
        if (side == 1)
            return new Color(40 / 255f, 70 / 255f, 0 / 255f, 255 / 255f);
        else if (side == 2)
            return new Color(0 / 255f, 35 / 255f, 100 / 255f, 255 / 255f);
        else if (side == 3)
            return new Color(100 / 255f, 0 / 255f, 0 / 255f, 255 / 255f);
        else if (side == 4)
            return new Color(30 / 255f, 100 / 255f, 110 / 255f, 255 / 255f);
        else if (side == 5)
            return new Color(90 / 255f, 50 / 255f, 110 / 255f, 255 / 255f);
        else if (side == 6)
            return new Color(120 / 255f, 90 / 255f, 30 / 255f, 255 / 255f);                                    
        else
            return new Color(50 / 255f, 50 / 255f, 50 / 255f, 255 / 255f);
    }

    public static AttrInfo GetSupportAttr(int heroId, int pid, int lv)
    {
        var friendLevel = ConfigManager.GetFriendLevel(heroId, pid);
        if(friendLevel <= 0)
            return null;
        
        var friendHeroCfg = HeroConfig.GetConfig(pid);
        var myHeroCfg = HeroConfig.GetConfig(heroId);
        
        // 获取三个属性值
        int friendMight = friendHeroCfg.Might;
        int friendAp = friendHeroCfg.Ap;
        int friendAtk = friendHeroCfg.Atk;
        
        int myMight = myHeroCfg.Might;
        int myAp = myHeroCfg.Ap;
        int myAtk = myHeroCfg.Atk;
        
        // 计算差值
        int mightDiff = friendMight - myMight;
        int apDiff = friendAp - myAp;
        int atkDiff = friendAtk - myAtk;
           
        int totalPoints;
        float[] weights = new float[3];

        weights[0] = FormulaLearnAttrConfig.GetConfig(mightDiff).Weight;
        weights[1] = FormulaLearnAttrConfig.GetConfig(apDiff).Weight;
        weights[2] = FormulaLearnAttrConfig.GetConfig(atkDiff).Weight;
        mightDiff = Math.Clamp(mightDiff, -6, 50);
        apDiff = Math.Clamp(apDiff, -6, 50);
        atkDiff = Math.Clamp(atkDiff, -6, 50);
        var factor = 1f; //单属性老师，给1.5倍属性
        if ( mightDiff > 0 && apDiff < 0 && atkDiff < 0 || mightDiff < 0 && apDiff > 0 && atkDiff < 0 || mightDiff < 0 && apDiff < 0 && atkDiff > 0)
            factor = 1.5f;
        totalPoints = (int)Math.Clamp((mightDiff + apDiff + atkDiff) * factor / 2.2f, 10, 30);
        if(friendLevel > 1)
            totalPoints = (int)Math.Clamp(totalPoints * (1 + 0.2f * (friendLevel - 1)), 10, 30);

        // 计算总权重
        float totalWeight = weights[0] + weights[1] + weights[2];
        if (totalWeight <= 0)
            return new AttrInfo();
        
        // 计算属性点分配，避免四舍五入导致总和不等的问题
        float[] diffs = { weights[0], weights[1], weights[2] };
        int[] addValues = new int[3];
        
        // 找出三个差值中的排序索引（从小到大）
        int[] indices = { 0, 1, 2 }; // 0=Might, 1=Ap, 2=Atk
        Array.Sort(indices, (a, b) => diffs[a].CompareTo(diffs[b]));
        
        // 先计算最低差值的属性
        addValues[indices[0]] = Mathf.FloorToInt(totalPoints * weights[indices[0]] / totalWeight);
        addValues[indices[1]] = Mathf.FloorToInt(totalPoints * weights[indices[1]] / totalWeight);
        addValues[indices[2]] = totalPoints - addValues[indices[0]] - addValues[indices[1]];

        var attr = new AttrInfo();
        attr.Might = addValues[0] * (lv + 9) / 10;
        attr.Ap = addValues[1] * (lv + 9) / 10;
        attr.Atk = addValues[2] * (lv + 9) / 10;
        
        return attr;
    }    
}
