using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

public class SkillShockWave : Skill
{
    private Vector3 targetPos;
    public SkillShockWave(int id, Chess unit) : base(id, unit)
    {
    }

    public override bool CheckAidSkill()
    {
        if (owner.targetChess == null)
            return false;

        if(!WorldManager.Instance.CheckInRange(owner.transform.position, owner.targetChess.transform.position, 50))
            return false;

        if(!CheckBurst())
            return false;

        this.targetPos = owner.targetChess.transform.position; // 使用目标位置而不是自身位置
        var magicStub = WorldManager.Instance.SpawnUnitsForRegion(owner.GetPlayerInfo(), 501001, owner.transform.position, owner.side, "");
        magicStub.SetLifeTime(skillCfg.SummonTime);

        var hitEffect = EffectManager.PlayPosSkillEffect(magicStub, owner.transform.position, skillCfg.EffectSize, skillCfg.HitEffect, skillCfg.SummonTime);
        owner.StartCoroutine(MoveHitEffectToTarget(hitEffect, skillCfg.SummonTime, skillCfg.SummonSpeed)); // 启动新协程让特效飞向目标

        return true;
    }

    // 让hitEffect飞向targetPos的协程
    IEnumerator MoveHitEffectToTarget(GameObject hitEffect, float time, float speed)
    {
        if (hitEffect == null)
            yield break;

        Vector3 currentPos = hitEffect.transform.position;
        Vector3 targetPosition = targetPos;
        Vector3 direction = (targetPosition - currentPos).normalized;
        direction.y = 0;
        float timePast = 0;
        float lastCheckTime = 0.2f;
        var unitList = new List<Chess>();

        while (true)
        {
            if (hitEffect == null || owner == null || owner.hp <= 0)
                yield break;

            // 计算本次移动的距离（基于速度和时间）
            float moveDistance = speed * Time.deltaTime;

            // 按方向和距离移动 
            currentPos = hitEffect.transform.position = currentPos + direction * moveDistance;  

            if (timePast - lastCheckTime >= 0.2f)
            {
                var unitsInRange = WorldManager.Instance.GetUnitsInRange(currentPos, Math.Max(speed * 0.2f, skillCfg.Range) * 1.5f, owner.side, true);
                WorldManager.Instance.RandomSelect(unitsInRange, skillCfg.TargetCount);

                var damageUnitList = new List<Chess>();
                foreach (var unit in unitsInRange)
                {
                    if (unitList.Contains(unit))
                        continue;
                    unitList.Add(unit);
                    damageUnitList.Add(unit);
                }
                //var damage = (int)(owner.GetAttr(skillCfg.Attr) * skillCfg.Strength);
                foreach (var unit in damageUnitList)
                {
                    unit.OnSkillDamaged(owner, 50);
                }
                lastCheckTime = timePast;
            }

            timePast += Time.deltaTime;
            if (timePast >= time)
                yield break;
            
            yield return null;
        }
    }

}
