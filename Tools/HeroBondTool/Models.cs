using System;
using System.Collections.Generic;

namespace HeroBondTool
{
    /// <summary>英雄配置（HeroConfig_s.cs 的剧本列已解析为 品质:阵营）。</summary>
    public sealed class HeroInfo
    {
        public int Id;
        public string Name;
        public string Job;
        public string[] Scenarios = new string[3];

        /// <summary>取指定剧本的原始字段（"品质:阵营"，空=不出场）。</summary>
        public string GetRaw(int scenarioId)
        {
            if (scenarioId < 1 || scenarioId > 3)
                return "";
            return Scenarios[scenarioId - 1] ?? "";
        }

        /// <summary>剧本品质；不出场或格式非法返回 0。</summary>
        public int GetQuality(int scenarioId)
        {
            string raw = GetRaw(scenarioId);
            if (string.IsNullOrEmpty(raw))
                return 0;
            int pos = raw.IndexOf(':');
            if (pos < 0)
                return 0;
            int q;
            return int.TryParse(raw.Substring(0, pos).Trim(), out q) ? q : 0;
        }

        /// <summary>剧本阵营短名；不出场返回空。</summary>
        public string GetSide(int scenarioId)
        {
            string raw = GetRaw(scenarioId);
            if (string.IsNullOrEmpty(raw))
                return "";
            int pos = raw.IndexOf(':');
            return pos < 0 ? "" : raw.Substring(pos + 1).Trim();
        }

        public bool IsPresent(int scenarioId)
        {
            return GetQuality(scenarioId) > 0 && !string.IsNullOrEmpty(GetSide(scenarioId));
        }
    }

    /// <summary>好友羁绊组（HeroFriendConfig_s.cs 一行）。</summary>
    public sealed class FriendGroup
    {
        public int Id;
        public string Name;
        public int Level;
        public string Type = "";
        public List<int> Heros = new List<int>();
        public string SkillId = "";
        public string LineColor = "";
    }

    public sealed class ForceInfo
    {
        public int Id;
        public string Name;
        public string Short;
        public string Color;
    }

    public sealed class JobInfo
    {
        public int Id;
        public string Name;
        public string Short;
    }

    public sealed class ScenarioInfo
    {
        public int Id;
        public string Name;
    }

    /// <summary>一次加载得到的全部配置数据与保存状态。</summary>
    public sealed class DataStore
    {
        public string RootDir;
        public string HeroPath;
        public string FriendPath;
        public string ForcePath;
        public string JobPath;
        public string ScenarioPath;

        public List<HeroInfo> Heroes = new List<HeroInfo>();
        public Dictionary<int, HeroInfo> HeroById = new Dictionary<int, HeroInfo>();
        public List<FriendGroup> Groups = new List<FriendGroup>();
        public Dictionary<string, ForceInfo> ForcesByShort = new Dictionary<string, ForceInfo>();
        public Dictionary<string, JobInfo> JobsByShort = new Dictionary<string, JobInfo>();
        public Dictionary<string, int> ForceOrder = new Dictionary<string, int>();
        public Dictionary<string, int> JobOrder = new Dictionary<string, int>();
        public List<ScenarioInfo> Scenarios = new List<ScenarioInfo>();

        /// <summary>本次编辑后待写回文件的羁绊组 Id。</summary>
        public HashSet<int> DirtyGroupIds = new HashSet<int>();

        public bool IsDirty
        {
            get { return DirtyGroupIds.Count > 0; }
        }

        public JobInfo GetJob(string shortName)
        {
            if (string.IsNullOrEmpty(shortName))
                return null;
            JobInfo job;
            return JobsByShort.TryGetValue(shortName, out job) ? job : null;
        }

        public ForceInfo GetForce(string shortName)
        {
            if (string.IsNullOrEmpty(shortName))
                return null;
            ForceInfo force;
            return ForcesByShort.TryGetValue(shortName, out force) ? force : null;
        }

        /// <summary>把英雄从一个羁绊组移动到另一个（源组移除，目标组追加）。</summary>
        public bool MoveHero(int heroId, int fromGroupId, int toGroupId)
        {
            if (toGroupId == fromGroupId)
                return false;
            FriendGroup from = Groups.Find(g => g.Id == fromGroupId);
            FriendGroup to = Groups.Find(g => g.Id == toGroupId);
            if (from == null || to == null)
                return false;
            if (!from.Heros.Contains(heroId) || to.Heros.Contains(heroId))
                return false;

            from.Heros.Remove(heroId);
            to.Heros.Add(heroId);
            DirtyGroupIds.Add(from.Id);
            DirtyGroupIds.Add(to.Id);
            return true;
        }
    }
}
