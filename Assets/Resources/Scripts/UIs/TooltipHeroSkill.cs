using UnityEngine;
using UnityEngine.UI;
using TMPro;

// 技能/好友行控件：承载 ToolTipHeroSkill.prefab
// 技能行 = 图标 + 技能文字(最多2行)；好友组 = 图标 + 技能描述(最多2行) + 人员列表(1行)
public class TooltipHeroSkill : MonoBehaviour
{
    public TMP_Text textSkill;
    public Image img;

    // 文字显示：字号相对预制体略缩小，超出截断
    private const float SkillFontScale = 0.9f;

    // 预制体基准字号（避免每次显示累乘缩小）
    private float skillFontBase = -1f;

    // 技能行：图标 + 文本（行高度固定100，由外部排列）
    public void SetSkill(string text, string icon)
    {
        img.sprite = Resources.Load<Sprite>("Textures/SkillPic/" + icon);
        textSkill.text = text;
        ApplyStyle(textSkill, ref skillFontBase);
    }

    // 好友组：图标 + 技能描述(最多2行) + 人员列表(1行，直接拼接到描述第3行)；行高固定，不改布局
    public void SetFriendSkill(string skillText, string icon, string listText)
    {
        // 人员列表拼接为第3行（不单独建文本控件、不调整本行高度）
        textSkill.text = skillText + "\n" + listText;
        ApplyStyle(textSkill, ref skillFontBase);

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
