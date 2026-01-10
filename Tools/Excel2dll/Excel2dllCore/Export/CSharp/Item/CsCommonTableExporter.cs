using System;
using System.Collections.Generic;
using System.Text;
using Excel2dllCore.Export.Filter;
using Excel2dllCore.Load;
using Excel2dllCore.Tools;
using System.IO;

namespace Excel2dllCore.Export.CSharp.Item
{
    public class CsCommonTableExporter : ITableExporter
    {
        // 生成Type文件
        public void ExportType(List<CellType> types, List<Record> records, string fullPath, string fileName, ICsFilter filter)
        {
            StringBuilder ctrParms = new StringBuilder();
            StringBuilder ctrDef = new StringBuilder();
            StringBuilder ctrData = new StringBuilder();
            StringBuilder ctrVars = new StringBuilder();

            List<string> validFieldNames = new List<string>(); // 用于记录有效的字段名

            foreach (CellType type in types)
            {
                string vtype = "unknown";

                if (filter.IsIgnore(type.CS))
                {
                    continue;
                }

                var typeName = RegisterType.GetTypeName(type.Type);
                if (typeName == "")
                {
                    Logger.Debug("unknown: " + type.Type.ToLower());
                }
                else
                {
                    vtype = typeName;
                }

                var fieldNameArray = type.FieldName.Split('_');
                if (fieldNameArray.Length > 1 && fieldNameArray[1] != "chs")
                    continue;

                var fieldName = fieldNameArray[0];
                validFieldNames.Add(fieldName); // 记录有效字段名

                ctrDef.AppendLine(string.Format("            this.{0} = {0};", fieldName));
                ctrParms.Append(vtype).Append(" ").Append(fieldName).Append(", ");
                ctrVars.AppendLine("        /// <summary>");
                ctrVars.AppendLine("        ///" + type.Desc.Replace("\n", "///"));
                ctrVars.AppendLine("        /// </summary>");
                ctrVars.AppendLine(string.Format("        public {0} {1};", vtype, fieldName));
            }

            // 生成对象构造代码
            foreach (var record in records)
            {
                StringBuilder args = new StringBuilder();
                foreach (var fieldName in validFieldNames)
                {
                    if (record.TryGetValue(fieldName, out var value))
                    {
                        // 根据字段类型处理值（如字符串加引号）
                        string formattedValue = FormatValueByType(value, GetFieldType(types, fieldName));
                        args.Append(formattedValue).Append(", ");
                    }
                    else
                    {
                        args.Append("null, "); // 默认值处理
                    }
                }

                if (args.Length > 2)
                    args.Length -= 2; // 去掉最后的 ", "

                ctrData.AppendLine($"            config[{record.Id}] = new {fileName}({args});");
            }

            if (types.Count > 0 && ctrParms.Length > 2)
                ctrParms.Length -= 2; // 去掉最后的 ", "

            using (TemplateFileWriterV2 writer = new TemplateFileWriterV2(fullPath, "XXConfig.cs"))
            {
                writer.SetVar("namespace", RegisterType.GetNamespaceStr());
                writer.SetVar("fileName", fileName);
                writer.SetVar("parms", ctrParms.ToString());
                writer.SetVar("construct", ctrDef.ToString());
                writer.SetVar("loadm", ctrData.ToString());
                writer.SetVar("vars", ctrVars.ToString());
            }
        }

        // 辅助方法：根据字段名获取字段类型
        private string GetFieldType(List<CellType> types, string fieldName)
        {
            foreach (var type in types)
            {
                var fieldNameArray = type.FieldName.Split('_');
                if (fieldNameArray[0] == fieldName)
                    return type.Type;
            }
            return "string"; // 默认
        }

        // 辅助方法：格式化值（如字符串加引号）
        private string FormatValueByType(object value, string type)
        {
            if (value == null) return "null";

            switch (type.ToLower())
            {
                case "string":
                case "text":
                    return $"\"{value.ToString().Replace("\"", "")}\"";
                case "bool":
                    return value.ToString().ToLower();
                default:
                    return value.ToString();
            }
        }

        // 生成Record文件
        public void ExportRecord(List<CellType> types, List<Record> records, string fullPath, string fileName, ICsFilter filter)
        {
          
        }
    }
}