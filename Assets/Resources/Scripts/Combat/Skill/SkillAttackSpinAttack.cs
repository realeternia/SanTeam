using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 张飞·旋风斩：普攻时对周围最多 TargetCount 个敌人造成 Strength2[0]% 攻击伤害，
/// 并对被溅射的敌人附加流血（Buff 短名由 skillCfg.BuffId 配置），每秒伤害 = 本次溅射伤害 × StrengthBuff1[0]，持续 BuffTime 秒。
/// </summary>
public class SkillAttackSpinAttack : Skill
{
    public SkillAttackSpinAttack(int id, Chess unit) : base(id, unit)
    {
    }

    public override void OnAttack(Chess defender, int damage)
    {
        if(CheckBurst(defender))
        {
            owner.PlayerAnim(skillCfg.Action);
            var unitsInRange = WorldManager.Instance.GetEnemyInRange(owner.transform.position, skillCfg.Area, owner.side);
            unitsInRange.Remove(defender);
            WorldManager.Instance.RandomSelect(unitsInRange, skillCfg.TargetCount);
            foreach(var unit in unitsInRange)
            {
                var spinDamage = (int)(damage * skillCfg.Strength2[0]);
                unit.OnSkillDamaged(owner, skillId, spinDamage);
                if (!string.IsNullOrEmpty(skillCfg.HitEffect))
                    EffectManager.PlaySkillEffect(unit, skillCfg.HitEffect);
                ApplyBleed(unit, spinDamage);
            }

            PlayAreaEffect(owner.transform.position);
        }
    }

    // 对目标附加流血：每秒伤害 = 本次溅射伤害 × StrengthBuff1[0]
    private void ApplyBleed(Chess target, int baseDamage)
    {
        if (target == null || target.hp <= 0 || baseDamage <= 0)
            return;
        if (skillCfg.StrengthBuff1 == null || skillCfg.StrengthBuff1.Length < 1)
            return;

        var bleed = (int)(baseDamage * skillCfg.StrengthBuff1[0]);
        if (bleed <= 0)
            return;

        var buffCfg = BuffConfig.GetConfigByNameS(skillCfg.BuffId);
        if (buffCfg == null)
        {
            GameLog.Error("SkillAttackSpinAttack: 未找到流血Buff短名：" + skillCfg.BuffId);
            return;
        }

        BuffManager.AddBuff(target, owner, id, buffCfg.Id, skillCfg.BuffTime);
        var buff = target.GetBuff(buffCfg.Id) as BuffTimeDamage;
        if (buff != null)
            buff.SetDamage(bleed);
    }
}