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
        public int Id;
        /// <summary>
        ///名字
        /// </summary>
        public string Name;
        /// <summary>
        ///说明
        /// </summary>
        public string Descript;
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
        ///范围
        /// </summary>
        public float Range;
        /// <summary>
        ///技能强度
        /// </summary>
        public float Strength;
        /// <summary>
        ///BuffId
        /// </summary>
        public int BuffId;
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
        ///图标
        /// </summary>
        public string Icon;


        public SkillConfig(int Id, string Name, string Descript, int Lv, float Rate, float CD, float Range, float Strength, int BuffId, int BuffTime, string ScriptName, string HitEffect, int Price, string Icon)
        {
            this.Id = Id;
            this.Name = Name;
            this.Descript = Descript;
            this.Lv = Lv;
            this.Rate = Rate;
            this.CD = CD;
            this.Range = Range;
            this.Strength = Strength;
            this.BuffId = BuffId;
            this.BuffTime = BuffTime;
            this.ScriptName = ScriptName;
            this.HitEffect = HitEffect;
            this.Price = Price;
            this.Icon = Icon;

        }

        public SkillConfig() { }

        private static Dictionary<int, SkillConfig> config = new Dictionary<int, SkillConfig>();
        public static Dictionary<int, SkillConfig>.ValueCollection ConfigList
        {
            get
            {
                return config.Values;
            }
        }

        public static void Refresh(Dictionary<int, SkillConfig> dict)
        {
            config.Clear();
            config = dict;
        }

        public static void Load()
        {
            config.Clear();
            config[200001] = new SkillConfig(200001, "旋风斩", "攻击对附近敌人造成35%伤害", 1, 0.35f, 6f, 20f, 0.3f, 0, 0, "SpinAttack", "SwordWhirlwindWhite", 3, "meng");
            config[200002] = new SkillConfig(200002, "愤怒一击", "攻击30%几率造成大量伤害", 1, 0.3f, 5f, 0, 0.01f, 0, 0, "CriticalAttack", "SwordHitRedCritical", 3, "che");
            config[200003] = new SkillConfig(200003, "主公技", "给与我方同阵营单位20%生命值护盾", 1, 0, 0, 0, 0.2f, 300001, 999, "MasterShield", "", 4, "shuai");
            config[200004] = new SkillConfig(200004, "坚硬皮肤", "受击时几率发动减伤", 1, 0.3f, 7f, 0, 0.4f, 300002, 5, "HardSkin", "", 2, "shi");
            config[200005] = new SkillConfig(200005, "突破", "移动时穿越敌人，降低远程伤害", 1, 1f, 7f, 20f, 0.1f, 0, 0, "RunCross", "LightningExplosionBlue", 3, "ma");
            config[200006] = new SkillConfig(200006, "部署", "射程较远,提升士兵等级", 1, 0, 0, 0, 1f, 0, 0, "SoldierUp", "MagicChargeYellow", 2, "xiang");
            config[200007] = new SkillConfig(200007, "射击", "射程很远", 1, 1f, 99f, 0, 0, 0, 0, "Dumb", "", 2, "pao");
            config[201001] = new SkillConfig(201001, "治疗", "给与友军治疗", 1, 1f, 3f, 50f, 0.3f, 0, 0, "Heal", "MagicBuffGreen", 3, "heal");
            config[201002] = new SkillConfig(201002, "鼓舞", "给与友军攻速祝福", 1, 1f, 3f, 50f, 0.2f, 0, 0, "Song", "MagicChargePink", 3, "song");
            config[201003] = new SkillConfig(201003, "富甲一方", "参战获得5金币", 1, 0, 0, 0, 5f, 0, 0, "Gold", "MagicChargeYellow", 5, "gold");
            config[201004] = new SkillConfig(201004, "反伤", "反弹30%近战伤害", 1, 0, 0, 20f, 0.3f, 0, 0, "Feedback", "SwordHitBlue", 3, "ci");
            config[201005] = new SkillConfig(201005, "狙击", "射程非常远", 1, 1f, 99f, 0, 0, 0, 0, "Dumb", "", 3, "ju");
            config[201006] = new SkillConfig(201006, "连射", "攻击近距离目标时40%触发连射", 1, 0.4f, 3f, 25f, 0.5f, 0, 0, "SpeedAttack", "", 2, "lian");
            config[201007] = new SkillConfig(201007, "藤甲", "受非智力攻击时高几率发动70%减伤", 1, 0.7f, 1f, 0, 0.7f, 0, 0, "PlantSkin", "", 4, "teng");
            config[201008] = new SkillConfig(201008, "多重箭", "攻击时40%发出2只箭", 1, 0.4f, 4f, 22f, 0, 0, 0, "MultiArrow", "", 3, "duo");
            config[201009] = new SkillConfig(201009, "指导", "提升队伍最低武将智力", 1, 0, 0, 0, 0.4f, 0, 0, "Help", "MagicChargeYellow", 2, "shi2");

        }

        public static SkillConfig GetConfig(int id)
        {
            SkillConfig data;
            if (config.TryGetValue(id, out data))
            {
                return data;
            }
            throw new NullReferenceException(string.Format("配置表SkillConfig不存在id={0}", id));
        }

        public static bool HasConfig(int id)
        {
            if (config.ContainsKey(id))
            {
                return true;
            }
            return false;
        }

        public static void Assign(int id, SkillConfig configData)
        {
            config[id] = configData; 
        }

        public static void Add(int id, SkillConfig configData)
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
