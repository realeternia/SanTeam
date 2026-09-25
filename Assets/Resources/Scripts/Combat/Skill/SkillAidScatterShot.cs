using CommonConfig;
using UnityEngine;

/// <summary>
/// 黄忠·乱射（ScriptName = "BowHuangZhong"）：
/// 区域多目标箭雨，对目标周围(Area)范围内敌人附加暴击加成伤害。
/// 每目标伤害 = 基础技能伤害 × (1 + Strength2)。
/// </summary>
public class SkillAidScatterShot : Skill
{
    public SkillAidScatterShot(int id, Chess unit) : base(id, unit)
    {
    }

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

        var targets = WorldManager.Instance.GetUnitsInRange(target.transform.position, skillCfg.Area, owner.side, true);
        WorldManager.Instance.RandomSelect(targets, skillCfg.TargetCount);

        foreach (var t in targets)
        {
            var dmg = GetSkillDamage() + (int)(GetSkillDamage() * skillCfg.Strength2);
            WorldManager.Instance.CreateSpellMissile(owner, t, owner.transform.position, id, dmg, owner.hitEffect);
        }
        return true;
    }
}
