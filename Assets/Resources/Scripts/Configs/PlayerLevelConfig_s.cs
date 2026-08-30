using System;
using System.Collections;
using System.Collections.Generic;

namespace CommonConfig
{
    public class PlayerLevelConfig
    {
        /// <summary>
        ///序列（即玩家等级，1~10，最高10级）
        /// </summary>
        public int Id;
        /// <summary>
        ///升级所需经验（达到该值升到下一级；满级10级为0）
        /// </summary>
        public int ExpToNext;
        /// <summary>
        ///上阵格子数（10级后9个格子全解锁）
        /// </summary>
        public int SlotCount;
        /// <summary>
        ///步兵数量（10级达到最大5）
        /// </summary>
        public int MeleeCount;
        /// <summary>
        ///弓兵数量（10级达到最大3）
        /// </summary>
        public int RangedCount;


        public PlayerLevelConfig(int Id, int ExpToNext, int SlotCount, int MeleeCount, int RangedCount)
        {
            this.Id = Id;
            this.ExpToNext = ExpToNext;
            this.SlotCount = SlotCount;
            this.MeleeCount = MeleeCount;
            this.RangedCount = RangedCount;

        }

        public PlayerLevelConfig() { }

        private static Dictionary<int, PlayerLevelConfig> config = new Dictionary<int, PlayerLevelConfig>();
        public static Dictionary<int, PlayerLevelConfig>.ValueCollection ConfigList
        {
            get
            {
                return config.Values;
            }
        }

        public static void Refresh(Dictionary<int, PlayerLevelConfig> dict)
        {
            config.Clear();
            config = dict;
            RebuildIndex();
        }
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
            {"ExpToNext", new FieldMetaInfo("升级所需经验", "int", 60)},
            {"SlotCount", new FieldMetaInfo("上阵格子数", "int", 60)},
            {"MeleeCount", new FieldMetaInfo("步兵数量", "int", 60)},
            {"RangedCount", new FieldMetaInfo("弓兵数量", "int", 60)},
        };

        public static Dictionary<string, FieldMetaInfo> FieldMeta { get { return fieldMeta; } }

        private static List<CellMeta> cellMeta = new List<CellMeta>();
        public static List<CellMeta> CellMetas { get { return cellMeta; } }

        public static void Load()
        {
            config.Clear();
            // 士兵数量与玩家等级挂钩：步兵1→5、弓兵0→3（10级达最大）；士兵攻防加成由士兵等级(SoldierLevelConfig)决定
            config[1] = new PlayerLevelConfig(1, 4, 2, 1, 0);
            config[2] = new PlayerLevelConfig(2, 8, 3, 1, 0);
            config[3] = new PlayerLevelConfig(3, 12, 4, 2, 0);
            config[4] = new PlayerLevelConfig(4, 20, 5, 2, 1);
            config[5] = new PlayerLevelConfig(5, 40, 6, 3, 1);
            config[6] = new PlayerLevelConfig(6, 72, 7, 3, 2);
            config[7] = new PlayerLevelConfig(7, 112, 8, 4, 2);
            config[8] = new PlayerLevelConfig(8, 160, 9, 4, 3);
            config[9] = new PlayerLevelConfig(9, 200, 9, 5, 3);
            config[10] = new PlayerLevelConfig(10, 0, 9, 5, 3);

            RebuildIndex();

        }

        private static void RebuildIndex()
        {
            foreach (var kv in config)
            {
            }
        }

        public static PlayerLevelConfig GetConfig(int id)
        {
            PlayerLevelConfig data;
            if (config.TryGetValue(id, out data))
            {
                return data;
            }
            throw new NullReferenceException(string.Format("配置表PlayerLevelConfig不存在id={0}", id));
        }


        public static bool HasConfig(int id)
        {
            if (config.ContainsKey(id))
            {
                return true;
            }
            return false;
        }

        public static void Assign(int id, PlayerLevelConfig configData)
        {
            config[id] = configData; 
        }

        public static void Add(int id, PlayerLevelConfig configData)
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
