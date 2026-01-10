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
            List<ApiData> apiList;
            if(!Global.ApiDict.TryGetValue(fileName, out apiList))
                apiList = new List<ApiData>();

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

            foreach (var api in apiList)
            {
                if (filter.IsIgnore(api.Cs))   //取包含c的部分
                    continue;

                keys.AppendLine("\trawset(self, \"DataBy" + api.ApiName + "\", {});");    //eg: rawset(self,"DataByIdAndChs",{});

                CreateDataByXxx(fileName, types, api, ref dataByXxx, ref getDataByXxx);
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

        /// <summary>
        ///  生成索引部分
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="types">字段类型总表</param>
        /// <param name="api">索引原始数据</param>
        /// <param name="dataByXxx">索引初始化</param>
        /// <param name="getDataByXxx">获取索引的接口</param>
        private void CreateDataByXxx(string fileName, List<CellType> types, ApiData api, ref StringBuilder dataByXxx, ref StringBuilder getDataByXxx)
        {
            var keysCount = api.Keys.Count; //key 的个数

            var dataByXx = "self.DataBy" + api.ApiName;
            var parms = string.Join(",", api.Keys.ToArray());
            getDataByXxx.AppendLine(string.Format("--根据{0}查找", parms));
            getDataByXxx.AppendLine(string.Format("function {0}Table:GetDataBy{1}({2})", fileName, api.ApiName, parms));  //function TextConfigTable:GetDataByIdAndChs(Id,Chs)
            getDataByXxx.AppendLine("\tself:Check();");                                                                   //    self:Check();

            for (int i = 0; i < keysCount; i++)
            {
                if (types.Find(field => field.FieldName == api.Keys[i]) == null)
                    throw new Exception(string.Format("ERROR: 不合法的api字段 fileName={0}, apiKey={1}", fileName, api.Keys[i]));
                dataByXx += "[" + api.Keys[i] + "]";
                if (i + 1 == keysCount)//最后一维
                {
                    if (api.ResultsOnly)
                    {
                        dataByXxx.AppendLine(string.Format("\t{0} = data;", dataByXx));     //self.DataByIdAndChs[Id][Chs] = data;
                    }
                    else
                    {
                        dataByXxx.AppendLine(string.Format("\tif({0} == nil) then", dataByXx));     //	if(self.DataByIdAndChs[Id][Chs] == nil) then
                        dataByXxx.AppendLine(string.Format("\t\t{0} = {1};", dataByXx, "{}"));       //		self.DataByIdAndChs[Id][Chs] = {};
                        dataByXxx.AppendLine("\tend");                                              //  end
                        dataByXxx.AppendLine(string.Format("\ttable.insert({0}, data);", dataByXx));//  table.insert(self.DataByIdAndChs[Id][Chs],data);

                    }
                    dataByXxx.AppendLine(); //空行  
                    getDataByXxx.AppendLine(string.Format("\treturn {0};", dataByXx));   //  return self.DataByIdAndChs[Id][Chs];         
                }
                else // 多维情况
                {
                    dataByXxx.AppendLine(string.Format("\tif({0} == nil) then", dataByXx));     //	if(self.DataByIdAndChs[Id] == nil) then
                    dataByXxx.AppendLine(string.Format("\t\t{0} = {1};", dataByXx, "{}"));      //		self.DataByIdAndChs[Id] = {};
                    dataByXxx.AppendLine("\tend");                                              //  end

                    getDataByXxx.AppendLine(string.Format("\tif({0} == nil) then", dataByXx));  //	if(self.DataByIdAndChs[Id] == nil) then
                    getDataByXxx.AppendLine("\t\treturn nil;");                                 //		return nil;
                    getDataByXxx.AppendLine("\tend");                                           //  end
                }
            }
            getDataByXxx.AppendLine("end");     //end
            getDataByXxx.AppendLine();          //空行
        }
    }
}
