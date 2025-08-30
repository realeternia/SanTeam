using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Missile : MonoBehaviour
{
    public Chess owner;
    public Chess target;
    public string effectName;

    public void Init(Chess sourceChess, Chess targetChess, string effectName)
    {
        this.effectName = effectName;
        owner = sourceChess;
        target = targetChess;

        var hitPrefab = Resources.Load<GameObject>("Prefabs/Missile/" + effectName);
        if (hitPrefab == null)
            hitPrefab = Resources.Load<GameObject>("Prefabs/Effect/" + effectName);

        transform.position = sourceChess.transform.position + new Vector3(0f, 5f, 0f);
        GameObject missileEffect = Instantiate(hitPrefab, transform.position, Quaternion.identity, transform);

        missileEffect.transform.localScale = hitPrefab.transform.localScale;

        // 启动协程让导弹飞向目标位置
        StartCoroutine(MoveMissileToTarget(gameObject, owner.missileSpeed, effectName.StartsWith("Bullet")));

       // Destroy(missileEffect, 2f);
    }


    // 定义协程方法，控制导弹移动
    IEnumerator MoveMissileToTarget(GameObject missile, int missileSpeed, bool isBullet)
    {
        var targetPos = target.transform.position + new Vector3(0f, 5f, 0f);

        float journeyLength = WorldManager.Instance.GetRange(missile.transform.position, targetPos);
        float totalLen = journeyLength;
        float realLen = 0;
        float startTime = Time.time;
        float speed = missileSpeed * 3; // 导弹移动速度

        float maxY = 0;
        if(isBullet)
            maxY = 1.5f; //抛物线最高点高度

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

                UnityEngine.Debug.Log("fractionOfJourney: " + fractionOfJourney);
                realLen += distCovered * 1.1f;
                if(realLen > totalLen)
                    realLen = totalLen;

                // 计算抛物线高度
                float parabolaHeight = maxY * Mathf.Sin((realLen / totalLen) * Mathf.PI);
                horizontalPos.y += parabolaHeight;
                
                missile.transform.position = horizontalPos;
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
            {
                owner.Attack(target);
            }
            Destroy(missile);
        }
    }

}