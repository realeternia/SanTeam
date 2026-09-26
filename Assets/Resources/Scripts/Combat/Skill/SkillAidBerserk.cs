using System;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 华雄·狂暴：给自身挂双刃"狂"（造成的伤害提升，但受到伤害也提升），增幅数值由 Buff 读取 strength2
/// </summary>
public class SkillAidBerserk : Skill
{
    public SkillAidBerserk(int id, Chess unit) : base(id, unit)
    {
    }

    public override bool CheckAidSkill()
    {
        if (!CheckBurst(null))
            return false;

        var frenzyId = BuffConfig.GetConfigByNameS("狂").Id;
        BuffManager.AddBuff(owner, owner, id, frenzyId, skillCfg.BuffTime);

        owner.PlayerAnim(skillCfg.Action);
        return true;
    }
}