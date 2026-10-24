﻿using System;
using System.Collections;
using System.Collections.Generic;

namespace CommonConfig
{
    public class BuffConfig
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
            {"Id", new FieldMetaInfo("序列", "int", 60)},
            {"Name", new FieldMetaInfo("名字", "string", 0)},
            {"NameS", new FieldMetaInfo("短名", "string", 0, "", true)},
            {"Des", new FieldMetaInfo("描述", "string", 509)},
            {"IsPositive", new FieldMetaInfo("是否正面", "bool", 0)},
            {"CanDispel", new FieldMetaInfo("是否可驱散", "bool", 0)},
            {"IsControl", new FieldMetaInfo("是否控制技能", "bool", 0)},
            {"StackMode", new FieldMetaInfo("叠加模式(1共存/2数值融合/3时间延长/4刷新)", "int", 72, "1:共存,2:数值融合,3:时间延长,4:刷新")},
            {"ScriptName", new FieldMetaInfo("脚本名", "string", 171)},
            {"ColorStart", new FieldMetaInfo("启动色", "string", 0)},
            {"ColorEnd", new FieldMetaInfo("结束色", "string", 0)},
            {"BuffEffect", new FieldMetaInfo("hit", "string", 220)},
            {"Material", new FieldMetaInfo("切材质", "string", 0)},
            {"Icon", new FieldMetaInfo("图标", "string", 0)},
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
        ///短名
        /// </summary>
        public string NameS;
        /// <summary>
        ///描述
        /// </summary>
        public string Des;
        /// <summary>
        ///是否正面
        /// </summary>
        public bool IsPositive;
        /// <summary>
        ///是否可驱散（true=普通buff可被驱散；false=英雄招牌buff不可驱散）
        /// </summary>
        public bool CanDispel;
        /// <summary>
        ///是否控制技能（true=控制类Buff，控制期间不回复技能蓝量）
        /// </summary>
        public bool IsControl;
        /// <summary>
        ///叠加模式（1共存 2数值融合 3时间延长 4刷新）
        /// </summary>
        public int StackMode;
        /// <summary>
        ///脚本名
        /// </summary>
        public string ScriptName;
        /// <summary>
        ///启动色
        /// </summary>
        public string ColorStart;
        /// <summary>
        ///结束色
        /// </summary>
        public string ColorEnd;
        /// <summary>
        ///hit
        /// </summary>
        public string BuffEffect;
        /// <summary>
        ///切材质
        /// </summary>
        public string Material;
        /// <summary>
        ///图标
        /// </summary>
        public string Icon;


        public BuffConfig(int Id, string Name, string NameS, string Des, bool IsPositive, bool CanDispel, bool IsControl, int StackMode, string ScriptName, string ColorStart, string ColorEnd, string BuffEffect, string Material, string Icon)
        {
            this.Id = Id;
            this.Name = Name;
            this.NameS = NameS;
            this.Des = Des;
            this.IsPositive = IsPositive;
            this.CanDispel = CanDispel;
            this.IsControl = IsControl;
            this.StackMode = StackMode;
            this.ScriptName = ScriptName;
            this.ColorStart = ColorStart;
            this.ColorEnd = ColorEnd;
            this.BuffEffect = BuffEffect;
            this.Material = Material;
            this.Icon = Icon;
        }

        public BuffConfig() { }

        private static Dictionary<int, BuffConfig> config = new Dictionary<int, BuffConfig>();
        public static Dictionary<int, BuffConfig>.ValueCollection ConfigList
        {
            get { return config.Values; }
        }

        public static void Refresh(Dictionary<int, BuffConfig> dict)
        {
            config.Clear();
            config = dict;
            RebuildIndex();
        }

        public static void Load()
        {
            config.Clear();
            config[300001] = new BuffConfig(300001, "护盾", "盾", "吸收受到的伤害，护盾值耗尽后消失", true, false, false, 2, "BuffShield", "", "", "ShieldSoftBlue", "", "");
            config[300002] = new BuffConfig(300002, "减伤盾", "硬", "受到攻击时按固定比例减免伤害", true, true, false, 1, "BuffShieldValue", "", "", "ShieldSoftYellow", "", "");
            config[300003] = new BuffConfig(300003, "吸血", "吸", "攻击时按造成伤害的一定比例回复生命", true, true, false, 1, "BuffSuck", "#FF0000", "#993333", "", "", "");
            config[300004] = new BuffConfig(300004, "伤害提升", "重", "造成的伤害按比例提升", true, true, false, 1, "BuffFrenzy", "", "", "", "", "");
            config[300005] = new BuffConfig(300005, "攻速提升", "快", "攻击速度按比例提升", true, true, false, 1, "BuffCoolDown", "", "", "HeartStream", "", "");
            config[300007] = new BuffConfig(300007, "急救", "愈", "每秒回复一定生命值", true, true, false, 1, "BuffTimeHeal", "", "", "MagicBuffGreen", "", "");
            config[300008] = new BuffConfig(300008, "攻击提升", "攻", "按数值提升自身攻击力", true, true, false, 1, "BuffAtkAdd", "", "", "", "", "");
            config[300009] = new BuffConfig(300009, "移速提升", "速", "按百分比提升移动速度", true, true, false, 1, "BuffMoveSpeedAdd", "", "", "", "", "");
            config[300012] = new BuffConfig(300012, "护甲", "甲", "提升护甲", true, true, false, 1, "BuffArmorAdd", "", "", "SparkleAreaWhite", "", "");
            config[300013] = new BuffConfig(300013, "魔抗提升", "抗", "提升自身魔抗", true, true, false, 1, "BuffMagicResAdd", "", "", "SparkleAreaWhite", "", "");
            config[300014] = new BuffConfig(300014, "生命回复", "血", "提升每秒生命回复", true, true, false, 1, "BuffHpRegenAdd", "", "", "MagicBuffGreen", "", "");
            config[300015] = new BuffConfig(300015, "闪避", "闪", "提升闪避几率", true, true, false, 1, "BuffEvasion", "", "", "StunnedCirclingShadowSimple", "", "");
            config[300016] = new BuffConfig(300016, "法强提升", "术", "提升自身法术强度", true, true, false, 1, "BuffApAdd", "", "", "MagicBuffBlue", "", "");
            config[300018] = new BuffConfig(300018, "法力回复", "蓝", "提升每秒法力回复", true, true, false, 1, "BuffMpRegenAdd", "", "", "MagicBuffBlue", "", "");
            config[300019] = new BuffConfig(300019, "暴击提升", "暴", "提升暴击率", true, true, false, 1, "BuffCritRate", "", "", "", "", "");
            config[300025] = new BuffConfig(300025, "反伤", "反", "受到伤害时将其中一部分返还给攻击者", true, true, false, 1, "BuffReflect", "", "", "", "", "");
            config[300030] = new BuffConfig(300030, "隐匿", "隐", "敌方索敌与单体技能无法锁定自身，单位呈半透明", true, true, false, 4, "BuffStealth", "", "", "", "BuffStealth", "");
            config[300101] = new BuffConfig(300101, "据守", "守", "获得临时双防，期间每次受击再叠一层双防，状态结束后临时双防消失", true, false, false, 4, "BuffDefStack", "", "", "", "", "");
            config[300102] = new BuffConfig(300102, "复活", "复", "死亡后3秒原地复活(仅一次)", true, false, false, 4, "BuffRevive", "", "", "", "", "");
            config[300103] = new BuffConfig(300103, "乱阵", "威", "携带者普攻按概率眩晕目标", true, false, false, 4, "BuffHitStun", "", "", "", "", "");
            config[300104] = new BuffConfig(300104, "多重箭", "箭", "攻击时额外射出多支箭", true, true, false, 4, "BuffMultiShot", "", "", "", "", "");
            config[300106] = new BuffConfig(300106, "生命链接", "链", "与链接方共享伤害与回复", true, false, false, 4, "BuffLifeLink", "", "", "", "", "");
            config[300107] = new BuffConfig(300107, "望族", "望", "提升/strength点护甲、/strength2点魔抗，每3秒衰减1/5，15秒后归零", true, true, false, 4, "BuffDecayDef", "", "", "", "", "");
            config[300108] = new BuffConfig(300108, "兼资", "兼", "攻击叠层：每层提升/strength攻击、/strength2法术强度，持续攻击刷新，停手后层数清零", true, true, false, 4, "BuffStackBuf", "", "", "", "", "");
            config[300109] = new BuffConfig(300109, "威震", "震", "携带者普攻按概率眩晕目标，且普攻转为真实伤害", true, false, false, 4, "BuffVengefulStance", "#0000CC", "#660099", "", "", "");
            config[300128] = new BuffConfig(300128, "护驾", "护", "受到的回复效果提升/strength%", true, false, false, 4, "BuffHealBoost", "", "", "", "", "");
            config[301001] = new BuffConfig(301001, "眩晕", "乱", "眩晕，无法行动", false, true, true, 3, "BuffNoAction", "", "", "StunnedCirclingStarsSimple", "", "");
            config[301003] = new BuffConfig(301003, "增伤", "伤", "受到的伤害按比例增加", false, true, false, 1, "BuffDamagedAddRate", "", "", "StunnedDamageUp", "", "");
            config[301004] = new BuffConfig(301004, "减攻", "慑", "降低目标攻击力", false, true, false, 1, "BuffAtkDown", "", "", "Misses", "", "");
            config[301005] = new BuffConfig(301005, "陷阵", "停", "无法移动", false, true, false, 3, "BuffNoMove", "", "", "", "BuffFrozen", "");
            config[301006] = new BuffConfig(301006, "溃败", "败", "持续受到随时间结算的伤害", false, true, false, 1, "BuffTimeDamage", "", "", "BloodExplosion", "", "");
            config[301007] = new BuffConfig(301007, "减疗", "疫", "降低目标受到的治疗效果", false, true, false, 1, "BuffHealDown", "", "", "MissesGreen", "", "");
            config[301008] = new BuffConfig(301008, "减速", "缓", "降低目标移动与攻速", false, true, false, 3, "BuffSlowDown", "", "", "MissesBlue", "", "");
            config[301009] = new BuffConfig(301009, "减防", "破", "降低目标护甲", false, true, false, 1, "BuffArmorDown", "", "", "MissesBlue", "", "");
            config[301010] = new BuffConfig(301010, "嘲讽", "嘲", "成为敌对单位强制优先攻击的目标", false, true, true, 3, "BuffTaunt", "", "", "Character_status_taunt", "", "");
            config[301011] = new BuffConfig(301011, "叛逃", "叛", "不受控制，向初始位置后退，移动速度减半", false, true, true, 3, "BuffDefect", "", "", "Character_status_sleep", "", "");
            config[301012] = new BuffConfig(301012, "混乱", "惑", "不受控制，攻击最近的友方单位", false, true, true, 3, "BuffChaos", "", "", "Character_status_fight", "", "");
            config[301102] = new BuffConfig(301102, "连锁", "锁", "被攻击时，向范围内同样带连锁的友军传递同比例伤害", false, true, false, 4, "BuffLock", "", "", "StunnedLock", "", "");

            RebuildIndex();

        }

        private static void RebuildIndex()
        {
            idxNameS.Clear();
            foreach (var kv in config)
            {
                if (!string.IsNullOrEmpty(kv.Value.NameS)) idxNameS[kv.Value.NameS] = kv.Key;
            }
        }

        public static BuffConfig GetConfig(int id)
        {
            BuffConfig data;
            if (config.TryGetValue(id, out data))
            {
                return data;
            }
            throw new NullReferenceException(string.Format("配置表BuffConfig不存在id={0}", id));
        }

        private static Dictionary<string, int> idxNameS = new Dictionary<string, int>();
        public static BuffConfig GetConfigByNameS(string val)
        {
            return GetConfig(idxNameS[val]);
        }


        public static bool HasConfig(int id)
        {
            if (config.ContainsKey(id))
            {
                return true;
            }
            return false;
        }

        public static void Assign(int id, BuffConfig configData)
        {
            config[id] = configData; 
        }

        public static void Add(int id, BuffConfig configData)
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
