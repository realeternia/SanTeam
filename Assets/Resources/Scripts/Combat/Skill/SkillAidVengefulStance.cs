using CommonConfig;
using UnityEngine;

/// <summary>
/// 张辽·威震：为自身附加威震状态（Buff "震" BuffVengefulStance）bufftime 秒。
/// 该状态下普攻按 Strength2[0] 概率眩晕目标，且普攻伤害类型转为真实伤害（无视护甲与护盾）。
/// Buff 短名由 SkillConfig.BuffId 以逗号分隔配置：第 1 个=威震状态Buff(震)，第 2 个=眩晕效果Buff(乱)。
/// </summary>
public class SkillAidVengefulStance : Skill
{
    public SkillAidVengefulStance(int id, Chess unit) : base(id, unit)
    {
    }

    public override bool CheckAidSkill()
    {
        var target = owner.targetChess;
        if (target == null || target.hp <= 0)
            return false;
        if (!CheckBurst(target))
            return false;

        // BuffId 逗号分隔：buffIds[0]=威震状态Buff(震)
        var buffIds = GetSkillBuffIds();
        if (buffIds.Count < 1)
        {
            GameLog.Error($"SkillAidVengefulStance: 未配置威震Buff BuffId={skillCfg.BuffId} 技能id={id}");
            return false;
        }

        owner.PlayerAnim(skillCfg.Action);
        PlaySelfEffect();

        BuffManager.AddBuff(owner, owner, id, buffIds[0], skillCfg.BuffTime);

        return true;
    }
}