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
        if(skillCfg.BuffId > 0 && !defender.HasBuff(skillCfg.BuffId))
            return;

        if(CheckBurst(defender))
        {
            owner.PlayerAnim(skillCfg.Action);

            damageBase += skillCfg.StrengthInt;
            if(skillCfg.SkillDamageRate > 0)
                damageMulti += skillCfg.SkillDamageRate;
            effect = skillCfg.HitEffect;
        }
    }

    public override void OnAttack(Chess defender, string damType, int damage)
    {
        if(isBurst)
            WorldManager.Instance.AddBattleText(damage.ToString() + "!", defender.transform.position, new UnityEngine.Vector2(0, 60), Color.red, 3);
    }

}
