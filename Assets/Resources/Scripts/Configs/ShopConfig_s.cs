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


        public ShopConfig(int Id, float MultiCardRate, int MultiPriceTotal, int ItemCount)
        {
            this.Id = Id;
            this.MultiCardRate = MultiCardRate;
            this.MultiPriceTotal = MultiPriceTotal;
            this.ItemCount = ItemCount;

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
            config[1] = new ShopConfig(1, 0f, 0, 1);
            config[2] = new ShopConfig(2, 0f, 0, 1);
            config[3] = new ShopConfig(3, 5f, 23, 1);
            config[4] = new ShopConfig(4, 10f, 28, 2);
            config[5] = new ShopConfig(5, 15f, 33, 2);
            config[6] = new ShopConfig(6, 20f, 38, 2);
            config[7] = new ShopConfig(7, 25f, 43, 3);
            config[8] = new ShopConfig(8, 30f, 48, 3);
            config[9] = new ShopConfig(9, 35f, 53, 3);
            config[10] = new ShopConfig(10, 40f, 58, 4);
            config[11] = new ShopConfig(11, 45f, 63, 4);
            config[12] = new ShopConfig(12, 50f, 68, 4);
            config[13] = new ShopConfig(13, 55f, 73, 4);
            config[14] = new ShopConfig(14, 60f, 78, 4);
            config[15] = new ShopConfig(15, 65f, 83, 4);
            config[16] = new ShopConfig(16, 70f, 88, 4);
            config[17] = new ShopConfig(17, 75f, 93, 4);
            config[18] = new ShopConfig(18, 75f, 98, 4);
            config[19] = new ShopConfig(19, 75f, 103, 4);
            config[20] = new ShopConfig(20, 75f, 108, 4);
            config[21] = new ShopConfig(21, 75f, 113, 4);
            config[22] = new ShopConfig(22, 75f, 118, 4);
            config[23] = new ShopConfig(23, 75f, 123, 4);
            config[24] = new ShopConfig(24, 75f, 128, 4);
            config[25] = new ShopConfig(25, 75f, 133, 4);
            config[26] = new ShopConfig(26, 75f, 138, 4);
            config[27] = new ShopConfig(27, 75f, 143, 4);
            config[28] = new ShopConfig(28, 75f, 148, 4);
            config[29] = new ShopConfig(29, 75f, 153, 4);
            config[30] = new ShopConfig(30, 75f, 158, 4);
            config[31] = new ShopConfig(31, 75f, 163, 4);
            config[32] = new ShopConfig(32, 75f, 168, 4);
            config[33] = new ShopConfig(33, 75f, 173, 4);
            config[34] = new ShopConfig(34, 75f, 178, 4);
            config[35] = new ShopConfig(35, 75f, 183, 4);
            config[36] = new ShopConfig(36, 75f, 188, 4);
            config[37] = new ShopConfig(37, 75f, 193, 4);
            config[38] = new ShopConfig(38, 75f, 198, 4);
            config[39] = new ShopConfig(39, 75f, 203, 4);
            config[40] = new ShopConfig(40, 75f, 208, 4);
            config[41] = new ShopConfig(41, 75f, 213, 4);
            config[42] = new ShopConfig(42, 75f, 218, 4);
            config[43] = new ShopConfig(43, 75f, 223, 4);
            config[44] = new ShopConfig(44, 75f, 228, 4);
            config[45] = new ShopConfig(45, 75f, 233, 4);
            config[46] = new ShopConfig(46, 75f, 238, 4);
            config[47] = new ShopConfig(47, 75f, 243, 4);
            config[48] = new ShopConfig(48, 75f, 248, 4);
            config[49] = new ShopConfig(49, 75f, 253, 4);
            config[50] = new ShopConfig(50, 75f, 258, 4);
            config[51] = new ShopConfig(51, 75f, 263, 4);
            config[52] = new ShopConfig(52, 75f, 268, 4);
            config[53] = new ShopConfig(53, 75f, 273, 4);
            config[54] = new ShopConfig(54, 75f, 278, 4);
            config[55] = new ShopConfig(55, 75f, 283, 4);
            config[56] = new ShopConfig(56, 75f, 288, 4);
            config[57] = new ShopConfig(57, 75f, 293, 4);
            config[58] = new ShopConfig(58, 75f, 298, 4);
            config[59] = new ShopConfig(59, 75f, 303, 4);
            config[60] = new ShopConfig(60, 75f, 308, 4);
            config[61] = new ShopConfig(61, 75f, 313, 4);
            config[62] = new ShopConfig(62, 75f, 318, 4);
            config[63] = new ShopConfig(63, 75f, 323, 4);
            config[64] = new ShopConfig(64, 75f, 328, 4);
            config[65] = new ShopConfig(65, 75f, 333, 4);
            config[66] = new ShopConfig(66, 75f, 338, 4);
            config[67] = new ShopConfig(67, 75f, 343, 4);
            config[68] = new ShopConfig(68, 75f, 348, 4);
            config[69] = new ShopConfig(69, 75f, 353, 4);
            config[70] = new ShopConfig(70, 75f, 358, 4);
            config[71] = new ShopConfig(71, 75f, 363, 4);
            config[72] = new ShopConfig(72, 75f, 368, 4);
            config[73] = new ShopConfig(73, 75f, 373, 4);
            config[74] = new ShopConfig(74, 75f, 378, 4);
            config[75] = new ShopConfig(75, 75f, 383, 4);
            config[76] = new ShopConfig(76, 75f, 388, 4);
            config[77] = new ShopConfig(77, 75f, 393, 4);
            config[78] = new ShopConfig(78, 75f, 398, 4);
            config[79] = new ShopConfig(79, 75f, 403, 4);
            config[80] = new ShopConfig(80, 75f, 408, 4);
            config[81] = new ShopConfig(81, 75f, 413, 4);
            config[82] = new ShopConfig(82, 75f, 418, 4);
            config[83] = new ShopConfig(83, 75f, 423, 4);
            config[84] = new ShopConfig(84, 75f, 428, 4);
            config[85] = new ShopConfig(85, 75f, 433, 4);
            config[86] = new ShopConfig(86, 75f, 438, 4);
            config[87] = new ShopConfig(87, 75f, 443, 4);
            config[88] = new ShopConfig(88, 75f, 448, 4);
            config[89] = new ShopConfig(89, 75f, 453, 4);
            config[90] = new ShopConfig(90, 75f, 458, 4);
            config[91] = new ShopConfig(91, 75f, 463, 4);
            config[92] = new ShopConfig(92, 75f, 468, 4);
            config[93] = new ShopConfig(93, 75f, 473, 4);
            config[94] = new ShopConfig(94, 75f, 478, 4);
            config[95] = new ShopConfig(95, 75f, 483, 4);
            config[96] = new ShopConfig(96, 75f, 488, 4);
            config[97] = new ShopConfig(97, 75f, 493, 4);
            config[98] = new ShopConfig(98, 75f, 498, 4);
            config[99] = new ShopConfig(99, 75f, 503, 4);
            config[100] = new ShopConfig(100, 75f, 508, 4);

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
