using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 埋伏：被攻击时，若攻击者距离超过技能距离阈值(Range，随等级提升)则瞬移近身，
/// 施加眩晕buff，同时给予一次技能伤害（GetSkillDamage，随等级提升）；瞬移后若有护盾强度(key "shield")则额外套吸收盾。
/// Buff 短名由 SkillConfig.BuffId 以逗号分隔配置：第 1 个=眩晕Buff，第 2 个=护盾Buff（如 "乱,盾"）。
/// </summary>
public class SkillAttackedTeleport : Skill
{
    public SkillAttackedTeleport(int id, Chess unit) : base(id, unit)
    {
    }

    public override void OnAttacked(Chess attacker, int damage)
    {
        // 攻击者距离小于 Range（Lv1=20）时不发动；超过阈值才瞬移反击
        var dist = WorldManager.Instance.GetRange(owner.transform.position, attacker.transform.position);
        if (dist > 20 && dist < skillCfg.Range && CheckBurst(attacker))
        {
            owner.PlayerAnim(skillCfg.Action);

            Vector3 direction = (attacker.transform.position - owner.transform.position).normalized;
            Vector3 randomPosition = attacker.transform.position - direction * 12;

            owner.MoveTo(randomPosition, true);
            owner.LockTarget(attacker);
            PlaySelfEffect();

            // BuffId 逗号分隔：buffIds[0]=眩晕Buff，buffIds[1]=护盾Buff
            var buffIds = GetSkillBuffIds();
            if (buffIds.Count < 1)
            {
                GameLog.Error($"SkillAttackedTeleport: 缺少眩晕Buff配置 BuffId={skillCfg.BuffId} 技能id={id}");
                return;
            }

            // 施加眩晕buff，同时给予一次技能伤害
            BuffManager.AddBuff(attacker, owner, id, buffIds[0], skillCfg.BuffTime);
            attacker.OnSkillDamaged(owner, id, GetSkillDamage());
            if (!string.IsNullOrEmpty(skillCfg.HitEffect))
                EffectManager.PlaySkillEffect(attacker, skillCfg.HitEffect);

            // 白衣渡江：瞬移反击后给自己施加吸收盾，盾量统一走 GetSkillShield（随法强成长）
            var shieldHp = GetSkillShield();
            if (shieldHp > 0 && buffIds.Count >= 2)
            {
                int shieldBuffId = buffIds[1];
                BuffManager.AddBuff(owner, owner, id, shieldBuffId, 999f);
                var shield = owner.GetBuff(shieldBuffId) as BuffShield;
                if (shield != null)
                    shield.AddHp(shieldHp);
            }
        }
    }

}
