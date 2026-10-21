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

        string path = icon.StartsWith("SkillPic/") ? "Textures/" + icon : "Textures/ItemPic/" + icon;
        return Resources.Load<Sprite>(path);
    }
}
