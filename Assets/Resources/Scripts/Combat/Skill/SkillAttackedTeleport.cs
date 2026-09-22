using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 埋伏：被攻击时，若攻击者距离超过技能距离阈值(Range，随等级提升)则瞬移近身，
/// 施加眩晕buff("乱")，同时给予一次技能伤害(Strength，随等级提升)
/// </summary>
public class SkillAttackedTeleport : Skill
{
    public SkillAttackedTeleport(int id, Chess unit) : base(id, unit)
    {
    }

    public override void OnAttacked(Chess attacker, int damage)
    {
        // 攻击者距离小于 Range（Lv1=20）时不发动；超过阈值才瞬移反击
        var dist = WorldManager.Instance.GetDistance(owner.transform.position, attacker.transform.position);
        if (dist > 20 && dist < skillCfg.Range && CheckBurst(attacker))
        {
            owner.PlayerAnim(skillCfg.Action);

            Vector3 direction = (attacker.transform.position - owner.transform.position).normalized;
            Vector3 randomPosition = attacker.transform.position - direction * 12;

            owner.MoveTo(randomPosition, true);
            owner.LockTarget(attacker);
            EffectManager.PlaySkillEffect(owner, skillCfg.HitEffect);

            // 施加眩晕buff，同时给予一次技能伤害
            BuffManager.AddBuff(attacker, owner, id, BuffConfig.GetConfigByNameS(skillCfg.BuffId).Id, skillCfg.BuffTime);
            attacker.OnSkillDamaged(owner, id, GetSkillDamage());
        }
    }

}
