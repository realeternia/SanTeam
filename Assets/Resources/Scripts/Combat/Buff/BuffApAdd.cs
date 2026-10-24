/// <summary>
/// 法强提升 Buff（短名"术"）：施加时按 StrengthBuff1 语义 key "ap" 提升携带者法术强度（ap），
/// Buff 移除时原样扣回，保持法强数值前后一致。用于曹操·护驾等"提升我方法强"的增益。
/// </summary>
public class BuffApAdd : Buff
{
    private int diff;

    public BuffApAdd(int id, int skillId, Chess caster, Chess target, float lastTime)
     : base(id, skillId, caster, target, lastTime)
    {
    }

    public override void OnAdd(Chess chess, Chess caster)
    {
        base.OnAdd(chess, caster);
        diff = (int)GetStrBuff("ap");
        chess.ap += diff;
    }

    public override void OnRemove(Chess chess)
    {
        base.OnRemove(chess);
        chess.ap -= diff;
    }
}
