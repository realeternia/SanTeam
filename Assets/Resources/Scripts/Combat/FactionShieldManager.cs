using System.Collections.Generic;
using CommonConfig;

/// <summary>
/// 默认护盾机制：战斗开始时统计同阵营英雄数量，达到档位(3/5/7/9)后该阵营英雄直接获得护盾(类似连线)。
/// 主公技(王/帅)：所在同阵营护盾效果加倍；有王(帅)在场时，全队护盾额外+10%。
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
            var faction = HeroConfig.GetConfig(unit.heroId).Side;
            if (!factionCount.ContainsKey(faction))
                factionCount[faction] = 0;
            factionCount[faction]++;
        }

        foreach (var kv in factionCount)
        {
            var rate = GetFactionShieldRate(kv.Value);
            if (rate <= 0)
                continue;

            // 主公技：同阵营存在主公时护盾加倍
            if (HasMasterShieldSkill(units, kv.Key))
                rate *= CombatConst.MasterShieldDouble;

            // 主公(帅/王)在场：本侧有王时，全队护盾额外+10%
            if (CountKings(units) > 0)
                rate += CombatConst.KingShieldBonusRate;

            foreach (var unit in units)
            {
                if (unit.hp <= 0 || !unit.isHero)
                    continue;
                if (HeroConfig.GetConfig(unit.heroId).Side != kv.Key)
                    continue;

                var shieldHp = (int)(unit.maxHp * rate);
                BuffManager.AddShield(unit, unit, shieldHp, CombatConst.FactionShieldTime);
                GameLog.Debug($"FactionShield 阵营{kv.Key} 英雄数{kv.Value} 护盾{shieldHp}({rate * 100:0}%)");
            }
        }
    }

    // 根据同阵营英雄数量获取护盾百分比，未达标返回0
    private static float GetFactionShieldRate(int count)
    {
        for (int i = CombatConst.FactionShieldCounts.Length - 1; i >= 0; i--)
        {
            if (count >= CombatConst.FactionShieldCounts[i])
                return CombatConst.FactionShieldRates[i];
        }
        return 0;
    }

    // 本侧上阵的王(帅/主公)英雄数：按英雄职业的职业技能缩写(JobConfig.SkillId)是否为 帅 判定
    private static int CountKings(List<Chess> units)
    {
        var count = 0;
        foreach (var unit in units)
        {
            if (unit.hp <= 0 || !unit.isHero)
                continue;
            if (HeroConfig.GetConfig(unit.heroId).Job == "帅")
                count++;
        }
        return count;
    }

    // 该阵营是否存在携带主公技的英雄
    private static bool HasMasterShieldSkill(List<Chess> units, int faction)
    {
        foreach (var unit in units)
        {
            if (unit.hp <= 0 || !unit.isHero)
                continue;
            if (HeroConfig.GetConfig(unit.heroId).Side != faction)
                continue;
            foreach (var skill in unit.skills)
            {
                if (skill.id == CombatConst.MasterShieldSkillId)
                    return true;
            }
        }
        return false;
    }
}
