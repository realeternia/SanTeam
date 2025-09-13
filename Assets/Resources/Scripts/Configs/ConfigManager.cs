using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CommonConfig;
using UnityEngine;

public static class ConfigManager
{
    private static Dictionary<int, Dictionary<int, int>> heroFriendDict = new Dictionary<int, Dictionary<int, int>>();
    private static Dictionary<int, HashSet<int>> heroFriendInfoDict = new Dictionary<int, HashSet<int>>(); // heroId, heroId, level
    private static int tempFriendIdIdx = 1000;
    private static bool hasInit = false;

    public static void Init()
    {
        if (hasInit)
            return;
        hasInit = true;
        
        HeroConfig.Load();
        SkillConfig.Load();
        BuffConfig.Load();
        ItemConfig.Load();
        SoldierConfig.Load();
        ShopConfig.Load();
        PlayerConfig.Load();
        HeroFriendConfig.Load();
        FormulaLearnAttrConfig.Load();

        PostModify();
    }

    public static void PostModify()
    {
        foreach (var heroCfg in HeroConfig.ConfigList)
        {
            if (heroCfg.Job == "shuai")
                AddSkill(heroCfg, 200003);
            else if (heroCfg.Job == "shi")
                AddSkill(heroCfg, 200004);
            else if (heroCfg.Job == "che")
                AddSkill(heroCfg, 200002);
            else if (heroCfg.Job == "ma")
                AddSkill(heroCfg, 200005);
            else if (heroCfg.Job == "xiang")
                AddSkill(heroCfg, 200006);
            else if (heroCfg.Job == "gong")
                AddSkill(heroCfg, 200007);
            else if (heroCfg.Job == "mou")
                AddSkill(heroCfg, 200008);
            AddSkill(heroCfg, 200008);
            if (Profile.Instance.cardLoves != null && Profile.Instance.cardLoves.Contains((int)heroCfg.Id))
                heroCfg.RateAbs = 65;
        }

        for (int i = 1; i <= 6; i++)
        {
            // 筛选side=i且FriendCount<4的英雄
            var heroList = HeroConfig.ConfigList.Where(x => x.Side == i && x.FriendCount < 4).ToList();
            
            // 智力分组
            var inteHighList = heroList.Where(x => x.Inte > 80).ToList();
            var inteLowList = heroList.Where(x => x.Inte < 60).ToList();
            
            // 随机2v2匹配
            CreateRandomFriendPairs(inteHighList, inteLowList, "智力辅佐");
            
            // 武力分组和匹配
            var strHighList = heroList.Where(x => x.Str > 80).ToList();
            var strLowList = heroList.Where(x => x.Str < 60).ToList();
            
            CreateRandomFriendPairs(strHighList, strLowList, "武力指导"); 

            heroList = HeroConfig.ConfigList.Where(x => x.Side == i && x.FriendCount < 2).ToList();
            if(heroList.Count > 2)
            {
                // 随机打乱heroList，然后分成前一半和后一半
                var random = new System.Random();
                var shuffledList = heroList.OrderBy(x => random.Next()).ToList();
                int halfCount = shuffledList.Count / 2;
                var firstHalf = shuffledList.Take(halfCount).ToList();
                var secondHalf = shuffledList.Skip(halfCount).ToList();
                
                CreateRandomFriendPairs(firstHalf, secondHalf, "协作如坚");
            }

            heroList = HeroConfig.ConfigList.Where(x => x.Side == i && x.FriendCount < 1).ToList();
            if(heroList.Count > 2)
            {
                // 随机打乱heroList，然后分成前一半和后一半
                var random = new System.Random();
                var shuffledList = heroList.OrderBy(x => random.Next()).ToList();
                int halfCount = shuffledList.Count / 2;
                var firstHalf = shuffledList.Take(halfCount).ToList();
                var secondHalf = shuffledList.Skip(halfCount).ToList();
                
                CreateRandomFriendPairs(firstHalf, secondHalf, "协作如坚2");
            }            
        }

        foreach (var heroFriendCfg in HeroFriendConfig.ConfigList)
        {
            var friendIds = heroFriendCfg.Heros;
            for (int i = 0; i < friendIds.Length; i++)
            {
                for (int j = i + 1; j < friendIds.Length; j++)
                {
                    int id1 = friendIds[i];
                    int id2 = friendIds[j];

                    // 双向添加，确保两两配对
                    if (!heroFriendDict.ContainsKey(id1))
                        heroFriendDict.Add(id1, new Dictionary<int, int>());
                    heroFriendDict[id1][id2] = Math.Max(heroFriendCfg.Level, heroFriendDict[id1].ContainsKey(id2) ? heroFriendDict[id1][id2] : 0);

                    if (!heroFriendDict.ContainsKey(id2))
                        heroFriendDict.Add(id2, new Dictionary<int, int>());
                    heroFriendDict[id2][id1] = Math.Max(heroFriendCfg.Level, heroFriendDict[id2].ContainsKey(id1) ? heroFriendDict[id2][id1] : 0);

                }
                if (!heroFriendInfoDict.ContainsKey(friendIds[i]))
                    heroFriendInfoDict.Add(friendIds[i], new HashSet<int>());
                heroFriendInfoDict[friendIds[i]].Add(heroFriendCfg.Id);
            }
        }

    }

    public static int GetFriendLevel(int heroId, int friendId)
    {
        if (heroFriendDict.TryGetValue(heroId, out Dictionary<int, int> value))
        {   
            return value.ContainsKey(friendId) ? value[friendId] : 0;
        }
        return 0;
    }

    public static HashSet<int> GetHeroFriendInfo(int heroId)
    {
        if (heroFriendInfoDict.TryGetValue(heroId, out HashSet<int> value))
        {
            return value;
        }
        return null;
    }

    private static void AddSkill(HeroConfig heroCfg, int skillId)
    {
        if (heroCfg.Skills == null)
            heroCfg.Skills = new int[1] { skillId };
        else
            System.Array.Resize(ref heroCfg.Skills, heroCfg.Skills.Length + 1);
        heroCfg.Skills[heroCfg.Skills.Length - 1] = skillId;
    }

    private static void CreateRandomFriendPairs(List<HeroConfig> inteHighList, List<HeroConfig> inteLowList, string name)
    {
        if (inteHighList.Count < 1 || inteLowList.Count < 1)
            return;

        var rnd = new System.Random();
        var shuffledHigh = inteHighList.OrderBy(x => rnd.Next()).ToList();
        var shuffledLow = inteLowList.OrderBy(x => rnd.Next()).ToList();

        // 循环配对直到消耗完所有英雄
        while (shuffledHigh.Count > 0 && shuffledLow.Count > 0)
        {
            // 每次取2个高智力和2个低智力
            var highPair = shuffledHigh.Take(UnityEngine.Random.Range(1, 3)).ToList();
            var lowPair = shuffledLow.Take(UnityEngine.Random.Range(1, 3)).ToList();
            
            var heroes = highPair.Concat(lowPair).Select(x => x.Id).ToArray();
            
            var config = new HeroFriendConfig(tempFriendIdIdx, name, 2, heroes);
            HeroFriendConfig.Add(tempFriendIdIdx, config);
            tempFriendIdIdx++;

            Debug.Log($"创建" + name + $" 配对: {string.Join(",", heroes.Select(id => HeroConfig.GetConfig(id).Name))}");

            // 移除已配对的英雄
            shuffledHigh.RemoveRange(0, highPair.Count);
            shuffledLow.RemoveRange(0, lowPair.Count);
        }
    }

 public static bool IsHeroCard(int cardId)
    {
        return cardId < 200000;
    }
}
