using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

public class SkillHardSkin : Skill
{
    public SkillHardSkin(int id, Chess unit) : base(id, unit)

    {
    }

    public override void DuringAttack(Chess defender, ref int damageBase, ref float damageMulti, ref string effect)
    {
       damageMulti -= skillCfg.Strength;
    }

    public override void DuringAttacked(Chess attacker, ref int damageBase, ref float damageMulti, ref string effect)
    {
        damageMulti -= skillCfg.Strength;
    }

}
