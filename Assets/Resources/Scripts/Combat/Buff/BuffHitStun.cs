using CommonConfig;
using UnityEngine;

/// <summary>
/// 乱阵·普攻眩晕 Buff：由乱阵(BattleBegin)挂在本侧近战士兵身上。
/// 带此 Buff 的士兵普攻时，按乱阵配置 StrengthInt(百分数) 概率眩晕目标(乱BuffNoAction)，眩晕时长取乱阵 BuffTime。
/// </summary>
public class BuffHitStun : Buff
{
    public BuffHitStun(int id, int skillId, Chess caster, Chess unit, float lastTime)
        : base(id, skillId, caster, unit, lastTime)
    {
    }

    public override void OnAttack(Chess defender, int damage)
    {
        if (SysRandom.Value >= skillCfg.StrengthInt * 0.01f)
            return;
        var stunBuffCfg = BuffConfig.GetConfigByNameS(SkillSoldierStun.StunBuffNameS);
        if (stunBuffCfg == null)
        {
            GameLog.Error("BuffHitStun.OnAttack: 未找到眩晕Buff：" + SkillSoldierStun.StunBuffNameS);
            return;
        }
        BuffManager.AddBuff(defender, owner, skillCfg.Id, stunBuffCfg.Id, 2f); // 眩晕时长为固定 2s
    }
}