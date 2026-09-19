using System;
using System.Collections.Generic;

namespace CommonConfig
{
    public class ItemCombineConfig
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
            {"Id", new FieldMetaInfo("序列", "int", 0, "", true)},
            {"ItemA", new FieldMetaInfo("材料AID", "int", 0)},
            {"ItemAcount", new FieldMetaInfo("材料A数量", "int", 0)},
            {"ItemB", new FieldMetaInfo("材料BID", "int", 0)},
            {"ItemBcount", new FieldMetaInfo("材料B数量", "int", 0)},
            {"ResultId", new FieldMetaInfo("合成结果ID", "int", 0)},
            {"ResultCount", new FieldMetaInfo("合成结果数量", "int", 0)},
        };

        public static Dictionary<string, FieldMetaInfo> FieldMeta { get { return fieldMeta; } }

        private static List<CellMeta> cellMeta = new List<CellMeta>();
        public static List<CellMeta> CellMetas { get { return cellMeta; } }

        /// <summary>
        ///序列
        /// </summary>
        public int Id;
        /// <summary>
        ///材料AID
        /// </summary>
        public int ItemA;
        /// <summary>
        ///材料A数量
        /// </summary>
        public int ItemAcount;
        /// <summary>
        ///材料BID
        /// </summary>
        public int ItemB;
        /// <summary>
        ///材料B数量
        /// </summary>
        public int ItemBcount;
        /// <summary>
        ///合成结果ID
        /// </summary>
        public int ResultId;
        /// <summary>
        ///合成结果数量
        /// </summary>
        public int ResultCount;

        public ItemCombineConfig(int Id, int ItemA, int ItemAcount, int ItemB, int ItemBcount, int ResultId, int ResultCount)
        {
            this.Id = Id;
            this.ItemA = ItemA;
            this.ItemAcount = ItemAcount;
            this.ItemB = ItemB;
            this.ItemBcount = ItemBcount;
            this.ResultId = ResultId;
            this.ResultCount = ResultCount;
        }

        public ItemCombineConfig() { }

        private static Dictionary<int, ItemCombineConfig> config = new Dictionary<int, ItemCombineConfig>();
        public static Dictionary<int, ItemCombineConfig>.ValueCollection ConfigList
        {
            get { return config.Values; }
        }

        public static void Refresh(Dictionary<int, ItemCombineConfig> dict)
        {
            config.Clear();
            config = dict;
        }

        public static void Load()
        {
            config.Clear();
            RebuildIndex();
        }

        private static void RebuildIndex()
        {
            // 空实现：以 Id 为主键，无需额外索引
        }

        public static ItemCombineConfig GetConfig(int id)
        {
            ItemCombineConfig data;
            if (config.TryGetValue(id, out data))
            {
                return data;
            }
            throw new NullReferenceException(string.Format("配置表ItemCombineConfig不存在id={0}", id));
        }

        public static bool HasConfig(int id)
        {
            return config.ContainsKey(id);
        }

        public static void Assign(int id, ItemCombineConfig configData)
        {
            config[id] = configData;
        }

        public static void Add(int id, ItemCombineConfig configData)
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