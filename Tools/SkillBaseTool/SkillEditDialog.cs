using System;
using System.Drawing;
using System.Windows.Forms;

namespace SkillBaseTool
{
    /// <summary>单个技能编辑弹窗：内容即原「技能 + 组件 试算」，确认后写回 Template / Modules。</summary>
    public sealed class SkillEditDialog : Form
    {
        private readonly ScoreView view;
        private readonly Panel bottom;
        private readonly Button btnOk;
        private readonly Button btnCancel;
        private readonly Label lblHint;

        public SkillEditDialog(DataStore store, SkillRow skill)
        {
            Text = "编辑技能 · " + skill.Display;
            StartPosition = FormStartPosition.CenterParent;
            Size = new Size(1200, 760);
            MinimumSize = new Size(980, 600);
            BackColor = Theme.Bg;
            ForeColor = Theme.Text;
            Font = Theme.Base;

            bottom = new Panel { Dock = DockStyle.Bottom, Height = 58, BackColor = Theme.Card, Padding = new Padding(16, 12, 16, 12) };
            lblHint = new Label
            {
                Text = "确定后写回 SkillConfig 的 Template / Modules（同 Sname 五级一起）",
                Font = Theme.Small,
                ForeColor = Theme.Muted,
                AutoSize = true,
                Location = new Point(16, 20),
            };
            btnOk = MakeButton("确定（回写）", 120);
            btnOk.BackColor = Theme.Accent;
            btnOk.ForeColor = ColorTranslator.FromHtml("#1A1A1A");
            btnOk.Click += (s, e) => Confirm();
            btnCancel = MakeButton("取消", 90);
            btnCancel.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };

            bottom.Controls.AddRange(new Control[] { lblHint, btnOk, btnCancel });
            bottom.Resize += (s, e) => LayoutButtons();

            view = new ScoreView(store, skill) { Dock = DockStyle.Fill };

            Controls.Add(view);
            Controls.Add(bottom);
            LayoutButtons();
        }

        private static Button MakeButton(string text, int width)
        {
            var b = new Button
            {
                Text = text,
                FlatStyle = FlatStyle.Flat,
                BackColor = Theme.Card2,
                ForeColor = Theme.Text,
                Size = new Size(width, 32),
            };
            b.FlatAppearance.BorderColor = Theme.Line;
            return b;
        }

        private void LayoutButtons()
        {
            btnOk.Left = bottom.ClientSize.Width - btnOk.Width - 16;
            btnOk.Top = 13;
            btnCancel.Left = btnOk.Left - btnCancel.Width - 10;
            btnCancel.Top = 13;
        }

        private void Confirm()
        {
            try
            {
                view.ApplyToSkill();
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "写回失败：" + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
