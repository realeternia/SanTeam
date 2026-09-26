using System;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 潘璋·偷刃：破甲——自身物理攻击(普攻)完全无视目标护甲（GetArmorDelta=-1），技能造成法术伤害并降低目标护甲（Buff "破"）。
/// </summary>
public class SkillAidStealBlade : Skill
{
    public SkillAidStealBlade(int id, Chess unit) : base(id, unit)
    {
    }

    public override float GetArmorDelta(bool isAttackerSide)
    {
        // 破甲：物理攻击完全无视目标护甲
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

        target.OnSkillDamaged(owner, skillId, GetSkillDamage());

        // 降低目标护甲（"破"）
        BuffManager.AddBuff(target, owner, id, BuffConfig.GetConfigByNameS("破").Id, skillCfg.BuffTime);

        EffectManager.PlaySkillEffect(target, skillCfg.HitEffect);
        return true;
    }
}