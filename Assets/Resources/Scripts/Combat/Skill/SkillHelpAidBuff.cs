using System;
using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

public class SkillHelpAidBuff : Skill
{
    public SkillHelpAidBuff(int id, Chess unit) : base(id, unit)
    {
    }

    public override bool CheckAidSkill()
    {
        var unitsInRange = WorldManager.Instance.GetUnitsInRange(owner.transform.position, skillCfg.Range, owner.side, false);
        unitsInRange = unitsInRange.FindAll(x => x != owner && x.IsInFight() && !x.HasBuff(skillCfg.BuffId));

        if (unitsInRange.Count == 0)
            return false;

        if (!CheckBurst(null))
            return false;

        owner.PlayerAnim(skillCfg.Action);

        //排序，优先给hero，然后优先给生命值低的
        unitsInRange.Sort((a, b) =>
        {
            if (a.isHero && !b.isHero)
                return -1;
            if (b.isHero && !a.isHero)
                return 1;
            return b.GetAttrTotal().CompareTo(a.GetAttrTotal());
        });

        var targetUnit = unitsInRange[0];
        BuffManager.AddBuff(targetUnit, owner, id, skillCfg.BuffId, skillCfg.BuffTime);
        EffectManager.PlaySkillEffect(targetUnit, skillCfg.HitEffect);

        return true;
    }
}
