using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DesignCoder
{
    public partial class Form1 : Form
    {
        // 相对可执行文件目录定位 Configs，保证工具复制到任意目录（含 SanTeam 部署）都能找到配置
        private static readonly string ConfigDir = Path.GetFullPath(
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\..\..\Assets\Resources\Scripts\Configs"));
        private const int HeaderRowCount = 3;

        private ConfigData currentConfig;
        private string currentFilePath;
        private DataTable dataTable;
        private int sortedColumnIndex = -1;
        private bool sortAscending = true;
        private bool isLoading = false;
        private int selectedColumnIndex = -1;
        private int selectedRowIndex = -1;
        private int firstDataColIdx = -1;

        private Dictionary<string, ConfigData> loadedConfigs = new Dictionary<string, ConfigData>();
        private Dictionary<string, string> originalSources = new Dictionary<string, string>();
        private HashSet<string> modifiedConfigs = new HashSet<string>();
        private string copiedCellValue = null;

        public Form1()
        {
            InitializeComponent();
            this.KeyPreview = true;
            this.KeyDown += Form1_KeyDown;
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.S)
            {
                btnSave_Click(sender, e);
                e.Handled = true;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            RefreshFileList();
        }

        private void RefreshFileList()
        {
            listView1.Items.Clear();
            if (!Directory.Exists(ConfigDir)) return;

            var files = Directory.GetFiles(ConfigDir, "*_s.cs");
            foreach (var f in files)
            {
                string name = Path.GetFileNameWithoutExtension(f);
                if (name.EndsWith("_s")) name = name.Substring(0, name.Length - 2);
                listView1.Items.Add(name);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            RefreshFileList();
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0) return;

            SaveCurrentEditingData();

            string selectedName = listView1.SelectedItems[0].Text.TrimEnd('*');
            string filePath = Path.Combine(ConfigDir, selectedName + "_s.cs");

            if (!File.Exists(filePath)) return;

            currentFilePath = filePath;
            LoadConfigFile(filePath, selectedName);
        }

        private void SaveCurrentEditingData()
        {
            if (dataGridView1 == null || dataTable == null || currentConfig == null) return;
            
            dataGridView1.EndEdit();
            
            var cm = (CurrencyManager)BindingContext[dataTable];
            if (cm != null)
            {
                cm.EndCurrentEdit();
            }
            
            foreach (DataRow row in dataTable.Rows)
            {
                if (row["_RowTag_"] == DBNull.Value || string.IsNullOrEmpty(row["_RowTag_"] as string))
                {
                    row["_RowTag_"] = "Data";
                    MarkCurrentConfigModified();
                }
            }
            
            SyncDataTableToConfig();
        }

        private void LoadConfigFile(string filePath, string configName)
        {
            isLoading = true;
            try
            {
                if (loadedConfigs.ContainsKey(configName) && modifiedConfigs.Contains(configName))
                {
                    currentConfig = loadedConfigs[configName];
                }
                else
                {
                    string source = File.ReadAllText(filePath, Encoding.UTF8);
                    currentConfig = ConfigData.Parse(source);
                    loadedConfigs[configName] = currentConfig;
                    originalSources[configName] = source;
                }
                
                sortedColumnIndex = -1;
                sortAscending = true;
                selectedColumnIndex = -1;
                selectedRowIndex = -1;
                BuildDataTable();
                SetupDataGridView();
                ApplyColors();
            }
            finally
            {
                isLoading = false;
            }
        }

        private void BuildDataTable()
        {
            foreach (DataGridViewColumn col in dataGridView1.Columns)
                col.Frozen = false;
            dataGridView1.DataSource = null;

            dataTable = new DataTable();

            dataTable.Columns.Add("_RowTag_", typeof(string));
            foreach (var field in currentConfig.Fields)
            {
                dataTable.Columns.Add(field.Name, typeof(string));
            }

            var fieldNameRow = dataTable.NewRow();
            fieldNameRow["_RowTag_"] = "FieldName";
            var chineseNameRow = dataTable.NewRow();
            chineseNameRow["_RowTag_"] = "ChineseName";
            var typeRow = dataTable.NewRow();
            typeRow["_RowTag_"] = "Type";

            for (int i = 0; i < currentConfig.Fields.Count; i++)
            {
                var field = currentConfig.Fields[i];
                fieldNameRow[field.Name] = field.IsIndex ? "★" + field.Name : field.Name;
                chineseNameRow[field.Name] = field.ChineseName ?? field.Comment ?? "";
                typeRow[field.Name] = field.Type;
            }

            dataTable.Rows.Add(fieldNameRow);
            dataTable.Rows.Add(chineseNameRow);
            dataTable.Rows.Add(typeRow);

            foreach (var row in currentConfig.Rows)
            {
                var dataRow = dataTable.NewRow();
                dataRow["_RowTag_"] = "Data";
                foreach (var field in currentConfig.Fields)
                {
                    string val = row.ContainsKey(field.Name) ? row[field.Name] : "";
                    dataRow[field.Name] = val;
                }
                dataTable.Rows.Add(dataRow);
            }

            dataGridView1.DataSource = dataTable;
        }

        private void SetupDataGridView()
        {
            dataGridView1.Columns["_RowTag_"].Visible = false;

            typeof(DataGridView).InvokeMember("DoubleBuffered", 
                System.Reflection.BindingFlags.SetProperty | 
                System.Reflection.BindingFlags.Instance | 
                System.Reflection.BindingFlags.NonPublic, 
                null, dataGridView1, new object[] { true });

            dataGridView1.SuspendLayout();

            Font dataFont = new Font("微软雅黑", 10F);
            Font headerFont = new Font("微软雅黑", 10F, FontStyle.Bold);
            Color darkBg = Color.FromArgb(45, 45, 48);
            Color darkRow = Color.FromArgb(37, 37, 38);
            Color darkAltRow = Color.FromArgb(42, 42, 44);
            Color darkSelection = Color.FromArgb(0, 122, 204);
            Color textColor = Color.FromArgb(220, 220, 220);
            Color deepMorandiBlue = Color.FromArgb(70, 90, 115);
            Color idColumnBg = Color.FromArgb(35, 60, 95);
            Color idColumnFg = Color.FromArgb(140, 190, 255);

            dataGridView1.BackgroundColor = darkBg;
            dataGridView1.DefaultCellStyle.Font = dataFont;
            dataGridView1.DefaultCellStyle.BackColor = darkRow;
            dataGridView1.DefaultCellStyle.ForeColor = textColor;
            dataGridView1.DefaultCellStyle.SelectionBackColor = darkSelection;
            dataGridView1.DefaultCellStyle.SelectionForeColor = Color.White;
            dataGridView1.AlternatingRowsDefaultCellStyle.BackColor = darkAltRow;

            dataGridView1.ColumnHeadersVisible = false;
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dataGridView1.ColumnHeadersHeight = 28;
            dataGridView1.RowTemplate.Height = 26;

            firstDataColIdx = -1;
            for (int i = 0; i < currentConfig.Fields.Count; i++)
            {
                string colName = currentConfig.Fields[i].Name;
                string fieldType = currentConfig.Fields[i].Type.ToLower();
                if (dataGridView1.Columns.Contains(colName))
                {
                    dataGridView1.Columns[colName].HeaderText = colName;
                    dataGridView1.Columns[colName].DefaultCellStyle.Font = dataFont;
                    dataGridView1.Columns[colName].DefaultCellStyle.BackColor = darkRow;
                    dataGridView1.Columns[colName].DefaultCellStyle.ForeColor = textColor;

                    int defaultWidth = -1;
                    if (colName == "Id")
                    {
                        defaultWidth = 60;
                    }
                    else if (fieldType == "int" || fieldType == "float")
                    {
                        defaultWidth = 60;
                    }

                    var field = currentConfig.Fields.FirstOrDefault(f => f.Name == colName);
                    if (field != null && field.Width.HasValue && field.Width.Value > 0)
                    {
                        dataGridView1.Columns[colName].Width = field.Width.Value;
                    }
                    else if (defaultWidth > 0)
                    {
                        dataGridView1.Columns[colName].Width = defaultWidth;
                    }

                    if (colName == "Id" && firstDataColIdx < 0)
                    {
                        firstDataColIdx = dataGridView1.Columns[colName].Index;
                    }
                }
            }

            if (firstDataColIdx < 0 && currentConfig.Fields.Count > 0)
            {
                string firstFieldName = currentConfig.Fields[0].Name;
                if (dataGridView1.Columns.Contains(firstFieldName))
                    firstDataColIdx = dataGridView1.Columns[firstFieldName].Index;
            }

            if (firstDataColIdx >= 0)
            {
                dataGridView1.Columns[firstDataColIdx].DefaultCellStyle.BackColor = idColumnBg;
                dataGridView1.Columns[firstDataColIdx].DefaultCellStyle.Font = headerFont;
                dataGridView1.Columns[firstDataColIdx].DefaultCellStyle.ForeColor = idColumnFg;
                dataGridView1.Columns[firstDataColIdx].Frozen = true;
            }

            dataGridView1.AllowUserToResizeRows = false;
            dataGridView1.RowHeadersVisible = false;

            for (int i = 0; i < HeaderRowCount && i < dataGridView1.Rows.Count; i++)
            {
                dataGridView1.Rows[i].Frozen = true;
                dataGridView1.Rows[i].ReadOnly = true;
                dataGridView1.Rows[i].DefaultCellStyle.BackColor = deepMorandiBlue;
                dataGridView1.Rows[i].DefaultCellStyle.ForeColor = Color.White;
                dataGridView1.Rows[i].DefaultCellStyle.Font = headerFont;
                dataGridView1.Rows[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGridView1.Rows[i].DefaultCellStyle.SelectionBackColor = deepMorandiBlue;
                dataGridView1.Rows[i].DefaultCellStyle.SelectionForeColor = Color.White;
            }

            dataGridView1.GridColor = Color.FromArgb(60, 60, 65);
            dataGridView1.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dataGridView1.EnableHeadersVisualStyles = false;
            dataGridView1.BorderStyle = BorderStyle.None;

            dataGridView1.ResumeLayout(false);

            dataGridView1.FirstDisplayedScrollingRowIndex = HeaderRowCount;
        }

        private void SyncDataTableToConfig()
        {
            if (currentConfig == null || dataTable == null) return;

            currentConfig.Rows.Clear();
            foreach (DataRow dr in dataTable.Rows)
            {
                string tag = dr["_RowTag_"] as string;
                if (tag != "Data") continue;

                var row = new Dictionary<string, string>();
                foreach (var field in currentConfig.Fields)
                {
                    row[field.Name] = dr[field.Name] == DBNull.Value ? "" : dr[field.Name].ToString();
                }
                currentConfig.Rows.Add(row);
            }
        }

        private void SyncCellMetasToConfig()
        {
            if (currentConfig == null) return;
            currentConfig.CellMetas.Clear();

            for (int rowIdx = HeaderRowCount; rowIdx < dataGridView1.Rows.Count; rowIdx++)
            {
                for (int colIdx = 0; colIdx < dataGridView1.Columns.Count; colIdx++)
                {
                    if (!dataGridView1.Columns[colIdx].Visible) continue;

                    var cell = dataGridView1.Rows[rowIdx].Cells[colIdx];
                    bool hasForeColor = cell.Style.ForeColor != Color.Empty && cell.Style.ForeColor != dataGridView1.DefaultCellStyle.ForeColor;
                    bool hasBackColor = cell.Style.BackColor != Color.Empty && cell.Style.BackColor != dataGridView1.DefaultCellStyle.BackColor;

                    if (hasForeColor || hasBackColor)
                    {
                        var cm = new CellMeta();
                        cm.Row = rowIdx - HeaderRowCount;
                        cm.Col = colIdx;
                        if (hasForeColor) cm.ForeColor = cell.Style.ForeColor.ToArgb();
                        if (hasBackColor) cm.BackColor = cell.Style.BackColor.ToArgb();
                        currentConfig.CellMetas.Add(cm);
                    }
                }
            }
        }

        private void SyncColumnWidthsToConfig()
        {
            if (currentConfig == null) return;

            foreach (var field in currentConfig.Fields)
            {
                if (dataGridView1.Columns.Contains(field.Name))
                {
                    int width = dataGridView1.Columns[field.Name].Width;
                    int defaultWidth = GetDefaultColumnWidth(field.Name, field.Type);
                    if (width != defaultWidth)
                    {
                        field.Width = width;
                    }
                    else
                    {
                        field.Width = null;
                    }
                }
            }
        }

        private int GetDefaultColumnWidth(string colName, string fieldType)
        {
            if (colName == "Id") return 50;
            fieldType = fieldType.ToLower();
            if (fieldType == "int" || fieldType == "float") return 50;
            return 100;
        }

        private void MarkCurrentConfigModified()
        {
            if (currentConfig == null || string.IsNullOrEmpty(currentFilePath)) return;
            
            string configName = Path.GetFileNameWithoutExtension(currentFilePath);
            if (configName.EndsWith("_s")) configName = configName.Substring(0, configName.Length - 2);
            
            modifiedConfigs.Add(configName);
            UpdateListViewModifiedMarks();
        }

        private void UpdateListViewModifiedMarks()
        {
            for (int i = 0; i < listView1.Items.Count; i++)
            {
                string name = listView1.Items[i].Text.TrimEnd('*');
                if (modifiedConfigs.Contains(name))
                {
                    listView1.Items[i].Text = name + "*";
                }
                else
                {
                    listView1.Items[i].Text = name;
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveCurrentEditingData();

            if (modifiedConfigs.Count == 0)
            {
                MessageBox.Show("没有需要保存的修改", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            foreach (string configName in modifiedConfigs)
            {
                string filePath = Path.Combine(ConfigDir, configName + "_s.cs");
                if (loadedConfigs.ContainsKey(configName))
                {
                    var config = loadedConfigs[configName];
                    string newSource = config.GenerateSource();
                    File.WriteAllText(filePath, newSource, Encoding.UTF8);
                    originalSources[configName] = newSource;
                }
            }
            
            modifiedConfigs.Clear();
            UpdateListViewModifiedMarks();
            MessageBox.Show("保存成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnNewRow_Click(object sender, EventArgs e)
        {
            if (dataTable == null) return;
            var row = dataTable.NewRow();
            row["_RowTag_"] = "Data";
            foreach (var field in currentConfig.Fields)
            {
                row[field.Name] = "";
            }
            dataTable.Rows.Add(row);
            SyncDataTableToConfig();
            MarkCurrentConfigModified();
        }

        private void btnDeleteRow_Click(object sender, EventArgs e)
        {
            if (dataGridView1 == null || dataGridView1.SelectedRows.Count == 0) return;

            foreach (DataGridViewRow selRow in dataGridView1.SelectedRows)
            {
                if (selRow.Index >= HeaderRowCount && !selRow.IsNewRow)
                    dataGridView1.Rows.Remove(selRow);
            }
            SyncDataTableToConfig();
            MarkCurrentConfigModified();
        }

        private void btnBatchFill_Click(object sender, EventArgs e)
        {
            if (dataGridView1 == null || dataGridView1.SelectedCells.Count == 0)
            {
                MessageBox.Show("请先选中要填充的单元格", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string input = ShowInputDialog("批量填充", "请输入要填充的值：");
            if (input == null) return;

            foreach (DataGridViewCell cell in dataGridView1.SelectedCells)
            {
                if (cell.RowIndex < HeaderRowCount) continue;
                if (cell.RowIndex >= 0 && cell.ColumnIndex >= 0)
                {
                    int fieldIdx = GetFieldIndexFromColumnIndex(cell.ColumnIndex);
                    if (fieldIdx >= 0 && fieldIdx < currentConfig.Fields.Count)
                    {
                        string fieldType = currentConfig.Fields[fieldIdx].Type;
                        if (!ValidateInput(input, fieldType))
                        {
                            MessageBox.Show(string.Format("值\"{0}\"不符合字段\"{1}\"的类型({2})", input, currentConfig.Fields[fieldIdx].Name, fieldType), "类型错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }
                }
            }

            foreach (DataGridViewCell cell in dataGridView1.SelectedCells)
            {
                if (cell.RowIndex >= HeaderRowCount && cell.ColumnIndex >= 0)
                {
                    cell.Value = input;
                }
            }
            SyncDataTableToConfig();
            MarkCurrentConfigModified();
        }

        private void btnMultiply_Click(object sender, EventArgs e)
        {
            if (dataGridView1 == null || dataGridView1.SelectedCells.Count == 0)
            {
                MessageBox.Show("请先选中要成倍计算的单元格", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 检查选中的单元格是否都是 int 类型
            foreach (DataGridViewCell cell in dataGridView1.SelectedCells)
            {
                if (cell.RowIndex < HeaderRowCount) continue;
                int fieldIdx = GetFieldIndexFromColumnIndex(cell.ColumnIndex);
                if (fieldIdx >= 0 && fieldIdx < currentConfig.Fields.Count)
                {
                    string fieldType = currentConfig.Fields[fieldIdx].Type.ToLower();
                    if (fieldType != "int")
                    {
                        MessageBox.Show(string.Format("成倍操作仅支持int类型字段，字段\"{0}\"的类型为{1}", currentConfig.Fields[fieldIdx].Name, currentConfig.Fields[fieldIdx].Type), "类型错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }
            }

            string input = ShowInputDialog("成倍计算", "请输入倍数（浮点数，如0.7）：");
            if (input == null) return;

            float multiplier;
            if (!float.TryParse(input, out multiplier))
            {
                MessageBox.Show("请输入有效的浮点数", "输入错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            bool anyChanged = false;
            foreach (DataGridViewCell cell in dataGridView1.SelectedCells)
            {
                if (cell.RowIndex < HeaderRowCount || cell.ColumnIndex < 0) continue;

                string cellValue = cell.Value != null ? cell.Value.ToString() : "0";
                int intVal;
                if (int.TryParse(cellValue, out intVal))
                {
                    int newVal = (int)Math.Round(intVal * multiplier);
                    cell.Value = newVal.ToString();
                    anyChanged = true;
                }
            }

            if (anyChanged)
            {
                SyncDataTableToConfig();
                MarkCurrentConfigModified();
            }
        }

        private int GetFieldIndexFromColumnIndex(int colIdx)
        {
            if (colIdx <= 0) return -1;
            string colName = dataGridView1.Columns[colIdx].Name;
            for (int i = 0; i < currentConfig.Fields.Count; i++)
            {
                if (currentConfig.Fields[i].Name == colName)
                    return i;
            }
            return -1;
        }

        private bool ValidateInput(string input, string type)
        {
            if (string.IsNullOrEmpty(input)) return true;

            if (type == "int")
            {
                int v;
                return int.TryParse(input, out v);
            }
            if (type == "float")
            {
                string s = input.TrimEnd('f', 'F');
                float v;
                return float.TryParse(s, out v);
            }
            if (type == "bool")
            {
                return input == "true" || input == "false" || input == "True" || input == "False";
            }
            if (type == "int[]")
            {
                if (string.IsNullOrWhiteSpace(input)) return true;
                var items = input.Split(',');
                foreach (var item in items)
                {
                    int v;
                    if (!int.TryParse(item.Trim(), out v)) return false;
                }
                return true;
            }
            if (type == "float[]")
            {
                if (string.IsNullOrWhiteSpace(input)) return true;
                var items = input.Split(',');
                foreach (var item in items)
                {
                    string s = item.Trim().TrimEnd('f', 'F');
                    float v;
                    if (!float.TryParse(s, out v)) return false;
                }
                return true;
            }
            return true;
        }

        private void btnForeColor_Click(object sender, EventArgs e)
        {
            if (dataGridView1 == null || dataGridView1.SelectedCells.Count == 0)
            {
                MessageBox.Show("请先选中要设置的单元格", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (colorDialog1.ShowDialog() != DialogResult.OK) return;

            foreach (DataGridViewCell cell in dataGridView1.SelectedCells)
            {
                if (cell.RowIndex >= HeaderRowCount)
                    cell.Style.ForeColor = colorDialog1.Color;
            }
            SyncCellMetasToConfig();
            MarkCurrentConfigModified();
        }

        private void btnBackColor_Click(object sender, EventArgs e)
        {
            if (dataGridView1 == null || dataGridView1.SelectedCells.Count == 0)
            {
                MessageBox.Show("请先选中要设置的单元格", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (colorDialog1.ShowDialog() != DialogResult.OK) return;

            foreach (DataGridViewCell cell in dataGridView1.SelectedCells)
            {
                if (cell.RowIndex >= HeaderRowCount)
                    cell.Style.BackColor = colorDialog1.Color;
            }
            SyncCellMetasToConfig();
            MarkCurrentConfigModified();
        }

        private void btnClearColors_Click(object sender, EventArgs e)
        {
            if (dataGridView1 == null || dataGridView1.SelectedCells.Count == 0)
            {
                MessageBox.Show("请先选中要清除颜色的单元格", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            foreach (DataGridViewCell cell in dataGridView1.SelectedCells)
            {
                if (cell.RowIndex >= HeaderRowCount)
                {
                    cell.Style.ForeColor = Color.Empty;
                    cell.Style.BackColor = Color.Empty;
                }
            }
            SyncCellMetasToConfig();
            MarkCurrentConfigModified();
        }

        private string ShowInputDialog(string title, string prompt)
        {
            Form inputForm = new Form();
            inputForm.Text = title;
            inputForm.Size = new Size(300, 150);
            inputForm.StartPosition = FormStartPosition.CenterParent;
            inputForm.FormBorderStyle = FormBorderStyle.FixedDialog;
            inputForm.MaximizeBox = false;
            inputForm.MinimizeBox = false;

            Label lbl = new Label();
            lbl.Text = prompt;
            lbl.Location = new Point(10, 10);
            lbl.AutoSize = true;
            inputForm.Controls.Add(lbl);

            TextBox txt = new TextBox();
            txt.Location = new Point(10, 35);
            txt.Size = new Size(260, 22);
            inputForm.Controls.Add(txt);

            Button btnOk = new Button();
            btnOk.Text = "确定";
            btnOk.DialogResult = DialogResult.OK;
            btnOk.Location = new Point(100, 70);
            inputForm.Controls.Add(btnOk);

            Button btnCancel = new Button();
            btnCancel.Text = "取消";
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.Location = new Point(190, 70);
            inputForm.Controls.Add(btnCancel);

            inputForm.AcceptButton = btnOk;
            inputForm.CancelButton = btnCancel;

            if (inputForm.ShowDialog() == DialogResult.OK)
                return txt.Text;
            return null;
        }

        private FieldDef ShowAddColumnDialog()
        {
            Form dlg = new Form();
            dlg.Text = "新增列";
            dlg.Size = new Size(360, 300);
            dlg.StartPosition = FormStartPosition.CenterParent;
            dlg.FormBorderStyle = FormBorderStyle.FixedDialog;
            dlg.MaximizeBox = false;
            dlg.MinimizeBox = false;
            dlg.Font = new Font("微软雅黑", 9F);

            int labelX = 15, inputX = 100, inputW = 220, rowH = 32, startY = 15;

            Label lblName = new Label(); lblName.Text = "字段名:"; lblName.Location = new Point(labelX, startY + 5); lblName.AutoSize = true; dlg.Controls.Add(lblName);
            TextBox txtName = new TextBox(); txtName.Location = new Point(inputX, startY); txtName.Size = new Size(inputW, 22); dlg.Controls.Add(txtName);

            startY += rowH;
            Label lblChinese = new Label(); lblChinese.Text = "中文名:"; lblChinese.Location = new Point(labelX, startY + 5); lblChinese.AutoSize = true; dlg.Controls.Add(lblChinese);
            TextBox txtChinese = new TextBox(); txtChinese.Location = new Point(inputX, startY); txtChinese.Size = new Size(inputW, 22); dlg.Controls.Add(txtChinese);

            startY += rowH;
            Label lblType = new Label(); lblType.Text = "类型:"; lblType.Location = new Point(labelX, startY + 5); lblType.AutoSize = true; dlg.Controls.Add(lblType);
            ComboBox cmbType = new ComboBox();
            cmbType.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbType.Items.AddRange(new object[] { "int", "float", "string", "bool", "string[]", "int[]", "float[]" });
            cmbType.Location = new Point(inputX, startY);
            cmbType.Size = new Size(inputW, 22);
            cmbType.SelectedIndex = 0;
            dlg.Controls.Add(cmbType);

            startY += rowH;
            Label lblRule = new Label(); lblRule.Text = "字段规则:"; lblRule.Location = new Point(labelX, startY + 5); lblRule.AutoSize = true; dlg.Controls.Add(lblRule);
            TextBox txtRule = new TextBox(); txtRule.Location = new Point(inputX, startY); txtRule.Size = new Size(inputW, 22); dlg.Controls.Add(txtRule);

            startY += rowH + 10;
            Button btnOk = new Button();
            btnOk.Text = "确定";
            btnOk.DialogResult = DialogResult.OK;
            btnOk.Location = new Point(inputX + 50, startY);
            dlg.Controls.Add(btnOk);

            Button btnCancel = new Button();
            btnCancel.Text = "取消";
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.Location = new Point(inputX + 140, startY);
            dlg.Controls.Add(btnCancel);

            dlg.AcceptButton = btnOk;
            dlg.CancelButton = btnCancel;

            if (dlg.ShowDialog() != DialogResult.OK)
                return null;

            string fieldName = txtName.Text.Trim();
            if (string.IsNullOrEmpty(fieldName))
            {
                MessageBox.Show("字段名不能为空", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return null;
            }

            if (currentConfig.Fields.Any(f => f.Name == fieldName))
            {
                MessageBox.Show(string.Format("字段名\"{0}\"已存在", fieldName), "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return null;
            }

            var fd = new FieldDef();
            fd.Name = fieldName;
            fd.ChineseName = txtChinese.Text.Trim();
            fd.Type = cmbType.SelectedItem.ToString();
            fd.Comment = txtChinese.Text.Trim();
            fd.FieldRule = txtRule.Text.Trim();
            return fd;
        }

        private string GetDefaultValueForType(string type)
        {
            switch (type)
            {
                case "int": return "0";
                case "float": return "0";
                case "bool": return "false";
                case "string": return "";
                case "string[]": return "";
                case "int[]": return "";
                case "float[]": return "";
                default: return "";
            }
        }

        private void InsertColumn(FieldDef newField, int insertAtFieldIndex)
        {
            isLoading = true;
            try
            {
                currentConfig.Fields.Insert(insertAtFieldIndex, newField);

                string defaultValue = GetDefaultValueForType(newField.Type);

                dataTable.Columns.Add(newField.Name, typeof(string));
                dataTable.Columns[newField.Name].SetOrdinal(insertAtFieldIndex + 1);

                dataTable.Rows[0][newField.Name] = newField.IsIndex ? "★" + newField.Name : newField.Name;
                dataTable.Rows[1][newField.Name] = newField.ChineseName ?? newField.Comment ?? "";
                dataTable.Rows[2][newField.Name] = newField.Type;

                for (int i = HeaderRowCount; i < dataTable.Rows.Count; i++)
                {
                    if (dataTable.Rows[i]["_RowTag_"] as string == "Data")
                        dataTable.Rows[i][newField.Name] = defaultValue;
                }

                foreach (var row in currentConfig.Rows)
                {
                    if (!row.ContainsKey(newField.Name))
                        row[newField.Name] = defaultValue;
                }

                AdjustCellMetasAfterColumnInsert(insertAtFieldIndex);

                dataGridView1.DataSource = null;
                dataGridView1.DataSource = dataTable;
                SetupDataGridView();
                ApplyColors();
                dataGridView1.Invalidate();

                MarkCurrentConfigModified();
            }
            finally
            {
                isLoading = false;
            }
        }

        private void AdjustCellMetasAfterColumnInsert(int insertedFieldIndex)
        {
            int insertedColIndex = insertedFieldIndex + 1;

            foreach (var cm in currentConfig.CellMetas)
            {
                if (cm.Col >= insertedColIndex)
                    cm.Col++;
            }
        }

        private int GetSelectedFieldIndex()
        {
            if (dataGridView1.CurrentCell == null) return -1;
            int colIdx = dataGridView1.CurrentCell.ColumnIndex;
            if (colIdx <= 0) return -1;
            string colName = dataGridView1.Columns[colIdx].Name;
            for (int i = 0; i < currentConfig.Fields.Count; i++)
            {
                if (currentConfig.Fields[i].Name == colName)
                    return i;
            }
            return -1;
        }

        private void menuAddColLeft_Click(object sender, EventArgs e)
        {
            if (currentConfig == null || dataTable == null) return;

            int fieldIdx = GetSelectedFieldIndex();
            if (fieldIdx < 0)
            {
                MessageBox.Show("请先选中一个数据列", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var newField = ShowAddColumnDialog();
            if (newField == null) return;

            InsertColumn(newField, fieldIdx);
        }

        private void menuAddColRight_Click(object sender, EventArgs e)
        {
            if (currentConfig == null || dataTable == null) return;

            int fieldIdx = GetSelectedFieldIndex();
            if (fieldIdx < 0)
            {
                MessageBox.Show("请先选中一个数据列", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var newField = ShowAddColumnDialog();
            if (newField == null) return;

            InsertColumn(newField, fieldIdx + 1);
        }

        private void menuDeleteCol_Click(object sender, EventArgs e)
        {
            if (currentConfig == null || dataTable == null) return;

            int fieldIdx = GetSelectedFieldIndex();
            if (fieldIdx < 0)
            {
                MessageBox.Show("请先选中一个数据列", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string fieldName = currentConfig.Fields[fieldIdx].Name;
            if (MessageBox.Show(string.Format("确定要删除列\"{0}\"吗？", fieldName), "确认删除", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) != DialogResult.OK)
                return;

            isLoading = true;
            try
            {
                int colIndex = fieldIdx + 1;

                currentConfig.Fields.RemoveAt(fieldIdx);

                foreach (var row in currentConfig.Rows)
                {
                    if (row.ContainsKey(fieldName))
                        row.Remove(fieldName);
                }

                var metasToRemove = currentConfig.CellMetas.Where(cm => cm.Col == colIndex).ToList();
                foreach (var cm in metasToRemove)
                    currentConfig.CellMetas.Remove(cm);

                foreach (var cm in currentConfig.CellMetas)
                {
                    if (cm.Col > colIndex)
                        cm.Col--;
                }

                dataTable.Columns.Remove(fieldName);

                dataGridView1.DataSource = null;
                dataGridView1.DataSource = dataTable;
                SetupDataGridView();
                ApplyColors();
                dataGridView1.Invalidate();

                MarkCurrentConfigModified();
            }
            finally
            {
                isLoading = false;
            }
        }

        private void menuSetIndex_Click(object sender, EventArgs e)
        {
            if (currentConfig == null || dataTable == null) return;

            int fieldIdx = GetSelectedFieldIndex();
            if (fieldIdx < 0) return;

            currentConfig.Fields[fieldIdx].IsIndex = true;

            string colName = currentConfig.Fields[fieldIdx].Name;
            if (dataTable.Columns.Contains(colName))
            {
                dataTable.Rows[0][colName] = "★" + colName;
            }

            MarkCurrentConfigModified();
        }

        private void menuCancelIndex_Click(object sender, EventArgs e)
        {
            if (currentConfig == null || dataTable == null) return;

            int fieldIdx = GetSelectedFieldIndex();
            if (fieldIdx < 0) return;

            currentConfig.Fields[fieldIdx].IsIndex = false;

            string colName = currentConfig.Fields[fieldIdx].Name;
            if (dataTable.Columns.Contains(colName))
            {
                dataTable.Rows[0][colName] = colName;
            }

            MarkCurrentConfigModified();
        }

        private void contextMenuStrip1_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            int fieldIdx = GetSelectedFieldIndex();
            bool isDataColumn = fieldIdx >= 0;
            bool isIndexed = isDataColumn && currentConfig.Fields[fieldIdx].IsIndex;
            bool isIntColumn = isDataColumn && currentConfig.Fields[fieldIdx].Type.ToLower() == "int";

            menuSetIndex.Enabled = isDataColumn && !isIndexed;
            menuCancelIndex.Enabled = isDataColumn && isIndexed;
            menuMoveColLeft.Enabled = isDataColumn && fieldIdx > 0;
            menuMoveColRight.Enabled = isDataColumn && fieldIdx < currentConfig.Fields.Count - 1;
            menuViewDistribution.Enabled = isIntColumn;
        }

        private void menuMoveColLeft_Click(object sender, EventArgs e)
        {
            if (currentConfig == null || dataTable == null) return;
            int fieldIdx = GetSelectedFieldIndex();
            if (fieldIdx <= 0) return;
            MoveColumn(fieldIdx, fieldIdx - 1);
        }

        private void menuMoveColRight_Click(object sender, EventArgs e)
        {
            if (currentConfig == null || dataTable == null) return;
            int fieldIdx = GetSelectedFieldIndex();
            if (fieldIdx < 0 || fieldIdx >= currentConfig.Fields.Count - 1) return;
            MoveColumn(fieldIdx, fieldIdx + 1);
        }

        private void MoveColumn(int fromFieldIdx, int toFieldIdx)
        {
            isLoading = true;
            try
            {
                var temp = currentConfig.Fields[fromFieldIdx];
                currentConfig.Fields[fromFieldIdx] = currentConfig.Fields[toFieldIdx];
                currentConfig.Fields[toFieldIdx] = temp;

                int fromColIdx = fromFieldIdx + 1;
                int toColIdx = toFieldIdx + 1;

                dataTable.Columns[fromColIdx].SetOrdinal(toColIdx);

                foreach (var cm in currentConfig.CellMetas)
                {
                    if (cm.Col == fromColIdx)
                        cm.Col = toColIdx;
                    else if (cm.Col == toColIdx)
                        cm.Col = fromColIdx;
                }

                dataGridView1.DataSource = null;
                dataGridView1.DataSource = dataTable;
                SetupDataGridView();
                ApplyColors();
                dataGridView1.Invalidate();

                MarkCurrentConfigModified();
            }
            finally
            {
                isLoading = false;
            }
        }

        private void ApplyColors()
        {
            if (currentConfig == null || currentConfig.CellMetas == null || currentConfig.CellMetas.Count == 0) return;

            foreach (var cm in currentConfig.CellMetas)
            {
                int rowIdx = cm.Row + HeaderRowCount;
                int colIdx = cm.Col;

                if (rowIdx >= HeaderRowCount && rowIdx < dataGridView1.Rows.Count &&
                    colIdx >= 0 && colIdx < dataGridView1.Columns.Count)
                {
                    var cell = dataGridView1.Rows[rowIdx].Cells[colIdx];
                    if (cm.ForeColor.HasValue)
                        cell.Style.ForeColor = Color.FromArgb(cm.ForeColor.Value);
                    if (cm.BackColor.HasValue)
                        cell.Style.BackColor = Color.FromArgb(cm.BackColor.Value);
                }
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == 2 && e.ColumnIndex >= 0)
            {
                SortDataByColumn(e.ColumnIndex);
            }

            if (e.ColumnIndex >= 0 && e.ColumnIndex != selectedColumnIndex)
            {
                int oldSel = selectedColumnIndex;
                selectedColumnIndex = e.ColumnIndex;
                InvalidateColumnHeaders(oldSel);
                InvalidateColumnHeaders(selectedColumnIndex);
            }
        }

        private void InvalidateColumnHeaders(int colIdx)
        {
            if (colIdx < 0 || colIdx >= dataGridView1.Columns.Count) return;
            for (int r = 0; r < HeaderRowCount && r < dataGridView1.Rows.Count; r++)
            {
                dataGridView1.InvalidateCell(colIdx, r);
            }
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (isLoading || dataGridView1 == null) return;

            int oldRow = selectedRowIndex;

            if (dataGridView1.CurrentCell != null && dataGridView1.CurrentCell.RowIndex >= HeaderRowCount)
                selectedRowIndex = dataGridView1.CurrentCell.RowIndex;
            else
                selectedRowIndex = -1;

            if (oldRow != selectedRowIndex)
            {
                if (oldRow >= HeaderRowCount && oldRow < dataGridView1.Rows.Count && firstDataColIdx >= 0)
                    dataGridView1.InvalidateCell(firstDataColIdx, oldRow);
                if (selectedRowIndex >= HeaderRowCount && selectedRowIndex < dataGridView1.Rows.Count && firstDataColIdx >= 0)
                    dataGridView1.InvalidateCell(firstDataColIdx, selectedRowIndex);
            }
        }

        private void dataGridView1_Scroll(object sender, ScrollEventArgs e)
        {
            if (e.ScrollOrientation == ScrollOrientation.HorizontalScroll && firstDataColIdx >= 0)
            {
                int firstRow = Math.Max(0, dataGridView1.FirstDisplayedScrollingRowIndex);
                int lastRow = Math.Min(dataGridView1.Rows.Count - 1, firstRow + dataGridView1.DisplayedRowCount(true));
                for (int r = 0; r < HeaderRowCount && r < dataGridView1.Rows.Count; r++)
                    dataGridView1.InvalidateCell(firstDataColIdx, r);
                for (int r = firstRow; r <= lastRow; r++)
                    dataGridView1.InvalidateCell(firstDataColIdx, r);
            }
        }

        private void dataGridView1_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dataGridView1.IsCurrentCellDirty)
            {
                dataGridView1.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        private void dataGridView1_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
                {
                    dataGridView1.CurrentCell = dataGridView1[e.ColumnIndex, e.RowIndex];
                }
                if (e.RowIndex < HeaderRowCount)
                {
                    dataGridView1.ContextMenuStrip = contextMenuStrip1;
                }
                else
                {
                    dataGridView1.ContextMenuStrip = contextMenuStripCell;
                }
            }
        }

        private void menuDeleteRowCtx_Click(object sender, EventArgs e)
        {
            if (dataGridView1 == null || currentConfig == null) return;

            if (dataGridView1.SelectedCells.Count == 0)
            {
                MessageBox.Show("请先选中要删除的行中的单元格", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            HashSet<int> rowsToDelete = new HashSet<int>();
            foreach (DataGridViewCell cell in dataGridView1.SelectedCells)
            {
                if (cell.RowIndex >= HeaderRowCount && !cell.OwningRow.IsNewRow)
                    rowsToDelete.Add(cell.RowIndex);
            }

            if (rowsToDelete.Count == 0) return;

            if (MessageBox.Show(string.Format("确定要删除选中的{0}行吗？", rowsToDelete.Count), "确认删除", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) != DialogResult.OK)
                return;

            var sortedRows = rowsToDelete.OrderByDescending(r => r).ToList();
            foreach (int rowIdx in sortedRows)
            {
                dataGridView1.Rows.RemoveAt(rowIdx);
            }

            SyncDataTableToConfig();
            MarkCurrentConfigModified();
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= HeaderRowCount && e.ColumnIndex >= 0)
            {
                SyncDataTableToConfig();
                MarkCurrentConfigModified();
            }
        }

        private void dataGridView1_ColumnWidthChanged(object sender, DataGridViewColumnEventArgs e)
        {
            if (isLoading) return;
            SyncColumnWidthsToConfig();
            MarkCurrentConfigModified();
        }

        private void SortDataByColumn(int columnIndex)
        {
            if (dataTable == null || columnIndex < 0) return;
            if (!dataGridView1.Columns[columnIndex].Visible) return;

            string columnName = dataGridView1.Columns[columnIndex].Name;
            string fieldType = "";
            if (dataTable.Rows.Count > 2)
            {
                var typeRow = dataTable.Rows[2];
                fieldType = typeRow[columnIndex] != null ? typeRow[columnIndex].ToString().ToLower() : "";
            }

            if (sortedColumnIndex == columnIndex)
            {
                sortAscending = !sortAscending;
            }
            else
            {
                sortedColumnIndex = columnIndex;
                sortAscending = true;
            }

            List<object[]> headerRows = new List<object[]>();
            List<object[]> dataRows = new List<object[]>();

            foreach (DataRow row in dataTable.Rows)
            {
                string tag = row["_RowTag_"] as string;
                if (tag == "Data")
                    dataRows.Add(row.ItemArray);
                else
                    headerRows.Add(row.ItemArray);
            }

            dataRows.Sort((r1, r2) =>
            {
                string v1 = r1[columnIndex] == null ? "" : r1[columnIndex].ToString();
                string v2 = r2[columnIndex] == null ? "" : r2[columnIndex].ToString();
                
                int cmp;
                if (fieldType == "int")
                {
                    int i1, i2;
                    int.TryParse(v1, out i1);
                    int.TryParse(v2, out i2);
                    cmp = i1.CompareTo(i2);
                }
                else if (fieldType == "float")
                {
                    float f1, f2;
                    string s1 = v1.TrimEnd('f', 'F');
                    string s2 = v2.TrimEnd('f', 'F');
                    float.TryParse(s1, out f1);
                    float.TryParse(s2, out f2);
                    cmp = f1.CompareTo(f2);
                }
                else
                {
                    cmp = string.Compare(v1, v2, StringComparison.Ordinal);
                }
                return sortAscending ? cmp : -cmp;
            });

            dataTable.Rows.Clear();
            foreach (var rowData in headerRows)
                dataTable.Rows.Add(rowData);
            foreach (var rowData in dataRows)
                dataTable.Rows.Add(rowData);

            SetupDataGridView();
            ApplyColors();
            dataGridView1.Invalidate();
        }

        private void dataGridView1_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            if (e.RowIndex < HeaderRowCount)
            {
                bool isSelectedCol = e.ColumnIndex == selectedColumnIndex;
                int fieldIdx = GetFieldIndexFromColumnIndex(e.ColumnIndex);
                bool isIndexCol = fieldIdx >= 0 && fieldIdx < currentConfig.Fields.Count && currentConfig.Fields[fieldIdx].IsIndex;
                e.Paint(e.CellBounds, DataGridViewPaintParts.All & ~DataGridViewPaintParts.ContentForeground & ~DataGridViewPaintParts.Background);

                Color headerBg;
                if (isIndexCol)
                    headerBg = isSelectedCol ? Color.FromArgb(160, 140, 70) : Color.FromArgb(130, 110, 50);
                else
                    headerBg = isSelectedCol ? Color.FromArgb(90, 120, 160) : Color.FromArgb(70, 90, 115);
                using (Brush bgBrush = new SolidBrush(headerBg))
                {
                    e.Graphics.FillRectangle(bgBrush, e.CellBounds);
                }

                if (e.RowIndex == 2)
                {
                    string cellValue = e.Value != null ? e.Value.ToString() : "";
                    string displayText = cellValue;
                    if (e.ColumnIndex == sortedColumnIndex)
                    {
                        displayText += sortAscending ? " ▲" : " ▼";
                    }
                    using (Brush brush = new SolidBrush(Color.White))
                    {
                        SizeF textSize = e.Graphics.MeasureString(displayText, e.CellStyle.Font);
                        float x = e.CellBounds.Left + (e.CellBounds.Width - textSize.Width) / 2;
                        float y = e.CellBounds.Top + (e.CellBounds.Height - textSize.Height) / 2;
                        e.Graphics.DrawString(displayText, e.CellStyle.Font, brush, x, y);
                    }
                }
                else
                {
                    string cellValue = e.Value != null ? e.Value.ToString() : "";
                    using (Brush brush = new SolidBrush(Color.White))
                    {
                        StringFormat sf = new StringFormat { LineAlignment = StringAlignment.Center, Alignment = StringAlignment.Center };
                        e.Graphics.DrawString(cellValue, e.CellStyle.Font, brush, e.CellBounds, sf);
                    }
                }

                using (Pen borderPen = new Pen(Color.FromArgb(60, 60, 65)))
                {
                    e.Graphics.DrawRectangle(borderPen, new Rectangle(e.CellBounds.X, e.CellBounds.Y, e.CellBounds.Width - 1, e.CellBounds.Height - 1));
                }
                e.Handled = true;
                return;
            }

            if (e.RowIndex >= HeaderRowCount)
            {
                string cellValue = e.Value != null ? e.Value.ToString() : "";
                int fieldIdx = GetFieldIndexFromColumnIndex(e.ColumnIndex);
                string fieldType = fieldIdx >= 0 && fieldIdx < currentConfig.Fields.Count ? currentConfig.Fields[fieldIdx].Type.ToLower() : "";
                Color? colorValue = TryParseColorString(cellValue);
                bool isBoolTrue = cellValue == "true" || cellValue == "True";
                bool isBoolFalse = cellValue == "false" || cellValue == "False";
                bool isNumberType = fieldType == "int" || fieldType == "float";
                bool isFirstColHighlight = (e.ColumnIndex == firstDataColIdx && e.RowIndex == selectedRowIndex);

                if (colorValue.HasValue || isBoolTrue || isBoolFalse || isNumberType || isFirstColHighlight)
                {
                    Color bgColor = e.CellStyle.BackColor;
                    Color fgColor = e.CellStyle.ForeColor;

                    if (isFirstColHighlight)
                    {
                        bgColor = Color.FromArgb(80, 130, 200);
                    }

                    if (isNumberType && !isFirstColHighlight)
                    {
                        fgColor = Color.FromArgb(100, 220, 130);
                        if (fieldIdx >= 0 && fieldIdx < currentConfig.Fields.Count)
                        {
                            var fieldDef = currentConfig.Fields[fieldIdx];
                            Color? ruleColor = GetRuleColor(fieldDef.FieldRule, cellValue);
                            if (ruleColor.HasValue)
                            {
                                bgColor = ruleColor.Value;
                                fgColor = e.CellStyle.ForeColor;
                            }
                        }
                    }
                    using (Brush bgBrush = new SolidBrush(bgColor))
                    {
                        e.Graphics.FillRectangle(bgBrush, e.CellBounds);
                    }
                    if ((e.State & DataGridViewElementStates.Selected) == DataGridViewElementStates.Selected)
                    {
                        using (Brush selBrush = new SolidBrush(e.CellStyle.SelectionBackColor))
                        {
                            e.Graphics.FillRectangle(selBrush, e.CellBounds);
                        }
                    }

                    int barWidth = 4;
                    int barPadding = 2;
                    int iconSize = 12;
                    int leftOffset = barPadding;

                    if (colorValue.HasValue)
                    {
                        Rectangle barRect = new Rectangle(
                            e.CellBounds.Left + barPadding,
                            e.CellBounds.Top + barPadding,
                            barWidth,
                            e.CellBounds.Height - barPadding * 2
                        );
                        using (Brush brush = new SolidBrush(colorValue.Value))
                        {
                            e.Graphics.FillRectangle(brush, barRect);
                        }
                        leftOffset = barWidth + barPadding * 2;
                    }

                    if (isBoolTrue || isBoolFalse)
                    {
                        int iconX = e.CellBounds.Left + barPadding + 2;
                        int iconY = e.CellBounds.Top + (e.CellBounds.Height - iconSize) / 2;

                        if (isBoolTrue)
                        {
                            using (Pen greenPen = new Pen(Color.FromArgb(100, 220, 100), 2))
                            {
                                e.Graphics.DrawLine(greenPen, iconX, iconY + 6, iconX + 4, iconY + 10);
                                e.Graphics.DrawLine(greenPen, iconX + 4, iconY + 10, iconX + 10, iconY + 2);
                            }
                        }
                        else
                        {
                            using (Pen redPen = new Pen(Color.FromArgb(220, 80, 80), 2))
                            {
                                e.Graphics.DrawLine(redPen, iconX, iconY, iconX + iconSize, iconY + iconSize);
                                e.Graphics.DrawLine(redPen, iconX + iconSize, iconY, iconX, iconY + iconSize);
                            }
                        }
                        leftOffset = iconSize + barPadding * 2 + 4;
                    }

                    using (Brush textBrush = new SolidBrush(fgColor))
                    {
                        Rectangle textRect = new Rectangle(
                            e.CellBounds.Left + leftOffset,
                            e.CellBounds.Top,
                            e.CellBounds.Width - leftOffset,
                            e.CellBounds.Height
                        );
                        StringFormat sf = new StringFormat
                        {
                            LineAlignment = StringAlignment.Center,
                            Alignment = StringAlignment.Near
                        };
                        e.Graphics.DrawString(cellValue, e.CellStyle.Font, textBrush, textRect, sf);
                    }

                    using (Pen borderPen = new Pen(dataGridView1.GridColor))
                    {
                        e.Graphics.DrawRectangle(borderPen, new Rectangle(e.CellBounds.X, e.CellBounds.Y, e.CellBounds.Width - 1, e.CellBounds.Height - 1));
                    }
                    e.Handled = true;
                }
            }
        }

        private Color? GetRuleColor(string fieldRule, string cellValue)
        {
            if (string.IsNullOrEmpty(fieldRule) || string.IsNullOrEmpty(cellValue)) return null;

            double numValue;
            if (!double.TryParse(cellValue, out numValue)) return null;

            string[] rules = fieldRule.Split(',');
            foreach (string rule in rules)
            {
                string trimmed = rule.Trim();
                int colonIdx = trimmed.LastIndexOf(':');
                if (colonIdx < 0) continue;

                string rangePart = trimmed.Substring(0, colonIdx).Trim();
                string colorPart = trimmed.Substring(colonIdx + 1).Trim();

                Color? color = TryParseColorString(colorPart);
                if (!color.HasValue) continue;

                int dashIdx = rangePart.IndexOf('-');
                if (dashIdx >= 0)
                {
                    double rangeMin, rangeMax;
                    if (double.TryParse(rangePart.Substring(0, dashIdx).Trim(), out rangeMin) &&
                        double.TryParse(rangePart.Substring(dashIdx + 1).Trim(), out rangeMax))
                    {
                        if (numValue >= rangeMin && numValue <= rangeMax)
                            return color;
                    }
                }
                else
                {
                    double exactVal;
                    if (double.TryParse(rangePart, out exactVal))
                    {
                        if (numValue == exactVal)
                            return color;
                    }
                }
            }

            return null;
        }

        private Color? TryParseColorString(string value)
        {
            if (string.IsNullOrEmpty(value)) return null;
            if (value.Length != 7 && value.Length != 9 && value.Length != 4 && value.Length != 5) return null;
            if (value[0] != '#') return null;

            try
            {
                string hex = value.Substring(1);
                if (hex.Length == 3)
                {
                    int r = Convert.ToInt32(hex[0].ToString() + hex[0], 16);
                    int g = Convert.ToInt32(hex[1].ToString() + hex[1], 16);
                    int b = Convert.ToInt32(hex[2].ToString() + hex[2], 16);
                    return Color.FromArgb(r, g, b);
                }
                else if (hex.Length == 4)
                {
                    int r = Convert.ToInt32(hex[0].ToString() + hex[0], 16);
                    int g = Convert.ToInt32(hex[1].ToString() + hex[1], 16);
                    int b = Convert.ToInt32(hex[2].ToString() + hex[2], 16);
                    int a = Convert.ToInt32(hex[3].ToString() + hex[3], 16);
                    return Color.FromArgb(a, r, g, b);
                }
                else if (hex.Length == 6)
                {
                    int rgb = Convert.ToInt32(hex, 16);
                    return Color.FromArgb((rgb >> 16) & 0xFF, (rgb >> 8) & 0xFF, rgb & 0xFF);
                }
                else if (hex.Length == 8)
                {
                    int argb = Convert.ToInt32(hex, 16);
                    return Color.FromArgb((argb >> 24) & 0xFF, (argb >> 16) & 0xFF, (argb >> 8) & 0xFF, argb & 0xFF);
                }
            }
            catch
            {
                return null;
            }
            return null;
        }

        private void menuViewDistribution_Click(object sender, EventArgs e)
        {
            if (currentConfig == null || dataTable == null) return;

            int fieldIdx = GetSelectedFieldIndex();
            if (fieldIdx < 0) return;

            var field = currentConfig.Fields[fieldIdx];
            if (field.Type.ToLower() != "int") return;

            List<int> values = new List<int>();
            string colName = field.Name;
            foreach (DataRow row in dataTable.Rows)
            {
                if (row["_RowTag_"] as string != "Data") continue;
                string val = row[colName] == DBNull.Value ? "" : row[colName].ToString();
                int intVal;
                if (int.TryParse(val, out intVal))
                    values.Add(intVal);
            }

            if (values.Count == 0)
            {
                MessageBox.Show("该列没有有效的int数据", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (var distForm = new DistributionForm(field.ChineseName ?? field.Name, field.Name, values))
            {
                distForm.ShowDialog(this);
            }
        }

        private void dataGridView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (currentConfig == null || dataTable == null) return;

            if (e.KeyCode == Keys.Delete)
            {
                if (dataGridView1.SelectedCells.Count == 0) return;

                bool anyChanged = false;
                foreach (DataGridViewCell cell in dataGridView1.SelectedCells)
                {
                    if (cell.RowIndex >= HeaderRowCount && cell.ColumnIndex >= 0)
                    {
                        int fieldIdx = GetFieldIndexFromColumnIndex(cell.ColumnIndex);
                        if (fieldIdx >= 0 && fieldIdx < currentConfig.Fields.Count)
                        {
                            string fieldType = currentConfig.Fields[fieldIdx].Type;
                            string defaultValue = GetDefaultValueForType(fieldType);
                            cell.Value = defaultValue;
                            anyChanged = true;
                        }
                    }
                }

                if (anyChanged)
                {
                    SyncDataTableToConfig();
                    MarkCurrentConfigModified();
                }
                e.Handled = true;
            }
            else if (e.Control && e.KeyCode == Keys.C)
            {
                if (dataGridView1.SelectedCells.Count == 1)
                {
                    var cell = dataGridView1.SelectedCells[0];
                    if (cell.RowIndex >= HeaderRowCount && cell.ColumnIndex >= 0)
                    {
                        copiedCellValue = cell.Value != null ? cell.Value.ToString() : "";
                    }
                }
                e.Handled = true;
            }
            else if (e.Control && e.KeyCode == Keys.V)
            {
                if (copiedCellValue != null && dataGridView1.SelectedCells.Count > 1)
                {
                    bool anyChanged = false;
                    foreach (DataGridViewCell cell in dataGridView1.SelectedCells)
                    {
                        if (cell.RowIndex >= HeaderRowCount && cell.ColumnIndex >= 0)
                        {
                            int fieldIdx = GetFieldIndexFromColumnIndex(cell.ColumnIndex);
                            if (fieldIdx >= 0 && fieldIdx < currentConfig.Fields.Count)
                            {
                                string fieldType = currentConfig.Fields[fieldIdx].Type;
                                if (!ValidateInput(copiedCellValue, fieldType))
                                {
                                    MessageBox.Show(string.Format("值\"{0}\"不符合字段\"{1}\"的类型({2})", copiedCellValue, currentConfig.Fields[fieldIdx].Name, fieldType), "类型错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    return;
                                }
                            }
                        }
                    }

                    foreach (DataGridViewCell cell in dataGridView1.SelectedCells)
                    {
                        if (cell.RowIndex >= HeaderRowCount && cell.ColumnIndex >= 0)
                        {
                            cell.Value = copiedCellValue;
                            anyChanged = true;
                        }
                    }

                    if (anyChanged)
                    {
                        SyncDataTableToConfig();
                        MarkCurrentConfigModified();
                    }
                }
                e.Handled = true;
            }
        }
    }
}
