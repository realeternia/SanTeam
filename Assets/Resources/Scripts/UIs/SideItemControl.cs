using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CommonConfig;

// 侧边栏·道具操作面板：展示道具图标/名称/说明，提供卖出；
// 存在可完成的合成配方时显示"合成"，持有分解锤且道具可分解时显示"分解"，持有西洋镜且道具可改铸时显示"改铸"
// 打开流程：背包卖出区(BagRecycler)拖入道具 → BagControl.OpenItemPanel → ShowSideBar("SideItemControl")
public class SideItemControl : MonoBehaviour
{
    public Image itemIcon;
    public TMP_Text itemName;
    public TMP_Text itemDes;
    public Button sellButton;
    public Button composeButton;
    public Button decomposeButton;
    public Button rerollButton;

    // 按钮纵向排布：第一个按钮的 y 坐标与按钮间距（隐藏的按钮不占位置）
    private const float ButtonStartY = -760f;
    private const float ButtonSpacing = 90f;

    private static BagControl bagControl;
    private static int currentItemId;

    /// <summary>
    /// 打开侧边栏前调用：bag 提供"可否卖出/分解/改铸"的判断与执行，itemId 为拖入卖出区的道具
    /// </summary>
    public static void SetContext(BagControl bag, int itemId)
    {
        bagControl = bag;
        currentItemId = itemId;

        GameLog.Info($"SideItemControl.SetContext: itemId={itemId}");
    }

    void Start()
    {
        if (sellButton != null)
            sellButton.onClick.AddListener(OnSell);
        if (composeButton != null)
            composeButton.onClick.AddListener(OnCompose);
        if (decomposeButton != null)
            decomposeButton.onClick.AddListener(OnDecompose);
        if (rerollButton != null)
            rerollButton.onClick.AddListener(OnReroll);

        RefreshView();
    }

    void RefreshView()
    {
        var itemCfg = ItemConfig.GetConfig(currentItemId);
        if (itemCfg == null)
        {
            GameLog.Error($"SideItemControl.RefreshView: ItemConfig 不存在 itemId={currentItemId}");
            return;
        }

        if (itemIcon != null)
            itemIcon.sprite = ItemIconLoader.Load(itemCfg.Icon);
        if (itemName != null)
        {
            itemName.text = itemCfg.Name;
            itemName.color = SysColor.GetQualityColor(itemCfg.Quality);
        }
        if (itemDes != null)
            itemDes.text = itemCfg.Des;

        if (bagControl == null)
        {
            GameLog.Error("SideItemControl.RefreshView: 未设置背包上下文，隐藏合成/分解/改铸按钮");
            SetButtonActive(composeButton, false);
            SetButtonActive(decomposeButton, false);
            SetButtonActive(rerollButton, false);
            ApplyButtonLayout();
            return;
        }

        // 合成/分解/改铸按钮按当前持有道具与条件动态显示
        SetButtonActive(composeButton, bagControl.CanComposeItem(currentItemId));
        SetButtonActive(decomposeButton, bagControl.CanDecomposeItem(currentItemId));
        SetButtonActive(rerollButton, bagControl.CanRerollItem(currentItemId));
        ApplyButtonLayout();
    }

    private void SetButtonActive(Button button, bool active)
    {
        if (button != null)
            button.gameObject.SetActive(active);
    }

    // 可见按钮从固定起点依次向下排布（隐藏的按钮不占位置），并统一染成可点击的金色
    private void ApplyButtonLayout()
    {
        float y = ButtonStartY;
        PlaceButton(sellButton, ref y);
        PlaceButton(composeButton, ref y);
        PlaceButton(decomposeButton, ref y);
        PlaceButton(rerollButton, ref y);
    }

    private void PlaceButton(Button button, ref float y)
    {
        if (button == null || !button.gameObject.activeSelf)
            return;

        if (button.transform is RectTransform rt)
            rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, y);
        y -= ButtonSpacing;
    }

    // 卖出1个该道具：先收起侧边栏，再由背包移除并按品质返还金币
    void OnSell()
    {
        if (bagControl == null)
        {
            GameLog.Warn("SideItemControl.OnSell: 未设置背包上下文，直接关闭侧边栏");
            PanelManager.Instance.HideSideBar();
            return;
        }

        GameManager.Instance.PlaySound("Sounds/click");
        PanelManager.Instance.HideSideBar(() => bagControl.SellItem(currentItemId));
    }

    // 合成：收起道具面板后打开合成配方面板，由玩家选择一条材料足够的配方
    void OnCompose()
    {
        if (bagControl == null)
        {
            GameLog.Warn("SideItemControl.OnCompose: 未设置背包上下文，直接关闭侧边栏");
            PanelManager.Instance.HideSideBar();
            return;
        }

        GameManager.Instance.PlaySound("Sounds/click");
        PanelManager.Instance.HideSideBar(() => bagControl.OpenComposeSelector(currentItemId));
    }

    // 分解：消耗1件该装备与1个分解锤，返还合成前的两件材料
    void OnDecompose()
    {
        if (bagControl == null)
        {
            GameLog.Warn("SideItemControl.OnDecompose: 未设置背包上下文，直接关闭侧边栏");
            PanelManager.Instance.HideSideBar();
            return;
        }

        GameManager.Instance.PlaySound("Sounds/click");
        PanelManager.Instance.HideSideBar(() => bagControl.DecomposeItem(currentItemId));
    }

    // 改铸：消耗1件该装备与1个西洋镜，随机产出同系列的另一件装备
    void OnReroll()
    {
        if (bagControl == null)
        {
            GameLog.Warn("SideItemControl.OnReroll: 未设置背包上下文，直接关闭侧边栏");
            PanelManager.Instance.HideSideBar();
            return;
        }

        GameManager.Instance.PlaySound("Sounds/click");
        PanelManager.Instance.HideSideBar(() => bagControl.RerollItem(currentItemId));
    }
}
