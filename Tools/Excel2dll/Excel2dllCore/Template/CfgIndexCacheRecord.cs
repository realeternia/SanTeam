@namespace
using System.IO;
namespace CommonConfig
{
    public class @fileNameRecordAdder
    {
        public static void Load(EBinaryReader br)
        {
            Dictionary<uint, @fileName> tpDict = new Dictionary<uint, @fileName>();
@dictList
            @loadData LoadData(br, tpDict @param);
            @fileName.Refresh(tpDict@param);
        }

        public static void LoadData(EBinaryReader br, Dictionary<uint, @fileName> tpDict@funcParam)
        {
            int count = 0;
            int line = br.ReadInt32();
            while (count < line)
            {
                @fileName info = new @fileName();
@methodDefine
                tpDict.Add(info.Id, info);
                count++;
            }
        }

        public static void Unload() { }

        public class @fileNameIndex
        {
@enumList
        }
    }
}
