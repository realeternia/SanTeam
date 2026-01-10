using System.Collections.Generic;
using System.IO;
using System.Text;
using Excel2dllCore.Load;

namespace Excel2dllCore.Tools
{
    internal class Utils
    {

        //等同.net4.0的string.IsNullOrWhiteSpace()，因为之前版本没有
        public static bool IsNullOrWhiteSpace(string value)
        {
            if (value != null)
            {
                for (int i = 0; i < value.Length; i++)
                {
                    if (!char.IsWhiteSpace(value[i]))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        //等同.net4.0的string.Join()，因为之前版本没有
        public static string Join(/*this string t, */ string separator, List<string> value)
        {
            string ret = "";
            foreach (var str in value)
            {
                ret = ret + str + separator;
            }
            char[] charsToTrim = separator.ToCharArray();
            ret = ret.TrimEnd(charsToTrim);
            return ret;
        }

        public static string CheckNumberArrayStr(string str)
        {
            StringBuilder sb = new StringBuilder(str.Length);
            char c;
            for (int i = 0; i < str.Length; ++i)
            {
                c = str[i];
                if (!char.IsWhiteSpace(c))
                    sb.Append(c);
            }
            if (sb.ToString().EndsWith(".") || sb.ToString().EndsWith(";"))
            {
                sb.Remove(sb.Length - 1, 1);
            }
            return sb.ToString();
        }

        /// <summary>
        /// 获取path目录下的所有配置文件，这些Excel都需要生成对应的.cs文件
        /// </summary>
        /// <param name="path">路径</param>
        public static List<string> GetAllConfigFiles(string path, string fileExtension)
        {
            if (!Directory.Exists(path))
            {
                Logger.Debug("\tCan't find directory: " + path);
                return null;
            }
            DirectoryInfo folder = new DirectoryInfo(path);
            List<string> files = new List<string>();
            // 目前只处理当前文件夹下的非隐藏文件，不会递归处理子文件夹
            foreach (FileInfo fileInfo in folder.GetFiles())
            {
                // 不处理隐藏文件
                if ((fileInfo.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden)
                {
                    Logger.Debug("\tSkip hidden file: " + fileInfo);
                }
                else if (fileInfo.Name.Length == 0 || fileInfo.Name[0] == '~')
                {
                    Logger.Debug("\tSkip ~ file: " + fileInfo);
                }
                else
                {
                    if (fileInfo.Extension.ToLower().StartsWith(fileExtension))
                    {
                        files.Add(fileInfo.FullName);
                    }
                }
            }
            return files;
        }


        public static string LowerFirst(string s)
        {
            var first = s.Substring(0, 1).ToLower();
            return first + s.Substring(1);
        }

        //复制文件夹
        public static void CopyFolder(string from, string to)
        {
            if (!Directory.Exists(to))
                Directory.CreateDirectory(to);

            // 子文件夹
            foreach (string sub in Directory.GetDirectories(from))
            {
                if(!sub.EndsWith(".svn"))   //过滤掉SVN文件，不然在打开SVN时无法删除该文件
                    CopyFolder(sub + "/", to + Path.GetFileName(sub) + "/");
            }
                
            // 文件
            foreach (string file in Directory.GetFiles(from))
            {
                File.Copy(file, to + Path.GetFileName(file), true);
            }              
        }

        //删除文件夹
        public static void DeleteFolder(string path)
        {
            //if (Directory.Exists(path))
            //{
            //    DirectoryInfo dir = new DirectoryInfo(path);
            //    dir.Delete(true);
            //}
        }

    }
}
