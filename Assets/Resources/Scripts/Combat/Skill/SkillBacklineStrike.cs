using System;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 优先打后排的单体射击（ScriptName = "SkillBacklineStrike"）：在搜索范围内锁定敌方后排目标，
/// 通过飞弹 CreateSpellMissile 结算 GetSkillDamage() 技能伤害。
/// 搜索半径：Area &gt; 0 时用 Area，否则用 max(attackRange, Range)。
///
/// TargetType 标记：
/// · smart = 三层距离带降级选人（远段 70%~100% → 中段 50%~70% → 全局），每层取对应抗性最低者，
///           且优先只在英雄中选、无英雄才选全部单位（郭图·强击）
/// · extra = 伤害追加 Strength2[0] 比例，即 基础伤害 × (1 + Strength2[0])（文鸯·单骑）
/// 缺省（无 smart）= 直接取搜索范围内距离最远的敌人（文鸯·单骑）。
/// </summary>
public class SkillBacklineStrike : Skill
{
    public SkillBacklineStrike(int id, Chess unit) : base(id, unit)
    {
    }

    public override bool CheckAidSkill()
    {
        var searchRange = skillCfg.Area > 0f ? skillCfg.Area : Math.Max(owner.attackRange, skillCfg.Range);
        var enemies = WorldManager.Instance.GetEnemyInRange(owner.transform.position, searchRange, owner.side);
        if (enemies == null || enemies.Count == 0)
            return false;

        var target = HasSkillFlag("smart") ? SelectSmart(enemies, searchRange) : SelectFarthest(enemies);
        if (target == null || target.hp <= 0)
            return false;
        if (!CheckBurst(target))
            return false;

        owner.PlayerAnim(skillCfg.Action);
        PlaySelfEffect();

        var damage = GetSkillDamage();
        if (HasSkillFlag("extra"))
            damage += (int)(damage * GetStrength2(0));

        WorldManager.Instance.CreateSpellMissile(owner, target, owner.transform.position, id, damage, skillCfg.HitEffect);
        return true;
    }

    /// <summary>默认选人：搜索范围内距离自身最远的敌人</summary>
    private Chess SelectFarthest(List<Chess> enemies)
    {
        Chess farthest = null;
        var maxDistance = -1f;
        foreach (var enemy in enemies)
        {
            if (enemy == null || enemy.hp <= 0)
                continue;
            var distance = Vector3.Distance(owner.transform.position, enemy.transform.position);
            if (distance > maxDistance)
            {
                maxDistance = distance;
                farthest = enemy;
            }
        }
        return farthest;
    }

    /// <summary>smart 选人：先只在英雄中选，范围内无英雄再对全部单位按同样规则选</summary>
    private Chess SelectSmart(List<Chess> units, float searchRange)
    {
        var hero = PickLowestResist(units, searchRange, true);
        return hero != null ? hero : PickLowestResist(units, searchRange, false);
    }

    /// <summary>三层降级：优先远段、其次中段、最后全局；每层返回抗性最低者</summary>
    private Chess PickLowestResist(List<Chess> units, float searchRange, bool heroOnly)
    {
        var nearEdge = searchRange * 0.7f;
        var midEdge = searchRange * 0.5f;

        var target = LowestResistInBand(units, heroOnly, nearEdge, float.MaxValue);
        if (target != null)
            return target;
        target = LowestResistInBand(units, heroOnly, midEdge, nearEdge);
        return target != null ? target : LowestResistInBand(units, heroOnly, 0f, float.MaxValue);
    }

    /// <summary>在距离区间 [minDist, maxDist) 内挑选抗性最低的目标；区间内无候选返回 null</summary>
    private Chess LowestResistInBand(List<Chess> units, bool heroOnly, float minDist, float maxDist)
    {
        Chess best = null;
        var bestResist = int.MaxValue;
        foreach (var unit in units)
        {
            if (unit == null || unit.hp <= 0)
                continue;
            if (heroOnly && !unit.isHero)
                continue;

            var distance = Vector3.Distance(owner.transform.position, unit.transform.position);
            if (distance < minDist || distance >= maxDist)
                continue;

            var resist = GetTargetResist(unit);
            if (best == null || resist < bestResist)
            {
                best = unit;
                bestResist = resist;
            }
        }
        return best;
    }

    /// <summary>按技能伤害类型取目标抗性：物理取护甲、法术取魔抗；真实伤害无视抗性，返回 0（无偏好）</summary>
    private int GetTargetResist(Chess target)
    {
        if (skillCfg.DamageType == CombatConst.DamageTypeMagic)
            return target.magicRes;
        if (skillCfg.DamageType == CombatConst.DamageTypeAttack)
            return target.armor;
        return 0;
    }
}
