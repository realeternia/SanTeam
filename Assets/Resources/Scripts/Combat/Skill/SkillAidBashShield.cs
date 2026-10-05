using System;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 程普·横扫：对目标造成魔法伤害，并给自身挂护盾"盾"（护盾值 = Strength2[0] × (100+法强)/100，随法强成长）
/// </summary>
public class SkillAidBashShield : Skill
{
    public SkillAidBashShield(int id, Chess unit) : base(id, unit)
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
        target.OnSkillDamaged(owner, id, GetSkillDamage());
        EffectManager.PlaySkillEffect(target, skillCfg.HitEffect);

        var shieldId = BuffConfig.GetConfigByNameS("盾").Id;
        BuffManager.AddBuff(owner, owner, id, shieldId, skillCfg.BuffTime);
        var sh = owner.GetBuff(shieldId) as BuffShield;
        if (sh != null)
            sh.SetHp(GetSkillShield(0));

        return true;
    }
}