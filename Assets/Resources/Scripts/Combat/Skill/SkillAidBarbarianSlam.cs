using System;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 曹洪·裂盾：对目标造成法术伤害；若目标带有吸收型护盾(BuffShield)，本次伤害整体提升至 Strength2[0] 倍。
/// </summary>
public class SkillAidBarbarianSlam : Skill
{
    public SkillAidBarbarianSlam(int id, Chess unit) : base(id, unit)
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

        var damage = GetSkillDamage();
        // 目标带吸收型护盾时，本次伤害整体乘以 Strength2[0]（配置缺失时按 1 倍，不放大）
        if (skillCfg.Strength2.Length > 0 && target.GetBuff(CombatConst.ShieldBuffId) is BuffShield)
            damage = Math.Max(1, (int)(damage * skillCfg.Strength2[0]));

        target.OnSkillDamaged(owner, id, damage);

        EffectManager.PlaySkillEffect(target, skillCfg.HitEffect);
        return true;
    }
}