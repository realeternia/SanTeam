@namespace
namespace CommonConfig
{
    public class @fileName
    {
@vars
@dict
        public @fileName(@parms)
        {
@construct
        }

        public @fileName() { }

        public static List<@fileName> ConfigList { get { return configList; } }

        public static void Refresh(@funcParam)
        {
@refresh
        }

@getConfig
@hasConfig
    }
}