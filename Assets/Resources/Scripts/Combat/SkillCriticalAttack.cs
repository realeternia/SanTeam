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
        var maxVal = Math.Max(skillCfg.Rate, 0.2f + 0.1f * Math.Max(owner.leadShip - defender.leadShip, owner.str - defender.str));
        maxVal = Math.Clamp(maxVal, 0.3f, 0.6f);
        if(CheckBurst(maxVal))
        {
            Debug.Log("CriticalAttack " + damageBase.ToString() + " " + damageMulti.ToString() + " " + effect);

            damageMulti += skillCfg.Strength;
            effect = skillCfg.HitEffect;
        }
    }

    public override void OnAttack(Chess defender, int damage)
    {
        if(isBurst)
            WorldManager.Instance.AddBattleText(damage.ToString() + "!", defender.transform.position, new UnityEngine.Vector2(0, 60), Color.red, 3);
    }

}
