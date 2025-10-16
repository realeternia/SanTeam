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
    public int priceI; //单价
    public bool isHeroCard;
    public Image soldImage;    
    public TMP_Text cardName;    
    public TMP_Text price;    
    public Button buyButton;
    public Button addButton;
    public Button reduceButton;

    private string cardNameS;

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
    public Image imagePlayerHead;

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
            var nowPlayer = CardShopManager.Instance.GetCurrentPlayer();
            if (!nowPlayer.isAI)
            {
                if (count == 1 || nowPlayer.gold < priceI * 2)
                {
                    if (CardShopManager.Instance.OnPlayerBuyCard(this, nowPlayer, cardId, isHeroCard, priceI, 1))
                        CardShopManager.Instance.AfterAct();
                }
                else
                {
                    if(!addButton.gameObject.activeSelf)
                    {
                        addButton.gameObject.SetActive(true);
                        reduceButton.gameObject.SetActive(true);
                    }
                    else
                    {
                        var nowCount = int.Parse(price.text) / priceI;
                        if (CardShopManager.Instance.OnPlayerBuyCard(this, nowPlayer, cardId, isHeroCard, priceI * nowCount, nowCount))
                            CardShopManager.Instance.AfterAct();
                    }
                }
            }
        });

        addButton.gameObject.SetActive(false);
        reduceButton.gameObject.SetActive(false);
        addButton.onClick.AddListener(() =>
        {
            var nowCount = int.Parse(price.text) / priceI;
            if(count > nowCount)
                price.text = (priceI * (nowCount + 1)).ToString();
        });
        reduceButton.onClick.AddListener(() =>
        {
            var nowCount = int.Parse(price.text) / priceI;
            if(nowCount > 1)
                price.text = (priceI * (nowCount - 1)).ToString();
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

    public void Init(int cid, bool isHero, int count1, int shopOpenIndex)
    {
        cardId = cid;
        isHeroCard = isHero;
        this.count = count1;

        if (isHero)
        {
            isHeroCardNode.SetActive(true);
            isItemCardNode.SetActive(false);

            var heroCfg = HeroConfig.GetConfig(cid);
            heroImage.sprite = Resources.Load<Sprite>("SkinsBig/" + heroCfg.Icon);
            cardNameS = heroCfg.Name;
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
            priceI = HeroSelectionTool.GetPrice(heroCfg);

            var player0 = GameManager.Instance.GetPlayer(0);
            if (player0.HasCard(cardId))
            {
                effectGreen.SetActive(true);
                effectYellow.SetActive(false);
            }
            else if (player0.HasFriend(cardId))
            {
                effectGreen.SetActive(false);
                effectYellow.SetActive(true);
            }
            else
            {
                effectGreen.SetActive(false);
                effectYellow.SetActive(false);
            }

            var player1 = GameManager.Instance.GetFirstNoAiPlayer();
            if (player1 != null && player1.HasCard(cardId))
            {
                imagePlayerHead.sprite = Resources.Load<Sprite>(player1.imgPath);
                imagePlayerHead.gameObject.SetActive(true);
            }
            else if (player1 != null && player1.HasFriend(cardId))
            {
                imagePlayerHead.sprite = Resources.Load<Sprite>(player1.imgPath);
                imagePlayerHead.color = Color.yellow;
                imagePlayerHead.gameObject.SetActive(true);
            }
            else
            {
                imagePlayerHead.gameObject.SetActive(false);
            }
        }
        else
        {
            isHeroCardNode.SetActive(false);
            isItemCardNode.SetActive(true);

            var itemCfg = ItemConfig.GetConfig(cid);
            cardNameS = itemCfg.Name;
            cardName.text = itemCfg.Name;
            if (count > 1)
                cardName.text += "x" + count;
            itemImage.sprite = Resources.Load<Sprite>("ItemPic/" + itemCfg.Icon);
            if (!string.IsNullOrEmpty(itemCfg.Attr1))
            {
                itemAttrImage1.sprite = Resources.Load<Sprite>("Textures/attr" + itemCfg.Attr1);
                itemAttrName1.text = itemCfg.Attr1Val.ToString();
            }
            else
            {
                itemAttrImage1.gameObject.SetActive(false);
                itemAttrName1.gameObject.SetActive(false);

            }

            if (!string.IsNullOrEmpty(itemCfg.Attr2))
            {
                itemAttrImage2.sprite = Resources.Load<Sprite>("Textures/attr" + itemCfg.Attr2);
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

            priceI = itemCfg.Price + (int)Math.Floor(itemCfg.PriceRound * shopOpenIndex);

            var player0 = GameManager.Instance.GetPlayer(0);
            if (player0.HasCard(cardId))
            {
                effectGreen.SetActive(true);
            }
            else
            {
                effectGreen.SetActive(false);
            }

            var player1 = GameManager.Instance.GetFirstNoAiPlayer();
            if (player1 != null && player1.HasCard(cardId))
            {
                imagePlayerHead.sprite = Resources.Load<Sprite>(player1.imgPath);
                imagePlayerHead.gameObject.SetActive(true);
            }
            else
            {
                imagePlayerHead.gameObject.SetActive(false);
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

    public void OnSold(PlayerInfo playerInfo, int sellCount)
    {
        if(sellCount > count || sellCount <= 0)
        {
            throw new ArgumentException("OnSold error, sellCount: " + sellCount + ", count: " + count);
        }

        count -= sellCount;
        if (count == 0)
        {
            isSold = true;
            buyButton.gameObject.SetActive(false);
            soldImage.gameObject.SetActive(true);

            if (effectGreen != null) //道具的情况
                effectGreen.SetActive(false);
            if (effectYellow != null) //道具的情况
                effectYellow.SetActive(false);
            imagePlayerHead.gameObject.SetActive(false);

            //把heroImage变灰色 - 改为将整个panel变成灰度图
            SetGrayscaleEffect();
            soldImage.color = playerInfo.lineColor;
        }
        else
        {
            cardName.text = cardNameS;
            if (count > 1)
                cardName.text += "x" + count;
        }
        addButton.gameObject.SetActive(false);
        reduceButton.gameObject.SetActive(false);

        //创建一个Image，启动携程 飞到 PlayerInfo的位置 
        StartCoroutine(MoveToPlayerInfoCount(playerInfo, sellCount));
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

    private System.Collections.IEnumerator MoveToPlayerInfoCount(PlayerInfo playerInfo, int count)
    {
        if(count == 1)
        {
            StartCoroutine(MoveToPlayerInfo(playerInfo));
        }
        else
        {
            for(int i = 0; i < count; i++)
            {
                StartCoroutine(MoveToPlayerInfo(playerInfo));
                yield return new WaitForSeconds(0.2f);
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
