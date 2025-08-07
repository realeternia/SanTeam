using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

public class CriticalAttack : Skill
{
    public CriticalAttack(int id) : base(id)
    {
    }

    public override void DuringAttack(Chess attacker, Chess defender, ref int damage, ref string effect)
    {
        Debug.Log("CriticalAttack");

        damage *= 2;
        effect = skillCfg.HitEffect;

        WorldManager.Instance.AddBattleText(damage.ToString() + "!", defender.transform.position, new UnityEngine.Vector2(0, 400), Color.red, 2);


        EffectManager.PlaySkillEffect(attacker, skillCfg.HitEffect);

    }
}
