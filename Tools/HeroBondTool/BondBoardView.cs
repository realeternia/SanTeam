using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace HeroBondTool
{
    /// <summary>拖拽载荷：从哪个羁绊组拖出的哪位英雄。</summary>
    public sealed class HeroDragData
    {
        public int HeroId;
        public int FromGroupId;

        public HeroDragData(int heroId, int fromGroupId)
        {
            HeroId = heroId;
            FromGroupId = fromGroupId;
        }
    }

    /// <summary>羁绊成员小方块（可拖拽）。</summary>
    public sealed class HeroCell : Control
    {
        public HeroInfo Hero;
        public int FromGroupId;
        public bool IsPresent;
        public bool IsBad;
        public int Quality;
        public string Side;

        public HeroCell(DataStore store, FriendGroup group, HeroInfo hero, int quality, string side, bool present)
        {
            Hero = hero;
            FromGroupId = group.Id;
            IsPresent = present;
            Quality = quality;
            Side = side;
            Store = store;
            Size = new Size(96, 64);
            BackColor = Theme.Card2;
            AllowDrop = true;
            Cursor = Cursors.Hand;
            DoubleBuffered = true;
        }

        public DataStore Store { get; private set; }

        /// <summary>原地更新成员格状态，仅在有变化时重绘自身。</summary>
        public void UpdateState(int quality, string side, bool present, bool isBad, bool badGroup)
        {
            bool changed = Quality != quality || Side != side || IsPresent != present || IsBad != isBad;
            Color back = badGroup ? Theme.DangerBg : Theme.Card2;
            if (BackColor.ToArgb() != back.ToArgb())
            {
                BackColor = back;
                changed = true;
            }
            if (!changed)
                return;

            Quality = quality;
            Side = side;
            IsPresent = present;
            IsBad = isBad;
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var rect = new Rectangle(0, 0, Width - 1, Height - 1);
            Color back = IsPresent ? ColorTranslator.FromHtml("#1B2027") : ColorTranslator.FromHtml("#171A1E");
            Color border = IsBad ? Theme.Danger : Theme.Line;
            Theme.FillRound(e.Graphics, rect, 6, back);
            Theme.DrawRoundBorder(e.Graphics, rect, 6, border, IsBad ? 1.6f : 1f);

            Color qualityColor = ColorUtil.QualityColor(Quality);
            if (!IsPresent)
                qualityColor = ColorTranslator.FromHtml("#3A4048");
            Theme.FillRound(e.Graphics, new Rectangle(2, 2, rect.Width - 4, 4), 2, qualityColor);

            Color nameColor = IsPresent ? Theme.Text : Theme.Faint;
            Theme.DrawEllipsis(e.Graphics, Hero.Name, Theme.Bold, nameColor, 8, 12, Width - 34);

            string qText = Quality > 0 ? "Q" + Quality : "—";
            Theme.DrawTextRight(e.Graphics, qText, Theme.Small, IsPresent ? qualityColor : Theme.Faint, Width - 7, 12);

            JobInfo job = Store.GetJob(Hero.Job);
            string jobText = job != null ? job.Name : (string.IsNullOrEmpty(Hero.Job) ? "未知" : Hero.Job);
            Theme.DrawEllipsis(e.Graphics, jobText, Theme.Small, IsPresent ? Theme.Muted : Theme.Faint, 8, 33, Width - 12);

            ForceInfo force = Store.GetForce(Side);
            Color sideColor = force != null ? ColorUtil.Brighten(force.Color) : Theme.Muted;
            Theme.DrawEllipsis(e.Graphics, string.IsNullOrEmpty(Side) ? "不在场" : Side, Theme.Small,
                IsPresent ? sideColor : Theme.Faint, 8, 47, Width - 12);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button != MouseButtons.Left)
                return;
            var data = new DataObject();
            data.SetData(typeof(HeroDragData), new HeroDragData(Hero.Id, FromGroupId));
            DoDragDrop(data, DragDropEffects.Move);
        }

        protected override void OnDragEnter(DragEventArgs drgevent)
        {
            base.OnDragEnter(drgevent);
            if (drgevent.Data.GetDataPresent(typeof(HeroDragData)))
                drgevent.Effect = DragDropEffects.Move;
        }
    }

    /// <summary>一个羁绊组大方块：标题/类型/成员格/违规信息，违规时背景红色。</summary>
    public sealed class BondCard : Panel
    {
        private const int CardWidth = 430;
        private const int HeaderHeight = 58;
        private const int CellWidth = 96;
        private const int CellHeight = 64;
        private const int CellGap = 6;
        private const int PadX = 14;

        private readonly DataStore store;
        private readonly FriendGroup group;
        private ScenarioAnalysis analysis;
        private List<string> checks;
        private HashSet<int> badIds;
        private readonly TextBox txtType;
        private readonly List<HeroCell> cells = new List<HeroCell>();
        private readonly Dictionary<int, HeroCell> cellById = new Dictionary<int, HeroCell>();
        private string lastSignature;
        private bool dragOver;

        public event Action GroupsChanged;
        public event Action TypeChanged;

        public BondCard(DataStore store, FriendGroup group, ScenarioAnalysis analysis, GroupViolation violation)
        {
            this.store = store;
            this.group = group;
            this.analysis = analysis;
            checks = violation != null ? violation.Checks : new List<string>();
            badIds = violation != null ? violation.BadHeroIds : new HashSet<int>();

            DoubleBuffered = true;
            Width = CardWidth;
            AllowDrop = true;
            BackColor = Theme.Card2;
            Margin = new Padding(0, 0, 14, 14);

            txtType = new TextBox();
            txtType.BorderStyle = BorderStyle.FixedSingle;
            txtType.BackColor = ColorTranslator.FromHtml("#161A20");
            txtType.ForeColor = Theme.Text;
            txtType.Font = Theme.Base;
            txtType.Width = 96;
            txtType.Location = new Point(CardWidth - 14 - txtType.Width, 8);
            txtType.PlaceholderText = "智谋/伤害/状态";
            txtType.Text = group.Type;
            txtType.TextChanged += (s, e) =>
            {
                if (group.Type == txtType.Text)
                    return;
                group.Type = txtType.Text;
                store.DirtyGroupIds.Add(group.Id);
                if (TypeChanged != null)
                    TypeChanged();
                Invalidate();
            };
            Controls.Add(txtType);

            SyncCells();
            Height = ComputeHeight();
            lastSignature = ComputeSignature(analysis, checks);

            DragEnter += OnCardDragEnter;
            DragLeave += (s, e) =>
            {
                dragOver = false;
                Invalidate();
            };
            DragDrop += OnCardDragDrop;
        }

        /// <summary>按成员列表差量同步格子：已有的原地更新，只增删真正变化的格子。</summary>
        private bool SyncCells()
        {
            bool changed = false;

            var wanted = new HashSet<int>(group.Heros);
            var removeIds = new List<int>();
            foreach (int heroId in cellById.Keys)
                if (!wanted.Contains(heroId))
                    removeIds.Add(heroId);
            foreach (int heroId in removeIds)
            {
                HeroCell old = cellById[heroId];
                cellById.Remove(heroId);
                cells.Remove(old);
                old.Dispose();
                changed = true;
            }

            // 展示顺序：本剧本出场品质从高到低；同品质保持配置顺序；不在场的排最后
            List<int> ordered = new List<int>(group.Heros);
            var sortQuality = new Dictionary<int, int>();
            var sortIndex = new Dictionary<int, int>();
            for (int i = 0; i < ordered.Count; i++)
            {
                int id = ordered[i];
                sortIndex[id] = i;
                HeroInfo h = store.HeroById.ContainsKey(id) ? store.HeroById[id] : null;
                sortQuality[id] = h != null && h.IsPresent(analysis.ScenarioId)
                    ? h.GetQuality(analysis.ScenarioId)
                    : -1;
            }
            ordered.Sort((x, y) =>
            {
                if (sortQuality[x] != sortQuality[y])
                    return sortQuality[y] - sortQuality[x];
                return sortIndex[x] - sortIndex[y];
            });

            bool badGroup = checks.Count > 0;
            int index = 0;
            foreach (int heroId in ordered)
            {
                HeroInfo hero = store.HeroById.ContainsKey(heroId) ? store.HeroById[heroId] : null;
                if (hero == null)
                    continue;

                bool isPresent = hero.IsPresent(analysis.ScenarioId);
                // 不在场的成员也显示品质/国家，取其它剧本的值并灰显
                int quality = isPresent ? hero.GetQuality(analysis.ScenarioId) : hero.GetReferenceQuality();
                string side = isPresent ? hero.GetSide(analysis.ScenarioId) : hero.GetReferenceSide();

                HeroCell cell;
                if (!cellById.TryGetValue(heroId, out cell))
                {
                    cell = new HeroCell(store, group, hero, quality, side, isPresent);
                    cell.DragEnter += OnCellDragEnter;
                    cell.DragDrop += OnCardDragDrop;
                    cellById[heroId] = cell;
                    cells.Add(cell);
                    Controls.Add(cell);
                    changed = true;
                }

                cell.UpdateState(quality, side, isPresent, badIds.Contains(heroId), badGroup);

                int col = index % 4;
                int row = index / 4;
                var loc = new Point(PadX + col * (CellWidth + CellGap),
                    HeaderHeight + row * (CellHeight + CellGap) + 4);
                if (cell.Location != loc)
                {
                    cell.Location = loc;
                    changed = true;
                }
                index++;
            }
            return changed;
        }

        /// <summary>用新的剧本分析结果原地更新卡片；无变化时不重绘（避免整页闪烁）。</summary>
        public bool UpdateFrom(ScenarioAnalysis newAnalysis, GroupViolation violation)
        {
            List<string> newChecks = violation != null ? violation.Checks : new List<string>();
            HashSet<int> newBadIds = violation != null ? violation.BadHeroIds : new HashSet<int>();
            string signature = ComputeSignature(newAnalysis, newChecks);
            if (signature == lastSignature)
                return false;

            bool checksChanged = string.Join("；", checks.ToArray()) != string.Join("；", newChecks.ToArray());

            analysis = newAnalysis;
            checks = newChecks;
            badIds = newBadIds;
            lastSignature = signature;

            bool structureChanged = SyncCells();
            int height = ComputeHeight();
            if (Height != height)
                Height = height;
            if (structureChanged || checksChanged)
                Invalidate();
            return true;
        }

        private string ComputeSignature(ScenarioAnalysis newAnalysis, List<string> newChecks)
        {
            var sb = new System.Text.StringBuilder();
            sb.Append(group.Level).Append('|').Append(group.Type).Append('|');
            foreach (int heroId in group.Heros)
            {
                HeroInfo hero = store.HeroById.ContainsKey(heroId) ? store.HeroById[heroId] : null;
                sb.Append(heroId).Append(':');
                if (hero == null)
                {
                    sb.Append("?;");
                    continue;
                }
                if (hero.IsPresent(newAnalysis.ScenarioId))
                    sb.Append(hero.Name).Append('/')
                        .Append(hero.GetQuality(newAnalysis.ScenarioId)).Append('/')
                        .Append(hero.GetSide(newAnalysis.ScenarioId));
                else
                    sb.Append(hero.Name).Append("/absent");
                sb.Append(';');
            }
            sb.Append("|").Append(string.Join("；", newChecks.ToArray()));
            return sb.ToString();
        }

        private int MemberRows
        {
            get
            {
                int count = 0;
                foreach (int id in group.Heros)
                    if (store.HeroById.ContainsKey(id))
                        count++;
                return Math.Max(1, (count + 3) / 4);
            }
        }

        private string CheckText
        {
            get { return checks.Count == 0 ? "" : "违规：" + string.Join("；", checks.ToArray()); }
        }

        private int ComputeHeight()
        {
            int h = HeaderHeight + MemberRows * (CellHeight + CellGap) + 6;
            if (checks.Count > 0)
                h += 8 + Theme.MeasureText(CheckText, Theme.Small, CardWidth - PadX * 2) + 10;
            return h + 8;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            bool bad = checks.Count > 0;
            Color back = bad ? Theme.DangerBg : Theme.Card2;
            Color border = bad ? Theme.Danger : Theme.Line;
            if (dragOver)
                border = Theme.Accent;

            var rect = new Rectangle(0, 0, Width - 1, Height - 1);
            Theme.FillRound(e.Graphics, rect, 8, back);
            Theme.DrawRoundBorder(e.Graphics, rect, 8, border, dragOver ? 2f : 1.2f);

            Theme.DrawEllipsis(e.Graphics, group.Name, Theme.Title, Theme.Text, PadX, 10,
                CardWidth - PadX - 14 - 96 - 44);
            string meta = string.Format("Lv{0} · {1}/{2} 人", group.Level, MemberCount(), group.Heros.Count);
            Theme.DrawText(e.Graphics, meta, Theme.Small, Theme.Muted, PadX, 36);

            if (bad)
            {
                var badge = new Rectangle(Width - 190, 34, 52, 18);
                Theme.FillRound(e.Graphics, badge, 4, Color.FromArgb(60, 224, 90, 90));
                Theme.DrawText(e.Graphics, "违规", Theme.Small, Theme.Danger, badge.X + 12, badge.Y + 2);
            }

            Theme.DrawText(e.Graphics, "类型", Theme.Small, Theme.Muted, CardWidth - 14 - 96 - 34, 13);

            if (checks.Count > 0)
            {
                int y = HeaderHeight + MemberRows * (CellHeight + CellGap) + 6;
                Theme.DrawWrapped(e.Graphics, CheckText, Theme.Small, ColorTranslator.FromHtml("#FFB3A7"),
                    PadX, y, Width - PadX * 2);
            }
        }

        private int MemberCount()
        {
            int n = 0;
            foreach (int id in group.Heros)
                if (store.HeroById.ContainsKey(id) && store.HeroById[id].IsPresent(analysis.ScenarioId))
                    n++;
            return n;
        }

        private void OnCellDragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(HeroDragData)))
                e.Effect = DragDropEffects.Move;
        }

        private void OnCardDragEnter(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(typeof(HeroDragData)))
                return;
            e.Effect = DragDropEffects.Move;
            dragOver = true;
            Invalidate();
        }

        private void OnCardDragDrop(object sender, DragEventArgs e)
        {
            dragOver = false;
            Invalidate();
            if (!e.Data.GetDataPresent(typeof(HeroDragData)))
                return;
            HeroDragData data = (HeroDragData)e.Data.GetData(typeof(HeroDragData));
            if (data == null || data.FromGroupId == group.Id)
                return;
            if (!store.MoveHero(data.HeroId, data.FromGroupId, group.Id))
                return;
            if (GroupsChanged != null)
                GroupsChanged();
        }
    }

    /// <summary>剧本羁绊页：所有羁绊组卡片，支持拖动成员换组。</summary>
    public sealed class BondBoardView : Panel
    {
        private readonly DataStore store;
        private readonly int scenarioId;
        private readonly FlowLayoutPanel flow;
        private readonly Dictionary<int, BondCard> cards = new Dictionary<int, BondCard>();

        public event Action GroupsChanged;
        public event Action TypeChanged;

        public BondBoardView(DataStore store, int scenarioId, ScenarioAnalysis analysis)
        {
            this.store = store;
            this.scenarioId = scenarioId;
            BackColor = Theme.Bg;
            DoubleBuffered = true;

            var hint = new Label();
            hint.Dock = DockStyle.Top;
            hint.Height = 46;
            hint.Padding = new Padding(16, 8, 16, 0);
            hint.ForeColor = Theme.Muted;
            hint.BackColor = ColorTranslator.FromHtml("#181C22");
            hint.Font = Theme.Small;
            hint.Text = "拖拽成员小方块到其它羁绊组即可换组；灰显方块 = 该英雄在本剧本不出场（不参与人数与合规统计）；"
                + "类型可直接编辑。羁绊成员为全局配置，修改会影响所有剧本，改完点右上「保存修改」写回 HeroFriendConfig_s.cs。";

            flow = new FlowLayoutPanel();
            flow.Dock = DockStyle.Fill;
            flow.AutoScroll = true;
            flow.BackColor = Theme.Bg;
            flow.Padding = new Padding(16);
            flow.WrapContents = true;

            Controls.Add(flow);
            Controls.Add(hint);
            SetData(analysis);
        }

        public void SetData(ScenarioAnalysis analysis)
        {
            int scrollX = -flow.AutoScrollPosition.X;
            int scrollY = -flow.AutoScrollPosition.Y;

            bool hasNewCard = false;
            foreach (FriendGroup group in store.Groups)
            {
                if (!cards.ContainsKey(group.Id))
                {
                    hasNewCard = true;
                    break;
                }
            }
            // 仅在新增卡片时才整体挂起布局，普通刷新只更新有变化的卡片/格子
            if (hasNewCard)
                flow.SuspendLayout();

            var violationByGroup = new Dictionary<int, GroupViolation>();
            foreach (GroupViolation v in analysis.Violations)
                violationByGroup[v.Group.Id] = v;

            foreach (FriendGroup group in store.Groups)
            {
                GroupViolation violation = violationByGroup.ContainsKey(group.Id) ? violationByGroup[group.Id] : null;
                BondCard card;
                if (cards.TryGetValue(group.Id, out card))
                {
                    card.UpdateFrom(analysis, violation);
                    continue;
                }

                card = new BondCard(store, group, analysis, violation);
                card.GroupsChanged += () =>
                {
                    if (GroupsChanged != null)
                        GroupsChanged();
                };
                card.TypeChanged += () =>
                {
                    if (TypeChanged != null)
                        TypeChanged();
                };
                cards[group.Id] = card;
                flow.Controls.Add(card);
            }

            if (hasNewCard)
                flow.ResumeLayout();
            if (-flow.AutoScrollPosition.X != scrollX || -flow.AutoScrollPosition.Y != scrollY)
                flow.AutoScrollPosition = new Point(scrollX, scrollY);
        }

        public int ScenarioId
        {
            get { return scenarioId; }
        }
    }
}
