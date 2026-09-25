using System;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 离间（貂蝉）：辅助技能，给生命比例最低的我方英雄附加吸血+攻速组合 buff。
/// 通过 Skill.CheckAidSkill 由 SkillManager 自动循环施放。
/// </summary>
public class SkillAidBuffHasteSteal : Skill
{
    public SkillAidBuffHasteSteal(int id, Chess unit) : base(id, unit)
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

        BuffManager.AddBuff(target, owner, id, BuffConfig.GetConfigByNameS(skillCfg.BuffId).Id, skillCfg.BuffTime);
        EffectManager.PlaySkillEffect(target, skillCfg.HitEffect);
        return true;
    }
}