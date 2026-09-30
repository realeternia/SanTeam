using System;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 曹洪·血战：单体法术伤害，自身生命越低伤害越高（额外伤害 = 已损生命比例 × Strength3 倍基础伤害），并对带护盾目标造成额外破盾伤害（AntiShield 标签直接打血，数值 = 基础伤害 × StrengthInt%）。
/// </summary>
public class SkillAidBloodyWar : Skill
{
    public SkillAidBloodyWar(int id, Chess unit) : base(id, unit)
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

        int damage = GetSkillDamage();
        // 自身已损生命比例越高伤害越高
        float lossRate = 1f - owner.HpRate;
        damage += (int)(damage * skillCfg.Strength3 * lossRate);

        target.OnSkillDamaged(owner, skillId, damage);

        // 对带护盾目标额外造成破盾伤害（绕过护盾直接打血）
        if ((target.GetBuff(CombatConst.ShieldBuffId) as BuffShield) != null)
        {
            var extra = Math.Max(1, (int)(damage * skillCfg.StrengthInt / 100f));
            target.OnSkillDamaged(owner, skillId, extra, false, CombatConst.AntiShieldHurtTag);
        }

        EffectManager.PlaySkillEffect(target, skillCfg.HitEffect);
        return true;
    }
}