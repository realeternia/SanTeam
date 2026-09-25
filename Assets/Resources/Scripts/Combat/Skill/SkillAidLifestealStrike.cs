using CommonConfig;
using UnityEngine;

/// <summary>
/// 文鸯·追袭：短时间吸血+增伤（"袭"Buff，读自身 skillCfg.Strength/Strength2）
/// </summary>
public class SkillAidLifestealStrike : Skill
{
    public SkillAidLifestealStrike(int id, Chess unit) : base(id, unit)
    {
    }

    /// <summary>
    /// 追袭：给自身挂吸血+增伤Buff
    /// </summary>
    public override bool CheckAidSkill()
    {
        if (!CheckBurst(null))
            return false;

        owner.PlayerAnim(skillCfg.Action);
        BuffManager.AddBuff(owner, owner, id, BuffConfig.GetConfigByNameS("袭").Id, skillCfg.BuffTime);
        return true;
    }
}
