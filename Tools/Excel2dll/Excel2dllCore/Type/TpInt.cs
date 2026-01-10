using Excel2dllCore.Tools;
using System.Text;

namespace Excel2dllCore.Type
{
    internal class TpInt : IDataType
    {
        public bool ProcessData(ref string value)
        {
            if (value.Length == 0)
            {
                value = "0";
            }
            else
            {
                if (value.Contains(","))
                {
                    value = "0";
                    return false;
                }
                int idata;
                if (!int.TryParse(value, out idata))
                {
                    value = "0";
                    return false;
                }
            }
            return true;
        }

        public void WriteData(string value, EBinaryWriter bw)
        {
            int data = 0;
            int.TryParse(value, out data);
            bw.Write(data);
        }

        public void ReadData(StringBuilder ctrDef)
        {
            ctrDef.AppendLine("br.ReadInt32();");
        }

        public string TypeName { get { return "int"; } }
    }
}