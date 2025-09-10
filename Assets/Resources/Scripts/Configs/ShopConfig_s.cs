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
            config[1] = new ShopConfig(1, 70, 0f, 0, 1);
            config[2] = new ShopConfig(2, 60, 0f, 0, 1);
            config[3] = new ShopConfig(3, 65, 5f, 26, 1);
            config[4] = new ShopConfig(4, 70, 10f, 28, 2);
            config[5] = new ShopConfig(5, 75, 15f, 30, 2);
            config[6] = new ShopConfig(6, 80, 20f, 32, 2);
            config[7] = new ShopConfig(7, 85, 25f, 34, 3);
            config[8] = new ShopConfig(8, 90, 30f, 36, 3);
            config[9] = new ShopConfig(9, 95, 35f, 38, 3);
            config[10] = new ShopConfig(10, 100, 40f, 40, 4);
            config[11] = new ShopConfig(11, 105, 45f, 42, 4);
            config[12] = new ShopConfig(12, 110, 50f, 44, 4);
            config[13] = new ShopConfig(13, 115, 55f, 46, 4);
            config[14] = new ShopConfig(14, 120, 60f, 48, 4);
            config[15] = new ShopConfig(15, 125, 60f, 50, 4);
            config[16] = new ShopConfig(16, 130, 60f, 52, 4);
            config[17] = new ShopConfig(17, 135, 60f, 54, 4);
            config[18] = new ShopConfig(18, 140, 60f, 56, 4);
            config[19] = new ShopConfig(19, 145, 60f, 58, 4);
            config[20] = new ShopConfig(20, 150, 60f, 60, 4);
            config[21] = new ShopConfig(21, 155, 60f, 62, 4);
            config[22] = new ShopConfig(22, 160, 60f, 64, 4);
            config[23] = new ShopConfig(23, 165, 60f, 66, 4);
            config[24] = new ShopConfig(24, 170, 60f, 68, 4);
            config[25] = new ShopConfig(25, 175, 60f, 70, 4);
            config[26] = new ShopConfig(26, 180, 60f, 72, 4);
            config[27] = new ShopConfig(27, 185, 60f, 74, 4);
            config[28] = new ShopConfig(28, 190, 60f, 76, 4);
            config[29] = new ShopConfig(29, 195, 60f, 78, 4);
            config[30] = new ShopConfig(30, 200, 60f, 80, 4);
            config[31] = new ShopConfig(31, 205, 60f, 82, 4);
            config[32] = new ShopConfig(32, 210, 60f, 84, 4);
            config[33] = new ShopConfig(33, 215, 60f, 86, 4);
            config[34] = new ShopConfig(34, 220, 60f, 88, 4);
            config[35] = new ShopConfig(35, 225, 60f, 90, 4);
            config[36] = new ShopConfig(36, 230, 60f, 92, 4);
            config[37] = new ShopConfig(37, 235, 60f, 94, 4);
            config[38] = new ShopConfig(38, 240, 60f, 96, 4);
            config[39] = new ShopConfig(39, 245, 60f, 98, 4);
            config[40] = new ShopConfig(40, 250, 60f, 100, 4);
            config[41] = new ShopConfig(41, 255, 60f, 102, 4);
            config[42] = new ShopConfig(42, 260, 60f, 104, 4);
            config[43] = new ShopConfig(43, 265, 60f, 106, 4);
            config[44] = new ShopConfig(44, 270, 60f, 108, 4);
            config[45] = new ShopConfig(45, 275, 60f, 110, 4);
            config[46] = new ShopConfig(46, 280, 60f, 112, 4);
            config[47] = new ShopConfig(47, 285, 60f, 114, 4);
            config[48] = new ShopConfig(48, 290, 60f, 116, 4);
            config[49] = new ShopConfig(49, 295, 60f, 118, 4);
            config[50] = new ShopConfig(50, 300, 60f, 120, 4);
            config[51] = new ShopConfig(51, 305, 60f, 122, 4);
            config[52] = new ShopConfig(52, 310, 60f, 124, 4);
            config[53] = new ShopConfig(53, 315, 60f, 126, 4);
            config[54] = new ShopConfig(54, 320, 60f, 128, 4);
            config[55] = new ShopConfig(55, 325, 60f, 130, 4);
            config[56] = new ShopConfig(56, 330, 60f, 132, 4);
            config[57] = new ShopConfig(57, 335, 60f, 134, 4);
            config[58] = new ShopConfig(58, 340, 60f, 136, 4);
            config[59] = new ShopConfig(59, 345, 60f, 138, 4);
            config[60] = new ShopConfig(60, 350, 60f, 140, 4);
            config[61] = new ShopConfig(61, 355, 60f, 142, 4);
            config[62] = new ShopConfig(62, 360, 60f, 144, 4);
            config[63] = new ShopConfig(63, 365, 60f, 146, 4);
            config[64] = new ShopConfig(64, 370, 60f, 148, 4);
            config[65] = new ShopConfig(65, 375, 60f, 150, 4);
            config[66] = new ShopConfig(66, 380, 60f, 152, 4);
            config[67] = new ShopConfig(67, 385, 60f, 154, 4);
            config[68] = new ShopConfig(68, 390, 60f, 156, 4);
            config[69] = new ShopConfig(69, 395, 60f, 158, 4);
            config[70] = new ShopConfig(70, 400, 60f, 160, 4);
            config[71] = new ShopConfig(71, 405, 60f, 162, 4);
            config[72] = new ShopConfig(72, 410, 60f, 164, 4);
            config[73] = new ShopConfig(73, 415, 60f, 166, 4);
            config[74] = new ShopConfig(74, 420, 60f, 168, 4);
            config[75] = new ShopConfig(75, 425, 60f, 170, 4);
            config[76] = new ShopConfig(76, 430, 60f, 172, 4);
            config[77] = new ShopConfig(77, 435, 60f, 174, 4);
            config[78] = new ShopConfig(78, 440, 60f, 176, 4);
            config[79] = new ShopConfig(79, 445, 60f, 178, 4);
            config[80] = new ShopConfig(80, 450, 60f, 180, 4);
            config[81] = new ShopConfig(81, 455, 60f, 182, 4);
            config[82] = new ShopConfig(82, 460, 60f, 184, 4);
            config[83] = new ShopConfig(83, 465, 60f, 186, 4);
            config[84] = new ShopConfig(84, 470, 60f, 188, 4);
            config[85] = new ShopConfig(85, 475, 60f, 190, 4);
            config[86] = new ShopConfig(86, 480, 60f, 192, 4);
            config[87] = new ShopConfig(87, 485, 60f, 194, 4);
            config[88] = new ShopConfig(88, 490, 60f, 196, 4);
            config[89] = new ShopConfig(89, 495, 60f, 198, 4);
            config[90] = new ShopConfig(90, 500, 60f, 200, 4);
            config[91] = new ShopConfig(91, 505, 60f, 202, 4);
            config[92] = new ShopConfig(92, 510, 60f, 204, 4);
            config[93] = new ShopConfig(93, 515, 60f, 206, 4);
            config[94] = new ShopConfig(94, 520, 60f, 208, 4);
            config[95] = new ShopConfig(95, 525, 60f, 210, 4);
            config[96] = new ShopConfig(96, 530, 60f, 212, 4);
            config[97] = new ShopConfig(97, 535, 60f, 214, 4);
            config[98] = new ShopConfig(98, 540, 60f, 216, 4);
            config[99] = new ShopConfig(99, 545, 60f, 218, 4);
            config[100] = new ShopConfig(100, 550, 60f, 220, 4);

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
