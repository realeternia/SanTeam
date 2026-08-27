using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

public class SkillHitRepeat : Skill
{
    public SkillHitRepeat(int id, Chess unit) : base(id, unit)
    {
    }

    public override void OnAttack(Chess defender, string damType, int damage)
    {
        if(CheckBurst(defender))
        {
            WorldManager.Instance.AddBattleText(damage.ToString() + "!", defender.transform.position, new UnityEngine.Vector2(0, 60), Color.red, 3);
            owner.PlayerAnim(skillCfg.Action);
            owner.StartCoroutine(DelayAttack(defender, damage));
        }
    }

    IEnumerator DelayAttack(Chess defender, int damage)
    {
        for (int i = 0; i < skillCfg.DoCount; i++)
        {
            yield return new WaitForSeconds(skillCfg.TimeDelay);
            if (defender != null && defender.hp > 0)
            {
                var d = (int)(damage * skillCfg.SkillDamageRate);
                defender.OnSkillDamaged(owner, skillId, d);
                EffectManager.PlaySkillEffect(defender, skillCfg.HitEffect);
                WorldManager.Instance.AddBattleText(d.ToString() + "!", defender.transform.position, new UnityEngine.Vector2(0, 60), Color.red, 3);
            }
        }
    }
}
