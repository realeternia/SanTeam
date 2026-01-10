using System;
using System.Collections.Generic;
using System.IO;
using Excel2dllCore.Tools;
using OfficeOpenXml;
using Excel2dllCore.Check;

namespace Excel2dllCore.Merge
{
    internal static class Merger
    {
        public static readonly List<IMerger> mergeDataList = new List<IMerger>();

        public static List<string> MergedFiles { get; private set; }

        static Merger()
        {
            MergedFiles = new List<string>();
        }

        /// <summary>
        /// 处理需要Merge的文件
        /// </summary>
        public static void Process()
        {
            //ReadConfigFile(ParseCommand.ConfigPath); //读取config.dat文件
            ReadConfigExcel(ParseCommand.ConfigPath); //读取config.xlsx文件

            if (mergeDataList.Count == 0)
            {
                return;
            }

            Logger.Debug(string.Format("开始数据合表，总数={0}", mergeDataList.Count));
            foreach (var newDataType in mergeDataList)
            {
                if (!newDataType.Check(ParseCommand.ConfigPath))
                {
                    throw new Exception("ERROR: 数据合表失败");
                }
            }

            foreach (var newDataType in mergeDataList)
            {
                newDataType.Process(ParseCommand.ConfigPath);
            }

            Logger.Debug(string.Format("数据合表成功，总数={0}", mergeDataList.Count));
        }

        // 读取config.dat文件
        private static void ReadConfigFile(string path)
        {
            string configPath = Path.GetFullPath(path) + Path.DirectorySeparatorChar + "config.dat";
            if (!File.Exists(configPath))
            {
                Logger.Warn("Merge Config File Not Found");
                return;
            }
            Logger.Debug("读取合表文件 config.dat");

            StreamReader sr = new StreamReader(configPath);
            string line = "";
            while ((line = sr.ReadLine()) != null)  //有2种表合并的方式
            {
                line = line.Trim();
                if (line == "Merge")                //抽取出多张表的前n列合并到新表中
                {
                    LoadMerge(sr);
                }
                else if (line == "ModelMerge")      //根据结构表合并到新表，如遇未填数据的则读取结构模板表中的默认数据
                {
                    LoadModelMerge(sr);
                }
            }
            sr.Close();
        }

        // 读取config.xlsx文件
        private static void ReadConfigExcel(string path)
        {
            string configName = Path.GetFullPath(path) + Path.DirectorySeparatorChar + "config.xlsx";
            if (!File.Exists(configName))
            {
                Logger.Warn("Merge Config Excel Not Found");
                return;
            }
            Logger.Debug("读取合表文件 config.xlsx");

            FileInfo file = new FileInfo(configName);
            ExcelPackage ep = ExcelFileOpener.Open(file, false);
            ExcelWorkbook workbookIn = ep.Workbook;

            for (int i = 1; i <= workbookIn.Worksheets.Count; i++)
            {
                ExcelWorksheet sheetIn = workbookIn.Worksheets[i];
                if (sheetIn.Dimension == null)
                    continue;
                if (sheetIn.Name == "Merge")
                {
                    ExcelLoadMerge(sheetIn);
                }
                else if (sheetIn.Name == "PartialMerge")
                {
                    ExcelLoadModelMerge(sheetIn);
                }
                else if (sheetIn.Name == "NoIdCheck")
                {
                    ExcelLoadNoIdCheck(sheetIn);
                }
                else if (sheetIn.Name == "ASConfig")
                {
                    ExcelLoadASConfig(sheetIn);
                }
            }
            ep.Dispose();        
        }

        /// <summary>
        /// 读取Merge配置信息
        /// </summary>
        private static void LoadMerge(StreamReader sr)
        {
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                line = line.Trim();
                if (line == "End")
                {
                    break;
                }
                if (line.StartsWith("//"))
                {
                    continue;
                }

                int columnCount;
                string[] infos = line.Split('\t');
                CommonMerger data = new CommonMerger();
                data.NewFileName = infos[0];

                if (int.TryParse(infos[1], out columnCount)) //第二行是不是数字
                {
                    data.ColumnCount = columnCount;
                    data.BranchFileList = new string[infos.Length - 2];
                    for (int i = 0; i < data.BranchFileList.Length; i++)
                    {
                        data.BranchFileList[i] = infos[i + 2];
                    }
                }
                else
                {
                    data.ColumnCount = 10000;//默认合全表，大于任何表列数
                    data.BranchFileList = new string[infos.Length - 1];
                    for (int i = 0; i < data.BranchFileList.Length; i++)
                    {
                        data.BranchFileList[i] = infos[i + 1];
                    }
                }
                mergeDataList.Add(data);
            }
        }

        /// <summary>
        /// 读取ModelMerge配置信息
        /// </summary>
        private static void LoadModelMerge(StreamReader sr)
        {
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                line = line.Trim();
                if (line == "End")
                {
                    break;
                }
                if (line.StartsWith("//"))
                {
                    continue;
                }

                string[] infos = line.Split('\t');
                ModelMerger data = new ModelMerger();
                data.NewFileName = infos[0];
                data.StructFileName = infos[1];
                data.ModelFileName = infos[2];
                data.BranchFileList = new string[infos.Length - 2];
                for (int i = 0; i < data.BranchFileList.Length; i++)
                {
                    data.BranchFileList[i] = infos[i + 2];
                }
                mergeDataList.Add(data);
            }
        }

        /// <summary>
        /// 读取Excel文件中的Merge配置信息
        /// </sum
        private static void ExcelLoadMerge(ExcelWorksheet sheetIn)
        {
            int rowLength = sheetIn.Dimension.End.Row;
            int colLength = sheetIn.Dimension.End.Column;

            if (colLength != 4)
            {
                throw new Exception("Error : Merge 子表填写格式不对");
            }
            for (int row = 2; row < rowLength + 1; row++)
            {
                if (sheetIn.GetValue(row, 4).ToString().Trim() == "xlsx")
                {
                    CommonMerger data = new CommonMerger();
                    int columnCount;
                    data.NewFileName = sheetIn.GetValue(row, 1).ToString().Trim();
                    MergedFiles.Add(data.NewFileName);
                    IdRepeatCheck.NoIdCheckFileNameList.Add(sheetIn.GetValue(row, 1).ToString().Trim());//合表后文件不进行id全局检查
                    var columnString = sheetIn.GetValue(row, 2);
                    if (columnString == null)
                    {
                        data.ColumnCount = 10000;//没有填写列数，默认合全表，大于任何表列数
                    }
                    else
                    {
                        int.TryParse(columnString.ToString().Trim(), out columnCount);
                        data.ColumnCount = columnCount;
                    }
                    string[] branchFiles = sheetIn.GetValue(row, 3).ToString().Trim().Split('|');
                    data.BranchFileList = new string[branchFiles.Length];
                    for (int i = 0; i < data.BranchFileList.Length; i++)
                    {
                        data.BranchFileList[i] = branchFiles[i];
                    }
                    mergeDataList.Add(data);
                }
                else
                {
                    CfgMerger data = new CfgMerger();
                    int columnCount;
                    data.NewFileName = sheetIn.GetValue(row, 1).ToString().Trim();
                    MergedFiles.Add(data.NewFileName);
                    IdRepeatCheck.NoIdCheckFileNameList.Add(sheetIn.GetValue(row, 1).ToString().Trim());//合表后文件不进行id全局检查
                    var columnString = sheetIn.GetValue(row, 2);
                    if (columnString == null)
                    {
                        data.ColumnCount = 10000;//没有填写列数，默认合全表，大于任何表列数
                    }
                    else
                    {
                        int.TryParse(columnString.ToString().Trim(), out columnCount);
                        data.ColumnCount = columnCount;
                    }
                    string[] branchFiles = sheetIn.GetValue(row, 3).ToString().Trim().Split('|');
                    data.BranchFileList = new string[branchFiles.Length];
                    for (int i = 0; i < data.BranchFileList.Length; i++)
                    {
                        data.BranchFileList[i] = branchFiles[i];
                    }
                    mergeDataList.Add(data);
                }
            }
        }

        /// <summary>
        /// 读取Excel文件中的ModerMerge配置信息
        /// </sum
        private static void ExcelLoadModelMerge(ExcelWorksheet sheetIn)
        {
            int rowLength = sheetIn.Dimension.End.Row;
            int colLength = sheetIn.Dimension.End.Column;

            if (colLength != 4)
            {
                throw new Exception("Error : ModelMerge 子表填写格式不对");
            }
            for (int row = 2; row < rowLength + 1; row++)
            {
                ModelMerger data = new ModelMerger();
                data.NewFileName = sheetIn.GetValue(row, 1).ToString().Trim();
                //if (CheckMergeExcelExist(data.NewFileName))
                //{
                //    Logger.Error(string.Format("ERROR: config表中填写的合表后文件出现在config文件中，文件名={0}，后续可能出现id及其别名重复问题", data.NewFileName));
                //}
                IdRepeatCheck.NoIdCheckFileNameList.Add(sheetIn.GetValue(row, 1).ToString().Trim());//合表后文件不进行id全局检查
                data.StructFileName = sheetIn.GetValue(row, 2).ToString().Trim();
                data.ModelFileName = sheetIn.GetValue(row, 3).ToString().Trim();
                MergedFiles.Add(data.ModelFileName);
                string[] branchFiles = sheetIn.GetValue(row, 4).ToString().Trim().Split('|');
                data.BranchFileList = new string[branchFiles.Length];
                for (int i = 0; i < data.BranchFileList.Length; i++)
                {
                    data.BranchFileList[i] = branchFiles[i];
                }
                mergeDataList.Add(data);
            }
        }

        /// <summary>
        /// 读取Excel文件中的NoIdCheck文件信息
        /// </summary>
        private static void ExcelLoadNoIdCheck(ExcelWorksheet sheetIn)
        {
            int rowLength = sheetIn.Dimension.End.Row;
            int colLength = sheetIn.Dimension.End.Column;

            if (colLength != 1)
            {
                throw new Exception("Error : NoIdCheck 子表填写格式不对");
            }
            for (int row = 2; row < rowLength + 1; row++)
            {
                string noIdCheckFileName = sheetIn.GetValue(row, 1) == null ? "" : sheetIn.GetValue(row,1).ToString().Trim();
                IdRepeatCheck.NoIdCheckFileNameList.Add(noIdCheckFileName);
            }
        }
        private static void ExcelLoadASConfig(ExcelWorksheet sheetIn)
        {
            int rowLength = sheetIn.Dimension.End.Row;
            int colLength = sheetIn.Dimension.End.Column;
            for (int row = 2; row < rowLength + 1; row++)
            {
                var configName = sheetIn.GetValue(row, 1)==null? null : sheetIn.GetValue(row, 1).ToString().Trim();
                var configParent = sheetIn.GetValue(row, 2) == null ? null : sheetIn.GetValue(row, 2).ToString().Trim() ;
                var configChild = sheetIn.GetValue(row, 3) == null ? null : sheetIn.GetValue(row, 3).ToString().Trim();
            }
         }
        //检查合表后的文件名是否已存在config文件夹中
        private static bool CheckMergeExcelExist(string mergeExcelName)
        {
            List<string> files = Utils.GetAllConfigFiles(ParseCommand.ConfigPath, ".xls");
            foreach (var file in files)
            {
                FileInfo fileInfo = new FileInfo(file);
                var fileName = fileInfo.Name.Substring(0, fileInfo.Name.Length - 5);
                if (fileName == mergeExcelName)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
