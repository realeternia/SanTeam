using CommonConfig;
using UnityEngine;

/// <summary>
/// 太史慈·连珠（ScriptName = "BowTaiShiCi"）：
/// 朝前方多发箭矢，距离越近伤害越高。
/// 每目标近度 near = max(0, 1 - dist/Range)，伤害 = 基础技能伤害 × (1 + Strength2 × near)。
/// </summary>
public class SkillAidBurstShot : Skill
{
    public SkillAidBurstShot(int id, Chess unit) : base(id, unit)
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

        var targets = WorldManager.Instance.GetUnitsInRange(owner.transform.position, skillCfg.Range, owner.side, true);
        WorldManager.Instance.RandomSelect(targets, skillCfg.TargetCount);

        foreach (var t in targets)
        {
            var dist = Vector3.Distance(owner.transform.position, t.transform.position);
            var near = Mathf.Max(0f, 1f - dist / skillCfg.Range);
            var dmg = GetSkillDamage() + (int)(GetSkillDamage() * skillCfg.Strength2 * near);
            WorldManager.Instance.CreateSpellMissile(owner, t, owner.transform.position, id, dmg, owner.hitEffect);
        }
        return true;
    }
}
