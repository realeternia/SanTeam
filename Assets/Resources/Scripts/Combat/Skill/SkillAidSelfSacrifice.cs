using System;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 黄盖·苦肉：苦肉计——嘲讽自身 Area 范围内敌人攻击自己（挂 BuffTaunt），并为所有友军附加 /strength% 最大生命的减伤盾（"硬"）。
/// </summary>
public class SkillAidSelfSacrifice : Skill
{
    public SkillAidSelfSacrifice(int id, Chess unit) : base(id, unit)
    {
    }

    public override bool CheckAidSkill()
    {
        if (owner.hp <= 0)
            return false;
        if (!CheckBurst(null))
            return false;

        owner.PlayerAnim(skillCfg.Action);

        // 嘲讽：自身挂嘲讽标记
        BuffManager.AddBuff(owner, owner, id, BuffConfig.GetConfigByNameS("嘲").Id, skillCfg.BuffTime);

        // 为所有友军附加减伤盾（"硬"=BuffShieldValue，按最大生命比例，需用护盾型容量收敛，此处给固定减伤盾）
        int shieldHardId = BuffConfig.GetConfigByNameS("硬").Id;
        var allies = WorldManager.Instance.GetUnitsInRange(owner.transform.position, 0f, owner.side, false);
        foreach (var ally in allies)
        {
            if (ally == null || ally.hp <= 0)
                continue;
            BuffManager.AddBuff(ally, owner, id, shieldHardId, skillCfg.BuffTime);
        }

        EffectManager.PlaySkillEffect(owner, skillCfg.HitEffect);
        return true;
    }
}