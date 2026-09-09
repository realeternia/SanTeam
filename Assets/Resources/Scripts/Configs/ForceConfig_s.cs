using System;
using System.Collections;
using System.Collections.Generic;

namespace CommonConfig
{
    public class ForceConfig
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
            {"Id", new FieldMetaInfo("势力Id（对应 HeroConfig.Side：1魏 2蜀 3吴 4晋 5群 6神 10野）", "int", 60)},
            {"Name", new FieldMetaInfo("势力名", "string", 0)},
            {"Colorstr", new FieldMetaInfo("势力主色（十六进制，如#284600，用于阵营背景/图标）", "string", 0)},
            {"KingId", new FieldMetaInfo("主公英雄Id（HeroConfig.Id，无主公为0）", "int", 60)},
            {"Icon", new FieldMetaInfo("图标", "string", 0)},
            {"JoinFactionShield", new FieldMetaInfo("是否参与同阵营护盾机制（野=10不参与）", "bool", 0)},
        };

        public static Dictionary<string, FieldMetaInfo> FieldMeta { get { return fieldMeta; } }

        private static List<CellMeta> cellMeta = new List<CellMeta>();
        public static List<CellMeta> CellMetas { get { return cellMeta; } }

        /// <summary>
        ///势力Id（对应 HeroConfig.Side：1魏 2蜀 3吴 4晋 5群 6神 10野）
        /// </summary>
        public int Id;
        /// <summary>
        ///势力名
        /// </summary>
        public string Name;
        /// <summary>
        ///势力主色（十六进制，如#284600，用于阵营背景/图标）
        /// </summary>
        public string Colorstr;
        /// <summary>
        ///主公英雄Id（HeroConfig.Id，无主公为0）
        /// </summary>
        public int KingId;
        /// <summary>
        ///图标
        /// </summary>
        public string Icon;
        /// <summary>
        ///是否参与同阵营护盾机制（野=10不参与）
        /// </summary>
        public bool JoinFactionShield;


        public ForceConfig(int Id, string Name, string Colorstr, int KingId, string Icon, bool JoinFactionShield)
        {
            this.Id = Id;
            this.Name = Name;
            this.Colorstr = Colorstr;
            this.KingId = KingId;
            this.Icon = Icon;
            this.JoinFactionShield = JoinFactionShield;
        }

        public ForceConfig() { }

        private static Dictionary<int, ForceConfig> config = new Dictionary<int, ForceConfig>();
        public static Dictionary<int, ForceConfig>.ValueCollection ConfigList
        {
            get { return config.Values; }
        }

        public static void Refresh(Dictionary<int, ForceConfig> dict)
        {
            config.Clear();
            config = dict;
            RebuildIndex();
        }

        public static void Load()
        {
            config.Clear();
            config[1] = new ForceConfig(1, "魏国", "#284600", 100001, "side1", true);
            config[2] = new ForceConfig(2, "蜀国", "#002364", 100002, "side2", true);
            config[3] = new ForceConfig(3, "吴国", "#640000", 100003, "side3", true);
            config[4] = new ForceConfig(4, "董侯", "#1E646E", 100004, "side4", true);
            config[5] = new ForceConfig(5, "晋国", "#5A326E", 100005, "side5", true);
            config[6] = new ForceConfig(6, "袁氏", "#785A1E", 100006, "side6", true);
            config[10] = new ForceConfig(10, "野", "#323232", 0, "", false);

            RebuildIndex();

        }

        private static void RebuildIndex()
        {
            foreach (var kv in config)
            {
            }
        }

        public static ForceConfig GetConfig(int id)
        {
            ForceConfig data;
            if (config.TryGetValue(id, out data))
            {
                return data;
            }
            throw new NullReferenceException(string.Format("配置表ForceConfig不存在id={0}", id));
        }


        public static bool HasConfig(int id)
        {
            if (config.ContainsKey(id))
            {
                return true;
            }
            return false;
        }

        public static void Assign(int id, ForceConfig configData)
        {
            config[id] = configData; 
        }

        public static void Add(int id, ForceConfig configData)
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
