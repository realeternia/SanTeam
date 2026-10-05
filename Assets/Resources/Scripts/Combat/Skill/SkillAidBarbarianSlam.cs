using System;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 曹洪·裂盾：对目标造成法术伤害并移除其吸收型护盾（对带 BuffShield 的目标触发移除）。
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

        target.OnSkillDamaged(owner, id, GetSkillDamage());
        // 移除吸收型护盾
        var shield = target.GetBuff(CombatConst.ShieldBuffId) as BuffShield;
        if (shield != null)
        {
            shield.SetHp(0);
            BuffManager.RemoveBuff(target, CombatConst.ShieldBuffId);
        }

        EffectManager.PlaySkillEffect(target, skillCfg.HitEffect);
        return true;
    }
}