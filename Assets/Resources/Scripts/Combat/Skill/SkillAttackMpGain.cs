using System;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 儒雅（儒将风范）：攻击命中时按 skillCfg.Rate 概率为自身额外获取法力。
/// Strength2 key "mpGain" 为普攻基础法力（CombatConst.MpPerAttack）的倍率（1=1倍、0.5=0.5倍），
/// 经棋子 mpRegen 百分比加成后施加到自身所有 MpCost>0 的技能。对应技能：儒雅（2010151~2010155，「儒」连锁）。
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

        var gain = GetStr2("mpGain");
        if (gain <= 0f)
        {
            GameLog.Error($"儒雅 武将{owner.heroId} 未配置回蓝数值 key \"mpGain\"");
            return;
        }

        // mpGain 为普攻基础法力的倍率；走统一入口，乘 mpRegen 百分比加成后加到所有 MpCost>0 的技能
        owner.GainMp(CombatConst.MpPerAttack * gain);
        GameLog.Debug($"儒雅 武将{owner.heroId} 攻击额外回蓝{CombatConst.MpPerAttack * gain * owner.MpGainMultiplier:0.##}");
    }
}