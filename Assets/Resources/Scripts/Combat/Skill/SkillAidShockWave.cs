using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 许褚·飞斧：向目标位置扔出飞斧，造成范围法术伤害；对拥有吸收盾(BuffShield)的目标造成 2 倍伤害。
/// </summary>
public class SkillAidShockWave : Skill
{
    public SkillAidShockWave(int id, Chess unit) : base(id, unit)
    {
    }

    public override bool CheckAidSkill()
    {
        if (owner.targetChess == null)
            return false;

        if (!WorldManager.Instance.CheckInRange(owner.transform.position, owner.targetChess.transform.position, skillCfg.Range))
            return false;

        if (!CheckBurst(null))
            return false;

        var targetPos = owner.targetChess.transform.position; // 使用目标位置而不是自身位置

        owner.PlayerAnim(skillCfg.Action);
        var damage = GetSkillDamage(); // 固定系数 + 比例系数×关联属性
        WorldManager.Instance.CreateSpellMissile(owner, targetPos, GetSummonTime(), skillCfg.SummonSpeed, skillCfg.Area, skillCfg.Id, damage, skillCfg.HitEffect);

        GameLog.Debug("SkillAidShockWave id=" + id.ToString() + " damage=" + damage.ToString());

        return true;
    }

    /// <summary>
    /// 伤害计算阶段·攻击方：飞斧命中拥有吸收盾(BuffShield)的目标时伤害 ×2（对盾克制），
    /// 只作用于本技能(飞斧)造成的伤害，不影响其他来源。
    /// </summary>
    public override void BeforeCalDamage(Chess target, SkillConfig castSkillCfg, ref int damageBase, ref float damageMulti, ref string effect, string hurtTag, bool isFeedback)
    {
        if (castSkillCfg == null || castSkillCfg.Sname != skillCfg.Sname || target == null)
            return;

        var shield = target.GetBuff(CombatConst.ShieldBuffId) as BuffShield;
        if (shield != null && shield.GetHp() > 0)
            damageMulti *= CombatConst.ShockWaveShieldDamageMulti;
    }
}