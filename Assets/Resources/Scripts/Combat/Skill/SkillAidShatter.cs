using CommonConfig;
using UnityEngine;

/// <summary>
/// 张郃·震碎（ScriptName = "SkillAidShatter"）：
/// 锁定 Range 射程内的目标后，以其为中心取 Area 半径内敌人，随机保留至多 TargetCount 名，
/// 对每个目标直接结算单体技能伤害并附加 BuffId 减益，持续 BuffTime 秒。
/// 表现：每个受击目标独立播放 HitEffect（单体特效），不播放范围特效。

public class SkillAidShatter : Skill
{
    public SkillAidShatter(int id, Chess unit) : base(id, unit)
    {
    }

    public override bool CheckAidSkill()
    {
        var target = owner.targetChess;
        if (target == null || target.hp <= 0)
            return false;

        // 射程校验：Range<=0 视为不限射程
        if (skillCfg.Range > 0f && !WorldManager.Instance.CheckInRange(owner.transform.position, target.transform.position, skillCfg.Range))
            return false;
        if (!CheckBurst(target))
            return false;

        // 候选：以锁定目标为中心的 Area 半径内敌人；TargetCount>0 时随机保留 N 名，避免 RandomSelect(0) 清空列表
        var targets = WorldManager.Instance.GetEnemyInRange(target.transform.position, skillCfg.Area, owner.side)
            .FindAll(u => u != null && u.hp > 0);
        if (targets.Count == 0)
            return false;
        if (skillCfg.TargetCount > 0)
            WorldManager.Instance.RandomSelect(targets, skillCfg.TargetCount);
        if (targets.Count == 0)
            return false;

        owner.PlayerAnim(skillCfg.Action);
        PlaySelfEffect();

        var damage = GetSkillDamage();
        var debuffIds = GetSkillBuffIds();
        foreach (var u in targets)
        {
            if (damage > 0)
            {
                u.OnSkillDamaged(owner, id, damage);
                if (!string.IsNullOrEmpty(skillCfg.HitEffect))
                    EffectManager.PlaySkillEffect(u, skillCfg.HitEffect);
            }
            foreach (var debuffId in debuffIds)
                BuffManager.AddBuff(u, owner, id, debuffId, skillCfg.BuffTime);
        }
        return true;
    }
}
