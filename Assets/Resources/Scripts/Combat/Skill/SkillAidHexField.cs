using System;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 徐庶·困敌（术）：范围AoE + 减攻(慑) + 减疗(疫)。
/// 普攻触发后对目标周围 skillCfg.Area 范围内至多 TargetCount 个敌人造成技能伤害，
/// 并给每个目标附加减攻与减疗Buff（时长 skillCfg.BuffTime），随后播放命中特效。
/// </summary>
public class SkillAidHexField : Skill
{
    public SkillAidHexField(int id, Chess unit) : base(id, unit)
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

        var shenBuffId = BuffConfig.GetConfigByNameS("慑").Id; // 减攻Buff
        var yiBuffId = BuffConfig.GetConfigByNameS("疫").Id;   // 减疗Buff
        var skillDamage = GetSkillDamage();
        foreach (var u in units)
        {
            if (skillDamage > 0)
                u.OnSkillDamaged(owner, skillId, skillDamage);
            BuffManager.AddBuff(u, owner, id, shenBuffId, skillCfg.BuffTime);
            BuffManager.AddBuff(u, owner, id, yiBuffId, skillCfg.BuffTime);
        }

        EffectManager.PlaySkillEffect(target, skillCfg.HitEffect);
        return true;
    }
}
