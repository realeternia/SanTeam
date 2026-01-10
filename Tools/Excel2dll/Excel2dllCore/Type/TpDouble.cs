using Excel2dllCore.Tools;
using System.Text;

namespace Excel2dllCore.Type
{
    internal class TpDouble : IDataType
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
                double fdata;
                if (!double.TryParse(value, out fdata))
                {
                    value = "0";
                    return false;
                }
            }
            return true;
        }

        public void WriteData(string value, EBinaryWriter bw)
        {
            double data;
            double.TryParse(value, out data);
            bw.Write(data);
        }

        public void ReadData(StringBuilder ctrDef)
        {
            ctrDef.AppendLine("br.ReadDouble();");
        }

        public string TypeName { get { return "double"; } }
    }
}