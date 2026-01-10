using Excel2dllCore.Tools;
using System.Text;

namespace Excel2dllCore.Type
{
    internal class TpFloat : IDataType
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
                float fdata;
                if (!float.TryParse(value, out fdata))
                {
                    value = "0";
                    return false;
                }
                if (ParseCommand.CodeLanguage == "cs")
                {
                    value = value + "f";
                }
            }
            return true;
        }

        public void WriteData(string value, EBinaryWriter bw)
        {
            float data;
            float.TryParse(value.Replace("f", ""), out data);
            bw.Write(data);
        }

        public void ReadData(StringBuilder sb)
        {
            sb.AppendLine("br.ReadSingle();");
        }

        public string TypeName { get { return "float"; } }
    }
}