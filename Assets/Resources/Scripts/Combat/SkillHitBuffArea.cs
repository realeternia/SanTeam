using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

public class SkillHitBuffArea : Skill
{
    public SkillHitBuffArea(int id, Chess unit) : base(id, unit)
    {
    }

    public override void OnAttack(Chess defender, int damage)
    {
        var myAttr = owner.GetAttr(skillCfg.Attr);
        var defAttr = defender.GetAttr(skillCfg.Attr);
        var rate = (myAttr - defAttr) * skillCfg.RateAttrHP;

        if(rate > 0)
            rate = skillCfg.RateAttrH + rate;
        else
            rate = skillCfg.Rate;

        var unitsInRange = WorldManager.Instance.GetUnitsInRange(defender.transform.position, skillCfg.Range, owner.side, true);
        unitsInRange.Remove(defender);
        if(CheckBurst(rate))
        {
            BuffManager.AddBuff(defender, owner, id, skillCfg.BuffId, skillCfg.BuffTime);

            if (unitsInRange.Count > 0)
            {
                WorldManager.Instance.RandomSelect(unitsInRange, skillCfg.TargetCount);

                foreach (var unit in unitsInRange)
                    BuffManager.AddBuff(unit, owner, id, skillCfg.BuffId, skillCfg.BuffTime);
            }
        }
    }

}
