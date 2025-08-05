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
        public uint Id;
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


        public HeroConfig(uint Id, string Name, int Lv, int Atk, int LeadShip, int Inte, int Str, int Total, int Hp, int Side, int MoveSpeed, int Range, int RateWeight, int RateAbs, string Job, int[] Skills, string HitEffect, string Icon)
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
            this.RateWeight = RateWeight;
            this.RateAbs = RateAbs;
            this.Job = Job;
            this.Skills = Skills;
            this.HitEffect = HitEffect;
            this.Icon = Icon;

        }

        public HeroConfig() { }

        private static Dictionary<uint, HeroConfig> config = new Dictionary<uint, HeroConfig>();
        public static Dictionary<uint, HeroConfig>.ValueCollection ConfigList
        {
            get
            {
                return config.Values;
            }
        }

        public static void Refresh(Dictionary<uint, HeroConfig> dict)
        {
            config.Clear();
            config = dict;
        }

        public static void Load()
        {
            config.Clear();
            config[100001] = new HeroConfig(100001, "文聘", 1, 24, 72, 65, 78, 215, 270, 2, 10, 17, 29, 0, "", null, "SwordHitYellowCritical", "wenpin");
            config[100002] = new HeroConfig(100002, "夏侯淳", 1, 28, 85, 70, 90, 245, 300, 2, 10, 17, 47, 0, "che", null, "SwordHitYellowCritical", "xiahoudun");
            config[100003] = new HeroConfig(100003, "赵云", 1, 28, 85, 86, 97, 268, 280, 1, 10, 17, 0, 40, "shi", null, "SwordHitYellowCritical", "zhaoyun");
            config[100004] = new HeroConfig(100004, "张飞", 1, 27, 82, 65, 98, 245, 330, 1, 10, 17, 0, 30, "che", null, "SwordHitYellowCritical", "zhangfei");
            config[100005] = new HeroConfig(100005, "周仓", 1, 22, 68, 60, 76, 204, 280, 1, 10, 17, 24, 0, "", null, "SwordHitYellowCritical", "zhoucang");
            config[100006] = new HeroConfig(100006, "张辽", 1, 30, 90, 78, 92, 260, 300, 2, 10, 17, 60, 0, "shi", null, "SwordHitYellowCritical", "zhangliao");
            config[100007] = new HeroConfig(100007, "诸葛亮", 1, 31, 95, 100, 65, 260, 200, 1, 7, 35, 0, 30, "xiang", null, "ExplosionFireballFire", "zhugeliang");
            config[100008] = new HeroConfig(100008, "于禁", 1, 26, 80, 72, 75, 227, 280, 2, 10, 17, 35, 0, "", null, "SwordHitYellowCritical", "yujin");
            config[100009] = new HeroConfig(100009, "夏侯渊", 1, 27, 83, 68, 88, 239, 220, 2, 7, 50, 43, 0, "pao", null, "BulletExplosionFire", "xiahouyuan");
            config[100010] = new HeroConfig(100010, "张欱", 1, 29, 87, 79, 92, 258, 300, 2, 10, 17, 58, 0, "shi", null, "SwordHitYellowCritical", "zhanghe");
            config[100011] = new HeroConfig(100011, "严颜", 1, 25, 75, 74, 82, 231, 280, 1, 10, 17, 38, 0, "shi", null, "SwordHitYellowCritical", "yanyan");
            config[100012] = new HeroConfig(100012, "徐庶", 1, 26, 78, 94, 65, 237, 220, 1, 7, 35, 41, 0, "xiang", null, "StormExplosion", "xusu");
            config[100013] = new HeroConfig(100013, "徐晃", 1, 27, 83, 65, 96, 244, 210, 2, 7, 50, 46, 0, "pao", null, "BulletExplosionFire", "xuhuang");
            config[100014] = new HeroConfig(100014, "曹仁", 1, 29, 88, 72, 84, 244, 320, 2, 10, 17, 46, 0, "shi", null, "SwordHitYellowCritical", "caoren");
            config[100015] = new HeroConfig(100015, "庞德", 1, 28, 84, 70, 94, 248, 290, 2, 15, 17, 49, 0, "ma", null, "SwordHitYellowCritical", "pangde");
            config[100016] = new HeroConfig(100016, "魏延", 1, 27, 83, 75, 90, 248, 300, 1, 10, 17, 49, 0, "che", null, "SwordHitYellowCritical", "weiyan");
            config[100017] = new HeroConfig(100017, "刘备", 1, 28, 85, 82, 75, 242, 320, 1, 10, 17, 0, 100, "shuai", null, "SwordHitYellowCritical", "liubei");
            config[100018] = new HeroConfig(100018, "黄忠", 1, 27, 81, 75, 95, 251, 200, 1, 7, 50, 52, 0, "pao", null, "BulletExplosionFire", "huangzhong");
            config[100019] = new HeroConfig(100019, "马超", 1, 29, 87, 70, 96, 253, 290, 1, 15, 17, 54, 0, "ma", null, "SwordHitYellowCritical", "machao");
            config[100020] = new HeroConfig(100020, "关羽", 1, 30, 92, 77, 97, 266, 300, 1, 10, 17, 0, 40, "che", new int[]{200001}, "SwordHitYellowCritical", "guanyu");
            config[100021] = new HeroConfig(100021, "曹操", 1, 32, 98, 92, 83, 273, 300, 2, 10, 17, 0, 100, "shuai", null, "SwordHitYellowCritical", "caocao");
            config[100022] = new HeroConfig(100022, "曹洪", 1, 25, 75, 68, 80, 223, 290, 2, 10, 17, 33, 0, "", null, "SwordHitYellowCritical", "caohong");
            config[100023] = new HeroConfig(100023, "许褚", 1, 27, 81, 56, 97, 234, 340, 2, 10, 17, 40, 0, "che", null, "SwordHitYellowCritical", "xuchu");
            config[100024] = new HeroConfig(100024, "典韦", 1, 26, 78, 50, 98, 226, 325, 2, 10, 17, 35, 0, "shi", null, "SwordHitYellowCritical", "dianwei");
            config[100025] = new HeroConfig(100025, "马岱", 1, 25, 76, 72, 83, 231, 300, 1, 15, 17, 38, 0, "ma", null, "SwordHitYellowCritical", "madai");
            config[100026] = new HeroConfig(100026, "乐进", 1, 27, 83, 70, 81, 234, 280, 2, 15, 17, 40, 0, "ma", null, "SwordHitYellowCritical", "lejin");
            config[100027] = new HeroConfig(100027, "贾诩", 1, 25, 75, 97, 62, 234, 220, 2, 7, 35, 40, 0, "xiang", null, "StormExplosion", "jiaxu");
            config[100028] = new HeroConfig(100028, "郭嘉", 1, 24, 72, 98, 35, 205, 220, 2, 7, 35, 25, 0, "xiang", null, "FrostExplosionBlue", "guojia");
            config[100029] = new HeroConfig(100029, "曹休", 1, 23, 70, 75, 72, 217, 320, 2, 10, 17, 30, 0, "", null, "SwordHitYellowCritical", "caoxiu");
            config[100030] = new HeroConfig(100030, "关兴", 1, 26, 80, 72, 85, 237, 300, 1, 15, 17, 41, 0, "ma", null, "SwordHitYellowCritical", "guanxing");
            config[100031] = new HeroConfig(100031, "关平", 1, 25, 78, 70, 82, 230, 290, 1, 15, 17, 37, 0, "ma", null, "SwordHitYellowCritical", "guanping");
            config[100032] = new HeroConfig(100032, "司马懿", 1, 30, 96, 99, 75, 270, 230, 2, 7, 35, 70, 0, "xiang", null, "ShadowExplosion", "simayi");
            config[100033] = new HeroConfig(100033, "邓艾", 1, 28, 92, 88, 80, 260, 280, 2, 10, 17, 60, 0, "che", null, "SwordHitYellowCritical", "dengai");
            config[100034] = new HeroConfig(100034, "姜维", 1, 29, 91, 92, 87, 270, 270, 1, 10, 17, 70, 0, "che", null, "SwordHitYellowCritical", "jiangwei");
            config[100035] = new HeroConfig(100035, "廖化", 1, 24, 74, 68, 78, 220, 310, 1, 10, 17, 32, 0, "", null, "SwordHitYellowCritical", "liaohua");
            config[100036] = new HeroConfig(100036, "孟获", 1, 27, 80, 60, 87, 227, 310, 1, 10, 17, 35, 0, "shi", null, "SwordHitYellowCritical", "menghuo");
            config[100037] = new HeroConfig(100037, "庞统", 1, 23, 74, 98, 55, 227, 210, 1, 7, 35, 35, 0, "xiang", null, "ShadowExplosion", "pangtong");
            config[100038] = new HeroConfig(100038, "李严", 1, 26, 83, 75, 80, 238, 290, 1, 10, 17, 42, 0, "shi", null, "SwordHitYellowCritical", "liyan");
            config[100039] = new HeroConfig(100039, "张苞", 1, 25, 75, 65, 84, 224, 320, 1, 10, 17, 34, 0, "che", null, "SwordHitYellowCritical", "zhangbao");
            config[100040] = new HeroConfig(100040, "张松", 1, 22, 65, 89, 70, 224, 260, 1, 10, 17, 34, 0, "", null, "SwordHitYellowCritical", "zhangsong");
            config[100041] = new HeroConfig(100041, "关索", 1, 25, 80, 72, 83, 235, 305, 1, 10, 17, 40, 0, "", null, "SwordHitYellowCritical", "guansuo");
            config[100042] = new HeroConfig(100042, "简雍", 1, 21, 63, 79, 68, 210, 250, 1, 10, 17, 27, 0, "", null, "SwordHitYellowCritical", "jianyong");
            config[100043] = new HeroConfig(100043, "夏侯霸", 1, 26, 85, 69, 77, 231, 320, 2, 10, 17, 38, 0, "", null, "SwordHitYellowCritical", "xiahouba");
            config[100044] = new HeroConfig(100044, "郝昭", 1, 25, 78, 76, 79, 233, 300, 2, 10, 17, 39, 0, "", null, "SwordHitYellowCritical", "haozhao");
            config[100045] = new HeroConfig(100045, "王双", 1, 26, 86, 62, 78, 226, 330, 2, 10, 17, 35, 0, "", null, "SwordHitYellowCritical", "wangshuang");
            config[100046] = new HeroConfig(100046, "程昱", 1, 24, 70, 90, 67, 227, 230, 2, 7, 35, 35, 0, "xiang", null, "StormExplosion", "chengyu");
            config[100047] = new HeroConfig(100047, "蒋琬", 1, 23, 68, 86, 62, 216, 230, 1, 7, 35, 30, 0, "xiang", null, "StormExplosion", "jiangwan");
            config[100048] = new HeroConfig(100048, "杨修", 1, 21, 60, 88, 70, 218, 220, 2, 7, 35, 31, 0, "xiang", null, "StormExplosion", "yangxiu");
            config[100049] = new HeroConfig(100049, "张任", 1, 25, 80, 72, 80, 232, 310, 4, 10, 17, 38, 0, "", null, "SwordHitYellowCritical", "zhangren");
            config[100050] = new HeroConfig(100050, "钟会", 1, 26, 78, 92, 74, 244, 220, 2, 7, 35, 46, 0, "xiang", null, "StormExplosion", "zhonghui");
            config[100051] = new HeroConfig(100051, "牛金", 1, 23, 71, 65, 77, 213, 290, 2, 10, 17, 28, 0, "", null, "SwordHitYellowCritical", "niujin");
            config[100052] = new HeroConfig(100052, "文鸯", 1, 27, 85, 72, 88, 245, 220, 2, 7, 50, 47, 0, "pao", null, "BulletExplosionFire", "wenyuan");
            config[100053] = new HeroConfig(100053, "曹真", 1, 25, 85, 78, 77, 240, 310, 2, 10, 17, 44, 0, "", null, "SwordHitYellowCritical", "caozhen");
            config[100054] = new HeroConfig(100054, "陈群", 1, 22, 65, 84, 60, 209, 270, 2, 10, 17, 26, 0, "", null, "SwordHitYellowCritical", "chenqun");
            config[100055] = new HeroConfig(100055, "李典", 1, 24, 72, 74, 73, 219, 300, 2, 10, 17, 31, 0, "", null, "SwordHitYellowCritical", "lidian");
            config[100056] = new HeroConfig(100056, "曹丕", 1, 25, 74, 83, 75, 232, 320, 2, 10, 17, 38, 0, "", null, "SwordHitYellowCritical", "caopi");
            config[100057] = new HeroConfig(100057, "孙乾", 1, 21, 62, 80, 54, 196, 250, 1, 10, 17, 21, 0, "", null, "SwordHitYellowCritical", "sunqian");
            config[100058] = new HeroConfig(100058, "荀彧", 1, 23, 65, 96, 57, 218, 210, 2, 7, 35, 31, 0, "xiang", null, "StormExplosion", "xunyu");
            config[100059] = new HeroConfig(100059, "荀攸", 1, 24, 60, 92, 67, 219, 225, 2, 7, 35, 31, 0, "xiang", null, "StormExplosion", "xunyou");
            config[100060] = new HeroConfig(100060, "曹植", 1, 22, 64, 83, 67, 214, 270, 2, 10, 17, 29, 0, "", null, "SwordHitYellowCritical", "caozhi");
            config[100061] = new HeroConfig(100061, "费祎", 1, 23, 68, 86, 59, 213, 240, 1, 7, 35, 28, 0, "xiang", null, "StormExplosion", "feiyi");
            config[100062] = new HeroConfig(100062, "刘晔", 1, 24, 72, 88, 60, 220, 230, 2, 7, 35, 32, 0, "xiang", null, "StormExplosion", "liuye");
            config[100063] = new HeroConfig(100063, "马谡", 1, 22, 66, 84, 72, 222, 270, 1, 10, 17, 33, 0, "", null, "SwordHitYellowCritical", "masu");
            config[100064] = new HeroConfig(100064, "董允", 1, 23, 67, 85, 65, 217, 280, 1, 10, 17, 30, 0, "", null, "SwordHitYellowCritical", "dongyun");
            config[100065] = new HeroConfig(100065, "王平", 1, 24, 76, 70, 78, 224, 300, 1, 10, 17, 34, 0, "", null, "SwordHitYellowCritical", "wangping");
            config[100066] = new HeroConfig(100066, "邓芝", 1, 23, 70, 80, 71, 221, 290, 1, 10, 17, 32, 0, "", null, "SwordHitYellowCritical", "dengzhi");
            config[100067] = new HeroConfig(100067, "郭攸之", 1, 21, 63, 82, 58, 203, 260, 1, 10, 17, 24, 0, "", null, "SwordHitYellowCritical", "guoyouzhi");
            config[100068] = new HeroConfig(100068, "马良", 1, 22, 65, 85, 60, 210, 250, 1, 7, 35, 27, 0, "xiang", null, "StormExplosion", "maliang");
            config[100069] = new HeroConfig(100069, "吕布", 1, 30, 85, 50, 100, 235, 350, 4, 15, 17, 0, 30, "ma", null, "SwordHitYellowCritical", "lvbu");
            config[100070] = new HeroConfig(100070, "颜良", 1, 28, 78, 45, 92, 215, 330, 4, 15, 17, 29, 0, "shi", null, "SwordHitYellowCritical", "yanliang");
            config[100071] = new HeroConfig(100071, "文丑", 1, 27, 76, 48, 90, 214, 320, 4, 10, 17, 29, 0, "shi", null, "SwordHitYellowCritical", "wenchou");
            config[100072] = new HeroConfig(100072, "华雄", 1, 26, 75, 42, 88, 205, 310, 4, 10, 17, 25, 0, "shi", null, "SwordHitYellowCritical", "huaxiong");
            config[100073] = new HeroConfig(100073, "孙尚香", 1, 24, 70, 65, 78, 213, 240, 3, 7, 50, 28, 0, "pao", null, "BulletExplosionFire", "sunshangxiang");
            config[100074] = new HeroConfig(100074, "黄月英", 1, 21, 60, 80, 50, 190, 250, 1, 10, 17, 20, 0, "", null, "SwordHitYellowCritical", "huangyueying");
            config[100075] = new HeroConfig(100075, "孙策", 1, 28, 89, 69, 95, 253, 340, 3, 10, 17, 54, 0, "che", null, "SwordHitYellowCritical", "sunce");
            config[100076] = new HeroConfig(100076, "公孙瓒", 1, 25, 85, 62, 81, 228, 320, 4, 15, 17, 36, 0, "ma", null, "SwordHitYellowCritical", "gongsunzan");
            config[100077] = new HeroConfig(100077, "孙坚", 1, 27, 82, 70, 86, 238, 330, 3, 10, 17, 42, 0, "", null, "SwordHitYellowCritical", "sunjian");
            config[100078] = new HeroConfig(100078, "孙权", 1, 25, 85, 87, 73, 245, 290, 3, 10, 17, 0, 100, "shuai", null, "SwordHitYellowCritical", "sunquan");
            config[100079] = new HeroConfig(100079, "太史慈", 1, 26, 78, 65, 91, 234, 220, 3, 7, 50, 40, 0, "pao", null, "BulletExplosionFire", "taishici");
            config[100080] = new HeroConfig(100080, "甘宁", 1, 27, 80, 60, 93, 233, 210, 3, 7, 50, 39, 0, "pao", null, "BulletExplosionFire", "ganning");
            config[100081] = new HeroConfig(100081, "黄盖", 1, 24, 78, 70, 83, 231, 320, 3, 10, 17, 38, 0, "shi", null, "SwordHitYellowCritical", "huanggai");
            config[100082] = new HeroConfig(100082, "周泰", 1, 25, 76, 55, 87, 218, 325, 3, 10, 17, 31, 0, "shi", null, "SwordHitYellowCritical", "zhoutai");
            config[100083] = new HeroConfig(100083, "鲁肃", 1, 23, 75, 93, 65, 233, 240, 3, 7, 35, 39, 0, "xiang", null, "StormExplosion", "lusu");
            config[100084] = new HeroConfig(100084, "周瑜", 1, 26, 82, 95, 75, 252, 220, 3, 7, 35, 53, 0, "xiang", null, "ExplosionFireballFire", "zhouyu");
            config[100085] = new HeroConfig(100085, "貂蝉", 1, 22, 65, 80, 65, 210, 280, 4, 10, 17, 27, 0, "", null, "SwordHitYellowCritical", "diaochan");
            config[100086] = new HeroConfig(100086, "法正", 1, 24, 70, 88, 65, 223, 290, 1, 10, 17, 33, 0, "", null, "SwordHitYellowCritical", "fazheng");
            config[100087] = new HeroConfig(100087, "蒋钦", 1, 23, 74, 60, 78, 212, 310, 3, 10, 17, 28, 0, "", null, "SwordHitYellowCritical", "jiangqing");
            config[100088] = new HeroConfig(100088, "刘禅", 1, 20, 40, 50, 45, 135, 250, 1, 10, 17, 8, 0, "", null, "SwordHitYellowCritical", "liushan");
            config[100089] = new HeroConfig(100089, "吕蒙", 1, 25, 85, 80, 82, 247, 330, 3, 10, 17, 49, 0, "", null, "SwordHitYellowCritical", "lvmeng");
            config[100090] = new HeroConfig(100090, "陆逊", 1, 24, 82, 92, 75, 249, 250, 3, 7, 35, 50, 0, "xiang", null, "StormExplosion", "luxun");
            config[100091] = new HeroConfig(100091, "张昭", 1, 22, 65, 87, 60, 212, 235, 3, 7, 35, 28, 0, "xiang", null, "StormExplosion", "zhangzhao");
            config[100092] = new HeroConfig(100092, "诸葛瑾", 1, 23, 70, 85, 65, 220, 290, 3, 10, 17, 32, 0, "", null, "SwordHitYellowCritical", "zhugejin");
            config[100093] = new HeroConfig(100093, "朱桓", 1, 24, 78, 70, 80, 228, 320, 3, 10, 17, 36, 0, "", null, "SwordHitYellowCritical", "zhuhuan");
            config[100094] = new HeroConfig(100094, "祝融", 1, 25, 75, 60, 85, 220, 330, 2, 10, 17, 32, 0, "", null, "SwordHitYellowCritical", "zhurong");
            config[100095] = new HeroConfig(100095, "大乔", 1, 21, 65, 82, 70, 217, 280, 3, 10, 17, 30, 0, "", null, "SwordHitYellowCritical", "daqiao");
            config[100096] = new HeroConfig(100096, "小乔", 1, 20, 60, 85, 65, 210, 270, 3, 10, 17, 27, 0, "", null, "SwordHitYellowCritical", "xiaoqiao");
            config[100097] = new HeroConfig(100097, "司马师", 1, 26, 80, 82, 78, 240, 340, 2, 10, 17, 44, 0, "", null, "SwordHitYellowCritical", "simashi");
            config[100098] = new HeroConfig(100098, "司马昭", 1, 25, 78, 85, 75, 238, 330, 2, 10, 17, 42, 0, "", null, "SwordHitYellowCritical", "simazhao");
            config[100099] = new HeroConfig(100099, "丁奉", 1, 24, 76, 68, 82, 226, 320, 3, 10, 17, 35, 0, "", null, "SwordHitYellowCritical", "dingfeng");
            config[100100] = new HeroConfig(100100, "董袭", 1, 23, 74, 60, 80, 214, 310, 3, 10, 17, 29, 0, "", null, "SwordHitYellowCritical", "dongxi");
            config[100101] = new HeroConfig(100101, "凌统", 1, 24, 75, 65, 83, 223, 315, 3, 10, 17, 33, 0, "", null, "SwordHitYellowCritical", "lingtong");
            config[100102] = new HeroConfig(100102, "潘璋", 1, 23, 72, 58, 78, 208, 305, 3, 10, 17, 26, 0, "", null, "SwordHitYellowCritical", "panzhang");
            config[100103] = new HeroConfig(100103, "董卓", 1, 26, 70, 50, 85, 205, 350, 4, 10, 17, 25, 0, "", null, "SwordHitYellowCritical", "dongzhuo");
            config[100104] = new HeroConfig(100104, "朱灵", 1, 23, 73, 60, 77, 210, 300, 2, 10, 17, 27, 0, "", null, "SwordHitYellowCritical", "zhuling");
            config[100105] = new HeroConfig(100105, "朱治", 1, 22, 70, 75, 68, 213, 290, 3, 10, 17, 28, 0, "", null, "SwordHitYellowCritical", "zhuzhi");
            config[100106] = new HeroConfig(100106, "华佗", 1, 20, 60, 85, 50, 195, 250, 4, 10, 17, 21, 0, "", null, "SwordHitYellowCritical", "huatuo");
            config[100107] = new HeroConfig(100107, "徐盛", 1, 24, 76, 70, 80, 226, 300, 3, 10, 17, 35, 0, "", null, "SwordHitYellowCritical", "xusheng");
            config[100108] = new HeroConfig(100108, "程普", 1, 25, 80, 72, 82, 234, 300, 3, 10, 17, 40, 0, "", null, "SwordHitYellowCritical", "chengpu");
            config[100109] = new HeroConfig(100109, "程武", 1, 22, 68, 75, 70, 213, 290, 3, 10, 17, 28, 0, "", null, "SwordHitYellowCritical", "chengwu");
            config[100110] = new HeroConfig(100110, "顾雍", 1, 21, 65, 81, 60, 206, 270, 3, 10, 17, 25, 0, "", null, "SwordHitYellowCritical", "guyong");
            config[100111] = new HeroConfig(100111, "步骘", 1, 22, 70, 80, 65, 215, 290, 3, 10, 17, 29, 0, "", null, "SwordHitYellowCritical", "buzhi");
            config[100112] = new HeroConfig(100112, "阚泽", 1, 20, 62, 84, 55, 201, 260, 3, 10, 17, 23, 0, "", null, "SwordHitYellowCritical", "kanze");

        }

        public static HeroConfig GetConfig(uint id)
        {
            HeroConfig data;
            if (config.TryGetValue(id, out data))
            {
                return data;
            }
            throw new NullReferenceException(string.Format("配置表HeroConfig不存在id={0}", id));
        }

        public static bool HasConfig(uint id)
        {
            if (config.ContainsKey(id))
            {
                return true;
            }
            return false;
        }

        public static void Assign(uint id, HeroConfig configData)
        {
            config[id] = configData; 
        }

        public static void Add(uint id, HeroConfig configData)
        {
            if (!config.ContainsKey(id))
            {
                config.Add(id, configData);
            }
        }

        public static void Remove(uint id)
        {
            if (config.ContainsKey(id))
            {
                config.Remove(id);
            }
        }
    }
}
