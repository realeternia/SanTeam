﻿﻿﻿﻿﻿using System;
using System.Collections;
using System.Collections.Generic;

namespace CommonConfig
{
    public class SkillBaseConfig
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
            {"Version", new FieldMetaInfo("版本", "float", 60, "1.0:#384639,1.1:#333954")},
            {"Descript", new FieldMetaInfo("说明(内联/字段名动态引用,如 /damagestrength、/strength2-<key>、/linkself-atk)", "string", 348)},
            {"Type", new FieldMetaInfo("分类", "string", 0)},
            {"Template", new FieldMetaInfo("技能模板名(对应技能模板表Name;空=不使用模板走原有各列)", "string", 78)},
            {"Modules", new FieldMetaInfo("组件名列表(对应技能模板组件表Name;空=用模板默认组件)", "string[]", 0)},
            {"Lv", new FieldMetaInfo("等级", "int", 60)},
            {"Rate", new FieldMetaInfo("发动概率", "float", 59)},
            {"CD", new FieldMetaInfo("发动cd", "float", 60)},
            {"MpCost", new FieldMetaInfo("技能MP消耗", "int", 60)},
            {"StartMpRate", new FieldMetaInfo("开局MP比例(0=从0充能,1=开局满MP)", "float", 60)},
            {"TriggerCondition", new FieldMetaInfo("触发条件", "string", 101)},
            {"DamageType", new FieldMetaInfo("伤害类型(0法术/1物理/2真实)", "int", 60, "0:法术,1:物理,2:真实")},
            {"HurtTag", new FieldMetaInfo("伤害标签(如AntiShield=绕过护盾打血)", "string", 0)},
            {"Range", new FieldMetaInfo("范围", "float", 60)},
            {"Area", new FieldMetaInfo("范围", "float", 60, "伤害范围等")},
            {"TargetType", new FieldMetaInfo("选取点", "string", 0)},
            {"TargetCount", new FieldMetaInfo("最大目标数", "int", 60)},
            {"DamageStrength", new FieldMetaInfo("技能强度（恒定,伤害/DOT基值专用）", "float", 60)},
            {"HealStrength", new FieldMetaInfo("技能治疗基值（恒定,治疗专用）", "float", 59)},
            {"Strength2", new FieldMetaInfo("数值槽（key:value,key:value；按键存取，如 /strength2-<key>）", "string", 214)},
            {"StrengthBuff1", new FieldMetaInfo("Buff数值槽（key:value,key:value；按键存取，如 /strengthbuff1-<key>）", "string", 176)},
            {"BuffId", new FieldMetaInfo("Buff短名(多buff用逗号分隔,如盾,甲)", "string", 0)},
            {"BuffTime", new FieldMetaInfo("Buff持续", "float", 60)},
            {"SummonTag", new FieldMetaInfo("召唤物标签", "string", 0)},
            {"SummonCount", new FieldMetaInfo("技能场数", "int", 60)},
            {"SummonTime", new FieldMetaInfo("技能场持续", "float", 60)},
            {"SummonHitInterval", new FieldMetaInfo("技能场间隔", "float", 60)},
            {"SummonSpeed", new FieldMetaInfo("技能场速度", "float", 60)},
            {"ItemId", new FieldMetaInfo("获得道具Id", "int", 60)},
            {"ScriptName", new FieldMetaInfo("脚本名", "string", 153)},
            {"Action", new FieldMetaInfo("动作", "string", 0)},
            {"SelfEffect", new FieldMetaInfo("施法自身特效(施法/触发瞬间在施法者身上播放,取增益/目标特效)", "string", 159, "effect")},
            {"HitEffect", new FieldMetaInfo("hit", "string", 159, "effect")},
            {"AreaEffect", new FieldMetaInfo("范围特效(仅范围技能用,按Area缩放)", "string", 204, "effect")},
            {"Icon", new FieldMetaInfo("图标", "string", 173)},
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
        ///版本（1.0=老技能，1.1=新增武将批次）
        /// </summary>
        public float Version;
        /// <summary>
        ///说明（Lv1 为模板，内联 /字段名 动态引用，如 /damagestrength、/strength2-<key>、/linkself-atk；其余等级清空自动回退 Lv1 模板并按本级字段值替换。无动态引用的整型文案可直接写完整说明）
        /// </summary>
        public string Descript;
        /// <summary>
        ///分类
        /// </summary>
        public string Type;
        /// <summary>
        ///技能模板名（对应 SkillTemplateConfig.Name；空=不使用模板，走原有各列配置；非空=基于模板+组件初始化，本行其余效果列可不配）
        /// </summary>
        public string Template;
        /// <summary>
        ///组件名列表（对应 SkillTemplateComponentConfig.Name；空=使用模板默认组件）
        /// </summary>
        public string[] Modules;
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
        ///技能MP消耗（0=不使用MP，>0则靠普攻/受击获取法力（mpRegen 为百分比加成），满才能发动，发动后清空）
        /// </summary>
        public int MpCost;
        /// <summary>
        ///开局MP比例（0~1；0=从0开始充能（默认），1=开局即满MP，介于两者表示开局已有一部分MP）
        /// </summary>
        public float StartMpRate;
        /// <summary>
        ///触发条件（满足才触发技能，如 hprate<50=自身生命低于50%；多条件用;分隔）
        /// </summary>
        public string TriggerCondition;
        /// <summary>
        ///伤害类型：0=法术（ap，按法强成长，受魔抗减免）；1=物理（atk，按攻击成长，受护甲减免）；2=真实（无视抗性，护盾不吸收）
        /// </summary>
        public int DamageType;
        /// <summary>
        ///伤害标签（如"AntiShield"=绕过护盾直接打血，破盾类技能用；空=无标签）
        /// </summary>
        public string HurtTag;
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
        ///伤害基值（伤害/DOT 专用）：技能伤害的固定系数（技能伤害=固定系数+比例系数×关联属性）
        /// </summary>
        public float DamageStrength;
        /// <summary>
        ///治疗基值（治疗专用）：治疗量固定系数（治疗量=固定系数×(100+ap)/100），与伤害解耦
        /// </summary>
        public float HealStrength;
        /// <summary>
        ///数值槽（key:value,key:value）：按键存取，语义由脚本与描述决定；空串=无，缺key读取得0
        /// </summary>
        public string Strength2;
        /// <summary>
        ///Buff数值槽（key:value,key:value）：按键存取，由 BuffXxx 脚本按各自固定 key 读取；空串=无
        /// </summary>
        public string StrengthBuff1;
        public string BuffId;
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
        ///施法自身特效（施法/触发瞬间在施法者身上播放，取"增益/目标"类特效；空=不播）
        /// </summary>
        public string SelfEffect;
        /// <summary>
        ///hit
        /// </summary>
        public string HitEffect;
        /// <summary>
        ///范围特效（仅范围技能使用，视觉缩放 = EffectConfig.Scale × Area / CombatConst.AreaEffectBaseRadius）
        /// </summary>
        public string AreaEffect;
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


        public SkillBaseConfig(int Id, string Name, string Sname, float Version, string Descript, string Type, string Template, string[] Modules, int Lv, float Rate, float CD, int MpCost, float StartMpRate, string TriggerCondition, int DamageType, string HurtTag, float Range, float Area, string TargetType, int TargetCount, float DamageStrength, float HealStrength, string Strength2, string StrengthBuff1, string BuffId, float BuffTime, string SummonTag, int SummonCount, float SummonTime, float SummonHitInterval, float SummonSpeed, int ItemId, string ScriptName, string Action, string SelfEffect, string HitEffect, string AreaEffect, string Icon, string LinkSelf, string LinkTeam, string AuroAttrs)
        {
            this.Id = Id;
            this.Name = Name;
            this.Sname = Sname;
            this.Version = Version;
            this.Descript = Descript;
            this.Type = Type;
            this.Template = Template;
            this.Modules = Modules;
            this.Lv = Lv;
            this.Rate = Rate;
            this.CD = CD;
            this.MpCost = MpCost;
            this.StartMpRate = StartMpRate;
            this.TriggerCondition = TriggerCondition;
            this.DamageType = DamageType;
            this.HurtTag = HurtTag;
            this.Range = Range;
            this.Area = Area;
            this.TargetType = TargetType;
            this.TargetCount = TargetCount;
            this.DamageStrength = DamageStrength;
            this.HealStrength = HealStrength;
            this.Strength2 = Strength2;
            this.StrengthBuff1 = StrengthBuff1;
            this.BuffId = BuffId;
            this.BuffTime = BuffTime;
            this.SummonTag = SummonTag;
            this.SummonCount = SummonCount;
            this.SummonTime = SummonTime;
            this.SummonHitInterval = SummonHitInterval;
            this.SummonSpeed = SummonSpeed;
            this.ItemId = ItemId;
            this.ScriptName = ScriptName;
            this.Action = Action;
            this.SelfEffect = SelfEffect;
            this.HitEffect = HitEffect;
            this.AreaEffect = AreaEffect;
            this.Icon = Icon;
            this.LinkSelf = LinkSelf;
            this.LinkTeam = LinkTeam;
            this.AuroAttrs = AuroAttrs;
        }

        public SkillBaseConfig() { }

        private static Dictionary<int, SkillBaseConfig> config = new Dictionary<int, SkillBaseConfig>();
        public static Dictionary<int, SkillBaseConfig>.ValueCollection ConfigList
        {
            get { return config.Values; }
        }

        public static void Refresh(Dictionary<int, SkillBaseConfig> dict)
        {
            config.Clear();
            config = dict;
            RebuildIndex();
        }

        public static void Load()
        {
            config.Clear();
            config[2020021] = new SkillBaseConfig(2020021, "一统", "司马炎", 1.0f, "以自身为中心/area范围内，近战单位获得/strengthbuff1-armor点护甲，远程单位获得/strengthbuff1-atk点攻击，持续/bufftime秒", "群增", "群体友方增益", new string[]{"加护甲大","加攻中","不按首段去重","近远分段"}, 1, 0f, 4f, 80, 0f, "", 0, "", 0f, 20f, "splitByRange,noDedupe", 0, 0f, 0f, "", "armor:10,atk:12", "甲,攻", 5f, "", 0, 0f, 0f, 0f, 0, "SkillAreaBuff", "sway", "ShieldSoftYellow", "LightningExplosionYellow", "MagicAuraYellow", "", "", "", "");
            config[2020022] = new SkillBaseConfig(2020022, "一统", "司马炎", 1.0f, "", "群增", "群体友方增益", new string[]{"加护甲大","加攻中","不按首段去重","近远分段"}, 2, 0f, 4f, 80, 0f, "", 0, "", 0f, 20f, "splitByRange,noDedupe", 0, 0f, 0f, "", "armor:14,atk:18", "甲,攻", 5f, "", 0, 0f, 0f, 0f, 0, "SkillAreaBuff", "sway", "ShieldSoftYellow", "LightningExplosionYellow", "MagicAuraYellow", "", "", "", "");
            config[2020023] = new SkillBaseConfig(2020023, "一统", "司马炎", 1.0f, "", "群增", "群体友方增益", new string[]{"加护甲大","加攻中","不按首段去重","近远分段"}, 3, 0f, 4f, 80, 0f, "", 0, "", 0f, 20f, "splitByRange,noDedupe", 0, 0f, 0f, "", "armor:20,atk:26", "甲,攻", 5f, "", 0, 0f, 0f, 0f, 0, "SkillAreaBuff", "sway", "ShieldSoftYellow", "LightningExplosionYellow", "MagicAuraYellow", "", "", "", "");
            config[2020024] = new SkillBaseConfig(2020024, "一统", "司马炎", 1.0f, "", "群增", "群体友方增益", new string[]{"加护甲大","加攻中","不按首段去重","近远分段"}, 4, 0f, 4f, 80, 0f, "", 0, "", 0f, 20f, "splitByRange,noDedupe", 0, 0f, 0f, "", "armor:28,atk:36", "甲,攻", 5f, "", 0, 0f, 0f, 0f, 0, "SkillAreaBuff", "sway", "ShieldSoftYellow", "LightningExplosionYellow", "MagicAuraYellow", "", "", "", "");
            config[2020025] = new SkillBaseConfig(2020025, "一统", "司马炎", 1.0f, "", "群增", "群体友方增益", new string[]{"加护甲大","加攻中","不按首段去重","近远分段"}, 5, 0f, 4f, 80, 0f, "", 0, "", 0f, 20f, "splitByRange,noDedupe", 0, 0f, 0f, "", "armor:38,atk:50", "甲,攻", 5f, "", 0, 0f, 0f, 0f, 0, "SkillAreaBuff", "sway", "ShieldSoftYellow", "LightningExplosionYellow", "MagicAuraYellow", "", "", "", "");
            config[2020026] = new SkillBaseConfig(2020026, "名门", "袁绍", 1.0f, "对目标造成/damagestrength点法术伤害，并以目标为中心对/area范围内其余敌人造成50%溅射伤害", "单伤", "单体伤害中", new string[]{}, 1, 0f, 2f, 60, 0f, "", 0, "", 50f, 35f, "", 0, 80f, 0f, "explodeRate:0.5", "", "", 0f, "", 0, 0f, 0f, 0f, 0, "SkillAidExplode", "sway", "MagicChargeGreen", "ExplosionFireballFire", "MagicNovaYellow", "", "", "", "");
            config[2020027] = new SkillBaseConfig(2020027, "名门", "袁绍", 1.0f, "", "单伤", "单体伤害中", new string[]{}, 2, 0f, 2f, 60, 0f, "", 0, "", 50f, 35f, "", 0, 120f, 0f, "explodeRate:0.5", "", "", 0f, "", 0, 0f, 0f, 0f, 0, "SkillAidExplode", "sway", "MagicChargeGreen", "ExplosionFireballFire", "MagicNovaYellow", "", "", "", "");
            config[2020028] = new SkillBaseConfig(2020028, "名门", "袁绍", 1.0f, "", "单伤", "单体伤害中", new string[]{}, 3, 0f, 2f, 60, 0f, "", 0, "", 50f, 35f, "", 0, 200f, 0f, "explodeRate:0.5", "", "", 0f, "", 0, 0f, 0f, 0f, 0, "SkillAidExplode", "sway", "MagicChargeGreen", "ExplosionFireballFire", "MagicNovaYellow", "", "", "", "");
            config[2020029] = new SkillBaseConfig(2020029, "名门", "袁绍", 1.0f, "", "单伤", "单体伤害中", new string[]{}, 4, 0f, 2f, 60, 0f, "", 0, "", 50f, 35f, "", 0, 320f, 0f, "explodeRate:0.5", "", "", 0f, "", 0, 0f, 0f, 0f, 0, "SkillAidExplode", "sway", "MagicChargeGreen", "ExplosionFireballFire", "MagicNovaYellow", "", "", "", "");
            config[2020030] = new SkillBaseConfig(2020030, "名门", "袁绍", 1.0f, "", "单伤", "单体伤害中", new string[]{}, 5, 0f, 2f, 60, 0f, "", 0, "", 50f, 35f, "", 0, 480f, 0f, "explodeRate:0.5", "", "", 0f, "", 0, 0f, 0f, 0f, 0, "SkillAidExplode", "sway", "MagicChargeGreen", "ExplosionFireballFire", "MagicNovaYellow", "", "", "", "");
            config[2020031] = new SkillBaseConfig(2020031, "腾驾", "马腾", 1.1f, "对目标造成/damagestrength点法术伤害，并降低其/strengthbuff1-armorDown点护甲与/strengthbuff1-atkDown点攻击，持续/bufftime秒", "单伤", "单体伤害中", new string[]{"减防中","减攻大"}, 1, 0f, 2f, 60, 0f, "", 0, "", 30f, 0f, "", 0, 80f, 0f, "", "armorDown:15,atkDown:20", "破,慑", 4f, "", 0, 0f, 0f, 0f, 0, "SkillSingleDamage", "saw", "MagicChargeBlue", "GasShootFire", "", "", "", "", "");
            config[2020032] = new SkillBaseConfig(2020032, "腾驾", "马腾", 1.1f, "", "单伤", "单体伤害中", new string[]{"减防中","减攻大"}, 2, 0f, 2f, 60, 0f, "", 0, "", 30f, 0f, "", 0, 120f, 0f, "", "armorDown:22,atkDown:30", "破,慑", 4f, "", 0, 0f, 0f, 0f, 0, "SkillSingleDamage", "saw", "MagicChargeBlue", "GasShootFire", "", "", "", "", "");
            config[2020033] = new SkillBaseConfig(2020033, "腾驾", "马腾", 1.1f, "", "单伤", "单体伤害中", new string[]{"减防中","减攻大"}, 3, 0f, 2f, 60, 0f, "", 0, "", 30f, 0f, "", 0, 200f, 0f, "", "armorDown:32,atkDown:42", "破,慑", 4f, "", 0, 0f, 0f, 0f, 0, "SkillSingleDamage", "saw", "MagicChargeBlue", "GasShootFire", "", "", "", "", "");
            config[2020034] = new SkillBaseConfig(2020034, "腾驾", "马腾", 1.1f, "", "单伤", "单体伤害中", new string[]{"减防中","减攻大"}, 4, 0f, 2f, 60, 0f, "", 0, "", 30f, 0f, "", 0, 320f, 0f, "", "armorDown:45,atkDown:58", "破,慑", 4f, "", 0, 0f, 0f, 0f, 0, "SkillSingleDamage", "saw", "MagicChargeBlue", "GasShootFire", "", "", "", "", "");
            config[2020035] = new SkillBaseConfig(2020035, "腾驾", "马腾", 1.1f, "", "单伤", "单体伤害中", new string[]{"减防中","减攻大"}, 5, 0f, 2f, 60, 0f, "", 0, "", 30f, 0f, "", 0, 480f, 0f, "", "armorDown:60,atkDown:80", "破,慑", 4f, "", 0, 0f, 0f, 0f, 0, "SkillSingleDamage", "saw", "MagicChargeBlue", "GasShootFire", "", "", "", "", "");
            config[2020036] = new SkillBaseConfig(2020036, "放权", "刘禅", 1.2f, "治疗生命比例最低的一名友军/healstrength点生命，并提升其/strengthbuff1-atk点攻击，持续/bufftime秒", "单疗", "单体治疗", new string[]{"加攻大"}, 1, 0f, 2f, 70, 0f, "", 0, "", 50f, 60f, "", 1, 0f, 100f, "", "atk:20", "攻", 5f, "", 0, 0f, 0f, 0f, 0, "SkillAreaHeal", "sway", "ShieldSoftBlue", "HeartStream", "", "", "", "", "");
            config[2020037] = new SkillBaseConfig(2020037, "放权", "刘禅", 1.2f, "", "单疗", "单体治疗", new string[]{"加攻大"}, 2, 0f, 2f, 70, 0f, "", 0, "", 50f, 60f, "", 1, 0f, 150f, "", "atk:30", "攻", 5f, "", 0, 0f, 0f, 0f, 0, "SkillAreaHeal", "sway", "ShieldSoftBlue", "HeartStream", "", "", "", "", "");
            config[2020038] = new SkillBaseConfig(2020038, "放权", "刘禅", 1.2f, "", "单疗", "单体治疗", new string[]{"加攻大"}, 3, 0f, 2f, 70, 0f, "", 0, "", 50f, 60f, "", 1, 0f, 250f, "", "atk:45", "攻", 5f, "", 0, 0f, 0f, 0f, 0, "SkillAreaHeal", "sway", "ShieldSoftBlue", "HeartStream", "", "", "", "", "");
            config[2020039] = new SkillBaseConfig(2020039, "放权", "刘禅", 1.2f, "", "单疗", "单体治疗", new string[]{"加攻大"}, 4, 0f, 2f, 70, 0f, "", 0, "", 50f, 60f, "", 1, 0f, 400f, "", "atk:65", "攻", 5f, "", 0, 0f, 0f, 0f, 0, "SkillAreaHeal", "sway", "ShieldSoftBlue", "HeartStream", "", "", "", "", "");
            config[2020040] = new SkillBaseConfig(2020040, "放权", "刘禅", 1.2f, "", "单疗", "单体治疗", new string[]{"加攻大"}, 5, 0f, 2f, 70, 0f, "", 0, "", 50f, 60f, "", 1, 0f, 600f, "", "atk:95", "攻", 5f, "", 0, 0f, 0f, 0f, 0, "SkillAreaHeal", "sway", "ShieldSoftBlue", "HeartStream", "", "", "", "", "");
            config[2020046] = new SkillBaseConfig(2020046, "暴敛", "孙皓", 1.1f, "以目标为中心对/area范围内敌人造成/damagestrength点法术伤害，并使其受到的伤害提升/strengthbuff1-damagedUp%，持续/bufftime秒", "群伤", "群体伤害近距离", new string[]{"增伤中"}, 1, 0f, 2f, 70, 0f, "", 0, "", 40f, 20f, "", 0, 70f, 0f, "", "damagedUp:0.1", "伤", 4f, "", 0, 0f, 0f, 0f, 0, "SkillAreaDamageDeBuff", "sway", "ShieldSoftYellow", "SharpExplosionGreen", "PoisonExplosionSoft", "", "", "", "");
            config[2020047] = new SkillBaseConfig(2020047, "暴敛", "孙皓", 1.1f, "", "群伤", "群体伤害近距离", new string[]{"增伤中"}, 2, 0f, 2f, 70, 0f, "", 0, "", 40f, 20f, "", 0, 105f, 0f, "", "damagedUp:0.15", "伤", 4f, "", 0, 0f, 0f, 0f, 0, "SkillAreaDamageDeBuff", "sway", "ShieldSoftYellow", "SharpExplosionGreen", "PoisonExplosionSoft", "", "", "", "");
            config[2020048] = new SkillBaseConfig(2020048, "暴敛", "孙皓", 1.1f, "", "群伤", "群体伤害近距离", new string[]{"增伤中"}, 3, 0f, 2f, 70, 0f, "", 0, "", 40f, 20f, "", 0, 175f, 0f, "", "damagedUp:0.2", "伤", 4f, "", 0, 0f, 0f, 0f, 0, "SkillAreaDamageDeBuff", "sway", "ShieldSoftYellow", "SharpExplosionGreen", "PoisonExplosionSoft", "", "", "", "");
            config[2020049] = new SkillBaseConfig(2020049, "暴敛", "孙皓", 1.1f, "", "群伤", "群体伤害近距离", new string[]{"增伤中"}, 4, 0f, 2f, 70, 0f, "", 0, "", 40f, 20f, "", 0, 280f, 0f, "", "damagedUp:0.26", "伤", 4f, "", 0, 0f, 0f, 0f, 0, "SkillAreaDamageDeBuff", "sway", "ShieldSoftYellow", "SharpExplosionGreen", "PoisonExplosionSoft", "", "", "", "");
            config[2020050] = new SkillBaseConfig(2020050, "暴敛", "孙皓", 1.1f, "", "群伤", "群体伤害近距离", new string[]{"增伤中"}, 5, 0f, 2f, 70, 0f, "", 0, "", 40f, 20f, "", 0, 420f, 0f, "", "damagedUp:0.33", "伤", 4f, "", 0, 0f, 0f, 0f, 0, "SkillAreaDamageDeBuff", "sway", "ShieldSoftYellow", "SharpExplosionGreen", "PoisonExplosionSoft", "", "", "", "");
            config[2020066] = new SkillBaseConfig(2020066, "守护", "刘璋", 1.2f, "治疗生命比例最低的一名友军/healstrength点生命，并附加/strengthbuff1-shieldReduce%减伤盾，持续/bufftime秒", "单疗", "单体治疗", new string[]{"减伤大"}, 1, 0f, 2f, 70, 0f, "", 0, "", 50f, 60f, "", 1, 0f, 100f, "", "shieldReduce:0.15", "硬", 5f, "", 0, 0f, 0f, 0f, 0, "SkillAreaHeal", "sway", "MagicChargeBlue", "ShieldSoftBlue", "", "", "", "", "");
            config[2020067] = new SkillBaseConfig(2020067, "守护", "刘璋", 1.2f, "", "单疗", "单体治疗", new string[]{"减伤大"}, 2, 0f, 2f, 70, 0f, "", 0, "", 50f, 60f, "", 1, 0f, 150f, "", "shieldReduce:0.2", "硬", 5f, "", 0, 0f, 0f, 0f, 0, "SkillAreaHeal", "sway", "MagicChargeBlue", "ShieldSoftBlue", "", "", "", "", "");
            config[2020068] = new SkillBaseConfig(2020068, "守护", "刘璋", 1.2f, "", "单疗", "单体治疗", new string[]{"减伤大"}, 3, 0f, 2f, 70, 0f, "", 0, "", 50f, 60f, "", 1, 0f, 250f, "", "shieldReduce:0.26", "硬", 5f, "", 0, 0f, 0f, 0f, 0, "SkillAreaHeal", "sway", "MagicChargeBlue", "ShieldSoftBlue", "", "", "", "", "");
            config[2020069] = new SkillBaseConfig(2020069, "守护", "刘璋", 1.2f, "", "单疗", "单体治疗", new string[]{"减伤大"}, 4, 0f, 2f, 70, 0f, "", 0, "", 50f, 60f, "", 1, 0f, 400f, "", "shieldReduce:0.33", "硬", 5f, "", 0, 0f, 0f, 0f, 0, "SkillAreaHeal", "sway", "MagicChargeBlue", "ShieldSoftBlue", "", "", "", "", "");
            config[2020070] = new SkillBaseConfig(2020070, "守护", "刘璋", 1.2f, "", "单疗", "单体治疗", new string[]{"减伤大"}, 5, 0f, 2f, 70, 0f, "", 0, "", 50f, 60f, "", 1, 0f, 600f, "", "shieldReduce:0.42", "硬", 5f, "", 0, 0f, 0f, 0f, 0, "SkillAreaHeal", "sway", "MagicChargeBlue", "ShieldSoftBlue", "", "", "", "", "");
            config[2020071] = new SkillBaseConfig(2020071, "乱政", "刘表", 1.1f, "对目标造成/damagestrength点法术伤害，并使其混乱/bufftime秒", "单控", "单体伤害小", new string[]{"混乱中"}, 1, 0f, 1.5f, 50, 0f, "", 0, "", 40f, 0f, "", 0, 60f, 0f, "", "", "惑", 0.8f, "", 0, 0f, 0f, 0f, 0, "SkillSingleDamage", "sway", "MagicChargePink", "MissesBlue", "", "", "", "", "");
            config[2020072] = new SkillBaseConfig(2020072, "乱政", "刘表", 1.1f, "", "单控", "单体伤害小", new string[]{"混乱中"}, 2, 0f, 1.5f, 50, 0f, "", 0, "", 40f, 0f, "", 0, 90f, 0f, "", "", "惑", 1f, "", 0, 0f, 0f, 0f, 0, "SkillSingleDamage", "sway", "MagicChargePink", "MissesBlue", "", "", "", "", "");
            config[2020073] = new SkillBaseConfig(2020073, "乱政", "刘表", 1.1f, "", "单控", "单体伤害小", new string[]{"混乱中"}, 3, 0f, 1.5f, 50, 0f, "", 0, "", 40f, 0f, "", 0, 150f, 0f, "", "", "惑", 1.2f, "", 0, 0f, 0f, 0f, 0, "SkillSingleDamage", "sway", "MagicChargePink", "MissesBlue", "", "", "", "", "");
            config[2020074] = new SkillBaseConfig(2020074, "乱政", "刘表", 1.1f, "", "单控", "单体伤害小", new string[]{"混乱中"}, 4, 0f, 1.5f, 50, 0f, "", 0, "", 40f, 0f, "", 0, 240f, 0f, "", "", "惑", 1.5f, "", 0, 0f, 0f, 0f, 0, "SkillSingleDamage", "sway", "MagicChargePink", "MissesBlue", "", "", "", "", "");
            config[2020075] = new SkillBaseConfig(2020075, "乱政", "刘表", 1.1f, "", "单控", "单体伤害小", new string[]{"混乱中"}, 5, 0f, 1.5f, 50, 0f, "", 0, "", 40f, 0f, "", 0, 360f, 0f, "", "", "惑", 1.8f, "", 0, 0f, 0f, 0f, 0, "SkillSingleDamage", "sway", "MagicChargePink", "MissesBlue", "", "", "", "", "");
            config[2020116] = new SkillBaseConfig(2020116, "标记", "朱桓", 1.0f, "对目标造成/damagestrength法术伤害，并使其受到的伤害提升/strengthbuff1-damagedUp%，持续/bufftime秒", "单伤", "单体伤害大", new string[]{"增伤大"}, 1, 0f, 2f, 70, 0f, "", 0, "", 40f, 0f, "", 0, 100f, 0f, "", "damagedUp:0.15", "伤", 4f, "", 0, 0f, 0f, 0f, 0, "SkillSingleDamage", "sway", "MagicChargePink", "SwordHitYellowCritical", "", "", "", "", "");
            config[2020117] = new SkillBaseConfig(2020117, "标记", "朱桓", 1.0f, "", "单伤", "单体伤害大", new string[]{"增伤大"}, 2, 0f, 2f, 70, 0f, "", 0, "", 40f, 0f, "", 0, 150f, 0f, "", "damagedUp:0.2", "伤", 4f, "", 0, 0f, 0f, 0f, 0, "SkillSingleDamage", "sway", "MagicChargePink", "SwordHitYellowCritical", "", "", "", "", "");
            config[2020118] = new SkillBaseConfig(2020118, "标记", "朱桓", 1.0f, "", "单伤", "单体伤害大", new string[]{"增伤大"}, 3, 0f, 2f, 70, 0f, "", 0, "", 40f, 0f, "", 0, 250f, 0f, "", "damagedUp:0.25", "伤", 4f, "", 0, 0f, 0f, 0f, 0, "SkillSingleDamage", "sway", "MagicChargePink", "SwordHitYellowCritical", "", "", "", "", "");
            config[2020119] = new SkillBaseConfig(2020119, "标记", "朱桓", 1.0f, "", "单伤", "单体伤害大", new string[]{"增伤大"}, 4, 0f, 2f, 70, 0f, "", 0, "", 40f, 0f, "", 0, 400f, 0f, "", "damagedUp:0.3", "伤", 4f, "", 0, 0f, 0f, 0f, 0, "SkillSingleDamage", "sway", "MagicChargePink", "SwordHitYellowCritical", "", "", "", "", "");
            config[2020120] = new SkillBaseConfig(2020120, "标记", "朱桓", 1.0f, "", "单伤", "单体伤害大", new string[]{"增伤大"}, 5, 0f, 2f, 70, 0f, "", 0, "", 40f, 0f, "", 0, 600f, 0f, "", "damagedUp:0.35", "伤", 4f, "", 0, 0f, 0f, 0f, 0, "SkillSingleDamage", "sway", "MagicChargePink", "SwordHitYellowCritical", "", "", "", "", "");
            config[2020121] = new SkillBaseConfig(2020121, "双射", "韩当", 1.0f, "对目标及附近/targetcount-1名敌人各造成/damagestrength法术伤害", "群伤", "群体伤害近距离", new string[]{}, 1, 0f, 2f, 70, 0f, "", 0, "", 40f, 20f, "", 2, 70f, 0f, "", "", "", 0f, "", 0, 0f, 0f, 0f, 0, "SkillAreaDamageDeBuff", "throw", "StarExplosionBlue", "SwordHitBlue", "MagicNovaBlue", "", "", "", "");
            config[2020122] = new SkillBaseConfig(2020122, "双射", "韩当", 1.0f, "", "群伤", "群体伤害近距离", new string[]{}, 2, 0f, 2f, 70, 0f, "", 0, "", 40f, 20f, "", 2, 105f, 0f, "", "", "", 0f, "", 0, 0f, 0f, 0f, 0, "SkillAreaDamageDeBuff", "throw", "StarExplosionBlue", "SwordHitBlue", "MagicNovaBlue", "", "", "", "");
            config[2020123] = new SkillBaseConfig(2020123, "双射", "韩当", 1.0f, "", "群伤", "群体伤害近距离", new string[]{}, 3, 0f, 2f, 70, 0f, "", 0, "", 40f, 20f, "", 2, 175f, 0f, "", "", "", 0f, "", 0, 0f, 0f, 0f, 0, "SkillAreaDamageDeBuff", "throw", "StarExplosionBlue", "SwordHitBlue", "MagicNovaBlue", "", "", "", "");
            config[2020124] = new SkillBaseConfig(2020124, "双射", "韩当", 1.0f, "", "群伤", "群体伤害近距离", new string[]{}, 4, 0f, 2f, 70, 0f, "", 0, "", 40f, 20f, "", 2, 280f, 0f, "", "", "", 0f, "", 0, 0f, 0f, 0f, 0, "SkillAreaDamageDeBuff", "throw", "StarExplosionBlue", "SwordHitBlue", "MagicNovaBlue", "", "", "", "");
            config[2020125] = new SkillBaseConfig(2020125, "双射", "韩当", 1.0f, "", "群伤", "群体伤害近距离", new string[]{}, 5, 0f, 2f, 70, 0f, "", 0, "", 40f, 20f, "", 2, 420f, 0f, "", "", "", 0f, "", 0, 0f, 0f, 0f, 0, "SkillAreaDamageDeBuff", "throw", "StarExplosionBlue", "SwordHitBlue", "MagicNovaBlue", "", "", "", "");
            config[2020136] = new SkillBaseConfig(2020136, "先锋斩", "廖化", 1.1f, "对目标造成/damagestrength法术伤害并降低其/strengthbuff1-armorDown点护甲，持续/bufftime秒", "单伤", "单体伤害小", new string[]{"减防大"}, 1, 1f, 1.5f, 50, 0f, "", 0, "", 35f, 0f, "", 0, 60f, 0f, "", "armorDown:20", "破", 4f, "", 0, 0f, 0f, 0f, 0, "SkillSingleDamage", "spin", "MagicChargeBlue", "BulletExplosionBlue", "", "", "", "", "");
            config[2020137] = new SkillBaseConfig(2020137, "先锋斩", "廖化", 1.1f, "", "单伤", "单体伤害小", new string[]{"减防大"}, 2, 1f, 1.5f, 50, 0f, "", 0, "", 35f, 0f, "", 0, 90f, 0f, "", "armorDown:30", "破", 4f, "", 0, 0f, 0f, 0f, 0, "SkillSingleDamage", "spin", "MagicChargeBlue", "BulletExplosionBlue", "", "", "", "", "");
            config[2020138] = new SkillBaseConfig(2020138, "先锋斩", "廖化", 1.1f, "", "单伤", "单体伤害小", new string[]{"减防大"}, 3, 1f, 1.5f, 50, 0f, "", 0, "", 35f, 0f, "", 0, 150f, 0f, "", "armorDown:45", "破", 4f, "", 0, 0f, 0f, 0f, 0, "SkillSingleDamage", "spin", "MagicChargeBlue", "BulletExplosionBlue", "", "", "", "", "");
            config[2020139] = new SkillBaseConfig(2020139, "先锋斩", "廖化", 1.1f, "", "单伤", "单体伤害小", new string[]{"减防大"}, 4, 1f, 1.5f, 50, 0f, "", 0, "", 35f, 0f, "", 0, 240f, 0f, "", "armorDown:65", "破", 4f, "", 0, 0f, 0f, 0f, 0, "SkillSingleDamage", "spin", "MagicChargeBlue", "BulletExplosionBlue", "", "", "", "", "");
            config[2020140] = new SkillBaseConfig(2020140, "先锋斩", "廖化", 1.1f, "", "单伤", "单体伤害小", new string[]{"减防大"}, 5, 1f, 1.5f, 50, 0f, "", 0, "", 35f, 0f, "", 0, 360f, 0f, "", "armorDown:90", "破", 4f, "", 0, 0f, 0f, 0f, 0, "SkillSingleDamage", "spin", "MagicChargeBlue", "BulletExplosionBlue", "", "", "", "", "");
            config[2020141] = new SkillBaseConfig(2020141, "归汉", "夏侯霸", 1.1f, "对目标造成/damagestrength法术伤害并降低其/strengthbuff1-armorDown点护甲，持续/bufftime秒", "单伤", "单体伤害小", new string[]{"减防大"}, 1, 1f, 1.5f, 50, 0f, "", 0, "", 35f, 0f, "", 0, 60f, 0f, "", "armorDown:20", "破", 4f, "", 0, 0f, 0f, 0f, 0, "SkillSingleDamage", "spin", "MagicChargeBlue", "BulletExplosionBlue", "", "", "", "", "");
            config[2020142] = new SkillBaseConfig(2020142, "归汉", "夏侯霸", 1.1f, "", "单伤", "单体伤害小", new string[]{"减防大"}, 2, 1f, 1.5f, 50, 0f, "", 0, "", 35f, 0f, "", 0, 90f, 0f, "", "armorDown:30", "破", 4f, "", 0, 0f, 0f, 0f, 0, "SkillSingleDamage", "spin", "MagicChargeBlue", "BulletExplosionBlue", "", "", "", "", "");
            config[2020143] = new SkillBaseConfig(2020143, "归汉", "夏侯霸", 1.1f, "", "单伤", "单体伤害小", new string[]{"减防大"}, 3, 1f, 1.5f, 50, 0f, "", 0, "", 35f, 0f, "", 0, 150f, 0f, "", "armorDown:45", "破", 4f, "", 0, 0f, 0f, 0f, 0, "SkillSingleDamage", "spin", "MagicChargeBlue", "BulletExplosionBlue", "", "", "", "", "");
            config[2020144] = new SkillBaseConfig(2020144, "归汉", "夏侯霸", 1.1f, "", "单伤", "单体伤害小", new string[]{"减防大"}, 4, 1f, 1.5f, 50, 0f, "", 0, "", 35f, 0f, "", 0, 240f, 0f, "", "armorDown:65", "破", 4f, "", 0, 0f, 0f, 0f, 0, "SkillSingleDamage", "spin", "MagicChargeBlue", "BulletExplosionBlue", "", "", "", "", "");
            config[2020145] = new SkillBaseConfig(2020145, "归汉", "夏侯霸", 1.1f, "", "单伤", "单体伤害小", new string[]{"减防大"}, 5, 1f, 1.5f, 50, 0f, "", 0, "", 35f, 0f, "", 0, 360f, 0f, "", "armorDown:90", "破", 4f, "", 0, 0f, 0f, 0f, 0, "SkillSingleDamage", "spin", "MagicChargeBlue", "BulletExplosionBlue", "", "", "", "", "");
            config[2020146] = new SkillBaseConfig(2020146, "断后", "诸葛绪", 1.1f, "对目标造成/damagestrength法术伤害并使其移动与攻速降低/strengthbuff1-slow%，持续/bufftime秒", "单伤", "单体伤害小", new string[]{"减速大"}, 1, 0f, 1.5f, 50, 0f, "", 0, "", 40f, 0f, "", 0, 60f, 0f, "", "slow:0.3", "缓", 3f, "", 0, 0f, 0f, 0f, 0, "SkillSingleDamage", "saw", "MagicChargeYellow", "SwordHitWhiteCritical", "", "", "", "", "");
            config[2020147] = new SkillBaseConfig(2020147, "断后", "诸葛绪", 1.1f, "", "单伤", "单体伤害小", new string[]{"减速大"}, 2, 0f, 1.5f, 50, 0f, "", 0, "", 40f, 0f, "", 0, 90f, 0f, "", "slow:0.4", "缓", 3f, "", 0, 0f, 0f, 0f, 0, "SkillSingleDamage", "saw", "MagicChargeYellow", "SwordHitWhiteCritical", "", "", "", "", "");
            config[2020148] = new SkillBaseConfig(2020148, "断后", "诸葛绪", 1.1f, "", "单伤", "单体伤害小", new string[]{"减速大"}, 3, 0f, 1.5f, 50, 0f, "", 0, "", 40f, 0f, "", 0, 150f, 0f, "", "slow:0.5", "缓", 3f, "", 0, 0f, 0f, 0f, 0, "SkillSingleDamage", "saw", "MagicChargeYellow", "SwordHitWhiteCritical", "", "", "", "", "");
            config[2020149] = new SkillBaseConfig(2020149, "断后", "诸葛绪", 1.1f, "", "单伤", "单体伤害小", new string[]{"减速大"}, 4, 0f, 1.5f, 50, 0f, "", 0, "", 40f, 0f, "", 0, 240f, 0f, "", "slow:0.6", "缓", 3f, "", 0, 0f, 0f, 0f, 0, "SkillSingleDamage", "saw", "MagicChargeYellow", "SwordHitWhiteCritical", "", "", "", "", "");
            config[2020150] = new SkillBaseConfig(2020150, "断后", "诸葛绪", 1.1f, "", "单伤", "单体伤害小", new string[]{"减速大"}, 5, 0f, 1.5f, 50, 0f, "", 0, "", 40f, 0f, "", 0, 360f, 0f, "", "slow:0.7", "缓", 3f, "", 0, 0f, 0f, 0f, 0, "SkillSingleDamage", "saw", "MagicChargeYellow", "SwordHitWhiteCritical", "", "", "", "", "");
            config[2020216] = new SkillBaseConfig(2020216, "御斩", "乐进", 1.2f, "对目标造成/damagestrength物理伤害，有/strength2-doubleChance%概率造成/strength2-doubleRate倍伤害", "单伤", "单体伤害中", new string[]{"概率双击"}, 1, 0f, 2f, 60, 0f, "", 1, "", 20f, 0f, "doubleChance", 0, 80f, 0f, "doubleChance:0.3,doubleRate:2", "", "", 0f, "", 0, 0f, 0f, 0f, 0, "SkillSingleDamage", "saw", "MagicChargePink", "SwordWhirlwindWhite", "", "", "", "", "");
            config[2020217] = new SkillBaseConfig(2020217, "御斩", "乐进", 1.2f, "", "单伤", "单体伤害中", new string[]{"概率双击"}, 2, 0f, 2f, 60, 0f, "", 1, "", 20f, 0f, "doubleChance", 0, 120f, 0f, "doubleChance:0.3,doubleRate:2", "", "", 0f, "", 0, 0f, 0f, 0f, 0, "SkillSingleDamage", "saw", "MagicChargePink", "SwordWhirlwindWhite", "", "", "", "", "");
            config[2020218] = new SkillBaseConfig(2020218, "御斩", "乐进", 1.2f, "", "单伤", "单体伤害中", new string[]{"概率双击"}, 3, 0f, 2f, 60, 0f, "", 1, "", 20f, 0f, "doubleChance", 0, 200f, 0f, "doubleChance:0.3,doubleRate:2", "", "", 0f, "", 0, 0f, 0f, 0f, 0, "SkillSingleDamage", "saw", "MagicChargePink", "SwordWhirlwindWhite", "", "", "", "", "");
            config[2020219] = new SkillBaseConfig(2020219, "御斩", "乐进", 1.2f, "", "单伤", "单体伤害中", new string[]{"概率双击"}, 4, 0f, 2f, 60, 0f, "", 1, "", 20f, 0f, "doubleChance", 0, 320f, 0f, "doubleChance:0.3,doubleRate:2", "", "", 0f, "", 0, 0f, 0f, 0f, 0, "SkillSingleDamage", "saw", "MagicChargePink", "SwordWhirlwindWhite", "", "", "", "", "");
            config[2020220] = new SkillBaseConfig(2020220, "御斩", "乐进", 1.2f, "", "单伤", "单体伤害中", new string[]{"概率双击"}, 5, 0f, 2f, 60, 0f, "", 1, "", 20f, 0f, "doubleChance", 0, 480f, 0f, "doubleChance:0.3,doubleRate:2", "", "", 0f, "", 0, 0f, 0f, 0f, 0, "SkillSingleDamage", "saw", "MagicChargePink", "SwordWhirlwindWhite", "", "", "", "", "");
            config[2020221] = new SkillBaseConfig(2020221, "横扫", "程普", 1.0f, "对目标造成/damagestrength法术伤害，并为自己附加/strength2-shield点护盾（随法强提升）", "单伤", "单体伤害小", new string[]{"护盾中"}, 1, 0f, 1.5f, 50, 0f, "", 0, "", 35f, 0f, "selfBuff", 0, 60f, 0f, "shield:70", "", "盾", 5f, "", 0, 0f, 0f, 0f, 0, "SkillSingleDamage", "spin", "StarExplosionBlue", "SwordHitBlackRedCritical", "", "", "", "", "");
            config[2020222] = new SkillBaseConfig(2020222, "横扫", "程普", 1.0f, "", "单伤", "单体伤害小", new string[]{"护盾中"}, 2, 0f, 1.5f, 50, 0f, "", 0, "", 35f, 0f, "selfBuff", 0, 90f, 0f, "shield:100", "", "盾", 5f, "", 0, 0f, 0f, 0f, 0, "SkillSingleDamage", "spin", "StarExplosionBlue", "SwordHitBlackRedCritical", "", "", "", "", "");
            config[2020223] = new SkillBaseConfig(2020223, "横扫", "程普", 1.0f, "", "单伤", "单体伤害小", new string[]{"护盾中"}, 3, 0f, 1.5f, 50, 0f, "", 0, "", 35f, 0f, "selfBuff", 0, 150f, 0f, "shield:140", "", "盾", 5f, "", 0, 0f, 0f, 0f, 0, "SkillSingleDamage", "spin", "StarExplosionBlue", "SwordHitBlackRedCritical", "", "", "", "", "");
            config[2020224] = new SkillBaseConfig(2020224, "横扫", "程普", 1.0f, "", "单伤", "单体伤害小", new string[]{"护盾中"}, 4, 0f, 1.5f, 50, 0f, "", 0, "", 35f, 0f, "selfBuff", 0, 240f, 0f, "shield:190", "", "盾", 5f, "", 0, 0f, 0f, 0f, 0, "SkillSingleDamage", "spin", "StarExplosionBlue", "SwordHitBlackRedCritical", "", "", "", "", "");
            config[2020225] = new SkillBaseConfig(2020225, "横扫", "程普", 1.0f, "", "单伤", "单体伤害小", new string[]{"护盾中"}, 5, 0f, 1.5f, 50, 0f, "", 0, "", 35f, 0f, "selfBuff", 0, 360f, 0f, "shield:260", "", "盾", 5f, "", 0, 0f, 0f, 0f, 0, "SkillSingleDamage", "spin", "StarExplosionBlue", "SwordHitBlackRedCritical", "", "", "", "", "");
            config[2020231] = new SkillBaseConfig(2020231, "乱政", "司马伦", 1.1f, "对/area范围内的敌人造成/damagestrength法术伤害并削减其护甲/strengthbuff1-armorDown点，持续/bufftime秒", "群伤", "群体伤害自身中心", new string[]{"减防大"}, 1, 0f, 2f, 70, 0f, "", 0, "", 30f, 20f, "self", 0, 70f, 0f, "", "armorDown:25", "破", 4f, "", 0, 0f, 0f, 0f, 0, "SkillAreaDamageDeBuff", "saw", "MagicChargePink", "SharpExplosionGreen", "PoisonExplosionSoftYellow", "", "", "", "");
            config[2020232] = new SkillBaseConfig(2020232, "乱政", "司马伦", 1.1f, "", "群伤", "群体伤害自身中心", new string[]{"减防大"}, 2, 0f, 2f, 70, 0f, "", 0, "", 30f, 20f, "self", 0, 105f, 0f, "", "armorDown:30", "破", 4f, "", 0, 0f, 0f, 0f, 0, "SkillAreaDamageDeBuff", "saw", "MagicChargePink", "SharpExplosionGreen", "PoisonExplosionSoftYellow", "", "", "", "");
            config[2020233] = new SkillBaseConfig(2020233, "乱政", "司马伦", 1.1f, "", "群伤", "群体伤害自身中心", new string[]{"减防大"}, 3, 0f, 2f, 70, 0f, "", 0, "", 30f, 20f, "self", 0, 175f, 0f, "", "armorDown:38", "破", 4f, "", 0, 0f, 0f, 0f, 0, "SkillAreaDamageDeBuff", "saw", "MagicChargePink", "SharpExplosionGreen", "PoisonExplosionSoftYellow", "", "", "", "");
            config[2020234] = new SkillBaseConfig(2020234, "乱政", "司马伦", 1.1f, "", "群伤", "群体伤害自身中心", new string[]{"减防大"}, 4, 0f, 2f, 70, 0f, "", 0, "", 30f, 20f, "self", 0, 280f, 0f, "", "armorDown:46", "破", 4f, "", 0, 0f, 0f, 0f, 0, "SkillAreaDamageDeBuff", "saw", "MagicChargePink", "SharpExplosionGreen", "PoisonExplosionSoftYellow", "", "", "", "");
            config[2020235] = new SkillBaseConfig(2020235, "乱政", "司马伦", 1.1f, "", "群伤", "群体伤害自身中心", new string[]{"减防大"}, 5, 0f, 2f, 70, 0f, "", 0, "", 30f, 20f, "self", 0, 420f, 0f, "", "armorDown:55", "破", 4f, "", 0, 0f, 0f, 0f, 0, "SkillAreaDamageDeBuff", "saw", "MagicChargePink", "SharpExplosionGreen", "PoisonExplosionSoftYellow", "", "", "", "");
            config[2020236] = new SkillBaseConfig(2020236, "屯田", "王昶", 1.1f, "在/area范围降下雷电阵，每秒对其中敌人造成/damagestrength法术伤害，持续/summontime秒，法阵持续期间向前缓慢移动/strength2-moveDist距离", "群伤", "召唤技能场", new string[]{}, 1, 0f, 2.5f, 90, 0f, "", 0, "", 40f, 35f, "", 4, 20f, 0f, "moveDist:40", "", "", 0f, "雷", 0, 5.2f, 1f, 0f, 0, "SkillAidHitRegion", "spin", "MagicChargeBlue", "", "top_down_stone_circle", "", "", "", "");
            config[2020237] = new SkillBaseConfig(2020237, "屯田", "王昶", 1.1f, "", "群伤", "召唤技能场", new string[]{}, 2, 0f, 2.5f, 90, 0f, "", 0, "", 40f, 35f, "", 4, 30f, 0f, "moveDist:40", "", "", 0f, "雷", 0, 5.2f, 1f, 0f, 0, "SkillAidHitRegion", "spin", "MagicChargeBlue", "", "top_down_stone_circle", "", "", "", "");
            config[2020238] = new SkillBaseConfig(2020238, "屯田", "王昶", 1.1f, "", "群伤", "召唤技能场", new string[]{}, 3, 0f, 2.5f, 90, 0f, "", 0, "", 40f, 35f, "", 4, 50f, 0f, "moveDist:40", "", "", 0f, "雷", 0, 5.2f, 1f, 0f, 0, "SkillAidHitRegion", "spin", "MagicChargeBlue", "", "top_down_stone_circle", "", "", "", "");
            config[2020239] = new SkillBaseConfig(2020239, "屯田", "王昶", 1.1f, "", "群伤", "召唤技能场", new string[]{}, 4, 0f, 2.5f, 90, 0f, "", 0, "", 40f, 35f, "", 4, 80f, 0f, "moveDist:40", "", "", 0f, "雷", 0, 5.2f, 1f, 0f, 0, "SkillAidHitRegion", "spin", "MagicChargeBlue", "", "top_down_stone_circle", "", "", "", "");
            config[2020240] = new SkillBaseConfig(2020240, "屯田", "王昶", 1.1f, "", "群伤", "召唤技能场", new string[]{}, 5, 0f, 2.5f, 90, 0f, "", 0, "", 40f, 35f, "", 4, 120f, 0f, "moveDist:40", "", "", 0f, "雷", 0, 5.2f, 1f, 0f, 0, "SkillAidHitRegion", "spin", "MagicChargeBlue", "", "top_down_stone_circle", "", "", "", "");
            config[2020316] = new SkillBaseConfig(2020316, "迅射", "凌统", 1.0f, "短时间内攻击速度提升/strengthbuff1-atkSpeed%，持续/bufftime秒", "自增", "自身强化", new string[]{"加攻速大"}, 1, 0f, 3f, 60, 0f, "", 0, "", 0f, 0f, "", 0, 0f, 0f, "", "atkSpeed:0.3", "快", 5f, "", 0, 0f, 0f, 0f, 0, "SkillSelfBuff", "saw", "StarExplosionBlue", "", "", "", "", "", "");
            config[2020317] = new SkillBaseConfig(2020317, "迅射", "凌统", 1.0f, "", "自增", "自身强化", new string[]{"加攻速大"}, 2, 0f, 3f, 60, 0f, "", 0, "", 0f, 0f, "", 0, 0f, 0f, "", "atkSpeed:0.45", "快", 5f, "", 0, 0f, 0f, 0f, 0, "SkillSelfBuff", "saw", "StarExplosionBlue", "", "", "", "", "", "");
            config[2020318] = new SkillBaseConfig(2020318, "迅射", "凌统", 1.0f, "", "自增", "自身强化", new string[]{"加攻速大"}, 3, 0f, 3f, 60, 0f, "", 0, "", 0f, 0f, "", 0, 0f, 0f, "", "atkSpeed:0.6", "快", 5f, "", 0, 0f, 0f, 0f, 0, "SkillSelfBuff", "saw", "StarExplosionBlue", "", "", "", "", "", "");
            config[2020319] = new SkillBaseConfig(2020319, "迅射", "凌统", 1.0f, "", "自增", "自身强化", new string[]{"加攻速大"}, 4, 0f, 3f, 60, 0f, "", 0, "", 0f, 0f, "", 0, 0f, 0f, "", "atkSpeed:0.9", "快", 5f, "", 0, 0f, 0f, 0f, 0, "SkillSelfBuff", "saw", "StarExplosionBlue", "", "", "", "", "", "");
            config[2020320] = new SkillBaseConfig(2020320, "迅射", "凌统", 1.0f, "", "自增", "自身强化", new string[]{"加攻速大"}, 5, 0f, 3f, 60, 0f, "", 0, "", 0f, 0f, "", 0, 0f, 0f, "", "atkSpeed:1.2", "快", 5f, "", 0, 0f, 0f, 0f, 0, "SkillSelfBuff", "saw", "StarExplosionBlue", "", "", "", "", "", "");
            config[2020321] = new SkillBaseConfig(2020321, "御守", "高览", 1.2f, "对目标造成/damagestrength法术伤害，并使其移动与攻速降低/strengthbuff1-slow%，持续/bufftime秒", "单伤", "单体伤害小", new string[]{"减速大"}, 1, 0f, 1.5f, 50, 0f, "", 0, "", 30f, 0f, "", 0, 60f, 0f, "", "slow:0.3", "缓", 3f, "", 0, 0f, 0f, 0f, 0, "SkillSingleDamage", "sway", "MagicChargeYellow", "FrostExplosionBlue", "", "", "", "", "");
            config[2020322] = new SkillBaseConfig(2020322, "御守", "高览", 1.2f, "", "单伤", "单体伤害小", new string[]{"减速大"}, 2, 0f, 1.5f, 50, 0f, "", 0, "", 30f, 0f, "", 0, 90f, 0f, "", "slow:0.35", "缓", 3f, "", 0, 0f, 0f, 0f, 0, "SkillSingleDamage", "sway", "MagicChargeYellow", "FrostExplosionBlue", "", "", "", "", "");
            config[2020323] = new SkillBaseConfig(2020323, "御守", "高览", 1.2f, "", "单伤", "单体伤害小", new string[]{"减速大"}, 3, 0f, 1.5f, 50, 0f, "", 0, "", 30f, 0f, "", 0, 150f, 0f, "", "slow:0.42", "缓", 3f, "", 0, 0f, 0f, 0f, 0, "SkillSingleDamage", "sway", "MagicChargeYellow", "FrostExplosionBlue", "", "", "", "", "");
            config[2020324] = new SkillBaseConfig(2020324, "御守", "高览", 1.2f, "", "单伤", "单体伤害小", new string[]{"减速大"}, 4, 0f, 1.5f, 50, 0f, "", 0, "", 30f, 0f, "", 0, 240f, 0f, "", "slow:0.5", "缓", 3f, "", 0, 0f, 0f, 0f, 0, "SkillSingleDamage", "sway", "MagicChargeYellow", "FrostExplosionBlue", "", "", "", "", "");
            config[2020325] = new SkillBaseConfig(2020325, "御守", "高览", 1.2f, "", "单伤", "单体伤害小", new string[]{"减速大"}, 5, 0f, 1.5f, 50, 0f, "", 0, "", 30f, 0f, "", 0, 360f, 0f, "", "slow:0.6", "缓", 3f, "", 0, 0f, 0f, 0f, 0, "SkillSingleDamage", "sway", "MagicChargeYellow", "FrostExplosionBlue", "", "", "", "", "");
            config[2020326] = new SkillBaseConfig(2020326, "毒箭", "张任", 1.0f, "造成/damagestrength点法术伤害，并附加每秒/strengthbuff1-dot点中毒伤害，持续/bufftime秒", "单伤", "单体伤害小", new string[]{}, 1, 0f, 1.5f, 50, 0f, "", 0, "", 30f, 0f, "", 0, 60f, 0f, "", "dot:12", "败", 4f, "", 0, 0f, 0f, 0f, 0, "SkillSingleDamage", "throw", "MagicChargeBlue", "LightningExplosionRed", "", "", "", "", "");
            config[2020327] = new SkillBaseConfig(2020327, "毒箭", "张任", 1.0f, "", "单伤", "单体伤害小", new string[]{}, 2, 0f, 1.5f, 50, 0f, "", 0, "", 30f, 0f, "", 0, 90f, 0f, "", "dot:16", "败", 4f, "", 0, 0f, 0f, 0f, 0, "SkillSingleDamage", "throw", "MagicChargeBlue", "LightningExplosionRed", "", "", "", "", "");
            config[2020328] = new SkillBaseConfig(2020328, "毒箭", "张任", 1.0f, "", "单伤", "单体伤害小", new string[]{}, 3, 0f, 1.5f, 50, 0f, "", 0, "", 30f, 0f, "", 0, 150f, 0f, "", "dot:22", "败", 4f, "", 0, 0f, 0f, 0f, 0, "SkillSingleDamage", "throw", "MagicChargeBlue", "LightningExplosionRed", "", "", "", "", "");
            config[2020329] = new SkillBaseConfig(2020329, "毒箭", "张任", 1.0f, "", "单伤", "单体伤害小", new string[]{}, 4, 0f, 1.5f, 50, 0f, "", 0, "", 30f, 0f, "", 0, 240f, 0f, "", "dot:30", "败", 4f, "", 0, 0f, 0f, 0f, 0, "SkillSingleDamage", "throw", "MagicChargeBlue", "LightningExplosionRed", "", "", "", "", "");
            config[2020330] = new SkillBaseConfig(2020330, "毒箭", "张任", 1.0f, "", "单伤", "单体伤害小", new string[]{}, 5, 0f, 1.5f, 50, 0f, "", 0, "", 30f, 0f, "", 0, 360f, 0f, "", "dot:40", "败", 4f, "", 0, 0f, 0f, 0f, 0, "SkillSingleDamage", "throw", "MagicChargeBlue", "LightningExplosionRed", "", "", "", "", "");
            config[2020331] = new SkillBaseConfig(2020331, "疑城", "徐盛", 1.2f, "进入强攻状态，造成的伤害提升/strengthbuff1-damageUp%，持续/bufftime秒", "自增", "自身强化", new string[]{"自身增伤大"}, 1, 1f, 3f, 60, 0f, "", 0, "", 0f, 0f, "", 0, 0f, 0f, "", "damageUp:0.25", "重", 5f, "", 0, 0f, 0f, 0f, 0, "SkillSelfBuff", "sway", "MagicChargeBlue", "", "", "", "", "", "");
            config[2020332] = new SkillBaseConfig(2020332, "疑城", "徐盛", 1.2f, "", "自增", "自身强化", new string[]{"自身增伤大"}, 2, 1f, 3f, 60, 0f, "", 0, "", 0f, 0f, "", 0, 0f, 0f, "", "damageUp:0.35", "重", 5f, "", 0, 0f, 0f, 0f, 0, "SkillSelfBuff", "sway", "MagicChargeBlue", "", "", "", "", "", "");
            config[2020333] = new SkillBaseConfig(2020333, "疑城", "徐盛", 1.2f, "", "自增", "自身强化", new string[]{"自身增伤大"}, 3, 1f, 3f, 60, 0f, "", 0, "", 0f, 0f, "", 0, 0f, 0f, "", "damageUp:0.5", "重", 5f, "", 0, 0f, 0f, 0f, 0, "SkillSelfBuff", "sway", "MagicChargeBlue", "", "", "", "", "", "");
            config[2020334] = new SkillBaseConfig(2020334, "疑城", "徐盛", 1.2f, "", "自增", "自身强化", new string[]{"自身增伤大"}, 4, 1f, 3f, 60, 0f, "", 0, "", 0f, 0f, "", 0, 0f, 0f, "", "damageUp:0.7", "重", 5f, "", 0, 0f, 0f, 0f, 0, "SkillSelfBuff", "sway", "MagicChargeBlue", "", "", "", "", "", "");
            config[2020335] = new SkillBaseConfig(2020335, "疑城", "徐盛", 1.2f, "", "自增", "自身强化", new string[]{"自身增伤大"}, 5, 1f, 3f, 60, 0f, "", 0, "", 0f, 0f, "", 0, 0f, 0f, "", "damageUp:0.95", "重", 5f, "", 0, 0f, 0f, 0f, 0, "SkillSelfBuff", "sway", "MagicChargeBlue", "", "", "", "", "", "");
            config[2020336] = new SkillBaseConfig(2020336, "楼船", "王浑", 1.2f, "对目标造成/damagestrength法术伤害", "单伤", "单体伤害大", new string[]{}, 1, 1f, 2f, 70, 0f, "", 0, "", 35f, 0f, "", 0, 100f, 0f, "", "", "", 0f, "", 0, 0f, 0f, 0f, 0, "SkillSingleDamage", "throw", "MagicChargePink", "LightningExplosionYellow", "", "", "", "", "");
            config[2020337] = new SkillBaseConfig(2020337, "楼船", "王浑", 1.2f, "", "单伤", "单体伤害大", new string[]{}, 2, 1f, 2f, 70, 0f, "", 0, "", 35f, 0f, "", 0, 150f, 0f, "", "", "", 0f, "", 0, 0f, 0f, 0f, 0, "SkillSingleDamage", "throw", "MagicChargePink", "LightningExplosionYellow", "", "", "", "", "");
            config[2020338] = new SkillBaseConfig(2020338, "楼船", "王浑", 1.2f, "", "单伤", "单体伤害大", new string[]{}, 3, 1f, 2f, 70, 0f, "", 0, "", 35f, 0f, "", 0, 250f, 0f, "", "", "", 0f, "", 0, 0f, 0f, 0f, 0, "SkillSingleDamage", "throw", "MagicChargePink", "LightningExplosionYellow", "", "", "", "", "");
            config[2020339] = new SkillBaseConfig(2020339, "楼船", "王浑", 1.2f, "", "单伤", "单体伤害大", new string[]{}, 4, 1f, 2f, 70, 0f, "", 0, "", 35f, 0f, "", 0, 400f, 0f, "", "", "", 0f, "", 0, 0f, 0f, 0f, 0, "SkillSingleDamage", "throw", "MagicChargePink", "LightningExplosionYellow", "", "", "", "", "");
            config[2020340] = new SkillBaseConfig(2020340, "楼船", "王浑", 1.2f, "", "单伤", "单体伤害大", new string[]{}, 5, 1f, 2f, 70, 0f, "", 0, "", 35f, 0f, "", 0, 600f, 0f, "", "", "", 0f, "", 0, 0f, 0f, 0f, 0, "SkillSingleDamage", "throw", "MagicChargePink", "LightningExplosionYellow", "", "", "", "", "");
            config[2020401] = new SkillBaseConfig(2020401, "连射", "马岱", 1.0f, "每次发动永久提升自身/strength2-selfAtkSpeed%攻击速度", "自增", "自身强化", new string[]{}, 1, 0f, 3f, 60, 0f, "", 0, "", 0f, 0f, "permanent", 0, 0f, 0f, "selfAtkSpeed:0.08", "", "", 0f, "", 0, 0f, 0f, 0f, 0, "SkillSelfBuff", "throw", "MagicChargeGreen", "", "", "", "", "", "");
            config[2020402] = new SkillBaseConfig(2020402, "连射", "马岱", 1.0f, "", "自增", "自身强化", new string[]{}, 2, 0f, 3f, 60, 0f, "", 0, "", 0f, 0f, "permanent", 0, 0f, 0f, "selfAtkSpeed:0.1", "", "", 0f, "", 0, 0f, 0f, 0f, 0, "SkillSelfBuff", "throw", "MagicChargeGreen", "", "", "", "", "", "");
            config[2020403] = new SkillBaseConfig(2020403, "连射", "马岱", 1.0f, "", "自增", "自身强化", new string[]{}, 3, 0f, 3f, 60, 0f, "", 0, "", 0f, 0f, "permanent", 0, 0f, 0f, "selfAtkSpeed:0.12", "", "", 0f, "", 0, 0f, 0f, 0f, 0, "SkillSelfBuff", "throw", "MagicChargeGreen", "", "", "", "", "", "");
            config[2020404] = new SkillBaseConfig(2020404, "连射", "马岱", 1.0f, "", "自增", "自身强化", new string[]{}, 4, 0f, 3f, 60, 0f, "", 0, "", 0f, 0f, "permanent", 0, 0f, 0f, "selfAtkSpeed:0.14", "", "", 0f, "", 0, 0f, 0f, 0f, 0, "SkillSelfBuff", "throw", "MagicChargeGreen", "", "", "", "", "", "");
            config[2020405] = new SkillBaseConfig(2020405, "连射", "马岱", 1.0f, "", "自增", "自身强化", new string[]{}, 5, 0f, 3f, 60, 0f, "", 0, "", 0f, 0f, "permanent", 0, 0f, 0f, "selfAtkSpeed:0.17", "", "", 0f, "", 0, 0f, 0f, 0f, 0, "SkillSelfBuff", "throw", "MagicChargeGreen", "", "", "", "", "", "");
            config[2020426] = new SkillBaseConfig(2020426, "贯穿箭", "鞠义", 1.0f, "向目标位置射出贯穿箭，对沿途最多/targetcount名敌人各造成/damagestrength法术伤害", "群伤", "群体伤害近距离", new string[]{}, 1, 0f, 2f, 70, 0f, "", 0, "", 40f, 20f, "", 3, 70f, 0f, "", "", "", 0f, "", 0, 2f, 0f, 55f, 0, "SkillShockWave", "jumpspin", "StarExplosionBlue", "SwordWaveBlue", "", "", "", "", "");
            config[2020427] = new SkillBaseConfig(2020427, "贯穿箭", "鞠义", 1.0f, "", "群伤", "群体伤害近距离", new string[]{}, 2, 0f, 2f, 70, 0f, "", 0, "", 40f, 20f, "", 3, 105f, 0f, "", "", "", 0f, "", 0, 2f, 0f, 55f, 0, "SkillShockWave", "jumpspin", "StarExplosionBlue", "SwordWaveBlue", "", "", "", "", "");
            config[2020428] = new SkillBaseConfig(2020428, "贯穿箭", "鞠义", 1.0f, "", "群伤", "群体伤害近距离", new string[]{}, 3, 0f, 2f, 70, 0f, "", 0, "", 40f, 20f, "", 3, 175f, 0f, "", "", "", 0f, "", 0, 2f, 0f, 55f, 0, "SkillShockWave", "jumpspin", "StarExplosionBlue", "SwordWaveBlue", "", "", "", "", "");
            config[2020429] = new SkillBaseConfig(2020429, "贯穿箭", "鞠义", 1.0f, "", "群伤", "群体伤害近距离", new string[]{}, 4, 0f, 2f, 70, 0f, "", 0, "", 40f, 20f, "", 3, 280f, 0f, "", "", "", 0f, "", 0, 2f, 0f, 55f, 0, "SkillShockWave", "jumpspin", "StarExplosionBlue", "SwordWaveBlue", "", "", "", "", "");
            config[2020430] = new SkillBaseConfig(2020430, "贯穿箭", "鞠义", 1.0f, "", "群伤", "群体伤害近距离", new string[]{}, 5, 0f, 2f, 70, 0f, "", 0, "", 40f, 20f, "", 3, 420f, 0f, "", "", "", 0f, "", 0, 2f, 0f, 55f, 0, "SkillShockWave", "jumpspin", "StarExplosionBlue", "SwordWaveBlue", "", "", "", "", "");
            config[2020431] = new SkillBaseConfig(2020431, "飞刀", "祝融", 1.1f, "向/area范围内最多/targetcount个敌人乱射，造成/damagestrength法术伤害，附/strength2-missileRate%暴击加成", "群伤", "群体伤害近距离", new string[]{}, 1, 0f, 2f, 70, 0f, "", 0, "", 40f, 20f, "missile", 3, 70f, 0f, "missileRate:0.5", "", "", 0f, "", 0, 0f, 0f, 0f, 0, "SkillAreaDamageDeBuff", "throw", "MagicChargeYellow", "BulletExplosionFire", "PoisonExplosionSoftYellow", "", "", "", "");
            config[2020432] = new SkillBaseConfig(2020432, "飞刀", "祝融", 1.1f, "", "群伤", "群体伤害近距离", new string[]{}, 2, 0f, 2f, 70, 0f, "", 0, "", 40f, 20f, "missile", 3, 105f, 0f, "missileRate:0.6", "", "", 0f, "", 0, 0f, 0f, 0f, 0, "SkillAreaDamageDeBuff", "throw", "MagicChargeYellow", "BulletExplosionFire", "PoisonExplosionSoftYellow", "", "", "", "");
            config[2020433] = new SkillBaseConfig(2020433, "飞刀", "祝融", 1.1f, "", "群伤", "群体伤害近距离", new string[]{}, 3, 0f, 2f, 70, 0f, "", 0, "", 40f, 20f, "missile", 3, 175f, 0f, "missileRate:0.7", "", "", 0f, "", 0, 0f, 0f, 0f, 0, "SkillAreaDamageDeBuff", "throw", "MagicChargeYellow", "BulletExplosionFire", "PoisonExplosionSoftYellow", "", "", "", "");
            config[2020434] = new SkillBaseConfig(2020434, "飞刀", "祝融", 1.1f, "", "群伤", "群体伤害近距离", new string[]{}, 4, 0f, 2f, 70, 0f, "", 0, "", 40f, 20f, "missile", 3, 280f, 0f, "missileRate:0.8", "", "", 0f, "", 0, 0f, 0f, 0f, 0, "SkillAreaDamageDeBuff", "throw", "MagicChargeYellow", "BulletExplosionFire", "PoisonExplosionSoftYellow", "", "", "", "");
            config[2020435] = new SkillBaseConfig(2020435, "飞刀", "祝融", 1.1f, "", "群伤", "群体伤害近距离", new string[]{}, 5, 0f, 2f, 70, 0f, "", 0, "", 40f, 20f, "missile", 3, 420f, 0f, "missileRate:1", "", "", 0f, "", 0, 0f, 0f, 0f, 0, "SkillAreaDamageDeBuff", "throw", "MagicChargeYellow", "BulletExplosionFire", "PoisonExplosionSoftYellow", "", "", "", "");
            config[2020501] = new SkillBaseConfig(2020501, "困敌", "徐庶", 1.0f, "对/area范围内最多/targetcount个敌人造成/damagestrength法术伤害，并降低其/strengthbuff1-atkDown点攻击与/strength2-healDown%治疗，持续/bufftime秒", "群伤", "群体伤害近距离", new string[]{"减攻中","减疗中"}, 1, 0f, 2f, 70, 0f, "", 0, "", 40f, 20f, "", 3, 70f, 0f, "healDown:0.2", "atkDown:15", "慑,疫", 4f, "", 0, 0f, 0f, 0f, 0, "SkillAreaDamageDeBuff", "spin", "MagicChargePink", "GasExplosionFire", "NukeVerticalExplosionFire", "", "", "", "");
            config[2020502] = new SkillBaseConfig(2020502, "困敌", "徐庶", 1.0f, "", "群伤", "群体伤害近距离", new string[]{"减攻中","减疗中"}, 2, 0f, 2f, 70, 0f, "", 0, "", 40f, 20f, "", 3, 105f, 0f, "healDown:0.25", "atkDown:20", "慑,疫", 4f, "", 0, 0f, 0f, 0f, 0, "SkillAreaDamageDeBuff", "spin", "MagicChargePink", "GasExplosionFire", "NukeVerticalExplosionFire", "", "", "", "");
            config[2020503] = new SkillBaseConfig(2020503, "困敌", "徐庶", 1.0f, "", "群伤", "群体伤害近距离", new string[]{"减攻中","减疗中"}, 3, 0f, 2f, 70, 0f, "", 0, "", 40f, 20f, "", 3, 175f, 0f, "healDown:0.3", "atkDown:28", "慑,疫", 4f, "", 0, 0f, 0f, 0f, 0, "SkillAreaDamageDeBuff", "spin", "MagicChargePink", "GasExplosionFire", "NukeVerticalExplosionFire", "", "", "", "");
            config[2020504] = new SkillBaseConfig(2020504, "困敌", "徐庶", 1.0f, "", "群伤", "群体伤害近距离", new string[]{"减攻中","减疗中"}, 4, 0f, 2f, 70, 0f, "", 0, "", 40f, 20f, "", 3, 280f, 0f, "healDown:0.35", "atkDown:38", "慑,疫", 4f, "", 0, 0f, 0f, 0f, 0, "SkillAreaDamageDeBuff", "spin", "MagicChargePink", "GasExplosionFire", "NukeVerticalExplosionFire", "", "", "", "");
            config[2020505] = new SkillBaseConfig(2020505, "困敌", "徐庶", 1.0f, "", "群伤", "群体伤害近距离", new string[]{"减攻中","减疗中"}, 5, 0f, 2f, 70, 0f, "", 0, "", 40f, 20f, "", 3, 420f, 0f, "healDown:0.4", "atkDown:50", "慑,疫", 4f, "", 0, 0f, 0f, 0f, 0, "SkillAreaDamageDeBuff", "spin", "MagicChargePink", "GasExplosionFire", "NukeVerticalExplosionFire", "", "", "", "");
            config[2020516] = new SkillBaseConfig(2020516, "奋威", "丁奉", 1.2f, "短时间内攻速提升/strengthbuff1-atkSpeed%，持续/bufftime秒", "自增", "自身强化", new string[]{"加攻速大"}, 1, 0f, 3f, 60, 0f, "", 0, "", 0f, 0f, "", 0, 0f, 0f, "", "atkSpeed:0.25", "快", 5f, "", 0, 0f, 0f, 0f, 0, "SkillSelfBuff", "spin", "ShieldSoftBlue", "", "", "", "", "", "");
            config[2020517] = new SkillBaseConfig(2020517, "奋威", "丁奉", 1.2f, "", "自增", "自身强化", new string[]{"加攻速大"}, 2, 0f, 3f, 60, 0f, "", 0, "", 0f, 0f, "", 0, 0f, 0f, "", "atkSpeed:0.35", "快", 5f, "", 0, 0f, 0f, 0f, 0, "SkillSelfBuff", "spin", "ShieldSoftBlue", "", "", "", "", "", "");
            config[2020518] = new SkillBaseConfig(2020518, "奋威", "丁奉", 1.2f, "", "自增", "自身强化", new string[]{"加攻速大"}, 3, 0f, 3f, 60, 0f, "", 0, "", 0f, 0f, "", 0, 0f, 0f, "", "atkSpeed:0.5", "快", 5f, "", 0, 0f, 0f, 0f, 0, "SkillSelfBuff", "spin", "ShieldSoftBlue", "", "", "", "", "", "");
            config[2020519] = new SkillBaseConfig(2020519, "奋威", "丁奉", 1.2f, "", "自增", "自身强化", new string[]{"加攻速大"}, 4, 0f, 3f, 60, 0f, "", 0, "", 0f, 0f, "", 0, 0f, 0f, "", "atkSpeed:0.65", "快", 5f, "", 0, 0f, 0f, 0f, 0, "SkillSelfBuff", "spin", "ShieldSoftBlue", "", "", "", "", "", "");
            config[2020520] = new SkillBaseConfig(2020520, "奋威", "丁奉", 1.2f, "", "自增", "自身强化", new string[]{"加攻速大"}, 5, 0f, 3f, 60, 0f, "", 0, "", 0f, 0f, "", 0, 0f, 0f, "", "atkSpeed:0.8", "快", 5f, "", 0, 0f, 0f, 0f, 0, "SkillSelfBuff", "spin", "ShieldSoftBlue", "", "", "", "", "", "");
            config[2020526] = new SkillBaseConfig(2020526, "疾击", "王濬", 1.0f, "短时间内攻击速度提升/strengthbuff1-atkSpeed%，持续/bufftime秒", "自增", "自身强化", new string[]{"加攻速大"}, 1, 0f, 3f, 60, 0f, "", 0, "", 0f, 0f, "", 0, 0f, 0f, "", "atkSpeed:0.25", "快", 5f, "", 0, 0f, 0f, 0f, 0, "SkillSelfBuff", "saw", "MagicChargeBlue", "", "", "", "", "", "");
            config[2020527] = new SkillBaseConfig(2020527, "疾击", "王濬", 1.0f, "", "自增", "自身强化", new string[]{"加攻速大"}, 2, 0f, 3f, 60, 0f, "", 0, "", 0f, 0f, "", 0, 0f, 0f, "", "atkSpeed:0.3", "快", 5f, "", 0, 0f, 0f, 0f, 0, "SkillSelfBuff", "saw", "MagicChargeBlue", "", "", "", "", "", "");
            config[2020528] = new SkillBaseConfig(2020528, "疾击", "王濬", 1.0f, "", "自增", "自身强化", new string[]{"加攻速大"}, 3, 0f, 3f, 60, 0f, "", 0, "", 0f, 0f, "", 0, 0f, 0f, "", "atkSpeed:0.36", "快", 5f, "", 0, 0f, 0f, 0f, 0, "SkillSelfBuff", "saw", "MagicChargeBlue", "", "", "", "", "", "");
            config[2020529] = new SkillBaseConfig(2020529, "疾击", "王濬", 1.0f, "", "自增", "自身强化", new string[]{"加攻速大"}, 4, 0f, 3f, 60, 0f, "", 0, "", 0f, 0f, "", 0, 0f, 0f, "", "atkSpeed:0.42", "快", 5f, "", 0, 0f, 0f, 0f, 0, "SkillSelfBuff", "saw", "MagicChargeBlue", "", "", "", "", "", "");
            config[2020530] = new SkillBaseConfig(2020530, "疾击", "王濬", 1.0f, "", "自增", "自身强化", new string[]{"加攻速大"}, 5, 0f, 3f, 60, 0f, "", 0, "", 0f, 0f, "", 0, 0f, 0f, "", "atkSpeed:0.5", "快", 5f, "", 0, 0f, 0f, 0f, 0, "SkillSelfBuff", "saw", "MagicChargeBlue", "", "", "", "", "", "");
            config[2020531] = new SkillBaseConfig(2020531, "苦肉", "黄盖", 1.1f, "对目标造成/damagestrength物理伤害，同时自身受到该伤害/strength2-selfHurt%的反噬（濒死时保留1点生命）", "单伤", "单体伤害中", new string[]{"自损"}, 1, 0f, 2f, 60, 0f, "", 1, "", 40f, 0f, "selfHurt", 0, 80f, 0f, "selfHurt:0.2", "", "", 0f, "", 0, 0f, 0f, 0f, 0, "SkillSingleDamage", "spin", "MagicChargeRed", "BloodExplosion", "", "", "", "", "");
            config[2020532] = new SkillBaseConfig(2020532, "苦肉", "黄盖", 1.1f, "", "单伤", "单体伤害中", new string[]{"自损"}, 2, 0f, 2f, 60, 0f, "", 1, "", 40f, 0f, "selfHurt", 0, 120f, 0f, "selfHurt:0.2", "", "", 0f, "", 0, 0f, 0f, 0f, 0, "SkillSingleDamage", "spin", "MagicChargeRed", "BloodExplosion", "", "", "", "", "");
            config[2020533] = new SkillBaseConfig(2020533, "苦肉", "黄盖", 1.1f, "", "单伤", "单体伤害中", new string[]{"自损"}, 3, 0f, 2f, 60, 0f, "", 1, "", 40f, 0f, "selfHurt", 0, 200f, 0f, "selfHurt:0.2", "", "", 0f, "", 0, 0f, 0f, 0f, 0, "SkillSingleDamage", "spin", "MagicChargeRed", "BloodExplosion", "", "", "", "", "");
            config[2020534] = new SkillBaseConfig(2020534, "苦肉", "黄盖", 1.1f, "", "单伤", "单体伤害中", new string[]{"自损"}, 4, 0f, 2f, 60, 0f, "", 1, "", 40f, 0f, "selfHurt", 0, 320f, 0f, "selfHurt:0.2", "", "", 0f, "", 0, 0f, 0f, 0f, 0, "SkillSingleDamage", "spin", "MagicChargeRed", "BloodExplosion", "", "", "", "", "");
            config[2020535] = new SkillBaseConfig(2020535, "苦肉", "黄盖", 1.1f, "", "单伤", "单体伤害中", new string[]{"自损"}, 5, 0f, 2f, 60, 0f, "", 1, "", 40f, 0f, "selfHurt", 0, 480f, 0f, "selfHurt:0.2", "", "", 0f, "", 0, 0f, 0f, 0f, 0, "SkillSingleDamage", "spin", "MagicChargeRed", "BloodExplosion", "", "", "", "", "");
            config[2020601] = new SkillBaseConfig(2020601, "御阵", "李典", 1.0f, "为自己及周围最多/targetcount名友军附加/strengthbuff1-armor点护甲，持续/bufftime秒", "群增", "群体友方增益", new string[]{"加护甲大","施法者必含"}, 1, 0f, 4f, 80, 0f, "", 0, "", 50f, 20f, "selfAlways", 3, 0f, 0f, "", "armor:10", "甲", 5f, "", 0, 0f, 0f, 0f, 0, "SkillAreaBuff", "jumpspin", "MagicChargeGreen", "ShieldSoftBlue", "MagicAuraYellow", "", "", "", "");
            config[2020602] = new SkillBaseConfig(2020602, "御阵", "李典", 1.0f, "", "群增", "群体友方增益", new string[]{"加护甲大","施法者必含"}, 2, 0f, 4f, 80, 0f, "", 0, "", 50f, 20f, "selfAlways", 3, 0f, 0f, "", "armor:15", "甲", 5f, "", 0, 0f, 0f, 0f, 0, "SkillAreaBuff", "jumpspin", "MagicChargeGreen", "ShieldSoftBlue", "MagicAuraYellow", "", "", "", "");
            config[2020603] = new SkillBaseConfig(2020603, "御阵", "李典", 1.0f, "", "群增", "群体友方增益", new string[]{"加护甲大","施法者必含"}, 3, 0f, 4f, 80, 0f, "", 0, "", 50f, 20f, "selfAlways", 3, 0f, 0f, "", "armor:22", "甲", 5f, "", 0, 0f, 0f, 0f, 0, "SkillAreaBuff", "jumpspin", "MagicChargeGreen", "ShieldSoftBlue", "MagicAuraYellow", "", "", "", "");
            config[2020604] = new SkillBaseConfig(2020604, "御阵", "李典", 1.0f, "", "群增", "群体友方增益", new string[]{"加护甲大","施法者必含"}, 4, 0f, 4f, 80, 0f, "", 0, "", 50f, 20f, "selfAlways", 3, 0f, 0f, "", "armor:30", "甲", 5f, "", 0, 0f, 0f, 0f, 0, "SkillAreaBuff", "jumpspin", "MagicChargeGreen", "ShieldSoftBlue", "MagicAuraYellow", "", "", "", "");
            config[2020605] = new SkillBaseConfig(2020605, "御阵", "李典", 1.0f, "", "群增", "群体友方增益", new string[]{"加护甲大","施法者必含"}, 5, 0f, 4f, 80, 0f, "", 0, "", 50f, 20f, "selfAlways", 3, 0f, 0f, "", "armor:40", "甲", 5f, "", 0, 0f, 0f, 0f, 0, "SkillAreaBuff", "jumpspin", "MagicChargeGreen", "ShieldSoftBlue", "MagicAuraYellow", "", "", "", "");
            config[2020606] = new SkillBaseConfig(2020606, "护军", "李严", 1.0f, "为生命最低的友军套/strength2-shield点护盾（随法强成长），并嘲讽其/area范围内敌人转移到自己身上", "单增", "单体友方增益", new string[]{"护盾中","低血筛选","全图","嘲讽周围","不按首段去重"}, 1, 0f, 3f, 60, 0f, "", 0, "", 50f, 25f, "lowestHp,noDedupe,wholeMap,tauntAroundTarget", 1, 0f, 0f, "shield:200", "", "盾", 5f, "", 0, 0f, 0f, 0f, 0, "SkillAreaBuff", "sway", "MagicChargeBlue", "", "", "", "", "", "");
            config[2020607] = new SkillBaseConfig(2020607, "护军", "李严", 1.0f, "", "单增", "单体友方增益", new string[]{"护盾中","低血筛选","全图","嘲讽周围","不按首段去重"}, 2, 0f, 3f, 60, 0f, "", 0, "", 50f, 25f, "lowestHp,noDedupe,wholeMap,tauntAroundTarget", 1, 0f, 0f, "shield:250", "", "盾", 5f, "", 0, 0f, 0f, 0f, 0, "SkillAreaBuff", "sway", "MagicChargeBlue", "", "", "", "", "", "");
            config[2020608] = new SkillBaseConfig(2020608, "护军", "李严", 1.0f, "", "单增", "单体友方增益", new string[]{"护盾中","低血筛选","全图","嘲讽周围","不按首段去重"}, 3, 0f, 3f, 60, 0f, "", 0, "", 50f, 25f, "lowestHp,noDedupe,wholeMap,tauntAroundTarget", 1, 0f, 0f, "shield:300", "", "盾", 5f, "", 0, 0f, 0f, 0f, 0, "SkillAreaBuff", "sway", "MagicChargeBlue", "", "", "", "", "", "");
            config[2020609] = new SkillBaseConfig(2020609, "护军", "李严", 1.0f, "", "单增", "单体友方增益", new string[]{"护盾中","低血筛选","全图","嘲讽周围","不按首段去重"}, 4, 0f, 3f, 60, 0f, "", 0, "", 50f, 25f, "lowestHp,noDedupe,wholeMap,tauntAroundTarget", 1, 0f, 0f, "shield:350", "", "盾", 5f, "", 0, 0f, 0f, 0f, 0, "SkillAreaBuff", "sway", "MagicChargeBlue", "", "", "", "", "", "");
            config[2020610] = new SkillBaseConfig(2020610, "护军", "李严", 1.0f, "", "单增", "单体友方增益", new string[]{"护盾中","低血筛选","全图","嘲讽周围","不按首段去重"}, 5, 0f, 3f, 60, 0f, "", 0, "", 50f, 25f, "lowestHp,noDedupe,wholeMap,tauntAroundTarget", 1, 0f, 0f, "shield:400", "", "盾", 5f, "", 0, 0f, 0f, 0f, 0, "SkillAreaBuff", "sway", "MagicChargeBlue", "", "", "", "", "", "");
            config[2020621] = new SkillBaseConfig(2020621, "黄天", "张梁", 1.2f, "为我方生命比例最低的未满血英雄附加/strengthbuff1-shieldReduce%减伤盾，持续/bufftime秒", "单增", "单体友方增益", new string[]{"减伤大","低血筛选","排除自身","满血跳过","不按首段去重","英雄限定"}, 1, 1f, 3f, 60, 1f, "", 0, "", 50f, 60f, "heroOnly,excludeSelf,lowestHp,fullHpSkip,noDedupe", 1, 0f, 0f, "", "shieldReduce:0.15", "硬", 5f, "", 0, 0f, 0f, 0f, 0, "SkillAreaBuff", "sway", "MagicChargePink", "", "", "", "", "", "");
            config[2020622] = new SkillBaseConfig(2020622, "黄天", "张梁", 1.2f, "", "单增", "单体友方增益", new string[]{"减伤大","低血筛选","排除自身","满血跳过","不按首段去重","英雄限定"}, 2, 1f, 3f, 60, 1f, "", 0, "", 50f, 60f, "heroOnly,excludeSelf,lowestHp,fullHpSkip,noDedupe", 1, 0f, 0f, "", "shieldReduce:0.2", "硬", 5f, "", 0, 0f, 0f, 0f, 0, "SkillAreaBuff", "sway", "MagicChargePink", "", "", "", "", "", "");
            config[2020623] = new SkillBaseConfig(2020623, "黄天", "张梁", 1.2f, "", "单增", "单体友方增益", new string[]{"减伤大","低血筛选","排除自身","满血跳过","不按首段去重","英雄限定"}, 3, 1f, 3f, 60, 1f, "", 0, "", 50f, 60f, "heroOnly,excludeSelf,lowestHp,fullHpSkip,noDedupe", 1, 0f, 0f, "", "shieldReduce:0.27", "硬", 5f, "", 0, 0f, 0f, 0f, 0, "SkillAreaBuff", "sway", "MagicChargePink", "", "", "", "", "", "");
            config[2020624] = new SkillBaseConfig(2020624, "黄天", "张梁", 1.2f, "", "单增", "单体友方增益", new string[]{"减伤大","低血筛选","排除自身","满血跳过","不按首段去重","英雄限定"}, 4, 1f, 3f, 60, 1f, "", 0, "", 50f, 60f, "heroOnly,excludeSelf,lowestHp,fullHpSkip,noDedupe", 1, 0f, 0f, "", "shieldReduce:0.35", "硬", 5f, "", 0, 0f, 0f, 0f, 0, "SkillAreaBuff", "sway", "MagicChargePink", "", "", "", "", "", "");
            config[2020625] = new SkillBaseConfig(2020625, "黄天", "张梁", 1.2f, "", "单增", "单体友方增益", new string[]{"减伤大","低血筛选","排除自身","满血跳过","不按首段去重","英雄限定"}, 5, 1f, 3f, 60, 1f, "", 0, "", 50f, 60f, "heroOnly,excludeSelf,lowestHp,fullHpSkip,noDedupe", 1, 0f, 0f, "", "shieldReduce:0.45", "硬", 5f, "", 0, 0f, 0f, 0f, 0, "SkillAreaBuff", "sway", "MagicChargePink", "", "", "", "", "", "");
            config[2020626] = new SkillBaseConfig(2020626, "死守", "张悌", 1.1f, "为自身附加/strength2-shield点护盾（随法强提升），并提升/strengthbuff1-armor点护甲，持续/bufftime秒", "自增", "自身强化", new string[]{"护盾中","加护甲大"}, 1, 0f, 3f, 60, 0f, "", 0, "", 0f, 0f, "", 0, 0f, 0f, "shield:80", "armor:10", "盾,甲", 5f, "", 0, 0f, 0f, 0f, 0, "SkillSelfBuff", "sway", "MagicBuffGreen", "", "", "", "", "", "");
            config[2020627] = new SkillBaseConfig(2020627, "死守", "张悌", 1.1f, "", "自增", "自身强化", new string[]{"护盾中","加护甲大"}, 2, 0f, 3f, 60, 0f, "", 0, "", 0f, 0f, "", 0, 0f, 0f, "shield:110", "armor:14", "盾,甲", 5f, "", 0, 0f, 0f, 0f, 0, "SkillSelfBuff", "sway", "MagicBuffGreen", "", "", "", "", "", "");
            config[2020628] = new SkillBaseConfig(2020628, "死守", "张悌", 1.1f, "", "自增", "自身强化", new string[]{"护盾中","加护甲大"}, 3, 0f, 3f, 60, 0f, "", 0, "", 0f, 0f, "", 0, 0f, 0f, "shield:150", "armor:18", "盾,甲", 5f, "", 0, 0f, 0f, 0f, 0, "SkillSelfBuff", "sway", "MagicBuffGreen", "", "", "", "", "", "");
            config[2020629] = new SkillBaseConfig(2020629, "死守", "张悌", 1.1f, "", "自增", "自身强化", new string[]{"护盾中","加护甲大"}, 4, 0f, 3f, 60, 0f, "", 0, "", 0f, 0f, "", 0, 0f, 0f, "shield:200", "armor:24", "盾,甲", 5f, "", 0, 0f, 0f, 0f, 0, "SkillSelfBuff", "sway", "MagicBuffGreen", "", "", "", "", "", "");
            config[2020630] = new SkillBaseConfig(2020630, "死守", "张悌", 1.1f, "", "自增", "自身强化", new string[]{"护盾中","加护甲大"}, 5, 0f, 3f, 60, 0f, "", 0, "", 0f, 0f, "", 0, 0f, 0f, "shield:260", "armor:30", "盾,甲", 5f, "", 0, 0f, 0f, 0f, 0, "SkillSelfBuff", "sway", "MagicBuffGreen", "", "", "", "", "", "");
            config[2020641] = new SkillBaseConfig(2020641, "江陵", "朱然", 1.1f, "受击时反弹自身受到伤害的/strength2-reflect%给/range范围内攻击者（冷却/cd秒）", "自增", "自身强化", new string[]{"伤害反射"}, 1, 0f, 3f, 60, 0f, "", 0, "", 12f, 0f, "", 0, 0f, 0f, "reflect:0.12", "", "", 0f, "", 0, 0f, 0f, 0f, 0, "SkillDefFeedback", "", "", "SwordSlashMiniWhite", "", "ci", "", "", "");
            config[2020642] = new SkillBaseConfig(2020642, "江陵", "朱然", 1.1f, "", "自增", "自身强化", new string[]{"伤害反射"}, 2, 0f, 3f, 60, 0f, "", 0, "", 12f, 0f, "", 0, 0f, 0f, "reflect:0.16", "", "", 0f, "", 0, 0f, 0f, 0f, 0, "SkillDefFeedback", "", "", "SwordSlashMiniWhite", "", "ci", "", "", "");
            config[2020643] = new SkillBaseConfig(2020643, "江陵", "朱然", 1.1f, "", "自增", "自身强化", new string[]{"伤害反射"}, 3, 0f, 3f, 60, 0f, "", 0, "", 12f, 0f, "", 0, 0f, 0f, "reflect:0.20", "", "", 0f, "", 0, 0f, 0f, 0f, 0, "SkillDefFeedback", "", "", "SwordSlashMiniWhite", "", "ci", "", "", "");
            config[2020644] = new SkillBaseConfig(2020644, "江陵", "朱然", 1.1f, "", "自增", "自身强化", new string[]{"伤害反射"}, 4, 0f, 3f, 60, 0f, "", 0, "", 12f, 0f, "", 0, 0f, 0f, "reflect:0.25", "", "", 0f, "", 0, 0f, 0f, 0f, 0, "SkillDefFeedback", "", "", "SwordSlashMiniWhite", "", "ci", "", "", "");
            config[2020645] = new SkillBaseConfig(2020645, "江陵", "朱然", 1.1f, "", "自增", "自身强化", new string[]{"伤害反射"}, 5, 0f, 3f, 60, 0f, "", 0, "", 12f, 0f, "", 0, 0f, 0f, "reflect:0.30", "", "", 0f, "", 0, 0f, 0f, 0f, 0, "SkillDefFeedback", "", "", "SwordSlashMiniWhite", "", "ci", "", "", "");
            config[2020706] = new SkillBaseConfig(2020706, "忠勇", "周仓", 1.2f, "给自己和附近生命比例最低的友军各提升/strengthbuff1-armor点护甲，持续/bufftime秒", "单增", "单体友方增益", new string[]{"加护甲大","低血筛选","排除自身","施法者必含","满血跳过","不按首段去重","英雄限定"}, 1, 1f, 3f, 60, 0f, "", 0, "", 50f, 60f, "heroOnly,excludeSelf,lowestHp,fullHpSkip,noDedupe,selfAlways", 1, 0f, 0f, "", "armor:60", "甲", 5f, "", 0, 0f, 0f, 0f, 0, "SkillAreaBuff", "sway", "ShieldSoftBlue", "ShieldSoftBlue", "", "", "", "", "");
            config[2020707] = new SkillBaseConfig(2020707, "忠勇", "周仓", 1.2f, "", "单增", "单体友方增益", new string[]{"加护甲大","低血筛选","排除自身","施法者必含","满血跳过","不按首段去重","英雄限定"}, 2, 1f, 3f, 60, 0f, "", 0, "", 50f, 60f, "heroOnly,excludeSelf,lowestHp,fullHpSkip,noDedupe,selfAlways", 1, 0f, 0f, "", "armor:85", "甲", 5f, "", 0, 0f, 0f, 0f, 0, "SkillAreaBuff", "sway", "ShieldSoftBlue", "ShieldSoftBlue", "", "", "", "", "");
            config[2020708] = new SkillBaseConfig(2020708, "忠勇", "周仓", 1.2f, "", "单增", "单体友方增益", new string[]{"加护甲大","低血筛选","排除自身","施法者必含","满血跳过","不按首段去重","英雄限定"}, 3, 1f, 3f, 60, 0f, "", 0, "", 50f, 60f, "heroOnly,excludeSelf,lowestHp,fullHpSkip,noDedupe,selfAlways", 1, 0f, 0f, "", "armor:120", "甲", 5f, "", 0, 0f, 0f, 0f, 0, "SkillAreaBuff", "sway", "ShieldSoftBlue", "ShieldSoftBlue", "", "", "", "", "");
            config[2020709] = new SkillBaseConfig(2020709, "忠勇", "周仓", 1.2f, "", "单增", "单体友方增益", new string[]{"加护甲大","低血筛选","排除自身","施法者必含","满血跳过","不按首段去重","英雄限定"}, 4, 1f, 3f, 60, 0f, "", 0, "", 50f, 60f, "heroOnly,excludeSelf,lowestHp,fullHpSkip,noDedupe,selfAlways", 1, 0f, 0f, "", "armor:165", "甲", 5f, "", 0, 0f, 0f, 0f, 0, "SkillAreaBuff", "sway", "ShieldSoftBlue", "ShieldSoftBlue", "", "", "", "", "");
            config[2020710] = new SkillBaseConfig(2020710, "忠勇", "周仓", 1.2f, "", "单增", "单体友方增益", new string[]{"加护甲大","低血筛选","排除自身","施法者必含","满血跳过","不按首段去重","英雄限定"}, 5, 1f, 3f, 60, 0f, "", 0, "", 50f, 60f, "heroOnly,excludeSelf,lowestHp,fullHpSkip,noDedupe,selfAlways", 1, 0f, 0f, "", "armor:220", "甲", 5f, "", 0, 0f, 0f, 0f, 0, "SkillAreaBuff", "sway", "ShieldSoftBlue", "ShieldSoftBlue", "", "", "", "", "");
            config[2020711] = new SkillBaseConfig(2020711, "老当益壮", "严颜", 1.0f, "生命低于30%受击时获得/strength2-shield点护盾（随法强成长）", "自增", "自身强化", new string[]{"护盾中"}, 1, 1f, 3f, 60, 0f, "hprate<30", 0, "", 0f, 0f, "", 0, 0f, 0f, "shield:180", "", "盾", 5f, "", 0, 0f, 0f, 0f, 0, "SkillAttackedShield", "sway", "MagicChargeGreen", "", "", "", "", "", "");
            config[2020712] = new SkillBaseConfig(2020712, "老当益壮", "严颜", 1.0f, "", "自增", "自身强化", new string[]{"护盾中"}, 2, 1f, 3f, 60, 0f, "hprate<30", 0, "", 0f, 0f, "", 0, 0f, 0f, "shield:210", "", "盾", 5f, "", 0, 0f, 0f, 0f, 0, "SkillAttackedShield", "sway", "MagicChargeGreen", "", "", "", "", "", "");
            config[2020713] = new SkillBaseConfig(2020713, "老当益壮", "严颜", 1.0f, "", "自增", "自身强化", new string[]{"护盾中"}, 3, 1f, 3f, 60, 0f, "hprate<30", 0, "", 0f, 0f, "", 0, 0f, 0f, "shield:240", "", "盾", 5f, "", 0, 0f, 0f, 0f, 0, "SkillAttackedShield", "sway", "MagicChargeGreen", "", "", "", "", "", "");
            config[2020714] = new SkillBaseConfig(2020714, "老当益壮", "严颜", 1.0f, "", "自增", "自身强化", new string[]{"护盾中"}, 4, 1f, 3f, 60, 0f, "hprate<30", 0, "", 0f, 0f, "", 0, 0f, 0f, "shield:270", "", "盾", 5f, "", 0, 0f, 0f, 0f, 0, "SkillAttackedShield", "sway", "MagicChargeGreen", "", "", "", "", "", "");
            config[2020715] = new SkillBaseConfig(2020715, "老当益壮", "严颜", 1.0f, "", "自增", "自身强化", new string[]{"护盾中"}, 5, 1f, 3f, 60, 0f, "hprate<30", 0, "", 0f, 0f, "", 0, 0f, 0f, "shield:300", "", "盾", 5f, "", 0, 0f, 0f, 0f, 0, "SkillAttackedShield", "sway", "MagicChargeGreen", "", "", "", "", "", "");
            config[2020726] = new SkillBaseConfig(2020726, "铁壁", "于禁", 1.0f, "为自身附加/strengthbuff1-shieldReduce%减伤盾持续/bufftime秒，嘲讽/area范围内敌人并对其造成/damagestrength法术伤害", "群控", "群体伤害近距离", new string[]{"减伤大","嘲讽大"}, 1, 0f, 2f, 70, 0f, "", 0, "", 40f, 20f, "requireTarget,selfBuff", 0, 70f, 0f, "", "shieldReduce:0.3", "硬,嘲", 5f, "", 0, 0f, 0f, 0f, 0, "SkillTaunt", "sway", "MagicChargePink", "StunnedDamageUp", "SlowAuraYellow", "", "", "", "");
            config[2020727] = new SkillBaseConfig(2020727, "铁壁", "于禁", 1.0f, "", "群控", "群体伤害近距离", new string[]{"减伤大","嘲讽大"}, 2, 0f, 2f, 70, 0f, "", 0, "", 40f, 20f, "requireTarget,selfBuff", 0, 105f, 0f, "", "shieldReduce:0.35", "硬,嘲", 5f, "", 0, 0f, 0f, 0f, 0, "SkillTaunt", "sway", "MagicChargePink", "StunnedDamageUp", "SlowAuraYellow", "", "", "", "");
            config[2020728] = new SkillBaseConfig(2020728, "铁壁", "于禁", 1.0f, "", "群控", "群体伤害近距离", new string[]{"减伤大","嘲讽大"}, 3, 0f, 2f, 70, 0f, "", 0, "", 40f, 20f, "requireTarget,selfBuff", 0, 175f, 0f, "", "shieldReduce:0.42", "硬,嘲", 5f, "", 0, 0f, 0f, 0f, 0, "SkillTaunt", "sway", "MagicChargePink", "StunnedDamageUp", "SlowAuraYellow", "", "", "", "");
            config[2020729] = new SkillBaseConfig(2020729, "铁壁", "于禁", 1.0f, "", "群控", "群体伤害近距离", new string[]{"减伤大","嘲讽大"}, 4, 0f, 2f, 70, 0f, "", 0, "", 40f, 20f, "requireTarget,selfBuff", 0, 280f, 0f, "", "shieldReduce:0.5", "硬,嘲", 5f, "", 0, 0f, 0f, 0f, 0, "SkillTaunt", "sway", "MagicChargePink", "StunnedDamageUp", "SlowAuraYellow", "", "", "", "");
            config[2020730] = new SkillBaseConfig(2020730, "铁壁", "于禁", 1.0f, "", "群控", "群体伤害近距离", new string[]{"减伤大","嘲讽大"}, 5, 0f, 2f, 70, 0f, "", 0, "", 40f, 20f, "requireTarget,selfBuff", 0, 420f, 0f, "", "shieldReduce:0.65", "硬,嘲", 5f, "", 0, 0f, 0f, 0f, 0, "SkillTaunt", "sway", "MagicChargePink", "StunnedDamageUp", "SlowAuraYellow", "", "", "", "");
            config[2020806] = new SkillBaseConfig(2020806, "舌辩", "张松", 1.2f, "对目标造成/damagestrength法术伤害，并使其移动与攻速降低/strengthbuff1-slow%，持续/bufftime秒", "单控", "单体伤害大", new string[]{"减速大"}, 1, 0f, 2f, 70, 0f, "", 0, "", 30f, 0f, "", 0, 100f, 0f, "", "slow:0.3", "缓", 3f, "", 0, 0f, 0f, 0f, 0, "SkillSingleDamage", "sway", "MagicChargePink", "StunnedCirclingStarsSimple", "", "", "", "", "");
            config[2020807] = new SkillBaseConfig(2020807, "舌辩", "张松", 1.2f, "", "单控", "单体伤害大", new string[]{"减速大"}, 2, 0f, 2f, 70, 0f, "", 0, "", 30f, 0f, "", 0, 150f, 0f, "", "slow:0.35", "缓", 3f, "", 0, 0f, 0f, 0f, 0, "SkillSingleDamage", "sway", "MagicChargePink", "StunnedCirclingStarsSimple", "", "", "", "", "");
            config[2020808] = new SkillBaseConfig(2020808, "舌辩", "张松", 1.2f, "", "单控", "单体伤害大", new string[]{"减速大"}, 3, 0f, 2f, 70, 0f, "", 0, "", 30f, 0f, "", 0, 250f, 0f, "", "slow:0.42", "缓", 3f, "", 0, 0f, 0f, 0f, 0, "SkillSingleDamage", "sway", "MagicChargePink", "StunnedCirclingStarsSimple", "", "", "", "", "");
            config[2020809] = new SkillBaseConfig(2020809, "舌辩", "张松", 1.2f, "", "单控", "单体伤害大", new string[]{"减速大"}, 4, 0f, 2f, 70, 0f, "", 0, "", 30f, 0f, "", 0, 400f, 0f, "", "slow:0.5", "缓", 3f, "", 0, 0f, 0f, 0f, 0, "SkillSingleDamage", "sway", "MagicChargePink", "StunnedCirclingStarsSimple", "", "", "", "", "");
            config[2020810] = new SkillBaseConfig(2020810, "舌辩", "张松", 1.2f, "", "单控", "单体伤害大", new string[]{"减速大"}, 5, 0f, 2f, 70, 0f, "", 0, "", 30f, 0f, "", 0, 600f, 0f, "", "slow:0.6", "缓", 3f, "", 0, 0f, 0f, 0f, 0, "SkillSingleDamage", "sway", "MagicChargePink", "StunnedCirclingStarsSimple", "", "", "", "", "");
            config[2020816] = new SkillBaseConfig(2020816, "断粮", "程昱", 1.0f, "对目标造成/damagestrength法术伤害并降低其/strengthbuff1-atkDown点攻击，并燃烧其/strength2-mpBurn点法力，持续/bufftime秒", "单伤", "单体伤害小", new string[]{"减攻中","燃烧法力"}, 1, 0f, 1.5f, 50, 0f, "", 0, "", 30f, 0f, "mpBurn", 0, 60f, 0f, "mpBurn:20", "atkDown:15", "慑", 4f, "", 0, 0f, 0f, 0f, 0, "SkillSingleDamage", "sway", "StarExplosionBlue", "SharpExplosionGreen", "", "", "", "", "");
            config[2020817] = new SkillBaseConfig(2020817, "断粮", "程昱", 1.0f, "", "单伤", "单体伤害小", new string[]{"减攻中","燃烧法力"}, 2, 0f, 1.5f, 50, 0f, "", 0, "", 30f, 0f, "mpBurn", 0, 90f, 0f, "mpBurn:25", "atkDown:22", "慑", 4f, "", 0, 0f, 0f, 0f, 0, "SkillSingleDamage", "sway", "StarExplosionBlue", "SharpExplosionGreen", "", "", "", "", "");
            config[2020818] = new SkillBaseConfig(2020818, "断粮", "程昱", 1.0f, "", "单伤", "单体伤害小", new string[]{"减攻中","燃烧法力"}, 3, 0f, 1.5f, 50, 0f, "", 0, "", 30f, 0f, "mpBurn", 0, 150f, 0f, "mpBurn:30", "atkDown:33", "慑", 4f, "", 0, 0f, 0f, 0f, 0, "SkillSingleDamage", "sway", "StarExplosionBlue", "SharpExplosionGreen", "", "", "", "", "");
            config[2020819] = new SkillBaseConfig(2020819, "断粮", "程昱", 1.0f, "", "单伤", "单体伤害小", new string[]{"减攻中","燃烧法力"}, 4, 0f, 1.5f, 50, 0f, "", 0, "", 30f, 0f, "mpBurn", 0, 240f, 0f, "mpBurn:35", "atkDown:55", "慑", 4f, "", 0, 0f, 0f, 0f, 0, "SkillSingleDamage", "sway", "StarExplosionBlue", "SharpExplosionGreen", "", "", "", "", "");
            config[2020820] = new SkillBaseConfig(2020820, "断粮", "程昱", 1.0f, "", "单伤", "单体伤害小", new string[]{"减攻中","燃烧法力"}, 5, 0f, 1.5f, 50, 0f, "", 0, "", 30f, 0f, "mpBurn", 0, 360f, 0f, "mpBurn:40", "atkDown:80", "慑", 4f, "", 0, 0f, 0f, 0f, 0, "SkillSingleDamage", "sway", "StarExplosionBlue", "SharpExplosionGreen", "", "", "", "", "");
            config[2020831] = new SkillBaseConfig(2020831, "傲言", "许攸", 1.0f, "对目标造成/damagestrength法术伤害并使其受到伤害提升/strengthbuff1-damagedUp%、驱散其正面buff，持续/bufftime秒", "单伤", "单体伤害小", new string[]{"增伤中","驱散"}, 1, 0f, 1.5f, 50, 0f, "", 0, "", 30f, 0f, "dispel", 0, 60f, 0f, "", "damagedUp:0.1", "伤", 4f, "", 0, 0f, 0f, 0f, 0, "SkillSingleDamage", "sway", "MagicChargeYellow", "ToolExplosion", "", "", "", "", "");
            config[2020832] = new SkillBaseConfig(2020832, "傲言", "许攸", 1.0f, "", "单伤", "单体伤害小", new string[]{"增伤中","驱散"}, 2, 0f, 1.5f, 50, 0f, "", 0, "", 30f, 0f, "dispel", 0, 90f, 0f, "", "damagedUp:0.15", "伤", 4f, "", 0, 0f, 0f, 0f, 0, "SkillSingleDamage", "sway", "MagicChargeYellow", "ToolExplosion", "", "", "", "", "");
            config[2020833] = new SkillBaseConfig(2020833, "傲言", "许攸", 1.0f, "", "单伤", "单体伤害小", new string[]{"增伤中","驱散"}, 3, 0f, 1.5f, 50, 0f, "", 0, "", 30f, 0f, "dispel", 0, 150f, 0f, "", "damagedUp:0.24", "伤", 4f, "", 0, 0f, 0f, 0f, 0, "SkillSingleDamage", "sway", "MagicChargeYellow", "ToolExplosion", "", "", "", "", "");
            config[2020834] = new SkillBaseConfig(2020834, "傲言", "许攸", 1.0f, "", "单伤", "单体伤害小", new string[]{"增伤中","驱散"}, 4, 0f, 1.5f, 50, 0f, "", 0, "", 30f, 0f, "dispel", 0, 240f, 0f, "", "damagedUp:0.36", "伤", 4f, "", 0, 0f, 0f, 0f, 0, "SkillSingleDamage", "sway", "MagicChargeYellow", "ToolExplosion", "", "", "", "", "");
            config[2020835] = new SkillBaseConfig(2020835, "傲言", "许攸", 1.0f, "", "单伤", "单体伤害小", new string[]{"增伤中","驱散"}, 5, 0f, 1.5f, 50, 0f, "", 0, "", 30f, 0f, "dispel", 0, 360f, 0f, "", "damagedUp:0.5", "伤", 4f, "", 0, 0f, 0f, 0f, 0, "SkillSingleDamage", "sway", "MagicChargeYellow", "ToolExplosion", "", "", "", "", "");
            config[2020836] = new SkillBaseConfig(2020836, "清谈", "司马攸", 1.2f, "对目标造成/damagestrength法术伤害，并降低其/strengthbuff1-atkDown点攻击，持续/bufftime秒", "单伤", "单体伤害小", new string[]{"减攻中"}, 1, 0f, 1.5f, 50, 0f, "", 0, "", 45f, 0f, "", 0, 60f, 0f, "", "atkDown:15", "慑", 4f, "", 0, 0f, 0f, 0f, 0, "SkillSingleDamage", "sway", "ShieldSoftYellow", "LightningExplosionYellow", "MagicAuraYellow", "", "", "", "");
            config[2020837] = new SkillBaseConfig(2020837, "清谈", "司马攸", 1.2f, "", "单伤", "单体伤害小", new string[]{"减攻中"}, 2, 0f, 1.5f, 50, 0f, "", 0, "", 45f, 0f, "", 0, 90f, 0f, "", "atkDown:22", "慑", 4f, "", 0, 0f, 0f, 0f, 0, "SkillSingleDamage", "sway", "ShieldSoftYellow", "LightningExplosionYellow", "MagicAuraYellow", "", "", "", "");
            config[2020838] = new SkillBaseConfig(2020838, "清谈", "司马攸", 1.2f, "", "单伤", "单体伤害小", new string[]{"减攻中"}, 3, 0f, 1.5f, 50, 0f, "", 0, "", 45f, 0f, "", 0, 150f, 0f, "", "atkDown:32", "慑", 4f, "", 0, 0f, 0f, 0f, 0, "SkillSingleDamage", "sway", "ShieldSoftYellow", "LightningExplosionYellow", "MagicAuraYellow", "", "", "", "");
            config[2020839] = new SkillBaseConfig(2020839, "清谈", "司马攸", 1.2f, "", "单伤", "单体伤害小", new string[]{"减攻中"}, 4, 0f, 1.5f, 50, 0f, "", 0, "", 45f, 0f, "", 0, 240f, 0f, "", "atkDown:45", "慑", 4f, "", 0, 0f, 0f, 0f, 0, "SkillSingleDamage", "sway", "ShieldSoftYellow", "LightningExplosionYellow", "MagicAuraYellow", "", "", "", "");
            config[2020840] = new SkillBaseConfig(2020840, "清谈", "司马攸", 1.2f, "", "单伤", "单体伤害小", new string[]{"减攻中"}, 5, 0f, 1.5f, 50, 0f, "", 0, "", 45f, 0f, "", 0, 360f, 0f, "", "atkDown:60", "慑", 4f, "", 0, 0f, 0f, 0f, 0, "SkillSingleDamage", "sway", "ShieldSoftYellow", "LightningExplosionYellow", "MagicAuraYellow", "", "", "", "");
            config[2020841] = new SkillBaseConfig(2020841, "伪书", "蒋济", 1.1f, "对/area范围内敌人造成/damagestrength法术伤害并降低其治疗/strength2-healDown%，持续/bufftime秒", "群伤", "群体伤害近距离", new string[]{"减疗中"}, 1, 0f, 2f, 70, 0f, "", 0, "", 40f, 20f, "", 4, 70f, 0f, "healDown:0.15", "", "疫", 4f, "", 0, 0f, 0f, 0f, 0, "SkillAreaDamageDeBuff", "sway", "MagicChargeBlue", "GasExplosionFire", "PoisonExplosionSoft", "", "", "", "");
            config[2020842] = new SkillBaseConfig(2020842, "伪书", "蒋济", 1.1f, "", "群伤", "群体伤害近距离", new string[]{"减疗中"}, 2, 0f, 2f, 70, 0f, "", 0, "", 40f, 20f, "", 4, 105f, 0f, "healDown:0.18", "", "疫", 4f, "", 0, 0f, 0f, 0f, 0, "SkillAreaDamageDeBuff", "sway", "MagicChargeBlue", "GasExplosionFire", "PoisonExplosionSoft", "", "", "", "");
            config[2020843] = new SkillBaseConfig(2020843, "伪书", "蒋济", 1.1f, "", "群伤", "群体伤害近距离", new string[]{"减疗中"}, 3, 0f, 2f, 70, 0f, "", 0, "", 40f, 20f, "", 4, 175f, 0f, "healDown:0.22", "", "疫", 4f, "", 0, 0f, 0f, 0f, 0, "SkillAreaDamageDeBuff", "sway", "MagicChargeBlue", "GasExplosionFire", "PoisonExplosionSoft", "", "", "", "");
            config[2020844] = new SkillBaseConfig(2020844, "伪书", "蒋济", 1.1f, "", "群伤", "群体伤害近距离", new string[]{"减疗中"}, 4, 0f, 2f, 70, 0f, "", 0, "", 40f, 20f, "", 4, 280f, 0f, "healDown:0.26", "", "疫", 4f, "", 0, 0f, 0f, 0f, 0, "SkillAreaDamageDeBuff", "sway", "MagicChargeBlue", "GasExplosionFire", "PoisonExplosionSoft", "", "", "", "");
            config[2020845] = new SkillBaseConfig(2020845, "伪书", "蒋济", 1.1f, "", "群伤", "群体伤害近距离", new string[]{"减疗中"}, 5, 0f, 2f, 70, 0f, "", 0, "", 40f, 20f, "", 4, 420f, 0f, "healDown:0.3", "", "疫", 4f, "", 0, 0f, 0f, 0f, 0, "SkillAreaDamageDeBuff", "sway", "MagicChargeBlue", "GasExplosionFire", "PoisonExplosionSoft", "", "", "", "");
            config[2020846] = new SkillBaseConfig(2020846, "刚愎", "诸葛恪", 1.1f, "对/area范围内敌人造成/damagestrength法术伤害并降低其治疗/strength2-healDown%，持续/bufftime秒", "群伤", "群体伤害近距离", new string[]{"减疗中"}, 1, 0f, 2f, 70, 0f, "", 0, "", 40f, 20f, "", 4, 70f, 0f, "healDown:0.15", "", "疫", 4f, "", 0, 0f, 0f, 0f, 0, "SkillAreaDamageDeBuff", "sway", "MagicChargeBlue", "GasExplosionFire", "PoisonExplosionSoft", "", "", "", "");
            config[2020847] = new SkillBaseConfig(2020847, "刚愎", "诸葛恪", 1.1f, "", "群伤", "群体伤害近距离", new string[]{"减疗中"}, 2, 0f, 2f, 70, 0f, "", 0, "", 40f, 20f, "", 4, 105f, 0f, "healDown:0.18", "", "疫", 4f, "", 0, 0f, 0f, 0f, 0, "SkillAreaDamageDeBuff", "sway", "MagicChargeBlue", "GasExplosionFire", "PoisonExplosionSoft", "", "", "", "");
            config[2020848] = new SkillBaseConfig(2020848, "刚愎", "诸葛恪", 1.1f, "", "群伤", "群体伤害近距离", new string[]{"减疗中"}, 3, 0f, 2f, 70, 0f, "", 0, "", 40f, 20f, "", 4, 175f, 0f, "healDown:0.22", "", "疫", 4f, "", 0, 0f, 0f, 0f, 0, "SkillAreaDamageDeBuff", "sway", "MagicChargeBlue", "GasExplosionFire", "PoisonExplosionSoft", "", "", "", "");
            config[2020849] = new SkillBaseConfig(2020849, "刚愎", "诸葛恪", 1.1f, "", "群伤", "群体伤害近距离", new string[]{"减疗中"}, 4, 0f, 2f, 70, 0f, "", 0, "", 40f, 20f, "", 4, 280f, 0f, "healDown:0.26", "", "疫", 4f, "", 0, 0f, 0f, 0f, 0, "SkillAreaDamageDeBuff", "sway", "MagicChargeBlue", "GasExplosionFire", "PoisonExplosionSoft", "", "", "", "");
            config[2020850] = new SkillBaseConfig(2020850, "刚愎", "诸葛恪", 1.1f, "", "群伤", "群体伤害近距离", new string[]{"减疗中"}, 5, 0f, 2f, 70, 0f, "", 0, "", 40f, 20f, "", 4, 420f, 0f, "healDown:0.3", "", "疫", 4f, "", 0, 0f, 0f, 0f, 0, "SkillAreaDamageDeBuff", "sway", "MagicChargeBlue", "GasExplosionFire", "PoisonExplosionSoft", "", "", "", "");
            config[2020901] = new SkillBaseConfig(2020901, "抚军", "蒋琬", 1.2f, "治疗生命最低的友军及其/area范围内最多/targetcount名友军各/healstrength点生命", "群疗", "范围治疗", new string[]{}, 1, 0f, 3f, 90, 0f, "", 0, "", 50f, 20f, "", 3, 0f, 70f, "", "", "", 0f, "", 0, 0f, 0f, 0f, 0, "SkillAreaHeal", "sway", "HeartStream", "MagicBuffGreen", "AuraSoftPurple", "", "", "", "");
            config[2020902] = new SkillBaseConfig(2020902, "抚军", "蒋琬", 1.2f, "", "群疗", "范围治疗", new string[]{}, 2, 0f, 3f, 90, 0f, "", 0, "", 50f, 20f, "", 3, 0f, 105f, "", "", "", 0f, "", 0, 0f, 0f, 0f, 0, "SkillAreaHeal", "sway", "HeartStream", "MagicBuffGreen", "AuraSoftPurple", "", "", "", "");
            config[2020903] = new SkillBaseConfig(2020903, "抚军", "蒋琬", 1.2f, "", "群疗", "范围治疗", new string[]{}, 3, 0f, 3f, 90, 0f, "", 0, "", 50f, 20f, "", 3, 0f, 175f, "", "", "", 0f, "", 0, 0f, 0f, 0f, 0, "SkillAreaHeal", "sway", "HeartStream", "MagicBuffGreen", "AuraSoftPurple", "", "", "", "");
            config[2020904] = new SkillBaseConfig(2020904, "抚军", "蒋琬", 1.2f, "", "群疗", "范围治疗", new string[]{}, 4, 0f, 3f, 90, 0f, "", 0, "", 50f, 20f, "", 3, 0f, 280f, "", "", "", 0f, "", 0, 0f, 0f, 0f, 0, "SkillAreaHeal", "sway", "HeartStream", "MagicBuffGreen", "AuraSoftPurple", "", "", "", "");
            config[2020905] = new SkillBaseConfig(2020905, "抚军", "蒋琬", 1.2f, "", "群疗", "范围治疗", new string[]{}, 5, 0f, 3f, 90, 0f, "", 0, "", 50f, 20f, "", 3, 0f, 420f, "", "", "", 0f, "", 0, 0f, 0f, 0f, 0, "SkillAreaHeal", "sway", "HeartStream", "MagicBuffGreen", "AuraSoftPurple", "", "", "", "");
            config[2020906] = new SkillBaseConfig(2020906, "励军", "马良", 1.0f, "为一名友方英雄及其/area范围内的小兵附加攻击提升/strengthbuff1-atk，持续/bufftime秒", "兵增", "兵种增益", new string[]{"加攻大"}, 1, 0f, 4f, 70, 0f, "", 0, "", 0f, 20f, "", 0, 0f, 0f, "", "atk:20", "攻", 5f, "", 0, 0f, 0f, 0f, 0, "SkillSoldierGrow", "sway", "HeartStream", "", "", "", "", "", "");
            config[2020907] = new SkillBaseConfig(2020907, "励军", "马良", 1.0f, "", "兵增", "兵种增益", new string[]{"加攻大"}, 2, 0f, 4f, 70, 0f, "", 0, "", 0f, 20f, "", 0, 0f, 0f, "", "atk:35", "攻", 5f, "", 0, 0f, 0f, 0f, 0, "SkillSoldierGrow", "sway", "HeartStream", "", "", "", "", "", "");
            config[2020908] = new SkillBaseConfig(2020908, "励军", "马良", 1.0f, "", "兵增", "兵种增益", new string[]{"加攻大"}, 3, 0f, 4f, 70, 0f, "", 0, "", 0f, 20f, "", 0, 0f, 0f, "", "atk:55", "攻", 5f, "", 0, 0f, 0f, 0f, 0, "SkillSoldierGrow", "sway", "HeartStream", "", "", "", "", "", "");
            config[2020909] = new SkillBaseConfig(2020909, "励军", "马良", 1.0f, "", "兵增", "兵种增益", new string[]{"加攻大"}, 4, 0f, 4f, 70, 0f, "", 0, "", 0f, 20f, "", 0, 0f, 0f, "", "atk:85", "攻", 5f, "", 0, 0f, 0f, 0f, 0, "SkillSoldierGrow", "sway", "HeartStream", "", "", "", "", "", "");
            config[2020910] = new SkillBaseConfig(2020910, "励军", "马良", 1.0f, "", "兵增", "兵种增益", new string[]{"加攻大"}, 5, 0f, 4f, 70, 0f, "", 0, "", 0f, 20f, "", 0, 0f, 0f, "", "atk:130", "攻", 5f, "", 0, 0f, 0f, 0f, 0, "SkillSoldierGrow", "sway", "HeartStream", "", "", "", "", "", "");
            config[2020921] = new SkillBaseConfig(2020921, "强弓", "沮授", 1.0f, "对一名敌方单位造成/damagestrength法术伤害，本侧每名远程士兵与沮授自身各射出一枚法术箭，远程兵越多伤害越高", "兵增", "兵种增益", new string[]{}, 1, 0f, 4f, 70, 0f, "", 0, "", 0f, 20f, "", 0, 15f, 0f, "", "", "", 0f, "", 0, 0f, 0f, 0f, 0, "SkillSoldierArcherBuff", "sway", "ShieldSoftBlue", "BulletExplosionGreen", "", "", "", "", "");
            config[2020922] = new SkillBaseConfig(2020922, "强弓", "沮授", 1.0f, "", "兵增", "兵种增益", new string[]{}, 2, 0f, 4f, 70, 0f, "", 0, "", 0f, 20f, "", 0, 22f, 0f, "", "", "", 0f, "", 0, 0f, 0f, 0f, 0, "SkillSoldierArcherBuff", "sway", "ShieldSoftBlue", "BulletExplosionGreen", "", "", "", "", "");
            config[2020923] = new SkillBaseConfig(2020923, "强弓", "沮授", 1.0f, "", "兵增", "兵种增益", new string[]{}, 3, 0f, 4f, 70, 0f, "", 0, "", 0f, 20f, "", 0, 32f, 0f, "", "", "", 0f, "", 0, 0f, 0f, 0f, 0, "SkillSoldierArcherBuff", "sway", "ShieldSoftBlue", "BulletExplosionGreen", "", "", "", "", "");
            config[2020924] = new SkillBaseConfig(2020924, "强弓", "沮授", 1.0f, "", "兵增", "兵种增益", new string[]{}, 4, 0f, 4f, 70, 0f, "", 0, "", 0f, 20f, "", 0, 46f, 0f, "", "", "", 0f, "", 0, 0f, 0f, 0f, 0, "SkillSoldierArcherBuff", "sway", "ShieldSoftBlue", "BulletExplosionGreen", "", "", "", "", "");
            config[2020925] = new SkillBaseConfig(2020925, "强弓", "沮授", 1.0f, "", "兵增", "兵种增益", new string[]{}, 5, 0f, 4f, 70, 0f, "", 0, "", 0f, 20f, "", 0, 64f, 0f, "", "", "", 0f, "", 0, 0f, 0f, 0f, 0, "SkillSoldierArcherBuff", "sway", "ShieldSoftBlue", "BulletExplosionGreen", "", "", "", "", "");
            config[2020931] = new SkillBaseConfig(2020931, "调度", "杨仪", 1.1f, "为自己及周围最多/targetcount名友军附加/strengthbuff1-armor点护甲，持续/bufftime秒", "群增", "群体友方增益", new string[]{"加护甲大","施法者必含"}, 1, 0f, 4f, 80, 0f, "", 0, "", 50f, 20f, "selfAlways", 3, 0f, 0f, "", "armor:10", "甲", 5f, "", 0, 0f, 0f, 0f, 0, "SkillAreaBuff", "jumpspin", "MagicChargeGreen", "ShieldSoftBlue", "MagicAuraYellow", "", "", "", "");
            config[2020932] = new SkillBaseConfig(2020932, "调度", "杨仪", 1.1f, "", "群增", "群体友方增益", new string[]{"加护甲大","施法者必含"}, 2, 0f, 4f, 80, 0f, "", 0, "", 50f, 20f, "selfAlways", 3, 0f, 0f, "", "armor:15", "甲", 5f, "", 0, 0f, 0f, 0f, 0, "SkillAreaBuff", "jumpspin", "MagicChargeGreen", "ShieldSoftBlue", "MagicAuraYellow", "", "", "", "");
            config[2020933] = new SkillBaseConfig(2020933, "调度", "杨仪", 1.1f, "", "群增", "群体友方增益", new string[]{"加护甲大","施法者必含"}, 3, 0f, 4f, 80, 0f, "", 0, "", 50f, 20f, "selfAlways", 3, 0f, 0f, "", "armor:22", "甲", 5f, "", 0, 0f, 0f, 0f, 0, "SkillAreaBuff", "jumpspin", "MagicChargeGreen", "ShieldSoftBlue", "MagicAuraYellow", "", "", "", "");
            config[2020934] = new SkillBaseConfig(2020934, "调度", "杨仪", 1.1f, "", "群增", "群体友方增益", new string[]{"加护甲大","施法者必含"}, 4, 0f, 4f, 80, 0f, "", 0, "", 50f, 20f, "selfAlways", 3, 0f, 0f, "", "armor:30", "甲", 5f, "", 0, 0f, 0f, 0f, 0, "SkillAreaBuff", "jumpspin", "MagicChargeGreen", "ShieldSoftBlue", "MagicAuraYellow", "", "", "", "");
            config[2020935] = new SkillBaseConfig(2020935, "调度", "杨仪", 1.1f, "", "群增", "群体友方增益", new string[]{"加护甲大","施法者必含"}, 5, 0f, 4f, 80, 0f, "", 0, "", 50f, 20f, "selfAlways", 3, 0f, 0f, "", "armor:40", "甲", 5f, "", 0, 0f, 0f, 0f, 0, "SkillAreaBuff", "jumpspin", "MagicChargeGreen", "ShieldSoftBlue", "MagicAuraYellow", "", "", "", "");
            config[2020936] = new SkillBaseConfig(2020936, "镇抚", "步骘", 1.1f, "我方全体护甲/auroattrs-armor，魔抗/auroattrs-magicres，随时间增强；每次释放自身永久+攻击/strengthbuff1-selfGrow", "光环", "光环", new string[]{}, 1, 0f, 3f, 120, 0f, "", 0, "", 0f, 0f, "", 0, 0f, 0f, "auraRate:0.5", "selfGrow:4", "", 0f, "", 0, 0f, 0f, 0f, 0, "SkillAura", "", "Par_PurpleField", "", "", "", "", "", "armor+5,magicres+5");
            config[2020937] = new SkillBaseConfig(2020937, "镇抚", "步骘", 1.1f, "", "光环", "光环", new string[]{}, 2, 0f, 3f, 120, 0f, "", 0, "", 0f, 0f, "", 0, 0f, 0f, "auraRate:0.5", "selfGrow:6", "", 0f, "", 0, 0f, 0f, 0f, 0, "SkillAura", "", "Par_PurpleField", "", "", "", "", "", "armor+7,magicres+7");
            config[2020938] = new SkillBaseConfig(2020938, "镇抚", "步骘", 1.1f, "", "光环", "光环", new string[]{}, 3, 0f, 3f, 120, 0f, "", 0, "", 0f, 0f, "", 0, 0f, 0f, "auraRate:0.5", "selfGrow:9", "", 0f, "", 0, 0f, 0f, 0f, 0, "SkillAura", "", "Par_PurpleField", "", "", "", "", "", "armor+12,magicres+12");
            config[2020939] = new SkillBaseConfig(2020939, "镇抚", "步骘", 1.1f, "", "光环", "光环", new string[]{}, 4, 0f, 3f, 120, 0f, "", 0, "", 0f, 0f, "", 0, 0f, 0f, "auraRate:0.5", "selfGrow:14", "", 0f, "", 0, 0f, 0f, 0f, 0, "SkillAura", "", "Par_PurpleField", "", "", "", "", "", "armor+18,magicres+18");
            config[2020940] = new SkillBaseConfig(2020940, "镇抚", "步骘", 1.1f, "", "光环", "光环", new string[]{}, 5, 0f, 3f, 120, 0f, "", 0, "", 0f, 0f, "", 0, 0f, 0f, "auraRate:0.5", "selfGrow:20", "", 0f, "", 0, 0f, 0f, 0f, 0, "SkillAura", "", "Par_PurpleField", "", "", "", "", "", "armor+26,magicres+26");
            config[2021006] = new SkillBaseConfig(2021006, "明断", "法正", 1.0f, "对/targetcount名敌人造成/damagestrength法术伤害（每跳递减）", "群伤", "跳跃弹射", new string[]{}, 1, 0f, 3f, 80, 0f, "", 0, "", 30f, 60f, "", 4, 60f, 0f, "", "", "", 4f, "", 0, 0f, 0f, 0f, 0, "SkillJumpChain", "sway", "FlashExplosionBlue", "top_down_lightning_dot_blue", "", "", "", "", "");
            config[2021007] = new SkillBaseConfig(2021007, "明断", "法正", 1.0f, "", "群伤", "跳跃弹射", new string[]{}, 2, 0f, 3f, 80, 0f, "", 0, "", 30f, 60f, "", 4, 90f, 0f, "", "", "", 4f, "", 0, 0f, 0f, 0f, 0, "SkillJumpChain", "sway", "FlashExplosionBlue", "top_down_lightning_dot_blue", "", "", "", "", "");
            config[2021008] = new SkillBaseConfig(2021008, "明断", "法正", 1.0f, "", "群伤", "跳跃弹射", new string[]{}, 3, 0f, 3f, 80, 0f, "", 0, "", 30f, 60f, "", 4, 150f, 0f, "", "", "", 4f, "", 0, 0f, 0f, 0f, 0, "SkillJumpChain", "sway", "FlashExplosionBlue", "top_down_lightning_dot_blue", "", "", "", "", "");
            config[2021009] = new SkillBaseConfig(2021009, "明断", "法正", 1.0f, "", "群伤", "跳跃弹射", new string[]{}, 4, 0f, 3f, 80, 0f, "", 0, "", 30f, 60f, "", 4, 240f, 0f, "", "", "", 4f, "", 0, 0f, 0f, 0f, 0, "SkillJumpChain", "sway", "FlashExplosionBlue", "top_down_lightning_dot_blue", "", "", "", "", "");
            config[2021010] = new SkillBaseConfig(2021010, "明断", "法正", 1.0f, "", "群伤", "跳跃弹射", new string[]{}, 5, 0f, 3f, 80, 0f, "", 0, "", 30f, 60f, "", 4, 360f, 0f, "", "", "", 4f, "", 0, 0f, 0f, 0f, 0, "SkillJumpChain", "sway", "FlashExplosionBlue", "top_down_lightning_dot_blue", "", "", "", "", "");
            config[2021031] = new SkillBaseConfig(2021031, "劝学", "谯周", 1.1f, "对目标造成/damagestrength法术伤害", "单伤", "单体伤害中", new string[]{}, 1, 0f, 2f, 60, 0f, "", 0, "", 35f, 0f, "", 0, 80f, 0f, "", "", "", 0f, "", 0, 0f, 0f, 0f, 0, "SkillSingleDamage", "sway", "MagicChargeYellow", "SwordHitWhiteCritical", "", "", "", "", "");
            config[2021032] = new SkillBaseConfig(2021032, "劝学", "谯周", 1.1f, "", "单伤", "单体伤害中", new string[]{}, 2, 0f, 2f, 60, 0f, "", 0, "", 35f, 0f, "", 0, 120f, 0f, "", "", "", 0f, "", 0, 0f, 0f, 0f, 0, "SkillSingleDamage", "sway", "MagicChargeYellow", "SwordHitWhiteCritical", "", "", "", "", "");
            config[2021033] = new SkillBaseConfig(2021033, "劝学", "谯周", 1.1f, "", "单伤", "单体伤害中", new string[]{}, 3, 0f, 2f, 60, 0f, "", 0, "", 35f, 0f, "", 0, 200f, 0f, "", "", "", 0f, "", 0, 0f, 0f, 0f, 0, "SkillSingleDamage", "sway", "MagicChargeYellow", "SwordHitWhiteCritical", "", "", "", "", "");
            config[2021034] = new SkillBaseConfig(2021034, "劝学", "谯周", 1.1f, "", "单伤", "单体伤害中", new string[]{}, 4, 0f, 2f, 60, 0f, "", 0, "", 35f, 0f, "", 0, 320f, 0f, "", "", "", 0f, "", 0, 0f, 0f, 0f, 0, "SkillSingleDamage", "sway", "MagicChargeYellow", "SwordHitWhiteCritical", "", "", "", "", "");
            config[2021035] = new SkillBaseConfig(2021035, "劝学", "谯周", 1.1f, "", "单伤", "单体伤害中", new string[]{}, 5, 0f, 2f, 60, 0f, "", 0, "", 35f, 0f, "", 0, 480f, 0f, "", "", "", 0f, "", 0, 0f, 0f, 0f, 0, "SkillSingleDamage", "sway", "MagicChargeYellow", "SwordHitWhiteCritical", "", "", "", "", "");
            config[2021036] = new SkillBaseConfig(2021036, "博物", "张华", 1.1f, "对/area范围内敌人造成/damagestrength法术伤害并使其移动与攻速降低/strengthbuff1-slow%，持续/bufftime秒", "群伤", "群体伤害近距离", new string[]{"减速大"}, 1, 0f, 2f, 70, 0f, "", 0, "", 40f, 20f, "", 3, 70f, 0f, "", "slow:0.3", "缓", 3f, "", 0, 0f, 0f, 0f, 0, "SkillAreaDamageDeBuff", "sway", "MagicChargeBlue", "GasExplosionFire", "PoisonExplosionSoft", "", "", "", "");
            config[2021037] = new SkillBaseConfig(2021037, "博物", "张华", 1.1f, "", "群伤", "群体伤害近距离", new string[]{"减速大"}, 2, 0f, 2f, 70, 0f, "", 0, "", 40f, 20f, "", 3, 105f, 0f, "", "slow:0.4", "缓", 3f, "", 0, 0f, 0f, 0f, 0, "SkillAreaDamageDeBuff", "sway", "MagicChargeBlue", "GasExplosionFire", "PoisonExplosionSoft", "", "", "", "");
            config[2021038] = new SkillBaseConfig(2021038, "博物", "张华", 1.1f, "", "群伤", "群体伤害近距离", new string[]{"减速大"}, 3, 0f, 2f, 70, 0f, "", 0, "", 40f, 20f, "", 3, 175f, 0f, "", "slow:0.5", "缓", 3f, "", 0, 0f, 0f, 0f, 0, "SkillAreaDamageDeBuff", "sway", "MagicChargeBlue", "GasExplosionFire", "PoisonExplosionSoft", "", "", "", "");
            config[2021039] = new SkillBaseConfig(2021039, "博物", "张华", 1.1f, "", "群伤", "群体伤害近距离", new string[]{"减速大"}, 4, 0f, 2f, 70, 0f, "", 0, "", 40f, 20f, "", 3, 280f, 0f, "", "slow:0.6", "缓", 3f, "", 0, 0f, 0f, 0f, 0, "SkillAreaDamageDeBuff", "sway", "MagicChargeBlue", "GasExplosionFire", "PoisonExplosionSoft", "", "", "", "");
            config[2021040] = new SkillBaseConfig(2021040, "博物", "张华", 1.1f, "", "群伤", "群体伤害近距离", new string[]{"减速大"}, 5, 0f, 2f, 70, 0f, "", 0, "", 40f, 20f, "", 3, 420f, 0f, "", "slow:0.7", "缓", 3f, "", 0, 0f, 0f, 0f, 0, "SkillAreaDamageDeBuff", "sway", "MagicChargeBlue", "GasExplosionFire", "PoisonExplosionSoft", "", "", "", "");
            config[2021041] = new SkillBaseConfig(2021041, "长啸", "阮籍", 1.2f, "为我方法强最高的远程英雄提升/strengthbuff1-ap点法强，持续/bufftime秒", "单增", "单体友方增益", new string[]{"加法强大","最高法强","远程优先","排除自身","不按首段去重","英雄限定"}, 1, 0f, 3f, 60, 0f, "", 0, "", 50f, 70f, "heroOnly,excludeSelf,rangedOnly,highestAp,noDedupe", 1, 0f, 0f, "", "ap:40", "术", 5f, "", 0, 0f, 0f, 0f, 0, "SkillAreaBuff", "sway", "MagicBuffGreen", "MagicChargePink", "", "", "", "", "");
            config[2021042] = new SkillBaseConfig(2021042, "长啸", "阮籍", 1.2f, "", "单增", "单体友方增益", new string[]{"加法强大","最高法强","远程优先","排除自身","不按首段去重","英雄限定"}, 2, 0f, 3f, 60, 0f, "", 0, "", 50f, 70f, "heroOnly,excludeSelf,rangedOnly,highestAp,noDedupe", 1, 0f, 0f, "", "ap:60", "术", 5f, "", 0, 0f, 0f, 0f, 0, "SkillAreaBuff", "sway", "MagicBuffGreen", "MagicChargePink", "", "", "", "", "");
            config[2021043] = new SkillBaseConfig(2021043, "长啸", "阮籍", 1.2f, "", "单增", "单体友方增益", new string[]{"加法强大","最高法强","远程优先","排除自身","不按首段去重","英雄限定"}, 3, 0f, 3f, 60, 0f, "", 0, "", 50f, 70f, "heroOnly,excludeSelf,rangedOnly,highestAp,noDedupe", 1, 0f, 0f, "", "ap:90", "术", 5f, "", 0, 0f, 0f, 0f, 0, "SkillAreaBuff", "sway", "MagicBuffGreen", "MagicChargePink", "", "", "", "", "");
            config[2021044] = new SkillBaseConfig(2021044, "长啸", "阮籍", 1.2f, "", "单增", "单体友方增益", new string[]{"加法强大","最高法强","远程优先","排除自身","不按首段去重","英雄限定"}, 4, 0f, 3f, 60, 0f, "", 0, "", 50f, 70f, "heroOnly,excludeSelf,rangedOnly,highestAp,noDedupe", 1, 0f, 0f, "", "ap:130", "术", 5f, "", 0, 0f, 0f, 0f, 0, "SkillAreaBuff", "sway", "MagicBuffGreen", "MagicChargePink", "", "", "", "", "");
            config[2021045] = new SkillBaseConfig(2021045, "长啸", "阮籍", 1.2f, "", "单增", "单体友方增益", new string[]{"加法强大","最高法强","远程优先","排除自身","不按首段去重","英雄限定"}, 5, 0f, 3f, 60, 0f, "", 0, "", 50f, 70f, "heroOnly,excludeSelf,rangedOnly,highestAp,noDedupe", 1, 0f, 0f, "", "ap:180", "术", 5f, "", 0, 0f, 0f, 0f, 0, "SkillAreaBuff", "sway", "MagicBuffGreen", "MagicChargePink", "", "", "", "", "");
            config[2021046] = new SkillBaseConfig(2021046, "明鉴", "辛宪英", 1.1f, "对目标造成/damagestrength法术伤害并使其受到伤害提升/strengthbuff1-damagedUp%，持续/bufftime秒", "单伤", "单体伤害小", new string[]{"增伤中"}, 1, 0f, 1.5f, 50, 0f, "", 0, "", 40f, 0f, "", 0, 60f, 0f, "", "damagedUp:0.1", "伤", 4f, "", 0, 0f, 0f, 0f, 0, "SkillSingleDamage", "sway", "MagicChargeYellow", "SwordHitWhiteCritical", "", "", "", "", "");
            config[2021047] = new SkillBaseConfig(2021047, "明鉴", "辛宪英", 1.1f, "", "单伤", "单体伤害小", new string[]{"增伤中"}, 2, 0f, 1.5f, 50, 0f, "", 0, "", 40f, 0f, "", 0, 90f, 0f, "", "damagedUp:0.15", "伤", 4f, "", 0, 0f, 0f, 0f, 0, "SkillSingleDamage", "sway", "MagicChargeYellow", "SwordHitWhiteCritical", "", "", "", "", "");
            config[2021048] = new SkillBaseConfig(2021048, "明鉴", "辛宪英", 1.1f, "", "单伤", "单体伤害小", new string[]{"增伤中"}, 3, 0f, 1.5f, 50, 0f, "", 0, "", 40f, 0f, "", 0, 150f, 0f, "", "damagedUp:0.24", "伤", 4f, "", 0, 0f, 0f, 0f, 0, "SkillSingleDamage", "sway", "MagicChargeYellow", "SwordHitWhiteCritical", "", "", "", "", "");
            config[2021049] = new SkillBaseConfig(2021049, "明鉴", "辛宪英", 1.1f, "", "单伤", "单体伤害小", new string[]{"增伤中"}, 4, 0f, 1.5f, 50, 0f, "", 0, "", 40f, 0f, "", 0, 240f, 0f, "", "damagedUp:0.36", "伤", 4f, "", 0, 0f, 0f, 0f, 0, "SkillSingleDamage", "sway", "MagicChargeYellow", "SwordHitWhiteCritical", "", "", "", "", "");
            config[2021050] = new SkillBaseConfig(2021050, "明鉴", "辛宪英", 1.1f, "", "单伤", "单体伤害小", new string[]{"增伤中"}, 5, 0f, 1.5f, 50, 0f, "", 0, "", 40f, 0f, "", 0, 360f, 0f, "", "damagedUp:0.5", "伤", 4f, "", 0, 0f, 0f, 0f, 0, "SkillSingleDamage", "sway", "MagicChargeYellow", "SwordHitWhiteCritical", "", "", "", "", "");
            config[2021101] = new SkillBaseConfig(2021101, "鼓舞", "孙乾", 1.2f, "我方全体生命回复/auroattrs-hpRegen/秒，随时间增强", "光环", "光环", new string[]{}, 1, 0f, 3f, 120, 0f, "", 0, "", 0f, 0f, "", 0, 0f, 0f, "auraRate:0.5", "", "", 0f, "", 0, 0f, 0f, 0f, 0, "SkillAura", "", "Par_LargeMagicCircleGreen", "", "", "", "", "", "hpRegen+8");
            config[2021102] = new SkillBaseConfig(2021102, "鼓舞", "孙乾", 1.2f, "", "光环", "光环", new string[]{}, 2, 0f, 3f, 120, 0f, "", 0, "", 0f, 0f, "", 0, 0f, 0f, "auraRate:0.5", "", "", 0f, "", 0, 0f, 0f, 0f, 0, "SkillAura", "", "Par_LargeMagicCircleGreen", "", "", "", "", "", "hpRegen+12");
            config[2021103] = new SkillBaseConfig(2021103, "鼓舞", "孙乾", 1.2f, "", "光环", "光环", new string[]{}, 3, 0f, 3f, 120, 0f, "", 0, "", 0f, 0f, "", 0, 0f, 0f, "auraRate:0.5", "", "", 0f, "", 0, 0f, 0f, 0f, 0, "SkillAura", "", "Par_LargeMagicCircleGreen", "", "", "", "", "", "hpRegen+18");
            config[2021104] = new SkillBaseConfig(2021104, "鼓舞", "孙乾", 1.2f, "", "光环", "光环", new string[]{}, 4, 0f, 3f, 120, 0f, "", 0, "", 0f, 0f, "", 0, 0f, 0f, "auraRate:0.5", "", "", 0f, "", 0, 0f, 0f, 0f, 0, "SkillAura", "", "Par_LargeMagicCircleGreen", "", "", "", "", "", "hpRegen+26");
            config[2021105] = new SkillBaseConfig(2021105, "鼓舞", "孙乾", 1.2f, "", "光环", "光环", new string[]{}, 5, 0f, 3f, 120, 0f, "", 0, "", 0f, 0f, "", 0, 0f, 0f, "auraRate:0.5", "", "", 0f, "", 0, 0f, 0f, 0f, 0, "SkillAura", "", "Par_LargeMagicCircleGreen", "", "", "", "", "", "hpRegen+36");
            config[2021106] = new SkillBaseConfig(2021106, "安民", "费祎", 1.0f, "我方全体防御/auroattrs-armor，随时间增强", "光环", "光环", new string[]{}, 1, 0f, 3f, 120, 0f, "", 0, "", 0f, 0f, "", 0, 0f, 0f, "auraRate:0.5", "", "", 0f, "", 0, 0f, 0f, 0f, 0, "SkillAura", "", "Par_PurpleField", "", "", "", "", "", "armor+5");
            config[2021107] = new SkillBaseConfig(2021107, "安民", "费祎", 1.0f, "", "光环", "光环", new string[]{}, 2, 0f, 3f, 120, 0f, "", 0, "", 0f, 0f, "", 0, 0f, 0f, "auraRate:0.5", "", "", 0f, "", 0, 0f, 0f, 0f, 0, "SkillAura", "", "Par_PurpleField", "", "", "", "", "", "armor+7");
            config[2021108] = new SkillBaseConfig(2021108, "安民", "费祎", 1.0f, "", "光环", "光环", new string[]{}, 3, 0f, 3f, 120, 0f, "", 0, "", 0f, 0f, "", 0, 0f, 0f, "auraRate:0.5", "", "", 0f, "", 0, 0f, 0f, 0f, 0, "SkillAura", "", "Par_PurpleField", "", "", "", "", "", "armor+12");
            config[2021109] = new SkillBaseConfig(2021109, "安民", "费祎", 1.0f, "", "光环", "光环", new string[]{}, 4, 0f, 3f, 120, 0f, "", 0, "", 0f, 0f, "", 0, 0f, 0f, "auraRate:0.5", "", "", 0f, "", 0, 0f, 0f, 0f, 0, "SkillAura", "", "Par_PurpleField", "", "", "", "", "", "armor+18");
            config[2021110] = new SkillBaseConfig(2021110, "安民", "费祎", 1.0f, "", "光环", "光环", new string[]{}, 5, 0f, 3f, 120, 0f, "", 0, "", 0f, 0f, "", 0, 0f, 0f, "auraRate:0.5", "", "", 0f, "", 0, 0f, 0f, 0f, 0, "SkillAura", "", "Par_PurpleField", "", "", "", "", "", "armor+26");
            config[2021131] = new SkillBaseConfig(2021131, "礼乐", "荀顗", 1.1f, "我方全体攻速/auroattrs-atkspeed%，随时间增强", "光环", "光环", new string[]{}, 1, 0f, 3f, 120, 0f, "", 0, "", 0f, 0f, "", 0, 0f, 0f, "auraRate:0.5", "", "", 0f, "", 0, 0f, 0f, 0f, 0, "SkillAura", "", "Par_PurpleField", "", "", "", "", "", "atkspeed+0.03");
            config[2021132] = new SkillBaseConfig(2021132, "礼乐", "荀顗", 1.1f, "", "光环", "光环", new string[]{}, 2, 0f, 3f, 120, 0f, "", 0, "", 0f, 0f, "", 0, 0f, 0f, "auraRate:0.5", "", "", 0f, "", 0, 0f, 0f, 0f, 0, "SkillAura", "", "Par_PurpleField", "", "", "", "", "", "atkspeed+0.05");
            config[2021133] = new SkillBaseConfig(2021133, "礼乐", "荀顗", 1.1f, "", "光环", "光环", new string[]{}, 3, 0f, 3f, 120, 0f, "", 0, "", 0f, 0f, "", 0, 0f, 0f, "auraRate:0.5", "", "", 0f, "", 0, 0f, 0f, 0f, 0, "SkillAura", "", "Par_PurpleField", "", "", "", "", "", "atkspeed+0.07");
            config[2021134] = new SkillBaseConfig(2021134, "礼乐", "荀顗", 1.1f, "", "光环", "光环", new string[]{}, 4, 0f, 3f, 120, 0f, "", 0, "", 0f, 0f, "", 0, 0f, 0f, "auraRate:0.5", "", "", 0f, "", 0, 0f, 0f, 0f, 0, "SkillAura", "", "Par_PurpleField", "", "", "", "", "", "atkspeed+0.10");
            config[2021135] = new SkillBaseConfig(2021135, "礼乐", "荀顗", 1.1f, "", "光环", "光环", new string[]{}, 5, 0f, 3f, 120, 0f, "", 0, "", 0f, 0f, "", 0, 0f, 0f, "auraRate:0.5", "", "", 0f, "", 0, 0f, 0f, 0f, 0, "SkillAura", "", "Par_PurpleField", "", "", "", "", "", "atkspeed+0.14");
            config[2021201] = new SkillBaseConfig(2021201, "强阵", "郭图", 1.1f, "使范围内一名友军攻速提升/strengthbuff1-atkSpeed%、暴击率提升/strengthbuff1-critRate%，持续/bufftime秒", "单增", "单体友方增益", new string[]{"加攻速大","暴击大"}, 1, 0f, 3f, 60, 0f, "", 0, "", 50f, 100f, "", 1, 0f, 0f, "", "atkSpeed:0.25,critRate:0.1", "快,暴", 5f, "", 0, 0f, 0f, 0f, 0, "SkillAreaBuff", "sway", "MagicChargeGreen", "StarExplosionBlue", "", "", "", "", "");
            config[2021202] = new SkillBaseConfig(2021202, "强阵", "郭图", 1.1f, "", "单增", "单体友方增益", new string[]{"加攻速大","暴击大"}, 2, 0f, 3f, 60, 0f, "", 0, "", 50f, 100f, "", 1, 0f, 0f, "", "atkSpeed:0.35,critRate:0.14", "快,暴", 5f, "", 0, 0f, 0f, 0f, 0, "SkillAreaBuff", "sway", "MagicChargeGreen", "StarExplosionBlue", "", "", "", "", "");
            config[2021203] = new SkillBaseConfig(2021203, "强阵", "郭图", 1.1f, "", "单增", "单体友方增益", new string[]{"加攻速大","暴击大"}, 3, 0f, 3f, 60, 0f, "", 0, "", 50f, 100f, "", 1, 0f, 0f, "", "atkSpeed:0.5,critRate:0.18", "快,暴", 5f, "", 0, 0f, 0f, 0f, 0, "SkillAreaBuff", "sway", "MagicChargeGreen", "StarExplosionBlue", "", "", "", "", "");
            config[2021204] = new SkillBaseConfig(2021204, "强阵", "郭图", 1.1f, "", "单增", "单体友方增益", new string[]{"加攻速大","暴击大"}, 4, 0f, 3f, 60, 0f, "", 0, "", 50f, 100f, "", 1, 0f, 0f, "", "atkSpeed:0.65,critRate:0.24", "快,暴", 5f, "", 0, 0f, 0f, 0f, 0, "SkillAreaBuff", "sway", "MagicChargeGreen", "StarExplosionBlue", "", "", "", "", "");
            config[2021205] = new SkillBaseConfig(2021205, "强阵", "郭图", 1.1f, "", "单增", "单体友方增益", new string[]{"加攻速大","暴击大"}, 5, 0f, 3f, 60, 0f, "", 0, "", 50f, 100f, "", 1, 0f, 0f, "", "atkSpeed:0.8,critRate:0.3", "快,暴", 5f, "", 0, 0f, 0f, 0f, 0, "SkillAreaBuff", "sway", "MagicChargeGreen", "StarExplosionBlue", "", "", "", "", "");
            config[2021206] = new SkillBaseConfig(2021206, "洛神", "甄宓", 1.2f, "为我方远程英雄提升/strengthbuff1-atk点攻击力（无远程英雄时改为生命比例最高的英雄），持续/bufftime秒", "单增", "单体友方增益", new string[]{"加攻大","远程优先","排除自身","不按首段去重","英雄限定"}, 1, 0f, 3f, 60, 0f, "", 0, "", 50f, 80f, "heroOnly,excludeSelf,rangedOnly,noDedupe", 1, 0f, 0f, "", "atk:50", "攻", 5f, "", 0, 0f, 0f, 0f, 0, "SkillAreaBuff", "sway", "MagicBuffGreen", "MagicChargePink", "", "", "", "", "");
            config[2021207] = new SkillBaseConfig(2021207, "洛神", "甄宓", 1.2f, "", "单增", "单体友方增益", new string[]{"加攻大","远程优先","排除自身","不按首段去重","英雄限定"}, 2, 0f, 3f, 60, 0f, "", 0, "", 50f, 80f, "heroOnly,excludeSelf,rangedOnly,noDedupe", 1, 0f, 0f, "", "atk:75", "攻", 5f, "", 0, 0f, 0f, 0f, 0, "SkillAreaBuff", "sway", "MagicBuffGreen", "MagicChargePink", "", "", "", "", "");
            config[2021208] = new SkillBaseConfig(2021208, "洛神", "甄宓", 1.2f, "", "单增", "单体友方增益", new string[]{"加攻大","远程优先","排除自身","不按首段去重","英雄限定"}, 3, 0f, 3f, 60, 0f, "", 0, "", 50f, 80f, "heroOnly,excludeSelf,rangedOnly,noDedupe", 1, 0f, 0f, "", "atk:120", "攻", 5f, "", 0, 0f, 0f, 0f, 0, "SkillAreaBuff", "sway", "MagicBuffGreen", "MagicChargePink", "", "", "", "", "");
            config[2021209] = new SkillBaseConfig(2021209, "洛神", "甄宓", 1.2f, "", "单增", "单体友方增益", new string[]{"加攻大","远程优先","排除自身","不按首段去重","英雄限定"}, 4, 0f, 3f, 60, 0f, "", 0, "", 50f, 80f, "heroOnly,excludeSelf,rangedOnly,noDedupe", 1, 0f, 0f, "", "atk:200", "攻", 5f, "", 0, 0f, 0f, 0f, 0, "SkillAreaBuff", "sway", "MagicBuffGreen", "MagicChargePink", "", "", "", "", "");
            config[2021210] = new SkillBaseConfig(2021210, "洛神", "甄宓", 1.2f, "", "单增", "单体友方增益", new string[]{"加攻大","远程优先","排除自身","不按首段去重","英雄限定"}, 5, 0f, 3f, 60, 0f, "", 0, "", 50f, 80f, "heroOnly,excludeSelf,rangedOnly,noDedupe", 1, 0f, 0f, "", "atk:300", "攻", 5f, "", 0, 0f, 0f, 0f, 0, "SkillAreaBuff", "sway", "MagicBuffGreen", "MagicChargePink", "", "", "", "", "");
            config[2021211] = new SkillBaseConfig(2021211, "胡笳", "蔡文姬", 1.0f, "【持续施法】在自身周围展开治疗领域，持续/summontime秒，每秒为范围内生命缺口最大的/targetcount个我方单位（含英雄与小兵）回复/healstrength点生命", "群疗", "召唤治疗场", new string[]{}, 1, 0f, 2.5f, 140, 0f, "", 0, "", 0f, 50f, "", 5, 0f, 30f, "", "", "", 0f, "疗", 0, 5f, 1f, 0f, 0, "SkillCastingAidAreaHeal", "sway", "HeartStream", "MagicChargeYellow", "MagicAuraBlue", "", "", "", "");
            config[2021212] = new SkillBaseConfig(2021212, "胡笳", "蔡文姬", 1.0f, "", "群疗", "召唤治疗场", new string[]{}, 2, 0f, 2.5f, 140, 0f, "", 0, "", 0f, 50f, "", 5, 0f, 45f, "", "", "", 0f, "疗", 0, 5f, 1f, 0f, 0, "SkillCastingAidAreaHeal", "sway", "HeartStream", "MagicChargeYellow", "MagicAuraBlue", "", "", "", "");
            config[2021213] = new SkillBaseConfig(2021213, "胡笳", "蔡文姬", 1.0f, "", "群疗", "召唤治疗场", new string[]{}, 3, 0f, 2.5f, 140, 0f, "", 0, "", 0f, 50f, "", 5, 0f, 65f, "", "", "", 0f, "疗", 0, 5f, 1f, 0f, 0, "SkillCastingAidAreaHeal", "sway", "HeartStream", "MagicChargeYellow", "MagicAuraBlue", "", "", "", "");
            config[2021214] = new SkillBaseConfig(2021214, "胡笳", "蔡文姬", 1.0f, "", "群疗", "召唤治疗场", new string[]{}, 4, 0f, 2.5f, 140, 0f, "", 0, "", 0f, 50f, "", 5, 0f, 100f, "", "", "", 0f, "疗", 0, 5f, 1f, 0f, 0, "SkillCastingAidAreaHeal", "sway", "HeartStream", "MagicChargeYellow", "MagicAuraBlue", "", "", "", "");
            config[2021215] = new SkillBaseConfig(2021215, "胡笳", "蔡文姬", 1.0f, "", "群疗", "召唤治疗场", new string[]{}, 5, 0f, 2.5f, 140, 0f, "", 0, "", 0f, 50f, "", 5, 0f, 150f, "", "", "", 0f, "疗", 0, 5f, 1f, 0f, 0, "SkillCastingAidAreaHeal", "sway", "HeartStream", "MagicChargeYellow", "MagicAuraBlue", "", "", "", "");
            config[2021216] = new SkillBaseConfig(2021216, "鼓舞", "小乔", 1.0f, "使范围内一名友军攻速提升/strengthbuff1-atkSpeed%、移速提升/strengthbuff1-moveSpeed%，持续/bufftime秒", "单增", "单体友方增益", new string[]{"加攻速大","加移速大"}, 1, 0f, 3f, 60, 0f, "", 0, "", 50f, 100f, "", 1, 0f, 0f, "", "atkSpeed:0.3,moveSpeed:0.3", "快,速", 5f, "", 0, 0f, 0f, 0f, 0, "SkillAreaBuff", "sway", "MagicChargeGreen", "StarExplosionBlue", "", "", "", "", "");
            config[2021217] = new SkillBaseConfig(2021217, "鼓舞", "小乔", 1.0f, "", "单增", "单体友方增益", new string[]{"加攻速大","加移速大"}, 2, 0f, 3f, 60, 0f, "", 0, "", 50f, 100f, "", 1, 0f, 0f, "", "atkSpeed:0.45,moveSpeed:0.45", "快,速", 5f, "", 0, 0f, 0f, 0f, 0, "SkillAreaBuff", "sway", "MagicChargeGreen", "StarExplosionBlue", "", "", "", "", "");
            config[2021218] = new SkillBaseConfig(2021218, "鼓舞", "小乔", 1.0f, "", "单增", "单体友方增益", new string[]{"加攻速大","加移速大"}, 3, 0f, 3f, 60, 0f, "", 0, "", 50f, 100f, "", 1, 0f, 0f, "", "atkSpeed:0.6,moveSpeed:0.6", "快,速", 5f, "", 0, 0f, 0f, 0f, 0, "SkillAreaBuff", "sway", "MagicChargeGreen", "StarExplosionBlue", "", "", "", "", "");
            config[2021219] = new SkillBaseConfig(2021219, "鼓舞", "小乔", 1.0f, "", "单增", "单体友方增益", new string[]{"加攻速大","加移速大"}, 4, 0f, 3f, 60, 0f, "", 0, "", 50f, 100f, "", 1, 0f, 0f, "", "atkSpeed:0.78,moveSpeed:0.78", "快,速", 5f, "", 0, 0f, 0f, 0f, 0, "SkillAreaBuff", "sway", "MagicChargeGreen", "StarExplosionBlue", "", "", "", "", "");
            config[2021220] = new SkillBaseConfig(2021220, "鼓舞", "小乔", 1.0f, "", "单增", "单体友方增益", new string[]{"加攻速大","加移速大"}, 5, 0f, 3f, 60, 0f, "", 0, "", 50f, 100f, "", 1, 0f, 0f, "", "atkSpeed:0.96,moveSpeed:0.96", "快,速", 5f, "", 0, 0f, 0f, 0f, 0, "SkillAreaBuff", "sway", "MagicChargeGreen", "StarExplosionBlue", "", "", "", "", "");
            config[2021226] = new SkillBaseConfig(2021226, "国色", "大乔", 1.0f, "使范围内生命比例最高的近战友军提升/strengthbuff1-armor点护甲（无近战友军时改为生命比例最高的友军），持续/bufftime秒", "单增", "单体友方增益", new string[]{"加护甲大","最高血量","近战优先"}, 1, 0f, 3f, 60, 0f, "", 0, "", 50f, 100f, "highestHp,meleeOnly", 1, 0f, 0f, "", "armor:50", "甲", 5f, "", 0, 0f, 0f, 0f, 0, "SkillAreaBuff", "sway", "MagicChargeYellow", "MagicBuffGreen", "MagicAuraBlue", "", "", "", "");
            config[2021227] = new SkillBaseConfig(2021227, "国色", "大乔", 1.0f, "", "单增", "单体友方增益", new string[]{"加护甲大","最高血量","近战优先"}, 2, 0f, 3f, 60, 0f, "", 0, "", 50f, 100f, "highestHp,meleeOnly", 1, 0f, 0f, "", "armor:80", "甲", 5f, "", 0, 0f, 0f, 0f, 0, "SkillAreaBuff", "sway", "MagicChargeYellow", "MagicBuffGreen", "MagicAuraBlue", "", "", "", "");
            config[2021228] = new SkillBaseConfig(2021228, "国色", "大乔", 1.0f, "", "单增", "单体友方增益", new string[]{"加护甲大","最高血量","近战优先"}, 3, 0f, 3f, 60, 0f, "", 0, "", 50f, 100f, "highestHp,meleeOnly", 1, 0f, 0f, "", "armor:120", "甲", 5f, "", 0, 0f, 0f, 0f, 0, "SkillAreaBuff", "sway", "MagicChargeYellow", "MagicBuffGreen", "MagicAuraBlue", "", "", "", "");
            config[2021229] = new SkillBaseConfig(2021229, "国色", "大乔", 1.0f, "", "单增", "单体友方增益", new string[]{"加护甲大","最高血量","近战优先"}, 4, 0f, 3f, 60, 0f, "", 0, "", 50f, 100f, "highestHp,meleeOnly", 1, 0f, 0f, "", "armor:200", "甲", 5f, "", 0, 0f, 0f, 0f, 0, "SkillAreaBuff", "sway", "MagicChargeYellow", "MagicBuffGreen", "MagicAuraBlue", "", "", "", "");
            config[2021230] = new SkillBaseConfig(2021230, "国色", "大乔", 1.0f, "", "单增", "单体友方增益", new string[]{"加护甲大","最高血量","近战优先"}, 5, 0f, 3f, 60, 0f, "", 0, "", 50f, 100f, "highestHp,meleeOnly", 1, 0f, 0f, "", "armor:300", "甲", 5f, "", 0, 0f, 0f, 0f, 0, "SkillAreaBuff", "sway", "MagicChargeYellow", "MagicBuffGreen", "MagicAuraBlue", "", "", "", "");
            config[2021231] = new SkillBaseConfig(2021231, "广陵", "嵇康", 1.1f, "使范围内一名友军攻速提升/strengthbuff1-atkSpeed%、移速提升/strengthbuff1-moveSpeed%，持续/bufftime秒", "单增", "单体友方增益", new string[]{"加攻速大","加移速大"}, 1, 0f, 3f, 60, 0f, "", 0, "", 50f, 100f, "", 1, 0f, 0f, "", "atkSpeed:0.3,moveSpeed:0.3", "快,速", 5f, "", 0, 0f, 0f, 0f, 0, "SkillAreaBuff", "sway", "MagicChargeGreen", "StarExplosionBlue", "", "", "", "", "");
            config[2021232] = new SkillBaseConfig(2021232, "广陵", "嵇康", 1.1f, "", "单增", "单体友方增益", new string[]{"加攻速大","加移速大"}, 2, 0f, 3f, 60, 0f, "", 0, "", 50f, 100f, "", 1, 0f, 0f, "", "atkSpeed:0.45,moveSpeed:0.45", "快,速", 5f, "", 0, 0f, 0f, 0f, 0, "SkillAreaBuff", "sway", "MagicChargeGreen", "StarExplosionBlue", "", "", "", "", "");
            config[2021233] = new SkillBaseConfig(2021233, "广陵", "嵇康", 1.1f, "", "单增", "单体友方增益", new string[]{"加攻速大","加移速大"}, 3, 0f, 3f, 60, 0f, "", 0, "", 50f, 100f, "", 1, 0f, 0f, "", "atkSpeed:0.6,moveSpeed:0.6", "快,速", 5f, "", 0, 0f, 0f, 0f, 0, "SkillAreaBuff", "sway", "MagicChargeGreen", "StarExplosionBlue", "", "", "", "", "");
            config[2021234] = new SkillBaseConfig(2021234, "广陵", "嵇康", 1.1f, "", "单增", "单体友方增益", new string[]{"加攻速大","加移速大"}, 4, 0f, 3f, 60, 0f, "", 0, "", 50f, 100f, "", 1, 0f, 0f, "", "atkSpeed:0.78,moveSpeed:0.78", "快,速", 5f, "", 0, 0f, 0f, 0f, 0, "SkillAreaBuff", "sway", "MagicChargeGreen", "StarExplosionBlue", "", "", "", "", "");
            config[2021235] = new SkillBaseConfig(2021235, "广陵", "嵇康", 1.1f, "", "单增", "单体友方增益", new string[]{"加攻速大","加移速大"}, 5, 0f, 3f, 60, 0f, "", 0, "", 50f, 100f, "", 1, 0f, 0f, "", "atkSpeed:0.96,moveSpeed:0.96", "快,速", 5f, "", 0, 0f, 0f, 0f, 0, "SkillAreaBuff", "sway", "MagicChargeGreen", "StarExplosionBlue", "", "", "", "", "");
            config[2021246] = new SkillBaseConfig(2021246, "淑德", "王元姬", 1.1f, "治疗生命最低的友军及其/area范围内最多/targetcount名友军各/healstrength点生命，并驱散其负面buff", "群疗", "范围治疗", new string[]{}, 1, 0f, 3f, 90, 0f, "", 0, "", 50f, 20f, "", 4, 0f, 70f, "", "", "", 0f, "", 0, 0f, 0f, 0f, 0, "SkillAreaHeal", "sway", "HeartStream", "MagicBuffGreen", "AuraSoftPurple", "", "", "", "");
            config[2021247] = new SkillBaseConfig(2021247, "淑德", "王元姬", 1.1f, "", "群疗", "范围治疗", new string[]{}, 2, 0f, 3f, 90, 0f, "", 0, "", 50f, 20f, "", 4, 0f, 105f, "", "", "", 0f, "", 0, 0f, 0f, 0f, 0, "SkillAreaHeal", "sway", "HeartStream", "MagicBuffGreen", "AuraSoftPurple", "", "", "", "");
            config[2021248] = new SkillBaseConfig(2021248, "淑德", "王元姬", 1.1f, "", "群疗", "范围治疗", new string[]{}, 3, 0f, 3f, 90, 0f, "", 0, "", 50f, 20f, "", 4, 0f, 175f, "", "", "", 0f, "", 0, 0f, 0f, 0f, 0, "SkillAreaHeal", "sway", "HeartStream", "MagicBuffGreen", "AuraSoftPurple", "", "", "", "");
            config[2021249] = new SkillBaseConfig(2021249, "淑德", "王元姬", 1.1f, "", "群疗", "范围治疗", new string[]{}, 4, 0f, 3f, 90, 0f, "", 0, "", 50f, 20f, "", 4, 0f, 280f, "", "", "", 0f, "", 0, 0f, 0f, 0f, 0, "SkillAreaHeal", "sway", "HeartStream", "MagicBuffGreen", "AuraSoftPurple", "", "", "", "");
            config[2021250] = new SkillBaseConfig(2021250, "淑德", "王元姬", 1.1f, "", "群疗", "范围治疗", new string[]{}, 5, 0f, 3f, 90, 0f, "", 0, "", 50f, 20f, "", 4, 0f, 420f, "", "", "", 0f, "", 0, 0f, 0f, 0f, 0, "SkillAreaHeal", "sway", "HeartStream", "MagicBuffGreen", "AuraSoftPurple", "", "", "", "");
            config[2021301] = new SkillBaseConfig(2021301, "青囊", "华佗", 1.0f, "治疗生命最低的友军及其/area范围内最多/targetcount名友军各/healstrength点生命，并驱散其负面buff", "群疗", "范围治疗", new string[]{}, 1, 0f, 3f, 90, 0f, "", 0, "", 50f, 20f, "", 4, 0f, 70f, "", "", "", 0f, "", 0, 0f, 0f, 0f, 0, "SkillAreaHeal", "sway", "HeartStream", "MagicBuffGreen", "AuraSoftPurple", "", "", "", "");
            config[2021302] = new SkillBaseConfig(2021302, "青囊", "华佗", 1.0f, "", "群疗", "范围治疗", new string[]{}, 2, 0f, 3f, 90, 0f, "", 0, "", 50f, 20f, "", 4, 0f, 105f, "", "", "", 0f, "", 0, 0f, 0f, 0f, 0, "SkillAreaHeal", "sway", "HeartStream", "MagicBuffGreen", "AuraSoftPurple", "", "", "", "");
            config[2021303] = new SkillBaseConfig(2021303, "青囊", "华佗", 1.0f, "", "群疗", "范围治疗", new string[]{}, 3, 0f, 3f, 90, 0f, "", 0, "", 50f, 20f, "", 4, 0f, 175f, "", "", "", 0f, "", 0, 0f, 0f, 0f, 0, "SkillAreaHeal", "sway", "HeartStream", "MagicBuffGreen", "AuraSoftPurple", "", "", "", "");
            config[2021304] = new SkillBaseConfig(2021304, "青囊", "华佗", 1.0f, "", "群疗", "范围治疗", new string[]{}, 4, 0f, 3f, 90, 0f, "", 0, "", 50f, 20f, "", 4, 0f, 280f, "", "", "", 0f, "", 0, 0f, 0f, 0f, 0, "SkillAreaHeal", "sway", "HeartStream", "MagicBuffGreen", "AuraSoftPurple", "", "", "", "");
            config[2021305] = new SkillBaseConfig(2021305, "青囊", "华佗", 1.0f, "", "群疗", "范围治疗", new string[]{}, 5, 0f, 3f, 90, 0f, "", 0, "", 50f, 20f, "", 4, 0f, 420f, "", "", "", 0f, "", 0, 0f, 0f, 0f, 0, "SkillAreaHeal", "sway", "HeartStream", "MagicBuffGreen", "AuraSoftPurple", "", "", "", "");
            config[2021306] = new SkillBaseConfig(2021306, "妖咒", "于吉", 1.0f, "治疗范围内生命最低的友军英雄/healstrength点生命，并给最多/targetcount名敌人挂溃败，每秒造成/strengthbuff1-dot点法术伤害，持续/bufftime秒", "单疗", "单体治疗", new string[]{}, 1, 0f, 2f, 70, 0f, "", 0, "", 50f, 60f, "", 3, 0f, 100f, "", "dot:30", "败", 3f, "", 0, 0f, 0f, 0f, 0, "SkillAidCurseHeal", "sway", "HeartStream", "ShieldSoftBlue", "", "", "", "", "");
            config[2021307] = new SkillBaseConfig(2021307, "妖咒", "于吉", 1.0f, "", "单疗", "单体治疗", new string[]{}, 2, 0f, 2f, 70, 0f, "", 0, "", 50f, 60f, "", 3, 0f, 150f, "", "dot:45", "败", 3f, "", 0, 0f, 0f, 0f, 0, "SkillAidCurseHeal", "sway", "HeartStream", "ShieldSoftBlue", "", "", "", "", "");
            config[2021308] = new SkillBaseConfig(2021308, "妖咒", "于吉", 1.0f, "", "单疗", "单体治疗", new string[]{}, 3, 0f, 2f, 70, 0f, "", 0, "", 50f, 60f, "", 3, 0f, 250f, "", "dot:75", "败", 3f, "", 0, 0f, 0f, 0f, 0, "SkillAidCurseHeal", "sway", "HeartStream", "ShieldSoftBlue", "", "", "", "", "");
            config[2021309] = new SkillBaseConfig(2021309, "妖咒", "于吉", 1.0f, "", "单疗", "单体治疗", new string[]{}, 4, 0f, 2f, 70, 0f, "", 0, "", 50f, 60f, "", 3, 0f, 400f, "", "dot:120", "败", 3f, "", 0, 0f, 0f, 0f, 0, "SkillAidCurseHeal", "sway", "HeartStream", "ShieldSoftBlue", "", "", "", "", "");
            config[2021310] = new SkillBaseConfig(2021310, "妖咒", "于吉", 1.0f, "", "单疗", "单体治疗", new string[]{}, 5, 0f, 2f, 70, 0f, "", 0, "", 50f, 60f, "", 3, 0f, 600f, "", "dot:155", "败", 3f, "", 0, 0f, 0f, 0f, 0, "SkillAidCurseHeal", "sway", "HeartStream", "ShieldSoftBlue", "", "", "", "", "");
            config[2021311] = new SkillBaseConfig(2021311, "妖疗", "张宝", 1.0f, "跳跃治疗/targetcount名友军，首跳回复/healstrength点生命，每跳递减", "群疗", "跳跃弹射", new string[]{}, 1, 0f, 3f, 80, 0f, "", 0, "", 30f, 60f, "heal", 4, 0f, 60f, "", "", "", 0f, "", 0, 0f, 0f, 0f, 0, "SkillJumpChain", "sway", "HeartStream", "MagicChargeBlue", "MagicFieldGreen", "", "", "", "");
            config[2021312] = new SkillBaseConfig(2021312, "妖疗", "张宝", 1.0f, "", "群疗", "跳跃弹射", new string[]{}, 2, 0f, 3f, 80, 0f, "", 0, "", 30f, 60f, "heal", 4, 0f, 90f, "", "", "", 0f, "", 0, 0f, 0f, 0f, 0, "SkillJumpChain", "sway", "HeartStream", "MagicChargeBlue", "MagicFieldGreen", "", "", "", "");
            config[2021313] = new SkillBaseConfig(2021313, "妖疗", "张宝", 1.0f, "", "群疗", "跳跃弹射", new string[]{}, 3, 0f, 3f, 80, 0f, "", 0, "", 30f, 60f, "heal", 4, 0f, 150f, "", "", "", 0f, "", 0, 0f, 0f, 0f, 0, "SkillJumpChain", "sway", "HeartStream", "MagicChargeBlue", "MagicFieldGreen", "", "", "", "");
            config[2021314] = new SkillBaseConfig(2021314, "妖疗", "张宝", 1.0f, "", "群疗", "跳跃弹射", new string[]{}, 4, 0f, 3f, 80, 0f, "", 0, "", 30f, 60f, "heal", 4, 0f, 240f, "", "", "", 0f, "", 0, 0f, 0f, 0f, 0, "SkillJumpChain", "sway", "HeartStream", "MagicChargeBlue", "MagicFieldGreen", "", "", "", "");
            config[2021315] = new SkillBaseConfig(2021315, "妖疗", "张宝", 1.0f, "", "群疗", "跳跃弹射", new string[]{}, 5, 0f, 3f, 80, 0f, "", 0, "", 30f, 60f, "heal", 4, 0f, 360f, "", "", "", 0f, "", 0, 0f, 0f, 0f, 0, "SkillJumpChain", "sway", "HeartStream", "MagicChargeBlue", "MagicFieldGreen", "", "", "", "");
            config[2021401] = new SkillBaseConfig(2021401, "崩山", "王双", 1.1f, "对目标造成/damagestrength法术伤害，对盾兵与卫士造成双倍伤害，并降低其/strengthbuff1-armorDown点护甲，持续/bufftime秒", "单伤", "单体伤害小", new string[]{"减防大","兵种克制"}, 1, 1f, 1.5f, 50, 0f, "", 0, "", 35f, 0f, "vsJob", 0, 60f, 0f, "vsJob:盾|士,vsJobRate:2", "armorDown:20", "破", 4f, "", 0, 0f, 0f, 0f, 0, "SkillSingleDamage", "spin", "MagicChargeBlue", "BulletExplosionBlue", "", "", "", "", "");
            config[2021402] = new SkillBaseConfig(2021402, "崩山", "王双", 1.1f, "", "单伤", "单体伤害小", new string[]{"减防大","兵种克制"}, 2, 1f, 1.5f, 50, 0f, "", 0, "", 35f, 0f, "vsJob", 0, 90f, 0f, "vsJob:盾|士,vsJobRate:2", "armorDown:30", "破", 4f, "", 0, 0f, 0f, 0f, 0, "SkillSingleDamage", "spin", "MagicChargeBlue", "BulletExplosionBlue", "", "", "", "", "");
            config[2021403] = new SkillBaseConfig(2021403, "崩山", "王双", 1.1f, "", "单伤", "单体伤害小", new string[]{"减防大","兵种克制"}, 3, 1f, 1.5f, 50, 0f, "", 0, "", 35f, 0f, "vsJob", 0, 150f, 0f, "vsJob:盾|士,vsJobRate:2", "armorDown:45", "破", 4f, "", 0, 0f, 0f, 0f, 0, "SkillSingleDamage", "spin", "MagicChargeBlue", "BulletExplosionBlue", "", "", "", "", "");
            config[2021404] = new SkillBaseConfig(2021404, "崩山", "王双", 1.1f, "", "单伤", "单体伤害小", new string[]{"减防大","兵种克制"}, 4, 1f, 1.5f, 50, 0f, "", 0, "", 35f, 0f, "vsJob", 0, 240f, 0f, "vsJob:盾|士,vsJobRate:2", "armorDown:65", "破", 4f, "", 0, 0f, 0f, 0f, 0, "SkillSingleDamage", "spin", "MagicChargeBlue", "BulletExplosionBlue", "", "", "", "", "");
            config[2021405] = new SkillBaseConfig(2021405, "崩山", "王双", 1.1f, "", "单伤", "单体伤害小", new string[]{"减防大","兵种克制"}, 5, 1f, 1.5f, 50, 0f, "", 0, "", 35f, 0f, "vsJob", 0, 360f, 0f, "vsJob:盾|士,vsJobRate:2", "armorDown:90", "破", 4f, "", 0, 0f, 0f, 0f, 0, "SkillSingleDamage", "spin", "MagicChargeBlue", "BulletExplosionBlue", "", "", "", "", "");
            config[2021416] = new SkillBaseConfig(2021416, "偷刃", "潘璋", 1.3f, "对目标造成/damagestrength法术伤害，并降低其/strengthbuff1-armorDown点护甲，持续/bufftime秒", "单伤", "单体伤害中", new string[]{"减防大"}, 1, 0f, 2f, 60, 0f, "", 0, "", 30f, 0f, "", 0, 80f, 0f, "", "armorDown:20", "破", 4f, "", 0, 0f, 0f, 0f, 0, "SkillSingleDamage", "flipspin", "MagicChargePink", "BulletExplosionBlue", "", "", "", "", "");
            config[2021417] = new SkillBaseConfig(2021417, "偷刃", "潘璋", 1.3f, "", "单伤", "单体伤害中", new string[]{"减防大"}, 2, 0f, 2f, 60, 0f, "", 0, "", 30f, 0f, "", 0, 120f, 0f, "", "armorDown:30", "破", 4f, "", 0, 0f, 0f, 0f, 0, "SkillSingleDamage", "flipspin", "MagicChargePink", "BulletExplosionBlue", "", "", "", "", "");
            config[2021418] = new SkillBaseConfig(2021418, "偷刃", "潘璋", 1.3f, "", "单伤", "单体伤害中", new string[]{"减防大"}, 3, 0f, 2f, 60, 0f, "", 0, "", 30f, 0f, "", 0, 200f, 0f, "", "armorDown:45", "破", 4f, "", 0, 0f, 0f, 0f, 0, "SkillSingleDamage", "flipspin", "MagicChargePink", "BulletExplosionBlue", "", "", "", "", "");
            config[2021419] = new SkillBaseConfig(2021419, "偷刃", "潘璋", 1.3f, "", "单伤", "单体伤害中", new string[]{"减防大"}, 4, 0f, 2f, 60, 0f, "", 0, "", 30f, 0f, "", 0, 320f, 0f, "", "armorDown:60", "破", 4f, "", 0, 0f, 0f, 0f, 0, "SkillSingleDamage", "flipspin", "MagicChargePink", "BulletExplosionBlue", "", "", "", "", "");
            config[2021420] = new SkillBaseConfig(2021420, "偷刃", "潘璋", 1.3f, "", "单伤", "单体伤害中", new string[]{"减防大"}, 5, 0f, 2f, 60, 0f, "", 0, "", 30f, 0f, "", 0, 480f, 0f, "", "armorDown:80", "破", 4f, "", 0, 0f, 0f, 0f, 0, "SkillSingleDamage", "flipspin", "MagicChargePink", "BulletExplosionBlue", "", "", "", "", "");
            config[2021426] = new SkillBaseConfig(2021426, "裂盾", "曹洪", 1.0f, "对目标造成/damagestrength法术伤害，若目标带有护盾则本次伤害提升至/strength2-shieldBonusRate倍", "单伤", "单体伤害大", new string[]{"按护盾加成伤害"}, 1, 0f, 2f, 70, 0f, "", 0, "", 35f, 0f, "shieldBonus", 0, 100f, 0f, "shieldBonusRate:1.5", "", "", 0f, "", 0, 0f, 0f, 0f, 0, "SkillSingleDamage", "jumpspin", "MagicChargeBlue", "BulletExplosionGreen", "", "", "", "", "");
            config[2021427] = new SkillBaseConfig(2021427, "裂盾", "曹洪", 1.0f, "", "单伤", "单体伤害大", new string[]{"按护盾加成伤害"}, 2, 0f, 2f, 70, 0f, "", 0, "", 35f, 0f, "shieldBonus", 0, 150f, 0f, "shieldBonusRate:1.8", "", "", 0f, "", 0, 0f, 0f, 0f, 0, "SkillSingleDamage", "jumpspin", "MagicChargeBlue", "BulletExplosionGreen", "", "", "", "", "");
            config[2021428] = new SkillBaseConfig(2021428, "裂盾", "曹洪", 1.0f, "", "单伤", "单体伤害大", new string[]{"按护盾加成伤害"}, 3, 0f, 2f, 70, 0f, "", 0, "", 35f, 0f, "shieldBonus", 0, 250f, 0f, "shieldBonusRate:2", "", "", 0f, "", 0, 0f, 0f, 0f, 0, "SkillSingleDamage", "jumpspin", "MagicChargeBlue", "BulletExplosionGreen", "", "", "", "", "");
            config[2021429] = new SkillBaseConfig(2021429, "裂盾", "曹洪", 1.0f, "", "单伤", "单体伤害大", new string[]{"按护盾加成伤害"}, 4, 0f, 2f, 70, 0f, "", 0, "", 35f, 0f, "shieldBonus", 0, 400f, 0f, "shieldBonusRate:2.2", "", "", 0f, "", 0, 0f, 0f, 0f, 0, "SkillSingleDamage", "jumpspin", "MagicChargeBlue", "BulletExplosionGreen", "", "", "", "", "");
            config[2021430] = new SkillBaseConfig(2021430, "裂盾", "曹洪", 1.0f, "", "单伤", "单体伤害大", new string[]{"按护盾加成伤害"}, 5, 0f, 2f, 70, 0f, "", 0, "", 35f, 0f, "shieldBonus", 0, 600f, 0f, "shieldBonusRate:2.5", "", "", 0f, "", 0, 0f, 0f, 0f, 0, "SkillSingleDamage", "jumpspin", "MagicChargeBlue", "BulletExplosionGreen", "", "", "", "", "");
            config[2021431] = new SkillBaseConfig(2021431, "奋锤", "文钦", 1.2f, "进入狂暴状态，造成的伤害提升/strengthbuff1-damageUp%，但同时受到的伤害提升/strengthbuff1-damagedUp%，持续/bufftime秒", "自增", "自身强化", new string[]{"自身增伤大","增伤中"}, 1, 0f, 3f, 60, 0f, "", 0, "", 0f, 0f, "", 0, 0f, 0f, "", "damageUp:0.5,damagedUp:0.1", "重,伤", 5f, "", 0, 0f, 0f, 0f, 0, "SkillSelfBuff", "spin", "HeartStream", "", "", "", "", "", "");
            config[2021432] = new SkillBaseConfig(2021432, "奋锤", "文钦", 1.2f, "", "自增", "自身强化", new string[]{"自身增伤大","增伤中"}, 2, 0f, 3f, 60, 0f, "", 0, "", 0f, 0f, "", 0, 0f, 0f, "", "damageUp:0.6,damagedUp:0.12", "重,伤", 5f, "", 0, 0f, 0f, 0f, 0, "SkillSelfBuff", "spin", "HeartStream", "", "", "", "", "", "");
            config[2021433] = new SkillBaseConfig(2021433, "奋锤", "文钦", 1.2f, "", "自增", "自身强化", new string[]{"自身增伤大","增伤中"}, 3, 0f, 3f, 60, 0f, "", 0, "", 0f, 0f, "", 0, 0f, 0f, "", "damageUp:0.75,damagedUp:0.15", "重,伤", 5f, "", 0, 0f, 0f, 0f, 0, "SkillSelfBuff", "spin", "HeartStream", "", "", "", "", "", "");
            config[2021434] = new SkillBaseConfig(2021434, "奋锤", "文钦", 1.2f, "", "自增", "自身强化", new string[]{"自身增伤大","增伤中"}, 4, 0f, 3f, 60, 0f, "", 0, "", 0f, 0f, "", 0, 0f, 0f, "", "damageUp:0.95,damagedUp:0.18", "重,伤", 5f, "", 0, 0f, 0f, 0f, 0, "SkillSelfBuff", "spin", "HeartStream", "", "", "", "", "", "");
            config[2021435] = new SkillBaseConfig(2021435, "奋锤", "文钦", 1.2f, "", "自增", "自身强化", new string[]{"自身增伤大","增伤中"}, 5, 0f, 3f, 60, 0f, "", 0, "", 0f, 0f, "", 0, 0f, 0f, "", "damageUp:1.2,damagedUp:0.22", "重,伤", 5f, "", 0, 0f, 0f, 0f, 0, "SkillSelfBuff", "spin", "HeartStream", "", "", "", "", "", "");
            config[2021506] = new SkillBaseConfig(2021506, "守势", "马谡", 1.0f, "对目标造成/damagestrength法术伤害并陷阵/bufftime秒", "单控", "单体伤害中", new string[]{"定身中"}, 1, 0f, 2f, 60, 0f, "", 0, "", 30f, 0f, "", 0, 80f, 0f, "", "", "停", 1f, "", 0, 0f, 0f, 0f, 0, "SkillSingleDamage", "sway", "StarExplosionBlue", "StunnedCirclingStarsSimple", "", "", "", "", "");
            config[2021507] = new SkillBaseConfig(2021507, "守势", "马谡", 1.0f, "", "单控", "单体伤害中", new string[]{"定身中"}, 2, 0f, 2f, 60, 0f, "", 0, "", 30f, 0f, "", 0, 120f, 0f, "", "", "停", 1.3f, "", 0, 0f, 0f, 0f, 0, "SkillSingleDamage", "sway", "StarExplosionBlue", "StunnedCirclingStarsSimple", "", "", "", "", "");
            config[2021508] = new SkillBaseConfig(2021508, "守势", "马谡", 1.0f, "", "单控", "单体伤害中", new string[]{"定身中"}, 3, 0f, 2f, 60, 0f, "", 0, "", 30f, 0f, "", 0, 200f, 0f, "", "", "停", 1.6f, "", 0, 0f, 0f, 0f, 0, "SkillSingleDamage", "sway", "StarExplosionBlue", "StunnedCirclingStarsSimple", "", "", "", "", "");
            config[2021509] = new SkillBaseConfig(2021509, "守势", "马谡", 1.0f, "", "单控", "单体伤害中", new string[]{"定身中"}, 4, 0f, 2f, 60, 0f, "", 0, "", 30f, 0f, "", 0, 320f, 0f, "", "", "停", 2f, "", 0, 0f, 0f, 0f, 0, "SkillSingleDamage", "sway", "StarExplosionBlue", "StunnedCirclingStarsSimple", "", "", "", "", "");
            config[2021510] = new SkillBaseConfig(2021510, "守势", "马谡", 1.0f, "", "单控", "单体伤害中", new string[]{"定身中"}, 5, 0f, 2f, 60, 0f, "", 0, "", 30f, 0f, "", 0, 480f, 0f, "", "", "停", 2.5f, "", 0, 0f, 0f, 0f, 0, "SkillSingleDamage", "sway", "StarExplosionBlue", "StunnedCirclingStarsSimple", "", "", "", "", "");
            config[2021521] = new SkillBaseConfig(2021521, "承志", "关平", 1.1f, "腾空而起，以目标为圆心、/area为半径盘旋一圈并下降，落地对范围内敌人造成/damagestrength法术伤害", "群伤", "群体伤害近距离", new string[]{}, 1, 0f, 2f, 70, 0f, "", 0, "", 40f, 20f, "", 0, 70f, 0f, "", "", "", 0f, "", 0, 0f, 0f, 0f, 0, "SkillAidChargeImpale", "jumpspin", "MagicChargePink", "", "MysticExplosionRed", "", "", "", "");
            config[2021522] = new SkillBaseConfig(2021522, "承志", "关平", 1.1f, "", "群伤", "群体伤害近距离", new string[]{}, 2, 0f, 2f, 70, 0f, "", 0, "", 40f, 20f, "", 0, 105f, 0f, "", "", "", 0f, "", 0, 0f, 0f, 0f, 0, "SkillAidChargeImpale", "jumpspin", "MagicChargePink", "", "MysticExplosionRed", "", "", "", "");
            config[2021523] = new SkillBaseConfig(2021523, "承志", "关平", 1.1f, "", "群伤", "群体伤害近距离", new string[]{}, 3, 0f, 2f, 70, 0f, "", 0, "", 40f, 20f, "", 0, 175f, 0f, "", "", "", 0f, "", 0, 0f, 0f, 0f, 0, "SkillAidChargeImpale", "jumpspin", "MagicChargePink", "", "MysticExplosionRed", "", "", "", "");
            config[2021524] = new SkillBaseConfig(2021524, "承志", "关平", 1.1f, "", "群伤", "群体伤害近距离", new string[]{}, 4, 0f, 2f, 70, 0f, "", 0, "", 40f, 20f, "", 0, 280f, 0f, "", "", "", 0f, "", 0, 0f, 0f, 0f, 0, "SkillAidChargeImpale", "jumpspin", "MagicChargePink", "", "MysticExplosionRed", "", "", "", "");
            config[2021525] = new SkillBaseConfig(2021525, "承志", "关平", 1.1f, "", "群伤", "群体伤害近距离", new string[]{}, 5, 0f, 2f, 70, 0f, "", 0, "", 40f, 20f, "", 0, 420f, 0f, "", "", "", 0f, "", 0, 0f, 0f, 0f, 0, "SkillAidChargeImpale", "jumpspin", "MagicChargePink", "", "MysticExplosionRed", "", "", "", "");
            config[2021601] = new SkillBaseConfig(2021601, "奇袭", "魏延", 1.0f, "对/area范围内的敌人造成/damagestrength法术伤害并降低其治疗/strength2-healDown%，持续/bufftime秒", "群伤", "群体伤害自身中心", new string[]{"减疗中"}, 1, 0f, 2f, 70, 0f, "", 0, "", 30f, 20f, "self", 0, 70f, 0f, "healDown:0.2", "", "疫", 4f, "", 0, 0f, 0f, 0f, 0, "SkillAreaDamageDeBuff", "jumpspin", "MagicChargeBlue", "ShadowExplosion", "StormExplosion", "", "", "", "");
            config[2021602] = new SkillBaseConfig(2021602, "奇袭", "魏延", 1.0f, "", "群伤", "群体伤害自身中心", new string[]{"减疗中"}, 2, 0f, 2f, 70, 0f, "", 0, "", 30f, 20f, "self", 0, 105f, 0f, "healDown:0.2", "", "疫", 4f, "", 0, 0f, 0f, 0f, 0, "SkillAreaDamageDeBuff", "jumpspin", "MagicChargeBlue", "ShadowExplosion", "StormExplosion", "", "", "", "");
            config[2021603] = new SkillBaseConfig(2021603, "奇袭", "魏延", 1.0f, "", "群伤", "群体伤害自身中心", new string[]{"减疗中"}, 3, 0f, 2f, 70, 0f, "", 0, "", 30f, 20f, "self", 0, 175f, 0f, "healDown:0.25", "", "疫", 4f, "", 0, 0f, 0f, 0f, 0, "SkillAreaDamageDeBuff", "jumpspin", "MagicChargeBlue", "ShadowExplosion", "StormExplosion", "", "", "", "");
            config[2021604] = new SkillBaseConfig(2021604, "奇袭", "魏延", 1.0f, "", "群伤", "群体伤害自身中心", new string[]{"减疗中"}, 4, 0f, 2f, 70, 0f, "", 0, "", 30f, 20f, "self", 0, 280f, 0f, "healDown:0.25", "", "疫", 4f, "", 0, 0f, 0f, 0f, 0, "SkillAreaDamageDeBuff", "jumpspin", "MagicChargeBlue", "ShadowExplosion", "StormExplosion", "", "", "", "");
            config[2021605] = new SkillBaseConfig(2021605, "奇袭", "魏延", 1.0f, "", "群伤", "群体伤害自身中心", new string[]{"减疗中"}, 5, 0f, 2f, 70, 0f, "", 0, "", 30f, 20f, "self", 0, 420f, 0f, "healDown:0.3", "", "疫", 4f, "", 0, 0f, 0f, 0f, 0, "SkillAreaDamageDeBuff", "jumpspin", "MagicChargeBlue", "ShadowExplosion", "StormExplosion", "", "", "", "");
            config[2021621] = new SkillBaseConfig(2021621, "江河", "蒋钦", 1.1f, "对目标造成/damagestrength法术伤害并使其每秒流失/strengthbuff1-dot点生命，持续/bufftime秒", "单伤", "单体伤害小", new string[]{}, 1, 0f, 1.5f, 50, 0f, "", 0, "", 30f, 0f, "", 0, 60f, 0f, "", "dot:4", "败", 4f, "", 0, 0f, 0f, 0f, 0, "SkillSingleDamage", "throw", "StarExplosionBlue", "LightningExplosionRed", "", "", "", "", "");
            config[2021622] = new SkillBaseConfig(2021622, "江河", "蒋钦", 1.1f, "", "单伤", "单体伤害小", new string[]{}, 2, 0f, 1.5f, 50, 0f, "", 0, "", 30f, 0f, "", 0, 90f, 0f, "", "dot:6", "败", 4f, "", 0, 0f, 0f, 0f, 0, "SkillSingleDamage", "throw", "StarExplosionBlue", "LightningExplosionRed", "", "", "", "", "");
            config[2021623] = new SkillBaseConfig(2021623, "江河", "蒋钦", 1.1f, "", "单伤", "单体伤害小", new string[]{}, 3, 0f, 1.5f, 50, 0f, "", 0, "", 30f, 0f, "", 0, 150f, 0f, "", "dot:8", "败", 5f, "", 0, 0f, 0f, 0f, 0, "SkillSingleDamage", "throw", "StarExplosionBlue", "LightningExplosionRed", "", "", "", "", "");
            config[2021624] = new SkillBaseConfig(2021624, "江河", "蒋钦", 1.1f, "", "单伤", "单体伤害小", new string[]{}, 4, 0f, 1.5f, 50, 0f, "", 0, "", 30f, 0f, "", 0, 240f, 0f, "", "dot:11", "败", 5f, "", 0, 0f, 0f, 0f, 0, "SkillSingleDamage", "throw", "StarExplosionBlue", "LightningExplosionRed", "", "", "", "", "");
            config[2021625] = new SkillBaseConfig(2021625, "江河", "蒋钦", 1.1f, "", "单伤", "单体伤害小", new string[]{}, 5, 0f, 1.5f, 50, 0f, "", 0, "", 30f, 0f, "", 0, 360f, 0f, "", "dot:15", "败", 5f, "", 0, 0f, 0f, 0f, 0, "SkillSingleDamage", "throw", "StarExplosionBlue", "LightningExplosionRed", "", "", "", "", "");
            config[2021636] = new SkillBaseConfig(2021636, "举兵", "诸葛诞", 1.1f, "对/area范围内的敌人造成/damagestrength法术伤害并削减其护甲/strengthbuff1-armorDown点，持续/bufftime秒", "群伤", "群体伤害自身中心", new string[]{"减防大"}, 1, 0f, 2f, 70, 0f, "", 0, "", 30f, 20f, "self", 0, 70f, 0f, "", "armorDown:25", "破", 4f, "", 0, 0f, 0f, 0f, 0, "SkillAreaDamageDeBuff", "saw", "MagicChargePink", "SharpExplosionGreen", "PoisonExplosionSoftYellow", "", "", "", "");
            config[2021637] = new SkillBaseConfig(2021637, "举兵", "诸葛诞", 1.1f, "", "群伤", "群体伤害自身中心", new string[]{"减防大"}, 2, 0f, 2f, 70, 0f, "", 0, "", 30f, 20f, "self", 0, 105f, 0f, "", "armorDown:30", "破", 4f, "", 0, 0f, 0f, 0f, 0, "SkillAreaDamageDeBuff", "saw", "MagicChargePink", "SharpExplosionGreen", "PoisonExplosionSoftYellow", "", "", "", "");
            config[2021638] = new SkillBaseConfig(2021638, "举兵", "诸葛诞", 1.1f, "", "群伤", "群体伤害自身中心", new string[]{"减防大"}, 3, 0f, 2f, 70, 0f, "", 0, "", 30f, 20f, "self", 0, 175f, 0f, "", "armorDown:38", "破", 4f, "", 0, 0f, 0f, 0f, 0, "SkillAreaDamageDeBuff", "saw", "MagicChargePink", "SharpExplosionGreen", "PoisonExplosionSoftYellow", "", "", "", "");
            config[2021639] = new SkillBaseConfig(2021639, "举兵", "诸葛诞", 1.1f, "", "群伤", "群体伤害自身中心", new string[]{"减防大"}, 4, 0f, 2f, 70, 0f, "", 0, "", 30f, 20f, "self", 0, 280f, 0f, "", "armorDown:46", "破", 4f, "", 0, 0f, 0f, 0f, 0, "SkillAreaDamageDeBuff", "saw", "MagicChargePink", "SharpExplosionGreen", "PoisonExplosionSoftYellow", "", "", "", "");
            config[2021640] = new SkillBaseConfig(2021640, "举兵", "诸葛诞", 1.1f, "", "群伤", "群体伤害自身中心", new string[]{"减防大"}, 5, 0f, 2f, 70, 0f, "", 0, "", 30f, 20f, "self", 0, 420f, 0f, "", "armorDown:55", "破", 4f, "", 0, 0f, 0f, 0f, 0, "SkillAreaDamageDeBuff", "saw", "MagicChargePink", "SharpExplosionGreen", "PoisonExplosionSoftYellow", "", "", "", "");
            config[2021711] = new SkillBaseConfig(2021711, "卸甲", "刘晔", 1.0f, "对/area范围内的敌人造成/damagestrength法术伤害，并使其受到的伤害提升/strengthbuff1-damagedUp%，持续/bufftime秒", "群伤", "群体伤害自身中心", new string[]{"增伤大"}, 1, 0f, 2f, 70, 0f, "", 0, "", 0f, 20f, "self", 0, 70f, 0f, "", "damagedUp:0.3", "伤", 4f, "", 0, 0f, 0f, 0f, 0, "SkillAreaDamageDeBuff", "sway", "MagicChargeYellow", "SwordSlashMiniWhite", "MysticExplosionDark", "", "", "", "");
            config[2021712] = new SkillBaseConfig(2021712, "卸甲", "刘晔", 1.0f, "", "群伤", "群体伤害自身中心", new string[]{"增伤大"}, 2, 0f, 2f, 70, 0f, "", 0, "", 0f, 20f, "self", 0, 105f, 0f, "", "damagedUp:0.35", "伤", 4f, "", 0, 0f, 0f, 0f, 0, "SkillAreaDamageDeBuff", "sway", "MagicChargeYellow", "SwordSlashMiniWhite", "MysticExplosionDark", "", "", "", "");
            config[2021713] = new SkillBaseConfig(2021713, "卸甲", "刘晔", 1.0f, "", "群伤", "群体伤害自身中心", new string[]{"增伤大"}, 3, 0f, 2f, 70, 0f, "", 0, "", 0f, 20f, "self", 0, 175f, 0f, "", "damagedUp:0.45", "伤", 4f, "", 0, 0f, 0f, 0f, 0, "SkillAreaDamageDeBuff", "sway", "MagicChargeYellow", "SwordSlashMiniWhite", "MysticExplosionDark", "", "", "", "");
            config[2021714] = new SkillBaseConfig(2021714, "卸甲", "刘晔", 1.0f, "", "群伤", "群体伤害自身中心", new string[]{"增伤大"}, 4, 0f, 2f, 70, 0f, "", 0, "", 0f, 20f, "self", 0, 280f, 0f, "", "damagedUp:0.55", "伤", 4f, "", 0, 0f, 0f, 0f, 0, "SkillAreaDamageDeBuff", "sway", "MagicChargeYellow", "SwordSlashMiniWhite", "MysticExplosionDark", "", "", "", "");
            config[2021715] = new SkillBaseConfig(2021715, "卸甲", "刘晔", 1.0f, "", "群伤", "群体伤害自身中心", new string[]{"增伤大"}, 5, 0f, 2f, 70, 0f, "", 0, "", 0f, 20f, "self", 0, 420f, 0f, "", "damagedUp:0.7", "伤", 4f, "", 0, 0f, 0f, 0f, 0, "SkillAreaDamageDeBuff", "sway", "MagicChargeYellow", "SwordSlashMiniWhite", "MysticExplosionDark", "", "", "", "");
            config[2021726] = new SkillBaseConfig(2021726, "律令", "陈群", 1.0f, "对目标造成/damagestrength法术伤害，并使其受治疗降低/strength2-healDown%，持续/bufftime秒", "单伤", "单体伤害大", new string[]{"减疗中"}, 1, 0f, 2f, 70, 0f, "", 0, "", 40f, 0f, "", 0, 100f, 0f, "healDown:0.5", "", "疫", 4f, "", 0, 0f, 0f, 0f, 0, "SkillSingleDamage", "sway", "StarExplosionBlue", "SwordHitBlackRedCritical", "", "", "", "", "");
            config[2021727] = new SkillBaseConfig(2021727, "律令", "陈群", 1.0f, "", "单伤", "单体伤害大", new string[]{"减疗中"}, 2, 0f, 2f, 70, 0f, "", 0, "", 40f, 0f, "", 0, 150f, 0f, "healDown:0.6", "", "疫", 4f, "", 0, 0f, 0f, 0f, 0, "SkillSingleDamage", "sway", "StarExplosionBlue", "SwordHitBlackRedCritical", "", "", "", "", "");
            config[2021728] = new SkillBaseConfig(2021728, "律令", "陈群", 1.0f, "", "单伤", "单体伤害大", new string[]{"减疗中"}, 3, 0f, 2f, 70, 0f, "", 0, "", 40f, 0f, "", 0, 250f, 0f, "healDown:0.7", "", "疫", 4f, "", 0, 0f, 0f, 0f, 0, "SkillSingleDamage", "sway", "StarExplosionBlue", "SwordHitBlackRedCritical", "", "", "", "", "");
            config[2021729] = new SkillBaseConfig(2021729, "律令", "陈群", 1.0f, "", "单伤", "单体伤害大", new string[]{"减疗中"}, 4, 0f, 2f, 70, 0f, "", 0, "", 40f, 0f, "", 0, 400f, 0f, "healDown:0.8", "", "疫", 4f, "", 0, 0f, 0f, 0f, 0, "SkillSingleDamage", "sway", "StarExplosionBlue", "SwordHitBlackRedCritical", "", "", "", "", "");
            config[2021730] = new SkillBaseConfig(2021730, "律令", "陈群", 1.0f, "", "单伤", "单体伤害大", new string[]{"减疗中"}, 5, 0f, 2f, 70, 0f, "", 0, "", 40f, 0f, "", 0, 600f, 0f, "healDown:1", "", "疫", 4f, "", 0, 0f, 0f, 0f, 0, "SkillSingleDamage", "sway", "StarExplosionBlue", "SwordHitBlackRedCritical", "", "", "", "", "");
            config[2021736] = new SkillBaseConfig(2021736, "筑城", "娄圭", 1.1f, "为自身及周围最多/targetcount名友军附加/strength2-shield点护盾（随法强成长），持续/bufftime秒", "群增", "群体友方增益", new string[]{"护盾中","施法者必含","不按首段去重","英雄限定"}, 1, 0f, 4f, 80, 0f, "", 0, "", 0f, 20f, "selfAlways,heroOnly,noDedupe", 2, 0f, 0f, "shield:120", "", "盾", 5f, "", 0, 0f, 0f, 0f, 0, "SkillAreaBuff", "sway", "MagicChargeBlue", "", "", "dun", "", "", "");
            config[2021737] = new SkillBaseConfig(2021737, "筑城", "娄圭", 1.1f, "", "群增", "群体友方增益", new string[]{"护盾中","施法者必含","不按首段去重","英雄限定"}, 2, 0f, 4f, 80, 0f, "", 0, "", 0f, 20f, "selfAlways,heroOnly,noDedupe", 2, 0f, 0f, "shield:160", "", "盾", 5f, "", 0, 0f, 0f, 0f, 0, "SkillAreaBuff", "sway", "MagicChargeBlue", "", "", "dun", "", "", "");
            config[2021738] = new SkillBaseConfig(2021738, "筑城", "娄圭", 1.1f, "", "群增", "群体友方增益", new string[]{"护盾中","施法者必含","不按首段去重","英雄限定"}, 3, 0f, 4f, 80, 0f, "", 0, "", 0f, 20f, "selfAlways,heroOnly,noDedupe", 2, 0f, 0f, "shield:210", "", "盾", 5f, "", 0, 0f, 0f, 0f, 0, "SkillAreaBuff", "sway", "MagicChargeBlue", "", "", "dun", "", "", "");
            config[2021739] = new SkillBaseConfig(2021739, "筑城", "娄圭", 1.1f, "", "群增", "群体友方增益", new string[]{"护盾中","施法者必含","不按首段去重","英雄限定"}, 4, 0f, 4f, 80, 0f, "", 0, "", 0f, 20f, "selfAlways,heroOnly,noDedupe", 2, 0f, 0f, "shield:270", "", "盾", 5f, "", 0, 0f, 0f, 0f, 0, "SkillAreaBuff", "sway", "MagicChargeBlue", "", "", "dun", "", "", "");
            config[2021740] = new SkillBaseConfig(2021740, "筑城", "娄圭", 1.1f, "", "群增", "群体友方增益", new string[]{"护盾中","施法者必含","不按首段去重","英雄限定"}, 5, 0f, 4f, 80, 0f, "", 0, "", 0f, 20f, "selfAlways,heroOnly,noDedupe", 2, 0f, 0f, "shield:340", "", "盾", 5f, "", 0, 0f, 0f, 0f, 0, "SkillAreaBuff", "sway", "MagicChargeBlue", "", "", "dun", "", "", "");
            RebuildIndex();

        }

        private static void RebuildIndex()
        {
            foreach (var kv in config)
            {
            }
        }

        public static SkillBaseConfig GetConfig(int id)
        {
            SkillBaseConfig data;
            if (config.TryGetValue(id, out data))
            {
                return data;
            }
            throw new NullReferenceException(string.Format("配置表SkillBaseConfig不存在id={0}", id));
        }


        public static bool HasConfig(int id)
        {
            if (config.ContainsKey(id))
            {
                return true;
            }
            return false;
        }

        public static void Assign(int id, SkillBaseConfig configData)
        {
            config[id] = configData; 
        }

        public static void Add(int id, SkillBaseConfig configData)
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
