using System;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 赵云·七进七出：对目标及范围内最多 TargetCount 名敌人各造成一次法术伤害，并为自己附加 Strength2[0]×(100+法强)/100 的护盾（随法强成长）。
/// </summary>
public class SkillAidSevenCharge : Skill
{
    public SkillAidSevenCharge(int id, Chess unit) : base(id, unit)
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

        var list = WorldManager.Instance.GetUnitsInRange(target.transform.position, skillCfg.Area, owner.side, true);
        WorldManager.Instance.RandomSelect(list, skillCfg.TargetCount);
        foreach (var u in list)
        {
            if (u != null && u.hp > 0)
                u.OnSkillDamaged(owner, id, GetSkillDamage());
        }

        // 冲杀后给自己套盾
        int shieldId = BuffConfig.GetConfigByNameS("盾").Id;
        BuffManager.AddBuff(owner, owner, id, shieldId, skillCfg.BuffTime);
        var sh = owner.GetBuff(shieldId) as BuffShield;
        if (sh != null)
            sh.SetHp(GetSkillShield(0));

        PlayAreaEffect(target.transform.position);
        EffectManager.PlaySkillEffect(owner, skillCfg.HitEffect);
        return true;
    }
}