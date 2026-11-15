using System;
using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

public class SkillEquipModifyShootSpeed : Skill
{
    public SkillEquipModifyShootSpeed(int id, Chess unit) : base(id, unit)
    {
    }

    public override void BattleBegin()
    {
        owner.missileSpeed = (int)(owner.missileSpeed * GetStr2("shootSpeedRate"));
    }
  
}
