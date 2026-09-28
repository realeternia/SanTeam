using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CommonConfig;

public static class EffectManager
{

    // 通过 EffectConfig 解析特效资源路径（相对 Resources）：配置存省略 Prefabs/ 前缀的路径（如 Effect/xxx），
    // 这里统一补全；缺失时回退到 Prefabs/Effect 目录。外部引用仍传特效名（HitEffect 值），由这里统一解析。
    private static string ResolveEffectPath(string effectName)
    {
        if (!string.IsNullOrEmpty(effectName))
        {
            var cfg = EffectConfig.GetConfigByname(effectName);
            if (cfg != null && !string.IsNullOrEmpty(cfg.EffPath))
                return "Prefabs/" + cfg.EffPath;
        }
        if (string.IsNullOrEmpty(effectName))
            return "Prefabs/Effect/";
        GameLog.Warn("EffectManager 未在EffectConfig中配置特效[" + effectName + "]，回退 Prefabs/Effect 目录加载");
        return "Prefabs/Effect/" + effectName;
    }

    public static void PlayHitEffect(Chess sourceChess, Chess targetChess, string effectName)
    {
        // if (sourceChess.isHero)
        // {
        //     var heroConfig = HeroConfig.GetConfig(sourceChess.heroId);

        //     if ((sourceChess.side == 1 || sourceChess.side == 2) && effectName.StartsWith("Sword"))
        //         GameManager.Instance.PlaySound("Sounds/sword");
        // }
        // 播放粒子特效
        var hitPrefab = Resources.Load<GameObject>(ResolveEffectPath(effectName));
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
        if (string.IsNullOrEmpty(effect))
        {
            GameLog.Warn("PlaySkillEffect 特效名为空，跳过播放");
            return null;
        }
        var hitPrefab = Resources.Load<GameObject>(ResolveEffectPath(effect));
        if (hitPrefab == null)
        {
            GameLog.Warn("PlaySkillEffect 特效资源不存在: " + effect);
            return null;
        }
        GameLog.Debug("PlaySkillEffect: " + effect);

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
        var hitPrefab = Resources.Load<GameObject>(ResolveEffectPath(effect));
        GameLog.Debug("PlayPosSkillEffect: " + effect);

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
        var hitPrefab = Resources.Load<GameObject>(ResolveEffectPath(effect));
        GameLog.Debug("PlayBuffEffect: " + effect);

        GameObject hitEffect = UnityEngine.Object.Instantiate(hitPrefab, sourceChess.transform.position, hitPrefab.transform.rotation);
        // 设置特效的父对象为目标单位，使其跟随目标移动
        hitEffect.transform.parent = sourceChess.transform;
        hitEffect.transform.localScale = hitPrefab.transform.localScale;
   hitEffect.transform.localPosition += new Vector3(0f, 1f, 0f);

        return hitEffect;

    }

}
