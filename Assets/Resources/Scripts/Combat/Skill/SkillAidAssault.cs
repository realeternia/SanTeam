using CommonConfig;
using UnityEngine;

/// <summary>
/// 鞠义·先锋：短时间射速提升（"快"Buff）
/// </summary>
public class SkillAidAssault : Skill
{
    public SkillAidAssault(int id, Chess unit) : base(id, unit)
    {
    }

    /// <summary>
    /// 先锋：给自身挂攻速提升Buff
    /// </summary>
    public override bool CheckAidSkill()
    {
        if (!CheckBurst(null))
            return false;

        owner.PlayerAnim(skillCfg.Action);
        BuffManager.AddBuff(owner, owner, id, BuffConfig.GetConfigByNameS("快").Id, skillCfg.BuffTime);
        return true;
    }
}
