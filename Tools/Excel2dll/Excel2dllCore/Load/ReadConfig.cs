using Excel2dllCore.Tools;
using System;
using System.Collections.Generic;
using System.IO;

namespace Excel2dllCore.Load
{
    public abstract class ReadConfig
    {
        internal readonly List<CellType> Types = new List<CellType>();    // 表头
        internal readonly List<Record> Records = new List<Record>();      // 表数据

        private Dictionary<string, CellType> typeDic = new Dictionary<string, CellType>();//建一个缓存

        /// <summary>
        /// 读取给定的配置文件，将其读取到types和records表中
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public bool ProcessFile(FileInfo fileInfo, string fileName)
        {
            LineData[] headerData;
            List<LineData> recordData;
            FormatContent(fileInfo, fileName, out headerData, out recordData);
            LoadDataToMem(fileName, "sheet1", headerData, recordData);
            return Types.Count > 0;
        }

        /// <summary>
        /// 加载给定的配置文件的Id列
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public bool LoadIdCol(FileInfo fileInfo, string fileName)
        {
            LineData[] headerData;
            List<LineData> recordData;
            FormatContent(fileInfo, fileName, out headerData, out recordData, true);
            LoadTypesAndRecords(fileName, "sheet1", headerData, recordData);
            return IsConfigValid();
        }

        /// <summary>
        /// 把数据加载到内存
        /// </summary>
        /// <param name="fileName">配置文件名</param>
        /// <param name="sheetName">表名</param>
        /// <param name="headerData">表头内容</param>
        /// <param name="recordData">数据内容</param>
        public void LoadDataToMem(string fileName, string sheetName, LineData[] headerData, List<LineData> recordData)
        {
            LoadTypesAndRecords(fileName, sheetName, headerData, recordData);

            CheckerDataType checkerData = new CheckerDataType();
            checkerData.FileName = fileName;
            checkerData.Index = 1;
            checkerData.Headers = headerData;
            checkerData.Records = recordData;
            Global.CheckerDataList.Add(checkerData);
        }

        public abstract void FormatContent(FileInfo fileInfo, string fileName, out LineData[] header, out List<LineData> record, bool idOnly = false);

        /// <summary>
        /// 把把类型信息、数据读入内存
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="sheetName"></param>
        /// <param name="headerData"></param>
        /// <param name="recordData"></param>
        private void LoadTypesAndRecords(string fileName, string sheetName, LineData[] headerData, List<LineData> recordData)
        {
            ProcessSheetDataToTypes(fileName, headerData, sheetName);//把类型信息读入内存
            ProcessSheetDataToRecords(headerData, recordData, fileName);//把数据读入内存
        }

        /// <summary>
        /// 配置表数据是否有效
        /// </summary>
        /// <returns></returns>
        private bool IsConfigValid()
        {
            if (Types.Count == 0 || Records.Count == 0)
            {
                Logger.Debug("\tFailed: No types or records.");
                return false;
            }
            return true;
        }

        /// <summary>
        /// 读取表头内容，并存入Types中
        /// </summary>
        /// <param name="fileName">配置文件名</param>
        /// <param name="headerData">表头</param>
        /// <param name="sheetName">表名</param>
        private void ProcessSheetDataToTypes(string fileName, LineData[] headerData, string sheetName)
        {
            var colCount = headerData[0].Data.Length;
            int meaningColumnCount = 0; //有效列数

            for (int col = 1; col <= colCount; col++)
            {
                // 每个sheet都有固定的前4行，首先将其取出
                string val1 = headerData[0].Data[col - 1];
                string val2 = headerData[1].Data[col - 1];
                string val3 = headerData[2].Data[col - 1];
                string val4 = headerData[3].Data[col - 1];

                if (val1 == "") break;
                if (val2 == "") break;

                if (val2.StartsWith("~"))
                    continue;

                if (val3 == "")
                {
                    throw new Exception(string.Format("ERROR: 检查到{0}/{1}：(3，{2}) 表头为空", fileName, sheetName, col));
                }

                if (val4 == "")
                {
                    throw new Exception(string.Format("ERROR: 检查到{0}/{1}：(4，{2}) 表头为空", fileName, sheetName, col));
                }

                CellType type = new CellType
                {
                    Desc = val1.Trim(),
                    FieldName = val2.Trim(),
                    Type = val3.Trim(),
                    CS = val4.ToString().Contains("|") ? val4.ToString().Trim().Split('|')[0] : val4.ToString().Trim(),
                    BuildIndex = val4.ToString().Contains("index")
                };

                if (CellType.IsIgnoreType(type)) //过滤与 ParseCommand.LocalLanguage 不一致的属性
                {
                    continue;
                }

                meaningColumnCount++;

//                if (meaningColumnCount == 1 && fileName != "GameConfig") //第一张表的第一列必须是id
//                {
//                    if (type.FieldName.ToLower() != "id")
//                    {
//                        throw new Exception(string.Format("ERROR: 配置表{0}错误，第一列必须是Id", fileName));
//                    }
//                }
                Types.Add(type);
                typeDic[type.FieldName + col] = type;
            }
        }

        /// <summary>
        /// 读取数据内容，并存入Records中
        /// </summary>
        /// <param name="headerData">表头内容</param>
        /// <param name="recordData">记录内容</param>
        /// <param name="fileName">文件名</param>
        private void ProcessSheetDataToRecords(LineData[] headerData, List<LineData> recordData, string fileName)
        {
            if (recordData.Count <= 0)
            {
                return;
            }

            List<string> ignoreList = new List<string>();//用来记录有多少负数id列，这些列会被忽略
            Logger.Debug(string.Format("\t数据预处理 {0}", fileName));

            var colCount = recordData[0].Data.Length;
            bool hasAlias = headerData[1].Data.Length >= 2 && headerData[1].Data[1] == "Alias";

            for (int row = 1; row <= recordData.Count; row++)
            {
                Record record = new Record();
                string idVal = recordData[row - 1].Data[0];
                if (idVal == "") break;
                string id = idVal.Trim();

                if (hasAlias)
                {
                    var idConstName = recordData[row - 1].Data[1];
                    if (!string.IsNullOrEmpty(idConstName))
                    {
                        record.ConstantName = idConstName;
                    }
                }

                for (int col = 1; col <= colCount; col++)
                {
                    string val2 = headerData[1].Data[col - 1];//取到类型的信息
                    string name = val2.Trim();
                    CellType type;
                    if (!typeDic.TryGetValue(name + col, out type))
                        continue;
                    if (CellType.IsIgnoreType(type))  //以"~"开头的列需要过滤
                        continue;

                    string value = "";

                    object val = recordData[row - 1].Data[col - 1];
                    if (val != null)
                    {
                        char[] charsToTrim = { '\r', '\n' };
                        if (type.Type.ToLower().Contains("string"))
                        {
                            value = val.ToString().Trim(charsToTrim);
                            value = value.TrimEnd();
                        }
                        else
                        {
                            value = val.ToString().Trim();
                        }
                    }

                    //为了处理类似36000008|InitialPotion的
                    if (col == 1)
                    {
                        value = id;
                    }

                    if (fileName != "GameConfig")//todo 先ws下了，绕过这张表
                    {
                        //数据类型转化成C#代码的写法
                        if (!RegisterType.HasType(type.Type))
                        {
                            throw new Exception("ERROR: 不支持的自定义类型 " + fileName + " " + type.Type);
                        }
                        if (RegisterType.ProcessValue(type.Type, ref value) == false)
                        {
                            string colIndexString = ExcelTool.IntToStringColumn(col);
                            Logger.Error(string.Format("ERROR: 文件{0} 第{1}行 第{2}列 输入数据类型不对", fileName, row + 4, colIndexString));
                        }
                    }

                    CellValue v = new CellValue { Type = type, Value = value };
                    record.Values.Add(v);
                }
                record.Id = id;
                Records.Add(record);
            }

            //if (ignoreList.Count > 0)
            //{
            //    //Logger.Debug("\n id " + XlsxUtils.Join(",", ignoreList) + " are ignored.");
            //}
        }

    }
}
