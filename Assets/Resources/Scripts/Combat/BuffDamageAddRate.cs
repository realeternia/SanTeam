using System;

public class BuffDamageAddRate : Buff
{
    public BuffDamageAddRate(int id, int skillId, Chess caster, Chess target, float lastTime)
     : base(id, skillId, caster, target, lastTime)
    {
    }

    public override void DuringAttack(Chess defender, string damType, ref int damageBase, ref float damageMulti,  ref string effect)
    {
        if (damageBase < 10)
        {
            damageBase = 13;
        }
        else
        {
            damageMulti += skillCfg.Strength;
        }
    }
}