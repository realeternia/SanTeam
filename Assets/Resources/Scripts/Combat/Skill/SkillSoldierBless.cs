using System.Linq;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 荀彧·兵精：每次主动释放对一名未祝福士兵大祝福(优先近战)：回复全部生命 + 攻击+X%(Strength2[0])、
/// 护甲+Strength2[2]、魔抗+Strength2[1] 大幅提升；每名士兵整场合仅祝福一次(BlessedByXunYu 标记)。
/// 同时每次释放为自身永久成长（StrengthBuff1[0]=攻击、StrengthBuff1[1]=生命），
/// 使纯士兵辅助在无兵/单兵情况下也能逐步成长为可独立作战的单位。
/// </summary>
public class SkillSoldierBless : Skill
{
    /// <summary>自身成长槽位（StrengthBuff1 数组下标）</summary>
    private const int SelfAtkGrowIdx = 0;
    private const int SelfHpGrowIdx = 1;

    public SkillSoldierBless(int id, Chess unit) : base(id, unit)
    {
    }

    public override bool CheckAidSkill()
    {
        if (!CheckBurst(null))
            return false;
        owner.PlayerAnim(skillCfg.Action);

        // 自身永久成长：每次释放无条件叠加（即使无士兵可祝福也能成长）
        GrowSelf();

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

    // 自身永久成长：每次释放为自身叠加攻击与生命（无Buff，直接永久累加），数值取自 StrengthBuff1
    private void GrowSelf()
    {
        if (skillCfg.StrengthBuff1 == null || skillCfg.StrengthBuff1.Length < 2)
            return;
        JobLinkManager.ApplyAttr(owner, "atk", skillCfg.StrengthBuff1[SelfAtkGrowIdx]);
        JobLinkManager.ApplyAttr(owner, "hp", skillCfg.StrengthBuff1[SelfHpGrowIdx]);
    }
}