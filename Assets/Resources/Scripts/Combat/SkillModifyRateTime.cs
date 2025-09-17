using System;
using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

public class SkillModifyRateTime : Skill
{
    public SkillModifyRateTime(int id, Chess unit) : base(id, unit)
    {
    }

    public override void OnCheckBurst(SkillConfig checkSkillCfg, ref float rate)
    {
        if(checkSkillCfg.Attr != skillCfg.Attr)
            return;
        rate += Math.Min(rate, checkSkillCfg.Rate);
    }

    public override void OnAddBuff(BuffConfig buffCfg, ref float time)
    {
        if(!buffCfg.IsPositive)
            time += Math.Max(1, time / 2) * skillCfg.BuffTime;
    }
}
