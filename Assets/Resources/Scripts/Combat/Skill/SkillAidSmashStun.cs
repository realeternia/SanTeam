using System;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 夏侯惇·歼灭：对单体造成魔法伤害并使其眩晕（"乱"，时长 bufftime）
/// </summary>
public class SkillAidSmashStun : Skill
{
    public SkillAidSmashStun(int id, Chess unit) : base(id, unit)
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

        var stunId = BuffConfig.GetConfigByNameS("乱").Id;
        BuffManager.AddBuff(target, owner, id, stunId, skillCfg.BuffTime);

        owner.PlayerAnim(skillCfg.Action);
        return true;
    }
}