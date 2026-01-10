using System;
using System.Collections.Generic;
using System.IO;
using Excel2dllCore.Load;
using Excel2dllCore.Tools;
using OfficeOpenXml;

namespace Excel2dllCore.Merge
{
    public class ModelMerger : IMerger
    {
        public string NewFileName { get; set; }         //新表名
        public string StructFileName { get; set; }      //结构表名
        public string ModelFileName { get; set; }       //模板表名 
        public string[] BranchFileList { get; set; }    //子表名（包括了模板表）

        private LineData[] structHeader;
        private List<LineData> record = new List<LineData>(); //保存数据
        private List<string> templateId = new List<string>(); //记录模板表中的Id

        public void Process(string path)
        {
            //处理各个子表
            foreach (var branchFile in BranchFileList)
            {
                if(branchFile=="")
                    continue;
                if (!File.Exists(path + Path.DirectorySeparatorChar + branchFile + ".xlsx"))
                {
                    Logger.Debug(branchFile + " not found. ");
                }

                FileInfo file = new FileInfo(path + Path.DirectorySeparatorChar + branchFile + ".xlsx");
                ExcelPackage ep = ExcelFileOpener.Open(file, false);
                CheckEpFile(ep, branchFile);
                ep.Dispose();
            }

            if (ParseCommand.MemMerge)
            {//直接从内存生成cs文件，中间不产生excel文件
                DataLoader.LoadFromMem(structHeader, record.ToArray(), NewFileName);
            }
            else
            { 
                ExcelTool.UpdateFile(path, NewFileName, structHeader, record.ToArray());//写文件
            }
        }

        public bool Check(string path)
        {
            if (StructFileName != null)
            {
                FileInfo file = new FileInfo(path + Path.DirectorySeparatorChar + StructFileName + ".xlsx");
                ExcelPackage ep = ExcelFileOpener.Open(file, false);
                ExcelWorkbook workbookIn = ep.Workbook;
                foreach (var sheetIn in workbookIn.Worksheets)
                {
                    if (sheetIn.Name[0] == '~')
                    {
                        continue;
                    }

                    structHeader = ExcelTool.ReadHeader(sheetIn, sheetIn.Dimension.End.Column);
                }
                ep.Dispose();
            }
            else
            {
                return false;
            }

            return true;
        }

        private void CheckEpFile(ExcelPackage ep, string branchFile)
        {
            ExcelWorkbook workbookIn = ep.Workbook;

            foreach (var sheetIn in workbookIn.Worksheets)
            {
                if (sheetIn.Name[0] == '~')
                {
                    continue;
                }

                //Logger.Debug(string.Format(" Model Merge File {0}/{1}", branchFile, sheetIn));
                if (sheetIn.Dimension == null)
                {
                    continue;
                }

                int branchCount = sheetIn.Dimension.End.Column; //sheet的列数
                for (int j = 1; j < sheetIn.Dimension.End.Column + 1; ++j) //通过读Id行来确定实际的列数
                {
                    if (sheetIn.GetValue(2, j) == null)
                    {
                        branchCount = j - 1;
                        break;
                    }
                }
                LineData[] branchHeader = ExcelTool.ReadHeader(sheetIn, branchCount);

                //循环读入每行子表数据
                for (int i = 5; i < sheetIn.Dimension.End.Row + 1; i++)
                {
                    string idTxt = sheetIn.GetValue(i, 1) == null ? "" : sheetIn.GetValue(i, 1).ToString(); //取到第一列的数据
                    if (Utils.IsNullOrWhiteSpace(idTxt)) //第一列为空串，后面就不读了
                    {
                        break;
                    }

                    LineData data = new LineData();
                    data.Data = new string[structHeader[0].Data.Length];

                    for (int j = 1; j < branchHeader[0].Data.Length + 1; j++)
                    {
                        var cell = sheetIn.GetValue(i, j);
                        string c = cell == null ? "" : cell.ToString();

                        for (int k = 0; k < structHeader[0].Data.Length; k++)
                        {
                            if (structHeader[1].Data[k] == branchHeader[1].Data[j - 1])
                            {
                                data.Data[k] = c;
                            }
                        }
                    }

                    if (branchFile == ModelFileName) //如果该表是模板表（其实循环的第一次必是模板表）
                    {
                        record.Add(data);
                        templateId.Add(data.Data[0]);
                    }
                    else
                    {
                        if (templateId.Contains(data.Data[0])) //如存在，则覆盖Record中的数据
                        {
                            foreach (var rowData in record)
                            {
                                if (rowData.Data[0] == data.Data[0]) //如果id相同则覆盖过去
                                {
                                    for (int k = 0; k < rowData.Data.Length; k++)
                                    {
                                        if (!Utils.IsNullOrWhiteSpace(data.Data[k]))
                                            rowData.Data[k] = data.Data[k]; //覆盖Record中的数据
                                    }
                                    break;
                                }
                            }
                            templateId.Remove(data.Data[0]);
                        }
                        else
                        {
                            record.Add(data); //即使是子表中数据重复，也先添加进去待后面检查
                            Logger.Warn(string.Format("{0}.xlsx中存在模板表中没有的数据行Id：{1}", branchFile, data.Data[0]));
                        }
                    }
                }
            }
        }
    }
}
