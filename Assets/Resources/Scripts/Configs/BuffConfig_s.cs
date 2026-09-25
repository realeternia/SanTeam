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
            {"Des", new FieldMetaInfo("描述", "string", 344)},
            {"IsPositive", new FieldMetaInfo("是否正面", "bool", 0)},
            {"ScriptName", new FieldMetaInfo("脚本名", "string", 171)},
            {"ColorStart", new FieldMetaInfo("启动色", "string", 0)},
            {"ColorEnd", new FieldMetaInfo("结束色", "string", 0)},
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
        ///描述
        /// </summary>
        public string Des;
        /// <summary>
        ///是否正面
        /// </summary>
        public bool IsPositive;
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
        ///hit
        /// </summary>
        public string BuffEffect;
        /// <summary>
        ///图标
        /// </summary>
        public string Icon;


        public BuffConfig(int Id, string Name, string NameS, string Des, bool IsPositive, string ScriptName, string ColorStart, string ColorEnd, string BuffEffect, string Icon)
        {
            this.Id = Id;
            this.Name = Name;
            this.NameS = NameS;
            this.Des = Des;
            this.IsPositive = IsPositive;
            this.ScriptName = ScriptName;
            this.ColorStart = ColorStart;
            this.ColorEnd = ColorEnd;
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
            config[300001] = new BuffConfig(300001, "护盾", "盾", "吸收受到的伤害，护盾值耗尽后消失（破盾标签的伤害可绕过）", true, "BuffShield", "", "", "ShieldSoftBlue", "");
            config[300002] = new BuffConfig(300002, "减伤盾", "硬", "受到攻击时按固定比例减免伤害", true, "BuffShieldValue", "#B25900", "#FFD24D", "", "");
            config[300003] = new BuffConfig(300003, "吸血", "吸", "攻击时按造成伤害的一定比例回复生命", true, "BuffSuck", "#FF0000", "#993333", "", "");
            config[300004] = new BuffConfig(300004, "伤害提升", "重", "造成的伤害按比例提升", true, "BuffDamageAddRate", "", "", "SparkleAreaWhite", "");
            config[300005] = new BuffConfig(300005, "攻速提升", "快", "攻击速度按比例提升", true, "BuffCoolDown", "", "", "HeartStream", "");
            config[300007] = new BuffConfig(300007, "急救", "愈", "每秒回复一定生命值", true, "BuffTimeHeal", "#00CC00", "#66FF66", "", "");
            config[300008] = new BuffConfig(300008, "攻击提升", "攻", "按数值提升自身攻击力", true, "BuffAtkAdd", "", "", "", "");
            config[300009] = new BuffConfig(300009, "汲血快攻", "汲", "攻击时按造成伤害吸血，并提升攻速", true, "BuffSuckHaste", "#FF0000", "#993333", "HeartStream", "");
            config[300010] = new BuffConfig(300010, "御风疾驰", "翼", "提升攻速与移动速度", true, "BuffHasteMoveSpeed", "", "", "HeartStream", "");
            config[301001] = new BuffConfig(301001, "混乱", "乱", "眩晕，无法行动", false, "BuffNoAction", "", "", "StunnedCirclingStarsSimple", "");
            config[301002] = new BuffConfig(301002, "连锁", "锁", "被攻击时，向范围内同样带连锁的友军传递同比例伤害", false, "BuffLock", "", "", "StunnedLock", "");
            config[301003] = new BuffConfig(301003, "增伤", "伤", "受到的伤害按比例增加", false, "BuffDamagedAddRate", "", "", "StunnedDamageUp", "");
            config[301004] = new BuffConfig(301004, "减速", "慢", "移动速度与攻击速度降低", false, "BuffSpeedDown", "", "", "SlowAuraYellow", "");
            config[301005] = new BuffConfig(301005, "陷阵", "停", "无法移动", false, "BuffNoMove", "", "", "AuraSoftPurple", "");
            config[301006] = new BuffConfig(301006, "溃败", "败", "持续受到随时间结算的伤害", false, "BuffTimeDamage", "", "", "BloodExplosion", "");

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
