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
        ///价格
        /// </summary>
        public int Price;
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


        public HeroConfig(uint Id, string Name, int Lv, int Atk, int LeadShip, int Inte, int Str, int Total, int Hp, int Side, int MoveSpeed, int Range, int Price, string Job, string HitEffect, string Icon)
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
            this.Price = Price;
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
            config[100004] = new HeroConfig(100004, "张飞", 1, 27, 82, 60, 98, 240, 330, 1, 10, 17, 7, "che", "SwordHitYellowCritical", "zhangfei");
            config[100020] = new HeroConfig(100020, "关羽", 1, 30, 92, 75, 97, 264, 300, 1, 10, 17, 10, "che", "SwordHitYellowCritical", "guanyu");
            config[100023] = new HeroConfig(100023, "许褚", 1, 27, 81, 56, 97, 234, 340, 2, 10, 17, 6, "che", "SwordHitYellowCritical", "xuchu");
            config[100002] = new HeroConfig(100002, "夏侯惇", 1, 28, 85, 70, 90, 245, 300, 2, 10, 17, 7, "che", "SwordHitYellowCritical", "xiahoudun");
            config[100016] = new HeroConfig(100016, "魏延", 1, 27, 83, 75, 89, 247, 300, 1, 10, 17, 8, "che", "SwordHitYellowCritical", "weiyan");
            config[100034] = new HeroConfig(100034, "姜维", 1, 29, 90, 92, 87, 269, 270, 1, 10, 17, 10, "che", "SwordHitYellowCritical", "jiangwei");
            config[100039] = new HeroConfig(100039, "张苞", 1, 25, 77, 65, 84, 226, 320, 1, 10, 17, 4, "che", "SwordHitYellowCritical", "zhangbao");
            config[100033] = new HeroConfig(100033, "邓艾", 1, 28, 89, 88, 80, 257, 280, 2, 10, 17, 9, "che", "SwordHitYellowCritical", "dengai");
            config[100019] = new HeroConfig(100019, "马超", 1, 29, 87, 70, 96, 253, 290, 1, 15, 17, 8, "ma", "SwordHitYellowCritical", "machao");
            config[100030] = new HeroConfig(100030, "关兴", 1, 26, 80, 72, 85, 237, 300, 1, 15, 17, 6, "ma", "SwordHitYellowCritical", "guanxing");
            config[100026] = new HeroConfig(100026, "乐进", 1, 26, 79, 70, 85, 234, 280, 2, 15, 17, 6, "ma", "SwordHitYellowCritical", "lejin");
            config[100025] = new HeroConfig(100025, "马岱", 1, 25, 76, 72, 83, 231, 300, 1, 15, 17, 5, "ma", "SwordHitYellowCritical", "madai");
            config[100031] = new HeroConfig(100031, "关平", 1, 25, 78, 70, 82, 230, 290, 1, 15, 17, 5, "ma", "SwordHitYellowCritical", "guanping");
            config[100015] = new HeroConfig(100015, "庞德", 1, 28, 84, 70, 94, 248, 290, 2, 15, 17, 8, "ma", "SwordHitYellowCritical", "pangde");
            config[100018] = new HeroConfig(100018, "黄忠", 1, 27, 81, 75, 95, 251, 190, 1, 7, 50, 8, "pao", "BulletExplosionFire", "huangzhong");
            config[100009] = new HeroConfig(100009, "夏侯渊", 1, 27, 83, 68, 88, 239, 210, 2, 7, 50, 6, "pao", "BulletExplosionFire", "xiahouyuan");
            config[100013] = new HeroConfig(100013, "徐晃", 1, 26, 80, 55, 96, 231, 200, 2, 7, 50, 5, "pao", "BulletExplosionFire", "xuhuang");
            config[100024] = new HeroConfig(100024, "典韦", 1, 26, 78, 50, 98, 226, 325, 2, 10, 17, 4, "shi", "SwordHitYellowCritical", "dianwei");
            config[100003] = new HeroConfig(100003, "赵云", 1, 27, 83, 75, 97, 255, 280, 1, 10, 17, 9, "shi", "SwordHitYellowCritical", "zhaoyun");
            config[100006] = new HeroConfig(100006, "张辽", 1, 30, 90, 78, 92, 260, 300, 2, 10, 17, 9, "shi", "SwordHitYellowCritical", "zhangliao");
            config[100010] = new HeroConfig(100010, "张郃", 1, 29, 87, 74, 92, 253, 300, 2, 10, 17, 8, "shi", "SwordHitYellowCritical", "zhanghe");
            config[100036] = new HeroConfig(100036, "孟获", 1, 27, 82, 60, 90, 232, 310, 1, 10, 17, 5, "shi", "SwordHitYellowCritical", "menghuo");
            config[100014] = new HeroConfig(100014, "曹仁", 1, 28, 86, 72, 84, 242, 320, 2, 10, 17, 7, "shi", "SwordHitYellowCritical", "caoren");
            config[100038] = new HeroConfig(100038, "李严", 1, 26, 83, 75, 82, 240, 290, 1, 10, 17, 7, "shi", "SwordHitYellowCritical", "liyan");
            config[100011] = new HeroConfig(100011, "严颜", 1, 25, 75, 70, 82, 227, 280, 1, 10, 17, 4, "shi", "SwordHitYellowCritical", "yanyan");
            config[100021] = new HeroConfig(100021, "曹操", 1, 32, 98, 92, 85, 275, 300, 2, 10, 17, 10, "shuai", "SwordHitYellowCritical", "caocao");
            config[100017] = new HeroConfig(100017, "刘备", 1, 29, 88, 82, 75, 245, 320, 1, 10, 17, 7, "shuai", "SwordHitYellowCritical", "liubei");
            config[100032] = new HeroConfig(100032, "司马懿", 1, 30, 96, 99, 75, 270, 210, 2, 7, 35, 10, "xiang", "ShadowExplosion", "simayi");
            config[100007] = new HeroConfig(100007, "诸葛亮", 1, 31, 95, 100, 65, 260, 180, 1, 7, 35, 9, "xiang", "ExplosionFireballFire", "zhugeliang");
            config[100012] = new HeroConfig(100012, "徐庶", 1, 26, 78, 94, 65, 237, 220, 1, 7, 35, 6, "xiang", "StormExplosion", "xusu");
            config[100027] = new HeroConfig(100027, "贾诩", 1, 25, 75, 97, 62, 234, 200, 2, 7, 35, 6, "xiang", "StormExplosion", "jiaxu");
            config[100037] = new HeroConfig(100037, "庞统", 1, 23, 79, 98, 55, 232, 200, 1, 7, 35, 5, "xiang", "ShadowExplosion", "pangtong");
            config[100028] = new HeroConfig(100028, "郭嘉", 1, 24, 72, 98, 35, 205, 210, 2, 7, 35, 3, "xiang", "FrostExplosionBlue", "guojia");
            config[100022] = new HeroConfig(100022, "曹洪", 1, 25, 75, 68, 80, 223, 290, 2, 10, 17, 4, "", "SwordHitYellowCritical", "caohong");
            config[100035] = new HeroConfig(100035, "廖化", 1, 24, 75, 68, 78, 221, 310, 1, 10, 17, 4, "", "SwordHitYellowCritical", "liaohua");
            config[100001] = new HeroConfig(100001, "文聘", 1, 24, 72, 65, 78, 215, 270, 2, 10, 17, 3, "", "SwordHitYellowCritical", "wenpin");
            config[100005] = new HeroConfig(100005, "周仓", 1, 22, 68, 60, 76, 204, 280, 1, 10, 17, 3, "", "SwordHitYellowCritical", "zhoucang");
            config[100008] = new HeroConfig(100008, "于禁", 1, 26, 80, 72, 75, 227, 280, 2, 10, 17, 4, "", "SwordHitYellowCritical", "yujin");
            config[100029] = new HeroConfig(100029, "曹休", 1, 23, 70, 75, 72, 217, 320, 2, 10, 17, 3, "", "SwordHitYellowCritical", "caoxiu");

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
