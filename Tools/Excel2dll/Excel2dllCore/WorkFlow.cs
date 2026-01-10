using Excel2dllCore.Check;
using Excel2dllCore.Export;
using Excel2dllCore.Export.CSharp;
using Excel2dllCore.Load;
using Excel2dllCore.Merge;
using Excel2dllCore.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Excel2dllCore.Export.Lua;

namespace Excel2dllCore
{
    public class WorkFlow
    {
        public static int Run(string[] args)
        {
            bool ret = ParseCommand.Parse(args);
            Logger.Set(Global.LogFile, ParseCommand.Quiet);
            if (!ret)
            {
                Logger.Error("ERROR: 输入参数错误，请检查！！！");
                return 1;
            }

            //try
            //{
                List<string> excelList = new List<string>();
                if (ParseCommand.CheckChange)
                {
                    if (!File.Exists(ParseCommand.ChangeFile))
                        return 0;

                    excelList = File.ReadAllLines(ParseCommand.ChangeFile, Encoding.UTF8).ToList();
                    excelList.RemoveAll(a => a.EndsWith(".xlsx") == false && a.EndsWith(".cfg") == false);
                    if (excelList.Count == 0)
                        return 0;
                }
                else
                {
                    excelList = DataLoader.AllFiles;
                }

                Merger.Process(); //合表
                DataLoader.ProcessConfigFiles(excelList); //所有Excel表数据/内存数据读到内存
                DataCheck.ProcessAll();//所有Excel表数据进行检查
                if (!ParseCommand.IsNoOutput)
                {
                    IExporter exporter;
                    switch(ParseCommand.CodeLanguage)
                    {
                        case "cs":
                            exporter = new CsExporter();
                            break;
                        case "ls":
                            exporter = new LuaExporter();
                            break;
                        default:
                            throw new Exception("ERROR: 不合法的语言参数");
                    }
                    exporter.Prework();
                    exporter.ExportType(); //导出类（Excel表头）
                    exporter.ExportRecord(); //导出Excel数据
                    exporter.ExportVersion();  //导出版本文件 
                    //ExportHotPatch.Export();//导出用于热更新配置表文件
                }
        //}
        //    catch (Exception e)
        //    {
        //        Logger.Error("ERROR: " + e);
        //    }

    //todo 暂时写下
    Logger.Debug("总打开文件次数 " + ExcelFileOpener.OpenFileCount);
            Logger.Debug("错误数量 " + Logger.ErrorCount);

            return Logger.ErrorCount;
        }
    }
}