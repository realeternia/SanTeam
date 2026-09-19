using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using CommonConfig;

// 侧边栏·装备合成面板：列出与拖入道具相关的全部合成配方（每格=结果+两个材料，材料不足的置灰并排到最后）
// 选中一条配方后通过回调交回调用方执行合成
public class SideComposeSelector : MonoBehaviour
{
    public ScrollRect scrollRectMain;
    public GameObject subRegionMain;
    public SideComposeItem itemPrefab;
    public Button confirmButton;

    private const int MAX_SELECT_COUNT = 1;

    private List<SideComposeItem> selectedItems = new List<SideComposeItem>();

    private static SideComposeSelector instance;
    private static int currentPid;
    private static int currentItemId;
    private static System.Action<ItemCombineConfig> onRecipeSelected;

    /// <summary>
    /// 打开侧边栏前调用：pid 指定取哪名玩家的背包，itemId 为拖入的道具（列出与该道具相关的配方），
    /// callback 在点确定时收到选中的合成配方
    /// </summary>
    public static void SetContext(int pid, int itemId, System.Action<ItemCombineConfig> callback)
    {
        currentPid = pid;
        currentItemId = itemId;
        onRecipeSelected = callback;

        GameLog.Info($"SideComposeSelector.SetContext: pid={pid}, itemId={itemId}");

        if (instance != null)
            instance.LoadRecipeList();
    }

    void Start()
    {
        instance = this;

        if (confirmButton != null)
        {
            confirmButton.onClick.AddListener(OnConfirm);
        }

        LoadRecipeList();
    }

    void OnDestroy()
    {
        if (instance == this)
            instance = null;
    }

    void LoadRecipeList()
    {
        foreach (Transform child in subRegionMain.transform)
        {
            Destroy(child.gameObject);
        }

        selectedItems.Clear();

        var player = GameManager.Instance.GetPlayer(currentPid);
        List<ItemCombineConfig> recipes = GetRelatedRecipes(player);

        foreach (var recipe in recipes)
        {
            GameObject item = Instantiate(itemPrefab.gameObject, subRegionMain.transform);
            item.transform.localScale = Vector3.one;
            SideComposeItem itemCell = item.GetComponent<SideComposeItem>();
            itemCell.SetData(recipe, CanCombine(player, recipe), currentItemId);
            itemCell.SetOnClickCallback(OnItemSelected);
        }

        RectTransform subRect = subRegionMain.GetComponent<RectTransform>();
        RectTransform itemRect = itemPrefab.GetComponent<RectTransform>();

        if (subRect != null && itemRect != null)
        {
            subRect.sizeDelta = new Vector2(subRect.sizeDelta.x, itemRect.sizeDelta.y * recipes.Count);
        }

        if (scrollRectMain != null)
        {
            scrollRectMain.normalizedPosition = new Vector2(0, 1);
        }
    }

    // 与拖入道具相关的全部配方：材料充足的可合成配方排前面，材料不足的排后面
    List<ItemCombineConfig> GetRelatedRecipes(PlayerInfo player)
    {
        List<ItemCombineConfig> result = new List<ItemCombineConfig>();
        if (player == null)
        {
            GameLog.Error($"SideComposeSelector.GetRelatedRecipes: 玩家不存在 pid={currentPid}");
            return result;
        }

        return ItemCombineConfig.ConfigList
            .Where(rcp => rcp.ItemA == currentItemId || rcp.ItemB == currentItemId)
            .OrderBy(rcp => CanCombine(player, rcp) ? 0 : 1)
            .ThenBy(rcp => rcp.ResultId)
            .ToList();
    }

    // 配方是否可合成：两个材料（未装备的可用副本）数量都满足配置需求
    bool CanCombine(PlayerInfo player, ItemCombineConfig rcp)
    {
        return player.GetItemFreeCount(rcp.ItemA) >= rcp.ItemAcount
            && player.GetItemFreeCount(rcp.ItemB) >= rcp.ItemBcount;
    }

    void OnItemSelected(SideComposeItem item)
    {
        // 材料不足的配方置灰不可选
        if (!item.IsCombinable())
            return;

        if (item.IsSelected())
        {
            item.SetSelected(false);
            selectedItems.Remove(item);
            return;
        }

        // 单选：已选满则把上一次的选择清掉，改为选中当前格子
        while (selectedItems.Count >= MAX_SELECT_COUNT)
        {
            SideComposeItem old = selectedItems[0];
            old.SetSelected(false);
            selectedItems.RemoveAt(0);
        }

        item.SetSelected(true);
        selectedItems.Add(item);
    }

    void OnConfirm()
    {
        if (selectedItems.Count == 0)
        {
            GameLog.Warn("SideComposeSelector.OnConfirm: 未选中任何配方");
            return;
        }

        // 合成与关闭侧边栏都由回调方负责（先收侧边栏，再在背包里播合成动画）
        if (onRecipeSelected == null)
        {
            GameLog.Warn("SideComposeSelector.OnConfirm: 未设置合成回调，直接关闭");
            PanelManager.Instance.HideSideBar();
            return;
        }

        onRecipeSelected.Invoke(selectedItems[0].GetRecipe());
    }
}
