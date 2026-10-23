﻿using System;
using System.Collections;
using System.Collections.Generic;

namespace CommonConfig
{
    public class MapConfig
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
            {"Id", new FieldMetaInfo("地图Id（与 MapInfo.Mapid 一致）", "int", 60)},
            {"NameE", new FieldMetaInfo("地图英文名（GameRoundConfig.MapWeights 的权重键，如 forest1v1:30）", "string", 0, "", true)},
            {"Des", new FieldMetaInfo("描述", "string", 377)},
            {"Rule", new FieldMetaInfo("地图规则（0=独立配对，1=两队混战，2=全场混战）", "int", 60)},
            {"Prefab", new FieldMetaInfo("地图prefab名（Resources/Maps 下的预制体名，如 Map1）", "string", 0)},
            {"IsTestDefault", new FieldMetaInfo("测试默认地图（调试阵容/测试战斗固定使用该地图；多个 true 时取第一个）", "bool", 0)},
        };

        public static Dictionary<string, FieldMetaInfo> FieldMeta { get { return fieldMeta; } }

        private static List<CellMeta> cellMeta = new List<CellMeta>();
        public static List<CellMeta> CellMetas { get { return cellMeta; } }

        /// <summary>
        ///地图Id（与 MapInfo.Mapid 一致）
        /// </summary>
        public int Id;
        /// <summary>
        ///地图英文名（GameRoundConfig.MapWeights 的权重键，如 forest1v1:30）
        /// </summary>
        public string NameE;
        /// <summary>
        ///描述
        /// </summary>
        public string Des;
        /// <summary>
        ///地图规则（0=独立配对，1=两队混战，2=全场混战）
        /// </summary>
        public int Rule;
        /// <summary>
        ///地图prefab名（Resources/Maps 下的预制体名，如 Map1）
        /// </summary>
        public string Prefab;
        /// <summary>
        ///测试默认地图（调试阵容/测试战斗固定使用该地图；多个 true 时取第一个）
        /// </summary>
        public bool IsTestDefault;


        public MapConfig(int Id, string NameE, string Des, int Rule, string Prefab, bool IsTestDefault)
        {
            this.Id = Id;
            this.NameE = NameE;
            this.Des = Des;
            this.Rule = Rule;
            this.Prefab = Prefab;
            this.IsTestDefault = IsTestDefault;
        }

        public MapConfig() { }

        private static Dictionary<int, MapConfig> config = new Dictionary<int, MapConfig>();
        public static Dictionary<int, MapConfig>.ValueCollection ConfigList
        {
            get { return config.Values; }
        }

        public static void Refresh(Dictionary<int, MapConfig> dict)
        {
            config.Clear();
            config = dict;
            RebuildIndex();
        }

        public static void Load()
        {
            config.Clear();
            config[1] = new MapConfig(1, "forest1v1", "独立配对：1-2/3-4/5-6/7-8 各自1v1，分出胜负即结束", 0, "Map1", false);
            config[2] = new MapConfig(2, "forest4v4", "两队混战：奇数位 vs 偶数位，一队全灭即结束", 1, "Map2", false);
            config[3] = new MapConfig(3, "islandteam", "两队混战：奇数位 vs 偶数位，一队全灭即结束", 1, "Map3", false);
            config[4] = new MapConfig(4, "map4", "全场混战：最后存活阵营获胜，按死亡顺序计分", 2, "Map4", false);
            config[5] = new MapConfig(5, "river", "独立配对：1-2/3-4/5-6/7-8 各自1v1，分出胜负即结束", 0, "Map11River", true);

            RebuildIndex();

        }

        private static void RebuildIndex()
        {
            idxNameE.Clear();
            foreach (var kv in config)
            {
                if (!string.IsNullOrEmpty(kv.Value.NameE)) idxNameE[kv.Value.NameE] = kv.Key;
            }
        }

        public static MapConfig GetConfig(int id)
        {
            MapConfig data;
            if (config.TryGetValue(id, out data))
            {
                return data;
            }
            throw new NullReferenceException(string.Format("配置表MapConfig不存在id={0}", id));
        }

        private static Dictionary<string, int> idxNameE = new Dictionary<string, int>();
        public static MapConfig GetConfigByNameE(string val)
        {
            return GetConfig(idxNameE[val]);
        }

        public static bool HasConfig(int id)
        {
            if (config.ContainsKey(id))
            {
                return true;
            }
            return false;
        }

        public static void Assign(int id, MapConfig configData)
        {
            config[id] = configData; 
        }

        public static void Add(int id, MapConfig configData)
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
