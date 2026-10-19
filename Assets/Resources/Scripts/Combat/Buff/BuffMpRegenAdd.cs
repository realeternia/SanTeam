using System;

/// <summary>
/// 法力回复提升（"蓝"，BuffConfig 300018）：按 StrengthBuff1 语义 key "mpRegen" 提升每秒法力回复，移除时还原。
/// </summary>
public class BuffMpRegenAdd : Buff
{
    private float diff;

    public BuffMpRegenAdd(int id, int skillId, Chess caster, Chess target, float lastTime)
     : base(id, skillId, caster, target, lastTime)
    {
    }

    public override void OnAdd(Chess chess, Chess caster)
    {
        base.OnAdd(chess, caster);
        diff = GetStrBuff("mpRegen");
        chess.mpRegen += diff;
    }

    public override void OnRemove(Chess chess)
    {
        chess.mpRegen -= diff;
        base.OnRemove(chess);
    }
}
