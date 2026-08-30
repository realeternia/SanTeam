using System.Collections.Generic;
using System.Text;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 兵种连锁（金铲铲式职业羁绊）：战斗开始时统计本侧同职业英雄数量，
/// 直接施加被动属性加成，不走技能系统。
/// 每个职业定义两组加成：
/// - Self：该职业每个英雄自身获得的属性
/// - Team：该职业每个英雄给全队（含自身、士兵）提供的属性，多个同职业英雄时叠加
/// 例：戟（防御单位）——每个戟英雄自身+10护甲，且每个戟英雄使全队+3护甲
/// </summary>
public static class JobLinkManager
{
    private class AttrBonus
    {
        public string Attr;
        public float Value;

        public AttrBonus(string attr, float value)
        {
            Attr = attr;
            Value = value;
        }
    }

    private class JobBonus
    {
        public AttrBonus[] Self; // 该职业每个英雄自身的加成
        public AttrBonus[] Team; // 该职业每个英雄给全队提供的加成（叠加）

        public JobBonus(AttrBonus[] self, AttrBonus[] team)
        {
            Self = self;
            Team = team;
        }
    }

    // 属性名：atk攻击 / ap法强 / might武力 / armor护甲 / magicRes魔抗 / maxHp生命 / critRate暴击率 / attackRate攻速
    private static readonly Dictionary<string, JobBonus> jobBonuses = new Dictionary<string, JobBonus>
    {
        // 近战
        { "帅", new JobBonus(new[] { new AttrBonus("atk", 12) }, new[] { new AttrBonus("atk", 3) }) },
        { "枪", new JobBonus(new[] { new AttrBonus("atk", 10) }, new[] { new AttrBonus("atk", 2) }) },
        { "戟", new JobBonus(new[] { new AttrBonus("armor", 10) }, new[] { new AttrBonus("armor", 3) }) },
        { "士", new JobBonus(new[] { new AttrBonus("armor", 6), new AttrBonus("magicRes", 6) }, new[] { new AttrBonus("armor", 2), new AttrBonus("magicRes", 2) }) },
        { "车", new JobBonus(new[] { new AttrBonus("maxHp", 90) }, new[] { new AttrBonus("maxHp", 30) }) },
        { "马", new JobBonus(new[] { new AttrBonus("might", 10) }, new[] { new AttrBonus("might", 3) }) },
        { "刀", new JobBonus(new[] { new AttrBonus("critRate", 0.06f) }, new[] { new AttrBonus("atk", 2) }) },
        { "盾", new JobBonus(new[] { new AttrBonus("armor", 10), new AttrBonus("magicRes", 5) }, new[] { new AttrBonus("armor", 3), new AttrBonus("magicRes", 2) }) },
        // 远程物理
        { "弓", new JobBonus(new[] { new AttrBonus("attackRate", 0.06f) }, new[] { new AttrBonus("attackRate", 0.02f) }) },
        { "弩", new JobBonus(new[] { new AttrBonus("atk", 12) }, new[] { new AttrBonus("range", 5) }) },
        { "炮", new JobBonus(new[] { new AttrBonus("atk", 10) }, new[] { new AttrBonus("atk", 2) }) },
        // 法系
        { "谋", new JobBonus(new[] { new AttrBonus("ap", 10) }, new[] { new AttrBonus("ap", 2) }) },
        { "扇", new JobBonus(new[] { new AttrBonus("ap", 8) }, new[] { new AttrBonus("ap", 2) }) },
        { "相", new JobBonus(new[] { new AttrBonus("ap", 6) }, new[] { new AttrBonus("soldierAtk", 0.08f), new AttrBonus("soldierHp", 0.08f) }) },
        { "鼓", new JobBonus(new[] { new AttrBonus("atk", 4) }, new[] { new AttrBonus("atk", 4) }) },
        { "乐", new JobBonus(new[] { new AttrBonus("attackRate", 0.03f) }, new[] { new AttrBonus("attackRate", 0.03f) }) },
        { "医", new JobBonus(new[] { new AttrBonus("maxHp", 50) }, new[] { new AttrBonus("maxHp", 40) }) },
    };

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
            if (!jobBonuses.TryGetValue(kv.Key, out var bonus))
                continue;

            // Self：该职业每个英雄自身获得加成
            foreach (var hero in kv.Value)
                foreach (var b in bonus.Self)
                    ApplyAttr(hero, b.Attr, b.Value);

            // Team：该职业每个英雄给全队提供加成（叠加），全队含英雄与士兵
            foreach (var hero in kv.Value)
                foreach (var unit in units)
                    foreach (var b in bonus.Team)
                        ApplyAttr(unit, b.Attr, b.Value);

            // 战报提示（显示在该职业第一个英雄头顶）
            var first = kv.Value[0];
            WorldManager.Instance.AddBattleText(BuildJobText(kv.Key, kv.Value.Count, bonus), first.transform.position, new Vector2(0, 60), SysColor.BattleText.JobLink, 3);
        }
    }

    private static string BuildJobText(string job, int count, JobBonus bonus)
    {
        var sb = new StringBuilder();
        sb.Append(job).Append('×').Append(count).Append(" 自身").Append(AttrText(bonus.Self, 1));
        sb.Append(" 全队").Append(AttrText(bonus.Team, count));
        return sb.ToString();
    }

    private static string AttrText(AttrBonus[] list, int mult)
    {
        var sb = new StringBuilder();
        for (var i = 0; i < list.Length; i++)
        {
            if (i > 0)
                sb.Append("、");
            sb.Append(AttrName(list[i].Attr)).Append("+").Append(FormatValue(list[i].Attr, list[i].Value * mult));
        }
        return sb.ToString();
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
