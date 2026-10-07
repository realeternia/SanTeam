// ============================================================
// 战斗模拟器 · 批量模拟参数窗口 —— BatchSimForm
// 由主窗体菜单打开：输入双方武将数量/小兵数量/小兵等级/总价(金币)/羁绊开关/运行轮数/是否随机装备，
// 点"开始战斗"后无界面连打并把各武将、各物品胜率报告输出到日志文件。
// ============================================================
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

public class BatchSimForm : Form
{
    private NumericUpDown _heroCountBox;
    private NumericUpDown _soldierCountBox;
    private NumericUpDown _soldierLevelBox;
    private NumericUpDown _totalPriceBox;
    private CheckBox _factionChk, _friendChk, _jobChk, _equipChk;
    private NumericUpDown _roundsBox;
    private Button _startBtn, _openBtn;
    private Label _status;
    private string _lastReport;

    public BatchSimForm()
    {
        Text = "批量模拟战斗";
        Width = 440;
        Height = 406;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        StartPosition = FormStartPosition.CenterParent;
        BuildUi();
    }

    private void BuildUi()
    {
        int left = 16, top = 16, h = 28;
        Label lbl;

        lbl = new Label { Text = "每侧武将数量:", Left = left, Top = top + 5, Width = 110 };
        Controls.Add(lbl);
        _heroCountBox = new NumericUpDown { Left = left + 115, Top = top, Width = 70, Minimum = 1, Maximum = 6, Value = 6 };
        Controls.Add(_heroCountBox);
        top += h + 4;

        lbl = new Label { Text = "每侧小兵数量(0~20):", Left = left, Top = top + 5, Width = 110 };
        Controls.Add(lbl);
        _soldierCountBox = new NumericUpDown { Left = left + 115, Top = top, Width = 70, Minimum = 0, Maximum = 20, Value = 4 };
        Controls.Add(_soldierCountBox);
        lbl = new Label { Text = "（全部近战小兵，占最前排）", Left = left + 190, Top = top + 5, Width = 180, ForeColor = Color.Gray };
        Controls.Add(lbl);
        top += h + 4;

        lbl = new Label { Text = "小兵等级(1~30):", Left = left, Top = top + 5, Width = 110 };
        Controls.Add(lbl);
        _soldierLevelBox = new NumericUpDown { Left = left + 115, Top = top, Width = 70, Minimum = 1, Maximum = 30, Value = 6 };
        Controls.Add(_soldierLevelBox);
        top += h + 4;

        lbl = new Label { Text = "总价(金币):", Left = left, Top = top + 5, Width = 110 };
        Controls.Add(lbl);
        _totalPriceBox = new NumericUpDown { Left = left + 115, Top = top, Width = 70, Minimum = 1, Maximum = 100000, Value = 50 };
        Controls.Add(_totalPriceBox);
        lbl = new Label { Text = "（每卡按自身价格折算可购张数→等级，最高5级）", Left = left + 190, Top = top + 5, Width = 210, ForeColor = Color.Gray };
        Controls.Add(lbl);
        top += h + 4;

        lbl = new Label { Text = "羁绊加成（开启后按该类型成组抽阵容，默认全关）:", Left = left, Top = top + 5, Width = 390 };
        Controls.Add(lbl);
        top += h;
        _factionChk = new CheckBox { Text = "国家", Left = left + 10, Top = top, Width = 90 };
        _friendChk = new CheckBox { Text = "好友", Left = left + 110, Top = top, Width = 90 };
        _jobChk = new CheckBox { Text = "职业", Left = left + 210, Top = top, Width = 90 };
        Controls.Add(_factionChk);
        Controls.Add(_friendChk);
        Controls.Add(_jobChk);
        top += h + 4;

        lbl = new Label { Text = "运行轮数:", Left = left, Top = top + 5, Width = 110 };
        Controls.Add(lbl);
        _roundsBox = new NumericUpDown { Left = left + 115, Top = top, Width = 70, Minimum = 1, Maximum = 100000, Value = 100 };
        Controls.Add(_roundsBox);
        top += h + 4;

        _equipChk = new CheckBox { Text = "随机给装备（每个武将一件 400 段道具）", Left = left, Top = top, Width = 390 };
        Controls.Add(_equipChk);
        top += h + 8;

        _startBtn = new Button { Text = "开始战斗", Left = left, Top = top, Width = 110, Height = 30 };
        _startBtn.Click += (s, e) => StartBatch();
        Controls.Add(_startBtn);

        _openBtn = new Button { Text = "打开报告", Left = left + 120, Top = top, Width = 100, Height = 30, Enabled = false };
        _openBtn.Click += (s, e) => OpenReport();
        Controls.Add(_openBtn);
        top += 42;

        _status = new Label { Left = left, Top = top, Width = 395, Height = 70, ForeColor = Color.Gray };
        Controls.Add(_status);
    }

    private void InitializeComponent()
    {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BatchSimForm));
            this.SuspendLayout();
            // 
            // BatchSimForm
            // 
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "BatchSimForm";
            this.ResumeLayout(false);

    }

    // 开始批量模拟（无界面连打，后台线程执行，避免卡住窗体）
    private async void StartBatch()
    {
        var opt = new BatchSimRunner.Options
        {
            HeroCount = (int)_heroCountBox.Value,
            SoldierCount = (int)_soldierCountBox.Value,
            SoldierLevel = (int)_soldierLevelBox.Value,
            TotalPrice = (int)_totalPriceBox.Value,
            EnableFaction = _factionChk.Checked,
            EnableFriend = _friendChk.Checked,
            EnableJob = _jobChk.Checked,
            Rounds = (int)_roundsBox.Value,
            RandomEquip = _equipChk.Checked,
        };

        SetRunning(true);
        _openBtn.Enabled = false;
        _status.ForeColor = Color.Gray;
        _status.Text = "模拟运行中…";

        string path = null;
        try
        {
            path = await Task.Run(() => BatchSimRunner.Run(opt, ReportProgress));
        }
        catch (Exception ex)
        {
            GameLog.Error("批量模拟异常: " + ex);
            MessageBox.Show(this, "模拟失败: " + ex.Message, "错误");
        }

        SetRunning(false);
        if (!string.IsNullOrEmpty(path))
        {
            _lastReport = path;
            _openBtn.Enabled = true;
            _status.ForeColor = Color.Green;
            _status.Text = "完成，报告已输出：" + path;
        }
        else
        {
            _status.ForeColor = Color.OrangeRed;
            _status.Text = "未生成报告（请检查英雄池/参数）";
        }
    }

    // 后台线程进度回调（切回 UI 线程更新）
    private void ReportProgress(int done, int total)
    {
        if (IsDisposed || !IsHandleCreated)
            return;
        try
        {
            BeginInvoke(new Action(() => { _status.Text = "模拟运行中… " + done + "/" + total + " 轮"; }));
        }
        catch
        {
            // 窗体已关闭，忽略
        }
    }

    private void SetRunning(bool running)
    {
        _heroCountBox.Enabled = !running;
        _soldierCountBox.Enabled = !running;
        _soldierLevelBox.Enabled = !running;
        _totalPriceBox.Enabled = !running;
        _factionChk.Enabled = !running;
        _friendChk.Enabled = !running;
        _jobChk.Enabled = !running;
        _roundsBox.Enabled = !running;
        _equipChk.Enabled = !running;
        _startBtn.Enabled = !running;
    }

    private void OpenReport()
    {
        if (string.IsNullOrEmpty(_lastReport) || !File.Exists(_lastReport))
            return;
        try
        {
            Process.Start(_lastReport);
        }
        catch (Exception ex)
        {
            GameLog.Warn("打开报告失败: " + ex.Message);
        }
    }
}