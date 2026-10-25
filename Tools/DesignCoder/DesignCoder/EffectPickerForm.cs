using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace DesignCoder
{
    /// <summary>
    /// 特效截图选择面板：按行列显示 EffectScreenshots 目录下的特效截图。
    /// 负面特效红框、正面（增益）特效绿框，伤害类灰框；缩略图左上显示资源路径，右下显示 EffectConfig 的 Area。
    /// 双击缩略图或点“确定”返回选中的特效名（EffectConfig.Name，即技能配置里存的值）。
    /// </summary>
    public class EffectPickerForm : Form
    {
        private const int ThumbSize = 128;
        private const int ItemPad = 10;
        private const int NameHeight = 20;
        private const int ItemWidth = ThumbSize + ItemPad * 2 + 6;
        private const int ItemHeight = ThumbSize + ItemPad * 2 + NameHeight;

        private static readonly Color PanelBg = Color.FromArgb(37, 37, 38);
        private static readonly Color ItemBg = Color.FromArgb(45, 45, 48);
        private static readonly Color NegativeColor = Color.FromArgb(225, 70, 70);   // 负面：红框
        private static readonly Color PositiveColor = Color.FromArgb(70, 200, 100);  // 正面：绿框
        private static readonly Color DamageColor = Color.FromArgb(150, 155, 165);   // 伤害：灰框
        private static readonly Color SelectedColor = Color.FromArgb(255, 215, 64);

        private readonly string effectConfigPath;
        private readonly string screenshotDir;
        private readonly string currentValue;

        private readonly FlowLayoutPanel panel;
        private readonly Label tipLabel;
        private readonly List<EffectItemControl> itemControls = new List<EffectItemControl>();
        private EffectItemControl selected;
        // 打开时默认选中的缩略图（等窗体显示后再滚动到可视区域）
        private EffectItemControl pendingDefault;

        /// <summary>选中返回的特效名（EffectConfig.Name），未选择时为 null</summary>
        public string SelectedEffect { get; private set; }

        public EffectPickerForm(string effectConfigPath, string screenshotDir, string currentValue)
        {
            this.effectConfigPath = effectConfigPath;
            this.screenshotDir = screenshotDir;
            this.currentValue = currentValue ?? "";

            Text = "选择特效";
            StartPosition = FormStartPosition.CenterParent;
            ShowInTaskbar = false;
            MinimizeBox = false;
            Size = new Size(1120, 800);
            BackColor = PanelBg;
            Font = new Font("微软雅黑", 9F);

            tipLabel = new Label();
            tipLabel.Dock = DockStyle.Top;
            tipLabel.Height = 30;
            tipLabel.ForeColor = Color.FromArgb(220, 220, 220);
            tipLabel.BackColor = Color.FromArgb(52, 52, 56);
            tipLabel.TextAlign = ContentAlignment.MiddleLeft;
            tipLabel.Padding = new Padding(10, 0, 0, 0);
            Controls.Add(tipLabel);

            var bottom = new Panel();
            bottom.Dock = DockStyle.Bottom;
            bottom.Height = 44;
            bottom.BackColor = Color.FromArgb(52, 52, 56);
            Controls.Add(bottom);

            var btnOk = new Button();
            btnOk.Text = "确定";
            btnOk.Size = new Size(90, 28);
            btnOk.Anchor = AnchorStyles.Right | AnchorStyles.Top;
            btnOk.Location = new Point(bottom.Width - 200, 8);
            btnOk.Click += btnOk_Click;
            bottom.Controls.Add(btnOk);

            var btnCancel = new Button();
            btnCancel.Text = "取消";
            btnCancel.Size = new Size(90, 28);
            btnCancel.Anchor = AnchorStyles.Right | AnchorStyles.Top;
            btnCancel.Location = new Point(bottom.Width - 100, 8);
            btnCancel.DialogResult = DialogResult.Cancel;
            bottom.Controls.Add(btnCancel);

            panel = new FlowLayoutPanel();
            panel.Dock = DockStyle.Fill;
            panel.AutoScroll = true;
            panel.WrapContents = true;
            panel.BackColor = PanelBg;
            panel.Padding = new Padding(8);
            Controls.Add(panel);
            panel.BringToFront();

            CancelButton = btnCancel;
            KeyPreview = true;
            KeyDown += EffectPickerForm_KeyDown;

            LoadItems();
        }

        private void EffectPickerForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                DialogResult = DialogResult.Cancel;
                Close();
            }
        }

        private void LoadItems()
        {
            Dictionary<string, Dictionary<string, string>> effects;
            try
            {
                effects = LoadEffectConfig(effectConfigPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "解析 EffectConfig 失败：" + ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                effects = new Dictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase);
            }

            if (!Directory.Exists(screenshotDir))
            {
                tipLabel.Text = "截图目录不存在：" + screenshotDir;
                return;
            }

            string[] files = Directory.GetFiles(screenshotDir, "*.png");
            Array.Sort(files, StringComparer.OrdinalIgnoreCase);

            foreach (string file in files)
            {
                string fileBase = Path.GetFileNameWithoutExtension(file);
                bool isMissile = fileBase.EndsWith("_Missile", StringComparison.OrdinalIgnoreCase);
                string effectName = isMissile ? fileBase.Substring(0, fileBase.Length - "_Missile".Length) : fileBase;

                Dictionary<string, string> cfg;
                effects.TryGetValue(effectName, out cfg);

                string type = GetConfigValue(cfg, "Type");
                string area = GetConfigValue(cfg, "Area");
                string resPath = isMissile ? GetConfigValue(cfg, "MissilePath") : GetConfigValue(cfg, "EffPath");
                if (string.IsNullOrEmpty(resPath))
                    resPath = fileBase;

                var item = new EffectItem();
                item.Value = cfg != null ? GetConfigValue(cfg, "Name") : effectName;
                item.DisplayName = fileBase;
                item.PathName = resPath;
                item.Area = area;
                item.BorderColor = GetBorderColor(type);
                item.Thumb = LoadThumbnail(file);

                var ctl = new EffectItemControl(item);
                ctl.Click += ItemControl_Click;
                ctl.DoubleClick += ItemControl_DoubleClick;
                itemControls.Add(ctl);
                panel.Controls.Add(ctl);
            }

            if (itemControls.Count == 0)
            {
                tipLabel.Text = "截图目录暂无图片：" + screenshotDir;
                return;
            }

            tipLabel.Text = string.Format("共 {0} 张（红框=负面，绿框=正面，灰框=伤害）；单击选中，双击或点“确定”应用　　当前值：{1}",
                itemControls.Count, string.IsNullOrEmpty(currentValue) ? "（空）" : currentValue);

            SelectDefault();
        }

        // 默认选中当前字符串对应的截图（同名特效优先取本体，其次导弹）
        private void SelectDefault()
        {
            if (string.IsNullOrEmpty(currentValue)) return;

            EffectItemControl match = null;
            foreach (var ctl in itemControls)
            {
                if (!string.Equals(ctl.Item.Value, currentValue, StringComparison.OrdinalIgnoreCase)) continue;
                if (!ctl.Item.DisplayName.EndsWith("_Missile", StringComparison.OrdinalIgnoreCase))
                {
                    match = ctl;
                    break;
                }
                if (match == null) match = ctl;
            }
            if (match == null) return;

            pendingDefault = match;
            SelectItem(match);
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            // 窗体真正显示后才能可靠滚动，把默认选中的缩略图带到可视区域
            if (pendingDefault != null)
                panel.ScrollControlIntoView(pendingDefault);
        }

        private static Dictionary<string, Dictionary<string, string>> LoadEffectConfig(string path)
        {
            var result = new Dictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase);
            if (string.IsNullOrEmpty(path) || !File.Exists(path)) return result;

            var data = ConfigData.Parse(File.ReadAllText(path, Encoding.UTF8));
            foreach (var row in data.Rows)
            {
                string name = GetConfigValue(row, "Name");
                if (string.IsNullOrEmpty(name)) continue;
                result[name] = row;
            }
            return result;
        }

        private static string GetConfigValue(Dictionary<string, string> row, string field)
        {
            if (row == null || string.IsNullOrEmpty(field)) return "";
            string value;
            return row.TryGetValue(field, out value) ? value : "";
        }

        private static Color GetBorderColor(string type)
        {
            if (type == "负面") return NegativeColor;
            if (type == "增益") return PositiveColor;
            return DamageColor;
        }

        private static Bitmap LoadThumbnail(string file)
        {
            using (var src = Image.FromFile(file))
            {
                var bmp = new Bitmap(ThumbSize, ThumbSize);
                using (var g = Graphics.FromImage(bmp))
                {
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    g.DrawImage(src, new Rectangle(0, 0, ThumbSize, ThumbSize));
                }
                return bmp;
            }
        }

        private void ItemControl_Click(object sender, EventArgs e)
        {
            var ctl = sender as EffectItemControl;
            if (ctl != null) SelectItem(ctl);
        }

        private void ItemControl_DoubleClick(object sender, EventArgs e)
        {
            var ctl = sender as EffectItemControl;
            if (ctl == null) return;
            SelectItem(ctl);
            ApplyAndClose();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            ApplyAndClose();
        }

        private void ApplyAndClose()
        {
            if (selected == null)
            {
                MessageBox.Show(this, "请先点击选择一张特效截图", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            SelectedEffect = selected.Item.Value;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void SelectItem(EffectItemControl ctl)
        {
            if (selected != null && selected != ctl)
            {
                selected.IsSelected = false;
                selected.Invalidate();
            }
            selected = ctl;
            ctl.IsSelected = true;
            ctl.Invalidate();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                foreach (var ctl in itemControls)
                {
                    if (ctl.Item != null && ctl.Item.Thumb != null)
                        ctl.Item.Thumb.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        private class EffectItem
        {
            public string Value;        // 写回配置的值（EffectConfig.Name）
            public string DisplayName;  // 截图文件名
            public string PathName;     // 左上显示的资源路径
            public string Area;         // 右下显示的 Area
            public Color BorderColor;
            public Bitmap Thumb;
        }

        private class EffectItemControl : Control
        {
            public readonly EffectItem Item;
            public bool IsSelected;

            private readonly Font nameFont = new Font("微软雅黑", 8.5F);
            private readonly Font overlayFont = new Font("微软雅黑", 7.5F);

            public EffectItemControl(EffectItem item)
            {
                Item = item;
                Size = new Size(ItemWidth, ItemHeight);
                BackColor = ItemBg;
                Margin = new Padding(4);
                Cursor = Cursors.Hand;
                SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);
            }

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    nameFont.Dispose();
                    overlayFont.Dispose();
                }
                base.Dispose(disposing);
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                Graphics g = e.Graphics;
                g.Clear(IsSelected ? Color.FromArgb(60, 60, 68) : ItemBg);

                var imgRect = new Rectangle(ItemPad + 3, ItemPad, ThumbSize, ThumbSize);
                if (Item.Thumb != null)
                    g.DrawImage(Item.Thumb, imgRect);

                using (var pen = new Pen(Item.BorderColor, 3f))
                {
                    g.DrawRectangle(pen, imgRect.X - 1, imgRect.Y - 1, imgRect.Width + 2, imgRect.Height + 2);
                }

                if (IsSelected)
                {
                    using (var pen = new Pen(SelectedColor, 2f))
                    {
                        g.DrawRectangle(pen, 1, 1, Width - 3, Height - 3);
                    }
                }

                // 左上：资源路径（半透明黑底 + 省略号）
                DrawOverlayText(g, Item.PathName, new Rectangle(imgRect.X + 1, imgRect.Y + 1, imgRect.Width - 2, 18), ContentAlignment.MiddleLeft);
                // 右下：EffectConfig.Area
                if (!string.IsNullOrEmpty(Item.Area))
                    DrawOverlayText(g, Item.Area, new Rectangle(imgRect.X + 1, imgRect.Bottom - 19, imgRect.Width - 2, 18), ContentAlignment.MiddleRight);

                // 底部：截图文件名
                var nameRect = new Rectangle(2, imgRect.Bottom + 2, Width - 4, NameHeight - 2);
                TextRenderer.DrawText(g, Item.DisplayName, nameFont, nameRect,
                    Color.FromArgb(215, 215, 215),
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);
            }

            private void DrawOverlayText(Graphics g, string text, Rectangle rect, ContentAlignment align)
            {
                if (string.IsNullOrEmpty(text)) return;

                using (var back = new SolidBrush(Color.FromArgb(175, 0, 0, 0)))
                {
                    g.FillRectangle(back, rect);
                }

                var flags = TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix;
                flags |= align == ContentAlignment.MiddleRight ? TextFormatFlags.Right : TextFormatFlags.Left;

                var textRect = new Rectangle(rect.X + 3, rect.Y, rect.Width - 6, rect.Height);
                TextRenderer.DrawText(g, text, overlayFont, textRect, Color.White, flags);
            }
        }
    }
}
