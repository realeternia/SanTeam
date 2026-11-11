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
            {"Quality", new FieldMetaInfo("品质(1~4)", "int", 60, "qs,1:#666666,2:#33CC33,3:#3333FF,4:#FFD700")},
            {"Score", new FieldMetaInfo("标准态评分(分,标准态=10分)", "float", 90)},
            {"Range", new FieldMetaInfo("射程(0=默认攻击距离;目标中心群体模板必填)", "float", 80)},
            {"Area", new FieldMetaInfo("范围半径(默认小范围=20;0=无范围)", "float", 80)},
            {"MpCost", new FieldMetaInfo("标准态MP消耗", "int", 80)},
            {"CD", new FieldMetaInfo("标准态冷却(秒)", "float", 80)},
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
        ///品质(1~4)
        /// </summary>
        public int Quality;
        /// <summary>
        ///标准态评分(分,标准态=10分)
        /// </summary>
        public float Score;
        /// <summary>
        ///射程(0=默认攻击距离;目标中心群体模板必填)
        /// </summary>
        public float Range;
        /// <summary>
        ///范围半径(默认小范围=20;0=无范围)
        /// </summary>
        public float Area;
        /// <summary>
        ///标准态MP消耗
        /// </summary>
        public int MpCost;
        /// <summary>
        ///标准态冷却(秒)
        /// </summary>
        public float CD;


        public SkillTemplateConfig(int Id, string Name, string Des, string Modules, string Values, int Quality, float Score, float Range, float Area, int MpCost, float CD)
        {
            this.Id = Id;
            this.Name = Name;
            this.Des = Des;
            this.Modules = Modules;
            this.Values = Values;
            this.Quality = Quality;
            this.Score = Score;
            this.Range = Range;
            this.Area = Area;
            this.MpCost = MpCost;
            this.CD = CD;
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
            RebuildIndex();
        }

        public static void Load()
        {
            config.Clear();
            config[10] = new SkillTemplateConfig(10, "单体伤害大", "对一个敌方单位结算一次高额伤害，标准输出基准（10分）。", "施法射程,MP消耗,CD", "100,150,250,400,600", 3, 10f, 0f, 0f, 7, 2f);
            config[11] = new SkillTemplateConfig(11, "单体伤害中", "对一个敌方单位结算一次中等伤害，数值降档，评分按同比例下调。", "施法射程,MP消耗,CD", "80,120,200,320,480", 2, 8f, 0f, 0f, 6, 2f);
            config[12] = new SkillTemplateConfig(12, "单体伤害小", "对一个敌方单位结算一次低额伤害，适合高频或附带其他效果的单体技能。", "施法射程,MP消耗,CD", "60,90,150,240,360", 1, 6f, 0f, 0f, 5, 1.5f);
            config[20] = new SkillTemplateConfig(20, "群体伤害近距离", "在近距离目标点（Range=40）对默认小范围（Area=20）敌人结算伤害，标准群体输出基准（10分）。", "最大目标数,施法射程,MP消耗,CD", "70,105,175,280,420", 3, 10f, 40f, 20f, 7, 2f);
            config[21] = new SkillTemplateConfig(21, "群体伤害自身中心", "以施法者自身为中心（Range=0）对默认小范围（Area=20）敌人结算伤害，省射程但需贴身。", "最大目标数,MP消耗,CD", "70,105,175,280,420", 3, 10f, 0f, 20f, 7, 2f);
            config[30] = new SkillTemplateConfig(30, "单体治疗", "选择一个友方单位进行治疗，数值口径与单体伤害对称。", "治疗量,属性增益,施法射程,MP消耗,CD", "100,150,250,400,600", 3, 10f, 0f, 0f, 7, 2f);
            config[31] = new SkillTemplateConfig(31, "范围治疗", "对默认小范围（Area=20）内友方单位施加治疗，覆盖人数与溢出效果共同影响价值。", "治疗量,最大目标数,施法射程,MP消耗,CD", "70,105,175,280,420", 4, 11f, 0f, 20f, 9, 3f);
            config[40] = new SkillTemplateConfig(40, "持续区域", "在目标点创建持续区域（Area=20），按周期结算伤害，需要射程列（Range=60）。", "持续区域,持续伤害,施法射程,MP消耗,CD", "40,60,100,160,240", 4, 11f, 60f, 20f, 9, 4f);
            config[50] = new SkillTemplateConfig(50, "自身强化", "对施法者自身施加短时属性增益/护盾，主要看持续时间和成长性。", "属性增益,护盾,永久成长", "20,30,50,80,120", 2, 8f, 0f, 0f, 6, 3f);
            config[60] = new SkillTemplateConfig(60, "光环", "对我方单位持续或全局生效的属性光环，按覆盖人数和持续时间估值。", "属性增益", "5,8,12,18,26", 2, 9f, 0f, 0f, 0, 0f);

            RebuildIndex();

        }

        private static void RebuildIndex()
        {
            idxId.Clear();
            foreach (var kv in config)
            {
                idxId[kv.Value.Id] = kv.Key;
            }
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

        private static Dictionary<int, int> idxId = new Dictionary<int, int>();
        public static SkillTemplateConfig GetConfigById(int val)
        {
            return GetConfig(idxId[val]);
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
