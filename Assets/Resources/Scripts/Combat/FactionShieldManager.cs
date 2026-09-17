using System.Collections.Generic;
using CommonConfig;

/// <summary>
/// 国家护盾机制：战斗开始时统计同阵营英雄数量，达到档位(2/3/4/5/6)后给该阵营英雄授予对应等级的国家护盾技能
/// （技能等级=档位 Lv1~5，效果由 SkillFactionShield 走技能/Buff 系统：护盾=最大生命×档位比例）。
/// 主公(王)加成由技能内结算（同阵营护盾额外+10%×王数，与旧默认护盾机制一致）。
/// </summary>
public static class FactionShieldManager
{
    public static void ApplyFactionShields()
    {
        var handledSides = new HashSet<int>();
        foreach (var player in GameManager.Instance.players)
        {
            if (!handledSides.Add(player.battleSide))
                continue;
            ApplyFactionShields(player.battleSide);
        }
    }

    private static void ApplyFactionShields(int side)
    {
        var units = WorldManager.Instance.GetUnitsMySide(side);
        if (units.Count == 0)
            return;

        // 统计各阵营英雄数量
        var factionCount = new Dictionary<int, int>();
        foreach (var unit in units)
        {
            if (!unit.isHero || unit.hp <= 0)
                continue;

            var heroCfg = HeroConfig.GetConfig(unit.heroId);
            var forceCfg = ConfigManager.GetForceConfig(heroCfg.Side);
            if (forceCfg == null || !forceCfg.JoinFactionShield)
                continue;
            var faction = heroCfg.Side;
            factionCount[faction] = factionCount.TryGetValue(faction, out var c) ? c + 1 : 1;
        }

        foreach (var kv in factionCount)
        {
            var level = GetFactionShieldLevel(kv.Value);
            if (level <= 0)
                continue;

            // 按等级取国家护盾技能Id（Sname+level → SkillConfig 2000001~2000005）
            var skillCfg = ConfigManager.GetSkillConfig(CombatConst.FactionShieldSkillSname, level);
            if (skillCfg == null)
            {
                GameLog.Error($"国家护盾技能配置缺失: Sname={CombatConst.FactionShieldSkillSname} Lv={level}");
                continue;
            }
            var skillId = skillCfg.Id;

            foreach (var unit in units)
            {
                if (unit.hp <= 0 || !unit.isHero)
                    continue;
                if (HeroConfig.GetConfig(unit.heroId).Side != kv.Key)
                    continue;

                unit.AddSkill(skillId, skillId, level);
                GameLog.Debug($"国家护盾 阵营{kv.Key} 英雄数{kv.Value} 授予技能等级{level}");
            }
        }
    }

    // 根据同阵营英雄数量获取技能等级（档位2/3/4/5/6人 → Lv1~5），未达标返回0
    private static int GetFactionShieldLevel(int count)
    {
        for (int i = CombatConst.FactionShieldCounts.Length - 1; i >= 0; i--)
        {
            if (count >= CombatConst.FactionShieldCounts[i])
                return i + 1;
        }
        return 0;
    }

}
