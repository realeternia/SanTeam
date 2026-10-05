// ============================================================
// 战斗模拟器 · 批量模拟 —— BatchSimRunner
// 无界面连打 N 轮：每轮随机抽双方阵容（近战前排/远程后排站位），可选给每个武将随机装备一件 400 段道具，
// 可开关 国家/好友/职业 加成，跑完后统计各武将、各物品的胜率并输出报告文件。
// ============================================================
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CommonConfig;

public static class BatchSimRunner
{
    // 单场参数（与 CLI 一致）
    private const int MaxSteps = 12000;      // 12000 × 0.05 = 600s 上限
    private const float StepDt = 0.05f;

    // 随机装备池：400 段成品装备（Id 400xxx）
    private const int ItemIdMin = 400000;
    private const int ItemIdMax = 401000;

    public class Options
    {
        public int HeroCount = 6;       // 每侧武将数量（近战/远程各半，奇数补 1 个随机）
        public int SoldierCount = 4;    // 每侧小兵数量（全部近战士兵，占最前排）
        public int SoldierLevel = 6;    // 小兵等级（1~30，决定小兵攻防加成）
        public int Quality1Level = 5;   // 设定等级：品质1~2 用该等级，品质3~4 用该等级-1
        public bool EnableFaction;      // 国家加成
        public bool EnableFriend;       // 好友加成
        public bool EnableJob;          // 职业加成
        public int Rounds = 100;        // 运行轮数
        public bool RandomEquip;        // 是否给每个武将随机一件 400 段道具
    }

    // 单个统计对象（武将或物品）的胜率
    private class StatRow
    {
        public int Id;
        public string Name;
        public string Job;              // 职业（仅武将行，物品行为"-"）
        public string Type;             // 组类型（仅成组行：国家/好友/职业）
        public string SkillName;        // 组联动技能名（仅好友成组行）
        public int Quality;             // 卡片品质（仅武将行，物品行为0）
        public int Appear, Win, Loss, Draw;
        public float HeroDamageTotal;   // 对敌方英雄造成的累计伤害（用于场均）
        public float MagicDamageTotal;  // 造成的累计法术伤害（DamageType=法术，任意目标）
        public float Rate { get { return Appear > 0 ? (float)Win / Appear : 0f; } }
        // 场均对英雄伤害 = 累计伤害 / 出场场次
        public float AvgHeroDamage { get { return Appear > 0 ? HeroDamageTotal / Appear : 0f; } }
        // 场均法术伤害 = 累计法术伤害 / 出场场次
        public float AvgMagicDamage { get { return Appear > 0 ? MagicDamageTotal / Appear : 0f; } }
    }

    // 一件随机装备的绑定记录（harness 桩仅提供二元 ValueTuple，故用结构体）
    private struct EquipRecord
    {
        public int heroId;
        public int itemId;
        public int side;
    }

    // 本场累积（由 Chess.OnDamageDealt 驱动，攻击方必须是英雄）：
    // _heroDamageToHero：对敌方英雄造成的伤害（英雄打英雄）
    // _magicDamage：造成的法术伤害（SkillConfig.DamageType=法术，任意目标）
    private static Dictionary<int, float> _heroDamageToHero;
    private static Dictionary<int, float> _magicDamage;

    static BatchSimRunner()
    {
        Chess.OnDamageDealt += (a, v, d, sk) =>
        {
            if (_heroDamageToHero == null || a == null || v == null || d <= 0)
                return;
            if (!a.isHero)
                return;
            if (v.isHero)
            {
                float cur;
                _heroDamageToHero.TryGetValue(a.heroId, out cur);
                _heroDamageToHero[a.heroId] = cur + d;
            }
            if (sk > 0)
            {
                var cfg = SkillConfig.GetConfig(sk);
                if (cfg != null && cfg.DamageType == CombatConst.DamageTypeMagic)
                {
                    float cur;
                    _magicDamage.TryGetValue(a.heroId, out cur);
                    _magicDamage[a.heroId] = cur + d;
                }
            }
        };
    }

    // 把本场伤害累加到各武将行（对英雄伤害 / 法术伤害）
    private static void AddRoundDamage(Dictionary<int, StatRow> stats, List<(int id, int lv)> team,
        Dictionary<int, float> toHero, Dictionary<int, float> magic)
    {
        if (toHero == null)
            return;
        foreach (var h in team)
        {
            StatRow row;
            if (!stats.TryGetValue(h.id, out row))
                continue;
            float v;
            if (toHero.TryGetValue(h.id, out v))
                row.HeroDamageTotal += v;
            if (magic != null && magic.TryGetValue(h.id, out v))
                row.MagicDamageTotal += v;
        }
    }

    // 执行批量模拟，返回报告文件路径；onProgress(已完成轮数, 总轮数)
    public static string Run(Options opt, Action<int, int> onProgress)
    {
        var heroIds = HeroLineup.AllHeroIds();
        var itemIds = ItemConfig.ConfigList
            .Where(c => c.Id >= ItemIdMin && c.Id < ItemIdMax)
            .Select(c => (int)c.Id)
            .OrderBy(x => x)
            .ToList();

        var heroStats = new Dictionary<int, StatRow>();
        var itemStats = new Dictionary<int, StatRow>();
        var groupStats = new Dictionary<string, StatRow>();   // key = 类型|组名
        int winA = 0, winB = 0, draw = 0;

        int heroCount = Math.Max(1, opt.HeroCount);
        int rounds = Math.Max(1, opt.Rounds);

        if (heroIds.Count == 0)
        {
            GameLog.Error("BatchSimRunner: 英雄池为空，无法模拟");
            return null;
        }
        if (opt.RandomEquip && itemIds.Count == 0)
            GameLog.Warn("BatchSimRunner: 400 段道具池为空，随机装备将被跳过");

        for (int round = 0; round < rounds; round++)
        {
            int seed = 1 + round;
            SysRandom.Seed(seed);

            // 开启的羁绊类型 → 按该类型成组抽阵容；未开启任何类型时退化为普通远近随机
            var enabledTypes = EnabledGroupTypes(opt);

            // 随机抽双方阵容：每侧先从已开启类型里随机挑 1 种、再随机挑 1 个组，取一半人成组，
            // 其余按近战/远程各半补齐；同一英雄不会同场出现在双方
            var used = new HashSet<int>();
            HeroLineup.GroupDef groupA, groupB;
            var teamA = HeroLineup.PickGroupLineup(heroCount, enabledTypes, used, out groupA)
                .Select(id => (id, LevelForQuality(id, opt.Quality1Level))).ToList();
            var teamB = HeroLineup.PickGroupLineup(heroCount, enabledTypes, used, out groupB)
                .Select(id => (id, LevelForQuality(id, opt.Quality1Level))).ToList();

            var battle = new BattleSim
            {
                SoldierLevel = Math.Max(1, opt.SoldierLevel),
                EnableFactionShield = opt.EnableFaction,
                EnableFriendLine = opt.EnableFriend,
                EnableJobLink = opt.EnableJob,
            };
            battle.SetupByRole(teamA, teamB, Math.Max(0, opt.SoldierCount));

            // 随机装备：每个武将一件随机 400 段道具（pid 0=甲 1=乙）
            var equips = new List<EquipRecord>();
            if (opt.RandomEquip && itemIds.Count > 0)
            {
                foreach (var hero in teamA)
                {
                    int itemId = itemIds[SysRandom.Range(0, itemIds.Count)];
                    battle.EquipItem(0, hero.id, itemId);
                    equips.Add(new EquipRecord { heroId = hero.id, itemId = itemId, side = 1 });
                }
                foreach (var hero in teamB)
                {
                    int itemId = itemIds[SysRandom.Range(0, itemIds.Count)];
                    battle.EquipItem(1, hero.id, itemId);
                    equips.Add(new EquipRecord { heroId = hero.id, itemId = itemId, side = 2 });
                }
            }

            _heroDamageToHero = new Dictionary<int, float>();
            _magicDamage = new Dictionary<int, float>();
            battle.Start(seed);
            int steps = 0;
            while (!battle.IsFinished && steps < MaxSteps)
            {
                battle.Step(StepDt);
                steps++;
            }
            var roundHeroDamage = _heroDamageToHero;
            var roundMagicDamage = _magicDamage;
            _heroDamageToHero = null;   // 本场结束，停止累积
            _magicDamage = null;

            int winner = !battle.IsFinished ? 0 : (battle.HasWin ? 1 : 2);   // 0=平局
            if (winner == 1) winA++;
            else if (winner == 2) winB++;
            else draw++;

            RecordTeam(teamA, 1, winner, heroStats);
            RecordTeam(teamB, 2, winner, heroStats);
            RecordGroup(groupStats, groupA, 1, winner);
            RecordGroup(groupStats, groupB, 2, winner);
            AddRoundDamage(heroStats, teamA, roundHeroDamage, roundMagicDamage);
            AddRoundDamage(heroStats, teamB, roundHeroDamage, roundMagicDamage);
            foreach (var e in equips)
                RecordOne(itemStats, e.itemId, ItemName(e.itemId), "-", 0, e.side, winner);

            if (onProgress != null)
                onProgress(round + 1, rounds);
        }

        var heroRows = Sort(heroStats.Values);
        var itemRows = Sort(itemStats.Values);
        var jobRows = AggregateByJob(heroRows);
        string report = BuildReport(opt, rounds, winA, winB, draw, heroRows, itemRows, jobRows, groupStats);

        string dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "sim_reports");
        Directory.CreateDirectory(dir);
        string path = Path.Combine(dir, "sim_report_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".txt");
        File.WriteAllText(path, report, new UTF8Encoding(false));
        GameLog.Info("批量模拟完成：甲胜 " + winA + " / 乙胜 " + winB + " / 平局 " + draw + "，报告已输出 " + path);
        return path;
    }

    // 记录一队武将的胜负
    private static void RecordTeam(List<(int id, int lv)> team, int side, int winner,
        Dictionary<int, StatRow> stats)
    {
        foreach (var h in team)
            RecordOne(stats, h.id, HeroName(h.id), HeroJob(h.id), HeroQuality(h.id), side, winner);
    }

    private static void RecordOne(Dictionary<int, StatRow> stats, int id, string name, string job, int quality, int side, int winner)
    {
        StatRow row;
        if (!stats.TryGetValue(id, out row))
        {
            row = new StatRow { Id = id, Name = name, Job = job, Quality = quality };
            stats[id] = row;
        }
        row.Appear++;
        if (winner == 0) row.Draw++;
        else if (winner == side) row.Win++;
        else row.Loss++;
    }

    // 已开启的羁绊类型（用于成组抽阵容）：未开启任何类型时返回空表 → 退化为普通远近随机
    private static List<string> EnabledGroupTypes(Options opt)
    {
        var types = new List<string>();
        if (opt.EnableFaction) types.Add(HeroLineup.GroupTypeFaction);
        if (opt.EnableFriend) types.Add(HeroLineup.GroupTypeFriend);
        if (opt.EnableJob) types.Add(HeroLineup.GroupTypeJob);
        return types;
    }

    // 记录一队"本场选中的组"的胜负（成组口径：出场=该组被选中的场次）
    private static void RecordGroup(Dictionary<string, StatRow> stats, HeroLineup.GroupDef group, int side, int winner)
    {
        if (group == null)
            return;
        string key = group.Type + "|" + group.Name;
        StatRow row;
        if (!stats.TryGetValue(key, out row))
        {
            row = new StatRow { Id = 0, Name = group.Name, Job = group.Type, Type = group.Type, SkillName = group.SkillName, Quality = 0 };
            stats[key] = row;
        }
        row.Appear++;
        if (winner == 0) row.Draw++;
        else if (winner == side) row.Win++;
        else row.Loss++;
    }

    private static List<StatRow> Sort(IEnumerable<StatRow> stats)
    {
        return stats
            .OrderByDescending(r => r.Rate)
            .ThenByDescending(r => r.Appear)
            .ThenBy(r => r.Id)
            .ToList();
    }

    // 按职业聚合：把各武将行的出场/胜负累加到所属职业（合并出场场次后的加权胜率）
    private static List<StatRow> AggregateByJob(List<StatRow> heroRows)
    {
        var map = new Dictionary<string, StatRow>();
        foreach (var h in heroRows)
        {
            string job = string.IsNullOrEmpty(h.Job) ? "-" : h.Job;
            StatRow r;
            if (!map.TryGetValue(job, out r))
            {
                r = new StatRow { Id = 0, Name = job, Job = job, Quality = 0 };
                map[job] = r;
            }
            r.Appear += h.Appear;
            r.Win += h.Win;
            r.Loss += h.Loss;
            r.Draw += h.Draw;
        }
        return Sort(map.Values);
    }

    private static string BuildReport(Options opt, int rounds, int winA, int winB, int draw,
        List<StatRow> heroRows, List<StatRow> itemRows, List<StatRow> jobRows,
        Dictionary<string, StatRow> groupStats)
    {
        var sb = new StringBuilder();
        sb.AppendLine("==================================================");
        sb.AppendLine("战斗模拟器 · 批量模拟报告");
        sb.AppendLine("生成时间: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        sb.AppendLine("每侧武将数量: " + opt.HeroCount + "    每侧小兵数量: " + opt.SoldierCount
            + "（近战）    小兵等级: " + opt.SoldierLevel);
        sb.AppendLine("设定等级: " + opt.Quality1Level
            + "（品质1~2=" + opt.Quality1Level + "级, 品质3~4=" + Math.Max(1, opt.Quality1Level - 1) + "级）");
        sb.AppendLine("国家加成: " + OnOff(opt.EnableFaction)
            + "    好友加成: " + OnOff(opt.EnableFriend)
            + "    职业加成: " + OnOff(opt.EnableJob));
        sb.AppendLine("随机装备(400 段道具): " + OnOff(opt.RandomEquip));
        sb.AppendLine("运行轮数: " + rounds + "    单场步数上限: " + MaxSteps + "(" + (MaxSteps * StepDt) + "s)");
        sb.AppendLine("--------------------------------------------------");
        sb.AppendLine("总胜负: 甲胜 " + winA + " / 乙胜 " + winB + " / 平局 " + draw);
        sb.AppendLine();

        sb.AppendLine("【武将胜率】按胜率降序（胜率 = 胜场 / 出场场次；场均伤害 = 对敌方英雄造成的伤害 / 出场场次；场均法术伤害 = 造成的法术伤害(任意目标) / 出场场次）");
        sb.AppendLine(HeroHeader());
        AppendHeroRows(sb, heroRows, opt.Quality1Level);
        sb.AppendLine();

        sb.AppendLine("【职业胜率】按胜率降序（该职业全部英雄的出场/胜负合并统计）");
        sb.AppendLine(JobHeader());
        AppendJobRows(sb, jobRows);
        sb.AppendLine();

        sb.AppendLine("【成组胜率】按胜率降序（成组 = 每侧本场选中的那个组；出场 = 该组被选中的场次数，胜率 = 选中该组的队伍胜场/出场）");
        AppendGroupSections(sb, groupStats);
        sb.AppendLine();

        sb.AppendLine("【物品胜率】按胜率降序（仅随机装备开启时有数据）");
        sb.AppendLine(Header());
        AppendRows(sb, itemRows);
        sb.AppendLine();
        sb.AppendLine("==================================================");
        return sb.ToString();
    }

    private static string Header()
    {
        return string.Format("{0,4}  {1,8}  {2,-10}  {3,-6}  {4,4}  {5,6}  {6,6}  {7,6}  {8,6}  {9,8}",
            "排名", "ID", "名字", "职业", "品质", "出场", "胜", "负", "平", "胜率");
    }

    private static void AppendRows(StringBuilder sb, List<StatRow> rows)
    {
        if (rows.Count == 0)
        {
            sb.AppendLine("(无数据)");
            return;
        }
        int rank = 1;
        foreach (var r in rows)
        {
            sb.AppendLine(string.Format("{0,4}  {1,8}  {2,-10}  {3,-6}  {4,4}  {5,6}  {6,6}  {7,6}  {8,6}  {9,8:P1}",
                rank++, r.Id, r.Name, string.IsNullOrEmpty(r.Job) ? "-" : r.Job,
                r.Quality > 0 ? r.Quality.ToString() : "-", r.Appear, r.Win, r.Loss, r.Draw, r.Rate));
        }
    }

    private static string HeroHeader()
    {
        return string.Format("{0,4}  {1,8}  {2,-10}  {3,-6}  {4,4}  {5,4}  {6,6}  {7,6}  {8,6}  {9,6}  {10,8}  {11,10}  {12,12}",
            "排名", "ID", "名字", "职业", "品质", "等级", "出场", "胜", "负", "平", "胜率", "场均伤害", "场均法术伤害");
    }

    private static void AppendHeroRows(StringBuilder sb, List<StatRow> rows, int quality1Level)
    {
        if (rows.Count == 0)
        {
            sb.AppendLine("(无数据)");
            return;
        }
        int rank = 1;
        foreach (var r in rows)
        {
            sb.AppendLine(string.Format("{0,4}  {1,8}  {2,-10}  {3,-6}  {4,4}  {5,4}  {6,6}  {7,6}  {8,6}  {9,6}  {10,8:P1}  {11,10:N0}  {12,12:N0}",
                rank++, r.Id, r.Name, string.IsNullOrEmpty(r.Job) ? "-" : r.Job,
                r.Quality > 0 ? r.Quality.ToString() : "-",
                LevelForQuality(r.Id, quality1Level),
                r.Appear, r.Win, r.Loss, r.Draw, r.Rate, r.AvgHeroDamage, r.AvgMagicDamage));
        }
    }

    private static string JobHeader()
    {
        return string.Format("{0,-10}  {1,8}  {2,8}  {3,8}  {4,8}  {5,8}",
            "职业", "出场", "胜", "负", "平", "胜率");
    }

    private static void AppendJobRows(StringBuilder sb, List<StatRow> rows)
    {
        if (rows.Count == 0)
        {
            sb.AppendLine("(无数据)");
            return;
        }
        foreach (var r in rows)
        {
            sb.AppendLine(string.Format("{0,-10}  {1,8}  {2,8}  {3,8}  {4,8}  {5,8:P1}",
                r.Name, r.Appear, r.Win, r.Loss, r.Draw, r.Rate));
        }
    }

    // 成组胜率：按类型（国家/好友/职业）分三张表，仅输出有数据的类型；好友行附带联动技能名
    private static void AppendGroupSections(StringBuilder sb, Dictionary<string, StatRow> groupStats)
    {
        bool any = false;
        foreach (var type in HeroLineup.GroupTypes)
        {
            var rows = Sort(groupStats.Values.Where(r => r.Type == type));
            if (rows.Count == 0)
                continue;
            any = true;
            sb.AppendLine("-- " + type + " --");
            sb.AppendLine(GroupHeader());
            foreach (var r in rows)
            {
                sb.AppendLine(string.Format("{0,-10}  {1,-10}  {2,8}  {3,8}  {4,8}  {5,8}  {6,8:P1}",
                    r.Name, string.IsNullOrEmpty(r.SkillName) ? "-" : r.SkillName,
                    r.Appear, r.Win, r.Loss, r.Draw, r.Rate));
            }
        }
        if (!any)
            sb.AppendLine("(无数据：未开启任何羁绊加成)");
    }

    private static string GroupHeader()
    {
        return string.Format("{0,-10}  {1,-10}  {2,8}  {3,8}  {4,8}  {5,8}  {6,8}",
            "组名", "技能", "出场", "胜", "负", "平", "胜率");
    }

    private static string HeroName(int heroId)
    {
        var cfg = HeroConfig.GetConfig(heroId);
        return cfg != null ? cfg.Name : ("hero" + heroId);
    }

    // 卡片品质（HeroConfig.Quality，取不到按1）
    private static int HeroQuality(int heroId)
    {
        var cfg = HeroConfig.GetConfig(heroId);
        return cfg != null ? cfg.Quality : 1;
    }

    // 职业名：HeroConfig.Job 为职业缩写（如"王"），取 JobConfig 全称（如"诸侯"）；取不到则用缩写本身
    private static string HeroJob(int heroId)
    {
        var cfg = HeroConfig.GetConfig(heroId);
        if (cfg == null || string.IsNullOrEmpty(cfg.Job))
            return "-";
        var jobCfg = ConfigManager.GetJobConfig(cfg.Job);
        return jobCfg != null ? jobCfg.Name : cfg.Job;
    }

    // 卡片等级：品质1~2 = 设定等级；品质3~4 = 设定等级-1（最低1级）
    private static int LevelForQuality(int heroId, int quality1Level)
    {
        int q = HeroQuality(heroId);
        return Math.Max(1, q <= 2 ? quality1Level : quality1Level - 1);
    }

    private static string ItemName(int itemId)
    {
        var cfg = ItemConfig.GetConfig(itemId);
        return cfg != null ? cfg.Name : ("item" + itemId);
    }

    private static string OnOff(bool on) { return on ? "开" : "关"; }
}