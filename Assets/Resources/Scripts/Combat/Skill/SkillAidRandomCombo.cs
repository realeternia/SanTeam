using System;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 马超·追猎：对单体连续造成多次魔法伤害，次数随机取 [strengthInt(下限), strengthInt(上限) + 1) 由配置控制（看运气）
/// </summary>
public class SkillAidRandomCombo : Skill
{
    public SkillAidRandomCombo(int id, Chess unit) : base(id, unit)
    {
    }

    public override bool CheckAidSkill()
    {
        var target = owner.targetChess;
        if (target == null || target.hp <= 0)
            return false;
        if (!WorldManager.Instance.CheckInRange(owner.transform.position, target.transform.position, skillCfg.Range))
            return false;
        if (!CheckBurst(target))
            return false;

        // 次数下限用 strengthInt？这里沿用召唤字段，下限=SummonCount，上限=StrengthInt（排他）+1 使含上限
        int times = SysRandom.Range(skillCfg.SummonCount, skillCfg.StrengthInt + 1);
        for (int i = 0; i < times; i++)
            target.OnSkillDamaged(owner, id, GetSkillDamage());

        owner.PlayerAnim(skillCfg.Action);
        return true;
    }
}