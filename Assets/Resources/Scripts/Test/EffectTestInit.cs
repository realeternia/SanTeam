using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 特效测试场景初始化：按 EffectConfig 遍历，把所有特效与导弹平铺创建出来。
/// 特效原地静止展示；导弹做左右往复摆动，方便观察拖尾。
/// </summary>
public class EffectTestInit : MonoBehaviour
{
    [Header("网格布局")]
    [Tooltip("网格中心（X/Z 为地面坐标，Y 为高度，比地面高 5）")]
    public Vector3 gridCenter = new Vector3(24.2f+387, 5f, 36.8f+199);
    [Tooltip("每行摆放数量")]
    public int columnCount = 10;
    [Tooltip("X 方向格子间距")]
    public float spacingX = 10f;
    [Tooltip("Z 方向格子间距（行距）")]
    public float spacingZ = 10f;

    [Header("导弹摆动")]
    [Tooltip("左右摆动幅度")]
    public float swingAmplitude = 4f;
    [Tooltip("左右摆动速度")]
    public float swingSpeed = 1.5f;

    private void Start()
    {
        ConfigManager.Init();

        // 按 EffectConfig 收集：先全部导弹、再全部特效（同一配置的导弹与特效分开摆放），各组内按 name 字母排序
        var missiles = new List<EffectItem>();
        var effects = new List<EffectItem>();
        foreach (var cfg in EffectConfig.ConfigList)
        {
            if (!string.IsNullOrEmpty(cfg.MissilePath))
                missiles.Add(new EffectItem("Prefabs/" + cfg.MissilePath, true, cfg.MissileScale, cfg.TintColor, cfg.Name));
        }
        foreach (var cfg in EffectConfig.ConfigList)
        {
            if (!string.IsNullOrEmpty(cfg.EffPath))
                effects.Add(new EffectItem("Prefabs/" + cfg.EffPath, false, cfg.Scale, cfg.TintColor, cfg.Name));
        }
        missiles.Sort(CompareByName);
        effects.Sort(CompareByName);

        var items = new List<EffectItem>(missiles.Count + effects.Count);
        items.AddRange(missiles);
        items.AddRange(effects);

        int total = items.Count;
        int rows = Mathf.CeilToInt((float)total / columnCount);
        int cols = Mathf.Min(columnCount, total);

        for (int i = 0; i < total; i++)
        {
            int col = i % columnCount;
            int row = i / columnCount;
            // 以 gridCenter 为中心向四周展开
            Vector3 pos = gridCenter + new Vector3(
                (col - (cols - 1) * 0.5f) * spacingX,
                0f,
                (row - (rows - 1) * 0.5f) * spacingZ);
            CreateItem(items[i].path, pos, items[i].isMissile, items[i].scale, items[i].tint);
        }

        GameLog.Info("EffectTestInit 创建特效/导弹共 " + total + " 个");
    }

    // 待创建的特效/导弹条目
    private class EffectItem
    {
        public string path;
        public bool isMissile;
        public float scale;
        public string tint;
        public string name;

        public EffectItem(string path, bool isMissile, float scale, string tint, string name)
        {
            this.path = path;
            this.isMissile = isMissile;
            this.scale = scale;
            this.tint = tint;
            this.name = name;
        }
    }

    // 按 name 字母排序
    private static int CompareByName(EffectItem a, EffectItem b)
    {
        return string.Compare(a.name, b.name, System.StringComparison.OrdinalIgnoreCase);
    }

    private void CreateItem(string resPath, Vector3 pos, bool isMissile, float scale, string tintHex)
    {
        var prefab = Resources.Load<GameObject>(resPath);
        if (prefab == null)
        {
            GameLog.Warn("EffectTestInit 资源不存在: " + resPath);
            return;
        }
        var go = Instantiate(prefab, pos, prefab.transform.rotation);
        // 缩放读 EffectConfig.Scale（当前配置统一为 5）
        go.transform.localScale = Vector3.one * scale;
        // 统一挂在 EffectTestInit 下，保持世界位置
        go.transform.SetParent(transform, true);
        // 配置了变色时自动挂染色脚本
        if (!string.IsNullOrEmpty(tintHex))
        {
            if (EffectTintColor.TryParseHex(tintHex, out var color))
            {
                var tint = go.GetComponent<EffectTintColor>();
                if (tint == null)
                    tint = go.AddComponent<EffectTintColor>();
                tint.SetTint(color);
            }
            else
            {
                GameLog.Warn("EffectTestInit 变色配置无效: " + resPath + " TintColor=" + tintHex);
            }
        }
        // 测试时循环播放，便于持续观察；同时把粒子缩放模式切到 Hierarchy，
        // 否则 prefab 多为 ScalingMode=Local（只认自身 localScale），根节点的 Scale/MissileScale 缩放对子节点粒子无效
        foreach (var ps in go.GetComponentsInChildren<ParticleSystem>(true))
        {
            ps.scalingMode = ParticleSystemScalingMode.Hierarchy;
            // MainModule 的 setter 直接写回原生粒子系统，无需写回 main 属性
            var main = ps.main;
            main.loop = true;
            ps.Play();
        }
        if (isMissile)
            StartCoroutine(Swing(go, pos, swingAmplitude, swingSpeed));
    }

    // 导弹左右往复摆动，露出拖尾
    private IEnumerator Swing(GameObject go, Vector3 basePos, float amplitude, float speed)
    {
        while (go != null)
        {
            float t = Time.time * speed;
            go.transform.position = basePos + new Vector3(Mathf.Sin(t) * amplitude, 0f, 0f);
            yield return null;
        }
    }

}
