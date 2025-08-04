using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBook
{
    public class PlayerCfg
    {
        public int id;
        public string name;
        public string imgPath;
    }

    public static PlayerCfg playerWang = new PlayerCfg() { id = 1, name = "旺仔", imgPath = "PlayerPic/wang", };
    private static Dictionary<int, PlayerCfg> playerCfgDic = new Dictionary<int, PlayerCfg>();
    static PlayerBook()
    {
        Load();
    }

    private static void Load()
    {
        playerCfgDic[2] = new PlayerCfg() { id = 2, name = "甲鱼", imgPath = "PlayerPic/jiayu", };
        playerCfgDic[3] = new PlayerCfg() { id = 3, name = "三哥", imgPath = "PlayerPic/sange", };
        playerCfgDic[4] = new PlayerCfg() { id = 4, name = "魔童", imgPath = "PlayerPic/nezha", };
        playerCfgDic[5] = new PlayerCfg() { id = 5, name = "八戒", imgPath = "PlayerPic/bajie", };
        playerCfgDic[6] = new PlayerCfg() { id = 6, name = "大虎", imgPath = "PlayerPic/dahu", };
        playerCfgDic[7] = new PlayerCfg() { id = 7, name = "蓝猫", imgPath = "PlayerPic/mao", };
        playerCfgDic[8] = new PlayerCfg() { id = 8, name = "巴爸", imgPath = "PlayerPic/baba", };
        playerCfgDic[9] = new PlayerCfg() { id = 9, name = "蜘蛛", imgPath = "PlayerPic/zhizhu", };
        playerCfgDic[11] = new PlayerCfg() { id = 11, name = "小红", imgPath = "PlayerPic/xiaohong", };
        playerCfgDic[12] = new PlayerCfg() { id = 12, name = "电怪", imgPath = "PlayerPic/picaqiu", };
    }

    public static PlayerCfg GetPlayerCfg(int id)
    {
        if (playerCfgDic.TryGetValue(id, out PlayerCfg cfg))
        {
            return cfg;
        }
        return null;
    }

    public static PlayerCfg[] GetRandomN(int n)
    {
        // 确保 n 不超过玩家配置数量，避免数组越界
        int count = playerCfgDic.Count;
        n = Mathf.Min(n, count);
        PlayerCfg[] cfgs = new PlayerCfg[n];
        List<int> ids = new List<int>(playerCfgDic.Keys);
        for (int i = 0; i < n; i++)
        {
            int index = UnityEngine.Random.Range(0, ids.Count);
            int id = ids[index];
            cfgs[i] = GetPlayerCfg(id);
            ids.RemoveAt(index);
        }
        return cfgs;
    }
    
}
