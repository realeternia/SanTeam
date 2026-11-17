/// <summary>
/// 减速 Buff（短名"缓"）：按 StrengthBuff1 语义 key "slow" 降低携带者的移动速度（moveSpeed 百分比）、
/// 按 key "atkSpeed" 降低攻速比例（attackSpeedRate），Buff 移除时原样加回，恢复原有移速与攻速。
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
        moveDiff = chess.moveSpeed * GetStrBuff("slow");
        chess.moveSpeed -= moveDiff;
        chess.attackSpeedRate -= GetStrBuff("atkSpeed");
    }

    public override void OnRemove(Chess chess)
    {
        chess.moveSpeed += moveDiff;
        chess.attackSpeedRate += GetStrBuff("atkSpeed");
        base.OnRemove(chess);
    }
}