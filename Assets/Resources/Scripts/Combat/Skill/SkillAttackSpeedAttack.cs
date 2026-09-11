using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

public class SkillAttackSpeedAttack : Skill
{
    public SkillAttackSpeedAttack(int id, Chess unit) : base(id, unit)
    {
    }

    public override void OnAttack(Chess defender, string damType, int damage)
    {
        if (CheckBurst(defender))
        {
            owner.PlayerAnim(skillCfg.Action);

            // Cooldown 参数为冷却百分比（1=完全冷却），最大不超过1
            owner.Cooldown(skillCfg.Strength);
        }
    }
}
