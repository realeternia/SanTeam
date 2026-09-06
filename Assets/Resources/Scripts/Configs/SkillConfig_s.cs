using System;
using System.Collections;
using System.Collections.Generic;

namespace CommonConfig
{
    public class SkillConfig
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
            {"Name", new FieldMetaInfo("名字", "string", 68)},
            {"Sname", new FieldMetaInfo("缩写", "string", 77)},
            {"Descript", new FieldMetaInfo("说明", "string", 402)},
            {"Type", new FieldMetaInfo("分类", "string", 0)},
            {"Comment", new FieldMetaInfo("备注", "string", 130)},
            {"Lv", new FieldMetaInfo("等级", "int", 60)},
            {"Rate", new FieldMetaInfo("发动概率", "float", 60)},
            {"CD", new FieldMetaInfo("发动cd", "float", 60)},
            {"MpCost", new FieldMetaInfo("技能MP消耗", "int", 60)},
            {"TriggerCondition", new FieldMetaInfo("触发条件", "string", 0)},
            {"AttackPointReduce", new FieldMetaInfo("攻击时间惩罚", "float", 60)},
            {"ConditionParm", new FieldMetaInfo("条件参数", "float", 60)},
            {"Attr", new FieldMetaInfo("相关属性", "string", 0)},
            {"CheckAttrs", new FieldMetaInfo("判定属性", "string[]", 0)},
            {"Range", new FieldMetaInfo("范围", "float", 60)},
            {"TargetType", new FieldMetaInfo("选取点", "string", 0)},
            {"TargetCount", new FieldMetaInfo("最大目标数", "int", 60)},
            {"Strength", new FieldMetaInfo("技能强度（恒定）", "float", 60)},
            {"StrengthInt", new FieldMetaInfo("技能强度（恒定）", "int", 60)},
            {"SkillAttrRate", new FieldMetaInfo("技能数值比例", "float", 60)},
            {"SkillDamageRate", new FieldMetaInfo("技能强度伤害比例", "float", 60)},
            {"SkillDamageAttrRate", new FieldMetaInfo("技能强度属性比例", "float", 60)},
            {"DoCount", new FieldMetaInfo("计数次数", "int", 60)},
            {"TimeDelay", new FieldMetaInfo("计数延迟", "float", 60)},
            {"UnitHelpType", new FieldMetaInfo("效果范围(1横向，2纵向)", "int", 60)},
            {"HelpSkill", new FieldMetaInfo("光环技能", "string", 0)},
            {"HelpSkillJob", new FieldMetaInfo("职业限定", "string", 0)},
            {"BuffId", new FieldMetaInfo("BuffId", "int", 60)},
            {"NegBuff", new FieldMetaInfo("是否针对负面buff", "bool", 0)},
            {"BuffTime", new FieldMetaInfo("Buff持续", "float", 60)},
            {"SummonTag", new FieldMetaInfo("召唤物标签", "string", 0)},
            {"SummonCount", new FieldMetaInfo("技能场数", "int", 60)},
            {"SummonArea", new FieldMetaInfo("技能场范围", "float", 60)},
            {"SummonTime", new FieldMetaInfo("技能场持续", "float", 60)},
            {"SummonHitInterval", new FieldMetaInfo("技能场间隔", "float", 60)},
            {"SummonSpeed", new FieldMetaInfo("技能场速度", "float", 60)},
            {"ScriptName", new FieldMetaInfo("脚本名", "string", 0)},
            {"Action", new FieldMetaInfo("动作", "string", 0)},
            {"HitEffect", new FieldMetaInfo("hit", "string", 0)},
            {"EffectSize", new FieldMetaInfo("size", "float", 60)},
            {"Icon", new FieldMetaInfo("图标", "string", 0)},
            {"LinkSelf", new FieldMetaInfo("连接英雄加成", "string", 188)},
            {"LinkTeam", new FieldMetaInfo("我方其他英雄加成", "string", 0)},
            {"AuroAttrs", new FieldMetaInfo("光环技能效果", "string", 0)},
        };

        public static Dictionary<string, FieldMetaInfo> FieldMeta { get { return fieldMeta; } }

        private static List<CellMeta> cellMeta = new List<CellMeta>();
        public static List<CellMeta> CellMetas { get { return cellMeta; } }

        /// <summary>
        ///序列
        /// </summary>
        public int Id;
        /// <summary>
        ///名字
        /// </summary>
        public string Name;
        /// <summary>
        ///缩写
        /// </summary>
        public string Sname;
        /// <summary>
        ///说明
        /// </summary>
        public string Descript;
        /// <summary>
        ///分类
        /// </summary>
        public string Type;
        /// <summary>
        ///备注
        /// </summary>
        public string Comment;
        /// <summary>
        ///等级
        /// </summary>
        public int Lv;
        /// <summary>
        ///发动概率
        /// </summary>
        public float Rate;
        /// <summary>
        ///发动cd
        /// </summary>
        public float CD;
        /// <summary>
        ///技能MP消耗（0=不使用MP，>0则每次行动充能MpCost/3，3次行动满，满才能发动，发动后清空）
        /// </summary>
        public int MpCost;
        /// <summary>
        ///触发条件（满足才触发技能，如 hprate<50=自身生命低于50%；多条件用;分隔）
        /// </summary>
        public string TriggerCondition;
        /// <summary>
        ///攻击时间惩罚
        /// </summary>
        public float AttackPointReduce;
        /// <summary>
        ///条件参数
        /// </summary>
        public float ConditionParm;
        /// <summary>
        ///相关属性：ap=法强 / atk=攻击（无双强度已并入 atk，物理技能统一按 atk 成长、护甲减免）
        /// </summary>
        public string Attr;
        /// <summary>
        ///判定属性
        /// </summary>
        public string[] CheckAttrs;
        /// <summary>
        ///范围
        /// </summary>
        public float Range;
        /// <summary>
        ///选取点
        /// </summary>
        public string TargetType;
        /// <summary>
        ///最大目标数
        /// </summary>
        public int TargetCount;
        /// <summary>
        ///固定系数（技能伤害=固定系数+比例系数×关联属性）
        /// </summary>
        public float Strength;
        /// <summary>
        ///技能强度（恒定）
        /// </summary>
        public int StrengthInt;
        /// <summary>
        ///技能数值比例
        /// </summary>
        public float SkillAttrRate;
        /// <summary>
        ///技能强度伤害比例
        /// </summary>
        public float SkillDamageRate;
        /// <summary>
        ///比例系数（技能伤害=固定系数+比例系数×关联属性）
        /// </summary>
        public float SkillDamageAttrRate;
        /// <summary>
        ///计数次数
        /// </summary>
        public int DoCount;
        /// <summary>
        ///计数延迟
        /// </summary>
        public float TimeDelay;
        /// <summary>
        ///效果范围(1横向，2纵向)
        /// </summary>
        public int UnitHelpType;
        /// <summary>
        ///光环技能
        /// </summary>
        public string HelpSkill;
        /// <summary>
        ///职业限定
        /// </summary>
        public string HelpSkillJob;
        /// <summary>
        ///BuffId
        /// </summary>
        public int BuffId;
        /// <summary>
        ///是否针对负面buff
        /// </summary>
        public bool NegBuff;
        /// <summary>
        ///Buff持续
        /// </summary>
        public float BuffTime;
        /// <summary>
        ///召唤物标签
        /// </summary>
        public string SummonTag;
        /// <summary>
        ///技能场数
        /// </summary>
        public int SummonCount;
        /// <summary>
        ///技能场范围
        /// </summary>
        public float SummonArea;
        /// <summary>
        ///技能场持续
        /// </summary>
        public float SummonTime;
        /// <summary>
        ///技能场间隔
        /// </summary>
        public float SummonHitInterval;
        /// <summary>
        ///技能场速度
        /// </summary>
        public float SummonSpeed;
        /// <summary>
        ///脚本名
        /// </summary>
        public string ScriptName;
        /// <summary>
        ///动作
        /// </summary>
        public string Action;
        /// <summary>
        ///hit
        /// </summary>
        public string HitEffect;
        /// <summary>
        ///size
        /// </summary>
        public float EffectSize;
        /// <summary>
        ///图标
        /// </summary>
        public string Icon;
        /// <summary>
        ///职业羁绊-连接英雄加成（该职业每个英雄自身获得，格式"atk+12,armor+6"，职业技能行专用，Lv1~Lv5对应上阵1/2/3/4/5人；soldierAtk/soldierHp例外，施加给本侧全部士兵）
        /// </summary>
        public string LinkSelf;
        /// <summary>
        ///职业羁绊-我方其他英雄加成（除该职业英雄外的我方全体英雄获得的总量，格式同LinkSelf，职业技能行专用）
        /// </summary>
        public string LinkTeam;
        /// <summary>
        ///光环技能效果（我方全体英雄获得，含提供者不用排除；格式同LinkTeam，效果受auroEffectRate修正）
        /// </summary>
        public string AuroAttrs;


        public SkillConfig(int Id, string Name, string Sname, string Descript, string Type, string Comment, int Lv, float Rate, float CD, int MpCost, string TriggerCondition, float AttackPointReduce, float ConditionParm, string Attr, string[] CheckAttrs, float Range, string TargetType, int TargetCount, float Strength, int StrengthInt, float SkillAttrRate, float SkillDamageRate, float SkillDamageAttrRate, int DoCount, float TimeDelay, int UnitHelpType, string HelpSkill, string HelpSkillJob, int BuffId, bool NegBuff, float BuffTime, string SummonTag, int SummonCount, float SummonArea, float SummonTime, float SummonHitInterval, float SummonSpeed, string ScriptName, string Action, string HitEffect, float EffectSize, string Icon, string LinkSelf, string LinkTeam, string AuroAttrs)
        {
            this.Id = Id;
            this.Name = Name;
            this.Sname = Sname;
            this.Descript = Descript;
            this.Type = Type;
            this.Comment = Comment;
            this.Lv = Lv;
            this.Rate = Rate;
            this.CD = CD;
            this.MpCost = MpCost;
            this.TriggerCondition = TriggerCondition;
            this.AttackPointReduce = AttackPointReduce;
            this.ConditionParm = ConditionParm;
            this.Attr = Attr;
            this.CheckAttrs = CheckAttrs;
            this.Range = Range;
            this.TargetType = TargetType;
            this.TargetCount = TargetCount;
            this.Strength = Strength;
            this.StrengthInt = StrengthInt;
            this.SkillAttrRate = SkillAttrRate;
            this.SkillDamageRate = SkillDamageRate;
            this.SkillDamageAttrRate = SkillDamageAttrRate;
            this.DoCount = DoCount;
            this.TimeDelay = TimeDelay;
            this.UnitHelpType = UnitHelpType;
            this.HelpSkill = HelpSkill;
            this.HelpSkillJob = HelpSkillJob;
            this.BuffId = BuffId;
            this.NegBuff = NegBuff;
            this.BuffTime = BuffTime;
            this.SummonTag = SummonTag;
            this.SummonCount = SummonCount;
            this.SummonArea = SummonArea;
            this.SummonTime = SummonTime;
            this.SummonHitInterval = SummonHitInterval;
            this.SummonSpeed = SummonSpeed;
            this.ScriptName = ScriptName;
            this.Action = Action;
            this.HitEffect = HitEffect;
            this.EffectSize = EffectSize;
            this.Icon = Icon;
            this.LinkSelf = LinkSelf;
            this.LinkTeam = LinkTeam;
            this.AuroAttrs = AuroAttrs;
        }

        public SkillConfig() { }

        private static Dictionary<int, SkillConfig> config = new Dictionary<int, SkillConfig>();
        public static Dictionary<int, SkillConfig>.ValueCollection ConfigList
        {
            get { return config.Values; }
        }

        public static void Refresh(Dictionary<int, SkillConfig> dict)
        {
            config.Clear();
            config = dict;
            RebuildIndex();
        }

        public static void Load()
        {
            config.Clear();
            config[2000011] = new SkillConfig(2000011, "诸侯", "王", "自身攻击+3、护甲+3、生命+40；阵营护盾额外+10%", "职业", "", 1, 0f, 0f, 0, "", 0f, 0f, "", null, 0f, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "Dumb", "", "", 0f, "shuai", "atk+3,armor+3,maxHp+40", "", "");
            config[2000012] = new SkillConfig(2000012, "诸侯", "王", "自身攻击+4、护甲+4、生命+60；阵营护盾额外+10%", "职业", "", 2, 0f, 0f, 0, "", 0f, 0f, "", null, 0f, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "Dumb", "", "", 0f, "shuai", "atk+4,armor+4,maxHp+60", "", "");
            config[2000013] = new SkillConfig(2000013, "诸侯", "王", "自身攻击+5、护甲+5、生命+80；阵营护盾额外+10%", "职业", "", 3, 0f, 0f, 0, "", 0f, 0f, "", null, 0f, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "Dumb", "", "", 0f, "shuai", "atk+5,armor+5,maxHp+80", "", "");
            config[2000014] = new SkillConfig(2000014, "诸侯", "王", "自身攻击+6、护甲+6、生命+100；阵营护盾额外+10%", "职业", "", 4, 0f, 0f, 0, "", 0f, 0f, "", null, 0f, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "Dumb", "", "", 0f, "shuai", "atk+6,armor+6,maxHp+100", "", "");
            config[2000015] = new SkillConfig(2000015, "诸侯", "王", "自身攻击+7、护甲+7、生命+120；阵营护盾额外+10%", "职业", "", 5, 0f, 0f, 0, "", 0f, 0f, "", null, 0f, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "Dumb", "", "", 0f, "shuai", "atk+7,armor+7,maxHp+120", "", "");
            config[2000021] = new SkillConfig(2000021, "羽扇", "扇", "自身施加的负面buff持续+10%", "职业", "", 1, 0f, 0f, 0, "", 0f, 0f, "", null, 0f, "", 0, 0.1f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, true, 0f, "", 0, 0f, 0f, 0f, 0f, "ModifyBuffTime", "", "", 0f, "shan", "", "mpRegen+2", "");
            config[2000022] = new SkillConfig(2000022, "羽扇", "扇", "自身施加的负面buff持续+15%", "职业", "", 2, 0f, 0f, 0, "", 0f, 0f, "", null, 0f, "", 0, 0.15f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, true, 0f, "", 0, 0f, 0f, 0f, 0f, "ModifyBuffTime", "", "", 0f, "shan", "", "mpRegen+3", "");
            config[2000023] = new SkillConfig(2000023, "羽扇", "扇", "自身施加的负面buff持续+20%", "职业", "", 3, 0f, 0f, 0, "", 0f, 0f, "", null, 0f, "", 0, 0.2f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, true, 0f, "", 0, 0f, 0f, 0f, 0f, "ModifyBuffTime", "", "", 0f, "shan", "", "mpRegen+4", "");
            config[2000024] = new SkillConfig(2000024, "羽扇", "扇", "自身施加的负面buff持续+25%", "职业", "", 4, 0f, 0f, 0, "", 0f, 0f, "", null, 0f, "", 0, 0.25f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, true, 0f, "", 0, 0f, 0f, 0f, 0f, "ModifyBuffTime", "", "", 0f, "shan", "", "mpRegen+5", "");
            config[2000025] = new SkillConfig(2000025, "羽扇", "扇", "自身施加的负面buff持续+30%", "职业", "", 5, 0f, 0f, 0, "", 0f, 0f, "", null, 0f, "", 0, 0.3f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, true, 0f, "", 0, 0f, 0f, 0f, 0f, "ModifyBuffTime", "", "", 0f, "shan", "", "mpRegen+6", "");
            config[2000031] = new SkillConfig(2000031, "猛将", "锤", "自身生命低于50%时攻击+6", "职业", "", 1, 0f, 0f, 0, "hprate<50", 0f, 0f, "", null, 0f, "", 0, 0f, 6, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "AttackAddDamage", "", "", 0f, "dao", "", "", "");
            config[2000032] = new SkillConfig(2000032, "猛将", "锤", "自身生命低于50%时攻击+9", "职业", "", 2, 0f, 0f, 0, "hprate<50", 0f, 0f, "", null, 0f, "", 0, 0f, 9, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "AttackAddDamage", "", "", 0f, "dao", "", "", "");
            config[2000033] = new SkillConfig(2000033, "猛将", "锤", "自身生命低于50%时攻击+12", "职业", "", 3, 0f, 0f, 0, "hprate<50", 0f, 0f, "", null, 0f, "", 0, 0f, 12, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "AttackAddDamage", "", "", 0f, "dao", "", "", "");
            config[2000034] = new SkillConfig(2000034, "猛将", "锤", "自身生命低于50%时攻击+15", "职业", "", 4, 0f, 0f, 0, "hprate<50", 0f, 0f, "", null, 0f, "", 0, 0f, 15, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "AttackAddDamage", "", "", 0f, "dao", "", "", "");
            config[2000035] = new SkillConfig(2000035, "猛将", "锤", "自身生命低于50%时攻击+18", "职业", "", 5, 0f, 0f, 0, "hprate<50", 0f, 0f, "", null, 0f, "", 0, 0f, 18, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "AttackAddDamage", "", "", 0f, "dao", "", "", "");
            config[2000041] = new SkillConfig(2000041, "坚韧", "士", "自身生命+100，全队生命+20", "职业", "", 1, 0f, 0f, 0, "", 0f, 0f, "", null, 0f, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "Dumb", "", "", 0f, "shi", "maxHp+100", "maxHp+20", "");
            config[2000042] = new SkillConfig(2000042, "坚韧", "士", "自身生命+100，全队生命+40", "职业", "", 2, 0f, 0f, 0, "", 0f, 0f, "", null, 0f, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "Dumb", "", "", 0f, "shi", "maxHp+100", "maxHp+40", "");
            config[2000043] = new SkillConfig(2000043, "坚韧", "士", "自身生命+100，全队生命+60", "职业", "", 3, 0f, 0f, 0, "", 0f, 0f, "", null, 0f, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "Dumb", "", "", 0f, "shi", "maxHp+100", "maxHp+60", "");
            config[2000044] = new SkillConfig(2000044, "坚韧", "士", "自身生命+100，全队生命+80", "职业", "", 4, 0f, 0f, 0, "", 0f, 0f, "", null, 0f, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "Dumb", "", "", 0f, "shi", "maxHp+100", "maxHp+80", "");
            config[2000045] = new SkillConfig(2000045, "坚韧", "士", "自身生命+100，全队生命+100", "职业", "", 5, 0f, 0f, 0, "", 0f, 0f, "", null, 0f, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "Dumb", "", "", 0f, "shi", "maxHp+100", "maxHp+100", "");
            config[2000051] = new SkillConfig(2000051, "灵活", "马", "自身闪避+8%", "职业", "", 1, 0f, 0f, 0, "", 0f, 0f, "", null, 0f, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "Dumb", "", "", 0f, "ma", "dodgeRate+0.08", "", "");
            config[2000052] = new SkillConfig(2000052, "灵活", "马", "自身闪避+12%", "职业", "", 2, 0f, 0f, 0, "", 0f, 0f, "", null, 0f, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "Dumb", "", "", 0f, "ma", "dodgeRate+0.12", "", "");
            config[2000053] = new SkillConfig(2000053, "灵活", "马", "自身闪避+16%", "职业", "", 3, 0f, 0f, 0, "", 0f, 0f, "", null, 0f, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "Dumb", "", "", 0f, "ma", "dodgeRate+0.16", "", "");
            config[2000054] = new SkillConfig(2000054, "灵活", "马", "自身闪避+20%", "职业", "", 4, 0f, 0f, 0, "", 0f, 0f, "", null, 0f, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "Dumb", "", "", 0f, "ma", "dodgeRate+0.2", "", "");
            config[2000055] = new SkillConfig(2000055, "灵活", "马", "自身闪避+24%", "职业", "", 5, 0f, 0f, 0, "", 0f, 0f, "", null, 0f, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "Dumb", "", "", 0f, "ma", "dodgeRate+0.24", "", "");
            config[2000061] = new SkillConfig(2000061, "运筹", "相", "全军士兵攻击+16%、生命+16%，我方其他英雄法力回复+2/秒", "职业", "", 1, 0f, 0f, 0, "", 0f, 0f, "", null, 0f, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "Dumb", "", "", 0f, "xiang", "soldierAtk+0.16,soldierHp+0.16", "mpRegen+2", "");
            config[2000062] = new SkillConfig(2000062, "运筹", "相", "全军士兵攻击+24%、生命+24%，我方其他英雄法力回复+3/秒", "职业", "", 2, 0f, 0f, 0, "", 0f, 0f, "", null, 0f, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "Dumb", "", "", 0f, "xiang", "soldierAtk+0.24,soldierHp+0.24", "mpRegen+3", "");
            config[2000063] = new SkillConfig(2000063, "运筹", "相", "全军士兵攻击+32%、生命+32%，我方其他英雄法力回复+4/秒", "职业", "", 3, 0f, 0f, 0, "", 0f, 0f, "", null, 0f, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "Dumb", "", "", 0f, "xiang", "soldierAtk+0.32,soldierHp+0.32", "mpRegen+4", "");
            config[2000064] = new SkillConfig(2000064, "运筹", "相", "全军士兵攻击+40%、生命+40%，我方其他英雄法力回复+5/秒", "职业", "", 4, 0f, 0f, 0, "", 0f, 0f, "", null, 0f, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "Dumb", "", "", 0f, "xiang", "soldierAtk+0.4,soldierHp+0.4", "mpRegen+5", "");
            config[2000065] = new SkillConfig(2000065, "运筹", "相", "全军士兵攻击+48%、生命+48%，我方其他英雄法力回复+6/秒", "职业", "", 5, 0f, 0f, 0, "", 0f, 0f, "", null, 0f, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "Dumb", "", "", 0f, "xiang", "soldierAtk+0.48,soldierHp+0.48", "mpRegen+6", "");
            config[2000071] = new SkillConfig(2000071, "弓手", "弓", "自身攻击+8，全队攻击+4", "职业", "", 1, 0f, 0f, 0, "", 0f, 0f, "", null, 0f, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "Dumb", "", "", 0f, "gong", "atk+8", "atk+4", "");
            config[2000072] = new SkillConfig(2000072, "弓手", "弓", "自身攻击+9，全队攻击+6", "职业", "", 2, 0f, 0f, 0, "", 0f, 0f, "", null, 0f, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "Dumb", "", "", 0f, "gong", "atk+9", "atk+6", "");
            config[2000073] = new SkillConfig(2000073, "弓手", "弓", "自身攻击+10，全队攻击+8", "职业", "", 3, 0f, 0f, 0, "", 0f, 0f, "", null, 0f, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "Dumb", "", "", 0f, "gong", "atk+10", "atk+8", "");
            config[2000074] = new SkillConfig(2000074, "弓手", "弓", "自身攻击+11，全队攻击+10", "职业", "", 4, 0f, 0f, 0, "", 0f, 0f, "", null, 0f, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "Dumb", "", "", 0f, "gong", "atk+11", "atk+10", "");
            config[2000075] = new SkillConfig(2000075, "弓手", "弓", "自身攻击+12，全队攻击+12", "职业", "", 5, 0f, 0f, 0, "", 0f, 0f, "", null, 0f, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "Dumb", "", "", 0f, "gong", "atk+12", "atk+12", "");
            config[2000081] = new SkillConfig(2000081, "谋略", "棋", "自身法强+8，全队法强+3", "职业", "", 1, 0f, 0f, 0, "", 0f, 0f, "", null, 0f, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "Dumb", "", "", 0f, "mou", "ap+8", "ap+3", "");
            config[2000082] = new SkillConfig(2000082, "谋略", "棋", "自身法强+9，全队法强+4", "职业", "", 2, 0f, 0f, 0, "", 0f, 0f, "", null, 0f, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "Dumb", "", "", 0f, "mou", "ap+9", "ap+4", "");
            config[2000083] = new SkillConfig(2000083, "谋略", "棋", "自身法强+10，全队法强+5", "职业", "", 3, 0f, 0f, 0, "", 0f, 0f, "", null, 0f, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "Dumb", "", "", 0f, "mou", "ap+10", "ap+5", "");
            config[2000084] = new SkillConfig(2000084, "谋略", "棋", "自身法强+11，全队法强+6", "职业", "", 4, 0f, 0f, 0, "", 0f, 0f, "", null, 0f, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "Dumb", "", "", 0f, "mou", "ap+11", "ap+6", "");
            config[2000085] = new SkillConfig(2000085, "谋略", "棋", "自身法强+12，全队法强+7", "职业", "", 5, 0f, 0f, 0, "", 0f, 0f, "", null, 0f, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "Dumb", "", "", 0f, "mou", "ap+12", "ap+7", "");
            config[2000091] = new SkillConfig(2000091, "炮车", "炮", "攻击时20%几率对周围造成50%溅射伤害，范围+10%", "职业", "", 1, 0.2f, 0f, 0, "", 0f, 0f, "atk", null, 6.6f, "", 3, 0f, 0, 0f, 0.5f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "HitArea", "", "SparkleAreaWhite", 0f, "pao", "", "", "");
            config[2000092] = new SkillConfig(2000092, "炮车", "炮", "攻击时20%几率对周围造成50%溅射伤害，范围+15%", "职业", "", 2, 0.2f, 0f, 0, "", 0f, 0f, "atk", null, 6.9f, "", 3, 0f, 0, 0f, 0.5f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "HitArea", "", "SparkleAreaWhite", 0f, "pao", "", "", "");
            config[2000093] = new SkillConfig(2000093, "炮车", "炮", "攻击时20%几率对周围造成50%溅射伤害，范围+20%", "职业", "", 3, 0.2f, 0f, 0, "", 0f, 0f, "atk", null, 7.2f, "", 3, 0f, 0, 0f, 0.5f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "HitArea", "", "SparkleAreaWhite", 0f, "pao", "", "", "");
            config[2000094] = new SkillConfig(2000094, "炮车", "炮", "攻击时20%几率对周围造成50%溅射伤害，范围+25%", "职业", "", 4, 0.2f, 0f, 0, "", 0f, 0f, "atk", null, 7.5f, "", 3, 0f, 0, 0f, 0.5f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "HitArea", "", "SparkleAreaWhite", 0f, "pao", "", "", "");
            config[2000095] = new SkillConfig(2000095, "炮车", "炮", "攻击时20%几率对周围造成50%溅射伤害，范围+30%", "职业", "", 5, 0.2f, 0f, 0, "", 0f, 0f, "atk", null, 7.8f, "", 3, 0f, 0, 0f, 0.5f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "HitArea", "", "SparkleAreaWhite", 0f, "pao", "", "", "");
            config[2000101] = new SkillConfig(2000101, "弩手", "弩", "自身攻速+0.04，全队攻速+0.04", "职业", "", 1, 0f, 0f, 0, "", 0f, 0f, "", null, 0f, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "Dumb", "", "", 0f, "nu", "attackRate+0.04", "attackRate+0.04", "");
            config[2000102] = new SkillConfig(2000102, "弩手", "弩", "自身攻速+0.05，全队攻速+0.05", "职业", "", 2, 0f, 0f, 0, "", 0f, 0f, "", null, 0f, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "Dumb", "", "", 0f, "nu", "attackRate+0.05", "attackRate+0.05", "");
            config[2000103] = new SkillConfig(2000103, "弩手", "弩", "自身攻速+0.06，全队攻速+0.06", "职业", "", 3, 0f, 0f, 0, "", 0f, 0f, "", null, 0f, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "Dumb", "", "", 0f, "nu", "attackRate+0.06", "attackRate+0.06", "");
            config[2000104] = new SkillConfig(2000104, "弩手", "弩", "自身攻速+0.07，全队攻速+0.07", "职业", "", 4, 0f, 0f, 0, "", 0f, 0f, "", null, 0f, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "Dumb", "", "", 0f, "nu", "attackRate+0.07", "attackRate+0.07", "");
            config[2000105] = new SkillConfig(2000105, "弩手", "弩", "自身攻速+0.08，全队攻速+0.08", "职业", "", 5, 0f, 0f, 0, "", 0f, 0f, "", null, 0f, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "Dumb", "", "", 0f, "nu", "attackRate+0.08", "attackRate+0.08", "");
            config[2000111] = new SkillConfig(2000111, "碾压", "车", "自身暴击+10%，全队暴击+1%", "职业", "", 1, 0f, 0f, 0, "", 0f, 0f, "", null, 0f, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "Dumb", "", "", 0f, "che", "critRate+0.1", "critRate+0.01", "");
            config[2000112] = new SkillConfig(2000112, "碾压", "车", "自身暴击+15%，全队暴击+2%", "职业", "", 2, 0f, 0f, 0, "", 0f, 0f, "", null, 0f, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "Dumb", "", "", 0f, "che", "critRate+0.15", "critRate+0.02", "");
            config[2000113] = new SkillConfig(2000113, "碾压", "车", "自身暴击+20%，全队暴击+3%", "职业", "", 3, 0f, 0f, 0, "", 0f, 0f, "", null, 0f, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "Dumb", "", "", 0f, "che", "critRate+0.2", "critRate+0.03", "");
            config[2000114] = new SkillConfig(2000114, "碾压", "车", "自身暴击+25%，全队暴击+4%", "职业", "", 4, 0f, 0f, 0, "", 0f, 0f, "", null, 0f, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "Dumb", "", "", 0f, "che", "critRate+0.25", "critRate+0.04", "");
            config[2000115] = new SkillConfig(2000115, "碾压", "车", "自身暴击+30%，全队暴击+5%", "职业", "", 5, 0f, 0f, 0, "", 0f, 0f, "", null, 0f, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "Dumb", "", "", 0f, "che", "critRate+0.3", "critRate+0.05", "");
            config[2000121] = new SkillConfig(2000121, "声乐", "琴", "自身施加的正面buff持续+10%，全队护甲+2", "职业", "", 1, 0f, 0f, 0, "", 0f, 0f, "", null, 0f, "", 0, 0.1f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "ModifyBuffTime", "", "", 0f, "song", "", "armor+2", "");
            config[2000122] = new SkillConfig(2000122, "声乐", "琴", "自身施加的正面buff持续+15%，全队护甲+3", "职业", "", 2, 0f, 0f, 0, "", 0f, 0f, "", null, 0f, "", 0, 0.15f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "ModifyBuffTime", "", "", 0f, "song", "", "armor+3", "");
            config[2000123] = new SkillConfig(2000123, "声乐", "琴", "自身施加的正面buff持续+20%，全队护甲+4", "职业", "", 3, 0f, 0f, 0, "", 0f, 0f, "", null, 0f, "", 0, 0.2f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "ModifyBuffTime", "", "", 0f, "song", "", "armor+4", "");
            config[2000124] = new SkillConfig(2000124, "声乐", "琴", "自身施加的正面buff持续+25%，全队护甲+5", "职业", "", 4, 0f, 0f, 0, "", 0f, 0f, "", null, 0f, "", 0, 0.25f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "ModifyBuffTime", "", "", 0f, "song", "", "armor+5", "");
            config[2000125] = new SkillConfig(2000125, "声乐", "琴", "自身施加的正面buff持续+30%，全队护甲+6", "职业", "", 5, 0f, 0f, 0, "", 0f, 0f, "", null, 0f, "", 0, 0.3f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "ModifyBuffTime", "", "", 0f, "song", "", "armor+6", "");
            config[2000131] = new SkillConfig(2000131, "治疗", "医", "自身治疗+10%，全队生命回复+3/秒", "职业", "", 1, 0f, 0f, 0, "", 0f, 0f, "", null, 0f, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "Dumb", "", "", 0f, "heal", "healRate+0.1", "hpRegen+3", "");
            config[2000132] = new SkillConfig(2000132, "治疗", "医", "自身治疗+15%，全队生命回复+6/秒", "职业", "", 2, 0f, 0f, 0, "", 0f, 0f, "", null, 0f, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "Dumb", "", "", 0f, "heal", "healRate+0.15", "hpRegen+6", "");
            config[2000133] = new SkillConfig(2000133, "治疗", "医", "自身治疗+20%，全队生命回复+9/秒", "职业", "", 3, 0f, 0f, 0, "", 0f, 0f, "", null, 0f, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "Dumb", "", "", 0f, "heal", "healRate+0.2", "hpRegen+9", "");
            config[2000134] = new SkillConfig(2000134, "治疗", "医", "自身治疗+25%，全队生命回复+12/秒", "职业", "", 4, 0f, 0f, 0, "", 0f, 0f, "", null, 0f, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "Dumb", "", "", 0f, "heal", "healRate+0.25", "hpRegen+12", "");
            config[2000135] = new SkillConfig(2000135, "治疗", "医", "自身治疗+30%，全队生命回复+15/秒", "职业", "", 5, 0f, 0f, 0, "", 0f, 0f, "", null, 0f, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "Dumb", "", "", 0f, "heal", "healRate+0.3", "hpRegen+15", "");
            config[2000141] = new SkillConfig(2000141, "枪阵", "枪", "攻击时8%几率眩晕目标1.5秒", "职业", "", 1, 0.08f, 0f, 0, "", 0f, 0f, "atk", null, 0f, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 301001, false, 1.5f, "", 0, 0f, 0f, 0f, 0f, "HitBuff", "", "", 0f, "qiang", "", "", "");
            config[2000142] = new SkillConfig(2000142, "枪阵", "枪", "攻击时10%几率眩晕目标1.5秒", "职业", "", 2, 0.1f, 0f, 0, "", 0f, 0f, "atk", null, 0f, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 301001, false, 1.5f, "", 0, 0f, 0f, 0f, 0f, "HitBuff", "", "", 0f, "qiang", "", "", "");
            config[2000143] = new SkillConfig(2000143, "枪阵", "枪", "攻击时12%几率眩晕目标1.5秒", "职业", "", 3, 0.12f, 0f, 0, "", 0f, 0f, "atk", null, 0f, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 301001, false, 1.5f, "", 0, 0f, 0f, 0f, 0f, "HitBuff", "", "", 0f, "qiang", "", "", "");
            config[2000144] = new SkillConfig(2000144, "枪阵", "枪", "攻击时14%几率眩晕目标1.5秒", "职业", "", 4, 0.14f, 0f, 0, "", 0f, 0f, "atk", null, 0f, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 301001, false, 1.5f, "", 0, 0f, 0f, 0f, 0f, "HitBuff", "", "", 0f, "qiang", "", "", "");
            config[2000145] = new SkillConfig(2000145, "枪阵", "枪", "攻击时16%几率眩晕目标1.5秒", "职业", "", 5, 0.16f, 0f, 0, "", 0f, 0f, "atk", null, 0f, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 301001, false, 1.5f, "", 0, 0f, 0f, 0f, 0f, "HitBuff", "", "", 0f, "qiang", "", "", "");
            config[2000151] = new SkillConfig(2000151, "戟阵", "戟", "攻击时10%几率对周围造成50%溅射伤害", "职业", "", 1, 0.1f, 0f, 0, "", 0f, 0f, "atk", null, 6f, "", 3, 0f, 0, 0f, 0.5f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "HitArea", "", "SparkleAreaWhite", 0f, "ji", "", "", "");
            config[2000152] = new SkillConfig(2000152, "戟阵", "戟", "攻击时15%几率对周围造成50%溅射伤害", "职业", "", 2, 0.15f, 0f, 0, "", 0f, 0f, "atk", null, 6f, "", 3, 0f, 0, 0f, 0.5f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "HitArea", "", "SparkleAreaWhite", 0f, "ji", "", "", "");
            config[2000153] = new SkillConfig(2000153, "戟阵", "戟", "攻击时20%几率对周围造成50%溅射伤害", "职业", "", 3, 0.2f, 0f, 0, "", 0f, 0f, "atk", null, 6f, "", 3, 0f, 0, 0f, 0.5f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "HitArea", "", "SparkleAreaWhite", 0f, "ji", "", "", "");
            config[2000154] = new SkillConfig(2000154, "戟阵", "戟", "攻击时25%几率对周围造成50%溅射伤害", "职业", "", 4, 0.25f, 0f, 0, "", 0f, 0f, "atk", null, 6f, "", 3, 0f, 0, 0f, 0.5f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "HitArea", "", "SparkleAreaWhite", 0f, "ji", "", "", "");
            config[2000155] = new SkillConfig(2000155, "戟阵", "戟", "攻击时30%几率对周围造成50%溅射伤害", "职业", "", 5, 0.3f, 0f, 0, "", 0f, 0f, "atk", null, 6f, "", 3, 0f, 0, 0f, 0.5f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "HitArea", "", "SparkleAreaWhite", 0f, "ji", "", "", "");
            config[2000161] = new SkillConfig(2000161, "战鼓", "鼓", "自身光环效果+16%", "职业", "", 1, 0f, 0f, 0, "", 0f, 0f, "", null, 0f, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "Dumb", "", "", 0f, "gu", "auroEffectRate+0.16", "", "");
            config[2000162] = new SkillConfig(2000162, "战鼓", "鼓", "自身光环效果+24%", "职业", "", 2, 0f, 0f, 0, "", 0f, 0f, "", null, 0f, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "Dumb", "", "", 0f, "gu", "auroEffectRate+0.24", "", "");
            config[2000163] = new SkillConfig(2000163, "战鼓", "鼓", "自身光环效果+32%", "职业", "", 3, 0f, 0f, 0, "", 0f, 0f, "", null, 0f, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "Dumb", "", "", 0f, "gu", "auroEffectRate+0.32", "", "");
            config[2000164] = new SkillConfig(2000164, "战鼓", "鼓", "自身光环效果+40%", "职业", "", 4, 0f, 0f, 0, "", 0f, 0f, "", null, 0f, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "Dumb", "", "", 0f, "gu", "auroEffectRate+0.4", "", "");
            config[2000165] = new SkillConfig(2000165, "战鼓", "鼓", "自身光环效果+48%", "职业", "", 5, 0f, 0f, 0, "", 0f, 0f, "", null, 0f, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "Dumb", "", "", 0f, "gu", "auroEffectRate+0.48", "", "");
            config[2000171] = new SkillConfig(2000171, "铁壁", "盾", "自身护甲+6、魔抗+6，全队护甲+2", "职业", "", 1, 0f, 0f, 0, "", 0f, 0f, "", null, 0f, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "Dumb", "", "", 0f, "dun", "armor+6,magicRes+6", "armor+2", "");
            config[2000172] = new SkillConfig(2000172, "铁壁", "盾", "自身护甲+9、魔抗+9，全队护甲+3", "职业", "", 2, 0f, 0f, 0, "", 0f, 0f, "", null, 0f, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "Dumb", "", "", 0f, "dun", "armor+9,magicRes+9", "armor+3", "");
            config[2000173] = new SkillConfig(2000173, "铁壁", "盾", "自身护甲+12、魔抗+12，全队护甲+4", "职业", "", 3, 0f, 0f, 0, "", 0f, 0f, "", null, 0f, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "Dumb", "", "", 0f, "dun", "armor+12,magicRes+12", "armor+4", "");
            config[2000174] = new SkillConfig(2000174, "铁壁", "盾", "自身护甲+15、魔抗+15，全队护甲+5", "职业", "", 4, 0f, 0f, 0, "", 0f, 0f, "", null, 0f, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "Dumb", "", "", 0f, "dun", "armor+15,magicRes+15", "armor+5", "");
            config[2000175] = new SkillConfig(2000175, "铁壁", "盾", "自身护甲+18、魔抗+18，全队护甲+6", "职业", "", 5, 0f, 0f, 0, "", 0f, 0f, "", null, 0f, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "Dumb", "", "", 0f, "dun", "armor+18,magicRes+18", "armor+6", "");
            config[2000181] = new SkillConfig(2000181, "机巧", "工", "战斗开始时召唤一个木牛流马lv1（肉盾）", "职业", "", 1, 0f, 0f, 0, "", 0f, 0f, "", null, 0f, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "Dumb", "", "", 0f, "gong3", "", "", "");
            config[2000182] = new SkillConfig(2000182, "机巧", "工", "战斗开始时召唤一个木牛流马lv2（肉盾）", "职业", "", 2, 0f, 0f, 0, "", 0f, 0f, "", null, 0f, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "Dumb", "", "", 0f, "gong3", "", "", "");
            config[2000183] = new SkillConfig(2000183, "机巧", "工", "战斗开始时召唤一个木牛流马lv2与一个喷火兽lv1（远程火）", "职业", "", 3, 0f, 0f, 0, "", 0f, 0f, "", null, 0f, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "Dumb", "", "", 0f, "gong3", "", "", "");
            config[2000184] = new SkillConfig(2000184, "机巧", "工", "战斗开始时召唤两个木牛流马lv2（肉盾）", "职业", "", 4, 0f, 0f, 0, "", 0f, 0f, "", null, 0f, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "Dumb", "", "", 0f, "gong3", "", "", "");
            config[2000185] = new SkillConfig(2000185, "机巧", "工", "战斗开始时召唤两个木牛流马lv2与一个辅助（加buff）", "职业", "", 5, 0f, 0f, 0, "", 0f, 0f, "", null, 0f, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "Dumb", "", "", 0f, "gong3", "", "", "");
            config[2010001] = new SkillConfig(2010001, "突破", "破", "移动时穿越敌人，造成额外伤害", "连接", "", 1, 1f, 7f, 20, "", 0f, 0f, "atk", null, 0f, "", 0, 0f, 0, 0f, 0.5f, 0f, 0, 0f, 0, "", "", 301003, false, 0f, "", 0, 0f, 0f, 0f, 0f, "AttackRunCross", "", "LightningExplosionBlue", 0f, "po", "", "", "");
            config[2010002] = new SkillConfig(2010002, "突破", "破", "移动时穿越敌人，造成额外伤害", "连接", "", 2, 1f, 7f, 20, "", 0f, 0f, "atk", null, 0f, "", 0, 0f, 0, 0f, 0.5f, 0f, 0, 0f, 0, "", "", 301003, false, 0f, "", 0, 0f, 0f, 0f, 0f, "AttackRunCross", "", "LightningExplosionBlue", 0f, "po", "", "", "");
            config[2010003] = new SkillConfig(2010003, "突破", "破", "移动时穿越敌人，造成额外伤害", "连接", "", 3, 1f, 7f, 20, "", 0f, 0f, "atk", null, 0f, "", 0, 0f, 0, 0f, 0.5f, 0f, 0, 0f, 0, "", "", 301003, false, 0f, "", 0, 0f, 0f, 0f, 0f, "AttackRunCross", "", "LightningExplosionBlue", 0f, "po", "", "", "");
            config[2010004] = new SkillConfig(2010004, "突破", "破", "移动时穿越敌人，造成额外伤害", "连接", "", 4, 1f, 7f, 20, "", 0f, 0f, "atk", null, 0f, "", 0, 0f, 0, 0f, 0.5f, 0f, 0, 0f, 0, "", "", 301003, false, 0f, "", 0, 0f, 0f, 0f, 0f, "AttackRunCross", "", "LightningExplosionBlue", 0f, "po", "", "", "");
            config[2010005] = new SkillConfig(2010005, "突破", "破", "移动时穿越敌人，造成额外伤害", "连接", "", 5, 1f, 7f, 20, "", 0f, 0f, "atk", null, 0f, "", 0, 0f, 0, 0f, 0.5f, 0f, 0, 0f, 0, "", "", 301003, false, 0f, "", 0, 0f, 0f, 0f, 0f, "AttackRunCross", "", "LightningExplosionBlue", 0f, "po", "", "", "");
            config[2010006] = new SkillConfig(2010006, "冲锋", "冲", "移动时穿越敌人，降低目标防御", "连接", "", 1, 1f, 7f, 20, "", 0f, 0f, "atk", null, 0f, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 301003, false, 3f, "", 0, 0f, 0f, 0f, 0f, "AttackRunCrossPlus", "", "LightningExplosionRed", 0f, "chong", "", "", "");
            config[2010007] = new SkillConfig(2010007, "冲锋", "冲", "移动时穿越敌人，降低目标防御", "连接", "", 2, 1f, 7f, 20, "", 0f, 0f, "atk", null, 0f, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 301003, false, 3f, "", 0, 0f, 0f, 0f, 0f, "AttackRunCrossPlus", "", "LightningExplosionRed", 0f, "chong", "", "", "");
            config[2010008] = new SkillConfig(2010008, "冲锋", "冲", "移动时穿越敌人，降低目标防御", "连接", "", 3, 1f, 7f, 20, "", 0f, 0f, "atk", null, 0f, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 301003, false, 3f, "", 0, 0f, 0f, 0f, 0f, "AttackRunCrossPlus", "", "LightningExplosionRed", 0f, "chong", "", "", "");
            config[2010009] = new SkillConfig(2010009, "冲锋", "冲", "移动时穿越敌人，降低目标防御", "连接", "", 4, 1f, 7f, 20, "", 0f, 0f, "atk", null, 0f, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 301003, false, 3f, "", 0, 0f, 0f, 0f, 0f, "AttackRunCrossPlus", "", "LightningExplosionRed", 0f, "chong", "", "", "");
            config[2010010] = new SkillConfig(2010010, "冲锋", "冲", "移动时穿越敌人，降低目标防御", "连接", "", 5, 1f, 7f, 20, "", 0f, 0f, "atk", null, 0f, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 301003, false, 3f, "", 0, 0f, 0f, 0f, 0f, "AttackRunCrossPlus", "", "LightningExplosionRed", 0f, "chong", "", "", "");
            config[2010021] = new SkillConfig(2010021, "铁骑", "铁", "自己和同行[马车]技能附带混乱效果", "攻击up", "", 1, 0f, 0f, 0, "", 0f, 0f, "atk", null, 0f, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 1, "铁", "马车", 301001, false, 2f, "", 0, 0f, 0f, 0f, 0f, "BuffTieqi", "", "", 0f, "", "", "", "");
            config[2010022] = new SkillConfig(2010022, "铁骑", "铁", "自己和同行[马车]技能附带混乱效果", "攻击up", "", 2, 0f, 0f, 0, "", 0f, 0f, "atk", null, 0f, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 1, "铁", "马车", 301001, false, 2f, "", 0, 0f, 0f, 0f, 0f, "BuffTieqi", "", "", 0f, "", "", "", "");
            config[2010023] = new SkillConfig(2010023, "铁骑", "铁", "自己和同行[马车]技能附带混乱效果", "攻击up", "", 3, 0f, 0f, 0, "", 0f, 0f, "atk", null, 0f, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 1, "铁", "马车", 301001, false, 2f, "", 0, 0f, 0f, 0f, 0f, "BuffTieqi", "", "", 0f, "", "", "", "");
            config[2010024] = new SkillConfig(2010024, "铁骑", "铁", "自己和同行[马车]技能附带混乱效果", "攻击up", "", 4, 0f, 0f, 0, "", 0f, 0f, "atk", null, 0f, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 1, "铁", "马车", 301001, false, 2f, "", 0, 0f, 0f, 0f, 0f, "BuffTieqi", "", "", 0f, "", "", "", "");
            config[2010025] = new SkillConfig(2010025, "铁骑", "铁", "自己和同行[马车]技能附带混乱效果", "攻击up", "", 5, 0f, 0f, 0, "", 0f, 0f, "atk", null, 0f, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 1, "铁", "马车", 301001, false, 2f, "", 0, 0f, 0f, 0f, 0f, "BuffTieqi", "", "", 0f, "", "", "", "");
            config[2010031] = new SkillConfig(2010031, "奇袭", "奇", "自己和同行队友统王技能造成的混乱时间增加50%", "攻击up", "", 1, 0f, 0f, 0, "", 0f, 0f, "atk", new string[]{"atk"}, 0f, "", 0, 0.5f, 0, 0f, 0f, 0f, 0, 0f, 1, "奇", "", 301001, true, 0f, "", 0, 0f, 0f, 0f, 0f, "ModifyBuffTime", "", "", 0f, "", "", "", "");
            config[2010032] = new SkillConfig(2010032, "奇袭", "奇", "自己和同行队友统王技能造成的混乱时间增加50%", "攻击up", "", 2, 0f, 0f, 0, "", 0f, 0f, "atk", new string[]{"atk"}, 0f, "", 0, 0.5f, 0, 0f, 0f, 0f, 0, 0f, 1, "奇", "", 301001, true, 0f, "", 0, 0f, 0f, 0f, 0f, "ModifyBuffTime", "", "", 0f, "", "", "", "");
            config[2010033] = new SkillConfig(2010033, "奇袭", "奇", "自己和同行队友统王技能造成的混乱时间增加50%", "攻击up", "", 3, 0f, 0f, 0, "", 0f, 0f, "atk", new string[]{"atk"}, 0f, "", 0, 0.5f, 0, 0f, 0f, 0f, 0, 0f, 1, "奇", "", 301001, true, 0f, "", 0, 0f, 0f, 0f, 0f, "ModifyBuffTime", "", "", 0f, "", "", "", "");
            config[2010034] = new SkillConfig(2010034, "奇袭", "奇", "自己和同行队友统王技能造成的混乱时间增加50%", "攻击up", "", 4, 0f, 0f, 0, "", 0f, 0f, "atk", new string[]{"atk"}, 0f, "", 0, 0.5f, 0, 0f, 0f, 0f, 0, 0f, 1, "奇", "", 301001, true, 0f, "", 0, 0f, 0f, 0f, 0f, "ModifyBuffTime", "", "", 0f, "", "", "", "");
            config[2010035] = new SkillConfig(2010035, "奇袭", "奇", "自己和同行队友统王技能造成的混乱时间增加50%", "攻击up", "", 5, 0f, 0f, 0, "", 0f, 0f, "atk", new string[]{"atk"}, 0f, "", 0, 0.5f, 0, 0f, 0f, 0f, 0, 0f, 1, "奇", "", 301001, true, 0f, "", 0, 0f, 0f, 0f, 0f, "ModifyBuffTime", "", "", 0f, "", "", "", "");
            config[2010061] = new SkillConfig(2010061, "连击", "连", "攻击时几率触发连续攻击", "攻击up", "", 1, 0.3f, 5f, 20, "", 0f, 0f, "atk", null, 0f, "", 0, 0.7f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "AttackSpeedAttack", "flipspin", "", 0f, "", "", "", "");
            config[2010062] = new SkillConfig(2010062, "连击", "连", "攻击时几率触发连续攻击", "攻击up", "", 2, 0.3f, 5f, 20, "", 0f, 0f, "atk", null, 0f, "", 0, 0.7f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "AttackSpeedAttack", "flipspin", "", 0f, "", "", "", "");
            config[2010063] = new SkillConfig(2010063, "连击", "连", "攻击时几率触发连续攻击", "攻击up", "", 3, 0.3f, 5f, 20, "", 0f, 0f, "atk", null, 0f, "", 0, 0.7f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "AttackSpeedAttack", "flipspin", "", 0f, "", "", "", "");
            config[2010064] = new SkillConfig(2010064, "连击", "连", "攻击时几率触发连续攻击", "攻击up", "", 4, 0.3f, 5f, 20, "", 0f, 0f, "atk", null, 0f, "", 0, 0.7f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "AttackSpeedAttack", "flipspin", "", 0f, "", "", "", "");
            config[2010065] = new SkillConfig(2010065, "连击", "连", "攻击时几率触发连续攻击", "攻击up", "", 5, 0.3f, 5f, 20, "", 0f, 0f, "atk", null, 0f, "", 0, 0.7f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "AttackSpeedAttack", "flipspin", "", 0f, "", "", "", "");
            config[2010071] = new SkillConfig(2010071, "速射", "速", "自己和同行[弓弩]箭矢飞行速度提升", "攻击up", "", 1, 0f, 0f, 0, "", 0f, 0f, "atk", null, 0f, "", 0, 2.5f, 0, 0f, 0f, 0f, 0, 0f, 1, "速", "弓弩", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "ModifyShootSpeed", "", "", 0f, "", "", "", "");
            config[2010072] = new SkillConfig(2010072, "速射", "速", "自己和同行[弓弩]箭矢飞行速度提升", "攻击up", "", 2, 0f, 0f, 0, "", 0f, 0f, "atk", null, 0f, "", 0, 2.5f, 0, 0f, 0f, 0f, 0, 0f, 1, "速", "弓弩", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "ModifyShootSpeed", "", "", 0f, "", "", "", "");
            config[2010073] = new SkillConfig(2010073, "速射", "速", "自己和同行[弓弩]箭矢飞行速度提升", "攻击up", "", 3, 0f, 0f, 0, "", 0f, 0f, "atk", null, 0f, "", 0, 2.5f, 0, 0f, 0f, 0f, 0, 0f, 1, "速", "弓弩", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "ModifyShootSpeed", "", "", 0f, "", "", "", "");
            config[2010074] = new SkillConfig(2010074, "速射", "速", "自己和同行[弓弩]箭矢飞行速度提升", "攻击up", "", 4, 0f, 0f, 0, "", 0f, 0f, "atk", null, 0f, "", 0, 2.5f, 0, 0f, 0f, 0f, 0, 0f, 1, "速", "弓弩", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "ModifyShootSpeed", "", "", 0f, "", "", "", "");
            config[2010075] = new SkillConfig(2010075, "速射", "速", "自己和同行[弓弩]箭矢飞行速度提升", "攻击up", "", 5, 0f, 0f, 0, "", 0f, 0f, "atk", null, 0f, "", 0, 2.5f, 0, 0f, 0f, 0f, 0, 0f, 1, "速", "弓弩", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "ModifyShootSpeed", "", "", 0f, "", "", "", "");
            config[2010081] = new SkillConfig(2010081, "箭雨", "雨", "攻击时30%发出2只箭", "", "", 1, 0.3f, 4f, 20, "", 0f, 0f, "atk", null, 30f, "", 1, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "AttackMultiArrow", "flipspin", "", 0f, "", "", "", "");
            config[2010082] = new SkillConfig(2010082, "箭雨", "雨", "攻击时30%发出2只箭", "", "", 2, 0.3f, 4f, 20, "", 0f, 0f, "atk", null, 30f, "", 1, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "AttackMultiArrow", "flipspin", "", 0f, "", "", "", "");
            config[2010083] = new SkillConfig(2010083, "箭雨", "雨", "攻击时30%发出2只箭", "", "", 3, 0.3f, 4f, 20, "", 0f, 0f, "atk", null, 30f, "", 1, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "AttackMultiArrow", "flipspin", "", 0f, "", "", "", "");
            config[2010084] = new SkillConfig(2010084, "箭雨", "雨", "攻击时30%发出2只箭", "", "", 4, 0.3f, 4f, 20, "", 0f, 0f, "atk", null, 30f, "", 1, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "AttackMultiArrow", "flipspin", "", 0f, "", "", "", "");
            config[2010085] = new SkillConfig(2010085, "箭雨", "雨", "攻击时30%发出2只箭", "", "", 5, 0.3f, 4f, 20, "", 0f, 0f, "atk", null, 30f, "", 1, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "AttackMultiArrow", "flipspin", "", 0f, "", "", "", "");
            config[2010091] = new SkillConfig(2010091, "共杀", "共", "击中目标时触发2次弹射", "", "", 1, 0.35f, 5f, 20, "", 0f, 0f, "ap", null, 30f, "", 3, 0f, 0, 0f, 0.25f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "AttackReboundArrow", "flipspin", "", 0f, "", "", "", "");
            config[2010092] = new SkillConfig(2010092, "共杀", "共", "击中目标时触发2次弹射", "", "", 2, 0.35f, 5f, 20, "", 0f, 0f, "ap", null, 30f, "", 3, 0f, 0, 0f, 0.25f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "AttackReboundArrow", "flipspin", "", 0f, "", "", "", "");
            config[2010093] = new SkillConfig(2010093, "共杀", "共", "击中目标时触发2次弹射", "", "", 3, 0.35f, 5f, 20, "", 0f, 0f, "ap", null, 30f, "", 3, 0f, 0, 0f, 0.25f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "AttackReboundArrow", "flipspin", "", 0f, "", "", "", "");
            config[2010094] = new SkillConfig(2010094, "共杀", "共", "击中目标时触发2次弹射", "", "", 4, 0.35f, 5f, 20, "", 0f, 0f, "ap", null, 30f, "", 3, 0f, 0, 0f, 0.25f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "AttackReboundArrow", "flipspin", "", 0f, "", "", "", "");
            config[2010095] = new SkillConfig(2010095, "共杀", "共", "击中目标时触发2次弹射", "", "", 5, 0.35f, 5f, 20, "", 0f, 0f, "ap", null, 30f, "", 3, 0f, 0, 0f, 0.25f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "AttackReboundArrow", "flipspin", "", 0f, "", "", "", "");
            config[2010101] = new SkillConfig(2010101, "旋风斩", "旋", "攻击时几率对附近敌人造成伤害", "技", "", 1, 0.4f, 5f, 20, "", 0f, 0f, "atk", null, 25f, "", 5, 0f, 0, 0f, 0.8f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "AttackSpinAttack", "spin", "SwordWhirlwindWhite", 0f, "", "", "", "");
            config[2010102] = new SkillConfig(2010102, "旋风斩", "旋", "攻击时几率对附近敌人造成伤害", "技", "", 2, 0.4f, 5f, 20, "", 0f, 0f, "atk", null, 25f, "", 5, 0f, 0, 0f, 0.8f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "AttackSpinAttack", "spin", "SwordWhirlwindWhite", 0f, "", "", "", "");
            config[2010103] = new SkillConfig(2010103, "旋风斩", "旋", "攻击时几率对附近敌人造成伤害", "技", "", 3, 0.4f, 5f, 20, "", 0f, 0f, "atk", null, 25f, "", 5, 0f, 0, 0f, 0.8f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "AttackSpinAttack", "spin", "SwordWhirlwindWhite", 0f, "", "", "", "");
            config[2010104] = new SkillConfig(2010104, "旋风斩", "旋", "攻击时几率对附近敌人造成伤害", "技", "", 4, 0.4f, 5f, 20, "", 0f, 0f, "atk", null, 25f, "", 5, 0f, 0, 0f, 0.8f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "AttackSpinAttack", "spin", "SwordWhirlwindWhite", 0f, "", "", "", "");
            config[2010105] = new SkillConfig(2010105, "旋风斩", "旋", "攻击时几率对附近敌人造成伤害", "技", "", 5, 0.4f, 5f, 20, "", 0f, 0f, "atk", null, 25f, "", 5, 0f, 0, 0f, 0.8f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "AttackSpinAttack", "spin", "SwordWhirlwindWhite", 0f, "", "", "", "");
            config[2010111] = new SkillConfig(2010111, "落雷", "天", "攻击召唤出持续伤害的雷电阵", "术", "", 1, 0.3f, 10f, 20, "", 0f, 0f, "ap", null, 0f, "", 1, 10f, 0, 0f, 0f, 0.15f, 0, 0f, 0, "", "", 0, false, 0f, "雷", 0, 25f, 5.2f, 0.5f, 0f, "HitRegion", "spin", "SummonStorm", 5f, "", "", "", "");
            config[2010112] = new SkillConfig(2010112, "落雷", "天", "攻击召唤出持续伤害的雷电阵", "术", "", 2, 0.3f, 10f, 20, "", 0f, 0f, "ap", null, 0f, "", 1, 15f, 0, 0f, 0f, 0.15f, 0, 0f, 0, "", "", 0, false, 0f, "雷", 0, 25f, 5.2f, 0.5f, 0f, "HitRegion", "spin", "SummonStorm", 5f, "", "", "", "");
            config[2010113] = new SkillConfig(2010113, "落雷", "天", "攻击召唤出持续伤害的雷电阵", "术", "", 3, 0.3f, 10f, 20, "", 0f, 0f, "ap", null, 0f, "", 1, 20f, 0, 0f, 0f, 0.15f, 0, 0f, 0, "", "", 0, false, 0f, "雷", 0, 25f, 5.2f, 0.5f, 0f, "HitRegion", "spin", "SummonStorm", 5f, "", "", "", "");
            config[2010114] = new SkillConfig(2010114, "落雷", "天", "攻击召唤出持续伤害的雷电阵", "术", "", 4, 0.3f, 10f, 20, "", 0f, 0f, "ap", null, 0f, "", 1, 25f, 0, 0f, 0f, 0.15f, 0, 0f, 0, "", "", 0, false, 0f, "雷", 0, 25f, 5.2f, 0.5f, 0f, "HitRegion", "spin", "SummonStorm", 5f, "", "", "", "");
            config[2010115] = new SkillConfig(2010115, "落雷", "天", "攻击召唤出持续伤害的雷电阵", "术", "", 5, 0.3f, 10f, 20, "", 0f, 0f, "ap", null, 0f, "", 1, 30f, 0, 0f, 0f, 0.15f, 0, 0f, 0, "", "", 0, false, 0f, "雷", 0, 25f, 5.2f, 0.5f, 0f, "HitRegion", "spin", "SummonStorm", 5f, "", "", "", "");
            config[2010121] = new SkillConfig(2010121, "火计", "火", "攻击时对目标放火", "术", "", 1, 0.3f, 5f, 20, "", 0f, 0f, "ap", null, 0f, "", 1, 10f, 0, 0f, 0f, 0.1f, 0, 0f, 0, "", "", 0, false, 0f, "火", 1, 8f, 3.2f, 1f, 0f, "HitWall", "throw", "SoftFireBigRed", 1.6f, "", "", "", "");
            config[2010122] = new SkillConfig(2010122, "火计", "火", "攻击时对目标放火", "术", "", 2, 0.3f, 5f, 20, "", 0f, 0f, "ap", null, 0f, "", 1, 15f, 0, 0f, 0f, 0.1f, 0, 0f, 0, "", "", 0, false, 0f, "火", 1, 8f, 3.2f, 1f, 0f, "HitWall", "throw", "SoftFireBigRed", 1.6f, "", "", "", "");
            config[2010123] = new SkillConfig(2010123, "火计", "火", "攻击时对目标放火", "术", "", 3, 0.3f, 5f, 20, "", 0f, 0f, "ap", null, 0f, "", 1, 20f, 0, 0f, 0f, 0.1f, 0, 0f, 0, "", "", 0, false, 0f, "火", 1, 8f, 3.2f, 1f, 0f, "HitWall", "throw", "SoftFireBigRed", 1.6f, "", "", "", "");
            config[2010124] = new SkillConfig(2010124, "火计", "火", "攻击时对目标放火", "术", "", 4, 0.3f, 5f, 20, "", 0f, 0f, "ap", null, 0f, "", 1, 25f, 0, 0f, 0f, 0.1f, 0, 0f, 0, "", "", 0, false, 0f, "火", 1, 8f, 3.2f, 1f, 0f, "HitWall", "throw", "SoftFireBigRed", 1.6f, "", "", "", "");
            config[2010125] = new SkillConfig(2010125, "火计", "火", "攻击时对目标放火", "术", "", 5, 0.3f, 5f, 20, "", 0f, 0f, "ap", null, 0f, "", 1, 30f, 0, 0f, 0f, 0.1f, 0, 0f, 0, "", "", 0, false, 0f, "火", 1, 8f, 3.2f, 1f, 0f, "HitWall", "throw", "SoftFireBigRed", 1.6f, "", "", "", "");
            config[2010131] = new SkillConfig(2010131, "火墙", "炎", "攻击召唤出持续伤害的火墙", "术", "", 1, 0.3f, 8.5f, 20, "", 0f, 0f, "ap", null, 0f, "", 1, 10f, 0, 0f, 0f, 0.08f, 0, 0f, 0, "", "", 0, false, 0f, "火", 5, 8f, 3.2f, 1f, 0f, "HitWall", "spin", "SoftFireBigRed", 1.6f, "", "", "", "");
            config[2010132] = new SkillConfig(2010132, "火墙", "炎", "攻击召唤出持续伤害的火墙", "术", "", 2, 0.3f, 8.5f, 20, "", 0f, 0f, "ap", null, 0f, "", 1, 15f, 0, 0f, 0f, 0.08f, 0, 0f, 0, "", "", 0, false, 0f, "火", 5, 8f, 3.2f, 1f, 0f, "HitWall", "spin", "SoftFireBigRed", 1.6f, "", "", "", "");
            config[2010133] = new SkillConfig(2010133, "火墙", "炎", "攻击召唤出持续伤害的火墙", "术", "", 3, 0.3f, 8.5f, 20, "", 0f, 0f, "ap", null, 0f, "", 1, 20f, 0, 0f, 0f, 0.08f, 0, 0f, 0, "", "", 0, false, 0f, "火", 5, 8f, 3.2f, 1f, 0f, "HitWall", "spin", "SoftFireBigRed", 1.6f, "", "", "", "");
            config[2010134] = new SkillConfig(2010134, "火墙", "炎", "攻击召唤出持续伤害的火墙", "术", "", 4, 0.3f, 8.5f, 20, "", 0f, 0f, "ap", null, 0f, "", 1, 25f, 0, 0f, 0f, 0.08f, 0, 0f, 0, "", "", 0, false, 0f, "火", 5, 8f, 3.2f, 1f, 0f, "HitWall", "spin", "SoftFireBigRed", 1.6f, "", "", "", "");
            config[2010135] = new SkillConfig(2010135, "火墙", "炎", "攻击召唤出持续伤害的火墙", "术", "", 5, 0.3f, 8.5f, 20, "", 0f, 0f, "ap", null, 0f, "", 1, 30f, 0, 0f, 0f, 0.08f, 0, 0f, 0, "", "", 0, false, 0f, "火", 5, 8f, 3.2f, 1f, 0f, "HitWall", "spin", "SoftFireBigRed", 1.6f, "", "", "", "");
            config[2010141] = new SkillConfig(2010141, "飞斧", "斧", "扔出飞斧攻击前方敌人", "技", "", 1, 1f, 6.7f, 20, "", 2f, 0f, "atk", null, 45f, "", 4, 20f, 0, 0f, 0f, 0.4f, 0, 0f, 0, "", "", 0, false, 0f, "武", 0, 9f, 1.5f, 0f, 40f, "AidShockWave", "spin", "AxeExplosion", 3f, "", "", "", "");
            config[2010142] = new SkillConfig(2010142, "飞斧", "斧", "扔出飞斧攻击前方敌人", "技", "", 2, 1f, 6.7f, 20, "", 2f, 0f, "atk", null, 45f, "", 4, 30f, 0, 0f, 0f, 0.4f, 0, 0f, 0, "", "", 0, false, 0f, "武", 0, 9f, 1.5f, 0f, 40f, "AidShockWave", "spin", "AxeExplosion", 3f, "", "", "", "");
            config[2010143] = new SkillConfig(2010143, "飞斧", "斧", "扔出飞斧攻击前方敌人", "技", "", 3, 1f, 6.7f, 20, "", 2f, 0f, "atk", null, 45f, "", 4, 40f, 0, 0f, 0f, 0.4f, 0, 0f, 0, "", "", 0, false, 0f, "武", 0, 9f, 1.5f, 0f, 40f, "AidShockWave", "spin", "AxeExplosion", 3f, "", "", "", "");
            config[2010144] = new SkillConfig(2010144, "飞斧", "斧", "扔出飞斧攻击前方敌人", "技", "", 4, 1f, 6.7f, 20, "", 2f, 0f, "atk", null, 45f, "", 4, 50f, 0, 0f, 0f, 0.4f, 0, 0f, 0, "", "", 0, false, 0f, "武", 0, 9f, 1.5f, 0f, 40f, "AidShockWave", "spin", "AxeExplosion", 3f, "", "", "", "");
            config[2010145] = new SkillConfig(2010145, "飞斧", "斧", "扔出飞斧攻击前方敌人", "技", "", 5, 1f, 6.7f, 20, "", 2f, 0f, "atk", null, 45f, "", 4, 60f, 0, 0f, 0f, 0.4f, 0, 0f, 0, "", "", 0, false, 0f, "武", 0, 9f, 1.5f, 0f, 40f, "AidShockWave", "spin", "AxeExplosion", 3f, "", "", "", "");
            config[2010151] = new SkillConfig(2010151, "驰羽", "羽", "能够射出箭矢", "技", "", 1, 1f, 7f, 20, "", 2f, 0f, "atk", null, 42f, "", 0, 0f, 0, 0f, 0f, 0.75f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "AidSuddenArrow", "sway", "BulletExplosionBlue", 3f, "", "", "", "");
            config[2010152] = new SkillConfig(2010152, "驰羽", "羽", "能够射出箭矢", "技", "", 2, 1f, 7f, 20, "", 2f, 0f, "atk", null, 42f, "", 0, 0f, 0, 0f, 0f, 0.75f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "AidSuddenArrow", "sway", "BulletExplosionBlue", 3f, "", "", "", "");
            config[2010153] = new SkillConfig(2010153, "驰羽", "羽", "能够射出箭矢", "技", "", 3, 1f, 7f, 20, "", 2f, 0f, "atk", null, 42f, "", 0, 0f, 0, 0f, 0f, 0.75f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "AidSuddenArrow", "sway", "BulletExplosionBlue", 3f, "", "", "", "");
            config[2010154] = new SkillConfig(2010154, "驰羽", "羽", "能够射出箭矢", "技", "", 4, 1f, 7f, 20, "", 2f, 0f, "atk", null, 42f, "", 0, 0f, 0, 0f, 0f, 0.75f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "AidSuddenArrow", "sway", "BulletExplosionBlue", 3f, "", "", "", "");
            config[2010155] = new SkillConfig(2010155, "驰羽", "羽", "能够射出箭矢", "技", "", 5, 1f, 7f, 20, "", 2f, 0f, "atk", null, 42f, "", 0, 0f, 0, 0f, 0f, 0.75f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "AidSuddenArrow", "sway", "BulletExplosionBlue", 3f, "", "", "", "");
            config[2010161] = new SkillConfig(2010161, "惊雷", "雷", "召唤3个惊雷攻击前方敌人", "术", "", 1, 1f, 6.7f, 20, "", 2f, 0f, "ap", null, 60f, "", 4, 15f, 0, 0f, 0f, 0.25f, 0, 0f, 0, "", "", 0, false, 0f, "雷", 0, 11f, 1.5f, 0f, 40f, "AidShockWave", "spin", "NukeMissileFires", 4f, "", "", "", "");
            config[2010162] = new SkillConfig(2010162, "惊雷", "雷", "召唤3个惊雷攻击前方敌人", "术", "", 2, 1f, 6.7f, 20, "", 2f, 0f, "ap", null, 60f, "", 4, 20f, 0, 0f, 0f, 0.25f, 0, 0f, 0, "", "", 0, false, 0f, "雷", 0, 11f, 1.5f, 0f, 40f, "AidShockWave", "spin", "NukeMissileFires", 4f, "", "", "", "");
            config[2010163] = new SkillConfig(2010163, "惊雷", "雷", "召唤3个惊雷攻击前方敌人", "术", "", 3, 1f, 6.7f, 20, "", 2f, 0f, "ap", null, 60f, "", 4, 25f, 0, 0f, 0f, 0.25f, 0, 0f, 0, "", "", 0, false, 0f, "雷", 0, 11f, 1.5f, 0f, 40f, "AidShockWave", "spin", "NukeMissileFires", 4f, "", "", "", "");
            config[2010164] = new SkillConfig(2010164, "惊雷", "雷", "召唤3个惊雷攻击前方敌人", "术", "", 4, 1f, 6.7f, 20, "", 2f, 0f, "ap", null, 60f, "", 4, 30f, 0, 0f, 0f, 0.25f, 0, 0f, 0, "", "", 0, false, 0f, "雷", 0, 11f, 1.5f, 0f, 40f, "AidShockWave", "spin", "NukeMissileFires", 4f, "", "", "", "");
            config[2010165] = new SkillConfig(2010165, "惊雷", "雷", "召唤3个惊雷攻击前方敌人", "术", "", 5, 1f, 6.7f, 20, "", 2f, 0f, "ap", null, 60f, "", 4, 35f, 0, 0f, 0f, 0.25f, 0, 0f, 0, "", "", 0, false, 0f, "雷", 0, 11f, 1.5f, 0f, 40f, "AidShockWave", "spin", "NukeMissileFires", 4f, "", "", "", "");
            config[2010171] = new SkillConfig(2010171, "鬼谋", "鬼", "攻击造成生命值比例伤害", "", "", 1, 0.25f, 2f, 20, "", 0f, 1f, "ap", null, 0f, "", 0, 0.2f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "DamageReal", "sway", "", 0f, "", "", "", "");
            config[2010172] = new SkillConfig(2010172, "鬼谋", "鬼", "攻击造成生命值比例伤害", "", "", 2, 0.25f, 2f, 20, "", 0f, 1f, "ap", null, 0f, "", 0, 0.2f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "DamageReal", "sway", "", 0f, "", "", "", "");
            config[2010173] = new SkillConfig(2010173, "鬼谋", "鬼", "攻击造成生命值比例伤害", "", "", 3, 0.25f, 2f, 20, "", 0f, 1f, "ap", null, 0f, "", 0, 0.2f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "DamageReal", "sway", "", 0f, "", "", "", "");
            config[2010174] = new SkillConfig(2010174, "鬼谋", "鬼", "攻击造成生命值比例伤害", "", "", 4, 0.25f, 2f, 20, "", 0f, 1f, "ap", null, 0f, "", 0, 0.2f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "DamageReal", "sway", "", 0f, "", "", "", "");
            config[2010175] = new SkillConfig(2010175, "鬼谋", "鬼", "攻击造成生命值比例伤害", "", "", 5, 0.25f, 2f, 20, "", 0f, 1f, "ap", null, 0f, "", 0, 0.2f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "DamageReal", "sway", "", 0f, "", "", "", "");
            config[2010181] = new SkillConfig(2010181, "斩", "斩", "直接杀死低生命值单位", "", "", 1, 0f, 7f, 20, "", 0f, 0.3f, "atk", null, 0f, "", 0, 1f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "DamageReal", "spin", "SwordHitRedCritical", 0f, "", "", "", "");
            config[2010182] = new SkillConfig(2010182, "斩", "斩", "直接杀死低生命值单位", "", "", 2, 0f, 7f, 20, "", 0f, 0.3f, "atk", null, 0f, "", 0, 1f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "DamageReal", "spin", "SwordHitRedCritical", 0f, "", "", "", "");
            config[2010183] = new SkillConfig(2010183, "斩", "斩", "直接杀死低生命值单位", "", "", 3, 0f, 7f, 20, "", 0f, 0.3f, "atk", null, 0f, "", 0, 1f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "DamageReal", "spin", "SwordHitRedCritical", 0f, "", "", "", "");
            config[2010184] = new SkillConfig(2010184, "斩", "斩", "直接杀死低生命值单位", "", "", 4, 0f, 7f, 20, "", 0f, 0.3f, "atk", null, 0f, "", 0, 1f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "DamageReal", "spin", "SwordHitRedCritical", 0f, "", "", "", "");
            config[2010185] = new SkillConfig(2010185, "斩", "斩", "直接杀死低生命值单位", "", "", 5, 0f, 7f, 20, "", 0f, 0.3f, "atk", null, 0f, "", 0, 1f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "DamageReal", "spin", "SwordHitRedCritical", 0f, "", "", "", "");
            config[2010191] = new SkillConfig(2010191, "魔神", "魔", "攻击时回复生命", "", "", 1, 0.35f, 6f, 0, "", 0f, 0f, "atk", null, 0f, "", 0, 0f, 0, 0f, 1f, 0f, 0, 0f, 0, "", "", 300003, false, 5f, "", 0, 0f, 0f, 0f, 0f, "AttackedBuff", "", "MagicBuffGreen", 0f, "", "", "", "");
            config[2010192] = new SkillConfig(2010192, "魔神", "魔", "攻击时回复生命", "", "", 2, 0.35f, 6f, 0, "", 0f, 0f, "atk", null, 0f, "", 0, 0f, 0, 0f, 1f, 0f, 0, 0f, 0, "", "", 300003, false, 5f, "", 0, 0f, 0f, 0f, 0f, "AttackedBuff", "", "MagicBuffGreen", 0f, "", "", "", "");
            config[2010193] = new SkillConfig(2010193, "魔神", "魔", "攻击时回复生命", "", "", 3, 0.35f, 6f, 0, "", 0f, 0f, "atk", null, 0f, "", 0, 0f, 0, 0f, 1f, 0f, 0, 0f, 0, "", "", 300003, false, 5f, "", 0, 0f, 0f, 0f, 0f, "AttackedBuff", "", "MagicBuffGreen", 0f, "", "", "", "");
            config[2010194] = new SkillConfig(2010194, "魔神", "魔", "攻击时回复生命", "", "", 4, 0.35f, 6f, 0, "", 0f, 0f, "atk", null, 0f, "", 0, 0f, 0, 0f, 1f, 0f, 0, 0f, 0, "", "", 300003, false, 5f, "", 0, 0f, 0f, 0f, 0f, "AttackedBuff", "", "MagicBuffGreen", 0f, "", "", "", "");
            config[2010195] = new SkillConfig(2010195, "魔神", "魔", "攻击时回复生命", "", "", 5, 0.35f, 6f, 0, "", 0f, 0f, "atk", null, 0f, "", 0, 0f, 0, 0f, 1f, 0f, 0, 0f, 0, "", "", 300003, false, 5f, "", 0, 0f, 0f, 0f, 0f, "AttackedBuff", "", "MagicBuffGreen", 0f, "", "", "", "");
            config[2010201] = new SkillConfig(2010201, "埋伏", "伏", "被攻击时瞬移到远程攻击者附近", "", "", 1, 1f, 6f, 0, "", 0f, 0f, "atk", null, 30f, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 301001, false, 1f, "", 0, 0f, 0f, 0f, 0f, "HitTeleport", "saw", "MagicNovaBlue", 0f, "", "", "", "");
            config[2010202] = new SkillConfig(2010202, "埋伏", "伏", "被攻击时瞬移到远程攻击者附近", "", "", 2, 1f, 6f, 0, "", 0f, 0f, "atk", null, 30f, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 301001, false, 1f, "", 0, 0f, 0f, 0f, 0f, "HitTeleport", "saw", "MagicNovaBlue", 0f, "", "", "", "");
            config[2010203] = new SkillConfig(2010203, "埋伏", "伏", "被攻击时瞬移到远程攻击者附近", "", "", 3, 1f, 6f, 0, "", 0f, 0f, "atk", null, 30f, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 301001, false, 1f, "", 0, 0f, 0f, 0f, 0f, "HitTeleport", "saw", "MagicNovaBlue", 0f, "", "", "", "");
            config[2010204] = new SkillConfig(2010204, "埋伏", "伏", "被攻击时瞬移到远程攻击者附近", "", "", 4, 1f, 6f, 0, "", 0f, 0f, "atk", null, 30f, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 301001, false, 1f, "", 0, 0f, 0f, 0f, 0f, "HitTeleport", "saw", "MagicNovaBlue", 0f, "", "", "", "");
            config[2010205] = new SkillConfig(2010205, "埋伏", "伏", "被攻击时瞬移到远程攻击者附近", "", "", 5, 1f, 6f, 0, "", 0f, 0f, "atk", null, 30f, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 301001, false, 1f, "", 0, 0f, 0f, 0f, 0f, "HitTeleport", "saw", "MagicNovaBlue", 0f, "", "", "", "");
            config[2010211] = new SkillConfig(2010211, "火矢", "矢", "攻击时射出火箭", "技", "", 1, 0.35f, 3f, 20, "", 0f, 0f, "atk", null, 0f, "", 1, 10f, 0, 0f, 0f, 0.12f, 0, 0f, 0, "", "", 0, false, 0f, "火", 1, 8f, 3.2f, 1f, 0f, "HitWall", "throw", "SoftFireBigRed", 1.6f, "", "", "", "");
            config[2010212] = new SkillConfig(2010212, "火矢", "矢", "攻击时射出火箭", "技", "", 2, 0.35f, 3f, 20, "", 0f, 0f, "atk", null, 0f, "", 1, 15f, 0, 0f, 0f, 0.12f, 0, 0f, 0, "", "", 0, false, 0f, "火", 1, 8f, 3.2f, 1f, 0f, "HitWall", "throw", "SoftFireBigRed", 1.6f, "", "", "", "");
            config[2010213] = new SkillConfig(2010213, "火矢", "矢", "攻击时射出火箭", "技", "", 3, 0.35f, 3f, 20, "", 0f, 0f, "atk", null, 0f, "", 1, 20f, 0, 0f, 0f, 0.12f, 0, 0f, 0, "", "", 0, false, 0f, "火", 1, 8f, 3.2f, 1f, 0f, "HitWall", "throw", "SoftFireBigRed", 1.6f, "", "", "", "");
            config[2010214] = new SkillConfig(2010214, "火矢", "矢", "攻击时射出火箭", "技", "", 4, 0.35f, 3f, 20, "", 0f, 0f, "atk", null, 0f, "", 1, 25f, 0, 0f, 0f, 0.12f, 0, 0f, 0, "", "", 0, false, 0f, "火", 1, 8f, 3.2f, 1f, 0f, "HitWall", "throw", "SoftFireBigRed", 1.6f, "", "", "", "");
            config[2010215] = new SkillConfig(2010215, "火矢", "矢", "攻击时射出火箭", "技", "", 5, 0.35f, 3f, 20, "", 0f, 0f, "atk", null, 0f, "", 1, 30f, 0, 0f, 0f, 0.12f, 0, 0f, 0, "", "", 0, false, 0f, "火", 1, 8f, 3.2f, 1f, 0f, "HitWall", "throw", "SoftFireBigRed", 1.6f, "", "", "", "");
            config[2010221] = new SkillConfig(2010221, "虎卫队", "虎", "攻击时几率对目标进行三连击", "技", "", 1, 0.15f, 6f, 20, "", 0f, 0f, "atk", null, 0f, "", 0, 0f, 0, 0f, 1f, 0f, 2, 0.3f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "HitRepeat", "jumpspin", "SwordHitRedCritical", 0f, "", "", "", "");
            config[2010222] = new SkillConfig(2010222, "虎卫队", "虎", "攻击时几率对目标进行三连击", "技", "", 2, 0.15f, 6f, 20, "", 0f, 0f, "atk", null, 0f, "", 0, 0f, 0, 0f, 1f, 0f, 2, 0.3f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "HitRepeat", "jumpspin", "SwordHitRedCritical", 0f, "", "", "", "");
            config[2010223] = new SkillConfig(2010223, "虎卫队", "虎", "攻击时几率对目标进行三连击", "技", "", 3, 0.15f, 6f, 20, "", 0f, 0f, "atk", null, 0f, "", 0, 0f, 0, 0f, 1f, 0f, 2, 0.3f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "HitRepeat", "jumpspin", "SwordHitRedCritical", 0f, "", "", "", "");
            config[2010224] = new SkillConfig(2010224, "虎卫队", "虎", "攻击时几率对目标进行三连击", "技", "", 4, 0.15f, 6f, 20, "", 0f, 0f, "atk", null, 0f, "", 0, 0f, 0, 0f, 1f, 0f, 2, 0.3f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "HitRepeat", "jumpspin", "SwordHitRedCritical", 0f, "", "", "", "");
            config[2010225] = new SkillConfig(2010225, "虎卫队", "虎", "攻击时几率对目标进行三连击", "技", "", 5, 0.15f, 6f, 20, "", 0f, 0f, "atk", null, 0f, "", 0, 0f, 0, 0f, 1f, 0f, 2, 0.3f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "HitRepeat", "jumpspin", "SwordHitRedCritical", 0f, "", "", "", "");
            config[2010231] = new SkillConfig(2010231, "青州兵", "青", "攻击时几率对目标进行2连击", "技", "", 1, 0.15f, 7f, 20, "", 0f, 0f, "atk", null, 0f, "", 0, 0f, 0, 0f, 1f, 0f, 1, 0.4f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "HitRepeat", "jumpspin", "SwordHitGreenCritical", 0f, "", "", "", "");
            config[2010232] = new SkillConfig(2010232, "青州兵", "青", "攻击时几率对目标进行2连击", "技", "", 2, 0.15f, 7f, 20, "", 0f, 0f, "atk", null, 0f, "", 0, 0f, 0, 0f, 1f, 0f, 1, 0.4f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "HitRepeat", "jumpspin", "SwordHitGreenCritical", 0f, "", "", "", "");
            config[2010233] = new SkillConfig(2010233, "青州兵", "青", "攻击时几率对目标进行2连击", "技", "", 3, 0.15f, 7f, 20, "", 0f, 0f, "atk", null, 0f, "", 0, 0f, 0, 0f, 1f, 0f, 1, 0.4f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "HitRepeat", "jumpspin", "SwordHitGreenCritical", 0f, "", "", "", "");
            config[2010234] = new SkillConfig(2010234, "青州兵", "青", "攻击时几率对目标进行2连击", "技", "", 4, 0.15f, 7f, 20, "", 0f, 0f, "atk", null, 0f, "", 0, 0f, 0, 0f, 1f, 0f, 1, 0.4f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "HitRepeat", "jumpspin", "SwordHitGreenCritical", 0f, "", "", "", "");
            config[2010235] = new SkillConfig(2010235, "青州兵", "青", "攻击时几率对目标进行2连击", "技", "", 5, 0.15f, 7f, 20, "", 0f, 0f, "atk", null, 0f, "", 0, 0f, 0, 0f, 1f, 0f, 1, 0.4f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "HitRepeat", "jumpspin", "SwordHitGreenCritical", 0f, "", "", "", "");
            config[2010241] = new SkillConfig(2010241, "乱战", "乱", "攻击晕眩单位造成额外伤害", "", "", 1, 0f, 3f, 20, "", 0f, 0f, "atk", null, 0f, "", 0, 0f, 0, 0f, 0.7f, 0f, 0, 0f, 0, "", "", 301001, false, 0f, "", 0, 0f, 0f, 0f, 0f, "AttackAddDamage", "saw", "SwordHitRedCritical", 0f, "", "", "", "");
            config[2010242] = new SkillConfig(2010242, "乱战", "乱", "攻击晕眩单位造成额外伤害", "", "", 2, 0f, 3f, 20, "", 0f, 0f, "atk", null, 0f, "", 0, 0f, 0, 0f, 0.7f, 0f, 0, 0f, 0, "", "", 301001, false, 0f, "", 0, 0f, 0f, 0f, 0f, "AttackAddDamage", "saw", "SwordHitRedCritical", 0f, "", "", "", "");
            config[2010243] = new SkillConfig(2010243, "乱战", "乱", "攻击晕眩单位造成额外伤害", "", "", 3, 0f, 3f, 20, "", 0f, 0f, "atk", null, 0f, "", 0, 0f, 0, 0f, 0.7f, 0f, 0, 0f, 0, "", "", 301001, false, 0f, "", 0, 0f, 0f, 0f, 0f, "AttackAddDamage", "saw", "SwordHitRedCritical", 0f, "", "", "", "");
            config[2010244] = new SkillConfig(2010244, "乱战", "乱", "攻击晕眩单位造成额外伤害", "", "", 4, 0f, 3f, 20, "", 0f, 0f, "atk", null, 0f, "", 0, 0f, 0, 0f, 0.7f, 0f, 0, 0f, 0, "", "", 301001, false, 0f, "", 0, 0f, 0f, 0f, 0f, "AttackAddDamage", "saw", "SwordHitRedCritical", 0f, "", "", "", "");
            config[2010245] = new SkillConfig(2010245, "乱战", "乱", "攻击晕眩单位造成额外伤害", "", "", 5, 0f, 3f, 20, "", 0f, 0f, "atk", null, 0f, "", 0, 0f, 0, 0f, 0.7f, 0f, 0, 0f, 0, "", "", 301001, false, 0f, "", 0, 0f, 0f, 0f, 0f, "AttackAddDamage", "saw", "SwordHitRedCritical", 0f, "", "", "", "");
            config[2010251] = new SkillConfig(2010251, "虐袭", "虐", "攻击护盾敌人时造成额外伤害", "技", "", 1, 0f, 0f, 20, "", 0f, 0f, "atk", null, 0f, "", 0, 0.5f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 300001, false, 0f, "", 0, 0f, 0f, 0f, 0f, "AttackAntiShield", "sway", "", 0f, "", "", "", "");
            config[2010252] = new SkillConfig(2010252, "虐袭", "虐", "攻击护盾敌人时造成额外伤害", "技", "", 2, 0f, 0f, 20, "", 0f, 0f, "atk", null, 0f, "", 0, 0.5f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 300001, false, 0f, "", 0, 0f, 0f, 0f, 0f, "AttackAntiShield", "sway", "", 0f, "", "", "", "");
            config[2010253] = new SkillConfig(2010253, "虐袭", "虐", "攻击护盾敌人时造成额外伤害", "技", "", 3, 0f, 0f, 20, "", 0f, 0f, "atk", null, 0f, "", 0, 0.5f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 300001, false, 0f, "", 0, 0f, 0f, 0f, 0f, "AttackAntiShield", "sway", "", 0f, "", "", "", "");
            config[2010254] = new SkillConfig(2010254, "虐袭", "虐", "攻击护盾敌人时造成额外伤害", "技", "", 4, 0f, 0f, 20, "", 0f, 0f, "atk", null, 0f, "", 0, 0.5f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 300001, false, 0f, "", 0, 0f, 0f, 0f, 0f, "AttackAntiShield", "sway", "", 0f, "", "", "", "");
            config[2010255] = new SkillConfig(2010255, "虐袭", "虐", "攻击护盾敌人时造成额外伤害", "技", "", 5, 0f, 0f, 20, "", 0f, 0f, "atk", null, 0f, "", 0, 0.5f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 300001, false, 0f, "", 0, 0f, 0f, 0f, 0f, "AttackAntiShield", "sway", "", 0f, "", "", "", "");
            config[2020011] = new SkillConfig(2020011, "刺甲", "刺", "反弹50%近战伤害", "防御up", "", 1, 0.3f, 0f, 0, "", 0f, 0f, "atk", new string[]{"atk"}, 20f, "", 0, 0.5f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "DefFeedback", "", "SwordHitBlue", 0f, "", "", "", "");
            config[2020012] = new SkillConfig(2020012, "刺甲", "刺", "反弹50%近战伤害", "防御up", "", 2, 0.3f, 0f, 0, "", 0f, 0f, "atk", new string[]{"atk"}, 20f, "", 0, 0.5f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "DefFeedback", "", "SwordHitBlue", 0f, "", "", "", "");
            config[2020013] = new SkillConfig(2020013, "刺甲", "刺", "反弹50%近战伤害", "防御up", "", 3, 0.3f, 0f, 0, "", 0f, 0f, "atk", new string[]{"atk"}, 20f, "", 0, 0.5f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "DefFeedback", "", "SwordHitBlue", 0f, "", "", "", "");
            config[2020014] = new SkillConfig(2020014, "刺甲", "刺", "反弹50%近战伤害", "防御up", "", 4, 0.3f, 0f, 0, "", 0f, 0f, "atk", new string[]{"atk"}, 20f, "", 0, 0.5f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "DefFeedback", "", "SwordHitBlue", 0f, "", "", "", "");
            config[2020015] = new SkillConfig(2020015, "刺甲", "刺", "反弹50%近战伤害", "防御up", "", 5, 0.3f, 0f, 0, "", 0f, 0f, "atk", new string[]{"atk"}, 20f, "", 0, 0.5f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "DefFeedback", "", "SwordHitBlue", 0f, "", "", "", "");
            config[2020021] = new SkillConfig(2020021, "藤甲", "藤", "受非智力攻击时高几率发动70%减伤", "", "", 1, 0.5f, 0.5f, 0, "", 0f, 0f, "atk", new string[]{"atk"}, 0f, "", 0, 0.7f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "DefPlantSkin", "", "", 0f, "", "", "", "");
            config[2020022] = new SkillConfig(2020022, "藤甲", "藤", "受非智力攻击时高几率发动70%减伤", "", "", 2, 0.5f, 0.5f, 0, "", 0f, 0f, "atk", new string[]{"atk"}, 0f, "", 0, 0.7f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "DefPlantSkin", "", "", 0f, "", "", "", "");
            config[2020023] = new SkillConfig(2020023, "藤甲", "藤", "受非智力攻击时高几率发动70%减伤", "", "", 3, 0.5f, 0.5f, 0, "", 0f, 0f, "atk", new string[]{"atk"}, 0f, "", 0, 0.7f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "DefPlantSkin", "", "", 0f, "", "", "", "");
            config[2020024] = new SkillConfig(2020024, "藤甲", "藤", "受非智力攻击时高几率发动70%减伤", "", "", 4, 0.5f, 0.5f, 0, "", 0f, 0f, "atk", new string[]{"atk"}, 0f, "", 0, 0.7f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "DefPlantSkin", "", "", 0f, "", "", "", "");
            config[2020025] = new SkillConfig(2020025, "藤甲", "藤", "受非智力攻击时高几率发动70%减伤", "", "", 5, 0.5f, 0.5f, 0, "", 0f, 0f, "atk", new string[]{"atk"}, 0f, "", 0, 0.7f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "DefPlantSkin", "", "", 0f, "", "", "", "");
            config[2020031] = new SkillConfig(2020031, "明镜", "镜", "自己和同行队友反弹智力伤害", "防御up", "", 1, 0.3f, 0f, 0, "", 0f, 0f, "atk", new string[]{"ap"}, 0f, "", 0, 0.5f, 0, 0f, 0f, 0f, 0, 0f, 1, "竟", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "DefFeedback", "", "SwordHitBlue", 0f, "", "", "", "");
            config[2020032] = new SkillConfig(2020032, "明镜", "镜", "自己和同行队友反弹智力伤害", "防御up", "", 2, 0.3f, 0f, 0, "", 0f, 0f, "atk", new string[]{"ap"}, 0f, "", 0, 0.5f, 0, 0f, 0f, 0f, 0, 0f, 1, "竟", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "DefFeedback", "", "SwordHitBlue", 0f, "", "", "", "");
            config[2020033] = new SkillConfig(2020033, "明镜", "镜", "自己和同行队友反弹智力伤害", "防御up", "", 3, 0.3f, 0f, 0, "", 0f, 0f, "atk", new string[]{"ap"}, 0f, "", 0, 0.5f, 0, 0f, 0f, 0f, 0, 0f, 1, "竟", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "DefFeedback", "", "SwordHitBlue", 0f, "", "", "", "");
            config[2020034] = new SkillConfig(2020034, "明镜", "镜", "自己和同行队友反弹智力伤害", "防御up", "", 4, 0.3f, 0f, 0, "", 0f, 0f, "atk", new string[]{"ap"}, 0f, "", 0, 0.5f, 0, 0f, 0f, 0f, 0, 0f, 1, "竟", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "DefFeedback", "", "SwordHitBlue", 0f, "", "", "", "");
            config[2020035] = new SkillConfig(2020035, "明镜", "镜", "自己和同行队友反弹智力伤害", "防御up", "", 5, 0.3f, 0f, 0, "", 0f, 0f, "atk", new string[]{"ap"}, 0f, "", 0, 0.5f, 0, 0f, 0f, 0f, 0, 0f, 1, "竟", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "DefFeedback", "", "SwordHitBlue", 0f, "", "", "", "");
            config[2020041] = new SkillConfig(2020041, "明镜小", "竟", "反弹30%智力伤害", "防御up", "", 1, 0.3f, 0f, 0, "", 0f, 0f, "atk", new string[]{"ap"}, 0f, "", 0, 0.3f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "DefFeedback", "", "SwordHitBlue", 0f, "", "", "", "");
            config[2020042] = new SkillConfig(2020042, "明镜小", "竟", "反弹30%智力伤害", "防御up", "", 2, 0.3f, 0f, 0, "", 0f, 0f, "atk", new string[]{"ap"}, 0f, "", 0, 0.3f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "DefFeedback", "", "SwordHitBlue", 0f, "", "", "", "");
            config[2020043] = new SkillConfig(2020043, "明镜小", "竟", "反弹30%智力伤害", "防御up", "", 3, 0.3f, 0f, 0, "", 0f, 0f, "atk", new string[]{"ap"}, 0f, "", 0, 0.3f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "DefFeedback", "", "SwordHitBlue", 0f, "", "", "", "");
            config[2020044] = new SkillConfig(2020044, "明镜小", "竟", "反弹30%智力伤害", "防御up", "", 4, 0.3f, 0f, 0, "", 0f, 0f, "atk", new string[]{"ap"}, 0f, "", 0, 0.3f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "DefFeedback", "", "SwordHitBlue", 0f, "", "", "", "");
            config[2020045] = new SkillConfig(2020045, "明镜小", "竟", "反弹30%智力伤害", "防御up", "", 5, 0.3f, 0f, 0, "", 0f, 0f, "atk", new string[]{"ap"}, 0f, "", 0, 0.3f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "DefFeedback", "", "SwordHitBlue", 0f, "", "", "", "");
            config[2020051] = new SkillConfig(2020051, "护卫", "护", "给与友军护盾祝福", "", "", 1, 1f, 8f, 20, "", 1.5f, 0f, "atk", null, 50f, "", 0, 0.18f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 300001, false, 10f, "", 0, 0f, 0f, 0f, 0f, "HelpAidBuff", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2020052] = new SkillConfig(2020052, "护卫", "护", "给与友军护盾祝福", "", "", 2, 1f, 8f, 20, "", 1.5f, 0f, "atk", null, 50f, "", 0, 0.18f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 300001, false, 10f, "", 0, 0f, 0f, 0f, 0f, "HelpAidBuff", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2020053] = new SkillConfig(2020053, "护卫", "护", "给与友军护盾祝福", "", "", 3, 1f, 8f, 20, "", 1.5f, 0f, "atk", null, 50f, "", 0, 0.18f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 300001, false, 10f, "", 0, 0f, 0f, 0f, 0f, "HelpAidBuff", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2020054] = new SkillConfig(2020054, "护卫", "护", "给与友军护盾祝福", "", "", 4, 1f, 8f, 20, "", 1.5f, 0f, "atk", null, 50f, "", 0, 0.18f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 300001, false, 10f, "", 0, 0f, 0f, 0f, 0f, "HelpAidBuff", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2020055] = new SkillConfig(2020055, "护卫", "护", "给与友军护盾祝福", "", "", 5, 1f, 8f, 20, "", 1.5f, 0f, "atk", null, 50f, "", 0, 0.18f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 300001, false, 10f, "", 0, 0f, 0f, 0f, 0f, "HelpAidBuff", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2020061] = new SkillConfig(2020061, "坚毅", "坚", "生命值低时降低50%伤害", "防御up", "", 1, 0.3f, 0.5f, 0, "", 0f, 0.35f, "atk", null, 0f, "", 0, 0.5f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "DefHpLow", "", "", 0f, "", "", "", "");
            config[2020062] = new SkillConfig(2020062, "坚毅", "坚", "生命值低时降低50%伤害", "防御up", "", 2, 0.3f, 0.5f, 0, "", 0f, 0.35f, "atk", null, 0f, "", 0, 0.5f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "DefHpLow", "", "", 0f, "", "", "", "");
            config[2020063] = new SkillConfig(2020063, "坚毅", "坚", "生命值低时降低50%伤害", "防御up", "", 3, 0.3f, 0.5f, 0, "", 0f, 0.35f, "atk", null, 0f, "", 0, 0.5f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "DefHpLow", "", "", 0f, "", "", "", "");
            config[2020064] = new SkillConfig(2020064, "坚毅", "坚", "生命值低时降低50%伤害", "防御up", "", 4, 0.3f, 0.5f, 0, "", 0f, 0.35f, "atk", null, 0f, "", 0, 0.5f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "DefHpLow", "", "", 0f, "", "", "", "");
            config[2020065] = new SkillConfig(2020065, "坚毅", "坚", "生命值低时降低50%伤害", "防御up", "", 5, 0.3f, 0.5f, 0, "", 0f, 0.35f, "atk", null, 0f, "", 0, 0.5f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "DefHpLow", "", "", 0f, "", "", "", "");
            config[2020111] = new SkillConfig(2020111, "敏锐", "敏", "提升15%闪避率", "防御up", "", 1, 0f, 0f, 0, "", 0f, 0f, "atk", null, 0f, "", 0, 0.2f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "InitAddDodge", "", "", 0f, "", "", "", "");
            config[2020112] = new SkillConfig(2020112, "敏锐", "敏", "提升15%闪避率", "防御up", "", 2, 0f, 0f, 0, "", 0f, 0f, "atk", null, 0f, "", 0, 0.2f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "InitAddDodge", "", "", 0f, "", "", "", "");
            config[2020113] = new SkillConfig(2020113, "敏锐", "敏", "提升15%闪避率", "防御up", "", 3, 0f, 0f, 0, "", 0f, 0f, "atk", null, 0f, "", 0, 0.2f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "InitAddDodge", "", "", 0f, "", "", "", "");
            config[2020114] = new SkillConfig(2020114, "敏锐", "敏", "提升15%闪避率", "防御up", "", 4, 0f, 0f, 0, "", 0f, 0f, "atk", null, 0f, "", 0, 0.2f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "InitAddDodge", "", "", 0f, "", "", "", "");
            config[2020115] = new SkillConfig(2020115, "敏锐", "敏", "提升15%闪避率", "防御up", "", 5, 0f, 0f, 0, "", 0f, 0f, "atk", null, 0f, "", 0, 0.2f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "InitAddDodge", "", "", 0f, "", "", "", "");
            config[2020131] = new SkillConfig(2020131, "复原", "复", "提升5点生命回复", "防御up", "", 1, 0f, 0f, 0, "", 0f, 0f, "atk", null, 0f, "", 0, 0f, 5, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "InitAddRege", "", "", 0f, "", "", "", "");
            config[2020132] = new SkillConfig(2020132, "复原", "复", "提升5点生命回复", "防御up", "", 2, 0f, 0f, 0, "", 0f, 0f, "atk", null, 0f, "", 0, 0f, 5, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "InitAddRege", "", "", 0f, "", "", "", "");
            config[2020133] = new SkillConfig(2020133, "复原", "复", "提升5点生命回复", "防御up", "", 3, 0f, 0f, 0, "", 0f, 0f, "atk", null, 0f, "", 0, 0f, 5, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "InitAddRege", "", "", 0f, "", "", "", "");
            config[2020134] = new SkillConfig(2020134, "复原", "复", "提升5点生命回复", "防御up", "", 4, 0f, 0f, 0, "", 0f, 0f, "atk", null, 0f, "", 0, 0f, 5, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "InitAddRege", "", "", 0f, "", "", "", "");
            config[2020135] = new SkillConfig(2020135, "复原", "复", "提升5点生命回复", "防御up", "", 5, 0f, 0f, 0, "", 0f, 0f, "atk", null, 0f, "", 0, 0f, 5, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "InitAddRege", "", "", 0f, "", "", "", "");
            config[2020141] = new SkillConfig(2020141, "药仙", "药", "自己和所有队友提升5点生命回复", "防御up", "", 1, 0f, 0f, 0, "", 0f, 0f, "ap", null, 0f, "", 0, 0f, 5, 0f, 0f, 0f, 0, 0f, 3, "复", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "InitAddRege", "", "", 0f, "", "", "", "");
            config[2020142] = new SkillConfig(2020142, "药仙", "药", "自己和所有队友提升5点生命回复", "防御up", "", 2, 0f, 0f, 0, "", 0f, 0f, "ap", null, 0f, "", 0, 0f, 5, 0f, 0f, 0f, 0, 0f, 3, "复", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "InitAddRege", "", "", 0f, "", "", "", "");
            config[2020143] = new SkillConfig(2020143, "药仙", "药", "自己和所有队友提升5点生命回复", "防御up", "", 3, 0f, 0f, 0, "", 0f, 0f, "ap", null, 0f, "", 0, 0f, 5, 0f, 0f, 0f, 0, 0f, 3, "复", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "InitAddRege", "", "", 0f, "", "", "", "");
            config[2020144] = new SkillConfig(2020144, "药仙", "药", "自己和所有队友提升5点生命回复", "防御up", "", 4, 0f, 0f, 0, "", 0f, 0f, "ap", null, 0f, "", 0, 0f, 5, 0f, 0f, 0f, 0, 0f, 3, "复", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "InitAddRege", "", "", 0f, "", "", "", "");
            config[2020145] = new SkillConfig(2020145, "药仙", "药", "自己和所有队友提升5点生命回复", "防御up", "", 5, 0f, 0f, 0, "", 0f, 0f, "ap", null, 0f, "", 0, 0f, 5, 0f, 0f, 0f, 0, 0f, 3, "复", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "InitAddRege", "", "", 0f, "", "", "", "");
            config[2030021] = new SkillConfig(2030021, "连锁", "锁", "锁定敌人目标,传递一半受到伤害", "术", "", 1, 0.5f, 3f, 20, "", 0f, 0f, "ap", null, 25f, "targetUnit", 1, 0f, 0, 0f, 0.5f, 0f, 0, 0f, 0, "", "", 301002, false, 8f, "", 0, 0f, 0f, 0f, 0f, "HitBuffArea", "throw", "MagicNovaYellow", 0f, "", "", "", "");
            config[2030022] = new SkillConfig(2030022, "连锁", "锁", "锁定敌人目标,传递一半受到伤害", "术", "", 2, 0.5f, 3f, 20, "", 0f, 0f, "ap", null, 25f, "targetUnit", 1, 0f, 0, 0f, 0.5f, 0f, 0, 0f, 0, "", "", 301002, false, 8f, "", 0, 0f, 0f, 0f, 0f, "HitBuffArea", "throw", "MagicNovaYellow", 0f, "", "", "", "");
            config[2030023] = new SkillConfig(2030023, "连锁", "锁", "锁定敌人目标,传递一半受到伤害", "术", "", 3, 0.5f, 3f, 20, "", 0f, 0f, "ap", null, 25f, "targetUnit", 1, 0f, 0, 0f, 0.5f, 0f, 0, 0f, 0, "", "", 301002, false, 8f, "", 0, 0f, 0f, 0f, 0f, "HitBuffArea", "throw", "MagicNovaYellow", 0f, "", "", "", "");
            config[2030024] = new SkillConfig(2030024, "连锁", "锁", "锁定敌人目标,传递一半受到伤害", "术", "", 4, 0.5f, 3f, 20, "", 0f, 0f, "ap", null, 25f, "targetUnit", 1, 0f, 0, 0f, 0.5f, 0f, 0, 0f, 0, "", "", 301002, false, 8f, "", 0, 0f, 0f, 0f, 0f, "HitBuffArea", "throw", "MagicNovaYellow", 0f, "", "", "", "");
            config[2030025] = new SkillConfig(2030025, "连锁", "锁", "锁定敌人目标,传递一半受到伤害", "术", "", 5, 0.5f, 3f, 20, "", 0f, 0f, "ap", null, 25f, "targetUnit", 1, 0f, 0, 0f, 0.5f, 0f, 0, 0f, 0, "", "", 301002, false, 8f, "", 0, 0f, 0f, 0f, 0f, "HitBuffArea", "throw", "MagicNovaYellow", 0f, "", "", "", "");
            config[2030041] = new SkillConfig(2030041, "威震", "威", "攻击时混乱周围目标", "", "", 1, 0.2f, 5f, 20, "", 0f, 0f, "atk", null, 20f, "castUnit", 3, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 301001, false, 1.5f, "", 0, 0f, 0f, 0f, 0f, "HitBuffArea", "spin", "MagicNovaYellow", 0f, "", "", "", "");
            config[2030042] = new SkillConfig(2030042, "威震", "威", "攻击时混乱周围目标", "", "", 2, 0.2f, 5f, 20, "", 0f, 0f, "atk", null, 20f, "castUnit", 3, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 301001, false, 1.5f, "", 0, 0f, 0f, 0f, 0f, "HitBuffArea", "spin", "MagicNovaYellow", 0f, "", "", "", "");
            config[2030043] = new SkillConfig(2030043, "威震", "威", "攻击时混乱周围目标", "", "", 3, 0.2f, 5f, 20, "", 0f, 0f, "atk", null, 20f, "castUnit", 3, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 301001, false, 1.5f, "", 0, 0f, 0f, 0f, 0f, "HitBuffArea", "spin", "MagicNovaYellow", 0f, "", "", "", "");
            config[2030044] = new SkillConfig(2030044, "威震", "威", "攻击时混乱周围目标", "", "", 4, 0.2f, 5f, 20, "", 0f, 0f, "atk", null, 20f, "castUnit", 3, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 301001, false, 1.5f, "", 0, 0f, 0f, 0f, 0f, "HitBuffArea", "spin", "MagicNovaYellow", 0f, "", "", "", "");
            config[2030045] = new SkillConfig(2030045, "威震", "威", "攻击时混乱周围目标", "", "", 5, 0.2f, 5f, 20, "", 0f, 0f, "atk", null, 20f, "castUnit", 3, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 301001, false, 1.5f, "", 0, 0f, 0f, 0f, 0f, "HitBuffArea", "spin", "MagicNovaYellow", 0f, "", "", "", "");
            config[2030051] = new SkillConfig(2030051, "击破", "泼", "攻击几率使目标增伤40%", "", "", 1, 0.4f, 2f, 20, "", 0f, 0f, "atk", null, 0f, "", 0, 0.4f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 301003, false, 3f, "", 0, 0f, 0f, 0f, 0f, "HitBuff", "jump", "SoftFireBigRed", 0f, "", "", "", "");
            config[2030052] = new SkillConfig(2030052, "击破", "泼", "攻击几率使目标增伤40%", "", "", 2, 0.4f, 2f, 20, "", 0f, 0f, "atk", null, 0f, "", 0, 0.4f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 301003, false, 3f, "", 0, 0f, 0f, 0f, 0f, "HitBuff", "jump", "SoftFireBigRed", 0f, "", "", "", "");
            config[2030053] = new SkillConfig(2030053, "击破", "泼", "攻击几率使目标增伤40%", "", "", 3, 0.4f, 2f, 20, "", 0f, 0f, "atk", null, 0f, "", 0, 0.4f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 301003, false, 3f, "", 0, 0f, 0f, 0f, 0f, "HitBuff", "jump", "SoftFireBigRed", 0f, "", "", "", "");
            config[2030054] = new SkillConfig(2030054, "击破", "泼", "攻击几率使目标增伤40%", "", "", 4, 0.4f, 2f, 20, "", 0f, 0f, "atk", null, 0f, "", 0, 0.4f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 301003, false, 3f, "", 0, 0f, 0f, 0f, 0f, "HitBuff", "jump", "SoftFireBigRed", 0f, "", "", "", "");
            config[2030055] = new SkillConfig(2030055, "击破", "泼", "攻击几率使目标增伤40%", "", "", 5, 0.4f, 2f, 20, "", 0f, 0f, "atk", null, 0f, "", 0, 0.4f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 301003, false, 3f, "", 0, 0f, 0f, 0f, 0f, "HitBuff", "jump", "SoftFireBigRed", 0f, "", "", "", "");
            config[2030061] = new SkillConfig(2030061, "延缓", "缓", "攻击几率使目标减速30%", "", "", 1, 0.4f, 3f, 20, "", 0f, 0f, "ap", null, 0f, "", 0, 0.3f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 301004, false, 5f, "", 0, 0f, 0f, 0f, 0f, "HitBuff", "sway", "MagicNovaYellow", 0f, "", "", "", "");
            config[2030062] = new SkillConfig(2030062, "延缓", "缓", "攻击几率使目标减速30%", "", "", 2, 0.4f, 3f, 20, "", 0f, 0f, "ap", null, 0f, "", 0, 0.3f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 301004, false, 5f, "", 0, 0f, 0f, 0f, 0f, "HitBuff", "sway", "MagicNovaYellow", 0f, "", "", "", "");
            config[2030063] = new SkillConfig(2030063, "延缓", "缓", "攻击几率使目标减速30%", "", "", 3, 0.4f, 3f, 20, "", 0f, 0f, "ap", null, 0f, "", 0, 0.3f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 301004, false, 5f, "", 0, 0f, 0f, 0f, 0f, "HitBuff", "sway", "MagicNovaYellow", 0f, "", "", "", "");
            config[2030064] = new SkillConfig(2030064, "延缓", "缓", "攻击几率使目标减速30%", "", "", 4, 0.4f, 3f, 20, "", 0f, 0f, "ap", null, 0f, "", 0, 0.3f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 301004, false, 5f, "", 0, 0f, 0f, 0f, 0f, "HitBuff", "sway", "MagicNovaYellow", 0f, "", "", "", "");
            config[2030065] = new SkillConfig(2030065, "延缓", "缓", "攻击几率使目标减速30%", "", "", 5, 0.4f, 3f, 20, "", 0f, 0f, "ap", null, 0f, "", 0, 0.3f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 301004, false, 5f, "", 0, 0f, 0f, 0f, 0f, "HitBuff", "sway", "MagicNovaYellow", 0f, "", "", "", "");
            config[2030071] = new SkillConfig(2030071, "陷阵", "陷", "攻击几率使目标陷阵", "", "", 1, 0.4f, 3f, 20, "", 0f, 0f, "ap", null, 0f, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 301005, false, 4f, "", 0, 0f, 0f, 0f, 0f, "HitBuff", "sway", "MagicNovaYellow", 0f, "", "", "", "");
            config[2030072] = new SkillConfig(2030072, "陷阵", "陷", "攻击几率使目标陷阵", "", "", 2, 0.4f, 3f, 20, "", 0f, 0f, "ap", null, 0f, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 301005, false, 4f, "", 0, 0f, 0f, 0f, 0f, "HitBuff", "sway", "MagicNovaYellow", 0f, "", "", "", "");
            config[2030073] = new SkillConfig(2030073, "陷阵", "陷", "攻击几率使目标陷阵", "", "", 3, 0.4f, 3f, 20, "", 0f, 0f, "ap", null, 0f, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 301005, false, 4f, "", 0, 0f, 0f, 0f, 0f, "HitBuff", "sway", "MagicNovaYellow", 0f, "", "", "", "");
            config[2030074] = new SkillConfig(2030074, "陷阵", "陷", "攻击几率使目标陷阵", "", "", 4, 0.4f, 3f, 20, "", 0f, 0f, "ap", null, 0f, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 301005, false, 4f, "", 0, 0f, 0f, 0f, 0f, "HitBuff", "sway", "MagicNovaYellow", 0f, "", "", "", "");
            config[2030075] = new SkillConfig(2030075, "陷阵", "陷", "攻击几率使目标陷阵", "", "", 5, 0.4f, 3f, 20, "", 0f, 0f, "ap", null, 0f, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 301005, false, 4f, "", 0, 0f, 0f, 0f, 0f, "HitBuff", "sway", "MagicNovaYellow", 0f, "", "", "", "");
            config[2030081] = new SkillConfig(2030081, "溃散", "溃", "攻击几率使目标溃败", "", "", 1, 0.4f, 4f, 20, "", 0f, 0f, "ap", null, 0f, "", 0, 0f, 0, 0f, 0f, 0.1f, 0, 0f, 0, "", "", 301006, false, 5.2f, "", 0, 0f, 0f, 0f, 0f, "HitBuff", "sway", "MagicNovaYellow", 0f, "", "", "", "");
            config[2030082] = new SkillConfig(2030082, "溃散", "溃", "攻击几率使目标溃败", "", "", 2, 0.4f, 4f, 20, "", 0f, 0f, "ap", null, 0f, "", 0, 0f, 0, 0f, 0f, 0.1f, 0, 0f, 0, "", "", 301006, false, 5.2f, "", 0, 0f, 0f, 0f, 0f, "HitBuff", "sway", "MagicNovaYellow", 0f, "", "", "", "");
            config[2030083] = new SkillConfig(2030083, "溃散", "溃", "攻击几率使目标溃败", "", "", 3, 0.4f, 4f, 20, "", 0f, 0f, "ap", null, 0f, "", 0, 0f, 0, 0f, 0f, 0.1f, 0, 0f, 0, "", "", 301006, false, 5.2f, "", 0, 0f, 0f, 0f, 0f, "HitBuff", "sway", "MagicNovaYellow", 0f, "", "", "", "");
            config[2030084] = new SkillConfig(2030084, "溃散", "溃", "攻击几率使目标溃败", "", "", 4, 0.4f, 4f, 20, "", 0f, 0f, "ap", null, 0f, "", 0, 0f, 0, 0f, 0f, 0.1f, 0, 0f, 0, "", "", 301006, false, 5.2f, "", 0, 0f, 0f, 0f, 0f, "HitBuff", "sway", "MagicNovaYellow", 0f, "", "", "", "");
            config[2030085] = new SkillConfig(2030085, "溃散", "溃", "攻击几率使目标溃败", "", "", 5, 0.4f, 4f, 20, "", 0f, 0f, "ap", null, 0f, "", 0, 0f, 0, 0f, 0f, 0.1f, 0, 0f, 0, "", "", 301006, false, 5.2f, "", 0, 0f, 0f, 0f, 0f, "HitBuff", "sway", "MagicNovaYellow", 0f, "", "", "", "");
            config[2030091] = new SkillConfig(2030091, "分兵", "分", "被攻击时产生一只有伤害部队", "", "", 1, 0.4f, 15f, 0, "", 0f, 0f, "atk", null, 15f, "", 0, 0f, 0, 0.5f, 0.5f, 0f, 4, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "AttackedShadow", "sway", "MagicFieldGreen", 0f, "", "", "", "");
            config[2030092] = new SkillConfig(2030092, "分兵", "分", "被攻击时产生一只有伤害部队", "", "", 2, 0.4f, 15f, 0, "", 0f, 0f, "atk", null, 15f, "", 0, 0f, 0, 0.5f, 0.5f, 0f, 4, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "AttackedShadow", "sway", "MagicFieldGreen", 0f, "", "", "", "");
            config[2030093] = new SkillConfig(2030093, "分兵", "分", "被攻击时产生一只有伤害部队", "", "", 3, 0.4f, 15f, 0, "", 0f, 0f, "atk", null, 15f, "", 0, 0f, 0, 0.5f, 0.5f, 0f, 4, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "AttackedShadow", "sway", "MagicFieldGreen", 0f, "", "", "", "");
            config[2030094] = new SkillConfig(2030094, "分兵", "分", "被攻击时产生一只有伤害部队", "", "", 4, 0.4f, 15f, 0, "", 0f, 0f, "atk", null, 15f, "", 0, 0f, 0, 0.5f, 0.5f, 0f, 4, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "AttackedShadow", "sway", "MagicFieldGreen", 0f, "", "", "", "");
            config[2030095] = new SkillConfig(2030095, "分兵", "分", "被攻击时产生一只有伤害部队", "", "", 5, 0.4f, 15f, 0, "", 0f, 0f, "atk", null, 15f, "", 0, 0f, 0, 0.5f, 0.5f, 0f, 4, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "AttackedShadow", "sway", "MagicFieldGreen", 0f, "", "", "", "");
            config[2080011] = new SkillConfig(2080011, "百出", "百", "降低自己和同行[扇谋相]技能CD时间", "智技up", "", 1, 0f, 0f, 0, "", 0f, 0f, "ap", new string[]{"ap"}, 0f, "", 0, 0.3f, 0, 0f, 0f, 0f, 0, 0f, 1, "白", "扇谋相", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "ModifySkillRateTime", "", "", 0f, "", "", "", "");
            config[2080012] = new SkillConfig(2080012, "百出", "百", "降低自己和同行[扇谋相]技能CD时间", "智技up", "", 2, 0f, 0f, 0, "", 0f, 0f, "ap", new string[]{"ap"}, 0f, "", 0, 0.3f, 0, 0f, 0f, 0f, 0, 0f, 1, "白", "扇谋相", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "ModifySkillRateTime", "", "", 0f, "", "", "", "");
            config[2080013] = new SkillConfig(2080013, "百出", "百", "降低自己和同行[扇谋相]技能CD时间", "智技up", "", 3, 0f, 0f, 0, "", 0f, 0f, "ap", new string[]{"ap"}, 0f, "", 0, 0.3f, 0, 0f, 0f, 0f, 0, 0f, 1, "白", "扇谋相", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "ModifySkillRateTime", "", "", 0f, "", "", "", "");
            config[2080014] = new SkillConfig(2080014, "百出", "百", "降低自己和同行[扇谋相]技能CD时间", "智技up", "", 4, 0f, 0f, 0, "", 0f, 0f, "ap", new string[]{"ap"}, 0f, "", 0, 0.3f, 0, 0f, 0f, 0f, 0, 0f, 1, "白", "扇谋相", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "ModifySkillRateTime", "", "", 0f, "", "", "", "");
            config[2080015] = new SkillConfig(2080015, "百出", "百", "降低自己和同行[扇谋相]技能CD时间", "智技up", "", 5, 0f, 0f, 0, "", 0f, 0f, "ap", new string[]{"ap"}, 0f, "", 0, 0.3f, 0, 0f, 0f, 0f, 0, 0f, 1, "白", "扇谋相", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "ModifySkillRateTime", "", "", 0f, "", "", "", "");
            config[2080021] = new SkillConfig(2080021, "百出小", "白", "降低技能CD时间", "智技up", "", 1, 0f, 0f, 0, "", 0f, 0f, "ap", new string[]{"ap"}, 0f, "", 0, 0.4f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "ModifySkillRateTime", "", "", 0f, "", "", "", "");
            config[2080022] = new SkillConfig(2080022, "百出小", "白", "降低技能CD时间", "智技up", "", 2, 0f, 0f, 0, "", 0f, 0f, "ap", new string[]{"ap"}, 0f, "", 0, 0.4f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "ModifySkillRateTime", "", "", 0f, "", "", "", "");
            config[2080023] = new SkillConfig(2080023, "百出小", "白", "降低技能CD时间", "智技up", "", 3, 0f, 0f, 0, "", 0f, 0f, "ap", new string[]{"ap"}, 0f, "", 0, 0.4f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "ModifySkillRateTime", "", "", 0f, "", "", "", "");
            config[2080024] = new SkillConfig(2080024, "百出小", "白", "降低技能CD时间", "智技up", "", 4, 0f, 0f, 0, "", 0f, 0f, "ap", new string[]{"ap"}, 0f, "", 0, 0.4f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "ModifySkillRateTime", "", "", 0f, "", "", "", "");
            config[2080025] = new SkillConfig(2080025, "百出小", "白", "降低技能CD时间", "智技up", "", 5, 0f, 0f, 0, "", 0f, 0f, "ap", new string[]{"ap"}, 0f, "", 0, 0.4f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "ModifySkillRateTime", "", "", 0f, "", "", "", "");
            config[2080031] = new SkillConfig(2080031, "神算", "神", "提升技能命中率和持续时间", "智技up", "", 1, 0.3f, 0f, 0, "", 0f, 0f, "ap", new string[]{"ap"}, 0f, "", 0, 0.5f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0.5f, "", 0, 0f, 0f, 0f, 0f, "ModifySkillRateTime", "", "", 0f, "", "", "", "");
            config[2080032] = new SkillConfig(2080032, "神算", "神", "提升技能命中率和持续时间", "智技up", "", 2, 0.3f, 0f, 0, "", 0f, 0f, "ap", new string[]{"ap"}, 0f, "", 0, 0.5f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0.5f, "", 0, 0f, 0f, 0f, 0f, "ModifySkillRateTime", "", "", 0f, "", "", "", "");
            config[2080033] = new SkillConfig(2080033, "神算", "神", "提升技能命中率和持续时间", "智技up", "", 3, 0.3f, 0f, 0, "", 0f, 0f, "ap", new string[]{"ap"}, 0f, "", 0, 0.5f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0.5f, "", 0, 0f, 0f, 0f, 0f, "ModifySkillRateTime", "", "", 0f, "", "", "", "");
            config[2080034] = new SkillConfig(2080034, "神算", "神", "提升技能命中率和持续时间", "智技up", "", 4, 0.3f, 0f, 0, "", 0f, 0f, "ap", new string[]{"ap"}, 0f, "", 0, 0.5f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0.5f, "", 0, 0f, 0f, 0f, 0f, "ModifySkillRateTime", "", "", 0f, "", "", "", "");
            config[2080035] = new SkillConfig(2080035, "神算", "神", "提升技能命中率和持续时间", "智技up", "", 5, 0.3f, 0f, 0, "", 0f, 0f, "ap", new string[]{"ap"}, 0f, "", 0, 0.5f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0.5f, "", 0, 0f, 0f, 0f, 0f, "ModifySkillRateTime", "", "", 0f, "", "", "", "");
            config[2080041] = new SkillConfig(2080041, "蔓延", "延", "自己和同行[扇谋相]技能负面状态扩散", "智技up", "", 1, 0.5f, 3f, 0, "", 0f, 0f, "ap", new string[]{"ap"}, 30f, "", 2, 0f, 0, 0f, 0f, 0f, 0, 0f, 1, "筵", "扇谋相", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "BuffExpand", "", "", 0f, "", "", "", "");
            config[2080042] = new SkillConfig(2080042, "蔓延", "延", "自己和同行[扇谋相]技能负面状态扩散", "智技up", "", 2, 0.5f, 3f, 0, "", 0f, 0f, "ap", new string[]{"ap"}, 30f, "", 2, 0f, 0, 0f, 0f, 0f, 0, 0f, 1, "筵", "扇谋相", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "BuffExpand", "", "", 0f, "", "", "", "");
            config[2080043] = new SkillConfig(2080043, "蔓延", "延", "自己和同行[扇谋相]技能负面状态扩散", "智技up", "", 3, 0.5f, 3f, 0, "", 0f, 0f, "ap", new string[]{"ap"}, 30f, "", 2, 0f, 0, 0f, 0f, 0f, 0, 0f, 1, "筵", "扇谋相", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "BuffExpand", "", "", 0f, "", "", "", "");
            config[2080044] = new SkillConfig(2080044, "蔓延", "延", "自己和同行[扇谋相]技能负面状态扩散", "智技up", "", 4, 0.5f, 3f, 0, "", 0f, 0f, "ap", new string[]{"ap"}, 30f, "", 2, 0f, 0, 0f, 0f, 0f, 0, 0f, 1, "筵", "扇谋相", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "BuffExpand", "", "", 0f, "", "", "", "");
            config[2080045] = new SkillConfig(2080045, "蔓延", "延", "自己和同行[扇谋相]技能负面状态扩散", "智技up", "", 5, 0.5f, 3f, 0, "", 0f, 0f, "ap", new string[]{"ap"}, 30f, "", 2, 0f, 0, 0f, 0f, 0f, 0, 0f, 1, "筵", "扇谋相", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "BuffExpand", "", "", 0f, "", "", "", "");
            config[2080051] = new SkillConfig(2080051, "蔓延小", "筵", "技能负面状态概率扩散", "智技up", "", 1, 0.5f, 3f, 0, "", 0f, 0f, "ap", new string[]{"ap"}, 30f, "", 2, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "BuffExpand", "", "", 0f, "", "", "", "");
            config[2080052] = new SkillConfig(2080052, "蔓延小", "筵", "技能负面状态概率扩散", "智技up", "", 2, 0.5f, 3f, 0, "", 0f, 0f, "ap", new string[]{"ap"}, 30f, "", 2, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "BuffExpand", "", "", 0f, "", "", "", "");
            config[2080053] = new SkillConfig(2080053, "蔓延小", "筵", "技能负面状态概率扩散", "智技up", "", 3, 0.5f, 3f, 0, "", 0f, 0f, "ap", new string[]{"ap"}, 30f, "", 2, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "BuffExpand", "", "", 0f, "", "", "", "");
            config[2080054] = new SkillConfig(2080054, "蔓延小", "筵", "技能负面状态概率扩散", "智技up", "", 4, 0.5f, 3f, 0, "", 0f, 0f, "ap", new string[]{"ap"}, 30f, "", 2, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "BuffExpand", "", "", 0f, "", "", "", "");
            config[2080055] = new SkillConfig(2080055, "蔓延小", "筵", "技能负面状态概率扩散", "智技up", "", 5, 0.5f, 3f, 0, "", 0f, 0f, "ap", new string[]{"ap"}, 30f, "", 2, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "BuffExpand", "", "", 0f, "", "", "", "");
            config[2080061] = new SkillConfig(2080061, "同调", "调", "自己和同行[鼓乐医]技能正面状态扩散", "智技up", "", 1, 0.5f, 3f, 0, "", 0f, 0f, "ap", new string[]{"ap"}, 30f, "", 2, 0f, 0, 0f, 0f, 0f, 0, 0f, 1, "碉", "鼓乐医", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "BuffExpandPos", "", "", 0f, "", "", "", "");
            config[2080062] = new SkillConfig(2080062, "同调", "调", "自己和同行[鼓乐医]技能正面状态扩散", "智技up", "", 2, 0.5f, 3f, 0, "", 0f, 0f, "ap", new string[]{"ap"}, 30f, "", 2, 0f, 0, 0f, 0f, 0f, 0, 0f, 1, "碉", "鼓乐医", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "BuffExpandPos", "", "", 0f, "", "", "", "");
            config[2080063] = new SkillConfig(2080063, "同调", "调", "自己和同行[鼓乐医]技能正面状态扩散", "智技up", "", 3, 0.5f, 3f, 0, "", 0f, 0f, "ap", new string[]{"ap"}, 30f, "", 2, 0f, 0, 0f, 0f, 0f, 0, 0f, 1, "碉", "鼓乐医", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "BuffExpandPos", "", "", 0f, "", "", "", "");
            config[2080064] = new SkillConfig(2080064, "同调", "调", "自己和同行[鼓乐医]技能正面状态扩散", "智技up", "", 4, 0.5f, 3f, 0, "", 0f, 0f, "ap", new string[]{"ap"}, 30f, "", 2, 0f, 0, 0f, 0f, 0f, 0, 0f, 1, "碉", "鼓乐医", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "BuffExpandPos", "", "", 0f, "", "", "", "");
            config[2080065] = new SkillConfig(2080065, "同调", "调", "自己和同行[鼓乐医]技能正面状态扩散", "智技up", "", 5, 0.5f, 3f, 0, "", 0f, 0f, "ap", new string[]{"ap"}, 30f, "", 2, 0f, 0, 0f, 0f, 0f, 0, 0f, 1, "碉", "鼓乐医", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "BuffExpandPos", "", "", 0f, "", "", "", "");
            config[2080071] = new SkillConfig(2080071, "同调小", "碉", "技能正面状态扩散", "智技up", "", 1, 0.5f, 3f, 0, "", 0f, 0f, "ap", new string[]{"ap"}, 30f, "", 2, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "BuffExpandPos", "", "", 0f, "", "", "", "");
            config[2080072] = new SkillConfig(2080072, "同调小", "碉", "技能正面状态扩散", "智技up", "", 2, 0.5f, 3f, 0, "", 0f, 0f, "ap", new string[]{"ap"}, 30f, "", 2, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "BuffExpandPos", "", "", 0f, "", "", "", "");
            config[2080073] = new SkillConfig(2080073, "同调小", "碉", "技能正面状态扩散", "智技up", "", 3, 0.5f, 3f, 0, "", 0f, 0f, "ap", new string[]{"ap"}, 30f, "", 2, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "BuffExpandPos", "", "", 0f, "", "", "", "");
            config[2080074] = new SkillConfig(2080074, "同调小", "碉", "技能正面状态扩散", "智技up", "", 4, 0.5f, 3f, 0, "", 0f, 0f, "ap", new string[]{"ap"}, 30f, "", 2, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "BuffExpandPos", "", "", 0f, "", "", "", "");
            config[2080075] = new SkillConfig(2080075, "同调小", "碉", "技能正面状态扩散", "智技up", "", 5, 0.5f, 3f, 0, "", 0f, 0f, "ap", new string[]{"ap"}, 30f, "", 2, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "BuffExpandPos", "", "", 0f, "", "", "", "");
            config[2080081] = new SkillConfig(2080081, "炽热", "炽", "提升本方火焰持续时间", "智技up", "", 1, 0f, 0f, 0, "", 0f, 0f, "ap", null, 0f, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 3, "炽", "", 0, false, 0f, "火", 0, 0f, 3f, 0f, 0f, "ModifySummonTime", "", "", 0f, "", "", "", "");
            config[2080082] = new SkillConfig(2080082, "炽热", "炽", "提升本方火焰持续时间", "智技up", "", 2, 0f, 0f, 0, "", 0f, 0f, "ap", null, 0f, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 3, "炽", "", 0, false, 0f, "火", 0, 0f, 3f, 0f, 0f, "ModifySummonTime", "", "", 0f, "", "", "", "");
            config[2080083] = new SkillConfig(2080083, "炽热", "炽", "提升本方火焰持续时间", "智技up", "", 3, 0f, 0f, 0, "", 0f, 0f, "ap", null, 0f, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 3, "炽", "", 0, false, 0f, "火", 0, 0f, 3f, 0f, 0f, "ModifySummonTime", "", "", 0f, "", "", "", "");
            config[2080084] = new SkillConfig(2080084, "炽热", "炽", "提升本方火焰持续时间", "智技up", "", 4, 0f, 0f, 0, "", 0f, 0f, "ap", null, 0f, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 3, "炽", "", 0, false, 0f, "火", 0, 0f, 3f, 0f, 0f, "ModifySummonTime", "", "", 0f, "", "", "", "");
            config[2080085] = new SkillConfig(2080085, "炽热", "炽", "提升本方火焰持续时间", "智技up", "", 5, 0f, 0f, 0, "", 0f, 0f, "ap", null, 0f, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 3, "炽", "", 0, false, 0f, "火", 0, 0f, 3f, 0f, 0f, "ModifySummonTime", "", "", 0f, "", "", "", "");
            config[2090051] = new SkillConfig(2090051, "学习", "学", "攻击时几率提升自己的属性", "", "", 1, 0.3f, 0f, 20, "", 0f, 0f, "ap", null, 0f, "", 0, 0f, 5, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "HitAttr", "", "MagicChargeYellow", 0f, "", "", "", "");
            config[2090052] = new SkillConfig(2090052, "学习", "学", "攻击时几率提升自己的属性", "", "", 2, 0.3f, 0f, 20, "", 0f, 0f, "ap", null, 0f, "", 0, 0f, 5, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "HitAttr", "", "MagicChargeYellow", 0f, "", "", "", "");
            config[2090053] = new SkillConfig(2090053, "学习", "学", "攻击时几率提升自己的属性", "", "", 3, 0.3f, 0f, 20, "", 0f, 0f, "ap", null, 0f, "", 0, 0f, 5, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "HitAttr", "", "MagicChargeYellow", 0f, "", "", "", "");
            config[2090054] = new SkillConfig(2090054, "学习", "学", "攻击时几率提升自己的属性", "", "", 4, 0.3f, 0f, 20, "", 0f, 0f, "ap", null, 0f, "", 0, 0f, 5, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "HitAttr", "", "MagicChargeYellow", 0f, "", "", "", "");
            config[2090055] = new SkillConfig(2090055, "学习", "学", "攻击时几率提升自己的属性", "", "", 5, 0.3f, 0f, 20, "", 0f, 0f, "ap", null, 0f, "", 0, 0f, 5, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "HitAttr", "", "MagicChargeYellow", 0f, "", "", "", "");

            RebuildIndex();

        }

        private static void RebuildIndex()
        {
            foreach (var kv in config)
            {
            }
        }

        public static SkillConfig GetConfig(int id)
        {
            SkillConfig data;
            if (config.TryGetValue(id, out data))
            {
                return data;
            }
            throw new NullReferenceException(string.Format("配置表SkillConfig不存在id={0}", id));
        }


        public static bool HasConfig(int id)
        {
            if (config.ContainsKey(id))
            {
                return true;
            }
            return false;
        }

        public static void Assign(int id, SkillConfig configData)
        {
            config[id] = configData; 
        }

        public static void Add(int id, SkillConfig configData)
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
