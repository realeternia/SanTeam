using System;
using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

public class SkillAidSuddenArrow : Skill
{
    public SkillAidSuddenArrow(int id, Chess unit) : base(id, unit)
    {
    }

    public override bool CheckAidSkill()
    {
        var searchRange = Math.Max(owner.attackRange, skillCfg.Range);
        var unitsInRange = WorldManager.Instance.GetEnemyInRange(owner.transform.position, searchRange, owner.side);
        unitsInRange.Remove(owner);

        if (unitsInRange.Count == 0)
            return false;

        if (!CheckBurst(null))
            return false;

        var targetUnit = SelectTarget(unitsInRange, searchRange);
        if (targetUnit == null)
            return false;

        owner.PlayerAnim(skillCfg.Action);
        
        var damage = GetSkillDamage();
        WorldManager.Instance.CreateSpellMissile(owner, targetUnit, owner.transform.position, id, damage, skillCfg.HitEffect);

        return true;
    }

    /// <summary>
    /// 分层选目标：前三层仅限英雄（远段70%~100%射程 → 中段50%~70%射程 → 全局，各层取抗性最低者）；
    /// 若范围内无英雄，则对全部敌方按同样三层规则选取。
    /// </summary>
    private Chess SelectTarget(List<Chess> units, float searchRange)
    {
        var hero = PickLowestResist(units, searchRange, true);
        if (hero != null)
            return hero;
        return PickLowestResist(units, searchRange, false);
    }

    /// <summary>
    /// 三层降级选取：优先远段、其次中段、最后全局；每层返回抗性（按伤害类型）最低者，该层无候选时进入下一层。
    /// </summary>
    private Chess PickLowestResist(List<Chess> units, float searchRange, bool heroOnly)
    {
        var nearEdge = searchRange * 0.7f;
        var midEdge = searchRange * 0.5f;

        // 第一层：远段 [70%, 最大]
        var t = LowestResistInBand(units, heroOnly, nearEdge, float.MaxValue);
        if (t != null)
            return t;

        // 第二层：中段 [50%, 70%)
        t = LowestResistInBand(units, heroOnly, midEdge, nearEdge);
        if (t != null)
            return t;

        // 第三层：全局
        return LowestResistInBand(units, heroOnly, 0f, float.MaxValue);
    }

    /// <summary>
    /// 在距离区间 [minDist, maxDist) 内挑选抗性最低的目标；heroOnly=true 时仅考虑英雄。区间内无候选返回 null。
    /// </summary>
    private Chess LowestResistInBand(List<Chess> units, bool heroOnly, float minDist, float maxDist)
    {
        Chess best = null;
        var bestResist = int.MaxValue;
        foreach (var u in units)
        {
            if (u == null || u.hp <= 0)
                continue;
            if (heroOnly && !u.isHero)
                continue;

            var dist = Vector3.Distance(owner.transform.position, u.transform.position);
            if (dist < minDist || dist >= maxDist)
                continue;

            var resist = GetTargetResist(u);
            if (best == null || resist < bestResist)
            {
                best = u;
                bestResist = resist;
            }
        }
        return best;
    }

    /// <summary>
    /// 按技能伤害类型取目标的对应抗性：物理取护甲、法术取魔抗；真实伤害无视抗性，返回 0（无偏好）。
    /// </summary>
    private int GetTargetResist(Chess target)
    {
        if (skillCfg.DamageType == CombatConst.DamageTypeMagic)
            return target.magicRes;
        if (skillCfg.DamageType == CombatConst.DamageTypeAttack)
            return target.armor;
        return 0;
    }

}
