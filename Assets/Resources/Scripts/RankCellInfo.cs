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
    public Image heroSkill;
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
        heroSkill.raycastTarget = false;
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
        bool isClickOnHeroSkill = RectTransformUtility.RectangleContainsScreenPoint(
            heroSkill.rectTransform, 
            eventData.position, 
            eventData.pressEventCamera);

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
            loveBtn.GetComponent<Image>().sprite = Resources.Load<Sprite>("love");
        else
            loveBtn.GetComponent<Image>().sprite = Resources.Load<Sprite>("loveoff");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
