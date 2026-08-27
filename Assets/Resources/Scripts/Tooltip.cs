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

    // 卡片属性显示：图标 + 属性值，一行两个（最多4个属性 = 2行）
    private Image[] imageAttrs;
    private TMP_Text[] textAttrs;
    private const float AttrRowHeight = 40f;

    // 道具描述文本（动态创建，道具卡显示）
    private TMP_Text textDes;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        // else
        //     Destroy(gameObject);

        CreateAttrControls();

        gameObject.SetActive(false);
    }

    // 用已有的技能图标/文本作为模板，动态生成属性控件（2行×2列）
    private void CreateAttrControls()
    {
        if (imageSkills == null || imageSkills.Length == 0 || textSkills == null || textSkills.Length == 0)
            return;

        imageAttrs = new Image[9];
        textAttrs = new TMP_Text[9];
        for (int i = 0; i < 9; i++)
        {
            int row = i / 2;
            int col = i % 2;
            float baseX = 20f + col * 200f;
            float y = -20f - row * AttrRowHeight;

            var iconGo = Instantiate(imageSkills[0].gameObject, rect);
            iconGo.name = "AttrIcon" + i;
            var iconRt = iconGo.GetComponent<RectTransform>();
            iconRt.anchorMin = new Vector2(0, 1);
            iconRt.anchorMax = new Vector2(0, 1);
            iconRt.anchoredPosition = new Vector2(baseX +10, y);
            iconRt.sizeDelta = new Vector2(38, 38);
            imageAttrs[i] = iconGo.GetComponent<Image>();

            var valGo = Instantiate(textSkills[0].gameObject, rect);
            valGo.name = "AttrVal" + i;
            var valRt = valGo.GetComponent<RectTransform>();
            valRt.anchorMin = new Vector2(0, 1);
            valRt.anchorMax = new Vector2(0, 1);
            valRt.anchoredPosition = new Vector2(baseX + 36+70, y + 15);
            valRt.sizeDelta = new Vector2(120, 30);
            textAttrs[i] = valGo.GetComponent<TMP_Text>();
        }

        // 道具描述文本（属性区下方）
        var desGo = Instantiate(textSkills[0].gameObject, rect);
        desGo.name = "AttrDes";
        var desRt = desGo.GetComponent<RectTransform>();
        desRt.anchorMin = new Vector2(0, 1);
        desRt.anchorMax = new Vector2(0, 1);
        desRt.anchoredPosition = new Vector2(20, -20);
        desRt.sizeDelta = new Vector2(360, 30);
        textDes = desGo.GetComponent<TMP_Text>();
        textDes.gameObject.SetActive(false);
    }

    private void Update()
    {

    }

    public void ShowTooltip(int[] skillIds, HashSet<int> friendInfo, int heroId, PlayerInfo player = null)
    {
        bool hasSkill = skillIds != null && skillIds.Length > 0;
        bool hasFriend = friendInfo != null && friendInfo.Count > 0;

        // 属性取值与战斗统一走 GetCardAttr（JobConfig 基础值 + 英雄覆盖 + 等级成长），无需外部传入
        AttrInfo attr;
        if (player != null)
        {
            int exp = player.cards.TryGetValue(heroId, out int e) ? e : 1;
            int lv = HeroSelectionTool.GetCardLevel(exp, ConfigManager.IsHeroCard(heroId));
            attr = HeroSelectionTool.GetCardAttr(player, heroId, lv);
        }
        else
        {
            // 无玩家上下文（如排行榜）：显示英雄基础属性
            var heroCfg = HeroConfig.GetConfig(heroId);
            attr = new AttrInfo() { Atk = heroCfg.Atk, Ap = heroCfg.Ap, Might = heroCfg.Might, Hp = heroCfg.Hp };
        }

        // 属性列表：英雄显示全部9项（攻/法/武/命/攻速/护甲/魔抗/移速/射程），道具只显示有效属性
        bool isHero = ConfigManager.IsHeroCard(heroId);
        string[] attrKeys;
        string[] attrVals;
        if (isHero)
        {
            var heroCfg = HeroConfig.GetConfig(heroId);
            attrKeys = new string[] { "atk", "ap", "might", "hp", "atkspeed", "armor", "magicres", "movespeed", "range" };
            attrVals = new string[]
            {
                attr.Atk.ToString(), attr.Ap.ToString(), attr.Might.ToString(), attr.Hp.ToString(),
                heroCfg.AtkSpeed.ToString(),
                heroCfg.Armor.ToString(), heroCfg.MagicRes.ToString(),
                heroCfg.MoveSpeed.ToString(), heroCfg.Range.ToString()
            };
        }
        else
        {
            attrKeys = new string[] { "atk", "ap", "might", "hp" };
            attrVals = new string[] { attr.Atk.ToString(), attr.Ap.ToString(), attr.Might.ToString(), attr.Hp.ToString() };
        }

        int attrRows = 0;
        int shownAttr = 0;
        if (imageAttrs != null)
        {
            for (int i = 0; i < imageAttrs.Length; i++)
            {
                bool show = i < attrKeys.Length && attrVals[i] != "0";
                imageAttrs[i].gameObject.SetActive(show);
                textAttrs[i].gameObject.SetActive(show);
                if (show)
                {
                    imageAttrs[i].sprite = Resources.Load<Sprite>("Textures/" + HeroSelectionTool.GetAttrIcon(attrKeys[i]));
                    textAttrs[i].text = attrVals[i];
                    shownAttr++;
                }
            }
            attrRows = (shownAttr + 1) / 2;
        }

        // 道具卡：显示道具描述（属性区下方）
        bool isItem = !ConfigManager.IsHeroCard(heroId);
        string itemDes = null;
        if (isItem)
        {
            var itemCfg = ItemConfig.GetConfig(heroId);
            if (itemCfg != null)
                itemDes = itemCfg.Des;
        }
        bool hasDes = !string.IsNullOrEmpty(itemDes);

        // 没有任何可显示内容时（如无属性的道具），不弹空 Tip
        if (shownAttr == 0 && !hasSkill && !hasFriend && !hasDes)
        {
            HideTooltip();
            return;
        }

        // 重置所有控件位置
        for(int i = 0; i < textSkills.Length; i++)
        {
            textSkills[i].gameObject.SetActive(skillIds!=null && skillIds.Length > i);
            imageSkills[i].gameObject.SetActive(skillIds!=null && skillIds.Length > i);
        }
        textFriend.gameObject.SetActive(hasFriend);
        
        float currentY = 10f + attrRows * AttrRowHeight; // 起始Y位置（属性区下方）
        float spacing = 15f;   // 控件间距

        if (hasDes)
        {
            textDes.gameObject.SetActive(true);
            textDes.text = itemDes;
            textDes.rectTransform.anchoredPosition = new Vector2(20, -currentY);
            textDes.rectTransform.sizeDelta = new Vector2(360, textDes.preferredHeight);
            currentY += textDes.preferredHeight + spacing;
        }
        else if (textDes != null)
        {
            textDes.gameObject.SetActive(false);
        }
        
        if (hasSkill)
        {
            for(int i = 0; i < skillIds.Length; i++)
            {
                var skillConfig = SkillConfig.GetConfig(skillIds[i]);
                var skillAttrStr = skillConfig.Attr == "might" ? "<color=red>[无双]</color>" : skillConfig.Attr == "atk" ? "<color=yellow>[攻]</color>" : skillConfig.Attr == "ap" ? "<color=blue>[法]</color>" : "";
                textSkills[i].text = skillAttrStr + skillConfig.Name + skillConfig.Descript; //富文本
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
                    if (!HeroSelectionTool.HasHeroInPool(hid))
                        textFriend.text += "<color=#808080>" + heroConfig.Name + "</color> ";
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