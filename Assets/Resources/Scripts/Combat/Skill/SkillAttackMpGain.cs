using System;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 儒雅（儒将风范）：攻击命中时按 skillCfg.Rate 概率为自身回复 skillCfg.Strength2[0] 点法力（配置为1，即每次回1点mp）。
/// 回复对象为首个有蓝耗的主动技能。对应技能：儒雅（2010151~2010155，「儒」连锁）。
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

        if (skillCfg.Strength2 == null || skillCfg.Strength2.Length == 0)
        {
            GameLog.Error($"儒雅 武将{owner.heroId} 未配置回蓝数值Strength2");
            return;
        }
        var gain = skillCfg.Strength2[0];

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

        active.mp = Mathf.Min(active.skillCfg.MpCost, active.mp + gain);
        GameLog.Debug($"儒雅 武将{owner.heroId} 攻击回蓝{gain} 至{(int)active.mp}/{active.skillCfg.MpCost}");
    }
}