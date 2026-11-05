using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace HeroBondTool
{
    /// <summary>深色主题配色与绘制辅助。</summary>
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
        public static readonly Color DangerBg = ColorTranslator.FromHtml("#3A1D20");
        public static readonly Color Ok = ColorTranslator.FromHtml("#7ED6A5");
        public static readonly Color RowBg = ColorTranslator.FromHtml("#161A20");

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

        public static void DrawRoundBorder(Graphics g, Rectangle rect, int radius, Color color, float width = 1f)
        {
            using (var path = RoundRect(rect, radius))
            using (var pen = new Pen(color, width))
                g.DrawPath(pen, path);
        }

        public static void DrawCard(Graphics g, Rectangle rect, Color? back = null, Color? border = null)
        {
            int radius = 8;
            FillRound(g, rect, radius, back ?? Card);
            DrawRoundBorder(g, rect, radius, border ?? Line);
        }

        public static void DrawText(Graphics g, string text, Font font, Color color, int x, int y)
        {
            using (var brush = new SolidBrush(color))
                g.DrawString(text ?? "", font, brush, x, y, StringFormat.GenericTypographic);
        }

        public static void DrawTextRight(Graphics g, string text, Font font, Color color, int right, int y)
        {
            float width = TextWidth(g, text, font);
            using (var brush = new SolidBrush(color))
                g.DrawString(text ?? "", font, brush, right - width, y, StringFormat.GenericTypographic);
        }

        /// <summary>GDI+ 测量（必须与 DrawString 保持一致，才能配合滚动平移）。</summary>
        public static int MeasureText(string text, Font font, int width)
        {
            if (string.IsNullOrEmpty(text))
                return font.Height;
            using (var g = Graphics.FromHwnd(IntPtr.Zero))
            {
                SizeF size = g.MeasureString(text, font, new SizeF(width, 100000f),
                    StringFormat.GenericDefault);
                return (int)Math.Ceiling(size.Height);
            }
        }

        private static float TextWidth(Graphics g, string text, Font font)
        {
            if (string.IsNullOrEmpty(text))
                return 0f;
            return g.MeasureString(text, font, PointF.Empty, StringFormat.GenericTypographic).Width;
        }

        public static void DrawWrapped(Graphics g, string text, Font font, Color color, int x, int y, int width)
        {
            if (string.IsNullOrEmpty(text))
                return;
            using (var brush = new SolidBrush(color))
                g.DrawString(text, font, brush, new RectangleF(x, y, width, 100000f), StringFormat.GenericDefault);
        }

        public static void DrawEllipsis(Graphics g, string text, Font font, Color color, int x, int y, int width)
        {
            if (string.IsNullOrEmpty(text))
                return;
            var format = new StringFormat(StringFormat.GenericTypographic);
            format.Trimming = StringTrimming.EllipsisCharacter;
            format.FormatFlags = StringFormatFlags.NoWrap;
            using (var brush = new SolidBrush(color))
                g.DrawString(text, font, brush, new RectangleF(x, y, width, font.GetHeight() + 4), format);
            format.Dispose();
        }

        public static Color Mix(Color a, Color b, double ratioB)
        {
            double r = 1 - ratioB;
            return Color.FromArgb(
                (int)Math.Round(a.R * r + b.R * ratioB),
                (int)Math.Round(a.G * r + b.G * ratioB),
                (int)Math.Round(a.B * r + b.B * ratioB));
        }
    }
}
