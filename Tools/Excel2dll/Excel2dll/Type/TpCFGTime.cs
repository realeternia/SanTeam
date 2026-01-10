using Excel2dllCore.Tools;
using System.Text;
using Excel2dllCore;
using Excel2dllCore.Type;

namespace Excel2dll.Type
{
    internal class TpCFGTime : IDataType
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
                if (ParseCommand.CodeLanguage == "ts")
                    value = value + "*1000";
                else
                    value = value + "f";
            }
            return true;
        }

        public void WriteData(string value, EBinaryWriter bw)
        {
            throw new System.NotImplementedException();
        }

        public void ReadData(StringBuilder ctrDef)
        {
            throw new System.NotImplementedException();
        }

        public string TypeName
        {
            get
            {
                return "float";
            }
        }

    }
}