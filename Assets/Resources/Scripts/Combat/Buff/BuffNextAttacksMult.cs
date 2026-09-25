/// <summary>
/// 倍击数次 Buff：自带 3 次倍击次数，每次造成伤害前按 Strength 成倍提升伤害倍率，
/// 次数用尽后恢复正常伤害输出。
/// </summary>
public class BuffNextAttacksMult : Buff
{
    private int charges = 3;

    public BuffNextAttacksMult(int id, int skillId, Chess caster, Chess target, float lastTime)
     : base(id, skillId, caster, target, lastTime)
    {
    }

    public override void BeforeCalDamage(Chess defender, ref int damageBase, ref float damageMulti, ref string effect, string hurtTag)
    {
        if (charges > 0)
        {
            damageMulti *= skillCfg.Strength;
            charges--;
        }
    }
}