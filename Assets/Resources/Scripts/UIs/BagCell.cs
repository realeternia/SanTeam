using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public TMP_Text textItemName;
    public Image itemImage;
    public Image[] equipImages; // 最多3件装备槽，需在预制体上按槽位顺序拖入引用
    
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

        // PointerDown 会被 cellButton（子物体）拦截，需挂转发组件把按下/抬起事件转发过来
        var forwarder = cellButton.gameObject.GetComponent<BagCellPointerForwarder>();
        if (forwarder == null)
            forwarder = cellButton.gameObject.AddComponent<BagCellPointerForwarder>();
        forwarder.bagCell = this;
    }
    
    public void UpdateHeroInfo()
    {
        var heroCfg = HeroConfig.GetConfig(cardId);
        textItemName.text = heroCfg.Name + level;
        textItemName.color = SysColor.GetQualityColor(heroCfg.Quality);

        // 显示已装备的装备（最多3个槽位）
        bagControl.bindPlayer.itemEquips.TryGetValue(cardId, out var slots);
        if (equipImages != null)
        {
            for (int i = 0; i < equipImages.Length; i++)
            {
                bool has = slots != null && i < slots.Length && slots[i] != 0;
                equipImages[i].gameObject.SetActive(has);
                if (has)
                    equipImages[i].sprite = Resources.Load<Sprite>("ItemPic/" + ItemConfig.GetConfig(slots[i]).Icon);
            }
        }

        itemImage.sprite = Resources.Load<Sprite>("SkinsBig/" + heroCfg.Icon);

        expBar.rectTransform.sizeDelta = new Vector2(140 * HeroSelectionTool.GetExpRate(count, true), 20);
    }


    public void UpdateItemInfo()
    {
        // 装备升级机制已移除：物品不叠加格子，每格一个，无升级进度条
        var itemCfg = ItemConfig.GetConfig(cardId);
        itemImage.sprite = Resources.Load<Sprite>("ItemPic/" + itemCfg.Icon);

        // 有副本处于装备中时显示角标
        bool equipped = bagControl.bindPlayer.itemEquips.Values.Any(v => v != null && v.Contains(cardId));
        shieldImage.gameObject.SetActive(equipped);

        expBar.rectTransform.sizeDelta = new Vector2(0, 15);

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
        // 拖拽开始时隐藏 tooltip
        if (Tooltip.Instance != null)
            Tooltip.Instance.HideTooltip();

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
       // GameLog.Debug("RemoveTagImg");
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

    // 按住卡片显示 tooltip（由 cellButton 上的 BagCellPointerForwarder 转发调用）
    public void ShowTooltip()
    {
        if (Tooltip.Instance == null || bagControl == null || bagControl.bindPlayer == null)
            return;

        var player = bagControl.bindPlayer;

        if (ConfigManager.IsHeroCard(cardId))
        {
            var heroCfg = HeroConfig.GetConfig(cardId);
            var friendInfo = ConfigManager.GetHeroFriendInfo(cardId);
            Tooltip.Instance.ShowTooltip(ConfigManager.GetHeroSkillConfigs(heroCfg), friendInfo, cardId, player);
        }
        else
        {
            Tooltip.Instance.ShowTooltip(null, null, cardId, player);
        }
    }

    // 松开卡片隐藏 tooltip
    public void HideTooltip()
    {
        if (Tooltip.Instance != null)
        {
            Tooltip.Instance.HideTooltip();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
