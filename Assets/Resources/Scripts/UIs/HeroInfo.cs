using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HeroInfo : MonoBehaviour
{
    public TMP_Text heroName;
    public TMP_Text heroLevelTxt;
    public TMP_Text heroHpTxt;
    public Image heroImage;
    public Image healthImg;
    public Image errorImg;
    public Image classImg;

    public TMP_Text heroInteTxt;
    public TMP_Text heroStrTxt;

    // Start is called before the first frame update
    void Start()
    {
        errorImg.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetAttr(int ap, int atk)
    {
        SetText(heroInteTxt, ap);

        // 无双强度已并入攻击：原“无双/武力”数值列隐藏，不再单独展示
        if (heroStrTxt != null)
            heroStrTxt.gameObject.SetActive(false);

        // 确定英雄的最高属性（法术强度 / 攻击，攻击含无双强度）
        string highestAttr = "";
        var total = ap + atk;
        if (ap >= atk)
        {
            highestAttr = "attrinte";
        }
        else
        {
            highestAttr = "attrlead";
        }

        if (highestAttr != "")
        {
            // 根据最高属性加载对应图片
            classImg.sprite = Resources.Load<Sprite>("Textures/" + highestAttr);
            if (total >= 600)
            {
                classImg.color = SysColor.Tier.Purple;
            }
            else if (total >= 500)
            {
                classImg.color = SysColor.Tier.Magenta;
            }
            else if (total >= 420)
            {
                classImg.color = Color.red;
            }
            else if (total >= 350)
            {
                classImg.color = SysColor.Tier.Orange;
            }
            else if (total >= 290)
            {
                classImg.color = Color.yellow;
            }
            else if (total >= 250)
            {
                classImg.color = Color.green;
            }
        }
    }

    private void SetText(TMP_Text text, int val)
    {
        text.text = val.ToString();
        if (val >= 250)
        {
            text.color = SysColor.Tier.Purple;
        }
        else if (val >= 210)
        {
            text.color = SysColor.Tier.Magenta;
        }
        else if (val >= 170)
        {
            text.color = Color.red;
        }
        else if (val >= 140)
        {
            text.color = SysColor.Tier.Orange;
        }
        else if (val >= 110)
        {
            text.color = Color.yellow;
        }
        else if (val >= 95)
        {
            text.color = Color.green;
        }
    }


    public void SetHpRate(int hp, int maxHp)
    {
        var hpRate = (float)hp / maxHp;
        heroHpTxt.text = hp + " / " + maxHp;
        healthImg.rectTransform.sizeDelta = new Vector2((int)(hpRate * 210), healthImg.rectTransform.sizeDelta.y);
        if (hpRate <= 0)
        {
            errorImg.gameObject.SetActive(true);
            heroName.color = Color.gray;
            heroLevelTxt.color = Color.gray;
        }
        else if(hpRate <= 0.5)
            healthImg.color = Color.yellow;
        else
            healthImg.color = Color.green;

    }
}
