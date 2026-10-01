using System;

/// <summary>
/// 护驾（曹操·护驾）：提升受治疗系数 healedRate（0.1=受到的治疗+10%），移除时还原。
/// </summary>
public class BuffHealBoost : Buff
{
    private float healedRateDiff;

    public BuffHealBoost(int id, int skillId, Chess caster, Chess unit, float lastTime)
        : base(id, skillId, caster, unit, lastTime)
    {
    }

    public override void OnAdd(Chess chess, Chess caster)
    {
        base.OnAdd(chess, caster);
        healedRateDiff = skillCfg.StrengthBuff1;
        chess.healedRate += healedRateDiff;
    }

    public override void OnRemove(Chess chess)
    {
        base.OnRemove(chess);
        chess.healedRate -= healedRateDiff;
    }
}
