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

    public override void BeforeCalDamage(Chess target, SkillConfig castSkillCfg, ref int damageBase, ref float damageMulti, ref string effect, string hurtTag, bool isFeedback)
    {
        if(!string.IsNullOrEmpty(skillCfg.BuffId) && !target.HasBuff(BuffConfig.GetConfigByNameS(skillCfg.BuffId).Id))
            return;

        if(CheckBurst(target))
        {
            owner.PlayerAnim(skillCfg.Action);

            damageBase += skillCfg.StrengthInt;
            if(skillCfg.Strength > 0)
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
