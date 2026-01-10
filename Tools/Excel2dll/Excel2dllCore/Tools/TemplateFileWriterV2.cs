using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Excel2dllCore.Tools
{
    public class TemplateFileWriterV2 : IDisposable
    {
        private StreamWriter sw;
        private StreamReader template;//模板文件读取器
        private Dictionary<string, string> varDict = new Dictionary<string, string>();//替换的字典

        public TemplateFileWriterV2(string outpath, string templatePath)
        {
            sw = new StreamWriter(outpath, false, Encoding.UTF8);
            template = new StreamReader(ResourceLoader.Load(templatePath), Encoding.UTF8);
        }

        public void SetVar(string var, string replaceText)
        {
            varDict[var] = replaceText;
        }

        public void Run()
        {
            string line;
            while ((line = template.ReadLine()) != null)
            {
                foreach (var keyValue in varDict)
                {
                    line = line.Replace("@" + keyValue.Key, keyValue.Value);
                }
                sw.WriteLine(line);
            }
        }

        public void Dispose()
        {
            Run();
            template.Close();
            sw.Close();
        }
    }
}