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
        ///启动色
        /// </summary>
        public string ColorStart;
        /// <summary>
        ///结束色
        /// </summary>
        public string ColorEnd;
        /// <summary>
        ///是否正面
        /// </summary>
        public bool IsPositive;
        /// <summary>
        ///hit
        /// </summary>
        public string BuffEffect;
        /// <summary>
        ///图标
        /// </summary>
        public string Icon;


        public BuffConfig(int Id, string Name, string ScriptName, string ColorStart, string ColorEnd, bool IsPositive, string BuffEffect, string Icon)
        {
            this.Id = Id;
            this.Name = Name;
            this.ScriptName = ScriptName;
            this.ColorStart = ColorStart;
            this.ColorEnd = ColorEnd;
            this.IsPositive = IsPositive;
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
            config[300001] = new BuffConfig(300001, "护盾", "BuffShield", "", "", true, "ShieldSoftBlue", "");
            config[300002] = new BuffConfig(300002, "减伤盾", "BuffShieldValue", "#B25900", "#FFD24D", true, "", "");
            config[300003] = new BuffConfig(300003, "吸血", "BuffSuck", "#FF0000", "#993333", true, "", "");
            config[300004] = new BuffConfig(300004, "伤害提升", "BuffDamageAddRate", "", "", true, "SparkleAreaWhite", "");
            config[300005] = new BuffConfig(300005, "攻速提升", "BuffCoolDown", "", "", true, "HeartStream", "");
            config[301001] = new BuffConfig(301001, "混乱", "BuffNoAction", "", "", false, "StunnedCirclingStarsSimple", "");
            config[301002] = new BuffConfig(301002, "连锁", "BuffLock", "", "", false, "StunnedLock", "");
            config[301003] = new BuffConfig(301003, "增伤", "BuffDamagedAddRate", "", "", false, "StunnedDamageUp", "");
            config[301004] = new BuffConfig(301004, "减速", "BuffSpeedDown", "", "", false, "SlowAuraYellow", "");
            config[301005] = new BuffConfig(301005, "陷阵", "BuffNoMove", "", "", false, "AuraSoftPurple", "");
            config[301006] = new BuffConfig(301006, "溃败", "BuffTimeDamage", "", "", false, "BloodExplosion", "");

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
