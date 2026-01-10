using System;
using System.Collections.Generic;

namespace Excel2dllCore
{
    /// <summary>
    /// 每一格表头格子，包含4行的信息
    /// </summary>
    public class CellType
    {
        /// <summary>
        /// 字段名
        /// </summary>
        public string FieldName;
        /// <summary>
        /// cs 标志
        /// </summary>
        public string CS;
        /// <summary>
        /// 类型
        /// </summary>
        public string Type;
        /// <summary>
        /// 中文描述
        /// </summary>
        public string Desc;

        /// <summary>
        /// ~名字前缀的类型无视 string类型语言不和传入参数一样的无视
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsIgnoreType(CellType type)
        {
            if (type.FieldName.StartsWith("~"))
                return true;

            bool langIgnore = false;
            var isString = type.Type.Contains("string");
            var isMultiLan = type.Type.Contains("lan");
            if (isString && isMultiLan)
            {
                var stringLanArray = type.Type.Split('_');
                var localLangType = stringLanArray[stringLanArray.Length - 1];
                langIgnore = localLangType != ParseCommand.LocalLanguage;
            }

            return langIgnore;
        }
    }

    /// <summary>
    /// 每一格数据
    /// </summary>
    public class CellValue
    {
        public CellType Type;
        public string Value;
    }

    /// <summary>
    /// 每一条记录，由excel文件中一行数据组成
    /// </summary>
    public class Record
    {
        public string Id;
        public string ConstantName; //别名，例如 Town
        public List<CellValue> Values = new List<CellValue>();

        internal bool TryGetValue(string fieldName, out object value)
        {
            value = null;

            if (Values == null || string.IsNullOrEmpty(fieldName))
                return false;

            foreach (var cellValue in Values)
            {
                if (cellValue?.Type?.FieldName == null)
                    continue;

                // 处理带下划线的字段名（如 fieldName_chs）
                var fieldNameArray = cellValue.Type.FieldName.Split('_');
                var baseFieldName = fieldNameArray[0];

                if (baseFieldName == fieldName)
                {
                    value = cellValue.Value ?? "";
                    return true;
                }
            }

            return false;
        }
    }

    public class LineData
    {
        public string[] Data { get; set; }
    }

    public class CheckerDataType
    {
        public string FileName;
        public int Index;
        public LineData[] Headers;
        public List<LineData> Records;
    }

    public class ApiData
    {
        public string Cs;   //客户端服务端标志
        public bool ResultsOnly;   //结果是唯一的
        public List<string> Keys;   //KeyList
        public string ApiName;
        public ApiData()
        {
            Keys = new List<string>();
            ApiName = "";
        }
    }
}
