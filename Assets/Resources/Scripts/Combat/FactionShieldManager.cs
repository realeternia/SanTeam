using System.Collections.Generic;
using CommonConfig;

/// <summary>
/// 默认护盾机制：战斗开始时统计同阵营英雄数量，达到档位(3/5/7/9)后该阵营英雄直接获得护盾(类似连线)。
/// 主公技(王/帅)：所在同阵营护盾效果加倍；同阵营护盾额外+10%（按上阵情况在初始化时结算一次）。
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
        var kingCount = 0;
        foreach (var unit in units)
        {
            if (!unit.isHero || unit.hp <= 0)
                continue;

            var heroCfg = HeroConfig.GetConfig(unit.heroId);
            if (heroCfg.Side  == 10)
                continue;
            var faction = heroCfg.Side;
            if (!factionCount.ContainsKey(faction))
                factionCount[faction] = 0;
            factionCount[faction]++;

            if (HeroConfig.GetConfig(unit.heroId).Job == "帅")
                kingCount++;
        }

        foreach (var kv in factionCount)
        {
            var rate = GetFactionShieldRate(kv.Value);
            if (rate <= 0)
                continue;

            // 主公(帅/王)上阵：同阵营护盾额外+10%（本侧有王即生效，初始化结算一次）
            if (kingCount > 0)
                rate += CombatConst.KingShieldBonusRate * kingCount;

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

}
