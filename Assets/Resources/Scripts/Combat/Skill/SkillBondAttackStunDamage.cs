using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 深谋（好友连锁·谋）：攻击时按概率(Rate)眩晕目标(BuffId=眩晕buff"乱"，时长BuffTime)，且攻击已被眩晕的目标时额外造成
/// key "stunDamage"(=0.5，50%)伤害。两个效果分别复用 SkillBondHitBuff（概率挂 Buff）与 SkillBondAttackAddDamage（攻击增伤）机制。
/// </summary>
public class SkillBondAttackStunDamage : Skill
{
    public SkillBondAttackStunDamage(int id, Chess unit) : base(id, unit)
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

    // 攻击已眩晕的目标时额外造成50%(key "stunDamage")伤害
    public override void BeforeCalDamage(Chess target, SkillConfig castSkillCfg, ref int damageBase, ref float damageMulti, ref string effect, string hurtTag, bool isFeedback)
    {
        // 仅普攻触发：技能伤害来源(castSkillCfg != null)不参与判定
        if (castSkillCfg != null)
            return;

        if (string.IsNullOrEmpty(skillCfg.BuffId) || !target.HasBuff(BuffConfig.GetConfigByNameS(skillCfg.BuffId).Id))
            return;
        if (GetStr2("stunDamage") > 0f)
            damageMulti += GetStr2("stunDamage");
        effect = skillCfg.HitEffect;
    }

}