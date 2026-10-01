using System;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 庞德·抬棺：怒喝嘲讽自身 Area 范围内敌人攻击自己（挂 BuffTaunt），并对这群敌人各造成一次法术伤害。
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

        var enemies = WorldManager.Instance.GetUnitsInRange(owner.transform.position, skillCfg.Area, owner.side, true);
        foreach (var e in enemies)
        {
            if (e == null || e.hp <= 0)
                continue;
            e.OnSkillDamaged(owner, id, GetSkillDamage());
        }

        // 嘲讽：自身挂嘲讽标记
        BuffManager.AddBuff(owner, owner, id, BuffConfig.GetConfigByNameS("嘲").Id, skillCfg.BuffTime);

        PlayAreaEffect(owner.transform.position);
        EffectManager.PlaySkillEffect(owner, skillCfg.HitEffect);
        return true;
    }
}