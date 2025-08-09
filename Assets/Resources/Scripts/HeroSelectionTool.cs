using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

// 定义一个单独的工具类
public static class HeroSelectionTool
{
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
            int randomIndex = Random.Range(0, tempIds.Count);
            result.Add(tempIds[randomIndex]);
            tempIds.RemoveAt(randomIndex);
        }

        return result;
    }

    private static List<int> heroPoolCache = new List<int>();
    private static void UpdateHeroPoolCache()
    {
        heroPoolCache.Clear();
        List<HeroConfig> allHeroes = new List<HeroConfig>(HeroConfig.ConfigList);

        // 先对allHeroes遍历，1-100随机，如果大于RateAbs，加入返回队列
        List<HeroConfig> tempHeroes = new List<HeroConfig>(allHeroes);
        foreach (var hero in tempHeroes)
        {
            if(hero.RateAbs <= 0)
                continue;
            int randomValue = Random.Range(1, 101);
            if (randomValue <= hero.RateAbs)
            {
                heroPoolCache.Add((int)hero.Id);
            }
            allHeroes.Remove(hero);
        }
        int targetCount = Mathf.Min(36, allHeroes.Count);

        for (int i = 0; i < targetCount; i++)
        {
            // 计算总权重
            float totalRate = 0;
            foreach (var hero in allHeroes)
            {
                totalRate += hero.RateWeight;
            }

            // 随机选择一个基于权重的位置
            float randomValue = Random.Range(0, totalRate);
            float accumulatedRate = 0;
            HeroConfig selectedHero = null;

            foreach (var hero in allHeroes)
            {
                if(hero.RateWeight <= 0)
                    continue;
                accumulatedRate += hero.RateWeight;
                if (accumulatedRate >= randomValue)
                {
                    selectedHero = hero;
                    break;
                }
            }

            // 将选中的英雄添加到缓存并从候选列表中移除
            if (selectedHero != null)
            {
                heroPoolCache.Add((int)selectedHero.Id);
                allHeroes.Remove(selectedHero);
            }
        }
    }

    public static int GetRandomHeroId()
    {
        if (heroPoolCache.Count == 0)
            UpdateHeroPoolCache();
        
        int randomIndex = Random.Range(0, heroPoolCache.Count);
        return heroPoolCache[randomIndex];
    }


    public static int GetPrice(HeroConfig heroCfg)
    {
        var baseP = (float)heroCfg.Total / 30 + 1;
        float bonus = 0;
        if (heroCfg.Str > 95) bonus+=0.5f;
        if (heroCfg.Str > 90) bonus+=0.5f;
        if (heroCfg.Inte > 95) bonus+=0.5f;
        if (heroCfg.Inte > 90) bonus+=0.5f;
        if (heroCfg.LeadShip > 95) bonus+=0.5f;
        if (heroCfg.LeadShip > 90) bonus+=0.5f;

        bonus += (((float)heroCfg.Hp + (float)heroCfg.Range / 17 * 30) - 300) / 40;

        if (heroCfg.Skills != null)
            foreach (var skillId in heroCfg.Skills)
                bonus += SkillConfig.GetConfig((uint)skillId).Price; //加上技能价格

        return Mathf.RoundToInt(baseP + bonus);
    }

    private static int[] cardExp = new int[] { 1, 2, 4, 7, 11, 16, 22, 29, 37, 46, 56, 67, 80, 94, 110, 127, 145, 164, 184, 205, };
    public static int GetCardLevel(int exp)
    {
        for(int i = 0; i < cardExp.Length; i++)
        {
            if(exp < cardExp[i])
                return i;
        }
        return cardExp.Length;
    }

    public static float GetExpRate(int exp)
    {
        int level = GetCardLevel(exp);
        if(level >= cardExp.Length)
            return 1f;
        if(level <= 1)
            return 0;
        return (float)(exp - cardExp[level - 1]) / (cardExp[level] - cardExp[level - 1]);
    }
}
