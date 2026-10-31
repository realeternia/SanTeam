using UnityEngine;
using UnityEngine.EventSystems;
using CommonConfig;

// 背包卖出区：拖入英雄卡滑出英雄操作面板(SideHeroControl)，拖入道具滑出道具操作面板(SideItemControl)。
// 卖出/卸装/分解/改铸都在对应侧边栏内执行，本组件只负责识别拖入对象并打开面板
public class BagRecycler : MonoBehaviour, IDropHandler
{
    public BagControl bagControl;

    // 只有 "sell" 卖出区生效；卸装区/合成区保留在面板上但不响应拖拽
    public string mode = "sell";

    public void OnDrop(PointerEventData eventData)
    {
        if (bagControl == null)
        {
            GameLog.Warn("BagRecycler.OnDrop: 未绑定 BagControl，忽略拖放");
            return;
        }
        if (mode != "sell")
            return;

        GameObject draggedObject = eventData.pointerDrag;
        if (draggedObject == null)
            return;

        BagCell draggedCell = draggedObject.GetComponent<BagCell>();
        if (draggedCell == null)
            return;

        draggedCell.RemoveTagImg();

        if (ConfigManager.IsHeroCard(draggedCell.cardId))
            bagControl.OpenHeroPanel(draggedCell.cardId);
        else
            bagControl.OpenItemPanel(draggedCell.cardId);
    }
}
