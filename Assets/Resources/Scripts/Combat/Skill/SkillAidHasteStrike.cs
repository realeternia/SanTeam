using System;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 曹彰·疾攻：对单体造成魔法伤害并给自身挂攻移速"翼"（时长 bufftime）
/// </summary>
public class SkillAidHasteStrike : Skill
{
    public SkillAidHasteStrike(int id, Chess unit) : base(id, unit)
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

        var hasteId = BuffConfig.GetConfigByNameS("翼").Id;
        BuffManager.AddBuff(owner, owner, id, hasteId, skillCfg.BuffTime);

        owner.PlayerAnim(skillCfg.Action);
        return true;
    }
}