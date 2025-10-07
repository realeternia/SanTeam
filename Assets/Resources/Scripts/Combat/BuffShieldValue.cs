using System;
using UnityEngine;

public class BuffShieldValue : Buff
{
    public BuffShieldValue(int id, int skillId, Chess caster, Chess target, float lastTime)
     : base(id, skillId, caster, target, lastTime)
    {
    }

    public override void DuringAttacked(Chess attacker, string damType, ref int damageBase, ref float damageMulti, ref string effect)
    {
        var strength = skillCfg.Strength;
        if((float)attacker.GetAttr(skillCfg.Attr) > owner.GetAttr(skillCfg.Attr) * 1.2f)
            strength *= .75f;

        damageMulti -= strength;
        WorldManager.Instance.AddBattleText("抵抗", owner.transform.position, new UnityEngine.Vector2(0, 60), Color.green, 3);
    }
}