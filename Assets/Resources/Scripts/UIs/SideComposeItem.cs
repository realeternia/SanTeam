using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CommonConfig;

// 侧边栏·配方格子：显示合成结果图标 + 两个材料图标，名字（结果道具名，按品质上色）与效果说明
// 材料不足的配方：名字/说明置灰、背景灰且不可选中
public class SideComposeItem : MonoBehaviour
{
    public Image BG;
    public TMP_Text itemName;
    public TMP_Text itemDesc;
    public Image itemResultIcon;
    public Image itemSrc1Icon;
    public Image itemSrc2Icon;

    public Button button;

    private bool isSelected = false;
    private bool isCombinable = true;
    private ItemCombineConfig recipe;
    private System.Action<SideComposeItem> onClickCallback;

    void Start()
    {
        if (button != null)
        {
            button.onClick.AddListener(OnItemClick);
        }
    }

    // canCombine=false 表示材料不足：格子置灰且不可选中
    // dropItemId 为玩家拖进合成区的道具，显示在第一个材料位（itemSrc1Icon）上
    public void SetData(ItemCombineConfig rcp, bool canCombine, int dropItemId)
    {
        if (rcp == null)
        {
            GameLog.Error("SideComposeItem.SetData: 配方为空");
            return;
        }
        recipe = rcp;
        isCombinable = canCombine;

        if (!ItemConfig.HasConfig(rcp.ResultId))
        {
            GameLog.Error($"SideComposeItem.SetData: 合成结果配置不存在 resultId={rcp.ResultId}");
            return;
        }
        var resultCfg = ItemConfig.GetConfig(rcp.ResultId);

        if (itemName != null)
        {
            itemName.text = resultCfg.Name;
            // 材料不足的配方名字同样置灰
            itemName.color = canCombine ? SysColor.GetQualityColor(resultCfg.Quality) : SysColor.Theme.DisabledTextColor;
        }

        if (itemDesc != null)
        {
            bool hasDesc = !string.IsNullOrEmpty(resultCfg.Des);
            itemDesc.gameObject.SetActive(hasDesc);
            itemDesc.text = hasDesc ? resultCfg.Des : "";
            itemDesc.color = canCombine ? Color.white : SysColor.Theme.DisabledTextColor;
        }

        // 拖入的道具固定放在第一个材料位，另一个材料放在第二位
        int src1Id = rcp.ItemA;
        int src2Id = rcp.ItemB;
        if (dropItemId == rcp.ItemB && rcp.ItemA != rcp.ItemB)
        {
            src1Id = rcp.ItemB;
            src2Id = rcp.ItemA;
        }

        SetIcon(itemResultIcon, rcp.ResultId);
        SetIcon(itemSrc1Icon, src1Id);
        SetIcon(itemSrc2Icon, src2Id);

        SetSelected(false);
    }

    // 图标按道具配置的 Icon 字段加载（Textures/ItemPic 下）
    private void SetIcon(Image icon, int itemId)
    {
        if (icon == null)
            return;
        if (!ItemConfig.HasConfig(itemId))
        {
            GameLog.Error($"SideComposeItem.SetIcon: ItemConfig 不存在 itemId={itemId}");
            icon.sprite = null;
            return;
        }
        icon.sprite = Resources.Load<Sprite>("Textures/ItemPic/" + ItemConfig.GetConfig(itemId).Icon);
    }

    public void SetOnClickCallback(System.Action<SideComposeItem> callback)
    {
        onClickCallback = callback;
    }

    public void OnItemClick()
    {
        onClickCallback?.Invoke(this);
    }

    public void SetSelected(bool selected)
    {
        isSelected = selected;
        if (BG != null)
        {
            if (isSelected)
                BG.color = SysColor.UI.MatchColor;
            else
                BG.color = isCombinable ? SysColor.Theme.CellNormalDark : SysColor.Theme.CellDisabled;
        }
    }

    public bool IsSelected()
    {
        return isSelected;
    }

    // 材料是否充足（不足时不可选中）
    public bool IsCombinable()
    {
        return isCombinable;
    }

    public ItemCombineConfig GetRecipe()
    {
        return recipe;
    }
}
