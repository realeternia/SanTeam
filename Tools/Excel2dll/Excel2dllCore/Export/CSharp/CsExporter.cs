using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Excel2dllCore.Export.Filter;
using Excel2dllCore.Load;
using Excel2dllCore.Tools;

namespace Excel2dllCore.Export.CSharp
{
    //导出数据代码
    internal class CsExporter : IExporter
    {
        private ICsFilter filter;
        public CsExporter()
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
            //删除上次生成的文件
            Utils.DeleteFolder(ParseCommand.OutPath + "/Csharp_c");
            Utils.DeleteFolder(ParseCommand.OutPath + "/Csharp_s");
        }

        public void ExportRecord()
        {
            Logger.Debug("开始数据导出");
            ExportRecord1();
            ExportRecordManager();
            ExportRecordTool();
            ConfigBinaryReader(); 
            Logger.Debug("数据导出完成");
        }

        private void ExportRecord1()
        {
            Dictionary<string, List<CellType>> types = Global.TypeDict;
            Dictionary<string, List<Record>> records = Global.DataDict;

            string recPath = ParseCommand.OutPath + Path.DirectorySeparatorChar + "Csharp_" + filter.Name;
            if (!Directory.Exists(recPath))
            {
                Directory.CreateDirectory(recPath);
                Logger.Debug("\tCreate Directory: " + recPath);
            }

            foreach (var fileName in records.Keys)
            {
                string recordFile = fileName + "RecordAdder_" + filter.Name + ".cs";
                string typePath = recPath + Path.DirectorySeparatorChar + recordFile;
                CsExportItemManager.ExportRecord(types[fileName], records[fileName], typePath, fileName, filter);
                Logger.Debug("\t导出数据文件" + typePath);
            }
        }

        private void ExportRecordManager()
        {
            string[] fileNameArray = Global.TypeDict.Keys.ToArray();
            string mgrFile = "RecordManager_" + filter.Name + ".cs";
            string rpath = ParseCommand.OutPath + Path.DirectorySeparatorChar + "Csharp_" + filter.Name;
            if (!Directory.Exists(rpath))
            {
                Directory.CreateDirectory(rpath);
                Logger.Debug("\tCreate Directory: " + rpath);
            }

            string fullpath = rpath + Path.DirectorySeparatorChar + mgrFile;
            StringBuilder bldMethodLoad = new StringBuilder();
            StringBuilder bldMethodUnload = new StringBuilder();

            foreach (string fileName in fileNameArray)
            {
                bldMethodLoad.AppendLine(string.Format("            {0}RecordAdder.Load(br);", fileName));
                bldMethodLoad.AppendLine(string.Format("            {0}RecordAdder.Unload();", fileName));
            }
            using (TemplateFileWriterV2 writer = new TemplateFileWriterV2(fullpath, "RecordMan.cs"))
            {
                writer.SetVar("namespace", RegisterType.GetNamespaceStr());
                writer.SetVar("methodLoad", bldMethodLoad.ToString());
                writer.SetVar("methodUnload", bldMethodUnload.ToString());
            }
        }

        private void ExportRecordTool()
        {
            string[] fileNameArray = Global.TypeDict.Keys.ToArray();
            string mgrFile = "RecordTool_" + filter.Name + ".cs";
            string rpath = ParseCommand.OutPath + Path.DirectorySeparatorChar + "Csharp_" + filter.Name;
            if (!Directory.Exists(rpath))
            {
                Directory.CreateDirectory(rpath);
                Logger.Debug("\tCreate Directory: " + rpath);
            }

            //将数据类型md5值写入模板
            System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] typeMd5 = md5.ComputeHash(Encoding.GetEncoding("utf-8").GetBytes(RegisterType.typeCount.ToString()));
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < typeMd5.Length; i++)
            {
                sb.Append(typeMd5[i].ToString("X2"));
            }

            string fullpath = rpath + Path.DirectorySeparatorChar + mgrFile;
            using (TemplateFileWriterV2 writer = new TemplateFileWriterV2(fullpath, "RecordTool.cs"))
            {
                writer.SetVar("namespace", RegisterType.GetNamespaceStr());
                writer.SetVar("typeMd5", sb.ToString());
            }
        }

        private void ConfigBinaryReader()
        {
            string[] fileNameArray = Global.TypeDict.Keys.ToArray();
            string mgrFile = "ConfigBinaryReader_" + filter.Name + ".cs";
            string rpath = ParseCommand.OutPath + Path.DirectorySeparatorChar + "Csharp_" + filter.Name;
            if (!Directory.Exists(rpath))
            {
                Directory.CreateDirectory(rpath);
                Logger.Debug("\tCreate Directory: " + rpath);
            }

            string fullpath = rpath + Path.DirectorySeparatorChar + mgrFile;

            using (TemplateFileWriterV2 writer = new TemplateFileWriterV2(fullpath, "ConfigBinaryReader.cs"))
            {
                writer.SetVar("namespace", RegisterType.GetNamespaceStr());
            }
        }

        //生成类型代码
        public void ExportType()
        {
            Logger.Debug("开始类型导出");
            Dictionary<string, List<CellType>> types = Global.TypeDict;
            Dictionary<string, List<Record>> records = Global.DataDict;

            var typePath = ParseCommand.OutPath + Path.DirectorySeparatorChar + "Csharp_" + filter.Name;
            if (!Directory.Exists(typePath))
            {
                Directory.CreateDirectory(typePath);
                Logger.Debug("\tCreate Directory: " + typePath);
            }

            foreach (var fileName in types.Keys)
            {
                string typeFile = fileName + "_" + filter.Name + ".cs";
                string path = typePath + Path.DirectorySeparatorChar + typeFile;
                CsExportItemManager.ExportType(types[fileName], records[fileName], path, fileName, filter);
                Logger.Debug("\t导出类型文件" + path);
            }
            Logger.Debug("开始类型完成");
        }

        public void ExportVersion()
        {
            const string versionFile = "AssemblyInfo.cs";
            string rpath = ParseCommand.OutPath + Path.DirectorySeparatorChar + "Csharp_" + filter.Name;
            if (!Directory.Exists(rpath))
            {
                Directory.CreateDirectory(rpath);
                Logger.Debug("\tCreate Directory: " + rpath);
            }

            string versionFilePath = rpath + Path.DirectorySeparatorChar + versionFile;
            int versionIndex = (DateTime.Now.Hour * 3600 + DateTime.Now.Minute * 60 + DateTime.Now.Second) % 65535;//版本的区别可以使一个assembly被一个appdomain多次加载

            using (TemplateFileWriterV2 writer = new TemplateFileWriterV2(versionFilePath, "Version.cs"))
            {
                writer.SetVar("version", versionIndex.ToString());
            }
        }
    }
}
