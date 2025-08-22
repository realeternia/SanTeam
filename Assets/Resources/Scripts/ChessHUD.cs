using System.Collections;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChessHUD : MonoBehaviour
{
    public Chess chessUnit;
    public Image healthImg;

    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;

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
        Vector3 worldPosition = new Vector3(chessUnit.transform.position.x, chessUnit.transform.position.y + 3f, chessUnit.transform.position.z);
        Vector3 screenPosition = mainCamera.WorldToScreenPoint(worldPosition);
        transform.position = screenPosition + new Vector3(chessUnit.isHero ? -50 : -35, chessUnit.isHero ? 25 : 15, 0);

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