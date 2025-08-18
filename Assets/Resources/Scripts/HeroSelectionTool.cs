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

        int[] sideCounts = new int[4];
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
                sideCounts[hero.Side - 1]++;
            }
            allHeroes.Remove(hero);
        }       

        // 先随机选择5-7张Side=4的卡牌
        List<HeroConfig> side4Heroes = allHeroes.FindAll(hero => hero.Side == 4);
        if (side4Heroes.Count > 0)
        {
            int side4Count = Random.Range(5, 8);
            side4Count = Mathf.Min(side4Count, side4Heroes.Count);
            
            List<HeroConfig> tempSide4Heroes = new List<HeroConfig>(side4Heroes);
            for (int i = sideCounts[3]; i < side4Count; i++)
            {
                int randomIndex = Random.Range(0, tempSide4Heroes.Count);
                heroPoolCache.Add((int)tempSide4Heroes[randomIndex].Id);
                allHeroes.Remove(tempSide4Heroes[randomIndex]);
                tempSide4Heroes.RemoveAt(randomIndex);
                sideCounts[3]++;
            }
        }

        // 准备按阵营1-3选择卡牌，保证各阵营相差最多一张
        List<List<HeroConfig>> sideHeroes = new List<List<HeroConfig>>
        {
            allHeroes.FindAll(hero => hero.Side == 1),
            allHeroes.FindAll(hero => hero.Side == 2),
            allHeroes.FindAll(hero => hero.Side == 3)
        };

        int targetCount = Mathf.Min(50, allHeroes.Count);

        while (heroPoolCache.Count < targetCount)
        {
            // 找出当前数量最少的阵营
            int minIndex = 0;
            for (int i = 1; i < 3; i++)
            {
                if (sideCounts[i] < sideCounts[minIndex])
                {
                    minIndex = i;
                }
            }

            // 从最少的阵营中按权重选择一张卡牌
            List<HeroConfig> currentSideHeroes = sideHeroes[minIndex];
            if (currentSideHeroes.Count > 0)
            {
                // 计算当前阵营总权重
                float totalRate = 0;
                foreach (var hero in currentSideHeroes)
                {
                    totalRate += hero.RateWeight;
                }

                if (totalRate > 0)
                {
                    float randomValue = Random.Range(0, totalRate);
                    float accumulatedRate = 0;
                    HeroConfig selectedHero = null;

                    foreach (var hero in currentSideHeroes)
                    {
                        if (hero.RateWeight <= 0)
                            continue;
                        accumulatedRate += hero.RateWeight;
                        if (accumulatedRate >= randomValue)
                        {
                            selectedHero = hero;
                            break;
                        }
                    }

                    if (selectedHero != null)
                    {
                        heroPoolCache.Add((int)selectedHero.Id);
                        allHeroes.Remove(selectedHero);
                        sideHeroes[minIndex].Remove(selectedHero);
                        sideCounts[minIndex]++;
                    }
                }
                else
                {
                    // 如果总权重为0，随机选一张
                    int randomIndex = Random.Range(0, currentSideHeroes.Count);
                    heroPoolCache.Add((int)currentSideHeroes[randomIndex].Id);
                    allHeroes.Remove(currentSideHeroes[randomIndex]);
                    sideHeroes[minIndex].RemoveAt(randomIndex);
                    sideCounts[minIndex]++;
                }
            }
            else
            {
                // 如果当前阵营没有卡牌了，跳过该阵营
                // 找到下一个还有卡牌的阵营
                for (int i = 0; i < 3; i++)
                {
                    if (sideHeroes[i].Count > 0)
                    {
                        minIndex = i;
                        break;
                    }
                }
            }
        }

        heroPoolCache.Sort((a, b) =>
        {
            var configA = HeroConfig.GetConfig(a);
            var configB = HeroConfig.GetConfig(b);
            int sideCompare = configA.Side.CompareTo(configB.Side);
            if (sideCompare != 0)
            {
                return sideCompare;
            }

            // 检查ID是否在100100以下
            bool isBelow100100A = a < 100100;
            bool isBelow100100B = b < 100100;
            if (isBelow100100A != isBelow100100B)
            {
                return isBelow100100A ? -1 : 1;
            }

            // 按Total排序
            return configB.Total.CompareTo(configA.Total);
        });

    }

    public static List<int> GetHeroPoolCache()
    {
        UpdateHeroPoolCache();
        return heroPoolCache;
    }

    public static void SetBanList(List<int> banList)
    {
        heroPoolCache.RemoveAll(id => banList.Contains(id));
    }



    public static int GetRandomHeroId()
    {
        if (heroPoolCache.Count == 0)
            UpdateHeroPoolCache();
        
        int randomIndex = Random.Range(0, heroPoolCache.Count);
        return heroPoolCache[randomIndex];
    }

    public static bool HasHeroInPool(int heroId)
    {
        return heroPoolCache.Contains(heroId);
    }

    public static int GetPrice(HeroConfig heroCfg)
    {
        var baseP = Mathf.Pow((float)heroCfg.Total, 1.4f) / 125;
        float bonus = 0;
        if (heroCfg.Str >= 90) bonus += (heroCfg.Str - 89) * 0.01f;
        if (heroCfg.Inte >= 90) bonus += (heroCfg.Inte - 89) * 0.01f;
        if (heroCfg.LeadShip >= 90) bonus += (heroCfg.LeadShip - 89) * 0.01f;
        bonus += ((float)heroCfg.Hp + (float)heroCfg.Range / 17 * 30 - 330) / 330;

        if (heroCfg.Total >= 220)
        { //救一下偏科的人
            if (heroCfg.Str < 70)
                bonus -= (70 - heroCfg.Str) * 0.005f;
            if (heroCfg.Inte < 70)
                bonus -= (70 - heroCfg.Inte) * 0.005f;
            if (heroCfg.LeadShip < 70)
                bonus -= (70 - heroCfg.LeadShip) * 0.005f;
        }

        var skillP = 0;
        if (heroCfg.Skills != null)
            foreach (var skillId in heroCfg.Skills)
                skillP += SkillConfig.GetConfig(skillId).Price; //加上技能价格

        var finalP = baseP * (1 + bonus) + skillP;
        return Mathf.RoundToInt(finalP);
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
