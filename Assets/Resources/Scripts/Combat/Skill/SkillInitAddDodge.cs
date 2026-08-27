using System;
using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

public class SkillInitAddDodge : Skill
{
    public SkillInitAddDodge(int id, Chess unit) : base(id, unit)
    {
    }

    public override void BattleBegin()
    {
        owner.dodgeRate += skillCfg.Strength;
        owner.PlayerAnim(skillCfg.Action);
    }


}
