using System;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 群体增益：给范围内最多 TargetCount 名未带此 buff 的友军附加 skillCfg.BuffId 对应的正面 buff。
/// 支持多段组合（如奇才 "攻,甲,抗"），去重判断只看第一段。通过 Skill.CheckAidSkill 自动循环施放。
/// </summary>
public class SkillAidBuffArea : Skill
{
    public SkillAidBuffArea(int id, Chess unit) : base(id, unit)
    {
    }

    public override bool CheckAidSkill()
    {
        if (!CheckBurst(null))
            return false;

        var buffIds = GetSkillBuffIds();
        if (buffIds.Count == 0)
        {
            GameLog.Error($"SkillAidBuffArea: 未配置Buff短名 技能id={id}");
            return false;
        }

        var units = WorldManager.Instance.GetMySideInRange(owner.transform.position, skillCfg.Area, owner.side)
            .FindAll(x => x.IsInFight() && !x.HasBuff(buffIds[0]));

        if (units.Count == 0)
            return false;

        owner.PlayerAnim(skillCfg.Action);
        PlaySelfEffect();
        PlayAreaEffect(owner.transform.position);

        WorldManager.Instance.RandomSelect(units, skillCfg.TargetCount);

        foreach (var unit in units)
        {
            foreach (var buffId in buffIds)
                BuffManager.AddBuff(unit, owner, id, buffId, skillCfg.BuffTime);
            EffectManager.PlaySkillEffect(unit, skillCfg.HitEffect);
        }
        return true;
    }
}
