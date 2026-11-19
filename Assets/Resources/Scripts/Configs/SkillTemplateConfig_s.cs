﻿﻿﻿using System;
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
            {"Name", new FieldMetaInfo("模板名", "string", 165, "", true)},
            {"Script", new FieldMetaInfo("脚本名(唯一绑定的技能类名)", "string", 153)},
            {"Des", new FieldMetaInfo("说明", "string", 341)},
            {"Modules", new FieldMetaInfo("默认组件名列表(逗号分隔)", "string", 300)},
            {"Effect", new FieldMetaInfo("效果k-v(分号多绑定: 列[:子键]=Lv1~5数值；如 DamageStrength=70,105,175,280,420、Buff:缓:slow=0.1,0.12,0.15,0.2,0.25;BuffTime=3、TargetType:lowestHp、None)", "string", 565)},
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
        ///脚本名（唯一绑定的技能类名，如 SkillComDamageSingle；空=不套模板）
        /// </summary>
        public string Script;
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


        public SkillTemplateConfig(int Id, string Name, string Script, string Des, string Modules, string Effect, int Quality, float Score, float Range, float Area, int TargetCount, int MpCost, float CD)
        {
            this.Id = Id;
            this.Name = Name;
            this.Script = Script;
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
            config[1000] = new SkillTemplateConfig(1000, "单体伤害大", "SkillComDamageSingle", "对一个敌方单位结算一次高额伤害，标准输出基准。", "", "SelectOnly:enemy;DamageStrength=100,150,250,400,600", 3, 10f, 40f, 0f, 1, 7, 2f);
            config[1001] = new SkillTemplateConfig(1001, "单体伤害中", "SkillComDamageSingle", "对一个敌方单位结算一次中等伤害，数值降档，评分按同比例下调。", "", "SelectOnly:enemy;DamageStrength=80,120,200,320,480", 2, 8f, 40f, 0f, 1, 7, 2f);
            config[1002] = new SkillTemplateConfig(1002, "单体伤害小", "SkillComDamageSingle", "对一个敌方单位结算一次低额伤害，适合高频或附带其他效果的单体技能。", "", "SelectOnly:enemy;DamageStrength=60,90,150,240,360", 1, 6f, 40f, 0f, 1, 7, 1.5f);
            config[1010] = new SkillTemplateConfig(1010, "群体伤害近距离大", "SkillComDamageArea", "在近距离目标点（Range=40）对默认小范围（Area=20）敌人结算伤害，群体输出大档基准。", "", "SelectOnly:enemy;DamageStrength=70,105,175,280,420", 3, 10f, 40f, 20f, 3, 10, 2f);
            config[1011] = new SkillTemplateConfig(1011, "群体伤害近距离中", "SkillComDamageArea", "同近距离群体伤害，数值为大档的80%，标准群体输出中档。", "", "SelectOnly:enemy;DamageStrength=56,84,140,224,336", 2, 8f, 40f, 20f, 3, 10, 2f);
            config[1012] = new SkillTemplateConfig(1012, "群体伤害近距离小", "SkillComDamageArea", "同近距离群体伤害，数值为大档的60%，适合高频或附带其他效果。", "", "SelectOnly:enemy;DamageStrength=42,63,105,168,252", 1, 6f, 40f, 20f, 3, 10, 2f);
            config[1020] = new SkillTemplateConfig(1020, "群体伤害自身中心大", "SkillComDamageArea", "以施法者自身为中心（Range=0）对默认小范围（Area=20）敌人结算伤害，省射程但需贴身，大档基准。", "", "SelectOnly:enemy;DamageStrength=70,105,175,280,420", 3, 10f, 0f, 20f, 3, 10, 2f);
            config[1021] = new SkillTemplateConfig(1021, "群体伤害自身中心中", "SkillComDamageArea", "同自身中心群体伤害，数值为大档的80%。", "", "SelectOnly:enemy;DamageStrength=56,84,140,224,336", 2, 8f, 0f, 20f, 3, 10, 2f);
            config[1022] = new SkillTemplateConfig(1022, "群体伤害自身中心小", "SkillComDamageArea", "同自身中心群体伤害，数值为大档的60%。", "", "SelectOnly:enemy;DamageStrength=42,63,105,168,252", 1, 6f, 0f, 20f, 3, 10, 2f);
            config[1030] = new SkillTemplateConfig(1030, "跳跃弹射-伤害大", "SkillComDamageJumpChain", "在多个敌人之间跳跃结算伤害，主数值为常规群体伤害的1.1倍，每跳按 Strength2[jumpDecay]=0.85 递减；价值由跳数、单跳数值与搜索半径决定，大档基准。", "", "SelectOnly:enemy;DamageStrength=77,116,193,308,462;Strength2:jumpDecay=0.85", 4, 10f, 30f, 40f, 3, 10, 3f);
            config[1031] = new SkillTemplateConfig(1031, "跳跃弹射-伤害中", "SkillComDamageJumpChain", "同跳跃弹射伤害，数值为大档的80%，每跳衰减 0.85。", "", "SelectOnly:enemy;DamageStrength=62,92,154,246,370;Strength2:jumpDecay=0.85", 3, 8f, 30f, 40f, 3, 10, 3f);
            config[1032] = new SkillTemplateConfig(1032, "跳跃弹射-伤害小", "SkillComDamageJumpChain", "同跳跃弹射伤害，数值为大档的60%，每跳衰减 0.85。", "", "SelectOnly:enemy;DamageStrength=46,69,116,185,277;Strength2:jumpDecay=0.85", 2, 6f, 30f, 40f, 3, 10, 3f);
            config[1040] = new SkillTemplateConfig(1040, "冲击波伤害大", "SkillComDamageShockWave", "朝目标方向射出冲击波导弹，命中路径上最多 TargetCount 名敌人并逐个结算伤害；可选手配 BuffId（减速等）与 Strength2 pushMin/pushMax（击退），大档基准。", "", "SelectOnly:enemy;DamageStrength=60,85,130,220,330", 3, 10f, 40f, 15f, 3, 9, 2f);
            config[1041] = new SkillTemplateConfig(1041, "冲击波伤害中", "SkillComDamageShockWave", "同冲击波伤害，数值为大档的80%。", "", "SelectOnly:enemy;DamageStrength=48,68,104,176,264", 2, 8f, 40f, 15f, 3, 9, 2f);
            config[1042] = new SkillTemplateConfig(1042, "冲击波伤害小", "SkillComDamageShockWave", "同冲击波伤害，数值为大档的60%。", "", "SelectOnly:enemy;DamageStrength=36,51,78,132,198", 1, 6f, 40f, 15f, 3, 9, 2f);
            config[1050] = new SkillTemplateConfig(1050, "持续伤害大", "SkillComDamageHitRegion", "在落点投放持续存在的伤害场，单次伤害为同档群体伤害的30%（取整）；召唤物标签、范围仍由技能行配置，大档基准。", "", "SummonTime=5.2;SummonHitInterval=1;DamageStrength=21,32,53,84,126", 3, 9f, 40f, 0f, 3, 10, 2.5f);
            config[1051] = new SkillTemplateConfig(1051, "持续伤害中", "SkillComDamageHitRegion", "同持续伤害场，单次伤害为同档群体伤害的30%（取整）。", "", "SummonTime=5.2;SummonHitInterval=1;DamageStrength=17,25,42,67,101", 2, 7f, 40f, 0f, 3, 10, 2.5f);
            config[1052] = new SkillTemplateConfig(1052, "持续伤害小", "SkillComDamageHitRegion", "同持续伤害场，单次伤害为同档群体伤害的30%（取整）。", "", "SummonTime=5.2;SummonHitInterval=1;DamageStrength=13,19,32,50,76", 1, 5.5f, 40f, 0f, 3, 10, 2.5f);
            config[2000] = new SkillTemplateConfig(2000, "单体治疗大", "SkillComHealSingle", "选择一个友方单位进行治疗，治疗量为同类型伤害的125%，大档基准。", "", "SelectOnly:ally;HealStrength=125,188,313,500,750", 3, 10f, 40f, 0f, 1, 7, 2f);
            config[2001] = new SkillTemplateConfig(2001, "单体治疗中", "SkillComHealSingle", "同单体治疗，数值为大档的80%。", "", "SelectOnly:ally;HealStrength=100,150,250,400,600", 2, 8f, 40f, 0f, 1, 7, 2f);
            config[2002] = new SkillTemplateConfig(2002, "单体治疗小", "SkillComHealSingle", "同单体治疗，数值为大档的60%。", "", "SelectOnly:ally;HealStrength=75,113,188,300,450", 1, 6f, 40f, 0f, 1, 7, 2f);
            config[2010] = new SkillTemplateConfig(2010, "范围治疗大", "SkillComHealArea", "对默认小范围（Area=20）内友方单位施加治疗，治疗量为同类型伤害的125%，覆盖人数与溢出效果共同影响价值，大档基准。", "", "SelectOnly:ally;HealStrength=88,131,219,350,525", 4, 10f, 40f, 20f, 3, 9, 3f);
            config[2011] = new SkillTemplateConfig(2011, "范围治疗中", "SkillComHealArea", "同范围治疗，数值为大档的80%。", "", "SelectOnly:ally;HealStrength=70,105,175,280,420", 3, 8f, 40f, 20f, 3, 9, 3f);
            config[2012] = new SkillTemplateConfig(2012, "范围治疗小", "SkillComHealArea", "同范围治疗，数值为大档的60%。", "", "SelectOnly:ally;HealStrength=53,79,131,210,315", 2, 6f, 40f, 20f, 3, 9, 3f);
            config[2020] = new SkillTemplateConfig(2020, "跳跃弹射-治疗大", "SkillComHealJumpChain", "在多个友军之间跳跃结算治疗，治疗量为同类型伤害的125%，主数值为常规群体治疗的1.1倍，每跳按 Strength2[jumpDecay]=0.85 递减，大档基准。", "", "SelectOnly:ally;HealStrength=96,145,241,385,578;Strength2:jumpDecay=0.85", 4, 10f, 30f, 40f, 3, 10, 3f);
            config[2021] = new SkillTemplateConfig(2021, "跳跃弹射-治疗中", "SkillComHealJumpChain", "同跳跃弹射治疗，数值为大档的80%，每跳衰减 0.85。", "", "SelectOnly:ally;HealStrength=78,115,193,308,463;Strength2:jumpDecay=0.85", 3, 8f, 30f, 40f, 3, 10, 3f);
            config[2022] = new SkillTemplateConfig(2022, "跳跃弹射-治疗小", "SkillComHealJumpChain", "同跳跃弹射治疗，数值为大档的60%，每跳衰减 0.85。", "", "SelectOnly:ally;HealStrength=58,86,145,231,346;Strength2:jumpDecay=0.85", 2, 6f, 30f, 40f, 3, 10, 3f);
            config[2030] = new SkillTemplateConfig(2030, "持续治疗大", "SkillComHealAreaCasting", "在落点投放持续存在的治疗场，单次治疗量为同档范围治疗的30%（取整，即同档持续伤害的125%）；范围仍由技能行配置，大档基准。", "", "SummonTime=5;SummonHitInterval=1;HealStrength=26,39,66,105,158", 3, 9f, 40f, 0f, 3, 10, 2.5f);
            config[2031] = new SkillTemplateConfig(2031, "持续治疗中", "SkillComHealAreaCasting", "同持续治疗场，单次治疗量为同档范围治疗的30%（取整）。", "", "SummonTime=5;SummonHitInterval=1;HealStrength=21,32,53,84,126", 2, 7f, 40f, 0f, 3, 10, 2.5f);
            config[2032] = new SkillTemplateConfig(2032, "持续治疗小", "SkillComHealAreaCasting", "同持续治疗场，单次治疗量为同档范围治疗的30%（取整）。", "", "SummonTime=5;SummonHitInterval=1;HealStrength=16,24,39,63,95", 1, 5.5f, 40f, 0f, 3, 10, 2.5f);
            config[3000] = new SkillTemplateConfig(3000, "自身强化", "SkillComBuffSingle", "对施法者自身施加短时属性增益/护盾；模板只计形态分，实际价值由所挂 Buff 决定。", "", "None", 2, 1f, 0f, 0f, 1, 6, 3f);
            config[3010] = new SkillTemplateConfig(3010, "光环中", "SkillComBuffAura", "开启后每3秒对半径40内按 SelectOnly/SelectFirst 选中的单位挂Buff（可指向敌方），不限制目标数；无MP消耗常开，有MP消耗则MP满开启、每秒消耗1点MP；模板只计形态分，价值由 Buff 决定。", "", "SelectOnly:ally", 2, 5f, 0f, 40f, 0, 10, 2.5f);
            config[3020] = new SkillTemplateConfig(3020, "单体友方增益", "SkillComBuffArea", "对一个友方单位施加属性增益或护盾；模板只计形态分，价值由强度、持续时间与所挂 Buff 决定。", "", "SelectOnly:ally", 1, 2f, 40f, 0f, 1, 6, 3f);
            config[3030] = new SkillTemplateConfig(3030, "群体友方增益", "SkillComBuffArea", "以自身为中心，对 Area 内多名友方施加属性增益或护盾；模板只计形态分，价值由覆盖人数与所挂 Buff 决定。", "", "SelectOnly:ally", 2, 4f, 40f, 20f, 3, 8, 4f);
            config[3032] = new SkillTemplateConfig(3032, "群体友方增益-目标小", "SkillComBuffArea", "以选中的首个友军目标为中心，半径25内的友方获得属性增益或护盾；模板只计形态分，价值由所挂 Buff 决定。", "", "SelectOnly:ally;TargetType:targetCenter", 3, 4f, 40f, 20f, 3, 8, 4f);
            config[3040] = new SkillTemplateConfig(3040, "单体减益", "SkillComBuffArea", "对一名敌人施加减益/控制类 Buff（不造成伤害）；模板只计形态分，价值由所挂 Buff 决定。", "", "SelectOnly:enemy", 1, 2f, 40f, 0f, 1, 6, 3f);
            config[3041] = new SkillTemplateConfig(3041, "群体减益", "SkillComBuffArea", "以自身为中心，对 Area 内多名敌人施加减益/控制类 Buff（不造成伤害）；模板只计形态分，价值由覆盖人数与所挂 Buff 决定。", "", "SelectOnly:enemy", 2, 4f, 40f, 20f, 3, 8, 4f);
            config[3050] = new SkillTemplateConfig(3050, "群体嘲讽", "SkillComDamageTaunt", "以施法者自身为中心，对 Area 内敌人施加嘲讽（BuffTime 秒）并造成伤害；可选自身减伤盾（BuffId=硬）。", "", "SelectOnly:enemy;DamageStrength=40,60,90,150,240", 3, 5f, 0f, 20f, 3, 8, 2f);

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
