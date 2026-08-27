using System;
using CommonConfig;

public class SkillModifySummonTime : Skill
{
    public SkillModifySummonTime(int id, Chess unit) : base(id, unit)
    {
    }

    public override void OnCheckSummonTime(SkillConfig checkSkillCfg, ref float summonTime)
    {
        if (checkSkillCfg.SummonTime <= 0)
            return;
        
        if(checkSkillCfg.SummonTag != skillCfg.SummonTag)
            return;
        
        summonTime += skillCfg.SummonTime;
    }

}
