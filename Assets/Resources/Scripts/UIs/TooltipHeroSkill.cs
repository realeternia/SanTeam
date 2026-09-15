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

    // 文字显示：字号相对预制体略缩小，超出截断
    private const float SkillFontScale = 0.9f;

    // 预制体基准字号（避免每次显示累乘缩小）
    private float skillFontBase = -1f;

    // 技能行：图标 + 文本（行高度固定100，由外部排列）
    public void SetSkill(string text, string icon)
    {
        img.sprite = Resources.Load<Sprite>("Textures/SkillPic/" + icon);
        img.gameObject.SetActive(true);
        textSkill.text = text;
        ApplyStyle(textSkill, ref skillFontBase);
        // 技能行无人员列表，隐藏旧列表残留
        textFriend.gameObject.SetActive(false);
    }

    // 好友组/职业技能：图标 + 技能描述(最多2行) + 人员列表(1行)；行高固定，不改布局
    public void SetFriendSkill(string skillText, string icon, string listText)
    {
        textSkill.text = skillText;
        ApplyStyle(textSkill, ref skillFontBase);

        // 人员列表（空则不显示，避免残留上次内容）
        bool hasList = !string.IsNullOrEmpty(listText);
        textFriend.gameObject.SetActive(hasList);
        if (hasList)
            textFriend.text = listText;

        // 图标：无图标（无连接技能）时隐藏
        img.gameObject.SetActive(!string.IsNullOrEmpty(icon));
        if (!string.IsNullOrEmpty(icon))
            img.sprite = Resources.Load<Sprite>("Textures/SkillPic/" + icon);
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
