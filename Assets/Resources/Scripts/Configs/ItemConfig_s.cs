using System;
using System.Collections;
using System.Collections.Generic;

namespace CommonConfig
{
    public class ItemConfig
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
        ///属性1
        /// </summary>
        public string Attr1;
        /// <summary>
        ///属性1值
        /// </summary>
        public int Attr1Val;
        /// <summary>
        ///属性2
        /// </summary>
        public string Attr2;
        /// <summary>
        ///属性2值
        /// </summary>
        public int Attr2Val;
        /// <summary>
        ///技能
        /// </summary>
        public int[] Skills;
        /// <summary>
        ///hit
        /// </summary>
        public string HitEffect;
        /// <summary>
        ///价格
        /// </summary>
        public int Price;
        /// <summary>
        ///背景图
        /// </summary>
        public string Icon;


        public ItemConfig(int Id, string Name, int Lv, string Attr1, int Attr1Val, string Attr2, int Attr2Val, int[] Skills, string HitEffect, int Price, string Icon)
        {
            this.Id = Id;
            this.Name = Name;
            this.Lv = Lv;
            this.Attr1 = Attr1;
            this.Attr1Val = Attr1Val;
            this.Attr2 = Attr2;
            this.Attr2Val = Attr2Val;
            this.Skills = Skills;
            this.HitEffect = HitEffect;
            this.Price = Price;
            this.Icon = Icon;

        }

        public ItemConfig() { }

        private static Dictionary<int, ItemConfig> config = new Dictionary<int, ItemConfig>();
        public static Dictionary<int, ItemConfig>.ValueCollection ConfigList
        {
            get
            {
                return config.Values;
            }
        }

        public static void Refresh(Dictionary<int, ItemConfig> dict)
        {
            config.Clear();
            config = dict;
        }

        public static void Load()
        {
            config.Clear();
            config[400001] = new ItemConfig(400001, "关王刀", 1, "str", 10, "", 0, null, "", 25, "guanwangdao");
            config[400002] = new ItemConfig(400002, "方天画戟", 1, "str", 15, "", 0, null, "", 40, "fangtian");
            config[400003] = new ItemConfig(400003, "丈八蛇矛", 1, "str", 11, "", 0, null, "", 27, "zhangba");
            config[400004] = new ItemConfig(400004, "檀木弓", 1, "str", 6, "", 0, null, "", 15, "tanmugong");
            config[400005] = new ItemConfig(400005, "大斧", 1, "str", 6, "", 0, null, "", 15, "dafu");
            config[400006] = new ItemConfig(400006, "三丈枪", 1, "str", 6, "", 0, null, "", 15, "sanzhangqiang");
            config[400007] = new ItemConfig(400007, "孙子兵法", 1, "lead", 15, "", 0, null, "", 40, "sunzi");
            config[400008] = new ItemConfig(400008, "墨子", 1, "lead", 6, "", 0, null, "", 15, "mozi");
            config[400009] = new ItemConfig(400009, "六韬", 1, "lead", 10, "", 0, null, "", 25, "liutao");
            config[400010] = new ItemConfig(400010, "诗经", 1, "inte", 6, "", 0, null, "", 15, "shijing");
            config[400011] = new ItemConfig(400011, "易经", 1, "inte", 12, "", 0, null, "", 30, "yijing");

        }

        public static ItemConfig GetConfig(int id)
        {
            ItemConfig data;
            if (config.TryGetValue(id, out data))
            {
                return data;
            }
            throw new NullReferenceException(string.Format("配置表ItemConfig不存在id={0}", id));
        }

        public static bool HasConfig(int id)
        {
            if (config.ContainsKey(id))
            {
                return true;
            }
            return false;
        }

        public static void Assign(int id, ItemConfig configData)
        {
            config[id] = configData; 
        }

        public static void Add(int id, ItemConfig configData)
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
