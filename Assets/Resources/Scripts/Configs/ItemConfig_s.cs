using System;
using System.Collections;
using System.Collections.Generic;

namespace CommonConfig
{
    public class ItemConfig
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
        ///效果说明
        /// </summary>
        public string Des;
        /// <summary>
        ///等级
        /// </summary>
        public int Lv;
        /// <summary>
        ///效果
        /// </summary>
        public string Effect;
        /// <summary>
        ///属性1
        /// </summary>
        public string Attr1;
        /// <summary>
        ///属性1值
        /// </summary>
        public int Attr1Val;
        /// <summary>
        ///属性2
        /// </summary>
        public string Attr2;
        /// <summary>
        ///属性2值
        /// </summary>
        public int Attr2Val;
        /// <summary>
        ///技能
        /// </summary>
        public int[] Skills;
        /// <summary>
        ///hit
        /// </summary>
        public string HitEffect;
        /// <summary>
        ///出场概率
        /// </summary>
        public int RateAbs;
        /// <summary>
        ///出场时间
        /// </summary>
        public int ShopIdx;
        /// <summary>
        ///自动使用
        /// </summary>
        public bool AutoUse;
        /// <summary>
        ///自动回收
        /// </summary>
        public bool AutoRemove;
        /// <summary>
        ///使用后消失
        /// </summary>
        public bool RemoveWhenUse;
        /// <summary>
        ///价格
        /// </summary>
        public int Price;
        /// <summary>
        ///价格回合
        /// </summary>
        public float PriceRound;
        /// <summary>
        ///只卖一个
        /// </summary>
        public bool SellOne;
        /// <summary>
        ///背景图
        /// </summary>
        public string Icon;


        public ItemConfig(int Id, string Name, string Des, int Lv, string Effect, string Attr1, int Attr1Val, string Attr2, int Attr2Val, int[] Skills, string HitEffect, int RateAbs, int ShopIdx, bool AutoUse, bool AutoRemove, bool RemoveWhenUse, int Price, float PriceRound, bool SellOne, string Icon)
        {
            this.Id = Id;
            this.Name = Name;
            this.Des = Des;
            this.Lv = Lv;
            this.Effect = Effect;
            this.Attr1 = Attr1;
            this.Attr1Val = Attr1Val;
            this.Attr2 = Attr2;
            this.Attr2Val = Attr2Val;
            this.Skills = Skills;
            this.HitEffect = HitEffect;
            this.RateAbs = RateAbs;
            this.ShopIdx = ShopIdx;
            this.AutoUse = AutoUse;
            this.AutoRemove = AutoRemove;
            this.RemoveWhenUse = RemoveWhenUse;
            this.Price = Price;
            this.PriceRound = PriceRound;
            this.SellOne = SellOne;
            this.Icon = Icon;

        }

        public ItemConfig() { }

        private static Dictionary<int, ItemConfig> config = new Dictionary<int, ItemConfig>();
        public static Dictionary<int, ItemConfig>.ValueCollection ConfigList
        {
            get
            {
                return config.Values;
            }
        }

        public static void Refresh(Dictionary<int, ItemConfig> dict)
        {
            config.Clear();
            config = dict;
        }

        public static void Load()
        {
            config.Clear();
            config[400001] = new ItemConfig(400001, "关王刀", "", 1, "attr", "str", 10, "", 0, null, "", 0, 5, false, false, false, 25, 0, false, "guanwangdao");
            config[400002] = new ItemConfig(400002, "方天画戟", "", 1, "attr", "str", 15, "", 0, null, "", 0, 8, false, false, false, 50, 0, false, "fangtian");
            config[400003] = new ItemConfig(400003, "丈八蛇矛", "", 1, "attr", "str", 11, "", 0, null, "", 0, 5, false, false, false, 28, 0, false, "zhangba");
            config[400004] = new ItemConfig(400004, "檀木弓", "", 1, "attr", "str", 6, "", 0, null, "", 0, 0, false, false, false, 12, 0, false, "tanmugong");
            config[400005] = new ItemConfig(400005, "大斧", "", 1, "attr", "str", 6, "", 0, null, "", 0, 0, false, false, false, 12, 0, false, "dafu");
            config[400006] = new ItemConfig(400006, "三丈枪", "", 1, "attr", "str", 6, "", 0, null, "", 0, 0, false, false, false, 12, 0, false, "sanzhangqiang");
            config[400007] = new ItemConfig(400007, "孙子兵法", "", 1, "attr", "lead", 15, "", 0, null, "", 0, 8, false, false, false, 50, 0, false, "sunzi");
            config[400008] = new ItemConfig(400008, "墨子", "", 1, "attr", "lead", 6, "", 0, null, "", 0, 0, false, false, false, 12, 0, false, "mozi");
            config[400009] = new ItemConfig(400009, "六韬", "", 1, "attr", "lead", 10, "", 0, null, "", 0, 5, false, false, false, 25, 0, false, "liutao");
            config[400010] = new ItemConfig(400010, "诗经", "", 1, "attr", "inte", 6, "", 0, null, "", 0, 0, false, false, false, 12, 0, false, "shijing");
            config[400011] = new ItemConfig(400011, "易经", "", 1, "attr", "inte", 10, "", 0, null, "", 0, 5, false, false, false, 25, 0, false, "yijing");
            config[400012] = new ItemConfig(400012, "道德经", "", 1, "attr", "inte", 15, "", 0, null, "", 0, 8, false, false, false, 50, 0, false, "daode");
            config[400013] = new ItemConfig(400013, "赤兔马", "", 1, "attr", "shield", 75, "", 0, null, "", 0, 8, false, false, false, 50, 0, false, "chitu");
            config[400014] = new ItemConfig(400014, "的卢马", "", 1, "attr", "shield", 50, "", 0, null, "", 0, 5, false, false, false, 25, 0, false, "dilu");
            config[400015] = new ItemConfig(400015, "大宛宝马", "", 1, "attr", "shield", 30, "", 0, null, "", 0, 0, false, false, false, 12, 0, false, "dawan");
            config[401001] = new ItemConfig(401001, "和氏璧", "先手选牌", 1, "first", "", 0, "", 0, null, "", 100, 0, true, true, false, 5, 0.6f, true, "heshi");
            config[401002] = new ItemConfig(401002, "士兵剑", "士兵攻up", 1, "sodatk", "", 3, "", 0, null, "", 40, 3, true, true, false, 25, 0, true, "jian1");
            config[401003] = new ItemConfig(401003, "士兵甲", "士兵命up", 1, "sodhp", "", 15, "", 0, null, "", 40, 3, true, true, false, 25, 0, true, "jia1");
            config[401004] = new ItemConfig(401004, "粮食", "粮食up", 1, "food", "", 10, "", 0, null, "", 45, 3, true, true, false, 25, 0, true, "food");
            config[401010] = new ItemConfig(401010, "豆腐", "武力+5", 1, "tpattr", "str", 5, "", 0, null, "", 0, 6, false, false, true, 15, 1f, true, "doufu");
            config[401011] = new ItemConfig(401011, "沙拉", "智力+5", 1, "tpattr", "inte", 5, "", 0, null, "", 0, 6, false, false, true, 15, 1f, true, "shala");
            config[401012] = new ItemConfig(401012, "烤鸭", "统帅+5", 1, "tpattr", "lead", 5, "", 0, null, "", 0, 6, false, false, true, 15, 1f, true, "kaoya");
            config[409001] = new ItemConfig(409001, "火尖枪", "", 1, "attr", "str", 15, "", 0, null, "", 0, 999, false, false, false, 30, 0, false, "huojianqiang");
            config[409002] = new ItemConfig(409002, "聚宝盆", "每年额外获得5金币", 1, "pattr", "roundgold", 5, "", 0, null, "", 0, 999, false, false, false, 30, 0, false, "jubaopeng");
            config[409003] = new ItemConfig(409003, "虎王重甲", "", 1, "pattr", "shp", 40, "", 0, null, "", 0, 999, false, false, false, 30, 0, false, "armor");
            config[409004] = new ItemConfig(409004, "玉如意", "出售卡牌多获得25%金币", 1, "sellhigh", "", 0, "", 0, null, "", 0, 999, false, false, false, 30, 0, false, "ruyi");
            config[409005] = new ItemConfig(409005, "酒", "", 1, "attr", "str", 10, "inte", 6, null, "", 0, 999, false, false, false, 30, 0, false, "jiu");

        }

        public static ItemConfig GetConfig(int id)
        {
            ItemConfig data;
            if (config.TryGetValue(id, out data))
            {
                return data;
            }
            throw new NullReferenceException(string.Format("配置表ItemConfig不存在id={0}", id));
        }

        public static bool HasConfig(int id)
        {
            if (config.ContainsKey(id))
            {
                return true;
            }
            return false;
        }

        public static void Assign(int id, ItemConfig configData)
        {
            config[id] = configData; 
        }

        public static void Add(int id, ItemConfig configData)
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
