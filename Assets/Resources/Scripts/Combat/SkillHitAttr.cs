using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

public class SkillHitAttr : Skill
{
    public SkillHitAttr(int id, Chess unit) : base(id, unit)
    {
    }

    public override void OnAttack(Chess defender, string damType, int damage)
    {
        if(CheckBurst(defender))
        {
            var roll = Random.Range(0, 3);
            var attr = roll == 0 ? "inte" : (roll == 1 ? "str" : "leadShip");
            owner.AddAttr(attr, skillCfg.StrengthInt);
            owner.PlayerAnim(skillCfg.Action);
            EffectManager.PlaySkillEffect(owner, skillCfg.HitEffect);
        }
    }

}
