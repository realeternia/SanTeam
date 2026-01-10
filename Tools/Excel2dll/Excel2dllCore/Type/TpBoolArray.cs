using Excel2dllCore.Tools;
using System.Text;

namespace Excel2dllCore.Type
{
    internal class TpBoolArray : IDataType
    {
        public bool ProcessData(ref string value)
        {
            if (value.Length == 0)
            {
               value = ParseCommand.CodeLanguage == "ls" ? "{}" : "null";
            }
            else
            {
                value = Utils.CheckNumberArrayStr(value);
                string[] bools = value.Split(',');
                for (int i = 0; i < bools.Length; i++)
                {
                    string sfloat = bools[i].ToLower();
                    if (string.IsNullOrEmpty(sfloat))
                        bools[i] = "false";
                    else
                    {
                        bool bdata;
                        if (!bool.TryParse(sfloat, out bdata))
                        {
                            value = "false";
                            return false;
                        }
                    }
                }
                if (ParseCommand.CodeLanguage == "as")//lua 导出
                    value = "[" + string.Join(",", bools) + "]";
                else if (ParseCommand.CodeLanguage == "ls")//lua 导出
                    value = "{" + string.Join(",", bools) + "}";
                else
                    value = "new bool[]{" + string.Join(",", bools) + "}";
            }
            return true;
        }

        public void WriteData(string value, EBinaryWriter bw)
        {
            value = value.Replace("new bool[]{", "");
            value = value.Replace("}", "");
            string[] boolArray = value.Split(',');

            if (boolArray.Length == 1 && boolArray[0] == "null")
            {
                bw.Write(0);
                return;
            }

            bw.Write(boolArray.Length);
            for (int i = 0; i < boolArray.Length; i++)
            {
                if (boolArray[i] == "null")
                    boolArray[i] = "false";
                bw.Write(bool.Parse(boolArray[i]));
            }
        }

        public void ReadData(StringBuilder ctrDef)
        {
            ctrDef.AppendLine("(bool[])RecordTool.ReadArray(br, \"bool[]\");");
        }

        public string TypeName { get { return "bool[]"; } }
    }
}