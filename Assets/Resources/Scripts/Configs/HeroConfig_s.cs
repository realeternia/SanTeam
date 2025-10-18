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
        ///攻击力
        /// </summary>
        public int Atk;
        /// <summary>
        ///统帅
        /// </summary>
        public int LeadShip;
        /// <summary>
        ///智力
        /// </summary>
        public int Inte;
        /// <summary>
        ///武力
        /// </summary>
        public int Str;
        /// <summary>
        ///总属性
        /// </summary>
        public int Total;
        /// <summary>
        ///生命
        /// </summary>
        public int Hp;
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


        public HeroConfig(int Id, string Name, int Lv, int Atk, int LeadShip, int Inte, int Str, int Total, int Hp, int Side, int MoveSpeed, int Range, int MissileSpeed, float MissileHight, int RateWeight, int RateAbs, int Pos, string Job, int[] Skills, string Skill1, string Skill2, string Group, string HitEffect, string Icon, int FriendCount)
        {
            this.Id = Id;
            this.Name = Name;
            this.Lv = Lv;
            this.Atk = Atk;
            this.LeadShip = LeadShip;
            this.Inte = Inte;
            this.Str = Str;
            this.Total = Total;
            this.Hp = Hp;
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

        }

        public HeroConfig() { }

        private static Dictionary<int, HeroConfig> config = new Dictionary<int, HeroConfig>();
        public static Dictionary<int, HeroConfig>.ValueCollection ConfigList
        {
            get
            {
                return config.Values;
            }
        }

        public static void Refresh(Dictionary<int, HeroConfig> dict)
        {
            config.Clear();
            config = dict;
        }

        public static void Load()
        {
            config.Clear();
            config[100001] = new HeroConfig(100001, "刘备", 1, 26, 80, 78, 77, 235, 330, 1, 10, 17, 0, 0, 0, 100, 1, "帅", null, "仁", "", "core", "SwordHitYellowCritical", "liubei", 6);
            config[100002] = new HeroConfig(100002, "曹操", 1, 32, 98, 92, 81, 271, 240, 2, 10, 17, 0, 0, 0, 100, 1, "帅", null, "识", "", "core", "SwordHitYellowCritical", "caocao", 6);
            config[100003] = new HeroConfig(100003, "孙权", 1, 26, 79, 80, 70, 229, 350, 3, 10, 17, 0, 0, 0, 100, 1, "帅", null, "衡", "", "core", "SwordHitYellowCritical", "sunquan", 3);
            config[100004] = new HeroConfig(100004, "董卓", 1, 28, 85, 75, 87, 247, 330, 4, 10, 17, 0, 0, 0, 0, 1, "帅", null, "", "", "core", "SwordHitYellowCritical", "dongzhuo", 2);
            config[100005] = new HeroConfig(100005, "司马炎", 1, 23, 69, 77, 59, 205, 385, 5, 10, 17, 0, 0, 0, 0, 1, "帅", null, "", "", "core", "SwordHitYellowCritical", "simayan", 2);
            config[100006] = new HeroConfig(100006, "袁绍", 1, 28, 86, 82, 73, 241, 310, 6, 10, 17, 0, 0, 0, 0, 1, "帅", null, "", "", "core", "SwordHitYellowCritical", "yuanshao", 2);
            config[101001] = new HeroConfig(101001, "赵云", 1, 30, 91, 76, 96, 263, 300, 1, 10, 17, 0, 0, 100, 0, 1, "马", null, "镜", "羽", "def", "SwordHitWhiteCritical", "zhaoyun", 4);
            config[101002] = new HeroConfig(101002, "张飞", 1, 30, 92, 48, 98, 238, 310, 1, 10, 17, 0, 0, 64, 0, 1, "枪", null, "威", "", "atk", "SwordHitYellowCritical", "zhangfei", 4);
            config[101003] = new HeroConfig(101003, "马超", 1, 30, 92, 54, 97, 243, 335, 1, 10, 17, 0, 0, 70, 0, 1, "马", null, "铁", "", "atk", "SwordHitWhiteCritical", "machao", 4);
            config[101004] = new HeroConfig(101004, "诸葛亮", 1, 32, 98, 100, 45, 243, 245, 1, 10, 17, 18, 0, 70, 0, 3, "谋", null, "神", "空", "inte", "LightningExplosionYellow", "zhugeliang", 6);
            config[101005] = new HeroConfig(101005, "关羽", 1, 32, 97, 77, 97, 271, 260, 1, 10, 17, 0, 0, 115, 0, 1, "车", null, "斩", "", "atk", "SwordHitGreenCritical", "guanyu", 7);
            config[101006] = new HeroConfig(101006, "徐庶", 1, 29, 87, 93, 65, 245, 230, 1, 10, 17, 15, 0, 73, 0, 3, "谋", null, "火", "共", "inte", "GasExplosionFire", "xusu", 1);
            config[101007] = new HeroConfig(101007, "魏延", 1, 28, 84, 67, 91, 242, 300, 1, 10, 17, 0, 0, 69, 0, 1, "戟", null, "破", "乱", "atk", "SwordHitYellowCritical", "weiyan", 1);
            config[101008] = new HeroConfig(101008, "黄忠", 1, 30, 90, 64, 93, 247, 210, 1, 10, 17, 22, 1.5f, 75, 0, 3, "弓", null, "矢", "速", "shoot", "BulletExplosionFire", "huangzhong", 3);
            config[101009] = new HeroConfig(101009, "周仓", 1, 21, 63, 55, 82, 200, 335, 1, 10, 17, 0, 0, 33, 0, 1, "刀", null, "劫", "", "atk", "SwordHitYellowCritical", "zhoucang", 1);
            config[101010] = new HeroConfig(101010, "姜维", 1, 30, 92, 90, 89, 271, 295, 1, 10, 17, 0, 0, 115, 0, 1, "车", null, "解", "", "def", "SwordHitYellowCritical", "jiangwei", 3);
            config[101011] = new HeroConfig(101011, "马岱", 1, 26, 80, 66, 85, 231, 300, 1, 10, 17, 0, 0, 57, 0, 1, "马", null, "坚", "羽", "atk", "SwordHitYellowCritical", "madai", 2);
            config[101012] = new HeroConfig(101012, "关兴", 1, 26, 80, 72, 85, 237, 300, 1, 10, 17, 0, 0, 63, 0, 1, "马", null, "奋", "", "atk", "SwordHitYellowCritical", "guanxing", 2);
            config[101013] = new HeroConfig(101013, "关平", 1, 26, 79, 72, 82, 233, 290, 1, 10, 17, 0, 0, 59, 0, 1, "戟", null, "连", "", "def", "SwordHitYellowCritical", "guanping", 1);
            config[101014] = new HeroConfig(101014, "严颜", 1, 26, 79, 71, 83, 233, 280, 1, 10, 17, 0, 0, 59, 0, 1, "士", null, "敏", "", "def", "SwordHitYellowCritical", "yanyan", 1);
            config[101015] = new HeroConfig(101015, "廖化", 1, 24, 74, 68, 78, 220, 330, 1, 10, 17, 0, 0, 47, 0, 1, "刀", null, "透", "", "atk", "SwordHitYellowCritical", "liaohua", 1);
            config[101016] = new HeroConfig(101016, "孟获", 1, 29, 87, 51, 87, 225, 310, 1, 10, 17, 0, 0, 51, 0, 1, "刀", null, "藤", "", "atk", "SwordHitYellowCritical", "menghuo", 1);
            config[101017] = new HeroConfig(101017, "庞统", 1, 28, 85, 98, 47, 230, 250, 1, 10, 17, 15, 0, 56, 0, 3, "谋", null, "锁", "火", "inte", "ExplosionFireballFire", "pangtong", 1);
            config[101018] = new HeroConfig(101018, "李严", 1, 27, 82, 75, 83, 240, 290, 1, 10, 17, 0, 0, 67, 0, 1, "士", null, "实", "", "def", "SwordHitYellowCritical", "liyan", 1);
            config[101019] = new HeroConfig(101019, "张苞", 1, 25, 75, 82, 87, 244, 305, 1, 10, 17, 0, 0, 71, 0, 1, "枪", null, "乱", "", "atk", "SwordHitYellowCritical", "zhangbao", 2);
            config[101020] = new HeroConfig(101020, "张松", 1, 21, 65, 89, 46, 200, 240, 1, 10, 17, 15, 0, 33, 0, 3, "扇", null, "", "", "inte", "FanExplosion", "zhangsong", 1);
            config[101021] = new HeroConfig(101021, "关索", 1, 24, 74, 69, 81, 224, 215, 1, 10, 17, 30, 1.5f, 50, 0, 3, "弩", null, "", "", "shoot", "BulletExplosionBlue", "guansuo", 1);
            config[101022] = new HeroConfig(101022, "简雍", 1, 24, 72, 69, 65, 206, 240, 1, 10, 17, 22, 1.5f, 36, 0, 3, "弓", null, "破", "", "shoot", "BulletExplosionBlue", "jianyong", 1);
            config[101023] = new HeroConfig(101023, "蒋琬", 1, 21, 64, 85, 52, 201, 240, 1, 10, 17, 18, 0, 33, 0, 3, "相", null, "", "", "help", "SharpExplosionGreen", "jiangwan", 1);
            config[101024] = new HeroConfig(101024, "孙乾", 1, 20, 62, 80, 54, 196, 250, 1, 10, 17, 35, 0, 31, 0, 3, "鼓", null, "白", "", "help", "SoulExplosionOrange", "sunqian", 1);
            config[101025] = new HeroConfig(101025, "费祎", 1, 22, 68, 86, 42, 196, 250, 1, 10, 17, 18, 0, 31, 0, 3, "相", null, "励", "", "help", "SharpExplosionGreen", "feiyi", 1);
            config[101026] = new HeroConfig(101026, "马谡", 1, 23, 70, 88, 72, 230, 235, 1, 10, 17, 15, 0, 56, 0, 3, "谋", null, "百", "", "inte", "StormExplosion", "masu", 1);
            config[101027] = new HeroConfig(101027, "董允", 1, 22, 67, 85, 65, 217, 280, 1, 10, 17, 15, 0, 44, 0, 3, "扇", null, "米", "", "inte", "FanExplosion", "dongyun", 1);
            config[101028] = new HeroConfig(101028, "王平", 1, 27, 83, 75, 78, 236, 300, 1, 10, 17, 0, 0, 62, 0, 1, "戟", null, "伏", "坚", "def", "SwordHitYellowCritical", "wangping", 1);
            config[101029] = new HeroConfig(101029, "邓芝", 1, 23, 70, 80, 71, 221, 290, 1, 10, 17, 0, 0, 48, 0, 1, "枪", null, "境", "", "def", "SwordHitYellowCritical", "dengzhi", 0);
            config[101030] = new HeroConfig(101030, "郭攸之", 1, 21, 63, 82, 48, 193, 260, 1, 10, 17, 35, 0, 29, 0, 3, "鼓", null, "陷", "", "help", "SoulExplosionOrange", "guoyouzhi", 1);
            config[101031] = new HeroConfig(101031, "马良", 1, 22, 68, 93, 60, 221, 270, 1, 10, 17, 18, 0, 48, 0, 3, "相", null, "静", "", "help", "SharpExplosionGreen", "maliang", 1);
            config[101032] = new HeroConfig(101032, "黄月英", 1, 19, 58, 86, 34, 178, 190, 1, 10, 17, 13, 2.5f, 25, 0, 3, "炮", null, "", "", "shoot", "GasShootFire", "huangyueying", 1);
            config[101033] = new HeroConfig(101033, "法正", 1, 27, 83, 94, 52, 229, 275, 1, 10, 17, 15, 0, 55, 0, 3, "谋", null, "溃", "", "inte", "GasExplosionFire", "fazheng", 2);
            config[101034] = new HeroConfig(101034, "刘禅", 1, 9, 27, 35, 41, 103, 360, 1, 10, 17, 35, 0, 25, 0, 3, "鼓", null, "碉", "", "help", "SoulExplosionOrange", "liushan", 3);
            config[101035] = new HeroConfig(101035, "祝融", 1, 25, 77, 43, 85, 205, 290, 1, 10, 17, 0, 0, 36, 0, 1, "刀", null, "藤", "", "def", "SwordHitYellowCritical", "zhurong", 1);
            config[101036] = new HeroConfig(101036, "黄权", 1, 25, 75, 82, 59, 216, 310, 1, 10, 17, 0, 0, 44, 0, 1, "枪", null, "缓", "", "atk", "SwordHitYellowCritical", "huangquan", 1);
            config[101037] = new HeroConfig(101037, "孟达", 1, 25, 75, 68, 73, 216, 250, 1, 10, 17, 22, 1.5f, 44, 0, 3, "弓", null, "乱", "", "shoot", "BulletExplosionBlue", "mengda", 0);
            config[101038] = new HeroConfig(101038, "刘封", 1, 25, 75, 44, 79, 198, 220, 1, 10, 17, 30, 1.5f, 32, 0, 3, "弩", null, "", "", "shoot", "BulletExplosionBlue", "liufeng", 1);
            config[101039] = new HeroConfig(101039, "李恢", 1, 26, 79, 79, 65, 223, 315, 1, 10, 17, 0, 0, 49, 0, 1, "戟", null, "境", "", "def", "SwordHitYellowCritical", "lihui", 0);
            config[101040] = new HeroConfig(101040, "刘巴", 1, 11, 33, 78, 24, 135, 290, 1, 10, 17, 15, 0, 25, 0, 3, "扇", null, "纷", "", "inte", "FanExplosion", "liuba", 0);
            config[102001] = new HeroConfig(102001, "郭嘉", 1, 24, 72, 98, 43, 213, 270, 2, 10, 17, 15, 0, 41, 0, 3, "谋", null, "天", "", "inte", "LightningExplosionBlue", "guojia", 2);
            config[102002] = new HeroConfig(102002, "夏侯惇", 1, 30, 91, 63, 91, 245, 305, 2, 10, 17, 0, 0, 73, 0, 1, "车", null, "青", "", "atk", "SwordHitYellowCritical", "xiahoudun", 2);
            config[102003] = new HeroConfig(102003, "荀彧", 1, 22, 67, 96, 47, 210, 285, 2, 10, 17, 18, 0, 39, 0, 3, "相", null, "国", "", "help", "FrostExplosionBlue", "xunyu", 3);
            config[102004] = new HeroConfig(102004, "张辽", 1, 31, 95, 78, 92, 265, 295, 2, 10, 17, 0, 0, 103, 0, 1, "马", null, "旋", "", "def", "SwordHitYellowCritical", "zhangliao", 5);
            config[102005] = new HeroConfig(102005, "许褚", 1, 21, 65, 44, 96, 205, 390, 2, 10, 17, 0, 0, 36, 0, 1, "士", null, "斧", "", "atk", "SwordHitYellowCritical", "xuchu", 2);
            config[102006] = new HeroConfig(102006, "夏侯渊", 1, 30, 90, 68, 89, 247, 230, 2, 10, 17, 22, 1.5f, 75, 0, 3, "弓", null, "雨", "", "shoot", "BulletExplosionBlue", "xiahouyuan", 2);
            config[102007] = new HeroConfig(102007, "典韦", 1, 19, 59, 43, 95, 197, 415, 2, 10, 17, 0, 0, 31, 0, 1, "士", null, "护", "", "def", "SwordHitYellowCritical", "dianwei", 1);
            config[102008] = new HeroConfig(102008, "张郃", 1, 29, 89, 75, 90, 254, 300, 2, 10, 17, 0, 0, 85, 0, 1, "车", null, "分", "", "def", "SwordHitYellowCritical", "zhanghe", 3);
            config[102009] = new HeroConfig(102009, "徐晃", 1, 30, 90, 77, 91, 258, 215, 2, 10, 17, 22, 1.5f, 91, 0, 3, "弓", null, "连", "", "shoot", "BulletExplosionBlue", "xuhuang", 2);
            config[102010] = new HeroConfig(102010, "荀攸", 1, 21, 63, 92, 53, 208, 270, 2, 10, 17, 15, 0, 38, 0, 3, "谋", null, "百", "米", "inte", "FrostExplosionBlue", "xunyou", 2);
            config[102011] = new HeroConfig(102011, "于禁", 1, 26, 80, 72, 75, 227, 280, 2, 10, 17, 0, 0, 53, 0, 1, "戟", null, "青", "破", "def", "SwordHitYellowCritical", "yujin", 2);
            config[102012] = new HeroConfig(102012, "曹仁", 1, 30, 90, 62, 86, 238, 300, 2, 10, 17, 0, 0, 64, 0, 1, "枪", null, "青", "", "atk", "SwordHitYellowCritical", "caoren", 2);
            config[102013] = new HeroConfig(102013, "曹洪", 1, 27, 82, 51, 83, 216, 310, 2, 10, 17, 0, 0, 44, 0, 1, "枪", null, "商", "", "atk", "SwordHitYellowCritical", "caohong", 1);
            config[102014] = new HeroConfig(102014, "庞德", 1, 29, 89, 74, 94, 257, 280, 2, 10, 17, 0, 0, 90, 0, 1, "枪", null, "坚", "", "atk", "SwordHitYellowCritical", "pangde", 2);
            config[102015] = new HeroConfig(102015, "乐进", 1, 26, 80, 55, 84, 219, 280, 2, 10, 17, 0, 0, 46, 0, 1, "戟", null, "奋", "", "atk", "SwordHitYellowCritical", "lejin", 2);
            config[102016] = new HeroConfig(102016, "文聘", 1, 26, 80, 65, 82, 227, 335, 2, 10, 17, 0, 0, 53, 0, 1, "戟", null, "透", "劫", "atk", "SwordHitYellowCritical", "wenpin", 1);
            config[102017] = new HeroConfig(102017, "曹休", 1, 24, 73, 72, 73, 218, 250, 2, 10, 17, 22, 1.5f, 45, 0, 3, "弓", null, "", "", "shoot", "BulletExplosionBlue", "caoxiu", 1);
            config[102018] = new HeroConfig(102018, "司马懿", 1, 32, 98, 98, 63, 259, 220, 2, 10, 17, 15, 0, 93, 0, 3, "谋", null, "鬼", "", "inte", "ShadowExplosion", "simayi", 5);
            config[102019] = new HeroConfig(102019, "夏侯霸", 1, 27, 82, 69, 77, 228, 320, 2, 10, 17, 0, 0, 54, 0, 1, "戟", null, "连", "", "atk", "SwordHitYellowCritical", "xiahouba", 1);
            config[102020] = new HeroConfig(102020, "郝昭", 1, 29, 87, 76, 79, 242, 300, 2, 10, 17, 0, 0, 69, 0, 1, "枪", null, "坚", "", "def", "SwordHitYellowCritical", "haozhao", 1);
            config[102021] = new HeroConfig(102021, "王双", 1, 22, 68, 38, 88, 194, 380, 2, 10, 17, 0, 0, 29, 0, 1, "枪", null, "透", "", "atk", "SwordHitYellowCritical", "wangshuang", 0);
            config[102022] = new HeroConfig(102022, "程昱", 1, 21, 63, 90, 54, 207, 240, 2, 10, 17, 15, 0, 37, 0, 3, "谋", null, "识", "火", "inte", "StormExplosion", "chengyu", 1);
            config[102023] = new HeroConfig(102023, "杨修", 1, 20, 60, 88, 57, 205, 260, 2, 10, 17, 18, 0, 36, 0, 3, "相", null, "虐", "", "help", "SharpExplosionGreen", "yangxiu", 0);
            config[102024] = new HeroConfig(102024, "牛金", 1, 23, 71, 65, 77, 213, 330, 2, 10, 17, 0, 0, 41, 0, 1, "刀", null, "伏", "", "atk", "SwordHitYellowCritical", "niujin", 1);
            config[102025] = new HeroConfig(102025, "文鸯", 1, 25, 76, 64, 91, 231, 240, 2, 10, 17, 22, 1.5f, 57, 0, 3, "弓", null, "速", "", "shoot", "BulletExplosionBlue", "wenyuan", 1);
            config[102026] = new HeroConfig(102026, "曹真", 1, 27, 82, 70, 74, 226, 310, 2, 10, 17, 0, 0, 52, 0, 1, "戟", null, "境", "", "def", "SwordHitYellowCritical", "caozhen", 1);
            config[102027] = new HeroConfig(102027, "陈群", 1, 21, 65, 84, 45, 194, 270, 2, 10, 17, 15, 0, 29, 0, 3, "扇", null, "励", "米", "help", "FanExplosion", "chenqun", 3);
            config[102028] = new HeroConfig(102028, "李典", 1, 24, 74, 71, 73, 218, 300, 2, 10, 17, 0, 0, 45, 0, 1, "枪", null, "伏", "坚", "def", "SwordHitYellowCritical", "lidian", 2);
            config[102029] = new HeroConfig(102029, "曹丕", 1, 26, 78, 79, 79, 236, 320, 2, 10, 17, 0, 0, 62, 0, 1, "刀", null, "敏", "", "def", "SwordHitYellowCritical", "caopi", 2);
            config[102030] = new HeroConfig(102030, "曹植", 1, 21, 64, 83, 67, 214, 300, 2, 10, 17, 15, 0, 42, 0, 3, "扇", null, "虐", "", "inte", "FanExplosion", "caozhi", 2);
            config[102031] = new HeroConfig(102031, "刘晔", 1, 21, 65, 88, 49, 202, 200, 2, 10, 17, 13, 2.5f, 34, 0, 3, "炮", null, "", "", "shoot", "GasShootFire", "liuye", 1);
            config[102032] = new HeroConfig(102032, "朱灵", 1, 24, 73, 60, 77, 210, 210, 2, 10, 17, 13, 2.5f, 39, 0, 3, "炮", null, "", "", "atk", "SwordHitYellowCritical", "zhuling", 0);
            config[102033] = new HeroConfig(102033, "曹彰", 1, 27, 82, 43, 90, 215, 320, 2, 10, 17, 0, 0, 43, 0, 1, "枪", null, "青", "", "atk", "SwordHitYellowCritical", "caozhang", 2);
            config[102034] = new HeroConfig(102034, "毌丘俭", 1, 26, 78, 55, 75, 208, 340, 2, 10, 17, 0, 0, 38, 0, 1, "刀", null, "劫", "", "atk", "SwordHitYellowCritical", "guanqiujian", 1);
            config[102035] = new HeroConfig(102035, "郭淮", 1, 29, 87, 71, 78, 236, 320, 2, 10, 17, 0, 0, 62, 0, 1, "士", null, "", "", "def", "SwordHitYellowCritical", "guohuai", 1);
            config[102036] = new HeroConfig(102036, "夏侯尚", 1, 26, 79, 72, 75, 226, 315, 2, 10, 17, 0, 0, 52, 0, 1, "戟", null, "敏", "", "def", "SwordHitYellowCritical", "xiahoushang", 1);
            config[102037] = new HeroConfig(102037, "钟繇", 1, 23, 70, 77, 37, 184, 290, 2, 10, 17, 18, 0, 25, 0, 3, "相", null, "米", "", "help", "SharpExplosionGreen", "zhongyao", 1);
            config[102038] = new HeroConfig(102038, "满宠", 1, 28, 84, 82, 64, 230, 280, 2, 10, 17, 0, 0, 56, 0, 1, "戟", null, "连", "", "atk", "SwordHitYellowCritical", "manchong", 0);
            config[102039] = new HeroConfig(102039, "曹冲", 1, 10, 31, 85, 21, 137, 260, 2, 10, 17, 15, 0, 25, 0, 3, "扇", null, "米", "", "inte", "FanExplosion", "caochong", 2);
            config[102040] = new HeroConfig(102040, "蒋济", 1, 16, 48, 85, 43, 176, 310, 2, 10, 17, 35, 0, 25, 0, 3, "鼓", null, "米", "", "help", "SoulExplosionOrange", "jiangji", 0);
            config[102041] = new HeroConfig(102041, "甄宓", 1, 18, 55, 74, 17, 146, 290, 2, 10, 17, 35, 0, 25, 0, 3, "鼓", null, "白", "", "help", "SoulExplosionOrange", "zhenshi", 0);
            config[102042] = new HeroConfig(102042, "戏志才", 1, 22, 66, 88, 24, 178, 270, 2, 10, 17, 15, 0, 25, 0, 3, "谋", null, "陷", "", "inte", "StormExplosion", "xizhicai", 0);
            config[103001] = new HeroConfig(103001, "孙坚", 1, 31, 93, 77, 90, 260, 290, 3, 10, 17, 0, 0, 95, 0, 1, "枪", null, "旋", "", "atk", "SwordHitYellowCritical", "sunjian", 3);
            config[103002] = new HeroConfig(103002, "孙策", 1, 32, 96, 74, 93, 263, 260, 3, 10, 17, 0, 0, 100, 0, 1, "车", null, "虎", "", "atk", "SwordHitYellowCritical", "sunce", 6);
            config[103003] = new HeroConfig(103003, "甘宁", 1, 31, 93, 76, 94, 263, 180, 3, 10, 17, 22, 1.5f, 100, 0, 3, "弓", null, "连", "", "shoot", "BulletExplosionBlue", "ganning", 2);
            config[103004] = new HeroConfig(103004, "太史慈", 1, 30, 90, 65, 93, 248, 230, 3, 10, 17, 22, 1.5f, 77, 0, 3, "弓", null, "雨", "", "shoot", "BulletExplosionBlue", "taishici", 2);
            config[103005] = new HeroConfig(103005, "黄盖", 1, 26, 80, 70, 83, 233, 320, 3, 10, 17, 0, 0, 59, 0, 1, "士", null, "奋", "", "def", "SwordHitYellowCritical", "huanggai", 2);
            config[103006] = new HeroConfig(103006, "周泰", 1, 28, 84, 51, 91, 226, 350, 3, 10, 17, 0, 0, 52, 0, 1, "枪", null, "连", "", "atk", "SwordHitYellowCritical", "zhoutai", 3);
            config[103007] = new HeroConfig(103007, "鲁肃", 1, 28, 85, 92, 61, 238, 250, 3, 10, 17, 18, 0, 64, 0, 3, "相", null, "商", "雷", "help", "SharpExplosionGreen", "lusu", 2);
            config[103008] = new HeroConfig(103008, "周瑜", 1, 32, 96, 96, 71, 263, 210, 3, 10, 17, 15, 0, 100, 0, 3, "谋", null, "炎", "炽", "inte", "ExplosionFireballFire", "zhouyu", 6);
            config[103009] = new HeroConfig(103009, "蒋钦", 1, 25, 77, 57, 84, 218, 310, 3, 10, 17, 0, 0, 45, 0, 1, "戟", null, "", "", "atk", "SwordHitYellowCritical", "jiangqing", 1);
            config[103010] = new HeroConfig(103010, "吕蒙", 1, 30, 92, 91, 80, 263, 250, 3, 10, 17, 0, 0, 100, 0, 1, "马", null, "学", "羽", "def", "SwordHitYellowCritical", "lvmeng", 3);
            config[103011] = new HeroConfig(103011, "陆逊", 1, 32, 96, 94, 69, 259, 220, 3, 10, 17, 15, 0, 93, 0, 3, "谋", null, "炎", "", "inte", "GasExplosionFire", "luxun", 2);
            config[103012] = new HeroConfig(103012, "张昭", 1, 21, 65, 89, 41, 195, 275, 3, 10, 17, 18, 0, 30, 0, 3, "相", null, "", "", "help", "SharpExplosionGreen", "zhangzhao", 1);
            config[103013] = new HeroConfig(103013, "诸葛瑾", 1, 25, 75, 83, 44, 202, 290, 3, 10, 17, 15, 0, 34, 0, 3, "扇", null, "励", "", "help", "FanExplosion", "zhugejin", 1);
            config[103014] = new HeroConfig(103014, "孙尚香", 1, 25, 76, 70, 83, 229, 240, 3, 10, 17, 22, 1.5f, 55, 0, 3, "弓", null, "", "", "shoot", "BulletExplosionBlue", "sunshangxiang", 2);
            config[103015] = new HeroConfig(103015, "朱桓", 1, 28, 84, 77, 82, 243, 320, 3, 10, 17, 0, 0, 70, 0, 1, "枪", null, "伏", "缓", "def", "SwordHitYellowCritical", "zhuhuan", 2);
            config[103016] = new HeroConfig(103016, "大乔", 1, 19, 57, 73, 14, 144, 280, 3, 10, 17, 15, 0, 25, 0, 3, "乐", null, "碉", "陷", "help", "StormExplosion", "daqiao", 2);
            config[103017] = new HeroConfig(103017, "小乔", 1, 13, 40, 74, 19, 133, 270, 3, 10, 17, 15, 0, 25, 0, 3, "乐", null, "曲", "陷", "help", "StormExplosion", "xiaoqiao", 2);
            config[103018] = new HeroConfig(103018, "丁奉", 1, 25, 76, 68, 82, 226, 205, 3, 10, 17, 13, 2.5f, 52, 0, 3, "炮", null, "", "", "shoot", "GasShootFire", "dingfeng", 1);
            config[103019] = new HeroConfig(103019, "董袭", 1, 24, 74, 60, 80, 214, 325, 3, 10, 17, 0, 0, 42, 0, 1, "刀", null, "透", "", "atk", "SwordHitYellowCritical", "dongxi", 1);
            config[103020] = new HeroConfig(103020, "凌统", 1, 25, 77, 62, 87, 226, 205, 3, 10, 17, 30, 1.5f, 52, 0, 3, "弩", null, "虐", "", "shoot", "BulletExplosionBlue", "lingtong", 1);
            config[103021] = new HeroConfig(103021, "潘璋", 1, 25, 75, 74, 80, 229, 295, 3, 10, 17, 0, 0, 55, 0, 1, "戟", null, "刺", "虐", "def", "SwordHitYellowCritical", "panzhang", 1);
            config[103022] = new HeroConfig(103022, "朱治", 1, 24, 73, 59, 72, 204, 235, 3, 10, 17, 22, 1.5f, 35, 0, 3, "弓", null, "敏", "", "shoot", "BulletExplosionBlue", "zhuzhi", 1);
            config[103023] = new HeroConfig(103023, "徐盛", 1, 28, 86, 78, 81, 245, 290, 3, 10, 17, 0, 0, 73, 0, 1, "士", null, "乱", "", "def", "SwordHitYellowCritical", "xusheng", 1);
            config[103024] = new HeroConfig(103024, "程普", 1, 28, 84, 78, 80, 242, 300, 3, 10, 17, 0, 0, 69, 0, 1, "戟", null, "实", "奋", "def", "SwordHitYellowCritical", "chengpu", 1);
            config[103025] = new HeroConfig(103025, "张纮", 1, 22, 68, 85, 50, 203, 250, 3, 10, 17, 18, 0, 35, 0, 3, "相", null, "励", "", "help", "SharpExplosionGreen", "zhanghong", 1);
            config[103026] = new HeroConfig(103026, "顾雍", 1, 21, 65, 81, 60, 206, 300, 3, 10, 17, 15, 0, 36, 0, 3, "扇", null, "连", "", "inte", "FanExplosion", "guyong", 1);
            config[103027] = new HeroConfig(103027, "步骘", 1, 23, 70, 80, 65, 215, 315, 3, 10, 17, 35, 0, 43, 0, 3, "鼓", null, "励", "", "help", "SoulExplosionOrange", "buzhi", 1);
            config[103028] = new HeroConfig(103028, "阚泽", 1, 20, 62, 84, 55, 201, 260, 3, 10, 17, 15, 0, 33, 0, 3, "谋", null, "炽", "", "inte", "StormExplosion", "kanze", 1);
            config[103029] = new HeroConfig(103029, "韩当", 1, 25, 76, 62, 85, 223, 300, 3, 10, 17, 0, 0, 49, 0, 1, "戟", null, "奋", "", "atk", "SwordHitYellowCritical", "handang", 1);
            config[103030] = new HeroConfig(103030, "陆抗", 1, 30, 91, 87, 63, 241, 195, 3, 10, 17, 30, 1.5f, 68, 0, 3, "弩", null, "透", "", "shoot", "BulletExplosionBlue", "lukang", 1);
            config[103031] = new HeroConfig(103031, "诸葛恪", 1, 24, 72, 90, 47, 209, 235, 3, 10, 17, 15, 0, 38, 0, 3, "谋", null, "缓", "", "inte", "StormExplosion", "zhugege", 2);
            config[103032] = new HeroConfig(103032, "苏飞", 1, 23, 69, 66, 63, 198, 330, 3, 10, 17, 0, 0, 32, 0, 1, "刀", null, "复", "", "def", "SwordHitYellowCritical", "sufei", 0);
            config[103033] = new HeroConfig(103033, "全琮", 1, 26, 78, 75, 72, 225, 310, 3, 10, 17, 0, 0, 51, 0, 1, "戟", null, "实", "", "def", "SwordHitYellowCritical", "quanzong", 0);
            config[103034] = new HeroConfig(103034, "陈武", 1, 25, 76, 47, 87, 210, 305, 3, 10, 17, 0, 0, 39, 0, 1, "枪", null, "劫", "", "def", "SwordHitYellowCritical", "chengwu", 1);
            config[103035] = new HeroConfig(103035, "朱然", 1, 26, 79, 71, 69, 219, 315, 3, 10, 17, 0, 0, 46, 0, 1, "枪", null, "竟", "", "def", "SwordHitYellowCritical", "zhuran", 2);
            config[103036] = new HeroConfig(103036, "孙韶", 1, 26, 80, 76, 79, 235, 310, 3, 10, 17, 0, 0, 61, 0, 1, "戟", null, "破", "", "def", "SwordHitYellowCritical", "sunshao", 1);
            config[103037] = new HeroConfig(103037, "孙桓", 1, 27, 82, 76, 73, 231, 315, 3, 10, 17, 0, 0, 57, 0, 1, "戟", null, "竟", "", "def", "SwordHitYellowCritical", "sunhuan", 1);
            config[103038] = new HeroConfig(103038, "严畯", 1, 20, 62, 82, 48, 192, 290, 3, 10, 17, 15, 0, 28, 0, 3, "扇", null, "虐", "", "inte", "FanExplosion", "yanjun", 1);
            config[104001] = new HeroConfig(104001, "吕布", 1, 32, 97, 43, 100, 240, 310, 4, 10, 17, 0, 0, 67, 0, 1, "车", null, "魔", "羽", "atk", "SwordHitBlackRedCritical", "lvbu", 5);
            config[104002] = new HeroConfig(104002, "华雄", 1, 29, 88, 60, 90, 238, 310, 4, 10, 17, 0, 0, 64, 0, 1, "车", null, "纷", "", "atk", "SwordHitYellowCritical", "huaxiong", 1);
            config[104003] = new HeroConfig(104003, "贾诩", 1, 28, 86, 97, 50, 233, 260, 4, 10, 17, 15, 0, 59, 0, 3, "谋", null, "延", "", "inte", "StormExplosion", "jiaxu", 3);
            config[104004] = new HeroConfig(104004, "貂蝉", 1, 9, 27, 81, 65, 173, 280, 4, 10, 17, 15, 0, 25, 0, 3, "乐", null, "曲", "", "help", "StormExplosion", "diaochan", 1);
            config[104005] = new HeroConfig(104005, "臧霸", 1, 26, 78, 53, 75, 206, 330, 4, 10, 17, 0, 0, 36, 0, 1, "马", null, "虐", "", "atk", "SwordHitYellowCritical", "zangba", 1);
            config[104006] = new HeroConfig(104006, "高顺", 1, 28, 85, 63, 86, 234, 215, 4, 10, 17, 13, 2.5f, 60, 0, 3, "炮", null, "", "", "shoot", "GasShootFire", "gaoshun", 2);
            config[104007] = new HeroConfig(104007, "李儒", 1, 21, 63, 91, 43, 197, 250, 4, 10, 17, 15, 0, 31, 0, 3, "谋", null, "火", "", "inte", "ShadowExplosion", "liru", 1);
            config[104008] = new HeroConfig(104008, "李傕", 1, 23, 69, 29, 74, 172, 370, 4, 10, 17, 0, 0, 25, 0, 1, "刀", null, "劫", "", "atk", "SwordHitYellowCritical", "lijue", 1);
            config[104009] = new HeroConfig(104009, "郭汜", 1, 21, 64, 18, 76, 158, 390, 4, 10, 17, 0, 0, 25, 0, 1, "刀", null, "劫", "", "atk", "SwordHitYellowCritical", "guosi", 1);
            config[104010] = new HeroConfig(104010, "陈宫", 1, 28, 84, 89, 55, 228, 260, 4, 10, 17, 15, 0, 54, 0, 3, "谋", null, "励", "溃", "inte", "ShadowExplosion", "chengong", 1);
            config[105001] = new HeroConfig(105001, "邓艾", 1, 31, 94, 89, 87, 270, 295, 5, 10, 17, 0, 0, 113, 0, 1, "枪", null, "奇", "", "def", "SwordHitYellowCritical", "dengai", 2);
            config[105002] = new HeroConfig(105002, "司马师", 1, 26, 80, 87, 67, 234, 280, 5, 10, 17, 18, 0, 60, 0, 3, "相", null, "", "", "help", "SharpExplosionGreen", "simashi", 1);
            config[105003] = new HeroConfig(105003, "司马昭", 1, 26, 78, 87, 57, 222, 290, 5, 10, 17, 18, 0, 48, 0, 3, "相", null, "溃", "", "help", "SharpExplosionGreen", "simazhao", 2);
            config[105004] = new HeroConfig(105004, "羊祜", 1, 30, 90, 84, 64, 238, 320, 5, 10, 17, 0, 0, 64, 0, 1, "戟", null, "敏", "", "atk", "SwordHitYellowCritical", "yangku", 2);
            config[105005] = new HeroConfig(105005, "钟会", 1, 27, 82, 92, 58, 232, 290, 5, 10, 17, 15, 0, 58, 0, 3, "谋", null, "缓", "", "inte", "StormExplosion", "zhonghui", 4);
            config[105006] = new HeroConfig(105006, "陈泰", 1, 28, 86, 84, 77, 247, 280, 5, 10, 17, 0, 0, 75, 0, 1, "士", null, "虐", "", "def", "SwordHitYellowCritical", "chentai", 1);
            config[105007] = new HeroConfig(105007, "杜预", 1, 28, 84, 85, 30, 199, 320, 5, 10, 17, 18, 0, 32, 0, 3, "相", null, "米", "", "inte", "SharpExplosionGreen", "duyu", 2);
            config[105008] = new HeroConfig(105008, "王濬", 1, 26, 80, 79, 52, 211, 340, 5, 10, 17, 0, 0, 40, 0, 1, "戟", null, "敏", "", "atk", "SwordHitYellowCritical", "wangrui", 2);
            config[105009] = new HeroConfig(105009, "辛宪英", 1, 14, 42, 84, 28, 154, 310, 5, 10, 17, 15, 0, 25, 0, 3, "扇", null, "缓", "", "inte", "FanExplosion", "xinxianying", 1);
            config[106001] = new HeroConfig(106001, "颜良", 1, 29, 88, 41, 93, 222, 340, 6, 10, 17, 0, 0, 48, 0, 1, "车", null, "破", "", "atk", "SwordHitYellowCritical", "yanliang", 1);
            config[106002] = new HeroConfig(106002, "文丑", 1, 29, 89, 48, 92, 229, 355, 6, 10, 17, 0, 0, 55, 0, 1, "车", null, "刺", "", "def", "SwordHitYellowCritical", "wenchou", 1);
            config[106003] = new HeroConfig(106003, "田丰", 1, 24, 72, 93, 33, 198, 250, 6, 10, 17, 15, 0, 32, 0, 3, "谋", null, "雷", "", "inte", "StormExplosion", "tianfeng", 1);
            config[106004] = new HeroConfig(106004, "鞠义", 1, 24, 72, 55, 78, 205, 250, 6, 10, 17, 22, 1.5f, 36, 0, 3, "弓", null, "", "", "shoot", "BulletExplosionBlue", "juyi", 0);
            config[106005] = new HeroConfig(106005, "许攸", 1, 13, 39, 80, 29, 148, 285, 6, 10, 17, 15, 0, 25, 0, 3, "谋", null, "火", "", "inte", "StormExplosion", "xuyou", 1);
            config[106006] = new HeroConfig(106006, "高览", 1, 25, 76, 68, 82, 226, 305, 6, 10, 17, 0, 0, 52, 0, 1, "枪", null, "", "", "atk", "SwordHitYellowCritical", "gaolan", 1);
            config[106007] = new HeroConfig(106007, "沮授", 1, 26, 78, 90, 35, 203, 260, 6, 10, 17, 15, 0, 35, 0, 3, "谋", null, "静", "", "inte", "StormExplosion", "jushou", 1);
            config[106008] = new HeroConfig(106008, "郭图", 1, 17, 52, 83, 50, 185, 290, 6, 10, 17, 15, 0, 25, 0, 3, "扇", null, "励", "米", "help", "FanExplosion", "guotu", 1);
            config[110001] = new HeroConfig(110001, "公孙瓒", 1, 27, 83, 75, 82, 240, 295, 10, 10, 17, 0, 0, 67, 0, 1, "马", null, "乱", "", "def", "SwordHitYellowCritical", "gongsunzan", 2);
            config[110002] = new HeroConfig(110002, "张任", 1, 29, 88, 75, 84, 247, 210, 10, 10, 17, 22, 1.5f, 75, 0, 3, "弓", null, "复", "", "shoot", "BulletExplosionBlue", "zhangren", 1);
            config[110003] = new HeroConfig(110003, "华佗", 1, 20, 60, 77, 34, 171, 330, 10, 10, 17, 14, 0, 25, 0, 3, "医", null, "药", "", "help", "ShadowExplosionGreen", "huatuo", 2);
            config[110004] = new HeroConfig(110004, "袁术", 1, 22, 67, 65, 65, 197, 335, 10, 10, 17, 0, 0, 31, 0, 1, "戟", null, "", "", "def", "SwordHitYellowCritical", "yuanshu", 3);
            config[110005] = new HeroConfig(110005, "马腾", 1, 27, 82, 51, 80, 213, 290, 10, 10, 17, 0, 0, 41, 0, 1, "马", null, "羽", "", "atk", "SwordHitYellowCritical", "mateng", 3);
            config[110006] = new HeroConfig(110006, "于吉", 1, 15, 47, 73, 41, 161, 310, 10, 10, 17, 14, 0, 25, 0, 3, "医", null, "调", "", "help", "ShadowExplosionGreen", "yuji", 0);
            config[110007] = new HeroConfig(110007, "张角", 1, 29, 87, 86, 29, 202, 280, 10, 10, 17, 15, 0, 34, 0, 3, "谋", null, "天", "陷", "inte", "LightningExplosionBlue", "zhangjiao", 1);
            config[110008] = new HeroConfig(110008, "张宝", 1, 27, 83, 81, 71, 235, 280, 10, 10, 17, 0, 0, 61, 0, 1, "枪", null, "劫", "", "atk", "SwordHitYellowCritical", "zhangbao2", 1);
            config[110009] = new HeroConfig(110009, "张梁", 1, 26, 78, 74, 80, 232, 225, 10, 10, 17, 13, 2.5f, 58, 0, 3, "炮", null, "", "", "def", "SwordHitYellowCritical", "zhangliang", 1);
            config[110010] = new HeroConfig(110010, "韩遂", 1, 27, 81, 82, 75, 238, 290, 10, 10, 17, 0, 0, 64, 0, 1, "马", null, "乱", "", "def", "SwordHitYellowCritical", "hansui", 1);
            config[110011] = new HeroConfig(110011, "王异", 1, 24, 73, 82, 51, 206, 290, 10, 10, 17, 0, 0, 36, 0, 1, "枪", null, "", "", "atk", "SwordHitYellowCritical", "wangyi", 0);
            config[110012] = new HeroConfig(110012, "蔡琰", 1, 20, 61, 77, 13, 151, 270, 10, 10, 17, 15, 0, 25, 0, 3, "乐", null, "碉", "", "help", "StormExplosion", "caiyan", 1);

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
