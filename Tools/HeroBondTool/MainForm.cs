using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace HeroBondTool
{
    /// <summary>主窗体：每个剧本两个 Tab（统计 / 羁绊），顶部可重新加载与保存。</summary>
    public sealed class MainForm : Form
    {
        private static readonly Color TopBg = ColorTranslator.FromHtml("#181C22");

        private readonly Label lblPath;
        private readonly Label lblStatus;
        private readonly Button btnReload;
        private readonly Button btnSave;
        private readonly TabControl scenarioTabs;
        private readonly Dictionary<int, AnalyticsView> analyticsViews = new Dictionary<int, AnalyticsView>();
        private readonly Dictionary<int, BondBoardView> boardViews = new Dictionary<int, BondBoardView>();
        private readonly Dictionary<int, TabPage> scenarioPages = new Dictionary<int, TabPage>();

        private DataStore store;

        public MainForm()
        {
            Text = "剧本羁绊工具 - 统计与编辑";
            Size = new Size(1500, 950);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Theme.Bg;
            ForeColor = Theme.Text;
            Font = Theme.Base;

            var top = new Panel();
            top.Dock = DockStyle.Top;
            top.Height = 66;
            top.BackColor = TopBg;

            var title = new Label();
            title.AutoSize = true;
            title.Text = "剧本羁绊 · 统计与编辑";
            title.Font = new Font("Microsoft YaHei UI", 14f, FontStyle.Bold);
            title.ForeColor = Theme.Accent;
            title.Location = new Point(18, 10);
            top.Controls.Add(title);

            lblPath = new Label();
            lblPath.AutoSize = false;
            lblPath.Text = "";
            lblPath.ForeColor = Theme.Faint;
            lblPath.Font = Theme.Small;
            lblPath.Location = new Point(20, 40);
            lblPath.Size = new Size(900, 18);
            top.Controls.Add(lblPath);

            var rightBar = new FlowLayoutPanel();
            rightBar.Dock = DockStyle.Right;
            rightBar.Width = 520;
            rightBar.FlowDirection = FlowDirection.RightToLeft;
            rightBar.WrapContents = false;
            rightBar.Padding = new Padding(0, 16, 10, 0);
            rightBar.BackColor = TopBg;

            btnSave = new Button();
            btnSave.Text = "保存修改";
            btnSave.Size = new Size(104, 32);
            btnSave.FlatStyle = FlatStyle.Flat;
            btnSave.FlatAppearance.BorderColor = Theme.Accent;
            btnSave.BackColor = ColorTranslator.FromHtml("#3A3323");
            btnSave.ForeColor = ColorTranslator.FromHtml("#FFDD9A");
            btnSave.Margin = new Padding(8, 0, 0, 0);
            btnSave.Click += (s, e) => SaveChanges();
            rightBar.Controls.Add(btnSave);

            btnReload = new Button();
            btnReload.Text = "重新加载";
            btnReload.Size = new Size(96, 32);
            btnReload.FlatStyle = FlatStyle.Flat;
            btnReload.FlatAppearance.BorderColor = Theme.Line;
            btnReload.BackColor = Theme.Card;
            btnReload.ForeColor = Theme.Muted;
            btnReload.Margin = new Padding(8, 0, 0, 0);
            btnReload.Click += (s, e) => ReloadAll(true);
            rightBar.Controls.Add(btnReload);

            lblStatus = new Label();
            lblStatus.AutoSize = false;
            lblStatus.Text = "";
            lblStatus.ForeColor = Theme.Muted;
            lblStatus.Font = Theme.Small;
            lblStatus.TextAlign = ContentAlignment.MiddleRight;
            lblStatus.Size = new Size(280, 32);
            lblStatus.Margin = new Padding(8, 0, 0, 0);
            rightBar.Controls.Add(lblStatus);

            top.Controls.Add(rightBar);

            scenarioTabs = new TabControl();
            scenarioTabs.Dock = DockStyle.Fill;
            scenarioTabs.Font = Theme.Base;

            Controls.Add(scenarioTabs);
            Controls.Add(top);

            ReloadAll(false);
        }

        private void ReloadAll(bool confirmDiscard)
        {
            if (confirmDiscard && store != null && store.IsDirty)
            {
                if (MessageBox.Show(this, "当前有未保存的修改，重新加载会丢弃它们，是否继续？",
                        "确认", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                    return;
            }

            try
            {
                string root = ConfigIO.LocateRoot();
                if (string.IsNullOrEmpty(root))
                {
                    MessageBox.Show(this, "未找到仓库根目录（需包含 Assets/Resources/Scripts/Configs）。",
                        "加载失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                store = ConfigIO.Load(root);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "加载配置失败：" + ex.Message, "加载失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            lblPath.Text = store.HeroPath;
            BuildTabs();
            UpdateStatus();
        }

        private void BuildTabs()
        {
            scenarioTabs.SuspendLayout();
            foreach (TabPage page in scenarioTabs.TabPages)
                page.Dispose();
            scenarioTabs.TabPages.Clear();
            analyticsViews.Clear();
            boardViews.Clear();
            scenarioPages.Clear();

            foreach (ScenarioInfo scenario in store.Scenarios)
            {
                var page = new TabPage();
                page.Text = scenario.Name;
                page.BackColor = Theme.Bg;
                page.ForeColor = Theme.Text;

                var inner = new TabControl();
                inner.Dock = DockStyle.Fill;
                inner.Font = Theme.Base;

                var statsPage = new TabPage("统计");
                statsPage.BackColor = Theme.Bg;
                statsPage.ForeColor = Theme.Text;

                var analytics = new AnalyticsView(store);
                analytics.Dock = DockStyle.Fill;
                statsPage.Controls.Add(analytics);
                analyticsViews[scenario.Id] = analytics;

                var bondPage = new TabPage("羁绊");
                bondPage.BackColor = Theme.Bg;
                bondPage.ForeColor = Theme.Text;

                int scenarioId = scenario.Id;
                var board = new BondBoardView(store, scenarioId, Analysis.Build(store, scenarioId));
                board.Dock = DockStyle.Fill;
                board.GroupsChanged += () => BeginInvoke(new Action(() =>
                {
                    UpdateStatus();
                    RefreshViews(false);
                }));
                board.TypeChanged += () =>
                {
                    UpdateStatus();
                };
                bondPage.Controls.Add(board);
                boardViews[scenarioId] = board;

                inner.TabPages.Add(statsPage);
                inner.TabPages.Add(bondPage);
                page.Controls.Add(inner);
                scenarioTabs.TabPages.Add(page);
                scenarioPages[scenario.Id] = page;
            }

            scenarioTabs.ResumeLayout();
            RefreshViews(true);
        }

        private void RefreshViews(bool rebuildTabs)
        {
            if (store == null)
                return;

            if (rebuildTabs)
            {
                foreach (KeyValuePair<int, TabPage> kv in scenarioPages)
                {
                    ScenarioInfo scenario = store.Scenarios.Find(x => x.Id == kv.Key);
                    if (scenario != null)
                        kv.Value.Text = scenario.Name;
                }
            }

            foreach (ScenarioInfo scenario in store.Scenarios)
            {
                ScenarioAnalysis analysis = Analysis.Build(store, scenario.Id);
                if (rebuildTabs)
                {
                    TabPage page = scenarioPages[scenario.Id];
                    if (page != null)
                        page.Text = string.Format("{0}（{1}人）", scenario.Name, analysis.Total);
                }
                if (analyticsViews.ContainsKey(scenario.Id))
                    analyticsViews[scenario.Id].SetData(analysis);
                if (boardViews.ContainsKey(scenario.Id))
                    boardViews[scenario.Id].SetData(analysis);
            }
            UpdateStatus();
        }

        private void UpdateStatus()
        {
            if (store == null)
                return;
            if (store.IsDirty)
            {
                lblStatus.ForeColor = Theme.Accent;
                lblStatus.Text = string.Format("未保存：{0} 个羁绊组已修改", store.DirtyGroupIds.Count);
            }
            else
            {
                lblStatus.ForeColor = Theme.Muted;
                lblStatus.Text = "已保存";
            }
        }

        private void SaveChanges()
        {
            if (store == null)
                return;
            if (!store.IsDirty)
            {
                MessageBox.Show(this, "没有需要保存的修改。", "保存", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int count = store.DirtyGroupIds.Count;
            if (MessageBox.Show(this,
                    string.Format("将把 {0} 个羁绊组的修改写入：\n{1}\n\n是否继续？", count, store.FriendPath),
                    "保存修改", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                return;

            try
            {
                List<int> ids = new List<int>(store.DirtyGroupIds);
                int written = ConfigIO.SaveGroups(store.FriendPath, store, ids);
                store = ConfigIO.Load(store.RootDir);
                BuildTabs();
                UpdateStatus();
                MessageBox.Show(this, string.Format("已写入 {0} 个羁绊组。", written),
                    "保存成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "写入失败：" + ex.Message, "保存失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
