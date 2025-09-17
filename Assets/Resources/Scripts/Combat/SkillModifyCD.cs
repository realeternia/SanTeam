using System;
using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

public class SkillModifyCD : Skill
{
    public SkillModifyCD(int id, Chess unit) : base(id, unit)
    {
    }

    public override void OnCheckCD(SkillConfig checkSkillCfg, ref float cdTime)
    {
        if(checkSkillCfg.Attr != skillCfg.Attr)
            return;
        
        cdTime = Math.Max(1, cdTime * skillCfg.Strength);
    }
}
