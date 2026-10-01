using System;

/// <summary>
/// 御风疾驰：按 Strength2 提升攻速，并按 Strength3 提升移动速度。
/// 用于鼓舞（小乔）给数名友军的攻速+移速组合 buff。
/// </summary>
public class BuffHasteMoveSpeed : Buff
{
    private float attackSpeedRateDiff;
    private float moveSpeedDiff;

    public BuffHasteMoveSpeed(int id, int skillId, Chess caster, Chess target, float lastTime)
     : base(id, skillId, caster, target, lastTime)
    {
    }

    public override void OnAdd(Chess chess, Chess caster)
    {
        base.OnAdd(chess, caster);
        attackSpeedRateDiff = skillCfg.StrengthBuff1;
        chess.attackSpeedRate += attackSpeedRateDiff;

        moveSpeedDiff = chess.moveSpeed * skillCfg.StrengthBuff2;
        chess.moveSpeed += moveSpeedDiff;
    }

    public override void OnRemove(Chess chess)
    {
        base.OnRemove(chess);
        chess.attackSpeedRate -= attackSpeedRateDiff;
        chess.moveSpeed -= moveSpeedDiff;
    }
}