using System;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 乐进·御斩：对单体造成魔法伤害并给自身挂减伤"硬"（时长 bufftime）
/// </summary>
public class SkillAidSlashDefend : Skill
{
    public SkillAidSlashDefend(int id, Chess unit) : base(id, unit)
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

        var armorId = BuffConfig.GetConfigByNameS("硬").Id;
        BuffManager.AddBuff(owner, owner, id, armorId, skillCfg.BuffTime);

        owner.PlayerAnim(skillCfg.Action);
        return true;
    }
}