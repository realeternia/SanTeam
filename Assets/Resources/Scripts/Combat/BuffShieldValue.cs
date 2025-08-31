using System;

public class BuffShieldValue : Buff
{
    public BuffShieldValue(int id, int skillId, Chess caster, Chess target, float lastTime)
     : base(id, skillId, caster, target, lastTime)
    {
    }

    public override void DuringAttacked(Chess attacker, string damType, ref int damageBase, ref float damageMulti, ref string effect)
    {
        var valChange = (owner.GetAttr(skillCfg.Attr) - attacker.GetAttr(skillCfg.Attr)) * skillCfg.Strength;
        if (damType == "inte")
            valChange = valChange * 2 / 3;
        valChange = Math.Clamp(valChange, 3, 40);

        damageBase -= (int)valChange;
    }
}