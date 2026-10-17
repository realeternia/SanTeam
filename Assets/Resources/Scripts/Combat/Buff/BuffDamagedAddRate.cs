using System;

public class BuffDamagedAddRate : Buff
{
    public BuffDamagedAddRate(int id, int skillId, Chess caster, Chess target, float lastTime)
     : base(id, skillId, caster, target, lastTime)
    {
    }

    // 受击增伤挂在伤害计算阶段：调整受伤倍率，普攻与技能伤害统一生效（原挂在普攻专属的 DuringAttacked 上，技能伤害不会触发）
    // 约定：DamageStrength 为伤害基值（仅伤害/治疗/DOT 使用），其余数值一律读 StrengthBuff1[k]
    // 槽位约定：单独使用（如铁骑/权奸/标记的"伤"）读 [0]；与其它数值 Buff 组合时（如华雄"重,伤"：StrengthBuff1=[增伤, 受击加深]）读 [1]
    public override void BeforeCalDamaged(Chess attacker, ref int damageBase, ref float damageMulti, ref string effect, string hurtTag)
    {
        // StrengthBuff1 为压缩数组（只存非零值）：空数组表示无受击增伤加成（如"铁骑"2010006~2010010）
        var slot = skillCfg.StrengthBuff1.Length > 1 ? 1 : 0;
        damageMulti += skillCfg.StrengthBuff1.Length > slot ? skillCfg.StrengthBuff1[slot] : 0f;
    }
}