using System;
using System.Collections;
using System.Collections.Generic;

namespace CommonConfig
{
    public class HeroFriendConfig
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
            {"Id", new FieldMetaInfo("序列", "int", 60)},
            {"Name", new FieldMetaInfo("名字", "string", 0)},
            {"Level", new FieldMetaInfo("支援级别（3最高，1最低（比如共事））", "int", 60)},
            {"Type", new FieldMetaInfo("羁绊类型（智谋/伤害/状态/辅助等）", "string", 60)},
            {"Heros", new FieldMetaInfo("英雄列表，一组 5-8 人", "int[]", 686)},
            {"SkillId", new FieldMetaInfo("关联技能缩写", "string", 60)},
            {"LineColor", new FieldMetaInfo("连线颜色", "string", 0)},
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
        public string Name;
        /// <summary>
        ///支援级别（3最高，1最低（比如共事））
        /// </summary>
        public int Level;
        /// <summary>
        ///羁绊类型（智谋/伤害/状态/辅助等）
        /// </summary>
        public string Type;
        /// <summary>
        ///英雄列表，一组 5-8 人
        /// </summary>
        public int[] Heros;
        /// <summary>
        ///关联技能（>0 表示特殊连锁：激活该技能且不再加属性；=0 表示普通连线加属性）
        /// </summary>
        public string SkillId;
        /// <summary>
        ///连线颜色（特殊连锁的拉线颜色，HTML色值如 #FF0000）
        /// </summary>
        public string LineColor;


        public HeroFriendConfig(int Id, string Name, int Level, string Type, int[] Heros, string SkillId, string LineColor)
        {
            this.Id = Id;
            this.Name = Name;
            this.Level = Level;
            this.Type = Type;
            this.Heros = Heros;
            this.SkillId = SkillId;
            this.LineColor = LineColor;
        }

        public HeroFriendConfig() { }

        private static Dictionary<int, HeroFriendConfig> config = new Dictionary<int, HeroFriendConfig>();
        public static Dictionary<int, HeroFriendConfig>.ValueCollection ConfigList
        {
            get { return config.Values; }
        }

        public static void Refresh(Dictionary<int, HeroFriendConfig> dict)
        {
            config.Clear();
            config = dict;
            RebuildIndex();
        }

        public static void Load()
        {
            config.Clear();
            config[1] = new HeroFriendConfig(1, "无双", 1, "伤害", new int[]{101002,104001,102014,102006,103010}, "双", "#3399CC");
            config[2] = new HeroFriendConfig(2, "野火", 1, "智谋", new int[]{101004,103008,104003,101012,101018,101020}, "火", "#FF0000");
            config[3] = new HeroFriendConfig(3, "骑射", 1, "伤害", new int[]{103004,104009,105008,106004,101008,102023}, "冷", "#0077FF");
            config[4] = new HeroFriendConfig(4, "风华", 1, "伤害", new int[]{104004,103001,101023,103008,102024,102025,101010}, "华", "#3789C2");
            config[5] = new HeroFriendConfig(5, "仁德", 1, "辅助", new int[]{110003,100001,101015,103007,103016,110011,101005}, "仁", "#FFAA33");
            config[6] = new HeroFriendConfig(6, "异禀", 1, "状态", new int[]{100003,110011,110007,101014,101019,105001,102001}, "异", "#FFCC00");
            config[7] = new HeroFriendConfig(7, "济世", 1, "辅助", new int[]{103012,102020,101017,100005,101012,102022,101019}, "济", "#FFD700");
            config[8] = new HeroFriendConfig(8, "益壮", 1, "状态", new int[]{101008,101022,103005,102005,103006,100002}, "老", "#CC7A00");
            config[9] = new HeroFriendConfig(9, "陷阵", 1, "伤害", new int[]{102004,103019,101001,102009,104006,106003,103022}, "破", "#FF6600");
            config[10] = new HeroFriendConfig(10, "深谋", 1, "智谋", new int[]{102011,104007,106003,106008,102016,102001}, "谋", "#0000FF");
            config[11] = new HeroFriendConfig(11, "严明", 1, "辅助", new int[]{101003,102017,102012,106006,102021,103012}, "律", "#556688");
            config[12] = new HeroFriendConfig(12, "王佐", 1, "辅助", new int[]{102010,102003,101006,105007,103011}, "佐", "#AA8800");
            config[13] = new HeroFriendConfig(13, "迅捷", 1, "伤害", new int[]{106001,102008,104002,102007,110009,101024}, "连", "#339933");
            config[14] = new HeroFriendConfig(14, "悍将", 1, "伤害", new int[]{101005,106005,105009,104002,103021,106002,102004}, "穿", "#FF3399");
            config[15] = new HeroFriendConfig(15, "义胆", 1, "辅助", new int[]{101001,101009,104008,101002,104004,110002}, "护", "#453572");
            config[17] = new HeroFriendConfig(17, "望族", 1, "状态", new int[]{103002,100006,101020,103001,102002,103015,103014}, "族", "#996633");
            config[18] = new HeroFriendConfig(18, "将相", 1, "状态", new int[]{102016,105005,103007,103011,105002}, "兼", "#5588AA");
            config[19] = new HeroFriendConfig(19, "自守", 1, "状态", new int[]{102008,102012,103016,101021,101007,104007,105003}, "刺", "#997757");
            config[21] = new HeroFriendConfig(21, "温良", 1, "状态", new int[]{102003,103017,101016,110006,103006,103013}, "俭", "#88AA66");
            config[22] = new HeroFriendConfig(22, "背水", 1, "辅助", new int[]{102015,102014,110008,101013,102007,103003}, "背", "#773599");
            config[23] = new HeroFriendConfig(23, "偷袭", 1, "伤害", new int[]{105001,101007,103020,102023,103010,102013,104003}, "偷", "#553333");
            config[24] = new HeroFriendConfig(24, "文采", 1, "辅助", new int[]{100002,102024,103013,105003,105006,104008}, "文", "#7A5CB8");
            config[26] = new HeroFriendConfig(26, "铁骑", 1, "伤害", new int[]{101003,106001,106002,103009,102005,102006}, "冲", "#FF9933");
            config[27] = new HeroFriendConfig(27, "智勇", 1, "智谋", new int[]{102019,103004,102009,100003,101010,101006}, "镜", "#CC99CC");
            config[29] = new HeroFriendConfig(29, "虎狼", 1, "伤害", new int[]{101011,104001,103018,100004,102002,103003}, "突", "#FF3366");
            config[31] = new HeroFriendConfig(31, "枭雄", 1, "状态", new int[]{100004,110007,101024,103002,110005,104009}, "枭", "#CC0000");
            config[32] = new HeroFriendConfig(32, "安民", 1, "辅助", new int[]{106007,100001,103023,102018,102010,103014}, "民", "#339933");

            RebuildIndex();

        }

        private static void RebuildIndex()
        {
            foreach (var kv in config)
            {
            }
        }

        public static HeroFriendConfig GetConfig(int id)
        {
            HeroFriendConfig data;
            if (config.TryGetValue(id, out data))
            {
                return data;
            }
            throw new NullReferenceException(string.Format("配置表HeroFriendConfig不存在id={0}", id));
        }


        public static bool HasConfig(int id)
        {
            if (config.ContainsKey(id))
            {
                return true;
            }
            return false;
        }

        public static void Assign(int id, HeroFriendConfig configData)
        {
            config[id] = configData; 
        }

        public static void Add(int id, HeroFriendConfig configData)
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
