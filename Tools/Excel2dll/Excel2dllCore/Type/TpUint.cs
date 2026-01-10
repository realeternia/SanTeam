using Excel2dllCore.Tools;
using System.Text;

namespace Excel2dllCore.Type
{
    internal class TpUint : IDataType
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
                uint idata;
                if (!uint.TryParse(value, out idata))
                {
                    value = "0";
                    return false;
                }
            }
            return true;

        }

        public void WriteData(string value, EBinaryWriter bw)
        {
            uint data;
            uint.TryParse(value, out data);
            bw.Write(data);
        }

        public void ReadData(StringBuilder ctrDef)
        {
            ctrDef.AppendLine("br.ReadUInt32();");
        }

        public string TypeName { get { return "uint"; } }

    }
}