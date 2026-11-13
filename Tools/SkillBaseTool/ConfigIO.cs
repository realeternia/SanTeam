using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SkillBaseTool
{
    /// <summary>读取 / 回写 Assets/Resources/Scripts/Configs 下的 C# 配置表。</summary>
    public static class ConfigIO
    {
        public const string SkillFile = "SkillConfig_s.cs";
        public const string BaseFile = "SkillBaseConfig_s.cs";
        public const string TemplateFile = "SkillTemplateConfig_s.cs";
        public const string ComponentFile = "SkillTemplateComponentConfig_s.cs";
        public const string HeroFile = "HeroConfig_s.cs";
        public const string BuffFile = "BuffConfig_s.cs";

        /// <summary>从可执行文件目录向上查找 Unity 工程根目录。</summary>
        public static string FindRoot()
        {
            var dir = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
            for (int i = 0; i < 8 && dir != null; i++, dir = dir.Parent)
            {
                string cfg = Path.Combine(dir.FullName, "Assets", "Resources", "Scripts", "Configs");
                if (Directory.Exists(cfg))
                    return dir.FullName;
            }
            return null;
        }

        public static DataStore Load(string root)
        {
            var store = new DataStore { RootDir = root };
            string cfgDir = Path.Combine(root, "Assets", "Resources", "Scripts", "Configs");
            store.SkillPath = Path.Combine(cfgDir, SkillFile);
            store.BasePath = Path.Combine(cfgDir, BaseFile);
            store.TemplatePath = Path.Combine(cfgDir, TemplateFile);
            store.ComponentPath = Path.Combine(cfgDir, ComponentFile);
            store.HeroPath = Path.Combine(cfgDir, HeroFile);
            store.BuffPath = Path.Combine(cfgDir, BuffFile);

            LoadHeroQualities(store);
            LoadBuffTokens(store);
            LoadTemplates(store);
            LoadComponents(store);
            LoadSkills(store);
            foreach (var kv in ParseRows(ReadText(store.BasePath, out _), "SkillBaseConfig"))
                store.BaseRows[kv.Key] = kv.Value;
            return store;
        }

        private static void LoadHeroQualities(DataStore store)
        {
            string text = ReadText(store.HeroPath, out _);
            foreach (var kv in ParseRows(text, "HeroConfig"))
            {
                var a = kv.Value;
                if (a.Length < 7)
                    continue;
                string name = CfgUtil.Unquote(a[1]);
                var qs = new List<int>();
                for (int i = 4; i <= 6; i++)
                {
                    string s = CfgUtil.Unquote(a[i]);
                    if (s.Length > 0 && char.IsDigit(s[0]))
                        qs.Add(s[0] - '0');
                }
                if (qs.Count > 0)
                    store.HeroQuality[name] = qs.Min();
            }
        }

        private static void LoadTemplates(DataStore store)
        {
            foreach (var kv in ParseRows(ReadText(store.TemplatePath, out _), "SkillTemplateConfig"))
            {
                var a = kv.Value;
                if (a.Length < 11)
                    continue;
                var t = new TemplateRow
                {
                    Id = kv.Key,
                    Name = CfgUtil.Unquote(a[1]),
                    Des = CfgUtil.Unquote(a[2]),
                    Modules = CfgUtil.ParseNames(CfgUtil.Unquote(a[3])),
                    Effect = CfgUtil.Unquote(a[4]),
                    Quality = CfgUtil.ParseInt(a[5]),
                    Score = CfgUtil.ParseFloat(a[6]),
                    Range = CfgUtil.ParseFloat(a[7]),
                    Area = CfgUtil.ParseFloat(a[8]),
                    MpCost = CfgUtil.ParseInt(a[9]),
                    CD = CfgUtil.ParseFloat(a[10]),
                };
                store.Templates.Add(t);
                store.TemplateByName[t.Name] = t;
            }
        }

        private static void LoadComponents(DataStore store)
        {
            foreach (var kv in ParseRows(ReadText(store.ComponentPath, out _), "SkillTemplateComponentConfig"))
            {
                var a = kv.Value;
                if (a.Length < 10)
                    continue;
                var c = new ComponentRow
                {
                    Id = kv.Key,
                    Name = CfgUtil.Unquote(a[1]),
                    Category = CfgUtil.Unquote(a[2]),
                    Templates = CfgUtil.ParseStringArray(a[3]),
                    Effect = CfgUtil.Unquote(a[4]),
                    Quality = CfgUtil.ParseInt(a[5]),
                    Score = CfgUtil.ParseFloat(a[6]),
                    Des = CfgUtil.Unquote(a[7]),
                    PriceNote = CfgUtil.Unquote(a[8]),
                    IsSpecial = a[9].Trim() == "true",
                };
                store.Components.Add(c);
                store.ComponentByName[c.Name] = c;
            }
        }

        /// <summary>收集 BuffConfig 的短名（第 3 列），用于校验 EffectKey 的 Buff: 绑定。</summary>
        private static void LoadBuffTokens(DataStore store)
        {
            foreach (var kv in ParseRows(ReadText(store.BuffPath, out _), "BuffConfig"))
            {
                var a = kv.Value;
                if (a.Length >= 3)
                    store.BuffTokens.Add(CfgUtil.Unquote(a[2]));
            }
        }

        private static void LoadSkills(DataStore store)
        {
            foreach (var kv in ParseRows(ReadText(store.SkillPath, out _), "SkillConfig"))
            {
                if (kv.Value.Length != Col.Count)
                    continue;
                var row = new SkillRow(kv.Key, kv.Value) { Quality = store.GetQuality(CfgUtil.Unquote(kv.Value[Col.Sname])) };
                store.Skills.Add(row);
                store.SkillById[row.Id] = row;
            }
        }

        /// <summary>把指定 Id 的技能行写回 SkillConfig_s.cs（只替换这些行，保留其余格式）。</summary>
        public static int SaveSkillRows(DataStore store, IEnumerable<int> ids)
        {
            var changed = new HashSet<int>(ids);
            string text = ReadText(store.SkillPath, out bool bom);
            var rx = new Regex(@"([ \t]*)config\[(\d+)\]\s*=\s*new\s+SkillConfig\((.*?)\);", RegexOptions.Singleline);
            text = rx.Replace(text, m =>
            {
                int id = int.Parse(m.Groups[2].Value);
                if (!changed.Contains(id) || !store.SkillById.ContainsKey(id))
                    return m.Value;
                return m.Groups[1].Value + "config[" + id + "] = new SkillConfig(" + string.Join(", ", store.SkillById[id].Args) + ");";
            });
            WriteText(store.SkillPath, text, bom);
            return changed.Count;
        }

        /// <summary>整表重写 SkillBaseConfig_s.cs 的 Load 数据行。</summary>
        public static int WriteBaseRows(DataStore store, Dictionary<int, string[]> rows)
        {
            string text = ReadText(store.BasePath, out bool bom);
            string nl = text.Contains("\r\n") ? "\r\n" : "\n";
            int load = text.IndexOf("public static void Load()", StringComparison.Ordinal);
            if (load < 0)
                throw new InvalidOperationException("SkillBaseConfig_s.cs 未找到 Load() 方法");
            int clear = text.IndexOf("config.Clear();", load, StringComparison.Ordinal);
            if (clear < 0)
                throw new InvalidOperationException("SkillBaseConfig_s.cs Load() 未找到 config.Clear();");
            int rebuild = text.IndexOf("RebuildIndex();", clear, StringComparison.Ordinal);
            if (rebuild < 0)
                throw new InvalidOperationException("SkillBaseConfig_s.cs Load() 未找到 RebuildIndex();");

            int bodyStart = text.IndexOf('\n', clear) + 1;
            int bodyEnd = text.LastIndexOf('\n', rebuild) + 1;
            var sb = new StringBuilder();
            foreach (var kv in rows.OrderBy(x => x.Key))
                sb.Append("            config[").Append(kv.Key).Append("] = new SkillBaseConfig(")
                  .Append(string.Join(", ", kv.Value)).Append(");").Append(nl);
            string result = text.Substring(0, bodyStart) + sb + text.Substring(bodyEnd);
            WriteText(store.BasePath, result, bom);
            return rows.Count;
        }

        public static string ReadText(string path, out bool bom)
        {
            byte[] bytes = File.ReadAllBytes(path);
            bom = bytes.Length >= 3 && bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF;
            return new UTF8Encoding(false).GetString(bytes, bom ? 3 : 0, bytes.Length - (bom ? 3 : 0));
        }

        public static void WriteText(string path, string text, bool bom)
        {
            File.WriteAllText(path, text, new UTF8Encoding(bom));
        }

        private static IEnumerable<KeyValuePair<int, string[]>> ParseRows(string text, string typeName)
        {
            var rx = new Regex(@"config\[(\d+)\]\s*=\s*new\s+" + typeName + @"\((.*?)\);", RegexOptions.Singleline);
            foreach (Match m in rx.Matches(text))
                yield return new KeyValuePair<int, string[]>(int.Parse(m.Groups[1].Value), SplitArgs(m.Groups[2].Value));
        }

        /// <summary>按顶层逗号拆分构造参数（忽略括号/大括号内的逗号）。</summary>
        public static string[] SplitArgs(string s)
        {
            var args = new List<string>();
            var cur = new StringBuilder();
            int depth = 0;
            bool quote = false;
            foreach (char ch in s)
            {
                if (ch == '"')
                    quote = !quote;
                if (!quote)
                {
                    if (ch == '{' || ch == '[' || ch == '(') depth++;
                    else if (ch == '}' || ch == ']' || ch == ')') depth--;
                    else if (ch == ',' && depth == 0)
                    {
                        args.Add(cur.ToString().Trim());
                        cur.Clear();
                        continue;
                    }
                }
                cur.Append(ch);
            }
            if (cur.ToString().Trim().Length > 0)
                args.Add(cur.ToString().Trim());
            return args.ToArray();
        }
    }
}
