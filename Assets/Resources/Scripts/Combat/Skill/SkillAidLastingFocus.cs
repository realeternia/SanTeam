using CommonConfig;
using UnityEngine;

/// <summary>
/// 马岱·连射：每次释放永久叠加攻速（无需Buff，技能自身叠加，每释放一次加一次）
/// </summary>
public class SkillAidLastingFocus : Skill
{
    public SkillAidLastingFocus(int id, Chess unit) : base(id, unit)
    {
    }

    /// <summary>
    /// 触发连射，永久叠加攻速
    /// </summary>
    public override bool CheckAidSkill()
    {
        if (!CheckBurst(null))
            return false;

        owner.PlayerAnim(skillCfg.Action);
        owner.attackSpeedRate += skillCfg.Strength;
        return true;
    }
}
