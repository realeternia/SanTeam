using UnityEngine;
using UnityEngine.UI;
using TMPro;

// 技能/好友行控件：承载 ToolTipHeroSkill.prefab
// 技能行 = 图标 + 技能文字(最多3行)；好友组 = 图标 + 技能描述(最多2行) + 人员列表(1行，带底色)
public class TooltipHeroSkill : MonoBehaviour
{
    public TMP_Text textSkill;
    public Image img;
    public TMP_Text textFriend;
    public TMP_Text textSkillLv;

    // 行最小宽度（预制体宽度）与文字排布：文字左边缘 86，右侧留白 14
    public const float RowMinWidth = 500f;
    private const float TextLeft = 86f;
    private const float RowRightMargin = 14f;

    // 文字显示：字号相对预制体略缩小，超出截断
    private const float SkillFontScale = 0.9f;

    // 技能文字区高度：个人技能行无人员列表，可占满整行（最多3行描述）；好友/职业行下方还要放人员列表，保持2行
    private const float SkillTextHeight = 100f;
    private const float FriendSkillTextHeight = 70f;

    // 人员列表：单行显示不换行（过长由外部拉宽 tips），文字下方铺一层略亮的底色与技能描述区分
    private const float FriendListTextHeight = 30f;
    private const float FriendListTextTop = -67.7f;
    private const float FriendListBgHeight = 36f;
    private const float FriendListBgTop = -63f;
    private Image friendListBg;

    // 预制体基准字号（避免每次显示累乘缩小）
    private float skillFontBase = -1f;

    // 技能行：图标 + 文本（行高度固定100，由外部排列），描述可占满整行显示3行；level<=0 时文字置灰（显示未激活的1级效果）
    // 无图标（如未配图的个人技能）时隐藏图标，避免残留上次的图或空图
    public void SetSkill(string text, string icon, int level = 1)
    {
        img.gameObject.SetActive(!string.IsNullOrEmpty(icon));
        if (!string.IsNullOrEmpty(icon))
            img.sprite = Resources.Load<Sprite>("Textures/SkillPic/" + icon);
        textSkill.text = text;
        ApplyStyle(textSkill, ref skillFontBase, SkillTextHeight);
        // 技能行无人员列表，隐藏旧列表/底色残留
        textFriend.gameObject.SetActive(false);
        if (friendListBg != null)
            friendListBg.gameObject.SetActive(false);
        SetLevel(level);
    }

    // 好友组/职业技能：图标 + 技能描述(最多2行) + 人员列表(1行带底色)；
    // 返回该行所需宽度（列表单行不换行，太长时外部按此加宽整个 tips）
    public float SetFriendSkill(string skillText, string icon, string listText, int level = 1)
    {
        textSkill.text = skillText;
        ApplyStyle(textSkill, ref skillFontBase, FriendSkillTextHeight);

        // 人员列表（空则不显示，避免残留上次内容）：不换行，按内容量出所需宽度
        bool hasList = !string.IsNullOrEmpty(listText);
        textFriend.gameObject.SetActive(hasList);
        float listWidth = 0f;
        if (hasList)
        {
            textFriend.text = listText;
            textFriend.enableWordWrapping = false;
            textFriend.overflowMode = TextOverflowModes.Overflow;
            listWidth = Mathf.Max(RowMinWidth - TextLeft - RowRightMargin, textFriend.GetPreferredValues(listText).x);

            // 左对齐锚定：加宽只向右延伸，不压到图标
            var listRt = textFriend.rectTransform;
            listRt.anchorMin = new Vector2(0, 1);
            listRt.anchorMax = new Vector2(0, 1);
            listRt.pivot = new Vector2(0, 1);
            listRt.anchoredPosition = new Vector2(TextLeft, FriendListTextTop);
            listRt.sizeDelta = new Vector2(listWidth, FriendListTextHeight);

            EnsureFriendListBg();
            friendListBg.gameObject.SetActive(true);
        }
        else if (friendListBg != null)
        {
            friendListBg.gameObject.SetActive(false);
        }

        // 图标：无图标（无连接技能）时隐藏；带路径的图标（如国家 Textures/Icons/xxx）直接加载，否则按技能图标 SkillPic/ 前缀
        img.gameObject.SetActive(!string.IsNullOrEmpty(icon));
        if (!string.IsNullOrEmpty(icon))
            img.sprite = Resources.Load<Sprite>(icon.Contains("/") ? icon : "Textures/SkillPic/" + icon);

        SetLevel(level);
        return Mathf.Max(RowMinWidth, TextLeft + listWidth + RowRightMargin);
    }

    // 统一行宽（tips 因名字列表过长整体加宽时调用）：技能描述同步拉宽，底色铺满整行
    public void SetRowWidth(float width)
    {
        float textWidth = Mathf.Max(RowMinWidth - TextLeft - RowRightMargin, width - TextLeft - RowRightMargin);

        // 技能描述左对齐锚定：加宽只向右延伸，左边缘位置与预制体保持一致
        var skillRt = textSkill.rectTransform;
        skillRt.anchorMin = new Vector2(0, 1);
        skillRt.anchorMax = new Vector2(0, 1);
        skillRt.pivot = new Vector2(0, 1);
        skillRt.anchoredPosition = new Vector2(TextLeft, -0.3f);
        skillRt.sizeDelta = new Vector2(textWidth, skillRt.sizeDelta.y);

        if (friendListBg != null)
            friendListBg.rectTransform.sizeDelta = new Vector2(Mathf.Max(0f, width - TextLeft), FriendListBgHeight);
    }

    // 人员列表底色：首次显示时按需创建，铺在列表文字下方，从文字左边缘起（避开左侧图标区），渲染在文字后面
    private void EnsureFriendListBg()
    {
        if (friendListBg != null)
            return;
        var go = new GameObject("FriendListBg", typeof(RectTransform), typeof(Image));
        var rt = (RectTransform)go.transform;
        rt.SetParent(transform, false);
        rt.SetAsFirstSibling();
        rt.anchorMin = new Vector2(0, 1);
        rt.anchorMax = new Vector2(0, 1);
        rt.pivot = new Vector2(0, 1);
        rt.anchoredPosition = new Vector2(TextLeft, FriendListBgTop);
        rt.sizeDelta = new Vector2(RowMinWidth - TextLeft, FriendListBgHeight);
        friendListBg = go.GetComponent<Image>();
        friendListBg.color = SysColor.UI.TooltipListBg;
        friendListBg.raycastTarget = false;
    }

    // 等级角标与置灰：level<=0 表示未激活（技能文字已由外部填1级效果），文字置灰显示
    private void SetLevel(int level)
    {
        textSkillLv.text = level.ToString();
        bool active = level > 0;
        textSkillLv.color = active ? Color.white : Color.gray;
        textSkill.color = active ? Color.white : Color.gray;
    }

    // 文字样式：字号略缩小、超出截断；高度按行类型设定（个人技能行可用满整行高度显示3行）
    private void ApplyStyle(TMP_Text text, ref float fontBase, float height)
    {
        if (fontBase < 0f)
            fontBase = text.fontSize;
        text.fontSize = fontBase * SkillFontScale;
        text.overflowMode = TextOverflowModes.Ellipsis;
        text.enableWordWrapping = true;
        var textRt = text.rectTransform;
        textRt.sizeDelta = new Vector2(textRt.sizeDelta.x, height);
    }
}
