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
        var rate = GetRate(defender);
        if (CheckBurst(rate))
        {
            targetPos = defender.transform.position;

            var chess = WorldManager.Instance.SpawnUnitsForRegion(owner.GetPlayerInfo(), 501001, targetPos, owner.side, "");
            chess.SetLifeTime(skillCfg.SummonTime);

            //创建一个hitEffect
            EffectManager.PlayPosSkillEffect(chess, targetPos, skillCfg.Range, skillCfg.HitEffect, skillCfg.SummonTime);

            owner.StartCoroutine(DelayDamage());
        }
    }

    IEnumerator DelayDamage()
    {
        var term = (int) System.Math.Floor(skillCfg.SummonTime / skillCfg.SummonHitInterval);
        for (int i = 0; i < term; i++)
        {
            if(owner == null || owner.hp <= 0)
                yield break;

            var unitsInRange = WorldManager.Instance.GetUnitsInRange(targetPos, skillCfg.Range, owner.side, true);
            if (unitsInRange.Count > 0)
            {
                WorldManager.Instance.RandomSelect(unitsInRange, skillCfg.TargetCount);
                var damage = (int)(owner.GetAttr(skillCfg.Attr) * skillCfg.Strength);
                foreach(var unit in unitsInRange)
                    unit.OnSkillDamaged(owner, damage);
            }
            yield return new WaitForSeconds(skillCfg.SummonHitInterval);
        }
    }

}
