// ============================================================
// 选牌模拟器 · WinForm 主窗体 —— MainForm
// 输入模拟回合数（默认 30），点“开始模拟”后复用游戏真实 AI/商店源码，
// 无头驱动 8 名 AI 玩家逐回合买牌；过程中买入/卖出记录实时输出到日志窗口并落盘。
// 模拟在后台线程执行，日志通过 BeginInvoke 回到 UI 线程刷新，避免界面卡死。
// ============================================================
using System;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;

public class MainForm : Form
{
    private NumericUpDown _roundBox;
    private Button _startBtn;
    private Button _openLogBtn;
    private RichTextBox _logBox;
    private Label _statusLabel;

    private volatile bool _running;
    private string _lastLogFile;

    public MainForm()
    {
        Text = "选牌模拟器";
        Width = 900;
        Height = 720;
        BuildUi();
    }

    private void BuildUi()
    {
        int left = 12, top = 12, h = 26;

        var lbl = new Label { Text = "模拟轮次:", Left = left, Top = top + 5, Width = 70, AutoSize = true };
        Controls.Add(lbl);

        _roundBox = new NumericUpDown
        {
            Left = left + 75,
            Top = top,
            Width = 70,
            Minimum = 1,
            Maximum = 100,   // 配置表 GameRoundConfig 上限 100
            Value = 30,
        };
        Controls.Add(_roundBox);

        _startBtn = new Button { Text = "开始模拟", Left = left + 160, Top = top, Width = 90 };
        _startBtn.Click += (_s, _e) => StartSim();
        Controls.Add(_startBtn);

        _openLogBtn = new Button { Text = "打开日志目录", Left = left + 258, Top = top, Width = 110, Enabled = false };
        _openLogBtn.Click += (_s, _e) => OpenLogDir();
        Controls.Add(_openLogBtn);

        _statusLabel = new Label
        {
            Left = left + 380,
            Top = top + 5,
            AutoSize = true,
            ForeColor = Color.Gray,
            Text = "就绪：输入轮次后点击“开始模拟”",
        };
        Controls.Add(_statusLabel);

        top += h + 6;

        _logBox = new RichTextBox
        {
            Left = left,
            Top = top,
            Width = ClientSize.Width - 2 * left,
            Height = ClientSize.Height - top - 16,
            ReadOnly = true,
            Multiline = true,
            ScrollBars = RichTextBoxScrollBars.Vertical,
            BackColor = Color.FromArgb(20, 24, 34),
            ForeColor = Color.LightGray,
            WordWrap = false,
            DetectUrls = false,
            Font = new Font("Microsoft YaHei", 9),
            BorderStyle = BorderStyle.FixedSingle,
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
        };
        Controls.Add(_logBox);
    }

    private void StartSim()
    {
        if (_running)
            return;

        int rounds = (int)_roundBox.Value;
        _running = true;
        _startBtn.Enabled = false;
        _roundBox.Enabled = false;
        _openLogBtn.Enabled = false;
        _logBox.Clear();
        _statusLabel.Text = "模拟中…";
        _statusLabel.ForeColor = Color.LimeGreen;

        var sim = new CardPickSim { Rounds = rounds };
        sim.InitLogFile(LogDir);
        _lastLogFile = sim.LogFilePath;
        sim.OnLog = line => AppendLog(line);
        sim.OnFinished = (ok, msg) => FinishSim(ok, msg);

        var thread = new Thread(() => sim.Run()) { IsBackground = true, Name = "CardPickSim" };
        thread.Start();
    }

    private void FinishSim(bool ok, string message)
    {
        if (IsDisposed || !IsHandleCreated)
            return;
        BeginInvoke((Action)(() =>
        {
            _running = false;
            _startBtn.Enabled = true;
            _roundBox.Enabled = true;
            _openLogBtn.Enabled = _lastLogFile != null;
            _statusLabel.Text = message;
            _statusLabel.ForeColor = ok ? Color.LimeGreen : Color.OrangeRed;
        }));
    }

    private void AppendLog(string line)
    {
        if (IsDisposed || !IsHandleCreated)
            return;
        BeginInvoke((Action)(() =>
        {
            _logBox.SelectionStart = _logBox.TextLength;
            _logBox.SelectionLength = 0;
            _logBox.AppendText(line + "\r\n");
            // 裁剪顶部，避免长跑后文本框过大
            if (_logBox.Lines.Length > 5000)
            {
                string[] lines = _logBox.Lines;
                int keep = 2000;
                var tail = new string[keep];
                Array.Copy(lines, lines.Length - keep, tail, 0, keep);
                _logBox.Lines = tail;
            }
            _logBox.SelectionStart = _logBox.TextLength;
            _logBox.ScrollToCaret();
        }));
    }

    private void OpenLogDir()
    {
        try
        {
            if (_lastLogFile != null && File.Exists(_lastLogFile))
                System.Diagnostics.Process.Start("explorer.exe", "/select,\"" + _lastLogFile + "\"");
            else
                System.Diagnostics.Process.Start("explorer.exe", "\"" + LogDir + "\"");
        }
        catch (Exception e)
        {
            MessageBox.Show("打开日志目录失败：" + e.Message, "提示");
        }
    }

    // 模拟器目录下 Logs/（与 GameLog 输出目录一致）
    private static string LogDir
    {
        get { return Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\Logs")); }
    }
}
