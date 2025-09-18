using System;
using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

public class SkillModifyBuffExpand : Skill
{
    public SkillModifyBuffExpand(int id, Chess unit) : base(id, unit)
    {
    }

    public override void OnAddBuff(Chess target, BuffConfig buffCfg, int checkSkillId, ref float time)
    {
        UnityEngine.Debug.Log($"SkillModifyBuffExpand OnAddBuff {target.name} {buffCfg.Name} {checkSkillId} {time}");
        if(checkSkillId == skillId)
            return; //自己挂的buff，不再连续触发

        if(SkillConfig.GetConfig(checkSkillId).Attr != skillCfg.Attr)
            return;
        if(buffCfg.IsPositive)
            return;
        
        var rate = GetRate(target);
        if (CheckBurst(rate))
        {
            var unitsInRange = WorldManager.Instance.GetUnitsInRange(target.transform.position, skillCfg.Range, owner.side, true);
            if (unitsInRange.Count > 0)
            {
                WorldManager.Instance.RandomSelect(unitsInRange, skillCfg.TargetCount);

                foreach (var unit in unitsInRange)
                    BuffManager.AddBuff(unit, owner, id, buffCfg.Id, time);
            }
        }
    }
  
}
