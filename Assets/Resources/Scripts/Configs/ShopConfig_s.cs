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


        public ShopConfig(int Id, int RoundGold, float MultiCardRate, int MultiPriceTotal, int ItemCount)
        {
            this.Id = Id;
            this.RoundGold = RoundGold;
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
            config[1] = new ShopConfig(1, 50, 0f, 0, 1);
            config[2] = new ShopConfig(2, 55, 0f, 0, 1);
            config[3] = new ShopConfig(3, 60, 5f, 36, 1);
            config[4] = new ShopConfig(4, 65, 10f, 39, 2);
            config[5] = new ShopConfig(5, 70, 15f, 42, 2);
            config[6] = new ShopConfig(6, 75, 20f, 45, 2);
            config[7] = new ShopConfig(7, 80, 25f, 48, 3);
            config[8] = new ShopConfig(8, 85, 30f, 51, 3);
            config[9] = new ShopConfig(9, 90, 35f, 54, 3);
            config[10] = new ShopConfig(10, 95, 40f, 57, 4);
            config[11] = new ShopConfig(11, 100, 45f, 60, 4);
            config[12] = new ShopConfig(12, 105, 50f, 63, 4);
            config[13] = new ShopConfig(13, 110, 55f, 66, 4);
            config[14] = new ShopConfig(14, 115, 60f, 69, 4);
            config[15] = new ShopConfig(15, 120, 65f, 72, 4);
            config[16] = new ShopConfig(16, 125, 70f, 75, 4);
            config[17] = new ShopConfig(17, 130, 75f, 78, 4);
            config[18] = new ShopConfig(18, 135, 75f, 81, 4);
            config[19] = new ShopConfig(19, 140, 75f, 84, 4);
            config[20] = new ShopConfig(20, 145, 75f, 87, 4);
            config[21] = new ShopConfig(21, 150, 75f, 90, 4);
            config[22] = new ShopConfig(22, 155, 75f, 93, 4);
            config[23] = new ShopConfig(23, 160, 75f, 96, 4);
            config[24] = new ShopConfig(24, 165, 75f, 99, 4);
            config[25] = new ShopConfig(25, 170, 75f, 102, 4);
            config[26] = new ShopConfig(26, 175, 75f, 105, 4);
            config[27] = new ShopConfig(27, 180, 75f, 108, 4);
            config[28] = new ShopConfig(28, 185, 75f, 111, 4);
            config[29] = new ShopConfig(29, 190, 75f, 114, 4);
            config[30] = new ShopConfig(30, 195, 75f, 117, 4);
            config[31] = new ShopConfig(31, 200, 75f, 120, 4);
            config[32] = new ShopConfig(32, 205, 75f, 123, 4);
            config[33] = new ShopConfig(33, 210, 75f, 126, 4);
            config[34] = new ShopConfig(34, 215, 75f, 129, 4);
            config[35] = new ShopConfig(35, 220, 75f, 132, 4);
            config[36] = new ShopConfig(36, 225, 75f, 135, 4);
            config[37] = new ShopConfig(37, 230, 75f, 138, 4);
            config[38] = new ShopConfig(38, 235, 75f, 141, 4);
            config[39] = new ShopConfig(39, 240, 75f, 144, 4);
            config[40] = new ShopConfig(40, 245, 75f, 147, 4);
            config[41] = new ShopConfig(41, 250, 75f, 150, 4);
            config[42] = new ShopConfig(42, 255, 75f, 153, 4);
            config[43] = new ShopConfig(43, 260, 75f, 156, 4);
            config[44] = new ShopConfig(44, 265, 75f, 159, 4);
            config[45] = new ShopConfig(45, 270, 75f, 162, 4);
            config[46] = new ShopConfig(46, 275, 75f, 165, 4);
            config[47] = new ShopConfig(47, 280, 75f, 168, 4);
            config[48] = new ShopConfig(48, 285, 75f, 171, 4);
            config[49] = new ShopConfig(49, 290, 75f, 174, 4);
            config[50] = new ShopConfig(50, 295, 75f, 177, 4);
            config[51] = new ShopConfig(51, 300, 75f, 180, 4);
            config[52] = new ShopConfig(52, 305, 75f, 183, 4);
            config[53] = new ShopConfig(53, 310, 75f, 186, 4);
            config[54] = new ShopConfig(54, 315, 75f, 189, 4);
            config[55] = new ShopConfig(55, 320, 75f, 192, 4);
            config[56] = new ShopConfig(56, 325, 75f, 195, 4);
            config[57] = new ShopConfig(57, 330, 75f, 198, 4);
            config[58] = new ShopConfig(58, 335, 75f, 201, 4);
            config[59] = new ShopConfig(59, 340, 75f, 204, 4);
            config[60] = new ShopConfig(60, 345, 75f, 207, 4);
            config[61] = new ShopConfig(61, 350, 75f, 210, 4);
            config[62] = new ShopConfig(62, 355, 75f, 213, 4);
            config[63] = new ShopConfig(63, 360, 75f, 216, 4);
            config[64] = new ShopConfig(64, 365, 75f, 219, 4);
            config[65] = new ShopConfig(65, 370, 75f, 222, 4);
            config[66] = new ShopConfig(66, 375, 75f, 225, 4);
            config[67] = new ShopConfig(67, 380, 75f, 228, 4);
            config[68] = new ShopConfig(68, 385, 75f, 231, 4);
            config[69] = new ShopConfig(69, 390, 75f, 234, 4);
            config[70] = new ShopConfig(70, 395, 75f, 237, 4);
            config[71] = new ShopConfig(71, 400, 75f, 240, 4);
            config[72] = new ShopConfig(72, 405, 75f, 243, 4);
            config[73] = new ShopConfig(73, 410, 75f, 246, 4);
            config[74] = new ShopConfig(74, 415, 75f, 249, 4);
            config[75] = new ShopConfig(75, 420, 75f, 252, 4);
            config[76] = new ShopConfig(76, 425, 75f, 255, 4);
            config[77] = new ShopConfig(77, 430, 75f, 258, 4);
            config[78] = new ShopConfig(78, 435, 75f, 261, 4);
            config[79] = new ShopConfig(79, 440, 75f, 264, 4);
            config[80] = new ShopConfig(80, 445, 75f, 267, 4);
            config[81] = new ShopConfig(81, 450, 75f, 270, 4);
            config[82] = new ShopConfig(82, 455, 75f, 273, 4);
            config[83] = new ShopConfig(83, 460, 75f, 276, 4);
            config[84] = new ShopConfig(84, 465, 75f, 279, 4);
            config[85] = new ShopConfig(85, 470, 75f, 282, 4);
            config[86] = new ShopConfig(86, 475, 75f, 285, 4);
            config[87] = new ShopConfig(87, 480, 75f, 288, 4);
            config[88] = new ShopConfig(88, 485, 75f, 291, 4);
            config[89] = new ShopConfig(89, 490, 75f, 294, 4);
            config[90] = new ShopConfig(90, 495, 75f, 297, 4);
            config[91] = new ShopConfig(91, 500, 75f, 300, 4);
            config[92] = new ShopConfig(92, 505, 75f, 303, 4);
            config[93] = new ShopConfig(93, 510, 75f, 306, 4);
            config[94] = new ShopConfig(94, 515, 75f, 309, 4);
            config[95] = new ShopConfig(95, 520, 75f, 312, 4);
            config[96] = new ShopConfig(96, 525, 75f, 315, 4);
            config[97] = new ShopConfig(97, 530, 75f, 318, 4);
            config[98] = new ShopConfig(98, 535, 75f, 321, 4);
            config[99] = new ShopConfig(99, 540, 75f, 324, 4);
            config[100] = new ShopConfig(100, 545, 75f, 327, 4);

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
