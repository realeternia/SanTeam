using System.Collections.Generic;
using System.Linq;
using CommonConfig;
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
            // 好友连锁·特殊（配置了关联技能）不再加属性，由 ApplyFriendSpecialSkills 单独处理
            if (!string.IsNullOrEmpty(ConfigManager.GetFriendSkillId(chess.heroId, friendId)))
                continue;

            chess.AddFriendId(friendId);
            // 线颜色取该武将对所在关系行配置的 LineColor（未配置默认暗灰）
            var lineColor = ParseLineColor(ConfigManager.GetFriendLineColor(chess.heroId, friendId), SysColor.FriendLine.DefaultLine);
            CreateFriendLine(chess, friendId, lineColor);
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
    private static void CreateFriendLine(Chess chess, int friendId, Color lineColor)
    {
        var friendChess = WorldManager.Instance.FindByHeroIdAndSide(friendId, chess.side);
        if (friendChess == null)
            return;

        GameObject linePrefab = Resources.Load<GameObject>("Prefabs/Battles/LaserLine");
        GameObject lineInstance = Object.Instantiate(linePrefab, Vector3.zero, Quaternion.identity);
        lineInstance.transform.SetParent(chess.transform);
        lineInstance.transform.localScale = new Vector3(1, 1, 1);
        var beam = lineInstance.transform.Find("Beam").GetComponent<GlowBeamController>();
        beam.SetSourceAndTarget(chess, friendChess);
        beam.SetGlowColor(lineColor);
    }

    /// <summary>
    /// 好友连锁·特殊：HeroFriendConfig 配置了关联技能(SkillId>0)的关系称为特殊连锁。
    /// 规则：该关系组在场成员每多一个，关联技能等级 +1（默认0级=无技能）；
    /// 特殊连锁不加攻击属性，但仍拉线，线颜色取 HeroFriendConfig.LineColor。
    /// </summary>
    public static void ApplyFriendSpecialSkills()
    {
        var handledSides = new HashSet<int>();
        foreach (var player in GameManager.Instance.players)
        {
            if (!handledSides.Add(player.battleSide))
                continue;
            foreach (var chess in WorldManager.Instance.GetUnitsMySide(player.battleSide))
            {
                if (chess == null || !chess.isHero || chess.hp <= 0)
                    continue;
                ApplyFriendSpecialSkills(chess);
            }
        }
    }

    private static void ApplyFriendSpecialSkills(Chess chess)
    {
        var relIds = ConfigManager.GetHeroFriendInfo(chess.heroId);
        if (relIds == null)
            return;

        foreach (var relId in relIds)
        {
            var relCfg = HeroFriendConfig.GetConfig(relId);
            if (relCfg == null || string.IsNullOrEmpty(relCfg.SkillId))
                continue;

            // 统计该关系组内、本侧在场的其他成员数量（每多一个好友 +1 级）
            var presentCount = 0;
            var presentMembers = new List<int>();
            foreach (var memberId in relCfg.Heros)
            {
                if (memberId == chess.heroId)
                    continue;
                if (WorldManager.Instance.FindByHeroIdAndSide(memberId, chess.side) != null)
                {
                    presentCount++;
                    presentMembers.Add(memberId);
                }
            }
            if (presentCount <= 0)
                continue;

            // 激活特殊关联技能，等级 = 在场好友数
            var skillLevel = CombatConst.FriendSpecialBaseLevel + presentCount;
            GrantFriendSpecialSkill(chess, relCfg.SkillId, skillLevel);

            // 特殊连锁仍然拉线，线颜色取 HeroFriendConfig.LineColor（未配置默认暗灰）
            var lineColor = ParseLineColor(relCfg.LineColor, SysColor.FriendLine.DefaultLine);
            foreach (var memberId in presentMembers)
                CreateFriendLine(chess, memberId, lineColor);

            GameLog.Debug($"FriendSpecial 关系{relCfg.Name} 武将{chess.heroId} 好友数{presentCount} 技能{relCfg.SkillId} 等级{skillLevel}");
        }
    }

    // 将特殊关联技能赋予英雄并按等级激活（技能需已配置在SkillConfig，按缩写引用）
    private static void GrantFriendSpecialSkill(Chess chess, string skillSname, int level)
    {
        var skillCfg = ConfigManager.GetSkillConfig(skillSname);
        if (skillCfg == null)
        {
            GameLog.Warn($"FriendSpecial 技能{skillSname}未在SkillConfig中配置，暂不激活");
            return;
        }
        var skillId = skillCfg.Id;
        if (chess.skills.Find(s => s.skillCfg.Sname == skillSname) == null)
            chess.AddSkill(skillId, 0);
        var skill = chess.skills.Find(s => s.skillCfg.Sname == skillSname);
        if (skill != null)
            skill.SetLevel(level);
    }

    // 解析HeroFriendConfig中的连线颜色（HTML色值），未配置时使用默认暗灰线色
    private static Color ParseLineColor(string colorStr, Color defaultColor)
    {
        if (string.IsNullOrEmpty(colorStr))
            return defaultColor;
        Color color;
        return ColorUtility.TryParseHtmlString(colorStr, out color) ? color : defaultColor;
    }
}
