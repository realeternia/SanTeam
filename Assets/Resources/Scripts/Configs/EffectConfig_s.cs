using System;
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
            {"Type", new FieldMetaInfo("类型（增益/负面/伤害）", "string", 80)},
            {"Area", new FieldMetaInfo("作用范围（目标/范围）", "string", 80)},
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
        ///作用范围（目标/范围）
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
            config[10010] = new EffectConfig(10010, "ExplosionFireballFire", "火球爆炸特效", "伤害", "目标", "Effect/ExplosionFireballFire", "Missile/ExplosionFireballFire", 5f, 1f, 8f, 1f, "");
            config[10020] = new EffectConfig(10020, "FanExplosion", "扇子爆裂特效", "伤害", "目标", "Effect/FanExplosion", "Missile/FanExplosion", 5f, 1f, 8f, 1f, "");
            config[10030] = new EffectConfig(10030, "FrostExplosionBlue", "蓝色冰冻爆裂特效", "伤害", "目标", "Effect/FrostExplosionBlue", "Missile/FrostExplosionBlue", 5f, 1f, 8f, 1f, "");
            config[10040] = new EffectConfig(10040, "GasExplosionFire", "火焰毒气爆裂特效", "伤害", "目标", "Effect/GasExplosionFire", "Missile/GasExplosionFire", 2.5f, 1f, 8f, 1f, "");
            config[10050] = new EffectConfig(10050, "LightningExplosionBlue", "蓝色闪电爆裂特效", "伤害", "目标", "Effect/LightningExplosionBlue", "Missile/LightningExplosionBlue", 3f, 1f, 5f, 1f, "");
            config[10051] = new EffectConfig(10051, "LightningExplosionYellow", "黄色闪电爆裂特效", "伤害", "目标", "Effect/LightningExplosionBlue", "Missile/LightningExplosionYellow", 3f, 1f, 5f, 1f, "#FFD800");
            config[10060] = new EffectConfig(10060, "SharpExplosionGreen", "绿色利刃爆裂特效", "伤害", "目标", "Effect/SharpExplosionGreen", "Missile/SharpExplosionGreen", 5f, 1f, 5f, 1f, "");
            config[10070] = new EffectConfig(10070, "SoulExplosionOrange", "橙色灵魂爆裂特效", "伤害", "目标", "Effect/SoulExplosionOrange", "Missile/SoulExplosionOrange", 5f, 1f, 8f, 1f, "");
            config[10080] = new EffectConfig(10080, "StormExplosion", "风暴爆裂特效", "伤害", "范围", "Effect/StormExplosion", "Missile/StormExplosion", 8f, 1f, 8f, 1f, "");
            config[10090] = new EffectConfig(10090, "ToolExplosion", "锤类爆裂特效", "伤害", "目标", "Effect/ToolExplosion", "Missile/ToolExplosion", 5f, 1f, 8f, 1f, "");
            config[10100] = new EffectConfig(10100, "AxeExplosion", "斧类爆裂特效（弹道）", "伤害", "目标", "", "Missile/AxeExplosion", 5f, 1f, 8f, 1f, "");
            config[10110] = new EffectConfig(10110, "GasShootFire", "火焰喷射弹道特效", "伤害", "目标", "", "Missile/GasShootFire", 5f, 1f, 8f, 1f, "");
            config[10120] = new EffectConfig(10120, "NukeMissileFires", "核弹导弹弹道特效", "伤害", "目标", "", "Missile/NukeMissileFires", 5f, 1f, 5f, 1f, "#00D5FF");
            config[10130] = new EffectConfig(10130, "ShadowExplosion", "暗影爆裂特效", "伤害", "目标", "Effect/ShadowExplosion", "Missile/ShadowExplosion", 5f, 1f, 8f, 1f, "");
            config[10131] = new EffectConfig(10131, "ShadowExplosionGreen", "绿色暗影爆裂特效", "伤害", "目标", "Effect/ShadowExplosion", "Missile/ShadowExplosionGreen", 5f, 1f, 8f, 1f, "#105E11");
            config[10140] = new EffectConfig(10140, "FlashExplosionBlue", "蓝色星爆", "伤害", "目标", "Effect/FlashExplosionBlue", "Missile/FlashExplosionBlue", 5f, 1f, 5f, 1f, "");
            config[10150] = new EffectConfig(10150, "LaserMeshMissileBlue", "蓝色星爆", "伤害", "目标", "Effect/FlashExplosionBlue", "Missile/LaserMeshMissileBlue", 5f, 1f, 5f, 1f, "");
            config[11000] = new EffectConfig(11000, "AuraSoftPurple", "紫色缓冲特效", "增益", "范围", "Effect/AuraSoftPurple", "", 3f, 1f, 8f, 1f, "");
            config[11010] = new EffectConfig(11010, "BloodExplosion", "鲜血爆裂特效", "伤害", "目标", "Effect/BloodExplosion", "", 3f, 1f, 8f, 1f, "");
            config[11020] = new EffectConfig(11020, "HeartStream", "爱心流光特效", "增益", "目标", "Effect/HeartStream", "", 8f, 1f, 8f, 1f, "");
            config[11030] = new EffectConfig(11030, "LightningExplosionRed", "红色闪电爆裂特效", "伤害", "目标", "Effect/LightningExplosionBlue", "", 3f, 1f, 5f, 1f, "#FF1C00");
            config[11040] = new EffectConfig(11040, "MagicBuffGreen", "绿色增益特效", "增益", "目标", "Effect/MagicBuffGreen", "", 10f, 1f, 8f, 1f, "");
            config[11050] = new EffectConfig(11050, "MagicFieldGreen", "绿色法阵特效", "增益", "范围", "Effect/MagicFieldGreen", "", 8f, 1f, 8f, 1f, "");
            config[11060] = new EffectConfig(11060, "ShieldSoftBlue", "蓝色护盾特效", "增益", "目标", "Effect/ShieldSoftBlue", "", 3f, 1f, 8f, 1f, "");
            config[11061] = new EffectConfig(11061, "ShieldSoftYellow", "黄色护盾特效", "增益", "目标", "Effect/ShieldSoftBlue", "", 3f, 1f, 8f, 1f, "#FFBB09");
            config[11070] = new EffectConfig(11070, "SlowAuraYellow", "黄色迟缓光环特效", "负面", "范围", "Effect/SlowAuraYellow", "", 3f, 2f, 8f, 1f, "");
            config[11080] = new EffectConfig(11080, "SoftFireBigRed", "红色大火特效", "伤害", "目标", "Effect/SoftFireBigRed", "", 10f, 1f, 8f, 1f, "");
            config[11090] = new EffectConfig(11090, "MagicChargeBlue", "蓝色蓄力特效", "增益", "目标", "Effect/MagicChargeBlue", "", 3f, 1f, 8f, 1f, "");
            config[11091] = new EffectConfig(11091, "MagicChargeGreen", "绿色蓄力特效", "增益", "目标", "Effect/MagicChargeBlue", "", 3f, 1f, 8f, 1f, "#00FF00");
            config[11092] = new EffectConfig(11092, "MagicChargePink", "粉色蓄力特效", "增益", "目标", "Effect/MagicChargeBlue", "", 3f, 1f, 8f, 1f, "#FF00EA");
            config[11093] = new EffectConfig(11093, "MagicChargeYellow", "黄色蓄力特效", "增益", "目标", "Effect/MagicChargeBlue", "", 3f, 1f, 8f, 1f, "#FFBB09");
            config[11100] = new EffectConfig(11100, "SparkleAreaWhite", "白色闪耀区域特效", "增益", "范围", "Effect/SparkleAreaWhite", "", 10f, 1f, 8f, 1f, "");
            config[11110] = new EffectConfig(11110, "StunnedCirclingStarsSimple", "眩晕环绕星特效", "负面", "目标", "Effect/StunnedCirclingStarsSimple", "", 3f, 1f, 8f, 1f, "");
            config[11120] = new EffectConfig(11120, "StunnedDamageUp", "眩晕增伤特效", "负面", "目标", "Effect/StunnedDamageUp", "", 3f, 1f, 8f, 1f, "");
            config[11130] = new EffectConfig(11130, "StunnedLock", "定身眩晕特效", "负面", "目标", "Effect/StunnedLock", "", 3f, 1f, 8f, 1f, "");
            config[11140] = new EffectConfig(11140, "SummonStorm", "召唤风暴特效", "伤害", "范围", "Effect/SummonStorm", "", 1.2f, 1f, 8f, 1f, "");
            config[11150] = new EffectConfig(11150, "MagicNovaBlue", "蓝色新星特效", "伤害", "范围", "Effect/MagicNovaBlue", "", 4f, 1f, 8f, 1f, "");
            config[11151] = new EffectConfig(11151, "MagicNovaYellow", "黄色新星特效", "伤害", "范围", "Effect/MagicNovaBlue", "", 4f, 1f, 8f, 1f, "#FFBB00");
            config[11160] = new EffectConfig(11160, "SwordSlashMiniWhite", "白色迷你斩击特效", "伤害", "目标", "Effect/SwordSlashMiniWhite", "", 5f, 1f, 8f, 1f, "");
            config[11170] = new EffectConfig(11170, "SwordWhirlwindWhite", "白色剑旋特效", "伤害", "目标", "Effect/SwordWhirlwindWhite", "", 3f, 1f, 8f, 1f, "");
            config[11180] = new EffectConfig(11180, "SwordHitBlackRedCritical", "黑红暴击剑击特效", "伤害", "目标", "Effect/SwordHitYellowCritical", "", 5f, 1f, 8f, 1f, "#A60B0B");
            config[11181] = new EffectConfig(11181, "SwordHitGreenCritical", "绿色暴击剑击特效", "伤害", "目标", "Effect/SwordHitYellowCritical", "", 5f, 1f, 8f, 1f, "#00FF1B");
            config[11182] = new EffectConfig(11182, "SwordHitRedCritical", "红色暴击剑击特效", "伤害", "目标", "Effect/SwordHitYellowCritical", "", 5f, 1f, 8f, 1f, "#FF0000");
            config[11183] = new EffectConfig(11183, "SwordHitWhiteCritical", "白色暴击剑击特效", "伤害", "目标", "Effect/SwordHitYellowCritical", "", 5f, 1f, 8f, 1f, "#FFFFFF");
            config[11184] = new EffectConfig(11184, "SwordHitYellowCritical", "黄色暴击剑击特效", "伤害", "目标", "Effect/SwordHitYellowCritical", "", 5f, 1f, 8f, 1f, "#FFFF00");
            config[11190] = new EffectConfig(11190, "SwordHitBlue", "蓝色剑击特效", "伤害", "目标", "Effect/SwordHitBlue", "", 10f, 1f, 8f, 1f, "");
            config[11200] = new EffectConfig(11200, "LaserImpactSpark", "激光命中火花特效", "伤害", "目标", "Effect/SwordHitBlue", "", 2f, 1f, 5f, 1f, "#00D5FF");
            config[11210] = new EffectConfig(11210, "MysticExplosionDark", "紫色神秘爆裂特效AOE", "伤害", "范围", "Effect/MysticExplosionDark", "", 3.5f, 1f, 5f, 1f, "");
            config[11211] = new EffectConfig(11211, "MysticExplosionRed", "红色神秘爆裂特效AOE", "伤害", "范围", "Effect/MysticExplosionDark", "", 3.5f, 1f, 5f, 1f, "#FF0000");
            config[11220] = new EffectConfig(11220, "NukeVerticalExplosionFire", "核弹垂直爆裂特效AOE", "伤害", "范围", "Effect/NukeVerticalExplosionFire", "", 2.8f, 1f, 5f, 1f, "");
            config[11230] = new EffectConfig(11230, "StarExplosionBlue", "蓝色星爆裂特效", "增益", "目标", "Effect/StarExplosionBlue", "", 8f, 1f, 5f, 1f, "");
            config[11240] = new EffectConfig(11240, "ScanExplosion", "地震爆裂特效", "伤害", "范围", "Effect/ScanExplosion", "", 3f, 1f, 5f, 1f, "");
            config[11250] = new EffectConfig(11250, "StunExplosion", "击晕爆裂特效", "伤害", "目标", "Effect/StunExplosion", "", 7f, 1f, 5f, 1f, "");
            config[11260] = new EffectConfig(11260, "PoisonExplosionSoft", "中毒爆裂特效", "伤害", "范围", "Effect/PoisonExplosionSoft", "", 7f, 1f, 5f, 1f, "");
            config[11261] = new EffectConfig(11261, "PoisonExplosionSoftYellow", "沙尘爆裂特效", "伤害", "范围", "Effect/PoisonExplosionSoft", "", 7f, 1f, 5f, 1f, "#CC6600");
            config[11262] = new EffectConfig(11262, "PoisonExplosionSoftPurple", "紫色迷雾爆裂特效", "伤害", "范围", "Effect/PoisonExplosionSoft", "", 7f, 1f, 5f, 1f, "#550066");
            config[11270] = new EffectConfig(11270, "MagicAuraBlue", "蓝色魔法区域特效", "增益", "范围", "Effect/MagicAuraBlue", "", 4f, 1f, 5f, 1f, "");
            config[11271] = new EffectConfig(11271, "MagicAuraYellow", "黄色魔法区域特效", "增益", "范围", "Effect/MagicAuraBlue", "", 4f, 1f, 5f, 1f, "#fbff00ff");

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
