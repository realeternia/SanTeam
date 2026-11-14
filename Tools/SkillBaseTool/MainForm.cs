using System;
using System.Drawing;
using System.Windows.Forms;

namespace SkillBaseTool
{
    public sealed class MainForm : Form
    {
        private DataStore store;
        private Label lblPath;
        private Label lblStatus;
        private GenerateView generateView;
        private CompareView compareView;

        public MainForm()
        {
            Text = "技能模板 / SkillBase 工具";
            StartPosition = FormStartPosition.CenterScreen;
            Size = new Size(1500, 960);
            MinimumSize = new Size(1200, 760);
            BackColor = Theme.Bg;
            ForeColor = Theme.Text;
            Font = Theme.Base;

            BuildUi();
            Reload();
        }

        private void BuildUi()
        {
            var top = new Panel { Dock = DockStyle.Top, Height = 58, BackColor = Theme.Card, Padding = new Padding(16, 10, 16, 8) };
            var title = new Label
            {
                Text = "技能模板工具",
                Font = Theme.TitleBig,
                ForeColor = Theme.Text,
                AutoSize = true,
                Location = new Point(16, 14),
            };
            lblPath = new Label
            {
                Text = "",
                Font = Theme.Small,
                ForeColor = Theme.Faint,
                AutoSize = false,
                Location = new Point(160, 20),
                Size = new Size(1050, 20),
            };
            var btnReload = new Button
            {
                Text = "重新加载配置",
                FlatStyle = FlatStyle.Flat,
                BackColor = Theme.Card2,
                ForeColor = Theme.Text,
                Size = new Size(120, 30),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Location = new Point(top.Width - 136, 14),
            };
            btnReload.FlatAppearance.BorderColor = Theme.Line;
            btnReload.Click += (s, e) => Reload();
            top.Controls.AddRange(new Control[] { title, lblPath, btnReload });
            top.Resize += (s, e) =>
            {
                btnReload.Left = top.Width - btnReload.Width - 16;
                lblPath.Width = Math.Max(120, btnReload.Left - lblPath.Left - 12);
            };

            var tabs = new TabControl { Dock = DockStyle.Fill, Font = Theme.Bold };
            generateView = new GenerateView(Reload) { Dock = DockStyle.Fill };
            compareView = new CompareView() { Dock = DockStyle.Fill };

            var pageGen = new TabPage("① 模板化 / 生成 SkillBase") { BackColor = Theme.Bg };
            var pageCompare = new TabPage("② 技能 vs 模板 对比") { BackColor = Theme.Bg };
            pageGen.Controls.Add(generateView);
            pageCompare.Controls.Add(compareView);
            tabs.TabPages.AddRange(new[] { pageGen, pageCompare });
            tabs.SelectedIndexChanged += (s, e) =>
            {
                if (tabs.SelectedIndex == 1)
                    compareView.EnsureBuilt();
            };

            var status = new Panel { Dock = DockStyle.Bottom, Height = 26, BackColor = Theme.Card, Padding = new Padding(16, 4, 16, 0) };
            lblStatus = new Label { Dock = DockStyle.Fill, Text = "", Font = Theme.Small, ForeColor = Theme.Muted };

            Controls.Add(tabs);
            Controls.Add(status);
            Controls.Add(top);
            status.Controls.Add(lblStatus);
        }

        private void Reload()
        {
            try
            {
                string root = ConfigIO.FindRoot();
                if (root == null)
                {
                    MessageBox.Show(this, "未找到 Assets/Resources/Scripts/Configs，请把工具放在工程 Tools 目录下运行。",
                        "读取失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                store = ConfigIO.Load(root);
                lblPath.Text = "工程：" + root;
                generateView.Bind(store);
                compareView.Bind(store);
                lblStatus.Text = string.Format("已加载 技能 {0} 行 / 模板 {1} 条 / 组件 {2} 条 / SkillBase {3} 行 / 英雄品质 {4} 条",
                    store.Skills.Count, store.Templates.Count, store.Components.Count, store.BaseRows.Count, store.HeroQuality.Count);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "读取配置失败：" + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
