using System.Text;
using Excel2dllCore.Tools;

namespace Excel2dllCore.Type
{
    internal class TpAny : IDataType
    {
        public bool ProcessData(ref string value)
        {
            if (value.Length == 0)
            {
                if (ParseCommand.CodeLanguage == "as")
                    value = "null";
                else
                    value = "null";
            }
            return true;
        }

        public void WriteData(string value, EBinaryWriter bw)
        {
            if(value.Length == 0)
            {
                if (ParseCommand.CodeLanguage == "as")
                    bw.Write("null");
                else
                    bw.Write("null");
                return;
            }
            bw.Write(value);
        }

        public void ReadData(StringBuilder ctrDef)
        {
            //ctrDef.Append("br.ReadString()");
        }

        public string TypeName { get { return "any"; } }

    }
}