using System;
using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 护卫 · 减伤盾：辅助技能，给生命比例最低的我方英雄施加数值型吸收盾(护盾/盾)。
/// 护盾容量 = 施法者最大生命 × Strength；CD 8s；
/// 若在场友方英雄生命均已满则不施放。通过 Skill.CheckAidSkill 由 SkillManager 自动循环施放。
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

        var units = WorldManager.Instance.GetUnitsInRange(owner.transform.position, skillCfg.Range, owner.side, false)
            .FindAll(x => x != owner && x.isHero && x.IsInFight());

        if (units.Count == 0)
            return false;

        // 找生命比例最低的我方英雄；生命已满的不参与，全部满血则不施放
        Chess target = null;
        float lowest = float.MaxValue;
        foreach (var u in units)
        {
            if (u.hp >= u.maxHp)
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

        // 数值型吸收盾：容量改按施法者AP × 比例计算
        var shieldId = BuffConfig.GetConfigByNameS(skillCfg.BuffId).Id;
        BuffManager.AddBuff(target, owner, id, shieldId, skillCfg.BuffTime);
        var shield = target.GetBuff(shieldId) as BuffShield;
        if (shield != null)
            shield.SetHp((int)(owner.maxHp * skillCfg.Strength));

        EffectManager.PlaySkillEffect(target, skillCfg.HitEffect);
        return true;
    }

}