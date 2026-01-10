using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Excel2dllCore.Export.CSharp.Item;
using Excel2dllCore.Export.Filter;
using Excel2dllCore.Export.Lua.Item;
using Excel2dllCore.Tools;

namespace Excel2dllCore.Export.Lua
{
    public class LuaExportItemManager
    {
        private static Dictionary<string, ITableExporter> userExporterDict = new Dictionary<string, ITableExporter>();
        private static ITableExporter _trivialTableExporter;
        static LuaExportItemManager()
        {
            _trivialTableExporter = new LuaCommonTableExporter();
            userExporterDict["GameConfig"] = new LuaGameTableExporter();
        }

        public static void ExportType(List<CellType> types, List<Record> records, string fullPath, string fileName, ICsFilter filter)
        {
        }

        public static void ExportRecord(List<CellType> types, List<Record> records, string fullPath, string fileName, ICsFilter filter)
        {
            var uselessCount = types.FindAll(a => filter.IsIgnore(a.CS)).Count;
            if (types.Count == uselessCount)
                return;

            ITableExporter tableExporter;
            if (userExporterDict.TryGetValue(fileName, out tableExporter))
            {
                tableExporter.ExportRecord(types, records, fullPath, fileName, filter);
            }
            else
            {
                _trivialTableExporter.ExportRecord(types, records, fullPath, fileName, filter);
            }
        }

        //导出ConfigDataManager配置文件
        public static void ExportConfigDataManager(List<string> configList, string path, ICsFilter filter)
        {
            StringBuilder require = new StringBuilder();    //请求部分
            StringBuilder clear = new StringBuilder();      //clear部分
            StringBuilder getConfig = new StringBuilder();  //get函数部分

            foreach (var configTable in configList)
            {
                require.AppendLine(string.Format("\trawset(self,\"{0}\",require \"Model/Data/CfgData/{0}\");", configTable));  //  rawset(self,"TextConfigTable",require "Model/Data/CfgData/TextConfigTable");

                clear.AppendLine(string.Format("\tself.{0}:Clear(language);",configTable));    //  self.TextConfigTable:Clear(language);

                getConfig.AppendLine(string.Format("function ConfigDataManager:Get{0}()",configTable)); //  function ConfigDataManager:GetTextConfigTable()
                getConfig.AppendLine(string.Format("\treturn self.{0};",configTable));                  //      return self.TextConfigTable;
                getConfig.AppendLine("end");                                                            //  end
                getConfig.AppendLine();
            }

            using (TemplateFileWriterV2 writer = new TemplateFileWriterV2(path, "ConfigDataManager.lua"))
            {
                writer.SetVar("requireConfigTable", require.ToString());
                writer.SetVar("clearConfigTable", clear.ToString());
                writer.SetVar("getConfigTable()", getConfig.ToString());

            }
        }
    }
}
