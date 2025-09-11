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
        rect.sizeDelta = new Vector2(400, height);
        
        // 调整位置 - 将屏幕坐标转换为Canvas局部坐标
        Vector2 mouseScreenPos = Input.mousePosition;
        
        // 获取Canvas的RectTransform
        RectTransform canvasRect = transform.parent as RectTransform;
        if (canvasRect != null)
        {
            // 将屏幕坐标转换为Canvas局部坐标
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect, 
                mouseScreenPos, 
                WorldManager.Instance.uiCamera, 
                out localPoint);
            
            // 计算tooltip位置（考虑偏移和tooltip大小）
            // 注意：Canvas局部坐标系原点在中心，Y轴向上为正
            // 但UI元素的Y轴通常向下为正，所以需要调整Y方向
            Vector2 tooltipPosition = localPoint + new Vector2(30, -height/2); // Y轴反向
            
          //  // 确保在Canvas边界内
          //  float canvasWidth = canvasRect.rect.width;
          //  float canvasHeight = canvasRect.rect.height;
            
            // 调整边界计算，考虑锚点在左上角的情况
         //   tooltipPosition.x = Mathf.Clamp(tooltipPosition.x, -canvasWidth/2 - rect.sizeDelta.x/2, canvasWidth/2 - rect.sizeDelta.x/2);
          //  tooltipPosition.y = Mathf.Clamp(tooltipPosition.y, -canvasHeight/2 - rect.sizeDelta.y/2, canvasHeight/2-rect.sizeDelta.y/2);
            
            rect.anchoredPosition = tooltipPosition;
        }
        
        gameObject.SetActive(true);
    }

    public void HideTooltip()
    {
        gameObject.SetActive(false);
    }
}