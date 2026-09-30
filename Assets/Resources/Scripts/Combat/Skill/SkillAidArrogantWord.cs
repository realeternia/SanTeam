using CommonConfig;
using UnityEngine;

/// <summary>
/// 许攸·傲言（术）：对目标造成 /strength 法术伤害（走统一公式 GetSkillDamage），并使其受到伤害提升 /strength2%
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

        // 直接伤害走统一公式（Strength2 为主数值/伤害槽，受施法者属性成长）
        var burst = GetSkillDamage();
        if (burst > 0)
            target.OnSkillDamaged(owner, skillId, burst);

        // 易伤 Buff "伤"：受击倍率增量读 skillCfg.Strength3
        BuffManager.AddBuff(target, owner, id, BuffConfig.GetConfigByNameS("伤").Id, skillCfg.BuffTime);

        EffectManager.PlaySkillEffect(target, skillCfg.HitEffect);
        return true;
    }
}