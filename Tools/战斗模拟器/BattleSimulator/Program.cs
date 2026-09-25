// ============================================================
// 战斗模拟器 · 程序入口
// 启动方式（均无需传参，配置入口见下）：
//   无参数            打开 GDI+ 主窗体（seed / 双方阵容在窗体中配置）
//   --cli             交互式命令行（输入 help 查看命令）
//   --headless        无界面跑一场写死的默认战斗（随机阵容 seed=1），供自动化验证
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

        if (args.Any(a => a == "--headless"))
        {
            // headless 复现：日志重定向到临时目录，避免与运行中的 GUI 实例抢写同一 GameLog
            UnityEngine.Application.SetPersistentDataPath(Path.Combine(Path.GetTempPath(), "bs_headless_logs"));
        }

        // 初始化游戏配置（加载全部配置类）
        ConfigManager.Init();

        if (args.Any(a => a == "--headless"))
        {
            // headless：随机阵容 + seed 1，供自动化验证（日志已重定向，不与 GUI 抢写）
            CliRunner.RunBattle(1, null, null);
            return;
        }

        // GUI：seed 与双方阵容均通过窗体控件配置
        System.Windows.Forms.Application.EnableVisualStyles();
        System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);
        System.Windows.Forms.Application.Run(new MainForm());
    }
}
