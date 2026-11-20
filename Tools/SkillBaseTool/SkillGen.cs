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
        public bool SelectDiff;
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
            "单体伤害大", "单体伤害中", "单体伤害小",
            "群体伤害近距离大", "群体伤害近距离中", "群体伤害近距离小",
            "群体伤害自身中心大", "群体伤害自身中心中", "群体伤害自身中心小",
            "跳跃弹射-伤害大", "跳跃弹射-伤害中", "跳跃弹射-伤害小",
            "冲击波伤害大", "冲击波伤害中", "冲击波伤害小",
            "持续伤害大", "持续伤害中", "持续伤害小"
        };

        public static readonly string[] HealTemplates =
        {
            "单体治疗大", "单体治疗中", "单体治疗小",
            "范围治疗大", "范围治疗中", "范围治疗小",
            "跳跃弹射-治疗大", "跳跃弹射-治疗中", "跳跃弹射-治疗小",
            "持续治疗大", "持续治疗中", "持续治疗小"
        };

        /// <summary>Buff 短名 → 组件族。</summary>
        private static readonly Dictionary<string, string> TokenFamily = new Dictionary<string, string>
        {
            { "破", "减防" }, { "慑", "减攻" }, { "缓", "减速" }, { "疫", "减疗" }, { "伤", "增伤" },
            { "乱", "眩晕" }, { "停", "定身" }, { "惑", "混乱" }, { "硬", "减伤" }, { "暴", "暴击" },
            { "攻", "加攻" }, { "甲", "加护甲" }, { "术", "加法强" }, { "抗", "加魔抗" },
            { "快", "加攻速" }, { "速", "加移速" }, { "重", "自身增伤" }, { "盾", "护盾" },
            { "败", "持续伤害" }, { "吸", "吸血" }, { "闪", "闪避" }, { "血", "加生命回复" },
        };

        /// <summary>Buff 短名 → 无档位组件名（唯一形态，直接命中组件）。</summary>
        private static readonly Dictionary<string, string> TokenDirectModule = new Dictionary<string, string>
        {
            { "叛", "叛逃" }, { "链", "生命链接" }, { "锁", "连锁扩散" }, { "守", "受击叠防" }, { "毒", "中毒" },
            { "反", "伤害反射" }, { "焰", "焰衣" }, { "震", "威震" }, { "愈", "急救" }, { "法暴", "法术暴击" },
        };

        /// <summary>组件族 → StrengthBuff1 数值键（空键表示族内无统一数值）。</summary>
        private static readonly Dictionary<string, string> FamilyKey = new Dictionary<string, string>
            {
                { "减防", "armorDown" },
                { "减攻", "atkDown" },
                { "减速", "slow" },
                { "减疗", "healDown" },
                { "增伤", "damagedUp" },
                { "减伤", "shieldReduce" },
                { "暴击", "critRate" },
                { "加攻", "atk" },
                { "加护甲", "armor" },
                { "加法强", "ap" },
                { "加魔抗", "magicRes" },
                { "加攻速", "atkSpeed" },
                { "加移速", "moveSpeed" },
                { "自身增伤", "damageUp" },
                { "护盾", "shield" },
                { "持续伤害", "dot" },
                { "加生命回复", "hpRegen" },
                { "吸血", "suck" },
                { "闪避", "evade" },
                { "眩晕", "" },
                { "定身", "" },
                { "混乱", "" },
            };

        /// <summary>SelectOnly 硬过滤规则 → 组件名（按出现顺序去重）。</summary>
        private static readonly Dictionary<string, string> SelectOnlyModules = new Dictionary<string, string>
        {
            { "hero", "英雄限定" }, { "notSelf", "排除自身" }, { "notFullHp", "满血跳过" },
        };

        /// <summary>SelectFirst 优先级规则 → 组件名（按出现顺序去重）。</summary>
        private static readonly Dictionary<string, string> SelectFirstModules = new Dictionary<string, string>
        {
            { "lowestHp", "低血筛选" }, { "highestHp", "最高血量" }, { "highestAp", "最高法强" },
            { "rangedFirst", "远程优先" }, { "meleeFirst", "近战优先" }, { "farthest", "后排筛选" },
        };

        /// <summary>TargetType 仅保留脚本私有行为标记（目标筛选已迁到 SelectOnly/SelectFirst）。</summary>
        private static readonly Dictionary<string, string> TargetTypeModules = new Dictionary<string, string>
        {
            { "selfAlways", "施法者必含" }, { "noDedupe", "不按首段去重" },
        };

        private static readonly Dictionary<string, string> TagModules = new Dictionary<string, string>
        {
            { "antiShield", "破盾" }, { "armorIgnore", "破盾" }, { "dispel", "驱散" },
            { "doubleChance", "概率双击" }, { "selfHurt", "自损" }, { "mpBurn", "燃烧法力" },
            { "shieldBonus", "按护盾加成伤害" }, { "vsJob", "兵种克制" },
            { "missile", "弹道" }, { "highHp", "高血增伤" }, { "missingHp", "残血增伤" },
            { "randomAmp", "随机增伤" }, { "isolatedAmp", "孤立增伤" }, { "shieldedAmp", "有盾增伤" },
            { "fullHp", "满血增伤" }, { "castStack", "逐次施法增伤" },
            { "execute", "斩杀" },
        };

        /// <summary>permanent 模式下 Strength2 成长键 → 永久成长组件（可挂任意模板）。</summary>
        private static readonly Dictionary<string, string> PermanentGrowthModules = new Dictionary<string, string>
        {
            { "selfAtkSpeed", "永久成长攻速" },
        };

        public static bool IsDamageTemplate(string t)
        {
            return t != null && DamageTemplates.Contains(t);
        }

        /// <summary>组件 Templates 的三大类取值：伤害。</summary>
        public const string CategoryDamage = "Damage";
        /// <summary>组件 Templates 的三大类取值：治疗。</summary>
        public const string CategoryHeal = "Heal";
        /// <summary>组件 Templates 的三大类取值：增益（含减益/控制等一切走 Buff 的效果）。</summary>
        public const string CategoryBuff = "Buff";
        /// <summary>模板分类列的防御取值（受击触发的防御模板）。</summary>
        public const string TemplateTypeDefend = "防御";

        /// <summary>模板所属大类：由其绑定的脚本类名推断（Damage / Heal / Buff）。</summary>
        public static string TemplateCategory(TemplateRow tpl)
        {
            if (tpl == null || string.IsNullOrEmpty(tpl.Script))
                return null;
            if (tpl.Script.Contains(CategoryDamage))
                return CategoryDamage;
            if (tpl.Script.Contains(CategoryHeal))
                return CategoryHeal;
            if (tpl.Script.Contains(CategoryBuff))
                return CategoryBuff;
            return null;
        }

        public static bool IsHealTemplate(string t)
        {
            return t != null && HealTemplates.Contains(t);
        }

        /// <summary>禁止在工具里改模板/组件的技能：开局固定加属性、无触发时机（如「初始属性」模板）。</summary>
        public static bool IsLockedForEdit(DataStore store, SkillRow skill)
        {
            if (skill == null || string.IsNullOrEmpty(skill.TemplateName))
                return false;
            TemplateRow tpl;
            return store.TemplateByName.TryGetValue(skill.TemplateName, out tpl) && tpl.Script == "SkillComInitAttr";
        }

        /// <summary>按技能脚本/类型推断模板；返回 null 表示不走模板（特化技能）。</summary>
        public static string PickTemplate(SkillRow a)
        {
            string type = a.Type;
            string script = a.ScriptName;
            float dmg = a.DamageStrength;
            float heal = a.HealStrength;

            if (script == "SkillAura")
                return "光环中";
            if (script == "SkillJumpChain")
                return (heal > 0 || a.TargetType.Contains("heal")) ? "跳跃弹射-治疗大" : "跳跃弹射-伤害大";
            if (script == "SkillSelfBuff")
                return "自身强化";
            if (script == "SkillTaunt")
                return "群体嘲讽";
            if (script == "SkillComDamageHitRegion")
                return "持续伤害大";
            if (heal > 0)
            {
                if (script == "SkillCastingAidAreaHeal")
                    return "持续治疗大";
                if (script != "SkillAreaHeal")
                    return null;
                return type == "单疗" ? "单体治疗大" : "范围治疗大";
            }
            if (script == "SkillAreaBuff")
                return type == "群增" ? "群体友方增益" : "单体友方增益";
            if (dmg > 0)
            {
                if (script.StartsWith("SkillAreaDamage", StringComparison.Ordinal))
                    return a.TargetType.Contains("self") ? "群体伤害自身中心大" : "群体伤害近距离大";
                if (script == "SkillShockWave")
                    return "冲击波伤害大";
                if (script == "SkillSingleDamage")
                    return DamageSize(dmg);
            }
            return null;
        }

        /// <summary>单体伤害按基值分档：≥90 大、≥60 中、其余小。</summary>
        private static string DamageSize(float dmg)
        {
            return dmg >= 90 ? "单体伤害大" : (dmg >= 60 ? "单体伤害中" : "单体伤害小");
        }

        /// <summary>模板 → 标准 Type，供编辑弹窗切换模板时联动；脚本取模板表 Script 列。</summary>
        private static readonly Dictionary<string, string> TemplateTypes =
            new Dictionary<string, string>
            {
                { "单体伤害大", "单伤" },
                { "单体伤害中", "单伤" },
                { "单体伤害小", "单伤" },
                { "群体伤害近距离大", "群伤" },
                { "群体伤害近距离中", "群伤" },
                { "群体伤害近距离小", "群伤" },
                { "群体伤害自身中心大", "群伤" },
                { "群体伤害自身中心中", "群伤" },
                { "群体伤害自身中心小", "群伤" },
                { "跳跃弹射-伤害大", "群伤" },
                { "跳跃弹射-伤害中", "群伤" },
                { "跳跃弹射-伤害小", "群伤" },
                { "冲击波伤害大", "群伤" },
                { "冲击波伤害中", "群伤" },
                { "冲击波伤害小", "群伤" },
                { "持续伤害大", "群伤" },
                { "持续伤害中", "群伤" },
                { "持续伤害小", "群伤" },
                { "单体治疗大", "单疗" },
                { "单体治疗中", "单疗" },
                { "单体治疗小", "单疗" },
                { "范围治疗大", "群疗" },
                { "范围治疗中", "群疗" },
                { "范围治疗小", "群疗" },
                { "跳跃弹射-治疗大", "群疗" },
                { "跳跃弹射-治疗中", "群疗" },
                { "跳跃弹射-治疗小", "群疗" },
                { "持续治疗大", "群疗" },
                { "持续治疗中", "群疗" },
                { "持续治疗小", "群疗" },
                { "自身强化", "自增" },
                { "光环中", "光环" },
                { "单体友方增益", "单增" },
                { "群体友方增益", "群增" },
                { "群体友方增益-目标小", "群增" },
                { "单体减益", "单控" },
                { "群体减益", "群控" },
                { "群体嘲讽", "群控" },
            };

        /// <summary>取模板对应的 Type / 脚本（脚本来自 SkillTemplateConfig.Script）；单/群控制技能沿用原「单控 / 群控」Type，避免切模板后丢控制标记。</summary>
        public static bool TryGetTemplateBinding(DataStore store, string template, string currentType, out string type, out string script)
        {
            type = null;
            script = null;
            if (store == null || string.IsNullOrEmpty(template))
                return false;
            TemplateRow tpl;
            if (!store.TemplateByName.TryGetValue(template, out tpl) || string.IsNullOrEmpty(tpl.Script))
                return false;
            if (!TemplateTypes.TryGetValue(template, out type))
                return false;
            script = tpl.Script;
            if (type == "单伤" && currentType == "单控")
                type = "单控";
            else if (type == "群伤" && currentType == "群控")
                type = "群控";
            return true;
        }

        /// <summary>从手配 BuffId / TargetType / 标签反推组件（按出现顺序去重）。</summary>
        public static List<string> ExtraModules(DataStore store, SkillRow a)
        {
            var result = new List<string>();
            string sb = a.StrengthBuff1;
            foreach (string tok in a.BuffId.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (tok == "伤" && a.ScriptName == "SkillSelfBuff")
                {
                    if (!result.Contains("自身易伤"))
                        result.Add("自身易伤");
                    continue;
                }
                string direct;
                if (TokenDirectModule.TryGetValue(tok, out direct))
                {
                    if (!result.Contains(direct))
                        result.Add(direct);
                    continue;
                }
                string family;
                if (!TokenFamily.TryGetValue(tok, out family))
                    continue;
                float? hand = null;
                string numKey;
                if (FamilyKey.TryGetValue(family, out numKey) && !string.IsNullOrEmpty(numKey))
                {
                    var m = Regex.Match(sb, Regex.Escape(numKey) + @":(-?\d+\.?\d*)");
                    if (!m.Success)
                        m = Regex.Match(a.Strength2, Regex.Escape(numKey) + @":(-?\d+\.?\d*)");
                    if (m.Success)
                        hand = CfgUtil.ParseFloat(m.Groups[1].Value);
                }
                string tier = PickTier(store, family, hand);
                if (tier != null && !result.Contains(tier))
                    result.Add(tier);
            }

            foreach (var kv in SelectOnlyModules)
                if (SplitTokens(a.SelectOnly, ';').Contains(kv.Key) && !result.Contains(kv.Value))
                    result.Add(kv.Value);
            foreach (var kv in SelectFirstModules)
                if (SplitTokens(a.SelectFirst, ';').Contains(kv.Key) && !result.Contains(kv.Value))
                    result.Add(kv.Value);
            foreach (var kv in TargetTypeModules)
                if (a.TargetType.Contains(kv.Key) && !result.Contains(kv.Value))
                    result.Add(kv.Value);
            foreach (var kv in TagModules)
                if (a.TargetType.Contains(kv.Key) && !result.Contains(kv.Value))
                    result.Add(kv.Value);

            if (a.ScriptName == "SkillSelfBuff" && a.BuffId.Contains("反") && !result.Contains("伤害反射"))
                result.Add("伤害反射");
            if (a.ScriptName == "SkillComDamageHitRegion" && a.Strength2.Contains("moveDist") && !result.Contains("持续场移动"))
                result.Add("持续场移动");
            if ((a.Strength2.Contains("pushMin") || a.Strength2.Contains("pushMax")) && !result.Contains("位移"))
                result.Add("位移");
            if (a.TargetType.Contains("permanent"))
            {
                foreach (var kv in PermanentGrowthModules)
                {
                    if (Regex.IsMatch(a.Strength2, Regex.Escape(kv.Key) + @":") && !result.Contains(kv.Value))
                        result.Add(kv.Value);
                }
            }
            return result;
        }

        /// <summary>按分隔符拆分规则串（SelectOnly/SelectFirst 用 ';'）。</summary>
        private static List<string> SplitTokens(string raw, char sep)
        {
            if (string.IsNullOrEmpty(raw))
                return new List<string>();
            return raw.Split(new[] { sep }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim()).Where(x => x.Length > 0).ToList();
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

        /// <summary>为品质 1~4 英雄技能自动填 Template / Modules，返回被改动的行 Id。</summary>
        public static HashSet<int> AutoAssign(DataStore store, out int assignedGroups, out List<string> skipped)
        {
            var changed = new HashSet<int>();
            assignedGroups = 0;
            skipped = new List<string>();
            foreach (var group in store.Skills.GroupBy(s => s.Sname))
            {
                var lv1 = group.FirstOrDefault(s => s.Lv == 1) ?? group.First();
                if (lv1.Quality < 1 || lv1.Quality > 4)
                    continue;
                // 已是锁定技能（如「才干」的初始属性）保持原样，不参与自动分配
                if (IsLockedForEdit(store, lv1))
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
            var writer = new BaseWriter(skill.Args, store);
            int lv = Math.Max(1, skill.Lv);

            var tplBindings = ParseEffect(tpl.Effect);
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
            if (tpl.TargetCount > 0)
                writer.B[Col.TargetCount] = CfgUtil.NumI(tpl.TargetCount);
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
            /// <summary>差值符号：0=绝对写入（=），1=叠加（+=），-1=扣减（-=）。</summary>
            public int DeltaSign;

            public bool IsDelta { get { return DeltaSign != 0; } }

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
                int deltaSign = 0;
                if (head.EndsWith("+", StringComparison.Ordinal))
                {
                    deltaSign = 1;
                    head = head.Substring(0, head.Length - 1).Trim();
                }
                else if (head.EndsWith("-", StringComparison.Ordinal))
                {
                    deltaSign = -1;
                    head = head.Substring(0, head.Length - 1).Trim();
                }
                int idx = head.IndexOf(':');
                var binding = new EffectBinding
                {
                    Column = idx < 0 ? head : head.Substring(0, idx).Trim(),
                    Sub = idx < 0 ? "" : head.Substring(idx + 1).Trim(),
                    DeltaSign = deltaSign,
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
            "DamageStrength", "HealStrength", "CD", "MpCost", "StartMpRate", "MpLimit", "AuraMpRate", "Range", "Area", "TargetCount",
            "Rate", "BuffTime", "BuffTimeRate", "BuffEffectRate", "TargetType", "SelectOnly", "SelectFirst", "TriggerCondition", "Strength2", "Buff",
            "SummonCount", "SummonTime", "SummonHitInterval", "SummonSpeed", "SummonTag",
        };

        /// <summary>运行时 ApplySkillBase 覆盖的列（与 ConfigManager 保持一致：只含模板/组件能改写的效果列）。</summary>
        public static readonly int[] AppliedColumns =
        {
            Col.Rate, Col.CD, Col.MpCost, Col.StartMpRate, Col.MpLimit, Col.AuraMpRate, Col.TriggerCondition, Col.Range, Col.Area,
            Col.SelectOnly, Col.SelectFirst,
            Col.TargetType, Col.TargetCount, Col.DamageStrength, Col.HealStrength,
            Col.Strength2, Col.StrengthBuff1, Col.BuffId, Col.BuffTime,
            Col.SummonTag, Col.SummonCount, Col.SummonTime, Col.SummonHitInterval, Col.SummonSpeed,
        };

        /// <summary>展开写入器：把模板/组件的 k-v 绑定落到 44 列与各字符串槽位。</summary>
        private sealed class BaseWriter
        {
            public readonly string[] B;
            public readonly List<float> BuffTimes = new List<float>();
            public readonly List<string> Warnings = new List<string>();

            private readonly DataStore store;
            private readonly List<string> buffIds;
            private readonly List<string> buffSegs;
            private readonly List<string> tags;
            private readonly List<string> selectOnly;
            private readonly List<string> selectFirst;
            private readonly List<string> triggers;
            private readonly List<string> s2;
            private readonly HashSet<string> handBuffKeys;
            private readonly HashSet<string> handS2Keys;
            private bool tagsChanged;
            private bool selectOnlyChanged;
            private bool selectFirstChanged;
            private bool triggersChanged;
            private bool s2Changed;

            public BaseWriter(string[] source, DataStore dataStore)
            {
                B = (string[])source.Clone();
                store = dataStore;
                buffIds = SplitList(B[Col.BuffId], ',');
                buffSegs = SplitList(B[Col.StrengthBuff1], ',');
                tags = SplitList(B[Col.TargetType], ',');
                selectOnly = SplitList(B[Col.SelectOnly], ';');
                selectFirst = SplitList(B[Col.SelectFirst], ';');
                triggers = SplitList(B[Col.TriggerCondition], ';');
                s2 = SplitList(B[Col.Strength2], ',');
                handBuffKeys = new HashSet<string>(KeyOf(buffSegs));
                handS2Keys = new HashSet<string>(KeyOf(s2));
            }

            /// <summary>BuffAttrDynamic 声明“受品质/等级影响的 Buff 效果字段”，供 BuffEffectRate 乘算；BuffTimeRate 为时长乘性修饰项。</summary>
            private readonly List<string> buffAttrNames = new List<string>();
            private readonly List<float> buffTimeRates = new List<float>();
            private readonly List<float> buffEffectRates = new List<float>();

            public void Apply(List<EffectBinding> bindings, int lv, string source)
            {
                foreach (var b in bindings)
                {
                    float v = b.ValueAt(lv);
                    switch (b.Column)
                    {
                        case "DamageStrength": B[Col.DamageStrength] = CfgUtil.NumF(Num(b, Col.DamageStrength, v)); break;
                        case "HealStrength": B[Col.HealStrength] = CfgUtil.NumF(Num(b, Col.HealStrength, v)); break;
                        case "CD": B[Col.CD] = CfgUtil.NumF(Num(b, Col.CD, v)); break;
                        case "MpCost": B[Col.MpCost] = CfgUtil.NumI((int)Math.Round(Num(b, Col.MpCost, v, 10f))); break;
                        case "StartMpRate": B[Col.StartMpRate] = CfgUtil.NumF(Num(b, Col.StartMpRate, v)); break;
                        case "MpLimit": B[Col.MpLimit] = CfgUtil.NumI((int)Math.Round(v)); break;
                        case "AuraMpRate": B[Col.AuraMpRate] = CfgUtil.NumF(Num(b, Col.AuraMpRate, v)); break;
                        case "Range": B[Col.Range] = CfgUtil.NumF(Num(b, Col.Range, v)); break;
                        case "Area": B[Col.Area] = CfgUtil.NumF(Num(b, Col.Area, v)); break;
                        case "TargetCount": B[Col.TargetCount] = CfgUtil.NumI((int)Math.Round(Num(b, Col.TargetCount, v))); break;
                        case "Rate": B[Col.Rate] = CfgUtil.NumF(Num(b, Col.Rate, v)); break;
                        case "BuffTime": BuffTimes.Add(v); break;
                        case "BuffTimeRate": buffTimeRates.Add(v); break;
                        case "BuffEffectRate": buffEffectRates.Add(v); break;
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
                        case "SelectOnly":
                            if (AddUnique(selectOnly, b.Sub))
                                selectOnlyChanged = true;
                            break;
                        case "SelectFirst":
                            if (AddUnique(selectFirst, b.Sub))
                                selectFirstChanged = true;
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

            /// <summary>绝对值直接写入；差值（+= / -=）在当前列值上叠加，MpCost 按表内单位×10。</summary>
            private float Num(EffectBinding b, int col, float v, float scale = 1f)
            {
                if (!b.IsDelta)
                    return v;
                float cur = CfgUtil.ParseFloat(B[col]);
                return cur + v * b.DeltaSign * scale;
            }

            private void ApplyStrength2(string key, float v)
            {
                // 手配优先：同名 key 已手配在任一槽位（Strength2 / StrengthBuff1）时都不覆盖；
                // 非手配时后写覆盖先写（模板默认值可被组件改写，如 jumpDecay）
                if (string.IsNullOrEmpty(key) || handS2Keys.Contains(key) || handBuffKeys.Contains(key))
                    return;
                int idx = s2.FindIndex(x => x.StartsWith(key + ":", StringComparison.Ordinal));
                if (idx >= 0)
                    s2[idx] = key + ":" + NumText(v);
                else
                    s2.Add(key + ":" + NumText(v));
                s2Changed = true;
            }

            private void ApplyStrength2Text(string key, string text)
            {
                if (string.IsNullOrEmpty(key) || handS2Keys.Contains(key) || handBuffKeys.Contains(key))
                    return;
                int idx = s2.FindIndex(x => x.StartsWith(key + ":", StringComparison.Ordinal));
                if (idx >= 0)
                    s2[idx] = key + ":" + text;
                else
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
                // Buff 效果字段以 BuffConfig.BuffAttrDynamic 为准：组件只需给 Buff 短名与数值，不再手写属性键
                string[] attrNames;
                if (store != null && store.BuffAttrDynamics.TryGetValue(token, out attrNames))
                {
                    foreach (string attr in attrNames)
                        AddUnique(buffAttrNames, attr);
                }
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
                ApplyBuffEffectRate();
                B[Col.BuffId] = CfgUtil.Quote(string.Join(",", buffIds));
                B[Col.StrengthBuff1] = CfgUtil.Quote(string.Join(",", buffSegs));
                if (tagsChanged)
                    B[Col.TargetType] = CfgUtil.Quote(string.Join(",", tags));
                if (selectOnlyChanged)
                    B[Col.SelectOnly] = CfgUtil.Quote(string.Join(";", selectOnly));
                if (selectFirstChanged)
                    B[Col.SelectFirst] = CfgUtil.Quote(string.Join(";", selectFirst));
                if (triggersChanged)
                    B[Col.TriggerCondition] = CfgUtil.Quote(string.Join(";", triggers));
                if (s2Changed)
                    B[Col.Strength2] = CfgUtil.Quote(string.Join(",", s2));
                if (BuffTimes.Count > 0)
                {
                    float max = BuffTimes.Max();
                    if (BuffTimes.Distinct().Count() > 1)
                        Warnings.Add("多个来源提供了不同的 Buff 时长（" + string.Join("/", BuffTimes.Select(x => x.ToString("0.##"))) + "），取最大值 " + max.ToString("0.##"));
                    if (buffTimeRates.Count > 0)
                    {
                        float rate = buffTimeRates.Max();
                        if (buffTimeRates.Distinct().Count() > 1)
                            Warnings.Add("多个时长延长组件同时存在（" + string.Join("/", buffTimeRates.Select(x => x.ToString("0.##"))) + "），取最大值 " + rate.ToString("0.##"));
                        max *= rate;
                    }
                    B[Col.BuffTime] = CfgUtil.NumF(max);
                }
            }

            /// <summary>
            /// 把 BuffEffectRate（效果提升%）乘到各状态组件用 BuffAttrDynamic 声明的动态 Buff 效果字段上。
            /// 只作用于组件写入的数值，手配优先（手配同名 key 不参与乘算）。
            /// </summary>
            private void ApplyBuffEffectRate()
            {
                if (buffEffectRates.Count == 0)
                    return;
                if (buffAttrNames.Count == 0)
                {
                    Warnings.Add("配置了效果提升（BuffEffectRate），但该技能没有提供任何动态 Buff 属性（BuffConfig.BuffAttrDynamic 为空），已忽略");
                    return;
                }
                float rate = buffEffectRates.Max();
                if (buffEffectRates.Distinct().Count() > 1)
                    Warnings.Add("多个效果提升组件同时存在（" + string.Join("/", buffEffectRates.Select(x => x.ToString("0.##"))) + "），取最大值 " + rate.ToString("0.##"));
                foreach (string key in buffAttrNames)
                {
                    if (handBuffKeys.Contains(key) || handS2Keys.Contains(key))
                        continue;
                    int i = buffSegs.FindIndex(x => x.StartsWith(key + ":", StringComparison.Ordinal));
                    if (i >= 0)
                        buffSegs[i] = ScaleSegValue(buffSegs[i], key, rate);
                    i = s2.FindIndex(x => x.StartsWith(key + ":", StringComparison.Ordinal));
                    if (i >= 0)
                    {
                        s2[i] = ScaleSegValue(s2[i], key, rate);
                        s2Changed = true;
                    }
                }
            }

            /// <summary>把 "key:1,2,3" 中每个数值乘以 rate，保持多值结构。</summary>
            private static string ScaleSegValue(string seg, string key, float rate)
            {
                string raw = seg.Substring(key.Length + 1);
                string[] parts = raw.Split(',');
                for (int i = 0; i < parts.Length; i++)
                {
                    float x;
                    if (float.TryParse(parts[i].Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out x))
                        parts[i] = NumText(x * rate);
                }
                return key + ":" + string.Join(",", parts);
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
                string op = b.IsDelta ? (b.DeltaSign > 0 ? "+=" : "-=") : "=";
                return target + op + NumText(b.ValueAt(lv));
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
                    if (c == Col.SelectOnly || c == Col.SelectFirst) item.SelectDiff = true;
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
                case Col.TargetType: return "行为标记";
                case Col.SelectOnly: return "目标硬过滤";
                case Col.SelectFirst: return "目标优先级";
                case Col.StartMpRate: return "开局MP";
                case Col.MpLimit: return "MP上限层数";
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
                case Col.AuraMpRate: return "光环耗蓝";
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
                    double score = ComponentScore(tpl, comp);
                    string family = TierFamily(store, comp.Name);
                    if (family != null)
                    {
                        ComponentRow prev;
                        if (families.TryGetValue(family, out prev))
                        {
                            if (ComponentScore(tpl, prev) >= score)
                                continue;
                            total -= ComponentScore(tpl, prev);
                        }
                        families[family] = comp;
                    }
                    total += score;
                }
            }
            return total;
        }

        /// <summary>概率类组件：防御模板下不产生收益，估值一律记 0。</summary>
        private static readonly HashSet<string> ProbabilityComponents = new HashSet<string>
        {
            "大概率", "中概率", "小概率",     // 施放节奏（Rate）
            "暴击", "法术暴击", "暴击率加成", // 暴击概率
            "概率双击", "闪避", "威震",       // 概率触发/概率免伤
        };

        /// <summary>组件在指定模板下的有效分值：防御模板里的概率类组件记 0。</summary>
        public static double ComponentScore(TemplateRow tpl, ComponentRow comp)
        {
            if (comp == null)
                return 0;
            if (tpl != null && tpl.Type == TemplateTypeDefend && ProbabilityComponents.Contains(comp.Name))
                return 0;
            return comp.Score;
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
                double score = ComponentScore(tpl, comp);
                r.Hints.Add(string.Format("组件 {0}：分值 {1:0.##}，Lv{2} 数值 {3:0.###}，写入 {4}",
                    comp.Name, score, lv, value, DescribeEffects(comp.Effect, lv)));
                if (score != comp.Score)
                    r.Hints.Add("  ⚠ 防御模板：概率类组件不产生收益，记 0 分");

                if (comp.Templates.Length > 0)
                {
                    if (comp.Templates.Any(t => t == CategoryDamage || t == CategoryHeal || t == CategoryBuff))
                    {
                        string tplCat = TemplateCategory(tpl);
                        if (tplCat != null && !comp.Templates.Contains(tplCat))
                            r.Hints.Add("  ⚠ 该组件不适用于「" + tplCat + "」大类模板（适用："
                                + string.Join("、", comp.Templates) + "）");
                    }
                    else if (tpl != null && !comp.Templates.Contains(tpl.Name))
                    {
                        r.Hints.Add("  ⚠ 该组件只适用于指定形态模板（" + string.Join("、", comp.Templates) + "）");
                    }
                }
                if (comp.IsSpecial)
                    r.Hints.Add("  ⚠ 特殊机制组件，需要人工评审（不走通用展开）");

                string family = TierFamily(store, comp.Name);
                if (family != null)
                {
                    ComponentRow prev;
                    if (families.TryGetValue(family, out prev))
                    {
                        if (ComponentScore(tpl, prev) >= score)
                        {
                            r.Hints.Add("  ⚠ 同族已选 " + prev.Name + "，本条不计分");
                            continue;
                        }
                        r.ComponentScore -= ComponentScore(tpl, prev);
                        r.Hints.Add("  ⚠ 同族已选 " + prev.Name + "，改用本条计分");
                    }
                    families[family] = comp;
                }
                r.ComponentScore += score;
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
            var reportedAttr = new HashSet<string>(StringComparer.Ordinal);
            foreach (var s in store.Skills)
            {
                if (s.Args.Length != Col.Count)
                    err.Add("SkillConfig " + s.Id + " 列数=" + s.Args.Length);
                else
                    CheckBuffAttrNames(err, store, s, reportedAttr);
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
                    if (t != CategoryDamage && t != CategoryHeal && t != CategoryBuff
                        && !store.TemplateByName.ContainsKey(t))
                        err.Add("组件 " + c.Name + " 的 Templates 只能是 Damage/Heal/Buff 或模板名：" + t);
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

        /// <summary>
        /// 校验 SkillConfig 引用 Buff 时，BuffConfig.BuffAttrDynamic/BuffAttrConst 声明的数值槽键是否都已赋值。
        /// 模板化技能按运行时实际生效的 SkillBaseConfig 行校验，其余按 SkillConfig 行校验；
        /// 同一 Sname 只报一次，避免 5 行等级重复刷屏。
        /// </summary>
        private static void CheckBuffAttrNames(List<string> err, DataStore store, SkillRow skill, HashSet<string> reported)
        {
            string[] args = skill.Args;
            string template = CfgUtil.Unquote(args[Col.Template]);
            if (!string.IsNullOrEmpty(template))
            {
                string[] baseArgs;
                if (store.BaseRows.TryGetValue(skill.Id, out baseArgs) && baseArgs.Length == Col.Count)
                    args = baseArgs;
            }

            string buffId = CfgUtil.Unquote(args[Col.BuffId]);
            if (string.IsNullOrEmpty(buffId))
                return;
            string sb1 = CfgUtil.Unquote(args[Col.StrengthBuff1]);
            string s2 = CfgUtil.Unquote(args[Col.Strength2]);

            foreach (string raw in buffId.Split(new[] { ',', '，', ';', '；' }, StringSplitOptions.RemoveEmptyEntries))
            {
                string token = raw.Trim();
                string[] attrNames;
                if (token.Length == 0 || !store.BuffAttrNames.TryGetValue(token, out attrNames))
                    continue;
                foreach (string attr in attrNames)
                {
                    if (attr.Length == 0 || HasSlotKey(sb1, attr) || HasSlotKey(s2, attr))
                        continue;
                    string key = (string.IsNullOrEmpty(skill.Sname) ? "#" + skill.Id : skill.Sname) + "|" + token + "|" + attr;
                    if (!reported.Add(key))
                        continue;
                    err.Add("SkillConfig " + skill.Id + "（" + skill.Display + "）引用 Buff「" + token
                        + "」缺少 BuffAttrDynamic/BuffAttrConst 数值键：" + attr + "（StrengthBuff1/Strength2 均未赋值）");
                }
            }
        }

        /// <summary>槽位串 "k:v,k:v" 中是否存在指定键。</summary>
        private static bool HasSlotKey(string slots, string key)
        {
            if (string.IsNullOrEmpty(slots))
                return false;
            foreach (string seg in slots.Split(new[] { ',', '，' }, StringSplitOptions.RemoveEmptyEntries))
            {
                int idx = seg.IndexOf(':');
                if (idx > 0 && seg.Substring(0, idx).Trim() == key)
                    return true;
            }
            return false;
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
