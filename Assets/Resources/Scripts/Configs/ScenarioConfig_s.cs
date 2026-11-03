using System;
using System.Collections;
using System.Collections.Generic;

namespace CommonConfig
{
    public class ScenarioConfig
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
            {"Id", new FieldMetaInfo("剧本Id（对应 HeroConfig.Scenario1/2/3）", "int", 60)},
            {"Name", new FieldMetaInfo("剧本名", "string", 0)},
        };

        public static Dictionary<string, FieldMetaInfo> FieldMeta { get { return fieldMeta; } }

        private static List<CellMeta> cellMeta = new List<CellMeta>();
        public static List<CellMeta> CellMetas { get { return cellMeta; } }

        /// <summary>
        ///剧本Id（对应 HeroConfig.Scenario1/2/3）
        /// </summary>
        public int Id;
        /// <summary>
        ///剧本名
        /// </summary>
        public string Name;


        public ScenarioConfig(int Id, string Name)
        {
            this.Id = Id;
            this.Name = Name;
        }

        public ScenarioConfig() { }

        private static Dictionary<int, ScenarioConfig> config = new Dictionary<int, ScenarioConfig>();
        public static Dictionary<int, ScenarioConfig>.ValueCollection ConfigList
        {
            get { return config.Values; }
        }

        public static void Refresh(Dictionary<int, ScenarioConfig> dict)
        {
            config.Clear();
            config = dict;
            RebuildIndex();
        }

        public static void Load()
        {
            config.Clear();
            config[1] = new ScenarioConfig(1, "群雄割据");
            config[2] = new ScenarioConfig(2, "三国鼎立");
            config[3] = new ScenarioConfig(3, "三家归晋");

            RebuildIndex();

        }

        private static void RebuildIndex()
        {
            foreach (var kv in config)
            {
            }
        }

        public static ScenarioConfig GetConfig(int id)
        {
            ScenarioConfig data;
            if (config.TryGetValue(id, out data))
            {
                return data;
            }
            throw new NullReferenceException(string.Format("配置表ScenarioConfig不存在id={0}", id));
        }


        public static bool HasConfig(int id)
        {
            if (config.ContainsKey(id))
            {
                return true;
            }
            return false;
        }

        public static void Assign(int id, ScenarioConfig configData)
        {
            config[id] = configData; 
        }

        public static void Add(int id, ScenarioConfig configData)
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
