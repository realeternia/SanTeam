using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChessHUD : MonoBehaviour
{
    public Chess chessUnit;
    public Image healthImg;
    public Image manaImg;
    private int lastHp;
    private float manaTimer; // 法力条刷新计时，满1s更新一次（与技能mp充能/回复节奏一致）

    void Start()
    {
        healthImg.gameObject.transform.parent.gameObject.SetActive(false);

        if (chessUnit != null)
        {
            UpdateHealthDisplay();
            UpdateManaDisplay();
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

        // 法力条每秒更新（与技能mp充能/回复节奏一致）
        manaTimer += Time.deltaTime;
        if (manaTimer >= 1f)
        {
            manaTimer -= 1f;
            UpdateManaDisplay();
        }
    }

    public void UpdateHealthDisplay()
    {
        if (chessUnit == null)
        {
            GameLog.Error("ChessUnit is null in UpdateHealthDisplay");
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

    /// <summary>
    /// 更新法力条显示：manaImg非空且存在消耗MP的技能(MpCost>0)时，显示第一个此类技能的充能进度 Skill.mp/MpCost；无此类技能则隐藏
    /// </summary>
    private void UpdateManaDisplay()
    {
        if (manaImg == null || chessUnit == null)
            return;

        Skill mpSkill = null;
        foreach (var skill in chessUnit.skills)
        {
            if (skill.skillCfg.MpCost > 0)
            {
                mpSkill = skill;
                break;
            }
        }

        if (mpSkill == null)
        {
            if (manaImg.gameObject.activeSelf)
                manaImg.gameObject.SetActive(false);
            return;
        }

        if (!manaImg.gameObject.activeSelf)
            manaImg.gameObject.SetActive(true);
        var wid = chessUnit.isHero ? 70f : 50f;
        manaImg.rectTransform.sizeDelta = new Vector2(mpSkill.mp * wid / mpSkill.skillCfg.MpCost, manaImg.rectTransform.sizeDelta.y);
    }
}