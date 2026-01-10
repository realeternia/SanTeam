using Excel2dllCore.Tools;
using System.Text;
using Excel2dllCore;
using Excel2dllCore.Type;

namespace Excel2dll.Type
{
    internal class TpCFGVector3Array : IDataType
    {
        public bool ProcessData(ref string value)
        {
            if (value.Length == 0)
            {
                value = "null";
            }
            else
            {
                string[] datas = value.Split('|'); //·Ö¸ô·û
                string innerText = "";
                foreach (var data in datas)
                {
                    string[] datatext = data.Split(',');
                    if (datatext.Length != 3)
                    {
                        value = "null";
                        return false;
                    }
                    if (innerText != "")
                    {
                        innerText += ",";
                    }
                    if (ParseCommand.CodeLanguage == "ts")
                        innerText = "AutoGen.DGVector3(\"" + data + "\")";
                    else
                        innerText += "CFGVector3.Parse(\"" + data + "\")";
                }
                value = "new CFGVector3[]{" + innerText + "}";
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
                if (ParseCommand.CodeLanguage == "ts")
                    return "DGVector3[]";
                return "CFGVector3[]";
            }
        }
    }
}