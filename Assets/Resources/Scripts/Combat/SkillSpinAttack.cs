using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

public class SkillSpinAttack : Skill
{
    public SkillSpinAttack(int id) : base(id)
    {
    }

    public override void OnAttack(Chess attacker, Chess defender, int damage)
    {
        Debug.Log("SkillSpinAttack");
        Vector2Int centerPos = WorldManager.Instance.WorldToGridPosition(attacker.transform.position, true);
        var unitsInRange = WorldManager.Instance.GetUnitsInRange(centerPos, 20, attacker.side, true);
        foreach(var unit in unitsInRange)
        {
            if(unit == defender)
                continue;
            unit.hp -= damage;
        }

        EffectManager.PlaySkillEffect(attacker, skillCfg.HitEffect);

    }
}
