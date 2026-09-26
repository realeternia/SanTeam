using System;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 姜维·流光：对前方目标点范围（Area）内的若干敌人（TargetCount）各造成魔法伤害，类双激光/aoe
/// </summary>
public class SkillAidDualLaser : Skill
{
    public SkillAidDualLaser(int id, Chess unit) : base(id, unit)
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

        var list = WorldManager.Instance.GetUnitsInRange(target.transform.position, skillCfg.Area, owner.side, true);
        WorldManager.Instance.RandomSelect(list, skillCfg.TargetCount);
        foreach (var u in list)
            u.OnSkillDamaged(owner, id, GetSkillDamage());

        owner.PlayerAnim(skillCfg.Action);
        return true;
    }
}