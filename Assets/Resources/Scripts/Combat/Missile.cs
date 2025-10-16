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
    private string hitEffectName;

    private float size;

    public int skillId;
    public int skillDamage;

    public void Init(Chess sourceChess, float size, string effectName)
    {
        this.effectName = effectName;
        hitEffectName = effectName;
        owner = sourceChess;
        this.size = size;
    }

    public void SetSkillInfo(int skillId, int damage)
    {
        this.skillId = skillId;        
        skillDamage = damage;
    }

    public void MoveToDirection(Vector3 targetPos, float time, float missileSpeed)
    {
        var missilePrefab = Resources.Load<GameObject>("Prefabs/Missile/" + effectName);
        if (missilePrefab == null)
            missilePrefab = Resources.Load<GameObject>("Prefabs/Effect/" + effectName);
        GameObject missileEffect = Instantiate(missilePrefab, transform.position, missilePrefab.transform.rotation, transform);
        transform.rotation = Quaternion.LookRotation(targetPos - transform.position);
        transform.position += new Vector3(0f, 2f, 0f); 
        transform.localScale = size * missilePrefab.transform.localScale;

        if(missileEffect.TryGetComponent(out MissileComp missileComp))
            hitEffectName = missileComp.hitEffectName;

        var detectArea = 10f;
        var targetCount = 1;
        if (skillId > 0)
        {
            var skillCfg = SkillConfig.GetConfig(skillId);
            detectArea = skillCfg.SummonArea * 1.5f;
            targetCount = skillCfg.TargetCount;
        }

        StartCoroutine(MoveMissileToDirection(gameObject, (targetPos - missileEffect.transform.position).normalized, time, missileSpeed, detectArea, targetCount));
    }

    public void MoveToTarget(Chess target, float missileSpeed, float missileHight)
    {
        var missilePrefab = Resources.Load<GameObject>("Prefabs/Missile/" + effectName);
        if (missilePrefab == null)
            missilePrefab = Resources.Load<GameObject>("Prefabs/Effect/" + effectName);

        GameObject missileEffect = Instantiate(missilePrefab, transform.position, Quaternion.identity, transform);
        transform.position += new Vector3(0f, 5f, 0f);
        missileEffect.transform.localScale = missilePrefab.transform.localScale;

        if(missileEffect.TryGetComponent(out MissileComp missileComp))
            hitEffectName = missileComp.hitEffectName;        

        StartCoroutine(MoveMissileToTarget(gameObject, target, missileSpeed, missileHight));
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
        while (missile != null && !WorldManager.Instance.CheckInRange(missile.transform.position, targetPos, 0.5f))
        {
            // if (owner == null || owner.hp <= 0)
            // {
            //     Destroy(missile);
            //     yield break;
            // }
            if(target != null && target.hp > 0)
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
            OnCrash(target);
            Destroy(missile);
        }
    }

 // 让hitEffect飞向targetPos的协程
    IEnumerator MoveMissileToDirection(GameObject missile, Vector3 direction, float time, float speed, float detectArea, int targetCount)
    {
        Vector3 currentPos = missile.transform.position;
        direction.y = 0;
        float timePast = 0;
        float lastCheckTime = 0.2f;
        var checkedList = new List<Chess>();

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
                var unitsInRange = WorldManager.Instance.GetUnitsInRange(currentPos, detectArea, owner.side, true);
                unitsInRange.RemoveAll(x => checkedList.Contains(x) || x.hp <= 0); //每个单位结算一次
                if (unitsInRange.Count > 0)
                {
                    if (unitsInRange.Count + checkedList.Count > targetCount)
                        WorldManager.Instance.RandomSelect(unitsInRange, targetCount - unitsInRange.Count - checkedList.Count);

                    foreach (var unit in unitsInRange)
                    {
                        checkedList.Add(unit);
                        OnCrash(unit);
                    }
                }

                lastCheckTime = timePast;
            }

            timePast += 0.025f;
            if (timePast >= time || checkedList.Count >= targetCount)
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

    private void OnCrash(Chess target)
    {
        if (target == null || target.hp <= 0 || owner == null || owner.hp <= 0)
            return;

        if (skillId == 0)
        {
            owner.Attack(target, hitEffectName);
        }
        else
        {
            target.OnSkillDamaged(owner, skillId, skillDamage);
            EffectManager.PlaySkillEffect(target, hitEffectName);
        }
    }
}