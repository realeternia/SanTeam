using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

public class SkillMultiArrow : Skill
{
    public SkillMultiArrow(int id, Chess unit) : base(id, unit)
    {
    }

    public override void AimTarget(Chess defender)
    {
        var unitsInRange = WorldManager.Instance.GetUnitsInRange(owner.transform.position, skillCfg.Range, owner.side, true);
        // 过滤掉当前目标defender
        var filteredUnits = new List<Chess>();
        foreach (var unit in unitsInRange)
        {
            if (unit != defender)
            {
                filteredUnits.Add(unit);
            }
        }

        if (filteredUnits.Count > 0 && CheckBurst())
        {
            int randomIndex = Random.Range(0, filteredUnits.Count);
            Chess randomTarget = filteredUnits[randomIndex];

            WorldManager.Instance.CreateMissile(owner, randomTarget, owner.hitEffect);
        }
    }
}
