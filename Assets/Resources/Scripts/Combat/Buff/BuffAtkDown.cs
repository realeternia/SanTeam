/// <summary>
/// 攻击降低 Buff：施加时按 Strength2 恒定削减携带者的攻击力（atk），
/// Buff 移除时原样加回，保持攻击力数值前后一致。
/// </summary>
public class BuffAtkDown : Buff
{
    private int diff;

    public BuffAtkDown(int id, int skillId, Chess caster, Chess target, float lastTime)
     : base(id, skillId, caster, target, lastTime)
    {
    }

    public override void OnAdd(Chess chess, Chess caster)
    {
        base.OnAdd(chess, caster);
        diff = (int)skillCfg.Strength2;
        chess.atk -= diff;
    }

    public override void OnRemove(Chess chess)
    {
        chess.atk += diff;
        base.OnRemove(chess);
    }
}