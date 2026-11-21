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
        /// <summary>刷新后需要重新选中的技能 Id（编辑保存后保持当前行，0 表示不指定）。</summary>
        private int pendingSelectId;

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
            var btnGen = MakeButton("② 生成待应用", 186, 130);
            btnGen.Click += (s, e) => GenerateBase();
            var btnCheck = MakeButton("③ 配置自检", 326, 110);
            btnCheck.Click += (s, e) => ValidateAll();
            var btnApply = MakeButton("④ 应用", 446, 90);
            btnApply.Click += (s, e) => ApplyStaged();
            var tip = new Label
            {
                Text = "生成/编辑只把与 SkillConfig 有差异的行写入 SkillBaseConfig（待应用）；「④ 应用」才把效果列写入 SkillConfig、Version 末位+1，然后清空 SkillBaseConfig\n点击任意列头可按该列值筛选，再次点击可修改或清除",
                Font = Theme.Small,
                ForeColor = Theme.Faint,
                AutoSize = true,
                Location = new Point(552, 12),
            };

            top.Controls.AddRange(new Control[] { lblStats, btnAuto, btnGen, btnCheck, btnApply, tip });

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
                if (SkillGen.IsLockedForEdit(store, rowSkills[e.RowIndex]))
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
                : ModuleSegments(skill.TemplateName, skill.Modules);
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

        private List<Theme.TextSegment> ModuleSegments(string templateName, string[] modules)
        {
            var segments = new List<Theme.TextSegment>();
            if (modules == null)
                return segments;
            TemplateRow tpl;
            if (string.IsNullOrEmpty(templateName) || !store.TemplateByName.TryGetValue(templateName, out tpl))
                tpl = null;
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
                segments.Add(new Theme.TextSegment("（" + PriceText(SkillGen.ComponentScore(tpl, comp)) + "）", Theme.Accent));
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
            // 重新加载（编辑保存 / 自动分配 / 生成 SkillBase）后保留已有列筛选，只恢复表头标记
            ApplyFilterHeaders();
            RefreshGrid();
        }

        private void RefreshGrid()
        {
            if (store == null)
                return;
            // 刷新前记录当前选中行，重建后恢复（否则编辑保存后会自动跳回第一行）
            int keepId = pendingSelectId;
            pendingSelectId = 0;
            if (keepId == 0 && grid.CurrentRow != null && grid.CurrentRow.Index < rowSkills.Count)
                keepId = rowSkills[grid.CurrentRow.Index].Id;
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
                    : (status == "待应用" ? Theme.Accent : (status == "未模板化" ? Theme.Muted : Theme.Danger));
                if (status != "OK")
                    grid.Rows[idx].DefaultCellStyle.BackColor = Theme.RowWarn;

                // 锁定技能（初始属性：无 MP/CD、不支持组件）只展示，不允许编辑
                if (SkillGen.IsLockedForEdit(store, s))
                {
                    var editBtn = grid.Rows[idx].Cells[editColumn];
                    editBtn.Value = "锁定";
                    editBtn.ReadOnly = true;
                    editBtn.Style.ForeColor = Theme.Faint;
                    editBtn.Style.SelectionForeColor = Theme.Faint;
                }
            }
            Theme.EndBulkFill(grid);
            if (keepId != 0)
                SelectSkill(keepId);
            lblStats.Text = string.Format("品质 1~4 英雄技能 {0} 个{1}：已模板化 {2}，待应用行 {3}",
                skills.Count, columnFilters.Count > 0 ? "（筛选显示 " + shown + "）" : "", templated, store.BaseRows.Count);
        }

        /// <summary>按技能 Id 选中对应行并滚动到可见位置；未找到（例如被筛选隐藏）时保持默认。</summary>
        private void SelectSkill(int id)
        {
            for (int i = 0; i < rowSkills.Count; i++)
            {
                if (rowSkills[i].Id != id)
                    continue;
                grid.ClearSelection();
                grid.CurrentCell = grid.Rows[i].Cells[0];
                grid.Rows[i].Selected = true;
                grid.FirstDisplayedScrollingRowIndex = i;
                return;
            }
        }

        private string StatusOf(SkillRow s)
        {
            if (string.IsNullOrEmpty(s.TemplateName))
                return "未模板化";
            if (!store.TemplateByName.ContainsKey(s.TemplateName))
                return "模板缺失";
            return store.BaseRows.ContainsKey(s.Id) ? "待应用" : "OK";
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

        /// <summary>按当前筛选状态恢复各列表头（保留筛选，只在表头追加 ▼）。</summary>
        private void ApplyFilterHeaders()
        {
            if (baseHeaders == null)
                return;
            for (int i = 0; i < baseHeaders.Length && i < grid.Columns.Count; i++)
                grid.Columns[i].HeaderText = baseHeaders[i] + (columnFilters.ContainsKey(i) ? " ▼" : "");
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

        /// <summary>打开编辑弹窗；确认后 Template/Modules 写回 SkillConfig，并把展开结果写入待应用表。</summary>
        private void EditSkill(SkillRow skill)
        {
            using (var dlg = new SkillEditDialog(store, skill))
            {
                if (dlg.ShowDialog(FindForm()) != DialogResult.OK)
                    return;
                pendingSelectId = skill.Id;
                StageSkillGroup(skill.Sname);
                reload();
            }
        }

        /// <summary>把同 Sname 的各级行按模板展开，仅把有差异的行写入待应用表。</summary>
        private void StageSkillGroup(string sname)
        {
            var rows = store.Skills.Where(x => x.Sname == sname).ToList();
            if (rows.Count > 0)
                SkillGen.StageSkills(store, rows);
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
            var staged = SkillGen.StageSkills(store, ids.Select(id => store.SkillById[id]).Where(x => x != null));
            string msg = string.Format("已为 {0} 个技能写入模板，共 {1} 行；其中 {2} 行有差异，已写入待应用。", groups, ids.Count, staged);
            if (skipped.Count > 0)
                msg += "\n未模板化（特化）：" + string.Join("、", skipped);
            MessageBox.Show(this, msg, "完成", MessageBoxButtons.OK, MessageBoxIcon.Information);
            reload();
        }

        /// <summary>按模板重新展开全部技能，只把与 SkillConfig 有差异的行写入待应用表。</summary>
        private void GenerateBase()
        {
            if (store == null)
                return;
            if (store.TemplateByName.Count == 0)
            {
                MessageBox.Show(this, "没有模板配置，请先维护 SkillTemplateConfig。", "提示",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            List<string> warnings;
            int n = SkillGen.StageSkills(store, store.Skills, out warnings);
            string msg = string.Format("已展开全部模板化技能：{0} 行有差异，已写入待应用（无差异的行不写）。", n);
            if (warnings.Count > 0)
                msg += "\n警告：" + string.Join("\n", warnings.Take(10));
            MessageBox.Show(this, msg, "生成待应用", MessageBoxButtons.OK, MessageBoxIcon.Information);
            reload();
        }

        /// <summary>把待应用表写入 SkillConfig（效果列），Version 提到下一批次，然后清空 SkillBaseConfig。</summary>
        public void ApplyStaged()
        {
            if (store == null)
                return;
            if (store.BaseRows.Count == 0)
            {
                MessageBox.Show(this, "当前没有待应用的数据（SkillBaseConfig 为空）。", "应用",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            float version = NextVersion();
            if (MessageBox.Show(this, string.Format(
                    "将把 {0} 行待应用数据写入 SkillConfig 的效果列（Version → {1}），随后清空 SkillBaseConfig。继续？",
                    store.BaseRows.Count, CfgUtil.NumF(version)), "应用", MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Question) != DialogResult.OK)
                return;

            var ids = new List<int>();
            foreach (var kv in store.BaseRows)
            {
                SkillRow skill;
                if (!store.SkillById.TryGetValue(kv.Key, out skill) || kv.Value.Length != Col.Count)
                    continue;
                foreach (int c in SkillGen.AppliedColumns)
                {
                    // 与运行时 ApplySkillBase 一致：开局MP / MP上限层数仅在待应用值 > 0 时覆盖
                    if ((c == Col.StartMpRate || c == Col.MpLimit) && CfgUtil.ParseFloat(kv.Value[c]) <= 0f)
                        continue;
                    skill.Args[c] = kv.Value[c];
                }
                skill.Version = version;
                ids.Add(skill.Id);
            }
            ConfigIO.SaveSkillRows(store, ids);
            store.BaseRows.Clear();
            ConfigIO.WriteBaseRows(store, store.BaseRows);

            MessageBox.Show(this, string.Format("已应用 {0} 行，Version = {1}，SkillBaseConfig 已清空。",
                ids.Count, CfgUtil.NumF(version)), "应用完成", MessageBoxButtons.OK, MessageBoxIcon.Information);
            reload();
        }

        /// <summary>下一批次版本：当前最高 Version 末位 +0.1（如 1.2 → 1.3）。</summary>
        private float NextVersion()
        {
            float max = 0f;
            foreach (var s in store.Skills)
                max = Math.Max(max, s.Version);
            return (float)(Math.Round(max * 10) + 1) / 10f;
        }

        private void ValidateAll()
        {
            if (store == null)
                return;
            var err = SkillGen.Validate(store);
            if (err.Count == 0)
            {
                MessageBox.Show(this, "校验通过：列数、模板/组件引用、BuffId 重复、BuffAttrDynamic/BuffAttrConst 赋值均正常。", "自检",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            MessageBox.Show(this, string.Format("发现 {0} 个问题：\n{1}", err.Count, string.Join("\n", err.Take(25))),
                "自检", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}
