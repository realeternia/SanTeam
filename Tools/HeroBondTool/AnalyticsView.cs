using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace HeroBondTool
{
    /// <summary>剧本统计页：整页自绘（深色卡片 + 条形图），可滚动。</summary>
    public sealed class AnalyticsView : Panel
    {
        private const int Edge = 22;
        private const int Gap = 14;
        private const int StatsHeight = 78;
        private const int CardHeader = 50;
        private const int RowHeight = 26;
        private const int CardBottom = 12;
        private const int StatCount = 5;

        private ScenarioAnalysis data;
        private readonly DataStore store;

        public AnalyticsView(DataStore store)
        {
            this.store = store;
            DoubleBuffered = true;
            AutoScroll = true;
            BackColor = Theme.Bg;
        }

        public void SetData(ScenarioAnalysis analysis)
        {
            int scrollX = -AutoScrollPosition.X;
            int scrollY = -AutoScrollPosition.Y;
            data = analysis;
            AutoScrollMinSize = new Size(0, ComputeHeight());
            if (-AutoScrollPosition.X != scrollX || -AutoScrollPosition.Y != scrollY)
                AutoScrollPosition = new Point(scrollX, scrollY);
            Invalidate();
        }

        private int ContentWidth
        {
            get
            {
                int w = ClientSize.Width - Edge * 2;
                if (AutoScrollMinSize.Height > ClientSize.Height)
                    w -= SystemInformation.VerticalScrollBarWidth;
                return Math.Max(w, 560);
            }
        }

        private int CardHeight(int rowCount)
        {
            return CardHeader + Math.Max(rowCount, 1) * RowHeight + CardBottom;
        }

        private int ViolationCardHeight(int width)
        {
            if (data == null)
                return 0;
            int inner = width - 40;
            int h = CardHeader + 8;
            h += Theme.MeasureText(RuleText(), Theme.Small, inner) + 12;
            if (data.Violations.Count == 0)
                return h + 44;
            foreach (GroupViolation v in data.Violations)
            {
                string tags = string.Join("；", v.Checks.ToArray());
                string members = MembersText(v);
                h += 12 + 22 + Theme.MeasureText(tags, Theme.Small, inner) + 6
                    + Theme.MeasureText(members, Theme.Small, inner) + 16;
            }
            return h + 12;
        }

        private string RuleText()
        {
            return string.Format(
                "规则：同品质 ≤{0} 人、同职业 ≤{1} 人、同国家 ≤{2} 人；同一对武将不得在多个羁绊组中同时出现（按本剧本出场成员统计）",
                Analysis.BondMaxQuality, Analysis.BondMaxJob, Analysis.BondMaxSide);
        }

        private string MembersText(GroupViolation v)
        {
            if (v.Members.Count == 0)
                return "成员：无";
            var parts = new List<string>();
            foreach (HeroSlot slot in v.Members)
            {
                JobInfo job = jobOf(slot);
                string jobName = job != null ? job.Name : (string.IsNullOrEmpty(slot.Hero.Job) ? "未知" : slot.Hero.Job);
                parts.Add(string.Format("{0}（Q{1}·{2}·{3}）", slot.Hero.Name, slot.Quality, jobName, slot.Side));
            }
            return "成员：" + string.Join("、", parts.ToArray());
        }

        private JobInfo jobOf(HeroSlot slot)
        {
            return store.GetJob(slot.Hero.Job);
        }

        private int ComputeHeight()
        {
            if (data == null)
                return 0;
            int width = ContentWidth;
            int colW = (width - Gap) / 2;
            int leftHeight = CardHeight(Math.Max(data.SideRows.Count, 1)) + Gap
                + CardHeight(data.QualityRows.Count) + Gap
                + CardHeight(data.JobRows.Count);
            int rightHeight = CardHeight(data.BondRows.Count);
            int columns = Math.Max(leftHeight, rightHeight);
            return Edge + StatsHeight + Gap + columns + Gap + ViolationCardHeight(width) + Edge;
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (data != null)
            {
                AutoScrollMinSize = new Size(0, ComputeHeight());
                Invalidate();
            }
        }

        protected override void OnScroll(ScrollEventArgs se)
        {
            base.OnScroll(se);
            Invalidate();
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.Clear(Theme.Bg);
            if (data == null)
                return;

            int offset = AutoScrollPosition.Y;
            e.Graphics.TranslateTransform(0, offset);

            int width = ContentWidth;
            int x0 = Edge;
            int x1 = Edge + (width - Gap) / 2 + Gap;
            int colW = (width - Gap) / 2;

            int statW = (width - Gap * (StatCount - 1)) / StatCount;
            int statY = Edge;
            string[] labels = { "总英雄数", "势力数", "职业数", "有成员的羁绊组", "不合规羁绊组" };
            int[] values = { data.Total, data.SideCount, data.JobCount, data.ActiveGroupCount, data.ViolationCount };
            for (int i = 0; i < StatCount; i++)
            {
                int x = Edge + i * (statW + Gap);
                DrawStatCard(e.Graphics, new Rectangle(x, statY, statW, StatsHeight), values[i], labels[i]);
            }

            int columnsY = statY + StatsHeight + Gap;
            int y0 = columnsY;
            DrawBarCard(e.Graphics, new Rectangle(x0, y0, colW, CardHeight(Math.Max(data.SideRows.Count, 1))),
                "国家分布", data.SideCount + " 个势力", data.SideRows, data.Total);
            y0 += CardHeight(Math.Max(data.SideRows.Count, 1)) + Gap;

            DrawBarCard(e.Graphics, new Rectangle(x0, y0, colW, CardHeight(data.QualityRows.Count)),
                "品质分布", data.QualityRows.Count + " 个品质档", data.QualityRows, data.Total);
            y0 += CardHeight(data.QualityRows.Count) + Gap;

            DrawBarCard(e.Graphics, new Rectangle(x0, y0, colW, CardHeight(data.JobRows.Count)),
                "职业分布", data.JobCount + " 个职业", data.JobRows, data.Total);
            int leftBottom = y0 + CardHeight(data.JobRows.Count);

            int y1 = columnsY;
            DrawBarCard(e.Graphics, new Rectangle(x1, y1, colW, CardHeight(data.BondRows.Count)),
                "好友羁绊人数", data.GroupCount + " 个羁绊组", data.BondRows, data.Total);
            int rightBottom = y1 + CardHeight(data.BondRows.Count);

            int violY = Math.Max(leftBottom, rightBottom) + Gap;
            DrawViolationCard(e.Graphics, new Rectangle(x0, violY, width, ViolationCardHeight(width)));

            e.Graphics.ResetTransform();
        }

        private void DrawStatCard(Graphics g, Rectangle rect, int value, string label)
        {
            Theme.DrawCard(g, rect, Theme.Card2);
            Theme.DrawText(g, value.ToString(), Theme.Num, Theme.Accent, rect.X + 18, rect.Y + 14);
            Theme.DrawText(g, label, Theme.Small, Theme.Muted, rect.X + 18, rect.Y + 52);
        }

        private void DrawBarCard(Graphics g, Rectangle rect, string title, string sub, List<BarRow> rows, int total)
        {
            Theme.DrawCard(g, rect, Theme.Card2);
            Theme.DrawText(g, title, Theme.Title, Theme.Text, rect.X + 16, rect.Y + 15);
            Theme.DrawTextRight(g, sub, Theme.Small, Theme.Muted, rect.Right - 16, rect.Y + 18);

            int max = 1;
            foreach (BarRow row in rows)
                max = Math.Max(max, row.Count);

            int rowY = rect.Y + CardHeader - 4;
            int labelW = 112;
            int valueW = 92;
            for (int i = 0; i < rows.Count; i++)
            {
                BarRow row = rows[i];
                int y = rowY + i * RowHeight;
                Theme.DrawEllipsis(g, row.Label, Theme.Base, Theme.Text, rect.X + 16, y + 4, labelW);

                var track = new Rectangle(rect.X + 16 + labelW + 8, y + 6, rect.Width - 16 - labelW - 8 - valueW - 16, 14);
                Theme.FillRound(g, track, 7, Theme.RowBg);
                Theme.DrawRoundBorder(g, track, 7, Theme.Line);

                double ratio = (double)row.Count / max;
                int fillW = (int)Math.Round(track.Width * ratio);
                if (fillW > 0)
                {
                    var fill = new Rectangle(track.X, track.Y, Math.Max(fillW, 4), track.Height);
                    Theme.FillRound(g, fill, 7, row.Color);
                }

                Theme.DrawTextRight(g, row.Count + "  ", Theme.Bold, Theme.Text, rect.Right - 16 - 48, y + 4);
                string pct = total > 0 ? string.Format("{0:0.0}%", row.Count * 100.0 / total) : "0.0%";
                Theme.DrawTextRight(g, pct, Theme.Small, Theme.Faint, rect.Right - 16, y + 5);
            }
        }

        private void DrawViolationCard(Graphics g, Rectangle rect)
        {
            Theme.DrawCard(g, rect, Theme.Card2);
            Theme.DrawText(g, "羁绊组合规检查", Theme.Title, Theme.Text, rect.X + 16, rect.Y + 15);
            string sub = data.GroupCount > 0
                ? string.Format("{0} / {1} 组不合规", data.ViolationCount, data.GroupCount)
                : "0 / 0 组不合规";
            Theme.DrawTextRight(g, sub, Theme.Small, Theme.Muted, rect.Right - 16, rect.Y + 18);

            int x = rect.X + 16;
            int inner = rect.Width - 32;
            int y = rect.Y + CardHeader + 2;
            int ruleH = Theme.MeasureText(RuleText(), Theme.Small, inner);
            Theme.DrawWrapped(g, RuleText(), Theme.Small, Theme.Muted, x, y, inner);
            y += ruleH + 10;

            if (data.Violations.Count == 0)
            {
                var okRect = new Rectangle(x, y, inner, 34);
                Theme.FillRound(g, okRect, 6, Color.FromArgb(24, 126, 214, 165));
                Theme.DrawRoundBorder(g, okRect, 6, Color.FromArgb(70, 126, 214, 165));
                Theme.DrawText(g, "全部羁绊组合规", Theme.Base, Theme.Ok, okRect.X + 12, okRect.Y + 8);
                return;
            }

            foreach (GroupViolation v in data.Violations)
            {
                string tags = string.Join("；", v.Checks.ToArray());
                string members = MembersText(v);
                int tagsH = Theme.MeasureText(tags, Theme.Small, inner - 24);
                int membersH = Theme.MeasureText(members, Theme.Small, inner - 24);
                int itemH = 12 + 20 + tagsH + 6 + membersH + 12;

                var item = new Rectangle(x, y, inner, itemH);
                Theme.FillRound(g, item, 6, Color.FromArgb(255, 26, 31, 38));
                Theme.DrawRoundBorder(g, item, 6, Theme.Line);
                g.FillRectangle(new SolidBrush(ColorTranslator.FromHtml("#E0A94F")), new Rectangle(item.X, item.Y + 8, 3, item.Height - 16));

                Theme.DrawText(g, v.Group.Name, Theme.Bold, Theme.Text, item.X + 12, item.Y + 8);
                Theme.DrawTextRight(g, v.Members.Count + " 人", Theme.Small, Theme.Muted, item.Right - 12, item.Y + 8);
                Theme.DrawWrapped(g, tags, Theme.Small, ColorTranslator.FromHtml("#FFD9A0"), item.X + 12, item.Y + 30, inner - 24);
                Theme.DrawWrapped(g, members, Theme.Small, Theme.Faint, item.X + 12, item.Y + 36 + tagsH, inner - 24);

                y += itemH + 8;
            }
        }
    }
}
