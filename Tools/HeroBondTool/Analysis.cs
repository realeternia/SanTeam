using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace HeroBondTool
{
    /// <summary>统计卡片里的一行（条形图）。</summary>
    public sealed class BarRow
    {
        public string Label;
        public string Title;
        public int Count;
        public Color Color;
    }

    /// <summary>剧本内出场的英雄（品质 + 阵营）。</summary>
    public sealed class HeroSlot
    {
        public HeroInfo Hero;
        public int Quality;
        public string Side;
    }

    public sealed class GroupViolation
    {
        public FriendGroup Group;
        public List<string> Checks = new List<string>();
        public HashSet<int> BadHeroIds = new HashSet<int>();
        public List<HeroSlot> Members = new List<HeroSlot>();
    }

    /// <summary>单个剧本的统计结果（对应 python 报告的 panel）。</summary>
    public sealed class ScenarioAnalysis
    {
        public int ScenarioId;
        public int Total;
        public int SideCount;
        public int JobCount;
        public int ActiveGroupCount;
        public int GroupCount;
        public int ViolationCount;

        public List<BarRow> SideRows = new List<BarRow>();
        public List<BarRow> QualityRows = new List<BarRow>();
        public List<BarRow> JobRows = new List<BarRow>();
        public List<BarRow> BondRows = new List<BarRow>();
        public List<GroupViolation> Violations = new List<GroupViolation>();

        /// <summary>羁绊组在本剧本的出场成员（按配置顺序），用于看板卡片。</summary>
        public Dictionary<int, List<HeroSlot>> PresentByGroup = new Dictionary<int, List<HeroSlot>>();
    }

    public static class Analysis
    {
        public const int BondMaxQuality = 2;
        public const int BondMaxJob = 2;
        public const int BondMaxSide = 3;

        public static ScenarioAnalysis Build(DataStore store, int scenarioId)
        {
            var result = new ScenarioAnalysis();
            result.ScenarioId = scenarioId;
            result.GroupCount = store.Groups.Count;

            // 出场英雄
            var slots = new List<HeroSlot>();
            foreach (HeroInfo hero in store.Heroes)
            {
                if (!hero.IsPresent(scenarioId))
                    continue;
                var slot = new HeroSlot();
                slot.Hero = hero;
                slot.Quality = hero.GetQuality(scenarioId);
                slot.Side = hero.GetSide(scenarioId);
                slots.Add(slot);
            }
            result.Total = slots.Count;

            // 国家分布
            var sideCounts = new Dictionary<string, int>();
            foreach (HeroSlot slot in slots)
            {
                string side = string.IsNullOrEmpty(slot.Side) ? "未知" : slot.Side;
                sideCounts[side] = (sideCounts.ContainsKey(side) ? sideCounts[side] : 0) + 1;
            }
            result.SideCount = sideCounts.Count;
            foreach (KeyValuePair<string, int> kv in sideCounts)
            {
                ForceInfo force = store.GetForce(kv.Key);
                result.SideRows.Add(new BarRow
                {
                    Label = kv.Key,
                    Count = kv.Value,
                    Color = force != null ? ColorUtil.Brighten(force.Color) : ColorUtil.Unknown,
                    Title = string.Format("{0}：{1} 位英雄", kv.Key, kv.Value)
                });
            }
            result.SideRows.Sort((a, b) =>
            {
                int cmp = b.Count.CompareTo(a.Count);
                if (cmp != 0)
                    return cmp;
                return OrderOf(store.ForceOrder, a.Label).CompareTo(OrderOf(store.ForceOrder, b.Label));
            });

            // 品质分布
            var qualityCounts = new SortedDictionary<int, int>();
            foreach (HeroSlot slot in slots)
                qualityCounts[slot.Quality] = (qualityCounts.ContainsKey(slot.Quality) ? qualityCounts[slot.Quality] : 0) + 1;
            result.QualityRows = qualityCounts.Select(kv => new BarRow
            {
                Label = "Q" + kv.Key + " " + ColorUtil.QualityName(kv.Key),
                Count = kv.Value,
                Color = ColorUtil.QualityColor(kv.Key),
                Title = ColorUtil.QualityName(kv.Key) + "：" + kv.Value + " 位英雄"
            }).ToList();

            // 职业分布
            var jobCounts = new Dictionary<string, int>();
            foreach (HeroSlot slot in slots)
            {
                string job = string.IsNullOrEmpty(slot.Hero.Job) ? "未知" : slot.Hero.Job;
                jobCounts[job] = (jobCounts.ContainsKey(job) ? jobCounts[job] : 0) + 1;
            }
            result.JobCount = jobCounts.Count;
            foreach (KeyValuePair<string, int> kv in jobCounts)
            {
                JobInfo job = store.GetJob(kv.Key);
                string label = job != null ? string.Format("{0}（{1}）", job.Name, job.Short) : (kv.Key == "未知" ? "未知" : kv.Key);
                result.JobRows.Add(new BarRow
                {
                    Label = label,
                    Count = kv.Value,
                    Color = ColorUtil.JobColor(OrderOf(store.JobOrder, kv.Key), Math.Max(store.JobOrder.Count, 1)),
                    Title = label + "：" + kv.Value + " 位英雄"
                });
            }
            result.JobRows.Sort((a, b) => b.Count.CompareTo(a.Count));

            // 羁绊出场成员 / 违规
            var groupMembers = new Dictionary<int, List<HeroSlot>>();
            foreach (FriendGroup group in store.Groups)
            {
                var members = new List<HeroSlot>();
                foreach (int heroId in group.Heros)
                {
                    HeroSlot slot = slots.FirstOrDefault(s => s.Hero.Id == heroId);
                    if (slot != null)
                        members.Add(slot);
                }
                groupMembers[group.Id] = members;
                result.PresentByGroup[group.Id] = members;
                result.BondRows.Add(new BarRow
                {
                    Label = group.Name,
                    Count = members.Count,
                    Color = ColorUtil.Brighten(string.IsNullOrEmpty(group.LineColor) ? "#5B9BD5" : group.LineColor),
                    Title = group.Name + "：" + members.Count + " 人"
                        + (members.Count > 0 ? "（" + string.Join("、", members.Select(x => x.Hero.Name).ToArray()) + "）" : "（本剧本无成员）")
                });
            }
            result.BondRows.Sort((a, b) =>
            {
                int cmp = b.Count.CompareTo(a.Count);
                return cmp != 0 ? cmp : string.CompareOrdinal(a.Label, b.Label);
            });
            result.ActiveGroupCount = result.BondRows.Count(r => r.Count > 0);

            // 每对英雄同时出现在哪些羁绊组（重复羁绊对检查）
            var pairGroups = new Dictionary<string, HashSet<string>>();
            foreach (FriendGroup group in store.Groups)
            {
                List<int> ids = groupMembers[group.Id].Select(s => s.Hero.Id).OrderBy(x => x).ToList();
                for (int i = 0; i < ids.Count; i++)
                {
                    for (int j = i + 1; j < ids.Count; j++)
                    {
                        string key = ids[i] + "|" + ids[j];
                        if (!pairGroups.ContainsKey(key))
                            pairGroups[key] = new HashSet<string>();
                        pairGroups[key].Add(group.Name);
                    }
                }
            }

            foreach (FriendGroup group in store.Groups)
            {
                List<HeroSlot> members = groupMembers[group.Id];
                var checks = new List<string>();
                var badIds = new HashSet<int>();

                // 同品质
                var qualityMap = new SortedDictionary<int, List<string>>();
                foreach (HeroSlot slot in members)
                {
                    if (slot.Quality <= 0)
                        continue;
                    if (!qualityMap.ContainsKey(slot.Quality))
                        qualityMap[slot.Quality] = new List<string>();
                    qualityMap[slot.Quality].Add(slot.Hero.Name);
                }
                foreach (KeyValuePair<int, List<string>> kv in qualityMap)
                {
                    if (kv.Value.Count <= BondMaxQuality)
                        continue;
                    checks.Add(string.Format("品质 Q{0}×{1}（{2}）", kv.Key, kv.Value.Count, string.Join("、", kv.Value.ToArray())));
                    foreach (string name in kv.Value)
                        badIds.Add(FindIdByName(members, name));
                }

                // 同职业
                var jobMap = new SortedDictionary<string, List<string>>();
                foreach (HeroSlot slot in members)
                {
                    string job = string.IsNullOrEmpty(slot.Hero.Job) ? "未知" : slot.Hero.Job;
                    if (!jobMap.ContainsKey(job))
                        jobMap[job] = new List<string>();
                    jobMap[job].Add(slot.Hero.Name);
                }
                foreach (KeyValuePair<string, List<string>> kv in jobMap)
                {
                    if (kv.Value.Count <= BondMaxJob)
                        continue;
                    JobInfo job = store.GetJob(kv.Key);
                    string label = job != null ? job.Name : (kv.Key == "未知" ? "未知" : kv.Key);
                    checks.Add(string.Format("职业 {0}×{1}（{2}）", label, kv.Value.Count, string.Join("、", kv.Value.ToArray())));
                    foreach (string name in kv.Value)
                        badIds.Add(FindIdByName(members, name));
                }

                // 同国家
                var sideMap = new SortedDictionary<string, List<string>>();
                foreach (HeroSlot slot in members)
                {
                    string side = string.IsNullOrEmpty(slot.Side) ? "未知" : slot.Side;
                    if (!sideMap.ContainsKey(side))
                        sideMap[side] = new List<string>();
                    sideMap[side].Add(slot.Hero.Name);
                }
                foreach (KeyValuePair<string, List<string>> kv in sideMap)
                {
                    if (kv.Value.Count <= BondMaxSide)
                        continue;
                    checks.Add(string.Format("国家 {0}×{1}（{2}）", kv.Key, kv.Value.Count, string.Join("、", kv.Value.ToArray())));
                    foreach (string name in kv.Value)
                        badIds.Add(FindIdByName(members, name));
                }

                // 重复羁绊对
                var dupPairs = new List<string>();
                List<int> memberIds = members.Select(s => s.Hero.Id).OrderBy(x => x).ToList();
                for (int i = 0; i < memberIds.Count; i++)
                {
                    for (int j = i + 1; j < memberIds.Count; j++)
                    {
                        HashSet<string> others = pairGroups[memberIds[i] + "|" + memberIds[j]];
                        var otherNames = others.Where(n => n != group.Name).OrderBy(n => n, StringComparer.Ordinal).ToList();
                        if (otherNames.Count == 0)
                            continue;
                        HeroInfo a = store.HeroById[memberIds[i]];
                        HeroInfo b = store.HeroById[memberIds[j]];
                        dupPairs.Add(string.Format("{0}-{1}（{2}）", a.Name, b.Name, string.Join("/", otherNames.ToArray())));
                    }
                }
                if (dupPairs.Count > 0)
                {
                    string shown = string.Join("、", dupPairs.Take(3).ToArray());
                    string more = dupPairs.Count > 3 ? string.Format(" 等{0}对", dupPairs.Count) : "";
                    checks.Add(string.Format("重复羁绊对 {0} 对：{1}{2}", dupPairs.Count, shown, more));
                }

                if (checks.Count == 0)
                    continue;
                var violation = new GroupViolation();
                violation.Group = group;
                violation.Checks = checks;
                violation.BadHeroIds = badIds;
                violation.Members = members;
                result.Violations.Add(violation);
            }

            result.Violations.Sort((a, b) =>
            {
                int cmp = b.Checks.Count.CompareTo(a.Checks.Count);
                return cmp != 0 ? cmp : string.CompareOrdinal(a.Group.Name, b.Group.Name);
            });
            result.ViolationCount = result.Violations.Count;
            return result;
        }

        private static int FindIdByName(List<HeroSlot> members, string name)
        {
            HeroSlot slot = members.FirstOrDefault(m => m.Hero.Name == name);
            return slot != null ? slot.Hero.Id : -1;
        }

        private static int OrderOf(Dictionary<string, int> map, string key)
        {
            int v;
            return map.TryGetValue(key, out v) ? v : 99;
        }
    }

    /// <summary>颜色工具（与 python 报告配色保持一致）。</summary>
    public static class ColorUtil
    {
        public static readonly Color Unknown = ColorTranslator.FromHtml("#7A8290");

        private static readonly Color[] QualityColors =
        {
            ColorTranslator.FromHtml("#A0A0A0"),
            ColorTranslator.FromHtml("#33CC33"),
            ColorTranslator.FromHtml("#5B7FFF"),
            ColorTranslator.FromHtml("#FFD700"),
            ColorTranslator.FromHtml("#FF4757"),
        };

        private static readonly string[] QualityNames = { "普通", "优秀", "精良", "史诗", "传说" };

        public static Color QualityColor(int quality)
        {
            if (quality >= 1 && quality <= QualityColors.Length)
                return QualityColors[quality - 1];
            return Unknown;
        }

        public static string QualityName(int quality)
        {
            if (quality >= 1 && quality <= QualityNames.Length)
                return QualityNames[quality - 1];
            return "未知";
        }

        public static Color FromHex(string hex, Color fallback)
        {
            if (string.IsNullOrEmpty(hex))
                return fallback;
            hex = hex.Trim();
            if (hex.StartsWith("#"))
                hex = hex.Substring(1);
            if (hex.Length != 6)
                return fallback;
            try
            {
                int r = Convert.ToInt32(hex.Substring(0, 2), 16);
                int g = Convert.ToInt32(hex.Substring(2, 2), 16);
                int b = Convert.ToInt32(hex.Substring(4, 2), 16);
                return Color.FromArgb(r, g, b);
            }
            catch
            {
                return fallback;
            }
        }

        /// <summary>把深色系强调色提亮到适合深色背景显示（对应 python brighten）。</summary>
        public static Color Brighten(string hex, int minMaxChannel = 200)
        {
            Color color = FromHex(hex, Unknown);
            int max = Math.Max(Math.Max(color.R, color.G), color.B);
            if (max >= minMaxChannel || max <= 0)
                return color;
            double k = (double)minMaxChannel / max;
            return Color.FromArgb(
                Math.Min(255, (int)Math.Round(color.R * k)),
                Math.Min(255, (int)Math.Round(color.G * k)),
                Math.Min(255, (int)Math.Round(color.B * k)));
        }

        /// <summary>按职业序号均匀取色（对应 python job_color：HSL 0.62/0.62）。</summary>
        public static Color JobColor(int index, int total)
        {
            double hue = ((double)index / Math.Max(total, 1) * 360 + 28) % 360;
            return HlsToRgb(hue / 360.0, 0.62, 0.62);
        }

        private static Color HlsToRgb(double h, double l, double s)
        {
            double r, g, b;
            if (s == 0)
            {
                r = g = b = l;
            }
            else
            {
                double q = l < 0.5 ? l * (1 + s) : l + s - l * s;
                double p = 2 * l - q;
                r = HueToRgb(p, q, h + 1.0 / 3);
                g = HueToRgb(p, q, h);
                b = HueToRgb(p, q, h - 1.0 / 3);
            }
            return Color.FromArgb(ToByte(r), ToByte(g), ToByte(b));
        }

        private static double HueToRgb(double p, double q, double t)
        {
            if (t < 0) t += 1;
            if (t > 1) t -= 1;
            if (t < 1.0 / 6) return p + (q - p) * 6 * t;
            if (t < 1.0 / 2) return q;
            if (t < 2.0 / 3) return p + (q - p) * (2.0 / 3 - t) * 6;
            return p;
        }

        private static int ToByte(double v)
        {
            return Math.Max(0, Math.Min(255, (int)Math.Round(v * 255)));
        }
    }
}
