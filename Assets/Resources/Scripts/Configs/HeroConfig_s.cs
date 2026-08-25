using System;
using System.Collections;
using System.Collections.Generic;

namespace CommonConfig
{
    public class HeroConfig
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
        ///等级
        /// </summary>
        public int Lv;
        /// <summary>
        ///攻击
        /// </summary>
        public int Atk;
        /// <summary>
        ///攻击成长百分比（每星，如80=每星+80%）
        /// </summary>
        public int AtkP;
        /// <summary>
        ///法术强度
        /// </summary>
        public int Ap;
        /// <summary>
        ///法术强度成长百分比（每星）
        /// </summary>
        public int ApP;
        /// <summary>
        ///无双强度
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
        ///攻速（攻击间隔秒数）
        /// </summary>
        public float AtkSpeed;
        /// <summary>
        ///护甲
        /// </summary>
        public int Armor;
        /// <summary>
        ///法术抗性
        /// </summary>
        public int MagicRes;
        /// <summary>
        ///阵营
        /// </summary>
        public int Side;
        /// <summary>
        ///移动速度
        /// </summary>
        public int MoveSpeed;
        /// <summary>
        ///攻击距离
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
        ///出场概率
        /// </summary>
        public int RateWeight;
        /// <summary>
        ///出场概率，绝对
        /// </summary>
        public int RateAbs;
        /// <summary>
        ///站位
        /// </summary>
        public int Pos;
        /// <summary>
        ///职业
        /// </summary>
        public string Job;
        /// <summary>
        ///技能
        /// </summary>
        public int[] Skills;
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
        /// <summary>
        ///关系数量
        /// </summary>
        public int FriendCount;
        /// <summary>
        ///品质：1普通 2优秀 3精良 4史诗
        /// </summary>
        public int Quality;
        /// <summary>
        ///价格（配表数据，2-10）
        /// </summary>
        public int Price;


        public HeroConfig(int Id, string Name, int Lv, int Atk, int AtkP, int Ap, int ApP, int Might, int MightP, int Hp, int HpP, float AtkSpeed, int Armor, int MagicRes, int Side, int MoveSpeed, int Range, int MissileSpeed, float MissileHight, int RateWeight, int RateAbs, int Pos, string Job, int[] Skills, string Skill1, string Skill2, string Group, string HitEffect, string Icon, int FriendCount, int Quality, int Price)
        {
            this.Id = Id;
            this.Name = Name;
            this.Lv = Lv;
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
            this.MoveSpeed = MoveSpeed;
            this.Range = Range;
            this.MissileSpeed = MissileSpeed;
            this.MissileHight = MissileHight;
            this.RateWeight = RateWeight;
            this.RateAbs = RateAbs;
            this.Pos = Pos;
            this.Job = Job;
            this.Skills = Skills;
            this.Skill1 = Skill1;
            this.Skill2 = Skill2;
            this.Group = Group;
            this.HitEffect = HitEffect;
            this.Icon = Icon;
            this.FriendCount = FriendCount;
            this.Quality = Quality;
            this.Price = Price;

        }

        public HeroConfig() { }

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
            {"Atk", new FieldMetaInfo("攻击", "int", 0, "95-100:#FF9900,90-94:#995500,80-89:#33CC33")},
            {"AtkP", new FieldMetaInfo("攻击成长百分比（每星，如80=每星+80%）", "int", 0)},
            {"Ap", new FieldMetaInfo("法术强度", "int", 0, "95-100:#FF9900,90-94:#995500,80-89:#33CC33")},
            {"ApP", new FieldMetaInfo("法术强度成长百分比（每星）", "int", 0)},
            {"Might", new FieldMetaInfo("无双强度", "int", 0, "95-100:#FF9900,90-94:#995500,80-89:#33CC33")},
            {"MightP", new FieldMetaInfo("无双强度成长百分比（每星）", "int", 0)},
            {"Hp", new FieldMetaInfo("生命", "int", 0)},
            {"HpP", new FieldMetaInfo("生命成长百分比（每星）", "int", 0)},
            {"AtkSpeed", new FieldMetaInfo("攻速（攻击间隔秒数）", "float", 0)},
            {"Armor", new FieldMetaInfo("护甲", "int", 0)},
            {"MagicRes", new FieldMetaInfo("法术抗性", "int", 0)},
            {"Side", new FieldMetaInfo("阵营", "int", 0)},
            {"MoveSpeed", new FieldMetaInfo("移动速度", "int", 0)},
            {"Range", new FieldMetaInfo("攻击距离", "int", 0)},
            {"MissileSpeed", new FieldMetaInfo("导弹速度", "int", 0)},
            {"MissileHight", new FieldMetaInfo("导弹高度", "float", 0)},
            {"RateWeight", new FieldMetaInfo("出场概率", "int", 0)},
            {"RateAbs", new FieldMetaInfo("出场概率，绝对", "int", 0)},
            {"Pos", new FieldMetaInfo("站位", "int", 0)},
            {"Job", new FieldMetaInfo("职业", "string", 0)},
            {"Skills", new FieldMetaInfo("技能", "int[]", 0)},
            {"Skill1", new FieldMetaInfo("技能", "string", 0)},
            {"Skill2", new FieldMetaInfo("技能2", "string", 0)},
            {"Group", new FieldMetaInfo("团队", "string", 0)},
            {"HitEffect", new FieldMetaInfo("hit", "string", 0)},
            {"Icon", new FieldMetaInfo("背景图", "string", 0)},
            {"FriendCount", new FieldMetaInfo("关系数量", "int", 0)},
            {"Quality", new FieldMetaInfo("品质：1普通 2优秀 3精良 4史诗", "int", 0, "4:#FF9900,3:#995500,2:#33CC33,1:#3333CC")},
            {"Price", new FieldMetaInfo("价格（配表数据，2-10）", "int", 0, "9-10:#FF9900,7-8:#995500,5-6:#33CC33,3-4:#3333CC")},
        };

        public static Dictionary<string, FieldMetaInfo> FieldMeta { get { return fieldMeta; } }

        private static List<CellMeta> cellMeta = new List<CellMeta>();
        public static List<CellMeta> CellMetas { get { return cellMeta; } }
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
            config[100001] = new HeroConfig(100001, "刘备", 1, 76, 80, 75, 80, 74, 80, 764,80, 1.5f, 40, 35, 1, 10, 17, 0, 0f, 0, 100, 1, "帅", new int[0], "仁", "", "core", "SwordHitYellowCritical", "liubei", 6, 3, 8);
            config[100002] = new HeroConfig(100002, "曹操", 1, 87, 80, 81, 80, 72, 80, 810,80, 1.5f, 50, 45, 2, 10, 17, 0, 0f, 0, 100, 1, "帅", new int[0], "识", "", "core", "SwordHitYellowCritical", "caocao", 6, 4, 9);
            config[100003] = new HeroConfig(100003, "孙权", 1, 78, 80, 78, 80, 69, 80, 774,80, 1.5f, 40, 35, 3, 10, 17, 0, 0f, 0, 100, 1, "帅", new int[0], "衡", "", "core", "SwordHitYellowCritical", "sunquan", 3, 3, 8);
            config[100004] = new HeroConfig(100004, "董卓", 1, 77, 80, 68, 80, 80, 80, 764,80, 1.5f, 40, 35, 4, 10, 17, 0, 0f, 0, 0, 1, "帅", new int[0], "", "", "core", "SwordHitYellowCritical", "dongzhuo", 2, 3, 8);
            config[100005] = new HeroConfig(100005, "司马炎", 1, 71, 80, 79, 80, 60, 80, 732,80, 1.5f, 40, 35, 5, 10, 17, 0, 0f, 0, 0, 1, "帅", new int[0], "", "", "core", "SwordHitYellowCritical", "simayan", 2, 3, 7);
            config[100006] = new HeroConfig(100006, "袁绍", 1, 75, 80, 71, 80, 64, 80, 695,80, 1.5f, 40, 35, 6, 10, 17, 0, 0f, 0, 0, 1, "帅", new int[0], "", "", "core", "SwordHitYellowCritical", "yuanshao", 2, 3, 7);
            config[101001] = new HeroConfig(101001, "赵云", 1, 88, 80, 74, 80, 93, 80, 924,80, 1.5f, 55, 45, 1, 10, 17, 0, 0f, 100, 0, 1, "马", new int[0], "镜", "羽", "def", "SwordHitWhiteCritical", "zhaoyun", 4, 4, 10);
            config[101002] = new HeroConfig(101002, "张飞", 1, 93, 80, 48, 80, 99, 80, 845,80, 1.5f, 55, 45, 1, 10, 17, 0, 0f, 64, 0, 1, "枪", new int[0], "威", "", "atk", "SwordHitYellowCritical", "zhangfei", 4, 4, 9);
            config[101003] = new HeroConfig(101003, "马超", 1, 91, 80, 53, 80, 96, 80, 858,80, 1.5f, 55, 45, 1, 10, 17, 0, 0f, 70, 0, 1, "马", new int[0], "铁", "", "atk", "SwordHitWhiteCritical", "machao", 4, 4, 9);
            config[101004] = new HeroConfig(101004, "诸葛亮", 1, 97, 80, 99, 80, 44, 80, 690,80, 1.5f, 45, 55, 1, 10, 40, 18, 0f, 70, 0, 3, "谋", new int[0], "神", "空", "inte", "LightningExplosionYellow", "zhugeliang", 6, 4, 9);
            config[101005] = new HeroConfig(101005, "关羽", 1, 92, 80, 72, 80, 91, 80, 904,80, 1.5f, 55, 45, 1, 10, 17, 0, 0f, 115, 0, 1, "车", new int[0], "斩", "", "atk", "SwordHitGreenCritical", "guanyu", 7, 4, 10);
            config[101006] = new HeroConfig(101006, "徐庶", 1, 80, 80, 85, 80, 60, 80, 606,80, 1.5f, 35, 45, 1, 10, 40, 15, 0f, 73, 0, 3, "谋", new int[0], "火", "共", "inte", "GasExplosionFire", "xusu", 1, 3, 8);
            config[101007] = new HeroConfig(101007, "魏延", 1, 78, 80, 62, 80, 85, 80, 750,80, 1.5f, 45, 35, 1, 10, 17, 0, 0f, 69, 0, 1, "戟", new int[0], "破", "乱", "atk", "SwordHitYellowCritical", "weiyan", 1, 3, 8);
            config[101008] = new HeroConfig(101008, "黄忠", 1, 77, 80, 54, 80, 79, 80, 548,80, 1.5f, 35, 40, 1, 10, 40, 22, 1.5f, 75, 0, 3, "弓", new int[0], "矢", "速", "shoot", "BulletExplosionFire", "huangzhong", 3, 3, 7);
            config[101009] = new HeroConfig(101009, "周仓", 1, 66, 80, 58, 80, 86, 80, 707,80, 1.5f, 25, 15, 1, 10, 17, 0, 0f, 33, 0, 1, "刀", new int[0], "劫", "", "atk", "SwordHitYellowCritical", "zhoucang", 1, 1, 7);
            config[101010] = new HeroConfig(101010, "姜维", 1, 86, 80, 85, 80, 84, 80, 922,80, 1.5f, 55, 45, 1, 10, 17, 0, 0f, 115, 0, 1, "车", new int[0], "解", "", "def", "SwordHitYellowCritical", "jiangwei", 3, 4, 10);
            config[101011] = new HeroConfig(101011, "马岱", 1, 78, 80, 64, 80, 83, 80, 750,80, 1.5f, 35, 25, 1, 10, 17, 0, 0f, 57, 0, 1, "马", new int[0], "坚", "羽", "atk", "SwordHitYellowCritical", "madai", 2, 2, 8);
            config[101012] = new HeroConfig(101012, "庞统", 1, 83, 80, 96, 80, 46, 80, 615,80, 1.5f, 35, 45, 1, 10, 40, 15, 0f, 56, 0, 3, "谋", new int[0], "锁", "火", "inte", "ExplosionFireballFire", "pangtong", 2, 3, 8);
            config[101013] = new HeroConfig(101013, "李严", 1, 72, 80, 66, 80, 72, 80, 685,80, 1.5f, 35, 25, 1, 10, 17, 0, 0f, 67, 0, 1, "士", new int[0], "实", "", "def", "SwordHitYellowCritical", "liyan", 1, 2, 7);
            config[101014] = new HeroConfig(101014, "张松", 1, 54, 80, 73, 80, 38, 80, 422,80, 1.5f, 15, 25, 1, 10, 40, 15, 0f, 33, 0, 3, "扇", new int[0], "", "", "inte", "FanExplosion", "zhangsong", 1, 1, 4);
            config[101015] = new HeroConfig(101015, "蒋琬", 1, 53, 80, 69, 80, 43, 80, 422,80, 1.5f, 15, 25, 1, 10, 40, 18, 0f, 33, 0, 3, "相", new int[0], "", "", "help", "SharpExplosionGreen", "jiangwan", 1, 1, 4);
            config[101016] = new HeroConfig(101016, "孙乾", 1, 57, 80, 73, 80, 50, 80, 476,80, 1.5f, 25, 35, 1, 10, 40, 35, 0f, 31, 0, 3, "鼓", new int[0], "白", "", "help", "SoulExplosionOrange", "sunqian", 1, 2, 5);
            config[101017] = new HeroConfig(101017, "费祎", 1, 62, 80, 79, 80, 39, 80, 476,80, 1.5f, 25, 35, 1, 10, 40, 18, 0f, 31, 0, 3, "相", new int[0], "励", "", "help", "SharpExplosionGreen", "feiyi", 1, 2, 5);
            config[101018] = new HeroConfig(101018, "马谡", 1, 64, 80, 80, 80, 66, 80, 558,80, 1.5f, 35, 45, 1, 10, 40, 15, 0f, 56, 0, 3, "谋", new int[0], "百", "", "inte", "StormExplosion", "masu", 1, 3, 7);
            config[101019] = new HeroConfig(101019, "马良", 1, 65, 80, 88, 80, 57, 80, 573,80, 1.5f, 35, 45, 1, 10, 40, 18, 0f, 48, 0, 3, "相", new int[0], "静", "", "help", "SharpExplosionGreen", "maliang", 2, 3, 7);
            config[101020] = new HeroConfig(101020, "法正", 1, 76, 80, 86, 80, 48, 80, 575,80, 1.5f, 35, 45, 1, 10, 40, 15, 0f, 55, 0, 3, "谋", new int[0], "溃", "", "inte", "GasExplosionFire", "fazheng", 1, 3, 7);
            config[101021] = new HeroConfig(101021, "刘禅", 1, 39, 80, 51, 80, 60, 80, 382,80, 1.5f, 15, 25, 1, 10, 40, 35, 0f, 25, 0, 3, "鼓", new int[0], "碉", "", "help", "SoulExplosionOrange", "liushan", 1, 1, 3);
            config[102001] = new HeroConfig(102001, "郭嘉", 1, 71, 80, 97, 80, 42, 80, 573,80, 1.5f, 35, 45, 2, 10, 40, 15, 0f, 41, 0, 3, "谋", new int[0], "天", "", "inte", "LightningExplosionBlue", "guojia", 2, 3, 7);
            config[102002] = new HeroConfig(102002, "夏侯惇", 1, 89, 80, 62, 80, 89, 80, 842,80, 1.5f, 55, 45, 2, 10, 17, 0, 0f, 73, 0, 1, "车", new int[0], "青", "", "atk", "SwordHitYellowCritical", "xiahoudun", 2, 4, 9);
            config[102003] = new HeroConfig(102003, "荀彧", 1, 67, 80, 96, 80, 47, 80, 579,80, 1.5f, 35, 45, 2, 10, 40, 18, 0f, 39, 0, 3, "相", new int[0], "国", "", "help", "FrostExplosionBlue", "xunyu", 3, 3, 7);
            config[102004] = new HeroConfig(102004, "张辽", 1, 91, 80, 75, 80, 89, 80, 922,80, 1.5f, 55, 45, 2, 10, 17, 0, 0f, 103, 0, 1, "马", new int[0], "旋", "", "def", "SwordHitYellowCritical", "zhangliao", 5, 4, 10);
            config[102005] = new HeroConfig(102005, "许褚", 1, 71, 80, 48, 80, 106, 80, 794,80, 1.5f, 45, 35, 2, 10, 17, 0, 0f, 36, 0, 1, "士", new int[0], "斧", "", "atk", "SwordHitYellowCritical", "xuchu", 2, 3, 8);
            config[102006] = new HeroConfig(102006, "夏侯渊", 1, 76, 80, 58, 80, 76, 80, 556,80, 1.5f, 35, 40, 2, 10, 40, 22, 1.5f, 75, 0, 3, "弓", new int[0], "雨", "", "shoot", "BulletExplosionBlue", "xiahouyuan", 2, 3, 7);
            config[102007] = new HeroConfig(102007, "典韦", 1, 67, 80, 49, 80, 109, 80, 807,80, 1.5f, 45, 35, 2, 10, 17, 0, 0f, 31, 0, 1, "士", new int[0], "护", "", "def", "SwordHitYellowCritical", "dianwei", 1, 3, 8);
            config[102008] = new HeroConfig(102008, "张郃", 1, 84, 80, 71, 80, 85, 80, 840,80, 1.5f, 55, 45, 2, 10, 17, 0, 0f, 85, 0, 1, "车", new int[0], "分", "", "def", "SwordHitYellowCritical", "zhanghe", 3, 4, 9);
            config[102009] = new HeroConfig(102009, "徐晃", 1, 73, 80, 63, 80, 74, 80, 549,80, 1.5f, 35, 40, 2, 10, 40, 22, 1.5f, 91, 0, 3, "弓", new int[0], "连", "", "shoot", "BulletExplosionBlue", "xuhuang", 2, 3, 7);
            config[102010] = new HeroConfig(102010, "荀攸", 1, 68, 80, 100, 80, 57, 80, 623,80, 1.5f, 35, 45, 2, 10, 40, 15, 0f, 38, 0, 3, "谋", new int[0], "百", "米", "inte", "FrostExplosionBlue", "xunyou", 2, 3, 8);
            config[102011] = new HeroConfig(102011, "于禁", 1, 80, 80, 71, 80, 74, 80, 740,80, 1.5f, 45, 35, 2, 10, 17, 0, 0f, 53, 0, 1, "戟", new int[0], "青", "破", "def", "SwordHitYellowCritical", "yujin", 2, 3, 8);
            config[102012] = new HeroConfig(102012, "曹仁", 1, 85, 80, 59, 80, 81, 80, 750,80, 1.5f, 45, 35, 2, 10, 17, 0, 0f, 64, 0, 1, "枪", new int[0], "青", "", "atk", "SwordHitYellowCritical", "caoren", 2, 3, 8);
            config[102013] = new HeroConfig(102013, "曹洪", 1, 80, 80, 50, 80, 80, 80, 695,80, 1.5f, 45, 35, 2, 10, 17, 0, 0f, 44, 0, 1, "枪", new int[0], "商", "", "atk", "SwordHitYellowCritical", "caohong", 1, 3, 7);
            config[102014] = new HeroConfig(102014, "庞德", 1, 83, 80, 69, 80, 88, 80, 830,80, 1.5f, 55, 45, 2, 10, 17, 0, 0f, 90, 0, 1, "枪", new int[0], "坚", "", "atk", "SwordHitYellowCritical", "pangde", 2, 4, 9);
            config[102015] = new HeroConfig(102015, "乐进", 1, 71, 80, 49, 80, 75, 80, 620,80, 1.5f, 35, 25, 2, 10, 17, 0, 0f, 46, 0, 1, "戟", new int[0], "奋", "", "atk", "SwordHitYellowCritical", "lejin", 2, 2, 6);
            config[102016] = new HeroConfig(102016, "司马懿", 1, 85, 80, 85, 80, 55, 80, 603,80, 1.5f, 35, 45, 2, 10, 40, 15, 0f, 93, 0, 3, "谋", new int[0], "鬼", "", "inte", "ShadowExplosion", "simayi", 1, 3, 8);
            config[102017] = new HeroConfig(102017, "程昱", 1, 64, 80, 91, 80, 55, 80, 561,80, 1.5f, 35, 45, 2, 10, 40, 15, 0f, 37, 0, 3, "谋", new int[0], "识", "火", "inte", "StormExplosion", "chengyu", 1, 3, 7);
            config[102018] = new HeroConfig(102018, "文鸯", 1, 64, 80, 54, 80, 77, 80, 510,80, 1.5f, 25, 30, 2, 10, 40, 22, 1.5f, 57, 0, 3, "弓", new int[0], "速", "", "shoot", "BulletExplosionBlue", "wenyuan", 5, 2, 6);
            config[102019] = new HeroConfig(102019, "曹真", 1, 76, 80, 65, 80, 69, 80, 695,80, 1.5f, 45, 35, 2, 10, 17, 0, 0f, 52, 0, 1, "戟", new int[0], "境", "", "def", "SwordHitYellowCritical", "caozhen", 1, 3, 7);
            config[102020] = new HeroConfig(102020, "陈群", 1, 65, 80, 85, 80, 45, 80, 522,80, 1.5f, 25, 35, 2, 10, 40, 15, 0f, 29, 0, 3, "扇", new int[0], "励", "米", "help", "FanExplosion", "chenqun", 1, 2, 6);
            config[102021] = new HeroConfig(102021, "李典", 1, 77, 80, 73, 80, 75, 80, 750,80, 1.5f, 45, 35, 2, 10, 17, 0, 0f, 45, 0, 1, "枪", new int[0], "伏", "坚", "def", "SwordHitYellowCritical", "lidian", 0, 3, 8);
            config[102022] = new HeroConfig(102022, "刘晔", 1, 53, 80, 72, 80, 40, 80, 405,80, 1.5f, 15, 20, 2, 10, 40, 13, 2.5f, 34, 0, 3, "炮", new int[0], "", "", "shoot", "GasShootFire", "liuye", 1, 1, 4);
            config[103001] = new HeroConfig(103001, "孙策", 1, 87, 80, 68, 80, 85, 80, 820,80, 1.5f, 55, 45, 3, 10, 17, 0, 0f, 100, 0, 1, "车", new int[0], "虎", "", "atk", "SwordHitYellowCritical", "sunce", 3, 4, 9);
            config[103002] = new HeroConfig(103002, "孙坚", 1, 86, 80, 71, 80, 83, 80, 835,80, 1.5f, 55, 45, 3, 10, 17, 0, 0f, 95, 0, 1, "枪", new int[0], "旋", "", "atk", "SwordHitYellowCritical", "sunjian", 6, 4, 9);
            config[103003] = new HeroConfig(103003, "甘宁", 1, 69, 80, 56, 80, 70, 80, 484,80, 1.5f, 25, 30, 3, 10, 40, 22, 1.5f, 100, 0, 3, "弓", new int[0], "连", "", "shoot", "BulletExplosionBlue", "ganning", 2, 2, 6);
            config[103004] = new HeroConfig(103004, "太史慈", 1, 76, 80, 55, 80, 79, 80, 556,80, 1.5f, 35, 40, 3, 10, 40, 22, 1.5f, 77, 0, 3, "弓", new int[0], "雨", "", "shoot", "BulletExplosionBlue", "taishici", 2, 3, 7);
            config[103005] = new HeroConfig(103005, "黄盖", 1, 77, 80, 68, 80, 80, 80, 760,80, 1.5f, 45, 35, 3, 10, 17, 0, 0f, 59, 0, 1, "士", new int[0], "奋", "", "def", "SwordHitYellowCritical", "huanggai", 2, 3, 8);
            config[103006] = new HeroConfig(103006, "周泰", 1, 84, 80, 51, 80, 90, 80, 774,80, 1.5f, 45, 35, 3, 10, 17, 0, 0f, 52, 0, 1, "枪", new int[0], "连", "", "atk", "SwordHitYellowCritical", "zhoutai", 3, 3, 8);
            config[103007] = new HeroConfig(103007, "鲁肃", 1, 80, 80, 87, 80, 58, 80, 615,80, 1.5f, 35, 45, 3, 10, 40, 18, 0f, 64, 0, 3, "相", new int[0], "商", "雷", "help", "SharpExplosionGreen", "lusu", 2, 3, 8);
            config[103008] = new HeroConfig(103008, "周瑜", 1, 87, 80, 88, 80, 65, 80, 675,80, 1.5f, 45, 55, 3, 10, 40, 15, 0f, 100, 0, 3, "谋", new int[0], "炎", "炽", "inte", "ExplosionFireballFire", "zhouyu", 6, 4, 9);
            config[103009] = new HeroConfig(103009, "蒋钦", 1, 69, 80, 51, 80, 75, 80, 635,80, 1.5f, 35, 25, 3, 10, 17, 0, 0f, 45, 0, 1, "戟", new int[0], "", "", "atk", "SwordHitYellowCritical", "jiangqing", 1, 2, 6);
            config[103010] = new HeroConfig(103010, "吕蒙", 1, 84, 80, 83, 80, 73, 80, 815,80, 1.5f, 55, 45, 3, 10, 17, 0, 0f, 100, 0, 1, "马", new int[0], "学", "羽", "def", "SwordHitYellowCritical", "lvmeng", 3, 4, 9);
            config[103011] = new HeroConfig(103011, "陆逊", 1, 83, 80, 82, 80, 60, 80, 603,80, 1.5f, 35, 45, 3, 10, 40, 15, 0f, 93, 0, 3, "谋", new int[0], "炎", "", "inte", "GasExplosionFire", "luxun", 2, 3, 8);
            config[103012] = new HeroConfig(103012, "张昭", 1, 60, 80, 82, 80, 38, 80, 486,80, 1.5f, 25, 35, 3, 10, 40, 18, 0f, 30, 0, 3, "相", new int[0], "", "", "help", "SharpExplosionGreen", "zhangzhao", 1, 2, 5);
            config[103013] = new HeroConfig(103013, "诸葛瑾", 1, 72, 80, 81, 80, 42, 80, 531,80, 1.5f, 25, 35, 3, 10, 40, 15, 0f, 34, 0, 3, "扇", new int[0], "励", "", "help", "FanExplosion", "zhugejin", 1, 2, 6);
            config[103014] = new HeroConfig(103014, "孙尚香", 1, 60, 80, 55, 80, 65, 80, 471,80, 1.5f, 25, 30, 3, 10, 40, 22, 1.5f, 55, 0, 3, "弓", new int[0], "", "", "shoot", "BulletExplosionBlue", "sunshangxiang", 2, 2, 5);
            config[103015] = new HeroConfig(103015, "朱桓", 1, 83, 80, 76, 80, 81, 80, 850,80, 1.5f, 55, 45, 3, 10, 17, 0, 0f, 70, 0, 1, "枪", new int[0], "伏", "缓", "def", "SwordHitYellowCritical", "zhuhuan", 2, 4, 9);
            config[103016] = new HeroConfig(103016, "大乔", 1, 71, 80, 91, 80, 18, 80, 488,80, 1.5f, 25, 35, 3, 10, 40, 15, 0f, 25, 0, 3, "乐", new int[0], "碉", "陷", "help", "StormExplosion", "daqiao", 2, 2, 5);
            config[103017] = new HeroConfig(103017, "小乔", 1, 50, 80, 91, 80, 24, 80, 435,80, 1.5f, 15, 25, 3, 10, 40, 15, 0f, 25, 0, 3, "乐", new int[0], "曲", "陷", "help", "StormExplosion", "xiaoqiao", 2, 1, 4);
            config[103018] = new HeroConfig(103018, "丁奉", 1, 55, 80, 50, 80, 60, 80, 408,80, 1.5f, 15, 20, 3, 10, 40, 13, 2.5f, 52, 0, 3, "炮", new int[0], "", "", "shoot", "GasShootFire", "dingfeng", 1, 1, 4);
            config[103019] = new HeroConfig(103019, "凌统", 1, 61, 80, 49, 80, 70, 80, 457,80, 1.5f, 25, 30, 3, 10, 60, 30, 1.5f, 52, 0, 3, "弩", new int[0], "虐", "", "shoot", "BulletExplosionBlue", "lingtong", 1, 2, 5);
            config[103020] = new HeroConfig(103020, "潘璋", 1, 74, 80, 73, 80, 78, 80, 747,80, 1.5f, 45, 35, 3, 10, 17, 0, 0f, 55, 0, 1, "戟", new int[0], "刺", "虐", "def", "SwordHitYellowCritical", "panzhang", 1, 3, 8);
            config[103021] = new HeroConfig(103021, "徐盛", 1, 74, 80, 67, 80, 69, 80, 685,80, 1.5f, 45, 35, 3, 10, 17, 0, 0f, 73, 0, 1, "士", new int[0], "乱", "", "def", "SwordHitYellowCritical", "xusheng", 1, 3, 7);
            config[103022] = new HeroConfig(103022, "程普", 1, 78, 80, 73, 80, 74, 80, 750,80, 1.5f, 45, 35, 3, 10, 17, 0, 0f, 69, 0, 1, "戟", new int[0], "实", "奋", "def", "SwordHitYellowCritical", "chengpu", 1, 3, 8);
            config[104001] = new HeroConfig(104001, "吕布", 1, 103, 80, 46, 80, 106, 80, 929,80, 1.5f, 55, 45, 4, 10, 17, 0, 0f, 67, 0, 1, "车", new int[0], "魔", "羽", "atk", "SwordHitBlackRedCritical", "lvbu", 5, 4, 10);
            config[104002] = new HeroConfig(104002, "华雄", 1, 83, 80, 57, 80, 85, 80, 754,80, 1.5f, 45, 35, 4, 10, 17, 0, 0f, 64, 0, 1, "车", new int[0], "纷", "", "atk", "SwordHitYellowCritical", "huaxiong", 1, 3, 8);
            config[104003] = new HeroConfig(104003, "贾诩", 1, 83, 80, 94, 80, 48, 80, 620,80, 1.5f, 35, 45, 4, 10, 40, 15, 0f, 59, 0, 3, "谋", new int[0], "延", "", "inte", "StormExplosion", "jiaxu", 3, 3, 8);
            config[104004] = new HeroConfig(104004, "貂蝉", 1, 26, 80, 77, 80, 62, 80, 439,80, 1.5f, 15, 25, 4, 10, 40, 15, 0f, 25, 0, 3, "乐", new int[0], "曲", "", "help", "StormExplosion", "diaochan", 1, 1, 4);
            config[104005] = new HeroConfig(104005, "臧霸", 1, 74, 80, 50, 80, 71, 80, 645,80, 1.5f, 35, 25, 4, 10, 17, 0, 0f, 36, 0, 1, "马", new int[0], "虐", "", "atk", "SwordHitYellowCritical", "zangba", 1, 2, 6);
            config[104006] = new HeroConfig(104006, "高顺", 1, 65, 80, 48, 80, 67, 80, 460,80, 1.5f, 25, 30, 4, 10, 40, 13, 2.5f, 60, 0, 3, "炮", new int[0], "", "", "shoot", "GasShootFire", "gaoshun", 2, 2, 5);
            config[104007] = new HeroConfig(104007, "李儒", 1, 58, 80, 83, 80, 39, 80, 476,80, 1.5f, 25, 35, 4, 10, 40, 15, 0f, 31, 0, 3, "谋", new int[0], "火", "", "inte", "ShadowExplosion", "liru", 1, 2, 5);
            config[104008] = new HeroConfig(104008, "陈宫", 1, 77, 80, 82, 80, 51, 80, 569,80, 1.5f, 35, 45, 4, 10, 40, 15, 0f, 54, 0, 3, "谋", new int[0], "励", "溃", "inte", "ShadowExplosion", "chengong", 1, 3, 7);
            config[105001] = new HeroConfig(105001, "邓艾", 1, 84, 80, 79, 80, 77, 80, 838,80, 1.5f, 55, 45, 5, 10, 17, 0, 0f, 113, 0, 1, "枪", new int[0], "奇", "", "def", "SwordHitYellowCritical", "dengai", 2, 4, 9);
            config[105002] = new HeroConfig(105002, "司马师", 1, 67, 80, 72, 80, 56, 80, 527,80, 1.5f, 25, 35, 5, 10, 40, 18, 0f, 60, 0, 3, "相", new int[0], "", "", "help", "SharpExplosionGreen", "simashi", 1, 2, 6);
            config[105003] = new HeroConfig(105003, "司马昭", 1, 74, 80, 82, 80, 54, 80, 582,80, 1.5f, 35, 45, 5, 10, 40, 18, 0f, 48, 0, 3, "相", new int[0], "溃", "", "help", "SharpExplosionGreen", "simazhao", 2, 3, 7);
            config[105004] = new HeroConfig(105004, "羊祜", 1, 85, 80, 79, 80, 61, 80, 760,80, 1.5f, 45, 35, 5, 10, 17, 0, 0f, 64, 0, 1, "戟", new int[0], "敏", "", "atk", "SwordHitYellowCritical", "yangku", 2, 3, 8);
            config[105005] = new HeroConfig(105005, "钟会", 1, 74, 80, 84, 80, 52, 80, 582,80, 1.5f, 35, 45, 5, 10, 40, 15, 0f, 58, 0, 3, "谋", new int[0], "缓", "", "inte", "StormExplosion", "zhonghui", 4, 3, 7);
            config[105006] = new HeroConfig(105006, "陈泰", 1, 74, 80, 71, 80, 65, 80, 680,80, 1.5f, 45, 35, 5, 10, 17, 0, 0f, 75, 0, 1, "士", new int[0], "虐", "", "def", "SwordHitYellowCritical", "chentai", 1, 3, 7);
            config[105007] = new HeroConfig(105007, "杜预", 1, 82, 80, 84, 80, 29, 80, 544,80, 1.5f, 25, 35, 5, 10, 40, 18, 0f, 32, 0, 3, "相", new int[0], "米", "", "inte", "SharpExplosionGreen", "duyu", 2, 2, 6);
            config[106001] = new HeroConfig(106001, "颜良", 1, 83, 80, 39, 80, 88, 80, 710,80, 1.5f, 45, 35, 6, 10, 17, 0, 0f, 48, 0, 1, "车", new int[0], "破", "", "atk", "SwordHitYellowCritical", "yanliang", 1, 3, 7);
            config[106002] = new HeroConfig(106002, "文丑", 1, 87, 80, 47, 80, 91, 80, 777,80, 1.5f, 45, 35, 6, 10, 17, 0, 0f, 55, 0, 1, "车", new int[0], "刺", "", "def", "SwordHitYellowCritical", "wenchou", 1, 3, 8);
            config[106003] = new HeroConfig(106003, "田丰", 1, 71, 80, 92, 80, 32, 80, 514,80, 1.5f, 25, 35, 6, 10, 40, 15, 0f, 32, 0, 3, "谋", new int[0], "雷", "", "inte", "StormExplosion", "tianfeng", 1, 2, 6);
            config[106004] = new HeroConfig(106004, "鞠义", 1, 63, 80, 48, 80, 69, 80, 476,80, 1.5f, 25, 30, 6, 10, 40, 22, 1.5f, 36, 0, 3, "弓", new int[0], "", "", "shoot", "BulletExplosionBlue", "juyi", 0, 2, 5);
            config[106005] = new HeroConfig(106005, "许攸", 1, 43, 80, 90, 80, 32, 80, 442,80, 1.5f, 15, 25, 6, 10, 40, 15, 0f, 25, 0, 3, "谋", new int[0], "火", "", "inte", "StormExplosion", "xuyou", 1, 1, 4);
            config[106006] = new HeroConfig(106006, "高览", 1, 66, 80, 59, 80, 70, 80, 632,80, 1.5f, 35, 25, 6, 10, 17, 0, 0f, 52, 0, 1, "枪", new int[0], "", "", "atk", "SwordHitYellowCritical", "gaolan", 1, 2, 6);
            config[106007] = new HeroConfig(106007, "沮授", 1, 75, 80, 86, 80, 34, 80, 518,80, 1.5f, 25, 35, 6, 10, 40, 15, 0f, 35, 0, 3, "谋", new int[0], "静", "", "inte", "StormExplosion", "jushou", 1, 2, 6);
            config[106008] = new HeroConfig(106008, "郭图", 1, 55, 80, 87, 80, 53, 80, 531,80, 1.5f, 25, 35, 6, 10, 40, 15, 0f, 25, 0, 3, "扇", new int[0], "励", "米", "help", "FanExplosion", "guotu", 1, 2, 6);
            config[110001] = new HeroConfig(110001, "公孙瓒", 1, 72, 80, 66, 80, 72, 80, 687,80, 1.5f, 45, 35, 10, 10, 17, 0, 0f, 67, 0, 1, "马", new int[0], "乱", "", "def", "SwordHitYellowCritical", "gongsunzan", 2, 3, 7);
            config[110002] = new HeroConfig(110002, "张任", 1, 70, 80, 59, 80, 66, 80, 497,80, 1.5f, 25, 30, 10, 10, 40, 22, 1.5f, 75, 0, 3, "弓", new int[0], "复", "", "shoot", "BulletExplosionBlue", "zhangren", 1, 2, 6);
            config[110003] = new HeroConfig(110003, "华佗", 1, 68, 80, 88, 80, 39, 80, 548,80, 1.5f, 25, 35, 10, 10, 40, 14, 0f, 25, 0, 3, "医", new int[0], "药", "", "help", "ShadowExplosionGreen", "huatuo", 2, 2, 6);
            config[110004] = new HeroConfig(110004, "袁术", 1, 67, 80, 64, 80, 64, 80, 647,80, 1.5f, 35, 25, 10, 10, 17, 0, 0f, 31, 0, 1, "戟", new int[0], "", "", "def", "SwordHitYellowCritical", "yuanshu", 3, 2, 6);
            config[110005] = new HeroConfig(110005, "马腾", 1, 75, 80, 47, 80, 73, 80, 625,80, 1.5f, 35, 25, 10, 10, 17, 0, 0f, 41, 0, 1, "马", new int[0], "羽", "", "atk", "SwordHitYellowCritical", "mateng", 3, 2, 6);
            config[110006] = new HeroConfig(110006, "于吉", 1, 53, 80, 81, 80, 46, 80, 501,80, 1.5f, 25, 35, 10, 10, 40, 14, 0f, 25, 0, 3, "医", new int[0], "调", "", "help", "ShadowExplosionGreen", "yuji", 0, 2, 5);
            config[110007] = new HeroConfig(110007, "张角", 1, 97, 80, 96, 80, 32, 80, 629,80, 1.5f, 35, 45, 10, 10, 40, 15, 0f, 34, 0, 3, "谋", new int[0], "天", "陷", "inte", "LightningExplosionBlue", "zhangjiao", 1, 3, 8);
            config[110008] = new HeroConfig(110008, "张宝", 1, 75, 80, 72, 80, 63, 80, 680,80, 1.5f, 45, 35, 10, 10, 17, 0, 0f, 61, 0, 1, "枪", new int[0], "劫", "", "atk", "SwordHitYellowCritical", "zhangbao2", 1, 3, 7);
            config[110009] = new HeroConfig(110009, "张梁", 1, 61, 80, 57, 80, 62, 80, 465,80, 1.5f, 25, 30, 10, 10, 40, 13, 2.5f, 58, 0, 3, "炮", new int[0], "", "", "def", "SwordHitYellowCritical", "zhangliang", 1, 2, 5);

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
