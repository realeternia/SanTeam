using System.Collections.Generic;
using System.Text;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 兵种连锁（金铲铲式职业羁绊）：战斗开始时统计本侧同职业英雄数量，
/// 按 SkillConfig 职业技能行（Sname=JobConfig.SkillId）的 LinkSelf/LinkTeam 施加被动属性加成，不走技能系统。
/// 档位：上阵该职业 1/2/3/4/5 人时对应职业技能 Lv1~5 行。
/// - LinkSelf：连接英雄（该职业每个英雄自身）获得的属性
/// - LinkTeam：我方其他英雄（全队含士兵）获得的总量，配置即该档位总量，不再乘人数
/// 数值统一由 SkillConfig 表配置，本类不再硬编码。
/// </summary>
public static class JobLinkManager
{
    // 职业羁绊档位：上阵该职业英雄数达到 1/2/3/4/5 人时对应职业技能 Lv1~5
    private static readonly int[] linkTiers = { 1, 2, 3, 4, 5 };

    private struct AttrBonus
    {
        public string Attr;
        public float Value;
    }

    public static void ApplyJobLinks()
    {
        var handledSides = new HashSet<int>();
        foreach (var player in GameManager.Instance.players)
        {
            if (!handledSides.Add(player.battleSide))
                continue;
            ApplyJobLinks(player.battleSide);
        }
    }

    private static void ApplyJobLinks(int side)
    {
        var units = WorldManager.Instance.GetUnitsMySide(side);
        if (units.Count == 0)
            return;

        // 按职业归组英雄
        var jobHeroes = new Dictionary<string, List<Chess>>();
        foreach (var unit in units)
        {
            if (!unit.isHero || unit.hp <= 0)
                continue;
            var job = HeroConfig.GetConfig(unit.heroId).Job;
            if (!jobHeroes.TryGetValue(job, out var list))
            {
                list = new List<Chess>();
                jobHeroes[job] = list;
            }
            list.Add(unit);
        }

        foreach (var kv in jobHeroes)
        {
            var cfg = GetTierConfig(kv.Key, kv.Value.Count);
            if (cfg == null)
                continue;

            var self = ParseBonuses(cfg.LinkSelf);
            var team = ParseBonuses(cfg.LinkTeam);

            // LinkSelf：该职业每个连接英雄自身获得加成
            foreach (var hero in kv.Value)
                foreach (var b in self)
                    ApplyAttr(hero, b.Attr, b.Value);

            // LinkTeam：该档位全队（含英雄与士兵）获得的总量
            foreach (var unit in units)
                foreach (var b in team)
                    ApplyAttr(unit, b.Attr, b.Value);
        }
    }

    /// <summary>
    /// 当前档位对应的职业技能配置行：上阵该职业英雄数即为档位等级（1~5人=Lv1~5）。
    /// </summary>
    public static SkillConfig GetTierConfig(string job, int fieldCount)
    {
        var jobCfg = ConfigManager.GetJobConfig(job);
        var sname = jobCfg != null ? jobCfg.SkillId : null;
        if (string.IsNullOrEmpty(sname))
            return null;

        var lv = GetTierLevel(fieldCount);
        if (lv <= 0)
            return null;
        return SkillConfig.GetConfig(sname, lv);
    }

    // 当前生效档位等级（Lv1~5），上阵0人返回0
    private static int GetTierLevel(int fieldCount)
    {
        var lv = 0;
        for (var i = 0; i < linkTiers.Length; i++)
        {
            if (fieldCount >= linkTiers[i])
                lv = i + 1;
        }
        return lv;
    }

    /// <summary>
    /// 生成职业羁绊的 tooltip 富文本：只显示当前档与下一档两行，
    /// 格式"(N人) 连接英雄加成 | 我方其他英雄加成"，当前档绿色、下一档灰色。
    /// 商店/排行榜等无上阵上下文（上阵0人）时默认显示1级档。
    /// </summary>
    public static string GetJobLinkTipText(string job, int fieldCount)
    {
        var jobCfg = ConfigManager.GetJobConfig(job);
        var sname = jobCfg != null ? jobCfg.SkillId : null;
        if (string.IsNullOrEmpty(sname))
            return "";

        // 当前档：上阵0人（商店卡等）默认取1级
        var activeLv = GetTierLevel(fieldCount);
        if (activeLv <= 0)
            activeLv = 1;

        var sb = new StringBuilder();
        AppendTierLine(sb, sname, activeLv, true);
        AppendTierLine(sb, sname, activeLv + 1, false);
        return sb.ToString();
    }

    // 追加一行档位文本（isCurrent=true 绿色，否则灰色）；等级超出配置时不追加
    private static void AppendTierLine(StringBuilder sb, string sname, int lv, bool isCurrent)
    {
        if (lv < 1 || lv > linkTiers.Length)
            return;
        var cfg = SkillConfig.GetConfig(sname, lv);
        if (cfg == null)
            return;

        sb.Append('\n');
        sb.Append(isCurrent ? "<color=green>" : "<color=#808080>");
        sb.Append('(').Append(linkTiers[lv - 1]).Append("人) ");
        sb.Append(AttrText(ParseBonuses(cfg.LinkSelf)));
        sb.Append(" | ");
        sb.Append(AttrText(ParseBonuses(cfg.LinkTeam)));
        sb.Append("</color>");
    }

    // 解析 "attr+value,attr+value" 格式的加成串
    private static List<AttrBonus> ParseBonuses(string str)
    {
        var list = new List<AttrBonus>();
        if (string.IsNullOrEmpty(str))
            return list;

        foreach (var seg in str.Split(','))
        {
            var idx = seg.LastIndexOf('+');
            if (idx <= 0)
                continue;
            float v;
            if (!float.TryParse(seg.Substring(idx + 1), out v))
                continue;
            list.Add(new AttrBonus { Attr = seg.Substring(0, idx), Value = v });
        }
        return list;
    }

    private static string AttrText(List<AttrBonus> list)
    {
        var sb = new StringBuilder();
        for (var i = 0; i < list.Count; i++)
        {
            if (i > 0)
                sb.Append("、");
            sb.Append(AttrName(list[i].Attr)).Append("+").Append(FormatValue(list[i].Attr, list[i].Value));
        }
        return sb.Length > 0 ? sb.ToString() : "无";
    }

    private static string AttrName(string attr)
    {
        switch (attr)
        {
            case "atk": return "攻";
            case "ap": return "法强";
            case "might": return "武力";
            case "armor": return "护甲";
            case "magicRes": return "魔抗";
            case "maxHp": return "生命";
            case "critRate": return "暴击";
            case "attackRate": return "攻速";
            case "soldierAtk": return "士兵攻";
            case "soldierHp": return "士兵生命";
            case "range": return "射程";
            default: return attr;
        }
    }

    private static string FormatValue(string attr, float v)
    {
        if (attr == "critRate" || attr == "soldierAtk" || attr == "soldierHp")
            return Mathf.RoundToInt(v * 100) + "%";
        if (v < 1f)
            return v.ToString("0.##");
        return ((int)v).ToString();
    }

    private static void ApplyAttr(Chess unit, string attr, float value)
    {
        switch (attr)
        {
            case "atk":
            case "ap":
            case "might":
                unit.AddAttr(attr, (int)value);
                break;
            case "armor":
                unit.armor += (int)value;
                break;
            case "magicRes":
                unit.magicRes += (int)value;
                break;
            case "maxHp":
            {
                var add = (int)value;
                unit.maxHp += add;
                unit.hp += add;
                if (unit.heroInfo != null)
                    unit.heroInfo.SetHpRate(unit.hp, unit.maxHp);
                break;
            }
            case "critRate":
                unit.critRate += value;
                break;
            case "attackRate":
                unit.attackRate += value;
                break;
            case "soldierAtk":
                // 相的羁绊：全军士兵攻击+%
                if (!unit.isHero)
                    unit.soldierAtkRate += value;
                break;
            case "soldierHp":
            {
                // 相的羁绊：全军士兵生命+%
                if (unit.isHero)
                    break;
                var add = (int)(unit.maxHp * value);
                unit.maxHp += add;
                unit.hp += add;
                break;
            }
            case "range":
                // 弩的羁绊：远程单位（射程>20，近战为17）射程增加
                if (unit.attackRange > 20f)
                    unit.attackRange += value;
                break;
        }
    }
}
