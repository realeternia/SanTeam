using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

public class SkillHitTeleport : Skill
{
    public SkillHitTeleport(int id, Chess unit) : base(id, unit)
    {
    }

    public override void OnAttacked(Chess attacker, string damType, int damage)
    {
        if(!WorldManager.Instance.CheckInRange(owner.transform.position, attacker.transform.position, skillCfg.Range) && CheckBurst(attacker))
        {
            owner.PlayerAnim(skillCfg.Action);

            Vector3 direction = (attacker.transform.position - owner.transform.position).normalized;
            Vector3 randomPosition = attacker.transform.position - direction * 12;

            owner.MoveTo(randomPosition, true);
            owner.LockTarget(attacker);
            EffectManager.PlaySkillEffect(owner, skillCfg.HitEffect);

            BuffManager.AddBuff(attacker, owner, id, skillCfg.BuffId, skillCfg.BuffTime);
        }
    }

}
