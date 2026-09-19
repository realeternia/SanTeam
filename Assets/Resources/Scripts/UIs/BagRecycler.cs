using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using CommonConfig;

public class BagRecycler : MonoBehaviour, IDropHandler
{
    public BagControl bagControl;

    // 功能模式："sell"=拖过来卖出，"unwear"=拖过来脱下该英雄所有装备进背包，"compose"=拖过来打开该道具的合成配方侧边栏
    public string mode = "sell";

    // 当有物体拖放到此对象上时调用
    public void OnDrop(PointerEventData eventData)
    {
        // 获取拖动的BagCell
        GameObject draggedObject = eventData.pointerDrag;
        if (draggedObject == null)
            return;

        BagCell draggedCell = draggedObject.GetComponent<BagCell>();
        if (draggedCell == null)
            return;

        draggedCell.RemoveTagImg();
        if (mode == "compose")
            bagControl.OpenComposePanel(draggedCell.cardId);
        else if (mode == "unwear")
            bagControl.UnwearHeroEquips(draggedCell.cardId);
        else
            bagControl.SellCard(draggedCell.cardId);
    }
}
