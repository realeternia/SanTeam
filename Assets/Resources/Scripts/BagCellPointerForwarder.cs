using UnityEngine;
using UnityEngine.EventSystems;

// 挂在 BagCell 的 cellButton 子物体上转发指针事件：
// PointerDown 事件从命中物体向上冒泡时，遇到子物体 Button（自带 IPointerDownHandler）即停止，
// 父物体 BagCell 的 OnPointerDown 收不到，因此需要此组件转发（drag 接口是向上查找实现者，不受影响）
public class BagCellPointerForwarder : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public BagCell bagCell;

    public void OnPointerDown(PointerEventData eventData)
    {
        bagCell.ShowTooltip();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        bagCell.HideTooltip();
    }
}
