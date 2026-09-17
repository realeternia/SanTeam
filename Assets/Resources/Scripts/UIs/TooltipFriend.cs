using UnityEngine;
using TMPro;
using CommonConfig;

// 羁绊提示（职业/好友/国家）：点击 SelectCardNodeControl（羁绊模式）时显示。
// 结构：第一行 = 羁绊名 + 等级；第二行 = 等级效果（当前级 + 下一级不同参数括号，淡绿）；
// 第三行 = 该羁绊当前上阵的英雄列表（按品质倒排上色，国家不显示）。
public class TooltipFriend : BaseTooltip
{
    // 第一行：羁绊名 + 等级（复用属性格预制体的文字控件，tooltip 最上方）
    private const float NameRowHeight = 30f;
    private TMP_Text textName;

    // 技能行：图标 + 等级效果(最多2行) + 人员列表(1行)，复用 ToolTipHeroSkill.prefab（行高固定100）
    private const float SkillRowHeight = 100f;
    private static GameObject skillRowPrefab;
    private TooltipHeroSkill effectRow;

    protected override void Awake()
    {
        base.Awake();
        CreateNameText();
    }

    // 用属性格预制体生成羁绊名文字控件（锚定 tooltip 左上角）
    private void CreateNameText()
    {
        var attrPrefab = Resources.Load<GameObject>("Prefabs/ToolTipHeroAttr");
        if (attrPrefab == null)
        {
            GameLog.Error("TooltipFriend 属性格预制体加载失败: Prefabs/ToolTipHeroAttr");
            return;
        }

        var nameGo = Instantiate(attrPrefab, rect);
        var nameCell = nameGo.GetComponent<TooltipHeroAttr>();
        if (nameCell == null || nameCell.text == null)
        {
            GameLog.Error("TooltipFriend 属性格预制体缺少 TooltipHeroAttr 组件或文字控件");
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
    }

    // 取技能行（ToolTipHeroSkill.prefab 实例，首次创建后缓存）
    private TooltipHeroSkill GetEffectRow()
    {
        if (effectRow != null)
            return effectRow;
        if (skillRowPrefab == null)
            skillRowPrefab = Resources.Load<GameObject>("Prefabs/ToolTipHeroSkill");
        if (skillRowPrefab == null)
        {
            GameLog.Error("TooltipFriend 技能行预制体加载失败: Prefabs/ToolTipHeroSkill");
            return null;
        }
        var go = Instantiate(skillRowPrefab, rect);
        go.name = "BondEffectRow";
        effectRow = go.GetComponent<TooltipHeroSkill>();
        if (effectRow == null)
        {
            GameLog.Error("TooltipFriend 技能行预制体缺少 TooltipHeroSkill 组件");
            Destroy(go);
        }
        return effectRow;
    }

    // 排列技能行：锚定 tooltip 左上角，行高固定100
    private void LayoutRow(TooltipHeroSkill row, float currentY)
    {
        var rowRt = (RectTransform)row.transform;
        rowRt.anchorMin = new Vector2(0, 1);
        rowRt.anchorMax = new Vector2(0, 1);
        rowRt.pivot = new Vector2(0.5f, 0.5f);
        rowRt.anchoredPosition = new Vector2(250, -currentY - SkillRowHeight * 0.5f);
    }

    public void ShowTooltip(BondTipData data)
    {
        if (data == null)
        {
            HideTooltip();
            return;
        }

        // 第一行：羁绊名 + 等级（好友连线色可选，默认白；国家/职业保持与列表一致的白）
        if (textName != null)
        {
            string name = data.Name;
            if (data.Color.HasValue)
                name = "<color=#" + ColorUtility.ToHtmlStringRGB(data.Color.Value) + ">" + name + "</color>";
            textName.text = name + " Lv" + data.Level;
            textName.gameObject.SetActive(true);
        }

        // 效果行：当前档位技能描述 + 下一档不同参数（括号淡绿）；0级（未激活）按1级文案显示并置灰
        var row = GetEffectRow();
        if (row == null)
        {
            HideTooltip();
            return;
        }
        row.gameObject.SetActive(true);

        int effLv = Mathf.Max(0, Mathf.Min(data.Level, 5));
        string skillText = "";
        if (!string.IsNullOrEmpty(data.SkillId))
        {
            var skillCfg = ConfigManager.GetSkillConfig(data.SkillId, Mathf.Max(1, effLv));
            if (skillCfg != null)
                skillText = skillCfg.Name + " " + ConfigManager.GetSkillDescript(skillCfg, true);
        }

        // 图标：国家护盾用国家图标（data.Icon 已带完整路径）；职业/好友用技能图标
        string icon = data.Icon ?? "";

        // 人员列表：职业/好友显示该羁绊全部成员（参考 TooltipHero：按品质倒排，上阵的按品质上色）；国家不显示
        string listText = "";
        if (data.Kind != BondKind.Force && data.HeroIds != null && data.HeroIds.Count > 0)
            listText = TooltipHero.BuildHeroNameList(data.HeroIds, data.Player, false);

        row.SetFriendSkill(skillText, icon, listText, effLv);

        // 内容填充完毕：名字行下方排效果行，调整背景大小后走基类统一显示/定位
        float currentY = 10f + NameRowHeight + 10f;
        LayoutRow(row, currentY);
        float height = Mathf.Max(50f, currentY + SkillRowHeight + 10f);
        rect.sizeDelta = new Vector2(500, height);
        Show();
    }
}
