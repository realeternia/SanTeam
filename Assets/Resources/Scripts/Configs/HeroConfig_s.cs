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
            {"Id", new FieldMetaInfo("序列", "int", 0)},
            {"Name", new FieldMetaInfo("名字", "string", 0)},
            {"Lv", new FieldMetaInfo("等级", "int", 0)},
            {"Job", new FieldMetaInfo("职业", "string", 0)},
            {"Quality", new FieldMetaInfo("品质：1普通 2优秀 3精良 4史诗", "int", 0, "4:#FF00FF,3:#3333FF,2:#33CC33,1:#666666")},
            {"Atk", new FieldMetaInfo("攻击（0=职业基准，>0与职业相加）", "int", 0, "95-100:#FF9900,90-94:#995500,80-89:#33CC33")},
            {"AtkP", new FieldMetaInfo("攻击成长百分比（每星，如80=每星+80%）", "int", 0)},
            {"Ap", new FieldMetaInfo("法术强度（0=职业基准，>0与职业相加）", "int", 0, "95-100:#FF9900,90-94:#995500,80-89:#33CC33")},
            {"ApP", new FieldMetaInfo("法术强度成长百分比（每星）", "int", 0)},
            {"Might", new FieldMetaInfo("无双强度（0=职业基准，>0与职业相加）", "int", 0, "95-100:#FF9900,90-94:#995500,80-89:#33CC33")},
            {"MightP", new FieldMetaInfo("无双强度成长百分比（每星）", "int", 0)},
            {"Hp", new FieldMetaInfo("生命", "int", 0)},
            {"HpP", new FieldMetaInfo("生命成长百分比（每星）", "int", 0)},
            {"AtkSpeed", new FieldMetaInfo("攻速（0=职业基准，>0与职业相加；30=每秒攻击1次，攻速20=1.5秒/次，15=2秒/次）", "int", 0)},
            {"Armor", new FieldMetaInfo("护甲（0=职业基准，>0与职业相加）", "int", 0)},
            {"MagicRes", new FieldMetaInfo("魔抗（0=职业基准，>0与职业相加）", "int", 0)},
            {"Side", new FieldMetaInfo("阵营", "int", 0)},
            {"Price", new FieldMetaInfo("价格（配表数据，2-10）", "int", 0, "9-10:#FF9900,7-8:#995500,5-6:#33CC33,3-4:#3333CC")},
            {"MoveSpeed", new FieldMetaInfo("移动速度（0=职业默认，>0与职业相加）", "int", 0)},
            {"Range", new FieldMetaInfo("攻击距离（0=职业默认，>0与职业相加）", "int", 0)},
            {"MissileSpeed", new FieldMetaInfo("导弹速度", "int", 0)},
            {"MissileHight", new FieldMetaInfo("导弹高度", "float", 0)},
            {"Pos", new FieldMetaInfo("站位", "int", 0)},
            {"Skill1", new FieldMetaInfo("技能", "string", 0)},
            {"Skill2", new FieldMetaInfo("技能2", "string", 0)},
            {"Group", new FieldMetaInfo("团队", "string", 0)},
            {"HitEffect", new FieldMetaInfo("hit", "string", 0)},
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
        ///攻击成长百分比（每星，如80=每星+80%）
        /// </summary>
        public int AtkP;
        /// <summary>
        ///法术强度（0=职业基准，>0与职业相加）
        /// </summary>
        public int Ap;
        /// <summary>
        ///法术强度成长百分比（每星）
        /// </summary>
        public int ApP;
        /// <summary>
        ///无双强度（0=职业基准，>0与职业相加）
        /// </summary>
        public int Might;
        /// <summary>
        ///无双强度成长百分比（每星）
        /// </summary>
        public int MightP;
        /// <summary>
        ///生命
        /// </summary>
        public int Hp;
        /// <summary>
        ///生命成长百分比（每星）
        /// </summary>
        public int HpP;
        /// <summary>
        ///攻速（0=职业基准，>0与职业相加；20=每秒攻击1次，40=每秒2次）
        /// </summary>
        public int AtkSpeed;
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
        ///导弹速度
        /// </summary>
        public int MissileSpeed;
        /// <summary>
        ///导弹高度
        /// </summary>
        public float MissileHight;
        /// <summary>
        ///站位
        /// </summary>
        public int Pos;
        /// <summary>
        ///技能
        /// </summary>
        public string Skill1;
        /// <summary>
        ///技能2
        /// </summary>
        public string Skill2;
        /// <summary>
        ///团队
        /// </summary>
        public string Group;
        /// <summary>
        ///hit
        /// </summary>
        public string HitEffect;
        /// <summary>
        ///背景图
        /// </summary>
        public string Icon;


        public HeroConfig(int Id, string Name, int Lv, string Job, int Quality, int Atk, int AtkP, int Ap, int ApP, int Might, int MightP, int Hp, int HpP, int AtkSpeed, int Armor, int MagicRes, int Side, int Price, int MoveSpeed, int Range, int MissileSpeed, float MissileHight, int Pos, string Skill1, string Skill2, string Group, string HitEffect, string Icon)
        {
            this.Id = Id;
            this.Name = Name;
            this.Lv = Lv;
            this.Job = Job;
            this.Quality = Quality;
            this.Atk = Atk;
            this.AtkP = AtkP;
            this.Ap = Ap;
            this.ApP = ApP;
            this.Might = Might;
            this.MightP = MightP;
            this.Hp = Hp;
            this.HpP = HpP;
            this.AtkSpeed = AtkSpeed;
            this.Armor = Armor;
            this.MagicRes = MagicRes;
            this.Side = Side;
            this.Price = Price;
            this.MoveSpeed = MoveSpeed;
            this.Range = Range;
            this.MissileSpeed = MissileSpeed;
            this.MissileHight = MissileHight;
            this.Pos = Pos;
            this.Skill1 = Skill1;
            this.Skill2 = Skill2;
            this.Group = Group;
            this.HitEffect = HitEffect;
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
            config[100001] = new HeroConfig(100001, "刘备", 1, "帅", 3, 1, 80, 0, 80, 4, 80, 764, 80, 0, 0, 0, 1, 7, 0, 0, 0, 0f, 1, "仁", "", "core", "SwordHitYellowCritical", "liubei");
            config[100002] = new HeroConfig(100002, "曹操", 1, "帅", 4, 12, 80, 6, 80, 2, 80, 810, 80, 0, 10, 10, 2, 9, 0, 0, 0, 0f, 1, "识", "", "core", "SwordHitYellowCritical", "caocao");
            config[100003] = new HeroConfig(100003, "孙权", 1, "帅", 3, 3, 80, 3, 80, -1, 80, 774, 80, 0, 0, 0, 3, 7, 0, 0, 0, 0f, 1, "衡", "", "core", "SwordHitYellowCritical", "sunquan");
            config[100004] = new HeroConfig(100004, "董卓", 1, "帅", 3, 2, 80, -5, 80, 10, 80, 764, 80, 0, 0, 0, 4, 7, 0, 0, 0, 0f, 1, "", "", "core", "SwordHitYellowCritical", "dongzhuo");
            config[100005] = new HeroConfig(100005, "司马炎", 1, "帅", 3, -3, 80, 5, 80, -10, 80, 732, 80, 0, 0, 0, 5, 7, 0, 0, 0, 0f, 1, "", "", "core", "SwordHitYellowCritical", "simayan");
            config[100006] = new HeroConfig(100006, "袁绍", 1, "帅", 3, 0, 80, -4, 80, -6, 80, 695, 80, 0, 0, 0, 6, 7, 0, 0, 0, 0f, 1, "", "", "core", "SwordHitYellowCritical", "yuanshao");
            config[101001] = new HeroConfig(101001, "赵云", 1, "士", 4, 8, 80, 19, 80, 13, 80, 924, 80, 0, 0, 0, 1, 9, 0, 0, 0, 0f, 1, "镜", "羽", "def", "SwordHitWhiteCritical", "zhaoyun");
            config[101002] = new HeroConfig(101002, "张飞", 1, "枪", 4, 13, 80, -7, 80, 19, 80, 845, 80, 0, 0, 0, 1, 9, 0, 0, 0, 0f, 1, "威", "", "atk", "SwordHitYellowCritical", "zhangfei");
            config[101003] = new HeroConfig(101003, "马超", 1, "马", 4, 11, 80, -2, 80, 16, 80, 858, 80, 0, 0, 0, 1, 9, 0, 0, 0, 0f, 1, "铁", "", "atk", "SwordHitWhiteCritical", "machao");
            config[101004] = new HeroConfig(101004, "诸葛亮", 1, "扇", 4, 27, 80, 9, 80, -1, 80, 690, 80, 0, 10, 10, 1, 9, 0, 0, 18, 0f, 3, "神", "空", "inte", "LightningExplosionYellow", "zhugeliang");
            config[101005] = new HeroConfig(101005, "关羽", 1, "车", 4, 2, 80, 22, 80, 1, 80, 904, 80, 0, 0, 0, 1, 9, 0, 0, 0, 0f, 1, "斩", "", "atk", "SwordHitGreenCritical", "guanyu");
            config[101006] = new HeroConfig(101006, "徐庶", 1, "炮", 3, 10, 80, -5, 80, 15, 80, 606, 80, 0, 0, 0, 1, 7, 0, 0, 15, 0f, 3, "火", "共", "inte", "GasExplosionFire", "xusu");
            config[101007] = new HeroConfig(101007, "魏延", 1, "戟", 3, 3, 80, 7, 80, 10, 80, 750, 80, 0, 0, 0, 1, 7, 0, 0, 0, 0f, 1, "破", "乱", "atk", "SwordHitYellowCritical", "weiyan");
            config[101008] = new HeroConfig(101008, "黄忠", 1, "弓", 3, 7, 80, -1, 80, 9, 80, 548, 80, 0, 10, 10, 1, 7, 0, 0, 22, 1.5f, 3, "矢", "速", "shoot", "BulletExplosionFire", "huangzhong");
            config[101009] = new HeroConfig(101009, "周仓", 1, "盾", 1, 1, 80, 3, 80, 1, 80, 707, 80, 0, 0, 0, 1, 2, 0, 0, 0, 0f, 1, "劫", "", "atk", "SwordHitYellowCritical", "zhoucang");
            config[101010] = new HeroConfig(101010, "姜维", 1, "车", 4, -4, 80, 35, 80, -6, 80, 922, 80, 0, 0, 0, 1, 9, 0, 0, 0, 0f, 1, "解", "", "def", "SwordHitYellowCritical", "jiangwei");
            config[101011] = new HeroConfig(101011, "马岱", 1, "弩", 2, -2, 80, 9, 80, 3, 80, 750, 80, 0, -20, -20, 1, 4, 0, 0, 0, 0f, 1, "坚", "羽", "atk", "SwordHitYellowCritical", "madai");
            config[101012] = new HeroConfig(101012, "庞统", 1, "棋", 4, 13, 80, 6, 80, 1, 80, 615, 80, 0, 0, 0, 1, 7, 0, 0, 15, 0f, 3, "锁", "火", "inte", "ExplosionFireballFire", "pangtong");
            config[101013] = new HeroConfig(101013, "李严", 1, "士", 2, 2, 80, 11, 80, -13, 80, 685, 80, 0, -10, -10, 1, 4, 0, 0, 0, 0f, 1, "实", "", "def", "SwordHitYellowCritical", "liyan");
            config[101014] = new HeroConfig(101014, "张松", 1, "扇", 1, -6, 80, -7, 80, -7, 80, 422, 80, 0, -10, -10, 1, 2, 0, 0, 15, 0f, 3, "", "", "inte", "FanExplosion", "zhangsong");
            config[101015] = new HeroConfig(101015, "蒋琬", 1, "相", 1, -12, 80, -16, 80, -2, 80, 422, 80, 0, -20, -20, 1, 2, 0, 0, 18, 0f, 3, "", "", "help", "SharpExplosionGreen", "jiangwan");
            config[101016] = new HeroConfig(101016, "孙乾", 1, "鼓", 1, 12, 80, 8, 80, -5, 80, 476, 80, 0, 0, 0, 1, 4, 0, 0, 35, 0f, 3, "白", "", "help", "SoulExplosionOrange", "sunqian");
            config[101017] = new HeroConfig(101017, "费祎", 1, "相", 2, -3, 80, -6, 80, -6, 80, 476, 80, 0, -10, -10, 1, 4, 0, 0, 18, 0f, 3, "励", "", "help", "SharpExplosionGreen", "feiyi");
            config[101018] = new HeroConfig(101018, "马谡", 1, "枪", 2, -6, 80, -10, 80, 21, 80, 558, 80, 0, 0, 0, 1, 6, 0, 0, 15, 0f, 3, "百", "", "inte", "StormExplosion", "masu");
            config[101019] = new HeroConfig(101019, "马良", 1, "鼓", 2, 0, 80, 3, 80, 12, 80, 573, 80, 0, 0, 0, 1, 6, 0, 0, 18, 0f, 3, "静", "", "help", "SharpExplosionGreen", "maliang");
            config[101020] = new HeroConfig(101020, "法正", 1, "棋", 3, 6, 80, -4, 80, 3, 80, 575, 80, 0, 0, 0, 1, 7, 0, 0, 15, 0f, 3, "溃", "", "inte", "GasExplosionFire", "fazheng");
            config[101021] = new HeroConfig(101021, "刘禅", 1, "医", 1, -6, 80, -14, 80, 5, 80, 382, 80, 0, -10, -10, 1, 2, 0, 0, 35, 0f, 3, "碉", "", "help", "SoulExplosionOrange", "liushan");
            config[102001] = new HeroConfig(102001, "郭嘉", 1, "棋", 4, 1, 80, 7, 80, -3, 80, 573, 80, 0, 0, 0, 2, 9, 0, 0, 15, 0f, 3, "天", "", "inte", "LightningExplosionBlue", "guojia");
            config[102002] = new HeroConfig(102002, "夏侯惇", 1, "车", 3, -1, 80, 12, 80, -1, 80, 842, 80, 0, 0, 0, 2, 7, 0, 0, 0, 0f, 1, "青", "", "atk", "SwordHitYellowCritical", "xiahoudun");
            config[102003] = new HeroConfig(102003, "荀彧", 1, "相", 4, 2, 80, 11, 80, 2, 80, 579, 80, 0, 0, 0, 2, 9, 0, 0, 18, 0f, 3, "国", "", "help", "FrostExplosionBlue", "xunyu");
            config[102004] = new HeroConfig(102004, "张辽", 1, "枪", 4, 11, 80, 20, 80, 9, 80, 922, 80, 0, 0, 0, 2, 9, 0, 0, 0, 0f, 1, "旋", "", "def", "SwordHitYellowCritical", "zhangliao");
            config[102005] = new HeroConfig(102005, "许褚", 1, "锤", 4, 1, 80, -7, 80, 21, 80, 794, 80, 0, 0, 0, 2, 7, 0, 0, 0, 0f, 1, "斧", "", "atk", "SwordHitYellowCritical", "xuchu");
            config[102006] = new HeroConfig(102006, "夏侯渊", 1, "弓", 3, 6, 80, 3, 80, 6, 80, 556, 80, 0, 10, 10, 2, 7, 0, 0, 22, 1.5f, 3, "雨", "", "shoot", "BulletExplosionBlue", "xiahouyuan");
            config[102007] = new HeroConfig(102007, "典韦", 1, "戟", 4, -3, 80, -6, 80, 24, 80, 807, 80, 0, 0, 0, 2, 7, 0, 0, 0, 0f, 1, "护", "", "def", "SwordHitYellowCritical", "dianwei");
            config[102008] = new HeroConfig(102008, "张郃", 1, "炮", 4, -6, 80, 21, 80, -5, 80, 840, 80, 0, 0, 0, 2, 9, 0, 0, 0, 0f, 1, "分", "", "def", "SwordHitYellowCritical", "zhanghe");
            config[102009] = new HeroConfig(102009, "徐晃", 1, "弩", 3, 3, 80, 8, 80, 4, 80, 549, 80, 0, 10, 10, 2, 7, 0, 0, 22, 1.5f, 3, "连", "", "shoot", "BulletExplosionBlue", "xuhuang");
            config[102010] = new HeroConfig(102010, "荀攸", 1, "棋", 3, -2, 80, 10, 80, 12, 80, 623, 80, 0, 0, 0, 2, 7, 0, 0, 15, 0f, 3, "百", "米", "inte", "FrostExplosionBlue", "xunyou");
            config[102011] = new HeroConfig(102011, "于禁", 1, "枪", 2, 5, 80, 16, 80, -1, 80, 740, 80, 0, 0, 0, 2, 5, 0, 0, 0, 0f, 1, "青", "破", "def", "SwordHitYellowCritical", "yujin");
            config[102012] = new HeroConfig(102012, "曹仁", 1, "盾", 3, 5, 80, 4, 80, 1, 80, 750, 80, 0, -10, -10, 2, 7, 0, 0, 0, 0f, 1, "青", "", "atk", "SwordHitYellowCritical", "caoren");
            config[102013] = new HeroConfig(102013, "曹洪", 1, "锤", 2, 0, 80, -5, 80, 0, 80, 695, 80, 0, -10, -10, 2, 4, 0, 0, 0, 0f, 1, "商", "", "atk", "SwordHitYellowCritical", "caohong");
            config[102014] = new HeroConfig(102014, "庞德", 1, "马", 3, 3, 80, 14, 80, 8, 80, 830, 80, 0, 0, 0, 2, 7, 0, 0, 0, 0f, 1, "坚", "", "atk", "SwordHitYellowCritical", "pangde");
            config[102015] = new HeroConfig(102015, "乐进", 1, "车", 1, -4, 80, -6, 80, 0, 80, 620, 80, 0, -10, -10, 2, 2, 0, 0, 0, 0f, 1, "奋", "", "atk", "SwordHitYellowCritical", "lejin");
            config[102016] = new HeroConfig(102016, "司马懿", 1, "扇", 4, 15, 80, -5, 80, 10, 80, 603, 80, 0, 0, 0, 2, 7, 0, 0, 15, 0f, 3, "鬼", "", "inte", "ShadowExplosion", "simayi");
            config[102017] = new HeroConfig(102017, "程昱", 1, "扇", 2, -6, 80, 1, 80, 10, 80, 561, 80, 0, 0, 0, 2, 4, 0, 0, 15, 0f, 3, "识", "火", "inte", "StormExplosion", "chengyu");
            config[102018] = new HeroConfig(102018, "文鸯", 1, "弩", 1, -6, 80, -1, 80, 7, 80, 510, 80, 0, 0, 0, 2, 4, 0, 0, 22, 1.5f, 3, "速", "", "shoot", "BulletExplosionBlue", "wenyuan");
            config[102019] = new HeroConfig(102019, "曹真", 1, "戟", 1, 1, 80, 10, 80, -6, 80, 695, 80, 0, 0, 0, 2, 2, 0, 0, 0, 0f, 1, "境", "", "def", "SwordHitYellowCritical", "caozhen");
            config[102020] = new HeroConfig(102020, "陈群", 1, "相", 1, 5, 80, 5, 80, 0, 80, 522, 80, 0, 0, 0, 2, 2, 0, 0, 15, 0f, 3, "励", "米", "help", "FanExplosion", "chenqun");
            config[102021] = new HeroConfig(102021, "李典", 1, "士", 2, -3, 80, 18, 80, -5, 80, 750, 80, 0, -10, -10, 2, 4, 0, 0, 0, 0f, 1, "伏", "坚", "def", "SwordHitYellowCritical", "lidian");
            config[102022] = new HeroConfig(102022, "刘晔", 1, "炮", 2, -7, 80, 12, 80, -20, 80, 405, 80, 0, -10, -10, 2, 2, 0, 0, 13, 2.5f, 3, "", "", "shoot", "GasShootFire", "liuye");
            config[103001] = new HeroConfig(103001, "孙策", 1, "戟", 4, -3, 80, 18, 80, -5, 80, 820, 80, 0, 0, 0, 3, 9, 0, 0, 0, 0f, 1, "虎", "", "atk", "SwordHitYellowCritical", "sunce");
            config[103002] = new HeroConfig(103002, "孙坚", 1, "枪", 3, 6, 80, 16, 80, 3, 80, 835, 80, 0, 0, 0, 3, 7, 0, 0, 0, 0f, 1, "旋", "", "atk", "SwordHitYellowCritical", "sunjian");
            config[103003] = new HeroConfig(103003, "甘宁", 1, "弩", 4, -1, 80, 1, 80, 0, 80, 484, 80, 0, 0, 0, 3, 7, 0, 0, 22, 1.5f, 3, "连", "", "shoot", "BulletExplosionBlue", "ganning");
            config[103004] = new HeroConfig(103004, "太史慈", 1, "弓", 4, 6, 80, 0, 80, 9, 80, 556, 80, 0, 10, 10, 3, 7, 0, 0, 22, 1.5f, 3, "雨", "", "shoot", "BulletExplosionBlue", "taishici");
            config[103005] = new HeroConfig(103005, "黄盖", 1, "士", 2, 7, 80, 13, 80, -5, 80, 760, 80, 0, 0, 0, 3, 7, 0, 0, 0, 0f, 1, "奋", "", "def", "SwordHitYellowCritical", "huanggai");
            config[103006] = new HeroConfig(103006, "周泰", 1, "锤", 3, 4, 80, -4, 80, 10, 80, 774, 80, 0, -10, -10, 3, 4, 0, 0, 0, 0f, 1, "连", "", "atk", "SwordHitYellowCritical", "zhoutai");
            config[103007] = new HeroConfig(103007, "鲁肃", 1, "相", 4, 15, 80, 2, 80, 13, 80, 615, 80, 0, 0, 0, 3, 9, 0, 0, 18, 0f, 3, "商", "雷", "help", "SharpExplosionGreen", "lusu");
            config[103008] = new HeroConfig(103008, "周瑜", 1, "棋", 4, 17, 80, -2, 80, 20, 80, 675, 80, 0, 10, 10, 3, 9, 0, 0, 15, 0f, 3, "炎", "炽", "inte", "ExplosionFireballFire", "zhouyu");
            config[103009] = new HeroConfig(103009, "蒋钦", 1, "戟", 2, -6, 80, -4, 80, 0, 80, 635, 80, 0, -10, -10, 3, 4, 0, 0, 0, 0f, 1, "", "", "atk", "SwordHitYellowCritical", "jiangqing");
            config[103010] = new HeroConfig(103010, "吕蒙", 1, "马", 4, 4, 80, 28, 80, -7, 80, 815, 80, 0, 0, 0, 3, 9, 0, 0, 0, 0f, 1, "学", "羽", "def", "SwordHitYellowCritical", "lvmeng");
            config[103011] = new HeroConfig(103011, "陆逊", 1, "炮", 4, 13, 80, -8, 80, 15, 80, 603, 80, 0, 0, 0, 3, 7, 0, 0, 15, 0f, 3, "炎", "", "inte", "GasExplosionFire", "luxun");
            config[103012] = new HeroConfig(103012, "张昭", 1, "相", 3, -5, 80, -3, 80, -7, 80, 486, 80, 0, -10, -10, 3, 7, 0, 0, 18, 0f, 3, "", "", "help", "SharpExplosionGreen", "zhangzhao");
            config[103013] = new HeroConfig(103013, "诸葛瑾", 1, "鼓", 3, 12, 80, 1, 80, -3, 80, 531, 80, 0, 0, 0, 3, 4, 0, 0, 15, 0f, 3, "励", "", "help", "FanExplosion", "zhugejin");
            config[103014] = new HeroConfig(103014, "孙尚香", 1, "弩", 3, -10, 80, 0, 80, -5, 80, 471, 80, 0, 0, 0, 3, 7, 0, 0, 22, 1.5f, 3, "", "", "shoot", "BulletExplosionBlue", "sunshangxiang");
            config[103015] = new HeroConfig(103015, "朱桓", 1, "鼓", 2, 3, 80, 21, 80, 1, 80, 850, 80, 0, 0, 0, 3, 4, 0, 0, 0, 0f, 1, "伏", "缓", "def", "SwordHitYellowCritical", "zhuhuan");
            config[103016] = new HeroConfig(103016, "大乔", 1, "琴", 3, 26, 80, 6, 80, -22, 80, 488, 80, 0, 10, 10, 3, 4, 0, 0, 15, 0f, 3, "碉", "陷", "help", "StormExplosion", "daqiao");
            config[103017] = new HeroConfig(103017, "小乔", 1, "琴", 2, 5, 80, 6, 80, -16, 80, 435, 80, 0, 0, 0, 3, 4, 0, 0, 15, 0f, 3, "曲", "陷", "help", "StormExplosion", "xiaoqiao");
            config[103018] = new HeroConfig(103018, "丁奉", 1, "炮", 1, -5, 80, -10, 80, 0, 80, 408, 80, 0, -10, -10, 3, 2, 0, 0, 13, 2.5f, 3, "", "", "shoot", "GasShootFire", "dingfeng");
            config[103019] = new HeroConfig(103019, "凌统", 1, "弓", 1, 1, 80, -1, 80, 0, 80, 457, 80, 0, 0, 0, 3, 4, 0, 0, 30, 1.5f, 3, "虐", "", "shoot", "BulletExplosionBlue", "lingtong");
            config[103020] = new HeroConfig(103020, "潘璋", 1, "锤", 1, -1, 80, 18, 80, 3, 80, 747, 80, 0, 0, 0, 3, 2, 0, 0, 0, 0f, 1, "刺", "虐", "def", "SwordHitYellowCritical", "panzhang");
            config[103021] = new HeroConfig(103021, "徐盛", 1, "盾", 1, 4, 80, 12, 80, -16, 80, 685, 80, 0, 0, 0, 3, 2, 0, 0, 0, 0f, 1, "乱", "", "def", "SwordHitYellowCritical", "xusheng");
            config[103022] = new HeroConfig(103022, "程普", 1, "盾", 2, 3, 80, 18, 80, -1, 80, 750, 80, 0, 0, 0, 3, 4, 0, 0, 0, 0f, 1, "实", "奋", "def", "SwordHitYellowCritical", "chengpu");
            config[104001] = new HeroConfig(104001, "吕布", 1, "马", 4, 13, 80, -4, 80, 16, 80, 929, 80, 0, 0, 0, 4, 9, 0, 0, 0, 0f, 1, "魔", "羽", "atk", "SwordHitBlackRedCritical", "lvbu");
            config[104002] = new HeroConfig(104002, "华雄", 1, "盾", 3, -7, 80, 7, 80, -5, 80, 754, 80, 0, -10, -10, 4, 6, 0, 0, 0, 0f, 1, "纷", "", "atk", "SwordHitYellowCritical", "huaxiong");
            config[104003] = new HeroConfig(104003, "贾诩", 1, "扇", 3, 13, 80, 4, 80, 3, 80, 620, 80, 0, 0, 0, 4, 7, 0, 0, 15, 0f, 3, "延", "", "inte", "StormExplosion", "jiaxu");
            config[104004] = new HeroConfig(104004, "貂蝉", 1, "琴", 3, -19, 80, -8, 80, 22, 80, 439, 80, 0, 0, 0, 4, 5, 0, 0, 15, 0f, 3, "曲", "", "help", "StormExplosion", "diaochan");
            config[104005] = new HeroConfig(104005, "臧霸", 1, "马", 2, -6, 80, -5, 80, -9, 80, 645, 80, 0, -20, -20, 4, 4, 0, 0, 0, 0f, 1, "虐", "", "atk", "SwordHitYellowCritical", "zangba");
            config[104006] = new HeroConfig(104006, "高顺", 1, "炮", 2, 5, 80, -12, 80, 7, 80, 460, 80, 0, 0, 0, 4, 4, 0, 0, 13, 2.5f, 3, "", "", "shoot", "GasShootFire", "gaoshun");
            config[104007] = new HeroConfig(104007, "李儒", 1, "扇", 3, -12, 80, -7, 80, -6, 80, 476, 80, 0, -10, -10, 4, 4, 0, 0, 15, 0f, 3, "火", "", "inte", "ShadowExplosion", "liru");
            config[104008] = new HeroConfig(104008, "陈宫", 1, "棋", 3, 7, 80, -8, 80, 6, 80, 569, 80, 0, 0, 0, 4, 7, 0, 0, 15, 0f, 3, "励", "溃", "inte", "ShadowExplosion", "chengong");
            config[105001] = new HeroConfig(105001, "邓艾", 1, "盾", 4, 4, 80, 24, 80, -3, 80, 838, 80, 0, 0, 0, 5, 9, 0, 0, 0, 0f, 1, "奇", "", "def", "SwordHitYellowCritical", "dengai");
            config[105002] = new HeroConfig(105002, "司马师", 1, "鼓", 2, 2, 80, -13, 80, 11, 80, 527, 80, 0, -10, -10, 5, 4, 0, 0, 18, 0f, 3, "", "", "help", "SharpExplosionGreen", "simashi");
            config[105003] = new HeroConfig(105003, "司马昭", 1, "相", 3, 9, 80, -3, 80, 9, 80, 582, 80, 0, 0, 0, 5, 5, 0, 0, 18, 0f, 3, "溃", "", "help", "SharpExplosionGreen", "simazhao");
            config[105004] = new HeroConfig(105004, "羊祜", 1, "锤", 2, 10, 80, 24, 80, -14, 80, 760, 80, 0, 0, 0, 5, 6, 0, 0, 0, 0f, 1, "敏", "", "atk", "SwordHitYellowCritical", "yangku");
            config[105005] = new HeroConfig(105005, "钟会", 1, "扇", 2, 4, 80, -6, 80, 7, 80, 582, 80, 0, 0, 0, 5, 4, 0, 0, 15, 0f, 3, "缓", "", "inte", "StormExplosion", "zhonghui");
            config[105006] = new HeroConfig(105006, "陈泰", 1, "马", 1, 4, 80, 16, 80, -20, 80, 680, 80, 0, 0, 0, 5, 2, 0, 0, 0, 0f, 1, "虐", "", "def", "SwordHitYellowCritical", "chentai");
            config[105007] = new HeroConfig(105007, "杜预", 1, "枪", 1, 17, 80, -1, 80, -16, 80, 544, 80, 0, -10, -10, 5, 2, 0, 0, 18, 0f, 3, "米", "", "inte", "SharpExplosionGreen", "duyu");
            config[106001] = new HeroConfig(106001, "颜良", 1, "车", 3, -7, 80, -11, 80, -2, 80, 710, 80, 0, -10, -10, 6, 7, 0, 0, 0, 0f, 1, "破", "", "atk", "SwordHitYellowCritical", "yanliang");
            config[106002] = new HeroConfig(106002, "文丑", 1, "锤", 3, -3, 80, -3, 80, 1, 80, 777, 80, 0, -10, -10, 6, 7, 0, 0, 0, 0f, 1, "刺", "", "def", "SwordHitYellowCritical", "wenchou");
            config[106003] = new HeroConfig(106003, "田丰", 1, "鼓", 3, 1, 80, 2, 80, -13, 80, 514, 80, 0, -10, -10, 6, 7, 0, 0, 15, 0f, 3, "雷", "", "inte", "StormExplosion", "tianfeng");
            config[106004] = new HeroConfig(106004, "鞠义", 1, "弩", 2, -7, 80, -7, 80, -1, 80, 476, 80, 0, 0, 0, 6, 4, 0, 0, 22, 1.5f, 3, "", "", "shoot", "BulletExplosionBlue", "juyi");
            config[106005] = new HeroConfig(106005, "许攸", 1, "棋", 2, -27, 80, 0, 80, -13, 80, 442, 80, 0, -20, -20, 6, 5, 0, 0, 15, 0f, 3, "火", "", "inte", "StormExplosion", "xuyou");
            config[106006] = new HeroConfig(106006, "高览", 1, "弓", 1, -14, 80, 4, 80, -10, 80, 632, 80, 0, -20, -20, 6, 2, 0, 0, 0, 0f, 1, "", "", "atk", "SwordHitYellowCritical", "gaolan");
            config[106007] = new HeroConfig(106007, "沮授", 1, "相", 2, 5, 80, -4, 80, -11, 80, 518, 80, 0, -10, -10, 6, 4, 0, 0, 15, 0f, 3, "静", "", "inte", "StormExplosion", "jushou");
            config[106008] = new HeroConfig(106008, "郭图", 1, "棋", 1, -5, 80, 7, 80, 8, 80, 531, 80, 0, 0, 0, 6, 2, 0, 0, 15, 0f, 3, "励", "米", "help", "FanExplosion", "guotu");
            config[110001] = new HeroConfig(110001, "公孙瓒", 1, "马", 3, -8, 80, 11, 80, -8, 80, 687, 80, 0, -10, -10, 10, 7, 0, 0, 0, 0f, 1, "乱", "", "def", "SwordHitYellowCritical", "gongsunzan");
            config[110002] = new HeroConfig(110002, "张任", 1, "弓", 2, 0, 80, 4, 80, -4, 80, 497, 80, 0, 0, 0, 10, 4, 0, 0, 22, 1.5f, 3, "复", "", "shoot", "BulletExplosionBlue", "zhangren");
            config[110003] = new HeroConfig(110003, "华佗", 1, "医", 2, 13, 80, 3, 80, -6, 80, 548, 80, 0, 0, 0, 10, 5, 0, 0, 14, 0f, 3, "药", "", "help", "ShadowExplosionGreen", "huatuo");
            config[110004] = new HeroConfig(110004, "袁术", 1, "车", 2, -8, 80, 9, 80, -11, 80, 647, 80, 0, -10, -10, 10, 4, 0, 0, 0, 0f, 1, "", "", "def", "SwordHitYellowCritical", "yuanshu");
            config[110005] = new HeroConfig(110005, "马腾", 1, "弩", 2, -5, 80, -8, 80, -7, 80, 625, 80, 0, -20, -20, 10, 4, 0, 0, 0, 0f, 1, "羽", "", "atk", "SwordHitYellowCritical", "mateng");
            config[110006] = new HeroConfig(110006, "于吉", 1, "医", 2, -2, 80, -4, 80, 1, 80, 501, 80, 0, 0, 0, 10, 4, 0, 0, 14, 0f, 3, "调", "", "help", "ShadowExplosionGreen", "yuji");
            config[110007] = new HeroConfig(110007, "张角", 1, "棋", 3, 27, 80, 6, 80, -13, 80, 629, 80, 0, 0, 0, 10, 7, 0, 0, 15, 0f, 3, "天", "陷", "inte", "LightningExplosionBlue", "zhangjiao");
            config[110008] = new HeroConfig(110008, "张宝", 1, "戟", 2, -5, 80, 17, 80, -17, 80, 680, 80, 0, -10, -10, 10, 4, 0, 0, 0, 0f, 1, "劫", "", "atk", "SwordHitYellowCritical", "zhangbao2");
            config[110009] = new HeroConfig(110009, "张梁", 1, "士", 1, 1, 80, -3, 80, 2, 80, 465, 80, 0, 0, 0, 10, 2, 0, 0, 13, 2.5f, 3, "", "", "def", "SwordHitYellowCritical", "zhangliang");

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
