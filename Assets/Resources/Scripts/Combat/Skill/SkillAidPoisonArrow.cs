using CommonConfig;
using UnityEngine;

/// <summary>
/// 张任·毒箭（ScriptName = "BowZhangRen"）：
/// 高额毒伤（基础技能伤害 × Strength2）并附加持续中毒Buff（"败"，其每秒dot读自身 skillCfg.Strength）。
/// </summary>
public class SkillAidPoisonArrow : Skill
{
    public SkillAidPoisonArrow(int id, Chess unit) : base(id, unit)
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

        // 高额直接伤害（基础技能伤害 × Strength2）
        target.OnSkillDamaged(owner, skillId, (int)(GetSkillDamage() * skillCfg.Strength2));

        // 附加持续中毒Buff（"败"，每秒dot读自身 skillCfg.Strength）
        BuffManager.AddBuff(target, owner, id, BuffConfig.GetConfigByNameS("败").Id, skillCfg.BuffTime);

        EffectManager.PlaySkillEffect(target, skillCfg.HitEffect);
        return true;
    }
}
