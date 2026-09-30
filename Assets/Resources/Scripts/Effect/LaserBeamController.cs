using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 技能激光控制器（配合预制体 Prefabs/Battles/LaserLine）：
/// 与点到点的 GlowBeamController 不同，本控制器从发射源沿固定长度射出一道激光，
/// SetTarget 设置目标后按 turnSpeed 缓慢转向（而非瞬间对准），
/// 并通过 GetHitUnits 返回当前被激光命中的敌方单位列表供技能结算。
/// 挂载方式与 GlowBeamController 一致：挂在光束子节点(Beam)上，parentTransform 指向激光根节点。
/// </summary>
[RequireComponent(typeof(Renderer))]
public class LaserBeamController : MonoBehaviour
{
    // 激光预制体路径（相对 Resources）
    public const string PrefabPath = "Prefabs/Battles/LaserLine";

    [Header("Beam Settings")]
    public Color beamColor = Color.white;
    public Color glowColor = Color.cyan;
    [Range(0.1f, 5f)]
    public float beamWidth = 1f;
    [Range(0f, 10f)]
    public float glowIntensity = 3f;
    [Range(0f, 5f)]
    public float scrollSpeed = 1f;
    [Range(0f, 1f)]
    public float opacity = 0.8f;

    [Header("Texture Settings")]
    public Texture2D beamTexture;
    [Range(0.1f, 10f)]
    public float noiseScale = 3f;

    [Header("Laser Settings")]
    public Transform parentTransform;   // 激光根节点（承载位置/旋转/缩放）
    public Chess source;                // 发射源（施法者）
    public Chess target;                // 当前照射目标
    public float length = 40f;          // 激光长度（世界单位，固定距离）
    public float hitWidth = 20f;        // 命中判定宽度（世界单位）
    public float turnSpeed = 120f;      // 转向速度（度/秒），目标切换后缓慢转过去
    public float crossScale = 0.3f;     // 激光截面缩放（Y/Z 轴）
    public float pulseSpeed = 7f;       // 能量脉动速度（核心粗细与强度呼吸）
    public float pulseAmp = 0.3f;       // 能量脉动幅度（0~1，越大"缩放"感越明显）
    public float tipBoost = 1.5f;       // 末端能量堆积（命中端更亮）

    [Header("Impact Settings")]
    public string impactEffect = "LaserImpactSpark"; // 命中目标时播放的粒子特效（EffectConfig 特效名，留空则不播）

    private Material material;
    private Vector3 curDir = Vector3.right; // 当前激光方向（世界空间，仅 xz 平面）

    void Awake()
    {
        RemoveLegacyController();
    }

    // LaserLine 由 FriendLink 复制而来，预制体上可能仍挂有点到点的 GlowBeamController，
    // 它会在 source/target 为空时销毁整个特效对象，这里先禁用再清理，避免激光刚生成就被销毁
    void RemoveLegacyController()
    {
        foreach (var legacy in GetComponentsInChildren<GlowBeamController>(true))
        {
            legacy.enabled = false;
            Destroy(legacy);
        }
    }

    /// <summary>
    /// 初始化激光：设置发射源、初始目标、长度与判定宽度（长度/宽度按技能配置的世界单位值传入）
    /// </summary>
    public void Init(Chess source, Chess target, float length, float hitWidth)
    {
        this.source = source;
        this.target = target;
        this.length = length;
        this.hitWidth = hitWidth;

        if (parentTransform == null)
            parentTransform = transform.parent != null ? transform.parent : transform;

        var beamRenderer = GetComponent<Renderer>();
        if (beamRenderer != null)
            material = beamRenderer.material;

        // 初始朝向直接对准起始目标（初次出现不摆动），后续目标切换由 SetTarget 缓慢转向
        var dir = DesiredDir();
        if (dir.sqrMagnitude > 0.0001f)
        {
            curDir = dir;
        }
        else
        {
            var forward = source != null ? source.transform.forward : Vector3.forward;
            forward.y = 0f;
            curDir = forward.sqrMagnitude > 0.0001f ? forward.normalized : Vector3.right;
        }

        UpdateBeamProperties();
        UpdateBeamTransform();
    }

    void Update()
    {
        if (source == null || source.hp <= 0)
        {
            Dispose();
            return;
        }

        // 缓慢转向当前目标
        if (target != null)
        {
            var dir = DesiredDir();
            if (dir.sqrMagnitude > 0.0001f)
                curDir = Vector3.RotateTowards(curDir, dir, turnSpeed * Mathf.Deg2Rad * Time.deltaTime, 0f);
        }

        UpdateBeamTransform();
    }

    // 从发射源指向当前目标的方向（仅取 xz 平面，y 轴不参与激光走向）
    Vector3 DesiredDir()
    {
        if (source == null || target == null)
            return Vector3.zero;

        var dir = target.transform.position - source.transform.position;
        dir.y = 0f;
        return dir.sqrMagnitude > 0.0001f ? dir.normalized : Vector3.zero;
    }

    void UpdateBeamProperties()
    {
        if (material == null)
            return;

        material.SetColor("_Color", beamColor);
        material.SetColor("_GlowColor", glowColor);
        material.SetFloat("_BeamWidth", beamWidth);
        material.SetFloat("_GlowIntensity", glowIntensity);
        material.SetFloat("_Speed", scrollSpeed);
        material.SetFloat("_Opacity", opacity);
        material.SetFloat("_NoiseScale", noiseScale);
        material.SetFloat("_PulseSpeed", pulseSpeed);
        material.SetFloat("_PulseAmp", pulseAmp);
        material.SetFloat("_TipBoost", tipBoost);

        if (beamTexture != null)
            material.SetTexture("_MainTex", beamTexture);
    }

    void UpdateBeamTransform()
    {
        if (source == null || parentTransform == null)
            return;

        // 激光以发射源为起点沿 curDir 延伸 length，根节点放在激光中点。
        // 单位根节点多为非等比缩放，按发射源 X 轴（足迹）做标量补偿，保证激光长度恒为配置的世界长度
        var emitter = parentTransform.parent;
        var parentScaleX = emitter != null ? emitter.lossyScale.x : 1f;
        var lengthFactor = parentScaleX != 0f ? length / parentScaleX : length;

        parentTransform.position = source.transform.position + curDir * (length * 0.5f);
        parentTransform.rotation = Quaternion.LookRotation(curDir) * Quaternion.Euler(0f, 90f, 0f);
        parentTransform.localScale = new Vector3(lengthFactor, crossScale, crossScale);
    }

    /// <summary>
    /// 设置/切换照射目标：激光会在后续帧按 turnSpeed 缓慢转向新目标，而不是瞬间对准
    /// </summary>
    public void SetTarget(Chess target)
    {
        this.target = target;
    }

    /// <summary>
    /// 返回当前被激光命中的敌方单位列表：以发射源为起点、curDir 为方向，
    /// 正投影落在 [0, length] 内且垂直距离不超过 hitWidth/2 的敌方单位
    /// </summary>
    public List<Chess> GetHitUnits()
    {
        var result = new List<Chess>();
        if (source == null)
            return result;

        var origin = source.transform.position;
        var half = hitWidth * 0.5f;
        foreach (var enemy in WorldManager.Instance.GetAllEnemys(source.side))
        {
            if (enemy == null || enemy.hp <= 0)
                continue;

            var rel = enemy.transform.position - origin;
            rel.y = 0f;
            var proj = Vector3.Dot(rel, curDir);
            if (proj < 0f || proj > length)
                continue;
            if ((rel - curDir * proj).sqrMagnitude <= half * half)
                result.Add(enemy);
        }
        return result;
    }

    /// <summary>
    /// 销毁整个激光实例：控制器挂在光束子节点(Beam)上，需向上找到激光根节点一并销毁，
    /// 否则会残留一个空的根节点挂在施法者下
    /// </summary>
    public void Dispose()
    {
        var root = parentTransform != null && parentTransform != transform ? parentTransform.gameObject : gameObject;
        Destroy(root);
    }

    /// <summary>
    /// 在命中目标处播放一次粒子特效（每次伤害结算调用，形成命中火花；impactEffect 留空则不播）
    /// </summary>
    public void PlayImpact(Chess hitTarget)
    {
        if (string.IsNullOrEmpty(impactEffect) || hitTarget == null || source == null)
            return;

        EffectManager.PlayHitEffect(source, hitTarget, impactEffect);
    }

    /// <summary>
    /// 设置发光颜色（沿用 GlowBeamController 的接口，便于按阵营/技能配色）
    /// </summary>
    public void SetGlowColor(Color color)
    {
        glowColor = color;
        UpdateBeamProperties();
    }

    /// <summary>
    /// 创建一道激光：加载 LaserLine 预制体挂到施法者下并初始化；
    /// 资源缺失时记录日志并返回 null（调用方需做空判断）
    /// </summary>
    public static LaserBeamController Spawn(Chess source, Chess target, float length, float hitWidth)
    {
        if (source == null)
            return null;

        var prefab = Resources.Load<GameObject>(PrefabPath);
        if (prefab == null)
        {
            GameLog.Warn("LaserBeamController 预制体缺失: " + PrefabPath);
            return null;
        }

        var instance = Object.Instantiate(prefab, Vector3.zero, Quaternion.identity);
        instance.transform.SetParent(source.transform);
        instance.transform.localScale = Vector3.one;

        var beamRenderer = instance.GetComponentInChildren<Renderer>();
        if (beamRenderer == null)
        {
            GameLog.Warn("LaserBeamController 预制体缺少 Renderer: " + PrefabPath);
            Object.Destroy(instance);
            return null;
        }

        // 控制器挂在光束子节点上（与旧 GlowBeamController 一致），预制体未挂时运行时补上
        var beam = beamRenderer.GetComponent<LaserBeamController>();
        if (beam == null)
            beam = beamRenderer.gameObject.AddComponent<LaserBeamController>();

        beam.parentTransform = beamRenderer.transform.parent != null ? beamRenderer.transform.parent : beamRenderer.transform;
        beam.Init(source, target, length, hitWidth);
        return beam;
    }
}
