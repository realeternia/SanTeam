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
        NormalizeRootScalingMode(effect);
        return effect;
    }

    // 根节点上的粒子若为 Local 缩放模式，渲染尺寸只受自身 localScale 影响（忽略父级缩放），
    // 而本方法为抵消父级缩放改写了根节点 localScale，会使这类粒子比参考尺寸（EffectTest 场景）小一个父级缩放的倍数
    // （单位根缩放 10 则缩小 10 倍）。根节点粒子统一改为 Hierarchy（跟随父级层级），
    // 使其世界缩放等于配置 Scale，与子级 Hierarchy 系统及测试场景观感一致。
    // 注：不需要遍历子级——子级 localScale 未被改动，两种模式下与测试场景表现相同。
    private static void NormalizeRootScalingMode(GameObject effect)
    {
        foreach (var ps in effect.GetComponents<ParticleSystem>())
        {
            if (ps.scalingMode == ParticleSystemScalingMode.Local)
                ps.scalingMode = ParticleSystemScalingMode.Hierarchy;
        }
    }

    // 战场场景容器（WorldManager.Units 节点，缩放1）：单位/导弹/对地特效统一挂其下，
    // 战斗结束（BattleEnd / 重新布阵）会销毁其全部子节点，保证对地特效不残留。
    private static Transform SceneContainer()
    {
        var wm = WorldManager.Instance;
        if (wm != null && wm.Units != null)
            return wm.Units.transform;
        GameLog.Warn("EffectManager 未取到 WorldManager.Units，对地特效回退挂到场景根");
        return null;
    }

    /// <summary>
    /// 场景锚定挂载（对地/范围特效专用）：挂到战场场景容器而非施法者节点，
    /// 世界位置=落点+配置高度偏移、朝向=预制体朝向、世界缩放=配置值，
    /// 均不随施法者移动/转向变化（挂施法者节点会随其移动或动作位移一起漂移）。
    /// </summary>
    private static GameObject MountEffectToScene(GameObject effect, Vector3 worldPos, Vector3 worldScale, float worldOffsetY)
    {
        var parent = SceneContainer();
        effect.transform.parent = parent;
        effect.transform.position = worldPos + Vector3.up * worldOffsetY;
        var parentScale = parent != null ? parent.lossyScale : Vector3.one;
        float factor = parentScale.x != 0 ? worldScale.x / parentScale.x : worldScale.x;
        effect.transform.localScale = new Vector3(factor, factor, factor);
        NormalizeRootScalingMode(effect);
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
        // 播放粒子特效
        var hitPrefab = Resources.Load<GameObject>(ResolveEffectPath(effectName));
        var cfg = EffectConfig.GetConfigByName(effectName);
        float scale = GetScale(effectName, cfg, hitPrefab != null ? hitPrefab.transform.localScale.x : 1f);
        float offsetY = GetOffsetY(effectName, cfg) + CombatConst.UnitEffectExtraOffsetY;
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
        float offsetY = GetOffsetY(effect, cfg) + CombatConst.UnitEffectExtraOffsetY;
        GameObject hitEffect = UnityEngine.Object.Instantiate(hitPrefab, sourceChess.transform.position, hitPrefab.transform.rotation);
        MountEffect(hitEffect, sourceChess.transform, new Vector3(scale, scale, scale), offsetY);
        ApplyTint(hitEffect, cfg);
        // 可以添加代码设置特效的生命周期，例如几秒钟后自动销毁
        UnityEngine.Object.Destroy(hitEffect, time);
        return hitEffect;
    }

    /// <summary>
    /// 对地/范围特效：在指定世界位置播放范围特效，缩放 = EffectConfig.Scale × (area / AreaEffectBaseRadius)，
    /// 即施法范围越大特效越大（area&lt;=0 时不做额外缩放）。范围技能统一经 Skill.PlayAreaEffect 调用。
    /// 特效挂在战场场景容器上（不挂施法者节点），落点固定，不随施法者移动/转向漂移。
    /// rotation 非空时按该朝向摆放特效（如扇形/矩形喷火需要朝向施法方向）。
    /// </summary>
    public static GameObject PlayPosSkillEffect(Vector3 sourcePos, float area, string effect, float time = 1.3f, Quaternion? rotation = null)
    {
        if (string.IsNullOrEmpty(effect))
        {
            GameLog.Warn("PlayPosSkillEffect 特效名为空，跳过播放");
            return null;
        }
        var hitPrefab = Resources.Load<GameObject>(ResolveEffectPath(effect));
        if (hitPrefab == null)
        {
            GameLog.Warn("PlayPosSkillEffect 特效资源不存在: " + effect);
            return null;
        }
        GameLog.Debug("PlayPosSkillEffect: " + effect);

        var cfg = EffectConfig.GetConfigByName(effect);
        float baseScale = GetScale(effect, cfg, hitPrefab.transform.localScale.x);
        float extra = (area > 0f && CombatConst.AreaEffectBaseRadius > 0f) ? area / CombatConst.AreaEffectBaseRadius : 1f;
        float scale = baseScale * extra;
        float offsetY = GetOffsetY(effect, cfg);
        GameObject hitEffect = UnityEngine.Object.Instantiate(hitPrefab, sourcePos, hitPrefab.transform.rotation);
        MountEffectToScene(hitEffect, sourcePos, new Vector3(scale, scale, scale), offsetY);
        if (rotation.HasValue)
            hitEffect.transform.rotation = rotation.Value;
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
        float offsetY = GetOffsetY(effect, cfg) + CombatConst.UnitEffectExtraOffsetY;
        GameObject hitEffect = UnityEngine.Object.Instantiate(hitPrefab, sourceChess.transform.position, hitPrefab.transform.rotation);
        MountEffect(hitEffect, sourceChess.transform, new Vector3(scale, scale, scale), offsetY);
        ApplyTint(hitEffect, cfg);
        // buff 特效随 buff 存在而持续，粒子需循环播放，避免 buff 未结束特效先熄灭
        foreach (var ps in hitEffect.GetComponentsInChildren<ParticleSystem>(true))
        {
            // MainModule 的 setter 直接写回原生粒子系统，无需写回 main 属性
            var main = ps.main;
            main.loop = true;
            ps.Play();
        }

        return hitEffect;

    }

    /// <summary>
    /// 持续型自身特效：把特效常驻挂在单位节点下（挂点/缩放规则同 PlayBuffEffect），粒子循环播放且不自动销毁，
    /// 随单位节点销毁（单位死亡/战斗结束）一并清理。用于需要永久存在于自身的视觉标识（如光环技能持有者）。
    /// </summary>
    public static GameObject PlayPersistentEffect(Chess sourceChess, string effect)
    {
        if (sourceChess == null || string.IsNullOrEmpty(effect))
        {
            GameLog.Warn("PlayPersistentEffect 参数为空，跳过播放 effect=" + effect);
            return null;
        }
        var prefab = Resources.Load<GameObject>(ResolveEffectPath(effect));
        if (prefab == null)
        {
            GameLog.Warn("PlayPersistentEffect 特效资源不存在: " + effect);
            return null;
        }
        GameLog.Debug("PlayPersistentEffect: " + effect);

        var cfg = EffectConfig.GetConfigByName(effect);
        float scale = GetScale(effect, cfg, prefab.transform.localScale.x);
        float offsetY = GetOffsetY(effect, cfg);
        GameObject hitEffect = UnityEngine.Object.Instantiate(prefab, sourceChess.transform.position, prefab.transform.rotation);
        MountEffect(hitEffect, sourceChess.transform, new Vector3(scale, scale, scale), offsetY);
        ApplyTint(hitEffect, cfg);

        // 常驻期间粒子需持续播放，循环避免中途熄灭；不设置销毁时间，交由单位节点销毁时清理
        foreach (var ps in hitEffect.GetComponentsInChildren<ParticleSystem>(true))
        {
            var main = ps.main;
            main.loop = true;
            ps.Play();
        }
        return hitEffect;
    }

    // 解析导弹资源路径（规则与 Missile.ResolveMissilePath 一致）：优先 EffectConfig.MissilePath，缺失回退特效路径，再回退 Prefabs/Missile 目录
    private static string ResolveMissilePath(string effectName)
    {
        var cfg = EffectConfig.GetConfigByName(effectName);
        if (cfg != null)
        {
            if (!string.IsNullOrEmpty(cfg.MissilePath))
                return "Prefabs/" + cfg.MissilePath;
            if (!string.IsNullOrEmpty(cfg.EffPath))
                return "Prefabs/" + cfg.EffPath;
        }
        GameLog.Warn("EffectManager 未在EffectConfig中配置导弹[" + effectName + "]，回退 Prefabs/Missile 目录加载");
        return "Prefabs/Missile/" + effectName;
    }

    /// <summary>
    /// 跟随型导弹特效：把导弹预制体挂到单位身上（作为其子节点），随单位移动持续播放粒子，
    /// 单位瞬移/位移时粒子在移动路径上留下拖尾残影（快闪类技能用）。time&gt;0 时到时自动销毁，否则由调用方销毁。
    /// </summary>
    public static GameObject PlayFollowMissileEffect(Chess unit, string effect, float time = 0f)
    {
        if (unit == null || string.IsNullOrEmpty(effect))
        {
            GameLog.Warn("PlayFollowMissileEffect 参数为空，跳过播放 effect=" + effect);
            return null;
        }

        var missilePrefab = Resources.Load<GameObject>(ResolveMissilePath(effect));
        if (missilePrefab == null)
        {
            GameLog.Warn("PlayFollowMissileEffect 导弹资源不存在: " + effect);
            return null;
        }
        GameLog.Debug("PlayFollowMissileEffect: " + effect);

        var cfg = EffectConfig.GetConfigByName(effect);
        float scale = cfg != null && cfg.MissileScale > 0 ? cfg.MissileScale : missilePrefab.transform.localScale.x;
        float offsetY = cfg != null ? cfg.MissileOffsetY : 0f;
        GameObject missileEffect = UnityEngine.Object.Instantiate(missilePrefab, unit.transform.position, missilePrefab.transform.rotation);
        MountEffect(missileEffect, unit.transform, new Vector3(scale, scale, scale), offsetY);
        ApplyTint(missileEffect, cfg);

        // 跟随期间粒子需持续播放（拖尾），循环避免中途熄灭
        foreach (var ps in missileEffect.GetComponentsInChildren<ParticleSystem>(true))
        {
            var main = ps.main;
            main.loop = true;
            ps.Play();
        }

        if (time > 0f)
            UnityEngine.Object.Destroy(missileEffect, time);
        return missileEffect;
    }

}
