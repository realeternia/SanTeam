﻿using System;
using System.Collections;
using System.Collections.Generic;

namespace CommonConfig
{
    public class SkillTemplateComponentConfig
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
            {"Name", new FieldMetaInfo("组件名", "string", 0, "", true)},
            {"Category", new FieldMetaInfo("类别(数值、射程、目标数、范围档、状态效果、持续、目标筛选、特殊机制、施放节奏)", "string", 0)},
            {"Templates", new FieldMetaInfo("适用大类列表(Damage伤害/Heal治疗/Buff增益；空=通用)", "string[]", 0)},
            {"Effect", new FieldMetaInfo("效果k-v(分号多绑定: 列[:子键]=Lv1~5数值；如 Area=30、Buff:缓:slow=0.1,0.12,0.15,0.2,0.25;BuffTime=3、TargetType:missile、SelectOnly:hero、SelectFirst:lowestHp、None)", "string", 511)},
            {"Quality", new FieldMetaInfo("品质(1~4)", "int", 60, "qs,1:#666666,2:#33CC33,3:#3333FF,4:#FFD700")},
            {"Score", new FieldMetaInfo("评分(分,标准态=10分)", "float", 90)},
            {"Des", new FieldMetaInfo("说明", "string", 300)},
            {"PriceNote", new FieldMetaInfo("估价要点", "string", 300)},
            {"IsSpecial", new FieldMetaInfo("是否特殊机制(需人工评审)", "bool", 0)},
        };

        public static Dictionary<string, FieldMetaInfo> FieldMeta { get { return fieldMeta; } }

        private static List<CellMeta> cellMeta = new List<CellMeta>();
        public static List<CellMeta> CellMetas { get { return cellMeta; } }

        /// <summary>
        ///序列
        /// </summary>
        public int Id;
        /// <summary>
        ///组件名
        /// </summary>
        public string Name;
        /// <summary>
        ///类别(数值、射程、目标数、范围档、状态效果、持续、目标筛选、特殊机制、施放节奏)
        /// </summary>
        public string Category;
        /// <summary>
        ///适用大类列表（Damage/Heal/Buff，空=通用）
        /// </summary>
        public string[] Templates;
        /// <summary>
        ///效果k-v（分号多绑定，列[:子键]=Lv1~5数值，None=不写列）
        /// </summary>
        public string Effect;
        /// <summary>
        ///品质(1~4)
        /// </summary>
        public int Quality;
        /// <summary>
        ///评分(分,标准态=10分)
        /// </summary>
        public float Score;
        /// <summary>
        ///说明
        /// </summary>
        public string Des;
        /// <summary>
        ///估价要点
        /// </summary>
        public string PriceNote;
        /// <summary>
        ///是否特殊机制(需人工评审)
        /// </summary>
        public bool IsSpecial;


        public SkillTemplateComponentConfig(int Id, string Name, string Category, string[] Templates, string Effect, int Quality, float Score, string Des, string PriceNote, bool IsSpecial)
        {
            this.Id = Id;
            this.Name = Name;
            this.Category = Category;
            this.Templates = Templates;
            this.Effect = Effect;
            this.Quality = Quality;
            this.Score = Score;
            this.Des = Des;
            this.PriceNote = PriceNote;
            this.IsSpecial = IsSpecial;
        }

        public SkillTemplateComponentConfig() { }

        private static Dictionary<int, SkillTemplateComponentConfig> config = new Dictionary<int, SkillTemplateComponentConfig>();
        public static Dictionary<int, SkillTemplateComponentConfig>.ValueCollection ConfigList
        {
            get { return config.Values; }
        }

        public static void Refresh(Dictionary<int, SkillTemplateComponentConfig> dict)
        {
            config.Clear();
            config = dict;
            RebuildIndex();
        }

        public static void Load()
        {
            config.Clear();
            config[30] = new SkillTemplateComponentConfig(30, "射程小", "射程", null, "Range+=20", 1, 1f, "射程 +20（模板默认 40 → 60）", "差值关系：在模板默认射程上叠加，按可触达目标与触发成功率估值", false);
            config[31] = new SkillTemplateComponentConfig(31, "射程中", "射程", null, "Range+=40", 1, 2f, "射程 +40（模板默认 40 → 80）", "差值关系：在模板默认射程上叠加，按可触达目标与触发成功率估值", false);
            config[32] = new SkillTemplateComponentConfig(32, "射程大", "射程", null, "Range+=70", 2, 3f, "射程 +70（模板默认 40 → 110）", "差值关系：在模板默认射程上叠加，按可触达目标与触发成功率估值", false);
            config[40] = new SkillTemplateComponentConfig(40, "目标增小", "目标数", null, "TargetCount+=2", 2, 1.5f, "目标数 +2（模板默认群体 3 → 5）", "差值关系：在模板默认目标数上叠加", false);
            config[41] = new SkillTemplateComponentConfig(41, "目标增大", "目标数", null, "TargetCount+=5", 3, 3f, "目标数 +5（模板默认群体 3 → 8）", "差值关系：在模板默认目标数上叠加", false);
            config[50] = new SkillTemplateComponentConfig(50, "小范围", "范围档", null, "Area+=15", 1, 1f, "半径 +15（模板默认小范围 20 → 35）", "差值关系：在模板默认半径上叠加，按实际预期命中数估值", false);
            config[51] = new SkillTemplateComponentConfig(51, "中范围", "范围档", null, "Area+=30", 1, 2f, "半径 +30（模板默认小范围 20 → 50）", "差值关系：在模板默认半径上叠加，按实际预期命中数估值", false);
            config[52] = new SkillTemplateComponentConfig(52, "大范围", "范围档", null, "Area+=50", 2, 4f, "半径 +50（模板默认小范围 20 → 70）", "差值关系：在模板默认半径上叠加，按实际预期命中数估值", false);
            config[91] = new SkillTemplateComponentConfig(91, "无衰减", "特殊机制", new string[]{"跳跃弹射-伤害大", "跳跃弹射-伤害中", "跳跃弹射-伤害小", "跳跃弹射-治疗大", "跳跃弹射-治疗中", "跳跃弹射-治疗小"}, "Strength2:jumpDecay=1", 2, 2f, "跳跃链每跳不再衰减（jumpDecay=1），总输出提升", "正向项：按跳数折算额外输出，避免与主数值重复计价", false);
            config[92] = new SkillTemplateComponentConfig(92, "衰减中", "特殊机制", new string[]{"跳跃弹射-伤害大", "跳跃弹射-伤害中", "跳跃弹射-伤害小", "跳跃弹射-治疗大", "跳跃弹射-治疗中", "跳跃弹射-治疗小"}, "Strength2:jumpDecay=0.7", 1, -1f, "跳跃链每跳衰减加重到0.7（默认0.85）", "负向项：按跳数折减总输出", false);
            config[93] = new SkillTemplateComponentConfig(93, "衰减大", "特殊机制", new string[]{"跳跃弹射-伤害大", "跳跃弹射-伤害中", "跳跃弹射-伤害小", "跳跃弹射-治疗大", "跳跃弹射-治疗中", "跳跃弹射-治疗小"}, "Strength2:jumpDecay=0.5", 1, -2f, "跳跃链每跳衰减加重到0.5（默认0.85）", "负向项：按跳数折减总输出", false);
            config[120] = new SkillTemplateComponentConfig(120, "MP小增加", "施放节奏", null, "MpCost+=2", 1, -0.5f, "技能 MP 消耗 +20（MpCost +2×10），发动频率略降", "负向项：节奏成本，按实际发动频率抵减技能总价值", false);
            config[121] = new SkillTemplateComponentConfig(121, "MP中增加", "施放节奏", null, "MpCost+=4", 1, -1f, "技能 MP 消耗 +40（MpCost +4×10），发动频率下降", "负向项：节奏成本，按实际发动频率抵减技能总价值", false);
            config[122] = new SkillTemplateComponentConfig(122, "MP大增加", "施放节奏", null, "MpCost+=6", 1, -1.5f, "技能 MP 消耗 +60（MpCost +6×10），发动频率明显下降", "负向项：节奏成本，按实际发动频率抵减技能总价值", false);
            config[123] = new SkillTemplateComponentConfig(123, "MP小节省", "施放节奏", null, "MpCost-=2", 1, 0.5f, "技能 MP 消耗 -20（MpCost -2×10），发动频率略升", "正向项：节奏收益，按实际发动频率提升技能总价值", false);
            config[124] = new SkillTemplateComponentConfig(124, "MP中节省", "施放节奏", null, "MpCost-=4", 1, 1f, "技能 MP 消耗 -40（MpCost -4×10），发动频率提升", "正向项：节奏收益，按实际发动频率提升技能总价值", false);
            config[125] = new SkillTemplateComponentConfig(125, "MP大节省", "施放节奏", null, "MpCost-=6", 1, 1.5f, "技能 MP 消耗 -60（MpCost -6×10），发动频率明显提升", "正向项：节奏收益，按实际发动频率提升技能总价值", false);
            config[140] = new SkillTemplateComponentConfig(140, "大概率", "施放节奏", null, "Rate=0.6", 1, -0.5f, "每次行动发动概率较高（Rate），百分比", "负向项：节奏成本，按实际发动频率抵减技能总价值", false);
            config[141] = new SkillTemplateComponentConfig(141, "中概率", "施放节奏", null, "Rate=0.4", 1, -1f, "每次行动发动概率中等（Rate），百分比", "负向项：节奏成本，按实际发动频率抵减技能总价值", false);
            config[142] = new SkillTemplateComponentConfig(142, "小概率", "施放节奏", null, "Rate=0.2", 1, -1.5f, "每次行动发动概率较低（Rate），百分比", "负向项：节奏成本，按实际发动频率抵减技能总价值", false);
            config[1002] = new SkillTemplateComponentConfig(1002, "减速", "状态效果", new string[]{"Damage", "Buff"}, "Buff:缓:slow=0.12,0.15,0.18,0.21,0.24;BuffTime=3;BuffEffect:slow", 1, 0.5f, "降低目标移动与攻速（缓）", "缓同时降低移速与攻速，按复合效果核算", false);
            config[1012] = new SkillTemplateComponentConfig(1012, "减防", "状态效果", new string[]{"Damage", "Buff"}, "Buff:破:armorDown=9,12,15,18,24;BuffTime=4;BuffEffect:armorDown", 1, 0.5f, "降低目标护甲（破）", "按效果强度、持续时间、命中对象数综合估值", false);
            config[1022] = new SkillTemplateComponentConfig(1022, "减攻", "状态效果", new string[]{"Damage", "Buff"}, "Buff:慑:atkDown=9,12,15,18,24;BuffTime=4;BuffEffect:atkDown", 1, 1f, "降低目标攻击（慑）", "按效果强度、持续时间、命中对象数综合估值", false);
            config[1032] = new SkillTemplateComponentConfig(1032, "减疗", "状态效果", new string[]{"Damage", "Buff"}, "Buff:疫;Strength2:healDown=0.18,0.21,0.24,0.3,0.36;BuffTime=4;BuffEffect:healDown", 1, 0.5f, "降低目标受到的治疗（疫）", "按效果强度、持续时间、命中对象数综合估值", false);
            config[1042] = new SkillTemplateComponentConfig(1042, "增伤", "状态效果", new string[]{"Damage", "Buff"}, "Buff:伤:damagedUp=0.06,0.09,0.12,0.15,0.18;BuffTime=4;BuffEffect:damagedUp", 1, 1f, "使目标受到的伤害提升（伤）", "按效果强度、持续时间、命中对象数综合估值", false);
            config[1052] = new SkillTemplateComponentConfig(1052, "眩晕", "状态效果", new string[]{"Damage", "Buff"}, "BuffTime=0.4,0.5,1,0.8,1;Buff:乱", 2, 2f, "无法行动（乱）", "控制按覆盖量计价：强度、时长、命中数、施放频率", false);
            config[1062] = new SkillTemplateComponentConfig(1062, "定身", "状态效果", new string[]{"Damage", "Buff"}, "BuffTime=1,0.8,1,1,1.5;Buff:停", 1, 1f, "无法移动（停）", "控制按覆盖量计价：强度、时长、命中数、施放频率", false);
            config[1072] = new SkillTemplateComponentConfig(1072, "混乱", "状态效果", new string[]{"Damage", "Buff"}, "BuffTime=0.5,1,0.7,0.9,1.1;Buff:惑", 2, 1.5f, "不受控制，攻击最近的友方单位（惑）", "控制按覆盖量计价：强度、时长、命中数、施放频率", false);
            config[1090] = new SkillTemplateComponentConfig(1090, "自身易伤", "状态效果", new string[]{"Buff"}, "Buff:伤:damagedUp=0.2;BuffTime=5;BuffEffect:damagedUp", 2, -1.5f, "自身受到的伤害提升（伤），属于代价项", "负向项，抵减技能总价值", false);
            config[1100] = new SkillTemplateComponentConfig(1100, "中毒", "状态效果", new string[]{"Damage", "Buff"}, "Buff:毒:dot=12,16,22,30,40;BuffTime=4;BuffEffect:dot", 1, 0.5f, "每秒造成固定跳伤（毒），不随属性成长", "跳伤按持续时间折算总量估值", false);
            config[1110] = new SkillTemplateComponentConfig(1110, "叛逃", "状态效果", new string[]{"Damage", "Buff"}, "Buff:叛;BuffTime=2", 4, 4f, "命中后目标短时脱离战斗（叛）", "按控制覆盖量计价：时长、命中数、施放频率", true);
            config[1120] = new SkillTemplateComponentConfig(1120, "连锁扩散", "状态效果", new string[]{"单体减益", "群体减益"}, "Buff:锁:chain=0.4;BuffTime=8;BuffEffect:chain", 3, 3f, "命中后向邻近单位扩散效果（锁）", "按传递伤害的比例与覆盖人数估值", true);
            config[2012] = new SkillTemplateComponentConfig(2012, "加攻", "状态效果", null, "Buff:攻:atk=6,9,15,24,36;BuffEffect:atk;BuffTime=5", 1, 1f, "提升攻击力", "按属性类型、强度、持续时间、覆盖人数综合估值", false);
            config[2022] = new SkillTemplateComponentConfig(2022, "加护甲", "状态效果", null, "Buff:甲:armor=3,5,7,11,16;BuffEffect:armor;BuffTime=5", 1, 0.5f, "提升护甲", "按属性类型、强度、持续时间、覆盖人数综合估值", false);
            config[2032] = new SkillTemplateComponentConfig(2032, "加法强", "状态效果", null, "Buff:术:ap=6,9,15,24,36;BuffEffect:ap;BuffTime=5", 1, 1f, "提升法术强度", "按属性类型、强度、持续时间、覆盖人数综合估值", false);
            config[2042] = new SkillTemplateComponentConfig(2042, "加魔抗", "状态效果", null, "Buff:抗:magicRes=3,5,7,11,16;BuffEffect:magicRes;BuffTime=5", 1, 0.5f, "提升魔法抗性", "按属性类型、强度、持续时间、覆盖人数综合估值", false);
            config[2052] = new SkillTemplateComponentConfig(2052, "加攻速", "状态效果", null, "Buff:快:atkSpeed=0.03,0.05,0.07,0.11,0.16;BuffEffect:atkSpeed;BuffTime=5", 1, 0.5f, "提升攻击速度", "按属性类型、强度、持续时间、覆盖人数综合估值", false);
            config[2062] = new SkillTemplateComponentConfig(2062, "加移速", "状态效果", null, "Buff:速:moveSpeed=0.03,0.05,0.07,0.11,0.16;BuffEffect:moveSpeed;BuffTime=5", 1, 0.5f, "提升移动速度", "按属性类型、强度、持续时间、覆盖人数综合估值", false);
            config[2072] = new SkillTemplateComponentConfig(2072, "加攻速率", "状态效果", null, "Buff:快:atkSpeed=0.03,0.05,0.07,0.11,0.16;BuffEffect:atkSpeed;BuffTime=5", 1, 0.5f, "按百分比提升攻击速度", "与点数型加攻速区分，按实际出手次数提升估值", false);
            config[2082] = new SkillTemplateComponentConfig(2082, "加生命回复", "状态效果", null, "Buff:血:hpRegen=3,5,7,11,16;BuffEffect:hpRegen;BuffTime=5", 1, 0.5f, "提升每秒生命回复", "按战斗时长折算总量后估值", false);
            config[2092] = new SkillTemplateComponentConfig(2092, "自身增伤", "状态效果", null, "Buff:重:damageUp=0.06,0.09,0.12,0.15,0.18;BuffEffect:damageUp;BuffTime=5", 1, 1f, "提升自身造成的伤害（重）", "与使目标受伤提升区分，按持续时间内输出增量估值", false);
            config[2102] = new SkillTemplateComponentConfig(2102, "护盾", "状态效果", null, "Buff:盾;Strength2:shield=36,54,90,144,216;BuffEffect:shield;BuffTime=5", 2, 2f, "吸收伤害的护盾（盾）", "护盾与治疗量分别估值", false);
            config[2112] = new SkillTemplateComponentConfig(2112, "减伤", "状态效果", null, "Buff:硬:shieldReduce=0.06,0.09,0.12,0.15,0.18;BuffEffect:shieldReduce;BuffTime=5", 2, 2f, "按百分比降低受到的伤害（硬）", "按持续时间内吸收的伤害量估值", false);
            config[2122] = new SkillTemplateComponentConfig(2122, "暴击", "状态效果", null, "Buff:暴:critRate=0.03,0.05,0.07,0.11,0.16;BuffEffect:critRate;BuffTime=5", 1, 1f, "提升暴击率（暴）", "按期望输出增量估值", false);
            config[2132] = new SkillTemplateComponentConfig(2132, "持续伤害", "状态效果", new string[]{"Damage", "Buff"}, "Buff:败:dot=18,25,35,48,62;BuffEffect:dot;BuffTime=5", 1, 0.5f, "每秒造成固定跳伤（败）", "跳伤不随属性成长，按持续时间折算总量估值", false);
            config[2142] = new SkillTemplateComponentConfig(2142, "吸血", "状态效果", null, "Buff:吸:suck=0.15,0.2,0.25,0.3,0.35;BuffEffect:suck;BuffTime=5", 1, 1f, "按造成伤害比例回复自身生命（吸）", "按持续时间内实际输出量与吸血比例折算", false);
            config[2152] = new SkillTemplateComponentConfig(2152, "闪避", "状态效果", null, "Buff:闪:evade=0.12,0.15,0.18,0.22,0.26;BuffEffect:evade;BuffTime=5", 2, 1.5f, "按概率完全闪避伤害（闪）", "按期望免伤折算等效治疗量", false);
            config[2160] = new SkillTemplateComponentConfig(2160, "急救", "状态效果", null, "Buff:愈:heal=25,38,52,70,90;BuffEffect:heal;BuffTime=5", 2, 1.5f, "每秒固定回复生命（愈）", "按持续时间折算总回复量估值", false);
            config[2170] = new SkillTemplateComponentConfig(2170, "伤害反射", "状态效果", new string[]{"Buff"}, "Buff:反:reflect=0.12,0.16,0.2,0.25,0.3;BuffEffect:reflect;BuffTime=5", 3, 3f, "给自己挂反伤 buff（reflect）", "按承受伤害与反弹比例折算", false);
            config[2180] = new SkillTemplateComponentConfig(2180, "生命链接", "状态效果", new string[]{"Heal"}, "Buff:链:damageShare=0.25;BuffEffect:damageShare;BuffTime=5", 3, 3f, "把目标受到的伤害按比例分摊给链接单位（链）", "按链接单位数与分摊比例折算等效减伤", true);
            config[2190] = new SkillTemplateComponentConfig(2190, "受击叠防", "状态效果", new string[]{"Buff"}, "Buff:守:armor=15,25,35,45,55;Buff:守:perStack=0.15,0.18,0.2,0.22,0.25;BuffEffect:armor;Strength2:maxStack=4,4,5,5,6;BuffTime=5", 3, 3f, "状态期间每次受击叠加双防（armor 初始双防、perStack 每层比例、maxStack 上限）", "按承受伤害次数与叠层上限折算等效减伤", true);
            config[2200] = new SkillTemplateComponentConfig(2200, "焰衣", "状态效果", new string[]{"Buff"}, "Buff:焰:damageRate=1;BuffEffect:damageRate;BuffTime=5", 2, 2.5f, "获得焰衣：每秒对自身周围 Area 内敌人造成技能伤害的 damageRate（伤害%，默认 100%，焰）", "按技能每秒伤害×伤害%×持续时间×覆盖人数估值", true);
            config[2210] = new SkillTemplateComponentConfig(2210, "威震", "状态效果", new string[]{"Buff"}, "Buff:震;BuffEffect:stunChance;Strength2:stunChance=0.4,0.45,0.5,0.55,0.6", 3, 2.5f, "普攻按概率眩晕目标并转为真实伤害（震）", "按攻速与眩晕时长折算期望收益", true);
            config[2300] = new SkillTemplateComponentConfig(2300, "时长延长小", "状态效果", null, "BuffTimeRate=1.3", 1, 1.5f, "Buff 持续时间延长 30%", "按持续时间等比放大所属状态效果的价值", false);
            config[2301] = new SkillTemplateComponentConfig(2301, "时长延长中", "状态效果", null, "BuffTimeRate=1.5", 2, 2.5f, "Buff 持续时间延长 50%", "按持续时间等比放大所属状态效果的价值", false);
            config[2310] = new SkillTemplateComponentConfig(2310, "效果提升50", "状态效果", null, "BuffEffectRate=1.5", 2, 2f, "该技能所有状态组件的 Buff 效果数值提升 50%", "乘性放大，按被提升状态的基准价值折算", true);
            config[2311] = new SkillTemplateComponentConfig(2311, "效果提升100", "状态效果", null, "BuffEffectRate=2", 3, 3.5f, "该技能所有状态组件的 Buff 效果数值提升 100%", "乘性放大，按被提升状态的基准价值折算", true);
            config[3000] = new SkillTemplateComponentConfig(3000, "低血筛选", "目标筛选", new string[]{"Heal","Buff"}, "SelectFirst:lowestHp", 1, 1f, "优先选择最低血量目标", "选择性越强未必更贵；看是否提高击杀或救援效率", false);
            config[3010] = new SkillTemplateComponentConfig(3010, "后排筛选", "目标筛选", null, "SelectFirst:farthest", 1, 0.5f, "优先选择后排（最远）目标", "选择性越强未必更贵；看是否提高击杀或救援效率", false);
            config[3020] = new SkillTemplateComponentConfig(3020, "英雄限定", "目标筛选", null, "SelectOnly:hero", 2, 1.5f, "仅对英雄/特定单位生效", "需看筛选是否提高击杀或覆盖效果", false);
            config[3030] = new SkillTemplateComponentConfig(3030, "友军筛选", "目标筛选", new string[]{"Heal","Buff"}, "SelectOnly:ally", 1, 0.5f, "仅对特定类型友军生效", "需看筛选是否提高救援或覆盖效果", false);
            config[3040] = new SkillTemplateComponentConfig(3040, "最高血量", "目标筛选", null, "SelectFirst:highestHp", 2, 1f, "优先选择生命百分比最高的目标", "选择性越强未必更贵；看是否提高救援或承伤效率", false);
            config[3050] = new SkillTemplateComponentConfig(3050, "最高法强", "目标筛选", null, "SelectFirst:highestAp", 2, 1f, "优先选择法术强度最高的目标", "选择性越强未必更贵；看是否提高增益收益", false);
            config[3060] = new SkillTemplateComponentConfig(3060, "远程优先", "目标筛选", null, "SelectFirst:rangedFirst", 1, 0.5f, "优先选择远程单位，无匹配时回退其余单位", "选择性本身不增值，看是否契合阵容", false);
            config[3070] = new SkillTemplateComponentConfig(3070, "近战优先", "目标筛选", null, "SelectFirst:meleeFirst", 1, 0.5f, "优先选择近战单位，无匹配时回退其余单位", "选择性本身不增值，看是否契合阵容", false);
            config[3080] = new SkillTemplateComponentConfig(3080, "排除自身", "目标筛选", new string[]{"Heal","Buff"}, "SelectOnly:notSelf", 1, 0.5f, "选人时排除施法者自身", "结构性开关，本身不增值", false);
            config[3090] = new SkillTemplateComponentConfig(3090, "施法者必含", "目标筛选", null, "TargetType:selfAlways", 1, 0.5f, "选人时施法者必然包含在内", "结构性开关，本身不增值", false);
            config[3100] = new SkillTemplateComponentConfig(3100, "满血跳过", "目标筛选", null, "SelectOnly:notFullHp", 1, 0.5f, "跳过满血目标，避免溢出浪费", "结构性开关，本身不增值", false);
            config[3130] = new SkillTemplateComponentConfig(3130, "不按首段去重", "目标筛选", null, "TargetType:noDedupe", 1, 0.5f, "多次结算时不按首段效果去重", "结构性开关，本身不增值", false);
            config[9000] = new SkillTemplateComponentConfig(9000, "斩杀", "特殊机制", new string[]{"Damage"}, "TargetType:execute;Strength2:execRate=2;Strength2:execHp=0.3", 4, 5f, "按血量阈值直接击杀或额外倍率", "特殊机制，人工评审，不套用普通伤害价格表", true);
            config[9010] = new SkillTemplateComponentConfig(9010, "驱散", "特殊机制", new string[]{"Damage","Heal"}, "TargetType:dispel", 3, 3f, "移除目标增益/减益", "特殊机制，人工评审", true);
            config[9020] = new SkillTemplateComponentConfig(9020, "破盾", "特殊机制", new string[]{"Damage"}, "TargetType:antiShield;TargetType:armorIgnore;Strength2:antiShieldRate=0.5,0.7,0.9,1.2,1.5", 2, 2f, "绕过或击破护盾", "特殊机制，人工评审", true);
            config[9030] = new SkillTemplateComponentConfig(9030, "位移", "特殊机制", new string[]{"Damage"}, "Strength2:pushMin=10;Strength2:pushMax=20", 2, 1.5f, "击退/拉近/换位", "特殊机制，人工评审", true);
            config[9050] = new SkillTemplateComponentConfig(9050, "永久成长攻速", "特殊机制", null, "TargetType:permanent;Strength2:selfAtkSpeed=0.08,0.1,0.12,0.14,0.17", 3, 3f, "每次施放永久提升自身攻击速度（selfAtkSpeed）", "特殊机制，人工评审", true);
            config[9072] = new SkillTemplateComponentConfig(9072, "弹道", "特殊机制", new string[]{"单体伤害大", "单体伤害中", "单体伤害小", "群体伤害近距离大", "群体伤害近距离中", "群体伤害近距离小", "群体伤害自身中心大", "群体伤害自身中心中", "群体伤害自身中心小"}, "TargetType:missile;Strength2:missileRate=0.5,0.6,0.7,0.8,1", 1, 0f, "逐个命中的飞行物", "按飞弹倍率与命中数共同估值", false);
            config[9080] = new SkillTemplateComponentConfig(9080, "概率双击", "特殊机制", new string[]{"Damage"}, "TargetType:doubleChance;Strength2:doubleChance=0.3;Strength2:doubleRate=2", 3, 3f, "有概率触发额外一次结算（doubleChance/doubleRate）", "按期望额外伤害估值", false);
            config[9081] = new SkillTemplateComponentConfig(9081, "自损", "特殊机制", new string[]{"单体伤害大", "单体伤害中", "单体伤害小"}, "TargetType:selfHurt;Strength2:selfHurt=0.2", 1, -1f, "施放时消耗自身生命（selfHurt），属于代价项", "负向项，抵减技能总价值", false);
            config[9082] = new SkillTemplateComponentConfig(9082, "燃烧法力", "特殊机制", new string[]{"Damage"}, "TargetType:mpBurn;Strength2:mpBurn=20,25,30,35,40", 2, 2f, "燃烧目标法力（mpBurn），压制技能循环", "按削弱的法力与技能频率估值", false);
            config[9083] = new SkillTemplateComponentConfig(9083, "按护盾加成伤害", "特殊机制", new string[]{"Damage"}, "TargetType:shieldBonus;Strength2:shieldBonusRate=1.5,1.8,2,2.2,2.5", 3, 3f, "按目标护盾量追加伤害（shieldBonusRate）", "对护盾阵容收益高，按期望追伤估值", false);
            config[9084] = new SkillTemplateComponentConfig(9084, "兵种克制", "特殊机制", new string[]{"Damage"}, "TargetType:vsJob;Strength2:vsJob=盾|士;Strength2:vsJobRate=2", 3, 3f, "对特定兵种额外倍率（vsJob）", "按阵容命中率折算期望收益", false);
            config[9100] = new SkillTemplateComponentConfig(9100, "周围友军加盾", "状态效果", new string[]{"群体嘲讽"}, "TargetType:allyShield", 2, 2f, "施放时把本技能的护盾等 Buff 同时挂给自身 Area 内其他友军（施法者已单独获得）", "按覆盖友军数与护盾量估值", true);
            config[9150] = new SkillTemplateComponentConfig(9150, "残血增伤", "特殊机制", new string[]{"Damage"}, "TargetType:missingHp;Strength2:missingHpRate=0.5", 2, 2f, "按目标已损失生命追加伤害（missingHpRate）", "特殊机制，人工评审", true);
            config[9160] = new SkillTemplateComponentConfig(9160, "最大生命伤害", "特殊机制", new string[]{"Damage"}, "TargetType:maxHpBonus;Strength2:maxHpBonusRate=0.08", 3, 3f, "按目标最大生命追加伤害（maxHpBonusRate）", "特殊机制，人工评审", true);
            config[9190] = new SkillTemplateComponentConfig(9190, "持续场移动", "特殊机制", new string[]{"持续伤害大", "持续伤害中", "持续伤害小"}, "Strength2:moveDist=40", 2, 1.5f, "持续伤害场沿方向移动（moveDist）", "特殊机制，人工评审", true);
            config[9270] = new SkillTemplateComponentConfig(9270, "士兵士气", "特殊机制", null, "TargetType:morale;BuffTime=999", 2, 1.5f, "施放后为我方全体小兵各叠一层士兵士气（每层+5攻击、+5护甲，战斗内近似永久）", "特殊机制，人工评审", true);
            config[9280] = new SkillTemplateComponentConfig(9280, "士兵增伤", "特殊机制", null, "Strength2:soldierDamage=0.02,0.025,0.03,0.035,0.04", 2, 2f, "伤害随我方存活士兵数提升：伤害 × (1 + 士兵数 × soldierDamage)", "特殊机制，人工评审", true);
            config[9290] = new SkillTemplateComponentConfig(9290, "士兵回蓝", "特殊机制", null, "Strength2:soldierMpRestore=0.1,0.15,0.2,0.25,0.3", 2, 1.5f, "施放成功后按我方存活士兵数直接回复 MP：每名士兵 0.1~0.3 点，单次最多回复 3 点", "特殊机制，人工评审", true);

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

        public static SkillTemplateComponentConfig GetConfig(int id)
        {
            SkillTemplateComponentConfig data;
            if (config.TryGetValue(id, out data))
            {
                return data;
            }
            throw new NullReferenceException(string.Format("配置表SkillTemplateComponentConfig不存在id={0}", id));
        }

        private static Dictionary<string, int> idxName = new Dictionary<string, int>();
        public static SkillTemplateComponentConfig GetConfigByName(string val)
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

        public static void Assign(int id, SkillTemplateComponentConfig configData)
        {
            config[id] = configData; 
        }

        public static void Add(int id, SkillTemplateComponentConfig configData)
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
