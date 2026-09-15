using System.Collections.Generic;
using System.Text;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 兵种连锁（金铲铲式职业羁绊）：战斗开始时统计本侧同职业英雄数量，
/// 按 SkillConfig 职业技能行（Sname=JobConfig.SkillId）触发羁绊效果。档位：上阵 1/2/3/4/5 人对应职业技能 Lv1~5 行。
/// 效果分两类：
/// 1. 属性加成（不走技能系统）：按 LinkSelf/LinkTeam/AuroAttrs 施加被动属性
///    - LinkSelf：连接英雄（该职业每个英雄自身）获得的属性；其中 soldierAtk/soldierHp 例外，按乘法系数施加给本侧全部士兵
///      （多职业组系数先累加、每侧结算前复位一次，最后统一按初始基准乘一次，避免多次施加累乘）
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

    internal struct AttrBonus
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
                if (unit.isHero && !jobGroup.Value.Contains(unit))
                {
                    foreach (var bonus in linkTeamBonuses)
                        ApplyAttr(unit, bonus.Attr, bonus.Value);
                }
                if (!unit.isHero)
                {
                    foreach (var bonus in linkSelfBonuses)
                        if (bonus.Attr == "soldierAtk" || bonus.Attr == "soldierHp")
                            ApplyAttr(unit, bonus.Attr, bonus.Value);
                }
            }
        }

        // 统一结算士兵加成：目标最大生命 = 初始基准快照 × 累计系数（只乘一次），
        // 多个职业组系数先累加，不会把已加成数值当基数二次乘算；
        // 士兵攻击系数同样在此一并发结算：atk = 当前atk × soldierAtkRate（只乘一次），
        // 结算后复位系数并折算进 atk，伤害计算时攻击基准统一为 atk，不再乘算
        foreach (var unit in allMySideUnits)
        {
            if (unit.isHero)
                continue;
            var targetMaxHp = (int)(unit.soldierBaseMaxHp * unit.soldierHpRate);
            unit.hp = targetMaxHp;
            unit.maxHp = targetMaxHp;
            unit.atk = (int)(unit.atk * unit.soldierAtkRate);
            unit.soldierAtkRate = 1f;
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
    /// 生成职业羁绊的 tooltip 文本（单行）：当前档数值 + 下一档不同数值（括号内），
    /// 格式"自身暴击+17%( +25% )，全队暴击+5% (+7%)"，只显示与下一档不同的属性。
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
        return GetTierDiffTipText(sname, activeLv);
    }

    /// <summary>
    /// 档位差值文本（职业/好友连接技能共用）：当前档数值 + 下一档不同的数值（括号内），
    /// 格式"自身暴击+17%( +25% )，全队暴击+5% (+7%)"。下一档等级超出配置时只显示当前档。
    /// </summary>
    public static string GetTierDiffTipText(string sname, int activeLv)
    {
        if (activeLv < 1)
            activeLv = 1;
        if (activeLv > linkTiers.Length)
            activeLv = linkTiers.Length;

        var curCfg = ConfigManager.GetSkillConfig(sname, activeLv);
        if (curCfg == null)
            return "";

        var nextCfg = activeLv < linkTiers.Length ? ConfigManager.GetSkillConfig(sname, activeLv + 1) : null;
        var nextSelf = nextCfg != null ? ParseBonuses(nextCfg.LinkSelf) : null;
        var nextTeam = nextCfg != null ? ParseBonuses(nextCfg.LinkTeam) : null;
        var nextAuro = nextCfg != null ? ParseBonuses(nextCfg.AuroAttrs) : null;

        var parts = new List<string>();
        if (!string.IsNullOrEmpty(curCfg.LinkSelf))
            parts.Add(AttrDiffText("自身", ParseBonuses(curCfg.LinkSelf), nextSelf));
        if (!string.IsNullOrEmpty(curCfg.LinkTeam))
            parts.Add(AttrDiffText("全队", ParseBonuses(curCfg.LinkTeam), nextTeam));
        if (!string.IsNullOrEmpty(curCfg.AuroAttrs))
            parts.Add(AttrDiffText("光环:", ParseBonuses(curCfg.AuroAttrs), nextAuro));

        if (parts.Count > 0)
        {
            // 脚本类技能（枪·眩晕/戟·AOE溅射/扇·buff延长等）同时配置属性加成与机制描述时用 " | " 并显
            var isScriptSkill = !string.IsNullOrEmpty(curCfg.ScriptName) && curCfg.ScriptName != "Dumb";
            if (isScriptSkill && !string.IsNullOrEmpty(curCfg.Descript))
                parts.Add(curCfg.Descript);
            return string.Join("，", parts.ToArray());
        }
        return curCfg.Descript;
    }

    // 生成一段属性差值文本：当前值 + 下一档不同值（括号内），如 "自身暴击+17%( +25% )"；
    // 下一档新增（当前档没有）的属性以"( +值 )"追加
    private static string AttrDiffText(string prefix, List<AttrBonus> curList, List<AttrBonus> nextList)
    {
        if (curList == null || curList.Count == 0)
            return "";
        var sb = new StringBuilder(prefix);
        for (var i = 0; i < curList.Count; i++)
        {
            if (i > 0)
                sb.Append("、");
            var cur = curList[i];
            sb.Append(AttrName(cur.Attr)).Append("+").Append(FormatValue(cur.Attr, cur.Value));
            if (TryGetBonus(nextList, cur.Attr, out var next) && next.Value != cur.Value)
                sb.Append("( +").Append(FormatValue(cur.Attr, next.Value)).Append(" )");
        }
        if (nextList != null)
        {
            foreach (var next in nextList)
            {
                if (TryGetBonus(curList, next.Attr, out _))
                    continue;
                if (sb.Length > 0)
                    sb.Append("、");
                sb.Append(AttrName(next.Attr)).Append("( +").Append(FormatValue(next.Attr, next.Value)).Append(" )");
            }
        }
        return sb.ToString();
    }

    // 在加成列表里按属性名查找（AttrBonus 是结构体，不能与 null 比较，用返回值表示是否存在）
    private static bool TryGetBonus(List<AttrBonus> list, string attr, out AttrBonus bonus)
    {
        bonus = default(AttrBonus);
        if (list == null)
            return false;
        for (var i = 0; i < list.Count; i++)
        {
            if (list[i].Attr == attr)
            {
                bonus = list[i];
                return true;
            }
        }
        return false;
    }

    // 解析 "attr+value,attr+value" 格式的加成串（职业技能 LinkSelf/LinkTeam 与开局属性技能共用）
    internal static List<AttrBonus> ParseBonuses(string str)
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

    // 属性施加（职业技能 LinkSelf/LinkTeam 与开局属性技能共用）
    internal static void ApplyAttr(Chess unit, string attr, float value)
    {
        switch (attr)
        {
            case "atk":
            case "ap":
            case "might": // 无双已并入攻击：Chess.AddAttr 内部按 atk 兼容处理
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
            case "attackSpeedRate":
                unit.attackSpeedRate += value;
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
                // 相的羁绊：全军士兵攻击+%（乘法系数，此处只累加，ApplyJobLinks 末尾统一折算进 atk）
                if (!unit.isHero)
                    unit.soldierAtkRate += value;
                break;
            case "soldierHp":
            {
                // 相的羁绊：全军士兵生命+%（乘法系数，此处只累加，ApplyJobLinks 末尾统一按初始基准结算一次）
                if (!unit.isHero)
                    unit.soldierHpRate += value;
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
