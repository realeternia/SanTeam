using CommonConfig;
using UnityEngine;

/// <summary>
/// 文丑·陷阵怒吼：对自身 /area 范围内敌人各造成 /strength 法术伤害，并额外造成其最大生命 /strength2 的伤害，
/// 总伤 = /strength法术伤 + 目标最大生命 × /strength2。不附加任何 buff。
/// </summary>
public class SkillAidColossalSlam : Skill
{
    public SkillAidColossalSlam(int id, Chess unit) : base(id, unit)
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

        var baseDamage = (int)(skillCfg.Strength * (100 + owner.GetAttr("ap")) / 100);
        var units = WorldManager.Instance.GetUnitsInRange(owner.transform.position, skillCfg.Area, owner.side, true);
        foreach (var u in units)
        {
            var total = baseDamage + (int)(skillCfg.Strength2 * u.maxHp);
            if (total > 0)
                u.OnSkillDamaged(owner, id, total);
        }

        return true;
    }
}