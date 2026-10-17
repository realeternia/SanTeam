/// <summary>
/// 复活（BuffConfig 300102，张昭·筑垒施加）：不提供任何属性加成，只把被加持单位标记为"死亡后可原地复活一次"。
/// 死亡时由 Chess.Ondying 复核 reviveArmed/reviveUsed，延迟 ReviveDelay 秒后在原地满血重建（仅士兵，英雄不生效）。
/// </summary>
public class BuffRevive : Buff
{
    /// <summary>死亡后原地复活延迟(秒)</summary>
    public const float ReviveDelay = 3f;

    public BuffRevive(int id, int skillId, Chess caster, Chess target, float lastTime)
     : base(id, skillId, caster, target, lastTime)
    {
    }

    public override void OnAdd(Chess chess, Chess caster)
    {
        base.OnAdd(chess, caster);
        chess.reviveArmed = true;   // 已武装：死亡可原地复活
        chess.reviveUsed = false;   // 每次重新加持，视为未使用过复活
    }

    public override void OnRemove(Chess chess)
    {
        chess.reviveArmed = false;
        base.OnRemove(chess);
    }
}
