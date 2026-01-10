using System.Collections.Generic;
using Excel2dllCore.Type;
using System.Text;
using Excel2dllCore.Tools;

namespace Excel2dllCore.Load
{

    //注册Excel表中表头的各种类型

    public static class RegisterType
    {
        private static Dictionary<string, IDataType> typeFunction = new Dictionary<string, IDataType>();
        private static List<string> userNameSpaceList = new List<string>();//一些用户自定义的命名空间需要手动导入
        internal static bool HasError = false;
        public static StringBuilder typeCount = new StringBuilder();    //记录数据类型用于md5校验
        static RegisterType()
        {
            userNameSpaceList.Add("using System;");
            userNameSpaceList.Add("using System.Collections;");
            userNameSpaceList.Add("using System.Collections.Generic;");

            typeFunction.Add("string", new TpString());
            typeFunction.Add("string[]", new TpStringArray());
            typeFunction.Add("int", new TpInt());
            typeFunction.Add("uint", new TpUint());
            typeFunction.Add("long", new TpLong());
            typeFunction.Add("ulong", new TpULong());
            typeFunction.Add("int[]", new TpIntArray());
            typeFunction.Add("uint[]", new TpUIntArray());
            typeFunction.Add("float", new TpFloat());
            typeFunction.Add("float[]", new TpFloatArray());
            typeFunction.Add("double", new TpDouble());
            typeFunction.Add("double[]", new TpDoubleArray());
            typeFunction.Add("bool", new TpBool());
            typeFunction.Add("bool[]", new TpBoolArray());
            typeFunction.Add("color", new TpColor());
            typeFunction.Add("any", new TpAny());
        }

        internal static string GetNamespaceStr()
        {
            return string.Join("\n", userNameSpaceList.ToArray()) + "\n";
        }

        public static void Register(string name, IDataType tp)
        {
            typeFunction.Add(name, tp);
        }

        public static void RegisterNameSpace(string name)
        {
            userNameSpaceList.Add(name);
        }

        internal static bool HasType(string type)
        {
            return typeFunction.ContainsKey(RealType(type).ToLower());
        }

        internal static bool ProcessValue(string type, ref string value)
        {
            return typeFunction[RealType(type).ToLower()].ProcessData(ref value);
        }

        internal static string GetTypeName(string type)
        {

            IDataType data;
            if (typeFunction.TryGetValue(RealType(type).ToLower(), out data))
            {
                return data.TypeName;
            }
            return "";
        }

        internal static void WriteValue(CellValue cv, EBinaryWriter bw)
        {
            typeFunction[RealType(cv.Type.Type.ToLower())].WriteData(cv.Value, bw);
        }

        internal static void ReadValue(string Type, StringBuilder ctrDef)
        {
            typeCount.Append(Type);
            typeFunction[RealType(Type.ToLower())].ReadData(ctrDef);
        }


        /// <summary>
        /// 类型 现在组成 realType(_config_value)
        /// realType是真正的类型
        /// config是配置项
        /// value 是配置值
        /// 比如string_lan_cht 真实的类型为string lan是配置项，表示string所用的语言 cht是配置值，表示繁体中文
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static string RealType(string type)
        {
            return type.Split('_')[0];
        }
    }
}
