using System.IO;
using System.Text;
using Excel2dllCore.Tools;
namespace Excel2dllCore.Type
{
    public interface IDataType
    {
        bool ProcessData(ref string value);
        string TypeName { get; }

        void WriteData(string value, EBinaryWriter bw);
        void ReadData(StringBuilder ctrDef);
    }
}