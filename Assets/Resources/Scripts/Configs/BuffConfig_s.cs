using System;
using System.Collections;
using System.Collections.Generic;

namespace CommonConfig
{
    public class BuffConfig
    {
        /// <summary>
        ///序列
        /// </summary>
        public uint Id;
        /// <summary>
        ///名字
        /// </summary>
        public string Name;
        /// <summary>
        ///脚本名
        /// </summary>
        public string ScriptName;
        /// <summary>
        ///hit
        /// </summary>
        public string BuffEffect;


        public BuffConfig(uint Id, string Name, string ScriptName, string BuffEffect)
        {
            this.Id = Id;
            this.Name = Name;
            this.ScriptName = ScriptName;
            this.BuffEffect = BuffEffect;

        }

        public BuffConfig() { }

        private static Dictionary<uint, BuffConfig> config = new Dictionary<uint, BuffConfig>();
        public static Dictionary<uint, BuffConfig>.ValueCollection ConfigList
        {
            get
            {
                return config.Values;
            }
        }

        public static void Refresh(Dictionary<uint, BuffConfig> dict)
        {
            config.Clear();
            config = dict;
        }

        public static void Load()
        {
            config.Clear();
            config[300001] = new BuffConfig(300001, "护盾", "BuffShield", "ShieldSoftBlue");

        }

        public static BuffConfig GetConfig(uint id)
        {
            BuffConfig data;
            if (config.TryGetValue(id, out data))
            {
                return data;
            }
            throw new NullReferenceException(string.Format("配置表BuffConfig不存在id={0}", id));
        }

        public static bool HasConfig(uint id)
        {
            if (config.ContainsKey(id))
            {
                return true;
            }
            return false;
        }

        public static void Assign(uint id, BuffConfig configData)
        {
            config[id] = configData; 
        }

        public static void Add(uint id, BuffConfig configData)
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
