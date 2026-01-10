using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Excel2dllCore.Export.Filter;
using Excel2dllCore.Load;
using Excel2dllCore.Tools;

namespace Excel2dllCore.Export.Lua.Item
{
    public class LuaGameTableExporter : ITableExporter 
    {
        public void ExportType(List<CellType> types, List<Record> records, string fullPath, string fileName, ICsFilter filter)
        {
        }

        public void ExportRecord(List<CellType> types, List<Record> records, string fullPath, string fileName, ICsFilter filter)
        {
            StringBuilder parms = new StringBuilder();

            //写字段定义
            for (int i = 0; i < records.Count; i++)
            {
                Record record = records[i];
                
                string recordname = record.Id; //字段名称
                string recordDesc = ""; //字段描述
                //string recordType = ""; //字段类型
                string recordVal = ""; //字段值

                string recordTypeOld = "";//字段类型

                foreach (CellValue value in record.Values)
                {
                    if (value.Type.FieldName == "Desc")
                    {
                        recordDesc = value.Value;
                    }
                    else if (value.Type.FieldName == "Type")
                    {
                        recordTypeOld = value.Value;
                        //recordType = RegisterType.GetTypeName(value.Value);
                    }
                    else if (value.Type.FieldName == "Value")
                    {
                        recordVal = value.Value;
                    }
                }
                //recordType = recordType.Replace("\"", "");

                RegisterType.ProcessValue(recordTypeOld, ref recordVal);

                parms.AppendLine(string.Format("\trawset(self, \"{0}\", {1} ); -- {2}", recordname, recordVal, recordDesc));
            }

            using (TemplateFileWriterV2 writer = new TemplateFileWriterV2(fullPath, "GameConfig.lua"))
            {
                writer.SetVar("xxxConfig", fileName);
                writer.SetVar("vars", parms.ToString());
            }
        }
    }
}
