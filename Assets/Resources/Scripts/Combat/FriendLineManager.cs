using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 连线(武将关系)初始化：战斗开始时，按连线好友数量分档强化攻击并创建连线特效。
/// 档位数值统一维护在 CombatConst。
/// </summary>
public static class FriendLineManager
{
    // 全场景入口：战斗开始时为所有英雄初始化连线(武将关系)
    public static void ApplyFriendLines()
    {
        var handledSides = new HashSet<int>();
        foreach (var player in GameManager.Instance.players)
        {
            if (!handledSides.Add(player.battleSide))
                continue;
            var friendIds = player.GetBattleCardList().Where(a => a != null).Select(a => a.Item1).ToList();
            foreach (var chess in WorldManager.Instance.GetUnitsMySide(player.battleSide))
            {
                if (chess == null || !chess.isHero || chess.hp <= 0)
                    continue;
                ApplyFriendLines(chess, friendIds);
            }
        }
    }

    private static void ApplyFriendLines(Chess chess, List<int> friendIds)
    {
        if (friendIds == null)
            return;

        var friendCount = 0;
        foreach (var friendId in friendIds)
        {
            // 只统计存在武将关系的在线好友
            if (ConfigManager.GetFriendLevel(chess.heroId, friendId) <= 0)
                continue;

            chess.AddFriendId(friendId);
            CreateFriendLine(chess, friendId);
            friendCount++;
        }

        chess.ApplyFriendAtkBonus(GetFriendLineAtkRate(friendCount));
        chess.RefreshHeroAttr();
    }

    // 根据连线好友数量获取攻击强化百分比，未达标返回0
    public static float GetFriendLineAtkRate(int count)
    {
        for (int i = CombatConst.FriendLineCounts.Length - 1; i >= 0; i--)
        {
            if (count >= CombatConst.FriendLineCounts[i])
                return CombatConst.FriendLineAtkRates[i];
        }
        return 0;
    }

    // 创建两个武将之间的连线特效
    private static void CreateFriendLine(Chess chess, int friendId)
    {
        var friendChess = WorldManager.Instance.FindByHeroIdAndSide(friendId, chess.side);
        if (friendChess == null)
            return;

        GameObject linePrefab = Resources.Load<GameObject>("Prefabs/LaserLine");
        GameObject lineInstance = Object.Instantiate(linePrefab, Vector3.zero, Quaternion.identity);
        lineInstance.transform.SetParent(chess.transform);
        lineInstance.transform.localScale = new Vector3(1, 1, 1);
        var beam = lineInstance.transform.Find("Beam").GetComponent<GlowBeamController>();
        beam.SetSourceAndTarget(chess, friendChess);
        beam.SetGlowColor(chess.GetPlayerInfo().lineColor);
    }
}
