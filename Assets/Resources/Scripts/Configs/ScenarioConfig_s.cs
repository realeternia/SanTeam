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
            {"Pic", new FieldMetaInfo("剧本图片（Textures/Scenario 下文件名，如 s1）", "string", 0)},
            {"Des", new FieldMetaInfo("剧本历史描述（年份与历史差异，选择时展示）", "string", 300)},
            {"StartYear", new FieldMetaInfo("剧本开始年份（第1回合对应此年份）", "int", 60)},
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
        /// <summary>
        ///剧本图片（Textures/Scenario 下文件名，如 s1）
        /// </summary>
        public string Pic;
        /// <summary>
        ///剧本历史描述（年份与历史差异，选择时展示）
        /// </summary>
        public string Des;
        /// <summary>
        ///剧本开始年份（第1回合对应此年份）
        /// </summary>
        public int StartYear;


        public ScenarioConfig(int Id, string Name, string Pic, string Des, int StartYear)
        {
            this.Id = Id;
            this.Name = Name;
            this.Pic = Pic;
            this.Des = Des;
            this.StartYear = StartYear;
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
            config[1] = new ScenarioConfig(1, "群雄割据", "s1", "184年·群雄割据：黄巾之乱后汉室衰微，董卓入京、天下大乱，州郡拥兵自立，曹操、袁绍、刘备、孙坚等诸侯并起，群雄逐鹿中原。", 184);
            config[2] = new ScenarioConfig(2, "三国鼎立", "s2", "220年·三国鼎立：曹丕代汉建魏，刘备成都称帝，孙权受封吴王，魏蜀吴三分天下、互相对峙，名将与谋臣辈出、各据一方。", 220);
            config[3] = new ScenarioConfig(3, "三家归晋", "s3", "263年·三家归晋：魏灭蜀，265年司马炎代魏建晋，280年晋军灭吴，天下重归一统，开国名将相继凋零，守成之将登上舞台。", 263);

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
