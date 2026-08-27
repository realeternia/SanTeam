using System;
using System.Collections;
using System.Collections.Generic;

namespace CommonConfig
{
    public class ShopConfig
    {
        /// <summary>
        ///序列
        /// </summary>
        public int Id;
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


        public ShopConfig(int Id, int RoundGold, float MultiCardRate, int MultiPriceTotal, int ItemCount, int ItemAmazingCount, int Quality2Rate, int Quality3Rate, int Quality4Rate)
        {
            this.Id = Id;
            this.RoundGold = RoundGold;
            this.MultiCardRate = MultiCardRate;
            this.MultiPriceTotal = MultiPriceTotal;
            this.ItemCount = ItemCount;
            this.ItemAmazingCount = ItemAmazingCount;
            this.Quality2Rate = Quality2Rate;
            this.Quality3Rate = Quality3Rate;
            this.Quality4Rate = Quality4Rate;

        }

        public ShopConfig() { }

        private static Dictionary<int, ShopConfig> config = new Dictionary<int, ShopConfig>();
        public static Dictionary<int, ShopConfig>.ValueCollection ConfigList
        {
            get
            {
                return config.Values;
            }
        }

        public static void Refresh(Dictionary<int, ShopConfig> dict)
        {
            config.Clear();
            config = dict;
        }
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
            {"RoundGold", new FieldMetaInfo("回合加钱", "int", 0)},
            {"MultiCardRate", new FieldMetaInfo("多等级概率", "float", 0)},
            {"MultiPriceTotal", new FieldMetaInfo("多等级", "int", 0)},
            {"ItemCount", new FieldMetaInfo("道具数量", "int", 0)},
            {"ItemAmazingCount", new FieldMetaInfo("道具大量", "int", 0)},
            {"Quality2Rate", new FieldMetaInfo("品质2概率（品质1=100-品质2-品质3-品质4，无需填）", "int", 0)},
            {"Quality3Rate", new FieldMetaInfo("品质3概率", "int", 0)},
            {"Quality4Rate", new FieldMetaInfo("品质4概率", "int", 0)},
        };

        public static Dictionary<string, FieldMetaInfo> FieldMeta { get { return fieldMeta; } }

        private static List<CellMeta> cellMeta = new List<CellMeta>();
        public static List<CellMeta> CellMetas { get { return cellMeta; } }

        /// <summary>
        ///序列
        /// </summary>
        public int Id;
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


        public ShopConfig(int Id, int RoundGold, float MultiCardRate, int MultiPriceTotal, int ItemCount, int ItemAmazingCount, int Quality2Rate, int Quality3Rate, int Quality4Rate)
        {
            this.Id = Id;
            this.RoundGold = RoundGold;
            this.MultiCardRate = MultiCardRate;
            this.MultiPriceTotal = MultiPriceTotal;
            this.ItemCount = ItemCount;
            this.ItemAmazingCount = ItemAmazingCount;
            this.Quality2Rate = Quality2Rate;
            this.Quality3Rate = Quality3Rate;
            this.Quality4Rate = Quality4Rate;
        }

        public ShopConfig() { }

        private static Dictionary<int, ShopConfig> config = new Dictionary<int, ShopConfig>();
        public static Dictionary<int, ShopConfig>.ValueCollection ConfigList
        {
            get { return config.Values; }
        }

        public static void Refresh(Dictionary<int, ShopConfig> dict)
        {
            config.Clear();
            config = dict;
            RebuildIndex();
        }

        public static void Load()
        {
            config.Clear();
            config[1] = new ShopConfig(1, 9, 0f, 0, 0, 0, 20, 0, 0);
            config[2] = new ShopConfig(2, 8, 0f, 0, 0, 0, 20, 0, 0);
            config[3] = new ShopConfig(3, 8, 15f, 18, 0, 0, 20, 5, 0);
            config[4] = new ShopConfig(4, 9, 18f, 20, 1, 0, 20, 5, 0);
            config[5] = new ShopConfig(5, 10, 21f, 21, 1, 0, 20, 5, 0);
            config[6] = new ShopConfig(6, 10, 24f, 22, 1, 5, 25, 7, 0);
            config[7] = new ShopConfig(7, 11, 27f, 24, 2, 5, 25, 7, 0);
            config[8] = new ShopConfig(8, 12, 30f, 25, 2, 5, 25, 7, 0);
            config[9] = new ShopConfig(9, 12, 33f, 26, 2, 5, 30, 10, 3);
            config[10] = new ShopConfig(10, 13, 36f, 28, 2, 5, 30, 10, 3);
            config[11] = new ShopConfig(11, 14, 39f, 29, 3, 10, 30, 10, 3);
            config[12] = new ShopConfig(12, 14, 42f, 31, 3, 10, 30, 10, 3);
            config[13] = new ShopConfig(13, 15, 45f, 32, 3, 10, 35, 15, 5);
            config[14] = new ShopConfig(14, 16, 48f, 34, 3, 10, 35, 15, 5);
            config[15] = new ShopConfig(15, 16, 51f, 35, 4, 10, 35, 15, 5);
            config[16] = new ShopConfig(16, 17, 54f, 36, 4, 20, 35, 15, 5);
            config[17] = new ShopConfig(17, 18, 57f, 38, 4, 20, 35, 15, 5);
            config[18] = new ShopConfig(18, 18, 60f, 39, 4, 20, 40, 20, 6);
            config[19] = new ShopConfig(19, 19, 60f, 40, 5, 20, 40, 20, 6);
            config[20] = new ShopConfig(20, 20, 60f, 42, 5, 20, 40, 20, 6);
            config[21] = new ShopConfig(21, 20, 60f, 43, 5, 50, 40, 20, 6);
            config[22] = new ShopConfig(22, 21, 60f, 45, 5, 50, 40, 20, 7);
            config[23] = new ShopConfig(23, 22, 60f, 46, 5, 50, 40, 20, 7);
            config[24] = new ShopConfig(24, 22, 60f, 48, 5, 50, 40, 20, 7);
            config[25] = new ShopConfig(25, 23, 60f, 49, 6, 50, 40, 20, 7);
            config[26] = new ShopConfig(26, 24, 60f, 50, 6, 50, 40, 20, 7);
            config[27] = new ShopConfig(27, 24, 60f, 52, 6, 50, 40, 20, 7);
            config[28] = new ShopConfig(28, 25, 60f, 53, 6, 50, 40, 20, 7);
            config[29] = new ShopConfig(29, 26, 60f, 54, 6, 50, 40, 20, 7);
            config[30] = new ShopConfig(30, 26, 60f, 56, 6, 50, 40, 20, 7);
            config[31] = new ShopConfig(31, 27, 60f, 57, 6, 50, 40, 20, 7);
            config[32] = new ShopConfig(32, 28, 60f, 59, 6, 50, 40, 20, 7);
            config[33] = new ShopConfig(33, 28, 60f, 60, 6, 50, 40, 20, 7);
            config[34] = new ShopConfig(34, 29, 60f, 62, 6, 50, 40, 20, 7);
            config[35] = new ShopConfig(35, 30, 60f, 63, 6, 50, 40, 20, 7);
            config[36] = new ShopConfig(36, 30, 60f, 64, 6, 50, 40, 20, 7);
            config[37] = new ShopConfig(37, 31, 60f, 66, 6, 50, 40, 20, 7);
            config[38] = new ShopConfig(38, 32, 60f, 67, 6, 50, 40, 20, 7);
            config[39] = new ShopConfig(39, 32, 60f, 68, 6, 50, 40, 20, 7);
            config[40] = new ShopConfig(40, 33, 60f, 70, 6, 50, 40, 20, 7);
            config[41] = new ShopConfig(41, 34, 60f, 71, 6, 50, 40, 20, 8);
            config[42] = new ShopConfig(42, 34, 60f, 73, 6, 50, 40, 20, 8);
            config[43] = new ShopConfig(43, 35, 60f, 74, 6, 50, 40, 20, 8);
            config[44] = new ShopConfig(44, 36, 60f, 76, 6, 50, 40, 20, 8);
            config[45] = new ShopConfig(45, 36, 60f, 77, 6, 50, 40, 20, 8);
            config[46] = new ShopConfig(46, 37, 60f, 78, 6, 50, 40, 20, 8);
            config[47] = new ShopConfig(47, 38, 60f, 80, 6, 50, 40, 20, 8);
            config[48] = new ShopConfig(48, 38, 60f, 81, 6, 50, 40, 20, 8);
            config[49] = new ShopConfig(49, 39, 60f, 82, 6, 50, 40, 20, 8);
            config[50] = new ShopConfig(50, 40, 60f, 84, 6, 50, 40, 20, 8);
            config[51] = new ShopConfig(51, 40, 60f, 85, 6, 50, 40, 20, 8);
            config[52] = new ShopConfig(52, 41, 60f, 87, 6, 50, 40, 20, 8);
            config[53] = new ShopConfig(53, 42, 60f, 88, 6, 50, 40, 20, 8);
            config[54] = new ShopConfig(54, 42, 60f, 90, 6, 50, 40, 20, 8);
            config[55] = new ShopConfig(55, 43, 60f, 91, 6, 50, 40, 20, 8);
            config[56] = new ShopConfig(56, 44, 60f, 92, 6, 50, 40, 20, 8);
            config[57] = new ShopConfig(57, 44, 60f, 94, 6, 50, 40, 20, 8);
            config[58] = new ShopConfig(58, 45, 60f, 95, 6, 50, 40, 20, 8);
            config[59] = new ShopConfig(59, 46, 60f, 96, 6, 50, 40, 20, 8);
            config[60] = new ShopConfig(60, 46, 60f, 98, 6, 50, 40, 20, 8);
            config[61] = new ShopConfig(61, 47, 60f, 99, 6, 50, 40, 20, 9);
            config[62] = new ShopConfig(62, 48, 60f, 101, 6, 50, 40, 20, 9);
            config[63] = new ShopConfig(63, 48, 60f, 102, 6, 50, 40, 20, 9);
            config[64] = new ShopConfig(64, 49, 60f, 104, 6, 50, 40, 20, 9);
            config[65] = new ShopConfig(65, 50, 60f, 105, 6, 50, 40, 20, 9);
            config[66] = new ShopConfig(66, 50, 60f, 106, 6, 50, 40, 20, 9);
            config[67] = new ShopConfig(67, 51, 60f, 108, 6, 50, 40, 20, 9);
            config[68] = new ShopConfig(68, 52, 60f, 109, 6, 50, 40, 20, 9);
            config[69] = new ShopConfig(69, 52, 60f, 110, 6, 50, 40, 20, 9);
            config[70] = new ShopConfig(70, 53, 60f, 112, 6, 50, 40, 20, 9);
            config[71] = new ShopConfig(71, 54, 60f, 113, 6, 50, 40, 20, 9);
            config[72] = new ShopConfig(72, 54, 60f, 115, 6, 50, 40, 20, 9);
            config[73] = new ShopConfig(73, 55, 60f, 116, 6, 50, 40, 20, 9);
            config[74] = new ShopConfig(74, 56, 60f, 118, 6, 50, 40, 20, 9);
            config[75] = new ShopConfig(75, 56, 60f, 119, 6, 50, 40, 20, 9);
            config[76] = new ShopConfig(76, 57, 60f, 120, 6, 50, 40, 20, 9);
            config[77] = new ShopConfig(77, 58, 60f, 122, 6, 50, 40, 20, 9);
            config[78] = new ShopConfig(78, 58, 60f, 123, 6, 50, 40, 20, 9);
            config[79] = new ShopConfig(79, 59, 60f, 124, 6, 50, 40, 20, 9);
            config[80] = new ShopConfig(80, 60, 60f, 126, 6, 50, 40, 20, 9);
            config[81] = new ShopConfig(81, 60, 60f, 127, 6, 50, 40, 20, 10);
            config[82] = new ShopConfig(82, 61, 60f, 129, 6, 50, 40, 20, 10);
            config[83] = new ShopConfig(83, 62, 60f, 130, 6, 50, 40, 20, 10);
            config[84] = new ShopConfig(84, 62, 60f, 132, 6, 50, 40, 20, 10);
            config[85] = new ShopConfig(85, 63, 60f, 133, 6, 50, 40, 20, 10);
            config[86] = new ShopConfig(86, 64, 60f, 134, 6, 50, 40, 20, 10);
            config[87] = new ShopConfig(87, 64, 60f, 136, 6, 50, 40, 20, 10);
            config[88] = new ShopConfig(88, 65, 60f, 137, 6, 50, 40, 20, 10);
            config[89] = new ShopConfig(89, 66, 60f, 138, 6, 50, 40, 20, 10);
            config[90] = new ShopConfig(90, 66, 60f, 140, 6, 50, 40, 20, 10);
            config[91] = new ShopConfig(91, 67, 60f, 141, 6, 50, 40, 20, 10);
            config[92] = new ShopConfig(92, 68, 60f, 143, 6, 50, 40, 20, 10);
            config[93] = new ShopConfig(93, 68, 60f, 144, 6, 50, 40, 20, 10);
            config[94] = new ShopConfig(94, 69, 60f, 146, 6, 50, 40, 20, 10);
            config[95] = new ShopConfig(95, 70, 60f, 147, 6, 50, 40, 20, 10);
            config[96] = new ShopConfig(96, 70, 60f, 148, 6, 50, 40, 20, 10);
            config[97] = new ShopConfig(97, 71, 60f, 150, 6, 50, 40, 20, 10);
            config[98] = new ShopConfig(98, 72, 60f, 151, 6, 50, 40, 20, 10);
            config[99] = new ShopConfig(99, 72, 60f, 152, 6, 50, 40, 20, 10);
            config[100] = new ShopConfig(100, 73, 60f, 154, 6, 50, 40, 20, 10);

            RebuildIndex();

        }

        private static void RebuildIndex()
        {
            foreach (var kv in config)
            {
            }
        }

        public static ShopConfig GetConfig(int id)
        {
            ShopConfig data;
            if (config.TryGetValue(id, out data))
            {
                return data;
            }
            throw new NullReferenceException(string.Format("配置表ShopConfig不存在id={0}", id));
        }


        public static bool HasConfig(int id)
        {
            if (config.ContainsKey(id))
            {
                return true;
            }
            return false;
        }

        public static void Assign(int id, ShopConfig configData)
        {
            config[id] = configData; 
        }

        public static void Add(int id, ShopConfig configData)
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
