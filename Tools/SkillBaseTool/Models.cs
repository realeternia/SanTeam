using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace SkillBaseTool
{
    /// <summary>SkillConfig / SkillBaseConfig 的 41 列索引。</summary>
    public static class Col
    {
        public const int Id = 0;
        public const int Name = 1;
        public const int Sname = 2;
        public const int Version = 3;
        public const int Descript = 4;
        public const int Type = 5;
        public const int Template = 6;
        public const int Modules = 7;
        public const int Lv = 8;
        public const int Rate = 9;
        public const int CD = 10;
        public const int MpCost = 11;
        public const int StartMpRate = 12;
        public const int TriggerCondition = 13;
        public const int DamageType = 14;
        public const int HurtTag = 15;
        public const int Range = 16;
        public const int Area = 17;
        public const int TargetType = 18;
        public const int TargetCount = 19;
        public const int DamageStrength = 20;
        public const int HealStrength = 21;
        public const int Strength2 = 22;
        public const int StrengthBuff1 = 23;
        public const int BuffId = 24;
        public const int BuffTime = 25;
        public const int SummonTag = 26;
        public const int SummonCount = 27;
        public const int SummonTime = 28;
        public const int SummonHitInterval = 29;
        public const int SummonSpeed = 30;
        public const int ItemId = 31;
        public const int ScriptName = 32;
        public const int Action = 33;
        public const int SelfEffect = 34;
        public const int HitEffect = 35;
        public const int AreaEffect = 36;
        public const int Icon = 37;
        public const int LinkSelf = 38;
        public const int LinkTeam = 39;
        public const int AuroAttrs = 40;
        public const int Count = 41;
    }

    /// <summary>配置行字符串工具：解析/格式化 C# 源码里的构造参数。</summary>
    public static class CfgUtil
    {
        public static string Unquote(string s)
        {
            if (s == null)
                return "";
            s = s.Trim();
            if (s.Length >= 2 && s[0] == s[s.Length - 1] && (s[0] == '"' || s[0] == '\''))
                return s.Substring(1, s.Length - 2);
            return s;
        }

        public static string Quote(string s)
        {
            return "\"" + (s ?? "").Replace("\\", "\\\\").Replace("\"", "\\\"") + "\"";
        }

        public static int ParseInt(string s)
        {
            if (string.IsNullOrEmpty(s))
                return 0;
            s = s.Trim().TrimEnd('f', 'F');
            int v;
            return int.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out v) ? v : 0;
        }

        public static float ParseFloat(string s)
        {
            if (string.IsNullOrEmpty(s))
                return 0f;
            s = s.Trim().TrimEnd('f', 'F');
            float v;
            return float.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out v) ? v : 0f;
        }

        public static float[] ParseFloats(string s)
        {
            var list = new List<float>();
            foreach (Match m in Regex.Matches(s ?? "", @"-?\d+\.?\d*"))
                list.Add(ParseFloat(m.Value));
            return list.ToArray();
        }

        /// <summary>逗号分隔的名字列表（模板 Modules 字段）。</summary>
        public static string[] ParseNames(string s)
        {
            return (s ?? "").Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim()).Where(x => x.Length > 0).ToArray();
        }

        /// <summary>new string[]{"a","b"} → ["a","b"]。</summary>
        public static string[] ParseStringArray(string s)
        {
            if (string.IsNullOrEmpty(s) || !s.Contains("{"))
                return new string[0];
            var list = new List<string>();
            foreach (Match m in Regex.Matches(s, "\"((?:[^\"\\\\]|\\\\.)*)\""))
                list.Add(m.Groups[1].Value);
            return list.ToArray();
        }

        public static string BuildStringArray(IEnumerable<string> names)
        {
            var arr = names == null ? new string[0] : names.Where(x => !string.IsNullOrEmpty(x)).ToArray();
            if (arr.Length == 0)
                return "new string[]{}";
            return "new string[]{" + string.Join(",", arr.Select(Quote)) + "}";
        }

        /// <summary>浮点输出保持原有风格：整数补 f（80f），小数最多三位。</summary>
        public static string NumF(float v)
        {
            if (Math.Abs(v - Math.Round(v)) < 0.000001)
                return ((long)Math.Round(v)).ToString(CultureInfo.InvariantCulture) + "f";
            return v.ToString("0.###", CultureInfo.InvariantCulture) + "f";
        }

        public static string NumI(int v)
        {
            return v.ToString(CultureInfo.InvariantCulture);
        }
    }

    /// <summary>SkillConfig_s.cs 的一行（保留 41 列原始参数以便写回）。</summary>
    public sealed class SkillRow
    {
        public int Id;
        public string[] Args;
        /// <summary>英雄品质（Sname 对应 HeroConfig，非英雄技能为 0）。</summary>
        public int Quality;

        public SkillRow(int id, string[] args)
        {
            Id = id;
            Args = args;
        }

        public int Lv { get { return CfgUtil.ParseInt(Args[Col.Lv]); } }
        public string Name { get { return CfgUtil.Unquote(Args[Col.Name]); } }
        public string Sname { get { return CfgUtil.Unquote(Args[Col.Sname]); } }
        public string Type { get { return CfgUtil.Unquote(Args[Col.Type]); } }
        public string ScriptName { get { return CfgUtil.Unquote(Args[Col.ScriptName]); } }
        public string TargetType { get { return CfgUtil.Unquote(Args[Col.TargetType]); } }
        public string BuffId { get { return CfgUtil.Unquote(Args[Col.BuffId]); } }
        public string StrengthBuff1 { get { return CfgUtil.Unquote(Args[Col.StrengthBuff1]); } }
        public string Strength2 { get { return CfgUtil.Unquote(Args[Col.Strength2]); } }
        public float BuffTime { get { return CfgUtil.ParseFloat(Args[Col.BuffTime]); } }
        public float DamageStrength { get { return CfgUtil.ParseFloat(Args[Col.DamageStrength]); } }
        public float HealStrength { get { return CfgUtil.ParseFloat(Args[Col.HealStrength]); } }

        public string TemplateName
        {
            get { return CfgUtil.Unquote(Args[Col.Template]); }
            set { Args[Col.Template] = CfgUtil.Quote(value); }
        }

        public string[] Modules
        {
            get { return CfgUtil.ParseStringArray(Args[Col.Modules]); }
            set { Args[Col.Modules] = CfgUtil.BuildStringArray(value); }
        }

        public string Display
        {
            get
            {
                string q = Quality > 0 ? " · Q" + Quality : "";
                return Sname + " · " + Name + q;
            }
        }
    }

    /// <summary>SkillTemplateConfig_s.cs 一行。</summary>
    public sealed class TemplateRow
    {
        public int Id;
        public string Name;
        public string Des;
        public string[] Modules;
        /// <summary>效果 k-v（分号多绑定，列[:子键]=Lv1~5数值，None=不写列）。</summary>
        public string Effect;
        public int Quality;
        public float Score;
        public float Range;
        public float Area;
        public int MpCost;
        public float CD;
    }

    /// <summary>SkillTemplateComponentConfig_s.cs 一行。</summary>
    public sealed class ComponentRow
    {
        public int Id;
        public string Name;
        public string Category;
        public string[] Templates;
        /// <summary>效果 k-v（分号多绑定，列[:子键]=Lv1~5数值，None=不写列）。</summary>
        public string Effect;
        public int Quality;
        public float Score;
        public string Des;
        public string PriceNote;
        public bool IsSpecial;

        public override string ToString()
        {
            return Name;
        }
    }

    /// <summary>一次加载得到的全部数据。</summary>
    public sealed class DataStore
    {
        public string RootDir;
        public string SkillPath;
        public string BasePath;
        public string TemplatePath;
        public string ComponentPath;
        public string HeroPath;
        public string BuffPath;

        public List<SkillRow> Skills = new List<SkillRow>();
        public Dictionary<int, SkillRow> SkillById = new Dictionary<int, SkillRow>();
        /// <summary>英雄名 → 剧本中最低品质。</summary>
        public Dictionary<string, int> HeroQuality = new Dictionary<string, int>();

        public List<TemplateRow> Templates = new List<TemplateRow>();
        public Dictionary<string, TemplateRow> TemplateByName = new Dictionary<string, TemplateRow>();
        public List<ComponentRow> Components = new List<ComponentRow>();
        public Dictionary<string, ComponentRow> ComponentByName = new Dictionary<string, ComponentRow>();
        /// <summary>BuffConfig 里已存在的 Buff 短名（校验 Effect 用）。</summary>
        public HashSet<string> BuffTokens = new HashSet<string>();

        /// <summary>当前 SkillBaseConfig_s.cs 里已有的行（Id → 41 列参数）。</summary>
        public Dictionary<int, string[]> BaseRows = new Dictionary<int, string[]>();

        public int GetQuality(string sname)
        {
            int q;
            return !string.IsNullOrEmpty(sname) && HeroQuality.TryGetValue(sname, out q) ? q : 0;
        }

        public IEnumerable<SkillRow> LevelOneSkills
        {
            get { return Skills.Where(s => s.Lv == 1); }
        }

        public IEnumerable<SkillRow> Quality12HeroSkills
        {
            get { return LevelOneSkills.Where(s => s.Quality == 1 || s.Quality == 2); }
        }
    }
}
