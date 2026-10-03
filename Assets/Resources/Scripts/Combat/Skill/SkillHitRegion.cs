using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

public class SkillHitRegion : Skill
{
    private Vector3 targetPos;
    public SkillHitRegion(int id, Chess unit) : base(id, unit)
    {
    }

    public override void OnAttack(Chess defender, int damage)
    {
        if (CheckBurst(defender))
        {
            owner.PlayerAnim(skillCfg.Action);

            targetPos = defender.transform.position;

            var magicStub = SummonMagicField(targetPos, out var summonTime);

            //创建一个hitEffect
            EffectManager.PlayPosSkillEffect(magicStub, targetPos, skillCfg.Area, skillCfg.AreaEffect, summonTime);

            owner.StartCasting(id, DelayDamage(summonTime));
        }
    }

    IEnumerator DelayDamage(float summonTime)
    {
        var term = (int) System.Math.Floor(summonTime / skillCfg.SummonHitInterval);
        for (int i = 0; i < term; i++)
        {
            // 被打断/死亡：castingSkillId 被 BreakCasting 置0，结束法阵
            if(owner == null || owner.hp <= 0 || owner.castingSkillId != id)
                yield break;

            // 引导期间每跳持续播放施法动作
            owner.PlayerAnim(skillCfg.Action);

            var unitsInRange = WorldManager.Instance.GetEnemyInRange(targetPos, skillCfg.Area, owner.side);
            if (unitsInRange.Count > 0)
            {
                WorldManager.Instance.RandomSelect(unitsInRange, skillCfg.TargetCount);
                var damage = GetSkillDamage(); // 固定系数 + 比例系数×关联属性
                foreach(var unit in unitsInRange)
                {
                    if (damage > 0)
                        unit.OnSkillDamaged(owner, skillId, damage);
                }
            }
            yield return new WaitForSeconds(skillCfg.SummonHitInterval);
        }
    }

}
