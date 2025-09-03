using System;
using System.Collections;
using System.Collections.Generic;

namespace CommonConfig
{
    public class PlayerConfig
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
        ///图标
        /// </summary>
        public string Imgpath;
        /// <summary>
        ///颜色
        /// </summary>
        public string Colorstr;
        /// <summary>
        ///低价区间
        /// </summary>
        public int Pricelower;
        /// <summary>
        ///高价区间
        /// </summary>
        public int Priceupper;
        /// <summary>
        ///区间外折扣
        /// </summary>
        public float Priceoutrate;
        /// <summary>
        ///同卡倍率
        /// </summary>
        public float sameCardRate;
        /// <summary>
        ///hero卡数量
        /// </summary>
        public int Cardherolimit;
        /// <summary>
        ///物品卡数量
        /// </summary>
        public int Carditemlimit;
        /// <summary>
        ///看未来
        /// </summary>
        public float Futurerate;
        /// <summary>
        ///找core概率
        /// </summary>
        public float Findmasterrate;
        /// <summary>
        ///阵营only
        /// </summary>
        public int Pickside;
        /// <summary>
        ///ban强卡
        /// </summary>
        public bool Banstrongcard;
        /// <summary>
        ///ban弱卡
        /// </summary>
        public bool Banweakcard;
        /// <summary>
        ///需求
        /// </summary>
        public string[] Cardsneed;


        public PlayerConfig(int Id, string Name, string Imgpath, string Colorstr, int Pricelower, int Priceupper, float Priceoutrate, float sameCardRate, int Cardherolimit, int Carditemlimit, float Futurerate, float Findmasterrate, int Pickside, bool Banstrongcard, bool Banweakcard, string[] Cardsneed)
        {
            this.Id = Id;
            this.Name = Name;
            this.Imgpath = Imgpath;
            this.Colorstr = Colorstr;
            this.Pricelower = Pricelower;
            this.Priceupper = Priceupper;
            this.Priceoutrate = Priceoutrate;
            this.sameCardRate = sameCardRate;
            this.Cardherolimit = Cardherolimit;
            this.Carditemlimit = Carditemlimit;
            this.Futurerate = Futurerate;
            this.Findmasterrate = Findmasterrate;
            this.Pickside = Pickside;
            this.Banstrongcard = Banstrongcard;
            this.Banweakcard = Banweakcard;
            this.Cardsneed = Cardsneed;

        }

        public PlayerConfig() { }

        private static Dictionary<int, PlayerConfig> config = new Dictionary<int, PlayerConfig>();
        public static Dictionary<int, PlayerConfig>.ValueCollection ConfigList
        {
            get
            {
                return config.Values;
            }
        }

        public static void Refresh(Dictionary<int, PlayerConfig> dict)
        {
            config.Clear();
            config = dict;
        }

        public static void Load()
        {
            config.Clear();
            config[1] = new PlayerConfig(1, "旺仔", "PlayerPic/wang", "", 0, 0, 0, 0, 0, 0, 0, 0, 0, false, false, null);
            config[2] = new PlayerConfig(2, "甲鱼", "PlayerPic/jiayu", "#333333", 6, 16, 0.1f, 5f, 7, 4, 0.6f, 1f, 0, true, false, new string[]{"\"atk\"，\"1\"，\"def\"，\"1\"，\"inte\"，\"1\""});
            config[3] = new PlayerConfig(3, "三哥", "PlayerPic/sange", "#FFFFFF", 18, 26, 0.3f, 3f, 8, 7, 0.3f, 1f, 0, false, false, new string[]{"\"atk\"，\"1\"，\"def\"，\"1\"，\"inte\"，\"1\""});
            config[4] = new PlayerConfig(4, "魔童", "PlayerPic/nezha", "#8C0000", 18, 26, 0.3f, 3f, 7, 6, 0.3f, 1f, 0, false, true, new string[]{"\"atk\"，\"1\"，\"def\"，\"1\"，\"inte\"，\"1\""});
            config[5] = new PlayerConfig(5, "八戒", "PlayerPic/bajie", "#FFCC99", 18, 26, 0.3f, 3f, 8, 6, 0.1f, 1f, 0, true, false, new string[]{"\"shoot\"，\"1\"，\"def\"，\"1\"，\"inte\"，\"1\""});
            config[6] = new PlayerConfig(6, "大虎", "PlayerPic/dahu", "#006633", 21, 30, 0.1f, 5f, 6, 4, 0.5f, 1f, 0, false, true, new string[]{"\"shoot\"，\"2\"，\"def\"，\"1\"，\"inte\"，\"1\""});
            config[7] = new PlayerConfig(7, "蓝猫", "PlayerPic/mao", "#5555FF", 18, 26, 0.3f, 3f, 7, 6, 0.3f, 2.5f, 2, false, true, new string[]{"\"atk\"，\"1\"，\"def\"，\"1\"，\"inte\"，\"1\""});
            config[8] = new PlayerConfig(8, "巴爸", "PlayerPic/baba", "#FF73FF", 17, 23, 0.3f, 3f, 8, 6, 0.1f, 1f, 0, false, false, new string[]{"\"shoot\"，\"2\"，\"help\"，\"2\""});
            config[9] = new PlayerConfig(9, "巴妈", "PlayerPic/bama", "#333333", 18, 26, 0.3f, 3f, 9, 6, 0.25f, 1f, 0, false, false, new string[]{"\"inte\"，\"1\"，\"help\"，\"1\"，\"shoot\"，\"1\""});
            config[11] = new PlayerConfig(11, "小红", "PlayerPic/xiaohong", "#FF3333", 18, 26, 0.3f, 3f, 7, 4, 0.3f, 2.5f, 3, false, true, new string[]{"\"atk\"，\"1\"，\"def\"，\"1\"，\"inte\"，\"1\""});
            config[12] = new PlayerConfig(12, "电怪", "PlayerPic/picaqiu", "#FFFF00", 18, 26, 0.3f, 3f, 6, 4, 0.4f, 3f, 1, false, true, new string[]{"\"atk\"，\"1\"，\"def\"，\"1\"，\"inte\"，\"1\""});

        }

        public static PlayerConfig GetConfig(int id)
        {
            PlayerConfig data;
            if (config.TryGetValue(id, out data))
            {
                return data;
            }
            throw new NullReferenceException(string.Format("配置表PlayerConfig不存在id={0}", id));
        }

        public static bool HasConfig(int id)
        {
            if (config.ContainsKey(id))
            {
                return true;
            }
            return false;
        }

        public static void Assign(int id, PlayerConfig configData)
        {
            config[id] = configData; 
        }

        public static void Add(int id, PlayerConfig configData)
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
