using System;
using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 张角·落雷（术）：主动辅助技能，引导期间在目标位置召唤雷电阵（法术场可视化），
/// 每秒对范围内敌人结算一次法术伤害，持续 SummonTime 秒；引导可被打断，施法者死亡则结束。
/// 仿法正·明断，由 SkillManager.CheckAidSkill 自动循环施放。
/// </summary>
public class SkillCastingAidHitRegion : Skill
{
    public SkillCastingAidHitRegion(int id, Chess unit) : base(id, unit)
    {
    }

    public override bool CheckAidSkill()
    {
        var target = owner.targetChess;
        if (target == null || target.hp <= 0)
            return false;
        if (!WorldManager.Instance.CheckInRange(owner.transform.position, target.transform.position, skillCfg.Range))
            return false;
        if (!CheckBurst(target))
            return false;

        owner.PlayerAnim(skillCfg.Action);

        var targetPos = target.transform.position;

        var magicStub = SummonMagicField(targetPos, out var summonTime);

        //创建一个hitEffect
        EffectManager.PlayPosSkillEffect(magicStub, targetPos, skillCfg.Area, skillCfg.AreaEffect, summonTime);

        owner.StartCasting(id, DelayDamage(targetPos, summonTime));
        return true;
    }

    IEnumerator DelayDamage(Vector3 targetPos, float summonTime)
    {
        var term = (int)Math.Floor(summonTime / skillCfg.SummonHitInterval);
        for (int i = 0; i < term; i++)
        {
            // 被打断/死亡：castingSkillId 被 BreakCasting 置0，结束法阵
            if (owner == null || owner.hp <= 0 || owner.castingSkillId != id)
                yield break;

            // 引导期间每跳持续播放施法动作
            owner.PlayerAnim(skillCfg.Action);

            var unitsInRange = WorldManager.Instance.GetEnemyInRange(targetPos, skillCfg.Area, owner.side);
            if (unitsInRange.Count > 0)
            {
                WorldManager.Instance.RandomSelect(unitsInRange, skillCfg.TargetCount);
                var damage = GetSkillDamage(); // 固定系数 + 比例系数×关联属性
                foreach (var unit in unitsInRange)
                {
                    if (damage > 0)
                        unit.OnSkillDamaged(owner, skillId, damage);
                }
            }
            yield return new WaitForSeconds(skillCfg.SummonHitInterval);
        }
    }
}
