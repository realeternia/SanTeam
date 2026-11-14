using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace SkillBaseTool
{
    /// <summary>① 自动分配模板 → 生成 SkillBaseConfig → 配置自检。</summary>
    public sealed class GenerateView : UserControl
    {
        private DataStore store;
        private readonly Action reload;
        private Label lblStats;
        private ComboBox cboJob;
        private DataGridView grid;
        /// <summary>行号 → 技能（Lv1 行），供编辑按钮回查。</summary>
        private readonly List<SkillRow> rowSkills = new List<SkillRow>();
        private int editColumn = -1;
        private int templateColumn = -1;
        private int moduleColumn = -1;
        /// <summary>重建职业下拉时不触发刷新。</summary>
        private bool suppressJob;

        public GenerateView(Action reload)
        {
            this.reload = reload;
            BackColor = Theme.Bg;
            BuildUi();
        }

        private void BuildUi()
        {
            var top = new Panel { Dock = DockStyle.Top, Height = 92, BackColor = Theme.Card, Padding = new Padding(16, 10, 16, 10) };
            lblStats = new Label
            {
                Text = "",
                Font = Theme.Base,
                ForeColor = Theme.Muted,
                AutoSize = true,
                Location = new Point(16, 12),
            };

            var btnAuto = MakeButton("① 自动分配模板", 16, 160);
            btnAuto.Click += (s, e) => AutoAssign();
            var btnGen = MakeButton("② 生成 SkillBase", 186, 170);
            btnGen.Click += (s, e) => GenerateBase();
            var btnCheck = MakeButton("③ 配置自检", 366, 120);
            btnCheck.Click += (s, e) => ValidateAll();
            var lblJob = new Label
            {
                Text = "职业",
                Font = Theme.Small,
                ForeColor = Theme.Muted,
                AutoSize = true,
                Location = new Point(500, 51),
            };
            cboJob = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                FlatStyle = FlatStyle.Flat,
                BackColor = Theme.Card2,
                ForeColor = Theme.Text,
                Font = Theme.Base,
                Location = new Point(534, 44),
                Size = new Size(96, 26),
            };
            cboJob.SelectedIndexChanged += (s, e) => { if (!suppressJob) RefreshGrid(); };
            var tip = new Label
            {
                Text = "规则：DamageStrength/HealStrength 取模板 Values，CD/MpCost 取模板（MpCost×10），Range/Area 仅模板>0 时覆盖，Buff 只按技能自己的 Modules 追加（模板默认组件仅估值）",
                Font = Theme.Small,
                ForeColor = Theme.Faint,
                AutoSize = true,
                Location = new Point(500, 18),
            };

            top.Controls.AddRange(new Control[] { lblStats, btnAuto, btnGen, btnCheck, lblJob, cboJob, tip });

            grid = new DataGridView { Dock = DockStyle.Fill, Margin = new Padding(0) };
            Theme.StyleGrid(grid);
            // 只读整表会吞掉按钮列的点击，故放开后逐列设为只读
            grid.ReadOnly = false;
            grid.Columns.Add(MakeColumn("技能", 150));
            grid.Columns.Add(MakeColumn("职业", 44));
            grid.Columns.Add(MakeColumn("品质", 48));
            grid.Columns.Add(MakeColumn("价格", 58));
            grid.Columns.Add(MakeColumn("类型", 55));
            grid.Columns.Add(MakeColumn("脚本", 165));
            grid.Columns.Add(MakeColumn("模板", 110));
            templateColumn = grid.Columns.Count - 1;
            grid.Columns.Add(MakeColumn("组件", 240));
            moduleColumn = grid.Columns.Count - 1;
            grid.Columns.Add(MakeColumn("状态", 90));

            var btnCol = new DataGridViewButtonColumn
            {
                HeaderText = "操作",
                Text = "编辑",
                UseColumnTextForButtonValue = true,
                FillWeight = 60,
                SortMode = DataGridViewColumnSortMode.NotSortable,
                ReadOnly = false,
                FlatStyle = FlatStyle.Flat,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleCenter,
                    BackColor = Theme.Card2,
                    ForeColor = Theme.Accent,
                    SelectionBackColor = Theme.Card2,
                    SelectionForeColor = Theme.Accent,
                },
            };
            grid.Columns.Add(btnCol);
            editColumn = grid.Columns.Count - 1;

            grid.CellContentClick += (s, e) =>
            {
                if (e.ColumnIndex != editColumn || e.RowIndex < 0 || e.RowIndex >= rowSkills.Count)
                    return;
                EditSkill(rowSkills[e.RowIndex]);
            };
            // 模板 / 组件列：单元格内多色文本（名字=品质色，费用=金黄色）
            grid.CellPainting += OnCellPainting;

            Controls.Add(grid);
            Controls.Add(top);
        }

        private static Button MakeButton(string text, int x, int width)
        {
            var b = new Button
            {
                Text = text,
                FlatStyle = FlatStyle.Flat,
                BackColor = Theme.Card2,
                ForeColor = Theme.Text,
                Size = new Size(width, 32),
                Location = new Point(x, 44),
            };
            b.FlatAppearance.BorderColor = Theme.Line;
            return b;
        }

        private static DataGridViewTextBoxColumn MakeColumn(string header, int weight)
        {
            return new DataGridViewTextBoxColumn
            {
                HeaderText = header,
                FillWeight = weight,
                ReadOnly = true,
                SortMode = DataGridViewColumnSortMode.NotSortable,
                DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleLeft },
            };
        }

        /// <summary>模板 / 组件列的富文本绘制：名字按品质染色，括号内费用金黄色。</summary>
        private void OnCellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= rowSkills.Count)
                return;
            if (e.ColumnIndex != templateColumn && e.ColumnIndex != moduleColumn)
                return;

            e.PaintBackground(e.ClipBounds, true);
            var skill = rowSkills[e.RowIndex];
            var segments = e.ColumnIndex == templateColumn
                ? TemplateSegments(skill.TemplateName)
                : ModuleSegments(skill.Modules);
            var rect = new Rectangle(e.CellBounds.X + 4, e.CellBounds.Y,
                Math.Max(0, e.CellBounds.Width - 8), e.CellBounds.Height);
            Theme.DrawSegments(e.Graphics, rect, segments, Theme.Base);
            e.Handled = true;
        }

        private List<Theme.TextSegment> TemplateSegments(string name)
        {
            var segments = new List<Theme.TextSegment>();
            if (string.IsNullOrEmpty(name))
                return segments;
            TemplateRow tpl;
            if (!store.TemplateByName.TryGetValue(name, out tpl))
            {
                segments.Add(new Theme.TextSegment(name + "（缺失）", Theme.Danger));
                return segments;
            }
            segments.Add(new Theme.TextSegment(name, Theme.QualityColor(tpl.Quality)));
            segments.Add(new Theme.TextSegment("（" + PriceText(tpl.Score) + "）", Theme.Accent));
            return segments;
        }

        private List<Theme.TextSegment> ModuleSegments(string[] modules)
        {
            var segments = new List<Theme.TextSegment>();
            if (modules == null)
                return segments;
            foreach (string name in modules)
            {
                if (string.IsNullOrEmpty(name))
                    continue;
                if (segments.Count > 0)
                    segments.Add(new Theme.TextSegment("、", Theme.Faint));
                ComponentRow comp;
                if (!store.ComponentByName.TryGetValue(name, out comp))
                {
                    segments.Add(new Theme.TextSegment(name + "（缺失）", Theme.Danger));
                    continue;
                }
                segments.Add(new Theme.TextSegment(name, Theme.QualityColor(comp.Quality)));
                segments.Add(new Theme.TextSegment("（" + PriceText(comp.Score) + "）", Theme.Accent));
            }
            return segments;
        }

        private static string PriceText(double score)
        {
            return (score >= 0 ? "+" : "") + score.ToString("0.##");
        }

        public void Bind(DataStore data)
        {
            store = data;
            FillJobFilter();
            RefreshGrid();
        }

        /// <summary>重建职业下拉（保留当前选中项），按技能数从多到少排列。</summary>
        private void FillJobFilter()
        {
            string current = cboJob.SelectedItem as string;
            suppressJob = true;
            cboJob.BeginUpdate();
            cboJob.Items.Clear();
            cboJob.Items.Add("全部");
            if (store != null)
            {
                foreach (var g in store.QualityHeroSkills
                    .Where(s => !string.IsNullOrEmpty(s.Job))
                    .GroupBy(s => s.Job)
                    .OrderByDescending(g => g.Count())
                    .ThenBy(g => g.Key, StringComparer.CurrentCulture))
                    cboJob.Items.Add(g.Key);
            }
            cboJob.EndUpdate();
            int idx = string.IsNullOrEmpty(current) ? 0 : cboJob.Items.IndexOf(current);
            cboJob.SelectedIndex = idx < 0 ? 0 : idx;
            suppressJob = false;
        }

        private void RefreshGrid()
        {
            if (store == null)
                return;
            Theme.BeginBulkFill(grid);
            rowSkills.Clear();
            string job = cboJob.SelectedItem as string ?? "全部";
            var skills = store.QualityHeroSkills
                .Where(s => job == "全部" || s.Job == job)
                .OrderBy(s => s.Quality)
                .ThenBy(s => s.Sname)
                .ToList();
            int templated = 0;
            foreach (var s in skills)
            {
                string status;
                if (string.IsNullOrEmpty(s.TemplateName))
                    status = "未模板化";
                else if (!store.TemplateByName.ContainsKey(s.TemplateName))
                    status = "模板缺失";
                else if (!store.BaseRows.ContainsKey(s.Id))
                    status = "缺 SkillBase";
                else
                {
                    status = "OK";
                    templated++;
                }
                double price = SkillGen.CalcPrice(store, s);
                int idx = grid.Rows.Add(s.Display, s.Job, "Q" + s.Quality, price.ToString("0.##"), s.Type, s.ScriptName,
                    s.TemplateName, string.Join("、", s.Modules), status, "编辑");
                rowSkills.Add(s);

                // 品质列按品质染色
                grid.Rows[idx].Cells[2].Style.ForeColor = Theme.QualityColor(s.Quality);
                // 价格列：>13 红 / >10 黄 / 其余常规
                grid.Rows[idx].Cells[3].Style.ForeColor = Theme.PriceColor(price);

                var cell = grid.Rows[idx].Cells[8];
                cell.Style.ForeColor = status == "OK" ? Theme.Ok
                    : (status == "未模板化" ? Theme.Muted : Theme.Danger);
                if (status != "OK")
                    grid.Rows[idx].DefaultCellStyle.BackColor = Theme.RowWarn;
            }
            Theme.EndBulkFill(grid);
            lblStats.Text = string.Format("{0}品质 1~4 英雄技能 {1} 个：已模板化 {2}，SkillBase 行 {3}",
                job == "全部" ? "" : "职业 " + job + " · ", skills.Count, templated, store.BaseRows.Count);
        }

        /// <summary>打开编辑弹窗；确认后已写回 SkillConfig，重新加载刷新列表。</summary>
        private void EditSkill(SkillRow skill)
        {
            using (var dlg = new SkillEditDialog(store, skill))
            {
                if (dlg.ShowDialog(FindForm()) == DialogResult.OK)
                    reload();
            }
        }

        private void AutoAssign()
        {
            if (store == null)
                return;
            if (MessageBox.Show(this, "将按技能脚本/类型为品质 1~4 英雄技能自动写入 Template 与 Modules（同 Sname 五级一起），并保存 SkillConfig。继续？",
                    "自动分配模板", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) != DialogResult.OK)
                return;
            int groups;
            List<string> skipped;
            var ids = SkillGen.AutoAssign(store, out groups, out skipped);
            if (ids.Count == 0)
            {
                MessageBox.Show(this, "没有可模板化的技能。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            ConfigIO.SaveSkillRows(store, ids);
            string msg = string.Format("已为 {0} 个技能写入模板，共 {1} 行。", groups, ids.Count);
            if (skipped.Count > 0)
                msg += "\n未模板化（特化）：" + string.Join("、", skipped);
            MessageBox.Show(this, msg, "完成", MessageBoxButtons.OK, MessageBoxIcon.Information);
            reload();
        }

        private void GenerateBase()
        {
            if (store == null)
                return;
            var gen = SkillGen.GenerateBase(store);
            if (gen.Rows.Count == 0)
            {
                MessageBox.Show(this, "没有已填模板的技能，请先执行 ① 自动分配模板。", "提示",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (MessageBox.Show(this, string.Format("将用 {0} 个模板化技能展开 {1} 行，整表覆盖 SkillBaseConfig。继续？",
                    gen.TemplatedSkills, gen.Rows.Count), "生成 SkillBase", MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Question) != DialogResult.OK)
                return;
            int n = ConfigIO.WriteBaseRows(store, gen.Rows);
            string msg = string.Format("已写入 {0} 行 SkillBaseConfig。", n);
            if (gen.Warnings.Count > 0)
                msg += "\n警告：" + string.Join("\n", gen.Warnings.Take(10));
            MessageBox.Show(this, msg, "完成", MessageBoxButtons.OK, MessageBoxIcon.Information);
            reload();
        }

        private void ValidateAll()
        {
            if (store == null)
                return;
            var err = SkillGen.Validate(store);
            if (err.Count == 0)
            {
                MessageBox.Show(this, "校验通过：列数、模板/组件引用、BuffId 重复、SkillBase 覆盖均正常。", "自检",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            MessageBox.Show(this, string.Format("发现 {0} 个问题：\n{1}", err.Count, string.Join("\n", err.Take(25))),
                "自检", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}
