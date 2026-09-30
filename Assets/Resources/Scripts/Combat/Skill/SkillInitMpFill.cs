using System;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 运筹（王佐之才）：战斗开始立即为自身首个有蓝耗的主动技能充能 mp += MpCost × skillCfg.Strength2，抢占先机。
/// 对应技能：运筹。
/// </summary>
public class SkillInitMpFill : Skill
{
    public SkillInitMpFill(int id, Chess unit) : base(id, unit)
    {
    }

    public override void BattleBegin()
    {
        Skill active = null;
        foreach (var s in owner.skills)
        {
            if (s.skillCfg.MpCost > 0)
            {
                active = s;
                break;
            }
        }
        if (active == null)
        {
            GameLog.Warn($"运筹 武将{owner.heroId} 未找到有蓝耗的主动技能，无法充能");
            return;
        }
        active.mp = Mathf.Min(active.skillCfg.MpCost, active.mp + active.skillCfg.MpCost * skillCfg.Strength2);
        GameLog.Debug($"运筹 武将{owner.heroId} 充能{(int)(active.skillCfg.MpCost * skillCfg.Strength2)} 至{(int)active.mp}/{active.skillCfg.MpCost}");
    }
}