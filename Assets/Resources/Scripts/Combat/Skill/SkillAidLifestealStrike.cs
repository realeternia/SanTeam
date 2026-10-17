using CommonConfig;
using UnityEngine;

/// <summary>
/// 徐晃·长驱：短时间吸血+增伤，由两个通用 Buff 组合实现（SkillConfig.BuffId = "重,吸"）——
/// 重（BuffFrenzy）读 StrengthBuff1[0] 增伤，吸（BuffSuck）读 StrengthBuff1[1] 吸血。
/// </summary>
public class SkillAidLifestealStrike : Skill
{
    public SkillAidLifestealStrike(int id, Chess unit) : base(id, unit)
    {
    }

    /// <summary>
    /// 长驱：把配置里由逗号分隔的多个 Buff（增伤 + 吸血）全部挂到自身
    /// </summary>
    public override bool CheckAidSkill()
    {
        if (!CheckBurst(null))
            return false;

        var buffIds = GetSkillBuffIds();
        if (buffIds.Count == 0)
        {
            GameLog.Error($"SkillAidLifestealStrike: 未配置Buff短名 技能id={id}");
            return false;
        }

        owner.PlayerAnim(skillCfg.Action);
        PlaySelfEffect();
        foreach (var buffId in buffIds)
            BuffManager.AddBuff(owner, owner, id, buffId, skillCfg.BuffTime);
        return true;
    }
}
