using System;
using System.Collections;
using System.Collections.Generic;

namespace CommonConfig
{
    public class HeroConfig
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
        ///等级
        /// </summary>
        public int Lv;
        /// <summary>
        ///攻击力
        /// </summary>
        public int Atk;
        /// <summary>
        ///生命
        /// </summary>
        public int Hp;
        /// <summary>
        ///阵营
        /// </summary>
        public int Side;
        /// <summary>
        ///移动速度
        /// </summary>
        public int MoveSpeed;
        /// <summary>
        ///攻击距离
        /// </summary>
        public int Range;
        /// <summary>
        ///背景图
        /// </summary>
        public string Icon;


        public HeroConfig(uint Id, string Name, int Lv, int Atk, int Hp, int Side, int MoveSpeed, int Range, string Icon)
        {
            this.Id = Id;
            this.Name = Name;
            this.Lv = Lv;
            this.Atk = Atk;
            this.Hp = Hp;
            this.Side = Side;
            this.MoveSpeed = MoveSpeed;
            this.Range = Range;
            this.Icon = Icon;

        }

        public HeroConfig() { }

        private static Dictionary<uint, HeroConfig> config = new Dictionary<uint, HeroConfig>();
        public static Dictionary<uint, HeroConfig>.ValueCollection ConfigList
        {
            get
            {
                return config.Values;
            }
        }

        public static void Refresh(Dictionary<uint, HeroConfig> dict)
        {
            config.Clear();
            config = dict;
        }

        public static void Load()
        {
            config.Clear();
            config[100001] = new HeroConfig(100001, "赵云", 1, 30, 400, 1, 10, 15, "zhaoyun");
            config[100002] = new HeroConfig(100002, "关羽", 1, 30, 300, 1, 10, 15, "guanyu");
            config[100003] = new HeroConfig(100003, "张飞", 1, 40, 300, 1, 10, 15, "zhangfei");
            config[100004] = new HeroConfig(100004, "夏侯惇", 1, 35, 250, 2, 10, 15, "xiahoudun");
            config[100005] = new HeroConfig(100005, "夏侯渊", 1, 30, 250, 2, 10, 25, "xiahouyuan");
            config[100006] = new HeroConfig(100006, "张辽", 1, 30, 300, 2, 10, 15, "zhangliao");

        }

        public static HeroConfig GetConfig(uint id)
        {
            HeroConfig data;
            if (config.TryGetValue(id, out data))
            {
                return data;
            }
            throw new NullReferenceException(string.Format("配置表HeroConfig不存在id={0}", id));
        }

        public static bool HasConfig(uint id)
        {
            if (config.ContainsKey(id))
            {
                return true;
            }
            return false;
        }

        public static void Assign(uint id, HeroConfig configData)
        {
            config[id] = configData; 
        }

        public static void Add(uint id, HeroConfig configData)
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
