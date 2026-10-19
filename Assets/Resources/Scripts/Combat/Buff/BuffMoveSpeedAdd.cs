using System;

/// <summary>
/// 移速提升（"速"，BuffConfig 300009）：按 StrengthBuff1 语义 key "moveSpeed" 的百分比提升移动速度，移除时还原。
/// </summary>
public class BuffMoveSpeedAdd : Buff
{
    private float moveSpeedDiff;

    public BuffMoveSpeedAdd(int id, int skillId, Chess caster, Chess target, float lastTime)
     : base(id, skillId, caster, target, lastTime)
    {
    }

    public override void OnAdd(Chess chess, Chess caster)
    {
        base.OnAdd(chess, caster);
        var rate = GetStrBuff("moveSpeed");
        moveSpeedDiff = chess.moveSpeed * rate;
        chess.moveSpeed += moveSpeedDiff;
    }

    public override void OnRemove(Chess chess)
    {
        chess.moveSpeed -= moveSpeedDiff;
        base.OnRemove(chess);
    }
}
