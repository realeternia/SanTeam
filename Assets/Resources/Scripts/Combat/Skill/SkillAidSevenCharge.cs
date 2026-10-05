using System;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 李典·御阵：为自身及范围内最多 TargetCount 名友军附加护甲提升 buff（"甲"，护甲值由 BuffArmorAdd 读 skillCfg.StrengthBuff1[0]），持续 BuffTime 秒。
/// </summary>
public class SkillAidSevenCharge : Skill
{
    public SkillAidSevenCharge(int id, Chess unit) : base(id, unit)
    {
    }

    public override bool CheckAidSkill()
    {
        if (!CheckBurst(null))
            return false;

        var buffId = BuffConfig.GetConfigByNameS(skillCfg.BuffId).Id;
        var units = WorldManager.Instance.GetMySideInRange(owner.transform.position, skillCfg.Range, owner.side)
            .FindAll(x => x.IsInFight() && x != owner && !x.HasBuff(buffId));
        WorldManager.Instance.RandomSelect(units, skillCfg.TargetCount);
        units.Add(owner); // 自己必定获得

        owner.PlayerAnim(skillCfg.Action);
        foreach (var unit in units)
        {
            BuffManager.AddBuff(unit, owner, id, buffId, skillCfg.BuffTime);
            EffectManager.PlaySkillEffect(unit, skillCfg.HitEffect);
        }
        return true;
    }
}