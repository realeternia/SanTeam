using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

public class SkillPlantSkin : Skill
{
    public SkillPlantSkin(int id, Chess unit) : base(id, unit)
    {
    }

    public override void DuringAttacked(Chess attacker, string damType, ref int damageBase, ref float damageMulti, ref string effect)
    {
        if (CheckBurst())
        {
            if (damType != "inte")
            {
                WorldManager.Instance.AddBattleText("弱点", owner.transform.position, new UnityEngine.Vector2(0, 60), Color.red, 3);
                damageMulti += skillCfg.Strength;
            }
            else
            {
                WorldManager.Instance.AddBattleText("抵抗", owner.transform.position, new UnityEngine.Vector2(0, 60), Color.green, 3);
                damageMulti -= skillCfg.Strength;
            }
        }
    }

}
