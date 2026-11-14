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
            {"Des", new FieldMetaInfo("说明", "string", 341)},
            {"Modules", new FieldMetaInfo("默认组件名列表(逗号分隔)", "string", 300)},
            {"Effect", new FieldMetaInfo("效果k-v(分号多绑定: 列[:子键]=Lv1~5数值；如 DamageStrength=70,105,175,280,420、Buff:缓:slow=0.1,0.12,0.15,0.2,0.25;BuffTime=3、TargetType:lowestHp、None)", "string", 384)},
            {"Quality", new FieldMetaInfo("品质(1~4)", "int", 60, "qs,1:#666666,2:#33CC33,3:#3333FF,4:#FFD700")},
            {"Score", new FieldMetaInfo("标准态评分(分,标准态=10分)", "float", 90)},
            {"Range", new FieldMetaInfo("标准态射程(0=使用默认攻击距离)", "float", 70)},
            {"Area", new FieldMetaInfo("标准态范围半径(0=非范围技能,默认小范围=20)", "float", 80)},
            {"TargetCount", new FieldMetaInfo("标准态目标数(群体默认3,单体=1)", "int", 60)},
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
        ///标准态目标数(群体默认3,单体=1)
        /// </summary>
        public int TargetCount;
        /// <summary>
        ///标准态MP消耗
        /// </summary>
        public int MpCost;
        /// <summary>
        ///标准态冷却(秒)
        /// </summary>
        public float CD;


        public SkillTemplateConfig(int Id, string Name, string Des, string Modules, string Effect, int Quality, float Score, float Range, float Area, int TargetCount, int MpCost, float CD)
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
            this.TargetCount = TargetCount;
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
            config[1000] = new SkillTemplateConfig(1000, "单体伤害大", "对一个敌方单位结算一次高额伤害，标准输出基准。", "MP中增加,CD中增加", "SelectOnly:enemy;DamageStrength=100,150,250,400,600", 3, 10f, 40f, 0f, 1, 7, 2f);
            config[1001] = new SkillTemplateConfig(1001, "单体伤害中", "对一个敌方单位结算一次中等伤害，数值降档，评分按同比例下调。", "MP中增加,CD中增加", "SelectOnly:enemy;DamageStrength=80,120,200,320,480", 2, 8f, 40f, 0f, 1, 6, 2f);
            config[1002] = new SkillTemplateConfig(1002, "单体伤害小", "对一个敌方单位结算一次低额伤害，适合高频或附带其他效果的单体技能。", "MP中增加,CD中增加", "SelectOnly:enemy;DamageStrength=60,90,150,240,360", 1, 6f, 40f, 0f, 1, 5, 1.5f);
            config[1010] = new SkillTemplateConfig(1010, "群体伤害近距离", "在近距离目标点（Range=40）对默认小范围（Area=20）敌人结算伤害，标准群体输出基准。", "MP中增加,CD中增加", "SelectOnly:enemy;DamageStrength=70,105,175,280,420", 3, 10f, 40f, 20f, 3, 7, 2f);
            config[1011] = new SkillTemplateConfig(1011, "群体伤害自身中心", "以施法者自身为中心（Range=0）对默认小范围（Area=20）敌人结算伤害，省射程但需贴身。", "MP中增加,CD中增加", "SelectOnly:enemy;DamageStrength=70,105,175,280,420", 3, 10f, 0f, 20f, 3, 7, 2f);
            config[1020] = new SkillTemplateConfig(1020, "跳跃弹射-伤害", "在多个敌人之间跳跃结算伤害，每跳递减，价值由跳数、单跳数值与搜索半径决定。", "伤害倍率,MP中增加,CD中增加", "SelectOnly:enemy;DamageStrength=60,90,150,240,360", 4, 12f, 30f, 60f, 3, 8, 3f);
            config[1030] = new SkillTemplateConfig(1030, "冲击波伤害", "朝目标方向射出冲击波导弹，命中路径上最多 TargetCount 名敌人并逐个结算伤害；可选手配 BuffId（减速等）与 Strength2 pushMin/pushMax（击退）。", "伤害倍率,MP中增加,CD中增加", "SelectOnly:enemy;DamageStrength=60,85,130,220,330", 3, 10f, 40f, 15f, 3, 7, 2f);
            config[1040] = new SkillTemplateConfig(1040, "持续伤害", "在落点投放持续存在的伤害场，数字为场持续与结算间隔；召唤物标签、范围与单次伤害仍由技能行配置。", "", "SummonTime=5.2;SummonHitInterval=1", 3, 9f, 40f, 0f, 3, 9, 2.5f);
            config[2000] = new SkillTemplateConfig(2000, "单体治疗", "选择一个友方单位进行治疗，数值口径与单体伤害对称。", "属性增益中,MP中增加,CD中增加", "SelectOnly:ally;HealStrength=100,150,250,400,600", 3, 10f, 40f, 0f, 1, 7, 2f);
            config[2010] = new SkillTemplateConfig(2010, "范围治疗", "对默认小范围（Area=20）内友方单位施加治疗，覆盖人数与溢出效果共同影响价值。", "MP中增加,CD中增加", "SelectOnly:ally;HealStrength=70,105,175,280,420", 4, 11f, 40f, 20f, 3, 9, 3f);
            config[2020] = new SkillTemplateConfig(2020, "跳跃弹射-治疗", "在多个友军之间跳跃结算治疗，每跳递减，价值由跳数、单跳治疗量与搜索半径决定。", "MP中增加,CD中增加", "SelectOnly:ally;HealStrength=60,90,150,240,360", 4, 12f, 30f, 60f, 3, 8, 3f);
            config[2030] = new SkillTemplateConfig(2030, "持续治疗", "在落点投放持续存在的治疗场，数字为场持续与结算间隔；治疗量与范围仍由技能行配置。", "", "SummonTime=5;SummonHitInterval=1", 3, 9f, 40f, 0f, 3, 14, 2.5f);
            config[3000] = new SkillTemplateConfig(3000, "自身强化", "对施法者自身施加短时属性增益/护盾，主要看持续时间和成长性。", "属性增益中,护盾中,永久成长", "None", 2, 8f, 0f, 0f, 1, 6, 3f);
            config[3010] = new SkillTemplateConfig(3010, "光环小", "开启后每3秒对半径25内按 SelectOnly/SelectFirst 选中的单位挂Buff（可指向敌方）；无MP消耗常开，有MP消耗则MP满开启、每秒消耗1点MP。", "属性增益中", "SelectOnly:ally", 2, 3f, 0f, 25f, 3, 10, 2.5f);
            config[3020] = new SkillTemplateConfig(3020, "单体友方增益", "对一个友方单位施加属性增益或护盾，价值由强度、持续时间与目标选择共同决定。", "属性增益中,护盾中,低血筛选,MP中增加,CD中增加", "SelectOnly:ally", 3, 9f, 40f, 0f, 1, 6, 3f);
            config[3030] = new SkillTemplateConfig(3030, "群体友方增益", "对范围内多名友方施加属性增益或护盾，按覆盖人数与溢出价值估值。", "属性增益中,护盾中,低血筛选,MP中增加,CD中增加", "SelectOnly:ally", 4, 11f, 40f, 20f, 3, 8, 4f);
            config[3031] = new SkillTemplateConfig(3031, "群体友方增益-自身小", "以自身为中心，半径25内的友方获得属性增益或护盾。", "属性增益中,护盾中,低血筛选,MP中增加,CD中增加", "SelectOnly:ally", 3, 6f, 0f, 25f, 3, 8, 4f);
            config[3032] = new SkillTemplateConfig(3032, "群体友方增益-目标小", "以选中的首个友军目标为中心，半径25内的友方获得属性增益或护盾。", "属性增益中,护盾中,低血筛选,MP中增加,CD中增加", "SelectOnly:ally;TargetType:targetCenter", 3, 7f, 40f, 25f, 3, 8, 4f);
            config[3033] = new SkillTemplateConfig(3033, "全体友军增益", "全图范围内所有友方获得属性增益或护盾，不受距离限制。", "属性增益中,护盾中,MP中增加,CD中增加", "SelectOnly:ally;TargetType:wholeMap", 4, 12f, 0f, 0f, 3, 8, 4f);
            config[3040] = new SkillTemplateConfig(3040, "兵种增益", "对己方士兵施加属性增益，数值按士兵基础属性折算。", "加攻中,加护甲中,加魔抗中,MP中增加,CD中增加", "SelectOnly:ally", 3, 9f, 40f, 20f, 3, 7, 4f);

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
