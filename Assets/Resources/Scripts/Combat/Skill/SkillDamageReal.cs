using System;
using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

public class SkillDamageReal : Skill
{
    public SkillDamageReal(int id, Chess unit) : base(id, unit)
    {
    }

    public override void DuringAttack(Chess defender, string damType, ref int damageBase, ref float damageMulti, ref int damageReal, ref string effect)
    {
        if((float)defender.hp / defender.maxHp > skillCfg.ConditionParm)
            return;

        if(!string.IsNullOrEmpty(skillCfg.HitEffect))
            effect = skillCfg.HitEffect;

        if(CheckBurst(defender))
        {
            owner.PlayerAnim(skillCfg.Action);
            damageReal = (int)(defender.maxHp * skillCfg.Strength);
        }
    }

    public override void OnAttack(Chess defender, string damType, int damage)
    {
        if(isBurst)
            WorldManager.Instance.AddBattleText(damage.ToString() + "!", defender.transform.position, new UnityEngine.Vector2(0, 60), new Color(.6f, 0, .8f), 3);
    }    
}
