using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

public class SkillAttackedBuff : Skill
{
    public SkillAttackedBuff(int id, Chess unit) : base(id, unit)
    {
    }

    public override void OnAttacked(Chess attacker, string damType, int damage)
    {
        if(damage > 10 && CheckBurst(attacker))
        {
            owner.PlayerAnim(skillCfg.Action);

            BuffManager.AddBuff(owner, owner, id, skillCfg.BuffId, skillCfg.BuffTime);
        }
    }

}
