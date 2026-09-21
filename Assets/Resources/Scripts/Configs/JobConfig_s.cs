using System;
using System.Collections;
using System.Collections.Generic;

namespace CommonConfig
{
    public class JobConfig
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
            {"NameS", new FieldMetaInfo("名字", "string", 0)},
            {"SkillId", new FieldMetaInfo("技能缩写", "string", 0)},
            {"EquipAttr", new FieldMetaInfo("喜欢装备的属性（最多3个，取值见 HeroAttrConfig.name，如 atk/ap/hp/armor/magicres/atkspeed/crit/mpRegen；装备匹配时按顺序优先）", "string[]", 0)},
            {"Atk", new FieldMetaInfo("攻击（职业基准模板，参照金铲铲角色定位）", "int", 60)},
            {"Ap", new FieldMetaInfo("法术强度（职业基准模板）", "int", 60)},
            {"Hp", new FieldMetaInfo("生命（职业基准值）", "int", 60)},
            {"Range", new FieldMetaInfo("射程（近战17 弓50 弩70 炮50 扇/相/棋/鼓/琴/医/工35）", "int", 60)},
            {"AtkSpeed", new FieldMetaInfo("攻速（30=每秒攻击1次，攻速20=1.5秒/次，15=2秒/次）", "int", 60)},
            {"HpRegen", new FieldMetaInfo("生命回复/秒（职业基准值）", "int", 60)},
            {"MpRegen", new FieldMetaInfo("魔法回复/秒（职业基准值）", "int", 60)},
            {"Armor", new FieldMetaInfo("护甲（职业基准值）", "int", 59)},
            {"MagicRes", new FieldMetaInfo("魔抗（职业基准值）", "int", 60)},
            {"MoveSpeed", new FieldMetaInfo("移动速度（王/士/盾/锤/枪/戟10 马/车12 弓/炮/扇/相/棋/鼓/琴/医/工8 弩7）", "int", 60)},
            {"MissileSpeed", new FieldMetaInfo("导弹速度", "int", 60)},
            {"MissileHight", new FieldMetaInfo("导弹高度", "float", 60)},
            {"HitEffect", new FieldMetaInfo("hit", "string", 364)},
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
        ///名字
        /// </summary>
        public string NameS;
        /// <summary>
        ///技能缩写（关联SkillConfig.Sname，如 王/刀/扇）
        /// </summary>
        public string SkillId;
        /// <summary>
        ///喜欢装备的属性（最多3个，取值见 HeroAttrConfig.name，如 atk/ap/hp/armor/magicres/atkspeed/crit/mpRegen；装备匹配时按顺序优先）
        /// </summary>
        public string[] EquipAttr;
        /// <summary>
        ///攻击（职业基准模板，参照金铲铲角色定位）
        /// </summary>
        public int Atk;
        /// <summary>
        ///法术强度（职业基准模板）
        /// </summary>
        public int Ap;
        /// <summary>
        ///生命（职业基准值，英雄生命 = 该基准 × (1 + HeroConfig.Hp修正%/100)）
        /// </summary>
        public int Hp;
        /// <summary>
        ///射程（近战17 弓50 弩70 炮50 扇/相/棋/鼓/琴/医/工35）
        /// </summary>
        public int Range;
        /// <summary>
        ///攻速（20=每秒攻击1次，40=每秒2次）
        /// </summary>
        public int AtkSpeed;
        /// <summary>
        ///生命回复/秒（职业基准值）
        /// </summary>
        public int HpRegen;
        /// <summary>
        ///魔法回复/秒（职业基准值）
        /// </summary>
        public int MpRegen;
        /// <summary>
        ///护甲（职业基准值）
        /// </summary>
        public int Armor;
        /// <summary>
        ///魔抗（职业基准值）
        /// </summary>
        public int MagicRes;
        /// <summary>
        ///移动速度（王/士/盾/锤/枪/戟10 马/车12 弓/炮/扇/相/棋/鼓/琴/医/工8 弩7）
        /// </summary>
        public int MoveSpeed;
        /// <summary>
        ///导弹速度（0=无导弹，近战职业默认0）
        /// </summary>
        public int MissileSpeed;
        /// <summary>
        ///导弹高度
        /// </summary>
        public float MissileHight;
        /// <summary>
        ///hit
        /// </summary>
        public string HitEffect;


        public JobConfig(int Id, string Name, string NameS, string SkillId, string[] EquipAttr, int Atk, int Ap, int Hp, int Range, int AtkSpeed, int HpRegen, int MpRegen, int Armor, int MagicRes, int MoveSpeed, int MissileSpeed, float MissileHight, string HitEffect)
        {
            this.Id = Id;
            this.Name = Name;
            this.NameS = NameS;
            this.SkillId = SkillId;
            this.EquipAttr = EquipAttr;
            this.Atk = Atk;
            this.Ap = Ap;
            this.Hp = Hp;
            this.Range = Range;
            this.AtkSpeed = AtkSpeed;
            this.HpRegen = HpRegen;
            this.MpRegen = MpRegen;
            this.Armor = Armor;
            this.MagicRes = MagicRes;
            this.MoveSpeed = MoveSpeed;
            this.MissileSpeed = MissileSpeed;
            this.MissileHight = MissileHight;
            this.HitEffect = HitEffect;
        }

        public JobConfig() { }

        private static Dictionary<int, JobConfig> config = new Dictionary<int, JobConfig>();
        public static Dictionary<int, JobConfig>.ValueCollection ConfigList
        {
            get { return config.Values; }
        }

        public static void Refresh(Dictionary<int, JobConfig> dict)
        {
            config.Clear();
            config = dict;
            RebuildIndex();
        }

        public static void Load()
        {
            config.Clear();
            config[1] = new JobConfig(1, "诸侯", "王", "王", new string[]{"atk","ap","hp"}, 50, 0, 550, 17, 20, 2, 1, 30, 30, 10, 0, 0f, "SwordHitYellowCritical");
            config[101] = new JobConfig(101, "骑士", "马", "马", new string[]{"atk","hp","atkspeed"}, 50, 0, 620, 17, 20, 2, 1, 35, 35, 12, 0, 0f, "SwordHitYellowCritical");
            config[102] = new JobConfig(102, "战车", "车", "车", new string[]{"atk","armor","hp"}, 55, 0, 580, 17, 20, 2, 1, 35, 35, 12, 0, 0f, "SwordHitYellowCritical");
            config[201] = new JobConfig(201, "弓手", "弓", "弓", new string[]{"atk","atkspeed","crit"}, 40, 0, 450, 45, 20, 2, 1, 15, 30, 8, 20, 1.5f, "BulletExplosionBlue");
            config[202] = new JobConfig(202, "弩手", "弩", "弩", new string[]{"atk","crit","atkspeed"}, 45, 0, 400, 60, 20, 2, 1, 15, 30, 8, 25, 0f, "BulletExplosionBlue");
            config[203] = new JobConfig(203, "炮手", "炮", "炮", new string[]{"ap","atk","atkspeed"}, 40, 0, 450, 45, 20, 2, 1, 35, 20, 8, 13, 2.5f, "GasShootFire");
            config[301] = new JobConfig(301, "卫士", "士", "士", new string[]{"armor","hp","magicres"}, 60, 0, 660, 17, 20, 2, 1, 40, 20, 10, 0, 0f, "SwordHitYellowCritical");
            config[302] = new JobConfig(302, "盾兵", "盾", "盾", new string[]{"hp","armor","magicres"}, 50, 0, 760, 17, 20, 2, 1, 45, 15, 10, 0, 0f, "SwordHitYellowCritical");
            config[401] = new JobConfig(401, "智士", "扇", "扇", new string[]{"ap","mpRegen","hp"}, 40, 0, 450, 35, 20, 2, 1, 15, 35, 8, 15, 0f, "StormExplosion");
            config[402] = new JobConfig(402, "丞相", "相", "相", new string[]{"ap","magicres","mpRegen"}, 35, 0, 500, 35, 20, 2, 1, 15, 35, 8, 18, 0f, "SharpExplosionGreen");
            config[403] = new JobConfig(403, "棋手", "棋", "棋", new string[]{"ap","crit","mpRegen"}, 40, 0, 450, 35, 20, 2, 1, 15, 35, 8, 15, 0f, "LightningExplosionBlue");
            config[501] = new JobConfig(501, "鼓手", "鼓", "鼓", new string[]{"ap","hp","mpRegen"}, 45, 0, 470, 35, 20, 2, 1, 15, 35, 8, 15, 0f, "SharpExplosionGreen");
            config[502] = new JobConfig(502, "琴手", "琴", "琴", new string[]{"ap","mpRegen","atkspeed"}, 45, 0, 430, 35, 20, 2, 1, 20, 20, 8, 15, 0f, "StormExplosion");
            config[503] = new JobConfig(503, "医士", "医", "医", new string[]{"ap","hpRegen","mpRegen"}, 45, 0, 430, 35, 20, 2, 1, 20, 20, 8, 14, 0f, "ShadowExplosionGreen");
            config[601] = new JobConfig(601, "锤兵", "锤", "锤", new string[]{"atk","hp","armor"}, 55, 0, 550, 17, 20, 2, 1, 40, 20, 10, 0, 0f, "SwordHitYellowCritical");
            config[602] = new JobConfig(602, "枪兵", "枪", "枪", new string[]{"atk","atkspeed","hp"}, 55, 0, 550, 17, 20, 2, 1, 25, 25, 10, 0, 0f, "SwordHitYellowCritical");
            config[603] = new JobConfig(603, "戟兵", "戟", "戟", new string[]{"atk","armor","hp"}, 50, 0, 600, 17, 20, 2, 1, 25, 25, 10, 0, 0f, "SwordHitYellowCritical");
            config[701] = new JobConfig(701, "工匠", "工", "工", new string[]{"ap","atkspeed","hp"}, 40, 0, 400, 35, 20, 2, 1, 15, 35, 8, 18, 1f, "ToolExplosion");

            RebuildIndex();

        }

        private static void RebuildIndex()
        {
            foreach (var kv in config)
            {
            }
        }

        public static JobConfig GetConfig(int id)
        {
            JobConfig data;
            if (config.TryGetValue(id, out data))
            {
                return data;
            }
            throw new NullReferenceException(string.Format("配置表JobConfig不存在id={0}", id));
        }


        public static bool HasConfig(int id)
        {
            if (config.ContainsKey(id))
            {
                return true;
            }
            return false;
        }

        public static void Assign(int id, JobConfig configData)
        {
            config[id] = configData; 
        }

        public static void Add(int id, JobConfig configData)
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
