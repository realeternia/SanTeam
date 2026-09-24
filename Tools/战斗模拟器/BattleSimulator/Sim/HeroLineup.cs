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

    // 随机选默认阵容：从配置里按座次偏移挑 5 个不同英雄
    public static List<int> PickRandomLineup(int seat)
    {
        var all = AllHeroIds();
        if (all.Count == 0)
            return new List<int>();
        int startIdx = (seat * 37 + 13) % all.Count;
        var lineup = new List<int>();
        for (int i = 0; i < all.Count && lineup.Count < 5; i++)
        {
            var id = all[(startIdx + i) % all.Count];
            if (!lineup.Contains(id))
                lineup.Add(id);
        }
        return lineup;
    }
}
