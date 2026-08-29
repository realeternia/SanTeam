using System;
using System.Collections;
using System.Collections.Generic;

namespace CommonConfig
{
    public class SoldierLevelConfig
    {
        /// <summary>
        ///等级（1~30，最高30级）
        /// </summary>
        public int Id;
        /// <summary>
        ///步兵攻击加成
        /// </summary>
        public int AtkAdd;
        /// <summary>
        ///步兵生命加成
        /// </summary>
        public int HpAdd;
        /// <summary>
        ///步兵数量（20级达到最大5）
        /// </summary>
        public int MeleeCount;
        /// <summary>
        ///弓兵数量（20级达到最大3）
        /// </summary>
        public int RangedCount;


        public SoldierLevelConfig(int Id, int AtkAdd, int HpAdd, int MeleeCount, int RangedCount)
        {
            this.Id = Id;
            this.AtkAdd = AtkAdd;
            this.HpAdd = HpAdd;
            this.MeleeCount = MeleeCount;
            this.RangedCount = RangedCount;

        }

        public SoldierLevelConfig() { }

        private static Dictionary<int, SoldierLevelConfig> config = new Dictionary<int, SoldierLevelConfig>();
        public static Dictionary<int, SoldierLevelConfig>.ValueCollection ConfigList
        {
            get
            {
                return config.Values;
            }
        }

        public static void Refresh(Dictionary<int, SoldierLevelConfig> dict)
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
            {"Id", new FieldMetaInfo("等级", "int", 60)},
            {"AtkAdd", new FieldMetaInfo("步兵攻击加成", "int", 60)},
            {"HpAdd", new FieldMetaInfo("步兵生命加成", "int", 60)},
            {"MeleeCount", new FieldMetaInfo("步兵数量", "int", 60)},
            {"RangedCount", new FieldMetaInfo("弓兵数量", "int", 60)},
        };

        public static Dictionary<string, FieldMetaInfo> FieldMeta { get { return fieldMeta; } }

        private static List<CellMeta> cellMeta = new List<CellMeta>();
        public static List<CellMeta> CellMetas { get { return cellMeta; } }

        public static void Load()
        {
            config.Clear();
            // 步兵数量：1~4级1个，5~8级2个，9~12级3个，13~16级4个，17级起5个（20级达最大）
            // 弓兵数量：1~6级0个，7~11级1个，12~16级2个，17级起3个（20级达最大）
            // 攻防加成：每级 Atk+2、Hp+10（30级 Atk+58、Hp+290）
            config[1] = new SoldierLevelConfig(1, 0, 0, 1, 0);
            config[2] = new SoldierLevelConfig(2, 2, 10, 1, 0);
            config[3] = new SoldierLevelConfig(3, 4, 20, 1, 0);
            config[4] = new SoldierLevelConfig(4, 6, 30, 1, 0);
            config[5] = new SoldierLevelConfig(5, 8, 40, 2, 0);
            config[6] = new SoldierLevelConfig(6, 10, 50, 2, 0);
            config[7] = new SoldierLevelConfig(7, 12, 60, 2, 1);
            config[8] = new SoldierLevelConfig(8, 14, 70, 2, 1);
            config[9] = new SoldierLevelConfig(9, 16, 80, 3, 1);
            config[10] = new SoldierLevelConfig(10, 18, 90, 3, 1);
            config[11] = new SoldierLevelConfig(11, 20, 100, 3, 1);
            config[12] = new SoldierLevelConfig(12, 22, 110, 3, 2);
            config[13] = new SoldierLevelConfig(13, 24, 120, 4, 2);
            config[14] = new SoldierLevelConfig(14, 26, 130, 4, 2);
            config[15] = new SoldierLevelConfig(15, 28, 140, 4, 2);
            config[16] = new SoldierLevelConfig(16, 30, 150, 4, 2);
            config[17] = new SoldierLevelConfig(17, 32, 160, 5, 3);
            config[18] = new SoldierLevelConfig(18, 34, 170, 5, 3);
            config[19] = new SoldierLevelConfig(19, 36, 180, 5, 3);
            config[20] = new SoldierLevelConfig(20, 38, 190, 5, 3);
            config[21] = new SoldierLevelConfig(21, 40, 200, 5, 3);
            config[22] = new SoldierLevelConfig(22, 42, 210, 5, 3);
            config[23] = new SoldierLevelConfig(23, 44, 220, 5, 3);
            config[24] = new SoldierLevelConfig(24, 46, 230, 5, 3);
            config[25] = new SoldierLevelConfig(25, 48, 240, 5, 3);
            config[26] = new SoldierLevelConfig(26, 50, 250, 5, 3);
            config[27] = new SoldierLevelConfig(27, 52, 260, 5, 3);
            config[28] = new SoldierLevelConfig(28, 54, 270, 5, 3);
            config[29] = new SoldierLevelConfig(29, 56, 280, 5, 3);
            config[30] = new SoldierLevelConfig(30, 58, 290, 5, 3);

            RebuildIndex();

        }

        private static void RebuildIndex()
        {
            foreach (var kv in config)
            {
            }
        }

        public static SoldierLevelConfig GetConfig(int id)
        {
            SoldierLevelConfig data;
            if (config.TryGetValue(id, out data))
            {
                return data;
            }
            throw new NullReferenceException(string.Format("配置表SoldierLevelConfig不存在id={0}", id));
        }


        public static bool HasConfig(int id)
        {
            if (config.ContainsKey(id))
            {
                return true;
            }
            return false;
        }

        public static void Assign(int id, SoldierLevelConfig configData)
        {
            config[id] = configData; 
        }

        public static void Add(int id, SoldierLevelConfig configData)
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
