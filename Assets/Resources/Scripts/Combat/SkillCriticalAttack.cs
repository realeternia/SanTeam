using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

public class SkillCriticalAttack : Skill
{
    public SkillCriticalAttack(int id, Chess unit) : base(id, unit)

    {
    }

    public override void DuringAttack(Chess defender, ref int damageBase, ref float damageMulti, ref string effect)
    {
        if(CheckBurst())
        {
            Debug.Log("CriticalAttack " + damageBase.ToString() + " " + damageMulti.ToString() + " " + effect);

            damageMulti += 1f;
            effect = skillCfg.HitEffect;

            BuffManager.AddBuff(owner, owner, 300001, 30f);
        }
    }

    public override void OnAttack(Chess defender, int damage)
    {
        if(isBurst)
            WorldManager.Instance.AddBattleText(damage.ToString() + "!", defender.transform.position, new UnityEngine.Vector2(0, 400), Color.red, 2);
    }

}
