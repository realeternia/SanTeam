using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChessHUD : MonoBehaviour
{
    public Chess chessUnit;
    public Image healthImg;
    private int lastHp;

    void Start()
    {
        healthImg.gameObject.transform.parent.gameObject.SetActive(false);

        if (chessUnit != null)
        {
            UpdateHealthDisplay();
        }
    }

    void Update()
    {
        if (chessUnit == null)
        {
            Destroy(gameObject);
            return;
        }

        // 更新血条位置，使其跟随单位
        Vector3 worldPosition = new Vector3(chessUnit.transform.position.x + 5, chessUnit.transform.position.y + 3f, chessUnit.transform.position.z + 5);
        // 将屏幕坐标转换为UI相机的Canvas局部坐标
        RectTransform rectTransform = GetComponent<RectTransform>();
        RectTransform parentCanvas = rectTransform.parent as RectTransform;
        var screenPosition = WorldManager.Instance.TransformWorldToScreen(worldPosition, parentCanvas);

        rectTransform.anchoredPosition = screenPosition + new Vector2(chessUnit.isHero ? -70 : -55, 0);

        // 更新血条显示
        UpdateHealthDisplay();
    }

    public void UpdateHealthDisplay()
    {
        if (chessUnit == null)
        {
            Debug.LogError("ChessUnit is null in UpdateHealthDisplay");
            return;
        }

        //chessUnit.hp如果变化不大，就return，降低开销
        if (Mathf.Abs(chessUnit.hp - lastHp) < 0.1f)
            return;
        lastHp = chessUnit.hp;

        if(chessUnit.hp < chessUnit.maxHp)
        {
            healthImg.gameObject.transform.parent.gameObject.SetActive(true);
            if(chessUnit.hp < chessUnit.maxHp * 0.5)
                healthImg.color = Color.yellow;
            else
                healthImg.color = Color.green;
        }

        if (healthImg != null)
        {
            var wid = chessUnit.isHero ? 70f : 50f;
            healthImg.rectTransform.sizeDelta = new Vector2(chessUnit.hp * wid / chessUnit.maxHp, healthImg.rectTransform.sizeDelta.y);
        }
    }
}