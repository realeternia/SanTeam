using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CommonConfig;

public static class EffectManager
{

    public static void PlayHitEffect(Chess sourceChess, Chess targetChess, string effectName)
    {
        // if (sourceChess.isHero)
        // {
        //     var heroConfig = HeroConfig.GetConfig(sourceChess.heroId);

        //     if ((sourceChess.side == 1 || sourceChess.side == 2) && effectName.StartsWith("Sword"))
        //         GameManager.Instance.PlaySound("Sounds/sword");
        // }
        // 播放粒子特效
        var hitPrefab = Resources.Load<GameObject>("Prefabs/Effect/" + effectName);
        GameObject hitEffect = UnityEngine.Object.Instantiate(hitPrefab, targetChess.transform.position, Quaternion.identity);
        // 设置特效的父对象为目标单位，使其跟随目标移动
        hitEffect.transform.parent = targetChess.transform;
        hitEffect.transform.localScale = hitPrefab.transform.localScale;
        hitEffect.transform.localPosition += new Vector3(0f, 1f, 0f);
        // 可以添加代码设置特效的生命周期，例如几秒钟后自动销毁
        UnityEngine.Object.Destroy(hitEffect, 1.3f);
    }

    public static GameObject PlaySkillEffect(Chess sourceChess, string effect, float time = 1.3f)
    {
        var hitPrefab = Resources.Load<GameObject>("Prefabs/Effect/" + effect);
        UnityEngine.Debug.Log("PlaySkillEffect: " + effect);

        GameObject hitEffect = UnityEngine.Object.Instantiate(hitPrefab, sourceChess.transform.position, hitPrefab.transform.rotation);
        // 设置特效的父对象为目标单位，使其跟随目标移动
        hitEffect.transform.parent = sourceChess.transform;
        hitEffect.transform.localScale = hitPrefab.transform.localScale;
        hitEffect.transform.localPosition += new Vector3(0f, 1f, 0f);
        // 可以添加代码设置特效的生命周期，例如几秒钟后自动销毁
        UnityEngine.Object.Destroy(hitEffect, time);
        return hitEffect;
    }

    public static GameObject PlayPosSkillEffect(Chess sourceChess, Vector3 sourcePos, float size, string effect, float time = 1.3f)
    {
        var hitPrefab = Resources.Load<GameObject>("Prefabs/Effect/" + effect);
        UnityEngine.Debug.Log("PlayPosSkillEffect: " + effect);

        GameObject hitEffect = UnityEngine.Object.Instantiate(hitPrefab, sourcePos, hitPrefab.transform.rotation);
        // 设置特效的父对象为目标单位，使其跟随目标移动
        hitEffect.transform.parent = sourceChess.transform;
        hitEffect.transform.localScale = size * hitPrefab.transform.localScale;
        hitEffect.transform.localPosition += new Vector3(0f, 1f, 0f);
        // 可以添加代码设置特效的生命周期，例如几秒钟后自动销毁
        UnityEngine.Object.Destroy(hitEffect, time);

        return hitEffect;
    }    

    public static GameObject PlayBuffEffect(Chess sourceChess, string effect)
    {
        var hitPrefab = Resources.Load<GameObject>("Prefabs/Effect/" + effect);
        UnityEngine.Debug.Log("PlayBuffEffect: " + effect);

        GameObject hitEffect = UnityEngine.Object.Instantiate(hitPrefab, sourceChess.transform.position, hitPrefab.transform.rotation);
        // 设置特效的父对象为目标单位，使其跟随目标移动
        hitEffect.transform.parent = sourceChess.transform;
        hitEffect.transform.localScale = hitPrefab.transform.localScale;
   hitEffect.transform.localPosition += new Vector3(0f, 1f, 0f);

        return hitEffect;

    }

}
