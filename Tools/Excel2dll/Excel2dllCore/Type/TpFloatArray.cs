using System.Text;
using Excel2dllCore.Tools;

namespace Excel2dllCore.Type
{
    internal class TpFloatArray : IDataType
    {
        public bool ProcessData(ref string value)
        {
            if (value.Length == 0)
                value = ParseCommand.CodeLanguage == "ls" ? "{}" : "null";
            else
            {
                value = Utils.CheckNumberArrayStr(value);
                string[] floats = value.Split(',');
                for (int i = 0; i < floats.Length; i++)
                {
                    string sfloat = floats[i];
                    if (string.IsNullOrEmpty(sfloat))
                        floats[i] = "0";
                    else
                    {
                        float fdata;
                        if (!float.TryParse(sfloat, out fdata))
                        {
                            value = "null";
                            return false;
                        }
                    }
                }
                if (ParseCommand.CodeLanguage == "as")
                    value = "[";
                else if (ParseCommand.CodeLanguage == "ls") //lua µ¼³ö
                    value = "{";
                else
                    value = "new float[]{";
                foreach (string sfloat in floats)
                {
                    if (ParseCommand.CodeLanguage == "as" || ParseCommand.CodeLanguage == "ls")
                        value += (sfloat + ",");
                    else
                        value += (sfloat + "f,");
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
            value = value.Replace("new float[]{", "");
            value = value.Replace("}", "");
            value = value.Replace("f", "");
            string[] floatArray = value.Split(',');

            if (floatArray.Length == 1 && floatArray[0] == "null")
            {
                bw.Write(0);
                return;
            }

            bw.Write(floatArray.Length);
            for(int i = 0; i < floatArray.Length;i++)
            {
                if(floatArray[i] == "null")
                    floatArray[i] = "0";
                bw.Write(float.Parse(floatArray[i]));
            }
        }

        public void ReadData(StringBuilder ctrDef)
        {
            ctrDef.AppendLine("(float[])RecordTool.ReadArray(br, \"float[]\");");
        }

        public string TypeName { get { return "float[]"; } }
    }
}