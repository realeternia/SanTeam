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

        public AICardConfig aiConfig = new AICardConfig();

    }


    // 提取 AI 相关配置的类
    public class AICardConfig
    {
        public int priceLower = 19;
        public int priceUpper = 22;
        public float priceOutRate = .3f; //价格区间外卡牌的兴趣度折扣
        public float sameCardRate = 3f; //已经拥有卡牌的兴趣倍率
        public int cardLimit = 7; //卡牌上限
        public float futureRate = 0.4f;
        public float findMasterRate = 1f; //寻找主卡的兴趣倍率

        public float pickRangeCardRate = 1.5f;
        public float pickInteCardRate = 1.6f;
        public float pickSide = 0; //0表示所有阵营
    }    

    public static PlayerCfg playerWang = new PlayerCfg() { id = 1, name = "旺仔", imgPath = "PlayerPic/wang", };
    private static Dictionary<int, PlayerCfg> playerCfgDic = new Dictionary<int, PlayerCfg>();
    static PlayerBook()
    {
        Load();
    }

    private static void Load()
    {
        // 低级叠卡
        playerCfgDic[2] = new PlayerCfg() { id = 2, name = "甲鱼", imgPath = "PlayerPic/jiayu", aiConfig = new AICardConfig() { priceLower = 15, priceUpper = 18, sameCardRate = 5f, cardLimit = 8, futureRate = 0.7f, priceOutRate = 0.1f } };

        // 默认ai
        playerCfgDic[3] = new PlayerCfg() { id = 3, name = "三哥", imgPath = "PlayerPic/sange", aiConfig = new AICardConfig() { cardLimit = 9, } };


        playerCfgDic[4] = new PlayerCfg() { id = 4, name = "魔童", imgPath = "PlayerPic/nezha", aiConfig = new AICardConfig() { pickInteCardRate = 3f } };
        playerCfgDic[5] = new PlayerCfg() { id = 5, name = "八戒", imgPath = "PlayerPic/bajie", aiConfig = new AICardConfig() { cardLimit = 9, pickRangeCardRate = 2.5f, pickInteCardRate = 2.5f, } };

        // 高级叠卡
        playerCfgDic[6] = new PlayerCfg() { id = 6, name = "大虎", imgPath = "PlayerPic/dahu", aiConfig = new AICardConfig() { priceLower = 20, priceUpper = 33, sameCardRate = 5f, futureRate = 0.6f, priceOutRate = 0.1f, } };

        // 曹操流
        playerCfgDic[7] = new PlayerCfg() { id = 7, name = "蓝猫", imgPath = "PlayerPic/mao", aiConfig = new AICardConfig() { pickSide = 2, findMasterRate = 2.5f, } };

        // 默认ai
        playerCfgDic[8] = new PlayerCfg() { id = 8, name = "巴爸", imgPath = "PlayerPic/baba", aiConfig = new AICardConfig() { futureRate = 0.2f, cardLimit = 9, } };

        playerCfgDic[9] = new PlayerCfg() { id = 9, name = "蜘蛛", imgPath = "PlayerPic/zhizhu", aiConfig = new AICardConfig() { cardLimit = 11, futureRate = 0.25f, } };

        // 孙权流
        playerCfgDic[11] = new PlayerCfg() { id = 11, name = "小红", imgPath = "PlayerPic/xiaohong", aiConfig = new AICardConfig() { pickSide = 3, findMasterRate = 2.5f, } };
        // 刘备
        playerCfgDic[12] = new PlayerCfg() { id = 12, name = "电怪", imgPath = "PlayerPic/picaqiu", aiConfig = new AICardConfig() { pickSide = 1, findMasterRate = 3f, } };

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
