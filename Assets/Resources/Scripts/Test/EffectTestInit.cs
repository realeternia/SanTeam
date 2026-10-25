using System.Collections;
using System.Collections.Generic;
using System.IO;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 特效测试场景初始化：按 EffectConfig 遍历，把所有特效与导弹平铺创建出来。
/// 特效原地静止展示；导弹做左右往复摆动，方便观察拖尾。
/// 场景启动后自动逐个特效截图，文件名取特效名，保存到项目根目录下的截图目录。
/// </summary>
public class EffectTestInit : MonoBehaviour
{
    [Header("网格布局")]
    [Tooltip("网格中心（X/Z 为地面坐标，Y 为高度，比地面高 5）")]
    public Vector3 gridCenter = new Vector3(24.2f+387, 5f, 36.8f+199);
    [Tooltip("每行摆放数量")]
    public int columnCount = 13;
    [Tooltip("X 方向格子间距")]
    public float spacingX = 10f;
    [Tooltip("Z 方向格子间距（行距）")]
    public float spacingZ = 10f;

    [Header("导弹摆动")]
    [Tooltip("左右摆动幅度")]
    public float swingAmplitude = 4f;
    [Tooltip("左右摆动速度")]
    public float swingSpeed = 1.5f;

    [Header("截图")]
    [Tooltip("场景启动后是否自动逐个特效截图")]
    public bool autoScreenshot = true;
    [Tooltip("截图保存目录（相对项目根目录，空则存到 persistentDataPath/EffectScreenshots）")]
    public string screenshotDir = "EffectScreenshots";
    [Tooltip("全部特效创建完成后等待多少秒再开始截图（留出粒子扩散时间）")]
    public float captureStartDelay = 2f;
    [Tooltip("重启粒子后采集的时间窗（秒），窗内逐帧采样取最清晰的一帧（覆盖特效完整发展过程，喷火/烟雾类需要时间成型）")]
    public float captureSampleWindow = 1.2f;
    [Tooltip("采样间隔（秒）")]
    public float captureSampleInterval = 0.05f;
    [Tooltip("按距离发射的拖尾类特效静止时不出粒子，截图前会先平移这段距离（世界单位）")]
    public float captureWarmupDistance = 8f;
    [Tooltip("平移预热耗时（秒）")]
    public float captureWarmupSeconds = 0.4f;
    [Tooltip("截图宽度")]
    public int captureWidth = 256;
    [Tooltip("截图高度")]
    public int captureHeight = 256;
    [Tooltip("截图相机取景方向（从世界原点指向相机）")]
    public Vector3 captureViewDirection = new Vector3(0f, 0.35f, -1f);
    [Tooltip("截图背景色")]
    public Color captureBackground = new Color(0.08f, 0.08f, 0.1f, 1f);
    [Tooltip("放大系数：越大特效在画面里越大（1=按包围盒取景，>1 放大）")]
    public float captureZoom = 1.5f;
    [Tooltip("暗背景下几乎看不到内容时，改用该亮背景补拍一张（暗色半透明烟雾类特效在暗底上看不清）")]
    public Color captureAltBackground = new Color(0.7f, 0.7f, 0.72f, 1f);
    [Tooltip("截图专用层（临时把待截图特效切到该层，只让截图相机看到它，默认 31）")]
    public int captureLayer = 31;

    // 暗背景下内容像素少于此值时，认为特效在暗底上看不清，改用亮背景补拍
    private const int AltBackgroundContentPixels = 30;

    // 场景内创建的全部特效/导弹条目（含截图用信息）
    private readonly List<EffectItem> createdItems = new List<EffectItem>();
    // 截图期间冻结导弹摆动，避免目标跑出取景范围
    private bool capturing;

    private void Start()
    {
        ConfigManager.Init();

        // 按 EffectConfig 收集：先全部导弹、再全部特效（同一配置的导弹与特效分开摆放），各组内按 name 字母排序
        var missiles = new List<EffectItem>();
        var effects = new List<EffectItem>();
        foreach (var cfg in EffectConfig.ConfigList)
        {
            if (!string.IsNullOrEmpty(cfg.MissilePath))
            {
                // 同一配置同时有特效与导弹时，导弹文件名加后缀区分，避免同名覆盖
                string shotName = string.IsNullOrEmpty(cfg.EffPath) ? cfg.Name : cfg.Name + "_Missile";
                missiles.Add(new EffectItem("Prefabs/" + cfg.MissilePath, true, cfg.MissileScale, cfg.TintColor, cfg.Name, shotName));
            }
        }
        foreach (var cfg in EffectConfig.ConfigList)
        {
            if (!string.IsNullOrEmpty(cfg.EffPath))
                effects.Add(new EffectItem("Prefabs/" + cfg.EffPath, false, cfg.Scale, cfg.TintColor, cfg.Name, cfg.Name));
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
            var go = CreateItem(items[i].path, pos, items[i].scale, items[i].tint);
            items[i].go = go;
            if (items[i].isMissile && go != null)
                StartCoroutine(Swing(go, pos, swingAmplitude, swingSpeed));
        }

        createdItems.Clear();
        createdItems.AddRange(items);

        GameLog.Info("EffectTestInit 创建特效/导弹共 " + total + " 个");

        if (autoScreenshot && total > 0)
            StartCoroutine(CaptureAllEffects());
    }

    // 待创建的特效/导弹条目
    private class EffectItem
    {
        public string path;
        public bool isMissile;
        public float scale;
        public string tint;
        public string name;
        // 截图文件名（不含扩展名）
        public string shotName;
        // 创建后的实例
        public GameObject go;

        public EffectItem(string path, bool isMissile, float scale, string tint, string name, string shotName)
        {
            this.path = path;
            this.isMissile = isMissile;
            this.scale = scale;
            this.tint = tint;
            this.name = name;
            this.shotName = shotName;
        }
    }

    // 按 name 字母排序
    private static int CompareByName(EffectItem a, EffectItem b)
    {
        return string.Compare(a.name, b.name, System.StringComparison.OrdinalIgnoreCase);
    }

    private GameObject CreateItem(string resPath, Vector3 pos, float scale, string tintHex)
    {
        var prefab = Resources.Load<GameObject>(resPath);
        if (prefab == null)
        {
            GameLog.Warn("EffectTestInit 资源不存在: " + resPath);
            return null;
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
        return go;
    }

    // 截图前重启特效内的全部粒子系统，让爆发类特效从起始阶段重新播放
    private static void RestartParticles(GameObject go)
    {
        foreach (var ps in go.GetComponentsInChildren<ParticleSystem>(true))
        {
            // Stop 后再 Play 才会把时间轴回到 0（仅 Clear+Play 不会重置循环相位，
            // 一次性爆发/按爆发点发射的特效可能被清空后等不到下一次发射）
            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            ps.Play(true);
        }
        // 拖尾渲染器不会自己清空，残留的旧轨迹会干扰取景
        foreach (var trail in go.GetComponentsInChildren<TrailRenderer>(true))
            trail.Clear();
    }

    // 单张截图的一次采样结果
    private class ShotSample
    {
        public byte[] png;
        // 与背景明显不同的像素数（内容量）
        public int content = -1;
        // 与背景的总对比度（越大越清晰）
        public long contrast;
        // 该帧渲染包围盒的半径（越大说明特效越成型）
        public float radius;
    }

    // 在时间窗内逐帧渲染采样，收集每帧结果（内容量/对比度/包围盒尺寸），之后统一挑选
    private IEnumerator SampleEffect(Camera cam, RenderTexture rt, Texture2D tex, EffectItem item, Vector3 originPos, bool move, Color background, List<ShotSample> samples)
    {
        cam.backgroundColor = background;
        float window = Mathf.Max(0.05f, move ? captureWarmupSeconds : captureSampleWindow);
        float interval = Mathf.Max(0.01f, captureSampleInterval);
        float elapsed = 0f;
        float nextSample = interval;
        while (elapsed < window && item.go != null)
        {
            elapsed += Time.deltaTime;
            // 平移预热：按距离发射的粒子（拖尾类）只有移动时才出粒子
            if (move)
                item.go.transform.position = originPos + Vector3.right * (captureWarmupDistance * Mathf.Clamp01(elapsed / window));

            if (elapsed >= nextSample)
            {
                nextSample += interval;
                if (TryGetEffectBounds(item.go, out var bounds))
                {
                    FrameCaptureCamera(cam, bounds.center, bounds.extents.magnitude, item.scale);
                    RenderToTexture(cam, rt, tex);
                    var sample = new ShotSample();
                    sample.content = MeasureFrame(tex, background, out sample.contrast);
                    sample.radius = bounds.extents.magnitude;
                    sample.png = tex.EncodeToPNG();
                    samples.Add(sample);
                }
            }
            yield return null;
        }
        if (move && item.go != null)
            item.go.transform.position = originPos;
    }

    // 挑选最合适的一帧：只看尺寸接近窗内最大值的帧（特效已成型），再取对比度最高的一帧。
    // 否则喷火/喷烟这类特效刚出膛时只有一个小点，相机会贴上去放大，反而成为“最清晰”的一帧，与 Unity 里看到的一整串完全不同。
    private static ShotSample PickBest(List<ShotSample> samples)
    {
        const float sizeTolerance = 0.85f;
        float maxRadius = 0f;
        foreach (var s in samples)
            maxRadius = Mathf.Max(maxRadius, s.radius);
        ShotSample best = null;
        foreach (var s in samples)
        {
            if (s.radius < maxRadius * sizeTolerance)
                continue;
            if (best == null || s.contrast > best.contrast)
                best = s;
        }
        return best;
    }

    // 用截图相机渲染一帧到 RenderTexture，并读回 Texture2D
    private static void RenderToTexture(Camera cam, RenderTexture rt, Texture2D tex)
    {
        cam.targetTexture = rt;
        cam.Render();
        RenderTexture.active = rt;
        tex.ReadPixels(new Rect(0f, 0f, tex.width, tex.height), 0, 0);
        tex.Apply();
        RenderTexture.active = null;
        cam.targetTexture = null;
    }

    // 统计与背景色差异明显的像素数（返回值），并累加整体对比度（contrast）
    // content 用于判断这一帧是否拍到了内容；contrast 用于挑帧以及比较不同背景下哪种更清晰
    private static int MeasureFrame(Texture2D tex, Color bg, out long contrast)
    {
        int bgR = Mathf.RoundToInt(bg.r * 255f);
        int bgG = Mathf.RoundToInt(bg.g * 255f);
        int bgB = Mathf.RoundToInt(bg.b * 255f);
        const int visibleThreshold = 24;
        // 逐通道差异低于该值视为背景噪声，不计入对比度，避免整幅淡雾被噪声淹没
        const int noiseFloor = 6;
        var pixels = tex.GetPixels32();
        int w = tex.width;
        int h = tex.height;
        int count = 0;
        long sum = 0;
        // 隔像素采样即可，无需全图统计
        for (int y = 0; y < h; y += 2)
        {
            int row = y * w;
            for (int x = 0; x < w; x += 2)
            {
                var c = pixels[row + x];
                int d = Mathf.Max(Mathf.Abs(c.r - bgR), Mathf.Max(Mathf.Abs(c.g - bgG), Mathf.Abs(c.b - bgB)));
                if (d > noiseFloor)
                    sum += d - noiseFloor;
                if (d > visibleThreshold)
                    count++;
            }
        }
        contrast = sum;
        return count;
    }

    // 逐个特效截图：把目标临时切到独立层，用只认该层的截图相机渲染到 RenderTexture，存为 特效名.png
    private IEnumerator CaptureAllEffects()
    {
        yield return new WaitForSeconds(captureStartDelay);

        string dir = GetScreenshotDir();
        if (!Directory.Exists(dir))
        {
            try
            {
                Directory.CreateDirectory(dir);
            }
            catch (System.Exception e)
            {
                GameLog.Error("EffectTestInit 创建截图目录失败: " + dir + " (" + e.Message + ")");
                yield break;
            }
        }
        GameLog.Info("EffectTestInit 开始逐个特效截图，目录: " + dir);

        int layer = Mathf.Clamp(captureLayer, 0, 31);
        int width = Mathf.Max(16, captureWidth);
        int height = Mathf.Max(16, captureHeight);
        var camGo = new GameObject("EffectCaptureCamera");
        var cam = camGo.AddComponent<Camera>();
        cam.enabled = false;                       // 只手动 Render，不参与屏幕渲染
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = captureBackground;
        cam.cullingMask = 1 << layer;              // 只渲染临时切到该层的目标特效
        cam.fieldOfView = 35f;
        cam.allowHDR = false;
        cam.allowMSAA = false;
        cam.aspect = width / (float)height;

        var rt = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32);
        rt.Create();
        var tex = new Texture2D(width, height, TextureFormat.RGB24, false);
        var originalLayers = new List<KeyValuePair<GameObject, int>>();

        capturing = true;
        int done = 0;
        foreach (var item in createdItems)
        {
            if (item.go == null)
                continue;

            // 记录并临时切换层级，让背景/地面/其他特效不出现在截图里
            originalLayers.Clear();
            foreach (var t in item.go.GetComponentsInChildren<Transform>(true))
            {
                originalLayers.Add(new KeyValuePair<GameObject, int>(t.gameObject, t.gameObject.layer));
                t.gameObject.layer = layer;
            }

            // 重启粒子系统，保证截到的是特效的起始爆发阶段；
            // 否则可能正好落在爆发间隙或淡出末尾，拍出全黑图
            Vector3 originalPos = item.go.transform.position;
            RestartParticles(item.go);

            // 阶段1：静止采样，覆盖特效完整的发展过程
            var samples = new List<ShotSample>();
            yield return SampleEffect(cam, rt, tex, item, originalPos, false, captureBackground, samples);

            // 阶段2：拖尾类特效（按距离发射）静止时不出粒子，平移预热后再采样
            if (samples.Count == 0)
                yield return SampleEffect(cam, rt, tex, item, originalPos, true, captureBackground, samples);

            ShotSample best = PickBest(samples);

            // 暗背景上看不清（暗色半透明烟雾类），换亮背景补拍，取更清晰的一张（按对比度比较）
            if (best == null || best.content < AltBackgroundContentPixels)
            {
                var altSamples = new List<ShotSample>();
                yield return SampleEffect(cam, rt, tex, item, originalPos, false, captureAltBackground, altSamples);
                if (altSamples.Count == 0)
                    yield return SampleEffect(cam, rt, tex, item, originalPos, true, captureAltBackground, altSamples);
                var altBest = PickBest(altSamples);
                if (altBest != null && (best == null || altBest.contrast > best.contrast))
                    best = altBest;
            }

            if (item.go != null)
                item.go.transform.position = originalPos;

            string path = Path.Combine(dir, SanitizeFileName(item.shotName) + ".png");
            byte[] png = best != null ? best.png : null;
            if (png == null)
            {
                // 兜底：仍然没有任何可渲染内容，按配置缩放给个固定取景拍一张，便于排查
                GameLog.Warn("EffectTestInit 未取到渲染包围盒，使用默认取景: " + item.shotName);
                cam.backgroundColor = captureBackground;
                FrameCaptureCamera(cam, originalPos, item.scale * 2f, item.scale);
                RenderToTexture(cam, rt, tex);
                png = tex.EncodeToPNG();
            }
            else if (best.content == 0)
            {
                GameLog.Warn("EffectTestInit 截图内容为空（全背景色）: " + item.shotName);
            }

            try
            {
                File.WriteAllBytes(path, png);
                done++;
            }
            catch (System.Exception e)
            {
                GameLog.Error("EffectTestInit 截图保存失败: " + path + " (" + e.Message + ")");
            }

            // 还原层级
            foreach (var pair in originalLayers)
            {
                if (pair.Key != null)
                    pair.Key.layer = pair.Value;
            }
            yield return null;
        }
        capturing = false;

        cam.targetTexture = null;
        Object.Destroy(rt);
        Object.Destroy(tex);
        Object.Destroy(camGo);
        GameLog.Info("EffectTestInit 特效截图完成，共 " + done + " 张，目录: " + dir);
    }

    // 汇总特效实例下所有可见 Renderer 的世界包围盒
    private static bool TryGetEffectBounds(GameObject go, out Bounds bounds)
    {
        bounds = new Bounds(go.transform.position, Vector3.one);
        bool has = false;
        foreach (var r in go.GetComponentsInChildren<Renderer>(true))
        {
            if (!r.enabled || !r.gameObject.activeInHierarchy)
                continue;
            // 无粒子的粒子系统包围盒为 0，跳过以免把取景中心拉到原点
            if (r.bounds.size.sqrMagnitude < 0.0001f)
                continue;
            if (has)
                bounds.Encapsulate(r.bounds);
            else
            {
                bounds = r.bounds;
                has = true;
            }
        }
        return has;
    }

    // 让透视相机从固定方向框住目标球；radius 上限参考配置缩放，避免个别超大的包围盒把特效拍得极小
    private void FrameCaptureCamera(Camera cam, Vector3 center, float boundsRadius, float scaleRef)
    {
        float maxRadius = Mathf.Max(1.5f, scaleRef * 2.5f);
        float radius = Mathf.Clamp(Mathf.Min(boundsRadius, maxRadius), 1.5f, 64f);
        // 放大系数：半径越小，特效在画面里越大
        radius /= Mathf.Max(0.1f, captureZoom);
        Vector3 dir = captureViewDirection.sqrMagnitude > 0.0001f
            ? captureViewDirection.normalized
            : new Vector3(0f, 0.35f, -1f).normalized;

        float dist = radius / Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad) + radius;
        cam.transform.position = center - dir * dist;
        cam.transform.rotation = Quaternion.LookRotation(dir, Vector3.up);
        cam.nearClipPlane = Mathf.Max(0.05f, dist - radius * 2f);
        cam.farClipPlane = dist + radius * 4f;
    }

    // 截图目录：配置了相对路径则落在项目根目录下，否则存 persistentDataPath
    private string GetScreenshotDir()
    {
        if (string.IsNullOrEmpty(screenshotDir))
            return Path.Combine(Application.persistentDataPath, "EffectScreenshots");
        string projectRoot = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
        return Path.GetFullPath(Path.Combine(projectRoot, screenshotDir));
    }

    // 去掉文件名非法字符，避免特效名里的 / : 等导致保存失败
    private static string SanitizeFileName(string name)
    {
        if (string.IsNullOrEmpty(name))
            return "unnamed";
        char[] invalid = Path.GetInvalidFileNameChars();
        var sb = new System.Text.StringBuilder(name.Length);
        foreach (char c in name)
            sb.Append(System.Array.IndexOf(invalid, c) >= 0 ? '_' : c);
        return sb.ToString();
    }

    // 导弹左右往复摆动，露出拖尾
    private IEnumerator Swing(GameObject go, Vector3 basePos, float amplitude, float speed)
    {
        while (go != null)
        {
            // 截图期间冻结，避免目标持续移动导致取景偏移
            if (!capturing)
            {
                float t = Time.time * speed;
                go.transform.position = basePos + new Vector3(Mathf.Sin(t) * amplitude, 0f, 0f);
            }
            yield return null;
        }
    }

}
