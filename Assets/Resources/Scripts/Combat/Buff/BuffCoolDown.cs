using System;

public class BuffCoolDown : Buff
{
    private float attackRateDiff;
    public BuffCoolDown(int id, int skillId, Chess caster, Chess target, float lastTime)
     : base(id, skillId, caster, target, lastTime)
    {
    }

    public override void OnAdd(Chess chess, Chess caster)
    {
        base.OnAdd(chess, caster);
        attackRateDiff = chess.attackRate * skillCfg.Strength;
        chess.attackRate += attackRateDiff;
    }

    public override void OnRemove(Chess chess)
    {
        base.OnRemove(chess);
        chess.attackRate -= attackRateDiff;
    }
}
