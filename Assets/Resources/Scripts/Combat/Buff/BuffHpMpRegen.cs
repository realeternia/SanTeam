using System;

/// <summary>
/// 仁政（刘备·仁德）：提升生命回复 hpRegen 与法力回复 mpRegen（每秒结算），移除时还原。
/// hpRegen 提升 skillCfg.StrengthBuff1 点/秒、mpRegen 提升 skillCfg.StrengthBuff2 点/秒。
/// </summary>
public class BuffHpMpRegen : Buff
{
    private float hpRegenDiff;
    private float mpRegenDiff;

    public BuffHpMpRegen(int id, int skillId, Chess caster, Chess unit, float lastTime)
        : base(id, skillId, caster, unit, lastTime)
    {
    }

    public override void OnAdd(Chess chess, Chess caster)
    {
        base.OnAdd(chess, caster);
        hpRegenDiff = skillCfg.StrengthBuff1;
        mpRegenDiff = skillCfg.StrengthBuff2;
        chess.hpRegen += hpRegenDiff;
        chess.mpRegen += mpRegenDiff;
    }

    public override void OnRemove(Chess chess)
    {
        base.OnRemove(chess);
        chess.hpRegen -= hpRegenDiff;
        chess.mpRegen -= mpRegenDiff;
    }
}
