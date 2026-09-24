using UnityEngine;
using TMPro;
using CommonConfig;
using System.Collections.Generic;

// 物品 Tooltip：继承通用 Tooltip 基类，结构 = 道具名(第一行) + 属性区(第二部分，复用英雄属性格) + 描述(第三部分，纯文字)
public class TooltipItem : BaseTooltip
{
    private const float NameRowHeight = 40f;   // 道具名行高
    private const float AttrRowHeight = 40f;   // 属性行高（一行两个）

    private static GameObject attrPrefab;                                           // 属性格预制体缓存（复用英雄属性格 ToolTipHeroAttr）
    private readonly List<TooltipHeroAttr> attrCells = new List<TooltipHeroAttr>(); // 属性格（按需生成）

    private TMP_Text textName; // 道具名（第一行，按品质上色）
    private TMP_Text textDes;  // 描述（第三部分，纯文字，自动换行）

    protected override void Awake()
    {
        base.Awake();
        CreateControls();
    }

    // 复用英雄属性格预制体创建控件：道具名(顶部) + 属性格(2行×2列，最多10格) + 描述(底部纯文字)
    private void CreateControls()
    {
        if (attrPrefab == null)
            attrPrefab = Resources.Load<GameObject>("Prefabs/ToolTipHeroAttr");
        if (attrPrefab == null)
        {
            GameLog.Error("TooltipItem 属性格预制体加载失败: Prefabs/ToolTipHeroAttr");
            return;
        }

        // 属性格（第二部分）：锚定 tooltip 左上角，自上而下 2 列排布
        for (int i = 0; i < 10; i++)
        {
            var go = Instantiate(attrPrefab, rect);
            go.name = "AttrCell" + i;
            var cell = go.GetComponent<TooltipHeroAttr>();
            if (cell == null)
            {
                GameLog.Error("TooltipItem 属性格预制体缺少 TooltipHeroAttr 组件");
                Destroy(go);
                continue;
            }
            int row = i / 2;
            int col = i % 2;
            float baseX = 20f + col * 200f;
            // 属性区顶部让出道具名行（名字行底 -50 起再留 15 间距，首行中心 -65）
            float y = -20f - NameRowHeight - 15f - row * AttrRowHeight;
            var rt = (RectTransform)cell.transform;
            rt.anchorMin = new Vector2(0, 1);
            rt.anchorMax = new Vector2(0, 1);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = new Vector2(baseX + 20, y);
            attrCells.Add(cell);
        }

        // 道具名（第一行）：复用属性格预制体的文字控件
        var nameGo = Instantiate(attrPrefab, rect);
        var nameCell = nameGo.GetComponent<TooltipHeroAttr>();
        if (nameCell == null || nameCell.text == null)
        {
            GameLog.Error("TooltipItem 属性格预制体缺少 TooltipHeroAttr 组件或文字控件");
            Destroy(nameGo);
            return;
        }
        textName = nameCell.text;
        textName.rectTransform.SetParent(rect, false);
        var nameRt = textName.rectTransform;
        nameRt.anchorMin = new Vector2(0, 1);
        nameRt.anchorMax = new Vector2(0, 1);
        nameRt.pivot = new Vector2(0, 1);
        nameRt.anchoredPosition = new Vector2(15, -10);
        nameRt.sizeDelta = new Vector2(360, NameRowHeight);
        textName.gameObject.SetActive(false);
        Destroy(nameGo);

        // 描述（第三部分，纯文字）：复用属性格预制体的文字控件
        var desGo = Instantiate(attrPrefab, rect);
        var desCell = desGo.GetComponent<TooltipHeroAttr>();
        if (desCell == null || desCell.text == null)
        {
            GameLog.Error("TooltipItem 属性格预制体缺少 TooltipHeroAttr 组件或文字控件");
            Destroy(desGo);
            return;
        }
        textDes = desCell.text;
        textDes.rectTransform.SetParent(rect, false);
        var desRt = textDes.rectTransform;
        desRt.anchorMin = new Vector2(0, 1);
        desRt.anchorMax = new Vector2(0, 1);
        desRt.pivot = new Vector2(0, 1);
        desRt.anchoredPosition = new Vector2(20, 0);
        desRt.sizeDelta = new Vector2(460, 30);
        textDes.gameObject.SetActive(false);
        Destroy(desGo);
    }

    public void ShowTooltip(int itemId)
    {
        var itemCfg = ItemConfig.GetConfig(itemId);
        if (itemCfg == null)
        {
            HideTooltip();
            return;
        }

        // 第一行：道具名（按品质上色）
        if (textName != null)
        {
            string nameHex = ColorUtility.ToHtmlStringRGB(SysColor.GetQualityColor(itemCfg.Quality));
            textName.text = "<color=#" + nameHex + ">" + itemCfg.Name + "</color>";
            textName.gameObject.SetActive(true);
        }

        // 第二部分：属性（只显示配置的有效属性行，图标统一读 HeroAttrConfig 表）
        var itemKeys = new List<string>();
        var itemVals = new List<string>();
        foreach (var bonus in JobLinkManager.ParseBonuses(itemCfg.Attrs))
            AddItemAttrRow(bonus.Attr, bonus.Value, itemKeys, itemVals);

        int shownAttr = 0;
        int lastShownRow = -1;
        for (int i = 0; i < attrCells.Count; i++)
        {
            bool show = i < itemKeys.Count;
            attrCells[i].gameObject.SetActive(show);
            if (show)
            {
                var attrCfg = HeroAttrConfig.GetConfigByname(itemKeys[i]);
                string attrIcon = string.IsNullOrEmpty(attrCfg.Icon) ? "attrhp" : attrCfg.Icon;
                attrCells[i].SetAttr(attrIcon, itemVals[i]);
                shownAttr++;
                lastShownRow = i / 2;
            }
        }
        // 按最后一个显示格的所在行计算高度（中间可能跳过无效项，不能按显示个数算）
        int attrRows = lastShownRow + 1;

        // 第三部分：描述（属性区下方，纯文字，高度自适应首选高度）
        bool hasDes = !string.IsNullOrEmpty(itemCfg.Des);
        float currentY = 10f + NameRowHeight + 15f + attrRows * AttrRowHeight;
        const float spacing = 5f;
        if (hasDes)
        {
            textDes.gameObject.SetActive(true);
            textDes.text = itemCfg.Des;
            textDes.rectTransform.anchoredPosition = new Vector2(20, -currentY);
            textDes.rectTransform.sizeDelta = new Vector2(460, textDes.preferredHeight);
            currentY += textDes.preferredHeight + spacing;
        }
        else if (textDes != null)
        {
            textDes.gameObject.SetActive(false);
        }

        // 没有任何可显示内容时不弹空 Tip
        if (shownAttr == 0 && !hasDes)
        {
            HideTooltip();
            return;
        }

        // 调整背景大小，走基类统一显示/定位逻辑（同一时刻只显示一个、贴边不出屏）
        rect.sizeDelta = new Vector2(500, Mathf.Max(50f, currentY + 10f));
        Show();
    }

    // 道具属性行：键值按配置输出，比例属性（攻速/暴击）带 % 后缀，其余直接显示数值
    private void AddItemAttrRow(string key, float value, List<string> keys, List<string> vals)
    {
        if (string.IsNullOrEmpty(key) || value == 0)
            return;
        keys.Add(key);
        bool isPercent = key == "atkspeed" || key == "crit";
        vals.Add(isPercent ? Mathf.RoundToInt(value * 100) + "%" : value.ToString("0.##"));
    }
}