// ============================================================
// 战斗模拟器 · 程序入口
// 启动方式（均无需传参，配置入口见下）：
//   无参数            打开 GDI+ 主窗体（seed / 双方阵容在窗体中配置）
//   --cli             交互式命令行（输入 help 查看命令）
//   --headless       无界面跑一场战斗，供自动化验证
//                    可选参数：--seed <n> --soldier <n> --a <ids> --b <ids> [--test-mp 1]
//                    --test-mp 1：测试模式速测，英雄初始技能MP拉满+生命×5（不改落盘配置）
//                    （如 --headless --seed 1 --soldier 10 --a 102025,103017 --b 102001）
// ============================================================
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using CommonConfig;
using UnityEngine;

static class Program
{
    [STAThread]
    static void Main(string[] args)
    {
        if (args.Any(a => a == "--cli"))
        {
            CliRunner.Run();
            return;
        }

        bool headless = args.Any(a => a == "--headless");
        if (headless)
        {
            // headless 复现：日志重定向到临时目录，避免与运行中的 GUI 实例抢写同一 GameLog
            UnityEngine.Application.SetPersistentDataPath(Path.Combine(Path.GetTempPath(), "bs_headless_logs"));
        }

        // 初始化游戏配置（加载全部配置类）
        ConfigManager.Init();

        if (headless)
        {
            // headless：解析可选参数 seed/soldier/a/b，未指定则随机阵容 + seed 1
            int seed = 1, soldier = 0, testMp = 0;
            List<int> teamA = null, teamB = null;
            var equips = new List<EquipSpec>();
            for (int i = 0; i < args.Length; i++)
            {
                if (i + 1 >= args.Length)
                    break;
                if (args[i] == "--seed" && int.TryParse(args[i + 1], out int s)) seed = s;
                else if (args[i] == "--soldier" && int.TryParse(args[i + 1], out int sl)) soldier = Math.Max(0, sl);
                else if (args[i] == "--test-mp" && int.TryParse(args[i + 1], out int tm)) testMp = Math.Max(0, tm);
                else if (args[i] == "--a") teamA = HeroLineup.ParseHeroList(args[i + 1]);
                else if (args[i] == "--b") teamB = HeroLineup.ParseHeroList(args[i + 1]);
                else if (args[i] == "--equip")
                {
                    // 格式 pid:heroId:itemId，可重复；pid 0=甲 1=乙
                    var parts = args[i + 1].Split(':');
                    if (parts.Length == 3 && int.TryParse(parts[0], out int pid) && int.TryParse(parts[1], out int h) && int.TryParse(parts[2], out int it))
                        equips.Add(new EquipSpec(pid, h, it));
                    else
                        GameLog.Warn("--equip 格式应为 pid:heroId:itemId，忽略: " + args[i + 1]);
                }
            }
            string eqDesc = string.Join(",", equips.Select(e => e.pid + ":" + e.heroId + ":" + e.itemId));
            if (equips.Count > 0)
                Console.WriteLine("装备注入: " + eqDesc);
            CliRunner.RunBattle(seed, teamA, teamB, soldier, testMp, equips);
            return;
        }

        // GUI：seed 与双方阵容均通过窗体控件配置
        System.Windows.Forms.Application.EnableVisualStyles();
        System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);
        System.Windows.Forms.Application.Run(new MainForm());
    }
}
