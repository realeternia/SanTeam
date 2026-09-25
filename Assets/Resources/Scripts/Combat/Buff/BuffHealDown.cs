/// <summary>
/// 降低治疗 Buff：施加时按 StrengthInt/100 提高携带者的受治疗系数 healedRate 数值上限（等价于削减其受到的治疗量），
/// Buff 移除时反向扣回，恢复原有受治疗效果。
/// </summary>
public class BuffHealDown : Buff
{
    private float diff;

    public BuffHealDown(int id, int skillId, Chess caster, Chess target, float lastTime)
     : base(id, skillId, caster, target, lastTime)
    {
    }

    public override void OnAdd(Chess chess, Chess caster)
    {
        base.OnAdd(chess, caster);
        diff = skillCfg.StrengthInt / 100f;
        chess.healedRate += diff;
    }

    public override void OnRemove(Chess chess)
    {
        chess.healedRate -= diff;
        base.OnRemove(chess);
    }
}