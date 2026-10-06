using CommonConfig;
using UnityEngine;

/// <summary>
/// 司马昭·篡逆（术）：对自身 "area 范围内敌人各造成 /damagestrength 法术伤害，
/// 并使其叛逃（Buff "叛"，时长 bufftime）：不受控制地向初始位置后退、移动速度减半，
/// 令敌方阵线自乱阵脚。
/// </summary>
public class SkillAidUsurpPressure : Skill
{
    public SkillAidUsurpPressure(int id, Chess unit) : base(id, unit)
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

        var units = WorldManager.Instance.GetEnemyInRange(owner.transform.position, skillCfg.Area, owner.side);
        var skillDamage = GetSkillDamage();
        // 施加叛逃（Buff短名由 SkillConfig.BuffId 配置，如"叛"）
        var fleeBuffIds = GetSkillBuffIds();
        foreach (var u in units)
        {
            if (skillDamage > 0)
                u.OnSkillDamaged(owner, skillId, skillDamage);
            foreach (var buffId in fleeBuffIds)
                BuffManager.AddBuff(u, owner, id, buffId, skillCfg.BuffTime);
        }

        PlayAreaEffect(owner.transform.position);
        EffectManager.PlaySkillEffect(target, skillCfg.HitEffect);
        return true;
    }
}