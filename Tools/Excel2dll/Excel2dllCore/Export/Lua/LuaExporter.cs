using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Excel2dllCore.Export.Filter;
using Excel2dllCore.Tools;

namespace Excel2dllCore.Export.Lua
{
    internal class LuaExporter : IExporter
    {
        private ICsFilter filter;
        public LuaExporter()
        {
            if (ParseCommand.IsClient)
            {
                filter = new CommonCsFilter("c");
            }
            else if (ParseCommand.IsServer)
            {
                filter = new CommonCsFilter("s");
            }
            else if (ParseCommand.IsAll)
            {
                filter = new FakeCsFilter();
            }
        }
        public void Prework()
        {
            Utils.DeleteFolder(ParseCommand.OutPath);
        }

        public void ExportRecord()
        {
            Dictionary<string, List<CellType>> types = Global.TypeDict;
            Dictionary<string, List<Record>> records = Global.DataDict;

            string recPath = ParseCommand.OutPath;
            if (!Directory.Exists(recPath))
            {
                Directory.CreateDirectory(recPath);
                Logger.Debug("\tCreate Directory: " + recPath);
            }

            foreach (var fileName in records.Keys)
            {
                string recordFile = fileName + "Table.lua";
                string typePath = recPath + Path.DirectorySeparatorChar + recordFile;
                LuaExportItemManager.ExportRecord(types[fileName], records[fileName], typePath, fileName, filter);
                Logger.Debug("\t导出数据文件" + typePath);
            }
        }

        public void ExportType()
        {
            
            Logger.Debug("开始ConfigDataManager导出");
            Dictionary<string, List<CellType>> types = Global.TypeDict;

            var typePath = ParseCommand.OutPath;
            if (!Directory.Exists(typePath))
            {
                Directory.CreateDirectory(typePath);
                Logger.Debug("\tCreate Directory: " + typePath);
            }

            List<string> configList = new List<string>();
            foreach (var fName in types.Keys)
            {
                configList.Add(fName + "Table");
            }
            string fileName = "ConfigDataManager.lua";
            string path = typePath + Path.DirectorySeparatorChar + fileName;
            //LuaExportItemManager.ExportType(types[fileName], records[fileName], path, fileName, filter);
            LuaExportItemManager.ExportConfigDataManager(configList, path, filter);
            Logger.Debug("导出ConfigDataManager完成");
            
        }

        public void ExportVersion()
        {
            
        }
    }
}
