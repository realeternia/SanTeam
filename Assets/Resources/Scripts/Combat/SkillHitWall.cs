using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

public class SkillHitWall : Skill
{
    private List<Vector3> targetPosList;
    public SkillHitWall(int id, Chess unit) : base(id, unit)
    {
    }

    public override void OnAttack(Chess defender, string damType, int damage)
    {
        if (CheckBurst(defender))
        {
            owner.PlayerAnim(skillCfg.Action);
            // 在目标位置，以及owner和defender方向90度两侧，各创建一个effect
            var targetPos = defender.transform.position;

            var magicStub = WorldManager.Instance.SpawnUnitsForRegion(owner.GetPlayerInfo(), 501001, -1, targetPos, owner.side, "");
            var summonTime = GetSummonTime();
            magicStub.SetLifeTime(summonTime);
            
            // 计算owner到defender的方向
            Vector3 direction = (defender.transform.position - owner.transform.position).normalized;
            
            // 计算90度和-90度旋转的方向
            Vector3 rightDirection = Quaternion.Euler(0, 90, 0) * direction;
            Vector3 leftDirection = Quaternion.Euler(0, -90, 0) * direction;

            targetPosList = new List<Vector3>();
            
            targetPosList.Add(targetPos);
            if (skillCfg.SummonCount > 1)
            {
                targetPosList.Add(targetPos + leftDirection * 10);
                targetPosList.Add(targetPos + rightDirection * 10);
            }
            if (skillCfg.SummonCount > 3)
            {
                targetPosList.Add(targetPos + rightDirection * 20);
                targetPosList.Add(targetPos + leftDirection * 20);
            }
            
            foreach(var pos in targetPosList)
            {
                EffectManager.PlayPosSkillEffect(magicStub, pos, skillCfg.EffectSize, skillCfg.HitEffect, summonTime);
            }
            owner.StartCoroutine(DelayDamage(summonTime));
        }
    }

    IEnumerator DelayDamage(float summonTime)
    {
        var term = (int)Math.Floor(summonTime / skillCfg.SummonHitInterval);
        for (int i = 0; i < term; i++)
        {
            if (owner == null || owner.hp <= 0)
                yield break;

            var unitList = new List<Chess>();
            foreach (var pos in targetPosList)
            {
                var unitsInRange = WorldManager.Instance.GetUnitsInRange(pos, skillCfg.SummonArea * 1.5f, owner.side, true);
                WorldManager.Instance.RandomSelect(unitsInRange, skillCfg.TargetCount);

                foreach (var unit in unitsInRange)
                {
                    if (unitList.Contains(unit))
                        continue;
                    unitList.Add(unit);
                }
            }
            var damage = (int)(owner.GetAttr(skillCfg.Attr) * skillCfg.SkillDamageAttrRate);
            foreach (var unit in unitList)
            {
                unit.OnSkillDamaged(owner, skillId, damage);
            }

            yield return new WaitForSeconds(skillCfg.SummonHitInterval);
        }
    }

}
