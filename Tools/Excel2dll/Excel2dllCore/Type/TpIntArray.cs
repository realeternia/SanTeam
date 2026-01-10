using System.Text;
using Excel2dllCore.Tools;

namespace Excel2dllCore.Type
{
    internal class TpIntArray : IDataType
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
                string[] ints = value.Split(',');
                for (int i = 0; i < ints.Length; i++)
                {
                    string sint = ints[i];
                    if (string.IsNullOrEmpty(sint))
                        ints[i] = "0";
                    else
                    {
                        int idata;
                        if (!int.TryParse(sint, out idata))
                        {
                            value = "null";
                            return false;
                        }
                    }
                }
                if (ParseCommand.CodeLanguage == "as")
                    value = "[" + string.Join(",", ints) + "]";
                else if (ParseCommand.CodeLanguage == "ls")
                    value = "{" + string.Join(",", ints) + "}";
                else
                    value = "new int[]{" + string.Join(",", ints) + "}";
            }
            return true;
        }

        public void WriteData(string value, EBinaryWriter bw)
        {
            value = value.Replace("new int[]{", "");
            value = value.Replace("}", "");
            string[] intArray = value.Split(',');

            if (intArray.Length == 1 && intArray[0] == "null")
            {
                bw.Write(0);
                return;
            }

            bw.Write(intArray.Length);
            for (int i = 0; i < intArray.Length; i++)
            {
                if (intArray[i] == "null")
                    intArray[i] = "0";
                bw.Write(int.Parse(intArray[i]));
            }
        }

        public void ReadData(StringBuilder ctrDef)
        {
            ctrDef.AppendLine("(int[])RecordTool.ReadArray(br, \"int[]\");");
        }

        public string TypeName { get { return "int[]"; } }
    }
}