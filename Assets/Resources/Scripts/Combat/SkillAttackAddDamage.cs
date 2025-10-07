using System;
using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

public class SkillAttackAddDamage : Skill
{
    public SkillAttackAddDamage(int id, Chess unit) : base(id, unit)
    {
    }

    public override void DuringAttack(Chess defender, string damType, ref int damageBase, ref float damageMulti, ref int damageReal, ref string effect)
    {
        if(CheckBurst(defender))
        {
            if(!string.IsNullOrEmpty(skillCfg.Action))
                owner.PlayerAnim(skillCfg.Action);            

            damageBase += skillCfg.StrengthInt;
            damageMulti += skillCfg.Strength;
            effect = skillCfg.HitEffect;
        }
    }

    public override void OnAttack(Chess defender, string damType, int damage)
    {
        if(isBurst)
            WorldManager.Instance.AddBattleText(damage.ToString() + "!", defender.transform.position, new UnityEngine.Vector2(0, 60), Color.red, 3);
    }

}
