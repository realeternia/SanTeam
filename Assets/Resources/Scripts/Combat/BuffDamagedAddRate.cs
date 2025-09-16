using System;

public class BuffDamagedAddRate : Buff
{
    public BuffDamagedAddRate(int id, int skillId, Chess caster, Chess target, float lastTime)
     : base(id, skillId, caster, target, lastTime)
    {
    }

    public override void DuringAttacked(Chess attacker, string damType, ref int damageBase, ref float damageMulti, ref string effect)
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