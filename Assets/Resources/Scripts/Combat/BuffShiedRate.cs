using System;

public class BuffShieldRate : Buff
{
    public BuffShieldRate(int id, int skillId, Chess unit, float lastTime)
     : base(id, skillId, unit, lastTime)
    {
    }

    public override void DuringAttacked(Chess attacker, string damType, ref int damageBase, ref float damageMulti, ref string effect)
    {
        var ratio = Math.Clamp(skillCfg.Strength + (owner.str - 80) * 0.005f, 0.5f, 0.75f);
        if (damType == "inte")
            ratio = ratio * 2 / 3;

        damageMulti -= ratio;
    }
}