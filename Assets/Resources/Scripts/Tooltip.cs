using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CommonConfig;
using System.Collections.Generic;

public class Tooltip : MonoBehaviour
{
    public static Tooltip Instance;

    public TMP_Text[] textSkills;
    public TMP_Text textFriend;
    public RectTransform rect;
    public Image[] imageSkills;
    public int maxWidth = 300;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        // else
        //     Destroy(gameObject);

        gameObject.SetActive(false);
    }

    private void Update()
    {

    }

    public void ShowTooltip(int[] skillIds, HashSet<int> friendInfo, int heroId)
    {
        bool hasSkill = skillIds != null && skillIds.Length > 0;
        bool hasFriend = friendInfo != null && friendInfo.Count > 0;
        
        // 重置所有控件位置
        for(int i = 0; i < textSkills.Length; i++)
        {
            textSkills[i].gameObject.SetActive(skillIds!=null && skillIds.Length > i);
            imageSkills[i].gameObject.SetActive(skillIds!=null && skillIds.Length > i);
        }
        textFriend.gameObject.SetActive(hasFriend);
        
        float currentY = 10f; // 起始Y位置
        float spacing = 15f;   // 控件间距
        
        if (hasSkill)
        {
            for(int i = 0; i < skillIds.Length; i++)
            {
                var skillConfig = SkillConfig.GetConfig(skillIds[i]);
                var jobAttrStr = "";
                if(i == 0)
                {
                    var jobCfg = ConfigManager.GetJobConfig(HeroConfig.GetConfig(heroId).Job);
                    if(!string.IsNullOrEmpty(jobCfg.OvercomeStrong) || !string.IsNullOrEmpty(jobCfg.OvercomeWeak))
                        jobAttrStr = "克制:<color=red>" + jobCfg.OvercomeStrong + "</color><color=yellow>" + jobCfg.OvercomeWeak + "</color>\n";
                }
                var skillAttrStr = skillConfig.Attr == "str" ? "<color=red>[武]</color>" : skillConfig.Attr == "leadShip" ? "<color=yellow>[统]</color>" : skillConfig.Attr == "inte" ? "<color=blue>[智]</color>" : "";
                textSkills[i].text = jobAttrStr + skillAttrStr + skillConfig.Name + "[<color=yellow>" + skillConfig.Price.ToString() + "元</color>]" + skillConfig.Descript; //富文本
                imageSkills[i].sprite = Resources.Load<Sprite>("SkillPic/" + skillConfig.Icon);
            }

            for (int i = 0; i < skillIds.Length; i++)
            {
                textSkills[i].rectTransform.anchoredPosition = new Vector2(textSkills[i].rectTransform.anchoredPosition.x, -currentY);
                imageSkills[i].rectTransform.anchoredPosition = new Vector2(imageSkills[i].rectTransform.anchoredPosition.x, -currentY - 27);
                textSkills[i].rectTransform.sizeDelta = new Vector2(textSkills[i].rectTransform.sizeDelta.x, textSkills[i].preferredHeight);
                currentY += Mathf.Max(textSkills[i].preferredHeight, 65f) + spacing;
            }
        }
        
        if (hasFriend)
        {
            textFriend.text = "相性:";
            foreach (var item in friendInfo)
            {
                var friendCfg = HeroFriendConfig.GetConfig(item);
                textFriend.text += "\n<color=green>" + friendCfg.Name + "</color>\n  ";
                foreach (var hid in friendCfg.Heros)
                {
                    var heroConfig = HeroConfig.GetConfig(hid);
                    var friendAttr = HeroSelectionTool.GetSupportAttr(heroId, hid, 1);
                    if(friendAttr == null)
                        textFriend.text += heroConfig.Name + " ";
                    else if(!HeroSelectionTool.HasHeroInPool(hid))
                        textFriend.text += "<color=#808080>" + heroConfig.Name + "</color> ";                    
                    else if(friendAttr.Total <= 10)
                        textFriend.text += "<color=blue>" + heroConfig.Name + "</color> ";
                    else if(friendAttr.Total <= 15)
                        textFriend.text += "<color=green>" + heroConfig.Name + "</color> ";
                    else if(friendAttr.Total <= 20)
                        textFriend.text += "<color=yellow>" + heroConfig.Name + "</color> ";
                    else if(friendAttr.Total <= 25)
                        textFriend.text += "<color=#d96d00>" + heroConfig.Name + "</color> ";
                    else if(friendAttr.Total < 30)
                        textFriend.text += "<color=red>" + heroConfig.Name + "</color> ";
                    else if(friendAttr.Total == 30)
                        textFriend.text += "<color=#d900d9>" + heroConfig.Name + "</color> ";                        
                    else
                        textFriend.text += heroConfig.Name + " ";
                }
            }
            
            // 设置好友加成位置
            textFriend.rectTransform.anchoredPosition = new Vector2(textFriend.rectTransform.anchoredPosition.x, -currentY);
            
            // 调整text组件高度以减少空白
            textFriend.rectTransform.sizeDelta = new Vector2(textFriend.rectTransform.sizeDelta.x, textFriend.preferredHeight);
            currentY += textFriend.preferredHeight + spacing;
        }
        
        // 调整背景大小
        float height = Mathf.Max(50f, currentY + 10f);
        rect.sizeDelta = new Vector2(400, height);
        
        // 调整位置 - 直接在屏幕坐标系下进行边界检测
        Vector2 mouseScreenPos = Input.mousePosition;
        
        // 获取屏幕尺寸
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;
        
        // 计算tooltip的宽高
        float tooltipWidth = rect.sizeDelta.x;
        float tooltipHeight = rect.sizeDelta.y;
        
        // 计算tooltip位置（鼠标右侧偏移30像素）
        
        
        // 边界判定：确保tooltip完全在屏幕内
        // X轴边界（左右边界）
        UnityEngine.Debug.Log("mouseScreenPos.x: " + mouseScreenPos.x + " w=" + tooltipWidth + " l=" + screenWidth);
        if (mouseScreenPos.x + tooltipWidth > screenWidth -tooltipWidth/2)
        {
            // 如果超出右边界，显示在鼠标左侧
            mouseScreenPos.x = screenWidth - tooltipWidth-tooltipWidth/2;
        }
        if (mouseScreenPos.x < 0)
        {
            // 如果超出左边界，紧贴左边缘
            mouseScreenPos.x = 10;
        }
        
        // Y轴边界（上下边界）
        if (mouseScreenPos.y < 0)
        {
            // 如果超出下边界，显示在鼠标上方
            mouseScreenPos.y = mouseScreenPos.y + 20;
        }
        if (mouseScreenPos.y + tooltipHeight > screenHeight)
        {
            // 如果超出上边界，紧贴顶部
            mouseScreenPos.y = screenHeight - tooltipHeight - 10;
        }
        Vector2 tooltipScreenPos = mouseScreenPos + new Vector2(30, -tooltipHeight/2);
        
        // 将屏幕坐标转换为Canvas局部坐标
        RectTransform canvasRect = transform.parent as RectTransform;
        if (canvasRect != null)
        {
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect, 
                tooltipScreenPos, 
                WorldManager.Instance.uiCamera, 
                out localPoint);
            
            rect.anchoredPosition = localPoint;
        }
        
        gameObject.SetActive(true);
    }

    public void HideTooltip()
    {
        gameObject.SetActive(false);
    }
}