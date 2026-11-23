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
public class SkillBondBuffLowHp : Skill
{
    public SkillBondBuffLowHp(int id, Chess unit) : base(id, unit)
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
            GameLog.Error("SkillBondBuffLowHp: BuffId 与 HealStrength 均为空，技能无法生效 id=" + skillCfg.Id);
            return false;
        }

        // 选人规则见 SelectOnly / SelectFirst（典型 ally;hero;notSelf;inFight + lowestHp）；
        // 纯护盾类跳过满血目标（全满血不施放），带治疗的守护允许满血目标吃护甲等增益
        var picked = SelectTargets(owner.transform.position, skillCfg.Area, true, 1, null,
            heal <= 0 ? (Func<Chess, bool>)(x => x.hp < x.maxHp) : null);
        var target = picked.Count > 0 ? picked[0] : null;
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
            PlaySkillEffect(target, skillCfg.HitEffect);
        return true;
    }

}
