using System;
using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

public class PlayerBook
{
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
            if (cfg.Id > 1) // 保持原有条件
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
            int index = UnityEngine.Random.Range(0, trueIds.Count);
            resultIds.Add(trueIds[index]);
            trueIds.RemoveAt(index);
        }
        
        // 从false列表中随机选择falseCount个
        for (int i = 0; i < falseCount; i++)
        {
            int index = UnityEngine.Random.Range(0, falseIds.Count);
            resultIds.Add(falseIds[index]);
            falseIds.RemoveAt(index);
        }
        
        // 对结果进行shuffle
        for (int i = 0; i < resultIds.Count; i++)
        {
            int j = UnityEngine.Random.Range(i, resultIds.Count);
            int temp = resultIds[i];
            resultIds[i] = resultIds[j];
            resultIds[j] = temp;
        }
        
        return resultIds.ToArray();
    }

    public static List<Tuple<string, int>> GetCardNeeds(int id)
    {
        List<Tuple<string, int>> needs = new List<Tuple<string, int>>();
        PlayerConfig cfg = PlayerConfig.GetConfig(id);
        if (cfg == null)
            return needs;
        //cfg.Cardsneed是字符串数组，形如["atk","1","def","1","inte","1"]
        string[] needsStr = cfg.Cardsneed;
        if (needsStr == null)
            return needs;
            
        for (int i = 0; i < needsStr.Length; i += 2)
        {
            if (i + 1 < needsStr.Length)
            {
                string type = needsStr[i].Trim('"');
                if (int.TryParse(needsStr[i + 1].Trim('"'), out int num))
                {
                    needs.Add(new Tuple<string, int>(type, num));
                }
            }
        }
        return needs;
    }
    
}
