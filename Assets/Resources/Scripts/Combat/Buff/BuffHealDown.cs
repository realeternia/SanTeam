/// <summary>
/// 降低治疗 Buff：施加时按（施法技能 Strength2 的末位非零值）/100 提高携带者的受治疗系数 healedRate 数值上限（等价于削减其受到的治疗量），
/// Buff 移除时反向扣回，恢复原有受治疗效果。
/// 说明：Strength2 为压缩数组（只存非零值），带减疗的"疫"技能其减疗值恒为末位槽（暗计/背水/律令仅此一值，鸩酒为 {中毒,减疗}），故取末位。
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
        float healDownRate = 0f;
        for (int i = skillCfg.Strength2.Length - 1; i >= 0; i--)
        {
            if (skillCfg.Strength2[i] != 0f)
            {
                healDownRate = skillCfg.Strength2[i];
                break;
            }
        }
        diff = (int)healDownRate / 100f;
        chess.healedRate += diff;
    }

    public override void OnRemove(Chess chess)
    {
        chess.healedRate -= diff;
        base.OnRemove(chess);
    }
}