using CommonConfig;
using UnityEngine;

/// <summary>
/// 杜预·发火：对当前目标造成 /strength 法术伤害，纯单体、不带任何 buff/dot（低费单体补刀）。
/// </summary>
public class SkillAidFireShot : Skill
{
    public SkillAidFireShot(int id, Chess unit) : base(id, unit)
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
