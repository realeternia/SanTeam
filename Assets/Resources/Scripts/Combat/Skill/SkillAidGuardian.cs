using CommonConfig;

/// <summary>
/// 高览·御守（ScriptName = "SkillAidGuardian"）：
/// 单体伤害并给自己提升 StrengthBuff1[0] 点护甲（短名「甲」，由 BuffArmorAdd 施加/到期扣回）。
/// </summary>
public class SkillAidGuardian : Skill
{
    public SkillAidGuardian(int id, Chess unit) : base(id, unit)
    {
    }

    public override bool CheckAidSkill()
    {
        var target = owner.targetChess;
        if (target == null || target.hp <= 0)
            return false;
        if (!WorldManager.Instance.CheckInRange(owner.transform.position, target.transform.position, skillCfg.Range))
            return false;
        if (!CheckBurst(target))
            return false;

        var buffIds = GetSkillBuffIds();
        if (buffIds.Count < 1)
        {
            GameLog.Error($"御守 武将{owner.heroId} BuffId未配置");
            return false;
        }

        owner.PlayerAnim(skillCfg.Action);

        target.OnSkillDamaged(owner, skillId, GetSkillDamage());
        if (!string.IsNullOrEmpty(skillCfg.HitEffect))
            EffectManager.PlaySkillEffect(target, skillCfg.HitEffect);

        // 给自己加甲（数值取 StrengthBuff1[0]）
        BuffManager.AddBuff(owner, owner, id, buffIds[0], skillCfg.BuffTime);

        PlaySelfEffect();
        return true;
    }
}
