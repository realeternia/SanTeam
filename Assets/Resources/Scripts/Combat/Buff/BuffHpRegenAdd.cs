using System;

/// <summary>
/// 生命回复提升（"血"，BuffConfig 300014）：按 StrengthBuff1[0] 提升每秒生命回复（hpRegen），移除时还原。
/// 槽位约定：组合使用时始终读 [0]（如刘备·仁德 "血,蓝"：StrengthBuff1=[生命回复, 法力回复]）。
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
        diff = skillCfg.StrengthBuff1.Length > 0 ? skillCfg.StrengthBuff1[0] : 0f;
        chess.hpRegen += diff;
    }

    public override void OnRemove(Chess chess)
    {
        chess.hpRegen -= diff;
        base.OnRemove(chess);
    }
}
