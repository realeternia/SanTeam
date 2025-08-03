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
        int targetCount = Mathf.Min(36, allHeroes.Count);

        for (int i = 0; i < targetCount; i++)
        {
            // 计算总权重
            float totalRate = 0;
            foreach (var hero in allHeroes)
            {
                totalRate += hero.Rate;
            }

            // 随机选择一个基于权重的位置
            float randomValue = Random.Range(0, totalRate);
            float accumulatedRate = 0;
            HeroConfig selectedHero = null;

            foreach (var hero in allHeroes)
            {
                accumulatedRate += hero.Rate;
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
        var baseP = heroCfg.Total / 30 + 1;
        int bonus = 0;
        if (heroCfg.Str > 95) bonus++;
        if (heroCfg.Str > 90) bonus++;
        if (heroCfg.Inte > 95) bonus++;
        if (heroCfg.Inte > 90) bonus++;
        if (heroCfg.LeadShip > 95) bonus++;
        if (heroCfg.LeadShip > 90) bonus++;
        baseP += bonus;
        return baseP;
    }


    private static Dictionary<int, float> priceRateCache = new Dictionary<int, float>();
    public static float GetTotalPriceRate(int heroId)
    {
        // 定义静态字典用于缓存计算结果
        
        if (priceRateCache.TryGetValue(heroId, out float cachedRate))
        {
            return cachedRate;
        }
        
        var heroCfg = HeroConfig.GetConfig((uint)heroId);
        if (heroCfg == null)
        {
            return 0f;
        }
        
        int heroPrice = GetPrice(heroCfg);
        int minTotal = int.MaxValue;
        
        // 遍历所有英雄配置，找出同价格卡牌的最低Total值
        foreach (var config in HeroConfig.ConfigList)
        {
            int currentPrice = GetPrice(config);
            if (currentPrice == heroPrice)
            {
                if (config.Total < minTotal)
                {
                    minTotal = config.Total;
                }
            }
        }
        
        if (minTotal == 0)
        {
            return 0f;
        }
        
        float rate = (float)heroCfg.Total / minTotal;
        priceRateCache[heroId] = rate;
        return rate;
    }
}
