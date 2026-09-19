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
            {"Imgpath", new FieldMetaInfo("图标", "string", 0)},
            {"Colorstr", new FieldMetaInfo("颜色", "string", 0)},
            {"CanPlay", new FieldMetaInfo("是否可选", "bool", 0)},
            {"InitGold", new FieldMetaInfo("初始资金", "int", 60)},
            {"InitCards", new FieldMetaInfo("初始卡牌列表", "int[]", 0)},
            {"Banstrongcard", new FieldMetaInfo("ban强卡", "bool", 82)},
            {"Banweakcard", new FieldMetaInfo("ban弱卡", "bool", 71)},
            {"sameCardRate", new FieldMetaInfo("同卡倍率", "float", 60)},
            {"Cardherolimit", new FieldMetaInfo("英雄卡上限额外数", "int", 60)},
            {"Futurerate", new FieldMetaInfo("看未来", "float", 60)},
            {"Findmasterrate", new FieldMetaInfo("找core概率", "float", 60)},
            {"Pickside", new FieldMetaInfo("阵营only", "int", 60)},
            {"FriendFactor", new FieldMetaInfo("好友因子", "float", 60)},
            {"OwnTooMuchCardRate", new FieldMetaInfo("卡牌风险把控", "float", 60)},
            {"SideFactor", new FieldMetaInfo("国家因子", "float", 60)},
            {"JobFactor", new FieldMetaInfo("职业因子", "float", 60)},
            {"PowerFactor", new FieldMetaInfo("强度因子", "float", 60)},
            {"BalanceFactor", new FieldMetaInfo("远近因子", "float", 60)},
            {"Intelligence", new FieldMetaInfo("聪明度（1-99，越高越能正确评估羁绊价值、越少手滑）", "int", 60)},
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
        ///同卡倍率
        /// </summary>
        public float sameCardRate;
        /// <summary>
        ///英雄卡上限额外数（当前等级卡片上限基础上额外+多少）
        /// </summary>
        public int Cardherolimit;
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
        ///好友因子
        /// </summary>
        public float FriendFactor;
        /// <summary>
        ///卡牌风险把控
        /// </summary>
        public float OwnTooMuchCardRate;
        /// <summary>
        ///国家因子（组同阵营/推进国家护盾档位的权重，越大越优先凑同阵营）
        /// </summary>
        public float SideFactor;
        /// <summary>
        ///职业因子（组同职业/推进职业连锁档位的权重，越大越优先凑同职业）
        /// </summary>
        public float JobFactor;
        /// <summary>
        ///强度因子（品质/面板/升星进度等硬实力权重，越大越偏好强力卡）
        /// </summary>
        public float PowerFactor;
        /// <summary>
        ///远近因子（近战远程搭配权重，越大越讲究前后排平衡）
        /// </summary>
        public float BalanceFactor;
        /// <summary>
        ///聪明度（1-99，越高越能正确评估羁绊价值、越少手滑）
        /// </summary>
        public int Intelligence;


        public PlayerConfig(int Id, string Name, string Imgpath, string Colorstr, bool CanPlay, int InitGold, int[] InitCards, bool Banstrongcard, bool Banweakcard, float sameCardRate, int Cardherolimit, float Futurerate, float Findmasterrate, int Pickside, float FriendFactor, float OwnTooMuchCardRate, float SideFactor, float JobFactor, float PowerFactor, float BalanceFactor, int Intelligence)
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
            this.sameCardRate = sameCardRate;
            this.Cardherolimit = Cardherolimit;
            this.Futurerate = Futurerate;
            this.Findmasterrate = Findmasterrate;
            this.Pickside = Pickside;
            this.FriendFactor = FriendFactor;
            this.OwnTooMuchCardRate = OwnTooMuchCardRate;
            this.SideFactor = SideFactor;
            this.JobFactor = JobFactor;
            this.PowerFactor = PowerFactor;
            this.BalanceFactor = BalanceFactor;
            this.Intelligence = Intelligence;
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
            config[1] = new PlayerConfig(1, "旺仔", "PlayerPic/wang", "#00FF00", false, 0, new int[0], false, false, 0f, 0, 0f, 0f, 0, 0f, 0f, 0f, 0f, 0f, 0f, 20);
            config[2] = new PlayerConfig(2, "布布", "PlayerPic/bubu", "#333333", true, 0, new int[0], true, false, 5f, 2, 0.6f, 1f, 0, 1f, 0.7f, 0.3f, 0.4f, 1.4f, 0.4f, 75);
            config[3] = new PlayerConfig(3, "翔阳", "PlayerPic/xiangyang", "#FF8000", true, 0, new int[0], false, false, 3f, 2, 0.5f, 1f, 0, 1f, 0.7f, 1.5f, 0.4f, 0.5f, 0.5f, 60);
            config[4] = new PlayerConfig(4, "屁屁", "PlayerPic/pp", "#F9BEB0", true, 0, new int[0], false, false, 3f, 1, 0.7f, 1f, 0, 1f, 0.7f, 0.4f, 1.5f, 0.5f, 0.5f, 60);
            config[5] = new PlayerConfig(5, "八戒", "PlayerPic/bajie", "#FFCC99", true, 0, new int[0], true, false, 3f, 2, 0.28f, 1f, 0, 1f, 0.7f, 1f, 0.8f, 0.7f, 0.6f, 50);
            config[6] = new PlayerConfig(6, "艾沙", "PlayerPic/aisha", "#2BD9F9", true, 0, new int[0], false, true, 3f, 1, 0.2f, 1f, 0, 0.65f, 0.7f, 0.6f, 0.7f, 0.6f, 0.8f, 55);
            config[8] = new PlayerConfig(8, "巴爸", "PlayerPic/baba", "#FF73FF", true, 0, new int[0], false, false, 3f, 2, 0.28f, 1f, 0, 1f, 0.85f, 0.6f, 1.2f, 0.7f, 0.6f, 65);
            config[9] = new PlayerConfig(9, "巴妈", "PlayerPic/bama", "#333333", true, 0, new int[0], false, false, 3f, 2, 0.35f, 1f, 0, 1.2f, 0.5f, 0.7f, 0.6f, 0.6f, 0.5f, 45);
            config[100] = new PlayerConfig(100, "魔童", "PlayerPic/nezha", "#8C0000", false, 0, new int[]{409001}, false, true, 3f, 2, 0.5f, 1f, 0, 0.5f, 0.9f, 0.3f, 0.3f, 1.3f, 0.5f, 50);
            config[101] = new PlayerConfig(101, "钱多", "PlayerPic/qian", "#FFFFFF", false, 2, new int[]{409002}, false, true, 5f, 3, 0.525f, 1f, 0, 1.5f, 0.7f, 0.3f, 0.4f, 1.5f, 0.4f, 80);
            config[102] = new PlayerConfig(102, "黄眉", "PlayerPic/huangmei", "#5555FF", false, 0, new int[]{100002,409004}, false, true, 3f, 2, 0.5f, 2.5f, 2, 1.2f, 0.85f, 1.6f, 0.6f, 0.6f, 0.5f, 85);
            config[103] = new PlayerConfig(103, "无量", "PlayerPic/wuliang", "#FF3333", false, 0, new int[]{100003,409005}, false, true, 3f, 2, 0.5f, 2.5f, 3, 0.5f, 0.9f, 1.5f, 0.5f, 0.6f, 0.6f, 85);
            config[104] = new PlayerConfig(104, "大虎", "PlayerPic/dahu", "#006633", false, 0, new int[]{100001,409003}, false, true, 3f, 2, 0.5f, 3f, 1, 0.5f, 0.9f, 1.7f, 0.4f, 0.7f, 0.5f, 90);
            config[999] = new PlayerConfig(999, "怪物", "PlayerPic/tower", "#FF0000", false, 0, new int[0], false, false, 0f, 0, 0f, 0f, 0, 0f, 0f, 0f, 0f, 0f, 0f, 10);

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
