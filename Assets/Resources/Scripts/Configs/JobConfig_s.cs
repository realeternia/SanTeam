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
            {"Id", new FieldMetaInfo("序列", "int", 0)},
            {"Name", new FieldMetaInfo("名字", "string", 0)},
            {"NameS", new FieldMetaInfo("名字", "string", 0)},
            {"SkillId", new FieldMetaInfo("技能缩写", "string", 0)},
            {"SourceJob", new FieldMetaInfo("元职业", "int", 0)},
            {"Range", new FieldMetaInfo("射程（近战17 弓50 弩70 炮50 扇/相/棋/鼓/琴/医35）", "int", 0)},
            {"MoveSpeed", new FieldMetaInfo("移动速度（帅/士/盾/锤/枪/戟10 马/车12 弓/炮/扇/相/棋/鼓/琴/医8 弩7）", "int", 0)},
            {"AtkSpeed", new FieldMetaInfo("攻速（30=每秒攻击1次，攻速20=1.5秒/次，15=2秒/次）", "int", 0)},
            {"Armor", new FieldMetaInfo("护甲（职业基准值）", "int", 0)},
            {"MagicRes", new FieldMetaInfo("魔抗（职业基准值）", "int", 0)},
            {"Atk", new FieldMetaInfo("攻击（职业基准模板，参照金铲铲角色定位）", "int", 0)},
            {"Ap", new FieldMetaInfo("法术强度（职业基准模板）", "int", 0)},
            {"Might", new FieldMetaInfo("无双强度（职业基准模板）", "int", 0)},
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
        ///技能缩写（关联SkillConfig.Sname，如 帅/刀/扇）
        /// </summary>
        public string SkillId;
        /// <summary>
        ///元职业
        /// </summary>
        public int SourceJob;
        /// <summary>
        ///射程（近战17 弓50 弩70 炮50 扇/相/棋/鼓/琴/医35）
        /// </summary>
        public int Range;
        /// <summary>
        ///移动速度（帅/士/盾/锤/枪/戟10 马/车12 弓/炮/扇/相/棋/鼓/琴/医8 弩7）
        /// </summary>
        public int MoveSpeed;
        /// <summary>
        ///攻速（20=每秒攻击1次，40=每秒2次）
        /// </summary>
        public int AtkSpeed;
        /// <summary>
        ///护甲（职业基准值）
        /// </summary>
        public int Armor;
        /// <summary>
        ///魔抗（职业基准值）
        /// </summary>
        public int MagicRes;
        /// <summary>
        ///攻击（职业基准模板，参照金铲铲角色定位）
        /// </summary>
        public int Atk;
        /// <summary>
        ///法术强度（职业基准模板）
        /// </summary>
        public int Ap;
        /// <summary>
        ///无双强度（职业基准模板）
        /// </summary>
        public int Might;


        public JobConfig(int Id, string Name, string NameS, string SkillId, int SourceJob, int Range, int MoveSpeed, int AtkSpeed, int Armor, int MagicRes, int Atk, int Ap, int Might)
        {
            this.Id = Id;
            this.Name = Name;
            this.NameS = NameS;
            this.SkillId = SkillId;
            this.SourceJob = SourceJob;
            this.Range = Range;
            this.MoveSpeed = MoveSpeed;
            this.AtkSpeed = AtkSpeed;
            this.Armor = Armor;
            this.MagicRes = MagicRes;
            this.Atk = Atk;
            this.Ap = Ap;
            this.Might = Might;
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
            config[1] = new JobConfig(1, "shuai", "帅", "帅", 0, 17, 10, 20, 40, 35, 75, 75, 70);
            config[101] = new JobConfig(101, "ma", "马", "马", 0, 17, 12, 20, 55, 45, 80, 55, 80);
            config[102] = new JobConfig(102, "mache", "车", "车", 101, 17, 12, 20, 55, 45, 90, 50, 90);
            config[201] = new JobConfig(201, "gong", "弓", "弓", 0, 50, 8, 20, 25, 30, 70, 55, 70);
            config[202] = new JobConfig(202, "gongnu", "弩", "弩", 201, 70, 7, 20, 25, 30, 60, 50, 70);
            config[203] = new JobConfig(203, "gongpao", "炮", "炮", 201, 50, 8, 20, 25, 30, 60, 60, 60);
            config[301] = new JobConfig(301, "shi", "士", "士", 0, 17, 10, 20, 45, 35, 70, 55, 85);
            config[302] = new JobConfig(302, "shidun", "盾", "", 301, 17, 10, 20, 45, 35, 65, 50, 80);
            config[401] = new JobConfig(401, "shan", "扇", "扇", 0, 35, 8, 20, 25, 35, 60, 80, 45);
            config[402] = new JobConfig(402, "shanxiang", "相", "相", 401, 35, 8, 20, 35, 45, 65, 85, 45);
            config[403] = new JobConfig(403, "qi", "棋", "棋", 401, 35, 8, 20, 35, 45, 70, 90, 45);
            config[501] = new JobConfig(501, "gu", "鼓", "鼓", 0, 35, 8, 20, 25, 35, 45, 65, 55);
            config[502] = new JobConfig(502, "qin", "琴", "琴", 501, 35, 8, 20, 15, 25, 45, 85, 40);
            config[503] = new JobConfig(503, "guyi", "医", "医", 501, 35, 8, 20, 25, 35, 55, 85, 45);
            config[601] = new JobConfig(601, "chui", "锤", "锤", 0, 17, 10, 20, 25, 15, 65, 55, 85);
            config[602] = new JobConfig(602, "daoqiang", "枪", "枪", 601, 17, 10, 20, 55, 45, 80, 55, 80);
            config[603] = new JobConfig(603, "daoji", "戟", "戟", 601, 17, 10, 20, 45, 35, 75, 55, 75);

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
