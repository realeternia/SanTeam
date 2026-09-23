using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 谋略：攻击时按概率(Rate)眩晕目标(BuffId=眩晕buff"乱"，时长BuffTime)，且攻击已被眩晕的目标时额外造成
/// Strength(=0.5，50%)伤害。两个效果复用枪·眩晕(SkillHitBuff)与增伤(SkillAttackAddDamage)机制。
/// </summary>
public class SkillAttackStunDamage : Skill
{
    public SkillAttackStunDamage(int id, Chess unit) : base(id, unit)
    {
    }

    // 攻击时概率眩晕目标（1~5级概率随 Rate 逐渐提升）
    public override void OnAttack(Chess defender, int damage)
    {
        if (CheckBurst(defender))
        {
            owner.PlayerAnim(skillCfg.Action);
            BuffManager.AddBuff(defender, owner, id, BuffConfig.GetConfigByNameS(skillCfg.BuffId).Id, skillCfg.BuffTime);
        }
    }

    // 攻击已眩晕的目标时额外造成50%(Strength)伤害
    public override void BeforeCalDamage(Chess target, SkillConfig castSkillCfg, ref int damageBase, ref float damageMulti, ref string effect, string hurtTag, bool isFeedback)
    {
        if (string.IsNullOrEmpty(skillCfg.BuffId) || !target.HasBuff(BuffConfig.GetConfigByNameS(skillCfg.BuffId).Id))
            return;
        if (skillCfg.Strength > 0)
            damageMulti += skillCfg.Strength;
        effect = skillCfg.HitEffect;
    }

}