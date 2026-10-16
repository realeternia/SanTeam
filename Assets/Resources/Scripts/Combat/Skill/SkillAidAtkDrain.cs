using CommonConfig;
using UnityEngine;

/// <summary>
/// 马腾·腾驾：对目标造成法术伤害，并降低目标攻击、提升自身攻击
/// （"慑"BuffAtkDown 与"攻"BuffAtkAdd 均读 StrengthBuff1[0]；伤害走 DamageStrength）。
/// Buff 短名由 SkillConfig.BuffId 以逗号分隔配置：第 1 个=目标减攻Buff，第 2 个=自身加攻Buff（如 "慑,攻"）。
/// </summary>
public class SkillAidAtkDrain : Skill
{
    public SkillAidAtkDrain(int id, Chess unit) : base(id, unit)
    {
    }

    /// <summary>
    /// 腾驾：对目标造成伤害，降低目标攻击、提升自身攻击
    /// </summary>
    public override bool CheckAidSkill()
    {
        var target = owner.targetChess;
        if (target == null || target.hp <= 0)
            return false;
        if (!WorldManager.Instance.CheckInRange(owner.transform.position, target.transform.position, skillCfg.Range))
            return false;
        if (!CheckBurst(target))
            return false;

        // BuffId 逗号分隔：buffIds[0]=目标减攻Buff(慑)，buffIds[1]=自身加攻Buff(攻)
        var buffIds = GetSkillBuffIds();
        if (buffIds.Count < 2)
        {
            GameLog.Error($"SkillAidAtkDrain: BuffId 需配置减攻与加攻两个buff（如\"慑,攻\"）技能id={id}");
            return false;
        }

        owner.PlayerAnim(skillCfg.Action);
        PlaySelfEffect();
        EffectManager.PlaySkillEffect(target, skillCfg.HitEffect);

        if (GetSkillDamage() > 0)
            target.OnSkillDamaged(owner, id, GetSkillDamage());

        BuffManager.AddBuff(target, owner, id, buffIds[0], skillCfg.BuffTime);
        BuffManager.AddBuff(owner, owner, id, buffIds[1], skillCfg.BuffTime);
        return true;
    }
}
