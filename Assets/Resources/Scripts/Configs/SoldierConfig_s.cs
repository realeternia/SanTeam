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
        ///攻速（20=每秒攻击1次，40=每秒2次）
        /// </summary>
        public int AtkSpeed;
        /// <summary>
        ///护甲
        /// </summary>
        public int Armor;
        /// <summary>
        ///魔抗
        /// </summary>
        public int MagicRes;
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
        ///导弹高度
        /// </summary>
        public float MissileHight;
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


        public SoldierConfig(int Id, string Name, int Lv, int Atk, int Hp, int AtkSpeed, int Armor, int MagicRes, int MoveSpeed, int Range, int MissileSpeed, float MissileHight, bool IsShadow, float SoldierAtkRate, float SoldierHpRate, int[] Skills, string Model, string HitEffect)
        {
            this.Id = Id;
            this.Name = Name;
            this.Lv = Lv;
            this.Atk = Atk;
            this.Hp = Hp;
            this.AtkSpeed = AtkSpeed;
            this.Armor = Armor;
            this.MagicRes = MagicRes;
            this.MoveSpeed = MoveSpeed;
            this.Range = Range;
            this.MissileSpeed = MissileSpeed;
            this.MissileHight = MissileHight;
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
        public class FieldMetaInfo
        {
            public string fieldName;
            public string fieldType;
            public int fieldWidth;
            public string fieldRule;
            public bool fieldIndex;
            public FieldMetaInfo(string name, string type, int width = 0, string rule = "", bool index = false)
            {
                fieldName = name;
                fieldType = type;
                fieldWidth = width;
                fieldRule = rule;
                fieldIndex = index;
            }
        }

        public class CellMeta
        {
            public int row;
            public int col;
            public int? foreColor;
            public int? backColor;
            public CellMeta(int row, int col, int? foreColor, int? backColor)
            {
                this.row = row;
                this.col = col;
                this.foreColor = foreColor;
                this.backColor = backColor;
            }
        }

        private static Dictionary<string, FieldMetaInfo> fieldMeta = new Dictionary<string, FieldMetaInfo>()
        {
            {"Id", new FieldMetaInfo("序列", "int", 0)},
            {"Name", new FieldMetaInfo("名字", "string", 0)},
            {"Lv", new FieldMetaInfo("等级", "int", 0)},
            {"Atk", new FieldMetaInfo("攻击力", "int", 0)},
            {"Hp", new FieldMetaInfo("生命", "int", 0)},
            {"AtkSpeed", new FieldMetaInfo("攻速（30=每秒攻击1次，攻速20=1.5秒/次，15=2秒/次）", "int", 0)},
            {"Armor", new FieldMetaInfo("护甲", "int", 0)},
            {"MagicRes", new FieldMetaInfo("魔抗", "int", 0)},
            {"MoveSpeed", new FieldMetaInfo("移动速度", "int", 0)},
            {"Range", new FieldMetaInfo("攻击距离", "int", 0)},
            {"MissileSpeed", new FieldMetaInfo("导弹速度", "int", 0)},
            {"MissileHight", new FieldMetaInfo("导弹高度", "float", 0)},
            {"IsShadow", new FieldMetaInfo("是否隐藏", "bool", 0)},
            {"SoldierAtkRate", new FieldMetaInfo("士兵加成攻击系数", "float", 0)},
            {"SoldierHpRate", new FieldMetaInfo("士兵加成hp系数", "float", 0)},
            {"Skills", new FieldMetaInfo("技能", "int[]", 0)},
            {"Model", new FieldMetaInfo("模型", "string", 0)},
            {"HitEffect", new FieldMetaInfo("hit", "string", 0)},
        };

        public static Dictionary<string, FieldMetaInfo> FieldMeta { get { return fieldMeta; } }

        private static List<CellMeta> cellMeta = new List<CellMeta>();
        public static List<CellMeta> CellMetas { get { return cellMeta; } }

        public static void Load()
        {
            config.Clear();
            config[500001] = new SoldierConfig(500001, "小兵", 1, 24, 130, 15, 0, 0, 10, 12, 0, 0f, false, 1f, 1f, new int[0], "UnitBing", "SwordHitBlue");
            config[500002] = new SoldierConfig(500002, "远程小兵", 1, 17, 90, 15, 0, 0, 7, 35, 15, 1.5f, false, .8f, .65f, new int[0], "UnitBing2", "BulletExplosionFire");
            config[501001] = new SoldierConfig(501001, "法术场", 1, 0, 9999, 15, 0, 0, 0, 0, 0, 0f, true, 0f, 0f, new int[0], "UnitSpell", "");
            config[501002] = new SoldierConfig(501002, "关羽影子", 1, 2, 2, 15, 0, 0, 10, 17, 0, 0f, false, 0f, 0f, new int[0], "UnitHero", "SwordHitYellowCritical");

            RebuildIndex();

        }

        private static void RebuildIndex()
        {
            foreach (var kv in config)
            {
            }
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
