using CommonConfig;

/// <summary>
/// 国家护盾技能：战斗开始(BattleBegin)时按技能等级(1~5)给自身施加同阵营护盾。
/// 护盾 = 最大生命 × Strength（0.15~0.75，对应配置行 2000001~2000005，同阵营 2~6 人）；
/// 主公(王)上阵时护盾比例额外 +KingShieldBonusRate×王数（与旧默认护盾机制一致）。
/// 技能由 FactionShieldManager 在战斗开始时按同阵营人数授予对应等级，效果统一走 Buff 系统。
/// </summary>
public class SkillFactionShield : Skill
{
    public SkillFactionShield(int id, Chess unit) : base(id, unit)
    {
    }

    public override void BattleBegin()
    {
        var buffCfg = BuffConfig.GetConfigByNameS(skillCfg.BuffId);
        if (buffCfg == null)
        {
            GameLog.Error($"国家护盾技能缺少护盾Buff配置: BuffId={skillCfg.BuffId} 技能id={id}");
            return;
        }

        float rate = skillCfg.Strength + CombatConst.KingShieldBonusRate * CountKingOnSide();
        var shieldHp = (int)(owner.maxHp * rate);
        BuffManager.AddBuff(owner, owner, id, buffCfg.Id, skillCfg.BuffTime);
        var shield = owner.GetBuff(buffCfg.Id) as BuffShield;
        if (shield != null)
            shield.SetHp(shieldHp);
        GameLog.Debug($"国家护盾 技能id={id} 等级={Level} 护盾={shieldHp}({rate * 100:0}%)");
    }

    // 统计本侧上阵主公(王)数量（战斗开始时结算一次）
    private int CountKingOnSide()
    {
        int count = 0;
        foreach (var unit in WorldManager.Instance.GetUnitsMySide(owner.side))
        {
            if (unit.isHero && unit.hp > 0 && ConfigManager.IsKingHero(unit.heroId))
                count++;
        }
        return count;
    }
}
