using CommonConfig;
using UnityEngine;

/// <summary>
/// 夏侯渊·猎鹰（ScriptName = "BowXiaHouYuan"）：
/// 单体大额伤害，并给目标施加减防Buff（"破"），减防数值由 BuffArmorDown 读 skillCfg.StrengthBuff1[0]。
/// </summary>
public class SkillAidArmorShred : Skill
{
    public SkillAidArmorShred(int id, Chess unit) : base(id, unit)
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
        EffectManager.PlaySkillEffect(target, skillCfg.HitEffect);

        target.OnSkillDamaged(owner, skillId, GetSkillDamage());

        // 给目标挂减防Buff（"破"，减防比例由 BuffArmorDown 读 skillCfg.StrengthBuff1[0]）
        if (skillCfg.StrengthBuff1[0] > 0)
            BuffManager.AddBuff(target, owner, id, BuffConfig.GetConfigByNameS("破").Id, skillCfg.BuffTime);
        return true;
    }
}
