using System;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 异禀（身负异禀）：战斗开始时，若自身未装备任何道具，则永久提升攻击与法强 skillCfg.Strength 点、护甲与魔抗 skillCfg.Strength2 点。
/// 直改 Chess 字段，不走 buff 不还原（参照制衡/巧工）。对应技能：异禀。
/// </summary>
public class SkillInitNoEquipAttr : Skill
{
    public SkillInitNoEquipAttr(int id, Chess unit) : base(id, unit)
    {
    }

    public override void BattleBegin()
    {
        var player = owner.GetPlayerInfo();
        if (player == null)
        {
            GameLog.Error($"异禀 武将{owner.heroId} 获取玩家信息失败，无法判定装备");
            return;
        }
        var equipIds = player.GetItemIdsOnHero(owner.heroId);
        if (equipIds.Count > 0)
            return;

        owner.atk += (int)skillCfg.Strength;
        owner.ap += (int)skillCfg.Strength;
        owner.armor += (int)skillCfg.Strength2;
        owner.magicRes += (int)skillCfg.Strength2;
        GameLog.Debug($"异禀 武将{owner.heroId} 无装备 永久提升攻击法强+{(int)skillCfg.Strength} 双防+{(int)skillCfg.Strength2}");
    }
}