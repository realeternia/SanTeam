using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

public class SkillBondHitBuff : Skill
{
    public SkillBondHitBuff(int id, Chess unit) : base(id, unit)
    {
    }

    public override void OnAttack(Chess defender, int damage)
    {
        if(CheckBurst(defender))
        {
            owner.PlayerAnim(skillCfg.Action);
            BuffManager.AddBuff(defender, owner, id, BuffConfig.GetConfigByNameS(skillCfg.BuffId).Id, skillCfg.BuffTime);
        }
    }

}
