using CommonConfig;
using UnityEngine;

/// <summary>
/// 马腾·腾驾：降低目标攻击并提升自身攻击（"慑"减攻读 Strength2，"攻"加攻读 Strength）
/// </summary>
public class SkillAidAtkDrain : Skill
{
    public SkillAidAtkDrain(int id, Chess unit) : base(id, unit)
    {
    }

    /// <summary>
    /// 腾驾：降低目标攻击、提升自身攻击
    /// </summary>
    public override bool CheckAidSkill()
    {
        var target = owner.targetChess;
        if (target == null || target.hp <= 0)
            return false;
        if (!WorldManager.Instance.CheckInRange(owner.transform.position, target.transform.position, skillCfg.Range))
            return false;
        if (!CheckBurst(target))
            return false;

        owner.PlayerAnim(skillCfg.Action);
        BuffManager.AddBuff(target, owner, id, BuffConfig.GetConfigByNameS("慑").Id, skillCfg.BuffTime);
        BuffManager.AddBuff(owner, owner, id, BuffConfig.GetConfigByNameS("攻").Id, skillCfg.BuffTime);
        return true;
    }
}
