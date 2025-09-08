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
            config[1] = new ShopConfig(1, 60, 0f, 0, 1);
            config[2] = new ShopConfig(2, 65, 0f, 0, 1);
            config[3] = new ShopConfig(3, 70, 5f, 42, 1);
            config[4] = new ShopConfig(4, 75, 10f, 45, 2);
            config[5] = new ShopConfig(5, 80, 15f, 48, 2);
            config[6] = new ShopConfig(6, 85, 20f, 51, 2);
            config[7] = new ShopConfig(7, 90, 25f, 54, 3);
            config[8] = new ShopConfig(8, 95, 30f, 57, 3);
            config[9] = new ShopConfig(9, 100, 35f, 60, 3);
            config[10] = new ShopConfig(10, 105, 40f, 63, 4);
            config[11] = new ShopConfig(11, 110, 45f, 66, 4);
            config[12] = new ShopConfig(12, 115, 50f, 69, 4);
            config[13] = new ShopConfig(13, 120, 55f, 72, 4);
            config[14] = new ShopConfig(14, 125, 60f, 75, 4);
            config[15] = new ShopConfig(15, 130, 60f, 78, 4);
            config[16] = new ShopConfig(16, 135, 60f, 81, 4);
            config[17] = new ShopConfig(17, 140, 60f, 84, 4);
            config[18] = new ShopConfig(18, 145, 60f, 87, 4);
            config[19] = new ShopConfig(19, 150, 60f, 90, 4);
            config[20] = new ShopConfig(20, 155, 60f, 93, 4);
            config[21] = new ShopConfig(21, 160, 60f, 96, 4);
            config[22] = new ShopConfig(22, 165, 60f, 99, 4);
            config[23] = new ShopConfig(23, 170, 60f, 102, 4);
            config[24] = new ShopConfig(24, 175, 60f, 105, 4);
            config[25] = new ShopConfig(25, 180, 60f, 108, 4);
            config[26] = new ShopConfig(26, 185, 60f, 111, 4);
            config[27] = new ShopConfig(27, 190, 60f, 114, 4);
            config[28] = new ShopConfig(28, 195, 60f, 117, 4);
            config[29] = new ShopConfig(29, 200, 60f, 120, 4);
            config[30] = new ShopConfig(30, 205, 60f, 123, 4);
            config[31] = new ShopConfig(31, 210, 60f, 126, 4);
            config[32] = new ShopConfig(32, 215, 60f, 129, 4);
            config[33] = new ShopConfig(33, 220, 60f, 132, 4);
            config[34] = new ShopConfig(34, 225, 60f, 135, 4);
            config[35] = new ShopConfig(35, 230, 60f, 138, 4);
            config[36] = new ShopConfig(36, 235, 60f, 141, 4);
            config[37] = new ShopConfig(37, 240, 60f, 144, 4);
            config[38] = new ShopConfig(38, 245, 60f, 147, 4);
            config[39] = new ShopConfig(39, 250, 60f, 150, 4);
            config[40] = new ShopConfig(40, 255, 60f, 153, 4);
            config[41] = new ShopConfig(41, 260, 60f, 156, 4);
            config[42] = new ShopConfig(42, 265, 60f, 159, 4);
            config[43] = new ShopConfig(43, 270, 60f, 162, 4);
            config[44] = new ShopConfig(44, 275, 60f, 165, 4);
            config[45] = new ShopConfig(45, 280, 60f, 168, 4);
            config[46] = new ShopConfig(46, 285, 60f, 171, 4);
            config[47] = new ShopConfig(47, 290, 60f, 174, 4);
            config[48] = new ShopConfig(48, 295, 60f, 177, 4);
            config[49] = new ShopConfig(49, 300, 60f, 180, 4);
            config[50] = new ShopConfig(50, 305, 60f, 183, 4);
            config[51] = new ShopConfig(51, 310, 60f, 186, 4);
            config[52] = new ShopConfig(52, 315, 60f, 189, 4);
            config[53] = new ShopConfig(53, 320, 60f, 192, 4);
            config[54] = new ShopConfig(54, 325, 60f, 195, 4);
            config[55] = new ShopConfig(55, 330, 60f, 198, 4);
            config[56] = new ShopConfig(56, 335, 60f, 201, 4);
            config[57] = new ShopConfig(57, 340, 60f, 204, 4);
            config[58] = new ShopConfig(58, 345, 60f, 207, 4);
            config[59] = new ShopConfig(59, 350, 60f, 210, 4);
            config[60] = new ShopConfig(60, 355, 60f, 213, 4);
            config[61] = new ShopConfig(61, 360, 60f, 216, 4);
            config[62] = new ShopConfig(62, 365, 60f, 219, 4);
            config[63] = new ShopConfig(63, 370, 60f, 222, 4);
            config[64] = new ShopConfig(64, 375, 60f, 225, 4);
            config[65] = new ShopConfig(65, 380, 60f, 228, 4);
            config[66] = new ShopConfig(66, 385, 60f, 231, 4);
            config[67] = new ShopConfig(67, 390, 60f, 234, 4);
            config[68] = new ShopConfig(68, 395, 60f, 237, 4);
            config[69] = new ShopConfig(69, 400, 60f, 240, 4);
            config[70] = new ShopConfig(70, 405, 60f, 243, 4);
            config[71] = new ShopConfig(71, 410, 60f, 246, 4);
            config[72] = new ShopConfig(72, 415, 60f, 249, 4);
            config[73] = new ShopConfig(73, 420, 60f, 252, 4);
            config[74] = new ShopConfig(74, 425, 60f, 255, 4);
            config[75] = new ShopConfig(75, 430, 60f, 258, 4);
            config[76] = new ShopConfig(76, 435, 60f, 261, 4);
            config[77] = new ShopConfig(77, 440, 60f, 264, 4);
            config[78] = new ShopConfig(78, 445, 60f, 267, 4);
            config[79] = new ShopConfig(79, 450, 60f, 270, 4);
            config[80] = new ShopConfig(80, 455, 60f, 273, 4);
            config[81] = new ShopConfig(81, 460, 60f, 276, 4);
            config[82] = new ShopConfig(82, 465, 60f, 279, 4);
            config[83] = new ShopConfig(83, 470, 60f, 282, 4);
            config[84] = new ShopConfig(84, 475, 60f, 285, 4);
            config[85] = new ShopConfig(85, 480, 60f, 288, 4);
            config[86] = new ShopConfig(86, 485, 60f, 291, 4);
            config[87] = new ShopConfig(87, 490, 60f, 294, 4);
            config[88] = new ShopConfig(88, 495, 60f, 297, 4);
            config[89] = new ShopConfig(89, 500, 60f, 300, 4);
            config[90] = new ShopConfig(90, 505, 60f, 303, 4);
            config[91] = new ShopConfig(91, 510, 60f, 306, 4);
            config[92] = new ShopConfig(92, 515, 60f, 309, 4);
            config[93] = new ShopConfig(93, 520, 60f, 312, 4);
            config[94] = new ShopConfig(94, 525, 60f, 315, 4);
            config[95] = new ShopConfig(95, 530, 60f, 318, 4);
            config[96] = new ShopConfig(96, 535, 60f, 321, 4);
            config[97] = new ShopConfig(97, 540, 60f, 324, 4);
            config[98] = new ShopConfig(98, 545, 60f, 327, 4);
            config[99] = new ShopConfig(99, 550, 60f, 330, 4);
            config[100] = new ShopConfig(100, 555, 60f, 333, 4);

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
