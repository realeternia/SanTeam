using Excel2dllCore.Tools;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;

namespace Excel2dllCore.Load
{
    public class ReadExcel : ReadConfig
    {
        private List<CellType> usefulHeader = new List<CellType>();

        public override void FormatContent(FileInfo fileInfo, string fileName, out LineData[] header, out List<LineData> record, bool idOnly = false)
        {
            using (ExcelPackage ep = ExcelFileOpener.Open(fileInfo, false))
            {
                var workbook = ep.Workbook;
                var colCount = idOnly ? 1 : workbook.Worksheets[1].Dimension.End.Column;
                record = LoadFirstSheet(workbook.Worksheets[1], fileName, colCount, out header);

                for (int i = 2; i <= workbook.Worksheets.Count; i++)
                {
                    var sheetIn = workbook.Worksheets[i];
                    if (sheetIn.Name.StartsWith("~") || sheetIn.Dimension == null)
                        continue;
                    record.AddRange(LoadOtherSheet(sheetIn, fileName, colCount, i));
                }
            }
        }

        private List<LineData> LoadFirstSheet(ExcelWorksheet sheetIn, string fileName, int columnCount, out LineData[] header)
        {
            if (sheetIn.Name.StartsWith("~") || sheetIn.Dimension == null)
                throw new Exception(string.Format("ERROR: 配置表{0}的第一个Sheet名不能以~开头，且不能为空", fileName));
            header = ExcelTool.ReadHeader(sheetIn, columnCount);
            for (int col = 1; col <= header[0].Data.Length; col++)
            {
                // 每个sheet都有固定的前4行，首先将其取出
                string val1 = header[0].Data[col - 1].Trim();   // Desc
                string val2 = header[1].Data[col - 1].Trim();   // Name
                string val3 = header[2].Data[col - 1].Trim();   // Type
                string val4 = header[3].Data[col - 1].Trim();   // CS

                if (val1 == "") break;
                if (val2 == "") break;

                if (val2.StartsWith("~"))
                    continue;
                if (val3 == "")
                {
                    throw new Exception(string.Format("ERROR: 检查到{0}/{1}：(3，{2}) 表头为空", fileName, sheetIn.Name, col));
                }
                if (val4 == "")
                {
                    throw new Exception(string.Format("ERROR: 检查到{0}/{1}：(4，{2}) 表头为空", fileName, sheetIn.Name, col));
                }

                CellType type = new CellType
                {
                    Desc = val1,
                    FieldName = val2,
                    Type = val3,
                    CS = val4.Split('|')[0].ToLower(),
                };

                if (CellType.IsIgnoreType(type)) //过滤与 ParseCommand.LocalLanguage 不一致的属性
                    continue;

                if (usefulHeader.Count == 0 && fileName != "GameConfig") //第一张表的第一列必须是id
                {
                    if (type.FieldName.ToLower() != "id")
                    {
                        throw new Exception(string.Format("ERROR: 配置表{0}错误，第一列必须是Id，cs", fileName));
                    }

                    type.FieldName = header[1].Data[col - 1] = "Id";
                    type.Type = header[2].Data[col - 1] = "int";
                }
                usefulHeader.Add(type);
            }

            return ExcelTool.ReadRecord(sheetIn, sheetIn.Dimension.End.Column);
        }

        private List<LineData> LoadOtherSheet(ExcelWorksheet sheetIn, string fileName, int columnCount, int index)
        {
            var headerData = ExcelTool.ReadHeader(sheetIn, columnCount);
            int meaningColumnCount = 0; // 有效列数
            for (int col = 1; col <= headerData[0].Data.Length; col++)
            {
                string val1 = headerData[0].Data[col - 1].Trim();   // Desc
                string val2 = headerData[1].Data[col - 1].Trim();   // Name
                string val3 = headerData[2].Data[col - 1].Trim();
                string val4 = headerData[3].Data[col - 1].Trim();

                if (val1 == "") break;
                if (val2 == "") break;

                if (val2.StartsWith("~"))
                    continue;
                if (val3 == "")
                {
                    throw new Exception(string.Format("ERROR: 检查到{0}/{1}：(3，{2}) 表头为空", fileName, sheetIn.Name, col));
                }
                if (val4 == "")
                {
                    throw new Exception(string.Format("ERROR: 检查到{0}/{1}：(4，{2}) 表头为空", fileName, sheetIn.Name, col));
                }

                CellType type = new CellType
                {
                    Desc = val1,
                    FieldName = val2,
                    Type = val3,
                    CS = val4.Split('|')[0],
                };

                if (CellType.IsIgnoreType(type)) //过滤与 ParseCommand.LocalLanguage 不一致的属性
                    continue;

                meaningColumnCount++;

                if (meaningColumnCount > usefulHeader.Count) //多出列了
                {
                    throw new Exception(string.Format("ERROR: 配置表{0}分页{1}类型表和分页1，表头列数不匹配", fileName, index));
                }
                if (type.FieldName != usefulHeader[meaningColumnCount - 1].FieldName)
                {
                    throw new Exception(string.Format("ERROR: 配置表{0}分页{1}类型表和分页1，表头列{2}不匹配，{3}-{4}", fileName, index, col, type.FieldName,
                        usefulHeader[meaningColumnCount - 1].FieldName));
                }
                if (type.Type != usefulHeader[meaningColumnCount - 1].Type)
                {
                    throw new Exception(string.Format("ERROR: 配置表{0}分页{1}类型表和分页1，表头列{2}不匹配，{3}-{4}", fileName, index, col, type.Type,
                        usefulHeader[meaningColumnCount - 1].Type));
                }
                if (type.CS != usefulHeader[meaningColumnCount - 1].CS)
                {
                    throw new Exception(string.Format("ERROR: 配置表{0}分页{1}类型表和分页1，表头列{2}不匹配，{3}-{4}", fileName, index, col, type.CS,
                        usefulHeader[meaningColumnCount - 1].CS));
                }
            }
            if (meaningColumnCount != usefulHeader.Count) // 分页和第一页的列数不匹配
            {
                throw new Exception(string.Format("ERROR: 检查到{0}/{1}：表头和第一分页表头不匹配", fileName, sheetIn.Name));
            }
            return ExcelTool.ReadRecord(sheetIn, sheetIn.Dimension.End.Column);
        }

    }
}
