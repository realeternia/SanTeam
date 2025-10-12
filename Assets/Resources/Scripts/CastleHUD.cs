using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using CommonConfig;

public class CastleHUD : MonoBehaviour
{
    public TMP_Text castleName;
    public TMP_Text textAtk;
    public TMP_Text textHp;
    public Image healthImg;
    public Image foodImg;
    private int soldierLevel;
    private PlayerInfo owner;
    private int lastFood;
    private bool isFlashing = false;

    private int baseAtk;
    private int baseHp;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateFoodDisplay();
    }

    public void Init(PlayerInfo p, GameObject castleSpawn)
    {
        owner = p;
        castleName.text = p.playerNameText.text;

        var soldierCfg = SoldierConfig.GetConfig(500001);
        baseAtk = soldierCfg.Atk + p.sodatk + p.GetItemPAttr("satk");
        baseHp = soldierCfg.Hp + p.sodhp + p.GetItemPAttr("shp");
        textAtk.text = baseAtk.ToString();
        textHp.text = baseHp.ToString();

        // 更新血条位置，使其跟随单位
        UpdatePosition(castleSpawn);
    }

    private void UpdatePosition(GameObject castleSpawn)
    {
        Vector3 worldPosition = new Vector3(castleSpawn.transform.position.x + 5, castleSpawn.transform.position.y + 3f, castleSpawn.transform.position.z + 5);
        RectTransform rectTransform = GetComponent<RectTransform>();
        RectTransform parentCanvas = rectTransform.parent as RectTransform;
        var screenPosition = WorldManager.Instance.TransformWorldToScreen(worldPosition, parentCanvas);
        rectTransform.anchoredPosition = screenPosition + new Vector2(-75, 0);
    }

    public void AddSoldierLevel(int level, int atkAdd, int hpAdd)
    {
        soldierLevel += level;
        var soldierCfg = SoldierConfig.GetConfig(500001);
        textAtk.text = (baseAtk + atkAdd).ToString();
        textHp.text = (baseHp + hpAdd).ToString();
        textAtk.color = Color.green;
        textHp.color = Color.green;
    }

    public void UpdateFoodDisplay()
    {
        if (owner == null)
        {
            Debug.LogError("Castle is null in UpdateHealthDisplay");
            return;
        }

        //owner.food如果变化不大，就return，降低开销
        if (Mathf.Abs(owner.food - lastFood) < 0.1f)
            return;
        lastFood = owner.food;

        if (owner.food < owner.maxFood * 0.3)
            healthImg.color = Color.red;

        if (healthImg != null)
        {
            healthImg.rectTransform.sizeDelta = new Vector2(owner.food * 90f / owner.maxFood, healthImg.rectTransform.sizeDelta.y);
        }

        // 当食物为0时，启动闪烁协程
        if (owner.food == 0 && !isFlashing)
        {
            StartCoroutine(FlashFoodImage());
        }
        // 当食物不为0且正在闪烁时，停止闪烁
        else if (owner.food > 0 && isFlashing)
        {
            StopCoroutine(FlashFoodImage());
            isFlashing = false;
            if (foodImg != null)
            {
                foodImg.color = Color.white;
            }
        }
    }

    /// <summary>
    /// 闪烁食物图像，从白色到红色
    /// </summary>
    private IEnumerator FlashFoodImage()
    {
        isFlashing = true;
        float flashSpeed = 0.5f; // 闪烁速度，单位：秒

        while (owner != null && owner.food == 0 && foodImg != null)
        {
            // 从白色渐变到红色
            float t = 0;
            while (t < 1)
            {
                t += Time.deltaTime / flashSpeed;
                foodImg.color = Color.Lerp(Color.white, Color.red, t);
                yield return null;
            }

            // 从红色渐变到白色
            t = 0;
            while (t < 1)
            {
                t += Time.deltaTime / flashSpeed;
                foodImg.color = Color.Lerp(Color.red, Color.white, t);
                yield return null;
            }

            yield return null;
        }

        isFlashing = false;
        if (foodImg != null)
        {
            foodImg.color = Color.white;
        }
    }
}
