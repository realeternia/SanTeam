using System;

public class BuffDamagedAddRate : Buff
{
    public BuffDamagedAddRate(int id, int skillId, Chess caster, Chess target, float lastTime)
     : base(id, skillId, caster, target, lastTime)
    {
    }

    // 受击增伤挂在伤害计算阶段：调整受伤倍率，普攻与技能伤害统一生效（原挂在普攻专属的 DuringAttacked 上，技能伤害不会触发）
    // 约定：DamageStrength 为伤害基值（仅伤害/治疗/DOT 使用），其余数值一律读 StrengthBuff1[k]
    public override void BeforeCalDamaged(Chess attacker, ref int damageBase, ref float damageMulti, ref string effect, string hurtTag)
    {
        // StrengthBuff1 为压缩数组（只存非零值）：空数组表示无受击增伤加成（如"冲锋"2010006~2010010）
        damageMulti += skillCfg.StrengthBuff1.Length > 0 ? skillCfg.StrengthBuff1[0] : 0f;
    }
}