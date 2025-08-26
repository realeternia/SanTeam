using System;
using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

public class SkillAtkDefRate : Skill
{
    public SkillAtkDefRate(int id, Chess unit) : base(id, unit)
    {
    }

    public override void DuringAttack(Chess defender, string damType, ref int damageBase, ref float damageMulti, ref string effect)
    {
        var rate = GetRate(defender);
        if(CheckBurst(rate))
        {
            damageMulti += skillCfg.Strength;
        }
    }

    public override void DuringAttacked(Chess attacker, string damType, ref int damageBase, ref float damageMulti, ref string effect)
    {
        var rate = GetRate(attacker);
        if(CheckBurst(rate))
        {
            damageMulti -= skillCfg.Strength;
        }
    }
}
