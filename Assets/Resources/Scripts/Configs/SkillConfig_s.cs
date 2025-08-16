using System;
using System.Collections;
using System.Collections.Generic;

namespace CommonConfig
{
    public class SkillConfig
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
        ///发动概率
        /// </summary>
        public float Rate;
        /// <summary>
        ///发动cd
        /// </summary>
        public float CD;
        /// <summary>
        ///技能强度
        /// </summary>
        public float Strength;
        /// <summary>
        ///BuffId
        /// </summary>
        public uint BuffId;
        /// <summary>
        ///BuffLast
        /// </summary>
        public int BuffTime;
        /// <summary>
        ///脚本名
        /// </summary>
        public string ScriptName;
        /// <summary>
        ///hit
        /// </summary>
        public string HitEffect;
        /// <summary>
        ///价值
        /// </summary>
        public int Price;
        /// <summary>
        ///背景图
        /// </summary>
        public string Icon;


        public SkillConfig(uint Id, string Name, int Lv, float Rate, float CD, float Strength, uint BuffId, int BuffTime, string ScriptName, string HitEffect, int Price, string Icon)
        {
            this.Id = Id;
            this.Name = Name;
            this.Lv = Lv;
            this.Rate = Rate;
            this.CD = CD;
            this.Strength = Strength;
            this.BuffId = BuffId;
            this.BuffTime = BuffTime;
            this.ScriptName = ScriptName;
            this.HitEffect = HitEffect;
            this.Price = Price;
            this.Icon = Icon;

        }

        public SkillConfig() { }

        private static Dictionary<uint, SkillConfig> config = new Dictionary<uint, SkillConfig>();
        public static Dictionary<uint, SkillConfig>.ValueCollection ConfigList
        {
            get
            {
                return config.Values;
            }
        }

        public static void Refresh(Dictionary<uint, SkillConfig> dict)
        {
            config.Clear();
            config = dict;
        }

        public static void Load()
        {
            config.Clear();
            config[200001] = new SkillConfig(200001, "转转转", 1, 0.2f, 5f, 0.6f, 0, 0, "SpinAttack", "SwordWhirlwindWhite", 5, "spinattack");
            config[200002] = new SkillConfig(200002, "车愤怒一击", 1, 0.2f, 5f, 0.5f, 0, 0, "CriticalAttack", "SwordHitRedCritical", 2, "crtattack");
            config[200003] = new SkillConfig(200003, "主公技", 1, 0, 0, 0.2f, 300001, 999, "MasterShield", "", 4, "");
            config[200004] = new SkillConfig(200004, "士技", 1, 0, 0, 0.3f, 0, 0, "HardSkin", "", 2, "");
            config[200005] = new SkillConfig(200005, "马技-突破", 1, 1f, 7f, 0.3f, 0, 0, "RunCross", "LightningMissileBlue", 2, "");

        }

        public static SkillConfig GetConfig(uint id)
        {
            SkillConfig data;
            if (config.TryGetValue(id, out data))
            {
                return data;
            }
            throw new NullReferenceException(string.Format("配置表SkillConfig不存在id={0}", id));
        }

        public static bool HasConfig(uint id)
        {
            if (config.ContainsKey(id))
            {
                return true;
            }
            return false;
        }

        public static void Assign(uint id, SkillConfig configData)
        {
            config[id] = configData; 
        }

        public static void Add(uint id, SkillConfig configData)
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
