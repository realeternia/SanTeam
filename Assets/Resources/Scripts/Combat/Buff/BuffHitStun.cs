using CommonConfig;
using UnityEngine;

/// <summary>
/// 乱阵·普攻眩晕 Buff：由乱阵(BattleBegin)挂在本侧近战士兵身上。
/// 带此 Buff 的士兵普攻时，按乱阵配置 Strength2 语义 key "stunChance"（比例）概率眩晕目标(乱BuffNoAction)，眩晕时长取乱阵 BuffTime。
/// 眩晕 Buff 短名取乱阵 SkillConfig.BuffId 逗号分隔的第 2 项（如 "威,乱,惑" 中的"乱"）。
/// </summary>
public class BuffHitStun : Buff
{
    public BuffHitStun(int id, int skillId, Chess caster, Chess unit, float lastTime)
        : base(id, skillId, caster, unit, lastTime)
    {
    }

    public override void OnAttack(Chess defender, int damage)
    {
        if (SysRandom.Value >= GetStr2("stunChance"))
            return;
        var buffIds = Skill.GetSkillBuffIds(skillCfg);
        if (buffIds.Count < 2)
        {
            GameLog.Error("BuffHitStun.OnAttack: 乱阵 BuffId 未配置眩晕Buff（需\"威,乱,惑\"）技能id=" + skillCfg.Id);
            return;
        }
        BuffManager.AddBuff(defender, owner, skillCfg.Id, buffIds[1], 2f); // 眩晕时长为固定 2s
    }
}