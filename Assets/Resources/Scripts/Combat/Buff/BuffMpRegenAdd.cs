using System;

/// <summary>
/// 法力回复提升（"蓝"，BuffConfig 300018）：按 StrengthBuff1 提升每秒法力回复（mpRegen），移除时还原。
/// 槽位约定：单独使用读 [0]；与其它数值 Buff 组合时（如刘备·仁德 "血,蓝"：StrengthBuff1=[生命回复, 法力回复]）读 [1]。
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
        var slot = skillCfg.StrengthBuff1.Length > 1 ? 1 : 0;
        diff = skillCfg.StrengthBuff1.Length > slot ? skillCfg.StrengthBuff1[slot] : 0f;
        chess.mpRegen += diff;
    }

    public override void OnRemove(Chess chess)
    {
        chess.mpRegen -= diff;
        base.OnRemove(chess);
    }
}
