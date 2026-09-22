using System;
using System.Collections;
using System.Collections.Generic;

namespace CommonConfig
{
    public class HeroConfig
    {
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
            {"Lv", new FieldMetaInfo("等级", "int", 60)},
            {"Job", new FieldMetaInfo("职业", "string", 0)},
            {"Quality", new FieldMetaInfo("品质：1普通 2优秀 3精良 4史诗", "int", 60, "4:#FF00FF,3:#3333FF,2:#33CC33,1:#666666")},
            {"Atk", new FieldMetaInfo("攻击（0=职业基准，>0与职业相加）", "int", 60, "95-100:#FF9900,90-94:#995500,80-89:#33CC33")},
            {"Ap", new FieldMetaInfo("法术强度（0=职业基准，>0与职业相加）", "int", 60, "95-100:#FF9900,90-94:#995500,80-89:#33CC33")},
            {"Hp", new FieldMetaInfo("生命", "int", 60)},
            {"AtkP", new FieldMetaInfo("攻击成长百分比（每星，如80=每星+80%）", "int", 60)},
            {"HpP", new FieldMetaInfo("生命成长百分比（每星）", "int", 60)},
            {"AtkSpeed", new FieldMetaInfo("攻速（0=职业基准，>0与职业相加；30=每秒攻击1次，攻速20=1.5秒/次，15=2秒/次）", "int", 83)},
            {"HpRegen", new FieldMetaInfo("生命回复/秒（0=职业基准，非0=相对职业基准±%）", "int", 60)},
            {"MpRegen", new FieldMetaInfo("魔法回复/秒（0=职业基准，非0=相对职业基准±%）", "int", 60)},
            {"Armor", new FieldMetaInfo("护甲（0=职业基准，>0与职业相加）", "int", 60)},
            {"MagicRes", new FieldMetaInfo("魔抗（0=职业基准，>0与职业相加）", "int", 79)},
            {"Side", new FieldMetaInfo("阵营", "int", 60)},
            {"Price", new FieldMetaInfo("价格（配表数据，2-10）", "int", 60, "9-10:#FF9900,7-8:#995500,5-6:#33CC33,3-4:#3333CC")},
            {"MoveSpeed", new FieldMetaInfo("移动速度（0=职业默认，>0与职业相加）", "int", 81)},
            {"Range", new FieldMetaInfo("攻击距离（0=职业默认，>0与职业相加）", "int", 60)},
            {"Skill1", new FieldMetaInfo("技能", "string", 0)},
            {"Icon", new FieldMetaInfo("背景图", "string", 0)},
        };

        public static Dictionary<string, FieldMetaInfo> FieldMeta { get { return fieldMeta; } }

        private static List<CellMeta> cellMeta = new List<CellMeta>();
        public static List<CellMeta> CellMetas { get { return cellMeta; } }

        /// <summary>
        ///序列
        /// </summary>
        public int Id;
        /// <summary>
        ///名字
        /// </summary>
        public string Name;
        /// <summary>
        ///等级
        /// </summary>
        public int Lv;
        /// <summary>
        ///职业
        /// </summary>
        public string Job;
        /// <summary>
        ///品质：1普通 2优秀 3精良 4史诗
        /// </summary>
        public int Quality;
        /// <summary>
        ///攻击（0=职业基准，>0与职业相加）
        /// </summary>
        public int Atk;
        /// <summary>
        ///法术强度（0=职业基准，>0与职业相加）
        /// </summary>
        public int Ap;
        /// <summary>
        ///生命
        /// </summary>
        public int Hp;
        /// <summary>
        ///攻击成长百分比（每星，如80=每星+80%）
        /// </summary>
        public int AtkP;
        /// <summary>
        ///生命成长百分比（每星）
        /// </summary>
        public int HpP;
        /// <summary>
        ///攻速（0=职业基准，>0与职业相加；20=每秒攻击1次，40=每秒2次）
        /// </summary>
        public int AtkSpeed;
        /// <summary>
        ///生命回复/秒（0=职业基准，非0=相对职业基准±%）
        /// </summary>
        public int HpRegen;
        /// <summary>
        ///魔法回复/秒（0=职业基准，非0=相对职业基准±%）
        /// </summary>
        public int MpRegen;
        /// <summary>
        ///护甲
        /// </summary>
        public int Armor;
        /// <summary>
        ///魔抗（0=职业基准，>0与职业相加）
        /// </summary>
        public int MagicRes;
        /// <summary>
        ///阵营
        /// </summary>
        public int Side;
        /// <summary>
        ///价格（配表数据，2-10）
        /// </summary>
        public int Price;
        /// <summary>
        ///移动速度（0=职业默认，>0与职业相加）
        /// </summary>
        public int MoveSpeed;
        /// <summary>
        ///攻击距离（0=职业默认，>0与职业相加）
        /// </summary>
        public int Range;
        /// <summary>
        ///技能
        /// </summary>
        public string Skill1;
        /// <summary>
        ///背景图
        /// </summary>
        public string Icon;


        public HeroConfig(int Id, string Name, int Lv, string Job, int Quality, int Atk, int Ap, int Hp, int AtkP, int HpP, int AtkSpeed, int HpRegen, int MpRegen, int Armor, int MagicRes, int Side, int Price, int MoveSpeed, int Range, string Skill1, string Icon)
        {
            this.Id = Id;
            this.Name = Name;
            this.Lv = Lv;
            this.Job = Job;
            this.Quality = Quality;
            this.Atk = Atk;
            this.Ap = Ap;
            this.Hp = Hp;
            this.AtkP = AtkP;
            this.HpP = HpP;
            this.AtkSpeed = AtkSpeed;
            this.HpRegen = HpRegen;
            this.MpRegen = MpRegen;
            this.Armor = Armor;
            this.MagicRes = MagicRes;
            this.Side = Side;
            this.Price = Price;
            this.MoveSpeed = MoveSpeed;
            this.Range = Range;
            this.Skill1 = Skill1;
            this.Icon = Icon;
        }

        public HeroConfig() { }

        private static Dictionary<int, HeroConfig> config = new Dictionary<int, HeroConfig>();
        public static Dictionary<int, HeroConfig>.ValueCollection ConfigList
        {
            get { return config.Values; }
        }

        public static void Refresh(Dictionary<int, HeroConfig> dict)
        {
            config.Clear();
            config = dict;
            RebuildIndex();
        }

        public static void Load()
        {
            config.Clear();
            config[100001] = new HeroConfig(100001, "刘备", 1, "王", 3, 0, 0, 0, 100, 100, 0, 0, 0, 0, 0, 1, 6, 0, 0, "强", "liubei");
            config[100002] = new HeroConfig(100002, "曹操", 1, "王", 4, 0, 0, 0, 100, 100, 0, 0, 0, 0, 0, 2, 8, 0, 0, "强", "caocao");
            config[100003] = new HeroConfig(100003, "孙权", 1, "王", 3, 0, 0, 0, 100, 100, 0, 0, 0, 0, 0, 3, 6, 0, 0, "强", "sunquan");
            config[100004] = new HeroConfig(100004, "董卓", 1, "王", 3, 0, 0, 0, 100, 100, 0, 0, 0, 0, 0, 4, 6, 0, 0, "强", "dongzhuo");
            config[100005] = new HeroConfig(100005, "司马炎", 1, "王", 2, 0, 0, 0, 100, 100, 0, 0, 0, 0, 0, 5, 4, 0, 0, "强", "simayan");
            config[100006] = new HeroConfig(100006, "袁绍", 1, "王", 2, 0, 0, 0, 100, 100, 0, 0, 0, 0, 0, 6, 4, 0, 0, "强", "yuanshao");
            config[101001] = new HeroConfig(101001, "赵云", 1, "士", 4, 0, 0, 0, 100, 100, 0, 0, 0, 0, 0, 1, 8, 0, 0, "强", "zhaoyun");
            config[101002] = new HeroConfig(101002, "张飞", 1, "枪", 4, 0, 0, 0, 100, 100, 0, 0, 0, 0, 0, 1, 8, 0, 0, "张飞", "zhangfei");
            config[101003] = new HeroConfig(101003, "马超", 1, "马", 4, 0, 0, 0, 100, 100, 0, 0, 0, 0, 0, 1, 8, 0, 0, "强", "machao");
            config[101004] = new HeroConfig(101004, "诸葛亮", 1, "工", 4, 0, 0, 0, 100, 100, 0, 0, 0, 0, 0, 1, 8, 0, 0, "强", "zhugeliang");
            config[101005] = new HeroConfig(101005, "关羽", 1, "车", 4, 0, 0, 0, 100, 100, 0, 0, 0, 0, 0, 1, 8, 0, 0, "强", "guanyu");
            config[101006] = new HeroConfig(101006, "徐庶", 1, "炮", 3, 0, 0, 0, 100, 100, 0, 0, 0, 0, 0, 1, 6, 0, 0, "强", "xusu");
            config[101007] = new HeroConfig(101007, "魏延", 1, "戟", 3, 0, 0, 0, 100, 100, 0, 0, 0, 0, 0, 1, 6, 0, 0, "强", "weiyan");
            config[101008] = new HeroConfig(101008, "黄忠", 1, "弓", 3, 0, 0, 0, 100, 100, 0, 0, 0, 0, 0, 1, 6, 0, 0, "黄忠", "huangzhong");
            config[101009] = new HeroConfig(101009, "周仓", 1, "盾", 1, 0, 0, 0, 100, 100, 0, 0, 0, 0, 0, 1, 2, 0, 0, "强", "zhoucang");
            config[101010] = new HeroConfig(101010, "姜维", 1, "车", 4, 0, 0, 0, 100, 100, 0, 0, 0, 0, 0, 1, 8, 0, 0, "强", "jiangwei");
            config[101011] = new HeroConfig(101011, "马岱", 1, "弩", 2, 0, 0, 0, 100, 100, 0, 0, 0, 0, 0, 1, 4, 0, 0, "强", "madai");
            config[101012] = new HeroConfig(101012, "庞统", 1, "棋", 4, 0, 0, 0, 100, 100, 0, 0, 0, 0, 0, 1, 8, 0, 0, "庞统", "pangtong");
            config[101013] = new HeroConfig(101013, "李严", 1, "士", 2, 0, 0, 0, 100, 100, 0, 0, 0, 0, 0, 1, 4, 0, 0, "强", "liyan");
            config[101014] = new HeroConfig(101014, "张松", 1, "扇", 1, 0, 0, 0, 100, 100, 0, 0, 0, 0, 0, 1, 2, 0, 0, "强", "zhangsong");
            config[101015] = new HeroConfig(101015, "蒋琬", 1, "相", 1, 0, 0, 0, 100, 100, 0, 0, 0, 0, 0, 1, 2, 0, 0, "强", "jiangwan");
            config[101016] = new HeroConfig(101016, "孙乾", 1, "鼓", 1, 0, 0, 0, 100, 100, 0, 0, 0, 0, 0, 1, 2, 0, 0, "强", "sunqian");
            config[101017] = new HeroConfig(101017, "费祎", 1, "鼓", 2, 0, 0, 0, 100, 100, 0, 0, 0, 0, 0, 1, 4, 0, 0, "强", "feiyi");
            config[101018] = new HeroConfig(101018, "马谡", 1, "枪", 1, 0, 0, 0, 100, 100, 0, 0, 0, 0, 0, 1, 2, 0, 0, "强", "masu");
            config[101019] = new HeroConfig(101019, "马良", 1, "相", 2, 0, 0, 0, 100, 100, 0, 0, 0, 0, 0, 1, 4, 0, 0, "强", "maliang");
            config[101020] = new HeroConfig(101020, "法正", 1, "棋", 3, 0, 0, 0, 100, 100, 0, 0, 0, 0, 0, 1, 6, 0, 0, "强", "fazheng");
            config[101021] = new HeroConfig(101021, "刘禅", 1, "医", 1, 0, 0, 0, 100, 100, 0, 0, 0, 0, 0, 1, 2, 0, 0, "强", "liushan");
            config[101022] = new HeroConfig(101022, "严颜", 1, "盾", 2, 0, 0, 0, 100, 100, 0, 0, 0, 0, 0, 1, 4, 0, 0, "强", "yanyan");
            config[101023] = new HeroConfig(101023, "黄月英", 1, "工", 2, 0, 0, 0, 100, 100, 0, 0, 0, 0, 0, 1, 4, 0, 0, "强", "huangyueying");
            config[102001] = new HeroConfig(102001, "郭嘉", 1, "棋", 4, 0, 0, 0, 100, 100, 0, 0, 0, 0, 0, 2, 8, 0, 0, "强", "guojia");
            config[102002] = new HeroConfig(102002, "夏侯惇", 1, "车", 3, 0, 0, 0, 100, 100, 0, 0, 0, 0, 0, 2, 6, 0, 0, "强", "xiahoudun");
            config[102003] = new HeroConfig(102003, "荀彧", 1, "相", 4, 0, 0, 0, 100, 100, 0, 0, 0, 0, 0, 2, 8, 0, 0, "强", "xunyu");
            config[102004] = new HeroConfig(102004, "张辽", 1, "枪", 4, 0, 0, 0, 100, 100, 0, 0, 0, 0, 0, 2, 8, 0, 0, "强", "zhangliao");
            config[102005] = new HeroConfig(102005, "许褚", 1, "锤", 4, 0, 0, 0, 100, 100, 0, 0, 0, 0, 0, 2, 8, 0, 0, "许褚", "xuchu");
            config[102006] = new HeroConfig(102006, "夏侯渊", 1, "弓", 3, 0, 0, 0, 100, 100, 0, 0, 0, 0, 0, 2, 6, 0, 0, "强", "xiahouyuan");
            config[102007] = new HeroConfig(102007, "典韦", 1, "戟", 4, 0, 0, 0, 100, 100, 0, 0, 0, 0, 0, 2, 8, 0, 0, "强", "dianwei");
            config[102008] = new HeroConfig(102008, "张郃", 1, "炮", 4, 0, 0, 0, 100, 100, 0, 0, 0, 0, 0, 2, 8, 0, 0, "强", "zhanghe");
            config[102009] = new HeroConfig(102009, "徐晃", 1, "弩", 3, 0, 0, 0, 100, 100, 0, 0, 0, 0, 0, 2, 6, 0, 0, "徐晃", "xuhuang");
            config[102010] = new HeroConfig(102010, "荀攸", 1, "棋", 3, 0, 0, 0, 100, 100, 0, 0, 0, 0, 0, 2, 6, 0, 0, "强", "xunyou");
            config[102011] = new HeroConfig(102011, "于禁", 1, "枪", 2, 0, 0, 0, 100, 100, 0, 0, 0, 0, 0, 2, 4, 0, 0, "强", "yujin");
            config[102012] = new HeroConfig(102012, "曹仁", 1, "盾", 3, 0, 0, 0, 100, 100, 0, 0, 0, 0, 0, 2, 6, 0, 0, "强", "caoren");
            config[102013] = new HeroConfig(102013, "曹洪", 1, "锤", 2, 0, 0, 0, 100, 100, 0, 0, 0, 0, 0, 2, 4, 0, 0, "强", "caohong");
            config[102014] = new HeroConfig(102014, "庞德", 1, "士", 3, 0, 0, 0, 100, 100, 0, 0, 0, 0, 0, 2, 6, 0, 0, "强", "pangde");
            config[102015] = new HeroConfig(102015, "乐进", 1, "车", 1, 0, 0, 0, 100, 100, 0, 0, 0, 0, 0, 2, 2, 0, 0, "强", "lejin");
            config[102016] = new HeroConfig(102016, "司马懿", 1, "扇", 4, 0, 0, 0, 100, 100, 0, 0, 0, 0, 0, 2, 8, 0, 0, "强", "simayi");
            config[102017] = new HeroConfig(102017, "程昱", 1, "扇", 2, 0, 0, 0, 100, 100, 0, 0, 0, 0, 0, 2, 4, 0, 0, "强", "chengyu");
            config[102018] = new HeroConfig(102018, "文鸯", 1, "弩", 1, 0, 0, 0, 100, 100, 0, 0, 0, 0, 0, 2, 2, 0, 0, "强", "wenyuan");
            config[102019] = new HeroConfig(102019, "曹真", 1, "戟", 1, 0, 0, 0, 100, 100, 0, 0, 0, 0, 0, 2, 2, 0, 0, "强", "caozhen");
            config[102020] = new HeroConfig(102020, "陈群", 1, "工", 1, 0, 0, 0, 100, 100, 0, 0, 0, 0, 0, 2, 2, 0, 0, "强", "chenqun");
            config[102021] = new HeroConfig(102021, "李典", 1, "士", 2, 0, 0, 0, 100, 100, 0, 0, 0, 0, 0, 2, 4, 0, 0, "强", "lidian");
            config[102022] = new HeroConfig(102022, "刘晔", 1, "工", 2, 0, 0, 0, 100, 100, 0, 0, 0, 0, 0, 2, 4, 0, 0, "强", "liuye");
            config[102023] = new HeroConfig(102023, "曹彰", 1, "马", 3, 0, 0, 0, 100, 100, 0, 0, 0, 0, 0, 2, 6, 0, 0, "强", "caozhang");
            config[102024] = new HeroConfig(102024, "蔡文姬", 1, "琴", 2, 0, 0, 0, 100, 100, 0, 0, 0, 0, 0, 2, 4, 0, 0, "强", "caiyan");
            config[102025] = new HeroConfig(102025, "甄宓", 1, "琴", 1, 0, 0, 0, 100, 100, 0, 0, 0, 0, 0, 2, 2, 0, 0, "强", "zhenshi");
            config[103001] = new HeroConfig(103001, "孙策", 1, "戟", 4, 0, 0, 0, 100, 100, 0, 0, 0, 0, 0, 3, 8, 0, 0, "强", "sunce");
            config[103002] = new HeroConfig(103002, "孙坚", 1, "枪", 3, 0, 0, 0, 100, 100, 0, 0, 0, 0, 0, 3, 6, 0, 0, "强", "sunjian");
            config[103003] = new HeroConfig(103003, "甘宁", 1, "弩", 4, 0, 0, 0, 100, 100, 0, 0, 0, 0, 0, 3, 8, 0, 0, "强", "ganning");
            config[103004] = new HeroConfig(103004, "太史慈", 1, "弓", 4, 0, 0, 0, 100, 100, 0, 0, 0, 0, 0, 3, 8, 0, 0, "强", "taishici");
            config[103005] = new HeroConfig(103005, "黄盖", 1, "士", 2, 0, 0, 0, 100, 100, 0, 0, 0, 0, 0, 3, 4, 0, 0, "强", "huanggai");
            config[103006] = new HeroConfig(103006, "周泰", 1, "锤", 3, 0, 0, 0, 100, 100, 0, 0, 0, 0, 0, 3, 6, 0, 0, "强", "zhoutai");
            config[103007] = new HeroConfig(103007, "鲁肃", 1, "鼓", 4, 0, 0, 0, 100, 100, 0, 0, 0, 0, 0, 3, 8, 0, 0, "鲁肃", "lusu");
            config[103008] = new HeroConfig(103008, "周瑜", 1, "扇", 4, 0, 0, 0, 100, 100, 0, 0, 0, 0, 0, 3, 8, 0, 0, "周瑜", "zhouyu");
            config[103009] = new HeroConfig(103009, "蒋钦", 1, "戟", 2, 0, 0, 0, 100, 100, 0, 0, 0, 0, 0, 3, 4, 0, 0, "强", "jiangqing");
            config[103010] = new HeroConfig(103010, "吕蒙", 1, "盾", 4, 0, 0, 0, 100, 100, 0, 0, 0, 0, 0, 3, 8, 0, 0, "强", "lvmeng");
            config[103011] = new HeroConfig(103011, "陆逊", 1, "炮", 4, 0, 0, 0, 100, 100, 0, 0, 0, 0, 0, 3, 8, 0, 0, "强", "luxun");
            config[103012] = new HeroConfig(103012, "张昭", 1, "相", 3, 0, 0, 0, 100, 100, 0, 0, 0, 0, 0, 3, 6, 0, 0, "强", "zhangzhao");
            config[103013] = new HeroConfig(103013, "诸葛瑾", 1, "鼓", 3, 0, 0, 0, 100, 100, 0, 0, 0, 0, 0, 3, 6, 0, 0, "强", "zhugejin");
            config[103014] = new HeroConfig(103014, "孙尚香", 1, "弩", 3, 0, 0, 0, 100, 100, 0, 0, 0, 0, 0, 3, 6, 0, 0, "强", "sunshangxiang");
            config[103015] = new HeroConfig(103015, "朱桓", 1, "马", 2, 0, 0, 0, 100, 100, 0, 0, 0, 0, 0, 3, 4, 0, 0, "强", "zhuhuan");
            config[103016] = new HeroConfig(103016, "大乔", 1, "琴", 3, 0, 0, 0, 100, 100, 0, 0, 0, 0, 0, 3, 6, 0, 0, "强", "daqiao");
            config[103017] = new HeroConfig(103017, "小乔", 1, "琴", 2, 0, 0, 0, 100, 100, 0, 0, 0, 0, 0, 3, 4, 0, 0, "强", "xiaoqiao");
            config[103018] = new HeroConfig(103018, "丁奉", 1, "炮", 1, 0, 0, 0, 100, 100, 0, 0, 0, 0, 0, 3, 2, 0, 0, "强", "dingfeng");
            config[103019] = new HeroConfig(103019, "凌统", 1, "弓", 1, 0, 0, 0, 100, 100, 0, 0, 0, 0, 0, 3, 2, 0, 0, "强", "lingtong");
            config[103020] = new HeroConfig(103020, "潘璋", 1, "锤", 1, 0, 0, 0, 100, 100, 0, 0, 0, 0, 0, 3, 2, 0, 0, "强", "panzhang");
            config[103021] = new HeroConfig(103021, "徐盛", 1, "盾", 1, 0, 0, 0, 100, 100, 0, 0, 0, 0, 0, 3, 2, 0, 0, "强", "xusheng");
            config[103022] = new HeroConfig(103022, "程普", 1, "车", 2, 0, 0, 0, 100, 100, 0, 0, 0, 0, 0, 3, 4, 0, 0, "强", "chengpu");
            config[103023] = new HeroConfig(103023, "韩当", 1, "马", 2, 0, 0, 0, 100, 100, 0, 0, 0, 0, 0, 3, 4, 0, 0, "强", "handang");
            config[104001] = new HeroConfig(104001, "吕布", 1, "马", 4, 0, 0, 0, 100, 100, 0, 0, 0, 0, 0, 4, 8, 0, 0, "吕布", "lvbu");
            config[104002] = new HeroConfig(104002, "华雄", 1, "马", 3, 0, 0, 0, 100, 100, 0, 0, 0, 0, 0, 4, 6, 0, 0, "强", "huaxiong");
            config[104003] = new HeroConfig(104003, "贾诩", 1, "相", 3, 0, 0, 0, 100, 100, 0, 0, 0, 0, 0, 4, 6, 0, 0, "强", "jiaxu");
            config[104004] = new HeroConfig(104004, "貂蝉", 1, "琴", 3, 0, 0, 0, 100, 100, 0, 0, 0, 0, 0, 4, 6, 0, 0, "强", "diaochan");
            config[104006] = new HeroConfig(104006, "高顺", 1, "炮", 2, 0, 0, 0, 100, 100, 0, 0, 0, 0, 0, 4, 4, 0, 0, "强", "gaoshun");
            config[104007] = new HeroConfig(104007, "李儒", 1, "扇", 3, 0, 0, 0, 100, 100, 0, 0, 0, 0, 0, 4, 6, 0, 0, "强", "liru");
            config[104008] = new HeroConfig(104008, "陈宫", 1, "棋", 3, 0, 0, 0, 100, 100, 0, 0, 0, 0, 0, 4, 6, 0, 0, "强", "chengong");
            config[104009] = new HeroConfig(104009, "张绣", 1, "枪", 3, 0, 0, 0, 100, 100, 0, 0, 0, 0, 0, 4, 6, 0, 0, "强", "zhangxiu");
            config[105001] = new HeroConfig(105001, "邓艾", 1, "盾", 4, 0, 0, 0, 100, 100, 0, 0, 0, 0, 0, 5, 8, 0, 0, "强", "dengai");
            config[105002] = new HeroConfig(105002, "司马师", 1, "鼓", 2, 0, 0, 0, 100, 100, 0, 0, 0, 0, 0, 5, 4, 0, 0, "强", "simashi");
            config[105003] = new HeroConfig(105003, "司马昭", 1, "扇", 3, 0, 0, 0, 100, 100, 0, 0, 0, 0, 0, 5, 6, 0, 0, "强", "simazhao");
            config[105005] = new HeroConfig(105005, "钟会", 1, "棋", 2, 0, 0, 0, 100, 100, 0, 0, 0, 0, 0, 5, 4, 0, 0, "强", "zhonghui");
            config[105006] = new HeroConfig(105006, "陈泰", 1, "马", 1, 0, 0, 0, 100, 100, 0, 0, 0, 0, 0, 5, 2, 0, 0, "强", "chentai");
            config[105007] = new HeroConfig(105007, "杜预", 1, "工", 1, 0, 0, 0, 100, 100, 0, 0, 0, 0, 0, 5, 2, 0, 0, "强", "duyu");
            config[105008] = new HeroConfig(105008, "王濬", 1, "炮", 2, 0, 0, 0, 100, 100, 0, 0, 0, 0, 0, 5, 4, 0, 0, "强", "wangrui");
            config[105009] = new HeroConfig(105009, "王双", 1, "锤", 2, 0, 0, 0, 100, 100, 0, 0, 0, 0, 0, 5, 4, 0, 0, "强", "wangshuang");
            config[106001] = new HeroConfig(106001, "颜良", 1, "车", 3, 0, 0, 0, 100, 100, 0, 0, 0, 0, 0, 6, 6, 0, 0, "强", "yanliang");
            config[106002] = new HeroConfig(106002, "文丑", 1, "戟", 3, 0, 0, 0, 100, 100, 0, 0, 0, 0, 0, 6, 6, 0, 0, "强", "wenchou");
            config[106003] = new HeroConfig(106003, "田丰", 1, "鼓", 3, 0, 0, 0, 100, 100, 0, 0, 0, 0, 0, 6, 6, 0, 0, "强", "tianfeng");
            config[106004] = new HeroConfig(106004, "鞠义", 1, "弩", 2, 0, 0, 0, 100, 100, 0, 0, 0, 0, 0, 6, 4, 0, 0, "强", "juyi");
            config[106005] = new HeroConfig(106005, "许攸", 1, "扇", 2, 0, 0, 0, 100, 100, 0, 0, 0, 0, 0, 6, 4, 0, 0, "强", "xuyou");
            config[106006] = new HeroConfig(106006, "高览", 1, "弓", 1, 0, 0, 0, 100, 100, 0, 0, 0, 0, 0, 6, 2, 0, 0, "强", "gaolan");
            config[106007] = new HeroConfig(106007, "沮授", 1, "相", 2, 0, 0, 0, 100, 100, 0, 0, 0, 0, 0, 6, 4, 0, 0, "强", "jushou");
            config[106008] = new HeroConfig(106008, "郭图", 1, "棋", 1, 0, 0, 0, 100, 100, 0, 0, 0, 0, 0, 6, 2, 0, 0, "强", "guotu");
            config[110002] = new HeroConfig(110002, "张任", 1, "弓", 2, 0, 0, 0, 100, 100, 0, 0, 0, 0, 0, 10, 4, 0, 0, "强", "zhangren");
            config[110003] = new HeroConfig(110003, "华佗", 1, "医", 2, 0, 0, 0, 100, 100, 0, 0, 0, 0, 0, 10, 4, 0, 0, "强", "huatuo");
            config[110005] = new HeroConfig(110005, "马腾", 1, "弩", 2, 0, 0, 0, 100, 100, 0, 0, 0, 0, 0, 10, 4, 0, 0, "强", "mateng");
            config[110006] = new HeroConfig(110006, "于吉", 1, "医", 2, 0, 0, 0, 100, 100, 0, 0, 0, 0, 0, 10, 4, 0, 0, "强", "yuji");
            config[110007] = new HeroConfig(110007, "张角", 1, "工", 3, 0, 0, 0, 100, 100, 0, 0, 0, 0, 0, 10, 6, 0, 0, "张角", "zhangjiao");
            config[110008] = new HeroConfig(110008, "张宝", 1, "医", 2, 0, 0, 0, 100, 100, 0, 0, 0, 0, 0, 10, 4, 0, 0, "强", "zhangbao2");
            config[110009] = new HeroConfig(110009, "张梁", 1, "士", 1, 0, 0, 0, 100, 100, 0, 0, 0, 0, 0, 10, 2, 0, 0, "强", "zhangliang");
            config[101024] = new HeroConfig(101024, "孟获", 1, "锤", 3, 0, 0, 0, 100, 100, 0, 0, 0, 0, 0, 1, 6, 0, 0, "强", "menghuo");
            config[110011] = new HeroConfig(110011, "左慈", 1, "医", 3, 0, 0, 0, 100, 100, 0, 0, 0, 0, 0, 10, 6, 0, 0, "强", "zuoci");

            RebuildIndex();

        }

        private static void RebuildIndex()
        {
            foreach (var kv in config)
            {
            }
        }

        public static HeroConfig GetConfig(int id)
        {
            HeroConfig data;
            if (config.TryGetValue(id, out data))
            {
                return data;
            }
            throw new NullReferenceException(string.Format("配置表HeroConfig不存在id={0}", id));
        }


        public static bool HasConfig(int id)
        {
            if (config.ContainsKey(id))
            {
                return true;
            }
            return false;
        }

        public static void Assign(int id, HeroConfig configData)
        {
            config[id] = configData; 
        }

        public static void Add(int id, HeroConfig configData)
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
