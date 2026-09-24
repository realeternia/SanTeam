// ============================================================
// 战斗模拟器 · Harness 辅助类型
// 空命名空间占位（using 不存在的命名空间不报错，这里仅兜底）
// 以及场景/UI 类的无头替身：HeroInfo / ChessHUD / GlowBeamController / CastleHUD
// ============================================================
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

// 城堡 HUD：仅 WorldManager.CreateCastleHUD 使用（未编译，无头空类）
public class CastleHUD : MonoBehaviour
{
    public void Init(PlayerInfo p, Transform center) { }
}
