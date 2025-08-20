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

        transform.position = sourceChess.transform.position + new Vector3(0f, 7.5f, 0f);
        GameObject missileEffect = Instantiate(hitPrefab, transform.position, Quaternion.identity, transform);

        missileEffect.transform.localScale = hitPrefab.transform.localScale;

        // 启动协程让导弹飞向目标位置
        StartCoroutine(MoveMissileToTarget(gameObject, "Prefabs/Effect/" + effectName));
        Destroy(missileEffect, 2f);
    }


    // 定义协程方法，控制导弹移动
    IEnumerator MoveMissileToTarget(GameObject missile, string effect)
    {
        var targetPos = target.transform.position + new Vector3(0f, 5f, 0f);

        float journeyLength = Vector3.Distance(missile.transform.position, targetPos);
        float startTime = Time.time;
        float speed = 20f; // 导弹移动速度

        while (missile != null && !WorldManager.Instance.CheckInRange(missile.transform.position, target.transform.position, 0.5f))
        {
            if (owner == null || owner.hp <= 0)
                yield break;

            targetPos = target.transform.position + new Vector3(0f, 5f, 0f);
            float distCovered = (Time.time - startTime) * speed;
            float fractionOfJourney = distCovered / journeyLength;
            missile.transform.position = Vector3.Lerp(missile.transform.position, targetPos, fractionOfJourney);
            yield return new WaitForSeconds(0.025f);
        }

        if (missile != null)
        {
            if (target != null && owner != null)
            {
                Destroy(missile);
                owner.Attack(target);
            }
        }
    }

}