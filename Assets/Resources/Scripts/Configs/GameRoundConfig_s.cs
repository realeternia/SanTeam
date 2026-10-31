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
            {"Name", new FieldMetaInfo("回合名字", "string", 0)},
            {"MapWeights", new FieldMetaInfo("可刷地图权重（格式 地图英文名:权重，逗号分隔，如 forest1v1:30,forest4v4:20；相对权重，≤0不参与）", "string", 275)},
            {"RoundGold", new FieldMetaInfo("回合加钱", "int", 60)},
            {"MultiCardRate", new FieldMetaInfo("多等级概率", "float", 60)},
            {"MultiPriceTotal", new FieldMetaInfo("多等级", "int", 73)},
            {"Quality2Rate", new FieldMetaInfo("品质2概率（品质1=100-品质2-品质3-品质4，无需填）", "int", 60)},
            {"Quality3Rate", new FieldMetaInfo("品质3概率", "int", 60)},
            {"Quality4Rate", new FieldMetaInfo("品质4概率", "int", 60)},
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
        ///回合名字
        /// </summary>
        public string Name;
        /// <summary>
        ///可刷地图权重（格式 地图英文名:权重，逗号分隔，如 forest1v1:30,forest4v4:20；相对权重，≤0不参与）
        /// </summary>
        public string MapWeights;
        /// <summary>
        ///回合加钱
        /// </summary>
        public int RoundGold;
        /// <summary>
        ///多等级概率
        /// </summary>
        public float MultiCardRate;
        /// <summary>
        ///多等级
        /// </summary>
        public int MultiPriceTotal;
        /// <summary>
        ///品质2概率（品质1=100-品质2-品质3-品质4，无需填）
        /// </summary>
        public int Quality2Rate;
        /// <summary>
        ///品质3概率
        /// </summary>
        public int Quality3Rate;
        /// <summary>
        ///品质4概率
        /// </summary>
        public int Quality4Rate;
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


        public GameRoundConfig(int Id, string Name, string MapWeights, int RoundGold, float MultiCardRate, int MultiPriceTotal, int Quality2Rate, int Quality3Rate, int Quality4Rate, int RoundType, int ShopType, int DamageCount, string SoldierList)
        {
            this.Id = Id;
            this.Name = Name;
            this.MapWeights = MapWeights;
            this.RoundGold = RoundGold;
            this.MultiCardRate = MultiCardRate;
            this.MultiPriceTotal = MultiPriceTotal;
            this.Quality2Rate = Quality2Rate;
            this.Quality3Rate = Quality3Rate;
            this.Quality4Rate = Quality4Rate;
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
            config[1] = new GameRoundConfig(1, "180年", "forest1v1:1", 2, 0f, 2, 0, 0, 0, 1, 0, 1, "590001;0|590001;23");
            config[2] = new GameRoundConfig(2, "181年", "forest1v1:1", 3, 0f, 3, 20, 0, 0, 1, 0, 1, "590001;0|590001;12|590001;23");
            config[3] = new GameRoundConfig(3, "182年", "forest1v1:1", 4, 0f, 4, 0, 0, 0, 0, 1, 1, "590001;0|590001;12|590001;23");
            config[4] = new GameRoundConfig(4, "183年", "forest1v1:1", 5, 0f, 5, 0, 0, 0, 0, 1, 1, "590001;0|590001;12|590001;23");
            config[5] = new GameRoundConfig(5, "184年", "forest1v1:1,forest4v4:1,islandteam:1", 6, 0f, 6, 0, 0, 0, 0, 1, 1, "590001;0|590001;12|590001;23");
            config[6] = new GameRoundConfig(6, "185年", "forest1v1:1,forest4v4:1,islandteam:1", 7, 0f, 7, 0, 0, 0, 0, 1, 1, "590001;0|590001;12|590001;23");
            config[7] = new GameRoundConfig(7, "186年", "forest1v1:2,river:1,forest4v4:1,islandteam:1", 8, 0f, 8, 0, 0, 0, 0, 1, 2, "590001;0|590001;12|590001;23");
            config[8] = new GameRoundConfig(8, "187年", "forest1v1:2,river:1,forest4v4:1,islandteam:1", 9, 0f, 9, 0, 0, 0, 0, 1, 2, "590001;0|590001;12|590001;23");
            config[9] = new GameRoundConfig(9, "188年", "forest1v1:1,river:1", 10, 0f, 10, 30, 15, 10, 1, 0, 2, "590001;0|590001;12|590001;23");
            config[10] = new GameRoundConfig(10, "189年", "iceland:1", 11, 5f, 11, 0, 0, 0, 0, 1, 2, "590001;0|590001;12|590001;23");
            config[11] = new GameRoundConfig(11, "190年", "forest1v1:1,forest4v4:1,islandteam:1", 12, 5f, 12, 0, 0, 0, 0, 1, 2, "590001;0|590001;12|590001;23");
            config[12] = new GameRoundConfig(12, "191年", "river:1", 13, 5f, 13, 0, 0, 0, 0, 1, 2, "590001;0|590001;12|590001;23");
            config[13] = new GameRoundConfig(13, "192年", "forest1v1:1,forest4v4:1,islandteam:1", 14, 5f, 14, 30, 15, 10, 0, 0, 3, "590001;0|590001;12|590001;23");
            config[14] = new GameRoundConfig(14, "193年", "forest1v1:1,river:1,forest4v4:1,islandteam:1", 15, 5f, 15, 0, 0, 0, 0, 1, 3, "590001;0|590001;12|590001;23");
            config[15] = new GameRoundConfig(15, "194年", "forest1v1:1,forest4v4:1,islandteam:1", 16, 5f, 16, 0, 0, 0, 0, 1, 3, "590001;0|590001;12|590001;23");
            config[16] = new GameRoundConfig(16, "195年", "forest1v1:1,river:1,forest4v4:1,islandteam:1", 17, 5f, 17, 0, 0, 0, 0, 1, 3, "590001;0|590001;12|590001;23");
            config[17] = new GameRoundConfig(17, "196年", "forest1v1:1", 18, 5f, 18, 30, 25, 20, 1, 0, 3, "590001;0|590001;12|590001;23");
            config[18] = new GameRoundConfig(18, "197年", "forest1v1:1,forest4v4:1,islandteam:1", 19, 5f, 19, 0, 0, 0, 0, 1, 3, "590001;0|590001;12|590001;23");
            config[19] = new GameRoundConfig(19, "198年", "forest4v4:1,islandteam:1", 20, 5f, 20, 0, 0, 0, 0, 1, 3, "590001;0|590001;12|590001;23");
            config[20] = new GameRoundConfig(20, "199年", "iceland:1", 20, 10f, 21, 0, 0, 0, 0, 1, 3, "590001;0|590001;12|590001;23");
            config[21] = new GameRoundConfig(21, "200年", "forest1v1:1,forest4v4:1,islandteam:1", 20, 10f, 22, 30, 25, 20, 0, 0, 4, "590001;0|590001;12|590001;23");
            config[22] = new GameRoundConfig(22, "201年", "forest1v1:1,forest4v4:1,islandteam:1", 20, 10f, 23, 0, 0, 0, 0, 1, 4, "590001;0|590001;12|590001;23");
            config[23] = new GameRoundConfig(23, "202年", "forest1v1:1,river:1,forest4v4:1,islandteam:1", 20, 10f, 24, 0, 0, 0, 0, 1, 4, "590001;0|590001;12|590001;23");
            config[24] = new GameRoundConfig(24, "203年", "forest1v1:1,forest4v4:1,islandteam:1", 20, 10f, 25, 0, 0, 0, 0, 1, 4, "590001;0|590001;12|590001;23");
            config[25] = new GameRoundConfig(25, "204年", "forest1v1:1", 20, 10f, 26, 40, 40, 20, 1, 0, 4, "590001;0|590001;12|590001;23");
            config[26] = new GameRoundConfig(26, "205年", "forest1v1:1,forest4v4:1,islandteam:1", 20, 10f, 27, 0, 0, 0, 0, 1, 4, "590001;0|590001;12|590001;23");
            config[27] = new GameRoundConfig(27, "206年", "forest4v4:1,islandteam:1", 20, 10f, 28, 0, 0, 0, 0, 1, 4, "590001;0|590001;12|590001;23");
            config[28] = new GameRoundConfig(28, "207年", "forest1v1:1,river:1,forest4v4:1,islandteam:1", 20, 10f, 29, 0, 0, 0, 0, 1, 4, "590001;0|590001;12|590001;23");
            config[29] = new GameRoundConfig(29, "208年", "forest1v1:1,forest4v4:1,islandteam:1", 20, 10f, 30, 40, 40, 20, 0, 0, 5, "590001;0|590001;12|590001;23");
            config[30] = new GameRoundConfig(30, "209年", "iceland:1", 20, 15f, 31, 0, 0, 0, 0, 1, 5, "590001;0|590001;12|590001;23");
            config[31] = new GameRoundConfig(31, "210年", "forest1v1:1,forest4v4:1,islandteam:1", 20, 15f, 32, 0, 0, 0, 0, 1, 5, "590001;0|590001;12|590001;23");
            config[32] = new GameRoundConfig(32, "211年", "forest1v1:1,forest4v4:1,islandteam:1", 20, 15f, 33, 0, 0, 0, 0, 1, 5, "590001;0|590001;12|590001;23");
            config[33] = new GameRoundConfig(33, "212年", "forest1v1:1", 20, 15f, 34, 35, 35, 30, 1, 0, 5, "590001;0|590001;12|590001;23");
            config[34] = new GameRoundConfig(34, "213年", "forest4v4:1,islandteam:1", 20, 15f, 35, 0, 0, 0, 0, 1, 5, "590001;0|590001;12|590001;23");
            config[35] = new GameRoundConfig(35, "214年", "forest1v1:1,forest4v4:1,islandteam:1", 20, 15f, 36, 0, 0, 0, 0, 1, 5, "590001;0|590001;12|590001;23");
            config[36] = new GameRoundConfig(36, "215年", "forest1v1:1,river:1,forest4v4:1,islandteam:1", 20, 15f, 37, 0, 0, 0, 0, 1, 5, "590001;0|590001;12|590001;23");
            config[37] = new GameRoundConfig(37, "216年", "forest1v1:1,river:1,forest4v4:1,islandteam:1", 20, 15f, 38, 35, 35, 30, 0, 0, 6, "590001;0|590001;12|590001;23");
            config[38] = new GameRoundConfig(38, "217年", "forest1v1:1,forest4v4:1,islandteam:1", 20, 15f, 39, 0, 0, 0, 0, 1, 6, "590001;0|590001;12|590001;23");
            config[39] = new GameRoundConfig(39, "218年", "forest1v1:1,forest4v4:1,islandteam:1", 20, 15f, 40, 0, 0, 0, 0, 1, 6, "590001;0|590001;12|590001;23");
            config[40] = new GameRoundConfig(40, "219年", "iceland:1", 20, 20f, 41, 0, 0, 0, 0, 1, 6, "590001;0|590001;12|590001;23");
            config[41] = new GameRoundConfig(41, "220年", "river:1", 20, 20f, 42, 25, 35, 40, 1, 0, 6, "590001;0|590001;12|590001;23");
            config[42] = new GameRoundConfig(42, "221年", "forest1v1:1,forest4v4:1,islandteam:1", 20, 20f, 43, 0, 0, 0, 0, 1, 6, "590001;0|590001;12|590001;23");
            config[43] = new GameRoundConfig(43, "222年", "forest1v1:1,forest4v4:1,islandteam:1", 20, 20f, 44, 0, 0, 0, 0, 1, 6, "590001;0|590001;12|590001;23");
            config[44] = new GameRoundConfig(44, "223年", "forest4v4:1,islandteam:1", 20, 20f, 45, 0, 0, 0, 0, 1, 6, "590001;0|590001;12|590001;23");
            config[45] = new GameRoundConfig(45, "224年", "forest1v1:1,forest4v4:1,islandteam:1", 20, 20f, 46, 25, 35, 40, 0, 0, 6, "590001;0|590001;12|590001;23");
            config[46] = new GameRoundConfig(46, "225年", "forest4v4:1,islandteam:1", 20, 20f, 47, 0, 0, 0, 0, 1, 7, "590001;0|590001;12|590001;23");
            config[47] = new GameRoundConfig(47, "226年", "forest1v1:1,forest4v4:1,islandteam:1", 20, 20f, 48, 0, 0, 0, 0, 1, 7, "590001;0|590001;12|590001;23");
            config[48] = new GameRoundConfig(48, "227年", "forest1v1:1,forest4v4:1,islandteam:1", 20, 20f, 49, 0, 0, 0, 0, 1, 7, "590001;0|590001;12|590001;23");
            config[49] = new GameRoundConfig(49, "228年", "forest1v1:1", 20, 20f, 50, 25, 30, 45, 1, 0, 7, "590001;0|590001;12|590001;23");
            config[50] = new GameRoundConfig(50, "229年", "iceland:1", 20, 25f, 51, 0, 0, 0, 0, 1, 8, "590001;0|590001;12|590001;23");
            config[51] = new GameRoundConfig(51, "230年", "forest1v1:1,forest4v4:1,islandteam:1", 20, 25f, 52, 0, 0, 0, 0, 1, 8, "590001;0|590001;12|590001;23");
            config[52] = new GameRoundConfig(52, "231年", "forest1v1:1,river:1,forest4v4:1,islandteam:1", 20, 25f, 53, 0, 0, 0, 0, 1, 8, "590001;0|590001;12|590001;23");
            config[53] = new GameRoundConfig(53, "232年", "river:1", 20, 25f, 54, 25, 30, 45, 0, 0, 8, "590001;0|590001;12|590001;23");
            config[54] = new GameRoundConfig(54, "233年", "forest1v1:1,river:1,forest4v4:1,islandteam:1", 20, 25f, 55, 0, 0, 0, 0, 1, 9, "590001;0|590001;12|590001;23");
            config[55] = new GameRoundConfig(55, "234年", ",forest4v4:1,islandteam:1", 20, 25f, 56, 0, 0, 0, 0, 1, 9, "590001;0|590001;12|590001;23");
            config[56] = new GameRoundConfig(56, "235年", "river:1", 20, 25f, 57, 0, 0, 0, 0, 1, 9, "590001;0|590001;12|590001;23");
            config[57] = new GameRoundConfig(57, "236年", "river:1", 20, 25f, 58, 20, 30, 50, 1, 0, 9, "590001;0|590001;12|590001;23");
            config[58] = new GameRoundConfig(58, "237年", "forest1v1:1,forest4v4:1,islandteam:1", 20, 25f, 59, 0, 0, 0, 0, 1, 10, "590001;0|590001;12|590001;23");
            config[59] = new GameRoundConfig(59, "238年", "forest1v1:1,forest4v4:1,islandteam:1", 20, 25f, 60, 0, 0, 0, 0, 1, 10, "590001;0|590001;12|590001;23");
            config[60] = new GameRoundConfig(60, "239年", "iceland:1", 20, 30f, 61, 0, 0, 0, 0, 1, 10, "590001;0|590001;12|590001;23");
            config[61] = new GameRoundConfig(61, "240年", "forest1v1:1,forest4v4:1,islandteam:1", 20, 30f, 62, 20, 30, 50, 0, 0, 10, "590001;0|590001;12|590001;23");
            config[62] = new GameRoundConfig(62, "241年", "forest1v1:1,forest4v4:1,islandteam:1", 20, 30f, 63, 0, 0, 0, 0, 1, 10, "590001;0|590001;12|590001;23");
            config[63] = new GameRoundConfig(63, "242年", "river:1", 20, 30f, 64, 0, 0, 0, 0, 1, 10, "590001;0|590001;12|590001;23");
            config[64] = new GameRoundConfig(64, "243年", "forest1v1:1,forest4v4:1,islandteam:1", 20, 30f, 65, 0, 0, 0, 0, 1, 10, "590001;0|590001;12|590001;23");
            config[65] = new GameRoundConfig(65, "244年", "forest1v1:1", 20, 30f, 66, 15, 35, 50, 1, 0, 10, "590001;0|590001;12|590001;23");
            config[66] = new GameRoundConfig(66, "245年", "forest1v1:1,forest4v4:1,islandteam:1", 20, 30f, 67, 0, 0, 0, 0, 1, 10, "590001;0|590001;12|590001;23");
            config[67] = new GameRoundConfig(67, "246年", "forest1v1:1,forest4v4:1,islandteam:1", 20, 30f, 68, 0, 0, 0, 0, 1, 10, "590001;0|590001;12|590001;23");
            config[68] = new GameRoundConfig(68, "247年", "forest1v1:1,river:1,forest4v4:1,islandteam:1", 20, 30f, 69, 15, 35, 50, 0, 1, 10, "590001;0|590001;12|590001;23");
            config[69] = new GameRoundConfig(69, "248年", "forest4v4:1,islandteam:1", 20, 30f, 70, 0, 0, 0, 0, 0, 10, "590001;0|590001;12|590001;23");
            config[70] = new GameRoundConfig(70, "249年", "iceland:1", 20, 35f, 71, 0, 0, 0, 0, 1, 10, "590001;0|590001;12|590001;23");
            config[71] = new GameRoundConfig(71, "250年", "forest1v1:1,forest4v4:1,islandteam:1", 20, 35f, 72, 0, 0, 0, 0, 1, 10, "590001;0|590001;12|590001;23");
            config[72] = new GameRoundConfig(72, "251年", "river:1", 20, 35f, 73, 0, 0, 0, 0, 1, 10, "590001;0|590001;12|590001;23");
            config[73] = new GameRoundConfig(73, "252年", "forest1v1:1", 20, 35f, 74, 20, 35, 45, 1, 0, 10, "590001;0|590001;12|590001;23");
            config[74] = new GameRoundConfig(74, "253年", "forest1v1:1,forest4v4:1,islandteam:1", 20, 35f, 75, 0, 0, 0, 0, 1, 10, "590001;0|590001;12|590001;23");
            config[75] = new GameRoundConfig(75, "254年", "forest1v1:1,river:1,forest4v4:1,islandteam:1", 20, 35f, 76, 0, 0, 0, 0, 1, 10, "590001;0|590001;12|590001;23");
            config[76] = new GameRoundConfig(76, "255年", "forest4v4:1,islandteam:1", 20, 35f, 77, 0, 0, 0, 0, 1, 10, "590001;0|590001;12|590001;23");
            config[77] = new GameRoundConfig(77, "256年", "forest1v1:1,forest4v4:1,islandteam:1", 20, 35f, 78, 20, 35, 45, 0, 0, 10, "590001;0|590001;12|590001;23");
            config[78] = new GameRoundConfig(78, "257年", "forest4v4:1,islandteam:1", 20, 35f, 79, 0, 0, 0, 0, 1, 10, "590001;0|590001;12|590001;23");
            config[79] = new GameRoundConfig(79, "258年", "forest1v1:1,forest4v4:1,islandteam:1", 20, 35f, 80, 0, 0, 0, 0, 1, 10, "590001;0|590001;12|590001;23");
            config[80] = new GameRoundConfig(80, "259年", "iceland:1", 20, 40f, 81, 0, 0, 0, 0, 1, 10, "590001;0|590001;12|590001;23");
            config[81] = new GameRoundConfig(81, "260年", "forest1v1:1", 20, 40f, 82, 25, 35, 45, 1, 0, 10, "590001;0|590001;12|590001;23");
            config[82] = new GameRoundConfig(82, "261年", "forest1v1:1,forest4v4:1,islandteam:1", 20, 40f, 83, 0, 0, 0, 0, 1, 10, "590001;0|590001;12|590001;23");
            config[83] = new GameRoundConfig(83, "262年", "river:1", 20, 40f, 84, 0, 0, 0, 0, 1, 10, "590001;0|590001;12|590001;23");
            config[84] = new GameRoundConfig(84, "263年", "river:1", 20, 40f, 85, 0, 0, 0, 0, 1, 10, "590001;0|590001;12|590001;23");
            config[85] = new GameRoundConfig(85, "264年", "forest1v1:1,river:1,forest4v4:1,islandteam:1", 20, 40f, 86, 25, 35, 45, 0, 0, 10, "590001;0|590001;12|590001;23");
            config[86] = new GameRoundConfig(86, "265年", "forest1v1:1,forest4v4:1,islandteam:1", 20, 40f, 87, 0, 0, 0, 0, 1, 10, "590001;0|590001;12|590001;23");
            config[87] = new GameRoundConfig(87, "266年", "forest1v1:1,river:1,forest4v4:1,islandteam:1", 20, 40f, 88, 0, 0, 0, 0, 1, 10, "590001;0|590001;12|590001;23");
            config[88] = new GameRoundConfig(88, "267年", "forest1v1:1,forest4v4:1,islandteam:1", 20, 40f, 89, 0, 0, 0, 0, 1, 10, "590001;0|590001;12|590001;23");
            config[89] = new GameRoundConfig(89, "268年", "forest1v1:1", 20, 40f, 90, 25, 35, 45, 1, 0, 10, "590001;0|590001;12|590001;23");
            config[90] = new GameRoundConfig(90, "269年", "iceland:1", 20, 45f, 91, 0, 0, 0, 0, 1, 10, "590001;0|590001;12|590001;23");
            config[91] = new GameRoundConfig(91, "270年", "forest1v1:1,forest4v4:1,islandteam:1", 20, 45f, 92, 0, 0, 0, 0, 1, 10, "590001;0|590001;12|590001;23");
            config[92] = new GameRoundConfig(92, "271年", "forest4v4:1,islandteam:1", 20, 45f, 93, 0, 0, 0, 0, 1, 10, "590001;0|590001;12|590001;23");
            config[93] = new GameRoundConfig(93, "272年", "forest1v1:1,forest4v4:1,islandteam:1", 20, 45f, 94, 0, 0, 0, 0, 1, 10, "590001;0|590001;12|590001;23");
            config[94] = new GameRoundConfig(94, "273年", "forest1v1:1,forest4v4:1,islandteam:1", 20, 45f, 95, 0, 0, 0, 0, 1, 10, "590001;0|590001;12|590001;23");
            config[95] = new GameRoundConfig(95, "274年", "forest1v1:1,forest4v4:1,islandteam:1", 20, 45f, 96, 0, 0, 0, 0, 1, 10, "590001;0|590001;12|590001;23");
            config[96] = new GameRoundConfig(96, "275年", "forest1v1:1,forest4v4:1,islandteam:1", 20, 45f, 97, 0, 0, 0, 0, 1, 10, "590001;0|590001;12|590001;23");
            config[97] = new GameRoundConfig(97, "276年", "forest1v1:1", 20, 45f, 98, 25, 35, 45, 1, 0, 10, "590001;0|590001;12|590001;23");
            config[98] = new GameRoundConfig(98, "277年", "forest1v1:1,forest4v4:1,islandteam:1", 20, 45f, 99, 0, 0, 0, 0, 1, 10, "590001;0|590001;12|590001;23");
            config[99] = new GameRoundConfig(99, "278年", "forest1v1:1,forest4v4:1,islandteam:1", 20, 45f, 100, 0, 0, 0, 0, 1, 10, "590001;0|590001;12|590001;23");
            config[100] = new GameRoundConfig(100, "279年", "iceland:1", 20, 50f, 101, 0, 0, 0, 0, 1, 10, "590001;0|590001;12|590001;23");

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
