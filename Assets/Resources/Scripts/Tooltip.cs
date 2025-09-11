using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CommonConfig;
using System.Collections.Generic;

public class Tooltip : MonoBehaviour
{
    public static Tooltip Instance;

    public TMP_Text tooltipText;
    public TMP_Text tooltipFriendText;
    public RectTransform rect;
    public Image tooltipIcon;
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
        tooltipText.gameObject.SetActive(hasSkill);
        tooltipIcon.gameObject.SetActive(hasSkill);
        tooltipFriendText.gameObject.SetActive(hasFriend);
        
        float currentY = 0f; // 起始Y位置
        float spacing = 10f;   // 控件间距
        
        if (hasSkill)
        {
            var skillConfig = SkillConfig.GetConfig(skillIds[0]);
            tooltipText.text = skillConfig.Name + "[<color=yellow>" + skillConfig.Price.ToString() + "元]</color>" + skillConfig.Descript; //富文本
            tooltipIcon.sprite = Resources.Load<Sprite>("SkillPic/" + skillConfig.Icon);
            
            tooltipText.rectTransform.sizeDelta = new Vector2(tooltipText.rectTransform.sizeDelta.x, tooltipText.preferredHeight);
            currentY +=Mathf.Max(tooltipText.preferredHeight, 65f) + spacing;
        }
        
        if (hasFriend)
        {
            tooltipFriendText.text = "相性:";
            foreach (var item in friendInfo)
            {
                var friendCfg = HeroFriendConfig.GetConfig(item);
                tooltipFriendText.text += "\n<color=green>" + friendCfg.Name + "</color>\n  ";
                foreach (var hid in friendCfg.Heros)
                {
                    var heroConfig = HeroConfig.GetConfig(hid);
                    if(hid == heroId)
                        tooltipFriendText.text += "<color=yellow>" + heroConfig.Name + "</color> ";
                    else if(!HeroSelectionTool.HasHeroInPool(hid))
                        tooltipFriendText.text += "<color=#808080>" + heroConfig.Name + "</color> ";
                    else
                        tooltipFriendText.text += heroConfig.Name + " ";
                }
            }
            
            // 设置好友加成位置
            tooltipFriendText.rectTransform.anchoredPosition = new Vector2(tooltipFriendText.rectTransform.anchoredPosition.x, -currentY);
            
            // 调整text组件高度以减少空白
            tooltipFriendText.rectTransform.sizeDelta = new Vector2(tooltipFriendText.rectTransform.sizeDelta.x, tooltipFriendText.preferredHeight);
            currentY += tooltipFriendText.preferredHeight + spacing;
        }
        
        // 调整背景大小
        float height = Mathf.Max(50f, currentY + 10f);
        rect.sizeDelta = new Vector2(450, height);
        
        // 调整位置
        transform.position = Input.mousePosition + new Vector3(0, rect.sizeDelta.y + 100, 0); // 稍微偏移鼠标位置     
        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, 0, Screen.height), transform.position.z); // 确保在屏幕内
        
        gameObject.SetActive(true);
    }

    public void HideTooltip()
    {
        gameObject.SetActive(false);
    }
}