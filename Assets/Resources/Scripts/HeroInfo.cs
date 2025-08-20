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
    public TMP_Text heroLeadTxt;

    // Start is called before the first frame update
    void Start()
    {
        errorImg.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetAttr(int inte, int str, int leadShip)
    {
        SetText(heroInteTxt, inte);
        SetText(heroStrTxt, str);
        SetText(heroLeadTxt, leadShip);

        // 确定英雄的最高属性
        string highestAttr = "";
        var highestAttrValue = 0;

        var total = inte + leadShip + str;
        if (inte >= leadShip && inte >= str)
        {
            highestAttr = "attrinte";
            highestAttrValue = inte;
        }
        else if (leadShip >= inte && leadShip >= str)
        {
            highestAttr = "attrlead";
            highestAttrValue = leadShip;

        }
        else if (str >= inte && str >= leadShip)
        {
            highestAttr = "attrstr";
            highestAttrValue = str;

        }
        else if (total >= 235)
        {
            highestAttr = "attrshield";
        }

        if (highestAttr != "")
        {
            // 根据最高属性加载对应图片
            classImg.sprite = Resources.Load<Sprite>(highestAttr);
            if (total >= 250)
            {
                classImg.color = Color.green;
            }
            else if (total >= 280)
            {
                classImg.color = Color.yellow;
            }
            else if (total >= 320)
            {
                classImg.color = Color.red;
            }
            else if (total >= 360)
            {
                classImg.color = new Color(0.8f, 0, 1);
            }
        }
    }

    private void SetText(TMP_Text text, int val)
    {
        text.text = val.ToString();
        if (val >= 95)
        {
            text.color = Color.green;
        }
        else if (val >= 110)
        {
            text.color = Color.yellow;
        }
        else if (val >= 135)
        {
            text.color = Color.red;
        }
        else if (val >= 160)
        {
            text.color = new Color(0.8f, 0, 1);
        }

    }


    public void SetHpRate(int hp, int maxHp)
    {
        var hpRate = (float)hp / maxHp;
        heroHpTxt.text = hp + " / " + maxHp;
        healthImg.rectTransform.sizeDelta = new Vector2((int)(hpRate * 230), healthImg.rectTransform.sizeDelta.y);
        if(hpRate <=0 )
        {
            errorImg.gameObject.SetActive(true);
            heroName.color = Color.gray;
            heroLevelTxt.color = Color.gray;
        }
    }
}
