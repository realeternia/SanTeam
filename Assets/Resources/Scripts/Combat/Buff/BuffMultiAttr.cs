using System;

/// <summary>
/// 名门（袁绍·名门）：祝福一员大将，提升攻击 skillCfg.Strength 点、护甲与魔抗 skillCfg.Strength2 点，移除时还原。
/// </summary>
public class BuffMultiAttr : Buff
{
    private int atkDiff;
    private int armorDiff;
    private int magicResDiff;

    public BuffMultiAttr(int id, int skillId, Chess caster, Chess unit, float lastTime)
        : base(id, skillId, caster, unit, lastTime)
    {
    }

    public override void OnAdd(Chess chess, Chess caster)
    {
        base.OnAdd(chess, caster);
        atkDiff = (int)skillCfg.Strength;
        armorDiff = (int)skillCfg.Strength2;
        magicResDiff = (int)skillCfg.Strength2;
        chess.atk += atkDiff;
        chess.armor += armorDiff;
        chess.magicRes += magicResDiff;
    }

    public override void OnRemove(Chess chess)
    {
        base.OnRemove(chess);
        chess.atk -= atkDiff;
        chess.armor -= armorDiff;
        chess.magicRes -= magicResDiff;
    }
}
