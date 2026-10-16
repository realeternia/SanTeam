using CommonConfig;
using UnityEngine;

/// <summary>
/// 文鸯·追袭：短时间吸血+增伤（"袭"BuffLifeStealAndDamage 读 StrengthBuff1[0] 增伤、StrengthBuff1[1] 吸血）
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
        PlaySelfEffect();
        BuffManager.AddBuff(owner, owner, id, BuffConfig.GetConfigByNameS(skillCfg.BuffId).Id, skillCfg.BuffTime);
        return true;
    }
}
