using Excel2dllCore.Tools;
using System.Text;
using Excel2dllCore;
using Excel2dllCore.Type;

namespace Excel2dll.Type
{
    internal class TpCFGVector2 : IDataType
    {
        public bool ProcessData(ref string value)
        {
            if (value.Length == 0)
            {
                if (ParseCommand.CodeLanguage == "ts")
                    value = "new AutoGen.DGVector2()";
                else
                    value = "CFGVector2.Zero";
            }
            else
            {
                string[] datas = value.Split(',');
                if (datas.Length != 2)
                {
                    if (ParseCommand.CodeLanguage == "ts")
                        value = "new AutoGen.DGVector2()";
                    else
                        value = "CFGVector2.Zero";
                    return false;
                }
                if (ParseCommand.CodeLanguage == "ts")
                    value = "new AutoGen.DGVector2(" + value + ")";
                else
                    value = "CFGVector2.Parse(\"" + value + "\")";
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
                    return "DGVector2";
                return "CFGVector2";
            }
        }

    }
}