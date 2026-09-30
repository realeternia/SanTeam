using System;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 卸甲（刘晔）：状态 Buff，挂在携带者(owner)身上。
/// 状态期该单位对目标造成伤害时，若目标带吸收型护盾(BuffShield)，对其伤害按 /strength2 提升。
/// （无视目标护甲的部分由 SkillAidDisarmState.GetArmorDelta 挂钩 GetEffectiveArmor 折算）
/// </summary>
public class BuffDisarm : Buff
{
    public BuffDisarm(int id, int skillId, Chess caster, Chess unit, float lastTime)
        : base(id, skillId, caster, unit, lastTime)
    {
    }

    public override void BeforeCalDamage(Chess defender, ref int damageBase, ref float damageMulti, ref string effect, string hurtTag)
    {
        // 目标带吸收型护盾(BuffShield)时，对其伤害额外放大 Strength3
        if (defender != null && defender.GetBuff(CombatConst.ShieldBuffId) != null)
        {
            damageMulti *= (1f + skillCfg.Strength3);
        }
    }
}
