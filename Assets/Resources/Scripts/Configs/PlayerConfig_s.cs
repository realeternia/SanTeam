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
        ///ban强卡
        /// </summary>
        public bool Banstrongcard;
        /// <summary>
        ///ban弱卡
        /// </summary>
        public bool Banweakcard;
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
        ///拿先手
        /// </summary>
        public float PickFirst;
        /// <summary>
        ///好友因子
        /// </summary>
        public float FriendFactor;
        /// <summary>
        ///拿士兵强化
        /// </summary>
        public float PickSoldierUp;
        /// <summary>
        ///英雄花钱比警戒线
        /// </summary>
        public float HeroGoldRate;
        /// <summary>
        ///道具花钱比警戒线
        /// </summary>
        public float ItemGoldRate;
        /// <summary>
        ///卡牌风险把控
        /// </summary>
        public float OwnTooMuchCardRate;
        /// <summary>
        ///需求
        /// </summary>
        public string[] Cardsneed;


        public PlayerConfig(int Id, string Name, string Imgpath, string Colorstr, bool Banstrongcard, bool Banweakcard, int Pricelower, int Priceupper, float Priceoutrate, float sameCardRate, int Cardherolimit, int Carditemlimit, float Futurerate, float Findmasterrate, int Pickside, float PickFirst, float FriendFactor, float PickSoldierUp, float HeroGoldRate, float ItemGoldRate, float OwnTooMuchCardRate, string[] Cardsneed)
        {
            this.Id = Id;
            this.Name = Name;
            this.Imgpath = Imgpath;
            this.Colorstr = Colorstr;
            this.Banstrongcard = Banstrongcard;
            this.Banweakcard = Banweakcard;
            this.Pricelower = Pricelower;
            this.Priceupper = Priceupper;
            this.Priceoutrate = Priceoutrate;
            this.sameCardRate = sameCardRate;
            this.Cardherolimit = Cardherolimit;
            this.Carditemlimit = Carditemlimit;
            this.Futurerate = Futurerate;
            this.Findmasterrate = Findmasterrate;
            this.Pickside = Pickside;
            this.PickFirst = PickFirst;
            this.FriendFactor = FriendFactor;
            this.PickSoldierUp = PickSoldierUp;
            this.HeroGoldRate = HeroGoldRate;
            this.ItemGoldRate = ItemGoldRate;
            this.OwnTooMuchCardRate = OwnTooMuchCardRate;
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
            config[1] = new PlayerConfig(1, "旺仔", "PlayerPic/wang", "#00FF00", false, false, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, null);
            config[2] = new PlayerConfig(2, "甲鱼", "PlayerPic/jiayu", "#333333", true, false, 6, 16, 0.1f, 5f, 7, 4, 0.6f, 1f, 0, 0.5f, 1f, 0.5f, 0.85f, 0.25f, 0.7f, new string[]{"\"def\"，\"2\"，\"inte\"，\"1\"，\"shoot\"，\"1\""});
            config[3] = new PlayerConfig(3, "三哥", "PlayerPic/sange", "#FFFFFF", false, false, 18, 26, 0.3f, 3f, 8, 7, 0.5f, 1f, 0, 0.5f, 1f, 0.5f, 0.8f, 0.3f, 0.7f, new string[]{"\"atk\"，\"1\"，\"def\"，\"1\"，\"inte\"，\"1\"，\"shoot\"，\"1\""});
            config[4] = new PlayerConfig(4, "魔童", "PlayerPic/nezha", "#8C0000", false, true, 18, 26, 0.3f, 3f, 7, 6, 0.5f, 1f, 0, 1f, 1f, 0.5f, 0.85f, 0.25f, 0.7f, new string[]{"\"inte\"，\"3\""});
            config[5] = new PlayerConfig(5, "八戒", "PlayerPic/bajie", "#FFCC99", true, false, 18, 26, 0.3f, 3f, 8, 6, 0.28f, 1f, 0, 0.5f, 1f, 0.5f, 0.85f, 0.25f, 0.85f, new string[]{"\"atk\"，\"1\"，\"def\"，\"1\"，\"inte\"，\"1\"，\"shoot\"，\"1\""});
            config[6] = new PlayerConfig(6, "大虎", "PlayerPic/dahu", "#006633", false, true, 21, 30, 0.1f, 5f, 6, 4, 0.525f, 1f, 0, 3f, 1.2f, 1f, 0.8f, 0.35f, 0.5f, new string[]{"\"shoot\"，\"2\"，\"def\"，\"2\""});
            config[7] = new PlayerConfig(7, "蓝猫", "PlayerPic/mao", "#5555FF", false, true, 18, 26, 0.3f, 3f, 7, 6, 0.5f, 2.5f, 2, 2f, 0.5f, 0.5f, 0.84f, 0.23f, 0.9f, new string[]{"\"atk\"，\"1\"，\"def\"，\"1\"，\"inte\"，\"1\""});
            config[8] = new PlayerConfig(8, "巴爸", "PlayerPic/baba", "#FF73FF", false, false, 17, 23, 0.3f, 3f, 7, 6, 0.28f, 1f, 0, 1f, 1.5f, 2f, 0.95f, 0.13f, 0.7f, new string[]{"\"shoot\"，\"1\"，\"help\"，\"2\"，\"def\"，\"1\""});
            config[9] = new PlayerConfig(9, "巴妈", "PlayerPic/bama", "#333333", false, false, 18, 26, 0.3f, 3f, 8, 6, 0.35f, 1f, 0, 1f, 1.2f, 1.5f, 0.95f, 0.12f, 0.85f, new string[]{"\"inte\"，\"1\"，\"help\"，\"1\"，\"def\"，\"2\""});
            config[11] = new PlayerConfig(11, "小红", "PlayerPic/xiaohong", "#FF3333", false, true, 18, 26, 0.3f, 3f, 7, 4, 0.5f, 2.5f, 3, 2f, 0.5f, 0.5f, 0.85f, 0.2f, 0.9f, new string[]{"\"atk\"，\"1\"，\"def\"，\"1\"，\"inte\"，\"1\"，\"shoot\"，\"1\""});
            config[12] = new PlayerConfig(12, "电怪", "PlayerPic/picaqiu", "#FFFF00", false, true, 18, 26, 0.3f, 3f, 6, 4, 0.5f, 3f, 1, 2f, 0.5f, 0.5f, 0.85f, 0.2f, 0.9f, new string[]{"\"atk\"，\"1\"，\"def\"，\"1\"，\"inte\"，\"2\""});

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
