using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace SkillBaseTool
{
    public sealed class GenerateResult
    {
        public Dictionary<int, string[]> Rows = new Dictionary<int, string[]>();
        public List<string> Warnings = new List<string>();
        public int TemplatedSkills;
    }

    public sealed class CompareItem
    {
        public SkillRow Skill;
        public string Template;
        public string[] Base;
        public List<string> Diffs = new List<string>();
        public bool MainValueDiff;
        public bool RhythmDiff;
        public bool BuffDiff;
        public bool HasDiff { get { return Diffs.Count > 0; } }
    }

    public sealed class ScoreResult
    {
        public double TemplateScore;
        public double ComponentScore;
        public List<string> Hints = new List<string>();
        public double Total { get { return TemplateScore + ComponentScore; } }
    }

    /// <summary>模板选择、SkillBase 展开、评分与对比的核心规则。</summary>
    public static class SkillGen
    {
        public static readonly string[] DamageTemplates =
        {
            "单体伤害大", "单体伤害中", "单体伤害小", "群体伤害近距离", "群体伤害自身中心", "跳跃弹射"
        };

        public static readonly string[] HealTemplates = { "单体治疗", "范围治疗" };

        /// <summary>Buff 短名 → 组件族。</summary>
        private static readonly Dictionary<string, string> TokenFamily = new Dictionary<string, string>
        {
            { "破", "减防" }, { "慑", "减攻" }, { "缓", "减速" }, { "疫", "减疗" }, { "伤", "增伤" },
            { "乱", "眩晕" }, { "停", "定身" }, { "惑", "混乱" }, { "硬", "减伤" }, { "暴", "暴击" },
            { "攻", "加攻" }, { "甲", "加护甲" }, { "术", "加法强" }, { "抗", "加魔抗" },
            { "快", "加攻速" }, { "速", "加移速" }, { "重", "自身增伤" }, { "盾", "护盾" },
        };

        /// <summary>组件族 → (StrengthBuff1 键, 百分比换算)。空键表示无独立数值。</summary>
        private static readonly Dictionary<string, KeyValuePair<string, int>> FamilyKey =
            new Dictionary<string, KeyValuePair<string, int>>
            {
                { "减防", new KeyValuePair<string, int>("armorDown", 1) },
                { "减攻", new KeyValuePair<string, int>("atkDown", 1) },
                { "减速", new KeyValuePair<string, int>("slow", 100) },
                { "减疗", new KeyValuePair<string, int>("healDown", 1) },
                { "增伤", new KeyValuePair<string, int>("damagedUp", 100) },
                { "减伤", new KeyValuePair<string, int>("shieldReduce", 100) },
                { "暴击", new KeyValuePair<string, int>("critRate", 100) },
                { "加攻", new KeyValuePair<string, int>("atk", 1) },
                { "加护甲", new KeyValuePair<string, int>("armor", 1) },
                { "加法强", new KeyValuePair<string, int>("ap", 1) },
                { "加魔抗", new KeyValuePair<string, int>("magicRes", 1) },
                { "加攻速", new KeyValuePair<string, int>("atkSpeed", 100) },
                { "加移速", new KeyValuePair<string, int>("moveSpeed", 100) },
                { "自身增伤", new KeyValuePair<string, int>("damageUp", 100) },
                { "护盾", new KeyValuePair<string, int>("shield", 1) },
                { "眩晕", new KeyValuePair<string, int>("", 0) },
                { "定身", new KeyValuePair<string, int>("", 0) },
                { "混乱", new KeyValuePair<string, int>("", 0) },
            };

        private static readonly Dictionary<string, string> TargetTypeModules = new Dictionary<string, string>
        {
            { "lowestHp", "低血筛选" }, { "highestHp", "最高血量" }, { "highestAp", "最高法强" },
            { "rangedOnly", "远程优先" }, { "meleeOnly", "近战优先" }, { "excludeSelf", "排除自身" },
            { "selfAlways", "施法者必含" }, { "fullHpSkip", "满血跳过" }, { "wholeMap", "全图" },
            { "tauntAroundTarget", "嘲讽周围" }, { "noDedupe", "不按首段去重" }, { "splitByRange", "近远分段" },
            { "heroOnly", "英雄限定" },
        };

        private static readonly Dictionary<string, string> TagModules = new Dictionary<string, string>
        {
            { "antiShield", "破盾" }, { "armorIgnore", "破盾" }, { "dispel", "驱散" },
            { "doubleChance", "概率双击" }, { "selfHurt", "自损" }, { "mpBurn", "燃烧法力" },
            { "shieldBonus", "按护盾加成伤害" }, { "vsJob", "兵种克制" },
        };

        public static bool IsDamageTemplate(string t)
        {
            return t != null && DamageTemplates.Contains(t);
        }

        public static bool IsHealTemplate(string t)
        {
            return t != null && HealTemplates.Contains(t);
        }

        /// <summary>按技能脚本/类型推断模板；返回 null 表示不走模板（特化技能）。</summary>
        public static string PickTemplate(SkillRow a)
        {
            string type = a.Type;
            string script = a.ScriptName;
            float dmg = a.DamageStrength;
            float heal = a.HealStrength;

            if (script == "SkillAura")
                return "光环";
            if (script == "SkillSoldierGrow" || script == "SkillSoldierArcherBuff" || script == "SkillSoldierBless" || script == "SkillSoldierFortify")
                return "兵种增益";
            if (script == "SkillJumpChain")
                return "跳跃弹射";
            if (script == "SkillSelfBuff" || script == "SkillAttackedShield" || script == "SkillDefFeedback")
                return "自身强化";
            if (heal > 0)
            {
                if (script == "SkillCastingAidAreaHeal")
                    return null;
                if (type == "单疗" || script == "SkillAidCurseHeal")
                    return "单体治疗";
                return "范围治疗";
            }
            if (type == "单增" || type == "群增" || script == "SkillAreaBuff")
            {
                if (script == "SkillAidCraftBuff")
                    return null;
                return type == "群增" ? "群体友方增益" : "单体友方增益";
            }
            if (dmg > 0)
            {
                if (script.StartsWith("SkillAreaDamage", StringComparison.Ordinal))
                    return a.TargetType.Contains("self") ? "群体伤害自身中心" : "群体伤害近距离";
                if (script == "SkillShockWave" || script == "SkillAidChargeImpale" || script == "SkillTaunt")
                    return "群体伤害近距离";
                if (script == "SkillSingleDamage" || script == "SkillAidAddBuff" || script == "SkillAidExplode")
                    return dmg >= 90 ? "单体伤害大" : (dmg >= 60 ? "单体伤害中" : "单体伤害小");
            }
            return null;
        }

        /// <summary>从手配 BuffId / TargetType / 标签反推组件（按出现顺序去重）。</summary>
        public static List<string> ExtraModules(DataStore store, SkillRow a)
        {
            var result = new List<string>();
            string sb = a.StrengthBuff1;
            foreach (string tok in a.BuffId.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                string family;
                if (!TokenFamily.TryGetValue(tok, out family))
                    continue;
                float? hand = null;
                var fk = FamilyKey[family];
                if (!string.IsNullOrEmpty(fk.Key))
                {
                    var m = Regex.Match(sb, Regex.Escape(fk.Key) + @":(-?\d+\.?\d*)");
                    if (m.Success)
                        hand = CfgUtil.ParseFloat(m.Groups[1].Value) * (fk.Value == 100 ? 100f : 1f);
                }
                string tier = PickTier(store, family, hand);
                if (tier != null && !result.Contains(tier))
                    result.Add(tier);
            }

            foreach (var kv in TargetTypeModules)
                if (a.TargetType.Contains(kv.Key) && !result.Contains(kv.Value))
                    result.Add(kv.Value);
            foreach (var kv in TagModules)
                if (a.TargetType.Contains(kv.Key) && !result.Contains(kv.Value))
                    result.Add(kv.Value);

            if (a.ScriptName == "SkillDefFeedback" && !result.Contains("伤害反射"))
                result.Add("伤害反射");
            if (a.ScriptName == "SkillTaunt")
            {
                string tier = PickTier(store, "嘲讽", a.BuffTime);
                if (tier != null && !result.Contains(tier))
                    result.Add(tier);
            }
            return result;
        }

        private static string PickTier(DataStore store, string family, float? handValue)
        {
            var tiers = new[] { family + "大", family + "中", family + "小" }
                .Where(store.ComponentByName.ContainsKey).ToList();
            if (tiers.Count == 0)
                return null;
            if (handValue == null)
                return tiers.Contains(family + "中") ? family + "中" : tiers[0];
            return tiers.OrderBy(t => Math.Abs(PrimaryValue(store.ComponentByName[t].Effect, 1) - handValue.Value)).First();
        }

        /// <summary>为品质 1/2 英雄技能自动填 Template / Modules，返回被改动的行 Id。</summary>
        public static HashSet<int> AutoAssign(DataStore store, out int assignedGroups, out List<string> skipped)
        {
            var changed = new HashSet<int>();
            assignedGroups = 0;
            skipped = new List<string>();
            foreach (var group in store.Skills.GroupBy(s => s.Sname))
            {
                var lv1 = group.FirstOrDefault(s => s.Lv == 1) ?? group.First();
                if (lv1.Quality != 1 && lv1.Quality != 2)
                    continue;
                string t = PickTemplate(lv1);
                if (string.IsNullOrEmpty(t) || !store.TemplateByName.ContainsKey(t))
                {
                    skipped.Add(lv1.Display + "（" + lv1.ScriptName + "）");
                    continue;
                }
                var mods = ExtraModules(store, lv1);
                foreach (var row in group)
                {
                    row.TemplateName = t;
                    row.Modules = mods.ToArray();
                    changed.Add(row.Id);
                }
                assignedGroups++;
            }
            return changed;
        }

        /// <summary>按模板 + 组件展开生成 SkillBaseConfig 行。</summary>
        public static GenerateResult GenerateBase(DataStore store)
        {
            var result = new GenerateResult();
            foreach (var skill in store.Skills)
            {
                string tname = skill.TemplateName;
                if (string.IsNullOrEmpty(tname))
                    continue;
                TemplateRow tpl;
                if (!store.TemplateByName.TryGetValue(tname, out tpl))
                {
                    result.Warnings.Add(skill.Display + " 引用了未知模板：" + tname);
                    continue;
                }
                result.TemplatedSkills++;
                result.Rows[skill.Id] = BuildBaseRow(store, skill, tpl, result);
            }
            return result;
        }

        private static string[] BuildBaseRow(DataStore store, SkillRow skill, TemplateRow tpl, GenerateResult result)
        {
            var writer = new BaseWriter(skill.Args);
            int lv = Math.Max(1, skill.Lv);

            // 模板主数值：按 Effect 的 k-v 写入列；跳跃弹射且原为治疗时改写到 HealStrength（沿用旧规则）
            bool jumpHeal = tpl.Name == "跳跃弹射" && skill.HealStrength > 0;
            var tplBindings = ParseEffect(tpl.Effect);
            if (jumpHeal)
            {
                foreach (var b in tplBindings)
                    if (b.Column == "DamageStrength")
                        b.Column = "HealStrength";
            }
            writer.Apply(tplBindings, lv, "模板:" + tpl.Name);
            // 主数值互斥：伤害模板清治疗，治疗模板清伤害
            if (tplBindings.Any(x => x.Column == "DamageStrength"))
                writer.B[Col.HealStrength] = "0f";
            if (tplBindings.Any(x => x.Column == "HealStrength"))
                writer.B[Col.DamageStrength] = "0f";

            // 模板结构列（保留模板专列语义，MpCost 表内单位=10）
            if (tpl.Range > 0)
                writer.B[Col.Range] = CfgUtil.NumF(tpl.Range);
            if (tpl.Area > 0)
                writer.B[Col.Area] = CfgUtil.NumF(tpl.Area);
            writer.B[Col.CD] = CfgUtil.NumF(tpl.CD);
            writer.B[Col.MpCost] = CfgUtil.NumI(tpl.MpCost * 10);

            // 模板默认 Modules 只参与估值，不注入效果；实际效果只来自技能自己的 Modules
            foreach (string module in skill.Modules)
            {
                ComponentRow comp;
                if (!store.ComponentByName.TryGetValue(module, out comp))
                {
                    writer.Warnings.Add(skill.Display + " 引用了未知组件：" + module);
                    continue;
                }
                writer.Apply(ParseEffect(comp.Effect), lv, "组件:" + comp.Name);
            }

            writer.Flush();
            result.Warnings.AddRange(writer.Warnings);
            return writer.B;
        }

        /// <summary>Effect 的一条绑定：写入哪列 / 子键，以及 Lv1~5 数值。</summary>
        public sealed class EffectBinding
        {
            public string Column;
            public string Sub;
            public float[] Values = new float[0];
            /// <summary>非数值绑定（如 vsJob=盾|士）的字面值；为空表示走 Values。</summary>
            public string Text;

            public float ValueAt(int lv)
            {
                if (Values.Length == 0)
                    return 0f;
                int idx = Math.Max(0, lv - 1);
                return idx < Values.Length ? Values[idx] : Values[Values.Length - 1];
            }
        }

        /// <summary>解析 Effect k-v（分号多绑定；Column[:sub][=v1,v2,...]，None=不写列）。</summary>
        public static List<EffectBinding> ParseEffect(string effect)
        {
            var list = new List<EffectBinding>();
            if (string.IsNullOrEmpty(effect))
                return list;
            foreach (string raw in effect.Split(';'))
            {
                string s = raw.Trim();
                if (s.Length == 0 || s.Equals("None", StringComparison.OrdinalIgnoreCase))
                    continue;
                var parts = s.Split('=');
                string head = parts[0].Trim();
                int idx = head.IndexOf(':');
                var binding = new EffectBinding
                {
                    Column = idx < 0 ? head : head.Substring(0, idx).Trim(),
                    Sub = idx < 0 ? "" : head.Substring(idx + 1).Trim(),
                };
                if (parts.Length > 1)
                {
                    string rawValue = parts[1].Trim();
                    string[] pieces = rawValue.Split(',');
                    float parsed;
                    if (pieces.All(x => float.TryParse(x.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out parsed)))
                        binding.Values = pieces.Select(x => float.Parse(x.Trim(), CultureInfo.InvariantCulture)).ToArray();
                    else
                        binding.Text = rawValue;
                }
                list.Add(binding);
            }
            return list;
        }

        /// <summary>取 Effect 中第一个带数值的绑定的 Lv 值（用于展示/档位匹配）。</summary>
        public static float PrimaryValue(string effect, int lv)
        {
            foreach (var b in ParseEffect(effect))
                if (b.Values.Length > 0)
                    return b.ValueAt(lv);
            return 0f;
        }

        /// <summary>EffectKey 支持的列名（校验用）。</summary>
        public static readonly string[] EffectColumns =
        {
            "DamageStrength", "HealStrength", "CD", "MpCost", "Range", "Area", "TargetCount",
            "Rate", "BuffTime", "TargetType", "TriggerCondition", "Strength2", "Buff",
            "SummonCount", "SummonTime", "SummonHitInterval", "SummonSpeed", "SummonTag",
        };

        /// <summary>运行时 ApplySkillBase 覆盖的列（与 ConfigManager 保持一致：只含模板/组件能改写的效果列）。</summary>
        public static readonly int[] AppliedColumns =
        {
            Col.Rate, Col.CD, Col.MpCost, Col.TriggerCondition, Col.Range, Col.Area,
            Col.TargetType, Col.TargetCount, Col.DamageStrength, Col.HealStrength,
            Col.Strength2, Col.StrengthBuff1, Col.BuffId, Col.BuffTime,
            Col.SummonTag, Col.SummonCount, Col.SummonTime, Col.SummonHitInterval, Col.SummonSpeed,
        };

        /// <summary>展开写入器：把模板/组件的 k-v 绑定落到 41 列与各字符串槽位。</summary>
        private sealed class BaseWriter
        {
            public readonly string[] B;
            public readonly List<float> BuffTimes = new List<float>();
            public readonly List<string> Warnings = new List<string>();

            private readonly List<string> buffIds;
            private readonly List<string> buffSegs;
            private readonly List<string> tags;
            private readonly List<string> triggers;
            private readonly List<string> s2;
            private readonly HashSet<string> handBuffKeys;
            private readonly HashSet<string> handS2Keys;
            private bool tagsChanged;
            private bool triggersChanged;
            private bool s2Changed;

            public BaseWriter(string[] source)
            {
                B = (string[])source.Clone();
                buffIds = SplitList(B[Col.BuffId], ',');
                buffSegs = SplitList(B[Col.StrengthBuff1], ',');
                tags = SplitList(B[Col.TargetType], ',');
                triggers = SplitList(B[Col.TriggerCondition], ';');
                s2 = SplitList(B[Col.Strength2], ',');
                handBuffKeys = new HashSet<string>(KeyOf(buffSegs));
                handS2Keys = new HashSet<string>(KeyOf(s2));
            }

            public void Apply(List<EffectBinding> bindings, int lv, string source)
            {
                foreach (var b in bindings)
                {
                    float v = b.ValueAt(lv);
                    switch (b.Column)
                    {
                        case "DamageStrength": B[Col.DamageStrength] = CfgUtil.NumF(v); break;
                        case "HealStrength": B[Col.HealStrength] = CfgUtil.NumF(v); break;
                        case "CD": B[Col.CD] = CfgUtil.NumF(v); break;
                        case "MpCost": B[Col.MpCost] = CfgUtil.NumI((int)Math.Round(v)); break;
                        case "Range": B[Col.Range] = CfgUtil.NumF(v); break;
                        case "Area": B[Col.Area] = CfgUtil.NumF(v); break;
                        case "TargetCount": B[Col.TargetCount] = CfgUtil.NumI((int)Math.Round(v)); break;
                        case "Rate": B[Col.Rate] = CfgUtil.NumF(v); break;
                        case "BuffTime": BuffTimes.Add(v); break;
                        case "SummonCount": B[Col.SummonCount] = CfgUtil.NumI((int)Math.Round(v)); break;
                        case "SummonTime": B[Col.SummonTime] = CfgUtil.NumF(v); break;
                        case "SummonHitInterval": B[Col.SummonHitInterval] = CfgUtil.NumF(v); break;
                        case "SummonSpeed": B[Col.SummonSpeed] = CfgUtil.NumF(v); break;
                        case "SummonTag":
                            if (CfgUtil.Unquote(B[Col.SummonTag]).Length == 0)
                                B[Col.SummonTag] = CfgUtil.Quote(b.Sub);
                            break;
                        case "TargetType":
                            if (AddUnique(tags, b.Sub))
                                tagsChanged = true;
                            break;
                        case "TriggerCondition":
                            if (AddUnique(triggers, b.Sub + NumText(v)))
                                triggersChanged = true;
                            break;
                        case "Strength2":
                            if (b.Text != null)
                                ApplyStrength2Text(b.Sub, b.Text);
                            else
                                ApplyStrength2(b.Sub, v);
                            break;
                        case "Buff": ApplyBuff(b.Sub, v); break;
                        default:
                            Warnings.Add(source + "：未知 Effect 列名「" + b.Column + "」，已忽略");
                            break;
                    }
                }
            }

            private void ApplyStrength2(string key, float v)
            {
                // 手配优先：同名 key 已手配在任一槽位（Strength2 / StrengthBuff1）时都不覆盖
                if (string.IsNullOrEmpty(key) || handS2Keys.Contains(key) || handBuffKeys.Contains(key))
                    return;
                if (s2.Any(x => x.StartsWith(key + ":", StringComparison.Ordinal)))
                    return;
                s2.Add(key + ":" + NumText(v));
                s2Changed = true;
            }

            private void ApplyStrength2Text(string key, string text)
            {
                if (string.IsNullOrEmpty(key) || handS2Keys.Contains(key) || handBuffKeys.Contains(key))
                    return;
                if (s2.Any(x => x.StartsWith(key + ":", StringComparison.Ordinal)))
                    return;
                s2.Add(key + ":" + text);
                s2Changed = true;
            }

            private void ApplyBuff(string sub, float v)
            {
                string[] parts = sub.Split(':');
                string token = parts[0].Trim();
                if (token.Length == 0)
                    return;
                if (!buffIds.Contains(token))
                    buffIds.Add(token);
                if (parts.Length < 2 || parts[1].Trim().Length == 0)
                    return;
                string key = parts[1].Trim();
                if (handBuffKeys.Contains(key) || handS2Keys.Contains(key)
                    || buffSegs.Any(x => x.StartsWith(key + ":", StringComparison.Ordinal)))
                    return;
                buffSegs.Add(key + ":" + NumText(v));
            }

            public void Flush()
            {
                B[Col.BuffId] = CfgUtil.Quote(string.Join(",", buffIds));
                B[Col.StrengthBuff1] = CfgUtil.Quote(string.Join(",", buffSegs));
                if (tagsChanged)
                    B[Col.TargetType] = CfgUtil.Quote(string.Join(",", tags));
                if (triggersChanged)
                    B[Col.TriggerCondition] = CfgUtil.Quote(string.Join(";", triggers));
                if (s2Changed)
                    B[Col.Strength2] = CfgUtil.Quote(string.Join(",", s2));
                if (BuffTimes.Count > 0)
                {
                    float max = BuffTimes.Max();
                    if (BuffTimes.Distinct().Count() > 1)
                        Warnings.Add("多个来源提供了不同的 Buff 时长（" + string.Join("/", BuffTimes.Select(x => x.ToString("0.##"))) + "），取最大值 " + max.ToString("0.##"));
                    B[Col.BuffTime] = CfgUtil.NumF(max);
                }
            }

            private static List<string> SplitList(string raw, char sep)
            {
                return CfgUtil.Unquote(raw)
                    .Split(new[] { sep }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => x.Trim()).Where(x => x.Length > 0).ToList();
            }

            private static IEnumerable<string> KeyOf(IEnumerable<string> segs)
            {
                return segs.Where(x => x.Contains(":")).Select(x => x.Split(':')[0].Trim());
            }

            private static bool AddUnique(List<string> list, string value)
            {
                if (string.IsNullOrEmpty(value) || list.Contains(value))
                    return false;
                list.Add(value);
                return true;
            }
        }

        /// <summary>数值文本：整数不带小数，其余最多三位。</summary>
        private static string NumText(float v)
        {
            if (Math.Abs(v - Math.Round(v)) < 0.000001)
                return ((long)Math.Round(v)).ToString(CultureInfo.InvariantCulture);
            return v.ToString("0.###", CultureInfo.InvariantCulture);
        }

        /// <summary>把 Effect 绑定描述成「列/子键=数值」，用于试算提示。</summary>
        public static string DescribeEffects(string effect, int lv)
        {
            var bindings = ParseEffect(effect);
            if (bindings.Count == 0)
                return "None（只计分，不写列）";
            return string.Join("；", bindings.Select(b =>
            {
                string target = string.IsNullOrEmpty(b.Sub) ? b.Column : b.Column + ":" + b.Sub;
                if (b.Text != null)
                    return target + "=" + b.Text;
                if (b.Values.Length == 0)
                    return target;
                return target + "=" + NumText(b.ValueAt(lv));
            }));
        }

        /// <summary>组件名 → 组件族；长名优先匹配（避免 加攻速 被当成 加攻）。</summary>
        public static string MatchFamily(string module)
        {
            string best = null;
            foreach (string f in FamilyKey.Keys)
                if (module.StartsWith(f, StringComparison.Ordinal) && (best == null || f.Length > best.Length))
                    best = f;
            return best;
        }

        public static string TokenOf(string family)
        {
            foreach (var kv in TokenFamily)
                if (kv.Value == family)
                    return kv.Key;
            return null;
        }

        /// <summary>对比现有技能行与模板展开结果。</summary>
        public static List<CompareItem> Compare(DataStore store, GenerateResult gen)
        {
            var list = new List<CompareItem>();
            foreach (var skill in store.LevelOneSkills)
            {
                if (string.IsNullOrEmpty(skill.TemplateName))
                    continue;
                string[] b;
                if (!gen.Rows.TryGetValue(skill.Id, out b))
                    continue;
                var item = new CompareItem { Skill = skill, Template = skill.TemplateName, Base = b };
                var a = skill.Args;
                foreach (int c in AppliedColumns)
                {
                    if (a[c] == b[c])
                        continue;
                    item.Diffs.Add(ColName(c) + " " + Display(a[c]) + "→" + Display(b[c]));
                    if (c == Col.DamageStrength || c == Col.HealStrength) item.MainValueDiff = true;
                    if (c == Col.CD || c == Col.MpCost) item.RhythmDiff = true;
                    if (c == Col.BuffId || c == Col.StrengthBuff1 || c == Col.BuffTime) item.BuffDiff = true;
                }
                list.Add(item);
            }
            return list;
        }

        public static string ColName(int col)
        {
            switch (col)
            {
                case Col.CD: return "CD";
                case Col.MpCost: return "MpCost";
                case Col.Rate: return "Rate";
                case Col.Range: return "Range";
                case Col.Area: return "Area";
                case Col.TargetCount: return "目标数";
                case Col.DamageStrength: return "伤害";
                case Col.HealStrength: return "治疗";
                case Col.TriggerCondition: return "触发条件";
                case Col.BuffId: return "BuffId";
                case Col.StrengthBuff1: return "Buff值";
                case Col.BuffTime: return "Buff时长";
                case Col.Strength2: return "数值槽";
                case Col.TargetType: return "选取点";
                case Col.AuroAttrs: return "光环效果";
                case Col.StartMpRate: return "开局MP";
                case Col.DamageType: return "伤害类型";
                case Col.HurtTag: return "伤害标签";
                case Col.SummonTag: return "召唤标签";
                case Col.SummonCount: return "召唤数";
                case Col.SummonTime: return "召唤持续";
                case Col.SummonHitInterval: return "召唤间隔";
                case Col.SummonSpeed: return "召唤速度";
                case Col.ItemId: return "道具Id";
                case Col.ScriptName: return "脚本";
                case Col.Action: return "动作";
                case Col.SelfEffect: return "自身特效";
                case Col.HitEffect: return "命中特效";
                case Col.AreaEffect: return "范围特效";
                case Col.Icon: return "图标";
                case Col.LinkSelf: return "连接加成";
                case Col.LinkTeam: return "队伍加成";
                default: return "列" + col;
            }
        }

        private static string Display(string raw)
        {
            return CfgUtil.Unquote(raw);
        }

        /// <summary>技能价格 = 模板标准分 + 组件分（同族大/中/小只取一条，规则与试算一致）。</summary>
        public static double CalcPrice(DataStore store, SkillRow skill)
        {
            return CalcPrice(store, skill.TemplateName, skill.Modules);
        }

        public static double CalcPrice(DataStore store, string templateName, IEnumerable<string> modules)
        {
            TemplateRow tpl;
            if (string.IsNullOrEmpty(templateName) || !store.TemplateByName.TryGetValue(templateName, out tpl))
                return 0;
            double total = tpl.Score;
            var families = new Dictionary<string, ComponentRow>();
            if (modules != null)
            {
                foreach (string name in modules)
                {
                    ComponentRow comp;
                    if (!store.ComponentByName.TryGetValue(name, out comp))
                        continue;
                    string family = TierFamily(store, comp.Name);
                    if (family != null)
                    {
                        ComponentRow prev;
                        if (families.TryGetValue(family, out prev))
                        {
                            if (prev.Score >= comp.Score)
                                continue;
                            total -= prev.Score;
                        }
                        families[family] = comp;
                    }
                    total += comp.Score;
                }
            }
            return total;
        }

        /// <summary>单技能试算：模板分 + 勾选组件分，并给出提示。</summary>
        public static ScoreResult Evaluate(DataStore store, SkillRow skill, string templateName, IList<string> moduleNames)
        {
            var r = new ScoreResult();
            TemplateRow tpl;
            if (string.IsNullOrEmpty(templateName) || !store.TemplateByName.TryGetValue(templateName, out tpl))
            {
                r.Hints.Add("未选择模板（该技能按原有各列数值生效）。");
                return r;
            }

            r.TemplateScore = tpl.Score;
            int lv = Math.Max(1, skill.Lv);
            r.Hints.Add(string.Format("模板：{0}（品质 Q{1}，标准分 {2:0.##}）", tpl.Name, tpl.Quality, tpl.Score));
            r.Hints.Add(string.Format("主数值（Lv{0}）：{1}", lv, PrimaryText(tpl, lv, skill)));
            r.Hints.Add("模板写入：" + DescribeEffects(tpl.Effect, lv));
            r.Hints.Add(string.Format("节奏：MpCost {0}（模板 {1}×10）、CD {2:0.##}、Range {3:0.##}、Area {4:0.##}",
                tpl.MpCost * 10, tpl.MpCost, tpl.CD, tpl.Range, tpl.Area));
            if (tpl.Modules.Length > 0)
                r.Hints.Add("模板默认组件：" + string.Join("、", tpl.Modules) + "（仅参与估值，不注入效果；生成只用本技能勾选的组件）");

            var picked = new HashSet<string>();
            var families = new Dictionary<string, ComponentRow>();
            foreach (string name in moduleNames)
            {
                ComponentRow comp;
                if (!store.ComponentByName.TryGetValue(name, out comp))
                {
                    r.Hints.Add("⚠ 组件不存在：" + name);
                    continue;
                }
                if (!picked.Add(name))
                    continue;

                float value = PrimaryValue(comp.Effect, lv);
                r.Hints.Add(string.Format("组件 {0}：分值 {1:0.##}，Lv{2} 数值 {3:0.###}，写入 {4}",
                    comp.Name, comp.Score, lv, value, DescribeEffects(comp.Effect, lv)));

                if (comp.Templates.Length > 0 && !comp.Templates.Contains(tpl.Name))
                    r.Hints.Add("  ⚠ 该组件不在模板「" + tpl.Name + "」的推荐列表（推荐："
                        + string.Join("、", comp.Templates) + "）");
                if (comp.IsSpecial)
                    r.Hints.Add("  ⚠ 特殊机制组件，需要人工评审（不走通用展开）");

                string family = TierFamily(store, comp.Name);
                if (family != null)
                {
                    ComponentRow prev;
                    if (families.TryGetValue(family, out prev))
                    {
                        if (prev.Score >= comp.Score)
                        {
                            r.Hints.Add("  ⚠ 同族已选 " + prev.Name + "，本条不计分");
                            continue;
                        }
                        r.ComponentScore -= prev.Score;
                        r.Hints.Add("  ⚠ 同族已选 " + prev.Name + "，改用本条计分");
                    }
                    families[family] = comp;
                }
                r.ComponentScore += comp.Score;
            }

            r.Hints.Add(string.Format("合计：模板 {0:0.##} + 组件 {1:0.##} = {2:0.##}", r.TemplateScore, r.ComponentScore, r.Total));

            if (skill.DamageStrength != 0 || skill.HealStrength != 0 || !string.IsNullOrEmpty(skill.TemplateName))
            {
                string hand = skill.DamageStrength != 0 ? "伤害 " + skill.DamageStrength.ToString("0.###") : "治疗 " + skill.HealStrength.ToString("0.###");
                string tplv = PrimaryText(tpl, lv, skill);
                r.Hints.Add("对比现有配置：手配 " + hand + "，模板生成 " + tplv);
            }
            return r;
        }

        private static string PrimaryText(TemplateRow tpl, int lv, SkillRow skill)
        {
            float v = PrimaryValue(tpl.Effect, lv);
            if (IsDamageTemplate(tpl.Name) && tpl.Name == "跳跃弹射" && skill != null && skill.HealStrength > 0)
                return "治疗 " + v.ToString("0.###");
            if (IsDamageTemplate(tpl.Name))
                return "伤害 " + v.ToString("0.###");
            if (IsHealTemplate(tpl.Name))
                return "治疗 " + v.ToString("0.###");
            return "无直接伤害/治疗（形态模板）";
        }

        /// <summary>名字以 大/中/小 结尾且前缀为已存在的组件族时，返回该族。</summary>
        private static string TierFamily(DataStore store, string name)
        {
            if (name.Length < 2)
                return null;
            char last = name[name.Length - 1];
            if (last != '大' && last != '中' && last != '小')
                return null;
            string family = name.Substring(0, name.Length - 1);
            if (store.ComponentByName.ContainsKey(family + "大") || store.ComponentByName.ContainsKey(family + "中")
                || store.ComponentByName.ContainsKey(family + "小"))
                return family;
            return null;
        }

        /// <summary>配置自检。</summary>
        public static List<string> Validate(DataStore store)
        {
            var err = new List<string>();
            foreach (var s in store.Skills)
            {
                if (s.Args.Length != Col.Count)
                    err.Add("SkillConfig " + s.Id + " 列数=" + s.Args.Length);
                string t = s.TemplateName;
                if (string.IsNullOrEmpty(t))
                    continue;
                if (!store.TemplateByName.ContainsKey(t))
                    err.Add("SkillConfig " + s.Id + " 模板不存在：" + t);
                foreach (string m in s.Modules)
                    if (!store.ComponentByName.ContainsKey(m))
                        err.Add("SkillConfig " + s.Id + " 组件不存在：" + m);
                var tokens = s.BuffId.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                if (tokens.Distinct().Count() != tokens.Length)
                    err.Add("SkillConfig " + s.Id + " BuffId 重复：" + s.BuffId);
                if (!store.BaseRows.ContainsKey(s.Id))
                    err.Add("SkillConfig " + s.Id + " 缺少同 Id 的 SkillBaseConfig");
            }
            foreach (var t in store.Templates)
                foreach (string m in t.Modules)
                    if (!store.ComponentByName.ContainsKey(m))
                        err.Add("模板 " + t.Name + " 引用未知组件：" + m);
            foreach (var c in store.Components)
                foreach (string t in c.Templates)
                    if (!store.TemplateByName.ContainsKey(t))
                        err.Add("组件 " + c.Name + " 引用未知模板：" + t);
            foreach (var t in store.Templates)
                CheckEffectKey(err, "模板 " + t.Name, t.Effect, store);
            foreach (var c in store.Components)
                CheckEffectKey(err, "组件 " + c.Name, c.Effect, store);

            // 被模板化技能选用、但 Effect=None 的组件：评分计入却不会写任何列（易漏配）
            var usedModules = new HashSet<string>(StringComparer.Ordinal);
            foreach (var s in store.Skills)
            {
                if (string.IsNullOrEmpty(s.TemplateName))
                    continue;
                foreach (string m in s.Modules)
                    usedModules.Add(m);
            }
            foreach (var c in store.Components)
            {
                if (!usedModules.Contains(c.Name) || ParseEffect(c.Effect).Count > 0)
                    continue;
                err.Add("⚠ 组件 " + c.Name + " 被技能选用但 Effect=None（只计分，不写任何列）");
            }
            return err;
        }

        /// <summary>校验 Effect：列名合法、数值齐全、Buff 短名存在、提供 Buff 时是否有时长。</summary>
        private static void CheckEffectKey(List<string> err, string owner, string effect, DataStore store)
        {
            if (string.IsNullOrEmpty(effect))
            {
                err.Add(owner + " 未配置 Effect");
                return;
            }
            var bindings = ParseEffect(effect);
            bool hasBuff = false;
            bool hasBuffTimeBinding = false;
            foreach (var b in bindings)
            {
                if (!EffectColumns.Contains(b.Column))
                {
                    err.Add(owner + " Effect 列名非法：" + b.Column);
                    continue;
                }
                bool tokenOnly = b.Column == "TargetType" || b.Column == "SummonTag" || b.Text != null
                    || (b.Column == "Buff" && b.Sub.IndexOf(':') < 0);
                if (!tokenOnly && b.Values.Length == 0)
                    err.Add(owner + " Effect 绑定缺少数值：" + b.Column + (string.IsNullOrEmpty(b.Sub) ? "" : ":" + b.Sub));
                if (b.Column == "Buff")
                {
                    hasBuff = true;
                    string token = b.Sub.Split(':')[0].Trim();
                    if (token.Length == 0)
                        err.Add(owner + " Effect Buff 缺少短名");
                    else if (store.BuffTokens.Count > 0 && !store.BuffTokens.Contains(token))
                        err.Add(owner + " Effect Buff 短名不存在：" + token);
                }
                if (b.Column == "BuffTime")
                    hasBuffTimeBinding = true;
            }
            if (hasBuff && !hasBuffTimeBinding)
                err.Add("⚠ " + owner + " 提供 Buff 但 Effect 未含 BuffTime（待扫描补时长）");
        }
    }
}
