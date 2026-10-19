using System;

/// <summary>
/// 生命回复提升（"血"，BuffConfig 300014）：按 StrengthBuff1 语义 key "hpRegen" 提升每秒生命回复，移除时还原。
/// </summary>
public class BuffHpRegenAdd : Buff
{
    private float diff;

    public BuffHpRegenAdd(int id, int skillId, Chess caster, Chess target, float lastTime)
     : base(id, skillId, caster, target, lastTime)
    {
    }

    public override void OnAdd(Chess chess, Chess caster)
    {
        base.OnAdd(chess, caster);
        diff = GetStrBuff("hpRegen");
        chess.hpRegen += diff;
    }

    public override void OnRemove(Chess chess)
    {
        chess.hpRegen -= diff;
        base.OnRemove(chess);
    }
}
