using System;

/// <summary>
/// 魔抗提升（"抗"，BuffConfig 300013）：按 StrengthBuff1 的数值提升魔抗（magicRes），移除时还原。
/// 槽位约定：单独使用读 [0]；与其它数值 Buff 组合时（如袁绍·名门 "攻,甲,抗"：StrengthBuff1=[攻击, 护甲, 魔抗]）读 [2]。
/// </summary>
public class BuffMagicResAdd : Buff
{
    private int diff;

    public BuffMagicResAdd(int id, int skillId, Chess caster, Chess target, float lastTime)
     : base(id, skillId, caster, target, lastTime)
    {
    }

    public override void OnAdd(Chess chess, Chess caster)
    {
        base.OnAdd(chess, caster);
        var slot = skillCfg.StrengthBuff1.Length > 2 ? 2 : 0;
        diff = skillCfg.StrengthBuff1.Length > slot ? (int)skillCfg.StrengthBuff1[slot] : 0;
        chess.magicRes += diff;
    }

    public override void OnRemove(Chess chess)
    {
        chess.magicRes -= diff;
        base.OnRemove(chess);
    }
}
