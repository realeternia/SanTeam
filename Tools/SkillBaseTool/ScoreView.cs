using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace SkillBaseTool
{
    /// <summary>技能编辑面板（弹窗内使用）：模板 / 组件按品质染色并显示价格，实时算总价与提示。</summary>
    public sealed class ScoreView : UserControl
    {
        private readonly DataStore store;
        private readonly SkillRow skill;
        private bool suppress;

        private ComboBox cboTemplate;
        private TextBox txtSearch;
        private ListBox clbModules;
        private Label lblSkillInfo;
        private Label lblTotal;
        private Label lblParts;
        private RichTextBox txtHints;
        /// <summary>已勾选的组件名（ListBox 不支持 owner-draw，勾选状态自行维护）。</summary>
        private readonly HashSet<string> checkedNames = new HashSet<string>();

        public ScoreView(DataStore data, SkillRow target)
        {
            store = data;
            skill = target;
            BackColor = Theme.Bg;
            BuildUi();
            FillTemplates();
            RebuildComponents(skill.Modules);
        }

        private void BuildUi()
        {
            var left = new Panel { Dock = DockStyle.Left, Width = 470, BackColor = Theme.Card, Padding = new Padding(12) };

            var leftTop = new Panel { Dock = DockStyle.Top, Height = 186, BackColor = Theme.Card };
            lblSkillInfo = new Label
            {
                Text = "技能：" + skill.Display + " · " + skill.Type + " · " + skill.ScriptName,
                Font = Theme.Bold,
                ForeColor = Theme.Text,
                AutoSize = false,
                Location = new Point(0, 0),
                Size = new Size(440, 20),
            };

            var lblTpl = new Label
            {
                Text = "技能模板（颜色=品质，右侧=价格）",
                Font = Theme.Small,
                ForeColor = Theme.Muted,
                Location = new Point(0, 28),
                AutoSize = true,
            };
            cboTemplate = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                FlatStyle = FlatStyle.Flat,
                BackColor = Theme.Card2,
                ForeColor = Theme.Text,
                Font = Theme.Base,
                Location = new Point(0, 48),
                Size = new Size(440, 26),
                DrawMode = DrawMode.OwnerDrawFixed,
                ItemHeight = 22,
            };
            cboTemplate.DrawItem += DrawTemplateItem;
            cboTemplate.SelectedIndexChanged += (s, e) => { if (!suppress) Recalc(); };

            var lblComp = new Label
            {
                Text = "组件（点击行切换勾选；颜色=品质，右侧=价格）",
                Font = Theme.Small,
                ForeColor = Theme.Muted,
                Location = new Point(0, 82),
                AutoSize = true,
            };
            txtSearch = new TextBox
            {
                BackColor = Theme.Card2,
                ForeColor = Theme.Text,
                BorderStyle = BorderStyle.FixedSingle,
                Font = Theme.Base,
                Location = new Point(0, 102),
                Size = new Size(440, 24),
            };
            txtSearch.TextChanged += (s, e) => RebuildComponents(null);

            leftTop.Controls.AddRange(new Control[] { lblSkillInfo, lblTpl, cboTemplate, lblComp, txtSearch });

            var leftBottom = new Panel { Dock = DockStyle.Bottom, Height = 70, BackColor = Theme.Card };
            lblTotal = new Label
            {
                Text = "0",
                Font = Theme.Num,
                ForeColor = Theme.Accent,
                Location = new Point(0, 2),
                Size = new Size(200, 40),
            };
            lblParts = new Label
            {
                Text = "",
                Font = Theme.Small,
                ForeColor = Theme.Muted,
                Location = new Point(0, 44),
                Size = new Size(440, 18),
            };
            leftBottom.Controls.AddRange(new Control[] { lblTotal, lblParts });

            clbModules = new ListBox
            {
                Dock = DockStyle.Fill,
                BackColor = Theme.RowBg,
                ForeColor = Theme.Text,
                BorderStyle = BorderStyle.None,
                Font = Theme.Base,
                IntegralHeight = false,
                DrawMode = DrawMode.OwnerDrawFixed,
                ItemHeight = 24,
            };
            clbModules.DrawItem += DrawComponentItem;
            // 单击整行切换勾选（绘制与状态均由本控件自己维护）
            clbModules.MouseDown += (s, e) =>
            {
                int idx = clbModules.IndexFromPoint(e.Location);
                if (idx < 0 || idx >= clbModules.Items.Count)
                    return;
                var comp = clbModules.Items[idx] as ComponentRow;
                if (comp == null)
                    return;
                if (!checkedNames.Remove(comp.Name))
                    checkedNames.Add(comp.Name);
                clbModules.Invalidate();
                Recalc();
            };

            left.Controls.Add(clbModules);
            left.Controls.Add(leftBottom);
            left.Controls.Add(leftTop);

            var right = new Panel { Dock = DockStyle.Fill, BackColor = Theme.Bg, Padding = new Padding(14) };
            var rightTop = new Panel { Dock = DockStyle.Top, Height = 34, BackColor = Theme.Bg };
            var rightTitle = new Label
            {
                Text = "价格明细 / 提示",
                Font = Theme.Bold,
                ForeColor = Theme.Text,
                AutoSize = true,
                Location = new Point(0, 6),
            };
            rightTop.Controls.Add(rightTitle);

            txtHints = new RichTextBox
            {
                Dock = DockStyle.Fill,
                BackColor = Theme.RowBg,
                ForeColor = Theme.Text,
                BorderStyle = BorderStyle.None,
                Font = Theme.Base,
                ReadOnly = true,
                DetectUrls = false,
                ScrollBars = RichTextBoxScrollBars.Vertical,
            };
            right.Controls.Add(txtHints);
            right.Controls.Add(rightTop);

            Controls.Add(right);
            Controls.Add(left);
        }

        private void FillTemplates()
        {
            suppress = true;
            cboTemplate.Items.Clear();
            cboTemplate.Items.Add("(无模板)");
            int selected = 0;
            foreach (var t in store.Templates.OrderBy(x => x.Id))
            {
                int idx = cboTemplate.Items.Add(t);
                if (t.Name == skill.TemplateName)
                    selected = idx;
            }
            cboTemplate.SelectedIndex = selected;
            suppress = false;
        }

        private void DrawTemplateItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0)
                return;
            bool selected = (e.State & DrawItemState.Selected) != 0;
            using (var bg = new SolidBrush(selected ? ColorTranslator.FromHtml("#2B3440") : Theme.Card2))
                e.Graphics.FillRectangle(bg, e.Bounds);

            var tpl = cboTemplate.Items[e.Index] as TemplateRow;
            if (tpl == null)
            {
                TextRenderer.DrawText(e.Graphics, "(无模板)", Theme.Base, e.Bounds, Theme.Muted,
                    TextFormatFlags.VerticalCenter | TextFormatFlags.Left);
                return;
            }
            Color color = Theme.QualityColor(tpl.Quality);
            var leftRect = new Rectangle(e.Bounds.X + 4, e.Bounds.Y, e.Bounds.Width - 170, e.Bounds.Height);
            var rightRect = new Rectangle(e.Bounds.Right - 164, e.Bounds.Y, 160, e.Bounds.Height);
            TextRenderer.DrawText(e.Graphics, tpl.Name, Theme.Base, leftRect, color,
                TextFormatFlags.VerticalCenter | TextFormatFlags.Left | TextFormatFlags.EndEllipsis);
            TextRenderer.DrawText(e.Graphics, "Q" + tpl.Quality + "  价格 " + tpl.Score.ToString("0.##"),
                Theme.Small, rightRect, color, TextFormatFlags.VerticalCenter | TextFormatFlags.Right);
        }

        private void DrawComponentItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0)
                return;
            var comp = clbModules.Items[e.Index] as ComponentRow;
            if (comp == null)
                return;
            bool selected = (e.State & DrawItemState.Selected) != 0;
            using (var bg = new SolidBrush(selected ? ColorTranslator.FromHtml("#2B3440") : Theme.RowBg))
                e.Graphics.FillRectangle(bg, e.Bounds);

            bool check = checkedNames.Contains(comp.Name);
            Color color = Theme.QualityColor(comp.Quality);
            string leftText = (check ? "☑ " : "☐ ") + comp.Name;
            var leftRect = new Rectangle(e.Bounds.X + 4, e.Bounds.Y, e.Bounds.Width - 180, e.Bounds.Height);
            var rightRect = new Rectangle(e.Bounds.Right - 174, e.Bounds.Y, 170, e.Bounds.Height);
            TextRenderer.DrawText(e.Graphics, leftText, Theme.Base, leftRect, color,
                TextFormatFlags.VerticalCenter | TextFormatFlags.Left | TextFormatFlags.EndEllipsis);
            TextRenderer.DrawText(e.Graphics, "Q" + comp.Quality + "  价格 " + comp.Score.ToString("0.##"),
                Theme.Small, rightRect, color, TextFormatFlags.VerticalCenter | TextFormatFlags.Right);
        }

        private void RebuildComponents(IEnumerable<string> presetChecked)
        {
            if (presetChecked != null)
            {
                checkedNames.Clear();
                foreach (string name in presetChecked)
                    checkedNames.Add(name);
            }
            suppress = true;
            clbModules.Items.Clear();
            string q = txtSearch.Text.Trim();
            foreach (var c in store.Components.OrderBy(x => x.Id))
            {
                if (q.Length > 0 && c.Name.IndexOf(q, StringComparison.OrdinalIgnoreCase) < 0
                    && c.Category.IndexOf(q, StringComparison.OrdinalIgnoreCase) < 0)
                    continue;
                clbModules.Items.Add(c);
            }
            suppress = false;
            clbModules.Invalidate();
            Recalc();
        }

        private List<string> CheckedNames()
        {
            return store.Components.Where(c => checkedNames.Contains(c.Name)).Select(c => c.Name).ToList();
        }

        private string SelectedTemplate()
        {
            var tpl = cboTemplate.SelectedItem as TemplateRow;
            return tpl == null ? null : tpl.Name;
        }

        private void Recalc()
        {
            var result = SkillGen.Evaluate(store, skill, SelectedTemplate(), CheckedNames());
            lblTotal.Text = result.Total.ToString("0.##");
            lblTotal.ForeColor = Theme.PriceColor(result.Total);
            lblParts.Text = string.Format("模板 {0:0.##}  +  组件 {1:0.##}（>10 黄 / >13 红）",
                result.TemplateScore, result.ComponentScore);

            txtHints.Clear();
            foreach (string line in result.Hints)
            {
                txtHints.SelectionStart = txtHints.TextLength;
                txtHints.SelectionColor = line.Contains("⚠") ? Theme.Warn : Theme.Text;
                txtHints.AppendText(line + Environment.NewLine);
            }
        }

        /// <summary>写回当前技能的 Template / Modules（同 Sname 五级一起），返回写入行数。</summary>
        public int ApplyToSkill()
        {
            string tpl = SelectedTemplate() ?? "";
            var mods = CheckedNames();
            var rows = store.Skills.Where(s => s.Sname == skill.Sname).ToList();
            foreach (var row in rows)
            {
                row.TemplateName = tpl;
                row.Modules = mods.ToArray();
            }
            ConfigIO.SaveSkillRows(store, rows.Select(r => r.Id));
            return rows.Count;
        }
    }
}
