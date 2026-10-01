using CommonConfig;
using UnityEngine;

/// <summary>
/// 陈宫·暗计（术）：对 /area 范围内敌人各造成 /damagestrength 法术伤害，
/// 并降低其受到的治疗（Buff "疫"，时长 bufftime）。
/// </summary>
public class SkillAidQuietPlot : Skill
{
    public SkillAidQuietPlot(int id, Chess unit) : base(id, unit)
    {
    }

    public override bool CheckAidSkill()
    {
        var target = owner.targetChess;
        if (target == null || target.hp <= 0)
            return false;
        if (!CheckBurst(target))
            return false;

        owner.PlayerAnim(skillCfg.Action);

        var units = WorldManager.Instance.GetUnitsInRange(owner.transform.position, skillCfg.Area, owner.side, true);
        var skillDamage = GetSkillDamage();
        var yiBuffId = BuffConfig.GetConfigByNameS("疫").Id;
        foreach (var u in units)
        {
            if (skillDamage > 0)
                u.OnSkillDamaged(owner, skillId, skillDamage);
            BuffManager.AddBuff(u, owner, id, yiBuffId, skillCfg.BuffTime);
        }

        PlayAreaEffect(owner.transform.position);
        EffectManager.PlaySkillEffect(target, skillCfg.HitEffect);
        return true;
    }
}