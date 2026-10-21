using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 连线(武将关系)初始化：战斗开始时，按 HeroFriendConfig 关联技能为在场好友组授予特殊连锁技能并创建连线特效。
/// 每组在场好友每多一个，关联技能等级 +1（默认0级）；加成由技能在 BattleBegin 施加。
/// </summary>
public static class FriendLineManager
{
    // 创建两个武将之间的连线特效
    private static void CreateFriendLine(Chess chess, int friendId, Color lineColor)
    {
        var friendChess = WorldManager.Instance.FindByHeroIdAndSide(friendId, chess.side);
        if (friendChess == null)
            return;

        GameObject linePrefab = Resources.Load<GameObject>("Prefabs/Battles/FriendLink");
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
        var owner = chess.GetPlayerInfo();
        // 真实所属组 + 装备的代友道具指代组（代友道具只参与人数与拉线，不授予该组技能）
        var relIds = CollectFriendRelationIds(owner, chess.heroId);
        if (relIds.Count == 0)
            return;

        // 本侧在场英雄：代友道具按装备者统计，每个道具实例=该羁绊组额外在场1人
        var sideHeroes = WorldManager.Instance.GetUnitsMySide(chess.side);

        foreach (var relId in relIds)
        {
            var relCfg = HeroFriendConfig.GetConfig(relId);
            if (relCfg == null || string.IsNullOrEmpty(relCfg.SkillId))
                continue;

            // 统计该关系组内、本侧在场的其他真实成员数量（每多一个好友 +1 级）
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
            // 代友道具：装备者视为该组在场1人（只提升人数/等级，自身不获得该组技能）
            var proxyHeroIds = GetProxyCarrierHeroIds(sideHeroes, relCfg.SkillId);
            var proxyCount = 0;
            foreach (var proxyHeroId in proxyHeroIds)
            {
                if (proxyHeroId != chess.heroId)
                    proxyCount++;
            }
            if (presentCount + proxyCount <= 0)
                continue;

            // 只有该组真实成员才获得关联技能，等级 = 在场好友数（含代友道具额外人数）
            if (ConfigManager.HeroHasFriendSkill(chess.heroId, relCfg.SkillId))
            {
                var skillLevel = CombatConst.FriendSpecialBaseLevel + presentCount + proxyCount;
                GrantFriendSpecialSkill(chess, relCfg.SkillId, skillLevel);

                GameLog.Debug($"FriendSpecial 关系{relCfg.Name} 武将{chess.heroId} 好友数{presentCount} 代友{proxyCount} 技能{relCfg.SkillId} 等级{skillLevel}");
            }

            // 特殊连锁仍然拉线，线颜色取 HeroFriendConfig.LineColor（未配置默认暗灰）
            // 真实成员与代友装备者都参与连线；好友对 A-B 会从双方各遍历到一次，只由 heroId 较小的一方拉线，避免重复光柱
            var lineColor = ParseLineColor(relCfg.LineColor, SysColor.FriendLine.DefaultLine);
            foreach (var memberId in presentMembers)
            {
                if (chess.heroId > memberId)
                    continue;
                CreateFriendLine(chess, memberId, lineColor);
            }
            foreach (var proxyHeroId in proxyHeroIds)
            {
                if (proxyHeroId == chess.heroId || chess.heroId > proxyHeroId)
                    continue;
                CreateFriendLine(chess, proxyHeroId, lineColor);
            }
        }
    }

    /// <summary>
    /// 英雄参与的好友羁绊组：真实所属组 + 装备的代友道具指代组（供人数统计与连线共用）
    /// </summary>
    public static HashSet<int> CollectFriendRelationIds(PlayerInfo owner, int heroId)
    {
        var ids = new HashSet<int>();
        var realIds = ConfigManager.GetHeroFriendInfo(heroId);
        if (realIds != null)
        {
            foreach (var relId in realIds)
                ids.Add(relId);
        }
        if (owner == null)
            return ids;

        foreach (var itemId in owner.GetItemIdsOnHero(heroId))
        {
            if (!ItemConfig.HasConfig(itemId))
                continue;
            var itemCfg = ItemConfig.GetConfig(itemId);
            if (itemCfg == null || itemCfg.Effect != CombatConst.FriendProxyItemEffect || string.IsNullOrEmpty(itemCfg.RefSkillSname))
                continue;
            foreach (var relCfg in HeroFriendConfig.ConfigList)
            {
                if (relCfg.SkillId == itemCfg.RefSkillSname)
                {
                    ids.Add(relCfg.Id);
                    break;
                }
            }
        }
        return ids;
    }

    // 本侧在场英雄中，装备了指代指定好友羁绊组代友道具的英雄id（同一英雄同组只算一次）
    private static List<int> GetProxyCarrierHeroIds(List<Chess> sideUnits, string friendSkillSname)
    {
        var heroIds = new List<int>();
        if (string.IsNullOrEmpty(friendSkillSname) || sideUnits == null)
            return heroIds;

        foreach (var unit in sideUnits)
        {
            if (unit == null || !unit.isHero)
                continue;
            if (CountProxyItemsOnHero(unit.GetPlayerInfo(), unit.heroId, friendSkillSname) > 0 && !heroIds.Contains(unit.heroId))
                heroIds.Add(unit.heroId);
        }
        return heroIds;
    }

    /// <summary>
    /// 统计玩家指定英雄上装备的代友道具数量（指代指定好友羁绊技能），供界面展示羁绊人数使用
    /// </summary>
    public static int CountProxyItems(PlayerInfo player, IList<int> heroIds, string friendSkillSname)
    {
        if (player == null || heroIds == null || string.IsNullOrEmpty(friendSkillSname))
            return 0;

        var count = 0;
        foreach (var heroId in heroIds)
            count += CountProxyItemsOnHero(player, heroId, friendSkillSname);
        return count;
    }

    private static int CountProxyItemsOnHero(PlayerInfo owner, int heroId, string friendSkillSname)
    {
        if (owner == null)
            return 0;

        var count = 0;
        foreach (var itemId in owner.GetItemIdsOnHero(heroId))
        {
            if (!ItemConfig.HasConfig(itemId))
                continue;
            var itemCfg = ItemConfig.GetConfig(itemId);
            if (itemCfg != null && itemCfg.Effect == CombatConst.FriendProxyItemEffect
                && itemCfg.RefSkillSname == friendSkillSname)
                count++;
        }
        return count;
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