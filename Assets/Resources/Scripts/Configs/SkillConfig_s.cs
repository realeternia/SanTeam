using System;
using System.Collections;
using System.Collections.Generic;

namespace CommonConfig
{
    public class SkillConfig
    {
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
        ///攻击时间惩罚
        /// </summary>
        public float AttackPointReduce;
        /// <summary>
        ///条件参数
        /// </summary>
        public float ConditionParm;
        /// <summary>
        ///相关属性
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
        ///范围外
        /// </summary>
        public bool RangeOut;
        /// <summary>
        ///选取点
        /// </summary>
        public string TargetType;
        /// <summary>
        ///最大目标数
        /// </summary>
        public int TargetCount;
        /// <summary>
        ///技能强度（恒定）
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
        ///技能强度属性比例
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
        ///技能MP消耗（0=不使用MP，>0则每次行动充能MpCost/3，3次行动满，满才能发动，发动后清空）
        /// </summary>
        public int MpCost;


        public SkillConfig(int Id, string Name, string Sname, string Descript, string Type, int Lv, float Rate, float CD, float AttackPointReduce, float ConditionParm, string Attr, string[] CheckAttrs, float Range, bool RangeOut, string TargetType, int TargetCount, float Strength, int StrengthInt, float SkillAttrRate, float SkillDamageRate, float SkillDamageAttrRate, int DoCount, float TimeDelay, int UnitHelpType, string HelpSkill, string HelpSkillJob, int BuffId, bool NegBuff, float BuffTime, string SummonTag, int SummonCount, float SummonArea, float SummonTime, float SummonHitInterval, float SummonSpeed, string ScriptName, string Action, string HitEffect, float EffectSize, string Icon, int MpCost)
        {
            this.Id = Id;
            this.Name = Name;
            this.Sname = Sname;
            this.Descript = Descript;
            this.Type = Type;
            this.Lv = Lv;
            this.Rate = Rate;
            this.CD = CD;
            this.AttackPointReduce = AttackPointReduce;
            this.ConditionParm = ConditionParm;
            this.Attr = Attr;
            this.CheckAttrs = CheckAttrs;
            this.Range = Range;
            this.RangeOut = RangeOut;
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
            this.MpCost = MpCost;
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
            {"Id", new FieldMetaInfo("序列", "int", 0)},
            {"Name", new FieldMetaInfo("名字", "string", 0)},
            {"Sname", new FieldMetaInfo("缩写", "string", 0)},
            {"Descript", new FieldMetaInfo("说明", "string", 0)},
            {"Type", new FieldMetaInfo("分类", "string", 0)},
            {"Lv", new FieldMetaInfo("等级", "int", 0)},
            {"Rate", new FieldMetaInfo("发动概率", "float", 0)},
            {"CD", new FieldMetaInfo("发动cd", "float", 0)},
            {"AttackPointReduce", new FieldMetaInfo("攻击时间惩罚", "float", 0)},
            {"ConditionParm", new FieldMetaInfo("条件参数", "float", 0)},
            {"Attr", new FieldMetaInfo("相关属性", "string", 0)},
            {"CheckAttrs", new FieldMetaInfo("判定属性", "string[]", 0)},
            {"Range", new FieldMetaInfo("范围", "float", 0)},
            {"RangeOut", new FieldMetaInfo("范围外", "bool", 0)},
            {"TargetType", new FieldMetaInfo("选取点", "string", 0)},
            {"TargetCount", new FieldMetaInfo("最大目标数", "int", 0)},
            {"Strength", new FieldMetaInfo("技能强度（恒定）", "float", 0)},
            {"StrengthInt", new FieldMetaInfo("技能强度（恒定）", "int", 0)},
            {"SkillAttrRate", new FieldMetaInfo("技能数值比例", "float", 0)},
            {"SkillDamageRate", new FieldMetaInfo("技能强度伤害比例", "float", 0)},
            {"SkillDamageAttrRate", new FieldMetaInfo("技能强度属性比例", "float", 0)},
            {"DoCount", new FieldMetaInfo("计数次数", "int", 0)},
            {"TimeDelay", new FieldMetaInfo("计数延迟", "float", 0)},
            {"UnitHelpType", new FieldMetaInfo("效果范围(1横向，2纵向)", "int", 0)},
            {"HelpSkill", new FieldMetaInfo("光环技能", "string", 0)},
            {"HelpSkillJob", new FieldMetaInfo("职业限定", "string", 0)},
            {"BuffId", new FieldMetaInfo("BuffId", "int", 0)},
            {"NegBuff", new FieldMetaInfo("是否针对负面buff", "bool", 0)},
            {"BuffTime", new FieldMetaInfo("Buff持续", "float", 0)},
            {"SummonTag", new FieldMetaInfo("召唤物标签", "string", 0)},
            {"SummonCount", new FieldMetaInfo("技能场数", "int", 0)},
            {"SummonArea", new FieldMetaInfo("技能场范围", "float", 0)},
            {"SummonTime", new FieldMetaInfo("技能场持续", "float", 0)},
            {"SummonHitInterval", new FieldMetaInfo("技能场间隔", "float", 0)},
            {"SummonSpeed", new FieldMetaInfo("技能场速度", "float", 0)},
            {"ScriptName", new FieldMetaInfo("脚本名", "string", 0)},
            {"Action", new FieldMetaInfo("动作", "string", 0)},
            {"HitEffect", new FieldMetaInfo("hit", "string", 0)},
            {"EffectSize", new FieldMetaInfo("size", "float", 0)},
            {"Icon", new FieldMetaInfo("图标", "string", 0)},
            {"MpCost", new FieldMetaInfo("技能MP消耗", "int", 0)},
        };

        public static Dictionary<string, FieldMetaInfo> FieldMeta { get { return fieldMeta; } }

        private static List<CellMeta> cellMeta = new List<CellMeta>();
        public static List<CellMeta> CellMetas { get { return cellMeta; } }

        public static void Load()
        {
            config.Clear();
            config[2000011] = new SkillConfig(2000011, "王", "帅", "主公所在同阵营护盾效果加倍", "职业", 1, 0f, 0f, 0f, 0f, "", null, 0f, false, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 300001, false, 999f, "", 0, 0f, 0f, 0f, 0f, "InitMasterShield", "", "", 0f, "shuai", 0);
            config[2000012] = new SkillConfig(2000012, "王", "帅", "主公所在同阵营护盾效果加倍", "职业", 2, 0f, 0f, 0f, 0f, "", null, 0f, false, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 300001, false, 999f, "", 0, 0f, 0f, 0f, 0f, "InitMasterShield", "", "", 0f, "shuai", 0);
            config[2000013] = new SkillConfig(2000013, "王", "帅", "主公所在同阵营护盾效果加倍", "职业", 3, 0f, 0f, 0f, 0f, "", null, 0f, false, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 300001, false, 999f, "", 0, 0f, 0f, 0f, 0f, "InitMasterShield", "", "", 0f, "shuai", 0);
            config[2000014] = new SkillConfig(2000014, "王", "帅", "主公所在同阵营护盾效果加倍", "职业", 4, 0f, 0f, 0f, 0f, "", null, 0f, false, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 300001, false, 999f, "", 0, 0f, 0f, 0f, 0f, "InitMasterShield", "", "", 0f, "shuai", 0);
            config[2000015] = new SkillConfig(2000015, "王", "帅", "主公所在同阵营护盾效果加倍", "职业", 5, 0f, 0f, 0f, 0f, "", null, 0f, false, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 300001, false, 999f, "", 0, 0f, 0f, 0f, 0f, "InitMasterShield", "", "", 0f, "shuai", 0);
            config[2000021] = new SkillConfig(2000021, "羽扇", "扇", "击中目标时触发弹射", "职业", 1, 0f, 0f, 0f, 0f, "ap", null, 30f, false, "", 1, 0f, 0, 0f, 0.15f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "AttackReboundArrow", "sway", "", 0f, "shan", 20);
            config[2000022] = new SkillConfig(2000022, "羽扇", "扇", "击中目标时触发弹射", "职业", 2, 0f, 0f, 0f, 0f, "ap", null, 30f, false, "", 1, 0f, 0, 0f, 0.15f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "AttackReboundArrow", "sway", "", 0f, "shan", 20);
            config[2000023] = new SkillConfig(2000023, "羽扇", "扇", "击中目标时触发弹射", "职业", 3, 0f, 0f, 0f, 0f, "ap", null, 30f, false, "", 1, 0f, 0, 0f, 0.15f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "AttackReboundArrow", "sway", "", 0f, "shan", 20);
            config[2000024] = new SkillConfig(2000024, "羽扇", "扇", "击中目标时触发弹射", "职业", 4, 0f, 0f, 0f, 0f, "ap", null, 30f, false, "", 1, 0f, 0, 0f, 0.15f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "AttackReboundArrow", "sway", "", 0f, "shan", 20);
            config[2000025] = new SkillConfig(2000025, "羽扇", "扇", "击中目标时触发弹射", "职业", 5, 0f, 0f, 0f, 0f, "ap", null, 30f, false, "", 1, 0f, 0, 0f, 0.15f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "AttackReboundArrow", "sway", "", 0f, "shan", 20);
            config[2000031] = new SkillConfig(2000031, "刀兵", "刀", "攻击几率造成额外伤害", "职业", 1, 0.15f, 5f, 0f, 0f, "atk", null, 0f, false, "", 0, 0f, 10, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "AttackAddDamage", "sway", "SwordHitRedCritical", 0f, "dao", 20);
            config[2000032] = new SkillConfig(2000032, "刀兵", "刀", "攻击几率造成额外伤害", "职业", 2, 0.15f, 5f, 0f, 0f, "atk", null, 0f, false, "", 0, 0f, 10, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "AttackAddDamage", "sway", "SwordHitRedCritical", 0f, "dao", 20);
            config[2000033] = new SkillConfig(2000033, "刀兵", "刀", "攻击几率造成额外伤害", "职业", 3, 0.15f, 5f, 0f, 0f, "atk", null, 0f, false, "", 0, 0f, 10, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "AttackAddDamage", "sway", "SwordHitRedCritical", 0f, "dao", 20);
            config[2000034] = new SkillConfig(2000034, "刀兵", "刀", "攻击几率造成额外伤害", "职业", 4, 0.15f, 5f, 0f, 0f, "atk", null, 0f, false, "", 0, 0f, 10, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "AttackAddDamage", "sway", "SwordHitRedCritical", 0f, "dao", 20);
            config[2000035] = new SkillConfig(2000035, "刀兵", "刀", "攻击几率造成额外伤害", "职业", 5, 0.15f, 5f, 0f, 0f, "atk", null, 0f, false, "", 0, 0f, 10, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "AttackAddDamage", "sway", "SwordHitRedCritical", 0f, "dao", 20);
            config[2000041] = new SkillConfig(2000041, "坚韧", "士", "受击时几率发动减伤", "职业", 1, 0.35f, 7f, 0f, 0f, "might", null, 0f, false, "", 0, 0.5f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 300002, false, 4.5f, "", 0, 0f, 0f, 0f, 0f, "AttackedBuff", "spin", "", 0f, "shi", 0);
            config[2000042] = new SkillConfig(2000042, "坚韧", "士", "受击时几率发动减伤", "职业", 2, 0.35f, 7f, 0f, 0f, "might", null, 0f, false, "", 0, 0.5f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 300002, false, 4.5f, "", 0, 0f, 0f, 0f, 0f, "AttackedBuff", "spin", "", 0f, "shi", 0);
            config[2000043] = new SkillConfig(2000043, "坚韧", "士", "受击时几率发动减伤", "职业", 3, 0.35f, 7f, 0f, 0f, "might", null, 0f, false, "", 0, 0.5f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 300002, false, 4.5f, "", 0, 0f, 0f, 0f, 0f, "AttackedBuff", "spin", "", 0f, "shi", 0);
            config[2000044] = new SkillConfig(2000044, "坚韧", "士", "受击时几率发动减伤", "职业", 4, 0.35f, 7f, 0f, 0f, "might", null, 0f, false, "", 0, 0.5f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 300002, false, 4.5f, "", 0, 0f, 0f, 0f, 0f, "AttackedBuff", "spin", "", 0f, "shi", 0);
            config[2000045] = new SkillConfig(2000045, "坚韧", "士", "受击时几率发动减伤", "职业", 5, 0.35f, 7f, 0f, 0f, "might", null, 0f, false, "", 0, 0.5f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 300002, false, 4.5f, "", 0, 0f, 0f, 0f, 0f, "AttackedBuff", "spin", "", 0f, "shi", 0);
            config[2000051] = new SkillConfig(2000051, "突破", "马", "移动时穿越敌人,造成额外伤害", "职业", 1, 1f, 7f, 0f, 0f, "atk", null, 0f, false, "", 0, 0f, 0, 0f, 0.5f, 0f, 0, 0f, 0, "", "", 301003, false, 0f, "", 0, 0f, 0f, 0f, 0f, "AttackRunCross", "", "LightningExplosionBlue", 0f, "ma", 20);
            config[2000052] = new SkillConfig(2000052, "突破", "马", "移动时穿越敌人,造成额外伤害", "职业", 2, 1f, 7f, 0f, 0f, "atk", null, 0f, false, "", 0, 0f, 0, 0f, 0.5f, 0f, 0, 0f, 0, "", "", 301003, false, 0f, "", 0, 0f, 0f, 0f, 0f, "AttackRunCross", "", "LightningExplosionBlue", 0f, "ma", 20);
            config[2000053] = new SkillConfig(2000053, "突破", "马", "移动时穿越敌人,造成额外伤害", "职业", 3, 1f, 7f, 0f, 0f, "atk", null, 0f, false, "", 0, 0f, 0, 0f, 0.5f, 0f, 0, 0f, 0, "", "", 301003, false, 0f, "", 0, 0f, 0f, 0f, 0f, "AttackRunCross", "", "LightningExplosionBlue", 0f, "ma", 20);
            config[2000054] = new SkillConfig(2000054, "突破", "马", "移动时穿越敌人,造成额外伤害", "职业", 4, 1f, 7f, 0f, 0f, "atk", null, 0f, false, "", 0, 0f, 0, 0f, 0.5f, 0f, 0, 0f, 0, "", "", 301003, false, 0f, "", 0, 0f, 0f, 0f, 0f, "AttackRunCross", "", "LightningExplosionBlue", 0f, "ma", 20);
            config[2000055] = new SkillConfig(2000055, "突破", "马", "移动时穿越敌人,造成额外伤害", "职业", 5, 1f, 7f, 0f, 0f, "atk", null, 0f, false, "", 0, 0f, 0, 0f, 0.5f, 0f, 0, 0f, 0, "", "", 301003, false, 0f, "", 0, 0f, 0f, 0f, 0f, "AttackRunCross", "", "LightningExplosionBlue", 0f, "ma", 20);
            config[2000061] = new SkillConfig(2000061, "运筹", "相", "提升士兵等级", "职业", 1, 0f, 0f, 0f, 0f, "ap", null, 0f, false, "", 0, 0f, 0, 0.25f, 0.05f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "InitSoldierUp", "", "MagicChargeYellow", 0f, "xiang", 0);
            config[2000062] = new SkillConfig(2000062, "运筹", "相", "提升士兵等级", "职业", 2, 0f, 0f, 0f, 0f, "ap", null, 0f, false, "", 0, 0f, 0, 0.25f, 0.05f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "InitSoldierUp", "", "MagicChargeYellow", 0f, "xiang", 0);
            config[2000063] = new SkillConfig(2000063, "运筹", "相", "提升士兵等级", "职业", 3, 0f, 0f, 0f, 0f, "ap", null, 0f, false, "", 0, 0f, 0, 0.25f, 0.05f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "InitSoldierUp", "", "MagicChargeYellow", 0f, "xiang", 0);
            config[2000064] = new SkillConfig(2000064, "运筹", "相", "提升士兵等级", "职业", 4, 0f, 0f, 0f, 0f, "ap", null, 0f, false, "", 0, 0f, 0, 0.25f, 0.05f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "InitSoldierUp", "", "MagicChargeYellow", 0f, "xiang", 0);
            config[2000065] = new SkillConfig(2000065, "运筹", "相", "提升士兵等级", "职业", 5, 0f, 0f, 0f, 0f, "ap", null, 0f, false, "", 0, 0f, 0, 0.25f, 0.05f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "InitSoldierUp", "", "MagicChargeYellow", 0f, "xiang", 0);
            config[2000071] = new SkillConfig(2000071, "弓手", "弓", "远程射击单位", "职业", 1, 1f, 99f, 0f, 0f, "atk", null, 0f, false, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "Dumb", "", "", 0f, "gong", 0);
            config[2000072] = new SkillConfig(2000072, "弓手", "弓", "远程射击单位", "职业", 2, 1f, 99f, 0f, 0f, "atk", null, 0f, false, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "Dumb", "", "", 0f, "gong", 0);
            config[2000073] = new SkillConfig(2000073, "弓手", "弓", "远程射击单位", "职业", 3, 1f, 99f, 0f, 0f, "atk", null, 0f, false, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "Dumb", "", "", 0f, "gong", 0);
            config[2000074] = new SkillConfig(2000074, "弓手", "弓", "远程射击单位", "职业", 4, 1f, 99f, 0f, 0f, "atk", null, 0f, false, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "Dumb", "", "", 0f, "gong", 0);
            config[2000075] = new SkillConfig(2000075, "弓手", "弓", "远程射击单位", "职业", 5, 1f, 99f, 0f, 0f, "atk", null, 0f, false, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "Dumb", "", "", 0f, "gong", 0);
            config[2000081] = new SkillConfig(2000081, "谋略", "谋", "一定几率混乱目标单位2s", "职业", 1, 0.15f, 4f, 0f, 0f, "ap", null, 0f, false, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 301001, false, 2f, "", 0, 0f, 0f, 0f, 0f, "HitBuff", "throw", "MagicChargeYellow", 0f, "mou", 20);
            config[2000082] = new SkillConfig(2000082, "谋略", "谋", "一定几率混乱目标单位2s", "职业", 2, 0.15f, 4f, 0f, 0f, "ap", null, 0f, false, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 301001, false, 2f, "", 0, 0f, 0f, 0f, 0f, "HitBuff", "throw", "MagicChargeYellow", 0f, "mou", 20);
            config[2000083] = new SkillConfig(2000083, "谋略", "谋", "一定几率混乱目标单位2s", "职业", 3, 0.15f, 4f, 0f, 0f, "ap", null, 0f, false, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 301001, false, 2f, "", 0, 0f, 0f, 0f, 0f, "HitBuff", "throw", "MagicChargeYellow", 0f, "mou", 20);
            config[2000084] = new SkillConfig(2000084, "谋略", "谋", "一定几率混乱目标单位2s", "职业", 4, 0.15f, 4f, 0f, 0f, "ap", null, 0f, false, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 301001, false, 2f, "", 0, 0f, 0f, 0f, 0f, "HitBuff", "throw", "MagicChargeYellow", 0f, "mou", 20);
            config[2000085] = new SkillConfig(2000085, "谋略", "谋", "一定几率混乱目标单位2s", "职业", 5, 0.15f, 4f, 0f, 0f, "ap", null, 0f, false, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 301001, false, 2f, "", 0, 0f, 0f, 0f, 0f, "HitBuff", "throw", "MagicChargeYellow", 0f, "mou", 20);
            config[2000091] = new SkillConfig(2000091, "炮车", "炮", "攻击目标发生爆炸", "职业", 1, 0.5f, 0f, 0f, 0f, "atk", null, 20f, false, "", 2, 0f, 0, 0f, 0.6f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "HitArea", "throw", "MagicNovaYellow", 0f, "pao", 20);
            config[2000092] = new SkillConfig(2000092, "炮车", "炮", "攻击目标发生爆炸", "职业", 2, 0.5f, 0f, 0f, 0f, "atk", null, 20f, false, "", 2, 0f, 0, 0f, 0.6f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "HitArea", "throw", "MagicNovaYellow", 0f, "pao", 20);
            config[2000093] = new SkillConfig(2000093, "炮车", "炮", "攻击目标发生爆炸", "职业", 3, 0.5f, 0f, 0f, 0f, "atk", null, 20f, false, "", 2, 0f, 0, 0f, 0.6f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "HitArea", "throw", "MagicNovaYellow", 0f, "pao", 20);
            config[2000094] = new SkillConfig(2000094, "炮车", "炮", "攻击目标发生爆炸", "职业", 4, 0.5f, 0f, 0f, 0f, "atk", null, 20f, false, "", 2, 0f, 0, 0f, 0.6f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "HitArea", "throw", "MagicNovaYellow", 0f, "pao", 20);
            config[2000095] = new SkillConfig(2000095, "炮车", "炮", "攻击目标发生爆炸", "职业", 5, 0.5f, 0f, 0f, 0f, "atk", null, 20f, false, "", 2, 0f, 0, 0f, 0.6f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "HitArea", "throw", "MagicNovaYellow", 0f, "pao", 20);
            config[2000101] = new SkillConfig(2000101, "弩手", "弩", "射程非常远", "职业", 1, 1f, 99f, 0f, 0f, "atk", null, 0f, false, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "Dumb", "", "", 0f, "nu", 0);
            config[2000102] = new SkillConfig(2000102, "弩手", "弩", "射程非常远", "职业", 2, 1f, 99f, 0f, 0f, "atk", null, 0f, false, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "Dumb", "", "", 0f, "nu", 0);
            config[2000103] = new SkillConfig(2000103, "弩手", "弩", "射程非常远", "职业", 3, 1f, 99f, 0f, 0f, "atk", null, 0f, false, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "Dumb", "", "", 0f, "nu", 0);
            config[2000104] = new SkillConfig(2000104, "弩手", "弩", "射程非常远", "职业", 4, 1f, 99f, 0f, 0f, "atk", null, 0f, false, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "Dumb", "", "", 0f, "nu", 0);
            config[2000105] = new SkillConfig(2000105, "弩手", "弩", "射程非常远", "职业", 5, 1f, 99f, 0f, 0f, "atk", null, 0f, false, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "Dumb", "", "", 0f, "nu", 0);
            config[2000111] = new SkillConfig(2000111, "冲锋", "车", "移动时穿越敌人,降低目标防御", "职业", 1, 1f, 7f, 0f, 0f, "atk", null, 0f, false, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 301003, false, 3f, "", 0, 0f, 0f, 0f, 0f, "AttackRunCrossPlus", "", "LightningExplosionRed", 0f, "che", 20);
            config[2000112] = new SkillConfig(2000112, "冲锋", "车", "移动时穿越敌人,降低目标防御", "职业", 2, 1f, 7f, 0f, 0f, "atk", null, 0f, false, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 301003, false, 3f, "", 0, 0f, 0f, 0f, 0f, "AttackRunCrossPlus", "", "LightningExplosionRed", 0f, "che", 20);
            config[2000113] = new SkillConfig(2000113, "冲锋", "车", "移动时穿越敌人,降低目标防御", "职业", 3, 1f, 7f, 0f, 0f, "atk", null, 0f, false, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 301003, false, 3f, "", 0, 0f, 0f, 0f, 0f, "AttackRunCrossPlus", "", "LightningExplosionRed", 0f, "che", 20);
            config[2000114] = new SkillConfig(2000114, "冲锋", "车", "移动时穿越敌人,降低目标防御", "职业", 4, 1f, 7f, 0f, 0f, "atk", null, 0f, false, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 301003, false, 3f, "", 0, 0f, 0f, 0f, 0f, "AttackRunCrossPlus", "", "LightningExplosionRed", 0f, "che", 20);
            config[2000115] = new SkillConfig(2000115, "冲锋", "车", "移动时穿越敌人,降低目标防御", "职业", 5, 1f, 7f, 0f, 0f, "atk", null, 0f, false, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 301003, false, 3f, "", 0, 0f, 0f, 0f, 0f, "AttackRunCrossPlus", "", "LightningExplosionRed", 0f, "che", 20);
            config[2000121] = new SkillConfig(2000121, "声乐", "乐", "给与友军攻速祝福", "职业", 1, 1f, 3.5f, 1f, 0f, "ap", null, 50f, false, "", 0, 0.45f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 300005, false, 5f, "", 0, 0f, 0f, 0f, 0f, "HelpAidBuff", "sway", "MagicChargePink", 0f, "song", 20);
            config[2000122] = new SkillConfig(2000122, "声乐", "乐", "给与友军攻速祝福", "职业", 2, 1f, 3.5f, 1f, 0f, "ap", null, 50f, false, "", 0, 0.45f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 300005, false, 5f, "", 0, 0f, 0f, 0f, 0f, "HelpAidBuff", "sway", "MagicChargePink", 0f, "song", 20);
            config[2000123] = new SkillConfig(2000123, "声乐", "乐", "给与友军攻速祝福", "职业", 3, 1f, 3.5f, 1f, 0f, "ap", null, 50f, false, "", 0, 0.45f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 300005, false, 5f, "", 0, 0f, 0f, 0f, 0f, "HelpAidBuff", "sway", "MagicChargePink", 0f, "song", 20);
            config[2000124] = new SkillConfig(2000124, "声乐", "乐", "给与友军攻速祝福", "职业", 4, 1f, 3.5f, 1f, 0f, "ap", null, 50f, false, "", 0, 0.45f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 300005, false, 5f, "", 0, 0f, 0f, 0f, 0f, "HelpAidBuff", "sway", "MagicChargePink", 0f, "song", 20);
            config[2000125] = new SkillConfig(2000125, "声乐", "乐", "给与友军攻速祝福", "职业", 5, 1f, 3.5f, 1f, 0f, "ap", null, 50f, false, "", 0, 0.45f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 300005, false, 5f, "", 0, 0f, 0f, 0f, 0f, "HelpAidBuff", "sway", "MagicChargePink", 0f, "song", 20);
            config[2000131] = new SkillConfig(2000131, "治疗", "医", "给与友军治疗", "职业", 1, 1f, 3f, 1f, 0f, "ap", null, 50f, false, "", 0, 0f, 0, 0.7f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "HelpAidHeal", "sway", "MagicBuffGreen", 0f, "heal", 20);
            config[2000132] = new SkillConfig(2000132, "治疗", "医", "给与友军治疗", "职业", 2, 1f, 3f, 1f, 0f, "ap", null, 50f, false, "", 0, 0f, 0, 0.7f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "HelpAidHeal", "sway", "MagicBuffGreen", 0f, "heal", 20);
            config[2000133] = new SkillConfig(2000133, "治疗", "医", "给与友军治疗", "职业", 3, 1f, 3f, 1f, 0f, "ap", null, 50f, false, "", 0, 0f, 0, 0.7f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "HelpAidHeal", "sway", "MagicBuffGreen", 0f, "heal", 20);
            config[2000134] = new SkillConfig(2000134, "治疗", "医", "给与友军治疗", "职业", 4, 1f, 3f, 1f, 0f, "ap", null, 50f, false, "", 0, 0f, 0, 0.7f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "HelpAidHeal", "sway", "MagicBuffGreen", 0f, "heal", 20);
            config[2000135] = new SkillConfig(2000135, "治疗", "医", "给与友军治疗", "职业", 5, 1f, 3f, 1f, 0f, "ap", null, 50f, false, "", 0, 0f, 0, 0.7f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "HelpAidHeal", "sway", "MagicBuffGreen", 0f, "heal", 20);
            config[2000141] = new SkillConfig(2000141, "枪阵", "枪", "一定几率混乱目标单位2s", "职业", 1, 0.15f, 4f, 0f, 0f, "atk", null, 0f, false, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 301001, false, 2f, "", 0, 0f, 0f, 0f, 0f, "HitBuff", "spin", "MagicChargeYellow", 0f, "qiang", 20);
            config[2000142] = new SkillConfig(2000142, "枪阵", "枪", "一定几率混乱目标单位2s", "职业", 2, 0.15f, 4f, 0f, 0f, "atk", null, 0f, false, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 301001, false, 2f, "", 0, 0f, 0f, 0f, 0f, "HitBuff", "spin", "MagicChargeYellow", 0f, "qiang", 20);
            config[2000143] = new SkillConfig(2000143, "枪阵", "枪", "一定几率混乱目标单位2s", "职业", 3, 0.15f, 4f, 0f, 0f, "atk", null, 0f, false, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 301001, false, 2f, "", 0, 0f, 0f, 0f, 0f, "HitBuff", "spin", "MagicChargeYellow", 0f, "qiang", 20);
            config[2000144] = new SkillConfig(2000144, "枪阵", "枪", "一定几率混乱目标单位2s", "职业", 4, 0.15f, 4f, 0f, 0f, "atk", null, 0f, false, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 301001, false, 2f, "", 0, 0f, 0f, 0f, 0f, "HitBuff", "spin", "MagicChargeYellow", 0f, "qiang", 20);
            config[2000145] = new SkillConfig(2000145, "枪阵", "枪", "一定几率混乱目标单位2s", "职业", 5, 0.15f, 4f, 0f, 0f, "atk", null, 0f, false, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 301001, false, 2f, "", 0, 0f, 0f, 0f, 0f, "HitBuff", "spin", "MagicChargeYellow", 0f, "qiang", 20);
            config[2000151] = new SkillConfig(2000151, "戟阵", "戟", "攻击目标时伤害周边敌人", "职业", 1, 0.35f, 4f, 0f, 0f, "atk", null, 25f, false, "", 2, 0f, 0, 0f, 0.6f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "HitAround", "throw", "SwordSlashMiniWhite", 0f, "ji", 20);
            config[2000152] = new SkillConfig(2000152, "戟阵", "戟", "攻击目标时伤害周边敌人", "职业", 2, 0.35f, 4f, 0f, 0f, "atk", null, 25f, false, "", 2, 0f, 0, 0f, 0.6f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "HitAround", "throw", "SwordSlashMiniWhite", 0f, "ji", 20);
            config[2000153] = new SkillConfig(2000153, "戟阵", "戟", "攻击目标时伤害周边敌人", "职业", 3, 0.35f, 4f, 0f, 0f, "atk", null, 25f, false, "", 2, 0f, 0, 0f, 0.6f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "HitAround", "throw", "SwordSlashMiniWhite", 0f, "ji", 20);
            config[2000154] = new SkillConfig(2000154, "戟阵", "戟", "攻击目标时伤害周边敌人", "职业", 4, 0.35f, 4f, 0f, 0f, "atk", null, 25f, false, "", 2, 0f, 0, 0f, 0.6f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "HitAround", "throw", "SwordSlashMiniWhite", 0f, "ji", 20);
            config[2000155] = new SkillConfig(2000155, "戟阵", "戟", "攻击目标时伤害周边敌人", "职业", 5, 0.35f, 4f, 0f, 0f, "atk", null, 25f, false, "", 2, 0f, 0, 0f, 0.6f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "HitAround", "throw", "SwordSlashMiniWhite", 0f, "ji", 20);
            config[2000161] = new SkillConfig(2000161, "战鼓", "鼓", "给与友军攻击力祝福", "职业", 1, 1f, 3.5f, 1f, 0f, "ap", null, 50f, false, "", 0, 0.4f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 300004, false, 5f, "", 0, 0f, 0f, 0f, 0f, "HelpAidBuff", "sway", "MagicChargeYellow", 0f, "gu", 20);
            config[2000162] = new SkillConfig(2000162, "战鼓", "鼓", "给与友军攻击力祝福", "职业", 2, 1f, 3.5f, 1f, 0f, "ap", null, 50f, false, "", 0, 0.4f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 300004, false, 5f, "", 0, 0f, 0f, 0f, 0f, "HelpAidBuff", "sway", "MagicChargeYellow", 0f, "gu", 20);
            config[2000163] = new SkillConfig(2000163, "战鼓", "鼓", "给与友军攻击力祝福", "职业", 3, 1f, 3.5f, 1f, 0f, "ap", null, 50f, false, "", 0, 0.4f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 300004, false, 5f, "", 0, 0f, 0f, 0f, 0f, "HelpAidBuff", "sway", "MagicChargeYellow", 0f, "gu", 20);
            config[2000164] = new SkillConfig(2000164, "战鼓", "鼓", "给与友军攻击力祝福", "职业", 4, 1f, 3.5f, 1f, 0f, "ap", null, 50f, false, "", 0, 0.4f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 300004, false, 5f, "", 0, 0f, 0f, 0f, 0f, "HelpAidBuff", "sway", "MagicChargeYellow", 0f, "gu", 20);
            config[2000165] = new SkillConfig(2000165, "战鼓", "鼓", "给与友军攻击力祝福", "职业", 5, 1f, 3.5f, 1f, 0f, "ap", null, 50f, false, "", 0, 0.4f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 300004, false, 5f, "", 0, 0f, 0f, 0f, 0f, "HelpAidBuff", "sway", "MagicChargeYellow", 0f, "gu", 20);
            config[2010021] = new SkillConfig(2010021, "铁骑", "铁", "自己和同行[马车]技能附带混乱效果", "攻击up", 1, 0f, 0f, 0f, 0f, "atk", null, 0f, false, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 1, "铁", "马车", 301001, false, 2f, "", 0, 0f, 0f, 0f, 0f, "BuffTieqi", "", "", 0f, "tie", 0);
            config[2010022] = new SkillConfig(2010022, "铁骑", "铁", "自己和同行[马车]技能附带混乱效果", "攻击up", 2, 0f, 0f, 0f, 0f, "atk", null, 0f, false, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 1, "铁", "马车", 301001, false, 2f, "", 0, 0f, 0f, 0f, 0f, "BuffTieqi", "", "", 0f, "tie", 0);
            config[2010023] = new SkillConfig(2010023, "铁骑", "铁", "自己和同行[马车]技能附带混乱效果", "攻击up", 3, 0f, 0f, 0f, 0f, "atk", null, 0f, false, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 1, "铁", "马车", 301001, false, 2f, "", 0, 0f, 0f, 0f, 0f, "BuffTieqi", "", "", 0f, "tie", 0);
            config[2010024] = new SkillConfig(2010024, "铁骑", "铁", "自己和同行[马车]技能附带混乱效果", "攻击up", 4, 0f, 0f, 0f, 0f, "atk", null, 0f, false, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 1, "铁", "马车", 301001, false, 2f, "", 0, 0f, 0f, 0f, 0f, "BuffTieqi", "", "", 0f, "tie", 0);
            config[2010025] = new SkillConfig(2010025, "铁骑", "铁", "自己和同行[马车]技能附带混乱效果", "攻击up", 5, 0f, 0f, 0f, 0f, "atk", null, 0f, false, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 1, "铁", "马车", 301001, false, 2f, "", 0, 0f, 0f, 0f, 0f, "BuffTieqi", "", "", 0f, "tie", 0);
            config[2010031] = new SkillConfig(2010031, "奇袭", "奇", "自己和同行队友统帅技能造成的混乱时间增加50%", "攻击up", 1, 0f, 0f, 0f, 0f, "atk", new string[]{"atk"}, 0f, false, "", 0, 0.5f, 0, 0f, 0f, 0f, 0, 0f, 1, "奇", "", 301001, true, 0f, "", 0, 0f, 0f, 0f, 0f, "ModifyBuffTime", "", "", 0f, "qi", 0);
            config[2010032] = new SkillConfig(2010032, "奇袭", "奇", "自己和同行队友统帅技能造成的混乱时间增加50%", "攻击up", 2, 0f, 0f, 0f, 0f, "atk", new string[]{"atk"}, 0f, false, "", 0, 0.5f, 0, 0f, 0f, 0f, 0, 0f, 1, "奇", "", 301001, true, 0f, "", 0, 0f, 0f, 0f, 0f, "ModifyBuffTime", "", "", 0f, "qi", 0);
            config[2010033] = new SkillConfig(2010033, "奇袭", "奇", "自己和同行队友统帅技能造成的混乱时间增加50%", "攻击up", 3, 0f, 0f, 0f, 0f, "atk", new string[]{"atk"}, 0f, false, "", 0, 0.5f, 0, 0f, 0f, 0f, 0, 0f, 1, "奇", "", 301001, true, 0f, "", 0, 0f, 0f, 0f, 0f, "ModifyBuffTime", "", "", 0f, "qi", 0);
            config[2010034] = new SkillConfig(2010034, "奇袭", "奇", "自己和同行队友统帅技能造成的混乱时间增加50%", "攻击up", 4, 0f, 0f, 0f, 0f, "atk", new string[]{"atk"}, 0f, false, "", 0, 0.5f, 0, 0f, 0f, 0f, 0, 0f, 1, "奇", "", 301001, true, 0f, "", 0, 0f, 0f, 0f, 0f, "ModifyBuffTime", "", "", 0f, "qi", 0);
            config[2010035] = new SkillConfig(2010035, "奇袭", "奇", "自己和同行队友统帅技能造成的混乱时间增加50%", "攻击up", 5, 0f, 0f, 0f, 0f, "atk", new string[]{"atk"}, 0f, false, "", 0, 0.5f, 0, 0f, 0f, 0f, 0, 0f, 1, "奇", "", 301001, true, 0f, "", 0, 0f, 0f, 0f, 0f, "ModifyBuffTime", "", "", 0f, "qi", 0);
            config[2010041] = new SkillConfig(2010041, "瓦解小", "透", "提升20%暴击率", "攻击up", 1, 0f, 0f, 0f, 0f, "might", null, 0f, false, "", 0, 0.2f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "InitAddCrit", "", "", 0f, "jie3", 0);
            config[2010042] = new SkillConfig(2010042, "瓦解小", "透", "提升20%暴击率", "攻击up", 2, 0f, 0f, 0f, 0f, "might", null, 0f, false, "", 0, 0.2f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "InitAddCrit", "", "", 0f, "jie3", 0);
            config[2010043] = new SkillConfig(2010043, "瓦解小", "透", "提升20%暴击率", "攻击up", 3, 0f, 0f, 0f, 0f, "might", null, 0f, false, "", 0, 0.2f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "InitAddCrit", "", "", 0f, "jie3", 0);
            config[2010044] = new SkillConfig(2010044, "瓦解小", "透", "提升20%暴击率", "攻击up", 4, 0f, 0f, 0f, 0f, "might", null, 0f, false, "", 0, 0.2f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "InitAddCrit", "", "", 0f, "jie3", 0);
            config[2010045] = new SkillConfig(2010045, "瓦解小", "透", "提升20%暴击率", "攻击up", 5, 0f, 0f, 0f, 0f, "might", null, 0f, false, "", 0, 0.2f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "InitAddCrit", "", "", 0f, "jie3", 0);
            config[2010051] = new SkillConfig(2010051, "瓦解", "解", "自己和同行队友提升20%暴击率", "攻击up", 1, 0f, 0f, 0f, 0f, "might", null, 0f, false, "", 0, 0.2f, 0, 0f, 0f, 0f, 0, 0f, 1, "透", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "InitAddCrit", "", "", 0f, "jie", 0);
            config[2010052] = new SkillConfig(2010052, "瓦解", "解", "自己和同行队友提升20%暴击率", "攻击up", 2, 0f, 0f, 0f, 0f, "might", null, 0f, false, "", 0, 0.2f, 0, 0f, 0f, 0f, 0, 0f, 1, "透", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "InitAddCrit", "", "", 0f, "jie", 0);
            config[2010053] = new SkillConfig(2010053, "瓦解", "解", "自己和同行队友提升20%暴击率", "攻击up", 3, 0f, 0f, 0f, 0f, "might", null, 0f, false, "", 0, 0.2f, 0, 0f, 0f, 0f, 0, 0f, 1, "透", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "InitAddCrit", "", "", 0f, "jie", 0);
            config[2010054] = new SkillConfig(2010054, "瓦解", "解", "自己和同行队友提升20%暴击率", "攻击up", 4, 0f, 0f, 0f, 0f, "might", null, 0f, false, "", 0, 0.2f, 0, 0f, 0f, 0f, 0, 0f, 1, "透", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "InitAddCrit", "", "", 0f, "jie", 0);
            config[2010055] = new SkillConfig(2010055, "瓦解", "解", "自己和同行队友提升20%暴击率", "攻击up", 5, 0f, 0f, 0f, 0f, "might", null, 0f, false, "", 0, 0.2f, 0, 0f, 0f, 0f, 0, 0f, 1, "透", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "InitAddCrit", "", "", 0f, "jie", 0);
            config[2010061] = new SkillConfig(2010061, "连击", "连", "攻击时几率触发连续攻击", "攻击up", 1, 0.3f, 5f, 0f, 0f, "atk", null, 0f, false, "", 0, 0.7f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "AttackSpeedAttack", "flipspin", "", 0f, "lian", 20);
            config[2010062] = new SkillConfig(2010062, "连击", "连", "攻击时几率触发连续攻击", "攻击up", 2, 0.3f, 5f, 0f, 0f, "atk", null, 0f, false, "", 0, 0.7f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "AttackSpeedAttack", "flipspin", "", 0f, "lian", 20);
            config[2010063] = new SkillConfig(2010063, "连击", "连", "攻击时几率触发连续攻击", "攻击up", 3, 0.3f, 5f, 0f, 0f, "atk", null, 0f, false, "", 0, 0.7f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "AttackSpeedAttack", "flipspin", "", 0f, "lian", 20);
            config[2010064] = new SkillConfig(2010064, "连击", "连", "攻击时几率触发连续攻击", "攻击up", 4, 0.3f, 5f, 0f, 0f, "atk", null, 0f, false, "", 0, 0.7f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "AttackSpeedAttack", "flipspin", "", 0f, "lian", 20);
            config[2010065] = new SkillConfig(2010065, "连击", "连", "攻击时几率触发连续攻击", "攻击up", 5, 0.3f, 5f, 0f, 0f, "atk", null, 0f, false, "", 0, 0.7f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "AttackSpeedAttack", "flipspin", "", 0f, "lian", 20);
            config[2010071] = new SkillConfig(2010071, "速射", "速", "自己和同行[弓弩]箭矢飞行速度提升", "攻击up", 1, 0f, 0f, 0f, 0f, "atk", null, 0f, false, "", 0, 2.5f, 0, 0f, 0f, 0f, 0, 0f, 1, "速", "弓弩", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "ModifyShootSpeed", "", "", 0f, "su", 0);
            config[2010072] = new SkillConfig(2010072, "速射", "速", "自己和同行[弓弩]箭矢飞行速度提升", "攻击up", 2, 0f, 0f, 0f, 0f, "atk", null, 0f, false, "", 0, 2.5f, 0, 0f, 0f, 0f, 0, 0f, 1, "速", "弓弩", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "ModifyShootSpeed", "", "", 0f, "su", 0);
            config[2010073] = new SkillConfig(2010073, "速射", "速", "自己和同行[弓弩]箭矢飞行速度提升", "攻击up", 3, 0f, 0f, 0f, 0f, "atk", null, 0f, false, "", 0, 2.5f, 0, 0f, 0f, 0f, 0, 0f, 1, "速", "弓弩", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "ModifyShootSpeed", "", "", 0f, "su", 0);
            config[2010074] = new SkillConfig(2010074, "速射", "速", "自己和同行[弓弩]箭矢飞行速度提升", "攻击up", 4, 0f, 0f, 0f, 0f, "atk", null, 0f, false, "", 0, 2.5f, 0, 0f, 0f, 0f, 0, 0f, 1, "速", "弓弩", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "ModifyShootSpeed", "", "", 0f, "su", 0);
            config[2010075] = new SkillConfig(2010075, "速射", "速", "自己和同行[弓弩]箭矢飞行速度提升", "攻击up", 5, 0f, 0f, 0f, 0f, "atk", null, 0f, false, "", 0, 2.5f, 0, 0f, 0f, 0f, 0, 0f, 1, "速", "弓弩", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "ModifyShootSpeed", "", "", 0f, "su", 0);
            config[2010081] = new SkillConfig(2010081, "箭雨", "雨", "攻击时30%发出2只箭", "", 1, 0.3f, 4f, 0f, 0f, "atk", null, 30f, false, "", 1, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "AttackMultiArrow", "flipspin", "", 0f, "duo", 20);
            config[2010082] = new SkillConfig(2010082, "箭雨", "雨", "攻击时30%发出2只箭", "", 2, 0.3f, 4f, 0f, 0f, "atk", null, 30f, false, "", 1, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "AttackMultiArrow", "flipspin", "", 0f, "duo", 20);
            config[2010083] = new SkillConfig(2010083, "箭雨", "雨", "攻击时30%发出2只箭", "", 3, 0.3f, 4f, 0f, 0f, "atk", null, 30f, false, "", 1, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "AttackMultiArrow", "flipspin", "", 0f, "duo", 20);
            config[2010084] = new SkillConfig(2010084, "箭雨", "雨", "攻击时30%发出2只箭", "", 4, 0.3f, 4f, 0f, 0f, "atk", null, 30f, false, "", 1, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "AttackMultiArrow", "flipspin", "", 0f, "duo", 20);
            config[2010085] = new SkillConfig(2010085, "箭雨", "雨", "攻击时30%发出2只箭", "", 5, 0.3f, 4f, 0f, 0f, "atk", null, 30f, false, "", 1, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "AttackMultiArrow", "flipspin", "", 0f, "duo", 20);
            config[2010091] = new SkillConfig(2010091, "共杀", "共", "击中目标时触发2次弹射", "", 1, 0.35f, 5f, 0f, 0f, "ap", null, 30f, false, "", 3, 0f, 0, 0f, 0.25f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "AttackReboundArrow", "flipspin", "", 0f, "gong2", 20);
            config[2010092] = new SkillConfig(2010092, "共杀", "共", "击中目标时触发2次弹射", "", 2, 0.35f, 5f, 0f, 0f, "ap", null, 30f, false, "", 3, 0f, 0, 0f, 0.25f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "AttackReboundArrow", "flipspin", "", 0f, "gong2", 20);
            config[2010093] = new SkillConfig(2010093, "共杀", "共", "击中目标时触发2次弹射", "", 3, 0.35f, 5f, 0f, 0f, "ap", null, 30f, false, "", 3, 0f, 0, 0f, 0.25f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "AttackReboundArrow", "flipspin", "", 0f, "gong2", 20);
            config[2010094] = new SkillConfig(2010094, "共杀", "共", "击中目标时触发2次弹射", "", 4, 0.35f, 5f, 0f, 0f, "ap", null, 30f, false, "", 3, 0f, 0, 0f, 0.25f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "AttackReboundArrow", "flipspin", "", 0f, "gong2", 20);
            config[2010095] = new SkillConfig(2010095, "共杀", "共", "击中目标时触发2次弹射", "", 5, 0.35f, 5f, 0f, 0f, "ap", null, 30f, false, "", 3, 0f, 0, 0f, 0.25f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "AttackReboundArrow", "flipspin", "", 0f, "gong2", 20);
            config[2010101] = new SkillConfig(2010101, "旋风斩", "旋", "攻击时几率对附近敌人造成伤害", "技", 1, 0.4f, 5f, 0f, 0f, "might", null, 25f, false, "", 5, 0f, 0, 0f, 0.8f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "AttackSpinAttack", "spin", "SwordWhirlwindWhite", 0f, "meng", 20);
            config[2010102] = new SkillConfig(2010102, "旋风斩", "旋", "攻击时几率对附近敌人造成伤害", "技", 2, 0.4f, 5f, 0f, 0f, "might", null, 25f, false, "", 5, 0f, 0, 0f, 0.8f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "AttackSpinAttack", "spin", "SwordWhirlwindWhite", 0f, "meng", 20);
            config[2010103] = new SkillConfig(2010103, "旋风斩", "旋", "攻击时几率对附近敌人造成伤害", "技", 3, 0.4f, 5f, 0f, 0f, "might", null, 25f, false, "", 5, 0f, 0, 0f, 0.8f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "AttackSpinAttack", "spin", "SwordWhirlwindWhite", 0f, "meng", 20);
            config[2010104] = new SkillConfig(2010104, "旋风斩", "旋", "攻击时几率对附近敌人造成伤害", "技", 4, 0.4f, 5f, 0f, 0f, "might", null, 25f, false, "", 5, 0f, 0, 0f, 0.8f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "AttackSpinAttack", "spin", "SwordWhirlwindWhite", 0f, "meng", 20);
            config[2010105] = new SkillConfig(2010105, "旋风斩", "旋", "攻击时几率对附近敌人造成伤害", "技", 5, 0.4f, 5f, 0f, 0f, "might", null, 25f, false, "", 5, 0f, 0, 0f, 0.8f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "AttackSpinAttack", "spin", "SwordWhirlwindWhite", 0f, "meng", 20);
            config[2010111] = new SkillConfig(2010111, "落雷", "天", "攻击召唤出持续伤害的雷电阵", "术", 1, 0.3f, 10f, 0f, 0f, "ap", null, 0f, false, "", 1, 0f, 0, 0f, 0f, 0.15f, 0, 0f, 0, "", "", 0, false, 0f, "雷", 0, 25f, 5.2f, 0.5f, 0f, "HitRegion", "spin", "SummonStorm", 5f, "tian", 20);
            config[2010112] = new SkillConfig(2010112, "落雷", "天", "攻击召唤出持续伤害的雷电阵", "术", 2, 0.3f, 10f, 0f, 0f, "ap", null, 0f, false, "", 1, 0f, 0, 0f, 0f, 0.15f, 0, 0f, 0, "", "", 0, false, 0f, "雷", 0, 25f, 5.2f, 0.5f, 0f, "HitRegion", "spin", "SummonStorm", 5f, "tian", 20);
            config[2010113] = new SkillConfig(2010113, "落雷", "天", "攻击召唤出持续伤害的雷电阵", "术", 3, 0.3f, 10f, 0f, 0f, "ap", null, 0f, false, "", 1, 0f, 0, 0f, 0f, 0.15f, 0, 0f, 0, "", "", 0, false, 0f, "雷", 0, 25f, 5.2f, 0.5f, 0f, "HitRegion", "spin", "SummonStorm", 5f, "tian", 20);
            config[2010114] = new SkillConfig(2010114, "落雷", "天", "攻击召唤出持续伤害的雷电阵", "术", 4, 0.3f, 10f, 0f, 0f, "ap", null, 0f, false, "", 1, 0f, 0, 0f, 0f, 0.15f, 0, 0f, 0, "", "", 0, false, 0f, "雷", 0, 25f, 5.2f, 0.5f, 0f, "HitRegion", "spin", "SummonStorm", 5f, "tian", 20);
            config[2010115] = new SkillConfig(2010115, "落雷", "天", "攻击召唤出持续伤害的雷电阵", "术", 5, 0.3f, 10f, 0f, 0f, "ap", null, 0f, false, "", 1, 0f, 0, 0f, 0f, 0.15f, 0, 0f, 0, "", "", 0, false, 0f, "雷", 0, 25f, 5.2f, 0.5f, 0f, "HitRegion", "spin", "SummonStorm", 5f, "tian", 20);
            config[2010121] = new SkillConfig(2010121, "火计", "火", "攻击时对目标放火", "术", 1, 0.3f, 5f, 0f, 0f, "ap", null, 0f, false, "", 1, 0f, 0, 0f, 0f, 0.1f, 0, 0f, 0, "", "", 0, false, 0f, "火", 1, 8f, 3.2f, 1f, 0f, "HitWall", "throw", "SoftFireBigRed", 1.6f, "huo", 20);
            config[2010122] = new SkillConfig(2010122, "火计", "火", "攻击时对目标放火", "术", 2, 0.3f, 5f, 0f, 0f, "ap", null, 0f, false, "", 1, 0f, 0, 0f, 0f, 0.1f, 0, 0f, 0, "", "", 0, false, 0f, "火", 1, 8f, 3.2f, 1f, 0f, "HitWall", "throw", "SoftFireBigRed", 1.6f, "huo", 20);
            config[2010123] = new SkillConfig(2010123, "火计", "火", "攻击时对目标放火", "术", 3, 0.3f, 5f, 0f, 0f, "ap", null, 0f, false, "", 1, 0f, 0, 0f, 0f, 0.1f, 0, 0f, 0, "", "", 0, false, 0f, "火", 1, 8f, 3.2f, 1f, 0f, "HitWall", "throw", "SoftFireBigRed", 1.6f, "huo", 20);
            config[2010124] = new SkillConfig(2010124, "火计", "火", "攻击时对目标放火", "术", 4, 0.3f, 5f, 0f, 0f, "ap", null, 0f, false, "", 1, 0f, 0, 0f, 0f, 0.1f, 0, 0f, 0, "", "", 0, false, 0f, "火", 1, 8f, 3.2f, 1f, 0f, "HitWall", "throw", "SoftFireBigRed", 1.6f, "huo", 20);
            config[2010125] = new SkillConfig(2010125, "火计", "火", "攻击时对目标放火", "术", 5, 0.3f, 5f, 0f, 0f, "ap", null, 0f, false, "", 1, 0f, 0, 0f, 0f, 0.1f, 0, 0f, 0, "", "", 0, false, 0f, "火", 1, 8f, 3.2f, 1f, 0f, "HitWall", "throw", "SoftFireBigRed", 1.6f, "huo", 20);
            config[2010131] = new SkillConfig(2010131, "火墙", "炎", "攻击召唤出持续伤害的火墙", "术", 1, 0.3f, 8.5f, 0f, 0f, "ap", null, 0f, false, "", 1, 0f, 0, 0f, 0f, 0.08f, 0, 0f, 0, "", "", 0, false, 0f, "火", 5, 8f, 3.2f, 1f, 0f, "HitWall", "spin", "SoftFireBigRed", 1.6f, "yan", 20);
            config[2010132] = new SkillConfig(2010132, "火墙", "炎", "攻击召唤出持续伤害的火墙", "术", 2, 0.3f, 8.5f, 0f, 0f, "ap", null, 0f, false, "", 1, 0f, 0, 0f, 0f, 0.08f, 0, 0f, 0, "", "", 0, false, 0f, "火", 5, 8f, 3.2f, 1f, 0f, "HitWall", "spin", "SoftFireBigRed", 1.6f, "yan", 20);
            config[2010133] = new SkillConfig(2010133, "火墙", "炎", "攻击召唤出持续伤害的火墙", "术", 3, 0.3f, 8.5f, 0f, 0f, "ap", null, 0f, false, "", 1, 0f, 0, 0f, 0f, 0.08f, 0, 0f, 0, "", "", 0, false, 0f, "火", 5, 8f, 3.2f, 1f, 0f, "HitWall", "spin", "SoftFireBigRed", 1.6f, "yan", 20);
            config[2010134] = new SkillConfig(2010134, "火墙", "炎", "攻击召唤出持续伤害的火墙", "术", 4, 0.3f, 8.5f, 0f, 0f, "ap", null, 0f, false, "", 1, 0f, 0, 0f, 0f, 0.08f, 0, 0f, 0, "", "", 0, false, 0f, "火", 5, 8f, 3.2f, 1f, 0f, "HitWall", "spin", "SoftFireBigRed", 1.6f, "yan", 20);
            config[2010135] = new SkillConfig(2010135, "火墙", "炎", "攻击召唤出持续伤害的火墙", "术", 5, 0.3f, 8.5f, 0f, 0f, "ap", null, 0f, false, "", 1, 0f, 0, 0f, 0f, 0.08f, 0, 0f, 0, "", "", 0, false, 0f, "火", 5, 8f, 3.2f, 1f, 0f, "HitWall", "spin", "SoftFireBigRed", 1.6f, "yan", 20);
            config[2010141] = new SkillConfig(2010141, "飞斧", "斧", "扔出飞斧攻击前方敌人", "技", 1, 1f, 6.7f, 2f, 0f, "might", null, 45f, false, "", 4, 0f, 0, 0f, 0f, 0.4f, 0, 0f, 0, "", "", 0, false, 0f, "武", 0, 9f, 1.5f, 0f, 40f, "AidShockWave", "spin", "AxeExplosion", 3f, "fu3", 20);
            config[2010142] = new SkillConfig(2010142, "飞斧", "斧", "扔出飞斧攻击前方敌人", "技", 2, 1f, 6.7f, 2f, 0f, "might", null, 45f, false, "", 4, 0f, 0, 0f, 0f, 0.4f, 0, 0f, 0, "", "", 0, false, 0f, "武", 0, 9f, 1.5f, 0f, 40f, "AidShockWave", "spin", "AxeExplosion", 3f, "fu3", 20);
            config[2010143] = new SkillConfig(2010143, "飞斧", "斧", "扔出飞斧攻击前方敌人", "技", 3, 1f, 6.7f, 2f, 0f, "might", null, 45f, false, "", 4, 0f, 0, 0f, 0f, 0.4f, 0, 0f, 0, "", "", 0, false, 0f, "武", 0, 9f, 1.5f, 0f, 40f, "AidShockWave", "spin", "AxeExplosion", 3f, "fu3", 20);
            config[2010144] = new SkillConfig(2010144, "飞斧", "斧", "扔出飞斧攻击前方敌人", "技", 4, 1f, 6.7f, 2f, 0f, "might", null, 45f, false, "", 4, 0f, 0, 0f, 0f, 0.4f, 0, 0f, 0, "", "", 0, false, 0f, "武", 0, 9f, 1.5f, 0f, 40f, "AidShockWave", "spin", "AxeExplosion", 3f, "fu3", 20);
            config[2010145] = new SkillConfig(2010145, "飞斧", "斧", "扔出飞斧攻击前方敌人", "技", 5, 1f, 6.7f, 2f, 0f, "might", null, 45f, false, "", 4, 0f, 0, 0f, 0f, 0.4f, 0, 0f, 0, "", "", 0, false, 0f, "武", 0, 9f, 1.5f, 0f, 40f, "AidShockWave", "spin", "AxeExplosion", 3f, "fu3", 20);
            config[2010151] = new SkillConfig(2010151, "驰羽", "羽", "能够射出箭矢", "技", 1, 1f, 7f, 2f, 0f, "atk", null, 42f, false, "", 0, 0f, 0, 0f, 0f, 0.75f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "AidSuddenArrow", "sway", "BulletExplosionBlue", 3f, "yu", 20);
            config[2010152] = new SkillConfig(2010152, "驰羽", "羽", "能够射出箭矢", "技", 2, 1f, 7f, 2f, 0f, "atk", null, 42f, false, "", 0, 0f, 0, 0f, 0f, 0.75f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "AidSuddenArrow", "sway", "BulletExplosionBlue", 3f, "yu", 20);
            config[2010153] = new SkillConfig(2010153, "驰羽", "羽", "能够射出箭矢", "技", 3, 1f, 7f, 2f, 0f, "atk", null, 42f, false, "", 0, 0f, 0, 0f, 0f, 0.75f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "AidSuddenArrow", "sway", "BulletExplosionBlue", 3f, "yu", 20);
            config[2010154] = new SkillConfig(2010154, "驰羽", "羽", "能够射出箭矢", "技", 4, 1f, 7f, 2f, 0f, "atk", null, 42f, false, "", 0, 0f, 0, 0f, 0f, 0.75f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "AidSuddenArrow", "sway", "BulletExplosionBlue", 3f, "yu", 20);
            config[2010155] = new SkillConfig(2010155, "驰羽", "羽", "能够射出箭矢", "技", 5, 1f, 7f, 2f, 0f, "atk", null, 42f, false, "", 0, 0f, 0, 0f, 0f, 0.75f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "AidSuddenArrow", "sway", "BulletExplosionBlue", 3f, "yu", 20);
            config[2010161] = new SkillConfig(2010161, "惊雷", "雷", "召唤3个惊雷攻击前方敌人", "术", 1, 1f, 6.7f, 2f, 0f, "ap", null, 60f, false, "", 4, 0f, 0, 0f, 0f, 0.25f, 0, 0f, 0, "", "", 0, false, 0f, "雷", 0, 11f, 1.5f, 0f, 40f, "AidShockWave", "spin", "NukeMissileFires", 4f, "lei", 20);
            config[2010162] = new SkillConfig(2010162, "惊雷", "雷", "召唤3个惊雷攻击前方敌人", "术", 2, 1f, 6.7f, 2f, 0f, "ap", null, 60f, false, "", 4, 0f, 0, 0f, 0f, 0.25f, 0, 0f, 0, "", "", 0, false, 0f, "雷", 0, 11f, 1.5f, 0f, 40f, "AidShockWave", "spin", "NukeMissileFires", 4f, "lei", 20);
            config[2010163] = new SkillConfig(2010163, "惊雷", "雷", "召唤3个惊雷攻击前方敌人", "术", 3, 1f, 6.7f, 2f, 0f, "ap", null, 60f, false, "", 4, 0f, 0, 0f, 0f, 0.25f, 0, 0f, 0, "", "", 0, false, 0f, "雷", 0, 11f, 1.5f, 0f, 40f, "AidShockWave", "spin", "NukeMissileFires", 4f, "lei", 20);
            config[2010164] = new SkillConfig(2010164, "惊雷", "雷", "召唤3个惊雷攻击前方敌人", "术", 4, 1f, 6.7f, 2f, 0f, "ap", null, 60f, false, "", 4, 0f, 0, 0f, 0f, 0.25f, 0, 0f, 0, "", "", 0, false, 0f, "雷", 0, 11f, 1.5f, 0f, 40f, "AidShockWave", "spin", "NukeMissileFires", 4f, "lei", 20);
            config[2010165] = new SkillConfig(2010165, "惊雷", "雷", "召唤3个惊雷攻击前方敌人", "术", 5, 1f, 6.7f, 2f, 0f, "ap", null, 60f, false, "", 4, 0f, 0, 0f, 0f, 0.25f, 0, 0f, 0, "", "", 0, false, 0f, "雷", 0, 11f, 1.5f, 0f, 40f, "AidShockWave", "spin", "NukeMissileFires", 4f, "lei", 20);
            config[2010171] = new SkillConfig(2010171, "鬼谋", "鬼", "攻击造成生命值比例伤害", "", 1, 0.25f, 2f, 0f, 1f, "ap", null, 0f, false, "", 0, 0.2f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "DamageReal", "sway", "", 0f, "gui", 20);
            config[2010172] = new SkillConfig(2010172, "鬼谋", "鬼", "攻击造成生命值比例伤害", "", 2, 0.25f, 2f, 0f, 1f, "ap", null, 0f, false, "", 0, 0.2f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "DamageReal", "sway", "", 0f, "gui", 20);
            config[2010173] = new SkillConfig(2010173, "鬼谋", "鬼", "攻击造成生命值比例伤害", "", 3, 0.25f, 2f, 0f, 1f, "ap", null, 0f, false, "", 0, 0.2f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "DamageReal", "sway", "", 0f, "gui", 20);
            config[2010174] = new SkillConfig(2010174, "鬼谋", "鬼", "攻击造成生命值比例伤害", "", 4, 0.25f, 2f, 0f, 1f, "ap", null, 0f, false, "", 0, 0.2f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "DamageReal", "sway", "", 0f, "gui", 20);
            config[2010175] = new SkillConfig(2010175, "鬼谋", "鬼", "攻击造成生命值比例伤害", "", 5, 0.25f, 2f, 0f, 1f, "ap", null, 0f, false, "", 0, 0.2f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "DamageReal", "sway", "", 0f, "gui", 20);
            config[2010181] = new SkillConfig(2010181, "斩", "斩", "直接杀死低生命值单位", "", 1, 0f, 7f, 0f, 0.3f, "might", null, 0f, false, "", 0, 1f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "DamageReal", "spin", "SwordHitRedCritical", 0f, "zhan", 20);
            config[2010182] = new SkillConfig(2010182, "斩", "斩", "直接杀死低生命值单位", "", 2, 0f, 7f, 0f, 0.3f, "might", null, 0f, false, "", 0, 1f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "DamageReal", "spin", "SwordHitRedCritical", 0f, "zhan", 20);
            config[2010183] = new SkillConfig(2010183, "斩", "斩", "直接杀死低生命值单位", "", 3, 0f, 7f, 0f, 0.3f, "might", null, 0f, false, "", 0, 1f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "DamageReal", "spin", "SwordHitRedCritical", 0f, "zhan", 20);
            config[2010184] = new SkillConfig(2010184, "斩", "斩", "直接杀死低生命值单位", "", 4, 0f, 7f, 0f, 0.3f, "might", null, 0f, false, "", 0, 1f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "DamageReal", "spin", "SwordHitRedCritical", 0f, "zhan", 20);
            config[2010185] = new SkillConfig(2010185, "斩", "斩", "直接杀死低生命值单位", "", 5, 0f, 7f, 0f, 0.3f, "might", null, 0f, false, "", 0, 1f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "DamageReal", "spin", "SwordHitRedCritical", 0f, "zhan", 20);
            config[2010191] = new SkillConfig(2010191, "魔神", "魔", "攻击时回复生命", "", 1, 0.35f, 6f, 0f, 0f, "might", null, 0f, false, "", 0, 0f, 0, 0f, 1f, 0f, 0, 0f, 0, "", "", 300003, false, 5f, "", 0, 0f, 0f, 0f, 0f, "AttackedBuff", "", "MagicBuffGreen", 0f, "mo", 0);
            config[2010192] = new SkillConfig(2010192, "魔神", "魔", "攻击时回复生命", "", 2, 0.35f, 6f, 0f, 0f, "might", null, 0f, false, "", 0, 0f, 0, 0f, 1f, 0f, 0, 0f, 0, "", "", 300003, false, 5f, "", 0, 0f, 0f, 0f, 0f, "AttackedBuff", "", "MagicBuffGreen", 0f, "mo", 0);
            config[2010193] = new SkillConfig(2010193, "魔神", "魔", "攻击时回复生命", "", 3, 0.35f, 6f, 0f, 0f, "might", null, 0f, false, "", 0, 0f, 0, 0f, 1f, 0f, 0, 0f, 0, "", "", 300003, false, 5f, "", 0, 0f, 0f, 0f, 0f, "AttackedBuff", "", "MagicBuffGreen", 0f, "mo", 0);
            config[2010194] = new SkillConfig(2010194, "魔神", "魔", "攻击时回复生命", "", 4, 0.35f, 6f, 0f, 0f, "might", null, 0f, false, "", 0, 0f, 0, 0f, 1f, 0f, 0, 0f, 0, "", "", 300003, false, 5f, "", 0, 0f, 0f, 0f, 0f, "AttackedBuff", "", "MagicBuffGreen", 0f, "mo", 0);
            config[2010195] = new SkillConfig(2010195, "魔神", "魔", "攻击时回复生命", "", 5, 0.35f, 6f, 0f, 0f, "might", null, 0f, false, "", 0, 0f, 0, 0f, 1f, 0f, 0, 0f, 0, "", "", 300003, false, 5f, "", 0, 0f, 0f, 0f, 0f, "AttackedBuff", "", "MagicBuffGreen", 0f, "mo", 0);
            config[2010201] = new SkillConfig(2010201, "埋伏", "伏", "被攻击时瞬移到远程攻击者附近", "", 1, 1f, 6f, 0f, 0f, "atk", null, 30f, false, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 301001, false, 1f, "", 0, 0f, 0f, 0f, 0f, "HitTeleport", "saw", "MagicNovaBlue", 0f, "fu", 0);
            config[2010202] = new SkillConfig(2010202, "埋伏", "伏", "被攻击时瞬移到远程攻击者附近", "", 2, 1f, 6f, 0f, 0f, "atk", null, 30f, false, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 301001, false, 1f, "", 0, 0f, 0f, 0f, 0f, "HitTeleport", "saw", "MagicNovaBlue", 0f, "fu", 0);
            config[2010203] = new SkillConfig(2010203, "埋伏", "伏", "被攻击时瞬移到远程攻击者附近", "", 3, 1f, 6f, 0f, 0f, "atk", null, 30f, false, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 301001, false, 1f, "", 0, 0f, 0f, 0f, 0f, "HitTeleport", "saw", "MagicNovaBlue", 0f, "fu", 0);
            config[2010204] = new SkillConfig(2010204, "埋伏", "伏", "被攻击时瞬移到远程攻击者附近", "", 4, 1f, 6f, 0f, 0f, "atk", null, 30f, false, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 301001, false, 1f, "", 0, 0f, 0f, 0f, 0f, "HitTeleport", "saw", "MagicNovaBlue", 0f, "fu", 0);
            config[2010205] = new SkillConfig(2010205, "埋伏", "伏", "被攻击时瞬移到远程攻击者附近", "", 5, 1f, 6f, 0f, 0f, "atk", null, 30f, false, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 301001, false, 1f, "", 0, 0f, 0f, 0f, 0f, "HitTeleport", "saw", "MagicNovaBlue", 0f, "fu", 0);
            config[2010211] = new SkillConfig(2010211, "火矢", "矢", "攻击时射出火箭", "技", 1, 0.35f, 3f, 0f, 0f, "might", null, 0f, false, "", 1, 0f, 0, 0f, 0f, 0.12f, 0, 0f, 0, "", "", 0, false, 0f, "火", 1, 8f, 3.2f, 1f, 0f, "HitWall", "throw", "SoftFireBigRed", 1.6f, "shi3", 20);
            config[2010212] = new SkillConfig(2010212, "火矢", "矢", "攻击时射出火箭", "技", 2, 0.35f, 3f, 0f, 0f, "might", null, 0f, false, "", 1, 0f, 0, 0f, 0f, 0.12f, 0, 0f, 0, "", "", 0, false, 0f, "火", 1, 8f, 3.2f, 1f, 0f, "HitWall", "throw", "SoftFireBigRed", 1.6f, "shi3", 20);
            config[2010213] = new SkillConfig(2010213, "火矢", "矢", "攻击时射出火箭", "技", 3, 0.35f, 3f, 0f, 0f, "might", null, 0f, false, "", 1, 0f, 0, 0f, 0f, 0.12f, 0, 0f, 0, "", "", 0, false, 0f, "火", 1, 8f, 3.2f, 1f, 0f, "HitWall", "throw", "SoftFireBigRed", 1.6f, "shi3", 20);
            config[2010214] = new SkillConfig(2010214, "火矢", "矢", "攻击时射出火箭", "技", 4, 0.35f, 3f, 0f, 0f, "might", null, 0f, false, "", 1, 0f, 0, 0f, 0f, 0.12f, 0, 0f, 0, "", "", 0, false, 0f, "火", 1, 8f, 3.2f, 1f, 0f, "HitWall", "throw", "SoftFireBigRed", 1.6f, "shi3", 20);
            config[2010215] = new SkillConfig(2010215, "火矢", "矢", "攻击时射出火箭", "技", 5, 0.35f, 3f, 0f, 0f, "might", null, 0f, false, "", 1, 0f, 0, 0f, 0f, 0.12f, 0, 0f, 0, "", "", 0, false, 0f, "火", 1, 8f, 3.2f, 1f, 0f, "HitWall", "throw", "SoftFireBigRed", 1.6f, "shi3", 20);
            config[2010221] = new SkillConfig(2010221, "虎卫队", "虎", "攻击时几率对目标进行三连击", "技", 1, 0.15f, 6f, 0f, 0f, "atk", null, 0f, false, "", 0, 0f, 0, 0f, 1f, 0f, 2, 0.3f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "HitRepeat", "jumpspin", "SwordHitRedCritical", 0f, "hu", 20);
            config[2010222] = new SkillConfig(2010222, "虎卫队", "虎", "攻击时几率对目标进行三连击", "技", 2, 0.15f, 6f, 0f, 0f, "atk", null, 0f, false, "", 0, 0f, 0, 0f, 1f, 0f, 2, 0.3f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "HitRepeat", "jumpspin", "SwordHitRedCritical", 0f, "hu", 20);
            config[2010223] = new SkillConfig(2010223, "虎卫队", "虎", "攻击时几率对目标进行三连击", "技", 3, 0.15f, 6f, 0f, 0f, "atk", null, 0f, false, "", 0, 0f, 0, 0f, 1f, 0f, 2, 0.3f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "HitRepeat", "jumpspin", "SwordHitRedCritical", 0f, "hu", 20);
            config[2010224] = new SkillConfig(2010224, "虎卫队", "虎", "攻击时几率对目标进行三连击", "技", 4, 0.15f, 6f, 0f, 0f, "atk", null, 0f, false, "", 0, 0f, 0, 0f, 1f, 0f, 2, 0.3f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "HitRepeat", "jumpspin", "SwordHitRedCritical", 0f, "hu", 20);
            config[2010225] = new SkillConfig(2010225, "虎卫队", "虎", "攻击时几率对目标进行三连击", "技", 5, 0.15f, 6f, 0f, 0f, "atk", null, 0f, false, "", 0, 0f, 0, 0f, 1f, 0f, 2, 0.3f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "HitRepeat", "jumpspin", "SwordHitRedCritical", 0f, "hu", 20);
            config[2010231] = new SkillConfig(2010231, "青州兵", "青", "攻击时几率对目标进行2连击", "技", 1, 0.15f, 7f, 0f, 0f, "atk", null, 0f, false, "", 0, 0f, 0, 0f, 1f, 0f, 1, 0.4f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "HitRepeat", "jumpspin", "SwordHitGreenCritical", 0f, "qing", 20);
            config[2010232] = new SkillConfig(2010232, "青州兵", "青", "攻击时几率对目标进行2连击", "技", 2, 0.15f, 7f, 0f, 0f, "atk", null, 0f, false, "", 0, 0f, 0, 0f, 1f, 0f, 1, 0.4f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "HitRepeat", "jumpspin", "SwordHitGreenCritical", 0f, "qing", 20);
            config[2010233] = new SkillConfig(2010233, "青州兵", "青", "攻击时几率对目标进行2连击", "技", 3, 0.15f, 7f, 0f, 0f, "atk", null, 0f, false, "", 0, 0f, 0, 0f, 1f, 0f, 1, 0.4f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "HitRepeat", "jumpspin", "SwordHitGreenCritical", 0f, "qing", 20);
            config[2010234] = new SkillConfig(2010234, "青州兵", "青", "攻击时几率对目标进行2连击", "技", 4, 0.15f, 7f, 0f, 0f, "atk", null, 0f, false, "", 0, 0f, 0, 0f, 1f, 0f, 1, 0.4f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "HitRepeat", "jumpspin", "SwordHitGreenCritical", 0f, "qing", 20);
            config[2010235] = new SkillConfig(2010235, "青州兵", "青", "攻击时几率对目标进行2连击", "技", 5, 0.15f, 7f, 0f, 0f, "atk", null, 0f, false, "", 0, 0f, 0, 0f, 1f, 0f, 1, 0.4f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "HitRepeat", "jumpspin", "SwordHitGreenCritical", 0f, "qing", 20);
            config[2010241] = new SkillConfig(2010241, "乱战", "乱", "攻击晕眩单位造成额外伤害", "", 1, 0f, 3f, 0f, 0f, "atk", null, 0f, false, "", 0, 0f, 0, 0f, 0.7f, 0f, 0, 0f, 0, "", "", 301001, false, 0f, "", 0, 0f, 0f, 0f, 0f, "AttackAddDamage", "saw", "SwordHitRedCritical", 0f, "luan", 20);
            config[2010242] = new SkillConfig(2010242, "乱战", "乱", "攻击晕眩单位造成额外伤害", "", 2, 0f, 3f, 0f, 0f, "atk", null, 0f, false, "", 0, 0f, 0, 0f, 0.7f, 0f, 0, 0f, 0, "", "", 301001, false, 0f, "", 0, 0f, 0f, 0f, 0f, "AttackAddDamage", "saw", "SwordHitRedCritical", 0f, "luan", 20);
            config[2010243] = new SkillConfig(2010243, "乱战", "乱", "攻击晕眩单位造成额外伤害", "", 3, 0f, 3f, 0f, 0f, "atk", null, 0f, false, "", 0, 0f, 0, 0f, 0.7f, 0f, 0, 0f, 0, "", "", 301001, false, 0f, "", 0, 0f, 0f, 0f, 0f, "AttackAddDamage", "saw", "SwordHitRedCritical", 0f, "luan", 20);
            config[2010244] = new SkillConfig(2010244, "乱战", "乱", "攻击晕眩单位造成额外伤害", "", 4, 0f, 3f, 0f, 0f, "atk", null, 0f, false, "", 0, 0f, 0, 0f, 0.7f, 0f, 0, 0f, 0, "", "", 301001, false, 0f, "", 0, 0f, 0f, 0f, 0f, "AttackAddDamage", "saw", "SwordHitRedCritical", 0f, "luan", 20);
            config[2010245] = new SkillConfig(2010245, "乱战", "乱", "攻击晕眩单位造成额外伤害", "", 5, 0f, 3f, 0f, 0f, "atk", null, 0f, false, "", 0, 0f, 0, 0f, 0.7f, 0f, 0, 0f, 0, "", "", 301001, false, 0f, "", 0, 0f, 0f, 0f, 0f, "AttackAddDamage", "saw", "SwordHitRedCritical", 0f, "luan", 20);
            config[2010251] = new SkillConfig(2010251, "虐袭", "虐", "攻击护盾敌人时造成额外伤害", "技", 1, 0f, 0f, 0f, 0f, "might", null, 0f, false, "", 0, 0.5f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 300001, false, 0f, "", 0, 0f, 0f, 0f, 0f, "AttackAntiShield", "sway", "", 0f, "nue", 20);
            config[2010252] = new SkillConfig(2010252, "虐袭", "虐", "攻击护盾敌人时造成额外伤害", "技", 2, 0f, 0f, 0f, 0f, "might", null, 0f, false, "", 0, 0.5f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 300001, false, 0f, "", 0, 0f, 0f, 0f, 0f, "AttackAntiShield", "sway", "", 0f, "nue", 20);
            config[2010253] = new SkillConfig(2010253, "虐袭", "虐", "攻击护盾敌人时造成额外伤害", "技", 3, 0f, 0f, 0f, 0f, "might", null, 0f, false, "", 0, 0.5f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 300001, false, 0f, "", 0, 0f, 0f, 0f, 0f, "AttackAntiShield", "sway", "", 0f, "nue", 20);
            config[2010254] = new SkillConfig(2010254, "虐袭", "虐", "攻击护盾敌人时造成额外伤害", "技", 4, 0f, 0f, 0f, 0f, "might", null, 0f, false, "", 0, 0.5f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 300001, false, 0f, "", 0, 0f, 0f, 0f, 0f, "AttackAntiShield", "sway", "", 0f, "nue", 20);
            config[2010255] = new SkillConfig(2010255, "虐袭", "虐", "攻击护盾敌人时造成额外伤害", "技", 5, 0f, 0f, 0f, 0f, "might", null, 0f, false, "", 0, 0.5f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 300001, false, 0f, "", 0, 0f, 0f, 0f, 0f, "AttackAntiShield", "sway", "", 0f, "nue", 20);
            config[2020011] = new SkillConfig(2020011, "刺甲", "刺", "反弹50%近战伤害", "防御up", 1, 0.3f, 0f, 0f, 0f, "might", new string[]{"might，atk"}, 20f, false, "", 0, 0.5f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "DefFeedback", "", "SwordHitBlue", 0f, "ci", 0);
            config[2020012] = new SkillConfig(2020012, "刺甲", "刺", "反弹50%近战伤害", "防御up", 2, 0.3f, 0f, 0f, 0f, "might", new string[]{"might，atk"}, 20f, false, "", 0, 0.5f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "DefFeedback", "", "SwordHitBlue", 0f, "ci", 0);
            config[2020013] = new SkillConfig(2020013, "刺甲", "刺", "反弹50%近战伤害", "防御up", 3, 0.3f, 0f, 0f, 0f, "might", new string[]{"might，atk"}, 20f, false, "", 0, 0.5f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "DefFeedback", "", "SwordHitBlue", 0f, "ci", 0);
            config[2020014] = new SkillConfig(2020014, "刺甲", "刺", "反弹50%近战伤害", "防御up", 4, 0.3f, 0f, 0f, 0f, "might", new string[]{"might，atk"}, 20f, false, "", 0, 0.5f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "DefFeedback", "", "SwordHitBlue", 0f, "ci", 0);
            config[2020015] = new SkillConfig(2020015, "刺甲", "刺", "反弹50%近战伤害", "防御up", 5, 0.3f, 0f, 0f, 0f, "might", new string[]{"might，atk"}, 20f, false, "", 0, 0.5f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "DefFeedback", "", "SwordHitBlue", 0f, "ci", 0);
            config[2020021] = new SkillConfig(2020021, "藤甲", "藤", "受非智力攻击时高几率发动70%减伤", "", 1, 0.5f, 0.5f, 0f, 0f, "might", new string[]{"might，atk"}, 0f, false, "", 0, 0.7f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "DefPlantSkin", "", "", 0f, "teng", 0);
            config[2020022] = new SkillConfig(2020022, "藤甲", "藤", "受非智力攻击时高几率发动70%减伤", "", 2, 0.5f, 0.5f, 0f, 0f, "might", new string[]{"might，atk"}, 0f, false, "", 0, 0.7f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "DefPlantSkin", "", "", 0f, "teng", 0);
            config[2020023] = new SkillConfig(2020023, "藤甲", "藤", "受非智力攻击时高几率发动70%减伤", "", 3, 0.5f, 0.5f, 0f, 0f, "might", new string[]{"might，atk"}, 0f, false, "", 0, 0.7f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "DefPlantSkin", "", "", 0f, "teng", 0);
            config[2020024] = new SkillConfig(2020024, "藤甲", "藤", "受非智力攻击时高几率发动70%减伤", "", 4, 0.5f, 0.5f, 0f, 0f, "might", new string[]{"might，atk"}, 0f, false, "", 0, 0.7f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "DefPlantSkin", "", "", 0f, "teng", 0);
            config[2020025] = new SkillConfig(2020025, "藤甲", "藤", "受非智力攻击时高几率发动70%减伤", "", 5, 0.5f, 0.5f, 0f, 0f, "might", new string[]{"might，atk"}, 0f, false, "", 0, 0.7f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "DefPlantSkin", "", "", 0f, "teng", 0);
            config[2020031] = new SkillConfig(2020031, "明镜", "镜", "自己和同行队友反弹智力伤害", "防御up", 1, 0.3f, 0f, 0f, 0f, "atk", new string[]{"ap"}, 0f, false, "", 0, 0.5f, 0, 0f, 0f, 0f, 0, 0f, 1, "竟", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "DefFeedback", "", "SwordHitBlue", 0f, "jing", 0);
            config[2020032] = new SkillConfig(2020032, "明镜", "镜", "自己和同行队友反弹智力伤害", "防御up", 2, 0.3f, 0f, 0f, 0f, "atk", new string[]{"ap"}, 0f, false, "", 0, 0.5f, 0, 0f, 0f, 0f, 0, 0f, 1, "竟", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "DefFeedback", "", "SwordHitBlue", 0f, "jing", 0);
            config[2020033] = new SkillConfig(2020033, "明镜", "镜", "自己和同行队友反弹智力伤害", "防御up", 3, 0.3f, 0f, 0f, 0f, "atk", new string[]{"ap"}, 0f, false, "", 0, 0.5f, 0, 0f, 0f, 0f, 0, 0f, 1, "竟", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "DefFeedback", "", "SwordHitBlue", 0f, "jing", 0);
            config[2020034] = new SkillConfig(2020034, "明镜", "镜", "自己和同行队友反弹智力伤害", "防御up", 4, 0.3f, 0f, 0f, 0f, "atk", new string[]{"ap"}, 0f, false, "", 0, 0.5f, 0, 0f, 0f, 0f, 0, 0f, 1, "竟", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "DefFeedback", "", "SwordHitBlue", 0f, "jing", 0);
            config[2020035] = new SkillConfig(2020035, "明镜", "镜", "自己和同行队友反弹智力伤害", "防御up", 5, 0.3f, 0f, 0f, 0f, "atk", new string[]{"ap"}, 0f, false, "", 0, 0.5f, 0, 0f, 0f, 0f, 0, 0f, 1, "竟", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "DefFeedback", "", "SwordHitBlue", 0f, "jing", 0);
            config[2020041] = new SkillConfig(2020041, "明镜小", "竟", "反弹30%智力伤害", "防御up", 1, 0.3f, 0f, 0f, 0f, "atk", new string[]{"ap"}, 0f, false, "", 0, 0.3f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "DefFeedback", "", "SwordHitBlue", 0f, "jing2", 0);
            config[2020042] = new SkillConfig(2020042, "明镜小", "竟", "反弹30%智力伤害", "防御up", 2, 0.3f, 0f, 0f, 0f, "atk", new string[]{"ap"}, 0f, false, "", 0, 0.3f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "DefFeedback", "", "SwordHitBlue", 0f, "jing2", 0);
            config[2020043] = new SkillConfig(2020043, "明镜小", "竟", "反弹30%智力伤害", "防御up", 3, 0.3f, 0f, 0f, 0f, "atk", new string[]{"ap"}, 0f, false, "", 0, 0.3f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "DefFeedback", "", "SwordHitBlue", 0f, "jing2", 0);
            config[2020044] = new SkillConfig(2020044, "明镜小", "竟", "反弹30%智力伤害", "防御up", 4, 0.3f, 0f, 0f, 0f, "atk", new string[]{"ap"}, 0f, false, "", 0, 0.3f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "DefFeedback", "", "SwordHitBlue", 0f, "jing2", 0);
            config[2020045] = new SkillConfig(2020045, "明镜小", "竟", "反弹30%智力伤害", "防御up", 5, 0.3f, 0f, 0f, 0f, "atk", new string[]{"ap"}, 0f, false, "", 0, 0.3f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "DefFeedback", "", "SwordHitBlue", 0f, "jing2", 0);
            config[2020051] = new SkillConfig(2020051, "护卫", "护", "给与友军护盾祝福", "", 1, 1f, 8f, 1.5f, 0f, "might", null, 50f, false, "", 0, 0.18f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 300001, false, 10f, "", 0, 0f, 0f, 0f, 0f, "HelpAidBuff", "sway", "MagicChargeYellow", 0f, "hu1", 20);
            config[2020052] = new SkillConfig(2020052, "护卫", "护", "给与友军护盾祝福", "", 2, 1f, 8f, 1.5f, 0f, "might", null, 50f, false, "", 0, 0.18f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 300001, false, 10f, "", 0, 0f, 0f, 0f, 0f, "HelpAidBuff", "sway", "MagicChargeYellow", 0f, "hu1", 20);
            config[2020053] = new SkillConfig(2020053, "护卫", "护", "给与友军护盾祝福", "", 3, 1f, 8f, 1.5f, 0f, "might", null, 50f, false, "", 0, 0.18f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 300001, false, 10f, "", 0, 0f, 0f, 0f, 0f, "HelpAidBuff", "sway", "MagicChargeYellow", 0f, "hu1", 20);
            config[2020054] = new SkillConfig(2020054, "护卫", "护", "给与友军护盾祝福", "", 4, 1f, 8f, 1.5f, 0f, "might", null, 50f, false, "", 0, 0.18f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 300001, false, 10f, "", 0, 0f, 0f, 0f, 0f, "HelpAidBuff", "sway", "MagicChargeYellow", 0f, "hu1", 20);
            config[2020055] = new SkillConfig(2020055, "护卫", "护", "给与友军护盾祝福", "", 5, 1f, 8f, 1.5f, 0f, "might", null, 50f, false, "", 0, 0.18f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 300001, false, 10f, "", 0, 0f, 0f, 0f, 0f, "HelpAidBuff", "sway", "MagicChargeYellow", 0f, "hu1", 20);
            config[2020061] = new SkillConfig(2020061, "坚毅", "坚", "生命值低时降低50%伤害", "防御up", 1, 0.3f, 0.5f, 0f, 0.35f, "might", null, 0f, false, "", 0, 0.5f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "DefHpLow", "", "", 0f, "jian", 0);
            config[2020062] = new SkillConfig(2020062, "坚毅", "坚", "生命值低时降低50%伤害", "防御up", 2, 0.3f, 0.5f, 0f, 0.35f, "might", null, 0f, false, "", 0, 0.5f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "DefHpLow", "", "", 0f, "jian", 0);
            config[2020063] = new SkillConfig(2020063, "坚毅", "坚", "生命值低时降低50%伤害", "防御up", 3, 0.3f, 0.5f, 0f, 0.35f, "might", null, 0f, false, "", 0, 0.5f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "DefHpLow", "", "", 0f, "jian", 0);
            config[2020064] = new SkillConfig(2020064, "坚毅", "坚", "生命值低时降低50%伤害", "防御up", 4, 0.3f, 0.5f, 0f, 0.35f, "might", null, 0f, false, "", 0, 0.5f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "DefHpLow", "", "", 0f, "jian", 0);
            config[2020065] = new SkillConfig(2020065, "坚毅", "坚", "生命值低时降低50%伤害", "防御up", 5, 0.3f, 0.5f, 0f, 0.35f, "might", null, 0f, false, "", 0, 0.5f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "DefHpLow", "", "", 0f, "jian", 0);
            config[2020071] = new SkillConfig(2020071, "识破", "识", "同行降低智力类技能伤害", "防御up", 1, 0.3f, 0.5f, 0f, 0f, "atk", new string[]{"ap"}, 0f, false, "", 0, 0.5f, 0, 0f, 0f, 0f, 0, 0f, 1, "实", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "DefSkillDamageReduce", "", "", 0f, "shi5", 0);
            config[2020072] = new SkillConfig(2020072, "识破", "识", "同行降低智力类技能伤害", "防御up", 2, 0.3f, 0.5f, 0f, 0f, "atk", new string[]{"ap"}, 0f, false, "", 0, 0.5f, 0, 0f, 0f, 0f, 0, 0f, 1, "实", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "DefSkillDamageReduce", "", "", 0f, "shi5", 0);
            config[2020073] = new SkillConfig(2020073, "识破", "识", "同行降低智力类技能伤害", "防御up", 3, 0.3f, 0.5f, 0f, 0f, "atk", new string[]{"ap"}, 0f, false, "", 0, 0.5f, 0, 0f, 0f, 0f, 0, 0f, 1, "实", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "DefSkillDamageReduce", "", "", 0f, "shi5", 0);
            config[2020074] = new SkillConfig(2020074, "识破", "识", "同行降低智力类技能伤害", "防御up", 4, 0.3f, 0.5f, 0f, 0f, "atk", new string[]{"ap"}, 0f, false, "", 0, 0.5f, 0, 0f, 0f, 0f, 0, 0f, 1, "实", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "DefSkillDamageReduce", "", "", 0f, "shi5", 0);
            config[2020075] = new SkillConfig(2020075, "识破", "识", "同行降低智力类技能伤害", "防御up", 5, 0.3f, 0.5f, 0f, 0f, "atk", new string[]{"ap"}, 0f, false, "", 0, 0.5f, 0, 0f, 0f, 0f, 0, 0f, 1, "实", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "DefSkillDamageReduce", "", "", 0f, "shi5", 0);
            config[2020081] = new SkillConfig(2020081, "识破小", "实", "降低智力类技能伤害", "防御up", 1, 0.3f, 0.5f, 0f, 0f, "atk", new string[]{"ap"}, 0f, false, "", 0, 0.5f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "DefSkillDamageReduce", "", "", 0f, "shi4", 0);
            config[2020082] = new SkillConfig(2020082, "识破小", "实", "降低智力类技能伤害", "防御up", 2, 0.3f, 0.5f, 0f, 0f, "atk", new string[]{"ap"}, 0f, false, "", 0, 0.5f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "DefSkillDamageReduce", "", "", 0f, "shi4", 0);
            config[2020083] = new SkillConfig(2020083, "识破小", "实", "降低智力类技能伤害", "防御up", 3, 0.3f, 0.5f, 0f, 0f, "atk", new string[]{"ap"}, 0f, false, "", 0, 0.5f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "DefSkillDamageReduce", "", "", 0f, "shi4", 0);
            config[2020084] = new SkillConfig(2020084, "识破小", "实", "降低智力类技能伤害", "防御up", 4, 0.3f, 0.5f, 0f, 0f, "atk", new string[]{"ap"}, 0f, false, "", 0, 0.5f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "DefSkillDamageReduce", "", "", 0f, "shi4", 0);
            config[2020085] = new SkillConfig(2020085, "识破小", "实", "降低智力类技能伤害", "防御up", 5, 0.3f, 0.5f, 0f, 0f, "atk", new string[]{"ap"}, 0f, false, "", 0, 0.5f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "DefSkillDamageReduce", "", "", 0f, "shi4", 0);
            config[2020091] = new SkillConfig(2020091, "冷静", "静", "降低同行智力类技能造成的负面状态时间", "防御up", 1, 0f, 0f, 0f, 0f, "atk", new string[]{"ap"}, 0f, false, "", 0, 0.5f, 0, 0f, 0f, 0f, 0, 0f, 1, "境", "", 0, true, 0f, "", 0, 0f, 0f, 0f, 0f, "ModifyBeBuffTime", "", "", 0f, "jing3", 0);
            config[2020092] = new SkillConfig(2020092, "冷静", "静", "降低同行智力类技能造成的负面状态时间", "防御up", 2, 0f, 0f, 0f, 0f, "atk", new string[]{"ap"}, 0f, false, "", 0, 0.5f, 0, 0f, 0f, 0f, 0, 0f, 1, "境", "", 0, true, 0f, "", 0, 0f, 0f, 0f, 0f, "ModifyBeBuffTime", "", "", 0f, "jing3", 0);
            config[2020093] = new SkillConfig(2020093, "冷静", "静", "降低同行智力类技能造成的负面状态时间", "防御up", 3, 0f, 0f, 0f, 0f, "atk", new string[]{"ap"}, 0f, false, "", 0, 0.5f, 0, 0f, 0f, 0f, 0, 0f, 1, "境", "", 0, true, 0f, "", 0, 0f, 0f, 0f, 0f, "ModifyBeBuffTime", "", "", 0f, "jing3", 0);
            config[2020094] = new SkillConfig(2020094, "冷静", "静", "降低同行智力类技能造成的负面状态时间", "防御up", 4, 0f, 0f, 0f, 0f, "atk", new string[]{"ap"}, 0f, false, "", 0, 0.5f, 0, 0f, 0f, 0f, 0, 0f, 1, "境", "", 0, true, 0f, "", 0, 0f, 0f, 0f, 0f, "ModifyBeBuffTime", "", "", 0f, "jing3", 0);
            config[2020095] = new SkillConfig(2020095, "冷静", "静", "降低同行智力类技能造成的负面状态时间", "防御up", 5, 0f, 0f, 0f, 0f, "atk", new string[]{"ap"}, 0f, false, "", 0, 0.5f, 0, 0f, 0f, 0f, 0, 0f, 1, "境", "", 0, true, 0f, "", 0, 0f, 0f, 0f, 0f, "ModifyBeBuffTime", "", "", 0f, "jing3", 0);
            config[2020101] = new SkillConfig(2020101, "冷静小", "境", "降低智力类技能造成的负面状态时间", "防御up", 1, 0f, 0f, 0f, 0f, "atk", new string[]{"ap"}, 0f, false, "", 0, 0.5f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, true, 0f, "", 0, 0f, 0f, 0f, 0f, "ModifyBeBuffTime", "", "", 0f, "jing4", 0);
            config[2020102] = new SkillConfig(2020102, "冷静小", "境", "降低智力类技能造成的负面状态时间", "防御up", 2, 0f, 0f, 0f, 0f, "atk", new string[]{"ap"}, 0f, false, "", 0, 0.5f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, true, 0f, "", 0, 0f, 0f, 0f, 0f, "ModifyBeBuffTime", "", "", 0f, "jing4", 0);
            config[2020103] = new SkillConfig(2020103, "冷静小", "境", "降低智力类技能造成的负面状态时间", "防御up", 3, 0f, 0f, 0f, 0f, "atk", new string[]{"ap"}, 0f, false, "", 0, 0.5f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, true, 0f, "", 0, 0f, 0f, 0f, 0f, "ModifyBeBuffTime", "", "", 0f, "jing4", 0);
            config[2020104] = new SkillConfig(2020104, "冷静小", "境", "降低智力类技能造成的负面状态时间", "防御up", 4, 0f, 0f, 0f, 0f, "atk", new string[]{"ap"}, 0f, false, "", 0, 0.5f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, true, 0f, "", 0, 0f, 0f, 0f, 0f, "ModifyBeBuffTime", "", "", 0f, "jing4", 0);
            config[2020105] = new SkillConfig(2020105, "冷静小", "境", "降低智力类技能造成的负面状态时间", "防御up", 5, 0f, 0f, 0f, 0f, "atk", new string[]{"ap"}, 0f, false, "", 0, 0.5f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, true, 0f, "", 0, 0f, 0f, 0f, 0f, "ModifyBeBuffTime", "", "", 0f, "jing4", 0);
            config[2020111] = new SkillConfig(2020111, "敏锐", "敏", "提升15%闪避率", "防御up", 1, 0f, 0f, 0f, 0f, "might", null, 0f, false, "", 0, 0.2f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "InitAddDodge", "", "", 0f, "min", 0);
            config[2020112] = new SkillConfig(2020112, "敏锐", "敏", "提升15%闪避率", "防御up", 2, 0f, 0f, 0f, 0f, "might", null, 0f, false, "", 0, 0.2f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "InitAddDodge", "", "", 0f, "min", 0);
            config[2020113] = new SkillConfig(2020113, "敏锐", "敏", "提升15%闪避率", "防御up", 3, 0f, 0f, 0f, 0f, "might", null, 0f, false, "", 0, 0.2f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "InitAddDodge", "", "", 0f, "min", 0);
            config[2020114] = new SkillConfig(2020114, "敏锐", "敏", "提升15%闪避率", "防御up", 4, 0f, 0f, 0f, 0f, "might", null, 0f, false, "", 0, 0.2f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "InitAddDodge", "", "", 0f, "min", 0);
            config[2020115] = new SkillConfig(2020115, "敏锐", "敏", "提升15%闪避率", "防御up", 5, 0f, 0f, 0f, 0f, "might", null, 0f, false, "", 0, 0.2f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "InitAddDodge", "", "", 0f, "min", 0);
            config[2020121] = new SkillConfig(2020121, "空城", "空", "自己和同列队友提升15%闪避率", "防御up", 1, 0f, 0f, 0f, 0f, "ap", null, 0f, false, "", 0, 0.2f, 0, 0f, 0f, 0f, 0, 0f, 2, "敏", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "InitAddDodge", "", "", 0f, "kong", 0);
            config[2020122] = new SkillConfig(2020122, "空城", "空", "自己和同列队友提升15%闪避率", "防御up", 2, 0f, 0f, 0f, 0f, "ap", null, 0f, false, "", 0, 0.2f, 0, 0f, 0f, 0f, 0, 0f, 2, "敏", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "InitAddDodge", "", "", 0f, "kong", 0);
            config[2020123] = new SkillConfig(2020123, "空城", "空", "自己和同列队友提升15%闪避率", "防御up", 3, 0f, 0f, 0f, 0f, "ap", null, 0f, false, "", 0, 0.2f, 0, 0f, 0f, 0f, 0, 0f, 2, "敏", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "InitAddDodge", "", "", 0f, "kong", 0);
            config[2020124] = new SkillConfig(2020124, "空城", "空", "自己和同列队友提升15%闪避率", "防御up", 4, 0f, 0f, 0f, 0f, "ap", null, 0f, false, "", 0, 0.2f, 0, 0f, 0f, 0f, 0, 0f, 2, "敏", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "InitAddDodge", "", "", 0f, "kong", 0);
            config[2020125] = new SkillConfig(2020125, "空城", "空", "自己和同列队友提升15%闪避率", "防御up", 5, 0f, 0f, 0f, 0f, "ap", null, 0f, false, "", 0, 0.2f, 0, 0f, 0f, 0f, 0, 0f, 2, "敏", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "InitAddDodge", "", "", 0f, "kong", 0);
            config[2020131] = new SkillConfig(2020131, "复原", "复", "提升5点生命回复", "防御up", 1, 0f, 0f, 0f, 0f, "atk", null, 0f, false, "", 0, 0f, 5, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "InitAddRege", "", "", 0f, "fu2", 0);
            config[2020132] = new SkillConfig(2020132, "复原", "复", "提升5点生命回复", "防御up", 2, 0f, 0f, 0f, 0f, "atk", null, 0f, false, "", 0, 0f, 5, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "InitAddRege", "", "", 0f, "fu2", 0);
            config[2020133] = new SkillConfig(2020133, "复原", "复", "提升5点生命回复", "防御up", 3, 0f, 0f, 0f, 0f, "atk", null, 0f, false, "", 0, 0f, 5, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "InitAddRege", "", "", 0f, "fu2", 0);
            config[2020134] = new SkillConfig(2020134, "复原", "复", "提升5点生命回复", "防御up", 4, 0f, 0f, 0f, 0f, "atk", null, 0f, false, "", 0, 0f, 5, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "InitAddRege", "", "", 0f, "fu2", 0);
            config[2020135] = new SkillConfig(2020135, "复原", "复", "提升5点生命回复", "防御up", 5, 0f, 0f, 0f, 0f, "atk", null, 0f, false, "", 0, 0f, 5, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "InitAddRege", "", "", 0f, "fu2", 0);
            config[2020141] = new SkillConfig(2020141, "药仙", "药", "自己和所有队友提升5点生命回复", "防御up", 1, 0f, 0f, 0f, 0f, "ap", null, 0f, false, "", 0, 0f, 5, 0f, 0f, 0f, 0, 0f, 3, "复", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "InitAddRege", "", "", 0f, "yao", 0);
            config[2020142] = new SkillConfig(2020142, "药仙", "药", "自己和所有队友提升5点生命回复", "防御up", 2, 0f, 0f, 0f, 0f, "ap", null, 0f, false, "", 0, 0f, 5, 0f, 0f, 0f, 0, 0f, 3, "复", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "InitAddRege", "", "", 0f, "yao", 0);
            config[2020143] = new SkillConfig(2020143, "药仙", "药", "自己和所有队友提升5点生命回复", "防御up", 3, 0f, 0f, 0f, 0f, "ap", null, 0f, false, "", 0, 0f, 5, 0f, 0f, 0f, 0, 0f, 3, "复", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "InitAddRege", "", "", 0f, "yao", 0);
            config[2020144] = new SkillConfig(2020144, "药仙", "药", "自己和所有队友提升5点生命回复", "防御up", 4, 0f, 0f, 0f, 0f, "ap", null, 0f, false, "", 0, 0f, 5, 0f, 0f, 0f, 0, 0f, 3, "复", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "InitAddRege", "", "", 0f, "yao", 0);
            config[2020145] = new SkillConfig(2020145, "药仙", "药", "自己和所有队友提升5点生命回复", "防御up", 5, 0f, 0f, 0f, 0f, "ap", null, 0f, false, "", 0, 0f, 5, 0f, 0f, 0f, 0, 0f, 3, "复", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "InitAddRege", "", "", 0f, "yao", 0);
            config[2030021] = new SkillConfig(2030021, "连锁", "锁", "锁定敌人目标,传递一半受到伤害", "术", 1, 0.5f, 3f, 0f, 0f, "ap", null, 25f, false, "targetUnit", 1, 0f, 0, 0f, 0.5f, 0f, 0, 0f, 0, "", "", 301002, false, 8f, "", 0, 0f, 0f, 0f, 0f, "HitBuffArea", "throw", "MagicNovaYellow", 0f, "suo", 20);
            config[2030022] = new SkillConfig(2030022, "连锁", "锁", "锁定敌人目标,传递一半受到伤害", "术", 2, 0.5f, 3f, 0f, 0f, "ap", null, 25f, false, "targetUnit", 1, 0f, 0, 0f, 0.5f, 0f, 0, 0f, 0, "", "", 301002, false, 8f, "", 0, 0f, 0f, 0f, 0f, "HitBuffArea", "throw", "MagicNovaYellow", 0f, "suo", 20);
            config[2030023] = new SkillConfig(2030023, "连锁", "锁", "锁定敌人目标,传递一半受到伤害", "术", 3, 0.5f, 3f, 0f, 0f, "ap", null, 25f, false, "targetUnit", 1, 0f, 0, 0f, 0.5f, 0f, 0, 0f, 0, "", "", 301002, false, 8f, "", 0, 0f, 0f, 0f, 0f, "HitBuffArea", "throw", "MagicNovaYellow", 0f, "suo", 20);
            config[2030024] = new SkillConfig(2030024, "连锁", "锁", "锁定敌人目标,传递一半受到伤害", "术", 4, 0.5f, 3f, 0f, 0f, "ap", null, 25f, false, "targetUnit", 1, 0f, 0, 0f, 0.5f, 0f, 0, 0f, 0, "", "", 301002, false, 8f, "", 0, 0f, 0f, 0f, 0f, "HitBuffArea", "throw", "MagicNovaYellow", 0f, "suo", 20);
            config[2030025] = new SkillConfig(2030025, "连锁", "锁", "锁定敌人目标,传递一半受到伤害", "术", 5, 0.5f, 3f, 0f, 0f, "ap", null, 25f, false, "targetUnit", 1, 0f, 0, 0f, 0.5f, 0f, 0, 0f, 0, "", "", 301002, false, 8f, "", 0, 0f, 0f, 0f, 0f, "HitBuffArea", "throw", "MagicNovaYellow", 0f, "suo", 20);
            config[2030041] = new SkillConfig(2030041, "威震", "威", "攻击时混乱周围目标", "", 1, 0.2f, 5f, 0f, 0f, "might", null, 20f, false, "castUnit", 3, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 301001, false, 1.5f, "", 0, 0f, 0f, 0f, 0f, "HitBuffArea", "spin", "MagicNovaYellow", 0f, "wei", 20);
            config[2030042] = new SkillConfig(2030042, "威震", "威", "攻击时混乱周围目标", "", 2, 0.2f, 5f, 0f, 0f, "might", null, 20f, false, "castUnit", 3, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 301001, false, 1.5f, "", 0, 0f, 0f, 0f, 0f, "HitBuffArea", "spin", "MagicNovaYellow", 0f, "wei", 20);
            config[2030043] = new SkillConfig(2030043, "威震", "威", "攻击时混乱周围目标", "", 3, 0.2f, 5f, 0f, 0f, "might", null, 20f, false, "castUnit", 3, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 301001, false, 1.5f, "", 0, 0f, 0f, 0f, 0f, "HitBuffArea", "spin", "MagicNovaYellow", 0f, "wei", 20);
            config[2030044] = new SkillConfig(2030044, "威震", "威", "攻击时混乱周围目标", "", 4, 0.2f, 5f, 0f, 0f, "might", null, 20f, false, "castUnit", 3, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 301001, false, 1.5f, "", 0, 0f, 0f, 0f, 0f, "HitBuffArea", "spin", "MagicNovaYellow", 0f, "wei", 20);
            config[2030045] = new SkillConfig(2030045, "威震", "威", "攻击时混乱周围目标", "", 5, 0.2f, 5f, 0f, 0f, "might", null, 20f, false, "castUnit", 3, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 301001, false, 1.5f, "", 0, 0f, 0f, 0f, 0f, "HitBuffArea", "spin", "MagicNovaYellow", 0f, "wei", 20);
            config[2030051] = new SkillConfig(2030051, "击破", "破", "攻击几率使目标增伤40%", "", 1, 0.4f, 2f, 0f, 0f, "might", null, 0f, false, "", 0, 0.4f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 301003, false, 3f, "", 0, 0f, 0f, 0f, 0f, "HitBuff", "jump", "SoftFireBigRed", 0f, "po", 20);
            config[2030052] = new SkillConfig(2030052, "击破", "破", "攻击几率使目标增伤40%", "", 2, 0.4f, 2f, 0f, 0f, "might", null, 0f, false, "", 0, 0.4f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 301003, false, 3f, "", 0, 0f, 0f, 0f, 0f, "HitBuff", "jump", "SoftFireBigRed", 0f, "po", 20);
            config[2030053] = new SkillConfig(2030053, "击破", "破", "攻击几率使目标增伤40%", "", 3, 0.4f, 2f, 0f, 0f, "might", null, 0f, false, "", 0, 0.4f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 301003, false, 3f, "", 0, 0f, 0f, 0f, 0f, "HitBuff", "jump", "SoftFireBigRed", 0f, "po", 20);
            config[2030054] = new SkillConfig(2030054, "击破", "破", "攻击几率使目标增伤40%", "", 4, 0.4f, 2f, 0f, 0f, "might", null, 0f, false, "", 0, 0.4f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 301003, false, 3f, "", 0, 0f, 0f, 0f, 0f, "HitBuff", "jump", "SoftFireBigRed", 0f, "po", 20);
            config[2030055] = new SkillConfig(2030055, "击破", "破", "攻击几率使目标增伤40%", "", 5, 0.4f, 2f, 0f, 0f, "might", null, 0f, false, "", 0, 0.4f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 301003, false, 3f, "", 0, 0f, 0f, 0f, 0f, "HitBuff", "jump", "SoftFireBigRed", 0f, "po", 20);
            config[2030061] = new SkillConfig(2030061, "延缓", "缓", "攻击几率使目标减速30%", "", 1, 0.4f, 3f, 0f, 0f, "ap", null, 0f, false, "", 0, 0.3f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 301004, false, 5f, "", 0, 0f, 0f, 0f, 0f, "HitBuff", "sway", "MagicNovaYellow", 0f, "huan", 20);
            config[2030062] = new SkillConfig(2030062, "延缓", "缓", "攻击几率使目标减速30%", "", 2, 0.4f, 3f, 0f, 0f, "ap", null, 0f, false, "", 0, 0.3f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 301004, false, 5f, "", 0, 0f, 0f, 0f, 0f, "HitBuff", "sway", "MagicNovaYellow", 0f, "huan", 20);
            config[2030063] = new SkillConfig(2030063, "延缓", "缓", "攻击几率使目标减速30%", "", 3, 0.4f, 3f, 0f, 0f, "ap", null, 0f, false, "", 0, 0.3f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 301004, false, 5f, "", 0, 0f, 0f, 0f, 0f, "HitBuff", "sway", "MagicNovaYellow", 0f, "huan", 20);
            config[2030064] = new SkillConfig(2030064, "延缓", "缓", "攻击几率使目标减速30%", "", 4, 0.4f, 3f, 0f, 0f, "ap", null, 0f, false, "", 0, 0.3f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 301004, false, 5f, "", 0, 0f, 0f, 0f, 0f, "HitBuff", "sway", "MagicNovaYellow", 0f, "huan", 20);
            config[2030065] = new SkillConfig(2030065, "延缓", "缓", "攻击几率使目标减速30%", "", 5, 0.4f, 3f, 0f, 0f, "ap", null, 0f, false, "", 0, 0.3f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 301004, false, 5f, "", 0, 0f, 0f, 0f, 0f, "HitBuff", "sway", "MagicNovaYellow", 0f, "huan", 20);
            config[2030071] = new SkillConfig(2030071, "陷阵", "陷", "攻击几率使目标陷阵", "", 1, 0.4f, 3f, 0f, 0f, "ap", null, 0f, false, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 301005, false, 4f, "", 0, 0f, 0f, 0f, 0f, "HitBuff", "sway", "MagicNovaYellow", 0f, "xian", 20);
            config[2030072] = new SkillConfig(2030072, "陷阵", "陷", "攻击几率使目标陷阵", "", 2, 0.4f, 3f, 0f, 0f, "ap", null, 0f, false, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 301005, false, 4f, "", 0, 0f, 0f, 0f, 0f, "HitBuff", "sway", "MagicNovaYellow", 0f, "xian", 20);
            config[2030073] = new SkillConfig(2030073, "陷阵", "陷", "攻击几率使目标陷阵", "", 3, 0.4f, 3f, 0f, 0f, "ap", null, 0f, false, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 301005, false, 4f, "", 0, 0f, 0f, 0f, 0f, "HitBuff", "sway", "MagicNovaYellow", 0f, "xian", 20);
            config[2030074] = new SkillConfig(2030074, "陷阵", "陷", "攻击几率使目标陷阵", "", 4, 0.4f, 3f, 0f, 0f, "ap", null, 0f, false, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 301005, false, 4f, "", 0, 0f, 0f, 0f, 0f, "HitBuff", "sway", "MagicNovaYellow", 0f, "xian", 20);
            config[2030075] = new SkillConfig(2030075, "陷阵", "陷", "攻击几率使目标陷阵", "", 5, 0.4f, 3f, 0f, 0f, "ap", null, 0f, false, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 301005, false, 4f, "", 0, 0f, 0f, 0f, 0f, "HitBuff", "sway", "MagicNovaYellow", 0f, "xian", 20);
            config[2030081] = new SkillConfig(2030081, "溃散", "溃", "攻击几率使目标溃败", "", 1, 0.4f, 4f, 0f, 0f, "ap", null, 0f, false, "", 0, 0f, 0, 0f, 0f, 0.1f, 0, 0f, 0, "", "", 301006, false, 5.2f, "", 0, 0f, 0f, 0f, 0f, "HitBuff", "sway", "MagicNovaYellow", 0f, "kui", 20);
            config[2030082] = new SkillConfig(2030082, "溃散", "溃", "攻击几率使目标溃败", "", 2, 0.4f, 4f, 0f, 0f, "ap", null, 0f, false, "", 0, 0f, 0, 0f, 0f, 0.1f, 0, 0f, 0, "", "", 301006, false, 5.2f, "", 0, 0f, 0f, 0f, 0f, "HitBuff", "sway", "MagicNovaYellow", 0f, "kui", 20);
            config[2030083] = new SkillConfig(2030083, "溃散", "溃", "攻击几率使目标溃败", "", 3, 0.4f, 4f, 0f, 0f, "ap", null, 0f, false, "", 0, 0f, 0, 0f, 0f, 0.1f, 0, 0f, 0, "", "", 301006, false, 5.2f, "", 0, 0f, 0f, 0f, 0f, "HitBuff", "sway", "MagicNovaYellow", 0f, "kui", 20);
            config[2030084] = new SkillConfig(2030084, "溃散", "溃", "攻击几率使目标溃败", "", 4, 0.4f, 4f, 0f, 0f, "ap", null, 0f, false, "", 0, 0f, 0, 0f, 0f, 0.1f, 0, 0f, 0, "", "", 301006, false, 5.2f, "", 0, 0f, 0f, 0f, 0f, "HitBuff", "sway", "MagicNovaYellow", 0f, "kui", 20);
            config[2030085] = new SkillConfig(2030085, "溃散", "溃", "攻击几率使目标溃败", "", 5, 0.4f, 4f, 0f, 0f, "ap", null, 0f, false, "", 0, 0f, 0, 0f, 0f, 0.1f, 0, 0f, 0, "", "", 301006, false, 5.2f, "", 0, 0f, 0f, 0f, 0f, "HitBuff", "sway", "MagicNovaYellow", 0f, "kui", 20);
            config[2030091] = new SkillConfig(2030091, "分兵", "分", "被攻击时产生一只有伤害部队", "", 1, 0.4f, 15f, 0f, 0f, "atk", null, 15f, false, "", 0, 0f, 0, 0.5f, 0.5f, 0f, 4, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "AttackedShadow", "sway", "MagicFieldGreen", 0f, "fen2", 0);
            config[2030092] = new SkillConfig(2030092, "分兵", "分", "被攻击时产生一只有伤害部队", "", 2, 0.4f, 15f, 0f, 0f, "atk", null, 15f, false, "", 0, 0f, 0, 0.5f, 0.5f, 0f, 4, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "AttackedShadow", "sway", "MagicFieldGreen", 0f, "fen2", 0);
            config[2030093] = new SkillConfig(2030093, "分兵", "分", "被攻击时产生一只有伤害部队", "", 3, 0.4f, 15f, 0f, 0f, "atk", null, 15f, false, "", 0, 0f, 0, 0.5f, 0.5f, 0f, 4, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "AttackedShadow", "sway", "MagicFieldGreen", 0f, "fen2", 0);
            config[2030094] = new SkillConfig(2030094, "分兵", "分", "被攻击时产生一只有伤害部队", "", 4, 0.4f, 15f, 0f, 0f, "atk", null, 15f, false, "", 0, 0f, 0, 0.5f, 0.5f, 0f, 4, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "AttackedShadow", "sway", "MagicFieldGreen", 0f, "fen2", 0);
            config[2030095] = new SkillConfig(2030095, "分兵", "分", "被攻击时产生一只有伤害部队", "", 5, 0.4f, 15f, 0f, 0f, "atk", null, 15f, false, "", 0, 0f, 0, 0.5f, 0.5f, 0f, 4, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "AttackedShadow", "sway", "MagicFieldGreen", 0f, "fen2", 0);
            config[2030101] = new SkillConfig(2030101, "分兵小", "纷", "被攻击时产生一只无伤害部队", "", 1, 0.4f, 15f, 0f, 0f, "atk", null, 15f, false, "", 0, 0f, 0, 0.4f, 0.01f, 0f, 4, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "AttackedShadow", "sway", "MagicFieldGreen", 0f, "fen3", 0);
            config[2030102] = new SkillConfig(2030102, "分兵小", "纷", "被攻击时产生一只无伤害部队", "", 2, 0.4f, 15f, 0f, 0f, "atk", null, 15f, false, "", 0, 0f, 0, 0.4f, 0.01f, 0f, 4, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "AttackedShadow", "sway", "MagicFieldGreen", 0f, "fen3", 0);
            config[2030103] = new SkillConfig(2030103, "分兵小", "纷", "被攻击时产生一只无伤害部队", "", 3, 0.4f, 15f, 0f, 0f, "atk", null, 15f, false, "", 0, 0f, 0, 0.4f, 0.01f, 0f, 4, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "AttackedShadow", "sway", "MagicFieldGreen", 0f, "fen3", 0);
            config[2030104] = new SkillConfig(2030104, "分兵小", "纷", "被攻击时产生一只无伤害部队", "", 4, 0.4f, 15f, 0f, 0f, "atk", null, 15f, false, "", 0, 0f, 0, 0.4f, 0.01f, 0f, 4, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "AttackedShadow", "sway", "MagicFieldGreen", 0f, "fen3", 0);
            config[2030105] = new SkillConfig(2030105, "分兵小", "纷", "被攻击时产生一只无伤害部队", "", 5, 0.4f, 15f, 0f, 0f, "atk", null, 15f, false, "", 0, 0f, 0, 0.4f, 0.01f, 0f, 4, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "AttackedShadow", "sway", "MagicFieldGreen", 0f, "fen3", 0);
            config[2080011] = new SkillConfig(2080011, "百出", "百", "降低自己和同行[扇谋相]技能CD时间", "智技up", 1, 0f, 0f, 0f, 0f, "ap", new string[]{"ap"}, 0f, false, "", 0, 0.3f, 0, 0f, 0f, 0f, 0, 0f, 1, "白", "扇谋相", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "ModifySkillRateTime", "", "", 0f, "bai", 0);
            config[2080012] = new SkillConfig(2080012, "百出", "百", "降低自己和同行[扇谋相]技能CD时间", "智技up", 2, 0f, 0f, 0f, 0f, "ap", new string[]{"ap"}, 0f, false, "", 0, 0.3f, 0, 0f, 0f, 0f, 0, 0f, 1, "白", "扇谋相", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "ModifySkillRateTime", "", "", 0f, "bai", 0);
            config[2080013] = new SkillConfig(2080013, "百出", "百", "降低自己和同行[扇谋相]技能CD时间", "智技up", 3, 0f, 0f, 0f, 0f, "ap", new string[]{"ap"}, 0f, false, "", 0, 0.3f, 0, 0f, 0f, 0f, 0, 0f, 1, "白", "扇谋相", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "ModifySkillRateTime", "", "", 0f, "bai", 0);
            config[2080014] = new SkillConfig(2080014, "百出", "百", "降低自己和同行[扇谋相]技能CD时间", "智技up", 4, 0f, 0f, 0f, 0f, "ap", new string[]{"ap"}, 0f, false, "", 0, 0.3f, 0, 0f, 0f, 0f, 0, 0f, 1, "白", "扇谋相", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "ModifySkillRateTime", "", "", 0f, "bai", 0);
            config[2080015] = new SkillConfig(2080015, "百出", "百", "降低自己和同行[扇谋相]技能CD时间", "智技up", 5, 0f, 0f, 0f, 0f, "ap", new string[]{"ap"}, 0f, false, "", 0, 0.3f, 0, 0f, 0f, 0f, 0, 0f, 1, "白", "扇谋相", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "ModifySkillRateTime", "", "", 0f, "bai", 0);
            config[2080021] = new SkillConfig(2080021, "百出小", "白", "降低技能CD时间", "智技up", 1, 0f, 0f, 0f, 0f, "ap", new string[]{"ap"}, 0f, false, "", 0, 0.4f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "ModifySkillRateTime", "", "", 0f, "bai2", 0);
            config[2080022] = new SkillConfig(2080022, "百出小", "白", "降低技能CD时间", "智技up", 2, 0f, 0f, 0f, 0f, "ap", new string[]{"ap"}, 0f, false, "", 0, 0.4f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "ModifySkillRateTime", "", "", 0f, "bai2", 0);
            config[2080023] = new SkillConfig(2080023, "百出小", "白", "降低技能CD时间", "智技up", 3, 0f, 0f, 0f, 0f, "ap", new string[]{"ap"}, 0f, false, "", 0, 0.4f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "ModifySkillRateTime", "", "", 0f, "bai2", 0);
            config[2080024] = new SkillConfig(2080024, "百出小", "白", "降低技能CD时间", "智技up", 4, 0f, 0f, 0f, 0f, "ap", new string[]{"ap"}, 0f, false, "", 0, 0.4f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "ModifySkillRateTime", "", "", 0f, "bai2", 0);
            config[2080025] = new SkillConfig(2080025, "百出小", "白", "降低技能CD时间", "智技up", 5, 0f, 0f, 0f, 0f, "ap", new string[]{"ap"}, 0f, false, "", 0, 0.4f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "ModifySkillRateTime", "", "", 0f, "bai2", 0);
            config[2080031] = new SkillConfig(2080031, "神算", "神", "提升技能命中率和持续时间", "智技up", 1, 0.3f, 0f, 0f, 0f, "ap", new string[]{"ap"}, 0f, false, "", 0, 0.5f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0.5f, "", 0, 0f, 0f, 0f, 0f, "ModifySkillRateTime", "", "", 0f, "shen", 0);
            config[2080032] = new SkillConfig(2080032, "神算", "神", "提升技能命中率和持续时间", "智技up", 2, 0.3f, 0f, 0f, 0f, "ap", new string[]{"ap"}, 0f, false, "", 0, 0.5f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0.5f, "", 0, 0f, 0f, 0f, 0f, "ModifySkillRateTime", "", "", 0f, "shen", 0);
            config[2080033] = new SkillConfig(2080033, "神算", "神", "提升技能命中率和持续时间", "智技up", 3, 0.3f, 0f, 0f, 0f, "ap", new string[]{"ap"}, 0f, false, "", 0, 0.5f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0.5f, "", 0, 0f, 0f, 0f, 0f, "ModifySkillRateTime", "", "", 0f, "shen", 0);
            config[2080034] = new SkillConfig(2080034, "神算", "神", "提升技能命中率和持续时间", "智技up", 4, 0.3f, 0f, 0f, 0f, "ap", new string[]{"ap"}, 0f, false, "", 0, 0.5f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0.5f, "", 0, 0f, 0f, 0f, 0f, "ModifySkillRateTime", "", "", 0f, "shen", 0);
            config[2080035] = new SkillConfig(2080035, "神算", "神", "提升技能命中率和持续时间", "智技up", 5, 0.3f, 0f, 0f, 0f, "ap", new string[]{"ap"}, 0f, false, "", 0, 0.5f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0.5f, "", 0, 0f, 0f, 0f, 0f, "ModifySkillRateTime", "", "", 0f, "shen", 0);
            config[2080041] = new SkillConfig(2080041, "蔓延", "延", "自己和同行[扇谋相]技能负面状态扩散", "智技up", 1, 0.5f, 3f, 0f, 0f, "ap", new string[]{"ap"}, 30f, false, "", 2, 0f, 0, 0f, 0f, 0f, 0, 0f, 1, "筵", "扇谋相", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "BuffExpand", "", "", 0f, "yan2", 0);
            config[2080042] = new SkillConfig(2080042, "蔓延", "延", "自己和同行[扇谋相]技能负面状态扩散", "智技up", 2, 0.5f, 3f, 0f, 0f, "ap", new string[]{"ap"}, 30f, false, "", 2, 0f, 0, 0f, 0f, 0f, 0, 0f, 1, "筵", "扇谋相", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "BuffExpand", "", "", 0f, "yan2", 0);
            config[2080043] = new SkillConfig(2080043, "蔓延", "延", "自己和同行[扇谋相]技能负面状态扩散", "智技up", 3, 0.5f, 3f, 0f, 0f, "ap", new string[]{"ap"}, 30f, false, "", 2, 0f, 0, 0f, 0f, 0f, 0, 0f, 1, "筵", "扇谋相", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "BuffExpand", "", "", 0f, "yan2", 0);
            config[2080044] = new SkillConfig(2080044, "蔓延", "延", "自己和同行[扇谋相]技能负面状态扩散", "智技up", 4, 0.5f, 3f, 0f, 0f, "ap", new string[]{"ap"}, 30f, false, "", 2, 0f, 0, 0f, 0f, 0f, 0, 0f, 1, "筵", "扇谋相", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "BuffExpand", "", "", 0f, "yan2", 0);
            config[2080045] = new SkillConfig(2080045, "蔓延", "延", "自己和同行[扇谋相]技能负面状态扩散", "智技up", 5, 0.5f, 3f, 0f, 0f, "ap", new string[]{"ap"}, 30f, false, "", 2, 0f, 0, 0f, 0f, 0f, 0, 0f, 1, "筵", "扇谋相", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "BuffExpand", "", "", 0f, "yan2", 0);
            config[2080051] = new SkillConfig(2080051, "蔓延小", "筵", "技能负面状态概率扩散", "智技up", 1, 0.5f, 3f, 0f, 0f, "ap", new string[]{"ap"}, 30f, false, "", 2, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "BuffExpand", "", "", 0f, "yan3", 0);
            config[2080052] = new SkillConfig(2080052, "蔓延小", "筵", "技能负面状态概率扩散", "智技up", 2, 0.5f, 3f, 0f, 0f, "ap", new string[]{"ap"}, 30f, false, "", 2, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "BuffExpand", "", "", 0f, "yan3", 0);
            config[2080053] = new SkillConfig(2080053, "蔓延小", "筵", "技能负面状态概率扩散", "智技up", 3, 0.5f, 3f, 0f, 0f, "ap", new string[]{"ap"}, 30f, false, "", 2, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "BuffExpand", "", "", 0f, "yan3", 0);
            config[2080054] = new SkillConfig(2080054, "蔓延小", "筵", "技能负面状态概率扩散", "智技up", 4, 0.5f, 3f, 0f, 0f, "ap", new string[]{"ap"}, 30f, false, "", 2, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "BuffExpand", "", "", 0f, "yan3", 0);
            config[2080055] = new SkillConfig(2080055, "蔓延小", "筵", "技能负面状态概率扩散", "智技up", 5, 0.5f, 3f, 0f, 0f, "ap", new string[]{"ap"}, 30f, false, "", 2, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "BuffExpand", "", "", 0f, "yan3", 0);
            config[2080061] = new SkillConfig(2080061, "同调", "调", "自己和同行[鼓乐医]技能正面状态扩散", "智技up", 1, 0.5f, 3f, 0f, 0f, "ap", new string[]{"ap"}, 30f, false, "", 2, 0f, 0, 0f, 0f, 0f, 0, 0f, 1, "碉", "鼓乐医", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "BuffExpandPos", "", "", 0f, "diao", 0);
            config[2080062] = new SkillConfig(2080062, "同调", "调", "自己和同行[鼓乐医]技能正面状态扩散", "智技up", 2, 0.5f, 3f, 0f, 0f, "ap", new string[]{"ap"}, 30f, false, "", 2, 0f, 0, 0f, 0f, 0f, 0, 0f, 1, "碉", "鼓乐医", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "BuffExpandPos", "", "", 0f, "diao", 0);
            config[2080063] = new SkillConfig(2080063, "同调", "调", "自己和同行[鼓乐医]技能正面状态扩散", "智技up", 3, 0.5f, 3f, 0f, 0f, "ap", new string[]{"ap"}, 30f, false, "", 2, 0f, 0, 0f, 0f, 0f, 0, 0f, 1, "碉", "鼓乐医", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "BuffExpandPos", "", "", 0f, "diao", 0);
            config[2080064] = new SkillConfig(2080064, "同调", "调", "自己和同行[鼓乐医]技能正面状态扩散", "智技up", 4, 0.5f, 3f, 0f, 0f, "ap", new string[]{"ap"}, 30f, false, "", 2, 0f, 0, 0f, 0f, 0f, 0, 0f, 1, "碉", "鼓乐医", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "BuffExpandPos", "", "", 0f, "diao", 0);
            config[2080065] = new SkillConfig(2080065, "同调", "调", "自己和同行[鼓乐医]技能正面状态扩散", "智技up", 5, 0.5f, 3f, 0f, 0f, "ap", new string[]{"ap"}, 30f, false, "", 2, 0f, 0, 0f, 0f, 0f, 0, 0f, 1, "碉", "鼓乐医", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "BuffExpandPos", "", "", 0f, "diao", 0);
            config[2080071] = new SkillConfig(2080071, "同调小", "碉", "技能正面状态扩散", "智技up", 1, 0.5f, 3f, 0f, 0f, "ap", new string[]{"ap"}, 30f, false, "", 2, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "BuffExpandPos", "", "", 0f, "diao2", 0);
            config[2080072] = new SkillConfig(2080072, "同调小", "碉", "技能正面状态扩散", "智技up", 2, 0.5f, 3f, 0f, 0f, "ap", new string[]{"ap"}, 30f, false, "", 2, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "BuffExpandPos", "", "", 0f, "diao2", 0);
            config[2080073] = new SkillConfig(2080073, "同调小", "碉", "技能正面状态扩散", "智技up", 3, 0.5f, 3f, 0f, 0f, "ap", new string[]{"ap"}, 30f, false, "", 2, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "BuffExpandPos", "", "", 0f, "diao2", 0);
            config[2080074] = new SkillConfig(2080074, "同调小", "碉", "技能正面状态扩散", "智技up", 4, 0.5f, 3f, 0f, 0f, "ap", new string[]{"ap"}, 30f, false, "", 2, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "BuffExpandPos", "", "", 0f, "diao2", 0);
            config[2080075] = new SkillConfig(2080075, "同调小", "碉", "技能正面状态扩散", "智技up", 5, 0.5f, 3f, 0f, 0f, "ap", new string[]{"ap"}, 30f, false, "", 2, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "BuffExpandPos", "", "", 0f, "diao2", 0);
            config[2080081] = new SkillConfig(2080081, "炽热", "炽", "提升本方火焰持续时间", "智技up", 1, 0f, 0f, 0f, 0f, "ap", null, 0f, false, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 3, "炽", "", 0, false, 0f, "火", 0, 0f, 3f, 0f, 0f, "ModifySummonTime", "", "", 0f, "chi", 0);
            config[2080082] = new SkillConfig(2080082, "炽热", "炽", "提升本方火焰持续时间", "智技up", 2, 0f, 0f, 0f, 0f, "ap", null, 0f, false, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 3, "炽", "", 0, false, 0f, "火", 0, 0f, 3f, 0f, 0f, "ModifySummonTime", "", "", 0f, "chi", 0);
            config[2080083] = new SkillConfig(2080083, "炽热", "炽", "提升本方火焰持续时间", "智技up", 3, 0f, 0f, 0f, 0f, "ap", null, 0f, false, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 3, "炽", "", 0, false, 0f, "火", 0, 0f, 3f, 0f, 0f, "ModifySummonTime", "", "", 0f, "chi", 0);
            config[2080084] = new SkillConfig(2080084, "炽热", "炽", "提升本方火焰持续时间", "智技up", 4, 0f, 0f, 0f, 0f, "ap", null, 0f, false, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 3, "炽", "", 0, false, 0f, "火", 0, 0f, 3f, 0f, 0f, "ModifySummonTime", "", "", 0f, "chi", 0);
            config[2080085] = new SkillConfig(2080085, "炽热", "炽", "提升本方火焰持续时间", "智技up", 5, 0f, 0f, 0f, 0f, "ap", null, 0f, false, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 3, "炽", "", 0, false, 0f, "火", 0, 0f, 3f, 0f, 0f, "ModifySummonTime", "", "", 0f, "chi", 0);
            config[2080091] = new SkillConfig(2080091, "曲扬", "曲", "正面祝福状态时间增加50%", "智技up", 1, 0f, 0f, 0f, 0f, "ap", new string[]{"ap"}, 0f, false, "", 0, 0.5f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "ModifyBuffTime", "", "", 0f, "qu", 0);
            config[2080092] = new SkillConfig(2080092, "曲扬", "曲", "正面祝福状态时间增加50%", "智技up", 2, 0f, 0f, 0f, 0f, "ap", new string[]{"ap"}, 0f, false, "", 0, 0.5f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "ModifyBuffTime", "", "", 0f, "qu", 0);
            config[2080093] = new SkillConfig(2080093, "曲扬", "曲", "正面祝福状态时间增加50%", "智技up", 3, 0f, 0f, 0f, 0f, "ap", new string[]{"ap"}, 0f, false, "", 0, 0.5f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "ModifyBuffTime", "", "", 0f, "qu", 0);
            config[2080094] = new SkillConfig(2080094, "曲扬", "曲", "正面祝福状态时间增加50%", "智技up", 4, 0f, 0f, 0f, 0f, "ap", new string[]{"ap"}, 0f, false, "", 0, 0.5f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "ModifyBuffTime", "", "", 0f, "qu", 0);
            config[2080095] = new SkillConfig(2080095, "曲扬", "曲", "正面祝福状态时间增加50%", "智技up", 5, 0f, 0f, 0f, 0f, "ap", new string[]{"ap"}, 0f, false, "", 0, 0.5f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "ModifyBuffTime", "", "", 0f, "qu", 0);
            config[2090011] = new SkillConfig(2090011, "富甲", "商", "战斗开始时获得金币", "", 1, 0f, 0f, 0f, 0f, "ap", null, 0f, false, "", 0, 0f, 5, 0.05f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "InitGold", "", "MagicChargeYellow", 0f, "gold", 0);
            config[2090012] = new SkillConfig(2090012, "富甲", "商", "战斗开始时获得金币", "", 2, 0f, 0f, 0f, 0f, "ap", null, 0f, false, "", 0, 0f, 5, 0.05f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "InitGold", "", "MagicChargeYellow", 0f, "gold", 0);
            config[2090013] = new SkillConfig(2090013, "富甲", "商", "战斗开始时获得金币", "", 3, 0f, 0f, 0f, 0f, "ap", null, 0f, false, "", 0, 0f, 5, 0.05f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "InitGold", "", "MagicChargeYellow", 0f, "gold", 0);
            config[2090014] = new SkillConfig(2090014, "富甲", "商", "战斗开始时获得金币", "", 4, 0f, 0f, 0f, 0f, "ap", null, 0f, false, "", 0, 0f, 5, 0.05f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "InitGold", "", "MagicChargeYellow", 0f, "gold", 0);
            config[2090015] = new SkillConfig(2090015, "富甲", "商", "战斗开始时获得金币", "", 5, 0f, 0f, 0f, 0f, "ap", null, 0f, false, "", 0, 0f, 5, 0.05f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "InitGold", "", "MagicChargeYellow", 0f, "gold", 0);
            config[2090021] = new SkillConfig(2090021, "国士", "国", "增加一个远程士兵并提升射程", "", 1, 0f, 0f, 0f, 0f, "ap", null, 15f, false, "", 0, 0f, 0, 0.075f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "InitSoldierSummon", "", "MagicChargeGreen", 0f, "guo", 0);
            config[2090022] = new SkillConfig(2090022, "国士", "国", "增加一个远程士兵并提升射程", "", 2, 0f, 0f, 0f, 0f, "ap", null, 15f, false, "", 0, 0f, 0, 0.075f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "InitSoldierSummon", "", "MagicChargeGreen", 0f, "guo", 0);
            config[2090023] = new SkillConfig(2090023, "国士", "国", "增加一个远程士兵并提升射程", "", 3, 0f, 0f, 0f, 0f, "ap", null, 15f, false, "", 0, 0f, 0, 0.075f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "InitSoldierSummon", "", "MagicChargeGreen", 0f, "guo", 0);
            config[2090024] = new SkillConfig(2090024, "国士", "国", "增加一个远程士兵并提升射程", "", 4, 0f, 0f, 0f, 0f, "ap", null, 15f, false, "", 0, 0f, 0, 0.075f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "InitSoldierSummon", "", "MagicChargeGreen", 0f, "guo", 0);
            config[2090025] = new SkillConfig(2090025, "国士", "国", "增加一个远程士兵并提升射程", "", 5, 0f, 0f, 0f, 0f, "ap", null, 15f, false, "", 0, 0f, 0, 0.075f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "InitSoldierSummon", "", "MagicChargeGreen", 0f, "guo", 0);
            config[2090031] = new SkillConfig(2090031, "仁德", "仁", "给与我方前排士兵12%生命值护盾", "", 1, 0f, 0f, 0f, 0f, "atk", null, 0f, false, "", 0, 0f, 0, 0.12f, 0f, 0f, 0, 0f, 0, "", "", 300001, false, 999f, "", 0, 0f, 0f, 0f, 0f, "InitSoldierShield", "", "", 0f, "ren", 0);
            config[2090032] = new SkillConfig(2090032, "仁德", "仁", "给与我方前排士兵12%生命值护盾", "", 2, 0f, 0f, 0f, 0f, "atk", null, 0f, false, "", 0, 0f, 0, 0.12f, 0f, 0f, 0, 0f, 0, "", "", 300001, false, 999f, "", 0, 0f, 0f, 0f, 0f, "InitSoldierShield", "", "", 0f, "ren", 0);
            config[2090033] = new SkillConfig(2090033, "仁德", "仁", "给与我方前排士兵12%生命值护盾", "", 3, 0f, 0f, 0f, 0f, "atk", null, 0f, false, "", 0, 0f, 0, 0.12f, 0f, 0f, 0, 0f, 0, "", "", 300001, false, 999f, "", 0, 0f, 0f, 0f, 0f, "InitSoldierShield", "", "", 0f, "ren", 0);
            config[2090034] = new SkillConfig(2090034, "仁德", "仁", "给与我方前排士兵12%生命值护盾", "", 4, 0f, 0f, 0f, 0f, "atk", null, 0f, false, "", 0, 0f, 0, 0.12f, 0f, 0f, 0, 0f, 0, "", "", 300001, false, 999f, "", 0, 0f, 0f, 0f, 0f, "InitSoldierShield", "", "", 0f, "ren", 0);
            config[2090035] = new SkillConfig(2090035, "仁德", "仁", "给与我方前排士兵12%生命值护盾", "", 5, 0f, 0f, 0f, 0f, "atk", null, 0f, false, "", 0, 0f, 0, 0.12f, 0f, 0f, 0, 0f, 0, "", "", 300001, false, 999f, "", 0, 0f, 0f, 0f, 0f, "InitSoldierShield", "", "", 0f, "ren", 0);
            config[2090041] = new SkillConfig(2090041, "激励", "励", "提升同列队友智力", "", 1, 0f, 0f, 0f, 0f, "ap", null, 0f, false, "", 0, 0f, 0, 0.7f, 0f, 0f, 0, 0f, 2, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "HelpInitTeach", "", "MagicChargeYellow", 0f, "li", 0);
            config[2090042] = new SkillConfig(2090042, "激励", "励", "提升同列队友智力", "", 2, 0f, 0f, 0f, 0f, "ap", null, 0f, false, "", 0, 0f, 0, 0.7f, 0f, 0f, 0, 0f, 2, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "HelpInitTeach", "", "MagicChargeYellow", 0f, "li", 0);
            config[2090043] = new SkillConfig(2090043, "激励", "励", "提升同列队友智力", "", 3, 0f, 0f, 0f, 0f, "ap", null, 0f, false, "", 0, 0f, 0, 0.7f, 0f, 0f, 0, 0f, 2, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "HelpInitTeach", "", "MagicChargeYellow", 0f, "li", 0);
            config[2090044] = new SkillConfig(2090044, "激励", "励", "提升同列队友智力", "", 4, 0f, 0f, 0f, 0f, "ap", null, 0f, false, "", 0, 0f, 0, 0.7f, 0f, 0f, 0, 0f, 2, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "HelpInitTeach", "", "MagicChargeYellow", 0f, "li", 0);
            config[2090045] = new SkillConfig(2090045, "激励", "励", "提升同列队友智力", "", 5, 0f, 0f, 0f, 0f, "ap", null, 0f, false, "", 0, 0f, 0, 0.7f, 0f, 0f, 0, 0f, 2, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "HelpInitTeach", "", "MagicChargeYellow", 0f, "li", 0);
            config[2090051] = new SkillConfig(2090051, "学习", "学", "攻击时几率提升自己的属性", "", 1, 0.3f, 0f, 0f, 0f, "ap", null, 0f, false, "", 0, 0f, 5, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "HitAttr", "", "MagicChargeYellow", 0f, "zhang", 20);
            config[2090052] = new SkillConfig(2090052, "学习", "学", "攻击时几率提升自己的属性", "", 2, 0.3f, 0f, 0f, 0f, "ap", null, 0f, false, "", 0, 0f, 5, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "HitAttr", "", "MagicChargeYellow", 0f, "zhang", 20);
            config[2090053] = new SkillConfig(2090053, "学习", "学", "攻击时几率提升自己的属性", "", 3, 0.3f, 0f, 0f, 0f, "ap", null, 0f, false, "", 0, 0f, 5, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "HitAttr", "", "MagicChargeYellow", 0f, "zhang", 20);
            config[2090054] = new SkillConfig(2090054, "学习", "学", "攻击时几率提升自己的属性", "", 4, 0.3f, 0f, 0f, 0f, "ap", null, 0f, false, "", 0, 0f, 5, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "HitAttr", "", "MagicChargeYellow", 0f, "zhang", 20);
            config[2090055] = new SkillConfig(2090055, "学习", "学", "攻击时几率提升自己的属性", "", 5, 0.3f, 0f, 0f, 0f, "ap", null, 0f, false, "", 0, 0f, 5, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "HitAttr", "", "MagicChargeYellow", 0f, "zhang", 20);
            config[2090061] = new SkillConfig(2090061, "制衡", "衡", "初始获得我方兵种数为3,4,5时,提升属性10,20,30", "", 1, 0f, 0f, 0f, 0f, "ap", null, 0f, false, "", 0, 0f, 5, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "InitAttrZhiheng", "", "MagicChargeYellow", 0f, "heng", 0);
            config[2090062] = new SkillConfig(2090062, "制衡", "衡", "初始获得我方兵种数为3,4,5时,提升属性10,20,30", "", 2, 0f, 0f, 0f, 0f, "ap", null, 0f, false, "", 0, 0f, 5, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "InitAttrZhiheng", "", "MagicChargeYellow", 0f, "heng", 0);
            config[2090063] = new SkillConfig(2090063, "制衡", "衡", "初始获得我方兵种数为3,4,5时,提升属性10,20,30", "", 3, 0f, 0f, 0f, 0f, "ap", null, 0f, false, "", 0, 0f, 5, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "InitAttrZhiheng", "", "MagicChargeYellow", 0f, "heng", 0);
            config[2090064] = new SkillConfig(2090064, "制衡", "衡", "初始获得我方兵种数为3,4,5时,提升属性10,20,30", "", 4, 0f, 0f, 0f, 0f, "ap", null, 0f, false, "", 0, 0f, 5, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "InitAttrZhiheng", "", "MagicChargeYellow", 0f, "heng", 0);
            config[2090065] = new SkillConfig(2090065, "制衡", "衡", "初始获得我方兵种数为3,4,5时,提升属性10,20,30", "", 5, 0f, 0f, 0f, 0f, "ap", null, 0f, false, "", 0, 0f, 5, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "InitAttrZhiheng", "", "MagicChargeYellow", 0f, "heng", 0);
            config[2090081] = new SkillConfig(2090081, "奋进", "奋", "提升同列队友武力", "", 1, 0f, 0f, 0f, 0f, "might", null, 0f, false, "", 0, 0f, 0, 0.7f, 0f, 0f, 0, 0f, 2, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "HelpInitTeach", "", "MagicChargePink", 0f, "fen", 0);
            config[2090082] = new SkillConfig(2090082, "奋进", "奋", "提升同列队友武力", "", 2, 0f, 0f, 0f, 0f, "might", null, 0f, false, "", 0, 0f, 0, 0.7f, 0f, 0f, 0, 0f, 2, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "HelpInitTeach", "", "MagicChargePink", 0f, "fen", 0);
            config[2090083] = new SkillConfig(2090083, "奋进", "奋", "提升同列队友武力", "", 3, 0f, 0f, 0f, 0f, "might", null, 0f, false, "", 0, 0f, 0, 0.7f, 0f, 0f, 0, 0f, 2, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "HelpInitTeach", "", "MagicChargePink", 0f, "fen", 0);
            config[2090084] = new SkillConfig(2090084, "奋进", "奋", "提升同列队友武力", "", 4, 0f, 0f, 0f, 0f, "might", null, 0f, false, "", 0, 0f, 0, 0.7f, 0f, 0f, 0, 0f, 2, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "HelpInitTeach", "", "MagicChargePink", 0f, "fen", 0);
            config[2090085] = new SkillConfig(2090085, "奋进", "奋", "提升同列队友武力", "", 5, 0f, 0f, 0f, 0f, "might", null, 0f, false, "", 0, 0f, 0, 0.7f, 0f, 0f, 0, 0f, 2, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "HelpInitTeach", "", "MagicChargePink", 0f, "fen", 0);

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

        public static SkillConfig GetConfig(string sname, int lv)
        {
            if (string.IsNullOrEmpty(sname))
                return null;
            SkillConfig fallback = null;
            foreach (var cfg in config.Values)
            {
                if (cfg.Sname != sname)
                    continue;
                if (cfg.Lv == lv)
                    return cfg;
                if (fallback == null || cfg.Lv == 5)
                    fallback = cfg;
            }
            return fallback;
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