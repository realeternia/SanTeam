using System;
using System.Collections;
using System.Collections.Generic;

namespace CommonConfig
{
    public class SoldierLevelConfig
    {
        /// <summary>
        ///等级（1~30，最高30级；每级成长数值由 SoldierConfig.AtkP/HpP 决定）
        /// </summary>
        public int Id;


        public SoldierLevelConfig(int Id)
        {
            this.Id = Id;

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
        };

        public static Dictionary<string, FieldMetaInfo> FieldMeta { get { return fieldMeta; } }

        private static List<CellMeta> cellMeta = new List<CellMeta>();
        public static List<CellMeta> CellMetas { get { return cellMeta; } }

        public static void Load()
        {
            config.Clear();
            // 本表仅定义士兵等级区间(1~30)；每级攻防成长由 SoldierConfig.AtkP/HpP 决定
            // 士兵等级 = 玩家等级自然升级 + 花金币额外升级，数量由玩家等级(PlayerLevelConfig)决定
            config[1] = new SoldierLevelConfig(1);
            config[2] = new SoldierLevelConfig(2);
            config[3] = new SoldierLevelConfig(3);
            config[4] = new SoldierLevelConfig(4);
            config[5] = new SoldierLevelConfig(5);
            config[6] = new SoldierLevelConfig(6);
            config[7] = new SoldierLevelConfig(7);
            config[8] = new SoldierLevelConfig(8);
            config[9] = new SoldierLevelConfig(9);
            config[10] = new SoldierLevelConfig(10);
            config[11] = new SoldierLevelConfig(11);
            config[12] = new SoldierLevelConfig(12);
            config[13] = new SoldierLevelConfig(13);
            config[14] = new SoldierLevelConfig(14);
            config[15] = new SoldierLevelConfig(15);
            config[16] = new SoldierLevelConfig(16);
            config[17] = new SoldierLevelConfig(17);
            config[18] = new SoldierLevelConfig(18);
            config[19] = new SoldierLevelConfig(19);
            config[20] = new SoldierLevelConfig(20);
            config[21] = new SoldierLevelConfig(21);
            config[22] = new SoldierLevelConfig(22);
            config[23] = new SoldierLevelConfig(23);
            config[24] = new SoldierLevelConfig(24);
            config[25] = new SoldierLevelConfig(25);
            config[26] = new SoldierLevelConfig(26);
            config[27] = new SoldierLevelConfig(27);
            config[28] = new SoldierLevelConfig(28);
            config[29] = new SoldierLevelConfig(29);
            config[30] = new SoldierLevelConfig(30);

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
