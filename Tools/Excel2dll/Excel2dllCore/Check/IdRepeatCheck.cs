using System.Collections.Generic;
using Excel2dllCore.Tools;

namespace Excel2dllCore.Check
{
    class IdRepeatCheck
    {
        public static List<string> NoIdCheckFileNameList = new List<string>();//用来储存不需要遵循全局id检查规则的文件
        public static Dictionary<int, string[]> IdRepeatDict = new Dictionary<int, string[]>();//用来检查所有一维表id是否有重复

        public static bool IdRepeat(string id, string fileName, int index, int row, ref HashSet<string> idSetLocal)
        {
            if (NoIdCheckFileNameList.Contains(fileName))
            {
                if (!idSetLocal.Add(id)) //不允许本表id重复
                {
                    Logger.Error(string.Format("ERROR: {0}文件 {1}分页 {2}行 id={3} 本表id重复定义  重复源：{4}文件 {5}分页", fileName, index, row + 4, id, fileName, index));
                    return false;
                }
            }
            else
            {
                int idRepeat = 0;
                int.TryParse(id, out idRepeat);
                if (idRepeat >= 10000000)
                {
                    if (IdRepeatDict.ContainsKey(idRepeat))
                    {
                        string[] repeatInfo = (string[])IdRepeatDict[idRepeat];
                        Logger.Error(string.Format("ERROR: {0}文件 {1}分页 {2}行 id={3} id重复定义  重复源：{4}文件 {5}分页", fileName, index, row + 4, id, repeatInfo[0], repeatInfo[1]));
                        return false;
                    }
                    else
                    {
                        string[] idFileNameIndex = { fileName, index.ToString() };
                        IdRepeatDict.Add(idRepeat, idFileNameIndex);
                    }
                }
            }
            return true;
        }

        public static bool IdConstNameRepeat(string idConstName, string fileName, int index, int row, ref HashSet<string> idConstNameSet)
        {
            if (!idConstNameSet.Add(idConstName))
            {
                Logger.Error(string.Format("ERROR: {0}文件 {1}分页 此分页内id别名重复 id别名{2}", fileName, index, idConstName));
                return false;
            }
            return true;
        }

    }
}
