using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

public class SkillHitBuffArea : Skill
{
    public SkillHitBuffArea(int id, Chess unit) : base(id, unit)
    {
    }

    public override void OnAttack(Chess defender, string damType, int damage)
    {
        if (CheckBurst(defender))
        {
            var targetUnit = skillCfg.TargetType == "targetUnit" ? defender : owner;
            EffectManager.PlaySkillEffect(targetUnit, skillCfg.HitEffect);

            var unitsInRange = WorldManager.Instance.GetUnitsInRange(targetUnit.transform.position, skillCfg.Range, owner.side, true);
            if (unitsInRange.Count > 0)
            {
                owner.PlayerAnim(skillCfg.Action);
                WorldManager.Instance.RandomSelect(unitsInRange, skillCfg.TargetCount);

                foreach (var unit in unitsInRange)
                    BuffManager.AddBuff(unit, owner, id, skillCfg.BuffId, skillCfg.BuffTime);
            }
        }
    }

}
