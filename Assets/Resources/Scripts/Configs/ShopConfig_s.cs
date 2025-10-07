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
            config[1] = new ShopConfig(1, 70, 0f, 0, 0, 0);
            config[2] = new ShopConfig(2, 60, 0f, 0, 0, 0);
            config[3] = new ShopConfig(3, 65, 15f, 45, 0, 0);
            config[4] = new ShopConfig(4, 70, 18f, 49, 1, 0);
            config[5] = new ShopConfig(5, 75, 21f, 52, 1, 0);
            config[6] = new ShopConfig(6, 80, 24f, 56, 1, 5);
            config[7] = new ShopConfig(7, 85, 27f, 59, 2, 5);
            config[8] = new ShopConfig(8, 90, 30f, 63, 2, 5);
            config[9] = new ShopConfig(9, 95, 33f, 66, 2, 5);
            config[10] = new ShopConfig(10, 100, 36f, 70, 2, 5);
            config[11] = new ShopConfig(11, 105, 39f, 73, 3, 10);
            config[12] = new ShopConfig(12, 110, 42f, 77, 3, 10);
            config[13] = new ShopConfig(13, 115, 45f, 80, 3, 10);
            config[14] = new ShopConfig(14, 120, 48f, 84, 3, 10);
            config[15] = new ShopConfig(15, 125, 51f, 87, 4, 10);
            config[16] = new ShopConfig(16, 130, 54f, 91, 4, 20);
            config[17] = new ShopConfig(17, 135, 57f, 94, 4, 20);
            config[18] = new ShopConfig(18, 140, 60f, 98, 4, 20);
            config[19] = new ShopConfig(19, 145, 60f, 101, 5, 20);
            config[20] = new ShopConfig(20, 150, 60f, 105, 5, 20);
            config[21] = new ShopConfig(21, 155, 60f, 108, 5, 50);
            config[22] = new ShopConfig(22, 160, 60f, 112, 5, 50);
            config[23] = new ShopConfig(23, 165, 60f, 115, 5, 50);
            config[24] = new ShopConfig(24, 170, 60f, 119, 5, 50);
            config[25] = new ShopConfig(25, 175, 60f, 122, 6, 50);
            config[26] = new ShopConfig(26, 180, 60f, 126, 6, 50);
            config[27] = new ShopConfig(27, 185, 60f, 129, 6, 50);
            config[28] = new ShopConfig(28, 190, 60f, 133, 6, 50);
            config[29] = new ShopConfig(29, 195, 60f, 136, 6, 50);
            config[30] = new ShopConfig(30, 200, 60f, 140, 6, 50);
            config[31] = new ShopConfig(31, 205, 60f, 143, 6, 50);
            config[32] = new ShopConfig(32, 210, 60f, 147, 6, 50);
            config[33] = new ShopConfig(33, 215, 60f, 150, 6, 50);
            config[34] = new ShopConfig(34, 220, 60f, 154, 6, 50);
            config[35] = new ShopConfig(35, 225, 60f, 157, 6, 50);
            config[36] = new ShopConfig(36, 230, 60f, 161, 6, 50);
            config[37] = new ShopConfig(37, 235, 60f, 164, 6, 50);
            config[38] = new ShopConfig(38, 240, 60f, 168, 6, 50);
            config[39] = new ShopConfig(39, 245, 60f, 171, 6, 50);
            config[40] = new ShopConfig(40, 250, 60f, 175, 6, 50);
            config[41] = new ShopConfig(41, 255, 60f, 178, 6, 50);
            config[42] = new ShopConfig(42, 260, 60f, 182, 6, 50);
            config[43] = new ShopConfig(43, 265, 60f, 185, 6, 50);
            config[44] = new ShopConfig(44, 270, 60f, 189, 6, 50);
            config[45] = new ShopConfig(45, 275, 60f, 192, 6, 50);
            config[46] = new ShopConfig(46, 280, 60f, 196, 6, 50);
            config[47] = new ShopConfig(47, 285, 60f, 199, 6, 50);
            config[48] = new ShopConfig(48, 290, 60f, 203, 6, 50);
            config[49] = new ShopConfig(49, 295, 60f, 206, 6, 50);
            config[50] = new ShopConfig(50, 300, 60f, 210, 6, 50);
            config[51] = new ShopConfig(51, 305, 60f, 213, 6, 50);
            config[52] = new ShopConfig(52, 310, 60f, 217, 6, 50);
            config[53] = new ShopConfig(53, 315, 60f, 220, 6, 50);
            config[54] = new ShopConfig(54, 320, 60f, 224, 6, 50);
            config[55] = new ShopConfig(55, 325, 60f, 227, 6, 50);
            config[56] = new ShopConfig(56, 330, 60f, 231, 6, 50);
            config[57] = new ShopConfig(57, 335, 60f, 234, 6, 50);
            config[58] = new ShopConfig(58, 340, 60f, 238, 6, 50);
            config[59] = new ShopConfig(59, 345, 60f, 241, 6, 50);
            config[60] = new ShopConfig(60, 350, 60f, 245, 6, 50);
            config[61] = new ShopConfig(61, 355, 60f, 248, 6, 50);
            config[62] = new ShopConfig(62, 360, 60f, 252, 6, 50);
            config[63] = new ShopConfig(63, 365, 60f, 255, 6, 50);
            config[64] = new ShopConfig(64, 370, 60f, 259, 6, 50);
            config[65] = new ShopConfig(65, 375, 60f, 262, 6, 50);
            config[66] = new ShopConfig(66, 380, 60f, 266, 6, 50);
            config[67] = new ShopConfig(67, 385, 60f, 269, 6, 50);
            config[68] = new ShopConfig(68, 390, 60f, 273, 6, 50);
            config[69] = new ShopConfig(69, 395, 60f, 276, 6, 50);
            config[70] = new ShopConfig(70, 400, 60f, 280, 6, 50);
            config[71] = new ShopConfig(71, 405, 60f, 283, 6, 50);
            config[72] = new ShopConfig(72, 410, 60f, 287, 6, 50);
            config[73] = new ShopConfig(73, 415, 60f, 290, 6, 50);
            config[74] = new ShopConfig(74, 420, 60f, 294, 6, 50);
            config[75] = new ShopConfig(75, 425, 60f, 297, 6, 50);
            config[76] = new ShopConfig(76, 430, 60f, 301, 6, 50);
            config[77] = new ShopConfig(77, 435, 60f, 304, 6, 50);
            config[78] = new ShopConfig(78, 440, 60f, 308, 6, 50);
            config[79] = new ShopConfig(79, 445, 60f, 311, 6, 50);
            config[80] = new ShopConfig(80, 450, 60f, 315, 6, 50);
            config[81] = new ShopConfig(81, 455, 60f, 318, 6, 50);
            config[82] = new ShopConfig(82, 460, 60f, 322, 6, 50);
            config[83] = new ShopConfig(83, 465, 60f, 325, 6, 50);
            config[84] = new ShopConfig(84, 470, 60f, 329, 6, 50);
            config[85] = new ShopConfig(85, 475, 60f, 332, 6, 50);
            config[86] = new ShopConfig(86, 480, 60f, 336, 6, 50);
            config[87] = new ShopConfig(87, 485, 60f, 339, 6, 50);
            config[88] = new ShopConfig(88, 490, 60f, 343, 6, 50);
            config[89] = new ShopConfig(89, 495, 60f, 346, 6, 50);
            config[90] = new ShopConfig(90, 500, 60f, 350, 6, 50);
            config[91] = new ShopConfig(91, 505, 60f, 353, 6, 50);
            config[92] = new ShopConfig(92, 510, 60f, 357, 6, 50);
            config[93] = new ShopConfig(93, 515, 60f, 360, 6, 50);
            config[94] = new ShopConfig(94, 520, 60f, 364, 6, 50);
            config[95] = new ShopConfig(95, 525, 60f, 367, 6, 50);
            config[96] = new ShopConfig(96, 530, 60f, 371, 6, 50);
            config[97] = new ShopConfig(97, 535, 60f, 374, 6, 50);
            config[98] = new ShopConfig(98, 540, 60f, 378, 6, 50);
            config[99] = new ShopConfig(99, 545, 60f, 381, 6, 50);
            config[100] = new ShopConfig(100, 550, 60f, 385, 6, 50);

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
