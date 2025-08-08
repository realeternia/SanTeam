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
    }
}
