// ============================================================
// 战斗模拟器 · Harness 辅助类型
// 空命名空间占位（using 不存在的命名空间不报错，这里仅兜底）
// 以及场景/UI 类的无头替身：HeroInfo / ChessHUD / GlowBeamController / LaserBeamController / EffectTintColor / CastleHUD
// ============================================================
using System.Collections.Generic;
using UnityEngine;

// 占位命名空间：桩程序集内留空即可，被 using 时无副作用
namespace UnityEngine.UI
{
}

namespace UnityEngine.EventSystems
{
}

namespace TMPro
{
}

namespace DG.Tweening
{
}

// 游戏 Combat/Skill 部分文件「using System.Buffers」但未实际使用类型 → 提供空命名空间占位
namespace System.Buffers
{
}

// ---- 无头替身：UIs/ 下未编译的类 ----

// 英雄头顶信息条：战斗代码只调用 SetHpRate/SetAttr
public class HeroInfo
{
    public void SetHpRate(int hp, int maxHp) { }
    public void SetAttr(int ap, int atk) { }
}

// 血条 HUD：仅 CreateHUD 使用
public class ChessHUD : MonoBehaviour
{
    public Chess chessUnit;

    public void UpdateHealthDisplay() { }
}

// 好友连线特效：仅 FriendLineManager.CreateFriendLine 使用，空实现
public class GlowBeamController : MonoBehaviour
{
    public void SetSourceAndTarget(Chess source, Chess target) { }
    public void SetGlowColor(Color color) { }
}

// 技能激光特效：SkillAidLastStrategy 使用。无头环境不渲染，只保留命中判定（朝向直接对准目标，不做缓转向）
public class LaserBeamController : MonoBehaviour
{
    public Chess source;
    public Chess target;
    public float length = 40f;
    public float hitWidth = 20f;

    public static LaserBeamController Spawn(Chess source, Chess target, float length, float hitWidth)
    {
        var go = new GameObject("LaserLine");
        var beam = go.AddComponent<LaserBeamController>();
        beam.source = source;
        beam.target = target;
        beam.length = length;
        beam.hitWidth = hitWidth;
        return beam;
    }

    public void SetTarget(Chess target)
    {
        this.target = target;
    }

    public void Dispose() { }

    // 命中粒子：无头环境不渲染
    public void PlayImpact(Chess hitTarget) { }

    public List<Chess> GetHitUnits()
    {
        var result = new List<Chess>();
        if (source == null || target == null)
            return result;

        var origin = source.transform.position;
        var dir = target.transform.position - origin;
        dir.y = 0f;
        if (dir.sqrMagnitude <= 0.0001f)
            return result;
        dir = dir.normalized;

        var half = hitWidth * 0.5f;
        foreach (var enemy in WorldManager.Instance.GetAllEnemys(source.side))
        {
            if (enemy == null || enemy.hp <= 0)
                continue;

            var rel = enemy.transform.position - origin;
            rel.y = 0f;
            var proj = Vector3.Dot(rel, dir);
            if (proj < 0f || proj > length)
                continue;
            if ((rel - dir * proj).sqrMagnitude <= half * half)
                result.Add(enemy);
        }
        return result;
    }
}

// 特效染色（Effect/EffectTintColor.cs 未编译）：仅 EffectManager.ApplyTint 使用，空实现
public class EffectTintColor : MonoBehaviour
{
    public static bool TryParseHex(string hex, out Color color)
    {
        color = Color.white;
        return !string.IsNullOrEmpty(hex);
    }

    public void SetTint(Color color) { }
}

// 城堡 HUD：仅 WorldManager.CreateCastleHUD 使用（未编译，无头空类）
public class CastleHUD : MonoBehaviour
{
    public void Init(PlayerInfo p, Transform center) { }
}
