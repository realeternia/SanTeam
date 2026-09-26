using CommonConfig;
using UnityEngine;

/// <summary>
/// 荀攸·连环（术）：对目标连续弹出最多 targetcount 段法术伤害（每段 /strength），
/// 并减速（Buff "缓"，时长 bufftime，减速幅度取 /strength2）牵制敌人。
/// </summary>
public class SkillAidSweeping : Skill
{
    public SkillAidSweeping(int id, Chess unit) : base(id, unit)
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

        var dmg = GetSkillDamage();
        for (int i = 0; i < skillCfg.TargetCount; i++)
        {
            if (target.hp <= 0)
                break;
            if (dmg > 0)
                target.OnSkillDamaged(owner, skillId, dmg);
        }

        BuffManager.AddBuff(target, owner, id, BuffConfig.GetConfigByNameS("缓").Id, skillCfg.BuffTime);
        EffectManager.PlaySkillEffect(target, skillCfg.HitEffect);
        return true;
    }
}