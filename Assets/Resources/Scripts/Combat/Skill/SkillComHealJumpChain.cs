using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 通用"跳跃治疗链"技能（ScriptName = "SkillComHealJumpChain"）：医疗波式的连续跳跃治疗。
///
/// 首跳按 SelectOnly / SelectFirst 在 Area 内选（典型 ally;hero;notFullHp + lowestHp），
/// 随后每跳寻找最近的、尚未治疗过的友方英雄，共 TargetCount 跳；
/// 每跳按 GetSkillHeal() 治疗，治疗量按 Strength2[jumpDecay]（默认 0.85）逐跳递减（张宝·妖疗）。
/// 伤害弹射见 SkillComDamageJumpChain。
/// </summary>
public class SkillComHealJumpChain : SkillComHealSingle
{
    /// <summary>治疗跳间隔(秒)</summary>
    private const float HealInterval = 0.2f;

    public SkillComHealJumpChain(int id, Chess unit) : base(id, unit)
    {
    }

    /// <summary>每跳相对上一跳的衰减系数：模板默认 0.85，组件可覆盖（无衰减=1 / 衰减中=0.7 / 衰减大=0.5）</summary>
    private float GetJumpDecay()
    {
        return Mathf.Clamp(GetStr2(CombatConst.JumpChainDecayKey, CombatConst.JumpChainDecay), 0f, 1f);
    }

    public override bool CheckAidSkill()
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

            HealUnit(current, currentHeal, false);
            PlayAreaEffect(current.transform.position);
            GameLog.Debug($"妖疗 技能id={id} 等级={Level} 第{i + 1}跳 治疗={currentHeal} 目标={current.heroId}");
            healedList.Add(current);

            // 下一跳：按 SelectOnly / SelectFirst 在 Area 内选（典型 ally;hero;notFullHp + lowestHp），跳过已治疗过的
            current = FindNextTarget(current.transform.position, healedList);
            currentHeal = Mathf.RoundToInt(currentHeal * decay);

            yield return new WaitForSeconds(HealInterval);
        }
    }

    // 在 from 周围 Area 内按选人规则找下一个友方目标（跳过已结算过的单位）
    private Chess FindNextTarget(Vector3 from, List<Chess> skipList)
    {
        var picked = SelectTargets(from, skillCfg.Area, true, 1, null, x => !skipList.Contains(x));
        return picked.Count > 0 ? picked[0] : null;
    }
}
