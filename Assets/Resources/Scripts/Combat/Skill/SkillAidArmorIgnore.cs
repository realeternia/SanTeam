using System;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 高顺·破甲（武）：破甲弹无视护甲，并对有护盾(吸收型)的目标额外造成破盾伤害。
/// GetArmorDelta 返回 -1 使自身物理攻击完全无视目标护甲；
/// 对有 BuffShield 的目标额外造成基于攻击基准(atk)的破盾伤害，携带 AntiShield 标签绕过护盾直接打血。
/// </summary>
public class SkillAidArmorIgnore : Skill
{
    public SkillAidArmorIgnore(int id, Chess unit) : base(id, unit)
    {
    }

    public override float GetArmorDelta(bool isAttackerSide)
    {
        // 破甲：攻击方物理攻击完全无视目标护甲
        return -1f;
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

        target.OnSkillDamaged(owner, skillId, GetSkillDamage());

        // 对有护盾(吸收型，BuffShield)的目标额外造成基于攻击基准的破盾伤害，绕过护盾直接打血
        if ((target.GetBuff(CombatConst.ShieldBuffId) as BuffShield) != null)
        {
            var extra = Math.Max(1, (int)(owner.GetAttr("atk") * skillCfg.Strength));
            target.OnSkillDamaged(owner, skillId, extra, false, CombatConst.AntiShieldHurtTag);
        }
        return true;
    }
}
