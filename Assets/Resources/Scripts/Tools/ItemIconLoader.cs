using UnityEngine;

/// <summary>
/// 道具图标加载：ItemConfig.Icon 以 "SkillPic/" 开头时取技能图标目录，否则取道具图标目录
/// </summary>
public static class ItemIconLoader
{
    public static Sprite Load(string icon)
    {
        if (string.IsNullOrEmpty(icon))
            return null;

        if (icon.StartsWith("SkillPic/"))
            return Resources.Load<Sprite>("Textures/" + icon);

        // 先找道具图标目录，找不到再回退到 Textures 根目录（如 drandom/dcompose 这类整图）
        Sprite sprite = Resources.Load<Sprite>("Textures/ItemPic/" + icon);
        return sprite != null ? sprite : Resources.Load<Sprite>("Textures/" + icon);
    }
}
