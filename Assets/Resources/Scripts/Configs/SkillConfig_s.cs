﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿using System;
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
            {"Descript", new FieldMetaInfo("说明(内联/字段名动态引用,如 /strength、/linkself-atk)", "string", 402)},
            {"Type", new FieldMetaInfo("分类", "string", 0)},
            {"Lv", new FieldMetaInfo("等级", "int", 60)},
            {"Rate", new FieldMetaInfo("发动概率", "float", 60)},
            {"CD", new FieldMetaInfo("发动cd", "float", 60)},
            {"MpCost", new FieldMetaInfo("技能MP消耗", "int", 60)},
            {"TriggerCondition", new FieldMetaInfo("触发条件", "string", 0)},
            {"DamageType", new FieldMetaInfo("伤害类型(0法术/1物理/2真实)", "int", 60, "0:法术,1:物理,2:真实")},
            {"HurtTag", new FieldMetaInfo("伤害标签(如AntiShield=绕过护盾打血)", "string", 0)},
            {"Range", new FieldMetaInfo("范围", "float", 60)},
            {"Area", new FieldMetaInfo("范围", "float", 60, "伤害范围等")},
            {"TargetType", new FieldMetaInfo("选取点", "string", 0)},
            {"TargetCount", new FieldMetaInfo("最大目标数", "int", 60)},
            {"Strength", new FieldMetaInfo("技能强度（恒定）", "float", 60)},
            {"Strength2", new FieldMetaInfo("伤害倍率系数（Strength被占用时的倍率槽）", "float", 60)},
            {"StrengthInt", new FieldMetaInfo("技能强度（恒定）", "int", 60)},
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
        ///说明（Lv1 为模板，内联 /字段名 动态引用，如 /strength、/linkself-atk；其余等级清空自动回退 Lv1 模板并按本级字段值替换。无动态引用的整型文案可直接写完整说明）
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
        ///技能MP消耗（0=不使用MP，>0则仅靠法力回复(mpRegen)充能，满才能发动，发动后清空）
        /// </summary>
        public int MpCost;
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
        ///固定系数（技能伤害=固定系数+比例系数×关联属性）
        /// </summary>
        public float Strength;
        /// <summary>
        ///强度（备用）/ 伤害倍率系数（Strength 被占用时的倍率槽，如公式 (1+Strength2)）
        /// </summary>
        public float Strength2;
        /// <summary>
        ///技能强度（恒定）
        /// </summary>
        public int StrengthInt;
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


        public SkillConfig(int Id, string Name, string Sname, string Descript, string Type, int Lv, float Rate, float CD, int MpCost, string TriggerCondition, int DamageType, string HurtTag, float Range, float Area, string TargetType, int TargetCount, float Strength, float Strength2, int StrengthInt, string BuffId, bool NegBuff, float BuffTime, string SummonTag, int SummonCount, float SummonTime, float SummonHitInterval, float SummonSpeed, int ItemId, string ScriptName, string Action, string HitEffect, float EffectSize, string Icon, string LinkSelf, string LinkTeam, string AuroAttrs)
        {
            this.Id = Id;
            this.Name = Name;
            this.Sname = Sname;
            this.Descript = Descript;
            this.Type = Type;
            this.Lv = Lv;
            this.Rate = Rate;
            this.CD = CD;
            this.MpCost = MpCost;
            this.TriggerCondition = TriggerCondition;
            this.DamageType = DamageType;
            this.HurtTag = HurtTag;
            this.Range = Range;
            this.Area = Area;
            this.TargetType = TargetType;
            this.TargetCount = TargetCount;
            this.Strength = Strength;
            this.Strength2 = Strength2;
            this.StrengthInt = StrengthInt;
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
            config[2000001] = new SkillConfig(2000001, "国家护盾", "国", "同阵营英雄获得最大生命+/strength的护盾", "职业", 1, 0f, 0f, 0, "", 1, "", 0f, 0f, "", 0, 0.15f, 0f, 0, "盾", false, 999f, "", 0, 0f, 0f, 0f, 0, "FactionShield", "", "", 0f, "shuai", "", "", "");
            config[2000002] = new SkillConfig(2000002, "国家护盾", "国", "", "职业", 2, 0f, 0f, 0, "", 1, "", 0f, 0f, "", 0, 0.3f, 0f, 0, "盾", false, 999f, "", 0, 0f, 0f, 0f, 0, "FactionShield", "", "", 0f, "shuai", "", "", "");
            config[2000003] = new SkillConfig(2000003, "国家护盾", "国", "", "职业", 3, 0f, 0f, 0, "", 1, "", 0f, 0f, "", 0, 0.45f, 0f, 0, "盾", false, 999f, "", 0, 0f, 0f, 0f, 0, "FactionShield", "", "", 0f, "shuai", "", "", "");
            config[2000004] = new SkillConfig(2000004, "国家护盾", "国", " ", "职业", 4, 0f, 0f, 0, "", 1, "", 0f, 0f, "", 0, 0.6f, 0f, 0, "盾", false, 999f, "", 0, 0f, 0f, 0f, 0, "FactionShield", "", "", 0f, "shuai", "", "", "");
            config[2000005] = new SkillConfig(2000005, "国家护盾", "国", "", "职业", 5, 0f, 0f, 0, "", 1, "", 0f, 0f, "", 0, 0.75f, 0f, 0, "盾", false, 999f, "", 0, 0f, 0f, 0f, 0, "FactionShield", "", "", 0f, "shuai", "", "", "");
            config[2000006] = new SkillConfig(2000006, "连线", "友", "连线好友提升自身攻击/linkself-atk", "连接", 1, 0f, 0f, 0, "", 1, "", 0f, 0f, "", 0, 0f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "InitAttrChange", "", "", 0f, "you", "atk+15", "", "");
            config[2000007] = new SkillConfig(2000007, "连线", "友", "", "连接", 2, 0f, 0f, 0, "", 1, "", 0f, 0f, "", 0, 0f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "InitAttrChange", "", "", 0f, "you", "atk+30", "", "");
            config[2000008] = new SkillConfig(2000008, "连线", "友", "", "连接", 3, 0f, 0f, 0, "", 1, "", 0f, 0f, "", 0, 0f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "InitAttrChange", "", "", 0f, "you", "atk+45", "", "");
            config[2000009] = new SkillConfig(2000009, "连线", "友", "", "连接", 4, 0f, 0f, 0, "", 1, "", 0f, 0f, "", 0, 0f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "InitAttrChange", "", "", 0f, "you", "atk+60", "", "");
            config[2000010] = new SkillConfig(2000010, "连线", "友", "", "连接", 5, 0f, 0f, 0, "", 1, "", 0f, 0f, "", 0, 0f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "InitAttrChange", "", "", 0f, "you", "atk+90", "", "");
            config[2000011] = new SkillConfig(2000011, "诸侯", "王", "自身攻击/linkself-atk，护甲/linkself-armor，生命/linkself-hp；阵营护盾额外/strength%", "职业", 1, 0f, 0f, 0, "", 1, "", 0f, 0f, "", 0, 0.1f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "Dumb", "", "", 0f, "shuai", "atk+5,armor+5,hp+50", "", "");
            config[2000012] = new SkillConfig(2000012, "诸侯", "王", "", "职业", 2, 0f, 0f, 0, "", 1, "", 0f, 0f, "", 0, 0.2f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "Dumb", "", "", 0f, "shuai", "atk+10,armor+10,hp+100", "", "");
            config[2000013] = new SkillConfig(2000013, "诸侯", "王", "", "职业", 3, 0f, 0f, 0, "", 1, "", 0f, 0f, "", 0, 0.3f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "Dumb", "", "", 0f, "shuai", "atk+20,armor+20,hp+200", "", "");
            config[2000014] = new SkillConfig(2000014, "诸侯", "王", "", "职业", 4, 0f, 0f, 0, "", 1, "", 0f, 0f, "", 0, 0.4f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "Dumb", "", "", 0f, "shuai", "atk+40,armor+40,hp+400", "", "");
            config[2000015] = new SkillConfig(2000015, "诸侯", "王", "", "职业", 5, 0f, 0f, 0, "", 1, "", 0f, 0f, "", 0, 0.5f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "Dumb", "", "", 0f, "shuai", "atk+60,armor+60,hp+600", "", "");
            config[2000021] = new SkillConfig(2000021, "羽扇", "扇", "自身施加的负面buff持续+/strength%，全队法力回复+/strengthInt/秒", "职业", 1, 0f, 0f, 0, "", 1, "", 0f, 0f, "", 0, .1f, 0f, 1, "", true, 0f, "", 0, 0f, 0f, 0f, 0, "ModifyBuffTime", "", "", 0f, "shan", "", "mpRegen+1", "");
            config[2000022] = new SkillConfig(2000022, "羽扇", "扇", "", "职业", 2, 0f, 0f, 0, "", 1, "", 0f, 0f, "", 0, .2f, 0f, 2, "", true, 0f, "", 0, 0f, 0f, 0f, 0, "ModifyBuffTime", "", "", 0f, "shan", "", "mpRegen+2", "");
            config[2000023] = new SkillConfig(2000023, "羽扇", "扇", "", "职业", 3, 0f, 0f, 0, "", 1, "", 0f, 0f, "", 0, .4f, 0f, 3, "", true, 0f, "", 0, 0f, 0f, 0f, 0, "ModifyBuffTime", "", "", 0f, "shan", "", "mpRegen+3", "");
            config[2000024] = new SkillConfig(2000024, "羽扇", "扇", "", "职业", 4, 0f, 0f, 0, "", 1, "", 0f, 0f, "", 0, .6f, 0f, 4, "", true, 0f, "", 0, 0f, 0f, 0f, 0, "ModifyBuffTime", "", "", 0f, "shan", "", "mpRegen+4", "");
            config[2000025] = new SkillConfig(2000025, "羽扇", "扇", "", "职业", 5, 0f, 0f, 0, "", 1, "", 0f, 0f, "", 0, 1f, 0f, 5, "", true, 0f, "", 0, 0f, 0f, 0f, 0, "ModifyBuffTime", "", "", 0f, "shan", "", "mpRegen+5", "");
            config[2000031] = new SkillConfig(2000031, "猛将", "锤", "自身生命低于50%时伤害+/strength", "职业", 1, 0f, 0f, 0, "hprate<50", 1, "", 0f, 0f, "", 0, 0.2f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AttackAddDamage", "", "", 0f, "dao", "", "", "");
            config[2000032] = new SkillConfig(2000032, "猛将", "锤", "", "职业", 2, 0f, 0f, 0, "hprate<50", 1, "", 0f, 0f, "", 0, 0.4f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AttackAddDamage", "", "", 0f, "dao", "", "", "");
            config[2000033] = new SkillConfig(2000033, "猛将", "锤", "", "职业", 3, 0f, 0f, 0, "hprate<50", 1, "", 0f, 0f, "", 0, 0.6f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AttackAddDamage", "", "", 0f, "dao", "", "", "");
            config[2000034] = new SkillConfig(2000034, "猛将", "锤", "", "职业", 4, 0f, 0f, 0, "hprate<50", 1, "", 0f, 0f, "", 0, 0.8f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AttackAddDamage", "", "", 0f, "dao", "", "", "");
            config[2000035] = new SkillConfig(2000035, "猛将", "锤", "", "职业", 5, 0f, 0f, 0, "hprate<50", 1, "", 0f, 0f, "", 0, 1f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AttackAddDamage", "", "", 0f, "dao", "", "", "");
            config[2000041] = new SkillConfig(2000041, "坚韧", "士", "自身生命/linkself-hp，全队生命/linkteam-hp", "职业", 1, 0f, 0f, 0, "", 1, "", 0f, 0f, "", 0, 0f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "Dumb", "", "", 0f, "shi", "hp+50", "hp+20", "");
            config[2000042] = new SkillConfig(2000042, "坚韧", "士", "", "职业", 2, 0f, 0f, 0, "", 1, "", 0f, 0f, "", 0, 0f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "Dumb", "", "", 0f, "shi", "hp+100", "hp+40", "");
            config[2000043] = new SkillConfig(2000043, "坚韧", "士", "", "职业", 3, 0f, 0f, 0, "", 1, "", 0f, 0f, "", 0, 0f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "Dumb", "", "", 0f, "shi", "hp+200", "hp+60", "");
            config[2000044] = new SkillConfig(2000044, "坚韧", "士", "", "职业", 4, 0f, 0f, 0, "", 1, "", 0f, 0f, "", 0, 0f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "Dumb", "", "", 0f, "shi", "hp+400", "hp+80", "");
            config[2000045] = new SkillConfig(2000045, "坚韧", "士", "", "职业", 5, 0f, 0f, 0, "", 1, "", 0f, 0f, "", 0, 0f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "Dumb", "", "", 0f, "shi", "hp+800", "hp+100", "");
            config[2000051] = new SkillConfig(2000051, "灵活", "马", "自身闪避/linkself-dodge%", "职业", 1, 0f, 0f, 0, "", 1, "", 0f, 0f, "", 0, 0f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "Dumb", "", "", 0f, "ma", "dodge+0.05", "", "");
            config[2000052] = new SkillConfig(2000052, "灵活", "马", "", "职业", 2, 0f, 0f, 0, "", 1, "", 0f, 0f, "", 0, 0f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "Dumb", "", "", 0f, "ma", "dodge+0.10", "", "");
            config[2000053] = new SkillConfig(2000053, "灵活", "马", "", "职业", 3, 0f, 0f, 0, "", 1, "", 0f, 0f, "", 0, 0f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "Dumb", "", "", 0f, "ma", "dodge+0.20", "", "");
            config[2000054] = new SkillConfig(2000054, "灵活", "马", "", "职业", 4, 0f, 0f, 0, "", 1, "", 0f, 0f, "", 0, 0f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "Dumb", "", "", 0f, "ma", "dodge+0.33", "", "");
            config[2000055] = new SkillConfig(2000055, "灵活", "马", "", "职业", 5, 0f, 0f, 0, "", 1, "", 0f, 0f, "", 0, 0f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "Dumb", "", "", 0f, "ma", "dodge+0.45", "", "");
            config[2000061] = new SkillConfig(2000061, "运筹", "相", "全军士兵攻击/linkself-soldierAtk%，生命/linkself-soldierHp%，全队法力回复/linkteam-mpRegen/秒", "职业", 1, 0f, 0f, 0, "", 1, "", 0f, 0f, "", 0, 0f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "Dumb", "", "", 0f, "xiang", "soldierAtk+0.1,soldierHp+0.1", "mpRegen+1", "");
            config[2000062] = new SkillConfig(2000062, "运筹", "相", "", "职业", 2, 0f, 0f, 0, "", 1, "", 0f, 0f, "", 0, 0f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "Dumb", "", "", 0f, "xiang", "soldierAtk+0.15,soldierHp+0.15", "mpRegen+2", "");
            config[2000063] = new SkillConfig(2000063, "运筹", "相", "", "职业", 3, 0f, 0f, 0, "", 1, "", 0f, 0f, "", 0, 0f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "Dumb", "", "", 0f, "xiang", "soldierAtk+0.2,soldierHp+0.2", "mpRegen+3", "");
            config[2000064] = new SkillConfig(2000064, "运筹", "相", "", "职业", 4, 0f, 0f, 0, "", 1, "", 0f, 0f, "", 0, 0f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "Dumb", "", "", 0f, "xiang", "soldierAtk+0.27,soldierHp+0.27", "mpRegen+4", "");
            config[2000065] = new SkillConfig(2000065, "运筹", "相", "", "职业", 5, 0f, 0f, 0, "", 1, "", 0f, 0f, "", 0, 0f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "Dumb", "", "", 0f, "xiang", "soldierAtk+0.35,soldierHp+0.35", "mpRegen+5", "");
            config[2000071] = new SkillConfig(2000071, "弓手", "弓", "自身攻击/linkself-atk，全队攻击/linkteam-atk", "职业", 1, 0f, 0f, 0, "", 1, "", 0f, 0f, "", 0, 0f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "Dumb", "", "", 0f, "gong", "atk+5", "atk+3", "");
            config[2000072] = new SkillConfig(2000072, "弓手", "弓", "", "职业", 2, 0f, 0f, 0, "", 1, "", 0f, 0f, "", 0, 0f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "Dumb", "", "", 0f, "gong", "atk+10", "atk+6", "");
            config[2000073] = new SkillConfig(2000073, "弓手", "弓", "", "职业", 3, 0f, 0f, 0, "", 1, "", 0f, 0f, "", 0, 0f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "Dumb", "", "", 0f, "gong", "atk+20", "atk+9", "");
            config[2000074] = new SkillConfig(2000074, "弓手", "弓", "", "职业", 4, 0f, 0f, 0, "", 1, "", 0f, 0f, "", 0, 0f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "Dumb", "", "", 0f, "gong", "atk+40", "atk+12", "");
            config[2000075] = new SkillConfig(2000075, "弓手", "弓", "", "职业", 5, 0f, 0f, 0, "", 1, "", 0f, 0f, "", 0, 0f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "Dumb", "", "", 0f, "gong", "atk+80", "atk+15", "");
            config[2000081] = new SkillConfig(2000081, "谋略", "棋", "自身法强/linkself-ap，全队法强/linkteam-ap", "职业", 1, 0f, 0f, 0, "", 1, "", 0f, 0f, "", 0, 0f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "Dumb", "", "", 0f, "mou", "ap+5", "ap+3", "");
            config[2000082] = new SkillConfig(2000082, "谋略", "棋", "", "职业", 2, 0f, 0f, 0, "", 1, "", 0f, 0f, "", 0, 0f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "Dumb", "", "", 0f, "mou", "ap+10", "ap+6", "");
            config[2000083] = new SkillConfig(2000083, "谋略", "棋", "", "职业", 3, 0f, 0f, 0, "", 1, "", 0f, 0f, "", 0, 0f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "Dumb", "", "", 0f, "mou", "ap+20", "ap+9", "");
            config[2000084] = new SkillConfig(2000084, "谋略", "棋", "", "职业", 4, 0f, 0f, 0, "", 1, "", 0f, 0f, "", 0, 0f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "Dumb", "", "", 0f, "mou", "ap+40", "ap+12", "");
            config[2000085] = new SkillConfig(2000085, "谋略", "棋", "", "职业", 5, 0f, 0f, 0, "", 1, "", 0f, 0f, "", 0, 0f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "Dumb", "", "", 0f, "mou", "ap+80", "ap+15", "");
            config[2000091] = new SkillConfig(2000091, "炮车", "炮", "攻击时/rate%几率对周围造成50%溅射伤害，范围/area", "职业", 1, 0.2f, 0f, 0, "", 1, "", 0f, 6f, "", 3, 0.5f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "HitArea", "", "SparkleAreaWhite", 0f, "pao", "", "", "");
            config[2000092] = new SkillConfig(2000092, "炮车", "炮", "", "职业", 2, 0.25f, 0f, 0, "", 1, "", 0f, 6.9f, "", 3, 0.5f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "HitArea", "", "SparkleAreaWhite", 0f, "pao", "", "", "");
            config[2000093] = new SkillConfig(2000093, "炮车", "炮", "", "职业", 3, 0.27f, 0f, 0, "", 1, "", 0f, 7.8f, "", 3, 0.5f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "HitArea", "", "SparkleAreaWhite", 0f, "pao", "", "", "");
            config[2000094] = new SkillConfig(2000094, "炮车", "炮", "", "职业", 4, 0.3f, 0f, 0, "", 1, "", 0f, 9f, "", 3, 0.5f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "HitArea", "", "SparkleAreaWhite", 0f, "pao", "", "", "");
            config[2000095] = new SkillConfig(2000095, "炮车", "炮", "", "职业", 5, 0.35f, 0f, 0, "", 1, "", 0f, 10.8f, "", 3, 0.5f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "HitArea", "", "SparkleAreaWhite", 0f, "pao", "", "", "");
            config[2000101] = new SkillConfig(2000101, "弩手", "弩", "自身攻速/linkself-atkspeed%，全队攻速/linkteam-atkspeed%", "职业", 1, 0f, 0f, 0, "", 1, "", 0f, 0f, "", 0, 0f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "Dumb", "", "", 0f, "nu", "atkspeed+0.1", "atkspeed+0.05", "");
            config[2000102] = new SkillConfig(2000102, "弩手", "弩", "", "职业", 2, 0f, 0f, 0, "", 1, "", 0f, 0f, "", 0, 0f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "Dumb", "", "", 0f, "nu", "atkspeed+0.2", "atkspeed+0.1", "");
            config[2000103] = new SkillConfig(2000103, "弩手", "弩", "", "职业", 3, 0f, 0f, 0, "", 1, "", 0f, 0f, "", 0, 0f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "Dumb", "", "", 0f, "nu", "atkspeed+0.35", "atkspeed+0.15", "");
            config[2000104] = new SkillConfig(2000104, "弩手", "弩", "", "职业", 4, 0f, 0f, 0, "", 1, "", 0f, 0f, "", 0, 0f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "Dumb", "", "", 0f, "nu", "atkspeed+0.5", "atkspeed+0.2", "");
            config[2000105] = new SkillConfig(2000105, "弩手", "弩", "", "职业", 5, 0f, 0f, 0, "", 1, "", 0f, 0f, "", 0, 0f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "Dumb", "", "", 0f, "nu", "atkspeed+0.7", "atkspeed+0.25", "");
            config[2000111] = new SkillConfig(2000111, "碾压", "车", "自身暴击/linkself-crit%，全队暴击/linkteam-crit%", "职业", 1, 0f, 0f, 0, "", 1, "", 0f, 0f, "", 0, 0f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "Dumb", "", "", 0f, "che", "crit+0.1", "crit+0.03", "");
            config[2000112] = new SkillConfig(2000112, "碾压", "车", "", "职业", 2, 0f, 0f, 0, "", 1, "", 0f, 0f, "", 0, 0f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "Dumb", "", "", 0f, "che", "crit+0.17", "crit+0.05", "");
            config[2000113] = new SkillConfig(2000113, "碾压", "车", "", "职业", 3, 0f, 0f, 0, "", 1, "", 0f, 0f, "", 0, 0f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "Dumb", "", "", 0f, "che", "crit+0.25", "crit+0.07", "");
            config[2000114] = new SkillConfig(2000114, "碾压", "车", "", "职业", 4, 0f, 0f, 0, "", 1, "", 0f, 0f, "", 0, 0f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "Dumb", "", "", 0f, "che", "crit+0.35", "crit+0.10", "");
            config[2000115] = new SkillConfig(2000115, "碾压", "车", "", "职业", 5, 0f, 0f, 0, "", 1, "", 0f, 0f, "", 0, 0f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "Dumb", "", "", 0f, "che", "crit+0.45", "crit+0.13", "");
            config[2000121] = new SkillConfig(2000121, "声乐", "琴", "自身施加的正面buff持续/strength%，全队生命回复/linkteam-hpRegen/秒", "职业", 1, 0f, 0f, 0, "", 1, "", 0f, 0f, "", 0, 0.1f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "ModifyBuffTime", "", "", 0f, "song", "", "hpRegen+2", "");
            config[2000122] = new SkillConfig(2000122, "声乐", "琴", "", "职业", 2, 0f, 0f, 0, "", 1, "", 0f, 0f, "", 0, 0.2f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "ModifyBuffTime", "", "", 0f, "song", "", "hpRegen+4", "");
            config[2000123] = new SkillConfig(2000123, "声乐", "琴", "", "职业", 3, 0f, 0f, 0, "", 1, "", 0f, 0f, "", 0, 0.35f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "ModifyBuffTime", "", "", 0f, "song", "", "hpRegen+6", "");
            config[2000124] = new SkillConfig(2000124, "声乐", "琴", "", "职业", 4, 0f, 0f, 0, "", 1, "", 0f, 0f, "", 0, 0.5f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "ModifyBuffTime", "", "", 0f, "song", "", "hpRegen+8", "");
            config[2000125] = new SkillConfig(2000125, "声乐", "琴", "", "职业", 5, 0f, 0f, 0, "", 1, "", 0f, 0f, "", 0, 0.7f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "ModifyBuffTime", "", "", 0f, "song", "", "hpRegen+10", "");
            config[2000131] = new SkillConfig(2000131, "治疗", "医", "自身治疗/linkself-healRate%，全队生命回复/linkteam-hpRegen/秒", "职业", 1, 0f, 0f, 0, "", 1, "", 0f, 0f, "", 0, 0f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "Dumb", "", "", 0f, "heal", "healRate+0.1", "hpRegen+3", "");
            config[2000132] = new SkillConfig(2000132, "治疗", "医", "", "职业", 2, 0f, 0f, 0, "", 1, "", 0f, 0f, "", 0, 0f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "Dumb", "", "", 0f, "heal", "healRate+0.15", "hpRegen+6", "");
            config[2000133] = new SkillConfig(2000133, "治疗", "医", "", "职业", 3, 0f, 0f, 0, "", 1, "", 0f, 0f, "", 0, 0f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "Dumb", "", "", 0f, "heal", "healRate+0.2", "hpRegen+9", "");
            config[2000134] = new SkillConfig(2000134, "治疗", "医", "", "职业", 4, 0f, 0f, 0, "", 1, "", 0f, 0f, "", 0, 0f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "Dumb", "", "", 0f, "heal", "healRate+0.25", "hpRegen+12", "");
            config[2000135] = new SkillConfig(2000135, "治疗", "医", "", "职业", 5, 0f, 0f, 0, "", 1, "", 0f, 0f, "", 0, 0f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "Dumb", "", "", 0f, "heal", "healRate+0.3", "hpRegen+15", "");
            config[2000141] = new SkillConfig(2000141, "枪阵", "枪", "攻击时/rate几率眩晕目标1.5秒", "职业", 1, 0.1f, 0f, 0, "", 1, "", 0f, 0f, "", 0, 0f, 0f, 0, "乱", false, 1.5f, "", 0, 0f, 0f, 0f, 0, "HitBuff", "", "", 0f, "qiang", "", "", "");
            config[2000142] = new SkillConfig(2000142, "枪阵", "枪", "", "职业", 2, 0.16f, 0f, 0, "", 1, "", 0f, 0f, "", 0, 0f, 0f, 0, "乱", false, 1.5f, "", 0, 0f, 0f, 0f, 0, "HitBuff", "", "", 0f, "qiang", "", "", "");
            config[2000143] = new SkillConfig(2000143, "枪阵", "枪", "", "职业", 3, 0.25f, 0f, 0, "", 1, "", 0f, 0f, "", 0, 0f, 0f, 0, "乱", false, 1.5f, "", 0, 0f, 0f, 0f, 0, "HitBuff", "", "", 0f, "qiang", "", "", "");
            config[2000144] = new SkillConfig(2000144, "枪阵", "枪", "", "职业", 4, 0.35f, 0f, 0, "", 1, "", 0f, 0f, "", 0, 0f, 0f, 0, "乱", false, 1.5f, "", 0, 0f, 0f, 0f, 0, "HitBuff", "", "", 0f, "qiang", "", "", "");
            config[2000145] = new SkillConfig(2000145, "枪阵", "枪", "", "职业", 5, 0.5f, 0f, 0, "", 1, "", 0f, 0f, "", 0, 0f, 0f, 0, "乱", false, 1.5f, "", 0, 0f, 0f, 0f, 0, "HitBuff", "", "", 0f, "qiang", "", "", "");
            config[2000151] = new SkillConfig(2000151, "戟阵", "戟", "攻击时/rate几率对周围造成50%溅射伤害", "职业", 1, 0.1f, 0f, 0, "", 1, "", 0f, 5f, "", 2, 0.5f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "HitArea", "", "SparkleAreaWhite", 0f, "ji", "", "", "");
            config[2000152] = new SkillConfig(2000152, "戟阵", "戟", "", "职业", 2, 0.16f, 0f, 0, "", 1, "", 0f, 5f, "", 2, 0.5f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "HitArea", "", "SparkleAreaWhite", 0f, "ji", "", "", "");
            config[2000153] = new SkillConfig(2000153, "戟阵", "戟", "", "职业", 3, 0.25f, 0f, 0, "", 1, "", 0f, 5f, "", 2, 0.5f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "HitArea", "", "SparkleAreaWhite", 0f, "ji", "", "", "");
            config[2000154] = new SkillConfig(2000154, "戟阵", "戟", "", "职业", 4, 0.35f, 0f, 0, "", 1, "", 0f, 5f, "", 2, 0.5f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "HitArea", "", "SparkleAreaWhite", 0f, "ji", "", "", "");
            config[2000155] = new SkillConfig(2000155, "戟阵", "戟", "", "职业", 5, 0.5f, 0f, 0, "", 1, "", 0f, 5f, "", 2, 0.5f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "HitArea", "", "SparkleAreaWhite", 0f, "ji", "", "", "");
            config[2000161] = new SkillConfig(2000161, "战鼓", "鼓", "自身光环效果/linkself-auroEffectRate%", "职业", 1, 0f, 0f, 0, "", 1, "", 0f, 0f, "", 0, 0f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "Dumb", "", "", 0f, "gu", "auroEffectRate+0.1", "", "");
            config[2000162] = new SkillConfig(2000162, "战鼓", "鼓", "", "职业", 2, 0f, 0f, 0, "", 1, "", 0f, 0f, "", 0, 0f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "Dumb", "", "", 0f, "gu", "auroEffectRate+0.2", "", "");
            config[2000163] = new SkillConfig(2000163, "战鼓", "鼓", "", "职业", 3, 0f, 0f, 0, "", 1, "", 0f, 0f, "", 0, 0f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "Dumb", "", "", 0f, "gu", "auroEffectRate+0.35", "", "");
            config[2000164] = new SkillConfig(2000164, "战鼓", "鼓", "", "职业", 4, 0f, 0f, 0, "", 1, "", 0f, 0f, "", 0, 0f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "Dumb", "", "", 0f, "gu", "auroEffectRate+0.5", "", "");
            config[2000165] = new SkillConfig(2000165, "战鼓", "鼓", "", "职业", 5, 0f, 0f, 0, "", 1, "", 0f, 0f, "", 0, 0f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "Dumb", "", "", 0f, "gu", "auroEffectRate+0.7", "", "");
            config[2000171] = new SkillConfig(2000171, "铁壁", "盾", "自身护甲/linkself-armor，全队护甲/linkteam-armor", "职业", 1, 0f, 0f, 0, "", 1, "", 0f, 0f, "", 0, 0f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "Dumb", "", "", 0f, "dun", "armor+10", "armor+3", "");
            config[2000172] = new SkillConfig(2000172, "铁壁", "盾", "", "职业", 2, 0f, 0f, 0, "", 1, "", 0f, 0f, "", 0, 0f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "Dumb", "", "", 0f, "dun", "armor+20", "armor+6", "");
            config[2000173] = new SkillConfig(2000173, "铁壁", "盾", "", "职业", 3, 0f, 0f, 0, "", 1, "", 0f, 0f, "", 0, 0f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "Dumb", "", "", 0f, "dun", "armor+35", "armor+9", "");
            config[2000174] = new SkillConfig(2000174, "铁壁", "盾", "", "职业", 4, 0f, 0f, 0, "", 1, "", 0f, 0f, "", 0, 0f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "Dumb", "", "", 0f, "dun", "armor+60", "armor+12", "");
            config[2000175] = new SkillConfig(2000175, "铁壁", "盾", "", "职业", 5, 0f, 0f, 0, "", 1, "", 0f, 0f, "", 0, 0f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "Dumb", "", "", 0f, "dun", "armor+90", "armor+15", "");
            config[2000181] = new SkillConfig(2000181, "机巧", "工", "战斗开始时召唤一个木牛流马lv1", "职业", 1, 0f, 0f, 0, "", 1, "", 0f, 0f, "", 0, 0f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "Dumb", "", "", 0f, "gong3", "", "", "");
            config[2000182] = new SkillConfig(2000182, "机巧", "工", "战斗开始时召唤一个木牛流马lv2", "职业", 2, 0f, 0f, 0, "", 1, "", 0f, 0f, "", 0, 0f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "Dumb", "", "", 0f, "gong3", "", "", "");
            config[2000183] = new SkillConfig(2000183, "机巧", "工", "战斗开始时召唤一个木牛流马lv2与一个喷火兽lv1", "职业", 3, 0f, 0f, 0, "", 1, "", 0f, 0f, "", 0, 0f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "Dumb", "", "", 0f, "gong3", "", "", "");
            config[2000184] = new SkillConfig(2000184, "机巧", "工", "战斗开始时召唤一个木牛流马lv2与一个喷火兽lv2", "职业", 4, 0f, 0f, 0, "", 1, "", 0f, 0f, "", 0, 0f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "Dumb", "", "", 0f, "gong3", "", "", "");
            config[2000185] = new SkillConfig(2000185, "机巧", "工", "战斗开始时召唤两个木牛流马lv2与一个喷火兽lv2与一个辅助兽", "职业", 5, 0f, 0f, 0, "", 1, "", 0f, 0f, "", 0, 0f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "Dumb", "", "", 0f, "gong3", "", "", "");
            config[2010001] = new SkillConfig(2010001, "突破", "突", "攻击时穿越敌人，造成/strength额外伤害", "连接", 1, 1f, 5f, 0, "", 1, "", 0f, 0f, "", 0, 50f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AttackRunCross", "", "LightningExplosionBlue", 0f, "tu", "", "", "");
            config[2010002] = new SkillConfig(2010002, "突破", "突", "", "连接", 2, 1f, 5f, 0, "", 1, "", 0f, 0f, "", 0, 100f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AttackRunCross", "", "LightningExplosionBlue", 0f, "tu", "", "", "");
            config[2010003] = new SkillConfig(2010003, "突破", "突", "", "连接", 3, 1f, 5f, 0, "", 1, "", 0f, 0f, "", 0, 150f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AttackRunCross", "", "LightningExplosionBlue", 0f, "tu", "", "", "");
            config[2010004] = new SkillConfig(2010004, "突破", "突", "", "连接", 4, 1f, 5f, 0, "", 1, "", 0f, 0f, "", 0, 200f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AttackRunCross", "", "LightningExplosionBlue", 0f, "tu", "", "", "");
            config[2010005] = new SkillConfig(2010005, "突破", "突", "", "连接", 5, 1f, 5f, 0, "", 1, "", 0f, 0f, "", 0, 250f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AttackRunCross", "", "LightningExplosionBlue", 0f, "tu", "", "", "");
            config[2010006] = new SkillConfig(2010006, "冲锋", "冲", "攻击时穿越敌人，降低目标防御/targetcounts(最多/targetcount目标)", "连接", 1, 1f, 6f, 0, "", 1, "", 12f, 0f, "", 2, 0f, 0f, 0, "伤", false, 2f, "", 0, 0f, 0f, 0f, 0, "AttackRunCrossPlus", "", "LightningExplosionRed", 0f, "chong", "", "", "");
            config[2010007] = new SkillConfig(2010007, "冲锋", "冲", "", "连接", 2, 1f, 6f, 0, "", 1, "", 12f, 0f, "", 3, 0f, 0f, 0, "伤", false, 3f, "", 0, 0f, 0f, 0f, 0, "AttackRunCrossPlus", "", "LightningExplosionRed", 0f, "chong", "", "", "");
            config[2010008] = new SkillConfig(2010008, "冲锋", "冲", "", "连接", 3, 1f, 6f, 0, "", 1, "", 12f, 0f, "", 4, 0f, 0f, 0, "伤", false, 4f, "", 0, 0f, 0f, 0f, 0, "AttackRunCrossPlus", "", "LightningExplosionRed", 0f, "chong", "", "", "");
            config[2010009] = new SkillConfig(2010009, "冲锋", "冲", "", "连接", 4, 1f, 6f, 0, "", 1, "", 12f, 0f, "", 5, 0f, 0f, 0, "伤", false, 5f, "", 0, 0f, 0f, 0f, 0, "AttackRunCrossPlus", "", "LightningExplosionRed", 0f, "chong", "", "", "");
            config[2010010] = new SkillConfig(2010010, "冲锋", "冲", "", "连接", 5, 1f, 6f, 0, "", 1, "", 12f, 0f, "", 6, 0f, 0f, 0, "伤", false, 6f, "", 0, 0f, 0f, 0f, 0, "AttackRunCrossPlus", "", "LightningExplosionRed", 0f, "chong", "", "", "");
            config[2010011] = new SkillConfig(2010011, "连击", "连", "攻击时/rate触发连续攻击", "连接", 1, 0.1f, 4f, 0, "", 1, "", 0f, 0f, "", 0, 0.9f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AttackSpeedAttack", "flipspin", "", 0f, "lian", "", "", "");
            config[2010012] = new SkillConfig(2010012, "连击", "连", "", "连接", 2, 0.2f, 3.5f, 0, "", 1, "", 0f, 0f, "", 0, 0.9f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AttackSpeedAttack", "flipspin", "", 0f, "lian", "", "", "");
            config[2010013] = new SkillConfig(2010013, "连击", "连", "", "连接", 3, .33f, 3f, 0, "", 1, "", 0f, 0f, "", 0, 0.9f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AttackSpeedAttack", "flipspin", "", 0f, "lian", "", "", "");
            config[2010014] = new SkillConfig(2010014, "连击", "连", "", "连接", 4, 0.45f, 2.5f, 0, "", 1, "", 0f, 0f, "", 0, 0.9f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AttackSpeedAttack", "flipspin", "", 0f, "lian", "", "", "");
            config[2010015] = new SkillConfig(2010015, "连击", "连", "", "连接", 5, 0.60f, 2f, 0, "", 1, "", 0f, 0f, "", 0, 0.9f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AttackSpeedAttack", "flipspin", "", 0f, "lian", "", "", "");
            config[2010016] = new SkillConfig(2010016, "火攻", "火", "攻击时/rate几率对目标放火，若目标有火则传递，造成/strength法术伤害", "连接", 1, 0.2f, 4f, 0, "", 0, "", 0f, 30f, "", 1, 30f, 0f, 0, "", false, 0f, "火", 1, 3.2f, 1f, 0f, 0, "HitFireArea", "throw", "SoftFireBigRed", 1.6f, "huo", "", "", "");
            config[2010017] = new SkillConfig(2010017, "火攻", "火", "", "连接", 2, 0.25f, 4f, 0, "", 0, "", 0f, 30f, "", 1, 40f, 0f, 0, "", false, 0f, "火", 1, 3.2f, 1f, 0f, 0, "HitFireArea", "throw", "SoftFireBigRed", 1.6f, "huo", "", "", "");
            config[2010018] = new SkillConfig(2010018, "火攻", "火", "", "连接", 3, 0.3f, 4f, 0, "", 0, "", 0f, 30f, "", 1, 50f, 0f, 0, "", false, 0f, "火", 1, 3.2f, 1f, 0f, 0, "HitFireArea", "throw", "SoftFireBigRed", 1.6f, "huo", "", "", "");
            config[2010019] = new SkillConfig(2010019, "火攻", "火", "", "连接", 4, 0.35f, 4f, 0, "", 0, "", 0f, 30f, "", 1, 60f, 0f, 0, "", false, 0f, "火", 1, 3.2f, 1f, 0f, 0, "HitFireArea", "throw", "SoftFireBigRed", 1.6f, "huo", "", "", "");
            config[2010020] = new SkillConfig(2010020, "火攻", "火", "", "连接", 5, 0.4f, 4f, 0, "", 0, "", 0f, 30f, "", 1, 70f, 0f, 0, "", false, 0f, "火", 1, 3.2f, 1f, 0f, 0, "HitFireArea", "throw", "SoftFireBigRed", 1.6f, "huo", "", "", "");
            config[2010021] = new SkillConfig(2010021, "破盾", "破", "每次攻击，对有护盾的目标/rate概率额外造成/strength%物理穿透伤害", "连接", 1, .2f, 0f, 0, "", 1, "AntiShield", 0f, 0f, "", 0, 0.3f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AttackShieldPierce", "", "SoftFireBigRed", 0f, "po", "", "", "");
            config[2010022] = new SkillConfig(2010022, "破盾", "破", "", "连接", 2, .25f, 0f, 0, "", 1, "AntiShield", 0f, 0f, "", 0, 0.35f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AttackShieldPierce", "", "SoftFireBigRed", 0f, "po", "", "", "");
            config[2010023] = new SkillConfig(2010023, "破盾", "破", "", "连接", 3, .3f, 0f, 0, "", 1, "AntiShield", 0f, 0f, "", 0, 0.4f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AttackShieldPierce", "", "SoftFireBigRed", 0f, "po", "", "", "");
            config[2010024] = new SkillConfig(2010024, "破盾", "破", "", "连接", 4, .4f, 0f, 0, "", 1, "AntiShield", 0f, 0f, "", 0, 0.45f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AttackShieldPierce", "", "SoftFireBigRed", 0f, "po", "", "", "");
            config[2010025] = new SkillConfig(2010025, "破盾", "破", "", "连接", 5, .5f, 0f, 0, "", 1, "AntiShield", 0f, 0f, "", 0, 0.5f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AttackShieldPierce", "", "SoftFireBigRed", 0f, "po", "", "", "");
            config[2010026] = new SkillConfig(2010026, "冷箭", "冷", "能够射出冷箭，造成/strength伤害", "连接", 1, 1f, 4f, 0, "", 1, "", 30f, 0f, "", 0, 80f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AidSuddenArrow", "sway", "BulletExplosionBlue", 3f, "leng", "", "", "");
            config[2010027] = new SkillConfig(2010027, "冷箭", "冷", "", "连接", 2, 1f, 4f, 0, "", 1, "", 40f, 0f, "", 0, 120f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AidSuddenArrow", "sway", "BulletExplosionBlue", 3f, "leng", "", "", "");
            config[2010028] = new SkillConfig(2010028, "冷箭", "冷", "", "连接", 3, 1f, 4f, 0, "", 1, "", 50f, 0f, "", 0, 160f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AidSuddenArrow", "sway", "BulletExplosionBlue", 3f, "leng", "", "", "");
            config[2010029] = new SkillConfig(2010029, "冷箭", "冷", "", "连接", 4, 1f, 4f, 0, "", 1, "", 60f, 0f, "", 0, 200f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AidSuddenArrow", "sway", "BulletExplosionBlue", 3f, "leng", "", "", "");
            config[2010030] = new SkillConfig(2010030, "冷箭", "冷", "", "连接", 5, 1f, 4f, 0, "", 1, "", 80f, 0f, "", 0, 250f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AidSuddenArrow", "sway", "BulletExplosionBlue", 3f, "leng", "", "", "");
            config[2010031] = new SkillConfig(2010031, "穿甲", "穿", "物理伤害无视目标/strength%护甲", "连接", 1, 0f, 0f, 0, "", 1, "", 0f, 0f, "", 0, 0.1f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AttackArmorPierce", "", "", 0f, "chuan", "", "", "");
            config[2010032] = new SkillConfig(2010032, "穿甲", "穿", "", "连接", 2, 0f, 0f, 0, "", 1, "", 0f, 0f, "", 0, 0.2f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AttackArmorPierce", "", "", 0f, "chuan", "", "", "");
            config[2010033] = new SkillConfig(2010033, "穿甲", "穿", "", "连接", 3, 0f, 0f, 0, "", 1, "", 0f, 0f, "", 0, 0.3f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AttackArmorPierce", "", "", 0f, "chuan", "", "", "");
            config[2010034] = new SkillConfig(2010034, "穿甲", "穿", "", "连接", 4, 0f, 0f, 0, "", 1, "", 0f, 0f, "", 0, 0.4f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AttackArmorPierce", "", "", 0f, "chuan", "", "", "");
            config[2010035] = new SkillConfig(2010035, "穿甲", "穿", "", "连接", 5, 0f, 0f, 0, "", 1, "", 0f, 0f, "", 0, 0.5f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AttackArmorPierce", "", "", 0f, "chuan", "", "", "");
            config[2010036] = new SkillConfig(2010036, "无双", "双", "攻击时/rate对目标进行三连击", "连接", 1, 0.10f, 8f, 0, "", 1, "", 0f, 0f, "", 0, 1f, 0f, 2, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "HitRepeat", "jumpspin", "SwordHitRedCritical", 0f, "shuang", "", "", "");
            config[2010037] = new SkillConfig(2010037, "无双", "双", "", "连接", 2, 0.15f, 7f, 0, "", 1, "", 0f, 0f, "", 0, 1f, 0f, 2, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "HitRepeat", "jumpspin", "SwordHitRedCritical", 0f, "shuang", "", "", "");
            config[2010038] = new SkillConfig(2010038, "无双", "双", "", "连接", 3, 0.2f, 6f, 0, "", 1, "", 0f, 0f, "", 0, 1f, 0f, 2, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "HitRepeat", "jumpspin", "SwordHitRedCritical", 0f, "shuang", "", "", "");
            config[2010039] = new SkillConfig(2010039, "无双", "双", "", "连接", 4, 0.25f, 5f, 0, "", 1, "", 0f, 0f, "", 0, 1f, 0f, 2, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "HitRepeat", "jumpspin", "SwordHitRedCritical", 0f, "shuang", "", "", "");
            config[2010040] = new SkillConfig(2010040, "无双", "双", "", "连接", 5, 0.30f, 4f, 0, "", 1, "", 0f, 0f, "", 0, 1f, 0f, 2, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "HitRepeat", "jumpspin", "SwordHitRedCritical", 0f, "shuang", "", "", "");
            config[2010041] = new SkillConfig(2010041, "坚毅", "坚", "生命值低时减免/strength%伤害", "连接", 1, 1f, 0f, 0, "hprate<50", 1, "", 0f, 0f, "", 0, 0.2f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "ReduceDamageRate", "", "", 0f, "jian", "", "", "");
            config[2010042] = new SkillConfig(2010042, "坚毅", "坚", "", "连接", 2, 1f, 0f, 0, "hprate<50", 1, "", 0f, 0f, "", 0, 0.3f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "ReduceDamageRate", "", "", 0f, "jian", "", "", "");
            config[2010043] = new SkillConfig(2010043, "坚毅", "坚", "", "连接", 3, 1f, 0f, 0, "hprate<50", 1, "", 0f, 0f, "", 0, 0.4f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "ReduceDamageRate", "", "", 0f, "jian", "", "", "");
            config[2010044] = new SkillConfig(2010044, "坚毅", "坚", "", "连接", 4, 1f, 0f, 0, "hprate<50", 1, "", 0f, 0f, "", 0, 0.5f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "ReduceDamageRate", "", "", 0f, "jian", "", "", "");
            config[2010045] = new SkillConfig(2010045, "坚毅", "坚", "", "连接", 5, 1f, 0f, 0, "hprate<50", 1, "", 0f, 0f, "", 0, 0.6f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "ReduceDamageRate", "", "", 0f, "jian", "", "", "");
            config[2010046] = new SkillConfig(2010046, "谋略", "谋", "攻击时/rate几率眩晕目标，攻击眩晕目标额外造成50%伤害", "连接", 1, 0.1f, 0f, 0, "", 1, "", 0f, 0f, "", 0, 0.5f, 0f, 0, "乱", false, 1.5f, "", 0, 0f, 0f, 0f, 0, "AttackStunDamage", "", "SoftFireBigRed", 0f, "mou2", "", "", "");
            config[2010047] = new SkillConfig(2010047, "谋略", "谋", "", "连接", 2, 0.16f, 0f, 0, "", 1, "", 0f, 0f, "", 0, 0.5f, 0f, 0, "乱", false, 1.5f, "", 0, 0f, 0f, 0f, 0, "AttackStunDamage", "", "SoftFireBigRed", 0f, "mou2", "", "", "");
            config[2010048] = new SkillConfig(2010048, "谋略", "谋", "", "连接", 3, 0.25f, 0f, 0, "", 1, "", 0f, 0f, "", 0, 0.5f, 0f, 0, "乱", false, 1.5f, "", 0, 0f, 0f, 0f, 0, "AttackStunDamage", "", "SoftFireBigRed", 0f, "mou2", "", "", "");
            config[2010049] = new SkillConfig(2010049, "谋略", "谋", "", "连接", 4, 0.35f, 0f, 0, "", 1, "", 0f, 0f, "", 0, 0.5f, 0f, 0, "乱", false, 1.5f, "", 0, 0f, 0f, 0f, 0, "AttackStunDamage", "", "SoftFireBigRed", 0f, "mou2", "", "", "");
            config[2010050] = new SkillConfig(2010050, "谋略", "谋", "", "连接", 5, 0.5f, 0f, 0, "", 1, "", 0f, 0f, "", 0, 0.5f, 0f, 0, "乱", false, 1.5f, "", 0, 0f, 0f, 0f, 0, "AttackStunDamage", "", "SoftFireBigRed", 0f, "mou2", "", "", "");
            config[2010051] = new SkillConfig(2010051, "明镜", "镜", "提升/linkself-magicres点法术抗性", "连接", 1, 0f, 0f, 0, "", 1, "", 0f, 0f, "", 0, 0f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "InitAttrChange", "", "", 0f, "jing", "magicres+20", "", "");
            config[2010052] = new SkillConfig(2010052, "明镜", "镜", "", "连接", 2, 0f, 0f, 0, "", 1, "", 0f, 0f, "", 0, 0f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "InitAttrChange", "", "", 0f, "jing", "magicres+40", "", "");
            config[2010053] = new SkillConfig(2010053, "明镜", "镜", "", "连接", 3, 0f, 0f, 0, "", 1, "", 0f, 0f, "", 0, 0f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "InitAttrChange", "", "", 0f, "jing", "magicres+60", "", "");
            config[2010054] = new SkillConfig(2010054, "明镜", "镜", "", "连接", 4, 0f, 0f, 0, "", 1, "", 0f, 0f, "", 0, 0f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "InitAttrChange", "", "", 0f, "jing", "magicres+80", "", "");
            config[2010055] = new SkillConfig(2010055, "明镜", "镜", "", "连接", 5, 0f, 0f, 0, "", 1, "", 0f, 0f, "", 0, 0f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "InitAttrChange", "", "", 0f, "jing", "magicres+100", "", "");
            config[2010056] = new SkillConfig(2010056, "背水", "背", "阵亡时回复同组我方英雄/strength%最大生命和攻击，提升/strengthInt法术强度", "连接", 1, 0f, 0f, 0, "", 1, "", 0f, 0f, "", 0, 0.2f, 0f, 5, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "DeathGroupHeal", "", "", 0f, "bei", "", "", "");
            config[2010057] = new SkillConfig(2010057, "背水", "背", "", "连接", 2, 0f, 0f, 0, "", 1, "", 0f, 0f, "", 0, 0.3f, 0f, 10, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "DeathGroupHeal", "", "", 0f, "bei", "", "", "");
            config[2010058] = new SkillConfig(2010058, "背水", "背", "", "连接", 3, 0f, 0f, 0, "", 1, "", 0f, 0f, "", 0, 0.4f, 0f, 15, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "DeathGroupHeal", "", "", 0f, "bei", "", "", "");
            config[2010059] = new SkillConfig(2010059, "背水", "背", "", "连接", 4, 0f, 0f, 0, "", 1, "", 0f, 0f, "", 0, 0.5f, 0f, 20, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "DeathGroupHeal", "", "", 0f, "bei", "", "", "");
            config[2010060] = new SkillConfig(2010060, "背水", "背", "", "连接", 5, 0f, 0f, 0, "", 1, "", 0f, 0f, "", 0, 0.6f, 0f, 25, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "DeathGroupHeal", "", "", 0f, "bei", "", "", "");
            config[2010061] = new SkillConfig(2010061, "护卫", "护", "给生命比例最低的友方英雄减伤盾，护盾值=自身最大生命的/strength%", "连接", 1, 1f, 8f, 0, "", 1, "", 80f, 0f, "", 0, .1f, 0f, 0, "盾", false, 999f, "", 0, 0f, 0f, 0f, 0, "AidBuffLowHp", "sway", "MagicChargeYellow", 0f, "hu", "", "", "");
            config[2010062] = new SkillConfig(2010062, "护卫", "护", "", "连接", 2, 1f, 8f, 0, "", 1, "", 80f, 0f, "", 0, .15f, 0f, 0, "盾", false, 999f, "", 0, 0f, 0f, 0f, 0, "AidBuffLowHp", "sway", "MagicChargeYellow", 0f, "hu", "", "", "");
            config[2010063] = new SkillConfig(2010063, "护卫", "护", "", "连接", 3, 1f, 8f, 0, "", 1, "", 80f, 0f, "", 0, .2f, 0f, 0, "盾", false, 999f, "", 0, 0f, 0f, 0f, 0, "AidBuffLowHp", "sway", "MagicChargeYellow", 0f, "hu", "", "", "");
            config[2010064] = new SkillConfig(2010064, "护卫", "护", "", "连接", 4, 1f, 8f, 0, "", 1, "", 80f, 0f, "", 0, .25f, 0f, 0, "盾", false, 999f, "", 0, 0f, 0f, 0f, 0, "AidBuffLowHp", "sway", "MagicChargeYellow", 0f, "hu", "", "", "");
            config[2010065] = new SkillConfig(2010065, "护卫", "护", "", "连接", 5, 1f, 8f, 0, "", 1, "", 80f, 0f, "", 0, .3f, 0f, 0, "盾", false, 999f, "", 0, 0f, 0f, 0f, 0, "AidBuffLowHp", "sway", "MagicChargeYellow", 0f, "hu", "", "", "");
            config[2010066] = new SkillConfig(2010066, "刺甲", "刺", "反弹/strength%攻击伤害", "连接", 1, 1f, 0f, 0, "", 1, "", 20f, 0f, "", 0, 0.2f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "DefFeedback", "", "SwordHitBlue", 0f, "ci", "", "", "");
            config[2010067] = new SkillConfig(2010067, "刺甲", "刺", "", "连接", 2, 1f, 0f, 0, "", 1, "", 20f, 0f, "", 0, 0.3f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "DefFeedback", "", "SwordHitBlue", 0f, "ci", "", "", "");
            config[2010068] = new SkillConfig(2010068, "刺甲", "刺", "", "连接", 3, 1f, 0f, 0, "", 1, "", 20f, 0f, "", 0, 0.4f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "DefFeedback", "", "SwordHitBlue", 0f, "ci", "", "", "");
            config[2010069] = new SkillConfig(2010069, "刺甲", "刺", "", "连接", 4, 1f, 0f, 0, "", 1, "", 20f, 0f, "", 0, 0.55f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "DefFeedback", "", "SwordHitBlue", 0f, "ci", "", "", "");
            config[2010070] = new SkillConfig(2010070, "刺甲", "刺", "", "连接", 5, 1f, 0f, 0, "", 1, "", 20f, 0f, "", 0, 0.7f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "DefFeedback", "", "SwordHitBlue", 0f, "ci", "", "", "");
            config[2010071] = new SkillConfig(2010071, "偷袭", "偷", "战斗开始突袭随机敌方英雄，造成/strength的魔法伤害，/rate概率交换位置", "连接", 1, 0.3f, 0f, 0, "", 0, "", 0f, 0f, "", 0, 100f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "InitSneakChangePos", "", "MagicNovaBlue", 0f, "tou", "", "", "");
            config[2010072] = new SkillConfig(2010072, "偷袭", "偷", "", "连接", 2, 0.35f, 0f, 0, "", 0, "", 0f, 0f, "", 0, 150f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "InitSneakChangePos", "", "MagicNovaBlue", 0f, "tou", "", "", "");
            config[2010073] = new SkillConfig(2010073, "偷袭", "偷", "", "连接", 3, 0.4f, 0f, 0, "", 0, "", 0f, 0f, "", 0, 200f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "InitSneakChangePos", "", "MagicNovaBlue", 0f, "tou", "", "", "");
            config[2010074] = new SkillConfig(2010074, "偷袭", "偷", "", "连接", 4, 0.45f, 0f, 0, "", 0, "", 0f, 0f, "", 0, 250f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "InitSneakChangePos", "", "MagicNovaBlue", 0f, "tou", "", "", "");
            config[2010075] = new SkillConfig(2010075, "偷袭", "偷", "", "连接", 5, 0.5f, 0f, 0, "", 0, "", 0f, 0f, "", 0, 300f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "InitSneakChangePos", "", "MagicNovaBlue", 0f, "tou", "", "", "");
            config[2010076] = new SkillConfig(2010076, "仁者无敌", "仁", "战斗开始时/rate概率获得1个万民书", "连接", 1, 0.5f, 0f, 0, "", 0, "", 0f, 0f, "", 0, 0f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 401012, "InitAddItem", "", "", 0f, "shu", "", "", "");
            config[2010077] = new SkillConfig(2010077, "仁者无敌", "仁", "", "连接", 2, 0.6f, 0f, 0, "", 0, "", 0f, 0f, "", 0, 0f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 401012, "InitAddItem", "", "", 0f, "shu", "", "", "");
            config[2010078] = new SkillConfig(2010078, "仁者无敌", "仁", "", "连接", 3, 0.7f, 0f, 0, "", 0, "", 0f, 0f, "", 0, 0f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 401012, "InitAddItem", "", "", 0f, "shu", "", "", "");
            config[2010079] = new SkillConfig(2010079, "仁者无敌", "仁", "", "连接", 4, 0.8f, 0f, 0, "", 0, "", 0f, 0f, "", 0, 0f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 401012, "InitAddItem", "", "", 0f, "shu", "", "", "");
            config[2010080] = new SkillConfig(2010080, "仁者无敌", "仁", "", "连接", 5, 0.9f, 0f, 0, "", 0, "", 0f, 0f, "", 0, 0f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 401012, "InitAddItem", "", "", 0f, "shu", "", "", "");
            config[2010081] = new SkillConfig(2010081, "风华", "华", "攻击目标时造成/strength额外魔法伤害", "连接", 1, 1f, 0f, 0, "", 0, "", 0f, 0f, "", 0, 0.2f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AttackMagicDamage", "", "", 0f, "hua", "", "", "");
            config[2010082] = new SkillConfig(2010082, "风华", "华", "", "连接", 2, 1f, 0f, 0, "", 0, "", 0f, 0f, "", 0, 0.3f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AttackMagicDamage", "", "", 0f, "hua", "", "", "");
            config[2010083] = new SkillConfig(2010083, "风华", "华", "", "连接", 3, 1f, 0f, 0, "", 0, "", 0f, 0f, "", 0, 0.4f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AttackMagicDamage", "", "", 0f, "hua", "", "", "");
            config[2010084] = new SkillConfig(2010084, "风华", "华", "", "连接", 4, 1f, 0f, 0, "", 0, "", 0f, 0f, "", 0, 0.45f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AttackMagicDamage", "", "", 0f, "hua", "", "", "");
            config[2010085] = new SkillConfig(2010085, "风华", "华", "", "连接", 5, 1f, 0f, 0, "", 0, "", 0f, 0f, "", 0, 0.5f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AttackMagicDamage", "", "", 0f, "hua", "", "", "");
            config[2010091] = new SkillConfig(2010091, "生财", "济", "每回合额外获得/strengthint金币", "连接", 1, 0f, 0f, 0, "", 0, "", 0f, 0f, "", 0, 0f, 0f, 2, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "Dumb", "", "", 0f, "ji2", "", "", "");
            config[2010092] = new SkillConfig(2010092, "生财", "济", "", "连接", 2, 0f, 0f, 0, "", 0, "", 0f, 0f, "", 0, 0f, 0f, 3, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "Dumb", "", "", 0f, "ji2", "", "", "");
            config[2010093] = new SkillConfig(2010093, "生财", "济", "", "连接", 3, 0f, 0f, 0, "", 0, "", 0f, 0f, "", 0, 0f, 0f, 4, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "Dumb", "", "", 0f, "ji2", "", "", "");
            config[2010094] = new SkillConfig(2010094, "生财", "济", "", "连接", 4, 0f, 0f, 0, "", 0, "", 0f, 0f, "", 0, 0f, 0f, 5, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "Dumb", "", "", 0f, "ji2", "", "", "");
            config[2010095] = new SkillConfig(2010095, "生财", "济", "", "连接", 5, 0f, 0f, 0, "", 0, "", 0f, 0f, "", 0, 0f, 0f, 6, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "Dumb", "", "", 0f, "ji2", "", "", "");
            config[2010096] = new SkillConfig(2010096, "弄权跋扈", "奸", "战斗开始时对随机1名敌方英雄施加增伤，其受到的伤害/strength%，持续/bufftime", "连接", 1, 0f, 0f, 0, "", 0, "", 0f, 0f, "", 0, 0.30f, 0f, 0, "伤", false, 10f, "", 0, 0f, 0f, 0f, 0, "InitEnemyRandomBuff", "", "", 0f, "jian2", "", "", "");
            config[2010097] = new SkillConfig(2010097, "弄权跋扈", "奸", "", "连接", 2, 0f, 0f, 0, "", 0, "", 0f, 0f, "", 0, 0.40f, 0f, 0, "伤", false, 12f, "", 0, 0f, 0f, 0f, 0, "InitEnemyRandomBuff", "", "", 0f, "jian2", "", "", "");
            config[2010098] = new SkillConfig(2010098, "弄权跋扈", "奸", "", "连接", 3, 0f, 0f, 0, "", 0, "", 0f, 0f, "", 0, 0.50f, 0f, 0, "伤", false, 15f, "", 0, 0f, 0f, 0f, 0, "InitEnemyRandomBuff", "", "", 0f, "jian2", "", "", "");
            config[2010099] = new SkillConfig(2010099, "弄权跋扈", "奸", "", "连接", 4, 0f, 0f, 0, "", 0, "", 0f, 0f, "", 0, 0.60f, 0f, 0, "伤", false, 18f, "", 0, 0f, 0f, 0f, 0, "InitEnemyRandomBuff", "", "", 0f, "jian2", "", "", "");
            config[2010100] = new SkillConfig(2010100, "弄权跋扈", "奸", "", "连接", 5, 0f, 0f, 0, "", 0, "", 0f, 0f, "", 0, 0.70f, 0f, 0, "伤", false, 20f, "", 0, 0f, 0f, 0f, 0, "InitEnemyRandomBuff", "", "", 0f, "jian2", "", "", "");
            config[2010106] = new SkillConfig(2010106, "宝刀未老", "老", "生命低于30%受击时给自己释放攻击/strength的护盾，冷却10秒，每次触发永久提升生命回复+/strengthint", "连接", 1, 1f, 15f, 0, "hprate<30", 0, "", 0f, 0f, "", 0, 2.4f, 0f, 2, "盾", false, 999f, "", 0, 0f, 0f, 0f, 0, "AttackedShield", "sway", "MagicChargeYellow", 0f, "lao", "", "", "");
            config[2010107] = new SkillConfig(2010107, "宝刀未老", "老", "", "连接", 2, 1f, 15f, 0, "hprate<30", 0, "", 0f, 0f, "", 0, 3f, 0f, 3, "盾", false, 999f, "", 0, 0f, 0f, 0f, 0, "AttackedShield", "sway", "MagicChargeYellow", 0f, "lao", "", "", "");
            config[2010108] = new SkillConfig(2010108, "宝刀未老", "老", "", "连接", 3, 1f, 15f, 0, "hprate<30", 0, "", 0f, 0f, "", 0, 3.6f, 0f, 4, "盾", false, 999f, "", 0, 0f, 0f, 0f, 0, "AttackedShield", "sway", "MagicChargeYellow", 0f, "lao", "", "", "");
            config[2010109] = new SkillConfig(2010109, "宝刀未老", "老", "", "连接", 4, 1f, 15f, 0, "hprate<30", 0, "", 0f, 0f, "", 0, 4.2f, 0f, 5, "盾", false, 999f, "", 0, 0f, 0f, 0f, 0, "AttackedShield", "sway", "MagicChargeYellow", 0f, "lao", "", "", "");
            config[2010110] = new SkillConfig(2010110, "宝刀未老", "老", "", "连接", 5, 1f, 15f, 0, "hprate<30", 0, "", 0f, 0f, "", 0, 4.8f, 0f, 6, "盾", false, 999f, "", 0, 0f, 0f, 0f, 0, "AttackedShield", "sway", "MagicChargeYellow", 0f, "lao", "", "", "");
            config[2010111] = new SkillConfig(2010111, "诗书传家", "文", "战斗开始时以/rate概率获得道具「文赋」（上一局战败时概率+50%；概率超过100%时必定获得1本，超出部分还有机会再获得1本）", "连接", 1, 0.4f, 0f, 0, "", 0, "", 0f, 0f, "", 0, 0f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 401014, "InitAddItemChance", "", "", 0f, "wen", "", "", "");
            config[2010112] = new SkillConfig(2010112, "诗书传家", "文", "", "连接", 2, 0.48f, 0f, 0, "", 0, "", 0f, 0f, "", 0, 0f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 401014, "InitAddItemChance", "", "", 0f, "wen", "", "", "");
            config[2010113] = new SkillConfig(2010113, "诗书传家", "文", "", "连接", 3, 0.56f, 0f, 0, "", 0, "", 0f, 0f, "", 0, 0f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 401014, "InitAddItemChance", "", "", 0f, "wen", "", "", "");
            config[2010114] = new SkillConfig(2010114, "诗书传家", "文", "", "连接", 4, 0.64f, 0f, 0, "", 0, "", 0f, 0f, "", 0, 0f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 401014, "InitAddItemChance", "", "", 0f, "wen", "", "", "");
            config[2010115] = new SkillConfig(2010115, "诗书传家", "文", "", "连接", 5, 0.72f, 0f, 0, "", 0, "", 0f, 0f, "", 0, 0f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 401014, "InitAddItemChance", "", "", 0f, "wen", "", "", "");
            config[2010116] = new SkillConfig(2010116, "异禀", "异", "身负异禀，不假外物：战斗开始时，若自身未装备任何道具，则攻击或法强提升/strength点、护甲魔抗提升/strength2点", "连接", 1, 0f, 0f, 0, "", 1, "", 0f, 0f, "", 0, 10f, 8f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "InitNoEquipAttr", "", "", 0f, "yi", "", "", "");
            config[2010117] = new SkillConfig(2010117, "异禀", "异", "", "连接", 2, 0f, 0f, 0, "", 1, "", 0f, 0f, "", 0, 20f, 16f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "InitNoEquipAttr", "", "", 0f, "yi", "", "", "");
            config[2010118] = new SkillConfig(2010118, "异禀", "异", "", "连接", 3, 0f, 0f, 0, "", 1, "", 0f, 0f, "", 0, 35f, 28f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "InitNoEquipAttr", "", "", 0f, "yi", "", "", "");
            config[2010119] = new SkillConfig(2010119, "异禀", "异", "", "连接", 4, 0f, 0f, 0, "", 1, "", 0f, 0f, "", 0, 55f, 44f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "InitNoEquipAttr", "", "", 0f, "yi", "", "", "");
            config[2010120] = new SkillConfig(2010120, "异禀", "异", "", "连接", 5, 0f, 0f, 0, "", 1, "", 0f, 0f, "", 0, 80f, 60f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "InitNoEquipAttr", "", "", 0f, "yi", "", "", "");
            config[2010121] = new SkillConfig(2010121, "军律", "律", "治军严明，军阵如铁：全组护甲提升/linkself-armor", "连接", 1, 0f, 0f, 0, "", 1, "", 0f, 0f, "", 0, 0f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "InitAttrChange", "", "", 0f, "lv", "armor+12", "", "");
            config[2010122] = new SkillConfig(2010122, "军律", "律", "", "连接", 2, 0f, 0f, 0, "", 1, "", 0f, 0f, "", 0, 0f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "InitAttrChange", "", "", 0f, "lv", "armor+24", "", "");
            config[2010123] = new SkillConfig(2010123, "军律", "律", "", "连接", 3, 0f, 0f, 0, "", 1, "", 0f, 0f, "", 0, 0f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "InitAttrChange", "", "", 0f, "lv", "armor+40", "", "");
            config[2010124] = new SkillConfig(2010124, "军律", "律", "", "连接", 4, 0f, 0f, 0, "", 1, "", 0f, 0f, "", 0, 0f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "InitAttrChange", "", "", 0f, "lv", "armor+60", "", "");
            config[2010125] = new SkillConfig(2010125, "军律", "律", "", "连接", 5, 0f, 0f, 0, "", 1, "", 0f, 0f, "", 0, 0f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "InitAttrChange", "", "", 0f, "lv", "armor+85", "", "");
            config[2010126] = new SkillConfig(2010126, "运筹", "佐", "王佐之才，算无遗策：战斗开始立即获得/strength%技能法力消耗的法力，抢占先机", "连接", 1, 0f, 0f, 0, "", 1, "", 0f, 0f, "", 0, 0.30f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "InitMpFill", "", "MagicChargeBlue", 0f, "zuo", "", "", "");
            config[2010127] = new SkillConfig(2010127, "运筹", "佐", "", "连接", 2, 0f, 0f, 0, "", 1, "", 0f, 0f, "", 0, 0.50f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "InitMpFill", "", "MagicChargeBlue", 0f, "zuo", "", "", "");
            config[2010128] = new SkillConfig(2010128, "运筹", "佐", "", "连接", 3, 0f, 0f, 0, "", 1, "", 0f, 0f, "", 0, 0.70f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "InitMpFill", "", "MagicChargeBlue", 0f, "zuo", "", "", "");
            config[2010129] = new SkillConfig(2010129, "运筹", "佐", "", "连接", 4, 0f, 0f, 0, "", 1, "", 0f, 0f, "", 0, 0.90f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "InitMpFill", "", "MagicChargeBlue", 0f, "zuo", "", "", "");
            config[2010130] = new SkillConfig(2010130, "运筹", "佐", "", "连接", 5, 0f, 0f, 0, "", 1, "", 0f, 0f, "", 0, 1.20f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "InitMpFill", "", "MagicChargeBlue", 0f, "zuo", "", "", "");
            config[2010131] = new SkillConfig(2010131, "门阀", "族", "名门望族，底蕴深厚：战斗开始获得/strength点护甲与/strength2点魔抗，每3秒衰减1/5，15秒后归零", "连接", 1, 0f, 0f, 0, "", 1, "", 0f, 0f, "", 0, 30f, 30f, 0, "望", false, 15f, "", 0, 0f, 0f, 0f, 0, "InitDecayDef", "", "MagicChargeYellow", 0f, "zu2", "", "", "");
            config[2010132] = new SkillConfig(2010132, "门阀", "族", "", "连接", 2, 0f, 0f, 0, "", 1, "", 0f, 0f, "", 0, 50f, 50f, 0, "望", false, 15f, "", 0, 0f, 0f, 0f, 0, "InitDecayDef", "", "MagicChargeYellow", 0f, "zu2", "", "", "");
            config[2010133] = new SkillConfig(2010133, "门阀", "族", "", "连接", 3, 0f, 0f, 0, "", 1, "", 0f, 0f, "", 0, 75f, 75f, 0, "望", false, 15f, "", 0, 0f, 0f, 0f, 0, "InitDecayDef", "", "MagicChargeYellow", 0f, "zu2", "", "", "");
            config[2010134] = new SkillConfig(2010134, "门阀", "族", "", "连接", 4, 0f, 0f, 0, "", 1, "", 0f, 0f, "", 0, 110f, 110f, 0, "望", false, 15f, "", 0, 0f, 0f, 0f, 0, "InitDecayDef", "", "MagicChargeYellow", 0f, "zu2", "", "", "");
            config[2010135] = new SkillConfig(2010135, "门阀", "族", "", "连接", 5, 0f, 0f, 0, "", 1, "", 0f, 0f, "", 0, 150f, 150f, 0, "望", false, 15f, "", 0, 0f, 0f, 0f, 0, "InitDecayDef", "", "MagicChargeYellow", 0f, "zu2", "", "", "");
            config[2010136] = new SkillConfig(2010136, "兼资", "兼", "出将入相，攻守愈强：攻击时叠加一层「兼资」，持续/bufftime秒；每层提升自身/strength攻击与/strength2法术强度，连续攻击不断刷新层数，层数越高加成越大", "连接", 1, 0f, 0f, 0, "", 1, "", 0f, 0f, "", 0, 3f, 2f, 0, "兼", false, 4f, "", 0, 0f, 0f, 0f, 0, "AttackStackBuffer", "", "", 0f, "jian4", "", "", "");
            config[2010137] = new SkillConfig(2010137, "兼资", "兼", "", "连接", 2, 0f, 0f, 0, "", 1, "", 0f, 0f, "", 0, 5f, 4f, 0, "兼", false, 4f, "", 0, 0f, 0f, 0f, 0, "AttackStackBuffer", "", "", 0f, "jian4", "", "", "");
            config[2010138] = new SkillConfig(2010138, "兼资", "兼", "", "连接", 3, 0f, 0f, 0, "", 1, "", 0f, 0f, "", 0, 8f, 6f, 0, "兼", false, 4f, "", 0, 0f, 0f, 0f, 0, "AttackStackBuffer", "", "", 0f, "jian4", "", "", "");
            config[2010139] = new SkillConfig(2010139, "兼资", "兼", "", "连接", 4, 0f, 0f, 0, "", 1, "", 0f, 0f, "", 0, 12f, 9f, 0, "兼", false, 4f, "", 0, 0f, 0f, 0f, 0, "AttackStackBuffer", "", "", 0f, "jian4", "", "", "");
            config[2010140] = new SkillConfig(2010140, "兼资", "兼", "", "连接", 5, 0f, 0f, 0, "", 1, "", 0f, 0f, "", 0, 18f, 12f, 0, "兼", false, 4f, "", 0, 0f, 0f, 0f, 0, "AttackStackBuffer", "", "", 0f, "jian4", "", "", "");
            config[2010141] = new SkillConfig(2010141, "奇才", "才", "身负奇才，天纵之资：周期为范围内最多/targetcount名友军提升攻击/strength、护甲魔抗/strength2，持续/bufftime秒", "连接", 1, 1f, 6f, 0, "", 1, "", 80f, 0f, "", 5, 8f, 6f, 0, "名", false, 6f, "", 0, 0f, 0f, 0f, 0, "AidBuffArea", "sway", "MagicChargeYellow", 0f, "cai", "", "", "");
            config[2010142] = new SkillConfig(2010142, "奇才", "才", "", "连接", 2, 1f, 6f, 0, "", 1, "", 80f, 0f, "", 5, 14f, 10f, 0, "名", false, 6f, "", 0, 0f, 0f, 0f, 0, "AidBuffArea", "sway", "MagicChargeYellow", 0f, "cai", "", "", "");
            config[2010143] = new SkillConfig(2010143, "奇才", "才", "", "连接", 3, 1f, 6f, 0, "", 1, "", 80f, 0f, "", 5, 22f, 16f, 0, "名", false, 6f, "", 0, 0f, 0f, 0f, 0, "AidBuffArea", "sway", "MagicChargeYellow", 0f, "cai", "", "", "");
            config[2010144] = new SkillConfig(2010144, "奇才", "才", "", "连接", 4, 1f, 6f, 0, "", 1, "", 80f, 0f, "", 5, 34f, 24f, 0, "名", false, 6f, "", 0, 0f, 0f, 0f, 0, "AidBuffArea", "sway", "MagicChargeYellow", 0f, "cai", "", "", "");
            config[2010145] = new SkillConfig(2010145, "奇才", "才", "", "连接", 5, 1f, 6f, 0, "", 1, "", 80f, 0f, "", 5, 50f, 36f, 0, "名", false, 6f, "", 0, 0f, 0f, 0f, 0, "AidBuffArea", "sway", "MagicChargeYellow", 0f, "cai", "", "", "");
            config[2010146] = new SkillConfig(2010146, "温良", "俭", "温良恭俭，谦和守身：每6秒为自己套/strength%最大生命护盾", "连接", 1, 1f, 6f, 0, "", 1, "", 0f, 0f, "", 0, 0.10f, 0f, 0, "盾", false, 999f, "", 0, 0f, 0f, 0f, 0, "AidSelfShield", "sway", "MagicChargeYellow", 0f, "jian3", "", "", "");
            config[2010147] = new SkillConfig(2010147, "温良", "俭", "", "连接", 2, 1f, 6f, 0, "", 1, "", 0f, 0f, "", 0, 0.15f, 0f, 0, "盾", false, 999f, "", 0, 0f, 0f, 0f, 0, "AidSelfShield", "sway", "MagicChargeYellow", 0f, "jian3", "", "", "");
            config[2010148] = new SkillConfig(2010148, "温良", "俭", "", "连接", 3, 1f, 6f, 0, "", 1, "", 0f, 0f, "", 0, 0.20f, 0f, 0, "盾", false, 999f, "", 0, 0f, 0f, 0f, 0, "AidSelfShield", "sway", "MagicChargeYellow", 0f, "jian3", "", "", "");
            config[2010149] = new SkillConfig(2010149, "温良", "俭", "", "连接", 4, 1f, 6f, 0, "", 1, "", 0f, 0f, "", 0, 0.25f, 0f, 0, "盾", false, 999f, "", 0, 0f, 0f, 0f, 0, "AidSelfShield", "sway", "MagicChargeYellow", 0f, "jian3", "", "", "");
            config[2010150] = new SkillConfig(2010150, "温良", "俭", "", "连接", 5, 1f, 6f, 0, "", 1, "", 0f, 0f, "", 0, 0.30f, 0f, 0, "盾", false, 999f, "", 0, 0f, 0f, 0f, 0, "AidSelfShield", "sway", "MagicChargeYellow", 0f, "jian3", "", "", "");
            config[2010151] = new SkillConfig(2010151, "儒雅", "儒", "儒将风范，以静制动：攻击时/rate概率回复自身/strength%技能法力消耗", "连接", 1, 0.15f, 0f, 0, "", 1, "", 0f, 0f, "", 0, 0.2f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AttackMpGain", "", "MagicChargeBlue", 0f, "ru", "", "", "");
            config[2010152] = new SkillConfig(2010152, "儒雅", "儒", "", "连接", 2, 0.20f, 0f, 0, "", 1, "", 0f, 0f, "", 0, 0.3f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AttackMpGain", "", "MagicChargeBlue", 0f, "ru", "", "", "");
            config[2010153] = new SkillConfig(2010153, "儒雅", "儒", "", "连接", 3, 0.25f, 0f, 0, "", 1, "", 0f, 0f, "", 0, 0.4f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AttackMpGain", "", "MagicChargeBlue", 0f, "ru", "", "", "");
            config[2010154] = new SkillConfig(2010154, "儒雅", "儒", "", "连接", 4, 0.30f, 0f, 0, "", 1, "", 0f, 0f, "", 0, 0.5f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AttackMpGain", "", "MagicChargeBlue", 0f, "ru", "", "", "");
            config[2010155] = new SkillConfig(2010155, "儒雅", "儒", "", "连接", 5, 0.40f, 0f, 0, "", 1, "", 0f, 0f, "", 0, 0.6f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AttackMpGain", "", "MagicChargeBlue", 0f, "ru", "", "", "");
            config[2010156] = new SkillConfig(2010156, "枭乱", "枭", "乱世枭雄，越战越强：每击杀一个敌方单位，永久提升自身/strength攻击，并回复/strength2%最大生命", "连接", 1, 0f, 0f, 0, "", 1, "", 0f, 0f, "", 0, 8f, 0.03f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "KillGain", "", "", 0f, "xiao", "", "", "");
            config[2010157] = new SkillConfig(2010157, "枭乱", "枭", "", "连接", 2, 0f, 0f, 0, "", 1, "", 0f, 0f, "", 0, 14f, 0.05f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "KillGain", "", "", 0f, "xiao", "", "", "");
            config[2010158] = new SkillConfig(2010158, "枭乱", "枭", "", "连接", 3, 0f, 0f, 0, "", 1, "", 0f, 0f, "", 0, 22f, 0.08f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "KillGain", "", "", 0f, "xiao", "", "", "");
            config[2010159] = new SkillConfig(2010159, "枭乱", "枭", "", "连接", 4, 0f, 0f, 0, "", 1, "", 0f, 0f, "", 0, 32f, 0.12f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "KillGain", "", "", 0f, "xiao", "", "", "");
            config[2010160] = new SkillConfig(2010160, "枭乱", "枭", "", "连接", 5, 0f, 0f, 0, "", 1, "", 0f, 0f, "", 0, 45f, 0.16f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "KillGain", "", "", 0f, "xiao", "", "", "");
            config[2010161] = new SkillConfig(2010161, "安民", "民", "济世安民，抚恤军士：战斗开始全组士兵最大生命提升/strength%、每秒生命回复提升/strength2点", "连接", 1, 0f, 0f, 0, "", 1, "", 0f, 0f, "", 0, 0.05f, 2f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "InitSoldierBuff", "", "MagicChargeYellow", 0f, "min", "", "", "");
            config[2010162] = new SkillConfig(2010162, "安民", "民", "", "连接", 2, 0f, 0f, 0, "", 1, "", 0f, 0f, "", 0, 0.10f, 4f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "InitSoldierBuff", "", "MagicChargeYellow", 0f, "min", "", "", "");
            config[2010163] = new SkillConfig(2010163, "安民", "民", "", "连接", 3, 0f, 0f, 0, "", 1, "", 0f, 0f, "", 0, 0.15f, 6f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "InitSoldierBuff", "", "MagicChargeYellow", 0f, "min", "", "", "");
            config[2010164] = new SkillConfig(2010164, "安民", "民", "", "连接", 4, 0f, 0f, 0, "", 1, "", 0f, 0f, "", 0, 0.20f, 8f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "InitSoldierBuff", "", "MagicChargeYellow", 0f, "min", "", "", "");
            config[2010165] = new SkillConfig(2010165, "安民", "民", "", "连接", 5, 0f, 0f, 0, "", 1, "", 0f, 0f, "", 0, 0.25f, 10f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "InitSoldierBuff", "", "MagicChargeYellow", 0f, "min", "", "", "");
            config[2020001] = new SkillConfig(2020001, "强击", "强", "对目标造成法强/strength的魔法伤害", "技", 1, 1f, 1f, 12, "", 0, "", 30f, 0f, "", 1, 60f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AidSuddenArrow", "sway", "BulletExplosionBlue", 3f, "", "", "", "");
            config[2020002] = new SkillConfig(2020002, "强击", "强", "", "技", 2, 1f, 1f, 12, "", 0, "", 30f, 0f, "", 1, 120f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AidSuddenArrow", "sway", "BulletExplosionBlue", 3f, "", "", "", "");
            config[2020003] = new SkillConfig(2020003, "强击", "强", "", "技", 3, 1f, 1f, 12, "", 0, "", 30f, 0f, "", 1, 180f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AidSuddenArrow", "sway", "BulletExplosionBlue", 3f, "", "", "", "");
            config[2020004] = new SkillConfig(2020004, "强击", "强", "", "技", 4, 1f, 1f, 12, "", 0, "", 30f, 0f, "", 1, 240f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AidSuddenArrow", "sway", "BulletExplosionBlue", 3f, "", "", "", "");
            config[2020005] = new SkillConfig(2020005, "强击", "强", "", "技", 5, 1f, 1f, 12, "", 0, "", 30f, 0f, "", 1, 300f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AidSuddenArrow", "sway", "BulletExplosionBlue", 3f, "", "", "", "");
            config[2020141] = new SkillConfig(2020141, "飞斧", "许褚", "扔出飞斧攻击前方敌人，造成/strength伤害", "技", 1, 1f, 5f, 15, "", 0, "", 45f, 9f, "", 4, 50f, 0f, 0, "", false, 0f, "武", 0, 1.5f, 0f, 40f, 0, "AidShockWave", "spin", "AxeExplosion", 3f, "", "", "", "");
            config[2020142] = new SkillConfig(2020142, "飞斧", "许褚", "", "技", 2, 1f, 5f, 15, "", 0, "", 45f, 9f, "", 4, 80f, 0f, 0, "", false, 0f, "武", 0, 1.5f, 0f, 40f, 0, "AidShockWave", "spin", "AxeExplosion", 3f, "", "", "", "");
            config[2020143] = new SkillConfig(2020143, "飞斧", "许褚", "", "技", 3, 1f, 5f, 15, "", 0, "", 45f, 9f, "", 4, 110f, 0f, 0, "", false, 0f, "武", 0, 1.5f, 0f, 40f, 0, "AidShockWave", "spin", "AxeExplosion", 3f, "", "", "", "");
            config[2020144] = new SkillConfig(2020144, "飞斧", "许褚", "", "技", 4, 1f, 5f, 15, "", 0, "", 45f, 9f, "", 4, 160f, 0f, 0, "", false, 0f, "武", 0, 1.5f, 0f, 40f, 0, "AidShockWave", "spin", "AxeExplosion", 3f, "", "", "", "");
            config[2020145] = new SkillConfig(2020145, "飞斧", "许褚", "", "技", 5, 1f, 5f, 15, "", 0, "", 45f, 9f, "", 4, 220f, 0f, 0, "", false, 0f, "武", 0, 1.5f, 0f, 40f, 0, "AidShockWave", "spin", "AxeExplosion", 3f, "", "", "", "");
            config[2020161] = new SkillConfig(2020161, "惊雷", "诸葛亮", "召唤3个惊雷攻击前方敌人，造成/strength法术伤害", "术", 1, 1f, 5f, 18, "", 0, "", 60f, 11f, "", 4, 100f, 0f, 0, "", false, 0f, "雷", 0, 1.5f, 0f, 40f, 0, "AidShockWave", "spin", "NukeMissileFires", 4f, "", "", "", "");
            config[2020162] = new SkillConfig(2020162, "惊雷", "诸葛亮", "", "术", 2, 1f, 5f, 18, "", 0, "", 60f, 11f, "", 4, 140f, 0f, 0, "", false, 0f, "雷", 0, 1.5f, 0f, 40f, 0, "AidShockWave", "spin", "NukeMissileFires", 4f, "", "", "", "");
            config[2020163] = new SkillConfig(2020163, "惊雷", "诸葛亮", "", "术", 3, 1f, 5f, 18, "", 0, "", 60f, 11f, "", 4, 180f, 0f, 0, "", false, 0f, "雷", 0, 1.5f, 0f, 40f, 0, "AidShockWave", "spin", "NukeMissileFires", 4f, "", "", "", "");
            config[2020164] = new SkillConfig(2020164, "惊雷", "诸葛亮", "", "术", 4, 1f, 5f, 18, "", 0, "", 60f, 11f, "", 4, 230f, 0f, 0, "", false, 0f, "雷", 0, 1.5f, 0f, 40f, 0, "AidShockWave", "spin", "NukeMissileFires", 4f, "", "", "", "");
            config[2020165] = new SkillConfig(2020165, "惊雷", "诸葛亮", "", "术", 5, 1f, 5f, 18, "", 0, "", 60f, 11f, "", 4, 300f, 0f, 0, "", false, 0f, "雷", 0, 1.5f, 0f, 40f, 0, "AidShockWave", "spin", "NukeMissileFires", 4f, "", "", "", "");
            config[2020191] = new SkillConfig(2020191, "魔神", "吕布", "对目标造成伤害并吸取造成/strength伤害，并回复等量生命", "", 1, 0f, 4f, 20, "", 0, "", 10f, 0f, "", 1, 100f, 0f, 0, "", false, 0f, "", 0, 0.6f, 0f, 15f, 0, "AidDrain", "", "MagicBuffGreen", 3f, "", "", "", "");
            config[2020192] = new SkillConfig(2020192, "魔神", "吕布", "", "", 2, 0f, 4f, 20, "", 0, "", 10f, 0f, "", 1, 180f, 0f, 0, "", false, 0f, "", 0, 0.6f, 0f, 15f, 0, "AidDrain", "", "MagicBuffGreen", 3f, "", "", "", "");
            config[2020193] = new SkillConfig(2020193, "魔神", "吕布", "", "", 3, 0f, 4f, 20, "", 0, "", 10f, 0f, "", 1, 250f, 0f, 0, "", false, 0f, "", 0, 0.6f, 0f, 15f, 0, "AidDrain", "", "MagicBuffGreen", 3f, "", "", "", "");
            config[2020194] = new SkillConfig(2020194, "魔神", "吕布", "", "", 4, 0f, 4f, 20, "", 0, "", 10f, 0f, "", 1, 350f, 0f, 0, "", false, 0f, "", 0, 0.6f, 0f, 15f, 0, "AidDrain", "", "MagicBuffGreen", 3f, "", "", "", "");
            config[2020195] = new SkillConfig(2020195, "魔神", "吕布", "", "", 5, 0f, 4f, 20, "", 0, "", 10f, 0f, "", 1, 500f, 0f, 0, "", false, 0f, "", 0, 0.6f, 0f, 15f, 0, "AidDrain", "", "MagicBuffGreen", 3f, "", "", "", "");
            config[2020201] = new SkillConfig(2020201, "埋伏", "吕蒙", "被攻击时，瞬移近身造成/strength法术伤害并眩晕，同时获得/strength2法强的护盾", "", 1, 0f, 4f, 14, "", 0, "", 40f, 0f, "", 0, 50f, 2f, 0, "乱", false, 1f, "", 0, 0f, 0f, 0f, 0, "AttackedTeleport", "saw", "MagicNovaBlue", 0f, "", "", "", "");
            config[2020202] = new SkillConfig(2020202, "埋伏", "吕蒙", "", "", 2, 0f, 4f, 14, "", 0, "", 45f, 0f, "", 0, 75f, 2.5f, 0, "乱", false, 1f, "", 0, 0f, 0f, 0f, 0, "AttackedTeleport", "saw", "MagicNovaBlue", 0f, "", "", "", "");
            config[2020203] = new SkillConfig(2020203, "埋伏", "吕蒙", "", "", 3, 0f, 4f, 14, "", 0, "", 50f, 0f, "", 0, 100f, 3f, 0, "乱", false, 1f, "", 0, 0f, 0f, 0f, 0, "AttackedTeleport", "saw", "MagicNovaBlue", 0f, "", "", "", "");
            config[2020204] = new SkillConfig(2020204, "埋伏", "吕蒙", "", "", 4, 0f, 4f, 14, "", 0, "", 60f, 0f, "", 0, 140f, 3.5f, 0, "乱", false, 1f, "", 0, 0f, 0f, 0f, 0, "AttackedTeleport", "saw", "MagicNovaBlue", 0f, "", "", "", "");
            config[2020205] = new SkillConfig(2020205, "埋伏", "吕蒙", "", "", 5, 0f, 4f, 14, "", 0, "", 80f, 0f, "", 0, 200f, 4f, 0, "乱", false, 1f, "", 0, 0f, 0f, 0f, 0, "AttackedTeleport", "saw", "MagicNovaBlue", 0f, "", "", "", "");
            config[2020206] = new SkillConfig(2020206, "火墙", "周瑜", "攻击召唤出持续伤害的火墙，造成每秒/strength伤害", "术", 1, 0f, 0f, 20, "", 0, "", 0f, 8f, "", 1, 25f, 0f, 0, "", false, 0f, "火", 5, 3.2f, 1f, 0f, 0, "HitWall", "spin", "SoftFireBigRed", 1.6f, "", "", "", "");
            config[2020207] = new SkillConfig(2020207, "火墙", "周瑜", "", "术", 2, 0f, 0f, 20, "", 0, "", 0f, 8f, "", 1, 40f, 0f, 0, "", false, 0f, "火", 5, 3.2f, 1f, 0f, 0, "HitWall", "spin", "SoftFireBigRed", 1.6f, "", "", "", "");
            config[2020208] = new SkillConfig(2020208, "火墙", "周瑜", "", "术", 3, 0f, 0f, 20, "", 0, "", 0f, 8f, "", 1, 60f, 0f, 0, "", false, 0f, "火", 5, 3.2f, 1f, 0f, 0, "HitWall", "spin", "SoftFireBigRed", 1.6f, "", "", "", "");
            config[2020209] = new SkillConfig(2020209, "火墙", "周瑜", "", "术", 4, 0f, 0f, 20, "", 0, "", 0f, 8f, "", 1, 80f, 0f, 0, "", false, 0f, "火", 5, 3.2f, 1f, 0f, 0, "HitWall", "spin", "SoftFireBigRed", 1.6f, "", "", "", "");
            config[2020210] = new SkillConfig(2020210, "火墙", "周瑜", "", "术", 5, 0f, 0f, 20, "", 0, "", 0f, 8f, "", 1, 110f, 0f, 0, "", false, 0f, "火", 5, 3.2f, 1f, 0f, 0, "HitWall", "spin", "SoftFireBigRed", 1.6f, "", "", "", "");
            config[2020211] = new SkillConfig(2020211, "火矢", "火矢", "攻击时射出火箭，造成每秒/strength伤害", "技", 1, 0f, 0f, 12, "", 0, "", 0f, 8f, "", 1, 40f, 0f, 0, "", false, 0f, "火", 1, 3.2f, 1f, 0f, 0, "HitWall", "throw", "SoftFireBigRed", 1.6f, "", "", "", "");
            config[2020212] = new SkillConfig(2020212, "火矢", "火矢", "", "技", 2, 0f, 0f, 12, "", 0, "", 0f, 8f, "", 1, 60f, 0f, 0, "", false, 0f, "火", 1, 3.2f, 1f, 0f, 0, "HitWall", "throw", "SoftFireBigRed", 1.6f, "", "", "", "");
            config[2020213] = new SkillConfig(2020213, "火矢", "火矢", "", "技", 3, 0f, 0f, 12, "", 0, "", 0f, 8f, "", 1, 80f, 0f, 0, "", false, 0f, "火", 1, 3.2f, 1f, 0f, 0, "HitWall", "throw", "SoftFireBigRed", 1.6f, "", "", "", "");
            config[2020214] = new SkillConfig(2020214, "火矢", "火矢", "", "技", 4, 0f, 0f, 12, "", 0, "", 0f, 8f, "", 1, 100f, 0f, 0, "", false, 0f, "火", 1, 3.2f, 1f, 0f, 0, "HitWall", "throw", "SoftFireBigRed", 1.6f, "", "", "", "");
            config[2020215] = new SkillConfig(2020215, "火矢", "火矢", "", "技", 5, 0f, 0f, 12, "", 0, "", 0f, 8f, "", 1, 120f, 0f, 0, "", false, 0f, "火", 1, 3.2f, 1f, 0f, 0, "HitWall", "throw", "SoftFireBigRed", 1.6f, "", "", "", "");
            config[2020216] = new SkillConfig(2020216, "共杀", "共杀", "攻击时对目标周围/targetcount个敌人弹射/strength%攻击伤害", "技", 1, 0f, 3f, 10, "", 0, "", 30f, 0f, "", 2, .3f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AttackReboundArrow", "flipspin", "", 0f, "", "", "", "");
            config[2020217] = new SkillConfig(2020217, "共杀", "共杀", "", "技", 2, 0f, 3f, 10, "", 0, "", 30f, 0f, "", 3, .35f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AttackReboundArrow", "flipspin", "", 0f, "", "", "", "");
            config[2020218] = new SkillConfig(2020218, "共杀", "共杀", "", "技", 3, 0f, 3f, 10, "", 0, "", 30f, 0f, "", 3, .4f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AttackReboundArrow", "flipspin", "", 0f, "", "", "", "");
            config[2020219] = new SkillConfig(2020219, "共杀", "共杀", "", "技", 4, 0f, 3f, 10, "", 0, "", 30f, 0f, "", 4, .5f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AttackReboundArrow", "flipspin", "", 0f, "", "", "", "");
            config[2020220] = new SkillConfig(2020220, "共杀", "共杀", "", "技", 5, 0f, 3f, 10, "", 0, "", 30f, 0f, "", 4, .6f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AttackReboundArrow", "flipspin", "", 0f, "", "", "", "");
            config[2020221] = new SkillConfig(2020221, "旋风斩", "张飞", "攻击时对周围最多/targetcount个敌人造成/strength%攻击伤害", "技", 1, 0f, 5f, 15, "", 0, "", 25f, 0f, "", 3, .5f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AttackSpinAttack", "spin", "SwordWhirlwindWhite", 0f, "", "", "", "");
            config[2020222] = new SkillConfig(2020222, "旋风斩", "张飞", "", "技", 2, 0f, 5f, 15, "", 0, "", 25f, 0f, "", 3, .6f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AttackSpinAttack", "spin", "SwordWhirlwindWhite", 0f, "", "", "", "");
            config[2020223] = new SkillConfig(2020223, "旋风斩", "张飞", "", "技", 3, 0f, 5f, 15, "", 0, "", 25f, 0f, "", 4, .7f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AttackSpinAttack", "spin", "SwordWhirlwindWhite", 0f, "", "", "", "");
            config[2020224] = new SkillConfig(2020224, "旋风斩", "张飞", "", "技", 4, 0f, 5f, 15, "", 0, "", 25f, 0f, "", 4, .8f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AttackSpinAttack", "spin", "SwordWhirlwindWhite", 0f, "", "", "", "");
            config[2020225] = new SkillConfig(2020225, "旋风斩", "张飞", "", "技", 5, 0f, 5f, 15, "", 0, "", 25f, 0f, "", 4, 1f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AttackSpinAttack", "spin", "SwordWhirlwindWhite", 0f, "", "", "", "");
            config[2020226] = new SkillConfig(2020226, "落雷", "张角", "攻击召唤持续伤害雷电阵，每秒造成/strength法术伤害", "术", 1, 0f, 8f, 18, "", 0, "", 0f, 25f, "", 4, 25f, 0f, 0, "", false, 0f, "雷", 0, 5.2f, 1f, 0f, 0, "HitRegion", "spin", "SummonStorm", 5f, "", "", "", "");
            config[2020227] = new SkillConfig(2020227, "落雷", "张角", "", "术", 2, 0f, 8f, 18, "", 0, "", 0f, 25f, "", 4, 40f, 0f, 0, "", false, 0f, "雷", 0, 5.2f, 1f, 0f, 0, "HitRegion", "spin", "SummonStorm", 5f, "", "", "", "");
            config[2020228] = new SkillConfig(2020228, "落雷", "张角", "", "术", 3, 0f, 8f, 18, "", 0, "", 0f, 25f, "", 4, 55f, 0f, 0, "", false, 0f, "雷", 0, 5.2f, 1f, 0f, 0, "HitRegion", "spin", "SummonStorm", 5f, "", "", "", "");
            config[2020229] = new SkillConfig(2020229, "落雷", "张角", "", "术", 4, 0f, 8f, 18, "", 0, "", 0f, 25f, "", 4, 70f, 0f, 0, "", false, 0f, "雷", 0, 5.2f, 1f, 0f, 0, "HitRegion", "spin", "SummonStorm", 5f, "", "", "", "");
            config[2020230] = new SkillConfig(2020230, "落雷", "张角", "", "术", 5, 0f, 8f, 18, "", 0, "", 0f, 25f, "", 4, 90f, 0f, 0, "", false, 0f, "雷", 0, 5.2f, 1f, 0f, 0, "HitRegion", "spin", "SummonStorm", 5f, "", "", "", "");
            config[2020231] = new SkillConfig(2020231, "安乐", "刘禅", "治疗生命最低的友军/strength点生命，溢出的治疗量转化为护盾，持续/bufftime秒", "技", 1, 0f, 6f, 10, "", 0, "", 60f, 0f, "", 0, 20f, 0f, 0, "盾", false, 6f, "", 0, 0f, 0f, 0f, 0, "AidHealShield", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2020232] = new SkillConfig(2020232, "安乐", "刘禅", "", "技", 2, 0f, 6f, 10, "", 0, "", 60f, 0f, "", 0, 35f, 0f, 0, "盾", false, 6f, "", 0, 0f, 0f, 0f, 0, "AidHealShield", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2020233] = new SkillConfig(2020233, "安乐", "刘禅", "", "技", 3, 0f, 6f, 10, "", 0, "", 60f, 0f, "", 0, 50f, 0f, 0, "盾", false, 6f, "", 0, 0f, 0f, 0f, 0, "AidHealShield", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2020234] = new SkillConfig(2020234, "安乐", "刘禅", "", "技", 4, 0f, 6f, 10, "", 0, "", 60f, 0f, "", 0, 70f, 0f, 0, "盾", false, 6f, "", 0, 0f, 0f, 0f, 0, "AidHealShield", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2020235] = new SkillConfig(2020235, "安乐", "刘禅", "", "技", 5, 0f, 6f, 10, "", 0, "", 60f, 0f, "", 0, 95f, 0f, 0, "盾", false, 6f, "", 0, 0f, 0f, 0f, 0, "AidHealShield", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2020236] = new SkillConfig(2020236, "青囊", "华佗", "治疗生命最低的友军及其/area范围内的友军各/strength点生命", "技", 1, 0f, 8f, 25, "", 0, "", 60f, 25f, "", 0, 15f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AidRangeHeal", "sway", "ShadowExplosionGreen", 5f, "", "", "", "");
            config[2020237] = new SkillConfig(2020237, "青囊", "华佗", "", "技", 2, 0f, 8f, 25, "", 0, "", 60f, 25f, "", 0, 25f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AidRangeHeal", "sway", "ShadowExplosionGreen", 5f, "", "", "", "");
            config[2020238] = new SkillConfig(2020238, "青囊", "华佗", "", "技", 3, 0f, 8f, 25, "", 0, "", 60f, 25f, "", 0, 35f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AidRangeHeal", "sway", "ShadowExplosionGreen", 5f, "", "", "", "");
            config[2020239] = new SkillConfig(2020239, "青囊", "华佗", "", "技", 4, 0f, 8f, 25, "", 0, "", 60f, 25f, "", 0, 50f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AidRangeHeal", "sway", "ShadowExplosionGreen", 5f, "", "", "", "");
            config[2020240] = new SkillConfig(2020240, "青囊", "华佗", "", "技", 5, 0f, 8f, 25, "", 0, "", 60f, 25f, "", 0, 70f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AidRangeHeal", "sway", "ShadowExplosionGreen", 5f, "", "", "", "");
            config[2020241] = new SkillConfig(2020241, "妖咒", "于吉", "对目标挂溃败，每秒造成/strength法术伤害，持续/bufftime秒；同时每秒按治疗量的/strengthint%回复附近生命最低的友军英雄", "术", 1, 0f, 8f, 20, "", 0, "", 40f, 0f, "", 0, 30f, 0f, 50, "败", false, 4f, "", 0, 0f, 0f, 0f, 0, "AidCurseHeal", "sway", "AuraSoftPurple", 0f, "", "", "", "");
            config[2020242] = new SkillConfig(2020242, "妖咒", "于吉", "", "术", 2, 0f, 8f, 20, "", 0, "", 40f, 0f, "", 0, 45f, 0f, 50, "败", false, 4f, "", 0, 0f, 0f, 0f, 0, "AidCurseHeal", "sway", "AuraSoftPurple", 0f, "", "", "", "");
            config[2020243] = new SkillConfig(2020243, "妖咒", "于吉", "", "术", 3, 0f, 8f, 20, "", 0, "", 40f, 0f, "", 0, 60f, 0f, 50, "败", false, 4f, "", 0, 0f, 0f, 0f, 0, "AidCurseHeal", "sway", "AuraSoftPurple", 0f, "", "", "", "");
            config[2020244] = new SkillConfig(2020244, "妖咒", "于吉", "", "术", 4, 0f, 8f, 20, "", 0, "", 40f, 0f, "", 0, 80f, 0f, 50, "败", false, 4f, "", 0, 0f, 0f, 0f, 0, "AidCurseHeal", "sway", "AuraSoftPurple", 0f, "", "", "", "");
            config[2020245] = new SkillConfig(2020245, "妖咒", "于吉", "", "术", 5, 0f, 8f, 20, "", 0, "", 40f, 0f, "", 0, 110f, 0f, 50, "败", false, 4f, "", 0, 0f, 0f, 0f, 0, "AidCurseHeal", "sway", "AuraSoftPurple", 0f, "", "", "", "");
            config[2020246] = new SkillConfig(2020246, "妖疗", "张宝", "跳跃治疗/targetcount名友军，首跳回复/strength点生命，每跳递减", "技", 1, 0f, 6f, 15, "", 0, "", 60f, 25f, "", 3, 25f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AidJumpHeal", "sway", "MagicBuffGreen", 0f, "", "", "", "");
            config[2020247] = new SkillConfig(2020247, "妖疗", "张宝", "", "技", 2, 0f, 6f, 15, "", 0, "", 60f, 25f, "", 3, 40f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AidJumpHeal", "sway", "MagicBuffGreen", 0f, "", "", "", "");
            config[2020248] = new SkillConfig(2020248, "妖疗", "张宝", "", "技", 3, 0f, 6f, 15, "", 0, "", 60f, 25f, "", 3, 60f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AidJumpHeal", "sway", "MagicBuffGreen", 0f, "", "", "", "");
            config[2020249] = new SkillConfig(2020249, "妖疗", "张宝", "", "技", 4, 0f, 6f, 15, "", 0, "", 60f, 25f, "", 3, 85f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AidJumpHeal", "sway", "MagicBuffGreen", 0f, "", "", "", "");
            config[2020250] = new SkillConfig(2020250, "妖疗", "张宝", "", "技", 5, 0f, 6f, 15, "", 0, "", 60f, 25f, "", 3, 120f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AidJumpHeal", "sway", "MagicBuffGreen", 0f, "", "", "", "");
            config[2020251] = new SkillConfig(2020251, "天书", "左慈", "在自身周围展开治疗领域，持续/summontime秒，每秒为我方英雄回复/strength点生命", "技", 1, 0f, 10f, 25, "", 0, "", 0f, 25f, "", 0, 12f, 0f, 0, "", false, 0f, "疗", 0, 4f, 1f, 0f, 0, "AidAreaHeal", "sway", "MagicFieldGreen", 6f, "", "", "", "");
            config[2020252] = new SkillConfig(2020252, "天书", "左慈", "", "技", 2, 0f, 10f, 25, "", 0, "", 0f, 25f, "", 0, 18f, 0f, 0, "", false, 0f, "疗", 0, 4f, 1f, 0f, 0, "AidAreaHeal", "sway", "MagicFieldGreen", 6f, "", "", "", "");
            config[2020253] = new SkillConfig(2020253, "天书", "左慈", "", "技", 3, 0f, 10f, 25, "", 0, "", 0f, 25f, "", 0, 25f, 0f, 0, "", false, 0f, "疗", 0, 4f, 1f, 0f, 0, "AidAreaHeal", "sway", "MagicFieldGreen", 6f, "", "", "", "");
            config[2020254] = new SkillConfig(2020254, "天书", "左慈", "", "技", 4, 0f, 10f, 25, "", 0, "", 0f, 25f, "", 0, 34f, 0f, 0, "", false, 0f, "疗", 0, 4f, 1f, 0f, 0, "AidAreaHeal", "sway", "MagicFieldGreen", 6f, "", "", "", "");
            config[2020255] = new SkillConfig(2020255, "天书", "左慈", "", "技", 5, 0f, 10f, 25, "", 0, "", 0f, 25f, "", 0, 45f, 0f, 0, "", false, 0f, "疗", 0, 4f, 1f, 0f, 0, "AidAreaHeal", "sway", "MagicFieldGreen", 6f, "", "", "", "");
            config[2020266] = new SkillConfig(2020266, "洛神", "甄宓", "为我方生命最低的英雄提升/strength点攻击力，持续/bufftime秒", "技", 1, 0f, 5f, 12, "", 1, "", 80f, 0f, "", 0, 30f, 0f, 0, "攻", false, 5f, "", 0, 0f, 0f, 0f, 0, "AidBuffAtk", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2020267] = new SkillConfig(2020267, "洛神", "甄宓", "", "技", 2, 0f, 5f, 12, "", 1, "", 80f, 0f, "", 0, 50f, 0f, 0, "攻", false, 5f, "", 0, 0f, 0f, 0f, 0, "AidBuffAtk", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2020268] = new SkillConfig(2020268, "洛神", "甄宓", "", "技", 3, 0f, 5f, 12, "", 1, "", 80f, 0f, "", 0, 75f, 0f, 0, "攻", false, 5f, "", 0, 0f, 0f, 0f, 0, "AidBuffAtk", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2020269] = new SkillConfig(2020269, "洛神", "甄宓", "", "技", 4, 0f, 5f, 12, "", 1, "", 80f, 0f, "", 0, 105f, 0f, 0, "攻", false, 5f, "", 0, 0f, 0f, 0f, 0, "AidBuffAtk", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2020270] = new SkillConfig(2020270, "洛神", "甄宓", "", "技", 5, 0f, 5f, 12, "", 1, "", 80f, 0f, "", 0, 140f, 0f, 0, "攻", false, 5f, "", 0, 0f, 0f, 0f, 0, "AidBuffAtk", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2020271] = new SkillConfig(2020271, "胡笳", "蔡文姬", "在自身周围展开治疗领域，持续/summontime秒，每秒为我方英雄回复/strength点生命", "技", 1, 0f, 9f, 18, "", 0, "", 0f, 25f, "", 0, 10f, 0f, 0, "", false, 0f, "疗", 0, 4f, 1f, 0f, 0, "AidAreaHeal", "sway", "MagicFieldGreen", 6f, "", "", "", "");
            config[2020272] = new SkillConfig(2020272, "胡笳", "蔡文姬", "", "技", 2, 0f, 9f, 18, "", 0, "", 0f, 25f, "", 0, 14f, 0f, 0, "", false, 0f, "疗", 0, 4f, 1f, 0f, 0, "AidAreaHeal", "sway", "MagicFieldGreen", 6f, "", "", "", "");
            config[2020273] = new SkillConfig(2020273, "胡笳", "蔡文姬", "", "技", 3, 0f, 9f, 18, "", 0, "", 0f, 25f, "", 0, 19f, 0f, 0, "", false, 0f, "疗", 0, 4f, 1f, 0f, 0, "AidAreaHeal", "sway", "MagicFieldGreen", 6f, "", "", "", "");
            config[2020274] = new SkillConfig(2020274, "胡笳", "蔡文姬", "", "技", 4, 0f, 9f, 18, "", 0, "", 0f, 25f, "", 0, 25f, 0f, 0, "", false, 0f, "疗", 0, 4f, 1f, 0f, 0, "AidAreaHeal", "sway", "MagicFieldGreen", 6f, "", "", "", "");
            config[2020275] = new SkillConfig(2020275, "胡笳", "蔡文姬", "", "技", 5, 0f, 9f, 18, "", 0, "", 0f, 25f, "", 0, 32f, 0f, 0, "", false, 0f, "疗", 0, 4f, 1f, 0f, 0, "AidAreaHeal", "sway", "MagicFieldGreen", 6f, "", "", "", "");
            config[2020276] = new SkillConfig(2020276, "鼓舞", "小乔", "鼓舞我军，使范围内最多/targetcount名友军攻速提升/strength%、移速提升/strength2%，持续/bufftime秒", "技", 1, 0f, 6f, 15, "", 1, "", 150f, 0f, "", 3, 0.10f, 0.10f, 0, "翼", false, 4f, "", 0, 0f, 0f, 0f, 0, "AidBuffHasteMoveSpeed", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2020277] = new SkillConfig(2020277, "鼓舞", "小乔", "", "技", 2, 0f, 6f, 15, "", 1, "", 150f, 0f, "", 3, 0.15f, 0.15f, 0, "翼", false, 4f, "", 0, 0f, 0f, 0f, 0, "AidBuffHasteMoveSpeed", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2020278] = new SkillConfig(2020278, "鼓舞", "小乔", "", "技", 3, 0f, 6f, 15, "", 1, "", 150f, 0f, "", 3, 0.20f, 0.20f, 0, "翼", false, 5f, "", 0, 0f, 0f, 0f, 0, "AidBuffHasteMoveSpeed", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2020279] = new SkillConfig(2020279, "鼓舞", "小乔", "", "技", 4, 0f, 6f, 15, "", 1, "", 150f, 0f, "", 4, 0.26f, 0.26f, 0, "翼", false, 5f, "", 0, 0f, 0f, 0f, 0, "AidBuffHasteMoveSpeed", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2020280] = new SkillConfig(2020280, "鼓舞", "小乔", "", "技", 5, 0f, 6f, 15, "", 1, "", 150f, 0f, "", 4, 0.32f, 0.32f, 0, "翼", false, 6f, "", 0, 0f, 0f, 0f, 0, "AidBuffHasteMoveSpeed", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2020281] = new SkillConfig(2020281, "离间", "貂蝉", "为我方生命最低的英雄附加/strength%吸血与/strength2%攻速，持续/bufftime秒", "技", 1, 0f, 6f, 18, "", 1, "", 80f, 0f, "", 0, 0.15f, 0.15f, 0, "汲", false, 6f, "", 0, 0f, 0f, 0f, 0, "AidBuffHasteSteal", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2020282] = new SkillConfig(2020282, "离间", "貂蝉", "", "技", 2, 0f, 6f, 18, "", 1, "", 80f, 0f, "", 0, 0.20f, 0.20f, 0, "汲", false, 6f, "", 0, 0f, 0f, 0f, 0, "AidBuffHasteSteal", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2020283] = new SkillConfig(2020283, "离间", "貂蝉", "", "技", 3, 0f, 6f, 18, "", 1, "", 80f, 0f, "", 0, 0.25f, 0.26f, 0, "汲", false, 7f, "", 0, 0f, 0f, 0f, 0, "AidBuffHasteSteal", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2020284] = new SkillConfig(2020284, "离间", "貂蝉", "", "技", 4, 0f, 6f, 18, "", 1, "", 80f, 0f, "", 0, 0.30f, 0.32f, 0, "汲", false, 7f, "", 0, 0f, 0f, 0f, 0, "AidBuffHasteSteal", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2020285] = new SkillConfig(2020285, "离间", "貂蝉", "", "技", 5, 0f, 6f, 18, "", 1, "", 80f, 0f, "", 0, 0.35f, 0.40f, 0, "汲", false, 8f, "", 0, 0f, 0f, 0f, 0, "AidBuffHasteSteal", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2020286] = new SkillConfig(2020286, "国色", "大乔", "鼓舞友军，使范围内最多/targetcount名友军攻击力提升/strength点，持续/bufftime秒", "技", 1, 0f, 8f, 20, "", 1, "", 150f, 0f, "", 3, 25f, 0f, 0, "攻", false, 5f, "", 0, 0f, 0f, 0f, 0, "AidBuffArea", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2020287] = new SkillConfig(2020287, "国色", "大乔", "", "技", 2, 0f, 8f, 20, "", 1, "", 150f, 0f, "", 3, 35f, 0f, 0, "攻", false, 5f, "", 0, 0f, 0f, 0f, 0, "AidBuffArea", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2020288] = new SkillConfig(2020288, "国色", "大乔", "", "技", 3, 0f, 8f, 20, "", 1, "", 150f, 0f, "", 4, 50f, 0f, 0, "攻", false, 6f, "", 0, 0f, 0f, 0f, 0, "AidBuffArea", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2020289] = new SkillConfig(2020289, "国色", "大乔", "", "技", 4, 0f, 8f, 20, "", 1, "", 150f, 0f, "", 4, 70f, 0f, 0, "攻", false, 6f, "", 0, 0f, 0f, 0f, 0, "AidBuffArea", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2020290] = new SkillConfig(2020290, "国色", "大乔", "", "技", 5, 0f, 8f, 20, "", 1, "", 150f, 0f, "", 5, 95f, 0f, 0, "攻", false, 7f, "", 0, 0f, 0f, 0f, 0, "AidBuffArea", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2020301] = new SkillConfig(2020301, "忠勇", "周仓", "给自己和附近生命比例最低的友军各加一个/strength%最大生命的护盾", "技", 1, 1f, 8f, 20, "", 0, "", 60f, 0f, "", 0, 0.15f, 0f, 0, "盾", false, 999f, "", 0, 0f, 0f, 0f, 0, "AidSelfAndLowHp", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2020302] = new SkillConfig(2020302, "忠勇", "周仓", "", "技", 2, 1f, 8f, 20, "", 0, "", 60f, 0f, "", 0, 0.20f, 0f, 0, "盾", false, 999f, "", 0, 0f, 0f, 0f, 0, "AidSelfAndLowHp", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2020303] = new SkillConfig(2020303, "忠勇", "周仓", "", "技", 3, 1f, 8f, 20, "", 0, "", 60f, 0f, "", 0, 0.25f, 0f, 0, "盾", false, 999f, "", 0, 0f, 0f, 0f, 0, "AidSelfAndLowHp", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2020304] = new SkillConfig(2020304, "忠勇", "周仓", "", "技", 4, 1f, 8f, 20, "", 0, "", 60f, 0f, "", 0, 0.30f, 0f, 0, "盾", false, 999f, "", 0, 0f, 0f, 0f, 0, "AidSelfAndLowHp", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2020305] = new SkillConfig(2020305, "忠勇", "周仓", "", "技", 5, 1f, 8f, 20, "", 0, "", 60f, 0f, "", 0, 0.35f, 0f, 0, "盾", false, 999f, "", 0, 0f, 0f, 0f, 0, "AidSelfAndLowHp", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2020311] = new SkillConfig(2020311, "疑城", "徐盛", "开场获得/strength%减伤盾，持续15秒", "技", 1, 1f, 0f, 20, "", 0, "", 0f, 0f, "", 0, 0.25f, 0f, 0, "硬", false, 15f, "", 0, 0f, 0f, 0f, 0, "InitShieldValue", "", "ShieldSoftBlue", 0f, "", "", "", "");
            config[2020312] = new SkillConfig(2020312, "疑城", "徐盛", "", "技", 2, 1f, 0f, 20, "", 0, "", 0f, 0f, "", 0, 0.30f, 0f, 0, "硬", false, 15f, "", 0, 0f, 0f, 0f, 0, "InitShieldValue", "", "ShieldSoftBlue", 0f, "", "", "", "");
            config[2020313] = new SkillConfig(2020313, "疑城", "徐盛", "", "技", 3, 1f, 0f, 20, "", 0, "", 0f, 0f, "", 0, 0.35f, 0f, 0, "硬", false, 15f, "", 0, 0f, 0f, 0f, 0, "InitShieldValue", "", "ShieldSoftBlue", 0f, "", "", "", "");
            config[2020314] = new SkillConfig(2020314, "疑城", "徐盛", "", "技", 4, 1f, 0f, 20, "", 0, "", 0f, 0f, "", 0, 0.40f, 0f, 0, "硬", false, 15f, "", 0, 0f, 0f, 0f, 0, "InitShieldValue", "", "ShieldSoftBlue", 0f, "", "", "", "");
            config[2020315] = new SkillConfig(2020315, "疑城", "徐盛", "", "技", 5, 1f, 0f, 20, "", 0, "", 0f, 0f, "", 0, 0.45f, 0f, 0, "硬", false, 15f, "", 0, 0f, 0f, 0f, 0, "InitShieldValue", "", "ShieldSoftBlue", 0f, "", "", "", "");
            config[2020321] = new SkillConfig(2020321, "老当益壮", "严颜", "生命低于50%受击时获得攻击/strength的护盾，每次触发永久+生命回复+/strengthint", "技", 1, 1f, 10f, 20, "hprate<50", 0, "", 0f, 0f, "", 0, 2.4f, 0f, 2, "盾", false, 999f, "", 0, 0f, 0f, 0f, 0, "AttackedShield", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2020322] = new SkillConfig(2020322, "老当益壮", "严颜", "", "技", 2, 1f, 10f, 20, "hprate<50", 0, "", 0f, 0f, "", 0, 3f, 0f, 3, "盾", false, 999f, "", 0, 0f, 0f, 0f, 0, "AttackedShield", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2020323] = new SkillConfig(2020323, "老当益壮", "严颜", "", "技", 3, 1f, 10f, 20, "hprate<50", 0, "", 0f, 0f, "", 0, 3.6f, 0f, 4, "盾", false, 999f, "", 0, 0f, 0f, 0f, 0, "AttackedShield", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2020324] = new SkillConfig(2020324, "老当益壮", "严颜", "", "技", 4, 1f, 10f, 20, "hprate<50", 0, "", 0f, 0f, "", 0, 4.2f, 0f, 5, "盾", false, 999f, "", 0, 0f, 0f, 0f, 0, "AttackedShield", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2020325] = new SkillConfig(2020325, "老当益壮", "严颜", "", "技", 5, 1f, 10f, 20, "hprate<50", 0, "", 0f, 0f, "", 0, 4.8f, 0f, 6, "盾", false, 999f, "", 0, 0f, 0f, 0f, 0, "AttackedShield", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2020331] = new SkillConfig(2020331, "天人守城", "曹仁", "自身无盾时套一个/strength%最大生命的护盾，盾破时对周围/area范围内敌人造成/strength2护盾值爆炸伤害", "技", 1, 1f, 6f, 20, "", 0, "", 0f, 30f, "", 0, 0.20f, 0.3f, 0, "盾", false, 999f, "", 0, 0f, 0f, 0f, 0, "AidSelfShieldBoom", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2020332] = new SkillConfig(2020332, "天人守城", "曹仁", "", "技", 2, 1f, 6f, 20, "", 0, "", 0f, 35f, "", 0, 0.25f, 0.35f, 0, "盾", false, 999f, "", 0, 0f, 0f, 0f, 0, "AidSelfShieldBoom", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2020333] = new SkillConfig(2020333, "天人守城", "曹仁", "", "技", 3, 1f, 6f, 20, "", 0, "", 0f, 40f, "", 0, 0.30f, 0.4f, 0, "盾", false, 999f, "", 0, 0f, 0f, 0f, 0, "AidSelfShieldBoom", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2020334] = new SkillConfig(2020334, "天人守城", "曹仁", "", "技", 4, 1f, 6f, 20, "", 0, "", 0f, 45f, "", 0, 0.35f, 0.45f, 0, "盾", false, 999f, "", 0, 0f, 0f, 0f, 0, "AidSelfShieldBoom", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2020335] = new SkillConfig(2020335, "天人守城", "曹仁", "", "技", 5, 1f, 6f, 20, "", 0, "", 0f, 50f, "", 0, 0.40f, 0.5f, 0, "盾", false, 999f, "", 0, 0f, 0f, 0f, 0, "AidSelfShieldBoom", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2020341] = new SkillConfig(2020341, "偷渡阴平", "邓艾", "生命低于30%受击时获得每秒/strength%最大生命+/strengthint的回血，持续/bufftime秒，并获得/strength2最大生命的护盾，冷却极长", "技", 1, 1f, 120f, 20, "hprate<30", 0, "", 0f, 0f, "", 0, 0.08f, 0.3f, 20, "盾", false, 8f, "", 0, 0f, 0f, 0f, 0, "LowHpFullHealShield", "saw", "MagicNovaBlue", 0f, "", "", "", "");
            config[2020342] = new SkillConfig(2020342, "偷渡阴平", "邓艾", "", "技", 2, 1f, 120f, 20, "hprate<30", 0, "", 0f, 0f, "", 0, 0.10f, 0.35f, 30, "盾", false, 9f, "", 0, 0f, 0f, 0f, 0, "LowHpFullHealShield", "saw", "MagicNovaBlue", 0f, "", "", "", "");
            config[2020343] = new SkillConfig(2020343, "偷渡阴平", "邓艾", "", "技", 3, 1f, 120f, 20, "hprate<30", 0, "", 0f, 0f, "", 0, 0.12f, 0.4f, 40, "盾", false, 10f, "", 0, 0f, 0f, 0f, 0, "LowHpFullHealShield", "saw", "MagicNovaBlue", 0f, "", "", "", "");
            config[2020344] = new SkillConfig(2020344, "偷渡阴平", "邓艾", "", "技", 4, 1f, 120f, 20, "hprate<30", 0, "", 0f, 0f, "", 0, 0.14f, 0.45f, 50, "盾", false, 11f, "", 0, 0f, 0f, 0f, 0, "LowHpFullHealShield", "saw", "MagicNovaBlue", 0f, "", "", "", "");
            config[2020345] = new SkillConfig(2020345, "偷渡阴平", "邓艾", "", "技", 5, 1f, 120f, 20, "hprate<30", 0, "", 0f, 0f, "", 0, 0.16f, 0.5f, 60, "盾", false, 12f, "", 0, 0f, 0f, 0f, 0, "LowHpFullHealShield", "saw", "MagicNovaBlue", 0f, "", "", "", "");
            config[2020351] = new SkillConfig(2020351, "抚军", "蒋琬", "战斗开始给本侧近战士兵套盾(/strength%最大生命)；释放额外为一名近战士兵套盾", "技", 1, 0f, 5f, 8, "", 0, "", 0f, 0f, "", 0, 0.25f, 0f, 0, "盾", false, 999f, "", 0, 0f, 0f, 0f, 0, "SoldierShield", "sway", "ShieldSoftBlue", 0f, "", "", "", "");
            config[2020352] = new SkillConfig(2020352, "抚军", "蒋琬", "", "技", 2, 0f, 5f, 8, "", 0, "", 0f, 0f, "", 0, 0.35f, 0f, 0, "盾", false, 999f, "", 0, 0f, 0f, 0f, 0, "SoldierShield", "sway", "ShieldSoftBlue", 0f, "", "", "", "");
            config[2020353] = new SkillConfig(2020353, "抚军", "蒋琬", "", "技", 3, 0f, 5f, 8, "", 0, "", 0f, 0f, "", 0, 0.5f, 0f, 0, "盾", false, 999f, "", 0, 0f, 0f, 0f, 0, "SoldierShield", "sway", "ShieldSoftBlue", 0f, "", "", "", "");
            config[2020354] = new SkillConfig(2020354, "抚军", "蒋琬", "", "技", 4, 0f, 5f, 8, "", 0, "", 0f, 0f, "", 0, 0.7f, 0f, 0, "盾", false, 999f, "", 0, 0f, 0f, 0f, 0, "SoldierShield", "sway", "ShieldSoftBlue", 0f, "", "", "", "");
            config[2020355] = new SkillConfig(2020355, "抚军", "蒋琬", "", "技", 5, 0f, 5f, 8, "", 0, "", 0f, 0f, "", 0, 0.95f, 0f, 0, "盾", false, 999f, "", 0, 0f, 0f, 0f, 0, "SoldierShield", "sway", "ShieldSoftBlue", 0f, "", "", "", "");
            config[2020356] = new SkillConfig(2020356, "励军", "马良", "释放：本侧所有士兵攻击+/strength%、护甲+/strengthint，永久可叠加", "技", 1, 0f, 5f, 15, "", 0, "", 0f, 0f, "", 0, 0.10f, 0f, 8, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "SoldierBuff", "sway", "MagicBuffGreen", 0f, "", "", "", "");
            config[2020357] = new SkillConfig(2020357, "励军", "马良", "", "技", 2, 0f, 5f, 15, "", 0, "", 0f, 0f, "", 0, 0.14f, 0f, 12, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "SoldierBuff", "sway", "MagicBuffGreen", 0f, "", "", "", "");
            config[2020358] = new SkillConfig(2020358, "励军", "马良", "", "技", 3, 0f, 5f, 15, "", 0, "", 0f, 0f, "", 0, 0.2f, 0f, 18, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "SoldierBuff", "sway", "MagicBuffGreen", 0f, "", "", "", "");
            config[2020359] = new SkillConfig(2020359, "励军", "马良", "", "技", 4, 0f, 5f, 15, "", 0, "", 0f, 0f, "", 0, 0.28f, 0f, 26, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "SoldierBuff", "sway", "MagicBuffGreen", 0f, "", "", "", "");
            config[2020360] = new SkillConfig(2020360, "励军", "马良", "", "技", 5, 0f, 5f, 15, "", 0, "", 0f, 0f, "", 0, 0.38f, 0f, 36, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "SoldierBuff", "sway", "MagicBuffGreen", 0f, "", "", "", "");
            config[2020361] = new SkillConfig(2020361, "筑垒", "张昭", "释放：为一名士兵永久强化护甲+/strength、魔抗+/strength2并回复满血；该士兵死亡3秒原地复活(仅一次)", "技", 1, 0f, 5f, 12, "", 0, "", 0f, 0f, "", 0, 12f, 8f, 0, "", false, 999f, "", 0, 0f, 0f, 0f, 0, "SoldierFortify", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2020362] = new SkillConfig(2020362, "筑垒", "张昭", "", "技", 2, 0f, 5f, 12, "", 0, "", 0f, 0f, "", 0, 18f, 12f, 0, "", false, 999f, "", 0, 0f, 0f, 0f, 0, "SoldierFortify", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2020363] = new SkillConfig(2020363, "筑垒", "张昭", "", "技", 3, 0f, 5f, 12, "", 0, "", 0f, 0f, "", 0, 26f, 18f, 0, "", false, 999f, "", 0, 0f, 0f, 0f, 0, "SoldierFortify", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2020364] = new SkillConfig(2020364, "筑垒", "张昭", "", "技", 4, 0f, 5f, 12, "", 0, "", 0f, 0f, "", 0, 36f, 25f, 0, "", false, 999f, "", 0, 0f, 0f, 0f, 0, "SoldierFortify", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2020365] = new SkillConfig(2020365, "筑垒", "张昭", "", "技", 5, 0f, 5f, 12, "", 0, "", 0f, 0f, "", 0, 50f, 35f, 0, "", false, 999f, "", 0, 0f, 0f, 0f, 0, "SoldierFortify", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2020366] = new SkillConfig(2020366, "乱阵", "贾诩", "释放：对目标造成/strength法术伤害并眩晕/bufftime秒；战斗开始给本侧近战士兵附加普攻眩晕Buff，普攻按/strengthint%概率眩晕2秒", "术", 1, 0f, 6f, 18, "", 0, "", 40f, 0f, "", 0, 30f, 0f, 10, "威", false, 1.5f, "", 0, 0f, 0f, 0f, 0, "SoldierStun", "sway", "MagicNovaBlue", 0f, "", "", "", "");
            config[2020367] = new SkillConfig(2020367, "乱阵", "贾诩", "", "术", 2, 0f, 6f, 18, "", 0, "", 40f, 0f, "", 0, 45f, 0f, 15, "威", false, 1.6f, "", 0, 0f, 0f, 0f, 0, "SoldierStun", "sway", "MagicNovaBlue", 0f, "", "", "", "");
            config[2020368] = new SkillConfig(2020368, "乱阵", "贾诩", "", "术", 3, 0f, 6f, 18, "", 0, "", 40f, 0f, "", 0, 60f, 0f, 20, "威", false, 1.7f, "", 0, 0f, 0f, 0f, 0, "SoldierStun", "sway", "MagicNovaBlue", 0f, "", "", "", "");
            config[2020369] = new SkillConfig(2020369, "乱阵", "贾诩", "", "术", 4, 0f, 6f, 18, "", 0, "", 40f, 0f, "", 0, 80f, 0f, 30, "威", false, 1.8f, "", 0, 0f, 0f, 0f, 0, "SoldierStun", "sway", "MagicNovaBlue", 0f, "", "", "", "");
            config[2020370] = new SkillConfig(2020370, "乱阵", "贾诩", "", "术", 5, 0f, 6f, 18, "", 0, "", 40f, 0f, "", 0, 100f, 0f, 40, "威", false, 2f, "", 0, 0f, 0f, 0f, 0, "SoldierStun", "sway", "MagicNovaBlue", 0f, "", "", "", "");
            config[2020376] = new SkillConfig(2020376, "强弓", "沮授", "释放：本侧弓兵攻击+/strength%、攻速+/strength2%，并强化自身攻击与攻速，永久可叠加", "技", 1, 0f, 6f, 15, "", 0, "", 0f, 0f, "", 0, 0.15f, 0.08f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "SoldierArcherBuff", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2020377] = new SkillConfig(2020377, "强弓", "沮授", "", "技", 2, 0f, 6f, 15, "", 0, "", 0f, 0f, "", 0, 0.22f, 0.10f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "SoldierArcherBuff", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2020378] = new SkillConfig(2020378, "强弓", "沮授", "", "技", 3, 0f, 6f, 15, "", 0, "", 0f, 0f, "", 0, 0.30f, 0.14f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "SoldierArcherBuff", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2020379] = new SkillConfig(2020379, "强弓", "沮授", "", "技", 4, 0f, 6f, 15, "", 0, "", 0f, 0f, "", 0, 0.40f, 0.18f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "SoldierArcherBuff", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2020380] = new SkillConfig(2020380, "强弓", "沮授", "", "技", 5, 0f, 6f, 15, "", 0, "", 0f, 0f, "", 0, 0.50f, 0.22f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "SoldierArcherBuff", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2020381] = new SkillConfig(2020381, "兵精", "荀彧", "释放：祝福一名士兵(优先近战)回复满血，攻+/strength%、护甲+/strengthint、魔抗+/strength2，每名士兵整场仅一次", "技", 1, 0f, 7f, 20, "", 0, "", 0f, 0f, "", 0, 0.50f, 30f, 40, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "SoldierBless", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2020382] = new SkillConfig(2020382, "兵精", "荀彧", "", "技", 2, 0f, 7f, 20, "", 0, "", 0f, 0f, "", 0, 0.70f, 40f, 55, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "SoldierBless", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2020383] = new SkillConfig(2020383, "兵精", "荀彧", "", "技", 3, 0f, 7f, 20, "", 0, "", 0f, 0f, "", 0, 0.95f, 55f, 75, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "SoldierBless", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2020384] = new SkillConfig(2020384, "兵精", "荀彧", "", "技", 4, 0f, 7f, 20, "", 0, "", 0f, 0f, "", 0, 1.25f, 75f, 100, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "SoldierBless", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2020385] = new SkillConfig(2020385, "兵精", "荀彧", "", "技", 5, 0f, 7f, 20, "", 0, "", 0f, 0f, "", 0, 1.60f, 95f, 130, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "SoldierBless", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2020386] = new SkillConfig(2020386, "连锁", "庞统", "锁定目标范围敌人，扩散/strength伤害，范围/area", "术", 1, 0f, 0f, 20, "", 0, "", 0f, 40f, "targetUnit", 3, 0.4f, 0f, 0, "锁", false, 8f, "", 0, 0f, 0f, 0f, 0, "HitBuffArea", "throw", "MagicNovaYellow", 0f, "", "", "", "");
            config[2020387] = new SkillConfig(2020387, "连锁", "庞统", "", "术", 2, 0f, 0f, 20, "", 0, "", 0f, 50f, "targetUnit", 3, 0.5f, 0f, 0, "锁", false, 8f, "", 0, 0f, 0f, 0f, 0, "HitBuffArea", "throw", "MagicNovaYellow", 0f, "", "", "", "");
            config[2020388] = new SkillConfig(2020388, "连锁", "庞统", "", "术", 3, 0f, 0f, 20, "", 0, "", 0f, 60f, "targetUnit", 3, 0.6f, 0f, 0, "锁", false, 8f, "", 0, 0f, 0f, 0f, 0, "HitBuffArea", "throw", "MagicNovaYellow", 0f, "", "", "", "");
            config[2020389] = new SkillConfig(2020389, "连锁", "庞统", "", "术", 4, 0f, 0f, 20, "", 0, "", 0f, 70f, "targetUnit", 3, 0.7f, 0f, 0, "锁", false, 8f, "", 0, 0f, 0f, 0f, 0, "HitBuffArea", "throw", "MagicNovaYellow", 0f, "", "", "", "");
            config[2020390] = new SkillConfig(2020390, "连锁", "庞统", "", "术", 5, 0f, 0f, 20, "", 0, "", 0f, 80f, "targetUnit", 3, 0.8f, 0f, 0, "锁", false, 8f, "", 0, 0f, 0f, 0f, 0, "HitBuffArea", "throw", "MagicNovaYellow", 0f, "", "", "", "");
            config[2020401] = new SkillConfig(2020401, "鼓舞", "孙乾", "我方全体生命回复/auroattrs-hpRegen/秒，随时间增强", "光环", 1, 0f, 5f, 0, "", 0, "", 0f, 0f, "", 0, 0.5f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AidAura", "", "", 0f, "", "", "", "hpRegen+2");
            config[2020402] = new SkillConfig(2020402, "鼓舞", "孙乾", "", "光环", 2, 0f, 5f, 0, "", 0, "", 0f, 0f, "", 0, 0.5f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AidAura", "", "", 0f, "", "", "", "hpRegen+3");
            config[2020403] = new SkillConfig(2020403, "鼓舞", "孙乾", "", "光环", 3, 0f, 5f, 0, "", 0, "", 0f, 0f, "", 0, 0.5f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AidAura", "", "", 0f, "", "", "", "hpRegen+4");
            config[2020404] = new SkillConfig(2020404, "鼓舞", "孙乾", "", "光环", 4, 0f, 5f, 0, "", 0, "", 0f, 0f, "", 0, 0.5f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AidAura", "", "", 0f, "", "", "", "hpRegen+5");
            config[2020405] = new SkillConfig(2020405, "鼓舞", "孙乾", "", "光环", 5, 0f, 5f, 0, "", 0, "", 0f, 0f, "", 0, 0.5f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AidAura", "", "", 0f, "", "", "", "hpRegen+7");
            config[2020406] = new SkillConfig(2020406, "安民", "费祎", "我方全体防御/auroattrs-armor，随时间增强", "光环", 1, 0f, 5f, 0, "", 0, "", 0f, 0f, "", 0, 0.5f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AidAura", "", "", 0f, "", "", "", "armor+5");
            config[2020407] = new SkillConfig(2020407, "安民", "费祎", "", "光环", 2, 0f, 5f, 0, "", 0, "", 0f, 0f, "", 0, 0.5f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AidAura", "", "", 0f, "", "", "", "armor+7");
            config[2020408] = new SkillConfig(2020408, "安民", "费祎", "", "光环", 3, 0f, 5f, 0, "", 0, "", 0f, 0f, "", 0, 0.5f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AidAura", "", "", 0f, "", "", "", "armor+10");
            config[2020409] = new SkillConfig(2020409, "安民", "费祎", "", "光环", 4, 0f, 5f, 0, "", 0, "", 0f, 0f, "", 0, 0.5f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AidAura", "", "", 0f, "", "", "", "armor+13");
            config[2020410] = new SkillConfig(2020410, "安民", "费祎", "", "光环", 5, 0f, 5f, 0, "", 0, "", 0f, 0f, "", 0, 0.5f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AidAura", "", "", 0f, "", "", "", "armor+16");
            config[2020411] = new SkillConfig(2020411, "敦睦", "诸葛瑾", "我方全体攻击/auroattrs-atk，随时间增强", "光环", 1, 0f, 5f, 0, "", 0, "", 0f, 0f, "", 0, 0.5f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AidAura", "", "", 0f, "", "", "", "atk+12");
            config[2020412] = new SkillConfig(2020412, "敦睦", "诸葛瑾", "", "光环", 2, 0f, 5f, 0, "", 0, "", 0f, 0f, "", 0, 0.5f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AidAura", "", "", 0f, "", "", "", "atk+16");
            config[2020413] = new SkillConfig(2020413, "敦睦", "诸葛瑾", "", "光环", 3, 0f, 5f, 0, "", 0, "", 0f, 0f, "", 0, 0.5f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AidAura", "", "", 0f, "", "", "", "atk+20");
            config[2020414] = new SkillConfig(2020414, "敦睦", "诸葛瑾", "", "光环", 4, 0f, 5f, 0, "", 0, "", 0f, 0f, "", 0, 0.5f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AidAura", "", "", 0f, "", "", "", "atk+26");
            config[2020415] = new SkillConfig(2020415, "敦睦", "诸葛瑾", "", "光环", 5, 0f, 5f, 0, "", 0, "", 0f, 0f, "", 0, 0.5f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AidAura", "", "", 0f, "", "", "", "atk+32");
            config[2020416] = new SkillConfig(2020416, "直谏", "田丰", "我方全体法强/auroattrs-ap，随时间增强", "光环", 1, 0f, 5f, 0, "", 0, "", 0f, 0f, "", 0, 0.5f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AidAura", "", "", 0f, "", "", "", "ap+15");
            config[2020417] = new SkillConfig(2020417, "直谏", "田丰", "", "光环", 2, 0f, 5f, 0, "", 0, "", 0f, 0f, "", 0, 0.5f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AidAura", "", "", 0f, "", "", "", "ap+20");
            config[2020418] = new SkillConfig(2020418, "直谏", "田丰", "", "光环", 3, 0f, 5f, 0, "", 0, "", 0f, 0f, "", 0, 0.5f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AidAura", "", "", 0f, "", "", "", "ap+25");
            config[2020419] = new SkillConfig(2020419, "直谏", "田丰", "", "光环", 4, 0f, 5f, 0, "", 0, "", 0f, 0f, "", 0, 0.5f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AidAura", "", "", 0f, "", "", "", "ap+32");
            config[2020420] = new SkillConfig(2020420, "直谏", "田丰", "", "光环", 5, 0f, 5f, 0, "", 0, "", 0f, 0f, "", 0, 0.5f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AidAura", "", "", 0f, "", "", "", "ap+40");
            config[2020421] = new SkillConfig(2020421, "果决", "司马师", "我方全体暴击率/auroattrs-crit%，随时间增强", "光环", 1, 0f, 5f, 0, "", 0, "", 0f, 0f, "", 0, 0.5f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AidAura", "", "", 0f, "", "", "", "crit+0.03");
            config[2020422] = new SkillConfig(2020422, "果决", "司马师", "", "光环", 2, 0f, 5f, 0, "", 0, "", 0f, 0f, "", 0, 0.5f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AidAura", "", "", 0f, "", "", "", "crit+0.05");
            config[2020423] = new SkillConfig(2020423, "果决", "司马师", "", "光环", 3, 0f, 5f, 0, "", 0, "", 0f, 0f, "", 0, 0.5f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AidAura", "", "", 0f, "", "", "", "crit+0.07");
            config[2020424] = new SkillConfig(2020424, "果决", "司马师", "", "光环", 4, 0f, 5f, 0, "", 0, "", 0f, 0f, "", 0, 0.5f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AidAura", "", "", 0f, "", "", "", "crit+0.10");
            config[2020425] = new SkillConfig(2020425, "果决", "司马师", "", "光环", 5, 0f, 5f, 0, "", 0, "", 0f, 0f, "", 0, 0.5f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AidAura", "", "", 0f, "", "", "", "crit+0.14");
            config[2020426] = new SkillConfig(2020426, "长者", "鲁肃", "我方全体护甲/auroattrs-armor，魔抗/auroattrs-magicres，随时间增强", "光环", 1, 0f, 5f, 0, "", 0, "", 0f, 0f, "", 0, 0.5f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AidAura", "", "", 0f, "", "", "", "armor+10,magicres+10");
            config[2020427] = new SkillConfig(2020427, "长者", "鲁肃", "", "光环", 2, 0f, 5f, 0, "", 0, "", 0f, 0f, "", 0, 0.5f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AidAura", "", "", 0f, "", "", "", "armor+14,magicres+14");
            config[2020428] = new SkillConfig(2020428, "长者", "鲁肃", "", "光环", 3, 0f, 5f, 0, "", 0, "", 0f, 0f, "", 0, 0.5f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AidAura", "", "", 0f, "", "", "", "armor+18,magicres+18");
            config[2020429] = new SkillConfig(2020429, "长者", "鲁肃", "", "光环", 4, 0f, 5f, 0, "", 0, "", 0f, 0f, "", 0, 0.5f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AidAura", "", "", 0f, "", "", "", "armor+24,magicres+24");
            config[2020430] = new SkillConfig(2020430, "长者", "鲁肃", "", "光环", 5, 0f, 5f, 0, "", 0, "", 0f, 0f, "", 0, 0.5f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "AidAura", "", "", 0f, "", "", "", "armor+30,magicres+30");
            config[2020501] = new SkillConfig(2020501, "困敌", "徐庶", "释放法阵，对/area范围内最多/targetcount个敌人造成/strength法术伤害，并降低其/strength2点攻击与/strengthint%治疗，持续/bufftime秒", "术", 1, 0f, 3f, 18, "", 0, "", 0f, 25f, "", 3, 18f, 15f, 20, "慑", false, 4f, "", 0, 0f, 0f, 0f, 0, "SkillAidHexField", "spin", "MagicNovaBlue", 5f, "", "", "", "");
            config[2020502] = new SkillConfig(2020502, "困敌", "徐庶", "", "术", 2, 0f, 3f, 18, "", 0, "", 0f, 25f, "", 3, 28f, 20f, 25, "慑", false, 4f, "", 0, 0f, 0f, 0f, 0, "SkillAidHexField", "spin", "MagicNovaBlue", 5f, "", "", "", "");
            config[2020503] = new SkillConfig(2020503, "困敌", "徐庶", "", "术", 3, 0f, 3f, 18, "", 0, "", 0f, 25f, "", 3, 40f, 28f, 30, "慑", false, 4f, "", 0, 0f, 0f, 0f, 0, "SkillAidHexField", "spin", "MagicNovaBlue", 5f, "", "", "", "");
            config[2020504] = new SkillConfig(2020504, "困敌", "徐庶", "", "术", 4, 0f, 3f, 18, "", 0, "", 0f, 25f, "", 3, 55f, 38f, 35, "慑", false, 4f, "", 0, 0f, 0f, 0f, 0, "SkillAidHexField", "spin", "MagicNovaBlue", 5f, "", "", "", "");
            config[2020505] = new SkillConfig(2020505, "困敌", "徐庶", "", "术", 5, 0f, 3f, 18, "", 0, "", 0f, 25f, "", 3, 75f, 50f, 40, "慑", false, 4f, "", 0, 0f, 0f, 0f, 0, "SkillAidHexField", "spin", "MagicNovaBlue", 5f, "", "", "", "");
            config[2020506] = new SkillConfig(2020506, "震碎", "张郃", "轰击地面，对/area范围内最多/targetcount个敌人造成/strength法术伤害并减速/strength2%，持续/bufftime秒", "术", 1, 0f, 4f, 20, "", 0, "", 0f, 25f, "", 3, 22f, 0.25f, 0, "缓", false, 3f, "", 0, 0f, 0f, 0f, 0, "SkillAidQuake", "spin", "MagicNovaBlue", 5f, "", "", "", "");
            config[2020507] = new SkillConfig(2020507, "震碎", "张郃", "", "术", 2, 0f, 4f, 20, "", 0, "", 0f, 25f, "", 3, 34f, 0.30f, 0, "缓", false, 3f, "", 0, 0f, 0f, 0f, 0, "SkillAidQuake", "spin", "MagicNovaBlue", 5f, "", "", "", "");
            config[2020508] = new SkillConfig(2020508, "震碎", "张郃", "", "术", 3, 0f, 4f, 20, "", 0, "", 0f, 25f, "", 3, 48f, 0.35f, 0, "缓", false, 3f, "", 0, 0f, 0f, 0f, 0, "SkillAidQuake", "spin", "MagicNovaBlue", 5f, "", "", "", "");
            config[2020509] = new SkillConfig(2020509, "震碎", "张郃", "", "术", 4, 0f, 4f, 20, "", 0, "", 0f, 25f, "", 3, 68f, 0.40f, 0, "缓", false, 3f, "", 0, 0f, 0f, 0f, 0, "SkillAidQuake", "spin", "MagicNovaBlue", 5f, "", "", "", "");
            config[2020510] = new SkillConfig(2020510, "震碎", "张郃", "", "术", 5, 0f, 4f, 20, "", 0, "", 0f, 25f, "", 3, 92f, 0.45f, 0, "缓", false, 3f, "", 0, 0f, 0f, 0f, 0, "SkillAidQuake", "spin", "MagicNovaBlue", 5f, "", "", "", "");
            config[2020511] = new SkillConfig(2020511, "火海", "陆逊", "召唤大量火焰散布于/range范围随机位置，每秒造成/strength法术灼烧伤害", "术", 1, 0f, 6f, 24, "", 0, "", 40f, 12f, "", 2, 22f, 0f, 0, "", false, 0f, "火", 4, 3.2f, 1f, 0f, 0, "SkillAidScorchedEarth", "spin", "SoftFireBigRed", 1.6f, "", "", "", "");
            config[2020512] = new SkillConfig(2020512, "火海", "陆逊", "", "术", 2, 0f, 6f, 24, "", 0, "", 40f, 12f, "", 2, 30f, 0f, 0, "", false, 0f, "火", 4, 3.2f, 1f, 0f, 0, "SkillAidScorchedEarth", "spin", "SoftFireBigRed", 1.6f, "", "", "", "");
            config[2020513] = new SkillConfig(2020513, "火海", "陆逊", "", "术", 3, 0f, 6f, 24, "", 0, "", 40f, 12f, "", 2, 42f, 0f, 0, "", false, 0f, "火", 5, 3.2f, 1f, 0f, 0, "SkillAidScorchedEarth", "spin", "SoftFireBigRed", 1.6f, "", "", "", "");
            config[2020514] = new SkillConfig(2020514, "火海", "陆逊", "", "术", 4, 0f, 6f, 24, "", 0, "", 40f, 12f, "", 2, 60f, 0f, 0, "", false, 0f, "火", 5, 3.2f, 1f, 0f, 0, "SkillAidScorchedEarth", "spin", "SoftFireBigRed", 1.6f, "", "", "", "");
            config[2020515] = new SkillConfig(2020515, "火海", "陆逊", "", "术", 5, 0f, 6f, 24, "", 0, "", 40f, 12f, "", 2, 80f, 0f, 0, "", false, 0f, "火", 6, 3.2f, 1f, 0f, 0, "SkillAidScorchedEarth", "spin", "SoftFireBigRed", 1.6f, "", "", "", "");
            config[2020516] = new SkillConfig(2020516, "奋威", "丁奉", "振奋精神，接下来3次攻击造成/strength倍伤害", "武", 1, 0f, 6f, 15, "", 1, "", 0f, 0f, "", 0, 2f, 0f, 0, "倍", false, 5f, "", 0, 0f, 0f, 0f, 0, "SkillAidEmpower", "spin", "MagicChargeYellow", 0f, "", "", "", "");
            config[2020517] = new SkillConfig(2020517, "奋威", "丁奉", "", "武", 2, 0f, 6f, 15, "", 1, "", 0f, 0f, "", 0, 2.2f, 0f, 0, "倍", false, 5f, "", 0, 0f, 0f, 0f, 0, "SkillAidEmpower", "spin", "MagicChargeYellow", 0f, "", "", "", "");
            config[2020518] = new SkillConfig(2020518, "奋威", "丁奉", "", "武", 3, 0f, 6f, 15, "", 1, "", 0f, 0f, "", 0, 2.4f, 0f, 0, "倍", false, 5f, "", 0, 0f, 0f, 0f, 0, "SkillAidEmpower", "spin", "MagicChargeYellow", 0f, "", "", "", "");
            config[2020519] = new SkillConfig(2020519, "奋威", "丁奉", "", "武", 4, 0f, 6f, 15, "", 1, "", 0f, 0f, "", 0, 2.6f, 0f, 0, "倍", false, 5f, "", 0, 0f, 0f, 0f, 0, "SkillAidEmpower", "spin", "MagicChargeYellow", 0f, "", "", "", "");
            config[2020520] = new SkillConfig(2020520, "奋威", "丁奉", "", "武", 5, 0f, 6f, 15, "", 1, "", 0f, 0f, "", 0, 3f, 0f, 0, "倍", false, 5f, "", 0, 0f, 0f, 0f, 0, "SkillAidEmpower", "spin", "MagicChargeYellow", 0f, "", "", "", "");
            config[2020521] = new SkillConfig(2020521, "破甲", "高顺", "炮弹无视目标护甲，对带护盾的目标额外造成/strength%攻击伤害", "武", 1, 0f, 3.5f, 14, "", 1, "", 0f, 0f, "", 0, 0.5f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "SkillAidArmorIgnore", "spin", "SwordWhirlwindWhite", 0f, "", "", "", "");
            config[2020522] = new SkillConfig(2020522, "破甲", "高顺", "", "武", 2, 0f, 3.5f, 14, "", 1, "", 0f, 0f, "", 0, 0.7f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "SkillAidArmorIgnore", "spin", "SwordWhirlwindWhite", 0f, "", "", "", "");
            config[2020523] = new SkillConfig(2020523, "破甲", "高顺", "", "武", 3, 0f, 3.5f, 14, "", 1, "", 0f, 0f, "", 0, 0.9f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "SkillAidArmorIgnore", "spin", "SwordWhirlwindWhite", 0f, "", "", "", "");
            config[2020524] = new SkillConfig(2020524, "破甲", "高顺", "", "武", 4, 0f, 3.5f, 14, "", 1, "", 0f, 0f, "", 0, 1.2f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "SkillAidArmorIgnore", "spin", "SwordWhirlwindWhite", 0f, "", "", "", "");
            config[2020525] = new SkillConfig(2020525, "破甲", "高顺", "", "武", 5, 0f, 3.5f, 14, "", 1, "", 0f, 0f, "", 0, 1.5f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "SkillAidArmorIgnore", "spin", "SwordWhirlwindWhite", 0f, "", "", "", "");
            config[2020526] = new SkillConfig(2020526, "疾击", "王濬", "短时间内攻击速度提升/strength%，持续/bufftime秒", "武", 1, 0f, 4.5f, 12, "", 1, "", 0f, 0f, "", 0, 0.25f, 0f, 0, "快", false, 2.5f, "", 0, 0f, 0f, 0f, 0, "SkillAidFrenzy", "shoot", "HeartStream", 0f, "", "", "", "");
            config[2020527] = new SkillConfig(2020527, "疾击", "王濬", "", "武", 2, 0f, 4.5f, 12, "", 1, "", 0f, 0f, "", 0, 0.30f, 0f, 0, "快", false, 2.5f, "", 0, 0f, 0f, 0f, 0, "SkillAidFrenzy", "shoot", "HeartStream", 0f, "", "", "", "");
            config[2020528] = new SkillConfig(2020528, "疾击", "王濬", "", "武", 3, 0f, 4.5f, 12, "", 1, "", 0f, 0f, "", 0, 0.36f, 0f, 0, "快", false, 2.5f, "", 0, 0f, 0f, 0f, 0, "SkillAidFrenzy", "shoot", "HeartStream", 0f, "", "", "", "");
            config[2020529] = new SkillConfig(2020529, "疾击", "王濬", "", "武", 4, 0f, 4.5f, 12, "", 1, "", 0f, 0f, "", 0, 0.42f, 0f, 0, "快", false, 2.5f, "", 0, 0f, 0f, 0f, 0, "SkillAidFrenzy", "shoot", "HeartStream", 0f, "", "", "", "");
            config[2020530] = new SkillConfig(2020530, "疾击", "王濬", "", "武", 5, 0f, 4.5f, 12, "", 1, "", 0f, 0f, "", 0, 0.50f, 0f, 0, "快", false, 2.5f, "", 0, 0f, 0f, 0f, 0, "SkillAidFrenzy", "shoot", "HeartStream", 0f, "", "", "", "");
            config[2020531] = new SkillConfig(2020531, "连射", "马岱", "每次发动永久提升自身/strength%攻击速度", "武", 1, 0f, 3f, 10, "", 1, "", 0f, 0f, "", 0, 0.1f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "SkillAidLastingFocus", "shoot", "HeartStream", 0f, "", "", "", "");
            config[2020532] = new SkillConfig(2020532, "连射", "马岱", "", "武", 2, 0f, 3f, 10, "", 1, "", 0f, 0f, "", 0, 0.12f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "SkillAidLastingFocus", "shoot", "HeartStream", 0f, "", "", "", "");
            config[2020533] = new SkillConfig(2020533, "连射", "马岱", "", "武", 3, 0f, 3f, 10, "", 1, "", 0f, 0f, "", 0, 0.14f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "SkillAidLastingFocus", "shoot", "HeartStream", 0f, "", "", "", "");
            config[2020534] = new SkillConfig(2020534, "连射", "马岱", "", "武", 4, 0f, 3f, 10, "", 1, "", 0f, 0f, "", 0, 0.17f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "SkillAidLastingFocus", "shoot", "HeartStream", 0f, "", "", "", "");
            config[2020535] = new SkillConfig(2020535, "连射", "马岱", "", "武", 5, 0f, 3f, 10, "", 1, "", 0f, 0f, "", 0, 0.20f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "SkillAidLastingFocus", "shoot", "HeartStream", 0f, "", "", "", "");
            config[2020536] = new SkillConfig(2020536, "疾袭", "徐晃", "攻击时优先射击敌阵最后方的敌人，额外造成/strength%伤害", "武", 1, 0f, 4f, 14, "", 1, "", 60f, 0f, "", 0, 0f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "SkillAidBacklineHunt", "shoot", "", 0f, "", "", "", "");
            config[2020537] = new SkillConfig(2020537, "疾袭", "徐晃", "", "武", 2, 0f, 4f, 14, "", 1, "", 60f, 0f, "", 0, 0.1f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "SkillAidBacklineHunt", "shoot", "", 0f, "", "", "", "");
            config[2020538] = new SkillConfig(2020538, "疾袭", "徐晃", "", "武", 3, 0f, 4f, 14, "", 1, "", 60f, 0f, "", 0, 0.2f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "SkillAidBacklineHunt", "shoot", "", 0f, "", "", "", "");
            config[2020539] = new SkillConfig(2020539, "疾袭", "徐晃", "", "武", 4, 0f, 4f, 14, "", 1, "", 60f, 0f, "", 0, 0.3f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "SkillAidBacklineHunt", "shoot", "", 0f, "", "", "", "");
            config[2020540] = new SkillConfig(2020540, "疾袭", "徐晃", "", "武", 5, 0f, 4f, 14, "", 1, "", 60f, 0f, "", 0, 0.4f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "SkillAidBacklineHunt", "shoot", "", 0f, "", "", "", "");
            config[2020541] = new SkillConfig(2020541, "追袭", "文鸯", "短时间内伤害提升/strength%并吸血/strength2%，持续/bufftime秒", "武", 1, 0f, 5f, 14, "", 1, "", 0f, 0f, "", 0, 0.3f, 0.2f, 0, "袭", false, 3.5f, "", 0, 0f, 0f, 0f, 0, "SkillAidLifestealStrike", "shoot", "BloodExplosion", 0f, "", "", "", "");
            config[2020542] = new SkillConfig(2020542, "追袭", "文鸯", "", "武", 2, 0f, 5f, 14, "", 1, "", 0f, 0f, "", 0, 0.4f, 0.25f, 0, "袭", false, 3.5f, "", 0, 0f, 0f, 0f, 0, "SkillAidLifestealStrike", "shoot", "BloodExplosion", 0f, "", "", "", "");
            config[2020543] = new SkillConfig(2020543, "追袭", "文鸯", "", "武", 3, 0f, 5f, 14, "", 1, "", 0f, 0f, "", 0, 0.5f, 0.30f, 0, "袭", false, 3.5f, "", 0, 0f, 0f, 0f, 0, "SkillAidLifestealStrike", "shoot", "BloodExplosion", 0f, "", "", "", "");
            config[2020544] = new SkillConfig(2020544, "追袭", "文鸯", "", "武", 4, 0f, 5f, 14, "", 1, "", 0f, 0f, "", 0, 0.6f, 0.35f, 0, "袭", false, 3.5f, "", 0, 0f, 0f, 0f, 0, "SkillAidLifestealStrike", "shoot", "BloodExplosion", 0f, "", "", "", "");
            config[2020545] = new SkillConfig(2020545, "追袭", "文鸯", "", "武", 5, 0f, 5f, 14, "", 1, "", 0f, 0f, "", 0, 0.75f, 0.45f, 0, "袭", false, 3.5f, "", 0, 0f, 0f, 0f, 0, "SkillAidLifestealStrike", "shoot", "BloodExplosion", 0f, "", "", "", "");
            config[2020546] = new SkillConfig(2020546, "连环", "甘宁", "短时间内攻击额外射出/strength支箭，并提升/strength2%闪避，持续/bufftime秒", "武", 1, 0f, 6f, 18, "", 1, "", 40f, 0f, "", 0, 1f, 0.2f, 0, "箭", false, 4f, "", 0, 0f, 0f, 0f, 0, "SkillAidVolley", "shoot", "ShadowExplosionGreen", 0f, "", "", "", "");
            config[2020547] = new SkillConfig(2020547, "连环", "甘宁", "", "武", 2, 0f, 6f, 18, "", 1, "", 40f, 0f, "", 0, 1f, 0.22f, 0, "箭", false, 4f, "", 0, 0f, 0f, 0f, 0, "SkillAidVolley", "shoot", "ShadowExplosionGreen", 0f, "", "", "", "");
            config[2020548] = new SkillConfig(2020548, "连环", "甘宁", "", "武", 3, 0f, 6f, 18, "", 1, "", 40f, 0f, "", 0, 2f, 0.25f, 0, "箭", false, 4f, "", 0, 0f, 0f, 0f, 0, "SkillAidVolley", "shoot", "ShadowExplosionGreen", 0f, "", "", "", "");
            config[2020549] = new SkillConfig(2020549, "连环", "甘宁", "", "武", 4, 0f, 6f, 18, "", 1, "", 40f, 0f, "", 0, 2f, 0.28f, 0, "箭", false, 4f, "", 0, 0f, 0f, 0f, 0, "SkillAidVolley", "shoot", "ShadowExplosionGreen", 0f, "", "", "", "");
            config[2020550] = new SkillConfig(2020550, "连环", "甘宁", "", "武", 5, 0f, 6f, 18, "", 1, "", 40f, 0f, "", 0, 3f, 0.32f, 0, "箭", false, 4f, "", 0, 0f, 0f, 0f, 0, "SkillAidVolley", "shoot", "ShadowExplosionGreen", 0f, "", "", "", "");
            config[2020551] = new SkillConfig(2020551, "纵舞", "孙尚香", "每射击2次后退一段距离，拉开与敌方的距离", "武", 1, 0f, 4f, 12, "", 1, "", 40f, 0f, "", 0, 0f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "SkillAidDance", "shoot", "", 0f, "", "", "", "");
            config[2020552] = new SkillConfig(2020552, "纵舞", "孙尚香", "", "武", 2, 0f, 4f, 12, "", 1, "", 40f, 0f, "", 0, 0f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "SkillAidDance", "shoot", "", 0f, "", "", "", "");
            config[2020553] = new SkillConfig(2020553, "纵舞", "孙尚香", "", "武", 3, 0f, 4f, 12, "", 1, "", 40f, 0f, "", 0, 0f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "SkillAidDance", "shoot", "", 0f, "", "", "", "");
            config[2020554] = new SkillConfig(2020554, "纵舞", "孙尚香", "", "武", 4, 0f, 4f, 12, "", 1, "", 40f, 0f, "", 0, 0f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "SkillAidDance", "shoot", "", 0f, "", "", "", "");
            config[2020555] = new SkillConfig(2020555, "纵舞", "孙尚香", "", "武", 5, 0f, 4f, 12, "", 1, "", 40f, 0f, "", 0, 0f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "SkillAidDance", "shoot", "", 0f, "", "", "", "");
            config[2020556] = new SkillConfig(2020556, "先锋", "鞠义", "短时间内攻击速度提升/strength%，持续/bufftime秒", "武", 1, 0f, 4.5f, 12, "", 1, "", 0f, 0f, "", 0, 0.2f, 0f, 0, "快", false, 2.5f, "", 0, 0f, 0f, 0f, 0, "SkillAidAssault", "shoot", "HeartStream", 0f, "", "", "", "");
            config[2020557] = new SkillConfig(2020557, "先锋", "鞠义", "", "武", 2, 0f, 4.5f, 12, "", 1, "", 0f, 0f, "", 0, 0.25f, 0f, 0, "快", false, 2.5f, "", 0, 0f, 0f, 0f, 0, "SkillAidAssault", "shoot", "HeartStream", 0f, "", "", "", "");
            config[2020558] = new SkillConfig(2020558, "先锋", "鞠义", "", "武", 3, 0f, 4.5f, 12, "", 1, "", 0f, 0f, "", 0, 0.30f, 0f, 0, "快", false, 2.5f, "", 0, 0f, 0f, 0f, 0, "SkillAidAssault", "shoot", "HeartStream", 0f, "", "", "", "");
            config[2020559] = new SkillConfig(2020559, "先锋", "鞠义", "", "武", 4, 0f, 4.5f, 12, "", 1, "", 0f, 0f, "", 0, 0.36f, 0f, 0, "快", false, 2.5f, "", 0, 0f, 0f, 0f, 0, "SkillAidAssault", "shoot", "HeartStream", 0f, "", "", "", "");
            config[2020560] = new SkillConfig(2020560, "先锋", "鞠义", "", "武", 5, 0f, 4.5f, 12, "", 1, "", 0f, 0f, "", 0, 0.42f, 0f, 0, "快", false, 2.5f, "", 0, 0f, 0f, 0f, 0, "SkillAidAssault", "shoot", "HeartStream", 0f, "", "", "", "");
            config[2020561] = new SkillConfig(2020561, "腾驾", "马腾", "降低目标/strength2点攻击并提升自身/strength点攻击，持续/bufftime秒", "武", 1, 0f, 4f, 14, "", 1, "", 40f, 0f, "", 0, 20f, 14f, 0, "攻", false, 4f, "", 0, 0f, 0f, 0f, 0, "SkillAidAtkDrain", "shoot", "MagicChargeYellow", 0f, "", "", "", "");
            config[2020562] = new SkillConfig(2020562, "腾驾", "马腾", "", "武", 2, 0f, 4f, 14, "", 1, "", 40f, 0f, "", 0, 30f, 20f, 0, "攻", false, 4f, "", 0, 0f, 0f, 0f, 0, "SkillAidAtkDrain", "shoot", "MagicChargeYellow", 0f, "", "", "", "");
            config[2020563] = new SkillConfig(2020563, "腾驾", "马腾", "", "武", 3, 0f, 4f, 14, "", 1, "", 40f, 0f, "", 0, 42f, 28f, 0, "攻", false, 4f, "", 0, 0f, 0f, 0f, 0, "SkillAidAtkDrain", "shoot", "MagicChargeYellow", 0f, "", "", "", "");
            config[2020564] = new SkillConfig(2020564, "腾驾", "马腾", "", "武", 4, 0f, 4f, 14, "", 1, "", 40f, 0f, "", 0, 58f, 38f, 0, "攻", false, 4f, "", 0, 0f, 0f, 0f, 0, "SkillAidAtkDrain", "shoot", "MagicChargeYellow", 0f, "", "", "", "");
            config[2020565] = new SkillConfig(2020565, "腾驾", "马腾", "", "武", 5, 0f, 4f, 14, "", 1, "", 40f, 0f, "", 0, 80f, 50f, 0, "攻", false, 4f, "", 0, 0f, 0f, 0f, 0, "SkillAidAtkDrain", "shoot", "MagicChargeYellow", 0f, "", "", "", "");
            config[2020566] = new SkillConfig(2020566, "乱射", "黄忠", "向/area范围内最多/targetcount个敌人乱射，造成/strength物理伤害，附/strength2%暴击加成", "武", 1, 0f, 4f, 12, "", 1, "", 40f, 25f, "", 3, 30f, 0.5f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "SkillAidScatterShot", "shoot", "", 0f, "", "", "", "");
            config[2020567] = new SkillConfig(2020567, "乱射", "黄忠", "", "武", 2, 0f, 4f, 12, "", 1, "", 40f, 25f, "", 3, 45f, 0.6f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "SkillAidScatterShot", "shoot", "", 0f, "", "", "", "");
            config[2020568] = new SkillConfig(2020568, "乱射", "黄忠", "", "武", 3, 0f, 4f, 12, "", 1, "", 40f, 25f, "", 3, 62f, 0.7f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "SkillAidScatterShot", "shoot", "", 0f, "", "", "", "");
            config[2020569] = new SkillConfig(2020569, "乱射", "黄忠", "", "武", 4, 0f, 4f, 12, "", 1, "", 40f, 25f, "", 3, 85f, 0.8f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "SkillAidScatterShot", "shoot", "", 0f, "", "", "", "");
            config[2020570] = new SkillConfig(2020570, "乱射", "黄忠", "", "武", 5, 0f, 4f, 12, "", 1, "", 40f, 25f, "", 3, 115f, 1.0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "SkillAidScatterShot", "shoot", "", 0f, "", "", "", "");
            config[2020571] = new SkillConfig(2020571, "猎鹰", "夏侯渊", "对单体目标造成/strength物理伤害并降低其/strength2点护甲，持续/bufftime秒", "武", 1, 0f, 4.5f, 18, "", 1, "", 30f, 0f, "", 0, 60f, 8f, 0, "破", false, 3f, "", 0, 0f, 0f, 0f, 0, "SkillAidArmorShred", "shoot", "", 0f, "", "", "", "");
            config[2020572] = new SkillConfig(2020572, "猎鹰", "夏侯渊", "", "武", 2, 0f, 4.5f, 18, "", 1, "", 30f, 0f, "", 0, 95f, 10f, 0, "破", false, 3f, "", 0, 0f, 0f, 0f, 0, "SkillAidArmorShred", "shoot", "", 0f, "", "", "", "");
            config[2020573] = new SkillConfig(2020573, "猎鹰", "夏侯渊", "", "武", 3, 0f, 4.5f, 18, "", 1, "", 30f, 0f, "", 0, 140f, 14f, 0, "破", false, 3f, "", 0, 0f, 0f, 0f, 0, "SkillAidArmorShred", "shoot", "", 0f, "", "", "", "");
            config[2020574] = new SkillConfig(2020574, "猎鹰", "夏侯渊", "", "武", 4, 0f, 4.5f, 18, "", 1, "", 30f, 0f, "", 0, 195f, 20f, 0, "破", false, 3f, "", 0, 0f, 0f, 0f, 0, "SkillAidArmorShred", "shoot", "", 0f, "", "", "", "");
            config[2020575] = new SkillConfig(2020575, "猎鹰", "夏侯渊", "", "武", 5, 0f, 4.5f, 18, "", 1, "", 30f, 0f, "", 0, 260f, 28f, 0, "破", false, 3f, "", 0, 0f, 0f, 0f, 0, "SkillAidArmorShred", "shoot", "", 0f, "", "", "", "");
            config[2020576] = new SkillConfig(2020576, "连珠", "太史慈", "向前方/range范围射出最多/targetcount支箭，距离越近伤害越高(最高/strength2%加成)，每支造成/strength物理伤害", "武", 1, 0f, 4f, 16, "", 1, "", 40f, 60f, "", 3, 25f, 0.5f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "SkillAidBurstShot", "shoot", "", 0f, "", "", "", "");
            config[2020577] = new SkillConfig(2020577, "连珠", "太史慈", "", "武", 2, 0f, 4f, 16, "", 1, "", 40f, 60f, "", 3, 38f, 0.6f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "SkillAidBurstShot", "shoot", "", 0f, "", "", "", "");
            config[2020578] = new SkillConfig(2020578, "连珠", "太史慈", "", "武", 3, 0f, 4f, 16, "", 1, "", 40f, 60f, "", 3, 54f, 0.7f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "SkillAidBurstShot", "shoot", "", 0f, "", "", "", "");
            config[2020579] = new SkillConfig(2020579, "连珠", "太史慈", "", "武", 4, 0f, 4f, 16, "", 1, "", 40f, 60f, "", 4, 76f, 0.8f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "SkillAidBurstShot", "shoot", "", 0f, "", "", "", "");
            config[2020580] = new SkillConfig(2020580, "连珠", "太史慈", "", "武", 5, 0f, 4f, 16, "", 1, "", 40f, 60f, "", 4, 105f, 1.0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "SkillAidBurstShot", "shoot", "", 0f, "", "", "", "");
            config[2020581] = new SkillConfig(2020581, "火箭", "凌统", "射出火箭，对目标及周围每秒造成/strength物理灼烧伤害", "武", 1, 0f, 4f, 12, "", 1, "", 0f, 8f, "", 1, 20f, 0f, 0, "", false, 0f, "火", 1, 3.2f, 1f, 0f, 0, "HitWall", "throw", "SoftFireBigRed", 1.6f, "", "", "", "");
            config[2020582] = new SkillConfig(2020582, "火箭", "凌统", "", "武", 2, 0f, 4f, 12, "", 1, "", 0f, 8f, "", 1, 30f, 0f, 0, "", false, 0f, "火", 1, 3.2f, 1f, 0f, 0, "HitWall", "throw", "SoftFireBigRed", 1.6f, "", "", "", "");
            config[2020583] = new SkillConfig(2020583, "火箭", "凌统", "", "武", 3, 0f, 4f, 12, "", 1, "", 0f, 8f, "", 1, 40f, 0f, 0, "", false, 0f, "火", 1, 3.2f, 1f, 0f, 0, "HitWall", "throw", "SoftFireBigRed", 1.6f, "", "", "", "");
            config[2020584] = new SkillConfig(2020584, "火箭", "凌统", "", "武", 4, 0f, 4f, 12, "", 1, "", 0f, 8f, "", 1, 55f, 0f, 0, "", false, 0f, "火", 1, 3.2f, 1f, 0f, 0, "HitWall", "throw", "SoftFireBigRed", 1.6f, "", "", "", "");
            config[2020585] = new SkillConfig(2020585, "火箭", "凌统", "", "武", 5, 0f, 4f, 12, "", 1, "", 0f, 8f, "", 1, 75f, 0f, 0, "", false, 0f, "火", 1, 3.2f, 1f, 0f, 0, "HitWall", "throw", "SoftFireBigRed", 1.6f, "", "", "", "");
            config[2020586] = new SkillConfig(2020586, "御守", "高览", "对单体目标造成/strength物理伤害，同时获得/strength2%最大生命的护盾，持续/bufftime秒", "武", 1, 0f, 4f, 16, "", 1, "", 40f, 0f, "", 0, 50f, 0.15f, 0, "盾", false, 6f, "", 0, 0f, 0f, 0f, 0, "SkillAidGuardian", "shoot", "MagicChargeYellow", 0f, "", "", "", "");
            config[2020587] = new SkillConfig(2020587, "御守", "高览", "", "武", 2, 0f, 4f, 16, "", 1, "", 40f, 0f, "", 0, 80f, 0.2f, 0, "盾", false, 6f, "", 0, 0f, 0f, 0f, 0, "SkillAidGuardian", "shoot", "MagicChargeYellow", 0f, "", "", "", "");
            config[2020588] = new SkillConfig(2020588, "御守", "高览", "", "武", 3, 0f, 4f, 16, "", 1, "", 40f, 0f, "", 0, 115f, 0.25f, 0, "盾", false, 6f, "", 0, 0f, 0f, 0f, 0, "SkillAidGuardian", "shoot", "MagicChargeYellow", 0f, "", "", "", "");
            config[2020589] = new SkillConfig(2020589, "御守", "高览", "", "武", 4, 0f, 4f, 16, "", 1, "", 40f, 0f, "", 0, 160f, 0.3f, 0, "盾", false, 6f, "", 0, 0f, 0f, 0f, 0, "SkillAidGuardian", "shoot", "MagicChargeYellow", 0f, "", "", "", "");
            config[2020590] = new SkillConfig(2020590, "御守", "高览", "", "武", 5, 0f, 4f, 16, "", 1, "", 40f, 0f, "", 0, 220f, 0.38f, 0, "盾", false, 6f, "", 0, 0f, 0f, 0f, 0, "SkillAidGuardian", "shoot", "MagicChargeYellow", 0f, "", "", "", "");
            config[2020591] = new SkillConfig(2020591, "毒箭", "张任", "射出淬毒之箭，造成/strength2倍/strength点物理伤害，并附加每秒/strength点中毒伤害，持续/bufftime秒", "武", 1, 0f, 4f, 16, "", 1, "", 40f, 0f, "", 0, 12f, 3f, 0, "败", false, 4f, "", 0, 0f, 0f, 0f, 0, "SkillAidPoisonArrow", "shoot", "", 0f, "", "", "", "");
            config[2020592] = new SkillConfig(2020592, "毒箭", "张任", "", "武", 2, 0f, 4f, 16, "", 1, "", 40f, 0f, "", 0, 16f, 3.2f, 0, "败", false, 4f, "", 0, 0f, 0f, 0f, 0, "SkillAidPoisonArrow", "shoot", "", 0f, "", "", "", "");
            config[2020593] = new SkillConfig(2020593, "毒箭", "张任", "", "武", 3, 0f, 4f, 16, "", 1, "", 40f, 0f, "", 0, 22f, 3.5f, 0, "败", false, 4f, "", 0, 0f, 0f, 0f, 0, "SkillAidPoisonArrow", "shoot", "", 0f, "", "", "", "");
            config[2020594] = new SkillConfig(2020594, "毒箭", "张任", "", "武", 4, 0f, 4f, 16, "", 1, "", 40f, 0f, "", 0, 30f, 3.8f, 0, "败", false, 4f, "", 0, 0f, 0f, 0f, 0, "SkillAidPoisonArrow", "shoot", "", 0f, "", "", "", "");
            config[2020595] = new SkillConfig(2020595, "毒箭", "张任", "", "武", 5, 0f, 4f, 16, "", 1, "", 40f, 0f, "", 0, 40f, 4.2f, 0, "败", false, 4f, "", 0, 0f, 0f, 0f, 0, "SkillAidPoisonArrow", "shoot", "", 0f, "", "", "", "");
            config[2020601] = new SkillConfig(2020601, "分兵", "关羽", "召唤/summoncount名影分身冲击敌人（继承自身/strength2攻击与生命），同时本体对目标造成/strength法术伤害", "术", 1, 0f, 6f, 18, "", 0, "", 60f, 25f, "", 1, 35f, 0.25f, 0, "", false, 0f, "", 1, 0f, 0f, 0f, 0, "SkillAidShadowSplit", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2020602] = new SkillConfig(2020602, "分兵", "关羽", "", "术", 2, 0f, 6f, 18, "", 0, "", 60f, 25f, "", 1, 55f, 0.30f, 0, "", false, 0f, "", 1, 0f, 0f, 0f, 0, "SkillAidShadowSplit", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2020603] = new SkillConfig(2020603, "分兵", "关羽", "", "术", 3, 0f, 6f, 18, "", 0, "", 60f, 25f, "", 1, 80f, 0.35f, 0, "", false, 0f, "", 1, 0f, 0f, 0f, 0, "SkillAidShadowSplit", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2020604] = new SkillConfig(2020604, "分兵", "关羽", "", "术", 4, 0f, 6f, 18, "", 0, "", 60f, 25f, "", 1, 110f, 0.40f, 0, "", false, 0f, "", 1, 0f, 0f, 0f, 0, "SkillAidShadowSplit", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2020605] = new SkillConfig(2020605, "分兵", "关羽", "", "术", 5, 0f, 6f, 18, "", 0, "", 60f, 25f, "", 1, 150f, 0.45f, 0, "", false, 0f, "", 1, 0f, 0f, 0f, 0, "SkillAidShadowSplit", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2020606] = new SkillConfig(2020606, "流光", "姜维", "对前方释放/targetcount道激光，每道对路径上的敌人造成/strength法术伤害", "术", 1, 0f, 5f, 18, "", 0, "", 60f, 20f, "", 2, 40f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "SkillAidDualLaser", "shoot", "MagicChargeYellow", 0f, "", "", "", "");
            config[2020607] = new SkillConfig(2020607, "流光", "姜维", "", "术", 2, 0f, 5f, 18, "", 0, "", 60f, 20f, "", 2, 65f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "SkillAidDualLaser", "shoot", "MagicChargeYellow", 0f, "", "", "", "");
            config[2020608] = new SkillConfig(2020608, "流光", "姜维", "", "术", 3, 0f, 5f, 18, "", 0, "", 60f, 20f, "", 2, 90f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "SkillAidDualLaser", "shoot", "MagicChargeYellow", 0f, "", "", "", "");
            config[2020609] = new SkillConfig(2020609, "流光", "姜维", "", "术", 4, 0f, 5f, 18, "", 0, "", 60f, 20f, "", 2, 125f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "SkillAidDualLaser", "shoot", "MagicChargeYellow", 0f, "", "", "", "");
            config[2020610] = new SkillConfig(2020610, "流光", "姜维", "", "术", 5, 0f, 5f, 18, "", 0, "", 60f, 20f, "", 2, 170f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "SkillAidDualLaser", "shoot", "MagicChargeYellow", 0f, "", "", "", "");
            config[2020611] = new SkillConfig(2020611, "歼灭", "夏侯惇", "对目标造成/strength法术伤害并眩晕/bufftime秒", "术", 1, 0f, 6f, 14, "", 0, "", 50f, 0f, "", 0, 40f, 0f, 0, "乱", true, 1.2f, "", 0, 0f, 0f, 0f, 0, "SkillAidSmashStun", "sway", "MagicNovaBlue", 0f, "", "", "", "");
            config[2020612] = new SkillConfig(2020612, "歼灭", "夏侯惇", "", "术", 2, 0f, 6f, 14, "", 0, "", 50f, 0f, "", 0, 60f, 0f, 0, "乱", true, 1.4f, "", 0, 0f, 0f, 0f, 0, "SkillAidSmashStun", "sway", "MagicNovaBlue", 0f, "", "", "", "");
            config[2020613] = new SkillConfig(2020613, "歼灭", "夏侯惇", "", "术", 3, 0f, 6f, 14, "", 0, "", 50f, 0f, "", 0, 85f, 0f, 0, "乱", true, 1.6f, "", 0, 0f, 0f, 0f, 0, "SkillAidSmashStun", "sway", "MagicNovaBlue", 0f, "", "", "", "");
            config[2020614] = new SkillConfig(2020614, "歼灭", "夏侯惇", "", "术", 4, 0f, 6f, 14, "", 0, "", 50f, 0f, "", 0, 115f, 0f, 0, "乱", true, 1.8f, "", 0, 0f, 0f, 0f, 0, "SkillAidSmashStun", "sway", "MagicNovaBlue", 0f, "", "", "", "");
            config[2020615] = new SkillConfig(2020615, "歼灭", "夏侯惇", "", "术", 5, 0f, 6f, 14, "", 0, "", 50f, 0f, "", 0, 155f, 0f, 0, "乱", true, 2.0f, "", 0, 0f, 0f, 0f, 0, "SkillAidSmashStun", "sway", "MagicNovaBlue", 0f, "", "", "", "");
            config[2020616] = new SkillConfig(2020616, "御斩", "乐进", "对目标造成/strength法术伤害，并为自身附加防御提升，持续/bufftime秒", "术", 1, 0f, 5f, 14, "", 0, "", 30f, 0f, "", 0, 40f, 0f, 0, "硬", false, 4f, "", 0, 0f, 0f, 0f, 0, "SkillAidSlashDefend", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2020617] = new SkillConfig(2020617, "御斩", "乐进", "", "术", 2, 0f, 5f, 14, "", 0, "", 30f, 0f, "", 0, 60f, 0f, 0, "硬", false, 4f, "", 0, 0f, 0f, 0f, 0, "SkillAidSlashDefend", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2020618] = new SkillConfig(2020618, "御斩", "乐进", "", "术", 3, 0f, 5f, 14, "", 0, "", 30f, 0f, "", 0, 85f, 0f, 0, "硬", false, 5f, "", 0, 0f, 0f, 0f, 0, "SkillAidSlashDefend", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2020619] = new SkillConfig(2020619, "御斩", "乐进", "", "术", 4, 0f, 5f, 14, "", 0, "", 30f, 0f, "", 0, 115f, 0f, 0, "硬", false, 5f, "", 0, 0f, 0f, 0f, 0, "SkillAidSlashDefend", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2020620] = new SkillConfig(2020620, "御斩", "乐进", "", "术", 5, 0f, 5f, 14, "", 0, "", 30f, 0f, "", 0, 155f, 0f, 0, "硬", false, 5f, "", 0, 0f, 0f, 0f, 0, "SkillAidSlashDefend", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2020621] = new SkillConfig(2020621, "横扫", "程普", "挺戟横扫，对/area范围内的敌人造成/strength法术伤害，并获得/strength2%最大生命的护盾", "术", 1, 0f, 5f, 14, "", 0, "", 0f, 20f, "", 3, 28f, 0.15f, 0, "盾", false, 6f, "", 0, 0f, 0f, 0f, 0, "SkillAidBashShield", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2020622] = new SkillConfig(2020622, "横扫", "程普", "", "术", 2, 0f, 5f, 14, "", 0, "", 0f, 20f, "", 3, 42f, 0.20f, 0, "盾", false, 6f, "", 0, 0f, 0f, 0f, 0, "SkillAidBashShield", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2020623] = new SkillConfig(2020623, "横扫", "程普", "", "术", 3, 0f, 5f, 14, "", 0, "", 0f, 20f, "", 3, 62f, 0.25f, 0, "盾", false, 6f, "", 0, 0f, 0f, 0f, 0, "SkillAidBashShield", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2020624] = new SkillConfig(2020624, "横扫", "程普", "", "术", 4, 0f, 5f, 14, "", 0, "", 0f, 20f, "", 3, 88f, 0.30f, 0, "盾", false, 6f, "", 0, 0f, 0f, 0f, 0, "SkillAidBashShield", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2020625] = new SkillConfig(2020625, "横扫", "程普", "", "术", 5, 0f, 5f, 14, "", 0, "", 0f, 20f, "", 3, 120f, 0.35f, 0, "盾", false, 6f, "", 0, 0f, 0f, 0f, 0, "SkillAidBashShield", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2020626] = new SkillConfig(2020626, "斩杀", "颜良", "对目标造成/strength法术伤害；若目标生命低于/strengthint%，伤害提升至/strength2倍", "术", 1, 0f, 6f, 18, "", 0, "", 50f, 0f, "", 0, 45f, 1.5f, 30, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "SkillAidExecute", "sway", "MagicNovaBlue", 0f, "", "", "", "");
            config[2020627] = new SkillConfig(2020627, "斩杀", "颜良", "", "术", 2, 0f, 6f, 18, "", 0, "", 50f, 0f, "", 0, 70f, 1.6f, 30, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "SkillAidExecute", "sway", "MagicNovaBlue", 0f, "", "", "", "");
            config[2020628] = new SkillConfig(2020628, "斩杀", "颜良", "", "术", 3, 0f, 6f, 18, "", 0, "", 50f, 0f, "", 0, 100f, 1.7f, 30, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "SkillAidExecute", "sway", "MagicNovaBlue", 0f, "", "", "", "");
            config[2020629] = new SkillConfig(2020629, "斩杀", "颜良", "", "术", 4, 0f, 6f, 18, "", 0, "", 50f, 0f, "", 0, 135f, 1.8f, 30, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "SkillAidExecute", "sway", "MagicNovaBlue", 0f, "", "", "", "");
            config[2020630] = new SkillConfig(2020630, "斩杀", "颜良", "", "术", 5, 0f, 6f, 18, "", 0, "", 50f, 0f, "", 0, 180f, 2.0f, 30, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "SkillAidExecute", "sway", "MagicNovaBlue", 0f, "", "", "", "");
            config[2020631] = new SkillConfig(2020631, "追猎", "马超", "疯狂追击目标/summoncount至/strengthint次，每次造成/strength法术伤害，次数随机", "术", 1, 0f, 5f, 16, "", 0, "", 60f, 0f, "", 0, 15f, 0f, 4, "", false, 0f, "", 2, 0f, 0f, 0f, 0, "SkillAidRandomCombo", "sway", "MagicNovaBlue", 0f, "", "", "", "");
            config[2020632] = new SkillConfig(2020632, "追猎", "马超", "", "术", 2, 0f, 5f, 16, "", 0, "", 60f, 0f, "", 0, 20f, 0f, 4, "", false, 0f, "", 2, 0f, 0f, 0f, 0, "SkillAidRandomCombo", "sway", "MagicNovaBlue", 0f, "", "", "", "");
            config[2020633] = new SkillConfig(2020633, "追猎", "马超", "", "术", 3, 0f, 5f, 16, "", 0, "", 60f, 0f, "", 0, 25f, 0f, 5, "", false, 0f, "", 2, 0f, 0f, 0f, 0, "SkillAidRandomCombo", "sway", "MagicNovaBlue", 0f, "", "", "", "");
            config[2020634] = new SkillConfig(2020634, "追猎", "马超", "", "术", 4, 0f, 5f, 16, "", 0, "", 60f, 0f, "", 0, 30f, 0f, 5, "", false, 0f, "", 2, 0f, 0f, 0f, 0, "SkillAidRandomCombo", "sway", "MagicNovaBlue", 0f, "", "", "", "");
            config[2020635] = new SkillConfig(2020635, "追猎", "马超", "", "术", 5, 0f, 5f, 16, "", 0, "", 60f, 0f, "", 0, 40f, 0f, 6, "", false, 0f, "", 3, 0f, 0f, 0f, 0, "SkillAidRandomCombo", "sway", "MagicNovaBlue", 0f, "", "", "", "");
            config[2020636] = new SkillConfig(2020636, "疾攻", "曹彰", "对目标造成/strength法术伤害，并获得攻速与移速提升，持续/bufftime秒", "术", 1, 0f, 4f, 14, "", 0, "", 50f, 0f, "", 0, 30f, 0f, 0, "翼", false, 3f, "", 0, 0f, 0f, 0f, 0, "SkillAidHasteStrike", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2020637] = new SkillConfig(2020637, "疾攻", "曹彰", "", "术", 2, 0f, 4f, 14, "", 0, "", 50f, 0f, "", 0, 45f, 0f, 0, "翼", false, 4f, "", 0, 0f, 0f, 0f, 0, "SkillAidHasteStrike", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2020638] = new SkillConfig(2020638, "疾攻", "曹彰", "", "术", 3, 0f, 4f, 14, "", 0, "", 50f, 0f, "", 0, 65f, 0f, 0, "翼", false, 4f, "", 0, 0f, 0f, 0f, 0, "SkillAidHasteStrike", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2020639] = new SkillConfig(2020639, "疾攻", "曹彰", "", "术", 4, 0f, 4f, 14, "", 0, "", 50f, 0f, "", 0, 90f, 0f, 0, "翼", false, 5f, "", 0, 0f, 0f, 0f, 0, "SkillAidHasteStrike", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2020640] = new SkillConfig(2020640, "疾攻", "曹彰", "", "术", 5, 0f, 4f, 14, "", 0, "", 50f, 0f, "", 0, 120f, 0f, 0, "翼", false, 5f, "", 0, 0f, 0f, 0f, 0, "SkillAidHasteStrike", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2020641] = new SkillConfig(2020641, "标记", "朱桓", "对目标造成/strength法术伤害，并使其受到的伤害提升/strength2%，持续/bufftime秒", "术", 1, 0f, 5f, 14, "", 0, "", 50f, 0f, "", 0, 25f, 0.15f, 0, "伤", true, 4f, "", 0, 0f, 0f, 0f, 0, "SkillAidMarkTarget", "sway", "MagicNovaBlue", 0f, "", "", "", "");
            config[2020642] = new SkillConfig(2020642, "标记", "朱桓", "", "术", 2, 0f, 5f, 14, "", 0, "", 50f, 0f, "", 0, 40f, 0.20f, 0, "伤", true, 4f, "", 0, 0f, 0f, 0f, 0, "SkillAidMarkTarget", "sway", "MagicNovaBlue", 0f, "", "", "", "");
            config[2020643] = new SkillConfig(2020643, "标记", "朱桓", "", "术", 3, 0f, 5f, 14, "", 0, "", 50f, 0f, "", 0, 58f, 0.25f, 0, "伤", true, 5f, "", 0, 0f, 0f, 0f, 0, "SkillAidMarkTarget", "sway", "MagicNovaBlue", 0f, "", "", "", "");
            config[2020644] = new SkillConfig(2020644, "标记", "朱桓", "", "术", 4, 0f, 5f, 14, "", 0, "", 50f, 0f, "", 0, 82f, 0.30f, 0, "伤", true, 5f, "", 0, 0f, 0f, 0f, 0, "SkillAidMarkTarget", "sway", "MagicNovaBlue", 0f, "", "", "", "");
            config[2020645] = new SkillConfig(2020645, "标记", "朱桓", "", "术", 5, 0f, 5f, 14, "", 0, "", 50f, 0f, "", 0, 112f, 0.35f, 0, "伤", true, 5f, "", 0, 0f, 0f, 0f, 0, "SkillAidMarkTarget", "sway", "MagicNovaBlue", 0f, "", "", "", "");
            config[2020646] = new SkillConfig(2020646, "双射", "韩当", "对目标及附近/targetcount-1名敌人各造成/strength法术伤害", "术", 1, 0f, 5f, 14, "", 0, "", 60f, 25f, "", 2, 32f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "SkillAidDoubleShot", "shoot", "", 0f, "", "", "", "");
            config[2020647] = new SkillConfig(2020647, "双射", "韩当", "", "术", 2, 0f, 5f, 14, "", 0, "", 60f, 25f, "", 2, 50f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "SkillAidDoubleShot", "shoot", "", 0f, "", "", "", "");
            config[2020648] = new SkillConfig(2020648, "双射", "韩当", "", "术", 3, 0f, 5f, 14, "", 0, "", 60f, 25f, "", 2, 72f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "SkillAidDoubleShot", "shoot", "", 0f, "", "", "", "");
            config[2020649] = new SkillConfig(2020649, "双射", "韩当", "", "术", 4, 0f, 5f, 14, "", 0, "", 60f, 25f, "", 2, 100f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "SkillAidDoubleShot", "shoot", "", 0f, "", "", "", "");
            config[2020650] = new SkillConfig(2020650, "双射", "韩当", "", "术", 5, 0f, 5f, 14, "", 0, "", 60f, 25f, "", 2, 135f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "SkillAidDoubleShot", "shoot", "", 0f, "", "", "", "");
            config[2020651] = new SkillConfig(2020651, "狂暴", "华雄", "进入狂暴状态，造成的伤害提升/strength2%，但同时受到的伤害提升/strength2%，持续/bufftime秒", "术", 1, 0f, 6f, 14, "", 0, "", 0f, 0f, "", 0, 0f, 0.20f, 0, "狂", false, 5f, "", 0, 0f, 0f, 0f, 0, "SkillAidBerserk", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2020652] = new SkillConfig(2020652, "狂暴", "华雄", "", "术", 2, 0f, 6f, 14, "", 0, "", 0f, 0f, "", 0, 0f, 0.25f, 0, "狂", false, 6f, "", 0, 0f, 0f, 0f, 0, "SkillAidBerserk", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2020653] = new SkillConfig(2020653, "狂暴", "华雄", "", "术", 3, 0f, 6f, 14, "", 0, "", 0f, 0f, "", 0, 0f, 0.30f, 0, "狂", false, 6f, "", 0, 0f, 0f, 0f, 0, "SkillAidBerserk", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2020654] = new SkillConfig(2020654, "狂暴", "华雄", "", "术", 4, 0f, 6f, 14, "", 0, "", 0f, 0f, "", 0, 0f, 0.35f, 0, "狂", false, 7f, "", 0, 0f, 0f, 0f, 0, "SkillAidBerserk", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2020655] = new SkillConfig(2020655, "狂暴", "华雄", "", "术", 5, 0f, 6f, 14, "", 0, "", 0f, 0f, "", 0, 0f, 0.40f, 0, "狂", false, 7f, "", 0, 0f, 0f, 0f, 0, "SkillAidBerserk", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2020656] = new SkillConfig(2020656, "换位", "陈泰", "与目标交换位置，并对其造成/strength法术伤害", "术", 1, 0f, 6f, 25, "", 0, "", 50f, 0f, "", 0, 50f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "SkillAidSwapStrike", "sway", "MagicNovaBlue", 0f, "", "", "", "");
            config[2020657] = new SkillConfig(2020657, "换位", "陈泰", "", "术", 2, 0f, 6f, 25, "", 0, "", 50f, 0f, "", 0, 80f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "SkillAidSwapStrike", "sway", "MagicNovaBlue", 0f, "", "", "", "");
            config[2020658] = new SkillConfig(2020658, "换位", "陈泰", "", "术", 3, 0f, 6f, 25, "", 0, "", 50f, 0f, "", 0, 115f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "SkillAidSwapStrike", "sway", "MagicNovaBlue", 0f, "", "", "", "");
            config[2020659] = new SkillConfig(2020659, "换位", "陈泰", "", "术", 4, 0f, 6f, 25, "", 0, "", 50f, 0f, "", 0, 155f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "SkillAidSwapStrike", "sway", "MagicNovaBlue", 0f, "", "", "", "");
            config[2020660] = new SkillConfig(2020660, "换位", "陈泰", "", "术", 5, 0f, 6f, 25, "", 0, "", 50f, 0f, "", 0, 205f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "SkillAidSwapStrike", "sway", "MagicNovaBlue", 0f, "", "", "", "");
            config[2020701] = new SkillConfig(2020701, "血战", "曹洪", "对目标造成/strength法术伤害，自身生命越低伤害越高，并对带护盾目标额外造成/strengthint%破盾伤害", "术", 1, 0f, 6f, 14, "", 0, "", 50f, 0f, "", 0, 40f, 0.5f, 40, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "SkillAidBloodyWar", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2020702] = new SkillConfig(2020702, "血战", "曹洪", "", "术", 2, 0f, 6f, 14, "", 0, "", 50f, 0f, "", 0, 60f, 0.6f, 50, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "SkillAidBloodyWar", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2020703] = new SkillConfig(2020703, "血战", "曹洪", "", "术", 3, 0f, 6f, 14, "", 0, "", 50f, 0f, "", 0, 85f, 0.7f, 60, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "SkillAidBloodyWar", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2020704] = new SkillConfig(2020704, "血战", "曹洪", "", "术", 4, 0f, 6f, 14, "", 0, "", 50f, 0f, "", 0, 115f, 0.8f, 70, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "SkillAidBloodyWar", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2020705] = new SkillConfig(2020705, "血战", "曹洪", "", "术", 5, 0f, 6f, 14, "", 0, "", 50f, 0f, "", 0, 155f, 1.0f, 80, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "SkillAidBloodyWar", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2020706] = new SkillConfig(2020706, "不屈", "周泰", "为自身附加/strength%最大生命的护盾，并回复/strength2%最大生命，持续/bufftime秒", "术", 1, 0f, 6f, 14, "", 0, "", 0f, 0f, "", 0, 0.15f, 0.10f, 0, "盾", false, 4f, "", 0, 0f, 0f, 0f, 0, "SkillAidUnbreakable", "sway", "ShieldSoftBlue", 0f, "", "", "", "");
            config[2020707] = new SkillConfig(2020707, "不屈", "周泰", "", "术", 2, 0f, 6f, 14, "", 0, "", 0f, 0f, "", 0, 0.18f, 0.13f, 0, "盾", false, 4f, "", 0, 0f, 0f, 0f, 0, "SkillAidUnbreakable", "sway", "ShieldSoftBlue", 0f, "", "", "", "");
            config[2020708] = new SkillConfig(2020708, "不屈", "周泰", "", "术", 3, 0f, 6f, 14, "", 0, "", 0f, 0f, "", 0, 0.22f, 0.16f, 0, "盾", false, 4f, "", 0, 0f, 0f, 0f, 0, "SkillAidUnbreakable", "sway", "ShieldSoftBlue", 0f, "", "", "", "");
            config[2020709] = new SkillConfig(2020709, "不屈", "周泰", "", "术", 4, 0f, 6f, 14, "", 0, "", 0f, 0f, "", 0, 0.26f, 0.20f, 0, "盾", false, 5f, "", 0, 0f, 0f, 0f, 0, "SkillAidUnbreakable", "sway", "ShieldSoftBlue", 0f, "", "", "", "");
            config[2020710] = new SkillConfig(2020710, "不屈", "周泰", "", "术", 5, 0f, 6f, 14, "", 0, "", 0f, 0f, "", 0, 0.30f, 0.25f, 0, "盾", false, 5f, "", 0, 0f, 0f, 0f, 0, "SkillAidUnbreakable", "sway", "ShieldSoftBlue", 0f, "", "", "", "");
            config[2020711] = new SkillConfig(2020711, "偷刃", "潘璋", "攻击无视目标护甲造成/strength法术伤害，并降低其/strength2护甲，持续/bufftime秒", "术", 1, 0f, 4f, 12, "", 0, "", 50f, 0f, "", 0, 30f, 8f, 0, "破", true, 3f, "", 0, 0f, 0f, 0f, 0, "SkillAidStealBlade", "sway", "MagicNovaBlue", 0f, "", "", "", "");
            config[2020712] = new SkillConfig(2020712, "偷刃", "潘璋", "", "术", 2, 0f, 4f, 12, "", 0, "", 50f, 0f, "", 0, 45f, 10f, 0, "破", true, 3f, "", 0, 0f, 0f, 0f, 0, "SkillAidStealBlade", "sway", "MagicNovaBlue", 0f, "", "", "", "");
            config[2020713] = new SkillConfig(2020713, "偷刃", "潘璋", "", "术", 3, 0f, 4f, 12, "", 0, "", 50f, 0f, "", 0, 62f, 14f, 0, "破", true, 3f, "", 0, 0f, 0f, 0f, 0, "SkillAidStealBlade", "sway", "MagicNovaBlue", 0f, "", "", "", "");
            config[2020714] = new SkillConfig(2020714, "偷刃", "潘璋", "", "术", 4, 0f, 4f, 12, "", 0, "", 50f, 0f, "", 0, 85f, 20f, 0, "破", true, 3f, "", 0, 0f, 0f, 0f, 0, "SkillAidStealBlade", "sway", "MagicNovaBlue", 0f, "", "", "", "");
            config[2020715] = new SkillConfig(2020715, "偷刃", "潘璋", "", "术", 5, 0f, 4f, 12, "", 0, "", 50f, 0f, "", 0, 115f, 28f, 0, "破", true, 3f, "", 0, 0f, 0f, 0f, 0, "SkillAidStealBlade", "sway", "MagicNovaBlue", 0f, "", "", "", "");
            config[2020716] = new SkillConfig(2020716, "流星锤", "王双", "掷出流星锤，对目标及周围最多/targetcount名敌人造成/strength法术伤害（无视护甲），对带护盾敌人额外造成/strength2倍破盾伤害", "术", 1, 0f, 4f, 14, "", 0, "", 60f, 20f, "", 3, 25f, 0.8f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "SkillAidMeteorHammer", "sway", "MagicNovaBlue", 0f, "", "", "", "");
            config[2020717] = new SkillConfig(2020717, "流星锤", "王双", "", "术", 2, 0f, 4f, 14, "", 0, "", 60f, 20f, "", 3, 38f, 1.0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "SkillAidMeteorHammer", "sway", "MagicNovaBlue", 0f, "", "", "", "");
            config[2020718] = new SkillConfig(2020718, "流星锤", "王双", "", "术", 3, 0f, 4f, 14, "", 0, "", 60f, 20f, "", 3, 55f, 1.2f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "SkillAidMeteorHammer", "sway", "MagicNovaBlue", 0f, "", "", "", "");
            config[2020719] = new SkillConfig(2020719, "流星锤", "王双", "", "术", 4, 0f, 4f, 14, "", 0, "", 60f, 20f, "", 3, 78f, 1.4f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "SkillAidMeteorHammer", "sway", "MagicNovaBlue", 0f, "", "", "", "");
            config[2020720] = new SkillConfig(2020720, "流星锤", "王双", "", "术", 5, 0f, 4f, 14, "", 0, "", 60f, 20f, "", 3, 108f, 1.6f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "SkillAidMeteorHammer", "sway", "MagicNovaBlue", 0f, "", "", "", "");
            config[2020721] = new SkillConfig(2020721, "蛮锤", "孟获", "纵锤砸地，对/area范围内敌人造成/strength法术伤害并移除其护盾", "术", 1, 0f, 5f, 16, "", 0, "", 0f, 25f, "", 0, 30f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "SkillAidBarbarianSlam", "sway", "MagicNovaBlue", 0f, "", "", "", "");
            config[2020722] = new SkillConfig(2020722, "蛮锤", "孟获", "", "术", 2, 0f, 5f, 16, "", 0, "", 0f, 25f, "", 0, 45f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "SkillAidBarbarianSlam", "sway", "MagicNovaBlue", 0f, "", "", "", "");
            config[2020723] = new SkillConfig(2020723, "蛮锤", "孟获", "", "术", 3, 0f, 5f, 16, "", 0, "", 0f, 25f, "", 0, 65f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "SkillAidBarbarianSlam", "sway", "MagicNovaBlue", 0f, "", "", "", "");
            config[2020724] = new SkillConfig(2020724, "蛮锤", "孟获", "", "术", 4, 0f, 5f, 16, "", 0, "", 0f, 25f, "", 0, 90f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "SkillAidBarbarianSlam", "sway", "MagicNovaBlue", 0f, "", "", "", "");
            config[2020725] = new SkillConfig(2020725, "蛮锤", "孟获", "", "术", 5, 0f, 5f, 16, "", 0, "", 0f, 25f, "", 0, 125f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "SkillAidBarbarianSlam", "sway", "MagicNovaBlue", 0f, "", "", "", "");
            config[2020726] = new SkillConfig(2020726, "七进七出", "赵云", "七进七出，对目标及周围最多/targetcount名敌人各造成/strength法术伤害，并为自己附加/strength2%最大生命护盾，持续/bufftime秒", "术", 1, 0f, 6f, 18, "", 0, "", 60f, 25f, "", 3, 30f, 0.20f, 0, "盾", false, 5f, "", 0, 0f, 0f, 0f, 0, "SkillAidSevenCharge", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2020727] = new SkillConfig(2020727, "七进七出", "赵云", "", "术", 2, 0f, 6f, 18, "", 0, "", 60f, 25f, "", 3, 45f, 0.25f, 0, "盾", false, 5f, "", 0, 0f, 0f, 0f, 0, "SkillAidSevenCharge", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2020728] = new SkillConfig(2020728, "七进七出", "赵云", "", "术", 3, 0f, 6f, 18, "", 0, "", 60f, 25f, "", 3, 65f, 0.30f, 0, "盾", false, 5f, "", 0, 0f, 0f, 0f, 0, "SkillAidSevenCharge", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2020729] = new SkillConfig(2020729, "七进七出", "赵云", "", "术", 4, 0f, 6f, 18, "", 0, "", 60f, 25f, "", 3, 90f, 0.35f, 0, "盾", false, 5f, "", 0, 0f, 0f, 0f, 0, "SkillAidSevenCharge", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2020730] = new SkillConfig(2020730, "七进七出", "赵云", "", "术", 5, 0f, 6f, 18, "", 0, "", 60f, 25f, "", 3, 125f, 0.40f, 0, "盾", false, 5f, "", 0, 0f, 0f, 0f, 0, "SkillAidSevenCharge", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2020731] = new SkillConfig(2020731, "护军", "李严", "为生命最低的友军套/strength%最大生命护盾，并嘲讽其/area范围内敌人转移到自己身上", "术", 1, 0f, 6f, 14, "", 0, "", 0f, 25f, "", 0, 0.20f, 0f, 0, "盾", false, 6f, "", 0, 0f, 0f, 0f, 0, "SkillAidGuardArmy", "sway", "ShieldSoftBlue", 0f, "", "", "", "");
            config[2020732] = new SkillConfig(2020732, "护军", "李严", "", "术", 2, 0f, 6f, 14, "", 0, "", 0f, 25f, "", 0, 0.25f, 0f, 0, "盾", false, 6f, "", 0, 0f, 0f, 0f, 0, "SkillAidGuardArmy", "sway", "ShieldSoftBlue", 0f, "", "", "", "");
            config[2020733] = new SkillConfig(2020733, "护军", "李严", "", "术", 3, 0f, 6f, 14, "", 0, "", 0f, 25f, "", 0, 0.30f, 0f, 0, "盾", false, 6f, "", 0, 0f, 0f, 0f, 0, "SkillAidGuardArmy", "sway", "ShieldSoftBlue", 0f, "", "", "", "");
            config[2020734] = new SkillConfig(2020734, "护军", "李严", "", "术", 4, 0f, 6f, 14, "", 0, "", 0f, 25f, "", 0, 0.35f, 0f, 0, "盾", false, 6f, "", 0, 0f, 0f, 0f, 0, "SkillAidGuardArmy", "sway", "ShieldSoftBlue", 0f, "", "", "", "");
            config[2020735] = new SkillConfig(2020735, "护军", "李严", "", "术", 5, 0f, 6f, 14, "", 0, "", 0f, 25f, "", 0, 0.40f, 0f, 0, "盾", false, 6f, "", 0, 0f, 0f, 0f, 0, "SkillAidGuardArmy", "sway", "ShieldSoftBlue", 0f, "", "", "", "");
            config[2020736] = new SkillConfig(2020736, "抬棺", "庞德", "怒喝嘲讽/area范围内敌人攻击自己，并猛击对其造成/strength法术伤害", "术", 1, 0f, 6f, 16, "", 0, "", 0f, 30f, "", 0, 35f, 0f, 0, "", false, 4f, "", 0, 0f, 0f, 0f, 0, "SkillAidTauntSlam", "sway", "MagicNovaBlue", 0f, "", "", "", "");
            config[2020737] = new SkillConfig(2020737, "抬棺", "庞德", "", "术", 2, 0f, 6f, 16, "", 0, "", 0f, 30f, "", 0, 55f, 0f, 0, "", false, 4f, "", 0, 0f, 0f, 0f, 0, "SkillAidTauntSlam", "sway", "MagicNovaBlue", 0f, "", "", "", "");
            config[2020738] = new SkillConfig(2020738, "抬棺", "庞德", "", "术", 3, 0f, 6f, 16, "", 0, "", 0f, 30f, "", 0, 80f, 0f, 0, "", false, 5f, "", 0, 0f, 0f, 0f, 0, "SkillAidTauntSlam", "sway", "MagicNovaBlue", 0f, "", "", "", "");
            config[2020739] = new SkillConfig(2020739, "抬棺", "庞德", "", "术", 4, 0f, 6f, 16, "", 0, "", 0f, 30f, "", 0, 110f, 0f, 0, "", false, 5f, "", 0, 0f, 0f, 0f, 0, "SkillAidTauntSlam", "sway", "MagicNovaBlue", 0f, "", "", "", "");
            config[2020740] = new SkillConfig(2020740, "抬棺", "庞德", "", "术", 5, 0f, 6f, 16, "", 0, "", 0f, 30f, "", 0, 150f, 0f, 0, "", false, 5f, "", 0, 0f, 0f, 0f, 0, "SkillAidTauntSlam", "sway", "MagicNovaBlue", 0f, "", "", "", "");
            config[2020741] = new SkillConfig(2020741, "武卫", "李典", "为自己附加/strength%最大生命护盾，护盾被击破时对/area范围敌人造成护盾容量×/strength2的爆裂伤害", "术", 1, 0f, 6f, 16, "", 0, "", 0f, 22f, "", 0, 0.25f, 0.6f, 0, "盾", false, 5f, "", 0, 0f, 0f, 0f, 0, "SkillAidShieldBurst", "sway", "ShieldSoftBlue", 0f, "", "", "", "");
            config[2020742] = new SkillConfig(2020742, "武卫", "李典", "", "术", 2, 0f, 6f, 16, "", 0, "", 0f, 22f, "", 0, 0.30f, 0.7f, 0, "盾", false, 5f, "", 0, 0f, 0f, 0f, 0, "SkillAidShieldBurst", "sway", "ShieldSoftBlue", 0f, "", "", "", "");
            config[2020743] = new SkillConfig(2020743, "武卫", "李典", "", "术", 3, 0f, 6f, 16, "", 0, "", 0f, 22f, "", 0, 0.35f, 0.8f, 0, "盾", false, 5f, "", 0, 0f, 0f, 0f, 0, "SkillAidShieldBurst", "sway", "ShieldSoftBlue", 0f, "", "", "", "");
            config[2020744] = new SkillConfig(2020744, "武卫", "李典", "", "术", 4, 0f, 6f, 16, "", 0, "", 0f, 22f, "", 0, 0.40f, 0.9f, 0, "盾", false, 5f, "", 0, 0f, 0f, 0f, 0, "SkillAidShieldBurst", "sway", "ShieldSoftBlue", 0f, "", "", "", "");
            config[2020745] = new SkillConfig(2020745, "武卫", "李典", "", "术", 5, 0f, 6f, 16, "", 0, "", 0f, 22f, "", 0, 0.45f, 1.0f, 0, "盾", false, 5f, "", 0, 0f, 0f, 0f, 0, "SkillAidShieldBurst", "sway", "ShieldSoftBlue", 0f, "", "", "", "");
            config[2020746] = new SkillConfig(2020746, "苦肉", "黄盖", "嘲讽/area范围内敌人攻击自己，并为所有友军附加/strength%最大生命的减伤盾，持续/bufftime秒", "术", 1, 0f, 7f, 16, "", 0, "", 0f, 30f, "", 0, 0.12f, 0f, 0, "硬", false, 4f, "", 0, 0f, 0f, 0f, 0, "SkillAidSelfSacrifice", "sway", "ShieldSoftBlue", 0f, "", "", "", "");
            config[2020747] = new SkillConfig(2020747, "苦肉", "黄盖", "", "术", 2, 0f, 7f, 16, "", 0, "", 0f, 30f, "", 0, 0.15f, 0f, 0, "硬", false, 4f, "", 0, 0f, 0f, 0f, 0, "SkillAidSelfSacrifice", "sway", "ShieldSoftBlue", 0f, "", "", "", "");
            config[2020748] = new SkillConfig(2020748, "苦肉", "黄盖", "", "术", 3, 0f, 7f, 16, "", 0, "", 0f, 30f, "", 0, 0.18f, 0f, 0, "硬", false, 5f, "", 0, 0f, 0f, 0f, 0, "SkillAidSelfSacrifice", "sway", "ShieldSoftBlue", 0f, "", "", "", "");
            config[2020749] = new SkillConfig(2020749, "苦肉", "黄盖", "", "术", 4, 0f, 7f, 16, "", 0, "", 0f, 30f, "", 0, 0.22f, 0f, 0, "硬", false, 5f, "", 0, 0f, 0f, 0f, 0, "SkillAidSelfSacrifice", "sway", "ShieldSoftBlue", 0f, "", "", "", "");
            config[2020750] = new SkillConfig(2020750, "苦肉", "黄盖", "", "术", 5, 0f, 7f, 16, "", 0, "", 0f, 30f, "", 0, 0.26f, 0f, 0, "硬", false, 5f, "", 0, 0f, 0f, 0f, 0, "SkillAidSelfSacrifice", "sway", "ShieldSoftBlue", 0f, "", "", "", "");
            config[2020751] = new SkillConfig(2020751, "妖术", "张梁", "与生命最低的友军建立生命链接，双方受到的伤害与回复按/strength2%共享，持续/bufftime秒", "术", 1, 0f, 6f, 14, "", 0, "", 0f, 0f, "", 0, 0f, 0.15f, 0, "链", false, 6f, "", 0, 0f, 0f, 0f, 0, "SkillAidLifeLink", "sway", "AuraSoftPurple", 0f, "", "", "", "");
            config[2020752] = new SkillConfig(2020752, "妖术", "张梁", "", "术", 2, 0f, 6f, 14, "", 0, "", 0f, 0f, "", 0, 0f, 0.20f, 0, "链", false, 6f, "", 0, 0f, 0f, 0f, 0, "SkillAidLifeLink", "sway", "AuraSoftPurple", 0f, "", "", "", "");
            config[2020753] = new SkillConfig(2020753, "妖术", "张梁", "", "术", 3, 0f, 6f, 14, "", 0, "", 0f, 0f, "", 0, 0f, 0.25f, 0, "链", false, 7f, "", 0, 0f, 0f, 0f, 0, "SkillAidLifeLink", "sway", "AuraSoftPurple", 0f, "", "", "", "");
            config[2020754] = new SkillConfig(2020754, "妖术", "张梁", "", "术", 4, 0f, 6f, 14, "", 0, "", 0f, 0f, "", 0, 0f, 0.30f, 0, "链", false, 7f, "", 0, 0f, 0f, 0f, 0, "SkillAidLifeLink", "sway", "AuraSoftPurple", 0f, "", "", "", "");
            config[2020755] = new SkillConfig(2020755, "妖术", "张梁", "", "术", 5, 0f, 6f, 14, "", 0, "", 0f, 0f, "", 0, 0f, 0.35f, 0, "链", false, 8f, "", 0, 0f, 0f, 0f, 0, "SkillAidLifeLink", "sway", "AuraSoftPurple", 0f, "", "", "", "");
            config[2020801] = new SkillConfig(2020801, "明断", "法正", "召雷降电，对/area范围持续降下雷阵，每秒对其中敌人造成/strength法术伤害，持续/bufftime秒", "术", 1, 0f, 7f, 16, "", 0, "", 40f, 25f, "", 0, 20f, 0f, 0, "", false, 3f, "", 1, 3f, 1f, 0f, 0, "SkillAidJudgement", "sway", "MagicChargeYellow", 30f, "", "", "", "");
            config[2020802] = new SkillConfig(2020802, "明断", "法正", "", "术", 2, 0f, 7f, 16, "", 0, "", 40f, 25f, "", 0, 30f, 0f, 0, "", false, 3f, "", 1, 3f, 1f, 0f, 0, "SkillAidJudgement", "sway", "MagicChargeYellow", 30f, "", "", "", "");
            config[2020803] = new SkillConfig(2020803, "明断", "法正", "", "术", 3, 0f, 7f, 16, "", 0, "", 40f, 25f, "", 0, 45f, 0f, 0, "", false, 4f, "", 1, 4f, 1f, 0f, 0, "SkillAidJudgement", "sway", "MagicChargeYellow", 30f, "", "", "", "");
            config[2020804] = new SkillConfig(2020804, "明断", "法正", "", "术", 4, 0f, 7f, 16, "", 0, "", 40f, 25f, "", 0, 62f, 0f, 0, "", false, 4f, "", 1, 4f, 1f, 0f, 0, "SkillAidJudgement", "sway", "MagicChargeYellow", 30f, "", "", "", "");
            config[2020805] = new SkillConfig(2020805, "明断", "法正", "", "术", 5, 0f, 7f, 16, "", 0, "", 40f, 25f, "", 0, 85f, 0f, 0, "", false, 5f, "", 1, 5f, 1f, 0f, 0, "SkillAidJudgement", "sway", "MagicChargeYellow", 30f, "", "", "", "");
            config[2020806] = new SkillConfig(2020806, "遗计", "郭嘉", "释放穿透激光，每秒对路径上所有敌人造成/strength法术伤害，持续/bufftime秒", "术", 1, 0f, 12f, 24, "", 0, "", 40f, 20f, "", 0, 30f, 0f, 0, "", false, 10f, "", 0, 0f, 0f, 0f, 0, "SkillAidLastStrategy", "sway", "AuraSoftPurple", 0f, "", "", "", "");
            config[2020807] = new SkillConfig(2020807, "遗计", "郭嘉", "", "术", 2, 0f, 12f, 24, "", 0, "", 40f, 20f, "", 0, 45f, 0f, 0, "", false, 10f, "", 0, 0f, 0f, 0f, 0, "SkillAidLastStrategy", "sway", "AuraSoftPurple", 0f, "", "", "", "");
            config[2020808] = new SkillConfig(2020808, "遗计", "郭嘉", "", "术", 3, 0f, 12f, 24, "", 0, "", 40f, 20f, "", 0, 65f, 0f, 0, "", false, 10f, "", 0, 0f, 0f, 0f, 0, "SkillAidLastStrategy", "sway", "AuraSoftPurple", 0f, "", "", "", "");
            config[2020809] = new SkillConfig(2020809, "遗计", "郭嘉", "", "术", 4, 0f, 12f, 24, "", 0, "", 40f, 20f, "", 0, 90f, 0f, 0, "", false, 10f, "", 0, 0f, 0f, 0f, 0, "SkillAidLastStrategy", "sway", "AuraSoftPurple", 0f, "", "", "", "");
            config[2020810] = new SkillConfig(2020810, "遗计", "郭嘉", "", "术", 5, 0f, 12f, 24, "", 0, "", 40f, 20f, "", 0, 125f, 0f, 0, "", false, 10f, "", 0, 0f, 0f, 0f, 0, "SkillAidLastStrategy", "sway", "AuraSoftPurple", 0f, "", "", "", "");
            config[2020811] = new SkillConfig(2020811, "连环", "荀攸", "连环计，对目标连续造成最多/targetcount段/strength法术伤害并减速，持续/bufftime秒", "术", 1, 0f, 5f, 14, "", 0, "", 40f, 0f, "", 3, 28f, 0.4f, 0, "缓", false, 3f, "", 0, 0f, 0f, 0f, 0, "SkillAidSweeping", "sway", "SoftFireBigRed", 0f, "", "", "", "");
            config[2020812] = new SkillConfig(2020812, "连环", "荀攸", "", "术", 2, 0f, 5f, 14, "", 0, "", 40f, 0f, "", 3, 42f, 0.4f, 0, "缓", false, 3f, "", 0, 0f, 0f, 0f, 0, "SkillAidSweeping", "sway", "SoftFireBigRed", 0f, "", "", "", "");
            config[2020813] = new SkillConfig(2020813, "连环", "荀攸", "", "术", 3, 0f, 5f, 14, "", 0, "", 40f, 0f, "", 3, 62f, 0.4f, 0, "缓", false, 4f, "", 0, 0f, 0f, 0f, 0, "SkillAidSweeping", "sway", "SoftFireBigRed", 0f, "", "", "", "");
            config[2020814] = new SkillConfig(2020814, "连环", "荀攸", "", "术", 4, 0f, 5f, 14, "", 0, "", 40f, 0f, "", 3, 88f, 0.4f, 0, "缓", false, 4f, "", 0, 0f, 0f, 0f, 0, "SkillAidSweeping", "sway", "SoftFireBigRed", 0f, "", "", "", "");
            config[2020815] = new SkillConfig(2020815, "连环", "荀攸", "", "术", 5, 0f, 5f, 14, "", 0, "", 40f, 0f, "", 3, 122f, 0.4f, 0, "缓", false, 5f, "", 0, 0f, 0f, 0f, 0, "SkillAidSweeping", "sway", "SoftFireBigRed", 0f, "", "", "", "");
            config[2020816] = new SkillConfig(2020816, "暗计", "陈宫", "暗布连环，对/area范围内敌人造成/strength法术伤害并降低其治疗/strengthint%，持续/bufftime秒", "术", 1, 0f, 6f, 14, "", 0, "", 40f, 24f, "", 0, 22f, 0f, 15, "疫", false, 4f, "", 0, 0f, 0f, 0f, 0, "SkillAidQuietPlot", "sway", "MagicNovaBlue", 0f, "", "", "", "");
            config[2020817] = new SkillConfig(2020817, "暗计", "陈宫", "", "术", 2, 0f, 6f, 14, "", 0, "", 40f, 24f, "", 0, 34f, 0f, 18, "疫", false, 4f, "", 0, 0f, 0f, 0f, 0, "SkillAidQuietPlot", "sway", "MagicNovaBlue", 0f, "", "", "", "");
            config[2020818] = new SkillConfig(2020818, "暗计", "陈宫", "", "术", 3, 0f, 6f, 14, "", 0, "", 40f, 24f, "", 0, 50f, 0f, 22, "疫", false, 5f, "", 0, 0f, 0f, 0f, 0, "SkillAidQuietPlot", "sway", "MagicNovaBlue", 0f, "", "", "", "");
            config[2020819] = new SkillConfig(2020819, "暗计", "陈宫", "", "术", 4, 0f, 6f, 14, "", 0, "", 40f, 24f, "", 0, 72f, 0f, 26, "疫", false, 5f, "", 0, 0f, 0f, 0f, 0, "SkillAidQuietPlot", "sway", "MagicNovaBlue", 0f, "", "", "", "");
            config[2020820] = new SkillConfig(2020820, "暗计", "陈宫", "", "术", 5, 0f, 6f, 14, "", 0, "", 40f, 24f, "", 0, 100f, 0f, 30, "疫", false, 5f, "", 0, 0f, 0f, 0f, 0, "SkillAidQuietPlot", "sway", "MagicNovaBlue", 0f, "", "", "", "");
            config[2020821] = new SkillConfig(2020821, "敛翼", "钟会", "对目标造成/strength法术伤害，目标生命低于/strengthint%时额外造成其已损生命/strength2%的斩杀伤害", "术", 1, 0f, 4.5f, 12, "", 0, "", 40f, 0f, "", 0, 30f, 0.15f, 30, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "SkillAidLethalGambit", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2020822] = new SkillConfig(2020822, "敛翼", "钟会", "", "术", 2, 0f, 4.5f, 12, "", 0, "", 40f, 0f, "", 0, 46f, 0.18f, 30, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "SkillAidLethalGambit", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2020823] = new SkillConfig(2020823, "敛翼", "钟会", "", "术", 3, 0f, 4.5f, 12, "", 0, "", 40f, 0f, "", 0, 66f, 0.20f, 35, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "SkillAidLethalGambit", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2020824] = new SkillConfig(2020824, "敛翼", "钟会", "", "术", 4, 0f, 4.5f, 12, "", 0, "", 40f, 0f, "", 0, 92f, 0.24f, 35, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "SkillAidLethalGambit", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2020825] = new SkillConfig(2020825, "敛翼", "钟会", "", "术", 5, 0f, 4.5f, 12, "", 0, "", 40f, 0f, "", 0, 128f, 0.28f, 40, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "SkillAidLethalGambit", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2020826] = new SkillConfig(2020826, "舌辩", "张松", "鼓舌如簧，对目标造成/strength法术伤害并大幅减速，持续/bufftime秒", "术", 1, 0f, 4f, 10, "", 0, "", 40f, 0f, "", 0, 28f, 0.45f, 0, "缓", false, 3f, "", 0, 0f, 0f, 0f, 0, "SkillAidSilverTongue", "sway", "AuraSoftPurple", 0f, "", "", "", "");
            config[2020827] = new SkillConfig(2020827, "舌辩", "张松", "", "术", 2, 0f, 4f, 10, "", 0, "", 40f, 0f, "", 0, 42f, 0.45f, 0, "缓", false, 3f, "", 0, 0f, 0f, 0f, 0, "SkillAidSilverTongue", "sway", "AuraSoftPurple", 0f, "", "", "", "");
            config[2020828] = new SkillConfig(2020828, "舌辩", "张松", "", "术", 3, 0f, 4f, 10, "", 0, "", 40f, 0f, "", 0, 62f, 0.45f, 0, "缓", false, 4f, "", 0, 0f, 0f, 0f, 0, "SkillAidSilverTongue", "sway", "AuraSoftPurple", 0f, "", "", "", "");
            config[2020829] = new SkillConfig(2020829, "舌辩", "张松", "", "术", 4, 0f, 4f, 10, "", 0, "", 40f, 0f, "", 0, 88f, 0.45f, 0, "缓", false, 4f, "", 0, 0f, 0f, 0f, 0, "SkillAidSilverTongue", "sway", "AuraSoftPurple", 0f, "", "", "", "");
            config[2020830] = new SkillConfig(2020830, "舌辩", "张松", "", "术", 5, 0f, 4f, 10, "", 0, "", 40f, 0f, "", 0, 122f, 0.45f, 0, "缓", false, 5f, "", 0, 0f, 0f, 0f, 0, "SkillAidSilverTongue", "sway", "AuraSoftPurple", 0f, "", "", "", "");
            config[2020831] = new SkillConfig(2020831, "隐忍", "司马懿", "突袭擒敌，对目标造成/strength法术伤害，将其拉拽到身边并眩晕/bufftime秒", "术", 1, 0f, 7f, 20, "", 0, "", 60f, 0f, "", 0, 55f, 0f, 0, "乱", false, 2f, "", 0, 0f, 0f, 0f, 0, "SkillAidEnduranceStrike", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2020832] = new SkillConfig(2020832, "隐忍", "司马懿", "", "术", 2, 0f, 7f, 20, "", 0, "", 60f, 0f, "", 0, 85f, 0f, 0, "乱", false, 2f, "", 0, 0f, 0f, 0f, 0, "SkillAidEnduranceStrike", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2020833] = new SkillConfig(2020833, "隐忍", "司马懿", "", "术", 3, 0f, 7f, 20, "", 0, "", 60f, 0f, "", 0, 120f, 0f, 0, "乱", false, 2f, "", 0, 0f, 0f, 0f, 0, "SkillAidEnduranceStrike", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2020834] = new SkillConfig(2020834, "隐忍", "司马懿", "", "术", 4, 0f, 7f, 20, "", 0, "", 60f, 0f, "", 0, 165f, 0f, 0, "乱", false, 2f, "", 0, 0f, 0f, 0f, 0, "SkillAidEnduranceStrike", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2020835] = new SkillConfig(2020835, "隐忍", "司马懿", "", "术", 5, 0f, 7f, 20, "", 0, "", 60f, 0f, "", 0, 220f, 0f, 0, "乱", false, 2f, "", 0, 0f, 0f, 0f, 0, "SkillAidEnduranceStrike", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2020836] = new SkillConfig(2020836, "断粮", "程昱", "断其粮道，对目标造成/strength法术伤害并使其每秒流失/strength2生命，持续/bufftime秒", "术", 1, 0f, 5f, 12, "", 0, "", 40f, 0f, "", 0, 20f, 3f, 0, "败", false, 4f, "", 0, 0f, 0f, 0f, 0, "SkillAidCutSupply", "sway", "SoftFireBigRed", 0f, "", "", "", "");
            config[2020837] = new SkillConfig(2020837, "断粮", "程昱", "", "术", 2, 0f, 5f, 12, "", 0, "", 40f, 0f, "", 0, 30f, 4f, 0, "败", false, 4f, "", 0, 0f, 0f, 0f, 0, "SkillAidCutSupply", "sway", "SoftFireBigRed", 0f, "", "", "", "");
            config[2020838] = new SkillConfig(2020838, "断粮", "程昱", "", "术", 3, 0f, 5f, 12, "", 0, "", 40f, 0f, "", 0, 45f, 6f, 0, "败", false, 5f, "", 0, 0f, 0f, 0f, 0, "SkillAidCutSupply", "sway", "SoftFireBigRed", 0f, "", "", "", "");
            config[2020839] = new SkillConfig(2020839, "断粮", "程昱", "", "术", 4, 0f, 5f, 12, "", 0, "", 40f, 0f, "", 0, 65f, 8f, 0, "败", false, 5f, "", 0, 0f, 0f, 0f, 0, "SkillAidCutSupply", "sway", "SoftFireBigRed", 0f, "", "", "", "");
            config[2020840] = new SkillConfig(2020840, "断粮", "程昱", "", "术", 5, 0f, 5f, 12, "", 0, "", 40f, 0f, "", 0, 90f, 11f, 0, "败", false, 6f, "", 0, 0f, 0f, 0f, 0, "SkillAidCutSupply", "sway", "SoftFireBigRed", 0f, "", "", "", "");
            config[2020841] = new SkillConfig(2020841, "鸩酒", "李儒", "奉上鸩酒，对目标造成/strength法术伤害，使其持续中毒每秒/strength2伤害并降低治疗，持续/bufftime秒", "术", 1, 0f, 5.5f, 14, "", 0, "", 40f, 0f, "", 0, 24f, 4f, 30, "疫", false, 4f, "", 0, 0f, 0f, 0f, 0, "SkillAidPoisonWine", "sway", "AuraSoftPurple", 0f, "", "", "", "");
            config[2020842] = new SkillConfig(2020842, "鸩酒", "李儒", "", "术", 2, 0f, 5.5f, 14, "", 0, "", 40f, 0f, "", 0, 36f, 5f, 30, "疫", false, 5f, "", 0, 0f, 0f, 0f, 0, "SkillAidPoisonWine", "sway", "AuraSoftPurple", 0f, "", "", "", "");
            config[2020843] = new SkillConfig(2020843, "鸩酒", "李儒", "", "术", 3, 0f, 5.5f, 14, "", 0, "", 40f, 0f, "", 0, 54f, 8f, 30, "疫", false, 5f, "", 0, 0f, 0f, 0f, 0, "SkillAidPoisonWine", "sway", "AuraSoftPurple", 0f, "", "", "", "");
            config[2020844] = new SkillConfig(2020844, "鸩酒", "李儒", "", "术", 4, 0f, 5.5f, 14, "", 0, "", 40f, 0f, "", 0, 78f, 11f, 30, "疫", false, 6f, "", 0, 0f, 0f, 0f, 0, "SkillAidPoisonWine", "sway", "AuraSoftPurple", 0f, "", "", "", "");
            config[2020845] = new SkillConfig(2020845, "鸩酒", "李儒", "", "术", 5, 0f, 5.5f, 14, "", 0, "", 40f, 0f, "", 0, 108f, 15f, 30, "疫", false, 6f, "", 0, 0f, 0f, 0f, 0, "SkillAidPoisonWine", "sway", "AuraSoftPurple", 0f, "", "", "", "");
            config[2020846] = new SkillConfig(2020846, "篡逆", "司马昭", "图谋篡逆，对/area范围内敌人造成/strength法术伤害并将其定身，持续/bufftime秒", "术", 1, 0f, 6f, 14, "", 0, "", 40f, 24f, "", 0, 22f, 0f, 0, "停", false, 1.5f, "", 0, 0f, 0f, 0f, 0, "SkillAidUsurpPressure", "sway", "MagicNovaBlue", 0f, "", "", "", "");
            config[2020847] = new SkillConfig(2020847, "篡逆", "司马昭", "", "术", 2, 0f, 6f, 14, "", 0, "", 40f, 24f, "", 0, 34f, 0f, 0, "停", false, 1.5f, "", 0, 0f, 0f, 0f, 0, "SkillAidUsurpPressure", "sway", "MagicNovaBlue", 0f, "", "", "", "");
            config[2020848] = new SkillConfig(2020848, "篡逆", "司马昭", "", "术", 3, 0f, 6f, 14, "", 0, "", 40f, 24f, "", 0, 50f, 0f, 0, "停", false, 2.0f, "", 0, 0f, 0f, 0f, 0, "SkillAidUsurpPressure", "sway", "MagicNovaBlue", 0f, "", "", "", "");
            config[2020849] = new SkillConfig(2020849, "篡逆", "司马昭", "", "术", 4, 0f, 6f, 14, "", 0, "", 40f, 24f, "", 0, 72f, 0f, 0, "停", false, 2.0f, "", 0, 0f, 0f, 0f, 0, "SkillAidUsurpPressure", "sway", "MagicNovaBlue", 0f, "", "", "", "");
            config[2020850] = new SkillConfig(2020850, "篡逆", "司马昭", "", "术", 5, 0f, 6f, 14, "", 0, "", 40f, 24f, "", 0, 100f, 0f, 0, "停", false, 2.5f, "", 0, 0f, 0f, 0f, 0, "SkillAidUsurpPressure", "sway", "MagicNovaBlue", 0f, "", "", "", "");
            config[2020851] = new SkillConfig(2020851, "傲言", "许攸", "傲言辱敌，对目标造成/strength2法术伤害并使其受到伤害提升/strength%，持续/bufftime秒", "术", 1, 0f, 4.5f, 12, "", 0, "", 40f, 0f, "", 0, 0.10f, 30f, 0, "伤", false, 4f, "", 0, 0f, 0f, 0f, 0, "SkillAidArrogantWord", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2020852] = new SkillConfig(2020852, "傲言", "许攸", "", "术", 2, 0f, 4.5f, 12, "", 0, "", 40f, 0f, "", 0, 0.13f, 46f, 0, "伤", false, 4f, "", 0, 0f, 0f, 0f, 0, "SkillAidArrogantWord", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2020853] = new SkillConfig(2020853, "傲言", "许攸", "", "术", 3, 0f, 4.5f, 12, "", 0, "", 40f, 0f, "", 0, 0.16f, 66f, 0, "伤", false, 5f, "", 0, 0f, 0f, 0f, 0, "SkillAidArrogantWord", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2020854] = new SkillConfig(2020854, "傲言", "许攸", "", "术", 4, 0f, 4.5f, 12, "", 0, "", 40f, 0f, "", 0, 0.20f, 92f, 0, "伤", false, 5f, "", 0, 0f, 0f, 0f, 0, "SkillAidArrogantWord", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2020855] = new SkillConfig(2020855, "傲言", "许攸", "", "术", 5, 0f, 4.5f, 12, "", 0, "", 40f, 0f, "", 0, 0.25f, 128f, 0, "伤", false, 5f, "", 0, 0f, 0f, 0f, 0, "SkillAidArrogantWord", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2020901] = new SkillConfig(2020901, "守势", "马谡", "守势，对目标造成/strength法术伤害并使其减速/strength2%，持续/bufftime秒", "术", 1, 0f, 6f, 13, "", 0, "", 50f, 0f, "", 0, 30f, 0.45f, 0, "缓", true, 3f, "", 0, 0f, 0f, 0f, 0, "SkillAidSlowStrike", "sway", "MagicNovaYellow", 0f, "", "", "", "");
            config[2020902] = new SkillConfig(2020902, "守势", "马谡", "", "术", 2, 0f, 6f, 13, "", 0, "", 50f, 0f, "", 0, 45f, 0.50f, 0, "缓", true, 3f, "", 0, 0f, 0f, 0f, 0, "SkillAidSlowStrike", "sway", "MagicNovaYellow", 0f, "", "", "", "");
            config[2020903] = new SkillConfig(2020903, "守势", "马谡", "", "术", 3, 0f, 6f, 13, "", 0, "", 50f, 0f, "", 0, 65f, 0.50f, 0, "缓", true, 4f, "", 0, 0f, 0f, 0f, 0, "SkillAidSlowStrike", "sway", "MagicNovaYellow", 0f, "", "", "", "");
            config[2020904] = new SkillConfig(2020904, "守势", "马谡", "", "术", 4, 0f, 6f, 13, "", 0, "", 50f, 0f, "", 0, 90f, 0.55f, 0, "缓", true, 4f, "", 0, 0f, 0f, 0f, 0, "SkillAidSlowStrike", "sway", "MagicNovaYellow", 0f, "", "", "", "");
            config[2020905] = new SkillConfig(2020905, "守势", "马谡", "", "术", 5, 0f, 6f, 13, "", 0, "", 50f, 0f, "", 0, 120f, 0.55f, 0, "缓", true, 4f, "", 0, 0f, 0f, 0f, 0, "SkillAidSlowStrike", "sway", "MagicNovaYellow", 0f, "", "", "", "");
            config[2020906] = new SkillConfig(2020906, "威震", "张辽", "威震逍遥津，获得威震状态持续/bufftime秒：期间普攻有/strengthint%几率眩晕目标，并把受到伤害的/strength%反还给攻击者", "术", 1, 0f, 7f, 16, "", 0, "", 0f, 0f, "", 0, 0.15f, 0f, 40, "威", false, 5f, "", 0, 0f, 0f, 0f, 0, "SkillAidVengefulStance", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2020907] = new SkillConfig(2020907, "威震", "张辽", "", "术", 2, 0f, 7f, 16, "", 0, "", 0f, 0f, "", 0, 0.20f, 0f, 45, "威", false, 5f, "", 0, 0f, 0f, 0f, 0, "SkillAidVengefulStance", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2020908] = new SkillConfig(2020908, "威震", "张辽", "", "术", 3, 0f, 7f, 16, "", 0, "", 0f, 0f, "", 0, 0.25f, 0f, 50, "威", false, 6f, "", 0, 0f, 0f, 0f, 0, "SkillAidVengefulStance", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2020909] = new SkillConfig(2020909, "威震", "张辽", "", "术", 4, 0f, 7f, 16, "", 0, "", 0f, 0f, "", 0, 0.30f, 0f, 55, "威", false, 6f, "", 0, 0f, 0f, 0f, 0, "SkillAidVengefulStance", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2020910] = new SkillConfig(2020910, "威震", "张辽", "", "术", 5, 0f, 7f, 16, "", 0, "", 0f, 0f, "", 0, 0.35f, 0f, 60, "威", false, 7f, "", 0, 0f, 0f, 0f, 0, "SkillAidVengefulStance", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2020911] = new SkillConfig(2020911, "铁壁", "于禁", "竖壁清野，为自身附加/strength%减伤盾持续/bufftime秒，嘲讽/area范围内敌人并对其造成/strength2法术伤害", "术", 1, 0f, 9f, 15, "", 0, "", 40f, 20f, "", 0, 0.20f, 10f, 0, "硬", false, 4f, "", 0, 0f, 0f, 0f, 0, "SkillAidIronGuard", "sway", "MagicNovaYellow", 0f, "", "", "", "");
            config[2020912] = new SkillConfig(2020912, "铁壁", "于禁", "", "术", 2, 0f, 9f, 15, "", 0, "", 40f, 20f, "", 0, 0.24f, 15f, 0, "硬", false, 4f, "", 0, 0f, 0f, 0f, 0, "SkillAidIronGuard", "sway", "MagicNovaYellow", 0f, "", "", "", "");
            config[2020913] = new SkillConfig(2020913, "铁壁", "于禁", "", "术", 3, 0f, 9f, 15, "", 0, "", 40f, 20f, "", 0, 0.28f, 22f, 0, "硬", false, 5f, "", 0, 0f, 0f, 0f, 0, "SkillAidIronGuard", "sway", "MagicNovaYellow", 0f, "", "", "", "");
            config[2020914] = new SkillConfig(2020914, "铁壁", "于禁", "", "术", 4, 0f, 9f, 15, "", 0, "", 40f, 20f, "", 0, 0.32f, 30f, 0, "硬", false, 5f, "", 0, 0f, 0f, 0f, 0, "SkillAidIronGuard", "sway", "MagicNovaYellow", 0f, "", "", "", "");
            config[2020915] = new SkillConfig(2020915, "铁壁", "于禁", "", "术", 5, 0f, 9f, 15, "", 0, "", 40f, 20f, "", 0, 0.36f, 42f, 0, "硬", false, 5f, "", 0, 0f, 0f, 0f, 0, "SkillAidIronGuard", "sway", "MagicNovaYellow", 0f, "", "", "", "");
            config[2020916] = new SkillConfig(2020916, "破阵", "孙坚", "江东猛虎，对/area范围内的敌人造成/strength法术伤害并削减其护甲/strength2点，持续/bufftime秒", "术", 1, 0f, 7f, 14, "", 0, "", 40f, 24f, "", 0, 25f, 25f, 0, "破", true, 4f, "", 0, 0f, 0f, 0f, 0, "SkillAidArmorBreak", "sway", "MagicNovaBlue", 0f, "", "", "", "");
            config[2020917] = new SkillConfig(2020917, "破阵", "孙坚", "", "术", 2, 0f, 7f, 14, "", 0, "", 40f, 24f, "", 0, 38f, 30f, 0, "破", true, 4f, "", 0, 0f, 0f, 0f, 0, "SkillAidArmorBreak", "sway", "MagicNovaBlue", 0f, "", "", "", "");
            config[2020918] = new SkillConfig(2020918, "破阵", "孙坚", "", "术", 3, 0f, 7f, 14, "", 0, "", 40f, 24f, "", 0, 55f, 38f, 0, "破", true, 5f, "", 0, 0f, 0f, 0f, 0, "SkillAidArmorBreak", "sway", "MagicNovaBlue", 0f, "", "", "", "");
            config[2020919] = new SkillConfig(2020919, "破阵", "孙坚", "", "术", 4, 0f, 7f, 14, "", 0, "", 40f, 24f, "", 0, 78f, 46f, 0, "破", true, 5f, "", 0, 0f, 0f, 0f, 0, "SkillAidArmorBreak", "sway", "MagicNovaBlue", 0f, "", "", "", "");
            config[2020920] = new SkillConfig(2020920, "破阵", "孙坚", "", "术", 5, 0f, 7f, 14, "", 0, "", 40f, 24f, "", 0, 105f, 55f, 0, "破", true, 5f, "", 0, 0f, 0f, 0f, 0, "SkillAidArmorBreak", "sway", "MagicNovaBlue", 0f, "", "", "", "");
            config[2020921] = new SkillConfig(2020921, "枪出如龙", "张绣", "枪出如龙，对/area范围内的敌人造成/strength法术伤害", "术", 1, 0f, 6f, 14, "", 0, "", 40f, 24f, "", 0, 30f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "SkillAidSweepingAoe", "sway", "AuraSoftPurple", 0f, "", "", "", "");
            config[2020922] = new SkillConfig(2020922, "枪出如龙", "张绣", "", "术", 2, 0f, 6f, 14, "", 0, "", 40f, 24f, "", 0, 45f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "SkillAidSweepingAoe", "sway", "AuraSoftPurple", 0f, "", "", "", "");
            config[2020923] = new SkillConfig(2020923, "枪出如龙", "张绣", "", "术", 3, 0f, 6f, 14, "", 0, "", 40f, 24f, "", 0, 65f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "SkillAidSweepingAoe", "sway", "AuraSoftPurple", 0f, "", "", "", "");
            config[2020924] = new SkillConfig(2020924, "枪出如龙", "张绣", "", "术", 4, 0f, 6f, 14, "", 0, "", 40f, 24f, "", 0, 90f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "SkillAidSweepingAoe", "sway", "AuraSoftPurple", 0f, "", "", "", "");
            config[2020925] = new SkillConfig(2020925, "枪出如龙", "张绣", "", "术", 5, 0f, 6f, 14, "", 0, "", 40f, 24f, "", 0, 120f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "SkillAidSweepingAoe", "sway", "AuraSoftPurple", 0f, "", "", "", "");
            config[2020926] = new SkillConfig(2020926, "背水", "魏延", "背水一战，对/area范围内的敌人造成/strength法术伤害并降低其治疗/strengthint%，持续/bufftime秒", "术", 1, 0f, 7f, 15, "", 0, "", 40f, 24f, "", 0, 28f, 0f, 20, "疫", true, 4f, "", 0, 0f, 0f, 0f, 0, "SkillAidDiveCleave", "sway", "SoftFireBigRed", 0f, "", "", "", "");
            config[2020927] = new SkillConfig(2020927, "背水", "魏延", "", "术", 2, 0f, 7f, 15, "", 0, "", 40f, 24f, "", 0, 42f, 0f, 20, "疫", true, 4f, "", 0, 0f, 0f, 0f, 0, "SkillAidDiveCleave", "sway", "SoftFireBigRed", 0f, "", "", "", "");
            config[2020928] = new SkillConfig(2020928, "背水", "魏延", "", "术", 3, 0f, 7f, 15, "", 0, "", 40f, 24f, "", 0, 60f, 0f, 25, "疫", true, 5f, "", 0, 0f, 0f, 0f, 0, "SkillAidDiveCleave", "sway", "SoftFireBigRed", 0f, "", "", "", "");
            config[2020929] = new SkillConfig(2020929, "背水", "魏延", "", "术", 4, 0f, 7f, 15, "", 0, "", 40f, 24f, "", 0, 85f, 0f, 25, "疫", true, 5f, "", 0, 0f, 0f, 0f, 0, "SkillAidDiveCleave", "sway", "SoftFireBigRed", 0f, "", "", "", "");
            config[2020930] = new SkillConfig(2020930, "背水", "魏延", "", "术", 5, 0f, 7f, 15, "", 0, "", 40f, 24f, "", 0, 115f, 0f, 30, "疫", true, 5f, "", 0, 0f, 0f, 0f, 0, "SkillAidDiveCleave", "sway", "SoftFireBigRed", 0f, "", "", "", "");
            config[2020931] = new SkillConfig(2020931, "恶来", "典韦", "古之恶来，对自身/area范围内的敌人造成/strength法术伤害，并进入狂暴状态使自身增伤与受击加深/strength2%，持续/bufftime秒", "术", 1, 0f, 8f, 16, "", 0, "", 40f, 20f, "", 0, 35f, 0.15f, 0, "狂", false, 5f, "", 0, 0f, 0f, 0f, 0, "SkillAidBerserkCleave", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2020932] = new SkillConfig(2020932, "恶来", "典韦", "", "术", 2, 0f, 8f, 16, "", 0, "", 40f, 20f, "", 0, 52f, 0.20f, 0, "狂", false, 5f, "", 0, 0f, 0f, 0f, 0, "SkillAidBerserkCleave", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2020933] = new SkillConfig(2020933, "恶来", "典韦", "", "术", 3, 0f, 8f, 16, "", 0, "", 40f, 20f, "", 0, 75f, 0.25f, 0, "狂", false, 6f, "", 0, 0f, 0f, 0f, 0, "SkillAidBerserkCleave", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2020934] = new SkillConfig(2020934, "恶来", "典韦", "", "术", 4, 0f, 8f, 16, "", 0, "", 40f, 20f, "", 0, 105f, 0.30f, 0, "狂", false, 6f, "", 0, 0f, 0f, 0f, 0, "SkillAidBerserkCleave", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2020935] = new SkillConfig(2020935, "恶来", "典韦", "", "术", 5, 0f, 8f, 16, "", 0, "", 40f, 20f, "", 0, 145f, 0.35f, 0, "狂", false, 7f, "", 0, 0f, 0f, 0f, 0, "SkillAidBerserkCleave", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2020936] = new SkillConfig(2020936, "虎豹", "曹真", "虎豹骑突刺，对目标造成/strength法术伤害", "术", 1, 0f, 6f, 12, "", 0, "", 50f, 0f, "", 0, 45f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "SkillAidThrust", "sway", "MagicNovaYellow", 0f, "", "", "", "");
            config[2020937] = new SkillConfig(2020937, "虎豹", "曹真", "", "术", 2, 0f, 6f, 12, "", 0, "", 50f, 0f, "", 0, 65f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "SkillAidThrust", "sway", "MagicNovaYellow", 0f, "", "", "", "");
            config[2020938] = new SkillConfig(2020938, "虎豹", "曹真", "", "术", 3, 0f, 6f, 12, "", 0, "", 50f, 0f, "", 0, 92f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "SkillAidThrust", "sway", "MagicNovaYellow", 0f, "", "", "", "");
            config[2020939] = new SkillConfig(2020939, "虎豹", "曹真", "", "术", 4, 0f, 6f, 12, "", 0, "", 50f, 0f, "", 0, 128f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "SkillAidThrust", "sway", "MagicNovaYellow", 0f, "", "", "", "");
            config[2020940] = new SkillConfig(2020940, "虎豹", "曹真", "", "术", 5, 0f, 6f, 12, "", 0, "", 50f, 0f, "", 0, 172f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "SkillAidThrust", "sway", "MagicNovaYellow", 0f, "", "", "", "");
            config[2020941] = new SkillConfig(2020941, "破阵冲阵", "孙策", "小霸王掣枪直撞，对前方穿透路径上的敌人造成/strength法术伤害", "术", 1, 0f, 9f, 17, "", 0, "", 60f, 20f, "", 0, 40f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "SkillAidChargeImpale", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2020942] = new SkillConfig(2020942, "破阵冲阵", "孙策", "", "术", 2, 0f, 9f, 17, "", 0, "", 60f, 20f, "", 0, 60f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "SkillAidChargeImpale", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2020943] = new SkillConfig(2020943, "破阵冲阵", "孙策", "", "术", 3, 0f, 9f, 17, "", 0, "", 60f, 20f, "", 0, 85f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "SkillAidChargeImpale", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2020944] = new SkillConfig(2020944, "破阵冲阵", "孙策", "", "术", 4, 0f, 9f, 17, "", 0, "", 60f, 20f, "", 0, 120f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "SkillAidChargeImpale", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2020945] = new SkillConfig(2020945, "破阵冲阵", "孙策", "", "术", 5, 0f, 9f, 17, "", 0, "", 60f, 20f, "", 0, 160f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "SkillAidChargeImpale", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2020946] = new SkillConfig(2020946, "江河", "蒋钦", "江河倾泻，对/area范围内的敌人造成/strength法术伤害并使其每秒流失/strength2点生命，持续/bufftime秒", "术", 1, 0f, 7f, 15, "", 0, "", 40f, 24f, "", 0, 22f, 3f, 0, "", false, 4f, "", 0, 0f, 0f, 0f, 0, "SkillAidScorchingRain", "sway", "SoftFireBigRed", 0f, "", "", "", "");
            config[2020947] = new SkillConfig(2020947, "江河", "蒋钦", "", "术", 2, 0f, 7f, 15, "", 0, "", 40f, 24f, "", 0, 32f, 4f, 0, "", false, 4f, "", 0, 0f, 0f, 0f, 0, "SkillAidScorchingRain", "sway", "SoftFireBigRed", 0f, "", "", "", "");
            config[2020948] = new SkillConfig(2020948, "江河", "蒋钦", "", "术", 3, 0f, 7f, 15, "", 0, "", 40f, 24f, "", 0, 46f, 6f, 0, "", false, 5f, "", 0, 0f, 0f, 0f, 0, "SkillAidScorchingRain", "sway", "SoftFireBigRed", 0f, "", "", "", "");
            config[2020949] = new SkillConfig(2020949, "江河", "蒋钦", "", "术", 4, 0f, 7f, 15, "", 0, "", 40f, 24f, "", 0, 64f, 8f, 0, "", false, 5f, "", 0, 0f, 0f, 0f, 0, "SkillAidScorchingRain", "sway", "SoftFireBigRed", 0f, "", "", "", "");
            config[2020950] = new SkillConfig(2020950, "江河", "蒋钦", "", "术", 5, 0f, 7f, 15, "", 0, "", 40f, 24f, "", 0, 88f, 11f, 0, "", false, 5f, "", 0, 0f, 0f, 0f, 0, "SkillAidScorchingRain", "sway", "SoftFireBigRed", 0f, "", "", "", "");
            config[2020951] = new SkillConfig(2020951, "陷阵怒吼", "文丑", "河北声威，对/area范围内的敌人造成/strength法术伤害，并额外造成其最大生命/strength2%的伤害", "术", 1, 0f, 8f, 18, "", 0, "", 40f, 24f, "", 0, 80f, 0.08f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "SkillAidColossalSlam", "sway", "MagicNovaBlue", 0f, "", "", "", "");
            config[2020952] = new SkillConfig(2020952, "陷阵怒吼", "文丑", "", "术", 2, 0f, 8f, 18, "", 0, "", 40f, 24f, "", 0, 110f, 0.10f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "SkillAidColossalSlam", "sway", "MagicNovaBlue", 0f, "", "", "", "");
            config[2020953] = new SkillConfig(2020953, "陷阵怒吼", "文丑", "", "术", 3, 0f, 8f, 18, "", 0, "", 40f, 24f, "", 0, 145f, 0.12f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "SkillAidColossalSlam", "sway", "MagicNovaBlue", 0f, "", "", "", "");
            config[2020954] = new SkillConfig(2020954, "陷阵怒吼", "文丑", "", "术", 4, 0f, 8f, 18, "", 0, "", 40f, 24f, "", 0, 185f, 0.14f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "SkillAidColossalSlam", "sway", "MagicNovaBlue", 0f, "", "", "", "");
            config[2020955] = new SkillConfig(2020955, "陷阵怒吼", "文丑", "", "术", 5, 0f, 8f, 18, "", 0, "", 40f, 24f, "", 0, 235f, 0.16f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "SkillAidColossalSlam", "sway", "MagicNovaBlue", 0f, "", "", "", "");
            config[2021001] = new SkillConfig(2021001, "卸甲", "刘晔", "启动机关卸甲，无视目标/strength%护甲；若目标带护盾，对其伤害提升/strength2%，持续/bufftime秒", "术", 1, 0f, 6f, 14, "", 0, "", 0f, 0f, "", 0, 0.20f, 0.30f, 0, "卸", false, 5f, "", 0, 0f, 0f, 0f, 0, "SkillAidDisarmState", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2021002] = new SkillConfig(2021002, "卸甲", "刘晔", "", "术", 2, 0f, 6f, 14, "", 0, "", 0f, 0f, "", 0, 0.25f, 0.35f, 0, "卸", false, 5f, "", 0, 0f, 0f, 0f, 0, "SkillAidDisarmState", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2021003] = new SkillConfig(2021003, "卸甲", "刘晔", "", "术", 3, 0f, 6f, 14, "", 0, "", 0f, 0f, "", 0, 0.30f, 0.45f, 0, "卸", false, 6f, "", 0, 0f, 0f, 0f, 0, "SkillAidDisarmState", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2021004] = new SkillConfig(2021004, "卸甲", "刘晔", "", "术", 4, 0f, 6f, 14, "", 0, "", 0f, 0f, "", 0, 0.35f, 0.55f, 0, "卸", false, 6f, "", 0, 0f, 0f, 0f, 0, "SkillAidDisarmState", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2021005] = new SkillConfig(2021005, "卸甲", "刘晔", "", "术", 5, 0f, 6f, 14, "", 0, "", 0f, 0f, "", 0, 0.40f, 0.70f, 0, "卸", false, 7f, "", 0, 0f, 0f, 0f, 0, "SkillAidDisarmState", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2021006] = new SkillConfig(2021006, "巧工", "黄月英", "机关巧思，使我方生命最低的英雄永久提升/strength点攻击，另一名英雄永久提升/strength2点护甲", "术", 1, 0f, 9f, 20, "", 0, "", 80f, 0f, "", 0, 12f, 16f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "SkillAidCraftBuff", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2021007] = new SkillConfig(2021007, "巧工", "黄月英", "", "术", 2, 0f, 9f, 20, "", 0, "", 80f, 0f, "", 0, 18f, 24f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "SkillAidCraftBuff", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2021008] = new SkillConfig(2021008, "巧工", "黄月英", "", "术", 3, 0f, 9f, 20, "", 0, "", 80f, 0f, "", 0, 26f, 34f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "SkillAidCraftBuff", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2021009] = new SkillConfig(2021009, "巧工", "黄月英", "", "术", 4, 0f, 9f, 20, "", 0, "", 80f, 0f, "", 0, 36f, 46f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "SkillAidCraftBuff", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2021010] = new SkillConfig(2021010, "巧工", "黄月英", "", "术", 5, 0f, 9f, 20, "", 0, "", 80f, 0f, "", 0, 48f, 60f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "SkillAidCraftBuff", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2021011] = new SkillConfig(2021011, "发火", "杜预", "对目标发射火矢，造成/strength法术伤害", "术", 1, 0f, 5f, 12, "", 0, "", 50f, 0f, "", 0, 30f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "SkillAidFireShot", "sway", "SoftFireBigRed", 0f, "", "", "", "");
            config[2021012] = new SkillConfig(2021012, "发火", "杜预", "", "术", 2, 0f, 5f, 12, "", 0, "", 50f, 0f, "", 0, 45f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "SkillAidFireShot", "sway", "SoftFireBigRed", 0f, "", "", "", "");
            config[2021013] = new SkillConfig(2021013, "发火", "杜预", "", "术", 3, 0f, 5f, 12, "", 0, "", 50f, 0f, "", 0, 65f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "SkillAidFireShot", "sway", "SoftFireBigRed", 0f, "", "", "", "");
            config[2021014] = new SkillConfig(2021014, "发火", "杜预", "", "术", 4, 0f, 5f, 12, "", 0, "", 50f, 0f, "", 0, 90f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "SkillAidFireShot", "sway", "SoftFireBigRed", 0f, "", "", "", "");
            config[2021015] = new SkillConfig(2021015, "发火", "杜预", "", "术", 5, 0f, 5f, 12, "", 0, "", 50f, 0f, "", 0, 120f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "SkillAidFireShot", "sway", "SoftFireBigRed", 0f, "", "", "", "");
            config[2021016] = new SkillConfig(2021016, "律令", "陈群", "颁布军令，对/area范围内敌人造成/strength法术伤害，并使其受治疗/strengthint%，持续/bufftime秒", "术", 1, 0f, 6f, 14, "", 0, "", 0f, 25f, "", 0, 24f, 0f, 30, "疫", false, 4f, "", 0, 0f, 0f, 0f, 0, "SkillAidDecreeAoe", "sway", "MagicNovaBlue", 0f, "", "", "", "");
            config[2021017] = new SkillConfig(2021017, "律令", "陈群", "", "术", 2, 0f, 6f, 14, "", 0, "", 0f, 25f, "", 0, 38f, 0f, 35, "疫", false, 4f, "", 0, 0f, 0f, 0f, 0, "SkillAidDecreeAoe", "sway", "MagicNovaBlue", 0f, "", "", "", "");
            config[2021018] = new SkillConfig(2021018, "律令", "陈群", "", "术", 3, 0f, 6f, 14, "", 0, "", 0f, 25f, "", 0, 56f, 0f, 40, "疫", false, 4f, "", 0, 0f, 0f, 0f, 0, "SkillAidDecreeAoe", "sway", "MagicNovaBlue", 0f, "", "", "", "");
            config[2021019] = new SkillConfig(2021019, "律令", "陈群", "", "术", 4, 0f, 6f, 14, "", 0, "", 0f, 25f, "", 0, 80f, 0f, 45, "疫", false, 4f, "", 0, 0f, 0f, 0f, 0, "SkillAidDecreeAoe", "sway", "MagicNovaBlue", 0f, "", "", "", "");
            config[2021020] = new SkillConfig(2021020, "律令", "陈群", "", "术", 5, 0f, 6f, 14, "", 0, "", 0f, 25f, "", 0, 110f, 0f, 50, "疫", false, 4f, "", 0, 0f, 0f, 0f, 0, "SkillAidDecreeAoe", "sway", "MagicNovaBlue", 0f, "", "", "", "");
            config[2021021] = new SkillConfig(2021021, "仁德", "刘备", "仁德之政，使我方全体英雄提升生命回复/strength点/秒、法力回复/strength2点/秒，持续/bufftime秒", "术", 1, 0f, 8f, 16, "", 0, "", 100f, 0f, "", 0, 6f, 1f, 0, "仁", false, 6f, "", 0, 0f, 0f, 0f, 0, "SkillAidBenevolence", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2021022] = new SkillConfig(2021022, "仁德", "刘备", "", "术", 2, 0f, 8f, 16, "", 0, "", 100f, 0f, "", 0, 7f, 1.5f, 0, "仁", false, 6f, "", 0, 0f, 0f, 0f, 0, "SkillAidBenevolence", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2021023] = new SkillConfig(2021023, "仁德", "刘备", "", "术", 3, 0f, 8f, 16, "", 0, "", 100f, 0f, "", 0, 8f, 2f, 0, "仁", false, 7f, "", 0, 0f, 0f, 0f, 0, "SkillAidBenevolence", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2021024] = new SkillConfig(2021024, "仁德", "刘备", "", "术", 4, 0f, 8f, 16, "", 0, "", 100f, 0f, "", 0, 9f, 2.2f, 0, "仁", false, 7f, "", 0, 0f, 0f, 0f, 0, "SkillAidBenevolence", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2021025] = new SkillConfig(2021025, "仁德", "刘备", "", "术", 5, 0f, 8f, 16, "", 0, "", 100f, 0f, "", 0, 10f, 2.5f, 0, "仁", false, 8f, "", 0, 0f, 0f, 0f, 0, "SkillAidBenevolence", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2021026] = new SkillConfig(2021026, "护驾", "曹操", "护驾亲临，将生命比例最低的友军英雄传送到身边，套/strength2%最大生命护盾，并提升其受疗/strength%，持续/bufftime秒", "术", 1, 0f, 10f, 18, "", 0, "", 80f, 0f, "", 0, 0.15f, 0.20f, 0, "护", false, 5f, "", 0, 0f, 0f, 0f, 0, "SkillAidImperialGuard", "sway", "ShieldSoftBlue", 0f, "", "", "", "");
            config[2021027] = new SkillConfig(2021027, "护驾", "曹操", "", "术", 2, 0f, 10f, 18, "", 0, "", 80f, 0f, "", 0, 0.20f, 0.25f, 0, "护", false, 5f, "", 0, 0f, 0f, 0f, 0, "SkillAidImperialGuard", "sway", "ShieldSoftBlue", 0f, "", "", "", "");
            config[2021028] = new SkillConfig(2021028, "护驾", "曹操", "", "术", 3, 0f, 10f, 18, "", 0, "", 80f, 0f, "", 0, 0.25f, 0.30f, 0, "护", false, 6f, "", 0, 0f, 0f, 0f, 0, "SkillAidImperialGuard", "sway", "ShieldSoftBlue", 0f, "", "", "", "");
            config[2021029] = new SkillConfig(2021029, "护驾", "曹操", "", "术", 4, 0f, 10f, 18, "", 0, "", 80f, 0f, "", 0, 0.28f, 0.35f, 0, "护", false, 6f, "", 0, 0f, 0f, 0f, 0, "SkillAidImperialGuard", "sway", "ShieldSoftBlue", 0f, "", "", "", "");
            config[2021030] = new SkillConfig(2021030, "护驾", "曹操", "", "术", 5, 0f, 10f, 18, "", 0, "", 80f, 0f, "", 0, 0.30f, 0.40f, 0, "护", false, 7f, "", 0, 0f, 0f, 0f, 0, "SkillAidImperialGuard", "sway", "ShieldSoftBlue", 0f, "", "", "", "");
            config[2021031] = new SkillConfig(2021031, "制衡", "孙权", "制衡之道，为范围内最多/targetcount名友军英雄永久随机提升攻击或ap/strength点、护甲或魔抗/strength2点", "术", 1, 0f, 8f, 16, "", 0, "", 100f, 0f, "", 3, 12f, 8f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "SkillAidBalance", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2021032] = new SkillConfig(2021032, "制衡", "孙权", "", "术", 2, 0f, 8f, 16, "", 0, "", 100f, 0f, "", 3, 18f, 12f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "SkillAidBalance", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2021033] = new SkillConfig(2021033, "制衡", "孙权", "", "术", 3, 0f, 8f, 16, "", 0, "", 100f, 0f, "", 4, 24f, 16f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "SkillAidBalance", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2021034] = new SkillConfig(2021034, "制衡", "孙权", "", "术", 4, 0f, 8f, 16, "", 0, "", 100f, 0f, "", 4, 30f, 20f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "SkillAidBalance", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2021035] = new SkillConfig(2021035, "制衡", "孙权", "", "术", 5, 0f, 8f, 16, "", 0, "", 100f, 0f, "", 5, 36f, 24f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "SkillAidBalance", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2021036] = new SkillConfig(2021036, "暴虐", "董卓", "暴虐之威，使范围内最多/targetcount名友军伤害提升/strength%，持续/bufftime秒", "术", 1, 0f, 7f, 12, "", 0, "", 80f, 0f, "", 3, 0.10f, 0f, 0, "重", false, 5f, "", 0, 0f, 0f, 0f, 0, "AidBuffArea", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2021037] = new SkillConfig(2021037, "暴虐", "董卓", "", "术", 2, 0f, 7f, 12, "", 0, "", 80f, 0f, "", 3, 0.12f, 0f, 0, "重", false, 5f, "", 0, 0f, 0f, 0f, 0, "AidBuffArea", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2021038] = new SkillConfig(2021038, "暴虐", "董卓", "", "术", 3, 0f, 7f, 12, "", 0, "", 80f, 0f, "", 3, 0.15f, 0f, 0, "重", false, 5f, "", 0, 0f, 0f, 0f, 0, "AidBuffArea", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2021039] = new SkillConfig(2021039, "暴虐", "董卓", "", "术", 4, 0f, 7f, 12, "", 0, "", 80f, 0f, "", 3, 0.18f, 0f, 0, "重", false, 5f, "", 0, 0f, 0f, 0f, 0, "AidBuffArea", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2021040] = new SkillConfig(2021040, "暴虐", "董卓", "", "术", 5, 0f, 7f, 12, "", 0, "", 80f, 0f, "", 3, 0.20f, 0f, 0, "重", false, 5f, "", 0, 0f, 0f, 0f, 0, "AidBuffArea", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2021041] = new SkillConfig(2021041, "一统", "司马炎", "一统天下之势，使范围内最多/targetcount名友军减伤/strength%，持续/bufftime秒", "术", 1, 0f, 7f, 12, "", 0, "", 80f, 0f, "", 3, 0.10f, 0f, 0, "硬", false, 5f, "", 0, 0f, 0f, 0f, 0, "AidBuffArea", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2021042] = new SkillConfig(2021042, "一统", "司马炎", "", "术", 2, 0f, 7f, 12, "", 0, "", 80f, 0f, "", 3, 0.12f, 0f, 0, "硬", false, 5f, "", 0, 0f, 0f, 0f, 0, "AidBuffArea", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2021043] = new SkillConfig(2021043, "一统", "司马炎", "", "术", 3, 0f, 7f, 12, "", 0, "", 80f, 0f, "", 3, 0.15f, 0f, 0, "硬", false, 5f, "", 0, 0f, 0f, 0f, 0, "AidBuffArea", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2021044] = new SkillConfig(2021044, "一统", "司马炎", "", "术", 4, 0f, 7f, 12, "", 0, "", 80f, 0f, "", 3, 0.18f, 0f, 0, "硬", false, 5f, "", 0, 0f, 0f, 0f, 0, "AidBuffArea", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2021045] = new SkillConfig(2021045, "一统", "司马炎", "", "术", 5, 0f, 7f, 12, "", 0, "", 80f, 0f, "", 3, 0.20f, 0f, 0, "硬", false, 5f, "", 0, 0f, 0f, 0f, 0, "AidBuffArea", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2021046] = new SkillConfig(2021046, "名门", "袁绍", "名门之望，祝福我方生命比例最高的一员大将，提升攻击/strength点、护甲与魔抗/strength2点，持续/bufftime秒", "术", 1, 0f, 8f, 14, "", 0, "", 80f, 0f, "", 0, 20f, 12f, 0, "名", false, 8f, "", 0, 0f, 0f, 0f, 0, "SkillAidNobleBless", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2021047] = new SkillConfig(2021047, "名门", "袁绍", "", "术", 2, 0f, 8f, 14, "", 0, "", 80f, 0f, "", 0, 28f, 17f, 0, "名", false, 8f, "", 0, 0f, 0f, 0f, 0, "SkillAidNobleBless", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2021048] = new SkillConfig(2021048, "名门", "袁绍", "", "术", 3, 0f, 8f, 14, "", 0, "", 80f, 0f, "", 0, 36f, 22f, 0, "名", false, 9f, "", 0, 0f, 0f, 0f, 0, "SkillAidNobleBless", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2021049] = new SkillConfig(2021049, "名门", "袁绍", "", "术", 4, 0f, 8f, 14, "", 0, "", 80f, 0f, "", 0, 44f, 27f, 0, "名", false, 9f, "", 0, 0f, 0f, 0f, 0, "SkillAidNobleBless", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2021050] = new SkillConfig(2021050, "名门", "袁绍", "", "术", 5, 0f, 8f, 14, "", 0, "", 80f, 0f, "", 0, 50f, 30f, 0, "名", false, 10f, "", 0, 0f, 0f, 0f, 0, "SkillAidNobleBless", "sway", "MagicChargeYellow", 0f, "", "", "", "");
            config[2090001] = new SkillConfig(2090001, "速射", "速射", "箭矢飞行速度提升", "道具", 1, 0f, 0f, 0, "", 1, "", 0f, 0f, "", 0, 2.5f, 0f, 0, "", false, 0f, "", 0, 0f, 0f, 0f, 0, "ModifyShootSpeed", "", "", 0f, "", "", "", "");

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
