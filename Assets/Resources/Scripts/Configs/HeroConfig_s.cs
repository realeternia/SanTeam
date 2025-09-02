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
        ///职业
        /// </summary>
        public string Job;
        /// <summary>
        ///站位
        /// </summary>
        public int Pos;
        /// <summary>
        ///技能
        /// </summary>
        public int[] Skills;
        /// <summary>
        ///hit
        /// </summary>
        public string HitEffect;
        /// <summary>
        ///背景图
        /// </summary>
        public string Icon;


        public HeroConfig(int Id, string Name, int Lv, int Atk, int LeadShip, int Inte, int Str, int Total, int Hp, int Side, int MoveSpeed, int Range, int MissileSpeed, float MissileHight, int RateWeight, int RateAbs, string Job, int Pos, int[] Skills, string HitEffect, string Icon)
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
            this.Job = Job;
            this.Pos = Pos;
            this.Skills = Skills;
            this.HitEffect = HitEffect;
            this.Icon = Icon;

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
            config[100001] = new HeroConfig(100001, "刘备", 1, 26, 80, 78, 77, 235, 310, 1, 10, 17, 0, 0, 0, 100, "shuai", 2, null, "SwordHitYellowCritical", "liubei");
            config[100002] = new HeroConfig(100002, "曹操", 1, 32, 98, 92, 81, 271, 265, 2, 10, 17, 0, 0, 0, 100, "shuai", 2, null, "SwordHitYellowCritical", "caocao");
            config[100003] = new HeroConfig(100003, "孙权", 1, 26, 79, 80, 70, 229, 315, 3, 10, 17, 0, 0, 0, 100, "shuai", 2, null, "SwordHitYellowCritical", "sunquan");
            config[101001] = new HeroConfig(101001, "赵云", 1, 30, 91, 76, 96, 263, 265, 1, 10, 17, 0, 0, 100, 0, "", 1, new int[]{201023}, "SwordHitYellowCritical", "zhaoyun");
            config[101002] = new HeroConfig(101002, "张飞", 1, 30, 92, 42, 98, 232, 330, 1, 10, 17, 0, 0, 58, 0, "", 1, new int[]{201024}, "SwordHitYellowCritical", "zhangfei");
            config[101003] = new HeroConfig(101003, "马超", 1, 30, 92, 51, 97, 240, 310, 1, 12, 17, 0, 0, 67, 0, "", 1, new int[]{201021}, "SwordHitYellowCritical", "machao");
            config[101004] = new HeroConfig(101004, "诸葛亮", 1, 32, 98, 100, 45, 243, 300, 1, 7, 35, 18, 0, 70, 0, "", 3, new int[]{201013}, "LightningExplosionBlue", "zhugeliang");
            config[101005] = new HeroConfig(101005, "关羽", 1, 32, 97, 77, 97, 271, 250, 1, 12, 17, 0, 0, 115, 0, "", 1, new int[]{201020}, "SwordHitYellowCritical", "guanyu");
            config[101006] = new HeroConfig(101006, "徐庶", 1, 29, 87, 93, 65, 245, 235, 1, 7, 35, 15, 0, 73, 0, "mou", 3, null, "GasExplosionFire", "xusu");
            config[101007] = new HeroConfig(101007, "魏延", 1, 28, 84, 67, 91, 242, 300, 1, 10, 17, 0, 0, 69, 0, "che", 1, null, "SwordHitYellowCritical", "weiyan");
            config[101008] = new HeroConfig(101008, "黄忠", 1, 30, 90, 64, 93, 247, 235, 1, 7, 50, 22, 1.5f, 75, 0, "", 3, new int[]{201025}, "BulletExplosionGreen", "huangzhong");
            config[101009] = new HeroConfig(101009, "周仓", 1, 21, 63, 55, 82, 200, 280, 1, 10, 17, 0, 0, 33, 0, "", 1, null, "SwordHitYellowCritical", "zhoucang");
            config[101010] = new HeroConfig(101010, "姜维", 1, 30, 92, 90, 89, 271, 285, 1, 10, 17, 0, 0, 115, 0, "che", 1, null, "SwordHitYellowCritical", "jiangwei");
            config[101011] = new HeroConfig(101011, "马岱", 1, 26, 80, 66, 85, 231, 300, 1, 12, 17, 0, 0, 57, 0, "ma", 1, null, "SwordHitYellowCritical", "madai");
            config[101012] = new HeroConfig(101012, "关兴", 1, 26, 80, 72, 85, 237, 300, 1, 12, 17, 0, 0, 63, 0, "ma", 1, null, "SwordHitYellowCritical", "guanxing");
            config[101013] = new HeroConfig(101013, "关平", 1, 26, 79, 72, 82, 233, 290, 1, 12, 17, 0, 0, 59, 0, "ma", 1, null, "SwordHitYellowCritical", "guanping");
            config[101014] = new HeroConfig(101014, "严颜", 1, 26, 79, 71, 83, 233, 280, 1, 10, 17, 0, 0, 59, 0, "shi", 1, null, "SwordHitYellowCritical", "yanyan");
            config[101015] = new HeroConfig(101015, "廖化", 1, 24, 74, 68, 78, 220, 310, 1, 10, 17, 0, 0, 47, 0, "", 1, null, "SwordHitYellowCritical", "liaohua");
            config[101016] = new HeroConfig(101016, "孟获", 1, 29, 87, 51, 87, 225, 310, 1, 10, 17, 0, 0, 51, 0, "", 1, new int[]{201007}, "SwordHitYellowCritical", "menghuo");
            config[101017] = new HeroConfig(101017, "庞统", 1, 28, 85, 98, 42, 225, 260, 1, 7, 35, 15, 0, 51, 0, "", 3, new int[]{201014}, "ExplosionFireballFire", "pangtong");
            config[101018] = new HeroConfig(101018, "李严", 1, 27, 82, 75, 83, 240, 290, 1, 10, 17, 0, 0, 67, 0, "shi", 1, null, "SwordHitYellowCritical", "liyan");
            config[101019] = new HeroConfig(101019, "张苞", 1, 25, 75, 82, 87, 244, 305, 1, 10, 17, 0, 0, 71, 0, "che", 1, null, "SwordHitYellowCritical", "zhangbao");
            config[101020] = new HeroConfig(101020, "张松", 1, 21, 65, 89, 46, 200, 260, 1, 7, 35, 15, 0, 33, 0, "xiang", 3, null, "StormExplosion", "zhangsong");
            config[101021] = new HeroConfig(101021, "关索", 1, 24, 74, 69, 81, 224, 255, 1, 6, 70, 30, 1.5f, 50, 0, "", 3, new int[]{201005}, "BulletExplosionGreen", "guansuo");
            config[101022] = new HeroConfig(101022, "简雍", 1, 24, 72, 69, 65, 206, 240, 1, 7, 50, 22, 1.5f, 36, 0, "gong", 3, null, "BulletExplosionGreen", "jianyong");
            config[101023] = new HeroConfig(101023, "蒋琬", 1, 21, 64, 85, 52, 201, 240, 1, 7, 35, 15, 0, 33, 0, "", 3, new int[]{201009}, "StormExplosion", "jiangwan");
            config[101024] = new HeroConfig(101024, "孙乾", 1, 20, 62, 80, 54, 196, 250, 1, 10, 17, 0, 0, 31, 0, "", 1, null, "SwordHitYellowCritical", "sunqian");
            config[101025] = new HeroConfig(101025, "费祎", 1, 22, 68, 86, 42, 196, 250, 1, 7, 35, 15, 0, 31, 0, "", 3, new int[]{201009}, "StormExplosion", "feiyi");
            config[101026] = new HeroConfig(101026, "马谡", 1, 23, 70, 88, 72, 230, 270, 1, 7, 35, 15, 0, 56, 0, "mou", 3, null, "StormExplosion", "masu");
            config[101027] = new HeroConfig(101027, "董允", 1, 22, 67, 85, 65, 217, 280, 1, 10, 17, 0, 0, 44, 0, "", 1, null, "SwordHitYellowCritical", "dongyun");
            config[101028] = new HeroConfig(101028, "王平", 1, 27, 83, 75, 78, 236, 300, 1, 10, 17, 0, 0, 62, 0, "", 1, new int[]{201022}, "SwordHitYellowCritical", "wangping");
            config[101029] = new HeroConfig(101029, "邓芝", 1, 23, 70, 80, 71, 221, 290, 1, 10, 17, 0, 0, 48, 0, "", 1, null, "SwordHitYellowCritical", "dengzhi");
            config[101030] = new HeroConfig(101030, "郭攸之", 1, 21, 63, 82, 48, 193, 260, 1, 7, 35, 15, 0, 29, 0, "", 3, new int[]{201009}, "StormExplosion", "guoyouzhi");
            config[101031] = new HeroConfig(101031, "马良", 1, 21, 65, 93, 60, 218, 250, 1, 7, 35, 15, 0, 45, 0, "xiang", 3, null, "StormExplosion", "maliang");
            config[101032] = new HeroConfig(101032, "黄月英", 1, 20, 60, 82, 50, 192, 250, 1, 7, 55, 13, 2.5f, 28, 0, "", 3, new int[]{201031}, "GasMissileFire", "huangyueying");
            config[101033] = new HeroConfig(101033, "法正", 1, 27, 83, 94, 52, 229, 255, 1, 7, 35, 15, 0, 55, 0, "mou", 3, null, "GasExplosionFire", "fazheng");
            config[101034] = new HeroConfig(101034, "刘禅", 1, 13, 40, 50, 45, 135, 250, 1, 10, 17, 0, 0, 25, 0, "", 1, null, "SwordHitYellowCritical", "liushan");
            config[101035] = new HeroConfig(101035, "祝融", 1, 25, 77, 43, 85, 205, 290, 1, 10, 17, 0, 0, 36, 0, "", 1, new int[]{201007}, "SwordHitYellowCritical", "zhurong");
            config[101036] = new HeroConfig(101036, "黄权", 1, 25, 75, 82, 59, 216, 310, 1, 10, 17, 0, 0, 44, 0, "", 1, null, "SwordHitYellowCritical", "huangquan");
            config[101037] = new HeroConfig(101037, "孟达", 1, 25, 75, 68, 73, 216, 250, 1, 7, 50, 22, 1.5f, 44, 0, "gong", 3, null, "BulletExplosionGreen", "mengda");
            config[101038] = new HeroConfig(101038, "刘封", 1, 25, 75, 44, 79, 198, 310, 1, 10, 17, 0, 0, 32, 0, "", 1, new int[]{201004}, "SwordHitYellowCritical", "liufeng");
            config[102001] = new HeroConfig(102001, "郭嘉", 1, 24, 72, 98, 43, 213, 260, 2, 7, 35, 15, 0, 41, 0, "", 3, new int[]{201011}, "LightningExplosionBlue", "guojia");
            config[102002] = new HeroConfig(102002, "夏侯惇", 1, 30, 91, 63, 91, 245, 320, 2, 10, 17, 0, 0, 73, 0, "", 1, new int[]{201028}, "SwordHitYellowCritical", "xiahoudun");
            config[102003] = new HeroConfig(102003, "荀彧", 1, 21, 65, 96, 43, 204, 270, 2, 7, 35, 14, 0, 35, 0, "", 3, new int[]{201018}, "FrostExplosionBlue", "xunyu");
            config[102004] = new HeroConfig(102004, "张辽", 1, 31, 95, 78, 92, 265, 275, 2, 12, 17, 0, 0, 103, 0, "", 1, new int[]{201010}, "SwordHitYellowCritical", "zhangliao");
            config[102005] = new HeroConfig(102005, "许褚", 1, 21, 65, 44, 96, 205, 340, 2, 10, 17, 0, 0, 36, 0, "", 1, new int[]{201026}, "SwordHitYellowCritical", "xuchu");
            config[102006] = new HeroConfig(102006, "夏侯渊", 1, 30, 90, 68, 89, 247, 230, 2, 7, 50, 22, 1.5f, 75, 0, "", 3, new int[]{201008}, "BulletExplosionBlue", "xiahouyuan");
            config[102007] = new HeroConfig(102007, "典韦", 1, 19, 59, 43, 95, 197, 350, 2, 10, 17, 0, 0, 31, 0, "", 1, new int[]{201026}, "SwordHitYellowCritical", "dianwei");
            config[102008] = new HeroConfig(102008, "张郃", 1, 29, 89, 75, 90, 254, 300, 2, 10, 17, 0, 0, 85, 0, "shi", 1, null, "SwordHitYellowCritical", "zhanghe");
            config[102009] = new HeroConfig(102009, "徐晃", 1, 30, 90, 77, 91, 258, 220, 2, 7, 50, 22, 1.5f, 91, 0, "", 3, new int[]{201006}, "BulletExplosionBlue", "xuhuang");
            config[102010] = new HeroConfig(102010, "荀攸", 1, 20, 60, 92, 51, 203, 240, 2, 7, 35, 15, 0, 35, 0, "", 3, new int[]{201018}, "FrostExplosionBlue", "xunyou");
            config[102011] = new HeroConfig(102011, "于禁", 1, 26, 80, 72, 75, 227, 280, 2, 10, 17, 0, 0, 53, 0, "", 1, new int[]{201028}, "SwordHitYellowCritical", "yujin");
            config[102012] = new HeroConfig(102012, "曹仁", 1, 30, 90, 62, 86, 238, 340, 2, 10, 17, 0, 0, 64, 0, "", 1, new int[]{201028}, "SwordHitYellowCritical", "caoren");
            config[102013] = new HeroConfig(102013, "曹洪", 1, 27, 82, 51, 83, 216, 260, 2, 10, 17, 0, 0, 44, 0, "", 1, new int[]{201003}, "SwordHitYellowCritical", "caohong");
            config[102014] = new HeroConfig(102014, "庞德", 1, 29, 89, 74, 94, 257, 290, 2, 12, 17, 0, 0, 90, 0, "", 1, new int[]{201030}, "SwordHitYellowCritical", "pangde");
            config[102015] = new HeroConfig(102015, "乐进", 1, 26, 80, 55, 84, 219, 280, 2, 12, 17, 0, 0, 46, 0, "ma", 1, null, "SwordHitYellowCritical", "lejin");
            config[102016] = new HeroConfig(102016, "贾诩", 1, 28, 86, 97, 50, 233, 250, 2, 7, 35, 15, 0, 59, 0, "", 3, new int[]{201015}, "StormExplosion", "jiaxu");
            config[102017] = new HeroConfig(102017, "文聘", 1, 26, 80, 65, 82, 227, 270, 2, 10, 17, 0, 0, 53, 0, "", 1, null, "SwordHitYellowCritical", "wenpin");
            config[102018] = new HeroConfig(102018, "曹休", 1, 24, 73, 72, 73, 218, 250, 2, 7, 50, 22, 1.5f, 45, 0, "gong", 3, null, "BulletExplosionBlue", "caoxiu");
            config[102019] = new HeroConfig(102019, "司马懿", 1, 32, 98, 98, 63, 259, 250, 2, 7, 35, 15, 0, 93, 0, "", 3, new int[]{201017}, "ShadowExplosion", "simayi");
            config[102020] = new HeroConfig(102020, "邓艾", 1, 31, 94, 89, 87, 270, 280, 2, 10, 17, 0, 0, 113, 0, "che", 1, null, "SwordHitYellowCritical", "dengai");
            config[102021] = new HeroConfig(102021, "司马师", 1, 26, 80, 87, 67, 234, 340, 2, 7, 35, 15, 0, 60, 0, "xiang", 3, null, "ShadowExplosion", "simashi");
            config[102022] = new HeroConfig(102022, "夏侯霸", 1, 27, 82, 69, 77, 228, 320, 2, 10, 17, 0, 0, 54, 0, "", 1, null, "SwordHitYellowCritical", "xiahouba");
            config[102023] = new HeroConfig(102023, "司马昭", 1, 26, 78, 87, 57, 222, 330, 2, 7, 35, 15, 0, 48, 0, "xiang", 3, null, "ShadowExplosion", "simazhao");
            config[102024] = new HeroConfig(102024, "郝昭", 1, 29, 87, 76, 79, 242, 300, 2, 10, 17, 0, 0, 69, 0, "shi", 1, null, "SwordHitYellowCritical", "haozhao");
            config[102025] = new HeroConfig(102025, "王双", 1, 22, 68, 38, 88, 194, 340, 2, 10, 17, 0, 0, 29, 0, "shi", 1, null, "SwordHitYellowCritical", "wangshuang");
            config[102026] = new HeroConfig(102026, "程昱", 1, 21, 63, 90, 54, 207, 240, 2, 7, 35, 15, 0, 37, 0, "mou", 3, null, "StormExplosion", "chengyu");
            config[102027] = new HeroConfig(102027, "杨修", 1, 20, 60, 88, 57, 205, 260, 2, 7, 35, 15, 0, 36, 0, "xiang", 3, null, "StormExplosion", "yangxiu");
            config[102028] = new HeroConfig(102028, "钟会", 1, 27, 82, 92, 55, 229, 260, 2, 7, 35, 15, 0, 55, 0, "mou", 3, null, "StormExplosion", "zhonghui");
            config[102029] = new HeroConfig(102029, "牛金", 1, 23, 71, 65, 77, 213, 290, 2, 10, 17, 0, 0, 41, 0, "", 1, null, "SwordHitYellowCritical", "niujin");
            config[102030] = new HeroConfig(102030, "文鸯", 1, 25, 76, 64, 91, 231, 230, 2, 7, 50, 22, 1.5f, 57, 0, "gong", 3, null, "BulletExplosionBlue", "wenyuan");
            config[102031] = new HeroConfig(102031, "曹真", 1, 27, 82, 70, 74, 226, 310, 2, 10, 17, 0, 0, 52, 0, "", 1, null, "SwordHitYellowCritical", "caozhen");
            config[102032] = new HeroConfig(102032, "陈群", 1, 21, 65, 84, 45, 194, 270, 2, 7, 35, 15, 0, 29, 0, "", 3, new int[]{201009}, "StormExplosion", "chenqun");
            config[102033] = new HeroConfig(102033, "李典", 1, 24, 74, 71, 73, 218, 300, 2, 10, 17, 0, 0, 45, 0, "", 1, new int[]{201022}, "SwordHitYellowCritical", "lidian");
            config[102034] = new HeroConfig(102034, "曹丕", 1, 26, 78, 79, 79, 236, 320, 2, 10, 17, 0, 0, 62, 0, "", 1, null, "SwordHitYellowCritical", "caopi");
            config[102035] = new HeroConfig(102035, "曹植", 1, 21, 64, 83, 67, 214, 270, 2, 10, 17, 0, 0, 42, 0, "", 1, null, "SwordHitYellowCritical", "caozhi");
            config[102036] = new HeroConfig(102036, "刘晔", 1, 21, 65, 88, 49, 202, 250, 2, 7, 55, 13, 2.5f, 34, 0, "", 3, new int[]{201031}, "GasMissileFire", "liuye");
            config[102037] = new HeroConfig(102037, "朱灵", 1, 24, 73, 60, 77, 210, 300, 2, 10, 17, 0, 0, 39, 0, "", 1, null, "SwordHitYellowCritical", "zhuling");
            config[102038] = new HeroConfig(102038, "羊枯", 1, 30, 90, 84, 64, 238, 320, 2, 10, 17, 0, 0, 64, 0, "", 1, null, "SwordHitYellowCritical", "yangku");
            config[102039] = new HeroConfig(102039, "陈泰", 1, 28, 86, 84, 77, 247, 300, 2, 10, 17, 0, 0, 75, 0, "", 1, null, "SwordHitYellowCritical", "chentai");
            config[102040] = new HeroConfig(102040, "曹彰", 1, 27, 82, 43, 90, 215, 350, 2, 10, 17, 0, 0, 43, 0, "", 1, new int[]{201028}, "SwordHitYellowCritical", "caozhang");
            config[103001] = new HeroConfig(103001, "孙坚", 1, 31, 93, 77, 90, 260, 290, 3, 10, 17, 0, 0, 95, 0, "", 1, new int[]{201010}, "SwordHitYellowCritical", "sunjian");
            config[103002] = new HeroConfig(103002, "孙策", 1, 32, 96, 74, 93, 263, 285, 3, 10, 17, 0, 0, 100, 0, "", 1, new int[]{201027}, "SwordHitYellowCritical", "sunce");
            config[103003] = new HeroConfig(103003, "甘宁", 1, 31, 93, 76, 94, 263, 230, 3, 7, 50, 22, 1.5f, 100, 0, "", 3, new int[]{201006}, "BulletExplosionFire", "ganning");
            config[103004] = new HeroConfig(103004, "太史慈", 1, 30, 90, 65, 93, 248, 230, 3, 7, 35, 15, 1.5f, 77, 0, "", 3, new int[]{201008}, "BulletExplosionFire", "taishici");
            config[103005] = new HeroConfig(103005, "黄盖", 1, 26, 80, 70, 83, 233, 320, 3, 10, 17, 0, 0, 59, 0, "shi", 1, null, "SwordHitYellowCritical", "huanggai");
            config[103006] = new HeroConfig(103006, "周泰", 1, 28, 84, 51, 91, 226, 325, 3, 10, 17, 0, 0, 52, 0, "shi", 1, null, "SwordHitYellowCritical", "zhoutai");
            config[103007] = new HeroConfig(103007, "鲁肃", 1, 28, 85, 92, 61, 238, 260, 3, 7, 35, 15, 0, 64, 0, "", 3, new int[]{201003}, "StormExplosion", "lusu");
            config[103008] = new HeroConfig(103008, "周瑜", 1, 32, 96, 96, 71, 263, 250, 3, 7, 35, 15, 0, 100, 0, "", 3, new int[]{201012}, "ExplosionFireballFire", "zhouyu");
            config[103009] = new HeroConfig(103009, "蒋钦", 1, 25, 77, 57, 84, 218, 310, 3, 10, 17, 0, 0, 45, 0, "shi", 1, null, "SwordHitYellowCritical", "jiangqing");
            config[103010] = new HeroConfig(103010, "吕蒙", 1, 30, 92, 91, 80, 263, 300, 3, 12, 17, 0, 0, 100, 0, "", 1, new int[]{201029}, "SwordHitYellowCritical", "lvmeng");
            config[103011] = new HeroConfig(103011, "陆逊", 1, 32, 96, 94, 69, 259, 270, 3, 7, 35, 15, 0, 93, 0, "", 3, new int[]{201012}, "GasExplosionFire", "luxun");
            config[103012] = new HeroConfig(103012, "张昭", 1, 21, 65, 89, 41, 195, 245, 3, 7, 35, 15, 0, 30, 0, "xiang", 3, null, "StormExplosion", "zhangzhao");
            config[103013] = new HeroConfig(103013, "诸葛瑾", 1, 25, 75, 83, 44, 202, 290, 3, 7, 35, 15, 0, 34, 0, "", 3, new int[]{201009}, "StormExplosion", "zhugejin");
            config[103014] = new HeroConfig(103014, "孙尚香", 1, 24, 73, 69, 81, 223, 240, 3, 7, 50, 22, 1.5f, 49, 0, "gong", 3, null, "BulletExplosionFire", "sunshangxiang");
            config[103015] = new HeroConfig(103015, "朱桓", 1, 28, 84, 77, 82, 243, 320, 3, 10, 17, 0, 0, 70, 0, "", 1, new int[]{201022}, "SwordHitYellowCritical", "zhuhuan");
            config[103016] = new HeroConfig(103016, "大乔", 1, 21, 65, 79, 70, 214, 280, 3, 10, 17, 0, 0, 42, 0, "", 1, new int[]{201002}, "SwordHitYellowCritical", "daqiao");
            config[103017] = new HeroConfig(103017, "小乔", 1, 20, 60, 85, 65, 210, 270, 3, 10, 17, 0, 0, 39, 0, "", 1, new int[]{201002}, "SwordHitYellowCritical", "xiaoqiao");
            config[103018] = new HeroConfig(103018, "丁奉", 1, 25, 76, 68, 82, 226, 320, 3, 10, 55, 13, 2.5f, 52, 0, "", 3, new int[]{201031}, "GasMissileFire", "dingfeng");
            config[103019] = new HeroConfig(103019, "董袭", 1, 24, 74, 60, 80, 214, 310, 3, 10, 17, 0, 0, 42, 0, "", 1, null, "SwordHitYellowCritical", "dongxi");
            config[103020] = new HeroConfig(103020, "凌统", 1, 25, 77, 62, 87, 226, 245, 3, 6, 70, 30, 1.5f, 52, 0, "", 3, new int[]{201005}, "BulletExplosionFire", "lingtong");
            config[103021] = new HeroConfig(103021, "潘璋", 1, 25, 75, 74, 80, 229, 295, 3, 10, 17, 0, 0, 55, 0, "", 1, new int[]{201004}, "SwordHitYellowCritical", "panzhang");
            config[103022] = new HeroConfig(103022, "朱治", 1, 24, 73, 59, 72, 204, 235, 3, 7, 50, 22, 1.5f, 35, 0, "gong", 3, null, "BulletExplosionFire", "zhuzhi");
            config[103023] = new HeroConfig(103023, "徐盛", 1, 28, 86, 78, 81, 245, 290, 3, 10, 17, 0, 0, 73, 0, "shi", 1, null, "SwordHitYellowCritical", "xusheng");
            config[103024] = new HeroConfig(103024, "程普", 1, 28, 84, 78, 80, 242, 300, 3, 10, 17, 0, 0, 69, 0, "", 1, null, "SwordHitYellowCritical", "chengpu");
            config[103025] = new HeroConfig(103025, "张纮", 1, 22, 68, 85, 50, 203, 250, 3, 7, 35, 15, 0, 35, 0, "", 3, new int[]{201009}, "StormExplosion", "zhanghong");
            config[103026] = new HeroConfig(103026, "顾雍", 1, 21, 65, 81, 60, 206, 270, 3, 10, 17, 0, 0, 36, 0, "", 1, null, "SwordHitYellowCritical", "guyong");
            config[103027] = new HeroConfig(103027, "步骘", 1, 23, 70, 80, 65, 215, 290, 3, 10, 17, 0, 0, 43, 0, "", 1, null, "SwordHitYellowCritical", "buzhi");
            config[103028] = new HeroConfig(103028, "阚泽", 1, 20, 62, 84, 55, 201, 260, 3, 7, 35, 15, 0, 33, 0, "mou", 3, null, "StormExplosion", "kanze");
            config[103029] = new HeroConfig(103029, "韩当", 1, 25, 76, 62, 85, 223, 300, 3, 10, 17, 0, 0, 49, 0, "ma", 1, null, "SwordHitYellowCritical", "handang");
            config[103030] = new HeroConfig(103030, "陆抗", 1, 30, 91, 87, 63, 241, 320, 3, 10, 17, 0, 0, 68, 0, "", 1, null, "SwordHitYellowCritical", "lukang");
            config[103031] = new HeroConfig(103031, "诸葛恪", 1, 24, 72, 90, 47, 209, 235, 3, 7, 35, 15, 0, 38, 0, "mou", 3, null, "StormExplosion", "zhugege");
            config[103032] = new HeroConfig(103032, "苏飞", 1, 23, 69, 66, 63, 198, 300, 3, 10, 17, 20, 0, 32, 0, "", 1, null, "SwordHitYellowCritical", "sufei");
            config[103033] = new HeroConfig(103033, "全琮", 1, 26, 78, 75, 72, 225, 300, 3, 10, 17, 0, 0, 51, 0, "", 1, null, "SwordHitYellowCritical", "quanzong");
            config[103034] = new HeroConfig(103034, "陈武", 1, 25, 76, 47, 87, 210, 305, 3, 12, 17, 0, 0, 39, 0, "shi", 1, null, "SwordHitYellowCritical", "chengwu");
            config[103035] = new HeroConfig(103035, "朱然", 1, 26, 79, 71, 69, 219, 315, 3, 10, 17, 0, 0, 46, 0, "", 1, null, "SwordHitYellowCritical", "zhuran");
            config[103036] = new HeroConfig(103036, "孙韶", 1, 26, 80, 76, 79, 235, 310, 3, 10, 17, 0, 0, 61, 0, "", 1, null, "SwordHitYellowCritical", "sunshao");
            config[103037] = new HeroConfig(103037, "孙桓", 1, 27, 82, 76, 73, 231, 300, 3, 10, 17, 0, 0, 57, 0, "", 1, null, "SwordHitYellowCritical", "sunhuan");
            config[103038] = new HeroConfig(103038, "严畯", 1, 20, 62, 82, 48, 192, 290, 3, 7, 35, 15, 0, 28, 0, "xiang", 3, null, "StormExplosion", "yanjun");
            config[103039] = new HeroConfig(103039, "于吉", 1, 15, 47, 73, 41, 161, 310, 3, 10, 17, 0, 0, 0, 45, "", 3, new int[]{201001}, "SwordHitYellowCritical", "yuji");
            config[104001] = new HeroConfig(104001, "董卓", 1, 28, 85, 75, 87, 247, 330, 4, 10, 17, 0, 0, 75, 0, "", 1, null, "SwordHitYellowCritical", "dongzhuo");
            config[104002] = new HeroConfig(104002, "吕布", 1, 32, 97, 41, 100, 238, 310, 4, 10, 17, 0, 0, 64, 0, "", 1, new int[]{201019}, "SwordHitYellowCritical", "lvbu");
            config[104003] = new HeroConfig(104003, "颜良", 1, 29, 88, 37, 93, 218, 315, 4, 10, 17, 0, 0, 45, 0, "", 1, new int[]{201030}, "SwordHitYellowCritical", "yanliang");
            config[104004] = new HeroConfig(104004, "文丑", 1, 29, 89, 48, 92, 229, 320, 4, 10, 17, 0, 0, 55, 0, "", 1, new int[]{201004}, "SwordHitYellowCritical", "wenchou");
            config[104005] = new HeroConfig(104005, "华雄", 1, 29, 88, 60, 90, 238, 310, 4, 10, 17, 0, 0, 64, 0, "che", 1, null, "SwordHitYellowCritical", "huaxiong");
            config[104006] = new HeroConfig(104006, "公孙瓒", 1, 27, 83, 75, 82, 240, 295, 4, 12, 17, 0, 0, 67, 0, "ma", 1, null, "SwordHitYellowCritical", "gongsunzan");
            config[104007] = new HeroConfig(104007, "貂蝉", 1, 21, 65, 80, 65, 210, 280, 4, 10, 17, 0, 0, 39, 0, "", 1, new int[]{201002}, "SwordHitYellowCritical", "diaochan");
            config[104008] = new HeroConfig(104008, "张任", 1, 29, 88, 75, 84, 247, 230, 4, 7, 50, 22, 1.5f, 75, 0, "gong", 3, null, "BulletExplosionFire", "zhangren");
            config[104009] = new HeroConfig(104009, "华佗", 1, 20, 60, 77, 34, 171, 330, 4, 10, 17, 0, 0, 0, 45, "", 3, new int[]{201001}, "SwordHitYellowCritical", "huatuo");
            config[104010] = new HeroConfig(104010, "臧霸", 1, 26, 78, 53, 75, 206, 250, 4, 10, 17, 0, 0, 36, 0, "", 1, null, "SwordHitYellowCritical", "zangba");
            config[104011] = new HeroConfig(104011, "高顺", 1, 28, 85, 63, 86, 234, 300, 4, 10, 55, 13, 2.5f, 60, 0, "", 3, new int[]{201031}, "GasMissileFire", "gaoshun");
            config[104012] = new HeroConfig(104012, "李儒", 1, 20, 61, 91, 33, 185, 245, 4, 7, 35, 15, 0, 25, 0, "mou", 3, null, "ShadowExplosion", "liru");
            config[104013] = new HeroConfig(104013, "袁绍", 1, 28, 86, 82, 73, 241, 310, 4, 10, 17, 0, 0, 68, 0, "", 1, null, "SwordHitYellowCritical", "yuanshao");
            config[104014] = new HeroConfig(104014, "袁术", 1, 22, 67, 65, 65, 197, 300, 4, 10, 17, 0, 0, 31, 0, "", 1, null, "SwordHitYellowCritical", "yuanshu");
            config[104015] = new HeroConfig(104015, "马腾", 1, 27, 82, 51, 80, 213, 275, 4, 12, 17, 0, 0, 41, 0, "ma", 1, null, "SwordHitYellowCritical", "mateng");

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
