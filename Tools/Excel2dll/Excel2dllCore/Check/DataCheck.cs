using System;
using System.Collections.Generic;
using Excel2dllCore.Tools;


namespace Excel2dllCore.Check
{
    public class DataCheck
    {
        public static bool DataExistCheck = true;
        public static Dictionary<string, int> ItemNoDataExist = new Dictionary<string, int>(10000); //检查是否遗漏跨表检查
        private static Dictionary<string, HashSet<string>> idConstantNameDict = new Dictionary<string, HashSet<string>>(); //记录所有表的id别名，用于同表id别名重复检查

        public static void ProcessAll()
        {
            using (new UXAutoStopWatch())
            {
                Logger.Debug(string.Format("开始数据函数检查，总数={0}", Global.CheckerDataList.Count));
                foreach (var checkerData in Global.CheckerDataList)
                {
                    DataChecker(checkerData.Headers, checkerData.Records, checkerData.FileName, checkerData.Index);
                }
                Logger.Debug(string.Format("数据函数检查完成，总数={0}", Global.CheckerDataList.Count));
            }
            foreach (var item in ItemNoDataExist)
            {
                Logger.Warn(item.Key + "字段含有疑似Id内容，但是却没有跨表检查，这种情况出现" + item.Value + "次");
            }
        }

        public static void DataChecker(LineData[] headers, List<LineData> records, string fileName, int index)
        {
          //  CheckInLuaKeys.CheckInLuaKeysResult(headers, records, fileName, index);//检查列名称和行实例别名是否在lua关键词列表中
            CheckColNameRepeat.CheckColNameRepeatInFile(headers, fileName, index);//检查同表中是否有列名称重复

            HashSet<string> idSetLocal = new HashSet<string>();//用来检查本表id是否有重复
            HashSet<string> idConstNameSet = new HashSet<string>();//用来检查同表id别名是否重复

            var colCount = headers[0].Data.Length;
            var rowCount = records.Count;
            Dictionary<int, List<Checker>> checkerDict = GetCheckerDict(headers, fileName, index);

            bool hasAlias = headers.Length >= 2 && headers[1].Data[1] == "Alias";

            for (int row = 1; row <= rowCount; row++)
            {
                string idVal = records[row - 1].Data[0];
                if (idVal == "") break;
                string id = idVal.Trim();

                if (hasAlias)
                {                    
                    var idConstName = records[row - 1].Data[1];
                    if (!string.IsNullOrEmpty(idConstName))
                    {
                        if (!IdRepeatCheck.IdConstNameRepeat(idConstName, fileName, index, row, ref idConstNameSet))
                            continue;
                    }
                }

                if (headers[1].Data[0].ToLower() == "id")
                {
                    if (!IdRepeatCheck.IdRepeat(id, fileName, index, row, ref idSetLocal))//Id重复检查
                    {
                        continue;
                    }
                }

                for (var col = 1; col <= colCount; col++)
                {
                    List<Checker> checkerList;
                    if(!IdRepeatCheck.NoIdCheckFileNameList.Contains(fileName))
                        CheckCrossFile(row, col, headers, records, fileName);
                    if (!checkerDict.TryGetValue(col, out checkerList))
                    {
                        continue;
                    }
                    try
                    {
                        foreach (var checker in checkerList)
                        {
                            string headerType = headers[2].Data[col - 1];
                            string cellData = records[row - 1].Data[col - 1];
                            var dataList = CheckerInfo.GetCheckDataList(checker.Rule, cellData, headerType);
                            string checkerString = checker.Rule.Substring(0, checker.Rule.IndexOf('('));
                            if (dataList == null && checkerString != "ArrayLength")
                            {
                                continue;
                            }
                            else if (dataList == null)
                            {
                                dataList = new List<string>();
                                dataList.Add("");
                            }

                            if (CheckerInfo.CheckerRefArgs(row, checker, records))
                            {
                                continue;
                            }
                            foreach (var checkData in dataList)
                            {
                                if (!checker.Check(checkData))
                                {
                                    string colIndexString = ExcelTool.IntToStringColumn(col);
                                    var line = String.Format("{0}表 {1}分页 {2}行 {3}列 {4}值未通过规则 {5}", fileName, index, row + 4, colIndexString, checkData, checker.Rule);
                                    Logger.Error(line);
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        string colIndexString = ExcelTool.IntToStringColumn(col);
                        var line = String.Format("{0}表 {1}分页 {2}行 {3}列 值未通过规则 ", fileName, index, row + 4, colIndexString);
                        Logger.Error(line);
                        Logger.Error(e.Message);
                    }
                }
            }

            if (idConstNameSet.Count == 0)
            {
                return;
            }
            if (idConstantNameDict.ContainsKey(fileName))
            {
                var existIdConstNameSet = new HashSet<string>();
                idConstantNameDict.TryGetValue(fileName, out existIdConstNameSet);
                foreach (var idConstName in idConstNameSet)
                {
                    if (!existIdConstNameSet.Add(idConstName))
                    {
                        Logger.Error(string.Format("ERROR: {0}文件 {1}分页 与本表其他分页id别名重复 id别名{2}", fileName, index, idConstName));
                    }
                }
                idConstantNameDict[fileName] = existIdConstNameSet;
            }
            else
            {
                idConstantNameDict.Add(fileName,idConstNameSet);
            }
        }

        public static Dictionary<int, List<Checker>> GetCheckerDict(LineData[] headers, string fileName, int index)
        {
            Dictionary<int, List<Checker>> checkerDict = new Dictionary<int, List<Checker>>();

            var colCount = headers[0].Data.Length;
            for (int col = 1; col <= colCount; col++)
            {
                string val2 = headers[1].Data[col - 1];
                string val4 = headers[3].Data[col - 1];

                if (val2.StartsWith("~"))
                {
                    continue;
                }

                string originRule = val4.Contains("|") ? val4.Substring(val4.IndexOf('|') + 1) : "";
                string[] rules = originRule.Split('|');

                List<Checker> checkerList = new List<Checker>();
                foreach (var rule in rules)
                {
                    if (rule == "")
                    {
                        continue;
                    }

                    string ruleText = RulePreprocessor.CheckText(rule);
                    if (!ruleText.Contains("("))
                    {
                        Logger.Warn(String.Format("{0} Rule {1} Not Implement", fileName, ruleText));
                        Logger.Warn("是否填错了配置表检查语句？");
                        break;
                    }

                    var refOtherCol = new Dictionary<int, int>();
                    string[] checkerArgs = CheckerInfo.CheckerArgs(ruleText, col, fileName, index, headers,ref refOtherCol);
                    Checker cellChecker = null;

                    string checkerRule = ruleText.Substring(0, ruleText.IndexOf('('));
                    switch (checkerRule)
                    {
                        case "Between":
                            cellChecker = new BetweenChecker(checkerArgs);
                            break;
                        case "BiggerThan":
                            cellChecker = new BiggerThanChecker(checkerArgs);
                            break;
                        case "BiggerEqualThan":
                            cellChecker = new BiggerEqualThanChecker(checkerArgs);
                            break;
                        case "SmallerThan":
                            cellChecker = new SmallerThanChecker(checkerArgs);
                            break;
                        case "SmallerEqualThan":
                            cellChecker = new SmallerEqualThanChecker(checkerArgs);
                            break;
                        case "Equal":
                            cellChecker = new EqualChecker(checkerArgs);
                            break;
                        case "ArrayLength":
                            cellChecker = new ArrayLengthChecker(checkerArgs);
                            break;
                        case "StringNotEmpty":
                            cellChecker = new StringNotEmptyChecker();
                            break;
                        case "ArrayLengthEqual":
                            cellChecker = new ArrayLengthEqualChecker(checkerArgs);
                            break;
                        case "ArrayLengthBiggerThan":
                            cellChecker = new ArrayLengthBiggerThanChecker(checkerArgs);
                            break;
                        case "ArrayLengthSmallerThan":
                            cellChecker = new ArrayLengthSmallerThanChecker(checkerArgs);
                            break;
                        case "ArraySmallerEqual":
                            cellChecker = new ArraySmallerEqualChecker(checkerArgs);
                            break;
                        case "StringLengthEqual":
                            cellChecker = new StringLengthEqualChecker(checkerArgs);
                            break;
                        case "StringLengthBiggerThan":
                            cellChecker = new StringLengthBiggerThanChecker(checkerArgs);
                            break;
                        case "StringLengthSmallerThan":
                            cellChecker = new StringLengthSmallerThanChecker(checkerArgs);
                            break;
                        case "StringLike":
                            cellChecker = new StringLikeChecker(checkerArgs);
                            break;
                        case "DataExist":
                            cellChecker = new DataExistChecker(checkerArgs);
                            break;
                        case "NullAs":
                            cellChecker = new NullAsChecker(checkerArgs);
                            break;
                        default:
                            Logger.Error(fileName + " Rule " + ruleText + " Not Implement");
                            Logger.Error("是否填错了配置表检查语句？");
                            break;
                    }
                    if (cellChecker != null)
                    {
                        if (refOtherCol.Count != 0)
                        {
                            cellChecker.RefOtherCol = refOtherCol;
                        }
                        checkerList.Add(cellChecker);
                    }
                }
                if (checkerList.Count != 0)
                {
                    checkerDict.Add(col, checkerList);
                }                
            }
            return checkerDict;
        }

        //检查是否遗漏跨表检查
        public static void CheckCrossFile(int row, int col, LineData[] headers, List<LineData> records,
            string fileName)
        {
            string headerName = headers[1].Data[col - 1];
            string headerType = headers[2].Data[col - 1];
            string headerCs = headers[3].Data[col - 1];

            string originRule = headerCs.Contains("|") ? headerCs.Substring(headerCs.IndexOf('|') + 1) : "";
            string cellText = records[row - 1].Data[col - 1].Trim().Split('|')[0];

            if (headerType == "uint[]" && !originRule.Contains("DataExist"))
            {
                foreach (var o in cellText.Split(','))
                {
                    if (o.Length == 8)
                    {
                        string key = fileName + "表格的" + headerName;
                        if (ItemNoDataExist.ContainsKey(key))
                        {
                            ItemNoDataExist[key]++;
                        }
                        else
                        {
                            ItemNoDataExist.Add(key, 1);
                        }
                    }
                }

            }
        }
    }
}
