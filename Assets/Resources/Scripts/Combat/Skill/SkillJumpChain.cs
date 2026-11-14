using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 通用"跳跃链"技能（ScriptName = "SkillJumpChain"）：医疗波 / 连锁闪电式的连续跳跃结算。
///
/// 伤害模式（缺省）：首跳取当前索敌目标（须在 Range 内），随后每跳在 Area 内寻找最近的、尚未命中的敌方单位，
/// 共 TargetCount 跳；每跳造成 GetSkillDamage() 并附加 BuffId 减益，伤害按 Strength2[jumpDecay]（默认 0.85）逐跳递减（法正·明断）。
///
/// 治疗模式（TargetType 含 heal）：首跳选 Area 内生命比例最低的友方英雄，随后每跳寻找最近的、尚未治疗的友方英雄，
/// 共 TargetCount 跳；每跳按 GetSkillHeal() 治疗，治疗量按 Strength2[jumpDecay]（默认 0.85）逐跳递减（张宝·妖疗）。
///
/// TargetType 标记：
/// · heal = 治疗型跳跃（友军英雄）；缺省为伤害型（敌方单位）
/// </summary>
public class SkillJumpChain : Skill
{
    /// <summary>伤害跳间隔(秒)</summary>
    private const float DamageInterval = 0.3f;
    /// <summary>治疗跳间隔(秒)</summary>
    private const float HealInterval = 0.2f;

    public SkillJumpChain(int id, Chess unit) : base(id, unit)
    {
    }

    /// <summary>每跳相对上一跳的衰减系数：模板默认 0.85，组件可覆盖（无衰减=1 / 衰减中=0.7 / 衰减大=0.5）</summary>
    private float GetJumpDecay()
    {
        return Mathf.Clamp(GetStr2(CombatConst.JumpChainDecayKey, CombatConst.JumpChainDecay), 0f, 1f);
    }

    public override bool CheckAidSkill()
    {
        return HasSkillFlag("heal") ? StartHealChain() : StartDamageChain();
    }

    // ---------- 治疗链 ----------

    private bool StartHealChain()
    {
        // 首跳目标：按 SelectOnly / SelectFirst 选出（典型 ally;hero;notFullHp + lowestHp）
        var picked = SelectTargets(owner.transform.position, skillCfg.Area, true, 1);
        var first = picked.Count > 0 ? picked[0] : null;
        if (first == null)
            return false;

        if (!CheckBurst(null))
            return false;

        owner.PlayerAnim(skillCfg.Action);
        PlaySelfEffect();
        owner.StartCoroutine(HealTicks(first, GetJumpDecay()));
        return true;
    }

    private IEnumerator HealTicks(Chess first, float decay)
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

            // 下一跳：按 SelectOnly / SelectFirst 在 Area 内选（典型 ally;hero;notFullHp + lowestHp），跳过已治疗过的
            current = FindNextTarget(current.transform.position, healedList);
            currentHeal = Mathf.RoundToInt(currentHeal * decay);

            yield return new WaitForSeconds(HealInterval);
        }
    }

    // 在 from 周围 Area 内按选人规则找下一个目标（跳过已结算过的单位）
    private Chess FindNextTarget(Vector3 from, List<Chess> skipList)
    {
        var defaultAlly = HasSkillFlag("heal");
        var picked = SelectTargets(from, skillCfg.Area, defaultAlly, 1, null, x => !skipList.Contains(x));
        return picked.Count > 0 ? picked[0] : null;
    }

    // ---------- 伤害链 ----------

    private bool StartDamageChain()
    {
        // 首跳目标：当前索敌目标，须在射程(Range)内
        var first = owner.targetChess;
        if (first == null || first.hp <= 0)
            return false;
        if (!WorldManager.Instance.CheckInRange(owner, first, skillCfg.Range))
            return false;
        if (!CheckBurst(first))
            return false;

        // 减益 buff 由 SkillConfig.BuffId 指定（留空表示不带减益），不在战斗代码硬编码 BuffId
        var debuffId = 0;
        if (!string.IsNullOrEmpty(skillCfg.BuffId))
        {
            var debuffCfg = BuffConfig.GetConfigByNameS(skillCfg.BuffId);
            if (debuffCfg == null)
            {
                GameLog.Error($"SkillJumpChain: 未找到减益Buff短名：{skillCfg.BuffId} 技能id={id}");
                return false;
            }
            debuffId = debuffCfg.Id;
        }

        owner.PlayerAnim(skillCfg.Action);
        PlaySelfEffect();
        owner.StartCoroutine(DamageTicks(first, debuffId, GetJumpDecay()));
        return true;
    }

    private IEnumerator DamageTicks(Chess first, int debuffId, float decay)
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
            // debuffId=0 表示本技能未配置减益，跳过挂载
            if (debuffId > 0)
                BuffManager.AddBuff(current, owner, id, debuffId, skillCfg.BuffTime);

            if (!string.IsNullOrEmpty(skillCfg.HitEffect))
                EffectManager.PlaySkillEffect(current, skillCfg.HitEffect);
            GameLog.Debug($"明断 技能id={id} 等级={Level} 第{i + 1}跳 伤害={currentDamage} 目标={current.heroId}");
            hitList.Add(current);

            // 下一跳：按 SelectOnly / SelectFirst 在 Area 内选（典型 enemy + nearest），跳过已命中过的
            current = FindNextTarget(pos, hitList);
            currentDamage = Mathf.RoundToInt(currentDamage * decay);

            yield return new WaitForSeconds(DamageInterval);
        }
    }
}
