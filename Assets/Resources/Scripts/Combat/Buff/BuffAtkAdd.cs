using System;

/// <summary>
/// 攻击力提升：按技能 Strength 数值提升自身攻击力（atk += Strength），移除时还原。
/// 用于洛神（甄宓）给友军加攻击力。
/// </summary>
public class BuffAtkAdd : Buff
{
    private int atkDiff;

    public BuffAtkAdd(int id, int skillId, Chess caster, Chess target, float lastTime)
     : base(id, skillId, caster, target, lastTime)
    {
    }

    public override void OnAdd(Chess chess, Chess caster)
    {
        base.OnAdd(chess, caster);
        atkDiff = (int)skillCfg.Strength;
        chess.atk += atkDiff;
    }

    public override void OnRemove(Chess chess)
    {
        base.OnRemove(chess);
        chess.atk -= atkDiff;
    }
}