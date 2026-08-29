using CommonConfig;
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
                    Debug.LogError(string.Format("IconLoader HeroAttrConfig不存在id={0}", configId));
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
                    Debug.LogError(string.Format("IconLoader SystemAttrConfig不存在id={0}", configId));
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
