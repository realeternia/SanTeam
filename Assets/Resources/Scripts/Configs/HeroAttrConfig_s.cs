using System;
using System.Collections.Generic;

namespace CommonConfig
{
    public class HeroAttrConfig
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
            {"Id", new FieldMetaInfo("序列", "int", 0)},
            {"name", new FieldMetaInfo("名字", "string", 0, "", true)},
            {"Cname", new FieldMetaInfo("中文名", "string", 0)},
            {"TextRule", new FieldMetaInfo("输出规则", "string", 0)},
            {"ColorRule", new FieldMetaInfo("颜色规则", "string", 0)},
            {"Icon", new FieldMetaInfo("icon", "string", 0)},
            {"IsArmsAttr", new FieldMetaInfo("是否兵种属性", "bool", 0)},
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
        public string name;
        /// <summary>
        ///中文名
        /// </summary>
        public string Cname;
        /// <summary>
        ///输出规则
        /// </summary>
        public string TextRule;
        /// <summary>
        ///颜色规则
        /// </summary>
        public string ColorRule;
        /// <summary>
        ///icon（对应 Resources/Textures/Icons 下的文件）
        /// </summary>
        public string Icon;
        /// <summary>
        ///是否兵种属性
        /// </summary>
        public bool IsArmsAttr;


        public HeroAttrConfig(int Id, string name, string Cname, string TextRule, string ColorRule, string Icon, bool IsArmsAttr)
        {
            this.Id = Id;
            this.name = name;
            this.Cname = Cname;
            this.TextRule = TextRule;
            this.ColorRule = ColorRule;
            this.Icon = Icon;
            this.IsArmsAttr = IsArmsAttr;
        }

        public HeroAttrConfig() { }

        private static Dictionary<int, HeroAttrConfig> config = new Dictionary<int, HeroAttrConfig>();
        public static Dictionary<int, HeroAttrConfig>.ValueCollection ConfigList
        {
            get { return config.Values; }
        }

        public static void Refresh(Dictionary<int, HeroAttrConfig> dict)
        {
            config.Clear();
            config = dict;
            RebuildIndex();
        }

        public static void Load()
        {
            config.Clear();
            config[1] = new HeroAttrConfig(1, "atk", "攻击", "", "", "attratk", false);
            config[2] = new HeroAttrConfig(2, "ap", "法术", "", "", "attrap", false);
            config[3] = new HeroAttrConfig(3, "might", "无双", "", "", "attrmight", false);
            config[4] = new HeroAttrConfig(4, "hp", "生命", "", "", "attrhp", false);
            config[5] = new HeroAttrConfig(5, "atkspeed", "攻速", "", "", "attackspeed", false);
            config[6] = new HeroAttrConfig(6, "armor", "护甲", "", "", "attrarmor", false);
            config[7] = new HeroAttrConfig(7, "magicres", "魔抗", "", "", "attrmagicshield", false);
            config[8] = new HeroAttrConfig(8, "movespeed", "移速", "", "", "attrspeed", false);
            config[9] = new HeroAttrConfig(9, "range", "射程", "", "", "attrrange", false);
            config[10] = new HeroAttrConfig(10, "crit", "暴击", "", "", "attrcrit", false);
            config[11] = new HeroAttrConfig(11, "dodge", "闪避", "", "", "attrdodge", false);
            // JobLink 属性键（SkillConfig LinkSelf/LinkTeam 使用的键名，中文名供 JobLinkManager tooltip 查询）
            config[12] = new HeroAttrConfig(12, "maxHp", "生命", "", "", "attrhp", false);
            config[13] = new HeroAttrConfig(13, "critRate", "暴击", "", "", "attrcrit", false);
            config[14] = new HeroAttrConfig(14, "attackRate", "攻速", "", "", "attackspeed", false);
            config[15] = new HeroAttrConfig(15, "dodgeRate", "闪避", "", "", "attrdodge", false);
            config[16] = new HeroAttrConfig(16, "magicRes", "魔抗", "", "", "attrmagicshield", false);
            config[17] = new HeroAttrConfig(17, "soldierAtk", "士兵攻", "", "", "attratk", false);
            config[18] = new HeroAttrConfig(18, "soldierHp", "士兵生命", "", "", "attrhp", false);
            config[19] = new HeroAttrConfig(19, "critDamageMulti", "暴伤", "", "", "attrcrit", false);
            config[20] = new HeroAttrConfig(20, "mpRegen", "法力回复", "", "", "", false);
            config[21] = new HeroAttrConfig(21, "hpRegen", "生命回复", "", "", "attrhp", false);
            config[22] = new HeroAttrConfig(22, "healRate", "治疗强化", "", "", "", false);
            config[23] = new HeroAttrConfig(23, "healedRate", "受治疗", "", "", "", false);
            config[24] = new HeroAttrConfig(24, "buffEffectRate", "buff效果", "", "", "", false);
            config[25] = new HeroAttrConfig(25, "debuffDur", "负面持续", "", "", "", false);
            config[26] = new HeroAttrConfig(26, "auroEffectRate", "光环效果", "", "", "", false);

            RebuildIndex();

        }

        private static void RebuildIndex()
        {
            idxname.Clear();
            foreach (var kv in config)
            {
                if (!string.IsNullOrEmpty(kv.Value.name)) idxname[kv.Value.name] = kv.Key;
            }
        }

        public static HeroAttrConfig GetConfig(int id)
        {
            HeroAttrConfig data;
            if (config.TryGetValue(id, out data))
            {
                return data;
            }
            throw new NullReferenceException(string.Format("配置表HeroAttrConfig不存在id={0}", id));
        }

        private static Dictionary<string, int> idxname = new Dictionary<string, int>();
        public static HeroAttrConfig GetConfigByname(string val)
        {
            return GetConfig(idxname[val]);
        }


        public static bool HasConfig(int id)
        {
            if (config.ContainsKey(id))
            {
                return true;
            }
            return false;
        }

        public static void Assign(int id, HeroAttrConfig configData)
        {
            config[id] = configData; 
        }

        public static void Add(int id, HeroAttrConfig configData)
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
