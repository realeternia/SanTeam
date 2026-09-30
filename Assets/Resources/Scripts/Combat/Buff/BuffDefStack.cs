using System;

/// <summary>
/// 据守（曹仁·天人守城）：临时双防。OnAdd 增加 skillCfg.Strength2 点护甲与魔抗；
/// 状态期间每次受到攻击再追加「初始双防 × skillCfg.Strength3」点，最多 skillCfg.StrengthInt 层；
/// Buff 到期（BuffTime 秒）由 BuffManager 移除时按累计值还原，临时双防全部消失。
/// </summary>
public class BuffDefStack : Buff
{
    private int baseBonus;      // 初始双防点数
    private int perStackBonus;  // 每层追加的双防点数
    private int stack;          // 当前叠层数
    private int armorBonus;     // 已施加的护甲总量（移除时还原）
    private int magicResBonus;  // 已施加的魔抗总量（移除时还原）

    public BuffDefStack(int id, int skillId, Chess caster, Chess unit, float lastTime)
        : base(id, skillId, caster, unit, lastTime)
    {
    }

    public override void OnAdd(Chess chess, Chess caster)
    {
        base.OnAdd(chess, caster);
        baseBonus = (int)skillCfg.Strength2;
        perStackBonus = (int)(baseBonus * skillCfg.Strength3);
        stack = 0;
        AddDefence(chess, baseBonus);
    }

    // 受击叠层：每次被打追加一层双防，达到配置上限后不再增加
    public override void OnAttacked(Chess attacker, int damage)
    {
        if (owner == null || owner.hp <= 0)
            return;
        if (stack >= skillCfg.StrengthInt)
            return;

        stack++;
        AddDefence(owner, perStackBonus);
    }

    private void AddDefence(Chess chess, int add)
    {
        if (add <= 0)
            return;
        chess.armor += add;
        //chess.magicRes += add;
        armorBonus += add;
        //magicResBonus += add;
    }

    public override void OnRemove(Chess chess)
    {
        base.OnRemove(chess);
        chess.armor -= armorBonus;
        //chess.magicRes -= magicResBonus;
        armorBonus = 0;
        //magicResBonus = 0;
    }
}
