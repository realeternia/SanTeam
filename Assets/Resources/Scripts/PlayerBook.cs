using System;
using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

public class PlayerBook
{
    // PVE怪物专用虚拟玩家id（PlayerConfig 999）：不参与PVP匹配，无PlayerInfo实体
    public const int MonsterPlayerId = 999;

    public static int GetWang()
    {
        return 1;
    }

    public static int[] GetRandomN(int n)
    {
        // 收集CanPlayer=true和CanPlayer=false的配置ID
        List<int> trueIds = new List<int>();
        List<int> falseIds = new List<int>();

        foreach (PlayerConfig cfg in PlayerConfig.ConfigList)
        {
            if (cfg.Id > 1 && cfg.Id != MonsterPlayerId) // 排除怪物虚拟玩家，不参与正常PVP匹配
            {
                if (cfg.CanPlay)
                    trueIds.Add(cfg.Id);
                else
                    falseIds.Add(cfg.Id);
            }
        }
        
        // 计算需要的数量，确保不超过实际可用数量
        int trueCount = Mathf.Min(4, trueIds.Count);
        int falseCount = Mathf.Max(0, Mathf.Min(n - 4, falseIds.Count));
        
        // 调整n，确保总数合理
        n = trueCount + falseCount;
        if (n <= 0) return new int[0];
        
        List<int> resultIds = new List<int>();
        
        // 从true列表中随机选择trueCount个
        for (int i = 0; i < trueCount; i++)
        {
            int index = SysRandom.Range(0, trueIds.Count);
            resultIds.Add(trueIds[index]);
            trueIds.RemoveAt(index);
        }
        
        // 从false列表中随机选择falseCount个
        for (int i = 0; i < falseCount; i++)
        {
            int index = SysRandom.Range(0, falseIds.Count);
            resultIds.Add(falseIds[index]);
            falseIds.RemoveAt(index);
        }
        
        // 对结果进行shuffle
        for (int i = 0; i < resultIds.Count; i++)
        {
            int j = SysRandom.Range(i, resultIds.Count);
            int temp = resultIds[i];
            resultIds[i] = resultIds[j];
            resultIds[j] = temp;
        }
        
        return resultIds.ToArray();
    }

}
