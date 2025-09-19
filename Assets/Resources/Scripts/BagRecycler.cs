using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using CommonConfig;

public class BagRecycler : MonoBehaviour, IDropHandler
{
    public BagControl bagControl;

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
        
        // 调用装备方法，与equipBtn相同的功能
        draggedCell.RemoveTagImg();
        bagControl.SellCard(draggedCell.cardId);
    }
}