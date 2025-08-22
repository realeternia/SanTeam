using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

public class SkillHardSkin : Skill
{
    public SkillHardSkin(int id, Chess unit) : base(id, unit)

    {
    }

    public override void DuringAttacked(Chess attacker, string damType, ref int damageBase, ref float damageMulti, ref string effect)
    {
        if(damageBase > 10 && CheckBurst())
        {
            //damageMulti -= skillCfg.Strength;
            BuffManager.AddBuff(owner, owner, id, skillCfg.BuffId, skillCfg.BuffTime);


        }
    }

}
