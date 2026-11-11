using System;
using System.Collections;
using System.Collections.Generic;

namespace CommonConfig
{
    public class SkillTemplateConfig
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
            {"Id", new FieldMetaInfo("模板Id", "int", 94, "", true)},
            {"Name", new FieldMetaInfo("模板名", "string", 0)},
            {"Des", new FieldMetaInfo("模板描述(基本语义与计价口径)", "string", 420)},
            {"Modules", new FieldMetaInfo("默认组件名列表(逗号分隔,对应技能模板组件表Name;技能Modules为空时使用)", "string", 240)},
            {"Values", new FieldMetaInfo("标准态主数值(Lv1~5逗号分隔;伤害模板填伤害值,治疗模板填治疗量)", "string", 150)},
            {"Range", new FieldMetaInfo("射程(0=默认攻击距离;远距离/近距离群体模板必填)", "float", 80)},
            {"MpCost", new FieldMetaInfo("标准态MP消耗", "int", 80)},
            {"CD", new FieldMetaInfo("标准态冷却(秒)", "float", 80)},
            {"Score", new FieldMetaInfo("标准态评分(分,标准态=10分)", "float", 90)},
        };

        public static Dictionary<string, FieldMetaInfo> FieldMeta { get { return fieldMeta; } }

        private static List<CellMeta> cellMeta = new List<CellMeta>();
        public static List<CellMeta> CellMetas { get { return cellMeta; } }

        /// <summary>
        ///模板Id
        /// </summary>
        public int Id;
        /// <summary>
        ///模板名
        /// </summary>
        public string Name;
        /// <summary>
        ///模板描述(基本语义与计价口径)
        /// </summary>
        public string Des;
        /// <summary>
        ///默认组件名列表(逗号分隔,对应技能模板组件表Name;技能Modules为空时使用)
        /// </summary>
        public string Modules;
        /// <summary>
        ///标准态主数值(Lv1~5逗号分隔;伤害模板填伤害值,治疗模板填治疗量)
        /// </summary>
        public string Values;
        /// <summary>
        ///射程(0=默认攻击距离;远距离/近距离群体模板必填)
        /// </summary>
        public float Range;
        /// <summary>
        ///标准态MP消耗
        /// </summary>
        public int MpCost;
        /// <summary>
        ///标准态冷却(秒)
        /// </summary>
        public float CD;
        /// <summary>
        ///标准态评分(分,标准态=10分)
        /// </summary>
        public float Score;


        public SkillTemplateConfig(int Id, string Name, string Des, string Modules, string Values, float Range, int MpCost, float CD, float Score)
        {
            this.Id = Id;
            this.Name = Name;
            this.Des = Des;
            this.Modules = Modules;
            this.Values = Values;
            this.Range = Range;
            this.MpCost = MpCost;
            this.CD = CD;
            this.Score = Score;
        }

        public SkillTemplateConfig() { }

        private static Dictionary<int, SkillTemplateConfig> config = new Dictionary<int, SkillTemplateConfig>();
        public static Dictionary<int, SkillTemplateConfig>.ValueCollection ConfigList
        {
            get { return config.Values; }
        }

        public static void Refresh(Dictionary<int, SkillTemplateConfig> dict)
        {
            config.Clear();
            config = dict;
        }

        public static void Load()
        {
            config.Clear();
            // 数据来源：docs/skill-template-plugin-analysis.md
            // 技能通过 SkillConfig.Template(模板名) 引用；SkillConfig.Modules(string[]) 覆盖默认组件名列表
            // Values=标准态主数值(Lv1~5，伤害档位即模板主数值，不再作为组件)，Range=射程(0=默认攻击距离)，MpCost/CD=标准态节奏，Score=标准态评分(10分基准)
            // 施放中心/射程/范围大小等变体由组件表达（施法射程、自身中心、小范围、中范围、大范围），模板只保留标准形态
            config[1] = new SkillTemplateConfig(1, "单体伤害大", "对一个敌方单位结算一次高额伤害，标准输出基准（10分）。", "施法射程,MP消耗,CD", "100,150,250,400,600", 0f, 7, 2f, 10f);
            config[2] = new SkillTemplateConfig(2, "单体伤害中", "对一个敌方单位结算一次中等伤害，数值降档，评分按同比例下调。", "施法射程,MP消耗,CD", "80,120,200,320,480", 0f, 6, 2f, 8f);
            config[3] = new SkillTemplateConfig(3, "单体伤害小", "对一个敌方单位结算一次低额伤害，适合高频或附带其他效果的单体技能。", "施法射程,MP消耗,CD", "60,90,150,240,360", 0f, 5, 1.5f, 6f);
            config[4] = new SkillTemplateConfig(4, "群体伤害近距离小范围", "在近距离目标点（Range=40）对小范围敌人结算伤害，标准群体输出基准（10分）。", "小范围,最大目标数,施法射程,MP消耗,CD", "70,105,175,280,420", 40f, 7, 2f, 10f);
            config[5] = new SkillTemplateConfig(5, "单体治疗", "选择一个友方单位进行治疗，数值口径与单体伤害对称。", "治疗量,属性增益,施法射程,MP消耗,CD", "100,150,250,400,600", 0f, 7, 2f, 10f);
            config[6] = new SkillTemplateConfig(6, "范围治疗", "对范围内友方单位施加治疗，覆盖人数与溢出效果共同影响价值。", "治疗量,小范围,最大目标数,施法射程,MP消耗,CD", "70,105,175,280,420", 0f, 9, 3f, 11f);
            config[7] = new SkillTemplateConfig(7, "持续区域", "在目标点创建持续区域，按周期结算伤害，需要射程列（Range=60）。", "持续区域,持续伤害,小范围,施法射程,MP消耗,CD", "40,60,100,160,240", 60f, 9, 4f, 11f);
            config[8] = new SkillTemplateConfig(8, "自身强化", "对施法者自身施加短时属性增益/护盾，主要看持续时间和成长性。", "属性增益,护盾,永久成长", "20,30,50,80,120", 0f, 6, 3f, 8f);
            config[9] = new SkillTemplateConfig(9, "光环", "对我方单位持续或全局生效的属性光环，按覆盖人数和持续时间估值。", "光环,属性增益", "5,8,12,18,26", 0f, 0, 0f, 9f);
        }

        public static SkillTemplateConfig GetConfig(int id)
        {
            SkillTemplateConfig data;
            if (config.TryGetValue(id, out data))
            {
                return data;
            }
            throw new NullReferenceException(string.Format("配置表SkillTemplateConfig不存在id={0}", id));
        }

        public static bool HasConfig(int id)
        {
            if (config.ContainsKey(id))
            {
                return true;
            }
            return false;
        }

        public static void Assign(int id, SkillTemplateConfig configData)
        {
            config[id] = configData; 
        }

        public static void Add(int id, SkillTemplateConfig configData)
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
