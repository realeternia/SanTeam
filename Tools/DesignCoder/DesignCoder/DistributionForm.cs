using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace DesignCoder
{
    public class DistributionForm : Form
    {
        private Panel chartPanel;
        private List<int> values;
        private string displayName;
        private string fieldName;

        private int minVal;
        private int maxVal;
        private int range;
        private int binCount;
        private Dictionary<int, int> histogram;
        private int maxCount;

        private Color themeBackColor = Color.FromArgb(30, 30, 30);
        private Color titleColor = Color.FromArgb(220, 220, 220);
        private Color axisColor = Color.FromArgb(120, 120, 130);
        private Color gridColor = Color.FromArgb(50, 50, 55);
        private Color tickColor = Color.FromArgb(160, 160, 170);
        private Color barFillColor = Color.FromArgb(70, 130, 200);
        private Color barBorderColor = Color.FromArgb(100, 160, 230);
        private Color curveColor = Color.FromArgb(100, 180, 255);
        private Color curveFillColor = Color.FromArgb(50, 100, 180, 255);
        private Color dotColor = Color.FromArgb(100, 200, 255);
        private Color infoColor = Color.FromArgb(140, 140, 150);

        public DistributionForm(string displayName, string fieldName, List<int> values)
        {
            this.displayName = displayName;
            this.fieldName = fieldName;
            this.values = values;

            InitializeForm();
            CalculateHistogram();
        }

        private void InitializeForm()
        {
            this.Text = string.Format("分布 - {0} ({1})", displayName, fieldName);
            this.Size = new Size(720, 520);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.MinimizeBox = false;
            this.MaximizeBox = false;
            this.Font = new Font("微软雅黑", 9F);
            this.BackColor = themeBackColor;

            chartPanel = new Panel();
            chartPanel.Dock = DockStyle.Fill;
            chartPanel.BackColor = themeBackColor;
            chartPanel.Paint += ChartPanel_Paint;
            chartPanel.Resize += (s, e) => chartPanel.Invalidate();
            this.Controls.Add(chartPanel);
        }

        private void CalculateHistogram()
        {
            minVal = values.Min();
            maxVal = values.Max();
            range = maxVal - minVal;
            binCount = Math.Min(range + 1, 50);
            if (range == 0) binCount = 1;

            histogram = new Dictionary<int, int>();
            if (range <= 50)
            {
                binCount = range + 1;
                for (int i = minVal; i <= maxVal; i++)
                    histogram[i] = 0;
                foreach (var v in values)
                    histogram[v]++;
            }
            else
            {
                double binWidth = (double)range / binCount;
                for (int i = 0; i < binCount; i++)
                    histogram[i] = 0;
                foreach (var v in values)
                {
                    int bin = (int)Math.Min((v - minVal) / binWidth, binCount - 1);
                    histogram[bin]++;
                }
            }

            maxCount = histogram.Values.Max();
        }

        private void ChartPanel_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            Rectangle chartArea = chartPanel.ClientRectangle;
            int marginLeft = 70;
            int marginRight = 30;
            int marginTop = 50;
            int marginBottom = 70;

            int plotW = chartArea.Width - marginLeft - marginRight;
            int plotH = chartArea.Height - marginTop - marginBottom;

            if (plotW <= 0 || plotH <= 0) return;

            DrawTitle(g, marginLeft, plotW);
            DrawAxes(g, marginLeft, marginTop, plotW, plotH);
            DrawGridLines(g, marginLeft, marginTop, plotW, plotH);
            DrawAxisLabels(g, marginLeft, marginTop, plotW, plotH);
            DrawHistogram(g, marginLeft, marginTop, plotW, plotH);
            DrawStatistics(g, marginLeft, marginTop, plotH);
        }

        private void DrawTitle(Graphics g, int marginLeft, int plotW)
        {
            using (Brush titleBrush = new SolidBrush(titleColor))
            {
                string title = string.Format("{0} 分布  (共{1}条数据, 范围: {2}~{3}, 均值: {4:F1})",
                    displayName, values.Count, minVal, maxVal, values.Average());
                Font titleFont = new Font("微软雅黑", 11F, FontStyle.Bold);
                SizeF titleSize = g.MeasureString(title, titleFont);
                g.DrawString(title, titleFont, titleBrush, marginLeft + (plotW - titleSize.Width) / 2, 12);
            }
        }

        private void DrawAxes(Graphics g, int marginLeft, int marginTop, int plotW, int plotH)
        {
            using (Pen axisPen = new Pen(axisColor, 1))
            {
                g.DrawLine(axisPen, marginLeft, marginTop, marginLeft, marginTop + plotH);
                g.DrawLine(axisPen, marginLeft, marginTop + plotH, marginLeft + plotW, marginTop + plotH);
            }
        }

        private void DrawGridLines(Graphics g, int marginLeft, int marginTop, int plotW, int plotH)
        {
            using (Pen gridPen = new Pen(gridColor, 1))
            {
                int yTickCount = 5;
                for (int i = 0; i <= yTickCount; i++)
                {
                    int y = marginTop + plotH - (int)(plotH * i / (double)yTickCount);
                    g.DrawLine(gridPen, marginLeft + 1, y, marginLeft + plotW, y);

                    int tickVal = (int)(maxCount * i / (double)yTickCount);
                    using (Brush tickBrush = new SolidBrush(tickColor))
                    {
                        StringFormat sf = new StringFormat { Alignment = StringAlignment.Far, LineAlignment = StringAlignment.Center };
                        g.DrawString(tickVal.ToString(), new Font("微软雅黑", 8F), tickBrush, marginLeft - 8, y, sf);
                    }
                }
            }
        }

        private void DrawAxisLabels(Graphics g, int marginLeft, int marginTop, int plotW, int plotH)
        {
            using (Brush labelBrush = new SolidBrush(tickColor))
            {
                StringFormat sf = new StringFormat { Alignment = StringAlignment.Center };
                int labelCount = Math.Min(binCount, 15);
                double step = binCount / (double)labelCount;
                Font labelFont = new Font("微软雅黑", 7.5F);

                for (int i = 0; i <= labelCount; i++)
                {
                    int binIdx = (int)Math.Min(i * step, binCount - 1);
                    float x = marginLeft + (float)(binIdx + 0.5) * plotW / binCount;
                    string label;
                    if (range <= 50)
                        label = (minVal + binIdx).ToString();
                    else
                    {
                        double binWidth = (double)range / binCount;
                        label = (minVal + binIdx * binWidth).ToString("F0");
                    }
                    g.DrawString(label, labelFont, labelBrush, x, marginTop + plotH + 6, sf);
                }

                StringFormat yLabelSf = new StringFormat { Alignment = StringAlignment.Center };
                g.TranslateTransform(18, marginTop + plotH / 2);
                g.RotateTransform(-90);
                g.DrawString("频次", new Font("微软雅黑", 9F), labelBrush, 0, 0, yLabelSf);
                g.ResetTransform();

                g.TranslateTransform(marginLeft + plotW / 2, marginTop + plotH + 45);
                g.DrawString("值", new Font("微软雅黑", 9F), labelBrush, 0, 0, sf);
                g.ResetTransform();
            }
        }

        private void DrawHistogram(Graphics g, int marginLeft, int marginTop, int plotW, int plotH)
        {
            float barW = (float)plotW / binCount;
            List<PointF> curvePoints = new List<PointF>();

            for (int i = 0; i < binCount; i++)
            {
                int count = histogram.ContainsKey(i) ? histogram[i] : 0;
                float barH = maxCount > 0 ? (float)count / maxCount * plotH : 0;
                float x = marginLeft + i * barW;
                float y = marginTop + plotH - barH;

                RectangleF barRect = new RectangleF(x + 1, y, barW - 2, barH);
                using (Brush fillBrush = new SolidBrush(Color.FromArgb(140, barFillColor)))
                {
                    g.FillRectangle(fillBrush, barRect);
                }
                using (Pen borderPen = new Pen(barBorderColor, 1))
                {
                    g.DrawRectangle(borderPen, barRect.X, barRect.Y, barRect.Width, barRect.Height);
                }

                curvePoints.Add(new PointF(x + barW / 2, y));
            }

            DrawDistributionCurve(g, marginLeft, marginTop, plotW, plotH, curvePoints);
        }

        private void DrawDistributionCurve(Graphics g, int marginLeft, int marginTop, int plotW, int plotH, List<PointF> curvePoints)
        {
            if (curvePoints.Count > 1)
            {
                List<PointF> smoothPoints = new List<PointF>();
                smoothPoints.Add(new PointF(marginLeft, marginTop + plotH));
                smoothPoints.AddRange(curvePoints);
                smoothPoints.Add(new PointF(marginLeft + plotW, marginTop + plotH));

                using (GraphicsPath fillPath = new GraphicsPath())
                {
                    fillPath.AddLines(smoothPoints.ToArray());
                    fillPath.CloseAllFigures();
                    using (Brush curveFillBrush = new SolidBrush(curveFillColor))
                    {
                        g.FillPath(curveFillBrush, fillPath);
                    }
                }

                if (curvePoints.Count >= 3)
                {
                    using (GraphicsPath curvePath = new GraphicsPath())
                    {
                        curvePath.AddCurve(curvePoints.ToArray(), 0.5f);
                        using (Pen curvePen = new Pen(curveColor, 2.5f))
                        {
                            g.DrawPath(curvePen, curvePath);
                        }
                    }
                }
                else
                {
                    using (Pen curvePen = new Pen(curveColor, 2.5f))
                    {
                        g.DrawLines(curvePen, curvePoints.ToArray());
                    }
                }

                DrawDataPoints(g, curvePoints);
            }
            else if (curvePoints.Count == 1)
            {
                DrawSingleDataPoint(g, curvePoints[0]);
            }
        }

        private void DrawDataPoints(Graphics g, List<PointF> curvePoints)
        {
            for (int i = 0; i < curvePoints.Count; i++)
            {
                int count = histogram.ContainsKey(i) ? histogram[i] : 0;
                if (count > 0)
                {
                    PointF pt = curvePoints[i];
                    using (Brush dotBrush = new SolidBrush(dotColor))
                    {
                        g.FillEllipse(dotBrush, pt.X - 3, pt.Y - 3, 6, 6);
                    }
                }
            }
        }

        private void DrawSingleDataPoint(Graphics g, PointF pt)
        {
            using (Brush dotBrush = new SolidBrush(dotColor))
            {
                g.FillEllipse(dotBrush, pt.X - 4, pt.Y - 4, 8, 8);
            }
            using (Brush valBrush = new SolidBrush(titleColor))
            {
                StringFormat sf = new StringFormat { Alignment = StringAlignment.Center };
                g.DrawString(string.Format("值: {0}, 频次: {1}", minVal, histogram[0]),
                    new Font("微软雅黑", 9F), valBrush, pt.X, pt.Y - 22, sf);
            }
        }

        private void DrawStatistics(Graphics g, int marginLeft, int marginTop, int plotH)
        {
            int infoY = marginTop + plotH + 55;
            using (Brush infoBrush = new SolidBrush(infoColor))
            {
                Font infoFont = new Font("微软雅黑", 8F);
                var sorted = values.OrderBy(v => v).ToList();
                double median = sorted.Count % 2 == 0
                    ? (sorted[sorted.Count / 2 - 1] + sorted[sorted.Count / 2]) / 2.0
                    : sorted[sorted.Count / 2];
                double variance = values.Select(v => (v - values.Average()) * (v - values.Average())).Average();
                double stdDev = Math.Sqrt(variance);
                string info = string.Format("中位数: {0:F1}  标准差: {1:F2}  众数: {2}",
                    median, stdDev, histogram.OrderByDescending(kv => kv.Value).First().Key);
                g.DrawString(info, infoFont, infoBrush, marginLeft, infoY);
            }
        }
    }
}
