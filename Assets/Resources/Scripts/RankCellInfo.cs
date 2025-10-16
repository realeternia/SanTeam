using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using CommonConfig;


public class RankCellInfo : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public Image heroPic;
    public Image[] heroSkill;
    public TMP_Text heroName;
    public TMP_Text heroStr;
    public TMP_Text heroInte;
    public TMP_Text heroLeadShip;
    public TMP_Text heroHp;
    public TMP_Text heroPrice;
    public Button loveBtn;

    public int heroId;
    public int str;
    public int inte;
    public int leadShip;
    public int hp;
    public int price;

    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < heroSkill.Length; i++)
            heroSkill[i].raycastTarget = false;
        heroName.raycastTarget = false;
        heroStr.raycastTarget = false;
        heroInte.raycastTarget = false;
        heroLeadShip.raycastTarget = false;
        heroHp.raycastTarget = false;
        heroPrice.raycastTarget = false;

        loveBtn.onClick.AddListener(() =>
        {
            if (Profile.Instance.cardLoves.Contains(heroId))
                Profile.Instance.cardLoves.Remove(heroId);
            else if(Profile.Instance.cardLoves.Count < 5)
                Profile.Instance.cardLoves.Add(heroId);
            else
                return;

            Profile.Instance.SaveTextFile();
            UpdateLoveBtn();
        });
        UpdateLoveBtn();

        if(!HeroSelectionTool.HasHeroInPool(heroId))
        {
            heroName.color = Color.gray;
        }
    }

    public void Init(HeroConfig heroConfig)
    {
        // 设置英雄信息
        heroPic.sprite = Resources.Load<Sprite>("Skins/" + heroConfig.Icon);

        for (int i = 0; i < heroSkill.Length; i++)
        {
            if (i < heroConfig.Skills.Length)
            {
                var skillIcon = SkillConfig.GetConfig(heroConfig.Skills[i]).Icon;
                heroSkill[i].sprite = Resources.Load<Sprite>("SkillPic/" + skillIcon);
            }
            else
            {
                heroSkill[i].sprite = null;
                if(i > 0)
                    heroSkill[i].gameObject.SetActive(false);// 第一个永远显示
            }
        }

        heroName.text = heroConfig.Name;
        heroId = (int)heroConfig.Id;
        str = heroConfig.Str;
        inte = heroConfig.Inte;
        leadShip = heroConfig.LeadShip;
        hp = heroConfig.Hp;
        price = HeroSelectionTool.GetPrice(heroConfig);
        if (heroConfig.Job == "shuai")
            loveBtn.gameObject.SetActive(false);

        var bg = GetComponent<Image>();
        bg.color = HeroSelectionTool.GetSideColor(heroConfig.Side);

        heroStr.text = heroConfig.Str.ToString();
        if (heroConfig.Str >= 95)
            heroStr.text = "<color=red>" + heroConfig.Str.ToString() + "</color>";
        else if (heroConfig.Str >= 90)
            heroStr.text = "<color=yellow>" + heroConfig.Str.ToString() + "</color>";

        heroInte.text = heroConfig.Inte.ToString();
        if (heroConfig.Inte >= 95)
            heroInte.text = "<color=red>" + heroConfig.Inte.ToString() + "</color>";
        else if (heroConfig.Inte >= 90)
            heroInte.text = "<color=yellow>" + heroConfig.Inte.ToString() + "</color>";

        heroLeadShip.text = heroConfig.LeadShip.ToString();
        if (heroConfig.LeadShip >= 95)
            heroLeadShip.text = "<color=red>" + heroConfig.LeadShip.ToString() + "</color>";
        else if (heroConfig.LeadShip >= 90)
            heroLeadShip.text = "<color=yellow>" + heroConfig.LeadShip.ToString() + "</color>";


        heroPrice.text = price.ToString();
        if (price >= 22)
            heroPrice.text = "<color=red>" + price.ToString() + "</color>";
        else if (price >= 19)
            heroPrice.text = "<color=yellow>" + price.ToString() + "</color>";

        heroHp.text = heroConfig.Hp.ToString();
    }


    public void OnPointerUp(PointerEventData eventData)
    {
        if (Tooltip.Instance != null)
        {
            Tooltip.Instance.HideTooltip();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log($"UI 元素被按下，位置：{eventData.position}");

        // 判断点击是否在heroSkill区域内
        bool isClickOnHeroSkill = false;
        for(int i = 0; i < heroSkill.Length; i++)
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(
            heroSkill[i].rectTransform, 
            eventData.position, 
            eventData.pressEventCamera))
            {
                isClickOnHeroSkill = true;
                break;
            }
        }

        if (!isClickOnHeroSkill)
            return;

        var heroCfg = HeroConfig.GetConfig(heroId);
        var friendInfo = ConfigManager.GetHeroFriendInfo(heroId);
        if (heroCfg.Skills != null && heroCfg.Skills.Length > 0 || friendInfo != null)
        {
            Tooltip.Instance.ShowTooltip(heroCfg.Skills, friendInfo, heroId);
        }
    }

    private void UpdateLoveBtn()
    {
        if (Profile.Instance.cardLoves.Contains(heroId))
            loveBtn.GetComponent<Image>().sprite = Resources.Load<Sprite>("Textures/love");
        else
            loveBtn.GetComponent<Image>().sprite = Resources.Load<Sprite>("Textures/loveoff");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
