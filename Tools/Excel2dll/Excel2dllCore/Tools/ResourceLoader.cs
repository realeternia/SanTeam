using System.IO;

namespace Excel2dllCore.Tools
{
    class ResourceLoader
    {
        public static Stream Load(string path)
        {
            System.Reflection.Assembly thisExe;
            thisExe = System.Reflection.Assembly.GetExecutingAssembly();
            System.IO.Stream file = thisExe.GetManifestResourceStream("Excel2dllCore.Template." + path);
            return file;
        }
    }
}
