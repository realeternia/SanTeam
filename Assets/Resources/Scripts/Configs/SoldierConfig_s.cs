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
        ///技能
        /// </summary>
        public int[] Skills;
        /// <summary>
        ///hit
        /// </summary>
        public string HitEffect;


        public SoldierConfig(int Id, string Name, int Lv, int Atk, int Hp, int MoveSpeed, int Range, int MissileSpeed, int[] Skills, string HitEffect)
        {
            this.Id = Id;
            this.Name = Name;
            this.Lv = Lv;
            this.Atk = Atk;
            this.Hp = Hp;
            this.MoveSpeed = MoveSpeed;
            this.Range = Range;
            this.MissileSpeed = MissileSpeed;
            this.Skills = Skills;
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
            config[500001] = new SoldierConfig(500001, "小兵", 1, 24, 120, 10, 12, 0, null, "SwordHitBlue");
            config[500002] = new SoldierConfig(500002, "远程小兵", 1, 18, 90, 7, 40, 15, null, "BulletExplosionFire");

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
