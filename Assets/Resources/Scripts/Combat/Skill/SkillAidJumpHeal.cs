using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 妖疗（ScriptName = "AidJumpHeal"）：辅助技能，医疗波式跳跃治疗（参考魔兽争霸3 暗影猎手/巫医的连锁治疗）。
/// 首跳选中 Range 内生命比例最低的友方英雄，随后每跳在 Area 内寻找最近的、尚未被治疗的友方英雄继续跳跃，
/// 共 TargetCount 跳；每跳治疗量按 JumpHealDecay 递减。治疗量走独立治疗公式 GetSkillHeal()。
/// 用于张宝：地公将军的太平妖术，符水在军中接力相传。
/// </summary>
public class SkillAidJumpHeal : Skill
{
    /// <summary>每跳治疗量相对上一跳的衰减系数（本技能专用，不进 CombatConst）</summary>
    private const float JumpHealDecay = 0.6f;
    /// <summary>每跳之间的间隔(秒)（本技能专用，不进 CombatConst）</summary>
    private const float JumpHealInterval = 0.2f;

    public SkillAidJumpHeal(int id, Chess unit) : base(id, unit)
    {
    }

    public override bool CheckAidSkill()
    {
        // 首跳目标：Range 内生命比例最低的友方英雄
        var units = WorldManager.Instance.GetUnitsInRange(owner.transform.position, skillCfg.Range, owner.side, false)
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
        owner.StartCoroutine(JumpHeal(first));
        return true;
    }

    private IEnumerator JumpHeal(Chess first)
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
            EffectManager.PlaySkillEffect(current, skillCfg.HitEffect);
            GameLog.Debug($"妖疗 技能id={id} 等级={Level} 第{i + 1}跳 治疗={currentHeal} 目标={current.heroId}");
            healedList.Add(current);

            // 下一跳：Area 内最近的、尚未被治疗的友方英雄
            current = FindNextHealTarget(current, healedList);
            currentHeal = Mathf.RoundToInt(currentHeal * JumpHealDecay);

            yield return new WaitForSeconds(JumpHealInterval);
        }
    }

    // 在 from 周围 Area 内找最近的、未治疗过的友方英雄（不给士兵）
    private Chess FindNextHealTarget(Chess from, List<Chess> healedList)
    {
        var units = WorldManager.Instance.GetUnitsInRange(from.transform.position, skillCfg.Area, owner.side, false)
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
}
