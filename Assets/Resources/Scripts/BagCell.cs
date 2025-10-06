using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using CommonConfig;

public class BagCell : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    public int cardId;
    public int count;
    public int level;
    public TMP_Text textItemCount;
    public TMP_Text textItemName;
    public Image itemImage;
    public Image equipImage;
    public Image shieldImage;
    public Button cellButton;
    public BagControl bagControl;

    public Image expBar;

    private GameObject dragInstance;
    private Transform originalParent;
    private Vector3 originalPosition;
    
    // Start is called before the first frame update
    void Start()
    {
        cellButton.onClick.AddListener(() => bagControl.OnCellClick(this));
    }
    
    public void UpdateHeroInfo()
    {
        var heroCfg = HeroConfig.GetConfig(cardId);
        textItemCount.text = level.ToString();
        textItemName.text = heroCfg.Name;
        if (bagControl.bindPlayer.itemEquips.ContainsKey(cardId))
        {
            equipImage.gameObject.SetActive(true);
            equipImage.sprite = Resources.Load<Sprite>("ItemPic/" + ItemConfig.GetConfig(bagControl.bindPlayer.itemEquips[cardId]).Icon);
        }
        else
        {
            equipImage.gameObject.SetActive(false);
        }
        itemImage.sprite = Resources.Load<Sprite>("SkinsBig/" + heroCfg.Icon);

        expBar.rectTransform.sizeDelta = new Vector2(140 * HeroSelectionTool.GetExpRate(count, true), 20);
    }


    public void UpdateItemInfo()
    {
        textItemCount.text = level.ToString();
        var itemCfg = ItemConfig.GetConfig(cardId);
        itemImage.sprite = Resources.Load<Sprite>("ItemPic/" + itemCfg.Icon);

        if (bagControl.bindPlayer.itemEquips.ContainsValue(cardId))
        {
            shieldImage.gameObject.SetActive(true);
        }
        else
        {
            shieldImage.gameObject.SetActive(false);
        }

        expBar.rectTransform.sizeDelta = new Vector2(90 * HeroSelectionTool.GetExpRate(count, false), 15);

    }    


    public void OnSelect(bool isSelect)
    {
        if(isSelect)
        {
            cellButton.image.color = Color.green;
        }
        else
        {
            cellButton.image.color = Color.white;
        }
    }

    // 开始拖动时调用
    public void OnBeginDrag(PointerEventData eventData)
    {
        if(bagControl.bindPlayer.isAI)
            return;

        // 保存原始位置和父对象
        originalParent = transform.parent;
        originalPosition = transform.localPosition;
        
        // 创建拖动时的预览对象
        dragInstance = new GameObject("DragIcon");
        dragInstance.transform.SetParent(GameObject.Find("Canvas").transform, false); // 设置到Canvas下
        dragInstance.transform.localScale = Vector3.one;
        
        // 添加Image组件显示图标
        Image dragImage = dragInstance.AddComponent<Image>();
        if(ConfigManager.IsHeroCard(cardId))
        {
            var heroCfg = HeroConfig.GetConfig(cardId);
            dragImage.sprite = Resources.Load<Sprite>("Skins/" + heroCfg.Icon);
        }
        else
        {
            var effect = ItemConfig.GetConfig(cardId).Effect;
            if(effect != "attr" && effect != "tpattr") //无法装备
                return;
            dragImage.sprite = itemImage.sprite;
        }
        dragImage.rectTransform.sizeDelta = new Vector2(100, 100); // 适当放大图标以便于查看
        dragImage.raycastTarget = false; // 不阻挡射线检测
        
        // 确保拖动图标在最上层显示
        dragInstance.transform.SetAsLastSibling();
        
        // 使当前对象半透明
        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        canvasGroup.alpha = 0.5f;
        canvasGroup.blocksRaycasts = false;

        bagControl.OnCellClick(this);
    }

    // 拖动过程中调用
    public void OnDrag(PointerEventData eventData)
    {
        if (dragInstance == null)
            return;
        
        // 更新拖动实例的位置，跟随鼠标移动
        Vector3 worldPosition;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(
            GameObject.Find("Canvas").GetComponent<RectTransform>(),
            Input.mousePosition,
            eventData.pressEventCamera,
            out worldPosition);
        
        dragInstance.transform.position = worldPosition;
       // bagControl.OnCellClick(this);
    }

    // 结束拖动时调用
    public void OnEndDrag(PointerEventData eventData)
    {
        RemoveTagImg();
    }

    public void OnDestroy()
    {
        RemoveTagImg();
    }

    public void RemoveTagImg()
    {
       // UnityEngine.Debug.Log("RemoveTagImg");
            // 恢复对象的透明度和射线检测
        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;
        }
        
        // 确保拖动实例被正确销毁
        if (dragInstance != null)
        {
            // 立即隐藏拖动图标
            Image dragImage = dragInstance.GetComponent<Image>();
            if (dragImage != null)
                dragImage.enabled = false;
            
            // 销毁对象
            Destroy(dragInstance);
            // 重置引用
            dragInstance = null;
        }
    }

    // 当有物体拖放到此对象上时调用
    public void OnDrop(PointerEventData eventData)
    {
        // 只有英雄才能接收拖放
        if (!ConfigManager.IsHeroCard(cardId))
            return;
        
        // 获取拖动的BagCell
        GameObject draggedObject = eventData.pointerDrag;
        if (draggedObject == null)
            return;
        
        BagCell draggedCell = draggedObject.GetComponent<BagCell>();
        if (draggedCell == null || ConfigManager.IsHeroCard(draggedCell.cardId))
            return;
        
        // 调用装备方法，与equipBtn相同的功能
        RemoveTagImg();
        bagControl.EquipItemToHero(draggedCell.cardId, this.cardId);

        bagControl.OnCellClick(draggedCell);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
