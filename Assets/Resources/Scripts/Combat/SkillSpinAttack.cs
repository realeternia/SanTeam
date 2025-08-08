using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

public class SkillSpinAttack : Skill
{
    public SkillSpinAttack(int id, Chess unit) : base(id, unit)

    {
    }

    public override void OnAttack(Chess defender, int damage)
    {
        if(CheckBurst())
        {
            Debug.Log("SkillSpinAttack");
            Vector2Int centerPos = WorldManager.Instance.WorldToGridPosition(owner.transform.position, true);
            var unitsInRange = WorldManager.Instance.GetUnitsInRange(centerPos, 20, owner.side, true);
            foreach(var unit in unitsInRange)
            {
                if(unit == defender)
                    continue;
                unit.hp -= damage;
            }

            EffectManager.PlaySkillEffect(owner, skillCfg.HitEffect);
        }
    }
}
