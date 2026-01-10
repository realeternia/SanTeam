using System.Collections.Generic;

namespace Excel2dllCore
{
    /// <summary>
    /// 缓存excel中的数据，衔接几个模块，防止一直传递
    /// </summary>
    internal static class Global
    {
        public static readonly string LogFile = "xlsxc.log";

        public static Dictionary<string, List<CellType>> TypeDict  = new Dictionary<string, List<CellType>>(); //保存类型
        public static Dictionary<string, List<Record>> DataDict   = new Dictionary<string, List<Record>>();   //保存数据

        public static List<CheckerDataType> CheckerDataList = new List<CheckerDataType>();//保存所有函数检查数据
    }
}
