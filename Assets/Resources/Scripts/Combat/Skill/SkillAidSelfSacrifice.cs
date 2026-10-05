using System;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 黄盖·苦肉：苦肉计——嘲讽自身 Area 范围内敌人攻击自己（把这些敌人的当前目标改为自己），
/// 并为自身附加 /strengthbuff1-1% 最大生命的减伤盾（"硬"）。
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

        // 嘲讽：把范围内敌人的当前目标改到自己身上
        TauntEnemies(skillCfg.Area);

        // 为自身附加减伤盾（"硬"=BuffShieldValue，按最大生命比例）
        int shieldHardId = BuffConfig.GetConfigByNameS("硬").Id;
        BuffManager.AddBuff(owner, owner, id, shieldHardId, skillCfg.BuffTime);

        EffectManager.PlaySkillEffect(owner, skillCfg.HitEffect);
        return true;
    }
}