// ============================================================
// 战斗模拟器 · 命令行交互版（CLI）
// 面向自动化/快速验证：stdin 逐行读命令，stdout 输出结果。
// 命令：
//   help            显示帮助
//   seed <n>        设置随机种子
//   soldier <n>     设置每侧小兵数量（默认 0）
//   a <ids>         设置甲阵容（如 a 101001,102001,103001）
//   b <ids>         设置乙阵容
//   clear           清空双方阵容（回退随机）
//   show            显示当前配置
//   run             用当前配置打一场
//   rand            随机双阵容打一场
//   batch <n>       随机阵容连打 n 场（seed 递增，输出汇总）
//   hero [关键字]   列出英雄 id 与名字（可过滤）
//   quit / exit     退出
// ============================================================
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using CommonConfig;

public static class CliRunner
{
    [DllImport("kernel32.dll")] private static extern bool AttachConsole(int pid);
    [DllImport("kernel32.dll")] private static extern bool AllocConsole();
    [DllImport("kernel32.dll")] private static extern IntPtr GetStdHandle(int nStdHandle);

    private const int StdInputHandle = -10;
    private const int MaxSteps = 12000;   // 12000 × 0.05 = 600s 上限
    private const float StepDt = 0.05f;

    public static void Run()
    {
        EnsureConsole();
        ConfigManager.Init();
        Utf8Console.WriteLine("=== 战斗模拟器 CLI（输入 help 查看命令，quit 退出） ===");

        int seed = 1;
        int soldier = 0;
        List<int> teamA = null, teamB = null;

        while (true)
        {
            Utf8Console.Write("> ");
            string line = Console.ReadLine();
            if (line == null)
                break; // stdin 管道结束
            line = line.Trim();
            if (line.Length == 0)
                continue;

            var parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            string cmd = parts[0].ToLowerInvariant();
            try
            {
                switch (cmd)
                {
                    case "quit": case "exit": case "q":
                        Utf8Console.WriteLine("bye");
                        return;

                    case "help": case "h":
                        PrintHelp();
                        break;

                    case "seed":
                        if (parts.Length > 1 && int.TryParse(parts[1], out int s))
                        {
                            seed = s;
                            Utf8Console.WriteLine("seed=" + s);
                        }
                        else
                            Utf8Console.WriteLine("用法: seed <数字>");
                        break;

                    case "soldier":
                        if (parts.Length > 1 && int.TryParse(parts[1], out int sl))
                        {
                            soldier = Math.Max(0, sl);
                            Utf8Console.WriteLine("每侧小兵=" + soldier);
                        }
                        else
                            Utf8Console.WriteLine("用法: soldier <0-10>");
                        break;

                    case "a": case "b":
                        if (parts.Length > 1)
                        {
                            var l = HeroLineup.ParseHeroList(parts[1]);
                            if (cmd == "a") teamA = l; else teamB = l;
                            Utf8Console.WriteLine((cmd == "a" ? "甲" : "乙") + "阵容已设置: " + string.Join(",", l));
                        }
                        else
                        {
                            if (cmd == "a") teamA = null; else teamB = null;
                            Utf8Console.WriteLine("已清空" + (cmd == "a" ? "甲" : "乙") + "（回退随机）");
                        }
                        break;

                    case "clear":
                        teamA = null; teamB = null;
                        Utf8Console.WriteLine("已清空双方阵容（回退随机）");
                        break;

                    case "show":
                        PrintConfig(seed, teamA, teamB, soldier);
                        break;

                    case "rand":
                        RunBattle(seed, null, null, soldier);
                        break;

                    case "run":
                        RunBattle(seed, teamA, teamB, soldier);
                        break;

                    case "batch":
                        int n = (parts.Length > 1 && int.TryParse(parts[1], out int bn)) ? bn : 5;
                        Batch(seed, n, soldier);
                        break;

                    case "hero":
                        ListHeroes(parts.Length > 1 ? string.Join(" ", parts.Skip(1)) : null);
                        break;

                    default:
                        Utf8Console.WriteLine("未知命令: " + cmd + "（输入 help 查看命令）");
                        break;
                }
            }
            catch (Exception ex)
            {
                Utf8Console.WriteLine("[CLI错误] " + ex.Message);
            }
        }
    }

    // 用当前配置跑一场并打印结果（headless 入口也复用此方法）
    public static void RunBattle(int seed, List<int> teamA, List<int> teamB, int soldierCount = 0)
    {
        var a = teamA ?? HeroLineup.PickRandomLineup(0);
        var b = teamB ?? HeroLineup.PickRandomLineup(1);
        var battle = new BattleSim();
        battle.Setup(a, b, soldierCount);
        battle.Start(seed);
        Utf8Console.WriteLine("seed=" + seed + " 每侧小兵=" + soldierCount
            + " 阵容A=" + string.Join(",", a) + " 阵容B=" + string.Join(",", b));

        var sw = Stopwatch.StartNew();
        int steps = 0;
        while (!battle.IsFinished && steps < MaxSteps)
        {
            battle.Step(StepDt);
            steps++;
        }
        sw.Stop();

        string result = !battle.IsFinished ? "平局(步数上限)" : (battle.HasWin ? "甲胜" : "乙胜");
        Utf8Console.WriteLine("结果: " + result + " | 墙钟 " + (sw.ElapsedMilliseconds / 1000.0).ToString("F1") + "s 步数 " + steps);
        Utf8Console.WriteLine(battle.GetResultSummary());
    }

    // 随机阵容连打 n 场（seed 递增），输出每场胜负与汇总
    private static void Batch(int baseSeed, int n, int soldierCount = 0)
    {
        int winA = 0, winB = 0, draw = 0;
        for (int i = 0; i < n; i++)
        {
            var a = HeroLineup.PickRandomLineup(i * 2);
            var b = HeroLineup.PickRandomLineup(i * 2 + 1);
            var battle = new BattleSim();
            battle.Setup(a, b, soldierCount);
            battle.Start(baseSeed + i);
            int steps = 0;
            while (!battle.IsFinished && steps < MaxSteps)
            {
                battle.Step(StepDt);
                steps++;
            }

            string r = !battle.IsFinished ? "平局" : (battle.HasWin ? "甲胜" : "乙胜");
            if (r == "甲胜") winA++;
            else if (r == "乙胜") winB++;
            else draw++;
            Utf8Console.WriteLine("第" + (i + 1) + "场 seed=" + (baseSeed + i) + " 结果: " + r + "  甲=" + string.Join(",", a) + " 乙=" + string.Join(",", b));
        }
        Utf8Console.WriteLine("汇总: 甲胜 " + winA + " / 乙胜 " + winB + " / 平局 " + draw);
    }

    private static void ListHeroes(string keyword)
    {
        var all = HeroConfig.ConfigList
            .Where(h => ConfigManager.IsHeroCard((int)h.Id))
            .Select(h => new { Id = (int)h.Id, Name = h.Name })
            .Distinct()
            .ToList();
        if (!string.IsNullOrEmpty(keyword))
            all = all.Where(x => x.Name != null && x.Name.Contains(keyword)).ToList();
        foreach (var h in all)
            Utf8Console.WriteLine(h.Id + " " + h.Name);
    }

    private static void PrintConfig(int seed, List<int> teamA, List<int> teamB, int soldier)
    {
        Utf8Console.WriteLine("seed=" + seed);
        Utf8Console.WriteLine("每侧小兵=" + soldier);
        Utf8Console.WriteLine("甲: " + (teamA != null ? string.Join(",", teamA) : "(随机)"));
        Utf8Console.WriteLine("乙: " + (teamB != null ? string.Join(",", teamB) : "(随机)"));
    }

    private static void PrintHelp()
    {
        Utf8Console.WriteLine("命令列表:");
        Utf8Console.WriteLine("  seed <n>        设置随机种子");
        Utf8Console.WriteLine("  soldier <n>     设置每侧小兵数量（默认 0）");
        Utf8Console.WriteLine("  a <ids>         设置甲阵容，如 a 101001,102001,103001");
        Utf8Console.WriteLine("  b <ids>         设置乙阵容");
        Utf8Console.WriteLine("  clear           清空双方阵容（回退随机）");
        Utf8Console.WriteLine("  show            显示当前配置");
        Utf8Console.WriteLine("  run             用当前配置打一场");
        Utf8Console.WriteLine("  rand            随机双阵容打一场");
        Utf8Console.WriteLine("  batch <n>       随机阵容连打 n 场（seed 递增）");
        Utf8Console.WriteLine("  hero [关键字]   列出英雄 id 与名字");
        Utf8Console.WriteLine("  quit / exit     退出");
    }

    // WinExe 无控制台：stdin 已被重定向（管道）时直接可用；否则附加父控制台，失败则新建
    private static void EnsureConsole()
    {
        try
        {
            IntPtr stdin = GetStdHandle(StdInputHandle);
            if (stdin != IntPtr.Zero && stdin != new IntPtr(-1))
                return;
            if (!AttachConsole(-1))
                AllocConsole();
        }
        catch { }
    }
}
