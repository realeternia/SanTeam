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

        // 按 EffectConfig 遍历，先排全部导弹，再排全部特效（同一配置的导弹与特效分开摆放）
        var paths = new List<string>();
        var isMissileList = new List<bool>();
        var scaleList = new List<float>();
        foreach (var cfg in EffectConfig.ConfigList)
        {
            if (!string.IsNullOrEmpty(cfg.MissilePath))
            {
                paths.Add("Prefabs/" + cfg.MissilePath);
                isMissileList.Add(true);
                scaleList.Add(cfg.MissileScale);
            }
        }
        foreach (var cfg in EffectConfig.ConfigList)
        {
            if (!string.IsNullOrEmpty(cfg.EffPath))
            {
                paths.Add("Prefabs/" + cfg.EffPath);
                isMissileList.Add(false);
                scaleList.Add(cfg.Scale);
            }
        }

        int total = paths.Count;
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
            CreateItem(paths[i], pos, isMissileList[i], scaleList[i]);
        }

        GameLog.Info("EffectTestInit 创建特效/导弹共 " + total + " 个");
    }

    private void CreateItem(string resPath, Vector3 pos, bool isMissile, float scale)
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
        // 测试时循环播放，便于持续观察
        foreach (var ps in go.GetComponentsInChildren<ParticleSystem>(true))
        {
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
