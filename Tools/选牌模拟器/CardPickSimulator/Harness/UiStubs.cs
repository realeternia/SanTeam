// ============================================================
// 选牌模拟器 · UI 无头替身
// 为「零修改链接」的游戏商店/AI/PlayerInfo 源码提供最小 UI 类：
//   UnityEngine.UI (Image/Button)、TMPro (TMP_Text)、EventSystems、
//   DG.Tweening（飞卡动画空实现）、以及面板/提示等游戏 UI 类。
// 所有可视行为均为空实现/哑对象，只保证逻辑代码路径不因 UI 缺失而崩溃。
// ============================================================
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;
using UnityEngine.UI;

namespace UnityEngine.UI
{
    public class Graphic : MonoBehaviour
    {
        public Color color = Color.white;
        public bool raycastTarget = true;
        public Sprite sprite;
    }

    public class Image : Graphic
    {
    }

    public class ButtonClickedEvent
    {
        public void AddListener(System.Action action) { }
        public void RemoveListener(System.Action action) { }
        public void RemoveAllListeners() { }
    }

    public class Button : MonoBehaviour
    {
        public ButtonClickedEvent onClick = new ButtonClickedEvent();
        public bool interactable = true;
    }
}

namespace TMPro
{
    public class TMP_Text : MonoBehaviour
    {
        public string text = "";
        public Color color = Color.white;
        public bool raycastTarget = true;
    }

    public class TextMeshProUGUI : TMP_Text
    {
    }
}

namespace UnityEngine.EventSystems
{
    public class PointerEventData
    {
        public Vector2 position;
    }

    public interface IPointerDownHandler
    {
        void OnPointerDown(PointerEventData eventData);
    }

    public interface IPointerUpHandler
    {
        void OnPointerUp(PointerEventData eventData);
    }
}

namespace DG.Tweening
{
    public enum Ease
    {
        Linear = 0,
        InQuad = 1,
    }

    /// <summary>空补间：不产生动画，OnComplete 立即回调（用于飞卡对象的销毁）</summary>
    public class Tweener
    {
        public Tweener SetEase(Ease ease) { return this; }
        public Tweener OnComplete(System.Action callback) { callback?.Invoke(); return this; }
    }

    public static class ShortcutExtensions
    {
        public static Tweener DOAnchorPos(this RectTransform target, Vector2 endValue, float duration)
        {
            target.anchoredPosition = endValue;
            return new Tweener();
        }

        public static Tweener DOSizeDelta(this RectTransform target, Vector2 endValue, float duration)
        {
            target.sizeDelta = endValue;
            return new Tweener();
        }
    }
}

// ---- 游戏 UI 类无头替身（原本依赖 Unity 场景/预制体） ----

/// <summary>面板管理器替身：仅保留商店流程引用的成员（Tooltip/面板开关/信号）</summary>
public class PanelManager : MonoBehaviour
{
    public static PanelManager Instance;
    public GameObject cardShopPanel;
    public List<GameObject> openPanelList = new List<GameObject>();

    public T GetTooltip<T>() where T : BaseTooltip
    {
        return null;
    }

    public void ShowBag() { }
    public void ShowRank() { }
    public void ShowRankPlayer() { }
    public void ShowShop() { }
    public void HideShop() { }
    public void ShowPick() { }
    public void HidePick() { }
    public void SendSignal(string signalName, string arg, int pid) { }
}

/// <summary>操作提示替身：写日志即可</summary>
public class SystemTip : MonoBehaviour
{
    public static SystemTip Instance;

    public static void Show(string tip)
    {
        GameLog.Info("SystemTip: " + tip);
    }

    public void ShowTip(string tip) { }
}

/// <summary>背景音乐替身</summary>
public class BGMPlayer : MonoBehaviour
{
    // 直接给一个可用实例，避免 CardShopManager 调用时空引用
    public static BGMPlayer Instance = new BGMPlayer();

    public void PlaySound(string path) { }
}

/// <summary>当前选卡/羁绊展示区替身（无 UI）</summary>
public class MySelectControl : MonoBehaviour
{
    public void UpdateCards(PlayerInfo player) { }
    public void QuickView(PlayerInfo player) { }
    public void QuickViewFin() { }
}

/// <summary>选牌阶段点赞格替身（PlayerAI.CheckLike 依赖其字段）</summary>
public class PickPanelCellControl : MonoBehaviour
{
    public int heroId;
    public int likeState;
    public bool canLike;
    public Image forbidImg;

    public void SetLike(int pid) { }
}

/// <summary>Tooltip 基类替身</summary>
public class BaseTooltip : MonoBehaviour
{
    public void HideTooltip() { }
}

/// <summary>英雄 Tooltip 替身</summary>
public class TooltipHero : BaseTooltip
{
    // 技能分类图标（stXXXX）：与真机 TooltipHero 一致，卡面(CardViewControl)会调用
    private static readonly Dictionary<string, string> SkillClassIcons = new Dictionary<string, string>
    {
        { "单伤", "st0001" }, { "群伤", "st0002" }, { "自增", "st0003" }, { "单增", "st0004" },
        { "光环", "st0005" }, { "单控", "st0006" }, { "群增", "st0007" }, { "兵增", "st0008" },
        { "群控", "st0009" }, { "群疗", "st0010" }, { "单疗", "st0011" }, { "召唤", "st0012" },
    };

    public static string GetClassIcon(string skillType)
    {
        if (!string.IsNullOrEmpty(skillType) && SkillClassIcons.TryGetValue(skillType, out var icon))
            return icon;
        return null;
    }

    public void ShowTooltip(List<SkillConfig> skillConfigs, HashSet<int> friendInfo, int heroId,
        PlayerInfo player = null, bool isShopCard = false)
    {
    }
}

/// <summary>道具 Tooltip 替身</summary>
public class TooltipItem : BaseTooltip
{
    public void ShowTooltip(int cardId) { }
}
