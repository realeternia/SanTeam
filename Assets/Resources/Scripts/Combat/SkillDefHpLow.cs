using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

public class SkillDefHpLow : Skill
{
    public SkillDefHpLow(int id, Chess unit) : base(id, unit)
    {
    }

    public override void DuringAttacked(Chess attacker, string damType, ref int damageBase, ref float damageMulti, ref string effect)
    {
        if (owner.HpRate < skillCfg.ConditionParm && CheckBurst())
        {
            WorldManager.Instance.AddBattleText("抵抗", owner.transform.position, new UnityEngine.Vector2(0, 60), Color.red, 3);
            damageMulti -= skillCfg.Strength;
        }
    }

}
