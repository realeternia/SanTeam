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
            config[400001] = new ItemConfig(400001, "关王刀", 1, "str", 5, "", 0, null, "SwordHitYellowCritical", 20, "guanwangdao");

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
