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
    /// 特效引用统计：主计数按类型取自不同表，ExtraCounts 为 JobConfig/SoldierConfig 的额外命中数。
    /// </summary>
    public class EffectUsageStats
    {
        public Dictionary<string, int> SkillCounts;    // 技能 SelfEffect/HitEffect/AreaEffect 引用技能数（同 Sname 只算一次）
        public Dictionary<string, int> MissileCounts;  // 技能 HitEffect 引用技能数（导弹页签）
        public Dictionary<string, int> BuffCounts;     // BuffConfig.BuffEffect 引用行数（Buff 页签）
        public Dictionary<string, int> ExtraCounts;    // JobConfig/SoldierConfig.HitEffect 引用行数（黄色标签）
    }

    /// <summary>
    /// 特效截图选择面板：分“导弹 / 自身 / 目标型 / 范围型 / 光环 / Buff”六个页签，按行列显示 EffectScreenshots 目录下的特效截图。
    /// 导弹看截图文件名 _Missile 后缀，其余按 EffectConfig.Area（自身/目标/范围/光环/Buff）分页签。
    /// 负面特效红框、正面（增益）特效绿框，伤害类灰框；缩略图左上显示资源路径，右下显示 EffectConfig 的 Area，
    /// 右上显示引用计数（导弹取 SkillConfig.HitEffect，Buff 取 BuffConfig.BuffEffect，其余取技能特效字段），
    /// 左下黄色标签显示 JobConfig/SoldierConfig.HitEffect 的额外命中数。
    /// 双击缩略图或点“确定”返回选中的特效名（EffectConfig.Name，即技能配置里存的值）。
    /// </summary>
    public class EffectPickerForm : Form
    {
        private const int ThumbSize = 90;      // 缩略图（比初版缩小 30%）
        private const int ItemPad = 9;
        private const int NameHeight = 18;
        private const int ItemWidth = ThumbSize + ItemPad * 2 + 6;
        private const int ItemHeight = ThumbSize + ItemPad * 2 + NameHeight;
        private const int BadgeWidth = 34;

        private const int TabMissile = 0;
        private const int TabSelf = 1;
        private const int TabTarget = 2;
        private const int TabArea = 3;
        private const int TabAura = 4;
        private const int TabBuff = 5;
        private static readonly string[] TabTitles = { "导弹", "自身", "目标型", "范围型", "光环", "Buff" };
        // 各页签主色（与 EffectConfig.Area 的枚举配色一致）
        private static readonly Color[] TabColors =
        {
            Color.FromArgb(85, 91, 96),    // 导弹：灰
            Color.FromArgb(47, 125, 78),   // 自身：绿
            Color.FromArgb(53, 95, 143),   // 目标：蓝
            Color.FromArgb(160, 100, 40),  // 范围：橙
            Color.FromArgb(76, 156, 149),  // 光环：青
            Color.FromArgb(107, 74, 143),  // Buff：紫
        };

        private static readonly Color PanelBg = Color.FromArgb(37, 37, 38);
        private static readonly Color ItemBg = Color.FromArgb(45, 45, 48);
        private static readonly Color NegativeColor = Color.FromArgb(225, 70, 70);   // 负面：红框
        private static readonly Color PositiveColor = Color.FromArgb(70, 200, 100);  // 正面：绿框
        private static readonly Color DamageColor = Color.FromArgb(150, 155, 165);   // 伤害：灰框
        private static readonly Color SelectedColor = Color.FromArgb(255, 215, 64);

        private readonly string effectConfigPath;
        private readonly string screenshotDir;
        private readonly string currentValue;
        private readonly EffectUsageStats usageStats;

        private readonly TabControl tabControl;
        private readonly TabPage[] tabPages = new TabPage[TabTitles.Length];
        private readonly FlowLayoutPanel[] tabPanels = new FlowLayoutPanel[TabTitles.Length];
        private readonly Label tipLabel;
        private readonly List<EffectItemControl> itemControls = new List<EffectItemControl>();
        private EffectItemControl selected;
        // 打开时默认选中的缩略图（等窗体显示后再切页签并滚动到可视区域）
        private EffectItemControl pendingDefault;

        /// <summary>选中返回的特效名（EffectConfig.Name），未选择时为 null</summary>
        public string SelectedEffect { get; private set; }

        /// <param name="usageStats">特效引用统计（key 为 EffectConfig.Name），可为 null</param>
        public EffectPickerForm(string effectConfigPath, string screenshotDir, string currentValue,
            EffectUsageStats usageStats)
        {
            this.effectConfigPath = effectConfigPath;
            this.screenshotDir = screenshotDir;
            this.currentValue = currentValue ?? "";
            this.usageStats = usageStats;

            Text = "选择特效";
            StartPosition = FormStartPosition.CenterParent;
            ShowInTaskbar = false;
            MinimizeBox = false;
            // 初版 1120x800，各放大 30%，超出屏幕工作区时收窄
            var wa = Screen.PrimaryScreen.WorkingArea;
            Size = new Size(Math.Min(1456, wa.Width - 40), Math.Min(1040, wa.Height - 40));
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

            tabControl = new TabControl();
            tabControl.Dock = DockStyle.Fill;
            tabControl.Font = new Font("微软雅黑", 10F);
            tabControl.BackColor = PanelBg;
            tabControl.DrawMode = TabDrawMode.OwnerDrawFixed;
            tabControl.ItemSize = new Size(112, 32);
            tabControl.SizeMode = TabSizeMode.Fixed;
            tabControl.DrawItem += TabControl_DrawItem;
            for (int i = 0; i < tabPages.Length; i++)
            {
                var page = new TabPage(TabTitles[i]);
                page.UseVisualStyleBackColor = false;
                page.BackColor = ScaleColor(TabColors[i], 0.22f);

                var flow = new FlowLayoutPanel();
                flow.Dock = DockStyle.Fill;
                flow.AutoScroll = true;
                flow.WrapContents = true;
                flow.BackColor = ScaleColor(TabColors[i], 0.22f);
                flow.Padding = new Padding(8);
                page.Controls.Add(flow);

                tabPanels[i] = flow;
                tabPages[i] = page;
                tabControl.TabPages.Add(page);
            }
            Controls.Add(tabControl);
            tabControl.BringToFront();

            CancelButton = btnCancel;
            KeyPreview = true;
            KeyDown += EffectPickerForm_KeyDown;

            LoadItems();
        }

        // 页签自绘：按 Area 主色区分，选中页签更亮并带底部亮条
        private void TabControl_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0 || e.Index >= tabControl.TabPages.Count) return;

            bool selected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;
            Color baseColor = TabColors[e.Index % TabColors.Length];
            Color bgColor = selected ? baseColor : ScaleColor(baseColor, 0.55f);

            var rect = e.Bounds;
            using (var brush = new SolidBrush(bgColor))
                e.Graphics.FillRectangle(brush, rect);

            if (selected)
            {
                using (var brush = new SolidBrush(ScaleColor(baseColor, 1.25f)))
                    e.Graphics.FillRectangle(brush, new Rectangle(rect.X, rect.Bottom - 3, rect.Width, 3));
            }

            using (var pen = new Pen(Color.FromArgb(25, 25, 28)))
                e.Graphics.DrawRectangle(pen, rect.X, rect.Y, rect.Width - 1, rect.Height - 1);

            var textRect = new Rectangle(rect.X + 4, rect.Y, rect.Width - 8, rect.Height - (selected ? 2 : 0));
            TextRenderer.DrawText(e.Graphics, tabControl.TabPages[e.Index].Text, tabControl.Font, textRect, Color.White,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix | TextFormatFlags.EndEllipsis);
        }

        private static Color ScaleColor(Color color, float factor)
        {
            return Color.FromArgb(
                ClampColor(color.R * factor),
                ClampColor(color.G * factor),
                ClampColor(color.B * factor));
        }

        private static int ClampColor(float value)
        {
            return Math.Max(0, Math.Min(255, (int)value));
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
                item.TabIndex = isMissile ? TabMissile : GetAreaTab(area);
                item.Usage = GetUsageCount(isMissile, area, item.Value);
                item.ExtraUsage = GetCount(usageStats != null ? usageStats.ExtraCounts : null, item.Value);
                item.Thumb = LoadThumbnail(file);

                var ctl = new EffectItemControl(item);
                ctl.Click += ItemControl_Click;
                ctl.DoubleClick += ItemControl_DoubleClick;
                itemControls.Add(ctl);
                tabPanels[item.TabIndex].Controls.Add(ctl);
            }

            for (int i = 0; i < tabPages.Length; i++)
                tabPages[i].Text = string.Format("{0} ({1})", TabTitles[i], tabPanels[i].Controls.Count);
            tabControl.Invalidate();

            if (itemControls.Count == 0)
            {
                tipLabel.Text = "截图目录暂无图片：" + screenshotDir;
                return;
            }

            tipLabel.Text = string.Format("共 {0} 张（红框=负面，绿框=正面，灰框=伤害；“用N”=导弹取技能HitEffect、Buff取Buff表、其余取技能特效；黄标=职业/小兵HitEffect）　　当前值：{1}",
                itemControls.Count, string.IsNullOrEmpty(currentValue) ? "（空）" : currentValue);

            SelectDefault();
        }

        private int GetUsageCount(bool isMissile, string area, string effectName)
        {
            if (usageStats == null || string.IsNullOrEmpty(effectName)) return 0;
            if (isMissile)
                return GetCount(usageStats.MissileCounts, effectName);
            if (string.Equals((area ?? "").Trim(), "Buff", StringComparison.OrdinalIgnoreCase))
                return GetCount(usageStats.BuffCounts, effectName);
            return GetCount(usageStats.SkillCounts, effectName);
        }

        private static int GetCount(Dictionary<string, int> counts, string effectName)
        {
            if (counts == null || string.IsNullOrEmpty(effectName)) return 0;
            int count;
            return counts.TryGetValue(effectName, out count) ? count : 0;
        }

        // 默认选中当前字符串对应的截图（同名特效优先取本体，其次导弹），并切到它所在的页签
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
            // 窗体真正显示后才能可靠切页签/滚动，把默认选中的缩略图带到可视区域
            if (pendingDefault != null)
            {
                tabControl.SelectedIndex = pendingDefault.Item.TabIndex;
                tabPanels[pendingDefault.Item.TabIndex].ScrollControlIntoView(pendingDefault);
            }
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

        // 按 EffectConfig.Area 分页签：自身 → 自身、范围 → 范围型、光环 → 光环、Buff → Buff、其余归目标型
        private static int GetAreaTab(string area)
        {
            if (area == "自身") return TabSelf;
            if (area == "范围") return TabArea;
            if (area == "光环") return TabAura;
            if (area == "Buff") return TabBuff;
            return TabTarget;
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
            public int TabIndex;        // 所属页签（导弹/目标型/范围型）
            public int Usage = 0;       // 主引用计数（导弹/Buff/技能按类型取不同表）
            public int ExtraUsage = 0;  // JobConfig/SoldierConfig 额外命中数（黄色标签）
        }

        private class EffectItemControl : Control
        {
            public readonly EffectItem Item;
            public bool IsSelected;

            private readonly Font nameFont = new Font("微软雅黑", 8F);
            private readonly Font overlayFont = new Font("微软雅黑", 7F);

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

                // 左上：资源路径（半透明黑底 + 省略号）；右上：引用计数
                var topRect = new Rectangle(imgRect.X + 1, imgRect.Y + 1, imgRect.Width - 2, 18);
                if (topRect.Width > BadgeWidth + 10)
                {
                    var badgeRect = new Rectangle(topRect.Right - BadgeWidth, topRect.Y, BadgeWidth, topRect.Height);
                    DrawOverlayText(g, Item.Usage.ToString(), badgeRect, ContentAlignment.MiddleRight, Item.Usage > 0);
                    topRect.Width -= BadgeWidth + 1;
                }
                DrawOverlayText(g, Item.PathName, topRect, ContentAlignment.MiddleLeft, false);

                // 底部：左侧黄色额外计数（Job/Soldier 命中），右侧 EffectConfig.Area
                var bottomRect = new Rectangle(imgRect.X + 1, imgRect.Bottom - 19, imgRect.Width - 2, 18);
                if (Item.ExtraUsage > 0)
                {
                    var extraRect = new Rectangle(bottomRect.X, bottomRect.Y, BadgeWidth, bottomRect.Height);
                    DrawOverlayText(g, Item.ExtraUsage.ToString(), extraRect, ContentAlignment.MiddleLeft, false,
                        Color.FromArgb(235, 198, 40), Color.FromArgb(35, 30, 10));
                    bottomRect.X += BadgeWidth + 1;
                    bottomRect.Width -= BadgeWidth + 1;
                }
                if (!string.IsNullOrEmpty(Item.Area))
                    DrawOverlayText(g, Item.Area, bottomRect, ContentAlignment.MiddleRight, false);

                // 底部：截图文件名
                var nameRect = new Rectangle(2, imgRect.Bottom + 2, Width - 4, NameHeight - 2);
                TextRenderer.DrawText(g, Item.DisplayName, nameFont, nameRect,
                    Color.FromArgb(215, 215, 215),
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);
            }

            private void DrawOverlayText(Graphics g, string text, Rectangle rect, ContentAlignment align, bool highlight,
                Color? backOverride = null, Color? textOverride = null)
            {
                if (string.IsNullOrEmpty(text)) return;

                Color backColor = backOverride ?? (highlight ? Color.FromArgb(190, 60, 130, 70) : Color.FromArgb(175, 0, 0, 0));
                using (var back = new SolidBrush(backColor))
                {
                    g.FillRectangle(back, rect);
                }

                var flags = TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix;
                flags |= align == ContentAlignment.MiddleRight ? TextFormatFlags.Right : TextFormatFlags.Left;

                var textRect = new Rectangle(rect.X + 3, rect.Y, rect.Width - 6, rect.Height);
                TextRenderer.DrawText(g, text, overlayFont, textRect, textOverride ?? Color.White, flags);
            }
        }
    }
}
