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

    public override void OnAttack(Chess defender, string damType, int damage)
    {
        if (CheckBurst(defender))
        {
            owner.PlayerAnim(skillCfg.Action);

            targetPos = defender.transform.position;

            var magicStub = WorldManager.Instance.SpawnUnitsForRegion(owner.GetPlayerInfo(), 501001, -1, targetPos, owner.side, "");
            var summonTime = GetSummonTime();
            magicStub.SetLifeTime(summonTime);

            //创建一个hitEffect
            EffectManager.PlayPosSkillEffect(magicStub, targetPos, skillCfg.EffectSize, skillCfg.HitEffect, summonTime);

            owner.StartCoroutine(DelayDamage(summonTime));
        }
    }

    IEnumerator DelayDamage(float summonTime)
    {
        var term = (int) System.Math.Floor(summonTime / skillCfg.SummonHitInterval);
        for (int i = 0; i < term; i++)
        {
            if(owner == null || owner.hp <= 0)
                yield break;

            var unitsInRange = WorldManager.Instance.GetUnitsInRange(targetPos, skillCfg.SummonArea, owner.side, true);
            if (unitsInRange.Count > 0)
            {
                WorldManager.Instance.RandomSelect(unitsInRange, skillCfg.TargetCount);
                var damage = (int)(owner.GetAttr(skillCfg.Attr) * skillCfg.SkillDamageAttrRate);
                foreach(var unit in unitsInRange)
                    unit.OnSkillDamaged(owner, skillId, damage);
            }
            yield return new WaitForSeconds(skillCfg.SummonHitInterval);
        }
    }

}
