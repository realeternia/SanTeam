using CommonConfig;
using UnityEngine;

/// <summary>
/// 妖咒（ScriptName = "AidCurseHeal"）：辅助技能，直接治疗 Range 内生命比例最低的友方英雄（走独立治疗公式 GetSkillHeal），
/// 并给 Range 内最多 TargetCount 名敌人挂"败"（溃败·每秒法术伤害，由 BuffTimeDamage 按 DamageStrength 结算），持续 BuffTime 秒。
/// 友军均满血且范围内无敌人时不施放。
/// 用于于吉：妖道符水，以敌之血养己之众。
/// </summary>
public class SkillAidCurseHeal : Skill
{
    public SkillAidCurseHeal(int id, Chess unit) : base(id, unit)
    {
    }

    public override bool CheckAidSkill()
    {
        // 找 Range 内生命比例最低的友方英雄（可含自身）
        Chess lowest = null;
        var lowestRate = float.MaxValue;
        foreach (var a in WorldManager.Instance.GetMySideInRange(owner.transform.position, skillCfg.Range, owner.side))
        {
            if (a == null || a.hp <= 0 || !a.isHero || a.hp >= a.maxHp)
                continue;
            if (a.HpRate < lowestRate)
            {
                lowestRate = a.HpRate;
                lowest = a;
            }
        }

        // Range 内敌人（含士兵），用于挂"败"
        var enemies = WorldManager.Instance.GetEnemyInRange(owner.transform.position, skillCfg.Range, owner.side);

        // 既无可治疗的友军、又无敌人可挂"败"时不施放，避免空耗 MP
        if (lowest == null && enemies.Count == 0)
            return false;

        if (!CheckBurst(null))
            return false;

        owner.PlayerAnim(skillCfg.Action);

        // 直接治疗生命比例最低的友军英雄
        var heal = GetSkillHeal();
        if (lowest != null && heal > 0)
        {
            owner.HealTarget(lowest, skillId, heal, true);
            EffectManager.PlaySkillEffect(lowest, skillCfg.HitEffect);
        }

        // 给范围内最多 TargetCount 名敌人挂"败"（每秒法术伤害，由 BuffTimeDamage 结算）
        var buffId = BuffConfig.GetConfigByNameS(skillCfg.BuffId).Id;
        WorldManager.Instance.RandomSelect(enemies, skillCfg.TargetCount);
        foreach (var e in enemies)
        {
            BuffManager.AddBuff(e, owner, id, buffId, skillCfg.BuffTime);
            EffectManager.PlaySkillEffect(e, skillCfg.HitEffect);
        }

        GameLog.Debug($"妖咒 技能id={id} 等级={Level} 治疗={heal} 治疗对象={lowest?.heroId} 挂败目标数={enemies.Count}");
        return true;
    }
}