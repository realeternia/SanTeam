using System;
using UnityEngine;

public class BuffShieldValue : Buff
{
    public BuffShieldValue(int id, int skillId, Chess caster, Chess target, float lastTime)
     : base(id, skillId, caster, target, lastTime)
    {
    }

    public override void DuringAttacked(Chess attacker, ref int damageBase, ref float damageMulti, ref string effect)
    {
        // 减伤盾：恒定按 Strength 减免，不再做攻守属性对比
        var strength = skillCfg.Strength;
        damageMulti -= strength;
        WorldManager.Instance.AddBattleText("抵抗", owner.transform.position, new UnityEngine.Vector2(0, 60), Color.green, 3);
    }
}