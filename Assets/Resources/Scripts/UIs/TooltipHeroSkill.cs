using UnityEngine;
using UnityEngine.UI;
using TMPro;

// 技能/好友行控件：承载 ToolTipHeroSkill.prefab
// 技能行 = 图标 + 技能文字(最多2行)；好友组 = 图标 + 技能描述(最多2行) + 人员列表(1行)
public class TooltipHeroSkill : MonoBehaviour
{
    public TMP_Text textSkill;
    public Image img;
    public TMP_Text textFriend;
    public TMP_Text textSkillLv;

    // 文字显示：字号相对预制体略缩小，超出截断
    private const float SkillFontScale = 0.9f;

    // 预制体基准字号（避免每次显示累乘缩小）
    private float skillFontBase = -1f;

    // 技能行：图标 + 文本（行高度固定100，由外部排列）；level<=0 时文字置灰（显示未激活的1级效果）
    // 无图标（如未配图的个人技能）时隐藏图标，避免残留上次的图或空图
    public void SetSkill(string text, string icon, int level = 1)
    {
        img.gameObject.SetActive(!string.IsNullOrEmpty(icon));
        if (!string.IsNullOrEmpty(icon))
            img.sprite = Resources.Load<Sprite>("Textures/SkillPic/" + icon);
        textSkill.text = text;
        ApplyStyle(textSkill, ref skillFontBase);
        // 技能行无人员列表，隐藏旧列表残留
        textFriend.gameObject.SetActive(false);
        SetLevel(level);
    }

    // 好友组/职业技能：图标 + 技能描述(最多2行) + 人员列表(1行)；行高固定，不改布局
    public void SetFriendSkill(string skillText, string icon, string listText, int level = 1)
    {
        textSkill.text = skillText;
        ApplyStyle(textSkill, ref skillFontBase);

        // 人员列表（空则不显示，避免残留上次内容）
        bool hasList = !string.IsNullOrEmpty(listText);
        textFriend.gameObject.SetActive(hasList);
        if (hasList)
            textFriend.text = listText;

        // 图标：无图标（无连接技能）时隐藏；带路径的图标（如国家 Textures/Icons/xxx）直接加载，否则按技能图标 SkillPic/ 前缀
        img.gameObject.SetActive(!string.IsNullOrEmpty(icon));
        if (!string.IsNullOrEmpty(icon))
            img.sprite = Resources.Load<Sprite>(icon.Contains("/") ? icon : "Textures/SkillPic/" + icon);

        SetLevel(level);
    }

    // 等级角标与置灰：level<=0 表示未激活（技能文字已由外部填1级效果），文字置灰显示
    private void SetLevel(int level)
    {
        textSkillLv.text = level.ToString();
        bool active = level > 0;
        textSkillLv.color = active ? Color.white : Color.gray;
        textSkill.color = active ? Color.white : Color.gray;
    }

    // 文字样式：字号略缩小、超出截断
    private void ApplyStyle(TMP_Text text, ref float fontBase)
    {
        if (fontBase < 0f)
            fontBase = text.fontSize;
        text.fontSize = fontBase * SkillFontScale;
        text.overflowMode = TextOverflowModes.Ellipsis;
        text.enableWordWrapping = true;
    }
}
