using UnityEngine;

// Tooltip 基类：统一管理"同一时刻只显示一个提示"与"贴边不出显示区域"的定位逻辑。
// 派生类只负责填充内容并设置 rect.sizeDelta，然后调用 Show() 显示、HideTooltip() 隐藏。
// 定位统一在根 Canvas 局部空间（与 sizeDelta 同单位）计算，边界用 canvasRect.rect，
// 再换算到父节点局部坐标，避免依赖父节点（如 TipNode）的锚点/尺寸配置。
public abstract class BaseTooltip : MonoBehaviour
{
    public RectTransform rect;

    // 当前正在显示的提示（同一时刻只显示一个，新的显示会把旧的隐藏）
    private static BaseTooltip current;

    // 整体缩放：图标、字体、背景一起等比放大
    protected const float UIScale = 1.3f;

    // 贴边边距
    private const float EdgeMargin = 10f;   // 左右/上边距
    private const float BottomMargin = 40f; // 底部保留边距
    private const float GapX = 20f;         // 与点击点的水平间隔

    protected virtual void Awake()
    {
        if (rect == null)
            rect = (RectTransform)transform;

        // 统一pivot/anchor为父物体中心，保证局部坐标计算与实际渲染位置一致
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.localScale = Vector3.one * UIScale;
    }

    // 显示提示：先隐藏当前已显示的其它提示，再按鼠标位置贴边定位
    protected void Show()
    {
        if (current != null && current != this)
            current.HideTooltip();
        current = this;
        gameObject.SetActive(true);
        PositionAtMouse();
    }

    // 隐藏提示
    public void HideTooltip()
    {
        if (current == this)
            current = null;
        gameObject.SetActive(false);
    }

    // 统一贴边定位：默认显示在鼠标右侧，右侧超出翻到左侧，左侧仍超出则贴左边界；
    // 垂直以鼠标为中心上下夹紧保证完整可见（底部留边距）。
    private void PositionAtMouse()
    {
        RectTransform canvasRect = transform.root as RectTransform;
        if (canvasRect == null)
            return;

        float baseWidth = rect.sizeDelta.x;
        float baseHeight = rect.sizeDelta.y;
        float viewWidth = canvasRect.rect.width;
        float viewHeight = canvasRect.rect.height;

        // 动态整体缩放：默认放大30%；若整体高度超过可视高度，则缩小到刚好撑满（留5%边距）
        float scale = UIScale;
        if (baseHeight * scale > viewHeight * 0.95f)
            scale = viewHeight * 0.95f / baseHeight;
        rect.localScale = Vector3.one * scale;

        // 缩放后的实际宽高
        float tooltipWidth = baseWidth * scale;
        float tooltipHeight = baseHeight * scale;

        // 鼠标位置转为根 Canvas 局部坐标
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect, Input.mousePosition, WorldManager.Instance.uiCamera, out localPoint);

        float halfW = viewWidth * 0.5f;
        float halfH = viewHeight * 0.5f;

        // 水平定位（触摸屏：tips不挡点击点）：默认从点击位置右边开始显示（左边贴点击点）
        float centerX = localPoint.x + GapX + tooltipWidth * 0.5f;
        // 右侧超出可视区：翻到点击位置左边显示（右边贴点击点）
        if (centerX + tooltipWidth * 0.5f > halfW - EdgeMargin)
            centerX = localPoint.x - GapX - tooltipWidth * 0.5f;
        // 左侧也超出（点击点太靠左）：夹在左边界内
        if (centerX - tooltipWidth * 0.5f < -halfW + EdgeMargin)
            centerX = -halfW + tooltipWidth * 0.5f + EdgeMargin;

        // 垂直定位：以点击点为中心，上下夹紧保证完整可见（底部留边距40）
        float centerY = localPoint.y;
        if (centerY - tooltipHeight * 0.5f < -halfH + BottomMargin)
            centerY = -halfH + tooltipHeight * 0.5f + BottomMargin;
        if (centerY + tooltipHeight * 0.5f > halfH - EdgeMargin)
            centerY = halfH - tooltipHeight * 0.5f - EdgeMargin;

        // 根 Canvas 局部坐标 → 世界 → 父节点局部坐标，避免依赖父节点（TipNode）锚点/尺寸
        Vector3 worldCenter = canvasRect.TransformPoint(centerX, centerY, 0);
        rect.localPosition = rect.parent.InverseTransformPoint(worldCenter);
    }
}
