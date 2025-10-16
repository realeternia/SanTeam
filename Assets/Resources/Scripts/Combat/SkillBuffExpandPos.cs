using System;
using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;
using System.Linq;

public class SkillBuffExpandPos : Skill
{
    public SkillBuffExpandPos(int id, Chess unit) : base(id, unit)
    {
    }

    public override void OnAddBuff(Chess target, ref int buffId, int checkSkillId, ref float time)
    {
        UnityEngine.Debug.Log($"SkillBuffExpandPos OnAddBuff {target.name} {buffId} {checkSkillId} {time}");
        if(checkSkillId == skillId)
            return; //自己挂的buff，不再连续触发

        if (skillCfg.CheckAttrs != null && !skillCfg.CheckAttrs.Contains(SkillConfig.GetConfig(checkSkillId).Attr))
            return;            

        var buffCfg = BuffConfig.GetConfig(buffId); 
        if(!buffCfg.IsPositive)
            return;
        
        if (CheckBurst(target))
        {
            var unitsInRange = WorldManager.Instance.GetUnitsInRange(target.transform.position, skillCfg.Range, owner.side, false);
            if (unitsInRange.Count > 0)
            {
                WorldManager.Instance.RandomSelect(unitsInRange, skillCfg.TargetCount);

                foreach (var unit in unitsInRange)
                    BuffManager.AddBuff(unit, owner, checkSkillId, buffId, time);
            }
        }
    }

    public override void OnHealTarget(Chess target, int checkSkillId, ref int addon)
    {
        if(checkSkillId == skillId)
            return; //自己挂的buff，不再连续触发

        if (skillCfg.CheckAttrs != null && !skillCfg.CheckAttrs.Contains(SkillConfig.GetConfig(checkSkillId).Attr))
            return;            
       
        if (CheckBurst(target))
        {
            var unitsInRange = WorldManager.Instance.GetUnitsInRange(target.transform.position, skillCfg.Range, owner.side, false);
            unitsInRange.RemoveAll(x => x.HpRate > 0.9f);
            if (unitsInRange.Count > 0)
            {
                owner.PlayerAnim(skillCfg.Action);
                WorldManager.Instance.RandomSelect(unitsInRange, skillCfg.TargetCount);

                foreach (var unit in unitsInRange)
                    owner.HealTarget(unit, checkSkillId, addon);
            }
        }
    }
  
}
