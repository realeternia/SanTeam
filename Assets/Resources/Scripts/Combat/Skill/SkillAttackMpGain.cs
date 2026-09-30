using System;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 儒雅（儒将风范）：攻击命中时按 skillCfg.Rate 概率回复自身首个有蓝耗主动技能 MpCost × skillCfg.Strength2 的法力。
/// 对应技能：儒雅。
/// </summary>
public class SkillAttackMpGain : Skill
{
    public SkillAttackMpGain(int id, Chess unit) : base(id, unit)
    {
    }

    public override void OnAttack(Chess defender, int damage)
    {
        if (defender == null || defender.playerId == owner.playerId)
            return;
        if (skillCfg.Rate > 0 && SysRandom.Value >= skillCfg.Rate)
            return;

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
            return;

        var gain = active.skillCfg.MpCost * skillCfg.Strength2;
        active.mp = Mathf.Min(active.skillCfg.MpCost, active.mp + gain);
        GameLog.Debug($"儒雅 武将{owner.heroId} 攻击回蓝{gain} 至{(int)active.mp}/{active.skillCfg.MpCost}");
    }
}