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
            var cfg = EffectConfig.GetConfigByName(effectName);
            if (cfg != null && !string.IsNullOrEmpty(cfg.EffPath))
                return "Prefabs/" + cfg.EffPath;
        }
        if (string.IsNullOrEmpty(effectName))
            return "Prefabs/Effect/";
        GameLog.Warn("EffectManager 未在EffectConfig中配置特效[" + effectName + "]，回退 Prefabs/Effect 目录加载");
        return "Prefabs/Effect/" + effectName;
    }

    // 获取特效表配置的世界缩放/Y偏移，未配置时回退默认（缩放1、偏移1）
    private static float GetScale(string effectName, EffectConfig cfg, float fallback)
    {
        if (cfg != null && cfg.Scale > 0)
            return cfg.Scale;
        return fallback;
    }

    private static float GetOffsetY(string effectName, EffectConfig cfg)
    {
        if (cfg != null)
            return cfg.OffsetY;
        return 1f;
    }

    // 统一挂载逻辑：特效挂到任意父节点下，视觉始终等比例且大小稳定。
    // 父节点多为非等比缩放（如单位根缩放 10/5/10），若按各轴分别补偿会产生非均匀 localScale，
    // 会拉伸本地空间粒子/广告板导致变形，故统一按父级 X 轴（足迹缩放）做标量补偿，保证特效不变形。
    private static GameObject MountEffect(GameObject effect, Transform parent, Vector3 worldScale, float worldOffsetY)
    {
        var parentScale = parent.lossyScale;
        float factor = parentScale.x != 0 ? worldScale.x / parentScale.x : 1f;
        effect.transform.parent = parent;
        effect.transform.localScale = new Vector3(factor, factor, factor);
        var pos = effect.transform.localPosition;
        effect.transform.localPosition = new Vector3(pos.x, parentScale.y != 0 ? worldOffsetY / parentScale.y : worldOffsetY, pos.z);
        return effect;
    }

    // 配置了变色（TintColor）时，自动挂染色脚本并设置目标颜色
    public static void ApplyTint(GameObject effect, EffectConfig cfg)
    {
        if (effect == null || cfg == null || string.IsNullOrEmpty(cfg.TintColor))
            return;
        if (EffectTintColor.TryParseHex(cfg.TintColor, out var color))
        {
            var tint = effect.GetComponent<EffectTintColor>();
            if (tint == null)
                tint = effect.AddComponent<EffectTintColor>();
            tint.SetTint(color);
        }
        else
        {
            GameLog.Warn("EffectManager 变色配置无效: " + cfg.Name + " TintColor=" + cfg.TintColor);
        }
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
        var cfg = EffectConfig.GetConfigByName(effectName);
        float scale = GetScale(effectName, cfg, hitPrefab != null ? hitPrefab.transform.localScale.x : 1f);
        float offsetY = GetOffsetY(effectName, cfg);
        GameObject hitEffect = UnityEngine.Object.Instantiate(hitPrefab, targetChess.transform.position, Quaternion.identity);
        MountEffect(hitEffect, targetChess.transform, new Vector3(scale, scale, scale), offsetY);
        ApplyTint(hitEffect, cfg);
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

        var cfg = EffectConfig.GetConfigByName(effect);
        float scale = GetScale(effect, cfg, hitPrefab.transform.localScale.x);
        float offsetY = GetOffsetY(effect, cfg);
        GameObject hitEffect = UnityEngine.Object.Instantiate(hitPrefab, sourceChess.transform.position, hitPrefab.transform.rotation);
        MountEffect(hitEffect, sourceChess.transform, new Vector3(scale, scale, scale), offsetY);
        ApplyTint(hitEffect, cfg);
        // 可以添加代码设置特效的生命周期，例如几秒钟后自动销毁
        UnityEngine.Object.Destroy(hitEffect, time);
        return hitEffect;
    }

    public static GameObject PlayPosSkillEffect(Chess sourceChess, Vector3 sourcePos, float size, string effect, float time = 1.3f)
    {
        var hitPrefab = Resources.Load<GameObject>(ResolveEffectPath(effect));
        GameLog.Debug("PlayPosSkillEffect: " + effect);

        var cfg = EffectConfig.GetConfigByName(effect);
        float scale = GetScale(effect, cfg, size);
        float offsetY = GetOffsetY(effect, cfg);
        GameObject hitEffect = UnityEngine.Object.Instantiate(hitPrefab, sourcePos, hitPrefab.transform.rotation);
        MountEffect(hitEffect, sourceChess.transform, new Vector3(scale, scale, scale), offsetY);
        ApplyTint(hitEffect, cfg);
        // 可以添加代码设置特效的生命周期，例如几秒钟后自动销毁
        UnityEngine.Object.Destroy(hitEffect, time);

        return hitEffect;
    }

    public static GameObject PlayBuffEffect(Chess sourceChess, string effect)
    {
        var hitPrefab = Resources.Load<GameObject>(ResolveEffectPath(effect));
        GameLog.Debug("PlayBuffEffect: " + effect);

        var cfg = EffectConfig.GetConfigByName(effect);
        float scale = GetScale(effect, cfg, hitPrefab != null ? hitPrefab.transform.localScale.x : 1f);
        float offsetY = GetOffsetY(effect, cfg);
        GameObject hitEffect = UnityEngine.Object.Instantiate(hitPrefab, sourceChess.transform.position, hitPrefab.transform.rotation);
        MountEffect(hitEffect, sourceChess.transform, new Vector3(scale, scale, scale), offsetY);
        ApplyTint(hitEffect, cfg);

        return hitEffect;

    }

}
