using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

public class SkillAttackReboundArrow : Skill
{
    public SkillAttackReboundArrow(int id, Chess unit) : base(id, unit)
    {
    }

    public override void OnAttack(Chess defender, int damage)
    {
        var unitsInRange = WorldManager.Instance.GetEnemyInRange(defender.transform.position, skillCfg.Range, owner.side);
        unitsInRange.Remove(defender);

        if (unitsInRange.Count > 0 && CheckBurst(defender))
        {
            owner.PlayerAnim(skillCfg.Action);
            WorldManager.Instance.RandomSelect(unitsInRange, skillCfg.TargetCount);

            var reboundDamage = (int)(damage * skillCfg.Strength2[0]);
            // 弱攻击按倍率向下取整为0时不弹射，避免生成0伤害导弹（0伤害导弹结算会触发异常）
            if (reboundDamage <= 0)
                return;
            foreach (var unit in unitsInRange)
                WorldManager.Instance.CreateSpellMissile(owner, unit, defender.transform.position, id, reboundDamage, owner.hitEffect);
        }
    }
}
