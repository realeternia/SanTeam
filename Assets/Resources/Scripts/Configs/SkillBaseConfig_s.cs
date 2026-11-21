﻿﻿﻿﻿﻿﻿﻿﻿﻿using System;
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
            {"Type", new FieldMetaInfo("分类", "string", 0, "单伤:#8F3A34,群伤:#B84A40,单疗:#2F7D4E,群疗:#3F9B62,单增:#355F8F,群增:#4678A8,自增:#2F7F78,光环:#4C9C95,单控:#6B4A8F,群控:#8B65A8,召唤:#A06428,兵增:#C08A3E,职业:#555B60,连接:#707C82,道具:#8C7A2F")},
            {"SelectOnly", new FieldMetaInfo("目标硬过滤(;分隔,全部满足才可选: ally/enemy/hero/soldier/notSelf/inFight/notFullHp/hpBelow:N/job:盾|士/hasBuff:短名)", "string", 0)},
            {"SelectFirst", new FieldMetaInfo("目标优先级(;分隔,按序比较、平手看下一条: lowestHp/highestHp/highestArmor/lowestArmor/highestAp/highestAtk/highestMagicRes/lowestMagicRes/highestMp/biggestHpGap/nearest/farthest/rangedFirst/meleeFirst/heroFirst/nearestToEnemy/random)", "string", 0)},
            {"Template", new FieldMetaInfo("技能模板名(对应技能模板表Name;空=不使用模板走原有各列)", "string", 78)},
            {"Modules", new FieldMetaInfo("组件名列表(对应技能模板组件表Name;空=用模板默认组件)", "string[]", 0)},
            {"Lv", new FieldMetaInfo("等级", "int", 60)},
            {"Rate", new FieldMetaInfo("发动概率", "float", 59)},
            {"CD", new FieldMetaInfo("发动cd", "float", 60)},
            {"MpCost", new FieldMetaInfo("技能MP消耗", "int", 60)},
            {"StartMpRate", new FieldMetaInfo("开局MP比例(0=从0充能,1=开局满MP)", "float", 60)},
            {"MpLimit", new FieldMetaInfo("MP累积上限层数(0=上限1×MpCost；N=最多累积N×MpCost，每次发动仍扣MpCost)", "int", 60)},
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
            {"AuraMpRate", new FieldMetaInfo("光环耗蓝倍率(仅光环技能：开启期间每秒消耗MP=1×该值，默认20)", "float", 60)},
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
        ///目标硬过滤（; 分隔，全部满足才可被选中；空=不过滤）：ally/enemy 候选阵营（都未配时用脚本默认阵营）、
        ///hero/soldier 仅英雄/仅士兵、notSelf 排除施法者、inFight 仅交战中、notFullHp 排除满血、
        ///hpBelow:N 仅生命百分比低于 N、job:盾|士 仅指定职业、hasBuff:短名 仅携带指定 Buff
        /// </summary>
        public string SelectOnly;
        /// <summary>
        ///目标优先级（; 分隔，按顺序逐条比较，前一条分出胜负即返回，平手看下一条；空=随机）：
        ///lowestHp/highestHp/highestArmor/lowestArmor/highestAp/highestAtk/highestMagicRes/lowestMagicRes/
        ///nearest/farthest/rangedFirst/meleeFirst/random
        /// </summary>
        public string SelectFirst;
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
        ///技能MP消耗（0=不使用MP，>0则靠普攻/受击获取法力（mpRegen 为百分比加成），满才能发动，发动后扣 MpCost）
        /// </summary>
        public int MpCost;
        /// <summary>
        ///开局MP比例（0~1；0=从0开始充能（默认），1=开局即满MP，介于两者表示开局已有一部分MP）
        /// </summary>
        public float StartMpRate;
        /// <summary>
        ///MP累积上限层数（0=上限1×MpCost；N=可累积N×MpCost，每次发动仍扣1×MpCost）
        /// </summary>
        public int MpLimit;
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
        ///范围特效（仅范围技能使用，视觉缩放 = EffectConfig.Scale × Area / EffectManager.AreaEffectBaseRadius）
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
        ///光环耗蓝倍率（仅 SkillComBuffAura 生效：开启期间每秒消耗 MP = 1 × AuraMpRate；默认 20，组件 150~154 可改为 15/20/25/30/40）
        /// </summary>
        public float AuraMpRate = 20f;


        public SkillBaseConfig(int Id, string Name, string Sname, float Version, string Descript, string Type, string SelectOnly, string SelectFirst, string Template, string[] Modules, int Lv, float Rate, float CD, int MpCost, float StartMpRate, int MpLimit, string TriggerCondition, int DamageType, string HurtTag, float Range, float Area, string TargetType, int TargetCount, float DamageStrength, float HealStrength, string Strength2, string StrengthBuff1, string BuffId, float BuffTime, string SummonTag, int SummonCount, float SummonTime, float SummonHitInterval, float SummonSpeed, int ItemId, string ScriptName, string Action, string SelfEffect, string HitEffect, string AreaEffect, string Icon, string LinkSelf, string LinkTeam, float AuraMpRate)
        {
            this.Id = Id;
            this.Name = Name;
            this.Sname = Sname;
            this.Version = Version;
            this.Descript = Descript;
            this.Type = Type;
            this.SelectOnly = SelectOnly;
            this.SelectFirst = SelectFirst;
            this.Template = Template;
            this.Modules = Modules;
            this.Lv = Lv;
            this.Rate = Rate;
            this.CD = CD;
            this.MpCost = MpCost;
            this.StartMpRate = StartMpRate;
            this.MpLimit = MpLimit;
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
            this.AuraMpRate = AuraMpRate;
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
