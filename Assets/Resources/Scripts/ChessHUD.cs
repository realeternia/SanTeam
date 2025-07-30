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
        if (mainCamera == null)
        {
            Debug.LogError("Main camera not found in ChessHUD");
        }

        if (chessUnit != null)
        {
            UpdateHealthDisplay();
        }
        else
        {
            Debug.LogError("ChessUnit is null in ChessHUD");
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
        transform.position = screenPosition + new Vector3(0, 10, 0);

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

        if (healthImg != null)
        {
            healthImg.rectTransform.sizeDelta = new Vector2(chessUnit.hp * 70f / chessUnit.maxHp, healthImg.rectTransform.sizeDelta.y);
         //   Debug.Log($"Health updated: {chessUnit.hp}/{chessUnit.maxHp}");
        }
        else
        {
            Debug.LogError("HealthSlider is null in ChessHUD");
        }
    }
}