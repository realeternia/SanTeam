using System;
using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

public class PlayerBook
{
    public static PlayerConfig GetWang()
    {
        return PlayerConfig.GetConfig(1);
    }

    public static PlayerConfig[] GetRandomN(int n)
    {
        // 确保 n 不超过玩家配置数量，避免数组越界
        int count = PlayerConfig.ConfigList.Count;
        n = Mathf.Min(n, count);
        PlayerConfig[] cfgs = new PlayerConfig[n];
        List<int> ids = new List<int>();
        foreach (PlayerConfig cfg in PlayerConfig.ConfigList)
            if(cfg.Id > 1)
                ids.Add(cfg.Id);
        for (int i = 0; i < n; i++)
        {
            int index = UnityEngine.Random.Range(0, ids.Count);
            int id = ids[index];
            cfgs[i] = PlayerConfig.GetConfig(id);
            ids.RemoveAt(index);
        }
        return cfgs;
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
