using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using CommonConfig;


public class RankCellInfo : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, ILoopScrollItem
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
    }

    public void Init(HeroConfig heroConfig)
    {
        // 设置英雄信息
        heroPic.sprite = Resources.Load<Sprite>("Skins/" + heroConfig.Icon);

        var skillCfgs = ConfigManager.GetHeroSkillConfigs(heroConfig);
        for (int i = 0; i < heroSkill.Length; i++)
        {
            if (i < skillCfgs.Count)
            {
                heroSkill[i].gameObject.SetActive(true); // 复用池中单元格时恢复之前被隐藏的图标
                var skillIcon = skillCfgs[i].Icon;
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
        heroName.color = SysColor.GetQualityColor(heroConfig.Quality);
        heroId = (int)heroConfig.Id;
        // 排行展示统一口径：HeroConfig 主数值经 PostModify 写回为 1星带品质面板（无双强度已并入 Atk，Str 列不再有数据）
        str = 0;
        inte = heroConfig.Ap;
        leadShip = heroConfig.Atk;
        hp = heroConfig.Hp;
        price = HeroSelectionTool.GetPrice(heroConfig);
        if (heroConfig.Job == "shuai")
            loveBtn.gameObject.SetActive(false);
        else
            loveBtn.gameObject.SetActive(true); // 复用池中单元格时恢复被隐藏的按钮

        var bg = GetComponent<Image>();
        bg.color = SysColor.GetSideColor(heroConfig.Side);

        // 不在卡池中的英雄名字置灰（池中单元格每次绑定都要刷新）
        if (!HeroSelectionTool.HasHeroInPool(heroId))
            heroName.color = Color.gray;

        // 数值不再按大小做红/黄特化色，统一展示1星面板
        heroStr.text = str.ToString();
        // 无双强度已并入攻击(Atk)：Str 列无数据，隐藏数值占位（待 UI 在预制体中移除该列）
        if (heroStr != null)
            heroStr.gameObject.SetActive(false);
        heroInte.text = inte.ToString();
        heroLeadShip.text = leadShip.ToString();
        heroPrice.text = price.ToString();
        heroHp.text = hp.ToString();

        UpdateLoveBtn(); // 复用池中单元格时刷新收藏图标
    }

    public void BindData(object data)
    {
        if (data is HeroConfig heroConfig)
        {
            Init(heroConfig);
        }
        else
        {
            GameLog.Warn($"RankCellInfo.BindData: 数据类型不匹配 data={data?.GetType().Name ?? "null"}");
        }
    }

    public void OnReturnToPool()
    {
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
        GameLog.Debug($"UI 元素被按下，位置：{eventData.position}");

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
        var skillCfgs = ConfigManager.GetHeroSkillConfigs(heroCfg);
        if (skillCfgs.Count > 0 || friendInfo != null)
        {
            Tooltip.Instance.ShowTooltip(skillCfgs, friendInfo, heroId);
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
