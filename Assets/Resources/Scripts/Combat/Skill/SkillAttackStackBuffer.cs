using System;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 兼资（出将入相）：攻击时给自身叠一层「兼资」Buff，持续 BuffTime(4秒) 窗口。
/// 已有兼资则再叠一层并刷新持续窗口；停手4秒后层数随 Buff 到期清零。
/// 对应技能：兼资。每层提升 attack skillCfg.Strength、法术强度 skillCfg.Strength2（见 BuffStackBuf）。
/// </summary>
public class SkillAttackStackBuffer : Skill
{
    public SkillAttackStackBuffer(int id, Chess unit) : base(id, unit)
    {
    }

    public override void OnAttack(Chess defender, int damage)
    {
        if (defender == null || defender.playerId == owner.playerId)
            return;
        if (string.IsNullOrEmpty(skillCfg.BuffId))
            return;

        var buffId = BuffConfig.GetConfigByNameS(skillCfg.BuffId).Id;
        var buff = owner.GetBuff(buffId) as BuffStackBuf;
        if (buff == null)
            BuffManager.AddBuff(owner, owner, id, buffId, skillCfg.BuffTime);
        else
        {
            buff.AddStack();
            buff.SetTime(skillCfg.BuffTime);
        }
    }
}