using CommonConfig;
using UnityEngine;

/// <summary>
/// 许攸·傲言（术）：对目标造成 /strength2 法术伤害，并使其受到伤害提升 /strength%
/// （Buff "伤"，时长 bufftime），配合队友集火。
/// </summary>
public class SkillAidArrogantWord : Skill
{
    public SkillAidArrogantWord(int id, Chess unit) : base(id, unit)
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

        // 直接法术伤害基数在 Strength2，受施法者法强成长
        var burst = (int)(skillCfg.Strength2 * (100 + owner.GetAttr("ap")) / 100);
        if (burst > 0)
            target.OnSkillDamaged(owner, skillId, burst);

        // 易伤 Buff "伤"：受击倍率增量读 skillCfg.Strength
        BuffManager.AddBuff(target, owner, id, BuffConfig.GetConfigByNameS("伤").Id, skillCfg.BuffTime);

        EffectManager.PlaySkillEffect(target, skillCfg.HitEffect);
        return true;
    }
}