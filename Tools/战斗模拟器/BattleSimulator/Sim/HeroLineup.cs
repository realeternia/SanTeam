// ============================================================
// 战斗模拟器 · 阵容工具（Program 入口与 CLI 共用）
// ============================================================
using System;
using System.Collections.Generic;
using System.Linq;
using CommonConfig;

public static class HeroLineup
{
    // 解析 "id,id,id" 字符串为英雄 id 列表
    public static List<int> ParseHeroList(string s)
    {
        var list = new List<int>();
        if (!string.IsNullOrEmpty(s))
        {
            foreach (var part in s.Split(','))
            {
                if (int.TryParse(part.Trim(), out int id))
                    list.Add(id);
            }
        }
        return list;
    }

    // 全部英雄卡 id（去重）
    public static List<int> AllHeroIds()
    {
        return HeroConfig.ConfigList
            .Where(h => ConfigManager.IsHeroCard((int)h.Id))
            .Select(h => (int)h.Id)
            .Distinct()
            .ToList();
    }

    // 近战英雄池（HeroConfig.Range <= 20）
    public static List<int> MeleeHeroIds()
    {
        return AllHeroIds()
            .Where(id => HeroSelectionTool.IsMeleeHero(HeroConfig.GetConfig(id)))
            .ToList();
    }

    // 远程英雄池（HeroConfig.Range > 20）
    public static List<int> RangedHeroIds()
    {
        return AllHeroIds()
            .Where(id => HeroSelectionTool.IsRangedHero(HeroConfig.GetConfig(id)))
            .ToList();
    }

    // 随机抽 count 名武将：近战/远程各占一半，奇数时多出的 1 个从全体随机
    // 抽中结果写入 used，保证同一场双方不出现相同武将
    public static List<int> PickByRole(int count, HashSet<int> used)
    {
        if (used == null)
            used = new HashSet<int>();
        int meleeCount = count / 2;
        int rangedCount = count / 2;
        int randomCount = count - meleeCount - rangedCount;

        var result = new List<int>();
        DrawRandom(result, MeleeHeroIds(), meleeCount, used);
        DrawRandom(result, RangedHeroIds(), rangedCount, used);
        DrawRandom(result, AllHeroIds(), randomCount, used);
        // 池子不足时用全体补齐，保证数量
        DrawRandom(result, AllHeroIds(), count - result.Count, used);
        return result;
    }

    // ---- 成组抽阵容（国家/好友/职业）----

    // 组定义：类型（国家/好友/职业）+ 组名 + 成员英雄 id
    public class GroupDef
    {
        public string Type;
        public string Name;
        public List<int> Heroes;
    }

    public const string GroupTypeFaction = "国家";
    public const string GroupTypeFriend = "好友";
    public const string GroupTypeJob = "职业";

    // 组类型清单（报告与抽取固定顺序）
    public static readonly string[] GroupTypes = { GroupTypeFaction, GroupTypeFriend, GroupTypeJob };

    // 半随机成组阵容：先从"已开启且仍有可用成员"的类型里随机挑 1 种，再从该类型里随机挑 1 个组，
    // 组内取 count/2 人（成组），其余按近战/远程各半补齐（不强求）；used 保证同场双方不重复。
    // chosen 返回本侧选中的组（供成组胜率统计）；未开启任何类型时 chosen=null，退化为普通远近随机。
    public static List<int> PickGroupLineup(int count, IList<string> enabledTypes, HashSet<int> used, out GroupDef chosen)
    {
        chosen = null;
        if (used == null)
            used = new HashSet<int>();
        var result = new List<int>();
        if (count <= 0)
            return result;

        int groupCount = count / 2;
        if (groupCount > 0 && enabledTypes != null && enabledTypes.Count > 0)
        {
            // 收集每个已开启类型下"仍有可用成员"的组
            var typeCandidates = new List<KeyValuePair<string, List<GroupCandidate>>>();
            foreach (var type in enabledTypes.Distinct())
            {
                var list = new List<GroupCandidate>();
                foreach (var g in GroupsOf(type))
                {
                    var avail = g.Heroes.Where(h => !used.Contains(h)).ToList();
                    if (avail.Count > 0)
                        list.Add(new GroupCandidate { Group = g, Available = avail });
                }
                if (list.Count > 0)
                    typeCandidates.Add(new KeyValuePair<string, List<GroupCandidate>>(type, list));
            }

            if (typeCandidates.Count > 0)
            {
                // 优先从"存在足量成员(>=groupCount)的组"的类型里挑，避免成组人数凑不满
                var eligibleTypes = typeCandidates
                    .Where(kv => kv.Value.Any(c => c.Available.Count >= groupCount)).ToList();
                var typePool = eligibleTypes.Count > 0 ? eligibleTypes : typeCandidates;
                var picked = typePool[SysRandom.Range(0, typePool.Count)];

                var eligibleGroups = picked.Value.Where(c => c.Available.Count >= groupCount).ToList();
                GroupCandidate best;
                if (eligibleGroups.Count > 0)
                {
                    best = eligibleGroups[SysRandom.Range(0, eligibleGroups.Count)];
                }
                else
                {
                    int max = picked.Value.Max(c => c.Available.Count);
                    var top = picked.Value.Where(c => c.Available.Count == max).ToList();
                    best = top[SysRandom.Range(0, top.Count)];
                }

                chosen = best.Group;
                DrawRandom(result, best.Group.Heroes, groupCount, used);
            }
        }

        // 其余名额按近战/远程各半补齐（不强求，池子不足再用全体补齐）
        int remain = count - result.Count;
        if (remain > 0)
        {
            DrawRandom(result, MeleeHeroIds(), remain / 2, used);
            DrawRandom(result, RangedHeroIds(), remain / 2, used);
            DrawRandom(result, AllHeroIds(), count - result.Count, used);
        }
        return result;
    }

    // 按类型取全部组
    public static List<GroupDef> GroupsOf(string type)
    {
        if (type == GroupTypeFaction)
            return FactionGroups();
        if (type == GroupTypeFriend)
            return FriendGroups();
        if (type == GroupTypeJob)
            return JobGroups();
        return new List<GroupDef>();
    }

    // 国家（阵营）组：HeroConfig.Side 分组，组名取 ForceConfig.Name
    private static List<GroupDef> FactionGroups()
    {
        var map = new Dictionary<int, List<int>>();
        foreach (var id in AllHeroIds())
        {
            var cfg = HeroConfig.GetConfig(id);
            if (cfg == null)
                continue;
            List<int> list;
            if (!map.TryGetValue(cfg.Side, out list))
            {
                list = new List<int>();
                map[cfg.Side] = list;
            }
            list.Add(id);
        }
        var result = new List<GroupDef>();
        foreach (var kv in map)
        {
            var forceCfg = ConfigManager.GetForceConfig(kv.Key);
            result.Add(new GroupDef
            {
                Type = GroupTypeFaction,
                Name = forceCfg != null ? forceCfg.Name : ("阵营" + kv.Key),
                Heroes = kv.Value,
            });
        }
        return result;
    }

    // 好友组：HeroFriendConfig.Heros（过滤掉非英雄卡）
    private static List<GroupDef> FriendGroups()
    {
        var valid = new HashSet<int>(AllHeroIds());
        var result = new List<GroupDef>();
        foreach (var cfg in HeroFriendConfig.ConfigList)
        {
            var heroes = (cfg.Heros ?? new int[0]).Where(valid.Contains).Distinct().ToList();
            if (heroes.Count == 0)
                continue;
            result.Add(new GroupDef { Type = GroupTypeFriend, Name = cfg.Name, Heroes = heroes });
        }
        return result;
    }

    // 职业组：HeroConfig.Job（职业缩写）分组，组名取 JobConfig.Name
    private static List<GroupDef> JobGroups()
    {
        var map = new Dictionary<string, List<int>>();
        foreach (var id in AllHeroIds())
        {
            var cfg = HeroConfig.GetConfig(id);
            if (cfg == null || string.IsNullOrEmpty(cfg.Job))
                continue;
            List<int> list;
            if (!map.TryGetValue(cfg.Job, out list))
            {
                list = new List<int>();
                map[cfg.Job] = list;
            }
            list.Add(id);
        }
        var result = new List<GroupDef>();
        foreach (var kv in map)
        {
            var jobCfg = ConfigManager.GetJobConfig(kv.Key);
            result.Add(new GroupDef
            {
                Type = GroupTypeJob,
                Name = jobCfg != null ? jobCfg.Name : kv.Key,
                Heroes = kv.Value,
            });
        }
        return result;
    }

    private class GroupCandidate
    {
        public GroupDef Group;
        public List<int> Available;
    }

    // 随机选默认阵容：按座次偏移挑 count 个不同英雄（近战/远程各半，确定性可复现）
    public static List<int> PickRandomLineup(int seat, int count = 6)
    {
        var all = AllHeroIds();
        if (all.Count == 0)
            return new List<int>();
        int meleeCount = count / 2;
        int rangedCount = count / 2;
        int randomCount = count - meleeCount - rangedCount;

        var lineup = new List<int>();
        AddByOffset(lineup, MeleeHeroIds(), meleeCount, seat * 37 + 13);
        AddByOffset(lineup, RangedHeroIds(), rangedCount, seat * 53 + 7);
        AddByOffset(lineup, all, randomCount, seat * 29 + 3);
        AddByOffset(lineup, all, count - lineup.Count, seat * 17 + 5);
        return lineup;
    }

    // 从池里随机抽至多 need 个未用过的英雄
    private static void DrawRandom(List<int> result, List<int> pool, int need, HashSet<int> used)
    {
        if (need <= 0 || pool.Count == 0)
            return;
        var candidates = pool.Where(x => !used.Contains(x)).ToList();
        Shuffle(candidates);
        int take = Math.Min(need, candidates.Count);
        for (int i = 0; i < take; i++)
        {
            result.Add(candidates[i]);
            used.Add(candidates[i]);
        }
    }

    // 从池里按座次偏移取至多 need 个未选过的英雄
    private static void AddByOffset(List<int> lineup, List<int> pool, int need, int startIdx)
    {
        if (need <= 0 || pool.Count == 0)
            return;
        for (int i = 0; i < pool.Count && need > 0; i++)
        {
            var id = pool[(startIdx + i) % pool.Count];
            if (lineup.Contains(id))
                continue;
            lineup.Add(id);
            need--;
        }
    }

    // Fisher-Yates 洗牌（用 SysRandom 保证可复现）
    private static void Shuffle(List<int> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = SysRandom.Range(0, i + 1);
            int t = list[i];
            list[i] = list[j];
            list[j] = t;
        }
    }
}
