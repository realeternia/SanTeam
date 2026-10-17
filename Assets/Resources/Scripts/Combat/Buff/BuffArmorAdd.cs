/// <summary>
/// 提升护甲 Buff：施加时按 StrengthBuff1 恒定提升携带者的护甲（armor），
/// Buff 移除时原样扣回，保持护甲数值前后一致。
/// 槽位约定：单独使用（以及"盾,甲"这类只有本值一段的组合）读 [0]；
/// 与其它数值 Buff 组合时（如袁绍·名门 "攻,甲,抗"：StrengthBuff1=[攻击, 护甲, 魔抗]）读 [1]。
/// </summary>
public class BuffArmorAdd : Buff
{
    private int diff;

    public BuffArmorAdd(int id, int skillId, Chess caster, Chess target, float lastTime)
     : base(id, skillId, caster, target, lastTime)
    {
    }

    public override void OnAdd(Chess chess, Chess caster)
    {
        base.OnAdd(chess, caster);
        var slot = skillCfg.StrengthBuff1.Length > 1 ? 1 : 0;
        diff = skillCfg.StrengthBuff1.Length > slot ? (int)skillCfg.StrengthBuff1[slot] : 0;
        chess.armor += diff;
    }

    public override void OnRemove(Chess chess)
    {
        chess.armor -= diff;
        base.OnRemove(chess);
    }

    /// <summary>
    /// 动态设置本 Buff 提供的护甲值（如刘禅·安乐把溢出治疗量转为护甲）：先扣回旧值再加新值，Buff 结束仍按最终值扣回。
    /// </summary>
    public void SetArmor(int value)
    {
        if (owner == null)
        {
            GameLog.Warn("BuffArmorAdd.SetArmor 携带者为空，跳过设置");
            return;
        }
        owner.armor += value - diff;
        diff = value;
    }
}