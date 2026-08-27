using System;
using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

public class SkillAttackAntiShield : Skill
{
    public SkillAttackAntiShield(int id, Chess unit) : base(id, unit)
    {
    }

    public override void DuringAttack(Chess defender, string damType, ref int damageBase, ref float damageMulti, ref int damageReal, ref string effect)
    {
        var buff = defender.GetBuff(skillCfg.BuffId);
        if (buff != null)
        {
            var shield = buff as BuffShield;
            if (shield != null)
            {
                owner.PlayerAnim(skillCfg.Action);
                shield.SubHp((int)(damageBase * skillCfg.Strength));
            }
        }
    }
}
