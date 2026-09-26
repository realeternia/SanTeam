using System;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 颜良·斩杀：对单体造成魔法伤害，目标生命低于 30%（strengthInt）时伤害按 strength2 倍加重
/// </summary>
public class SkillAidExecute : Skill
{
    public SkillAidExecute(int id, Chess unit) : base(id, unit)
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

        if ((int)(target.HpRate * 100f) < skillCfg.StrengthInt)
            target.OnSkillDamaged(owner, id, (int)(GetSkillDamage() * skillCfg.Strength2));
        else
            target.OnSkillDamaged(owner, id, GetSkillDamage());

        owner.PlayerAnim(skillCfg.Action);
        return true;
    }
}