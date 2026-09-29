using UnityEngine;

/// <summary>
/// 粒子染色脚本：用 tintColor 变量把本物体（含子级）所有 ParticleSystem 的颜色改为目标色。
/// 同时覆盖起始颜色(startColor)与 Color by Lifetime(colorOverLifetime)，保留各颜色关键点的 alpha。
/// 用于 MagicChargeYellow / MagicChargePink 这类仅换色的特效变体，避免各自维护一套粒子设置。
/// tintColor 的 alpha ≤ 0 表示不改色。
/// </summary>
public class EffectTintColor : MonoBehaviour
{
    /// <summary>
    ///目标颜色（alpha=0 表示不改色；白色也是合法目标色，如白色暴击变体）
    /// </summary>
    public Color tintColor = Color.white;

    private void Awake()
    {
        Apply();
    }

    /// <summary>
    ///把本物体下所有 ParticleSystem 染成目标色（保留原 alpha）
    /// </summary>
    public void Apply()
    {
        // alpha ≤ 0 视为未配置，跳过；白色也是合法目标色（如白色暴击变体）
        if (tintColor.a <= 0f)
            return;
        var systems = GetComponentsInChildren<ParticleSystem>(true);
        foreach (var ps in systems)
        {
            TintStartColor(ps);
            TintColorOverLifetime(ps);
        }
    }

    /// <summary>
    ///设置目标颜色并立即染色（用于创建时按配置色初始化，避免先 Awake 后赋值的时序问题）
    /// </summary>
    public void SetTint(Color color)
    {
        tintColor = color;
        Apply();
    }

    /// <summary>
    ///解析颜色配置串（支持 #RRGGBB / RRGGBB，可带 AA 透明度），失败返回 false 并输出白色
    /// </summary>
    public static bool TryParseHex(string hex, out Color color)
    {
        color = Color.white;
        if (string.IsNullOrEmpty(hex))
            return false;
        string h = hex.Trim().TrimStart('#');
        if (h.Length != 6 && h.Length != 8)
            return false;
        return ColorUtility.TryParseHtmlString("#" + h, out color);
    }

    // 起始颜色：换成目标色（保留原 alpha）
    private void TintStartColor(ParticleSystem ps)
    {
        var main = ps.main;
        var sc = main.startColor;
        main.startColor = new ParticleSystem.MinMaxGradient(
            new Color(tintColor.r, tintColor.g, tintColor.b, sc.color.a));
    }

    // Color by Lifetime：颜色关键点 RGB 全部换成目标色（保留各自 alpha）
    private void TintColorOverLifetime(ParticleSystem ps)
    {
        var module = ps.colorOverLifetime;
        if (!module.enabled)
            return;
        var col = module.color;
        switch (col.mode)
        {
            case ParticleSystemGradientMode.Color:
                module.color = new ParticleSystem.MinMaxGradient(
                    new Color(tintColor.r, tintColor.g, tintColor.b, col.color.a));
                break;
            case ParticleSystemGradientMode.TwoColors:
                module.color = new ParticleSystem.MinMaxGradient(
                    new Color(tintColor.r, tintColor.g, tintColor.b, col.colorMin.a),
                    new Color(tintColor.r, tintColor.g, tintColor.b, col.colorMax.a));
                break;
            default: // Gradient / TwoGradients
                var grad = col.gradient;
                var colorKeys = grad.colorKeys;
                for (int i = 0; i < colorKeys.Length; i++)
                {
                    var c = colorKeys[i].color;
                    colorKeys[i].color = new Color(tintColor.r, tintColor.g, tintColor.b, c.a);
                }
                var newGrad = new Gradient();
                newGrad.SetKeys(colorKeys, grad.alphaKeys);
                module.color = new ParticleSystem.MinMaxGradient(newGrad);
                break;
        }
    }
}
