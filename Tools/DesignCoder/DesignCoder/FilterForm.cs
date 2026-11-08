using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace DesignCoder
{
    /// <summary>
    /// 列筛选对话框：类似 Excel 自动筛选，勾选该列允许显示的单元格值。
    /// 顶部可输入关键字过滤值列表；全选/全不选/清除筛选；确定后仅显示勾选的行。
    /// </summary>
    public sealed class FilterForm : Form
    {
        private readonly List<string> allValues;
        private readonly Dictionary<string, bool> checkedState = new Dictionary<string, bool>();
        private readonly CheckedListBox listValues;
        private readonly TextBox txtSearch;
        private readonly Label lblSummary;
        private bool rebuilding = false;

        /// <summary>true 表示“清除筛选”（未选任何值或全选）。</summary>
        public bool Cleared { get; private set; }

        /// <summary>勾选的值集合（Cleared 为 false 时有效）。</summary>
        public HashSet<string> SelectedValues { get; private set; }

        public FilterForm(string fieldTitle, List<string> distinctValues, HashSet<string> currentSelection)
        {
            allValues = new List<string>(distinctValues);
            foreach (string value in allValues)
                checkedState[value] = currentSelection == null || currentSelection.Contains(value);

            Text = "筛选 - " + fieldTitle;
            Size = new Size(420, 640);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MinimizeBox = false;
            MaximizeBox = false;
            Font = new Font("微软雅黑", 9F);
            BackColor = Color.FromArgb(30, 30, 30);
            ForeColor = Color.FromArgb(220, 220, 220);

            var lblTitle = new Label();
            lblTitle.Text = string.Format("列：{0}（共 {1} 个值）", fieldTitle, allValues.Count);
            lblTitle.Location = new Point(12, 10);
            lblTitle.Size = new Size(388, 22);
            lblTitle.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            Controls.Add(lblTitle);

            var lblSearch = new Label();
            lblSearch.Text = "搜索";
            lblSearch.Location = new Point(12, 44);
            lblSearch.Size = new Size(40, 22);
            Controls.Add(lblSearch);

            txtSearch = new TextBox();
            txtSearch.Location = new Point(54, 40);
            txtSearch.Size = new Size(346, 24);
            txtSearch.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtSearch.BackColor = Color.FromArgb(45, 45, 48);
            txtSearch.ForeColor = Color.FromArgb(230, 230, 230);
            txtSearch.BorderStyle = BorderStyle.FixedSingle;
            txtSearch.TextChanged += (s, e) => RebuildValueList();
            Controls.Add(txtSearch);

            listValues = new CheckedListBox();
            listValues.Location = new Point(12, 72);
            listValues.Size = new Size(388, 444);
            listValues.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            listValues.CheckOnClick = true;
            listValues.BorderStyle = BorderStyle.FixedSingle;
            listValues.BackColor = Color.FromArgb(37, 37, 38);
            listValues.ForeColor = Color.FromArgb(220, 220, 220);
            listValues.IntegralHeight = false;
            listValues.ItemCheck += listValues_ItemCheck;
            Controls.Add(listValues);

            lblSummary = new Label();
            lblSummary.Location = new Point(12, 522);
            lblSummary.Size = new Size(388, 22);
            lblSummary.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lblSummary.ForeColor = Color.FromArgb(160, 160, 170);
            Controls.Add(lblSummary);

            AddButton("全选", 12, btnAll_Click);
            AddButton("全不选", 88, btnNone_Click);
            AddButton("清除筛选", 168, btnClear_Click);
            AddButton("确定", 240, btnOk_Click);
            AddButton("取消", 328, btnCancel_Click, true);

            AcceptButton = null;
            CancelButton = null;
            RebuildValueList();
        }

        private void AddButton(string text, int x, EventHandler handler, bool cancel = false)
        {
            var btn = new Button();
            btn.Text = text;
            btn.Location = new Point(x, 556);
            btn.Size = new Size(cancel ? 72 : 68, 28);
            btn.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderColor = Color.FromArgb(90, 90, 96);
            btn.BackColor = Color.FromArgb(52, 52, 56);
            btn.ForeColor = Color.FromArgb(225, 225, 225);
            btn.Click += handler;
            Controls.Add(btn);
        }

        // 按搜索关键字重建值列表，勾选状态保存在 checkedState 中
        private void RebuildValueList()
        {
            if (rebuilding) return;
            rebuilding = true;

            string keyword = txtSearch.Text != null ? txtSearch.Text.Trim() : "";
            listValues.BeginUpdate();
            listValues.Items.Clear();
            foreach (string value in allValues)
            {
                string display = DisplayValue(value);
                if (keyword.Length > 0 && display.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) < 0)
                    continue;
                listValues.Items.Add(new ValueItem(value), checkedState[value]);
            }
            listValues.EndUpdate();

            rebuilding = false;
            UpdateSummary();
        }

        private void listValues_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (rebuilding) return;
            ValueItem item = listValues.Items[e.Index] as ValueItem;
            if (item == null) return;
            checkedState[item.Value] = e.NewValue == CheckState.Checked;
            BeginInvoke(new Action(UpdateSummary));
        }

        private void UpdateSummary()
        {
            int checkedCount = 0;
            foreach (bool checkedValue in checkedState.Values)
                if (checkedValue) checkedCount++;
            lblSummary.Text = string.Format("已勾选 {0} / {1} 个值", checkedCount, allValues.Count);
        }

        private static string DisplayValue(string value)
        {
            return string.IsNullOrEmpty(value) ? "(空)" : value;
        }

        private void btnAll_Click(object sender, EventArgs e)
        {
            foreach (string key in new List<string>(checkedState.Keys))
                checkedState[key] = true;
            RebuildValueList();
        }

        private void btnNone_Click(object sender, EventArgs e)
        {
            foreach (string key in new List<string>(checkedState.Keys))
                checkedState[key] = false;
            RebuildValueList();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            Cleared = true;
            SelectedValues = null;
            DialogResult = DialogResult.OK;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            var selected = new HashSet<string>(StringComparer.Ordinal);
            foreach (KeyValuePair<string, bool> kv in checkedState)
                if (kv.Value) selected.Add(kv.Key);

            // 没勾选任何值或全选，等价于不筛选
            if (selected.Count == 0 || selected.Count == allValues.Count)
            {
                Cleared = true;
                SelectedValues = null;
            }
            else
            {
                Cleared = false;
                SelectedValues = selected;
            }
            DialogResult = DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        /// <summary>列表项包装：显示文本把空字符串显示为“(空)”，勾选值仍是原始单元格值。</summary>
        private sealed class ValueItem
        {
            public readonly string Value;

            public ValueItem(string value)
            {
                Value = value;
            }

            public override string ToString()
            {
                return DisplayValue(Value);
            }
        }
    }
}
