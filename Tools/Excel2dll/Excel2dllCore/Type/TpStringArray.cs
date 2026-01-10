using Excel2dllCore.Tools;
using System.Text;

namespace Excel2dllCore.Type
{
    internal class TpStringArray : IDataType
    {
        public bool ProcessData(ref string value)
        {
            if (value.Length == 0)
            {
                value = "null";
            }
            else
            {
                value = value.Replace(",", "，");//把英文逗号转中文逗号                
                value = value.Replace("|\n", "|");//没个Array以 | 隔开
                value = value.Replace("\n", "\\n");
                value = value.Replace("\r", "\\r");
                value = value.Replace("|", ","); // | 是真正的分割符号
                var vs = value.Split(',');
                value = "new string[]{\"" + string.Join("\",\"", vs) + "\"}";
            }
            return true;
        }

        public void WriteData(string value, EBinaryWriter bw)
        {
            value = value.Replace("new string[]{\"","");
            value = value.Replace("\"}", "");
            value = value.Replace("\\", "");
            string[] stringArray = value.Split(',');

            if (stringArray.Length == 1 && stringArray[0] == "null")
            {
                bw.Write(0);
                return;
            }

            bw.Write(stringArray.Length);
            for (int i = 0; i < stringArray.Length; i++)
            {
                if (stringArray[i] == "null")
                    stringArray[i] = "";
                bw.Write(stringArray[i]);
            }
        }

        public void ReadData(StringBuilder ctrDef)
        {
            ctrDef.AppendLine("(string[])RecordTool.ReadArray(br, \"string[]\");");
        }

        public string TypeName { get { return "string[]"; } }

    }
}