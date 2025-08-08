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
        foreach(var heroCfg in HeroConfig.ConfigList)
        {
            if (heroCfg.Job == "shuai")
            {
                if (heroCfg.Skills == null)
                    heroCfg.Skills = new int[1] { 200003 };
                else
                    System.Array.Resize(ref heroCfg.Skills, heroCfg.Skills.Length + 1);
                    heroCfg.Skills[heroCfg.Skills.Length - 1] = 200003;
            }
        }

    }
}
