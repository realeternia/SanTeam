using System;
using UnityEngine;

public class BuffShieldValue : Buff
{
    public BuffShieldValue(int id, int skillId, Chess caster, Chess target, float lastTime)
     : base(id, skillId, caster, target, lastTime)
    {
    }

    public override void BeforeCalDamaged(Chess attacker, ref int damageBase, ref float damageMulti, ref string effect, string hurtTag)
    {
        // 减伤盾：恒定按 Strength2（比例槽）减免，不再做攻守属性对比（挂在伤害计算阶段，普攻与技能伤害统一生效）
        // 约定：Strength 为技能主数值/伤害，比例类参数放 Strength2
        var rate = skillCfg.Strength2;
        damageMulti -= rate;
        WorldManager.Instance.AddBattleText("抵抗", owner.transform.position, new UnityEngine.Vector2(0, 60), Color.green, 3);
    }
}