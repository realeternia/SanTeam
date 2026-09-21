using System;

public class BuffDamageAddRate : Buff
{
    public BuffDamageAddRate(int id, int skillId, Chess caster, Chess target, float lastTime)
     : base(id, skillId, caster, target, lastTime)
    {
    }

    // 增伤挂在伤害计算阶段：调整伤害倍率，普攻与技能伤害统一生效（原挂在普攻专属的 DuringAttack 上，技能伤害不会触发）
    public override void BeforeCalDamage(Chess defender, ref int damageBase, ref float damageMulti, ref string effect, string hurtTag)
    {
        damageMulti += skillCfg.Strength;
    }
}