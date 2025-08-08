using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

public class SkillCriticalAttack : Skill
{
    public SkillCriticalAttack(int id) : base(id)
    {
    }

    public override void DuringAttack(Chess attacker, Chess defender, ref int damageBase, ref float damageMulti, ref string effect)
    {
        Debug.Log("CriticalAttack " + damageBase.ToString() + " " + damageMulti.ToString() + " " + effect);

        damageMulti += 1f;
        effect = skillCfg.HitEffect;

        BuffManager.AddBuff(attacker, attacker, 300001, 30f);
        UpdateCD();
    }

    public override void OnAttack(Chess attacker, Chess defender, int damage)
    {
        WorldManager.Instance.AddBattleText(damage.ToString() + "!", defender.transform.position, new UnityEngine.Vector2(0, 400), Color.red, 2);
    }

}
