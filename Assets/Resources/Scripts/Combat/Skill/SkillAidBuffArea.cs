using System;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 群体增益：给范围内最多 TargetCount 名未带此 buff 的友军附加 skillCfg.BuffId 对应的正面 buff。
/// 用于大乔·国色（群体攻击力提升，buff=攻）。通过 Skill.CheckAidSkill 自动循环施放。
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

        var buffId = BuffConfig.GetConfigByNameS(skillCfg.BuffId).Id;
        var units = WorldManager.Instance.GetUnitsInRange(owner.transform.position, skillCfg.Range, owner.side, false)
            .FindAll(x => x.IsInFight() && !x.HasBuff(buffId));

        if (units.Count == 0)
            return false;

        owner.PlayerAnim(skillCfg.Action);

        WorldManager.Instance.RandomSelect(units, skillCfg.TargetCount);

        foreach (var unit in units)
        {
            BuffManager.AddBuff(unit, owner, id, buffId, skillCfg.BuffTime);
            EffectManager.PlaySkillEffect(unit, skillCfg.HitEffect);
        }
        return true;
    }
}