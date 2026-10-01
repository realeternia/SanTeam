using CommonConfig;
using UnityEngine;

/// <summary>
/// 文丑·陷阵怒吼：对自身 /area 范围内敌人各造成 /damagestrength 法术伤害（走统一公式 GetSkillDamage），
/// 并额外造成其最大生命 /strength2-1% 的伤害，总伤 = /damagestrength法术伤 + 目标最大生命 × /strength2-1%。不附加任何 buff。
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
        PlayAreaEffect(owner.transform.position);

        var baseDamage = GetSkillDamage();
        var units = WorldManager.Instance.GetUnitsInRange(owner.transform.position, skillCfg.Area, owner.side, true);
        foreach (var u in units)
        {
            var total = baseDamage + (int)(skillCfg.Strength2[0] * u.maxHp);
            if (total > 0)
                u.OnSkillDamaged(owner, id, total);
        }

        return true;
    }
}