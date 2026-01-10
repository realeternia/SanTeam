using Excel2dllCore.Tools;
using System.Text;

namespace Excel2dllCore.Type
{
    internal class TpColor : IDataType
    {
        public bool ProcessData(ref string value)
        {
            if (value.Length == 0)
            {
                value = "\"#FFFFFF\"";
            }
            else
            {
                value = "\"#" + value + "\""; //16½øÖÆÑÕÉ«
            }
            return true;
        }

        public void WriteData(string value, EBinaryWriter bw)
        {
            bw.Write(value);
        }

        public void ReadData(StringBuilder ctrDef)
        {
            ctrDef.AppendLine("br.ReadString();");
        }

        public string TypeName { get { return "string"; } }
    }
}