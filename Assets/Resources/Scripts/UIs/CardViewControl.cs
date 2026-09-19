using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CommonConfig;
using DG.Tweening;
using UnityEngine.EventSystems;
using System;

public class CardViewControl : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public int cardId;
    public int count;
    public bool isSold = false;
    public int priceI; //单价
    public int roundLeft;
    public bool isHeroCard;
    public Image soldImage;    
    public TMP_Text cardName;    
    public TMP_Text price;    
    public TMP_Text roundLeftText;    
    public Button buyButton;
    public Button addButton;
    public Button reduceButton;

    private string cardNameS;

    public GameObject isHeroCardNode;
    public GameObject isItemCardNode;

    //英雄卡相关
    public Image heroImage;
    public Image[] heroJobImage;


    public Image itemImage;
    public GameObject effectGreen;
    public GameObject effectYellow;
    public GameObject effectGray;
    public GameObject effectLayer;

    // Start is called before the first frame update
    void Start()
    {
        cardName.raycastTarget = false;

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
            else
            {
                // 对手(AI)回合：牌面仍可点击查看，但无法购买
                SystemTip.Show("当前是对手回合，无法购买");
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
        PanelManager.Instance.GetTooltip<BaseTooltip>()?.HideTooltip();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        GameLog.Debug($"UI 元素被按下，位置：{eventData.position}");

        // 属性取值在 Tooltip 内部统一走 GetCardAttr（与战斗一致），这里只需传当前玩家
        var player = CardShopManager.Instance.GetCurrentPlayer();

        if (isHeroCard)
        {
            var heroCfg = HeroConfig.GetConfig(cardId);
            var friendInfo = ConfigManager.GetHeroFriendInfo(cardId);
            // 商店牌：职业技能默认显示1级
            PanelManager.Instance.GetTooltip<TooltipHero>()?.ShowTooltip(ConfigManager.GetHeroSkillConfigs(heroCfg), friendInfo, cardId, player, true);
        }
        else
        {
            PanelManager.Instance.GetTooltip<TooltipHero>()?.ShowTooltip(null, null, cardId, player, true);
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
            heroImage.sprite = Resources.Load<Sprite>("Textures/SkinsBig/" + heroCfg.Icon);
            cardNameS = heroCfg.Name;
            cardName.text = heroCfg.Name;
            cardName.color = SysColor.GetQualityColor(heroCfg.Quality);
            if (count > 1)
                cardName.text += "x" + count;

            var heroSkillCfgs = ConfigManager.GetHeroSkillConfigs(heroCfg);
            for (int i = 0; i < heroJobImage.Length; i++)
            {
                if (i < heroSkillCfgs.Count)
                {
                    heroJobImage[i].gameObject.SetActive(true);
                    heroJobImage[i].sprite = Resources.Load<Sprite>("Textures/SkillPic/" + heroSkillCfgs[i].Icon);
                }
                else
                {
                    heroJobImage[i].gameObject.SetActive(false);
                }

            }

            gameObject.GetComponent<Image>().color = SysColor.GetSideColor(heroCfg.Side);
            priceI = HeroSelectionTool.GetPrice(heroCfg);

            UpdateEffects();
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
            itemImage.sprite = Resources.Load<Sprite>("Textures/ItemPic/" + itemCfg.Icon);

            // 物品仅掉落获得，不参与商店购买，价格恒为0（物品卡不再出现于商店）
            priceI = 0;

            UpdateEffects();
        }

        price.text = priceI.ToString();

        roundLeft = 3;
        UpdateRoundLeft();
    }

    // 刷新剩余轮数显示
    public void UpdateRoundLeft()
    {
        if (roundLeftText != null)
            roundLeftText.text = roundLeft.ToString();
    }

    // 根据玩家0的卡牌/好友情况重算效果标记（卡片刷新后也会重新计算）
    private void UpdateEffects()
    {
        var player0 = GameManager.Instance.GetPlayer(0);

        if (isHeroCard)
        {
            var heroCfg = HeroConfig.GetConfig(cardId);
            if (player0.HasCard(cardId))
            {
                effectGreen.SetActive(true);
                effectYellow.SetActive(false);
                effectGray.SetActive(false);
            }
            else if (player0.HasFriend(cardId))
            {
                effectGreen.SetActive(false);
                effectYellow.SetActive(true);
                effectGray.SetActive(false);
            }
            else if (HasSameJob(player0, heroCfg.Job))
            {
                effectGreen.SetActive(false);
                effectYellow.SetActive(false);
                effectGray.SetActive(true);
            }
            else
            {
                effectGreen.SetActive(false);
                effectYellow.SetActive(false);
                effectGray.SetActive(false);
            }
        }
        else
        {
            if (player0.HasCard(cardId))
                effectGreen.SetActive(true);
            else
                effectGreen.SetActive(false);
        }
    }

    // 玩家已拥有（卡牌集合中）与指定职业相同的英雄
    private bool HasSameJob(PlayerInfo player0, string job)
    {
        foreach (var ownedId in player0.cards.Keys)
        {
            if (HeroConfig.HasConfig(ownedId) && HeroConfig.GetConfig(ownedId).Job == job)
                return true;
        }
        return false;
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
            if (effectGray != null) //道具的情况
                effectGray.SetActive(false);

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
        // 获取所有Image组件并应用灰度效果（特效节点除外，售出时隐藏、刷新后由Init重新显示）
        Image[] allImages = GetComponentsInChildren<Image>(true);
        
        foreach (Image img in allImages)
        {
            if (img == null || IsEffectNode(img))
                continue;
            // 设置灰度颜色
            img.color = new Color(SysColor.Card.SoldGray.r, SysColor.Card.SoldGray.g, SysColor.Card.SoldGray.b, img.color.a);
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

    // 特效节点（effectGreen/effectYellow/effectLayer）不做灰度处理
    private bool IsEffectNode(Image img)
    {
        var go = img.gameObject;
        return (effectGreen != null && go.transform.IsChildOf(effectGreen.transform))
            || (effectYellow != null && go.transform.IsChildOf(effectYellow.transform))
            || (effectLayer != null && go.transform.IsChildOf(effectLayer.transform));
    }

    // 恢复灰度前的颜色：直接代码重新赋值，不缓存
    public void RestoreColor()
    {
        var panel = gameObject.GetComponent<Image>();
        if (isHeroCard)
            panel.color = Color.white; // Init 会重新赋阵营色
        else
            panel.color = SysColor.Card.ItemPanel; // 道具卡面板默认色

        foreach (Image img in GetComponentsInChildren<Image>(true))
        {
            if (img == null || img == panel || IsEffectNode(img))
                continue;
            img.color = Color.white;
        }
        foreach (TMP_Text t in GetComponentsInChildren<TMP_Text>(true))
        {
            if (t != null)
                t.color = Color.white;
        }
    }

    // 卡位被刷新前，重置售出状态
    public void ResetSold()
    {
        isSold = false;
        RestoreColor();
        soldImage.gameObject.SetActive(false);
        buyButton.gameObject.SetActive(true);
        addButton.gameObject.SetActive(false);
        reduceButton.gameObject.SetActive(false);
    }

    private System.Collections.IEnumerator MoveToPlayerInfoCount(PlayerInfo playerInfo, int count)
    {
        if(count == 1)
        {
            MoveToPlayerInfo(playerInfo);
        }
        else
        {
            for(int i = 0; i < count; i++)
            {
                MoveToPlayerInfo(playerInfo);
                yield return new WaitForSeconds(0.2f);
            }
        }
    }

    // 买卡后卡牌飞向玩家头像：DoTween 加速曲线（起步慢、越飞越快），同时缩到 50%
    private void MoveToPlayerInfo(PlayerInfo playerInfo)
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

        // 移动动画
        const float duration = 0.7f; // 移动持续时间
        const float shrinkScale = 0.5f; // 最终缩小到50%

        RectTransform cardRect = movingCardImage.GetComponent<RectTransform>();
        cardRect.anchoredPosition = startLocalPos;

        cardRect.DOAnchorPos(targetLocalPos, duration).SetEase(Ease.InQuad);
        cardRect.DOSizeDelta(cardRect.sizeDelta * shrinkScale, duration).SetEase(Ease.InQuad)
            .OnComplete(() => Destroy(movingCardImage));
    }

    public void ShowEffectLayer(bool isShow)
    {
        effectLayer.SetActive(isShow);
    }

    // 同步特效层容器：绿/黄/灰（与背包已拥有卡重复/好友/同职业关联）任意一个点亮时显示容器
    public void UpdateEffectLayer()
    {
        if (effectLayer == null)
            return;
        bool show = (effectGreen != null && effectGreen.activeSelf)
            || (effectYellow != null && effectYellow.activeSelf)
            || (effectGray != null && effectGray.activeSelf);
        effectLayer.SetActive(show);
    }

}
