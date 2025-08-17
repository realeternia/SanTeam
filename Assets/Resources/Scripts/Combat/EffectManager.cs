using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CommonConfig;

public static class EffectManager
{

    public static void PlayHitEffect(Chess sourceChess, Chess targetChess, string effectName)
    {
        var needMissile = false;
        if (sourceChess.isHero)
        {
            var heroConfig = HeroConfig.GetConfig(sourceChess.heroId);
            needMissile = heroConfig.Range >= 20;

            if ((sourceChess.side == 1 || sourceChess.side == 2) && effectName.StartsWith("Sword"))
                GameManager.Instance.PlaySound("Sounds/sword");
        }
        // 播放粒子特效
        var hitPrefab = Resources.Load<GameObject>("Prefabs/" + effectName);
        if (needMissile)
        {
            GameObject missileEffect = UnityEngine.Object.Instantiate(hitPrefab, sourceChess.transform.position, Quaternion.identity);
            missileEffect.transform.parent = sourceChess.transform;
            missileEffect.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

            // 启动协程让导弹飞向目标位置
            sourceChess.StartCoroutine(MoveMissileToTarget(missileEffect, targetChess, "Prefabs/" + effectName));
            UnityEngine.Object.Destroy(missileEffect, 2f);
        }
        else
        {
            GameObject hitEffect = UnityEngine.Object.Instantiate(hitPrefab, targetChess.transform.position, Quaternion.identity);
            // 设置特效的父对象为目标单位，使其跟随目标移动
            hitEffect.transform.parent = targetChess.transform;
            hitEffect.transform.localScale = new Vector3(1f, 1f, 1f);
            hitEffect.transform.localPosition += new Vector3(0f, 1f, 0f);
            // 可以添加代码设置特效的生命周期，例如几秒钟后自动销毁
            UnityEngine.Object.Destroy(hitEffect, 2f);
        }
    }

    // 定义协程方法，控制导弹移动
    static IEnumerator MoveMissileToTarget(GameObject missile, Chess targetChess, string effect)
    {
        var targetPos = targetChess.transform.position + new Vector3(0f, 1f, 0f);

        float journeyLength = Vector3.Distance(missile.transform.position, targetPos);
        float startTime = Time.time;
        float speed = 20f; // 导弹移动速度

        while (missile != null && Vector3.Distance(missile.transform.position, targetPos) > 0.1f)
        {
            float distCovered = (Time.time - startTime) * speed;
            float fractionOfJourney = distCovered / journeyLength;
            missile.transform.position = Vector3.Lerp(missile.transform.position, targetPos, fractionOfJourney);
            yield return new WaitForSeconds(0.02f);
        }

        if (missile != null)
        {
            if (targetChess != null)
            {
                UnityEngine.Object.Destroy(missile);

                var hitPrefab = Resources.Load<GameObject>(effect);
                GameObject hitEffect = UnityEngine.Object.Instantiate(hitPrefab, targetChess.transform.position, Quaternion.identity);
                // 设置特效的父对象为目标单位，使其跟随目标移动
                hitEffect.transform.parent = targetChess.transform;
                hitEffect.transform.localScale = new Vector3(1f, 1f, 1f);
                hitEffect.transform.localPosition += new Vector3(0f, 1f, 0f);
                // 可以添加代码设置特效的生命周期，例如几秒钟后自动销毁
                UnityEngine.Object.Destroy(hitEffect, 2f);
            }
        }
    }  

    public static void PlaySkillEffect(Chess sourceChess, string effect)
    {
        var hitPrefab = Resources.Load<GameObject>("Prefabs/" + effect);
        UnityEngine.Debug.Log("PlaySkillEffect: " + effect);

        GameObject hitEffect = UnityEngine.Object.Instantiate(hitPrefab, sourceChess.transform.position, hitPrefab.transform.rotation);
        // 设置特效的父对象为目标单位，使其跟随目标移动
        hitEffect.transform.parent = sourceChess.transform;
        hitEffect.transform.localScale = new Vector3(1f, 1f, 1f);
        hitEffect.transform.localPosition += new Vector3(0f, 1f, 0f);
        // 可以添加代码设置特效的生命周期，例如几秒钟后自动销毁
        UnityEngine.Object.Destroy(hitEffect, 1f);
    }

    public static GameObject PlayBuffEffect(Chess sourceChess, string effect)
    {
        var hitPrefab = Resources.Load<GameObject>("Prefabs/" + effect);
        UnityEngine.Debug.Log("PlayBuffEffect: " + effect);

        GameObject hitEffect = UnityEngine.Object.Instantiate(hitPrefab, sourceChess.transform.position, hitPrefab.transform.rotation);
        // 设置特效的父对象为目标单位，使其跟随目标移动
        hitEffect.transform.parent = sourceChess.transform;
        hitEffect.transform.localScale = hitPrefab.transform.localScale;
        hitEffect.transform.localPosition += hitPrefab.transform.localPosition;

        // 可以添加代码设置特效的生命周期，例如几秒钟后自动销毁
        return hitEffect;

    }

}
