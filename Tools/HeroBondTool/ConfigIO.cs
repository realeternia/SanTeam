using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace HeroBondTool
{
    /// <summary>读取/回写 Assets/Resources/Scripts/Configs 下的配置表（C# 源码，保持原有格式）。</summary>
    public static class ConfigIO
    {
        private const string HeroFile = "HeroConfig_s.cs";
        private const string FriendFile = "HeroFriendConfig_s.cs";
        private const string ForceFile = "ForceConfig_s.cs";
        private const string JobFile = "JobConfig_s.cs";
        private const string ScenarioFile = "ScenarioConfig_s.cs";

        /// <summary>从 exe 目录/当前目录逐级向上查找仓库根（含 Assets/Resources/Scripts/Configs）。</summary>
        public static string LocateRoot()
        {
            return SearchRoot(AppContext.BaseDirectory) ?? SearchRoot(Environment.CurrentDirectory);
        }

        private static string SearchRoot(string startPath)
        {
            if (string.IsNullOrEmpty(startPath))
                return null;
            DirectoryInfo dir;
            try
            {
                dir = new DirectoryInfo(startPath);
            }
            catch
            {
                return null;
            }
            for (int i = 0; i < 8 && dir != null; i++, dir = dir.Parent)
            {
                string configDir = Path.Combine(dir.FullName, "Assets", "Resources", "Scripts", "Configs");
                if (File.Exists(Path.Combine(configDir, HeroFile)))
                    return dir.FullName;
            }
            return null;
        }

        public static DataStore Load(string rootDir)
        {
            if (string.IsNullOrEmpty(rootDir))
                throw new InvalidOperationException("未找到仓库根目录（需包含 Assets/Resources/Scripts/Configs）");

            string configDir = Path.Combine(rootDir, "Assets", "Resources", "Scripts", "Configs");
            var store = new DataStore();
            store.RootDir = rootDir;
            store.HeroPath = Path.Combine(configDir, HeroFile);
            store.FriendPath = Path.Combine(configDir, FriendFile);
            store.ForcePath = Path.Combine(configDir, ForceFile);
            store.JobPath = Path.Combine(configDir, JobFile);
            store.ScenarioPath = Path.Combine(configDir, ScenarioFile);

            ParseHeroes(ReadText(store.HeroPath), store);
            ParseFriendGroups(ReadText(store.FriendPath), store);
            ParseForces(ReadText(store.ForcePath), store);
            ParseJobs(ReadText(store.JobPath), store);
            ParseScenarios(ReadText(store.ScenarioPath), store);
            return store;
        }

        private static string ReadText(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException("找不到配置文件：" + path, path);
            return File.ReadAllText(path, Encoding.UTF8);
        }

        // ---------- 解析 ----------

        private static void ParseHeroes(string text, DataStore store)
        {
            foreach (Match m in IterRows(text, "HeroConfig"))
            {
                List<string> args = SplitArgs(m.Groups[1].Value);
                if (args.Count < 7)
                    continue;
                var hero = new HeroInfo();
                hero.Id = ParseInt(args[0]);
                hero.Name = Unquote(args[1]);
                hero.Job = Unquote(args[3]);
                for (int i = 0; i < 3; i++)
                    hero.Scenarios[i] = i + 4 < args.Count ? Unquote(args[4 + i]) : "";
                store.Heroes.Add(hero);
                store.HeroById[hero.Id] = hero;
            }
        }

        private static void ParseFriendGroups(string text, DataStore store)
        {
            foreach (Match m in IterRows(text, "HeroFriendConfig"))
            {
                List<string> args = SplitArgs(m.Groups[1].Value);
                if (args.Count < 6)
                    continue;
                var group = new FriendGroup();
                group.Id = ParseInt(args[0]);
                group.Name = Unquote(args[1]);
                group.Level = ParseInt(args[2]);
                if (args.Count >= 7)
                {
                    // 新格式：Id, Name, Level, Type, Heros, SkillId, LineColor
                    group.Type = Unquote(args[3]);
                    group.Heros = ParseIntArray(args[4]);
                    group.SkillId = Unquote(args[5]);
                    group.LineColor = Unquote(args[6]);
                }
                else
                {
                    // 旧格式：Id, Name, Level, Heros, SkillId, LineColor
                    group.Heros = ParseIntArray(args[3]);
                    group.SkillId = Unquote(args[4]);
                    group.LineColor = Unquote(args[5]);
                }
                store.Groups.Add(group);
            }
        }

        private static void ParseForces(string text, DataStore store)
        {
            foreach (Match m in IterRows(text, "ForceConfig"))
            {
                List<string> args = SplitArgs(m.Groups[1].Value);
                if (args.Count < 4)
                    continue;
                var force = new ForceInfo();
                force.Id = ParseInt(args[0]);
                force.Name = Unquote(args[1]);
                force.Short = Unquote(args[2]);
                force.Color = Unquote(args[3]);
                store.ForcesByShort[force.Short] = force;
                store.ForcesByShort[force.Name] = force;
            }
        }

        private static void ParseJobs(string text, DataStore store)
        {
            foreach (Match m in IterRows(text, "JobConfig"))
            {
                List<string> args = SplitArgs(m.Groups[1].Value);
                if (args.Count < 3)
                    continue;
                var job = new JobInfo();
                job.Id = ParseInt(args[0]);
                job.Name = Unquote(args[1]);
                job.Short = Unquote(args[2]);
                store.JobsByShort[job.Short] = job;
            }
        }

        private static void ParseScenarios(string text, DataStore store)
        {
            foreach (Match m in IterRows(text, "ScenarioConfig"))
            {
                List<string> args = SplitArgs(m.Groups[1].Value);
                if (args.Count < 2)
                    continue;
                var scenario = new ScenarioInfo();
                scenario.Id = ParseInt(args[0]);
                scenario.Name = Unquote(args[1]);
                store.Scenarios.Add(scenario);
            }
            store.Scenarios.Sort((a, b) => a.Id.CompareTo(b.Id));

            // 势力/职业顺序（与 python 报告一致：按配置 Id 升序）
            var forces = new List<ForceInfo>(new HashSet<ForceInfo>(store.ForcesByShort.Values));
            forces.Sort((a, b) => a.Id.CompareTo(b.Id));
            for (int i = 0; i < forces.Count; i++)
                store.ForceOrder[forces[i].Short] = i;

            var jobs = new List<JobInfo>(store.JobsByShort.Values);
            jobs.Sort((a, b) => a.Id.CompareTo(b.Id));
            for (int i = 0; i < jobs.Count; i++)
                store.JobOrder[jobs[i].Short] = i;
        }

        private static IEnumerable<Match> IterRows(string text, string typeName)
        {
            var rx = new Regex(@"config\[[^\]]+\]\s*=\s*new\s+" + typeName + @"\((.*?)\)\s*;", RegexOptions.Singleline);
            foreach (Match m in rx.Matches(text))
            {
                int lineStart = text.LastIndexOf('\n', m.Index) + 1;
                string prefix = text.Substring(lineStart, m.Index - lineStart);
                if (prefix.Contains("//"))
                    continue;
                yield return m;
            }
        }

        /// <summary>按顶层逗号切分构造参数（忽略引号与括号内的逗号）。</summary>
        internal static List<string> SplitArgs(string argstr)
        {
            var args = new List<string>();
            var cur = new StringBuilder();
            bool inQuote = false;
            char quoteChar = '\0';
            bool escape = false;
            int depth = 0;

            foreach (char ch in argstr)
            {
                if (escape)
                {
                    cur.Append(ch);
                    escape = false;
                    continue;
                }
                if (ch == '\\')
                {
                    cur.Append(ch);
                    escape = true;
                    continue;
                }
                if (inQuote)
                {
                    cur.Append(ch);
                    if (ch == quoteChar)
                        inQuote = false;
                    continue;
                }
                if (ch == '"' || ch == '\'')
                {
                    inQuote = true;
                    quoteChar = ch;
                    cur.Append(ch);
                    continue;
                }
                if (ch == '(' || ch == '[' || ch == '{')
                    depth++;
                else if (ch == ')' || ch == ']' || ch == '}')
                    depth--;

                if (ch == ',' && depth == 0)
                {
                    args.Add(cur.ToString().Trim());
                    cur.Length = 0;
                    continue;
                }
                cur.Append(ch);
            }
            if (cur.Length > 0)
                args.Add(cur.ToString().Trim());
            return args;
        }

        internal static string Unquote(string s)
        {
            if (s == null)
                return "";
            s = s.Trim();
            if (s.Length >= 2 && s[0] == s[s.Length - 1] && (s[0] == '"' || s[0] == '\''))
                return s.Substring(1, s.Length - 2);
            return s;
        }

        private static int ParseInt(string s)
        {
            int v;
            return int.TryParse(s.Trim(), out v) ? v : 0;
        }

        private static List<int> ParseIntArray(string s)
        {
            var list = new List<int>();
            foreach (Match m in Regex.Matches(s, @"\d+"))
                list.Add(int.Parse(m.Value));
            return list;
        }

        // ---------- 写回 ----------

        /// <summary>把指定羁绊组（Type / Heros 等）写回 HeroFriendConfig_s.cs，保留其余文本与换行。</summary>
        public static int SaveGroups(string path, DataStore store, ICollection<int> groupIds)
        {
            var changed = new Dictionary<int, FriendGroup>();
            foreach (int id in groupIds)
            {
                FriendGroup g = store.Groups.Find(x => x.Id == id);
                if (g != null)
                    changed[id] = g;
            }
            if (changed.Count == 0)
                return 0;

            byte[] raw = File.ReadAllBytes(path);
            bool hasBom = raw.Length >= 3 && raw[0] == 0xEF && raw[1] == 0xBB && raw[2] == 0xBF;
            string text = Encoding.UTF8.GetString(raw, hasBom ? 3 : 0, raw.Length - (hasBom ? 3 : 0));

            var rx = new Regex(@"config\[[^\]]+\]\s*=\s*new\s+HeroFriendConfig\((.*?)\)\s*;", RegexOptions.Singleline);
            int written = 0;
            string result = rx.Replace(text, m =>
            {
                int lineStart = text.LastIndexOf('\n', m.Index) + 1;
                string indent = text.Substring(lineStart, m.Index - lineStart);
                if (indent.Contains("//"))
                    return m.Value;

                List<string> args = SplitArgs(m.Groups[1].Value);
                int id = ParseInt(args[0]);
                FriendGroup g;
                if (!changed.TryGetValue(id, out g))
                    return m.Value;

                written++;
                return string.Format(
                    "config[{0}] = new HeroFriendConfig({0}, \"{1}\", {2}, \"{3}\", new int[]{{{4}}}, \"{5}\", \"{6}\");",
                    g.Id, Escape(g.Name), g.Level, Escape(g.Type),
                    string.Join(",", g.Heros.ToArray()), Escape(g.SkillId), Escape(g.LineColor));
            });

            File.WriteAllText(path, result, new UTF8Encoding(hasBom));
            return written;
        }

        private static string Escape(string s)
        {
            if (s == null)
                return "";
            return s.Replace("\\", "\\\\").Replace("\"", "\\\"");
        }
    }
}
