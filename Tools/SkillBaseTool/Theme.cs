using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Reflection;
using System.Windows.Forms;

namespace SkillBaseTool
{
    /// <summary>深色主题配色与绘制辅助（与 HeroBondTool 保持一致）。</summary>
    public static class Theme
    {
        public static readonly Color Bg = ColorTranslator.FromHtml("#14161A");
        public static readonly Color Card = ColorTranslator.FromHtml("#1D2127");
        public static readonly Color Card2 = ColorTranslator.FromHtml("#22272F");
        public static readonly Color Line = ColorTranslator.FromHtml("#2B313A");
        public static readonly Color Text = ColorTranslator.FromHtml("#E7EBF1");
        public static readonly Color Muted = ColorTranslator.FromHtml("#8B93A1");
        public static readonly Color Faint = ColorTranslator.FromHtml("#6F7785");
        public static readonly Color Accent = ColorTranslator.FromHtml("#D4A24A");
        public static readonly Color Danger = ColorTranslator.FromHtml("#E05A5A");
        public static readonly Color Ok = ColorTranslator.FromHtml("#7ED6A5");
        public static readonly Color Warn = ColorTranslator.FromHtml("#E0B15A");
        public static readonly Color RowBg = ColorTranslator.FromHtml("#161A20");
        public static readonly Color RowAlt = ColorTranslator.FromHtml("#1A1F26");
        public static readonly Color RowWarn = ColorTranslator.FromHtml("#2A2418");
        public static readonly Color RowOk = ColorTranslator.FromHtml("#182019");

        /// <summary>品质色：1灰 / 2绿 / 3蓝 / 4金（与配表染色一致）。</summary>
        public static Color QualityColor(int quality)
        {
            switch (quality)
            {
                case 1: return ColorTranslator.FromHtml("#8A8A8A");
                case 2: return ColorTranslator.FromHtml("#33CC33");
                case 3: return ColorTranslator.FromHtml("#5B7BFF");
                case 4: return ColorTranslator.FromHtml("#FFD700");
                default: return Muted;
            }
        }

        /// <summary>价格色：>13 红，>10 黄，其余常规色。</summary>
        public static Color PriceColor(double price)
        {
            if (price > 13)
                return Danger;
            if (price > 10)
                return Warn;
            return Text;
        }

        public static readonly Font Base = new Font("Microsoft YaHei UI", 9f);
        public static readonly Font Small = new Font("Microsoft YaHei UI", 8.25f);
        public static readonly Font Bold = new Font("Microsoft YaHei UI", 9f, FontStyle.Bold);
        public static readonly Font Title = new Font("Microsoft YaHei UI", 11f, FontStyle.Bold);
        public static readonly Font TitleBig = new Font("Microsoft YaHei UI", 13f, FontStyle.Bold);
        public static readonly Font Num = new Font("Microsoft YaHei UI", 16f, FontStyle.Bold);

        public static GraphicsPath RoundRect(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            if (rect.Width <= 0 || rect.Height <= 0)
                return path;
            int r = Math.Max(0, Math.Min(radius, Math.Min(rect.Width, rect.Height) / 2));
            path.AddArc(rect.X, rect.Y, r * 2, r * 2, 180, 90);
            path.AddArc(rect.Right - r * 2, rect.Y, r * 2, r * 2, 270, 90);
            path.AddArc(rect.Right - r * 2, rect.Bottom - r * 2, r * 2, r * 2, 0, 90);
            path.AddArc(rect.X, rect.Bottom - r * 2, r * 2, r * 2, 90, 90);
            path.CloseFigure();
            return path;
        }

        public static void FillRound(Graphics g, Rectangle rect, int radius, Color color)
        {
            using (var path = RoundRect(rect, radius))
            using (var brush = new SolidBrush(color))
                g.FillPath(brush, path);
        }

        public static void DrawCard(Graphics g, Rectangle rect, Color? back = null, Color? border = null)
        {
            int radius = 8;
            FillRound(g, rect, radius, back ?? Card);
            using (var path = RoundRect(rect, radius))
            using (var pen = new Pen(border ?? Line, 1f))
                g.DrawPath(pen, path);
        }

        /// <summary>统一的深色 DataGridView 样式。</summary>
        public static void StyleGrid(DataGridView grid)
        {
            grid.BackgroundColor = Bg;
            grid.BorderStyle = BorderStyle.None;
            grid.GridColor = Line;
            grid.EnableHeadersVisualStyles = false;
            grid.RowHeadersVisible = false;
            grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false;
            grid.AllowUserToResizeRows = false;
            grid.ReadOnly = true;
            grid.MultiSelect = false;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.ColumnHeadersDefaultCellStyle.BackColor = Card2;
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Muted;
            grid.ColumnHeadersDefaultCellStyle.Font = Bold;
            grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            grid.ColumnHeadersHeight = 30;
            grid.DefaultCellStyle.BackColor = RowBg;
            grid.DefaultCellStyle.ForeColor = Text;
            grid.DefaultCellStyle.SelectionBackColor = ColorTranslator.FromHtml("#2B3440");
            grid.DefaultCellStyle.SelectionForeColor = Text;
            grid.DefaultCellStyle.Font = Base;
            grid.RowTemplate.Height = 26;

            // 双缓冲：避免整表刷新时逐行重绘造成的闪烁与卡顿
            typeof(DataGridView).GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic)
                ?.SetValue(grid, true);
        }

        /// <summary>整表重填期间关闭自动列宽/挂起布局，填完再恢复，避免逐行触发布局重算。</summary>
        public static void BeginBulkFill(DataGridView grid)
        {
            grid.SuspendLayout();
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            grid.Rows.Clear();
        }

        public static void EndBulkFill(DataGridView grid)
        {
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.ResumeLayout();
        }
    }
}
