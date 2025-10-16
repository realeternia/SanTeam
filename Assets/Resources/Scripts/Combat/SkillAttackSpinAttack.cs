using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

public class SkillAttackSpinAttack : Skill
{
    public SkillAttackSpinAttack(int id, Chess unit) : base(id, unit)
    {
    }

    public override void OnAttack(Chess defender, string damType, int damage)
    {
        if(CheckBurst(defender))
        {
            owner.PlayerAnim(skillCfg.Action);
            var unitsInRange = WorldManager.Instance.GetUnitsInRange(owner.transform.position, skillCfg.Range, owner.side, true);
            unitsInRange.Remove(defender);
            WorldManager.Instance.RandomSelect(unitsInRange, skillCfg.TargetCount);
            foreach(var unit in unitsInRange)
            {
                unit.OnSkillDamaged(owner, skillId, (int)(damage * skillCfg.SkillDamageRate));
            }

            EffectManager.PlaySkillEffect(owner, skillCfg.HitEffect);
        }
    }
}
