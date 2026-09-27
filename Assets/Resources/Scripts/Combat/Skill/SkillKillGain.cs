using System;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 枭乱（乱世枭雄）：每击杀一个敌方单位，永久提升自身 skillCfg.Strength 点攻击，并回复 maxHp × skillCfg.Strength2 的生命。
/// 订阅 Chess.OnDamageDealt 静态事件；战斗结束/技能移除时退订（退订在 OnDeath 兜底）。
/// </summary>
public class SkillKillGain : Skill
{
    private bool subscribed;

    public SkillKillGain(int id, Chess unit) : base(id, unit)
    {
    }

    public override void BattleBegin()
    {
        if (!subscribed)
        {
            Chess.OnDamageDealt += OnDamageDealt;
            subscribed = true;
        }
    }

    private void OnDamageDealt(Chess attacker, Chess victim, int damage, int skillId)
    {
        if (!subscribed)
            return;
        if (attacker != owner || victim == null || victim.playerId == owner.playerId)
            return;
        if (victim.hp > 0)
            return;

        owner.atk += (int)skillCfg.Strength;
        var heal = (int)(owner.maxHp * skillCfg.Strength2);
        if (heal > 0)
            owner.HealTarget(owner, id, heal, false);
        GameLog.Debug($"枭乱 武将{owner.heroId} 击杀{victim.heroId}({(victim.isHero ? "英雄" : "士兵")}) 攻击+{(int)skillCfg.Strength} 回复{heal}");
    }

    public override void OnDeath()
    {
        if (subscribed)
        {
            Chess.OnDamageDealt -= OnDamageDealt;
            subscribed = false;
        }
    }
}