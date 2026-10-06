/// <summary>
/// 叠加破甲 Buff（短名"削"，BuffConfig 301010）：每次施加/刷新按 StrengthBuff1[0] 为一层，
/// 每层恒定削减携带者护甲(armor)，层数可叠加（越打护甲越低）；Buff 移除时按总层数加回，保持数值前后一致。
/// 由马超·西凉铁骑连击时反复施加，实现"连击叠加破甲"。
/// </summary>
public class BuffArmorShred : Buff
{
    /// <summary>每层破甲值</summary>
    private int perStack;
    /// <summary>当前已叠加层数</summary>
    private int stacks;

    public BuffArmorShred(int id, int skillId, Chess caster, Chess target, float lastTime)
     : base(id, skillId, caster, target, lastTime)
    {
    }

    public override void OnAdd(Chess chess, Chess caster)
    {
        base.OnAdd(chess, caster);
        perStack = (int)skillCfg.StrengthBuff1[0];
        stacks = 1;
        chess.armor -= perStack;
    }

    // 重复施加同一 Buff 时走 Refresh：叠一层并刷新时长
    public override void Refresh(Chess caster, float lastTime)
    {
        base.Refresh(caster, lastTime);
        if (owner == null)
            return;
        stacks++;
        owner.armor -= perStack;
    }

    public override void OnRemove(Chess chess)
    {
        chess.armor += perStack * stacks;
        base.OnRemove(chess);
    }
}