using CommonConfig;

/// <summary>
/// 通用"士兵属性成长"技能（ScriptName = "SkillSoldierGrow"）：
/// 本侧士兵永久叠加 Strength2[0] 比例的攻击；第二条数值按标记写入护甲或攻速。
///
/// TargetType 标记：
/// · archers = 只作用于弓兵（attackRange &gt;= CombatConst.MeleeRange，沮授·强弓）
/// · selfToo = 施法者自身也同步获得加成（沮授·强弓）
/// · haste   = Strength2[1] 作为攻速加成（沮授·强弓）；缺省作为固定护甲（马良·励军）
/// </summary>
public class SkillSoldierGrow : Skill
{
    public SkillSoldierGrow(int id, Chess unit) : base(id, unit)
    {
    }

    public override bool CheckAidSkill()
    {
        if (!CheckBurst(null))
            return false;

        owner.PlayerAnim(skillCfg.Action);

        var atkPct = GetStrength2(0);
        var second = GetStrength2(1);
        var archers = HasSkillFlag("archers");
        var haste = HasSkillFlag("haste");

        foreach (var s in WorldManager.Instance.GetUnitsMySide(owner.side))
        {
            if (s.isHero)
                continue;
            if (archers && s.attackRange < CombatConst.MeleeRange)
                continue;
            ApplyGrow(s, atkPct, second, haste);
        }

        if (HasSkillFlag("selfToo"))
            ApplyGrow(owner, atkPct, second, haste);

        PlaySelfEffect();
        return true;
    }

    private static void ApplyGrow(Chess c, float atkPct, float second, bool haste)
    {
        c.atk += (int)(c.atk * atkPct);
        if (haste)
            c.attackSpeedRate += second;
        else
            c.armor += (int)second;
    }
}
