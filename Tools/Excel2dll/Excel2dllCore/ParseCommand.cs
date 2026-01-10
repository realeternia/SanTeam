namespace Excel2dllCore
{
    //解析输入的命令参数
    public static class ParseCommand
    {
        public static string ConfigPath;              //Config文件夹
        public static string OutPath { get; set; } //*.cs文件的输出目录，默认当前目录
        public static bool IsClient;
        public static bool IsServer;
        public static bool IsAll;
        public static bool IsNoOutput;
        public static bool Quiet;               //安静模式，控制台不打印Debug日志
        public static bool MemMerge = true;               //在内存中merge
        public static string CodeLanguage = "cs";
        public static string LocalLanguage = string.Empty;
        public static bool CheckChange;
        public static string ChangeFile;

        public static bool Parse(string[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "-config" && i + 1 < args.Length)
                {
                    i++;
                    ConfigPath = args[i];
                }
                else if (args[i] == "-out" && i + 1 < args.Length)
                {
                    i++;
                    OutPath = args[i];
                }
                else if (args[i] == "-lan" && i + 1 < args.Length)
                {
                    i++;
                    CodeLanguage = args[i];
                }
                else if (args[i] == "-localLan" && i + 1 < args.Length)
                {
                    i++;
                    LocalLanguage = args[i];
                }
                else if (args[i].ToLower() == "-c")
                {
                    IsClient = true;
                }
                else if (args[i].ToLower() == "-s")
                {
                    IsServer = true;
                }
                else if (args[i].ToLower() == "-a")
                {
                    IsAll = true;
                }
                else if (args[i].ToLower() == "-check" && i + 1 < args.Length)
                {
                    i++;
                    CheckChange = true;
                    ChangeFile = args[i];
                }
                else if (args[i].ToLower() == "-n")
                {
                    IsNoOutput = true;
                }
                else if (args[i].ToLower() == "-mergefile")//生成中间过程文件
                {
                    MemMerge = false;
                }
                else if (args[i].ToLower() == "-quiet" || args[i].ToLower() == "-q")
                {
                    Quiet = true;
                }
                else
                {
                    return false;
                }
            }

            return true;
        }
    }
}
