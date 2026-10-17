using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

public class SkillHitBuffArea : Skill
{
    public SkillHitBuffArea(int id, Chess unit) : base(id, unit)
    {
    }

    public override void OnAttack(Chess defender, int damage)
    {
        if (CheckBurst(defender))
        {
            
            var targetUnit = skillCfg.TargetType == "targetUnit" ? defender : owner;

            PlaySelfEffect();
            owner.PlayerAnim(skillCfg.Action);

            var unitsInRange = WorldManager.Instance.GetEnemyInRange(targetUnit.transform.position, skillCfg.Area, owner.side);
            if (unitsInRange.Count > 0)
            {
                PlayAreaEffect(targetUnit.transform.position);

                WorldManager.Instance.RandomSelect(unitsInRange, skillCfg.TargetCount);
                // 配置了 DamageStrength 才结算伤害（为0时仅上buff），统一走 GetSkillDamage 按 DamageType 计算
                var skillDamage = GetSkillDamage();
                var buffCfg = BuffConfig.GetConfigByNameS(skillCfg.BuffId);
                foreach (var unit in unitsInRange)
                {
                    if (skillDamage > 0)
                        unit.OnSkillDamaged(owner, skillId, skillDamage);
                    BuffManager.AddBuff(unit, owner, id, buffCfg.Id, skillCfg.BuffTime);
                    EffectManager.PlaySkillEffect(targetUnit, skillCfg.HitEffect);
                }
            }
        }
    }

}
