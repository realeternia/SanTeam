using System;
using System.Collections.Generic;
using System.IO;
using Excel2dllCore.Load;
using Excel2dllCore.Tools;

namespace Excel2dllCore.Merge
{
    public class CfgMerger : IMerger
    {
        public string NewFileName { get; set; }      //合并后的新文件
        public int ColumnCount;        //列数
        public string[] BranchFileList { get; set; } //子文件列表
        public LineData[] Header { get; set; }      //第一个子文件的表头
        private string fileName1 = "";
        private readonly List<LineData> record = new List<LineData>();  //所有子文件数据先保存在这里

        public void Process(string path)
        {
            foreach (var branchFile in BranchFileList)
            {
                FileInfo file = new FileInfo(path + Path.DirectorySeparatorChar + branchFile + ".cfg");
                //数据读出来
                record.AddRange(CfgTool.ReadRecord(file, ColumnCount));
            }

            if (ParseCommand.MemMerge)
            {
                //直接从内存生成cs文件，中间不产生excel文件
                DataLoader.LoadFromMem(Header, record.ToArray(), NewFileName);
            }
            else
            {
                var srcPath = path + Path.DirectorySeparatorChar + BranchFileList[0] + ".cfg";
                string destPath = path + Path.DirectorySeparatorChar + NewFileName + ".cfg";
                CfgTool.UpdateFile(path, NewFileName, Header, record.ToArray());    //写文件    
            }
        }

        //检查Merge中合表的前n列表头是否相同
        public bool Check(string path)
        {
            bool flag = true;
            foreach (var branchFile in BranchFileList)
            {
                FileInfo fileInfo = new FileInfo(path + Path.DirectorySeparatorChar + branchFile + ".cfg");
                if (!CheckCfgFile(fileInfo, branchFile))
                {
                    flag = false;
                    break;
                }
            }
            return flag;
        }

        private bool CheckCfgFile(FileInfo fileInfo, string fileName)
        {
            if (Header == null)
            {
                fileName1 = fileName;
                Header = CfgTool.ReadHeader(fileInfo, fileName, ColumnCount);
            }
            else
            {
                var header2 = CfgTool.ReadHeader(fileInfo, fileName, ColumnCount);
                ColumnCount = Math.Min(ColumnCount, header2[0].Data.Length);
                for (int i = 2; i < 5; i++) //第一列是描述 ，就算了
                {
                    for (int j = 1; j < ColumnCount + 1; j++)
                    {
                        if (Header[i - 1].Data[j - 1].ToLower() != header2[i - 1].Data[j - 1].ToLower())
                        {
                            Logger.Error(string.Format(@"ERROR: Merge Check {0}/{1}和 表头 ({2},{3}) 不一致", fileName, fileName1, Header[i - 1].Data[j - 1].ToLower(), header2[i - 1].Data[j - 1].ToLower()));
                            return false;
                        }
                    }
                }
            }
            return true;
        }
    }
}
