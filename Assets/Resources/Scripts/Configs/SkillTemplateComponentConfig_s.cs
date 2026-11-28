﻿﻿﻿using System;
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
            {"Category", new FieldMetaInfo("类别(数值、射程、目标数、范围档、状态效果、持续、目标筛选、增伤、士兵、特殊机制、施放节奏)", "string", 0, "数值:#555B60,射程:#355F8F,目标数:#4678A8,范围档:#4C9C95,状态效果:#2F7D4E,持续:#3F9B62,目标筛选:#6B4A8F,增伤:#B03A48,士兵:#5F7F3F,特殊机制:#A06428,施放节奏:#8C7A2F")},
            {"Templates", new FieldMetaInfo("适用大类列表(Damage伤害/Heal治疗/Buff增益/Defend防御；也可直接填模板名；空=通用)", "string[]", 0)},
            {"Effect", new FieldMetaInfo("效果k-v(分号多绑定: 列[:子键]=数值；Buff属性只填一级(取Lv3)，如 Area=30、Buff:缓:slow=0.18;Buff:缓:atkSpeed=0.18;BuffTime=3、TargetType:missile、SelectOnly:hero、SelectFirst:lowestHp、TriggerCondition:hprate<50、None)", "string", 511)},
            {"Quality", new FieldMetaInfo("品质(1~4)", "int", 60, "qs,1:#666666,2:#33CC33,3:#3333FF,4:#C7A800")},
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
        ///类别(数值、射程、目标数、范围档、状态效果、持续、目标筛选、增伤、士兵、特殊机制、施放节奏)
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
            config[30] = new SkillTemplateComponentConfig(30, "射程小", "射程", null, "Range+=20", 1, 1.5f, "射程 +20（模板默认 40 → 60）", "差值关系：在模板默认射程上叠加，按可触达目标与触发成功率估值", false);
            config[31] = new SkillTemplateComponentConfig(31, "射程中", "射程", null, "Range+=40", 2, 3f, "射程 +40（模板默认 40 → 80）", "差值关系：在模板默认射程上叠加，按可触达目标与触发成功率估值", false);
            config[32] = new SkillTemplateComponentConfig(32, "射程大", "射程", null, "Range+=70", 4, 5f, "射程 +70（模板默认 40 → 110）", "差值关系：在模板默认射程上叠加，按可触达目标与触发成功率估值", false);
            config[40] = new SkillTemplateComponentConfig(40, "目标增小", "目标数", null, "TargetCount+=2", 2, 1.5f, "目标数 +2（模板默认群体 3 → 5）", "差值关系：在模板默认目标数上叠加", false);
            config[41] = new SkillTemplateComponentConfig(41, "目标增大", "目标数", null, "TargetCount+=5", 3, 3f, "目标数 +5（模板默认群体 3 → 8）", "差值关系：在模板默认目标数上叠加", false);
            config[50] = new SkillTemplateComponentConfig(50, "小范围", "范围档", null, "Area+=15", 1, 2f, "半径 +15（模板默认小范围 20 → 35）", "差值关系：在模板默认半径上叠加，按实际预期命中数估值", false);
            config[51] = new SkillTemplateComponentConfig(51, "中范围", "范围档", null, "Area+=30", 2, 3.5f, "半径 +30（模板默认小范围 20 → 50）", "差值关系：在模板默认半径上叠加，按实际预期命中数估值", false);
            config[52] = new SkillTemplateComponentConfig(52, "大范围", "范围档", null, "Area+=50", 4, 5f, "半径 +50（模板默认小范围 20 → 70）", "差值关系：在模板默认半径上叠加，按实际预期命中数估值", false);
            config[61] = new SkillTemplateComponentConfig(61, "初始法力50%", "施放节奏", null, "StartMpRate=0.5", 1, 2f, "开局初始 MP 为满值的 50%（StartMpRate=0.5）", "正向项：提前首次施放窗口，按发动频率提升估值", false);
            config[62] = new SkillTemplateComponentConfig(62, "初始法力100%", "施放节奏", null, "StartMpRate=1", 2, 4f, "开局 MP 满值（StartMpRate=1），可立即施放第一次技能", "正向项：开局即满蓝，按首次施放提前量与后续循环估值", false);
            config[91] = new SkillTemplateComponentConfig(91, "无衰减", "特殊机制", new string[]{"跳跃弹射-伤害大","跳跃弹射-伤害中","跳跃弹射-伤害小","跳跃弹射-治疗大","跳跃弹射-治疗中","跳跃弹射-治疗小"}, "Strength2:jumpDecay=1", 2, 2f, "跳跃链每跳不再衰减（jumpDecay=1），总输出提升", "正向项：按跳数折算额外输出，避免与主数值重复计价", false);
            config[92] = new SkillTemplateComponentConfig(92, "衰减中", "特殊机制", new string[]{"跳跃弹射-伤害大","跳跃弹射-伤害中","跳跃弹射-伤害小","跳跃弹射-治疗大","跳跃弹射-治疗中","跳跃弹射-治疗小"}, "Strength2:jumpDecay=0.7", 1, -2f, "跳跃链每跳衰减加重到0.7（默认0.85）", "负向项：按跳数折减总输出", false);
            config[93] = new SkillTemplateComponentConfig(93, "衰减大", "特殊机制", new string[]{"跳跃弹射-伤害大","跳跃弹射-伤害中","跳跃弹射-伤害小","跳跃弹射-治疗大","跳跃弹射-治疗中","跳跃弹射-治疗小"}, "Strength2:jumpDecay=0.5", 1, -3.5f, "跳跃链每跳衰减加重到0.5（默认0.85）", "负向项：按跳数折减总输出", false);
            config[120] = new SkillTemplateComponentConfig(120, "MP小增加", "施放节奏", null, "MpCost+=2", 1, -2f, "技能 MP 消耗 +20（MpCost +2×10），发动频率略降", "负向项：节奏成本，按实际发动频率抵减技能总价值", false);
            config[121] = new SkillTemplateComponentConfig(121, "MP中增加", "施放节奏", null, "MpCost+=4", 1, -3.5f, "技能 MP 消耗 +40（MpCost +4×10），发动频率下降", "负向项：节奏成本，按实际发动频率抵减技能总价值", false);
            config[122] = new SkillTemplateComponentConfig(122, "MP大增加", "施放节奏", null, "MpCost+=6", 1, -5f, "技能 MP 消耗 +60（MpCost +6×10），发动频率明显下降", "负向项：节奏成本，按实际发动频率抵减技能总价值", false);
            config[123] = new SkillTemplateComponentConfig(123, "MP小节省", "施放节奏", null, "MpCost-=2", 1, 2f, "技能 MP 消耗 -20（MpCost -2×10），发动频率略升", "正向项：节奏收益，按实际发动频率提升技能总价值", false);
            config[124] = new SkillTemplateComponentConfig(124, "MP中节省", "施放节奏", null, "MpCost-=4", 2, 3.5f, "技能 MP 消耗 -40（MpCost -4×10），发动频率提升", "正向项：节奏收益，按实际发动频率提升技能总价值", false);
            config[125] = new SkillTemplateComponentConfig(125, "MP大节省", "施放节奏", null, "MpCost-=6", 4, 5f, "技能 MP 消耗 -60（MpCost -6×10），发动频率明显提升", "正向项：节奏收益，按实际发动频率提升技能总价值", false);
            config[140] = new SkillTemplateComponentConfig(140, "大概率", "施放节奏", null, "Rate=0.6", 1, -1f, "每次行动发动概率较高（Rate），百分比", "负向项：节奏成本，按实际发动频率抵减技能总价值", false);
            config[141] = new SkillTemplateComponentConfig(141, "中概率", "施放节奏", null, "Rate=0.4", 1, -2.5f, "每次行动发动概率中等（Rate），百分比", "负向项：节奏成本，按实际发动频率抵减技能总价值", false);
            config[142] = new SkillTemplateComponentConfig(142, "小概率", "施放节奏", null, "Rate=0.2", 1, -4f, "每次行动发动概率较低（Rate），百分比", "负向项：节奏成本，按实际发动频率抵减技能总价值", false);
            config[150] = new SkillTemplateComponentConfig(150, "光环耗蓝15x", "施放节奏", new string[]{"光环中"}, "AuraMpRate=15", 1, 1.5f, "光环开启期间每秒消耗 15 点 MP（默认 20）", "正向项：耗蓝更慢，光环持续时间更长", false);
            config[151] = new SkillTemplateComponentConfig(151, "光环耗蓝20x", "施放节奏", new string[]{"光环中"}, "AuraMpRate=20", 1, 0f, "光环开启期间每秒消耗 20 点 MP（默认）", "基准档，不额外计价", false);
            config[152] = new SkillTemplateComponentConfig(152, "光环耗蓝25x", "施放节奏", new string[]{"光环中"}, "AuraMpRate=25", 1, -1.5f, "光环开启期间每秒消耗 25 点 MP", "负向项：耗蓝更快，光环持续时间更短", false);
            config[153] = new SkillTemplateComponentConfig(153, "光环耗蓝30x", "施放节奏", new string[]{"光环中"}, "AuraMpRate=30", 1, -3f, "光环开启期间每秒消耗 30 点 MP", "负向项：耗蓝更快，光环持续时间更短", false);
            config[154] = new SkillTemplateComponentConfig(154, "光环耗蓝40x", "施放节奏", new string[]{"光环中"}, "AuraMpRate=40", 1, -4.5f, "光环开启期间每秒消耗 40 点 MP", "负向项：耗蓝更快，光环持续时间更短", false);
            config[160] = new SkillTemplateComponentConfig(160, "生命值低于50%", "施放节奏", null, "TriggerCondition:hprate<50", 2, .5f, "仅当自身生命值低于 50% 时才可触发该技能（通用组件）", "触发条件限制越严价值越高，按触发覆盖率折算", false);
            config[161] = new SkillTemplateComponentConfig(161, "MP叠3层", "施放节奏", null, "MpLimit=3", 3, 3f, "MP 最多累积到 3×MpCost，每次发动扣 1×MpCost（通用组件）", "正向项：按可持续连续施放次数提升估值", false);
            config[162] = new SkillTemplateComponentConfig(162, "MP叠5层", "施放节奏", null, "MpLimit=5", 4, 5f, "MP 最多累积到 5×MpCost，每次发动扣 1×MpCost（通用组件）", "正向项：按可持续连续施放次数提升估值", false);
            config[163] = new SkillTemplateComponentConfig(163, "多段", "施放节奏", null, "Strength2:hitCount=3,3,4,4,5;Strength2:hitInterval=0.2", 2, 2f, "同一目标分多段结算伤害，每段间隔 hitInterval 秒（hitCount 可逐级配置）", "按总段数与单段数值折算总输出，注意目标死亡会截断剩余段数", false);
            config[1002] = new SkillTemplateComponentConfig(1002, "减速", "状态效果", new string[]{"Damage","Buff"}, "Buff:缓:slow=0.1;Buff:缓:atkSpeed=0.15;BuffTime=4", 2, 2.2f, "降低目标移动与攻速（缓）", "缓同时降低移速与攻速，按复合效果核算", false);
            config[1012] = new SkillTemplateComponentConfig(1012, "减防", "状态效果", new string[]{"Damage","Buff"}, "Buff:破:armorDown=25;BuffTime=4", 1, 1.5f, "降低目标护甲（破）", "按效果强度、持续时间、命中对象数综合估值", false);
            config[1022] = new SkillTemplateComponentConfig(1022, "减攻", "状态效果", new string[]{"Damage","Buff"}, "Buff:慑:atkDown=15;BuffTime=4", 1, 1.5f, "降低目标攻击（慑）", "按效果强度、持续时间、命中对象数综合估值", false);
            config[1032] = new SkillTemplateComponentConfig(1032, "减疗", "状态效果", new string[]{"Damage","Buff"}, "Buff:疫;Strength2:healDown=0.3;BuffTime=4", 1, 1f, "降低目标受到的治疗（疫）", "按效果强度、持续时间、命中对象数综合估值", false);
            config[1042] = new SkillTemplateComponentConfig(1042, "增伤", "状态效果", new string[]{"Damage","Buff"}, "Buff:伤:damagedUp=0.15;BuffTime=4", 2, 1.8f, "使目标受到的伤害提升（伤）", "按效果强度、持续时间、命中对象数综合估值", false);
            config[1052] = new SkillTemplateComponentConfig(1052, "眩晕", "状态效果", new string[]{"Damage","Buff"}, "BuffTime=1.5;Buff:乱", 2, 3f, "无法行动（乱）", "控制按覆盖量计价：强度、时长、命中数、施放频率", false);
            config[1062] = new SkillTemplateComponentConfig(1062, "定身", "状态效果", new string[]{"Damage","Buff"}, "BuffTime=1.5;Buff:停", 1, 1f, "无法移动（停）", "控制按覆盖量计价：强度、时长、命中数、施放频率", false);
            config[1072] = new SkillTemplateComponentConfig(1072, "混乱", "状态效果", new string[]{"Damage","Buff"}, "BuffTime=1.5;Buff:惑", 3, 3.5f, "不受控制，攻击最近的友方单位（惑）", "控制按覆盖量计价：强度、时长、命中数、施放频率", false);
            config[1080] = new SkillTemplateComponentConfig(1080, "叛逃", "状态效果", new string[]{"Damage","Buff"}, "Buff:叛;BuffTime=2", 4, 3.5f, "命中后目标短时脱离战斗（叛）", "按控制覆盖量计价：时长、命中数、施放频率", true);
            config[1090] = new SkillTemplateComponentConfig(1090, "自身易伤", "状态效果", new string[]{"Buff"}, "Buff:伤:damagedUp=0.2;BuffTime=5", 2, -1.5f, "自身受到的伤害提升（伤），属于代价项", "负向项，抵减技能总价值", false);
            config[1100] = new SkillTemplateComponentConfig(1100, "中毒", "状态效果", new string[]{"Damage","Buff"}, "Buff:毒:dot=22;BuffTime=4", 2, 2.4f, "每秒造成固定跳伤（毒），不随属性成长", "跳伤按持续时间折算总量估值", false);
            config[1101] = new SkillTemplateComponentConfig(1101, "持续伤害", "状态效果", new string[]{"Damage","Buff"}, "Buff:败:dot=15;BuffTime=5", 2, 2f, "每秒造成固定跳伤（败）", "跳伤不随属性成长，按持续时间折算总量估值", false);
            config[1120] = new SkillTemplateComponentConfig(1120, "连锁扩散", "状态效果", new string[]{"单体减益","群体减益"}, "Buff:锁:chain=0.4;BuffTime=5", 3, 3f, "命中后向邻近单位扩散效果（锁）", "按传递伤害的比例与覆盖人数估值", true);
            config[2012] = new SkillTemplateComponentConfig(2012, "加攻", "状态效果", null, "Buff:攻:atk=15;BuffTime=5", 1, 1.5f, "提升攻击力", "按属性类型、强度、持续时间、覆盖人数综合估值", false);
            config[2022] = new SkillTemplateComponentConfig(2022, "加护甲", "状态效果", null, "Buff:甲:armor=25;BuffTime=5", 1, 1.5f, "提升护甲", "按属性类型、强度、持续时间、覆盖人数综合估值", false);
            config[2032] = new SkillTemplateComponentConfig(2032, "加法强", "状态效果", null, "Buff:术:ap=15;BuffTime=5", 1, 1.2f, "提升法术强度", "按属性类型、强度、持续时间、覆盖人数综合估值", false);
            config[2042] = new SkillTemplateComponentConfig(2042, "加魔抗", "状态效果", null, "Buff:抗:magicRes=25;BuffTime=5", 1, 1.2f, "提升魔法抗性", "按属性类型、强度、持续时间、覆盖人数综合估值", false);
            config[2052] = new SkillTemplateComponentConfig(2052, "加攻速", "状态效果", null, "Buff:快:atkSpeed=0.15;BuffTime=5", 1, 1.5f, "提升攻击速度", "按属性类型、强度、持续时间、覆盖人数综合估值", false);
            config[2062] = new SkillTemplateComponentConfig(2062, "加移速", "状态效果", null, "Buff:速:moveSpeed=0.1;BuffTime=5", 1, 0.5f, "提升移动速度", "按属性类型、强度、持续时间、覆盖人数综合估值", false);
            config[2082] = new SkillTemplateComponentConfig(2082, "加生命回复", "状态效果", null, "Buff:血:hpRegen=10;BuffTime=5", 1, 1.2f, "提升每秒生命回复", "按战斗时长折算总量后估值", false);
            config[2092] = new SkillTemplateComponentConfig(2092, "自身增伤", "状态效果", null, "Buff:重:damageUp=0.15;BuffTime=5", 2, 1.8f, "提升自身造成的伤害（重）", "与使目标受伤提升区分，按持续时间内输出增量估值", false);
            config[2102] = new SkillTemplateComponentConfig(2102, "护盾", "状态效果", null, "Buff:盾;Strength2:shield=100;BuffTime=10", 2, 2.5f, "吸收伤害的护盾（盾）", "护盾与治疗量分别估值", false);
            config[2112] = new SkillTemplateComponentConfig(2112, "减伤", "状态效果", null, "Buff:硬:shieldReduce=0.15;BuffTime=5", 2, 2f, "按百分比降低受到的伤害（硬）", "按持续时间内吸收的伤害量估值", false);
            config[2122] = new SkillTemplateComponentConfig(2122, "暴击", "状态效果", null, "Buff:暴:critRate=0.2;BuffTime=5", 2, 2f, "提升暴击率（暴）", "按期望输出增量估值", false);
            config[2132] = new SkillTemplateComponentConfig(2132, "法术暴击", "状态效果", null, "Buff:法暴:magicCritChance=0.3;Buff:法暴:magicCritRate=2;BuffTime=5", 2, 2f, "获得法术暴击：法术技能伤害 30% 概率翻倍（法暴，Buff 300020）", "按法术伤害占比×暴击概率×(倍率-1)折算期望增伤", false);
            config[2142] = new SkillTemplateComponentConfig(2142, "吸血", "状态效果", null, "Buff:吸:suck=0.3;BuffTime=5", 3, 2.2f, "按造成伤害比例回复自身生命（吸）", "按持续时间内实际输出量与吸血比例折算", false);
            config[2152] = new SkillTemplateComponentConfig(2152, "闪避", "状态效果", null, "Buff:闪:evade=0.2;BuffTime=5", 3, 2.2f, "按概率完全闪避伤害（闪）", "按期望免伤折算等效治疗量", false);
            config[2170] = new SkillTemplateComponentConfig(2170, "伤害反射", "状态效果", new string[]{"Buff","Defend"}, "Buff:反:reflect=0.25;BuffTime=5", 3, 2.5f, "给自己挂反伤 buff（reflect）", "按承受伤害与反弹比例折算", false);
            config[2180] = new SkillTemplateComponentConfig(2180, "生命链接", "状态效果", null, "Buff:链:damageShare=0.5;BuffTime=5", 3, 3f, "把目标受到的伤害按比例分摊给链接单位（链）", "按链接单位数与分摊比例折算等效减伤", true);
            config[2190] = new SkillTemplateComponentConfig(2190, "受击叠防", "状态效果", new string[]{"Buff","Defend"}, "Buff:守:armor=20;Buff:守:perStack=0.2;Strength2:maxStack=5;BuffTime=5", 3, 2.5f, "状态期间每次受击叠加双防（armor 初始双防、perStack 每层比例、maxStack 上限）", "按承受伤害次数与叠层上限折算等效减伤", true);
            config[2200] = new SkillTemplateComponentConfig(2200, "焰衣", "状态效果", new string[]{"Buff","Defend"}, "Buff:焰:damageShare=0.25;BuffTime=4;TargetCount=3;DamageStrength=120,180,280,440,640", 4, 6f, "获得焰衣：每秒对自身周围 Area 内至多 TargetCount 名敌人造成技能伤害×damageShare（伤害分摊，默认 0.25=4 秒总伤害的 1/4，焰）", "按技能每秒伤害×伤害分摊×持续时间×覆盖人数估值", true);
            config[2201] = new SkillTemplateComponentConfig(2201, "雷衣", "状态效果", new string[]{"Buff","Defend"}, "Buff:雷:damageShare=0.2;BuffTime=5;TargetCount=1;DamageStrength=225,340,525,825,1200", 4, 8f, "获得雷衣：每秒对自身 Area 内随机 1 名敌人降下雷击（5 秒总伤害按 1/5 分摊）", "按技能每秒伤害×伤害分摊×持续时间×覆盖人数估值", true);
            config[2202] = new SkillTemplateComponentConfig(2202, "命中减益", "状态效果", new string[]{"Buff"}, "Strength2:hitBuff=破;Strength2:hitBuffTime=2;Strength2:armorDown=25", 1, 4f, "命中目标时叠加减益 Buff：hitBuff 为 Buff 短名、hitBuffTime 为持续秒数，可选数值键（如 armorDown 护甲降低）", "按减益强度与持续时间覆盖率估值", false);
            config[2210] = new SkillTemplateComponentConfig(2210, "威震", "状态效果", null, "Buff:震;Strength2:stunChance=0.5;BuffTime=5", 4, 4f, "普攻按概率眩晕目标并转为真实伤害（震）", "按攻速与眩晕时长折算期望收益", true);
            config[2300] = new SkillTemplateComponentConfig(2300, "时长延长小", "状态效果", null, "BuffTimeRate=1.3", 1, 1.5f, "Buff 持续时间延长 30%", "按持续时间等比放大所属状态效果的价值", false);
            config[2301] = new SkillTemplateComponentConfig(2301, "时长延长中", "状态效果", null, "BuffTimeRate=1.5", 2, 2.5f, "Buff 持续时间延长 50%", "按持续时间等比放大所属状态效果的价值", false);
            config[2305] = new SkillTemplateComponentConfig(2305, "时长逐级延长20", "状态效果", null, "BuffTimeRate=1,1.3,1.6,2,2.5", 3, 3f, "Buff 持续时间随等级逐级延长 20%（Lv1 +20% … Lv5 +100%）", "按持续时间等比放大所属状态效果的价值", true);
            config[2310] = new SkillTemplateComponentConfig(2310, "效果提升30", "状态效果", null, "BuffEffectRate=1.3", 2, 1.5f, "该技能所有状态组件的 Buff 效果数值提升 30%", "乘性放大，按被提升状态的基准价值折算", true);
            config[2311] = new SkillTemplateComponentConfig(2311, "效果提升50", "状态效果", null, "BuffEffectRate=1.5", 3, 2.5f, "该技能所有状态组件的 Buff 效果数值提升 50%", "乘性放大，按被提升状态的基准价值折算", true);
            config[2910] = new SkillTemplateComponentConfig(2910, "效果逐级提升20", "状态效果", null, "BuffEffectRate=1,1.5,2.2,3,4", 3, 4f, "该技能所有状态组件的 Buff 效果数值随等级逐级提升 20%（Lv1 +20% … Lv5 +100%）", "乘性放大，按被提升状态的基准价值折算", true);
            config[3000] = new SkillTemplateComponentConfig(3000, "低血筛选", "目标筛选", new string[]{"Heal","Buff"}, "SelectFirst:lowestHp", 1, .5f, "优先选择最低血量目标", "选择性越强未必更贵；看是否提高击杀或救援效率", false);
            config[3010] = new SkillTemplateComponentConfig(3010, "后排筛选", "目标筛选", null, "SelectFirst:farthest", 1, .3f, "优先选择后排（最远）目标", "选择性越强未必更贵；看是否提高击杀或救援效率", false);
            config[3020] = new SkillTemplateComponentConfig(3020, "英雄限定", "目标筛选", null, "SelectOnly:hero", 2, .5f, "仅对英雄/特定单位生效", "需看筛选是否提高击杀或覆盖效果", false);
            config[3030] = new SkillTemplateComponentConfig(3030, "友军筛选", "目标筛选", new string[]{"Heal","Buff"}, "SelectOnly:ally", 1, 0f, "仅对特定类型友军生效", "需看筛选是否提高救援或覆盖效果", false);
            config[3040] = new SkillTemplateComponentConfig(3040, "最高血量", "目标筛选", null, "SelectFirst:highestHp", 1, .3f, "优先选择生命百分比最高的目标", "选择性越强未必更贵；看是否提高救援或承伤效率", false);
            config[3050] = new SkillTemplateComponentConfig(3050, "最高法强", "目标筛选", null, "SelectFirst:highestAp", 1, .3f, "优先选择法术强度最高的目标", "选择性越强未必更贵；看是否提高增益收益", false);
            config[3060] = new SkillTemplateComponentConfig(3060, "远程优先", "目标筛选", null, "SelectFirst:rangedFirst", 1, .3f, "优先选择远程单位，无匹配时回退其余单位", "选择性本身不增值，看是否契合阵容", false);
            config[3070] = new SkillTemplateComponentConfig(3070, "近战优先", "目标筛选", null, "SelectFirst:meleeFirst", 1, .3f, "优先选择近战单位，无匹配时回退其余单位", "选择性本身不增值，看是否契合阵容", false);
            config[3080] = new SkillTemplateComponentConfig(3080, "排除自身", "目标筛选", new string[]{"Heal","Buff"}, "SelectOnly:notSelf", 1, 0f, "选人时排除施法者自身", "结构性开关，本身不增值", false);
            config[3090] = new SkillTemplateComponentConfig(3090, "施法者必含", "目标筛选", null, "TargetType:selfAlways", 1, .5f, "选人时施法者必然包含在内", "结构性开关，本身不增值", false);
            config[3100] = new SkillTemplateComponentConfig(3100, "满血跳过", "目标筛选", null, "SelectOnly:notFullHp", 1, .3f, "跳过满血目标，避免溢出浪费", "结构性开关，本身不增值", false);
            config[3130] = new SkillTemplateComponentConfig(3130, "不按首段去重", "目标筛选", null, "TargetType:noDedupe", 1, 0f, "多次结算时不按首段效果去重", "结构性开关，本身不增值", false);
            config[3140] = new SkillTemplateComponentConfig(3140, "受击·伤害源为英雄", "目标筛选", new string[]{"Defend"}, "Strength2:DamageSource=hero", 2, -1f, "仅当本次伤害来源为英雄时触发防御（小兵/召唤物造成的伤害不触发）", "限制触发来源，按覆盖率折价", false);
            config[3150] = new SkillTemplateComponentConfig(3150, "受击·伤害>自身生命3%", "施放节奏", new string[]{"Defend"}, "Strength2:minHpRate=0.03", 2, -1f, "仅当本次伤害超过自身最大生命的 3% 时触发防御，过滤小额伤害", "限制触发频率，按覆盖率折价", false);
            config[4000] = new SkillTemplateComponentConfig(4000, "斩杀", "增伤", null, "TargetType:execute;Strength2:execRate=3;Strength2:execHp=0.3", 3, 3f, "按血量阈值直接击杀或额外倍率", "按目标低于阈值时的覆盖比例折算额外倍率", false);
            config[4010] = new SkillTemplateComponentConfig(4010, "残血增伤", "增伤", null, "TargetType:missingHp;Strength2:missingHpRate=0.7", 2, 2f, "按施法者已损失生命追加伤害（missingHpRate）", "按施法者残血阶段的平均损失生命折算增伤", false);
            config[4020] = new SkillTemplateComponentConfig(4020, "高血增伤", "增伤", null, "TargetType:highHp;Strength2:highHpThreshold=0.5;Strength2:highHpRate=0.5", 3, 3f, "目标生命比例高于阈值（highHpThreshold，默认 50%）时，本次伤害 ×(1+highHpRate)（默认 +50%）", "按目标处于高血量阶段的时间占比与增伤比例估值", false);
            config[4030] = new SkillTemplateComponentConfig(4030, "兵种克制", "增伤", null, "TargetType:vsJob;Strength2:vsJob=盾|士;Strength2:vsJobRate=1.5", 3, 2f, "对特定兵种额外倍率（vsJob）", "按阵容命中率折算期望收益", false);
            config[4040] = new SkillTemplateComponentConfig(4040, "按护盾加成伤害", "增伤", null, "TargetType:shieldBonus;Strength2:shieldBonusRate=2", 3, 3f, "按目标护盾量追加伤害（shieldBonusRate）", "对护盾阵容收益高，按期望追伤估值", false);
            config[4060] = new SkillTemplateComponentConfig(4060, "暴击率加成", "增伤", null, "Strength2:critRateBonus=0.5", 2, 1.4f, "技能伤害按施法者暴击率+0.7判定暴击，暴击时伤害×(1+暴击伤害倍率)，不依赖弹道形态", "按暴击率×暴击伤害倍率折算期望增伤", false);
            config[4070] = new SkillTemplateComponentConfig(4070, "概率双击", "增伤", null, "TargetType:doubleChance;Strength2:doubleChance=0.3;Strength2:doubleRate=2", 2, 3f, "有概率触发额外一次结算（doubleChance/doubleRate）", "按期望额外伤害估值", false);
            config[4080] = new SkillTemplateComponentConfig(4080, "随机增伤", "增伤", null, "TargetType:randomAmp;Strength2:randomAmpMin=0.8;Strength2:randomAmpMax=1.5", 2, 2f, "每次伤害结算在 randomAmpMin~randomAmpMax 倍之间随机（独立于暴击）", "按随机倍率期望值估值", false);
            config[4090] = new SkillTemplateComponentConfig(4090, "孤立增伤", "增伤", null, "TargetType:isolatedAmp;Strength2:isolatedAmpRate=0.4;Strength2:isolatedRange=1", 1, 1f, "目标周围 isolatedRange 格内没有其他敌方英雄时，伤害 ×(1+isolatedAmpRate)", "按阵型分散程度与触发覆盖率估值", false);
            config[4100] = new SkillTemplateComponentConfig(4100, "有盾增伤", "增伤", null, "TargetType:shieldedAmp;Strength2:shieldedAmpRate=0.3", 1, 1.5f, "施法者自身带护盾时，伤害 ×(1+shieldedAmpRate)", "按自身获得护盾的频率与覆盖率估值", false);
            config[4110] = new SkillTemplateComponentConfig(4110, "满血增伤", "增伤", null, "TargetType:fullHp;Strength2:fullHpRate=0.3", 2, 2f, "施法者满血时，伤害 ×(1+fullHpRate)", "按战斗前中期满血窗口估值", false);
            config[4120] = new SkillTemplateComponentConfig(4120, "逐次施法增伤", "增伤", null, "TargetType:castStack;Strength2:castStackRate=0.15;Strength2:castStackMax=2", 2, 2.3f, "本场第 2 次施放起伤害逐次提升：每次 +castStackRate，最多叠 castStackMax 层", "按一场战斗的预期施放次数与平均层数估值", false);
            config[4130] = new SkillTemplateComponentConfig(4130, "中心双倍", "增伤", null, "TargetType:centerDouble;Strength2:centerRate=2", 2, 3f, "范围伤害时，锁定的主目标承受 centerRate 倍伤害，其余目标正常", "按主目标承伤占比与倍率折算额外伤害", false);
            config[4140] = new SkillTemplateComponentConfig(4140, "距离增伤", "增伤", null, "TargetType:distanceAmp;Strength2:distanceAmpMin=1;Strength2:distanceAmpMax=2", 2, 3f, "伤害倍率随施法者到目标的距离线性提升：distanceAmpMin~distanceAmpMax，跨度为技能射程（含射程组件加成）", "远距离索敌技能近似稳定吃满，按中高档估值", false);
            config[5000] = new SkillTemplateComponentConfig(5000, "受击·仅物理", "目标筛选", new string[]{"Defend"}, "Strength2:onlyPhys=1", 2, -1f, "仅对物理伤害（普攻/物理技能）生效；不配受击类型组件时物理+魔法都算", "限制一半伤害来源，按覆盖率折价", false);
            config[5010] = new SkillTemplateComponentConfig(5010, "受击·仅魔法", "目标筛选", new string[]{"Defend"}, "Strength2:onlyMagic=1", 2, -1f, "仅对魔法伤害（法术技能）生效；不配受击类型组件时物理+魔法都算", "限制一半伤害来源，按覆盖率折价", false);
            config[5020] = new SkillTemplateComponentConfig(5020, "防御减免·40%", "数值", new string[]{"Defend"}, "Strength2:reduce=0.4", 1, 1.5f, "受伤前减免比例调整为 15%（模板基准 25%）", "负向项：按减免比例下降折算", false);
            config[5030] = new SkillTemplateComponentConfig(5030, "防御减免·60%", "数值", new string[]{"Defend"}, "Strength2:reduce=0.6", 2, 3f, "受伤前减免比例提升到 35%", "正向项：按减免比例提升折算", false);
            config[5040] = new SkillTemplateComponentConfig(5040, "反伤比例·30%", "数值", new string[]{"Defend"}, "Strength2:reflect=0.3", 2, 2f, "额外附带 15% 反伤（按减免后伤害反弹给攻击者）", "正向项：按反伤比例折算", false);
            config[5050] = new SkillTemplateComponentConfig(5050, "反伤比例·60%", "数值", new string[]{"Defend"}, "Strength2:reflect=0.6", 3, 4f, "额外附带 35% 反伤（按减免后伤害反弹给攻击者）", "正向项：按反伤比例折算", false);
            config[5060] = new SkillTemplateComponentConfig(5060, "防御减免·逐级", "数值", new string[]{"Defend"}, "Strength2:reduce=0.4,0.45,0.5,0.55,0.6", 2, 3f, "受伤前减免比例按等级成长：Lv1~5 依次为 40%/45%/50%/55%/60%", "正向项：按各级减免比例提升折算", false);
            config[6000] = new SkillTemplateComponentConfig(6000, "折射·弹射", "伤害组", new string[]{"单体伤害大","单体伤害中","单体伤害小"}, "Strength2:refractRate=0.3;Strength2:refractRadius=40;Strength2:refractCount=1", 2, 3f, "主伤害落地后，以主目标为中心 40 范围内随机折射 1 名敌人，造成 30% 伤害", "按额外目标数与折射比例折算总输出，人工评审", true);
            config[6001] = new SkillTemplateComponentConfig(6001, "折射·三连", "伤害组", new string[]{"单体伤害大","单体伤害中","单体伤害小"}, "Strength2:refractRate=0.3;Strength2:refractRadius=50;Strength2:refractCount=3", 4, 4.5f, "主伤害落地后，向周围 60 范围内折射最多 3 名敌人，各造成 30% 伤害", "按额外目标数（3 人）与折射比例折算，人工评审", true);
            config[6010] = new SkillTemplateComponentConfig(6010, "折射·范围小", "伤害组", new string[]{"单体伤害大","单体伤害中","单体伤害小"}, "Strength2:refractRate=0.3;Strength2:refractRadius=20;Strength2:refractCount=3", 3, 4f, "主伤害落地后，以主目标为中心 20 范围内最多 3 名敌人各受 30% 伤害", "按覆盖人数与折射比例折算，人工评审", true);
            config[6011] = new SkillTemplateComponentConfig(6011, "折射·范围中", "伤害组", new string[]{"单体伤害大","单体伤害中","单体伤害小"}, "Strength2:refractRate=0.3;Strength2:refractRadius=35;Strength2:refractCount=3", 3, 5f, "同折射·范围小，半径 35（覆盖更多目标）", "按覆盖人数与折射比例折算，人工评审", true);
            config[6012] = new SkillTemplateComponentConfig(6012, "折射·范围大", "伤害组", new string[]{"单体伤害大","单体伤害中","单体伤害小"}, "Strength2:refractRate=0.3;Strength2:refractRadius=50;Strength2:refractCount=3", 4, 6f, "同折射·范围小，半径 50（大范围扩散）", "按覆盖人数与折射比例折算，人工评审", true);
            config[6090] = new SkillTemplateComponentConfig(6090, "折射飞弹", "伤害组", new string[]{"单体伤害大","单体伤害中","单体伤害小"}, "TargetType:refractMissile;Strength2:refractSpeed=20", 1, 0.5f, "折射改为飞弹表现：以主目标位置为起点逐个飞向折射目标，速度取 refractSpeed（默认 20），命中结算派生伤害", "形态组件，不计分", false);
            config[6100] = new SkillTemplateComponentConfig(6100, "折射增幅·小", "伤害组", new string[]{"单体伤害大","单体伤害中","单体伤害小"}, "Strength2:refractAmp=0.15", 1, 1.5f, "折射比例 +15%（与折射比例加算，如 30% → 45%）", "正向项：按折射目标数与增量折算", true);
            config[6101] = new SkillTemplateComponentConfig(6101, "折射增幅·中", "伤害组", new string[]{"单体伤害大","单体伤害中","单体伤害小"}, "Strength2:refractAmp=0.3", 2, 3f, "折射比例 +30%（与折射比例加算，如 30% → 60%）", "正向项：按折射目标数与增量折算", true);
            config[6102] = new SkillTemplateComponentConfig(6102, "折射增幅·逐级", "伤害组", new string[]{"单体伤害大","单体伤害中","单体伤害小"}, "Strength2:refractAmp=0,0.05,0.1,0.15,0.2", 2, 2f, "折射比例随等级递增：Lv1~5 依次 +0%/+5%/+10%/+15%/+20%（加算）", "正向项：按各级增量与目标数折算", true);
            config[8000] = new SkillTemplateComponentConfig(8000, "士兵士气", "士兵", null, "TargetType:morale;BuffTime=999", 2, 1.5f, "施放后为我方全体小兵各叠一层士兵士气（每层+5攻击、+5护甲，战斗内近似永久）", "按覆盖士兵数与每层攻击/护甲折算", false);
            config[8010] = new SkillTemplateComponentConfig(8010, "士兵增伤", "士兵", null, "Strength2:soldierDamage=0.02,0.025,0.03,0.035,0.04", 2, 2f, "伤害随我方存活士兵数提升：伤害 × (1 + 士兵数 × soldierDamage)", "按平均存活士兵数与每兵增伤折算期望收益", false);
            config[8020] = new SkillTemplateComponentConfig(8020, "士兵回蓝", "士兵", null, "Strength2:soldierMpRestore=0.1,0.15,0.2,0.25,0.3", 2, 1.5f, "施放成功后按我方存活士兵数直接回复 MP：每名士兵 0.1~0.3 点，单次最多回复 3 点", "按平均存活士兵数与每兵回蓝折算 MP 收益", false);
            config[8030] = new SkillTemplateComponentConfig(8030, "周围友军加盾", "士兵", new string[]{"群体嘲讽"}, "TargetType:allyShield", 2, 2f, "施放时把本技能的护盾等 Buff 同时挂给自身 Area 内其他友军（施法者已单独获得，含小兵）", "按覆盖友军数与护盾量估值", false);
            config[9010] = new SkillTemplateComponentConfig(9010, "驱散", "特殊机制", new string[]{"Damage","Heal"}, "TargetType:dispel", 2, 2.5f, "移除目标增益/减益", "特殊机制，人工评审", true);
            config[9020] = new SkillTemplateComponentConfig(9020, "破盾", "特殊机制", null, "TargetType:antiShield;TargetType:armorIgnore;Strength2:antiShieldRate=0.9", 2, 2.5f, "绕过或击破护盾", "特殊机制，人工评审", true);
            config[9030] = new SkillTemplateComponentConfig(9030, "位移", "特殊机制", null, "Strength2:pushMin=15;Strength2:pushMax=25", 2, 1.5f, "击退/拉近/换位", "特殊机制，人工评审", true);
            config[9031] = new SkillTemplateComponentConfig(9031, "拖回出生点", "特殊机制", new string[]{"单体伤害大","单体伤害中","单体伤害小"}, "Strength2:pullToSpawn=1;Buff:停;BuffTime=1.5", 3, 4f, "命中后将目标强制拖到我方出生点（pullToSpawn），并挂「停」1.5 秒，防止其立刻走回", "强控：位移+定身，按控制时长与位移距离计价，人工评审", true);
            config[9040] = new SkillTemplateComponentConfig(9040, "治疗诅咒", "特殊机制", new string[]{"Heal"}, "TargetType:curseNearest;Strength2:curseRange=60", 2, 2.5f, "治疗时把 BuffId 挂给被治疗目标周围 curseRange 内最近的 TargetCount 名敌人", "按可覆盖敌人数与 debuff 强度估值，人工评审", true);
            config[9041] = new SkillTemplateComponentConfig(9041, "链接双方", "特殊机制", new string[]{"Heal"}, "TargetType:linkBoth", 2, 1.5f, "治疗目标后，把 BuffId 同时挂给目标与施法者自身（双方生命链接）", "按链接双方的自损/分摊收益估值，人工评审", true);
            config[9042] = new SkillTemplateComponentConfig(9042, "向前冲锋", "特殊机制", new string[]{"Damage"}, "TargetType:selfDash;Strength2:dashStop=10;Strength2:dashTime=0.3", 2, 2f, "施法后平滑冲到目标身前 dashStop 处（dashTime 秒），落地再结算伤害/Buff", "位移本身不增值，按战术收益人工评审", true);
            config[9043] = new SkillTemplateComponentConfig(9043, "后退", "特殊机制", new string[]{"Damage"}, "TargetType:selfBackstep;Strength2:backstepDist=30;Strength2:backstepTime=0.3", 2, 2f, "命中后沿目标→自身方向平滑后退 backstepDist（backstepTime 秒）", "位移本身不增值，按生存收益人工评审", true);
            config[9044] = new SkillTemplateComponentConfig(9044, "跳跃", "特殊机制", new string[]{"Damage"}, "TargetType:selfJump;Strength2:jumpStop=0;Strength2:jumpHeight=25;Strength2:jumpTime=0.55", 3, 2f, "施法后以抛物线跳到落点（jumpStop=0 落在目标脚下），落地再结算伤害/Buff", "位移本身不增值，按战术收益人工评审", true);
            config[9050] = new SkillTemplateComponentConfig(9050, "永久成长攻速", "特殊机制", null, "TargetType:permanent;Strength2:selfAtkSpeed=0.08,0.1,0.12,0.14,0.17", 3, 3.5f, "每次施放永久提升自身攻击速度（selfAtkSpeed）", "特殊机制，人工评审", true);
            config[9060] = new SkillTemplateComponentConfig(9060, "立刻回蓝", "特殊机制", new string[]{"Buff"}, "Strength2:mpRestore=3", 4, 6f, "施放后为每个目标立即回复 3 点法力（直接加 MP，不挂 Buff、不吃 mpRegen 加成）", "按回复法力折算技能循环收益", false);
            config[9070] = new SkillTemplateComponentConfig(9070, "弹道", "特殊机制", new string[]{"单体伤害大","单体伤害中","单体伤害小","群体伤害近距离大","群体伤害近距离中","群体伤害近距离小","群体伤害自身中心大","群体伤害自身中心中","群体伤害自身中心小"}, "TargetType:missile;SummonSpeed=20", 1, 0.5f, "以飞弹逐个结算，飞弹特效取技能 HitEffect，飞行速度取 SummonSpeed（为0时用单位导弹速度，最低14）", "形态组件，不计分", false);
            config[9081] = new SkillTemplateComponentConfig(9081, "自损", "特殊机制", new string[]{"单体伤害大","单体伤害中","单体伤害小"}, "TargetType:selfHurt;Strength2:selfHurt=0.2", 1, -1.5f, "施放时消耗自身生命（selfHurt），属于代价项", "负向项，抵减技能总价值", false);
            config[9082] = new SkillTemplateComponentConfig(9082, "燃烧法力", "特殊机制", null, "TargetType:mpBurn;Strength2:mpBurn=20", 2, 2f, "燃烧目标法力（mpBurn），压制技能循环", "按削弱的法力与技能频率估值", false);
            config[9083] = new SkillTemplateComponentConfig(9083, "法力转移", "特殊机制", new string[]{"Damage"}, "TargetType:mpTransfer;Strength2:mpTransferRange=40", 3, 4f, "命中后按 mpBurn 燃烧目标法力，实燃量×mpShareRate（默认1）转给 mpTransferRange 内法力最低、最近的1名友军；无友军只燃不转", "按燃蓝量与转移收益估值，人工评审", false);
            config[9084] = new SkillTemplateComponentConfig(9084, "近远分档", "特殊机制", new string[]{"Buff"}, "TargetType:rangeSplit", 1, .5f, "按目标攻击距离分流：近战取 BuffId 第1段，远程取第2段（BuffId/StrengthBuff1 逗号分列）", "结构性开关，本身不增值", false);
            config[9190] = new SkillTemplateComponentConfig(9190, "持续场移动", "特殊机制", new string[]{"持续伤害大","持续伤害中","持续伤害小"}, "Strength2:moveDist=40", 2, 1.5f, "持续伤害场沿方向移动（moveDist）", "特殊机制，人工评审", true);

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
