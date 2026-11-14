using CommonConfig;
using UnityEngine;

/// <summary>
/// 沮授·强弓（ScriptName = "SkillSoldierArcherBuff"）：
/// 对一名敌方单位施法，本侧每名远程士兵（attackRange &gt;= CombatConst.MeleeRange）与沮授自身
/// 各射出一枚法术箭飞向目标，每枚按 GetSkillDamage() 结算；远程兵越多总伤害越高。
/// </summary>
public class SkillSoldierArcherBuff : Skill
{
    public SkillSoldierArcherBuff(int id, Chess unit) : base(id, unit)
    {
    }

    public override bool CheckAidSkill()
    {
        if (!CheckBurst(null))
            return false;

        // 目标：按 SelectOnly / SelectFirst 选（典型 enemy + nearest）
        var picked = SelectTargets(owner.transform.position, skillCfg.Area, false, 1);
        var target = picked.Count > 0 ? picked[0] : null;
        if (target == null)
            return false;

        var damage = GetSkillDamage();
        if (damage <= 0)
            return false;

        owner.PlayerAnim(skillCfg.Action);
        PlaySelfEffect();

        // 每个远程士兵各射一箭
        foreach (var s in WorldManager.Instance.GetUnitsMySide(owner.side))
        {
            if (s.isHero || s.hp <= 0 || s.attackRange < CombatConst.MeleeRange)
                continue;
            WorldManager.Instance.CreateSpellMissile(owner, target, s.transform.position, id, damage, skillCfg.HitEffect);
        }
        // 沮授自身再射一箭
        WorldManager.Instance.CreateSpellMissile(owner, target, owner.transform.position, id, damage, skillCfg.HitEffect);

        return true;
    }
}
