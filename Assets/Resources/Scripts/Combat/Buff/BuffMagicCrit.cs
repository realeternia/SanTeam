/// <summary>
/// 法术暴击（短名"法暴"，BuffConfig 300020）：携带者造成的法术技能伤害按概率翻倍。
/// 判定统一在 Skill.ApplyDamageModifiers 中完成（仅 DamageType=法术 的技能伤害），
/// 概率与倍率从施加技能 StrengthBuff1 的 magicCritChance / magicCritRate 读取（默认 30% / 2 倍）。
/// </summary>
public class BuffMagicCrit : Buff
{
    public BuffMagicCrit(int id, int skillId, Chess caster, Chess target, float lastTime)
     : base(id, skillId, caster, target, lastTime)
    {
    }

    /// <summary>法术暴击概率（0.3 = 30%）。</summary>
    public float Chance { get { return GetStrBuff("magicCritChance", 0.3f); } }

    /// <summary>法术暴击伤害倍率（2 = 双倍法术伤害）。</summary>
    public float Rate { get { return GetStrBuff("magicCritRate", 2f); } }
}
