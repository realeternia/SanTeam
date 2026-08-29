using System;
using System.Collections;
using System.Collections.Generic;

namespace CommonConfig
{
    public class SystemAttrConfig
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
            {"name", new FieldMetaInfo("名字", "string", 0, "", true)},
            {"Cname", new FieldMetaInfo("中文名", "string", 0)},
            {"Icon", new FieldMetaInfo("icon", "string", 0)},
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
        public string name;
        /// <summary>
        ///中文名
        /// </summary>
        public string Cname;
        /// <summary>
        ///icon（对应 Resources/Textures/Icons 下的文件）
        /// </summary>
        public string Icon;


        public SystemAttrConfig(int Id, string name, string Cname, string Icon)
        {
            this.Id = Id;
            this.name = name;
            this.Cname = Cname;
            this.Icon = Icon;
        }

        public SystemAttrConfig() { }

        private static Dictionary<int, SystemAttrConfig> config = new Dictionary<int, SystemAttrConfig>();
        public static Dictionary<int, SystemAttrConfig>.ValueCollection ConfigList
        {
            get { return config.Values; }
        }

        public static void Refresh(Dictionary<int, SystemAttrConfig> dict)
        {
            config.Clear();
            config = dict;
            RebuildIndex();
        }

        public static void Load()
        {
            config.Clear();
            config[1] = new SystemAttrConfig(1, "gold", "金币", "othcoin");
            config[2] = new SystemAttrConfig(2, "time", "时间", "othtime");

            RebuildIndex();

        }

        private static void RebuildIndex()
        {
            idxname.Clear();
            foreach (var kv in config)
            {
                if (!string.IsNullOrEmpty(kv.Value.name)) idxname[kv.Value.name] = kv.Key;
            }
        }

        public static SystemAttrConfig GetConfig(int id)
        {
            SystemAttrConfig data;
            if (config.TryGetValue(id, out data))
            {
                return data;
            }
            throw new NullReferenceException(string.Format("配置表SystemAttrConfig不存在id={0}", id));
        }

        private static Dictionary<string, int> idxname = new Dictionary<string, int>();
        public static SystemAttrConfig GetConfigByname(string val)
        {
            return GetConfig(idxname[val]);
        }


        public static bool HasConfig(int id)
        {
            if (config.ContainsKey(id))
            {
                return true;
            }
            return false;
        }

        public static void Assign(int id, SystemAttrConfig configData)
        {
            config[id] = configData; 
        }

        public static void Add(int id, SystemAttrConfig configData)
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
