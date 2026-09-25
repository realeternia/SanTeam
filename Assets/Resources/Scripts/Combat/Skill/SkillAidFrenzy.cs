using System;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 王濬·疾击（武）：短时间内射速大幅提升。
/// 普攻触发后给自己附加攻速提升Buff（"快"，时长 skillCfg.BuffTime），增幅数值由Buff读取 skillCfg.Strength。
/// </summary>
public class SkillAidFrenzy : Skill
{
    public SkillAidFrenzy(int id, Chess unit) : base(id, unit)
    {
    }

    public override bool CheckAidSkill()
    {
        if (!CheckBurst(null))
            return false;

        owner.PlayerAnim(skillCfg.Action);

        BuffManager.AddBuff(owner, owner, id, BuffConfig.GetConfigByNameS("快").Id, skillCfg.BuffTime);
        return true;
    }
}
