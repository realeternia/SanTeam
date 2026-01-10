using System.Text;
using Excel2dllCore.Tools;

namespace Excel2dllCore.Type
{
    internal class TpUIntArray : IDataType
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
                string[] uints = value.Split(',');
                for (int i = 0; i < uints.Length; i++)
                {
                    string suint = uints[i];
                    if (string.IsNullOrEmpty(suint))
                        uints[i] = "0";
                    else
                    {
                        uint idata;
                        if (!uint.TryParse(suint, out idata))
                        {
                            value = "null";
                            return false;
                        }
                    }
                }
                if (ParseCommand.CodeLanguage == "as")
                    value = "[" + string.Join(",", uints) + "]";
                else if (ParseCommand.CodeLanguage == "ls") //µ¼³ölua
                    value = "{" + string.Join(",", uints) + "}";
                else
                    value = "new uint[]{" + string.Join(",", uints) + "}";
            }
            return true;
        }

        public void WriteData(string value, EBinaryWriter bw)
        {
            value = value.Replace("new uint[]{", "");
            value = value.Replace("}", "");
            string[] uintArray = value.Split(',');

            if (uintArray.Length == 1 && uintArray[0] == "null")
            {
                bw.Write(0);
                return;
            }

            bw.Write(uintArray.Length);
            for (int i = 0; i < uintArray.Length; i++)
            {
                if (uintArray[i] == "null")
                    uintArray[i] = "0";
                bw.Write(uint.Parse(uintArray[i]));
            }
        }

        public void ReadData(StringBuilder ctrDef)
        {
            ctrDef.AppendLine("(uint[])RecordTool.ReadArray(br, \"uint[]\");");
        }

        public string TypeName { get { return "uint[]"; } }
    }
}