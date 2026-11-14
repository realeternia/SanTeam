using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace SkillBaseTool
{
    /// <summary>模板 / 组件视角：treemap 布局，大方块=模板或组件，内部小方块=使用它的英雄技能（品质染色 + 价格）。</summary>
    public sealed class TreemapView : UserControl
    {
        public enum Mode
        {
            Template,
            Component,
        }

        private readonly Mode mode;
        private Label lblSummary;
        private Canvas canvas;
        private readonly ToolTip tooltip = new ToolTip();
        private DataStore store;
        private List<Block> blocks = new List<Block>();
        private Item tipItem;
        private Size layoutSize;
        private bool layoutDirty = true;

        public TreemapView(Mode mode)
        {
            this.mode = mode;
            BackColor = Theme.Bg;
            BuildUi();
        }

        private void BuildUi()
        {
            var top = new Panel { Dock = DockStyle.Top, Height = 54, BackColor = Theme.Card, Padding = new Padding(16, 8, 16, 8) };
            lblSummary = new Label
            {
                Text = "",
                Font = Theme.Base,
                ForeColor = Theme.Muted,
                AutoSize = true,
                Location = new Point(16, 8),
            };
            var legend = new Label
            {
                Text = "颜色 = 品质（灰 Q1 / 绿 Q2 / 蓝 Q3 / 金 Q4）；大方块面积 = 技能数；小方块 Q 标签后为价格",
                Font = Theme.Small,
                ForeColor = Theme.Faint,
                AutoSize = true,
                Location = new Point(16, 30),
            };
            top.Controls.AddRange(new Control[] { lblSummary, legend });

            canvas = new Canvas(this) { Dock = DockStyle.Fill };
            canvas.MouseMove += OnCanvasMouseMove;
            canvas.MouseLeave += (s, e) => ClearTip();

            Controls.Add(canvas);
            Controls.Add(top);
        }

        public void Bind(DataStore data)
        {
            store = data;
            RebuildBlocks();
            ClearTip();
            layoutDirty = true;
            canvas.Invalidate();
        }

        /// <summary>按模板 / 组件分组重建大方块（只统计 Q1–Q2 英雄技能）。</summary>
        private void RebuildBlocks()
        {
            blocks = new List<Block>();
            if (store == null)
            {
                lblSummary.Text = "";
                return;
            }
            var skills = store.Quality12HeroSkills.ToList();
            if (mode == Mode.Template)
            {
                foreach (var tpl in store.Templates)
                {
                    var used = skills.Where(s => s.TemplateName == tpl.Name).ToList();
                    if (used.Count > 0)
                        blocks.Add(MakeBlock(tpl.Name, tpl.Quality, tpl.Score, used, true));
                }
                int unassigned = skills.Count(s => string.IsNullOrEmpty(s.TemplateName));
                lblSummary.Text = string.Format("模板视角：{0} 个模板 / {1} 个 Q1–Q2 英雄技能（未模板化 {2}）",
                    blocks.Count, skills.Count, unassigned);
            }
            else
            {
                foreach (var comp in store.Components)
                {
                    var used = skills.Where(s => s.Modules.Contains(comp.Name)).ToList();
                    if (used.Count > 0)
                        blocks.Add(MakeBlock(comp.Name, comp.Quality, comp.Score, used, false));
                }
                lblSummary.Text = string.Format("组件视角：{0} 个组件 / {1} 个 Q1–Q2 英雄技能（一个技能可同时出现在多个组件方块中）",
                    blocks.Count, skills.Count);
            }
            // treemap 要求按面积（技能数）降序
            blocks = blocks.OrderByDescending(b => b.Count).ThenByDescending(b => b.Score).ToList();
        }

        private Block MakeBlock(string title, int quality, double score, List<SkillRow> skills, bool isTemplate)
        {
            var block = new Block { Title = title, Color = Theme.QualityColor(quality), Score = score };
            foreach (var skill in skills)
                block.Items.Add(new Item { Skill = skill, Block = block, Price = SkillGen.CalcPrice(store, skill) });
            block.Items = block.Items
                .OrderByDescending(i => i.Skill.Quality)
                .ThenByDescending(i => i.Price)
                .ToList();
            block.Count = block.Items.Count;
            double average = block.Items.Average(i => i.Price);
            block.Meta = isTemplate
                ? string.Format("×{0} · 模板分 {1} · 均价 {2}", block.Count, PriceText(score), PriceText(average))
                : string.Format("×{0} · 单价 {1} · 均价 {2}", block.Count, PriceText(score), PriceText(average));
            return block;
        }

        private void OnCanvasMouseMove(object sender, MouseEventArgs e)
        {
            Item hit = null;
            foreach (var block in blocks)
            {
                foreach (var item in block.Items)
                {
                    if (item.Rect.Width > 0 && item.Rect.Contains(e.Location))
                    {
                        hit = item;
                        break;
                    }
                }
                if (hit != null)
                    break;
            }
            if (hit == tipItem)
                return;
            tipItem = hit;
            tooltip.SetToolTip(canvas, hit == null ? "" : BuildTip(hit));
        }

        private void ClearTip()
        {
            tipItem = null;
            tooltip.SetToolTip(canvas, "");
        }

        private string BuildTip(Item item)
        {
            var s = item.Skill;
            var lines = new List<string>
            {
                string.Format("{0} · {1}（Q{2}）", s.Sname, s.Name, s.Quality),
                (mode == Mode.Template ? "模板：" : "组件：") + item.Block.Title,
                "价格：" + PriceText(item.Price),
                "组件：" + (s.Modules.Length > 0 ? string.Join("、", s.Modules) : "无"),
            };
            return string.Join(Environment.NewLine, lines);
        }

        private static string PriceText(double score)
        {
            return (score >= 0 ? "+" : "") + score.ToString("0.##");
        }

        #region 布局

        private void PaintCanvas(Graphics g, Size size)
        {
            g.Clear(Theme.Bg);
            if (store == null || size.Width <= 0 || size.Height <= 0)
                return;
            if (layoutDirty || layoutSize != size)
            {
                LayoutBlocks(new RectangleF(0, 0, size.Width, size.Height));
                layoutSize = size;
                layoutDirty = false;
            }
            foreach (var block in blocks)
                DrawBlock(g, block);
        }

        private void LayoutBlocks(RectangleF bounds)
        {
            if (blocks.Count == 0)
                return;
            var values = blocks.Select(b => (double)Math.Max(1, b.Count)).ToList();
            var rects = new List<RectangleF>();
            Squarify(values, bounds, rects);
            for (int i = 0; i < blocks.Count; i++)
            {
                var block = blocks[i];
                block.Rect = i < rects.Count ? rects[i] : RectangleF.Empty;
                LayoutItems(block);
            }
        }

        /// <summary>方块内小方块：按可用宽高比自动选列数，尽量接近正方形。</summary>
        private static void LayoutItems(Block block)
        {
            const float header = 26f;
            const float pad = 6f;
            const float gap = 4f;
            var inner = new RectangleF(block.Rect.X + pad, block.Rect.Y + header,
                block.Rect.Width - pad * 2, block.Rect.Height - header - pad);
            int n = block.Items.Count;
            if (n == 0 || inner.Width <= 0 || inner.Height <= 0)
            {
                foreach (var item in block.Items)
                    item.Rect = RectangleF.Empty;
                return;
            }
            int cols = (int)Math.Ceiling(Math.Sqrt(n * inner.Width / inner.Height));
            cols = Math.Max(1, Math.Min(n, cols));
            int rows = (int)Math.Ceiling(n / (double)cols);
            float cellW = inner.Width / cols;
            float cellH = inner.Height / rows;
            for (int i = 0; i < n; i++)
            {
                int r = i / cols;
                int c = i % cols;
                block.Items[i].Rect = new RectangleF(inner.X + c * cellW, inner.Y + r * cellH,
                    Math.Max(0, cellW - gap), Math.Max(0, cellH - gap));
            }
        }

        /// <summary>squarified treemap：values 需降序，输出面积成比例的矩形。</summary>
        private static void Squarify(IList<double> values, RectangleF bounds, IList<RectangleF> result)
        {
            int n = values.Count;
            if (n == 0 || bounds.Width <= 0 || bounds.Height <= 0)
                return;
            double total = values.Sum();
            if (total <= 0)
                return;
            double factor = bounds.Width * (double)bounds.Height / total;
            var areas = values.Select(v => Math.Max(v, 0.0001) * factor).ToList();

            var free = bounds;
            int idx = 0;
            while (idx < n)
            {
                if (free.Width <= 0.5f || free.Height <= 0.5f)
                {
                    while (idx < n)
                    {
                        result.Add(RectangleF.Empty);
                        idx++;
                    }
                    break;
                }
                double side = Math.Min(free.Width, free.Height);
                var row = new List<double> { areas[idx] };
                idx++;
                while (idx < n && Worst(row, side) >= WorstWith(row, areas[idx], side))
                {
                    row.Add(areas[idx]);
                    idx++;
                }
                free = LayoutRow(row, free, result);
            }
        }

        private static double Worst(List<double> row, double side)
        {
            double sum = row.Sum();
            if (sum <= 0 || side <= 0)
                return double.MaxValue;
            double max = row.Max();
            double min = row.Min();
            double s2 = sum * sum;
            double w2 = side * side;
            return Math.Max(w2 * max / s2, s2 / (w2 * min));
        }

        private static double WorstWith(List<double> row, double value, double side)
        {
            var copy = new List<double>(row) { value };
            return Worst(copy, side);
        }

        /// <summary>把一行铺到剩余矩形的最短边上（W≥H 竖条，否则横条）。</summary>
        private static RectangleF LayoutRow(List<double> row, RectangleF free, IList<RectangleF> result)
        {
            double rowSum = row.Sum();
            if (rowSum <= 0)
                return free;
            if (free.Width >= free.Height)
            {
                double thickness = Math.Min(free.Width, rowSum / free.Height);
                if (thickness <= 0)
                    return RectangleF.Empty;
                double y = free.Y;
                foreach (double area in row)
                {
                    double h = Math.Min(free.Bottom - y, area / thickness);
                    result.Add(new RectangleF(free.X, (float)y, (float)thickness, (float)Math.Max(0, h)));
                    y += h;
                }
                return new RectangleF(free.X + (float)thickness, free.Y,
                    (float)Math.Max(0, free.Width - thickness), free.Height);
            }
            else
            {
                double thickness = Math.Min(free.Height, rowSum / free.Width);
                if (thickness <= 0)
                    return RectangleF.Empty;
                double x = free.X;
                foreach (double area in row)
                {
                    double w = Math.Min(free.Right - x, area / thickness);
                    result.Add(new RectangleF((float)x, free.Y, (float)Math.Max(0, w), (float)thickness));
                    x += w;
                }
                return new RectangleF(free.X, free.Y + (float)thickness,
                    free.Width, (float)Math.Max(0, free.Height - thickness));
            }
        }

        #endregion

        #region 绘制

        private void DrawBlock(Graphics g, Block block)
        {
            var rect = Rectangle.Round(block.Rect);
            if (rect.Width < 24 || rect.Height < 24)
                return;
            Theme.DrawCard(g, rect, Theme.Card, Theme.Line);

            const TextFormatFlags textFlags = TextFormatFlags.VerticalCenter
                | TextFormatFlags.SingleLine | TextFormatFlags.NoPadding;
            int metaWidth = TextRenderer.MeasureText(g, block.Meta, Theme.Small, Size.Empty, textFlags).Width;
            // 方块太窄时只留标题，避免标题与计数挤在一起
            bool showMeta = rect.Width - 16 - metaWidth - 10 >= 40;
            int titleWidth = showMeta ? rect.Width - 16 - metaWidth - 10 : rect.Width - 16;
            if (titleWidth > 0)
            {
                var titleRect = new Rectangle(rect.X + 8, rect.Y + 3, titleWidth, 20);
                TextRenderer.DrawText(g, block.Title, Theme.Bold, titleRect, block.Color,
                    textFlags | TextFormatFlags.Left | TextFormatFlags.EndEllipsis);
            }
            if (showMeta)
            {
                var metaRect = new Rectangle(rect.Right - 8 - metaWidth, rect.Y + 3, metaWidth, 20);
                TextRenderer.DrawText(g, block.Meta, Theme.Small, metaRect, Theme.Accent,
                    textFlags | TextFormatFlags.Right);
            }
            if (rect.Width >= 40)
            {
                using (var pen = new Pen(Theme.Line))
                    g.DrawLine(pen, rect.X + 6, rect.Y + 24, rect.Right - 7, rect.Y + 24);
            }

            foreach (var item in block.Items)
                DrawItem(g, item);
        }

        private void DrawItem(Graphics g, Item item)
        {
            var r = Rectangle.Round(item.Rect);
            if (r.Width < 8 || r.Height < 8)
                return;
            Color qualityColor = Theme.QualityColor(item.Skill.Quality);
            using (var brush = new SolidBrush(Theme.RowBg))
                g.FillRectangle(brush, r);
            using (var pen = new Pen(Color.FromArgb(170, qualityColor)))
                g.DrawRectangle(pen, r.X, r.Y, r.Width - 1, r.Height - 1);
            if (r.Width < 24)
                return;

            // 第 1 行=人名（技能品质色），其后每行一个组件名（组件品质色），末行=Q + 价格
            var lines = new List<Theme.TextSegment[]>
            {
                new[] { new Theme.TextSegment(item.Skill.Sname, qualityColor) },
            };
            if (item.Skill.Modules != null)
            {
                foreach (string name in item.Skill.Modules)
                {
                    if (string.IsNullOrEmpty(name))
                        continue;
                    ComponentRow comp;
                    Color color = store != null && store.ComponentByName.TryGetValue(name, out comp)
                        ? Theme.QualityColor(comp.Quality)
                        : Theme.Danger;
                    lines.Add(new[] { new Theme.TextSegment(name, color) });
                }
            }
            lines.Add(new[]
            {
                new Theme.TextSegment("Q" + item.Skill.Quality, qualityColor),
                new Theme.TextSegment(" " + PriceText(item.Price), Theme.Accent),
            });

            int available = r.Height - 4;
            int lineHeight = Math.Min(14, Math.Max(12, available / Math.Max(1, lines.Count)));
            int maxLines = Math.Max(1, available / lineHeight);
            int y = r.Y + 2;
            for (int i = 0; i < lines.Count && i < maxLines; i++)
            {
                var lineRect = new Rectangle(r.X + 3, y, Math.Max(0, r.Width - 6), lineHeight);
                DrawCenteredSegments(g, lineRect, lines[i]);
                y += lineHeight;
            }
        }

        private static void DrawCenteredSegments(Graphics g, Rectangle rect, IList<Theme.TextSegment> segments)
        {
            int width = 0;
            foreach (var seg in segments)
            {
                width += TextRenderer.MeasureText(g, seg.Text, Theme.Small, Size.Empty,
                    TextFormatFlags.NoPadding | TextFormatFlags.SingleLine).Width;
            }
            var target = new Rectangle(rect.X + Math.Max(0, (rect.Width - width) / 2), rect.Y,
                Math.Min(width, rect.Width), rect.Height);
            Theme.DrawSegments(g, target, segments, Theme.Small);
        }

        #endregion

        private sealed class Block
        {
            public string Title;
            public string Meta;
            public Color Color;
            public int Count;
            public double Score;
            public List<Item> Items = new List<Item>();
            public RectangleF Rect = RectangleF.Empty;
        }

        private sealed class Item
        {
            public SkillRow Skill;
            public Block Block;
            public double Price;
            public RectangleF Rect = RectangleF.Empty;
        }

        /// <summary>双缓冲画布，避免重绘闪烁。</summary>
        private sealed class Canvas : Control
        {
            private readonly TreemapView owner;

            public Canvas(TreemapView owner)
            {
                this.owner = owner;
                BackColor = Theme.Bg;
                DoubleBuffered = true;
                ResizeRedraw = true;
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                owner.PaintCanvas(e.Graphics, ClientSize);
            }
        }
    }
}
