using System;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 徐庶·困敌（术）：范围AoE + 减攻(慑) + 减疗(疫)。
/// 普攻触发后对目标周围 skillCfg.Area 范围内至多 TargetCount 个敌人造成技能伤害，
/// 并给每个目标附加减攻与减疗Buff（时长 skillCfg.BuffTime），随后播放命中特效。
/// Buff 短名由 SkillConfig.BuffId 以逗号分隔配置：第 1 个=减攻Buff，第 2 个=减疗Buff（如 "慑,疫"）。
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

        // BuffId 逗号分隔：buffIds[0]=减攻Buff(慑)，buffIds[1]=减疗Buff(疫)
        var buffIds = GetSkillBuffIds();
        if (buffIds.Count < 2)
        {
            GameLog.Error($"SkillAidHexField: BuffId 需配置减攻与减疗两个buff（如\"慑,疫\"）技能id={id}");
            return false;
        }
        int shenBuffId = buffIds[0]; // 减攻Buff
        int yiBuffId = buffIds[1];   // 减疗Buff

        owner.PlayerAnim(skillCfg.Action);
        PlaySelfEffect();

        var units = WorldManager.Instance.GetEnemyInRange(target.transform.position, skillCfg.Area, owner.side);
        WorldManager.Instance.RandomSelect(units, skillCfg.TargetCount);

        var skillDamage = GetSkillDamage();
        foreach (var u in units)
        {
            if (skillDamage > 0)
                u.OnSkillDamaged(owner, skillId, skillDamage);
            BuffManager.AddBuff(u, owner, id, shenBuffId, skillCfg.BuffTime);
            BuffManager.AddBuff(u, owner, id, yiBuffId, skillCfg.BuffTime);
        }

        PlayAreaEffect(target.transform.position);
        EffectManager.PlaySkillEffect(target, skillCfg.HitEffect);
        return true;
    }
}
