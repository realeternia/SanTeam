using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Excel2dllCore.Export.Filter;
using Excel2dllCore.Load;
using Excel2dllCore.Tools;

namespace Excel2dllCore.Export.Lua.Item
{
    public class LuaCommonTableExporter : ITableExporter
    {
        public void ExportType(List<CellType> types, List<Record> records, string fullPath, string fileName, ICsFilter filter)
        {
            
        }

        public void ExportRecord(List<CellType> types, List<Record> records, string fullPath, string fileName, ICsFilter filter)
        {
            /**
             * xxxConfig 结构部分 
             **/
            StringBuilder parms = new StringBuilder();  //参数
            StringBuilder fields = new StringBuilder(); //字段名


            /**
             * xxxConfigTable 内容部分
             **/
            StringBuilder keys = new StringBuilder();   //api key

            StringBuilder alias = new StringBuilder();  //别名

            StringBuilder chsDatas = new StringBuilder();   //简体数据部分
            StringBuilder chtDatas = new StringBuilder();   //繁体数据部分
            StringBuilder engDatas = new StringBuilder();   //英文数据部分

            StringBuilder size = new StringBuilder();   //数据大小


            /**
             * dataByXxx 函数部分
             **/
            StringBuilder dataByXxx = new StringBuilder();
            StringBuilder getDataByXxx = new StringBuilder();

            foreach (CellType type in types)
            {
                //string vtype = "unknown";

                if (filter.IsIgnore(type.CS))   //生成字段过滤 取包含c的部分
                    continue;
                if (type.Type.Equals("any"))
                    continue;

                //var typeName = RegisterType.GetTypeName(type.Type);// 字段的类型
                //if (typeName == "")
                //    Logger.Debug("unknown: " + type.Type.ToLower());
                //else
                //    vtype = typeName;

                var fieldNameArray = type.FieldName.Split('_');
                if (fieldNameArray.Length > 1 && fieldNameArray[1] != "chs")
                    continue;

                parms.Append(parms.Length > 0 ? ", " : "");
                parms.Append(fieldNameArray[0]);
                fields.AppendLine(string.Format("\trawset(self, \"{0}\", {0}); --{1}", fieldNameArray[0], type.Desc));
            }

            //数据部分
            if (records.Count > 0)
            {
                foreach (var data in records)
                {
                    if (!string.IsNullOrEmpty(data.ConstantName))//别名
                        alias.AppendLine(string.Format("\trawset(self,\"{0}\", {1});", data.ConstantName, data.Id));    //	rawset(self,"Alias1", 1);

                    var feildData = new Dictionary<string, string>();
                    foreach (var feild in data.Values)
                    {
                        if (filter.IsIgnore(feild.Type.CS))   //生成字段过滤 取包含c的部分
                            continue;
                        if (feild.Type.Type.Equals("any"))
                            continue;

                        var fieldNameArray = feild.Type.FieldName.Split('_'); // 字段名
                        string recordVal = feild.Value; //字段值
                        if (fieldNameArray.Length > 1)
                        {
                            var key = fieldNameArray[1];
                            if (!feildData.ContainsKey(key))
                                continue;
                            feildData[key] += feildData[key].Length > 0 ? ", " : "";
                            feildData[key] += recordVal;
                        }
                        else //公共部分赋值
                        {
                            var keyList = new List<string>(feildData.Keys); ;
                            foreach (var key in keyList)
                            {
                                feildData[key] += feildData[key].Length > 0 ? ", " : "";
                                feildData[key] += recordVal;
                            }                            
                        }

                        //RegisterType.ProcessValue(feild.Type.Type, ref recordVal);// string[], 2,3 => {"2", "3"}
                        //feildData += feildData.Length > 0 ? ", " : "";
                        //feildData += recordVal;
                    }
                    if(feildData.ContainsKey("chs"))
                        chsDatas.AppendLine(string.Format("\t\tself:AddData({0});", feildData["chs"])); // self:AddData(1, "龙", 10);
                    if(feildData.ContainsKey("cht"))
                        chtDatas.AppendLine(string.Format("\t\tself:AddData({0});", feildData["cht"])); // self:AddData(1, "龍", 10);
                    if(feildData.ContainsKey("eng"))
                        engDatas.AppendLine(string.Format("\t\tself:AddData({0});", feildData["eng"])); // self:AddData(1, "Dragon", 10);
                }
                size.AppendLine(string.Format("\trawset(self, \"Size\", {0});", records.Count));//rawset(self, "Size", 2);
            }
            using (TemplateFileWriterV2 writer = new TemplateFileWriterV2(fullPath, "XXAddRecord.lua"))
            {
                writer.SetVar("xxxConfig", fileName);
                writer.SetVar("parms", parms.ToString());
                writer.SetVar("fields", fields.ToString());
                writer.SetVar("keys", keys.ToString());
                writer.SetVar("alias", alias.ToString());
                writer.SetVar("chsDatas", chsDatas.ToString());
                writer.SetVar("chtDatas", chtDatas.ToString());
                writer.SetVar("engDatas", engDatas.ToString());
                writer.SetVar("size", size.ToString());
                writer.SetVar("dataByXxx", dataByXxx.ToString());
                writer.SetVar("getDataByXxx()", getDataByXxx.ToString());

            }
        }

    }
}
