using System;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 通用"自身增益"技能（ScriptName = "SkillAidSelfBuff"）：触发时给施法者自身挂 1~N 个通用 Buff，
/// Buff 短名由 SkillConfig.BuffId 配置（英文/中文逗号或分号分隔多个，如"快"、"倍"、"重,吸"、"重,伤"），
/// 各 Buff 的效果数值由其自身读取对应的 Strength2/StrengthBuff1，本类不写死数值；时长取 BuffTime。
/// 覆盖：丁奉·奋威("倍") / 王濬·疾击、凌统·迅射、鞠义·先锋("快") /
/// 华雄·狂暴("重,伤") / 徐晃·长驱("重,吸") / 徐盛·疑城、张梁·黄天("硬") / 张辽·威震("震")。
/// </summary>
public class SkillAidSelfBuff : Skill
{
    public SkillAidSelfBuff(int id, Chess unit) : base(id, unit)
    {
    }

    public override bool CheckAidSkill()
    {
        if (!CheckBurst(null))
            return false;

        var buffIds = GetSkillBuffIds();
        if (buffIds.Count == 0)
        {
            GameLog.Error($"SkillAidSelfBuff: 未配置Buff短名 技能id={id}");
            return false;
        }

        owner.PlayerAnim(skillCfg.Action);
        PlaySelfEffect();
        foreach (var buffId in buffIds)
            BuffManager.AddBuff(owner, owner, id, buffId, skillCfg.BuffTime);
        return true;
    }
}
