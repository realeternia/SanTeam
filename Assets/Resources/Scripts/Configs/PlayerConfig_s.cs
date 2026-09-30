using System;
using System.Collections;
using System.Collections.Generic;

namespace CommonConfig
{
    public class PlayerConfig
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
            {"Personality", new FieldMetaInfo("性格描述（备注用，游戏内不参与逻辑）", "string", 172)},
            {"Imgpath", new FieldMetaInfo("图标", "string", 194)},
            {"Colorstr", new FieldMetaInfo("颜色", "string", 0)},
            {"CanPlay", new FieldMetaInfo("是否可选", "bool", 0)},
            {"InitGold", new FieldMetaInfo("初始资金", "int", 60)},
            {"InitCards", new FieldMetaInfo("初始卡牌列表", "int[]", 0)},
            {"SameCardRate", new FieldMetaInfo("同卡倍率", "float", 60)},
            {"Cardherolimit", new FieldMetaInfo("英雄卡上限额外数", "int", 60)},
            {"Futurerate", new FieldMetaInfo("看未来", "float", 60)},
            {"LikeJob", new FieldMetaInfo("喜欢的职业Id列表(可0-4个，取 JobConfig.Id；命中即固定倍率加成，0个=不关注)", "int[]", 137)},
            {"LikeForce", new FieldMetaInfo("喜欢的势力Id列表(可0-2个，取 ForceConfig.Id/阵营side；命中即固定倍率加成，0个=不关注)", "int[]", 85)},
            {"LikeFriend", new FieldMetaInfo("喜欢的好友组Id列表(可2-6个，取 HeroFriendConfig.Id；命中即固定倍率加成，0个=不关注)", "int[]", 155)},
            {"OwnTooMuchCardRate", new FieldMetaInfo("卡牌风险把控", "float", 60)},
            {"BalanceFactor", new FieldMetaInfo("远近因子", "float", 60)},
            {"Intelligence", new FieldMetaInfo("聪明度（1-99，越高越能正确评估羁绊价值、越少手滑）", "int", 60)},
            {"AccumulatedCostBias", new FieldMetaInfo("累积购卡倾向（越大上升越快，1.0 时 target≈sqrt(year)）", "float", 60)},
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
        ///性格描述（仅备注，游戏内不参与逻辑）
        /// </summary>
        public string Personality;
        /// <summary>
        ///图标
        /// </summary>
        public string Imgpath;
        /// <summary>
        ///颜色
        /// </summary>
        public string Colorstr;
        /// <summary>
        ///是否可选
        /// </summary>
        public bool CanPlay;
        /// <summary>
        ///初始资金
        /// </summary>
        public int InitGold;
        /// <summary>
        ///初始卡牌列表
        /// </summary>
        public int[] InitCards;
        /// <summary>
        ///同卡倍率
        /// </summary>
        public float SameCardRate;
        /// <summary>
        ///英雄卡上限额外数（当前等级卡片上限基础上额外+多少）
        /// </summary>
        public int Cardherolimit;
        /// <summary>
        ///看未来
        /// </summary>
        public float Futurerate;
        /// <summary>
        ///喜欢的职业Id列表(可0-4个，取 JobConfig.Id)：候选卡命中其中任一职业时按档给固定倍率，空列表=不关注
        /// </summary>
        public int[] LikeJob;
        /// <summary>
        ///喜欢的势力Id列表(可0-2个，取 ForceConfig.Id/阵营side)：候选卡命中其中任一势力时按档给固定倍率，空列表=不关注
        /// </summary>
        public int[] LikeForce;
        /// <summary>
        ///喜欢的好友组Id列表(可2-6个，取 HeroFriendConfig.Id)：候选卡命中任一好友组时按档给固定倍率（每档加成最大），空列表=不关注
        /// </summary>
        public int[] LikeFriend;
        /// <summary>
        ///卡牌风险把控
        /// </summary>
        public float OwnTooMuchCardRate;
        /// <summary>
        ///远近因子（近战远程搭配权重，越大越讲究前后排平衡）
        /// </summary>
        public float BalanceFactor;
        /// <summary>
        ///聪明度（1-99，越高越能正确评估羁绊价值、越少手滑）
        /// </summary>
        public int Intelligence;
        /// <summary>
        ///累积购卡倾向参数（越大上升越快，1.0 时 target≈sqrt(year)）
        /// </summary>
        public float AccumulatedCostBias;


        public PlayerConfig(int Id, string Name, string Personality, string Imgpath, string Colorstr, bool CanPlay, int InitGold, int[] InitCards, float SameCardRate, int Cardherolimit, float Futurerate, int[] LikeJob, int[] LikeForce, int[] LikeFriend, float OwnTooMuchCardRate, float BalanceFactor, int Intelligence, float AccumulatedCostBias)
        {
            this.Id = Id;
            this.Name = Name;
            this.Personality = Personality;
            this.Imgpath = Imgpath;
            this.Colorstr = Colorstr;
            this.CanPlay = CanPlay;
            this.InitGold = InitGold;
            this.InitCards = InitCards;
            this.SameCardRate = SameCardRate;
            this.Cardherolimit = Cardherolimit;
            this.Futurerate = Futurerate;
            this.LikeJob = LikeJob;
            this.LikeForce = LikeForce;
            this.LikeFriend = LikeFriend;
            this.OwnTooMuchCardRate = OwnTooMuchCardRate;
            this.BalanceFactor = BalanceFactor;
            this.Intelligence = Intelligence;
            this.AccumulatedCostBias = AccumulatedCostBias;
        }

        public PlayerConfig() { }

        private static Dictionary<int, PlayerConfig> config = new Dictionary<int, PlayerConfig>();
        public static Dictionary<int, PlayerConfig>.ValueCollection ConfigList
        {
            get { return config.Values; }
        }

        public static void Refresh(Dictionary<int, PlayerConfig> dict)
        {
            config.Clear();
            config = dict;
            RebuildIndex();
        }

        public static void Load()
        {
            config.Clear();
            config[1] = new PlayerConfig(1, "旺仔", "元气满满的小吉祥物", "PlayerPic/wang", "#00FF00", false, 0, new int[0], 0f, 0, 0f, new int[0], new int[0], new int[0], 0f, 0f, 0, 0f);
            config[2] = new PlayerConfig(2, "布布", "爱攒同卡急升星", "PlayerPic/bubu", "#333333", true, 0, new int[0], 5f, 2, 0.6f, new int[]{101}, new int[]{1,2}, new int[]{7,12,8,28}, 0.7f, 0.4f, 75, 0.95f);
            config[3] = new PlayerConfig(3, "翔阳", "热血青年", "PlayerPic/xiangyang", "#FF8000", true, 0, new int[0], 3f, 2, 0.5f, new int[]{101,102}, new int[]{2,3}, new int[]{9,13,29,22,26}, 0.7f, 0.5f, 60, 1.00f);
            config[4] = new PlayerConfig(4, "屁屁", "侦探机制又果决", "PlayerPic/pp", "#F9BEB0", true, 0, new int[0], 3f, 1, 0.7f, new int[]{201,202,403,301}, new int[0], new int[]{10,23,19,27}, 0.7f, 0.5f, 60, 0.9f);
            config[5] = new PlayerConfig(5, "八戒", "呆头呆脑四肢发达", "PlayerPic/bajie", "#FFCC99", true, 0, new int[0], 3f, 2, 0.28f, new int[0], new int[]{10,4}, new int[]{14,26}, 0.7f, 0.6f, 50, 1.1f);
            config[6] = new PlayerConfig(6, "艾沙", "勇敢美丽的少女", "PlayerPic/aisha", "#2BD9F9", true, 0, new int[0], 3f, 1, 0.2f, new int[]{401,503}, new int[]{2}, new int[]{5,15,21,32,4}, 0.7f, 0.8f, 55, 0.85f);
            config[7] = new PlayerConfig(7, "大雄", "憧憬勇武 爱逞强自大", "PlayerPic/daxiong", "#FF9900", true, 0, new int[0], 3f, 1, 0.3f, new int[]{601,602,603}, new int[]{1,2}, new int[]{1,13,14}, 0.5f, 0.3f, 15, 0.6f);
            config[8] = new PlayerConfig(8, "巴爸", "稳重和谐幽默", "PlayerPic/baba", "#FF73FF", true, 0, new int[0], 3f, 2, 0.28f, new int[]{301,302}, new int[]{1}, new int[]{11,25,28,18,7}, 0.85f, 0.6f, 65, 1.05f);
            config[9] = new PlayerConfig(9, "巴妈", "重情护友聪明憨厚", "PlayerPic/bama", "#333333", true, 0, new int[0], 3f, 2, 0.35f, new int[]{501}, new int[]{3}, new int[]{15,18,21,32,5,19}, 0.5f, 0.5f, 45, 0.88f);
            config[10] = new PlayerConfig(10, "朱迪", "爱凑热闹 横冲直撞", "PlayerPic/zhudi", "#FF6600", true, 0, new int[0], 3f, 1, 0.3f, new int[]{203}, new int[]{2,6}, new int[]{2,9,22}, 0.5f, 0.4f, 30, 0.7f);
            config[11] = new PlayerConfig(11, "光头", "鬼点子多 爱出风头", "PlayerPic/guangtou", "#00CC00", true, 0, new int[0], 3f, 1, 0.3f, new int[]{602,603}, new int[]{3,4}, new int[]{3,12,29}, 0.5f, 0.3f, 18, 0.65f);
            config[12] = new PlayerConfig(12, "可霏", "温婉重情 乐善好施", "PlayerPic/kefei", "#0099CC", true, 0, new int[0], 3f, 1, 0.4f, new int[]{502,701}, new int[]{6,3}, new int[]{4,7,15,32,21}, 0.45f, 0.5f, 40, 0.75f);
            config[100] = new PlayerConfig(100, "魔童", "野性难驯小魔头", "PlayerPic/nezha", "#8C0000", false, 0, new int[]{409001}, 3f, 2, 0.5f, new int[]{102}, new int[]{4,10}, new int[]{13,14,31,9}, 0.9f, 0.5f, 50, 0.75f);
            config[101] = new PlayerConfig(101, "钱多", "多金老练出手阔少爷", "PlayerPic/qian", "#FFFFFF", false, 2, new int[]{409002}, 5f, 3, 0.525f, new int[]{701,402}, new int[0], new int[]{17,24,12,20,7}, 0.7f, 0.4f, 80, 0.8f);
            config[102] = new PlayerConfig(102, "黄眉", "法力无边目中无人", "PlayerPic/huangmei", "#5555FF", false, 0, new int[]{100002,409004}, 3f, 2, 0.5f, new int[]{401,402,403}, new int[]{3,5}, new int[]{20,16,31,6}, 0.85f, 0.6f, 90, 1.2f);
            config[103] = new PlayerConfig(103, "无量", "法力无边和蔼莽撞", "PlayerPic/wuliang", "#FF3333", false, 0, new int[]{100003,409005}, 3f, 2, 0.5f, new int[]{403,401}, new int[]{3,2}, new int[]{26,9,29,5,13}, 0.9f, 0.6f, 85, 1.15f);
            config[104] = new PlayerConfig(104, "大虎", "聪明军师", "PlayerPic/dahu", "#006633", false, 0, new int[]{100001,409003}, 3f, 2, 0.5f, new int[]{402,403,401,503}, new int[0], new int[]{10,27,11,25,23}, 0.9f, 0.5f, 90, 0.7f);
            config[999] = new PlayerConfig(999, "怪物", "守关无名怪物", "PlayerPic/tower", "#FF0000", false, 0, new int[0], 0f, 0, 0f, new int[0], new int[0], new int[0], 0f, 0f, 0, 0f);

            RebuildIndex();

        }

        private static void RebuildIndex()
        {
            foreach (var kv in config)
            {
            }
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
