using CommonConfig;
using UnityEngine;

/// <summary>
/// 高览·御守（ScriptName = "SkillAidGuardian"）：
/// 单体伤害并给自己套盾，护盾量 = Strength2[0] × (100+法强)/100（随法强成长）。
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

        owner.PlayerAnim(skillCfg.Action);

        target.OnSkillDamaged(owner, skillId, GetSkillDamage());

        // 给自己套盾（"盾"，护盾量 = Strength2[0] × (100+法强)/100）
        int shieldId = BuffConfig.GetConfigByNameS("盾").Id;
        BuffManager.AddBuff(owner, owner, id, shieldId, skillCfg.BuffTime);
        var sh = owner.GetBuff(shieldId) as BuffShield;
        if (sh != null)
            sh.SetHp(GetSkillShield(0));

        EffectManager.PlaySkillEffect(owner, skillCfg.HitEffect);
        return true;
    }
}
