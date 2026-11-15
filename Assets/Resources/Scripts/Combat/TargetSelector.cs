using System;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 通用目标筛选器：统一实现 SkillConfig.SelectOnly / SelectFirst 的选敌规则（单体/群体共用）。
///
/// SelectOnly —— 硬过滤，用 ";" 分隔，全部满足才可被选中（未配置 = 不过滤）：
///   ally / enemy    候选阵营（两者都配 = 取并集；都未配时用调用方给的默认阵营）
///   hero / soldier  仅英雄 / 仅士兵
///   notSelf         排除施法者
///   inFight         仅处于交战中的单位
///   notFullHp       排除满血单位
///   hpBelow:N       仅生命百分比低于 N 的单位
///   job:盾|士        仅指定职业（竖线分隔多个职业短名）
///   hasBuff:盾       仅携带指定短名 Buff 的单位
///
/// SelectFirst —— 优先级链，用 ";" 分隔，按顺序逐条比较，前一条分出胜负即返回，平手看下一条：
///   lowestHp / highestHp / highestArmor / lowestArmor / highestAp / highestAtk /
///   highestMagicRes / lowestMagicRes / highestMp / biggestHpGap /
///   nearest / farthest / rangedFirst / meleeFirst / heroFirst / nearestToEnemy / random
///   （nearest / farthest 以本次搜索的中心点为基准，而非施法者位置）
///   biggestHpGap = 生命缺口（maxHp-hp）最大优先；highestMp = 带蓝耗技能的法力总和最高优先；
///   nearestToEnemy = 该单位到其最近敌人的距离最近优先（衡量"站位最靠前"）
///
/// 目标数：limit&lt;=0 表示全选；limit&gt;0 时有 SelectFirst 按优先级降序取前 limit，未配 SelectFirst 则随机取 limit。
/// 未识别规则只记录一次警告并跳过。
/// </summary>
public static class TargetSelector
{
    private static readonly HashSet<string> warnedRules = new HashSet<string>();

    /// <summary>
    /// 按 SelectOnly/SelectFirst 选出目标。
    /// defaultAlly=true 表示 SelectOnly 未指定阵营时默认选友方，否则默认选敌方；
    /// exclude 为额外排除的单位（如"施法者必含"时先在选人阶段排除施法者）；
    /// extraFilter 为脚本私有的附加过滤（如 Buff 去重、跳过已命中单位），在截断目标数之前生效。
    /// </summary>
    public static List<Chess> Select(Chess owner, Vector3 center, float range, bool defaultAlly,
        string selectOnly, string selectFirst, int limit, Chess exclude = null, Func<Chess, bool> extraFilter = null)
    {
        var result = new List<Chess>();
        if (owner == null)
            return result;

        var only = ParseTokens(selectOnly, ";,，；");
        var hasAlly = only.Contains("ally");
        var hasEnemy = only.Contains("enemy");
        var ally = hasAlly || (!hasEnemy && defaultAlly);
        var enemy = hasEnemy || (!hasAlly && !defaultAlly);

        var pool = new List<Chess>();
        if (ally)
            AppendDistinct(pool, WorldManager.Instance.GetMySideInRange(center, range, owner.side));
        if (enemy)
            AppendDistinct(pool, WorldManager.Instance.GetEnemyInRange(center, range, owner.side));

        return Filter(owner, center, pool, selectOnly, selectFirst, limit, exclude, extraFilter);
    }

    /// <summary>
    /// 对给定候选列表按 SelectOnly/SelectFirst/limit 做过滤、排序、截断（用于几何命中、逐跳等非范围查询的选人）。
    /// 参数语义与 <see cref="Select"/> 完全一致。
    /// </summary>
    public static List<Chess> Filter(Chess owner, Vector3 center, List<Chess> candidates,
        string selectOnly, string selectFirst, int limit, Chess exclude = null, Func<Chess, bool> extraFilter = null)
    {
        var result = new List<Chess>();
        if (owner == null || candidates == null)
            return result;

        var only = ParseTokens(selectOnly, ";,，；");
        foreach (var unit in candidates)
        {
            if (unit == null || result.Contains(unit))
                continue;
            if (unit == exclude)
                continue;
            if (extraFilter != null && !extraFilter(unit))
                continue;
            if (PassAll(owner, unit, only))
                result.Add(unit);
        }

        var first = ParseTokens(selectFirst, ";,，；");
        var randomOnly = first.Count == 0 || (first.Count == 1 && first[0] == "random");

        if (limit > 0)
        {
            // 无 SelectFirst（或仅 random）= 随机取 limit 名；有 SelectFirst = 按优先级降序取前 limit 名。
            // 候选数不超过上限时同样排序，保证"最合适的目标"排在前面（单体技能取首个）。
            if (randomOnly)
                Shuffle(result);
            else
                result.Sort((a, b) => Compare(center, a, b, first));

            if (result.Count > limit)
                result.RemoveRange(limit, result.Count - limit);
        }

        return result;
    }

    // ---------------- 过滤 ----------------

    private static bool PassAll(Chess owner, Chess unit, List<string> only)
    {
        foreach (var token in only)
        {
            switch (token)
            {
                case "ally":
                case "enemy":
                    break;
                case "hero":
                    if (!unit.isHero) return false;
                    break;
                case "soldier":
                    if (unit.isHero) return false;
                    break;
                case "notSelf":
                    if (unit == owner) return false;
                    break;
                case "inFight":
                    if (!unit.IsInFight()) return false;
                    break;
                case "notFullHp":
                    if (unit.hp >= unit.maxHp) return false;
                    break;
                default:
                    if (token.StartsWith("hpBelow:", StringComparison.Ordinal))
                    {
                        if (!MatchHpBelow(unit, token)) return false;
                    }
                    else if (token.StartsWith("job:", StringComparison.Ordinal))
                    {
                        if (!MatchJob(unit, token)) return false;
                    }
                    else if (token.StartsWith("hasBuff:", StringComparison.Ordinal))
                    {
                        if (!MatchBuff(unit, token)) return false;
                    }
                    else
                    {
                        WarnOnce("SelectOnly", token);
                    }
                    break;
            }
        }
        return true;
    }

    private static bool MatchHpBelow(Chess unit, string token)
    {
        float pct;
        if (!float.TryParse(token.Substring("hpBelow:".Length), out pct))
        {
            WarnOnce("SelectOnly", token);
            return true;
        }
        return unit.HpRate * 100f < pct;
    }

    private static bool MatchJob(Chess unit, string token)
    {
        if (unit == null || !unit.isHero || unit.heroId <= 0)
            return false;
        var heroCfg = HeroConfig.GetConfig(unit.heroId);
        if (heroCfg == null || string.IsNullOrEmpty(heroCfg.Job))
            return false;
        foreach (var job in token.Substring("job:".Length).Split('|'))
        {
            if (job.Trim() == heroCfg.Job)
                return true;
        }
        return false;
    }

    private static bool MatchBuff(Chess unit, string token)
    {
        var name = token.Substring("hasBuff:".Length).Trim();
        var cfg = BuffConfig.GetConfigByNameS(name);
        if (cfg == null)
        {
            WarnOnce("SelectOnly", token);
            return true;
        }
        return unit.HasBuff(cfg.Id);
    }

    // ---------------- 排序 ----------------

    private static int Compare(Vector3 center, Chess a, Chess b, List<string> rules)
    {
        foreach (var rule in rules)
        {
            var r = CompareOne(center, a, b, rule);
            if (r != 0)
                return r;
        }
        return 0;
    }

    private static int CompareOne(Vector3 center, Chess a, Chess b, string rule)
    {
        switch (rule)
        {
            case "lowestHp": return a.HpRate.CompareTo(b.HpRate);
            case "highestHp": return b.HpRate.CompareTo(a.HpRate);
            case "highestArmor": return b.armor.CompareTo(a.armor);
            case "lowestArmor": return a.armor.CompareTo(b.armor);
            case "highestAp": return b.ap.CompareTo(a.ap);
            case "highestAtk": return b.atk.CompareTo(a.atk);
            case "highestMagicRes": return b.magicRes.CompareTo(a.magicRes);
            case "lowestMagicRes": return a.magicRes.CompareTo(b.magicRes);
            case "highestMp": return TotalMp(b).CompareTo(TotalMp(a));
            case "biggestHpGap": return (b.maxHp - b.hp).CompareTo(a.maxHp - a.hp);
            case "nearest": return Distance(center, a).CompareTo(Distance(center, b));
            case "farthest": return Distance(center, b).CompareTo(Distance(center, a));
            case "rangedFirst": return RangeRank(a).CompareTo(RangeRank(b));
            case "meleeFirst": return RangeRank(b).CompareTo(RangeRank(a));
            case "heroFirst": return (a.isHero ? 0 : 1).CompareTo(b.isHero ? 0 : 1);
            case "nearestToEnemy": return DistanceToNearestEnemy(a).CompareTo(DistanceToNearestEnemy(b));
            case "random":
                return 0;
            default:
                WarnOnce("SelectFirst", rule);
                return 0;
        }
    }

    private static float Distance(Vector3 center, Chess target)
    {
        return Vector3.Distance(center, target.transform.position);
    }

    /// <summary>远程=0、近战=1（攻击距离 ≥ CombatConst.MeleeRange 视为远程）</summary>
    private static int RangeRank(Chess unit)
    {
        return unit.attackRange >= CombatConst.MeleeRange ? 0 : 1;
    }

    /// <summary>单位所有带蓝耗（MpCost&gt;0）技能的当前法力总和</summary>
    private static int TotalMp(Chess unit)
    {
        var total = 0;
        if (unit.skills == null)
            return total;
        foreach (var skill in unit.skills)
        {
            if (skill != null && skill.skillCfg != null && skill.skillCfg.MpCost > 0)
                total += (int)skill.mp;
        }
        return total;
    }

    /// <summary>单位到其最近敌人的距离（无敌人时返回极大值，排序时排最后）</summary>
    private static float DistanceToNearestEnemy(Chess unit)
    {
        var best = float.MaxValue;
        var enemies = WorldManager.Instance.GetEnemyInRange(unit.transform.position, 0f, unit.side);
        if (enemies == null)
            return best;
        foreach (var enemy in enemies)
        {
            if (enemy == null)
                continue;
            var d = Vector3.Distance(unit.transform.position, enemy.transform.position);
            if (d < best)
                best = d;
        }
        return best;
    }

    // ---------------- 工具 ----------------

    private static List<string> ParseTokens(string raw, string separators)
    {
        var list = new List<string>();
        if (string.IsNullOrEmpty(raw))
            return list;
        foreach (var seg in raw.Split(separators.ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
        {
            var token = seg.Trim();
            if (token.Length > 0)
                list.Add(token);
        }
        return list;
    }

    private static void AppendDistinct(List<Chess> target, List<Chess> source)
    {
        if (source == null)
            return;
        foreach (var unit in source)
        {
            if (unit != null && !target.Contains(unit))
                target.Add(unit);
        }
    }

    private static void Shuffle(List<Chess> list)
    {
        for (var i = list.Count - 1; i > 0; i--)
        {
            var j = SysRandom.Range(0, i + 1);
            var tmp = list[i];
            list[i] = list[j];
            list[j] = tmp;
        }
    }

    private static void WarnOnce(string column, string token)
    {
        var key = column + ":" + token;
        if (warnedRules.Contains(key))
            return;
        warnedRules.Add(key);
        GameLog.Warn($"TargetSelector: 未识别的{column}规则[{token}]，已跳过");
    }
}
