using System;
using System.Collections;
using System.Collections.Generic;

namespace CommonConfig
{
    public class HeroFriendConfig
    {
        /// <summary>
        ///序列
        /// </summary>
        public int Id;
        /// <summary>
        ///名字
        /// </summary>
        public string Name;
        /// <summary>
        ///支援级别（3最高，1最低（比如共事））
        /// </summary>
        public int Level;
        /// <summary>
        ///英雄列表，最多5人，一般2-3人较多
        /// </summary>
        public int[] Heros;


        public HeroFriendConfig(int Id, string Name, int Level, int[] Heros)
        {
            this.Id = Id;
            this.Name = Name;
            this.Level = Level;
            this.Heros = Heros;

        }

        public HeroFriendConfig() { }

        private static Dictionary<int, HeroFriendConfig> config = new Dictionary<int, HeroFriendConfig>();
        public static Dictionary<int, HeroFriendConfig>.ValueCollection ConfigList
        {
            get
            {
                return config.Values;
            }
        }

        public static void Refresh(Dictionary<int, HeroFriendConfig> dict)
        {
            config.Clear();
            config = dict;
        }

        public static void Load()
        {
            config.Clear();
            config[1] = new HeroFriendConfig(1, "桃源三结义", 2, new int[]{100001,101002,101005});
            config[2] = new HeroFriendConfig(2, "五虎上将", 1, new int[]{101001,101002,101003,101005,101008});
            config[3] = new HeroFriendConfig(3, "诸葛亮姜维", 2, new int[]{101004,101010});
            config[4] = new HeroFriendConfig(4, "卧龙凤雏", 1, new int[]{101004,101017});
            config[5] = new HeroFriendConfig(5, "马腾父子", 2, new int[]{101003,101011,110005});
            config[6] = new HeroFriendConfig(6, "关羽关平关兴", 2, new int[]{101005,101013,101012,101021});
            config[7] = new HeroFriendConfig(7, "张飞张苞", 2, new int[]{101002,101019});
            config[8] = new HeroFriendConfig(8, "诸葛亮黄月英", 2, new int[]{101004,101032});
            config[9] = new HeroFriendConfig(9, "孟获祝融", 2, new int[]{101016,101035});
            config[10] = new HeroFriendConfig(10, "魏延黄忠", 1, new int[]{101007,101008});
            config[11] = new HeroFriendConfig(11, "严颜张飞", 2, new int[]{101014,101002});
            config[12] = new HeroFriendConfig(12, "法正张松", 1, new int[]{101033,101020});
            config[13] = new HeroFriendConfig(13, "徐庶诸葛亮", 2, new int[]{101006,101004});
            config[14] = new HeroFriendConfig(14, "曹魏宗室核心", 1, new int[]{102002,102006,102012,102013});
            config[15] = new HeroFriendConfig(15, "曹操父子", 3, new int[]{102029,100002,102030,102033});
            config[16] = new HeroFriendConfig(16, "五子良将", 1, new int[]{102004,102008,102009,102011,102015});
            config[17] = new HeroFriendConfig(17, "颍川谋士团", 1, new int[]{102001,102003,102010,102022});
            config[18] = new HeroFriendConfig(18, "虎卫双雄", 1, new int[]{102005,102007});
            config[19] = new HeroFriendConfig(19, "夏侯双雄", 2, new int[]{102002,102006});
            config[20] = new HeroFriendConfig(20, "司马张郃", 1, new int[]{102018,102008});
            config[21] = new HeroFriendConfig(21, "江东基业", 1, new int[]{103001,103002,100003});
            config[22] = new HeroFriendConfig(22, "江表老臣", 1, new int[]{103005,103024,103029});
            config[23] = new HeroFriendConfig(23, "虎臣", 1, new int[]{103009,103006,103019,103034});
            config[24] = new HeroFriendConfig(24, "都督传承", 1, new int[]{103008,103010,103011});
            config[25] = new HeroFriendConfig(25, "孙策周瑜", 2, new int[]{103002,103008});
            config[26] = new HeroFriendConfig(26, "二乔姐妹", 2, new int[]{103016,103017});
            config[27] = new HeroFriendConfig(27, "甘宁太史慈", 2, new int[]{103003,103004});
            config[28] = new HeroFriendConfig(28, "陆逊陆抗", 2, new int[]{103011,103030});
            config[29] = new HeroFriendConfig(29, "吕布军团", 1, new int[]{104001,104006,104005});
            config[30] = new HeroFriendConfig(30, "河北猛将", 1, new int[]{106006,106001,106002,102008});
            config[31] = new HeroFriendConfig(31, "黄巾之乱", 1, new int[]{110007,110008,110009});
            config[32] = new HeroFriendConfig(32, "司马家族", 2, new int[]{105002,105003,100005,102018});
            config[33] = new HeroFriendConfig(33, "西晋名将", 1, new int[]{105001,105005,105008});
            config[34] = new HeroFriendConfig(34, "吕布貂蝉", 2, new int[]{104001,104004});
            config[35] = new HeroFriendConfig(35, "关羽张辽", 2, new int[]{101005,102004});
            config[36] = new HeroFriendConfig(36, "曹操关羽", 2, new int[]{100002,101005});
            config[37] = new HeroFriendConfig(37, "鲁肃周瑜", 2, new int[]{103007,103008});
            config[38] = new HeroFriendConfig(38, "孙权周泰", 2, new int[]{100003,103006});
            config[39] = new HeroFriendConfig(39, "曹操典韦", 2, new int[]{100002,102007,102005});
            config[40] = new HeroFriendConfig(40, "刘备赵云", 2, new int[]{100001,101001});
            config[41] = new HeroFriendConfig(41, "诸葛亮司马懿", 2, new int[]{101004,102018});
            config[42] = new HeroFriendConfig(42, "孙策太史慈", 2, new int[]{103002,103004});
            config[43] = new HeroFriendConfig(43, "法正黄忠", 2, new int[]{101033,101008});
            config[44] = new HeroFriendConfig(44, "郭嘉张辽", 2, new int[]{102001,102004});
            config[45] = new HeroFriendConfig(45, "司马懿郭淮", 2, new int[]{102018,102035});
            config[46] = new HeroFriendConfig(46, "杜预王濬", 2, new int[]{105007,105008});
            config[47] = new HeroFriendConfig(47, "阚泽黄盖", 2, new int[]{103008,103028,103005});
            config[48] = new HeroFriendConfig(48, "吕布陈宫", 2, new int[]{104001,104010});
            config[49] = new HeroFriendConfig(49, "许攸曹操", 2, new int[]{106005,100002});
            config[50] = new HeroFriendConfig(50, "司马昭邓艾", 2, new int[]{105003,105001});
            config[51] = new HeroFriendConfig(51, "羊祜杜预", 2, new int[]{105004,105007,100005});
            config[52] = new HeroFriendConfig(52, "张辽李典", 2, new int[]{102004,102028});
            config[53] = new HeroFriendConfig(53, "吕布董卓", 2, new int[]{100004,104001});
            config[54] = new HeroFriendConfig(54, "四世三公", 2, new int[]{100006,110004});
            config[55] = new HeroFriendConfig(55, "河北智囊", 2, new int[]{106003,106007,106008});
            config[56] = new HeroFriendConfig(56, "英雄美人", 3, new int[]{103002,103016});
            config[57] = new HeroFriendConfig(57, "周郎顾曲", 3, new int[]{103008,103017});
            config[58] = new HeroFriendConfig(58, "孙氏兄妹", 3, new int[]{100003,103014});
            config[59] = new HeroFriendConfig(59, "夫妻", 3, new int[]{100001,103014});
            config[61] = new HeroFriendConfig(61, "荀氏叔侄", 2, new int[]{102003,102010});
            config[62] = new HeroFriendConfig(62, "江东二张", 2, new int[]{103012,103025});
            config[63] = new HeroFriendConfig(63, "马良马谡", 2, new int[]{101031,101026});
            config[64] = new HeroFriendConfig(64, "诸葛兄弟", 2, new int[]{103013,103031});
            config[65] = new HeroFriendConfig(65, "阿斗赵云", 3, new int[]{101034,101001});
            config[66] = new HeroFriendConfig(66, "托孤大臣", 2, new int[]{101023,101025,101027,101030});
            config[67] = new HeroFriendConfig(67, "李傕郭汜", 1, new int[]{104008,104009,104003});
            config[68] = new HeroFriendConfig(68, "关心张宝", 2, new int[]{101012,101019});
            config[69] = new HeroFriendConfig(69, "钟会姜维", 2, new int[]{101010,105005,102019});
            config[70] = new HeroFriendConfig(70, "义子", 2, new int[]{101038,100001});
            config[71] = new HeroFriendConfig(71, "父子君臣", 3, new int[]{100001,101034});
            config[72] = new HeroFriendConfig(72, "三气周瑜", 2, new int[]{103008,101004});
            config[73] = new HeroFriendConfig(73, "川蜀集团", 1, new int[]{101033,101020,101036,101018});
            config[74] = new HeroFriendConfig(74, "孙家子弟", 2, new int[]{103036,103037});
            config[75] = new HeroFriendConfig(75, "东吴砥柱", 2, new int[]{103023,103018});
            config[76] = new HeroFriendConfig(76, "江东铁壁", 2, new int[]{103009,103006});
            config[77] = new HeroFriendConfig(77, "合肥之盾", 2, new int[]{102028,102015});
            config[78] = new HeroFriendConfig(78, "陷阵之锋", 3, new int[]{104006,102004});
            config[80] = new HeroFriendConfig(80, "淮南骁将", 2, new int[]{102034,102025});
            config[81] = new HeroFriendConfig(81, "忠勇随扈", 3, new int[]{101005,101009});
            config[82] = new HeroFriendConfig(82, "蜀汉先锋", 2, new int[]{101015,101010});
            config[83] = new HeroFriendConfig(83, "刘备旧部", 2, new int[]{101022,101024});
            config[84] = new HeroFriendConfig(84, "汉中砥柱", 1, new int[]{101028,102020});
            config[85] = new HeroFriendConfig(85, "曹家千里驹", 2, new int[]{102017,102026,102033});
            config[86] = new HeroFriendConfig(86, "曹仁牛金", 1, new int[]{102024,102012});
            config[87] = new HeroFriendConfig(87, "制度建立", 1, new int[]{102027,102003});
            config[88] = new HeroFriendConfig(88, "钟氏父子", 2, new int[]{102037,105005});
            config[89] = new HeroFriendConfig(89, "江东屏障", 2, new int[]{103015,103006});
            config[90] = new HeroFriendConfig(90, "谋士", 2, new int[]{104007,100004});
            config[91] = new HeroFriendConfig(91, "陈氏父子", 2, new int[]{105006,102027});
            config[92] = new HeroFriendConfig(92, "同窗", 2, new int[]{110001,100001});
            config[93] = new HeroFriendConfig(93, "河北旧主", 2, new int[]{110001,101001});
            config[94] = new HeroFriendConfig(94, "东吴栋梁", 2, new int[]{103027,103026,103038});
            config[95] = new HeroFriendConfig(95, "仇人", 2, new int[]{103003,103020});
            config[96] = new HeroFriendConfig(96, "文武双全", 2, new int[]{104002,104003});
            config[97] = new HeroFriendConfig(97, "西凉军", 2, new int[]{101003,101011,102014});
            config[98] = new HeroFriendConfig(98, "樊城防御", 1, new int[]{102011,102014});
            config[99] = new HeroFriendConfig(99, "虎侯之威", 1, new int[]{102005,101003});
            config[100] = new HeroFriendConfig(100, "讨董联盟", 2, new int[]{100002,103001,100006,110004,110005});
            config[101] = new HeroFriendConfig(101, "破吴之役", 1, new int[]{102016,102009,102036});
            config[102] = new HeroFriendConfig(102, "刮目相看", 2, new int[]{103007,103010});
            config[103] = new HeroFriendConfig(103, "白衣渡江", 2, new int[]{103021,103010,103035});
            config[104] = new HeroFriendConfig(104, "朱朱", 3, new int[]{103022,103035,103015});
            config[105] = new HeroFriendConfig(105, "救人", 2, new int[]{100002,110003});
            config[106] = new HeroFriendConfig(106, "救人", 2, new int[]{101005,110003});
            config[107] = new HeroFriendConfig(107, "曹丕司马懿", 2, new int[]{102018,102029});

        }

        public static HeroFriendConfig GetConfig(int id)
        {
            HeroFriendConfig data;
            if (config.TryGetValue(id, out data))
            {
                return data;
            }
            throw new NullReferenceException(string.Format("配置表HeroFriendConfig不存在id={0}", id));
        }

        public static bool HasConfig(int id)
        {
            if (config.ContainsKey(id))
            {
                return true;
            }
            return false;
        }

        public static void Assign(int id, HeroFriendConfig configData)
        {
            config[id] = configData; 
        }

        public static void Add(int id, HeroFriendConfig configData)
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
