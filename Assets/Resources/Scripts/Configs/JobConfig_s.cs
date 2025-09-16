using System;
using System.Collections;
using System.Collections.Generic;

namespace CommonConfig
{
    public class JobConfig
    {
        /// <summary>
        ///序列
        /// </summary>
        public int Id;
        /// <summary>
        ///名字
        /// </summary>
        public string Name;
        /// <summary>
        ///脚本名
        /// </summary>
        public int SkillId;
        /// <summary>
        ///元职业
        /// </summary>
        public int SourceJob;


        public JobConfig(int Id, string Name, int SkillId, int SourceJob)
        {
            this.Id = Id;
            this.Name = Name;
            this.SkillId = SkillId;
            this.SourceJob = SourceJob;

        }

        public JobConfig() { }

        private static Dictionary<int, JobConfig> config = new Dictionary<int, JobConfig>();
        public static Dictionary<int, JobConfig>.ValueCollection ConfigList
        {
            get
            {
                return config.Values;
            }
        }

        public static void Refresh(Dictionary<int, JobConfig> dict)
        {
            config.Clear();
            config = dict;
        }

        public static void Load()
        {
            config.Clear();
            config[1] = new JobConfig(1, "shuai", 200001, 0);
            config[101] = new JobConfig(101, "ma", 200005, 0);
            config[102] = new JobConfig(102, "mache", 200011, 101);
            config[201] = new JobConfig(201, "gong", 200007, 0);
            config[202] = new JobConfig(202, "gongnu", 200010, 201);
            config[203] = new JobConfig(203, "gongpao", 200009, 201);
            config[301] = new JobConfig(301, "shi", 200004, 0);
            config[302] = new JobConfig(302, "shidun", 0, 301);
            config[401] = new JobConfig(401, "shan", 200002, 0);
            config[402] = new JobConfig(402, "shanxiang", 200006, 401);
            config[403] = new JobConfig(403, "shanmou", 200008, 401);
            config[501] = new JobConfig(501, "gu", 0, 0);
            config[502] = new JobConfig(502, "song", 200012, 501);
            config[503] = new JobConfig(503, "yi", 200013, 501);
            config[601] = new JobConfig(601, "dao", 200003, 0);
            config[602] = new JobConfig(602, "daoqiang", 200014, 601);
            config[603] = new JobConfig(603, "daoji", 200015, 601);

        }

        public static JobConfig GetConfig(int id)
        {
            JobConfig data;
            if (config.TryGetValue(id, out data))
            {
                return data;
            }
            throw new NullReferenceException(string.Format("配置表JobConfig不存在id={0}", id));
        }

        public static bool HasConfig(int id)
        {
            if (config.ContainsKey(id))
            {
                return true;
            }
            return false;
        }

        public static void Assign(int id, JobConfig configData)
        {
            config[id] = configData; 
        }

        public static void Add(int id, JobConfig configData)
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
