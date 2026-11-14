using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace SkillBaseTool
{
    /// <summary>技能编辑面板（弹窗内使用）：模板 / 目标选取 / 特殊效果 分栏勾选，右侧竖列已选模块与价格。</summary>
    public sealed class ScoreView : UserControl
    {
        /// <summary>目标筛选类组件的分类名（单独成栏）。</summary>
        private const string TargetCategory = "目标筛选";

        private readonly DataStore store;
        private readonly SkillRow skill;
        private bool suppress;

        private ComboBox cboTemplate;
        private TextBox txtSearchSelect;
        private TextBox txtSearchEffect;
        private ListBox clbSelect;
        private ListBox clbEffect;
        private ListBox clbDetail;
        private Label lblSkillInfo;
        private Label lblTotal;
        private Label lblParts;
        private Label lblDetailTitle;
        private RichTextBox txtHints;
        /// <summary>已勾选的组件名（ListBox 不支持 owner-draw 勾选状态，勾选状态自行维护）。</summary>
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
            var left = new Panel { Dock = DockStyle.Left, Width = 820, BackColor = Theme.Card, Padding = new Padding(12) };

            var leftTop = new Panel { Dock = DockStyle.Top, Height = 124, BackColor = Theme.Card };
            lblSkillInfo = new Label
            {
                Text = "技能：" + skill.Display + " · " + skill.Type + " · " + skill.ScriptName,
                Font = Theme.Bold,
                ForeColor = Theme.Text,
                AutoSize = false,
                Location = new Point(0, 0),
                Size = new Size(780, 20),
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
                Size = new Size(780, 26),
                DrawMode = DrawMode.OwnerDrawFixed,
                ItemHeight = 22,
            };
            cboTemplate.DrawItem += DrawTemplateItem;
            cboTemplate.SelectedIndexChanged += (s, e) => { if (!suppress) Recalc(); };

            leftTop.Controls.AddRange(new Control[] { lblSkillInfo, lblTpl, cboTemplate });

            var panes = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                BackColor = Theme.Card,
                Padding = new Padding(0),
            };
            panes.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));
            panes.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));
            panes.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
            panes.Controls.Add(BuildPane("目标选取", "目标筛选组件（勾选后写入 Modules）",
                out clbSelect, out txtSearchSelect), 0, 0);
            panes.Controls.Add(BuildPane("特殊效果", "状态 / 特殊机制 / 数值与节奏组件",
                out clbEffect, out txtSearchEffect), 1, 0);

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
                Size = new Size(780, 18),
            };
            leftBottom.Controls.AddRange(new Control[] { lblTotal, lblParts });

            left.Controls.Add(panes);
            left.Controls.Add(leftBottom);
            left.Controls.Add(leftTop);

            var right = new Panel { Dock = DockStyle.Fill, BackColor = Theme.Bg, Padding = new Padding(14) };
            var rightTop = new Panel { Dock = DockStyle.Top, Height = 34, BackColor = Theme.Bg };
            lblDetailTitle = new Label
            {
                Text = "已选模块 / 价格明细",
                Font = Theme.Bold,
                ForeColor = Theme.Text,
                AutoSize = true,
                Location = new Point(0, 6),
            };
            rightTop.Controls.Add(lblDetailTitle);

            clbDetail = new ListBox
            {
                Dock = DockStyle.Top,
                Height = 300,
                BackColor = Theme.RowBg,
                ForeColor = Theme.Text,
                BorderStyle = BorderStyle.None,
                Font = Theme.Base,
                IntegralHeight = false,
                DrawMode = DrawMode.OwnerDrawFixed,
                ItemHeight = 30,
                HorizontalScrollbar = false,
            };
            clbDetail.DrawItem += DrawDetailItem;

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
            right.Controls.Add(clbDetail);
            right.Controls.Add(rightTop);

            Controls.Add(right);
            Controls.Add(left);
        }

        /// <summary>构建单个组件选择栏（标题 + 说明 + 搜索 + 可勾选列表）。</summary>
        private Panel BuildPane(string title, string hint, out ListBox list, out TextBox search)
        {
            var pane = new Panel { Dock = DockStyle.Fill, BackColor = Theme.Card, Padding = new Padding(0, 4, 8, 0) };
            var lblTitle = new Label
            {
                Text = title,
                Dock = DockStyle.Top,
                Height = 22,
                Font = Theme.Bold,
                ForeColor = Theme.Text,
            };
            var lblHint = new Label
            {
                Text = hint,
                Dock = DockStyle.Top,
                Height = 18,
                Font = Theme.Small,
                ForeColor = Theme.Muted,
            };
            var box = new TextBox
            {
                Dock = DockStyle.Top,
                BackColor = Theme.Card2,
                ForeColor = Theme.Text,
                BorderStyle = BorderStyle.FixedSingle,
                Font = Theme.Base,
            };
            var listBox = new ListBox
            {
                Dock = DockStyle.Fill,
                BackColor = Theme.RowBg,
                ForeColor = Theme.Text,
                BorderStyle = BorderStyle.None,
                Font = Theme.Base,
                IntegralHeight = false,
                DrawMode = DrawMode.OwnerDrawFixed,
                ItemHeight = 26,
                HorizontalScrollbar = false,
            };
            listBox.DrawItem += DrawComponentItem;
            listBox.MouseDown += ToggleComponentAt;
            box.TextChanged += (s, e) => ApplyFilter();

            pane.Controls.Add(listBox);
            pane.Controls.Add(box);
            pane.Controls.Add(lblHint);
            pane.Controls.Add(lblTitle);

            list = listBox;
            search = box;
            return pane;
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
            var list = sender as ListBox;
            if (list == null || e.Index < 0)
                return;
            var comp = list.Items[e.Index] as ComponentRow;
            if (comp == null)
                return;
            bool selected = (e.State & DrawItemState.Selected) != 0;
            using (var bg = new SolidBrush(selected ? ColorTranslator.FromHtml("#2B3440") : Theme.RowBg))
                e.Graphics.FillRectangle(bg, e.Bounds);

            bool check = checkedNames.Contains(comp.Name);
            Color color = Theme.QualityColor(comp.Quality);
            string leftText = (check ? "☑ " : "☐ ") + comp.Name;
            var leftRect = new Rectangle(e.Bounds.X + 4, e.Bounds.Y, e.Bounds.Width - 96, e.Bounds.Height);
            var priceRect = new Rectangle(e.Bounds.Right - 88, e.Bounds.Y, 82, e.Bounds.Height);
            TextRenderer.DrawText(e.Graphics, leftText, Theme.Base, leftRect, color,
                TextFormatFlags.VerticalCenter | TextFormatFlags.Left | TextFormatFlags.EndEllipsis);
            DrawPrice(e.Graphics, comp.Score, priceRect, Theme.Bold);
        }

        /// <summary>已选模块明细：名字按品质染色，价格右对齐加粗放大。</summary>
        private void DrawDetailItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0)
                return;
            var comp = clbDetail.Items[e.Index] as ComponentRow;
            if (comp == null)
                return;
            using (var bg = new SolidBrush(e.Index % 2 == 0 ? Theme.RowBg : Theme.RowAlt))
                e.Graphics.FillRectangle(bg, e.Bounds);

            Color color = Theme.QualityColor(comp.Quality);
            var nameRect = new Rectangle(e.Bounds.X + 8, e.Bounds.Y, e.Bounds.Width - 240, e.Bounds.Height);
            var catRect = new Rectangle(nameRect.Right, e.Bounds.Y, 90, e.Bounds.Height);
            var priceRect = new Rectangle(e.Bounds.Right - 134, e.Bounds.Y, 120, e.Bounds.Height);
            TextRenderer.DrawText(e.Graphics, comp.Name, Theme.Bold, nameRect, color,
                TextFormatFlags.VerticalCenter | TextFormatFlags.Left | TextFormatFlags.EndEllipsis);
            TextRenderer.DrawText(e.Graphics, comp.Category, Theme.Small, catRect, Theme.Faint,
                TextFormatFlags.VerticalCenter | TextFormatFlags.Right | TextFormatFlags.EndEllipsis);
            DrawPrice(e.Graphics, comp.Score, priceRect, Theme.Title);
        }

        private static void DrawPrice(Graphics g, double score, Rectangle rect, Font font)
        {
            Color color = score < 0 ? Theme.Ok : Theme.PriceColor(score);
            string text = (score >= 0 ? "+" : "") + score.ToString("0.##");
            TextRenderer.DrawText(g, text, font, rect, color,
                TextFormatFlags.VerticalCenter | TextFormatFlags.Right);
        }

        private void RebuildComponents(IEnumerable<string> presetChecked)
        {
            if (presetChecked != null)
            {
                checkedNames.Clear();
                foreach (string name in presetChecked)
                {
                    if (!string.IsNullOrEmpty(name))
                        checkedNames.Add(name);
                }
            }
            ApplyFilter();
            clbSelect.Invalidate();
            clbEffect.Invalidate();
            Recalc();
        }

        /// <summary>按各自搜索词刷新两栏列表（目标筛选 vs 其余组件）。</summary>
        private void ApplyFilter()
        {
            FillList(clbSelect, txtSearchSelect.Text, c => c.Category == TargetCategory);
            FillList(clbEffect, txtSearchEffect.Text, c => c.Category != TargetCategory);
            clbSelect.Invalidate();
            clbEffect.Invalidate();
        }

        private void FillList(ListBox list, string keyword, Func<ComponentRow, bool> inPane)
        {
            string q = (keyword ?? "").Trim();
            list.BeginUpdate();
            list.Items.Clear();
            foreach (var c in store.Components.OrderBy(x => x.Id))
            {
                if (!inPane(c))
                    continue;
                if (q.Length > 0 && c.Name.IndexOf(q, StringComparison.OrdinalIgnoreCase) < 0
                    && c.Category.IndexOf(q, StringComparison.OrdinalIgnoreCase) < 0)
                    continue;
                list.Items.Add(c);
            }
            list.EndUpdate();
        }

        /// <summary>单击整行切换勾选（两个组件栏共用）。</summary>
        private void ToggleComponentAt(object sender, MouseEventArgs e)
        {
            var list = sender as ListBox;
            if (list == null)
                return;
            int idx = list.IndexFromPoint(e.Location);
            if (idx < 0 || idx >= list.Items.Count)
                return;
            var comp = list.Items[idx] as ComponentRow;
            if (comp == null)
                return;
            if (!checkedNames.Remove(comp.Name))
                checkedNames.Add(comp.Name);
            list.Invalidate();
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

            var picked = store.Components.Where(c => checkedNames.Contains(c.Name)).OrderBy(x => x.Id).ToList();
            lblDetailTitle.Text = string.Format("已选模块 {0} 个 · 组件价 {1:0.##} · 总价 {2:0.##}",
                picked.Count, result.ComponentScore, result.Total);
            lblDetailTitle.ForeColor = Theme.PriceColor(result.Total);

            clbDetail.BeginUpdate();
            clbDetail.Items.Clear();
            foreach (var comp in picked)
                clbDetail.Items.Add(comp);
            clbDetail.EndUpdate();
            clbDetail.Invalidate();

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
