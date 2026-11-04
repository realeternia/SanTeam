using System;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 剧本英雄数据访问封装。
/// HeroConfig 不再持有全局 Side/Quality 字段，改为按剧本列 Scenario1/2/3（格式“品质:阵营”，空=该剧本不出场）解析：
/// 剧本Id 1 群雄割据 / 2 三国鼎立 / 3 三家归晋；0 = 老存档，英雄池取剧本1+2并集（品质/阵营优先剧本2，其次剧本1，最后剧本3兜底）。
/// 同时缓存“品质无关”的英雄基础面板与专属技能强度，在切换剧本时按当前剧本品质重算，保证数值与品质一致。
/// </summary>
public static class HeroScenario
{
    /// <summary>老存档剧本Id：英雄池使用剧本1+2并集</summary>
    public const int LegacyId = 0;

    private const int MaxScenarioId = 3;

    private class HeroInfo
    {
        public int[] quality = new int[MaxScenarioId + 1];
        public int[] side = new int[MaxScenarioId + 1];
    }

    private class BaseHeroPanel
    {
        public float atk;
        public float hp;
    }

    private class BaseSkillStrength
    {
        public int heroId;
        public SkillConfig cfg;
        public float damage;
        public float heal;
        public float factor; // 品质系数之外的修正：MP消耗系数 × 法术倍率
    }

    private static int currentId = LegacyId;
    private static Dictionary<int, HeroInfo> heroInfos = new Dictionary<int, HeroInfo>();
    private static Dictionary<int, BaseHeroPanel> baseHeroPanels = new Dictionary<int, BaseHeroPanel>();
    private static List<BaseSkillStrength> baseSkillStrengths = new List<BaseSkillStrength>();

    /// <summary>当前剧本Id（0 = 老存档）</summary>
    public static int CurrentId { get { return currentId; } }

    /// <summary>当前是否老存档（英雄池为剧本1+2并集）</summary>
    public static bool IsLegacy { get { return currentId == LegacyId; } }

    /// <summary>剧本Id是否有效（0 或 ScenarioConfig 已登记）</summary>
    public static bool IsValidScenarioId(int scenarioId)
    {
        return scenarioId == LegacyId || ScenarioConfig.HasConfig(scenarioId);
    }

    /// <summary>剧本名（0 显示为老存档）</summary>
    public static string GetScenarioName(int scenarioId)
    {
        if (scenarioId == LegacyId)
            return "老存档(剧本1+2)";
        return ScenarioConfig.HasConfig(scenarioId) ? ScenarioConfig.GetConfig(scenarioId).Name : "未知剧本";
    }

    /// <summary>清理解析/基准缓存（配置重载时调用）</summary>
    public static void Reset()
    {
        heroInfos.Clear();
        baseHeroPanels.Clear();
        baseSkillStrengths.Clear();
    }

    /// <summary>切换当前剧本，并按新剧本品质重算英雄面板与专属技能强度</summary>
    public static void SetCurrent(int scenarioId)
    {
        if (!IsValidScenarioId(scenarioId))
        {
            GameLog.Error(string.Format("HeroScenario.SetCurrent: 剧本Id[{0}]无效，回退为老存档(0)", scenarioId));
            scenarioId = LegacyId;
        }
        currentId = scenarioId;
        ApplyCurrentScenario();
        GameLog.Debug("HeroScenario 当前剧本: " + GetScenarioName(scenarioId));
    }

    /// <summary>英雄当前剧本品质（不可用返回0）</summary>
    public static int GetQuality(HeroConfig heroCfg)
    {
        return heroCfg == null ? 0 : GetQuality(heroCfg.Id);
    }

    /// <summary>英雄当前剧本品质（不可用返回0）</summary>
    public static int GetQuality(int heroId)
    {
        var info = GetInfo(heroId);
        return info == null ? 0 : ResolveQuality(info, currentId);
    }

    /// <summary>英雄当前剧本阵营Id（不可用返回0）</summary>
    public static int GetSide(HeroConfig heroCfg)
    {
        return heroCfg == null ? 0 : GetSide(heroCfg.Id);
    }

    /// <summary>英雄当前剧本阵营Id（不可用返回0）</summary>
    public static int GetSide(int heroId)
    {
        var info = GetInfo(heroId);
        return info == null ? 0 : ResolveSide(info, currentId);
    }

    /// <summary>英雄是否在当前剧本出场</summary>
    public static bool IsAvailable(int heroId)
    {
        return IsAvailable(heroId, currentId);
    }

    /// <summary>英雄是否在指定剧本出场（0 = 剧本1+2并集）</summary>
    public static bool IsAvailable(int heroId, int scenarioId)
    {
        var info = GetInfo(heroId);
        if (info == null)
            return false;
        if (scenarioId == LegacyId)
            return HasField(info, 1) || HasField(info, 2);
        if (scenarioId < 1 || scenarioId > MaxScenarioId)
            return false;
        return HasField(info, scenarioId);
    }

    /// <summary>当前剧本可用英雄Id列表</summary>
    public static List<int> GetHeroIds()
    {
        return GetHeroIds(currentId);
    }

    /// <summary>指定剧本可用英雄Id列表（0 = 剧本1+2并集）</summary>
    public static List<int> GetHeroIds(int scenarioId)
    {
        var ids = new List<int>();
        foreach (var heroCfg in HeroConfig.ConfigList)
        {
            if (heroCfg != null && heroCfg.Id > 0 && IsAvailable(heroCfg.Id, scenarioId))
                ids.Add(heroCfg.Id);
        }
        if (ids.Count == 0)
            GameLog.Error("HeroScenario.GetHeroIds: 剧本[" + scenarioId + "]没有可用英雄，请检查 HeroConfig 剧本列");
        return ids;
    }

    /// <summary>登记品质无关的英雄基础面板（PostModify 中调用）</summary>
    public static void RegisterBaseHeroPanel(int heroId, float atk, float hp)
    {
        var panel = new BaseHeroPanel();
        panel.atk = atk;
        panel.hp = hp;
        baseHeroPanels[heroId] = panel;
    }

    /// <summary>登记品质无关的专属技能强度（PostModify 中调用）</summary>
    public static void RegisterBaseSkillStrength(int heroId, SkillConfig cfg, float damage, float heal, float factor)
    {
        if (cfg == null)
            return;
        var data = new BaseSkillStrength();
        data.heroId = heroId;
        data.cfg = cfg;
        data.damage = damage;
        data.heal = heal;
        data.factor = factor;
        baseSkillStrengths.Add(data);
    }

    // 按当前剧本品质重算英雄主属性面板与专属技能强度
    private static void ApplyCurrentScenario()
    {
        foreach (var kv in baseHeroPanels)
        {
            if (!HeroConfig.HasConfig(kv.Key))
                continue;
            var heroCfg = HeroConfig.GetConfig(kv.Key);
            float factor = CombatConst.QualityAttrFactors[Mathf.Clamp(GetQuality(kv.Key), 1, CombatConst.QualityAttrFactors.Length) - 1];
            heroCfg.Atk = (int)Math.Round(kv.Value.atk * factor);
            heroCfg.Hp = (int)Math.Round(kv.Value.hp * factor);
        }

        foreach (var data in baseSkillStrengths)
        {
            float factor = CombatConst.QualitySkillFactors[Mathf.Clamp(GetQuality(data.heroId), 1, CombatConst.QualitySkillFactors.Length) - 1] * data.factor;
            data.cfg.DamageStrength = data.damage * factor;
            data.cfg.HealStrength = data.heal * factor;
        }
    }

    // 剧本字段「品质:阵营」，如 3:蜀；空字段表示该剧本不出场
    private static void ParseScenarioField(HeroConfig heroCfg, int scenarioId, string field, HeroInfo info)
    {
        if (string.IsNullOrEmpty(field))
            return;

        string[] parts = field.Split(':');
        if (parts.Length != 2)
        {
            GameLog.Error(string.Format("HeroScenario: 英雄[{0}]剧本{1}字段[{2}]格式错误，应为 品质:阵营（如3:蜀）", heroCfg.Name, scenarioId, field));
            return;
        }

        int quality;
        if (!int.TryParse(parts[0].Trim(), out quality) || quality < 1)
        {
            GameLog.Error(string.Format("HeroScenario: 英雄[{0}]剧本{1}字段[{2}]品质非法", heroCfg.Name, scenarioId, field));
            return;
        }

        int side = ConfigManager.ParseSideName(parts[1].Trim());
        if (side == 0)
        {
            GameLog.Error(string.Format("HeroScenario: 英雄[{0}]剧本{1}字段[{2}]阵营无法识别", heroCfg.Name, scenarioId, field));
            return;
        }

        info.quality[scenarioId] = quality;
        info.side[scenarioId] = side;
    }

    private static HeroInfo GetInfo(int heroId)
    {
        if (heroInfos.TryGetValue(heroId, out HeroInfo info))
            return info;

        if (!HeroConfig.HasConfig(heroId))
        {
            GameLog.Error("HeroScenario: 英雄Id[" + heroId + "]在HeroConfig中不存在");
            return null;
        }

        var heroCfg = HeroConfig.GetConfig(heroId);
        info = new HeroInfo();
        ParseScenarioField(heroCfg, 1, heroCfg.Scenario1, info);
        ParseScenarioField(heroCfg, 2, heroCfg.Scenario2, info);
        ParseScenarioField(heroCfg, 3, heroCfg.Scenario3, info);
        heroInfos[heroId] = info;
        return info;
    }

    private static bool HasField(HeroInfo info, int scenarioId)
    {
        return info.quality[scenarioId] > 0 && info.side[scenarioId] > 0;
    }

    // 当前剧本有该英雄则取当前剧本；未出场时退回老存档口径（优先剧本2，其次剧本1，最后剧本3兜底）。
    // 未出场英雄不会进入英雄池，退回口径仅用于图鉴/调试窗口等非池内展示，避免出现品质0/阵营0。
    private static int ResolveQuality(HeroInfo info, int scenarioId)
    {
        if (scenarioId >= 1 && scenarioId <= MaxScenarioId && info.quality[scenarioId] > 0)
            return info.quality[scenarioId];
        if (info.quality[2] > 0) return info.quality[2];
        if (info.quality[1] > 0) return info.quality[1];
        return info.quality[3];
    }

    private static int ResolveSide(HeroInfo info, int scenarioId)
    {
        if (scenarioId >= 1 && scenarioId <= MaxScenarioId && info.side[scenarioId] > 0)
            return info.side[scenarioId];
        if (info.side[2] > 0) return info.side[2];
        if (info.side[1] > 0) return info.side[1];
        return info.side[3];
    }
}
