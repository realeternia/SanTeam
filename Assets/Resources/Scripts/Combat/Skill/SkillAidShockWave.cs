using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 许褚·飞斧：向目标位置扔出飞斧，造成范围法术伤害；对拥有吸收盾(BuffShield)的目标造成 2 倍伤害。
/// 飞斧伤害由飞行中的导弹逐个命中目标结算，故"对盾翻倍"通过 Missile 的每目标伤害倍率回调实现
/// （施法者自身技能无法在 BeforeCalDamage 中修改自己的伤害：伤害计算阶段会跳过当前施放技能自身）。
/// </summary>
public class SkillAidShockWave : Skill
{
    /// <summary>对拥有吸收盾(BuffShield)且未破盾的目标造成的伤害倍率</summary>
    private const float ShieldDamageMulti = 2f;

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
        WorldManager.Instance.CreateSpellMissile(owner, targetPos, GetSummonTime(), skillCfg.SummonSpeed, skillCfg.Area, skillCfg.Id, damage, skillCfg.HitEffect, ShieldTargetDamageMulti);

        GameLog.Debug("SkillAidShockWave id=" + id.ToString() + " damage=" + damage.ToString());

        return true;
    }

    /// <summary>
    /// 每目标伤害倍率：目标携带吸收盾(BuffShield)且护盾未破时，飞斧伤害 ×2（对盾克制），否则原伤害。
    /// </summary>
    private float ShieldTargetDamageMulti(Chess target)
    {
        if (target == null)
            return 1f;
        var shield = target.GetBuff(CombatConst.ShieldBuffId) as BuffShield;
        return shield != null && shield.GetHp() > 0 ? ShieldDamageMulti : 1f;
    }
}