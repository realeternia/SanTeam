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
            // 品质2装备合成（两块品质1材料 1:1 → 品质2装备各1件）
            // 材料：402001长刀=攻击 402002皮革甲=护甲 402003檀木弓=攻速 402004斗篷=魔抗 402005葫芦=法力回复 402006护手=暴击 402007羽扇=法强 402008名马=生命
            config[1] = new ItemCombineConfig(1, 402001, 1, 402002, 1, 400001, 1); // 长刀+皮革甲 → 关王刀(atk+armor)
            config[2] = new ItemCombineConfig(2, 402001, 1, 402006, 1, 400002, 1); // 长刀+护手 → 方天画戟(atk+crit)
            config[3] = new ItemCombineConfig(3, 402001, 1, 402008, 1, 400003, 1); // 长刀+名马 → 丈八蛇矛(atk+hp)
            config[4] = new ItemCombineConfig(4, 402007, 1, 402001, 1, 400007, 1); // 羽扇+长刀 → 孙子兵法(ap+atk)
            config[5] = new ItemCombineConfig(5, 402007, 1, 402008, 1, 400008, 1); // 羽扇+名马 → 诗经(ap+hp)
            config[6] = new ItemCombineConfig(6, 402007, 1, 402005, 1, 400011, 1); // 羽扇+葫芦 → 易经(ap+mpRegen)
            config[7] = new ItemCombineConfig(7, 402007, 1, 402004, 1, 400012, 1); // 羽扇+斗篷 → 道德经(ap+magicres)
            config[8] = new ItemCombineConfig(8, 402003, 1, 402004, 1, 400013, 1); // 檀木弓+斗篷 → 赤兔马(atkspeed+magicres)
            config[9] = new ItemCombineConfig(9, 402008, 1, 402005, 1, 400014, 1); // 名马+葫芦 → 的卢马(hp+hpRegen)（hpRegen 无专属品质1材料，就近取葫芦）
            config[10] = new ItemCombineConfig(10, 402003, 1, 402005, 1, 402009, 1); // 檀木弓+葫芦 → 李广弓(atkspeed+mpRegen)
            config[11] = new ItemCombineConfig(11, 402008, 1, 402003, 1, 402010, 1); // 名马+檀木弓 → 养由基弓(atkspeed+hp)
            // 补全：保证每个品质1材料（402001~402008）都至少有4条合成路径（当前每样各5条）
            config[12] = new ItemCombineConfig(12, 402002, 1, 402003, 1, 400016, 1); // 皮革甲+檀木弓 → 飞羽甲(armor+atkspeed)
            config[13] = new ItemCombineConfig(13, 402002, 1, 402004, 1, 400017, 1); // 皮革甲+斗篷 → 明光铠(armor+magicres)
            config[14] = new ItemCombineConfig(14, 402002, 1, 402006, 1, 400018, 1); // 皮革甲+护手 → 兽面吞头铠(armor+crit)
            config[15] = new ItemCombineConfig(15, 402002, 1, 402007, 1, 400019, 1); // 皮革甲+羽扇 → 八卦袍(ap+armor)
            config[16] = new ItemCombineConfig(16, 402002, 1, 402008, 1, 400020, 1); // 皮革甲+名马 → 西凉铁骑(armor+hp)
            config[17] = new ItemCombineConfig(17, 402003, 1, 402006, 1, 400021, 1); // 檀木弓+护手 → 穿云弓(atkspeed+crit)
            config[18] = new ItemCombineConfig(18, 402004, 1, 402006, 1, 400022, 1); // 斗篷+护手 → 锦帆斗篷(crit+magicres)
            config[19] = new ItemCombineConfig(19, 402004, 1, 402005, 1, 400023, 1); // 斗篷+葫芦 → 鹤氅(magicres+mpRegen)
            config[20] = new ItemCombineConfig(20, 402004, 1, 402008, 1, 400024, 1); // 斗篷+名马 → 绝影(hp+magicres)
            config[21] = new ItemCombineConfig(21, 402001, 1, 402005, 1, 400025, 1); // 长刀+葫芦 → 古锭刀(atk+mpRegen)
            // 补齐 7 条缺失的不同材料组合（与 1~21 合构成完整的 28 种不同配对）
            config[22] = new ItemCombineConfig(22, 402001, 1, 402003, 1, 400026, 1); // 长刀+檀木弓 → 淬毒长弓(atk+atkspeed)
            config[23] = new ItemCombineConfig(23, 402001, 1, 402004, 1, 400027, 1); // 长刀+斗篷 → 破军玄刀(atk+magicres)
            config[24] = new ItemCombineConfig(24, 402002, 1, 402005, 1, 400028, 1); // 皮革甲+葫芦 → 活力玄甲(armor+mpRegen)
            config[25] = new ItemCombineConfig(25, 402006, 1, 402007, 1, 400029, 1); // 护手+羽扇 → 灵犀扇(crit+ap)
            config[26] = new ItemCombineConfig(26, 402006, 1, 402008, 1, 400030, 1); // 护手+名马 → 赤纹蹄印(crit+hp)
            config[27] = new ItemCombineConfig(27, 402006, 1, 402005, 1, 400031, 1); // 护手+葫芦 → 聚灵护手(crit+mpRegen)
            config[28] = new ItemCombineConfig(28, 402007, 1, 402003, 1, 400032, 1); // 羽扇+檀木弓 → 追风羽扇(ap+atkspeed)
            // 8 条两个相同材料合成 → 聚焦单属性品质2（ItemA==ItemB，需同 id 2 件；AI自动合成里按两份数量校验）
            config[29] = new ItemCombineConfig(29, 402001, 1, 402001, 1, 400033, 1); // 长刀+长刀 → 双股剑(atk+40)
            config[30] = new ItemCombineConfig(30, 402002, 1, 402002, 1, 400034, 1); // 皮革甲+皮革甲 → 重装玄甲(armor+40)
            config[31] = new ItemCombineConfig(31, 402003, 1, 402003, 1, 400035, 1); // 檀木弓+檀木弓 → 连环弩(atkspeed+30%)
            config[32] = new ItemCombineConfig(32, 402004, 1, 402004, 1, 400036, 1); // 斗篷+斗篷 → 玄光法袍(magicres+40)
            config[33] = new ItemCombineConfig(33, 402005, 1, 402005, 1, 400037, 1); // 葫芦+葫芦 → 玉京葫芦(mpRegen+4)
            config[34] = new ItemCombineConfig(34, 402006, 1, 402006, 1, 400038, 1); // 护手+护手 → 风雷双护手(crit+35%)
            config[35] = new ItemCombineConfig(35, 402007, 1, 402007, 1, 400039, 1); // 羽扇+羽扇 → 鹤骨羽扇(ap+25)
            config[36] = new ItemCombineConfig(36, 402008, 1, 402008, 1, 400040, 1); // 名马+名马 → 天外飞驹(hp+500)
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