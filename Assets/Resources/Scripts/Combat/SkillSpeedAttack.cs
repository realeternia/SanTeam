using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

public class SkillSpeedAttack : Skill
{
    public SkillSpeedAttack(int id, Chess unit) : base(id, unit)
    {
    }

    public override void OnAttack(Chess defender, string damType, int damage)
    {
        if (CheckBurst(defender))
        {
            Debug.Log("SkillSpeedAttack");

            owner.Cooldown(2 * skillCfg.Strength);
        }
    }
}
