/// <summary>
/// 减速 Buff：按 Strength2 降低携带者的移动速度（moveSpeed 百分比）与攻速比例（attackSpeedRate），
/// Buff 移除时原样加回，恢复原有移速与攻速。
/// </summary>
public class BuffSlowDown : Buff
{
    private float moveDiff;

    public BuffSlowDown(int id, int skillId, Chess caster, Chess target, float lastTime)
     : base(id, skillId, caster, target, lastTime)
    {
    }

    public override void OnAdd(Chess chess, Chess caster)
    {
        base.OnAdd(chess, caster);
        moveDiff = chess.moveSpeed * skillCfg.Strength2;
        chess.moveSpeed -= moveDiff;
        chess.attackSpeedRate -= skillCfg.Strength2;
    }

    public override void OnRemove(Chess chess)
    {
        chess.moveSpeed += moveDiff;
        chess.attackSpeedRate += skillCfg.Strength2;
        base.OnRemove(chess);
    }
}