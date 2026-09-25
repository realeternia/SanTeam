using System;

/// <summary>
/// 筑垒（张昭·筑垒）：对被加持的士兵永久强化双防（护甲/魔抗，数值只加一次，Buff被移除时还原），
/// 并标记该士兵"筑垒复活"状态——死亡后由 Chess.Ondying 复核并原地复活一次。
/// OnAdd/OnRemove 读取 skillCfg.Strength（护甲）+、Strength2（魔抗）+。
/// </summary>
public class BuffBuildFort : Buff
{
    /// <summary>被加持士兵死亡后原地复活延迟(秒)</summary>
    public const float ReviveDelay = 3f;

    private int armorDiff;
    private int magicResDiff;

    public BuffBuildFort(int id, int skillId, Chess caster, Chess target, float lastTime)
     : base(id, skillId, caster, target, lastTime)
    {
    }

    public override void OnAdd(Chess chess, Chess caster)
    {
        base.OnAdd(chess, caster);
        armorDiff = (int)skillCfg.Strength;
        magicResDiff = (int)skillCfg.Strength2;
        chess.armor += armorDiff;
        chess.magicRes += magicResDiff;
        chess.buildFortArmed = true;      // 已武装筑垒：死亡可原地复活
        chess.buildFortRevived = false;   // 每次筑垒重新加持，视为未使用过复活
    }

    public override void OnRemove(Chess chess)
    {
        chess.armor -= armorDiff;
        chess.magicRes -= magicResDiff;
        chess.buildFortArmed = false;
        base.OnRemove(chess);
    }
}