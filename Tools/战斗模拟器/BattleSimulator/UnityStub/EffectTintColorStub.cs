// ============================================================
// 战斗模拟器 · EffectTintColor 桩
// 游戏里 EffectManager(Combat/*) 会引用 Scripts/Effect/EffectTintColor（粒子染色）。
// 该组件依赖 ParticleSystem/Gradient，属 Unity 粒子特性，headless 无需实现。
// 这里用最小桩满足编译，让 EffectManager.cs 能通过 harness 桥接编译。
// ============================================================
using UnityEngine;

/// <summary>
/// 桩版粒子染色组件：仅保留 EffectManager 用到的接口签名，实机特效染色交给真 Unity 组件。
/// </summary>
public class EffectTintColor : MonoBehaviour
{
    /// <summary>目标颜色（alpha<=0 表示不改色）</summary>
    public Color tintColor = Color.white;

    /// <summary>染色入口（headless 空实现）</summary>
    public void Apply()
    {
    }

    /// <summary>设置目标色并立即染色（headless 空实现）</summary>
    public void SetTint(Color color)
    {
        tintColor = color;
    }

    /// <summary>解析颜色配置串（支持 #RRGGBB / RRGGBB，可带 AA 透明度）</summary>
    public static bool TryParseHex(string hex, out Color color)
    {
        color = Color.white;
        return false;
    }
}