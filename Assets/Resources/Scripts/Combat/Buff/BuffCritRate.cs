/// <summary>
/// 暴击率提升 Buff：施加时按 StrengthBuff1[0] 提升携带者的暴击率（critRate），
/// Buff 移除时原样扣回，保持暴击率数值前后一致。
/// 数值口径与面板 critRate 一致（0.3 = +30% 暴击率）。
/// </summary>
public class BuffCritRate : Buff
{
    private float diff;

    public BuffCritRate(int id, int skillId, Chess caster, Chess target, float lastTime)
     : base(id, skillId, caster, target, lastTime)
    {
    }

    public override void OnAdd(Chess chess, Chess caster)
    {
        base.OnAdd(chess, caster);
        diff = skillCfg.StrengthBuff1.Length > 0 ? skillCfg.StrengthBuff1[0] : 0f;
        chess.critRate += diff;
    }

    public override void OnRemove(Chess chess)
    {
        chess.critRate -= diff;
        base.OnRemove(chess);
    }
}
