using UnityEngine;
using UnityEngine.UI;
using TMPro;

// 属性单元格：图标 + 属性值（ToolTipHeroAttr.prefab，属性区每格一个）
public class TooltipHeroAttr : MonoBehaviour
{
    public Image img;
    public TMP_Text text;

    // 设置属性：图标 + 数值
    public void SetAttr(string icon, string value)
    {
        img.sprite = Resources.Load<Sprite>("Textures/Icons/" + icon);
        text.text = value;
    }
}
