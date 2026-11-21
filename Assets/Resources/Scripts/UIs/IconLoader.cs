using CommonConfig;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum IconSourceType
{
    Path,
    HeroAttr,
    SysAttr
}

// 图标加载组件：按配置把 Resources/Textures/Icons 下的图标赋给 Image
public class IconLoader : MonoBehaviour
{
    public IconSourceType sourceType = IconSourceType.Path;
    public string iconPath;
    public int configId;
    public Image image;

    private static readonly Dictionary<string, Sprite> spriteCache = new Dictionary<string, Sprite>();

    // 静态加载：优先按 Sprite 读取；若图片按默认 Texture 导入则用 Texture2D 运行时生成 Sprite
    public static Sprite LoadSprite(string iconName)
    {
        if (string.IsNullOrEmpty(iconName))
            return null;

        Sprite cached;
        if (spriteCache.TryGetValue(iconName, out cached) && cached != null)
            return cached;

        string path = "Textures/Icons/" + iconName;
        Sprite sprite = Resources.Load<Sprite>(path);
        if (sprite == null)
        {
            Texture2D texture = Resources.Load<Texture2D>(path);
            if (texture == null)
            {
                GameLog.Error(string.Format("IconLoader 图标不存在：{0}", path));
                return null;
            }
            sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        }
        spriteCache[iconName] = sprite;
        return sprite;
    }

    void Start()
    {
        RefreshIcon();
    }

    public void RefreshIcon()
    {
        string path = ResolveIconPath();
        if (string.IsNullOrEmpty(path))
            return;

        Sprite sprite = Resources.Load<Sprite>(path);
        if (sprite == null)
            return;

        if (image == null)
        {
            image = GetComponent<Image>();
        }
        if (image != null)
        {
            image.sprite = sprite;
        }
    }

    public void SetId(int id)
    {
        configId = id;
    }

    private string ResolveIconPath()
    {
        switch (sourceType)
        {
            case IconSourceType.HeroAttr:
            {
                if (!HeroAttrConfig.HasConfig(configId))
                {
                    GameLog.Error(string.Format("IconLoader HeroAttrConfig不存在id={0}", configId));
                    return null;
                }
                HeroAttrConfig cfg = HeroAttrConfig.GetConfig(configId);
                if (string.IsNullOrEmpty(cfg.Icon))
                {
                    return null;
                }
                return "Textures/Icons/" + cfg.Icon;
            }
            case IconSourceType.SysAttr:
            {
                if (!SystemAttrConfig.HasConfig(configId))
                {
                    GameLog.Error(string.Format("IconLoader SystemAttrConfig不存在id={0}", configId));
                    return null;
                }
                SystemAttrConfig cfg = SystemAttrConfig.GetConfig(configId);
                if (string.IsNullOrEmpty(cfg.Icon))
                {
                    return null;
                }
                return "Textures/Icons/" + cfg.Icon;
            }
            default:
            {
                if (!string.IsNullOrEmpty(iconPath))
                {
                    return "Textures/Icons/" + iconPath;
                }
                return null;
            }
        }
    }
}
