using System;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 通用"目标中心"范围 AoE（张郃·震碎 / 司马昭·篡逆 共用）：锁定目标在 skillCfg.Range 射程内时，
/// 以目标位置为中心，对 skillCfg.Area 范围内敌人（TargetCount&gt;0 时至多随机 N 名，否则范围内全部）
/// 造成 GetSkillDamage() 技能伤害，并给每个目标附加 SkillConfig.BuffId 指定的减益 buff（时长 BuffTime）。
/// 范围特效播在目标位置、命中特效播在每个受击目标身上。
/// </summary>
public class SkillAidQuake : Skill
{
    public SkillAidQuake(int id, Chess unit) : base(id, unit)
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

        owner.PlayerAnim(skillCfg.Action);
        PlaySelfEffect();
        PlayAreaEffect(target.transform.position);

        var units = WorldManager.Instance.GetEnemyInRange(target.transform.position, skillCfg.Area, owner.side);
        // 命中数上限：TargetCount>0 时随机保留 N 名（同落雷/火海）；<=0 表示不限，避免 RandomSelect(0) 清空列表
        if (skillCfg.TargetCount > 0)
            WorldManager.Instance.RandomSelect(units, skillCfg.TargetCount);

        // 减益 buff 由 SkillConfig.BuffId 短名指定（如"缓"=减速、"叛"=叛逃），不在战斗代码硬编码 BuffId
        var debuffId = 0;
        if (!string.IsNullOrEmpty(skillCfg.BuffId))
        {
            var debuffCfg = BuffConfig.GetConfigByNameS(skillCfg.BuffId);
            if (debuffCfg != null)
                debuffId = debuffCfg.Id;
            else
                GameLog.Error($"SkillAidQuake: 未找到减益Buff短名：{skillCfg.BuffId} 技能id={id}");
        }

        var skillDamage = GetSkillDamage();
        foreach (var u in units)
        {
            if (skillDamage > 0)
            {
                u.OnSkillDamaged(owner, skillId, skillDamage);
                if (!string.IsNullOrEmpty(skillCfg.HitEffect))
                    EffectManager.PlaySkillEffect(u, skillCfg.HitEffect);
            }
            if (debuffId > 0)
                BuffManager.AddBuff(u, owner, id, debuffId, skillCfg.BuffTime);
        }
        return true;
    }
}
