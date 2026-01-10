using Excel2dllCore.Tools;
using System.Text;

namespace Excel2dllCore.Type
{
    internal class TpBool : IDataType
    {
        public bool ProcessData(ref string value)
        {
            if (value.Length == 0)
            {
                value = "false";
            }
            else
            {
                value = value.ToLower();
                if (value.Contains(","))
                {
                    value = "false";
                    return false;
                }
                bool bdata;
                if (!bool.TryParse(value, out bdata))
                {
                    value = "false";
                    return false;
                }
            }
            return true;
        }

        public void WriteData(string value, EBinaryWriter bw)
        {
            bool data;
            bool.TryParse(value.ToLower(), out data);
            bw.Write(data);
        }

        public void ReadData(StringBuilder ctrDef)
        {
            ctrDef.AppendLine("br.ReadBoolean();");
        }

        public string TypeName { get { return "bool"; } }
    }
}