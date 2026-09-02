using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace DesignCoder
{
    public class FieldDef
    {
        public string Name;
        public string Type;
        public string Comment;
        public string ChineseName;
        public int? Width;
        public string FieldRule;
        public bool IsIndex;
    }

    public class CellMeta
    {
        public int Row;
        public int Col;
        public int? ForeColor;
        public int? BackColor;

        public CellMeta() { }

        public CellMeta(int row, int col, int? foreColor, int? backColor)
        {
            Row = row;
            Col = col;
            ForeColor = foreColor;
            BackColor = backColor;
        }
    }

    public class ConfigData
    {
        public string ClassName;
        public string Namespace;
        public List<FieldDef> Fields = new List<FieldDef>();
        public List<Dictionary<string, string>> Rows = new List<Dictionary<string, string>>();
        public List<CellMeta> CellMetas = new List<CellMeta>();
        public bool HasFieldMeta;
        public string UsingSection;
        public string PreFieldCode;
        public string PreLoadCode;
        public string PreFieldDeclCode;
        public string PostConstructorCode;
        public string PostLoadCode;

        public static ConfigData Parse(string source)
        {
            var data = new ConfigData();

            var nsMatch = Regex.Match(source, @"namespace\s+(\w+)");
            if (nsMatch.Success)
                data.Namespace = nsMatch.Groups[1].Value;

            var classMatch = Regex.Match(source, @"public\s+class\s+(\w+)");
            if (classMatch.Success)
                data.ClassName = classMatch.Groups[1].Value;

            ParseFieldMetaInfo(source, data);
            ParseCellMetas(source, data);
            ParseFields(source, data);
            ParseLoadMethod(source, data);
            SplitCodeSections(source, data);
            SplitPreLoadCode(data);
            ParseIndexFields(source, data);

            return data;
        }

        private static void ParseFieldMetaInfo(string source, ConfigData data)
        {
            var metaPattern = @"private\s+static\s+Dictionary<string\s*,\s*FieldMetaInfo>\s+fieldMeta\s*=\s*new\s+Dictionary<string\s*,\s*FieldMetaInfo>\s*\(\s*\)\s*\{([\s\S]*?)\};";
            var match = Regex.Match(source, metaPattern);
            if (!match.Success) return;
            data.HasFieldMeta = true;

            string body = match.Groups[1].Value;
            var itemPattern = @"\{\s*""(\w+)""\s*,\s*new\s+FieldMetaInfo\s*\(\s*""([^""]*)""\s*,\s*""([^""]+)""(?:\s*,\s*(\d+))?(?:\s*,\s*""([^""]*)"")?(?:\s*,\s*(true|false))?\s*\)\s*\}";

            var itemMatches = Regex.Matches(body, itemPattern);
            foreach (Match m in itemMatches)
            {
                string fieldName = m.Groups[1].Value;
                string chineseName = m.Groups[2].Value;
                string fieldType = m.Groups[3].Value;
                int? width = m.Groups[4].Success ? (int?)int.Parse(m.Groups[4].Value) : null;
                string fieldRule = m.Groups[5].Success ? m.Groups[5].Value : null;
                bool isIndex = m.Groups[6].Success && m.Groups[6].Value == "true";

                var existingField = data.Fields.FirstOrDefault(f => f.Name == fieldName);
                if (existingField != null)
                {
                    existingField.ChineseName = chineseName;
                    existingField.Type = fieldType;
                    if (width.HasValue) existingField.Width = width;
                    if (fieldRule != null) existingField.FieldRule = fieldRule;
                    existingField.IsIndex = isIndex;
                }
                else
                {
                    var fd = new FieldDef();
                    fd.Name = fieldName;
                    fd.ChineseName = chineseName;
                    fd.Type = fieldType;
                    fd.Width = width;
                    fd.FieldRule = fieldRule;
                    fd.IsIndex = isIndex;
                    data.Fields.Add(fd);
                }
            }
        }

        private static void ParseCellMetas(string source, ConfigData data)
        {
            var cellMetaPattern = @"private\s+static\s+List<CellMeta>\s+cellMeta\s*=\s*new\s+List<CellMeta>\s*\(\s*\)\s*\{([\s\S]*?)\};";
            var match = Regex.Match(source, cellMetaPattern);
            if (!match.Success) return;

            string body = match.Groups[1].Value;
            var itemPattern = @"new\s+CellMeta\s*\(\s*(-?\d+)\s*,\s*(-?\d+)\s*,\s*(-?\d+|null)\s*,\s*(-?\d+|null)\s*\)";
            var itemMatches = Regex.Matches(body, itemPattern);

            foreach (Match m in itemMatches)
            {
                var cm = new CellMeta();
                cm.Row = int.Parse(m.Groups[1].Value);
                cm.Col = int.Parse(m.Groups[2].Value);
                cm.ForeColor = ParseNullableInt(m.Groups[3].Value);
                cm.BackColor = ParseNullableInt(m.Groups[4].Value);
                data.CellMetas.Add(cm);
            }
        }

        private static int? ParseNullableInt(string s)
        {
            if (s == "null") return null;
            return int.Parse(s);
        }

        private static void ParseFields(string source, ConfigData data)
        {
            string pattern = @"///\s*<summary>\s*\r?\n\s*///\s*(.*?)\s*\r?\n\s*///\s*</summary>\s*\r?\n\s*public\s+([\w<>]+(?:\[\])?)\s+(\w+)\s*;";
            var matches = Regex.Matches(source, pattern);
            foreach (Match m in matches)
            {
                string fieldName = m.Groups[3].Value.Trim();
                var existingField = data.Fields.FirstOrDefault(f => f.Name == fieldName);
                if (existingField != null)
                {
                    existingField.Comment = m.Groups[1].Value.Trim();
                    if (string.IsNullOrEmpty(existingField.Type))
                        existingField.Type = m.Groups[2].Value.Trim();
                }
                else
                {
                    // 已存在 fieldMeta 时字段集合以 fieldMeta 为准，
                    // 忽略文件中残留的字段声明，否则被删除的旧列（如 FriendCount）会重新混入 Fields，导致行参数数量校验失败
                    if (data.HasFieldMeta) continue;

                    var fd = new FieldDef();
                    fd.Comment = m.Groups[1].Value.Trim();
                    fd.Type = m.Groups[2].Value.Trim();
                    fd.Name = fieldName;
                    data.Fields.Add(fd);
                }
            }
        }

        private static void ParseLoadMethod(string source, ConfigData data)
        {
            int loadStart = source.IndexOf("public static void Load()");
            if (loadStart < 0) return;

            int braceStart = source.IndexOf('{', loadStart);
            if (braceStart < 0) return;

            int depth = 0;
            int loadEnd = -1;
            for (int i = braceStart; i < source.Length; i++)
            {
                if (source[i] == '{') depth++;
                else if (source[i] == '}')
                {
                    depth--;
                    if (depth == 0) { loadEnd = i; break; }
                }
            }
            if (loadEnd < 0) return;

            string loadBody = source.Substring(braceStart + 1, loadEnd - braceStart - 1);

            string entryPattern = @"config\s*\[\s*(\d+)\s*\]\s*=\s*new\s+" + Regex.Escape(data.ClassName) + @"\s*\(";
            var entryMatches = Regex.Matches(loadBody, entryPattern);

            foreach (Match em in entryMatches)
            {
                int argsStart = em.Index + em.Length;
                var args = ParseConstructorArgs(loadBody, argsStart);
                int id = int.Parse(em.Groups[1].Value);
                
                if (args.Count != data.Fields.Count)
                {
                    throw new Exception(string.Format("配置表 {0} 第 {1} 行参数数量不匹配：期望 {2} 个字段，实际解析到 {3} 个参数。请检查构造函数参数是否正确。", 
                        data.ClassName, id, data.Fields.Count, args.Count));
                }
                
                var row = new Dictionary<string, string>();
                for (int i = 0; i < data.Fields.Count && i < args.Count; i++)
                {
                    string displayVal = RawToDisplay(args[i], data.Fields[i].Type);
                    row[data.Fields[i].Name] = displayVal;
                }
                data.Rows.Add(row);
            }
        }

        private static List<string> ParseConstructorArgs(string source, int startIndex)
        {
            var args = new List<string>();
            int i = startIndex;
            int depth = 1;

            while (i < source.Length && depth > 0)
            {
                while (i < source.Length && char.IsWhiteSpace(source[i])) i++;
                if (i >= source.Length) break;

                if (source[i] == ')') { depth--; if (depth == 0) break; i++; continue; }
                if (source[i] == ',') { i++; continue; }

                string arg = ExtractOneArg(source, ref i);
                args.Add(arg);

                while (i < source.Length && char.IsWhiteSpace(source[i])) i++;
                if (i < source.Length && source[i] == ',') i++;
                else if (i < source.Length && source[i] == ')') { depth--; break; }
            }

            return args;
        }

        private static string ExtractOneArg(string source, ref int i)
        {
            while (i < source.Length && char.IsWhiteSpace(source[i])) i++;
            if (i >= source.Length) return "";

            if (source[i] == '"')
                return ExtractStringArg(source, ref i);

            if (i + 3 < source.Length && source.Substring(i, 4) == "new ")
                return ExtractNewArg(source, ref i);

            if (i + 3 < source.Length && source.Substring(i, 4) == "null")
            { i += 4; return "null"; }

            if (i + 3 < source.Length && source.Substring(i, 4) == "true")
            { i += 4; return "true"; }

            if (i + 4 < source.Length && source.Substring(i, 5) == "false")
            { i += 5; return "false"; }

            var sb = new StringBuilder();
            while (i < source.Length && source[i] != ',' && source[i] != ')')
            {
                sb.Append(source[i]);
                i++;
            }
            return sb.ToString().Trim();
        }

        private static string ExtractStringArg(string source, ref int i)
        {
            var sb = new StringBuilder();
            sb.Append(source[i]); i++;
            while (i < source.Length)
            {
                if (source[i] == '\\')
                {
                    sb.Append(source[i]); i++;
                    if (i < source.Length) { sb.Append(source[i]); i++; }
                    continue;
                }
                sb.Append(source[i]);
                if (source[i] == '"') { i++; break; }
                i++;
            }
            return sb.ToString();
        }

        private static string ExtractNewArg(string source, ref int i)
        {
            int d = 0;
            var sb = new StringBuilder();
            while (i < source.Length)
            {
                if (source[i] == '{' || source[i] == '(' || source[i] == '[') d++;
                else if (source[i] == '}' || source[i] == ')' || source[i] == ']') d--;

                sb.Append(source[i]);
                i++;

                if (d == 0)
                {
                    string trimmed = sb.ToString().TrimEnd();
                    if (trimmed.EndsWith("}") || trimmed.EndsWith(")") || trimmed.EndsWith("]"))
                    {
                        while (i < source.Length && (source[i] == ' ' || source[i] == '\t')) i++;
                        if (i >= source.Length || source[i] == ',' || source[i] == ')')
                            break;
                    }
                }
            }
            return sb.ToString().Trim();
        }

        private static string RawToDisplay(string raw, string type)
        {
            raw = raw.Trim();
            if (raw == "null") return "";

            if (type == "string")
            {
                if (raw.Length >= 2 && raw.StartsWith("\"") && raw.EndsWith("\""))
                    return raw.Substring(1, raw.Length - 2).Replace("\\\"", "\"");
                return raw;
            }

            if (type == "float")
            {
                string v = raw;
                if (v.EndsWith("f") || v.EndsWith("F")) v = v.Substring(0, v.Length - 1);
                return v;
            }

            if (type == "string[]")
                return ParseArrayToDisplay(raw, true, false);

            if (type == "int[]")
                return ParseArrayToDisplay(raw, false, false);

            if (type == "float[]")
                return ParseArrayToDisplay(raw, false, true);

            return raw;
        }

        private static string ParseArrayToDisplay(string raw, bool isString, bool isFloat)
        {
            var match = Regex.Match(raw, @"new\s+\w+\s*\[\s*\]\s*\{(.*)\}", RegexOptions.Singleline);
            if (!match.Success) return "";

            string inner = match.Groups[1].Value.Trim();
            if (string.IsNullOrWhiteSpace(inner)) return "";

            var items = new List<string>();
            int idx = 0;
            while (idx < inner.Length)
            {
                while (idx < inner.Length && char.IsWhiteSpace(inner[idx])) idx++;
                if (idx >= inner.Length) break;

                if (inner[idx] == '"')
                {
                    idx++;
                    var sb = new StringBuilder();
                    while (idx < inner.Length)
                    {
                        if (inner[idx] == '\\') { idx++; if (idx < inner.Length) { sb.Append(inner[idx]); idx++; } continue; }
                        if (inner[idx] == '"') { idx++; break; }
                        sb.Append(inner[idx]); idx++;
                    }
                    items.Add(sb.ToString());
                }
                else
                {
                    var sb = new StringBuilder();
                    while (idx < inner.Length && inner[idx] != ',')
                    {
                        sb.Append(inner[idx]); idx++;
                    }
                    string val = sb.ToString().Trim();
                    if (isFloat && !string.IsNullOrEmpty(val))
                    {
                        if (val.EndsWith("f") || val.EndsWith("F"))
                            val = val.Substring(0, val.Length - 1);
                    }
                    if (!string.IsNullOrEmpty(val)) items.Add(val);
                }

                while (idx < inner.Length && char.IsWhiteSpace(inner[idx])) idx++;
                if (idx < inner.Length && inner[idx] == ',') idx++;
            }

            return string.Join(",", items.ToArray());
        }

        private static void SplitCodeSections(string source, ConfigData data)
        {
            int loadStart = source.IndexOf("public static void Load()");
            if (loadStart < 0)
            {
                data.PreFieldCode = source;
                data.PreLoadCode = "";
                data.PostLoadCode = "";
                return;
            }

            int braceStart = source.IndexOf('{', loadStart);
            int depth = 0;
            int loadEnd = -1;
            for (int i = braceStart; i < source.Length; i++)
            {
                if (source[i] == '{') depth++;
                else if (source[i] == '}')
                {
                    depth--;
                    if (depth == 0) { loadEnd = i; break; }
                }
            }

            int fieldMetaInfoStart = source.IndexOf("public class FieldMetaInfo");
            int fieldMetaEnd = source.IndexOf("public static Dictionary<string, FieldMetaInfo> FieldMeta");
            
            if (fieldMetaInfoStart > 0 && fieldMetaEnd > 0)
            {
                // 只保留类头部（using/namespace/class 等）：
                // 剔除 FieldMetaInfo 之前残留的字段声明/构造器段，否则生成时会输出两份字段与构造器
                data.PreFieldCode = TrimPreFieldCode(source.Substring(0, fieldMetaInfoStart));

                int sectionEndPos;
                int cellMetasEnd = source.IndexOf("public static List<CellMeta> CellMetas");
                if (cellMetasEnd > fieldMetaEnd)
                {
                    int cellMetasEndBrace = source.IndexOf('}', cellMetasEnd);
                    if (cellMetasEndBrace > 0)
                    {
                        int nextLine = source.IndexOf('\n', cellMetasEndBrace);
                        sectionEndPos = nextLine > 0 ? nextLine + 1 : cellMetasEndBrace + 1;
                    }
                    else
                    {
                        sectionEndPos = cellMetasEnd;
                    }
                }
                else
                {
                    int fieldMetaEndBrace = source.IndexOf('}', fieldMetaEnd);
                    if (fieldMetaEndBrace > 0)
                    {
                        int nextLine = source.IndexOf('\n', fieldMetaEndBrace);
                        sectionEndPos = nextLine > 0 ? nextLine + 1 : fieldMetaEndBrace + 1;
                    }
                    else
                    {
                        sectionEndPos = fieldMetaEnd;
                    }
                }

                data.PreLoadCode = source.Substring(sectionEndPos, loadStart - sectionEndPos);
            }
            else
            {
                int fieldStart = FindFieldSectionStart(source, loadStart);
                if (fieldStart > 0)
                {
                    data.PreFieldCode = TrimPreFieldCode(source.Substring(0, fieldStart));
                    data.PreLoadCode = source.Substring(fieldStart, loadStart - fieldStart);
                }
                else
                {
                    data.PreFieldCode = "";
                    data.PreLoadCode = source.Substring(0, loadStart);
                }
            }
            data.PostLoadCode = source.Substring(loadEnd + 1);
        }

        private static int FindFieldSectionStart(string source, int loadStart)
        {
            int lastBrace = -1;
            int depth = 0;
            for (int i = 0; i < loadStart; i++)
            {
                if (source[i] == '{') depth++;
                else if (source[i] == '}')
                {
                    depth--;
                    lastBrace = i;
                }
            }
            return lastBrace + 1;
        }

        /// <summary>
        /// 截取 PreFieldCode 中第一个字段声明块之前的头部内容，
        /// 避免把源文件中位于 FieldMetaInfo 之前的历史字段声明/构造器原样保留（它们会由生成器统一重建，保留会造成重复定义）。
        /// </summary>
        private static string TrimPreFieldCode(string preField)
        {
            if (string.IsNullOrEmpty(preField)) return preField;

            var m = Regex.Match(preField, @"public\s+\w+(?:\[\])?\s+\w+\s*;");
            if (!m.Success) return preField;

            // 定位字段声明所在行号
            int lineIndex = 0;
            for (int i = 0; i < m.Index && i < preField.Length; i++)
            {
                if (preField[i] == '\n') lineIndex++;
            }

            // 连同字段声明上方的 XML 注释与空行一起回溯移除
            string[] lines = preField.Split('\n');
            while (lineIndex > 0)
            {
                string prev = lines[lineIndex - 1].Trim();
                if (prev.StartsWith("///") || prev.Length == 0) lineIndex--;
                else break;
            }

            var sb = new StringBuilder();
            for (int i = 0; i < lineIndex && i < lines.Length; i++)
            {
                sb.Append(lines[i]);
                if (i < lines.Length - 1) sb.Append('\n');
            }
            return sb.ToString().TrimEnd();
        }

        private static void SplitPreLoadCode(ConfigData data)
        {
            if (string.IsNullOrEmpty(data.PreLoadCode))
            {
                data.PreFieldDeclCode = "";
                data.PostConstructorCode = "";
                return;
            }

            var firstFieldMatch = Regex.Match(data.PreLoadCode, @"public\s+\w+(?:\[\])?\s+\w+\s*;");
            if (!firstFieldMatch.Success)
            {
                data.PreFieldDeclCode = data.PreLoadCode;
                data.PostConstructorCode = "";
                return;
            }

            data.PreFieldDeclCode = data.PreLoadCode.Substring(0, firstFieldMatch.Index);

            string ctorPattern = @"public\s+" + Regex.Escape(data.ClassName) + @"\s*\(";
            var ctorMatch = Regex.Match(data.PreLoadCode, ctorPattern);
            if (!ctorMatch.Success)
            {
                var lastFieldMatch = Regex.Match(data.PreLoadCode, @"public\s+\w+(?:\[\])?\s+\w+\s*;", RegexOptions.RightToLeft);
                int endPos = lastFieldMatch.Success ? lastFieldMatch.Index + lastFieldMatch.Length : 0;
                data.PostConstructorCode = data.PreLoadCode.Substring(endPos);
                return;
            }

            int braceStart = data.PreLoadCode.IndexOf('{', ctorMatch.Index);
            if (braceStart < 0)
            {
                data.PostConstructorCode = "";
                return;
            }

            int depth = 0;
            int ctorEnd = -1;
            for (int i = braceStart; i < data.PreLoadCode.Length; i++)
            {
                if (data.PreLoadCode[i] == '{') depth++;
                else if (data.PreLoadCode[i] == '}')
                {
                    depth--;
                    if (depth == 0) { ctorEnd = i; break; }
                }
            }

            if (ctorEnd < 0)
            {
                data.PostConstructorCode = "";
                return;
            }

            data.PostConstructorCode = data.PreLoadCode.Substring(ctorEnd + 1);
        }

        private static void ParseIndexFields(string source, ConfigData data)
        {
            var pattern = @"private\s+static\s+Dictionary<(\w+),\s*int>\s+idx(\w+)\s*=\s*new\s+Dictionary<\w+,\s*int>\s*\(\s*\)\s*;";
            var matches = Regex.Matches(source, pattern);
            foreach (Match m in matches)
            {
                string fieldName = m.Groups[2].Value;
                var existingField = data.Fields.FirstOrDefault(f => f.Name == fieldName);
                if (existingField != null)
                {
                    existingField.IsIndex = true;
                }
            }
        }

        public string GenerateSource()
        {
            var sb = new StringBuilder();

            string preField = (PreFieldCode ?? "").TrimEnd();
            sb.Append(preField);
            if (preField.Length > 0 && !preField.EndsWith("\n"))
                sb.AppendLine();

            sb.AppendLine("        public class FieldMetaInfo");
            sb.AppendLine("        {");
            sb.AppendLine("            public string fieldName;");
            sb.AppendLine("            public string fieldType;");
            sb.AppendLine("            public int fieldWidth;");
            sb.AppendLine("            public string fieldRule;");
            sb.AppendLine("            public bool fieldIndex;");
            sb.AppendLine("            public FieldMetaInfo(string name, string type, int width = 0, string rule = \"\", bool index = false)");
            sb.AppendLine("            {");
            sb.AppendLine("                fieldName = name;");
            sb.AppendLine("                fieldType = type;");
            sb.AppendLine("                fieldWidth = width;");
            sb.AppendLine("                fieldRule = rule;");
            sb.AppendLine("                fieldIndex = index;");
            sb.AppendLine("            }");
            sb.AppendLine("        }");
            sb.AppendLine();

            sb.AppendLine("        public class CellMeta");
            sb.AppendLine("        {");
            sb.AppendLine("            public int row;");
            sb.AppendLine("            public int col;");
            sb.AppendLine("            public int? foreColor;");
            sb.AppendLine("            public int? backColor;");
            sb.AppendLine("            public CellMeta(int row, int col, int? foreColor, int? backColor)");
            sb.AppendLine("            {");
            sb.AppendLine("                this.row = row;");
            sb.AppendLine("                this.col = col;");
            sb.AppendLine("                this.foreColor = foreColor;");
            sb.AppendLine("                this.backColor = backColor;");
            sb.AppendLine("            }");
            sb.AppendLine("        }");
            sb.AppendLine();

            sb.AppendLine("        private static Dictionary<string, FieldMetaInfo> fieldMeta = new Dictionary<string, FieldMetaInfo>()");
            sb.AppendLine("        {");
            foreach (var field in Fields)
            {
                string chineseName = field.ChineseName ?? field.Comment ?? "";
                int width = field.Width ?? 0;
                string rule = field.FieldRule ?? "";
                if (field.IsIndex)
                    sb.AppendLine(string.Format("            {{\"{0}\", new FieldMetaInfo(\"{1}\", \"{2}\", {3}, \"{4}\", true)}},", field.Name, chineseName, field.Type, width, rule));
                else if (!string.IsNullOrEmpty(rule))
                    sb.AppendLine(string.Format("            {{\"{0}\", new FieldMetaInfo(\"{1}\", \"{2}\", {3}, \"{4}\")}},", field.Name, chineseName, field.Type, width, rule));
                else
                    sb.AppendLine(string.Format("            {{\"{0}\", new FieldMetaInfo(\"{1}\", \"{2}\", {3})}},", field.Name, chineseName, field.Type, width));
            }
            sb.AppendLine("        };");
            sb.AppendLine();
            sb.AppendLine("        public static Dictionary<string, FieldMetaInfo> FieldMeta { get { return fieldMeta; } }");
            sb.AppendLine();

            if (CellMetas.Count > 0)
            {
                sb.AppendLine("        private static List<CellMeta> cellMeta = new List<CellMeta>()");
                sb.AppendLine("        {");
                foreach (var cm in CellMetas)
                {
                    string fore = cm.ForeColor.HasValue ? cm.ForeColor.Value.ToString() : "null";
                    string back = cm.BackColor.HasValue ? cm.BackColor.Value.ToString() : "null";
                    sb.AppendLine(string.Format("            new CellMeta({0}, {1}, {2}, {3}),", cm.Row, cm.Col, fore, back));
                }
                sb.AppendLine("        };");
            }
            else
            {
                sb.AppendLine("        private static List<CellMeta> cellMeta = new List<CellMeta>();");
            }
            sb.AppendLine("        public static List<CellMeta> CellMetas { get { return cellMeta; } }");
            sb.AppendLine();

            foreach (var field in Fields)
            {
                if (!string.IsNullOrEmpty(field.Comment))
                {
                    sb.AppendLine("        /// <summary>");
                    sb.AppendLine("        ///" + field.Comment);
                    sb.AppendLine("        /// </summary>");
                }
                sb.AppendLine("        public " + field.Type + " " + field.Name + ";");
            }

            sb.AppendLine();
            sb.AppendLine();
            sb.Append("        public " + ClassName + "(");
            for (int i = 0; i < Fields.Count; i++)
            {
                if (i > 0) sb.Append(", ");
                sb.Append(Fields[i].Type + " " + Fields[i].Name);
            }
            sb.AppendLine(")");
            sb.AppendLine("        {");
            foreach (var field in Fields)
            {
                sb.AppendLine("            this." + field.Name + " = " + field.Name + ";");
            }
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        public " + ClassName + "() { }");
            sb.AppendLine();

            sb.AppendLine("        private static Dictionary<int, " + ClassName + "> config = new Dictionary<int, " + ClassName + ">();");
            sb.AppendLine("        public static Dictionary<int, " + ClassName + ">.ValueCollection ConfigList");
            sb.AppendLine("        {");
            sb.AppendLine("            get { return config.Values; }");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        public static void Refresh(Dictionary<int, " + ClassName + "> dict)");
            sb.AppendLine("        {");
            sb.AppendLine("            config.Clear();");
            sb.AppendLine("            config = dict;");
            sb.AppendLine("            RebuildIndex();");
            sb.AppendLine("        }");
            sb.AppendLine();

            sb.AppendLine("        public static void Load()");
            sb.AppendLine("        {");
            sb.AppendLine("            config.Clear();");

            foreach (var row in Rows)
            {
                sb.Append("            config[");
                string idVal = row.ContainsKey("Id") ? row["Id"] : "0";
                sb.Append(idVal);
                sb.Append("] = new ");
                sb.Append(ClassName);
                sb.Append("(");

                for (int i = 0; i < Fields.Count; i++)
                {
                    if (i > 0) sb.Append(", ");
                    string displayVal = row.ContainsKey(Fields[i].Name) ? row[Fields[i].Name] : "";
                    sb.Append(DisplayToRaw(displayVal, Fields[i].Type));
                }

                sb.Append(");");
                sb.AppendLine();
            }

            sb.AppendLine();
            sb.AppendLine("            RebuildIndex();");
            sb.AppendLine();
            sb.AppendLine("        }");

            sb.AppendLine();
            sb.AppendLine("        private static void RebuildIndex()");
            sb.AppendLine("        {");
            foreach (var field in Fields)
            {
                if (!field.IsIndex) continue;
                sb.AppendLine("            idx" + field.Name + ".Clear();");
            }
            sb.AppendLine("            foreach (var kv in config)");
            sb.AppendLine("            {");
            foreach (var field in Fields)
            {
                if (!field.IsIndex) continue;
                string keyType = field.Type == "int" ? "int" : "string";
                if (keyType == "string")
                {
                    sb.AppendLine("                if (!string.IsNullOrEmpty(kv.Value." + field.Name + ")) idx" + field.Name + "[kv.Value." + field.Name + "] = kv.Key;");
                }
                else
                {
                    sb.AppendLine("                idx" + field.Name + "[kv.Value." + field.Name + "] = kv.Key;");
                }
            }
            sb.AppendLine("            }");
            sb.AppendLine("        }");

            sb.AppendLine();
            sb.AppendLine("        public static " + ClassName + " GetConfig(int id)");
            sb.AppendLine("        {");
            sb.AppendLine("            " + ClassName + " data;");
            sb.AppendLine("            if (config.TryGetValue(id, out data))");
            sb.AppendLine("            {");
            sb.AppendLine("                return data;");
            sb.AppendLine("            }");
            sb.AppendLine("            throw new NullReferenceException(string.Format(\"配置表" + ClassName + "不存在id={0}\", id));");
            sb.AppendLine("        }");

            foreach (var field in Fields)
            {
                if (!field.IsIndex) continue;

                string keyType = field.Type == "int" ? "int" : "string";
                string paramType = keyType;

                sb.AppendLine();
                sb.AppendLine("        private static Dictionary<" + keyType + ", int> idx" + field.Name + " = new Dictionary<" + keyType + ", int>();");
                sb.AppendLine("        public static " + ClassName + " GetConfigBy" + field.Name + "(" + paramType + " val)");
                sb.AppendLine("        {");
                sb.AppendLine("            return GetConfig(idx" + field.Name + "[val]);");
                sb.AppendLine("        }");
            }

            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine("        public static bool HasConfig(int id)");
            sb.AppendLine("        {");
            sb.AppendLine("            if (config.ContainsKey(id))");
            sb.AppendLine("            {");
            sb.AppendLine("                return true;");
            sb.AppendLine("            }");
            sb.AppendLine("            return false;");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        public static void Assign(int id, " + ClassName + " configData)");
            sb.AppendLine("        {");
            sb.AppendLine("            config[id] = configData; ");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        public static void Add(int id, " + ClassName + " configData)");
            sb.AppendLine("        {");
            sb.AppendLine("            if (!config.ContainsKey(id))");
            sb.AppendLine("            {");
            sb.AppendLine("                config.Add(id, configData);");
            sb.AppendLine("            }");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        public static void Remove(int id)");
            sb.AppendLine("        {");
            sb.AppendLine("            if (config.ContainsKey(id))");
            sb.AppendLine("            {");
            sb.AppendLine("                config.Remove(id);");
            sb.AppendLine("            }");
            sb.AppendLine("        }");

            sb.AppendLine("    }");
            sb.AppendLine("}");

            return sb.ToString();
        }

        private string DisplayToRaw(string display, string type)
        {
            if (type == "int")
            {
                if (string.IsNullOrEmpty(display)) return "0";
                return display.Trim();
            }

            if (type == "float")
            {
                if (string.IsNullOrEmpty(display)) return "0f";
                string v = display.Trim();
                if (!v.EndsWith("f") && !v.EndsWith("F")) v = v + "f";
                return v;
            }

            if (type == "string")
            {
                if (display == null) return "\"\"";
                return "\"" + display.Replace("\\", "\\\\").Replace("\"", "\\\"") + "\"";
            }

            if (type == "bool")
            {
                if (string.IsNullOrEmpty(display)) return "false";
                return display.Trim().ToLower();
            }

            if (type == "string[]")
            {
                if (string.IsNullOrEmpty(display)) return "null";
                var items = display.Split(',');
                var sb = new StringBuilder();
                sb.Append("new string[]{");
                for (int i = 0; i < items.Length; i++)
                {
                    if (i > 0) sb.Append(",");
                    sb.Append("\"");
                    sb.Append(items[i].Trim().Replace("\\", "\\\\").Replace("\"", "\\\""));
                    sb.Append("\"");
                }
                sb.Append("}");
                return sb.ToString();
            }

            if (type == "int[]")
            {
                if (string.IsNullOrEmpty(display)) return "new int[0]";
                var items = display.Split(',');
                var sb = new StringBuilder();
                sb.Append("new int[]{");
                for (int i = 0; i < items.Length; i++)
                {
                    if (i > 0) sb.Append(",");
                    sb.Append(items[i].Trim());
                }
                sb.Append("}");
                return sb.ToString();
            }

            if (type == "float[]")
            {
                if (string.IsNullOrEmpty(display)) return "new float[0]";
                var items = display.Split(',');
                var sb = new StringBuilder();
                sb.Append("new float[]{");
                for (int i = 0; i < items.Length; i++)
                {
                    if (i > 0) sb.Append(",");
                    string v = items[i].Trim();
                    if (!v.EndsWith("f") && !v.EndsWith("F")) v = v + "f";
                    sb.Append(v);
                }
                sb.Append("}");
                return sb.ToString();
            }

            return display;
        }
    }
}
