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
        ///名字
        /// </summary>
        public string NameS;
        /// <summary>
        ///脚本名
        /// </summary>
        public int SkillId;
        /// <summary>
        ///元职业
        /// </summary>
        public int SourceJob;
        /// <summary>
        ///强克制
        /// </summary>
        public string OvercomeStrong;
        /// <summary>
        ///弱克制
        /// </summary>
        public string OvercomeWeak;
        /// <summary>
        ///射程（近战17 弓50 弩70 炮50 扇/相/谋/鼓/乐/医35）
        /// </summary>
        public int Range;
        /// <summary>
        ///移动速度（帅/士/盾/刀/枪/戟10 马/车12 弓/炮/扇/相/谋/鼓/乐/医8 弩7）
        /// </summary>
        public int MoveSpeed;
        /// <summary>
        ///攻击间隔秒数（职业基准，各职业统一1.5）
        /// </summary>
        public float AtkSpeed;
        /// <summary>
        ///护甲（职业基准值）
        /// </summary>
        public int Armor;
        /// <summary>
        ///魔抗（职业基准值）
        /// </summary>
        public int MagicRes;


        public JobConfig(int Id, string Name, string NameS, int SkillId, int SourceJob, string OvercomeStrong, string OvercomeWeak, int Range, int MoveSpeed, float AtkSpeed, int Armor, int MagicRes)
        {
            this.Id = Id;
            this.Name = Name;
            this.NameS = NameS;
            this.SkillId = SkillId;
            this.SourceJob = SourceJob;
            this.OvercomeStrong = OvercomeStrong;
            this.OvercomeWeak = OvercomeWeak;
            this.Range = Range;
            this.MoveSpeed = MoveSpeed;
            this.AtkSpeed = AtkSpeed;
            this.Armor = Armor;
            this.MagicRes = MagicRes;

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
            config[1] = new JobConfig(1, "shuai", "帅", 200001, 0, "", "", 17, 10, 1.5f, 40, 35);
            config[101] = new JobConfig(101, "ma", "马", 200005, 0, "戟弩炮车", "弓", 17, 12, 1.5f, 55, 45);
            config[102] = new JobConfig(102, "mache", "车", 200011, 101, "戟弩弓", "炮", 17, 12, 1.5f, 55, 45);
            config[201] = new JobConfig(201, "gong", "弓", 200007, 0, "枪戟", "刀", 50, 8, 1.5f, 25, 30);
            config[202] = new JobConfig(202, "gongnu", "弩", 200010, 201, "枪戟", "刀", 70, 7, 1.5f, 25, 30);
            config[203] = new JobConfig(203, "gongpao", "炮", 200009, 201, "枪戟", "刀", 50, 8, 1.5f, 25, 30);
            config[301] = new JobConfig(301, "shi", "士", 200004, 0, "", "弓弩炮", 17, 10, 1.5f, 45, 35);
            config[302] = new JobConfig(302, "shidun", "盾", 0, 301, "弓弩炮", "", 17, 10, 1.5f, 45, 35);
            config[401] = new JobConfig(401, "shan", "扇", 200002, 0, "", "", 35, 8, 1.5f, 25, 35);
            config[402] = new JobConfig(402, "shanxiang", "相", 200006, 401, "", "", 35, 8, 1.5f, 35, 45);
            config[403] = new JobConfig(403, "shanmou", "谋", 200008, 401, "", "", 35, 8, 1.5f, 35, 45);
            config[501] = new JobConfig(501, "gu", "鼓", 200016, 0, "", "", 35, 8, 1.5f, 25, 35);
            config[502] = new JobConfig(502, "gusong", "乐", 200012, 501, "", "", 35, 8, 1.5f, 15, 25);
            config[503] = new JobConfig(503, "guyi", "医", 200013, 501, "", "", 35, 8, 1.5f, 25, 35);
            config[601] = new JobConfig(601, "dao", "刀", 200003, 0, "盾", "士", 17, 10, 1.5f, 25, 15);
            config[602] = new JobConfig(602, "daoqiang", "枪", 200014, 601, "马车", "", 17, 10, 1.5f, 55, 45);
            config[603] = new JobConfig(603, "daoji", "戟", 200015, 601, "枪", "", 17, 10, 1.5f, 45, 35);

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
