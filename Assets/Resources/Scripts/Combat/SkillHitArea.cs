using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

public class SkillHitArea : Skill
{
    private Vector3 targetPos;
    public SkillHitArea(int id, Chess unit) : base(id, unit)
    {
    }

    public override void OnAttack(Chess defender, string damType, int damage)
    {
        if (CheckBurst())
        {
            targetPos = defender.transform.position;

            var chess = WorldManager.Instance.SpawnUnitsForRegion(owner.GetPlayerInfo(), 501001, targetPos, owner.side, "");
            chess.SetLifeTime(skillCfg.LastTime);

            //创建一个hitEffect
            EffectManager.PlayPosSkillEffect(chess, targetPos, skillCfg.Range, skillCfg.HitEffect, skillCfg.LastTime);

            owner.StartCoroutine(DelayDamage());
        }
    }

    IEnumerator DelayDamage()
    {
        for (int i = 0; i < 10; i++)
        {
            if(owner == null || owner.hp <= 0)
                yield break;

            var unitsInRange = WorldManager.Instance.GetUnitsInRange(targetPos, skillCfg.Range, owner.side, true);
            if (unitsInRange.Count > 0)
            {
                WorldManager.Instance.RandomSelect(unitsInRange, skillCfg.TargetCount);
                var damage = (int)(owner.inte * skillCfg.Strength);
                foreach(var unit in unitsInRange)
                    unit.OnSkillDamaged(owner, damage);
            }
            yield return new WaitForSeconds(skillCfg.LastInterval);
        }
    }

}
