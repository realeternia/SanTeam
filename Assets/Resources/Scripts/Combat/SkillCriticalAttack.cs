using System;
using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

public class SkillCriticalAttack : Skill
{
    public SkillCriticalAttack(int id, Chess unit) : base(id, unit)

    {
    }

    public override void DuringAttack(Chess defender, string damType, ref int damageBase, ref float damageMulti, ref string effect)
    {
        if(CheckBurst())
        {
            var maxVal = Math.Max(10, Math.Max(owner.leadShip - defender.leadShip, owner.str - defender.str));
            Debug.Log("CriticalAttack " + damageBase.ToString() + " " + damageMulti.ToString() + " " + effect);

            damageMulti += 0.4f + (float)maxVal * skillCfg.Strength;

            effect = skillCfg.HitEffect;
        }
    }

    public override void OnAttack(Chess defender, int damage)
    {
        if(isBurst)
            WorldManager.Instance.AddBattleText(damage.ToString() + "!", defender.transform.position, new UnityEngine.Vector2(0, 60), Color.red, 3);
    }

}
