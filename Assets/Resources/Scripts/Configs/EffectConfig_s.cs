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
            {"EffPath", new FieldMetaInfo("特效资源路径（省略 Prefabs/ 前缀，如 Effect/xxx，对应 Prefabs/Effect 下资源，无则留空）", "string", 210)},
            {"MissilePath", new FieldMetaInfo("导弹资源路径（省略 Prefabs/ 前缀，如 Missile/xxx，对应 Prefabs/Missile 下资源，无则留空）", "string", 175)},
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


        public EffectConfig(int Id, string Name, string Cname, string EffPath, string MissilePath, float Scale, float OffsetY, float MissileScale, float MissileOffsetY, string TintColor)
        {
            this.Id = Id;
            this.Name = Name;
            this.Cname = Cname;
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
            config[1] = new EffectConfig(1, "AuraSoftPurple", "紫色缓冲特效", "Effect/AuraSoftPurple", "", 5f, 1f, 5f, 1f, "");
            config[2] = new EffectConfig(2, "BloodExplosion", "鲜血爆裂特效", "Effect/BloodExplosion", "", 5f, 1f, 5f, 1f, "");
            config[3] = new EffectConfig(3, "BulletExplosionBlue", "蓝色子弹爆裂特效", "Effect/BulletExplosionBlue", "Missile/BulletExplosionBlue", 5f, 1f, 10f, 1f, "");
            config[4] = new EffectConfig(4, "BulletExplosionFire", "火焰子弹爆裂特效", "Effect/BulletExplosionBlue", "Missile/BulletExplosionFire", 5f, 1f, 10f, 1f, "#FF7A00");
            config[5] = new EffectConfig(5, "BulletExplosionGreen", "绿色子弹爆裂特效", "Effect/BulletExplosionBlue", "Missile/BulletExplosionGreen", 5f, 1f, 10f, 1f, "#A7FF00");
            config[6] = new EffectConfig(6, "ExplosionFireballFire", "火球爆炸特效", "Effect/ExplosionFireballFire", "Missile/ExplosionFireballFire", 5f, 1f, 5f, 1f, "");
            config[7] = new EffectConfig(7, "FanExplosion", "扇子爆裂特效", "Effect/FanExplosion", "Missile/FanExplosion", 5f, 1f, 5f, 1f, "");
            config[8] = new EffectConfig(8, "FrostExplosionBlue", "蓝色冰冻爆裂特效", "Effect/FrostExplosionBlue", "Missile/FrostExplosionBlue", 5f, 1f, 5f, 1f, "");
            config[9] = new EffectConfig(9, "GasExplosionFire", "火焰毒气爆裂特效", "Effect/GasExplosionFire", "Missile/GasExplosionFire", 5f, 1f, 5f, 1f, "");
            config[10] = new EffectConfig(10, "HeartStream", "爱心流光特效", "Effect/HeartStream", "", 5f, 1f, 5f, 1f, "");
            config[11] = new EffectConfig(11, "LightningExplosionBlue", "蓝色闪电爆裂特效", "Effect/LightningExplosionBlue", "Missile/LightningExplosionBlue", 3f, 1f, 5f, 1f, "");
            config[12] = new EffectConfig(12, "LightningExplosionRed", "红色闪电爆裂特效", "Effect/LightningExplosionBlue", "", 3f, 1f, 5f, 1f, "#FF1C00");
            config[13] = new EffectConfig(13, "LightningExplosionYellow", "黄色闪电爆裂特效", "Effect/LightningExplosionBlue", "Missile/LightningExplosionYellow", 3f, 1f, 5f, 1f, "#FFD800");
            config[14] = new EffectConfig(14, "MagicBuffGreen", "绿色增益特效", "Effect/MagicBuffGreen", "", 10f, 1f, 5f, 1f, "");
            config[15] = new EffectConfig(15, "MagicChargeBlue", "蓝色蓄力特效", "Effect/MagicChargeBlue", "", 5f, 1f, 5f, 1f, "");
            config[16] = new EffectConfig(16, "MagicChargeGreen", "绿色蓄力特效", "Effect/MagicChargeBlue", "", 5f, 1f, 5f, 1f, "#00FF00");
            config[17] = new EffectConfig(17, "MagicChargePink", "粉色蓄力特效", "Effect/MagicChargeBlue", "", 5f, 1f, 5f, 1f, "#FF00EA");
            config[18] = new EffectConfig(18, "MagicChargeYellow", "黄色蓄力特效", "Effect/MagicChargeBlue", "", 5f, 1f, 5f, 1f, "#FFBB09");
            config[19] = new EffectConfig(19, "MagicFieldGreen", "绿色法阵特效", "Effect/MagicFieldGreen", "", 5f, 1f, 5f, 1f, "");
            config[20] = new EffectConfig(20, "MagicNovaBlue", "蓝色新星特效", "Effect/MagicNovaBlue", "", 5f, 1f, 5f, 1f, "");
            config[21] = new EffectConfig(21, "MagicNovaYellow", "黄色新星特效", "Effect/MagicNovaBlue", "", 5f, 1f, 5f, 1f, "#FFBB00");
            config[22] = new EffectConfig(22, "ShadowExplosion", "暗影爆裂特效", "Effect/ShadowExplosion", "Missile/ShadowExplosion", 5f, 1f, 5f, 1f, "");
            config[23] = new EffectConfig(23, "ShadowExplosionGreen", "绿色暗影爆裂特效", "Effect/ShadowExplosion", "Missile/ShadowExplosionGreen", 5f, 1f, 5f, 1f, "#105E11");
            config[24] = new EffectConfig(24, "SharpExplosionGreen", "绿色利刃爆裂特效", "Effect/SharpExplosionGreen", "Missile/SharpExplosionGreen", 5f, 1f, 5f, 1f, "");
            config[25] = new EffectConfig(25, "ShieldSoftBlue", "蓝色护盾特效", "Effect/ShieldSoftBlue", "", 3f, 1f, 5f, 1f, "");
            config[26] = new EffectConfig(26, "SlowAuraYellow", "黄色迟缓光环特效", "Effect/SlowAuraYellow", "", 5f, 1f, 5f, 1f, "");
            config[27] = new EffectConfig(27, "SoftFireBigRed", "红色大火特效", "Effect/SoftFireBigRed", "", 8f, 1f, 5f, 1f, "");
            config[28] = new EffectConfig(28, "SoulExplosionOrange", "橙色灵魂爆裂特效", "Effect/SoulExplosionOrange", "Missile/SoulExplosionOrange", 5f, 1f, 5f, 1f, "");
            config[29] = new EffectConfig(29, "SparkleAreaWhite", "白色闪耀区域特效", "Effect/SparkleAreaWhite", "", 7f, 1f, 5f, 1f, "");
            config[30] = new EffectConfig(30, "StormExplosion", "风暴爆裂特效", "Effect/StormExplosion", "Missile/StormExplosion", 7f, 1f, 5f, 1f, "");
            config[31] = new EffectConfig(31, "StunnedCirclingStarsSimple", "眩晕环绕星特效", "Effect/StunnedCirclingStarsSimple", "", 3f, 1f, 5f, 1f, "");
            config[32] = new EffectConfig(32, "StunnedDamageUp", "眩晕增伤特效", "Effect/StunnedDamageUp", "", 3f, 1f, 5f, 1f, "");
            config[33] = new EffectConfig(33, "StunnedLock", "定身眩晕特效", "Effect/StunnedLock", "", 3f, 1f, 5f, 1f, "");
            config[34] = new EffectConfig(34, "SummonStorm", "召唤风暴特效", "Effect/SummonStorm", "", 2f, 1f, 5f, 1f, "");
            config[35] = new EffectConfig(35, "SwordHitBlackRedCritical", "黑红暴击剑击特效", "Effect/SwordHitYellowCritical", "", 5f, 1f, 5f, 1f, "#A60B0B");
            config[36] = new EffectConfig(36, "SwordHitBlue", "蓝色剑击特效", "Effect/SwordHitBlue", "", 5f, 1f, 5f, 1f, "");
            config[37] = new EffectConfig(37, "SwordHitGreenCritical", "绿色暴击剑击特效", "Effect/SwordHitYellowCritical", "", 5f, 1f, 5f, 1f, "#00FF1B");
            config[38] = new EffectConfig(38, "SwordHitRedCritical", "红色暴击剑击特效", "Effect/SwordHitYellowCritical", "", 5f, 1f, 5f, 1f, "#FF0000");
            config[39] = new EffectConfig(39, "SwordHitWhiteCritical", "白色暴击剑击特效", "Effect/SwordHitYellowCritical", "", 5f, 1f, 5f, 1f, "#FFFFFF");
            config[40] = new EffectConfig(40, "SwordHitYellowCritical", "黄色暴击剑击特效", "Effect/SwordHitYellowCritical", "", 5f, 1f, 5f, 1f, "#FFFF00");
            config[41] = new EffectConfig(41, "SwordSlashMiniWhite", "白色迷你斩击特效", "Effect/SwordSlashMiniWhite", "", 10f, 1f, 5f, 1f, "");
            config[42] = new EffectConfig(42, "SwordWhirlwindWhite", "白色剑旋特效", "Effect/SwordWhirlwindWhite", "", 7f, 1f, 5f, 1f, "");
            config[43] = new EffectConfig(43, "ToolExplosion", "锤类爆裂特效", "Effect/ToolExplosion", "Missile/ToolExplosion", 5f, 1f, 5f, 1f, "");
            config[44] = new EffectConfig(44, "AxeExplosion", "斧类爆裂特效（弹道）", "", "Missile/AxeExplosion", 5f, 1f, 5f, 1f, "");
            config[45] = new EffectConfig(45, "GasShootFire", "火焰喷射弹道特效", "", "Missile/GasShootFire", 5f, 1f, 5f, 1f, "");
            config[46] = new EffectConfig(46, "NukeMissileFires", "核弹导弹弹道特效", "", "Missile/NukeMissileFires", 5f, 1f, 5f, 1f, "");

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
