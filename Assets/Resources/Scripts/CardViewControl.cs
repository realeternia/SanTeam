using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardViewControl : MonoBehaviour
{
    public int cardId;
    public bool isSold = false;
    public int priceI;

    public Image cardImage;
    public Image soldImage;
    public TMP_Text cardName;
    public TMP_Text price;
    public TMP_Text lead;
    public TMP_Text inte;
    public TMP_Text str;
    public TMP_Text hp;
    public Button buyButton;

    // Start is called before the first frame update
    void Start()
    {
        buyButton.onClick.AddListener(() =>
        {
            CardShopManager.Instance.OnPlayerBuyCard(this, 0, cardId, priceI);
        });
    }

    // Update is called once per frame
    void Update()
    {
        
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
        // 创建一个新的Image对象
        GameObject imgObj = new GameObject("MovingCardImage");
        Image img = imgObj.AddComponent<Image>();
        img.sprite = cardImage.sprite;
        img.rectTransform.sizeDelta = new Vector2(100, 140);
        // 设置初始位置为当前卡片的屏幕位置
        RectTransform cardRect = GetComponent<RectTransform>();
        Vector2 screenPos;

        Canvas canvas = FindObjectOfType<Canvas>();        
        RectTransform canvasRect = canvas.transform as RectTransform;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect,
            transform.position, canvas.worldCamera, out screenPos);
        img.rectTransform.anchoredPosition = screenPos;
        imgObj.transform.SetParent(canvas.transform, false);

        // 移动动画
        float duration = 0.7f; // 移动持续时间
        float elapsedTime = 0;
        Vector3 startPos = imgObj.transform.position;
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
            imgObj.transform.position = Vector3.Lerp(startPos, endPos, t);
            //逐渐缩小，最终缩小到50%
            img.rectTransform.sizeDelta = new Vector2(100, 140) * (1f - 0.5f * t);
            yield return null;
        }

        // 到达目标后销毁Image
        Destroy(imgObj);
    }

}
