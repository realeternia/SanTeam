using System;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 华雄·狂暴：给自身挂"重,伤"两个通用 Buff ——
/// 重（BuffFrenzy）读 StrengthBuff1[0] 提升造成伤害，伤（BuffDamagedAddRate）读 StrengthBuff1[1] 加深受到伤害。
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

        var buffIds = GetSkillBuffIds();
        if (buffIds.Count == 0)
        {
            GameLog.Error($"SkillAidBerserk: 未配置Buff短名 技能id={id}");
            return false;
        }

        owner.PlayerAnim(skillCfg.Action);
        PlaySelfEffect();
        foreach (var buffId in buffIds)
            BuffManager.AddBuff(owner, owner, id, buffId, skillCfg.BuffTime);
        return true;
    }
}
