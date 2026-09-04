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
        /// <summary>
        ///关联技能（>0 表示特殊连锁：激活该技能且不再加属性；=0 表示普通连线加属性）
        /// </summary>
        public string SkillId;
        /// <summary>
        ///连线颜色（特殊连锁的拉线颜色，HTML色值如 #FF0000）
        /// </summary>
        public string LineColor;


        public HeroFriendConfig(int Id, string Name, int Level, int[] Heros, string SkillId = "", string LineColor = "")
        {
            this.Id = Id;
            this.Name = Name;
            this.Level = Level;
            this.Heros = Heros;
            this.SkillId = SkillId;
            this.LineColor = LineColor;

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
            RebuildIndex();
        }
        public class FieldMetaInfo
        {
            public string fieldName;
            public string fieldType;
            public int fieldWidth;
            public string fieldRule;
            public bool fieldIndex;
            public FieldMetaInfo(string name, string type, int width = 0, string rule = "", bool index = false)
            {
                fieldName = name;
                fieldType = type;
                fieldWidth = width;
                fieldRule = rule;
                fieldIndex = index;
            }
        }

        public class CellMeta
        {
            public int row;
            public int col;
            public int? foreColor;
            public int? backColor;
            public CellMeta(int row, int col, int? foreColor, int? backColor)
            {
                this.row = row;
                this.col = col;
                this.foreColor = foreColor;
                this.backColor = backColor;
            }
        }

        private static Dictionary<string, FieldMetaInfo> fieldMeta = new Dictionary<string, FieldMetaInfo>()
        {
            {"Id", new FieldMetaInfo("序列", "int", 60)},
            {"Name", new FieldMetaInfo("名字", "string", 0)},
            {"Level", new FieldMetaInfo("支援级别（3最高，1最低（比如共事））", "int", 60)},
            {"Heros", new FieldMetaInfo("英雄列表，最多5人，一般2-3人较多", "int[]", 794)},
            {"SkillId", new FieldMetaInfo("关联技能缩写", "string", 60)},
            {"LineColor", new FieldMetaInfo("连线颜色", "string", 0)},
        };

        public static Dictionary<string, FieldMetaInfo> FieldMeta { get { return fieldMeta; } }

        private static List<CellMeta> cellMeta = new List<CellMeta>();
        public static List<CellMeta> CellMetas { get { return cellMeta; } }

        public static void Load()
        {
            config.Clear();
            config[1] = new HeroFriendConfig(1, "桃源结义", 3, new int[]{100001,101002,101005}, "", "");
            config[2] = new HeroFriendConfig(2, "五虎上将", 2, new int[]{101001,101002,101003,101005,101008}, "", "");
            config[3] = new HeroFriendConfig(3, "师徒护汉", 3, new int[]{101004,101010}, "", "");
            config[4] = new HeroFriendConfig(4, "卧龙凤雏", 2, new int[]{101004,101012}, "", "");
            config[5] = new HeroFriendConfig(5, "马腾父子", 3, new int[]{101003,101011,110005}, "", "");
            config[10] = new HeroFriendConfig(10, "长沙守将", 1, new int[]{101007,101008}, "", "");
            config[12] = new HeroFriendConfig(12, "孔明元直", 2, new int[]{101006,101004}, "", "");
            config[13] = new HeroFriendConfig(13, "曹魏宗室核心", 2, new int[]{102002,102006,102012,102013}, "", "");
            config[15] = new HeroFriendConfig(15, "五子良将", 2, new int[]{102004,102008,102009,102011,102015}, "", "");
            config[16] = new HeroFriendConfig(16, "颍川谋士团", 1, new int[]{102001,102003,102010,102017}, "", "");
            config[17] = new HeroFriendConfig(17, "夏侯双雄", 2, new int[]{102002,102006}, "", "");
            config[18] = new HeroFriendConfig(18, "司马张郃", 1, new int[]{102016,102008}, "", "");
            config[19] = new HeroFriendConfig(19, "江东基业", 3, new int[]{103002,103001,100003}, "", "");
            config[20] = new HeroFriendConfig(20, "江表老臣", 1, new int[]{103002,103001,103005,103022}, "", "");
            config[21] = new HeroFriendConfig(21, "江东虎臣", 1, new int[]{103009,103006}, "", "");
            config[22] = new HeroFriendConfig(22, "都督传承", 2, new int[]{103008,103010,103011}, "", "");
            config[23] = new HeroFriendConfig(23, "瑜策", 3, new int[]{103001,103008}, "", "");
            config[24] = new HeroFriendConfig(24, "二乔", 3, new int[]{103016,103017}, "", "");
            config[25] = new HeroFriendConfig(25, "帆箭", 2, new int[]{103003,103004}, "", "");
            config[27] = new HeroFriendConfig(27, "吕布帐下", 1, new int[]{104001,104006,102004}, "", "");
            config[28] = new HeroFriendConfig(28, "河北庭柱", 2, new int[]{106006,106001,106002,102008}, "", "");
            config[29] = new HeroFriendConfig(29, "黄巾之乱", 3, new int[]{110007,110008,110009}, "", "");
            config[30] = new HeroFriendConfig(30, "司马家族", 3, new int[]{105002,105003,100005,102016}, "", "");
            config[31] = new HeroFriendConfig(31, "西晋名将", 1, new int[]{105001,105005}, "", "");
            config[32] = new HeroFriendConfig(32, "英雄美人", 3, new int[]{104001,104004}, "", "");
            config[33] = new HeroFriendConfig(33, "英雄相惜", 2, new int[]{101005,102004}, "", "");
            config[34] = new HeroFriendConfig(34, "汉寿之恩", 1, new int[]{100002,101005}, "", "");
            config[35] = new HeroFriendConfig(35, "总角之好", 2, new int[]{103007,103008}, "", "");
            config[36] = new HeroFriendConfig(36, "护主之功", 2, new int[]{100003,103006}, "", "");
            config[37] = new HeroFriendConfig(37, "护主双戟", 2, new int[]{100002,102007,102005}, "", "");
            config[38] = new HeroFriendConfig(38, "仁主良将", 2, new int[]{100001,101001}, "", "");
            config[39] = new HeroFriendConfig(39, "祁山斗智", 1, new int[]{101004,102016}, "", "");
            config[40] = new HeroFriendConfig(40, "神亭酣战", 1, new int[]{103001,103004}, "", "");
            config[41] = new HeroFriendConfig(41, "定军扬威", 1, new int[]{101020,101008}, "", "");
            config[42] = new HeroFriendConfig(42, "北征帷幄", 1, new int[]{102001,102004}, "", "");
            config[45] = new HeroFriendConfig(45, "苦肉献策", 1, new int[]{103008,103005}, "", "");
            config[46] = new HeroFriendConfig(46, "有勇有谋", 2, new int[]{104001,104008}, "", "");
            config[47] = new HeroFriendConfig(47, "官渡奇谋", 1, new int[]{106005,100002}, "", "");
            config[48] = new HeroFriendConfig(48, "灭蜀之功", 1, new int[]{105003,105001}, "", "");
            config[49] = new HeroFriendConfig(49, "舟楫平吴", 1, new int[]{105008,105007,100005}, "", "");
            config[50] = new HeroFriendConfig(50, "合肥同心", 1, new int[]{102004,102021}, "", "");
            config[51] = new HeroFriendConfig(51, "凤仪亭", 1, new int[]{100004,104001}, "", "");
            config[53] = new HeroFriendConfig(53, "河北智囊", 2, new int[]{106003,106007,106008}, "", "");
            config[54] = new HeroFriendConfig(54, "英雄美人", 3, new int[]{103001,103016}, "", "");
            config[55] = new HeroFriendConfig(55, "顾曲周郎", 3, new int[]{103008,103017}, "", "");
            config[56] = new HeroFriendConfig(56, "孙氏兄妹", 3, new int[]{100003,103014}, "", "");
            config[57] = new HeroFriendConfig(57, "蜀汉同盟", 2, new int[]{100001,103014}, "", "");
            config[58] = new HeroFriendConfig(58, "荀氏叔侄", 3, new int[]{102003,102010}, "", "");
            config[60] = new HeroFriendConfig(60, "襄阳马氏", 2, new int[]{101019,101018}, "", "");
            config[62] = new HeroFriendConfig(62, "长坂护主", 3, new int[]{101021,101001}, "", "");
            config[63] = new HeroFriendConfig(63, "托孤大臣", 2, new int[]{101021,101015,101017}, "", "");
            config[66] = new HeroFriendConfig(66, "二士争功", 2, new int[]{101010,105005}, "", "");
            config[68] = new HeroFriendConfig(68, "父子君臣", 3, new int[]{100001,101021}, "", "");
            config[69] = new HeroFriendConfig(69, "瑜亮", 2, new int[]{103008,101004}, "", "");
            config[70] = new HeroFriendConfig(70, "川蜀集团", 1, new int[]{101020,101014,101013,110002}, "", "");
            config[72] = new HeroFriendConfig(72, "东吴砥柱", 2, new int[]{103021,103018}, "", "");
            config[74] = new HeroFriendConfig(74, "合肥之盾", 2, new int[]{102021,102015}, "", "");
            config[75] = new HeroFriendConfig(75, "陷阵之锋", 3, new int[]{104006,102004}, "", "");
            config[77] = new HeroFriendConfig(77, "扛刀牵马", 2, new int[]{101005,101009}, "", "");
            config[83] = new HeroFriendConfig(83, "制度奠基", 1, new int[]{102020,102003}, "", "");
            config[85] = new HeroFriendConfig(85, "江东屏障", 2, new int[]{103015,103006}, "", "");
            config[86] = new HeroFriendConfig(86, "毒士枭雄", 3, new int[]{104007,100004}, "", "");
            config[87] = new HeroFriendConfig(87, "陈氏父子", 2, new int[]{105006,102020}, "", "");
            config[91] = new HeroFriendConfig(91, "杀父之仇", 2, new int[]{103003,103019}, "", "");
            config[92] = new HeroFriendConfig(92, "文武双全", 2, new int[]{104002,104003}, "", "");
            config[93] = new HeroFriendConfig(93, "西凉军", 2, new int[]{101003,101011,102014}, "", "");
            config[94] = new HeroFriendConfig(94, "樊城防御", 1, new int[]{102011,102014}, "", "");
            config[95] = new HeroFriendConfig(95, "虎侯之威", 1, new int[]{102005,101003}, "", "");
            config[96] = new HeroFriendConfig(96, "讨董联盟", 1, new int[]{100002,103002,100006,110005}, "", "");
            config[98] = new HeroFriendConfig(98, "吴下阿蒙", 2, new int[]{103007,103010}, "", "");
            config[99] = new HeroFriendConfig(99, "白衣渡江", 2, new int[]{103020,103010}, "", "");
            config[101] = new HeroFriendConfig(101, "华佗治病", 2, new int[]{100002,110003}, "", "");
            config[102] = new HeroFriendConfig(102, "刮骨疗毒", 2, new int[]{101005,110003}, "", "");
            config[103] = new HeroFriendConfig(103, "文帝潜邸", 2, new int[]{102016,102022,102020,104003}, "", "");

            RebuildIndex();

        }

        private static void RebuildIndex()
        {
            foreach (var kv in config)
            {
            }
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
