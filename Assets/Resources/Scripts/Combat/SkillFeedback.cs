using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

public class SkillFeedback : Skill
{
    public SkillFeedback(int id, Chess unit) : base(id, unit)
    {
    }

    public override void OnAttacked(Chess attacker, string damType, int damage)
    {
        if(skillCfg.Attr != null && skillCfg.Attr !=damType)
            return;

        if(skillCfg.RangeOut && WorldManager.Instance.CheckInRange(owner.transform.position, attacker.transform.position, skillCfg.Range))
            return;
        if(!skillCfg.RangeOut && !WorldManager.Instance.CheckInRange(owner.transform.position, attacker.transform.position, skillCfg.Range))
            return;            

        if (CheckBurst())
        {
            var damageBack = (int)(damage * skillCfg.Strength);
            attacker.OnSkillDamaged(owner, damageBack);
            EffectManager.PlaySkillEffect(attacker, skillCfg.HitEffect);

            WorldManager.Instance.AddBattleText("反" + damageBack.ToString(), attacker.transform.position, new UnityEngine.Vector2(0, 150), new UnityEngine.Color(0.65f, 0.31f, 0), 3);
        }
    }
}
