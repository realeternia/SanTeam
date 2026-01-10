using System.Collections.Generic;
using System.Text;
using Excel2dllCore.Export.Filter;
using Excel2dllCore.Load;
using Excel2dllCore.Tools;

namespace Excel2dllCore.Export.CSharp.Item
{
    public class CsGameTableExporter : ITableExporter
    {

        // 生成Type文件
        public void ExportType(List<CellType> types, List<Record> records, string fullPath, string fileName, ICsFilter filter)
        {
            StringBuilder ctrParms = new StringBuilder();

            //写字段定义
            for (int i = 0; i < records.Count; i++)
            {
                Record record = records[i];
                //字段名称
                string recordname = record.Id;
                //字段描述
                string recordDesc = "";
                //字段类型
                string recordType = "";

                foreach (CellValue value in record.Values)
                {
                    if (value.Type.FieldName == "Desc")
                    {
                        recordDesc = value.Value;
                    }
                    else if (value.Type.FieldName == "Type")
                    {
                        recordType = RegisterType.GetTypeName(value.Value);
                    }
                }
                recordType = recordType.Replace("\"", "");

                ctrParms.AppendLine("\t\t/// <summary>");
                ctrParms.AppendLine("\t\t///" + recordDesc);
                ctrParms.AppendLine("\t\t/// </summary>");
                ctrParms.AppendLine("\t\tpublic static " + recordType + " " + recordname + ";");
            }

            using (TemplateFileWriterV2 writer = new TemplateFileWriterV2(fullPath, "GameConfig.cs"))
            {
                writer.SetVar("namespace", RegisterType.GetNamespaceStr());
                writer.SetVar("vars", ctrParms.ToString());
            }
        }


        // 生成Record文件
        public void ExportRecord(List<CellType> types, List<Record> records, string fullPath, string fileName, ICsFilter filter)
        {
            StringBuilder bldMethodDefine = new StringBuilder();

            List<string> typeList = new List<string>();
            //写数据记录
            for (int i = 0; i < records.Count; i++)
            {
                Record record = records[i];
                //字段名称
                string recordname = record.Id;
                //字段值
                string recordValue = "";
                //字段类型
                string recordType = "";

                foreach (CellValue value in record.Values)
                {
                    if (value.Type.FieldName == "Value")
                    {
                        recordValue = value.Value;
                    }
                    else if (value.Type.FieldName == "Type")
                    {
                        recordType = value.Value;
                    }
                }

                typeList.Add(recordname);

                recordType = recordType.Replace("\"", "");
                RegisterType.ProcessValue(recordType, ref recordValue);

                bldMethodDefine.AppendLine(string.Format("\t\t\t{0}.{1}={2};", fileName, recordname, recordValue));
            }

            using (TemplateFileWriterV2 writer = new TemplateFileWriterV2(fullPath, "GameAddRecord.cs"))
            {
                writer.SetVar("namespace", RegisterType.GetNamespaceStr());
                writer.SetVar("fileName", fileName);
                writer.SetVar("methodDefine", bldMethodDefine.ToString());
            }
        }
    }
}