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
            config[1] = new HeroFriendConfig(1, "桃源结义", 3, new int[]{100001,101002,101005});
            config[2] = new HeroFriendConfig(2, "五虎上将", 2, new int[]{101001,101002,101003,101005,101008});
            config[3] = new HeroFriendConfig(3, "师徒护汉", 3, new int[]{101004,101010});
            config[4] = new HeroFriendConfig(4, "卧龙凤雏", 2, new int[]{101004,101017});
            config[5] = new HeroFriendConfig(5, "马腾父子", 3, new int[]{101003,101011,110005});
            config[6] = new HeroFriendConfig(6, "关式虎将", 3, new int[]{101005,101013,101012,101021});
            config[7] = new HeroFriendConfig(7, "张氏父子", 3, new int[]{101002,101019});
            config[8] = new HeroFriendConfig(8, "孔明夫妻", 3, new int[]{101004,101032});
            config[9] = new HeroFriendConfig(9, "南蛮夫妻", 3, new int[]{101016,101035});
            config[10] = new HeroFriendConfig(10, "长沙守将", 1, new int[]{101007,101008});
            config[11] = new HeroFriendConfig(11, "粗中有细", 1, new int[]{101014,101002});
            config[12] = new HeroFriendConfig(12, "孔明元直", 2, new int[]{101006,101004});
            config[13] = new HeroFriendConfig(13, "曹魏宗室核心", 2, new int[]{102002,102006,102012,102013});
            config[14] = new HeroFriendConfig(14, "曹操父子", 3, new int[]{102029,100002,102030,102033,102039});
            config[15] = new HeroFriendConfig(15, "五子良将", 2, new int[]{102004,102008,102009,102011,102015});
            config[16] = new HeroFriendConfig(16, "颍川谋士团", 1, new int[]{102001,102003,102010,102022});
            config[17] = new HeroFriendConfig(17, "夏侯双雄", 2, new int[]{102002,102006});
            config[18] = new HeroFriendConfig(18, "司马张郃", 1, new int[]{102018,102008});
            config[19] = new HeroFriendConfig(19, "江东基业", 3, new int[]{103001,103002,100003});
            config[20] = new HeroFriendConfig(20, "江表老臣", 1, new int[]{103001,103002,103005,103024,103029});
            config[21] = new HeroFriendConfig(21, "江东虎臣", 1, new int[]{103009,103006,103019,103034});
            config[22] = new HeroFriendConfig(22, "都督传承", 2, new int[]{103008,103010,103011});
            config[23] = new HeroFriendConfig(23, "瑜策", 3, new int[]{103002,103008});
            config[24] = new HeroFriendConfig(24, "二乔", 3, new int[]{103016,103017});
            config[25] = new HeroFriendConfig(25, "帆箭", 2, new int[]{103003,103004});
            config[26] = new HeroFriendConfig(26, "陆氏父子", 3, new int[]{103011,103030});
            config[27] = new HeroFriendConfig(27, "吕布帐下", 1, new int[]{104001,104006,104005});
            config[28] = new HeroFriendConfig(28, "河北庭柱", 2, new int[]{106006,106001,106002,102008});
            config[29] = new HeroFriendConfig(29, "黄巾之乱", 3, new int[]{110007,110008,110009});
            config[30] = new HeroFriendConfig(30, "司马家族", 3, new int[]{105002,105003,100005,102018});
            config[31] = new HeroFriendConfig(31, "西晋名将", 1, new int[]{105001,105005,105008});
            config[32] = new HeroFriendConfig(32, "英雄美人", 3, new int[]{104001,104004});
            config[33] = new HeroFriendConfig(33, "英雄相惜", 2, new int[]{101005,102004});
            config[34] = new HeroFriendConfig(34, "汉寿之恩", 1, new int[]{100002,101005});
            config[35] = new HeroFriendConfig(35, "总角之好", 2, new int[]{103007,103008});
            config[36] = new HeroFriendConfig(36, "护主之功", 2, new int[]{100003,103006});
            config[37] = new HeroFriendConfig(37, "护主双戟", 2, new int[]{100002,102007,102005});
            config[38] = new HeroFriendConfig(38, "仁主良将", 2, new int[]{100001,101001});
            config[39] = new HeroFriendConfig(39, "祁山斗智", 1, new int[]{101004,102018});
            config[40] = new HeroFriendConfig(40, "神亭酣战", 1, new int[]{103002,103004});
            config[41] = new HeroFriendConfig(41, "定军扬威", 1, new int[]{101033,101008});
            config[42] = new HeroFriendConfig(42, "北征帷幄", 1, new int[]{102001,102004});
            config[43] = new HeroFriendConfig(43, "雍凉屏障", 1, new int[]{102018,102035});
            config[44] = new HeroFriendConfig(44, "平吴先锋", 1, new int[]{105007,105008});
            config[45] = new HeroFriendConfig(45, "苦肉献策", 1, new int[]{103008,103028,103005});
            config[46] = new HeroFriendConfig(46, "有勇有谋", 2, new int[]{104001,104010});
            config[47] = new HeroFriendConfig(47, "官渡奇谋", 1, new int[]{106005,100002});
            config[48] = new HeroFriendConfig(48, "灭蜀之功", 1, new int[]{105003,105001});
            config[49] = new HeroFriendConfig(49, "舟楫平吴", 1, new int[]{105004,105007,100005});
            config[50] = new HeroFriendConfig(50, "合肥同心", 1, new int[]{102004,102028});
            config[51] = new HeroFriendConfig(51, "凤仪亭", 1, new int[]{100004,104001});
            config[52] = new HeroFriendConfig(52, "四世三公", 2, new int[]{100006,110004});
            config[53] = new HeroFriendConfig(53, "河北智囊", 2, new int[]{106003,106007,106008});
            config[54] = new HeroFriendConfig(54, "英雄美人", 3, new int[]{103002,103016});
            config[55] = new HeroFriendConfig(55, "顾曲周郎", 3, new int[]{103008,103017});
            config[56] = new HeroFriendConfig(56, "孙氏兄妹", 3, new int[]{100003,103014});
            config[57] = new HeroFriendConfig(57, "蜀汉同盟", 2, new int[]{100001,103014});
            config[58] = new HeroFriendConfig(58, "荀氏叔侄", 3, new int[]{102003,102010});
            config[59] = new HeroFriendConfig(59, "江东二张", 2, new int[]{103012,103025});
            config[60] = new HeroFriendConfig(60, "襄阳马氏", 2, new int[]{101031,101026});
            config[61] = new HeroFriendConfig(61, "龙虎兄弟", 2, new int[]{103013,103031});
            config[62] = new HeroFriendConfig(62, "长坂护主", 3, new int[]{101034,101001});
            config[63] = new HeroFriendConfig(63, "托孤大臣", 2, new int[]{101034,101023,101025,101027,101030});
            config[64] = new HeroFriendConfig(64, "祸乱长安", 1, new int[]{104008,104009,104003});
            config[65] = new HeroFriendConfig(65, "将门虎子", 3, new int[]{101012,101019});
            config[66] = new HeroFriendConfig(66, "二士争功", 2, new int[]{101010,105005,102019});
            config[67] = new HeroFriendConfig(67, "汉中王嗣", 2, new int[]{101038,100001});
            config[68] = new HeroFriendConfig(68, "父子君臣", 3, new int[]{100001,101034});
            config[69] = new HeroFriendConfig(69, "瑜亮", 2, new int[]{103008,101004});
            config[70] = new HeroFriendConfig(70, "川蜀集团", 1, new int[]{101033,101020,101036,101018,110002});
            config[71] = new HeroFriendConfig(71, "孙家子弟", 2, new int[]{103036,103037});
            config[72] = new HeroFriendConfig(72, "东吴砥柱", 2, new int[]{103023,103018});
            config[74] = new HeroFriendConfig(74, "合肥之盾", 2, new int[]{102028,102015});
            config[75] = new HeroFriendConfig(75, "陷阵之锋", 3, new int[]{104006,102004});
            config[76] = new HeroFriendConfig(76, "淮南骁将", 1, new int[]{102034,102025});
            config[77] = new HeroFriendConfig(77, "扛刀牵马", 2, new int[]{101005,101009});
            config[78] = new HeroFriendConfig(78, "蜀汉先锋", 2, new int[]{101015,101010});
            config[79] = new HeroFriendConfig(79, "刘备旧部", 1, new int[]{101022,101024});
            config[80] = new HeroFriendConfig(80, "汉中砥柱", 1, new int[]{101028,102020});
            config[81] = new HeroFriendConfig(81, "曹家千里驹", 2, new int[]{102017,102026,102033});
            config[82] = new HeroFriendConfig(82, "曹仁牛金", 1, new int[]{102024,102012});
            config[83] = new HeroFriendConfig(83, "制度奠基", 1, new int[]{102027,102003});
            config[84] = new HeroFriendConfig(84, "钟氏父子", 3, new int[]{102037,105005});
            config[85] = new HeroFriendConfig(85, "江东屏障", 2, new int[]{103015,103006});
            config[86] = new HeroFriendConfig(86, "毒士枭雄", 3, new int[]{104007,100004});
            config[87] = new HeroFriendConfig(87, "陈氏父子", 2, new int[]{105006,102027});
            config[88] = new HeroFriendConfig(88, "卢门同窗", 1, new int[]{110001,100001});
            config[89] = new HeroFriendConfig(89, "白马白卫", 1, new int[]{110001,101001});
            config[90] = new HeroFriendConfig(90, "东吴栋梁", 1, new int[]{103027,103026,103038});
            config[91] = new HeroFriendConfig(91, "杀父之仇", 2, new int[]{103003,103020});
            config[92] = new HeroFriendConfig(92, "文武双全", 2, new int[]{104002,104003});
            config[93] = new HeroFriendConfig(93, "西凉军", 2, new int[]{101003,101011,102014});
            config[94] = new HeroFriendConfig(94, "樊城防御", 1, new int[]{102011,102014});
            config[95] = new HeroFriendConfig(95, "虎侯之威", 1, new int[]{102005,101003});
            config[96] = new HeroFriendConfig(96, "讨董联盟", 1, new int[]{100002,103001,100006,110004,110005});
            config[97] = new HeroFriendConfig(97, "破吴之役", 1, new int[]{102016,102009,102036});
            config[98] = new HeroFriendConfig(98, "吴下阿蒙", 2, new int[]{103007,103010});
            config[99] = new HeroFriendConfig(99, "白衣渡江", 2, new int[]{103021,103010,103035});
            config[100] = new HeroFriendConfig(100, "吴郡朱氏", 3, new int[]{103022,103035,103015});
            config[101] = new HeroFriendConfig(101, "华佗治病", 2, new int[]{100002,110003});
            config[102] = new HeroFriendConfig(102, "刮骨疗毒", 2, new int[]{101005,110003});
            config[103] = new HeroFriendConfig(103, "文帝潜邸", 2, new int[]{102018,102029,102031,102027,104003});
            config[104] = new HeroFriendConfig(104, "西凉双雄", 2, new int[]{110005,110010});
            config[105] = new HeroFriendConfig(105, "羊家智囊", 2, new int[]{105004,105009});
            config[106] = new HeroFriendConfig(106, "才貌兼修", 2, new int[]{102030,110012});
            config[107] = new HeroFriendConfig(107, "年少多才", 2, new int[]{103031,102039,105005});
            config[108] = new HeroFriendConfig(108, "盟友抗曹", 2, new int[]{110004,104001,103002});

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
