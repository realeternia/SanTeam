using System;
using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

public class SkillHitFood : Skill
{
    public SkillHitFood(int id, Chess unit) : base(id, unit)
    {
    }

    public override void OnAttack(Chess defender, string damType, int damage)
    {
        if (CheckBurst(defender))
        {
            var sub = defender.GetPlayerInfo().SubFood(skillCfg.StrengthInt);
            if (sub > 0)
            {
                owner.PlayerAnim(skillCfg.Action);
                owner.GetPlayerInfo().AddFood(sub);
                WorldManager.Instance.AddBattleText("粮-" + sub.ToString(), defender.transform.position, new UnityEngine.Vector2(0, -30), Color.red, 3);
                WorldManager.Instance.AddBattleText("粮+" + sub.ToString(), owner.transform.position, new UnityEngine.Vector2(0, 60), Color.green, 3);
            }
        }
    }

}
