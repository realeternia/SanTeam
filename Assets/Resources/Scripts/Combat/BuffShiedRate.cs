using System;

public class BuffShieldRate : Buff
{
    public BuffShieldRate(int id, int skillId, Chess caster, Chess target, float lastTime)
     : base(id, skillId, caster, target, lastTime)
    {
    }

    public override void DuringAttacked(Chess attacker, string damType, ref int damageBase, ref float damageMulti, ref string effect)
    {
        var ratio = 0.3f + (owner.str - attacker.str) * skillCfg.Strength;
        if (damType == "inte")
            ratio = ratio * 2 / 3;
        ratio = Math.Clamp(ratio, 0.35f, 0.85f);

        damageMulti -= ratio;
    }
}