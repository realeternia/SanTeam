﻿using System;
using System.Collections;
using System.Collections.Generic;

namespace CommonConfig
{
    public class EffectConfig
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
            {"Name", new FieldMetaInfo("特效名（外部引用 key，即 HitEffect 值）", "string", 188, "", true)},
            {"Cname", new FieldMetaInfo("中文名", "string", 171)},
            {"Type", new FieldMetaInfo("类型（增益/负面/伤害/正面）", "string", 80, "增益:#2F7D4E,负面:#8F3A34,伤害:#555B60,正面:#2F7F78")},
            {"Area", new FieldMetaInfo("作用范围（自身/目标/范围/Buff/光环）", "string", 80, "自身:#2F7D4E,目标:#355F8F,范围:#A06428,Buff:#6B4A8F,光环:#4C9C95")},
            {"EffPath", new FieldMetaInfo("特效资源路径（省略 Prefabs/ 前缀，如 Effect/xxx，对应 Prefabs/Effect 下资源，无则留空）", "string", 210)},
            {"MissilePath", new FieldMetaInfo("导弹资源路径（省略 Prefabs/ 前缀，如 Missile/xxx，对应 Prefabs/Missile 下资源，无则留空）", "string", 245)},
            {"Scale", new FieldMetaInfo("特效世界缩放（挂载后实际大小，与挂点无关，默认取 prefab 根缩放）", "float", 60)},
            {"OffsetY", new FieldMetaInfo("特效世界Y偏移（挂载后相对单位的高度，默认1）", "float", 60)},
            {"MissileScale", new FieldMetaInfo("导弹世界缩放（挂载后实际大小，与挂点无关，未配置时取 Scale）", "float", 60)},
            {"MissileOffsetY", new FieldMetaInfo("导弹世界Y偏移（未配置时取 OffsetY）", "float", 60)},
            {"TintColor", new FieldMetaInfo("特效变色（#RRGGBB 十六进制，同 PlayerConfig.Colorstr 格式，如 #00FF00 绿色；配置后创建时自动染色，可复用同形态特效）", "string", 150)},
        };

        public static Dictionary<string, FieldMetaInfo> FieldMeta { get { return fieldMeta; } }

        private static List<CellMeta> cellMeta = new List<CellMeta>();
        public static List<CellMeta> CellMetas { get { return cellMeta; } }

        /// <summary>
        ///序列
        /// </summary>
        public int Id;
        /// <summary>
        ///特效名（外部引用 key，即 HitEffect 值）
        /// </summary>
        public string Name;
        /// <summary>
        ///中文名
        /// </summary>
        public string Cname;
        /// <summary>
        ///类型（增益/负面/伤害）
        /// </summary>
        public string Type;
        /// <summary>
        ///作用范围（自身/目标/范围/Buff/光环）
        /// </summary>
        public string Area;
        /// <summary>
        ///特效资源路径（省略 Prefabs/ 前缀，如 Effect/xxx，对应 Prefabs/Effect 下资源，无则留空）
        /// </summary>
        public string EffPath;
        /// <summary>
        ///导弹资源路径（省略 Prefabs/ 前缀，如 Missile/xxx，对应 Prefabs/Missile 下资源，无则留空）
        /// </summary>
        public string MissilePath;
        /// <summary>
        ///特效世界缩放（挂载后实际大小，与挂点无关，默认取 prefab 根缩放）
        /// </summary>
        public float Scale;
        /// <summary>
        ///特效世界Y偏移（挂载后相对单位的高度，默认1）
        /// </summary>
        public float OffsetY;
        /// <summary>
        ///导弹世界缩放（挂载后实际大小，与挂点无关，未配置时取 Scale）
        /// </summary>
        public float MissileScale;
        /// <summary>
        ///导弹世界Y偏移（未配置时取 OffsetY）
        /// </summary>
        public float MissileOffsetY;
        /// <summary>
        ///特效变色（#RRGGBB 十六进制，同 PlayerConfig.Colorstr 格式，如 #00FF00 绿色；配置后创建时自动染色，可复用同形态特效）
        /// </summary>
        public string TintColor;


        public EffectConfig(int Id, string Name, string Cname, string Type, string Area, string EffPath, string MissilePath, float Scale, float OffsetY, float MissileScale, float MissileOffsetY, string TintColor)
        {
            this.Id = Id;
            this.Name = Name;
            this.Cname = Cname;
            this.Type = Type;
            this.Area = Area;
            this.EffPath = EffPath;
            this.MissilePath = MissilePath;
            this.Scale = Scale;
            this.OffsetY = OffsetY;
            this.MissileScale = MissileScale;
            this.MissileOffsetY = MissileOffsetY;
            this.TintColor = TintColor;
        }

        public EffectConfig() { }

        private static Dictionary<int, EffectConfig> config = new Dictionary<int, EffectConfig>();
        public static Dictionary<int, EffectConfig>.ValueCollection ConfigList
        {
            get { return config.Values; }
        }

        public static void Refresh(Dictionary<int, EffectConfig> dict)
        {
            config.Clear();
            config = dict;
            RebuildIndex();
        }

        public static void Load()
        {
            config.Clear();
            config[10000] = new EffectConfig(10000, "BulletExplosionBlue", "蓝色子弹爆裂特效", "伤害", "目标", "Effect/BulletExplosionBlue", "Missile/BulletExplosionBlue", 8f, 1f, 12f, 1f, "");
            config[10001] = new EffectConfig(10001, "BulletExplosionFire", "火焰子弹爆裂特效", "伤害", "目标", "Effect/BulletExplosionBlue", "Missile/BulletExplosionFire", 8f, 1f, 12f, 1f, "#FF7A00");
            config[10002] = new EffectConfig(10002, "BulletExplosionGreen", "绿色子弹爆裂特效", "伤害", "目标", "Effect/BulletExplosionBlue", "Missile/BulletExplosionGreen", 8f, 1f, 12f, 1f, "#A7FF00");
            config[10003] = new EffectConfig(10003, "BulletExplosionStone", "石块爆裂特效（猿猴投石）", "伤害", "目标", "Effect/BulletExplosionBlue", "Missile/BulletExplosionBlue", 8f, 1f, 12f, 1f, "#9E9E9E");
            config[10010] = new EffectConfig(10010, "ExplosionFireballFire", "火球爆炸特效", "伤害", "目标", "Effect/ExplosionFireballFire", "Missile/ExplosionFireballFire", 5f, 1f, 8f, 1f, "");
            config[10020] = new EffectConfig(10020, "FanExplosion", "扇子爆裂特效", "伤害", "目标", "Effect/FanExplosion", "Missile/FanExplosion", 5f, 1f, 8f, 1f, "");
            config[10030] = new EffectConfig(10030, "FrostExplosionBlue", "蓝色冰冻爆裂特效", "伤害", "目标", "Effect/FrostExplosionBlue", "Missile/FrostExplosionBlue", 5f, 1f, 8f, 1f, "");
            config[10040] = new EffectConfig(10040, "GasExplosionFire", "火焰毒气爆裂特效", "伤害", "目标", "Effect/GasExplosionFire", "Missile/GasExplosionFire", 2.5f, 1f, 8f, 1f, "");
            config[10041] = new EffectConfig(10041, "GasShootFire", "火焰喷射弹道特效", "伤害", "目标", "Effect/GasExplosionFire", "Missile/GasShootFire", 5f, 1f, 8f, 1f, "");
            config[10050] = new EffectConfig(10050, "LightningExplosionBlue", "蓝色闪电爆裂特效", "伤害", "目标", "Effect/LightningExplosionBlue", "Missile/LightningExplosionBlue", 3f, 1f, 5f, 1f, "");
            config[10051] = new EffectConfig(10051, "LightningExplosionYellow", "黄色闪电爆裂特效", "伤害", "目标", "Effect/LightningExplosionBlue", "Missile/LightningExplosionYellow", 3f, 1f, 5f, 1f, "#FFD800");
            config[10060] = new EffectConfig(10060, "SharpExplosionGreen", "绿色利刃爆裂特效", "伤害", "目标", "Effect/SharpExplosionGreen", "Missile/SharpExplosionGreen", 5f, 1f, 5f, 1f, "");
            config[10070] = new EffectConfig(10070, "SoulExplosionOrange", "橙色灵魂爆裂特效", "伤害", "目标", "Effect/SoulExplosionOrange", "Missile/SoulExplosionOrange", 5f, 1f, 8f, 1f, "");
            config[10080] = new EffectConfig(10080, "StormExplosion", "风暴爆裂特效", "伤害", "目标", "Effect/StormExplosion", "Missile/StormExplosion", 8f, 1f, 8f, 1f, "");
            config[10081] = new EffectConfig(10081, "SwordWaveBlue", "蓝色冲击波", "伤害", "目标", "Effect/StormExplosion", "Missile/SwordWaveBlue", 5f, 1f, 3.4f, 1f, "");
            config[10090] = new EffectConfig(10090, "ToolExplosion", "锤类爆裂特效", "伤害", "目标", "Effect/ToolExplosion", "Missile/ToolExplosion", 5f, 1f, 8f, 1f, "");
            config[10100] = new EffectConfig(10100, "AxeExplosion", "斧类爆裂特效（弹道）", "伤害", "目标", "Effect/SwordHitBlue", "Missile/AxeExplosion", 5f, 1f, 8f, 1f, "");
            config[10110] = new EffectConfig(10110, "NukeMissileFires", "核弹导弹弹道特效", "伤害", "目标", "Effect/top_down_lightning_dot_orange", "Missile/NukeMissileFires", 5f, 1f, 5f, 1f, "#00D5FF");
            config[10120] = new EffectConfig(10120, "ShadowExplosion", "暗影爆裂特效", "伤害", "目标", "Effect/ShadowExplosion", "Missile/ShadowExplosion", 5f, 1f, 8f, 1f, "");
            config[10121] = new EffectConfig(10121, "ShadowExplosionGreen", "绿色暗影爆裂特效", "伤害", "目标", "Effect/ShadowExplosion", "Missile/ShadowExplosionGreen", 5f, 1f, 8f, 1f, "#105E11");
            config[10130] = new EffectConfig(10130, "FlashExplosionBlue", "蓝色星爆", "伤害", "目标", "Effect/FlashExplosionBlue", "Missile/FlashExplosionBlue", 5f, 1f, 5f, 1f, "");
            config[10131] = new EffectConfig(10131, "LaserMeshMissileBlue", "蓝色星爆", "伤害", "目标", "Effect/FlashExplosionBlue", "Missile/LaserMeshMissileBlue", 5f, 1f, 5f, 1f, "");
            config[10140] = new EffectConfig(10140, "Par_FireShoot_Bullet", "火焰射击", "伤害", "目标", "Effect/Par_FireShoot_Hit", "Missile/Par_FireShoot_Bullet", 1.5f, 1f, 3f, 1f, "");
            config[11000] = new EffectConfig(11000, "LightningExplosionRed", "红色闪电爆裂特效", "伤害", "目标", "Effect/LightningExplosionBlue", "", 3f, 1f, 5f, 1f, "#FF1C00");
            config[11010] = new EffectConfig(11010, "MagicBuffGreen", "绿色增益特效", "增益", "目标", "Effect/MagicBuffGreen", "", 8f, 1f, 8f, 1f, "");
            config[11011] = new EffectConfig(11011, "MagicBuffBlue", "蓝色增益特效", "增益", "目标", "Effect/MagicBuffGreen", "", 8f, 1f, 8f, 1f, "#3333FF");
            config[11020] = new EffectConfig(11020, "SwordSlashMiniWhite", "白色迷你斩击特效", "伤害", "目标", "Effect/SwordSlashMiniWhite", "", 5f, 1f, 8f, 1f, "");
            config[11030] = new EffectConfig(11030, "SwordHitBlackRedCritical", "黑红暴击剑击特效", "伤害", "目标", "Effect/SwordHitYellowCritical", "", 5f, 1f, 8f, 1f, "#A60B0B");
            config[11031] = new EffectConfig(11031, "SwordHitGreenCritical", "绿色暴击剑击特效", "伤害", "目标", "Effect/SwordHitYellowCritical", "", 5f, 1f, 8f, 1f, "#00FF1B");
            config[11032] = new EffectConfig(11032, "SwordHitRedCritical", "红色暴击剑击特效", "伤害", "目标", "Effect/SwordHitYellowCritical", "", 5f, 1f, 8f, 1f, "#FF0000");
            config[11033] = new EffectConfig(11033, "SwordHitWhiteCritical", "白色暴击剑击特效", "伤害", "目标", "Effect/SwordHitYellowCritical", "", 5f, 1f, 8f, 1f, "#FFFFFF");
            config[11034] = new EffectConfig(11034, "SwordHitYellowCritical", "黄色暴击剑击特效", "伤害", "目标", "Effect/SwordHitYellowCritical", "", 5f, 1f, 8f, 1f, "#FFFF00");
            config[11040] = new EffectConfig(11040, "SwordHitBlue", "蓝色剑击特效", "伤害", "目标", "Effect/SwordHitBlue", "", 10f, 1f, 8f, 1f, "");
            config[11041] = new EffectConfig(11041, "LaserImpactSpark", "激光命中火花特效", "伤害", "目标", "Effect/SwordHitBlue", "", 2f, 1f, 5f, 1f, "#00D5FF");
            config[11050] = new EffectConfig(11050, "StarExplosionBlue", "蓝色星爆裂特效", "增益", "目标", "Effect/StarExplosionBlue", "", 8f, 1f, 5f, 1f, "");
            config[11060] = new EffectConfig(11060, "StunExplosion", "击晕爆裂特效", "伤害", "目标", "Effect/StunExplosion", "", 7f, 1f, 5f, 1f, "");
            config[11070] = new EffectConfig(11070, "Wet", "蓝色下雨效果", "负面", "目标", "Effect/Wet", "", 6f, 1f, 5f, 1f, "");
            config[11080] = new EffectConfig(11080, "top_down_lightning_dot_blue", "攻击命中", "负面", "目标", "Effect/top_down_lightning_dot_orange", "", 7f, .5f, 5f, 1f, "#00D5FF");
            config[11090] = new EffectConfig(11090, "top_down_ice_dot", "攻击命中", "负面", "目标", "Effect/top_down_ice_dot", "", 5f, .5f, 5f, 1f, "");
            config[12000] = new EffectConfig(12000, "MagicFieldGreen", "绿色法阵特效", "增益", "范围", "Effect/MagicFieldGreen", "", 8f, 1f, 8f, 1f, "");
            config[12010] = new EffectConfig(12010, "SlowAuraYellow", "黄色迟缓光环特效", "负面", "范围", "Effect/SlowAuraYellow", "", 3f, 2f, 8f, 1f, "");
            config[12020] = new EffectConfig(12020, "SoftFireBigRed", "红色大火特效", "伤害", "范围", "Effect/SoftFireBigRed", "", 10f, 1f, 8f, 1f, "");
            config[12030] = new EffectConfig(12030, "SparkleAreaWhite", "白色闪耀区域特效", "增益", "范围", "Effect/SparkleAreaWhite", "", 10f, 1f, 8f, 1f, "");
            config[12040] = new EffectConfig(12040, "SummonStorm", "召唤风暴特效", "伤害", "范围", "Effect/SummonStorm", "", 1.2f, 1f, 8f, 1f, "");
            config[12050] = new EffectConfig(12050, "MagicNovaBlue", "蓝色新星特效", "伤害", "范围", "Effect/MagicNovaBlue", "", 4f, 1f, 8f, 1f, "");
            config[12051] = new EffectConfig(12051, "MagicNovaYellow", "黄色新星特效", "伤害", "范围", "Effect/MagicNovaBlue", "", 4f, 1f, 8f, 1f, "#FFBB00");
            config[12060] = new EffectConfig(12060, "SwordWhirlwindWhite", "白色剑旋特效", "伤害", "范围", "Effect/SwordWhirlwindWhite", "", 4f, 1f, 8f, 1f, "");
            config[12070] = new EffectConfig(12070, "MysticExplosionDark", "紫色神秘爆裂特效AOE", "伤害", "范围", "Effect/MysticExplosionDark", "", 3.5f, 1f, 5f, 1f, "");
            config[12071] = new EffectConfig(12071, "MysticExplosionRed", "红色神秘爆裂特效AOE", "伤害", "范围", "Effect/MysticExplosionDark", "", 3.5f, 1f, 5f, 1f, "#FF0000");
            config[12080] = new EffectConfig(12080, "NukeVerticalExplosionFire", "核弹垂直爆裂特效AOE", "伤害", "范围", "Effect/NukeVerticalExplosionFire", "", 2.8f, 1f, 5f, 1f, "");
            config[12090] = new EffectConfig(12090, "ScanExplosion", "地震爆裂特效", "伤害", "范围", "Effect/ScanExplosion", "", 3f, 1f, 5f, 1f, "");
            config[12100] = new EffectConfig(12100, "PoisonExplosionSoft", "中毒爆裂特效", "伤害", "范围", "Effect/PoisonExplosionSoft", "", 7f, 1f, 5f, 1f, "");
            config[12101] = new EffectConfig(12101, "PoisonExplosionSoftYellow", "沙尘爆裂特效", "伤害", "范围", "Effect/PoisonExplosionSoft", "", 7f, 1f, 5f, 1f, "#CC6600");
            config[12102] = new EffectConfig(12102, "PoisonExplosionSoftPurple", "紫色迷雾爆裂特效", "伤害", "范围", "Effect/PoisonExplosionSoft", "", 7f, 1f, 5f, 1f, "#550066");
            config[12110] = new EffectConfig(12110, "MagicAuraBlue", "蓝色魔法区域特效", "增益", "范围", "Effect/MagicAuraBlue", "", 4f, 1f, 5f, 1f, "");
            config[12111] = new EffectConfig(12111, "MagicAuraYellow", "黄色魔法区域特效", "增益", "范围", "Effect/MagicAuraBlue", "", 4f, 1f, 5f, 1f, "#fbff00ff");
            config[12120] = new EffectConfig(12120, "top_down_starfall_circle_rainbow", "星落效果，aoe", "负面", "范围", "Effect/top_down_starfall_circle_rainbow", "", 2f, .5f, 5f, 1f, "");
            config[12130] = new EffectConfig(12130, "top_down_stone_circle", "攻击命中", "负面", "范围", "Effect/top_down_stone_circle", "", 2f, .5f, 5f, 1f, "");
            config[12140] = new EffectConfig(12140, "FlamethrowerSpikyRed", "攻击命中", "负面", "范围", "Effect/FlamethrowerSpikyRed", "", 8.5f, .5f, 5f, 1f, "");
            config[12150] = new EffectConfig(12150, "AuraSoftPurple", "紫色缓冲特效", "增益", "范围", "Effect/AuraSoftPurple", "", 3f, 1f, 8f, 1f, "");
            config[13000] = new EffectConfig(13000, "BloodExplosion", "鲜血爆裂特效", "伤害", "Buff", "Effect/BloodExplosion", "", 3f, 1f, 8f, 1f, "");
            config[13010] = new EffectConfig(13010, "HeartStream", "爱心流光特效", "增益", "Buff", "Effect/HeartStream", "", 8f, 1f, 8f, 1f, "");
            config[13020] = new EffectConfig(13020, "ShieldSoftBlue", "蓝色护盾特效", "增益", "Buff", "Effect/ShieldSoftBlue", "", 3f, 1f, 8f, 1f, "");
            config[13021] = new EffectConfig(13021, "ShieldSoftYellow", "黄色护盾特效", "增益", "Buff", "Effect/ShieldSoftBlue", "", 3f, 1f, 8f, 1f, "#FFBB09");
            config[13030] = new EffectConfig(13030, "StunnedCirclingStarsSimple", "眩晕环绕星特效", "负面", "Buff", "Effect/StunnedCirclingStarsSimple", "", 3f, 1f, 8f, 1f, "");
            config[13040] = new EffectConfig(13040, "StunnedDamageUp", "眩晕增伤特效", "负面", "Buff", "Effect/StunnedDamageUp", "", 3f, 1f, 8f, 1f, "");
            config[13050] = new EffectConfig(13050, "StunnedLock", "定身眩晕特效", "负面", "Buff", "Effect/StunnedLock", "", 3f, 1f, 8f, 1f, "");
            config[13060] = new EffectConfig(13060, "Misses", "红色烟雾效果", "负面", "Buff", "Effect/Misses", "", 8f, 1f, 5f, 1f, "");
            config[13061] = new EffectConfig(13061, "MissesBlue", "蓝色烟雾效果", "负面", "Buff", "Effect/Misses", "", 8f, 1f, 5f, 1f, "#3333FF");
            config[13062] = new EffectConfig(13062, "MissesGreen", "绿色烟雾效果", "负面", "Buff", "Effect/Misses", "", 8f, 1f, 5f, 1f, "#00FF1B");
            config[13070] = new EffectConfig(13070, "Character_status_sleep", "sleep", "负面", "Buff", "Effect/Character_status_sleep", "", 8f, 1f, 5f, 1f, "#3333FF");
            config[13080] = new EffectConfig(13080, "Character_status_fight", "好战", "负面", "Buff", "Effect/Character_status_fight", "", 8f, 1f, 5f, 1f, "");
            config[13090] = new EffectConfig(13090, "Character_status_taunt", "嘲讽", "负面", "Buff", "Effect/Character_status_taunt", "", 8f, 1f, 5f, 1f, "");
            config[13100] = new EffectConfig(13100, "StunnedCirclingShadowSimple", "闪避", "负面", "Buff", "Effect/StunnedCirclingShadowSimple", "", 4.5f, .5f, 5f, 1f, "");
            config[13110] = new EffectConfig(13110, "CirclingArmorJia", "金色护甲甲字环绕特效", "增益", "Buff", "Effect/CirclingArmorJia", "", 3f, 1f, 8f, 1f, "#00D5FF");
            config[13120] = new EffectConfig(13120, "CirclingMagicResKang", "魔抗提升抗字环绕特效", "增益", "Buff", "Effect/CirclingMagicResKang", "", 3f, 1f, 8f, 1f, "#6FA8FF");
            config[13130] = new EffectConfig(13130, "CirclingAtkGong", "攻击提升攻字环绕特效", "增益", "Buff", "Effect/CirclingAtkGong", "", 3f, 1f, 8f, 1f, "#FF4A3D");
            config[13140] = new EffectConfig(13140, "CirclingDamageShang", "伤害提升伤字环绕特效", "增益", "Buff", "Effect/CirclingDamageShang", "", 3f, 1f, 8f, 1f, "#FF7A1A");
            config[13150] = new EffectConfig(13150, "CirclingMoveSpeedYi", "移速提升移字环绕特效", "增益", "Buff", "Effect/CirclingMoveSpeedYi", "", 3f, 1f, 8f, 1f, "#4DFF9E");
            config[13160] = new EffectConfig(13160, "CirclingAtkSpeedSu", "攻速提升速字环绕特效", "增益", "Buff", "Effect/CirclingAtkSpeedSu", "", 3f, 1f, 8f, 1f, "#FFD84D");
            config[13170] = new EffectConfig(13170, "CirclingLifestealXi", "吸血吸字环绕特效", "增益", "Buff", "Effect/CirclingLifestealXi", "", 3f, 1f, 8f, 1f, "#D92B4B");
            config[13180] = new EffectConfig(13180, "CirclingApFa", "法强提升法字环绕特效", "增益", "Buff", "Effect/CirclingApFa", "", 3f, 1f, 8f, 1f, "#B36BFF");
            config[13190] = new EffectConfig(13190, "CirclingCritBao", "暴击提升暴字环绕特效", "增益", "Buff", "Effect/CirclingCritBao", "", 3f, 1f, 8f, 1f, "#FFB020");
            config[13200] = new EffectConfig(13200, "CirclingSpellCritJue", "法术暴击绝字环绕特效", "增益", "Buff", "Effect/CirclingSpellCritJue", "", 3f, 1f, 8f, 1f, "#FF4DD2");
            config[13210] = new EffectConfig(13210, "CirclingReflectCi", "反伤六刺环绕特效", "增益", "Buff", "Effect/CirclingReflectCi", "", 3f, 1f, 8f, 1f, "");
            config[13220] = new EffectConfig(13220, "RisingArrowGreen", "绿色上升箭头特效（属性强化）", "增益", "Buff", "Effect/RisingArrowGreen", "", 3f, 1f, 8f, 1f, "");
            config[14000] = new EffectConfig(14000, "Par_LargeMagicCircle", "蓝色光环", "正面", "光环", "Effect/Par_LargeMagicCircle", "", 1.5f, .5f, 5f, 1f, "");
            config[14001] = new EffectConfig(14001, "Par_LargeMagicCircleGreen", "绿色光环", "正面", "光环", "Effect/Par_LargeMagicCircle", "", 1.5f, .5f, 5f, 1f, "#00FF1B");
            config[14010] = new EffectConfig(14010, "Par_HolyMagicCircle", "金色光环", "正面", "光环", "Effect/Par_HolyMagicCircle", "", 1.5f, .5f, 5f, 1f, "");
            config[14020] = new EffectConfig(14020, "Par_PurpleField", "紫色光环", "正面", "光环", "Effect/Par_PurpleField", "", 4f, .5f, 5f, 1f, "");
            config[14030] = new EffectConfig(14030, "Par_FireField", "红色光环", "正面", "光环", "Effect/Par_FireField", "", 4.5f, .5f, 5f, 1f, "");
            config[14031] = new EffectConfig(14031, "Par_FireFieldRed", "红色光环", "正面", "光环", "Effect/Par_FireField", "", 4.5f, .5f, 5f, 1f, "#FF0000");
            config[15000] = new EffectConfig(15000, "SelfPrismSnapBlue", "自身特效-棱镜闪·蓝（群体治疗）", "增益", "自身", "Effect/SelfPrismSnap", "", 3f, 1f, 8f, 1f, "#4AA8FF");
            config[15001] = new EffectConfig(15001, "SelfPrismSnapCyan", "自身特效-棱镜闪·青（单体治疗）", "增益", "自身", "Effect/SelfPrismSnap", "", 3f, 1f, 8f, 1f, "#4DE8E1");
            config[15002] = new EffectConfig(15002, "SelfPrismSnapPink", "自身特效-棱镜闪·粉（辅助/防御）", "增益", "自身", "Effect/SelfPrismSnap", "", 3f, 1f, 8f, 1f, "#FF7BD5");
            config[15010] = new EffectConfig(15010, "SelfArcaneOrbitBlue", "自身特效-奥术环·蓝（法术群体伤害）", "增益", "自身", "Effect/SelfArcaneOrbit", "", 3f, 1f, 8f, 1f, "#4AA8FF");
            config[15011] = new EffectConfig(15011, "SelfArcaneOrbitRed", "自身特效-奥术环·红（法术单体伤害）", "增益", "自身", "Effect/SelfArcaneOrbit", "", 3f, 1f, 8f, 1f, "#FF4444");
            config[15012] = new EffectConfig(15012, "SelfArcaneOrbitYellow", "自身特效-奥术环·土黄（法术控制）", "增益", "自身", "Effect/SelfArcaneOrbit", "", 3f, 1f, 8f, 1f, "#C9A227");
            config[15020] = new EffectConfig(15020, "SelfBuffPulseGreen", "自身特效-强化脉冲·绿（群体增益）", "增益", "自身", "Effect/SelfBuffPulse", "", 3f, 1f, 8f, 1f, "#4DFF9E");
            config[15021] = new EffectConfig(15021, "SelfBuffPulseYellow", "自身特效-强化脉冲·黄（单体增益）", "增益", "自身", "Effect/SelfBuffPulse", "", 3f, 1f, 8f, 1f, "#FFD24D");
            config[15022] = new EffectConfig(15022, "SelfBuffPulseOrange", "自身特效-强化脉冲·橙（自身/召唤强化）", "增益", "自身", "Effect/SelfBuffPulse", "", 3f, 1f, 8f, 1f, "#FF8A3D");
            config[15030] = new EffectConfig(15030, "MagicChargeRed", "自身特效-蓄力·红（物理单体伤害）", "增益", "自身", "Effect/MagicChargeBlue", "", 3f, 1f, 8f, 1f, "#FF3B3B");
            config[15031] = new EffectConfig(15031, "MagicChargeGold", "自身特效-蓄力·金（物理群体伤害）", "增益", "自身", "Effect/MagicChargeBlue", "", 3f, 1f, 8f, 1f, "#FFC93C");
            config[15032] = new EffectConfig(15032, "MagicChargePurple", "自身特效-蓄力·紫（物理控制）", "增益", "自身", "Effect/MagicChargeBlue", "", 3f, 1f, 8f, 1f, "#A96BFF");

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

        public static EffectConfig GetConfig(int id)
        {
            EffectConfig data;
            if (config.TryGetValue(id, out data))
            {
                return data;
            }
            throw new NullReferenceException(string.Format("配置表EffectConfig不存在id={0}", id));
        }

        private static Dictionary<string, int> idxName = new Dictionary<string, int>();
        public static EffectConfig GetConfigByName(string val)
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

        public static void Assign(int id, EffectConfig configData)
        {
            config[id] = configData; 
        }

        public static void Add(int id, EffectConfig configData)
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
