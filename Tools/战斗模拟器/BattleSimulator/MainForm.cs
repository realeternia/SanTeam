// ============================================================
// 战斗模拟器 · GDI+ 主窗体 —— MainForm
// 绘制战斗地图网格 / 单位（英雄方形+头像图片，士兵圆形）/ HP 条 / 飘字，
// 不做技能特效。控制区：seed、英雄下拉选择、随机阵容、开始/暂停/重置、速度。
// 防闪烁：画布用自定义 CanvasPanel 开启 OptimizedDoubleBuffer。
// ============================================================
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using CommonConfig;
using UnityEngine;
using SDColor = System.Drawing.Color;

// 双缓冲画布：UserPaint + AllPaintingInWmPaint + OptimizedDoubleBuffer 消除重绘闪烁
public class CanvasPanel : Panel
{
    public CanvasPanel()
    {
        SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                 ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
    }
}

public class MainForm : Form
{
    private BattleSim _battle;
    private Timer _timer;

    // 控件
    private TextBox _seedBox;
    private ComboBox _heroBox;            // 英雄下拉（id+名字）
    private Button _toABtn, _toBBtn;      // 添加到甲/乙
    private Button _removeABtn, _removeBBtn;
    private ListBox _teamAList, _teamBList;
    private Button _randomABtn, _randomBBtn;
    private Button _startBtn, _pauseBtn, _resetBtn;
    private ComboBox _speedBox;
    private Label _statusLabel;
    private CanvasPanel _canvas;

    private float _speed = 1f;
    private bool _paused = true;
    private bool _battleDone;

    // 世界 → 画面缩放
    private float _scale = 8f;
    private PointF _viewCenter;

    // 英雄头像缓存（Textures/Skins/{Icon}.jpg）
    private static readonly string SkinDir = Path.GetFullPath(Path.Combine(
        AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\..\..\Assets\Resources\Textures\Skins\"));
    private readonly Dictionary<string, Image> _iconCache = new Dictionary<string, Image>();

    // 下拉项：显示 "id 名字"，Tag 存 heroId
    private class HeroItem
    {
        public int Id;
        public string Name;
        public override string ToString() { return Id + " " + Name; }
    }

    public MainForm()
    {
        Text = "战斗模拟器";
        Width = 1100;
        Height = 800;
        _battle = new BattleSim();

        BuildUi();
        _timer = new Timer { Interval = 16 };
        _timer.Tick += (_s, _e) => Tick();
        _timer.Start();

        _canvas.Paint += OnPaintCanvas;
        _canvas.Resize += (_s, _e) => _canvas.Invalidate();
    }

    // ---------- UI ----------
    private void BuildUi()
    {
        int left = 10, top = 8, h = 26;
        Label lbl;

        // seed
        lbl = new Label { Text = "seed:", Left = left, Top = top + 5, Width = 40 };
        Controls.Add(lbl);
        _seedBox = new TextBox { Text = "1", Left = left + 45, Top = top, Width = 60 };
        Controls.Add(_seedBox);

        // 英雄下拉选择
        lbl = new Label { Text = "选择英雄:", Left = left + 118, Top = top + 5, Width = 60 };
        Controls.Add(lbl);
        _heroBox = new ComboBox { Left = left + 178, Top = top, Width = 300, DropDownStyle = ComboBoxStyle.DropDownList };
        foreach (var hid in HeroConfig.ConfigList
                     .Where(c => ConfigManager.IsHeroCard((int)c.Id))
                     .Select(c => new HeroItem { Id = (int)c.Id, Name = c.Name })
                     .OrderBy(x => x.Id))
            _heroBox.Items.Add(hid);
        if (_heroBox.Items.Count > 0)
            _heroBox.SelectedIndex = 0;
        Controls.Add(_heroBox);

        _toABtn = new Button { Text = "→甲", Left = left + 484, Top = top, Width = 46 };
        _toABtn.Click += (_s, _e) => AddToTeam(_teamAList);
        Controls.Add(_toABtn);
        _toBBtn = new Button { Text = "→乙", Left = left + 534, Top = top, Width = 46 };
        _toBBtn.Click += (_s, _e) => AddToTeam(_teamBList);
        Controls.Add(_toBBtn);
        _randomABtn = new Button { Text = "随机甲", Left = left + 586, Top = top, Width = 60 };
        _randomABtn.Click += (_s, _e) => { FillRandom(_teamAList); };
        Controls.Add(_randomABtn);
        _randomBBtn = new Button { Text = "随机乙", Left = left + 650, Top = top, Width = 60 };
        _randomBBtn.Click += (_s, _e) => { FillRandom(_teamBList); };
        Controls.Add(_randomBBtn);
        top += h + 4;

        // 甲阵容列表
        lbl = new Label { Text = "甲(英雄):", Left = left, Top = top + 5, Width = 70 };
        Controls.Add(lbl);
        _teamAList = new ListBox { Left = left + 75, Top = top, Width = 380, Height = 150 };
        _teamAList.Items.AddRange(ParseHeroList("101001,101002,103007"));
        Controls.Add(_teamAList);
        _removeABtn = new Button { Text = "移除", Left = left + 460, Top = top, Width = 60 };
        _removeABtn.Click += (_s, _e) => RemoveSelected(_teamAList);
        Controls.Add(_removeABtn);

        // 乙阵容列表
        lbl = new Label { Text = "乙(英雄):", Left = left + 545, Top = top + 5, Width = 70 };
        Controls.Add(lbl);
        _teamBList = new ListBox { Left = left + 620, Top = top, Width = 380, Height = 150 };
        _teamBList.Items.AddRange(ParseHeroList("102001,102002,102003"));
        Controls.Add(_teamBList);
        _removeBBtn = new Button { Text = "移除", Left = left + 1005, Top = top, Width = 60 };
        _removeBBtn.Click += (_s, _e) => RemoveSelected(_teamBList);
        Controls.Add(_removeBBtn);
        top += 154;

        // 控制按钮
        _startBtn = new Button { Text = "开始", Left = left, Top = top, Width = 70 };
        _startBtn.Click += (_s, _e) => StartBattle();
        Controls.Add(_startBtn);

        _pauseBtn = new Button { Text = "暂停", Left = left + 76, Top = top, Width = 70 };
        _pauseBtn.Click += (_s, _e) =>
        {
            _paused = !_paused;
            _pauseBtn.Text = _paused ? "继续" : "暂停";
        };
        Controls.Add(_pauseBtn);

        _resetBtn = new Button { Text = "重置", Left = left + 152, Top = top, Width = 70 };
        _resetBtn.Click += (_s, _e) => ResetBattle();
        Controls.Add(_resetBtn);

        lbl = new Label { Text = "速度:", Left = left + 235, Top = top + 5, Width = 45 };
        Controls.Add(lbl);
        _speedBox = new ComboBox { Left = left + 280, Top = top, Width = 60, DropDownStyle = ComboBoxStyle.DropDownList };
        _speedBox.Items.AddRange(new object[] { "1x", "2x", "4x" });
        _speedBox.SelectedIndex = 0;
        _speedBox.SelectedIndexChanged += (_s, _e) =>
        {
            _speed = new[] { 1f, 2f, 4f }[_speedBox.SelectedIndex];
        };
        Controls.Add(_speedBox);

        _statusLabel = new Label { Left = left + 360, Top = top + 5, AutoSize = true, ForeColor = SDColor.Gray };
        Controls.Add(_statusLabel);
        top += h + 8;

        // 画布
        _canvas = new CanvasPanel { Left = left, Top = top, Width = ClientSize.Width - 2 * left - 16, Height = ClientSize.Height - top - 16 };
        _canvas.BackColor = SDColor.FromArgb(30, 36, 52);
        Controls.Add(_canvas);
    }

    // 把下拉选中的英雄加到对应阵容（去重，最多 5 个）
    private void AddToTeam(ListBox team)
    {
        if (_heroBox.SelectedItem is HeroItem item)
        {
            if (team.Items.Cast<HeroItem>().Any(x => x.Id == item.Id))
                return;
            if (team.Items.Count >= 5)
            {
                _statusLabel.Text = "阵容最多 5 个英雄";
                return;
            }
            team.Items.Add(item);
        }
    }

    private void RemoveSelected(ListBox team)
    {
        if (team.SelectedIndex >= 0)
            team.Items.RemoveAt(team.SelectedIndex);
    }

    private void FillRandom(ListBox team)
    {
        team.Items.Clear();
        team.Items.AddRange(BuildRandomLineup());
    }

    // 从 "id,id" 字符串构造 HeroItem 列表（默认阵容初始化用）
    private HeroItem[] ParseHeroList(string text)
    {
        var list = new List<HeroItem>();
        foreach (var part in text.Split(','))
        {
            if (int.TryParse(part.Trim(), out int id) && ConfigManager.IsHeroCard(id))
            {
                var cfg = HeroConfig.GetConfig(id);
                if (cfg != null)
                    list.Add(new HeroItem { Id = id, Name = cfg.Name });
            }
        }
        return list.ToArray();
    }

    private HeroItem[] BuildRandomLineup()
    {
        var all = HeroConfig.ConfigList
            .Where(h => ConfigManager.IsHeroCard((int)h.Id))
            .Select(h => new HeroItem { Id = (int)h.Id, Name = h.Name })
            .Distinct()
            .OrderBy(x => SysRandom.Range(0, 1000))   // 伪随机洗牌
            .Take(5)
            .ToArray();
        return all;
    }

    // ---------- 战斗控制 ----------
    public void SetSeed(int seed) { _seedBox.Text = seed.ToString(); }
    public void SetTeamA(List<int> ids) { FillTeam(_teamAList, ids); }
    public void SetTeamB(List<int> ids) { FillTeam(_teamBList, ids); }

    private void FillTeam(ListBox team, List<int> ids)
    {
        team.Items.Clear();
        foreach (var id in ids)
        {
            if (ConfigManager.IsHeroCard(id))
            {
                var cfg = HeroConfig.GetConfig(id);
                if (cfg != null)
                    team.Items.Add(new HeroItem { Id = id, Name = cfg.Name });
            }
        }
    }

    private List<int> TeamIds(ListBox team)
    {
        return team.Items.Cast<HeroItem>().Select(x => x.Id).ToList();
    }

    private void StartBattle()
    {
        if (!int.TryParse(_seedBox.Text, out int seed))
            seed = 1;

        var teamA = TeamIds(_teamAList);
        var teamB = TeamIds(_teamBList);
        if (teamA.Count == 0)
        {
            MessageBox.Show("阵容甲不能为空，请通过下拉选择英雄", "提示");
            return;
        }

        _battle.Setup(teamA, teamB);
        _battle.Start(seed);
        _paused = false;
        _battleDone = false;
        _pauseBtn.Text = "暂停";
        _statusLabel.Text = "战斗进行中… 甲" + teamA.Count + " vs 乙" + teamB.Count;
        _statusLabel.ForeColor = SDColor.LimeGreen;
        _canvas.Invalidate();
    }

    private void ResetBattle()
    {
        _battle = new BattleSim();
        _paused = true;
        _battleDone = false;
        _pauseBtn.Text = "暂停";
        _statusLabel.Text = "已重置，点击开始";
        _statusLabel.ForeColor = SDColor.Gray;
        _canvas.Invalidate();
    }

    private void Tick()
    {
        if (_paused || _battleDone || _battle == null)
            return;
        int n = (int)_speed;
        for (int i = 0; i < n; i++)
        {
            _battle.Step(0.05f);
            if (_battle.IsFinished)
            {
                _battleDone = true;
                _statusLabel.Text = "战斗结束：" + (_battle.HasWin ? "甲胜" : "乙胜")
                    + "（时长 " + _battle.BattleTime.ToString("F1") + "s）";
                _statusLabel.ForeColor = SDColor.Orange;
                Utf8Console.WriteLine(_battle.GetResultSummary());
                break;
            }
        }
        _canvas.Invalidate();
    }

    // ---------- 绘制 ----------
    private void OnPaintCanvas(object sender, PaintEventArgs e)
    {
        var g = e.Graphics;
        g.Clear(_canvas.BackColor);
        if (_battle == null || _battle.World == null)
            return;

        _viewCenter = new PointF(_canvas.Width / 2f, _canvas.Height / 2f + 20);
        _scale = Math.Min(_canvas.Width, _canvas.Height) / 90f;

        DrawGrids(g);
        DrawUnits(g);

        // 飘字（淡出）
        var texts = _battle.World.battleTexts;
        float now = Time.time;
        foreach (var t in texts)
        {
            float life = t.expireTime - now;
            if (life <= 0)
                continue;
            var sp = WorldToScreen(t.worldPos);
            float alpha = Math.Max(0, life);
            int a = (int)(Math.Min(1f, alpha) * 255);
            using (var brush = new SolidBrush(SDColor.FromArgb(a, ToColor(t.color))))
            {
                g.DrawString(t.text, new Font("Arial", 10), brush, sp);
            }
        }
    }

    private void DrawGrids(Graphics g)
    {
        using (var pen = new Pen(SDColor.FromArgb(60, 60, 80), 1))
        {
            float half = 26f * _scale;
            int cells = 6;
            for (int i = 0; i <= cells; i++)
            {
                float off = -half + i * (half * 2 / cells);
                g.DrawLine(pen, _viewCenter.X - half, _viewCenter.Y + off, _viewCenter.X + half, _viewCenter.Y + off);
                g.DrawLine(pen, _viewCenter.X + off, _viewCenter.Y - half, _viewCenter.X + off, _viewCenter.Y + half);
            }
        }
    }

    // 加载英雄头像（Textures/Skins/{Icon}.jpg），带缓存；失败返回 null
    private Image LoadIcon(string name)
    {
        if (string.IsNullOrEmpty(name))
            return null;
        if (_iconCache.TryGetValue(name, out var cached))
            return cached;
        Image img = null;
        string path = Path.Combine(SkinDir, name + ".jpg");
        if (File.Exists(path))
        {
            try { img = Image.FromFile(path); } catch { img = null; }
        }
        _iconCache[name] = img;
        return img;
    }

    private void DrawUnits(Graphics g)
    {
        // 侧1(甲)在上方，侧2(乙)在下方；英雄方形+头像，士兵圆形
        foreach (var chess in _battle.World.chessList)
        {
            if (chess == null || chess.hp <= 0)
                continue;
            var sp = WorldToScreen(chess.transform.position);
            var col = chess.side == 1 ? SDColor.FromArgb(80, 160, 255) : SDColor.FromArgb(255, 120, 90);

            using (var brush = new SolidBrush(col))
            using (var border = new Pen(SDColor.Black, 1))
            {
                if (chess.isHero)
                {
                    float s = 22f;   // 方形边长
                    var rect = new RectangleF(sp.X - s / 2, sp.Y - s / 2, s, s);
                    var img = LoadIcon(chess.chessName);
                    if (img != null)
                        g.DrawImage(img, rect);
                    else
                        g.FillRectangle(brush, rect.X, rect.Y, rect.Width, rect.Height);
                    g.DrawRectangle(border, (int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height);
                }
                else
                {
                    float r = 7f;
                    g.FillEllipse(brush, sp.X - r, sp.Y - r, r * 2, r * 2);
                    g.DrawEllipse(border, sp.X - r, sp.Y - r, r * 2, r * 2);
                }
            }

            // HP 条
            float rate = chess.maxHp > 0 ? (float)chess.hp / chess.maxHp : 0;
            float bw = chess.isHero ? 22f : 14f, bh = 3;
            using (var bg = new SolidBrush(SDColor.FromArgb(60, 60, 60)))
            using (var fg = new SolidBrush(SDColor.FromArgb(80, 220, 120)))
            using (var border = new Pen(SDColor.Black, 1))
            {
                g.FillRectangle(bg, sp.X - bw / 2, sp.Y - 13, bw, bh);
                g.FillRectangle(fg, sp.X - bw / 2, sp.Y - 13, bw * Math.Max(0, rate), bh);
                g.DrawRectangle(border, sp.X - bw / 2, sp.Y - 13, bw, bh);
            }
        }
    }

    private PointF WorldToScreen(Vector3 wpos)
    {
        // 世界 z 向上为屏幕方向（side1 -z 在屏幕上侧）
        float sx = _viewCenter.X + wpos.x * _scale;
        float sy = _viewCenter.Y - wpos.z * _scale;
        return new PointF(sx, sy);
    }

    private static SDColor ToColor(UnityEngine.Color c)
    {
        return SDColor.FromArgb(
            (int)(Math.Min(1f, c.r) * 255),
            (int)(Math.Min(1f, c.g) * 255),
            (int)(Math.Min(1f, c.b) * 255));
    }
}
