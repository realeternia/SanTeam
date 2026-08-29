using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using CommonConfig;

public class BagFieldUnitControl : MonoBehaviour, IDropHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public int posId;
    public Image heroIcon;
    public Image jobIcon;
    public BagControl bagControl;
    public int myHeroId;

    private GameObject dragInstance;

    public void SetInfo(int id, int unitId)
    {
        myHeroId = unitId;
        posId = id;
        if (unitId == 0)
        {
            heroIcon.gameObject.SetActive(false);
            jobIcon.gameObject.SetActive(false);
        }
        else if (unitId == 500001 || unitId == 500002)
        {
            // 小兵格：近战显示bing1，远程显示bing2
            heroIcon.gameObject.SetActive(true);
            jobIcon.gameObject.SetActive(false);
            heroIcon.sprite = Resources.Load<Sprite>("Textures/" + (unitId == 500001 ? "bing1" : "bing2"));
        }
        else
        {
            heroIcon.gameObject.SetActive(true);
            jobIcon.gameObject.SetActive(true);
            var heroCfg = HeroConfig.GetConfig(unitId);
            heroIcon.sprite = Resources.Load<Sprite>("Skins/" + heroCfg.Icon);
            var skillCfgs = ConfigManager.GetHeroSkillConfigs(heroCfg);
            if (skillCfgs.Count > 0)
            {
                jobIcon.sprite = Resources.Load<Sprite>("SkillPic/" + skillCfgs[0].Icon);
            }
        }
    }

    // 当有物体拖放到此对象上时调用
    public void OnDrop(PointerEventData eventData)
    {       
        GameObject draggedObject = eventData.pointerDrag;
        if (draggedObject == null)
            return;

        // 从背包拖英雄布阵
        BagCell draggedCell = draggedObject.GetComponent<BagCell>();
        if (draggedCell != null && ConfigManager.IsHeroCard(draggedCell.cardId))
        {
            draggedCell.RemoveTagImg();
            bagControl.SetHeroForBattle(draggedCell.cardId, posId);
            return;
        }

        // 从布阵格拖单位（英雄/小兵）到此格：交换/移动位置
        BagFieldUnitControl draggedField = draggedObject.GetComponent<BagFieldUnitControl>();
        if (draggedField != null)
        {
            draggedField.RemoveDragIcon();
            bagControl.SwapFieldUnit(draggedField.posId, posId);
        }
    }

    // 开始拖动时调用
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (bagControl == null || bagControl.bindPlayer.isAI)
            return;
        if (myHeroId == 0)
            return;

        // 创建拖动时的预览对象
        dragInstance = new GameObject("FieldUnitDragIcon");
        dragInstance.transform.SetParent(GameObject.Find("Canvas").transform, false);
        dragInstance.transform.localScale = Vector3.one;

        Image dragImage = dragInstance.AddComponent<Image>();
        dragImage.sprite = GetUnitSprite(myHeroId);
        dragImage.rectTransform.sizeDelta = new Vector2(80, 80);
        dragImage.raycastTarget = false;
        dragInstance.transform.SetAsLastSibling();

        // 使当前对象半透明
        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        canvasGroup.alpha = 0.5f;
        canvasGroup.blocksRaycasts = false;
    }

    // 拖动过程中调用
    public void OnDrag(PointerEventData eventData)
    {
        if (dragInstance == null)
            return;

        Vector3 worldPosition;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(
            GameObject.Find("Canvas").GetComponent<RectTransform>(),
            Input.mousePosition,
            eventData.pressEventCamera,
            out worldPosition);

        dragInstance.transform.position = worldPosition;
    }

    // 结束拖动时调用
    public void OnEndDrag(PointerEventData eventData)
    {
        RemoveDragIcon();
    }

    public void OnDestroy()
    {
        RemoveDragIcon();
    }

    public void RemoveDragIcon()
    {
        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;
        }
        if (dragInstance != null)
        {
            Image dragImage = dragInstance.GetComponent<Image>();
            if (dragImage != null)
                dragImage.enabled = false;
            Destroy(dragInstance);
            dragInstance = null;
        }
    }

    private Sprite GetUnitSprite(int unitId)
    {
        if (unitId == 500001)
            return Resources.Load<Sprite>("Textures/bing1");
        if (unitId == 500002)
            return Resources.Load<Sprite>("Textures/bing2");
        var heroCfg = HeroConfig.GetConfig(unitId);
        return Resources.Load<Sprite>("Skins/" + heroCfg.Icon);
    }
}
