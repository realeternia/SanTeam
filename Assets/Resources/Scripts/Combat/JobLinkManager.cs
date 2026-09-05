using System.Collections.Generic;
using System.Text;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 兵种连锁（金铲铲式职业羁绊）：战斗开始时统计本侧同职业英雄数量，
/// 按 SkillConfig 职业技能行（Sname=JobConfig.SkillId）触发羁绊效果。档位：上阵 1/2/3/4/5 人对应职业技能 Lv1~5 行。
/// 效果分两类：
/// 1. 属性加成（不走技能系统）：按 LinkSelf/LinkTeam/AuroAttrs 施加被动属性
///    - LinkSelf：连接英雄（该职业每个英雄自身）获得的属性；其中 soldierAtk/soldierHp 例外，施加给本侧全部士兵（全军士兵）
///    - LinkTeam：我方其他英雄（除该职业英雄外的全体英雄）获得的总量，配置即该档位总量，不再乘人数
///    - AuroAttrs：光环技能效果（ApplyJobLinks 后由 ApplyAuroAttrs 阶段单独结算），遍历每个英雄携带的光环技能，
///      各自对本侧全体英雄（含提供者）生效，效果值受光环来源英雄 auroEffectRate 修正
/// 2. 脚本技能效果：职业技能行 ScriptName 挂真实技能脚本，运行时 SetJobSkillLevel 切到当前档位行，
///    通过技能事件生效（如 枪·眩晕/戟·AOE溅射/炮·AOE范围走 HitBuff 系技能；扇·负面buff延长/琴·正面buff延长走 ModifyBuffTime 的 OnAddBuff 事件）
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
            // 光环技能效果：独立于职业属性加成，按每个英雄携带的光环技能对本侧全体英雄生效
            ApplyAuroAttrs(player.battleSide);
        }
    }

    private static void ApplyJobLinks(int side)
    {
        var allMySideUnits = WorldManager.Instance.GetUnitsMySide(side);
        if (allMySideUnits.Count == 0)
            return;

        // 按职业归组英雄
        var heroesByJob = new Dictionary<string, List<Chess>>();
        foreach (var unit in allMySideUnits)
        {
            if (!unit.isHero || unit.hp <= 0)
                continue;
            var job = HeroConfig.GetConfig(unit.heroId).Job;
            if (!heroesByJob.TryGetValue(job, out var jobHeroes))
            {
                jobHeroes = new List<Chess>();
                heroesByJob[job] = jobHeroes;
            }
            jobHeroes.Add(unit);
        }

        foreach (var jobGroup in heroesByJob)
        {
            var tierLv = GetTierLevel(jobGroup.Value.Count);
            if (tierLv <= 0)
                continue;

            // 兵种技能按同职业英雄数 SetLevel 匹配对应档位的技能行：
            // 脚本类技能（枪·眩晕/戟·AOE溅射/炮·AOE范围/扇·负面buff延长/琴·正面buff延长）由此以当前档位生效；
            // 属性类占位技能(Dumb)无实际效果，加成仍走下方 LinkSelf/LinkTeam
            SetJobSkillLevel(jobGroup.Value, jobGroup.Key, tierLv);

            var cfg = GetTierConfig(jobGroup.Key, jobGroup.Value.Count);
            if (cfg == null)
                continue;

            var linkSelfBonuses = ParseBonuses(cfg.LinkSelf);
            var linkTeamBonuses = ParseBonuses(cfg.LinkTeam);

            // LinkSelf：该职业每个连接英雄自身获得加成；
            // 士兵类属性（soldierAtk/soldierHp）例外：按"全军士兵"施加给本侧全部士兵单位（总量不乘人数）
            foreach (var hero in jobGroup.Value)
                foreach (var bonus in linkSelfBonuses)
                    if (bonus.Attr != "soldierAtk" && bonus.Attr != "soldierHp")
                        ApplyAttr(hero, bonus.Attr, bonus.Value);

            foreach (var unit in allMySideUnits)
            {
                if (unit.isHero)
                    continue;
                foreach (var bonus in linkSelfBonuses)
                    if (bonus.Attr == "soldierAtk" || bonus.Attr == "soldierHp")
                        ApplyAttr(unit, bonus.Attr, bonus.Value);
            }

            // LinkTeam：该档位除该职业英雄外的我方全体英雄获得的总量（该职业英雄已由 LinkSelf 覆盖，不重复给）
            foreach (var unit in allMySideUnits)
            {
                if (!unit.isHero || jobGroup.Value.Contains(unit))
                    continue;
                foreach (var bonus in linkTeamBonuses)
                    ApplyAttr(unit, bonus.Attr, bonus.Value);
            }
        }
    }

    // 结算光环技能效果（AuroAttrs）：遍历本侧每个英雄携带的光环技能，各自对本侧全体英雄生效（含提供者）。
    // 效果值按光环来源英雄自身的 auroEffectRate 修正（鼓·战鼓 LinkSelf 提升该值），
    // 因此在 ApplyJobLinks 之后调用，保证先完成 LinkSelf 属性加成再取值。
    private static void ApplyAuroAttrs(int side)
    {
        var allMySideUnits = WorldManager.Instance.GetUnitsMySide(side);
        if (allMySideUnits.Count == 0)
            return;

        foreach (var provider in allMySideUnits)
        {
            if (!provider.isHero || provider.hp <= 0)
                continue;
            foreach (var skill in provider.skills)
            {
                var cfg = skill != null ? skill.skillCfg : null;
                if (cfg == null || string.IsNullOrEmpty(cfg.AuroAttrs))
                    continue;
                var auraBonuses = ParseBonuses(cfg.AuroAttrs);
                if (auraBonuses.Count == 0)
                    continue;

                foreach (var unit in allMySideUnits)
                {
                    if (!unit.isHero)
                        continue;
                    foreach (var bonus in auraBonuses)
                        ApplyAttr(unit, bonus.Attr, bonus.Value * provider.auroEffectRate);
                }
            }
        }
    }

    // 将同职业英雄的兵种技能 SetLevel 到当前档位（机械类技能依赖技能行参数，如 枪·眩晕几率/戟·AOE溅射）
    private static void SetJobSkillLevel(List<Chess> heroes, string job, int lv)
    {
        var jobCfg = ConfigManager.GetJobConfig(job);
        var sname = jobCfg != null ? jobCfg.SkillId : null;
        if (string.IsNullOrEmpty(sname))
            return;
        foreach (var hero in heroes)
        {
            foreach (var skill in hero.skills)
            {
                if (skill.skillCfg.Sname == sname)
                {
                    skill.SetLevel(lv);
                    break;
                }
            }
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
        return ConfigManager.GetSkillConfig(sname, lv);
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
        var cfg = ConfigManager.GetSkillConfig(sname, lv);
        if (cfg == null)
            return;

        sb.Append('\n');
        sb.Append(isCurrent ? "<color=green>" : "<color=#808080>");
        sb.Append('(').Append(linkTiers[lv - 1]).Append("人) ");
        // 脚本类技能（枪·眩晕/戟·AOE溅射/炮·AOE范围/扇·负面buff延长/琴·正面buff延长等）不走属性加成：
        // 未配置属性加成时直接展示技能描述（如 枪·眩晕 整行）；
        // 同时配置了属性加成（扇/琴的 LinkTeam 属性 + ModifyBuffTime 机制）时，属性文本与机制描述用 " | " 并显
        var isScriptSkill = !string.IsNullOrEmpty(cfg.ScriptName) && cfg.ScriptName != "Dumb";
        var parts = new List<string>();
        if (!string.IsNullOrEmpty(cfg.LinkSelf))
            parts.Add(AttrText(ParseBonuses(cfg.LinkSelf)));
        if (!string.IsNullOrEmpty(cfg.LinkTeam))
            parts.Add(AttrText(ParseBonuses(cfg.LinkTeam)));
        if (!string.IsNullOrEmpty(cfg.AuroAttrs))
            parts.Add("光环:" + AttrText(ParseBonuses(cfg.AuroAttrs)));
        if (parts.Count > 0)
        {
            sb.Append(string.Join(" | ", parts.ToArray()));
            if (isScriptSkill && !string.IsNullOrEmpty(cfg.Descript))
                sb.Append(" | ").Append(cfg.Descript);
        }
        else
            sb.Append(cfg.Descript);
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

    // 属性中文名：从 HeroAttrConfig 查询（name=JobLink属性键）；未登记的键告警并回退原始键名
    private static string AttrName(string attr)
    {
        try
        {
            return HeroAttrConfig.GetConfigByname(attr).Cname;
        }
        catch (KeyNotFoundException)
        {
            GameLog.Warn("JobLink 属性键未配置中文名 attr=" + attr);
            return attr;
        }
    }

    private static string FormatValue(string attr, float v)
    {
        // 百分比类属性：v为比例值（0.1=10%）
        if (attr == "critRate" || attr == "soldierAtk" || attr == "soldierHp"
            || attr == "dodgeRate" || attr == "critDamageMulti"
            || attr == "healRate" || attr == "healedRate"
            || attr == "auroEffectRate")
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
            case "dodgeRate":
                // 马·闪避
                unit.dodgeRate += value;
                break;
            case "critDamageMulti":
                unit.critDamageMulti += value;
                break;
            case "mpRegen":
                // 相/扇·法力回复
                unit.mpRegen += value;
                break;
            case "hpRegen":
                // 医·生命回复
                unit.hpRegen += (int)value;
                break;
            case "healRate":
                // 医·治疗强化
                unit.healRate += value;
                break;
            case "healedRate":
                // 受治疗系数（可为负=减疗）
                unit.healedRate += value;
                break;
            case "auroEffectRate":
                // 鼓·光环技能效果：修正 AuroAttrs 光环属性的效果值
                unit.auroEffectRate += value;
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
