using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CommonConfig;
using UnityEngine.EventSystems;
using System;

public class CardViewControl : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public int cardId;
    public int count;
    public bool isSold = false;
    public int priceI; //总价
    public bool isHeroCard;
    public Image soldImage;    
    public TMP_Text cardName;    
    public TMP_Text price;    
    public Button buyButton;    

    public GameObject isHeroCardNode;
    public GameObject isItemCardNode;

    //英雄卡相关
    public Image heroImage;
    public Image[] heroJobImage;
    public TMP_Text lead;
    public TMP_Text inte;
    public TMP_Text str;
    public TMP_Text hp;

    //物品卡相关
    public Image itemImage;
    public Image itemAttrImage1;
    public Image itemAttrImage2;
    public TMP_Text itemAttrName1;
    public TMP_Text itemAttrName2;
    public TMP_Text itemDes;

    public GameObject effectGreen;
    public GameObject effectYellow;
    public GameObject effectLayer;

    // Start is called before the first frame update
    void Start()
    {
        cardName.raycastTarget = false;
        if (isHeroCard)
        {
            lead.raycastTarget = false;
            inte.raycastTarget = false;
            str.raycastTarget = false;
            hp.raycastTarget = false;
        }

        buyButton.onClick.AddListener(() =>
        {
            CardShopManager.Instance.OnPlayerBuyCard(this, 0, cardId, isHeroCard, priceI, count);
        });
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (Tooltip.Instance != null)
        {
            Tooltip.Instance.HideTooltip();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log($"UI 元素被按下，位置：{eventData.position}");

        if (isHeroCard)
        {
            var heroCfg = HeroConfig.GetConfig(cardId);
            var friendInfo = ConfigManager.GetHeroFriendInfo(cardId);
            if (heroCfg.Skills != null && heroCfg.Skills.Length > 0 || friendInfo != null)
            { 
                Tooltip.Instance.ShowTooltip(heroCfg.Skills, friendInfo, cardId);
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Init(int cid, bool isHero, int count, int shopOpenIndex)
    {
        cardId = cid;
        isHeroCard = isHero;
        this.count = count;

        if (isHero)
        {
            isHeroCardNode.SetActive(true);
            isItemCardNode.SetActive(false);

            var heroCfg = HeroConfig.GetConfig(cid);
            heroImage.sprite = Resources.Load<Sprite>("SkinsBig/" + heroCfg.Icon);
            cardName.text = heroCfg.Name;
            if (count > 1)
                cardName.text += "x" + count;

            for (int i = 0; i < heroJobImage.Length; i++)
            {
                if (i < heroCfg.Skills.Length)
                {
                    heroJobImage[i].gameObject.SetActive(true);
                    heroJobImage[i].sprite = Resources.Load<Sprite>("SkillPic/" + SkillConfig.GetConfig(heroCfg.Skills[i]).Icon);
                }
                else
                {
                    heroJobImage[i].gameObject.SetActive(false);
                }

            }

            SetColoredText(lead, heroCfg.LeadShip);
            SetColoredText(inte, heroCfg.Inte);
            SetColoredText(str, heroCfg.Str);
            SetColoredText(hp, heroCfg.Hp);

            gameObject.GetComponent<Image>().color = HeroSelectionTool.GetSideColor(heroCfg.Side);
            priceI = HeroSelectionTool.GetPrice(heroCfg) * count;

            var playerInfo = GameManager.Instance.GetPlayer(0);
            if(playerInfo.HasCard(cardId))
            {
                effectGreen.SetActive(true);
                effectYellow.SetActive(false);
            }
            else if(playerInfo.HasFriend(cardId))
            {
                effectGreen.SetActive(false);
                effectYellow.SetActive(true);
            }
            else
            {
                effectGreen.SetActive(false);
                effectYellow.SetActive(false);
            }
        }
        else
        {
            isHeroCardNode.SetActive(false);
            isItemCardNode.SetActive(true);

            var itemCfg = ItemConfig.GetConfig(cid);
            cardName.text = itemCfg.Name;
            if (count > 1)
                cardName.text += "x" + count;
            itemImage.sprite = Resources.Load<Sprite>("ItemPic/" + itemCfg.Icon);
            if(!string.IsNullOrEmpty(itemCfg.Attr1))
            {
                itemAttrImage1.sprite = Resources.Load<Sprite>("attr" + itemCfg.Attr1);
                itemAttrName1.text = itemCfg.Attr1Val.ToString();
            }
            else
            {
                itemAttrImage1.gameObject.SetActive(false);
                itemAttrName1.gameObject.SetActive(false);

            }

            if(!string.IsNullOrEmpty(itemCfg.Attr2))
            {
                itemAttrImage2.sprite = Resources.Load<Sprite>("attr" + itemCfg.Attr2);
                itemAttrName2.text = itemCfg.Attr2Val.ToString();
            }
            else
            {
                itemAttrImage2.gameObject.SetActive(false);
                itemAttrName2.gameObject.SetActive(false);
            }

            if (!string.IsNullOrEmpty(itemCfg.Des))
            {
                itemDes.gameObject.SetActive(true);
                itemDes.text = itemCfg.Des;
            }
            else
            {
                itemDes.gameObject.SetActive(false);
            }

            priceI = itemCfg.Price * count + (int)Math.Floor(itemCfg.PriceRound * shopOpenIndex);

            var playerInfo = GameManager.Instance.GetPlayer(0);
            if(playerInfo.HasCard(cardId))
            {
                effectGreen.SetActive(true);
            }
            else
            {
                effectGreen.SetActive(false);
            }            
        }

        price.text = priceI.ToString();

    }

    private void SetColoredText(TMP_Text text, int value)
    {
        if (value >= 95)
        {
            text.color = Color.red;
        }
        else if (value >= 90)
        {
            text.color = Color.yellow;
        }

        text.text = value.ToString();
    }

    public void OnSold(PlayerInfo playerInfo)
    {
        isSold = true;
        buyButton.gameObject.SetActive(false);
        soldImage.gameObject.SetActive(true);

        if(effectGreen != null) //道具的情况
            effectGreen.SetActive(false);
        if(effectYellow != null) //道具的情况
            effectYellow.SetActive(false);

        //把heroImage变灰色 - 改为将整个panel变成灰度图
        SetGrayscaleEffect();
        soldImage.color = playerInfo.lineColor;

        //创建一个Image，启动携程 飞到 PlayerInfo的位置 
        StartCoroutine(MoveToPlayerInfo(playerInfo));
    }

    private void SetGrayscaleEffect()
    {
        // 获取所有Image组件并应用灰度效果
        Image[] allImages = GetComponentsInChildren<Image>(true);
        
        foreach (Image img in allImages)
        {
            if (img != null)
            {
                // 设置灰度颜色
                img.color = new Color(0.3f, 0.3f, 0.3f, img.color.a);
            }
        }
        
        // 获取所有TextMeshProUGUI组件并应用灰度效果
        TMP_Text[] allTMPTexts = GetComponentsInChildren<TMP_Text>(true);
        foreach (var tmpText in allTMPTexts)
        {
            if (tmpText != null)
            {
                // 设置TMP文本为灰色
                tmpText.color = Color.gray;
            }
        }
    }

    private System.Collections.IEnumerator MoveToPlayerInfo(PlayerInfo playerInfo)
    {
        // 创建一个新的Image对象并缓存
        var movingCardPrefab = Resources.Load<GameObject>("Prefabs/MovingCard");
        var movingCardImage = Instantiate(movingCardPrefab);
        Canvas canvas = FindObjectOfType<Canvas>();
        movingCardImage.transform.SetParent(canvas.transform, false);
        Image img = movingCardImage.GetComponent<Image>();
        img.sprite = isHeroCard ? heroImage.sprite : itemImage.sprite;

        // 获取Canvas的RectTransform
        RectTransform canvasRect = canvas.transform as RectTransform;

        // 计算起始位置：将当前卡片的屏幕坐标转换为Canvas局部坐标
        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(WorldManager.Instance.uiCamera, transform.position);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPoint, WorldManager.Instance.uiCamera, out Vector2 startLocalPos);
        
        // 计算目标位置：将PlayerInfo的屏幕坐标转换为Canvas局部坐标
        Vector2 targetScreenPoint = RectTransformUtility.WorldToScreenPoint(WorldManager.Instance.uiCamera, playerInfo.transform.position);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, targetScreenPoint, WorldManager.Instance.uiCamera, out Vector2 targetLocalPos);

        targetLocalPos += new Vector2(80, 0);

        // 设置初始位置
        movingCardImage.GetComponent<RectTransform>().anchoredPosition = startLocalPos;

        // 移动动画
        float duration = 0.7f; // 移动持续时间
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            // 使用平滑插值
            movingCardImage.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(startLocalPos, targetLocalPos, t);
            //逐渐缩小，最终缩小到50%
            img.rectTransform.sizeDelta = new Vector2(100, 140) * (1f - 0.5f * t);
            yield return null;
        }

        // 到达目标后销毁
        Destroy(movingCardImage);
        movingCardImage = null;
    }

    public void ShowEffectLayer(bool isShow)
    {
        effectLayer.SetActive(isShow);
    }

}
