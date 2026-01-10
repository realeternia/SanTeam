using Excel2dllCore.Tools;
using System.Collections.Generic;
using System.IO;

namespace Excel2dllCore.Load
{
    internal class DataLoader
    {
        public static List<string> AllFiles { get; private set; }

        static DataLoader()
        {
            AllFiles = Utils.GetAllConfigFiles(ParseCommand.ConfigPath, ".xls");
        }

        public static void ProcessConfigFiles(List<string> files)
        {
            using (new UXAutoStopWatch())
            {
                Logger.Debug(string.Format("开始数据读取，总数={0}", files.Count));
                foreach (var file in files)
                {
                    FileInfo fileInfo = new FileInfo(file);
                    var fileName = fileInfo.Name.Substring(0, fileInfo.Name.Length - fileInfo.Extension.Length);
                    LoadFromFile(fileInfo, fileName);
                }
                Logger.Debug(string.Format("数据读取完成，总数={0}", Global.CheckerDataList.Count));
            }
        }

        public static bool TryLoadConfigIdCol(string excelName)
        {
            var fileName1 = string.Format("\\{0}.xlsx", excelName);
            var fileName2 = string.Format("\\{0}.xls", excelName);
            var fileName3 = string.Format("\\{0}.cfg", excelName);
            var filePath = AllFiles.Find(a => a.EndsWith(fileName1) || a.EndsWith(fileName2) || a.EndsWith(fileName3));
            if (string.IsNullOrEmpty(filePath))
                return false;

            FileInfo fileInfo = new FileInfo(filePath);
            ReadConfig readConfig;
            readConfig = new ReadExcel();

            if (readConfig.LoadIdCol(fileInfo, excelName))
            {
                Global.TypeDict[excelName] = readConfig.Types;
                Global.DataDict[excelName] = readConfig.Records;
                return true;
            }
            return false;
        }

        public static void LoadFromMem(LineData[] headers, LineData[] records, string fileName)
        {
            ReadConfig readExcel = new ReadExcel();
            readExcel.LoadDataToMem(fileName, "sheet1", headers, new List<LineData>(records));
            Global.TypeDict[fileName] = readExcel.Types;
            Global.DataDict[fileName] = readExcel.Records;
        }

        private static void LoadFromFile(FileInfo fileInfo, string fileName)
        {
            ReadConfig readConfig;
            readConfig = new ReadExcel();

            if (readConfig.ProcessFile(fileInfo, fileName))
            {
                Global.TypeDict[fileName] = readConfig.Types; //一个文件处理完后，就把数据存到全局容器中
                Global.DataDict[fileName] = readConfig.Records;
            }
        }

    }
}