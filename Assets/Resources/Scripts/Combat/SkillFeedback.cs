using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

public class SkillFeedback : Skill
{
    public SkillFeedback(int id, Chess unit) : base(id, unit)
    {
    }

    public override void OnAttacked(Chess attacker, int damage)
    {
        if(CheckBurst())
        {

            if (WorldManager.Instance.CheckInRange(owner.transform.position, attacker.transform.position, skillCfg.Range))
            {
                attacker.OnSkillDamaged((int)(damage * skillCfg.Strength));
                EffectManager.PlaySkillEffect(attacker, skillCfg.HitEffect);

                WorldManager.Instance.AddBattleText("反" +damage.ToString(), attacker.transform.position, new UnityEngine.Vector2(0, 150), new UnityEngine.Color(0.65f, 0.31f, 0), 3);
            }
            
        }
    }
}
