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
        private DataGridView grid;
        /// <summary>行号 → 技能（Lv1 行），供编辑按钮回查。</summary>
        private readonly List<SkillRow> rowSkills = new List<SkillRow>();
        private int editColumn = -1;
        private int templateColumn = -1;
        private int moduleColumn = -1;
        /// <summary>列索引 → 允许显示的单元格文本；不存在表示该列不过滤。</summary>
        private readonly Dictionary<int, HashSet<string>> columnFilters = new Dictionary<int, HashSet<string>>();
        /// <summary>各列原始表头（不含筛选标记），用于恢复。</summary>
        private string[] baseHeaders;
        /// <summary>当前打开的列筛选下拉。</summary>
        private ToolStripDropDown filterDropDown;

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
            var tip = new Label
            {
                Text = "规则：DamageStrength/HealStrength 取模板 Values，CD/MpCost 取模板（MpCost×10），Range/Area 仅模板>0 时覆盖，Buff 只按技能自己的 Modules 追加（模板默认组件仅估值）\n点击任意列头可按该列值筛选，再次点击可修改或清除",
                Font = Theme.Small,
                ForeColor = Theme.Faint,
                AutoSize = true,
                Location = new Point(500, 12),
            };

            top.Controls.AddRange(new Control[] { lblStats, btnAuto, btnGen, btnCheck, tip });

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
            baseHeaders = grid.Columns.Cast<DataGridViewColumn>().Select(c => c.HeaderText).ToArray();
            grid.ColumnHeaderMouseClick += OnHeaderMouseClick;

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
            columnFilters.Clear();
            ResetHeaders();
            RefreshGrid();
        }

        private void RefreshGrid()
        {
            if (store == null)
                return;
            Theme.BeginBulkFill(grid);
            rowSkills.Clear();
            var skills = store.QualityHeroSkills
                .OrderBy(s => s.Quality)
                .ThenBy(s => s.Sname)
                .ToList();
            int templated = 0;
            int shown = 0;
            foreach (var s in skills)
            {
                string status = StatusOf(s);
                if (status == "OK")
                    templated++;
                double price = SkillGen.CalcPrice(store, s);
                string[] cells =
                {
                    s.Display, s.Job, "Q" + s.Quality, price.ToString("0.##"), s.Type, s.ScriptName,
                    s.TemplateName, string.Join("、", s.Modules), status,
                };
                if (!PassFilters(cells))
                    continue;
                int idx = grid.Rows.Add(cells[0], cells[1], cells[2], cells[3], cells[4], cells[5],
                    cells[6], cells[7], cells[8], "编辑");
                rowSkills.Add(s);
                shown++;

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
            lblStats.Text = string.Format("品质 1~4 英雄技能 {0} 个{1}：已模板化 {2}，SkillBase 行 {3}",
                skills.Count, columnFilters.Count > 0 ? "（筛选显示 " + shown + "）" : "", templated, store.BaseRows.Count);
        }

        private string StatusOf(SkillRow s)
        {
            if (string.IsNullOrEmpty(s.TemplateName))
                return "未模板化";
            if (!store.TemplateByName.ContainsKey(s.TemplateName))
                return "模板缺失";
            if (!store.BaseRows.ContainsKey(s.Id))
                return "缺 SkillBase";
            return "OK";
        }

        /// <summary>按列索引取出该行的显示文本，供列筛选使用。</summary>
        private string ColumnValue(SkillRow s, int col)
        {
            switch (col)
            {
                case 0: return s.Display;
                case 1: return s.Job;
                case 2: return "Q" + s.Quality;
                case 3: return SkillGen.CalcPrice(store, s).ToString("0.##");
                case 4: return s.Type;
                case 5: return s.ScriptName;
                case 6: return s.TemplateName;
                case 7: return string.Join("、", s.Modules);
                case 8: return StatusOf(s);
                default: return null;
            }
        }

        private bool PassFilters(string[] cells)
        {
            foreach (var kv in columnFilters)
            {
                if (kv.Key < 0 || kv.Key >= cells.Length)
                    continue;
                if (!kv.Value.Contains(cells[kv.Key] ?? ""))
                    return false;
            }
            return true;
        }

        private void ResetHeaders()
        {
            if (baseHeaders == null)
                return;
            for (int i = 0; i < baseHeaders.Length && i < grid.Columns.Count; i++)
                grid.Columns[i].HeaderText = baseHeaders[i];
        }

        private void OnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left || e.ColumnIndex < 0 || e.ColumnIndex == editColumn)
                return;
            ShowFilterPanel(e.ColumnIndex);
        }

        /// <summary>列头点击：弹出该列的枚举值过滤面板（勾选=保留）。</summary>
        private void ShowFilterPanel(int col)
        {
            if (store == null)
                return;
            if (filterDropDown != null)
                filterDropDown.Close();

            var values = new SortedSet<string>(StringComparer.CurrentCulture);
            foreach (var s in store.QualityHeroSkills)
            {
                string v = ColumnValue(s, col);
                if (!string.IsNullOrEmpty(v))
                    values.Add(v);
            }
            if (values.Count == 0)
                return;

            HashSet<string> active;
            columnFilters.TryGetValue(col, out active);

            var list = new CheckedListBox
            {
                CheckOnClick = true,
                BorderStyle = BorderStyle.None,
                BackColor = Theme.Card2,
                ForeColor = Theme.Text,
                Font = Theme.Base,
                IntegralHeight = false,
                Width = 220,
                Height = Math.Min(300, Math.Max(24, values.Count * 20 + 4)),
            };
            foreach (string v in values)
                list.Items.Add(v, active == null || active.Contains(v));

            var panel = new Panel
            {
                BackColor = Theme.Card,
                Padding = new Padding(8),
                Size = new Size(236, list.Height + 52),
            };
            list.Location = new Point(8, 8);

            var btnAll = MakeSmallButton("全选", 8, list.Height + 16, 62);
            btnAll.Click += (s2, e2) =>
            {
                bool allChecked = true;
                for (int i = 0; i < list.Items.Count; i++)
                {
                    if (!list.GetItemChecked(i))
                    {
                        allChecked = false;
                        break;
                    }
                }
                for (int i = 0; i < list.Items.Count; i++)
                    list.SetItemChecked(i, !allChecked);
                btnAll.Text = allChecked ? "全选" : "全不选";
            };
            var btnClear = MakeSmallButton("清除筛选", 74, list.Height + 16, 82);
            var btnApply = MakeSmallButton("应用", 160, list.Height + 16, 62);

            panel.Controls.Add(list);
            panel.Controls.Add(btnAll);
            panel.Controls.Add(btnClear);
            panel.Controls.Add(btnApply);

            var dd = new ToolStripDropDown
            {
                Padding = Padding.Empty,
                AutoClose = true,
                DropShadowEnabled = true,
                BackColor = Theme.Card,
            };
            dd.Items.Add(new ToolStripControlHost(panel) { Margin = Padding.Empty, Padding = Padding.Empty, AutoSize = false, Size = panel.Size });
            filterDropDown = dd;
            dd.Closed += (s2, e2) =>
            {
                if (ReferenceEquals(filterDropDown, dd))
                    filterDropDown = null;
                // ToolStripManager 关闭后还会访问该下拉，立即 Dispose 会抛 ObjectDisposedException，延后一帧回收
                if (!IsDisposed && IsHandleCreated)
                    BeginInvoke(new Action(() => dd.Dispose()));
                else
                    dd.Dispose();
            };

            btnApply.Click += (s2, e2) =>
            {
                var set = new HashSet<string>(StringComparer.CurrentCulture);
                foreach (object item in list.CheckedItems)
                    set.Add(item.ToString());
                if (set.Count == values.Count)
                    columnFilters.Remove(col);
                else
                    columnFilters[col] = set;
                grid.Columns[col].HeaderText = baseHeaders[col] + (columnFilters.ContainsKey(col) ? " ▼" : "");
                RefreshGrid();
                dd.Close();
            };
            btnClear.Click += (s2, e2) =>
            {
                columnFilters.Remove(col);
                grid.Columns[col].HeaderText = baseHeaders[col];
                RefreshGrid();
                dd.Close();
            };

            // 贴在列头下沿弹出（GetColumnDisplayRectangle 的数据区顶部在表头下方，直接用表头单元格底边）
            Rectangle header = grid.GetCellDisplayRectangle(col, -1, true);
            if (header.IsEmpty)
                header = grid.GetColumnDisplayRectangle(col, true);
            Point pt = grid.PointToScreen(new Point(header.Left, header.Bottom));
            dd.Show(pt);
        }

        private static Button MakeSmallButton(string text, int x, int y, int width)
        {
            var b = new Button
            {
                Text = text,
                FlatStyle = FlatStyle.Flat,
                BackColor = Theme.Card2,
                ForeColor = Theme.Text,
                Font = Theme.Small,
                Size = new Size(width, 26),
                Location = new Point(x, y),
            };
            b.FlatAppearance.BorderColor = Theme.Line;
            return b;
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
                MessageBox.Show(this, "校验通过：列数、模板/组件引用、BuffId 重复、BuffAttrDynamic/BuffAttrConst 赋值、SkillBase 覆盖均正常。", "自检",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            MessageBox.Show(this, string.Format("发现 {0} 个问题：\n{1}", err.Count, string.Join("\n", err.Take(25))),
                "自检", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}
