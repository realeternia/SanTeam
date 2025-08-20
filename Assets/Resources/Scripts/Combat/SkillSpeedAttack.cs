using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

public class SkillSpeedAttack : Skill
{
    public SkillSpeedAttack(int id, Chess unit) : base(id, unit)
    {
    }

    public override void OnAttack(Chess defender, int damage)
    {
        if (WorldManager.Instance.CheckInRange(owner.transform.position, defender.transform.position, skillCfg.Range))
        {
            if (CheckBurst())
            {
                Debug.Log("SkillSpeedAttack");

                owner.Cooldown(2 * skillCfg.Strength);
            }
        }
    }
}
