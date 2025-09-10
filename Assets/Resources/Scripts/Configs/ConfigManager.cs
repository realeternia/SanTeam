using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

public static class ConfigManager
{
    private static Dictionary<int, HashSet<int>> heroFriendDict = new Dictionary<int, HashSet<int>>();

    public static void Init()
    {
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
            if (Profile.Instance.cardLoves != null && Profile.Instance.cardLoves.Contains((int)heroCfg.Id))
                heroCfg.RateAbs = 65;
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
                        heroFriendDict.Add(id1, new HashSet<int>());
                    heroFriendDict[id1].Add(id2);

                    if (!heroFriendDict.ContainsKey(id2))
                        heroFriendDict.Add(id2, new HashSet<int>());
                    heroFriendDict[id2].Add(id1);
                }
            }
        }

    }

    public static bool IsFriend(int heroId, int friendId)
    {
        if (heroFriendDict.TryGetValue(heroId, out HashSet<int> value))
        {
            return value.Contains(friendId);
        }
        return false;
    }

    private static void AddSkill(HeroConfig heroCfg, int skillId)
    {
        if (heroCfg.Skills == null)
            heroCfg.Skills = new int[1] { skillId };
        else
            System.Array.Resize(ref heroCfg.Skills, heroCfg.Skills.Length + 1);
        heroCfg.Skills[heroCfg.Skills.Length - 1] = skillId;
    }

    public static bool IsHeroCard(int cardId)
    {
        return cardId < 200000;
    }
}
