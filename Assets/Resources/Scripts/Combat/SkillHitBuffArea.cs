using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

public class SkillHitBuffArea : Skill
{
    public SkillHitBuffArea(int id, Chess unit) : base(id, unit)
    {
    }

    public override void OnAttack(Chess defender, int damage)
    {
        var myAttr = owner.GetAttr(skillCfg.Attr);
        var defAttr = defender.GetAttr(skillCfg.Attr);
        var rate = (myAttr - defAttr) * 0.01f;

        var unitsInRange = WorldManager.Instance.GetUnitsInRange(defender.transform.position, skillCfg.Range, owner.side, true);
        unitsInRange.Remove(defender);
        if(unitsInRange.Count > 0 && CheckBurst(rate))
        {
            BuffManager.AddBuff(defender, owner, id, skillCfg.BuffId, skillCfg.BuffTime);
            WorldManager.Instance.RandomSelect(unitsInRange, skillCfg.TargetCount);

            foreach(var unit in unitsInRange)
            {
                BuffManager.AddBuff(unit, owner, id, skillCfg.BuffId, skillCfg.BuffTime);
            }
        }
    }

}
