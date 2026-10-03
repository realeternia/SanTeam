using System;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 庞德·抬棺：怒喝嘲讽自身 Area 范围内敌人攻击自己（把这些敌人的当前目标改为自己），并对这群敌人各造成一次法术伤害。
/// </summary>
public class SkillAidTauntSlam : Skill
{
    public SkillAidTauntSlam(int id, Chess unit) : base(id, unit)
    {
    }

    public override bool CheckAidSkill()
    {
        if (owner.hp <= 0)
            return false;
        if (!CheckBurst(null))
            return false;

        owner.PlayerAnim(skillCfg.Action);

        var enemies = WorldManager.Instance.GetEnemyInRange(owner.transform.position, skillCfg.Area, owner.side);
        foreach (var e in enemies)
        {
            if (e == null || e.hp <= 0)
                continue;
            e.OnSkillDamaged(owner, id, GetSkillDamage());
        }

        // 嘲讽：把范围内敌人的当前目标改到自己身上
        TauntEnemies(skillCfg.Area);

        PlayAreaEffect(owner.transform.position);
        EffectManager.PlaySkillEffect(owner, skillCfg.HitEffect);
        return true;
    }
}