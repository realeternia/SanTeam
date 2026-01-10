@namespace
using System.IO;
using System.Text;
using System.Security.Cryptography;

namespace CommonConfig
{
    class RecordTool
    {
        public static object ReadArray(EBinaryReader br, string type)
        {
            int count = br.ReadInt32();
            if (count == 0)
                return null;
            switch (type)
            {
                case "string[]":
                    string[] stringArray = new string[count];
                    for (int i = 0; i < count; i++)
                    {
                        stringArray[i] = br.ReadString();
                    }
                    return stringArray;
                case "uint[]":
                    uint[] uintArray = new uint[count];
                    for (int i = 0; i < count; i++)
                    {
                        uintArray[i] = br.ReadUInt32();
                    }
                    return uintArray;
                case "bool[]":
                    bool[] boolArray = new bool[count];
                    for (int i = 0; i < count; i++)
                    {
                        boolArray[i] = br.ReadBoolean();
                    }
                    return boolArray;
                case "double[]":
                    double[] doubleArray = new double[count];
                    for (int i = 0; i < count; i++)
                    {
                        doubleArray[i] = br.ReadDouble();
                    }
                    return doubleArray;
                case "float[]":
                    float[] floatArray = new float[count];
                    for (int i = 0; i < count; i++)
                    {
                        floatArray[i] = br.ReadSingle();
                    }
                    return floatArray;
                case "int[]":
                    int[] intArray = new int[count];
                    for (int i = 0; i < count; i++)
                    {
                        intArray[i] = br.ReadInt32();
                    }
                    return intArray;
                default:
                    return null;
            }
        }

        public static void CheckMD5(FileStream fs, EBinaryReader br)
        {
            //校验文件类型
            byte[] bytes = new byte[6];
            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] = br.ReadByte();
            }
            string fileType = Encoding.UTF8.GetString(bytes);
            if (fileType != "cfgdat")
            {
                throw new ApplicationException(string.Format("文件类型不一致"));
            }

            //读取文件末尾的MD5
            br.Position = (int)fs.Length - 66;
            string md5All = br.ReadString();
            string fileMd5Record = md5All.Substring(0, 32);
            br.Position = 6;

            //获取去除文件末尾的md5
            byte[] fileByte = new byte[(int)fs.Length - 66];
            fs.Position = 0;
            fs.Read(fileByte, 0, fileByte.Length);

            StringBuilder fileMd5 = new StringBuilder();
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] md5Byte = md5.ComputeHash(fileByte);
            for (int i = 0; i < md5Byte.Length; i++)
            {
                fileMd5.Append(md5Byte[i].ToString("X2"));
            }

            if (fileMd5Record != fileMd5.ToString())
            {
                throw new ApplicationException(string.Format("文件内容被篡改, MD5值不匹配"));
            }

            //数据结构校验
            string dataTypeMd5 = "@typeMd5";
            string dataTypeMd5Record = md5All.Substring(32);
            if (dataTypeMd5 != dataTypeMd5Record)
            {
                throw new ApplicationException(string.Format("ServerConfig数据读取结构不一致，请更新config.dat"));
            }
        }
    }
}
