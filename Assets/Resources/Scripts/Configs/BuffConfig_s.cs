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
        public int Id;
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
        /// <summary>
        ///图标
        /// </summary>
        public string Icon;


        public BuffConfig(int Id, string Name, string ScriptName, string BuffEffect, string Icon)
        {
            this.Id = Id;
            this.Name = Name;
            this.ScriptName = ScriptName;
            this.BuffEffect = BuffEffect;
            this.Icon = Icon;

        }

        public BuffConfig() { }

        private static Dictionary<int, BuffConfig> config = new Dictionary<int, BuffConfig>();
        public static Dictionary<int, BuffConfig>.ValueCollection ConfigList
        {
            get
            {
                return config.Values;
            }
        }

        public static void Refresh(Dictionary<int, BuffConfig> dict)
        {
            config.Clear();
            config = dict;
        }

        public static void Load()
        {
            config.Clear();
            config[300001] = new BuffConfig(300001, "护盾", "BuffShield", "ShieldSoftBlue", "spinattack");

        }

        public static BuffConfig GetConfig(int id)
        {
            BuffConfig data;
            if (config.TryGetValue(id, out data))
            {
                return data;
            }
            throw new NullReferenceException(string.Format("配置表BuffConfig不存在id={0}", id));
        }

        public static bool HasConfig(int id)
        {
            if (config.ContainsKey(id))
            {
                return true;
            }
            return false;
        }

        public static void Assign(int id, BuffConfig configData)
        {
            config[id] = configData; 
        }

        public static void Add(int id, BuffConfig configData)
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
