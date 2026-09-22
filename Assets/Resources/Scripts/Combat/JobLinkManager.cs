using System.Collections.Generic;
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
    /// 档位差值文本已移除：职业/好友连接技能档位展示统一走 ConfigManager.GetSkillDescript(cfg, withNext:true)，
    /// 由 Lv1 模板 + 本级 DescriptVal 拼出当前档描述，每个参数位附下一档不同值（括号内），如"自身生命+10%(+20%)"。
    /// </summary>

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
            case "magicres":
                unit.magicRes += (int)value;
                break;
            case "hp":
            {
                var add = (int)value;
                unit.maxHp += add;
                unit.hp += add;
                if (unit.heroInfo != null)
                    unit.heroInfo.SetHpRate(unit.hp, unit.maxHp);
                break;
            }
            case "crit":
                unit.critRate += value;
                break;
            case "atkspeed":
                unit.attackSpeedRate += value;
                break;
            case "dodge":
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
                unit.hpRegen += value;
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
