/// <summary>
/// 士兵士气（短名"气"）：仅对非英雄小兵生效，每层提升攻击力与护甲。
/// StackMode=1 共存：每次施加生成独立实例各自加成，实现叠层；持续 999 秒，战斗内近似永久。
/// </summary>
public class BuffMorale : Buff
{
    private int atkDiff;
    private int armorDiff;

    public BuffMorale(int id, int skillId, Chess caster, Chess target, float lastTime)
     : base(id, skillId, caster, target, lastTime)
    {
    }

    public override void OnAdd(Chess chess, Chess caster)
    {
        base.OnAdd(chess, caster);

        // 英雄不吃士兵士气：不加数值，移除时也不会扣回
        if (chess.isHero)
            return;

        atkDiff = CombatConst.SoldierMoraleAtk;
        armorDiff = CombatConst.SoldierMoraleArmor;
        chess.atk += atkDiff;
        chess.armor += armorDiff;
    }

    public override void OnRemove(Chess chess)
    {
        if (atkDiff != 0)
            chess.atk -= atkDiff;
        if (armorDiff != 0)
            chess.armor -= armorDiff;
        base.OnRemove(chess);
    }
}
