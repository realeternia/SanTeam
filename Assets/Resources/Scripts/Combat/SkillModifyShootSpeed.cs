using System;
using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

public class SkillModifyShootSpeed : Skill
{
    public SkillModifyShootSpeed(int id, Chess unit) : base(id, unit)
    {
    }

    public override void BattleBegin()
    {
        owner.missileSpeed = (int)(owner.missileSpeed * skillCfg.Strength);
    }
  
}
