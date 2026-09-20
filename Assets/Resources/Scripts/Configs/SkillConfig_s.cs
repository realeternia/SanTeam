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
            {"Sname", new FieldMetaInfo("缩写", "string", 78)},
            {"Descript", new FieldMetaInfo("说明", "string", 402)},
            {"DescriptVal", new FieldMetaInfo("说明参数(按/1 /2替换模板,多个用;分隔)", "string", 153)},
            {"Type", new FieldMetaInfo("分类", "string", 0)},
            {"Comment", new FieldMetaInfo("备注", "string", 130)},
            {"Lv", new FieldMetaInfo("等级", "int", 60)},
            {"Rate", new FieldMetaInfo("发动概率", "float", 60)},
            {"CD", new FieldMetaInfo("发动cd", "float", 60)},
            {"MpCost", new FieldMetaInfo("技能MP消耗", "int", 60)},
            {"TriggerCondition", new FieldMetaInfo("触发条件", "string", 0)},
            {"AttackPointReduce", new FieldMetaInfo("攻击时间惩罚", "float", 75)},
            {"IsMagic", new FieldMetaInfo("是否法术(ap=法/否则攻)", "bool", 67)},
            {"HurtTag", new FieldMetaInfo("伤害标签(如AntiShield=绕过护盾打血)", "string", 0)},
            {"CheckType", new FieldMetaInfo("按类型限定(0不限/1仅物理/2仅法术)", "int", 90, "0:不限,1:仅物理,2:仅法术")},
            {"Range", new FieldMetaInfo("范围", "float", 60)},
            {"Area", new FieldMetaInfo("范围", "float", 60, "伤害范围等")},
            {"TargetType", new FieldMetaInfo("选取点", "string", 0)},
            {"TargetCount", new FieldMetaInfo("最大目标数", "int", 60)},
            {"Strength", new FieldMetaInfo("技能强度（恒定）", "float", 60)},
            {"StrengthInt", new FieldMetaInfo("技能强度（恒定）", "int", 60)},
            {"SkillDamageRate", new FieldMetaInfo("技能强度伤害/属性比例", "float", 65)},
            {"UnitHelpType", new FieldMetaInfo("效果范围(1横向，2纵向)", "int", 60)},
            {"HelpSkill", new FieldMetaInfo("光环技能", "string", 0)},
            {"HelpSkillJob", new FieldMetaInfo("职业限定", "string", 0)},
            {"BuffId", new FieldMetaInfo("BuffId", "string", 0)},
            {"NegBuff", new FieldMetaInfo("是否针对负面buff", "bool", 0)},
            {"BuffTime", new FieldMetaInfo("Buff持续", "float", 60)},
            {"SummonTag", new FieldMetaInfo("召唤物标签", "string", 0)},
            {"SummonCount", new FieldMetaInfo("技能场数", "int", 60)},
            {"SummonTime", new FieldMetaInfo("技能场持续", "float", 60)},
            {"SummonHitInterval", new FieldMetaInfo("技能场间隔", "float", 60)},
            {"SummonSpeed", new FieldMetaInfo("技能场速度", "float", 60)},
            {"ItemId", new FieldMetaInfo("获得道具Id", "int", 60)},
            {"ScriptName", new FieldMetaInfo("脚本名", "string", 124)},
            {"Action", new FieldMetaInfo("动作", "string", 0)},
            {"HitEffect", new FieldMetaInfo("hit", "string", 0)},
            {"EffectSize", new FieldMetaInfo("size", "float", 60)},
            {"Icon", new FieldMetaInfo("图标", "string", 0)},
            {"LinkSelf", new FieldMetaInfo("连接英雄加成", "string", 231)},
            {"LinkTeam", new FieldMetaInfo("我方其他英雄加成", "string", 155)},
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
        ///说明参数（配合 Descript 模板使用：Lv1 的 Descript 填模板，占位符/1 /2按顺序被 DescriptVal 替换；多个参数用;分隔；无占位符的模板或结构无法用模板表达的技能该列留空）
        /// </summary>
        public string DescriptVal;
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
        ///技能MP消耗（0=不使用MP，>0则仅靠法力回复(mpRegen)充能，满才能发动，发动后清空）
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
        ///是否法术伤害：true=法术（原ap，按法强成长，受魔抗减免）；false=物理（原atk/might，无双已并入，按攻击成长，受护甲减免）
        /// </summary>
        public bool IsMagic;
        /// <summary>
        ///伤害标签（如"AntiShield"=绕过护盾直接打血，破盾类技能用；空=无标签）
        /// </summary>
        public string HurtTag;
        /// <summary>
        ///按技能伤害类型限定：0=不限；1=仅物理；2=仅法术
        /// </summary>
        public int CheckType;
        /// <summary>
        ///范围
        /// </summary>
        public float Range;
        /// <summary>
        ///范围
        /// </summary>
        public float Area;
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
        ///技能强度伤害/属性比例（伤害倍率，或技能伤害公式 Strength+属性×比例 的系数，二选一使用）
        /// </summary>
        public float SkillDamageRate;
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
        ///Buff短名（BuffConfig.NameS，如"盾"；空=无buff），运行时经 BuffConfig.GetConfigByNameS 转为id
        /// </summary>
        public string BuffId;
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
        ///获得道具Id（战斗开始时按发动概率给所属玩家1个该道具；0=无）
        /// </summary>
        public int ItemId;
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


        public SkillConfig(int Id, string Name, string Sname, string Descript, string DescriptVal, string Type, string Comment, int Lv, float Rate, float CD, int MpCost, string TriggerCondition, float AttackPointReduce, bool IsMagic, string HurtTag, int CheckType, float Range, float Area, string TargetType, int TargetCount, float Strength, int StrengthInt, float SkillDamageRate, int UnitHelpType, string HelpSkill, string HelpSkillJob, string BuffId, bool NegBuff, float BuffTime, string SummonTag, int SummonCount, float SummonTime, float SummonHitInterval, float SummonSpeed, int ItemId, string ScriptName, string Action, string HitEffect, float EffectSize, string Icon, string LinkSelf, string LinkTeam, string AuroAttrs)
        {
            this.Id = Id;
            this.Name = Name;
            this.Sname = Sname;
            this.Descript = Descript;
            this.DescriptVal = DescriptVal;
            this.Type = Type;
            this.Comment = Comment;
            this.Lv = Lv;
            this.Rate = Rate;
            this.CD = CD;
            this.MpCost = MpCost;
            this.TriggerCondition = TriggerCondition;
            this.AttackPointReduce = AttackPointReduce;
            this.IsMagic = IsMagic;
            this.HurtTag = HurtTag;
            this.CheckType = CheckType;
            this.Range = Range;
            this.Area = Area;
            this.TargetType = TargetType;
            this.TargetCount = TargetCount;
            this.Strength = Strength;
            this.StrengthInt = StrengthInt;
            this.SkillDamageRate = SkillDamageRate;
            this.UnitHelpType = UnitHelpType;
            this.HelpSkill = HelpSkill;
            this.HelpSkillJob = HelpSkillJob;
            this.BuffId = BuffId;
            this.NegBuff = NegBuff;
            this.BuffTime = BuffTime;
            this.SummonTag = SummonTag;
            this.SummonCount = SummonCount;
            this.SummonTime = SummonTime;
            this.SummonHitInterval = SummonHitInterval;
            this.SummonSpeed = SummonSpeed;
            this.ItemId = ItemId;
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
            config[2000001] = new SkillConfig(2000001, "国家护盾", "国", "同阵营英雄获得最大生命/1的护盾", "+15%", "职业", "", 1, 0f, 0f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, 0f, 0, 0.15f, 0, "", "", "盾", false, 999f, "", 0, 0f, 0f, 0f, 0, "FactionShield", "", "", 0f, "shuai", "", "", "");
            config[2000002] = new SkillConfig(2000002, "国家护盾", "国", "", "+30%", "职业", "", 2, 0f, 0f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, 0f, 0, 0.30f, 0, "", "", "盾", false, 999f, "", 0, 0f, 0f, 0f, 0, "FactionShield", "", "", 0f, "shuai", "", "", "");
            config[2000003] = new SkillConfig(2000003, "国家护盾", "国", "", "+45%", "职业", "", 3, 0f, 0f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, 0f, 0, 0.45f, 0, "", "", "盾", false, 999f, "", 0, 0f, 0f, 0f, 0, "FactionShield", "", "", 0f, "shuai", "", "", "");
            config[2000004] = new SkillConfig(2000004, "国家护盾", "国", "", "+60%", "职业", "", 4, 0f, 0f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, 0f, 0, 0.60f, 0, "", "", "盾", false, 999f, "", 0, 0f, 0f, 0f, 0, "FactionShield", "", "", 0f, "shuai", "", "", "");
            config[2000005] = new SkillConfig(2000005, "国家护盾", "国", "", "+75%", "职业", "", 5, 0f, 0f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, 0f, 0, 0.75f, 0, "", "", "盾", false, 999f, "", 0, 0f, 0f, 0f, 0, "FactionShield", "", "", 0f, "shuai", "", "", "");
            config[2000006] = new SkillConfig(2000006, "连线", "友", "连线好友提升自身攻击/1", "+15", "连接", "", 1, 0f, 0f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, 0f, 0, 0f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "InitAttrChange", "", "", 0f, "you", "atk+15", "", "");
            config[2000007] = new SkillConfig(2000007, "连线", "友", "", "+30", "连接", "", 2, 0f, 0f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, 0f, 0, 0f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "InitAttrChange", "", "", 0f, "you", "atk+30", "", "");
            config[2000008] = new SkillConfig(2000008, "连线", "友", "", "+45", "连接", "", 3, 0f, 0f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, 0f, 0, 0f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "InitAttrChange", "", "", 0f, "you", "atk+45", "", "");
            config[2000009] = new SkillConfig(2000009, "连线", "友", "", "+60", "连接", "", 4, 0f, 0f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, 0f, 0, 0f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "InitAttrChange", "", "", 0f, "you", "atk+60", "", "");
            config[2000010] = new SkillConfig(2000010, "连线", "友", "", "+90", "连接", "", 5, 0f, 0f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, 0f, 0, 0f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "InitAttrChange", "", "", 0f, "you", "atk+90", "", "");
            config[2000011] = new SkillConfig(2000011, "诸侯", "王", "自身攻击/1，护甲/2，生命/3；阵营护盾额外/4", "+5;+5;+50;+10%", "职业", "", 1, 0f, 0f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, 0f, 0, 0f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "Dumb", "", "", 0f, "shuai", "atk+5,armor+5,hp+50", "", "");
            config[2000012] = new SkillConfig(2000012, "诸侯", "王", "", "+10;+10;+100;+20%", "职业", "", 2, 0f, 0f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, 0f, 0, 0f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "Dumb", "", "", 0f, "shuai", "atk+10,armor+10,hp+100", "", "");
            config[2000013] = new SkillConfig(2000013, "诸侯", "王", "", "+20;+20;+200;+30%", "职业", "", 3, 0f, 0f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, 0f, 0, 0f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "Dumb", "", "", 0f, "shuai", "atk+20,armor+20,hp+200", "", "");
            config[2000014] = new SkillConfig(2000014, "诸侯", "王", "", "+40;+40;+400;+40%", "职业", "", 4, 0f, 0f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, 0f, 0, 0f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "Dumb", "", "", 0f, "shuai", "atk+40,armor+40,hp+400", "", "");
            config[2000015] = new SkillConfig(2000015, "诸侯", "王", "", "+60;+60;+600;+50%", "职业", "", 5, 0f, 0f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, 0f, 0, 0f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "Dumb", "", "", 0f, "shuai", "atk+60,armor+60,hp+600", "", "");
            config[2000021] = new SkillConfig(2000021, "羽扇", "扇", "自身施加的负面buff持续/1，全队法力回复/2/秒", "+10%;+1", "职业", "", 1, 0f, 0f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, .1f, 0, 0f, 0, "", "", "", true, 0f, "", 0, 0f, 0f, 0f, 0, "ModifyBuffTime", "", "", 0f, "shan", "", "mpRegen+1", "");
            config[2000022] = new SkillConfig(2000022, "羽扇", "扇", "", "+20%;+2", "职业", "", 2, 0f, 0f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, .2f, 0, 0f, 0, "", "", "", true, 0f, "", 0, 0f, 0f, 0f, 0, "ModifyBuffTime", "", "", 0f, "shan", "", "mpRegen+2", "");
            config[2000023] = new SkillConfig(2000023, "羽扇", "扇", "", "+40%;+3", "职业", "", 3, 0f, 0f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, .4f, 0, 0f, 0, "", "", "", true, 0f, "", 0, 0f, 0f, 0f, 0, "ModifyBuffTime", "", "", 0f, "shan", "", "mpRegen+3", "");
            config[2000024] = new SkillConfig(2000024, "羽扇", "扇", "", "+60%;+4", "职业", "", 4, 0f, 0f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, .6f, 0, 0f, 0, "", "", "", true, 0f, "", 0, 0f, 0f, 0f, 0, "ModifyBuffTime", "", "", 0f, "shan", "", "mpRegen+4", "");
            config[2000025] = new SkillConfig(2000025, "羽扇", "扇", "", "+100%;+5", "职业", "", 5, 0f, 0f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, 1f, 0, 0f, 0, "", "", "", true, 0f, "", 0, 0f, 0f, 0f, 0, "ModifyBuffTime", "", "", 0f, "shan", "", "mpRegen+5", "");
            config[2000031] = new SkillConfig(2000031, "猛将", "锤", "自身生命低于50%时伤害/1", "+20%", "职业", "", 1, 0f, 0f, 0, "hprate<50", 0f, false, "", 0, 0f, 0f, "", 0, 0f, 0, 0.2f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AttackAddDamage", "", "", 0f, "dao", "", "", "");
            config[2000032] = new SkillConfig(2000032, "猛将", "锤", "", "+40%", "职业", "", 2, 0f, 0f, 0, "hprate<50", 0f, false, "", 0, 0f, 0f, "", 0, 0f, 0, 0.4f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AttackAddDamage", "", "", 0f, "dao", "", "", "");
            config[2000033] = new SkillConfig(2000033, "猛将", "锤", "", "+60%", "职业", "", 3, 0f, 0f, 0, "hprate<50", 0f, false, "", 0, 0f, 0f, "", 0, 0f, 0, 0.6f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AttackAddDamage", "", "", 0f, "dao", "", "", "");
            config[2000034] = new SkillConfig(2000034, "猛将", "锤", "", "+80%", "职业", "", 4, 0f, 0f, 0, "hprate<50", 0f, false, "", 0, 0f, 0f, "", 0, 0f, 0, 0.8f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AttackAddDamage", "", "", 0f, "dao", "", "", "");
            config[2000035] = new SkillConfig(2000035, "猛将", "锤", "", "+100%", "职业", "", 5, 0f, 0f, 0, "hprate<50", 0f, false, "", 0, 0f, 0f, "", 0, 0f, 0, 1f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AttackAddDamage", "", "", 0f, "dao", "", "", "");
            config[2000041] = new SkillConfig(2000041, "坚韧", "士", "自身生命/1，全队生命/2", "+50;+20", "职业", "", 1, 0f, 0f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, 0f, 0, 0f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "Dumb", "", "", 0f, "shi", "hp+50", "hp+20", "");
            config[2000042] = new SkillConfig(2000042, "坚韧", "士", "", "+100;+40", "职业", "", 2, 0f, 0f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, 0f, 0, 0f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "Dumb", "", "", 0f, "shi", "hp+100", "hp+40", "");
            config[2000043] = new SkillConfig(2000043, "坚韧", "士", "", "+200;+60", "职业", "", 3, 0f, 0f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, 0f, 0, 0f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "Dumb", "", "", 0f, "shi", "hp+200", "hp+60", "");
            config[2000044] = new SkillConfig(2000044, "坚韧", "士", "", "+400;+80", "职业", "", 4, 0f, 0f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, 0f, 0, 0f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "Dumb", "", "", 0f, "shi", "hp+400", "hp+80", "");
            config[2000045] = new SkillConfig(2000045, "坚韧", "士", "", "+800;+100", "职业", "", 5, 0f, 0f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, 0f, 0, 0f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "Dumb", "", "", 0f, "shi", "hp+800", "hp+100", "");
            config[2000051] = new SkillConfig(2000051, "灵活", "马", "自身闪避/1", "+5%", "职业", "", 1, 0f, 0f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, 0f, 0, 0f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "Dumb", "", "", 0f, "ma", "dodge+0.05", "", "");
            config[2000052] = new SkillConfig(2000052, "灵活", "马", "", "+10%", "职业", "", 2, 0f, 0f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, 0f, 0, 0f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "Dumb", "", "", 0f, "ma", "dodge+0.10", "", "");
            config[2000053] = new SkillConfig(2000053, "灵活", "马", "", "+20%", "职业", "", 3, 0f, 0f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, 0f, 0, 0f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "Dumb", "", "", 0f, "ma", "dodge+0.20", "", "");
            config[2000054] = new SkillConfig(2000054, "灵活", "马", "", "+33%", "职业", "", 4, 0f, 0f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, 0f, 0, 0f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "Dumb", "", "", 0f, "ma", "dodge+0.33", "", "");
            config[2000055] = new SkillConfig(2000055, "灵活", "马", "", "+45%", "职业", "", 5, 0f, 0f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, 0f, 0, 0f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "Dumb", "", "", 0f, "ma", "dodge+0.45", "", "");
            config[2000061] = new SkillConfig(2000061, "运筹", "相", "全军士兵攻击/1，生命/2，全队法力回复/3/秒", "+10%;+10%;+1", "职业", "", 1, 0f, 0f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, 0f, 0, 0f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "Dumb", "", "", 0f, "xiang", "soldierAtk+0.1,soldierHp+0.1", "mpRegen+1", "");
            config[2000062] = new SkillConfig(2000062, "运筹", "相", "", "+15%;+15%;+2", "职业", "", 2, 0f, 0f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, 0f, 0, 0f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "Dumb", "", "", 0f, "xiang", "soldierAtk+0.15,soldierHp+0.15", "mpRegen+2", "");
            config[2000063] = new SkillConfig(2000063, "运筹", "相", "", "+20%;+20%;+3", "职业", "", 3, 0f, 0f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, 0f, 0, 0f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "Dumb", "", "", 0f, "xiang", "soldierAtk+0.2,soldierHp+0.2", "mpRegen+3", "");
            config[2000064] = new SkillConfig(2000064, "运筹", "相", "", "+27%;+27%;+4", "职业", "", 4, 0f, 0f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, 0f, 0, 0f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "Dumb", "", "", 0f, "xiang", "soldierAtk+0.27,soldierHp+0.27", "mpRegen+4", "");
            config[2000065] = new SkillConfig(2000065, "运筹", "相", "", "+35%;+35%;+5", "职业", "", 5, 0f, 0f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, 0f, 0, 0f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "Dumb", "", "", 0f, "xiang", "soldierAtk+0.35,soldierHp+0.35", "mpRegen+5", "");
            config[2000071] = new SkillConfig(2000071, "弓手", "弓", "自身攻击/1，全队攻击/2", "+5;+3", "职业", "", 1, 0f, 0f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, 0f, 0, 0f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "Dumb", "", "", 0f, "gong", "atk+5", "atk+3", "");
            config[2000072] = new SkillConfig(2000072, "弓手", "弓", "", "+10;+6", "职业", "", 2, 0f, 0f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, 0f, 0, 0f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "Dumb", "", "", 0f, "gong", "atk+10", "atk+6", "");
            config[2000073] = new SkillConfig(2000073, "弓手", "弓", "", "+20;+9", "职业", "", 3, 0f, 0f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, 0f, 0, 0f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "Dumb", "", "", 0f, "gong", "atk+20", "atk+9", "");
            config[2000074] = new SkillConfig(2000074, "弓手", "弓", "", "+40;+12", "职业", "", 4, 0f, 0f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, 0f, 0, 0f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "Dumb", "", "", 0f, "gong", "atk+40", "atk+12", "");
            config[2000075] = new SkillConfig(2000075, "弓手", "弓", "", "+80;+15", "职业", "", 5, 0f, 0f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, 0f, 0, 0f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "Dumb", "", "", 0f, "gong", "atk+80", "atk+15", "");
            config[2000081] = new SkillConfig(2000081, "谋略", "棋", "自身法强/1，全队法强/2", "+5;+3", "职业", "", 1, 0f, 0f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, 0f, 0, 0f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "Dumb", "", "", 0f, "mou", "ap+5", "ap+3", "");
            config[2000082] = new SkillConfig(2000082, "谋略", "棋", "", "+10;+6", "职业", "", 2, 0f, 0f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, 0f, 0, 0f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "Dumb", "", "", 0f, "mou", "ap+10", "ap+6", "");
            config[2000083] = new SkillConfig(2000083, "谋略", "棋", "", "+20;+9", "职业", "", 3, 0f, 0f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, 0f, 0, 0f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "Dumb", "", "", 0f, "mou", "ap+20", "ap+9", "");
            config[2000084] = new SkillConfig(2000084, "谋略", "棋", "", "+40;+12", "职业", "", 4, 0f, 0f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, 0f, 0, 0f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "Dumb", "", "", 0f, "mou", "ap+40", "ap+12", "");
            config[2000085] = new SkillConfig(2000085, "谋略", "棋", "", "+80;+15", "职业", "", 5, 0f, 0f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, 0f, 0, 0f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "Dumb", "", "", 0f, "mou", "ap+80", "ap+15", "");
            config[2000091] = new SkillConfig(2000091, "炮车", "炮", "攻击时/1几率对周围造成50%溅射伤害，范围/2", "20%;+0", "职业", "", 1, 0.2f, 0f, 0, "", 0f, false, "", 0, 0f, 6f, "", 3, 0f, 0, 0.5f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "HitArea", "", "SparkleAreaWhite", 0f, "pao", "", "", "");
            config[2000092] = new SkillConfig(2000092, "炮车", "炮", "", "25%;+15%", "职业", "", 2, 0.25f, 0f, 0, "", 0f, false, "", 0, 0f, 6.9f, "", 3, 0f, 0, 0.5f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "HitArea", "", "SparkleAreaWhite", 0f, "pao", "", "", "");
            config[2000093] = new SkillConfig(2000093, "炮车", "炮", "", "27%;+30%", "职业", "", 3, 0.27f, 0f, 0, "", 0f, false, "", 0, 0f, 7.8f, "", 3, 0f, 0, 0.5f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "HitArea", "", "SparkleAreaWhite", 0f, "pao", "", "", "");
            config[2000094] = new SkillConfig(2000094, "炮车", "炮", "", "30%;+50%", "职业", "", 4, 0.3f, 0f, 0, "", 0f, false, "", 0, 0f, 9f, "", 3, 0f, 0, 0.5f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "HitArea", "", "SparkleAreaWhite", 0f, "pao", "", "", "");
            config[2000095] = new SkillConfig(2000095, "炮车", "炮", "", "35%;+80%", "职业", "", 5, 0.35f, 0f, 0, "", 0f, false, "", 0, 0f, 10.8f, "", 3, 0f, 0, 0.5f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "HitArea", "", "SparkleAreaWhite", 0f, "pao", "", "", "");
            config[2000101] = new SkillConfig(2000101, "弩手", "弩", "自身攻速/1，全队攻速/2", "+10%;+5%", "职业", "", 1, 0f, 0f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, 0f, 0, 0f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "Dumb", "", "", 0f, "nu", "atkspeed+0.1", "atkspeed+0.05", "");
            config[2000102] = new SkillConfig(2000102, "弩手", "弩", "", "+20%;+10%", "职业", "", 2, 0f, 0f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, 0f, 0, 0f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "Dumb", "", "", 0f, "nu", "atkspeed+0.2", "atkspeed+0.1", "");
            config[2000103] = new SkillConfig(2000103, "弩手", "弩", "", "+35%;+15%", "职业", "", 3, 0f, 0f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, 0f, 0, 0f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "Dumb", "", "", 0f, "nu", "atkspeed+0.35", "atkspeed+0.15", "");
            config[2000104] = new SkillConfig(2000104, "弩手", "弩", "", "+50%;+20%", "职业", "", 4, 0f, 0f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, 0f, 0, 0f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "Dumb", "", "", 0f, "nu", "atkspeed+0.5", "atkspeed+0.2", "");
            config[2000105] = new SkillConfig(2000105, "弩手", "弩", "", "+70%;+25%", "职业", "", 5, 0f, 0f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, 0f, 0, 0f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "Dumb", "", "", 0f, "nu", "atkspeed+0.7", "atkspeed+0.25", "");
            config[2000111] = new SkillConfig(2000111, "碾压", "车", "自身暴击/1，全队暴击/2", "+10%;+3%", "职业", "", 1, 0f, 0f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, 0f, 0, 0f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "Dumb", "", "", 0f, "che", "crit+0.1", "crit+0.03", "");
            config[2000112] = new SkillConfig(2000112, "碾压", "车", "", "+17%;+5%", "职业", "", 2, 0f, 0f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, 0f, 0, 0f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "Dumb", "", "", 0f, "che", "crit+0.17", "crit+0.05", "");
            config[2000113] = new SkillConfig(2000113, "碾压", "车", "", "+25%;+7%", "职业", "", 3, 0f, 0f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, 0f, 0, 0f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "Dumb", "", "", 0f, "che", "crit+0.25", "crit+0.07", "");
            config[2000114] = new SkillConfig(2000114, "碾压", "车", "", "+35%;+10%", "职业", "", 4, 0f, 0f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, 0f, 0, 0f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "Dumb", "", "", 0f, "che", "crit+0.35", "crit+0.10", "");
            config[2000115] = new SkillConfig(2000115, "碾压", "车", "", "+45%;+13%", "职业", "", 5, 0f, 0f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, 0f, 0, 0f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "Dumb", "", "", 0f, "che", "crit+0.45", "crit+0.13", "");
            config[2000121] = new SkillConfig(2000121, "声乐", "琴", "自身施加的正面buff持续/1，全队生命回复/2/秒", "+10%;+2", "职业", "", 1, 0f, 0f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, 0.1f, 0, 0f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "ModifyBuffTime", "", "", 0f, "song", "", "hpRegen+2", "");
            config[2000122] = new SkillConfig(2000122, "声乐", "琴", "", "+20%;+4", "职业", "", 2, 0f, 0f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, 0.2f, 0, 0f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "ModifyBuffTime", "", "", 0f, "song", "", "hpRegen+4", "");
            config[2000123] = new SkillConfig(2000123, "声乐", "琴", "", "+35%;+6", "职业", "", 3, 0f, 0f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, 0.35f, 0, 0f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "ModifyBuffTime", "", "", 0f, "song", "", "hpRegen+6", "");
            config[2000124] = new SkillConfig(2000124, "声乐", "琴", "", "+50%;+8", "职业", "", 4, 0f, 0f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, 0.5f, 0, 0f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "ModifyBuffTime", "", "", 0f, "song", "", "hpRegen+8", "");
            config[2000125] = new SkillConfig(2000125, "声乐", "琴", "", "+70%;+10", "职业", "", 5, 0f, 0f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, 0.7f, 0, 0f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "ModifyBuffTime", "", "", 0f, "song", "", "hpRegen+10", "");
            config[2000131] = new SkillConfig(2000131, "治疗", "医", "自身治疗/1，全队生命回复/2/秒", "+10%;+3", "职业", "", 1, 0f, 0f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, 0f, 0, 0f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "Dumb", "", "", 0f, "heal", "healRate+0.1", "hpRegen+3", "");
            config[2000132] = new SkillConfig(2000132, "治疗", "医", "", "+15%;+6", "职业", "", 2, 0f, 0f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, 0f, 0, 0f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "Dumb", "", "", 0f, "heal", "healRate+0.15", "hpRegen+6", "");
            config[2000133] = new SkillConfig(2000133, "治疗", "医", "", "+20%;+9", "职业", "", 3, 0f, 0f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, 0f, 0, 0f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "Dumb", "", "", 0f, "heal", "healRate+0.2", "hpRegen+9", "");
            config[2000134] = new SkillConfig(2000134, "治疗", "医", "", "+25%;+12", "职业", "", 4, 0f, 0f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, 0f, 0, 0f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "Dumb", "", "", 0f, "heal", "healRate+0.25", "hpRegen+12", "");
            config[2000135] = new SkillConfig(2000135, "治疗", "医", "", "+30%;+15", "职业", "", 5, 0f, 0f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, 0f, 0, 0f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "Dumb", "", "", 0f, "heal", "healRate+0.3", "hpRegen+15", "");
            config[2000141] = new SkillConfig(2000141, "枪阵", "枪", "攻击时/1几率眩晕目标1.5秒", "10%", "职业", "", 1, 0.1f, 0f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, 0f, 0, 0f, 0, "", "", "乱", false, 1.5f, "", 0, 0f, 0f, 0f, 0, "HitBuff", "", "", 0f, "qiang", "", "", "");
            config[2000142] = new SkillConfig(2000142, "枪阵", "枪", "", "16%", "职业", "", 2, 0.16f, 0f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, 0f, 0, 0f, 0, "", "", "乱", false, 1.5f, "", 0, 0f, 0f, 0f, 0, "HitBuff", "", "", 0f, "qiang", "", "", "");
            config[2000143] = new SkillConfig(2000143, "枪阵", "枪", "", "25%", "职业", "", 3, 0.25f, 0f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, 0f, 0, 0f, 0, "", "", "乱", false, 1.5f, "", 0, 0f, 0f, 0f, 0, "HitBuff", "", "", 0f, "qiang", "", "", "");
            config[2000144] = new SkillConfig(2000144, "枪阵", "枪", "", "35%", "职业", "", 4, 0.35f, 0f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, 0f, 0, 0f, 0, "", "", "乱", false, 1.5f, "", 0, 0f, 0f, 0f, 0, "HitBuff", "", "", 0f, "qiang", "", "", "");
            config[2000145] = new SkillConfig(2000145, "枪阵", "枪", "", "50%", "职业", "", 5, 0.5f, 0f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, 0f, 0, 0f, 0, "", "", "乱", false, 1.5f, "", 0, 0f, 0f, 0f, 0, "HitBuff", "", "", 0f, "qiang", "", "", "");
            config[2000151] = new SkillConfig(2000151, "戟阵", "戟", "攻击时/1几率对周围造成50%溅射伤害", "10%", "职业", "", 1, 0.1f, 0f, 0, "", 0f, false, "", 0, 0f, 5f, "", 2, 0f, 0, 0.5f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "HitArea", "", "SparkleAreaWhite", 0f, "ji", "", "", "");
            config[2000152] = new SkillConfig(2000152, "戟阵", "戟", "", "16%", "职业", "", 2, 0.16f, 0f, 0, "", 0f, false, "", 0, 0f, 5f, "", 2, 0f, 0, 0.5f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "HitArea", "", "SparkleAreaWhite", 0f, "ji", "", "", "");
            config[2000153] = new SkillConfig(2000153, "戟阵", "戟", "", "25%", "职业", "", 3, 0.25f, 0f, 0, "", 0f, false, "", 0, 0f, 5f, "", 2, 0f, 0, 0.5f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "HitArea", "", "SparkleAreaWhite", 0f, "ji", "", "", "");
            config[2000154] = new SkillConfig(2000154, "戟阵", "戟", "", "35%", "职业", "", 4, 0.35f, 0f, 0, "", 0f, false, "", 0, 0f, 5f, "", 2, 0f, 0, 0.5f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "HitArea", "", "SparkleAreaWhite", 0f, "ji", "", "", "");
            config[2000155] = new SkillConfig(2000155, "戟阵", "戟", "", "50%", "职业", "", 5, 0.5f, 0f, 0, "", 0f, false, "", 0, 0f, 5f, "", 2, 0f, 0, 0.5f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "HitArea", "", "SparkleAreaWhite", 0f, "ji", "", "", "");
            config[2000161] = new SkillConfig(2000161, "战鼓", "鼓", "自身光环效果/1", "+10%", "职业", "", 1, 0f, 0f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, 0f, 0, 0f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "Dumb", "", "", 0f, "gu", "auroEffectRate+0.1", "", "");
            config[2000162] = new SkillConfig(2000162, "战鼓", "鼓", "", "+20%", "职业", "", 2, 0f, 0f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, 0f, 0, 0f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "Dumb", "", "", 0f, "gu", "auroEffectRate+0.2", "", "");
            config[2000163] = new SkillConfig(2000163, "战鼓", "鼓", "", "+35%", "职业", "", 3, 0f, 0f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, 0f, 0, 0f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "Dumb", "", "", 0f, "gu", "auroEffectRate+0.35", "", "");
            config[2000164] = new SkillConfig(2000164, "战鼓", "鼓", "", "+50%", "职业", "", 4, 0f, 0f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, 0f, 0, 0f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "Dumb", "", "", 0f, "gu", "auroEffectRate+0.5", "", "");
            config[2000165] = new SkillConfig(2000165, "战鼓", "鼓", "", "+70%", "职业", "", 5, 0f, 0f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, 0f, 0, 0f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "Dumb", "", "", 0f, "gu", "auroEffectRate+0.7", "", "");
            config[2000171] = new SkillConfig(2000171, "铁壁", "盾", "自身护甲/1，全队护甲/2", "+10;+3", "职业", "", 1, 0f, 0f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, 0f, 0, 0f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "Dumb", "", "", 0f, "dun", "armor+10", "armor+3", "");
            config[2000172] = new SkillConfig(2000172, "铁壁", "盾", "", "+20;+6", "职业", "", 2, 0f, 0f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, 0f, 0, 0f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "Dumb", "", "", 0f, "dun", "armor+20", "armor+6", "");
            config[2000173] = new SkillConfig(2000173, "铁壁", "盾", "", "+35;+9", "职业", "", 3, 0f, 0f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, 0f, 0, 0f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "Dumb", "", "", 0f, "dun", "armor+35", "armor+9", "");
            config[2000174] = new SkillConfig(2000174, "铁壁", "盾", "", "+60;+12", "职业", "", 4, 0f, 0f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, 0f, 0, 0f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "Dumb", "", "", 0f, "dun", "armor+60", "armor+12", "");
            config[2000175] = new SkillConfig(2000175, "铁壁", "盾", "", "+90;+15", "职业", "", 5, 0f, 0f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, 0f, 0, 0f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "Dumb", "", "", 0f, "dun", "armor+90", "armor+15", "");
            config[2000181] = new SkillConfig(2000181, "机巧", "工", "战斗开始时召唤一个木牛流马lv1", "", "职业", "", 1, 0f, 0f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, 0f, 0, 0f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "Dumb", "", "", 0f, "gong3", "", "", "");
            config[2000182] = new SkillConfig(2000182, "机巧", "工", "战斗开始时召唤一个木牛流马lv2", "", "职业", "", 2, 0f, 0f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, 0f, 0, 0f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "Dumb", "", "", 0f, "gong3", "", "", "");
            config[2000183] = new SkillConfig(2000183, "机巧", "工", "战斗开始时召唤一个木牛流马lv2与一个喷火兽lv1", "", "职业", "", 3, 0f, 0f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, 0f, 0, 0f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "Dumb", "", "", 0f, "gong3", "", "", "");
            config[2000184] = new SkillConfig(2000184, "机巧", "工", "战斗开始时召唤一个木牛流马lv2与一个喷火兽lv2", "", "职业", "", 4, 0f, 0f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, 0f, 0, 0f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "Dumb", "", "", 0f, "gong3", "", "", "");
            config[2000185] = new SkillConfig(2000185, "机巧", "工", "战斗开始时召唤两个木牛流马lv2与一个喷火兽lv2与一个辅助兽", "", "职业", "", 5, 0f, 0f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, 0f, 0, 0f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "Dumb", "", "", 0f, "gong3", "", "", "");
            config[2010001] = new SkillConfig(2010001, "突破", "突", "攻击时穿越敌人，造成/1额外伤害", "15%", "连接", "", 1, 1f, 5f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, 0f, 0, 0.15f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AttackRunCross", "", "LightningExplosionBlue", 0f, "tu", "", "", "");
            config[2010002] = new SkillConfig(2010002, "突破", "突", "", "25%", "连接", "", 2, 1f, 5f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, 0f, 0, 0.25f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AttackRunCross", "", "LightningExplosionBlue", 0f, "tu", "", "", "");
            config[2010003] = new SkillConfig(2010003, "突破", "突", "", "50%", "连接", "", 3, 1f, 5f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, 0f, 0, 0.5f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AttackRunCross", "", "LightningExplosionBlue", 0f, "tu", "", "", "");
            config[2010004] = new SkillConfig(2010004, "突破", "突", "", "80%", "连接", "", 4, 1f, 5f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, 0f, 0, 0.8f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AttackRunCross", "", "LightningExplosionBlue", 0f, "tu", "", "", "");
            config[2010005] = new SkillConfig(2010005, "突破", "突", "", "130%", "连接", "", 5, 1f, 5f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, 0f, 0, 1.3f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AttackRunCross", "", "LightningExplosionBlue", 0f, "tu", "", "", "");
            config[2010006] = new SkillConfig(2010006, "冲锋", "冲", "攻击时穿越敌人，降低目标防御/1s(最多/2目标)", "2;2", "连接", "", 1, 1f, 6f, 0, "", 0f, false, "", 0, 12f, 0f, "", 2, 0f, 0, 0f, 0, "", "", "伤", false, 2f, "", 0, 0f, 0f, 0f, 0, "AttackRunCrossPlus", "", "LightningExplosionRed", 0f, "chong", "", "", "");
            config[2010007] = new SkillConfig(2010007, "冲锋", "冲", "", "3;3", "连接", "", 2, 1f, 6f, 0, "", 0f, false, "", 0, 12f, 0f, "", 3, 0f, 0, 0f, 0, "", "", "伤", false, 3f, "", 0, 0f, 0f, 0f, 0, "AttackRunCrossPlus", "", "LightningExplosionRed", 0f, "chong", "", "", "");
            config[2010008] = new SkillConfig(2010008, "冲锋", "冲", "", "4;4", "连接", "", 3, 1f, 6f, 0, "", 0f, false, "", 0, 12f, 0f, "", 4, 0f, 0, 0f, 0, "", "", "伤", false, 4f, "", 0, 0f, 0f, 0f, 0, "AttackRunCrossPlus", "", "LightningExplosionRed", 0f, "chong", "", "", "");
            config[2010009] = new SkillConfig(2010009, "冲锋", "冲", "", "5;5", "连接", "", 4, 1f, 6f, 0, "", 0f, false, "", 0, 12f, 0f, "", 5, 0f, 0, 0f, 0, "", "", "伤", false, 5f, "", 0, 0f, 0f, 0f, 0, "AttackRunCrossPlus", "", "LightningExplosionRed", 0f, "chong", "", "", "");
            config[2010010] = new SkillConfig(2010010, "冲锋", "冲", "", "6;6", "连接", "", 5, 1f, 6f, 0, "", 0f, false, "", 0, 12f, 0f, "", 6, 0f, 0, 0f, 0, "", "", "伤", false, 6f, "", 0, 0f, 0f, 0f, 0, "AttackRunCrossPlus", "", "LightningExplosionRed", 0f, "chong", "", "", "");
            config[2010011] = new SkillConfig(2010011, "连击", "连", "攻击时/1触发连续攻击", "10%", "连接", "", 1, 0.1f, 4f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, 0.9f, 0, 0f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AttackSpeedAttack", "flipspin", "", 0f, "lian", "", "", "");
            config[2010012] = new SkillConfig(2010012, "连击", "连", "", "20%", "连接", "", 2, 0.2f, 3.5f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, 0.9f, 0, 0f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AttackSpeedAttack", "flipspin", "", 0f, "lian", "", "", "");
            config[2010013] = new SkillConfig(2010013, "连击", "连", "", "33%", "连接", "", 3, .33f, 3f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, 0.9f, 0, 0f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AttackSpeedAttack", "flipspin", "", 0f, "lian", "", "", "");
            config[2010014] = new SkillConfig(2010014, "连击", "连", "", "45%", "连接", "", 4, 0.45f, 2.5f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, 0.9f, 0, 0f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AttackSpeedAttack", "flipspin", "", 0f, "lian", "", "", "");
            config[2010015] = new SkillConfig(2010015, "连击", "连", "", "60%", "连接", "", 5, 0.60f, 2f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, 0.9f, 0, 0f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AttackSpeedAttack", "flipspin", "", 0f, "lian", "", "", "");
            config[2010016] = new SkillConfig(2010016, "火攻", "火", "攻击时/1几率对目标放火，若目标有火则传递，造成/2法术伤害", "20%;10%", "连接", "", 1, 0.2f, 4f, 0, "", 0f, true, "", 0, 0f, 30f, "", 1, .1f, 0, 0.1f, 0, "", "", "", false, 0f, "火", 1, 3.2f, 1f, 0f, 0, "HitFireArea", "throw", "SoftFireBigRed", 1.6f, "huo", "", "", "");
            config[2010017] = new SkillConfig(2010017, "火攻", "火", "", "25%;15%", "连接", "", 2, 0.25f, 4f, 0, "", 0f, true, "", 0, 0f, 30f, "", 1, .15f, 0, 0.15f, 0, "", "", "", false, 0f, "火", 1, 3.2f, 1f, 0f, 0, "HitFireArea", "throw", "SoftFireBigRed", 1.6f, "huo", "", "", "");
            config[2010018] = new SkillConfig(2010018, "火攻", "火", "", "30%;20%", "连接", "", 3, 0.3f, 4f, 0, "", 0f, true, "", 0, 0f, 30f, "", 1, .2f, 0, 0.2f, 0, "", "", "", false, 0f, "火", 1, 3.2f, 1f, 0f, 0, "HitFireArea", "throw", "SoftFireBigRed", 1.6f, "huo", "", "", "");
            config[2010019] = new SkillConfig(2010019, "火攻", "火", "", "35%;25%", "连接", "", 4, 0.35f, 4f, 0, "", 0f, true, "", 0, 0f, 30f, "", 1, .25f, 0, 0.25f, 0, "", "", "", false, 0f, "火", 1, 3.2f, 1f, 0f, 0, "HitFireArea", "throw", "SoftFireBigRed", 1.6f, "huo", "", "", "");
            config[2010020] = new SkillConfig(2010020, "火攻", "火", "", "40%;30%", "连接", "", 5, 0.4f, 4f, 0, "", 0f, true, "", 0, 0f, 30f, "", 1, .3f, 0, 0.3f, 0, "", "", "", false, 0f, "火", 1, 3.2f, 1f, 0f, 0, "HitFireArea", "throw", "SoftFireBigRed", 1.6f, "huo", "", "", "");
            config[2010021] = new SkillConfig(2010021, "破盾", "破", "每次攻击，对有护盾的目标/1概率额外造成/2物理穿透伤害", "20%;30%", "连接", "", 1, .2f, 0f, 0, "", 0f, false, "AntiShield", 0, 0f, 0f, "", 0, 0.3f, 0, 0f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AttackShieldPierce", "", "SoftFireBigRed", 0f, "po", "", "", "");
            config[2010022] = new SkillConfig(2010022, "破盾", "破", "", "25%;35%", "连接", "", 2, .25f, 0f, 0, "", 0f, false, "AntiShield", 0, 0f, 0f, "", 0, 0.35f, 0, 0f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AttackShieldPierce", "", "SoftFireBigRed", 0f, "po", "", "", "");
            config[2010023] = new SkillConfig(2010023, "破盾", "破", "", "30%;40%", "连接", "", 3, .3f, 0f, 0, "", 0f, false, "AntiShield", 0, 0f, 0f, "", 0, 0.4f, 0, 0f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AttackShieldPierce", "", "SoftFireBigRed", 0f, "po", "", "", "");
            config[2010024] = new SkillConfig(2010024, "破盾", "破", "", "40%;45%", "连接", "", 4, .4f, 0f, 0, "", 0f, false, "AntiShield", 0, 0f, 0f, "", 0, 0.45f, 0, 0f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AttackShieldPierce", "", "SoftFireBigRed", 0f, "po", "", "", "");
            config[2010025] = new SkillConfig(2010025, "破盾", "破", "", "50%;50%", "连接", "", 5, .5f, 0f, 0, "", 0f, false, "AntiShield", 0, 0f, 0f, "", 0, 0.5f, 0, 0f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AttackShieldPierce", "", "SoftFireBigRed", 0f, "po", "", "", "");
            config[2010026] = new SkillConfig(2010026, "冷箭", "冷", "能够射出冷箭，造成/1伤害", "60%", "连接", "", 1, 1f, 4f, 0, "", 1f, false, "", 0, 30f, 0f, "", 0, 0f, 0, 0.6f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AidSuddenArrow", "sway", "BulletExplosionBlue", 3f, "leng", "", "", "");
            config[2010027] = new SkillConfig(2010027, "冷箭", "冷", "", "80%", "连接", "", 2, 1f, 4f, 0, "", 1f, false, "", 0, 40f, 0f, "", 0, 0f, 0, 0.8f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AidSuddenArrow", "sway", "BulletExplosionBlue", 3f, "leng", "", "", "");
            config[2010028] = new SkillConfig(2010028, "冷箭", "冷", "", "100%", "连接", "", 3, 1f, 4f, 0, "", 1f, false, "", 0, 50f, 0f, "", 0, 0f, 0, 1f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AidSuddenArrow", "sway", "BulletExplosionBlue", 3f, "leng", "", "", "");
            config[2010029] = new SkillConfig(2010029, "冷箭", "冷", "", "120%", "连接", "", 4, 1f, 4f, 0, "", 1f, false, "", 0, 60f, 0f, "", 0, 0f, 0, 1.2f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AidSuddenArrow", "sway", "BulletExplosionBlue", 3f, "leng", "", "", "");
            config[2010030] = new SkillConfig(2010030, "冷箭", "冷", "", "140%", "连接", "", 5, 1f, 4f, 0, "", 1f, false, "", 0, 80f, 0f, "", 0, 0f, 0, 1.5f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AidSuddenArrow", "sway", "BulletExplosionBlue", 3f, "leng", "", "", "");
            config[2010031] = new SkillConfig(2010031, "穿甲", "穿", "物理伤害无视目标/1护甲", "10%", "连接", "", 1, 0f, 0f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, 0.1f, 0, 0f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AttackArmorPierce", "", "", 0f, "chuan", "", "", "");
            config[2010032] = new SkillConfig(2010032, "穿甲", "穿", "", "20%", "连接", "", 2, 0f, 0f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, 0.2f, 0, 0f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AttackArmorPierce", "", "", 0f, "chuan", "", "", "");
            config[2010033] = new SkillConfig(2010033, "穿甲", "穿", "", "30%", "连接", "", 3, 0f, 0f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, 0.3f, 0, 0f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AttackArmorPierce", "", "", 0f, "chuan", "", "", "");
            config[2010034] = new SkillConfig(2010034, "穿甲", "穿", "", "40%", "连接", "", 4, 0f, 0f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, 0.4f, 0, 0f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AttackArmorPierce", "", "", 0f, "chuan", "", "", "");
            config[2010035] = new SkillConfig(2010035, "穿甲", "穿", "", "50%", "连接", "", 5, 0f, 0f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, 0.5f, 0, 0f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AttackArmorPierce", "", "", 0f, "chuan", "", "", "");
            config[2010036] = new SkillConfig(2010036, "无双", "双", "攻击时/1对目标进行三连击", "10%", "连接", "", 1, 0.10f, 8f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, 0f, 2, 1f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "HitRepeat", "jumpspin", "SwordHitRedCritical", 0f, "shuang", "", "", "");
            config[2010037] = new SkillConfig(2010037, "无双", "双", "", "15%", "连接", "", 2, 0.15f, 7f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, 0f, 2, 1f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "HitRepeat", "jumpspin", "SwordHitRedCritical", 0f, "shuang", "", "", "");
            config[2010038] = new SkillConfig(2010038, "无双", "双", "", "20%", "连接", "", 3, 0.2f, 6f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, 0f, 2, 1f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "HitRepeat", "jumpspin", "SwordHitRedCritical", 0f, "shuang", "", "", "");
            config[2010039] = new SkillConfig(2010039, "无双", "双", "", "25%", "连接", "", 4, 0.25f, 5f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, 0f, 2, 1f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "HitRepeat", "jumpspin", "SwordHitRedCritical", 0f, "shuang", "", "", "");
            config[2010040] = new SkillConfig(2010040, "无双", "双", "", "30%", "连接", "", 5, 0.30f, 4f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, 0f, 2, 1f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "HitRepeat", "jumpspin", "SwordHitRedCritical", 0f, "shuang", "", "", "");
            config[2010041] = new SkillConfig(2010041, "坚毅", "坚", "生命值低时减免/1伤害", "20%", "连接", "", 1, 1f, 0f, 0, "hprate<50", 0f, false, "", 0, 0f, 0f, "", 0, 0.2f, 0, 0f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "ReduceDamageRate", "", "", 0f, "jian", "", "", "");
            config[2010042] = new SkillConfig(2010042, "坚毅", "坚", "", "30%", "连接", "", 2, 1f, 0f, 0, "hprate<50", 0f, false, "", 0, 0f, 0f, "", 0, 0.3f, 0, 0f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "ReduceDamageRate", "", "", 0f, "jian", "", "", "");
            config[2010043] = new SkillConfig(2010043, "坚毅", "坚", "", "40%", "连接", "", 3, 1f, 0f, 0, "hprate<50", 0f, false, "", 0, 0f, 0f, "", 0, 0.4f, 0, 0f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "ReduceDamageRate", "", "", 0f, "jian", "", "", "");
            config[2010044] = new SkillConfig(2010044, "坚毅", "坚", "", "50%", "连接", "", 4, 1f, 0f, 0, "hprate<50", 0f, false, "", 0, 0f, 0f, "", 0, 0.5f, 0, 0f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "ReduceDamageRate", "", "", 0f, "jian", "", "", "");
            config[2010045] = new SkillConfig(2010045, "坚毅", "坚", "", "60%", "连接", "", 5, 1f, 0f, 0, "hprate<50", 0f, false, "", 0, 0f, 0f, "", 0, 0.6f, 0, 0f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "ReduceDamageRate", "", "", 0f, "jian", "", "", "");
            config[2010046] = new SkillConfig(2010046, "谋略", "谋", "攻击时/1几率眩晕目标，攻击眩晕目标额外造成50%伤害", "10%", "连接", "", 1, 0.1f, 0f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, 0f, 0, 0.5f, 0, "", "", "乱", false, 1.5f, "", 0, 0f, 0f, 0f, 0, "AttackStunDamage", "", "SoftFireBigRed", 0f, "mou2", "", "", "");
            config[2010047] = new SkillConfig(2010047, "谋略", "谋", "", "16%", "连接", "", 2, 0.16f, 0f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, 0f, 0, 0.5f, 0, "", "", "乱", false, 1.5f, "", 0, 0f, 0f, 0f, 0, "AttackStunDamage", "", "SoftFireBigRed", 0f, "mou2", "", "", "");
            config[2010048] = new SkillConfig(2010048, "谋略", "谋", "", "25%", "连接", "", 3, 0.25f, 0f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, 0f, 0, 0.5f, 0, "", "", "乱", false, 1.5f, "", 0, 0f, 0f, 0f, 0, "AttackStunDamage", "", "SoftFireBigRed", 0f, "mou2", "", "", "");
            config[2010049] = new SkillConfig(2010049, "谋略", "谋", "", "35%", "连接", "", 4, 0.35f, 0f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, 0f, 0, 0.5f, 0, "", "", "乱", false, 1.5f, "", 0, 0f, 0f, 0f, 0, "AttackStunDamage", "", "SoftFireBigRed", 0f, "mou2", "", "", "");
            config[2010050] = new SkillConfig(2010050, "谋略", "谋", "", "50%", "连接", "", 5, 0.5f, 0f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, 0f, 0, 0.5f, 0, "", "", "乱", false, 1.5f, "", 0, 0f, 0f, 0f, 0, "AttackStunDamage", "", "SoftFireBigRed", 0f, "mou2", "", "", "");
            config[2010051] = new SkillConfig(2010051, "明镜", "镜", "提升/1点法术抗性", "20", "连接", "", 1, 0f, 0f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, 0f, 0, 0f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "InitAttrChange", "", "", 0f, "jing", "magicres+20", "", "");
            config[2010052] = new SkillConfig(2010052, "明镜", "镜", "", "40", "连接", "", 2, 0f, 0f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, 0f, 0, 0f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "InitAttrChange", "", "", 0f, "jing", "magicres+40", "", "");
            config[2010053] = new SkillConfig(2010053, "明镜", "镜", "", "60", "连接", "", 3, 0f, 0f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, 0f, 0, 0f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "InitAttrChange", "", "", 0f, "jing", "magicres+60", "", "");
            config[2010054] = new SkillConfig(2010054, "明镜", "镜", "", "80", "连接", "", 4, 0f, 0f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, 0f, 0, 0f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "InitAttrChange", "", "", 0f, "jing", "magicres+80", "", "");
            config[2010055] = new SkillConfig(2010055, "明镜", "镜", "", "100", "连接", "", 5, 0f, 0f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, 0f, 0, 0f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "InitAttrChange", "", "", 0f, "jing", "magicres+100", "", "");
            config[2010056] = new SkillConfig(2010056, "背水", "背", "阵亡时回复同组我方英雄/1最大生命，提升其/2攻击与法术强度", "20%;20%", "连接", "", 1, 0f, 0f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, 0.2f, 0, 0.2f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "DeathGroupHeal", "", "", 0f, "bei", "", "", "");
            config[2010057] = new SkillConfig(2010057, "背水", "背", "", "30%;25%", "连接", "", 2, 0f, 0f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, 0.3f, 0, 0.25f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "DeathGroupHeal", "", "", 0f, "bei", "", "", "");
            config[2010058] = new SkillConfig(2010058, "背水", "背", "", "40%;30%", "连接", "", 3, 0f, 0f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, 0.4f, 0, 0.3f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "DeathGroupHeal", "", "", 0f, "bei", "", "", "");
            config[2010059] = new SkillConfig(2010059, "背水", "背", "", "50%;35%", "连接", "", 4, 0f, 0f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, 0.5f, 0, 0.35f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "DeathGroupHeal", "", "", 0f, "bei", "", "", "");
            config[2010060] = new SkillConfig(2010060, "背水", "背", "", "60%;40%", "连接", "", 5, 0f, 0f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, 0.6f, 0, 0.4f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "DeathGroupHeal", "", "", 0f, "bei", "", "", "");
            config[2010061] = new SkillConfig(2010061, "护卫", "护", "给生命比例最低的友方英雄减伤盾，护盾值=自身法术强度的/1", "200%", "连接", "", 1, 1f, 8f, 0, "", 1f, false, "", 0, 80f, 0f, "", 0, 0f, 0, 2f, 0, "", "", "盾", false, 999f, "", 0, 0f, 0f, 0f, 0, "AidBuffLowHp", "sway", "MagicChargeYellow", 0f, "hu", "", "", "");
            config[2010062] = new SkillConfig(2010062, "护卫", "护", "", "250%", "连接", "", 2, 1f, 8f, 0, "", 1f, false, "", 0, 80f, 0f, "", 0, 0f, 0, 2.5f, 0, "", "", "盾", false, 999f, "", 0, 0f, 0f, 0f, 0, "AidBuffLowHp", "sway", "MagicChargeYellow", 0f, "hu", "", "", "");
            config[2010063] = new SkillConfig(2010063, "护卫", "护", "", "300%", "连接", "", 3, 1f, 8f, 0, "", 1f, false, "", 0, 80f, 0f, "", 0, 0f, 0, 3f, 0, "", "", "盾", false, 999f, "", 0, 0f, 0f, 0f, 0, "AidBuffLowHp", "sway", "MagicChargeYellow", 0f, "hu", "", "", "");
            config[2010064] = new SkillConfig(2010064, "护卫", "护", "", "350%", "连接", "", 4, 1f, 8f, 0, "", 1f, false, "", 0, 80f, 0f, "", 0, 0f, 0, 3.5f, 0, "", "", "盾", false, 999f, "", 0, 0f, 0f, 0f, 0, "AidBuffLowHp", "sway", "MagicChargeYellow", 0f, "hu", "", "", "");
            config[2010065] = new SkillConfig(2010065, "护卫", "护", "", "400%", "连接", "", 5, 1f, 8f, 0, "", 1f, false, "", 0, 80f, 0f, "", 0, 0f, 0, 4f, 0, "", "", "盾", false, 999f, "", 0, 0f, 0f, 0f, 0, "AidBuffLowHp", "sway", "MagicChargeYellow", 0f, "hu", "", "", "");
            config[2010066] = new SkillConfig(2010066, "刺甲", "刺", "反弹/1攻击伤害", "20%", "连接", "", 1, 1f, 0f, 0, "", 0f, false, "", 0, 20f, 0f, "", 0, 0.2f, 0, 0f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "DefFeedback", "", "SwordHitBlue", 0f, "ci", "", "", "");
            config[2010067] = new SkillConfig(2010067, "刺甲", "刺", "", "30%", "连接", "", 2, 1f, 0f, 0, "", 0f, false, "", 0, 20f, 0f, "", 0, 0.3f, 0, 0f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "DefFeedback", "", "SwordHitBlue", 0f, "ci", "", "", "");
            config[2010068] = new SkillConfig(2010068, "刺甲", "刺", "", "40%", "连接", "", 3, 1f, 0f, 0, "", 0f, false, "", 0, 20f, 0f, "", 0, 0.4f, 0, 0f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "DefFeedback", "", "SwordHitBlue", 0f, "ci", "", "", "");
            config[2010069] = new SkillConfig(2010069, "刺甲", "刺", "", "55%", "连接", "", 4, 1f, 0f, 0, "", 0f, false, "", 0, 20f, 0f, "", 0, 0.55f, 0, 0f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "DefFeedback", "", "SwordHitBlue", 0f, "ci", "", "", "");
            config[2010070] = new SkillConfig(2010070, "刺甲", "刺", "", "70%", "连接", "", 5, 1f, 0f, 0, "", 0f, false, "", 0, 20f, 0f, "", 0, 0.7f, 0, 0f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "DefFeedback", "", "SwordHitBlue", 0f, "ci", "", "", "");
            config[2010071] = new SkillConfig(2010071, "偷袭", "偷", "战斗开始突袭随机敌方英雄，造成法术强度/1的魔法伤害，/2概率交换位置", "30%;30%", "连接", "", 1, 0.3f, 0f, 0, "", 0f, true, "", 0, 0f, 0f, "", 0, 0f, 0, 0.3f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "InitSneakChangePos", "", "MagicNovaBlue", 0f, "tou", "", "", "");
            config[2010072] = new SkillConfig(2010072, "偷袭", "偷", "", "40%;35%", "连接", "", 2, 0.35f, 0f, 0, "", 0f, true, "", 0, 0f, 0f, "", 0, 0f, 0, 0.4f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "InitSneakChangePos", "", "MagicNovaBlue", 0f, "tou", "", "", "");
            config[2010073] = new SkillConfig(2010073, "偷袭", "偷", "", "60%;40%", "连接", "", 3, 0.4f, 0f, 0, "", 0f, true, "", 0, 0f, 0f, "", 0, 0f, 0, 0.6f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "InitSneakChangePos", "", "MagicNovaBlue", 0f, "tou", "", "", "");
            config[2010074] = new SkillConfig(2010074, "偷袭", "偷", "", "80%;45%", "连接", "", 4, 0.45f, 0f, 0, "", 0f, true, "", 0, 0f, 0f, "", 0, 0f, 0, 0.8f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "InitSneakChangePos", "", "MagicNovaBlue", 0f, "tou", "", "", "");
            config[2010075] = new SkillConfig(2010075, "偷袭", "偷", "", "100%;50%", "连接", "", 5, 0.5f, 0f, 0, "", 0f, true, "", 0, 0f, 0f, "", 0, 0f, 0, 1f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "InitSneakChangePos", "", "MagicNovaBlue", 0f, "tou", "", "", "");
            config[2010076] = new SkillConfig(2010076, "仁者无敌", "仁", "战斗开始时/1概率获得1个万民书", "50%", "连接", "", 1, 0.5f, 0f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, 0f, 0, 0f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 401012, "InitAddItem", "", "", 0f, "shu", "", "", "");
            config[2010077] = new SkillConfig(2010077, "仁者无敌", "仁", "", "60%", "连接", "", 2, 0.6f, 0f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, 0f, 0, 0f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 401012, "InitAddItem", "", "", 0f, "shu", "", "", "");
            config[2010078] = new SkillConfig(2010078, "仁者无敌", "仁", "", "70%", "连接", "", 3, 0.7f, 0f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, 0f, 0, 0f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 401012, "InitAddItem", "", "", 0f, "shu", "", "", "");
            config[2010079] = new SkillConfig(2010079, "仁者无敌", "仁", "", "80%", "连接", "", 4, 0.8f, 0f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, 0f, 0, 0f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 401012, "InitAddItem", "", "", 0f, "shu", "", "", "");
            config[2010080] = new SkillConfig(2010080, "仁者无敌", "仁", "", "90%", "连接", "", 5, 0.9f, 0f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, 0f, 0, 0f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 401012, "InitAddItem", "", "", 0f, "shu", "", "", "");
            config[2010081] = new SkillConfig(2010081, "风华", "华", "攻击目标时造成/1额外魔法伤害", "20%", "连接", "", 1, 1f, 0f, 0, "", 0f, true, "", 0, 0f, 0f, "", 0, 0f, 0, 0.2f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AttackMagicDamage", "", "", 0f, "hua", "", "", "");
            config[2010082] = new SkillConfig(2010082, "风华", "华", "", "30%", "连接", "", 2, 1f, 0f, 0, "", 0f, true, "", 0, 0f, 0f, "", 0, 0f, 0, 0.3f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AttackMagicDamage", "", "", 0f, "hua", "", "", "");
            config[2010083] = new SkillConfig(2010083, "风华", "华", "", "40%", "连接", "", 3, 1f, 0f, 0, "", 0f, true, "", 0, 0f, 0f, "", 0, 0f, 0, 0.4f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AttackMagicDamage", "", "", 0f, "hua", "", "", "");
            config[2010084] = new SkillConfig(2010084, "风华", "华", "", "45%", "连接", "", 4, 1f, 0f, 0, "", 0f, true, "", 0, 0f, 0f, "", 0, 0f, 0, 0.45f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AttackMagicDamage", "", "", 0f, "hua", "", "", "");
            config[2010085] = new SkillConfig(2010085, "风华", "华", "", "50%", "连接", "", 5, 1f, 0f, 0, "", 0f, true, "", 0, 0f, 0f, "", 0, 0f, 0, 0.5f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AttackMagicDamage", "", "", 0f, "hua", "", "", "");
            config[2010091] = new SkillConfig(2010091, "生财", "济", "每回合额外获得/1金币", "+2", "连接", "", 1, 0f, 0f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, 0f, 0, 0f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "Dumb", "", "", 0f, "ji2", "", "", "");
            config[2010092] = new SkillConfig(2010092, "生财", "济", "", "+3", "连接", "", 2, 0f, 0f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, 0f, 0, 0f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "Dumb", "", "", 0f, "ji2", "", "", "");
            config[2010093] = new SkillConfig(2010093, "生财", "济", "", "+4", "连接", "", 3, 0f, 0f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, 0f, 0, 0f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "Dumb", "", "", 0f, "ji2", "", "", "");
            config[2010094] = new SkillConfig(2010094, "生财", "济", "", "+5", "连接", "", 4, 0f, 0f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, 0f, 0, 0f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "Dumb", "", "", 0f, "ji2", "", "", "");
            config[2010095] = new SkillConfig(2010095, "生财", "济", "", "+6", "连接", "", 5, 0f, 0f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, 0f, 0, 0f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "Dumb", "", "", 0f, "ji2", "", "", "");
            config[2010096] = new SkillConfig(2010096, "弄权跋扈", "奸", "战斗开始时对随机1名敌方英雄施加增伤，其受到的伤害/1，持续/2", "+30%;10s", "连接", "", 1, 0f, 0f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, 0.30f, 0, 0f, 0, "", "", "伤", false, 10f, "", 0, 0f, 0f, 0f, 0, "InitEnemyRandomBuff", "", "", 0f, "jian2", "", "", "");
            config[2010097] = new SkillConfig(2010097, "弄权跋扈", "奸", "", "+40%;12s", "连接", "", 2, 0f, 0f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, 0.40f, 0, 0f, 0, "", "", "伤", false, 12f, "", 0, 0f, 0f, 0f, 0, "InitEnemyRandomBuff", "", "", 0f, "jian2", "", "", "");
            config[2010098] = new SkillConfig(2010098, "弄权跋扈", "奸", "", "+50%;15s", "连接", "", 3, 0f, 0f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, 0.50f, 0, 0f, 0, "", "", "伤", false, 15f, "", 0, 0f, 0f, 0f, 0, "InitEnemyRandomBuff", "", "", 0f, "jian2", "", "", "");
            config[2010099] = new SkillConfig(2010099, "弄权跋扈", "奸", "", "+60%;18s", "连接", "", 4, 0f, 0f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, 0.60f, 0, 0f, 0, "", "", "伤", false, 18f, "", 0, 0f, 0f, 0f, 0, "InitEnemyRandomBuff", "", "", 0f, "jian2", "", "", "");
            config[2010100] = new SkillConfig(2010100, "弄权跋扈", "奸", "", "+70%;20s", "连接", "", 5, 0f, 0f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, 0.70f, 0, 0f, 0, "", "", "伤", false, 20f, "", 0, 0f, 0f, 0f, 0, "InitEnemyRandomBuff", "", "", 0f, "jian2", "", "", "");
            config[2010106] = new SkillConfig(2010106, "宝刀未老", "老", "生命低于30%受击时给自己释放攻击/1的护盾，冷却10秒，每次触发永久提升生命回复/2", "240%;+2", "连接", "", 1, 1f, 15f, 0, "hprate<30", 0f, false, "", 0, 0f, 0f, "", 0, 0f, 2, 2.4f, 0, "", "", "盾", false, 999f, "", 0, 0f, 0f, 0f, 0, "AttackedShield", "sway", "MagicChargeYellow", 0f, "lao", "", "", "");
            config[2010107] = new SkillConfig(2010107, "宝刀未老", "老", "", "300%;+3", "连接", "", 2, 1f, 15f, 0, "hprate<30", 0f, false, "", 0, 0f, 0f, "", 0, 0f, 3, 3f, 0, "", "", "盾", false, 999f, "", 0, 0f, 0f, 0f, 0, "AttackedShield", "sway", "MagicChargeYellow", 0f, "lao", "", "", "");
            config[2010108] = new SkillConfig(2010108, "宝刀未老", "老", "", "360%;+4", "连接", "", 3, 1f, 15f, 0, "hprate<30", 0f, false, "", 0, 0f, 0f, "", 0, 0f, 4, 3.6f, 0, "", "", "盾", false, 999f, "", 0, 0f, 0f, 0f, 0, "AttackedShield", "sway", "MagicChargeYellow", 0f, "lao", "", "", "");
            config[2010109] = new SkillConfig(2010109, "宝刀未老", "老", "", "420%;+5", "连接", "", 4, 1f, 15f, 0, "hprate<30", 0f, false, "", 0, 0f, 0f, "", 0, 0f, 5, 4.2f, 0, "", "", "盾", false, 999f, "", 0, 0f, 0f, 0f, 0, "AttackedShield", "sway", "MagicChargeYellow", 0f, "lao", "", "", "");
            config[2010110] = new SkillConfig(2010110, "宝刀未老", "老", "", "480%;+6", "连接", "", 5, 1f, 15f, 0, "hprate<30", 0f, false, "", 0, 0f, 0f, "", 0, 0f, 6, 4.8f, 0, "", "", "盾", false, 999f, "", 0, 0f, 0f, 0f, 0, "AttackedShield", "sway", "MagicChargeYellow", 0f, "lao", "", "", "");
            config[2010111] = new SkillConfig(2010111, "诗书传家", "文", "战斗开始时以/1概率获得道具「文赋」（上一局战败时概率+50%；概率超过100%时必定获得1本，超出部分还有机会再获得1本）", "40%", "连接", "", 1, 0.4f, 0f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, 0f, 0, 0f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 401014, "InitAddItemChance", "", "", 0f, "wen", "", "", "");
            config[2010112] = new SkillConfig(2010112, "诗书传家", "文", "", "48%", "连接", "", 2, 0.48f, 0f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, 0f, 0, 0f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 401014, "InitAddItemChance", "", "", 0f, "wen", "", "", "");
            config[2010113] = new SkillConfig(2010113, "诗书传家", "文", "", "56%", "连接", "", 3, 0.56f, 0f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, 0f, 0, 0f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 401014, "InitAddItemChance", "", "", 0f, "wen", "", "", "");
            config[2010114] = new SkillConfig(2010114, "诗书传家", "文", "", "64%", "连接", "", 4, 0.64f, 0f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, 0f, 0, 0f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 401014, "InitAddItemChance", "", "", 0f, "wen", "", "", "");
            config[2010115] = new SkillConfig(2010115, "诗书传家", "文", "", "72%", "连接", "", 5, 0.72f, 0f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, 0f, 0, 0f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 401014, "InitAddItemChance", "", "", 0f, "wen", "", "", "");
            config[2020001] = new SkillConfig(2020001, "强击", "强", "对目标造成法强/1的魔法伤害", "100%", "技", "", 1, 1f, 0f, 12, "", 0f, true, "", 0, 30f, 0f, "", 1, 0f, 0, 1f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AidSuddenArrow", "sway", "BulletExplosionBlue", 3f, "", "", "", "");
            config[2020002] = new SkillConfig(2020002, "强击", "强", "", "130%", "技", "", 2, 1f, 0f, 12, "", 0f, true, "", 0, 30f, 0f, "", 1, 0f, 0, 1.3f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AidSuddenArrow", "sway", "BulletExplosionBlue", 3f, "", "", "", "");
            config[2020003] = new SkillConfig(2020003, "强击", "强", "", "160%", "技", "", 3, 1f, 0f, 12, "", 0f, true, "", 0, 30f, 0f, "", 1, 0f, 0, 1.6f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AidSuddenArrow", "sway", "BulletExplosionBlue", 3f, "", "", "", "");
            config[2020004] = new SkillConfig(2020004, "强击", "强", "", "200%", "技", "", 4, 1f, 0f, 12, "", 0f, true, "", 0, 30f, 0f, "", 1, 0f, 0, 2f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AidSuddenArrow", "sway", "BulletExplosionBlue", 3f, "", "", "", "");
            config[2020005] = new SkillConfig(2020005, "强击", "强", "", "250%", "技", "", 5, 1f, 0f, 12, "", 0f, true, "", 0, 30f, 0f, "", 1, 0f, 0, 2.5f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AidSuddenArrow", "sway", "BulletExplosionBlue", 3f, "", "", "", "");
            config[2020071] = new SkillConfig(2020071, "速射", "速", "自己和同行[弓弩]箭矢飞行速度提升", "", "攻击up", "", 1, 0f, 0f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, 2.5f, 0, 0f, 1, "速", "弓弩", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "ModifyShootSpeed", "", "", 0f, "", "", "", "");
            config[2020072] = new SkillConfig(2020072, "速射", "速", "", "", "攻击up", "", 2, 0f, 0f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, 2.5f, 0, 0f, 1, "速", "弓弩", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "ModifyShootSpeed", "", "", 0f, "", "", "", "");
            config[2020073] = new SkillConfig(2020073, "速射", "速", "", "", "攻击up", "", 3, 0f, 0f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, 2.5f, 0, 0f, 1, "速", "弓弩", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "ModifyShootSpeed", "", "", 0f, "", "", "", "");
            config[2020074] = new SkillConfig(2020074, "速射", "速", "", "", "攻击up", "", 4, 0f, 0f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, 2.5f, 0, 0f, 1, "速", "弓弩", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "ModifyShootSpeed", "", "", 0f, "", "", "", "");
            config[2020075] = new SkillConfig(2020075, "速射", "速", "", "", "攻击up", "", 5, 0f, 0f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, 2.5f, 0, 0f, 1, "速", "弓弩", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "ModifyShootSpeed", "", "", 0f, "", "", "", "");
            config[2020091] = new SkillConfig(2020091, "共杀", "共", "击中目标时触发2次弹射", "", "", "", 1, 0.35f, 5f, 20, "", 0f, true, "", 0, 30f, 0f, "", 3, 0f, 0, 0.25f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AttackReboundArrow", "flipspin", "", 0f, "", "", "", "");
            config[2020092] = new SkillConfig(2020092, "共杀", "共", "", "", "", "", 2, 0.35f, 5f, 20, "", 0f, true, "", 0, 30f, 0f, "", 3, 0f, 0, 0.25f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AttackReboundArrow", "flipspin", "", 0f, "", "", "", "");
            config[2020093] = new SkillConfig(2020093, "共杀", "共", "", "", "", "", 3, 0.35f, 5f, 20, "", 0f, true, "", 0, 30f, 0f, "", 3, 0f, 0, 0.25f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AttackReboundArrow", "flipspin", "", 0f, "", "", "", "");
            config[2020094] = new SkillConfig(2020094, "共杀", "共", "", "", "", "", 4, 0.35f, 5f, 20, "", 0f, true, "", 0, 30f, 0f, "", 3, 0f, 0, 0.25f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AttackReboundArrow", "flipspin", "", 0f, "", "", "", "");
            config[2020095] = new SkillConfig(2020095, "共杀", "共", "", "", "", "", 5, 0.35f, 5f, 20, "", 0f, true, "", 0, 30f, 0f, "", 3, 0f, 0, 0.25f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AttackReboundArrow", "flipspin", "", 0f, "", "", "", "");
            config[2020101] = new SkillConfig(2020101, "旋风斩", "旋", "攻击时几率对附近敌人造成伤害", "", "技", "", 1, 0.4f, 5f, 20, "", 0f, false, "", 0, 25f, 0f, "", 5, 0f, 0, 0.8f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AttackSpinAttack", "spin", "SwordWhirlwindWhite", 0f, "", "", "", "");
            config[2020102] = new SkillConfig(2020102, "旋风斩", "旋", "", "", "技", "", 2, 0.4f, 5f, 20, "", 0f, false, "", 0, 25f, 0f, "", 5, 0f, 0, 0.8f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AttackSpinAttack", "spin", "SwordWhirlwindWhite", 0f, "", "", "", "");
            config[2020103] = new SkillConfig(2020103, "旋风斩", "旋", "", "", "技", "", 3, 0.4f, 5f, 20, "", 0f, false, "", 0, 25f, 0f, "", 5, 0f, 0, 0.8f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AttackSpinAttack", "spin", "SwordWhirlwindWhite", 0f, "", "", "", "");
            config[2020104] = new SkillConfig(2020104, "旋风斩", "旋", "", "", "技", "", 4, 0.4f, 5f, 20, "", 0f, false, "", 0, 25f, 0f, "", 5, 0f, 0, 0.8f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AttackSpinAttack", "spin", "SwordWhirlwindWhite", 0f, "", "", "", "");
            config[2020105] = new SkillConfig(2020105, "旋风斩", "旋", "", "", "技", "", 5, 0.4f, 5f, 20, "", 0f, false, "", 0, 25f, 0f, "", 5, 0f, 0, 0.8f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AttackSpinAttack", "spin", "SwordWhirlwindWhite", 0f, "", "", "", "");
            config[2020111] = new SkillConfig(2020111, "落雷", "天", "攻击召唤出持续伤害的雷电阵", "", "术", "", 1, 0.3f, 10f, 20, "", 0f, true, "", 0, 0f, 25f, "", 1, 10f, 0, 0.15f, 0, "", "", "", false, 0f, "雷", 0, 5.2f, 0.5f, 0f, 0, "HitRegion", "spin", "SummonStorm", 5f, "", "", "", "");
            config[2020112] = new SkillConfig(2020112, "落雷", "天", "", "", "术", "", 2, 0.3f, 10f, 20, "", 0f, true, "", 0, 0f, 25f, "", 1, 15f, 0, 0.15f, 0, "", "", "", false, 0f, "雷", 0, 5.2f, 0.5f, 0f, 0, "HitRegion", "spin", "SummonStorm", 5f, "", "", "", "");
            config[2020113] = new SkillConfig(2020113, "落雷", "天", "", "", "术", "", 3, 0.3f, 10f, 20, "", 0f, true, "", 0, 0f, 25f, "", 1, 20f, 0, 0.15f, 0, "", "", "", false, 0f, "雷", 0, 5.2f, 0.5f, 0f, 0, "HitRegion", "spin", "SummonStorm", 5f, "", "", "", "");
            config[2020114] = new SkillConfig(2020114, "落雷", "天", "", "", "术", "", 4, 0.3f, 10f, 20, "", 0f, true, "", 0, 0f, 25f, "", 1, 25f, 0, 0.15f, 0, "", "", "", false, 0f, "雷", 0, 5.2f, 0.5f, 0f, 0, "HitRegion", "spin", "SummonStorm", 5f, "", "", "", "");
            config[2020115] = new SkillConfig(2020115, "落雷", "天", "", "", "术", "", 5, 0.3f, 10f, 20, "", 0f, true, "", 0, 0f, 25f, "", 1, 30f, 0, 0.15f, 0, "", "", "", false, 0f, "雷", 0, 5.2f, 0.5f, 0f, 0, "HitRegion", "spin", "SummonStorm", 5f, "", "", "", "");
            config[2020121] = new SkillConfig(2020121, "火计", "钬", "攻击时对目标放火", "", "术", "", 1, 0.3f, 5f, 20, "", 0f, true, "", 0, 0f, 8f, "", 1, 10f, 0, 0.1f, 0, "", "", "", false, 0f, "火", 1, 3.2f, 1f, 0f, 0, "HitWall", "throw", "SoftFireBigRed", 1.6f, "", "", "", "");
            config[2020122] = new SkillConfig(2020122, "火计", "钬", "", "", "术", "", 2, 0.3f, 5f, 20, "", 0f, true, "", 0, 0f, 8f, "", 1, 15f, 0, 0.1f, 0, "", "", "", false, 0f, "火", 1, 3.2f, 1f, 0f, 0, "HitWall", "throw", "SoftFireBigRed", 1.6f, "", "", "", "");
            config[2020123] = new SkillConfig(2020123, "火计", "钬", "", "", "术", "", 3, 0.3f, 5f, 20, "", 0f, true, "", 0, 0f, 8f, "", 1, 20f, 0, 0.1f, 0, "", "", "", false, 0f, "火", 1, 3.2f, 1f, 0f, 0, "HitWall", "throw", "SoftFireBigRed", 1.6f, "", "", "", "");
            config[2020124] = new SkillConfig(2020124, "火计", "钬", "", "", "术", "", 4, 0.3f, 5f, 20, "", 0f, true, "", 0, 0f, 8f, "", 1, 25f, 0, 0.1f, 0, "", "", "", false, 0f, "火", 1, 3.2f, 1f, 0f, 0, "HitWall", "throw", "SoftFireBigRed", 1.6f, "", "", "", "");
            config[2020125] = new SkillConfig(2020125, "火计", "钬", "", "", "术", "", 5, 0.3f, 5f, 20, "", 0f, true, "", 0, 0f, 8f, "", 1, 30f, 0, 0.1f, 0, "", "", "", false, 0f, "火", 1, 3.2f, 1f, 0f, 0, "HitWall", "throw", "SoftFireBigRed", 1.6f, "", "", "", "");
            config[2020131] = new SkillConfig(2020131, "火墙", "炎", "攻击召唤出持续伤害的火墙", "", "术", "", 1, 0.3f, 8.5f, 20, "", 0f, true, "", 0, 0f, 8f, "", 1, 10f, 0, 0.08f, 0, "", "", "", false, 0f, "火", 5, 3.2f, 1f, 0f, 0, "HitWall", "spin", "SoftFireBigRed", 1.6f, "", "", "", "");
            config[2020132] = new SkillConfig(2020132, "火墙", "炎", "", "", "术", "", 2, 0.3f, 8.5f, 20, "", 0f, true, "", 0, 0f, 8f, "", 1, 15f, 0, 0.08f, 0, "", "", "", false, 0f, "火", 5, 3.2f, 1f, 0f, 0, "HitWall", "spin", "SoftFireBigRed", 1.6f, "", "", "", "");
            config[2020133] = new SkillConfig(2020133, "火墙", "炎", "", "", "术", "", 3, 0.3f, 8.5f, 20, "", 0f, true, "", 0, 0f, 8f, "", 1, 20f, 0, 0.08f, 0, "", "", "", false, 0f, "火", 5, 3.2f, 1f, 0f, 0, "HitWall", "spin", "SoftFireBigRed", 1.6f, "", "", "", "");
            config[2020134] = new SkillConfig(2020134, "火墙", "炎", "", "", "术", "", 4, 0.3f, 8.5f, 20, "", 0f, true, "", 0, 0f, 8f, "", 1, 25f, 0, 0.08f, 0, "", "", "", false, 0f, "火", 5, 3.2f, 1f, 0f, 0, "HitWall", "spin", "SoftFireBigRed", 1.6f, "", "", "", "");
            config[2020135] = new SkillConfig(2020135, "火墙", "炎", "", "", "术", "", 5, 0.3f, 8.5f, 20, "", 0f, true, "", 0, 0f, 8f, "", 1, 30f, 0, 0.08f, 0, "", "", "", false, 0f, "火", 5, 3.2f, 1f, 0f, 0, "HitWall", "spin", "SoftFireBigRed", 1.6f, "", "", "", "");
            config[2020141] = new SkillConfig(2020141, "飞斧", "斧", "扔出飞斧攻击前方敌人", "", "技", "", 1, 1f, 6.7f, 20, "", 2f, false, "", 0, 45f, 9f, "", 4, 20f, 0, 0.4f, 0, "", "", "", false, 0f, "武", 0, 1.5f, 0f, 40f, 0, "AidShockWave", "spin", "AxeExplosion", 3f, "", "", "", "");
            config[2020142] = new SkillConfig(2020142, "飞斧", "斧", "", "", "技", "", 2, 1f, 6.7f, 20, "", 2f, false, "", 0, 45f, 9f, "", 4, 30f, 0, 0.4f, 0, "", "", "", false, 0f, "武", 0, 1.5f, 0f, 40f, 0, "AidShockWave", "spin", "AxeExplosion", 3f, "", "", "", "");
            config[2020143] = new SkillConfig(2020143, "飞斧", "斧", "", "", "技", "", 3, 1f, 6.7f, 20, "", 2f, false, "", 0, 45f, 9f, "", 4, 40f, 0, 0.4f, 0, "", "", "", false, 0f, "武", 0, 1.5f, 0f, 40f, 0, "AidShockWave", "spin", "AxeExplosion", 3f, "", "", "", "");
            config[2020144] = new SkillConfig(2020144, "飞斧", "斧", "", "", "技", "", 4, 1f, 6.7f, 20, "", 2f, false, "", 0, 45f, 9f, "", 4, 50f, 0, 0.4f, 0, "", "", "", false, 0f, "武", 0, 1.5f, 0f, 40f, 0, "AidShockWave", "spin", "AxeExplosion", 3f, "", "", "", "");
            config[2020145] = new SkillConfig(2020145, "飞斧", "斧", "", "", "技", "", 5, 1f, 6.7f, 20, "", 2f, false, "", 0, 45f, 9f, "", 4, 60f, 0, 0.4f, 0, "", "", "", false, 0f, "武", 0, 1.5f, 0f, 40f, 0, "AidShockWave", "spin", "AxeExplosion", 3f, "", "", "", "");
            config[2020161] = new SkillConfig(2020161, "惊雷", "雷", "召唤3个惊雷攻击前方敌人", "", "术", "", 1, 1f, 6.7f, 20, "", 2f, true, "", 0, 60f, 11f, "", 4, 15f, 0, 0.25f, 0, "", "", "", false, 0f, "雷", 0, 1.5f, 0f, 40f, 0, "AidShockWave", "spin", "NukeMissileFires", 4f, "", "", "", "");
            config[2020162] = new SkillConfig(2020162, "惊雷", "雷", "", "", "术", "", 2, 1f, 6.7f, 20, "", 2f, true, "", 0, 60f, 11f, "", 4, 20f, 0, 0.25f, 0, "", "", "", false, 0f, "雷", 0, 1.5f, 0f, 40f, 0, "AidShockWave", "spin", "NukeMissileFires", 4f, "", "", "", "");
            config[2020163] = new SkillConfig(2020163, "惊雷", "雷", "", "", "术", "", 3, 1f, 6.7f, 20, "", 2f, true, "", 0, 60f, 11f, "", 4, 25f, 0, 0.25f, 0, "", "", "", false, 0f, "雷", 0, 1.5f, 0f, 40f, 0, "AidShockWave", "spin", "NukeMissileFires", 4f, "", "", "", "");
            config[2020164] = new SkillConfig(2020164, "惊雷", "雷", "", "", "术", "", 4, 1f, 6.7f, 20, "", 2f, true, "", 0, 60f, 11f, "", 4, 30f, 0, 0.25f, 0, "", "", "", false, 0f, "雷", 0, 1.5f, 0f, 40f, 0, "AidShockWave", "spin", "NukeMissileFires", 4f, "", "", "", "");
            config[2020165] = new SkillConfig(2020165, "惊雷", "雷", "", "", "术", "", 5, 1f, 6.7f, 20, "", 2f, true, "", 0, 60f, 11f, "", 4, 35f, 0, 0.25f, 0, "", "", "", false, 0f, "雷", 0, 1.5f, 0f, 40f, 0, "AidShockWave", "spin", "NukeMissileFires", 4f, "", "", "", "");
            config[2020191] = new SkillConfig(2020191, "魔神", "魔", "攻击时回复生命", "", "", "", 1, 0.35f, 6f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, 0f, 0, 1f, 0, "", "", "吸", false, 5f, "", 0, 0f, 0f, 0f, 0, "AttackedBuff", "", "MagicBuffGreen", 0f, "", "", "", "");
            config[2020192] = new SkillConfig(2020192, "魔神", "魔", "", "", "", "", 2, 0.35f, 6f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, 0f, 0, 1f, 0, "", "", "吸", false, 5f, "", 0, 0f, 0f, 0f, 0, "AttackedBuff", "", "MagicBuffGreen", 0f, "", "", "", "");
            config[2020193] = new SkillConfig(2020193, "魔神", "魔", "", "", "", "", 3, 0.35f, 6f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, 0f, 0, 1f, 0, "", "", "吸", false, 5f, "", 0, 0f, 0f, 0f, 0, "AttackedBuff", "", "MagicBuffGreen", 0f, "", "", "", "");
            config[2020194] = new SkillConfig(2020194, "魔神", "魔", "", "", "", "", 4, 0.35f, 6f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, 0f, 0, 1f, 0, "", "", "吸", false, 5f, "", 0, 0f, 0f, 0f, 0, "AttackedBuff", "", "MagicBuffGreen", 0f, "", "", "", "");
            config[2020195] = new SkillConfig(2020195, "魔神", "魔", "", "", "", "", 5, 0.35f, 6f, 0, "", 0f, false, "", 0, 0f, 0f, "", 0, 0f, 0, 1f, 0, "", "", "吸", false, 5f, "", 0, 0f, 0f, 0f, 0, "AttackedBuff", "", "MagicBuffGreen", 0f, "", "", "", "");
            config[2020201] = new SkillConfig(2020201, "埋伏", "伏", "被攻击时瞬移到远程攻击者附近", "", "", "", 1, 1f, 6f, 0, "", 0f, false, "", 0, 30f, 0f, "", 0, 0f, 0, 0f, 0, "", "", "乱", false, 1f, "", 0, 0f, 0f, 0f, 0, "HitTeleport", "saw", "MagicNovaBlue", 0f, "", "", "", "");
            config[2020202] = new SkillConfig(2020202, "埋伏", "伏", "", "", "", "", 2, 1f, 6f, 0, "", 0f, false, "", 0, 30f, 0f, "", 0, 0f, 0, 0f, 0, "", "", "乱", false, 1f, "", 0, 0f, 0f, 0f, 0, "HitTeleport", "saw", "MagicNovaBlue", 0f, "", "", "", "");
            config[2020203] = new SkillConfig(2020203, "埋伏", "伏", "", "", "", "", 3, 1f, 6f, 0, "", 0f, false, "", 0, 30f, 0f, "", 0, 0f, 0, 0f, 0, "", "", "乱", false, 1f, "", 0, 0f, 0f, 0f, 0, "HitTeleport", "saw", "MagicNovaBlue", 0f, "", "", "", "");
            config[2020204] = new SkillConfig(2020204, "埋伏", "伏", "", "", "", "", 4, 1f, 6f, 0, "", 0f, false, "", 0, 30f, 0f, "", 0, 0f, 0, 0f, 0, "", "", "乱", false, 1f, "", 0, 0f, 0f, 0f, 0, "HitTeleport", "saw", "MagicNovaBlue", 0f, "", "", "", "");
            config[2020205] = new SkillConfig(2020205, "埋伏", "伏", "", "", "", "", 5, 1f, 6f, 0, "", 0f, false, "", 0, 30f, 0f, "", 0, 0f, 0, 0f, 0, "", "", "乱", false, 1f, "", 0, 0f, 0f, 0f, 0, "HitTeleport", "saw", "MagicNovaBlue", 0f, "", "", "", "");
            config[2020211] = new SkillConfig(2020211, "火矢", "矢", "攻击时射出火箭", "", "技", "", 1, 0.35f, 3f, 20, "", 0f, false, "", 0, 0f, 8f, "", 1, 10f, 0, 0.12f, 0, "", "", "", false, 0f, "火", 1, 3.2f, 1f, 0f, 0, "HitWall", "throw", "SoftFireBigRed", 1.6f, "", "", "", "");
            config[2020212] = new SkillConfig(2020212, "火矢", "矢", "", "", "技", "", 2, 0.35f, 3f, 20, "", 0f, false, "", 0, 0f, 8f, "", 1, 15f, 0, 0.12f, 0, "", "", "", false, 0f, "火", 1, 3.2f, 1f, 0f, 0, "HitWall", "throw", "SoftFireBigRed", 1.6f, "", "", "", "");
            config[2020213] = new SkillConfig(2020213, "火矢", "矢", "", "", "技", "", 3, 0.35f, 3f, 20, "", 0f, false, "", 0, 0f, 8f, "", 1, 20f, 0, 0.12f, 0, "", "", "", false, 0f, "火", 1, 3.2f, 1f, 0f, 0, "HitWall", "throw", "SoftFireBigRed", 1.6f, "", "", "", "");
            config[2020214] = new SkillConfig(2020214, "火矢", "矢", "", "", "技", "", 4, 0.35f, 3f, 20, "", 0f, false, "", 0, 0f, 8f, "", 1, 25f, 0, 0.12f, 0, "", "", "", false, 0f, "火", 1, 3.2f, 1f, 0f, 0, "HitWall", "throw", "SoftFireBigRed", 1.6f, "", "", "", "");
            config[2020215] = new SkillConfig(2020215, "火矢", "矢", "", "", "技", "", 5, 0.35f, 3f, 20, "", 0f, false, "", 0, 0f, 8f, "", 1, 30f, 0, 0.12f, 0, "", "", "", false, 0f, "火", 1, 3.2f, 1f, 0f, 0, "HitWall", "throw", "SoftFireBigRed", 1.6f, "", "", "", "");
            config[2030021] = new SkillConfig(2030021, "连锁", "锁", "锁定敌人目标,传递一半受到伤害", "", "术", "", 1, 0.5f, 3f, 20, "", 0f, true, "", 0, 25f, 0f, "targetUnit", 1, 0f, 0, 0.5f, 0, "", "", "锁", false, 8f, "", 0, 0f, 0f, 0f, 0, "HitBuffArea", "throw", "MagicNovaYellow", 0f, "", "", "", "");
            config[2030022] = new SkillConfig(2030022, "连锁", "锁", "", "", "术", "", 2, 0.5f, 3f, 20, "", 0f, true, "", 0, 25f, 0f, "targetUnit", 1, 0f, 0, 0.5f, 0, "", "", "锁", false, 8f, "", 0, 0f, 0f, 0f, 0, "HitBuffArea", "throw", "MagicNovaYellow", 0f, "", "", "", "");
            config[2030023] = new SkillConfig(2030023, "连锁", "锁", "", "", "术", "", 3, 0.5f, 3f, 20, "", 0f, true, "", 0, 25f, 0f, "targetUnit", 1, 0f, 0, 0.5f, 0, "", "", "锁", false, 8f, "", 0, 0f, 0f, 0f, 0, "HitBuffArea", "throw", "MagicNovaYellow", 0f, "", "", "", "");
            config[2030024] = new SkillConfig(2030024, "连锁", "锁", "", "", "术", "", 4, 0.5f, 3f, 20, "", 0f, true, "", 0, 25f, 0f, "targetUnit", 1, 0f, 0, 0.5f, 0, "", "", "锁", false, 8f, "", 0, 0f, 0f, 0f, 0, "HitBuffArea", "throw", "MagicNovaYellow", 0f, "", "", "", "");
            config[2030025] = new SkillConfig(2030025, "连锁", "锁", "", "", "术", "", 5, 0.5f, 3f, 20, "", 0f, true, "", 0, 25f, 0f, "targetUnit", 1, 0f, 0, 0.5f, 0, "", "", "锁", false, 8f, "", 0, 0f, 0f, 0f, 0, "HitBuffArea", "throw", "MagicNovaYellow", 0f, "", "", "", "");
            config[2030041] = new SkillConfig(2030041, "威震", "威", "攻击时混乱周围目标", "", "", "", 1, 0.2f, 5f, 20, "", 0f, false, "", 0, 20f, 0f, "castUnit", 3, 0f, 0, 0f, 0, "", "", "乱", false, 1.5f, "", 0, 0f, 0f, 0f, 0, "HitBuffArea", "spin", "MagicNovaYellow", 0f, "", "", "", "");
            config[2030042] = new SkillConfig(2030042, "威震", "威", "", "", "", "", 2, 0.2f, 5f, 20, "", 0f, false, "", 0, 20f, 0f, "castUnit", 3, 0f, 0, 0f, 0, "", "", "乱", false, 1.5f, "", 0, 0f, 0f, 0f, 0, "HitBuffArea", "spin", "MagicNovaYellow", 0f, "", "", "", "");
            config[2030043] = new SkillConfig(2030043, "威震", "威", "", "", "", "", 3, 0.2f, 5f, 20, "", 0f, false, "", 0, 20f, 0f, "castUnit", 3, 0f, 0, 0f, 0, "", "", "乱", false, 1.5f, "", 0, 0f, 0f, 0f, 0, "HitBuffArea", "spin", "MagicNovaYellow", 0f, "", "", "", "");
            config[2030044] = new SkillConfig(2030044, "威震", "威", "", "", "", "", 4, 0.2f, 5f, 20, "", 0f, false, "", 0, 20f, 0f, "castUnit", 3, 0f, 0, 0f, 0, "", "", "乱", false, 1.5f, "", 0, 0f, 0f, 0f, 0, "HitBuffArea", "spin", "MagicNovaYellow", 0f, "", "", "", "");
            config[2030045] = new SkillConfig(2030045, "威震", "威", "", "", "", "", 5, 0.2f, 5f, 20, "", 0f, false, "", 0, 20f, 0f, "castUnit", 3, 0f, 0, 0f, 0, "", "", "乱", false, 1.5f, "", 0, 0f, 0f, 0f, 0, "HitBuffArea", "spin", "MagicNovaYellow", 0f, "", "", "", "");
            config[2030051] = new SkillConfig(2030051, "击破", "泼", "攻击几率使目标增伤40%", "", "", "", 1, 0.4f, 2f, 20, "", 0f, false, "", 0, 0f, 0f, "", 0, 0.4f, 0, 0f, 0, "", "", "伤", false, 3f, "", 0, 0f, 0f, 0f, 0, "HitBuff", "jump", "SoftFireBigRed", 0f, "", "", "", "");
            config[2030052] = new SkillConfig(2030052, "击破", "泼", "", "", "", "", 2, 0.4f, 2f, 20, "", 0f, false, "", 0, 0f, 0f, "", 0, 0.4f, 0, 0f, 0, "", "", "伤", false, 3f, "", 0, 0f, 0f, 0f, 0, "HitBuff", "jump", "SoftFireBigRed", 0f, "", "", "", "");
            config[2030053] = new SkillConfig(2030053, "击破", "泼", "", "", "", "", 3, 0.4f, 2f, 20, "", 0f, false, "", 0, 0f, 0f, "", 0, 0.4f, 0, 0f, 0, "", "", "伤", false, 3f, "", 0, 0f, 0f, 0f, 0, "HitBuff", "jump", "SoftFireBigRed", 0f, "", "", "", "");
            config[2030054] = new SkillConfig(2030054, "击破", "泼", "", "", "", "", 4, 0.4f, 2f, 20, "", 0f, false, "", 0, 0f, 0f, "", 0, 0.4f, 0, 0f, 0, "", "", "伤", false, 3f, "", 0, 0f, 0f, 0f, 0, "HitBuff", "jump", "SoftFireBigRed", 0f, "", "", "", "");
            config[2030055] = new SkillConfig(2030055, "击破", "泼", "", "", "", "", 5, 0.4f, 2f, 20, "", 0f, false, "", 0, 0f, 0f, "", 0, 0.4f, 0, 0f, 0, "", "", "伤", false, 3f, "", 0, 0f, 0f, 0f, 0, "HitBuff", "jump", "SoftFireBigRed", 0f, "", "", "", "");
            config[2030061] = new SkillConfig(2030061, "延缓", "缓", "攻击几率使目标减速30%", "", "", "", 1, 0.4f, 3f, 20, "", 0f, true, "", 0, 0f, 0f, "", 0, 0.3f, 0, 0f, 0, "", "", "慢", false, 5f, "", 0, 0f, 0f, 0f, 0, "HitBuff", "sway", "MagicNovaYellow", 0f, "", "", "", "");
            config[2030062] = new SkillConfig(2030062, "延缓", "缓", "", "", "", "", 2, 0.4f, 3f, 20, "", 0f, true, "", 0, 0f, 0f, "", 0, 0.3f, 0, 0f, 0, "", "", "慢", false, 5f, "", 0, 0f, 0f, 0f, 0, "HitBuff", "sway", "MagicNovaYellow", 0f, "", "", "", "");
            config[2030063] = new SkillConfig(2030063, "延缓", "缓", "", "", "", "", 3, 0.4f, 3f, 20, "", 0f, true, "", 0, 0f, 0f, "", 0, 0.3f, 0, 0f, 0, "", "", "慢", false, 5f, "", 0, 0f, 0f, 0f, 0, "HitBuff", "sway", "MagicNovaYellow", 0f, "", "", "", "");
            config[2030064] = new SkillConfig(2030064, "延缓", "缓", "", "", "", "", 4, 0.4f, 3f, 20, "", 0f, true, "", 0, 0f, 0f, "", 0, 0.3f, 0, 0f, 0, "", "", "慢", false, 5f, "", 0, 0f, 0f, 0f, 0, "HitBuff", "sway", "MagicNovaYellow", 0f, "", "", "", "");
            config[2030065] = new SkillConfig(2030065, "延缓", "缓", "", "", "", "", 5, 0.4f, 3f, 20, "", 0f, true, "", 0, 0f, 0f, "", 0, 0.3f, 0, 0f, 0, "", "", "慢", false, 5f, "", 0, 0f, 0f, 0f, 0, "HitBuff", "sway", "MagicNovaYellow", 0f, "", "", "", "");
            config[2030071] = new SkillConfig(2030071, "陷阵", "陷", "攻击几率使目标陷阵", "", "", "", 1, 0.4f, 3f, 20, "", 0f, true, "", 0, 0f, 0f, "", 0, 0f, 0, 0f, 0, "", "", "停", false, 4f, "", 0, 0f, 0f, 0f, 0, "HitBuff", "sway", "MagicNovaYellow", 0f, "", "", "", "");
            config[2030072] = new SkillConfig(2030072, "陷阵", "陷", "", "", "", "", 2, 0.4f, 3f, 20, "", 0f, true, "", 0, 0f, 0f, "", 0, 0f, 0, 0f, 0, "", "", "停", false, 4f, "", 0, 0f, 0f, 0f, 0, "HitBuff", "sway", "MagicNovaYellow", 0f, "", "", "", "");
            config[2030073] = new SkillConfig(2030073, "陷阵", "陷", "", "", "", "", 3, 0.4f, 3f, 20, "", 0f, true, "", 0, 0f, 0f, "", 0, 0f, 0, 0f, 0, "", "", "停", false, 4f, "", 0, 0f, 0f, 0f, 0, "HitBuff", "sway", "MagicNovaYellow", 0f, "", "", "", "");
            config[2030074] = new SkillConfig(2030074, "陷阵", "陷", "", "", "", "", 4, 0.4f, 3f, 20, "", 0f, true, "", 0, 0f, 0f, "", 0, 0f, 0, 0f, 0, "", "", "停", false, 4f, "", 0, 0f, 0f, 0f, 0, "HitBuff", "sway", "MagicNovaYellow", 0f, "", "", "", "");
            config[2030075] = new SkillConfig(2030075, "陷阵", "陷", "", "", "", "", 5, 0.4f, 3f, 20, "", 0f, true, "", 0, 0f, 0f, "", 0, 0f, 0, 0f, 0, "", "", "停", false, 4f, "", 0, 0f, 0f, 0f, 0, "HitBuff", "sway", "MagicNovaYellow", 0f, "", "", "", "");
            config[2030081] = new SkillConfig(2030081, "溃散", "溃", "攻击几率使目标溃败", "", "", "", 1, 0.4f, 4f, 20, "", 0f, true, "", 0, 0f, 0f, "", 0, 0f, 0, 0.1f, 0, "", "", "败", false, 5.2f, "", 0, 0f, 0f, 0f, 0, "HitBuff", "sway", "MagicNovaYellow", 0f, "", "", "", "");
            config[2030082] = new SkillConfig(2030082, "溃散", "溃", "", "", "", "", 2, 0.4f, 4f, 20, "", 0f, true, "", 0, 0f, 0f, "", 0, 0f, 0, 0.1f, 0, "", "", "败", false, 5.2f, "", 0, 0f, 0f, 0f, 0, "HitBuff", "sway", "MagicNovaYellow", 0f, "", "", "", "");
            config[2030083] = new SkillConfig(2030083, "溃散", "溃", "", "", "", "", 3, 0.4f, 4f, 20, "", 0f, true, "", 0, 0f, 0f, "", 0, 0f, 0, 0.1f, 0, "", "", "败", false, 5.2f, "", 0, 0f, 0f, 0f, 0, "HitBuff", "sway", "MagicNovaYellow", 0f, "", "", "", "");
            config[2030084] = new SkillConfig(2030084, "溃散", "溃", "", "", "", "", 4, 0.4f, 4f, 20, "", 0f, true, "", 0, 0f, 0f, "", 0, 0f, 0, 0.1f, 0, "", "", "败", false, 5.2f, "", 0, 0f, 0f, 0f, 0, "HitBuff", "sway", "MagicNovaYellow", 0f, "", "", "", "");
            config[2030085] = new SkillConfig(2030085, "溃散", "溃", "", "", "", "", 5, 0.4f, 4f, 20, "", 0f, true, "", 0, 0f, 0f, "", 0, 0f, 0, 0.1f, 0, "", "", "败", false, 5.2f, "", 0, 0f, 0f, 0f, 0, "HitBuff", "sway", "MagicNovaYellow", 0f, "", "", "", "");
            config[2030091] = new SkillConfig(2030091, "分兵", "分", "被攻击时产生一只有伤害部队", "", "", "", 1, 0.4f, 15f, 0, "", 0f, false, "", 0, 15f, 0f, "", 0, 0f, 4, 0.5f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AttackedShadow", "sway", "MagicFieldGreen", 0f, "", "", "", "");
            config[2030092] = new SkillConfig(2030092, "分兵", "分", "", "", "", "", 2, 0.4f, 15f, 0, "", 0f, false, "", 0, 15f, 0f, "", 0, 0f, 4, 0.5f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AttackedShadow", "sway", "MagicFieldGreen", 0f, "", "", "", "");
            config[2030093] = new SkillConfig(2030093, "分兵", "分", "", "", "", "", 3, 0.4f, 15f, 0, "", 0f, false, "", 0, 15f, 0f, "", 0, 0f, 4, 0.5f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AttackedShadow", "sway", "MagicFieldGreen", 0f, "", "", "", "");
            config[2030094] = new SkillConfig(2030094, "分兵", "分", "", "", "", "", 4, 0.4f, 15f, 0, "", 0f, false, "", 0, 15f, 0f, "", 0, 0f, 4, 0.5f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AttackedShadow", "sway", "MagicFieldGreen", 0f, "", "", "", "");
            config[2030095] = new SkillConfig(2030095, "分兵", "分", "", "", "", "", 5, 0.4f, 15f, 0, "", 0f, false, "", 0, 15f, 0f, "", 0, 0f, 4, 0.5f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AttackedShadow", "sway", "MagicFieldGreen", 0f, "", "", "", "");
            config[2080011] = new SkillConfig(2080011, "百出", "百", "降低自己和同行[扇谋相]技能CD时间", "", "智技up", "", 1, 0f, 0f, 0, "", 0f, true, "", 2, 0f, 0f, "", 0, 0.3f, 0, 0f, 1, "白", "扇谋相", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "ModifySkillRateTime", "", "", 0f, "", "", "", "");
            config[2080012] = new SkillConfig(2080012, "百出", "百", "", "", "智技up", "", 2, 0f, 0f, 0, "", 0f, true, "", 2, 0f, 0f, "", 0, 0.3f, 0, 0f, 1, "白", "扇谋相", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "ModifySkillRateTime", "", "", 0f, "", "", "", "");
            config[2080013] = new SkillConfig(2080013, "百出", "百", "", "", "智技up", "", 3, 0f, 0f, 0, "", 0f, true, "", 2, 0f, 0f, "", 0, 0.3f, 0, 0f, 1, "白", "扇谋相", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "ModifySkillRateTime", "", "", 0f, "", "", "", "");
            config[2080014] = new SkillConfig(2080014, "百出", "百", "", "", "智技up", "", 4, 0f, 0f, 0, "", 0f, true, "", 2, 0f, 0f, "", 0, 0.3f, 0, 0f, 1, "白", "扇谋相", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "ModifySkillRateTime", "", "", 0f, "", "", "", "");
            config[2080015] = new SkillConfig(2080015, "百出", "百", "", "", "智技up", "", 5, 0f, 0f, 0, "", 0f, true, "", 2, 0f, 0f, "", 0, 0.3f, 0, 0f, 1, "白", "扇谋相", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "ModifySkillRateTime", "", "", 0f, "", "", "", "");
            config[2080021] = new SkillConfig(2080021, "百出小", "白", "降低技能CD时间", "", "智技up", "", 1, 0f, 0f, 0, "", 0f, true, "", 2, 0f, 0f, "", 0, 0.4f, 0, 0f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "ModifySkillRateTime", "", "", 0f, "", "", "", "");
            config[2080022] = new SkillConfig(2080022, "百出小", "白", "", "", "智技up", "", 2, 0f, 0f, 0, "", 0f, true, "", 2, 0f, 0f, "", 0, 0.4f, 0, 0f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "ModifySkillRateTime", "", "", 0f, "", "", "", "");
            config[2080023] = new SkillConfig(2080023, "百出小", "白", "", "", "智技up", "", 3, 0f, 0f, 0, "", 0f, true, "", 2, 0f, 0f, "", 0, 0.4f, 0, 0f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "ModifySkillRateTime", "", "", 0f, "", "", "", "");
            config[2080024] = new SkillConfig(2080024, "百出小", "白", "", "", "智技up", "", 4, 0f, 0f, 0, "", 0f, true, "", 2, 0f, 0f, "", 0, 0.4f, 0, 0f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "ModifySkillRateTime", "", "", 0f, "", "", "", "");
            config[2080025] = new SkillConfig(2080025, "百出小", "白", "", "", "智技up", "", 5, 0f, 0f, 0, "", 0f, true, "", 2, 0f, 0f, "", 0, 0.4f, 0, 0f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "ModifySkillRateTime", "", "", 0f, "", "", "", "");
            config[2080031] = new SkillConfig(2080031, "神算", "神", "提升技能命中率和持续时间", "", "智技up", "", 1, 0.3f, 0f, 0, "", 0f, true, "", 2, 0f, 0f, "", 0, 0.5f, 0, 0f, 0, "", "", "", false, 0.5f, "", 0, 0f, 0f, 0f, 0, "ModifySkillRateTime", "", "", 0f, "", "", "", "");
            config[2080032] = new SkillConfig(2080032, "神算", "神", "", "", "智技up", "", 2, 0.3f, 0f, 0, "", 0f, true, "", 2, 0f, 0f, "", 0, 0.5f, 0, 0f, 0, "", "", "", false, 0.5f, "", 0, 0f, 0f, 0f, 0, "ModifySkillRateTime", "", "", 0f, "", "", "", "");
            config[2080033] = new SkillConfig(2080033, "神算", "神", "", "", "智技up", "", 3, 0.3f, 0f, 0, "", 0f, true, "", 2, 0f, 0f, "", 0, 0.5f, 0, 0f, 0, "", "", "", false, 0.5f, "", 0, 0f, 0f, 0f, 0, "ModifySkillRateTime", "", "", 0f, "", "", "", "");
            config[2080034] = new SkillConfig(2080034, "神算", "神", "", "", "智技up", "", 4, 0.3f, 0f, 0, "", 0f, true, "", 2, 0f, 0f, "", 0, 0.5f, 0, 0f, 0, "", "", "", false, 0.5f, "", 0, 0f, 0f, 0f, 0, "ModifySkillRateTime", "", "", 0f, "", "", "", "");
            config[2080035] = new SkillConfig(2080035, "神算", "神", "", "", "智技up", "", 5, 0.3f, 0f, 0, "", 0f, true, "", 2, 0f, 0f, "", 0, 0.5f, 0, 0f, 0, "", "", "", false, 0.5f, "", 0, 0f, 0f, 0f, 0, "ModifySkillRateTime", "", "", 0f, "", "", "", "");
            config[2080041] = new SkillConfig(2080041, "蔓延", "延", "自己和同行[扇谋相]技能负面状态扩散", "", "智技up", "", 1, 0.5f, 3f, 0, "", 0f, true, "", 2, 30f, 0f, "", 2, 0f, 0, 0f, 1, "筵", "扇谋相", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "BuffExpand", "", "", 0f, "", "", "", "");
            config[2080042] = new SkillConfig(2080042, "蔓延", "延", "", "", "智技up", "", 2, 0.5f, 3f, 0, "", 0f, true, "", 2, 30f, 0f, "", 2, 0f, 0, 0f, 1, "筵", "扇谋相", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "BuffExpand", "", "", 0f, "", "", "", "");
            config[2080043] = new SkillConfig(2080043, "蔓延", "延", "", "", "智技up", "", 3, 0.5f, 3f, 0, "", 0f, true, "", 2, 30f, 0f, "", 2, 0f, 0, 0f, 1, "筵", "扇谋相", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "BuffExpand", "", "", 0f, "", "", "", "");
            config[2080044] = new SkillConfig(2080044, "蔓延", "延", "", "", "智技up", "", 4, 0.5f, 3f, 0, "", 0f, true, "", 2, 30f, 0f, "", 2, 0f, 0, 0f, 1, "筵", "扇谋相", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "BuffExpand", "", "", 0f, "", "", "", "");
            config[2080045] = new SkillConfig(2080045, "蔓延", "延", "", "", "智技up", "", 5, 0.5f, 3f, 0, "", 0f, true, "", 2, 30f, 0f, "", 2, 0f, 0, 0f, 1, "筵", "扇谋相", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "BuffExpand", "", "", 0f, "", "", "", "");
            config[2080051] = new SkillConfig(2080051, "蔓延小", "筵", "技能负面状态概率扩散", "", "智技up", "", 1, 0.5f, 3f, 0, "", 0f, true, "", 2, 30f, 0f, "", 2, 0f, 0, 0f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "BuffExpand", "", "", 0f, "", "", "", "");
            config[2080052] = new SkillConfig(2080052, "蔓延小", "筵", "", "", "智技up", "", 2, 0.5f, 3f, 0, "", 0f, true, "", 2, 30f, 0f, "", 2, 0f, 0, 0f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "BuffExpand", "", "", 0f, "", "", "", "");
            config[2080053] = new SkillConfig(2080053, "蔓延小", "筵", "", "", "智技up", "", 3, 0.5f, 3f, 0, "", 0f, true, "", 2, 30f, 0f, "", 2, 0f, 0, 0f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "BuffExpand", "", "", 0f, "", "", "", "");
            config[2080054] = new SkillConfig(2080054, "蔓延小", "筵", "", "", "智技up", "", 4, 0.5f, 3f, 0, "", 0f, true, "", 2, 30f, 0f, "", 2, 0f, 0, 0f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "BuffExpand", "", "", 0f, "", "", "", "");
            config[2080055] = new SkillConfig(2080055, "蔓延小", "筵", "", "", "智技up", "", 5, 0.5f, 3f, 0, "", 0f, true, "", 2, 30f, 0f, "", 2, 0f, 0, 0f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "BuffExpand", "", "", 0f, "", "", "", "");
            config[2080061] = new SkillConfig(2080061, "同调", "调", "自己和同行[鼓乐医]技能正面状态扩散", "", "智技up", "", 1, 0.5f, 3f, 0, "", 0f, true, "", 2, 30f, 0f, "", 2, 0f, 0, 0f, 1, "碉", "鼓乐医", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "BuffExpandPos", "", "", 0f, "", "", "", "");
            config[2080062] = new SkillConfig(2080062, "同调", "调", "", "", "智技up", "", 2, 0.5f, 3f, 0, "", 0f, true, "", 2, 30f, 0f, "", 2, 0f, 0, 0f, 1, "碉", "鼓乐医", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "BuffExpandPos", "", "", 0f, "", "", "", "");
            config[2080063] = new SkillConfig(2080063, "同调", "调", "", "", "智技up", "", 3, 0.5f, 3f, 0, "", 0f, true, "", 2, 30f, 0f, "", 2, 0f, 0, 0f, 1, "碉", "鼓乐医", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "BuffExpandPos", "", "", 0f, "", "", "", "");
            config[2080064] = new SkillConfig(2080064, "同调", "调", "", "", "智技up", "", 4, 0.5f, 3f, 0, "", 0f, true, "", 2, 30f, 0f, "", 2, 0f, 0, 0f, 1, "碉", "鼓乐医", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "BuffExpandPos", "", "", 0f, "", "", "", "");
            config[2080065] = new SkillConfig(2080065, "同调", "调", "", "", "智技up", "", 5, 0.5f, 3f, 0, "", 0f, true, "", 2, 30f, 0f, "", 2, 0f, 0, 0f, 1, "碉", "鼓乐医", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "BuffExpandPos", "", "", 0f, "", "", "", "");
            config[2080071] = new SkillConfig(2080071, "同调小", "碉", "技能正面状态扩散", "", "智技up", "", 1, 0.5f, 3f, 0, "", 0f, true, "", 2, 30f, 0f, "", 2, 0f, 0, 0f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "BuffExpandPos", "", "", 0f, "", "", "", "");
            config[2080072] = new SkillConfig(2080072, "同调小", "碉", "", "", "智技up", "", 2, 0.5f, 3f, 0, "", 0f, true, "", 2, 30f, 0f, "", 2, 0f, 0, 0f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "BuffExpandPos", "", "", 0f, "", "", "", "");
            config[2080073] = new SkillConfig(2080073, "同调小", "碉", "", "", "智技up", "", 3, 0.5f, 3f, 0, "", 0f, true, "", 2, 30f, 0f, "", 2, 0f, 0, 0f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "BuffExpandPos", "", "", 0f, "", "", "", "");
            config[2080074] = new SkillConfig(2080074, "同调小", "碉", "", "", "智技up", "", 4, 0.5f, 3f, 0, "", 0f, true, "", 2, 30f, 0f, "", 2, 0f, 0, 0f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "BuffExpandPos", "", "", 0f, "", "", "", "");
            config[2080075] = new SkillConfig(2080075, "同调小", "碉", "", "", "智技up", "", 5, 0.5f, 3f, 0, "", 0f, true, "", 2, 30f, 0f, "", 2, 0f, 0, 0f, 0, "", "", "", false, 0f, "", 0, 0f, 0f, 0f, 0, "BuffExpandPos", "", "", 0f, "", "", "", "");
            config[2080081] = new SkillConfig(2080081, "炽热", "炽", "提升本方火焰持续时间", "", "智技up", "", 1, 0f, 0f, 0, "", 0f, true, "", 0, 0f, 0f, "", 0, 0f, 0, 0f, 3, "炽", "", "", false, 0f, "火", 0, 3f, 0f, 0f, 0, "Dumb", "", "", 0f, "", "", "", "");
            config[2080082] = new SkillConfig(2080082, "炽热", "炽", "", "", "智技up", "", 2, 0f, 0f, 0, "", 0f, true, "", 0, 0f, 0f, "", 0, 0f, 0, 0f, 3, "炽", "", "", false, 0f, "火", 0, 3f, 0f, 0f, 0, "Dumb", "", "", 0f, "", "", "", "");
            config[2080083] = new SkillConfig(2080083, "炽热", "炽", "", "", "智技up", "", 3, 0f, 0f, 0, "", 0f, true, "", 0, 0f, 0f, "", 0, 0f, 0, 0f, 3, "炽", "", "", false, 0f, "火", 0, 3f, 0f, 0f, 0, "Dumb", "", "", 0f, "", "", "", "");
            config[2080084] = new SkillConfig(2080084, "炽热", "炽", "", "", "智技up", "", 4, 0f, 0f, 0, "", 0f, true, "", 0, 0f, 0f, "", 0, 0f, 0, 0f, 3, "炽", "", "", false, 0f, "火", 0, 3f, 0f, 0f, 0, "Dumb", "", "", 0f, "", "", "", "");
            config[2080085] = new SkillConfig(2080085, "炽热", "炽", "", "", "智技up", "", 5, 0f, 0f, 0, "", 0f, true, "", 0, 0f, 0f, "", 0, 0f, 0, 0f, 3, "炽", "", "", false, 0f, "火", 0, 3f, 0f, 0f, 0, "Dumb", "", "", 0f, "", "", "", "");

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
