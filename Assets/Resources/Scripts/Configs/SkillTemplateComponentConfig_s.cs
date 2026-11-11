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


        public SkillTemplateComponentConfig(int Id, string Name, string Category, string[] Templates, string Values, float Score, string Des, string PriceNote, bool IsSpecial)
        {
            this.Id = Id;
            this.Name = Name;
            this.Category = Category;
            this.Templates = Templates;
            this.Values = Values;
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
            config[2] = new SkillTemplateComponentConfig(2, "伤害倍率", "数值", new string[]{"单体伤害大","单体伤害中","单体伤害小","群体伤害近距离小范围","单体治疗","范围治疗","持续区域","自身强化"}, "", 0f, "按属性成长的伤害倍率", "与固定基值分开计量，避免重复计价", false);
            config[3] = new SkillTemplateComponentConfig(3, "治疗量", "数值", new string[]{"单体治疗","范围治疗"}, "", 0f, "治疗/回复量档位", "与伤害分别标定", false);
            config[4] = new SkillTemplateComponentConfig(4, "施法射程", "射程", new string[]{"单体伤害大","单体伤害中","单体伤害小","群体伤害近距离小范围","单体治疗","范围治疗","持续区域","自身强化","光环"}, "", 0f, "增加施法距离 Range，决定目标是否可被触达", "与攻击射程、AoE 半径区分，按可触达目标和触发成功率估值", false);
            config[5] = new SkillTemplateComponentConfig(5, "最大目标数", "目标数", new string[]{"单体伤害大","单体伤害中","单体伤害小","群体伤害近距离小范围","单体治疗","范围治疗","持续区域"}, "", 0f, "最多命中 N 个目标", "有限目标数与范围内全部目标分开；TargetCount<=0 在不同脚本中可能表示不限，不可按零成本处理", false);
            config[6] = new SkillTemplateComponentConfig(6, "小范围", "范围档", new string[]{"群体伤害近距离小范围","范围治疗","持续区域"}, "", 0f, "小半径 AoE 范围档", "按实际预期命中数估值，不按半径线性估价", false);
            config[7] = new SkillTemplateComponentConfig(7, "中范围", "范围档", new string[]{"群体伤害近距离小范围","范围治疗","持续区域"}, "", 0f, "中半径 AoE 范围档", "按实际预期命中数估值，不按半径线性估价", false);
            config[8] = new SkillTemplateComponentConfig(8, "减速", "状态效果", new string[]{"单体伤害大","单体伤害中","单体伤害小","群体伤害近距离小范围"}, "", 0f, "降低目标移动与攻速（缓）", "缓同时降低移速与攻速，按复合效果核算", false);
            config[9] = new SkillTemplateComponentConfig(9, "减防", "状态效果", new string[]{"单体伤害大","单体伤害中","单体伤害小","群体伤害近距离小范围"}, "", 0f, "降低目标护甲（破）", "按效果强度、持续时间、命中对象数综合估值", false);
            config[10] = new SkillTemplateComponentConfig(10, "减攻", "状态效果", new string[]{"单体伤害大","单体伤害中","单体伤害小","群体伤害近距离小范围"}, "", 0f, "降低目标攻击（慑）", "按效果强度、持续时间、命中对象数综合估值", false);
            config[11] = new SkillTemplateComponentConfig(11, "减疗", "状态效果", new string[]{"单体伤害大","单体伤害中","单体伤害小","群体伤害近距离小范围"}, "", 0f, "降低目标受到的治疗（疫）", "按效果强度、持续时间、命中对象数综合估值", false);
            config[12] = new SkillTemplateComponentConfig(12, "增伤", "状态效果", new string[]{"单体伤害大","单体伤害中","单体伤害小","群体伤害近距离小范围"}, "", 0f, "使目标受到的伤害提升（伤）", "按效果强度、持续时间、命中对象数综合估值", false);
            config[13] = new SkillTemplateComponentConfig(13, "眩晕", "状态效果", new string[]{"单体伤害大","单体伤害中","单体伤害小","群体伤害近距离小范围"}, "", 0f, "无法行动（乱）", "控制按覆盖量计价：强度、时长、命中数、施放频率", false);
            config[14] = new SkillTemplateComponentConfig(14, "定身", "状态效果", new string[]{"单体伤害大","单体伤害中","单体伤害小","群体伤害近距离小范围"}, "", 0f, "无法移动（停）", "控制按覆盖量计价：强度、时长、命中数、施放频率", false);
            config[15] = new SkillTemplateComponentConfig(15, "护盾", "状态效果", new string[]{"单体治疗","范围治疗","自身强化"}, "", 0f, "吸收伤害的护盾（盾）", "护盾与治疗量分别估值", false);
            config[16] = new SkillTemplateComponentConfig(16, "属性增益", "状态效果", new string[]{"单体治疗","范围治疗","自身强化"}, "", 0f, "攻速/移速/攻击/护甲/法强等属性 Buff", "按属性类型、强度、持续时间、覆盖人数综合估值", false);
            config[17] = new SkillTemplateComponentConfig(17, "连击", "持续", new string[]{"单体伤害大","单体伤害中","单体伤害小","群体伤害近距离小范围"}, "", 0f, "同一次施放结算多段", "按预期总段数估价，避免只按单次伤害估值", false);
            config[18] = new SkillTemplateComponentConfig(18, "持续伤害", "持续", new string[]{"单体伤害大","单体伤害中","单体伤害小","群体伤害近距离小范围","持续区域"}, "", 0f, "按周期结算的持续伤害（毒/败）", "按持续结算次数与目标滞留时间估价", false);
            config[19] = new SkillTemplateComponentConfig(19, "持续施法", "持续", new string[]{"群体伤害近距离小范围","持续区域"}, "", 0f, "施法期间持续结算", "按总结算次数与目标是否持续处于范围估价", false);
            config[20] = new SkillTemplateComponentConfig(20, "持续区域", "持续", new string[]{"持续区域"}, "", 0f, "创建持续一段时间的区域，周期结算伤害/控制", "按持续结算次数与目标是否持续留在范围估价", false);
            config[21] = new SkillTemplateComponentConfig(21, "低血筛选", "目标筛选", new string[]{"单体治疗","范围治疗"}, "", 0f, "优先选择最低血量目标", "选择性越强未必更贵；看是否提高击杀或救援效率", false);
            config[22] = new SkillTemplateComponentConfig(22, "后排筛选", "目标筛选", new string[]{"单体治疗","范围治疗"}, "", 0f, "优先选择后排目标", "选择性越强未必更贵；看是否提高击杀或救援效率", false);
            config[23] = new SkillTemplateComponentConfig(23, "英雄限定", "目标筛选", new string[]{"单体伤害大","单体伤害中","单体伤害小","群体伤害近距离小范围"}, "", 0f, "仅对英雄/特定单位生效", "需看筛选是否提高击杀或覆盖效果", false);
            config[24] = new SkillTemplateComponentConfig(24, "友军筛选", "目标筛选", new string[]{"单体治疗","范围治疗"}, "", 0f, "仅对特定类型友军生效", "需看筛选是否提高救援或覆盖效果", false);
            config[25] = new SkillTemplateComponentConfig(25, "斩杀", "特殊机制", new string[]{"单体伤害大","单体伤害中","单体伤害小","群体伤害近距离小范围"}, "", 0f, "按血量阈值直接击杀或额外倍率", "特殊机制，人工评审，不套用普通伤害价格表", true);
            config[26] = new SkillTemplateComponentConfig(26, "驱散", "特殊机制", new string[]{"单体伤害大","单体伤害中","单体伤害小","群体伤害近距离小范围","单体治疗","范围治疗"}, "", 0f, "移除目标增益/减益", "特殊机制，人工评审", true);
            config[27] = new SkillTemplateComponentConfig(27, "破盾", "特殊机制", new string[]{"单体伤害大","单体伤害中","单体伤害小","群体伤害近距离小范围"}, "", 0f, "绕过或击破护盾", "特殊机制，人工评审", true);
            config[28] = new SkillTemplateComponentConfig(28, "位移", "特殊机制", new string[]{"单体伤害大","单体伤害中","单体伤害小","群体伤害近距离小范围"}, "", 0f, "击退/拉近/换位", "特殊机制，人工评审", true);
            config[29] = new SkillTemplateComponentConfig(29, "召唤", "特殊机制", new string[]{"持续区域"}, "", 0f, "召唤单位或技能场", "特殊机制，人工评审", true);
            config[30] = new SkillTemplateComponentConfig(30, "永久成长", "特殊机制", new string[]{"自身强化"}, "", 0f, "永久属性成长", "特殊机制，人工评审", true);
            config[31] = new SkillTemplateComponentConfig(31, "复制", "特殊机制", new string[]{"自身强化"}, "", 0f, "复制效果或单位", "特殊机制，人工评审", true);
            config[32] = new SkillTemplateComponentConfig(32, "溅射", "特殊机制", new string[]{"单体伤害大","单体伤害中","单体伤害小"}, "", 0f, "主目标附近溅射伤害", "主目标与溅射目标伤害分别计量，溅射不是纯单体模板", false);
            config[33] = new SkillTemplateComponentConfig(33, "穿透", "特殊机制", new string[]{"群体伤害近距离小范围"}, "", 0f, "直线穿透多个目标", "按实际穿透命中数估值", false);
            config[34] = new SkillTemplateComponentConfig(34, "弹道", "特殊机制", new string[]{"群体伤害近距离小范围"}, "", 0f, "逐个命中的飞行物", "按飞弹倍率与命中数共同估值", false);
            config[35] = new SkillTemplateComponentConfig(35, "MP消耗", "施放节奏", new string[]{}, "", 0f, "技能 MP 消耗（MpCost）", "作为频率修正项，用实际发动频率校准，不与单次效果成本混为一项", false);
            config[36] = new SkillTemplateComponentConfig(36, "CD", "施放节奏", new string[]{}, "", 0f, "发动冷却（CD）", "作为频率修正项，用实际发动频率校准", false);
            config[37] = new SkillTemplateComponentConfig(37, "概率", "施放节奏", new string[]{}, "", 0f, "每次行动发动概率（Rate）", "作为频率修正项，用实际发动频率校准", false);
            config[38] = new SkillTemplateComponentConfig(38, "触发条件", "施放节奏", new string[]{}, "", 0f, "满足条件才触发（TriggerCondition）", "作为频率修正项，用实际发动频率校准", false);
            config[39] = new SkillTemplateComponentConfig(39, "光环", "光环", new string[]{"光环"}, "", 0f, "对全队持续或全局生效的属性效果（AuroAttrs）", "光环强度与覆盖人数共同估值", false);
            config[40] = new SkillTemplateComponentConfig(40, "混乱", "状态效果", new string[]{"单体伤害大","单体伤害中","单体伤害小","群体伤害近距离小范围"}, "", 0f, "不受控制，攻击最近的友方单位（惑）", "控制按覆盖量计价：强度、时长、命中数、施放频率", false);
            config[41] = new SkillTemplateComponentConfig(41, "大范围", "范围档", new string[]{"群体伤害近距离小范围"}, "", 0f, "大半径 AoE 范围档", "按实际预期命中数估值，不按半径线性估价", false);
            config[42] = new SkillTemplateComponentConfig(42, "自身中心", "范围档", new string[]{"群体伤害近距离小范围"}, "", 0f, "以施法者自身为范围中心，不做射程校验，站位受限", "省去射程但要求贴身，按可命中目标数估值", false);

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
