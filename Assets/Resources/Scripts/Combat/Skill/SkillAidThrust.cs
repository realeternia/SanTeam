using CommonConfig;
using UnityEngine;

/// <summary>
/// 曹真·虎豹 / 张梁·黄天：对目标造成 /damagestrength 法术伤害，不附加任何 buff（低费单体补刀）
/// </summary>
public class SkillAidThrust : Skill
{
    public SkillAidThrust(int id, Chess unit) : base(id, unit)
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

        owner.PlayerAnim(skillCfg.Action);

        if (GetSkillDamage() > 0)
            target.OnSkillDamaged(owner, id, GetSkillDamage());

        EffectManager.PlaySkillEffect(target, skillCfg.HitEffect);
        return true;
    }
}