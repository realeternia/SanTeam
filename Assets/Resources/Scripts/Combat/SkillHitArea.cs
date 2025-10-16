using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

public class SkillHitArea : Skill
{
    public SkillHitArea(int id, Chess unit) : base(id, unit)
    {
    }

    public override void OnAttack(Chess defender, string damType, int damage)
    {
        if (CheckBurst(defender))
        {
            owner.PlayerAnim(skillCfg.Action);

            var targetPos = defender.transform.position;
            //创建一个hitEffect
            EffectManager.PlaySkillEffect(defender, skillCfg.HitEffect);

            var unitsInRange = WorldManager.Instance.GetUnitsInRange(targetPos, skillCfg.Range, owner.side, true);
            unitsInRange.Remove(defender);
            if (unitsInRange.Count > 0)
            {
                WorldManager.Instance.RandomSelect(unitsInRange, skillCfg.TargetCount);
                var damage2 = (int)(damage * skillCfg.SkillDamageRate);
                foreach(var unit in unitsInRange)
                    unit.OnSkillDamaged(owner, skillId, damage2);
            }
        }
    }

}
