using CommonConfig;
using UnityEngine;

/// <summary>
/// 张绣·枪出如龙：对自身 /area 范围内敌人各造成 /damagestrength 法术伤害，不附加任何 buff
/// </summary>
public class SkillAidSweepingAoe : Skill
{
    public SkillAidSweepingAoe(int id, Chess unit) : base(id, unit)
    {
    }

    public override bool CheckAidSkill()
    {
        var target = owner.targetChess;
        if (target == null || target.hp <= 0)
            return false;
        if (!CheckBurst(target))
            return false;

        owner.PlayerAnim(skillCfg.Action);

        var skillDamage = GetSkillDamage();
        var units = WorldManager.Instance.GetUnitsInRange(owner.transform.position, skillCfg.Area, owner.side, true);
        foreach (var u in units)
        {
            if (skillDamage > 0)
                u.OnSkillDamaged(owner, id, skillDamage);
        }

        return true;
    }
}