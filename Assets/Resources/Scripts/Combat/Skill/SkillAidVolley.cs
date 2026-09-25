using CommonConfig;
using UnityEngine;

/// <summary>
/// 甘宁·连环：多重箭+闪避提升（"箭"=多重箭，"闪"=闪避Buff）
/// </summary>
public class SkillAidVolley : Skill
{
    public SkillAidVolley(int id, Chess unit) : base(id, unit)
    {
    }

    /// <summary>
    /// 连环：给自身挂多重箭与闪避Buff
    /// </summary>
    public override bool CheckAidSkill()
    {
        if (!CheckBurst(null))
            return false;

        owner.PlayerAnim(skillCfg.Action);
        BuffManager.AddBuff(owner, owner, id, BuffConfig.GetConfigByNameS("箭").Id, skillCfg.BuffTime);
        BuffManager.AddBuff(owner, owner, id, BuffConfig.GetConfigByNameS("闪").Id, skillCfg.BuffTime);
        return true;
    }
}
