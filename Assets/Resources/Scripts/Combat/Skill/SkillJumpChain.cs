using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 通用"跳跃链"技能（ScriptName = "SkillJumpChain"）：医疗波 / 连锁闪电式的连续跳跃结算。
///
/// 伤害模式（缺省）：首跳取当前索敌目标（须在 Range 内），随后每跳在 Area 内寻找最近的、尚未命中的敌方单位，
/// 共 TargetCount 跳；每跳造成 GetSkillDamage() 并附加 BuffId 减益，伤害按 0.85 逐跳递减（法正·明断）。
///
/// 治疗模式（TargetType 含 heal）：首跳选 Area 内生命比例最低的友方英雄，随后每跳寻找最近的、尚未治疗的友方英雄，
/// 共 TargetCount 跳；每跳按 GetSkillHeal() 治疗，治疗量按 0.85 逐跳递减（张宝·妖疗）。
///
/// TargetType 标记：
/// · heal = 治疗型跳跃（友军英雄）；缺省为伤害型（敌方单位）
/// </summary>
public class SkillJumpChain : Skill
{
    /// <summary>每跳相对上一跳的衰减系数（两种模式一致）</summary>
    private const float JumpDecay = 0.85f;
    /// <summary>伤害跳间隔(秒)</summary>
    private const float DamageInterval = 0.3f;
    /// <summary>治疗跳间隔(秒)</summary>
    private const float HealInterval = 0.2f;

    public SkillJumpChain(int id, Chess unit) : base(id, unit)
    {
    }

    public override bool CheckAidSkill()
    {
        return HasSkillFlag("heal") ? StartHealChain() : StartDamageChain();
    }

    // ---------- 治疗链 ----------

    private bool StartHealChain()
    {
        // 首跳目标：Area 内生命比例最低的友方英雄
        var units = WorldManager.Instance.GetMySideInRange(owner.transform.position, skillCfg.Area, owner.side)
            .FindAll(x => x.isHero && x.IsInFight() && x.hp < x.maxHp);

        Chess first = null;
        var lowest = float.MaxValue;
        foreach (var u in units)
        {
            if (u.HpRate < lowest)
            {
                lowest = u.HpRate;
                first = u;
            }
        }
        if (first == null)
            return false;

        if (!CheckBurst(null))
            return false;

        owner.PlayerAnim(skillCfg.Action);
        PlaySelfEffect();
        owner.StartCoroutine(HealTicks(first));
        return true;
    }

    private IEnumerator HealTicks(Chess first)
    {
        var healedList = new List<Chess>();
        var current = first;
        var currentHeal = GetSkillHeal();
        var jumps = Mathf.Max(1, skillCfg.TargetCount);

        for (var i = 0; i < jumps && current != null; i++)
        {
            if (owner == null || owner.hp <= 0)
                yield break;

            if (currentHeal > 0)
                owner.HealTarget(current, skillId, currentHeal, true);
            PlayAreaEffect(current.transform.position);
            EffectManager.PlaySkillEffect(current, skillCfg.HitEffect);
            GameLog.Debug($"妖疗 技能id={id} 等级={Level} 第{i + 1}跳 治疗={currentHeal} 目标={current.heroId}");
            healedList.Add(current);

            // 下一跳：Area 内最近的、尚未被治疗的友方英雄
            current = FindNextHealTarget(current, healedList);
            currentHeal = Mathf.RoundToInt(currentHeal * JumpDecay);

            yield return new WaitForSeconds(HealInterval);
        }
    }

    // 在 from 周围 Area 内找最近的、未治疗过的友方英雄（不给士兵）
    private Chess FindNextHealTarget(Chess from, List<Chess> healedList)
    {
        var units = WorldManager.Instance.GetMySideInRange(from.transform.position, skillCfg.Area, owner.side)
            .FindAll(x => x.isHero && x.hp < x.maxHp && !healedList.Contains(x));

        Chess nearest = null;
        var minDist = float.MaxValue;
        foreach (var u in units)
        {
            var dist = WorldManager.Instance.GetRange(from.transform.position, u.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = u;
            }
        }
        return nearest;
    }

    // ---------- 伤害链 ----------

    private bool StartDamageChain()
    {
        // 首跳目标：当前索敌目标，须在射程(Range)内
        var first = owner.targetChess;
        if (first == null || first.hp <= 0)
            return false;
        if (!WorldManager.Instance.CheckInRange(owner.transform.position, first.transform.position, skillCfg.Range))
            return false;
        if (!CheckBurst(first))
            return false;

        // 减益 buff 由 SkillConfig.BuffId 指定，不在战斗代码硬编码 BuffId
        var debuffCfg = BuffConfig.GetConfigByNameS(skillCfg.BuffId);
        if (debuffCfg == null)
        {
            GameLog.Error($"SkillJumpChain: 未找到减益Buff短名：{skillCfg.BuffId} 技能id={id}");
            return false;
        }

        owner.PlayerAnim(skillCfg.Action);
        PlaySelfEffect();
        owner.StartCoroutine(DamageTicks(first, debuffCfg.Id));
        return true;
    }

    private IEnumerator DamageTicks(Chess first, int debuffId)
    {
        var hitList = new List<Chess>();
        var current = first;
        var currentDamage = GetSkillDamage();
        var jumps = Mathf.Max(1, skillCfg.TargetCount);

        for (var i = 0; i < jumps && current != null; i++)
        {
            if (owner == null || owner.hp <= 0)
                yield break;
            // 目标在跳跃间隙被他人击杀：停止后续跳跃
            if (current.hp <= 0)
                break;

            // 先取位置：目标可能在本次伤害后阵亡，下一跳仍以其原位置为中心
            var pos = current.transform.position;

            if (currentDamage > 0)
                current.OnSkillDamaged(owner, skillId, currentDamage);
            BuffManager.AddBuff(current, owner, id, debuffId, skillCfg.BuffTime);

            if (!string.IsNullOrEmpty(skillCfg.HitEffect))
                EffectManager.PlaySkillEffect(current, skillCfg.HitEffect);
            GameLog.Debug($"明断 技能id={id} 等级={Level} 第{i + 1}跳 伤害={currentDamage} 目标={current.heroId}");
            hitList.Add(current);

            // 下一跳：Area 内最近的、尚未被命中的敌方单位
            current = FindNextDamageTarget(pos, hitList);
            currentDamage = Mathf.RoundToInt(currentDamage * JumpDecay);

            yield return new WaitForSeconds(DamageInterval);
        }
    }

    // 在 fromPos 周围 Area 内找最近的、未命中过的敌方单位
    private Chess FindNextDamageTarget(Vector3 fromPos, List<Chess> hitList)
    {
        var units = WorldManager.Instance.GetEnemyInRange(fromPos, skillCfg.Area, owner.side)
            .FindAll(x => x.hp > 0 && !hitList.Contains(x));

        Chess nearest = null;
        var minDist = float.MaxValue;
        foreach (var u in units)
        {
            var dist = WorldManager.Instance.GetRange(fromPos, u.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = u;
            }
        }
        return nearest;
    }
}
