using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Excel2dllCore.Load;

namespace Excel2dllCore.Tools
{
    public class CfgTool
    {
        public static LineData[] ReadHeader(FileInfo fileInfo, string fileName, int columnCount)
        {
            var header = new LineData[] { };
            var content = ReadCfg.ReadFile(fileInfo.FullName);
            var lines = content.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            var headerRaws = new List<string[]>();

            bool bHead = false;
            int index = 0;
            //Id	uint	cs	序号
            foreach (var line in lines)
            {
                if (string.IsNullOrEmpty(line))
                    continue;
                if (line.StartsWith("[head]"))
                {
                    bHead = true;
                    continue;
                }
                if (line.StartsWith("[api]") || line.StartsWith("[data]"))
                {
                    bHead = false;
                    break;
                }
                if (bHead)
                {
                    if (index == columnCount)
                        break;
                    var infos = line.Split(new[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);
                    if (infos.Length < 4)
                    {
                        throw new Exception(string.Format("ERROR: 检查到{0}表头配置错误", fileName));
                    }
                    if (fileName != "GameConfig" && headerRaws.Count == 0 && infos[0].Trim().ToLower() != "id")
                    {
                        throw new Exception(string.Format("ERROR: 配置表{0}错误，第一列必须是Id  跳过", fileInfo.Name));
                    }

                    var parmName = infos[0].Trim();
                    if (parmName == "base") // 属性命名问题修复
                    {
                        parmName += "Val";
                    }
                    if (parmName == fileName)
                    {
                        parmName += "Data";
                    }
                    var parmType = infos[1].Trim().Replace("char", "string").Replace("bigint", "long");
                    var langAttr = infos[2].Trim();
                    string codeLang = langAttr;
                    string limit = "";
                    if (langAttr.Contains("|"))
                    {
                        var spiltIndex = langAttr.IndexOf("|");
                        codeLang = langAttr.Substring(0, spiltIndex).ToLower();
                        limit = langAttr.Substring(spiltIndex);
                    }

                    if (codeLang.StartsWith("cs") && codeLang != "c" && codeLang != "s")
                        codeLang = "cs";
                    codeLang += limit;

                    if (headerRaws.Count == 0)
                    {
                        parmName = "Id";
                        parmType = "uint";
                    }
                    headerRaws.Add(new string[] { infos[3].Trim(), parmName, parmType, codeLang });
                    index++;
                }
            }
            header = ReadCfg.ProcessHeader(headerRaws, headerRaws.Count);
            return header;
        }

        public static List<LineData> ReadRecord(FileInfo fileInfo, int ColumnCount)
        {
            var content = ReadCfg.ReadFile(fileInfo.FullName);
            var lines = content.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            bool bData = false;
            List<LineData> records = new List<LineData>();
            foreach (var line in lines)
            {
                if (string.IsNullOrEmpty(line))
                    continue;
                if (line.StartsWith("[data]"))
                {
                    bData = true;
                    continue;
                }
                if (line.StartsWith("[api]") || line.StartsWith("[head]"))
                {
                    bData = false;
                    continue;
                }
                if (bData)
                {
                    LineData data = new LineData();
                    string[] allData = line.Split(new[] { '\t' });
                    string[] temp = new string[ColumnCount];
                    if (ColumnCount != 10000)
                    {
                        for (int j = 0; j < allData.Length; j++)
                        {
                            for (int i = 0; i < ColumnCount; i++)
                            {
                                temp[i] = allData[i];
                            }
                        }
                    }
                    else
                        data.Data = line.Split(new[] { '\t' });
                    data.Data = temp;
                    records.Add(data);
                }
            }
            return records;
        }

        public static void UpdateFile(string path, string name, LineData[] header, LineData[] record)
        {
            StringBuilder buffer = new StringBuilder();
            //写表头
            buffer.AppendLine("[head]");
            for (int i = 0; i < header[0].Data.Length; i++)
            { 
                buffer.AppendLine(string.Format("{0}\t{1}\t{2}\t{3}", header[1].Data[i], header[2].Data[i], header[3].Data[i], header[0].Data[i]));
            }

            //写数据
            buffer.AppendLine("[data]");
            int index = 0;
            foreach (var dataList in record)
            {
                foreach(var info in dataList.Data)
                {
                    buffer.Append(info);
                    if (index < dataList.Data.Length - 1)
                    {
                        index++;
                        buffer.Append("\t");
                    }
                }
                index = 0;
                buffer.Append("\r");
            }
            string localNewGenerateFileName = path + Path.DirectorySeparatorChar + name + ".cfg";
            byte[] allData = Encoding.UTF8.GetBytes(buffer.ToString());
            FileStream fs = fs = new FileStream(localNewGenerateFileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            fs.Write(allData, 0, allData.Length);
            fs.Close();
        }
    }
}
