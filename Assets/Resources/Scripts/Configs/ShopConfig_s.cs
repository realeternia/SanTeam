using System;
using System.Collections;
using System.Collections.Generic;

namespace CommonConfig
{
    public class ShopConfig
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
            config[1] = new ShopConfig(1, 2, 0f, 2, 0, 0, 20, 0, 0);
            config[2] = new ShopConfig(2, 3, 0f, 3, 0, 0, 20, 0, 0);
            config[3] = new ShopConfig(3, 4, 0f, 4, 0, 0, 20, 5, 0);
            config[4] = new ShopConfig(4, 5, 0f, 5, 1, 0, 20, 5, 0);
            config[5] = new ShopConfig(5, 6, 0f, 6, 1, 0, 20, 5, 0);
            config[6] = new ShopConfig(6, 7, 0f, 7, 1, 5, 25, 7, 0);
            config[7] = new ShopConfig(7, 8, 0f, 8, 2, 5, 25, 7, 0);
            config[8] = new ShopConfig(8, 9, 0f, 9, 2, 5, 25, 7, 0);
            config[9] = new ShopConfig(9, 10, 0f, 10, 2, 5, 30, 10, 3);
            config[10] = new ShopConfig(10, 11, 5f, 11, 2, 5, 30, 10, 3);
            config[11] = new ShopConfig(11, 12, 5f, 12, 3, 10, 30, 10, 3);
            config[12] = new ShopConfig(12, 13, 5f, 13, 3, 10, 30, 10, 3);
            config[13] = new ShopConfig(13, 14, 5f, 14, 3, 10, 35, 15, 5);
            config[14] = new ShopConfig(14, 15, 5f, 15, 3, 10, 35, 15, 5);
            config[15] = new ShopConfig(15, 16, 5f, 16, 4, 10, 35, 15, 5);
            config[16] = new ShopConfig(16, 17, 5f, 17, 4, 20, 35, 15, 5);
            config[17] = new ShopConfig(17, 18, 5f, 18, 4, 20, 35, 15, 5);
            config[18] = new ShopConfig(18, 19, 5f, 19, 4, 20, 40, 20, 6);
            config[19] = new ShopConfig(19, 20, 5f, 20, 5, 20, 40, 20, 6);
            config[20] = new ShopConfig(20, 21, 10f, 21, 5, 20, 40, 20, 6);
            config[21] = new ShopConfig(21, 22, 10f, 22, 5, 50, 40, 20, 6);
            config[22] = new ShopConfig(22, 23, 10f, 23, 5, 50, 40, 20, 7);
            config[23] = new ShopConfig(23, 24, 10f, 24, 5, 50, 40, 20, 7);
            config[24] = new ShopConfig(24, 25, 10f, 25, 5, 50, 40, 20, 7);
            config[25] = new ShopConfig(25, 26, 10f, 26, 6, 50, 40, 20, 7);
            config[26] = new ShopConfig(26, 27, 10f, 27, 6, 50, 40, 20, 7);
            config[27] = new ShopConfig(27, 28, 10f, 28, 6, 50, 40, 20, 7);
            config[28] = new ShopConfig(28, 29, 10f, 29, 6, 50, 40, 20, 7);
            config[29] = new ShopConfig(29, 30, 10f, 30, 6, 50, 40, 20, 7);
            config[30] = new ShopConfig(30, 31, 15f, 31, 6, 50, 40, 20, 7);
            config[31] = new ShopConfig(31, 32, 15f, 32, 6, 50, 40, 20, 7);
            config[32] = new ShopConfig(32, 33, 15f, 33, 6, 50, 40, 20, 7);
            config[33] = new ShopConfig(33, 34, 15f, 34, 6, 50, 40, 20, 7);
            config[34] = new ShopConfig(34, 35, 15f, 35, 6, 50, 40, 20, 7);
            config[35] = new ShopConfig(35, 36, 15f, 36, 6, 50, 40, 20, 7);
            config[36] = new ShopConfig(36, 37, 15f, 37, 6, 50, 40, 20, 7);
            config[37] = new ShopConfig(37, 38, 15f, 38, 6, 50, 40, 20, 7);
            config[38] = new ShopConfig(38, 39, 15f, 39, 6, 50, 40, 20, 7);
            config[39] = new ShopConfig(39, 40, 15f, 40, 6, 50, 40, 20, 7);
            config[40] = new ShopConfig(40, 41, 20f, 41, 6, 50, 40, 20, 7);
            config[41] = new ShopConfig(41, 42, 20f, 42, 6, 50, 40, 20, 8);
            config[42] = new ShopConfig(42, 43, 20f, 43, 6, 50, 40, 20, 8);
            config[43] = new ShopConfig(43, 44, 20f, 44, 6, 50, 40, 20, 8);
            config[44] = new ShopConfig(44, 45, 20f, 45, 6, 50, 40, 20, 8);
            config[45] = new ShopConfig(45, 46, 20f, 46, 6, 50, 40, 20, 8);
            config[46] = new ShopConfig(46, 47, 20f, 47, 6, 50, 40, 20, 8);
            config[47] = new ShopConfig(47, 48, 20f, 48, 6, 50, 40, 20, 8);
            config[48] = new ShopConfig(48, 49, 20f, 49, 6, 50, 40, 20, 8);
            config[49] = new ShopConfig(49, 50, 20f, 50, 6, 50, 40, 20, 8);
            config[50] = new ShopConfig(50, 51, 25f, 51, 6, 50, 40, 20, 8);
            config[51] = new ShopConfig(51, 52, 25f, 52, 6, 50, 40, 20, 8);
            config[52] = new ShopConfig(52, 53, 25f, 53, 6, 50, 40, 20, 8);
            config[53] = new ShopConfig(53, 54, 25f, 54, 6, 50, 40, 20, 8);
            config[54] = new ShopConfig(54, 55, 25f, 55, 6, 50, 40, 20, 8);
            config[55] = new ShopConfig(55, 56, 25f, 56, 6, 50, 40, 20, 8);
            config[56] = new ShopConfig(56, 57, 25f, 57, 6, 50, 40, 20, 8);
            config[57] = new ShopConfig(57, 58, 25f, 58, 6, 50, 40, 20, 8);
            config[58] = new ShopConfig(58, 59, 25f, 59, 6, 50, 40, 20, 8);
            config[59] = new ShopConfig(59, 60, 25f, 60, 6, 50, 40, 20, 8);
            config[60] = new ShopConfig(60, 61, 30f, 61, 6, 50, 40, 20, 8);
            config[61] = new ShopConfig(61, 62, 30f, 62, 6, 50, 40, 20, 9);
            config[62] = new ShopConfig(62, 63, 30f, 63, 6, 50, 40, 20, 9);
            config[63] = new ShopConfig(63, 64, 30f, 64, 6, 50, 40, 20, 9);
            config[64] = new ShopConfig(64, 65, 30f, 65, 6, 50, 40, 20, 9);
            config[65] = new ShopConfig(65, 66, 30f, 66, 6, 50, 40, 20, 9);
            config[66] = new ShopConfig(66, 67, 30f, 67, 6, 50, 40, 20, 9);
            config[67] = new ShopConfig(67, 68, 30f, 68, 6, 50, 40, 20, 9);
            config[68] = new ShopConfig(68, 69, 30f, 69, 6, 50, 40, 20, 9);
            config[69] = new ShopConfig(69, 70, 30f, 70, 6, 50, 40, 20, 9);
            config[70] = new ShopConfig(70, 71, 35f, 71, 6, 50, 40, 20, 9);
            config[71] = new ShopConfig(71, 72, 35f, 72, 6, 50, 40, 20, 9);
            config[72] = new ShopConfig(72, 73, 35f, 73, 6, 50, 40, 20, 9);
            config[73] = new ShopConfig(73, 74, 35f, 74, 6, 50, 40, 20, 9);
            config[74] = new ShopConfig(74, 75, 35f, 75, 6, 50, 40, 20, 9);
            config[75] = new ShopConfig(75, 76, 35f, 76, 6, 50, 40, 20, 9);
            config[76] = new ShopConfig(76, 77, 35f, 77, 6, 50, 40, 20, 9);
            config[77] = new ShopConfig(77, 78, 35f, 78, 6, 50, 40, 20, 9);
            config[78] = new ShopConfig(78, 79, 35f, 79, 6, 50, 40, 20, 9);
            config[79] = new ShopConfig(79, 80, 35f, 80, 6, 50, 40, 20, 9);
            config[80] = new ShopConfig(80, 81, 40f, 81, 6, 50, 40, 20, 9);
            config[81] = new ShopConfig(81, 82, 40f, 82, 6, 50, 40, 20, 10);
            config[82] = new ShopConfig(82, 83, 40f, 83, 6, 50, 40, 20, 10);
            config[83] = new ShopConfig(83, 84, 40f, 84, 6, 50, 40, 20, 10);
            config[84] = new ShopConfig(84, 85, 40f, 85, 6, 50, 40, 20, 10);
            config[85] = new ShopConfig(85, 86, 40f, 86, 6, 50, 40, 20, 10);
            config[86] = new ShopConfig(86, 87, 40f, 87, 6, 50, 40, 20, 10);
            config[87] = new ShopConfig(87, 88, 40f, 88, 6, 50, 40, 20, 10);
            config[88] = new ShopConfig(88, 89, 40f, 89, 6, 50, 40, 20, 10);
            config[89] = new ShopConfig(89, 90, 40f, 90, 6, 50, 40, 20, 10);
            config[90] = new ShopConfig(90, 91, 45f, 91, 6, 50, 40, 20, 10);
            config[91] = new ShopConfig(91, 92, 45f, 92, 6, 50, 40, 20, 10);
            config[92] = new ShopConfig(92, 93, 45f, 93, 6, 50, 40, 20, 10);
            config[93] = new ShopConfig(93, 94, 45f, 94, 6, 50, 40, 20, 10);
            config[94] = new ShopConfig(94, 95, 45f, 95, 6, 50, 40, 20, 10);
            config[95] = new ShopConfig(95, 96, 45f, 96, 6, 50, 40, 20, 10);
            config[96] = new ShopConfig(96, 97, 45f, 97, 6, 50, 40, 20, 10);
            config[97] = new ShopConfig(97, 98, 45f, 98, 6, 50, 40, 20, 10);
            config[98] = new ShopConfig(98, 99, 45f, 99, 6, 50, 40, 20, 10);
            config[99] = new ShopConfig(99, 100, 45f, 100, 6, 50, 40, 20, 10);
            config[100] = new ShopConfig(100, 101, 50f, 101, 6, 50, 40, 20, 10);

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
