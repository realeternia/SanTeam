﻿﻿﻿using System;
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
            {"Id", new FieldMetaInfo("势力Id（对应 HeroConfig.Side：1蜀 2魏 3吴 4董 5晋 6袁 7刘璋 8刘表 9马腾 10野）", "int", 60)},
            {"Name", new FieldMetaInfo("势力名", "string", 0)},
            {"Names", new FieldMetaInfo("势力短名（剧本字段 品质:短名 使用，如 蜀/魏/吴/董/晋/袁/璋/表/马/野）", "string", 0)},
            {"Colorstr", new FieldMetaInfo("势力主色（十六进制，如#284600，用于阵营背景/图标）", "string", 0)},
            {"Icon", new FieldMetaInfo("图标", "string", 0)},
            {"JoinFactionShield", new FieldMetaInfo("是否参与同阵营护盾机制（野=10不参与）", "bool", 0)},
        };

        public static Dictionary<string, FieldMetaInfo> FieldMeta { get { return fieldMeta; } }

        private static List<CellMeta> cellMeta = new List<CellMeta>();
        public static List<CellMeta> CellMetas { get { return cellMeta; } }

        /// <summary>
        ///势力Id（对应 HeroConfig.Side：1蜀 2魏 3吴 4董 5晋 6袁 7刘璋 8刘表 10野）
        /// </summary>
        public int Id;
        /// <summary>
        ///势力名
        /// </summary>
        public string Name;
        /// <summary>
        ///势力短名（剧本字段 品质:短名 使用，如 蜀/魏/吴/董/晋/袁/璋/表/野）
        /// </summary>
        public string Names;
        /// <summary>
        ///势力主色（十六进制，如#284600，用于阵营背景/图标）
        /// </summary>
        public string Colorstr;
        /// <summary>
        ///图标
        /// </summary>
        public string Icon;
        /// <summary>
        ///是否参与同阵营护盾机制（野=10不参与）
        /// </summary>
        public bool JoinFactionShield;


        public ForceConfig(int Id, string Name, string Names, string Colorstr, string Icon, bool JoinFactionShield)
        {
            this.Id = Id;
            this.Name = Name;
            this.Names = Names;
            this.Colorstr = Colorstr;
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
            config[1] = new ForceConfig(1, "蜀国", "蜀", "#284600", "side1", true);
            config[2] = new ForceConfig(2, "魏国", "魏", "#002364", "side2", true);
            config[3] = new ForceConfig(3, "吴国", "吴", "#640000", "side3", true);
            config[4] = new ForceConfig(4, "董侯", "董", "#1E646E", "side4", true);
            config[5] = new ForceConfig(5, "晋国", "晋", "#5A326E", "side5", true);
            config[6] = new ForceConfig(6, "袁氏", "袁", "#785A1E", "side6", true);
            config[7] = new ForceConfig(7, "刘璋", "璋", "#4C6A2E", "", true);
            config[8] = new ForceConfig(8, "刘表", "表", "#2E5A7A", "", true);
            config[9] = new ForceConfig(9, "马腾", "马", "#8B5A2B", "", true);
            config[10] = new ForceConfig(10, "野", "野", "#323232", "", false);

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
