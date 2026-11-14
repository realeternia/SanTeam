using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace SkillBaseTool
{
    /// <summary>③ 对比现有技能数值与模板展开后的 SkillBase 数值。</summary>
    public sealed class CompareView : UserControl
    {
        private DataStore store;
        private List<CompareItem> items = new List<CompareItem>();
        private Label lblSummary;
        private ComboBox cboFilter;
        private DataGridView grid;
        /// <summary>数据是否已变更、需要重新展开对比（切到本页时才计算）。</summary>
        private bool dirty;

        public CompareView()
        {
            BackColor = Theme.Bg;
            BuildUi();
        }

        private void BuildUi()
        {
            var top = new Panel { Dock = DockStyle.Top, Height = 84, BackColor = Theme.Card, Padding = new Padding(16, 10, 16, 10) };
            lblSummary = new Label
            {
                Text = "",
                Font = Theme.Base,
                ForeColor = Theme.Muted,
                AutoSize = true,
                Location = new Point(16, 12),
            };

            var lblFilter = new Label { Text = "筛选", Font = Theme.Small, ForeColor = Theme.Muted, AutoSize = true, Location = new Point(16, 48) };
            cboFilter = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                FlatStyle = FlatStyle.Flat,
                BackColor = Theme.Card2,
                ForeColor = Theme.Text,
                Font = Theme.Base,
                Location = new Point(52, 44),
                Size = new Size(180, 26),
            };
            cboFilter.Items.AddRange(new object[] { "全部", "仅有差异", "仅主数值差异", "仅节奏差异", "仅 Buff 差异" });
            cboFilter.SelectedIndex = 1;
            cboFilter.SelectedIndexChanged += (s, e) => RefreshGrid();

            var btnRefresh = new Button
            {
                Text = "重新对比",
                FlatStyle = FlatStyle.Flat,
                BackColor = Theme.Card2,
                ForeColor = Theme.Text,
                Location = new Point(246, 44),
                Size = new Size(100, 28),
            };
            btnRefresh.FlatAppearance.BorderColor = Theme.Line;
            btnRefresh.Click += (s, e) => Rebuild();
            top.Controls.AddRange(new Control[] { lblSummary, lblFilter, cboFilter, btnRefresh });

            grid = new DataGridView { Dock = DockStyle.Fill };
            Theme.StyleGrid(grid);
            grid.Columns.Add(MakeColumn("技能", 160));
            grid.Columns.Add(MakeColumn("类型", 55));
            grid.Columns.Add(MakeColumn("模板", 105));
            grid.Columns.Add(MakeColumn("主数值", 110));
            grid.Columns.Add(MakeColumn("CD", 70));
            grid.Columns.Add(MakeColumn("MpCost", 70));
            grid.Columns.Add(MakeColumn("Range / Area", 130));
            grid.Columns.Add(MakeColumn("Buff", 180));
            grid.Columns.Add(MakeColumn("全部差异", 260));

            Controls.Add(grid);
            Controls.Add(top);
        }

        private static DataGridViewTextBoxColumn MakeColumn(string header, int weight)
        {
            return new DataGridViewTextBoxColumn
            {
                HeaderText = header,
                FillWeight = weight,
                SortMode = DataGridViewColumnSortMode.NotSortable,
            };
        }

        public void Bind(DataStore data)
        {
            store = data;
            dirty = true;
            // 未切到本页时先不做展开对比，等 Tab 激活再算，避免每次刷新都白跑一遍
            if (Visible)
                Rebuild();
        }

        /// <summary>Tab 激活时调用：数据有变化才重新对比。</summary>
        public void EnsureBuilt()
        {
            if (dirty)
                Rebuild();
        }

        private void Rebuild()
        {
            if (store == null)
                return;
            dirty = false;
            var gen = SkillGen.GenerateBase(store);
            items = SkillGen.Compare(store, gen);

            int noDiff = items.Count(x => !x.HasDiff);
            int main = items.Count(x => x.MainValueDiff);
            int rhythm = items.Count(x => x.RhythmDiff);
            int buff = items.Count(x => x.BuffDiff);
            double ratio = 0;
            int rc = 0;
            foreach (var it in items)
            {
                float hand = it.Skill.DamageStrength != 0 ? it.Skill.DamageStrength : it.Skill.HealStrength;
                float tpl = CfgUtil.ParseFloat(it.Base[Col.DamageStrength]) != 0
                    ? CfgUtil.ParseFloat(it.Base[Col.DamageStrength])
                    : CfgUtil.ParseFloat(it.Base[Col.HealStrength]);
                if (tpl > 0 && hand > 0)
                {
                    ratio += hand / tpl;
                    rc++;
                }
            }
            lblSummary.Text = string.Format(
                "模板化技能 {0} 个（Lv1）：完全一致 {1}，主数值差异 {2}，节奏差异 {3}，Buff 差异 {4}；手配/模板主数值平均比 {5}",
                items.Count, noDiff, main, rhythm, buff, rc > 0 ? (ratio / rc).ToString("0.00") : "-");
            RefreshGrid();
        }

        private void RefreshGrid()
        {
            Theme.BeginBulkFill(grid);
            string mode = cboFilter.SelectedItem as string ?? "全部";
            foreach (var it in items)
            {
                if (mode == "仅有差异" && !it.HasDiff) continue;
                if (mode == "仅主数值差异" && !it.MainValueDiff) continue;
                if (mode == "仅节奏差异" && !it.RhythmDiff) continue;
                if (mode == "仅 Buff 差异" && !it.BuffDiff) continue;

                string main = Join(it, "伤害", "治疗");
                string cd = Join(it, "CD ");
                string mp = Join(it, "MpCost ");
                string range = Join(it, "Range ", "Area ");
                string buff = Join(it, "BuffId ", "Buff值 ");
                int idx = grid.Rows.Add(it.Skill.Display, it.Skill.Type, it.Template,
                    main, cd, mp, range, buff, string.Join("；", it.Diffs));
                var row = grid.Rows[idx];
                if (!it.HasDiff)
                    row.DefaultCellStyle.BackColor = Theme.RowOk;
                else if (it.MainValueDiff)
                {
                    row.DefaultCellStyle.BackColor = Theme.RowWarn;
                    row.Cells[3].Style.ForeColor = Theme.Accent;
                    row.Cells[3].Style.Font = Theme.Bold;
                }
                else
                    row.DefaultCellStyle.BackColor = Theme.RowWarn;
                row.Cells[8].ToolTipText = string.Join(Environment.NewLine, it.Diffs);
            }
            Theme.EndBulkFill(grid);
        }

        private static string Join(CompareItem item, params string[] prefixes)
        {
            var parts = item.Diffs.Where(d => prefixes.Any(p => d.StartsWith(p, StringComparison.Ordinal))).ToList();
            return parts.Count == 0 ? "=" : string.Join("  ", parts);
        }
    }
}
