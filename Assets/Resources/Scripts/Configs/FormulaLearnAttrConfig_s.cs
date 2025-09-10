using System;
using System.Collections;
using System.Collections.Generic;

namespace CommonConfig
{
    public class FormulaLearnAttrConfig
    {
        /// <summary>
        ///序列
        /// </summary>
        public int Id;
        /// <summary>
        ///重量
        /// </summary>
        public int Weight;


        public FormulaLearnAttrConfig(int Id, int Weight)
        {
            this.Id = Id;
            this.Weight = Weight;

        }

        public FormulaLearnAttrConfig() { }

        private static Dictionary<int, FormulaLearnAttrConfig> config = new Dictionary<int, FormulaLearnAttrConfig>();
        public static Dictionary<int, FormulaLearnAttrConfig>.ValueCollection ConfigList
        {
            get
            {
                return config.Values;
            }
        }

        public static void Refresh(Dictionary<int, FormulaLearnAttrConfig> dict)
        {
            config.Clear();
            config = dict;
        }

        public static void Load()
        {
            config.Clear();
            config[-100] = new FormulaLearnAttrConfig(-100, 1);
            config[-99] = new FormulaLearnAttrConfig(-99, 1);
            config[-98] = new FormulaLearnAttrConfig(-98, 1);
            config[-97] = new FormulaLearnAttrConfig(-97, 1);
            config[-96] = new FormulaLearnAttrConfig(-96, 1);
            config[-95] = new FormulaLearnAttrConfig(-95, 1);
            config[-94] = new FormulaLearnAttrConfig(-94, 1);
            config[-93] = new FormulaLearnAttrConfig(-93, 1);
            config[-92] = new FormulaLearnAttrConfig(-92, 1);
            config[-91] = new FormulaLearnAttrConfig(-91, 1);
            config[-90] = new FormulaLearnAttrConfig(-90, 1);
            config[-89] = new FormulaLearnAttrConfig(-89, 1);
            config[-88] = new FormulaLearnAttrConfig(-88, 1);
            config[-87] = new FormulaLearnAttrConfig(-87, 1);
            config[-86] = new FormulaLearnAttrConfig(-86, 1);
            config[-85] = new FormulaLearnAttrConfig(-85, 1);
            config[-84] = new FormulaLearnAttrConfig(-84, 1);
            config[-83] = new FormulaLearnAttrConfig(-83, 1);
            config[-82] = new FormulaLearnAttrConfig(-82, 1);
            config[-81] = new FormulaLearnAttrConfig(-81, 1);
            config[-80] = new FormulaLearnAttrConfig(-80, 1);
            config[-79] = new FormulaLearnAttrConfig(-79, 1);
            config[-78] = new FormulaLearnAttrConfig(-78, 1);
            config[-77] = new FormulaLearnAttrConfig(-77, 1);
            config[-76] = new FormulaLearnAttrConfig(-76, 1);
            config[-75] = new FormulaLearnAttrConfig(-75, 1);
            config[-74] = new FormulaLearnAttrConfig(-74, 1);
            config[-73] = new FormulaLearnAttrConfig(-73, 1);
            config[-72] = new FormulaLearnAttrConfig(-72, 1);
            config[-71] = new FormulaLearnAttrConfig(-71, 1);
            config[-70] = new FormulaLearnAttrConfig(-70, 1);
            config[-69] = new FormulaLearnAttrConfig(-69, 1);
            config[-68] = new FormulaLearnAttrConfig(-68, 1);
            config[-67] = new FormulaLearnAttrConfig(-67, 1);
            config[-66] = new FormulaLearnAttrConfig(-66, 1);
            config[-65] = new FormulaLearnAttrConfig(-65, 1);
            config[-64] = new FormulaLearnAttrConfig(-64, 1);
            config[-63] = new FormulaLearnAttrConfig(-63, 1);
            config[-62] = new FormulaLearnAttrConfig(-62, 1);
            config[-61] = new FormulaLearnAttrConfig(-61, 1);
            config[-60] = new FormulaLearnAttrConfig(-60, 1);
            config[-59] = new FormulaLearnAttrConfig(-59, 1);
            config[-58] = new FormulaLearnAttrConfig(-58, 1);
            config[-57] = new FormulaLearnAttrConfig(-57, 1);
            config[-56] = new FormulaLearnAttrConfig(-56, 1);
            config[-55] = new FormulaLearnAttrConfig(-55, 1);
            config[-54] = new FormulaLearnAttrConfig(-54, 1);
            config[-53] = new FormulaLearnAttrConfig(-53, 1);
            config[-52] = new FormulaLearnAttrConfig(-52, 1);
            config[-51] = new FormulaLearnAttrConfig(-51, 1);
            config[-50] = new FormulaLearnAttrConfig(-50, 1);
            config[-49] = new FormulaLearnAttrConfig(-49, 1);
            config[-48] = new FormulaLearnAttrConfig(-48, 1);
            config[-47] = new FormulaLearnAttrConfig(-47, 1);
            config[-46] = new FormulaLearnAttrConfig(-46, 1);
            config[-45] = new FormulaLearnAttrConfig(-45, 1);
            config[-44] = new FormulaLearnAttrConfig(-44, 1);
            config[-43] = new FormulaLearnAttrConfig(-43, 1);
            config[-42] = new FormulaLearnAttrConfig(-42, 1);
            config[-41] = new FormulaLearnAttrConfig(-41, 1);
            config[-40] = new FormulaLearnAttrConfig(-40, 1);
            config[-39] = new FormulaLearnAttrConfig(-39, 1);
            config[-38] = new FormulaLearnAttrConfig(-38, 1);
            config[-37] = new FormulaLearnAttrConfig(-37, 1);
            config[-36] = new FormulaLearnAttrConfig(-36, 1);
            config[-35] = new FormulaLearnAttrConfig(-35, 1);
            config[-34] = new FormulaLearnAttrConfig(-34, 1);
            config[-33] = new FormulaLearnAttrConfig(-33, 1);
            config[-32] = new FormulaLearnAttrConfig(-32, 1);
            config[-31] = new FormulaLearnAttrConfig(-31, 1);
            config[-30] = new FormulaLearnAttrConfig(-30, 2);
            config[-29] = new FormulaLearnAttrConfig(-29, 2);
            config[-28] = new FormulaLearnAttrConfig(-28, 2);
            config[-27] = new FormulaLearnAttrConfig(-27, 2);
            config[-26] = new FormulaLearnAttrConfig(-26, 2);
            config[-25] = new FormulaLearnAttrConfig(-25, 2);
            config[-24] = new FormulaLearnAttrConfig(-24, 2);
            config[-23] = new FormulaLearnAttrConfig(-23, 2);
            config[-22] = new FormulaLearnAttrConfig(-22, 2);
            config[-21] = new FormulaLearnAttrConfig(-21, 2);
            config[-20] = new FormulaLearnAttrConfig(-20, 3);
            config[-19] = new FormulaLearnAttrConfig(-19, 3);
            config[-18] = new FormulaLearnAttrConfig(-18, 3);
            config[-17] = new FormulaLearnAttrConfig(-17, 3);
            config[-16] = new FormulaLearnAttrConfig(-16, 3);
            config[-15] = new FormulaLearnAttrConfig(-15, 3);
            config[-14] = new FormulaLearnAttrConfig(-14, 3);
            config[-13] = new FormulaLearnAttrConfig(-13, 3);
            config[-12] = new FormulaLearnAttrConfig(-12, 3);
            config[-11] = new FormulaLearnAttrConfig(-11, 3);
            config[-10] = new FormulaLearnAttrConfig(-10, 4);
            config[-9] = new FormulaLearnAttrConfig(-9, 4);
            config[-8] = new FormulaLearnAttrConfig(-8, 4);
            config[-7] = new FormulaLearnAttrConfig(-7, 4);
            config[-6] = new FormulaLearnAttrConfig(-6, 4);
            config[-5] = new FormulaLearnAttrConfig(-5, 4);
            config[-4] = new FormulaLearnAttrConfig(-4, 4);
            config[-3] = new FormulaLearnAttrConfig(-3, 4);
            config[-2] = new FormulaLearnAttrConfig(-2, 4);
            config[-1] = new FormulaLearnAttrConfig(-1, 4);
            config[0] = new FormulaLearnAttrConfig(0, 5);
            config[1] = new FormulaLearnAttrConfig(1, 5);
            config[2] = new FormulaLearnAttrConfig(2, 5);
            config[3] = new FormulaLearnAttrConfig(3, 5);
            config[4] = new FormulaLearnAttrConfig(4, 5);
            config[5] = new FormulaLearnAttrConfig(5, 6);
            config[6] = new FormulaLearnAttrConfig(6, 6);
            config[7] = new FormulaLearnAttrConfig(7, 6);
            config[8] = new FormulaLearnAttrConfig(8, 6);
            config[9] = new FormulaLearnAttrConfig(9, 6);
            config[10] = new FormulaLearnAttrConfig(10, 7);
            config[11] = new FormulaLearnAttrConfig(11, 7);
            config[12] = new FormulaLearnAttrConfig(12, 7);
            config[13] = new FormulaLearnAttrConfig(13, 7);
            config[14] = new FormulaLearnAttrConfig(14, 7);
            config[15] = new FormulaLearnAttrConfig(15, 8);
            config[16] = new FormulaLearnAttrConfig(16, 8);
            config[17] = new FormulaLearnAttrConfig(17, 8);
            config[18] = new FormulaLearnAttrConfig(18, 8);
            config[19] = new FormulaLearnAttrConfig(19, 8);
            config[20] = new FormulaLearnAttrConfig(20, 9);
            config[21] = new FormulaLearnAttrConfig(21, 9);
            config[22] = new FormulaLearnAttrConfig(22, 9);
            config[23] = new FormulaLearnAttrConfig(23, 9);
            config[24] = new FormulaLearnAttrConfig(24, 9);
            config[25] = new FormulaLearnAttrConfig(25, 10);
            config[26] = new FormulaLearnAttrConfig(26, 10);
            config[27] = new FormulaLearnAttrConfig(27, 10);
            config[28] = new FormulaLearnAttrConfig(28, 10);
            config[29] = new FormulaLearnAttrConfig(29, 10);
            config[30] = new FormulaLearnAttrConfig(30, 11);
            config[31] = new FormulaLearnAttrConfig(31, 11);
            config[32] = new FormulaLearnAttrConfig(32, 11);
            config[33] = new FormulaLearnAttrConfig(33, 11);
            config[34] = new FormulaLearnAttrConfig(34, 11);
            config[35] = new FormulaLearnAttrConfig(35, 12);
            config[36] = new FormulaLearnAttrConfig(36, 12);
            config[37] = new FormulaLearnAttrConfig(37, 12);
            config[38] = new FormulaLearnAttrConfig(38, 12);
            config[39] = new FormulaLearnAttrConfig(39, 12);
            config[40] = new FormulaLearnAttrConfig(40, 13);
            config[41] = new FormulaLearnAttrConfig(41, 13);
            config[42] = new FormulaLearnAttrConfig(42, 13);
            config[43] = new FormulaLearnAttrConfig(43, 13);
            config[44] = new FormulaLearnAttrConfig(44, 13);
            config[45] = new FormulaLearnAttrConfig(45, 14);
            config[46] = new FormulaLearnAttrConfig(46, 14);
            config[47] = new FormulaLearnAttrConfig(47, 14);
            config[48] = new FormulaLearnAttrConfig(48, 14);
            config[49] = new FormulaLearnAttrConfig(49, 14);
            config[50] = new FormulaLearnAttrConfig(50, 15);
            config[51] = new FormulaLearnAttrConfig(51, 15);
            config[52] = new FormulaLearnAttrConfig(52, 15);
            config[53] = new FormulaLearnAttrConfig(53, 15);
            config[54] = new FormulaLearnAttrConfig(54, 15);
            config[55] = new FormulaLearnAttrConfig(55, 16);
            config[56] = new FormulaLearnAttrConfig(56, 16);
            config[57] = new FormulaLearnAttrConfig(57, 16);
            config[58] = new FormulaLearnAttrConfig(58, 16);
            config[59] = new FormulaLearnAttrConfig(59, 16);
            config[60] = new FormulaLearnAttrConfig(60, 17);
            config[61] = new FormulaLearnAttrConfig(61, 17);
            config[62] = new FormulaLearnAttrConfig(62, 17);
            config[63] = new FormulaLearnAttrConfig(63, 17);
            config[64] = new FormulaLearnAttrConfig(64, 17);
            config[65] = new FormulaLearnAttrConfig(65, 18);
            config[66] = new FormulaLearnAttrConfig(66, 18);
            config[67] = new FormulaLearnAttrConfig(67, 18);
            config[68] = new FormulaLearnAttrConfig(68, 18);
            config[69] = new FormulaLearnAttrConfig(69, 18);
            config[70] = new FormulaLearnAttrConfig(70, 19);
            config[71] = new FormulaLearnAttrConfig(71, 19);
            config[72] = new FormulaLearnAttrConfig(72, 19);
            config[73] = new FormulaLearnAttrConfig(73, 19);
            config[74] = new FormulaLearnAttrConfig(74, 19);
            config[75] = new FormulaLearnAttrConfig(75, 20);
            config[76] = new FormulaLearnAttrConfig(76, 20);
            config[77] = new FormulaLearnAttrConfig(77, 20);
            config[78] = new FormulaLearnAttrConfig(78, 20);
            config[79] = new FormulaLearnAttrConfig(79, 20);
            config[80] = new FormulaLearnAttrConfig(80, 21);
            config[81] = new FormulaLearnAttrConfig(81, 21);
            config[82] = new FormulaLearnAttrConfig(82, 21);
            config[83] = new FormulaLearnAttrConfig(83, 21);
            config[84] = new FormulaLearnAttrConfig(84, 21);
            config[85] = new FormulaLearnAttrConfig(85, 22);
            config[86] = new FormulaLearnAttrConfig(86, 22);
            config[87] = new FormulaLearnAttrConfig(87, 22);
            config[88] = new FormulaLearnAttrConfig(88, 22);
            config[89] = new FormulaLearnAttrConfig(89, 22);
            config[90] = new FormulaLearnAttrConfig(90, 23);
            config[91] = new FormulaLearnAttrConfig(91, 23);
            config[92] = new FormulaLearnAttrConfig(92, 23);
            config[93] = new FormulaLearnAttrConfig(93, 23);
            config[94] = new FormulaLearnAttrConfig(94, 23);
            config[95] = new FormulaLearnAttrConfig(95, 24);
            config[96] = new FormulaLearnAttrConfig(96, 24);
            config[97] = new FormulaLearnAttrConfig(97, 24);
            config[98] = new FormulaLearnAttrConfig(98, 24);
            config[99] = new FormulaLearnAttrConfig(99, 24);
            config[100] = new FormulaLearnAttrConfig(100, 25);

        }

        public static FormulaLearnAttrConfig GetConfig(int id)
        {
            FormulaLearnAttrConfig data;
            if (config.TryGetValue(id, out data))
            {
                return data;
            }
            throw new NullReferenceException(string.Format("配置表FormulaLearnAttrConfig不存在id={0}", id));
        }

        public static bool HasConfig(int id)
        {
            if (config.ContainsKey(id))
            {
                return true;
            }
            return false;
        }

        public static void Assign(int id, FormulaLearnAttrConfig configData)
        {
            config[id] = configData; 
        }

        public static void Add(int id, FormulaLearnAttrConfig configData)
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
