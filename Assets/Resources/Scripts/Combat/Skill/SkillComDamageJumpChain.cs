using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 通用"跳跃伤害链"技能（ScriptName = "SkillComDamageJumpChain"）：连锁闪电式的连续跳跃伤害。
///
/// 首跳取当前索敌目标（须在 Range 内），随后每跳在 Area 内寻找最近的、尚未命中的敌方单位，
/// 共 TargetCount 跳；每跳造成 GetSkillDamage() 并附加 BuffId 减益，
/// 伤害按 Strength2[jumpDecay]（默认 0.85）逐跳递减（法正·明断）。
/// 每跳伤害走基类统一伤害路径（CalcDamage），斩杀/护盾加成/残血增伤/兵种克制/最大生命追加/燃蓝/驱散等组件同样生效。
/// 治疗弹射见 SkillComHealJumpChain。
/// </summary>
public class SkillComDamageJumpChain : SkillComDamageSingle
{
    /// <summary>伤害跳间隔(秒)</summary>
    private const float DamageInterval = 0.3f;

    public SkillComDamageJumpChain(int id, Chess unit) : base(id, unit)
    {
    }

    /// <summary>每跳相对上一跳的衰减系数：模板默认 0.85，组件可覆盖（无衰减=1 / 衰减中=0.7 / 衰减大=0.5）</summary>
    private float GetJumpDecay()
    {
        return Mathf.Clamp(GetStr2(CombatConst.JumpChainDecayKey, CombatConst.JumpChainDecay), 0f, 1f);
    }

    public override bool CheckAidSkill()
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
                GameLog.Error($"SkillComDamageJumpChain: 未找到减益Buff短名：{skillCfg.BuffId} 技能id={id}");
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

            // 统一伤害路径：本跳衰减后的基础值经 CalcDamage 修正（斩杀/护盾加成/残血增伤/兵种克制/最大生命追加等）
            // → 双击 / 直伤/弹道 + 命中特效 → 破盾 / 燃蓝 / 击退 / 驱散
            if (currentDamage > 0)
                ResolveDamage(current, currentDamage);
            // debuffId=0 表示本技能未配置减益，跳过挂载
            if (debuffId > 0)
                BuffManager.AddBuff(current, owner, id, debuffId, skillCfg.BuffTime);
            GameLog.Debug($"明断 技能id={id} 等级={Level} 第{i + 1}跳 伤害={currentDamage} 目标={current.heroId}");
            hitList.Add(current);

            // 下一跳：按 SelectOnly / SelectFirst 在 Area 内选（典型 enemy + nearest），跳过已命中过的
            current = FindNextTarget(pos, hitList);
            currentDamage = Mathf.RoundToInt(currentDamage * decay);

            yield return new WaitForSeconds(DamageInterval);
        }
    }

    // 在 from 周围 Area 内按选人规则找下一个敌方目标（跳过已结算过的单位）
    private Chess FindNextTarget(Vector3 from, List<Chess> skipList)
    {
        var picked = SelectTargets(from, skillCfg.Area, false, 1, null, x => !skipList.Contains(x));
        return picked.Count > 0 ? picked[0] : null;
    }
}
