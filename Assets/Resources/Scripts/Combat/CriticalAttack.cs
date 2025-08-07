using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

public class CriticalAttack : Skill
{
    public CriticalAttack(int id) : base(id)
    {
    }

    public override void DuringAttack(Chess attacker, Chess defender, ref int damageBase, ref float damageMulti, ref string effect)
    {
        Debug.Log("CriticalAttack " + damageBase.ToString() + " " + damageMulti.ToString() + " " + effect);

        damageMulti += 1f;
        effect = skillCfg.HitEffect;

        WorldManager.Instance.AddBattleText(damageBase.ToString() + "!", defender.transform.position, new UnityEngine.Vector2(0, 400), Color.red, 2);

        EffectManager.PlaySkillEffect(attacker, skillCfg.HitEffect);

        UpdateCD();

    }
}
