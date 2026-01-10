using Excel2dllCore.Tools;
using System.Text;
using Excel2dllCore;
using Excel2dllCore.Type;

namespace Excel2dll.Type
{
    internal class TpCFGVector3 : IDataType
    {
        public bool ProcessData(ref string value)
        {
            if (value.Length == 0)
            {
                if (ParseCommand.CodeLanguage == "ts")
                    value = "new AutoGen.DGVector3()";
                else
                    value = "CFGVector3.Zero";
            }
            else
            {
                string[] datas = value.Split(',');
                if (datas.Length != 3)
                {
                    if (ParseCommand.CodeLanguage == "ts")
                        value = "new AutoGen.DGVector3()";
                    else
                        value = "CFGVector3.Zero";
                    return false;
                }
                if (ParseCommand.CodeLanguage == "ts")
                    value = "new AutoGen.DGVector3(" + value + ")";
                else
                    value = "CFGVector3.Parse(\"" + value + "\")";
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
                    return "DGVector3";
                return "CFGVector3";
            }
        }

    }
}