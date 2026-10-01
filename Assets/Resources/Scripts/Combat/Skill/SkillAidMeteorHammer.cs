using System;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 王双·流星锤：抛物范围 AOE，对目标及范围内最多 TargetCount 名敌人各造成法术伤害（GetArmorDelta=-1 无视护甲），对带护盾敌人额外造成 ×Strength2[0] 破盾伤害。
/// </summary>
public class SkillAidMeteorHammer : Skill
{
    public SkillAidMeteorHammer(int id, Chess unit) : base(id, unit)
    {
    }

    public override float GetArmorDelta(bool isAttackerSide)
    {
        // 无视护甲
        return isAttackerSide ? -1f : 0f;
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
            if (u == null || u.hp <= 0)
                continue;
            int dmg = GetSkillDamage();
            u.OnSkillDamaged(owner, id, dmg);
            // 对带护盾敌人额外造成破盾伤害
            if ((u.GetBuff(CombatConst.ShieldBuffId) as BuffShield) != null)
            {
                var extra = Math.Max(1, (int)(dmg * skillCfg.Strength2[0]));
                u.OnSkillDamaged(owner, id, extra, false, CombatConst.AntiShieldHurtTag);
            }
        }

        PlayAreaEffect(target.transform.position);
        EffectManager.PlaySkillEffect(target, skillCfg.HitEffect);
        return true;
    }
}