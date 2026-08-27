using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

public class SkillAttackReboundArrow : Skill
{
    public SkillAttackReboundArrow(int id, Chess unit) : base(id, unit)
    {
    }

    public override void OnAttack(Chess defender, string damType, int damage)
    {
        var unitsInRange = WorldManager.Instance.GetUnitsInRange(defender.transform.position, skillCfg.Range, owner.side, true);
        unitsInRange.Remove(defender);

        if (unitsInRange.Count > 0 && CheckBurst(defender))
        {
            owner.PlayerAnim(skillCfg.Action);
            WorldManager.Instance.RandomSelect(unitsInRange, skillCfg.TargetCount);

            var reboundDamage = (int)(damage * skillCfg.SkillDamageRate);
            foreach (var unit in unitsInRange)
                WorldManager.Instance.CreateSpellMissile(owner, unit, defender.transform.position, id, reboundDamage, owner.hitEffect);
        }
    }
}
