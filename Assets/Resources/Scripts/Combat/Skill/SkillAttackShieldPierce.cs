using System;
using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 破盾：每次攻击对有护盾(吸收型)的目标额外造成%物理伤害（基于攻击基准值），
/// 携带 HurtTag 标签(如"AntiShield")绕过护盾直接打血（BuffShield 判定该标签不吸收）。
/// 伤害经 OnSkillDamaged 统一结算：物理(DamageType=物理)受护甲减免一次后打血
/// </summary>
public class SkillAttackShieldPierce : Skill
{
    public SkillAttackShieldPierce(int id, Chess unit) : base(id, unit)
    {
    }

    public override void BeforeCalDamage(Chess target, SkillConfig castSkillCfg, ref int damageBase, ref float damageMulti, ref string effect, string hurtTag, bool isFeedback)
    {
        // 破盾自身的穿透伤害(AntiShield)再进入伤害计算时不重复触发，避免递归
        if (hurtTag == CombatConst.AntiShieldHurtTag)
            return;

        // 只对有护盾(吸收型，BuffShield)的目标生效
        var shield = target.GetBuff(CombatConst.ShieldBuffId) as BuffShield;
        if (shield == null)
            return;

        if (!CheckBurst(target))
            return;

        owner.PlayerAnim(skillCfg.Action);
        if (!string.IsNullOrEmpty(skillCfg.HitEffect))
            effect = skillCfg.HitEffect;

        // 额外造成%物理伤害（基于攻击基准 atk：士兵的士兵攻击加成系数已折算进 atk；OnSkillDamaged 统一按物理护甲减免一次后绕过护盾打血）
        var attackBase = owner.GetAttr("atk");
        var pierce = Math.Max(1, (int)(attackBase * skillCfg.Strength));
        target.OnSkillDamaged(owner, skillId, pierce, false, skillCfg.HurtTag);
    }
}
