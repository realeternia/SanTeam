using System;
using System.Collections.Generic;
using System.IO;
using OfficeOpenXml;

namespace Excel2dllCore.Tools
{
    public class ExcelTool
    {
        public static LineData[] ReadHeader(ExcelWorksheet sheetIn, int columnCount)
        {
            var header = new LineData[4];
            for (int i = 1; i < 5; i++)
            {
                var headerData = new LineData();
                headerData.Data = new string[columnCount];
                for (int j = 1; j < columnCount + 1; j++)
                {
                    if (sheetIn.GetValue(i, j) == null)
                        headerData.Data[j - 1] = "";
                    else
                        headerData.Data[j - 1] = sheetIn.GetValue(i, j).ToString().Trim();
                }
                header[i - 1] = headerData;
            }
            return header;
        }

        public static List<LineData> ReadRecord(ExcelWorksheet sheetIn, int columnCount)
        {
            var records = new List<LineData>();
            for (int i = 5; i < sheetIn.Dimension.End.Row + 1; i++)
            {
                string idTxt = sheetIn.GetValue(i, 1) == null ? "" : sheetIn.GetValue(i, 1).ToString();//取到第一列的数据
                if (Utils.IsNullOrWhiteSpace(idTxt)) //第一列为空串，后面就不读了
                {
                    break;
                }

                LineData data = new LineData();
                data.Data = new string[columnCount];

                for (int j = 1; j < columnCount + 1; j++)
                {
                    string c = sheetIn.GetValue(i, j) == null ? "" : sheetIn.GetValue(i, j).ToString();
                    data.Data[j - 1] = c;
                }
                records.Add(data);
            }
            return records;
        }

        public static void UpdateFile(string path, string name, LineData[] header, LineData[] record)
        {
            string localNewGenerateFileName = path + Path.DirectorySeparatorChar + name + ".xlsx";

            FileInfo file = new FileInfo(localNewGenerateFileName);
            ExcelPackage ep = ExcelFileOpener.Open(file, true);
            ExcelWorkbook workbookIn = ep.Workbook;
            ExcelWorksheet sheetIn = null;
            foreach (var excelWorksheet in workbookIn.Worksheets)
            {
                sheetIn = excelWorksheet; //把模板表的数据清空
                sheetIn.DeleteRow(1, sheetIn.Cells.Rows - 5);
                break;
            }
            
            {
                for (int i = 1; i < 5; i++)//写表头
                {
                    for (int j = 1; j < header[i - 1].Data.Length + 1; j++)
                    {
                        sheetIn.SetValue(i, j, header[i - 1].Data[j - 1]);
                    }
                }

                for (int i = 5; i < record.Length + 5; i++)//写数据
                {
                    for (int j = 1; j < record[i - 5].Data.Length + 1; j++)
                    {
                        if (Utils.IsNullOrWhiteSpace(record[i - 5].Data[0]))
                        {
                            continue;
                        }
                        sheetIn.SetValue(i, j, record[i - 5].Data[j - 1]);
                    }
                }
            }
            ep.Save();
            ep.Dispose();

            //Logger.Debug(name + "fix to Upper");
            File.Move(path + Path.DirectorySeparatorChar + name + ".xlsx", file.FullName);
        }

        public static string IntToStringColumn(int colNum)
        {
            const int alphaLen = 26;
            char colIndexFirst = Convert.ToChar(colNum / alphaLen + 'A' - 1);
            char colIndexSecond = Convert.ToChar(colNum % alphaLen + 'A' - 1);
            string colIndexEnd = "";

            if (colNum % 26 == 0)
            {
                colIndexFirst = Convert.ToChar(colNum / alphaLen + 'A' - 1);
                if (colIndexFirst == 'A')
                {
                    colIndexEnd = "Z";
                }
                else
                {
                    colIndexEnd = Convert.ToChar(colNum / alphaLen + 'A' - 2) + "Z";
                }
            }
            else
            {
                if (colIndexFirst >= 'A')//索引由2个英文字母组成.
                {
                    colIndexEnd = colIndexFirst.ToString() + colIndexSecond.ToString();
                }
                else
                {
                    colIndexEnd = colIndexSecond.ToString();
                }
            }
            return colIndexEnd;
        }
    }
}