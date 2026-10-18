using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 明断（ScriptName = "SkillAidJumpDamage"）：攻击向跳跃伤害技能（医疗波/连锁闪电式）。
/// 首跳取当前索敌目标(owner.targetChess，须在 Range 射程内)，随后每跳在 Area 内寻找最近的、尚未被命中的敌方单位继续跳跃，
/// 共 TargetCount 跳；每跳对目标造成 GetSkillDamage() 伤害并附加 skillCfg.BuffId 指定的减益 buff，伤害按 JumpDamageDecay 逐跳递减。
/// 用于法正：明断。
/// </summary>
public class SkillAidJumpDamage : Skill
{
    /// <summary>每跳伤害相对上一跳的衰减系数（本技能专用，不进 CombatConst）</summary>
    private const float JumpDamageDecay = 0.85f;
    /// <summary>每跳之间的间隔(秒)（本技能专用，不进 CombatConst）</summary>
    private const float JumpInterval = 0.3f;

    public SkillAidJumpDamage(int id, Chess unit) : base(id, unit)
    {
    }

    public override bool CheckAidSkill()
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
            GameLog.Error($"SkillAidJumpDamage: 未找到减益Buff短名：{skillCfg.BuffId} 技能id={id}");
            return false;
        }

        owner.PlayerAnim(skillCfg.Action);
        PlaySelfEffect();
        owner.StartCoroutine(JumpDamage(first, debuffCfg.Id));
        return true;
    }

    private IEnumerator JumpDamage(Chess first, int debuffId)
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
            current = FindNextTarget(pos, hitList);
            currentDamage = Mathf.RoundToInt(currentDamage * JumpDamageDecay);

            yield return new WaitForSeconds(JumpInterval);
        }
    }

    // 在 fromPos 周围 Area 内找最近的、未命中过的敌方单位
    private Chess FindNextTarget(Vector3 fromPos, List<Chess> hitList)
    {
        var units = WorldManager.Instance.GetEnemyInRange(fromPos, skillCfg.Area, owner.side).FindAll(x => x.hp > 0 && !hitList.Contains(x));

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
