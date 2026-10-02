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
        public int HeroCount = 5;       // 每侧武将数量
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
        public int Quality;             // 卡片品质（仅武将行，物品行为0）
        public int Appear, Win, Loss, Draw;
        public float HeroDamageTotal;   // 对敌方英雄造成的累计伤害（用于场均）
        public float Rate { get { return Appear > 0 ? (float)Win / Appear : 0f; } }
        // 场均对英雄伤害 = 累计伤害 / 出场场次
        public float AvgHeroDamage { get { return Appear > 0 ? HeroDamageTotal / Appear : 0f; } }
    }

    // 一件随机装备的绑定记录（harness 桩仅提供二元 ValueTuple，故用结构体）
    private struct EquipRecord
    {
        public int heroId;
        public int itemId;
        public int side;
    }

    // 本场累积：攻击方 heroId -> 对敌方英雄造成的伤害（由 Chess.OnDamageDealt 驱动，仅统计"英雄打英雄"）
    private static Dictionary<int, float> _heroDamageToHero;

    static BatchSimRunner()
    {
        Chess.OnDamageDealt += (a, v, d, sk) =>
        {
            if (_heroDamageToHero == null || a == null || v == null || d <= 0)
                return;
            if (!a.isHero || !v.isHero)
                return;
            float cur;
            _heroDamageToHero.TryGetValue(a.heroId, out cur);
            _heroDamageToHero[a.heroId] = cur + d;
        };
    }

    // 把本场"英雄打英雄"的伤害累加到各武将行（用于场均伤害）
    private static void AddHeroDamage(Dictionary<int, StatRow> stats, List<(int id, int lv)> team,
        Dictionary<int, float> roundDamage)
    {
        if (roundDamage == null)
            return;
        foreach (var h in team)
        {
            float v;
            if (!roundDamage.TryGetValue(h.id, out v))
                continue;
            StatRow row;
            if (stats.TryGetValue(h.id, out row))
                row.HeroDamageTotal += v;
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

            // 抽 2N 个不重复英雄，前半给甲、后半给乙（同一英雄不会同场出现在双方）
            var pool = new List<int>(heroIds);
            Shuffle(pool);
            int need = Math.Min(pool.Count, heroCount * 2);
            var teamA = new List<(int id, int lv)>();
            var teamB = new List<(int id, int lv)>();
            for (int i = 0; i < need; i++)
            {
                var item = (pool[i], LevelForQuality(pool[i], opt.Quality1Level));
                if (teamA.Count < heroCount) teamA.Add(item);
                else teamB.Add(item);
            }

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
            battle.Start(seed);
            int steps = 0;
            while (!battle.IsFinished && steps < MaxSteps)
            {
                battle.Step(StepDt);
                steps++;
            }
            var roundHeroDamage = _heroDamageToHero;
            _heroDamageToHero = null;   // 本场结束，停止累积

            int winner = !battle.IsFinished ? 0 : (battle.HasWin ? 1 : 2);   // 0=平局
            if (winner == 1) winA++;
            else if (winner == 2) winB++;
            else draw++;

            RecordTeam(teamA, 1, winner, heroStats);
            RecordTeam(teamB, 2, winner, heroStats);
            AddHeroDamage(heroStats, teamA, roundHeroDamage);
            AddHeroDamage(heroStats, teamB, roundHeroDamage);
            foreach (var e in equips)
                RecordOne(itemStats, e.itemId, ItemName(e.itemId), "-", 0, e.side, winner);

            if (onProgress != null)
                onProgress(round + 1, rounds);
        }

        var heroRows = Sort(heroStats.Values);
        var itemRows = Sort(itemStats.Values);
        var jobRows = AggregateByJob(heroRows);
        string report = BuildReport(opt, rounds, winA, winB, draw, heroRows, itemRows, jobRows);

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
        List<StatRow> heroRows, List<StatRow> itemRows, List<StatRow> jobRows)
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

        sb.AppendLine("【武将胜率】按胜率降序（胜率 = 胜场 / 出场场次；场均伤害 = 对敌方英雄造成的伤害 / 出场场次）");
        sb.AppendLine(HeroHeader());
        AppendHeroRows(sb, heroRows);
        sb.AppendLine();

        sb.AppendLine("【职业胜率】按胜率降序（该职业全部英雄的出场/胜负合并统计）");
        sb.AppendLine(JobHeader());
        AppendJobRows(sb, jobRows);
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
        return string.Format("{0,4}  {1,8}  {2,-10}  {3,-6}  {4,4}  {5,6}  {6,6}  {7,6}  {8,6}  {9,8}  {10,10}",
            "排名", "ID", "名字", "职业", "品质", "出场", "胜", "负", "平", "胜率", "场均伤害");
    }

    private static void AppendHeroRows(StringBuilder sb, List<StatRow> rows)
    {
        if (rows.Count == 0)
        {
            sb.AppendLine("(无数据)");
            return;
        }
        int rank = 1;
        foreach (var r in rows)
        {
            sb.AppendLine(string.Format("{0,4}  {1,8}  {2,-10}  {3,-6}  {4,4}  {5,6}  {6,6}  {7,6}  {8,6}  {9,8:P1}  {10,10:N0}",
                rank++, r.Id, r.Name, string.IsNullOrEmpty(r.Job) ? "-" : r.Job,
                r.Quality > 0 ? r.Quality.ToString() : "-", r.Appear, r.Win, r.Loss, r.Draw, r.Rate, r.AvgHeroDamage));
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

    // Fisher-Yates 洗牌（用 SysRandom 保证可复现）
    private static void Shuffle(List<int> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = SysRandom.Range(0, i + 1);
            int t = list[i];
            list[i] = list[j];
            list[j] = t;
        }
    }
}