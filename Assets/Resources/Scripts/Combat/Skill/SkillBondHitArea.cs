using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

public class SkillBondHitArea : Skill
{
    public SkillBondHitArea(int id, Chess unit) : base(id, unit)
    {
    }

    public override void OnAttack(Chess defender, int damage)
    {
        if (CheckBurst(defender))
        {
            owner.PlayerAnim(skillCfg.Action);

            var targetPos = defender.transform.position;
            //创建一个hitEffect
            PlaySkillEffect(defender, skillCfg.HitEffect);

            // 溅射范围：按 SelectOnly / SelectFirst 选（默认敌方的溅射目标，排除被普攻命中的单位）
            var unitsInRange = SelectTargets(targetPos, skillCfg.Area, false, skillCfg.TargetCount, defender);
            if (unitsInRange.Count > 0)
            {
                var damage2 = (int)(damage * GetStr2("splashRate"));
                foreach (var unit in unitsInRange)
                    if (damage2 > 0)
                        unit.OnSkillDamaged(owner, skillId, damage2);
            }
        }
    }

}
