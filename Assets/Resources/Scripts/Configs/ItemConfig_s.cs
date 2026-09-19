using System;
using System.Collections;
using System.Collections.Generic;

namespace CommonConfig
{
    public class ItemConfig
    {
        public class FieldMetaInfo
        {
            public string fieldName;
            public string fieldType;
            public int fieldWidth;
            public string fieldRule;
            public bool fieldIndex;
            public FieldMetaInfo(string name, string type, int width = 0, string rule = "", bool index = false)
            {
                fieldName = name;
                fieldType = type;
                fieldWidth = width;
                fieldRule = rule;
                fieldIndex = index;
            }
        }

        public class CellMeta
        {
            public int row;
            public int col;
            public int? foreColor;
            public int? backColor;
            public CellMeta(int row, int col, int? foreColor, int? backColor)
            {
                this.row = row;
                this.col = col;
                this.foreColor = foreColor;
                this.backColor = backColor;
            }
        }

        private static Dictionary<string, FieldMetaInfo> fieldMeta = new Dictionary<string, FieldMetaInfo>()
        {
            {"Id", new FieldMetaInfo("序列", "int", 60)},
            {"Name", new FieldMetaInfo("名字", "string", 0)},
            {"Des", new FieldMetaInfo("效果说明", "string", 0)},
            {"Quality", new FieldMetaInfo("品质", "int", 60)},
            {"Effect", new FieldMetaInfo("效果", "string", 0)},
            {"Attrs", new FieldMetaInfo("属性加成", "string", 210)},
            {"Skills", new FieldMetaInfo("技能", "int[]", 0)},
            {"LimitSkillSname", new FieldMetaInfo("使用限制技能（空=不限；目标英雄需属于该技能的好友羁绊组）", "string", 0)},
            {"RateAbs", new FieldMetaInfo("出场概率", "int", 60)},
            {"ShopIdx", new FieldMetaInfo("出场时间", "int", 60)},
            {"AutoUse", new FieldMetaInfo("自动使用", "bool", 0)},
            {"AutoRemove", new FieldMetaInfo("自动回收", "bool", 0)},
            {"RemoveWhenUse", new FieldMetaInfo("使用后消失", "bool", 0)},
            {"StackLimit", new FieldMetaInfo("堆叠上限", "int", 60)},
            {"CombineId", new FieldMetaInfo("合成目标", "int", 60)},
            {"CombineNeed", new FieldMetaInfo("合成数量", "int", 60)},
            {"Price", new FieldMetaInfo("价格", "int", 60)},
            {"PriceRound", new FieldMetaInfo("价格回合", "float", 60)},
            {"Icon", new FieldMetaInfo("背景图", "string", 0)},
        };

        public static Dictionary<string, FieldMetaInfo> FieldMeta { get { return fieldMeta; } }

        private static List<CellMeta> cellMeta = new List<CellMeta>();
        public static List<CellMeta> CellMetas { get { return cellMeta; } }

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
        ///品质（1白 2绿 3蓝 4紫，同英雄品质）
        /// </summary>
        public int Quality;
        /// <summary>
        ///效果
        /// </summary>
        public string Effect;
        /// <summary>
        ///属性加成（Attrs 格式："attr+value,attr+value"，比例属性如攻速/暴击直接配比例，0.1=+10%）
        /// </summary>
        public string Attrs;
        /// <summary>
        ///技能
        /// </summary>
        public int[] Skills;
        /// <summary>
        ///使用限制技能（空=不限；仅可对属于该技能好友羁绊组的英雄卡使用）
        /// </summary>
        public string LimitSkillSname;
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
        ///堆叠上限（默认1；RemoveWhenUse 消耗品为99）
        /// </summary>
        public int StackLimit;
        /// <summary>
        ///合成目标（0=不参与合成；达到合成数量时自动合成1个该道具）
        /// </summary>
        public int CombineId;
        /// <summary>
        ///合成数量（合成1个目标所需的本道具数量）
        /// </summary>
        public int CombineNeed;
        /// <summary>
        ///价格
        /// </summary>
        public int Price;
        /// <summary>
        ///价格回合
        /// </summary>
        public float PriceRound;
        /// <summary>
        ///背景图
        /// </summary>
        public string Icon;


        public ItemConfig(int Id, string Name, string Des, int Quality, string Effect, string Attrs, int[] Skills, string LimitSkillSname, int RateAbs, int ShopIdx, bool AutoUse, bool AutoRemove, bool RemoveWhenUse, int StackLimit, int CombineId, int CombineNeed, int Price, float PriceRound, string Icon)
        {
            this.Id = Id;
            this.Name = Name;
            this.Des = Des;
            this.Quality = Quality;
            this.Effect = Effect;
            this.Attrs = Attrs;
            this.Skills = Skills;
            this.LimitSkillSname = LimitSkillSname;
            this.RateAbs = RateAbs;
            this.ShopIdx = ShopIdx;
            this.AutoUse = AutoUse;
            this.AutoRemove = AutoRemove;
            this.RemoveWhenUse = RemoveWhenUse;
            this.StackLimit = StackLimit;
            this.CombineId = CombineId;
            this.CombineNeed = CombineNeed;
            this.Price = Price;
            this.PriceRound = PriceRound;
            this.Icon = Icon;
        }

        public ItemConfig() { }

        private static Dictionary<int, ItemConfig> config = new Dictionary<int, ItemConfig>();
        public static Dictionary<int, ItemConfig>.ValueCollection ConfigList
        {
            get { return config.Values; }
        }

        public static void Refresh(Dictionary<int, ItemConfig> dict)
        {
            config.Clear();
            config = dict;
            RebuildIndex();
        }

        public static void Load()
        {
            config.Clear();
            config[400001] = new ItemConfig(400001, "关王刀", "", 1, "attr", "atk+10", new int[0], "", 0, 5, false, false, false, 1, 0, 0, 10, 0f, "guanwangdao");
            config[400002] = new ItemConfig(400002, "方天画戟", "", 1, "attr", "atk+15", new int[0], "", 0, 8, false, false, false, 1, 0, 0, 20, 0f, "fangtian");
            config[400003] = new ItemConfig(400003, "丈八蛇矛", "", 1, "attr", "atk+11", new int[0], "", 0, 5, false, false, false, 1, 0, 0, 11, 0f, "zhangba");
            config[400005] = new ItemConfig(400005, "大斧", "", 1, "attr", "atk+6", new int[0], "", 0, 0, false, false, false, 1, 0, 0, 5, 0f, "dafu");
            config[400006] = new ItemConfig(400006, "三丈枪", "", 1, "attr", "atk+6", new int[0], "", 0, 0, false, false, false, 1, 0, 0, 5, 0f, "sanzhangqiang");
            config[400007] = new ItemConfig(400007, "孙子兵法", "", 1, "attr", "atk+15", new int[0], "", 0, 8, false, false, false, 1, 0, 0, 20, 0f, "sunzi");
            config[400008] = new ItemConfig(400008, "墨子", "", 1, "attr", "atk+6", new int[0], "", 0, 0, false, false, false, 1, 0, 0, 5, 0f, "mozi");
            config[400009] = new ItemConfig(400009, "六韬", "", 1, "attr", "atk+10", new int[0], "", 0, 5, false, false, false, 1, 0, 0, 10, 0f, "liutao");
            config[400010] = new ItemConfig(400010, "诗经", "", 1, "attr", "ap+6", new int[0], "", 0, 0, false, false, false, 1, 0, 0, 5, 0f, "shijing");
            config[400011] = new ItemConfig(400011, "易经", "", 1, "attr", "ap+10", new int[0], "", 0, 5, false, false, false, 1, 0, 0, 10, 0f, "yijing");
            config[400012] = new ItemConfig(400012, "道德经", "", 1, "attr", "ap+15", new int[0], "", 0, 8, false, false, false, 1, 0, 0, 20, 0f, "daode");
            config[400013] = new ItemConfig(400013, "赤兔马", "", 1, "attr", "hp+75", new int[0], "", 0, 8, false, false, false, 1, 0, 0, 20, 0f, "chitu");
            config[400014] = new ItemConfig(400014, "的卢马", "", 1, "attr", "hp+50", new int[0], "", 0, 5, false, false, false, 1, 0, 0, 10, 0f, "dilu");
            config[401012] = new ItemConfig(401012, "万民书", "护甲+1，魔法抗性+1（仅限拥有「仁」的英雄）", 1, "tpattr", "armor+1,magicres+1", new int[0], "仁", 0, 6, false, false, true, 99, 401013, 5, 6, 0.4f, "wanming1");
            config[401013] = new ItemConfig(401013, "万民书·精", "护甲+5，魔法抗性+5（仅限拥有「仁」的英雄）", 2, "tpattr", "armor+5,magicres+5", new int[0], "仁", 0, 6, false, false, true, 99, 0, 0, 6, 0.4f, "wanming2");
            config[401014] = new ItemConfig(401014, "文赋", "法术强度+1，魔法抗性+1", 1, "tpattr", "ap+1,magicres+1", new int[0], "", 0, 6, false, false, true, 99, 401015, 5, 6, 0.4f, "wenfu1");
            config[401015] = new ItemConfig(401015, "文赋·精", "法术强度+5，魔法抗性+5", 2, "tpattr", "ap+5,magicres+5", new int[0], "", 0, 6, false, false, true, 99, 0, 0, 6, 0.4f, "wenfu2");
            config[402001] = new ItemConfig(402001, "长刀", "攻击+10", 1, "attr", "atk+10", new int[0], "", 0, 0, false, false, false, 1, 0, 0, 5, 0f, "jian1");
            config[402002] = new ItemConfig(402002, "皮革甲", "护甲+20", 1, "attr", "armor+20", new int[0], "", 0, 5, false, false, false, 1, 0, 0, 10, 0f, "jia1");
            config[402003] = new ItemConfig(402003, "檀木弓", "攻速+10%", 1, "attr", "atkspeed+0.1", new int[0], "", 0, 5, false, false, false, 1, 0, 0, 10, 0f, "tanmugong");
            config[402004] = new ItemConfig(402004, "斗篷", "魔抗+20", 1, "attr", "magicres+20", new int[0], "", 0, 5, false, false, false, 1, 0, 0, 10, 0f, "doupeng");
            config[402005] = new ItemConfig(402005, "葫芦", "每秒回复1点法力", 1, "attr", "mpRegen+1", new int[0], "", 0, 5, false, false, false, 1, 0, 0, 10, 0f, "hulu");
            config[402006] = new ItemConfig(402006, "护手", "暴击+15%", 1, "attr", "crit+0.15", new int[0], "", 0, 5, false, false, false, 1, 0, 0, 10, 0f, "hushou");
            config[402007] = new ItemConfig(402007, "羽扇", "法强+10", 1, "attr", "ap+10", new int[0], "", 0, 0, false, false, false, 0, 0, 0, 0, 0f, "yushan");
            config[402008] = new ItemConfig(402008, "长生镜", "生命+150", 1, "attr", "hp+150", new int[0], "", 0, 0, false, false, false, 1, 0, 0, 2, 0.24f, "jingzi");
            config[409001] = new ItemConfig(409001, "火尖枪", "", 5, "attr", "atk+15", new int[0], "", 0, 999, false, false, false, 1, 0, 0, 12, 0f, "huojianqiang");
            config[409002] = new ItemConfig(409002, "聚宝盆", "每年额外获得2金币", 5, "pattr", "roundgold+2", new int[0], "", 0, 999, false, false, false, 1, 0, 0, 12, 0f, "jubaopeng");
            config[409003] = new ItemConfig(409003, "虎王重甲", "", 5, "pattr", "shp+40", new int[0], "", 0, 999, false, false, false, 1, 0, 0, 12, 0f, "armor");
            config[409004] = new ItemConfig(409004, "玉如意", "出售卡牌多获得25%金币", 5, "sellhigh", "", new int[0], "", 0, 999, false, false, false, 1, 0, 0, 12, 0f, "ruyi");
            config[409005] = new ItemConfig(409005, "酒", "", 5, "attr", "atk+10,ap+6", new int[0], "", 0, 999, false, false, false, 1, 0, 0, 12, 0f, "jiu");

            RebuildIndex();

        }

        private static void RebuildIndex()
        {
            foreach (var kv in config)
            {
            }
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
