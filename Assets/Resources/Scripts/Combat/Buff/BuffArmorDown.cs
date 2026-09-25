/// <summary>
/// 降低护甲 Buff：施加时按 Strength2 恒定削减携带者的护甲（armor），
/// Buff 移除时原样加回，保持护甲数值前后一致。
/// </summary>
public class BuffArmorDown : Buff
{
    private int diff;

    public BuffArmorDown(int id, int skillId, Chess caster, Chess target, float lastTime)
     : base(id, skillId, caster, target, lastTime)
    {
    }

    public override void OnAdd(Chess chess, Chess caster)
    {
        base.OnAdd(chess, caster);
        diff = (int)skillCfg.Strength2;
        chess.armor -= diff;
    }

    public override void OnRemove(Chess chess)
    {
        chess.armor += diff;
        base.OnRemove(chess);
    }
}