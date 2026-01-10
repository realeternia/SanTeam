@namespace
namespace CommonConfig
{
    public class @fileName
    {
@vars
        private static Dictionary<uint, @fileName> config = new Dictionary<uint, @fileName>();
@dict
        public @fileName(@parms)
        {
@construct
        }

        public @fileName() { }

        public static Dictionary<uint, @fileName>.ValueCollection ConfigList
        {
            get
            {
                return config.Values;
            }
        }

        public static void Refresh(Dictionary<uint, @fileName> dict@funcParam)
        {
            config.Clear();
            config = dict;
@refresh
        }

        public static @fileName GetConfig(uint id)
        {
            @fileName data;
            if (config.TryGetValue(id, out data))
            {
                return data;
            }
            throw new NullReferenceException(string.Format("配置表@fileName不存在id={0}", id));
        }

@func
        public static bool HasConfig(uint id)
        {
            if (config.ContainsKey(id))
            {
                return true;
            }
            return false;
        }

        public static void Assign(uint id, @fileName configData)
        {
            config[id] = configData;
        }

        public static void Add(uint id, @fileName configData)
        {
            if (!config.ContainsKey(id))
            {
                config.Add(id, configData);
            }
        }

        public static void Remove(uint id)
        {
            if (config.ContainsKey(id))
            {
                config.Remove(id);
            }
        }
    }
}
