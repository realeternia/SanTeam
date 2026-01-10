using System;
using System.Collections.Generic;
using System.Net;
using Excel2dllCore.Tools;

namespace Excel2dllCore.Check
{
    public class CheckerInfo
    {
        //得到检查函数的参数
        public static string[] CheckerArgs(string ruleText, int col, string fileName, int index, LineData[] headers,ref Dictionary<int, int> refOtherCol)
        {
            string splText = (ruleText.Substring(ruleText.IndexOf('(') + 1, ruleText.IndexOf(')') - ruleText.IndexOf('(') - 1));
            string[] mdata;
            if (splText == "")
            {
                mdata = new string[0];
            }
            else if (!splText.Contains(","))
            {
                mdata = new string[] { splText };
            }
            else
            {
                mdata = splText.Length > 1 ? splText.Split(',') : new string[0];
            }
            for (int i = 0; i < mdata.Length; i++)
            {
                if (mdata[i].StartsWith("$"))
                {
                    string member = mdata[i].Substring(1);
                    int colNum = 0;
                    for (int k = 1; k <= headers[0].Data.Length; k++)
                    {
                        if (member == headers[1].Data[k - 1])
                        {
                            colNum = k;
                            break;
                        }
                    }
                    if (colNum == 0)
                    {
                        string colIndexString = ExcelTool.IntToStringColumn(col);
                        var line = String.Format("{0}表 {1}分页 {2}列 未通过规则 {3}, 未找到{4}，填错了检查语句？", fileName, index, colIndexString, ruleText, member);
                        Logger.Error(line);
                    }
                    else
                    {
                        refOtherCol.Add(i,colNum);
                    }
                }
            }
            return mdata;
        }

        //根据检查规则rule，由Excel单元格数据得到检查数据
        public static List<string> GetCheckDataList(string rule, string cellData, string headerType)
        {
            List<string> checkerText = new List<string>();
            string cellText;

            if (cellData == "")
            {
                return null;
            }
            else
            {
                char[] charsToTrim = { '\r', '\n' };
                if (headerType.ToLower() == "string" || headerType.ToLower() == "string[]")
                {
                    cellText = cellData.Trim(charsToTrim);
                    cellText = cellText.TrimEnd().Split('|')[0];
                }
                else
                {
                    cellText = cellData.Trim().Split('|')[0];
                }
            }

            if (!headerType.Contains("[]"))
            {
                checkerText.Add(cellText);
            }
            else
            {
                if (rule.StartsWith("ArrayLength")) //这个是统计数组长度的函数要特殊处理
                {
                    checkerText.Add(cellText);
                }
                else
                {
                    string[] singleVal = cellText.Split(',');
                    for (int i = 0; i < singleVal.Length; i++)
                    {
                        checkerText.Add(singleVal[i]);
                    }
                }
            }
            return checkerText;
        }

        //检验函数的参数取其他列数据
        public static bool CheckerRefArgs(int row, Checker checker, List<LineData> records)
        {
            bool noCheck = false;
            if (checker.Vals == null || checker.RefOtherCol == null)
            {
                return false;
            }
            for (int num = 0; num < checker.Vals.Length; num++)
            {
                int colNum;
                if (checker.RefOtherCol.TryGetValue(num, out colNum))
                {
                    string obj = records[row - 1].Data[colNum - 1];
                    if (obj == "")
                    {
                        if (checker.Rule.Contains("NullAs") || checker.Rule.Contains("StringLike") || checker.Rule.Contains("ArrayLength"))
                        {
                            checker.Vals[num] = "";
                        }
                        else
                        {
                            noCheck = true;
                            break;
                        }
                    }
                    else
                    {
                        checker.Vals[num] = obj.Trim(); //按上面多判断两步
                    }
                }
            }
            return noCheck;
        }

    }
}
