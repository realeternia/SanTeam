using System;
using System.Collections;
using System.Collections.Generic;

namespace CommonConfig
{
    public class BuffConfig
    {
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
            {"Id", new FieldMetaInfo("序列", "int", 60)},
            {"Name", new FieldMetaInfo("名字", "string", 0)},
            {"NameS", new FieldMetaInfo("短名", "string", 0, "", true)},
            {"ScriptName", new FieldMetaInfo("脚本名", "string", 135)},
            {"ColorStart", new FieldMetaInfo("启动色", "string", 0)},
            {"ColorEnd", new FieldMetaInfo("结束色", "string", 0)},
            {"IsPositive", new FieldMetaInfo("是否正面", "bool", 0)},
            {"BuffEffect", new FieldMetaInfo("hit", "string", 0)},
            {"Icon", new FieldMetaInfo("图标", "string", 0)},
        };

        public static Dictionary<string, FieldMetaInfo> FieldMeta { get { return fieldMeta; } }

        private static List<CellMeta> cellMeta = new List<CellMeta>();
        public static List<CellMeta> CellMetas { get { return cellMeta; } }

        /// <summary>
        ///序列
        /// </summary>
        public int Id;
        /// <summary>
        ///名字
        /// </summary>
        public string Name;
        /// <summary>
        ///短名
        /// </summary>
        public string NameS;
        /// <summary>
        ///脚本名
        /// </summary>
        public string ScriptName;
        /// <summary>
        ///启动色
        /// </summary>
        public string ColorStart;
        /// <summary>
        ///结束色
        /// </summary>
        public string ColorEnd;
        /// <summary>
        ///是否正面
        /// </summary>
        public bool IsPositive;
        /// <summary>
        ///hit
        /// </summary>
        public string BuffEffect;
        /// <summary>
        ///图标
        /// </summary>
        public string Icon;


        public BuffConfig(int Id, string Name, string NameS, string ScriptName, string ColorStart, string ColorEnd, bool IsPositive, string BuffEffect, string Icon)
        {
            this.Id = Id;
            this.Name = Name;
            this.NameS = NameS;
            this.ScriptName = ScriptName;
            this.ColorStart = ColorStart;
            this.ColorEnd = ColorEnd;
            this.IsPositive = IsPositive;
            this.BuffEffect = BuffEffect;
            this.Icon = Icon;
        }

        public BuffConfig() { }

        private static Dictionary<int, BuffConfig> config = new Dictionary<int, BuffConfig>();
        public static Dictionary<int, BuffConfig>.ValueCollection ConfigList
        {
            get { return config.Values; }
        }

        public static void Refresh(Dictionary<int, BuffConfig> dict)
        {
            config.Clear();
            config = dict;
            RebuildIndex();
        }

        public static void Load()
        {
            config.Clear();
            config[300001] = new BuffConfig(300001, "护盾", "盾", "BuffShield", "", "", true, "ShieldSoftBlue", "");
            config[300002] = new BuffConfig(300002, "减伤盾", "硬", "BuffShieldValue", "#B25900", "#FFD24D", true, "", "");
            config[300003] = new BuffConfig(300003, "吸血", "吸", "BuffSuck", "#FF0000", "#993333", true, "", "");
            config[300004] = new BuffConfig(300004, "伤害提升", "重", "BuffDamageAddRate", "", "", true, "SparkleAreaWhite", "");
            config[300005] = new BuffConfig(300005, "攻速提升", "快", "BuffCoolDown", "", "", true, "HeartStream", "");
            config[301001] = new BuffConfig(301001, "混乱", "乱", "BuffNoAction", "", "", false, "StunnedCirclingStarsSimple", "");
            config[301002] = new BuffConfig(301002, "连锁", "锁", "BuffLock", "", "", false, "StunnedLock", "");
            config[301003] = new BuffConfig(301003, "增伤", "伤", "BuffDamagedAddRate", "", "", false, "StunnedDamageUp", "");
            config[301004] = new BuffConfig(301004, "减速", "慢", "BuffSpeedDown", "", "", false, "SlowAuraYellow", "");
            config[301005] = new BuffConfig(301005, "陷阵", "停", "BuffNoMove", "", "", false, "AuraSoftPurple", "");
            config[301006] = new BuffConfig(301006, "溃败", "败", "BuffTimeDamage", "", "", false, "BloodExplosion", "");

            RebuildIndex();

        }

        private static void RebuildIndex()
        {
            idxNameS.Clear();
            foreach (var kv in config)
            {
                if (!string.IsNullOrEmpty(kv.Value.NameS)) idxNameS[kv.Value.NameS] = kv.Key;
            }
        }

        public static BuffConfig GetConfig(int id)
        {
            BuffConfig data;
            if (config.TryGetValue(id, out data))
            {
                return data;
            }
            throw new NullReferenceException(string.Format("配置表BuffConfig不存在id={0}", id));
        }

        private static Dictionary<string, int> idxNameS = new Dictionary<string, int>();
        public static BuffConfig GetConfigByNameS(string val)
        {
            return GetConfig(idxNameS[val]);
        }


        public static bool HasConfig(int id)
        {
            if (config.ContainsKey(id))
            {
                return true;
            }
            return false;
        }

        public static void Assign(int id, BuffConfig configData)
        {
            config[id] = configData; 
        }

        public static void Add(int id, BuffConfig configData)
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
