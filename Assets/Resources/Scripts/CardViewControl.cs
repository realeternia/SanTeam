using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CommonConfig;

public class CardViewControl : MonoBehaviour
{
    public int cardId;
    public int count;
    public bool isSold = false;
    public int priceI;
    public bool isHeroCard;
    public Image soldImage;    
    public TMP_Text cardName;    
    public TMP_Text price;    
    public Button buyButton;    

    public GameObject isHeroCardNode;
    public GameObject isItemCardNode;

    //英雄卡相关
    public Image heroImage;
    public Image jobImage;
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

    // Start is called before the first frame update
    void Start()
    {
        buyButton.onClick.AddListener(() =>
        {
            CardShopManager.Instance.OnPlayerBuyCard(this, 0, cardId, isHeroCard, priceI, count);

        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Init(int cid, bool isHero, int count)
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

            if (heroCfg.Skills != null && heroCfg.Skills.Length > 0)
                jobImage.sprite = Resources.Load<Sprite>("SkillPic/" + SkillConfig.GetConfig(heroCfg.Skills[0]).Icon);
            else
                jobImage.gameObject.SetActive(false);
            lead.text = GetColoredText(heroCfg.LeadShip);
            inte.text = GetColoredText(heroCfg.Inte);
            str.text = GetColoredText(heroCfg.Str);
            hp.text = heroCfg.Hp.ToString();

            if (heroCfg.Side == 1)
                gameObject.GetComponent<Image>().color = new Color(40 / 255f, 70 / 255f, 0 / 255f, 255 / 255f);
            else if (heroCfg.Side == 2)
                gameObject.GetComponent<Image>().color = new Color(0 / 255f, 35 / 255f, 100 / 255f, 255 / 255f);
            else if (heroCfg.Side == 3)
                gameObject.GetComponent<Image>().color = new Color(100 / 255f, 0 / 255f, 0 / 255f, 255 / 255f);
            else
                gameObject.GetComponent<Image>().color = new Color(50 / 255f, 50 / 255f, 50 / 255f, 255 / 255f);
            priceI = HeroSelectionTool.GetPrice(heroCfg) * count;
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

            priceI = itemCfg.Price * count;
        }

        price.text = priceI.ToString();

    }

    string GetColoredText(int value)
    {
        if (value >= 95)
        {
            return $"<color=red>{value}</color>";
        }
        else if (value >= 90)
        {
            return $"<color=yellow>{value}</color>";
        }
        return value.ToString();
    }    

    public void OnSold(PlayerInfo playerInfo)
    {
        isSold = true;
        buyButton.gameObject.SetActive(false);
        soldImage.gameObject.SetActive(true);

        //创建一个Image，启动携程 飞到 PlayerInfo的位置 
        StartCoroutine(MoveToPlayerInfo(playerInfo));
    }

    private System.Collections.IEnumerator MoveToPlayerInfo(PlayerInfo playerInfo)
    {
        // 创建一个新的Image对象并缓存
        var movingCardImage = new GameObject("MovingCardImage");
        movingCardImage.tag = "MovingCard";
        Image img = movingCardImage.AddComponent<Image>();
        img.sprite = heroImage.sprite;
        img.rectTransform.sizeDelta = new Vector2(100, 140);
        // 设置初始位置为当前卡片的屏幕位置
        RectTransform cardRect = GetComponent<RectTransform>();
        Vector2 screenPos;

        Canvas canvas = FindObjectOfType<Canvas>();
        RectTransform canvasRect = canvas.transform as RectTransform;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect,
            transform.position, canvas.worldCamera, out screenPos);
        img.rectTransform.anchoredPosition = screenPos;
        movingCardImage.transform.SetParent(canvas.transform, false);

        // 移动动画
        float duration = 0.7f; // 移动持续时间
        float elapsedTime = 0;
        Vector3 startPos = movingCardImage.transform.position;
        // 计算目标PlayerInfo在Canvas中的局部位置
        Vector2 targetScreenPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform,
            playerInfo.transform.position + new Vector3(120, 0), canvas.worldCamera, out targetScreenPos);
        Vector3 endPos = canvas.transform.TransformPoint(targetScreenPos);

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            // 使用平滑插值
            movingCardImage.transform.position = Vector3.Lerp(startPos, endPos, t);
            //逐渐缩小，最终缩小到50%
            img.rectTransform.sizeDelta = new Vector2(100, 140) * (1f - 0.5f * t);
            yield return null;
        }

        // 到达目标后重置引用，由Hide方法统一销毁
        Destroy(movingCardImage);
        movingCardImage = null;
    }

}
