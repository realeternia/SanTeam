using System;
using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 风华：攻击命中目标时造成额外魔法伤害（Strength=0.2~0.5，即攻击力的20%~50%）。
/// 附加伤害作为独立魔法伤害经 OnSkillDamaged 结算（IsMagic=true 受目标魔抗减免），
/// 不并入普攻的物理伤害，也不随暴击/闪避，仅普攻实际命中时触发。
/// </summary>
public class SkillAttackMagicDamage : Skill
{
    public SkillAttackMagicDamage(int id, Chess unit) : base(id, unit)
    {
    }

    public override void OnAttack(Chess defender, int damage)
    {
        if (skillCfg.Strength <= 0)
            return;

        // 以攻击基准值(atk)的百分比作为额外魔法伤害，独立结算（魔法伤害受目标魔抗减免）
        var attackBase = owner.GetAttr("atk");
        var magicDmg = Math.Max(1, (int)(attackBase * skillCfg.Strength));
        defender.OnSkillDamaged(owner, skillId, magicDmg, false, skillCfg.HurtTag);

        if (!string.IsNullOrEmpty(skillCfg.HitEffect))
            EffectManager.PlaySkillEffect(defender, skillCfg.HitEffect);
    }
}
