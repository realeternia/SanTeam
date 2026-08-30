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
            {"MapIds", new FieldMetaInfo("可刷地图", "int[]", 0)},
            {"RoundGold", new FieldMetaInfo("回合加钱", "int", 60)},
            {"MultiCardRate", new FieldMetaInfo("多等级概率", "float", 60)},
            {"MultiPriceTotal", new FieldMetaInfo("多等级", "int", 60)},
            {"ItemCount", new FieldMetaInfo("道具数量", "int", 60)},
            {"ItemAmazingCount", new FieldMetaInfo("道具大量", "int", 60)},
            {"Quality2Rate", new FieldMetaInfo("品质2概率（品质1=100-品质2-品质3-品质4，无需填）", "int", 60)},
            {"Quality3Rate", new FieldMetaInfo("品质3概率", "int", 60)},
            {"Quality4Rate", new FieldMetaInfo("品质4概率", "int", 60)},
            {"RoundType", new FieldMetaInfo("类型（0=PVP，1=PVE）", "int", 132)},
            {"SoldierList", new FieldMetaInfo("PVE怪物布阵（怪物id;布阵格|怪物id;布阵格，布阵格0~24：0=左上，24=右下）", "string", 0)},
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
        ///可刷地图
        /// </summary>
        public int[] MapIds;
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
        ///道具数量
        /// </summary>
        public int ItemCount;
        /// <summary>
        ///道具大量
        /// </summary>
        public int ItemAmazingCount;
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
        ///PVE怪物布阵（怪物id;布阵格|怪物id;布阵格，布阵格0~24：0=左上，24=右下，PVE时生效）
        /// </summary>
        public string SoldierList;


        public GameRoundConfig(int Id, string Name, int[] MapIds, int RoundGold, float MultiCardRate, int MultiPriceTotal, int ItemCount, int ItemAmazingCount, int Quality2Rate, int Quality3Rate, int Quality4Rate, int RoundType, string SoldierList)
        {
            this.Id = Id;
            this.Name = Name;
            this.MapIds = MapIds;
            this.RoundGold = RoundGold;
            this.MultiCardRate = MultiCardRate;
            this.MultiPriceTotal = MultiPriceTotal;
            this.ItemCount = ItemCount;
            this.ItemAmazingCount = ItemAmazingCount;
            this.Quality2Rate = Quality2Rate;
            this.Quality3Rate = Quality3Rate;
            this.Quality4Rate = Quality4Rate;
            this.RoundType = RoundType;
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
            config[1] = new GameRoundConfig(1, "180年", new int[] { 1 }, 2, 0f, 2, 0, 0, 20, 0, 0, 1, "590001;0|590001;23");
            config[2] = new GameRoundConfig(2, "181年", new int[] { 1 }, 3, 0f, 3, 0, 0, 20, 0, 0, 1, "590001;0|590001;12|590001;23");
            config[3] = new GameRoundConfig(3, "182年", new int[]{1}, 4, 0f, 4, 0, 0, 20, 5, 0, 0, "");
            config[4] = new GameRoundConfig(4, "183年", new int[]{1}, 5, 0f, 5, 1, 0, 20, 5, 0, 0, "");
            config[5] = new GameRoundConfig(5, "184年", new int[]{1,2,3,4}, 6, 0f, 6, 1, 0, 20, 5, 0, 0, "");
            config[6] = new GameRoundConfig(6, "185年", new int[]{1,2,3,4}, 7, 0f, 7, 1, 5, 25, 7, 0, 0, "");
            config[7] = new GameRoundConfig(7, "186年", new int[]{1,2,3,4}, 8, 0f, 8, 2, 5, 25, 7, 0, 0, "");
            config[8] = new GameRoundConfig(8, "187年", new int[]{1,2,3,4}, 9, 0f, 9, 2, 5, 25, 7, 0, 0, "");
            config[9] = new GameRoundConfig(9, "188年", new int[]{1,2,3,4}, 10, 0f, 10, 2, 5, 30, 10, 3, 0, "");
            config[10] = new GameRoundConfig(10, "189年", new int[]{1,2,3,4}, 11, 5f, 11, 2, 5, 30, 10, 3, 0, "");
            config[11] = new GameRoundConfig(11, "190年", new int[]{1,2,3,4}, 12, 5f, 12, 3, 10, 30, 10, 3, 0, "");
            config[12] = new GameRoundConfig(12, "191年", new int[]{1,2,3,4}, 13, 5f, 13, 3, 10, 30, 10, 3, 0, "");
            config[13] = new GameRoundConfig(13, "192年", new int[]{1,2,3,4}, 14, 5f, 14, 3, 10, 35, 15, 5, 0, "");
            config[14] = new GameRoundConfig(14, "193年", new int[]{1,2,3,4}, 15, 5f, 15, 3, 10, 35, 15, 5, 0, "");
            config[15] = new GameRoundConfig(15, "194年", new int[]{1,2,3,4}, 16, 5f, 16, 4, 10, 35, 15, 5, 0, "");
            config[16] = new GameRoundConfig(16, "195年", new int[]{1,2,3,4}, 17, 5f, 17, 4, 20, 35, 15, 5, 0, "");
            config[17] = new GameRoundConfig(17, "196年", new int[]{1,2,3,4}, 18, 5f, 18, 4, 20, 35, 15, 5, 0, "");
            config[18] = new GameRoundConfig(18, "197年", new int[]{1,2,3,4}, 19, 5f, 19, 4, 20, 40, 20, 6, 0, "");
            config[19] = new GameRoundConfig(19, "198年", new int[]{1,2,3,4}, 20, 5f, 20, 5, 20, 40, 20, 6, 0, "");
            config[20] = new GameRoundConfig(20, "199年", new int[]{1,2,3,4}, 21, 10f, 21, 5, 20, 40, 20, 6, 0, "");
            config[21] = new GameRoundConfig(21, "200年", new int[]{1,2,3,4}, 22, 10f, 22, 5, 50, 40, 20, 6, 0, "");
            config[22] = new GameRoundConfig(22, "201年", new int[]{1,2,3,4}, 23, 10f, 23, 5, 50, 40, 20, 7, 0, "");
            config[23] = new GameRoundConfig(23, "202年", new int[]{1,2,3,4}, 24, 10f, 24, 5, 50, 40, 20, 7, 0, "");
            config[24] = new GameRoundConfig(24, "203年", new int[]{1,2,3,4}, 25, 10f, 25, 5, 50, 40, 20, 7, 0, "");
            config[25] = new GameRoundConfig(25, "204年", new int[]{1,2,3,4}, 26, 10f, 26, 6, 50, 40, 20, 7, 0, "");
            config[26] = new GameRoundConfig(26, "205年", new int[]{1,2,3,4}, 27, 10f, 27, 6, 50, 40, 20, 7, 0, "");
            config[27] = new GameRoundConfig(27, "206年", new int[]{1,2,3,4}, 28, 10f, 28, 6, 50, 40, 20, 7, 0, "");
            config[28] = new GameRoundConfig(28, "207年", new int[]{1,2,3,4}, 29, 10f, 29, 6, 50, 40, 20, 7, 0, "");
            config[29] = new GameRoundConfig(29, "208年", new int[]{1,2,3,4}, 30, 10f, 30, 6, 50, 40, 20, 7, 0, "");
            config[30] = new GameRoundConfig(30, "209年", new int[]{1,2,3,4}, 31, 15f, 31, 6, 50, 40, 20, 7, 0, "");
            config[31] = new GameRoundConfig(31, "210年", new int[]{1,2,3,4}, 32, 15f, 32, 6, 50, 40, 20, 7, 0, "");
            config[32] = new GameRoundConfig(32, "211年", new int[]{1,2,3,4}, 33, 15f, 33, 6, 50, 40, 20, 7, 0, "");
            config[33] = new GameRoundConfig(33, "212年", new int[]{1,2,3,4}, 34, 15f, 34, 6, 50, 40, 20, 7, 0, "");
            config[34] = new GameRoundConfig(34, "213年", new int[]{1,2,3,4}, 35, 15f, 35, 6, 50, 40, 20, 7, 0, "");
            config[35] = new GameRoundConfig(35, "214年", new int[]{1,2,3,4}, 36, 15f, 36, 6, 50, 40, 20, 7, 0, "");
            config[36] = new GameRoundConfig(36, "215年", new int[]{1,2,3,4}, 37, 15f, 37, 6, 50, 40, 20, 7, 0, "");
            config[37] = new GameRoundConfig(37, "216年", new int[]{1,2,3,4}, 38, 15f, 38, 6, 50, 40, 20, 7, 0, "");
            config[38] = new GameRoundConfig(38, "217年", new int[]{1,2,3,4}, 39, 15f, 39, 6, 50, 40, 20, 7, 0, "");
            config[39] = new GameRoundConfig(39, "218年", new int[]{1,2,3,4}, 40, 15f, 40, 6, 50, 40, 20, 7, 0, "");
            config[40] = new GameRoundConfig(40, "219年", new int[]{1,2,3,4}, 41, 20f, 41, 6, 50, 40, 20, 7, 0, "");
            config[41] = new GameRoundConfig(41, "220年", new int[]{1,2,3,4}, 42, 20f, 42, 6, 50, 40, 20, 8, 0, "");
            config[42] = new GameRoundConfig(42, "221年", new int[]{1,2,3,4}, 43, 20f, 43, 6, 50, 40, 20, 8, 0, "");
            config[43] = new GameRoundConfig(43, "222年", new int[]{1,2,3,4}, 44, 20f, 44, 6, 50, 40, 20, 8, 0, "");
            config[44] = new GameRoundConfig(44, "223年", new int[]{1,2,3,4}, 45, 20f, 45, 6, 50, 40, 20, 8, 0, "");
            config[45] = new GameRoundConfig(45, "224年", new int[]{1,2,3,4}, 46, 20f, 46, 6, 50, 40, 20, 8, 0, "");
            config[46] = new GameRoundConfig(46, "225年", new int[]{1,2,3,4}, 47, 20f, 47, 6, 50, 40, 20, 8, 0, "");
            config[47] = new GameRoundConfig(47, "226年", new int[]{1,2,3,4}, 48, 20f, 48, 6, 50, 40, 20, 8, 0, "");
            config[48] = new GameRoundConfig(48, "227年", new int[]{1,2,3,4}, 49, 20f, 49, 6, 50, 40, 20, 8, 0, "");
            config[49] = new GameRoundConfig(49, "228年", new int[]{1,2,3,4}, 50, 20f, 50, 6, 50, 40, 20, 8, 0, "");
            config[50] = new GameRoundConfig(50, "229年", new int[]{1,2,3,4}, 51, 25f, 51, 6, 50, 40, 20, 8, 0, "");
            config[51] = new GameRoundConfig(51, "230年", new int[]{1,2,3,4}, 52, 25f, 52, 6, 50, 40, 20, 8, 0, "");
            config[52] = new GameRoundConfig(52, "231年", new int[]{1,2,3,4}, 53, 25f, 53, 6, 50, 40, 20, 8, 0, "");
            config[53] = new GameRoundConfig(53, "232年", new int[]{1,2,3,4}, 54, 25f, 54, 6, 50, 40, 20, 8, 0, "");
            config[54] = new GameRoundConfig(54, "233年", new int[]{1,2,3,4}, 55, 25f, 55, 6, 50, 40, 20, 8, 0, "");
            config[55] = new GameRoundConfig(55, "234年", new int[]{1,2,3,4}, 56, 25f, 56, 6, 50, 40, 20, 8, 0, "");
            config[56] = new GameRoundConfig(56, "235年", new int[]{1,2,3,4}, 57, 25f, 57, 6, 50, 40, 20, 8, 0, "");
            config[57] = new GameRoundConfig(57, "236年", new int[]{1,2,3,4}, 58, 25f, 58, 6, 50, 40, 20, 8, 0, "");
            config[58] = new GameRoundConfig(58, "237年", new int[]{1,2,3,4}, 59, 25f, 59, 6, 50, 40, 20, 8, 0, "");
            config[59] = new GameRoundConfig(59, "238年", new int[]{1,2,3,4}, 60, 25f, 60, 6, 50, 40, 20, 8, 0, "");
            config[60] = new GameRoundConfig(60, "239年", new int[]{1,2,3,4}, 61, 30f, 61, 6, 50, 40, 20, 8, 0, "");
            config[61] = new GameRoundConfig(61, "240年", new int[]{1,2,3,4}, 62, 30f, 62, 6, 50, 40, 20, 9, 0, "");
            config[62] = new GameRoundConfig(62, "241年", new int[]{1,2,3,4}, 63, 30f, 63, 6, 50, 40, 20, 9, 0, "");
            config[63] = new GameRoundConfig(63, "242年", new int[]{1,2,3,4}, 64, 30f, 64, 6, 50, 40, 20, 9, 0, "");
            config[64] = new GameRoundConfig(64, "243年", new int[]{1,2,3,4}, 65, 30f, 65, 6, 50, 40, 20, 9, 0, "");
            config[65] = new GameRoundConfig(65, "244年", new int[]{1,2,3,4}, 66, 30f, 66, 6, 50, 40, 20, 9, 0, "");
            config[66] = new GameRoundConfig(66, "245年", new int[]{1,2,3,4}, 67, 30f, 67, 6, 50, 40, 20, 9, 0, "");
            config[67] = new GameRoundConfig(67, "246年", new int[]{1,2,3,4}, 68, 30f, 68, 6, 50, 40, 20, 9, 0, "");
            config[68] = new GameRoundConfig(68, "247年", new int[]{1,2,3,4}, 69, 30f, 69, 6, 50, 40, 20, 9, 0, "");
            config[69] = new GameRoundConfig(69, "248年", new int[]{1,2,3,4}, 70, 30f, 70, 6, 50, 40, 20, 9, 0, "");
            config[70] = new GameRoundConfig(70, "249年", new int[]{1,2,3,4}, 71, 35f, 71, 6, 50, 40, 20, 9, 0, "");
            config[71] = new GameRoundConfig(71, "250年", new int[]{1,2,3,4}, 72, 35f, 72, 6, 50, 40, 20, 9, 0, "");
            config[72] = new GameRoundConfig(72, "251年", new int[]{1,2,3,4}, 73, 35f, 73, 6, 50, 40, 20, 9, 0, "");
            config[73] = new GameRoundConfig(73, "252年", new int[]{1,2,3,4}, 74, 35f, 74, 6, 50, 40, 20, 9, 0, "");
            config[74] = new GameRoundConfig(74, "253年", new int[]{1,2,3,4}, 75, 35f, 75, 6, 50, 40, 20, 9, 0, "");
            config[75] = new GameRoundConfig(75, "254年", new int[]{1,2,3,4}, 76, 35f, 76, 6, 50, 40, 20, 9, 0, "");
            config[76] = new GameRoundConfig(76, "255年", new int[]{1,2,3,4}, 77, 35f, 77, 6, 50, 40, 20, 9, 0, "");
            config[77] = new GameRoundConfig(77, "256年", new int[]{1,2,3,4}, 78, 35f, 78, 6, 50, 40, 20, 9, 0, "");
            config[78] = new GameRoundConfig(78, "257年", new int[]{1,2,3,4}, 79, 35f, 79, 6, 50, 40, 20, 9, 0, "");
            config[79] = new GameRoundConfig(79, "258年", new int[]{1,2,3,4}, 80, 35f, 80, 6, 50, 40, 20, 9, 0, "");
            config[80] = new GameRoundConfig(80, "259年", new int[]{1,2,3,4}, 81, 40f, 81, 6, 50, 40, 20, 9, 0, "");
            config[81] = new GameRoundConfig(81, "260年", new int[]{1,2,3,4}, 82, 40f, 82, 6, 50, 40, 20, 10, 0, "");
            config[82] = new GameRoundConfig(82, "261年", new int[]{1,2,3,4}, 83, 40f, 83, 6, 50, 40, 20, 10, 0, "");
            config[83] = new GameRoundConfig(83, "262年", new int[]{1,2,3,4}, 84, 40f, 84, 6, 50, 40, 20, 10, 0, "");
            config[84] = new GameRoundConfig(84, "263年", new int[]{1,2,3,4}, 85, 40f, 85, 6, 50, 40, 20, 10, 0, "");
            config[85] = new GameRoundConfig(85, "264年", new int[]{1,2,3,4}, 86, 40f, 86, 6, 50, 40, 20, 10, 0, "");
            config[86] = new GameRoundConfig(86, "265年", new int[]{1,2,3,4}, 87, 40f, 87, 6, 50, 40, 20, 10, 0, "");
            config[87] = new GameRoundConfig(87, "266年", new int[]{1,2,3,4}, 88, 40f, 88, 6, 50, 40, 20, 10, 0, "");
            config[88] = new GameRoundConfig(88, "267年", new int[]{1,2,3,4}, 89, 40f, 89, 6, 50, 40, 20, 10, 0, "");
            config[89] = new GameRoundConfig(89, "268年", new int[]{1,2,3,4}, 90, 40f, 90, 6, 50, 40, 20, 10, 0, "");
            config[90] = new GameRoundConfig(90, "269年", new int[]{1,2,3,4}, 91, 45f, 91, 6, 50, 40, 20, 10, 0, "");
            config[91] = new GameRoundConfig(91, "270年", new int[]{1,2,3,4}, 92, 45f, 92, 6, 50, 40, 20, 10, 0, "");
            config[92] = new GameRoundConfig(92, "271年", new int[]{1,2,3,4}, 93, 45f, 93, 6, 50, 40, 20, 10, 0, "");
            config[93] = new GameRoundConfig(93, "272年", new int[]{1,2,3,4}, 94, 45f, 94, 6, 50, 40, 20, 10, 0, "");
            config[94] = new GameRoundConfig(94, "273年", new int[]{1,2,3,4}, 95, 45f, 95, 6, 50, 40, 20, 10, 0, "");
            config[95] = new GameRoundConfig(95, "274年", new int[]{1,2,3,4}, 96, 45f, 96, 6, 50, 40, 20, 10, 0, "");
            config[96] = new GameRoundConfig(96, "275年", new int[]{1,2,3,4}, 97, 45f, 97, 6, 50, 40, 20, 10, 0, "");
            config[97] = new GameRoundConfig(97, "276年", new int[]{1,2,3,4}, 98, 45f, 98, 6, 50, 40, 20, 10, 0, "");
            config[98] = new GameRoundConfig(98, "277年", new int[]{1,2,3,4}, 99, 45f, 99, 6, 50, 40, 20, 10, 0, "");
            config[99] = new GameRoundConfig(99, "278年", new int[]{1,2,3,4}, 100, 45f, 100, 6, 50, 40, 20, 10, 0, "");
            config[100] = new GameRoundConfig(100, "279年", new int[]{1,2,3,4}, 101, 50f, 101, 6, 50, 40, 20, 10, 0, "");

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
