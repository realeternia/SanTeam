using System;
using System.Collections.Generic;
using Excel2dllCore.Tools;

namespace Excel2dllCore.Check
{
    class CheckColNameRepeat
    {
        public static void CheckColNameRepeatInFile(LineData[] headers, string fileName, int index)
        {            
            if (index == 1)
            {
                List<string> colNameList = new List<string>();
                var colCount = headers[0].Data.Length;

                for (int col = 0; col < colCount; col++)
                {
                    // 每个sheet都有固定的前4行，首先将前两行取出   
                    string description= headers[0].Data[col];
                    string headerName = headers[1].Data[col];
                    if (description == "")
                        break;
                    if (headerName == "")
                        break;
                    if (headerName.StartsWith("~")) //以"~"开头的列需要过滤
                        continue;

                    var isString = headers[2].Data[col].Contains("string");
                    var isMultiLan = headers[2].Data[col].Contains("lan");
                    if (colNameList.Contains(headerName) && !(isString && isMultiLan))//todo 先这样猥琐 应该是判断名字+类型
                    {
                        Logger.Error(String.Format("{0}表中有多列列名称为{1}，重复 ", fileName, headerName));
                    }
                    else
                    {
                        colNameList.Add(headerName);
                    }
                }
            }
        }
    }
}
