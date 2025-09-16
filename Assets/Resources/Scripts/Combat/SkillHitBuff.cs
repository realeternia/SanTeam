using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

public class SkillHitBuff : Skill
{
    public SkillHitBuff(int id, Chess unit) : base(id, unit)
    {
    }

    public override void OnAttack(Chess defender, string damType, int damage)
    {
        var rate = GetRate(defender);
        if(CheckBurst(rate))
        {
            if(!string.IsNullOrEmpty(skillCfg.Action))
                owner.PlayerAnim(skillCfg.Action);
            BuffManager.AddBuff(defender, owner, id, skillCfg.BuffId, skillCfg.BuffTime);
        }
    }

}
