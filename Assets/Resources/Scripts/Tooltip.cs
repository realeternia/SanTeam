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

    // 整体缩放：图标、字体、背景一起等比放大
    private const float UIScale = 1.3f;

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

        // 统一pivot/anchor为父物体中心，保证ShowTooltip中的局部坐标计算与实际渲染位置一致
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);

        // 整体放大（图标、字体、背景等比缩放；ShowTooltip中会按屏幕高度自适应覆盖）
        rect.localScale = Vector3.one * UIScale;

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

    public void ShowTooltip(List<SkillConfig> skillCfgs, HashSet<int> friendInfo, int heroId, PlayerInfo player = null)
    {
        bool hasSkill = skillCfgs != null && skillCfgs.Count > 0;
        bool hasFriend = friendInfo != null && friendInfo.Count > 0;

        // 属性取值与战斗统一：HeroConfig 数值经 PostModify 写回为 1星带品质面板（四主），星级成长走 GetCardAttr
        AttrInfo attr;
        if (player != null)
        {
            int exp = player.cards.TryGetValue(heroId, out int e) ? e : 1;
            int lv = HeroSelectionTool.GetCardLevel(exp, ConfigManager.IsHeroCard(heroId));
            attr = HeroSelectionTool.GetCardAttr(player, heroId, lv);
        }
        else
        {
            // 无玩家上下文（如排行榜）：直接显示写回的 1星带品质面板
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
            // 道具只显示配置的有效属性行（四主属性 + 护甲/魔抗/攻速/暴击/回蓝等扩展属性）
            var itemCfg = ItemConfig.GetConfig(heroId);
            var listKeys = new List<string>();
            var listVals = new List<string>();
            AddItemAttrRow(itemCfg.Attr1, itemCfg.Attr1Val, listKeys, listVals);
            AddItemAttrRow(itemCfg.Attr2, itemCfg.Attr2Val, listKeys, listVals);
            attrKeys = listKeys.ToArray();
            attrVals = listVals.ToArray();
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
                    // 属性图标统一读 HeroAttrConfig 表（name → Icon）
                    var attrCfg = HeroAttrConfig.GetConfigByname(attrKeys[i]);
                    string attrIcon = string.IsNullOrEmpty(attrCfg.Icon) ? "attrhp" : attrCfg.Icon;
                    imageAttrs[i].sprite = Resources.Load<Sprite>("Textures/Icons/" + attrIcon);
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
            textSkills[i].gameObject.SetActive(skillCfgs!=null && skillCfgs.Count > i);
            imageSkills[i].gameObject.SetActive(skillCfgs!=null && skillCfgs.Count > i);
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
            // 职业技能行：统计当前玩家上阵同职业英雄数，用于羁绊档位高亮
            int jobFieldCount = 0;
            string heroJob = null;
            if (isHero)
            {
                heroJob = HeroConfig.GetConfig(heroId).Job;
                if (player != null)
                {
                    foreach (var cardId in player.battleCards)
                    {
                        if (cardId > 0 && ConfigManager.IsHeroCard(cardId) && HeroConfig.GetConfig(cardId).Job == heroJob)
                            jobFieldCount++;
                    }
                }
            }

            for(int i = 0; i < skillCfgs.Count; i++)
            {
                var skillConfig = skillCfgs[i];
                var skillAttrStr = skillConfig.Attr == "might" ? "<color=red>[无双]</color>" : skillConfig.Attr == "atk" ? "<color=yellow>[攻]</color>" : skillConfig.Attr == "ap" ? "<color=blue>[法]</color>" : "";
                if (skillConfig.Type == "职业")
                {
                    // 职业技能（兵种连锁）：显示各档位两列数值并按上阵数高亮当前档位
                    textSkills[i].text = skillConfig.Name + JobLinkManager.GetJobLinkTipText(heroJob, jobFieldCount);
                }
                else
                {
                    textSkills[i].text = skillAttrStr + skillConfig.Name + skillConfig.Descript; //富文本
                }
                imageSkills[i].sprite = Resources.Load<Sprite>("SkillPic/" + skillConfig.Icon);
            }

            for (int i = 0; i < skillCfgs.Count; i++)
            {
                textSkills[i].rectTransform.anchoredPosition = new Vector2(textSkills[i].rectTransform.anchoredPosition.x, -currentY);
                imageSkills[i].rectTransform.anchoredPosition = new Vector2(imageSkills[i].rectTransform.anchoredPosition.x, -currentY - 27);
                textSkills[i].rectTransform.sizeDelta = new Vector2(textSkills[i].rectTransform.sizeDelta.x, textSkills[i].preferredHeight);
                currentY += Mathf.Max(textSkills[i].preferredHeight, 65f) + spacing;
            }
        }
        
        if (hasFriend)
        {
            textFriend.text = "";
            foreach (var item in friendInfo)
            {
                var friendCfg = HeroFriendConfig.GetConfig(item);
                textFriend.text += "<color=green>" + friendCfg.Name + "</color>\n  ";
                foreach (var hid in friendCfg.Heros)
                {
                    var heroConfig = HeroConfig.GetConfig(hid);
                    if (!HeroSelectionTool.HasHeroInPool(hid))
                        textFriend.text += "<color=#808080>" + heroConfig.Name + "</color> ";
                    else
                        textFriend.text += heroConfig.Name + " ";
                }
                textFriend.text += "\n";

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

        RectTransform canvasRect = transform.parent as RectTransform;
        if (canvasRect == null)
        {
            gameObject.SetActive(true);
            return;
        }

        // Canvas挂了CanvasScaler，屏幕像素与UI单位不一致；
        // 统一在canvas局部空间（与sizeDelta同单位）计算，边界用canvasRect.rect，避免像素/UI单位换算误差
        float baseWidth = rect.sizeDelta.x;
        float baseHeight = rect.sizeDelta.y;
        float viewWidth = canvasRect.rect.width;
        float viewHeight = canvasRect.rect.height;

        // 动态整体缩放：默认放大30%；若整体高度超过可视高度，则缩小到刚好撑满（留5%边距）
        float scale = UIScale;
        if (baseHeight * scale > viewHeight * 0.95f)
            scale = viewHeight * 0.95f / baseHeight;
        rect.localScale = Vector3.one * scale;

        // 缩放后的实际宽高
        float tooltipWidth = baseWidth * scale;
        float tooltipHeight = baseHeight * scale;

        // 鼠标位置转为canvas局部坐标
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect, Input.mousePosition, WorldManager.Instance.uiCamera, out localPoint);

        float halfW = viewWidth * 0.5f;
        float halfH = viewHeight * 0.5f;

        // 水平定位（触摸屏：tips不挡点击点）：默认从点击位置右边开始显示（左边贴点击点）
        const float gapX = 20f;
        float centerX = localPoint.x + gapX + tooltipWidth * 0.5f;
        // 右侧超出可视区：翻到点击位置左边显示（右边贴点击点）
        if (centerX + tooltipWidth * 0.5f > halfW - 10f)
            centerX = localPoint.x - gapX - tooltipWidth * 0.5f;
        // 左侧也超出（点击点太靠左）：夹在左边界内
        if (centerX - tooltipWidth * 0.5f < -halfW + 10f)
            centerX = -halfW + tooltipWidth * 0.5f + 10f;

        // 垂直定位：以点击点为中心，上下夹紧保证完整可见（底部留边距40）
        float centerY = localPoint.y;
        if (centerY - tooltipHeight * 0.5f < -halfH + 40f)
            centerY = -halfH + tooltipHeight * 0.5f + 40f;
        if (centerY + tooltipHeight * 0.5f > halfH - 10f)
            centerY = halfH - tooltipHeight * 0.5f - 10f;

        // pivot已统一为中心点，anchoredPosition即tooltip中心位置
        rect.anchoredPosition = new Vector2(centerX, centerY);

        gameObject.SetActive(true);
    }

    public void HideTooltip()
    {
        gameObject.SetActive(false);
    }

    // 道具属性行：键值按配置输出，比例属性（攻速/暴击）带 % 后缀，其余直接显示数值
    private void AddItemAttrRow(string key, int value, List<string> keys, List<string> vals)
    {
        if (string.IsNullOrEmpty(key) || value == 0)
            return;
        keys.Add(key);
        bool isPercent = key == "attackRate" || key == "critRate";
        vals.Add(isPercent ? value + "%" : value.ToString());
    }
}