using System.Linq;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 荀彧·兵精：每次主动释放对一名未祝福士兵大祝福(优先近战)：回复全部生命 + 攻击+X%(Strength)、
/// 护甲+StrengthInt、魔抗+Strength2 大幅提升；每名士兵整场合仅祝福一次(BlessedByXunYu 标记)。
/// </summary>
public class SkillSoldierBless : Skill
{
    public SkillSoldierBless(int id, Chess unit) : base(id, unit)
    {
    }

    public override bool CheckAidSkill()
    {
        if (!CheckBurst(null))
            return false;
        owner.PlayerAnim(skillCfg.Action);

        var alive = WorldManager.Instance.GetUnitsMySide(owner.side)
            .Where(x => !x.isHero && x.hp > 0 && !x.blessedByXunYu).ToList();
        var melee = alive.Where(x => x.attackRange < CombatConst.MeleeRange).ToList();
        var target = melee.Count > 0 ? melee[0] : (alive.Count > 0 ? alive[0] : null);
        if (target != null)
        {
            target.blessedByXunYu = true;
            if (target.hp < target.maxHp)
                owner.HealTarget(target, skillId, target.maxHp - target.hp, false);
            target.atk += (int)(target.atk * skillCfg.Strength);
            target.armor += skillCfg.StrengthInt;
            target.magicRes += (int)skillCfg.Strength2;
            EffectManager.PlaySkillEffect(target, skillCfg.HitEffect);
        }
        return true;
    }
}