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
            {"Category", new FieldMetaInfo("类别(数值、射程、目标数、范围档、状态效果、持续、目标筛选、特殊机制、施放节奏、光环)", "string", 0)},
            {"Templates", new FieldMetaInfo("适用模板名列表(空=通用)", "string[]", 0)},
            {"Values", new FieldMetaInfo("组件数值(Lv1~5逗号分隔;按组件语义填伤害/治疗/射程/目标数/范围/强度/持续时间等)", "string", 150)},
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
        ///类别(数值、射程、目标数、范围档、状态效果、持续、目标筛选、特殊机制、施放节奏、光环)
        /// </summary>
        public string Category;
        /// <summary>
        ///适用模板名列表（空=通用）
        /// </summary>
        public string[] Templates;
        /// <summary>
        ///组件数值(Lv1~5逗号分隔;按组件语义填伤害/治疗/射程/目标数/范围/强度/持续时间等)
        /// </summary>
        public string Values;
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


        public SkillTemplateComponentConfig(int Id, string Name, string Category, string[] Templates, string Values, int Quality, float Score, string Des, string PriceNote, bool IsSpecial)
        {
            this.Id = Id;
            this.Name = Name;
            this.Category = Category;
            this.Templates = Templates;
            this.Values = Values;
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
            config[10] = new SkillTemplateComponentConfig(10, "伤害倍率", "数值", new string[]{"单体伤害大","单体伤害中","单体伤害小","群体伤害近距离","单体治疗","范围治疗","持续区域","自身强化","群体伤害自身中心"}, "10,15,25,40,60", 2, 2f, "按属性成长的伤害倍率", "与固定基值分开计量，避免重复计价", false);
            config[20] = new SkillTemplateComponentConfig(20, "治疗量", "数值", new string[]{"单体治疗","范围治疗"}, "30,45,75,120,180", 2, 2f, "治疗/回复量档位", "与伤害分别标定", false);
            config[30] = new SkillTemplateComponentConfig(30, "施法射程", "射程", new string[]{"单体伤害大","单体伤害中","单体伤害小","群体伤害近距离","单体治疗","范围治疗","持续区域","自身强化","光环"}, "30,40,50,65,80", 1, 1f, "增加施法距离 Range，决定目标是否可被触达", "与攻击射程、AoE 半径区分，按可触达目标和触发成功率估值", false);
            config[40] = new SkillTemplateComponentConfig(40, "最大目标数", "目标数", new string[]{"单体伤害大","单体伤害中","单体伤害小","群体伤害近距离","单体治疗","范围治疗","持续区域","群体伤害自身中心"}, "2,3,3,4,5", 2, 1.5f, "最多命中 N 个目标", "有限目标数与范围内全部目标分开；TargetCount<=0 在不同脚本中可能表示不限，不可按零成本处理", false);
            config[50] = new SkillTemplateComponentConfig(50, "中范围", "范围档", new string[]{"群体伤害近距离","范围治疗","持续区域","群体伤害自身中心"}, "30,30,30,30,30", 1, 1f, "中半径 AoE 范围档（模板默认小范围=20，本组件改为30）", "按实际预期命中数估值，不按半径线性估价", false);
            config[51] = new SkillTemplateComponentConfig(51, "大范围", "范围档", new string[]{"群体伤害近距离","群体伤害自身中心"}, "40,40,40,40,40", 2, 2f, "大半径 AoE 范围档（模板默认小范围=20，本组件改为40）", "按实际预期命中数估值，不按半径线性估价", false);
            config[90] = new SkillTemplateComponentConfig(90, "连击", "持续", new string[]{"单体伤害大","单体伤害中","单体伤害小","群体伤害近距离","群体伤害自身中心"}, "2,2,3,3,4", 3, 3f, "同一次施放结算多段", "按预期总段数估价，避免只按单次伤害估值", false);
            config[91] = new SkillTemplateComponentConfig(91, "持续伤害", "持续", new string[]{"单体伤害大","单体伤害中","单体伤害小","群体伤害近距离","持续区域","群体伤害自身中心"}, "20,30,50,80,120", 3, 2.5f, "按周期结算的持续伤害（毒/败）", "按持续结算次数与目标滞留时间估价", false);
            config[92] = new SkillTemplateComponentConfig(92, "持续施法", "持续", new string[]{"群体伤害近距离","持续区域","群体伤害自身中心"}, "3,3,4,4,5", 2, 2f, "施法期间持续结算", "按总结算次数与目标是否持续处于范围估价", false);
            config[93] = new SkillTemplateComponentConfig(93, "持续区域", "持续", new string[]{"持续区域"}, "2,2.5,3,3.5,4", 3, 3f, "创建持续一段时间的区域，周期结算伤害/控制", "按持续结算次数与目标是否持续留在范围估价", false);
            config[100] = new SkillTemplateComponentConfig(100, "低血筛选", "目标筛选", new string[]{"单体治疗","范围治疗"}, "30,35,40,45,50", 1, 1f, "优先选择最低血量目标", "选择性越强未必更贵；看是否提高击杀或救援效率", false);
            config[101] = new SkillTemplateComponentConfig(101, "后排筛选", "目标筛选", new string[]{"单体治疗","范围治疗"}, "50,55,60,65,70", 1, 0.5f, "优先选择后排目标", "选择性越强未必更贵；看是否提高击杀或救援效率", false);
            config[102] = new SkillTemplateComponentConfig(102, "英雄限定", "目标筛选", new string[]{"单体伤害大","单体伤害中","单体伤害小","群体伤害近距离","群体伤害自身中心"}, "10,15,20,25,30", 2, 1.5f, "仅对英雄/特定单位生效", "需看筛选是否提高击杀或覆盖效果", false);
            config[103] = new SkillTemplateComponentConfig(103, "友军筛选", "目标筛选", new string[]{"单体治疗","范围治疗"}, "2,3,4,5,6", 1, 0.5f, "仅对特定类型友军生效", "需看筛选是否提高救援或覆盖效果", false);
            config[120] = new SkillTemplateComponentConfig(120, "MP消耗", "施放节奏", new string[]{}, "7,7,7,7,7", 2, 1.5f, "技能 MP 消耗（MpCost），数值=MP上限", "分值仅作节奏修正参考，用实际发动频率校准，不与单次效果成本混为一项", false);
            config[121] = new SkillTemplateComponentConfig(121, "CD", "施放节奏", new string[]{}, "2,2,2,2,2", 2, 1.5f, "发动冷却（CD），数值=秒", "分值仅作节奏修正参考，用实际发动频率校准", false);
            config[122] = new SkillTemplateComponentConfig(122, "概率", "施放节奏", new string[]{}, "20,30,40,50,60", 1, 1f, "每次行动发动概率（Rate），数值=百分比", "分值仅作节奏修正参考，用实际发动频率校准", false);
            config[123] = new SkillTemplateComponentConfig(123, "触发条件", "施放节奏", new string[]{}, "50,50,55,60,65", 1, 0.5f, "满足条件才触发（TriggerCondition），数值=生命百分比阈值", "分值仅作节奏修正参考，用实际发动频率校准", false);
            config[1000] = new SkillTemplateComponentConfig(1000, "减速", "状态效果", new string[]{"单体伤害大","单体伤害中","单体伤害小"}, "20,25,30,35,40", 1, 1f, "降低目标移动与攻速（缓）", "缓同时降低移速与攻速，按复合效果核算", false);
            config[1001] = new SkillTemplateComponentConfig(1001, "减防", "状态效果", new string[]{"单体伤害大","单体伤害中","单体伤害小"}, "15,20,25,30,40", 1, 1f, "降低目标护甲（破）", "按效果强度、持续时间、命中对象数综合估值", false);
            config[1002] = new SkillTemplateComponentConfig(1002, "减攻", "状态效果", new string[]{"单体伤害大","单体伤害中","单体伤害小"}, "15,20,25,30,40", 2, 1.5f, "降低目标攻击（慑）", "按效果强度、持续时间、命中对象数综合估值", false);
            config[1003] = new SkillTemplateComponentConfig(1003, "减疗", "状态效果", new string[]{"单体伤害大","单体伤害中","单体伤害小"}, "30,35,40,50,60", 1, 1f, "降低目标受到的治疗（疫）", "按效果强度、持续时间、命中对象数综合估值", false);
            config[1004] = new SkillTemplateComponentConfig(1004, "增伤", "状态效果", new string[]{"单体伤害大","单体伤害中","单体伤害小"}, "10,15,20,25,30", 2, 1.5f, "使目标受到的伤害提升（伤）", "按效果强度、持续时间、命中对象数综合估值", false);
            config[1010] = new SkillTemplateComponentConfig(1010, "眩晕", "状态效果", new string[]{"单体伤害大","单体伤害中","单体伤害小"}, "0.6,0.8,1,1.3,1.6", 3, 3f, "无法行动（乱）", "控制按覆盖量计价：强度、时长、命中数、施放频率", false);
            config[1011] = new SkillTemplateComponentConfig(1011, "定身", "状态效果", new string[]{"单体伤害大","单体伤害中","单体伤害小"}, "1,1.3,1.6,2,2.5", 2, 2f, "无法移动（停）", "控制按覆盖量计价：强度、时长、命中数、施放频率", false);
            config[1012] = new SkillTemplateComponentConfig(1012, "混乱", "状态效果", new string[]{"单体伤害大","单体伤害中","单体伤害小"}, "0.8,1,1.2,1.5,1.8", 3, 2.5f, "不受控制，攻击最近的友方单位（惑）", "控制按覆盖量计价：强度、时长、命中数、施放频率", false);
            config[2000] = new SkillTemplateComponentConfig(2000, "属性增益", "状态效果", new string[]{"单体治疗","范围治疗","自身强化","光环"}, "5,8,12,18,26", 2, 2f, "攻速/移速/攻击/护甲/法强等属性 Buff", "按属性类型、强度、持续时间、覆盖人数综合估值", false);
            config[2001] = new SkillTemplateComponentConfig(2001, "加攻", "状态效果", new string[]{"光环","自身强化","单体治疗","范围治疗"}, "10,15,25,40,60", 2, 2f, "提升攻击力", "按属性类型、强度、持续时间、覆盖人数综合估值", false);
            config[2002] = new SkillTemplateComponentConfig(2002, "加护甲", "状态效果", new string[]{"光环","自身强化","单体治疗","范围治疗"}, "5,8,12,18,26", 1, 1f, "提升护甲", "按属性类型、强度、持续时间、覆盖人数综合估值", false);
            config[2003] = new SkillTemplateComponentConfig(2003, "加法强", "状态效果", new string[]{"光环","自身强化","单体治疗","范围治疗"}, "10,15,25,40,60", 2, 2f, "提升法术强度", "按属性类型、强度、持续时间、覆盖人数综合估值", false);
            config[2004] = new SkillTemplateComponentConfig(2004, "加魔抗", "状态效果", new string[]{"光环","自身强化","单体治疗","范围治疗"}, "5,8,12,18,26", 1, 1f, "提升魔法抗性", "按属性类型、强度、持续时间、覆盖人数综合估值", false);
            config[2005] = new SkillTemplateComponentConfig(2005, "加攻速", "状态效果", new string[]{"光环","自身强化","单体治疗","范围治疗"}, "5,8,12,18,26", 1, 1f, "提升攻击速度", "按属性类型、强度、持续时间、覆盖人数综合估值", false);
            config[2006] = new SkillTemplateComponentConfig(2006, "加移速", "状态效果", new string[]{"光环","自身强化","单体治疗","范围治疗"}, "5,8,12,18,26", 1, 1f, "提升移动速度", "按属性类型、强度、持续时间、覆盖人数综合估值", false);
            config[2010] = new SkillTemplateComponentConfig(2010, "护盾", "状态效果", new string[]{"单体治疗","范围治疗","自身强化"}, "60,90,150,240,360", 3, 3f, "吸收伤害的护盾（盾）", "护盾与治疗量分别估值", false);
            config[9000] = new SkillTemplateComponentConfig(9000, "斩杀", "特殊机制", new string[]{"单体伤害大","单体伤害中","单体伤害小","群体伤害近距离","群体伤害自身中心"}, "10,12,15,18,22", 4, 5f, "按血量阈值直接击杀或额外倍率", "特殊机制，人工评审，不套用普通伤害价格表", true);
            config[9010] = new SkillTemplateComponentConfig(9010, "驱散", "特殊机制", new string[]{"单体伤害大","单体伤害中","单体伤害小","群体伤害近距离","单体治疗","范围治疗","群体伤害自身中心"}, "1,1,2,2,3", 3, 3f, "移除目标增益/减益", "特殊机制，人工评审", true);
            config[9020] = new SkillTemplateComponentConfig(9020, "破盾", "特殊机制", new string[]{"单体伤害大","单体伤害中","单体伤害小","群体伤害近距离","群体伤害自身中心"}, "30,40,50,60,80", 2, 2f, "绕过或击破护盾", "特殊机制，人工评审", true);
            config[9030] = new SkillTemplateComponentConfig(9030, "位移", "特殊机制", new string[]{"单体伤害大","单体伤害中","单体伤害小","群体伤害近距离","群体伤害自身中心"}, "1,1,2,2,3", 2, 1.5f, "击退/拉近/换位", "特殊机制，人工评审", true);
            config[9040] = new SkillTemplateComponentConfig(9040, "召唤", "特殊机制", new string[]{"持续区域"}, "20,25,30,35,40", 4, 4f, "召唤单位或技能场", "特殊机制，人工评审", true);
            config[9050] = new SkillTemplateComponentConfig(9050, "永久成长", "特殊机制", new string[]{"自身强化"}, "2,3,4,5,6", 3, 3f, "永久属性成长", "特殊机制，人工评审", true);
            config[9060] = new SkillTemplateComponentConfig(9060, "复制", "特殊机制", new string[]{"自身强化"}, "30,40,50,60,70", 4, 4f, "复制效果或单位", "特殊机制，人工评审", true);
            config[9070] = new SkillTemplateComponentConfig(9070, "溅射", "特殊机制", new string[]{"单体伤害大","单体伤害中","单体伤害小"}, "30,35,40,50,60", 2, 2f, "主目标附近溅射伤害", "主目标与溅射目标伤害分别计量，溅射不是纯单体模板", false);
            config[9071] = new SkillTemplateComponentConfig(9071, "穿透", "特殊机制", new string[]{"群体伤害近距离","群体伤害自身中心"}, "2,2,3,3,4", 2, 2f, "直线穿透多个目标", "按实际穿透命中数估值", false);
            config[9072] = new SkillTemplateComponentConfig(9072, "弹道", "特殊机制", new string[]{"群体伤害近距离","群体伤害自身中心"}, "2,3,3,4,5", 2, 2f, "逐个命中的飞行物", "按飞弹倍率与命中数共同估值", false);

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
