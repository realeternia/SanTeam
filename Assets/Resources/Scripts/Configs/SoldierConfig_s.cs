using System;
using System.Collections;
using System.Collections.Generic;

namespace CommonConfig
{
    public class SoldierConfig
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
        ///生命
        /// </summary>
        public int Hp;
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
        ///是否隐藏
        /// </summary>
        public bool IsShadow;
        /// <summary>
        ///士兵加成攻击系数
        /// </summary>
        public float SoldierAtkRate;
        /// <summary>
        ///士兵加成hp系数
        /// </summary>
        public float SoldierHpRate;
        /// <summary>
        ///技能
        /// </summary>
        public int[] Skills;
        /// <summary>
        ///模型
        /// </summary>
        public string Model;
        /// <summary>
        ///hit
        /// </summary>
        public string HitEffect;


        public SoldierConfig(int Id, string Name, int Lv, int Atk, int Hp, int MoveSpeed, int Range, int MissileSpeed, bool IsShadow, float SoldierAtkRate, float SoldierHpRate, int[] Skills, string Model, string HitEffect)
        {
            this.Id = Id;
            this.Name = Name;
            this.Lv = Lv;
            this.Atk = Atk;
            this.Hp = Hp;
            this.MoveSpeed = MoveSpeed;
            this.Range = Range;
            this.MissileSpeed = MissileSpeed;
            this.IsShadow = IsShadow;
            this.SoldierAtkRate = SoldierAtkRate;
            this.SoldierHpRate = SoldierHpRate;
            this.Skills = Skills;
            this.Model = Model;
            this.HitEffect = HitEffect;

        }

        public SoldierConfig() { }

        private static Dictionary<int, SoldierConfig> config = new Dictionary<int, SoldierConfig>();
        public static Dictionary<int, SoldierConfig>.ValueCollection ConfigList
        {
            get
            {
                return config.Values;
            }
        }

        public static void Refresh(Dictionary<int, SoldierConfig> dict)
        {
            config.Clear();
            config = dict;
        }

        public static void Load()
        {
            config.Clear();
            config[500001] = new SoldierConfig(500001, "小兵", 1, 24, 130, 10, 12, 0, false, 1f, 1f, null, "UnitBing", "SwordHitBlue");
            config[500002] = new SoldierConfig(500002, "远程小兵", 1, 17, 90, 7, 35, 15, false, .8f, .65f, null, "UnitBing2", "BulletExplosionFire");
            config[501001] = new SoldierConfig(501001, "法术场", 1, 0, 9999, 0, 0, 0, true, 0, 0, null, "UnitSpell", "");
            config[501002] = new SoldierConfig(501002, "关羽影子", 1, 2, 2, 10, 17, 0, false, 0, 0, null, "UnitHero", "SwordHitYellowCritical");

        }

        public static SoldierConfig GetConfig(int id)
        {
            SoldierConfig data;
            if (config.TryGetValue(id, out data))
            {
                return data;
            }
            throw new NullReferenceException(string.Format("配置表SoldierConfig不存在id={0}", id));
        }

        public static bool HasConfig(int id)
        {
            if (config.ContainsKey(id))
            {
                return true;
            }
            return false;
        }

        public static void Assign(int id, SoldierConfig configData)
        {
            config[id] = configData; 
        }

        public static void Add(int id, SoldierConfig configData)
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
