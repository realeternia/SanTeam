using CommonConfig;
using UnityEngine;

/// <summary>
/// 司马昭·篡逆（术）：对自身 "area 范围内敌人各造成 /strength 法术伤害，
/// 并将其定身无法移动（Buff "停"，时长 bufftime），锁住敌方阵线。
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

        var units = WorldManager.Instance.GetUnitsInRange(owner.transform.position, skillCfg.Area, owner.side, true);
        var skillDamage = GetSkillDamage();
        var tingBuffId = BuffConfig.GetConfigByNameS("停").Id;
        foreach (var u in units)
        {
            if (skillDamage > 0)
                u.OnSkillDamaged(owner, skillId, skillDamage);
            BuffManager.AddBuff(u, owner, id, tingBuffId, skillCfg.BuffTime);
        }

        EffectManager.PlaySkillEffect(target, skillCfg.HitEffect);
        return true;
    }
}