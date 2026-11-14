using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace SkillBaseTool
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            using (var form = new MainForm())
            {
                form.Icon = LoadAppIcon();
                Application.Run(form);
            }
        }

        private static Icon LoadAppIcon()
        {
            using (Stream stream = typeof(Program).Assembly.GetManifestResourceStream("SkillBaseTool.SkillBaseTool.ico"))
            {
                if (stream != null)
                {
                    return new Icon(stream, SystemInformation.IconSize);
                }
            }
            return Icon.ExtractAssociatedIcon(Application.ExecutablePath);
        }
    }
}
