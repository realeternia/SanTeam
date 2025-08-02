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

    public static int GetRandomHeroId()
    {
        // Get the values from the dictionary and convert to a list
        List<HeroConfig> configValues = new List<HeroConfig>(HeroConfig.ConfigList);
        
        // Check if the list is empty to avoid index errors
        if (configValues.Count == 0)
        {
            Debug.LogError("HeroConfig.ConfigList is empty!");
            return -1; // Return invalid ID if no heroes are available
        }
        
        // Generate a random index within the valid range
        int randomIndex = Random.Range(0, configValues.Count);
        return (int)configValues[randomIndex].Id;
    }

    
 public static    int GetPrice(HeroConfig heroCfg)
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
}
