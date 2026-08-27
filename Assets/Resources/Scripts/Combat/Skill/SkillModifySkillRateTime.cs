using System;
using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;
using System.Linq;

public class SkillModifySkillRateTime : Skill
{
    public SkillModifySkillRateTime(int id, Chess unit) : base(id, unit)
    {
    }

    public override void OnCheckBurst(SkillConfig checkSkillCfg, ref float rate)
    {
        if(checkSkillCfg.Rate == 0)
            return;
        if (skillCfg.CheckAttrs != null && !skillCfg.CheckAttrs.Contains(checkSkillCfg.Attr))
            return;                 
        rate += Math.Min(rate, checkSkillCfg.Rate);
    }

    public override void OnAddBuff(Chess target, ref int buffId, int checkSkillId, ref float time)
    {
        if(skillCfg.BuffTime == 0)
            return;
        if (skillCfg.CheckAttrs != null && !skillCfg.CheckAttrs.Contains(SkillConfig.GetConfig(checkSkillId).Attr))
            return;              
        var buffCfg = BuffConfig.GetConfig(buffId);
        if(!buffCfg.IsPositive)
            time += Math.Max(1, time / 2) * skillCfg.BuffTime;
    }
    
    public override void OnCheckCD(SkillConfig checkSkillCfg, ref float cdTime)
    {
        if(skillCfg.Strength == 0)
            return;
        if (skillCfg.CheckAttrs != null && !skillCfg.CheckAttrs.Contains(checkSkillCfg.Attr))
            return; 
        
        UnityEngine.Debug.Log(owner.id + " OnCheckCD " + cdTime + " skillId " + skillId);
        cdTime = Math.Max(1, cdTime * skillCfg.Strength);
    }    
}
