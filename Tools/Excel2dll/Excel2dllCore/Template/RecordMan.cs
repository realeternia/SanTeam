@namespace
using System.IO;

namespace CommonConfig
{
    public class RecordManager
    {
        public static void LoadAll()
        {
            string ConfigFilePath = "@configFilePath";
            FileStream fs = new FileStream(ConfigFilePath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
            byte[] data = new byte[fs.Length];
            fs.Read(data, 0, (int)fs.Length);
            EBinaryReader br = new EBinaryReader(data);
            RecordTool.CheckMD5(fs, br);     
            fs.Close();
@methodLoad
        }
        public static void UnloadAll()
        {
            @methodUnload
        }
    }


}
