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
            {"Des", new FieldMetaInfo("效果说明", "string", 155)},
            {"Quality", new FieldMetaInfo("品质", "int", 60)},
            {"Effect", new FieldMetaInfo("效果", "string", 0)},
            {"Attrs", new FieldMetaInfo("属性加成", "string", 210)},
            {"MainAttr", new FieldMetaInfo("主属性（该道具的主导属性名，取值见 HeroAttrConfig.name，如 atk/hp/ap/armor/magicres/atkspeed/crit/mpRegen；玩家级道具用其自身属性名，无属性留空）", "string", 0)},
            {"Skills", new FieldMetaInfo("技能", "int[]", 0)},
            {"LimitSkillSname", new FieldMetaInfo("使用限制技能（空=不限；目标英雄需属于该技能的好友羁绊组）", "string", 0)},
            {"RemoveWhenUse", new FieldMetaInfo("使用后消失", "bool", 0)},
            {"CombineId", new FieldMetaInfo("合成目标", "int", 60)},
            {"CombineNeed", new FieldMetaInfo("合成数量", "int", 60)},
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
        ///属性加成（Attrs 格式：逗号分隔多条 "attr+value,attr+value"；比例属性如攻速/暴击直接配比例，0.1=+10%；禁止用空格分隔）
        /// </summary>
        public string Attrs;
        /// <summary>
        ///主属性（该道具的主导属性名，取值见 HeroAttrConfig.name，如 atk/hp/ap/armor/magicres/atkspeed/crit/mpRegen；玩家级道具用其自身属性名，无属性留空）
        /// </summary>
        public string MainAttr;
        /// <summary>
        ///技能
        /// </summary>
        public int[] Skills;
        /// <summary>
        ///使用限制技能（空=不限；仅可对属于该技能好友羁绊组的英雄卡使用）
        /// </summary>
        public string LimitSkillSname;
        /// <summary>
        ///使用后消失
        /// </summary>
        public bool RemoveWhenUse;
        /// <summary>
        ///合成目标（0=不参与合成；达到合成数量时自动合成1个该道具）
        /// </summary>
        public int CombineId;
        /// <summary>
        ///合成数量（合成1个目标所需的本道具数量）
        /// </summary>
        public int CombineNeed;
        /// <summary>
        ///背景图
        /// </summary>
        public string Icon;


        public ItemConfig(int Id, string Name, string Des, int Quality, string Effect, string Attrs, string MainAttr, int[] Skills, string LimitSkillSname, bool RemoveWhenUse, int CombineId, int CombineNeed, string Icon)
        {
            this.Id = Id;
            this.Name = Name;
            this.Des = Des;
            this.Quality = Quality;
            this.Effect = Effect;
            this.Attrs = Attrs;
            this.MainAttr = MainAttr;
            this.Skills = Skills;
            this.LimitSkillSname = LimitSkillSname;
            this.RemoveWhenUse = RemoveWhenUse;
            this.CombineId = CombineId;
            this.CombineNeed = CombineNeed;
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
            config[400001] = new ItemConfig(400001, "关王刀", "攻击+30，护甲+30", 2, "attr", "atk+30,armor+30", "atk", new int[0], "", false, 0, 0, "guanwangdao");
            config[400002] = new ItemConfig(400002, "方天画戟", "攻击+30，暴击+22%", 2, "attr", "atk+30,crit+0.22", "atk", new int[0], "", false, 0, 0, "fangtian");
            config[400003] = new ItemConfig(400003, "丈八蛇矛", "攻击+30，生命+250", 2, "attr", "atk+30,hp+250", "atk", new int[0], "", false, 0, 0, "zhangba");
            config[400007] = new ItemConfig(400007, "孙子兵法", "法强+15，攻击+20", 2, "attr", "ap+15,atk+20", "ap", new int[0], "", false, 0, 0, "sunzi");
            config[400008] = new ItemConfig(400008, "诗经", "法强+15，生命+250", 2, "attr", "ap+15,hp+250", "ap", new int[0], "", false, 0, 0, "shijing");
            config[402009] = new ItemConfig(402009, "李广弓", "攻速+10%，法力回复+2", 2, "attr", "atkspeed+0.1,mpRegen+2", "atkspeed", new int[0], "", false, 0, 0, "gong2");
            config[402010] = new ItemConfig(402010, "养由基弓", "攻速+15%，生命+250", 2, "attr", "atkspeed+0.15,hp+250", "atkspeed", new int[0], "", false, 0, 0, "gong3");
            config[400011] = new ItemConfig(400011, "易经", "法强+10，法力回复+2", 2, "attr", "ap+10,mpRegen+2", "ap", new int[0], "", false, 0, 0, "yijing");
            config[400012] = new ItemConfig(400012, "道德经", "法强+15，魔法抗性+30", 2, "attr", "ap+15,magicres+30", "ap", new int[0], "", false, 0, 0, "daode");
            config[400013] = new ItemConfig(400013, "赤兔马", "攻速+15%，魔法抗性+30", 2, "attr", "atkspeed+0.15,magicres+30", "atkspeed", new int[0], "", false, 0, 0, "chitu");
            config[400014] = new ItemConfig(400014, "的卢马", "生命+250，生命回复+2", 2, "attr", "hp+250,hpRegen+2", "hp", new int[0], "", false, 0, 0, "dilu");
            config[400015] = new ItemConfig(400015, "长生镜", "生命+150", 1, "attr", "hp+150", "hp", new int[0], "", false, 0, 0, "jingzi");
            config[400016] = new ItemConfig(400016, "飞羽甲", "护甲+30，攻速+15%", 2, "attr", "armor+30,atkspeed+0.15", "armor", new int[0], "", false, 0, 0, "jia3");
            config[400017] = new ItemConfig(400017, "明光铠", "护甲+30，魔抗+30", 2, "attr", "armor+30,magicres+30", "armor", new int[0], "", false, 0, 0, "jia2");
            config[400018] = new ItemConfig(400018, "兽面吞头铠", "护甲+30，暴击+22%", 2, "attr", "armor+30,crit+0.22", "armor", new int[0], "", false, 0, 0, "jia4");
            config[400019] = new ItemConfig(400019, "八卦袍", "法强+15，护甲+30", 2, "attr", "ap+15,armor+30", "ap", new int[0], "", false, 0, 0, "pao1");
            config[400020] = new ItemConfig(400020, "爪电飞黄", "护甲+30，生命+250", 2, "attr", "armor+30,hp+250", "armor", new int[0], "", false, 0, 0, "ma3");
            config[400021] = new ItemConfig(400021, "穿云弓", "攻速+15%，暴击+22%", 2, "attr", "atkspeed+0.15,crit+0.22", "atkspeed", new int[0], "", false, 0, 0, "gong4");
            config[400022] = new ItemConfig(400022, "白玉环", "暴击+22%，魔抗+30", 2, "attr", "crit+0.22,magicres+30", "crit", new int[0], "", false, 0, 0, "huan1");
            config[400023] = new ItemConfig(400023, "玉龙壁", "魔抗+30，每秒回复2点法力", 2, "attr", "magicres+30,mpRegen+2", "magicres", new int[0], "", false, 0, 0, "huan2");
            config[400024] = new ItemConfig(400024, "绝影", "生命+250，魔抗+30", 2, "attr", "hp+250,magicres+30", "hp", new int[0], "", false, 0, 0, "ma2");
            config[400025] = new ItemConfig(400025, "古锭刀", "攻击+20，每秒回复2点法力", 2, "attr", "atk+20,mpRegen+2", "atk", new int[0], "", false, 0, 0, "jian2");
            config[401012] = new ItemConfig(401012, "万民书", "护甲+1，魔法抗性+1（仅限拥有「仁」的英雄）", 1, "tpattr", "armor+1,magicres+1", "armor", new int[0], "仁", true, 401013, 5, "wanming1");
            config[401013] = new ItemConfig(401013, "万民书·精", "护甲+5，魔法抗性+5（仅限拥有「仁」的英雄）", 2, "tpattr", "armor+5,magicres+5", "armor", new int[0], "仁", true, 0, 0, "wanming2");
            config[401014] = new ItemConfig(401014, "文赋", "法术强度+1，魔法抗性+1", 1, "tpattr", "ap+1,magicres+1", "ap", new int[0], "", true, 401015, 5, "wenfu1");
            config[401015] = new ItemConfig(401015, "文赋·精", "法术强度+5，魔法抗性+5", 2, "tpattr", "ap+5,magicres+5", "ap", new int[0], "", true, 0, 0, "wenfu2");
            config[402001] = new ItemConfig(402001, "长刀", "攻击+10", 1, "attr", "atk+10", "atk", new int[0], "", false, 0, 0, "jian1");
            config[402002] = new ItemConfig(402002, "皮革甲", "护甲+20", 1, "attr", "armor+20", "armor", new int[0], "", false, 0, 0, "jia1");
            config[402003] = new ItemConfig(402003, "檀木弓", "攻速+10%", 1, "attr", "atkspeed+0.1", "atkspeed", new int[0], "", false, 0, 0, "tanmugong");
            config[402004] = new ItemConfig(402004, "斗篷", "魔抗+20", 1, "attr", "magicres+20", "magicres", new int[0], "", false, 0, 0, "doupeng");
            config[402005] = new ItemConfig(402005, "葫芦", "每秒回复1点法力", 1, "attr", "mpRegen+1", "mpRegen", new int[0], "", false, 0, 0, "hulu");
            config[402006] = new ItemConfig(402006, "护手", "暴击+15%", 1, "attr", "crit+0.15", "crit", new int[0], "", false, 0, 0, "hushou");
            config[402007] = new ItemConfig(402007, "羽扇", "法强+10", 1, "attr", "ap+10", "ap", new int[0], "", false, 0, 0, "yushan");
            config[402008] = new ItemConfig(402008, "名马", "生命+150", 1, "attr", "hp+150", "hp", new int[0], "", false, 0, 0, "ma1");
            config[409001] = new ItemConfig(409001, "火尖枪", "攻击+20", 5, "attr", "atk+20", "atk", new int[0], "", false, 0, 0, "huojianqiang");
            config[409002] = new ItemConfig(409002, "聚宝盆", "每年额外获得2金币", 5, "pattr", "roundgold+2", "", new int[0], "", false, 0, 0, "jubaopeng");
            config[409003] = new ItemConfig(409003, "虎王重甲", "士兵生命+100", 5, "pattr", "shp+100", "", new int[0], "", false, 0, 0, "armor");
            config[409004] = new ItemConfig(409004, "玉如意", "出售卡牌多获得25%金币", 5, "sellhigh", "", "", new int[0], "", false, 0, 0, "ruyi");
            config[409005] = new ItemConfig(409005, "酒", "攻击+10，法强+10", 5, "attr", "atk+10,ap+10", "atk", new int[0], "", false, 0, 0, "jiu");

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
