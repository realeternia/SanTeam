using System;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 程普·横扫：对自身范围（Area）内若干敌人（TargetCount）各造成魔法伤害，并给自身挂护盾"盾"（护盾值 = 最大生命 × strength2）
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
        if (!CheckBurst(target))
            return false;

        var list = WorldManager.Instance.GetUnitsInRange(owner.transform.position, skillCfg.Area, owner.side, true);
        WorldManager.Instance.RandomSelect(list, skillCfg.TargetCount);
        foreach (var u in list)
            u.OnSkillDamaged(owner, id, GetSkillDamage());

        var shieldId = BuffConfig.GetConfigByNameS("盾").Id;
        BuffManager.AddBuff(owner, owner, id, shieldId, skillCfg.BuffTime);
        var sh = owner.GetBuff(shieldId) as BuffShield;
        if (sh != null)
            sh.SetHp((int)(owner.maxHp * skillCfg.Strength2));

        owner.PlayerAnim(skillCfg.Action);
        return true;
    }
}