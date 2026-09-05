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


        public PlayerConfig(int Id, string Name, string Imgpath, string Colorstr, bool CanPlay, int InitGold, int[] InitCards, bool Banstrongcard, bool Banweakcard, int Pricelower, int Priceupper, float Priceoutrate, float sameCardRate, int Cardherolimit, int Carditemlimit, float Futurerate, float Findmasterrate, int Pickside, float PickFirst, float FriendFactor, float PickSoldierUp, float HeroGoldRate, float ItemGoldRate, float OwnTooMuchCardRate)
        {
            this.Id = Id;
            this.Name = Name;
            this.Imgpath = Imgpath;
            this.Colorstr = Colorstr;
            this.CanPlay = CanPlay;
            this.InitGold = InitGold;
            this.InitCards = InitCards;
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

        }

        public PlayerConfig() { }
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
            {"Id", new FieldMetaInfo("序列", "int", 0)},
            {"Name", new FieldMetaInfo("名字", "string", 0)},
            {"Imgpath", new FieldMetaInfo("图标", "string", 0)},
            {"Colorstr", new FieldMetaInfo("颜色", "string", 0)},
            {"CanPlay", new FieldMetaInfo("是否可选", "bool", 0)},
            {"InitGold", new FieldMetaInfo("初始资金", "int", 0)},
            {"InitCards", new FieldMetaInfo("初始卡牌列表", "int[]", 0)},
            {"Banstrongcard", new FieldMetaInfo("ban强卡", "bool", 0)},
            {"Banweakcard", new FieldMetaInfo("ban弱卡", "bool", 0)},
            {"Pricelower", new FieldMetaInfo("低价区间", "int", 0)},
            {"Priceupper", new FieldMetaInfo("高价区间", "int", 0)},
            {"Priceoutrate", new FieldMetaInfo("区间外折扣", "float", 0)},
            {"sameCardRate", new FieldMetaInfo("同卡倍率", "float", 0)},
            {"Cardherolimit", new FieldMetaInfo("hero卡数量", "int", 0)},
            {"Carditemlimit", new FieldMetaInfo("物品卡数量", "int", 0)},
            {"Futurerate", new FieldMetaInfo("看未来", "float", 0)},
            {"Findmasterrate", new FieldMetaInfo("找core概率", "float", 0)},
            {"Pickside", new FieldMetaInfo("阵营only", "int", 0)},
            {"PickFirst", new FieldMetaInfo("拿先手", "float", 0)},
            {"FriendFactor", new FieldMetaInfo("好友因子", "float", 0)},
            {"PickSoldierUp", new FieldMetaInfo("拿士兵强化", "float", 0)},
            {"HeroGoldRate", new FieldMetaInfo("英雄花钱比警戒线", "float", 0)},
            {"ItemGoldRate", new FieldMetaInfo("道具花钱比警戒线", "float", 0)},
            {"OwnTooMuchCardRate", new FieldMetaInfo("卡牌风险把控", "float", 0)},
        };

        public static Dictionary<string, FieldMetaInfo> FieldMeta { get { return fieldMeta; } }

        private static List<CellMeta> cellMeta = new List<CellMeta>();
        public static List<CellMeta> CellMetas { get { return cellMeta; } }

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
            config[1] = new PlayerConfig(1, "旺仔", "PlayerPic/wang", "#00FF00", false, 0, new int[0], false, false, 0, 0, 0f, 0f, 0, 0, 0f, 0f, 0, 0f, 0f, 0f, 0f, 0f, 0f);
            config[2] = new PlayerConfig(2, "布布", "PlayerPic/bubu", "#333333", true, 0, new int[0], true, false, 2, 6, 0.1f, 5f, 8, 5, 0.6f, 1f, 0, 0.5f, 1f, 0.5f, 0.85f, 0.25f, 0.7f);
            config[3] = new PlayerConfig(3, "翔阳", "PlayerPic/xiangyang", "#FF8000", true, 0, new int[0], false, false, 8, 12, 0.3f, 3f, 7, 6, 0.5f, 1f, 0, 0.5f, 1f, 0.5f, 0.8f, 0.3f, 0.7f);
            config[4] = new PlayerConfig(4, "屁屁", "PlayerPic/pp", "#F9BEB0", true, 0, new int[0], false, false, 8, 12, 0.2f, 3f, 7, 6, 0.7f, 1f, 0, 0.5f, 1f, 0.5f, 0.8f, 0.3f, 0.7f);
            config[5] = new PlayerConfig(5, "八戒", "PlayerPic/bajie", "#FFCC99", true, 0, new int[0], true, false, 8, 12, 0.3f, 3f, 8, 6, 0.28f, 1f, 0, 1f, 1f, 0.5f, 0.85f, 0.25f, 0.7f);
            config[6] = new PlayerConfig(6, "艾沙", "PlayerPic/aisha", "#2BD9F9", true, 0, new int[0], false, true, 8, 12, 0.3f, 3f, 9, 6, 0.2f, 1f, 0, 0.5f, 0.65f, 0.3f, 0.8f, 0.3f, 0.7f);
            config[8] = new PlayerConfig(8, "巴爸", "PlayerPic/baba", "#FF73FF", true, 0, new int[0], false, false, 7, 9, 0.3f, 3f, 8, 6, 0.28f, 1f, 0, 0.5f, 1f, 0.5f, 0.85f, 0.25f, 0.85f);
            config[9] = new PlayerConfig(9, "巴妈", "PlayerPic/bama", "#333333", true, 0, new int[0], false, false, 8, 10, 0.3f, 3f, 9, 6, 0.35f, 1f, 0, 3f, 1.2f, 1f, 0.8f, 0.35f, 0.5f);
            config[100] = new PlayerConfig(100, "魔童", "PlayerPic/nezha", "#8C0000", false, -5, new int[]{409001}, false, true, 8, 12, 0.3f, 3f, 7, 6, 0.5f, 1f, 0, 2f, 0.5f, 0.5f, 0.84f, 0.23f, 0.9f);
            config[101] = new PlayerConfig(101, "钱多", "PlayerPic/qian", "#FFFFFF", false, 5, new int[]{409002}, false, true, 9, 12, 0.1f, 5f, 8, 7, 0.525f, 1f, 0, 1f, 1.5f, 2f, 0.95f, 0.13f, 0.7f);
            config[102] = new PlayerConfig(102, "黄眉", "PlayerPic/huangmei", "#5555FF", false, -5, new int[]{100002,409004}, false, true, 8, 12, 0.2f, 3f, 7, 6, 0.5f, 2.5f, 2, 1f, 1.2f, 1.5f, 0.95f, 0.12f, 0.85f);
            config[103] = new PlayerConfig(103, "无量", "PlayerPic/wuliang", "#FF3333", false, -5, new int[]{100003,409005}, false, true, 9, 12, 0.2f, 3f, 8, 7, 0.5f, 2.5f, 3, 2f, 0.5f, 0.5f, 0.85f, 0.2f, 0.9f);
            config[104] = new PlayerConfig(104, "大虎", "PlayerPic/dahu", "#006633", false, -5, new int[]{100001,409003}, false, true, 8, 12, 0.3f, 3f, 7, 6, 0.5f, 3f, 1, 2f, 0.5f, 0.5f, 0.85f, 0.2f, 0.9f);
            config[999] = new PlayerConfig(999, "怪物", "PlayerPic/tower", "#FF0000", false, 0, new int[0], false, false, 0, 0, 0f, 0f, 0, 0, 0f, 0f, 0, 0f, 0f, 0f, 0f, 0f, 0f);

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
