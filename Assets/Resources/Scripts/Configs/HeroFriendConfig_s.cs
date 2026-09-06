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
            {"Heros", new FieldMetaInfo("英雄列表，一组 5-6 人", "int[]", 794)},
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
        ///英雄列表，一组 5-6 人
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


        public HeroFriendConfig(int Id, string Name, int Level, int[] Heros, string SkillId, string LineColor)
        {
            this.Id = Id;
            this.Name = Name;
            this.Level = Level;
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
            config[1] = new HeroFriendConfig(1, "万人敌", 1, new int[]{101005,101002,104001,102005,102007}, "", "");
            config[2] = new HeroFriendConfig(2, "神机妙算", 1, new int[]{101004,102001,103008,104003,101012}, "", "");
            config[3] = new HeroFriendConfig(3, "骑射无双", 1, new int[]{103004,103003,102023,104009,105008}, "", "");
            config[4] = new HeroFriendConfig(4, "风华绝代", 1, new int[]{104004,103001,101023,102001,103014}, "", "");
            config[5] = new HeroFriendConfig(5, "仁德济世", 1, new int[]{110003,100001,101015,110009,103007,103016}, "", "");
            config[6] = new HeroFriendConfig(6, "身负异禀", 1, new int[]{100003,110011,110007,101014,101019,105001}, "", "");
            config[7] = new HeroFriendConfig(7, "经世济民", 1, new int[]{103012,102020,101017,100005,101012}, "", "");
            config[8] = new HeroFriendConfig(8, "老当益壮", 1, new int[]{101008,101022,103005,103022,102005}, "", "");
            config[9] = new HeroFriendConfig(9, "先登陷阵", 1, new int[]{102004,103019,101001,105006,102009}, "", "");
            config[10] = new HeroFriendConfig(10, "深谋远虑", 1, new int[]{103010,102011,104007,106003,103012}, "", "");
            config[11] = new HeroFriendConfig(11, "治军严明", 1, new int[]{101003,104006,102017,101013,102012,106006}, "", "");
            config[12] = new HeroFriendConfig(12, "王佐之才", 1, new int[]{101010,103013,102010,102003,101006,105007}, "", "");
            config[13] = new HeroFriendConfig(13, "剽悍迅捷", 1, new int[]{103006,106001,102008,104002,102007}, "", "");
            config[14] = new HeroFriendConfig(14, "骄兵悍将", 1, new int[]{101005,106005,105009,104002,103021,106002}, "", "");
            config[15] = new HeroFriendConfig(15, "忠肝义胆", 1, new int[]{101001,101009,110002,104008,104004,102018}, "", "");
            config[16] = new HeroFriendConfig(16, "权奸当道", 1, new int[]{100004,104007,106008,106004,105002}, "", "");
            config[17] = new HeroFriendConfig(17, "名门望族", 1, new int[]{103002,100006,102025,101020,103001,102002}, "", "");
            config[18] = new HeroFriendConfig(18, "出将入相", 1, new int[]{102016,105005,103007,103011,104003}, "", "");
            config[19] = new HeroFriendConfig(19, "谦冲自守", 1, new int[]{102008,102012,103016,101021,105003,101007}, "", "");
            config[20] = new HeroFriendConfig(20, "身负奇才", 1, new int[]{100002,102022,103008,103010,110011}, "", "");
            config[21] = new HeroFriendConfig(21, "温良恭俭", 1, new int[]{102003,103009,103017,101016,110006,102002}, "", "");
            config[22] = new HeroFriendConfig(22, "短兵搏杀", 1, new int[]{102015,101002,102014,110008,110010}, "", "");
            config[23] = new HeroFriendConfig(23, "偷袭高手", 1, new int[]{103003,102004,105001,101007,103020,102023}, "", "");
            config[24] = new HeroFriendConfig(24, "文采出众", 1, new int[]{100002,101004,102024,101020,103013}, "", "");
            config[25] = new HeroFriendConfig(25, "儒将风范", 1, new int[]{103011,101010,102021,105003,101018}, "", "");
            config[26] = new HeroFriendConfig(26, "铁骑纵横", 1, new int[]{101003,106001,106002,102014,102006}, "冲", "#FF0000");
            config[27] = new HeroFriendConfig(27, "智勇双全", 1, new int[]{102019,103004,102009,100003,104008,101006}, "", "");
            config[28] = new HeroFriendConfig(28, "坚壁善守", 1, new int[]{102016,103015,101008,102013,104009}, "", "");
            config[29] = new HeroFriendConfig(29, "虎狼之师", 1, new int[]{101011,104001,103006,103018,102006}, "破", "#0000FF");
            config[31] = new HeroFriendConfig(31, "乱世枭雄", 1, new int[]{100004,110007,110010,103002,110005}, "", "");
            config[32] = new HeroFriendConfig(32, "济世安民", 1, new int[]{106007,100001,106003,102010,103023,103014}, "", "");

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
