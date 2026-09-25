using System;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 张郃·震碎（术）：范围AoE + 减速(缓)。
/// 普攻触发后对目标周围 skillCfg.Area 范围内至多 TargetCount 个敌人造成技能伤害，
/// 并给每个目标附加减速Buff（时长 skillCfg.BuffTime）。
/// </summary>
public class SkillAidQuake : Skill
{
    public SkillAidQuake(int id, Chess unit) : base(id, unit)
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

        var units = WorldManager.Instance.GetUnitsInRange(target.transform.position, skillCfg.Area, owner.side, true);
        WorldManager.Instance.RandomSelect(units, skillCfg.TargetCount);

        var huanBuffId = BuffConfig.GetConfigByNameS("缓").Id; // 减速Buff
        var skillDamage = GetSkillDamage();
        foreach (var u in units)
        {
            if (skillDamage > 0)
                u.OnSkillDamaged(owner, skillId, skillDamage);
            BuffManager.AddBuff(u, owner, id, huanBuffId, skillCfg.BuffTime);
        }
        return true;
    }
}
