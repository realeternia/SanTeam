using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Excel2dllCore.Load
{
    public class ReadCfg : ReadConfig
    {
        public override void FormatContent(FileInfo fileInfo, string fileName, out LineData[] headers, out List<LineData> records, bool idOnly = false)
        {
            var content = ReadFile(fileInfo.FullName);
            var lines = content.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            var headerRaws = new List<string[]>();

            bool bHead = false, bApi = false, bData = false, bInHerit = false;
            records = new List<LineData>();
            List<ApiData> apiList = new List<ApiData>();
            foreach (var line in lines)
            {
                if (string.IsNullOrEmpty(line))
                    continue;
                if (line.StartsWith("[head]"))
                {
                    bHead = true;
                    bApi = false;
                    bData = false;
                    bInHerit = false;
                    continue;
                }
                if (line.StartsWith("[api]"))
                {
                    bHead = false;
                    bApi = true;
                    bData = false;
                    bInHerit = false;
                    continue;
                }
                if (line.StartsWith("[inherit]")) //继承的类
                {
                    bHead = false;
                    bApi = false;
                    bData = false;
                    bInHerit = true;
                    continue;
                }
                if (line.StartsWith("[data]"))
                {
                    bHead = false;
                    bApi = false;
                    bData = true;
                    bInHerit = false;
                    continue;
                }
                if (bHead)
                {
                    //Id	uint	cs	序号
                    var infos = line.Split(new[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);
                    if (infos.Length < 4)
                    {
                        throw new Exception(string.Format("ERROR: 检查到{0}表头配置错误", fileName));
                    }
//                    if (fileName != "GameConfig" && headerRaws.Count == 0 && infos[0].Trim().ToLower() != "id")
//                    {
//                        throw new Exception(string.Format("ERROR: 配置表{0}错误，第一列必须是Id  跳过", fileInfo.Name));
//                    }

                    var parmName = infos[0].Trim();
                    if (parmName == "base") // 属性命名问题修复
                    {
                        parmName += "Val";
                    }
                    if (parmName == fileName)
                    {
                        parmName += "Data";
                    }
                    var parmType = infos[1].Trim().Replace("varchar","string").Replace("char", "string").Replace("bigint", "long");
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

//                    if (headerRaws.Count == 0)
//                    {
//                        parmName = "Id";
//                        parmType = "uint";
//                    }

                    headerRaws.Add(new string[] { infos[3].Trim(), parmName, parmType, codeLang });
                    continue;
                }
                if (bApi)
                {
                    var infos = line.Split(new[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);
                    if (infos.Length < 3)
                    {
                        throw new Exception(string.Format("ERROR: 配置表{0}错误，检查[api] {1} 的格式", fileName, line));
                    }
                    ApiData api = new ApiData();
                    var cs = infos[0].Trim().ToLower();//c 表示客户端，s 表示服务端
                    if(cs != "c" && cs != "s" && cs != "cs")
                        throw new Exception(string.Format("ERROR: 配置表{0}错误，检查[api] {1} 的cs格式", fileName, line));
                    api.Cs = cs;

                    var am = infos[1].Trim().ToUpper();
                    switch (am)
                    {
                        case "A":   // A 表示结果是唯一的;
                            api.ResultsOnly = true;
                            break;
                        case "M":   // M 表示结果是多个
                            api.ResultsOnly = false;
                            break;
                        default:
                            throw new Exception(string.Format("ERROR: 配置表{0}错误，检查[api] {1} 的AM格式", fileName, line));
                    }
                    for (int i = 2; i < infos.Length; i++)
                    {
                        if (api.Keys.Count > 0)
                            api.ApiName += "And";
                        var field = infos[i].Trim();
                        api.Keys.Add(field);
                        api.ApiName += field;
                    }
                    apiList.Add(api);
                }
                if (bInHerit)
                {
                    //lyx 继承的类
                }
                if (bData)
                {
                    LineData data = new LineData();
                    data.Data = line.Split(new[] { '\t' });
                    records.Add(data);
                }
            }
            var parmCount = idOnly ? 1 : headerRaws.Count;
            headers = ProcessHeader(headerRaws, parmCount);
            if(apiList.Count > 0)
                Global.ApiDict.Add(fileName, apiList);
        }

        public static string ReadFile(string filePath)
        {
            byte[] buffer = File.ReadAllBytes(filePath);
            if (buffer == null)
                return null;

            if (buffer.Length <= 3)
            {
                return Encoding.UTF8.GetString(buffer);
            }

            byte[] bomBuffer = new byte[] { 0xef, 0xbb, 0xbf };

            if (buffer[0] == bomBuffer[0]
                && buffer[1] == bomBuffer[1]
                && buffer[2] == bomBuffer[2])
            {
                return new UTF8Encoding(false).GetString(buffer, 3, buffer.Length - 3);
            }

            return Encoding.UTF8.GetString(buffer);
        }

        public static LineData[] ProcessHeader(List<string[]> headerRaw, int parmCount)
        {
            var header = new LineData[4];
            for (int i = 0; i < 4; i++)
            {
                var headerData = new LineData();
                headerData.Data = new string[headerRaw.Count];
                header[i] = headerData;
            }
            for (int j = 0; j < parmCount; j++)
            {
                header[0].Data[j] = headerRaw[j][0].ToString().Trim();
                header[1].Data[j] = headerRaw[j][1].ToString().Trim();
                header[2].Data[j] = headerRaw[j][2].ToString().Trim();
                header[3].Data[j] = headerRaw[j][3].ToString().Trim();
            }
            return header;
        }

    }
}
