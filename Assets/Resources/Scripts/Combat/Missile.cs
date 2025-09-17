using System;
using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;
using UnityEngine.UI;


public class Missile : MonoBehaviour
{
    public Chess owner;
    public string effectName;
    public int skillId;

    public void Init(Chess sourceChess, Chess targetChess, float speed, float hight, string effectName)
    {
        this.effectName = effectName;
        owner = sourceChess;

        var hitPrefab = Resources.Load<GameObject>("Prefabs/Missile/" + effectName);
        if (hitPrefab == null)
            hitPrefab = Resources.Load<GameObject>("Prefabs/Effect/" + effectName);

        transform.position = sourceChess.transform.position + new Vector3(0f, 5f, 0f);
        GameObject missileEffect = Instantiate(hitPrefab, transform.position, Quaternion.identity, transform);

        missileEffect.transform.localScale = hitPrefab.transform.localScale;

        // 启动协程让导弹飞向目标位置
        StartCoroutine(MoveMissileToTarget(gameObject, targetChess, speed, hight));

       // Destroy(missileEffect, 2f);
    }
    
    public void Init(Chess sourceChess, Vector3 targetPos, float time, float speed, float size, int skillId, string effectName)
    {
        this.effectName = effectName;
        owner = sourceChess;
        this.skillId = skillId;

        var hitPrefab = Resources.Load<GameObject>("Prefabs/Missile/" + effectName);
        if (hitPrefab == null)
            hitPrefab = Resources.Load<GameObject>("Prefabs/Effect/" + effectName);

        transform.position = sourceChess.transform.position + new Vector3(0f, 2f, 0f);
        GameObject missileEffect = Instantiate(hitPrefab, transform.position, hitPrefab.transform.rotation, transform);

        transform.localScale = size * hitPrefab.transform.localScale;
        transform.rotation = Quaternion.LookRotation(targetPos - transform.position);

        // 启动协程让导弹飞向目标位置
        StartCoroutine(MoveMissileToDirection(gameObject, (targetPos - missileEffect.transform.position).normalized, time, speed));

       // Destroy(missileEffect, 2f);
    }


    // 定义协程方法，控制导弹移动
    IEnumerator MoveMissileToTarget(GameObject missile, Chess target, float missileSpeed, float missileHight)
    {
        var targetPos = target.transform.position + new Vector3(0f, 5f, 0f);

        float journeyLength = WorldManager.Instance.GetRange(missile.transform.position, targetPos);
        float totalLen = journeyLength;
        float realLen = 0;
        float startTime = Time.time;
        float speed = missileSpeed * 2.5f; // 导弹移动速度

        float maxY = missileHight;

        var lastTime = Time.time;
        while (missile != null && target != null && !WorldManager.Instance.CheckInRange(missile.transform.position, targetPos, 0.5f) && !ReferenceEquals(target, null))
        {
            // if (owner == null || owner.hp <= 0)
            // {
            //     Destroy(missile);
            //     yield break;
            // }
            targetPos = target.transform.position + new Vector3(0f, 5f, 0f); //修正目标点
            float distCovered = (Time.time - lastTime) * speed;
            journeyLength = WorldManager.Instance.GetRange(missile.transform.position, targetPos);
            float fractionOfJourney = distCovered / journeyLength;
            
            if (maxY > 0)
            {
                Vector3 horizontalPos = Vector3.Lerp(missile.transform.position, targetPos, fractionOfJourney);

                // UnityEngine.Debug.Log("fractionOfJourney: " + fractionOfJourney);
                realLen += distCovered * 1.1f;
                if(realLen > totalLen)
                    realLen = totalLen;

                // 计算抛物线高度
                float parabolaHeight = maxY * Mathf.Sin((realLen / totalLen) * Mathf.PI);
                horizontalPos.y += parabolaHeight;
                
                missile.transform.position = horizontalPos;
                missile.transform.rotation = Quaternion.LookRotation(targetPos - missile.transform.position);
            }
            else
            {
                // 直线路径
                missile.transform.position = Vector3.Lerp(missile.transform.position, targetPos, fractionOfJourney);
            }
            lastTime = Time.time;
            yield return new WaitForSeconds(0.025f);
        }

        if (missile != null)
        {
            if (target != null && owner != null && owner.hp > 0)
                owner.Attack(target);
            Destroy(missile);
        }
    }

 // 让hitEffect飞向targetPos的协程
    IEnumerator MoveMissileToDirection(GameObject missile, Vector3 direction, float time, float speed)
    {
        Vector3 currentPos = missile.transform.position;
        direction.y = 0;
        float timePast = 0;
        float lastCheckTime = 0.2f;
        var unitList = new List<Chess>();
        var skillCfg = SkillConfig.GetConfig(skillId);

        while (missile != null)
        {
            // if (owner == null || owner.hp <= 0)
            //     yield break;

            // 计算本次移动的距离（基于速度和时间）
            float moveDistance = speed * 0.025f;

            // 按方向和距离移动 
            currentPos = missile.transform.position = currentPos + direction * moveDistance;  

            if (timePast - lastCheckTime >= 0.2f)
            {
                var unitsInRange = WorldManager.Instance.GetUnitsInRange(currentPos, skillCfg.SummonArea * 1.5f, owner.side, true);
                WorldManager.Instance.RandomSelect(unitsInRange, skillCfg.TargetCount);

                var damageUnitList = new List<Chess>();
                foreach (var unit in unitsInRange)
                {
                    if (unitList.Contains(unit))
                        continue;
                    unitList.Add(unit);
                    damageUnitList.Add(unit);
                }
                var damage = (int)(owner.GetAttr(skillCfg.Attr) * skillCfg.Strength);
                if (owner != null && owner.hp > 0)
                {
                    foreach (var unit in damageUnitList)
                        unit.OnSkillDamaged(owner, damage);
                }
                lastCheckTime = timePast;
            }

            timePast += 0.025f;
            if (timePast >= time)
            {
                if (missile != null)
                {
                    Destroy(missile);
                }
                yield break;
            }

            yield return new WaitForSeconds(0.025f);
        }


    }
}