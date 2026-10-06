using System;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 群体增益 + 净化：给范围内最多 TargetCount 名未带此 buff 的友方单位附加 skillCfg.BuffId 对应的正面 buff，
/// 并驱散这些目标身上的负面 Buff（不可驱散的招牌 Buff 除外，见 BuffManager.DispelNegative）。
/// 用于左慈·天书（群体闪避 + 净化）。通过 Skill.CheckAidSkill 自动循环施放。
/// </summary>
public class SkillAidDispelBuffArea : Skill
{
    public SkillAidDispelBuffArea(int id, Chess unit) : base(id, unit)
    {
    }

    public override bool CheckAidSkill()
    {
        if (!CheckBurst(null))
            return false;

        var buffId = BuffConfig.GetConfigByNameS(skillCfg.BuffId).Id;
        var units = WorldManager.Instance.GetMySideInRange(owner.transform.position, skillCfg.Range, owner.side)
            .FindAll(x => x.IsInFight() && !x.HasBuff(buffId));

        if (units.Count == 0)
            return false;

        owner.PlayerAnim(skillCfg.Action);

        WorldManager.Instance.RandomSelect(units, skillCfg.TargetCount);

        foreach (var unit in units)
        {
            BuffManager.AddBuff(unit, owner, id, buffId, skillCfg.BuffTime);
            // 天书附带净化：驱散被施法友军的负面 Buff
            BuffManager.DispelNegative(unit);
            EffectManager.PlaySkillEffect(unit, skillCfg.HitEffect);
        }
        return true;
    }
}
