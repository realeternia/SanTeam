using System;

/// <summary>
/// 狂暴（双刃）：造成的伤害提升，但自身受到的伤害也提升。
/// 增伤挂在 BeforeCalDamage（攻击方出手结算倍率），受击加深挂在 BeforeCalDamaged（受击方结算倍率），
/// 数值统一读技能配置的 Strength2（SkillConfig）。
/// </summary>
public class BuffFrenzy : Buff
{
    public BuffFrenzy(int id, int skillId, Chess caster, Chess target, float lastTime)
     : base(id, skillId, caster, target, lastTime)
    {
    }

    // 增伤：出手结算阶段
    public override void BeforeCalDamage(Chess defender, ref int damageBase, ref float damageMulti, ref string effect, string hurtTag)
    {
        damageMulti += skillCfg.Strength2;
    }

    // 受击加深：受击结算阶段
    public override void BeforeCalDamaged(Chess attacker, ref int damageBase, ref float damageMulti, ref string effect, string hurtTag)
    {
        damageMulti += skillCfg.Strength2;
    }
}