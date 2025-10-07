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
        var unitsInRange = WorldManager.Instance.GetUnitsInRange(defender.transform.position, skillCfg.Range, owner.side, true);
        unitsInRange.Remove(defender);

        if (unitsInRange.Count > 0 && CheckBurst(defender))
        {
            WorldManager.Instance.RandomSelect(unitsInRange, skillCfg.TargetCount);
            foreach (var unit in unitsInRange)
                WorldManager.Instance.CreateAttackMissile(owner, unit, owner.hitEffect);
        }
    }
}
