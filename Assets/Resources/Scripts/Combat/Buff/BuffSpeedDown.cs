using System;

public class BuffSpeedDown : Buff
{
    private float moveSpeedDiff;
    private float attackSpeedRateDiff;
    public BuffSpeedDown(int id, int skillId, Chess caster, Chess target, float lastTime)
     : base(id, skillId, caster, target, lastTime)
    {
    }

    public override void OnAdd(Chess chess, Chess caster)
    {
        base.OnAdd(chess, caster);
        moveSpeedDiff = chess.moveSpeed * skillCfg.Strength;
        chess.moveSpeed -= moveSpeedDiff;

        attackSpeedRateDiff = skillCfg.Strength;
        chess.attackSpeedRate -= attackSpeedRateDiff;
    }

    public override void OnRemove(Chess chess)
    {
        base.OnRemove(chess);
        chess.moveSpeed += moveSpeedDiff;
        chess.attackSpeedRate += attackSpeedRateDiff;
    }
}
