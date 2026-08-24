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


        public ShopConfig(int Id, int RoundGold, float MultiCardRate, int MultiPriceTotal, int ItemCount, int ItemAmazingCount)
        {
            this.Id = Id;
            this.RoundGold = RoundGold;
            this.MultiCardRate = MultiCardRate;
            this.MultiPriceTotal = MultiPriceTotal;
            this.ItemCount = ItemCount;
            this.ItemAmazingCount = ItemAmazingCount;

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

        public static void Load()
        {
            config.Clear();
            config[1] = new ShopConfig(1, 28, 0f, 0, 0, 0);
            config[2] = new ShopConfig(2, 24, 0f, 0, 0, 0);
            config[3] = new ShopConfig(3, 26, 15f, 18, 0, 0);
            config[4] = new ShopConfig(4, 28, 18f, 20, 1, 0);
            config[5] = new ShopConfig(5, 30, 21f, 21, 1, 0);
            config[6] = new ShopConfig(6, 32, 24f, 22, 1, 5);
            config[7] = new ShopConfig(7, 34, 27f, 24, 2, 5);
            config[8] = new ShopConfig(8, 36, 30f, 25, 2, 5);
            config[9] = new ShopConfig(9, 38, 33f, 26, 2, 5);
            config[10] = new ShopConfig(10, 40, 36f, 28, 2, 5);
            config[11] = new ShopConfig(11, 42, 39f, 29, 3, 10);
            config[12] = new ShopConfig(12, 44, 42f, 31, 3, 10);
            config[13] = new ShopConfig(13, 46, 45f, 32, 3, 10);
            config[14] = new ShopConfig(14, 48, 48f, 34, 3, 10);
            config[15] = new ShopConfig(15, 50, 51f, 35, 4, 10);
            config[16] = new ShopConfig(16, 52, 54f, 36, 4, 20);
            config[17] = new ShopConfig(17, 54, 57f, 38, 4, 20);
            config[18] = new ShopConfig(18, 56, 60f, 39, 4, 20);
            config[19] = new ShopConfig(19, 58, 60f, 40, 5, 20);
            config[20] = new ShopConfig(20, 60, 60f, 42, 5, 20);
            config[21] = new ShopConfig(21, 62, 60f, 43, 5, 50);
            config[22] = new ShopConfig(22, 64, 60f, 45, 5, 50);
            config[23] = new ShopConfig(23, 66, 60f, 46, 5, 50);
            config[24] = new ShopConfig(24, 68, 60f, 48, 5, 50);
            config[25] = new ShopConfig(25, 70, 60f, 49, 6, 50);
            config[26] = new ShopConfig(26, 72, 60f, 50, 6, 50);
            config[27] = new ShopConfig(27, 74, 60f, 52, 6, 50);
            config[28] = new ShopConfig(28, 76, 60f, 53, 6, 50);
            config[29] = new ShopConfig(29, 78, 60f, 54, 6, 50);
            config[30] = new ShopConfig(30, 80, 60f, 56, 6, 50);
            config[31] = new ShopConfig(31, 82, 60f, 57, 6, 50);
            config[32] = new ShopConfig(32, 84, 60f, 59, 6, 50);
            config[33] = new ShopConfig(33, 86, 60f, 60, 6, 50);
            config[34] = new ShopConfig(34, 88, 60f, 62, 6, 50);
            config[35] = new ShopConfig(35, 90, 60f, 63, 6, 50);
            config[36] = new ShopConfig(36, 92, 60f, 64, 6, 50);
            config[37] = new ShopConfig(37, 94, 60f, 66, 6, 50);
            config[38] = new ShopConfig(38, 96, 60f, 67, 6, 50);
            config[39] = new ShopConfig(39, 98, 60f, 68, 6, 50);
            config[40] = new ShopConfig(40, 100, 60f, 70, 6, 50);
            config[41] = new ShopConfig(41, 102, 60f, 71, 6, 50);
            config[42] = new ShopConfig(42, 104, 60f, 73, 6, 50);
            config[43] = new ShopConfig(43, 106, 60f, 74, 6, 50);
            config[44] = new ShopConfig(44, 108, 60f, 76, 6, 50);
            config[45] = new ShopConfig(45, 110, 60f, 77, 6, 50);
            config[46] = new ShopConfig(46, 112, 60f, 78, 6, 50);
            config[47] = new ShopConfig(47, 114, 60f, 80, 6, 50);
            config[48] = new ShopConfig(48, 116, 60f, 81, 6, 50);
            config[49] = new ShopConfig(49, 118, 60f, 82, 6, 50);
            config[50] = new ShopConfig(50, 120, 60f, 84, 6, 50);
            config[51] = new ShopConfig(51, 122, 60f, 85, 6, 50);
            config[52] = new ShopConfig(52, 124, 60f, 87, 6, 50);
            config[53] = new ShopConfig(53, 126, 60f, 88, 6, 50);
            config[54] = new ShopConfig(54, 128, 60f, 90, 6, 50);
            config[55] = new ShopConfig(55, 130, 60f, 91, 6, 50);
            config[56] = new ShopConfig(56, 132, 60f, 92, 6, 50);
            config[57] = new ShopConfig(57, 134, 60f, 94, 6, 50);
            config[58] = new ShopConfig(58, 136, 60f, 95, 6, 50);
            config[59] = new ShopConfig(59, 138, 60f, 96, 6, 50);
            config[60] = new ShopConfig(60, 140, 60f, 98, 6, 50);
            config[61] = new ShopConfig(61, 142, 60f, 99, 6, 50);
            config[62] = new ShopConfig(62, 144, 60f, 101, 6, 50);
            config[63] = new ShopConfig(63, 146, 60f, 102, 6, 50);
            config[64] = new ShopConfig(64, 148, 60f, 104, 6, 50);
            config[65] = new ShopConfig(65, 150, 60f, 105, 6, 50);
            config[66] = new ShopConfig(66, 152, 60f, 106, 6, 50);
            config[67] = new ShopConfig(67, 154, 60f, 108, 6, 50);
            config[68] = new ShopConfig(68, 156, 60f, 109, 6, 50);
            config[69] = new ShopConfig(69, 158, 60f, 110, 6, 50);
            config[70] = new ShopConfig(70, 160, 60f, 112, 6, 50);
            config[71] = new ShopConfig(71, 162, 60f, 113, 6, 50);
            config[72] = new ShopConfig(72, 164, 60f, 115, 6, 50);
            config[73] = new ShopConfig(73, 166, 60f, 116, 6, 50);
            config[74] = new ShopConfig(74, 168, 60f, 118, 6, 50);
            config[75] = new ShopConfig(75, 170, 60f, 119, 6, 50);
            config[76] = new ShopConfig(76, 172, 60f, 120, 6, 50);
            config[77] = new ShopConfig(77, 174, 60f, 122, 6, 50);
            config[78] = new ShopConfig(78, 176, 60f, 123, 6, 50);
            config[79] = new ShopConfig(79, 178, 60f, 124, 6, 50);
            config[80] = new ShopConfig(80, 180, 60f, 126, 6, 50);
            config[81] = new ShopConfig(81, 182, 60f, 127, 6, 50);
            config[82] = new ShopConfig(82, 184, 60f, 129, 6, 50);
            config[83] = new ShopConfig(83, 186, 60f, 130, 6, 50);
            config[84] = new ShopConfig(84, 188, 60f, 132, 6, 50);
            config[85] = new ShopConfig(85, 190, 60f, 133, 6, 50);
            config[86] = new ShopConfig(86, 192, 60f, 134, 6, 50);
            config[87] = new ShopConfig(87, 194, 60f, 136, 6, 50);
            config[88] = new ShopConfig(88, 196, 60f, 137, 6, 50);
            config[89] = new ShopConfig(89, 198, 60f, 138, 6, 50);
            config[90] = new ShopConfig(90, 200, 60f, 140, 6, 50);
            config[91] = new ShopConfig(91, 202, 60f, 141, 6, 50);
            config[92] = new ShopConfig(92, 204, 60f, 143, 6, 50);
            config[93] = new ShopConfig(93, 206, 60f, 144, 6, 50);
            config[94] = new ShopConfig(94, 208, 60f, 146, 6, 50);
            config[95] = new ShopConfig(95, 210, 60f, 147, 6, 50);
            config[96] = new ShopConfig(96, 212, 60f, 148, 6, 50);
            config[97] = new ShopConfig(97, 214, 60f, 150, 6, 50);
            config[98] = new ShopConfig(98, 216, 60f, 151, 6, 50);
            config[99] = new ShopConfig(99, 218, 60f, 152, 6, 50);
            config[100] = new ShopConfig(100, 220, 60f, 154, 6, 50);

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
