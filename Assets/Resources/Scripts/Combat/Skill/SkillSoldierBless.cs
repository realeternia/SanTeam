using System.Linq;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 荀彧·兵精：每次主动释放对一名未祝福士兵大祝福(优先近战)：回复全部生命 + 攻击+X%(Strength2[0])、
/// 护甲+Strength2[2]、魔抗+Strength2[1] 大幅提升；每名士兵整场合仅祝福一次(BlessedByXunYu 标记)。
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

        // 祝福优先同侧(battleSide)士兵，无同侧目标再退到跨侧友军(alley)士兵
        var pool = WorldManager.Instance.GetMySideInRange(owner.transform.position, 0f, owner.side)
            .Where(x => !x.isHero && x.hp > 0 && !x.blessedByXunYu).ToList();
        if (pool.Count == 0)
        {
            pool = WorldManager.Instance.GetUnitsInRangeAll(owner.transform.position, 0f)
                .Where(x => !x.isHero && x.hp > 0 && !x.blessedByXunYu
                    && !WorldManager.Instance.IsEnemy(x.side, owner.side)).ToList();
        }
        var melee = pool.Where(x => x.attackRange < CombatConst.MeleeRange).ToList();
        var target = melee.Count > 0 ? melee[0] : (pool.Count > 0 ? pool[0] : null);
        if (target != null)
        {
            target.blessedByXunYu = true;
            if (target.hp < target.maxHp)
                owner.HealTarget(target, skillId, target.maxHp - target.hp, false);
            target.atk += (int)(target.atk * skillCfg.Strength2[0]);
            target.armor += (int)skillCfg.Strength2[2];
            target.magicRes += (int)skillCfg.Strength2[1];
            EffectManager.PlaySkillEffect(target, skillCfg.HitEffect);
        }
        return true;
    }
}