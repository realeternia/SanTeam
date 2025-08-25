using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

public class SkillHitBuff : Skill
{
    public SkillHitBuff(int id, Chess unit) : base(id, unit)
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

        if(CheckBurst(rate))
        {
            BuffManager.AddBuff(defender, owner, id, skillCfg.BuffId, skillCfg.BuffTime);
        }
    }

}
