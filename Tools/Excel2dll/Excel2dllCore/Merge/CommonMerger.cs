using System;
using System.Collections.Generic;
using System.IO;
using Excel2dllCore.Load;
using Excel2dllCore.Tools;
using OfficeOpenXml;

namespace Excel2dllCore.Merge
{
    public class CommonMerger : IMerger
    {
        public string NewFileName { get; set; }      //合并后的新文件
        public int ColumnCount { get; set; }         //列数
        public string[] BranchFileList { get; set; } //子文件列表
        public LineData[] Header { get; set; }       //第一个子文件的表头
        private readonly List<LineData> record = new List<LineData>();  //所有子文件数据先保存在这里
        public void Process(string path)
        {
            // Logger.Debug("Begin Merge -> {0}", ClassName);
            foreach (var branchFile in BranchFileList)
            {
                FileInfo file = new FileInfo(path + Path.DirectorySeparatorChar + branchFile + ".xlsx");
                ExcelPackage ep = ExcelFileOpener.Open(file, false);
                ExcelWorkbook workbookIn = ep.Workbook;
                foreach (var sheetIn in workbookIn.Worksheets)
                {
                    if (sheetIn.Name[0] == '~')
                    {
                        continue;
                    }

                    //Logger.Debug(string.Format(" Merge File {0}/{1}", branchFile, sheetIn));
                    if (sheetIn.Dimension == null)
                    {
                        continue;
                    }
                    //数据读出来
                    record.AddRange(ExcelTool.ReadRecord(sheetIn, ColumnCount));
                }
                ep.Dispose();
            }

            if (ParseCommand.MemMerge)
            {//直接从内存生成cs文件，中间不产生excel文件
                DataLoader.LoadFromMem(Header, record.ToArray(), NewFileName);
            }
            else
            {
                var srcPath = path + Path.DirectorySeparatorChar + BranchFileList[0] + ".xlsx";
                string destPath = path + Path.DirectorySeparatorChar + NewFileName + ".xlsx";
                File.Copy(srcPath, destPath, true);
                ExcelTool.UpdateFile(path, NewFileName, Header, record.ToArray());//写文件    
            }
        }

        //检查Merge中合表的前n列表头是否相同
        public bool Check(string path)
        {
            bool flag = true;
            foreach (var branchFile in BranchFileList)
            {
                FileInfo fileInfo = new FileInfo(path + Path.DirectorySeparatorChar + branchFile + ".xlsx");
                ExcelPackage ep = ExcelFileOpener.Open(fileInfo, false);
                if (!CheckEpFile(ep, branchFile))
                {
                    flag = false;
                    break;
                }
                ep.Dispose();
            }
            return flag;
        }

        private bool CheckEpFile(ExcelPackage ep, string branchFile)
        {
            ExcelWorkbook workbookIn = ep.Workbook;
            foreach (var sheetIn in workbookIn.Worksheets)
            {
                if (sheetIn.Name[0] == '~')
                {
                    continue;
                }
                if (sheetIn.Dimension == null)
                {
                    continue;
                }

                ColumnCount = Math.Min(ColumnCount, sheetIn.Dimension.End.Column);

                if (Header == null)
                {
                    Header = ExcelTool.ReadHeader(sheetIn, ColumnCount); //check时候顺便把header的结构也解析出来了
                }
                else
                {
                    for (int i = 2; i < 5; i++) //第一列是描述 ，就算了
                    {
                        for (int j = 1; j < ColumnCount + 1; j++)
                        {
                            //与 保存在lastHeader中的上个文件的前ColumnCount列的表头 进行比较
                            if (sheetIn.GetValue(i, j) == null || sheetIn.GetValue(i, j).ToString().ToLower() != Header[i - 1].Data[j - 1].ToLower())
                            {
                                Logger.Error(string.Format(@"ERROR: Merge Check {0}/{1}和 表头 ({2},{3}) 不一致", branchFile,
                                    sheetIn, sheetIn.GetValue(1, j), sheetIn.GetValue(i, j)));
                                return false;
                            }
                        }
                    }
                }
            }
            return true;
        }
    }
}
