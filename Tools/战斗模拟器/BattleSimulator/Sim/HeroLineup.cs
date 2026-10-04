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
