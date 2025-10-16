using System;
using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

public class SkillInitAddRege : Skill
{
    public SkillInitAddRege(int id, Chess unit) : base(id, unit)
    {
    }

    public override void BattleBegin()
    {
        owner.regeHp += skillCfg.StrengthInt;
        owner.PlayerAnim(skillCfg.Action);
    }


}
