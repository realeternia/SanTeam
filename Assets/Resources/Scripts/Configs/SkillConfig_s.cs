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


        public SkillConfig(int Id, string Name, string Sname, string Descript, string Type, int Lv, float Rate, float CD, float AttackPointReduce, float ConditionParm, string Attr, string[] CheckAttrs, float Range, bool RangeOut, string TargetType, int TargetCount, float Strength, int StrengthInt, float SkillAttrRate, float SkillDamageRate, float SkillDamageAttrRate, int DoCount, float TimeDelay, int UnitHelpType, string HelpSkill, string HelpSkillJob, int BuffId, bool NegBuff, float BuffTime, string SummonTag, int SummonCount, float SummonArea, float SummonTime, float SummonHitInterval, float SummonSpeed, string ScriptName, string Action, string HitEffect, float EffectSize, string Icon)
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
        };

        public static Dictionary<string, FieldMetaInfo> FieldMeta { get { return fieldMeta; } }

        private static List<CellMeta> cellMeta = new List<CellMeta>();
        public static List<CellMeta> CellMetas { get { return cellMeta; } }

        public static void Load()
        {
            config.Clear();
            config[200001] = new SkillConfig(200001, "王", "帅", "给与我方同阵营单位17%生命值护盾", "职业", 1, 0f, 0f, 0f, 0f, "", null, 0f, false, "", 0, 0f, 0, 0.17f, 0f, 0f, 0, 0f, 0, "", "", 300001, false, 999f, "", 0, 0f, 0f, 0f, 0f, "InitMasterShield", "", "", 0f, "shuai");
            config[200002] = new SkillConfig(200002, "羽扇", "扇", "击中目标时触发弹射", "职业", 1, 0f, 0f, 0f, 0f, "ap", null, 30f, false, "", 1, 0f, 0, 0f, 0.15f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "AttackReboundArrow", "sway", "", 0f, "shan");
            config[200003] = new SkillConfig(200003, "刀兵", "刀", "攻击几率造成额外伤害", "职业", 1, 0.15f, 5f, 0f, 0f, "atk", null, 0f, false, "", 0, 0f, 10, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "AttackAddDamage", "sway", "SwordHitRedCritical", 0f, "dao");
            config[200004] = new SkillConfig(200004, "坚韧", "士", "受击时几率发动减伤", "职业", 1, 0.35f, 7f, 0f, 0f, "might", null, 0f, false, "", 0, 0.5f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 300002, false, 4.5f, "", 0, 0f, 0f, 0f, 0f, "AttackedBuff", "spin", "", 0f, "shi");
            config[200005] = new SkillConfig(200005, "突破", "马", "移动时穿越敌人,造成额外伤害", "职业", 1, 1f, 7f, 0f, 0f, "atk", null, 0f, false, "", 0, 0f, 0, 0f, 0.5f, 0f, 0, 0f, 0, "", "", 301003, false, 0f, "", 0, 0f, 0f, 0f, 0f, "AttackRunCross", "", "LightningExplosionBlue", 0f, "ma");
            config[200006] = new SkillConfig(200006, "运筹", "相", "提升士兵等级", "职业", 1, 0f, 0f, 0f, 0f, "ap", null, 0f, false, "", 0, 0f, 0, 0.25f, 0.05f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "InitSoldierUp", "", "MagicChargeYellow", 0f, "xiang");
            config[200007] = new SkillConfig(200007, "弓手", "弓", "远程射击单位", "职业", 1, 1f, 99f, 0f, 0f, "atk", null, 0f, false, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "Dumb", "", "", 0f, "gong");
            config[200008] = new SkillConfig(200008, "谋略", "谋", "一定几率混乱目标单位2s", "职业", 1, 0.15f, 4f, 0f, 0f, "ap", null, 0f, false, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 301001, false, 2f, "", 0, 0f, 0f, 0f, 0f, "HitBuff", "throw", "MagicChargeYellow", 0f, "mou");
            config[200009] = new SkillConfig(200009, "炮车", "炮", "攻击目标发生爆炸", "职业", 1, 0.5f, 0f, 0f, 0f, "atk", null, 20f, false, "", 2, 0f, 0, 0f, 0.6f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "HitArea", "throw", "MagicNovaYellow", 0f, "pao");
            config[200010] = new SkillConfig(200010, "弩手", "弩", "射程非常远", "职业", 1, 1f, 99f, 0f, 0f, "atk", null, 0f, false, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "Dumb", "", "", 0f, "nu");
            config[200011] = new SkillConfig(200011, "冲锋", "车", "移动时穿越敌人,降低目标防御", "职业", 1, 1f, 7f, 0f, 0f, "atk", null, 0f, false, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 301003, false, 3f, "", 0, 0f, 0f, 0f, 0f, "AttackRunCrossPlus", "", "LightningExplosionRed", 0f, "che");
            config[200012] = new SkillConfig(200012, "声乐", "乐", "给与友军攻速祝福", "职业", 1, 1f, 3.5f, 1f, 0f, "ap", null, 50f, false, "", 0, 0.45f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 300005, false, 5f, "", 0, 0f, 0f, 0f, 0f, "HelpAidBuff", "sway", "MagicChargePink", 0f, "song");
            config[200013] = new SkillConfig(200013, "治疗", "医", "给与友军治疗", "职业", 1, 1f, 3f, 1f, 0f, "ap", null, 50f, false, "", 0, 0f, 0, 0.7f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "HelpAidHeal", "sway", "MagicBuffGreen", 0f, "heal");
            config[200014] = new SkillConfig(200014, "枪阵", "枪", "一定几率混乱目标单位2s", "职业", 1, 0.15f, 4f, 0f, 0f, "atk", null, 0f, false, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 301001, false, 2f, "", 0, 0f, 0f, 0f, 0f, "HitBuff", "spin", "MagicChargeYellow", 0f, "qiang");
            config[200015] = new SkillConfig(200015, "戟阵", "戟", "攻击目标时伤害周边敌人", "职业", 1, 0.35f, 4f, 0f, 0f, "atk", null, 25f, false, "", 2, 0f, 0, 0f, 0.6f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "HitAround", "throw", "SwordSlashMiniWhite", 0f, "ji");
            config[200016] = new SkillConfig(200016, "战鼓", "鼓", "给与友军攻击力祝福", "职业", 1, 1f, 3.5f, 1f, 0f, "ap", null, 50f, false, "", 0, 0.4f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 300004, false, 5f, "", 0, 0f, 0f, 0f, 0f, "HelpAidBuff", "sway", "MagicChargeYellow", 0f, "gu");
            config[201002] = new SkillConfig(201002, "铁骑", "铁", "自己和同行[马车]技能附带混乱效果", "攻击up", 1, 0f, 0f, 0f, 0f, "atk", null, 0f, false, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 1, "铁", "马车", 301001, false, 2f, "", 0, 0f, 0f, 0f, 0f, "BuffTieqi", "", "", 0f, "tie");
            config[201003] = new SkillConfig(201003, "奇袭", "奇", "自己和同行队友统帅技能造成的混乱时间增加50%", "攻击up", 1, 0f, 0f, 0f, 0f, "atk", new string[]{"atk"}, 0f, false, "", 0, 0.5f, 0, 0f, 0f, 0f, 0, 0f, 1, "奇", "", 301001, true, 0f, "", 0, 0f, 0f, 0f, 0f, "ModifyBuffTime", "", "", 0f, "qi");
            config[201004] = new SkillConfig(201004, "瓦解小", "透", "提升20%暴击率", "攻击up", 1, 0f, 0f, 0f, 0f, "might", null, 0f, false, "", 0, 0.2f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "InitAddCrit", "", "", 0f, "jie3");
            config[201005] = new SkillConfig(201005, "瓦解", "解", "自己和同行队友提升20%暴击率", "攻击up", 1, 0f, 0f, 0f, 0f, "might", null, 0f, false, "", 0, 0.2f, 0, 0f, 0f, 0f, 0, 0f, 1, "透", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "InitAddCrit", "", "", 0f, "jie");
            config[201006] = new SkillConfig(201006, "连击", "连", "攻击时几率触发连续攻击", "攻击up", 1, 0.3f, 5f, 0f, 0f, "atk", null, 0f, false, "", 0, 0.7f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "AttackSpeedAttack", "flipspin", "", 0f, "lian");
            config[201007] = new SkillConfig(201007, "速射", "速", "自己和同行[弓弩]箭矢飞行速度提升", "攻击up", 1, 0f, 0f, 0f, 0f, "atk", null, 0f, false, "", 0, 2.5f, 0, 0f, 0f, 0f, 0, 0f, 1, "速", "弓弩", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "ModifyShootSpeed", "", "", 0f, "su");
            config[201008] = new SkillConfig(201008, "箭雨", "雨", "攻击时30%发出2只箭", "", 1, 0.3f, 4f, 0f, 0f, "atk", null, 30f, false, "", 1, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "AttackMultiArrow", "flipspin", "", 0f, "duo");
            config[201009] = new SkillConfig(201009, "共杀", "共", "击中目标时触发2次弹射", "", 1, 0.35f, 5f, 0f, 0f, "ap", null, 30f, false, "", 3, 0f, 0, 0f, 0.25f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "AttackReboundArrow", "flipspin", "", 0f, "gong2");
            config[201010] = new SkillConfig(201010, "旋风斩", "旋", "攻击时几率对附近敌人造成伤害", "技", 1, 0.4f, 5f, 0f, 0f, "might", null, 25f, false, "", 5, 0f, 0, 0f, 0.8f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "AttackSpinAttack", "spin", "SwordWhirlwindWhite", 0f, "meng");
            config[201011] = new SkillConfig(201011, "落雷", "天", "攻击召唤出持续伤害的雷电阵", "术", 1, 0.3f, 10f, 0f, 0f, "ap", null, 0f, false, "", 1, 0f, 0, 0f, 0f, 0.15f, 0, 0f, 0, "", "", 0, false, 0f, "雷", 0, 25f, 5.2f, 0.5f, 0f, "HitRegion", "spin", "SummonStorm", 5f, "tian");
            config[201012] = new SkillConfig(201012, "火计", "火", "攻击时对目标放火", "术", 1, 0.3f, 5f, 0f, 0f, "ap", null, 0f, false, "", 1, 0f, 0, 0f, 0f, 0.1f, 0, 0f, 0, "", "", 0, false, 0f, "火", 1, 8f, 3.2f, 1f, 0f, "HitWall", "throw", "SoftFireBigRed", 1.6f, "huo");
            config[201013] = new SkillConfig(201013, "火墙", "炎", "攻击召唤出持续伤害的火墙", "术", 1, 0.3f, 8.5f, 0f, 0f, "ap", null, 0f, false, "", 1, 0f, 0, 0f, 0f, 0.08f, 0, 0f, 0, "", "", 0, false, 0f, "火", 5, 8f, 3.2f, 1f, 0f, "HitWall", "spin", "SoftFireBigRed", 1.6f, "yan");
            config[201014] = new SkillConfig(201014, "飞斧", "斧", "扔出飞斧攻击前方敌人", "技", 1, 1f, 6.7f, 2f, 0f, "might", null, 45f, false, "", 4, 0f, 0, 0f, 0f, 0.4f, 0, 0f, 0, "", "", 0, false, 0f, "武", 0, 9f, 1.5f, 0f, 40f, "AidShockWave", "spin", "AxeExplosion", 3f, "fu3");
            config[201015] = new SkillConfig(201015, "驰羽", "羽", "能够射出箭矢", "技", 1, 1f, 7f, 2f, 0f, "atk", null, 42f, false, "", 0, 0f, 0, 0f, 0f, 0.75f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "AidSuddenArrow", "sway", "BulletExplosionBlue", 3f, "yu");
            config[201016] = new SkillConfig(201016, "惊雷", "雷", "召唤3个惊雷攻击前方敌人", "术", 1, 1f, 6.7f, 2f, 0f, "ap", null, 60f, false, "", 4, 0f, 0, 0f, 0f, 0.25f, 0, 0f, 0, "", "", 0, false, 0f, "雷", 0, 11f, 1.5f, 0f, 40f, "AidShockWave", "spin", "NukeMissileFires", 4f, "lei");
            config[201017] = new SkillConfig(201017, "鬼谋", "鬼", "攻击造成生命值比例伤害", "", 1, 0.25f, 2f, 0f, 1f, "ap", null, 0f, false, "", 0, 0.2f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "DamageReal", "sway", "", 0f, "gui");
            config[201018] = new SkillConfig(201018, "斩", "斩", "直接杀死低生命值单位", "", 1, 0f, 7f, 0f, 0.3f, "might", null, 0f, false, "", 0, 1f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "DamageReal", "spin", "SwordHitRedCritical", 0f, "zhan");
            config[201019] = new SkillConfig(201019, "魔神", "魔", "攻击时回复生命", "", 1, 0.35f, 6f, 0f, 0f, "might", null, 0f, false, "", 0, 0f, 0, 0f, 1f, 0f, 0, 0f, 0, "", "", 300003, false, 5f, "", 0, 0f, 0f, 0f, 0f, "AttackedBuff", "", "MagicBuffGreen", 0f, "mo");
            config[201020] = new SkillConfig(201020, "埋伏", "伏", "被攻击时瞬移到远程攻击者附近", "", 1, 1f, 6f, 0f, 0f, "atk", null, 30f, false, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 301001, false, 1f, "", 0, 0f, 0f, 0f, 0f, "HitTeleport", "saw", "MagicNovaBlue", 0f, "fu");
            config[201021] = new SkillConfig(201021, "火矢", "矢", "攻击时射出火箭", "技", 1, 0.35f, 3f, 0f, 0f, "might", null, 0f, false, "", 1, 0f, 0, 0f, 0f, 0.12f, 0, 0f, 0, "", "", 0, false, 0f, "火", 1, 8f, 3.2f, 1f, 0f, "HitWall", "throw", "SoftFireBigRed", 1.6f, "shi3");
            config[201022] = new SkillConfig(201022, "虎卫队", "虎", "攻击时几率对目标进行三连击", "技", 1, 0.15f, 6f, 0f, 0f, "atk", null, 0f, false, "", 0, 0f, 0, 0f, 1f, 0f, 2, 0.3f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "HitRepeat", "jumpspin", "SwordHitRedCritical", 0f, "hu");
            config[201023] = new SkillConfig(201023, "青州兵", "青", "攻击时几率对目标进行2连击", "技", 1, 0.15f, 7f, 0f, 0f, "atk", null, 0f, false, "", 0, 0f, 0, 0f, 1f, 0f, 1, 0.4f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "HitRepeat", "jumpspin", "SwordHitGreenCritical", 0f, "qing");
            config[201024] = new SkillConfig(201024, "乱战", "乱", "攻击晕眩单位造成额外伤害", "", 1, 0f, 3f, 0f, 0f, "atk", null, 0f, false, "", 0, 0f, 0, 0f, 0.7f, 0f, 0, 0f, 0, "", "", 301001, false, 0f, "", 0, 0f, 0f, 0f, 0f, "AttackAddDamage", "saw", "SwordHitRedCritical", 0f, "luan");
            config[201025] = new SkillConfig(201025, "虐袭", "虐", "攻击护盾敌人时造成额外伤害", "技", 1, 0f, 0f, 0f, 0f, "might", null, 0f, false, "", 0, 0.5f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 300001, false, 0f, "", 0, 0f, 0f, 0f, 0f, "AttackAntiShield", "sway", "", 0f, "nue");
            config[202001] = new SkillConfig(202001, "刺甲", "刺", "反弹50%近战伤害", "防御up", 1, 0.3f, 0f, 0f, 0f, "might", new string[]{"might，atk"}, 20f, false, "", 0, 0.5f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "DefFeedback", "", "SwordHitBlue", 0f, "ci");
            config[202002] = new SkillConfig(202002, "藤甲", "藤", "受非智力攻击时高几率发动70%减伤", "", 1, 0.5f, 0.5f, 0f, 0f, "might", new string[]{"might，atk"}, 0f, false, "", 0, 0.7f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "DefPlantSkin", "", "", 0f, "teng");
            config[202003] = new SkillConfig(202003, "明镜", "镜", "自己和同行队友反弹智力伤害", "防御up", 1, 0.3f, 0f, 0f, 0f, "atk", new string[]{"ap"}, 0f, false, "", 0, 0.5f, 0, 0f, 0f, 0f, 0, 0f, 1, "竟", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "DefFeedback", "", "SwordHitBlue", 0f, "jing");
            config[202004] = new SkillConfig(202004, "明镜小", "竟", "反弹30%智力伤害", "防御up", 1, 0.3f, 0f, 0f, 0f, "atk", new string[]{"ap"}, 0f, false, "", 0, 0.3f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "DefFeedback", "", "SwordHitBlue", 0f, "jing2");
            config[202005] = new SkillConfig(202005, "护卫", "护", "给与友军护盾祝福", "", 1, 1f, 8f, 1.5f, 0f, "might", null, 50f, false, "", 0, 0.18f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 300001, false, 10f, "", 0, 0f, 0f, 0f, 0f, "HelpAidBuff", "sway", "MagicChargeYellow", 0f, "hu1");
            config[202006] = new SkillConfig(202006, "坚毅", "坚", "生命值低时降低50%伤害", "防御up", 1, 0.3f, 0.5f, 0f, 0.35f, "might", null, 0f, false, "", 0, 0.5f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "DefHpLow", "", "", 0f, "jian");
            config[202007] = new SkillConfig(202007, "识破", "识", "同行降低智力类技能伤害", "防御up", 1, 0.3f, 0.5f, 0f, 0f, "atk", new string[]{"ap"}, 0f, false, "", 0, 0.5f, 0, 0f, 0f, 0f, 0, 0f, 1, "实", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "DefSkillDamageReduce", "", "", 0f, "shi5");
            config[202008] = new SkillConfig(202008, "识破小", "实", "降低智力类技能伤害", "防御up", 1, 0.3f, 0.5f, 0f, 0f, "atk", new string[]{"ap"}, 0f, false, "", 0, 0.5f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "DefSkillDamageReduce", "", "", 0f, "shi4");
            config[202009] = new SkillConfig(202009, "冷静", "静", "降低同行智力类技能造成的负面状态时间", "防御up", 1, 0f, 0f, 0f, 0f, "atk", new string[]{"ap"}, 0f, false, "", 0, 0.5f, 0, 0f, 0f, 0f, 0, 0f, 1, "境", "", 0, true, 0f, "", 0, 0f, 0f, 0f, 0f, "ModifyBeBuffTime", "", "", 0f, "jing3");
            config[202010] = new SkillConfig(202010, "冷静小", "境", "降低智力类技能造成的负面状态时间", "防御up", 1, 0f, 0f, 0f, 0f, "atk", new string[]{"ap"}, 0f, false, "", 0, 0.5f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, true, 0f, "", 0, 0f, 0f, 0f, 0f, "ModifyBeBuffTime", "", "", 0f, "jing4");
            config[202011] = new SkillConfig(202011, "敏锐", "敏", "提升15%闪避率", "防御up", 1, 0f, 0f, 0f, 0f, "might", null, 0f, false, "", 0, 0.2f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "InitAddDodge", "", "", 0f, "min");
            config[202012] = new SkillConfig(202012, "空城", "空", "自己和同列队友提升15%闪避率", "防御up", 1, 0f, 0f, 0f, 0f, "ap", null, 0f, false, "", 0, 0.2f, 0, 0f, 0f, 0f, 0, 0f, 2, "敏", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "InitAddDodge", "", "", 0f, "kong");
            config[202013] = new SkillConfig(202013, "复原", "复", "提升5点生命回复", "防御up", 1, 0f, 0f, 0f, 0f, "atk", null, 0f, false, "", 0, 0f, 5, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "InitAddRege", "", "", 0f, "fu2");
            config[202014] = new SkillConfig(202014, "药仙", "药", "自己和所有队友提升5点生命回复", "防御up", 1, 0f, 0f, 0f, 0f, "ap", null, 0f, false, "", 0, 0f, 5, 0f, 0f, 0f, 0, 0f, 3, "复", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "InitAddRege", "", "", 0f, "yao");
            config[203002] = new SkillConfig(203002, "连锁", "锁", "锁定敌人目标,传递一半受到伤害", "术", 1, 0.5f, 3f, 0f, 0f, "ap", null, 25f, false, "targetUnit", 1, 0f, 0, 0f, 0.5f, 0f, 0, 0f, 0, "", "", 301002, false, 8f, "", 0, 0f, 0f, 0f, 0f, "HitBuffArea", "throw", "MagicNovaYellow", 0f, "suo");
            config[203003] = new SkillConfig(203003, "劫粮", "劫", "攻击几率获取对方粮食", "", 1, 0.2f, 4f, 0f, 0f, "atk", null, 0f, false, "", 0, 0f, 2, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "HitFood", "sway", "", 0f, "jie2");
            config[203004] = new SkillConfig(203004, "威震", "威", "攻击时混乱周围目标", "", 1, 0.2f, 5f, 0f, 0f, "might", null, 20f, false, "castUnit", 3, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 301001, false, 1.5f, "", 0, 0f, 0f, 0f, 0f, "HitBuffArea", "spin", "MagicNovaYellow", 0f, "wei");
            config[203005] = new SkillConfig(203005, "击破", "破", "攻击几率使目标增伤40%", "", 1, 0.4f, 2f, 0f, 0f, "might", null, 0f, false, "", 0, 0.4f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 301003, false, 3f, "", 0, 0f, 0f, 0f, 0f, "HitBuff", "jump", "SoftFireBigRed", 0f, "po");
            config[203006] = new SkillConfig(203006, "延缓", "缓", "攻击几率使目标减速30%", "", 1, 0.4f, 3f, 0f, 0f, "ap", null, 0f, false, "", 0, 0.3f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 301004, false, 5f, "", 0, 0f, 0f, 0f, 0f, "HitBuff", "sway", "MagicNovaYellow", 0f, "huan");
            config[203007] = new SkillConfig(203007, "陷阵", "陷", "攻击几率使目标陷阵", "", 1, 0.4f, 3f, 0f, 0f, "ap", null, 0f, false, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 301005, false, 4f, "", 0, 0f, 0f, 0f, 0f, "HitBuff", "sway", "MagicNovaYellow", 0f, "xian");
            config[203008] = new SkillConfig(203008, "溃散", "溃", "攻击几率使目标溃败", "", 1, 0.4f, 4f, 0f, 0f, "ap", null, 0f, false, "", 0, 0f, 0, 0f, 0f, 0.1f, 0, 0f, 0, "", "", 301006, false, 5.2f, "", 0, 0f, 0f, 0f, 0f, "HitBuff", "sway", "MagicNovaYellow", 0f, "kui");
            config[203009] = new SkillConfig(203009, "分兵", "分", "被攻击时产生一只有伤害部队", "", 1, 0.4f, 15f, 0f, 0f, "atk", null, 15f, false, "", 0, 0f, 0, 0.5f, 0.5f, 0f, 4, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "AttackedShadow", "sway", "MagicFieldGreen", 0f, "fen2");
            config[203010] = new SkillConfig(203010, "分兵小", "纷", "被攻击时产生一只无伤害部队", "", 1, 0.4f, 15f, 0f, 0f, "atk", null, 15f, false, "", 0, 0f, 0, 0.4f, 0.01f, 0f, 4, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "AttackedShadow", "sway", "MagicFieldGreen", 0f, "fen3");
            config[208001] = new SkillConfig(208001, "百出", "百", "降低自己和同行[扇谋相]技能CD时间", "智技up", 1, 0f, 0f, 0f, 0f, "ap", new string[]{"ap"}, 0f, false, "", 0, 0.3f, 0, 0f, 0f, 0f, 0, 0f, 1, "白", "扇谋相", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "ModifySkillRateTime", "", "", 0f, "bai");
            config[208002] = new SkillConfig(208002, "百出小", "白", "降低技能CD时间", "智技up", 1, 0f, 0f, 0f, 0f, "ap", new string[]{"ap"}, 0f, false, "", 0, 0.4f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "ModifySkillRateTime", "", "", 0f, "bai2");
            config[208003] = new SkillConfig(208003, "神算", "神", "提升技能命中率和持续时间", "智技up", 1, 0.3f, 0f, 0f, 0f, "ap", new string[]{"ap"}, 0f, false, "", 0, 0.5f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0.5f, "", 0, 0f, 0f, 0f, 0f, "ModifySkillRateTime", "", "", 0f, "shen");
            config[208004] = new SkillConfig(208004, "蔓延", "延", "自己和同行[扇谋相]技能负面状态扩散", "智技up", 1, 0.5f, 3f, 0f, 0f, "ap", new string[]{"ap"}, 30f, false, "", 2, 0f, 0, 0f, 0f, 0f, 0, 0f, 1, "筵", "扇谋相", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "BuffExpand", "", "", 0f, "yan2");
            config[208005] = new SkillConfig(208005, "蔓延小", "筵", "技能负面状态概率扩散", "智技up", 1, 0.5f, 3f, 0f, 0f, "ap", new string[]{"ap"}, 30f, false, "", 2, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "BuffExpand", "", "", 0f, "yan3");
            config[208006] = new SkillConfig(208006, "同调", "调", "自己和同行[鼓乐医]技能正面状态扩散", "智技up", 1, 0.5f, 3f, 0f, 0f, "ap", new string[]{"ap"}, 30f, false, "", 2, 0f, 0, 0f, 0f, 0f, 0, 0f, 1, "碉", "鼓乐医", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "BuffExpandPos", "", "", 0f, "diao");
            config[208007] = new SkillConfig(208007, "同调小", "碉", "技能正面状态扩散", "智技up", 1, 0.5f, 3f, 0f, 0f, "ap", new string[]{"ap"}, 30f, false, "", 2, 0f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "BuffExpandPos", "", "", 0f, "diao2");
            config[208008] = new SkillConfig(208008, "炽热", "炽", "提升本方火焰持续时间", "智技up", 1, 0f, 0f, 0f, 0f, "ap", null, 0f, false, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0f, 3, "炽", "", 0, false, 0f, "火", 0, 0f, 3f, 0f, 0f, "ModifySummonTime", "", "", 0f, "chi");
            config[208009] = new SkillConfig(208009, "曲扬", "曲", "正面祝福状态时间增加50%", "智技up", 1, 0f, 0f, 0f, 0f, "ap", new string[]{"ap"}, 0f, false, "", 0, 0.5f, 0, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "ModifyBuffTime", "", "", 0f, "qu");
            config[209001] = new SkillConfig(209001, "富甲", "商", "战斗开始时获得金币", "", 1, 0f, 0f, 0f, 0f, "ap", null, 0f, false, "", 0, 0f, 5, 0.05f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "InitGold", "", "MagicChargeYellow", 0f, "gold");
            config[209002] = new SkillConfig(209002, "国士", "国", "增加一个远程士兵并提升射程", "", 1, 0f, 0f, 0f, 0f, "ap", null, 15f, false, "", 0, 0f, 0, 0.075f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "InitSoldierSummon", "", "MagicChargeGreen", 0f, "guo");
            config[209003] = new SkillConfig(209003, "仁德", "仁", "给与我方前排士兵12%生命值护盾", "", 1, 0f, 0f, 0f, 0f, "atk", null, 0f, false, "", 0, 0f, 0, 0.12f, 0f, 0f, 0, 0f, 0, "", "", 300001, false, 999f, "", 0, 0f, 0f, 0f, 0f, "InitSoldierShield", "", "", 0f, "ren");
            config[209004] = new SkillConfig(209004, "激励", "励", "提升同列队友智力", "", 1, 0f, 0f, 0f, 0f, "ap", null, 0f, false, "", 0, 0f, 0, 0.7f, 0f, 0f, 0, 0f, 2, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "HelpInitTeach", "", "MagicChargeYellow", 0f, "li");
            config[209005] = new SkillConfig(209005, "学习", "学", "攻击时几率提升自己的属性", "", 1, 0.3f, 0f, 0f, 0f, "ap", null, 0f, false, "", 0, 0f, 5, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "HitAttr", "", "MagicChargeYellow", 0f, "zhang");
            config[209006] = new SkillConfig(209006, "制衡", "衡", "初始获得我方兵种数为3,4,5时,提升属性10,20,30", "", 1, 0f, 0f, 0f, 0f, "ap", null, 0f, false, "", 0, 0f, 5, 0f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "InitAttrZhiheng", "", "MagicChargeYellow", 0f, "heng");
            config[209007] = new SkillConfig(209007, "米道", "米", "战斗开始时获得粮食", "", 1, 0f, 0f, 0f, 0f, "ap", null, 0f, false, "", 0, 0f, 10, 0.1f, 0f, 0f, 0, 0f, 0, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "InitFood", "", "MagicChargeYellow", 0f, "food");
            config[209008] = new SkillConfig(209008, "奋进", "奋", "提升同列队友武力", "", 1, 0f, 0f, 0f, 0f, "might", null, 0f, false, "", 0, 0f, 0, 0.7f, 0f, 0f, 0, 0f, 2, "", "", 0, false, 0f, "", 0, 0f, 0f, 0f, 0f, "HelpInitTeach", "", "MagicChargePink", 0f, "fen");

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