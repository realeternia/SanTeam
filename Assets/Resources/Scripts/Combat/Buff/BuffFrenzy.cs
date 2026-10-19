using System;

/// <summary>
/// 伤害提升（"重"，BuffConfig 300004）：造成的伤害按 StrengthBuff1 语义 key "damageUp" 比例提升。
/// 只负责出手增伤；"受到伤害提升"由通用负面 Buff"伤"（BuffDamagedAddRate）承担，
/// 需要双刃效果时在 SkillConfig.BuffId 里配置多段组合（如华雄·狂暴 "重,伤"）。
/// </summary>
public class BuffFrenzy : Buff
{
    public BuffFrenzy(int id, int skillId, Chess caster, Chess target, float lastTime)
     : base(id, skillId, caster, target, lastTime)
    {
    }

    // 增伤：出手结算阶段，读 StrengthBuff1 语义 key "damageUp"
    public override void BeforeCalDamage(Chess defender, ref int damageBase, ref float damageMulti, ref string effect, string hurtTag)
    {
        damageMulti += GetStrBuff("damageUp");
    }
}
