using System;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 鼓舞（小乔）：辅助技能，给范围内最多 TargetCount 名友军附加攻速+移速组合 buff。
/// 通过 Skill.CheckAidSkill 由 SkillManager 自动循环施放。
/// </summary>
public class SkillAidBuffHasteMoveSpeed : Skill
{
    public SkillAidBuffHasteMoveSpeed(int id, Chess unit) : base(id, unit)
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