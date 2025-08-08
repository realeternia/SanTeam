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
            config[100001] = new HeroConfig(100001, "刘备", 1, 27, 81, 82, 75, 238, 320, 1, 10, 17, 0, 100, "shuai", null, "SwordHitYellowCritical", "liubei");
            config[100002] = new HeroConfig(100002, "曹操", 1, 31, 94, 92, 81, 267, 280, 2, 10, 17, 0, 100, "shuai", null, "SwordHitYellowCritical", "caocao");
            config[100003] = new HeroConfig(100003, "孙权", 1, 25, 85, 87, 70, 242, 310, 3, 10, 17, 0, 100, "shuai", null, "SwordHitYellowCritical", "sunquan");
            config[101001] = new HeroConfig(101001, "赵云", 1, 28, 85, 86, 97, 268, 280, 1, 10, 17, 0, 40, "shi", null, "SwordHitYellowCritical", "zhaoyun");
            config[101002] = new HeroConfig(101002, "张飞", 1, 27, 82, 65, 98, 245, 330, 1, 10, 17, 0, 30, "che", new int[]{200002}, "SwordHitYellowCritical", "zhangfei");
            config[101003] = new HeroConfig(101003, "马超", 1, 29, 87, 70, 96, 253, 290, 1, 15, 17, 54, 0, "ma", null, "SwordHitYellowCritical", "machao");
            config[101004] = new HeroConfig(101004, "诸葛亮", 1, 31, 95, 100, 65, 260, 200, 1, 7, 35, 0, 30, "xiang", null, "ExplosionFireballFire", "zhugeliang");
            config[101005] = new HeroConfig(101005, "关羽", 1, 30, 92, 77, 97, 266, 300, 1, 10, 17, 0, 40, "che", new int[]{200001}, "SwordHitYellowCritical", "guanyu");
            config[101006] = new HeroConfig(101006, "徐庶", 1, 26, 78, 94, 65, 237, 220, 1, 7, 35, 41, 0, "xiang", null, "StormExplosion", "xusu");
            config[101007] = new HeroConfig(101007, "魏延", 1, 27, 83, 75, 90, 248, 300, 1, 10, 17, 49, 0, "che", null, "SwordHitYellowCritical", "weiyan");
            config[101008] = new HeroConfig(101008, "黄忠", 1, 27, 81, 75, 95, 251, 200, 1, 7, 50, 52, 0, "pao", null, "BulletExplosionFire", "huangzhong");
            config[101009] = new HeroConfig(101009, "周仓", 1, 22, 68, 60, 76, 204, 280, 1, 10, 17, 24, 0, "", null, "SwordHitYellowCritical", "zhoucang");
            config[101010] = new HeroConfig(101010, "姜维", 1, 29, 91, 92, 87, 270, 270, 1, 10, 17, 70, 0, "che", null, "SwordHitYellowCritical", "jiangwei");
            config[101011] = new HeroConfig(101011, "马岱", 1, 25, 76, 72, 83, 231, 300, 1, 15, 17, 38, 0, "ma", null, "SwordHitYellowCritical", "madai");
            config[101012] = new HeroConfig(101012, "关兴", 1, 26, 80, 72, 85, 237, 300, 1, 15, 17, 41, 0, "ma", null, "SwordHitYellowCritical", "guanxing");
            config[101013] = new HeroConfig(101013, "关平", 1, 25, 78, 70, 82, 230, 290, 1, 15, 17, 37, 0, "ma", null, "SwordHitYellowCritical", "guanping");
            config[101014] = new HeroConfig(101014, "严颜", 1, 25, 75, 74, 82, 231, 280, 1, 10, 17, 38, 0, "shi", null, "SwordHitYellowCritical", "yanyan");
            config[101015] = new HeroConfig(101015, "廖化", 1, 24, 74, 68, 78, 220, 310, 1, 10, 17, 32, 0, "", null, "SwordHitYellowCritical", "liaohua");
            config[101016] = new HeroConfig(101016, "孟获", 1, 27, 80, 60, 87, 227, 310, 1, 10, 17, 35, 0, "shi", null, "SwordHitYellowCritical", "menghuo");
            config[101017] = new HeroConfig(101017, "庞统", 1, 23, 74, 98, 55, 227, 210, 1, 7, 35, 35, 0, "xiang", null, "ShadowExplosion", "pangtong");
            config[101018] = new HeroConfig(101018, "李严", 1, 26, 83, 75, 80, 238, 290, 1, 10, 17, 42, 0, "shi", null, "SwordHitYellowCritical", "liyan");
            config[101019] = new HeroConfig(101019, "张苞", 1, 25, 75, 65, 84, 224, 320, 1, 10, 17, 34, 0, "che", null, "SwordHitYellowCritical", "zhangbao");
            config[101020] = new HeroConfig(101020, "张松", 1, 22, 65, 89, 70, 224, 260, 1, 10, 17, 34, 0, "", null, "SwordHitYellowCritical", "zhangsong");
            config[101021] = new HeroConfig(101021, "关索", 1, 25, 80, 72, 83, 235, 305, 1, 10, 17, 40, 0, "", null, "SwordHitYellowCritical", "guansuo");
            config[101022] = new HeroConfig(101022, "简雍", 1, 21, 63, 79, 68, 210, 250, 1, 10, 17, 27, 0, "", null, "SwordHitYellowCritical", "jianyong");
            config[101023] = new HeroConfig(101023, "蒋琬", 1, 23, 68, 86, 62, 216, 230, 1, 7, 35, 30, 0, "xiang", null, "StormExplosion", "jiangwan");
            config[101024] = new HeroConfig(101024, "孙乾", 1, 21, 62, 80, 54, 196, 250, 1, 10, 17, 21, 0, "", null, "SwordHitYellowCritical", "sunqian");
            config[101025] = new HeroConfig(101025, "费祎", 1, 23, 68, 86, 59, 213, 240, 1, 7, 35, 28, 0, "xiang", null, "StormExplosion", "feiyi");
            config[101026] = new HeroConfig(101026, "马谡", 1, 22, 66, 84, 72, 222, 270, 1, 10, 17, 33, 0, "", null, "SwordHitYellowCritical", "masu");
            config[101027] = new HeroConfig(101027, "董允", 1, 23, 67, 85, 65, 217, 280, 1, 10, 17, 30, 0, "", null, "SwordHitYellowCritical", "dongyun");
            config[101028] = new HeroConfig(101028, "王平", 1, 24, 76, 70, 78, 224, 300, 1, 10, 17, 34, 0, "", null, "SwordHitYellowCritical", "wangping");
            config[101029] = new HeroConfig(101029, "邓芝", 1, 23, 70, 80, 71, 221, 290, 1, 10, 17, 32, 0, "", null, "SwordHitYellowCritical", "dengzhi");
            config[101030] = new HeroConfig(101030, "郭攸之", 1, 21, 63, 82, 58, 203, 260, 1, 10, 17, 24, 0, "", null, "SwordHitYellowCritical", "guoyouzhi");
            config[101031] = new HeroConfig(101031, "马良", 1, 22, 65, 85, 60, 210, 250, 1, 7, 35, 27, 0, "xiang", null, "StormExplosion", "maliang");
            config[101032] = new HeroConfig(101032, "黄月英", 1, 21, 60, 80, 50, 190, 250, 1, 10, 17, 20, 0, "", null, "SwordHitYellowCritical", "huangyueying");
            config[101033] = new HeroConfig(101033, "法正", 1, 24, 70, 88, 65, 223, 290, 1, 10, 17, 33, 0, "", null, "SwordHitYellowCritical", "fazheng");
            config[101034] = new HeroConfig(101034, "刘禅", 1, 20, 40, 50, 45, 135, 250, 1, 10, 17, 8, 0, "", null, "SwordHitYellowCritical", "liushan");
            config[101035] = new HeroConfig(101035, "祝融", 1, 25, 75, 60, 85, 220, 330, 1, 10, 17, 32, 0, "", null, "SwordHitYellowCritical", "zhurong");
            config[102001] = new HeroConfig(102001, "郭嘉", 1, 24, 72, 98, 35, 205, 220, 2, 7, 35, 25, 0, "xiang", null, "FrostExplosionBlue", "guojia");
            config[102002] = new HeroConfig(102002, "夏侯淳", 1, 28, 85, 70, 90, 245, 300, 2, 10, 17, 47, 0, "che", null, "SwordHitYellowCritical", "xiahoudun");
            config[102003] = new HeroConfig(102003, "荀彧", 1, 23, 65, 96, 57, 218, 210, 2, 7, 35, 31, 0, "xiang", null, "StormExplosion", "xunyu");
            config[102004] = new HeroConfig(102004, "张辽", 1, 30, 90, 78, 92, 260, 300, 2, 10, 17, 60, 0, "shi", null, "SwordHitYellowCritical", "zhangliao");
            config[102005] = new HeroConfig(102005, "许褚", 1, 27, 81, 56, 97, 234, 340, 2, 10, 17, 40, 0, "che", null, "SwordHitYellowCritical", "xuchu");
            config[102006] = new HeroConfig(102006, "夏侯渊", 1, 27, 83, 68, 88, 239, 220, 2, 7, 50, 43, 0, "pao", null, "BulletExplosionFire", "xiahouyuan");
            config[102007] = new HeroConfig(102007, "典韦", 1, 26, 78, 50, 98, 226, 325, 2, 10, 17, 35, 0, "shi", null, "SwordHitYellowCritical", "dianwei");
            config[102008] = new HeroConfig(102008, "张欱", 1, 29, 87, 79, 92, 258, 300, 2, 10, 17, 58, 0, "shi", null, "SwordHitYellowCritical", "zhanghe");
            config[102009] = new HeroConfig(102009, "徐晃", 1, 27, 83, 65, 96, 244, 210, 2, 7, 50, 46, 0, "pao", null, "BulletExplosionFire", "xuhuang");
            config[102010] = new HeroConfig(102010, "荀攸", 1, 24, 60, 92, 67, 219, 225, 2, 7, 35, 31, 0, "xiang", null, "StormExplosion", "xunyou");
            config[102011] = new HeroConfig(102011, "于禁", 1, 26, 80, 72, 75, 227, 280, 2, 10, 17, 35, 0, "", null, "SwordHitYellowCritical", "yujin");
            config[102012] = new HeroConfig(102012, "曹仁", 1, 29, 88, 72, 84, 244, 320, 2, 10, 17, 46, 0, "shi", null, "SwordHitYellowCritical", "caoren");
            config[102013] = new HeroConfig(102013, "曹洪", 1, 25, 75, 68, 80, 223, 290, 2, 10, 17, 33, 0, "", null, "SwordHitYellowCritical", "caohong");
            config[102014] = new HeroConfig(102014, "庞德", 1, 28, 84, 70, 94, 248, 290, 2, 15, 17, 49, 0, "ma", null, "SwordHitYellowCritical", "pangde");
            config[102015] = new HeroConfig(102015, "乐进", 1, 27, 83, 70, 81, 234, 280, 2, 15, 17, 40, 0, "ma", null, "SwordHitYellowCritical", "lejin");
            config[102016] = new HeroConfig(102016, "贾诩", 1, 25, 75, 97, 62, 234, 220, 2, 7, 35, 40, 0, "xiang", null, "StormExplosion", "jiaxu");
            config[102017] = new HeroConfig(102017, "文聘", 1, 24, 72, 65, 78, 215, 270, 2, 10, 17, 29, 0, "", null, "SwordHitYellowCritical", "wenpin");
            config[102018] = new HeroConfig(102018, "曹休", 1, 23, 70, 75, 72, 217, 320, 2, 10, 17, 30, 0, "", null, "SwordHitYellowCritical", "caoxiu");
            config[102019] = new HeroConfig(102019, "司马懿", 1, 30, 96, 99, 75, 270, 230, 2, 7, 35, 70, 0, "xiang", null, "ShadowExplosion", "simayi");
            config[102020] = new HeroConfig(102020, "邓艾", 1, 28, 92, 88, 80, 260, 280, 2, 10, 17, 60, 0, "che", null, "SwordHitYellowCritical", "dengai");
            config[102021] = new HeroConfig(102021, "司马师", 1, 26, 80, 82, 78, 240, 340, 2, 10, 17, 44, 0, "", null, "SwordHitYellowCritical", "simashi");
            config[102022] = new HeroConfig(102022, "夏侯霸", 1, 26, 85, 69, 77, 231, 320, 2, 10, 17, 38, 0, "", null, "SwordHitYellowCritical", "xiahouba");
            config[102023] = new HeroConfig(102023, "司马昭", 1, 25, 78, 85, 75, 238, 330, 2, 10, 17, 42, 0, "", null, "SwordHitYellowCritical", "simazhao");
            config[102024] = new HeroConfig(102024, "郝昭", 1, 25, 78, 76, 79, 233, 300, 2, 10, 17, 39, 0, "", null, "SwordHitYellowCritical", "haozhao");
            config[102025] = new HeroConfig(102025, "王双", 1, 26, 86, 62, 78, 226, 330, 2, 10, 17, 35, 0, "", null, "SwordHitYellowCritical", "wangshuang");
            config[102026] = new HeroConfig(102026, "程昱", 1, 24, 70, 90, 67, 227, 230, 2, 7, 35, 35, 0, "xiang", null, "StormExplosion", "chengyu");
            config[102027] = new HeroConfig(102027, "杨修", 1, 21, 60, 88, 70, 218, 220, 2, 7, 35, 31, 0, "xiang", null, "StormExplosion", "yangxiu");
            config[102028] = new HeroConfig(102028, "钟会", 1, 26, 78, 92, 74, 244, 220, 2, 7, 35, 46, 0, "xiang", null, "StormExplosion", "zhonghui");
            config[102029] = new HeroConfig(102029, "牛金", 1, 23, 71, 65, 77, 213, 290, 2, 10, 17, 28, 0, "", null, "SwordHitYellowCritical", "niujin");
            config[102030] = new HeroConfig(102030, "文鸯", 1, 27, 85, 72, 88, 245, 220, 2, 7, 50, 47, 0, "pao", null, "BulletExplosionFire", "wenyuan");
            config[102031] = new HeroConfig(102031, "曹真", 1, 25, 85, 78, 77, 240, 310, 2, 10, 17, 44, 0, "", null, "SwordHitYellowCritical", "caozhen");
            config[102032] = new HeroConfig(102032, "陈群", 1, 22, 65, 84, 60, 209, 270, 2, 10, 17, 26, 0, "", null, "SwordHitYellowCritical", "chenqun");
            config[102033] = new HeroConfig(102033, "李典", 1, 24, 72, 74, 73, 219, 300, 2, 10, 17, 31, 0, "", null, "SwordHitYellowCritical", "lidian");
            config[102034] = new HeroConfig(102034, "曹丕", 1, 25, 74, 83, 75, 232, 320, 2, 10, 17, 38, 0, "", null, "SwordHitYellowCritical", "caopi");
            config[102035] = new HeroConfig(102035, "曹植", 1, 22, 64, 83, 67, 214, 270, 2, 10, 17, 29, 0, "", null, "SwordHitYellowCritical", "caozhi");
            config[102036] = new HeroConfig(102036, "刘晔", 1, 24, 72, 88, 60, 220, 230, 2, 7, 35, 32, 0, "xiang", null, "StormExplosion", "liuye");
            config[102037] = new HeroConfig(102037, "朱灵", 1, 23, 73, 60, 77, 210, 300, 2, 10, 17, 27, 0, "", null, "SwordHitYellowCritical", "zhuling");
            config[103001] = new HeroConfig(103001, "孙坚", 1, 27, 82, 70, 86, 238, 330, 3, 10, 17, 42, 0, "", null, "SwordHitYellowCritical", "sunjian");
            config[103002] = new HeroConfig(103002, "孙策", 1, 28, 89, 69, 95, 253, 340, 3, 10, 17, 54, 0, "che", null, "SwordHitYellowCritical", "sunce");
            config[103003] = new HeroConfig(103003, "甘宁", 1, 27, 80, 60, 93, 233, 210, 3, 7, 50, 39, 0, "pao", null, "BulletExplosionFire", "ganning");
            config[103004] = new HeroConfig(103004, "太史慈", 1, 26, 78, 65, 91, 234, 220, 3, 7, 50, 40, 0, "pao", null, "BulletExplosionFire", "taishici");
            config[103005] = new HeroConfig(103005, "黄盖", 1, 24, 78, 70, 83, 231, 320, 3, 10, 17, 38, 0, "shi", null, "SwordHitYellowCritical", "huanggai");
            config[103006] = new HeroConfig(103006, "周泰", 1, 25, 76, 55, 87, 218, 325, 3, 10, 17, 31, 0, "shi", null, "SwordHitYellowCritical", "zhoutai");
            config[103007] = new HeroConfig(103007, "鲁肃", 1, 23, 75, 93, 65, 233, 240, 3, 7, 35, 39, 0, "xiang", null, "StormExplosion", "lusu");
            config[103008] = new HeroConfig(103008, "周瑜", 1, 26, 82, 95, 75, 252, 220, 3, 7, 35, 53, 0, "xiang", null, "ExplosionFireballFire", "zhouyu");
            config[103009] = new HeroConfig(103009, "蒋钦", 1, 23, 74, 60, 78, 212, 310, 3, 10, 17, 28, 0, "", null, "SwordHitYellowCritical", "jiangqing");
            config[103010] = new HeroConfig(103010, "吕蒙", 1, 25, 85, 80, 82, 247, 330, 3, 10, 17, 49, 0, "", null, "SwordHitYellowCritical", "lvmeng");
            config[103011] = new HeroConfig(103011, "陆逊", 1, 24, 82, 92, 75, 249, 250, 3, 7, 35, 50, 0, "xiang", null, "StormExplosion", "luxun");
            config[103012] = new HeroConfig(103012, "张昭", 1, 22, 65, 87, 60, 212, 235, 3, 7, 35, 28, 0, "xiang", null, "StormExplosion", "zhangzhao");
            config[103013] = new HeroConfig(103013, "诸葛瑾", 1, 23, 70, 85, 65, 220, 290, 3, 10, 17, 32, 0, "", null, "SwordHitYellowCritical", "zhugejin");
            config[103014] = new HeroConfig(103014, "孙尚香", 1, 24, 70, 65, 78, 213, 240, 3, 7, 50, 28, 0, "pao", null, "BulletExplosionFire", "sunshangxiang");
            config[103015] = new HeroConfig(103015, "朱桓", 1, 24, 78, 70, 80, 228, 320, 3, 10, 17, 36, 0, "", null, "SwordHitYellowCritical", "zhuhuan");
            config[103016] = new HeroConfig(103016, "大乔", 1, 21, 65, 82, 70, 217, 280, 3, 10, 17, 30, 0, "", null, "SwordHitYellowCritical", "daqiao");
            config[103017] = new HeroConfig(103017, "小乔", 1, 20, 60, 85, 65, 210, 270, 3, 10, 17, 27, 0, "", null, "SwordHitYellowCritical", "xiaoqiao");
            config[103018] = new HeroConfig(103018, "丁奉", 1, 24, 76, 68, 82, 226, 320, 3, 10, 17, 35, 0, "", null, "SwordHitYellowCritical", "dingfeng");
            config[103019] = new HeroConfig(103019, "董袭", 1, 23, 74, 60, 80, 214, 310, 3, 10, 17, 29, 0, "", null, "SwordHitYellowCritical", "dongxi");
            config[103020] = new HeroConfig(103020, "凌统", 1, 24, 75, 65, 83, 223, 315, 3, 10, 17, 33, 0, "", null, "SwordHitYellowCritical", "lingtong");
            config[103021] = new HeroConfig(103021, "潘璋", 1, 23, 72, 58, 78, 208, 305, 3, 10, 17, 26, 0, "", null, "SwordHitYellowCritical", "panzhang");
            config[103022] = new HeroConfig(103022, "朱治", 1, 22, 70, 75, 68, 213, 290, 3, 10, 17, 28, 0, "", null, "SwordHitYellowCritical", "zhuzhi");
            config[103023] = new HeroConfig(103023, "徐盛", 1, 24, 76, 70, 80, 226, 300, 3, 10, 17, 35, 0, "", null, "SwordHitYellowCritical", "xusheng");
            config[103024] = new HeroConfig(103024, "程普", 1, 25, 80, 72, 82, 234, 300, 3, 10, 17, 40, 0, "", null, "SwordHitYellowCritical", "chengpu");
            config[103025] = new HeroConfig(103025, "程武", 1, 22, 68, 75, 70, 213, 290, 3, 10, 17, 28, 0, "", null, "SwordHitYellowCritical", "chengwu");
            config[103026] = new HeroConfig(103026, "顾雍", 1, 21, 65, 81, 60, 206, 270, 3, 10, 17, 25, 0, "", null, "SwordHitYellowCritical", "guyong");
            config[103027] = new HeroConfig(103027, "步骘", 1, 22, 70, 80, 65, 215, 290, 3, 10, 17, 29, 0, "", null, "SwordHitYellowCritical", "buzhi");
            config[103028] = new HeroConfig(103028, "阚泽", 1, 20, 62, 84, 55, 201, 260, 3, 10, 17, 23, 0, "", null, "SwordHitYellowCritical", "kanze");
            config[104001] = new HeroConfig(104001, "董卓", 1, 26, 70, 50, 85, 205, 350, 4, 10, 17, 25, 0, "", null, "SwordHitYellowCritical", "dongzhuo");
            config[104002] = new HeroConfig(104002, "吕布", 1, 30, 85, 50, 100, 235, 350, 4, 15, 17, 0, 30, "ma", null, "SwordHitYellowCritical", "lvbu");
            config[104003] = new HeroConfig(104003, "颜良", 1, 28, 78, 45, 92, 215, 330, 4, 15, 17, 29, 0, "shi", null, "SwordHitYellowCritical", "yanliang");
            config[104004] = new HeroConfig(104004, "文丑", 1, 27, 76, 48, 90, 214, 320, 4, 10, 17, 29, 0, "shi", null, "SwordHitYellowCritical", "wenchou");
            config[104005] = new HeroConfig(104005, "华雄", 1, 26, 75, 42, 88, 205, 310, 4, 10, 17, 25, 0, "shi", null, "SwordHitYellowCritical", "huaxiong");
            config[104006] = new HeroConfig(104006, "公孙瓒", 1, 25, 85, 62, 81, 228, 320, 4, 15, 17, 36, 0, "ma", null, "SwordHitYellowCritical", "gongsunzan");
            config[104007] = new HeroConfig(104007, "貂蝉", 1, 22, 65, 80, 65, 210, 280, 4, 10, 17, 27, 0, "", null, "SwordHitYellowCritical", "diaochan");
            config[104008] = new HeroConfig(104008, "张任", 1, 25, 80, 72, 80, 232, 310, 4, 10, 17, 38, 0, "", null, "SwordHitYellowCritical", "zhangren");
            config[104009] = new HeroConfig(104009, "华佗", 1, 20, 60, 85, 50, 195, 250, 4, 10, 17, 21, 0, "", null, "SwordHitYellowCritical", "huatuo");

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
