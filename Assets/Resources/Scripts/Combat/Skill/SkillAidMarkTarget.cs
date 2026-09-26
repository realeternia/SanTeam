using System;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 朱桓·标记：对单体造成魔法伤害并给目标挂增伤"伤"（target 受击加深，时长 bufftime）
/// </summary>
public class SkillAidMarkTarget : Skill
{
    public SkillAidMarkTarget(int id, Chess unit) : base(id, unit)
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

        var markId = BuffConfig.GetConfigByNameS("伤").Id;
        BuffManager.AddBuff(target, owner, id, markId, skillCfg.BuffTime);

        owner.PlayerAnim(skillCfg.Action);
        return true;
    }
}