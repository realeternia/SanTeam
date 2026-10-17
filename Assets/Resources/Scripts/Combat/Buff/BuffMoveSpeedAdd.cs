using System;

/// <summary>
/// 移速提升（"速"，BuffConfig 300009）：按 StrengthBuff1 的百分比提升移动速度，移除时还原。
/// 槽位约定：单独使用读 [0]；与其它数值 Buff 组合时（如小乔·鼓舞 "快,速"：StrengthBuff1=[攻速, 移速]）读 [1]。
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
        var slot = skillCfg.StrengthBuff1.Length > 1 ? 1 : 0;
        var rate = skillCfg.StrengthBuff1.Length > slot ? skillCfg.StrengthBuff1[slot] : 0f;
        moveSpeedDiff = chess.moveSpeed * rate;
        chess.moveSpeed += moveSpeedDiff;
    }

    public override void OnRemove(Chess chess)
    {
        chess.moveSpeed -= moveSpeedDiff;
        base.OnRemove(chess);
    }
}
