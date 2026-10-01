using System;

public class BuffCoolDown : Buff
{
    private float attackSpeedRateDiff;
    public BuffCoolDown(int id, int skillId, Chess caster, Chess target, float lastTime)
     : base(id, skillId, caster, target, lastTime)
    {
    }

    public override void OnAdd(Chess chess, Chess caster)
    {
        base.OnAdd(chess, caster);
        attackSpeedRateDiff = skillCfg.StrengthBuff1[0];
        chess.attackSpeedRate += attackSpeedRateDiff;
    }

    public override void OnRemove(Chess chess)
    {
        base.OnRemove(chess);
        chess.attackSpeedRate -= attackSpeedRateDiff;
    }
}
