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
        ///背景图
        /// </summary>
        public string Icon;


        public HeroConfig(uint Id, string Name, int Lv, int Atk, int LeadShip, int Inte, int Str, int Total, int Hp, int Side, int MoveSpeed, int Range, int Price, string Icon)
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
            config[100001] = new HeroConfig(100001, "文聘", 1, 24, 72, 65, 78, 215, 300, 2, 10, 15, 3, "wenpin");
            config[100002] = new HeroConfig(100002, "夏侯惇", 1, 28, 85, 70, 90, 245, 375, 2, 10, 15, 7, "xiahoudun");
            config[100003] = new HeroConfig(100003, "赵云", 1, 27, 83, 75, 97, 255, 350, 1, 10, 15, 9, "zhaoyun");
            config[100004] = new HeroConfig(100004, "张飞", 1, 27, 82, 60, 98, 240, 420, 1, 10, 15, 7, "zhangfei");
            config[100005] = new HeroConfig(100005, "周仓", 1, 22, 68, 60, 76, 204, 350, 1, 10, 15, 3, "zhoucang");
            config[100006] = new HeroConfig(100006, "张辽", 1, 30, 90, 78, 92, 260, 380, 2, 10, 15, 9, "zhangliao");
            config[100007] = new HeroConfig(100007, "诸葛亮", 1, 31, 95, 100, 65, 260, 290, 1, 10, 15, 9, "zhugeliang");
            config[100008] = new HeroConfig(100008, "于禁", 1, 26, 80, 72, 75, 227, 360, 2, 10, 15, 4, "yujin");
            config[100009] = new HeroConfig(100009, "夏侯渊", 1, 27, 83, 68, 88, 239, 390, 2, 10, 15, 6, "xiahouyuan");
            config[100010] = new HeroConfig(100010, "张郃", 1, 29, 87, 74, 92, 253, 330, 2, 10, 15, 8, "zhanghe");
            config[100011] = new HeroConfig(100011, "严颜", 1, 25, 75, 70, 82, 227, 360, 1, 10, 15, 4, "yanyan");
            config[100012] = new HeroConfig(100012, "徐庶", 1, 26, 78, 94, 65, 237, 310, 1, 10, 15, 6, "xusu");
            config[100013] = new HeroConfig(100013, "徐晃", 1, 26, 80, 55, 96, 231, 430, 2, 10, 15, 5, "xuhuang");
            config[100014] = new HeroConfig(100014, "曹仁", 1, 28, 86, 72, 84, 242, 410, 2, 10, 15, 7, "caoren");
            config[100015] = new HeroConfig(100015, "庞德", 1, 28, 84, 70, 94, 248, 400, 2, 10, 15, 8, "pangde");
            config[100016] = new HeroConfig(100016, "魏延", 1, 27, 83, 75, 89, 247, 380, 1, 10, 15, 8, "weiyan");
            config[100017] = new HeroConfig(100017, "刘备", 1, 29, 88, 82, 75, 245, 400, 1, 10, 15, 7, "liubei");
            config[100018] = new HeroConfig(100018, "黄忠", 1, 27, 81, 75, 95, 251, 330, 1, 10, 15, 8, "huangzhong");
            config[100019] = new HeroConfig(100019, "马超", 1, 29, 87, 70, 96, 253, 410, 1, 10, 15, 8, "machao");
            config[100020] = new HeroConfig(100020, "关羽", 1, 30, 92, 75, 97, 264, 400, 1, 10, 15, 10, "guanyu");
            config[100021] = new HeroConfig(100021, "曹操", 1, 32, 98, 92, 85, 275, 300, 2, 10, 15, 10, "caocao");
            config[100022] = new HeroConfig(100022, "曹洪", 1, 25, 75, 68, 80, 223, 370, 2, 10, 15, 4, "caohong");
            config[100023] = new HeroConfig(100023, "许褚", 1, 27, 81, 56, 97, 234, 450, 2, 10, 15, 6, "xuchu");
            config[100024] = new HeroConfig(100024, "典韦", 1, 26, 78, 50, 98, 226, 460, 2, 10, 15, 4, "dianwei");
            config[100025] = new HeroConfig(100025, "马岱", 1, 25, 76, 72, 83, 231, 380, 1, 10, 15, 5, "madai");
            config[100026] = new HeroConfig(100026, "乐进", 1, 26, 79, 70, 85, 234, 360, 2, 10, 15, 6, "lejin");
            config[100027] = new HeroConfig(100027, "贾诩", 1, 25, 75, 97, 62, 234, 330, 2, 10, 15, 6, "jiaxu");
            config[100028] = new HeroConfig(100028, "郭嘉", 1, 24, 72, 98, 35, 205, 340, 2, 10, 15, 3, "guojia");
            config[100029] = new HeroConfig(100029, "曹休", 1, 23, 70, 75, 72, 217, 400, 2, 10, 15, 3, "caoxiu");
            config[100030] = new HeroConfig(100030, "关兴", 1, 26, 80, 72, 85, 237, 380, 1, 10, 15, 6, "guanxing");
            config[100031] = new HeroConfig(100031, "关平", 1, 25, 78, 70, 82, 230, 370, 1, 10, 15, 5, "guanping");
            config[100032] = new HeroConfig(100032, "司马懿", 1, 30, 96, 99, 75, 270, 320, 2, 10, 15, 10, "simayi");
            config[100033] = new HeroConfig(100033, "邓艾", 1, 28, 89, 88, 80, 257, 360, 2, 10, 15, 9, "dengai");
            config[100034] = new HeroConfig(100034, "姜维", 1, 29, 90, 92, 87, 269, 340, 1, 10, 15, 10, "jiangwei");
            config[100035] = new HeroConfig(100035, "廖化", 1, 24, 75, 68, 78, 221, 390, 1, 10, 15, 4, "liaohua");
            config[100036] = new HeroConfig(100036, "孟获", 1, 27, 82, 60, 90, 232, 450, 1, 10, 15, 5, "menghuo");
            config[100037] = new HeroConfig(100037, "庞统", 1, 23, 79, 98, 55, 232, 280, 1, 10, 15, 5, "pangtong");
            config[100038] = new HeroConfig(100038, "李严", 1, 26, 83, 75, 82, 240, 370, 1, 10, 15, 7, "liyan");
            config[100039] = new HeroConfig(100039, "张苞", 1, 25, 77, 65, 84, 226, 400, 1, 10, 15, 4, "zhangbao");

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
