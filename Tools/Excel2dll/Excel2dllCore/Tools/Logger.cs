using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Excel2dllCore.Tools
{
    public static class Logger
    {
       // private static bool firstError = false;
        public static int ErrorCount; //错误数量

        private static bool quietMode = false;
        private static bool firstLog = false;

        private static string targetFile = "Log/xlsxc.txt";   //默认文件

        public static void Debug(string info)   //普通信息
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            if(!quietMode)
                Console.WriteLine(info);

            WriteLog("Debug", info);
        }

        public static void Warn(string info)    //警告信息
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            
            Console.WriteLine(info);

            WriteLog("Warn", info);
        }

        public static void Error(string info)   //错误信息
        {
            ErrorCount++;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Error.WriteLine(info);
            WriteLog("Error", info);

//            if (!firstError)
//            {
//                if (Environment.OSVersion.Platform != PlatformID.Unix)
//                {
//                    MessageBox.Show(info, "错误", MessageBoxButtons.OK);
//                }
//                else
//                {
//                    Console.WriteLine("Error: {0}", info);
//                }
//                
//                firstError = true;
//            }
        }

        private static void WriteLog(string tag, string info)
        {
            try
            {
                if (!Directory.Exists("Log"))
                {
                    Directory.CreateDirectory("Log");
                }
                if (firstLog == false)
                {
                    firstLog = true;
                    using (var sw = new StreamWriter(targetFile, false, Encoding.UTF8))
                    {
                        var time = DateTime.Now.ToString("[yyyy-MM-dd HH:mm:ss:fff]");
                        sw.WriteLine(time + "[{0}]" + info, tag);
                    }
                }
                else
                {
                    using (var sw = new StreamWriter(targetFile, true, Encoding.UTF8))
                    {
                        var time = DateTime.Now.ToString("[yyyy-MM-dd HH:mm:ss:fff]");
                        info = info.Replace("{", "{{").Replace("}","}}");                     
                        sw.WriteLine(time + "[{0}]" + info, tag);
                    }
                }
            }
            catch (Exception ep)
            {
                Logger.Error("ERROR: 写日志报错： " + ep);
            }
        }

        public static void Set(string logFile, bool quiet)
        {
            quietMode = quiet;
       //     firstError = quiet;
            targetFile = "Log/" + logFile;
        }
    }
}
