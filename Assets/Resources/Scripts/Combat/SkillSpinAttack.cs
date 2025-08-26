using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

public class SkillSpinAttack : Skill
{
    public SkillSpinAttack(int id, Chess unit) : base(id, unit)
    {
    }

    public override void OnAttack(Chess defender, string damType, int damage)
    {
        if(CheckBurst())
        {
            Debug.Log("SkillSpinAttack");
            var unitsInRange = WorldManager.Instance.GetUnitsInRange(owner.transform.position, skillCfg.Range, owner.side, true);
            unitsInRange.Remove(defender);
            WorldManager.Instance.RandomSelect(unitsInRange, skillCfg.TargetCount);
            foreach(var unit in unitsInRange)
            {
                unit.OnSkillDamaged(owner, (int)(damage * skillCfg.Strength));
            }

            EffectManager.PlaySkillEffect(owner, skillCfg.HitEffect);
        }
    }
}
