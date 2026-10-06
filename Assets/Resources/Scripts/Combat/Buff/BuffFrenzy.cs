using System;

/// <summary>
/// 狂暴（双刃）：造成的伤害提升，但自身受到的伤害也提升。
/// 增伤挂在 BeforeCalDamage（攻击方出手结算倍率），受击加深挂在 BeforeCalDamaged（受击方结算倍率），
/// 两值分槽配置（SkillConfig.StrengthBuff1）：[0]=造成的伤害提升，[1]=受到的伤害提升，后者应小于前者。
/// 同时复用于"重"（300004，纯造成伤害提升，只配 [0]，受击槽缺省按 0 处理，等价于原 BuffDamageAddRate）。
/// </summary>
public class BuffFrenzy : Buff
{
    public BuffFrenzy(int id, int skillId, Chess caster, Chess target, float lastTime)
     : base(id, skillId, caster, target, lastTime)
    {
    }

    // 增伤：出手结算阶段，读 StrengthBuff1[0]
    public override void BeforeCalDamage(Chess defender, ref int damageBase, ref float damageMulti, ref string effect, string hurtTag)
    {
        // StrengthBuff1 为压缩数组（只存非零值）：长度不足时该值按 0 处理
        damageMulti += skillCfg.StrengthBuff1 != null && skillCfg.StrengthBuff1.Length > 0 ? skillCfg.StrengthBuff1[0] : 0f;
    }

    // 受击加深：受击结算阶段，读 StrengthBuff1[1]
    public override void BeforeCalDamaged(Chess attacker, ref int damageBase, ref float damageMulti, ref string effect, string hurtTag)
    {
        damageMulti += skillCfg.StrengthBuff1 != null && skillCfg.StrengthBuff1.Length > 1 ? skillCfg.StrengthBuff1[1] : 0f;
    }
}
