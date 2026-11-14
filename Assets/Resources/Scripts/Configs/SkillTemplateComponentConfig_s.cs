using System;
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
            {"Templates", new FieldMetaInfo("适用模板名列表(空=通用)", "string[]", 0)},
            {"Effect", new FieldMetaInfo("效果k-v(分号多绑定: 列[:子键]=Lv1~5数值；如 Area=30、Buff:缓:slow=0.1,0.12,0.15,0.2,0.25;BuffTime=3、TargetType:lowestHp、None)", "string", 289)},
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
        ///适用模板名列表（空=通用）
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
            config[10] = new SkillTemplateComponentConfig(10, "伤害倍率", "数值", new string[]{"单体伤害大","单体伤害中","单体伤害小","群体伤害近距离","单体治疗","范围治疗","自身强化","群体伤害自身中心","跳跃弹射"}, "None", 2, 2f, "按属性成长的伤害倍率", "与固定基值分开计量，避免重复计价", false);
            config[30] = new SkillTemplateComponentConfig(30, "施法射程", "射程", new string[]{"单体伤害大","单体伤害中","单体伤害小","群体伤害近距离","单体治疗","范围治疗","自身强化","光环","单体友方增益","群体友方增益","跳跃弹射"}, "Range=30,40,50,65,80", 1, 1f, "增加施法距离 Range，决定目标是否可被触达", "与攻击射程、AoE 半径区分，按可触达目标和触发成功率估值", false);
            config[40] = new SkillTemplateComponentConfig(40, "最大目标数", "目标数", new string[]{"单体伤害大","单体伤害中","单体伤害小","群体伤害近距离","单体治疗","范围治疗","群体伤害自身中心","群体友方增益","兵种增益","跳跃弹射"}, "TargetCount=2,3,3,4,5", 2, 1.5f, "最多命中 N 个目标", "有限目标数与范围内全部目标分开；TargetCount<=0 在不同脚本中可能表示不限，不可按零成本处理", false);
            config[50] = new SkillTemplateComponentConfig(50, "中范围", "范围档", new string[]{"群体伤害近距离","范围治疗","群体伤害自身中心"}, "Area=30,30,30,30,30", 1, 1f, "中半径 AoE 范围档（模板默认小范围=20，本组件改为30）", "按实际预期命中数估值，不按半径线性估价", false);
            config[51] = new SkillTemplateComponentConfig(51, "大范围", "范围档", new string[]{"群体伤害近距离","群体伤害自身中心"}, "Area=40,40,40,40,40", 2, 2f, "大半径 AoE 范围档（模板默认小范围=20，本组件改为40）", "按实际预期命中数估值，不按半径线性估价", false);
            config[90] = new SkillTemplateComponentConfig(90, "连击", "持续", new string[]{"单体伤害大","单体伤害中","单体伤害小","群体伤害近距离","群体伤害自身中心"}, "Strength2:hitCount=2;Strength2:hitRate=1", 3, 3f, "同一次施放结算多段", "按预期总段数估价，避免只按单次伤害估值", false);
            config[120] = new SkillTemplateComponentConfig(120, "MP小增加", "施放节奏", null, "MpCost=4,4,4,4,4", 1, -0.5f, "技能 MP 上限小幅提高（MpCost），发动频率略降", "负向项：节奏成本，按实际发动频率抵减技能总价值", false);
            config[121] = new SkillTemplateComponentConfig(121, "MP中增加", "施放节奏", null, "MpCost=7,7,7,7,7", 1, -1f, "技能 MP 上限中等提高（MpCost），标准节奏", "负向项：节奏成本，按实际发动频率抵减技能总价值", false);
            config[122] = new SkillTemplateComponentConfig(122, "MP大增加", "施放节奏", null, "MpCost=10,10,10,10,10", 1, -1.5f, "技能 MP 上限大幅提高（MpCost），发动频率明显下降", "负向项：节奏成本，按实际发动频率抵减技能总价值", false);
            config[130] = new SkillTemplateComponentConfig(130, "CD小增加", "施放节奏", null, "CD=1,1,1,1,1", 1, -0.5f, "发动冷却小幅增加（CD），秒", "负向项：节奏成本，按实际发动频率抵减技能总价值", false);
            config[131] = new SkillTemplateComponentConfig(131, "CD中增加", "施放节奏", null, "CD=2,2,2,2,2", 1, -1f, "发动冷却中等增加（CD），秒", "负向项：节奏成本，按实际发动频率抵减技能总价值", false);
            config[132] = new SkillTemplateComponentConfig(132, "CD大增加", "施放节奏", null, "CD=3.5,3.5,3.5,3.5,3.5", 1, -1.5f, "发动冷却大幅增加（CD），秒", "负向项：节奏成本，按实际发动频率抵减技能总价值", false);
            config[140] = new SkillTemplateComponentConfig(140, "大概率", "施放节奏", null, "Rate=0.6,0.6,0.6,0.6,0.6", 1, -0.5f, "每次行动发动概率较高（Rate），百分比", "负向项：节奏成本，按实际发动频率抵减技能总价值", false);
            config[141] = new SkillTemplateComponentConfig(141, "中概率", "施放节奏", null, "Rate=0.4,0.4,0.4,0.4,0.4", 1, -1f, "每次行动发动概率中等（Rate），百分比", "负向项：节奏成本，按实际发动频率抵减技能总价值", false);
            config[142] = new SkillTemplateComponentConfig(142, "小概率", "施放节奏", null, "Rate=0.2,0.2,0.2,0.2,0.2", 1, -1.5f, "每次行动发动概率较低（Rate），百分比", "负向项：节奏成本，按实际发动频率抵减技能总价值", false);
            config[150] = new SkillTemplateComponentConfig(150, "光环生效比例", "数值", new string[]{"光环"}, "Strength2:auraRate=0.4,0.45,0.5,0.55,0.6", 2, 1.5f, "光环数值的生效比例（auraRate）", "与光环属性值一起计量，避免只看面板值", false);
            config[160] = new SkillTemplateComponentConfig(160, "光环自身成长", "数值", new string[]{"光环"}, "Strength2:selfGrow=2,3,4,5,6", 2, 1.5f, "光环持有者自身的额外成长（selfGrow）", "按成长持续回合折算", false);
            config[1000] = new SkillTemplateComponentConfig(1000, "减速大", "状态效果", new string[]{"单体伤害大","单体伤害中","单体伤害小"}, "Buff:缓:slow=0.3,0.38,0.45,0.52,0.6;BuffTime=3", 2, 1.5f, "降低目标移动与攻速（缓）（强度大）", "缓同时降低移速与攻速，按复合效果核算", false);
            config[1001] = new SkillTemplateComponentConfig(1001, "减速中", "状态效果", new string[]{"单体伤害大","单体伤害中","单体伤害小"}, "Buff:缓:slow=0.2,0.25,0.3,0.35,0.4;BuffTime=3", 1, 1f, "降低目标移动与攻速（缓）（强度中）", "缓同时降低移速与攻速，按复合效果核算", false);
            config[1002] = new SkillTemplateComponentConfig(1002, "减速小", "状态效果", new string[]{"单体伤害大","单体伤害中","单体伤害小"}, "Buff:缓:slow=0.12,0.15,0.18,0.21,0.24;BuffTime=3", 1, 0.5f, "降低目标移动与攻速（缓）（强度小）", "缓同时降低移速与攻速，按复合效果核算", false);
            config[1010] = new SkillTemplateComponentConfig(1010, "减防大", "状态效果", new string[]{"单体伤害大","单体伤害中","单体伤害小"}, "Buff:破:armorDown=22,30,38,45,60;BuffTime=4", 2, 1.5f, "降低目标护甲（破）（强度大）", "按效果强度、持续时间、命中对象数综合估值", false);
            config[1011] = new SkillTemplateComponentConfig(1011, "减防中", "状态效果", new string[]{"单体伤害大","单体伤害中","单体伤害小"}, "Buff:破:armorDown=15,20,25,30,40;BuffTime=4", 1, 1f, "降低目标护甲（破）（强度中）", "按效果强度、持续时间、命中对象数综合估值", false);
            config[1012] = new SkillTemplateComponentConfig(1012, "减防小", "状态效果", new string[]{"单体伤害大","单体伤害中","单体伤害小"}, "Buff:破:armorDown=9,12,15,18,24;BuffTime=4", 1, 0.5f, "降低目标护甲（破）（强度小）", "按效果强度、持续时间、命中对象数综合估值", false);
            config[1020] = new SkillTemplateComponentConfig(1020, "减攻大", "状态效果", new string[]{"单体伤害大","单体伤害中","单体伤害小"}, "Buff:慑:atkDown=22,30,38,45,60;BuffTime=4", 2, 2f, "降低目标攻击（慑）（强度大）", "按效果强度、持续时间、命中对象数综合估值", false);
            config[1021] = new SkillTemplateComponentConfig(1021, "减攻中", "状态效果", new string[]{"单体伤害大","单体伤害中","单体伤害小"}, "Buff:慑:atkDown=15,20,25,30,40;BuffTime=4", 2, 1.5f, "降低目标攻击（慑）（强度中）", "按效果强度、持续时间、命中对象数综合估值", false);
            config[1022] = new SkillTemplateComponentConfig(1022, "减攻小", "状态效果", new string[]{"单体伤害大","单体伤害中","单体伤害小"}, "Buff:慑:atkDown=9,12,15,18,24;BuffTime=4", 1, 1f, "降低目标攻击（慑）（强度小）", "按效果强度、持续时间、命中对象数综合估值", false);
            config[1030] = new SkillTemplateComponentConfig(1030, "减疗大", "状态效果", new string[]{"单体伤害大","单体伤害中","单体伤害小"}, "Buff:疫;Strength2:healDown=0.45,0.52,0.6,0.75,0.9;BuffTime=4", 2, 1.5f, "降低目标受到的治疗（疫）（强度大）", "按效果强度、持续时间、命中对象数综合估值", false);
            config[1031] = new SkillTemplateComponentConfig(1031, "减疗中", "状态效果", new string[]{"单体伤害大","单体伤害中","单体伤害小"}, "Buff:疫;Strength2:healDown=0.3,0.35,0.4,0.5,0.6;BuffTime=4", 1, 1f, "降低目标受到的治疗（疫）（强度中）", "按效果强度、持续时间、命中对象数综合估值", false);
            config[1032] = new SkillTemplateComponentConfig(1032, "减疗小", "状态效果", new string[]{"单体伤害大","单体伤害中","单体伤害小"}, "Buff:疫;Strength2:healDown=0.18,0.21,0.24,0.3,0.36;BuffTime=4", 1, 0.5f, "降低目标受到的治疗（疫）（强度小）", "按效果强度、持续时间、命中对象数综合估值", false);
            config[1040] = new SkillTemplateComponentConfig(1040, "增伤大", "状态效果", new string[]{"单体伤害大","单体伤害中","单体伤害小"}, "Buff:伤:damagedUp=0.15,0.22,0.3,0.38,0.45;BuffTime=4", 2, 2f, "使目标受到的伤害提升（伤）（强度大）", "按效果强度、持续时间、命中对象数综合估值", false);
            config[1041] = new SkillTemplateComponentConfig(1041, "增伤中", "状态效果", new string[]{"单体伤害大","单体伤害中","单体伤害小"}, "Buff:伤:damagedUp=0.1,0.15,0.2,0.25,0.3;BuffTime=4", 2, 1.5f, "使目标受到的伤害提升（伤）（强度中）", "按效果强度、持续时间、命中对象数综合估值", false);
            config[1042] = new SkillTemplateComponentConfig(1042, "增伤小", "状态效果", new string[]{"单体伤害大","单体伤害中","单体伤害小"}, "Buff:伤:damagedUp=0.06,0.09,0.12,0.15,0.18;BuffTime=4", 1, 1f, "使目标受到的伤害提升（伤）（强度小）", "按效果强度、持续时间、命中对象数综合估值", false);
            config[1050] = new SkillTemplateComponentConfig(1050, "眩晕大", "状态效果", new string[]{"单体伤害大","单体伤害中","单体伤害小"}, "BuffTime=0.9,1.2,2,2,2.4;Buff:乱", 4, 4.5f, "无法行动（乱）（强度大）", "控制按覆盖量计价：强度、时长、命中数、施放频率", false);
            config[1051] = new SkillTemplateComponentConfig(1051, "眩晕中", "状态效果", new string[]{"单体伤害大","单体伤害中","单体伤害小"}, "BuffTime=0.6,0.8,1,1.3,1.6;Buff:乱", 3, 3f, "无法行动（乱）（强度中）", "控制按覆盖量计价：强度、时长、命中数、施放频率", false);
            config[1052] = new SkillTemplateComponentConfig(1052, "眩晕小", "状态效果", new string[]{"单体伤害大","单体伤害中","单体伤害小"}, "BuffTime=0.4,0.5,1,0.8,1;Buff:乱", 2, 2f, "无法行动（乱）（强度小）", "控制按覆盖量计价：强度、时长、命中数、施放频率", false);
            config[1060] = new SkillTemplateComponentConfig(1060, "定身大", "状态效果", new string[]{"单体伤害大","单体伤害中","单体伤害小"}, "BuffTime=2,2,2.4,3,3.8;Buff:停", 3, 3f, "无法移动（停）（强度大）", "控制按覆盖量计价：强度、时长、命中数、施放频率", false);
            config[1061] = new SkillTemplateComponentConfig(1061, "定身中", "状态效果", new string[]{"单体伤害大","单体伤害中","单体伤害小"}, "BuffTime=1,1.3,1.6,2,2.5;Buff:停", 2, 2f, "无法移动（停）（强度中）", "控制按覆盖量计价：强度、时长、命中数、施放频率", false);
            config[1062] = new SkillTemplateComponentConfig(1062, "定身小", "状态效果", new string[]{"单体伤害大","单体伤害中","单体伤害小"}, "BuffTime=1,0.8,1,1,1.5;Buff:停", 1, 1f, "无法移动（停）（强度小）", "控制按覆盖量计价：强度、时长、命中数、施放频率", false);
            config[1070] = new SkillTemplateComponentConfig(1070, "混乱大", "状态效果", new string[]{"单体伤害大","单体伤害中","单体伤害小"}, "BuffTime=1.2,2,1.8,2.2,2.7;Buff:惑", 4, 4f, "不受控制，攻击最近的友方单位（惑）（强度大）", "控制按覆盖量计价：强度、时长、命中数、施放频率", false);
            config[1071] = new SkillTemplateComponentConfig(1071, "混乱中", "状态效果", new string[]{"单体伤害大","单体伤害中","单体伤害小"}, "BuffTime=0.8,1,1.2,1.5,1.8;Buff:惑", 3, 2.5f, "不受控制，攻击最近的友方单位（惑）（强度中）", "控制按覆盖量计价：强度、时长、命中数、施放频率", false);
            config[1072] = new SkillTemplateComponentConfig(1072, "混乱小", "状态效果", new string[]{"单体伤害大","单体伤害中","单体伤害小"}, "BuffTime=0.5,1,0.7,0.9,1.1;Buff:惑", 2, 1.5f, "不受控制，攻击最近的友方单位（惑）（强度小）", "控制按覆盖量计价：强度、时长、命中数、施放频率", false);
            config[1080] = new SkillTemplateComponentConfig(1080, "嘲讽大", "状态效果", new string[]{"单体伤害大","单体伤害中","单体伤害小","群体伤害近距离","群体伤害自身中心"}, "BuffTime=2,2,2.4,3,3.8;Buff:嘲", 4, 4f, "强制目标攻击施法者（嘲）（强度大）", "控制按覆盖量计价：强度、时长、命中数、施放频率", false);
            config[1081] = new SkillTemplateComponentConfig(1081, "嘲讽中", "状态效果", new string[]{"单体伤害大","单体伤害中","单体伤害小","群体伤害近距离","群体伤害自身中心"}, "BuffTime=1,1.3,1.6,2,2.5;Buff:嘲", 3, 2.5f, "强制目标攻击施法者（嘲）（强度中）", "控制按覆盖量计价：强度、时长、命中数、施放频率", false);
            config[1082] = new SkillTemplateComponentConfig(1082, "嘲讽小", "状态效果", new string[]{"单体伤害大","单体伤害中","单体伤害小","群体伤害近距离","群体伤害自身中心"}, "BuffTime=1,0.8,1,1,1.5;Buff:嘲", 2, 1.5f, "强制目标攻击施法者（嘲）（强度小）", "控制按覆盖量计价：强度、时长、命中数、施放频率", false);
            config[2000] = new SkillTemplateComponentConfig(2000, "属性增益大", "状态效果", new string[]{"单体治疗","范围治疗","自身强化","光环","单体友方增益","群体友方增益","兵种增益"}, "None", 3, 3f, "攻速/移速/攻击/护甲/法强等属性 Buff（强度大）", "按属性类型、强度、持续时间、覆盖人数综合估值", false);
            config[2001] = new SkillTemplateComponentConfig(2001, "属性增益中", "状态效果", new string[]{"单体治疗","范围治疗","自身强化","光环","单体友方增益","群体友方增益","兵种增益"}, "None", 2, 2f, "攻速/移速/攻击/护甲/法强等属性 Buff（强度中）", "按属性类型、强度、持续时间、覆盖人数综合估值", false);
            config[2002] = new SkillTemplateComponentConfig(2002, "属性增益小", "状态效果", new string[]{"单体治疗","范围治疗","自身强化","光环","单体友方增益","群体友方增益","兵种增益"}, "None", 1, 1f, "攻速/移速/攻击/护甲/法强等属性 Buff（强度小）", "按属性类型、强度、持续时间、覆盖人数综合估值", false);
            config[2010] = new SkillTemplateComponentConfig(2010, "加攻大", "状态效果", new string[]{"光环","自身强化","单体治疗","范围治疗","单体友方增益","群体友方增益","兵种增益"}, "Buff:攻:atk=15,22,38,60,90;BuffTime=5", 3, 3f, "提升攻击力（强度大）", "按属性类型、强度、持续时间、覆盖人数综合估值", false);
            config[2011] = new SkillTemplateComponentConfig(2011, "加攻中", "状态效果", new string[]{"光环","自身强化","单体治疗","范围治疗","单体友方增益","群体友方增益","兵种增益"}, "Buff:攻:atk=10,15,25,40,60;BuffTime=5", 2, 2f, "提升攻击力（强度中）", "按属性类型、强度、持续时间、覆盖人数综合估值", false);
            config[2012] = new SkillTemplateComponentConfig(2012, "加攻小", "状态效果", new string[]{"光环","自身强化","单体治疗","范围治疗","单体友方增益","群体友方增益","兵种增益"}, "Buff:攻:atk=6,9,15,24,36;BuffTime=5", 1, 1f, "提升攻击力（强度小）", "按属性类型、强度、持续时间、覆盖人数综合估值", false);
            config[2020] = new SkillTemplateComponentConfig(2020, "加护甲大", "状态效果", new string[]{"光环","自身强化","单体治疗","范围治疗","单体友方增益","群体友方增益","兵种增益"}, "Buff:甲:armor=8,12,18,27,39;BuffTime=5", 2, 1.5f, "提升护甲（强度大）", "按属性类型、强度、持续时间、覆盖人数综合估值", false);
            config[2021] = new SkillTemplateComponentConfig(2021, "加护甲中", "状态效果", new string[]{"光环","自身强化","单体治疗","范围治疗","单体友方增益","群体友方增益","兵种增益"}, "Buff:甲:armor=5,8,12,18,26;BuffTime=5", 1, 1f, "提升护甲（强度中）", "按属性类型、强度、持续时间、覆盖人数综合估值", false);
            config[2022] = new SkillTemplateComponentConfig(2022, "加护甲小", "状态效果", new string[]{"光环","自身强化","单体治疗","范围治疗","单体友方增益","群体友方增益","兵种增益"}, "Buff:甲:armor=3,5,7,11,16;BuffTime=5", 1, 0.5f, "提升护甲（强度小）", "按属性类型、强度、持续时间、覆盖人数综合估值", false);
            config[2030] = new SkillTemplateComponentConfig(2030, "加法强大", "状态效果", new string[]{"光环","自身强化","单体治疗","范围治疗","单体友方增益","群体友方增益","兵种增益"}, "Buff:术:ap=15,22,38,60,90;BuffTime=5", 3, 3f, "提升法术强度（强度大）", "按属性类型、强度、持续时间、覆盖人数综合估值", false);
            config[2031] = new SkillTemplateComponentConfig(2031, "加法强中", "状态效果", new string[]{"光环","自身强化","单体治疗","范围治疗","单体友方增益","群体友方增益","兵种增益"}, "Buff:术:ap=10,15,25,40,60;BuffTime=5", 2, 2f, "提升法术强度（强度中）", "按属性类型、强度、持续时间、覆盖人数综合估值", false);
            config[2032] = new SkillTemplateComponentConfig(2032, "加法强小", "状态效果", new string[]{"光环","自身强化","单体治疗","范围治疗","单体友方增益","群体友方增益","兵种增益"}, "Buff:术:ap=6,9,15,24,36;BuffTime=5", 1, 1f, "提升法术强度（强度小）", "按属性类型、强度、持续时间、覆盖人数综合估值", false);
            config[2040] = new SkillTemplateComponentConfig(2040, "加魔抗大", "状态效果", new string[]{"光环","自身强化","单体治疗","范围治疗","单体友方增益","群体友方增益","兵种增益"}, "Buff:抗:magicRes=8,12,18,27,39;BuffTime=5", 2, 1.5f, "提升魔法抗性（强度大）", "按属性类型、强度、持续时间、覆盖人数综合估值", false);
            config[2041] = new SkillTemplateComponentConfig(2041, "加魔抗中", "状态效果", new string[]{"光环","自身强化","单体治疗","范围治疗","单体友方增益","群体友方增益","兵种增益"}, "Buff:抗:magicRes=5,8,12,18,26;BuffTime=5", 1, 1f, "提升魔法抗性（强度中）", "按属性类型、强度、持续时间、覆盖人数综合估值", false);
            config[2042] = new SkillTemplateComponentConfig(2042, "加魔抗小", "状态效果", new string[]{"光环","自身强化","单体治疗","范围治疗","单体友方增益","群体友方增益","兵种增益"}, "Buff:抗:magicRes=3,5,7,11,16;BuffTime=5", 1, 0.5f, "提升魔法抗性（强度小）", "按属性类型、强度、持续时间、覆盖人数综合估值", false);
            config[2050] = new SkillTemplateComponentConfig(2050, "加攻速大", "状态效果", new string[]{"光环","自身强化","单体治疗","范围治疗","单体友方增益","群体友方增益","兵种增益"}, "Buff:快:atkSpeed=0.08,0.12,0.18,0.27,0.39;BuffTime=5", 2, 1.5f, "提升攻击速度（强度大）", "按属性类型、强度、持续时间、覆盖人数综合估值", false);
            config[2051] = new SkillTemplateComponentConfig(2051, "加攻速中", "状态效果", new string[]{"光环","自身强化","单体治疗","范围治疗","单体友方增益","群体友方增益","兵种增益"}, "Buff:快:atkSpeed=0.05,0.08,0.12,0.18,0.26;BuffTime=5", 1, 1f, "提升攻击速度（强度中）", "按属性类型、强度、持续时间、覆盖人数综合估值", false);
            config[2052] = new SkillTemplateComponentConfig(2052, "加攻速小", "状态效果", new string[]{"光环","自身强化","单体治疗","范围治疗","单体友方增益","群体友方增益","兵种增益"}, "Buff:快:atkSpeed=0.03,0.05,0.07,0.11,0.16;BuffTime=5", 1, 0.5f, "提升攻击速度（强度小）", "按属性类型、强度、持续时间、覆盖人数综合估值", false);
            config[2060] = new SkillTemplateComponentConfig(2060, "加移速大", "状态效果", new string[]{"光环","自身强化","单体治疗","范围治疗","单体友方增益","群体友方增益","兵种增益"}, "Buff:速:moveSpeed=0.08,0.12,0.18,0.27,0.39;BuffTime=5", 2, 1.5f, "提升移动速度（强度大）", "按属性类型、强度、持续时间、覆盖人数综合估值", false);
            config[2061] = new SkillTemplateComponentConfig(2061, "加移速中", "状态效果", new string[]{"光环","自身强化","单体治疗","范围治疗","单体友方增益","群体友方增益","兵种增益"}, "Buff:速:moveSpeed=0.05,0.08,0.12,0.18,0.26;BuffTime=5", 1, 1f, "提升移动速度（强度中）", "按属性类型、强度、持续时间、覆盖人数综合估值", false);
            config[2062] = new SkillTemplateComponentConfig(2062, "加移速小", "状态效果", new string[]{"光环","自身强化","单体治疗","范围治疗","单体友方增益","群体友方增益","兵种增益"}, "Buff:速:moveSpeed=0.03,0.05,0.07,0.11,0.16;BuffTime=5", 1, 0.5f, "提升移动速度（强度小）", "按属性类型、强度、持续时间、覆盖人数综合估值", false);
            config[2070] = new SkillTemplateComponentConfig(2070, "加攻速率大", "状态效果", new string[]{"单体友方增益","群体友方增益","兵种增益"}, "Buff:快:atkSpeed=0.08,0.12,0.18,0.27,0.39;BuffTime=5", 2, 1.5f, "按百分比提升攻击速度（强度大）", "与点数型加攻速区分，按实际出手次数提升估值", false);
            config[2071] = new SkillTemplateComponentConfig(2071, "加攻速率中", "状态效果", new string[]{"单体友方增益","群体友方增益","兵种增益"}, "Buff:快:atkSpeed=0.05,0.08,0.12,0.18,0.26;BuffTime=5", 1, 1f, "按百分比提升攻击速度（强度中）", "与点数型加攻速区分，按实际出手次数提升估值", false);
            config[2072] = new SkillTemplateComponentConfig(2072, "加攻速率小", "状态效果", new string[]{"单体友方增益","群体友方增益","兵种增益"}, "Buff:快:atkSpeed=0.03,0.05,0.07,0.11,0.16;BuffTime=5", 1, 0.5f, "按百分比提升攻击速度（强度小）", "与点数型加攻速区分，按实际出手次数提升估值", false);
            config[2080] = new SkillTemplateComponentConfig(2080, "加生命回复大", "状态效果", new string[]{"单体友方增益","群体友方增益","兵种增益"}, "Buff:血:hpRegen=8,12,18,27,39;BuffTime=5", 2, 1.5f, "提升每秒生命回复（强度大）", "按战斗时长折算总量后估值", false);
            config[2081] = new SkillTemplateComponentConfig(2081, "加生命回复中", "状态效果", new string[]{"单体友方增益","群体友方增益","兵种增益"}, "Buff:血:hpRegen=5,8,12,18,26;BuffTime=5", 1, 1f, "提升每秒生命回复（强度中）", "按战斗时长折算总量后估值", false);
            config[2082] = new SkillTemplateComponentConfig(2082, "加生命回复小", "状态效果", new string[]{"单体友方增益","群体友方增益","兵种增益"}, "Buff:血:hpRegen=3,5,7,11,16;BuffTime=5", 1, 0.5f, "提升每秒生命回复（强度小）", "按战斗时长折算总量后估值", false);
            config[2090] = new SkillTemplateComponentConfig(2090, "自身增伤大", "状态效果", new string[]{"自身强化","单体友方增益","群体友方增益"}, "Buff:重:damageUp=0.15,0.22,0.3,0.38,0.45;BuffTime=5", 3, 3f, "提升自身造成的伤害（重）（强度大）", "与使目标受伤提升区分，按持续时间内输出增量估值", false);
            config[2091] = new SkillTemplateComponentConfig(2091, "自身增伤中", "状态效果", new string[]{"自身强化","单体友方增益","群体友方增益"}, "Buff:重:damageUp=0.1,0.15,0.2,0.25,0.3;BuffTime=5", 2, 2f, "提升自身造成的伤害（重）（强度中）", "与使目标受伤提升区分，按持续时间内输出增量估值", false);
            config[2092] = new SkillTemplateComponentConfig(2092, "自身增伤小", "状态效果", new string[]{"自身强化","单体友方增益","群体友方增益"}, "Buff:重:damageUp=0.06,0.09,0.12,0.15,0.18;BuffTime=5", 1, 1f, "提升自身造成的伤害（重）（强度小）", "与使目标受伤提升区分，按持续时间内输出增量估值", false);
            config[2100] = new SkillTemplateComponentConfig(2100, "护盾大", "状态效果", new string[]{"单体治疗","范围治疗","自身强化","单体友方增益","群体友方增益"}, "Buff:盾;Strength2:shield=90,135,225,360,540;BuffTime=5", 4, 4.5f, "吸收伤害的护盾（盾）（强度大）", "护盾与治疗量分别估值", false);
            config[2101] = new SkillTemplateComponentConfig(2101, "护盾中", "状态效果", new string[]{"单体治疗","范围治疗","自身强化","单体友方增益","群体友方增益"}, "Buff:盾;Strength2:shield=60,90,150,240,360;BuffTime=5", 3, 3f, "吸收伤害的护盾（盾）（强度中）", "护盾与治疗量分别估值", false);
            config[2102] = new SkillTemplateComponentConfig(2102, "护盾小", "状态效果", new string[]{"单体治疗","范围治疗","自身强化","单体友方增益","群体友方增益"}, "Buff:盾;Strength2:shield=36,54,90,144,216;BuffTime=5", 2, 2f, "吸收伤害的护盾（盾）（强度小）", "护盾与治疗量分别估值", false);
            config[2110] = new SkillTemplateComponentConfig(2110, "减伤大", "状态效果", new string[]{"自身强化","单体友方增益","群体友方增益"}, "Buff:硬:shieldReduce=0.15,0.22,0.3,0.38,0.45;BuffTime=5", 4, 4.5f, "按百分比降低受到的伤害（硬）（强度大）", "按持续时间内吸收的伤害量估值", false);
            config[2111] = new SkillTemplateComponentConfig(2111, "减伤中", "状态效果", new string[]{"自身强化","单体友方增益","群体友方增益"}, "Buff:硬:shieldReduce=0.1,0.15,0.2,0.25,0.3;BuffTime=5", 3, 3f, "按百分比降低受到的伤害（硬）（强度中）", "按持续时间内吸收的伤害量估值", false);
            config[2112] = new SkillTemplateComponentConfig(2112, "减伤小", "状态效果", new string[]{"自身强化","单体友方增益","群体友方增益"}, "Buff:硬:shieldReduce=0.06,0.09,0.12,0.15,0.18;BuffTime=5", 2, 2f, "按百分比降低受到的伤害（硬）（强度小）", "按持续时间内吸收的伤害量估值", false);
            config[2120] = new SkillTemplateComponentConfig(2120, "暴击大", "状态效果", new string[]{"自身强化","单体友方增益","群体友方增益"}, "Buff:暴:critRate=0.08,0.12,0.18,0.27,0.39;BuffTime=5", 3, 3f, "提升暴击率（暴）（强度大）", "按期望输出增量估值", false);
            config[2121] = new SkillTemplateComponentConfig(2121, "暴击中", "状态效果", new string[]{"自身强化","单体友方增益","群体友方增益"}, "Buff:暴:critRate=0.05,0.08,0.12,0.18,0.26;BuffTime=5", 2, 2f, "提升暴击率（暴）（强度中）", "按期望输出增量估值", false);
            config[2122] = new SkillTemplateComponentConfig(2122, "暴击小", "状态效果", new string[]{"自身强化","单体友方增益","群体友方增益"}, "Buff:暴:critRate=0.03,0.05,0.07,0.11,0.16;BuffTime=5", 1, 1f, "提升暴击率（暴）（强度小）", "按期望输出增量估值", false);
            config[3000] = new SkillTemplateComponentConfig(3000, "低血筛选", "目标筛选", new string[]{"单体治疗","范围治疗","单体友方增益","群体友方增益"}, "TargetType:lowestHp", 1, 1f, "优先选择最低血量目标", "选择性越强未必更贵；看是否提高击杀或救援效率", false);
            config[3010] = new SkillTemplateComponentConfig(3010, "后排筛选", "目标筛选", new string[]{"单体治疗","范围治疗","单体友方增益","群体友方增益"}, "None", 1, 0.5f, "优先选择后排目标", "选择性越强未必更贵；看是否提高击杀或救援效率", false);
            config[3020] = new SkillTemplateComponentConfig(3020, "英雄限定", "目标筛选", new string[]{"单体伤害大","单体伤害中","单体伤害小","群体伤害近距离","群体伤害自身中心"}, "TargetType:heroOnly", 2, 1.5f, "仅对英雄/特定单位生效", "需看筛选是否提高击杀或覆盖效果", false);
            config[3030] = new SkillTemplateComponentConfig(3030, "友军筛选", "目标筛选", new string[]{"单体治疗","范围治疗","单体友方增益","群体友方增益"}, "None", 1, 0.5f, "仅对特定类型友军生效", "需看筛选是否提高救援或覆盖效果", false);
            config[3040] = new SkillTemplateComponentConfig(3040, "最高血量", "目标筛选", new string[]{"单体友方增益","群体友方增益"}, "TargetType:highestHp", 2, 1f, "优先选择生命百分比最高的目标", "选择性越强未必更贵；看是否提高救援或承伤效率", false);
            config[3050] = new SkillTemplateComponentConfig(3050, "最高法强", "目标筛选", new string[]{"单体友方增益","群体友方增益"}, "TargetType:highestAp", 2, 1f, "优先选择法术强度最高的目标", "选择性越强未必更贵；看是否提高增益收益", false);
            config[3060] = new SkillTemplateComponentConfig(3060, "远程优先", "目标筛选", new string[]{"单体友方增益","群体友方增益"}, "TargetType:rangedOnly", 1, 0.5f, "优先选择远程单位，无匹配时回退其余单位", "选择性本身不增值，看是否契合阵容", false);
            config[3070] = new SkillTemplateComponentConfig(3070, "近战优先", "目标筛选", new string[]{"单体友方增益","群体友方增益"}, "TargetType:meleeOnly", 1, 0.5f, "优先选择近战单位，无匹配时回退其余单位", "选择性本身不增值，看是否契合阵容", false);
            config[3080] = new SkillTemplateComponentConfig(3080, "排除自身", "目标筛选", new string[]{"单体友方增益","群体友方增益"}, "TargetType:excludeSelf", 1, 0.5f, "选人时排除施法者自身", "结构性开关，本身不增值", false);
            config[3090] = new SkillTemplateComponentConfig(3090, "施法者必含", "目标筛选", new string[]{"单体友方增益","群体友方增益"}, "TargetType:selfAlways", 1, 0.5f, "选人时施法者必然包含在内", "结构性开关，本身不增值", false);
            config[3100] = new SkillTemplateComponentConfig(3100, "满血跳过", "目标筛选", new string[]{"单体友方增益","群体友方增益"}, "TargetType:fullHpSkip", 1, 0.5f, "跳过满血目标，避免溢出浪费", "结构性开关，本身不增值", false);
            config[3110] = new SkillTemplateComponentConfig(3110, "全图", "目标筛选", new string[]{"单体友方增益","群体友方增益"}, "TargetType:wholeMap", 2, 1.5f, "全图选人，不受射程限制", "大幅扩大可触达范围，按覆盖提升估值", false);
            config[3120] = new SkillTemplateComponentConfig(3120, "嘲讽周围", "目标筛选", new string[]{"单体友方增益","群体友方增益"}, "TargetType:tauntAroundTarget", 2, 1.5f, "对目标周围敌人附加嘲讽", "与嘲讽组件配合，按控制覆盖量估值", false);
            config[3130] = new SkillTemplateComponentConfig(3130, "不按首段去重", "目标筛选", new string[]{"单体友方增益","群体友方增益"}, "TargetType:noDedupe", 1, 0.5f, "多次结算时不按首段效果去重", "结构性开关，本身不增值", false);
            config[3140] = new SkillTemplateComponentConfig(3140, "近远分段", "目标筛选", new string[]{"单体友方增益","群体友方增益"}, "TargetType:splitByRange", 1, 0.5f, "按近战/远程分段施加不同效果", "结构性开关，本身不增值", false);
            config[9000] = new SkillTemplateComponentConfig(9000, "斩杀", "特殊机制", new string[]{"单体伤害大","单体伤害中","单体伤害小","群体伤害近距离","群体伤害自身中心"}, "TargetType:execute;Strength2:execRate=1.5,1.6,1.7,1.8,2;Strength2:execHp=0.3", 4, 5f, "按血量阈值直接击杀或额外倍率", "特殊机制，人工评审，不套用普通伤害价格表", true);
            config[9010] = new SkillTemplateComponentConfig(9010, "驱散", "特殊机制", new string[]{"单体伤害大","单体伤害中","单体伤害小","群体伤害近距离","单体治疗","范围治疗","群体伤害自身中心"}, "TargetType:dispel", 3, 3f, "移除目标增益/减益", "特殊机制，人工评审", true);
            config[9020] = new SkillTemplateComponentConfig(9020, "破盾", "特殊机制", new string[]{"单体伤害大","单体伤害中","单体伤害小","群体伤害近距离","群体伤害自身中心"}, "TargetType:antiShield;TargetType:armorIgnore;Strength2:antiShieldRate=0.5,0.7,0.9,1.2,1.5", 2, 2f, "绕过或击破护盾", "特殊机制，人工评审", true);
            config[9030] = new SkillTemplateComponentConfig(9030, "位移", "特殊机制", new string[]{"单体伤害大","单体伤害中","单体伤害小","群体伤害近距离","群体伤害自身中心"}, "Strength2:pushMin=10;Strength2:pushMax=20", 2, 1.5f, "击退/拉近/换位", "特殊机制，人工评审", true);
            config[9050] = new SkillTemplateComponentConfig(9050, "永久成长", "特殊机制", new string[]{"自身强化"}, "TargetType:permanent;Strength2:selfAtkSpeed=0.08,0.1,0.12,0.14,0.17", 3, 3f, "永久属性成长", "特殊机制，人工评审", true);
            config[9060] = new SkillTemplateComponentConfig(9060, "复制", "特殊机制", new string[]{"自身强化"}, "Strength2:inheritRate=0.35,0.45,0.55,0.65,0.8", 4, 4f, "复制效果或单位", "特殊机制，人工评审", true);
            config[9070] = new SkillTemplateComponentConfig(9070, "溅射", "特殊机制", new string[]{"单体伤害大","单体伤害中","单体伤害小"}, "Strength2:splashRate=0.5,0.6,0.7,0.8,1", 2, 2f, "主目标附近溅射伤害", "主目标与溅射目标伤害分别计量，溅射不是纯单体模板", false);
            config[9071] = new SkillTemplateComponentConfig(9071, "穿透", "特殊机制", new string[]{"群体伤害近距离","群体伤害自身中心"}, "Strength2:pierceRate=0.3,0.45,0.7,1,1.5", 2, 2f, "直线穿透多个目标", "按实际穿透命中数估值", false);
            config[9072] = new SkillTemplateComponentConfig(9072, "弹道", "特殊机制", new string[]{"群体伤害近距离","群体伤害自身中心"}, "TargetType:missile;Strength2:missileRate=0.5,0.6,0.7,0.8,1", 2, 2f, "逐个命中的飞行物", "按飞弹倍率与命中数共同估值", false);
            config[9080] = new SkillTemplateComponentConfig(9080, "概率双击", "特殊机制", new string[]{"单体伤害大","单体伤害中","单体伤害小"}, "TargetType:doubleChance;Strength2:doubleChance=0.3;Strength2:doubleRate=2", 3, 3f, "有概率触发额外一次结算（doubleChance/doubleRate）", "按期望额外伤害估值", false);
            config[9081] = new SkillTemplateComponentConfig(9081, "自损", "特殊机制", new string[]{"单体伤害大","单体伤害中","单体伤害小"}, "TargetType:selfHurt;Strength2:selfHurt=0.2", 1, -1.5f, "施放时消耗自身生命（selfHurt），属于代价项", "负向项，抵减技能总价值", false);
            config[9082] = new SkillTemplateComponentConfig(9082, "燃烧法力", "特殊机制", new string[]{"单体伤害大","单体伤害中","单体伤害小","群体伤害近距离","群体伤害自身中心"}, "TargetType:mpBurn;Strength2:mpBurn=20,25,30,35,40", 2, 2f, "燃烧目标法力（mpBurn），压制技能循环", "按削弱的法力与技能频率估值", false);
            config[9083] = new SkillTemplateComponentConfig(9083, "按护盾加成伤害", "特殊机制", new string[]{"单体伤害大","单体伤害中","单体伤害小"}, "TargetType:shieldBonus;Strength2:shieldBonusRate=1.5,1.8,2,2.2,2.5", 3, 3f, "按目标护盾量追加伤害（shieldBonusRate）", "对护盾阵容收益高，按期望追伤估值", false);
            config[9084] = new SkillTemplateComponentConfig(9084, "兵种克制", "特殊机制", new string[]{"单体伤害大","单体伤害中","单体伤害小"}, "TargetType:vsJob;Strength2:vsJob=盾|士;Strength2:vsJobRate=2", 3, 3f, "对特定兵种额外倍率（vsJob）", "按阵容命中率折算期望收益", false);
            config[9085] = new SkillTemplateComponentConfig(9085, "爆炸", "特殊机制", new string[]{"单体伤害大","单体伤害中","单体伤害小"}, "Strength2:explodeRate=0.5", 2, 2f, "命中后对周围溅射伤害（explodeRate）", "与溅射类似，按额外命中数估值", false);
            config[9086] = new SkillTemplateComponentConfig(9086, "伤害反射", "特殊机制", new string[]{"自身强化"}, "Strength2:reflect=0.12,0.16,0.2,0.25,0.3", 3, 3f, "受击时反弹伤害（reflect）", "按承受伤害与反弹比例折算", false);
            config[9087] = new SkillTemplateComponentConfig(9087, "冲锋位移", "特殊机制", new string[]{"群体伤害近距离","群体伤害自身中心"}, "Strength2:moveDist=40", 2, 1.5f, "向目标方向冲锋并沿途结算伤害", "移动形态本身不增值，看实际可命中数", false);

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
