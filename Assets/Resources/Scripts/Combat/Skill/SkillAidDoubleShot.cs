using System;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 韩当·双射：对目标点范围（Area）内若干个敌人（TargetCount）各造成魔法伤害
/// </summary>
public class SkillAidDoubleShot : Skill
{
    public SkillAidDoubleShot(int id, Chess unit) : base(id, unit)
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