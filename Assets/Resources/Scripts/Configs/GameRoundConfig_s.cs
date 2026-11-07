using System;
using System.Collections;
using System.Collections.Generic;

namespace CommonConfig
{
    public class GameRoundConfig
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
            {"MapWeights", new FieldMetaInfo("可刷地图权重（格式 地图英文名:权重，逗号分隔，如 forest1v1:30,forest4v4:20；相对权重，≤0不参与）", "string", 275)},
            {"RoundGold", new FieldMetaInfo("回合加钱", "int", 60)},
            {"RoundType", new FieldMetaInfo("类型（0=PVP，1=PVE）", "int", 132)},
            {"ShopType", new FieldMetaInfo("商店类型（0=共享轮换，1=独立买卡）", "int", 71)},
            {"DamageCount", new FieldMetaInfo("失败扣除生命值（玩家初始生命100，每轮战斗失败扣该值，生命≤0淘汰移出游戏）", "int", 73)},
            {"SoldierList", new FieldMetaInfo("PVE怪物布阵（怪物id;布阵格|怪物id;布阵格，布阵格0~24：0=左上，24=右下）", "string", 586)},
        };

        public static Dictionary<string, FieldMetaInfo> FieldMeta { get { return fieldMeta; } }

        private static List<CellMeta> cellMeta = new List<CellMeta>();
        public static List<CellMeta> CellMetas { get { return cellMeta; } }

        /// <summary>
        ///序列
        /// </summary>
        public int Id;
        /// <summary>
        ///可刷地图权重（格式 地图英文名:权重，逗号分隔，如 forest1v1:30,forest4v4:20；相对权重，≤0不参与）
        /// </summary>
        public string MapWeights;
        /// <summary>
        ///回合加钱
        /// </summary>
        public int RoundGold;
        /// <summary>
        ///类型（0=PVP，1=PVE）
        /// </summary>
        public int RoundType;
        /// <summary>
        ///商店类型（0=共享轮换，1=独立买卡）
        /// </summary>
        public int ShopType;
        /// <summary>
        ///失败扣除生命值（玩家初始生命100，每轮战斗失败扣该值，生命≤0淘汰移出游戏）
        /// </summary>
        public int DamageCount;
        /// <summary>
        ///PVE怪物布阵（怪物id;布阵格|怪物id;布阵格，布阵格0~24：0=左上，24=右下，PVE时生效）
        /// </summary>
        public string SoldierList;


        public GameRoundConfig(int Id, string MapWeights, int RoundGold, int RoundType, int ShopType, int DamageCount, string SoldierList)
        {
            this.Id = Id;
            this.MapWeights = MapWeights;
            this.RoundGold = RoundGold;
            this.RoundType = RoundType;
            this.ShopType = ShopType;
            this.DamageCount = DamageCount;
            this.SoldierList = SoldierList;
        }

        public GameRoundConfig() { }

        private static Dictionary<int, GameRoundConfig> config = new Dictionary<int, GameRoundConfig>();
        public static Dictionary<int, GameRoundConfig>.ValueCollection ConfigList
        {
            get { return config.Values; }
        }

        public static void Refresh(Dictionary<int, GameRoundConfig> dict)
        {
            config.Clear();
            config = dict;
            RebuildIndex();
        }

        public static void Load()
        {
            config.Clear();
            config[1] = new GameRoundConfig(1, "forest1v1:1", 2, 1, 0, 1, "590001;0|590001;23");
            config[2] = new GameRoundConfig(2, "forest1v1:1", 3, 1, 0, 1, "590001;0|590001;12|590001;23");
            config[3] = new GameRoundConfig(3, "forest1v1:1", 4, 0, 1, 1, "590001;0|590001;12|590001;23");
            config[4] = new GameRoundConfig(4, "forest1v1:1", 5, 0, 1, 1, "590001;0|590001;12|590001;23");
            config[5] = new GameRoundConfig(5, "forest1v1:1,forest4v4:1,islandteam:1", 6, 0, 1, 1, "590001;0|590001;12|590001;23");
            config[6] = new GameRoundConfig(6, "forest1v1:1,forest4v4:1,islandteam:1", 7, 0, 1, 1, "590001;0|590001;12|590001;23");
            config[7] = new GameRoundConfig(7, "forest1v1:2,river:1,forest4v4:1,islandteam:1", 8, 0, 1, 2, "590001;0|590001;12|590001;23");
            config[8] = new GameRoundConfig(8, "forest1v1:2,river:1,forest4v4:1,islandteam:1", 9, 0, 1, 2, "590001;0|590001;12|590001;23");
            config[9] = new GameRoundConfig(9, "forest1v1:1,river:1", 10, 1, 0, 2, "590001;0|590001;12|590001;23");
            config[10] = new GameRoundConfig(10, "iceland:1", 11, 0, 1, 2, "590001;0|590001;12|590001;23");
            config[11] = new GameRoundConfig(11, "forest1v1:1,forest4v4:1,islandteam:1", 12, 0, 1, 2, "590001;0|590001;12|590001;23");
            config[12] = new GameRoundConfig(12, "river:1", 13, 0, 1, 2, "590001;0|590001;12|590001;23");
            config[13] = new GameRoundConfig(13, "forest1v1:1,forest4v4:1,islandteam:1", 14, 0, 0, 3, "590001;0|590001;12|590001;23");
            config[14] = new GameRoundConfig(14, "forest1v1:1,river:1,forest4v4:1,islandteam:1", 15, 0, 1, 3, "590001;0|590001;12|590001;23");
            config[15] = new GameRoundConfig(15, "forest1v1:1,forest4v4:1,islandteam:1", 16, 0, 1, 3, "590001;0|590001;12|590001;23");
            config[16] = new GameRoundConfig(16, "forest1v1:1,river:1,forest4v4:1,islandteam:1", 17, 0, 1, 3, "590001;0|590001;12|590001;23");
            config[17] = new GameRoundConfig(17, "forest1v1:1", 18, 1, 0, 3, "590001;0|590001;12|590001;23");
            config[18] = new GameRoundConfig(18, "forest1v1:1,forest4v4:1,islandteam:1", 19, 0, 1, 3, "590001;0|590001;12|590001;23");
            config[19] = new GameRoundConfig(19, "forest4v4:1,islandteam:1", 20, 0, 1, 3, "590001;0|590001;12|590001;23");
            config[20] = new GameRoundConfig(20, "iceland:1", 20, 0, 1, 3, "590001;0|590001;12|590001;23");
            config[21] = new GameRoundConfig(21, "forest1v1:1,forest4v4:1,islandteam:1", 20, 0, 0, 4, "590001;0|590001;12|590001;23");
            config[22] = new GameRoundConfig(22, "forest1v1:1,forest4v4:1,islandteam:1", 20, 0, 1, 4, "590001;0|590001;12|590001;23");
            config[23] = new GameRoundConfig(23, "forest1v1:1,river:1,forest4v4:1,islandteam:1", 20, 0, 1, 4, "590001;0|590001;12|590001;23");
            config[24] = new GameRoundConfig(24, "forest1v1:1,forest4v4:1,islandteam:1", 20, 0, 1, 4, "590001;0|590001;12|590001;23");
            config[25] = new GameRoundConfig(25, "forest1v1:1", 20, 1, 0, 4, "590001;0|590001;12|590001;23");
            config[26] = new GameRoundConfig(26, "forest1v1:1,forest4v4:1,islandteam:1", 20, 0, 1, 4, "590001;0|590001;12|590001;23");
            config[27] = new GameRoundConfig(27, "forest4v4:1,islandteam:1", 20, 0, 1, 4, "590001;0|590001;12|590001;23");
            config[28] = new GameRoundConfig(28, "forest1v1:1,river:1,forest4v4:1,islandteam:1", 20, 0, 1, 4, "590001;0|590001;12|590001;23");
            config[29] = new GameRoundConfig(29, "forest1v1:1,forest4v4:1,islandteam:1", 20, 0, 0, 5, "590001;0|590001;12|590001;23");
            config[30] = new GameRoundConfig(30, "iceland:1", 20, 0, 1, 5, "590001;0|590001;12|590001;23");
            config[31] = new GameRoundConfig(31, "forest1v1:1,forest4v4:1,islandteam:1", 20, 0, 1, 5, "590001;0|590001;12|590001;23");
            config[32] = new GameRoundConfig(32, "forest1v1:1,forest4v4:1,islandteam:1", 20, 0, 1, 5, "590001;0|590001;12|590001;23");
            config[33] = new GameRoundConfig(33, "forest1v1:1", 20, 1, 0, 5, "590001;0|590001;12|590001;23");
            config[34] = new GameRoundConfig(34, "forest4v4:1,islandteam:1", 20, 0, 1, 5, "590001;0|590001;12|590001;23");
            config[35] = new GameRoundConfig(35, "forest1v1:1,forest4v4:1,islandteam:1", 20, 0, 1, 5, "590001;0|590001;12|590001;23");
            config[36] = new GameRoundConfig(36, "forest1v1:1,river:1,forest4v4:1,islandteam:1", 20, 0, 1, 5, "590001;0|590001;12|590001;23");
            config[37] = new GameRoundConfig(37, "forest1v1:1,river:1,forest4v4:1,islandteam:1", 20, 0, 0, 6, "590001;0|590001;12|590001;23");
            config[38] = new GameRoundConfig(38, "forest1v1:1,forest4v4:1,islandteam:1", 20, 0, 1, 6, "590001;0|590001;12|590001;23");
            config[39] = new GameRoundConfig(39, "forest1v1:1,forest4v4:1,islandteam:1", 20, 0, 1, 6, "590001;0|590001;12|590001;23");
            config[40] = new GameRoundConfig(40, "iceland:1", 20, 0, 1, 6, "590001;0|590001;12|590001;23");
            config[41] = new GameRoundConfig(41, "river:1", 20, 1, 0, 6, "590001;0|590001;12|590001;23");
            config[42] = new GameRoundConfig(42, "forest1v1:1,forest4v4:1,islandteam:1", 20, 0, 1, 6, "590001;0|590001;12|590001;23");
            config[43] = new GameRoundConfig(43, "forest1v1:1,forest4v4:1,islandteam:1", 20, 0, 1, 6, "590001;0|590001;12|590001;23");
            config[44] = new GameRoundConfig(44, "forest4v4:1,islandteam:1", 20, 0, 1, 6, "590001;0|590001;12|590001;23");
            config[45] = new GameRoundConfig(45, "forest1v1:1,forest4v4:1,islandteam:1", 20, 0, 0, 6, "590001;0|590001;12|590001;23");
            config[46] = new GameRoundConfig(46, "forest4v4:1,islandteam:1", 20, 0, 1, 7, "590001;0|590001;12|590001;23");
            config[47] = new GameRoundConfig(47, "forest1v1:1,forest4v4:1,islandteam:1", 20, 0, 1, 7, "590001;0|590001;12|590001;23");
            config[48] = new GameRoundConfig(48, "forest1v1:1,forest4v4:1,islandteam:1", 20, 0, 1, 7, "590001;0|590001;12|590001;23");
            config[49] = new GameRoundConfig(49, "forest1v1:1", 20, 1, 0, 7, "590001;0|590001;12|590001;23");
            config[50] = new GameRoundConfig(50, "iceland:1", 20, 0, 1, 8, "590001;0|590001;12|590001;23");
            config[51] = new GameRoundConfig(51, "forest1v1:1,forest4v4:1,islandteam:1", 20, 0, 1, 8, "590001;0|590001;12|590001;23");
            config[52] = new GameRoundConfig(52, "forest1v1:1,river:1,forest4v4:1,islandteam:1", 20, 0, 1, 8, "590001;0|590001;12|590001;23");
            config[53] = new GameRoundConfig(53, "river:1", 20, 0, 0, 8, "590001;0|590001;12|590001;23");
            config[54] = new GameRoundConfig(54, "forest1v1:1,river:1,forest4v4:1,islandteam:1", 20, 0, 1, 9, "590001;0|590001;12|590001;23");
            config[55] = new GameRoundConfig(55, ",forest4v4:1,islandteam:1", 20, 0, 1, 9, "590001;0|590001;12|590001;23");
            config[56] = new GameRoundConfig(56, "river:1", 20, 0, 1, 9, "590001;0|590001;12|590001;23");
            config[57] = new GameRoundConfig(57, "river:1", 20, 1, 0, 9, "590001;0|590001;12|590001;23");
            config[58] = new GameRoundConfig(58, "forest1v1:1,forest4v4:1,islandteam:1", 20, 0, 1, 10, "590001;0|590001;12|590001;23");
            config[59] = new GameRoundConfig(59, "forest1v1:1,forest4v4:1,islandteam:1", 20, 0, 1, 10, "590001;0|590001;12|590001;23");
            config[60] = new GameRoundConfig(60, "iceland:1", 20, 0, 1, 10, "590001;0|590001;12|590001;23");
            config[61] = new GameRoundConfig(61, "forest1v1:1,forest4v4:1,islandteam:1", 20, 0, 0, 10, "590001;0|590001;12|590001;23");
            config[62] = new GameRoundConfig(62, "forest1v1:1,forest4v4:1,islandteam:1", 20, 0, 1, 10, "590001;0|590001;12|590001;23");
            config[63] = new GameRoundConfig(63, "river:1", 20, 0, 1, 10, "590001;0|590001;12|590001;23");
            config[64] = new GameRoundConfig(64, "forest1v1:1,forest4v4:1,islandteam:1", 20, 0, 1, 10, "590001;0|590001;12|590001;23");
            config[65] = new GameRoundConfig(65, "forest1v1:1", 20, 1, 0, 10, "590001;0|590001;12|590001;23");
            config[66] = new GameRoundConfig(66, "forest1v1:1,forest4v4:1,islandteam:1", 20, 0, 1, 10, "590001;0|590001;12|590001;23");
            config[67] = new GameRoundConfig(67, "forest1v1:1,forest4v4:1,islandteam:1", 20, 0, 1, 10, "590001;0|590001;12|590001;23");
            config[68] = new GameRoundConfig(68, "forest1v1:1,river:1,forest4v4:1,islandteam:1", 20, 0, 1, 10, "590001;0|590001;12|590001;23");
            config[69] = new GameRoundConfig(69, "forest4v4:1,islandteam:1", 20, 0, 0, 10, "590001;0|590001;12|590001;23");
            config[70] = new GameRoundConfig(70, "iceland:1", 20, 0, 1, 10, "590001;0|590001;12|590001;23");
            config[71] = new GameRoundConfig(71, "forest1v1:1,forest4v4:1,islandteam:1", 20, 0, 1, 10, "590001;0|590001;12|590001;23");
            config[72] = new GameRoundConfig(72, "river:1", 20, 0, 1, 10, "590001;0|590001;12|590001;23");
            config[73] = new GameRoundConfig(73, "forest1v1:1", 20, 1, 0, 10, "590001;0|590001;12|590001;23");
            config[74] = new GameRoundConfig(74, "forest1v1:1,forest4v4:1,islandteam:1", 20, 0, 1, 10, "590001;0|590001;12|590001;23");
            config[75] = new GameRoundConfig(75, "forest1v1:1,river:1,forest4v4:1,islandteam:1", 20, 0, 1, 10, "590001;0|590001;12|590001;23");
            config[76] = new GameRoundConfig(76, "forest4v4:1,islandteam:1", 20, 0, 1, 10, "590001;0|590001;12|590001;23");
            config[77] = new GameRoundConfig(77, "forest1v1:1,forest4v4:1,islandteam:1", 20, 0, 0, 10, "590001;0|590001;12|590001;23");
            config[78] = new GameRoundConfig(78, "forest4v4:1,islandteam:1", 20, 0, 1, 10, "590001;0|590001;12|590001;23");
            config[79] = new GameRoundConfig(79, "forest1v1:1,forest4v4:1,islandteam:1", 20, 0, 1, 10, "590001;0|590001;12|590001;23");
            config[80] = new GameRoundConfig(80, "iceland:1", 20, 0, 1, 10, "590001;0|590001;12|590001;23");
            config[81] = new GameRoundConfig(81, "forest1v1:1", 20, 1, 0, 10, "590001;0|590001;12|590001;23");
            config[82] = new GameRoundConfig(82, "forest1v1:1,forest4v4:1,islandteam:1", 20, 0, 1, 10, "590001;0|590001;12|590001;23");
            config[83] = new GameRoundConfig(83, "river:1", 20, 0, 1, 10, "590001;0|590001;12|590001;23");
            config[84] = new GameRoundConfig(84, "river:1", 20, 0, 1, 10, "590001;0|590001;12|590001;23");
            config[85] = new GameRoundConfig(85, "forest1v1:1,river:1,forest4v4:1,islandteam:1", 20, 0, 0, 10, "590001;0|590001;12|590001;23");
            config[86] = new GameRoundConfig(86, "forest1v1:1,forest4v4:1,islandteam:1", 20, 0, 1, 10, "590001;0|590001;12|590001;23");
            config[87] = new GameRoundConfig(87, "forest1v1:1,river:1,forest4v4:1,islandteam:1", 20, 0, 1, 10, "590001;0|590001;12|590001;23");
            config[88] = new GameRoundConfig(88, "forest1v1:1,forest4v4:1,islandteam:1", 20, 0, 1, 10, "590001;0|590001;12|590001;23");
            config[89] = new GameRoundConfig(89, "forest1v1:1", 20, 1, 0, 10, "590001;0|590001;12|590001;23");
            config[90] = new GameRoundConfig(90, "iceland:1", 20, 0, 1, 10, "590001;0|590001;12|590001;23");
            config[91] = new GameRoundConfig(91, "forest1v1:1,forest4v4:1,islandteam:1", 20, 0, 1, 10, "590001;0|590001;12|590001;23");
            config[92] = new GameRoundConfig(92, "forest4v4:1,islandteam:1", 20, 0, 1, 10, "590001;0|590001;12|590001;23");
            config[93] = new GameRoundConfig(93, "forest1v1:1,forest4v4:1,islandteam:1", 20, 0, 1, 10, "590001;0|590001;12|590001;23");
            config[94] = new GameRoundConfig(94, "forest1v1:1,forest4v4:1,islandteam:1", 20, 0, 1, 10, "590001;0|590001;12|590001;23");
            config[95] = new GameRoundConfig(95, "forest1v1:1,forest4v4:1,islandteam:1", 20, 0, 1, 10, "590001;0|590001;12|590001;23");
            config[96] = new GameRoundConfig(96, "forest1v1:1,forest4v4:1,islandteam:1", 20, 0, 1, 10, "590001;0|590001;12|590001;23");
            config[97] = new GameRoundConfig(97, "forest1v1:1", 20, 1, 0, 10, "590001;0|590001;12|590001;23");
            config[98] = new GameRoundConfig(98, "forest1v1:1,forest4v4:1,islandteam:1", 20, 0, 1, 10, "590001;0|590001;12|590001;23");
            config[99] = new GameRoundConfig(99, "forest1v1:1,forest4v4:1,islandteam:1", 20, 0, 1, 10, "590001;0|590001;12|590001;23");
            config[100] = new GameRoundConfig(100, "iceland:1", 20, 0, 1, 10, "590001;0|590001;12|590001;23");

            RebuildIndex();

        }

        private static void RebuildIndex()
        {
            foreach (var kv in config)
            {
            }
        }

        public static GameRoundConfig GetConfig(int id)
        {
            GameRoundConfig data;
            if (config.TryGetValue(id, out data))
            {
                return data;
            }
            throw new NullReferenceException(string.Format("配置表GameRoundConfig不存在id={0}", id));
        }


        public static bool HasConfig(int id)
        {
            if (config.ContainsKey(id))
            {
                return true;
            }
            return false;
        }

        public static void Assign(int id, GameRoundConfig configData)
        {
            config[id] = configData; 
        }

        public static void Add(int id, GameRoundConfig configData)
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
