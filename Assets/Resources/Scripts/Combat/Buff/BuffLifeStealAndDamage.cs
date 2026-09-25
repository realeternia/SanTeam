/// <summary>
/// 吸血增伤 Buff：造成伤害前按 Strength 提升伤害倍率（增伤），
/// 命中对敌人造成的伤害按 Strength2 比例转化为自身治疗（吸血）。
/// </summary>
public class BuffLifeStealAndDamage : Buff
{
    public BuffLifeStealAndDamage(int id, int skillId, Chess caster, Chess target, float lastTime)
     : base(id, skillId, caster, target, lastTime)
    {
    }

    public override void BeforeCalDamage(Chess defender, ref int damageBase, ref float damageMulti, ref string effect, string hurtTag)
    {
        damageMulti += skillCfg.Strength;
    }

    public override void OnAttack(Chess defender, int damage)
    {
        owner.HealTarget(owner, skillCfg.Id, (int)(damage * skillCfg.Strength2), false);
    }
}