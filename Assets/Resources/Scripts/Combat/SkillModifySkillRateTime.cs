using System;
using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

public class SkillModifySkillRateTime : Skill
{
    public SkillModifySkillRateTime(int id, Chess unit) : base(id, unit)
    {
    }

    public override void OnCheckBurst(SkillConfig checkSkillCfg, ref float rate)
    {
        if(checkSkillCfg.Rate == 0)
            return;
        if(checkSkillCfg.Attr != skillCfg.Attr)
            return;
        rate += Math.Min(rate, checkSkillCfg.Rate);
    }

    public override void OnAddBuff(Chess target, BuffConfig buffCfg, int checkSkillId, ref float time)
    {
        if(skillCfg.BuffTime == 0)
            return;
        if(SkillConfig.GetConfig(checkSkillId).Attr != skillCfg.Attr)
            return;
        if(!buffCfg.IsPositive)
            time += Math.Max(1, time / 2) * skillCfg.BuffTime;
    }
    
    public override void OnCheckCD(SkillConfig checkSkillCfg, ref float cdTime)
    {
        if(skillCfg.Strength == 0)
            return;
        if(checkSkillCfg.Attr != skillCfg.Attr)
            return;
        
        cdTime = Math.Max(1, cdTime * skillCfg.Strength);
    }    
}
