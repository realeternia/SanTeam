using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using OfficeOpenXml;

namespace Excel2dllCore.Tools
{
    internal class ExcelFileOpener
    {
        public static int OpenFileCount = 0;//文件打开总次数

        public static ExcelPackage Open(FileInfo file, bool isWrite)
        {
            OpenFileCount++;
            if (isWrite)
            {
                return OpenWrite(file);
            }
            return OpenRead(file);
        }

        private static ExcelPackage OpenWrite(FileInfo file)
        {
            ExcelPackage ep = null;

            while (ep == null)
            {
                try
                {
                    ep = new ExcelPackage(file);
                }
                catch (IOException)
                {
                    if (Environment.OSVersion.Platform != PlatformID.Unix)
                    {
                        var result = MessageBox.Show("配置表" + file.FullName + "打开着，请先关闭。选“是”关闭后重试，选“否”忽略这个警告", "错误",
                            MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                        if (result == DialogResult.Yes)
                        {
                            Thread.Sleep(2000);
                        }
                        else
                        {
                            break;
                        }
                    }
                    else
                    {
                        Logger.Warn(string.Format("配置表 {0} 打开着，请先关闭!!!", file.FullName));
                    }
                }
            }
            return ep;
        }

        private static ExcelPackage OpenRead(FileInfo file)
        {
            ExcelPackage ep = null;
            string tempFile = null;
            try
            {
                ep = new ExcelPackage(file);
            }
            catch
            {
                Logger.Debug("\t文件打开中，创建临时文件重试. " + file.Name);
                tempFile = Path.GetTempFileName();
                ep = new ExcelPackage(file.CopyTo(tempFile, true));
            }

            return ep;
        }
    }
}
