// ============================================================
// 选牌模拟器 · 程序入口
// 复用游戏真实源码（CardShopManager / ShopMode / PlayerAI / PlayerInfo），
// 在 WinForm 中输入回合数后，无头驱动 8 名 AI 玩家逐回合在随机商店中买牌，
// 用于验证实际游戏的 AI 选牌正确性。
// 配置初始化（ConfigManager.Init）在 CardPickSim.Run 内完成。
// 支持命令行无头模式：--headless --rounds N（stdout 输出 UTF-8，供自动化冒烟）。
// ============================================================
using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;

static class Program
{
    [STAThread]
    static void Main(string[] args)
    {
        if (args.Any(a => a == "--headless"))
        {
            RunHeadless(args);
            return;
        }

        System.Windows.Forms.Application.EnableVisualStyles();
        System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);
        System.Windows.Forms.Application.Run(new MainForm());
    }

    // 无头冒烟：--headless --rounds 50 [--seed N]，逐行 UTF-8 输出到 stdout
    private static void RunHeadless(string[] args)
    {
        int rounds = 50;
        int seed = int.MinValue;
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == "--rounds" && i + 1 < args.Length && int.TryParse(args[i + 1], out int r))
                rounds = Math.Max(1, r);
            else if (args[i] == "--seed" && i + 1 < args.Length && int.TryParse(args[i + 1], out int s))
                seed = s;
        }
        // 指定 seed 时固定随机序列，便于 A/B 对比复现
        if (seed != int.MinValue)
            SysRandom.Seed(seed);

        // 日志重定向到临时目录，避免与运行中的 GUI 实例抢写同一 GameLog 文件
        UnityEngine.Application.SetPersistentDataPath(Path.Combine(Path.GetTempPath(), "cps_headless_logs"));

        var sim = new CardPickSim { Rounds = rounds };
        sim.InitLogFile(Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\Logs")));
        sim.OnLog = line => Utf8Console.WriteLine(line);
        bool ok = false;
        sim.OnFinished = (success, msg) => { ok = success; Utf8Console.WriteLine($"FINISHED ok={success} {msg}"); };
        sim.Run();
        Environment.Exit(ok ? 0 : 1);
    }
}
