using CommonConfig;
using UnityEngine;

/// <summary>
/// 孙尚香·纵舞：每射击2次（受CD/Mp限制）向远离目标方向后撤一段距离，
/// 边撤边射——向后撤前的目标回射一箭，造成 /damagestrength 法术伤害。
/// </summary>
public class SkillAidDance : Skill
{
    /// <summary>后撤距离(米)</summary>
    private const float RetreatDistance = 15f;

    public SkillAidDance(int id, Chess unit) : base(id, unit)
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

        // 沿远离目标方向后撤 RetreatDistance 米
        Vector3 dir = (owner.transform.position - target.transform.position).normalized;
        if (dir.magnitude < 0.001f)
            dir = Vector3.forward;
        owner.transform.position += dir * RetreatDistance;

        owner.PlayerAnim(skillCfg.Action);
        EffectManager.PlaySkillEffect(owner, skillCfg.HitEffect);

        // 边撤边射：向后撤前的目标回射一箭
        if (skillCfg.DamageStrength > 0f)
            WorldManager.Instance.CreateSpellMissile(owner, target, owner.transform.position, id, GetSkillDamage(), skillCfg.HitEffect);
        return true;
    }
}
