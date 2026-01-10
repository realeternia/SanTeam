@namespace
namespace CommonConfig
{
    public class @fileNameRecordAdder
    {
        public static void Load(EBinaryReader br)
        {
@dictList
            @loadDataLoadData(br, @param);
            @fileName.Refresh(@param);
        }

        public static void LoadData(EBinaryReader br, @funcParam)
        {
            int count = 0;
            int line = br.ReadInt32();
            while (count < line)
            {
                @fileName info = new @fileName();
@methodDefine
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
