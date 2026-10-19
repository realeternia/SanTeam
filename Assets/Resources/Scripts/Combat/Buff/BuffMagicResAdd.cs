using System;

/// <summary>
/// 魔抗提升（"抗"，BuffConfig 300013）：按 StrengthBuff1 语义 key "magicRes" 的数值提升魔抗（magicRes），移除时还原。
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
        diff = (int)GetStrBuff("magicRes");
        chess.magicRes += diff;
    }

    public override void OnRemove(Chess chess)
    {
        chess.magicRes -= diff;
        base.OnRemove(chess);
    }
}
