using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

public static class ConfigManager
{
    public static void Init()
    {
        HeroConfig.Load();
        SkillConfig.Load();
        BuffConfig.Load();

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
            else if (heroCfg.Job == "pao")   
                AddSkill(heroCfg, 200007);                
            if (Profile.Instance.cardLoves.Contains((int)heroCfg.Id))
                heroCfg.RateAbs += 50;
        }

    }

    private static void AddSkill(HeroConfig heroCfg, int skillId)
    {
        if (heroCfg.Skills == null)
            heroCfg.Skills = new int[1] { skillId };
        else
            System.Array.Resize(ref heroCfg.Skills, heroCfg.Skills.Length + 1);
        heroCfg.Skills[heroCfg.Skills.Length - 1] = skillId;
    }
}
