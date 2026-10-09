// ============================================================
// 选牌模拟器 · 程序入口
// 复用游戏真实源码（CardShopManager / ShopMode / PlayerAI / PlayerInfo），
// 在 WinForm 中输入回合数后，无头驱动 8 名 AI 玩家逐回合在随机商店中买牌，
// 用于验证实际游戏的 AI 选牌正确性。
// 配置初始化（ConfigManager.Init）在 CardPickSim.Run 内完成。
// ============================================================
using System;
using System.Windows.Forms;

static class Program
{
    [STAThread]
    static void Main(string[] args)
    {
        System.Windows.Forms.Application.EnableVisualStyles();
        System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);
        System.Windows.Forms.Application.Run(new MainForm());
    }
}
