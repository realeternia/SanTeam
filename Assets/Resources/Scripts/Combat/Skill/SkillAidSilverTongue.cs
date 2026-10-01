using CommonConfig;
using UnityEngine;

/// <summary>
/// 张松·舌辩（术）：对目标造成 /damagestrength 法术伤害，并大幅减速（Buff "缓"，减速比例由 BuffSlowDown 读 StrengthBuff1[0]，时长 bufftime）。
/// </summary>
public class SkillAidSilverTongue : Skill
{
    public SkillAidSilverTongue(int id, Chess unit) : base(id, unit)
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
            target.OnSkillDamaged(owner, skillId, GetSkillDamage());
        BuffManager.AddBuff(target, owner, id, BuffConfig.GetConfigByNameS("缓").Id, skillCfg.BuffTime);

        EffectManager.PlaySkillEffect(target, skillCfg.HitEffect);
        return true;
    }
}