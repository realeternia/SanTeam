namespace DesignCoder
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnRefresh = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.btnSave = new System.Windows.Forms.ToolStripButton();
            this.btnNewRow = new System.Windows.Forms.ToolStripButton();
            this.btnDeleteRow = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.btnBatchFill = new System.Windows.Forms.ToolStripButton();
            this.btnMultiply = new System.Windows.Forms.ToolStripButton();
            this.btnForeColor = new System.Windows.Forms.ToolStripButton();
            this.btnBackColor = new System.Windows.Forms.ToolStripButton();
            this.btnClearColors = new System.Windows.Forms.ToolStripButton();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.listView1 = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menuAddColLeft = new System.Windows.Forms.ToolStripMenuItem();
            this.menuAddColRight = new System.Windows.Forms.ToolStripMenuItem();
            this.menuDeleteCol = new System.Windows.Forms.ToolStripMenuItem();
            this.menuSetIndex = new System.Windows.Forms.ToolStripMenuItem();
            this.menuCancelIndex = new System.Windows.Forms.ToolStripMenuItem();
            this.menuMoveColLeft = new System.Windows.Forms.ToolStripMenuItem();
            this.menuMoveColRight = new System.Windows.Forms.ToolStripMenuItem();
            this.menuViewDistribution = new System.Windows.Forms.ToolStripMenuItem();
            this.menuBatchFill = new System.Windows.Forms.ToolStripMenuItem();
            this.menuForeColor = new System.Windows.Forms.ToolStripMenuItem();
            this.menuBackColor = new System.Windows.Forms.ToolStripMenuItem();
            this.menuClearColors = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStripCell = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menuDeleteRowCtx = new System.Windows.Forms.ToolStripMenuItem();
            this.menuCellBatchFill = new System.Windows.Forms.ToolStripMenuItem();
            this.menuCellMultiply = new System.Windows.Forms.ToolStripMenuItem();
            this.menuCellForeColor = new System.Windows.Forms.ToolStripMenuItem();
            this.menuCellBackColor = new System.Windows.Forms.ToolStripMenuItem();
            this.menuCellClearColors = new System.Windows.Forms.ToolStripMenuItem();
            this.colorDialog1 = new System.Windows.Forms.ColorDialog();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            this.contextMenuStripCell.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(184)))), ((int)(((byte)(217)))));
            this.toolStrip1.Font = new System.Drawing.Font("微软雅黑", 10F);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnRefresh,
            this.toolStripSeparator1,
            this.btnSave,
            this.btnNewRow,
            this.btnDeleteRow,
            this.toolStripSeparator2,
            this.btnBatchFill,
            this.btnMultiply,
            this.btnForeColor,
            this.btnBackColor,
            this.btnClearColors});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Padding = new System.Windows.Forms.Padding(5, 0, 2, 0);
            this.toolStrip1.Size = new System.Drawing.Size(1399, 27);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // btnRefresh
            // 
            this.btnRefresh.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnRefresh.Font = new System.Drawing.Font("微软雅黑", 10F);
            this.btnRefresh.ForeColor = System.Drawing.Color.White;
            this.btnRefresh.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(41, 24);
            this.btnRefresh.Text = "刷新";
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 27);
            // 
            // btnSave
            // 
            this.btnSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnSave.Font = new System.Drawing.Font("微软雅黑", 10F);
            this.btnSave.ForeColor = System.Drawing.Color.White;
            this.btnSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(41, 24);
            this.btnSave.Text = "保存";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnNewRow
            // 
            this.btnNewRow.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnNewRow.Font = new System.Drawing.Font("微软雅黑", 10F);
            this.btnNewRow.ForeColor = System.Drawing.Color.White;
            this.btnNewRow.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnNewRow.Name = "btnNewRow";
            this.btnNewRow.Size = new System.Drawing.Size(55, 24);
            this.btnNewRow.Text = "新增行";
            this.btnNewRow.Click += new System.EventHandler(this.btnNewRow_Click);
            // 
            // btnDeleteRow
            // 
            this.btnDeleteRow.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnDeleteRow.Font = new System.Drawing.Font("微软雅黑", 10F);
            this.btnDeleteRow.ForeColor = System.Drawing.Color.White;
            this.btnDeleteRow.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnDeleteRow.Name = "btnDeleteRow";
            this.btnDeleteRow.Size = new System.Drawing.Size(55, 24);
            this.btnDeleteRow.Text = "删除行";
            this.btnDeleteRow.Click += new System.EventHandler(this.btnDeleteRow_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 27);
            // 
            // btnBatchFill
            // 
            this.btnBatchFill.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnBatchFill.Font = new System.Drawing.Font("微软雅黑", 10F);
            this.btnBatchFill.ForeColor = System.Drawing.Color.White;
            this.btnBatchFill.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnBatchFill.Name = "btnBatchFill";
            this.btnBatchFill.Size = new System.Drawing.Size(69, 24);
            this.btnBatchFill.Text = "批量填充";
            this.btnBatchFill.Click += new System.EventHandler(this.btnBatchFill_Click);
            // 
            // btnMultiply
            // 
            this.btnMultiply.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnMultiply.Font = new System.Drawing.Font("微软雅黑", 10F);
            this.btnMultiply.ForeColor = System.Drawing.Color.White;
            this.btnMultiply.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnMultiply.Name = "btnMultiply";
            this.btnMultiply.Size = new System.Drawing.Size(41, 24);
            this.btnMultiply.Text = "成倍";
            this.btnMultiply.Click += new System.EventHandler(this.btnMultiply_Click);
            // 
            // btnForeColor
            // 
            this.btnForeColor.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnForeColor.Font = new System.Drawing.Font("微软雅黑", 10F);
            this.btnForeColor.ForeColor = System.Drawing.Color.White;
            this.btnForeColor.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnForeColor.Name = "btnForeColor";
            this.btnForeColor.Size = new System.Drawing.Size(55, 24);
            this.btnForeColor.Text = "前景色";
            this.btnForeColor.Click += new System.EventHandler(this.btnForeColor_Click);
            // 
            // btnBackColor
            // 
            this.btnBackColor.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnBackColor.Font = new System.Drawing.Font("微软雅黑", 10F);
            this.btnBackColor.ForeColor = System.Drawing.Color.White;
            this.btnBackColor.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnBackColor.Name = "btnBackColor";
            this.btnBackColor.Size = new System.Drawing.Size(55, 24);
            this.btnBackColor.Text = "背景色";
            this.btnBackColor.Click += new System.EventHandler(this.btnBackColor_Click);
            // 
            // btnClearColors
            // 
            this.btnClearColors.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnClearColors.Font = new System.Drawing.Font("微软雅黑", 10F);
            this.btnClearColors.ForeColor = System.Drawing.Color.White;
            this.btnClearColors.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnClearColors.Name = "btnClearColors";
            this.btnClearColors.Size = new System.Drawing.Size(69, 24);
            this.btnClearColors.Text = "清除颜色";
            this.btnClearColors.Click += new System.EventHandler(this.btnClearColors_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.IsSplitterFixed = true;
            this.splitContainer1.Location = new System.Drawing.Point(0, 27);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.listView1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.dataGridView1);
            this.splitContainer1.Size = new System.Drawing.Size(1399, 974);
            this.splitContainer1.SplitterDistance = 180;
            this.splitContainer1.TabIndex = 0;
            // 
            // listView1
            // 
            this.listView1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(252)))), ((int)(((byte)(255)))));
            this.listView1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.listView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView1.Font = new System.Drawing.Font("微软雅黑", 10F);
            this.listView1.FullRowSelect = true;
            this.listView1.GridLines = true;
            this.listView1.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.listView1.HideSelection = false;
            this.listView1.Location = new System.Drawing.Point(0, 0);
            this.listView1.MultiSelect = false;
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(180, 974);
            this.listView1.TabIndex = 0;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            this.listView1.SelectedIndexChanged += new System.EventHandler(this.listView1_SelectedIndexChanged);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "配置表";
            this.columnHeader1.Width = 160;
            // 
            // dataGridView1
            // 
            this.dataGridView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.ContextMenuStrip = this.contextMenuStrip1;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(0, 0);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersWidth = 30;
            this.dataGridView1.RowTemplate.Height = 26;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dataGridView1.Size = new System.Drawing.Size(1215, 974);
            this.dataGridView1.TabIndex = 0;
            this.dataGridView1.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellClick);
            this.dataGridView1.CellMouseDown += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridView1_CellMouseDown);
            this.dataGridView1.CellPainting += new System.Windows.Forms.DataGridViewCellPaintingEventHandler(this.dataGridView1_CellPainting);
            this.dataGridView1.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellValueChanged);
            this.dataGridView1.ColumnWidthChanged += new System.Windows.Forms.DataGridViewColumnEventHandler(this.dataGridView1_ColumnWidthChanged);
            this.dataGridView1.CurrentCellDirtyStateChanged += new System.EventHandler(this.dataGridView1_CurrentCellDirtyStateChanged);
            this.dataGridView1.Scroll += new System.Windows.Forms.ScrollEventHandler(this.dataGridView1_Scroll);
            this.dataGridView1.SelectionChanged += new System.EventHandler(this.dataGridView1_SelectionChanged);
            this.dataGridView1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dataGridView1_KeyDown);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuAddColLeft,
            this.menuAddColRight,
            this.menuDeleteCol,
            this.menuSetIndex,
            this.menuCancelIndex,
            this.menuMoveColLeft,
            this.menuMoveColRight,
            this.menuViewDistribution});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(137, 180);
            this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
            // 
            // menuAddColLeft
            // 
            this.menuAddColLeft.Name = "menuAddColLeft";
            this.menuAddColLeft.Size = new System.Drawing.Size(136, 22);
            this.menuAddColLeft.Text = "左加一列";
            this.menuAddColLeft.Click += new System.EventHandler(this.menuAddColLeft_Click);
            // 
            // menuAddColRight
            // 
            this.menuAddColRight.Name = "menuAddColRight";
            this.menuAddColRight.Size = new System.Drawing.Size(136, 22);
            this.menuAddColRight.Text = "右加一列";
            this.menuAddColRight.Click += new System.EventHandler(this.menuAddColRight_Click);
            // 
            // menuDeleteCol
            // 
            this.menuDeleteCol.Name = "menuDeleteCol";
            this.menuDeleteCol.Size = new System.Drawing.Size(136, 22);
            this.menuDeleteCol.Text = "删除此列";
            this.menuDeleteCol.Click += new System.EventHandler(this.menuDeleteCol_Click);
            // 
            // menuSetIndex
            // 
            this.menuSetIndex.Name = "menuSetIndex";
            this.menuSetIndex.Size = new System.Drawing.Size(136, 22);
            this.menuSetIndex.Text = "设为索引列";
            this.menuSetIndex.Click += new System.EventHandler(this.menuSetIndex_Click);
            // 
            // menuCancelIndex
            // 
            this.menuCancelIndex.Name = "menuCancelIndex";
            this.menuCancelIndex.Size = new System.Drawing.Size(136, 22);
            this.menuCancelIndex.Text = "取消索引列";
            this.menuCancelIndex.Click += new System.EventHandler(this.menuCancelIndex_Click);
            // 
            // menuMoveColLeft
            // 
            this.menuMoveColLeft.Name = "menuMoveColLeft";
            this.menuMoveColLeft.Size = new System.Drawing.Size(136, 22);
            this.menuMoveColLeft.Text = "左移列";
            this.menuMoveColLeft.Click += new System.EventHandler(this.menuMoveColLeft_Click);
            // 
            // menuMoveColRight
            // 
            this.menuMoveColRight.Name = "menuMoveColRight";
            this.menuMoveColRight.Size = new System.Drawing.Size(136, 22);
            this.menuMoveColRight.Text = "右移列";
            this.menuMoveColRight.Click += new System.EventHandler(this.menuMoveColRight_Click);
            // 
            // menuViewDistribution
            // 
            this.menuViewDistribution.Name = "menuViewDistribution";
            this.menuViewDistribution.Size = new System.Drawing.Size(136, 22);
            this.menuViewDistribution.Text = "查看分布";
            this.menuViewDistribution.Click += new System.EventHandler(this.menuViewDistribution_Click);
            // 
            // menuBatchFill
            // 
            this.menuBatchFill.Name = "menuBatchFill";
            this.menuBatchFill.Size = new System.Drawing.Size(136, 22);
            this.menuBatchFill.Text = "批量填充";
            this.menuBatchFill.Click += new System.EventHandler(this.btnBatchFill_Click);
            // 
            // menuForeColor
            // 
            this.menuForeColor.Name = "menuForeColor";
            this.menuForeColor.Size = new System.Drawing.Size(136, 22);
            this.menuForeColor.Text = "设置前景色";
            this.menuForeColor.Click += new System.EventHandler(this.btnForeColor_Click);
            // 
            // menuBackColor
            // 
            this.menuBackColor.Name = "menuBackColor";
            this.menuBackColor.Size = new System.Drawing.Size(136, 22);
            this.menuBackColor.Text = "设置背景色";
            this.menuBackColor.Click += new System.EventHandler(this.btnBackColor_Click);
            // 
            // menuClearColors
            // 
            this.menuClearColors.Name = "menuClearColors";
            this.menuClearColors.Size = new System.Drawing.Size(136, 22);
            this.menuClearColors.Text = "清除颜色";
            this.menuClearColors.Click += new System.EventHandler(this.btnClearColors_Click);
            // 
            // contextMenuStripCell
            // 
            this.contextMenuStripCell.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.contextMenuStripCell.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuDeleteRowCtx,
            this.menuCellBatchFill,
            this.menuCellMultiply,
            this.menuCellForeColor,
            this.menuCellBackColor,
            this.menuCellClearColors});
            this.contextMenuStripCell.Name = "contextMenuStripCell";
            this.contextMenuStripCell.Size = new System.Drawing.Size(137, 136);
            // 
            // menuDeleteRowCtx
            // 
            this.menuDeleteRowCtx.Name = "menuDeleteRowCtx";
            this.menuDeleteRowCtx.Size = new System.Drawing.Size(136, 22);
            this.menuDeleteRowCtx.Text = "删除整行";
            this.menuDeleteRowCtx.Click += new System.EventHandler(this.menuDeleteRowCtx_Click);
            // 
            // menuCellBatchFill
            // 
            this.menuCellBatchFill.Name = "menuCellBatchFill";
            this.menuCellBatchFill.Size = new System.Drawing.Size(136, 22);
            this.menuCellBatchFill.Text = "批量填充";
            this.menuCellBatchFill.Click += new System.EventHandler(this.btnBatchFill_Click);
            // 
            // menuCellMultiply
            // 
            this.menuCellMultiply.Name = "menuCellMultiply";
            this.menuCellMultiply.Size = new System.Drawing.Size(136, 22);
            this.menuCellMultiply.Text = "成倍";
            this.menuCellMultiply.Click += new System.EventHandler(this.btnMultiply_Click);
            // 
            // menuCellForeColor
            // 
            this.menuCellForeColor.Name = "menuCellForeColor";
            this.menuCellForeColor.Size = new System.Drawing.Size(136, 22);
            this.menuCellForeColor.Text = "设置前景色";
            this.menuCellForeColor.Click += new System.EventHandler(this.btnForeColor_Click);
            // 
            // menuCellBackColor
            // 
            this.menuCellBackColor.Name = "menuCellBackColor";
            this.menuCellBackColor.Size = new System.Drawing.Size(136, 22);
            this.menuCellBackColor.Text = "设置背景色";
            this.menuCellBackColor.Click += new System.EventHandler(this.btnBackColor_Click);
            // 
            // menuCellClearColors
            // 
            this.menuCellClearColors.Name = "menuCellClearColors";
            this.menuCellClearColors.Size = new System.Drawing.Size(136, 22);
            this.menuCellClearColors.Text = "清除颜色";
            this.menuCellClearColors.Click += new System.EventHandler(this.btnClearColors_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(244)))), ((int)(((byte)(248)))));
            this.ClientSize = new System.Drawing.Size(1399, 1001);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.toolStrip1);
            this.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "ConfigCoder";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            this.contextMenuStripCell.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton btnRefresh;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton btnSave;
        private System.Windows.Forms.ToolStripButton btnNewRow;
        private System.Windows.Forms.ToolStripButton btnDeleteRow;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton btnBatchFill;
        private System.Windows.Forms.ToolStripButton btnMultiply;
        private System.Windows.Forms.ToolStripButton btnForeColor;
        private System.Windows.Forms.ToolStripButton btnBackColor;
        private System.Windows.Forms.ToolStripButton btnClearColors;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem menuBatchFill;
        private System.Windows.Forms.ToolStripMenuItem menuForeColor;
        private System.Windows.Forms.ToolStripMenuItem menuBackColor;
        private System.Windows.Forms.ToolStripMenuItem menuClearColors;
        private System.Windows.Forms.ToolStripMenuItem menuAddColLeft;
        private System.Windows.Forms.ToolStripMenuItem menuAddColRight;
        private System.Windows.Forms.ToolStripMenuItem menuDeleteCol;
        private System.Windows.Forms.ToolStripMenuItem menuSetIndex;
        private System.Windows.Forms.ToolStripMenuItem menuCancelIndex;
        private System.Windows.Forms.ToolStripMenuItem menuMoveColLeft;
        private System.Windows.Forms.ToolStripMenuItem menuMoveColRight;
        private System.Windows.Forms.ToolStripMenuItem menuViewDistribution;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripCell;
        private System.Windows.Forms.ToolStripMenuItem menuDeleteRowCtx;
        private System.Windows.Forms.ToolStripMenuItem menuCellBatchFill;
        private System.Windows.Forms.ToolStripMenuItem menuCellMultiply;
        private System.Windows.Forms.ToolStripMenuItem menuCellForeColor;
        private System.Windows.Forms.ToolStripMenuItem menuCellBackColor;
        private System.Windows.Forms.ToolStripMenuItem menuCellClearColors;
        private System.Windows.Forms.ColorDialog colorDialog1;
    }
}
