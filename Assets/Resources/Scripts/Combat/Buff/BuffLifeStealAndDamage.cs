/// <summary>
/// 吸血增伤 Buff：造成伤害前按 StrengthBuff1[0] 提升伤害倍率（增伤），
/// 命中对敌人造成的伤害按 StrengthBuff1[1] 比例转化为自身治疗（吸血）。
/// </summary>
public class BuffLifeStealAndDamage : Buff
{
    public BuffLifeStealAndDamage(int id, int skillId, Chess caster, Chess target, float lastTime)
     : base(id, skillId, caster, target, lastTime)
    {
    }

    public override void BeforeCalDamage(Chess defender, ref int damageBase, ref float damageMulti, ref string effect, string hurtTag)
    {
        damageMulti += skillCfg.StrengthBuff1[0];
    }

    public override void OnAttack(Chess defender, int damage)
    {
        owner.HealTarget(owner, skillCfg.Id, (int)(damage * skillCfg.StrengthBuff1[1]), false);
    }
}