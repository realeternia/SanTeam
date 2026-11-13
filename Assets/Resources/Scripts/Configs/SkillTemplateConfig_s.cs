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
            {"Id", new FieldMetaInfo("序列", "int", 94)},
            {"Name", new FieldMetaInfo("模板名", "string", 0, "", true)},
            {"Des", new FieldMetaInfo("说明", "string", 300)},
            {"Modules", new FieldMetaInfo("默认组件名列表(逗号分隔)", "string", 300)},
            {"Effect", new FieldMetaInfo("效果k-v(分号多绑定: 列[:子键]=Lv1~5数值；如 DamageStrength=70,105,175,280,420、Buff:缓:slow=0.1,0.12,0.15,0.2,0.25;BuffTime=3、TargetType:lowestHp、None)", "string", 384)},
            {"Quality", new FieldMetaInfo("品质(1~4)", "int", 60, "qs,1:#666666,2:#33CC33,3:#3333FF,4:#FFD700")},
            {"Score", new FieldMetaInfo("标准态评分(分,标准态=10分)", "float", 90)},
            {"Range", new FieldMetaInfo("标准态射程(0=使用默认攻击距离)", "float", 70)},
            {"Area", new FieldMetaInfo("标准态范围半径(0=非范围技能,默认小范围=20)", "float", 80)},
            {"MpCost", new FieldMetaInfo("标准态MP消耗", "int", 80)},
            {"CD", new FieldMetaInfo("标准态冷却(秒)", "float", 80)},
        };

        public static Dictionary<string, FieldMetaInfo> FieldMeta { get { return fieldMeta; } }

        private static List<CellMeta> cellMeta = new List<CellMeta>();
        public static List<CellMeta> CellMetas { get { return cellMeta; } }

        /// <summary>
        ///序列
        /// </summary>
        public int Id;
        /// <summary>
        ///模板名
        /// </summary>
        public string Name;
        /// <summary>
        ///说明
        /// </summary>
        public string Des;
        /// <summary>
        ///默认组件名列表
        /// </summary>
        public string Modules;
        /// <summary>
        ///效果k-v（分号多绑定，列[:子键]=Lv1~5数值，None=不写列）
        /// </summary>
        public string Effect;
        /// <summary>
        ///品质(1~4)
        /// </summary>
        public int Quality;
        /// <summary>
        ///标准态评分(分,标准态=10分)
        /// </summary>
        public float Score;
        /// <summary>
        ///标准态射程
        /// </summary>
        public float Range;
        /// <summary>
        ///标准态范围半径
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


        public SkillTemplateConfig(int Id, string Name, string Des, string Modules, string Effect, int Quality, float Score, float Range, float Area, int MpCost, float CD)
        {
            this.Id = Id;
            this.Name = Name;
            this.Des = Des;
            this.Modules = Modules;
            this.Effect = Effect;
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
            config[10] = new SkillTemplateConfig(10, "单体伤害大", "对一个敌方单位结算一次高额伤害，标准输出基准。", "施法射程,MP中增加,CD中增加", "DamageStrength=100,150,250,400,600", 3, 10f, 0f, 0f, 7, 2f);
            config[11] = new SkillTemplateConfig(11, "单体伤害中", "对一个敌方单位结算一次中等伤害，数值降档，评分按同比例下调。", "施法射程,MP中增加,CD中增加", "DamageStrength=80,120,200,320,480", 2, 8f, 0f, 0f, 6, 2f);
            config[12] = new SkillTemplateConfig(12, "单体伤害小", "对一个敌方单位结算一次低额伤害，适合高频或附带其他效果的单体技能。", "施法射程,MP中增加,CD中增加", "DamageStrength=60,90,150,240,360", 1, 6f, 0f, 0f, 5, 1.5f);
            config[20] = new SkillTemplateConfig(20, "群体伤害近距离", "在近距离目标点（Range=40）对默认小范围（Area=20）敌人结算伤害，标准群体输出基准。", "最大目标数,施法射程,MP中增加,CD中增加", "DamageStrength=70,105,175,280,420", 3, 10f, 40f, 20f, 7, 2f);
            config[21] = new SkillTemplateConfig(21, "群体伤害自身中心", "以施法者自身为中心（Range=0）对默认小范围（Area=20）敌人结算伤害，省射程但需贴身。", "最大目标数,MP中增加,CD中增加", "DamageStrength=70,105,175,280,420", 3, 10f, 0f, 20f, 7, 2f);
            config[30] = new SkillTemplateConfig(30, "单体治疗", "选择一个友方单位进行治疗，数值口径与单体伤害对称。", "属性增益中,施法射程,MP中增加,CD中增加", "HealStrength=100,150,250,400,600", 3, 10f, 0f, 0f, 7, 2f);
            config[31] = new SkillTemplateConfig(31, "范围治疗", "对默认小范围（Area=20）内友方单位施加治疗，覆盖人数与溢出效果共同影响价值。", "最大目标数,施法射程,MP中增加,CD中增加", "HealStrength=70,105,175,280,420", 4, 11f, 0f, 20f, 9, 3f);
            config[50] = new SkillTemplateConfig(50, "自身强化", "对施法者自身施加短时属性增益/护盾，主要看持续时间和成长性。", "属性增益中,护盾中,永久成长", "None", 2, 8f, 0f, 0f, 6, 3f);
            config[60] = new SkillTemplateConfig(60, "光环", "对我方单位持续或全局生效的属性光环，按覆盖人数和持续时间估值；光环需主动释放（CD/MP）来叠加自身成长或刷新效果。", "属性增益中", "None", 9, 0f, 0f, 0f, 12, 3f);
            config[70] = new SkillTemplateConfig(70, "单体友方增益", "对一个友方单位施加属性增益或护盾，价值由强度、持续时间与目标选择共同决定。", "属性增益中,护盾中,低血筛选,施法射程,MP中增加,CD中增加", "None", 3, 9f, 50f, 0f, 6, 3f);
            config[80] = new SkillTemplateConfig(80, "群体友方增益", "对范围内多名友方施加属性增益或护盾，按覆盖人数与溢出价值估值。", "属性增益中,护盾中,最大目标数,低血筛选,施法射程,MP中增加,CD中增加", "None", 4, 11f, 0f, 20f, 8, 4f);
            config[90] = new SkillTemplateConfig(90, "兵种增益", "对己方士兵施加属性增益，数值按士兵基础属性折算。", "加攻中,加护甲中,加魔抗中,最大目标数,MP中增加,CD中增加", "None", 3, 9f, 0f, 20f, 7, 4f);
            config[110] = new SkillTemplateConfig(110, "跳跃弹射", "在多个目标之间跳跃结算伤害或治疗，每跳递减，价值由跳数、单跳数值与搜索半径决定。", "伤害倍率,最大目标数,施法射程,MP中增加,CD中增加", "DamageStrength=60,90,150,240,360", 4, 12f, 30f, 60f, 8, 3f);

            RebuildIndex();

        }

        private static void RebuildIndex()
        {
            idxName.Clear();
            foreach (var kv in config)
            {
                if (!string.IsNullOrEmpty(kv.Value.Name)) idxName[kv.Value.Name] = kv.Key;
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

        private static Dictionary<string, int> idxName = new Dictionary<string, int>();
        public static SkillTemplateConfig GetConfigByName(string val)
        {
            return GetConfig(idxName[val]);
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
