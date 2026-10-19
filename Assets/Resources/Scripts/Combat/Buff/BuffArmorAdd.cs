/// <summary>
/// 提升护甲 Buff（短名"甲"）：施加时按 StrengthBuff1 语义 key "armor" 恒定提升携带者的护甲（armor），
/// Buff 移除时原样扣回，保持护甲数值前后一致。
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
        diff = (int)GetStrBuff("armor");
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