using UnityEngine;
using CommonConfig;

public static class SysColor
{
    private static readonly Color[] ArmsLevelColors = new Color[]
    {
        new Color(0.5f, 0.5f, 0.5f, 1f),
        Color.white,
        new Color(0.27f, 0.51f, 0.9f, 1f),
        new Color(0.2f, 0.8f, 0.2f, 1f),
        new Color(1f, 0.85f, 0f, 1f),
        new Color(1f, 0.5f, 0f, 1f)
    };

    public static Color GetArmsLevelColor(int level)
    {
        if (level >= 0 && level < ArmsLevelColors.Length)
            return ArmsLevelColors[level];
        return ArmsLevelColors[ArmsLevelColors.Length - 1];
    }

    // 英雄品质色（1普通-白 2优秀-绿 3精良-蓝 4史诗-紫）
    public static Color GetQualityColor(int quality)
    {
        switch (quality)
        {
            case 1: return new Color(255 / 255f, 255 / 255f, 255 / 255f); // 普通-白
            case 2: return new Color(30 / 255f, 255 / 255f, 0 / 255f);    // 优秀-绿
            case 3: return new Color(0 / 255f, 112 / 255f, 221 / 255f);   // 精良-蓝
            default: return new Color(163 / 255f, 53 / 255f, 238 / 255f); // 史诗-紫
        }
    }

    // 阵营背景色（1魏 2蜀 3吴 4晋 5群 6神，其他-灰）
    public static Color GetSideColor(int side)
    {
        switch (side)
        {
            case 1: return new Color(40 / 255f, 70 / 255f, 0 / 255f, 255 / 255f);
            case 2: return new Color(0 / 255f, 35 / 255f, 100 / 255f, 255 / 255f);
            case 3: return new Color(100 / 255f, 0 / 255f, 0 / 255f, 255 / 255f);
            case 4: return new Color(30 / 255f, 100 / 255f, 110 / 255f, 255 / 255f);
            case 5: return new Color(90 / 255f, 50 / 255f, 110 / 255f, 255 / 255f);
            case 6: return new Color(120 / 255f, 90 / 255f, 30 / 255f, 255 / 255f);
            default: return new Color(50 / 255f, 50 / 255f, 50 / 255f, 255 / 255f);
        }
    }

    // 技能属性连接线颜色（ap-智谋蓝 might-武力红 其他-黄绿）
    public static Color GetSkillAttrColor(string attr)
    {
        switch (attr)
        {
            case "ap": return new Color(0.55f, 0.55f, 1f, 0.6f);
            case "might": return new Color(0.95f, 0.4f, 0.4f, 0.6f);
            default: return new Color(0.7f, 0.8f, 0.3f, 0.6f);
        }
    }

    public static Color GetColorByValue(string attrName, int value)
    {
        var cfg = HeroAttrConfig.GetConfigByname(attrName);
        if (cfg == null || string.IsNullOrEmpty(cfg.ColorRule))
            return Color.white;
        return ParseColorRule(cfg.ColorRule, value);
    }

    public static string GetColoredText(string attrName, int value)
    {
        Color color = GetColorByValue(attrName, value);
        string colorHex = ColorUtility.ToHtmlStringRGB(color);
        return $"<color=#{colorHex}>{value}</color>";
    }

    public static Color GetTextColorOnBackground(Color bgColor)
    {
        float brightness = 0.299f * bgColor.r + 0.587f * bgColor.g + 0.114f * bgColor.b;
        return brightness > 0.65f ? new Color(0.4f, 0.4f, 0.4f, 1f) : Color.white;
    }

    private static Color ParseColorRule(string rule, int value)
    {
        if (string.IsNullOrEmpty(rule))
            return Color.white;

        string[] rules = rule.Split(',');
        foreach (string r in rules)
        {
            string[] parts = r.Split(':');
            if (parts.Length != 2)
                continue;

            string thresholdStr = parts[0].Trim();
            string colorStr = parts[1].Trim();

            if (TryMatchThreshold(thresholdStr, value))
            {
                if (ColorUtility.TryParseHtmlString(colorStr, out Color color))
                    return color;
            }
        }

        return Color.white;
    }

    private static bool TryMatchThreshold(string thresholdStr, int value)
    {
        if (thresholdStr.Contains("-"))
        {
            string[] range = thresholdStr.Split('-');
            if (range.Length == 2 && int.TryParse(range[0], out int min) && int.TryParse(range[1], out int max))
                return value >= min && value <= max;
        }
        else if (int.TryParse(thresholdStr, out int threshold))
        {
            return value >= threshold;
        }
        return false;
    }

    public static class Theme
    {
        public static readonly Color CellNormal = new Color(0.2f, 0.2f, 0.2f, 0.8f);
        public static readonly Color CellNormalDark = new Color(0, 0, 0, 1);
        public static readonly Color CellSelected = new Color(0.3f, 0.7f, 0.4f, 1f);
        public static readonly Color CellDisabled = new Color(0.1f, 0.1f, 0.1f, 0.5f);
        public static readonly Color ActedHeroTextColor = new Color(0.5f, 0.5f, 0.5f, 1f);
    }

    public static class UI
    {
        public static readonly Color DropDownNormal = new Color(0.2f, 0.2f, 0.25f, 0.9f);
        public static readonly Color DropDownSelected = new Color(0.3f, 0.5f, 0.7f, 1f);
        public static readonly Color DropDownHover = new Color(0.35f, 0.35f, 0.4f, 0.95f);

        public static readonly Color BorderColor = new Color(0.5f, 0.5f, 0.5f, 1f);
        public static readonly Color BorderSelectedColor = Color.green;

        public static readonly Color MatchColor = new Color(0.3f, 0.7f, 0.3f, 1f);

        public static readonly Color DragHighlightColor = Color.green;
        public static readonly Color DragResetColor = Color.white;

        public static readonly Color CheckBtnSelected = new Color(1f, 0.843f, 0f, 1f);
        public static readonly Color CheckBtnNormal = new Color(0.15f, 0.15f, 0.15f, 1f);
    }

    public static class Battle
    {
        public static readonly Color DamageColor = new Color(1f, 0f, 0f);
        public static readonly Color HealColor = new Color(0f, 1f, 0f);
        public static readonly Color FoodLossColor = Color.red;
        public static readonly Color FoodGainColor = Color.green;
        public static readonly Color DeadColor = new Color(0.3f, 0.3f, 0.3f, 1f);
        public static readonly Color HealthLowColor = new Color(0.4f, 0.33f, 0f);
        public static readonly Color HealthNormalColor = new Color(0f, 0.4f, 0.1f);
        public static readonly Color HealthWarningColor = Color.yellow;
        public static readonly Color AttackSuccessColor = Color.red;
        public static readonly Color AttackFailColor = Color.green;
        public static readonly Color CapturedOutlineColor = Color.red;
    }

    public static class Hero
    {
        public static readonly Color TierHighColor = Color.red;
        public static readonly Color TierMediumColor = Color.yellow;
        public static readonly Color TierLowColor = Color.green;
    }

    public static class Chess
    {
        public static readonly Color GoldMain = new Color(1f, 0.843f, 0f, 1f);
        public static readonly Color GoldEmission = new Color(1f, 0.7f, 0f, 1f);
        public static readonly Color GoldOutline = new Color(0.9f, 0.7f, 0.1f, 1f);
        public static readonly Color GoldSpec = new Color(1f, 0.9f, 0.5f, 1f);

        public static readonly Color SilverMain = new Color(0.753f, 0.753f, 0.753f, 1f);
        public static readonly Color SilverEmission = new Color(0.4f, 0.4f, 0.45f, 1f);
        public static readonly Color SilverOutline = new Color(0.6f, 0.6f, 0.65f, 1f);
        public static readonly Color SilverSpec = new Color(1f, 1f, 1f, 1f);
    }

    public static class WorldMap
    {
        public static readonly Color RoadFriendlyColor = new Color(0.2f, 0.8f, 0.2f, 0.4f);
        public static readonly Color RoadNeutralColor = new Color(0.6f, 0.5f, 0.35f, 0.4f);
        public static readonly Color RoadHostileColor = new Color(0.9f, 0.2f, 0.2f, 0.4f);
    }

    public static class Tech
    {
        public static readonly Color BattleColor = new Color(0.8f, 0.25f, 0.2f, 1f);
        public static readonly Color DevelopmentColor = new Color(0.2f, 0.65f, 0.3f, 1f);
        public static readonly Color InstitutionColor = new Color(0.25f, 0.45f, 0.85f, 1f);
        public static readonly Color EngineeringColor = new Color(0.85f, 0.7f, 0.15f, 1f);
        public static readonly Color LockedColor = new Color(0.35f, 0.35f, 0.35f, 1f);

        public static Color GetCategoryColor(string category)
        {
            switch (category)
            {
                case "Battle": return BattleColor;
                case "Development": return DevelopmentColor;
                case "Institution": return InstitutionColor;
                case "Engineering": return EngineeringColor;
                default: return LockedColor;
            }
        }
    }

    public static class City
    {
        public static readonly Color LevelColor = new Color(0.56f, 0.93f, 0.56f, 1f);
    }

    // 属性阶级色（职业图标/属性文本分档着色，最低档不着色由调用方保持原色）
    public static class Tier
    {
        public static readonly Color Purple = new Color(0.3f, 0f, 0.6f);   // 顶级-深紫
        public static readonly Color Magenta = new Color(0.8f, 0f, 1f);    // 卓越-洋红
        public static readonly Color Orange = new Color(1f, 0.5f, 0f);     // 优良-橙
        // 其余档位直接使用 Color.red / Color.yellow / Color.green
    }

    // 卡片相关颜色
    public static class Card
    {
        public static readonly Color SoldGray = new Color(0.3f, 0.3f, 0.3f);            // 售出灰度
        public static readonly Color ItemPanel = new Color(0.6037736f, 0.46531343f, 0.13955145f); // 道具卡面板默认色
    }

    // 战斗飘字颜色
    public static class BattleText
    {
        public static readonly Color JobLink = new Color(0.4f, 1f, 0.4f);   // 羁绊加成-绿
        public static readonly Color RealDamage = new Color(0.6f, 0f, 0.8f); // 真实伤害-紫
        public static readonly Color SkillName = new Color(1f, 0.9f, 0.1f);  // 技能触发-金黄
        public static readonly Color Zhiheng = new Color(1f, 0.3f, 0.3f);    // 制衡-红
    }

    // 玩家状态
    public static class Player
    {
        public static readonly Color DeadBg = new Color(0.1f, 0.1f, 0.1f, 0.8f); // 淘汰玩家头像底色（闪烁终止色）
    }
}
