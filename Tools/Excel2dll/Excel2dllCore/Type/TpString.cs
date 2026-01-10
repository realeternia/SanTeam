using Excel2dllCore.Tools;
using System.Text;

namespace Excel2dllCore.Type
{
    internal class TpString : IDataType
    {
        public bool ProcessData(ref string value)
        {
            if (value.Length == 0)
            {
                if (ParseCommand.CodeLanguage == "as" || ParseCommand.CodeLanguage == "ls")
                    value = "\"\"";
                else
                    value = "";
            }
            else
            {
                value = value.Replace("\"", "\\\"");
                value = value.Replace("\n", "\\n");
                value = value.Replace("\r", "\\r");
                value = "\"" + value + "\"";
            }
            return true;
        }

        public void WriteData(string value, EBinaryWriter bw)
        {
            value = value.Replace("\"", "");
            bw.Write(value);
        }

        public void ReadData(StringBuilder ctrDef)
        {
            ctrDef.AppendLine("br.ReadString();");
        }

        public string TypeName { get { return "string"; } }

    }
}