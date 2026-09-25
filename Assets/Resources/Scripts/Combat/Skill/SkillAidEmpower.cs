using System;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 丁奉·奋威（武）：下一次普攻双倍伤害（倍率由"倍"Buff读取 skillCfg.Strength）。
/// 普攻触发后给自己附加倍击Buff（时长 skillCfg.BuffTime），由Buff结算倍率，本类不写死数值。
/// </summary>
public class SkillAidEmpower : Skill
{
    public SkillAidEmpower(int id, Chess unit) : base(id, unit)
    {
    }

    public override bool CheckAidSkill()
    {
        if (!CheckBurst(null))
            return false;

        owner.PlayerAnim(skillCfg.Action);

        BuffManager.AddBuff(owner, owner, id, BuffConfig.GetConfigByNameS("倍").Id, skillCfg.BuffTime);
        return true;
    }
}
