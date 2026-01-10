@namespace
using System.IO;
namespace CommonConfig
{
    public class @fileNameRecordAdder
    {
        public static void Load(EBinaryReader br)
        {
            Dictionary<uint, @fileName> tpDict = new Dictionary<uint, @fileName>();
            @loadData LoadData(br, tpDict);
@methods
            @fileName.Refresh(tpDict);
        }

        public static void LoadData(EBinaryReader br, Dictionary<uint, @fileName> tpDict)
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
