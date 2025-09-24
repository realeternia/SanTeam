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
            config[1] = new ShopConfig(1, 70, 0f, 0, 0);
            config[2] = new ShopConfig(2, 60, 0f, 0, 0);
            config[3] = new ShopConfig(3, 65, 5f, 26, 0);
            config[4] = new ShopConfig(4, 70, 10f, 28, 1);
            config[5] = new ShopConfig(5, 75, 15f, 30, 1);
            config[6] = new ShopConfig(6, 80, 20f, 32, 1);
            config[7] = new ShopConfig(7, 85, 25f, 34, 2);
            config[8] = new ShopConfig(8, 90, 30f, 36, 2);
            config[9] = new ShopConfig(9, 95, 35f, 38, 2);
            config[10] = new ShopConfig(10, 100, 40f, 40, 2);
            config[11] = new ShopConfig(11, 105, 45f, 42, 3);
            config[12] = new ShopConfig(12, 110, 50f, 44, 3);
            config[13] = new ShopConfig(13, 115, 55f, 46, 3);
            config[14] = new ShopConfig(14, 120, 60f, 48, 3);
            config[15] = new ShopConfig(15, 125, 60f, 50, 4);
            config[16] = new ShopConfig(16, 130, 60f, 52, 4);
            config[17] = new ShopConfig(17, 135, 60f, 54, 4);
            config[18] = new ShopConfig(18, 140, 60f, 56, 4);
            config[19] = new ShopConfig(19, 145, 60f, 58, 5);
            config[20] = new ShopConfig(20, 150, 60f, 60, 5);
            config[21] = new ShopConfig(21, 155, 60f, 62, 5);
            config[22] = new ShopConfig(22, 160, 60f, 64, 5);
            config[23] = new ShopConfig(23, 165, 60f, 66, 5);
            config[24] = new ShopConfig(24, 170, 60f, 68, 5);
            config[25] = new ShopConfig(25, 175, 60f, 70, 6);
            config[26] = new ShopConfig(26, 180, 60f, 72, 6);
            config[27] = new ShopConfig(27, 185, 60f, 74, 6);
            config[28] = new ShopConfig(28, 190, 60f, 76, 6);
            config[29] = new ShopConfig(29, 195, 60f, 78, 6);
            config[30] = new ShopConfig(30, 200, 60f, 80, 6);
            config[31] = new ShopConfig(31, 205, 60f, 82, 6);
            config[32] = new ShopConfig(32, 210, 60f, 84, 6);
            config[33] = new ShopConfig(33, 215, 60f, 86, 6);
            config[34] = new ShopConfig(34, 220, 60f, 88, 6);
            config[35] = new ShopConfig(35, 225, 60f, 90, 6);
            config[36] = new ShopConfig(36, 230, 60f, 92, 6);
            config[37] = new ShopConfig(37, 235, 60f, 94, 6);
            config[38] = new ShopConfig(38, 240, 60f, 96, 6);
            config[39] = new ShopConfig(39, 245, 60f, 98, 6);
            config[40] = new ShopConfig(40, 250, 60f, 100, 6);
            config[41] = new ShopConfig(41, 255, 60f, 102, 6);
            config[42] = new ShopConfig(42, 260, 60f, 104, 6);
            config[43] = new ShopConfig(43, 265, 60f, 106, 6);
            config[44] = new ShopConfig(44, 270, 60f, 108, 6);
            config[45] = new ShopConfig(45, 275, 60f, 110, 6);
            config[46] = new ShopConfig(46, 280, 60f, 112, 6);
            config[47] = new ShopConfig(47, 285, 60f, 114, 6);
            config[48] = new ShopConfig(48, 290, 60f, 116, 6);
            config[49] = new ShopConfig(49, 295, 60f, 118, 6);
            config[50] = new ShopConfig(50, 300, 60f, 120, 6);
            config[51] = new ShopConfig(51, 305, 60f, 122, 6);
            config[52] = new ShopConfig(52, 310, 60f, 124, 6);
            config[53] = new ShopConfig(53, 315, 60f, 126, 6);
            config[54] = new ShopConfig(54, 320, 60f, 128, 6);
            config[55] = new ShopConfig(55, 325, 60f, 130, 6);
            config[56] = new ShopConfig(56, 330, 60f, 132, 6);
            config[57] = new ShopConfig(57, 335, 60f, 134, 6);
            config[58] = new ShopConfig(58, 340, 60f, 136, 6);
            config[59] = new ShopConfig(59, 345, 60f, 138, 6);
            config[60] = new ShopConfig(60, 350, 60f, 140, 6);
            config[61] = new ShopConfig(61, 355, 60f, 142, 6);
            config[62] = new ShopConfig(62, 360, 60f, 144, 6);
            config[63] = new ShopConfig(63, 365, 60f, 146, 6);
            config[64] = new ShopConfig(64, 370, 60f, 148, 6);
            config[65] = new ShopConfig(65, 375, 60f, 150, 6);
            config[66] = new ShopConfig(66, 380, 60f, 152, 6);
            config[67] = new ShopConfig(67, 385, 60f, 154, 6);
            config[68] = new ShopConfig(68, 390, 60f, 156, 6);
            config[69] = new ShopConfig(69, 395, 60f, 158, 6);
            config[70] = new ShopConfig(70, 400, 60f, 160, 6);
            config[71] = new ShopConfig(71, 405, 60f, 162, 6);
            config[72] = new ShopConfig(72, 410, 60f, 164, 6);
            config[73] = new ShopConfig(73, 415, 60f, 166, 6);
            config[74] = new ShopConfig(74, 420, 60f, 168, 6);
            config[75] = new ShopConfig(75, 425, 60f, 170, 6);
            config[76] = new ShopConfig(76, 430, 60f, 172, 6);
            config[77] = new ShopConfig(77, 435, 60f, 174, 6);
            config[78] = new ShopConfig(78, 440, 60f, 176, 6);
            config[79] = new ShopConfig(79, 445, 60f, 178, 6);
            config[80] = new ShopConfig(80, 450, 60f, 180, 6);
            config[81] = new ShopConfig(81, 455, 60f, 182, 6);
            config[82] = new ShopConfig(82, 460, 60f, 184, 6);
            config[83] = new ShopConfig(83, 465, 60f, 186, 6);
            config[84] = new ShopConfig(84, 470, 60f, 188, 6);
            config[85] = new ShopConfig(85, 475, 60f, 190, 6);
            config[86] = new ShopConfig(86, 480, 60f, 192, 6);
            config[87] = new ShopConfig(87, 485, 60f, 194, 6);
            config[88] = new ShopConfig(88, 490, 60f, 196, 6);
            config[89] = new ShopConfig(89, 495, 60f, 198, 6);
            config[90] = new ShopConfig(90, 500, 60f, 200, 6);
            config[91] = new ShopConfig(91, 505, 60f, 202, 6);
            config[92] = new ShopConfig(92, 510, 60f, 204, 6);
            config[93] = new ShopConfig(93, 515, 60f, 206, 6);
            config[94] = new ShopConfig(94, 520, 60f, 208, 6);
            config[95] = new ShopConfig(95, 525, 60f, 210, 6);
            config[96] = new ShopConfig(96, 530, 60f, 212, 6);
            config[97] = new ShopConfig(97, 535, 60f, 214, 6);
            config[98] = new ShopConfig(98, 540, 60f, 216, 6);
            config[99] = new ShopConfig(99, 545, 60f, 218, 6);
            config[100] = new ShopConfig(100, 550, 60f, 220, 6);

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
