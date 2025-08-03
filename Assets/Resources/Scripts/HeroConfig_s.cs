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
        public int Rate;
        /// <summary>
        ///职业
        /// </summary>
        public string Job;
        /// <summary>
        ///hit
        /// </summary>
        public string HitEffect;
        /// <summary>
        ///背景图
        /// </summary>
        public string Icon;


        public HeroConfig(uint Id, string Name, int Lv, int Atk, int LeadShip, int Inte, int Str, int Total, int Hp, int Side, int MoveSpeed, int Range, int Rate, string Job, string HitEffect, string Icon)
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
            this.Rate = Rate;
            this.Job = Job;
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
            config[100001] = new HeroConfig(100001, "文聘", 1, 24, 72, 65, 78, 215, 270, 2, 10, 17, 29, "", "SwordHitYellowCritical", "wenpin");
            config[100002] = new HeroConfig(100002, "夏侯惇", 1, 28, 85, 70, 90, 245, 300, 2, 10, 17, 47, "che", "SwordHitYellowCritical", "xiahoudun");
            config[100003] = new HeroConfig(100003, "赵云", 1, 28, 85, 86, 97, 268, 280, 1, 10, 17, 68, "shi", "SwordHitYellowCritical", "zhaoyun");
            config[100004] = new HeroConfig(100004, "张飞", 1, 27, 82, 65, 98, 245, 330, 1, 10, 17, 47, "che", "SwordHitYellowCritical", "zhangfei");
            config[100005] = new HeroConfig(100005, "周仓", 1, 22, 68, 60, 76, 204, 280, 1, 10, 17, 24, "", "SwordHitYellowCritical", "zhoucang");
            config[100006] = new HeroConfig(100006, "张辽", 1, 30, 90, 78, 92, 260, 300, 2, 10, 17, 60, "shi", "SwordHitYellowCritical", "zhangliao");
            config[100007] = new HeroConfig(100007, "诸葛亮", 1, 31, 95, 100, 65, 260, 180, 1, 7, 35, 60, "xiang", "ExplosionFireballFire", "zhugeliang");
            config[100008] = new HeroConfig(100008, "于禁", 1, 26, 80, 72, 75, 227, 280, 2, 10, 17, 35, "", "SwordHitYellowCritical", "yujin");
            config[100009] = new HeroConfig(100009, "夏侯渊", 1, 27, 83, 68, 88, 239, 210, 2, 7, 50, 43, "pao", "BulletExplosionFire", "xiahouyuan");
            config[100010] = new HeroConfig(100010, "张郃", 1, 29, 87, 79, 92, 258, 300, 2, 10, 17, 58, "shi", "SwordHitYellowCritical", "zhanghe");
            config[100011] = new HeroConfig(100011, "严颜", 1, 25, 75, 74, 82, 231, 280, 1, 10, 17, 38, "shi", "SwordHitYellowCritical", "yanyan");
            config[100012] = new HeroConfig(100012, "徐庶", 1, 26, 78, 94, 65, 237, 220, 1, 7, 35, 41, "xiang", "StormExplosion", "xusu");
            config[100013] = new HeroConfig(100013, "徐晃", 1, 27, 83, 65, 96, 244, 200, 2, 7, 50, 46, "pao", "BulletExplosionFire", "xuhuang");
            config[100014] = new HeroConfig(100014, "曹仁", 1, 29, 88, 72, 84, 244, 320, 2, 10, 17, 46, "shi", "SwordHitYellowCritical", "caoren");
            config[100015] = new HeroConfig(100015, "庞德", 1, 28, 84, 70, 94, 248, 290, 2, 15, 17, 49, "ma", "SwordHitYellowCritical", "pangde");
            config[100016] = new HeroConfig(100016, "魏延", 1, 27, 83, 75, 90, 248, 300, 1, 10, 17, 49, "che", "SwordHitYellowCritical", "weiyan");
            config[100017] = new HeroConfig(100017, "刘备", 1, 28, 85, 82, 75, 242, 320, 1, 10, 17, 45, "shuai", "SwordHitYellowCritical", "liubei");
            config[100018] = new HeroConfig(100018, "黄忠", 1, 27, 81, 75, 95, 251, 190, 1, 7, 50, 52, "pao", "BulletExplosionFire", "huangzhong");
            config[100019] = new HeroConfig(100019, "马超", 1, 29, 87, 70, 96, 253, 290, 1, 15, 17, 54, "ma", "SwordHitYellowCritical", "machao");
            config[100020] = new HeroConfig(100020, "关羽", 1, 30, 92, 77, 97, 266, 300, 1, 10, 17, 66, "che", "SwordHitYellowCritical", "guanyu");
            config[100021] = new HeroConfig(100021, "曹操", 1, 32, 98, 92, 83, 273, 300, 2, 10, 17, 74, "shuai", "SwordHitYellowCritical", "caocao");
            config[100022] = new HeroConfig(100022, "曹洪", 1, 25, 75, 68, 80, 223, 290, 2, 10, 17, 33, "", "SwordHitYellowCritical", "caohong");
            config[100023] = new HeroConfig(100023, "许褚", 1, 27, 81, 56, 97, 234, 340, 2, 10, 17, 40, "che", "SwordHitYellowCritical", "xuchu");
            config[100024] = new HeroConfig(100024, "典韦", 1, 26, 78, 50, 98, 226, 325, 2, 10, 17, 35, "shi", "SwordHitYellowCritical", "dianwei");
            config[100025] = new HeroConfig(100025, "马岱", 1, 25, 76, 72, 83, 231, 300, 1, 15, 17, 38, "ma", "SwordHitYellowCritical", "madai");
            config[100026] = new HeroConfig(100026, "乐进", 1, 27, 83, 70, 81, 234, 280, 2, 15, 17, 40, "ma", "SwordHitYellowCritical", "lejin");
            config[100027] = new HeroConfig(100027, "贾诩", 1, 25, 75, 97, 62, 234, 200, 2, 7, 35, 40, "xiang", "StormExplosion", "jiaxu");
            config[100028] = new HeroConfig(100028, "郭嘉", 1, 24, 72, 98, 35, 205, 210, 2, 7, 35, 25, "xiang", "FrostExplosionBlue", "guojia");
            config[100029] = new HeroConfig(100029, "曹休", 1, 23, 70, 75, 72, 217, 320, 2, 10, 17, 30, "", "SwordHitYellowCritical", "caoxiu");
            config[100030] = new HeroConfig(100030, "关兴", 1, 26, 80, 72, 85, 237, 300, 1, 15, 17, 41, "ma", "SwordHitYellowCritical", "guanxing");
            config[100031] = new HeroConfig(100031, "关平", 1, 25, 78, 70, 82, 230, 290, 1, 15, 17, 37, "ma", "SwordHitYellowCritical", "guanping");
            config[100032] = new HeroConfig(100032, "司马懿", 1, 30, 96, 99, 75, 270, 210, 2, 7, 35, 70, "xiang", "ShadowExplosion", "simayi");
            config[100033] = new HeroConfig(100033, "邓艾", 1, 28, 92, 88, 80, 260, 280, 2, 10, 17, 60, "che", "SwordHitYellowCritical", "dengai");
            config[100034] = new HeroConfig(100034, "姜维", 1, 29, 91, 92, 87, 270, 270, 1, 10, 17, 70, "che", "SwordHitYellowCritical", "jiangwei");
            config[100035] = new HeroConfig(100035, "廖化", 1, 24, 74, 68, 78, 220, 310, 1, 10, 17, 32, "", "SwordHitYellowCritical", "liaohua");
            config[100036] = new HeroConfig(100036, "孟获", 1, 27, 80, 60, 87, 227, 310, 1, 10, 17, 35, "shi", "SwordHitYellowCritical", "menghuo");
            config[100037] = new HeroConfig(100037, "庞统", 1, 23, 74, 98, 55, 227, 200, 1, 7, 35, 35, "xiang", "ShadowExplosion", "pangtong");
            config[100038] = new HeroConfig(100038, "李严", 1, 26, 83, 75, 80, 238, 290, 1, 10, 17, 42, "shi", "SwordHitYellowCritical", "liyan");
            config[100039] = new HeroConfig(100039, "张苞", 1, 25, 75, 65, 84, 224, 320, 1, 10, 17, 34, "che", "SwordHitYellowCritical", "zhangbao");
            config[100040] = new HeroConfig(100040, "张松", 1, 22, 65, 89, 70, 224, 260, 1, 10, 17, 34, "", "SwordHitYellowCritical", "zhangsong");
            config[100041] = new HeroConfig(100041, "关索", 1, 25, 80, 72, 83, 235, 305, 1, 10, 17, 40, "", "SwordHitYellowCritical", "guansuo");
            config[100042] = new HeroConfig(100042, "简雍", 1, 21, 63, 79, 68, 210, 250, 1, 10, 17, 27, "", "SwordHitYellowCritical", "jianyong");
            config[100043] = new HeroConfig(100043, "夏侯霸", 1, 26, 85, 69, 77, 231, 320, 1, 10, 17, 38, "", "SwordHitYellowCritical", "xiahouba");
            config[100044] = new HeroConfig(100044, "郝昭", 1, 25, 78, 76, 79, 233, 300, 1, 10, 17, 39, "", "SwordHitYellowCritical", "haozhao");
            config[100045] = new HeroConfig(100045, "王双", 1, 26, 86, 62, 78, 226, 330, 1, 10, 17, 35, "", "SwordHitYellowCritical", "wangshuang");
            config[100046] = new HeroConfig(100046, "程昱", 1, 24, 70, 90, 67, 227, 230, 1, 7, 35, 35, "xiang", "StormExplosion", "chengyu");
            config[100047] = new HeroConfig(100047, "蒋琬", 1, 23, 68, 86, 62, 216, 230, 1, 7, 35, 30, "xiang", "StormExplosion", "jiangwan");
            config[100048] = new HeroConfig(100048, "杨修", 1, 21, 60, 88, 70, 218, 220, 1, 7, 35, 31, "xiang", "StormExplosion", "yangxiu");
            config[100049] = new HeroConfig(100049, "张任", 1, 25, 80, 72, 80, 232, 310, 1, 10, 17, 38, "", "SwordHitYellowCritical", "zhangren");
            config[100050] = new HeroConfig(100050, "钟会", 1, 26, 78, 92, 74, 244, 220, 1, 7, 35, 46, "xiang", "StormExplosion", "zhonghui");
            config[100051] = new HeroConfig(100051, "牛金", 1, 23, 71, 65, 77, 213, 290, 1, 10, 17, 28, "", "SwordHitYellowCritical", "niujin");
            config[100052] = new HeroConfig(100052, "文鸯", 1, 27, 85, 72, 88, 245, 340, 1, 10, 17, 47, "che", "SwordHitYellowCritical", "wenyuan");
            config[100053] = new HeroConfig(100053, "曹真", 1, 25, 85, 78, 77, 240, 310, 1, 10, 17, 44, "", "SwordHitYellowCritical", "caozhen");
            config[100054] = new HeroConfig(100054, "陈群", 1, 22, 65, 84, 60, 209, 270, 1, 10, 17, 26, "", "SwordHitYellowCritical", "chenqun");
            config[100055] = new HeroConfig(100055, "李典", 1, 24, 72, 74, 73, 219, 300, 1, 10, 17, 31, "", "SwordHitYellowCritical", "lidian");
            config[100056] = new HeroConfig(100056, "曹丕", 1, 25, 74, 83, 75, 232, 320, 1, 10, 17, 38, "", "SwordHitYellowCritical", "caopi");
            config[100057] = new HeroConfig(100057, "孙乾", 1, 21, 62, 80, 54, 196, 250, 1, 10, 17, 21, "", "SwordHitYellowCritical", "sunqian");
            config[100058] = new HeroConfig(100058, "荀彧", 1, 23, 65, 96, 57, 218, 180, 1, 7, 35, 31, "xiang", "StormExplosion", "xunyu");
            config[100059] = new HeroConfig(100059, "荀攸", 1, 24, 60, 92, 67, 219, 225, 1, 7, 35, 31, "xiang", "StormExplosion", "xunyou");
            config[100060] = new HeroConfig(100060, "曹植", 1, 22, 64, 83, 67, 214, 270, 1, 10, 17, 29, "", "SwordHitYellowCritical", "caozhi");
            config[100061] = new HeroConfig(100061, "费祎", 1, 23, 68, 86, 59, 213, 240, 1, 7, 35, 28, "xiang", "StormExplosion", "feiyi");
            config[100062] = new HeroConfig(100062, "刘晔", 1, 24, 72, 88, 60, 220, 230, 1, 7, 35, 32, "xiang", "StormExplosion", "liuye");
            config[100063] = new HeroConfig(100063, "马谡", 1, 22, 66, 84, 72, 222, 270, 1, 10, 17, 33, "", "SwordHitYellowCritical", "masu");
            config[100064] = new HeroConfig(100064, "董允", 1, 23, 67, 85, 65, 217, 280, 1, 10, 17, 30, "", "SwordHitYellowCritical", "dongyun");
            config[100065] = new HeroConfig(100065, "王平", 1, 24, 76, 70, 78, 224, 300, 1, 10, 17, 34, "", "SwordHitYellowCritical", "wangping");
            config[100066] = new HeroConfig(100066, "邓芝", 1, 23, 70, 80, 71, 221, 290, 1, 10, 17, 32, "", "SwordHitYellowCritical", "dengzhi");
            config[100067] = new HeroConfig(100067, "郭攸之", 1, 21, 63, 82, 58, 203, 260, 1, 10, 17, 24, "", "SwordHitYellowCritical", "guoyouzhi");
            config[100068] = new HeroConfig(100068, "马良", 1, 22, 65, 85, 60, 210, 250, 1, 7, 35, 27, "xiang", "StormExplosion", "maliang");

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
