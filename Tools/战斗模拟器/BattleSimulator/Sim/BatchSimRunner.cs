// ============================================================
// 战斗模拟器 · 批量模拟 —— BatchSimRunner
// 无界面连打 N 轮：每轮随机抽双方阵容（近战前排/远程后排站位），按总价(金币)+各卡价格折算英雄等级，
// 可选给每个武将随机装备一件 400 段道具，可开关 国家/好友/职业 加成，跑完后统计各武将、各物品的胜率
// 与每场耗时分布（正常比赛时间）并输出报告文件。
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
    // 正常比赛耗时 = 逻辑步数 × StepDt + 开场等待：StepDt 与游戏 WorldManager.GameUpdate 的 tick(0.05s) 一致，
    // 模拟器只是按同一步长无等待连续计算（省去真机每步 0.05s 的真实等待），故逻辑时间即真实一局时长
    private const float BattleStartDelaySeconds = 0.5f;   // 游戏 BattleBegin 后 WaitForSeconds(0.5) 才进入 tick 循环
    // 耗时分布区间上界(秒)，最后一档为 ≥ 末值
    private static readonly float[] TimeBuckets = { 5f, 10f, 15f, 20f, 30f, 45f, 60f, 90f, 120f };

    // 随机装备池：400 段成品装备（Id 400xxx）
    private const int ItemIdMin = 400000;
    private const int ItemIdMax = 401000;

    public class Options
    {
        public int HeroCount = 6;       // 每侧武将数量（近战/远程各半，奇数补 1 个随机）
        public int SoldierCount = 4;    // 每侧小兵数量（全部近战士兵，占最前排）
        public int SoldierLevel = 6;    // 小兵等级（1~30，决定小兵攻防加成）
        public int TotalPrice = 50;      // 总价(金币)：每张卡按自身价格折算可购张数→卡牌等级(最高5级)
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
        public float HealTotal;         // 技能真实治疗累计（有效治疗量，不超过目标生命缺口）
        public float DamageTakenTotal;  // 承伤累计（自身生命实际损失 + 自身护盾抵挡的伤害）
        public float ShieldAddTotal;    // 加盾累计（该英雄作为施法者提供的护盾值增量）
        public float Rate { get { return Appear > 0 ? (float)Win / Appear : 0f; } }
        // 场均对英雄伤害 = 累计伤害 / 出场场次
        public float AvgHeroDamage { get { return Appear > 0 ? HeroDamageTotal / Appear : 0f; } }
        // 场均法术伤害 = 累计法术伤害 / 出场场次
        public float AvgMagicDamage { get { return Appear > 0 ? MagicDamageTotal / Appear : 0f; } }
        // 场均治疗量
        public float AvgHeal { get { return Appear > 0 ? HealTotal / Appear : 0f; } }
        // 场均承伤
        public float AvgDamageTaken { get { return Appear > 0 ? DamageTakenTotal / Appear : 0f; } }
        // 场均加盾
        public float AvgShieldAdd { get { return Appear > 0 ? ShieldAddTotal / Appear : 0f; } }
    }

    // 一件随机装备的绑定记录（harness 桩仅提供二元 ValueTuple，故用结构体）
    private struct EquipRecord
    {
        public int heroId;
        public int itemId;
        public int side;
    }

    // 本场累积（由 Chess / BuffShield 事件驱动，统计对象必须是英雄）：
    // _heroDamageToHero：对敌方英雄造成的伤害（英雄打英雄）
    // _magicDamage：造成的法术伤害（SkillConfig.DamageType=法术，任意目标）
    // _healDone：技能真实治疗量（healer → 有效治疗量）
    // _damageTaken：承伤（受击英雄的生命实际损失 + 施法者护盾抵挡的伤害，均按英雄归属）
    // _shieldAdd：加盾（施法者提供的护盾值增量）
    private static Dictionary<int, float> _heroDamageToHero;
    private static Dictionary<int, float> _magicDamage;
    private static Dictionary<int, float> _healDone;
    private static Dictionary<int, float> _damageTaken;
    private static Dictionary<int, float> _shieldAdd;

    static BatchSimRunner()
    {
        Chess.OnDamageDealt += (a, v, d, sk) =>
        {
            // 输出侧：对敌方英雄伤害 / 法术伤害（攻击方必须是英雄）
            if (_heroDamageToHero != null && a != null && a.isHero && d > 0)
            {
                if (v != null && v.isHero)
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
            }
            // 承受侧：受击英雄的生命实际损失（护盾抵挡部分由 OnShieldAbsorb 单独归施法者）
            if (_damageTaken != null && v != null && v.isHero && d > 0)
            {
                float cur;
                _damageTaken.TryGetValue(v.heroId, out cur);
                _damageTaken[v.heroId] = cur + d;
            }
        };
        Chess.OnHealDealt += (healer, target, amount) =>
        {
            if (_healDone == null || healer == null || amount <= 0 || !healer.isHero)
                return;
            float cur;
            _healDone.TryGetValue(healer.heroId, out cur);
            _healDone[healer.heroId] = cur + amount;
        };
        BuffShield.OnShieldAdd += (caster, target, amount) =>
        {
            if (_shieldAdd == null || caster == null || amount <= 0 || !caster.isHero)
                return;
            float cur;
            _shieldAdd.TryGetValue(caster.heroId, out cur);
            _shieldAdd[caster.heroId] = cur + amount;
        };
        BuffShield.OnShieldAbsorb += (caster, defender, amount) =>
        {
            if (_damageTaken == null || caster == null || amount <= 0 || !caster.isHero)
                return;
            float cur;
            _damageTaken.TryGetValue(caster.heroId, out cur);
            _damageTaken[caster.heroId] = cur + amount;
        };
    }

    // 把本场统计累加到各武将行（伤害 / 法术伤害 / 治疗 / 承伤 / 加盾）
    private static void AddRoundStats(Dictionary<int, StatRow> stats, List<(int id, int lv)> team,
        Dictionary<int, float> toHero, Dictionary<int, float> magic,
        Dictionary<int, float> heal, Dictionary<int, float> taken, Dictionary<int, float> shield)
    {
        foreach (var h in team)
        {
            StatRow row;
            if (!stats.TryGetValue(h.id, out row))
                continue;
            float v;
            if (toHero != null && toHero.TryGetValue(h.id, out v))
                row.HeroDamageTotal += v;
            if (magic != null && magic.TryGetValue(h.id, out v))
                row.MagicDamageTotal += v;
            if (heal != null && heal.TryGetValue(h.id, out v))
                row.HealTotal += v;
            if (taken != null && taken.TryGetValue(h.id, out v))
                row.DamageTakenTotal += v;
            if (shield != null && shield.TryGetValue(h.id, out v))
                row.ShieldAddTotal += v;
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
        var roundTimes = new List<float>(rounds);   // 每场耗时(正常比赛秒数)
        int timeouts = 0;                            // 打到步数上限仍无胜负的场次

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
                .Select(id => (id, LevelForPrice(id, opt.TotalPrice))).ToList();
            var teamB = HeroLineup.PickGroupLineup(heroCount, enabledTypes, used, out groupB)
                .Select(id => (id, LevelForPrice(id, opt.TotalPrice))).ToList();

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
            _healDone = new Dictionary<int, float>();
            _damageTaken = new Dictionary<int, float>();
            _shieldAdd = new Dictionary<int, float>();
            battle.Start(seed);
            int steps = 0;
            while (!battle.IsFinished && steps < MaxSteps)
            {
                battle.Step(StepDt);
                steps++;
            }
            // 本场耗时：逻辑步数 × 步长 + 开场等待（= 真机同参数下的一局时长）
            if (!battle.IsFinished)
                timeouts++;
            roundTimes.Add(steps * StepDt + BattleStartDelaySeconds);
            var roundHeroDamage = _heroDamageToHero;
            var roundMagicDamage = _magicDamage;
            var roundHeal = _healDone;
            var roundTaken = _damageTaken;
            var roundShield = _shieldAdd;
            _heroDamageToHero = null;   // 本场结束，停止累积
            _magicDamage = null;
            _healDone = null;
            _damageTaken = null;
            _shieldAdd = null;

            int winner = (!battle.IsFinished || battle.IsDraw) ? 0 : (battle.HasWin ? 1 : 2);   // 0=平局(步数上限或双方同刻全灭)
            if (winner == 1) winA++;
            else if (winner == 2) winB++;
            else draw++;

            RecordTeam(teamA, 1, winner, heroStats);
            RecordTeam(teamB, 2, winner, heroStats);
            RecordGroup(groupStats, groupA, 1, winner);
            RecordGroup(groupStats, groupB, 2, winner);
            AddRoundStats(heroStats, teamA, roundHeroDamage, roundMagicDamage, roundHeal, roundTaken, roundShield);
            AddRoundStats(heroStats, teamB, roundHeroDamage, roundMagicDamage, roundHeal, roundTaken, roundShield);
            foreach (var e in equips)
                RecordOne(itemStats, e.itemId, ItemName(e.itemId), "-", 0, e.side, winner);

            if (onProgress != null)
                onProgress(round + 1, rounds);
        }

        var heroRows = Sort(heroStats.Values);
        var itemRows = Sort(itemStats.Values);
        var jobRows = AggregateByJob(heroRows);
        string report = BuildReport(opt, rounds, winA, winB, draw, heroRows, itemRows, jobRows, groupStats, roundTimes, timeouts);

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
        Dictionary<string, StatRow> groupStats, List<float> roundTimes, int timeouts)
    {
        var sb = new StringBuilder();
        sb.AppendLine("==================================================");
        sb.AppendLine("战斗模拟器 · 批量模拟报告");
        sb.AppendLine("生成时间: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        sb.AppendLine("每侧武将数量: " + opt.HeroCount + "    每侧小兵数量: " + opt.SoldierCount
            + "（近战）    小兵等级: " + opt.SoldierLevel);
        sb.AppendLine("总价: " + opt.TotalPrice
            + " 金币（每张卡按自身价格折算可购张数，再查累计卡数曲线 1/4/8/13/20 得等级，最高5级）");
        sb.AppendLine("国家加成: " + OnOff(opt.EnableFaction)
            + "    好友加成: " + OnOff(opt.EnableFriend)
            + "    职业加成: " + OnOff(opt.EnableJob));
        sb.AppendLine("随机装备(400 段道具): " + OnOff(opt.RandomEquip));
        sb.AppendLine("运行轮数: " + rounds + "    单场步数上限: " + MaxSteps + "(" + (MaxSteps * StepDt) + "s)");
        sb.AppendLine("--------------------------------------------------");
        sb.AppendLine("总胜负: 甲胜 " + winA + " / 乙胜 " + winB + " / 平局 " + draw);
        sb.AppendLine();

        sb.AppendLine("【战斗耗时分布】（正常比赛时间 = 逻辑步数×" + StepDt + "s + 开场等待" + BattleStartDelaySeconds + "s；"
            + "模拟器按与游戏 BattleBegin 相同的 0.05s tick 无等待连算，故逻辑时间即真机一局时长）");
        AppendTimeDistribution(sb, roundTimes, timeouts);
        sb.AppendLine();

        sb.AppendLine("【武将胜率】按胜率降序（胜率 = 胜场 / 出场场次；场均伤害 = 对敌方英雄造成的伤害 / 出场场次；场均法术伤害 = 造成的法术伤害(任意目标) / 出场场次）");
        sb.AppendLine(HeroHeader());
        AppendHeroRows(sb, heroRows, opt.TotalPrice);
        sb.AppendLine();

        sb.AppendLine("【治疗榜 · 前30】按场均治疗量降序（仅技能真实治疗 isHeal=true；有效治疗量 = 不超过目标生命缺口的治疗量；场均 = 累计 / 出场场次）");
        AppendContributionBoard(sb, heroRows, r => r.HealTotal, r => r.AvgHeal, "治疗量");
        sb.AppendLine();

        sb.AppendLine("【承伤榜 · 前30】按场均承伤降序（承伤 = 受击英雄的生命实际损失 + 该英雄作为施法者其护盾抵挡的伤害；场均 = 累计 / 出场场次）");
        AppendContributionBoard(sb, heroRows, r => r.DamageTakenTotal, r => r.AvgDamageTaken, "承伤");
        sb.AppendLine();

        sb.AppendLine("【加盾榜 · 前30】按场均加盾降序（加盾 = 该英雄作为施法者提供的护盾值增量（刷新护盾只计增量），归施法者；场均 = 累计 / 出场场次）");
        AppendContributionBoard(sb, heroRows, r => r.ShieldAddTotal, r => r.AvgShieldAdd, "加盾");
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

    // 战斗耗时分布：平均/最短/最长/中位/P90 + 分区间直方图 + 步数上限未分胜负场次
    private static void AppendTimeDistribution(StringBuilder sb, List<float> times, int timeouts)
    {
        if (times == null || times.Count == 0)
        {
            sb.AppendLine("(无数据)");
            return;
        }
        var sorted = new List<float>(times);
        sorted.Sort();
        float sum = 0f;
        foreach (var t in times)
            sum += t;
        sb.AppendLine(string.Format("  平均 {0:N1}s    最短 {1:N1}s    最长 {2:N1}s    中位P50 {3:N1}s    P90 {4:N1}s",
            sum / times.Count, sorted[0], sorted[sorted.Count - 1], Percentile(sorted, 0.5f), Percentile(sorted, 0.9f)));
        sb.AppendLine("  打到步数上限(" + MaxSteps + "步=" + (MaxSteps * StepDt) + "s)仍未分胜负（计入平局）: " + timeouts + " 场");

        var buckets = new int[TimeBuckets.Length + 1];
        foreach (var t in times)
        {
            int idx = TimeBuckets.Length;   // 默认落最后一档(≥末值)
            for (int i = 0; i < TimeBuckets.Length; i++)
            {
                if (t < TimeBuckets[i]) { idx = i; break; }
            }
            buckets[idx]++;
        }
        sb.AppendLine(string.Format("  {0,-12}{1,8}{2,10}", "耗时区间", "场次", "占比"));
        float low = 0f;
        for (int i = 0; i < TimeBuckets.Length; i++)
        {
            sb.AppendLine(string.Format("  {0,-12}{1,8}{2,10:P1}",
                RangeLabel(low, TimeBuckets[i]), buckets[i], (float)buckets[i] / times.Count));
            low = TimeBuckets[i];
        }
        sb.AppendLine(string.Format("  {0,-12}{1,8}{2,10:P1}",
            "≥" + TimeBuckets[TimeBuckets.Length - 1] + "s", buckets[TimeBuckets.Length],
            (float)buckets[TimeBuckets.Length] / times.Count));
    }

    private static string RangeLabel(float low, float high)
    {
        return (low <= 0f ? "0" : low.ToString("0")) + "~" + high.ToString("0") + "s";
    }

    // 已排序列表的 p 分位（0~1）
    private static float Percentile(List<float> sorted, float p)
    {
        if (sorted.Count == 0)
            return 0f;
        int idx = (int)Math.Round((sorted.Count - 1) * p);
        return sorted[Math.Max(0, Math.Min(sorted.Count - 1, idx))];
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

    private static void AppendHeroRows(StringBuilder sb, List<StatRow> rows, int totalPrice)
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
                LevelForPrice(r.Id, totalPrice),
                r.Appear, r.Win, r.Loss, r.Draw, r.Rate, r.AvgHeroDamage, r.AvgMagicDamage));
        }
    }

    // 英雄贡献榜（治疗/承伤/加盾）：按场均降序取前30，仅列出该指标累计>0的英雄
    private static void AppendContributionBoard(StringBuilder sb, List<StatRow> heroRows,
        Func<StatRow, float> total, Func<StatRow, float> avg, string valueName)
    {
        var rows = heroRows
            .Where(r => avg(r) > 0f)
            .OrderByDescending(avg)
            .ThenByDescending(total)
            .ThenBy(r => r.Id)
            .Take(30)
            .ToList();
        if (rows.Count == 0)
        {
            sb.AppendLine("(无数据)");
            return;
        }
        sb.AppendLine(string.Format("{0,4}  {1,8}  {2,-10}  {3,-6}  {4,4}  {5,6}  {6,12}  {7,12}",
            "排名", "ID", "名字", "职业", "品质", "出场", "场均" + valueName, "累计" + valueName));
        int rank = 1;
        foreach (var r in rows)
        {
            sb.AppendLine(string.Format("{0,4}  {1,8}  {2,-10}  {3,-6}  {4,4}  {5,6}  {6,12:N0}  {7,12:N0}",
                rank++, r.Id, r.Name, string.IsNullOrEmpty(r.Job) ? "-" : r.Job,
                r.Quality > 0 ? r.Quality.ToString() : "-",
                r.Appear, avg(r), total(r)));
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

    // 卡片等级：以总价(金币)为预算，按卡自身价格折算可购张数，再走累计卡数曲线(1/4/8/13/20)得等级，最高5级
    private static int LevelForPrice(int heroId, int totalPrice)
    {
        int price = HeroPrice(heroId);
        if (price <= 0)
            return 1;
        int copies = Math.Max(1, totalPrice / price);   // 价格高于预算时至少按 Lv1 出场
        return HeroSelectionTool.GetCardLevel(copies, true);
    }

    // 卡片价格（HeroConfig.Price，2~10；取不到按0）
    private static int HeroPrice(int heroId)
    {
        var cfg = HeroConfig.GetConfig(heroId);
        return cfg != null ? cfg.Price : 0;
    }

    private static string ItemName(int itemId)
    {
        var cfg = ItemConfig.GetConfig(itemId);
        return cfg != null ? cfg.Name : ("item" + itemId);
    }

    private static string OnOff(bool on) { return on ? "开" : "关"; }
}