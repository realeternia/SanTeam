using System;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 陈泰·换位：对单体造成魔法伤害，并与目标交换位置（扭转敌我站位）
/// </summary>
public class SkillAidSwapStrike : Skill
{
    public SkillAidSwapStrike(int id, Chess unit) : base(id, unit)
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

        target.OnSkillDamaged(owner, id, GetSkillDamage());

        // 与目标交换位置
        Vector3 tempPos = owner.transform.position;
        owner.transform.position = target.transform.position;
        target.transform.position = tempPos;

        owner.PlayerAnim(skillCfg.Action);
        return true;
    }
}