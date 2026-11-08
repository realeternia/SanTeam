using System;
using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 护卫 / 守护：辅助技能，给生命比例最低的我方英雄施加防护。
/// - 支持 BuffId 多段（如 "盾,甲"）：每段按 BuffConfig 短名依次施加，护盾类（盾）容量 = key "shield" × (100+法强)/100；
/// - HealStrength &gt; 0 时先对目标治疗一次（走 GetSkillHeal 公式，随法强成长）；
/// - 仅配置护盾/无治疗时，跳过满血目标且全满血不施放；带治疗时满血目标也可获得护甲等增益。
/// 通过 Skill.CheckAidSkill 由 SkillManager 自动循环施放。
/// </summary>
public class SkillAidBuffLowHp : Skill
{
    public SkillAidBuffLowHp(int id, Chess unit) : base(id, unit)
    {
    }

    public override bool CheckAidSkill()
    {
        if (!CheckBurst(null))
            return false;

        var buffIds = GetSkillBuffIds();
        var heal = GetSkillHeal();
        if (buffIds.Count == 0 && heal <= 0)
        {
            GameLog.Error("SkillAidBuffLowHp: BuffId 与 HealStrength 均为空，技能无法生效 id=" + skillCfg.Id);
            return false;
        }

        var units = WorldManager.Instance.GetMySideInRange(owner.transform.position, skillCfg.Area, owner.side)
            .FindAll(x => x != owner && x.isHero && x.IsInFight());

        if (units.Count == 0)
            return false;

        // 找生命比例最低的我方英雄；纯护盾类跳过满血目标（全满血不施放），带治疗的守护允许满血目标吃护甲等增益
        Chess target = null;
        float lowest = float.MaxValue;
        foreach (var u in units)
        {
            if (heal <= 0 && u.hp >= u.maxHp)
                continue;
            var rate = u.hp / (float)u.maxHp;
            if (rate < lowest)
            {
                lowest = rate;
                target = u;
            }
        }
        if (target == null)
            return false;

        owner.PlayerAnim(skillCfg.Action);
        PlaySelfEffect();

        if (heal > 0)
            owner.HealTarget(target, id, heal, true);

        // 多段 Buff 依次施加；护盾段容量 = key "shield" × (100+法强)/100（随法强成长）
        var shieldValue = GetSkillShield();
        foreach (var buffId in buffIds)
        {
            BuffManager.AddBuff(target, owner, id, buffId, skillCfg.BuffTime);
            var shield = target.GetBuff(buffId) as BuffShield;
            if (shield != null)
                shield.AddHp(shieldValue);
        }

        if (!string.IsNullOrEmpty(skillCfg.HitEffect))
            EffectManager.PlaySkillEffect(target, skillCfg.HitEffect);
        return true;
    }

}