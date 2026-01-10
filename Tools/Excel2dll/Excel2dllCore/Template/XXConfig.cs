@namespace
namespace CommonConfig
{
    public class @fileName
    {
@vars

        public @fileName(@parms)
        {
@construct
        }

        public @fileName() { }

        private static Dictionary<int, @fileName> config = new Dictionary<int, @fileName>();
        public static Dictionary<int, @fileName>.ValueCollection ConfigList
        {
            get
            {
                return config.Values;
            }
        }

        public static void Refresh(Dictionary<int, @fileName> dict)
        {
            config.Clear();
            config = dict;
        }

        public static void Load()
        {
            config.Clear();
@loadm
        }

        public static @fileName GetConfig(int id)
        {
            @fileName data;
            if (config.TryGetValue(id, out data))
            {
                return data;
            }
            throw new NullReferenceException(string.Format("配置表@fileName不存在id={0}", id));
        }

        public static bool HasConfig(int id)
        {
            if (config.ContainsKey(id))
            {
                return true;
            }
            return false;
        }

        public static void Assign(int id, @fileName configData)
        {
            config[id] = configData; 
        }

        public static void Add(int id, @fileName configData)
        {
            if (!config.ContainsKey(id))
            {
                config.Add(id, configData);
            }
        }

        public static void Remove(int id)
        {
            if (config.ContainsKey(id))
            {
                config.Remove(id);
            }
        }
    }
}
