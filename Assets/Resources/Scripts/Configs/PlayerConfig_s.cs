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
            {"LikeDeck", new FieldMetaInfo("喜欢卡组风格（由 LikeJob/LikeForce/LikeFriend 派生的玩法描述，仅备注用）", "string", 393)},
            {"LikeJob", new FieldMetaInfo("喜欢的职业Id列表(可0-4个，取 JobConfig.Id；命中即固定倍率加成，0个=不关注)", "int[]", 137)},
            {"LikeForce", new FieldMetaInfo("喜欢的势力Id列表(可0-2个，取 ForceConfig.Id/阵营side；命中即固定倍率加成，0个=不关注)", "int[]", 85)},
            {"LikeFriend", new FieldMetaInfo("喜欢的好友组Id列表(可2-6个，取 HeroFriendConfig.Id；命中即固定倍率加成，0个=不关注)", "int[]", 155)},
            {"UpgradeSoldierRate", new FieldMetaInfo("士兵强化概率(0-1)：每局商店开始时随机0-1，小于该值时花金币强化1级士兵", "float", 80)},
            {"OwnTooMuchCardRate", new FieldMetaInfo("卡牌风险把控", "float", 60)},
            {"BalanceFactor", new FieldMetaInfo("远近因子", "float", 60)},
            {"Intelligence", new FieldMetaInfo("聪明度（1-99，越高越能正确评估羁绊价值、越少手滑）", "int", 60)},
            {"AccumulatedCostBias", new FieldMetaInfo("累积购卡倾向（越大上升越快，1.0 时 target≈sqrt(year)）", "float", 60)},
            {"ExpCheckOff", new FieldMetaInfo("经验期望系数（AI经验检查用，0.2~1，仅正；期望总经验=round×2.5×本系数）", "float", 60)},
            {"EliminateQuality1", new FieldMetaInfo("淘汰品质1卡回合（基准值，实际回合=基准±round((1-smart)×5)；到达后非主力上阵的品质1卡不再买、立刻卖；0=不淘汰）", "int", 60)},
            {"EliminateQuality2", new FieldMetaInfo("淘汰品质2卡回合（基准值，同上；到达后非主力上阵的品质2及以下卡不再买、立刻卖；0=不淘汰）", "int", 60)},
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
        ///喜欢卡组风格（由 LikeJob/LikeForce/LikeFriend 派生的玩法描述，仅备注用）
        /// </summary>
        public string LikeDeck;
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
        ///士兵强化概率（0-1）：每局商店开始时随机 0-1，小于该值时花金币强化 1 级士兵
        /// </summary>
        public float UpgradeSoldierRate;
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
        /// <summary>
        ///经验期望系数（AI经验检查用，范围 0.2~1，仅正；期望总经验 = round×2.5×本系数，弱的角色偏 0.2）
        /// </summary>
        public float ExpCheckOff;
        /// <summary>
        ///淘汰品质1卡回合（基准值；实际回合 = 基准 ± round((1-smart)×5)，smart=聪明度/99；到达后"非主力上阵"的品质1卡不再买、手上的立刻卖；0=不淘汰）
        /// </summary>
        public int EliminateQuality1;
        /// <summary>
        ///淘汰品质2卡回合（基准值，同上；到达后"非主力上阵"的品质2及以下卡不再买、手上的立刻卖；0=不淘汰）
        /// </summary>
        public int EliminateQuality2;


        public PlayerConfig(int Id, string Name, string Personality, string Imgpath, string Colorstr, bool CanPlay, int InitGold, int[] InitCards, float SameCardRate, int Cardherolimit, string LikeDeck, int[] LikeJob, int[] LikeForce, int[] LikeFriend, float UpgradeSoldierRate, float OwnTooMuchCardRate, float BalanceFactor, int Intelligence, float AccumulatedCostBias, float ExpCheckOff, int EliminateQuality1, int EliminateQuality2)
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
            this.LikeDeck = LikeDeck;
            this.LikeJob = LikeJob;
            this.LikeForce = LikeForce;
            this.LikeFriend = LikeFriend;
            this.UpgradeSoldierRate = UpgradeSoldierRate;
            this.OwnTooMuchCardRate = OwnTooMuchCardRate;
            this.BalanceFactor = BalanceFactor;
            this.Intelligence = Intelligence;
            this.AccumulatedCostBias = AccumulatedCostBias;
            this.ExpCheckOff = ExpCheckOff;
            this.EliminateQuality1 = EliminateQuality1;
            this.EliminateQuality2 = EliminateQuality2;
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
            config[1] = new PlayerConfig(1, "旺仔", "元气满满的小吉祥物", "PlayerPic/wang", "#00FF00", false, 0, new int[0], 0f, 0, "无固定卡组偏好（不参加对局）", new int[0], new int[0], new int[0], 0.1f, 0f, 0f, 0, 0f, 0f, 0, 0);
            config[2] = new PlayerConfig(2, "布布", "爱攒同卡急升星", "PlayerPic/bubu", "#333333", true, 0, new int[0], 1.5f, 3, "同卡升星滚雪球：骑士前排 + 蜀国 + 济世/王佐/善守续航", new int[]{101}, new int[]{1}, new int[]{7,12,28}, 0.1f, 0.7f, 0.4f, 75, 0.95f, .5f, 14, 28);
            config[3] = new PlayerConfig(3, "翔阳", "热血青年", "PlayerPic/xiangyang", "#FF8000", true, 0, new int[0], 1.5f, 3, "热血冲锋：骑士 + 魏国 + 陷阵/虎狼/背水", new int[]{101}, new int[]{2}, new int[]{9,29,22}, 0.1f, 0.7f, 0.5f, 60, 1.00f, 0.6f, 10, 23);
            config[4] = new PlayerConfig(4, "屁屁", "侦探机制又果决", "PlayerPic/pp", "#F9BEB0", true, 0, new int[0], 1.5f, 2, "弩手爆发+棋手机制：卫士前排 + 深谋/偷袭", new int[]{202,301,403}, new int[0], new int[]{10,23}, 0.1f, 0.7f, 0.5f, 60, 0.9f, 0.65f, 10, 22);
            config[5] = new PlayerConfig(5, "八戒", "呆头呆脑四肢发达", "PlayerPic/bajie", "#FFCC99", true, 0, new int[0], 1.5f, 3, "蛮力肉搏：野/董侯势力 + 悍将/铁骑", new int[0], new int[]{10,4}, new int[]{14,26}, 0.1f, 0.7f, 0.6f, 50, 1.1f, .35f, 11, 24);
            config[6] = new PlayerConfig(6, "艾沙", "勇敢美丽的少女", "PlayerPic/aisha", "#2BD9F9", true, 0, new int[0], 1.5f, 2, "治疗续航：医士 + 魏国 + 仁德/义胆/善守护卫", new int[]{503}, new int[]{2}, new int[]{5,15,28}, 0.1f, 0.7f, 0.8f, 55, 0.85f, 0.5f, 11, 24);
            config[7] = new PlayerConfig(7, "大雄", "憧憬勇武 爱逞强自大", "PlayerPic/daxiong", "#FF9900", true, 0, new int[0], 1.5f, 2, "纯近战勇武：锤/枪/戟 + 无双/悍将", new int[]{601,602,603}, new int[0], new int[]{1,14}, 0.1f, 0.5f, 0.3f, 15, 0.6f, 0.2f, 14, 28);
            config[8] = new PlayerConfig(8, "巴爸", "稳重和谐幽默", "PlayerPic/baba", "#FF73FF", true, 0, new int[0], 1.5f, 3, "士兵增益+龟甲防守：卫士/丞相 + 蜀国 + 严明/善守", new int[]{301,402}, new int[]{1}, new int[]{11,28}, 0.3f, 0.85f, 0.6f, 65, 1.05f, .5f, 9, 21);
            config[9] = new PlayerConfig(9, "巴妈", "重情护友聪明憨厚", "PlayerPic/bama", "#333333", true, 0, new int[0], 1.5f, 3, "光环辅助：鼓手/丞相 + 将相/温良/安民/仁德", new int[]{501,402}, new int[0], new int[]{18,21,32,5}, 0.3f, 0.5f, 0.5f, 45, 0.88f, 0.45f, 12, 25);
            config[10] = new PlayerConfig(10, "朱迪", "爱凑热闹 横冲直撞", "PlayerPic/zhudi", "#FF6600", true, 0, new int[0], 1.5f, 2, "炮击爆发：炮手 + 蜀国 + 野火/陷阵/背水", new int[]{203}, new int[]{1}, new int[]{2,9,22}, 0.1f, 0.5f, 0.4f, 30, 0.7f, 0.3f, 13, 27);
            config[11] = new PlayerConfig(11, "光头", "鬼点子多 爱出风头", "PlayerPic/guangtou", "#00CC00", true, 0, new int[0], 1.5f, 2, "枪阵突袭：枪兵 + 吴国 + 骑射/王佐/虎狼", new int[]{602}, new int[]{3}, new int[]{3,12,29}, 0.1f, 0.5f, 0.3f, 18, 0.65f, 0.2f, 14, 28);
            config[12] = new PlayerConfig(12, "可霏", "温婉重情 乐善好施", "PlayerPic/kefei", "#0099CC", true, 0, new int[0], 1.5f, 2, "乐师辅助：琴手 + 吴国 + 济世/义胆/安民", new int[]{502}, new int[]{3}, new int[]{7,15,32}, 0.1f, 0.45f, 0.5f, 40, 0.75f, 0.4f, 12, 26);
            config[100] = new PlayerConfig(100, "魔童", "野性难驯小魔头", "PlayerPic/nezha", "#8C0000", false, 0, new int[]{409001}, 1.5f, 3, "战车突击：战车 + 迅捷/悍将/枭雄/陷阵", new int[]{102}, new int[0], new int[]{13,14,31,9}, 0.1f, 0.9f, 0.5f, 50, 0.75f, 0.6f, 11, 24);
            config[101] = new PlayerConfig(101, "钱多", "多金老练出手阔少爷", "PlayerPic/qian", "#FFFFFF", false, 2, new int[]{409002}, 1.5f, 4, "阔少战车+谋士：战车/丞相 + 望族/文采/王佐/奇才", new int[]{102,402}, new int[0], new int[]{17,24,12,20}, 0.1f, 0.7f, 0.4f, 80, 0.8f, 0.9f, 8, 20);
            config[102] = new PlayerConfig(102, "黄眉", "法力无边目中无人", "PlayerPic/huangmei", "#5555FF", false, 0, new int[]{100002,409004}, 1.5f, 3, "权谋法术：智士 + 袁氏 + 权奸/善守", new int[]{401}, new int[]{6}, new int[]{16,28}, 0.1f, 0.85f, 0.6f, 90, 1.2f, .85f, 8, 20);
            config[103] = new PlayerConfig(103, "无量", "法力无边和蔼莽撞", "PlayerPic/wuliang", "#FF3333", false, 0, new int[]{100003,409005}, 1.5f, 3, "法力+莽撞和蔼：智士/盾兵 + 仁德/虎狼", new int[]{401,302}, new int[0], new int[]{5,29}, 0.1f, 0.9f, 0.6f, 85, 1.15f, 0.9f, 8, 20);
            config[104] = new PlayerConfig(104, "大虎", "聪明军师", "PlayerPic/dahu", "#006633", false, 0, new int[]{100001,409003}, 1.5f, 4, "谋略军团+亲卫：丞相/智士/盾兵 + 深谋/智勇", new int[]{302,401,402}, new int[0], new int[]{10,27}, 0.1f, 0.9f, 0.5f, 90, 0.7f, .75f, 8, 20);
            config[999] = new PlayerConfig(999, "怪物", "守关无名怪物", "PlayerPic/tower", "#FF0000", false, 0, new int[0], 0f, 0, "无固定卡组偏好（守关怪物）", new int[0], new int[0], new int[0], 0.1f, 0f, 0f, 0, 0f, 0f, 0, 0);

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
