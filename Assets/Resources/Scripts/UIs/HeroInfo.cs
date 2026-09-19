using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CommonConfig;

public class HeroInfo : MonoBehaviour
{
    public TMP_Text heroName;
    public TMP_Text heroLevelTxt;
    public TMP_Text heroHpTxt;
    public Image heroImage;
    public Image healthImg;
    public Image errorImg;
    public Image classImg;

    public TMP_Text heroApTxt;
    public TMP_Text heroAtkTxt;

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
        SetText(heroApTxt, ap);
        SetText(heroAtkTxt, atk);
    }

    // 职业图标（职业技能图标，约定与布阵格 jobIcon 一致：Textures/SkillPic/ + 职业技能 Icon）
    public void SetJobIcon(HeroConfig heroCfg)
    {
        if (classImg == null)
            return;
        var skillCfgs = ConfigManager.GetHeroSkillConfigs(heroCfg);
        classImg.sprite = skillCfgs.Count > 0
            ? Resources.Load<Sprite>("Textures/SkillPic/" + skillCfgs[0].Icon)
            : null;
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
