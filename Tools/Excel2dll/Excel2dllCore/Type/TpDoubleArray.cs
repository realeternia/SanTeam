using System.Text;
using Excel2dllCore.Tools;

namespace Excel2dllCore.Type
{
    internal class TpDoubleArray : IDataType
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
                string[] doubles = value.Split(',');
                for (int i = 0; i < doubles.Length; i++)
                {
                    string sdouble = doubles[i];
                    if (string.IsNullOrEmpty(sdouble))
                        doubles[i] = "0";
                    else
                    {
                        float ddata;
                        if (!float.TryParse(sdouble, out ddata))
                        {
                            value = "null";
                            return false;
                        }
                    }
                }
                if (ParseCommand.CodeLanguage == "as")
                    value = "[";
                else if (ParseCommand.CodeLanguage == "ls")
                    value = "{";
                else
                    value = "new double[]{";
                foreach (string sdouble in doubles)
                {
                    value += (sdouble + ",");
                }
                value = value.Remove(value.LastIndexOf(','));
                if (ParseCommand.CodeLanguage == "as")
                    value += "]";
                else
                    value += "}";
            }
            return true;
        }

        public void WriteData(string value, EBinaryWriter bw)
        {
            value = value.Replace("new double[]{","");
            value = value.Replace("}", "");
            string[] doubleArray = value.Split(',');

            if (doubleArray.Length == 1 && doubleArray[0] == "null")
            {
                bw.Write(0);
                return;
            }

            bw.Write(doubleArray.Length);
            for (int i = 0; i < doubleArray.Length; i++)
            {
                if (doubleArray[i] == "null")
                    doubleArray[i] = "0";
                bw.Write(double.Parse(doubleArray[i]));
            }
        }

        public void ReadData(StringBuilder ctrDef)
        {
            ctrDef.AppendLine("(double[])RecordTool.ReadArray(br, \"double[]\");");
        }

        public string TypeName { get { return "double[]"; } }
    }
}